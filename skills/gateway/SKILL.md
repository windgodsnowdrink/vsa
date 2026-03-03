# Gateway Agent Skill - Gateway 技能

## 技能概述

基于 .NET 10 的高性能 Gateway 技能，为 .NET 开发者提供强大的网关功能。支持 AOT 编译，提供单文件执行脚本，具有高性能、低内存占用的特点。

## 快速开始指南

### 安装依赖

在主应用程序的 runfile 中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.Extensions.Http@10.0.0
#:package Yarp.ReverseProxy@2.0.0
```

### 注册服务

在主应用程序中注册 Gateway 服务：

```csharp
// 注册 Gateway 服务
builder.Services.Configure<GatewayOptions>(builder.Configuration.GetSection("Gateway"));
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IGatewayService, GatewayService>();
builder.Services.AddSingleton<GatewayAotEngine>();
```

### 使用示例

```csharp
// 获取 Gateway 服务
var gatewayService = serviceProvider.GetRequiredService<IGatewayService>();

// 启动网关服务
var startResult = await gatewayService.StartAsync();
Console.WriteLine($"启动结果: {(startResult.Success ? "成功" : "失败")}");

// 添加路由
var addRouteResult = await gatewayService.AddRouteAsync("api_route", "/api/{**remainder}", "api_cluster", "http://localhost:5000");
Console.WriteLine($"添加路由结果: {(addRouteResult.Success ? "成功" : "失败")}");

// 列出路由
var listRoutesResult = await gatewayService.ListRoutesAsync();
Console.WriteLine($"列出路由结果: {(listRoutesResult.Success ? "成功" : "失败")}");
```

## 导航地图

```
gateway/
????? index.yaml                   # 元数据索引描述
????? SKILL.md                    # 技能入口点（当前文件）
????? reference/                  # 参考文件
??  ????? README.md              # 完整功能描述
??  ????? examples.md            # 使用示例
????? scripts/                    # 脚本和工具
    ????? gateway_aot.cs           # Gateway AOT 核心实现
    ????? gateway_aot.run.json     # 运行配置
    ????? gateway_aot.setting.json # 设置文件
    ????? castle_proxy_demo.cs     # Castle 代理演示
    ????? reverse_proxy_demo.cs    # 反向代理演示
    ????? yarp_advanced_features.cs # YARP 高级功能
    ????? yarp_batch_middleware.cs # YARP 批处理中间件
    ????? yarp_circuit_breaker.cs  # YARP 电路 breaker
    ????? yarp_compression_middleware.cs # YARP 压缩中间件
    ????? yarp_integration.cs      # YARP 集成
    ????? yarp_service_discovery.cs # YARP 服务发现
    ????? yarp_signature_middleware.cs # YARP 签名中间件
```

## 主要功能

1. **网关服务管理**：启动、停止、重启网关服务
2. **路由管理**：添加、删除、列出路由
3. **状态监控**：获取网关服务状态
4. **版本信息**：获取网关版本信息
5. **AOT 编译优化**：使用 .NET 10 AOT 编译，提高性能和降低内存占用
6. **高性能设计**：优化的请求处理和路由转发
7. **易用的 API**：简单直观的 API 设计
8. **可扩展架构**：支持自定义扩展
9. **电路 breaker**：内置电路 breaker 功能，提高系统稳定性
10. **压缩支持**：内置请求/响应压缩功能

## 扩展说明

本技能提供了完整的 Gateway 解决方案，您可以根据需要进行扩展：

1. **自定义实现**：实现 IGatewayService 接口
2. **扩展功能**：添加新的网关功能
3. **与其他系统集成**：与其他系统集成
4. **性能优化**：针对特定场景优化性能
5. **中间件扩展**：添加自定义中间件

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：正确处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **AOT 编译**：使用 AOT 编译提高性能和降低内存占用
7. **单文件部署**：使用单文件发布简化部署
8. **路由管理**：合理规划路由配置
9. **电路 breaker**：合理配置电路 breaker 参数
10. **压缩配置**：根据实际情况配置压缩选项
