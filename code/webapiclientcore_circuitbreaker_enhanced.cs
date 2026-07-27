#:sdk Microsoft.NET.Sdk.Web
#:package WebApiClientCore@2.0.0
#:package Polly@8.0.0
#:package Microsoft.Extensions.Diagnostics.HealthChecks@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using Polly.CircuitBreaker;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 熔断状态通道
var circuitChannel = Channel.CreateBounded<CircuitState>(
    new BoundedChannelOptions(1000)
    {
        SingleReader = true,
        FullMode = BoundedChannelFullMode.Wait
    });

// 健康检查集成
builder.Services.AddHealthChecks()
    .AddCircuitBreakerCheck(circuitChannel.Reader);

// 零拷贝状态处理器
builder.Services.AddSingleton<ICircuitStateHandler>(sp => 
    new ChannelCircuitStateHandler(
        circuitChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[128])));

var app = builder.Build();
app.MapHealthChecks("/health");
app.MapGet("/", () => "Enhanced Circuit Breaker Ready");
app.Run();

public class ChannelCircuitStateHandler : ICircuitStateHandler
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void HandleStateChange(CircuitState state)
    {
        // 使用SIMD指令处理状态变更
    }
}