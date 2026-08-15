// infra.cs — PLC·AIOT 多租户 SaaS 共享基础设施（File-based App）
// 由 Program.cs 与各切片经 #include 引入。#r 集中声明 NuGet 引用（ADR-100/102）。
// 类型定义用 #if !INFRA_CS 守卫，避免多切片 #include 导致重复定义。
// 红线：以下任何类型/方法均不含 price/currency/amount/money（ADR-108）。

#r "nuget:Microsoft.EntityFrameworkCore, 11.0.0-preview.6.26359.118"
#r "nuget:Npgsql.EntityFrameworkCore.PostgreSQL, 11.0.0-preview.6"
#r "nuget:Microsoft.AspNetCore.Identity.EntityFrameworkCore, 11.0.0-preview.6.26359.118"
#r "nuget:Microsoft.AspNetCore.Authentication.JwtBearer, 11.0.0-preview.6.26359.118"
#r "nuget:System.IdentityModel.Tokens.Jwt, 8.22.0"
#r "nuget:Mediator.Abstractions, 3.0.2"
#r "nuget:Mediator.SourceGenerator, 3.0.2"

#if !INFRA_CS
#define INFRA_CS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Npgsql;
using Mediator;
using System.Threading.RateLimiting;

// ===================== 常量 =====================
public static class SaasRoles
{
    public const string Admin = "Admin";
    public const string Analyst = "Analyst";
    public const string Operator = "Operator";
    public const string DeviceEng = "DeviceEng";
    public const string SuperAdmin = "SuperAdmin"; // 平台超管（tid=root）
    public static readonly IReadOnlyList<string> All = [Admin, Analyst, Operator, DeviceEng];
}

public static class SaasTenant { public const string Root = "root"; }

public static class SaasClaims
{
    public const string TenantId = "tid";
    public const string UserId = "uid";
    public const string Role = ClaimTypes.Role;
}

public static class SaasPlans
{
    public const string Free = "免费";
    public const string Pro = "专业";
    public const string Business = "商业";
    public const string Enterprise = "企业";
    public static readonly IReadOnlyList<string> All = [Free, Pro, Business, Enterprise];
}

// ===================== 多租户核心 =====================
public interface ITenantEntity { Guid TenantId { get; set; } }

public interface ICurrentTenant
{
    Guid TenantId { get; set; }
    bool IsRoot { get; set; }
}

public sealed class CurrentTenant : ICurrentTenant
{
    public Guid TenantId { get; set; } = Guid.Empty;
    public bool IsRoot { get; set; }
}

// 跨租户写入被框架级拦截（超开发者纪律，ADR-103）
public sealed class CrossTenantWriteException : Exception
{
    public CrossTenantWriteException(string entity, Guid attempted, Guid current)
        : base($"Cross-tenant write rejected: entity '{entity}' carried TenantId={attempted} but current context is {current}.") { }
}

// ===================== Identity 实体（ITenantEntity）=====================
public sealed class ApplicationUser : IdentityUser<Guid>, ITenantEntity
{
    public Guid TenantId { get; set; }
    public string? Name { get; set; }
}

public sealed class ApplicationRole : IdentityRole<Guid>, ITenantEntity
{
    public ApplicationRole() { }
    public ApplicationRole(string name) : base(name) { }
    public Guid TenantId { get; set; }
}

public sealed class ApplicationUserRole : IdentityUserRole<Guid>, ITenantEntity
{
    public Guid TenantId { get; set; }
}

// ===================== 拦截器 =====================
// 写时盖戳 + 拒绝跨租户写入（ADR-103 ③.2）
public sealed class TenantStampInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentTenant _tenant;
    public TenantStampInterceptor(ICurrentTenant tenant) => _tenant = tenant;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is BaseDbContext ctx)
            Stamp(ctx);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is BaseDbContext ctx)
            Stamp(ctx);
        return base.SavingChanges(eventData, result);
    }

    private void Stamp(BaseDbContext ctx)
    {
        foreach (var entry in ctx.ChangeTracker.Entries<ITenantEntity>()
                     .Where(e => e.State is EntityState.Added or EntityState.Modified))
        {
            if (entry.Entity.TenantId == Guid.Empty)
                entry.Entity.TenantId = _tenant.TenantId; // 自动盖戳
            else if (entry.Entity.TenantId != _tenant.TenantId && !_tenant.IsRoot)
                throw new CrossTenantWriteException(entry.Entity.GetType().Name, entry.Entity.TenantId, _tenant.TenantId);
        }
    }
}

