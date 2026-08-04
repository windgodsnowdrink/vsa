#:sdk Microsoft.NET.Sdk.Web
#:package MassTransit@8.2.3
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable

using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// 纯内存配置
builder.Services.AddMassTransit(x =>
{
    x.UsingInMemory((context, cfg) =>
    {
        cfg.UseConcurrencyLimit(Environment.ProcessorCount * 2);
        cfg.UseMessageRetry(r => r.Interval(3, 100));
        
        cfg.ConfigureEndpoints(context);
    });
    
    x.AddConsumer<InMemoryMessageConsumer>();
});

var app = builder.Build();
app.Run();

public class InMemoryMessageConsumer : IConsumer<InMemoryMessage>
{
    public Task Consume(ConsumeContext<InMemoryMessage> context)
    {
        // 处理内存消息
        return Task.CompletedTask;
    }
}

public record InMemoryMessage(string Content);