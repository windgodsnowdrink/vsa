#:sdk Microsoft.NET.Sdk.Web
#:package LongChain.NET@1.0.0
#:package Microsoft.Extensions.Options@8.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using LongChain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

public class LongChainOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = "https://api.longchain.ai";
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxConcurrentRequests { get; set; } = 10;
    public bool EnableAOT { get; set; } = true;
    public bool EnableDistributedTracing { get; set; } = true;
}

public interface ILongChainService
{
    Task<string> GenerateTextAsync(string prompt, CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> StreamGenerateTextAsync(string prompt, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> BatchGenerateTextAsync(IEnumerable<string> prompts, CancellationToken cancellationToken = default);
    void EnableTracing(ActivitySource activitySource);
    void ConfigureRetryPolicy(IAsyncPolicy retryPolicy);
}

public class LongChainService : ILongChainService
{
    private readonly LongChainClient _client;
    private readonly ILogger<LongChainService> _logger;
    private readonly Channel<string> _requestChannel;
    private readonly ActivitySource _activitySource;
    private IAsyncPolicy _retryPolicy;

    public LongChainService(
        LongChainClient client,
        ILogger<LongChainService> logger,
        IOptions<LongChainOptions> options,
        ActivitySource activitySource)
    {
        _client = client;
        _logger = logger;
        _activitySource = activitySource;
        _requestChannel = Channel.CreateBounded<string>(options.Value.MaxConcurrentRequests);
    }

    public async Task<string> GenerateTextAsync(string prompt, CancellationToken cancellationToken = default)
    {
        using var activity = _activitySource.StartActivity("LongChain.GenerateText");
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            return await _client.GenerateTextAsync(prompt, cancellationToken);
        });
    }

    public async IAsyncEnumerable<string> StreamGenerateTextAsync(string prompt, CancellationToken cancellationToken = default)
    {
        using var activity = _activitySource.StartActivity("LongChain.StreamGenerateText");
        await foreach (var chunk in _client.StreamGenerateTextAsync(prompt, cancellationToken))
        {
            yield return chunk;
        }
    }

    public async Task<IReadOnlyList<string>> BatchGenerateTextAsync(IEnumerable<string> prompts, CancellationToken cancellationToken = default)
    {
        using var activity = _activitySource.StartActivity("LongChain.BatchGenerateText");
        var results = new List<string>();
        await Parallel.ForEachAsync(prompts, cancellationToken, async (prompt, ct) =>
        {
            var result = await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _client.GenerateTextAsync(prompt, ct);
            });
            results.Add(result);
        });
        return results;
    }

    public void EnableTracing(ActivitySource activitySource)
    {
        _activitySource = activitySource;
    }

    public void ConfigureRetryPolicy(IAsyncPolicy retryPolicy)
    {
        _retryPolicy = retryPolicy;
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLongChain(this IServiceCollection services, Action<LongChainOptions> configureOptions)
    {
        services.AddOptions<LongChainOptions>().Configure(configureOptions);
        
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<LongChainOptions>>().Value;
            return new LongChainClient(options.ApiKey, options.Endpoint)
            {
                Timeout = options.Timeout
            };
        });
        
        services.AddSingleton<ActivitySource>(sp => 
            new ActivitySource("LongChainService"));
        
        services.AddSingleton<IAsyncPolicy>(sp => 
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(1));
        });
        
        services.AddSingleton<ILongChainService, LongChainService>();
        
        return services;
    }
}

// Example usage
var builder = WebApplication.CreateBuilder();
builder.Services.AddLongChain(options =>
{
    options.ApiKey = "your-api-key";
    options.Endpoint = "https://api.longchain.ai";
    options.Timeout = TimeSpan.FromSeconds(30);
    options.MaxConcurrentRequests = 10;
    options.EnableAOT = true;
    options.EnableDistributedTracing = true;
});

var app = builder.Build();
app.MapGet("/generate", async (ILongChainService service, string prompt) =>
{
    return await service.GenerateTextAsync(prompt);
});
app.Run();