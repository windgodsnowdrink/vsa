#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.SemanticKernel@1.1.1
#:package Microsoft.SemanticKernel.Memory@1.1.1
#:package Microsoft.EntityFrameworkCore.Sqlite@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property UserSecretsId 210f4926-30c7-45ca-a020-391f82b3b3a1
#:property DockerDefaultTargetOS Linux
#:property DockerComposeProjectPath ..\docker-compose.dcproj

using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using System.ComponentModel.DataAnnotations;

// 数据模型
class TodoItem
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}

// 数据库上下文
class TodoDbContext : DbContext
{
    public DbSet<TodoItem> Todos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=todos.db");
    }
}

// Semantic Kernel 技能
class TodoSkill
{
    private readonly TodoDbContext _dbContext;

    public TodoSkill(TodoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [SKFunction, SKName("AddTodo")]
    [SKDescription("根据自然语言描述添加新的待办事项")]
    public async Task<string> AddTodoAsync(string description)
    {
        var newTodo = new TodoItem { Title = description, IsCompleted = false };
        _dbContext.Todos.Add(newTodo);
        await _dbContext.SaveChangesAsync();
        return $"已添加待办事项: {description}";
    }

    [SKFunction, SKName("ListTodos")]
    [SKDescription("列出所有待办事项")]
    public async Task<string> ListTodosAsync()
    {
        var todos = await _dbContext.Todos.ToListAsync();
        return todos.Count == 0 ? "没有待办事项" : "待办事项列表:\n" + string.Join("\n", todos.Select(t => $"[{(t.IsCompleted ? "✓" : " ")}] {t.Title}"));
    }
}

var builder = WebApplication.CreateBuilder();

// 配置数据库
builder.Services.AddDbContext<TodoDbContext>();

// 内存优化配置
builder.Services.AddSingleton<IMemoryStore>(sp => 
    new VolatileMemoryStore(
        new ThreadLocal<Span<byte>>(() => stackalloc byte[1024])));

// 内核构建初始化Semantic Kernel
builder.Services.AddSingleton<IKernel>(sp => {
    var kernel = Kernel.Builder
        .WithMemory(sp.GetRequiredService<IMemoryStore>())
        .WithLoggerFactory(LoggerFactory.Create(c => c.AddConsole()))
        .Build();
        
    // 插件加载
    kernel.ImportSkill(new TimeSkill(), "time");
    return kernel;
});

// 注册技能
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    dbContext.Database.EnsureCreated();
    kernel.ImportSkill(new TodoSkill(dbContext), "TodoSkill");
}

var app = builder.Build();

app.MapPost("/api/todos", async (string command) =>
{
    try
    {
        var result = await kernel.RunAsync(command, kernel.Skills.GetFunction("TodoSkill", "AddTodo"));
        return Results.Ok(result.Result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapGet("/api/todos", async () =>
{
    try
    {
        var result = await kernel.RunAsync("", kernel.Skills.GetFunction("TodoSkill", "ListTodos"));
        return Results.Ok(result.Result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.Run();