# Gateway - 使用示例

## 快速开始

### 1. 基本使用示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Gateway.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Gateway 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 创建主机
        var builder = Host.CreateApplicationBuilder();
        
        // 配置服务
        builder.Services.Configure<GatewayOptions>(builder.Configuration.GetSection("Gateway"));
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<IGatewayService, GatewayService>();
        builder.Services.AddSingleton<GatewayAotEngine>();
        
        // 构建主机
        using var host = builder.Build();
        
        // 获取 Gateway 服务
        var gatewayService = host.Services.GetRequiredService<IGatewayService>();
        
        // 启动网关服务
        Console.WriteLine("1. 启动网关服务...");
        var startResult = await gatewayService.StartAsync();
        Console.WriteLine($"   启动结果: {(startResult.Success ? "成功" : "失败")}");
        if (!startResult.Success)
        {
            Console.WriteLine($"   错误信息: {startResult.ErrorMessage}");
            return;
        }
        
        // 添加路由
        Console.WriteLine("2. 添加路由...");
        var addRouteResult = await gatewayService.AddRouteAsync("api_route", "/api/{**remainder}", "api_cluster", "http://localhost:5000");
        Console.WriteLine($"   添加路由结果: {(addRouteResult.Success ? "成功" : "失败")}");
        if (!addRouteResult.Success)
        {
            Console.WriteLine($"   错误信息: {addRouteResult.ErrorMessage}");
            return;
        }
        
        // 列出路由
        Console.WriteLine("3. 列出路由...");
        var listRoutesResult = await gatewayService.ListRoutesAsync();
        Console.WriteLine($"   列出路由结果: {(listRoutesResult.Success ? "成功" : "失败")}");
        if (listRoutesResult.Success && listRoutesResult.Routes != null)
        {
            foreach (var route in listRoutesResult.Routes)
            {
                Console.WriteLine($"   - {route.RouteId}: {route.MatchPath} -> {route.DestinationAddress}");
            }
        }
        
        // 获取状态
        Console.WriteLine("4. 获取网关状态...");
        var statusResult = await gatewayService.GetStatusAsync();
        Console.WriteLine($"   获取状态结果: {(statusResult.Success ? "成功" : "失败")}");
        if (statusResult.Success)
        {
            foreach (var item in statusResult.Results)
            {
                Console.WriteLine($"   - {item}");
            }
        }
        
        // 获取版本信息
        Console.WriteLine("5. 获取版本信息...");
        var versionResult = await gatewayService.GetVersionInfoAsync();
        Console.WriteLine($"   获取版本信息结果: {(versionResult.Success ? "成功" : "失败")}");
        if (versionResult.Success)
        {
            foreach (var item in versionResult.Results)
            {
                Console.WriteLine($"   - {item}");
            }
        }
        
        // 停止网关服务
        Console.WriteLine("6. 停止网关服务...");
        var stopResult = await gatewayService.StopAsync();
        Console.WriteLine($"   停止结果: {(stopResult.Success ? "成功" : "失败")}");
        
        Console.WriteLine("\n示例完成！");
    }
}
```

### 2. 命令行使用示例

Gateway AOT 引擎支持通过命令行执行各种操作，以下是命令行使用示例：

#### 启动网关服务
```bash
# 使用命令行启动网关服务
gateway_aot start

# 输出示例
命令执行结果: 成功
执行时间: 1050 ms
- 成功启动网关服务
- 服务器地址: http://localhost:8080
- 启用压缩: True
- 启用电路 breaker: True
- 启动时间: 2024-01-19 12:00:00
```

#### 停止网关服务
```bash
# 使用命令行停止网关服务
gateway_aot stop

# 输出示例
命令执行结果: 成功
执行时间: 550 ms
- 成功停止网关服务
- 停止时间: 2024-01-19 12:00:05
```

#### 重启网关服务
```bash
# 使用命令行重启网关服务
gateway_aot restart

# 输出示例
命令执行结果: 成功
执行时间: 1600 ms
- 成功重启网关服务
- 成功启动网关服务
- 服务器地址: http://localhost:8080
- 启用压缩: True
- 启用电路 breaker: True
- 启动时间: 2024-01-19 12:00:10
```

#### 获取网关状态
```bash
# 使用命令行获取网关状态
gateway_aot status

# 输出示例
命令执行结果: 成功
执行时间: 100 ms
- 网关服务状态: 运行中
- 服务器地址: http://localhost:8080
- 启用压缩: True
- 启用电路 breaker: True
- 路由数量: 2
```

#### 添加路由
```bash
# 使用命令行添加路由
gateway_aot add api_route /api/{**remainder} api_cluster http://localhost:5000

