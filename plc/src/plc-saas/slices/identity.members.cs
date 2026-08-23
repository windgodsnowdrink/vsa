// slices/identity.members.cs — 租户内成员与角色（Identity 上下文，§5.2）
// GET/POST /tenant/members（管理员） · POST /tenant/members/{id}/role（管理员）
// 四角色：Admin/Analyst/Operator/DeviceEng；分析师只读（AC-04）。
#include "../infra.cs"
using System.Security.Cryptography;

public static class IdentityMembersApi
{
    public static void Map(RouteGroupBuilder api)
    {
        api.MapGet("/tenant/members", static async (IMediator m) =>
            await EndpointHelpers.Run(async () => Api.Ok(await m.Send(new ListMembersQuery())))).RequireAuthorization("TenantUser");

        api.MapPost("/tenant/members", static async (MemberAddRequest req, IMediator m) =>
            await EndpointHelpers.Run(async () => Api.Ok(await m.Send(new AddMemberCommand(req.Email, req.Name, req.Role)), "成员已添加")))
            .RequireAuthorization("TenantAdminOnly");

        api.MapPost("/tenant/members/{id}/role", static async (Guid id, RoleUpdateRequest req, IMediator m) =>
            await EndpointHelpers.Run(async () => Api.Ok(await m.Send(new UpdateMemberRoleCommand(id, req.Role)))))
            .RequireAuthorization("TenantAdminOnly");
    }
}

public sealed record MemberAddRequest(string Email, string Name, string Role);
public sealed record RoleUpdateRequest(string Role);
public sealed record MemberResponse(Guid Id, string Email, string Name, string Role);

public sealed record ListMembersQuery : IRequest<List<MemberResponse>>;
public sealed class ListMembersHandler : IRequestHandler<ListMembersQuery, List<MemberResponse>>
{
    private readonly BaseDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    public ListMembersHandler(BaseDbContext db, UserManager<ApplicationUser> users) => (_db, _users) = (db, users);

    public async ValueTask<List<MemberResponse>> Handle(ListMembersQuery _, CancellationToken ct)
    {
        var users = await _db.Users.ToListAsync(ct); // 全局过滤器已限定本租户
        var result = new List<MemberResponse>();
        foreach (var u in users)
        {
            var roles = await _users.GetRolesAsync(u);
            result.Add(new MemberResponse(u.Id, u.Email ?? "", u.Name ?? "", roles.FirstOrDefault() ?? SaasRoles.Operator));
        }
        return result;
    }
}

public sealed record AddMemberCommand(string Email, string Name, string Role) : IRequest<MemberResponse>;
public sealed class AddMemberHandler : IRequestHandler<AddMemberCommand, MemberResponse>
{
    private readonly BaseDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    private readonly ICurrentTenant _current;
    public AddMemberHandler(BaseDbContext db, UserManager<ApplicationUser> users, ICurrentTenant current)
        => (_db, _users, _current) = (db, users, current);

    public async ValueTask<MemberResponse> Handle(AddMemberCommand cmd, CancellationToken ct)
    {
        if (!SaasRoles.All.Contains(cmd.Role)) throw new DomainException(40000, "角色不在允许范围（Admin/Analyst/Operator/DeviceEng）");
        if (await _db.Users.AnyAsync(u => u.NormalizedEmail == cmd.Email.ToUpperInvariant(), ct))
            throw new DomainException(40900, "该邮箱成员已存在");

        // 开发期：生成临时口令；生产应走邀请邮件 + 一次性设置链接（不在本期范围）
        var pwd = Convert.ToHexString(RandomNumberGenerator.GetBytes(9)) + "Aa1!";
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            TenantId = _current.TenantId,
            UserName = cmd.Email,
            Email = cmd.Email,
            NormalizedEmail = cmd.Email.ToUpperInvariant(),
            Name = cmd.Name,
        };
        var create = await _users.CreateAsync(user, pwd);
        if (!create.Succeeded)
            throw new DomainException(40000, "成员创建失败：" + string.Join("; ", create.Errors.Select(e => e.Description)));
        await _users.AddToRoleAsync(user, cmd.Role);
        return new MemberResponse(user.Id, user.Email ?? "", user.Name ?? "", cmd.Role);
    }
}

public sealed record UpdateMemberRoleCommand(Guid Id, string Role) : IRequest<MemberResponse>;
public sealed class UpdateMemberRoleHandler : IRequestHandler<UpdateMemberRoleCommand, MemberResponse>
{
    private readonly BaseDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    public UpdateMemberRoleHandler(BaseDbContext db, UserManager<ApplicationUser> users) => (_db, _users) = (db, users);

    public async ValueTask<MemberResponse> Handle(UpdateMemberRoleCommand cmd, CancellationToken ct)
    {
        if (!SaasRoles.All.Contains(cmd.Role)) throw new DomainException(40000, "角色不在允许范围");
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == cmd.Id, ct)
            ?? throw new DomainException(40400, "成员不存在");
        var cur = await _users.GetRolesAsync(user);
        await _users.RemoveFromRolesAsync(user, cur);
        await _users.AddToRoleAsync(user, cmd.Role);
        return new MemberResponse(user.Id, user.Email ?? "", user.Name ?? "", cmd.Role);
    }
}
