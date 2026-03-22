#:sdk Microsoft.NET.Sdk.Web
#:package WolverineFx@4.2.0
#:package WolverineFx.Marten@4.2.0
#:package WolverineFx.RDBMS@4.2.0
#:package WolverineFx.Postgresql@4.2.0
#:package WolverineFx.FluentValidation@4.2.0
#:package WolverineFx.Http@4.2.0
#:package WolverineFx.RabbitMQ@4.2.0
#:package WolverineFx.AzureServiceBus@4.2.0
#:package WolverineFx.Http.FluentValidation@4.2.0
#:package WolverineFx.Http.Marten@4.2.0
#:package WolverineFx.AmazonSqs@4.2.0
#:package WolverineFx.EntityFrameworkCore@4.2.0
#:package WolverineFx.SqlServer@4.2.0
#:package WolverineFx.Kafka@4.2.0
#:package WolverineFx.MemoryPack@4.2.0
#:package WolverineFx.MessagePack@4.2.0
#:package WolverineFx.MQTT@4.2.0
#:package WolverineFx.Pubsub@4.2.0
#:package WolverineFx.Pulsar@4.2.0
#:package WolverineFx.RavenDb@4.2.0
#:package Microsoft.CodeAnalysis.Common@4.14.0
#:package Microsoft.CodeAnalysis.Workspaces.Common@4.14.0
#:package Swashbuckle.AspNetCore@9.0.3
#:package Swashbuckle.AspNetCore.Swagger@9.0.3
#:package Swashbuckle.AspNetCore.SwaggerGen@9.0.3
#:package Swashbuckle.AspNetCore.SwaggerUI@9.0.3
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property GenerateJsonSourceGeneration=true

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Contracts.Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
// using Bff.Api;                     // 项目命名空间
using Contracts.Messages.SagaState;
using Microsoft.AspNetCore.Mvc;
using Contracts.Messages;
using Contracts.Messages.SagaState;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

