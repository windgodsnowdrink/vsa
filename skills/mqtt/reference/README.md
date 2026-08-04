# mqtt - 参考文档

## 概述

mqtt是一个基于.NET 10的高性能MQTT（Message Queuing Telemetry Transport）消息队列系统，专为.NET开发者设计，支持AOT（Ahead-of-Time）编译，提供强大的MQTT客户端和服务器功能。

## 核心组件

### 1. MQTT客户端服务 (IMqttClientService)
- **位置**: scripts/mqtt_integration.cs, scripts/mqtt_optimized.cs等
- **功能**: 提供MQTT客户端的核心功能，包括连接管理、消息发布、订阅管理等
- **特性**: 
  - 支持MQTT v3.1.1和v5.0协议
  - 异步编程模型，避免阻塞主线程
  - 支持多种QoS级别（AtMostOnce、AtLeastOnce、ExactlyOnce）
  - 自动重连机制
  - 高性能设计，支持高并发消息处理
  - 完善的错误处理和日志记录

### 2. MQTT服务器服务 (IMqttServerService)
- **位置**: scripts/mqttnet_server_only.cs, scripts/mqtt_integration.cs等
- **功能**: 提供MQTT服务器的核心功能，包括连接管理、消息路由、订阅管理等
- **特性**: 
  - 支持MQTT v3.1.1和v5.0协议
  - 高性能设计，支持大量并发客户端
  - 可扩展的插件架构
  - 完善的安全机制（认证、授权、TLS/SSL）
  - 支持持久化会话
  - 实时监控和统计

### 3. MQTT消息处理器 (IMqttMessageHandler)
- **位置**: 可在应用程序中自定义实现
- **功能**: 处理收到的MQTT消息
- **特性**: 
  - 支持自定义消息处理逻辑
  - 可根据主题路由消息
  - 支持异步消息处理
  - 可扩展的插件架构

### 4. MQTT安全服务 (IMqttSecurityService)
- **位置**: scripts/mqtt_security.cs, scripts/mqtt_advanced_security.cs等
- **功能**: 提供MQTT安全相关的功能
- **特性**: 
  - 支持用户名密码认证
  - 支持客户端证书认证
  - 支持TLS/SSL加密
  - 支持访问控制列表（ACL）
  - 支持高级安全特性（如生物识别、量子安全）

## 使用示例

### 基本MQTT客户端使用

```csharp
// 创建并配置MQTT客户端选项
var clientOptions = new MqttClientOptionsBuilder()
    .WithClientId("client-id-123")
    .WithTcpServer("localhost", 1883)
    .WithCleanSession()
    .Build();

// 创建MQTT客户端
var factory = new MqttFactory();
var mqttClient = factory.CreateMqttClient();

// 连接到MQTT服务器
await mqttClient.ConnectAsync(clientOptions);

// 发布消息
await mqttClient.PublishAsync(
    topic: "test/topic",
    payload: "Hello MQTT!",
    qualityOfServiceLevel: MqttQualityOfServiceLevel.AtLeastOnce
);

// 订阅主题
await mqttClient.SubscribeAsync(
    topicFilter: "test/#",
    qualityOfServiceLevel: MqttQualityOfServiceLevel.AtMostOnce
);

// 注册消息接收事件
mqttClient.ApplicationMessageReceivedAsync += e => {
    Console.WriteLine($"收到消息: 主题 = {e.ApplicationMessage.Topic}, 负载 = {Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)}");
    return Task.CompletedTask;
};

// 断开连接
await mqttClient.DisconnectAsync();
```

### 基本MQTT服务器使用

