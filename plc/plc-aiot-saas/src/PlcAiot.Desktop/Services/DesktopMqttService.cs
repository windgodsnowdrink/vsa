using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using PlcAiot.Shared.Services;

namespace PlcAiot.Desktop.Services;

/// <summary>
/// Desktop-specific MQTT wrapper that exposes the shared <see cref="MqttClientService"/>
/// with convenient command/telemetry helpers.
/// </summary>
public sealed class DesktopMqttService
{
    private readonly MqttClientService _mqtt;

    public DesktopMqttService(MqttClientService mqtt)
    {
        _mqtt = mqtt;
    }

    /// <summary>
    /// Exposes incoming MQTT messages from all subscribed topics.
    /// </summary>
    public event EventHandler<MqttApplicationMessageReceivedEventArgs>? OnMessageReceived
    {
        add => _mqtt.OnMessageReceived += value;
        remove => _mqtt.OnMessageReceived -= value;
    }

    public Task ConnectAsync(CancellationToken cancellationToken = default)
        => _mqtt.ConnectAsync(cancellationToken);

    public Task DisconnectAsync(CancellationToken cancellationToken = default)
        => _mqtt.DisconnectAsync(cancellationToken);

    /// <summary>
    /// Publishes a command payload to the specified topic with QoS 1.
    /// </summary>
    public Task PublishCommandAsync(string topic, string payload, CancellationToken cancellationToken = default)
        => _mqtt.PublishAsync(topic, payload, cancellationToken);

    /// <summary>
    /// Subscribes to a telemetry topic.
    /// </summary>
    public Task SubscribeTelemetryAsync(string topic, CancellationToken cancellationToken = default)
        => _mqtt.SubscribeAsync(topic, cancellationToken);
}
