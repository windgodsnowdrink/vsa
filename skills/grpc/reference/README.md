# gRPC - 参考文档

## 概述

gRPC 是基于 .NET 10 的高性能 gRPC 系统，专为 .NET 开发者设计，支持 AOT 编译，提供单文件执行脚本，具有高性能、低内存占用的特点。

## 核心组件

### 1. gRPC AOT 引擎
- **位置**: scripts/grpc_aot.cs
- **功能**: 核心 gRPC 业务逻辑处理，包括服务端和客户端实现
- **特性**: 
  - 完整的 gRPC 服务端实现
  - 完整的 gRPC 客户端实现
  - 多种服务定义（Greeter、Health、Ping）
  - AOT 编译优化
  - 命令行工具支持
  - 配置管理
  - 性能监控
  - 错误处理
  - 网络优化

## 使用示例

### 基本用法

```csharp
// 启动 gRPC 服务器
var server = new GrpcServer();
await server.StartAsync();

// 使用 gRPC 客户端
var client = new GrpcClient();
var response = await client.SayHelloAsync("World");
Console.WriteLine(response.Message);
```

### 高级配置

```csharp
var grpcSettings = new GrpcSettings {
    ServerHost = "localhost",
    ServerPort = 50051,
    MaxConcurrentCalls = 100,
    KeepAliveTimeSeconds = 60,
    MaxMessageSize = 4194304,
    EnableTls = false
};

builder.Services.Configure<GrpcSettings>(options => {
    options.ServerHost = grpcSettings.ServerHost;
    options.ServerPort = grpcSettings.ServerPort;
    options.MaxConcurrentCalls = grpcSettings.MaxConcurrentCalls;
    options.KeepAliveTimeSeconds = grpcSettings.KeepAliveTimeSeconds;
    options.MaxMessageSize = grpcSettings.MaxMessageSize;
    options.EnableTls = grpcSettings.EnableTls;
});
```

## 配置选项

### gRPC 配置

```json
{
  "GrpcSettings": {
    "ServerHost": "localhost",         // 服务器主机
    "ServerPort": 50051,               // 服务器端口
    "MaxConcurrentCalls": 100,         // 最大并发调用
    "KeepAliveTimeSeconds": 60,        // 心跳时间
    "MaxMessageSize": 4194304,         // 最大消息大小
    "EnableTls": false,                // 启用 TLS
    "CertificatePath": "",            // 证书路径
    "CertificatePassword": ""          // 证书密码
  }
}
```

## 性能优化

1. **AOT 编译**: 使用 .NET 10 AOT 编译，提高性能和降低内存占用
2. **连接池**: 使用 HTTP/2 连接池管理资源
3. **异步编程**: 使用异步 API 避免阻塞
4. **批处理**: 批量处理提高效率
5. **内存优化**: 使用对象池和缓存减少内存分配
6. **网络优化**: HTTP/2 多路复用和心跳配置

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
   - 调整线程池大小

3. **AOT 编译问题**
   - 确保使用支持的依赖项
   - 检查 Trim 模式配置
   - 验证运行时标识符设置

## 扩展开发

### 添加自定义服务

```csharp
public class CustomService : Custom.CustomBase
{
    public override async Task<CustomResponse> CustomMethod(CustomRequest request, ServerCallContext context)
    {
        // 实现自定义逻辑
        return new CustomResponse {
            Result = "Success",
            Message = $"Processed: {request.Input}"
        };
    }
}

// 注册服务
builder.Services.AddSingleton<CustomService>();
```
