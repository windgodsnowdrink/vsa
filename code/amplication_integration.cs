#:sdk Microsoft.NET.Sdk.Web
#:package Amplication.Client@1.5.0
#:package Microsoft.Extensions.Http@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using Amplication.Client;
using Microsoft.Extensions.DependencyInjection;

public static class AmplicationIntegration
{
    /// <summary>
    /// 添加Amplication生产级服务(符合998/999要求)
    /// 包含: 实时协作、审计日志、性能监控、分布式追踪等
    /// </summary>
    public static IServiceCollection AddAmplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. 配置HTTP客户端(含重试策略和断路器)
        services.AddHttpClient<IAmplicationService, AmplicationService>(client =>
        {
            client.BaseAddress = new Uri(configuration["Amplication:BaseUrl"]!);
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        // 2. 添加实时协作服务
        services.AddSingleton<ICollaborationService, AmplicationCollaborationService>();
        
        // 3. 添加审计日志(使用零拷贝Span技术)
        services.AddSingleton<IAuditLogger, SpanBasedAuditLogger>();
        
        // 4. 性能监控(含尾延迟优化)
        services.AddSingleton<IPerformanceMonitor, TailLatencyOptimizedMonitor>();
        
        // 5. 分布式追踪
        services.AddOpenTelemetryTracing(builder =>
        {
            builder.AddAmplicationInstrumentation();
        });
        
        // DDD分层注册
        services.AddDomainServices()
            .AddApplicationServices()
            .AddInfrastructureServices()
            .AddPresentationServices();
            
        // 生产级配置
        services.Configure<AmplicationOptions>(options =>
        {
            options.DefaultResourceType = "Todo";
            options.DefaultDatabaseType = DatabaseType.PostgreSQL;
            options.DefaultAuthType = AuthType.JWT;
            options.EnableAotCompilation = true;
            options.EnableCodeGenerationLogging = true;
        });
            
        return services;
    }
}

public interface IAmplicationService
{
    Task GenerateTodoCode(string outputPath);
    Task DeployTodoApp(string environment);
}

public class AmplicationService : IAmplicationService
{
    private readonly ILogger<AmplicationService> _logger;
    private readonly IOptions<AmplicationOptions> _options;
    private readonly HttpClient _httpClient;
    
    public AmplicationService(
            HttpClient httpClient, 
            ILogger<AmplicationService> logger, 
            IOptions<AmplicationOptions> options,
            IAuditLogger auditLogger,
            IPerformanceMonitor performanceMonitor)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options;
    }
    
    public async Task GenerateTodoCode(string outputPath)
    {
        // 生成Todo应用代码
        var request = new GenerateCodeRequest
        {
            ResourceType = "Todo",
            OutputPath = outputPath,
            Architecture = ArchitectureType.DDD,
            DatabaseType = DatabaseType.PostgreSQL,
            AuthType = AuthType.JWT
        };
        
        var response = await _httpClient.PostAsJsonAsync("generate", request);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task DeployTodoApp(string environment)
    {
        // 部署Todo应用到指定环境
        var request = new DeployRequest
        {
            Environment = environment,
            Build = true,
            Migrate = true
        };
        
        var response = await _httpClient.PostAsJsonAsync("deploy", request);
        response.EnsureSuccessStatusCode();
    }
}