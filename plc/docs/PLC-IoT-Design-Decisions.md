# PLC IoT / AIOT 平台 — 详细设计决策与代码骨架（V2）

> 承接 `PLC-IoT-HotPlug-Architecture.md`，本轮把**具体选型**落到可编码的契约与代码骨架。
> 关键词：MEAI+MCP+Qdrant · Stateless · MQTT/EMQX · Channel+Redis 背压 · Polly · CQRS(MediatR/Wolverine Outbox·Inbox) · Scrutor · Carter · 协议帧抽象 · 生产级 PLC 问题。

---

## 0. 本轮选型决策总览

| 关注点 | 决策 | 备注 / GitHub |
|---|---|---|
| AI | **Microsoft.Extensions.AI (MEAI)** + **MCP** 协议 + **Qdrant** 向量库 | dotnet/extensions · modelcontextprotocol/csharp-sdk · qdrant/qdrant-dotnet |
| PLC 设备状态机 | **Stateless**（Configure 转换 / Permit 触发器 / Fire 转换） | dotnet-state-machine/stateless |
| AIOT 传输 | **MQTT** + **EMQX** | emqx/emqx |
| 消息队列 | `System.Threading.Channels` **生产者/消费者** + **Redis 背压** | 订阅/发布队列 |
| 弹性 | **Polly**：超时 / 熔断降级 / 优先级队列 / 舱壁 / **死信队列 + 重试次数** | App-vNext/Polly（v8 ResiliencePipeline） |
| CQRS | **MediatR**（进程内）+ **Outbox/Inbox 可靠消息**（Wolverine 或自建事务发件箱） | WolverineFx/wolverine（注：原文 "Signalynx" 未识别，Wolverine 为生产级 Outbox/Inbox 方案） |
| IOC | **Scrutor** 在 MS.DI 之上做程序集扫描 + 外观 AOP | khellang/Scrutor |
| 模块化 | **Carter** 实现模块化端点 | CarterCommunity/Carter |
| 协议规范 | 帧格式 `| 帧头\|长度\|命令字\|Payload\|校验\|帧尾\|`；解析引擎 / 帧分发器抽象隔离 | — |
| 抽象层级 | **driver 驱动抽象 / device 设备抽象 / pipeline 数据处理通道 / registry 注册中心** | — |
| Modbus | **NModbus4**（NuGet，上游维护 `NModbus/NModbus`）+ 遵守 `IProtocolAdapter` | NModbus/NModbus |
| OPC | **OPC Foundation.NetStandard.Opc.Ua** + **OpcUaHelper** + 遵守 `IProtocolAdapter` | OPCFoundation/UA-.NETStandard |
| 设备配置 | **json + Options** 运行时配置 | — |
| 心跳/健康 | 内置 `HealthCheck`（设备心跳 + 存活探针） | — |
| 西门子 S7 | **s7netplus** + 遵守 `IProtocolAdapter` | S7NetPlus/s7netplus |
| 生产级 PLC | 大小端 / 粘包拆包 / 噪声 / 校验失败 / 批量 / **RingBuffer** / 主线程⇄UI 线程 / 连接设置 / **单次解析互不影响** / **GC 释放** | — |
| 流式输出 | **IAsyncEnumerable\<T>** + **EnumeratorCancellation** | — |
| 数据库 | **SQLite + EF Core + Dapper AOT**（边缘/本地）· **InfluxDB**（时序）· **PostgreSQL**（业务）· **日志**→ Seq（热）+ ClickHouse（冷分析） | — |
| 前端 | **MVVM + Rx.NET** 实时发布 Mqtt；前端 **Mqtt.js** 订阅渲染 | — |

---

## 1. 协议插件抽象（核心）

### 1.1 统一帧格式
```
| 帧头(Header) | 长度(Length) | 命令字(Command) | 载荷(Payload) | 校验(Checksum) | 帧尾(Footer) |
```
- `Header/Footer`：协议特定魔数（如 `0xAA 0x55`）。
- `Length`：Payload 字节数（注意**大小端**，按设备规范）。
- `Command`：功能码 / 命令字。
- `Checksum`：CRC16/CRC32/异或和（协议特定；校验失败直接丢弃并计数）。

