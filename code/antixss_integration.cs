#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Security.Application@5.1.2
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Threading.Channels;
using Microsoft.Security.Application;
using Microsoft.Extensions.ObjectPool;

[SkipLocalsInit]
public sealed class AntiXssService : IAsyncDisposable
{
    private readonly Channel<SanitizeRequest> _requestChannel;
    private readonly ObjectPool<Encoder> _encoderPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly CancellationTokenSource _cts;

    public AntiXssService()
    {
        _latencyOptimizer = new TailLatencyOptimizer();
        _cts = new CancellationTokenSource();
        
        _requestChannel = Channel.CreateBounded<SanitizeRequest>(
            new BoundedChannelOptions(10_000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
        
        _encoderPool = new DefaultObjectPool<Encoder>(
            new EncoderPooledPolicy(), 
            Environment.ProcessorCount * 2);
        
        _ = Task.Run(ProcessRequestsAsync);
    }

    // HTML编码处理
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<string> HtmlEncodeAsync(string input)
    {
        var request = new SanitizeRequest(input, SanitizeType.Html);
        await _requestChannel.Writer.WriteAsync(request, _cts.Token);
        return await request.Completion.Task;
    }

    // JavaScript编码处理
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<string> JavaScriptEncodeAsync(string input)
    {
        var request = new SanitizeRequest(input, SanitizeType.JavaScript);
        await _requestChannel.Writer.WriteAsync(request, _cts.Token);
        return await request.Completion.Task;
    }

    // URL编码处理
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<string> UrlEncodeAsync(string input)
    {
        var request = new SanitizeRequest(input, SanitizeType.Url);
        await _requestChannel.Writer.WriteAsync(request, _cts.Token);
        return await request.Completion.Task;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessRequestsAsync()
    {
        await foreach (var request in _requestChannel.Reader.ReadAllAsync(_cts.Token))
        {
            using var latencyToken = _latencyOptimizer.BeginOperation();
            var encoder = _encoderPool.Get();
            try
            {
                var result = ProcessSanitize(encoder, request);
                request.Completion.SetResult(result);
            }
            finally
            {
                _encoderPool.Return(encoder);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private string ProcessSanitize(Encoder encoder, SanitizeRequest request)
    {
        return request.Type switch
        {
            SanitizeType.Html => encoder.HtmlEncode(request.Input),
            SanitizeType.JavaScript => encoder.JavaScriptEncode(request.Input),
            SanitizeType.Url => encoder.UrlEncode(request.Input),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _requestChannel.Writer.Complete();
        await _requestChannel.Reader.Completion;
    }
}

internal record SanitizeRequest(string Input, SanitizeType Type)
{
    public TaskCompletionSource<string> Completion { get; } = new();
}

internal enum SanitizeType { Html, JavaScript, Url }

[SkipLocalsInit]
internal sealed class EncoderPooledPolicy : PooledObjectPolicy<Encoder>
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override Encoder Create() => new Encoder();

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override bool Return(Encoder obj) => true;
}

// 启动配置
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AntiXssService>();

var app = builder.Build();
app.MapGet("/", () => "AntiXSS Security Service");
app.Run();