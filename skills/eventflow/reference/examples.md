# EventFlow - 使用示例

## 快速入门

### 1. 基本用法示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EventFlow.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("EventFlow 基本用法示例");
        Console.WriteLine("=" * 50);
        
        // 创建主机构建器
        var builder = Host.CreateApplicationBuilder();
        
        // 配置 EventFlow
        builder.Services.Configure<EventFlowOptions>(options => {
            options.EventStoreType = "InMemory";
            options.EventBusType = "InMemory";
            options.EnableDetailedLogging = true;
        });
        
        // 注册服务
        builder.Services.AddSingleton<IEventFlowService, EventFlowService>();
        
        // 构建主机
        var host = builder.Build();
        var serviceProvider = host.Services;
        
        // 获取 EventFlow 服务
        var eventFlowService = serviceProvider.GetRequiredService<IEventFlowService>();
        
        // 使用 EventFlow 功能 - 创建事件
        var createResult = await eventFlowService.CreateEventAsync(
            "UserCreated", 
            "{\"userId\":\"123\",\"name\":\"测试用户\",\"email\":\"test@example.com\"}",
            "ExampleApp"
        );
        
        Console.WriteLine($"创建事件结果: {createResult.Success}");
        if (createResult.Success && !string.IsNullOrEmpty(createResult.EventId))
        {
            Console.WriteLine($"事件ID: {createResult.EventId}");
            
            // 获取刚创建的事件
            var getResult = await eventFlowService.GetEventAsync(createResult.EventId);
            Console.WriteLine($"获取事件结果: {getResult.Success}");
            if (getResult.Success && getResult.Events != null && getResult.Events.Count > 0)
            {
                var evt = getResult.Events[0];
                Console.WriteLine($"事件类型: {evt.EventType}");
                Console.WriteLine($"事件时间: {evt.EventTime:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"事件数据: {evt.EventDataContent}");
            }
        }
    }
}
```

### 2. AOT 单文件执行示例

EventFlow 提供了 .NET 10 AOT 编译的单文件执行脚本，可以直接运行，无需安装 .NET 运行时。

#### 编译命令

```bash
# 编译为 Windows AOT 单文件
dotnet publish -c Release -r win-x64 -p:PublishAot=true -p:PublishSingleFile=true --self-contained true

# 编译为 Linux AOT 单文件
dotnet publish -c Release -r linux-x64 -p:PublishAot=true -p:PublishSingleFile=true --self-contained true

# 编译为 macOS AOT 单文件
dotnet publish -c Release -r osx-x64 -p:PublishAot=true -p:PublishSingleFile=true --self-contained true
```

#### 运行示例

```bash
# 显示版本信息
./eventflow_aot version

# 创建事件
./eventflow_aot create UserCreated '{"userId":"123","name":"测试用户"}'

# 获取事件（使用实际事件ID）
./eventflow_aot get 550e8400-e29b-41d4-a716-446655440000

# 列出事件
./eventflow_aot list UserCreated 5

# 列出所有事件，限制10个
./eventflow_aot list

# 发布事件
./eventflow_aot publish OrderPlaced '{"orderId":"456","amount":100,"customerId":"123"}'

# 订阅事件
./eventflow_aot subscribe UserCreated Subscriber1
```

### 3. 事件发布与订阅示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EventFlow.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("EventFlow 事件发布与订阅示例");
        Console.WriteLine("=" * 50);
        
        // 创建主机构建器
        var builder = Host.CreateApplicationBuilder();
        
        // 配置和注册服务
        builder.Services.Configure<EventFlowOptions>(options => {
            options.EventStoreType = "InMemory";
            options.EventBusType = "InMemory";
        });
        builder.Services.AddSingleton<IEventFlowService, EventFlowService>();
        
        var host = builder.Build();
        var eventFlowService = host.Services.GetRequiredService<IEventFlowService>();
        
        // 订阅事件
        var subscribeResult = await eventFlowService.SubscribeEventAsync("OrderPlaced", "OrderProcessor");
        Console.WriteLine($"订阅事件结果: {subscribeResult.Success}");
        
        // 发布多个事件
        for (int i = 1; i <= 3; i++)
        {
            var orderId = $"order-{i:D3}