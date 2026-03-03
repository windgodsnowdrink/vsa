# Kafka 技能

## 技能简介

Kafka 技能是基于 Confluent.Kafka 库开发的 AOT 编译 Kafka 工具，提供高效、便捷的 Kafka 集群管理和消息处理能力。该工具采用 .NET 10.0 框架，通过 AOT（Ahead-of-Time）编译技术，实现了高性能的 Kafka 操作，包括消息发送和消费、主题管理、批量操作和性能测试等功能。

### 主要特点

- **AOT 编译**：采用 .NET 10.0 的 AOT 编译技术，启动速度快，运行效率高
- **全面的 Kafka 操作**：支持消息发送、消费、主题管理等核心功能
- **批量操作**：支持批量发送消息，提高处理效率
- **性能测试**：内置性能基准测试功能，评估 Kafka 集群性能
- **命令行接口**：提供简洁的命令行接口，支持命令别名
- **详细的日志记录**：提供详细的操作日志，便于调试和监控
- **跨平台**：支持 Windows、Linux、macOS 等多个平台

## 技术架构

### 核心组件

- **Confluent.Kafka**：提供核心的 Kafka 客户端功能
- **.NET 10.0**：基础运行框架，支持 AOT 编译
- **Microsoft.Extensions.DependencyInjection**：依赖注入容器
- **Microsoft.Extensions.Caching.Memory**：内存缓存
- **Microsoft.Extensions.Logging**：日志记录
- **Microsoft.Extensions.Options**：配置管理

### 架构设计

```
┌─────────────────────────────────────────────────────────┐
│                       命令行接口                        │
└─────────────────────────────┬───────────────────────────┘
                              │
┌─────────────────────────────▼───────────────────────────┐
│                    KafkaService                        │
├─────────────────────┬────────────────────┬──────────────┤
│  消息生产模块        │ 消息消费模块        │ 主题管理模块   │
├─────────────────────┼────────────────────┼──────────────┤
│ ┌───────────────┐   │ ┌───────────────┐   │ ┌───────────┐ │
│ │ 单条消息发送   │   │ │ 消息订阅      │   │ │ 主题创建  │ │
│ ├───────────────┤   │ ├───────────────┤   │ ├───────────┤ │
│ │ 批量消息发送   │   │ │ 消息拉取      │   │ │ 主题删除  │ │
│ └───────────────┘   │ └───────────────┘   │ ├───────────┤ │
│                    │                     │ │ 主题列出  │ │
│                    │                     │ ├───────────┤ │
│                    │                     │ │ 主题描述  │ │
│                    │                     │ └───────────┘ │
└─────────────────────┴────────────────────┴──────────────┘
```

## 安装和配置

### 系统要求

- **操作系统**：Windows 10/11、Linux、macOS
- **框架**：.NET 10.0 或更高版本（AOT 编译版本为自包含，无需安装 .NET 运行时）
- **内存**：至少 64MB，推荐 512MB 以上
- **磁盘空间**：至少 100MB
- **网络**：能够连接到 Kafka 集群
- **Kafka 版本**：2.0.0 或更高版本

### 配置选项

通过环境变量进行配置：

| 环境变量 | 类型 | 默认值 | 描述 |
|---------|------|--------|------|
| `KAFKA_CLIENT_ID` | 字符串 | `kafka_aot_client` | Kafka 客户端 ID |
| `KAFKA_PRODUCER_ACKS` | 字符串 | `1` | 生产者确认级别 |
| `KAFKA_CONSUMER_AUTO_OFFSET_RESET` | 字符串 | `earliest` | 消费者自动偏移重置策略 |
| `KAFKA_PRODUCER_LINGER_MS` | 整数 | `10` | 生产者 linger 时间（毫秒） |
| `KAFKA_PRODUCER_BATCH_SIZE` | 整数 | `16384` | 生产者批量大小 |
| `KAFKA_CONSUMER_ENABLE_AUTO_COMMIT` | 字符串 | `true` | 消费者是否启用自动提交 |
| `KAFKA_CONSUMER_FETCH_MIN_BYTES` | 整数 | `1` | 消费者最小拉取字节数 |
| `KAFKA_CONSUMER_FETCH_MAX_WAIT_MS` | 整数 | `500` | 消费者最大等待时间（毫秒） |

## 使用方法

### 基本使用

