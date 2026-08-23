// slices/ingest.bridge.cs — 真实 MQTT/EMQX 摄取桥（ADR-113，P0 数据平面关键路径）
// 双摄取路径，统一收敛到 Ingest*Command：
//   ① 主路径：EMQX 规则引擎 → POST /api/v1/ingest/*（slices/ingest.http.cs）
//   ② 边缘中继（可选）：MqttBridgeService 订阅 EMQX 主题 → 同命令
// 设备映射：deviceId(Guid) 全局唯一；首条遥测自动纳管设备（V1-now 友好演示）。
// 隔离：tid 来自主题/头；写操作显式设置 ICurrentTenant.TenantId；租户暂停则拒（AC-07）。
#include "../infra.cs"
using System.Text.Json;
using MQTTnet;

// —— 设备解析（全局唯一 + 跨租户拒绝 + 自动纳管）——
internal static class DeviceResolver
{
    public static async Task<Device> ResolveOrProvisionAsync(BaseDbContext db, Guid tenantId, Guid deviceId, CancellationToken ct)
    {
        var existing = await db.Devices.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.Id == deviceId, ct);
        if (existing is not null)
        {
            if (existing.TenantId != tenantId)
                throw new CrossTenantWriteException(nameof(Device), existing.TenantId, tenantId);
            return existing;
        }
        var device = new Device
        {
            Id = deviceId,
            TenantId = tenantId,
            Name = deviceId.ToString(),
            Status = DeviceStatus.online,
            LastHeartbeat = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };
        db.Devices.Add(device);
        return device; // SaveChanges 由 handler 统一提交（含 Outbox）
    }
}

// —— 摄取守卫（租户存在 + 活跃）——
internal static class IngestGuards
{
    public static async Task EnsureTenantActive(BaseDbContext db, Guid tenantId, CancellationToken ct)
    {
        if (tenantId == Guid.Empty) throw new DomainException(40300, "非法租户");
        var tenant = await db.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Id == tenantId, ct);
        if (tenant is null) throw new DomainException(40400, "租户不存在");
        if (tenant.Status == TenantStatus.Suspended) throw new DomainException(40900, "租户已暂停，摄取被拒绝");
    }
}

// ===================== 遥测摄取 =====================
public sealed record IngestTelemetryCommand(
    Guid TenantId, Guid DeviceId, DateTime Timestamp,
    IReadOnlyDictionary<string, double> Metrics, long PointCount) : IRequest<IngestTelemetryResponse>;

public sealed record IngestTelemetryResponse(Guid DeviceId, DateTime LastHeartbeat);

public sealed class IngestTelemetryHandler : IRequestHandler<IngestTelemetryCommand, IngestTelemetryResponse>
{
    private readonly BaseDbContext _db;
    private readonly ICurrentTenant _current;
    private readonly ITelemetryStore _store;
    public IngestTelemetryHandler(BaseDbContext db, ICurrentTenant current, ITelemetryStore store)
        => (_db, _current, _store) = (db, current, store);

    public async ValueTask<IngestTelemetryResponse> Handle(IngestTelemetryCommand cmd, CancellationToken ct)
    {
        await IngestGuards.EnsureTenantActive(_db, cmd.TenantId, ct);
        _current.TenantId = cmd.TenantId; // 匿名摄取无 JWT，显式设置连接级 SESSION_CONTEXT N'TenantId'（RLS 兜底）

        var device = await DeviceResolver.ResolveOrProvisionAsync(_db, cmd.TenantId, cmd.DeviceId, ct);
        device.Status = DeviceStatus.online;
        device.LastHeartbeat = cmd.Timestamp == default ? DateTime.UtcNow : cmd.Timestamp;
        var ts = device.LastHeartbeat ?? DateTime.UtcNow;

        await _store.WriteAsync(cmd.TenantId, cmd.DeviceId, ts, cmd.Metrics, ct);

        // Outbox：计量代理消费 TelemetryIngested 累加 usage_meters.telemetry_points（ADR-102/108）
        _db.OutboxMessages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            TenantId = cmd.TenantId,
            Type = "TelemetryIngested",
            Payload = JsonSerializer.Serialize(new { points = cmd.PointCount, deviceId = cmd.DeviceId, ts }),
            CreatedAt = DateTime.UtcNow,
        });

        await _db.SaveChangesAsync(ct);
        return new IngestTelemetryResponse(device.Id, ts);
    }
}

