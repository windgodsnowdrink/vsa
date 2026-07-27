#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package InfluxDB.Client@3.0.0
#:package Microsoft.Extensions.Hosting@8.0.0
#:package Microsoft.Extensions.Logging@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.Metrics;
using System.Threading.Channels;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfluxDb("http://localhost:8086", "token", "org", "bucket");
builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();
app.Run();

/// <summary>
/// InfluxDB扩展方法
/// </summary>
public static class InfluxDbExtensions
{
    /// <summary>
    /// 添加InfluxDB服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="url">InfluxDB URL</param>
    /// <param name="token">认证令牌</param>
    /// <param name="org">组织</param>
    /// <param name="bucket">桶</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddInfluxDb(this IServiceCollection services, string url, string token, string org, string bucket)
    {
        services.AddSingleton(sp => InfluxDBClientFactory.Create(url, token.ToCharArray()));
        services.AddSingleton(new InfluxDbOptions(org, bucket));
        services.AddSingleton<IResiliencePipelineProvider, ResiliencePipelineProvider>();
        services.AddHostedService<InfluxDbBackgroundWriter>();
        services.AddSingleton<IInfluxDbWriter>(sp => sp.GetRequiredService<InfluxDbBackgroundWriter>());
        return services;
    }
}

/// <summary>
/// InfluxDB选项
/// </summary>
/// <param name="Org">组织</param>
/// <param name="Bucket">桶</param>
public record InfluxDbOptions(string Org, string Bucket);

/// <summary>
/// InfluxDB写入器接口
/// </summary>
public interface IInfluxDbWriter
{
    /// <summary>
    /// 写入点数据
    /// </summary>
    /// <param name="point">点数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task WritePointAsync(PointData point, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量写入点数据
    /// </summary>
    /// <param name="points">点数据集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task WritePointsAsync(IEnumerable<PointData> points, CancellationToken cancellationToken = default);
}

/// <summary>
/// 弹性管道提供者
/// </summary>
public class ResiliencePipelineProvider : IResiliencePipelineProvider
{
    private readonly Dictionary<string, IResiliencePipeline> _pipelines = new();

    /// <summary>
    /// 初始化弹性管道提供者
    /// </summary>
    public ResiliencePipelineProvider()
    {
        // 创建重试和断路器管道
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(100),
                BackoffType = DelayBackoffType.Exponential,
                OnRetry = args =>
                {
                    Console.WriteLine($"Retry attempt {args.AttemptNumber} after {args.Delay.TotalMilliseconds}ms");
                    return default;
                }
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(10),
                MinimumThroughput = 10,
                BreakDuration = TimeSpan.FromSeconds(30),
                OnCircuitClosed = args =>
                {
                    Console.WriteLine("Circuit breaker closed");
                    return default;
                },
                OnCircuitOpened = args =>
                {
                    Console.WriteLine($"Circuit breaker opened for {args.BreakDuration.TotalSeconds}s");
                    return default;
                }
            })
            .Build();

        _pipelines["influx-retry"] = pipeline;
    }

    /// <summary>
    /// 获取弹性管道
    /// </summary>
    /// <param name="name">管道名称</param>
    /// <returns>弹性管道</returns>
    public IResiliencePipeline GetPipeline(string name)
    {
        return _pipelines[name];
    }
}

/// <summary>
/// 弹性管道提供者接口
/// </summary>
public interface IResiliencePipelineProvider
{
    /// <summary>
    /// 获取弹性管道
    /// </summary>
    /// <param name="name">管道名称</param>
    /// <returns>弹性管道</returns>
    IResiliencePipeline GetPipeline(string name);
}

/// <summary>
/// InfluxDB后台写入器
/// </summary>
public sealed class InfluxDbBackgroundWriter : BackgroundService, IInfluxDbWriter, IDisposable
{
    private readonly Channel<PointData> _channel;
    private readonly ArrayPool<PointData> _pool;
    private readonly IResiliencePipeline _resiliencePipeline;
    private readonly ILogger<InfluxDbBackgroundWriter> _logger;
    private readonly InfluxDBClient _client;
    private readonly InfluxDbOptions _options;

    /// <summary>
    /// 写入指标
    /// </summary>
    private static readonly Counter WriteOperationsCounter = new Counter<long>(
        "influxdb_write_operations_total", 
        "Total number of InfluxDB write operations",
        new CounterOptions { LabelNames = new[] { "status" } });

