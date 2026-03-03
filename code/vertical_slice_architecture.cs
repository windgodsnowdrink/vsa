#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.App.Ref@10.0.0-preview.4
#:package Microsoft.EntityFrameworkCore.SqlServer@10.0.0-preview.4
#:package Microsoft.AspNetCore.Mvc.NewtonsoftJson@10.0.0-preview.4
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property UserSecretsId 210f4926-30c7-45ca-a020-391f82b3b3a1
#:property DockerDefaultTargetOS Linux
#:property DockerComposeProjectPath ..\docker-compose.dcproj
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

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

// 切片：获取所有 Todo 项
app.MapGet("/todos", async (TodoDbContext db) =>
{
    return await db.TodoItems.ToListAsync();
}).Produces<List<TodoItem>>(StatusCodes.Status200OK);

// 切片：创建新的 Todo 项
app.MapPost("/todos", async (TodoItem todo, TodoDbContext db, IMessageBus bus) =>
{
    db.TodoItems.Add(todo);
    await db.SaveChangesAsync();
    
    // 发布领域事件
    await bus.PublishAsync(new TodoItemCreatedEvent(todo.Id));
    
    return Results.Created($"/todos/{todo.Id}", todo);
}).Accepts<TodoItem>("application/json").Produces<TodoItem>(StatusCodes.Status201Created);

// 切片：获取单个 Todo 项
app.MapGet("/todos/{id}", async (int id, TodoDbContext db) =>
{
    return await db.TodoItems.FindAsync(id) is TodoItem todo ? Results.Ok(todo) : Results.NotFound();
}).Produces<TodoItem>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);

// 切片：更新 Todo 项
app.MapPut("/todos/{id}", async (int id, TodoItem inputTodo, TodoDbContext db) =>
{
    var todo = await db.TodoItems.FindAsync(id);
    if (todo is null) return Results.NotFound();
    todo.Title = inputTodo.Title;
    todo.IsCompleted = inputTodo.IsCompleted;
    await db.SaveChangesAsync();
    return Results.NoContent();
}).Accepts<TodoItem>("application/json").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound);

// 切片：删除 Todo 项
app.MapDelete("/todos/{id}", async (int id, TodoDbContext db) =>
{
    if (await db.TodoItems.FindAsync(id) is TodoItem todo)
    {
        db.TodoItems.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
}).Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound);

var builder = WebApplication.CreateBuilder();

// 配置数据库连接
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=Todo.db"));

// 增强1：添加缓存层
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "TodoCache_";
});

// 增强2：添加内存池和性能优化
builder.Services.AddSingleton<ObjectPool<Memory<byte>>>(new DefaultObjectPool<Memory<byte>>(
    new DefaultPooledObjectPolicy<Memory<byte>>(), 1000));
builder.Services.AddSingleton<TailLatencyOptimizer>();

// 增强3：添加事件总线支持
builder.Services.AddWolverine(opts =>
{
    opts.UseEntityFrameworkCorePersistence<TodoDbContext>();
    opts.Policies.AutoApplyTransactions();
});

var app = builder.Build();

// 启用数据库迁移
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<TodoDbContext>();
    context.Database.Migrate();
}

app.Run();