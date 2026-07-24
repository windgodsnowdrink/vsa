#:sdk Microsoft.NET.Sdk.Web
#:package Resonance@6.0.0
#:package RabbitMQ.Client@6.6.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using Resonance;
using RabbitMQ.Client;

public class DistributedMessageBus : IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ObjectPool<ResonanceMessage> _messagePool;

    public DistributedMessageBus(string connectionString)
    {
        var factory = new ConnectionFactory { Uri = new Uri(connectionString) };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _messagePool = new DefaultObjectPool<ResonanceMessage>(
            new MessagePooledObjectPolicy(),
            Environment.ProcessorCount * 4);
    }

    public async ValueTask PublishAsync<T>(T message) where T : IResonanceMessage
    {
        var msg = _messagePool.Get();
        try
        {
            msg.Payload = message;
            var body = ResonanceSerializer.Serialize(msg);
            
            _channel.BasicPublish(
                exchange: "resonance.bus",
                routingKey: typeof(T).Name,
                basicProperties: null,
                body: body);
        }
        finally
        {
            _messagePool.Return(msg);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}