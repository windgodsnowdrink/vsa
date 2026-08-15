// slices/ops.roles.cs — 平台运营：全局角色与权限矩阵（§5.5，超管）
// 四角色权限定义（分析师强制只读，AC-04）。
#include "../infra.cs"

public static class OpsRolesApi
{
    public static void Map(RouteGroupBuilder api)
    {
        api.MapGet("/ops/roles", static async (IMediator m) =>
            await EndpointHelpers.Run(async () => Api.Ok(await m.Send(new GetRoleMatrixQuery()))))
            .RequireAuthorization("SuperAdminOnly");
    }
}

public sealed record RolePermission(string Role, List<string> Permissions);
public sealed record RoleMatrixResponse(List<RolePermission> Roles);

public sealed record GetRoleMatrixQuery : IRequest<RoleMatrixResponse>;
public sealed class GetRoleMatrixHandler : IRequestHandler<GetRoleMatrixQuery, RoleMatrixResponse>
{
    public ValueTask<RoleMatrixResponse> Handle(GetRoleMatrixQuery _, CancellationToken ct) => ValueTask.FromResult(new RoleMatrixResponse(new List<RolePermission>
    {
        new(SaasRoles.Admin, ["tenant.read", "device.readwrite", "fault.ack", "member.manage", "plan.change"]),
        new(SaasRoles.Analyst, ["tenant.read", "device.read", "fault.read"]), // 只读，无写入口
        new(SaasRoles.Operator, ["tenant.read", "device.readwrite", "fault.ack"]),
        new(SaasRoles.DeviceEng, ["tenant.read", "device.readwrite", "fault.read"]),
    }));
}
