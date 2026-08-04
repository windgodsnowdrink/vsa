#:sdk Microsoft.NET.Sdk.Web
#:package MQTTnet@4.1.5
#:package MessagePack@2.3.85
#:package System.Text.Json@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using MQTTnet;
using MessagePack;
using System.Text.Json;
using System.Threading.Channels;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.ObjectPool;
using System.Buffers;

/// <summary>
/// 物模型抽象基类，定义物模型的核心结构和基本操作
/// </summary>
public abstract class ThingModelBase
{
    /// <summary>
    /// 物模型Schema定义，包含版本、配置文件和功能定义
    /// </summary>
    public class ThingModelSchema
    {
        public string Schema { get; set; }
        public string Version { get; set; }
        public ThingModelProfile Profile { get; set; }
        public List<ThingModelProperty> Properties { get; set; }
        public List<ThingModelEvent> Events { get; set; }
        public List<ThingModelAction> Actions { get; set; }
        public List<ThingModelFunctionBlock> FunctionBlocks { get; set; }
    }

    /// <summary>
    /// 物模型配置文件，包含产品唯一标识
    /// </summary>
    public class ThingModelProfile
    {
        public string ProductKey { get; set; }
    }

    /// <summary>
    /// 物模型属性定义，包含标识符、名称、描述和数据类型
    /// </summary>
    public class ThingModelProperty
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public ThingModelDataType DataType { get; set; }
    }

    /// <summary>
    /// 物模型事件定义，支持多种事件类型和输出数据
    /// </summary>
    public class ThingModelEvent
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string EventType { get; set; }
        public string Method { get; set; }
        public List<ThingModelData> OutputData { get; set; }
    }

    /// <summary>
    /// 物模型动作定义，支持输入输出数据和方法调用
    /// </summary>
    public class ThingModelAction
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Method { get; set; }
        public List<ThingModelData> InputData { get; set; }
        public List<ThingModelData> OutputData { get; set; }
    }

    /// <summary>
    /// 功能块定义，支持模块化功能组合
    /// </summary>
    public class ThingModelFunctionBlock
    {
        public string FunctionBlockId { get; set; }
        public string FunctionBlockName { get; set; }
        public string FunctionBlockDesc { get; set; }
    }

    /// <summary>
    /// 通用数据定义，用于属性、事件和动作中的数据
    /// </summary>
    public class ThingModelData
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public ThingModelDataType DataType { get; set; }
    }

    /// <summary>
    /// 通用数据定义，用于属性、事件和动作中的数据
    /// </summary>
    /// <summary>
    /// 数据类型定义，支持多种类型和规格
    /// </summary>
    public class ThingModelDataType
    {
        public string Type { get; set; }
        public ThingModelDataSpecs Specs { get; set; }
    }

    /// <summary>
    /// 通用数据定义，用于属性、事件和动作中的数据
    /// </summary>
    /// <summary>
    /// 数据规格定义，支持范围、枚举等
    /// </summary>
    public class ThingModelDataSpecs
    {
        public List<ThingModelDataRange> Range { get; set; }
    }

    /// <summary>
    /// 通用数据定义，用于属性、事件和动作中的数据
    /// </summary>
    /// <summary>
    /// 数据范围定义，用于枚举类型
    /// </summary>
    public class ThingModelDataRange
    {
        public int Key { get; set; }
        public string Value { get; set; }
    }

    // 设备注册
    public abstract Task RegisterDevice(DeviceInfo device);

    // 设备连接
    public abstract Task ConnectDevice(string deviceId);

    // 设备关闭
    public abstract Task DisconnectDevice(string deviceId);

    // 心跳处理
    public abstract Task HandleHeartbeat(string deviceId);
}

// 具体物模型实现
/// <summary>
/// MQTT物模型实现类，包含高性能消息处理、零拷贝序列化和尾延迟优化
/// </summary>
[SkipLocalsInit]
public class MqttThingModel : ThingModelBase
{
    // 使用对象池管理MQTT客户端连接，减少创建开销
private readonly ObjectPool<IMqttClient> _mqttClientPool;
    private readonly ITargetBlock<DeviceMessage> _pipeline;
    // 线程本地缓冲区，用于零拷贝序列化，每个线程独立8192字节缓存
private readonly ThreadLocal<Memory<byte>> _threadLocalBuffer;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly Channel<DeviceMessage> _messageChannel;

    /// <summary>
    /// 构造函数，初始化MQTT客户端池、线程本地缓冲区和数据处理管道
    /// </summary>
    /// <param name="mqttClientPool">MQTT客户端对象池</param>
    public MqttThingModel(ObjectPool<IMqttClient> mqttClientPool)
    {
        _mqttClientPool = mqttClientPool;
        _threadLocalBuffer = new ThreadLocal<Memory<byte>>(() => new byte[4096]);

        // 构建Dataflow处理管道
        var options = new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 10000,
            MaxDegreeOfParallelism = Environment.ProcessorCount * 2
        };

        _pipeline = new ActionBlock<DeviceMessage>(async msg =>
        {
            using var client = _mqttClientPool.Get();
            var buffer = _threadLocalBuffer.Value;
            MessagePackSerializer.Serialize(msg, buffer.Span);
            await client.PublishAsync(new MqttApplicationMessage
            {
                Topic = $"thing/msg/{msg.DeviceId}",
                Payload = buffer.Slice(0, MessagePackSerializer.GetSerializedSize(msg))
            });
        }, options);