// 每条连接打开时设置 app.tenant_id（RLS 兜底，ADR-103 ③.4）
public sealed class TenantConnectionInterceptor : DbConnectionInterceptor
{
    private readonly ICurrentTenant _tenant;
    public TenantConnectionInterceptor(ICurrentTenant tenant) => _tenant = tenant;

    public override Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        if (connection is NpgsqlConnection npg)
        {
            using var cmd = npg.CreateCommand();
            cmd.CommandText = "SET app.tenant_id = :tid;";
            cmd.Parameters.AddWithValue("tid", _tenant.IsRoot ? Guid.Empty : _tenant.TenantId);
            cmd.ExecuteNonQuery();
        }
        return Task.CompletedTask;
    }
}

// ===================== DbContext =====================
public sealed class BaseDbContext : IdentityDbContext<
    ApplicationUser, ApplicationRole, Guid,
    IdentityUserClaim<Guid>, ApplicationUserRole, IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    private readonly ICurrentTenant _currentTenant;
    public BaseDbContext(DbContextOptions<BaseDbContext> options, ICurrentTenant currentTenant) : base(options) => _currentTenant = currentTenant;

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<UsageMeter> UsageMeters => Set<UsageMeter>();
    public DbSet<QuotaPolicy> QuotaPolicies => Set<QuotaPolicy>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<FaultEvent> FaultEvents => Set<FaultEvent>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // 蛇形表名（匹配 RLS DDL 与 DB Schema §4）
        b.Entity<ApplicationUser>().ToTable("asp_net_users");
        b.Entity<ApplicationRole>().ToTable("asp_net_roles");
        b.Entity<ApplicationUserRole>().ToTable("asp_net_user_roles");
        b.Entity<IdentityUserClaim<Guid>>().ToTable("asp_net_user_claims");
        b.Entity<IdentityUserLogin<Guid>>().ToTable("asp_net_user_logins");
        b.Entity<IdentityRoleClaim<Guid>>().ToTable("asp_net_role_claims");
        b.Entity<IdentityUserToken<Guid>>().ToTable("asp_net_user_tokens");

        b.Entity<Tenant>().ToTable("tenants");
        b.Entity<Plan>().ToTable("plans");
        b.Entity<Subscription>().ToTable("subscriptions");
        b.Entity<UsageMeter>().ToTable("usage_meters");
        b.Entity<QuotaPolicy>().ToTable("quota_policies");
        b.Entity<Device>().ToTable("devices");
        b.Entity<FaultEvent>().ToTable("fault_events");
        b.Entity<OutboxMessage>().ToTable("outbox_messages");
        b.Entity<RefreshToken>().ToTable("refresh_tokens");
        b.Entity<AuditLog>().ToTable("audit_logs");

        // 枚举文本化（与 PG CHECK 约束一致）
        b.Entity<Tenant>().Property(t => t.Status).HasConversion<string>();
        b.Entity<Subscription>().Property(s => s.Status).HasConversion<string>();
        b.Entity<Device>().Property(d => d.Status).HasConversion<string>();
        b.Entity<FaultEvent>().Property(f => f.Status).HasConversion<string>();
        b.Entity<QuotaPolicy>().Property(q => q.Metric).HasConversion<string>();
        b.Entity<QuotaPolicy>().Property(q => q.Action).HasConversion<string>();

        // jsonb 列
        b.Entity<Device>().Property(d => d.Profile).HasColumnType("jsonb");
        b.Entity<OutboxMessage>().Property(o => o.Payload).HasColumnType("jsonb");
        b.Entity<AuditLog>().Property(a => a.Detail).HasColumnType("jsonb");

        // 全局查询过滤器（ITenantEntity）——RLS 是 DB 级兜底
        b.Entity<ApplicationUser>().HasQueryFilter(u => u.TenantId == _currentTenant.TenantId);
        b.Entity<ApplicationRole>().HasQueryFilter(r => r.TenantId == _currentTenant.TenantId);
        b.Entity<ApplicationUserRole>().HasQueryFilter(r => r.TenantId == _currentTenant.TenantId);
        b.Entity<Device>().HasQueryFilter(d => d.TenantId == _currentTenant.TenantId);
        b.Entity<FaultEvent>().HasQueryFilter(f => f.TenantId == _currentTenant.TenantId);
        b.Entity<Subscription>().HasQueryFilter(s => s.TenantId == _currentTenant.TenantId);
        b.Entity<UsageMeter>().HasQueryFilter(m => m.TenantId == _currentTenant.TenantId);
        b.Entity<OutboxMessage>().HasQueryFilter(o => o.TenantId == _currentTenant.TenantId);
        b.Entity<RefreshToken>().HasQueryFilter(rt => rt.TenantId == _currentTenant.TenantId);

        // 索引
        b.Entity<Tenant>().HasIndex(t => t.Slug).IsUnique();
        b.Entity<Plan>().HasIndex(p => p.Name).IsUnique();
        b.Entity<ApplicationUser>().HasIndex(u => new { u.TenantId, u.NormalizedEmail }).IsUnique();
        b.Entity<ApplicationRole>().HasIndex(r => new { r.TenantId, r.NormalizedName }).IsUnique();
        b.Entity<ApplicationUserRole>().HasIndex(r => r.TenantId);
        b.Entity<Subscription>().HasIndex(s => s.TenantId).IsUnique();
        b.Entity<Device>().HasIndex(d => new { d.TenantId, d.CreatedAt });
        b.Entity<FaultEvent>().HasIndex(f => new { f.TenantId, f.CreatedAt });
        b.Entity<FaultEvent>().HasIndex(f => new { f.TenantId, f.Status });
        b.Entity<UsageMeter>().HasIndex(m => new { m.TenantId, m.Period }).IsUnique();
        b.Entity<OutboxMessage>().HasIndex(o => o.SentAt).HasFilter("sent_at IS NULL");
        b.Entity<AuditLog>().HasIndex(a => new { a.TenantId, a.At });

        // 关系
        b.Entity<Subscription>().HasOne<Plan>().WithMany().HasForeignKey(s => s.PlanId);
        b.Entity<QuotaPolicy>().HasOne<Plan>().WithMany().HasForeignKey(q => q.PlanId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ===================== 统一响应信封 =====================
public sealed record ApiEnvelope<T>(int Code, string Message, T? Data);

public static class Api
{
    public static IResult Ok<T>(T data, string message = "") => Results.Ok(new ApiEnvelope<T>(0, message, data));
    public static IResult Ok(string message = "") => Results.Ok(new ApiEnvelope<object?>(0, message, null));
    public static IResult Fail(int code, string message) =>
        Results.Json(new ApiEnvelope<object?>(code, message, null), statusCode: HttpFor(code));

    private static int HttpFor(int code) => code switch
    {
        40000 => 400,
        40100 => 401,
        40300 => 403,
        40400 => 404,
        40900 => 409,
        42900 => 429,
        _ => 500
    };
}

// ===================== 进程内通知（SignalR 直推 / 计量）=====================
public sealed record FaultAcked(Guid TenantId, Guid FaultId, Guid AckedBy, DateTime AckedAt) : INotification;
public sealed record QuotaThresholdReached(Guid TenantId, string Metric, long Used, long Quota, decimal Pct) : INotification;
public sealed record QuotaExceeded(Guid TenantId, string Metric, long Used, long Quota) : INotification;
public sealed record TelemetryIngested(Guid TenantId, Guid DeviceId, long Points) : INotification;

// ===================== JWT 签发（含 tid 声明；ADR-104）=====================
public static class JwtIssuer
{
    public static string IssueAccessToken(string tid, string uid, string role, IConfiguration jwt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(SaasClaims.TenantId, tid),
            new(SaasClaims.UserId, uid),
            new(SaasClaims.Role, role),
        };
        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"], audience: jwt["Audience"], claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["AccessTokenMinutes"]!)),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // 派生短时 MQTT 凭据（含 tid；ADR-107）。Broker(EMQX) 经 JWT auth 校验。
    public static string IssueMqttToken(string tid, string uid, IConfiguration jwt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(SaasClaims.TenantId, tid),
            new(SaasClaims.UserId, uid),
            new("scope", "mqtt"),
        };
        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"], audience: jwt["Audience"], claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["MqttTokenMinutes"]!)),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string NewRefreshToken() => Convert.ToHexString(Guid.NewGuid().ToByteArray()) + Convert.ToHexString(Guid.NewGuid().ToByteArray());
}

// ===================== DI 扩展 =====================
public static class SaasServiceExtensions
{
    public static IServiceCollection AddSaasDb(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<ICurrentTenant, CurrentTenant>();
        services.AddScoped<TenantStampInterceptor>();
        services.AddScoped<TenantConnectionInterceptor>();

        services.AddDbContext<BaseDbContext>((sp, opt) =>
        {
            var conn = config.GetConnectionString("Default")!;
            opt.UseNpgsql(conn);
            // 拦截器必须挂到 DbContext 选项上才会生效（EF Core 要求）
            opt.AddInterceptors(
                sp.GetRequiredService<TenantStampInterceptor>(),
                sp.GetRequiredService<TenantConnectionInterceptor>());
        });
        return services;
    }

    public static IServiceCollection AddSaasAuth(this IServiceCollection services, IConfiguration config)
    {
        var jwt = config.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwt["Key"]!);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwt["Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role,
                };
                o.MapInboundClaims = false;
            });

        services.AddAuthorization(o =>
        {
            o.AddPolicy("SuperAdminOnly", p => p.RequireAssertion(ctx => ctx.User.FindFirst(SaasClaims.TenantId)?.Value == SaasTenant.Root));
            o.AddPolicy("TenantUser", p => p.RequireAssertion(ctx =>
                ctx.User.FindFirst(SaasClaims.TenantId)?.Value is { } tid && tid != SaasTenant.Root));
            o.AddPolicy("TenantAdminOnly", p => p.RequireAssertion(ctx =>
                ctx.User.FindFirst(SaasClaims.TenantId)?.Value is { } tid && tid != SaasTenant.Root &&
                ctx.User.IsInRole(SaasRoles.Admin)));
            // 分析师禁止任何写入口（AC-04）
            o.AddPolicy("NotAnalyst", p => p.RequireAssertion(ctx =>
                !ctx.User.IsInRole(SaasRoles.Analyst)));
        });

        services.AddIdentityCore<ApplicationUser>(o => { o.Password.RequiredLength = 8; o.User.RequireUniqueEmail = false; })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<BaseDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddSaasMediator(this IServiceCollection services)
    {
        services.AddMediator(); // 由 Mediator.SourceGenerator 编译期生成
        return services;
    }

    public static IServiceCollection AddSaasSignalR(this IServiceCollection services)
    {
        services.AddSignalR();
        return services;
    }

    public static IServiceCollection AddSaasRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(o =>
        {
            o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            o.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
            {
                var tid = ctx.User.FindFirst(SaasClaims.TenantId)?.Value ?? ctx.Connection.RemoteIpAddress?.ToString() ?? "anon";
                return RateLimitPartition.GetTokenBucketLimiter(tid, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 200,
                    TokensPerPeriod = 50,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                    AutoReplenishment = true,
                });
            });
        });
        return services;
    }
}

