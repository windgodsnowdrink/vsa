#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.SignalR.Client@8.0.0
#:package MediatR@12.1.1
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Threading.Channels;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;

public class SignalRNotificationHandler<TNotification> : INotificationHandler<TNotification> 
    where TNotification : INotification
{
    private readonly HubConnection _connection;

    public SignalRNotificationHandler(HubConnection connection)
    {
        _connection = connection;
    }

    public async Task Handle(TNotification notification, CancellationToken cancellationToken)
    {
        await _connection.SendAsync("ReceiveNotification", notification, cancellationToken);
    }
}

// 9. 更新DI扩展方法
public static class MediatRSignalRIntegrationExtensions
{
    public static IServiceCollection AddMediatRSignalRIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        // 基础SignalR配置
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
            options.MaximumReceiveMessageSize = 1024 * 1024; // 1MB
            options.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            
            // 添加消息压缩
            options.AddMessagePackProtocol(options =>
            {
                options.SerializerOptions = MessagePackSerializerOptions.Standard
                    .WithCompression(MessagePackCompression.Lz4BlockArray);
            });
        });

        // MediatR配置
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(MediatRSignalRIntegrationExtensions).Assembly);
        });

        // 核心服务
        services.AddSingleton<SignalRConnectionManager>();
        services.AddSingleton<SignalRMessageSerializer>();
        services.AddSingleton<IMemoryPool<byte>>(new MemoryPool<byte>(ArrayPool<byte>.Shared));
        
        // 处理器
        services.AddTransient(typeof(INotificationHandler<>), typeof(SignalRNotificationHandler<>));
        services.AddTransient<SignalRSecurityHandler>();
        
        // 后台服务
        services.AddHostedService<RealtimeQueryService>();
        services.AddHostedService<SignalRPerformanceMonitor>();

        // 配置SignalR客户端
        services.AddSingleton<HubConnection>(provider =>
        {
            var hubUrl = configuration["SignalR:HubUrl"] ?? "http://localhost:5000/notificationHub";
            return new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .AddMessagePackProtocol(options =>
                {
                    options.SerializerOptions = MessagePackSerializerOptions.Standard
                        .WithCompression(MessagePackCompression.Lz4BlockArray);
                })
                .WithAutomaticReconnect()
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();
        });

        // 添加性能监控
        services.AddSingleton<SignalRPerformanceMetrics>();
        
        // 添加负载均衡
        services.AddSingleton<SignalRLoadBalancer>();

        return services;
    }
}

public class RealtimeQueryService
{
    private readonly HubConnection _connection;
    private readonly Channel<object> _queryUpdatesChannel;

    public RealtimeQueryService(HubConnection connection)
    {
        _connection = connection;
        _queryUpdatesChannel = Channel.CreateUnbounded<object>();
        
        _connection.On<object>("QueryUpdated", update => 
        {
            _queryUpdatesChannel.Writer.TryWrite(update);
        });
    }

    public IAsyncEnumerable<object> GetQueryUpdatesAsync(CancellationToken cancellationToken)
    {
        return _queryUpdatesChannel.Reader.ReadAllAsync(cancellationToken);
    }
}

// 在Startup中配置
services.AddMediatRSignalRIntegration("https://your-hub-url/hub");

// 在前端监听
connection.on("ReceiveNotification", (notification) => {
    console.log("Received notification:", notification);
});