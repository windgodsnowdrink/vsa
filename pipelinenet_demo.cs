#:sdk Microsoft.NET.Sdk.Web
#:package PipelineNet@2.0.0
#:package MemoryPack@1.9.11
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using PipelineNet.Middleware;
using PipelineNet.Pipelines;
using System.Threading.Channels;

// Todo事件模型
[MemoryPackable]
public partial class TodoEvent
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}

// 验证中间件
public class ValidationMiddleware : IMiddleware<TodoEvent>
{
    public Task Run(TodoEvent parameter, Func<TodoEvent, Task> next)
    {
        if (string.IsNullOrEmpty(parameter.Title))
            throw new ArgumentException("Title不能为空");
        
        return next(parameter);
    }
}

// 日志中间件
public class LoggingMiddleware : IMiddleware<TodoEvent>
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public Task Run(TodoEvent parameter, Func<TodoEvent, Task> next)
    {
        _logger.LogInformation("处理Todo: {Id}", parameter.Id);
        return next(parameter);
    }
}

// 高性能管道处理器
public class TodoPipelineProcessor
{
    private readonly IPipeline<TodoEvent> _pipeline;
    private readonly Channel<TodoEvent> _channel;
    private readonly ObjectPool<Memory<byte>> _memoryPool;

    public TodoPipelineProcessor(ILogger<LoggingMiddleware> logger)
    {
        _memoryPool = new DefaultObjectPool<Memory<byte>>(
            new DefaultPooledObjectPolicy<Memory<byte>>(), 1000);
            
        _channel = Channel.CreateBounded<TodoEvent>(10000);
        _pipeline = new Pipeline<TodoEvent>()
            .UseTracing("TodoService")
            .UseBatchProcessing()
            .UseResilience()
            .Add(new ValidationMiddleware())
            .Add(new LoggingMiddleware(logger));
            .Add(new BatchProcessingMiddleware());
    }

    public async Task ProcessAsync(TodoEvent todo)
    {
        await _channel.Writer.WriteAsync(todo);
    }

    private async Task StartProcessing()
    {
        await foreach (var todo in _channel.Reader.ReadAllAsync())
        {
            using var memory = _memoryPool.Get();
            var span = memory.Span;
            
            // 零拷贝序列化
            var bytesWritten = MemoryPackSerializer.Serialize(span, todo);
            await _pipeline.Execute(memory[..bytesWritten]);
        }
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder();

// 配置服务
builder.Services.AddSingleton<TodoPipelineProcessor>();
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddMeter("TodoPipeline")
        .AddPrometheusExporter());

var app = builder.Build();

// 测试端点
app.MapPost("/todos", async (TodoPipelineProcessor pipeline, TodoEvent todo) =>
{
    await pipeline.ProcessAsync(todo);
    return Results.Ok();
});

app.Run();


public class BatchProcessingMiddleware : IMiddleware<TodoEvent>
{
    private readonly int _batchSize;
    private readonly TimeSpan _batchTimeout;
    private readonly ObjectPool<List<TodoEvent>> _batchPool;

    public BatchProcessingMiddleware(int batchSize = 100, TimeSpan? batchTimeout = null)
    {
        _batchSize = batchSize;
        _batchTimeout = batchTimeout ?? TimeSpan.FromMilliseconds(500);
        _batchPool = new DefaultObjectPool<List<TodoEvent>>(
            new ListPoolPolicy(), 10);
    }

    public async Task Run(TodoEvent parameter, Func<TodoEvent, Task> next)
    {
        var batch = _batchPool.Get();
        try
        {
            batch.Add(parameter);
            if (batch.Count >= _batchSize)
            {
                await ProcessBatch(batch);
                return;
            }

            using var cts = new CancellationTokenSource(_batchTimeout);
            while (batch.Count < _batchSize && !cts.Token.IsCancellationRequested)
            {
                await Task.Delay(100, cts.Token);
            }

            await ProcessBatch(batch);
        }
        finally
        {
            batch.Clear();
            _batchPool.Return(batch);
        }
    }

    private async Task ProcessBatch(List<TodoEvent> batch)
    {
        using var memory = MemoryPackSerializer.Serialize(batch);
        // 批量处理逻辑...
    }
}

private class ListPoolPolicy : IPooledObjectPolicy<List<TodoEvent>>
{
    public List<TodoEvent> Create() => new(100);
    public bool Return(List<TodoEvent> obj) => true;
}


public class ResilientMiddleware : IMiddleware<TodoEvent>
{
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;

    public ResilientMiddleware()
    {
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, attempt => 
                TimeSpan.FromSeconds(Math.Pow(2, attempt)));
            
        _circuitBreaker = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }

    public Task Run(TodoEvent parameter, Func<TodoEvent, Task> next)
    {
        return Policy.WrapAsync(_retryPolicy, _circuitBreaker)
            .ExecuteAsync(() => next(parameter));
    }
}

public class TracingMiddleware : IMiddleware<TodoEvent>
{
    private readonly ActivitySource _activitySource;

    public TracingMiddleware(string serviceName)
    {
        _activitySource = new ActivitySource(serviceName);
    }

    public async Task Run(TodoEvent parameter, Func<TodoEvent, Task> next)
    {
        using var activity = _activitySource.StartActivity("ProcessTodo");
        activity?.AddTag("todo.id", parameter.Id);
        
        try
        {
            await next(parameter);
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


public static class PipelineBuilderExtensions
{
    public static IPipeline<T> UseBatchProcessing<T>(
        this IPipeline<T> pipeline, 
        int batchSize = 100, 
        TimeSpan? batchTimeout = null)
    {
        return pipeline.Add(new BatchProcessingMiddleware(batchSize, batchTimeout));
    }

    public static IPipeline<T> UseResilience<T>(this IPipeline<T> pipeline)
    {
        return pipeline.Add(new ResilientMiddleware());
    }

    public static IPipeline<T> UseTracing<T>(
        this IPipeline<T> pipeline, 
        string serviceName)
    {
        return pipeline.Add(new TracingMiddleware(serviceName));
    }
}

// 使用示例
_pipeline = new Pipeline<TodoEvent>()
    .UseTracing("TodoService")
    .UseBatchProcessing()
    .UseResilience()
    .Add(new ValidationMiddleware())
    .Add(new LoggingMiddleware(logger));