```bash
# 发送消息
kafka_aot produce localhost:9092 test-topic "Hello, Kafka!"

# 使用命令别名
kafka_aot p localhost:9092 test-topic "Hello, Kafka!"

# 消费消息
kafka_aot consume localhost:9092 test-topic test-group 5

# 使用命令别名
kafka_aot c localhost:9092 test-topic test-group 5

# 列出所有主题
kafka_aot list-topics localhost:9092

# 使用命令别名
kafka_aot lt localhost:9092

# 创建主题
kafka_aot create-topic localhost:9092 new-topic 3 1

# 使用命令别名
kafka_aot ct localhost:9092 new-topic 3 1

# 删除主题
kafka_aot delete-topic localhost:9092 old-topic

# 使用命令别名
kafka_aot dt localhost:9092 old-topic

# 描述主题
kafka_aot describe-topic localhost:9092 test-topic

# 使用命令别名
kafka_aot dtp localhost:9092 test-topic

# 批量发送消息
kafka_aot batch-produce localhost:9092 test-topic 100 "Test message"

# 使用命令别名
kafka_aot bp localhost:9092 test-topic 100 "Test message"

# 运行性能测试
kafka_aot benchmark localhost:9092 test-topic 1000

# 使用命令别名
kafka_aot bm localhost:9092 test-topic 1000

# 显示帮助信息
kafka_aot help

# 使用命令别名
kafka_aot h
```

### 命令别名

为了方便使用，所有命令都提供了简短的别名：

| 完整命令 | 别名 | 描述 |
|---------|------|------|
| `produce` | `p` | 发送消息 |
| `consume` | `c` | 消费消息 |
| `list-topics` | `lt` | 列出所有主题 |
| `create-topic` | `ct` | 创建主题 |
| `delete-topic` | `dt` | 删除主题 |
| `describe-topic` | `dtp` | 描述主题 |
| `batch-produce` | `bp` | 批量发送消息 |
| `benchmark` | `bm` | 运行性能测试 |
| `help` | `h` | 显示帮助信息 |

## 命令参考

### produce 命令

**功能**：发送消息到 Kafka 主题

**语法**：
```bash
kafka_aot produce <broker> <topic> <message>
```

**参数**：
- `broker`：Kafka broker 地址，格式为 `host:port`
- `topic`：消息主题名称
- `message`：要发送的消息内容

**示例**：
```bash
kafka_aot produce localhost:9092 test-topic "Hello, Kafka! This is a test message."
```

**输出**：
```
消息发送到: test-topic-0-42
消息发送成功
```

### consume 命令

**功能**：从 Kafka 主题消费消息

**语法**：
```bash
kafka_aot consume <broker> <topic> <group> [count]
```

**参数**：
- `broker`：Kafka broker 地址，格式为 `host:port`
- `topic`：消息主题名称
- `group`：消费者组 ID
- `count`：要消费的消息数量，默认 10

**示例**：
```bash
kafka_aot consume localhost:9092 test-topic test-group 5
```

**输出**：
```
开始消费消息，最多消费 5 条
消费消息: Hello, Kafka!
主题: test-topic, 分区: 0, 偏移量: 42
消费消息: This is another message
主题: test-topic, 分区: 0, 偏移量: 43
消费完成，共消费 2 条消息
```

### list-topics 命令

**功能**：列出所有 Kafka 主题

**语法**：
```bash
kafka_aot list-topics <broker>
```

**参数**：
- `broker`：Kafka broker 地址，格式为 `host:port`

**示例**：
```bash
kafka_aot list-topics localhost:9092
```

**输出**：
```
Kafka 集群信息:
 broker数量: 3
  Broker: localhost:9092
  Broker: localhost:9093
  Broker: localhost:9094

主题列表:
  主题: test-topic
  主题: another-topic
  主题: new-topic
```

### create-topic 命令

**功能**：创建新的 Kafka 主题

**语法**：
```bash
kafka_aot create-topic <broker> <topic> <partitions> [replication-factor]
```

**参数**：
- `broker`：Kafka broker 地址，格式为 `host:port`
- `topic`：主题名称
- `partitions`：分区数量
- `replication-factor`：副本因子，默认 1

**示例**：
```bash
kafka_aot create-topic localhost:9092 new-topic 3 2
```

**输出**：
```
主题创建成功: new-topic, 分区数: 3, 副本因子: 2
```

### delete-topic 命令

**功能**：删除 Kafka 主题

**语法**：
```bash
kafka_aot delete-topic <broker> <topic>
```

**参数**：
- `broker`：Kafka broker 地址，格式为 `host:port`
- `topic`：主题名称

