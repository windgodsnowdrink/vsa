#:sdk Microsoft.NET.Sdk.Web
#:package IdentityModel@6.0.0
#:package IdentityServer4.Admin@4.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Threading.Channels;
using IdentityServer4;
using IdentityServer4.Admin;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.ObjectPool;

// 1. SSO核心服务实现
[SkipLocalsInit]
public sealed class SsoService : BackgroundService
{
    private readonly Channel<SsoRequest> _requestChannel;
    private readonly ObjectPool<SsoContext> _contextPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly IClientStore _clientStore;
    private readonly IResourceStore _resourceStore;

    public SsoService(
        IClientStore clientStore,
        IResourceStore resourceStore)
    {
        _clientStore = clientStore;
        _resourceStore = resourceStore;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        _requestChannel = Channel.CreateBounded<SsoRequest>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        _contextPool = new DefaultObjectPool<SsoContext>(
            new SsoContextPooledPolicy(), 1000);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ProcessRequestAsync(SsoRequest request)
    {
        await _requestChannel.Writer.WriteAsync(request);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var request in _requestChannel.Reader.ReadAllAsync(stoppingToken))
        {
            var context = _contextPool.Get();
            try
            {
                _latencyOptimizer.Optimize(() => 
                {
                    context.Process(request, _clientStore, _resourceStore);
                });
            }
            finally
            {
                _contextPool.Return(context);
            }
        }
    }
}

// 2. 主程序集成
var builder = WebApplication.CreateBuilder(args);

// 配置IdentityServer
builder.Services.AddIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryClients(Config.Clients)
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiResources(Config.ApiResources)
    .AddInMemoryApiScopes(Config.ApiScopes);

// 注册Admin UI
builder.Services.AddIdentityServerAdminUI()
    .AddInMemoryClients(Config.Clients)
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiResources(Config.ApiResources)
    .AddInMemoryApiScopes(Config.ApiScopes);

// 注册SSO服务
builder.Services.AddSingleton<SsoService>();
builder.Services.AddHostedService<SsoService>();

var app = builder.Build();

app.UseIdentityServer();
app.UseIdentityServerAdminUI();

app.MapGet("/sso/request", async (SsoRequest request, SsoService service) =>
{
    await service.ProcessRequestAsync(request);
    return Results.Ok();
});

app.Run();

// 3. 配置类
public static class Config
{
    public static IEnumerable<Client> Clients => new List<Client>
    {
        new Client
        {
            ClientId = "mvc",
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedGrantTypes = GrantTypes.Code,
            RedirectUris = { "https://localhost:5002/signin-oidc" },
            PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
            AllowedScopes = new List<string>
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile
            }
        }
    };

    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiResource> ApiResources => new List<ApiResource>
    {
        new ApiResource("api1", "My API")
    };

    public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
    {
        new ApiScope("api1", "My API")
    };
}

// 4. 辅助类
public record SsoRequest(string ClientId, string RedirectUri, string Scope);

public class SsoContext
{
    public void Process(
        SsoRequest request,
        IClientStore clientStore,
        IResourceStore resourceStore)
    {
        // 处理SSO请求逻辑
        var client = clientStore.FindClientByIdAsync(request.ClientId).Result;
        var resources = resourceStore.FindResourcesByScopeAsync(request.Scope.Split(' ')).Result;
    }
}

public class SsoContextPooledPolicy : IPooledObjectPolicy<SsoContext>
{
    public SsoContext Create() => new SsoContext();
    public bool Return(SsoContext obj) => true;
}