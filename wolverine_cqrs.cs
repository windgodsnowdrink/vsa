#:sdk Microsoft.NET.Sdk.Web
#:package Wolverine@1.0.0
#:package Microsoft.EntityFrameworkCore.Sqlite@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Microsoft.EntityFrameworkCore;
using Wolverine;

// 数据模型
public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}

// 数据库上下文
public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }
    public DbSet<TodoItem> Todos { get; set; }
}

// ========== CQRS 命令和查询 ==========
public record CreateTodoCommand(string Title) : IMessage;
public record CreateTodoResponse(int Id, string Title);
public record GetTodoQuery(int Id) : IMessage;
public record GetTodoResponse(int Id, string Title, bool IsCompleted);

// 命令处理程序
public class CreateTodoHandler
{
    public async Task<CreateTodoResponse> Handle(CreateTodoCommand command, TodoDbContext dbContext)
    {
        var todo = new TodoItem { Title = command.Title, IsCompleted = false };
        dbContext.Todos.Add(todo);
        await dbContext.SaveChangesAsync();
        return new CreateTodoResponse(todo.Id, todo.Title);
    }
}

// 查询处理程序
public class GetTodoHandler
{
    public async Task<GetTodoResponse> Handle(GetTodoQuery query, TodoDbContext dbContext)
    {
        var todo = await dbContext.Todos.FindAsync(query.Id);
        return todo == null ? null : new GetTodoResponse(todo.Id, todo.Title, todo.IsCompleted);
    }
}