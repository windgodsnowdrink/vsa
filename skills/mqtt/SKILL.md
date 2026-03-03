# MQTT 智能体技能 - MQTT 消息队列技能

## 技能概述

基于 .NET 10 的高性能 MQTT（Message Queuing Telemetry Transport）消息队列技能，采用 AOT 编译优化，为 .NET 开发者提供强大的 MQTT 功能，支持 MQTT 客户端和服务器实现，提供高性能、高可靠性的消息传递服务。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package MQTTnet@4.1.4.561
#:package System.Threading.Channels@8.0.0
#:package System.IO.Pipelines@7.0.0
```

### 注册服务

在您的主应用程序中注册 MQTT 服务：

```csharp
// 注册 MQTT 服务
builder.Services.AddSingleton<IMqttClientService, MqttClientService>();
builder.Services.AddSingleton<IMqttServerService, MqttServerService>();
builder.Services.AddOptions<MqttClientOptions>()
    .Configure(options => {
        options.ClientId = "your-client-id";
        options.Host = "localhost";
        options.Port = 1883;
        options.ProtocolVersion = MQTTnet.Formatter.MqttProtocolVersion.V500;
    });
```

### 使用示例

```csharp
// 获取 MQTT 客户端服务
var mqttClientService = serviceProvider.GetRequiredService<IMqttClientService>();

// 连接到 MQTT 服务器
await mqttClientService.ConnectAsync();

// 发布消息
await mqttClientService.PublishAsync(
    topic: "test/topic",
    payload: "Hello MQTT!",
    qualityOfServiceLevel: MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce
);

// 订阅主题
await mqttClientService.SubscribeAsync(
    topicFilter: "test/#",
    qualityOfServiceLevel: MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce,
    messageHandler: (sender, e) => {
        Console.WriteLine($"收到消息: 主题 = {e.ApplicationMessage.Topic}, 负载 = {Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)}");
    }
);

// 断开连接
await mqttClientService.DisconnectAsync();
```

## AOT 架构执行

### AOT 编译配置

```yaml
#:property PublishAot=true
#:property IncludeNativeLibrariesForSelfExtract=true
#:property EnableCppCodeGen=true
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
#:property RuntimeIdentifier=linux-x64
#:property RuntimeIdentifier=osx-x64
```

### 执行流程

1. **编译阶段**：使用 .NET 10 的 AOT 编译功能将 MQTT 代码编译为本地机器码，提高运行性能
2. **打包阶段**：将编译后的代码打包为单文件可执行文件，包含所有依赖项
3. **部署阶段**：将打包后的可执行文件部署到目标环境，无需安装 .NET 运行时
4. **运行阶段**：执行单文件可执行文件，处理 MQTT 消息传递，享受 AOT 编译带来的性能优势

### AOT 架构优势

1. **启动速度快**：AOT 编译消除了 JIT 编译开销，启动时间显著缩短，特别适合 IoT 设备等资源受限场景
2. **内存占用低**：减少了运行时编译所需的内存，降低了内存使用，适合内存有限的环境
3. **执行效率高**：本地机器码执行效率更高，特别是对于消息处理等计算密集型操作
4. **部署简单**：单文件可执行文件，无需依赖外部运行时，简化了部署流程
5. **安全性强**：减少了运行时攻击面，提高了应用程序安全性

## 导航地图

```
mqtt/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── mqtt_integration.cs      # MQTT 集成实现
    ├── mqtt_integration.run.json  # MQTT 集成运行配置
    ├── mqtt_integration.setting.json  # MQTT 集成设置
    ├── mqtt_optimized.cs        # 优化的 MQTT 实现
    ├── mqtt_optimized.run.json  # 优化的 MQTT 运行配置
    ├── mqtt_optimized.setting.json  # 优化的 MQTT 设置
    ├── mqtt_advanced_security.cs  # 高级 MQTT 安全实现
    ├── mqtt_advanced_security.run.json  # 高级 MQTT 安全运行配置
    ├── mqtt_advanced_security.setting.json  # 高级 MQTT 安全设置
    └── ...                     # 更多 MQTT 相关脚本