// 统一使用 InMemory transport（开发环境）
// 生产环境只需改成 RabbitMQ、Kafka、Azure Service Bus
builder.Host.UseWolverine(opts =>
{
    / 1️⃣ 配置业务 DbContext（包括 Saga 表）
    opts.Services.AddDbContext<AppDbContext>(c =>
        c.UseNpgsql(builder.Configuration.GetConnectionString("AppDb")));

    // 2️⃣ 启用 Wolverine 的 EF Core 持久化 + Outbox
    opts.UseEntityFrameworkCoreTransactions();   // 让 Wolverine 与 EF Core 同事务
    // opts.UseNewtonsoftJson();                  // 序列化（或 System.Text.Json）
    opts.Persistence.UseEntityFrameworkCore<AppDbContext>(op =>
    {
        // 告诉 Wolverine 哪些 DbSet 用来存 Saga
        // (AppDbContext 中定义 public DbSet<BindDeviceSagaState> BindDeviceSagas { get; set; })
        op.Sagas.Add<BindDeviceSagaState>();
    });

    // 3️⃣ 开启 Outbox（本地事务 + 可靠投递）
    // 开启 Outbox 以实现 BFF 本地事务 + 下游命令原子提交
    // 并在 BindDeviceCommand、UnbindDeviceCommand 等调用前后使用同一 DbContext/IDbConnection，确保业务 DB 与 Outbox DB 同时提交.
    opts.EnableOutbox();

    // 4️⃣ 设置重试/超时策略（防止死循环）
    opts.Policies.OnException<Exception>(ex => ex
        .Retry(3)                // 最多 3 次重试
        .Wait(250)               // 250 ms 递增等待
        .Timeout(5_000));        // 单次调用不超过 5 秒

    // 5️⃣ 日志、诊断
    opts.EnableDiagnostics();

    // opts.UseMarten(...).ConfigureMartenSagaPersistence();
    // 启用 Saga 存储（默认 InMemory，若想持久化可换成 EF/Marten）
    opts.EnableSagas(saga =>
    {
        saga.Persistently();               // 把 Saga 状态写入持久化存储（可选）
    });

    // 为调试打开日志
    opts.Services.AddLogging(logging => logging.AddConsole());

    //  Grafana/Prometheus、Jaeger 集成，监控每一步延迟、Saga 成功率.
    opts.EnableOpenTelemetry();

    // 把 protobuf‑net 设为默认序列化方式，UseProtobufNet()会自动为所有实现了 ProtoContract（即 pbnet 生成的类）注册序列化器.发送方和接收方只要引用同一套 Contracts NuGet，消息即可无缝兼容
    // opts.UseProtobufNet();

    // -------------------------------------------------
    // 3.3 MQTT 传输层
    // -------------------------------------------------
    // ① 直接映射（最简单）：所有 UserDeviceSaved 自动变成
    //     MQTT 主题   "devices/{UserId}/{DeviceId}/saved"
    opts.Transports.Mqtt(m =>
    {
        // 连接到本地或云端的 MQTT Broker
        m.Hostname = builder.Configuration["Mqtt:Host"];   // 例如 "broker.hivemq.com"
        m.Port     = int.Parse(builder.Configuration["Mqtt:Port"] ?? "1883");
        m.Username = builder.Configuration["Mqtt:User"];    // 若有
        m.Password = builder.Configuration["Mqtt:Pwd"];     // 若有

        // ④ QoS 与 Retain（可根据业务需求调节）
        m.DefaultQualityOfServiceLevel = QualityOfServiceLevel.AtLeastOnce; // QoS 1
        m.RetainMessages               = false;

        // ⑤ “自动转发”规则——把任意已发布的 UserDeviceSaved 推送到 MQTT
        //    注意：这里的规则是针对 **本地总线**（InMemory/Db）上的消息.
        //    当业务服务调用 `bus.PublishAsync(new UserDeviceSaved(...))`，
        //    Wolverine 会在内部先保存到 Outbox，然后依据下面的 Forward
        //    把消息发送到 MQTT.
        m.Forward<UserDeviceSaved>(msg =>
        {
            // 生成主题：devices/{UserId}/{DeviceId}/saved
            var topic = $"devices/{msg.UserId}/{msg.DeviceId}/saved";

            // 你可以在这里自定义 payload（默认是 JSON 序列化 whole message）
            // return new MqttPublication(topic, msg, qos: QualityOfServiceLevel.AtLeastOnce);
            return new MqttPublication(topic, msg);
        });

        // ⑥ 订阅（如果 BFF 也想从外部 MQTT 接收消息，使用下面的 Subscribe）
        //    这里演示 BFF **只发布**；如果要接收，可写类似：
        // m.Subscribe<DeviceCommand>(topic: "devices/+/command", handler: ctx => ...);

        // 订阅外部主题：“devices/+/status”
        // 把接收到的 MQTT 消息映射为内部消息类型 DeviceStatusUpdate
        m.Subscribe<DeviceStatusUpdate>("devices/+/status", (topic, payload) =>
        {
            // 解析 topic 中的变量
            // topic example: devices/12345/status   → deviceId = 12345
            var parts = topic.Split('/');
            var deviceId = Guid.Parse(parts[1]);

            // payload（JSON） → POCO
            var update = JsonSerializer.Deserialize<DeviceStatusUpdate>(payload)!;
            update.DeviceId = deviceId;          // 把 topic 里的 ID 塞进去
            return update;                       // 返回的对象会被直接投递到内部总线
        });
    });

    // -------------------------------------------------
    // 3.4 (可选) 额外的内部传输：InMemory（同进程）或 RabbitMQ、Kafka 等
    // 如果所有微服务都跑在同一个宿主进程，直接 InMemory 即可：
    opts.Transports.InMemory();

    // -------------------------------------------------
    // 3.5 注册所有消息处理器
    opts.Services.AddScoped<UserDeviceSavedNotifier>();

    // -------------------------------------------------
    // 3.6 开启诊断（开发调试时打开，生产可关闭或写入日志系统）
    if (builder.Environment.IsDevelopment())
    {
        opts.EnableDiagnostics();
        opts.Services.AddLogging(l => l.SetMinimumLevel(LogLevel.Information));
    }
});

builder.Services.AddControllers();

// 注入 IMessageBus（Wolverine 已经自动注册）
builder.Services.AddSingleton<ISagaFactory, SagaFactory>(); // 可选：自定义工厂

var app = builder.Build();

app.MapControllers();
// SignalR endpoint
app.MapHub<DeviceHub>("/deviceHub");

app.Run();

// BFF 同时是 Saga 发起者（持有 StartBindDeviceSaga），不要再单独订阅 UserDeviceSaved，否则会出现 两次完成（一次在 Saga、一次在 BFF 的普通 handler）导致冲突.只保留 Saga 中的 Handle(UserDeviceSaved) 即可.
// BFF 只做 通知（不参与 Saga），可以另外写一个普通的 MessageHandler
// builder.Services.AddSignalR(); 
public class UserDeviceSavedNotifier
{
    private readonly ILogger<UserDeviceSavedNotifier> _log;
    private readonly IHubContext<DeviceHub> _hub;   // SignalR hub（可选）

    public UserDeviceSavedNotifier(
        ILogger<UserDeviceSavedNotifier> log,
        IHubContext<DeviceHub> hub)
    {
        _log = log;
        _hub = hub;
    }

