/*
 * MediatR生产级集成方案
 * 功能概述:
 * 1. 完整的CQRS实现(命令查询职责分离)
 * 2. 内置管道行为(日志、验证、性能监控等)
 * 3. 领域事件集成
 * 4. 内存事件总线实现
 * 5. 通知处理配置
 * 
 * 技术要点:
 * - 使用.NET 10最新特性
 * - 遵循生产级代码标准
 * - 包含详细注释和最佳实践
 */
#:sdk Microsoft.NET.Sdk
#:package MediatR@12.1.1
#:package MediatR.Extensions.Microsoft.DependencyInjection@12.1.1
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

// 1. 定义命令和查询
// CQRS模式中的命令 - 用于修改系统状态
public record CreateTodoCommand(string Title, string Description) : IRequest<Guid>;

// CQRS模式中的查询 - 用于获取系统状态
public record GetTodoQuery(Guid Id) : IRequest<TodoDto>;

// 2. 定义DTO(数据传输对象)
// 用于在应用层和表示层之间传输数据
// 使用record类型确保不可变性和值语义
public record TodoDto(Guid Id, string Title, string Description, DateTime CreatedAt);

// 3. 实现处理器
// 命令处理器 - 实现业务逻辑
// 遵循单一职责原则，每个处理器只处理一个命令
public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, Guid>
{
    private readonly ILogger<CreateTodoCommandHandler> _logger;
    private readonly ITodoRepository _repository;

    public CreateTodoCommandHandler(ILogger<CreateTodoCommandHandler> logger, ITodoRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        _logger.LogInformation("Creating todo with title: {Title}", request.Title);
        
        var todo = new Todo(Guid.NewGuid(), request.Title, request.Description, DateTime.UtcNow);
        await _repository.AddAsync(todo);
        
        _logger.LogInformation("Created todo {Id} in {Elapsed}ms", todo.Id, sw.ElapsedMilliseconds);
        return todo.Id;
    }
}

public class GetTodoQueryHandler : IRequestHandler<GetTodoQuery, TodoDto>
{
    private readonly ILogger<GetTodoQueryHandler> _logger;
    private readonly ITodoRepository _repository;

    public GetTodoQueryHandler(ILogger<GetTodoQueryHandler> logger, ITodoRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<TodoDto> Handle(GetTodoQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting todo with id: {Id}", request.Id);
        var todo = await _repository.GetByIdAsync(request.Id);
        return new TodoDto(todo.Id, todo.Title, todo.Description, todo.CreatedAt);
    }
}

// 4. 领域模型
// 核心业务对象，包含业务逻辑和状态
// 使用record类型确保领域模型的不可变性
public record Todo(Guid Id, string Title, string Description, DateTime CreatedAt);

// 5. 仓储接口
// 定义数据访问契约，遵循仓储模式
// 实现持久化无关性，便于替换底层存储
public interface ITodoRepository
{
    Task AddAsync(Todo todo);
    Task<Todo> GetByIdAsync(Guid id);
}

// 6. DI扩展方法
// 集中配置MediatR相关服务
// 遵循.NET依赖注入最佳实践
public static class MediatRServiceExtensions
{
    public static IServiceCollection AddMediatRServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(typeof(MediatRServiceExtensions).Assembly);
            // 添加管道行为
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));
            cfg.AddOpenBehavior(typeof(NotificationBehavior<>));
            cfg.AddOpenBehavior(typeof(ParallelBehavior<>));
            cfg.AddRequestPreProcessor(typeof(ValidationPreProcessor<>));
            cfg.AddRequestPostProcessor(typeof(LoggingPostProcessor<>));
            
            // 配置通知处理
            cfg.NotificationPublisher = new TaskWhenAllPublisher();
            cfg.NotificationPublisherType = typeof(TaskWhenAllPublisher);
        });
        
        services.AddScoped<ITodoRepository, TodoRepository>();
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddSingleton<IEventBus, InMemoryEventBus>();
        
        return services;
    }
}

// 7. 管道行为
// 日志记录行为 - AOP风格的横切关注点
// 自动记录请求处理前后的日志
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);
        var response = await next();
        _logger.LogInformation("Handled {RequestName}", typeof(TRequest).Name);
        return response;
    }
}

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = results.SelectMany(r => r.Errors).Where(f => f != null).ToList();
            
            if (failures.Count != 0)
                throw new ValidationException(failures);
        }
        return await next();
    }
}

// 7. 性能监控行为
// 监控请求处理耗时
// 超过500ms的请求会记录警告日志
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();
        
        if (sw.ElapsedMilliseconds > 500)
            _logger.LogWarning("Performance: {RequestName} took {Elapsed}ms", typeof(TRequest).Name, sw.ElapsedMilliseconds);
            
        return response;
    }
}

// 8. 领域事件集成
// 领域事件分发器接口
// 用于解耦领域模型和事件处理逻辑
public interface IDomainEventDispatcher
{
    Task Dispatch(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public DomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task Dispatch(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return _mediator.Publish(domainEvent, cancellationToken);
    }
}

// 9. 事件总线实现
// 内存事件总线接口
// 支持发布/订阅模式的事件驱动架构
public interface IEventBus
{
    Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : INotification;
}

public class InMemoryEventBus : IEventBus
{
    private readonly IMediator _mediator;

    public InMemoryEventBus(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : INotification
    {
        return _mediator.Publish(@event, cancellationToken);
    }
}

// 10. 示例使用
var services = new ServiceCollection();
services.AddLogging(configure => configure.AddConsole());
services.AddMediatRServices();

var provider = services.BuildServiceProvider();
var mediator = provider.GetRequiredService<IMediator>();
var eventBus = provider.GetRequiredService<IEventBus>();

// 创建Todo
var todoId = await mediator.Send(new CreateTodoCommand("Learn MediatR", "Implement CQRS pattern with MediatR"));

// 查询Todo
var todo = await mediator.Send(new GetTodoQuery(todoId));
Console.WriteLine($"Todo: {todo.Title}, Created: {todo.CreatedAt}");

// 发布领域事件
await eventBus.Publish(new TodoCreatedEvent(todoId, todo.Title));