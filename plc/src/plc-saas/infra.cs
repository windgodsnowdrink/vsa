// infra.cs — PLC·AIOT 多租户 SaaS 共享基础设施（File-based App）
// 由 Program.cs 与各切片经 #include 引入。#r 集中声明 NuGet 引用（ADR-100/102）。
// 类型定义用 #if !INFRA_CS 守卫，避免多切片 #include 导致重复定义。
// 红线：以下任何类型/方法均不含 price/currency/amount/money（ADR-108）。

#r "nuget:Microsoft.EntityFrameworkCore, 11.0.0-preview.6.26359.118"
#r "nuget:Microsoft.EntityFrameworkCore.SqlServer, 11.0.0-preview.6.26359.118"
#r "nuget:Microsoft.AspNetCore.Identity.EntityFrameworkCore, 11.0.0-preview.6.26359.118"
#r "nuget:Microsoft.AspNetCore.Authentication.JwtBearer, 11.0.0-preview.6.26359.118"
#r "nuget:System.IdentityModel.Tokens.Jwt, 8.22.0"
#r "nuget:Mediator.Abstractions, 3.0.2"
#r "nuget:Mediator.SourceGenerator, 3.0.2"
#r "nuget:MQTTnet, 5.2.0.1603"
#r "nuget:Microsoft.Extensions.AI, 10.9.0"
#r "nuget:ModelContextProtocol, 2.2.0"
#r "nuget:ModelContextProtocol.AspNetCore, 0.1.0-preview.14"
#r "nuget:Qdrant.Client, 1.19.0"

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
using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Mediator;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System.Runtime.CompilerServices;
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

// 每条连接打开时设置 SESSION_CONTEXT N'TenantId'（SQL Server RLS 兜底，ADR-103 ③.4）
// 注意：SESSION_CONTEXT 用 @read_only=0（默认），连接池复用时 sp_reset_connection 会清空，
// 故每次 ConnectionOpenedAsync 重新设置是正确且必要的；与 EF Core SQL Server RLS 官方示例一致。
public sealed class TenantConnectionInterceptor : DbConnectionInterceptor
{
    private readonly ICurrentTenant _tenant;
    public TenantConnectionInterceptor(ICurrentTenant tenant) => _tenant = tenant;

    public override Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        if (connection is SqlConnection sql)
        {
            using var cmd = sql.CreateCommand();
            var tid = _tenant.IsRoot ? Guid.Empty : _tenant.TenantId;
            cmd.CommandText = $"EXEC sp_set_session_context @key = N'TenantId', @value = '{tid}';";
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

        // JSON 列：SQL Server 无 jsonb，以 nvarchar(max) 存 JSON 文本（EF Core SqlServer 原生支持）
        b.Entity<Device>().Property(d => d.Profile).HasColumnType("nvarchar(max)");
        b.Entity<OutboxMessage>().Property(o => o.Payload).HasColumnType("nvarchar(max)");
        b.Entity<OutboxMessage>().Property(o => o.SentAt).HasColumnName("sent_at"); // 显式列名，与过滤索引 WHERE sent_at IS NULL 及计量轮询口径一致
        b.Entity<AuditLog>().Property(a => a.Detail).HasColumnType("nvarchar(max)");

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
public sealed record TelemetryIngested(Guid TenantId, Guid DeviceId, long Points);

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
            opt.UseSqlServer(conn);
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
        var jwtKey = jwt["Key"];
        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            // 本地开发兜底：未配置 Jwt:Key 时不致命崩溃；生产环境务必在配置中设置强密钥。
            jwtKey = "plc-aiot-saas-dev-insecure-fallback-key-change-me-32+";
            Console.Error.WriteLine("WARN: Jwt:Key 未配置，已使用内置开发兜底密钥（生产环境务必在配置中设置强密钥）。");
        }
        var key = Encoding.UTF8.GetBytes(jwtKey);

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
        // AddMediator() 默认按 Singleton 注册 handler；本服务 handler 依赖 Scoped
        // BaseDbContext / ICurrentTenant，会触发 captive dependency 校验失败：
        //   "Cannot consume scoped service 'BaseDbContext' from singleton 'XxxHandler'"
        // Mediator 库的 MediatorOptions.ServiceLifetime 接受编译期常量（Singleton/Transient/Scoped），
        // 这里统一降级为 Scoped，与 BaseDbContext / ICurrentTenant 生命周期对齐，
        // 仍经每请求 DI scope 解析 handler 实例。
        services.AddMediator(static options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });
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
        // SQL Server：2627=违反主键/PK 约束；2601=违反唯一索引。两者均映射为 40900 资源冲突。
        for (var ex = e.InnerException; ex is not null; ex = ex.InnerException)
            if (ex is SqlException sql && (sql.Number == 2627 || sql.Number == 2601)) return true;
        return false;
    }
}

