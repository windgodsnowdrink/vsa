#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.ApplicationInsights.AspNetCore@2.21.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 配置告警引擎
builder.Services.AddSingleton<Channel<AlertEvent>>(Channel.CreateUnbounded<AlertEvent>());
builder.Services.AddHostedService<SmartAlertEngine>();

var app = builder.Build();
app.MapGet("/", () => "Alerting System Ready");
app.Run();

public class SmartAlertEngine : BackgroundService
{
    private readonly ChannelReader<AlertEvent> _reader;
    private readonly ObjectPool<Memory<byte>> _memoryPool;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var memory = _memoryPool.Get();
        while (await _reader.WaitToReadAsync(stoppingToken))
        {
            while (_reader.TryRead(out var alert))
            {
                // 动态阈值计算逻辑
            }
        }
    }
}