```

## 主要功能

1. **MQTT 客户端实现**: 支持 MQTT v3.1.1 和 v5.0 协议，提供高性能、可靠的 MQTT 客户端功能
2. **MQTT 服务器实现**: 支持 MQTT v3.1.1 和 v5.0 协议，提供可扩展、高性能的 MQTT 服务器功能
3. **高级安全特性**: 支持 TLS/SSL 加密、用户名密码认证、客户端证书认证等安全机制
4. **高性能设计**: 采用 .NET 10 最新特性，包括 System.Threading.Channels、Span 零拷贝等技术，提供高性能的 MQTT 消息处理
5. **异步编程模型**: 全面支持异步编程，提供高效的并发处理能力
6. **可扩展架构**: 采用模块化设计，支持自定义消息处理、协议扩展等
7. **丰富的示例**: 提供多种场景下的 MQTT 使用示例，包括基本连接、发布订阅、高级安全、性能优化等

## 扩展说明

此技能提供完整的 MQTT 解决方案，您可以根据需要进行扩展：

1. **自定义消息处理器**: 实现自定义的 MQTT 消息处理器，处理特定类型的消息
2. **协议扩展**: 扩展 MQTT 协议，支持自定义的 MQTT 功能
3. **集成其他系统**: 将 MQTT 与其他系统集成，如数据库、消息队列、云服务等
4. **性能优化**: 针对特定场景优化 MQTT 性能，如高并发、大数据量等
5. **安全增强**: 实现更高级的安全机制，如生物识别、量子安全等

## 最佳实践

1. **依赖注入**: 使用依赖注入来管理 MQTT 服务，提高代码的可测试性和可维护性
2. **异步编程**: 优先使用异步 API 进行 MQTT 操作，避免阻塞主线程
3. **错误处理**: 正确处理 MQTT 操作中的异常情况，确保系统的可靠性
4. **日志记录**: 添加适当的日志记录，便于调试和监控
5. **性能监控**: 监控 MQTT 系统的性能指标，如消息延迟、吞吐量等
6. **资源管理**: 正确管理 MQTT 连接、订阅等资源，避免资源泄漏
7. **安全配置**: 配置适当的安全机制，保护 MQTT 通信的安全性
8. **负载均衡**: 对于高并发场景，考虑使用负载均衡来分布 MQTT 服务器的负载

## 配置选项

### MqttClientOptions 配置

```json
{
  "MqttClientOptions": {
    "ClientId": "your-client-id",
    "Host": "localhost",
    "Port": 1883,
    "ProtocolVersion": "V500",
    "UserName": "your-username",
    "Password": "your-password",
    "EnableTls": false,
    "KeepAlivePeriod": "00:00:30",
    "CleanSession": true,
    "ConnectionTimeout": "00:01:00"
  }
}
```

### MqttServerOptions 配置

```json
{
  "MqttServerOptions": {
    "DefaultEndpointPort": 1883,
    "DefaultEndpointEnabled": true,
    "ConnectionBacklog": 100,
    "MaximumPendingMessagesPerClient": 1000,
    "EnablePersistentSessions": true,
    "KeepAliveMonitorInterval": "00:00:30",
    "ConnectionValidator": {
      "Enable": true,
      "AllowAnonymous": false
    }
  }
}
```

## 性能优化建议

1. **使用高性能的消息处理机制**: 如 System.Threading.Channels 进行消息队列处理
2. **采用零拷贝技术**: 使用 Span<T> 和 Memory<T> 减少内存拷贝
3. **优化连接管理**: 合理设置连接超时、心跳间隔等参数
4. **使用异步 I/O**: 充分利用 .NET 10 的异步 I/O 特性
5. **配置适当的线程池**: 根据系统负载调整线程池大小
6. **使用高性能的序列化机制**: 如 MessagePack、Protobuf 等
7. **优化消息批处理**: 对于大量小消息，考虑使用批处理减少网络开销
8. **合理设置 QoS 级别**: 根据业务需求选择适当的 QoS 级别，避免不必要的开销

## 故障排除

### 常见问题

1. **连接失败**: 检查 MQTT 服务器地址、端口、用户名密码等配置
2. **消息丢失**: 检查 QoS 级别设置、网络连接稳定性、服务器资源等
3. **性能问题**: 检查系统资源使用情况、线程池配置、消息处理逻辑等
4. **安全问题**: 检查 TLS/SSL 配置、认证机制、访问控制等
5. **兼容性问题**: 检查 MQTT 协议版本、客户端库版本等

### 调试建议

1. **启用详细日志**: 配置 MQTT 客户端和服务器的详细日志记录
2. **使用 MQTT 调试工具**: 如 MQTT Explorer、Mosquitto CLI 等工具进行调试
3. **监控系统资源**: 监控 CPU、内存、网络等系统资源的使用情况
4. **分析消息流量**: 分析 MQTT 消息的流量、延迟、丢失率等指标
5. **进行压力测试**: 使用压力测试工具验证系统在高负载下的表现

## AOT 编译最佳实践

1. **避免反射**: 使用静态分析可检测的代码，减少反射使用
2. **避免动态类型**: 使用强类型，提高编译时类型检查
3. **避免运行时代码生成**: 使用预编译代码，减少运行时开销
4. **优化内存使用**: 使用 Span<T> 和 Memory<T>，减少内存拷贝
5. **减少依赖**: 最小化依赖项，减少编译时间和可执行文件大小
6. **使用值类型**: 减少 GC 压力，提高内存访问效率
7. **避免大对象分配**: 避免分配大于 85KB 的对象，减少大对象堆使用
8. **使用对象池**: 对于频繁创建和销毁的对象，使用对象池提高性能
9. **资源管理**: 确保所有资源在 AOT 编译时都能被正确处理
10. **测试验证**: 在 AOT 编译后进行充分的测试，确保应用正常运行

## 部署说明

### 部署步骤

1. **编译**: 使用 .NET 10 SDK 编译代码
2. **打包**: 打包为单文件可执行文件
3. **部署**: 部署到目标环境
4. **配置**: 配置环境变量和配置文件
5. **启动**: 启动服务

### 环境要求

- .NET 10 运行时或更高版本
- 支持 AOT 编译的操作系统
- 网络连接（用于 MQTT 通信）
- 足够的内存和 CPU 资源

### 配置文件

```json
{
  "MqttClientOptions": {
    "ClientId": "your-client-id",
    "Host": "localhost",
    "Port": 1883
  },
  "MqttServerOptions": {
    "DefaultEndpointPort": 1883,
    "DefaultEndpointEnabled": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 监控和维护

### 监控指标

1. **消息吞吐量**: 单位时间内处理的消息数量
2. **消息延迟**: 消息从发布到接收的时间
3. **连接数**: 当前活跃的 MQTT 连接数
4. **订阅数**: 当前活跃的 MQTT 订阅数
5. **错误率**: MQTT 操作的错误率
6. **资源使用**: CPU、内存、网络等资源的使用情况

### 维护建议

1. **定期检查**: 定期检查 MQTT 服务状态
2. **优化配置**: 根据实际使用情况优化配置
3. **更新依赖**: 定期更新 MQTTnet 等依赖项
4. **性能测试**: 定期进行性能测试
5. **安全审计**: 定期进行安全审计
6. **备份**: 定期备份配置和关键数据
7. **监控**: 建立完善的监控系统
8. **告警**: 设置合理的告警阈值