    // 这是普通的 Wolverine 消息处理器，会在内部总线（InMemory/DB）上被调用
    public async Task Handle(UserDeviceSaved @event, IMessageBus bus)
    {
        _log.LogInformation(
            "🔔 Device {DeviceId} 已绑定到用户 {UserId}",
            @event.DeviceId, @event.UserId);

        // ---- SignalR（WebSocket）推送给前端 ----------
        if (_hub != null)
        {
            await _hub.Clients.User(@event.UserId.ToString())
                .SendAsync("DeviceBound", new
                {
                    DeviceId = @event.DeviceId,
                    SavedAt  = @event.SavedAt
                });
        }

        // ---- MQTT 推送给外部订阅者 ----------
        // 这里我们不直接使用 MQTT 客户端，而是让 Wolverine 把消息 *再* 发布到
        // MQTT 传输层.只要在下面的配置里声明 “将此消息映射到 MQTT 主题”，
        // 发送到 bus 的同一条消息会自动被转发.
        await bus.PublishAsync(@event);   // 已经在这里调用，后面会被 MQTT 发送
    }
}

public class DeviceStatusHandler
{
    private readonly ILogger<DeviceStatusHandler> _log;

    public DeviceStatusHandler(ILogger<DeviceStatusHandler> log) => _log = log;

    public Task Handle(DeviceStatusUpdate update)
    {
        _log.LogInformation("🛰️ Device {DeviceId} 状态: {Status}", update.DeviceId, update.Status);
        // 业务处理、写库、推送 SignalR 等…
        return Task.CompletedTask;
    }
}


namespace Bff.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserDeviceController : ControllerBase
    {
        private readonly IMessageBus _bus;
        private readonly ILogger<UserDeviceController> _log;

        public UserDeviceController(IMessageBus bus, ILogger<UserDeviceController> log)
        {
            _bus = bus;
            _log = log;
        }

        /// <summary>前端一次请求完成「用户‑设备绑定」的完整业务流程</summary>
        [HttpPost("bind")]
        public async Task<IActionResult> Bind([FromBody] BindRequest req)
        {
            // 1️⃣ 创建 Saga（如果已经存在相同 CorrelationId，则继续）
            var sagaId = Guid.NewGuid(); // 业务上可以让前端生成或 BFF 统一生成
            var start = new StartBindDeviceSaga(sagaId, req.DeviceId, req.UserId);
            await _bus.PublishAsync(start);               // 触发 Saga 开始

            // 2️⃣ 这里直接返回 202（Accepted），后端通过事件 / SSE / WebSocket 把进度推给前端
            //    如果想同步阻塞，可改为 await _bus.InvokeAsync<BindDeviceSagaResult>(new GetSagaResult(sagaId));
            // Saga 可能涉及 多个异步子事务（检查设备、检查用户、绑定、保存关联），在真实生产环境里往往需要 几百毫秒到几秒；返回 202 + 状态轮询 或 WebSocket 推送 更友好.
            // 如果业务对时效要求极高，可以把 await _bus.InvokeAsync<BindDeviceSagaResult> 包装成 同步 调用（内部仍走 Saga）
            return Accepted(new { CorrelationId = sagaId });
        }

        /// <summary>查询 Saga 执行结果（轮询或 WebSocket 可用）</summary>
        [HttpGet("status/{correlationId:guid}")]
        public async Task<IActionResult> Status(Guid correlationId)
        {
            var state = await _bus.InvokeAsync<BindDeviceSagaState?>(new GetSagaState(correlationId));
            if (state == null) return NotFound();

            return Ok(new
            {
                state.Id,
                state.CurrentStep,
                state.FailureReason,
                Completed = state.CurrentStep == BindDeviceStep.Completed,
                Failed = state.CurrentStep == BindDeviceStep.Failed
            });
        }
    }

    // 请求体
    public record BindRequest(Guid DeviceId, Guid UserId);
}

