#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.SignalR.Client@8.0.0
#:package WolverineFx@2.7.0
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Threading.Channels;
using Wolverine;
using Microsoft.AspNetCore.SignalR.Client;

public static class WolverineSignalRIntegrationExtensions
{
    public static WolverineOptions UseSignalRTransport(this WolverineOptions options, IConfiguration configuration)
    {
        // 基础SignalR配置
        options.Services.AddSignalR(options =>
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

        // 核心服务
        options.Services.AddSingleton<SignalRConnectionManager>();
        options.Services.AddSingleton<SignalRMessageSerializer>();
        options.Services.AddSingleton<IMemoryPool<byte>>(new MemoryPool<byte>(ArrayPool<byte>.Shared));
        
        // 配置SignalR客户端
        options.Services.AddSingleton<HubConnection>(provider =>
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
        options.Services.AddSingleton<SignalRPerformanceMetrics>();
        
        // 添加负载均衡
        options.Services.AddSingleton<SignalRLoadBalancer>();

        // 使用Wolverine内置的MessagePack序列化
        options.UseMessagePackSerialization();
        
        // 配置Wolverine使用SignalR传输
        options.PublishAllMessages().ToSignalRHub();
        
        return options;
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

// 在Program.cs中配置
builder.Services.AddWolverine(options =>
{
    options.UseSignalRTransport(builder.Configuration);
});

// 在前端监听
connection.on("ReceiveNotification", (notification) => {
    console.log("Received notification:", notification);
});