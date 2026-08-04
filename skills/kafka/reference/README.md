# Kafka 技能技术参考文档

## 1. 技术架构

### 1.1 系统架构

Kafka 技能采用分层架构设计，主要包含以下层次：

| 层次 | 组件 | 职责 |
|------|------|------|
| 命令行接口层 | Program 类 | 处理命令行参数，解析命令，调用相应的服务方法 |
| 服务层 | KafkaService 类 | 封装核心 Kafka 操作，提供异步 API |
| 核心客户端层 | Confluent.Kafka 库 | 提供 Kafka 客户端功能，如生产者、消费者、管理客户端 |
| 基础设施层 | .NET 10.0 | 提供运行时环境、依赖注入、缓存、日志等基础设施 |

### 1.2 核心组件

#### 1.2.1 Program 类

**职责**：作为应用程序入口，处理命令行参数，解析用户命令，并调用相应的服务方法。

**主要功能**：
- 命令行参数解析
- 命令路由
- 依赖注入容器初始化
- 日志配置
- 错误处理

#### 1.2.2 KafkaService 类

**职责**：封装 Confluent.Kafka 的核心功能，提供异步 API，支持各种 Kafka 操作。

**主要功能**：
- 消息发送（单条和批量）
- 消息消费
- 主题管理（列出、创建、删除、描述）
- 性能测试
- 错误处理

#### 1.2.3 Confluent.Kafka 库

**职责**：提供核心的 Kafka 客户端功能。

**主要组件**：
- `ProducerBuilder`：用于创建 Kafka 生产者
- `ConsumerBuilder`：用于创建 Kafka 消费者
- `AdminClientBuilder`：用于创建 Kafka 管理客户端
- `ProducerConfig`：生产者配置
- `ConsumerConfig`：消费者配置
- `AdminClientConfig`：管理客户端配置

## 2. API 参考

### 2.1 KafkaService 类

#### 2.1.1 ProduceMessageAsync 方法

**功能**：发送单条消息到 Kafka 主题

**签名**：
```csharp
public async Task ProduceMessageAsync(string broker, string topic, string message)
```

**参数**：
- `broker`：Kafka broker 地址，格式为 `host:port`
- `topic`：消息主题名称
- `message`：要发送的消息内容

**返回值**：
- 无

**示例**：
```csharp
await kafkaService.ProduceMessageAsync("localhost:9092", "test-topic", "Hello, Kafka!");
```

#### 2.1.2 ConsumeMessagesAsync 方法

**功能**：从 Kafka 主题消费指定数量的消息

**签名**：
```csharp
public async Task ConsumeMessagesAsync(string broker, string topic, string groupId, int count)
```

**参数**：
- `broker`：Kafka broker 地址，格式为 `host:port`
- `topic`：消息主题名称
- `groupId`：消费者组 ID
- `count`：要消费的消息数量

**返回值**：
- 无

**示例**：
```csharp
await kafkaService.ConsumeMessagesAsync("localhost:9092", "test-topic", "test-group", 10);
```

#### 2.1.3 ListTopicsAsync 方法

**功能**：列出所有 Kafka 主题

**签名**：
```csharp
public async Task ListTopicsAsync(string broker)
```

**参数**：
- `broker`：Kafka broker 地址，格式为 `host:port`

**返回值**：
- 无

**示例**：
```csharp
await kafkaService.ListTopicsAsync("localhost:9092");
```

#### 2.1.4 CreateTopicAsync 方法

**功能**：创建新的 Kafka 主题

**签名**：
```csharp
public async Task CreateTopicAsync(string broker, string topic, int partitions, short replicationFactor)
```

**参数**：
- `broker`：Kafka broker 地址，格式为 `host:port`
- `topic`：主题名称
- `partitions`：分区数量
- `replicationFactor`：副本因子

**返回值**：
- 无

**示例**：
```csharp
await kafkaService.CreateTopicAsync("localhost:9092", "new-topic", 3, 1);
```

#### 2.1.5 DeleteTopicAsync 方法

**功能**：删除指定的 Kafka 主题

**签名**：
```csharp
public async Task DeleteTopicAsync(string broker, string topic)
```

**参数**：
- `broker`：Kafka broker 地址，格式为 `host:port`
- `topic`：主题名称

**返回值**：
- 无

**示例**：
```csharp
await kafkaService.DeleteTopicAsync("localhost:9092", "old-topic");
```

#### 2.1.6 DescribeTopicAsync 方法

**功能**：描述指定 Kafka 主题的详细信息

**签名**：
```csharp
public async Task DescribeTopicAsync(string broker, string topic)
```

