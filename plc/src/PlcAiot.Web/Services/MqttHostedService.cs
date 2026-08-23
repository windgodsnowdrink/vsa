using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;

namespace PlcAiot.Web.Services;

/// <summary>
/// Background service that connects to the MQTT broker, subscribes to
/// PLC telemetry/status topics and logs incoming messages.
/// </summary>
public class MqttHostedService : BackgroundService
{
    private readonly ILogger<MqttHostedService> _logger;
    private readonly IConfiguration _configuration;
    private IMqttClient? _client;

    public MqttHostedService(ILogger<MqttHostedService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var brokerHost = _configuration["Mqtt:Host"] ?? "localhost";
        var brokerPort = _configuration.GetValue<int>("Mqtt:Port", 1883);

        var factory = new MqttFactory();
        _client = factory.CreateMqttClient();

        _client.ApplicationMessageReceivedAsync += e =>
        {
            _logger.LogInformation("MQTT message received on topic {Topic}: {Payload}",
                e.ApplicationMessage.Topic,
                GetPayloadString(e.ApplicationMessage.PayloadSegment));
            return Task.CompletedTask;
        };

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(brokerHost, brokerPort)
            .WithProtocolVersion(MqttProtocolVersion.V500)
            .WithClientId($"plc-aiot-web-{Guid.NewGuid()}")
            .Build();

        await _client.ConnectAsync(options, stoppingToken);

        var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter(f => f.WithTopic("plc/+/telemetry"))
            .WithTopicFilter(f => f.WithTopic("plc/+/status"))
            .Build();

        await _client.SubscribeAsync(subscribeOptions, stoppingToken);

        _logger.LogInformation("MQTT hosted service started. Broker: {Host}:{Port}", brokerHost, brokerPort);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Expected when service is stopping.
        }

        if (_client.IsConnected)
        {
            await _client.DisconnectAsync();
        }
    }

    private static string GetPayloadString(ArraySegment<byte> payload)
    {
        if (payload.Count == 0)
        {
            return string.Empty;
        }

        return Encoding.UTF8.GetString(payload.Array ?? Array.Empty<byte>(), payload.Offset, payload.Count);
    }
}
