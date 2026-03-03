# NATS - 参考文档

## 功能说明

NATS 是一个基于 .NET 10 的高性能消息系统，专为 .NET 开发者设计。它提供了强大的消息发布与订阅功能，支持高性能的消息传递和处理。

## 核心组件

### 1. INatsService (NATS 服务)
- **位置**: scripts/csharp_nats_integration.cs
- **功能**: 核心业务逻辑处理，连接管理和状态监控
- **特性**: 
  - 核心功能实现
  - 性能优化
  - 错误处理
  - 日志记录
  - 连接状态监控

### 2. INatsPublisher (NATS 发布器)
- **位置**: scripts/csharp_nats_integration.cs
- **功能**: 高性能消息发布
- **特性**: 
  - 使用通道进行异步消息处理
  - 内存池优化
  - 批量处理支持
  - 错误重试机制

### 3. INatsSubscriber (NATS 订阅器)
- **位置**: scripts/csharp_nats_integration.cs
- **功能**: 消息订阅和处理
- **特性**: 
  - 高性能消息订阅
  - 消息处理回调
  - 错误处理
  - 订阅管理

## 使用示例

### 基本用法

`csharp
// 获取 NATS 服务
var natsService = serviceProvider.GetRequiredService<INatsService>();
var publisher = serviceProvider.GetRequiredService<INatsPublisher>();
var subscriber = serviceProvider.GetRequiredService<INatsSubscriber>();

// 发布消息
await publisher.PublishAsync("demo.subject", Encoding.UTF8.GetBytes("Hello NATS!"));

// 订阅消息
await subscriber.SubscribeAsync("demo.subject", (subject, message) => {
    Console.WriteLine($"Received message from {subject}: {Encoding.UTF8.GetString(message)}");
});

// 检查连接状态
var isConnected = await natsService.IsConnectedAsync();
Console.WriteLine($"NATS connection status: {isConnected}");
`

### 高级配置

`csharp
// 配置 NATS 选项
builder.Services.Configure<NatsOptions>(options => {
    options.Url = "nats://localhost:4222";
    options.ConnectionTimeout = 10000;
    options.MaxPayloadSize = 2097152; // 2MB
    options.ReconnectWait = 3000;
});

// 注册服务
builder.Services.AddSingleton<INatsService, NatsService>();
builder.Services.AddSingleton<INatsPublisher, NatsPublisher>();
builder.Services.AddSingleton<INatsSubscriber, NatsSubscriber>();

// 获取配置
var settings = serviceProvider.GetRequiredService<IOptions<NatsOptions>>().Value;
Console.WriteLine($"Configuration: Url={settings.Url}, Timeout={settings.ConnectionTimeout}");
`

## 配置选项

### NatsOptions 配置

`json
{
  "NatsOptions": {
    "Url": "nats://localhost:4222",          // NATS 服务器 URL
    "ConnectionTimeout": 5000,            // 连接超时时间（毫秒）
    "MaxPayloadSize": 1048576,            // 最大消息大小（字节）
    "ReconnectWait": 2000                // 重连等待时间（毫秒）
  }
}
`

## 性能优化

1. **使用内存池**: 通过 ArrayPool<byte> 减少内存分配和 GC 压力
2. **异步编程**: 使用异步 API 避免阻塞，提高系统吞吐量
3. **批处理**: 对于大量消息，使用批处理提高效率
4. **连接池**: 合理管理 NATS 连接，避免连接泄漏
5. **通道使用**: 使用 Channel 进行消息缓冲，提高并发处理能力
6. **Span 优化**: 使用 Span<byte> 减少内存拷贝，提高性能

## 故障排除

### 常见问题

1. **连接失败**
   - 检查配置文件中的 URL 是否正确
   - 验证网络连接是否正常
   - 检查 NATS 服务器是否运行
   - 查看日志信息以获取详细错误

2. **性能问题**
   - 启用内存池和通道优化
   - 增加消息批处理大小
   - 检查网络延迟
   - 考虑使用多个 NATS 连接进行负载均衡

3. **消息丢失**
   - 确保 NATS 服务器配置了适当的持久化
   - 使用请求-响应模式确保消息传递
   - 实现消息确认机制