# 输出示例
命令执行结果: 成功
执行时间: 150 ms
- 成功添加路由: api_route
- 匹配路径: /api/{**remainder}
- 集群 ID: api_cluster
- 目标地址: http://localhost:5000
- 添加时间: 2024-01-19 12:00:15
```

#### 删除路由
```bash
# 使用命令行删除路由
gateway_aot remove api_route

# 输出示例
命令执行结果: 成功
执行时间: 120 ms
- 成功删除路由: api_route
- 匹配路径: /api/{**remainder}
- 集群 ID: api_cluster
- 目标地址: http://localhost:5000
- 删除时间: 2024-01-19 12:00:20
```

#### 列出路由
```bash
# 使用命令行列出路由
gateway_aot list

# 输出示例
命令执行结果: 成功
执行时间: 100 ms
- 成功获取路由列表
- 路由数量: 2
- api_route: /api/{**remainder} -> http://localhost:5000
- web_route: /web/{**remainder} -> http://localhost:5001

路由详情:
  api_route:
    匹配路径: /api/{**remainder}
    集群 ID: api_cluster
    目标地址: http://localhost:5000
  web_route:
    匹配路径: /web/{**remainder}
    集群 ID: web_cluster
    目标地址: http://localhost:5001
```

#### 显示版本信息
```bash
# 使用命令行显示版本信息
gateway_aot version

# 输出示例
命令执行结果: 成功
执行时间: 100 ms
- Gateway AOT Engine
- 版本: 1.0.0
- .NET 版本: 10.0.0
- 操作系统: Microsoft Windows 10.0.19045
- 架构: X64
- AOT 编译: True
- 服务器地址: http://localhost:8080
- 启用 SSL: False
- 工作目录: d:\Trae\vsa\skills\gateway
```

### 3. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Gateway.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Gateway 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 创建配置
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Gateway:ServerAddress", "localhost" },
                { "Gateway:ServerPort", "8080" },
                { "Gateway:EnableSsl", "false" },
                { "Gateway:EnableCompression", "true" },
                { "Gateway:EnableCircuitBreaker", "true" },
                { "Gateway:CircuitBreakerFailureThreshold", "5" },
                { "Gateway:CircuitBreakerResetTimeMs", "30000" },
                { "Gateway:EnableDetailedLogging", "true" },
                { "Gateway:EnablePerformanceMonitoring", "true" },
                { "Gateway:RequestTimeoutMs", "30000" }
            })
            .Build();
        
        // 创建主机
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddConfiguration(configuration);
        
        // 配置服务
        builder.Services.Configure<GatewayOptions>(builder.Configuration.GetSection("Gateway"));
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<IGatewayService, GatewayService>();
        builder.Services.AddSingleton<GatewayAotEngine>();
        
        // 构建主机
        using var host = builder.Build();
        
        // 获取配置
        var options = host.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<GatewayOptions>>().Value;
        Console.WriteLine("配置信息:");
        Console.WriteLine($"- 服务器地址: {options.ServerAddress}:{options.ServerPort}");
        Console.WriteLine($"- 启用 SSL: {options.EnableSsl}");
        Console.WriteLine($"- 启用压缩: {options.EnableCompression}");
        Console.WriteLine($"- 启用电路 breaker: {options.EnableCircuitBreaker}");
        Console.WriteLine($"- 电路 breaker 失败阈值: {options.CircuitBreakerFailureThreshold}");
        Console.WriteLine($"- 电路 breaker 重置时间: {options.CircuitBreakerResetTimeMs} ms");
        Console.WriteLine($"- 请求超时: {options.RequestTimeoutMs} ms");
        Console.WriteLine($"- 启用详细日志: {options.EnableDetailedLogging}");
        Console.WriteLine($"- 启用性能监控: {options.EnablePerformanceMonitoring}");
        
        // 获取 Gateway 服务
        var gatewayService = host.Services.GetRequiredService<IGatewayService>();
        
        // 使用服务
        Console.WriteLine("\n使用 Gateway 服务...");
        var versionResult = await gatewayService.GetVersionInfoAsync();
        if (versionResult.Success)
        {
            Console.WriteLine("版本信息获取成功:");
            foreach (var item in versionResult.Results)
            {
                Console.WriteLine($"- {item}");
            }
        }
    }
}
```