**参数**：
- `broker`：Kafka broker 地址，格式为 `host:port`
- `topic`：主题名称

**返回值**：
- 无

**示例**：
```csharp
await kafkaService.DescribeTopicAsync("localhost:9092", "test-topic");
```

#### 2.1.7 BatchProduceAsync 方法

**功能**：批量发送消息到 Kafka 主题

**签名**：
```csharp
public async Task BatchProduceAsync(string broker, string topic, int messageCount, string messagePrefix)
```

**参数**：
- `broker`：Kafka broker 地址，格式为 `host:port`
- `topic`：消息主题名称
- `messageCount`：要发送的消息数量
- `messagePrefix`：消息前缀

**返回值**：
- 无

**示例**：
```csharp
await kafkaService.BatchProduceAsync("localhost:9092", "test-topic", 100, "Test message");
```

#### 2.1.8 RunBenchmarkAsync 方法

**功能**：运行 Kafka 性能基准测试

**签名**：
```csharp
public async Task RunBenchmarkAsync(string broker, string topic, int messageCount)
```

**参数**：
- `broker`：Kafka broker 地址，格式为 `host:port`
- `topic`：消息主题名称
- `messageCount`：要测试的消息数量

**返回值**：
- 无

**示例**：
```csharp
await kafkaService.RunBenchmarkAsync("localhost:9092", "test-topic", 1000);
```

## 3. 配置选项

### 3.1 环境变量

| 环境变量 | 类型 | 默认值 | 描述 |
|---------|------|--------|------|
| `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT` | 布尔值 | `false` | 是否启用全球化不变模式 |
| `KAFKA_CLIENT_ID` | 字符串 | `kafka_aot_client` | Kafka 客户端 ID |
| `KAFKA_PRODUCER_ACKS` | 字符串 | `1` | 生产者确认级别 |
| `KAFKA_CONSUMER_AUTO_OFFSET_RESET` | 字符串 | `earliest` | 消费者自动偏移重置策略 |
| `KAFKA_PRODUCER_LINGER_MS` | 整数 | 10 | 生产者 linger 时间（毫秒） |
| `KAFKA_PRODUCER_BATCH_SIZE` | 整数 | 16384 | 生产者批量大小 |
| `KAFKA_CONSUMER_ENABLE_AUTO_COMMIT` | 字符串 | `true` | 消费者是否启用自动提交 |
| `KAFKA_CONSUMER_FETCH_MIN_BYTES` | 整数 | 1 | 消费者最小拉取字节数 |
| `KAFKA_CONSUMER_FETCH_MAX_WAIT_MS` | 整数 | 500 | 消费者最大等待时间（毫秒） |
| `KAFKA_BATCH_SIZE` | 整数 | 100 | 批量操作的批次大小 |

### 3.2 运行时配置

| 配置项 | 类型 | 默认值 | 描述 |
|--------|------|--------|------|
| `runtime.framework` | 字符串 | `net10.0` | 运行时框架版本 |
| `runtime.aot` | 布尔值 | `true` | 是否启用 AOT 编译 |
| `runtime.selfContained` | 布尔值 | `true` | 是否为自包含部署 |
| `runtime.runtimeIdentifier` | 字符串 | `win-x64` | 运行时标识符 |
| `runtime.optimizationLevel` | 字符串 | `Release` | 优化级别 |
| `memory.initial` | 整数 | 64 | 初始内存大小（MB） |
| `memory.maximum` | 整数 | 512 | 最大内存大小（MB） |
| `timeouts.command` | 整数 | 60000 | 命令超时时间（毫秒） |
| `timeouts.produce` | 整数 | 30000 | 生产者超时时间（毫秒） |
| `timeouts.consume` | 整数 | 60000 | 消费者超时时间（毫秒） |
| `timeouts.adminOperation` | 整数 | 30000 | 管理操作超时时间（毫秒） |
| `timeouts.batchOperation` | 整数 | 120000 | 批量操作超时时间（毫秒） |
| `timeouts.benchmark` | 整数 | 300000 | 性能测试超时时间（毫秒） |

## 4. 性能优化

### 4.1 生产者优化

1. **批量发送**：使用 `BatchProduceAsync` 方法批量发送消息，减少网络往返
2. **调整 linger.ms**：适当增加 `KAFKA_PRODUCER_LINGER_MS`，增加批量大小
3. **调整 batch.size**：根据消息大小调整 `KAFKA_PRODUCER_BATCH_SIZE`
4. **异步发送**：使用异步 API，避免同步阻塞
5. **合理的确认级别**：根据可靠性要求选择合适的 `KAFKA_PRODUCER_ACKS` 值