## 扩展开发

### 添加自定义功能

`csharp
public class CustomNatsPublisher : INatsPublisher
{
    private readonly IConnection _connection;
    private readonly ChannelWriter<(string subject, byte[] message)> _channelWriter;
    private readonly Channel<(string subject, byte[] message)> _channel = Channel.CreateBounded<(string, byte[])>(10000);

    public CustomNatsPublisher(NatsOptions options)
    {
        var opts = ConnectionFactory.GetDefaultOptions();
        opts.Url = options.Url;
        _connection = new ConnectionFactory().CreateConnection(opts);
        _channelWriter = _channel.Writer;

        // 后台处理线程
        Task.Run(ProcessMessagesAsync);
    }

    private async Task ProcessMessagesAsync()
    {
        await foreach (var (subject, message) in _channel.Reader.ReadAllAsync())
        {
            try
            {
                // 添加自定义处理逻辑
                Console.WriteLine($"Publishing to {subject}");
                _connection.Publish(subject, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message publish failed: {ex.Message}");
            }
        }
    }

    public ValueTask PublishAsync(string subject, byte[] message) 
        => _channelWriter.WriteAsync((subject, message));

    public void Dispose()
    {
        _channelWriter.Complete();
        _connection?.Dispose();
    }
}
`

### 自定义消息处理器

`csharp
public class CustomMessageHandler
{
    public void HandleMessage(string subject, byte[] message)
    {
        // 自定义消息处理逻辑
        Console.WriteLine($"Custom handler received message from {subject}");
        
        // 处理消息
        var messageString = Encoding.UTF8.GetString(message);
        Console.WriteLine($"Message content: {messageString}");
        
        // 可以进行其他处理，如存储到数据库、调用其他服务等
    }
}

// 使用自定义处理器
var subscriber = serviceProvider.GetRequiredService<INatsSubscriber>();
var handler = new CustomMessageHandler();
await subscriber.SubscribeAsync("demo.subject", handler.HandleMessage);
`

## 部署指南

### 容器化部署

1. **创建 Dockerfile**

`dockerfile
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["csharp_nats_integration.csproj", "."]
RUN dotnet restore "./csharp_nats_integration.csproj"
COPY . .
WORKDIR "/src/.."
RUN dotnet build "csharp_nats_integration.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "csharp_nats_integration.csproj" -c Release -o /app/publish /p:PublishAot=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./csharp_nats_integration"]
`

2. **构建和运行容器**

`bash
docker build -t nats-integration .
docker run --name nats-integration -d nats-integration
`

### 云平台部署

NATS 技能可以部署在各种云平台上，如 Azure、AWS、GCP 等。以下是在 Azure 上部署的示例：

1. **创建 Azure Container Instance**
2. **配置环境变量**：设置 NATS 服务器 URL 和其他配置
3. **部署容器**：使用上述 Docker 镜像
4. **监控和日志**：配置 Azure Monitor 进行监控和日志收集

## 安全最佳实践

1. **使用 TLS 加密**：配置 NATS 服务器使用 TLS 加密连接
2. **认证机制**：使用 NATS 服务器的认证机制，如用户名/密码或令牌
3. **访问控制**：配置基于主题的访问控制，限制消息发布和订阅权限
4. **消息加密**：对于敏感消息，考虑在应用层进行加密
5. **定期轮换凭证**：定期更新认证凭证，提高安全性

## 监控和维护

1. **健康检查**：定期检查 NATS 连接状态
2. **性能监控**：监控消息发布和订阅的延迟和吞吐量
3. **日志管理**：集中管理日志，便于故障排查
4. **告警机制**：设置告警机制，及时发现和处理问题
5. **定期备份**：对于重要消息，定期进行备份

## 版本历史

| 版本 | 日期 | 变更内容 |
|------|------|----------|
| 1.0.0 | 2026-01-24 | 初始版本，基于 .NET 10 |
| 1.0.1 | 2026-02-01 | 性能优化，添加批处理支持 |
| 1.0.2 | 2026-02-15 | 修复连接管理问题，增加重试机制 |
