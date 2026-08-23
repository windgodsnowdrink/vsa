// slices/identity.login.cs — 登录与刷新（Identity 上下文，§5.2）
// POST /auth/login（匿名） · POST /auth/refresh（匿名）
// JWT 含 tid 声明；access 15min / refresh 7d（ADR-104）。超管 tid=root。
#include "../infra.cs"

public static class IdentityLoginApi
{
    public static void Map(RouteGroupBuilder api)
    {
        api.MapPost("/auth/login", static async (LoginRequest req, IMediator m) =>
            await EndpointHelpers.Run(async () => Api.Ok(await m.Send(new LoginCommand(req.Tenant, req.Email, req.Password)))))
            .AllowAnonymous();

        api.MapPost("/auth/refresh", static async (RefreshRequest req, IMediator m) =>
            await EndpointHelpers.Run(async () => Api.Ok(await m.Send(new RefreshCommand(req.RefreshToken)))))
            .AllowAnonymous();
    }
}

public sealed record LoginRequest(string Tenant, string Email, string Password);
public sealed record RefreshRequest(string RefreshToken);
public sealed record TokenResponse(
    string AccessToken, string RefreshToken, int ExpiresIn, string TokenType, string TenantId);

public sealed record LoginCommand(string Tenant, string Email, string Password) : IRequest<TokenResponse>;
public sealed class LoginHandler : IRequestHandler<LoginCommand, TokenResponse>
{
    private readonly BaseDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    private readonly IConfiguration _jwt;
    public LoginHandler(BaseDbContext db, UserManager<ApplicationUser> users, IConfiguration jwt) => (_db, _users, _jwt) = (db, users, jwt);

    public async ValueTask<TokenResponse> Handle(LoginCommand cmd, CancellationToken ct)
    {
        ApplicationUser? user;
        string tid;
        string role;

        if (cmd.Tenant == SaasTenant.Root)
        {
            user = await _db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.NormalizedEmail == cmd.Email.ToUpperInvariant(), ct);
            tid = SaasTenant.Root;
            role = SaasRoles.SuperAdmin;
        }
        else
        {
            var tenant = await _db.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Slug == cmd.Tenant, ct)
                ?? throw new DomainException(40100, "租户不存在");
            if (tenant.Status == TenantStatus.Suspended)
                throw new DomainException(40300, "租户已暂停");
            user = await _db.Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.TenantId == tenant.Id && u.NormalizedEmail == cmd.Email.ToUpperInvariant(), ct);
            tid = tenant.Id.ToString();
            var roles = user is not null ? await _users.GetRolesAsync(user) : [];
            role = roles.FirstOrDefault() ?? SaasRoles.Operator;
        }

        if (user is null || !await _users.CheckPasswordAsync(user, cmd.Password))
            throw new DomainException(40100, "账号或密码错误");

        return IssueTokens(tid, user.Id, role);
    }

    private TokenResponse IssueTokens(string tid, Guid uid, string role)
    {
        var access = JwtIssuer.IssueAccessToken(tid, uid.ToString(), role, _jwt);
        var refresh = JwtIssuer.NewRefreshToken();
        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            TenantId = tid == SaasTenant.Root ? Guid.Empty : Guid.Parse(tid),
            UserId = uid,
            TokenHash = TokenHasher.Hash(refresh),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
        });
        _db.SaveChanges();
        return new TokenResponse(access, refresh, 900, "Bearer", tid);
    }
}

public sealed record RefreshCommand(string RefreshToken) : IRequest<TokenResponse>;
public sealed class RefreshHandler : IRequestHandler<RefreshCommand, TokenResponse>
{
    private readonly BaseDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    private readonly IConfiguration _jwt;
    public RefreshHandler(BaseDbContext db, UserManager<ApplicationUser> users, IConfiguration jwt) => (_db, _users, _jwt) = (db, users, jwt);

    public async ValueTask<TokenResponse> Handle(RefreshCommand cmd, CancellationToken ct)
    {
        var hash = TokenHasher.Hash(cmd.RefreshToken);
        var rt = await _db.RefreshTokens.IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.TokenHash == hash, ct)
            ?? throw new DomainException(40100, "刷新令牌无效");
        if (rt.RevokedAt is not null || rt.ExpiresAt < DateTime.UtcNow)
            throw new DomainException(40100, "刷新令牌已失效");

        var user = await _db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == rt.UserId, ct)
            ?? throw new DomainException(40100, "用户不存在");
        var roles = await _users.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? SaasRoles.Operator;
        var tid = user.TenantId == Guid.Empty ? SaasTenant.Root : user.TenantId.ToString();

        // 轮换刷新令牌
        rt.RevokedAt = DateTime.UtcNow;
        var newRefresh = JwtIssuer.NewRefreshToken();
        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            TenantId = user.TenantId,
            UserId = user.Id,
            TokenHash = TokenHasher.Hash(newRefresh),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
        });
        _db.SaveChanges();

        var access = JwtIssuer.IssueAccessToken(tid, user.Id.ToString(), role, _jwt);
        return new TokenResponse(access, newRefresh, 900, "Bearer", tid);
    }
}