// ① 幂等性	所有 Command / Event 必须带 全局唯一 CorrelationId（也可以是 SagaId + StepId）<br>下游服务在处理前先 SELECT … WHERE CorrelationId = @id，若已处理直接返回成功.<br>在数据库层面可以建立唯一键 (DeviceId, UserId, CorrelationId).
// ② 补偿不再触发业务	给 业务事件 加上 DomainEvent.Source = "Business"，而 补偿事件 加上 Source = "Compensation"，在业务侧的 Handler 中 过滤 Source == "Compensation"，这样补偿产生的 DeviceBoundToUser 不会再次走绑定流程.
// ③ 限制补偿的重试次数	在 Wolverine 中使用 opts.Policies.OnException<Exception>(ex => ex.Retry(3).Wait(200))，或在 Saga 状态里记录 CompensationAttempts 并在超过阈值后 标记为不可恢复、发送告警.
// ④ 超时 & 回退	为每一步 设置明确的 Timeout（如 5s），若超时直接进入 Failed 并走补偿.补偿也要有 单独 Timeout，超时后记录 CompensationTimeout 并发送 告警事件（如 CompensationFailedEvent).
// ⑤ 补偿的幂等层	对每个补偿操作（UnbindDeviceCommand、CompensateUserDevice）同样要求 Idempotent，并在下游服务用 补偿日志表（CompensationLog）记录已执行的补偿 CorrelationId + Step. 再次收到相同补偿时直接返回成功.
// ⑥ 状态机分离	业务状态（BusinessStep） 与 补偿状态（CompensatingStep） 使用两个独立的枚举，Saga 只在 Failed → Compensating → Compensated 之间切换，避免在 Compensating 阶段仍然执行业务分支.
// BindDeviceSaga.cs
namespace Bff.Api.Services
{
    using Contracts.Messages;
    using Contracts.Messages.SagaState;
    using Wolverine;
    using Wolverine.Attributes;
    using Wolverine.Persistence.Sagas;

    /// <summary>
    ///   Saga 用来把「绑定」拆成多个步骤，每一步都是一个 Command/Query，
    ///   失败时执行对应的补偿（Compensate）
    /// </summary>
    public class BindDeviceSaga 
    // 声明 Saga 持久化的状态类型
    : Saga<BindDeviceSagaState>          // 自动实现 IStatefulSaga<BindDeviceSagaState>
    {
        // ---------- 1️⃣ 启动 ----------
        // 外部 BFF Controller 发起的启动消息
        public async Task Handle(StartBindDeviceSaga start, IMessageBus bus)
        {
            // 初始化状态（如果是新 saga，会自动创建）
            State.Id          = start.CorrelationId;
            State.DeviceId    = start.DeviceId;
            State.UserId      = start.UserId;
            State.StartedAt   = DateTimeOffset.UtcNow;
            State.CurrentStep = BindDeviceStep.NotStarted;

            // --------- 步骤 1：检查设备是否可用 ----------
            var chk = new CheckDeviceAvailabilityQuery(start.DeviceId);
            var devResult = await bus.InvokeAsync<DeviceAvailabilityResponse>(chk).WithTimeout(TimeSpan.FromSeconds(5));

            if (!devResult.IsAvailable)
            {
                // 直接进入 Failed 状态，写入原因
                await MarkFailedAsync($"Device unavailable: {devResult.Reason}");
                return;
            }

            State.CurrentStep = BindDeviceStep.DeviceChecked;
            // 保存状态（默认自动保存，显式调用可保证及时持久化）
            await SaveChangesAsync();

            // --------- 步骤 2：检查用户是否合法 ----------
            var userChk = new UserExistsQuery(start.UserId);
            var userResult = await bus.InvokeAsync<UserExistsResponse>(userChk).WithTimeout(TimeSpan.FromSeconds(5));

            if (!userResult.Exists)
            {
                await MarkFailedAsync($"User {start.UserId} does not exist");
                return;
            }

            State.CurrentStep = BindDeviceStep.UserChecked;
            await SaveChangesAsync();

            // --------- 步骤 3：调用 Device 服务执行绑定 ----------
            var bindCmd = new BindDeviceCommand(
                DeviceId: start.DeviceId,
                UserId:   start.UserId,
                CorrelationId: start.CorrelationId,
                RequestTime: DateTimeOffset.UtcNow);

            var bindResp = await bus.InvokeAsync<BindDeviceResponse>(bindCmd).WithTimeout(TimeSpan.FromSeconds(5));

            if (!bindResp.Success)
            {
                await MarkFailedAsync($"Device service error: {bindResp.ErrorMessage}");
                return;
            }

            State.CurrentStep = BindDeviceStep.DeviceBound;
            await SaveChangesAsync();

            // --------- 步骤 4：确认关联表已保存 ----------
            // 我们约定：DeviceBoundToUser 事件会被 ComponentB 处理并写库.
            // 为了让 Saga 等待它完成，可以订阅一个 ACK 事件（或轮询查询）.
            // 这里演示「轮询」方式（生产可改为 ACK 事件）.
            // 事件 ACK（替代轮询）:BFF 可以订阅 UserDeviceSaved（自定义事件），在 Handle(UserDeviceSaved) 中 CompleteSaga，消除轮询延迟.
            var timeout = TimeSpan.FromSeconds(5);
            var deadline = DateTimeOffset.UtcNow.Add(timeout);
            while (DateTimeOffset.UtcNow < deadline)
            {
                var check = new QueryUserDevices(start.UserId);
                var resp = await bus.InvokeAsync<UserDevicesResponse>(check);
                if (resp.DeviceIds.Contains(start.DeviceId))
                {
                    // 关联表已经完成写入
                    State.CurrentStep = BindDeviceStep.DeviceSaved;
                    await SaveChangesAsync();
                    await MarkCompletedAsync();
                    return;
                }

                await Task.Delay(200); // 轮询间隔
            }

            // 超时：视为失败，需要补偿
            await MarkFailedAsync("Timeout waiting for UserDevice table update");
        }

        // ---------- 辅助方法 ----------
        private async Task MarkFailedAsync(string reason)
        {
            State.CurrentStep = BindDeviceStep.Failed;
            State.FailureReason = reason;
            await SaveChangesAsync();               // 持久化
            // 可以发布一个统一的失败事件，让前端或监控系统感知
            await _bus.PublishAsync(new BindDeviceFailed(State.Id, reason));
        }

        private async Task MarkCompletedAsync()
        {
            State.CurrentStep = BindDeviceStep.Completed;
            State.CompletedAt = DateTimeOffset.UtcNow;
            await SaveChangesAsync();

            // 成功事件（可推送给 UI）
            await _bus.PublishAsync(new BindDeviceSucceeded(State.Id));
        }

        // ---------- 5️⃣ 补偿（Compensate） ----------
        // 当 Saga 状态进入 Failed 时，Wolverine 会调用对应的 Compensate 方法
        // 这里按照已完成的步骤逆序回滚

        public async Task Compensate(CancelBindDeviceSaga cancel, IMessageBus bus)
        {
            // 只要已经进入 DeviceBound 步骤，就需要撤销 Device 端的绑定
            if (State.CurrentStep >= BindDeviceStep.DeviceBound)
            {
                // Device 端 的撤销通过 UnbindDeviceCommand 完成（与绑定逻辑相反）
                var unbind = new UnbindDeviceCommand(
                    DeviceId: State.DeviceId,
                    UserId:   State.UserId,
                    CorrelationId: State.Id,
                    RequestTime: DateTimeOffset.UtcNow);

                var resp = await bus.InvokeAsync<UnbindDeviceResponse>(unbind);
                // 即使撤销失败，我们仍旧记录日志，继续执行剩余补偿
            }

            // 若已经写入关联表（DeviceSaved），则需要把它删掉
            if (State.CurrentStep >= BindDeviceStep.DeviceSaved)
            {
                // 直接发送一个补偿事件（B 端实现相应的 Remove）
                await bus.PublishAsync(new CompensateUserDevice(
                    DeviceId: State.DeviceId,
                    UserId:   State.UserId,
                    CorrelationId: State.Id));
            }

            // 最终把 Saga 标记为 Completed（补偿完成），保持状态干净
            State.CurrentStep = BindDeviceStep.Completed;
            await SaveChangesAsync();
        }
    }

