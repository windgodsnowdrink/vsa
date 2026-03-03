# FreeIM - 参考文档

## 概述

FreeIM 是基于 .NET 10 的高性能即时通讯系统，专为 .NET 开发者设计。支持 AOT 编译，提供单文件执行脚本，具有高性能、低内存占用的特点。

## 核心组件

### 1. FreeIMService (FreeIM 服务)
- **位置**: scripts/freeim_aot.cs
- **功能**: 核心业务逻辑处理
- **特性**: 
  - 消息发送与接收
  - 用户连接管理
  - 在线用户列表
  - 版本信息查询
  - 性能优化
  - 错误处理
  - 日志记录

### 2. FreeIMAotEngine (FreeIM AOT 引擎)
- **位置**: scripts/freeim_aot.cs
- **功能**: 命令行引擎，处理命令行参数和执行命令
- **特性**: 
  - 命令行参数解析
  - 别名支持
  - 结果输出格式化
  - 错误处理

## 使用示例

### 基本使用

```csharp
// 获取 FreeIM 服务
var freeIMService = serviceProvider.GetRequiredService<IFreeIMService>();

// 连接到 IM 服务器
var connectResult = await freeIMService.ConnectAsync("user1");
Console.WriteLine($"连接结果: {(connectResult.Success ? "成功" : "失败")}");

// 发送消息
var sendResult = await freeIMService.SendMessageAsync("user1", "user2", "Hello World");
Console.WriteLine($"发送消息结果: {(sendResult.Success ? "成功" : "失败")}");

// 接收消息
var receiveResult = await freeIMService.ReceiveMessageAsync("user2");
Console.WriteLine($"接收消息结果: {(receiveResult.Success ? "成功" : "失败")}");

// 列出用户
var listResult = await freeIMService.ListUsersAsync();
Console.WriteLine($"列出用户结果: {(listResult.Success ? "成功" : "失败")}");

// 断开连接
var disconnectResult = await freeIMService.DisconnectAsync("user1");
Console.WriteLine($"断开连接结果: {(disconnectResult.Success ? "成功" : "失败")}");
```

### 高级配置

```csharp
// 配置 FreeIM 选项
var freeIMOptions = new FreeIMOptions
{
    EnableCache = true,
    CacheSize = 1000,
    RequestTimeoutMs = 30000,
    EnableDetailedLogging = false,
    ServerAddress = "localhost",
    ServerPort = 8080,
    EnableSsl = false
};

builder.Services.Configure<FreeIMOptions>(options => {
    options.EnableCache = freeIMOptions.EnableCache;
    options.CacheSize = freeIMOptions.CacheSize;
    options.RequestTimeoutMs = freeIMOptions.RequestTimeoutMs;
    options.EnableDetailedLogging = freeIMOptions.EnableDetailedLogging;
    options.ServerAddress = freeIMOptions.ServerAddress;
    options.ServerPort = freeIMOptions.ServerPort;
    options.EnableSsl = freeIMOptions.EnableSsl;
});
```

## 配置选项

### FreeIM 配置

```json
{
  "FreeIM": {
    "WorkingDirectory": "d:\\Trae\\vsa\\skills\\freeim",
    "EnableDetailedLogging": false,
    "EnablePerformanceMonitoring": true,
    "RequestTimeoutMs": 30000,
    "EnableCache": true,
    "CacheSize": 1000,
    "ServerAddress": "localhost",
    "ServerPort": 8080,
    "EnableSsl": false
  }
}
```

## 性能优化

1. **缓存使用**: 启用缓存以提高性能
2. **异步编程**: 使用异步 API 避免阻塞
3. **批处理**: 批量处理以提高效率
4. **连接池**: 使用连接池管理资源
5. **AOT 编译**: 使用 AOT 编译提高启动速度和运行性能
6. **单文件部署**: 减少依赖，提高部署效率

## 故障排除

### 常见问题

1. **连接失败**
   - 检查配置文件
   - 验证网络连接
   - 检查日志信息

2. **性能问题**
   - 启用缓存
   - 优化查询条件
   - 增加资源限制
   - 使用 AOT 编译

3. **AOT 编译问题**
   - 确保使用 .NET 10
   - 检查依赖项是否支持 AOT
   - 查看编译错误信息

## 扩展开发

### 添加自定义功能

```csharp
public class CustomFreeIMService : IFreeIMService
{
    private readonly FreeIMOptions _options;
    private readonly ILogger<CustomFreeIMService> _logger;

    public CustomFreeIMService(IOptions<FreeIMOptions> options, ILogger<CustomFreeIMService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<FreeIMCommandResult> SendMessageAsync(string sender, string receiver, string content, string type = "text")
    {
        // 自定义实现
        var result = new FreeIMCommandResult();
        // 实现逻辑
        return result;
    }

    // 实现其他接口方法...
}
```

### 注册自定义服务

```csharp
// 注册自定义 FreeIM 服务
builder.Services.Configure<FreeIMOptions>(builder.Configuration.GetSection("FreeIM"));
builder.Services.AddSingleton<IFreeIMService, CustomFreeIMService>();
builder.Services.AddSingleton<FreeIMAotEngine>();
```
