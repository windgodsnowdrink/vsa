#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.SignalR@8.0.0
#:package OpenTelemetry.Exporter.Console@1.7.0
#:package Microsoft.Extensions.Configuration.Json@8.0.0
#:package Microsoft.Extensions.Options.ConfigurationExtensions@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using OpenTelemetry.Metrics;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder();

// 1. 压缩算法性能监控
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddMeter("Compression.Metrics")
        .AddConsoleExporter());

// 2. 消息过期事件通知
builder.Services.AddSingleton<IMessageExpirationNotifier>(sp => 
    new ExpirationEventNotifier(
        new ThreadLocal<Span<byte>>(() => stackalloc byte[256])));

// 3. QoS策略热更新
builder.Services.Configure<QoSOptions>(builder.Configuration.GetSection("QoS"));
builder.Services.AddSingleton<IQoSConfigUpdater, HotUpdateQoSConfig>();

var app = builder.Build();
app.MapHub<MonitoringHub>("/monitoring");
app.Run();

// 压缩监控器
[SkipLocalsInit]
public class CompressionMonitor
{
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public CompressionMonitor(ThreadLocal<Span<byte>> buffer)
    {
        _buffer = buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe void RecordMetrics(CompressionMetrics metrics)
    {
        Span<byte> buffer = _buffer.Value;
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0)
            {
                // SIMD优化指标记录
            }
        }
    }
}

// 过期事件通知器
[SkipLocalsInit]
public class ExpirationEventNotifier : IMessageExpirationNotifier
{
    private readonly Channel<ExpiredMessageEvent> _channel;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public ExpirationEventNotifier(ThreadLocal<Span<byte>> buffer)
    {
        _buffer = buffer;
        _channel = Channel.CreateBounded<ExpiredMessageEvent>(1000);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe void NotifyExpired(string messageId)
    {
        Span<byte> buffer = _buffer.Value;
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0)
            {
                _channel.Writer.TryWrite(new ExpiredMessageEvent(messageId));
            }
        }
    }
}

// QoS热更新器
[SkipLocalsInit]
public class HotUpdateQoSConfig : IQoSConfigUpdater
{
    private readonly IOptionsMonitor<QoSOptions> _monitor;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public HotUpdateQoSConfig(
        IOptionsMonitor<QoSOptions> monitor,
        ThreadLocal<Span<byte>> buffer)
    {
        _monitor = monitor;
        _buffer = buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe void UpdateConfig()
    {
        Span<byte> buffer = _buffer.Value;
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0)
            {
                // 配置热更新逻辑
            }
        }
    }
}

public record CompressionMetrics(
    string Algorithm,
    double Ratio,
    TimeSpan Duration);

public record ExpiredMessageEvent(string MessageId);

public class QoSOptions
{
    public int HighPriorityLimit { get; set; }
    public int NormalPriorityLimit { get; set; }
}