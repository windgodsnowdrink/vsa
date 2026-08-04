#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.SemanticKernel@1.0.0
#:package Microsoft.Extensions.Http@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Microsoft.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics.Metrics;
using System.Threading.Channels;
using Polly;

namespace SemanticKernelIntegration
{
    public class SemanticKernelOptions
{
    public string DeploymentName { get; set; } = "text-davinci-003";
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public int MaxRetryCount { get; set; } = 3;
    public int MemoryPoolSize { get; set; } = 1024 * 1024;
    public int ChannelCapacity { get; set; } = 1000;
    public int DisruptorRingSize { get; set; } = 1024;
    public bool EnableZeroCopy { get; set; } = true;
    public int SpanBufferSize { get; set; } = 4096;
}

public interface ISemanticKernelService
{
    Task<string> GenerateTextAsync(string prompt, CancellationToken cancellationToken = default);
}

public class SemanticKernelService : ISemanticKernelService
{
    private readonly IKernel _kernel;
    private readonly Channel<string> _inputChannel;
    private readonly Meter _meter = new("SemanticKernel");
    private readonly Counter<int> _requestCounter;
    private readonly MemoryPool<byte> _memoryPool;
    private readonly SemanticKernelOptions _options;

    public SemanticKernelService(IOptions<SemanticKernelOptions> options)
    {
        _options = options.Value;
        _memoryPool = MemoryPool<byte>.Shared;
        _inputChannel = Channel.CreateBounded<string>(_options.ChannelCapacity);
        _requestCounter = _meter.CreateCounter<int>("requests");

        var kernelBuilder = Kernel.Builder
            .WithRetryConfig(new RetryConfig
            {
                MaxRetryCount = _options.MaxRetryCount,
                MinRetryDelay = TimeSpan.FromMilliseconds(200),
                MaxRetryDelay = TimeSpan.FromSeconds(1)
            });

        _kernel = kernelBuilder.Build();
    }

    public async Task<string> GenerateTextAsync(string prompt, CancellationToken cancellationToken = default)
    {
        _requestCounter.Add(1);
        
        using var memoryOwner = _memoryPool.Rent(_options.MemoryPoolSize);
        var memory = memoryOwner.Memory;
        
        await _inputChannel.Writer.WriteAsync(prompt, cancellationToken);
        
        var result = await _kernel.RunAsync(
            _inputChannel.Reader,
            cancellationToken,
            memory);
            
        return result.ToString();
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSemanticKernelService(this IServiceCollection services, Action<SemanticKernelOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<ISemanticKernelService, SemanticKernelService>();
        return services;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddSemanticKernelService(options =>
        {
            options.Endpoint = "https://your-endpoint.com";
            options.ApiKey = "your-api-key";
            options.DeploymentName = "text-davinci-003";
        });
        
        var app = builder.Build();
        
        app.MapGet("/generate", async (ISemanticKernelService service, string prompt) =>
        {
            return await service.GenerateTextAsync(prompt);
        });
        
        app.Run();
    }
}