```csharp
// 创建并配置MQTT服务器选项
var serverOptions = new MqttServerOptionsBuilder()
    .WithDefaultEndpoint()
    .WithDefaultEndpointPort(1883)
    .Build();

// 创建MQTT服务器
var factory = new MqttFactory();
var mqttServer = factory.CreateMqttServer();

// 注册客户端连接事件
mqttServer.ClientConnectedAsync += e => {
    Console.WriteLine($"客户端已连接: {e.ClientId}");
    return Task.CompletedTask;
};

// 注册客户端断开连接事件
mqttServer.ClientDisconnectedAsync += e => {
    Console.WriteLine($"客户端已断开连接: {e.ClientId}");
    return Task.CompletedTask;
};

// 启动MQTT服务器
await mqttServer.StartAsync(serverOptions);

// 等待用户输入
Console.WriteLine("MQTT服务器已启动，按Enter键停止");
Console.ReadLine();

// 停止MQTT服务器
await mqttServer.StopAsync();
```

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
    "TlsOptions": {
      "UseTls": false,
      "CertificateValidationCallback": null,
      "SslProtocol": "Tls12"
    },
    "KeepAlivePeriod": "00:00:30",
    "CleanSession": true,
    "ConnectionTimeout": "00:01:00",
    "WillMessage": {
      "Topic": "will/topic",
      "Payload": "Client disconnected",
      "QualityOfServiceLevel": "AtMostOnce",
      "Retain": false
    }
  }
}
```

### MqttServerOptions 配置

```json
{
  "MqttServerOptions": {
    "DefaultEndpoint": {
      "Enabled": true,
      "Port": 1883
    },
    "DefaultEndpointOptions": {
      "BoundInterNetworkAddress": "0.0.0.0",
      "BoundInterNetworkV6Address": "::",
      "Port": 1883
    },
    "ConnectionBacklog": 100,
    "MaximumPendingMessagesPerClient": 1000,
    "EnablePersistentSessions": true,
    "KeepAliveMonitorInterval": "00:00:30",
    "ClientIdValidationRegex": "^[a-zA-Z0-9_\\-]+$",
    "ConnectionValidator": {
      "Enable": true,
      "AllowAnonymous": false,
      "Validators": [
        {
          "Type": "UsernamePassword",
          "Username": "admin",
          "Password": "password"
        }
      ]
    },
    "SubscriptionValidator": {
      "Enable": true,
      "Rules": [
        {
          "ClientId": "client-1",
          "AllowedTopics": ["test/1", "test/2"]
        }
      ]
    }
  }
}
```

## 性能优化

### 客户端性能优化

1. **使用适当的QoS级别**: 根据业务需求选择适当的QoS级别，避免不必要的开销
2. **批量发布消息**: 对于大量小消息，考虑使用批量发布减少网络开销
3. **优化连接管理**: 合理设置连接超时、心跳间隔等参数
4. **使用异步API**: 优先使用异步API进行MQTT操作，避免阻塞主线程
5. **优化消息处理**: 确保消息处理逻辑高效，避免在消息处理回调中执行耗时操作
6. **合理设置并发连接数**: 根据系统资源和网络条件，合理设置并发连接数

### 服务器性能优化

1. **优化连接处理**: 优化服务器的连接处理逻辑，提高并发连接能力
2. **优化消息路由**: 优化消息路由算法，提高消息传递效率
3. **使用高性能存储**: 对于持久化会话和消息，使用高性能存储
4. **配置适当的线程池**: 根据系统负载调整线程池大小
5. **优化内存使用**: 合理管理内存，避免内存泄漏和过度分配
6. **使用负载均衡**: 对于高并发场景，考虑使用负载均衡来分布服务器负载

### AOT编译优化

1. **避免使用反射**: 在AOT编译环境中，避免在运行时使用反射，或使用Source Generator替代
2. **避免动态代码生成**: 避免使用动态代码生成，如System.Reflection.Emit
3. **优化资源加载**: 确保所有资源都能在AOT编译时被正确处理
4. **使用兼容AOT的库**: 确保使用的第三方库支持AOT编译
5. **优化序列化**: 使用AOT兼容的序列化库，如MessagePack、Protobuf等
6. **测试验证**: 在AOT编译后进行充分的测试，确保应用正常运行

## 故障排除

### 常见问题及解决方案

1. **连接失败**
   - 检查MQTT服务器地址、端口是否正确
   - 检查网络连接是否正常
   - 检查防火墙设置，确保MQTT端口已开放
   - 检查认证信息（用户名、密码、证书）是否正确
   - 检查MQTT服务器是否正在运行

2. **消息丢失**
   - 检查QoS级别设置是否正确
   - 检查网络连接稳定性
   - 检查MQTT服务器资源使用情况（CPU、内存、磁盘）
   - 检查客户端和服务器的消息队列是否溢出
   - 检查客户端是否在发布消息后立即断开连接

3. **性能问题**
   - 检查系统资源使用情况（CPU、内存、网络）
   - 检查MQTT服务器配置是否合理
   - 检查客户端和服务器的并发连接数
   - 检查消息处理逻辑是否高效
   - 考虑使用更高级别的QoS是否必要

4. **安全问题**
   - 检查TLS/SSL配置是否正确
   - 检查认证机制是否配置正确
   - 检查访问控制列表（ACL）是否配置正确
   - 检查证书是否过期或无效
   - 考虑使用更高级的安全机制

5. **兼容性问题**
   - 检查MQTT协议版本是否匹配
   - 检查MQTT客户端库版本是否兼容
   - 检查MQTT服务器版本是否兼容
   - 检查自定义MQTT特性是否被所有客户端支持

## 扩展开发

### 实现自定义MQTT消息处理器

```csharp
public class CustomMqttMessageHandler : IMqttMessageHandler
{
    private readonly ILogger<CustomMqttMessageHandler> _logger;

