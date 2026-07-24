#:sdk Microsoft.NET.Sdk.Web
#:package Exceptionless.AspNetCore@6.0.0
#:package Exceptionless.Profiler@6.0.0
#:package System.Threading.Channels@7.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using Exceptionless;
using System.Threading.Channels;
using System.Runtime.CompilerServices;

/*
{
  "Exceptionless": {
    "ApiKey": "YOUR_API_KEY",
    "ServerUrl": "https://collector.exceptionless.io"
  }
}
*/
[SkipLocalsInit]
public static class ExceptionlessConfig
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static WebApplicationBuilder AddExceptionless(this WebApplicationBuilder builder)
    {
        builder.Services.AddExceptionless(builder.Configuration["Exceptionless:ApiKey"]);
        
        // 高性能异常处理通道
        builder.Services.AddSingleton<ExceptionChannel>();
        builder.Services.AddHostedService<ExceptionBackgroundService>();
        
        return builder;
    }
}

// 高性能异常处理通道
[SkipLocalsInit]
public sealed class ExceptionChannel : IAsyncDisposable
{
    private readonly Channel<Event> _channel;
    private readonly CancellationTokenSource _cts;

    public ExceptionChannel()
    {
        _cts = new CancellationTokenSource();
        _channel = Channel.CreateBounded<Event>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait,
            AllowSynchronousContinuations = false
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public ValueTask WriteAsync(Event @event)
    {
        return _channel.Writer.WriteAsync(@event, _cts.Token);
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _channel.Writer.Complete();
        await _channel.Reader.Completion;
    }
}

// 后台异常处理服务
[SkipLocalsInit]
public sealed class ExceptionBackgroundService : BackgroundService
{
    private readonly ExceptionlessClient _client;
    private readonly ExceptionChannel _channel;

    public ExceptionBackgroundService(
        ExceptionlessClient client,
        ExceptionChannel channel)
    {
        _client = client;
        _channel = channel;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var @event in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await _client.SubmitEventAsync(@event);
            }
            catch
            {
                // 异常处理失败时记录到本地
                File.AppendAllText("exception-fallback.log", 
                    $"{DateTime.UtcNow:O} - {@event.ToJson()}\n");
            }
        }
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder(args);
builder.AddExceptionless(); // 添加Exceptionless支持

var app = builder.Build();

// 全局异常处理中间件
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (Exception ex)
    {
        var channel = context.RequestServices.GetRequiredService<ExceptionChannel>();
        await channel.WriteAsync(ex.ToExceptionless()
            .SetHttpContext(context)
            .AddRequestInfo(context.Request)
            .AddObject(context.Items, "HttpContext.Items")
            .AddTags("unhandled")
            .MarkAsCritical());
        
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An error occurred");
    }
});

app.MapGet("/", () => "Exceptionless Integration Demo");
app.Run();