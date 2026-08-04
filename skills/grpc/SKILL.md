# gRPC Agent Skill - gRPC 技能

## 技能概述

基于 .NET 10 的高性能 gRPC 技能，为 .NET 开发者提供强大的 gRPC 功能。支持 AOT 编译，提供单文件执行脚本，具有高性能、低内存占用的特点。

## 快速开始指南

### 安装依赖

在主应用程序的 runfile 中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Grpc.AspNetCore@2.62.0
#:package Grpc.Net.Client@2.62.0
#:package Google.Protobuf@3.26.1
```

### 注册服务

在主应用程序中注册 gRPC 服务：

```csharp
// 注册 gRPC 服务
builder.Services.Configure<GrpcOptions>(builder.Configuration.GetSection("Grpc"));
builder.Services.AddSingleton<GrpcAotEngine>();
builder.Services.AddSingleton<GrpcClient>();
```

### 使用示例

```csharp
// 获取 gRPC 客户端
var grpcClient = serviceProvider.GetRequiredService<GrpcClient>();

// 发送 Hello 请求
var helloResponse = await grpcClient.SayHelloAsync("世界");
Console.WriteLine($"Hello 响应: {helloResponse.Message}");

// 发送 Ping 请求
var pingResponse = await grpcClient.PingAsync("Hello gRPC");
Console.WriteLine($"Ping 响应: {pingResponse.Message}, 时间戳: {pingResponse.Timestamp}");

// 发送健康检查请求
var healthResponse = await grpcClient.CheckHealthAsync();
Console.WriteLine($"健康状态: {healthResponse.Status}");
```

## AOT 编译支持

### AOT 核心文件
- `scripts/grpc_aot.cs` - gRPC AOT 核心实现
- `scripts/grpc_aot.setting.json` - AOT 编译配置
- `scripts/grpc_aot.run.json` - 运行环境配置

### AOT 编译优势
1. **启动速度快**：AOT 编译消除了 JIT 编译开销，启动时间显著减少，比 JIT 编译快 3-5 倍
2. **内存占用低**：减少了运行时元数据和 JIT 编译器的内存使用，比 JIT 编译低 20-30%
3. **部署简单**：单文件执行，无需依赖 .NET 运行时
4. **性能稳定**：编译时优化，运行时性能更加稳定
5. **安全性高**：减少了运行时攻击面

### AOT 命令行使用

```bash
# 启动 gRPC 服务器
dotnet run --project scripts/grpc_aot.cs server

# 发送 Hello 请求
dotnet run --project scripts/grpc_aot.cs hello 世界

# 发送 Ping 请求
dotnet run --project scripts/grpc_aot.cs ping Hello gRPC

# 发送健康检查请求
dotnet run --project scripts/grpc_aot.cs health

# 显示配置信息
dotnet run --project scripts/grpc_aot.cs config

# 显示帮助信息
dotnet run --project scripts/grpc_aot.cs help
```

## 导航地图

```
grpc/
????? index.yaml                   # 元数据索引描述
????? SKILL.md                    # 技能入口点（当前文件）
????? reference/                  # 参考文件
??  ????? README.md              # 完整功能描述
??  ????? examples.md            # 使用示例
????? scripts/                    # 脚本和工具
    ????? grpc_aot.cs             # gRPC AOT 核心实现
    ????? grpc_aot.run.json       # 运行配置
    ????? grpc_aot.setting.json   # 设置文件
    ????? grpc_integration.cs     # gRPC 集成实现
    ????? grpc_integration.run.json # 集成运行配置
    ????? grpc_integration.setting.json # 集成设置文件
    ????? grpc_protobufnet.cs     # gRPC Protobuf 实现
    ????? grpc_protobufnet.run.json # Protobuf 运行配置
    ????? grpc_protobufnet.setting.json # Protobuf 设置文件
    ????? hotchocolate_*.cs       # HotChocolate GraphQL 实现