    public class BindDeviceSaga : Saga<BindDeviceSagaState>
    {
        // ----------- 配置超时、最大补偿次数 ------------
        public const int MaxBusinessRetries = 3;
        public const int MaxCompensationRetries = 2;
        public static readonly TimeSpan StepTimeout = TimeSpan.FromSeconds(5);

        // ---------- 业务入口 ----------
        public async Task Handle(StartBindDeviceSaga start, IMessageBus bus)
        {
            // ① 初始化 Saga 状态（如果是首次则 Insert，否则 Load）
            State.Id          = start.CorrelationId;
            State.DeviceId    = start.DeviceId;
            State.UserId      = start.UserId;
            State.StartedAt   = DateTimeOffset.UtcNow;
            State.CurrentStep = BindDeviceStep.NotStarted;

            await SaveChangesAsync(); // 持久化 + Outbox 原子提交

            // ② 业务步骤（每一步都使用 timeout、重试计数）
            await ExecuteStepAsync(() => CheckDeviceAsync(bus), BindDeviceStep.DeviceChecked);
            await ExecuteStepAsync(() => CheckUserAsync(bus),   BindDeviceStep.UserChecked);
            await ExecuteStepAsync(() => BindDeviceAsync(bus), BindDeviceStep.DeviceBound);
            await ExecuteStepAsync(() => VerifyUserDeviceAsync(bus), BindDeviceStep.DeviceSaved);

            // ③ 成功结束
            await MarkCompletedAsync();
        }