### 4.2 消费者优化

1. **批量消费**：增加 `KAFKA_CONSUMER_FETCH_MIN_BYTES`，减少网络往返
2. **调整 fetch.max.wait.ms**：根据业务需求调整 `KAFKA_CONSUMER_FETCH_MAX_WAIT_MS`
3. **并行消费**：使用多个消费者实例，提高消费速度
4. **自动提交**：启用 `KAFKA_CONSUMER_ENABLE_AUTO_COMMIT`，简化代码
5. **合理的偏移重置策略**：根据业务需求选择合适的 `KAFKA_CONSUMER_AUTO_OFFSET_RESET` 值

### 4.3 系统优化

1. **内存管理**：根据实际情况调整内存限制
2. **网络优化**：确保网络连接稳定，减少网络延迟
3. **错误处理**：实现合理的错误重试机制
4. **资源释放**：使用 `using` 语句确保资源正确释放
5. **日志级别**：根据环境调整日志级别，减少日志开销

## 5. 错误处理

### 5.1 异常类型

| 异常类型 | 描述 | 处理方式 |
|---------|------|---------|
| `ProduceException` | 消息发送失败 | 记录错误日志，根据需要重试 |
| `ConsumeException` | 消息消费失败 | 记录错误日志，继续消费下一条消息 |
| `AdminException` | 管理操作失败 | 记录错误日志，返回错误信息 |
| `KafkaException` | Kafka 客户端异常 | 记录错误日志，返回错误信息 |
| `ArgumentException` | 参数无效 | 记录错误日志，显示帮助信息 |
| `IOException` | IO 异常 | 记录错误日志，返回错误信息 |
| `Exception` | 其他异常 | 记录错误日志，返回错误信息 |

### 5.2 错误处理策略

1. **命令行参数验证**：在执行命令前验证参数是否有效
2. **网络连接检查**：在执行操作前检查网络连接是否正常
3. **异常捕获**：使用 try-catch 捕获并处理异常
4. **错误日志**：记录详细的错误信息，便于调试
5. **用户友好的错误信息**：向用户显示清晰、易懂的错误信息
6. **重试机制**：对于网络临时性错误，实现合理的重试机制

## 6. 部署与发布

### 6.1 AOT 编译

Kafka 技能使用 .NET 10.0 的 AOT 编译功能，生成自包含的可执行文件，无需安装 .NET 运行时。