    public CustomMqttMessageHandler(ILogger<CustomMqttMessageHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleMessageAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        try
        {
            // 自定义消息处理逻辑
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            
            _logger.LogInformation("处理自定义MQTT消息: 主题 = {Topic}, 负载 = {Payload}", topic, payload);
            
            // 根据主题处理不同类型的消息
            if (topic.StartsWith("device/"))
            {
                await HandleDeviceMessageAsync(topic, payload);
            }
            else if (topic.StartsWith("sensor/"))
            {
                await HandleSensorMessageAsync(topic, payload);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理MQTT消息时发生错误");
        }
    }

    private async Task HandleDeviceMessageAsync(string topic, string payload)
    {
        // 处理设备消息的逻辑
        await Task.CompletedTask;
    }

    private async Task HandleSensorMessageAsync(string topic, string payload)
    {
        // 处理传感器消息的逻辑
        await Task.CompletedTask;
    }
}
```

### 实现自定义MQTT连接验证器

```csharp
public class CustomMqttConnectionValidator : IMqttServerConnectionValidator
{
    private readonly ILogger<CustomMqttConnectionValidator> _logger;
    private readonly IMqttUserService _userService;

    public CustomMqttConnectionValidator(ILogger<CustomMqttConnectionValidator> logger, IMqttUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    public async Task ValidateConnectionAsync(MqttConnectionValidatorContext context)
    {
        _logger.LogInformation("验证MQTT连接: 客户端ID = {ClientId}, 用户名 = {Username}", context.ClientId, context.Username);

        // 自定义连接验证逻辑
        if (string.IsNullOrEmpty(context.ClientId))
        {
            context.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
            return;
        }

        if (string.IsNullOrEmpty(context.Username))
        {
            context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
            return;
        }

        // 验证用户名和密码
        var isValid = await _userService.ValidateCredentialsAsync(context.Username, context.Password);
        if (!isValid)
        {
            context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
            return;
        }

        // 验证通过
        context.ReasonCode = MqttConnectReasonCode.Success;
    }
}
```

### 扩展MQTT协议

```csharp
public class CustomMqttProtocolExtension : IMqttProtocolExtension
{
    private readonly ILogger<CustomMqttProtocolExtension> _logger;

    public CustomMqttProtocolExtension(ILogger<CustomMqttProtocolExtension> logger)
    {
        _logger = logger;
    }

