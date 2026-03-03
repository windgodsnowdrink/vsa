#:sdk Microsoft.NET.Sdk.Web
#:package System.Threading.Tasks.Dataflow@8.0.0
#:package MemoryPack@1.9.11
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Threading.Tasks.Dataflow;
using System.Threading.Channels;

// Todo事件模型
[MemoryPackable]
public partial class TodoEvent
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}

// 高性能流处理器
public class TodoStreamProcessor : IAsyncDisposable
{
    private readonly TransformBlock<TodoEvent, ReadOnlyMemory<byte>> _transformBlock;
    private readonly ActionBlock<ReadOnlyMemory<byte>> _actionBlock;
    private readonly ObjectPool<Memory<byte>> _memoryPool;
    private readonly CancellationTokenSource _cts = new();

    public TodoStreamProcessor()
    {
        _memoryPool = new DefaultObjectPool<Memory<byte>>(
            new DefaultPooledObjectPolicy<Memory<byte>>(), 1000);

        var options = new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 10000,
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            EnsureOrdered = false,
            CancellationToken = _cts.Token
        };

        _transformBlock = new TransformBlock<TodoEvent, ReadOnlyMemory<byte>>(async todo =>
        {
            using var memory = _memoryPool.Get();
            var span = memory.Span;
            var bytesWritten = MemoryPackSerializer.Serialize(span, todo);
            return memory[..bytesWritten];
        }, options);

        _actionBlock = new ActionBlock<ReadOnlyMemory<byte>>(async data =>
        {
            // 实际业务处理
            await ProcessDataAsync(data);
        }, options);

        _transformBlock.LinkTo(_actionBlock, new DataflowLinkOptions { PropagateCompletion = true });
    }

    public async ValueTask ProcessAsync(TodoEvent todo)
    {
        await _transformBlock.SendAsync(todo, _cts.Token);
    }

    protected virtual ValueTask ProcessDataAsync(ReadOnlyMemory<byte> data)
    {
        // 子类实现具体处理逻辑
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _transformBlock.Complete();
        await _actionBlock.Completion;
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder();

// 配置服务
builder.Services.AddSingleton<TodoStreamProcessor>();
public class TracingMiddleware
{
    private readonly ActivitySource _activitySource;

    public TracingMiddleware(string serviceName)
    {
        _activitySource = new ActivitySource(serviceName);
    }

    public async Task<T> ExecuteWithTracing<T>(Func<Task<T>> operation, string operationName)
    {
        using var activity = _activitySource.StartActivity(operationName);
        try
        {
            activity?.AddTag("thread.id", Environment.CurrentManagedThreadId);
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
        .AddSource("DataflowPipeline")
        .AddZipkinExporter());
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddMeter("TodoStreamProcessor")
        .AddPrometheusExporter());

var app = builder.Build();

// 测试端点
app.MapPost("/todos", async (TodoStreamProcessor processor, TodoEvent todo) =>
{
    await processor.ProcessAsync(todo);
    return Results.Ok();
});

app.Run();


public class BatchProcessor
{
    private readonly BatchBlock<TodoEvent> _batchBlock;
    private readonly TransformBlock<TodoEvent[], ReadOnlyMemory<byte>> _transformBlock;
    private readonly ActionBlock<ReadOnlyMemory<byte>> _actionBlock;
    private readonly ObjectPool<Memory<byte>> _memoryPool;

    public BatchProcessor(int batchSize = 100)
    {
        _memoryPool = new DefaultObjectPool<Memory<byte>>(
            new DefaultPooledObjectPolicy<Memory<byte>>(), 1000);

        var options = new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 10000,
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            EnsureOrdered = false
        };

        _batchBlock = new BatchBlock<TodoEvent>(batchSize);
        _transformBlock = new TransformBlock<TodoEvent[], ReadOnlyMemory<byte>>(events =>
        {
            using var memory = _memoryPool.Get();
            var span = memory.Span;
            var bytesWritten = MemoryPackSerializer.Serialize(span, events);
            return memory[..bytesWritten];
        }, options);

        _actionBlock = new ActionBlock<ReadOnlyMemory<byte>>(async data =>
        {
            await ProcessBatchAsync(data);
        }, options);

        _batchBlock.LinkTo(_transformBlock);
        _transformBlock.LinkTo(_actionBlock);
    }
}


public class BackpressureStrategy
{
    private readonly SemaphoreSlim _throttler;
    private readonly Timer _throughputTimer;
    private int _messagesProcessed;
    
    public BackpressureStrategy(int capacity, TimeSpan samplingInterval)
    {
        _throttler = new SemaphoreSlim(capacity);
        _throughputTimer = new Timer(AdjustThroughput, null, samplingInterval, samplingInterval);
    }

    public async Task ApplyAsync(Func<Task> operation)
    {
        await _throttler.WaitAsync();
        try
        {
            await operation();
            Interlocked.Increment(ref _messagesProcessed);
        }
        finally
        {
            _throttler.Release();
        }
    }

    private void AdjustThroughput(object? state)
    {
        var throughput = Interlocked.Exchange(ref _messagesProcessed, 0);
        // 动态调整背压逻辑...
    }
}


public class ErrorHandlingStrategy
{
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    private readonly ILogger<ErrorHandlingStrategy> _logger;

    public ErrorHandlingStrategy(ILogger<ErrorHandlingStrategy> logger)
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

    public async Task ExecuteWithResilienceAsync(Func<Task> operation)
    {
        await Policy.WrapAsync(_retryPolicy, _circuitBreaker)
            .ExecuteAsync(operation);
    }
}