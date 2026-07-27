#:sdk Microsoft.NET.Sdk
#:package Confluent.Kafka@2.3.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Caching.Memory@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaAot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole())
                .AddMemoryCache()
                .AddSingleton<KafkaService>()
                .BuildServiceProvider();

            var kafkaService = serviceProvider.GetRequiredService<KafkaService>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            try
            {
                var command = args[0].ToLower();
                switch (command)
                {
                    case "produce":
                    case "p":
                        if (args.Length < 4)
                        {
                            logger.LogError("请提供broker地址、主题和消息内容");
                            return;
                        }
                        var broker = args[1];
                        var topic = args[2];
                        var message = string.Join(" ", args.Skip(3));
                        await kafkaService.ProduceMessageAsync(broker, topic, message);
                        logger.LogInformation("消息发送成功");
                        break;

                    case "consume":
                    case "c":
                        if (args.Length < 4)
                        {
                            logger.LogError("请提供broker地址、主题和消费者组");
                            return;
                        }
                        broker = args[1];
                        topic = args[2];
                        var groupId = args[3];
                        var count = args.Length > 4 ? int.TryParse(args[4], out var c) ? c : 10 : 10;
                        await kafkaService.ConsumeMessagesAsync(broker, topic, groupId, count);
                        break;

                    case "list-topics":
                    case "lt":
                        if (args.Length < 2)
                        {
                            logger.LogError("请提供broker地址");
                            return;
                        }
                        broker = args[1];
                        await kafkaService.ListTopicsAsync(broker);
                        break;

                    case "create-topic":
                    case "ct":
                        if (args.Length < 4)
                        {
                            logger.LogError("请提供broker地址、主题名称和分区数");
                            return;
                        }
                        broker = args[1];
                        topic = args[2];
                        var partitions = int.TryParse(args[3], out var p) ? p : 1;
                        var replicationFactor = args.Length > 4 ? short.TryParse(args[4], out var rf) ? rf : (short)1 : (short)1;
                        await kafkaService.CreateTopicAsync(broker, topic, partitions, replicationFactor);
                        break;

                    case "delete-topic":
                    case "dt":
                        if (args.Length < 3)
                        {
                            logger.LogError("请提供broker地址和主题名称");
                            return;
                        }
                        broker = args[1];
                        topic = args[2];
                        await kafkaService.DeleteTopicAsync(broker, topic);
                        break;

                    case "describe-topic":
                    case "dtp":
                        if (args.Length < 3)
                        {
                            logger.LogError("请提供broker地址和主题名称");
                            return;
                        }
                        broker = args[1];
                        topic = args[2];
                        await kafkaService.DescribeTopicAsync(broker, topic);
                        break;

                    case "batch-produce":
                    case "bp":
                        if (args.Length < 5)
                        {
                            logger.LogError("请提供broker地址、主题、消息数量和消息前缀");
                            return;
                        }
                        broker = args[1];
                        topic = args[2];
                        var messageCount = int.TryParse(args[3], out var mc) ? mc : 100;
                        var messagePrefix = string.Join(" ", args.Skip(4));
                        await kafkaService.BatchProduceAsync(broker, topic, messageCount, messagePrefix);
                        break;

                    case "benchmark":
                    case "bm":
                        if (args.Length < 4)
                        {
                            logger.LogError("请提供broker地址、主题和消息数量");
                            return;
                        }
                        broker = args[1];
                        topic = args[2];
                        messageCount = int.TryParse(args[3], out var bc) ? bc : 1000;
                        await kafkaService.RunBenchmarkAsync(broker, topic, messageCount);
                        break;

                    case "help":
                    case "h":
                        ShowHelp();
                        break;

                    default:
                        logger.LogError("未知命令: {Command}", command);
                        ShowHelp();
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "执行命令时发生错误");
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Kafka AOT 工具");
            Console.WriteLine("=============");
            Console.WriteLine("命令列表:");
            Console.WriteLine("  produce|p <broker> <topic> <message> - 发送消息");
            Console.WriteLine("  consume|c <broker> <topic> <group> [count] - 消费消息");
            Console.WriteLine("  list-topics|lt <broker> - 列出所有主题");
            Console.WriteLine("  create-topic|ct <broker> <topic> <partitions> [replication-factor] - 创建主题");
            Console.WriteLine("  delete-topic|dt <broker> <topic> - 删除主题");
            Console.WriteLine("  describe-topic|dtp <broker> <topic> - 描述主题");
            Console.WriteLine("  batch-produce|bp <broker> <topic> <count> <prefix> - 批量发送消息");
            Console.WriteLine("  benchmark|bm <broker> <topic> <count> - 运行性能基准测试");
            Console.WriteLine("  help|h - 显示帮助信息");
        }
    }

    public class KafkaService
    {
        private readonly ILogger<KafkaService> _logger;

        public KafkaService()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole())
                .BuildServiceProvider();
            _logger = serviceProvider.GetRequiredService<ILogger<KafkaService>>();
        }

        public async Task ProduceMessageAsync(string broker, string topic, string message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = broker,
                ClientId = $"kafka-producer-{Guid.NewGuid()}"
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            var deliveryResult = await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
            _logger.LogInformation("消息发送到: {Topic}-{Partition}-{Offset}", deliveryResult.Topic, deliveryResult.Partition, deliveryResult.Offset);
        }

        public async Task ConsumeMessagesAsync(string broker, string topic, string groupId, int count)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = broker,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(topic);

            var messageCount = 0;
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            _logger.LogInformation("开始消费消息，最多消费 {Count} 条", count);

            try
            {
                while (messageCount < count)
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    _logger.LogInformation("消费消息: {Message}", consumeResult.Message.Value);
                    _logger.LogInformation("主题: {Topic}, 分区: {Partition}, 偏移量: {Offset}", consumeResult.Topic, consumeResult.Partition, consumeResult.Offset);
                    messageCount++;
                }
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
            finally
            {
                cancellationTokenSource.Cancel();
            }

            _logger.LogInformation("消费完成，共消费 {Count} 条消息", messageCount);
        }

        public async Task ListTopicsAsync(string broker)
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = broker
            };

            using var adminClient = new AdminClientBuilder(config).Build();
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));

            _logger.LogInformation("Kafka 集群信息:");
            _logger.LogInformation(" broker数量: {Count}", metadata.Brokers.Count);
            foreach (var brokerInfo in metadata.Brokers)
            {
                _logger.LogInformation("  Broker: {Host}:{Port}", brokerInfo.Host, brokerInfo.Port);
            }

            _logger.LogInformation("\n主题列表:");
            foreach (var topicMetadata in metadata.Topics)
            {
                if (!topicMetadata.Topic.StartsWith("__"))
                {
                    _logger.LogInformation("  主题: {Topic}", topicMetadata.Topic);
                }
            }
        }

        public async Task CreateTopicAsync(string broker, string topic, int partitions, short replicationFactor)
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = broker
            };

            using var adminClient = new AdminClientBuilder(config).Build();
            var topicSpecification = new TopicSpecification
            {
                Name = topic,
                NumPartitions = partitions,
                ReplicationFactor = replicationFactor
            };

            await adminClient.CreateTopicsAsync(new List<TopicSpecification> { topicSpecification });
            _logger.LogInformation("主题创建成功: {Topic}, 分区数: {Partitions}, 副本因子: {ReplicationFactor}", topic, partitions, replicationFactor);
        }

        public async Task DeleteTopicAsync(string broker, string topic)
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = broker
            };

            using var adminClient = new AdminClientBuilder(config).Build();
            await adminClient.DeleteTopicsAsync(new List<string> { topic });
            _logger.LogInformation("主题删除成功: {Topic}", topic);
        }

        public async Task DescribeTopicAsync(string broker, string topic)
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = broker
            };

            using var adminClient = new AdminClientBuilder(config).Build();
            var metadata = adminClient.GetMetadata(topic, TimeSpan.FromSeconds(10));

            var topicMetadata = metadata.Topics.FirstOrDefault(t => t.Topic == topic);
            if (topicMetadata == null)
            {
                _logger.LogError("主题不存在: {Topic}", topic);
                return;
            }

            _logger.LogInformation("主题信息: {Topic}", topic);
            _logger.LogInformation("  分区数: {Count}", topicMetadata.Partitions.Count);
            foreach (var partition in topicMetadata.Partitions)
            {
                _logger.LogInformation("  分区: {Partition}, 领导者: {Leader}, 副本: {Replicas}, ISR: {Isr}", 
                    partition.PartitionId, partition.Leader, 
                    string.Join(",", partition.Replicas), 
                    string.Join(",", partition.InSyncReplicas));
            }
        }

        public async Task BatchProduceAsync(string broker, string topic, int messageCount, string messagePrefix)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = broker,
                ClientId = $"kafka-batch-producer-{Guid.NewGuid()}",
                LingerMs = 10,
                BatchSize = 16384
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            var tasks = new List<Task>();

            _logger.LogInformation("开始批量发送消息，共发送 {Count} 条", messageCount);

            for (int i = 0; i < messageCount; i++)
            {
                var message = $"{messagePrefix} - 消息 {i + 1}";
                var task = producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
                tasks.Add(task);

                if (tasks.Count >= 100)
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                    _logger.LogInformation("已发送 {SentCount} 条消息", i + 1);
                }
            }

            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
            }

            _logger.LogInformation("批量发送完成，共发送 {Count} 条消息", messageCount);
        }

        public async Task RunBenchmarkAsync(string broker, string topic, int messageCount)
        {
            _logger.LogInformation("开始性能基准测试...");
            _logger.LogInformation(new string('=', 60));

            // 测试生产性能
            var producerStopwatch = Stopwatch.StartNew();
            await BatchProduceAsync(broker, topic, messageCount, "benchmark");
            producerStopwatch.Stop();
            var produceRate = messageCount / producerStopwatch.Elapsed.TotalSeconds;

            _logger.LogInformation("生产性能测试:");
            _logger.LogInformation("  消息数量: {Count}", messageCount);
            _logger.LogInformation("  耗时: {Elapsed} ms", producerStopwatch.ElapsedMilliseconds);
            _logger.LogInformation("  吞吐量: {Rate:F2} 条/秒", produceRate);

            // 测试消费性能
            var consumerGroup = $"benchmark-group-{Guid.NewGuid()}";
            var consumerStopwatch = Stopwatch.StartNew();
            await ConsumeMessagesAsync(broker, topic, consumerGroup, messageCount);
            consumerStopwatch.Stop();
            var consumeRate = messageCount / consumerStopwatch.Elapsed.TotalSeconds;

            _logger.LogInformation("\n消费性能测试:");
            _logger.LogInformation("  消息数量: {Count}", messageCount);
            _logger.LogInformation("  耗时: {Elapsed} ms", consumerStopwatch.ElapsedMilliseconds);
            _logger.LogInformation("  吞吐量: {Rate:F2} 条/秒", consumeRate);

            _logger.LogInformation(new string('=', 60));
            _logger.LogInformation("性能基准测试完成");
        }
    }
}