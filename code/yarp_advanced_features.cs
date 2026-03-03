#:sdk Microsoft.NET.Sdk.Web
#:package Yarp.ReverseProxy@2.0.0
#:package Microsoft.AspNetCore.Authentication.JwtBearer@8.0.0
#:package NSwag.AspNetCore@13.20.0
#:package Microsoft.Extensions.Http.Polly@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using NSwag.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. OpenAPI集成
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "BFF Gateway API";
    config.Version = "v1";
    config.AddSecurity("JWT", Enumerable.Empty<string>(),
        new NSwag.OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "JWT认证头"
        });
});

// 2. 认证授权配置
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://auth.server";
        options.Audience = "gateway-api";
        options.TokenValidationParameters = new()
        {
            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role
        };
    });

// 3. 负载均衡策略
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddLoadBalancingPolicy("RoundRobin", _ => new RoundRobinLoadBalancingPolicy())
    .AddLoadBalancingPolicy("Weighted", _ => new WeightedLoadBalancingPolicy())
    .AddSessionAffinityProvider("Cookie", _ => new CookieSessionAffinityProvider());

// 4. 服务聚合配置
builder.Services.AddHttpClient("Aggregator")
    .AddTransientHttpErrorPolicy(policy => 
        policy.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(200)));

// 5. SSO集成
builder.Services.AddSingleton<ISsoClient, SsoClient>();
builder.Services.AddSingleton<ITicketStore, DistributedCacheTicketStore>();

var app = builder.Build();

// OpenAPI中间件
app.UseOpenApi();
app.UseSwaggerUi3();

// 认证授权中间件
app.UseAuthentication();
app.UseAuthorization();

// 聚合路由示例
app.MapGet("/api/aggregated", async (IHttpClientFactory factory) =>
{
    var client = factory.CreateClient("Aggregator");
    var tasks = new[]
    {
        client.GetStringAsync("http://service1/api/data"),
        client.GetStringAsync("http://service2/api/data")
    };
    
    var results = await Task.WhenAll(tasks);
    return Results.Ok(new { Service1 = results[0], Service2 = results[1] });
});

// 权重路由示例
app.MapGet("/weighted", () => "Weighted Route Example");

// 代理配置
app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.Use(async (context, next) =>
    {
        // 认证检查
        if (!context.User.Identity?.IsAuthenticated ?? false)
        {
            context.Response.StatusCode = 401;
            return;
        }
        
        // 权限检查
        if (!context.User.IsInRole("Admin"))
        {
            context.Response.StatusCode = 403;
            return;
        }
        
        await next();
    });
});

app.Run();

// 自定义负载均衡策略
[SkipLocalsInit]
public class RoundRobinLoadBalancingPolicy : ILoadBalancingPolicy
{
    private volatile int _index;
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public DestinationState PickDestination(
        HttpContext context, 
        IReadOnlyList<DestinationState> availableDestinations)
    {
        var index = Interlocked.Increment(ref _index);
        return availableDestinations[index % availableDestinations.Count];
    }
}

[SkipLocalsInit]
public class WeightedLoadBalancingPolicy : ILoadBalancingPolicy
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public DestinationState PickDestination(
        HttpContext context, 
        IReadOnlyList<DestinationState> availableDestinations)
    {
        var totalWeight = availableDestinations.Sum(d => GetWeight(d));
        var random = Random.Shared.Next(totalWeight);
        var sum = 0;
        
        foreach (var dest in availableDestinations)
        {
            sum += GetWeight(dest);
            if (random < sum) return dest;
        }
        
        return availableDestinations[0];
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetWeight(DestinationState destination) => 
        destination.Destination.Metadata?.GetValue<int>("Weight") ?? 1;
}

// SSO客户端实现
[SkipLocalsInit]
public class SsoClient : ISsoClient
{
    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;
    
    public SsoClient(HttpClient client, IMemoryCache cache) => 
        (_client, _cache) = (client, cache);
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<ClaimsPrincipal> ValidateTokenAsync(string token)
    {
        if (_cache.TryGetValue(token, out ClaimsPrincipal principal))
            return principal;
            
        var response = await _client.PostAsJsonAsync("/validate", new { Token = token });
        if (!response.IsSuccessStatusCode)
            throw new AuthenticationException("Token validation failed");
        
        var content = await response.Content.ReadFromJsonAsync<SsoValidationResult>();
        principal = new ClaimsPrincipal(new ClaimsIdentity(content.Claims));
        
        _cache.Set(token, principal, TimeSpan.FromMinutes(5));
        return principal;
    }
}