# Dapr AOT - 使用示例

## 快速开始

### 1. 状态管理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dapr.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Dapr AOT 状态管理示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机
        var builder = Host.CreateApplicationBuilder();
        
        // 配置Dapr选项
        builder.Configuration.AddJsonFile("dapr_aot.setting.json", optional: true);
        builder.Services.Configure<DaprOptions>(builder.Configuration.GetSection("Dapr"));
        
        // 注册服务
        var daprOptions = builder.Configuration.GetSection("Dapr").Get<DaprOptions>() ?? new DaprOptions();
        builder.Services.AddDaprClient(daprOptions);
        builder.Services.AddSingleton<IDaprService, DaprService>();
        builder.Services.AddSingleton<DaprAotEngine>();
        
        // 构建主机并获取服务提供者
        var host = builder.Build();
        var serviceProvider = host.Services;
        
        // 获取Dapr AOT引擎
        var engine = serviceProvider.GetRequiredService<DaprAotEngine>();
        
        // 准备测试数据
        var testData = new {
            Id = "test-key-1",
            Value = "Hello Dapr AOT!",
            Timestamp = DateTime.Now,
            Counter = 42
        };
        
        // 保存状态
        Console.WriteLine("\n1. 保存状态...");
        var saveResult = await engine.SaveStateAsync("test-state", testData);
        Console.WriteLine($"   保存结果: {saveResult.Success ? "成功" : "失败"}");
        if (!saveResult.Success)
        {
            Console.WriteLine($"   错误信息: {saveResult.ErrorMessage}");
            return;
        }
        
        // 获取状态
        Console.WriteLine("\n2. 获取状态...");
        var getResult = await engine.GetStateAsync<object>("test-state");
        Console.WriteLine($"   获取结果: {getResult.Success ? "成功" : "失败"}");
        if (getResult.Success && getResult.ResultData != null)
        {
            Console.WriteLine($"   状态值: {Newtonsoft.Json.JsonConvert.SerializeObject(getResult.ResultData)}");
        }
        
        // 删除状态
        Console.WriteLine("\n3. 删除状态...");
        var deleteResult = await engine.DeleteStateAsync("test-state");
        Console.WriteLine($"   删除结果: {deleteResult.Success ? "成功" : "失败"}");
        
        // 验证删除
        Console.WriteLine("\n4. 验证删除...");
        var verifyResult = await engine.GetStateAsync<object>("test-state");
        Console.WriteLine($"   验证结果: {verifyResult.Success && verifyResult.ResultData == null ? "成功删除" : "删除失败或状态仍存在"}");
        
        Console.WriteLine("\n" + "=" * 50);
        Console.WriteLine("状态管理示例完成！");
    }
}
```

### 2. 发布订阅示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dapr.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Dapr AOT 发布订阅示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机
        var builder = Host.CreateApplicationBuilder();
        
        // 配置Dapr选项
        builder.Configuration.AddJsonFile("dapr_aot.setting.json", optional: true);
        builder.Services.Configure<DaprOptions>(builder.Configuration.GetSection("Dapr"));
        
        // 注册服务
        var daprOptions = builder.Configuration.GetSection("Dapr").Get<DaprOptions>() ?? new DaprOptions();
        builder.Services.AddDaprClient(daprOptions);
        builder.Services.AddSingleton<IDaprService, DaprService>();
        builder.Services.AddSingleton<DaprAotEngine>();
        
        // 构建主机并获取服务提供者
        var host = builder.Build();
        var serviceProvider = host.Services;
        
        // 获取Dapr AOT引擎
        var engine = serviceProvider.GetRequiredService<DaprAotEngine>();
        
        // 准备测试数据
        var testMessage = new {
            MessageId = Guid.NewGuid().ToString(),
            Content = "Dapr AOT 发布订阅测试消息",
            Sender = "DaprAotExample",
            Timestamp = DateTime.Now,
            Priority = "High"
        };
        
        // 发布消息到主题
        Console.WriteLine($"\n1. 发布消息到主题 'test-topic'...");
        var publishResult = await engine.PublishMessageAsync("test-topic", testMessage);
        Console.WriteLine($"   发布结果: {publishResult.Success ? "成功" : "失败"}");
        if (publishResult.Success)
        {
            Console.WriteLine($"   消息ID: {testMessage.MessageId}");
            Console.WriteLine($"   执行时间: {publishResult.ExecutionTimeMs} ms");
        }
        else
        {
            Console.WriteLine($"   错误信息: {publishResult.ErrorMessage}");
        }
        
        // 发布多条消息
        Console.WriteLine("\n2. 发布多条消息...");
        for (int i = 0; i < 5; i++)
        {
            var batchMessage = new {
                MessageId = Guid.NewGuid().ToString(),
                Content = $"批量消息 #{i + 1}",
                BatchId = "batch-001",
                Timestamp = DateTime.Now,
                Sequence = i + 1
            };
            
            var batchResult = await engine.PublishMessageAsync("batch-topic", batchMessage);
            Console.WriteLine($"   消息 #{i + 1}: {batchResult.Success ? "成功" : "失败"}");
        }
        
        Console.WriteLine("\n" + "=" * 50);
        Console.WriteLine("发布订阅示例完成！");
    }
}
```

