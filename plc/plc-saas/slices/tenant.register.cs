// slices/tenant.register.cs — 租户开通与运营暂停/恢复（Tenant 上下文，§5.1）
// POST /tenants（匿名） · POST /ops/tenants/{id}/suspend · POST /ops/tenants/{id}/resume（超管）
#include "../infra.cs"
using System.Text.RegularExpressions;

public static class TenantRegisterApi
{
    public static void Map(RouteGroupBuilder api)
    {
        api.MapPost("/tenants", static async (TenantCreateRequest req, IMediator m) =>
            await EndpointHelpers.Run(async () =>
            {
                var r = await m.Send(new CreateTenantCommand(req.CompanyName, req.Slug, req.AdminEmail, req.AdminPassword));
                return Api.Ok(r);
            })).AllowAnonymous();

        api.MapPost("/ops/tenants/{id}/suspend", static async (Guid id, IMediator m) =>
            await EndpointHelpers.Run(async () => Api.Ok(await m.Send(new SuspendTenantCommand(id)))))
            .RequireAuthorization("SuperAdminOnly");

        api.MapPost("/ops/tenants/{id}/resume", static async (Guid id, IMediator m) =>
            await EndpointHelpers.Run(async () => Api.Ok(await m.Send(new ResumeTenantCommand(id)))))
            .RequireAuthorization("SuperAdminOnly");
    }
}

public sealed record TenantCreateRequest(string CompanyName, string Slug, string AdminEmail, string AdminPassword);
public sealed record TenantResponse(Guid Id, string Name, string Slug, string Status, DateTime CreatedAt, Guid AdminUserId);

public sealed record CreateTenantCommand(string CompanyName, string Slug, string AdminEmail, string AdminPassword) : IRequest<TenantResponse>;
public sealed class CreateTenantHandler : IRequestHandler<CreateTenantCommand, TenantResponse>
{
    private readonly BaseDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    private readonly RoleManager<ApplicationRole> _roles;
    private readonly ICurrentTenant _current;
    public CreateTenantHandler(BaseDbContext db, UserManager<ApplicationUser> users, RoleManager<ApplicationRole> roles, ICurrentTenant current)
        => (_db, _users, _roles, _current) = (db, users, roles, current);

    public async ValueTask<TenantResponse> Handle(CreateTenantCommand cmd, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cmd.CompanyName) || string.IsNullOrWhiteSpace(cmd.AdminEmail))
            throw new DomainException(40000, "企业名称与管理员邮箱必填");
        if (!Regex.IsMatch(cmd.Slug, "^[a-z0-9][a-z0-9-]{2,30}$"))
            throw new DomainException(40000, "子域格式不合法（小写字母/数字/连字符，长度 3-31）");
        if (cmd.AdminPassword.Length < 8)
            throw new DomainException(40000, "管理员密码至少 8 位");

        if (await _db.Tenants.IgnoreQueryFilters().AnyAsync(t => t.Slug == cmd.Slug, ct))
            throw new DomainException(40900, "子域已被占用，请更换");

        var tenantId = Guid.NewGuid();
        var tenant = new Tenant { Id = tenantId, Name = cmd.CompanyName, Slug = cmd.Slug, Status = TenantStatus.Active, CreatedAt = DateTime.UtcNow };

        // 受信任的开通操作：以 root 身份写入该租户作用域实体（ADR-103 兜底放行）
        _current.IsRoot = true;
        _current.TenantId = tenantId;

        foreach (var roleName in SaasRoles.All)
            await _roles.CreateAsync(new ApplicationRole(roleName) { TenantId = tenantId });

        var adminUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserName = cmd.AdminEmail,
            Email = cmd.AdminEmail,
            NormalizedEmail = cmd.AdminEmail.ToUpperInvariant(),
            Name = cmd.CompanyName + " 管理员",
        };
        var create = await _users.CreateAsync(adminUser, cmd.AdminPassword);
        if (!create.Succeeded)
            throw new DomainException(40000, "管理员账号创建失败：" + string.Join("; ", create.Errors.Select(e => e.Description)));
        await _users.AddToRoleAsync(adminUser, SaasRoles.Admin);

        var freePlan = await _db.Plans.IgnoreQueryFilters().FirstAsync(p => p.Name == SaasPlans.Free, ct);
        _db.Subscriptions.Add(new Subscription
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            PlanId = freePlan.Id,
            Status = SubscriptionStatus.Active,
            StartAt = DateTime.UtcNow,
        });
        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync(ct);

        return new TenantResponse(tenant.Id, tenant.Name, tenant.Slug, tenant.Status.ToString(), tenant.CreatedAt, adminUser.Id);
    }
}

public sealed record SuspendTenantCommand(Guid Id) : IRequest<TenantResponse>;
public sealed class SuspendTenantHandler : IRequestHandler<SuspendTenantCommand, TenantResponse>
{
    private readonly BaseDbContext _db;
    private readonly ICurrentTenant _current;
    public SuspendTenantHandler(BaseDbContext db, ICurrentTenant current) => (_db, _current) = (db, current);

    public async ValueTask<TenantResponse> Handle(SuspendTenantCommand cmd, CancellationToken ct)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == cmd.Id, ct)
            ?? throw new DomainException(40400, "租户不存在");
        _current.IsRoot = true; _current.TenantId = tenant.Id; // 超管跨租户写入放行
        tenant.Status = TenantStatus.Suspended;
        var sub = await _db.Subscriptions.FirstOrDefaultAsync(s => s.TenantId == tenant.Id, ct);
        if (sub is not null) sub.Status = SubscriptionStatus.Suspended;
        await _db.SaveChangesAsync(ct);
        return new TenantResponse(tenant.Id, tenant.Name, tenant.Slug, tenant.Status.ToString(), tenant.CreatedAt, Guid.Empty);
    }
}

public sealed record ResumeTenantCommand(Guid Id) : IRequest<TenantResponse>;
public sealed class ResumeTenantHandler : IRequestHandler<ResumeTenantCommand, TenantResponse>
{
    private readonly BaseDbContext _db;
    private readonly ICurrentTenant _current;
    public ResumeTenantHandler(BaseDbContext db, ICurrentTenant current) => (_db, _current) = (db, current);

    public async ValueTask<TenantResponse> Handle(ResumeTenantCommand cmd, CancellationToken ct)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == cmd.Id, ct)
            ?? throw new DomainException(40400, "租户不存在");
        _current.IsRoot = true; _current.TenantId = tenant.Id;
        tenant.Status = TenantStatus.Active;
        var sub = await _db.Subscriptions.FirstOrDefaultAsync(s => s.TenantId == tenant.Id, ct);
        if (sub is not null) sub.Status = SubscriptionStatus.Active;
        await _db.SaveChangesAsync(ct);
        return new TenantResponse(tenant.Id, tenant.Name, tenant.Slug, tenant.Status.ToString(), tenant.CreatedAt, Guid.Empty);
    }
}
