using System.Security.Authentication;
using System.Text;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;

namespace PlcAiot.Shared.Services;

/// <summary>
/// Shared MQTT client wrapper usable from Web, Desktop, and Mobile via DI.
/// Wraps <see cref="IManagedMqttClient"/> so consumers get auto-reconnect and
/// queued publishing without managing the lifecycle manually.
/// </summary>
public sealed class MqttClientService : IDisposable
{
    private readonly IManagedMqttClient _client;
    private readonly ManagedMqttClientOptions _options;
    private bool _disposed;

    /// <summary>
    /// Raised when an MQTT application message is received from any subscribed topic.
    /// </summary>
    public event EventHandler<MqttApplicationMessageReceivedEventArgs>? OnMessageReceived;

    public MqttClientService(IOptions<MqttClientServiceOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = BuildManagedOptions(options.Value);
        _client = new MqttFactory().CreateManagedMqttClient();
        _client.ApplicationMessageReceivedAsync += args =>
        {
            OnMessageReceived?.Invoke(this, args);
            return Task.CompletedTask;
        };
    }

    /// <summary>
    /// True when the underlying client is currently connected to the broker.
    /// </summary>
    public bool IsConnected => _client.IsConnected;

    /// <summary>
    /// Starts the managed client and begins connecting to the configured broker.
    /// </summary>
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        await _client.StartAsync(_options).ConfigureAwait(false);
    }

    /// <summary>
    /// Stops the managed client and cleanly disconnects from the broker.
    /// </summary>
    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        await _client.StopAsync(true).ConfigureAwait(false);
    }

    /// <summary>
    /// Subscribes to an MQTT topic with QoS 1.
    /// </summary>
    public async Task SubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);
        ObjectDisposedException.ThrowIf(_disposed, this);

        var filter = new MqttTopicFilterBuilder()
            .WithTopic(topic)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        await _client.SubscribeAsync(new[] { filter }).ConfigureAwait(false);
    }

    /// <summary>
    /// Publishes a UTF-8 payload to an MQTT topic with QoS 1.
    /// </summary>
    public async Task PublishAsync(string topic, string payload, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);
        ArgumentNullException.ThrowIfNull(payload);
        ObjectDisposedException.ThrowIf(_disposed, this);

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(Encoding.UTF8.GetBytes(payload))
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag(false)
            .Build();

        await _client.EnqueueAsync(message).ConfigureAwait(false);
    }

    /// <summary>
    /// Publishes a raw byte payload to an MQTT topic with QoS 1.
    /// </summary>
    public async Task PublishAsync(string topic, byte[] payload, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);
        ArgumentNullException.ThrowIfNull(payload);
        ObjectDisposedException.ThrowIf(_disposed, this);

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag(false)
            .Build();

        await _client.EnqueueAsync(message).ConfigureAwait(false);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _client?.Dispose();
        _disposed = true;
    }

    private static ManagedMqttClientOptions BuildManagedOptions(MqttClientServiceOptions options)
    {
        var clientOptionsBuilder = new MqttClientOptionsBuilder()
            .WithTcpServer(options.Host, options.Port)
            .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500)
            .WithClientId(options.ClientId ?? $"plca-{Guid.NewGuid():N}");

        if (!string.IsNullOrWhiteSpace(options.Username) && !string.IsNullOrWhiteSpace(options.Password))
        {
            clientOptionsBuilder.WithCredentials(options.Username, options.Password);
        }

        if (options.UseTls)
        {
            clientOptionsBuilder.WithTlsOptions(o =>
                o.WithSslProtocols(SslProtocols.Tls12 | SslProtocols.Tls13));
        }

        return new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(options.AutoReconnectDelay)
            .WithClientOptions(clientOptionsBuilder.Build())
            .Build();
    }
}

/// <summary>
/// Configuration options for <see cref="MqttClientService"/>.
/// </summary>
public sealed class MqttClientServiceOptions
{
    public string Host { get; set; } = "localhost";

    public int Port { get; set; } = 1883;

    public string? ClientId { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public bool UseTls { get; set; }

    public TimeSpan AutoReconnectDelay { get; set; } = TimeSpan.FromSeconds(5);
}