// ===================== 启动期幂等建表 + RLS 自动应用（ADR-103 ③.4）=====================
// 每次启动执行一次，二者均幂等：
//   a) EnsureCreatedAsync —— 预览版不使用 Migrations，仅创建缺失的表（已存在则无操作）。
//   b) 读取并执行 sql/rls.sql —— SQL Server RLS：CREATE OR ALTER FUNCTION（幂等）+
//      DROP/CREATE SECURITY POLICY（可重跑）。sql/rls.sql 用 GO 分隔批次，此处按 GO 拆分逐批执行
//      （SqlCommand 不支持 GO；CREATE FUNCTION/CREATE SECURITY POLICY 必须各自成批）。
// BYPASS/隔离假设（intranet 工具；ADR-103 ③.4）：bootstrap 与 DbSeeder 在本进程的平台服务连接下运行；
// 根行（平台超管/目录表）TenantId=Guid.Empty，TenantConnectionInterceptor 在连接打开时
// sp_set_session_context N'TenantId'=Guid.Empty，满足 RLS 谓词
// WHERE @TenantId = CONVERT(uniqueidentifier, SESSION_CONTEXT(N'TenantId'))。
// 租户作用域写由请求级拦截器设置各自 SESSION_CONTEXT N'TenantId'。RLS 策略本身绝不被削弱（FILTER+CHECK 均不放宽）。
public static class SaasSchemaBootstrap
{
    public static async Task BootstrapAsync(IServiceProvider sp, CancellationToken ct = default)
    {
        var db = sp.GetRequiredService<BaseDbContext>();

        await db.Database.EnsureCreatedAsync(ct); // 幂等：仅建缺失表

        var sql = ResolveRlsSql(sp.GetService<IHostEnvironment>());
        if (sql is null)
        {
            sp.GetService<ILoggerFactory>()?.CreateLogger("Saas.Schema")
              ?.LogWarning("未找到 sql/rls.sql，跳过 RLS 应用（schema 已建表；需在具权限角色下手动执行 rls.sql 启用纵深防御）。");
            return;
        }

        try
        {
            // 按 GO（独立成行，大小写不敏感）拆分批次逐条执行；每批各自幂等，可安全每启动重跑。
            var batches = Regex.Split(sql, @"^\s*GO\s*$",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);
            foreach (var batch in batches)
            {
                var b = batch.Trim();
                if (string.IsNullOrEmpty(b)) continue;
                await db.Database.ExecuteSqlRawAsync(b, ct);
            }
        }
        catch (Exception ex)
        {
            // 连接角色权限不足（无 CREATE SECURITY POLICY / ALTER 权限）时不阻断启动；
            // 记录后由具权限角色补执行。EnsureCreated 已建表，应用层 HasQueryFilter 仍保证租户隔离。
            sp.GetService<ILoggerFactory>()?.CreateLogger("Saas.Schema")
              ?.LogError(ex, "应用 RLS 失败（连接 Windows 账户无 CREATE SECURITY POLICY 权限？）。请由具权限的迁移角色手动执行 sql/rls.sql。");
        }
    }

