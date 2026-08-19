namespace dapr_spike;

/// <summary>
/// Minimal POC: prove the Dapr .NET SDK API surface compiles and a DaprClient can be
/// constructed on .NET 11. The actual sidecar calls are wrapped in try/catch because no
/// Dapr sidecar is running in this spike — we only validate API shape + compile-time fit.
/// </summary>
public class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Building DaprClient via DaprClientBuilder...");
        var client = new Dapr.Client.DaprClientBuilder().Build();
        logger.LogInformation("DaprClient constructed OK (HTTP/gRPC client ready).");

        // 1) Metadata call (no state store configured) — exercises a real sidecar endpoint.
        try
        {
            var meta = await client.GetMetadataAsync(stoppingToken);
            logger.LogInformation("GetMetadataAsync OK. AppId={AppId}", meta?.Id);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                "GetMetadataAsync failed as expected without a running Dapr sidecar: {Type}: {Msg}",
                ex.GetType().Name, ex.Message);
        }

        // 2) State store write — exercises the State Management building block API shape.
        try
        {
            await client.SaveStateAsync("statestore", "device:line-1", new { Temp = 23.5, Ts = DateTimeOffset.UtcNow },
                cancellationToken: stoppingToken);
            logger.LogInformation("SaveStateAsync accepted.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                "SaveStateAsync failed as expected without a running Dapr sidecar: {Type}: {Msg}",
                ex.GetType().Name, ex.Message);
        }

        logger.LogInformation("POC complete: SDK compiles & constructs on net11; runtime needs Dapr sidecar.");
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