// ===================== 故障摄取 =====================
public sealed record IngestFaultCommand(
    Guid TenantId, Guid DeviceId, string Code, string Severity, string? Message) : IRequest<IngestFaultResponse>;

public sealed record IngestFaultResponse(Guid FaultId, string Status);

public sealed class IngestFaultHandler : IRequestHandler<IngestFaultCommand, IngestFaultResponse>
{
    private readonly BaseDbContext _db;
    private readonly ICurrentTenant _current;
    private readonly IMediator _mediator;
    public IngestFaultHandler(BaseDbContext db, ICurrentTenant current, IMediator mediator)
        => (_db, _current, _mediator) = (db, current, mediator);

    public async ValueTask<IngestFaultResponse> Handle(IngestFaultCommand cmd, CancellationToken ct)
    {
        await IngestGuards.EnsureTenantActive(_db, cmd.TenantId, ct);
        _current.TenantId = cmd.TenantId;

        var device = await DeviceResolver.ResolveOrProvisionAsync(_db, cmd.TenantId, cmd.DeviceId, ct);
        device.Status = DeviceStatus.fault;

        var fault = new FaultEvent
        {
            Id = Guid.NewGuid(),
            TenantId = cmd.TenantId,
            DeviceId = cmd.DeviceId,
            Code = cmd.Code,
            Severity = cmd.Severity,
            Status = FaultStatus.open,
            CreatedAt = DateTime.UtcNow,
        };
        _db.FaultEvents.Add(fault);
        await _db.SaveChangesAsync(ct);

        // 热路径强一致：FaultRaised → SignalR 直推（同 ADR-105 FaultAcked 范式）
        await _mediator.Publish(new FaultRaised(cmd.TenantId, fault.Id, cmd.DeviceId, cmd.Code, cmd.Severity), ct);
        return new IngestFaultResponse(fault.Id, fault.Status.ToString());
    }
}

// 故障产生通知 → SignalR 直推租户组（前端角标，AC-08）
public sealed record FaultRaised(Guid TenantId, Guid FaultId, Guid DeviceId, string Code, string Severity) : INotification;

public sealed class FaultRaisedPushHandler : INotificationHandler<FaultRaised>
{
    private readonly IHubContext<FaultsHub> _hub;
    public FaultRaisedPushHandler(IHubContext<FaultsHub> hub) => _hub = hub;
    public async ValueTask Handle(FaultRaised n, CancellationToken ct)
    {
        var tid = n.TenantId == Guid.Empty ? SaasTenant.Root : n.TenantId.ToString();
        await _hub.Clients.Group(tid).SendAsync("fault.raised",
            new { type = "fault_raised", tenantId = tid, faultId = n.FaultId, deviceId = n.DeviceId, code = n.Code, severity = n.Severity }, ct);
    }
}