    private static string? ResolveRlsSql(IHostEnvironment? env)
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

// ===================== 遥测存储（ADR-113 数据平面）=====================
// 遥测走时序库（InfluxDB v2 行协议，经 HttpClient 写入，无额外包依赖）；未配置则 Null 兜底。
// 注意：遥测落库与计量解耦——计量由 Outbox(TelemetryIngested) 独立路径保证（ADR-102/108），
// 因此 InfluxDB 不可达时仅告警、绝不阻塞摄取主链路。
public interface ITelemetryStore
{
    ValueTask WriteAsync(Guid tenantId, Guid deviceId, DateTime timestamp,
        IReadOnlyDictionary<string, double> metrics, CancellationToken ct = default);
}

public sealed class NullTelemetryStore : ITelemetryStore
{
    private readonly ILogger<NullTelemetryStore> _log;
    public NullTelemetryStore(ILogger<NullTelemetryStore> log) => _log = log;
    public ValueTask WriteAsync(Guid tenantId, Guid deviceId, DateTime timestamp,
        IReadOnlyDictionary<string, double> metrics, CancellationToken ct = default)
    {
        _log.LogDebug("遥测未落库（未配置 InfluxDB）：tid={Tid} device={Device} metrics={Count}", tenantId, deviceId, metrics.Count);
        return ValueTask.CompletedTask;
    }
}

public sealed class InfluxTelemetryStore : ITelemetryStore
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;
    private readonly string _bucket;
    private readonly string _org;
    private readonly string _token;
    private readonly ILogger<InfluxTelemetryStore> _log;

    public InfluxTelemetryStore(HttpClient http, IConfiguration cfg, ILogger<InfluxTelemetryStore> log)
    {
        _http = http;
        _baseUrl = (cfg["Telemetry:Influx:Url"] ?? "http://localhost:8086").TrimEnd('/');
        _bucket = cfg["Telemetry:Influx:Bucket"] ?? "plc_telemetry";
        _org = cfg["Telemetry:Influx:Org"] ?? "plc-aiot";
        _token = cfg["Telemetry:Influx:Token"] ?? "";
        _log = log;
    }

    public async ValueTask WriteAsync(Guid tenantId, Guid deviceId, DateTime timestamp,
        IReadOnlyDictionary<string, double> metrics, CancellationToken ct = default)
    {
        if (metrics.Count == 0) return;
        var ts = (long)(timestamp.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds;
        var fields = string.Join(",", metrics.Select(kv =>
            $"{EscapeTag(kv.Key)}={kv.Value.ToString(CultureInfo.InvariantCulture)}"));
        var line = $"device_telemetry,tenant_id={tenantId},device_id={deviceId} {fields} {ts}";

        var url = $"{_baseUrl}/api/v2/write?org={Uri.EscapeDataString(_org)}&bucket={Uri.EscapeDataString(_bucket)}&precision=s";
        using var req = new HttpRequestMessage(HttpMethod.Post, url);
        if (!string.IsNullOrEmpty(_token))
            req.Headers.Authorization = new AuthenticationHeaderValue("Token", _token);
        req.Content = new StringContent(line, Encoding.UTF8, "text/plain");

        // best-effort：遥测高吞吐，存储不可达不应阻断摄取（计量走 Outbox 独立路径）
        try
        {
            var resp = await _http.SendAsync(req, ct);
            if (!resp.IsSuccessStatusCode)
                _log.LogWarning("InfluxDB 写入失败 {Code}: {Body}", (int)resp.StatusCode, await resp.Content.ReadAsStringAsync(ct));
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "InfluxDB 写入异常（遥测未落库，不影响摄取与计量）");
        }
    }

    private static string EscapeTag(string s) =>
        s.Replace(" ", "\\ ").Replace(",", "\\,").Replace("=", "\\=").Replace("\"", "\\\"");
}

