# Kafka 技能使用示例

## 快速开始

### 1. 基本使用示例

#### 1.1 发送消息示例

```bash
# 发送单条消息
kafka_aot produce localhost:9092 test-topic "Hello, Kafka!"

# 使用命令别名
kafka_aot p localhost:9092 test-topic "Hello, Kafka!"
```

#### 1.2 消费消息示例

```bash
# 消费消息
kafka_aot consume localhost:9092 test-topic test-group 5

# 使用命令别名
kafka_aot c localhost:9092 test-topic test-group 5
```

#### 1.3 主题管理示例

```bash
# 列出所有主题
kafka_aot list-topics localhost:9092

# 创建主题
kafka_aot create-topic localhost:9092 new-topic 3 1

# 删除主题
kafka_aot delete-topic localhost:9092 old-topic

# 描述主题
kafka_aot describe-topic localhost:9092 test-topic
```

## 高级使用示例

### 2. 批量操作示例

#### 2.1 批量发送消息

```bash
# 批量发送1000条消息
kafka_aot batch-produce localhost:9092 test-topic 1000 "Test message"

# 使用命令别名
kafka_aot bp localhost:9092 test-topic 1000 "Test message"
```

#### 2.2 性能测试示例

```bash
# 运行性能测试，发送10000条消息
kafka_aot benchmark localhost:9092 test-topic 10000

# 使用命令别名
kafka_aot bm localhost:9092 test-topic 10000
```

### 3. 编程集成示例

#### 3.1 基本集成示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;

public class KafkaIntegrationExample
{
    public static async Task Main()
    {
        Console.WriteLine("Kafka 集成示例");
        Console.WriteLine("=" * 50);
        
        // 生产者配置
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            ClientId = "kafka-integration-example",
            Acks = Acks.One
        };
        
        // 消费者配置
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "kafka-integration-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };
        
        // 发送消息
        using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        var deliveryResult = await producer.ProduceAsync("test-topic", new Message<Null, string> { Value = "Integration test message" });
        Console.WriteLine($"消息发送到: {deliveryResult.Topic}-{deliveryResult.Partition}-{deliveryResult.Offset}");
        
        // 消费消息
        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        consumer.Subscribe("test-topic");
        
        Console.WriteLine("开始消费消息...");
        for (int i = 0; i < 5; i++)
        {
            var consumeResult = consumer.Consume(TimeSpan.FromSeconds(10));
            if (consumeResult != null)
            {
                Console.WriteLine($"消费消息: {consumeResult.Message.Value}");
                Console.WriteLine($"主题: {consumeResult.Topic}, 分区: {consumeResult.Partition}, 偏移量: {consumeResult.Offset}");
            }
        }
        
        consumer.Close();
    }
}
```

#### 3.2 依赖注入集成示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class KafkaDependencyInjectionExample
{
    public static async Task Main()
    {
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddSingleton<KafkaService>()
            .BuildServiceProvider();
        
        // 获取服务
        var kafkaService = serviceProvider.GetRequiredService<KafkaService>();
        var logger = serviceProvider.GetRequiredService<ILogger<KafkaDependencyInjectionExample>>();
        
        logger.LogInformation("Kafka 依赖注入集成示例");
        logger.LogInformation(new string('=', 50));
        
        try
        {
            // 发送消息
            await kafkaService.ProduceMessageAsync("localhost:9092", "test-topic", "DI integration test message");
            logger.LogInformation("消息发送成功");
            
            // 列出主题
            await kafkaService.ListTopicsAsync("localhost:9092");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "操作失败");
        }
    }
}
```

## 使用场景示例

### 4. 消息队列管理场景

#### 4.1 实时日志收集

```bash
# 启动日志收集消费者
kafka_aot consume localhost:9092 logs-app1 log-group 100

# 发送日志消息
kafka_aot produce localhost:9092 logs-app1 "[ERROR] Failed to connect to database"
kafka_aot produce localhost:9092 logs-app1 "[INFO] User login: admin"
kafka_aot produce localhost:9092 logs-app1 "[WARNING] Disk space low"
```

#### 4.2 事件驱动架构

```bash
# 发布事件
kafka_aot produce localhost:9092 events-user user-registered '{"userId": 123, "email": "user@example.com"}'
kafka_aot produce localhost:9092 events-order order-created '{"orderId": 456, "amount": 100.0}'

# 订阅事件
kafka_aot consume localhost:9092 events-user user-service 10
kafka_aot consume localhost:9092 events-order order-service 10
```

### 5. 数据流处理场景

#### 5.1 实时数据采集

```bash
# 批量发送传感器数据
kafka_aot batch-produce localhost:9092 sensor-data 1000 "Sensor-1: Temperature="

# 消费并处理数据
kafka_aot consume localhost:9092 sensor-data data-processor 100
```

#### 5.2 数据集成和ETL

```bash
# 从源系统提取数据
kafka_aot batch-produce localhost:9092 etl-source 5000 "SourceData:"

# 转换和加载数据
kafka_aot consume localhost:9092 etl-source etl-processor 5000
```

### 6. 微服务通信场景

#### 6.1 服务间异步通信

```bash
# 服务A发送消息
kafka_aot produce localhost:9092 service-events payment-processed '{"paymentId": 789, "status": "completed"}'

# 服务B消费消息
kafka_aot consume localhost:9092 service-events notification-service 10
```

#### 6.2 服务解耦

```bash
# 创建服务间通信主题
kafka_aot create-topic localhost:9092 service-a-events 3 2
kafka_aot create-topic localhost:9092 service-b-events 3 2
kafka_aot create-topic localhost:9092 service-c-events 3 2

# 查看主题列表
kafka_aot list-topics localhost:9092
```

## 性能优化示例

### 7. 生产者优化示例

```bash
# 设置优化的环境变量
set KAFKA_PRODUCER_LINGER_MS=50
set KAFKA_PRODUCER_BATCH_SIZE=32768

# 运行批量发送
kafka_aot batch-produce localhost:9092 test-topic 10000 "Optimized message"
```

### 8. 消费者优化示例

```bash
# 设置优化的环境变量
set KAFKA_CONSUMER_FETCH_MIN_BYTES=1024
set KAFKA_CONSUMER_FETCH_MAX_WAIT_MS=200

# 运行消费
kafka_aot consume localhost:9092 test-topic optimized-group 1000
```

## 错误处理示例

### 9. 常见错误处理

#### 9.1 连接错误处理

```bash
# 尝试连接到不存在的broker
kafka_aot list-topics localhost:9999

# 输出示例：
# Error: No such host is known. (localhost:9999)
```

#### 9.2 主题不存在错误处理

```bash
# 尝试发送消息到不存在的主题
kafka_aot produce localhost:9092 non-existent-topic "Test message"

# 输出示例：
# Error: Local: Topic not present or not leader for partition
```

#### 9.3 权限错误处理

```bash
# 尝试删除需要权限的主题
kafka_aot delete-topic localhost:9092 protected-topic

# 输出示例：
# Error: Admin: Authorization failed.
```

## 总结

本文档提供了 Kafka 技能的各种使用示例，涵盖了从基本操作到高级场景的各种应用。通过这些示例，您可以：

1. 快速上手 Kafka 基本操作
2. 掌握批量操作和性能测试
3. 了解如何在编程中集成 Kafka
4. 应用 Kafka 到各种实际场景
5. 优化 Kafka 性能
6. 处理常见错误情况

Kafka 技能采用 .NET 10 最佳实践设计，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。
