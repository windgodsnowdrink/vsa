using Microsoft.Extensions.DependencyInjection;
using PlcAiot.Shared.Services;

namespace PlcAiot.Shared.Extensions;

/// <summary>
/// DI registration helpers for the shared PLC·AIOT Razor class library.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers shared PLC·AIOT services and options.
    /// </summary>
    public static IServiceCollection AddPlcAiotShared(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<MqttClientServiceOptions>();

        // MqttClientService maintains connection state and should be scoped per consumer
        // (e.g., per circuit in Blazor Server, per page in MAUI/WinForms/WPF).
        services.AddScoped<MqttClientService>();

        return services;
    }

    /// <summary>
    /// Registers shared PLC·AIOT services with explicit MQTT broker configuration.
    /// </summary>
    public static IServiceCollection AddPlcAiotShared(
        this IServiceCollection services,
        Action<MqttClientServiceOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.Configure(configureOptions);
        services.AddPlcAiotShared();

        return services;
    }
}
