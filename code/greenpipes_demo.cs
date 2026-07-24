#:sdk Microsoft.NET.Sdk.Web
#:package GreenPipes@4.3.0
#:package MassTransit@8.2.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Threading.Channels;
using GreenPipes;
using MassTransit;
using System.Threading.Tasks.Dataflow;

// Todo事件模型
public class TodoEvent
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}

// 管道过滤器1：验证过滤器
public class ValidateTodoFilter : IFilter<TodoEvent>
{
    public async Task Send(TodoContext context, IPipe<TodoContext> next)
    {
        if (string.IsNullOrEmpty(context.Message.Title))
            throw new ArgumentException("Title不能为空");
        
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("validate");
    }
}

// 管道过滤器2：日志过滤器
public class LogTodoFilter : IFilter<TodoEvent>
{
    private readonly ILogger<LogTodoFilter> _logger;

    public LogTodoFilter(ILogger<LogTodoFilter> logger)
    {
        _logger = logger;
    }

    public async Task Send(TodoContext context, IPipe<TodoContext> next)
    {
        _logger.LogInformation("处理Todo: {Id}", context.Message.Id);
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("logging");
    }
}

// 管道过滤器3：分布式追踪过滤器
public class DistributedTracingFilter : IFilter<TodoContext>
{
    public async Task Send(TodoContext context, IPipe<TodoContext> next)
    {
        using var activity = new ActivitySource("GreenPipes").StartActivity("ProcessTodo");
        activity?.AddTag("todo.id", context.Message.Id);
        
        try
        {
            await next.Send(context);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("tracing");
    }
}

// 在Startup中配置OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("GreenPipes")
        .AddZipkinExporter());

// 管道构建器
public class TodoPipelineBuilder
{
    private readonly IPipe<TodoContext> _pipeline;

    public TodoPipelineBuilder(ILogger<LogTodoFilter> logger)
    {
        _pipeline = Pipe.New<TodoContext>(cfg =>
        {
            cfg.UseConcurrencyLimit(10); // 并发控制
            cfg.UseRetry(r => r.Interval(3, TimeSpan.FromSeconds(1))); // 重试策略
            cfg.UseCircuitBreaker(cb => // 熔断器
            {
                cb.TripThreshold = 5;
                cb.ActiveThreshold = 1;
                cb.ResetInterval = TimeSpan.FromMinutes(1);
            });
            cfg.UseFilter(new ValidateTodoFilter());
            cfg.UseFilter(new LogTodoFilter(logger));
            // 使用自定义中间件
            cfg.UseCustomMiddleware(new CustomMiddleware(memoryCache));
        });
    }

    public async Task ProcessAsync(TodoEvent todo)
    {
        var context = new TodoContext(todo);
        await _pipeline.Send(context);
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder();

// 配置服务
builder.Services.AddSingleton<TodoPipelineBuilder>();
builder.Services.AddSingleton<ObjectPool<Memory<byte>>>(new DefaultObjectPool<Memory<byte>>(
    new DefaultPooledObjectPolicy<Memory<byte>>(), 1000));

var app = builder.Build();

// 测试端点
app.MapPost("/todos", async (TodoPipelineBuilder pipeline, TodoEvent todo) =>
{
    await pipeline.ProcessAsync(todo);
    return Results.Ok();
});

app.Run();


public class DynamicPipelineBuilder
{
    private readonly List<Type> _filterTypes = new();
    private readonly IServiceProvider _serviceProvider;

    public DynamicPipelineBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public DynamicPipelineBuilder AddFilter<TFilter>() where TFilter : class
    {
        _filterTypes.Add(typeof(TFilter));
        return this;
    }

    public IPipe<TodoContext> Build()
    {
        return Pipe.New<TodoContext>(cfg =>
        {
            cfg.UseConcurrencyLimit(10);
            
            foreach (var filterType in _filterTypes)
            {
                var filter = _serviceProvider.GetRequiredService(filterType);
                cfg.UseFilter((dynamic)filter);
            }
        });
    }
}

// 在Startup中注册
builder.Services.AddTransient<ValidateTodoFilter>();
builder.Services.AddTransient<LogTodoFilter>();

public class CustomMiddleware : 
    IPipeSpecification<TodoContext>,
    IFilter<TodoContext>
{
    private readonly IMemoryCache _cache;

    public CustomMiddleware(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void Apply(IPipeBuilder<TodoContext> builder)
    {
        builder.AddFilter(this);
    }

    public async Task Send(TodoContext context, IPipe<TodoContext> next)
    {
        var cacheKey = $"todo_{context.Message.Id}";
        if (!_cache.TryGetValue(cacheKey, out _))
        {
            _cache.Set(cacheKey, context.Message, TimeSpan.FromMinutes(5));
        }
        
        await next.Send(context);
    }

    public IEnumerable<ValidationResult> Validate()
    {
        yield break;
    }
}


public class PerformanceMetricsFilter : IFilter<TodoContext>
{
    private readonly Counter<int> _processedCounter;
    private readonly Histogram<double> _processingTime;

    public PerformanceMetricsFilter(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("GreenPipes");
        _processedCounter = meter.CreateCounter<int>("todos.processed");
        _processingTime = meter.CreateHistogram<double>("processing.time.ms");
    }

    public async Task Send(TodoContext context, IPipe<TodoContext> next)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await next.Send(context);
            _processedCounter.Add(1);
        }
        finally
        {
            stopwatch.Stop();
            _processingTime.Record(stopwatch.ElapsedMilliseconds);
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("metrics");
    }
}

// 在Startup中配置指标
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddMeter("GreenPipes")
        .AddPrometheusExporter());