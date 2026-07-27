#:package Microsoft.SemanticKernel@1.0.0
#:package Microsoft.Extensions.Hosting@8.0.0
#:package Polly@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Microsoft.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;
using Polly;

namespace AutoGenIntegration
{
    public class AutoGenOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Endpoint { get; set; } = "https://api.autogen.ai/v1";
        public int Timeout { get; set; } = 30;
        public int MaxConcurrentRequests { get; set; } = 10;
        public bool EnableAOT { get; set; } = false;
        public bool EnableDistributedTracing { get; set; } = true;
        public string ModelName { get; set; } = "gpt-4";
        public int MaxTokens { get; set; } = 2048;
        public float Temperature { get; set; } = 0.7f;
        public int MemoryPoolSize { get; set; } = 1024 * 1024; // 1MB
        public int DisruptorRingSize { get; set; } = 1024;
        public int SpanBufferSize { get; set; } = 4096;
        public bool EnableZeroCopy { get; set; } = true;
    }

public interface IAutoGenService
{
    Task<string> GenerateAsync(string prompt);
    Task ProcessBatchAsync(IEnumerable<string> prompts);
}

public sealed class AutoGenService : IAutoGenService, IAsyncDisposable
{
    private readonly MemoryPool<byte> _memoryPool;
    private readonly ThreadLocal<Memory<byte>> _threadLocalBuffer;
    private readonly Disruptor.Dsl.Disruptor<string> _disruptor;
    private readonly RingBuffer<string> _ringBuffer;
    private readonly ISequenceBarrier _sequenceBarrier;
    private readonly Sequence _sequence = new Sequence();
    private readonly IKernel _kernel;
    private readonly AutoGenOptions _options;
    private readonly ILogger<AutoGenService> _logger;
    private readonly IMeter _meter;
    private readonly Counter<int> _requestCounter;
    private readonly Histogram<double> _responseTimeHistogram;
    private readonly ActionBlock<string> _processingPipeline;
    private readonly Channel<string> _batchChannel;
    private readonly AsyncRetryPolicy _retryPolicy;

    public AutoGenService(
        IKernel kernel,
        IOptions<AutoGenOptions> options,
        ILogger<AutoGenService> logger,
        IMeterFactory meterFactory)
    {
        // 内存池初始化
        _memoryPool = MemoryPool<byte>.Shared;
        _threadLocalBuffer = new ThreadLocal<Memory<byte>>(() => 
            _memoryPool.Rent(options.Value.SpanBufferSize).Memory);
            
        // Disruptor模式初始化
        _disruptor = new Disruptor.Dsl.Disruptor<string>(
            () => string.Empty,
            options.Value.DisruptorRingSize,
            TaskScheduler.Default,
            ProducerType.Multi,
            new BlockingWaitStrategy());
            
        _disruptor.HandleEventsWith(new BatchEventHandler(this));
        _ringBuffer = _disruptor.Start();
        _sequenceBarrier = _ringBuffer.NewBarrier();
        _kernel = kernel;
        _options = options.Value;
        _logger = logger;

        // 监控指标
        _meter = meterFactory.Create("AutoGenService");
        _requestCounter = _meter.CreateCounter<int>("autogen.requests.total");
        _responseTimeHistogram = _meter.CreateHistogram<double>("autogen.response.time.seconds");

        // 批处理管道
        _batchChannel = Channel.CreateBounded<string>(100);
        _processingPipeline = new ActionBlock<string>(
            async prompt => await ProcessSingleAsync(prompt),
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _options.MaxConcurrency
            });

        // 重试策略
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                _options.RetryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (ex, time, retryCount, context) => 
                {
                    _logger.LogWarning(ex, $"Request failed. Retry attempt {retryCount} in {time.TotalSeconds} seconds.");
                });
    }

    public async Task<string> GenerateAsync(string prompt)
    {
        using var timer = _responseTimeHistogram.NewTimer();
        _requestCounter.Add(1);
        
        if (_options.EnableZeroCopy)
        {
            var buffer = _threadLocalBuffer.Value;
            Encoding.UTF8.GetBytes(prompt, buffer.Span);
            return await ProcessWithZeroCopy(buffer.Span);
        }
        
        return await _retryPolicy.ExecuteAsync(async () => 
        {
            var function = _kernel.CreateFunctionFromPrompt(prompt);
            return await _kernel.InvokeAsync<string>(function);
        });
    }
    
    private async Task<string> ProcessWithZeroCopy(ReadOnlySpan<byte> promptSpan)
    {
        try
        {
            var prompt = Encoding.UTF8.GetString(promptSpan);
            var function = _kernel.CreateFunctionFromPrompt(prompt);
            return await _kernel.InvokeAsync<string>(function);
        }
        finally
        {
            promptSpan.Clear();
        }
    }

    public async Task ProcessBatchAsync(IEnumerable<string> prompts)
    {
        foreach (var prompt in prompts)
        {
            await _batchChannel.Writer.WriteAsync(prompt);
        }

        _batchChannel.Writer.Complete();
        await _processingPipeline.Completion;
    }

    private async Task ProcessSingleAsync(string prompt)
    {
        try
        {
            var result = await GenerateAsync(prompt);
            _logger.LogInformation("Generated content for prompt: {Prompt}", prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process prompt: {Prompt}", prompt);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _processingPipeline.Complete();
        await _processingPipeline.Completion;
    }
}

public static class AutoGenExtensions
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AutoGenService))]
    public static IServiceCollection AddAutoGenService(
        this IServiceCollection services,
        Action<AutoGenOptions> configure)
    {
        services.AddOptions<AutoGenOptions>()
            .Configure(configure)
            .Validate(options => 
            {
                if (string.IsNullOrEmpty(options.ApiKey))
                {
                    return false;
                }
                return true;
            });

        services.AddSingleton<IKernel>(provider => 
        {
            var options = provider.GetRequiredService<IOptions<AutoGenOptions>>().Value;
            return Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(
                    modelId: options.ModelId,
                    apiKey: options.ApiKey,
                    endpoint: options.Endpoint)
                .Build();
        });

        services.AddSingleton<IAutoGenService, AutoGenService>();
        services.AddLogging();
        services.AddMetrics();
        
        return services;
    }
}

[RequiresUnreferencedCode("AOT compilation may trim required code")]
public class Program
{
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddAutoGenService(options =>
                {
                    options.ModelId = "gpt-4";
                    options.ApiKey = "your-api-key";
                    options.Endpoint = "https://api.openai.com/v1";
                    options.MaxConcurrency = 8;
                });
            })
            .Build();

        host.Run();
    }
}