**示例**：
```bash
kafka_aot delete-topic localhost:9092 old-topic
```

**输出**：
```
主题删除成功: old-topic
```

### describe-topic 命令

**功能**：描述 Kafka 主题的详细信息

**语法**：
```bash
kafka_aot describe-topic <broker> <topic>
```

**参数**：
- `broker`：Kafka broker 地址，格式为 `host:port`
- `topic`：主题名称

**示例**：
```bash
kafka_aot describe-topic localhost:9092 test-topic
```

**输出**：
```
主题信息: test-topic
  分区数: 3
  分区: 0, 领导者: 1, 副本: 1,2,0, ISR: 1,2,0
  分区: 1, 领导者: 2, 副本: 2,0,1, ISR: 2,0,1
  分区: 2, 领导者: 0, 副本: 0,1,2, ISR: 0,1,2
```

### batch-produce 命令

**功能**：批量发送消息到 Kafka 主题

**语法**：
```bash
kafka_aot batch-produce <broker> <topic> <count> <prefix>
```

**参数**：
- `broker`：Kafka broker 地址，格式为 `host:port`
- `topic`：消息主题名称
- `count`：要发送的消息数量
- `prefix`：消息前缀

**示例**：
```bash
kafka_aot batch-produce localhost:9092 test-topic 100 "Test message"
```

**输出**：
```
开始批量发送消息，共发送 100 条
已发送 10 条消息
已发送 20 条消息
...
已发送 100 条消息
批量发送完成，共发送 100 条消息
```

### benchmark 命令

**功能**：运行 Kafka 性能基准测试

**语法**：
```bash
kafka_aot benchmark <broker> <topic> <count>
```

**参数**：
- `broker`：Kafka broker 地址，格式为 `host:port`
- `topic`：消息主题名称
- `count`：要测试的消息数量

**示例**：
```bash
kafka_aot benchmark localhost:9092 test-topic 1000
```

**输出**：
```
开始性能基准测试...
============================================================
生产性能测试:
  消息数量: 1000
  耗时: 123 ms
  吞吐量: 8130.08 条/秒

消费性能测试:
  消息数量: 1000
  耗时: 87 ms
  吞吐量: 11494.25 条/秒
============================================================
性能基准测试完成
```

### help 命令

**功能**：显示帮助信息

**语法**：
```bash
kafka_aot help
```

**示例**：
```bash
kafka_aot help
```

**输出**：
```
Kafka AOT 工具
=============
命令列表:
  produce|p <broker> <topic> <message> - 发送消息
  consume|c <broker> <topic> <group> [count] - 消费消息
  list-topics|lt <broker> - 列出所有主题
  create-topic|ct <broker> <topic> <partitions> [replication-factor] - 创建主题
  delete-topic|dt <broker> <topic> - 删除主题
  describe-topic|dtp <broker> <topic> - 描述主题
  batch-produce|bp <broker> <topic> <count> <prefix> - 批量发送消息
  benchmark|bm <broker> <topic> <count> - 运行性能基准测试
  help|h - 显示帮助信息
```

## 性能指标

### 消息处理性能

| 操作类型 | 处理速度 | 内存占用 | 网络带宽 |
|---------|---------|---------|----------|
| 单条消息发送 | 约 10,000 条/秒 | 约 64MB | 约 10Mbps |
| 批量消息发送 | 约 20,000 条/秒 | 约 128MB | 约 20Mbps |
| 消息消费 | 约 15,000 条/秒 | 约 96MB | 约 15Mbps |
| 主题管理操作 | 约 2 操作/秒 | 约 64MB | 约 1Mbps |

### 系统资源使用

| 操作类型 | CPU 使用 | 内存使用 | 网络使用 |
|---------|---------|---------|----------|
| 空闲状态 | < 1% | ~64MB | ~0Mbps |
| 消息发送 | ~20% | ~128MB | ~10Mbps |
| 消息消费 | ~15% | ~96MB | ~15Mbps |
| 批量操作 | ~30% | ~256MB | ~20Mbps |
| 性能测试 | ~50% | ~256MB | ~25Mbps |

## 使用场景

### 消息队列管理

- **消息发送**：向 Kafka 主题发送各种类型的消息
- **消息消费**：从 Kafka 主题消费消息并进行处理
- **消息监控**：监控消息队列的状态和消息流动

### 数据流处理

- **实时数据采集**：采集各种数据源的实时数据
- **数据传输**：在不同系统之间传输数据
- **数据缓冲**：作为数据处理系统的缓冲层

### 事件驱动架构