    public int ProtocolVersion => 5;
    public string ExtensionName => "custom-extension";

    public Task InitializeAsync(MqttServer server)
    {
        _logger.LogInformation("初始化自定义MQTT协议扩展");
        // 初始化逻辑
        return Task.CompletedTask;
    }

    public Task HandleCustomMessageAsync(MqttServer server, MqttChannelAdapter client, MqttBasePacket packet)
    {
        // 处理自定义MQTT消息
        _logger.LogInformation("处理自定义MQTT消息: 类型 = {PacketType}", packet.PacketType);
        return Task.CompletedTask;
    }
}
```

## AOT编译支持

### AOT编译配置

在项目文件中添加以下配置以支持AOT编译：

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
  <TrimMode>Full</TrimMode>
  <PublishReadyToRun>true</PublishReadyToRun>
  <PublishSingleFile>true</PublishSingleFile>
  <SelfContained>true</SelfContained>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
</PropertyGroup>
```

### AOT编译命令

```bash
# 编译为Windows x64原生可执行文件
dotnet publish -c Release -r win-x64 --self-contained

# 编译为Linux x64原生可执行文件
dotnet publish -c Release -r linux-x64 --self-contained

# 编译为macOS x64原生可执行文件
dotnet publish -c Release -r osx-x64 --self-contained
```

### AOT编译注意事项

1. **反射使用**: 避免在运行时使用反射，或使用Source Generator替代
2. **动态代码生成**: 避免使用动态代码生成，如System.Reflection.Emit
3. **资源加载**: 确保所有资源都能在AOT编译时被正确处理
4. **第三方库兼容性**: 确保使用的第三方库支持AOT编译
5. **测试验证**: 在AOT编译后进行充分的测试，确保应用正常运行
6. **序列化**: 使用AOT兼容的序列化库，如MessagePack、Protobuf等
7. **依赖注入**: 确保依赖注入配置正确，避免在运行时动态解析类型
8. **配置管理**: 使用编译时配置绑定，避免运行时配置绑定

## 监控和调试

### 客户端监控

1. **启用详细日志**: 配置MQTT客户端的详细日志记录
2. **监控连接状态**: 监控客户端的连接状态变化
3. **监控消息统计**: 统计发送和接收的消息数量、大小等
4. **监控性能指标**: 监控消息延迟、吞吐量等性能指标
5. **使用调试工具**: 使用MQTT调试工具（如MQTT Explorer）进行调试

### 服务器监控

1. **启用详细日志**: 配置MQTT服务器的详细日志记录
2. **监控连接统计**: 监控客户端连接数量、连接率、断开率等
3. **监控消息统计**: 统计消息的发布、订阅、路由等情况
4. **监控性能指标**: 监控CPU、内存、网络等系统资源使用情况
5. **监控错误和警告**: 监控系统中的错误和警告信息
6. **使用监控工具**: 使用Prometheus、Grafana等工具进行监控

## 部署和运维

### 客户端部署

1. **打包为单一可执行文件**: 使用`PublishSingleFile`选项打包为单一可执行文件
2. **配置文件管理**: 使用外部配置文件或环境变量管理配置
3. **日志管理**: 配置适当的日志级别和日志输出方式
4. **健康检查**: 实现健康检查端点，便于监控客户端状态
5. **自动重启**: 配置自动重启机制，确保客户端持续运行

### 服务器部署

1. **使用容器化部署**: 将MQTT服务器打包为Docker容器
2. **使用编排工具**: 使用Kubernetes等编排工具进行部署和管理
3. **配置负载均衡**: 对于高并发场景，使用负载均衡分布服务器负载
4. **实现高可用**: 配置多节点集群，实现高可用和容错
5. **数据持久化**: 配置适当的数据持久化策略
6. **备份和恢复**: 实现定期备份和灾难恢复机制
7. **安全配置**: 配置适当的安全机制，保护服务器安全
8. **监控和告警**: 配置监控和告警系统，及时发现和处理问题
