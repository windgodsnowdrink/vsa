#:sdk Microsoft.NET.Sdk.Web
#:package WolverineFx@4.2.0
#:package WolverineFx.Marten@4.2.0
#:package WolverineFx.RDBMS@4.2.0
#:package WolverineFx.Postgresql@4.2.0
#:package WolverineFx.FluentValidation@4.2.0
#:package WolverineFx.Http@4.2.0
#:package WolverineFx.RabbitMQ@4.2.0
#:package WolverineFx.AzureServiceBus@4.2.0
#:package WolverineFx.Http.FluentValidation@4.2.0
#:package WolverineFx.Http.Marten@4.2.0
#:package WolverineFx.AmazonSqs@4.2.0
#:package WolverineFx.EntityFrameworkCore@4.2.0
#:package WolverineFx.SqlServer@4.2.0
#:package WolverineFx.Kafka@4.2.0
#:package WolverineFx.MemoryPack@4.2.0
#:package WolverineFx.MessagePack@4.2.0
#:package WolverineFx.MQTT@4.2.0
#:package WolverineFx.Pubsub@4.2.0
#:package WolverineFx.Pulsar@4.2.0
#:package WolverineFx.RavenDb@4.2.0
#:package Microsoft.CodeAnalysis.Common@4.14.0
#:package Microsoft.CodeAnalysis.Workspaces.Common@4.14.0
#:package Swashbuckle.AspNetCore@9.0.3
#:package Swashbuckle.AspNetCore.Swagger@9.0.3
#:package Swashbuckle.AspNetCore.SwaggerGen@9.0.3
#:package Swashbuckle.AspNetCore.SwaggerUI@9.0.3
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property GenerateJsonSourceGeneration=true

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Contracts.Messages;

namespace Contracts.Messages
{
    using Wolverine.Attributes;
    using System;

    // 所有 Command/Query 加入 InitiatedByUserId，在每个微服务的 Handler 里验证 RBAC（例如只有 admin 可以绑定）
    // 给每条消息加 TenantId，在 Handler 里过滤/校验对应租户的资源
    /// <summary>
    /// Device 被创建或更新
    /// </summary>
    public sealed record DeviceCreated(Guid DeviceId, string Serial, DateTimeOffset CreatedAt);
    public sealed record DeviceUpdated(
        Guid DeviceId,
        string? Serial = null,
        bool? IsBound = null,
        DateTimeOffset? UpdatedAt = null);

    /// <summary>
    /// User 被创建或更新
    /// </summary>
    public sealed record UserCreated(Guid UserId, string Name, DateTimeOffset CreatedAt);
    public sealed record UserUpdated(Guid UserId, string? Name = null, DateTimeOffset? UpdatedAt = null);

    // <summary>绑定 / 解绑 事件（**UserDevice** 负责订阅）</summary>
    [Topic("user-device-bind")]
    public sealed record DeviceBoundToUser(Guid DeviceId, Guid UserId, DateTimeOffset BoundAt);

    [Topic("user-device-unbind")]
    public sealed record DeviceUnboundFromUser(Guid DeviceId, Guid UserId, DateTimeOffset UnboundAt);

    /// <summary>查询（Request‑Response）</summary>
    [MessageIdentity("QueryUserDevices")]           // 用来做幂等、日志追踪
    public sealed record QueryUserDevices(Guid UserId);

    public sealed record QueryDeviceUsers(Guid DeviceId);

    /// <summary>
    /// 设备绑定到用户的事件（发布/订阅）
    /// </summary>
    [Topic("user-device")]                     // 可选：指定 Topic（跨进程时有用）
    public sealed record UserDeviceAdded(
        Guid UserId,
        Guid DeviceId,
        DateTimeOffset AddedAt
    );

    // 删除事件
    [Topic("user-device")]
    public sealed record UserDeviceRemoved(
        Guid UserId,
        Guid DeviceId,
        DateTimeOffset RemovedAt
    );

    [MessageIdentity("GetUserDevices")]        // 用于去重、幂等（若需要）
    public sealed record GetUserDevices(Guid UserId);

    // 查询返回值
    public sealed record UserDevicesResponse(
        Guid UserId,
        IReadOnlyList<Guid> DeviceIds);

    public sealed record DeviceUsersResponse(Guid DeviceId, IReadOnlyList<Guid> UserIds);

    /// <summary>请求 Device 服务把设备绑定到用户</summary>
    public sealed record BindDeviceCommand(
        Guid DeviceId,
        Guid UserId,
        Guid CorrelationId,                 // 用来追踪 Saga
        DateTimeOffset RequestTime);

    public sealed record BindDeviceResponse(
        Guid DeviceId,
        Guid UserId,
        bool Success,
        string? ErrorMessage,
        Guid CorrelationId);

    public sealed record UnbindDeviceCommand(
        Guid DeviceId,
        Guid UserId,
        Guid CorrelationId,
        DateTimeOffset RequestTime);

    public sealed record UnbindDeviceResponse(
        Guid DeviceId,
        Guid UserId,
        bool Success,
        string? ErrorMessage,
        Guid CorrelationId);

    public sealed record UserExistsQuery(
        Guid UserId,
        Guid CorrelationId,
        DateTimeOffset RequestTime);

    public sealed record UserExistsResponse(
        bool Success,
        string? ErrorMessage,
        Guid CorrelationId);

    /// <summary>
    /// 当业务层（Device 服务）把 Device‑User 关联写入数据库后发出的事件
    /// 必须包含 SagaId（CorrelationId），以便 Saga 能定位自己
    /// </summary>
    public sealed record UserDeviceSaved(
        Guid CorrelationId,   // 对应 Saga.Id
        Guid DeviceId,
        Guid UserId,
        DateTimeOffset SavedAt);
}

namespace Contracts.Messages.SagaState
{
    public enum BindDeviceStep
    {
        NotStarted,
        DeviceChecked,         // 检查设备是否可用
        UserChecked,           // 检查用户是否存在、是否有权限
        DeviceBound,           // 已向 A 发送 Bind 命令并成功
        DeviceSaved,           // B 已成功保存关联记录
        Completed,
        Failed
    }

    public class BindDeviceSagaState
    {
        // Wolverine 会把它当成持久化的 Saga 状态对象
        public Guid Id { get; set; }                 // 也就是 CorrelationId
        public Guid DeviceId { get; set; }
        public Guid UserId { get; set; }

        public BindDeviceStep CurrentStep { get; set; } = BindDeviceStep.NotStarted;
        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        public string? FailureReason { get; set; }
    }
}