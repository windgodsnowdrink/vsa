#:sdk Microsoft.NET.Sdk.Web
#:package Surging.Core.CPlatform@1.0.0
#:package Surging.Core.Consul@1.0.0
#:package Surging.Core.EventBusRabbitMQ@1.0.0
#:package Surging.Core.Kestrel@1.0.0
#:package Surging.Core.Protocol.Http@1.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Surging.Core.CPlatform;
using Surging.Core.Consul;
using Surging.Core.EventBusRabbitMQ;
using Surging.Core.Kestrel;
using Surging.Core.Protocol.Http;

// 服务接口定义
public interface IOrderService
{
    Task<Order> GetOrderAsync(int orderId);
    Task<bool> CreateOrderAsync(Order order);
}

// 服务实现
[ServiceBundle("api/order")]
public class OrderService : IOrderService
{
    private readonly IEventBus _eventBus;
    
    public OrderService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task<Order> GetOrderAsync(int orderId)
    {
        // 实现获取订单逻辑
        return await Task.FromResult(new Order { Id = orderId });
    }

    public async Task<bool> CreateOrderAsync(Order order)
    {
        // 发布订单创建事件
        await _eventBus.PublishAsync(new OrderCreatedEvent(order));
        return true;
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
            // 使用Consul作为服务注册中心
            builder.UseConsulManager(new ConfigInfo("Consul", "127.0.0.1", 8500));
            
            // 使用RabbitMQ作为事件总线
            builder.UseRabbitMQTransport(options =>
            {
                options.HostName = "localhost";
                options.UserName = "guest";
                options.Password = "guest";
            });
            
            // 配置Kestrel作为HTTP主机
            builder.UseServer(options =>
            {
                options.Ip = "127.0.0.1";
                options.Port = 80;
                options.Token = "True";
                options.ExecutionTimeoutInMilliseconds = 30000;
                options.MaxConcurrentRequests = 200;
            });
            
            // 添加API网关
            builder.UseHttpProtocol();
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

// DTO定义
public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public decimal Amount { get; set; }
}

// 领域事件
public class OrderCreatedEvent
{
    public OrderCreatedEvent(Order order)
    {
        Order = order;
        Timestamp = DateTime.UtcNow;
    }
    
    public Order Order { get; }
    public DateTime Timestamp { get; }
}