#:sdk Microsoft.NET.Sdk.Web
#:package MassTransit@8.2.3
#:package MQTTnet@4.1.4.436
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsights enable

using MassTransit;
using MQTTnet;
using MQTTnet.Client;

var builder = WebApplication.CreateBuilder(args);

// 配置MassTransit使用MQTTnet
builder.Services.AddMassTransit(x =>
{
    x.UsingMqtt((context, cfg) =>
    {
        cfg.Host("localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ReceiveEndpoint("mqtt-bridge", e =>
        {
            e.Consumer<RabbitToMqttBridgeConsumer>();
        });
        // 高性能配置
        cfg.UseConcurrencyLimit(Environment.ProcessorCount * 2);
        cfg.UseMessageRetry(r => r.Interval(3, 100));

        var factory = new MqttFactory();
        var client = factory.CreateMqttClient();

        // 连接配置
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost")
            .Build();

        client.ConnectAsync(options).Wait();

        // 消息发布/订阅实现
        return new CustomMqttTransport(client);
    });

    // x.AddConsumer<MqttMessageConsumer>();
    x.AddConsumer<MqttToRabbitBridgeConsumer>();
});

var app = builder.Build();
app.Run();

// 自定义MQTT传输实现
public class CustomMqttTransport : IMessageTransport,
    ITransportProvider,
    ISendTransport,
    IReceiveTransport
{
    private readonly IMqttClient _client;

    public CustomMqttTransport(IMqttClient client)
    {
        _client = client;
    }

    // 实现必要接口方法
    // ... existing code ...
}

public class MqttMessageConsumer : IConsumer<MqttMessage>
{
    private readonly IMqttClient _mqttClient;

    public Task Consume(ConsumeContext<MqttMessage> context)
    {
        // 处理MQTT消息
        // 转换逻辑
        var rabbitMsg = new RabbitMessage(
            context.Message.Topic,
            context.Message.Payload);
            
        return _publishEndpoint.Publish(rabbitMsg);
    }
}

// 桥接服务实现
public class RabbitToMqttBridgeConsumer : IConsumer<RabbitMessage>
{
    private readonly IMqttClient _mqttClient;
    
    public Task Consume(ConsumeContext<RabbitMessage> context)
    {
        // 转换逻辑
        var mqttMsg = new MqttApplicationMessageBuilder()
            .WithTopic(context.Message.RoutingKey)
            .WithPayload(context.Message.Body)
            .Build();
            
        return _mqttClient.PublishAsync(mqttMsg);
    }
}

public class MqttToRabbitBridgeConsumer : IConsumer<MqttMessage>
{
    private readonly IPublishEndpoint _publishEndpoint;
    
    public Task Consume(ConsumeContext<MqttMessage> context)
    {
        // 转换逻辑
        var rabbitMsg = new RabbitMessage(
            context.Message.Topic,
            context.Message.Payload);
            
        return _publishEndpoint.Publish(rabbitMsg);
    }
}

public record MqttMessage(string Topic, byte[] Payload);