    /// <summary>
    /// 写入延迟指标
    /// </summary>
    private static readonly Histogram<double> WriteLatencyHistogram = new Histogram<double>(
        "influxdb_write_latency_seconds", 
        "InfluxDB write operation latency in seconds");

    /// <summary>
    /// 初始化InfluxDB后台写入器
    /// </summary>
    /// <param name="client">InfluxDB客户端</param>
    /// <param name="options">InfluxDB选项</param>
    /// <param name="pipelineProvider">弹性管道提供者</param>
    /// <param name="logger">日志记录器</param>
    public InfluxDbBackgroundWriter(
        InfluxDBClient client,
        InfluxDbOptions options,
        IResiliencePipelineProvider pipelineProvider,
        ILogger<InfluxDbBackgroundWriter> logger)
    {
        _channel = Channel.CreateBounded<PointData>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });

        _pool = ArrayPool<PointData>.Create();
        _resiliencePipeline = pipelineProvider.GetPipeline("influx-retry");
        _logger = logger;
        _client = client;
        _options = options;
    }

    /// <summary>
    /// 写入点数据
    /// </summary>
    /// <param name="point">点数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public async Task WritePointAsync(PointData point, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(point, cancellationToken);
    }

    /// <summary>
    /// 批量写入点数据
    /// </summary>
    /// <param name="points">点数据集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public async Task WritePointsAsync(IEnumerable<PointData> points, CancellationToken cancellationToken = default)
    {
        foreach (var point in points)
        {
            await _channel.Writer.WriteAsync(point, cancellationToken);
        }
    }

    /// <summary>
    /// 执行后台服务
    /// </summary>
    /// <param name="stoppingToken">停止令牌</param>
    /// <returns>任务</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ProcessPointsAsync(stoppingToken);
    }

    /// <summary>
    /// 处理点数据
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task ProcessPointsAsync(CancellationToken cancellationToken)
    {
        var batchSize = 100;
        var batch = _pool.Rent(batchSize);
        var count = 0;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (await _channel.Reader.WaitToReadAsync(cancellationToken))
                {
                    while (count < batchSize && _channel.Reader.TryRead(out var point))
                    {
                        batch[count] = point;
                        count++;
                    }

                    if (count > 0)
                    {
                        await WriteBatchAsync(batch.AsSpan(0, count), cancellationToken);
                        count = 0;
                    }
                }
            }

            // 处理剩余数据
            if (count > 0)
            {
                await WriteBatchAsync(batch.AsSpan(0, count), cancellationToken);
            }
        }
        finally
        {
            _pool.Return(batch);
        }
    }

    /// <summary>
    /// 写入批处理数据
    /// </summary>
    /// <param name="points">点数据跨度</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task WriteBatchAsync(Span<PointData> points, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _resiliencePipeline.ExecuteAsync(async (ctx) =>
            {
                using var writeApi = _client.GetWriteApiAsync();
                await writeApi.WritePointsAsync(points.ToArray(), _options.Bucket, _options.Org, cancellationToken);
            });
            WriteOperationsCounter.Add(points.Length, new TagList { { "status", "success" } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write batch of {Count} points", points.Length);
            WriteOperationsCounter.Add(points.Length, new TagList { { "status", "error" } });
        }
        finally
        {
            stopwatch.Stop();
            WriteLatencyHistogram.Record(stopwatch.Elapsed.TotalSeconds);
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        _client.Dispose();
        _channel.Writer.Complete();
    }
}

/// <summary>
/// InfluxDB控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InfluxDbController : ControllerBase
{
    private readonly IInfluxDbWriter _writer;

    /// <summary>
    /// 初始化InfluxDB控制器
    /// </summary>
    /// <param name="writer">InfluxDB写入器</param>
    public InfluxDbController(IInfluxDbWriter writer)
    {
        _writer = writer;
    }

    /// <summary>
    /// 写入指标
    /// </summary>
    /// <param name="request">写入请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("write")]
    public async Task<IActionResult> Write([FromBody] WriteRequest request, CancellationToken cancellationToken)
    {
        var point = PointData.Measurement(request.Measurement)
            .Tag("host", request.Host)
            .Field("value", request.Value)
            .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

        await _writer.WritePointAsync(point, cancellationToken);
        return Ok();
    }
}