### 1.2 抽象接口（位于 `PlcPlatform.Abstractions`）
```csharp
// 协议适配器：所有 PLC 协议（Modbus/BACnet/OPC/S7…）对外统一契约
public interface IProtocolAdapter : IDisposable
{
    string ProtocolId { get; }                       // "Modbus" / "OpcUa" / "S7" ...
    ProtocolCapabilities Capabilities { get; }
    ValueTask StartAsync(DeviceOptions options, CancellationToken ct);
    ValueTask StopAsync(CancellationToken ct);
    // 统一出口：解析后的点表以 IAsyncEnumerable 流式吐出
    IAsyncEnumerable<DeviceFrame> ReadAsync(CancellationToken ct);
    ValueTask WriteAsync(WriteCommand cmd, CancellationToken ct);
}

// 驱动抽象：与物理链路（串口/网口）相关的收发
public interface IDriver
{
    Endianness Endianness { get; }
    ValueTask<int> SendAsync(ReadOnlyMemory<byte> frame, CancellationToken ct);
    IAsyncEnumerable<ReadOnlyMemory<byte>> ReceiveAsync(CancellationToken ct);
}

// 设备抽象：一个具体 PLC 设备实例（连接/心跳/状态机）
public interface IDevice
{
    string DeviceId { get; }
    DeviceStateMachine State { get; }                // Stateless 状态机
    IProtocolAdapter Adapter { get; }
    DeviceOptions Options { get; }
    ValueTask HeartbeatAsync(CancellationToken ct);
}

// 数据处理通道：粘包拆包 / 校验 / 大小端归一化
public interface IDataPipeline
{
    // 输入原始字节流，输出已校验、已拆分的标准帧
    IAsyncEnumerable<DeviceFrame> ProcessAsync(IAsyncEnumerable<ReadOnlyMemory<byte>> raw, CancellationToken ct);
}

// 注册中心：设备/协议/能力的全局注册与发现
public interface IRegistry
{
    void RegisterDevice(IDevice device);
    void RegisterAdapter(IProtocolAdapter adapter);
    IReadOnlyCollection<IDevice> Devices { get; }
    IDevice? Resolve(string deviceId);
}
```

### 1.3 解析引擎 + 帧分发器（抽象隔离）
- **解析引擎**（`IDataPipeline` 实现）：基于 `System.IO.Pipelines` 的 `PipeReader` 做**零拷贝粘包拆包**，用 `ReadOnlySequence<byte>` 在 `Span` 上做帧头定位、长度截断、大小端读取、校验。
- **帧分发器**（`FrameDispatcher`）：把标准帧按 `Command` 路由到对应处理通道（读点表 / 写命令 / 告警 / 心跳），各通道独立 `Channel`，**互不影响**。

```csharp
// 粘包拆包 + 大小端 + 校验的零拷贝解析（生产级骨架）
public sealed class FrameParser : IDataPipeline
{
    private readonly FrameSpec _spec;                 // 帧格式定义
    public async IAsyncEnumerable<DeviceFrame> ProcessAsync(
        IAsyncEnumerable<ReadOnlyMemory<byte>> raw,
        [EnumeratorCancellation] CancellationToken ct)
    {
        await foreach (var seg in raw.WithCancellation(ct))
        {
            // 用 Span 在原始内存上解析，不拷贝
            var span = seg.Span;
            int pos = FindHeader(span, _spec.Header);
            while (pos >= 0 && span.Length - pos >= _spec.MinSize)
            {
                int length = ReadLength(span.Slice(pos), _spec.Endianness);
                if (span.Length - pos < length + _spec.Overhead) break; // 半包，等下一段
                var payload = span.Slice(pos + _spec.HeaderLen, length);
                if (!VerifyChecksum(payload, _spec)) { Metrics.ChecksumFailures++; pos++; continue; } // 噪声/坏帧
                yield return BuildFrame(payload, _spec.Endianness);
                pos += length + _spec.Overhead;
            }
        }
    }
}
```

### 1.4 抽象基类（减少重复）
```csharp
public abstract class BaseProtocolDriver : IDriver
{
    public abstract Endianness Endianness { get; }
    protected readonly RingBuffer<byte> _ring = new(1 << 16); // 每设备独立 RingBuffer
    // 连接设置：地址/端口/串口/波特率 由 DeviceOptions 注入
    public abstract ValueTask<int> SendAsync(ReadOnlyMemory<byte> frame, CancellationToken ct);
    public abstract IAsyncEnumerable<ReadOnlyMemory<byte>> ReceiveAsync(CancellationToken ct);
}
```

### 1.5 设备配置（json + Options 运行时）
```json
// device.modbus.1.json
{ "DeviceId": "PLC-A1", "Protocol": "Modbus", "Transport": "Tcp",
  "Host": "192.168.1.10", "Port": 502, "SlaveId": 1, "Endianness": "Big",
  "PollingMs": 1000, "HeartbeatMs": 5000, "BaudRate": 0 }
```
```csharp
builder.Services.AddOptions<DeviceOptions>()
       .Bind(builder.Configuration.GetSection("Device:PLC-A1"))
       .ValidateDataAnnotations()
       .ValidateOnStart();           // 运行时热更新：IOptionsMonitor<DeviceOptions>
```

