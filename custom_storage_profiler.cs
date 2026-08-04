#:sdk Microsoft.NET.Sdk.Web
#:package MiniProfiler.AspNetCore@4.2.22
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using StackExchange.Profiling;
using StackExchange.Profiling.Storage;

public class RedisStorageProvider : IAsyncStorage
{
    private readonly IConnectionMultiplexer _redis;

    public RedisStorageProvider(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public Task SaveAsync(MiniProfiler profiler)
    {
        var db = _redis.GetDatabase();
        return db.StringSetAsync($"profiler:{profiler.Id}", profiler.ToJson());
    }
}

var builder = WebApplication.CreateBuilder();

builder.Services.AddMiniProfiler(options =>
{
    options.Storage = new RedisStorageProvider(ConnectionMultiplexer.Connect("localhost"));
});

var app = builder.Build();
app.UseMiniProfiler();
app.MapGet("/", () => "Custom Storage Profiler Ready");
app.Run();