**编译命令**：
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishAot=true
```

### 6.2 发布配置

| 配置项 | 值 | 描述 |
|--------|-----|------|
| `TargetFramework` | `net10.0` | 目标框架版本 |
| `PublishAot` | `true` | 启用 AOT 编译 |
| `SelfContained` | `true` | 自包含部署 |
| `RuntimeIdentifier` | `win-x64` | 运行时标识符 |
| `OptimizationLevel` | `Release` | 优化级别 |
| `LangVersion` | `preview` | C# 语言版本 |
| `Nullable` | `enable` | 启用可空引用类型 |
| `ImplicitUsings` | `enable` | 启用隐式 using |

### 6.3 跨平台支持

| 平台 | 运行时标识符 | 支持状态 |
|------|-------------|----------|
| Windows x64 | `win-x64` | 完全支持 |
| Windows x86 | `win-x86` | 支持 |
| Linux x64 | `linux-x64` | 支持 |
| Linux ARM64 | `linux-arm64` | 支持 |
| macOS x64 | `osx-x64` | 支持 |
| macOS ARM64 | `osx-arm64` | 支持 |

## 7. 监控与日志

### 7.1 日志配置

Kafka 技能使用 `Microsoft.Extensions.Logging` 进行日志记录，默认配置为控制台输出。

**日志级别**：
- `Information`：普通信息，如操作结果、进度信息等
- `Warning`：警告信息，如参数不完整、配置项缺失等
- `Error`：错误信息，如网络错误、Kafka 操作失败等
- `Critical`：严重错误，如系统崩溃、资源耗尽等

### 7.2 监控指标

| 指标名称 | 描述 | 单位 |
|---------|------|------|
| 消息发送成功率 | 成功发送的消息数 / 总发送消息数 | % |
| 消息消费成功率 | 成功消费的消息数 / 总消费消息数 | % |
| 消息发送延迟 | 消息发送的平均延迟 | ms |
| 消息消费延迟 | 消息消费的平均延迟 | ms |
| 主题操作成功率 | 成功的主题操作数 / 总主题操作数 | % |
| 系统资源使用率 | CPU、内存、网络的使用率 | % |
| 错误率 | 错误操作数 / 总操作数 | % |

## 8. 扩展性

### 8.1 功能扩展

可以通过以下方式扩展 Kafka 技能的功能：

1. **添加新命令**：在 `Program.Main` 方法中添加新的命令处理逻辑
2. **扩展 KafkaService**：在 `KafkaService` 类中添加新的方法
3. **集成其他 Kafka 功能**：可以集成 Kafka Streams、Kafka Connect 等功能
4. **自定义序列化**：实现自定义的消息序列化和反序列化
5. **添加监控功能**：集成监控系统，如 Prometheus、Grafana 等

### 8.2 配置扩展

1. **添加新的环境变量**：在 `kafka_aot.run.json` 中添加新的环境变量配置
2. **添加新的运行时配置**：在 `kafka_aot.run.json` 中添加新的运行时配置项
3. **添加新的命令配置**：在 `kafka_aot.setting.json` 中添加新的命令配置

### 8.3 集成扩展

1. **与其他系统集成**：可以与其他系统集成，如日志系统、监控系统等
2. **与云服务集成**：可以与云服务集成，如 AWS MSK、Azure Event Hubs 等
3. **与容器平台集成**：可以与 Docker、Kubernetes 等容器平台集成

## 9. 最佳实践

### 9.1 生产环境最佳实践

1. **配置优化**：根据生产环境的实际情况优化配置参数
2. **监控设置**：设置合理的监控指标和告警
3. **错误处理**：实现完善的错误处理和重试机制
4. **资源管理**：合理配置系统资源，避免资源耗尽
5. **安全设置**：配置 Kafka 的安全设置，如 SSL、SASL 等
6. **备份策略**：制定合理的消息备份策略
7. **容量规划**：根据业务需求进行合理的容量规划
8. **版本管理**：使用稳定的 Kafka 版本，定期升级

### 9.2 开发环境最佳实践

1. **本地 Kafka 集群**：使用 Docker 运行本地 Kafka 集群
2. **测试数据**：准备充足的测试数据
3. **自动化测试**：编写自动化测试脚本
4. **代码规范**：遵循 C# 代码规范
5. **文档更新**：及时更新文档，保持文档与代码同步
6. **版本控制**：使用版本控制系统管理代码
7. **代码审查**：进行代码审查，确保代码质量

### 9.3 使用最佳实践

1. **命令行使用**：
   - 使用命令别名，如 `p` 代替 `produce`
   - 对于长消息，使用文件输入而不是命令行参数

2. **批量操作**：
   - 对于大量消息，使用批量操作，提高效率
   - 合理设置批量大小，避免内存使用过大

3. **性能测试**：
   - 在测试环境进行性能测试，避免影响生产环境
   - 合理设置测试消息数量，避免对 Kafka 集群造成压力

4. **错误处理**：
   - 注意捕获和处理异常
   - 查看详细的错误日志，了解错误原因
   - 根据错误类型采取相应的处理措施

## 10. 技术栈

| 技术/库 | 版本 | 用途 |
|---------|------|------|
| C# | 10.0 | 开发语言 |
| .NET | 10.0 | 运行时框架 |
| Confluent.Kafka | 2.3.0 | Kafka 客户端库 |
| Microsoft.Extensions.DependencyInjection | 10.0.0 | 依赖注入容器 |
| Microsoft.Extensions.Caching.Memory | 10.0.0 | 内存缓存 |
| Microsoft.Extensions.Logging | 10.0.0 | 日志记录 |
| Microsoft.Extensions.Options | 10.0.0 | 配置管理 |

## 11. 常见问题与解决方案

### 11.1 连接问题

**问题**：无法连接到 Kafka 集群

**解决方案**：
- 检查 Kafka 集群是否运行正常
- 检查 broker 地址是否正确
- 检查网络连接是否正常
- 检查防火墙设置是否允许连接
- 检查 Kafka 集群的安全设置

### 11.2 消息发送问题

**问题**：消息发送失败

**解决方案**：
- 检查主题是否存在
- 检查生产者配置是否正确
- 检查网络连接是否稳定
- 检查 Kafka 集群是否有足够的存储空间
- 检查消息大小是否超过 Kafka 的限制

### 11.3 消息消费问题

**问题**：消息消费不到或消费失败

**解决方案**：
- 检查主题是否存在
- 检查消费者组 ID 是否正确
- 检查偏移量重置策略是否合适
- 确认主题中是否有消息
- 检查消费者配置是否正确

### 11.4 主题管理问题

**问题**：主题创建/删除/描述失败

**解决方案**：
- 检查是否有足够的权限执行操作
- 检查 Kafka 集群配置是否允许主题管理操作
- 检查分区数和副本因子是否合理
- 检查网络连接是否稳定

### 11.5 性能问题

**问题**：操作速度慢

**解决方案**：
- 优化生产者和消费者配置
- 增加批量大小，减少网络往返
- 提高网络带宽和稳定性
- 增加系统资源（CPU、内存）
- 优化 Kafka 集群配置

### 11.6 错误处理问题

**问题**：错误信息不明确

**解决方案**：
- 查看详细的错误日志
- 检查网络连接和 Kafka 集群状态
- 验证命令参数是否正确
- 检查配置项是否完整

## 12. 版本历史

| 版本 | 日期 | 变更内容 |
|------|------|----------|
| 1.0.0 | 2026-01-21 | 初始版本，支持基本的 Kafka 操作、主题管理、批量操作和性能测试 |

## 13. 参考资料

1. [Confluent.Kafka 官方文档](https://docs.confluent.io/kafka-clients/dotnet/current/overview.html)
2. [Apache Kafka 官方文档](https://kafka.apache.org/documentation/)
3. [.NET 10.0 官方文档](https://learn.microsoft.com/zh-cn/dotnet/)
4. [Microsoft.Extensions.DependencyInjection 文档](https://learn.microsoft.com/zh-cn/dotnet/core/extensions/dependency-injection)
5. [Microsoft.Extensions.Logging 文档](https://learn.microsoft.com/zh-cn/dotnet/core/extensions/logging)
6. [Kafka 性能调优指南](https://docs.confluent.io/platform/current/installation/performance.html)
7. [Kafka 最佳实践](https://developer.confluent.io/learn/kafka-best-practices/)

## 14. 附录

### 14.1 Kafka 配置参考

| 配置项 | 描述 | 默认值 | 推荐值 |
|--------|------|--------|--------|
| `bootstrap.servers` | Kafka broker 地址列表 | - | 生产环境中设置所有 broker 地址 |
| `client.id` | 客户端 ID | - | 唯一标识客户端的字符串 |
| `acks` | 生产者确认级别 | 1 | 0/1/all |
| `retries` | 生产者重试次数 | 2147483647 | 3 |
| `linger.ms` | 生产者 linger 时间 | 0 | 10-100 |
| `batch.size` | 生产者批量大小 | 16384 | 16384-65536 |
| `buffer.memory` | 生产者缓冲区大小 | 33554432 | 33554432 |
| `auto.offset.reset` | 消费者偏移重置策略 | latest | earliest/latest |
| `enable.auto.commit` | 消费者自动提交 | true | true/false |
| `auto.commit.interval.ms` | 消费者自动提交间隔 | 5000 | 1000-5000 |
| `fetch.min.bytes` | 消费者最小拉取字节数 | 1 | 1-1024 |
| `fetch.max.wait.ms` | 消费者最大等待时间 | 500 | 100-500 |
| `max.poll.records` | 消费者最大拉取记录数 | 500 | 100-1000 |

### 14.2 命令行参数表

| 命令 | 别名 | 参数 | 描述 |
|------|------|------|------|
| `produce` | `p` | `<broker> <topic> <message>` | 发送消息到 Kafka 主题 |
| `consume` | `c` | `<broker> <topic> <group> [count]` | 从 Kafka 主题消费消息 |
| `list-topics` | `lt` | `<broker>` | 列出所有 Kafka 主题 |
| `create-topic` | `ct` | `<broker> <topic> <partitions> [replication-factor]` | 创建新的 Kafka 主题 |
| `delete-topic` | `dt` | `<broker> <topic>` | 删除 Kafka 主题 |
| `describe-topic` | `dtp` | `<broker> <topic>` | 描述 Kafka 主题的详细信息 |
| `batch-produce` | `bp` | `<broker> <topic> <count> <prefix>` | 批量发送消息到 Kafka 主题 |
| `benchmark` | `bm` | `<broker> <topic> <count>` | 运行 Kafka 性能基准测试 |
| `help` | `h` | 无 | 显示帮助信息 |

### 14.3 错误代码参考

| 错误代码 | 描述 | 可能的原因 |
|---------|------|----------|
| 0 | 成功 | 操作成功完成 |
| 1 | 参数错误 | 命令参数无效或缺失 |
| 2 | 网络错误 | 网络连接失败或超时 |
| 3 | Kafka 错误 | Kafka 集群返回错误 |
| 4 | 权限错误 | 没有足够的权限执行操作 |
| 5 | 资源错误 | 系统资源不足 |
| 6 | 配置错误 | 配置项无效或缺失 |
| 7 | 超时错误 | 操作超时 |
| 8 | 其他错误 | 其他未分类的错误 |
