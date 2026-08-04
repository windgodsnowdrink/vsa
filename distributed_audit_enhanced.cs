#:sdk Microsoft.NET.Sdk.Web
#:package DotNetCore.CAP@7.2.0
#:package DotNetCore.CAP.Sqlite@7.2.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using DotNetCore.CAP;

var builder = WebApplication.CreateBuilder();
builder.Services.AddCap(x => {
    x.UseSqlite("Data Source=cap.db");
    x.UseDashboard();
});

// 审计事件发布者
public class AuditEventPublisher
{
    private readonly ICapPublisher _publisher;
    
    public async Task PublishAsync(AuditEvent auditEvent)
    {
        await _publisher.PublishAsync("audit.event.created", auditEvent);
    }
}

// 审计事件消费者
public class AuditEventConsumer : ICapSubscribe
{
    [CapSubscribe("audit.event.created")]
    public async Task ProcessAsync(AuditEvent auditEvent)
    {
        // 处理逻辑...
    }
}