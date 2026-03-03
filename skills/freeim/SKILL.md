# FreeIM Agent Skill - FreeIM 技能

## 技能概述

基于 .NET 10 的高性能 FreeIM 技能，为 .NET 开发者提供强大的即时通讯功能。支持 AOT 编译，提供单文件执行脚本，具有高性能、低内存占用的特点。

## 快速开始指南

### 安装依赖

在主应用程序的 runfile 中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
```

### 注册服务

在主应用程序中注册 FreeIM 服务：

```csharp
// 注册 FreeIM 服务
builder.Services.Configure<FreeIMOptions>(builder.Configuration.GetSection("FreeIM"));
builder.Services.AddSingleton<IFreeIMService, FreeIMService>();
builder.Services.AddSingleton<FreeIMAotEngine>();
```

### 使用示例

```csharp
// 获取 FreeIM 服务
var freeIMService = serviceProvider.GetRequiredService<IFreeIMService>();

// 连接到 IM 服务器
var connectResult = await freeIMService.ConnectAsync("user1");
Console.WriteLine($"连接结果: {(connectResult.Success ? "成功" : "失败")}");

// 发送消息
var sendResult = await freeIMService.SendMessageAsync("user1", "user2", "Hello World");
Console.WriteLine($"发送消息结果: {(sendResult.Success ? "成功" : "失败")}");
```

## 导航地图

```
freeim/
????? index.yaml                   # 元数据索引描述
????? SKILL.md                    # 技能入口点（当前文件）
????? reference/                  # 参考文件
??  ????? README.md              # 完整功能描述
??  ????? examples.md            # 使用示例
????? scripts/                    # 脚本和工具
    ????? freeim_aot.cs           # FreeIM AOT 核心实现
    ????? freeim_aot.run.json     # 运行配置
    ????? freeim_aot.setting.json # 设置文件
    ????? freeim_integration.cs   # FreeIM 集成实现
    ????? freeim_integration.run.json  # 集成运行配置
    ????? freeim_integration.setting.json  # 集成设置文件
```

## 主要功能

1. **消息发送与接收**：支持文本消息的发送和接收
2. **用户连接管理**：处理用户的连接和断开操作
3. **在线用户列表**：获取当前在线用户列表
4. **版本信息查询**：获取 FreeIM 引擎的版本信息
5. **AOT 编译优化**：使用 .NET 10 AOT 编译，提高性能和降低内存占用
6. **高性能设计**：优化的消息处理和用户管理实现
7. **易用的 API**：简单直观的 API 设计
8. **可扩展架构**：支持自定义扩展

## 扩展说明

本技能提供了完整的 FreeIM 解决方案，您可以根据需要进行扩展：

1. **自定义实现**：实现 IFreeIMService 接口
2. **扩展功能**：添加新的 FreeIM 功能
3. **与其他系统集成**：与其他系统集成
4. **性能优化**：针对特定场景优化性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：正确处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **AOT 编译**：使用 AOT 编译提高性能和降低内存占用
7. **单文件部署**：使用单文件发布简化部署