// ===================== 租户上下文中间件（注入 tid；暂停拦截写，AC-07）=====================
public static class TenantContextExtensions
{
    public static IApplicationBuilder UseTenantContext(this IApplicationBuilder app) =>
        app.Use(async (http, next) =>
        {
            var current = http.RequestServices.GetRequiredService<ICurrentTenant>();
            var tid = http.User.FindFirst(SaasClaims.TenantId)?.Value;

            if (tid == SaasTenant.Root)
            {
                current.IsRoot = true;
                current.TenantId = Guid.Empty;
            }
            else if (Guid.TryParse(tid, out var t) && t != Guid.Empty)
            {
                current.TenantId = t;
                current.IsRoot = false;
                if (http.Request.Method != HttpMethods.Get)
                {
                    var db = http.RequestServices.GetService<BaseDbContext>();
                    if (db is not null)
                    {
                        var tenant = await db.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == t);
                        if (tenant is not null && tenant.Status == TenantStatus.Suspended)
                        {
                            http.Response.StatusCode = 409; // 暂停态：写被拒（读降级只读快照）
                            await http.Response.WriteAsJsonAsync(new ApiEnvelope<object?>(40900, "租户已暂停，写操作被拒绝", null));
                            return;
                        }
                    }
                }
            }
            else
            {
                current.TenantId = Guid.Empty;
                current.IsRoot = false;
            }

            await next();
        });
}

