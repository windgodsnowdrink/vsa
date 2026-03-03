#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authorization@8.0.0
#:package Microsoft.Extensions.Options@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

public class AbacRequirement : IAuthorizationRequirement { }

public class AbacHandler : AuthorizationHandler<AbacRequirement>
{
    private readonly AbacPolicyProvider _policyProvider;

    public AbacHandler(AbacPolicyProvider policyProvider)
    {
        _policyProvider = policyProvider;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AbacRequirement requirement)
    {
        var policy = await _policyProvider.GetPolicyAsync(context.Resource);
        if (policy != null && await policy.EvaluateAsync(context.User, context.Resource))
        {
            context.Succeed(requirement);
        }
    }
}

public class AbacPolicyProvider
{
    private readonly IOptionsMonitor<AbacOptions> _options;

    public AbacPolicyProvider(IOptionsMonitor<AbacOptions> options)
    {
        _options = options;
    }

    public async Task<IAbacPolicy> GetPolicyAsync(object resource)
    {
        var resourceType = resource.GetType();
        return _options.CurrentValue.Policies.TryGetValue(resourceType, out var policy) 
            ? policy 
            : null;
    }
}

public interface IAbacPolicy
{
    Task<bool> EvaluateAsync(ClaimsPrincipal user, object resource);
}

public class AbacOptions
{
    public Dictionary<Type, IAbacPolicy> Policies { get; } = new();
}

// 使用示例
public class DocumentAbacPolicy : IAbacPolicy
{
    public Task<bool> EvaluateAsync(ClaimsPrincipal user, object resource)
    {
        var doc = resource as Document;
        return Task.FromResult(
            user.HasClaim("Department", doc.Department) &&
            user.HasClaim("ClearanceLevel", doc.RequiredClearance.ToString()));
    }
}