### 4. 性能优化示例

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Gateway.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Gateway 性能优化示例");
        Console.WriteLine("=" * 50);
        
        // 创建主机
        var builder = Host.CreateApplicationBuilder();
        
        // 配置服务
        builder.Services.Configure<GatewayOptions>(options => {
            options.EnableCompression = true;
            options.EnableCircuitBreaker = true;
            options.EnablePerformanceMonitoring = true;
        });
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<IGatewayService, GatewayService>();
        builder.Services.AddSingleton<GatewayAotEngine>();
        
        // 构建主机
        using var host = builder.Build();
        
        // 获取 Gateway 服务
        var gatewayService = host.Services.GetRequiredService<IGatewayService>();
        
        // 启动网关
        await gatewayService.StartAsync();
        
        // 性能测试
        const int iterations = 100;
        var totalTime = 0L;
        
        Console.WriteLine($"执行 {iterations} 次路由操作测试...");
        
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            var routeId = $"test_route_{i}";
            var matchPath = $"/test/{i}/{{**remainder}}";
            var clusterId = $"test_cluster_{i}";
            var destinationAddress = "http://localhost:5000";
            
            // 添加路由
            var addResult = await gatewayService.AddRouteAsync(routeId, matchPath, clusterId, destinationAddress);
            if (!addResult.Success)
            {
                Console.WriteLine($"测试失败: {addResult.ErrorMessage}");
                break;
            }
            totalTime += addResult.ExecutionTimeMs;
            
            // 删除路由
            var removeResult = await gatewayService.RemoveRouteAsync(routeId);
            if (!removeResult.Success)
            {
                Console.WriteLine($"测试失败: {removeResult.ErrorMessage}");
                break;
            }
            totalTime += removeResult.ExecutionTimeMs;
        }
        
        stopwatch.Stop();
        
        Console.WriteLine($"测试完成！");
        Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每次操作时间: {totalTime / (double)(iterations * 2):F3} ms");
        Console.WriteLine($"AOT 编译提升性能: 启动速度快，内存占用低");
        
        // 停止网关
        await gatewayService.StopAsync();
    }
}
```

### 5. 错误处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Gateway.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Gateway 错误处理示例");
        Console.WriteLine("=" * 50);
        
        // 创建主机
        var builder = Host.CreateApplicationBuilder();
        
        // 配置服务
        builder.Services.Configure<GatewayOptions>(builder.Configuration.GetSection("Gateway"));
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<IGatewayService, GatewayService>();
        builder.Services.AddSingleton<GatewayAotEngine>();
        
        // 构建主机
        using var host = builder.Build();
        
        // 获取 Gateway 服务
        var gatewayService = host.Services.GetRequiredService<IGatewayService>();
        
        // 测试错误处理
        Console.WriteLine("1. 测试重复添加路由...");
        var addResult1 = await gatewayService.AddRouteAsync("test_route", "/test/{**remainder}", "test_cluster", "http://localhost:5000");
        Console.WriteLine($"   第一次添加结果: {(addResult1.Success ? "成功" : "失败")}");
        
        var addResult2 = await gatewayService.AddRouteAsync("test_route", "/test/{**remainder}", "test_cluster", "http://localhost:5000");
        Console.WriteLine($"   第二次添加结果: {(addResult2.Success ? "成功" : "失败")}");
        Console.WriteLine($"   错误信息: {addResult2.ErrorMessage}");
        
        Console.WriteLine("2. 测试删除不存在的路由...");
        var removeResult = await gatewayService.RemoveRouteAsync("nonexistent_route");
        Console.WriteLine($"   删除结果: {(removeResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"   错误信息: {removeResult.ErrorMessage}");
        
        Console.WriteLine("3. 测试重复停止网关...");
        // 先停止网关
        var stopResult1 = await gatewayService.StopAsync();
        Console.WriteLine($"   第一次停止结果: {(stopResult1.Success ? "成功" : "失败")}");
        
        var stopResult2 = await gatewayService.StopAsync();
        Console.WriteLine($"   第二次停止结果: {(stopResult2.Success ? "成功" : "失败")}");
        Console.WriteLine($"   错误信息: {stopResult2.ErrorMessage}");
        
        Console.WriteLine("4. 测试在未启动的情况下获取状态...");
        var statusResult = await gatewayService.GetStatusAsync();
        Console.WriteLine($"   获取状态结果: {(statusResult.Success ? "成功" : "失败")}");
        if (statusResult.Success)
        {
            foreach (var item in statusResult.Results)
            {
                Console.WriteLine($"   - {item}");
            }
        }
        
        Console.WriteLine("\n错误处理测试完成！");
    }
}
```

## 总结

以上示例展示了 Gateway AOT 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本操作
2. 配置高级选项
3. 优化性能
4. 处理错误情况
5. 使用命令行工具执行操作

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。通过 AOT 编译，Gateway 引擎获得了更快的启动速度和更低的内存占用，同时保持了完整的功能和良好的用户体验。