// 当前用户辅助
public static class PrincipalExtensions
{
    public static Guid CurrentUserId(this ClaimsPrincipal p) =>
        Guid.TryParse(p.FindFirst(SaasClaims.UserId)?.Value, out var id) ? id : Guid.Empty;
    public static string CurrentTenantId(this ClaimsPrincipal p) => p.FindFirst(SaasClaims.TenantId)?.Value ?? string.Empty;
}

// ===================== 错误处理与辅助 =====================
public sealed class DomainException : Exception
{
    public int Code { get; }
    public DomainException(int code, string message) : base(message) => Code = code;
}

public static class TokenHasher
{
    public static string Hash(string token) =>
        Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}

// 端点统一异常映射（DomainException / 跨租户 / 唯一约束 / 兜底）
public static class EndpointHelpers
{
    public static async Task<IResult> Run(Func<Task<IResult>> fn)
    {
        try { return await fn(); }
        catch (DomainException e) { return Api.Fail(e.Code, e.Message); }
        catch (CrossTenantWriteException) { return Api.Fail(40300, "跨租户写入被拒绝"); }
        catch (DbUpdateException e) when (IsUniqueViolation(e)) { return Api.Fail(40900, "资源冲突：唯一约束被违反"); }
        catch (Exception) { return Api.Fail(50000, "内部服务器错误"); }
    }

