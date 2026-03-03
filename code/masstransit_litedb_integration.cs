#:sdk Microsoft.NET.Sdk.Web
#:package MassTransit@8.2.3
#:package LiteDB@5.0.17
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable

using MassTransit;
using LiteDB;

var builder = WebApplication.CreateBuilder(args);

// 配置MassTransit使用LiteDB
builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .LiteDbRepository(r =>
        {
            r.ConnectionString = "Filename=orders.db;Connection=shared";
            r.DatabaseLogLevel = LiteDB.LogLevel.Error;
        });
    
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();
app.Run();

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    // 状态机实现
}

public class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
}