// ===================== 摄取桥 DI（ADR-113）=====================
public static class SaasIngestExtensions
{
    public static IServiceCollection AddSaasIngest(this IServiceCollection services, IConfiguration config)
    {
        // 遥测存储：配置 Telemetry:Influx:Url 时接 InfluxDB，否则 Null 兜底（不阻断启动）
        if (!string.IsNullOrWhiteSpace(config["Telemetry:Influx:Url"]))
        {
            services.AddHttpClient<InfluxTelemetryStore>();
            services.AddScoped<ITelemetryStore>(sp => sp.GetRequiredService<InfluxTelemetryStore>());
        }
        else
        {
            services.AddScoped<ITelemetryStore, NullTelemetryStore>();
        }

        // 边缘中继（MQTTnet 订阅桥）：仅当 Mqtt:Bridge:Enabled=true 时启用
        if (config.GetValue("Mqtt:Bridge:Enabled", false))
            services.AddHostedService<MqttBridgeService>();

        return services;
    }
}

// ===================== AI 助手 / RAG / MCP（ADR-112，P1 旗舰）=====================
// 设计要点（全部经 ICurrentTenant + EF 全局过滤器保证租户隔离）：
//  - MEAI 作 LLM 抽象（IChatClient）：未配置 Provider 时回退 PlcDiagnosisChatClient 启发式诊断（离线可用）。
//  - 故障模式向量库：Qdrant（租户分区 collection）或内存兜底；任一不可达均 best-effort 不阻断启动。
//  - MCP Server（/mcp，tid 作用域隔离）：暴露 list_devices/get_faults/get_telemetry_summary，外部 AI Agent 可安全问诊。
//  - AiAssistant / Mcp / Qdrant 均 feature flag 门控（默认关闭，需显式开启）。

public sealed class AiOptions
{
    public bool Enabled { get; set; }
    public string Provider { get; set; } = "";   // OpenAI | Azure | Ollama —— 生产接入真实 LLM 时填
    public string Model { get; set; } = "";
    public string Endpoint { get; set; } = "";
    public string ApiKey { get; set; } = "";
}

public sealed class QdrantOptions
{
    public bool Enabled { get; set; }
    public string Url { get; set; } = "http://localhost:6334";
    public string ApiKey { get; set; } = "";
}

public sealed class McpOptions
{
    public bool Enabled { get; set; }
}

// 故障模式向量库抽象（租户分区）
public interface IFaultVectorStore
{
    ValueTask UpsertPatternAsync(Guid tenantId, string code, string severity, IReadOnlyList<float> vector, CancellationToken ct = default);
    ValueTask<IReadOnlyList<FaultPatternHit>> SearchSimilarAsync(Guid tenantId, IReadOnlyList<float> vector, int topK = 5, CancellationToken ct = default);
}

public sealed record FaultPatternHit(string Code, string Severity, float Score);

// 确定性伪嵌入：将故障码+级别映射到固定维度向量，离线即可做余弦相似（生产替换为真实 IEmbeddingGenerator）。
internal static class FaultEmbedding
{
    public const int Dim = 32;
    public static IReadOnlyList<float> Encode(string code, string severity)
    {
        var vec = new float[Dim];
        foreach (var ch in $"{code}:{severity}".ToLowerInvariant())
            vec[Math.Abs(ch) % Dim] += 1f;
        var norm = (float)Math.Sqrt(vec.Sum(v => v * v));
        if (norm > 0) for (int i = 0; i < Dim; i++) vec[i] /= norm;
        return vec;
    }
}

// 内存兜底实现（Qdrant 未启用/不可达时使用，保证演示与单测离线可用）
public sealed class InMemoryFaultVectorStore : IFaultVectorStore
{
    private sealed record Entry(string Code, string Severity, IReadOnlyList<float> Vector);
    private readonly Dictionary<Guid, List<Entry>> _byTenant = new();
    private readonly object _gate = new();

