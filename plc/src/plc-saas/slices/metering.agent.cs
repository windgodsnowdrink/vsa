// slices/metering.agent.cs — 计量后台（Outbox 消费者 + 配额评估 + SignalR 推送，§SaaS-Metering / ADR-102/108）
// 轮询 OutboxMessages(sent_at IS NULL) → 聚合 UsageMeters → 评估 QuotaPolicies → 发布事件经 SignalR 直推。
// 仅计量，无 price/currency/amount/money 字段。
#include "../infra.cs"
using System.Text.Json;
using System.Collections.Concurrent;

public sealed class MeteringAgent : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _cfg;
    private readonly ILogger<MeteringAgent> _log;
    // 阈值提醒去重（降回阈值下后允许再次提醒）
    private static readonly ConcurrentDictionary<string, bool> _thresholdFired = new();

    public MeteringAgent(IServiceScopeFactory scopeFactory, IConfiguration cfg, ILogger<MeteringAgent> log)
        => (_scopeFactory, _cfg, _log) = (scopeFactory, cfg, log);

    protected override async Task ExecuteAsync(CancellationToken stopping)
    {
        var interval = TimeSpan.FromSeconds(_cfg.GetValue("Metering:PollIntervalSeconds", 30));
        using var timer = new PeriodicTimer(interval);
        await RunOnce(stopping); // 启动即跑一次
        while (await timer.WaitForNextTickAsync(stopping))
            await RunOnce(stopping);
    }

    private async Task RunOnce(CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BaseDbContext>();
            var current = scope.ServiceProvider.GetRequiredService<ICurrentTenant>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            current.IsRoot = true; // 后台作业显式 restore tenant（payload 携带，Schema §3）

            await DrainOutbox(db, current, mediator, ct);
            await SnapshotAndEvaluate(db, current, mediator, ct);
        }
        catch (OperationCanceledException) { /* 优雅停机 */ }
        catch (Exception ex) { _log.LogError(ex, "计量任务执行异常"); }
    }

    // 消费 Outbox（幂等；tenant_id 显式来自 payload）
    private async Task DrainOutbox(BaseDbContext db, ICurrentTenant current, IMediator mediator, CancellationToken ct)
    {
        var pending = await db.OutboxMessages.IgnoreQueryFilters()
            .Where(o => o.SentAt == null).OrderBy(o => o.CreatedAt).Take(100).ToListAsync(ct);
        foreach (var msg in pending)
        {
            current.TenantId = msg.TenantId; // restore 连接级 SESSION_CONTEXT N'TenantId'
            if (msg.Type == "TelemetryIngested")
            {
                var period = DateTime.UtcNow.ToString("yyyy-MM");
                await UpsertUsage(db, msg.TenantId, period, 0, ExtractPoints(msg.Payload), ct);
            }
            msg.SentAt = DateTime.UtcNow; // 标记已投递
        }
        if (pending.Count > 0) await db.SaveChangesAsync(ct);
    }

    // 设备数月度快照 + 配额阈值评估（G5/G8）
    private async Task SnapshotAndEvaluate(BaseDbContext db, ICurrentTenant current, IMediator mediator, CancellationToken ct)
    {
        var period = DateTime.UtcNow.ToString("yyyy-MM");
        var tenants = await db.Tenants.IgnoreQueryFilters().Where(t => t.Status == TenantStatus.Active).ToListAsync(ct);
        foreach (var t in tenants)
        {
            current.TenantId = t.Id; // 全局过滤器按此租户匹配
            var sub = await db.Subscriptions.FirstOrDefaultAsync(s => s.TenantId == t.Id, ct);
            if (sub is null) continue;
            var plan = await db.Plans.FirstAsync(p => p.Id == sub.PlanId, ct);

            var deviceCount = await db.Devices.CountAsync(d => d.TenantId == t.Id, ct);
            await EnsureUsageRow(db, t.Id, period, ct);
            var meter = await db.UsageMeters.FirstAsync(m => m.TenantId == t.Id && m.Period == period, ct);
            meter.DeviceCount = deviceCount; // 月度快照
            await db.SaveChangesAsync(ct);

            var policies = await db.QuotaPolicies.Where(q => q.PlanId == plan.Id).ToListAsync(ct);
            foreach (var pol in policies)
            {
                long used = pol.Metric == QuotaMetric.device ? deviceCount : meter.TelemetryPoints;
                long quota = pol.Metric == QuotaMetric.device ? plan.DeviceQuota : plan.TelemetryQuota;
                if (quota <= 0) continue;
                var pct = (decimal)used / quota * 100m;
                var key = $"{t.Id}:{pol.Metric}:{period}";

                if (used > quota)
                {
                    await mediator.Publish(new QuotaExceeded(t.Id, pol.Metric.ToString(), used, quota), ct);
                    _thresholdFired[key] = true;
                }
                else if (pct >= pol.ThresholdPct && pol.Action == QuotaAction.Warn)
                {
                    if (!_thresholdFired.GetOrAdd(key, false))
                    {
                        await mediator.Publish(new QuotaThresholdReached(t.Id, pol.Metric.ToString(), used, quota, pct), ct);
                        _thresholdFired[key] = true; // 去重，降回阈值下前不重复
                    }
                }
                else if (pct < pol.ThresholdPct)
                {
                    _thresholdFired[key] = false;
                }
            }
        }
    }

    private async Task EnsureUsageRow(BaseDbContext db, Guid tenantId, string period, CancellationToken ct)
    {
        if (!await db.UsageMeters.AnyAsync(m => m.TenantId == tenantId && m.Period == period, ct))
        {
            db.UsageMeters.Add(new UsageMeter
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Period = period,
                DeviceCount = 0,
                TelemetryPoints = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            });
            await db.SaveChangesAsync(ct);
        }
    }

    private async Task UpsertUsage(BaseDbContext db, Guid tenantId, string period, int deviceDelta, long pointDelta, CancellationToken ct)
    {
        var m = await db.UsageMeters.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Period == period, ct);
        if (m is null)
            db.UsageMeters.Add(new UsageMeter
            {
                Id = Guid.NewGuid(), TenantId = tenantId, Period = period,
                DeviceCount = deviceDelta, TelemetryPoints = pointDelta,
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow,
            });
        else
        {
            m.DeviceCount += deviceDelta;
            m.TelemetryPoints += pointDelta;
            m.UpdatedAt = DateTime.UtcNow;
        }
    }

    private static long ExtractPoints(string payload)
    {
        try { using var d = JsonDocument.Parse(payload); return d.RootElement.GetProperty("points").GetInt64(); }
        catch { return 0; }
    }
}

