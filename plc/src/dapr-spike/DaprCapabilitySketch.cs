// Sketch of how Dapr would drop into plc-host as a Tier2 capability module.
// The ICapabilityModule / ICarterModule shapes below mirror plc-host's contracts;
// they are re-declared here ONLY so this spike compiles standalone (we do NOT touch plc-host).
using Dapr.Workflow;
using Microsoft.AspNetCore.Routing;

namespace dapr_spike.sketch;

public interface ICapabilityModule
{
    string Id { get; }
    void RegisterServices(IServiceCollection services);
}

public interface ICarterModule
{
    void AddRoutes(IEndpointRouteBuilder app);
}

/// <summary>
/// One class == one capability. Discovered by Scrutor (AddCapabilityModules) and wired by
/// Carter (AddCarter/MapCarter) with zero Program.cs edits — same pattern as existing modules.
/// </summary>
public sealed class DaprCapability : ICapabilityModule, ICarterModule
{
    public string Id => "dapr";

    public void RegisterServices(IServiceCollection services)
    {
        // Dapr.AspNetCore: registers DaprClient + sidecar-facing helpers.
        services.AddDaprClient();
        // Optional: Workflow / Virtual Actor building blocks.
        services.AddDaprWorkflow();
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Example: expose a Pub/Sub subscription endpoint (AIOT event bus ingress).
        // app.MapPost("/dapr/subscribe", ...);  // Dapr invokes this on topic delivery
    }
}
