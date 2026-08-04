#:sdk Microsoft.NET.Sdk.Web
#:package HttpReports@3.0.0
#:package HttpReports.Dashboard@3.0.0
#:package HttpReports.Storage.SQLServer@3.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using HttpReports;
using HttpReports.Dashboard;

var builder = WebApplication.CreateBuilder();

// 配置HttpReports监控服务
builder.Services.AddHttpReports(options =>
{
    // 基础配置
    options.AppName = "SampleApp"; // 应用名称
    options.Node = "Production";   // 节点名称
    
    // 存储配置
    options.Storage = new SQLServerStorageOptions
    {
        ConnectionString = "Server=.;Database=HttpReports;Trusted_Connection=True;",
        DeferSecond = 5,          // 延迟写入秒数
        DeferThreshold = 100      // 延迟写入阈值
    };
    
    // 性能配置
    options.Batch = 50;           // 批量处理数量
    options.Background = true;    // 启用后台处理
    options.Switch = true;        // 启用监控开关
    
    // 采样率配置
    options.Sample = 1.0;         // 采样率(0-1)
    
    // 请求过滤配置
    options.RequestFilter = (request) => 
    {
        // 过滤健康检查等请求
        return !request.RequestUrl.Contains("/health");
    };
    
    // 自定义字段
    options.CustomFields = new List<string> { "UserId", "TraceId" };
});

// 添加Dashboard支持
builder.Services.AddHttpReportsDashboard();

var app = builder.Build();

// 启用HttpReports中间件
app.UseHttpReports();

// 启用Dashboard
app.UseHttpReportsDashboard();

app.MapGet("/", () => "HttpReports APM Ready");

// 自定义监控点示例
app.MapGet("/api/orders", async context =>
{
    // 记录自定义监控数据
    var monitor = context.RequestServices.GetService<IMonitorService>();
    await monitor.Record("OrderService", "GetOrders", 1);
    
    await context.Response.WriteAsync("Orders Data");
});

app.Run();