#:sdk Microsoft.NET.Sdk.Web
#:package StackExchange.Redis@2.7.121
#:package Microsoft.Data.Sqlite@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0

using System.Threading.Channels;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder();

// Redis分片配置
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => 
    ConnectionMultiplexer.Connect(new ConfigurationOptions {
        EndPoints = { "redis1:6379", "redis2:6379" },
        Proxy = Proxy.Twemproxy
    }));

// SQLite分片存储
builder.Services.AddDbContextFactory<AuditShardContext>(options => 
    options.UseSqlite("Data Source=audit_shard_{0}.db"));

// 分布式写入通道
var writeChannel = Channel.CreateBounded<AuditEvent>(10000);
builder.Services.AddSingleton(writeChannel);
builder.Services.AddHostedService<DistributedAuditWriter>();

public class DistributedAuditWriter : BackgroundService
{
    private readonly ChannelReader<AuditEvent> _reader;
    private readonly IConnectionMultiplexer _redis;
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var auditEvent in _reader.ReadAllAsync(ct))
        {
            // 使用CPU Cache-line对齐的内存分配
            var buffer = ArrayPool<byte>.Shared.Rent(1024);
            try {
                // 写入Redis和SQLite
            }
            finally {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
}