    private static bool IsUniqueViolation(DbUpdateException e)
    {
        for (var ex = e.InnerException; ex is not null; ex = ex.InnerException)
            if (ex is PostgresException pg && pg.SqlState == "23505") return true;
        return false;
    }
}

// ===================== RLS 自动应用（ADR-103 ③.4；部署可选，默认关闭）=====================
// 纵深防御：即便代码误用 IgnoreQueryFilters() 或原始 SQL，RLS 仍强制租户隔离。
// rls.sql 已幂等（DROP POLICY IF EXISTS + CREATE POLICY；ENABLE ROW LEVEL SECURITY 可重跑）。
// 仅当 Saas:ApplyRlsOnStartup=true 时执行；连接角色权限不足（非迁移角色）时记录告警并跳过，不阻断启动。
// 生产连接约定：租户作用域角色 + 平台/计量用 BYPASSRLS 角色（见 rls.sql §2）。
public static class SaasRls
{
    public static async Task ApplyAsync(IServiceProvider sp, BaseDbContext db, CancellationToken ct = default)
    {
        var cfg = sp.GetRequiredService<IConfiguration>();
        if (!string.Equals(cfg["Saas:ApplyRlsOnStartup"], "true", StringComparison.OrdinalIgnoreCase))
            return;

        var sql = ResolveSql(sp.GetService<IHostEnvironment>());
        if (sql is null)
        {
            sp.GetService<ILoggerFactory>()?.CreateLogger("Saas.RLS")
              ?.LogWarning("Saas:ApplyRlsOnStartup=true 但未找到 sql/rls.sql，跳过 RLS 应用。");
            return;
        }

        try
        {
            await db.Database.ExecuteSqlRawAsync(sql, ct);
        }
        catch (Exception ex)
        {
            sp.GetService<ILoggerFactory>()?.CreateLogger("Saas.RLS")
              ?.LogError(ex, "应用 RLS 失败（连接角色权限不足？）。请由具备 ALTER/CREATE POLICY 权限的迁移角色手动执行 sql/rls.sql。");
        }
    }

    private static string? ResolveSql(IHostEnvironment? env)
    {
        var candidates = new List<string>(capacity: 5);
        if (env is not null)
            candidates.Add(Path.Combine(env.ContentRootPath, "sql", "rls.sql"));
        candidates.Add(Path.Combine(AppContext.BaseDirectory, "sql", "rls.sql"));
        candidates.Add(Path.Combine(Directory.GetCurrentDirectory(), "sql", "rls.sql"));
        candidates.Add(Path.Combine(AppContext.BaseDirectory, "..", "sql", "rls.sql"));
        candidates.Add(Path.Combine(AppContext.BaseDirectory, "..", "..", "sql", "rls.sql"));
        foreach (var c in candidates)
            if (File.Exists(c))
                return File.ReadAllText(c);
        return null;
    }
}
#endif