### 1.6 心跳与健康检查
```csharp
builder.Services.AddHealthChecks()
    .AddCheck<DeviceHeartbeatHealthCheck>("plc-a1");   // 设备存活探针
// IDevice.HeartbeatAsync 按 HeartbeatMs 周期上报，失败则状态机切入 Reconnecting
```

### 1.7 生产级 PLC 问题对策表
| 问题 | 对策 |
|---|---|
| 大小端差异 | `Endianness` 在 `IDriver` 声明，解析统一用 `BinaryPrimitives`（Span 上读写） |
| 粘包/拆包 | `PipeReader` + `ReadOnlySequence` 累积缓冲，按 `Length` 截断 |
| 噪声/坏帧 | 帧头扫描 + 校验失败丢弃并 `Metrics.ChecksumFailures++`，不抛异常 |
| 校验失败 | 计数 + 可选重传；不影响其它帧 |
| 批量操作 | 命令合并 + `Channel` 批处理写；读用 `IAsyncEnumerable` 流式 |
| RingBuffer | 每设备独立 `RingBuffer`，背压满则丢最旧或阻塞（按策略） |
| 主线程⇄UI 线程 | 解析在后台 `Channel`/Task；UI 通过 `IProgress<T>` / `IScheduler`(Rx) 切回 |
| 连接设置 | `DeviceOptions` 收敛地址/端口/串口/波特率/从站号 |
| **单次解析互不影响** | 每协议插件独立 ALC + 独立 `Channel` + 独立 `RingBuffer` + 独立状态机 |
| **GC 释放** | `ArrayPool`/`RecyclableMemoryStream` 复用；解析用 `Span`；ALC 卸载强制 GC |

---

## 2. PLC 设备 Stateless 状态机
```csharp
using Stateless;

public enum DeviceState { Disconnected, Connecting, Connected, Running, Error, Reconnecting }
public enum DeviceTrigger { Connect, Connected, Start, Stop, Fail, Recover, HeartbeatLost }

public sealed class DeviceStateMachine
{
    private readonly StateMachine<DeviceState, DeviceTrigger> _m;
    public DeviceStateMachine()
    {
        _m = new(DeviceState.Disconnected);
        // Configure 转换规则；Permit 触发器 → 目标状态；Fire 触发转换
        _m.Configure(DeviceState.Disconnected).Permit(DeviceTrigger.Connect, DeviceState.Connecting);
        _m.Configure(DeviceState.Connecting).Permit(DeviceTrigger.Connected, DeviceState.Connected)
                                            .Permit(DeviceTrigger.Fail, DeviceState.Error);
        _m.Configure(DeviceState.Connected).Permit(DeviceTrigger.Start, DeviceState.Running)
                                          .Permit(DeviceTrigger.HeartbeatLost, DeviceState.Reconnecting);
        _m.Configure(DeviceState.Running).Permit(DeviceTrigger.Stop, DeviceState.Connected)
                                         .Permit(DeviceTrigger.Fail, DeviceState.Error)
                                         .Permit(DeviceTrigger.HeartbeatLost, DeviceState.Reconnecting);
        _m.Configure(DeviceState.Error).Permit(DeviceTrigger.Recover, DeviceState.Reconnecting);
        _m.Configure(DeviceState.Reconnecting).Permit(DeviceTrigger.Connected, DeviceState.Connected)
                                              .Permit(DeviceTrigger.Fail, DeviceState.Error);
        _m.OnTransitioned(t => Metrics.StateTransition(t.Source, t.Destination));
    }
    public DeviceState State => _m.State;
    public void Fire(DeviceTrigger trigger) => _m.Fire(trigger);
}
```

---