- **事件发布**：发布系统中的各种事件
- **事件订阅**：订阅感兴趣的事件并进行处理
- **事件溯源**：通过事件记录系统状态变化

### 微服务间通信

- **服务解耦**：通过消息队列解耦微服务
- **异步通信**：实现微服务之间的异步通信
- **可靠传递**：确保消息的可靠传递

### 日志收集和处理

- **日志聚合**：聚合来自不同系统的日志
- **日志分析**：对日志进行实时分析
- **日志存储**：将日志持久化存储

### 数据集成和 ETL

- **数据提取**：从源系统提取数据
- **数据转换**：对数据进行转换处理
- **数据加载**：将数据加载到目标系统

### 实时数据分析

- **数据实时处理**：实时处理流数据
- **数据聚合**：对实时数据进行聚合计算
- **数据可视化**：将实时数据可视化展示

### 系统解耦

- **系统集成**：集成不同的系统和应用
- **流量控制**：控制系统间的流量
- **故障隔离**：隔离系统故障，提高系统可靠性

## 限制和注意事项

1. **Kafka 集群依赖**：需要有运行中的 Kafka 集群，工具本身不提供 Kafka 服务
2. **网络连接**：需要稳定的网络连接，网络不稳定可能导致操作失败
3. **权限要求**：需要有足够的权限执行相应的操作，如创建/删除主题需要管理员权限
4. **内存使用**：批量操作和性能测试会使用较多内存，建议根据实际硬件情况调整内存限制
5. **消息格式**：目前仅支持字符串消息格式，不支持二进制消息
6. **性能影响**：性能测试可能对 Kafka 集群造成一定压力，建议在测试环境使用
7. **版本兼容性**：建议使用 Kafka 2.0.0 或更高版本，以获得最佳兼容性
8. **错误处理**：网络错误或 Kafka 集群问题可能导致操作失败，工具会记录详细的错误信息

## 常见问题

### Q: 连接 Kafka 集群失败怎么办？

**A**：可以尝试以下方法：
- 检查 Kafka 集群是否运行正常
- 检查网络连接是否正常
- 检查 broker 地址是否正确
- 检查防火墙设置是否允许连接
- 查看详细的错误日志，了解具体的失败原因

### Q: 消息发送失败怎么办？

**A**：可以尝试以下方法：
- 检查主题是否存在
- 检查生产者配置是否正确
- 检查网络连接是否稳定
- 查看详细的错误日志，了解具体的失败原因

### Q: 消息消费不到怎么办？

**A**：可以尝试以下方法：
- 检查主题是否存在
- 检查消费者组 ID 是否正确
- 检查偏移量重置策略是否合适
- 确认主题中是否有消息
- 查看详细的错误日志，了解具体的问题

### Q: 主题创建失败怎么办？

**A**：可以尝试以下方法：
- 检查是否有足够的权限创建主题
- 检查 Kafka 集群配置是否允许自动创建主题
- 检查分区数和副本因子是否合理
- 查看详细的错误日志，了解具体的失败原因

### Q: 批量操作速度慢怎么办？

**A**：可以尝试以下方法：
- 调整 `KAFKA_PRODUCER_LINGER_MS` 和 `KAFKA_PRODUCER_BATCH_SIZE` 参数
- 增加内存限制
- 确保网络连接稳定
- 检查 Kafka 集群的性能状况

### Q: 性能测试结果不理想怎么办？

**A**：可以尝试以下方法：
- 确保 Kafka 集群运行正常
- 检查网络连接是否稳定
- 调整生产者和消费者配置
- 在不同的网络环境下进行测试
- 考虑使用更高性能的硬件

## 技术支持

如果您在使用过程中遇到问题，可以：

1. **查看帮助信息**：运行 `kafka_aot help` 查看命令使用方法
2. **查看日志**：检查详细的错误日志，了解问题原因
3. **检查配置**：确保环境变量和配置选项正确设置
4. **参考文档**：查看本文档和技术参考文档
5. **联系技术支持**：如果问题仍然无法解决，请联系技术支持团队

## 更新日志

### v1.0.0 (2026-01-21)

- 初始版本
- 支持消息发送和消费
- 支持主题管理（列出、创建、删除、描述）
- 支持批量消息发送
- 支持性能基准测试
- 提供命令行接口和命令别名
- 采用 AOT 编译技术，提高性能
- 支持 .NET 10.0
- 支持跨平台运行

## 许可证

本项目采用 MIT 许可证，详见 LICENSE 文件。