/// <summary>
/// 写入请求
/// </summary>
public class WriteRequest
{
    /// <summary>
    /// 测量名称
    /// </summary>
    public string Measurement { get; set; }

    /// <summary>
    /// 主机
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public double Value { get; set; }
}

/// <summary>
/// 弹性管道接口
/// </summary>
public interface IResiliencePipeline
{
    /// <summary>
    /// 执行异步操作
    /// </summary>
    /// <param name="action">操作</param>
    /// <returns>任务</returns>
    Task ExecuteAsync(Func<Task> action);

    /// <summary>
    /// 执行异步操作
    /// </summary>
    /// <param name="action">操作</param>
    /// <returns>任务</returns>
    Task ExecuteAsync(Func<ResilienceContext, Task> action);
}

/// <summary>
/// 弹性上下文
/// </summary>
public class ResilienceContext
{
}

/// <summary>
/// 简单的弹性管道实现
/// </summary>
public class SimpleResiliencePipeline : IResiliencePipeline
{
    private readonly AsyncPolicy _policy;

    /// <summary>
    /// 初始化简单的弹性管道
    /// </summary>
    /// <param name="policy">策略</param>
    public SimpleResiliencePipeline(AsyncPolicy policy)
    {
        _policy = policy;
    }

    /// <summary>
    /// 执行异步操作
    /// </summary>
    /// <param name="action">操作</param>
    /// <returns>任务</returns>
    public Task ExecuteAsync(Func<Task> action)
    {
        return _policy.ExecuteAsync(action);
    }

    /// <summary>
    /// 执行异步操作
    /// </summary>
    /// <param name="action">操作</param>
    /// <returns>任务</returns>
    public Task ExecuteAsync(Func<ResilienceContext, Task> action)
    {
        return _policy.ExecuteAsync(() => action(new ResilienceContext()));
    }
}

/// <summary>
/// 计数器选项
/// </summary>
public class CounterOptions
{
    /// <summary>
    /// 标签名称
    /// </summary>
    public string[] LabelNames { get; set; }
}

/// <summary>
/// 直方图选项
/// </summary>
public class HistogramOptions
{
    /// <summary>
    /// 标签名称
    /// </summary>
    public string[] LabelNames { get; set; }
}

/// <summary>
/// 标签列表
/// </summary>
public class TagList : Dictionary<string, object>
{
}

/// <summary>
/// 计数器
/// </summary>
/// <typeparam name="T">值类型</typeparam>
public class Counter<T> where T : struct
{
    private readonly string _name;
    private readonly string _description;
    private readonly CounterOptions _options;

    /// <summary>
    /// 初始化计数器
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="description">描述</param>
    /// <param name="options">选项</param>
    public Counter(string name, string description, CounterOptions options)
    {
        _name = name;
        _description = description;
        _options = options;
    }

    /// <summary>
    /// 添加值
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="tags">标签</param>
    public void Add(T value, TagList tags)
    {
        // 模拟实现
    }
}

/// <summary>
/// 直方图
/// </summary>
/// <typeparam name="T">值类型</typeparam>
public class Histogram<T> where T : struct
{
    private readonly string _name;
    private readonly string _description;

    /// <summary>
    /// 初始化直方图
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="description">描述</param>
    public Histogram(string name, string description)
    {
        _name = name;
        _description = description;
    }

    /// <summary>
    /// 记录值
    /// </summary>
    /// <param name="value">值</param>
    public void Record(T value)
    {
        // 模拟实现
    }
}

/// <summary>
/// 秒表
/// </summary>
public class Stopwatch
{
    private DateTime _startTime;
    private bool _running;

    /// <summary>
    /// 开始新的秒表
    /// </summary>
    /// <returns>秒表实例</returns>
    public static Stopwatch StartNew()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        return stopwatch;
    }

    /// <summary>
    /// 开始计时
    /// </summary>
    public void Start()
    {
        _startTime = DateTime.UtcNow;
        _running = true;
    }

    /// <summary>
    /// 停止计时
    /// </summary>
    public void Stop()
    {
        _running = false;
    }

    /// <summary>
    /// 经过的时间
    /// </summary>
    public TimeSpan Elapsed
    {
        get { return _running ? DateTime.UtcNow - _startTime : TimeSpan.Zero; }
    }
}
