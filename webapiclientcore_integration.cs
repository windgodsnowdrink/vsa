#:sdk Microsoft.NET.Sdk.Web
#:package WebApiClientCore@2.0.0
#:package Polly@8.0.0
#:package Microsoft.Extensions.Diagnostics.HealthChecks@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Threading.Channels;
using WebApiClientCore;
using Polly;
using Microsoft.Extensions.Diagnostics.HealthChecks;

/*
- 定义API接口 ：

- 创建接口并使用特性标注HTTP方法和路由
- 使用 [HttpGet] 、 [HttpPost] 等特性定义端点
- 使用 [JsonContent] 、 [FormContent] 等特性定义请求体格式
- 配置依赖注入 ：

- 调用 AddWebApiClientCore() 注册核心服务
- 使用 ConfigureHttpApi<T>() 配置特定API客户端
- 配置弹性策略 ：

- 使用Polly添加重试、熔断等策略
- 配置适当的失败处理条件和参数
- 集成健康检查 ：

- 添加健康检查端点
- 实现自定义健康检查逻辑
- 性能优化 ：

- 使用 System.Threading.Channels 实现高性能事件处理
- 使用 ThreadLocal<Span<byte>> 实现零拷贝处理
- 考虑AOT编译优化
- 监控和诊断 ：

- 集成OpenTelemetry或Application Insights
- 添加适当的日志记录
*/
var builder = WebApplication.CreateBuilder();

// 1. 配置WebApiClientCore
builder.Services.AddWebApiClientCore()
    .ConfigureHttpApi<IMyApiService>(options =>
    {
        options.UsePolly(policy => policy
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = args => args.Outcome switch
                {
                    { Exception: HttpRequestException } => PredicateResult.True(),
                    _ => PredicateResult.False()
                },
                Delay = TimeSpan.FromSeconds(1),
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                ShouldHandle = args => args.Outcome switch
                {
                    { Exception: HttpRequestException } => PredicateResult.True(),
                    _ => PredicateResult.False()
                },
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(30),
                MinimumThroughput = 8,
                BreakDuration = TimeSpan.FromSeconds(30)
            }));
    });

// 2. 高性能通道处理器
var channel = Channel.CreateBounded<ApiRequestEvent>(1000);
builder.Services.AddSingleton(channel);
builder.Services.AddHostedService<ApiRequestProcessor>();

// 3. 健康检查集成
builder.Services.AddHealthChecks()
    .AddCheck<ApiHealthCheck>("api_health");

var app = builder.Build();
app.MapHealthChecks("/health");
app.MapGet("/", () => "WebApiClientCore Integration Ready");
app.Run();

// API服务接口定义
public interface IMyApiService
{
    [HttpGet("/api/users/{id}")]
    Task<User> GetUserAsync(int id);
    
    [HttpPost("/api/users")]
    Task<User> CreateUserAsync([JsonContent] User user);
}

// 高性能请求处理器
public class ApiRequestProcessor : BackgroundService
{
    private readonly ChannelReader<ApiRequestEvent> _reader;
    private readonly ThreadLocal<Span<byte>> _buffer;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var request in _reader.ReadAllAsync(stoppingToken))
        {
            var span = _buffer.Value;
            // 零拷贝处理请求事件
        }
    }
}

// 健康检查实现
public class ApiHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}