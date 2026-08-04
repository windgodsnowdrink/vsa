#:sdk Microsoft.NET.Sdk.Web
#:package WebApiClientCore@2.0.0
#:package Polly@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Polly;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 配置熔断策略
builder.Services.AddResiliencePipeline("default", pipeline => 
{
    pipeline.AddCircuitBreaker(new CircuitBreakerStrategyOptions
    {
        FailureRatio = 0.5,
        SamplingDuration = TimeSpan.FromSeconds(10),
        MinimumThroughput = 8,
        BreakDuration = TimeSpan.FromSeconds(30)
    });
});

// 熔断事件处理器
builder.Services.AddSingleton<ICircuitEventHandler>(sp => 
    new ChannelCircuitEventHandler(
        Channel.CreateBounded<CircuitEvent>(1000),
        new ThreadLocal<Span<byte>>(() => stackalloc byte[128])));

var app = builder.Build();
app.MapGet("/", () => "Circuit Breaker Ready");
app.Run();

public class ChannelCircuitEventHandler : ICircuitEventHandler
{
    private readonly ChannelWriter<CircuitEvent> _writer;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public ChannelCircuitEventHandler(Channel<CircuitEvent> channel, ThreadLocal<Span<byte>> buffer)
    {
        _writer = channel.Writer;
        _buffer = buffer;
    }
}