        // 尾延迟优化器
        _latencyOptimizer = new TailLatencyOptimizer(_pipeline);
        _messageChannel = Channel.CreateBounded<DeviceMessage>(10000);
    }

    /// <summary>
    /// 注册设备，发布设备信息到MQTT Broker
    /// </summary>
    /// <param name="device">设备信息</param>
    public override async Task RegisterDevice(DeviceInfo device)
    {
        using var client = _mqttClientPool.Get();
        var buffer = _threadLocalBuffer.Value;
        MessagePackSerializer.Serialize(device, buffer.Span);

        await client.PublishAsync(new MqttApplicationMessage
        {
            Topic = $"thing/{device.ProductId}/{device.DeviceId}/register",
            Payload = buffer.Slice(0, MessagePackSerializer.GetSerializedSize(device))
        });
    }

    public override async Task ConnectDevice(string deviceId)
    {
        var topic = $"thing/connect/{deviceId}";
        await _mqttClient.PublishAsync(new MqttApplicationMessage
        {
            Topic = topic,
            Payload = Array.Empty<byte>()
        });
    }

    public override async Task DisconnectDevice(string deviceId)
    {
        var topic = $"thing/disconnect/{deviceId}";
        await _mqttClient.PublishAsync(new MqttApplicationMessage
        {
            Topic = topic,
            Payload = Array.Empty<byte>()
        });
    }

    public override async Task HandleHeartbeat(string deviceId)
    {
        var topic = $"thing/heartbeat/{deviceId}";
        await _mqttClient.PublishAsync(new MqttApplicationMessage
        {
            Topic = topic,
            Payload = Array.Empty<byte>()
        });
    }

    // 事件发布
    /// <summary>
    /// 发布事件，将事件数据序列化后发送到指定主题
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <param name="event">事件定义</param>
    /// <param name="payload">事件数据</param>
    public async Task PublishEvent(string deviceId, ThingModelEvent @event, object payload)
    {
        var topic = $"thing/event/{deviceId}/{@event.Identifier}";
        var buffer = _threadLocalBuffer.Value;
        MessagePackSerializer.Serialize(payload, buffer.Span);

        await _mqttClient.PublishAsync(new MqttApplicationMessage
        {
            Topic = topic,
            Payload = buffer.Slice(0, MessagePackSerializer.GetSerializedSize(payload))
        });
    }

    // 动作调用
    /// <summary>
    /// 调用动作，采用请求-响应模式实现设备控制
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <param name="action">动作定义</param>
    /// <param name="input">输入参数</param>
    /// <returns>动作执行结果</returns>
    public async Task<object> InvokeAction(string deviceId, ThingModelAction action, object input)
    {
        var requestTopic = $"thing/action/{deviceId}/{action.Identifier}/request";
        var responseTopic = $"thing/action/{deviceId}/{action.Identifier}/response";

        // 发布请求
        var buffer = _threadLocalBuffer.Value;
        MessagePackSerializer.Serialize(input, buffer.Span);
        await _mqttClient.PublishAsync(new MqttApplicationMessage
        {
            Topic = requestTopic,
            Payload = buffer.Slice(0, MessagePackSerializer.GetSerializedSize(input))
        });

        // 订阅响应
        var tcs = new TaskCompletionSource<object>();
        var subscription = new MqttTopicFilterBuilder()
            .WithTopic(responseTopic)
            .Build();

        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            if (e.ApplicationMessage.Topic == responseTopic)
            {
                var result = MessagePackSerializer.Deserialize<object>(e.ApplicationMessage.Payload);
                tcs.SetResult(result);
            }
            return Task.CompletedTask;
        };

        await _mqttClient.SubscribeAsync(subscription);
        return await tcs.Task;
    }

    [MessagePackObject]
    public class DeviceMessage
    {
        [Key(0)]
        public string DeviceId { get; set; }

        [Key(1)]
        public ReadOnlyMemory<byte> Payload { get; set; }

        [Key(2)]
        public DateTimeOffset Timestamp { get; set; }
    }
}

// Resonance适配器集成
public class ThingModelResonanceAdapter : ResonanceMessageAdapter<DeviceMessage>
{
    protected override async ValueTask<object> AdaptInboundAsync(ReadOnlyMemory<byte> data)
    {
        // MessagePack反序列化内部通信
        return MessagePackSerializer.Deserialize<DeviceMessage>(data.Span);
    }

    protected override async ValueTask<ReadOnlyMemory<byte>> AdaptOutboundAsync(object message)
    {
        // JSON序列化外部输出
        return JsonSerializer.SerializeToUtf8Bytes(message);
    }
}

var builder = WebApplication.CreateBuilder();

// DI注册
builder.Services.AddSingleton<ObjectPool<IMqttClient>>(sp =>
{
    var factory = new MqttFactory();
    return new DefaultObjectPool<IMqttClient>(new MqttClientPooledPolicy(factory), Environment.ProcessorCount * 2);
});

builder.Services.AddSingleton<ThingModelBase, MqttThingModel>();
builder.Services.AddSingleton<ThingModelResonanceAdapter>();

// 注册AOT编译优化
builder.Services.AddHostedService<AotCompilerService>();

// 注册分层内存服务
builder.Services.AddSingleton<ITieredMemoryService, TieredMemoryService>();

var app = builder.Build();
app.MapGet("/", () => "MQTT Thing Model Ready");
app.Run();