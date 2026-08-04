#:sdk Microsoft.NET.Sdk.Web
#:package WatchDog.NET@3.0.0
#:package WatchDog.Alerting@3.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using WatchDog.Alerting;

var builder = WebApplication.CreateBuilder();

// 配置报警规则
builder.Services.AddWatchDogAlerting(alert => {
    alert.AddRule(new AlertRule {
        Name = "高延迟请求",
        Condition = x => x.ResponseTime > 1000, // 1秒阈值
        NotifyType = NotifyType.Email | NotifyType.Webhook,
        NotifyEmails = new[] { "devops@example.com" },
        WebhookUrl = "https://alert.example.com/webhook",
        CheckInterval = TimeSpan.FromMinutes(5)
    });

    alert.AddRule(new AlertRule {
        Name = "内存溢出风险",
        Condition = x => GC.GetTotalMemory(false) > 500 * 1024 * 1024, // 500MB阈值
        NotifyType = NotifyType.SMS,
        PhoneNumbers = new[] { "+8613800138000" }
    });
});

var app = builder.Build();
app.UseWatchDog();
app.MapGet("/", () => "WatchDog Alerting Ready");
app.Run();