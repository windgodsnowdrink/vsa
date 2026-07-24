#:sdk Microsoft.NET.Sdk.Web
#:package Mapster@8.5.0
#:package Mapster.DependencyInjection@8.5.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// 定义数据模型
public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}

// 定义DTO
public class TodoItemDto
{
    public int TodoId { get; set; }
    public string TodoTitle { get; set; } = string.Empty;
    public bool IsDone { get; set; }
}

// 配置Mapster运行时映射规则
TypeAdapterConfig<TodoItem, TodoItemDto>.NewConfig()
    .Map(dest => dest.TodoId, src => src.Id)
    .Map(dest => dest.TodoTitle, src => src.Title)
    .Map(dest => dest.IsDone, src => src.IsCompleted);

var builder = WebApplication.CreateBuilder();

// 注册Mapster到依赖注入容器
builder.Services.AddMapster();

// 增强1：添加内存池和零拷贝优化
builder.Services.AddSingleton<ObjectPool<Memory<byte>>>(new DefaultObjectPool<Memory<byte>>(
    new DefaultPooledObjectPolicy<Memory<byte>>(), 1000));
builder.Services.AddSingleton<TailLatencyOptimizer>();

// 增强2：添加高性能通道处理
builder.Services.AddSingleton<Channel<TodoItem>>(Channel.CreateUnbounded<TodoItem>(
    new UnboundedChannelOptions { SingleReader = true }));

// 增强3：添加Span/Memory优化支持
builder.Services.AddSingleton<IMemoryOptimizer, SpanMemoryOptimizer>();

var app = builder.Build();

app.MapGet("/todos/{id}", (int id, IMapper mapper) =>
{
    // 模拟从数据库获取Todo项
    var todoItem = new TodoItem { Id = id, Title = "示例任务", IsCompleted = false };
    
    // 使用Mapster进行对象映射
    var todoDto = mapper.Map<TodoItemDto>(todoItem);
    
    return Results.Ok(todoDto);
});

app.Run();


// 在HandleAsync方法中使用分布式事务
public override async Task HandleAsync(CreateTodoRequest req, CancellationToken ct)
{
    using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
    try
    {
        var todo = new TodoItem { Title = req.Title };
        _dbContext.TodoItems.Add(todo);
        await _dbContext.SaveChangesAsync(ct);
        
        // 发布领域事件
        await PublishAsync(new TodoItemCreatedEvent(todo.Id), ct);
        
        await transaction.CommitAsync(ct);
        
        var response = new CreateTodoResponse
        {
            Id = todo.Id,
            Title = todo.Title,
            IsCompleted = todo.IsCompleted
        };
        await SendCreatedAtAsync<GetTodoEndpoint>(new { id = todo.Id }, response, cancellation: ct);
    }
    catch
    {
        await transaction.RollbackAsync(ct);
        throw;
    }
}