### 3. 服务调用示例

```csharp
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dapr.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Dapr AOT 服务调用示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机
        var builder = Host.CreateApplicationBuilder();
        
        // 配置Dapr选项
        builder.Configuration.AddJsonFile("dapr_aot.setting.json", optional: true);
        builder.Services.Configure<DaprOptions>(builder.Configuration.GetSection("Dapr"));
        
        // 注册服务
        var daprOptions = builder.Configuration.GetSection("Dapr").Get<DaprOptions>() ?? new DaprOptions();
        builder.Services.AddDaprClient(daprOptions);
        builder.Services.AddSingleton<IDaprService, DaprService>();
        builder.Services.AddSingleton<DaprAotEngine>();
        
        // 构建主机并获取服务提供者
        var host = builder.Build();
        var serviceProvider = host.Services;
        
        // 获取Dapr AOT引擎
        var engine = serviceProvider.GetRequiredService<DaprAotEngine>();
        
        // 准备服务调用数据
        var requestData = new {
            Operation = "add",
            Numbers = new[] { 10, 20, 30 }
        };
        
        // 调用服务
        Console.WriteLine($"\n1. 调用服务 'calculator-service' 方法 'api/calculate'...");
        try
        {
            var invokeResult = await engine.InvokeServiceAsync<object, object>(
                "calculator-service", "/api/calculate", requestData);
            
            Console.WriteLine($"   调用结果: {invokeResult.Success ? "成功" : "失败"}");
            if (invokeResult.Success && invokeResult.ResultData != null)
            {
                Console.WriteLine($"   返回结果: {Newtonsoft.Json.JsonConvert.SerializeObject(invokeResult.ResultData)}");
                Console.WriteLine($"   执行时间: {invokeResult.ExecutionTimeMs} ms");
            }
            else
            {
                Console.WriteLine($"   错误信息: {invokeResult.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   调用异常: {ex.Message}");
            Console.WriteLine($"   注意: 确保目标服务 'calculator-service' 已启动并注册到Dapr");
        }
        
        Console.WriteLine("\n" + "=" * 50);
        Console.WriteLine("服务调用示例完成！");
    }
}
```

