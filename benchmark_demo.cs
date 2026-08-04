#:sdk Microsoft.NET.Sdk.Web
#:package BenchmarkDotNet@0.13.12
#:package Microsoft.EntityFrameworkCore.InMemory@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using System.Threading.Channels;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks.Dataflow;

// Todo数据模型
public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}

// 数据库上下文
public class TodoDbContext : DbContext
{
    public DbSet<TodoItem> Todos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("TodoBenchmark");
    }
}

// 性能测试类
[MemoryDiagnoser]
[ThreadingDiagnoser]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
public class TodoBenchmark
{
    private TodoDbContext _dbContext;
    private Channel<TodoItem> _channel;
    private ObjectPool<TodoItem> _objectPool;

    [GlobalSetup]
    public void Setup()
    {
        _dbContext = new TodoDbContext();
        _channel = Channel.CreateUnbounded<TodoItem>();
        _objectPool = new DefaultObjectPool<TodoItem>(
            new DefaultPooledObjectPolicy<TodoItem>(), 1000);
        
        // 初始化测试数据
        for (int i = 0; i < 1000; i++)
        {
            _dbContext.Todos.Add(new TodoItem 
            { 
                Id = i, 
                Title = $"Todo {i}", 
                IsCompleted = false 
            });
        }
        _dbContext.SaveChanges();
    }

    [Benchmark]
    public async Task CreateTodo()
    {
        var todo = _objectPool.Get();
        try
        {
            todo.Id = Random.Shared.Next(1000, 10000);
            todo.Title = $"New Todo {todo.Id}";
            todo.IsCompleted = false;
            
            _dbContext.Todos.Add(todo);
            await _dbContext.SaveChangesAsync();
        }
        finally
        {
            _objectPool.Return(todo);
        }
    }

    [Benchmark]
    public async Task<TodoItem> ReadTodo()
    {
        var id = Random.Shared.Next(0, 1000);
        return await _dbContext.Todos.FindAsync(id);
    }

    [Benchmark]
    public async Task UpdateTodo()
    {
        var id = Random.Shared.Next(0, 1000);
        var todo = await _dbContext.Todos.FindAsync(id);
        if (todo != null)
        {
            todo.IsCompleted = !todo.IsCompleted;
            await _dbContext.SaveChangesAsync();
        }
    }

    [Benchmark]
    public async Task DeleteTodo()
    {
        var id = Random.Shared.Next(0, 1000);
        var todo = await _dbContext.Todos.FindAsync(id);
        if (todo != null)
        {
            _dbContext.Todos.Remove(todo);
            await _dbContext.SaveChangesAsync();
        }
    }

    [Benchmark]
    public async Task ChannelWriteRead()
    {
        var todo = new TodoItem 
        { 
            Id = Random.Shared.Next(1000, 10000),
            Title = $"Channel Todo",
            IsCompleted = false 
        };
        
        await _channel.Writer.WriteAsync(todo);
        await _channel.Reader.ReadAsync();
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<TodoBenchmark>();
    }
}