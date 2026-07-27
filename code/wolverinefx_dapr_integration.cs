#:sdk Microsoft.NET.Sdk.Web
#:package WolverineFx@1.10.0
#:package WolverineFx.Dapr@1.10.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Wolverine;
using Wolverine.Dapr;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

public class DaprEventBusOptions
{
    [Required(ErrorMessage = "PubSubName is required")]
    [MinLength(3, ErrorMessage = "PubSubName must be at least 3 characters")]
    public string PubSubName { get; set; } = "pubsub";

    [Required(ErrorMessage = "StateStoreName is required")]
    [MinLength(3, ErrorMessage = "StateStoreName must be at least 3 characters")]
    public string StateStoreName { get; set; } = "statestore";

    [Range(1, 10, ErrorMessage = "RetryCount must be between 1 and 10")]
    public int RetryCount { get; set; } = 3;

    [Range(100, 5000, ErrorMessage = "Timeout must be between 100 and 5000 ms")]
    public int TimeoutMs { get; set; } = 1000;
}

public static class DaprEventBusExtensions
{
    public static WolverineOptions UseDaprEventBus(this WolverineOptions options, 
        Action<DaprEventBusOptions> configure)
    {
        options.Services.AddDaprClient(builder => builder
            .UseJsonSerializationOptions(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            }));

        options.Services.AddOptions<DaprEventBusOptions>()
            .Configure(configure)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        options.Services.AddHealthChecks()
            .AddCheck<DaprHealthCheck>("dapr")
            .AddPublisherCheck("dapr_pubsub", () => 
                new HealthCheckResult(HealthStatus.Healthy, "Dapr PubSub is healthy"));

        options.Services.AddOpenTelemetry()
            .WithTracing(builder => builder
                .AddSource("DaprEventBus")
                .AddDaprInstrumentation());

        // Configure Wolverine to use Dapr transport
        options.UseDapr(opts =>
        {
            opts.PubSubName = options.Services.BuildServiceProvider()
                .GetRequiredService<IOptions<DaprEventBusOptions>>().Value.PubSubName;
            
            // Enable Wolverine's built-in retry policies
            opts.RetryPolicy = new DaprRetryPolicy
            {
                MaximumAttempts = 3,
                PauseBetweenAttempts = TimeSpan.FromMilliseconds(250)
            };
            
            // Enable Wolverine's dead letter queue handling
            opts.DeadLetterQueueName = "deadletters";
        });

        return options;
    }
}

public class DaprHealthCheck : IHealthCheck
{
    private readonly DaprClient _daprClient;

    public DaprHealthCheck(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            await _daprClient.GetMetadataAsync(ct);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message);
        }
    }
}

// Message handler example with Wolverine
public static class OrderHandlers
{
    [DaprTopic("pubsub", "orderevent")]
    public static async Task Handle(OrderEvent @event, DaprClient daprClient)
    {
        // Process order event
        await daprClient.SaveStateAsync("statestore", @event.OrderId, @event);
    }
}