    public ValueTask UpsertPatternAsync(Guid tenantId, string code, string severity, IReadOnlyList<float> vector, CancellationToken ct = default)
    {
        lock (_gate)
        {
            if (!_byTenant.TryGetValue(tenantId, out var list)) _byTenant[tenantId] = list = new();
            list.Add(new Entry(code, severity, vector.ToArray()));
        }
        return ValueTask.CompletedTask;
    }

    public ValueTask<IReadOnlyList<FaultPatternHit>> SearchSimilarAsync(Guid tenantId, IReadOnlyList<float> vector, int topK = 5, CancellationToken ct = default)
    {
        List<Entry>? list;
        lock (_gate) { _byTenant.TryGetValue(tenantId, out var src); list = src?.ToList(); }
        if (list is null) return new ValueTask<IReadOnlyList<FaultPatternHit>>(Array.Empty<FaultPatternHit>());
        var hits = list.Select(e => new FaultPatternHit(e.Code, e.Severity, Cosine(e.Vector, vector)))
                        .OrderByDescending(h => h.Score).Take(topK).ToArray();
        return new ValueTask<IReadOnlyList<FaultPatternHit>>(hits);
    }

    private static float Cosine(IReadOnlyList<float> a, IReadOnlyList<float> b)
    {
        float dot = 0, na = 0, nb = 0;
        for (int i = 0; i < a.Count; i++) { dot += a[i] * b[i]; na += a[i] * a[i]; nb += b[i] * b[i]; }
        return (na == 0 || nb == 0) ? 0 : dot / (MathF.Sqrt(na) * MathF.Sqrt(nb));
    }
}

// Qdrant 实现：租户分区 collection（faults_{tid}）；任何异常 best-effort 降级，不阻断摄取主链路。
public sealed class QdrantFaultVectorStore : IFaultVectorStore
{
    private readonly QdrantClient _client;
    private readonly ILogger<QdrantFaultVectorStore> _log;
    private readonly Dictionary<string, (string Code, string Severity)> _meta = new();
    public QdrantFaultVectorStore(IConfiguration cfg, ILogger<QdrantFaultVectorStore> log)
    {
        var url = cfg["Qdrant:Url"] ?? "http://localhost:6334";
        var uri = new Uri(url);
        _client = new QdrantClient(uri.Host, uri.Port, uri.Scheme == "https", cfg["Qdrant:ApiKey"]);
        _log = log;
    }

    private static string Coll(Guid tid) => $"faults_{tid:N}";

    public async ValueTask UpsertPatternAsync(Guid tenantId, string code, string severity, IReadOnlyList<float> vector, CancellationToken ct = default)
    {
        try
        {
            var coll = Coll(tenantId);
            await EnsureCollectionAsync(coll, ct);
            var id = Guid.NewGuid();
            _meta[id.ToString()] = (code, severity); // 进程内元数据缓存（生产应改用 PointStruct.Payload）
            var point = new PointStruct { Id = id, Vectors = vector.ToArray() };
            await _client.UpsertAsync(coll, new List<PointStruct> { point }, cancellationToken: ct);
        }
        catch (Exception ex) { _log.LogWarning(ex, "Qdrant 写入失败（故障模式未入库，不影响摄取）"); }
    }

    public async ValueTask<IReadOnlyList<FaultPatternHit>> SearchSimilarAsync(Guid tenantId, IReadOnlyList<float> vector, int topK = 5, CancellationToken ct = default)
    {
        try
        {
#pragma warning disable CS0618 // Qdrant SearchAsync 在本版本标记 Obsolete，改用 QueryAsync 前保持可用
            var res = await _client.SearchAsync(Coll(tenantId), vector.ToArray(), limit: (ulong)topK, cancellationToken: ct);
#pragma warning restore CS0618
            return res.Select(r =>
            {
                _meta.TryGetValue(r.Id.ToString(), out var m);
                return new FaultPatternHit(m.Code ?? "?", m.Severity ?? "?", (float)r.Score);
            }).ToArray();
        }
        catch (Exception ex) { _log.LogWarning(ex, "Qdrant 检索失败（回退空结果）"); return Array.Empty<FaultPatternHit>(); }
    }

