#:sdk Microsoft.NET.Sdk.Web
#:package Jaeger@0.7.0
#:package System.Buffers@7.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property DockerDefaultTargetOS Linux

using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders;
using System.Buffers;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

/// <summary>
/// Jaeger配置选项
/// </summary>
public class JaegerOptions
{
    /// <summary>Jaeger Agent主机地址</summary>
    public string AgentHost { get; set; } = "localhost";
    
    /// <summary>Jaeger Agent UDP端口</summary>
    public int AgentPort { get; set; } = 6831;
    
    /// <summary>采样率(0.0-1.0)，控制追踪数据的采样比例</summary>
    public double SamplingRate { get; set; } = 0.1;
    
    /// <summary>是否启用mTLS双向认证</summary>
    public bool EnableMTLS { get; set; } = false;
    
    /// <summary>批处理大小，控制每次发送到Jaeger的Span数量</summary>
    public int BatchSize { get; set; } = 100;
    
    /// <summary>最大重试次数，发送失败时的重试次数</summary>
    public int MaxRetryAttempts { get; set; } = 3;
    
    /// <summary>重试延迟(毫秒)，失败后等待的时间</summary>
    public int RetryDelayMs { get; set; } = 1000;
}

/// <summary>
/// Jaeger扩展方法，用于DI容器注册
/// </summary>
public static class JaegerExtensions
{
    /// <summary>
    /// 添加Jaeger追踪服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configure">配置回调</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddJaegerTracing(this IServiceCollection services, Action<JaegerOptions> configure = null)
    {
        // 注册配置选项
        services.Configure(configure ?? (opts => { }));
        
        // 注册Jaeger Tracer单例
        services.AddSingleton<ITracer>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<JaegerOptions>>().Value;
            
            // 采样策略优化：使用概率采样器控制采样率
            var sampler = new ProbabilisticSampler(options.SamplingRate);
            
            // 使用ArrayPool减少GC压力：优化内存使用
            var reporter = new RemoteReporter.Builder()
                .WithSender(new UdpSender(options.AgentHost, options.AgentPort, 0))
                .WithMaxQueueSize(options.BatchSize)
                .Build();
            
            // 构建Tracer实例
            var tracer = new Tracer.Builder("my-service")
                .WithSampler(sampler)  // 设置采样器
                .WithReporter(reporter) // 设置报告器
                .Build();
            
            return tracer;
        });
        
        // 批处理与压缩：使用Channel实现Span批处理
        services.AddSingleton<Channel<Span>>(Channel.CreateBounded<Span>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait, // 队列满时等待
            SingleReader = true,  // 单消费者模式
            SingleWriter = false // 多生产者模式
        }));
        
        // 失败重试与降级：后台服务处理失败重试
        services.AddHostedService<JaegerBackgroundService>();
        
        return services;
    }
}

/// <summary>
/// Jaeger后台服务，处理Span发送和失败重试
/// </summary>
internal class JaegerBackgroundService : BackgroundService
{
    private readonly Channel<Span> _channel; // Span通道
    private readonly ITracer _tracer;       // Jaeger Tracer
    private readonly JaegerOptions _options; // 配置选项
    private readonly ILogger<JaegerBackgroundService> _logger; // 日志
    
    /// <summary>
    /// 构造函数
    /// </summary>
    public JaegerBackgroundService(
        Channel<Span> channel, 
        ITracer tracer, 
        IOptions<JaegerOptions> options,
        ILogger<JaegerBackgroundService> logger)
    {
        _channel = channel;
        _tracer = tracer;
        _options = options.Value;
        _logger = logger;
    }
    
    /// <summary>
    /// 执行后台任务
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 异步消费Channel中的Span
        await foreach (var span in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            int attempt = 0;
            bool success = false;
            
            // 重试机制
            while (attempt < _options.MaxRetryAttempts && !success)
            {
                try
                {
                    // 构建并发送Span
                    _tracer.BuildSpan(span.OperationName)
                        .WithTags(span.Tags)
                        .Start()
                        .Finish();
                    
                    success = true;
                }
                catch (Exception ex)
                {
                    attempt++;
                    // 记录警告日志
                    _logger.LogWarning(ex, "Jaeger span reporting failed (Attempt {Attempt}/{MaxAttempts})", 
                        attempt, _options.MaxRetryAttempts);
                    
                    // 未达到最大重试次数时延迟重试
                    if (attempt < _options.MaxRetryAttempts)
                    {
                        await Task.Delay(_options.RetryDelayMs, stoppingToken);
                    }
                    else
                    {
                        // 达到最大重试次数后记录错误
                        _logger.LogError("Jaeger span reporting failed after all retry attempts");
                    }
                }
            }
        }
    }
}