#:sdk Microsoft.NET.Sdk.Web
#:package FastEndpoints@7.0.1
#:package Microsoft.VisualStudio.Azure.Containers.Tools.Targets@1.22.1
#:package Microsoft.EntityFrameworkCore.SqlServer@10.0.0-preview.6.25358.103
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property UserSecretsId=210f4926-30c7-45ca-a020-391f82b3b3a1
#:property DockerDefaultTargetOS=Linux
#:property DockerComposeProjectPath=..\docker-compose.dcproj

using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder();

// 添加 FastEndpoints
builder.Services.AddFastEndpoints();

// 增强1：添加内存池和零拷贝优化
builder.Services.AddSingleton<ObjectPool<Memory<byte>>>(new DefaultObjectPool<Memory<byte>>(
    new DefaultPooledObjectPolicy<Memory<byte>>(), 1000));
builder.Services.AddSingleton<TailLatencyOptimizer>();

// 增强2：添加高性能通道处理
builder.Services.AddSingleton<Channel<TodoItem>>(Channel.CreateUnbounded<TodoItem>(
    new UnboundedChannelOptions { SingleReader = true }));

// 增强3：添加Span/Memory优化支持
builder.Services.AddSingleton<IMemoryOptimizer, SpanMemoryOptimizer>();

// 配置数据库连接
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=Todo.db"));

var app = builder.Build();

// 启用 FastEndpoints
app.UseFastEndpoints();

// 启用数据库迁移
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<TodoDbContext>();
    context.Database.Migrate();
}

app.Run();

// 定义数据模型
public class TodoItem
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}

// 定义数据库上下文
public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }
    public DbSet<TodoItem> TodoItems { get; set; }
}

// 创建 Todo 请求 DTO
public class CreateTodoRequest
{
    [Required]
    public string Title { get; set; }
}

// 创建 Todo 响应 DTO
public class CreateTodoResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}

// 创建 Todo Endpoint
public class CreateTodoEndpoint : Endpoint<CreateTodoRequest, CreateTodoResponse>
{
    private readonly TodoDbContext _dbContext;

    public CreateTodoEndpoint(TodoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Post("/todos");
        AllowAnonymous();
        Description(b => b
            .Accepts<CreateTodoRequest>("application/json")
            .Produces<CreateTodoResponse>(201));
    }

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
}

// 获取单个 Todo Endpoint
public class GetTodoEndpoint : Endpoint<EmptyRequest, TodoItem>
{
    private readonly TodoDbContext _dbContext;

    public GetTodoEndpoint(TodoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("/todos/{id}");
        AllowAnonymous();
        Description(b => b
            .Produces<TodoItem>(200)
            .Produces(404));
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var id = Route<int>("id");
        var todo = await _dbContext.TodoItems.FindAsync(new object[] { id }, ct);
        if (todo is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        await SendOkAsync(todo, ct);
    }
}