        // -------------------------------------------------
        // 抽象出统一的“执行 + 重试 + 超时”逻辑
        private async Task ExecuteStepAsync(Func<Task<bool>> stepFunc, BindDeviceStep nextStep)
        {
            int attempts = 0;
            while (attempts < MaxBusinessRetries)
            {
                attempts++;
                try
                {
                    using var cts = new CancellationTokenSource(StepTimeout);
                    var ok = await stepFunc().WaitAsync(cts.Token);
                    if (ok)
                    {
                        State.CurrentStep = nextStep;
                        await SaveChangesAsync(); // 持久化当前进度
                        return;
                    }
                }
                catch (OperationCanceledException) when (!cts.IsCancellationRequested)
                {
                    // 超时
                    await MarkFailedAsync($"Step {nextStep} timed out (attempt {attempts})");
                    return;
                }
                catch (Exception ex)
                {
                    // 业务异常
                    if (attempts >= MaxBusinessRetries)
                    {
                        await MarkFailedAsync($"Step {nextStep} failed after {attempts} attempts: {ex.Message}");
                        return;
                    }
                    // 轻微异常延迟后重试
                    await Task.Delay(TimeSpan.FromMilliseconds(200 * attempts));
                }
            }
        }

        // -------------------------------------------------
        // 各业务步骤实现（均返回 bool 表示“成功”）
        private async Task<bool> CheckDeviceAsync(IMessageBus bus)
        {
            var qry = new CheckDeviceAvailabilityQuery(State.DeviceId);
            var rsp = await bus.InvokeAsync<DeviceAvailabilityResponse>(qry);

            if (!rsp.IsAvailable)
            {
                // 直接标记失败（业务终止）
                await MarkFailedAsync($"Device unavailable: {rsp.Reason}");
                return false;
            }
            return true;
        }

        private async Task<bool> CheckUserAsync(IMessageBus bus)
        {
            var qry = new UserExistsQuery(State.UserId);
            var rsp = await bus.InvokeAsync<UserExistsResponse>(qry);
            if (!rsp.Exists)
            {
                await MarkFailedAsync($"User {State.UserId} does not exist");
                return false;
            }
            return true;
        }

        private async Task<bool> BindDeviceAsync(IMessageBus bus)
        {
            var cmd = new BindDeviceCommand(
                DeviceId: State.DeviceId,
                UserId:   State.UserId,
                CorrelationId: State.Id,
                RequestTime: DateTimeOffset.UtcNow);

            // var rsp = await bus.InvokeAsync<BindDeviceResponse>(cmd);
            // if (!rsp.Success)
            // {
            //     await MarkFailedAsync($"Bind failed: {rsp.ErrorMessage}");
            //     return false;
            // }
            // return true;
            // 假设这些步骤全部成功，最后一步是写入关联表并发布 UserDeviceSaved
            await bus.SendAsync(cmd);

            // ③ **Schedule a timeout** – 如果在 N 秒内没有收到 ACK，就进入补偿
            var timeout = TimeSpan.FromSeconds(30);                 // 业务侧容忍时间
            await bus.ScheduleSendMessageIn<UserDeviceSavedTimeout>(new UserDeviceSavedTimeout(start.CorrelationId), timeout);
        }

        // ① 收到业务产生的 ACK 事件 → 完成 Saga
        public async Task Handle(UserDeviceSaved @event, IMessageBus bus)
        {
            // 只处理本 SagaId 对应的事件（防止其他 Saga 误收）
            if (@event.CorrelationId != State.Id) return;

            // 业务已成功写入关联表
            State.CurrentStep = BindDeviceStep.DeviceSaved;
            State.CompletedAt = DateTimeOffset.UtcNow;
            await SaveChangesAsync();

            // 发送成功通知（可选）
            await bus.PublishAsync(new BindDeviceSucceeded(State.Id));

            // 标记 Saga 完成（Wolverine 会自动归档/删除）
            await MarkCompletedAsync();
        }

        // ② 超时消息（如果在规定时间内未收到 UserDeviceSaved）
        public async Task Handle(UserDeviceSavedTimeout timeout, IMessageBus bus)
        {
            // 只针对当前 Saga
            if (timeout.CorrelationId != State.Id) return;

            // 超时 → 进入补偿或直接标记失败
            State.CurrentStep = BindDeviceStep.Failed;
            State.FailureReason = "Device‑User 关联写入超时，未收到 UserDeviceSaved 事件";
            await SaveChangesAsync();

            // 触发补偿（逆序撤销已经完成的步骤）
            await bus.PublishAsync(new CancelBindDeviceSaga(State.Id));
        }

        private async Task<bool> VerifyUserDeviceAsync(IMessageBus bus)
        {
            var qry = new QueryUserDevices(State.UserId);
            var rsp = await bus.InvokeAsync<UserDevicesResponse>(qry);
            return rsp.DeviceIds.Contains(State.DeviceId);
        }

