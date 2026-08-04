#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.Sqlite@8.0.0
#:package StackExchange.Redis@2.7.121
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using StackExchange.Redis;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

// 配置Redis分布式缓存
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379"));

// 配置SQLite分片存储
builder.Services.AddDbContextFactory<ShardDbContext>(options => 
    options.UseSqlite("Data Source=history_shard_{0}.db"));

// 配置分布式写入通道
var writeChannel = Channel.CreateBounded<HistoryEvent>(
    new BoundedChannelOptions(10_000)
    {
        SingleReader = false,
        AllowSynchronousContinuations = true
    });

builder.Services.AddSingleton(writeChannel);
builder.Services.AddHostedService<DistributedHistoryWriter>();

var app = builder.Build();
app.Run();

// 分布式写入服务
public class DistributedHistoryWriter : BackgroundService
{
    private readonly ChannelReader<HistoryEvent> _reader;
    private readonly IConnectionMultiplexer _redis;
    private readonly IDbContextFactory<ShardDbContext> _dbFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var batch = new List<HistoryEvent>(1000);
        while (await _reader.WaitToReadAsync(stoppingToken))
        {
            while (_reader.TryRead(out var @event))
            {
                batch.Add(@event);
                if (batch.Count >= 1000)
                {
                    await ProcessBatch(batch);
                    batch.Clear();
                }
            }
        }
    }

    private async Task ProcessBatch(List<HistoryEvent> batch)
    {
        // 使用Redis进行分布式协调
        var db = _redis.GetDatabase();
        var transaction = db.CreateTransaction();
        
        // 分片写入SQLite
        var shards = batch.GroupBy(e => e.EntityId.GetHashCode() % 10);
        foreach (var shard in shards)
        {
            using var context = _dbFactory.CreateDbContext();
            context.AutoHistory.AddRange(shard.Select(e => new AutoHistory
            {
                EntityId = e.EntityId,
                EntityType = e.EntityType,
                CompressedData = e.Data
            }));
            await context.SaveChangesAsync();
        }
        
        await transaction.ExecuteAsync();
    }
}

public record HistoryEvent(string EntityType, string EntityId, byte[] Data);