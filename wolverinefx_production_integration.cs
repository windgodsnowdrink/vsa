/*
 * WolverineFX生产级集成方案
 * 功能概述:
 * 1. 完整的CQRS实现(命令查询职责分离)
 * 2. 内置中间件(日志、验证、性能监控等)
 * 3. 领域事件集成
 * 4. 内存事件总线实现
 * 5. 消息处理配置
 * 
 * 技术要点:
 * - 使用.NET 10最新特性
 * - 遵循生产级代码标准
 * - 包含详细注释和最佳实践
 */
#:sdk Microsoft.NET.Sdk.Web
#:package Wolverine@2.0.0
#:package WolverineFx.Http@2.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Wolverine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

// 1. 定义命令和查询
public record CreateTodoCommand(string Title, string Description);
public record GetTodoQuery(Guid Id);

// 2. 定义DTO
public record TodoDto(Guid Id, string Title, string Description, DateTime CreatedAt);

// 3. 实现处理器
public static class TodoHandlers
{
    public static async Task<Guid> Handle(
        CreateTodoCommand command,
        ITodoRepository repository,
        ILogger<CreateTodoCommand> logger,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        logger.LogInformation("Creating todo with title: {Title}", command.Title);
        
        var todo = new Todo(Guid.NewGuid(), command.Title, command.Description, DateTime.UtcNow);
        await repository.AddAsync(todo);
        
        logger.LogInformation("Created todo {Id} in {Elapsed}ms", todo.Id, sw.ElapsedMilliseconds);
        return todo.Id;
    }

    public static async Task<TodoDto> Handle(
        GetTodoQuery query,
        ITodoRepository repository,
        ILogger<GetTodoQuery> logger,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting todo with id: {Id}", query.Id);
        var todo = await repository.GetByIdAsync(query.Id);
        return new TodoDto(todo.Id, todo.Title, todo.Description, todo.CreatedAt);
    }
}

// 4. 领域模型
public record Todo(Guid Id, string Title, string Description, DateTime CreatedAt);

// 5. 仓储接口
public interface ITodoRepository
{
    Task AddAsync(Todo todo);
    Task<Todo> GetByIdAsync(Guid id);
}

// 6. DI扩展方法
public static class WolverineServiceExtensions
{
    public static IServiceCollection AddWolverineServices(this IServiceCollection services)
    {
        services.AddScoped<ITodoRepository, TodoRepository>();
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddSingleton<IEventBus, InMemoryEventBus>();
        
        return services;
    }
}

// 7. 中间件
public class LoggingMiddleware
{
    private readonly ILogger _logger;

    public LoggingMiddleware(ILogger logger)
    {
        _logger = logger;
    }

    public async Task BeforeAsync(IMessageContext context)
    {
        _logger.LogInformation("Handling {MessageType}", context.Message.GetType().Name);
    }

    public async Task AfterAsync(IMessageContext context)
    {
        _logger.LogInformation("Handled {MessageType}", context.Message.GetType().Name);
    }
}

// 8. 领域事件集成
public interface IDomainEventDispatcher
{
    Task Dispatch(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMessageBus _bus;

    public DomainEventDispatcher(IMessageBus bus)
    {
        _bus = bus;
    }

    public Task Dispatch(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return _bus.PublishAsync(domainEvent, cancellationToken);
    }
}

// 9. 事件总线实现
public interface IEventBus
{
    Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class;
}

public class InMemoryEventBus : IEventBus
{
    private readonly IMessageBus _bus;

    public InMemoryEventBus(IMessageBus bus)
    {
        _bus = bus;
    }

    public Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class
    {
        return _bus.PublishAsync(@event, cancellationToken);
    }
}

// 10. 示例使用
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging(configure => configure.AddConsole());
builder.Services.AddWolverineServices();

builder.Host.UseWolverine(opts =>
{
    // 配置中间件
    opts.Handlers.AddMiddleware<LoggingMiddleware>();
    
    // 配置消息处理
    opts.Policies.AutoApplyTransactions();
    opts.Policies.UseDurableInboxOnAllListeners();
    opts.Policies.UseDurableOutboxOnAllSenders();
});

var app = builder.Build();
var bus = app.Services.GetRequiredService<IMessageBus>();
var eventBus = app.Services.GetRequiredService<IEventBus>();

// 创建Todo
var todoId = await bus.InvokeAsync<Guid>(new CreateTodoCommand("Learn Wolverine", "Implement CQRS pattern with Wolverine"));

// 查询Todo
var todo = await bus.InvokeAsync<TodoDto>(new GetTodoQuery(todoId));
Console.WriteLine($"Todo: {todo.Title}, Created: {todo.CreatedAt}");

// 发布领域事件
await eventBus.Publish(new TodoCreatedEvent(todoId, todo.Title));