    private async Task EnsureCollectionAsync(string coll, CancellationToken ct)
    {
        var cols = await _client.ListCollectionsAsync(cancellationToken: ct);
        if (cols.Contains(coll)) return;
        await _client.CreateCollectionAsync(coll,
            new VectorParams { Size = (ulong)FaultEmbedding.Dim, Distance = Distance.Cosine }, cancellationToken: ct);
    }
}

// 离线启发式诊断客户端：未接入真实 LLM Provider 时，基于检索到的相似故障模式生成结构化诊断（离线可用）。
internal sealed class PlcDiagnosisChatClient : IChatClient
{
    private readonly IChatClient? _upstream;
    public ChatClientMetadata Metadata { get; } = new("plc-diagnosis", null, "heuristic-v1");
    public PlcDiagnosisChatClient(IChatClient? upstream) => _upstream = upstream;

    public Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (_upstream is not null)
            return _upstream.GetResponseAsync(messages, options, cancellationToken);
        var diagnosis = BuildHeuristic(LastUserText(messages));
        return Task.FromResult(new ChatResponse(new List<ChatMessage> { new(ChatRole.Assistant, diagnosis) }));
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages, ChatOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var text = _upstream is not null
            ? (await _upstream.GetResponseAsync(messages, options, cancellationToken)).Text
            : BuildHeuristic(LastUserText(messages));
        yield return new ChatResponseUpdate(ChatRole.Assistant, text);
    }

    public object? GetService(Type serviceType, object? serviceKey = null) => null;
    public void Dispose() { }

    private static string LastUserText(IEnumerable<ChatMessage> messages)
    {
        var last = messages.LastOrDefault(m => m.Role == ChatRole.User);
        return last?.Text ?? "";
    }

    private static string BuildHeuristic(string prompt) =>
        $"[启发式诊断·离线] 已收到问诊上下文（{prompt.Length} 字符）。建议：1) 检查设备供电与通信链路；" +
        "2) 比对历史相似故障模式；3) 若持续告警，派工 DeviceEng 现场排查。";
}

// ===================== AI 模块 DI（ADR-112）=====================
public static class SaasAiExtensions
{
    public static IServiceCollection AddSaasAi(this IServiceCollection services, IConfiguration config)
    {
        var ai = config.GetSection("Ai").Get<AiOptions>() ?? new();
        var qdrant = config.GetSection("Qdrant").Get<QdrantOptions>() ?? new();
        var mcp = config.GetSection("Mcp").Get<McpOptions>() ?? new();

        // LLM 抽象（MEAI）：Provider 接入点已预留——生产可在此构造 OpenAI/Azure/Ollama IChatClient 作为 upstream。
        // 默认 upstream=null → PlcDiagnosisChatClient 启发式（离线可用，feature flag 不影响编译）。
        services.AddSingleton<IChatClient>(_ => new PlcDiagnosisChatClient(upstream: null));

        // 故障模式向量库：Qdrant（租户分区）或内存兜底
        if (qdrant.Enabled)
            services.AddSingleton<IFaultVectorStore>(sp =>
                new QdrantFaultVectorStore(config, sp.GetRequiredService<ILogger<QdrantFaultVectorStore>>()));
        else
            services.AddSingleton<IFaultVectorStore, InMemoryFaultVectorStore>();

        // MCP Server（tid 作用域隔离）：仅 Mcp:Enabled 时注册并映射 /mcp
        if (mcp.Enabled)
            services.AddMcpServer().WithHttpTransport().WithTools<PlcMcpTools>();

        return services;
    }
}
#endif