### 4. 绑定调用示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dapr.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Dapr AOT 绑定调用示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机
        var builder = Host.CreateApplicationBuilder();
        
        // 配置Dapr选项
        builder.Configuration.AddJsonFile("dapr_aot.setting.json", optional: true);
        builder.Services.Configure<DaprOptions>(builder.Configuration.GetSection("Dapr"));
        
        // 注册服务
        var daprOptions = builder.Configuration.GetSection("Dapr").Get<DaprOptions>() ?? new DaprOptions();
        builder.Services.AddDaprClient(daprOptions);
        builder.Services.AddSingleton<IDaprService, DaprService>();
        builder.Services.AddSingleton<DaprAotEngine>();
        
        // 构建主机并获取服务提供者
        var host = builder.Build();
        var serviceProvider = host.Services;
        
        // 获取Dapr AOT引擎
        var engine = serviceProvider.GetRequiredService<DaprAotEngine>();
        
        // 准备绑定调用数据
        var bindingData = new {
            Message = "Dapr AOT 绑定调用测试",
            Source = "DaprAotExample",
            Timestamp = DateTime.Now,
            Action = "test"
        };
        
        // 调用绑定
        Console.WriteLine($"\n1. 调用绑定 'test-binding' 操作 'create'...");
        try
        {
            var bindingResult = await engine.InvokeBindingAsync("test-binding", "create", bindingData);
            
            Console.WriteLine($"   绑定调用结果: {bindingResult.Success ? "成功" : "失败"}");
            if (bindingResult.Success)
            {
                Console.WriteLine($"   执行时间: {bindingResult.ExecutionTimeMs} ms");
            }
            else
            {
                Console.WriteLine($"   错误信息: {bindingResult.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   绑定调用异常: {ex.Message}");
            Console.WriteLine($"   注意: 确保绑定组件 'test-binding' 已正确配置");
        }
        
        Console.WriteLine("\n" + "=" * 50);
        Console.WriteLine("绑定调用示例完成！");
    }
}
```

### 5. 状态监控示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dapr.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Dapr AOT 状态监控示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机
        var builder = Host.CreateApplicationBuilder();
        
        // 配置Dapr选项
        builder.Configuration.AddJsonFile("dapr_aot.setting.json", optional: true);
        builder.Services.Configure<DaprOptions>(builder.Configuration.GetSection("Dapr"));
        
        // 注册服务
        var daprOptions = builder.Configuration.GetSection("Dapr").Get<DaprOptions>() ?? new DaprOptions();
        builder.Services.AddDaprClient(daprOptions);
        builder.Services.AddSingleton<IDaprService, DaprService>();
        builder.Services.AddSingleton<DaprAotEngine>();
        
        // 构建主机并获取服务提供者
        var host = builder.Build();
        var serviceProvider = host.Services;
        
        // 获取Dapr AOT引擎
        var engine = serviceProvider.GetRequiredService<DaprAotEngine>();
        
        // 执行一些操作来生成状态数据
        Console.WriteLine("1. 执行一些操作来生成状态数据...");
        for (int i = 0; i < 3; i++)
        {
            var testData = new {
                Id = $"status-test-{i}",
                Value = $"Status test data {i + 1}",
                Timestamp = DateTime.Now
            };
            
            await engine.SaveStateAsync($"status-key-{i}", testData);
            Console.WriteLine($"   操作 #{i + 1} 完成");
        }
        
        // 获取Dapr状态
        Console.WriteLine("\n2. 获取Dapr服务状态...");
        var status = await engine.GetStatusAsync();
        
        Console.WriteLine($"   运行状态: {status.IsRunning ? "正常" : "异常"}");
        Console.WriteLine($"   Dapr客户端: {status.IsDaprClientEnabled ? "已启用" : "已禁用"}");
        Console.WriteLine($"   Dapr主机: {status.DaprHost}:{status.DaprHttpPort}");
        Console.WriteLine($"   已处理请求: {status.ProcessedRequests}");
        Console.WriteLine($"   成功请求: {status.SuccessfulRequests}");
        Console.WriteLine($"   失败请求: {status.FailedRequests}");
        Console.WriteLine($"   平均执行时间: {status.AverageExecutionTimeMs} ms");
        Console.WriteLine($"   服务启动时间: {status.StartTime.ToLocalTime()}");
        
        // 重置状态
        Console.WriteLine("\n3. 重置Dapr服务状态...");
        var resetResult = await engine.ResetStatusAsync();
        Console.WriteLine($"   重置结果: {resetResult ? "成功" : "失败"}");
        
        // 验证重置
        Console.WriteLine("\n4. 验证状态重置...");
        var resetStatus = await engine.GetStatusAsync();
        Console.WriteLine($"   已处理请求: {resetStatus.ProcessedRequests} (应为 0)");
        Console.WriteLine($"   成功请求: {resetStatus.SuccessfulRequests} (应为 0)");
        Console.WriteLine($"   失败请求: {resetStatus.FailedRequests} (应为 0)");
        
        Console.WriteLine("\n" + "=" * 50);
        Console.WriteLine("状态监控示例完成！");
    }
}
```

## 总结

以上示例展示了Dapr AOT技能的主要功能和使用方法，通过这些示例，您可以：

1. 快速上手Dapr AOT的基本操作
2. 掌握状态管理、发布订阅、服务调用和绑定等核心功能
3. 了解如何配置和使用Dapr AOT引擎
4. 学习如何监控Dapr服务状态
5. 掌握错误处理和性能优化技巧

系统设计遵循.NET 10最佳实践，具有良好的扩展性和可维护性，适合各种规模和复杂度的项目。
