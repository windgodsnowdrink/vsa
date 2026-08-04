# Dapr AOT - 参考文档

## 概述

Dapr AOT是基于.NET 10 AOT架构的高性能Dapr分布式应用运行时，专为.NET开发者设计，提供强大、高效的分布式应用开发能力。

## 核心组件

### 1. IDaprService (Dapr服务接口)
- **位置**: scripts/dapr_aot.cs
- **功能**: 定义Dapr的核心功能接口
- **特性**: 
  - 状态管理（保存、获取、删除）
  - 发布订阅（发布消息）
  - 服务调用
  - 绑定调用
  - 状态监控

### 2. DaprService (Dapr服务实现)
- **位置**: scripts/dapr_aot.cs
- **功能**: 实现IDaprService接口，提供核心Dapr功能
- **特性**: 
  - 高性能实现
  - 错误处理
  - 详细日志记录
  - 性能监控
  - 异步编程模型

### 3. DaprAotEngine (Dapr AOT执行引擎)
- **位置**: scripts/dapr_aot.cs
- **功能**: 管理Dapr功能调用的执行引擎
- **特性**: 
  - 统一的API入口
  - 简化的调用方式
  - 内置的性能监控
  - 灵活的配置选项

### 4. DaprOptions (Dapr配置选项)
- **位置**: scripts/dapr_aot.cs
- **功能**: 配置Dapr的行为
- **特性**: 
  - 支持多种配置来源
  - 灵活的选项设置
  - 默认值合理
  - 类型安全

## 使用示例

### 基本用法

```csharp
// 获取Dapr AOT引擎
var engine = serviceProvider.GetRequiredService<Dapr.AOT.DaprAotEngine>();

// 准备测试数据
var testData = new {
    Id = "test-key",
    Value = "Hello Dapr!",
    Timestamp = DateTime.Now
};

// 保存状态
var saveResult = await engine.SaveStateAsync("test-state", testData);
Console.WriteLine($"保存状态{saveResult.Success ? "成功" : "失败"}");

// 获取状态
var getResult = await engine.GetStateAsync<object>("test-state");
if (getResult.Success && getResult.ResultData != null)
{
    Console.WriteLine($"获取状态成功，值: {Newtonsoft.Json.JsonConvert.SerializeObject(getResult.ResultData)}");
}

// 发布消息
var publishResult = await engine.PublishMessageAsync("test-topic", testData);
Console.WriteLine($"发布消息{publishResult.Success ? "成功" : "失败"}");
```

### 高级配置

```csharp
// 自定义Dapr选项
var daprOptions = new Dapr.AOT.DaprOptions
{
    DaprHost = "http://localhost",
    DaprHttpPort = 3500,
    DaprGrpcPort = 50001,
    EnableTracing = true,
    DefaultStateStore = "statestore",
    DefaultPubSubName = "pubsub",
    Timeout = TimeSpan.FromSeconds(60),
    RetryCount = 5,
    RetryInterval = TimeSpan.FromMilliseconds(1000),
    EnableDetailedLogging = true
};

// 配置Dapr客户端
builder.Services.AddDaprClient(daprOptions);
```

## 配置选项

### Dapr配置

```json
{
  "Dapr": {
    "EnableDaprClient": true,          // 是否启用Dapr客户端
    "DaprHost": "http://localhost",    // Dapr主机地址
    "DaprHttpPort": 3500,              // Dapr HTTP端口
    "DaprGrpcPort": 50001,             // Dapr gRPC端口
    "EnableTracing": true,             // 是否启用跟踪
    "DefaultStateStore": "statestore", // 默认状态存储名称
    "DefaultPubSubName": "pubsub",    // 默认发布订阅组件名称
    "DefaultBindingName": "binding",   // 默认绑定名称
    "Timeout": "00:00:30",             // 超时时间
    "RetryCount": 3,                   // 重试次数
    "RetryInterval": "00:00:00.5",     // 重试间隔
    "EnableDetailedLogging": false      // 是否启用详细日志
  }
}
```

## 性能优化

1. **启用AOT编译** - 启用AOT编译以获得最佳性能
2. **使用异步API** - 优先使用异步API，提高并发性能
3. **合理配置超时时间** - 根据网络状况和业务需求配置合适的超时时间
4. **启用分布式跟踪** - 在生产环境中启用分布式跟踪，便于监控和调试
5. **使用连接池** - 确保Dapr客户端使用连接池，减少连接建立的开销
6. **批量操作** - 对于批量数据，使用Dapr的批量操作功能
7. **缓存频繁访问的数据** - 对于频繁访问的数据，考虑在应用层添加缓存
8. **优化网络配置** - 确保网络配置优化，减少网络延迟

## 故障排除

### 常见问题

1. **Dapr客户端连接失败**
   - 检查Dapr运行时是否正常运行
   - 检查Dapr主机地址和端口是否正确
   - 检查网络连接是否正常
   - 查看日志信息，了解具体错误原因

2. **状态保存失败**
   - 检查状态存储组件是否正确配置
   - 检查状态键和值是否符合要求
   - 检查Dapr运行时日志，了解具体错误原因
   - 检查权限是否足够

3. **消息发布失败**
   - 检查发布订阅组件是否正确配置
   - 检查主题名称是否正确
   - 检查消息格式是否符合要求
   - 检查Dapr运行时日志，了解具体错误原因

4. **服务调用失败**
   - 检查目标服务是否正常运行
   - 检查应用ID和方法名称是否正确
   - 检查请求格式是否符合要求
   - 检查网络连接是否正常
   - 检查Dapr运行时日志，了解具体错误原因

## 扩展开发

### 自定义Dapr服务

```csharp
// 自定义Dapr服务实现
public class CustomDaprService : Dapr.AOT.DaprService
{
    public CustomDaprService(ILogger<DaprService> logger, IOptions<Dapr.AOT.DaprOptions> options, DaprClient daprClient)
        : base(logger, options, daprClient)
    {
    }
    
    // 重写SaveStateAsync方法，添加自定义逻辑
    public override async Task<Dapr.AOT.DaprResult> SaveStateAsync<T>(string key, T value, string? stateStore = null) where T : class
    {
        // 自定义预处理逻辑
        _logger.LogInformation("自定义保存状态处理开始");
        
        // 添加自定义字段
        var enhancedValue = new {
            Original = value,
            CustomField = "custom-value",
            EnhancedAt = DateTime.Now
        };
        
        // 调用基类方法执行保存
        var result = await base.SaveStateAsync(key, enhancedValue, stateStore);
        
        // 自定义后处理逻辑
        _logger.LogInformation("自定义保存状态处理完成");
        
        return result;
    }
}

// 注册自定义服务
builder.Services.AddSingleton<Dapr.AOT.IDaprService, CustomDaprService>();
```

### 自定义Dapr客户端配置

```csharp
// 自定义Dapr客户端配置
builder.Services.AddDaprClient(builder =>
{
    builder.UseHttpEndpoint("http://localhost:3500");
    builder.UseGrpcEndpoint("http://localhost:50001");
    builder.UseTracing();
    builder.UseJsonSerializationOptions(new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    });
});
```

