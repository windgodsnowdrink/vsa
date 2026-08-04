#:sdk Microsoft.NET.Sdk.Web
#:package Wolverine@2.0.0
#:package WolverineFx.Http@2.0.0
#:package WolverineFx.Postgresql@2.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Wolverine;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

// 1. 配置Wolverine
builder.Host.UseWolverine(opts =>
{
    // 使用PostgreSQL作为消息存储
    opts.PersistMessagesWithPostgresql("Host=localhost;Port=5432;Database=wolverine;Username=postgres;Password=P@ssw0rd");
    
    // 配置重试策略
    opts.Handlers.OnException<TimeoutException>()
        .RetryWithCooldown(50.Milliseconds(), 100.Milliseconds(), 250.Milliseconds());
    
    // 启用零拷贝消息处理
    opts.Advanced.UseZeroCopyMessageSerialization();
});

var app = builder.Build();

// 2. 映射Wolverine HTTP端点
app.MapWolverineEndpoints(opts =>
{
    // 配置JSON序列化
    opts.UseNewtonsoftJsonForSerialization();
    
    // 启用OpenAPI支持
    opts.UseOpenApi(o =>
    {
        o.Title = "Wolverine API";
        o.Version = "v1";
    });
});

app.Run();

// 3. 示例消息处理器
public static class TodoHandlers
{
    public static async Task<TodoCreated> Handle(CreateTodo command, IDocumentSession session)
    {
        var todo = new Todo { Name = command.Name };
        session.Store(todo);
        return new TodoCreated(todo.Id);
    }
}

// 4. HTTP端点
public static class TodoEndpoints
{
    [WolverinePost("/todos")]
    public static (TodoCreated, IResult) Create(CreateTodo command)
    {
        return (new TodoCreated(1), Results.Created($"/todos/1", null));
    }
}

public record CreateTodo(string Name);
public record TodoCreated(int Id);
public class Todo { public int Id { get; set; } public string Name { get; set; } }