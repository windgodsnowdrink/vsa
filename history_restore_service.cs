#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.Sqlite@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

// 配置恢复任务通道
var restoreChannel = Channel.CreateBounded<RestoreRequest>(
    new BoundedChannelOptions(1000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true,
        FullMode = BoundedChannelFullMode.Wait
    });

builder.Services.AddSingleton(restoreChannel);
builder.Services.AddHostedService<HistoryRestoreService>();

var app = builder.Build();

// 恢复API端点
app.MapPost("/api/history/restore", async (RestoreRequest request, ChannelWriter<RestoreRequest> writer) =>
{
    await writer.WriteAsync(request);
    return Results.Accepted();
});

app.Run();

// 恢复服务
public class HistoryRestoreService : BackgroundService
{
    private readonly ChannelReader<RestoreRequest> _reader;
    private readonly HistoryDbContext _dbContext;
    private readonly HistoryCompressionService _compression;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var request in _reader.ReadAllAsync(stoppingToken))
        {
            using var memory = ArrayPool<byte>.Shared.Rent(1024);
            var history = await _dbContext.AutoHistory
                .FirstOrDefaultAsync(h => h.Id == request.HistoryId);
                
            if (history != null)
            {
                var json = _compression.Decompress(history.CompressedData);
                // 执行恢复逻辑...
            }
        }
    }
}

public record RestoreRequest(string EntityType, string EntityId, string HistoryId);