#:sdk Microsoft.NET.Sdk.Web
#:package MiniProfiler.AspNetCore@4.2.22
#:package MiniProfiler.EntityFrameworkCore@4.2.22
#:package MiniProfiler.Providers.SqlServer@4.2.22
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using StackExchange.Profiling;
using StackExchange.Profiling.Storage;

var builder = WebApplication.CreateBuilder();

// 配置MiniProfiler
builder.Services.AddMiniProfiler(options =>
{
    options.RouteBasePath = "/profiler";
    options.Storage = new SqlServerStorage("Server=.;Database=Profiler;Trusted_Connection=True;");
    options.TrackConnectionOpenClose = true;
    
    // 使用高性能内存存储
    options.Storage = new MemoryCacheStorage(
        new MemoryCacheStorageOptions
        {
            CacheDuration = TimeSpan.FromMinutes(30),
            CacheSize = 1000
        });
    
    // 配置EF Core跟踪
    options.EnableEntityFrameworkTracking();
}).AddEntityFramework();

var app = builder.Build();

// 启用MiniProfiler中间件
app.UseMiniProfiler();

// 自定义探查点示例
app.MapGet("/api/orders", () =>
{
    using (MiniProfiler.Current.Step("获取订单数据"))
    {
        // ... existing code ...
        using (MiniProfiler.Current.CustomTiming("SQL", "SELECT * FROM Orders"))
        {
            // 数据库查询代码
        }
        return Results.Ok();
    }
});

app.MapGet("/", () => "MiniProfiler Ready");
app.Run();