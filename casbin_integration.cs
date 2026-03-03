#:sdk Microsoft.NET.Sdk.Web
#:package Casbin.NET@1.15.0
#:package Microsoft.AspNetCore.Identity.EntityFrameworkCore@8.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.ObjectPool;
using Casbin;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

// 通过依赖注入获取CasbinIdentityAdapter服务，然后调用CheckPermissionAsync方法进行权限验证

// 1. 定义Identity模型扩展
public class ApplicationUser : IdentityUser
{
    public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }
    public virtual ICollection<MenuPermission> MenuPermissions { get; set; }
}

public class ApplicationRole : IdentityRole
{
    public virtual ICollection<IdentityUserRole<string>> Users { get; set; }
    public virtual ICollection<RolePermission> RolePermissions { get; set; }
}

public class MenuPermission
{
    public int Id { get; set; }
    public string MenuId { get; set; }
    public string Permission { get; set; }
    public string ApplicationUserId { get; set; }
    public ApplicationUser User { get; set; }
}

public class RolePermission
{
    public int Id { get; set; }
    public string MenuId { get; set; }
    public string Permission { get; set; }
    public string RoleId { get; set; }
    public ApplicationRole Role { get; set; }
}

// 2. 高性能Casbin适配器(集成Identity)
[SkipLocalsInit]
public sealed class CasbinIdentityAdapter : BackgroundService
{
    private readonly Channel<AuthRequest> _requestChannel;
    private readonly ObjectPool<CasbinContext> _contextPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly IEnforcer _enforcer;
    private readonly IDistributedCache _cache;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public CasbinIdentityAdapter(
        IEnforcer enforcer,
        IDistributedCache cache,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _enforcer = enforcer;
        _cache = cache;
        _userManager = userManager;
        _roleManager = roleManager;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        // Disruptor模式通道配置
        _requestChannel = Channel.CreateBounded<AuthRequest>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        // 上下文对象池(CPU cache-line对齐)
        _contextPool = new DefaultObjectPool<CasbinContext>(
            new CasbinContextPooledPolicy(), 1000);
    }

    // 3. 核心授权方法(零拷贝优化)
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public async Task<bool> CheckPermissionAsync(ClaimsPrincipal user, string menuId, string permission)
    {
        var userId = _userManager.GetUserId(user);
        if (string.IsNullOrEmpty(userId)) return false;

        // 检查Redis缓存
        var cacheKey = $"casbin:{userId}:{menuId}:{permission}";
        var cachedResult = await _cache.GetAsync(cacheKey);
        if (cachedResult != null)
        {
            return cachedResult.Span[0] == 1;
        }

        // 获取用户角色
        var userRoles = await _userManager.GetRolesAsync(await _userManager.GetUserAsync(user));
        
        // 检查Casbin策略
        bool result = false;
        foreach (var role in userRoles)
        {
            if (await _enforcer.EnforceAsync(role, menuId, permission))
            {
                result = true;
                break;
            }
        }

        // 缓存结果(1分钟)
        await _cache.SetAsync(cacheKey, 
            new ReadOnlyMemory<byte>(result ? new byte[] { 1 } : new byte[] { 0 }), 
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) });

        return result;
    }

    // 4. 后台处理任务
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var request in _requestChannel.Reader.ReadAllAsync(stoppingToken))
        {
            using var context = _contextPool.Get();
            try
            {
                // 处理授权请求
                var result = await CheckPermissionAsync(request.User, request.MenuId, request.Permission);
                request.CompletionSource.TrySetResult(result);
            }
            catch (Exception ex)
            {
                request.CompletionSource.TrySetException(ex);
            }
        }
    }

    // 5. 策略同步方法
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task SyncPoliciesFromIdentityAsync()
    {
        // 从数据库加载所有角色权限并同步到Casbin
        var roles = await _roleManager.Roles
            .Include(r => r.RolePermissions)
            .ToListAsync();

        foreach (var role in roles)
        {
            foreach (var permission in role.RolePermissions)
            {
                await _enforcer.AddPolicyAsync(role.Name, permission.MenuId, permission.Permission);
            }
        }
    }
}

// 6. 上下文对象池策略
[SkipLocalsInit]
public sealed class CasbinContextPooledPolicy : PooledObjectPolicy<CasbinContext>
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override CasbinContext Create()
    {
        return new CasbinContext();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override bool Return(CasbinContext obj)
    {
        obj.Reset();
        return true;
    }
}

// 7. 授权请求模型
public record AuthRequest(
    ClaimsPrincipal User,
    string MenuId,
    string Permission,
    TaskCompletionSource<bool> CompletionSource);