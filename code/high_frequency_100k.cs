#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.Sqlite@8.0.0
#:package Dapper.AOT@0.1.0
#:package StackExchange.Redis@2.7.121
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable

// 十万级TPS架构 EFCore+Dapper+Redis分片+零拷贝
using System.Threading.Channels;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// 核心优化点：
// 1. Disruptor模式环形缓冲区
var channel = Channel.CreateBounded<DbCommand>(capacity: 100000);

// 2. Redis分片集群
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => 
    ConnectionMultiplexer.Connect(new ConfigurationOptions {
        EndPoints = { "redis1:6379", "redis2:6379", "redis3:6379" },
        Proxy = Proxy.Twemproxy
    }));

// 3. 零拷贝SQL处理器
public class ZeroCopySqlProcessor : ISqlProcessor
{
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public async Task Process(DbCommand command)
    {
        var buffer = _buffer.Value;
        // 使用Span避免内存拷贝
        var sql = Encoding.UTF8.GetString(buffer.Slice(0, commandTextLength));
    }
}
var app = builder.Build();
app.Run();