        // -------------------------------------------------
        // 成功 / 失败 / 补偿 统一实现
        private async Task MarkFailedAsync(string reason)
        {
            State.CurrentStep = BindDeviceStep.Failed;
            State.FailureReason = reason;
            await SaveChangesAsync();

            // 触发补偿（Wolverine 会自动调用 Compensate 方法）
            await _bus.PublishAsync(new BindDeviceFailed(State.Id, reason));
        }

        private async Task MarkCompletedAsync()
        {
            State.CurrentStep = BindDeviceStep.Completed;
            State.CompletedAt = DateTimeOffset.UtcNow;
            await SaveChangesAsync();

            await _bus.PublishAsync(new BindDeviceSucceeded(State.Id));
        }

        // ------------------- 补偿 -------------------
        // 与前面介绍的 逆序补偿 思路保持一致，只是这里的触发点是 UserDeviceSavedTimeout 超时补偿.
        public async Task Compensate(CancelBindDeviceSaga _cancel, IMessageBus bus)
        {
            int attempts = 0;
            // 逆序补偿，只对已完成的步骤进行
            while (attempts < MaxCompensationRetries)
            {
                attempts++;
                try
                {
                    // ① 解绑 Device（如果已经绑定）
                    if (State.CurrentStep >= BindDeviceStep.DeviceBound)
                    {
                        var unbind = new UnbindDeviceCommand(
                            DeviceId: State.DeviceId,
                            UserId:   State.UserId,
                            CorrelationId: State.Id,
                            RequestTime: DateTimeOffset.UtcNow);
                        var resp = await bus.InvokeAsync<UnbindDeviceResponse>(unbind);
                        // Idempotent：下游服务自行判断已解绑则返回 Success
                    }

                    // ② 删除 UserDevice 关联（如果已经写入）
                    if (State.CurrentStep >= BindDeviceStep.DeviceSaved)
                    {
                        await bus.PublishAsync(new CompensateUserDevice(
                            DeviceId: State.DeviceId,
                            UserId:   State.UserId,
                            CorrelationId: State.Id));
                    }

                    // ③ 所有补偿成功，标记为 Completed（已补偿）
                    State.CurrentStep = BindDeviceStep.Completed;
                    await SaveChangesAsync();
                    return; // 正常退出
                }
                catch (Exception ex)
                {
                    // 记录日志，重试
                    _logger.LogError(ex,
                        "Compensation attempt {Attempt} for Saga {SagaId} failed", attempts, State.Id);

                    if (attempts >= MaxCompensationRetries)
                    {
                        // 进入不可恢复状态，发告警
                        await _bus.PublishAsync(new CompensationFailed(State.Id, ex.Message));
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(300 * attempts));
                    }
                }
            }
        }
    }

    // ---------- 启动消息 ----------
    public sealed record StartBindDeviceSaga(Guid CorrelationId, Guid DeviceId, Guid UserId);

    // 用于超时的内部消息（只会在当前 Saga 的节点上出现一次）
    public sealed record UserDeviceSavedTimeout(Guid CorrelationId);

    // ---------- 查询 Saga 状态 ----------
    public sealed record GetSagaState(Guid CorrelationId);
    public class GetSagaStateHandler
    {
        private readonly ISagaHost _host;
        public GetSagaStateHandler(ISagaHost host) => _host = host;

        public async Task<BindDeviceSagaState?> Handle(GetSagaState query)
        {
            // 通过 SagaHost 读取持久化的 Saga 状态（若使用 InMemory，直接取内存对象）
            var saga = await _host.LoadAsync<BindDeviceSagaState>(query.CorrelationId);
            return saga;
        }
    }

    // ---------- 成功 / 失败事件（供 UI/监控订阅） ----------
    public sealed record BindDeviceSucceeded(Guid CorrelationId);
    public sealed record BindDeviceFailed(Guid CorrelationId, string Reason);

    // ---------- 补偿命令（发送到 ComponentB） ----------
    // UserDevice 端 的撤销通过 CompensateUserDevice（自定义事件）完成，ComponentB 只需要实现对应的 Handle(CompensateUserDevice) 删除关联记录
    public sealed record CompensateUserDevice(Guid DeviceId, Guid UserId, Guid CorrelationId);
}


// # ① 创建用户（假设已有）
// curl -X POST http://localhost:5002/user/create -H "Content-Type: application/json" -d '{"name":"Alice"}'
// # ⇒ {"userId":"c7a1e3f3-..."}   → 记下 userId

