// entities.cs — 共享领域实体（SaaS Schema 锁定表，§6 / SaaS-DB-Schema.md）
// 由 infra.cs 经 #include 引入（File-based）；Project 验证工程直接编译。
// 关键红线：不含任何 price / currency / amount / money 字段（ADR-108 / AC-10）。
#if !ENTITIES_CS
#define ENTITIES_CS

namespace PlcAiot.Saas.Domain;

// —— 枚举（存储为文本，匹配 PG CHECK 约束；中文/小写与 Schema 一致）——
public enum TenantStatus { Active, Suspended }
public enum SubscriptionStatus { Active, Suspended }
public enum DeviceStatus { online, offline, fault }
public enum FaultStatus { open, acked, resolved, closed }
public enum QuotaMetric { device, telemetry }
public enum QuotaAction { Warn, Throttle }

// —— 租户根（非 ITenantEntity；全租户共享）——
public sealed class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public TenantStatus Status { get; set; } = TenantStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// —— 套餐档（目录表，无价格字段，仅配额差异化）——
public sealed class Plan
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;        // 免费 / 专业 / 商业 / 企业
    public int DeviceQuota { get; set; }
    public int TelemetryQuota { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// —— 租户订阅（ITenantEntity；运营态与租户同步暂停）——
public sealed class Subscription : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid PlanId { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public DateTime StartAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// —— 用量计量（ITenantEntity；仅计数，无货币）——
public sealed class UsageMeter : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Period { get; set; } = string.Empty;       // YYYY-MM
    public int DeviceCount { get; set; }
    public long TelemetryPoints { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// —— 配额策略（目录表；无货币）——
public sealed class QuotaPolicy
{
    public Guid Id { get; set; }
    public Guid PlanId { get; set; }
    public QuotaMetric Metric { get; set; }
    public decimal ThresholdPct { get; set; }
    public QuotaAction Action { get; set; }
}

// —— 设备（既有 + TenantId，ITenantEntity）——
public sealed class Device : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DeviceStatus Status { get; set; } = DeviceStatus.offline;
    public string? Profile { get; set; }                     // jsonb（点表）
    public DateTime? LastHeartbeat { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// —— 故障事件（既有 + TenantId，ITenantEntity）——
public sealed class FaultEvent : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid DeviceId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public FaultStatus Status { get; set; } = FaultStatus.open;
    public string? CorrelationId { get; set; }
    public Guid? AckedBy { get; set; }
    public DateTime? AckedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// —— 发件箱（ITenantEntity；payload 显式携带 tenant_id，ADR-102）——
public sealed class OutboxMessage : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Payload { get; set; } = "{}";              // jsonb
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// —— 刷新令牌（ITenantEntity；仅存哈希）——
public sealed class RefreshToken : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}

// —— 审计日志（Backlog 启用；tenant_id 可空 = 平台级为 NULL）——
public sealed class AuditLog
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string Actor { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public DateTime At { get; set; } = DateTime.UtcNow;
    public string? Detail { get; set; }                      // jsonb
}
#endif
