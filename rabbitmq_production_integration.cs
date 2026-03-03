#:sdk Microsoft.NET.Sdk.Web

// RabbitMQ高级生产级集成方案
public static class AdvancedRabbitMQFeatures
{
    // 1. 消息确认机制
    public static void ConfigureMessageAcknowledgement(ConnectionFactory factory)
    {
        factory.AutomaticRecoveryEnabled = true;
        factory.TopologyRecoveryEnabled = true;
        factory.RequestedHeartbeat = TimeSpan.FromSeconds(60);
    }

    // 2. 死信队列实现
    public static void ConfigureDeadLetterExchange(IModel channel, string queueName)
    {
        var args = new Dictionary<string, object>
        {
            {"x-dead-letter-exchange", "dlx"},
            {"x-dead-letter-routing-key", queueName}
        };
        channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, args);
    }

    // 3. 延迟队列实现
    public static void ConfigureDelayedMessages(IModel channel)
    {
        var args = new Dictionary<string, object>
        {
            {"x-delayed-type", "direct"}
        };
        channel.ExchangeDeclare("delayed", "x-delayed-message", durable: true, autoDelete: false, args);
    }

    // 4. RPC模式实现
    public static void ConfigureRpcPattern(IConnection connection)
    {
        var channel = connection.CreateModel();
        var replyQueueName = channel.QueueDeclare().QueueName;
        var consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume(replyQueueName, true, consumer);
    }

    // 5. 集群管理实现
    public static void ConfigureCluster(ConnectionFactory factory, List<string> hosts)
    {
        factory.HostName = hosts.First();
        factory.AutomaticRecoveryEnabled = true;
        factory.TopologyRecoveryEnabled = true;
    }

    // 6. 镜像队列配置
    public static void ConfigureMirroredQueue(IModel channel, string queueName)
    {
        var args = new Dictionary<string, object>
        {
            {"x-ha-policy", "all"}
        };
        channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, args);
    }

    // 7. 消息追踪实现
    public static void ConfigureMessageTracing(IModel channel)
    {
        channel.ConfirmSelect();
        channel.BasicAcks += (sender, ea) => { /* 消息确认处理 */ };
        channel.BasicNacks += (sender, ea) => { /* 消息拒绝处理 */ };
    }

    // 8. 流量控制实现
    public static void ConfigureFlowControl(IModel channel, ushort prefetchCount)
    {
        channel.BasicQos(0, prefetchCount, false);
    }
}
#:package RabbitMQ.Client@6.4.0
#:property TargetFramework=net10.0
#:property Nullable=enable

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class RabbitMQOptions
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public int RetryCount { get; set; } = 5;
    public int PrefetchCount { get; set; } = 100;
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, Action<RabbitMQOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<IConnectionFactory>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
            return new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password,
                VirtualHost = options.VirtualHost,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
        });
        services.AddSingleton<IConnection>(sp => 
            sp.GetRequiredService<IConnectionFactory>().CreateConnection());
        return services;
    }
}

public class RabbitMQProducer
{
    private readonly IConnection _connection;
    private readonly RabbitMQOptions _options;

    public RabbitMQProducer(IConnection connection, IOptions<RabbitMQOptions> options)
    {
        _connection = connection;
        _options = options.Value;
    }

    public void Publish<T>(string exchange, string routingKey, T message)
    {
        using var channel = _connection.CreateModel();
        channel.ExchangeDeclare(exchange, ExchangeType.Direct, durable: true);
        var body = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(message));
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        channel.BasicPublish(exchange, routingKey, properties, body);
    }
}

public class RabbitMQConsumer
{
    private readonly IConnection _connection;
    private readonly RabbitMQOptions _options;

    public RabbitMQConsumer(IConnection connection, IOptions<RabbitMQOptions> options)
    {
        _connection = connection;
        _options = options.Value;
    }

    public void Consume<T>(string queue, string exchange, string routingKey, Func<T, Task> handler)
    {
        var channel = _connection.CreateModel();
        channel.ExchangeDeclare(exchange, ExchangeType.Direct, durable: true);
        channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(queue, exchange, routingKey);
        channel.BasicQos(0, _options.PrefetchCount, false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = System.Text.Json.JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(body));
                await handler(message);
                channel.BasicAck(ea.DeliveryTag, false);
            }
            catch
            {
                channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        channel.BasicConsume(queue, false, consumer);
    }
}

public static class Program
{
    public static async Task Main()
    {
        var services = new ServiceCollection();
        services.AddRabbitMQ(options =>
        {
            options.HostName = "localhost";
            options.PrefetchCount = 50;
        });
        services.AddSingleton<RabbitMQProducer>();
        services.AddSingleton<RabbitMQConsumer>();

        var provider = services.BuildServiceProvider();
        var producer = provider.GetRequiredService<RabbitMQProducer>();
        var consumer = provider.GetRequiredService<RabbitMQConsumer>();

        // 示例使用
        consumer.Consume<string>("test-queue", "test-exchange", "test-routing", async msg =>
        {
            Console.WriteLine($"Received: {msg}");
            await Task.CompletedTask;
        });

        producer.Publish("test-exchange", "test-routing", "Hello RabbitMQ!");
    }
}