// # ② 创建设备
// curl -X POST http://localhost:5000/device/create -H "Content-Type: application/json" -d '{"serial":"SN-001"}'
// # ⇒ {"deviceId":"57d9b9f0-..."} → 记下 deviceId

// # ③ 通过 BFF 发起绑定请求
// curl -X POST http://localhost:5001/api/UserDevice/bind \
//      -H "Content-Type: application/json" \
//      -d '{"deviceId":"57d9b9f0-...","userId":"c7a1e3f3-..."}'
// # 返回 202 {"correlationId":"a2b3c4d5-..."}

// # ④ 轮询状态（可改为 SSE/WebSocket）
// curl http://localhost:5001/api/UserDevice/status/a2b3c4d5-...

// #   可能的返回：
// #   { "currentStep":"Completed","completed":true }
// #   或者 { "currentStep":"Failed","failureReason":"Device unavailable: ..."}

// # 查询 B（关联表）直接 API（ComponentB）或 BFF 通过内部查询返回
// curl http://localhost:5002/user/c7a1e3f3-.../devices
// # => { "userId":"c7a1e3f3-...", "deviceIds":["57d9b9f0-..."] }

// curl http://localhost:5000/device/57d9b9f0-...   # 查看 Device 状态
// # => { "id":"57d9b9f0-...", "ownerId":"c7a1e3f3-...", "boundAt":"2025-08-09T.." }


+-------------------+          +-------------------+          +-------------------+
|   BFF / API       |  Start   |   BindDeviceSaga  |  Send    |  Device Service   |
| (StartBindDevice) |--------->|  (state=NotStarted)|------->| (BindDeviceCommand)|
+-------------------+          +-------------------+          +-------------------+
                                         |
                                         |   (业务步骤：CheckDevice、CheckUser …) 
                                         v
                                   +-------------------+
                                   |  BindDeviceHandler|
                                   |  (写入关联表)     |
                                   +-------------------+
                                         |
                                         |  Publish(UserDeviceSaved)   (Outbox事务)
                                         v
                               +---------------------------+
                               |   Message Bus (RabbitMQ)  |
                               +---------------------------+
                                         |
                         +-----------------------------+
                         |  BFF  (Saga)   或   其他 |
                         |  Handle(UserDeviceSaved) |
                         +-----------------------------+
                                         |
                                         |   MarkCompleted()
                                         v
                                   +-------------------+
                                   |  Saga 结束（删除） |
                                   +-------------------+

   ──────────────────────────────────────────────────────────────────────
   If 10s 未收到 UserDeviceSaved → Scheduler 发送
   UserDeviceSavedTimeout → Saga 进入补偿路径 → 逆向撤销

常见错误 & 防坑指南
错误情形	可能原因	解决办法
Saga 永远不结束	UserDeviceSaved 未携带正确的 CorrelationId 或者 PublishAsync 没走 Outbox（事务未提交）	① 确认 StartBindDeviceSaga 生成的 CorrelationId 正确传递到 BindDeviceCommand 与 UserDeviceSaved；② 检查 opts.EnableOutbox() 是否打开，await bus.PublishAsync 是否在同一个 DI / IMessageBus 实例中调用.
收到多条同样的 ACK	业务服务重试导致同一个 UserDeviceSaved 被多次发布	在消费端（Handle(UserDeviceSaved)) 用 if (State.IsCompleted) return; 或者 if (State.CurrentStep >= BindDeviceStep.DeviceSaved) return; 防止重复处理.
超时补偿被误触发	UserDeviceSaved 在 timeout 到达前已投递，但 Saga 已完成，UserDeviceSavedTimeout 仍在队列中，被错误地当作超时处理	在 Handle(UserDeviceSavedTimeout) 中首行加 if (State.IsCompleted) return;，或者使用 await bus.ScheduleSendMessageIn<UserDeviceSavedTimeout>(…, timeout, opt => opt.CorrelateById(state => state.Id)); 让 Wolverine 自动把超时消息与 Saga 关联，Saga 完成后自动 取消 计划任务（Wolverine 会在 MarkCompletedAsync 时撤销同 Id 的延迟消息）.
找不到 State.Id	SagaState 没有实现 IStatefulSaga（或 Id 并未映射为主键）	确保 BindDeviceSagaState 继承自 SagaState 并定义 public Guid Id { get; set; }，并在 DbContext 中配置 builder.HasKey(x => x.Id);
事件发不出去	消息总线使用的是 InMemory，但微服务部署在不同进程/容器里	替换 opts.Transports.InMemory() 为 opts.Transports.RabbitMq(...)、Kafka, 或者使用 Redis Streams（opts.Transports.Redis(...)）实现跨进程投递.