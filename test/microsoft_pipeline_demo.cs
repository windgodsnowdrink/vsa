#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.Hosting@8.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using System.Threading.Tasks.Dataflow;

// Todo事件模型
[MemoryPackable]
public partial class TodoEvent
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}

// Pipeline处理器
public class TodoPipeline : IAsyncDisposable
{
    private readonly Channel<TodoEvent> _channel;
    private readonly Task _processingTask;
    private readonly CancellationTokenSource _cts = new();
    private readonly ObjectPool<Memory<byte>> _memoryPool;

    public TodoPipeline()
    {
        _memoryPool = new DefaultObjectPool<Memory<byte>>(
            new DefaultPooledObjectPolicy<Memory<byte>>(), 1000);
            
        _channel = Channel.CreateBounded<TodoEvent>(new BoundedChannelOptions(10000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = false,
            SingleReader = false
        });

        _processingTask = Task.Run(ProcessItemsAsync);
    }

    // 生产者方法
    public async ValueTask ProcessAsync(TodoEvent todo)
    {
        await _channel.Writer.WriteAsync(todo, _cts.Token);
    }

    // 消费者处理逻辑
    private async Task ProcessItemsAsync()
    {
        await foreach (var todo in _channel.Reader.ReadAllAsync(_cts.Token))
        {
            using var memory = _memoryPool.Get();
            var span = memory.Span;
            
            // 使用MemoryPack进行零拷贝序列化
            var bytesWritten = MemoryPackSerializer.Serialize(span, todo);
            
            // 处理逻辑...
            await ProcessTodoAsync(memory[..bytesWritten]);
        }
    }

    private ValueTask ProcessTodoAsync(ReadOnlyMemory<byte> data)
    {
        // 实际业务处理
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _channel.Writer.Complete();
        await _processingTask;
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder();

// 配置服务
builder.Services.AddSingleton<TodoPipeline>();
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddMeter("TodoPipeline")
        .AddPrometheusExporter());

var app = builder.Build();

// 测试端点
app.MapPost("/todos", async (TodoPipeline pipeline, TodoEvent todo) =>
{
    await pipeline.ProcessAsync(todo);
    return Results.Ok();
});

app.Run();


public class BatchProcessingMiddleware
{
    private readonly int _batchSize;
    private readonly TimeSpan _batchTimeout;
    private readonly ObjectPool<List<TodoEvent>> _batchPool;

    public BatchProcessingMiddleware(int batchSize, TimeSpan batchTimeout)
    {
        _batchSize = batchSize;
        _batchTimeout = batchTimeout;
        _batchPool = new DefaultObjectPool<List<TodoEvent>>(
            new ListPoolPolicy(), 10);
    }

    public async Task ProcessAsync(TodoEvent todo, Func<TodoEvent, Task> next)
    {
        var batch = _batchPool.Get();
        try
        {
            batch.Add(todo);
            if (batch.Count >= _batchSize)
            {
                await ProcessBatchAsync(batch);
                return;
            }

            using var cts = new CancellationTokenSource(_batchTimeout);
            while (batch.Count < _batchSize && !cts.Token.IsCancellationRequested)
            {
                await Task.Delay(100, cts.Token);
            }

            await ProcessBatchAsync(batch);
        }
        finally
        {
            batch.Clear();
            _batchPool.Return(batch);
        }
    }

    private async Task ProcessBatchAsync(List<TodoEvent> batch)
    {
        // 使用MemoryPack批量序列化
        using var memory = MemoryPackSerializer.Serialize(batch);
        // 批量处理逻辑...
    }
}

private class ListPoolPolicy : IPooledObjectPolicy<List<TodoEvent>>
{
    public List<TodoEvent> Create() => new(100);
    public bool Return(List<TodoEvent> obj) => true;
}


public class BackpressureChannel<T> : Channel<T>
{
    private readonly SemaphoreSlim _throttler;
    private readonly Timer _throughputTimer;
    private int _messagesProcessed;
    
    public BackpressureChannel(int capacity, TimeSpan samplingInterval)
    {
        _throttler = new SemaphoreSlim(capacity);
        _throughputTimer = new Timer(AdjustThroughput, null, samplingInterval, samplingInterval);
        
        var channel = Channel.CreateBounded<T>(new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
        Reader = channel.Reader;
        Writer = channel.Writer;
    }

    public new async ValueTask WriteAsync(T item, CancellationToken ct = default)
    {
        await _throttler.WaitAsync(ct);
        try
        {
            await base.Writer.WriteAsync(item, ct);
        }
        finally
        {
            Interlocked.Increment(ref _messagesProcessed);
            _throttler.Release();
        }
    }

    private void AdjustThroughput(object? state)
    {
        var throughput = Interlocked.Exchange(ref _messagesProcessed, 0);
        // 动态调整背压逻辑...
    }
}


public class TracingMiddleware
{
    private readonly ActivitySource _activitySource;

    public TracingMiddleware(string serviceName)
    {
        _activitySource = new ActivitySource(serviceName);
    }

    public async Task<TResult> ExecuteAsync<TResult>(
        Func<Task<TResult>> operation,
        string operationName,
        IDictionary<string, object?>? tags = null)
    {
        using var activity = _activitySource.StartActivity(operationName);
        try
        {
            activity?.AddTag("thread.id", Environment.CurrentManagedThreadId);
            tags?.ToList().ForEach(t => activity?.AddTag(t.Key, t.Value));
            
            return await operation();
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}

// 在Startup中配置
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("TodoPipeline")
        .AddZipkinExporter());


public class ResilientPipeline
{
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    private readonly ILogger<ResilientPipeline> _logger;

    public ResilientPipeline(ILogger<ResilientPipeline> logger)
    {
        _logger = logger;
        
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, attempt => 
                TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                (ex, delay) => _logger.LogWarning(ex, "Retrying after delay: {Delay}", delay));
            
        _circuitBreaker = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (ex, breakDelay) => _logger.LogError(ex, "Circuit broken for {Delay}", breakDelay),
                onReset: () => _logger.LogInformation("Circuit reset"),
                onHalfOpen: () => _logger.LogInformation("Circuit half-open"));
    }

    public async Task ExecuteAsync(Func<Task> operation)
    {
        await Policy.WrapAsync(_retryPolicy, _circuitBreaker)
            .ExecuteAsync(operation);
    }
}