#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package RabbitMQ.Client@6.8.1
#:package Polly@8.3.1
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property ReadyToRun=true
#:property TieredCompilation=true
#:property Optimize=true

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqSkill
{
    // 配置选项
    public class RabbitMqOptions
    {
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public bool EnableConnectionPooling { get; set; } = true;
        public int MaxConnections { get; set; } = 10;
        public int ConnectionTimeout { get; set; } = 30;
        public bool EnableAutomaticRecovery { get; set; } = true;
        public TimeSpan RequestedHeartbeat { get; set; } = TimeSpan.FromSeconds(60);
        public bool EnableMessageCompression { get; set; } = false;
        public bool EnableBatchProcessing { get; set; } = false;
        public int BatchSize { get; set; } = 100;
        public TimeSpan BatchTimeout { get; set; } = TimeSpan.FromMilliseconds(100);
        public int MaxRetries { get; set; } = 3;
        public TimeSpan RetryInterval { get; set; } = TimeSpan.FromMilliseconds(500);
        public bool EnableCircuitBreaker { get; set; } = true;
        public int CircuitBreakerFailureThreshold { get; set; } = 50;
        public TimeSpan CircuitBreakerResetTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }

    // 消息选项
    public class MessageOptions
    {
        public bool Persistent { get; set; } = true;
        public TimeSpan? Expiration { get; set; }
        public string CorrelationId { get; set; }
        public string ReplyTo { get; set; }
        public string MessageId { get; set; }
        public IDictionary<string, object> Headers { get; set; } = new Dictionary<string, object>();
    }

    // 交换器类型
    public enum ExchangeType
    {
        Direct,
        Fanout,
        Topic,
        Headers
    }

    // 消息处理器委托
    public delegate Task<bool> MessageHandler<in T>(T message, IDictionary<string, object> headers, CancellationToken cancellationToken);

    // 连接池接口
    public interface IRabbitMqConnectionPool : IDisposable
    {
        Task<IConnection> AcquireConnectionAsync(CancellationToken cancellationToken = default);
        void ReleaseConnection(IConnection connection);
    }

    // 连接工厂接口
    public interface IRabbitMqConnectionFactory
    {
        IConnection CreateConnection();
        ConnectionFactory CreateConnectionFactory();
    }

    // RabbitMQ服务接口
    public interface IRabbitMqService : IDisposable
    {
        // 发布消息
        Task PublishAsync<T>(string exchange, string routingKey, T message, MessageOptions options = null, CancellationToken cancellationToken = default);
        Task PublishBatchAsync<T>(string exchange, string routingKey, IEnumerable<T> messages, MessageOptions options = null, CancellationToken cancellationToken = default);

        // 订阅消息
        Task<IDisposable> SubscribeAsync<T>(string exchange, string queue, string routingKey, MessageHandler<T> handler, CancellationToken cancellationToken = default);

        // RPC调用
        Task<TResponse> RpcCallAsync<TRequest, TResponse>(string queue, TRequest request, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

        // 声明交换器
        Task DeclareExchangeAsync(string exchange, ExchangeType type, bool durable = true, bool autoDelete = false, CancellationToken cancellationToken = default);

        // 声明队列
        Task DeclareQueueAsync(string queue, bool durable = true, bool exclusive = false, bool autoDelete = false, IDictionary<string, object> arguments = null, CancellationToken cancellationToken = default);

        // 绑定队列
        Task BindQueueAsync(string queue, string exchange, string routingKey, CancellationToken cancellationToken = default);

        // 声明死信队列
        Task DeclareDeadLetterQueueAsync(string queue, string deadLetterExchange, CancellationToken cancellationToken = default);

        // 声明延迟队列
        Task DeclareDelayedQueueAsync(string queue, int delayMilliseconds, CancellationToken cancellationToken = default);
    }

    // 连接工厂实现
    public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
    {
        private readonly RabbitMqOptions _options;

        public RabbitMqConnectionFactory(IOptions<RabbitMqOptions> options)
        {
            _options = options.Value;
        }

        public IConnection CreateConnection()
        {
            var factory = CreateConnectionFactory();
            return factory.CreateConnection();
        }

        public ConnectionFactory CreateConnectionFactory()
        {
            return new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(_options.ConnectionTimeout),
                AutomaticRecoveryEnabled = _options.EnableAutomaticRecovery,
                RequestedHeartbeat = _options.RequestedHeartbeat,
                TopologyRecoveryEnabled = true
            };
        }
    }

    // 连接池实现
    public class RabbitMqConnectionPool : IRabbitMqConnectionPool
    {
        private readonly ConcurrentBag<IConnection> _connections;
        private readonly IRabbitMqConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMqConnectionPool> _logger;
        private readonly int _maxConnections;
        private int _currentConnections;
        private bool _disposed;

        public RabbitMqConnectionPool(IRabbitMqConnectionFactory connectionFactory, IOptions<RabbitMqOptions> options, ILogger<RabbitMqConnectionPool> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _maxConnections = options.Value.MaxConnections;
            _connections = new ConcurrentBag<IConnection>();
            _currentConnections = 0;
            _disposed = false;
        }

        public async Task<IConnection> AcquireConnectionAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RabbitMqConnectionPool));

            // 尝试从连接池获取连接
            if (_connections.TryTake(out var connection) && connection.IsOpen)
            {
                _logger.LogDebug("Acquired connection from pool");
                return connection;
            }

            // 如果连接池为空且未达到最大连接数，创建新连接
            if (Interlocked.Increment(ref _currentConnections) <= _maxConnections)
            {
                try
                {
                    _logger.LogDebug("Creating new connection");
                    connection = await Task.Run(() => _connectionFactory.CreateConnection(), cancellationToken);
                    connection.ConnectionShutdown += (sender, args) =>
                    {
                        _logger.LogWarning("Connection shutdown: {Reason}", args.ReplyText);
                        Interlocked.Decrement(ref _currentConnections);
                    };
                    return connection;
                }
                catch
                {
                    Interlocked.Decrement(ref _currentConnections);
                    throw;
                }
            }
            else
            {
                Interlocked.Decrement(ref _currentConnections);

                // 等待连接可用
                _logger.LogDebug("Connection pool exhausted, waiting for available connection");
                for (int i = 0; i < 10; i++)
                {
                    if (_connections.TryTake(out connection) && connection.IsOpen)
                    {
                        _logger.LogDebug("Acquired connection from pool after waiting");
                        return connection;
                    }
                    await Task.Delay(100, cancellationToken);
                }

                // 如果仍然没有可用连接，创建新连接（超过最大连接数）
                _logger.LogWarning("Connection pool exhausted, creating emergency connection");
                connection = await Task.Run(() => _connectionFactory.CreateConnection(), cancellationToken);
                connection.ConnectionShutdown += (sender, args) =>
                {
                    _logger.LogWarning("Emergency connection shutdown: {Reason}", args.ReplyText);
                };
                return connection;
            }
        }

        public void ReleaseConnection(IConnection connection)
        {
            if (_disposed || connection == null || !connection.IsOpen)
            {
                if (connection != null && !connection.IsOpen)
                {
                    _logger.LogDebug("Released closed connection");
                    Interlocked.Decrement(ref _currentConnections);
                }
                return;
            }

            _connections.Add(connection);
            _logger.LogDebug("Released connection back to pool");
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            foreach (var connection in _connections)
            {
                try
                {
                    if (connection.IsOpen)
                        connection.Close();
                    connection.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing connection");
                }
            }

            _connections.Clear();
        }
    }

    // RabbitMQ服务实现
    public class RabbitMqService : IRabbitMqService
    {
        private readonly IRabbitMqConnectionPool _connectionPool;
        private readonly ILogger<RabbitMqService> _logger;
        private readonly RabbitMqOptions _options;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
        private readonly ConcurrentDictionary<string, IDisposable> _subscriptions;
        private readonly JsonSerializerOptions _jsonOptions;
        private bool _disposed;

        public RabbitMqService(
            IRabbitMqConnectionPool connectionPool,
            IOptions<RabbitMqOptions> options,
            ILogger<RabbitMqService> logger)
        {
            _connectionPool = connectionPool;
            _options = options.Value;
            _logger = logger;
            _subscriptions = new ConcurrentDictionary<string, IDisposable>();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            // 配置重试策略
            _retryPolicy = Policy
                .Handle<Exception>(ex => ex is RabbitMQ.Client.Exceptions.BrokerUnreachableException || ex is RabbitMQ.Client.Exceptions.AlreadyClosedException)
                .WaitAndRetryAsync(
                    retryCount: _options.MaxRetries,
                    sleepDurationProvider: attempt => _options.RetryInterval * Math.Pow(2, attempt - 1),
                    onRetry: (exception, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception, "Retry {RetryCount} after {Delay}ms due to: {Message}", retryCount, timespan.TotalMilliseconds, exception.Message);
                    });

            // 配置熔断策略
            _circuitBreakerPolicy = Policy
                .Handle<Exception>(ex => ex is RabbitMQ.Client.Exceptions.BrokerUnreachableException || ex is RabbitMQ.Client.Exceptions.AlreadyClosedException)
                .CircuitBreakerAsync(
                    failureThreshold: _options.CircuitBreakerFailureThreshold / 100.0,
                    samplingDuration: TimeSpan.FromSeconds(30),
                    minimumThroughput: 10,
                    durationOfBreak: _options.CircuitBreakerResetTimeout,
                    onBreak: (exception, breakDuration) =>
                    {
                        _logger.LogWarning(exception, "Circuit breaker opened for {Duration}", breakDuration);
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation("Circuit breaker reset");
                    },
                    onHalfOpen: () =>
                    {
                        _logger.LogInformation("Circuit breaker half-open");
                    });
        }

        // 发布消息
        public async Task PublishAsync<T>(string exchange, string routingKey, T message, MessageOptions options = null, CancellationToken cancellationToken = default)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    using var connection = await _connectionPool.AcquireConnectionAsync(cancellationToken);
                    using var channel = connection.CreateModel();

                    // 序列化消息
                    var messageBytes = SerializeMessage(message);

                    // 压缩消息
                    if (_options.EnableMessageCompression && messageBytes.Length > 1024)
                    {
                        messageBytes = CompressMessage(messageBytes);
                    }

                    // 创建基本属性
                    var properties = channel.CreateBasicProperties();
                    ConfigureMessageProperties(properties, options);

                    // 发布消息
                    channel.BasicPublish(
                        exchange: exchange,
                        routingKey: routingKey,
                        basicProperties: properties,
                        body: messageBytes);

                    _logger.LogDebug("Published message to exchange {Exchange} with routing key {RoutingKey}", exchange, routingKey);

                    _connectionPool.ReleaseConnection(connection);
                });
            });
        }

        // 批量发布消息
        public async Task PublishBatchAsync<T>(string exchange, string routingKey, IEnumerable<T> messages, MessageOptions options = null, CancellationToken cancellationToken = default)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    using var connection = await _connectionPool.AcquireConnectionAsync(cancellationToken);
                    using var channel = connection.CreateModel();

                    foreach (var message in messages)
                    {
                        // 序列化消息
                        var messageBytes = SerializeMessage(message);

                        // 压缩消息
                        if (_options.EnableMessageCompression && messageBytes.Length > 1024)
                        {
                            messageBytes = CompressMessage(messageBytes);
                        }

                        // 创建基本属性
                        var properties = channel.CreateBasicProperties();
                        ConfigureMessageProperties(properties, options);

                        // 发布消息
                        channel.BasicPublish(
                            exchange: exchange,
                            routingKey: routingKey,
                            basicProperties: properties,
                            body: messageBytes);
                    }

                    _logger.LogDebug("Published batch of messages to exchange {Exchange} with routing key {RoutingKey}", exchange, routingKey);

                    _connectionPool.ReleaseConnection(connection);
                });
            });
        }

        // 订阅消息
        public async Task<IDisposable> SubscribeAsync<T>(string exchange, string queue, string routingKey, MessageHandler<T> handler, CancellationToken cancellationToken = default)
        {
            var subscriptionId = $"{exchange}:{queue}:{routingKey}:{Guid.NewGuid()}";

            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    var connection = await _connectionPool.AcquireConnectionAsync(cancellationToken);
                    var channel = connection.CreateModel();

                    // 声明交换器
                    await DeclareExchangeAsync(exchange, ExchangeType.Direct, cancellationToken: cancellationToken);

                    // 声明队列
                    await DeclareQueueAsync(queue, cancellationToken: cancellationToken);

                    // 绑定队列
                    await BindQueueAsync(queue, exchange, routingKey, cancellationToken);

                    // 配置消费者
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += async (sender, args) =>
                    {
                        var consumerChannel = (IModel)sender;
                        bool acknowledged = false;

                        try
                        {
                            // 解压缩消息
                            var messageBytes = args.Body.ToArray();
                            if (args.BasicProperties.Headers?.ContainsKey("compressed") == true)
                            {
                                messageBytes = DecompressMessage(messageBytes);
                            }

                            // 反序列化消息
                            var message = DeserializeMessage<T>(messageBytes);

                            // 处理消息
                            acknowledged = await handler(message, args.BasicProperties.Headers ?? new Dictionary<string, object>(), cancellationToken);

                            if (acknowledged)
                            {
                                consumerChannel.BasicAck(args.DeliveryTag, multiple: false);
                            }
                            else
                            {
                                consumerChannel.BasicNack(args.DeliveryTag, multiple: false, requeue: false);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing message");
                            consumerChannel.BasicNack(args.DeliveryTag, multiple: false, requeue: false);
                        }
                    };

                    consumer.Shutdown += (sender, args) =>
                    {
                        _logger.LogWarning("Consumer shutdown: {Reason}", args.ReplyText);
                        _connectionPool.ReleaseConnection(connection);
                    };

                    var consumerTag = channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);

                    var subscription = new Subscription(
                        subscriptionId,
                        () =>
                        {
                            try
                            {
                                channel.BasicCancel(consumerTag);
                                channel.Dispose();
                                _connectionPool.ReleaseConnection(connection);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error disposing subscription");
                            }
                        });

                    _subscriptions.TryAdd(subscriptionId, subscription);
                });
            });

            return _subscriptions[subscriptionId];
        }

        // RPC调用
        public async Task<TResponse> RpcCallAsync<TRequest, TResponse>(string queue, TRequest request, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    using var connection = await _connectionPool.AcquireConnectionAsync(cancellationToken);
                    using var channel = connection.CreateModel();

                    // 声明队列
                    await DeclareQueueAsync(queue, cancellationToken: cancellationToken);

                    // 创建回调队列
                    var callbackQueue = channel.QueueDeclare().QueueName;
                    var correlationId = Guid.NewGuid().ToString();

                    // 创建消费者
                    var consumer = new EventingBasicConsumer(channel);
                    var responseCompletion = new TaskCompletionSource<TResponse>();

                    consumer.Received += (sender, args) =>
                    {
                        if (args.BasicProperties.CorrelationId == correlationId)
                        {
                            try
                            {
                                // 解压缩消息
                                var messageBytes = args.Body.ToArray();
                                if (args.BasicProperties.Headers?.ContainsKey("compressed") == true)
                                {
                                    messageBytes = DecompressMessage(messageBytes);
                                }

                                // 反序列化响应
                                var response = DeserializeMessage<TResponse>(messageBytes);
                                responseCompletion.TrySetResult(response);
                            }
                            catch (Exception ex)
                            {
                                responseCompletion.TrySetException(ex);
                            }
                        }
                    };

                    channel.BasicConsume(queue: callbackQueue, autoAck: true, consumer: consumer);

                    // 序列化请求
                    var requestBytes = SerializeMessage(request);

                    // 压缩消息
                    if (_options.EnableMessageCompression && requestBytes.Length > 1024)
                    {
                        requestBytes = CompressMessage(requestBytes);
                    }

                    // 创建基本属性
                    var properties = channel.CreateBasicProperties();
                    properties.CorrelationId = correlationId;
                    properties.ReplyTo = callbackQueue;
                    properties.Persistent = true;

                    if (_options.EnableMessageCompression && requestBytes.Length > 1024)
                    {
                        properties.Headers = new Dictionary<string, object> { { "compressed", true } };
                    }

                    // 发布消息
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: queue,
                        basicProperties: properties,
                        body: requestBytes);

                    // 等待响应
                    var responseTask = responseCompletion.Task;
                    var timeoutTask = Task.Delay(timeout ?? TimeSpan.FromSeconds(30), cancellationToken);

                    var completedTask = await Task.WhenAny(responseTask, timeoutTask);
                    if (completedTask == timeoutTask)
                    {
                        throw new TimeoutException("RPC call timed out");
                    }

                    _connectionPool.ReleaseConnection(connection);
                    return await responseTask;
                });
            });
        }

        // 声明交换器
        public async Task DeclareExchangeAsync(string exchange, ExchangeType type, bool durable = true, bool autoDelete = false, CancellationToken cancellationToken = default)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    using var connection = await _connectionPool.AcquireConnectionAsync(cancellationToken);
                    using var channel = connection.CreateModel();

                    var exchangeType = type switch
                    {
                        ExchangeType.Direct => RabbitMQ.Client.ExchangeType.Direct,
                        ExchangeType.Fanout => RabbitMQ.Client.ExchangeType.Fanout,
                        ExchangeType.Topic => RabbitMQ.Client.ExchangeType.Topic,
                        ExchangeType.Headers => RabbitMQ.Client.ExchangeType.Headers,
                        _ => RabbitMQ.Client.ExchangeType.Direct
                    };

                    channel.ExchangeDeclare(
                        exchange: exchange,
                        type: exchangeType,
                        durable: durable,
                        autoDelete: autoDelete,
                        arguments: null);

                    _connectionPool.ReleaseConnection(connection);
                });
            });
        }

        // 声明队列
        public async Task DeclareQueueAsync(string queue, bool durable = true, bool exclusive = false, bool autoDelete = false, IDictionary<string, object> arguments = null, CancellationToken cancellationToken = default)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    using var connection = await _connectionPool.AcquireConnectionAsync(cancellationToken);
                    using var channel = connection.CreateModel();

                    channel.QueueDeclare(
                        queue: queue,
                        durable: durable,
                        exclusive: exclusive,
                        autoDelete: autoDelete,
                        arguments: arguments);

                    _connectionPool.ReleaseConnection(connection);
                });
            });
        }

        // 绑定队列
        public async Task BindQueueAsync(string queue, string exchange, string routingKey, CancellationToken cancellationToken = default)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    using var connection = await _connectionPool.AcquireConnectionAsync(cancellationToken);
                    using var channel = connection.CreateModel();

                    channel.QueueBind(
                        queue: queue,
                        exchange: exchange,
                        routingKey: routingKey);

                    _connectionPool.ReleaseConnection(connection);
                });
            });
        }

        // 声明死信队列
        public async Task DeclareDeadLetterQueueAsync(string queue, string deadLetterExchange, CancellationToken cancellationToken = default)
        {
            var arguments = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", deadLetterExchange }
            };

            await DeclareQueueAsync(queue, arguments: arguments, cancellationToken: cancellationToken);
            await DeclareExchangeAsync(deadLetterExchange, ExchangeType.Direct, cancellationToken: cancellationToken);
        }

        // 声明延迟队列
        public async Task DeclareDelayedQueueAsync(string queue, int delayMilliseconds, CancellationToken cancellationToken = default)
        {
            var delayedExchange = $"{queue}-delayed";
            var arguments = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "" },
                { "x-dead-letter-routing-key", queue },
                { "x-message-ttl", delayMilliseconds }
            };

            await DeclareQueueAsync($"{queue}-delayed", arguments: arguments, cancellationToken: cancellationToken);
            await DeclareQueueAsync(queue, cancellationToken: cancellationToken);
        }

        // 序列化消息
        private byte[] SerializeMessage<T>(T message)
        {
            return JsonSerializer.SerializeToUtf8Bytes(message, _jsonOptions);
        }

        // 反序列化消息
        private T DeserializeMessage<T>(byte[] data)
        {
            return JsonSerializer.Deserialize<T>(data, _jsonOptions);
        }

        // 压缩消息
        private byte[] CompressMessage(byte[] data)
        {
            using var outputStream = new MemoryStream();
            using var gzipStream = new GZipStream(outputStream, CompressionLevel.Optimal);
            gzipStream.Write(data, 0, data.Length);
            gzipStream.Flush();
            return outputStream.ToArray();
        }

        // 解压缩消息
        private byte[] DecompressMessage(byte[] data)
        {
            using var inputStream = new MemoryStream(data);
            using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();
            gzipStream.CopyTo(outputStream);
            return outputStream.ToArray();
        }

        // 配置消息属性
        private void ConfigureMessageProperties(IBasicProperties properties, MessageOptions options)
        {
            properties.Persistent = options?.Persistent ?? true;
            properties.CorrelationId = options?.CorrelationId;
            properties.ReplyTo = options?.ReplyTo;
            properties.MessageId = options?.MessageId ?? Guid.NewGuid().ToString();

            if (options?.Expiration != null)
            {
                properties.Expiration = options.Expiration.Value.TotalMilliseconds.ToString("F0");
            }

            if (options?.Headers != null && options.Headers.Count > 0)
            {
                properties.Headers = options.Headers;
            }

            if (_options.EnableMessageCompression)
            {
                if (properties.Headers == null)
                {
                    properties.Headers = new Dictionary<string, object>();
                }
                properties.Headers["compressed"] = true;
            }
        }

        // 订阅类
        private class Subscription : IDisposable
        {
            private readonly string _id;
            private readonly Action _disposeAction;
            private bool _disposed;

            public Subscription(string id, Action disposeAction)
            {
                _id = id;
                _disposeAction = disposeAction;
                _disposed = false;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
                _disposeAction();
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            // 清理所有订阅
            foreach (var subscription in _subscriptions.Values)
            {
                try
                {
                    subscription.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing subscription");
                }
            }

            _subscriptions.Clear();
        }
    }

    // 依赖注入扩展
    public static class RabbitMqServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMqServices(this IServiceCollection services, Action<RabbitMqOptions> configureOptions = null)
        {
            // 配置选项
            services.Configure(configureOptions ?? (options => { }));

            // 注册服务
            services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
            services.AddSingleton<IRabbitMqConnectionPool, RabbitMqConnectionPool>();
            services.AddSingleton<IRabbitMqService, RabbitMqService>();

            return services;
        }
    }

    // 高级RabbitMQ功能
    public static class AdvancedRabbitMqFeatures
    {
        // 1. 消息确认机制
        public static void ConfigureMessageAcknowledgement(ConnectionFactory factory)
        {
            factory.AutomaticRecoveryEnabled = true;
            factory.TopologyRecoveryEnabled = true;
            factory.RequestedHeartbeat = TimeSpan.FromSeconds(60);
        }

        // 2. 死信队列实现
        public static void ConfigureDeadLetterExchange(IModel channel, string queueName, string deadLetterExchange)
        {
            var args = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", deadLetterExchange }
            };
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, args);
        }

        // 3. 延迟队列实现
        public static void ConfigureDelayedMessages(IModel channel, string queueName, string delayedExchange, int delayMilliseconds)
        {
            var args = new Dictionary<string, object>
            {
                { "x-delayed-type", "direct" }
            };
            channel.ExchangeDeclare(delayedExchange, "x-delayed-message", durable: true, autoDelete: false, args);

            var queueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "" },
                { "x-dead-letter-routing-key", queueName },
                { "x-message-ttl", delayMilliseconds }
            };
            channel.QueueDeclare($"{queueName}-delayed", durable: true, exclusive: false, autoDelete: false, queueArgs);
            channel.QueueBind($"{queueName}-delayed", delayedExchange, queueName);
        }

        // 4. RPC模式实现
        public static string ConfigureRpcPattern(IModel channel, out EventingBasicConsumer consumer)
        {
            var replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(replyQueueName, true, consumer);
            return replyQueueName;
        }

        // 5. 集群管理实现
        public static void ConfigureCluster(ConnectionFactory factory, List<string> hosts)
        {
            factory.HostName = hosts[0];
            factory.AutomaticRecoveryEnabled = true;
            factory.TopologyRecoveryEnabled = true;
        }

        // 6. 镜像队列配置
        public static void ConfigureMirroredQueue(IModel channel, string queueName)
        {
            var args = new Dictionary<string, object>
            {
                { "x-ha-policy", "all" }
            };
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, args);
        }

        // 7. 消息追踪实现
        public static void EnableMessageTracing(IModel channel, string exchangeName)
        {
            var args = new Dictionary<string, object>
            {
                { "alternate-exchange", "amq.rabbitmq.trace" }
            };
            channel.ExchangeDeclare(exchangeName, RabbitMQ.Client.ExchangeType.Topic, durable: true, autoDelete: false, args);
        }

        // 8. 流控配置
        public static void ConfigureFlowControl(IModel channel, ushort prefetchCount)
        {
            channel.BasicQos(prefetchSize: 0, prefetchCount: prefetchCount, global: false);
        }

        // 9. 批量发布配置
        public static void ConfigureBatchPublishing(IModel channel, int batchSize)
        {
            channel.ConfirmSelect();
        }

        // 10. 连接池配置
        public static void ConfigureConnectionPool(RabbitMqOptions options, int maxConnections, int maxChannelsPerConnection)
        {
            options.MaxConnections = maxConnections;
        }
    }
}
