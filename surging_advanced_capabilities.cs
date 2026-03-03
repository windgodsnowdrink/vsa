#:sdk Microsoft.NET.Sdk.Web
#:package Surging.Core.CPlatform@1.0.0
#:package Surging.Core.Consul@1.0.0
#:package Surging.Core.DotNetty@1.0.0
#:package Surging.Core.Zookeeper@1.0.0
#:package Surging.Core.Swagger@1.0.0
#:package Surging.Core.EventBusKafka@1.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Surging.Core.CPlatform;
using Surging.Core.Consul;
using Surging.Core.DotNetty;
using Surging.Core.Zookeeper;
using Surging.Core.Swagger;
using Surging.Core.EventBusKafka;

// 服务治理配置
public static class ServiceGovernanceExtensions
{
    public static IServiceCollection AddServiceGovernance(this IServiceCollection services)
    {
        // 负载均衡策略
        services.AddSingleton<ILoadBalance, RoundRobinLoadBalance>();
        
        // 熔断降级配置
        services.AddSingleton<ICircuitBreakerPolicy>(provider => 
            new CircuitBreakerPolicy(
                exceptionsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30)
            ));
            
        // 服务限流
        services.AddSingleton<IRateLimitPolicy>(new RateLimitPolicy(100, TimeSpan.FromMinutes(1)));
        
        return services;
    }
}

// 分布式追踪集成
public static class DistributedTracingExtensions
{
    public static IServiceCollection AddDistributedTracing(this IServiceCollection services)
    {
        // 使用Zipkin作为分布式追踪系统
        services.AddZipkinTracing(options =>
        {
            options.ZipkinEndpoint = "http://localhost:9411/api/v2/spans";
            options.ServiceName = "order-service";
            options.Rate = 1.0f;
        });
        
        return services;
    }
}

// 多协议支持配置
public static class MultiProtocolExtensions
{
    public static IServiceCollection AddMultiProtocolSupport(this IServiceCollection services)
    {
        // 添加HTTP协议支持
        services.AddHttpProtocol();
        
        // 添加gRPC协议支持
        services.AddGrpcProtocol();
        
        // 添加WebSocket协议支持
        services.AddWebSocketProtocol();
        
        return services;
    }
}

// 启动配置
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // 添加Surging核心服务
        services.AddSurging(builder =>
        {
            // 使用Zookeeper作为服务注册中心
            builder.UseZookeeperManager(new ConfigInfo("Zookeeper", "127.0.0.1", 2181));
            
            // 使用Kafka作为事件总线
            builder.UseKafkaTransport(options =>
            {
                options.BootstrapServers = "localhost:9092";
                options.GroupId = "order-service-group";
            });
            
            // 添加服务治理
            builder.AddServiceGovernance();
            
            // 添加分布式追踪
            builder.AddDistributedTracing();
            
            // 添加多协议支持
            builder.AddMultiProtocolSupport();
            
            // 配置DotNetty作为RPC主机
            builder.UseDotNettyTransport(options =>
            {
                options.Ip = "127.0.0.1";
                options.Port = 88;
                options.Token = "True";
            });
            
            // 添加Swagger文档
            builder.UseSwagger();
        });
    }
}

// 主程序
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

// 分布式追踪配置
builder.AddOpenTelemetry(opt => {
    opt.AddZipkinExporter();
    opt.AddJaegerExporter();
    opt.AddOtlpExporter();
});

// 2. 多协议支持
builder.AddProtocols(opt => {
    opt.AddHttpProtocol();
    opt.AddGrpcProtocol();
    opt.AddWebSocketProtocol();
    opt.AddMqttProtocol();
});

// 3. 多注册中心
builder.AddRegistryCenter(opt => {
    opt.AddConsul();
    opt.AddZookeeper();
    opt.AddNacos();
});

// 4. 多序列化支持
builder.AddSerializers(opt => {
    opt.AddMessagePackSerializer();
    opt.AddProtobufSerializer();
    opt.AddJsonSerializer();
});

// 5. Sidecar模式
builder.AddSidecar(opt => {
    opt.UseServiceMesh();
    opt.AddApolloConfig();
    opt.AddNacosConfig();
});