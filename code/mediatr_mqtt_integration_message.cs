#:sdk Microsoft.NET.Sdk
#:package MQTTnet@4.1.5
#:package MediatR@12.1.1
#:package MediatR.Extensions.Microsoft.DependencyInjection@12.1.1
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Text.Json;
using MediatR;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

// 1. 定义跨进程MediatR消息包装器
public record ProcessMessage<T>(T Message) : INotification 
    where T : INotification;

// 2. 实现MQTT消息发布行为
public class MqttPublishBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttOptions _options;

    public MqttPublishBehavior(IMqttClient mqttClient, IOptions<MqttOptions> options)
    {
        _mqttClient = mqttClient;
        _options = options.Value;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // 先执行本地处理
        var response = await next();

        // 如果是需要跨进程的消息，则通过MQTT发布
        if (request is INotification)
        {
            var message = new ProcessMessage<TRequest>(request);
            var payload = JsonSerializer.Serialize(message);
            
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(_options.Topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag()
                .Build();

            await _mqttClient.PublishAsync(mqttMessage, cancellationToken);
        }

        return response;
    }
}

// 3. 实现MQTT消息订阅处理
public class MqttMessageHandler : IMqttMessageHandler
{
    private readonly IMediator _mediator;

    public MqttMessageHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task HandleMessageAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
    {
        var payload = eventArgs.ApplicationMessage.Payload;
        var message = JsonSerializer.Deserialize<ProcessMessage<INotification>>(payload);
        
        if (message != null)
        {
            await _mediator.Publish(message.Message);
        }
    }
}

// 4. 分布式事务集成
public class DistributedTransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IMqttClient _mqttClient;
    private readonly ITransactionCoordinator _coordinator;

    public DistributedTransactionBehavior(
        IMqttClient mqttClient, 
        ITransactionCoordinator coordinator)
    {
        _mqttClient = mqttClient;
        _coordinator = coordinator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // 开始分布式事务
        var transaction = await _coordinator.BeginTransactionAsync();
        
        try
        {
            // 执行本地事务
            var response = await next();
            
            // 提交事务
            await transaction.CommitAsync();
            
            return response;
        }
        catch (Exception)
        {
            // 回滚事务
            await transaction.RollbackAsync();
            throw;
        }
    }
}

// 5. DI扩展方法
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatRMqttIntegration(
        this IServiceCollection services,
        Action<MqttOptions> configureOptions)
    {
        // 配置MQTT
        services.Configure(configureOptions);
        
        // 注册MQTT客户端
        services.AddSingleton<IMqttClient>(provider => 
        {
            var options = provider.GetRequiredService<IOptions<MqttOptions>>().Value;
            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();
            
            var clientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(options.Server)
                .WithClientId(options.ClientId)
                .Build();
                
            client.ConnectAsync(clientOptions).Wait();
            return client;
        });
        
        // 注册消息处理器
        services.AddSingleton<IMqttMessageHandler, MqttMessageHandler>();
        
        // 注册MediatR
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssemblyContaining<ProcessMessage<INotification>>();
            
            // 添加MQTT发布行为
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(MqttPublishBehavior<,>));
            
            // 添加分布式事务行为
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(DistributedTransactionBehavior<,>));
        });
        
        return services;
    }
}

// 6. 配置选项
public class MqttOptions
{
    public string Server { get; set; } = "localhost";
    public int Port { get; set; } = 1883;
    public string ClientId { get; set; } = Guid.NewGuid().ToString();
    public string Topic { get; set; } = "mediatr/messages";
}