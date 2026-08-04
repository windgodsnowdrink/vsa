#:sdk Microsoft.NET.Sdk.Web
#:package WatchDog.NET@3.0.0
#:package WatchDog.Serilog@3.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using WatchDog;
using WatchDog.src.Enums;

var builder = WebApplication.CreateBuilder();

// 配置WatchDog监控服务
builder.Services.AddWatchDogServices(settings =>
{
    settings.IsAutoClear = true; // 自动清理旧日志
    settings.ClearTimeSchedule = WatchDogAutoClearScheduleEnum.Daily; // 每日清理
    settings.DbDriverOption = WatchDogDbDriverEnum.MSSQL; // 使用SQL Server存储
    settings.SetExternalDbConnString = "Server=.;Database=WatchDog;Trusted_Connection=True;";
    
    // 高级配置
    settings.WatchPageUsername = "admin"; // 监控面板用户名
    settings.WatchPagePassword = "admin@123"; // 监控面板密码
    settings.Blacklist = "/health,/favicon.ico"; // 黑名单路由
    settings.EnableExceptionLogging = true; // 启用异常日志
    settings.EnableRequestLogging = true; // 启用请求日志
    settings.Serializer = WatchDogSerializerEnum.Newtonsoft; // 使用Newtonsoft序列化
});

// 集成Serilog日志
builder.Logging.AddWatchDogSerilog(config =>
{
    config.MinimumLevel.Information(); // 最小日志级别
    config.WriteTo.WatchDog(); // 写入WatchDog
});

var app = builder.Build();

// 启用WatchDog中间件
app.UseWatchDog(options =>
{
    options.WatchPageUsername = "admin";
    options.WatchPagePassword = "admin@123";
    options.Blacklist = "/health,/favicon.ico";
});

// 自定义监控点示例
app.MapGet("/api/products", () =>
{
    // 记录自定义监控数据
    WatchLogger.Log("获取产品数据");
    return Results.Ok();
});

app.MapGet("/", () => "WatchDog Monitoring Ready");
app.Run();