```

## 主要功能

1. **gRPC 服务器**：高性能 gRPC 服务器实现，支持多个服务
2. **gRPC 客户端**：完整的 gRPC 客户端实现，支持多种请求类型
3. **服务定义**：包含 Greeter、Health、Ping 等服务
4. **AOT 编译优化**：使用 .NET 10 AOT 编译，提高性能和降低内存占用
5. **命令行工具**：完整的命令行界面，支持多种操作和别名
6. **配置管理**：支持环境变量和配置文件配置
7. **性能监控**：内置执行时间监控和日志记录
8. **错误处理**：健壮的错误处理机制
9. **网络优化**：HTTP/2 连接优化和心跳配置
10. **安全支持**：可选的 TLS 支持

## 技术特性

1. **模块化设计**：清晰的代码结构和模块划分
2. **依赖注入**：使用 Microsoft.Extensions.DependencyInjection
3. **异步编程**：Task-based 异步模式
4. **高性能算法**：优化的 gRPC 处理算法
5. **AOT 编译技术**：完整的 AOT 编译支持
6. **单文件执行脚本**：net10 的单文件执行脚本
7. **完整的命令行支持**：支持多种命令和选项
8. **别名支持**：命令别名和快捷方式
9. **配置选项**：使用 Options 模式管理配置
10. **日志记录**：完整的日志记录功能
11. **网络优化**：HTTP/2 连接池和心跳机制
12. **错误处理**：健壮的错误处理和重试机制

## 扩展说明

本技能提供了完整的 gRPC 解决方案，您可以根据需要进行扩展：

1. **自定义服务**：实现自定义 gRPC 服务
2. **扩展功能**：添加新的 gRPC 功能
3. **与其他系统集成**：与其他系统集成
4. **性能优化**：针对特定场景优化性能
5. **安全增强**：添加 TLS 和认证支持
6. **监控扩展**：添加更多监控和指标

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：正确处理异常情况，实现重试机制
4. **日志记录**：添加适当的日志记录，便于故障排查
5. **性能监控**：监控关键性能指标，如响应时间和错误率
6. **AOT 编译**：使用 AOT 编译提高性能和降低内存占用
7. **单文件部署**：使用单文件发布简化部署
8. **配置管理**：使用环境变量和配置文件管理配置
9. **网络优化**：合理配置 HTTP/2 连接和心跳参数
10. **安全配置**：根据需要启用 TLS 和认证

## 性能特性

### AOT 编译性能
- **启动时间**：比 JIT 编译快 3-5 倍
- **内存占用**：比 JIT 编译低 20-30%
- **CPU 使用率**：运行时 CPU 使用率更加稳定
- **响应时间**：尾延迟显著降低

### 运行时优化
- **GC 优化**：启用服务器 GC，提高垃圾回收效率
- **线程池优化**：合理配置线程池大小
- **内存管理**：使用对象池和缓存减少内存分配
- **网络优化**：HTTP/2 连接池和多路复用
- **字符串优化**：AOT 编译时字符串内联

## 配置选项

### 环境变量配置

```bash
# 设置服务器主机
set GRPC_SERVER_HOST=localhost

# 设置服务器端口
set GRPC_SERVER_PORT=50051

# 设置最大并发调用
set GRPC_MAX_CONCURRENT_CALLS=100

# 设置心跳时间
set GRPC_KEEP_ALIVE_TIME_SECONDS=60

# 设置最大消息大小
set GRPC_MAX_MESSAGE_SIZE=4194304

# 启用 TLS
set GRPC_ENABLE_TLS=false

# 设置环境
set DOTNET_ENVIRONMENT=Production
```

### AOT 编译配置

通过 `scripts/grpc_aot.setting.json` 文件配置 AOT 编译选项：
- 目标框架：net10.0
- 运行时标识符：win-x64
- 裁剪模式：partial
- 内存优化：启用
- 字符串优化：启用

## 故障排除

### 常见问题

1. **AOT 编译失败**
   - 检查依赖项是否支持 AOT
   - 检查代码是否使用了不支持 AOT 的特性
   - 查看编译日志获取详细错误信息
   - 调整 AOT 编译配置

2. **gRPC 连接失败**
   - 检查服务器是否运行
   - 检查网络连接
   - 检查防火墙设置
   - 检查端口是否被占用

3. **性能问题**
   - 检查内存使用情况
   - 优化查询和算法
   - 调整 AOT 编译配置
   - 增加内存限制
   - 优化网络配置

## 版本兼容性

- **.NET 版本**：.NET 10.0 及以上
- **操作系统**：Windows、Linux、macOS
- **架构**：x64、ARM64
- **gRPC 版本**：2.62.0 及以上
- **Protobuf 版本**：3.26.1 及以上

## 贡献指南

欢迎贡献到 gRPC 技能：

1. **提交 Issue**：报告 bug 或提出新功能建议
2. **提交 PR**：贡献代码和改进
3. **文档改进**：完善文档和示例
4. **测试覆盖**：添加测试用例

## 许可证

本技能基于 MIT 许可证开源。