## 3. AIOT：MQTT + Channel + Redis 背压
```csharp
// EMQX broker；Channel 生产者/消费者；Redis 做背压（队列深度限流）
var channel = Channel.CreateBounded<DeviceFrame>(new BoundedChannelOptions(1000)
{
    FullMode = BoundedChannelFullMode.Wait,        // 满则背压（生产者阻塞）
    SingleReader = false, SingleWriter = false
});

// 生产者：协议插件 → Channel（受 Redis 深度闸门控制）
async Task ProduceAsync(DeviceFrame frame, CancellationToken ct)
{
    var depth = await redis.StringGetAsync("mq:depth");   // Redis 背压信号
    if (depth != null && (int)depth > 5000) { await Task.Delay(50, ct); } // 限流
    await channel.Writer.WriteAsync(frame, ct);
    await redis.IncrAsync("mq:depth");
}

// 消费者：Channel → MQTTnet 发布到 EMQX
await foreach (var frame in channel.Reader.ReadAllAsync(ct))
{
    await mqttClient.PublishAsync(new MqttApplicationMessage
    {
        Topic = $"devices/{frame.DeviceId}/telemetry",
        PayloadSegment = frame.Payload,
        QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce
    }, ct);
    await redis.DecrAsync("mq:depth");
}
```
> 背压双保险：`BoundedChannel` 的 `Wait` + Redis 分布式深度计数，避免消费慢导致内存膨胀。

---

## 4. 弹性：Polly（超时/熔断/舱壁/优先级/死信/重试）
```csharp
// Polly v8 ResiliencePipeline
var pipeline = new ResiliencePipelineBuilder<DeviceFrame>()
    .AddRetry(new RetryStrategyOptions<DeviceFrame>
    {
        MaxRetryAttempts = 3,                           // 重试次数
        Delay = TimeSpan.FromMilliseconds(200),
        OnRetry = args => { Metrics.Retries++; return ValueTask.CompletedTask; }
    })
    .AddTimeout(TimeSpan.FromSeconds(2))               // 超时
    .AddCircuitBreaker(new CircuitBreakerStrategyOptions<DeviceFrame>  // 熔断降级
    {
        BreakingDuration = TimeSpan.FromSeconds(30),
        MinimumThroughput = 20,
        ShouldHandle = new PredicateBuilder<DeviceFrame>().Handle<IOException>()
    })
    .AddConcurrencyLimiter(10)                         // 舱壁（并发上限）
    .Build();

// 死信队列：多次失败进入 DLQ
async ValueTask SendWithDlqAsync(DeviceFrame f, CancellationToken ct)
{
    try { await pipeline.ExecuteAsync(async tk => await PublishAsync(f, tk), ct); }
    catch { await deadLetter.EnqueueAsync(f, ct); }    // 死信队列
}
```
> 优先级队列：在 `Channel` 之前用 `PriorityChannel<T>` 或按 QoS 分级多个 `Channel`（高/中/低）。

---

## 5. CQRS：MediatR（进程内）+ Outbox/Inbox（Wolverine 可靠消息）
```csharp
// 命令（CQRS：写路径）
public record WriteRegisterCommand(string DeviceId, ushort Address, short Value) : IRequest<Unit>;

public sealed class WriteRegisterHandler : IRequestHandler<WriteRegisterCommand, Unit>
{
    private readonly IRegistry _registry;
    public WriteRegisterHandler(IRegistry registry) => _registry = registry;
    public async Task<Unit> Handle(WriteRegisterCommand cmd, CancellationToken ct)
    {
        var dev = _registry.Resolve(cmd.DeviceId)
                   ?? throw new KeyNotFoundException(cmd.DeviceId);
        await dev.Adapter.WriteAsync(new WriteCommand(cmd.Address, cmd.Value), ct);
        return Unit.Value;
    }
}

// Outbox/Inbox：用 Wolverine 的持久化发件箱/收件箱保证"库写 + 消息投递"原子
// Program.cs
builder.Host.UseWolverine(opts =>
{
    opts.UseEntityFrameworkCoreTransactions();         // 与 EF Core 同事务
    opts.UsePostgresPersistenceAndMessageStore(connStr); // Outbox/Inbox 落 PostgreSQL
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opts.Policies.UseDurableInboxOnAllListeners();
});
```
> 若不便引入 Wolverine，可**自建事务发件箱**：业务表 + `Outbox` 表同事务写入，后台 Agent 轮询 `Outbox` 投递到 EMQX/RabbitMQ，成功即标记（幂等消费靠 `Inbox` 去重表）。

---

## 6. IOC + 模块化：Scrutor + Carter
```csharp
// Scrutor：程序集扫描 + 自动注册 + 装饰器(AOP)
builder.Services.Scan(scan => scan
    .FromAssembliesOf(typeof(ModbusAdapter))           // 插件程序集
    .AddClasses(c => c.AssignableTo<IProtocolAdapter>())
    .AsImplementedInterfaces()
    .WithScopedLifetime());

// 外观 AOP：日志/耗时装饰器（Scrutor Decorate）
builder.Services.AddScoped<IProtocolAdapter, ModbusAdapter>();
builder.Services.Decorate<IProtocolAdapter, AdapterTelemetryDecorator>(); // 自动包裹

// Carter：模块化端点
builder.Services.AddCarter();
// 模块
public sealed class DeviceModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/devices/{id}", (string id, IRegistry r) => r.Resolve(id) is { } d
            ? Results.Ok(d.Options) : Results.NotFound());
    }
}
// Program.cs: app.MapCarter();
```

