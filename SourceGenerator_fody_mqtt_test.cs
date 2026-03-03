#:sdk Microsoft.NET.Sdk.Web
#:package Fody@6.8.0
#:package MQTTnet@4.3.1
#:package xunit@2.6.0
#:package Moq@4.18.4
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using Xunit;
using Moq;

// 1. Fody 配置类
public class FodyConfiguration
{
    public static void Configure()
    {
        // 配置Fody编织器
        // ... existing code ...
    }
}

// 2. MQTT事件总线实现
public class MqttEventBus : IEventBus
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttFactory _mqttFactory;
    private readonly MqttClientOptions _options;

    public MqttEventBus(string brokerUrl, int port)
    {
        _mqttFactory = new MqttFactory();
        _mqttClient = _mqttFactory.CreateMqttClient();
        
        _options = new MqttClientOptionsBuilder()
            .WithTcpServer(brokerUrl, port)
            .WithCleanSession()
            .Build();
    }

    public async Task ConnectAsync()
    {
        await _mqttClient.ConnectAsync(_options);
    }

    public async Task PublishAsync(string topic, string payload)
    {
        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        await _mqttClient.PublishAsync(message);
    }

    public async Task SubscribeAsync(string topic, Func<string, Task> handler)
    {
        _mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            if (e.ApplicationMessage.Topic == topic)
            {
                await handler(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
            }
        };

        await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
            .WithTopic(topic)
            .Build());
    }
}

// 3. 单元测试
public class MqttEventBusTests
{
    [Fact]
    public async Task Should_Publish_And_Receive_Message()
    {
        // Arrange
        var mockMqttClient = new Mock<IMqttClient>();
        var eventBus = new MqttEventBus("localhost", 1883);
        
        // 使用反射设置私有字段
        var field = typeof(MqttEventBus).GetField("_mqttClient", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(eventBus, mockMqttClient.Object);

        string receivedMessage = null;
        await eventBus.SubscribeAsync("test/topic", msg => 
        {
            receivedMessage = msg;
            return Task.CompletedTask;
        });

        // Act
        await eventBus.PublishAsync("test/topic", "test message");

        // Assert
        mockMqttClient.Verify(m => 
            m.PublishAsync(It.IsAny<MqttApplicationMessage>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}

// 4. 集成测试
public class MqttIntegrationTests : IAsyncLifetime
{
    private MqttEventBus _eventBus;
    
    public async Task InitializeAsync()
    {
        _eventBus = new MqttEventBus("localhost", 1883);
        await _eventBus.ConnectAsync();
    }

    [Fact]
    public async Task Should_Receive_Published_Message()
    {
        // Arrange
        var tcs = new TaskCompletionSource<string>();
        await _eventBus.SubscribeAsync("integration/test", msg => 
        {
            tcs.SetResult(msg);
            return Task.CompletedTask;
        });

        // Act
        await _eventBus.PublishAsync("integration/test", "integration test message");
        var receivedMessage = await tcs.Task;

        // Assert
        Assert.Equal("integration test message", receivedMessage);
    }

    public async Task DisposeAsync()
    {
        await _eventBus.DisconnectAsync();
    }
}