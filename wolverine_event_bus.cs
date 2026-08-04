#:sdk Microsoft.NET.Sdk.Web
#:package WolverineFx.Http@1.0.0-rc1
#:package WolverineFx.Persistence.EntityFrameworkCore@1.0.0-rc1
#:package Microsoft.EntityFrameworkCore.SqlServer@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using Wolverine;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

// 配置数据库连接
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 添加Wolverine服务
builder.Host.UseWolverine(opts =>
{
    // 配置持久化存储
    opts.PersistMessagesWithEntityFrameworkCore<TodoDbContext>();
    
    // 配置消息重试策略
    opts.Policies.ConfigureConcurrencyForAllHandlers(10);
    opts.Policies.RetryOnException<Exception>(3);
});

var app = builder.Build();

// 应用数据库迁移
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    dbContext.Database.Migrate();
}

// 启动Wolverine
await app.Services.GetRequiredService<ICommandBus>().BootstrapAsync();

app.Run();

// 事件定义
public record TodoCreated(int Id, string Title);
public record TodoCompleted(int Id);

// 事件处理器
public class TodoEventHandler
{
    private readonly ObjectPool<Memory<byte>> _memoryPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public TodoEventHandler(ObjectPool<Memory<byte>> memoryPool, TailLatencyOptimizer latencyOptimizer)
    {
        _memoryPool = memoryPool;
        _latencyOptimizer = latencyOptimizer;
    }

    [WolverineHandler]
    public void Handle(TodoCreated created)
    {
        _latencyOptimizer.Optimize(() =>
        {
            var memory = _memoryPool.Get();
            try
            {
                var data = Encoding.UTF8.GetBytes($"Todo created: ID={created.Id}, Title={created.Title}");
                data.AsSpan().CopyTo(memory.Span);
                Console.WriteLine(Encoding.UTF8.GetString(memory.Span.Slice(0, data.Length)));
            }
            finally
            {
                _memoryPool.Return(memory);
            }
        });
    }

    [WolverineHandler]
    public void Handle(TodoCompleted completed)
    {
        Console.WriteLine($"Todo completed: ID={completed.Id}");
    }
}