// ===================== 边缘中继：MQTTnet 订阅桥（可选，Mqtt:Bridge:Enabled）=====================
// 以 EMQX superuser 身份订阅跨租户主题，从主题解析 tid+deviceId，复用 Ingest*Command（与 HTTP 路径同逻辑）。
// Broker 不可达时退避重试，绝不崩溃宿主进程；仅 Mqtt:Bridge:Enabled=true 时由 AddSaasIngest 注册。
public sealed class MqttBridgeService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _cfg;
    private readonly ILogger<MqttBridgeService> _log;
    private IMqttClient? _client;

    public MqttBridgeService(IServiceScopeFactory scopeFactory, IConfiguration cfg, ILogger<MqttBridgeService> log)
        => (_scopeFactory, _cfg, _log) = (scopeFactory, cfg, log);

    protected override async Task ExecuteAsync(CancellationToken stopping)
    {
        while (!stopping.IsCancellationRequested)
        {
            try { await ConnectAndLoopAsync(stopping); }
            catch (OperationCanceledException) when (stopping.IsCancellationRequested) { break; }
            catch (Exception ex) { _log.LogError(ex, "MQTT 桥连接异常，10s 后退避重试"); }

            if (stopping.IsCancellationRequested) break;
            try { await Task.Delay(TimeSpan.FromSeconds(10), stopping); }
            catch (OperationCanceledException) { break; }
        }
        _log.LogInformation("MQTT 桥已停止");
    }

    private async Task ConnectAndLoopAsync(CancellationToken stopping)
    {
        var broker = _cfg["Mqtt:BrokerUrl"] ?? "mqtt://localhost:1883";
        var uri = new Uri(broker);
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 1883;

        var factory = new MqttClientFactory();
        _client = factory.CreateMqttClient();
        _client.ApplicationMessageReceivedAsync += OnMessageAsync;

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(host, port)
            .WithClientId(_cfg["Mqtt:Bridge:ClientId"] ?? "plc-saas-bridge")
            .WithCredentials(_cfg["Mqtt:Bridge:Username"] ?? "plc-saas", _cfg["Mqtt:Bridge:Password"] ?? "")
            .Build();

        await _client.ConnectAsync(options, stopping);
        var sub = new MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter("tenants/+/devices/+/telemetry")
            .WithTopicFilter("tenants/+/devices/+/fault")
            .Build();
        await _client.SubscribeAsync(sub, stopping);
        _log.LogInformation("MQTT 桥已连接 {Broker}，订阅 tenants/+/devices/+/telemetry|fault", broker);

        try { await Task.Delay(Timeout.Infinite, stopping); }
        finally
        {
            try { if (_client.IsConnected) await _client.DisconnectAsync(); } catch { /* 优雅退出 */ }
        }
    }

    private Task OnMessageAsync(MqttApplicationMessageReceivedEventArgs e)
        => HandleMqttMessageAsync(e.ApplicationMessage.Topic, e.ApplicationMessage.ConvertPayloadToString());

    private async Task HandleMqttMessageAsync(string topic, string payload)
    {
        try
        {
            var segs = topic.Split('/');
            if (segs.Length != 5 || segs[0] != "tenants" || segs[2] != "devices") return;
            if (!Guid.TryParse(segs[1], out var tid) || !Guid.TryParse(segs[3], out var deviceId)) return;
            var kind = segs[4]; // telemetry | fault

            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            if (kind == "telemetry")
            {
                var (ts, metrics) = ParseTelemetry(payload);
                await mediator.Send(new IngestTelemetryCommand(tid, deviceId, ts, metrics, metrics.Count));
            }
            else if (kind == "fault")
            {
                var (code, severity, message) = ParseFault(payload);
                await mediator.Send(new IngestFaultCommand(tid, deviceId, code, severity, message));
            }
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "MQTT 消息处理失败 topic={Topic}", topic);
        }
    }

    private static (DateTime ts, Dictionary<string, double> metrics) ParseTelemetry(string payload)
    {
        var metrics = new Dictionary<string, double>();
        var ts = DateTime.UtcNow;
        try
        {
            using var d = JsonDocument.Parse(payload);
            var root = d.RootElement;
            if (root.TryGetProperty("ts", out var tsEl) && tsEl.ValueKind == JsonValueKind.Number)
                ts = DateTimeOffset.FromUnixTimeSeconds((long)tsEl.GetDouble()).UtcDateTime;
            if (root.TryGetProperty("metrics", out var m) && m.ValueKind == JsonValueKind.Object)
                foreach (var p in m.EnumerateObject())
                    if (p.Value.ValueKind == JsonValueKind.Number)
                        metrics[p.Name] = p.Value.GetDouble();
        }
        catch { /* 脏负载：降级为空指标，仍记录设备心跳 */ }
        return (ts, metrics);
    }

    private static (string code, string severity, string? message) ParseFault(string payload)
    {
        var code = "UNKNOWN"; var severity = "medium"; string? message = null;
        try
        {
            using var d = JsonDocument.Parse(payload);
            var root = d.RootElement;
            if (root.TryGetProperty("code", out var c) && c.ValueKind == JsonValueKind.String) code = c.GetString()!;
            if (root.TryGetProperty("severity", out var s) && s.ValueKind == JsonValueKind.String) severity = s.GetString()!;
            if (root.TryGetProperty("message", out var m) && m.ValueKind == JsonValueKind.String) message = m.GetString();
        }
        catch { /* 脏负载：用默认码/级别 */ }
        return (code, severity, message);
    }
}
