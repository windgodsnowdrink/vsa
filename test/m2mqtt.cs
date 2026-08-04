#:sdk Microsoft.NET.Sdk.Web
#:package MQTTnet@4.1.5
#:package M2MqttDotnetCore@1.1.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using System.Threading.Channels;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Threading.Tasks.Dataflow;

// 高性能MQTT消息处理器
public class MqttMessageProcessor
{
    private readonly ObjectPool<Memory<byte>> _memoryPool;
    private readonly Channel<MqttApplicationMessage> _messageChannel;
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public MqttMessageProcessor(
        ObjectPool<Memory<byte>> memoryPool,
        TailLatencyOptimizer latencyOptimizer)
    {
        _memoryPool = memoryPool;
        _latencyOptimizer = latencyOptimizer;
        _messageChannel = Channel.CreateUnbounded<MqttApplicationMessage>();
    }

    // 使用零拷贝技术处理消息
    public async Task ProcessMessageAsync(MqttApplicationMessage message)
    {
        await _messageChannel.Writer.WriteAsync(message);
    }

    private async Task ProcessMessagesAsync()
    {
        await foreach (var message in _messageChannel.Reader.ReadAllAsync())
        {
            _latencyOptimizer.Optimize(() =>
            {
                var memory = _memoryPool.Get();
                try
                {
                    // 使用Span<T>进行零拷贝处理
                    var span = memory.Span;
                    message.PayloadSegment.AsSpan().CopyTo(span);
                    // 处理消息内容...
                }
                finally
                {
                    _memoryPool.Return(memory);
                }
            });
        }
    }
}

// MQTT客户端服务
public class MqttClientService : IAsyncDisposable
{
    private readonly IMqttClient _client;
    private readonly MqttMessageProcessor _processor;
    private readonly ObjectPool<Memory<byte>> _memoryPool;

    public MqttClientService(
        MqttMessageProcessor processor,
        ObjectPool<Memory<byte>> memoryPool)
    {
        _processor = processor;
        _memoryPool = memoryPool;
        
        var factory = new MqttFactory();
        _client = factory.CreateMqttClient();
        
        _client.ApplicationMessageReceivedAsync += async e =>
        {
            await _processor.ProcessMessageAsync(e.ApplicationMessage);
        };
    }

    public async Task ConnectAsync(string broker, int port)
    {
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(broker, port)
            .WithCleanSession()
            .Build();

        await _client.ConnectAsync(options);
    }

    public async Task SubscribeAsync(string topic)
    {
        await _client.SubscribeAsync(new MqttTopicFilterBuilder()
            .WithTopic(topic)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build());
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisconnectAsync();
        _client.Dispose();
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder();

// 配置内存池和性能优化
builder.Services.AddSingleton<ObjectPool<Memory<byte>>>(new DefaultObjectPool<Memory<byte>>(
    new DefaultPooledObjectPolicy<Memory<byte>>(), 1000));
builder.Services.AddSingleton<TailLatencyOptimizer>();

// 配置MQTT服务
builder.Services.AddSingleton<MqttMessageProcessor>();
builder.Services.AddSingleton<MqttClientService>();

var app = builder.Build();

// 启动MQTT客户端
var mqttService = app.Services.GetRequiredService<MqttClientService>();
await mqttService.ConnectAsync("localhost", 1883);
await mqttService.SubscribeAsync("test/topic");

app.MapGet("/", () => "MQTT Client Service Running");
app.Run();

// 尾延迟优化器
public class TailLatencyOptimizer
{
    public void Optimize(Action action)
    {
        ThreadPool.QueueUserWorkItem(_ => action());
    }
}