// quota 事件 → SignalR 直推（同 UoW 强一致路径，ADR-105）
public sealed class QuotaThresholdPushHandler : INotificationHandler<QuotaThresholdReached>
{
    private readonly IHubContext<FaultsHub> _hub;
    public QuotaThresholdPushHandler(IHubContext<FaultsHub> hub) => _hub = hub;
    public async ValueTask Handle(QuotaThresholdReached n, CancellationToken ct)
    {
        var tid = n.TenantId == Guid.Empty ? SaasTenant.Root : n.TenantId.ToString();
        await _hub.Clients.Group(tid).SendAsync("quota.threshold_reached",
            new { type = "quota_threshold_reached", tenantId = tid, metric = n.Metric, used = n.Used, quota = n.Quota, pct = n.Pct }, ct);
    }
}

public sealed class QuotaExceededPushHandler : INotificationHandler<QuotaExceeded>
{
    private readonly IHubContext<FaultsHub> _hub;
    public QuotaExceededPushHandler(IHubContext<FaultsHub> hub) => _hub = hub;
    public async ValueTask Handle(QuotaExceeded n, CancellationToken ct)
    {
        var tid = n.TenantId == Guid.Empty ? SaasTenant.Root : n.TenantId.ToString();
        await _hub.Clients.Group(tid).SendAsync("quota.exceeded",
            new { type = "quota_exceeded", tenantId = tid, metric = n.Metric, used = n.Used, quota = n.Quota }, ct);
    }
}
