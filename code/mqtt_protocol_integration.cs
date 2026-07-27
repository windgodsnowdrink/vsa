#:sdk Microsoft.NET.Sdk.Web
#:package MQTTnet@4.1.5
#:package MessagePack@2.4.59
#:package System.Text.Json@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using MQTTnet;
using MQTTnet.Client;
using MessagePack;
using System.Buffers;

[SkipLocalsInit]
public sealed class MqttProtocolAdapter : IDeviceModelProtocol
{
    private readonly IMqttClient _client;
    private readonly Channel<DeviceModel> _modelChannel;
    private readonly BoundedChannelOptions _channelOptions;
    private readonly IMemoryPool<byte> _memoryPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public string ProtocolName => "MQTT";

    public MqttProtocolAdapter(
        IMqttClient client, 
        BoundedChannelOptions? options = null)
    {
        _client = client;
        _channelOptions = options ?? new BoundedChannelOptions(10_000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        };
        
        _modelChannel = Channel.CreateBounded<DeviceModel>(_channelOptions);
        _memoryPool = MemoryPool<byte>.Shared;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        _client.ApplicationMessageReceivedAsync += OnMessageReceived;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async ValueTask<DeviceModel> ParseModelAsync(ReadOnlyMemory<byte> payload)
    {
        // 使用MemoryMarshal直接读取避免拷贝
        // if (MemoryMarshal.TryGetArray(payload, out var segment))
        // {
        //     return MessagePackSerializer.Deserialize<DeviceModel>(segment);
        // }

        using var buffer = _memoryPool.Rent(payload.Length);
        payload.CopyTo(buffer.Memory);
        
        // 使用MessagePack零拷贝解析
        var model = MessagePackSerializer.Deserialize<DeviceModel>(
            buffer.Memory, 
            MessagePackSerializerOptions.Standard);
            
        return model;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async ValueTask<byte[]> BuildRequestAsync(DeviceCommand command)
    {
        // 内部使用MessagePack序列化
        var msgpackData = MessagePackSerializer.Serialize(command);
        
        // 对外输出JSON格式
        return JsonSerializer.SerializeToUtf8Bytes(
            MessagePackSerializer.Deserialize<object>(msgpackData));
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        using var latencyToken = _latencyOptimizer.BeginOperation();
        using var buffer = _memoryPool.Rent(e.ApplicationMessage.Payload.Length);
        
        // 零拷贝处理MQTT消息
        e.ApplicationMessage.Payload.CopyTo(buffer.Memory);
        var model = await ParseModelAsync(buffer.Memory);
        
        // 背压控制
        await _modelChannel.Writer.WriteAsync(model);
    }

    // 背压感知的消息发布
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task PublishWithBackpressureAsync(
        string topic, 
        DeviceCommand command, 
        CancellationToken ct)
    {
        var payload = await BuildRequestAsync(command);
        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .Build();
            
        // 基于通道容量的背压控制
        while (!ct.IsCancellationRequested)
        {
            if (_modelChannel.Reader.Count < _channelOptions.Capacity * 0.8)
            {
                await _client.PublishAsync(message, ct);
                break;
            }
            await Task.Delay(100, ct);
        }
    }

    // 瞬时流量控制
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ProcessMessagesWithRateLimitAsync(
        Func<DeviceModel, ValueTask> processor,
        int maxConcurrent,
        CancellationToken ct)
    {
        var semaphore = new SemaphoreSlim(maxConcurrent);
        await foreach (var model in _modelChannel.Reader.ReadAllAsync(ct))
        {
            await semaphore.WaitAsync(ct);
            _ = Task.Run(async () =>
            {
                try { await processor(model); }
                finally { semaphore.Release(); }
            }, ct);
        }
    }
}

// 集成到主服务
var builder = WebApplication.CreateBuilder(args);

// MQTT客户端配置
var mqttFactory = new MqttFactory();
var mqttClient = mqttFactory.CreateMqttClient();
var mqttOptions = new MqttClientOptionsBuilder()
    .WithTcpServer("localhost")
    .Build();

await mqttClient.ConnectAsync(mqttOptions);

// 注册MQTT协议适配器
builder.Services.AddSingleton<IDeviceModelProtocol>(sp => 
    new MqttProtocolAdapter(mqttClient));

var app = builder.Build();
app.MapGet("/", () => "MQTT Protocol Adapter Service");
app.Run();