---

## 7. AI：MEAI + MCP + Qdrant（RAG 质检/诊断）
```csharp
// Microsoft.Extensions.AI 统一 IChatClient；MCP 暴露工具；Qdrant 存设备知识向量
IChatClient chat = new OpenAIClient(apiKey).AsChatClient("gpt-4o-mini");
var qdrant = new QdrantClient("localhost");            // Qdrant.Client

// 1) 设备日志/手册嵌入进 Qdrant（离线）
await qdrant.UpsertAsync("plc-kb", embeddings);

// 2) 运行时：检索 + MEAI 生成诊断（通过 MCP 工具调用设备）
var hits = await qdrant.SearchAsync("plc-kb", queryVec, limit: 5);
var reply = await chat.CompleteAsync(
    $"基于以下知识：{hits.AsContext()}；设备 PLC-A1 报 0x05 错误，给出处置步骤。");
```
> MCP：`modelcontextprotocol/csharp-sdk` 把 `AIFunction` 暴露为 MCP Tools，供 MEAI/Agent 调用（读点位、下发命令）。

---

## 8. 数据库拓扑
| 数据 | 存储 | 用法 |
|---|---|---|
| 边缘/本地业务+缓存 | **SQLite + EF Core + Dapper AOT** | 边缘网关本地落库；热读用 `DapperAOT` 生成 AOT 代码 |
| 时序（点位/指标） | **InfluxDB** | 高频采样，按设备+时间检索；`influxdb-client-csharp` |
| 业务（订单/配置/用户） | **PostgreSQL** | 主业务库；Outbox/Inbox 亦落此 |
| 日志（热/冷） | **Seq**（实时排查）+ **ClickHouse**（冷分析） | 结构化日志 → Seq；海量审计/日志分析 → ClickHouse |

```csharp
// 分库：SQLite 本地 + PostgreSQL 业务
builder.Services.AddDbContext<EdgeDbContext>(o => o.UseSqlite("Data Source=edge.db"));
builder.Services.AddDbContext<BusinessDbContext>(o => o.UseNpgsql(connStr));
// 时序写（零分配批写）
await influx.WritePointsAsync(points, ct);
```

---

## 9. 前端：MVVM + Rx.NET 发布 Mqtt；Mqtt.js 订阅渲染
```csharp
// 后端：Rx.NET 把设备点表以 Subject 暴露，并桥接到 MQTT
var bus = new Subject<DeviceFrame>();
device.ReadAsync(ct).Subscribe(frame => bus.OnNext(frame));
bus.Subscribe(frame => mqttClient.PublishAsync(
    new MqttApplicationMessage { Topic = $"devices/{frame.DeviceId}/telemetry",
                                 PayloadSegment = frame.Payload }, CancellationToken.None));
```
```javascript
// 前端：Mqtt.js 订阅并渲染（MVVM 由框架处理，如 Vue/Knockout）
import mqtt from 'mqtt';
const client = mqtt.connect('wss://emqx-host/mqtt');
client.on('connect', () => client.subscribe('devices/+/telemetry'));
client.on('message', (topic, payload) => vm.points.update(JSON.parse(payload.toString())));
```

---

## 10. 落地顺序（与 V1 衔接）
1. 先落地 `Abstractions`（IProtocolAdapter/IDriver/IDevice/IDataPipeline/IRegistry）+ `FrameParser` + `DeviceStateMachine`，把现有 Modbus/S7/OPC 驱动包成遵守 `IProtocolAdapter` 的插件（验证热插拔 + 隔离解析）。
2. 接 `Channels` + EMQX + Redis 背压 + Polly 弹性；CQRS 用 MediatR，Outbox/Inbox 先用自建事务发件箱，再视需要换 Wolverine。
3. IOC 用 Scrutor 扫描插件程序集 + Carter 暴露端点；AI 用 MEAI+MCP+Qdrant 做诊断助手。
4. 数据库分库（SQLite/InfluxDB/PostgreSQL/Seq+ClickHouse）；前端 MVVM+Rx.NET+Mqtt.js 实时渲染。

---
*V2 在 V1（底座/能力/入口/出口 + 框架推荐）之上，把你的 20+ 项具体选型转化为可编码契约与代码骨架，重点解决「协议隔离、零分配解析、状态机、可靠消息、弹性、AI 集成、分库」等生产级问题。*
