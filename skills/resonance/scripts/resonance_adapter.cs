#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Text.Json@10.0.0
#:package System.Text.Encodings.Web@10.0.0
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property PublishAot=true
#:property TrimMode=partial
#:property EnableCompressionInSingleFile=true
#:property SelfContained=true

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// 消息选项
public class MessageOptions
{
    public int MaxMessageSize { get; set; } = 1024 * 1024; // 1MB
    public int QueueCapacity { get; set; } = 1000;
    public int WorkerCount { get; set; } = Environment.ProcessorCount;
    public int RetryCount { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 100;
    public bool EnableDeadLetterQueue { get; set; } = true;
}

// 消息上下文
public class MessageContext
{
    public string MessageId { get; set; } = Guid.NewGuid().ToString();
    public string Source { get; set; }
    public string Destination { get; set; }
    public string Type { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public object Body { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
}

// 消息结果
public class MessageResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public object? Result { get; set; }
    public TimeSpan ExecutionTime { get; set; }
}

// 消息处理器
public delegate Task<MessageResult> MessageHandler(MessageContext context);

// 消息管道组件
public interface IMessagePipeline
{
    Task<MessageResult> ProcessAsync(MessageContext context);
    IMessagePipeline AddMiddleware(Func<MessageContext, Func<MessageContext, Task<MessageResult>>, Task<MessageResult>> middleware);
}

// 共振适配器接口
public interface IResonanceAdapter
{
    // 发送消息
    Task<MessageResult> SendAsync(MessageContext context);
    
    // 发送消息（泛型）
    Task<MessageResult> SendAsync<T>(string destination, T message, string source = "");
    
    // 注册消息处理器
    void RegisterHandler(string messageType, MessageHandler handler);
    
    // 注册消息处理器（泛型）
    void RegisterHandler<T>(Func<MessageContext, T, Task<MessageResult>> handler);
    
    // 注册消息处理器（带中间件）
    void RegisterHandler(string messageType, MessageHandler handler, params Func<MessageContext, Func<MessageContext, Task<MessageResult>>, Task<MessageResult>>[] middleware);
    
    // 取消注册消息处理器
    void UnregisterHandler(string messageType);
    
    // 获取所有注册的处理器
    IEnumerable<string> GetRegisteredHandlers();
    
    // 启动适配器
    Task StartAsync(CancellationToken cancellationToken = default);
    
    // 停止适配器
    Task StopAsync(CancellationToken cancellationToken = default);
    
    // 获取消息处理统计
    MessageStats GetStats();
}

// 消息统计
public class MessageStats
{
    public long TotalMessages { get; set; }
    public long SuccessfulMessages { get; set; }
    public long FailedMessages { get; set; }
    public long RetriedMessages { get; set; }
    public double AverageProcessingTimeMs { get; set; }
    public int ActiveHandlers { get; set; }
    public int QueueDepth { get; set; }
}

// 消息管道实现
public class MessagePipeline : IMessagePipeline
{
    private readonly List<Func<MessageContext, Func<MessageContext, Task<MessageResult>>, Task<MessageResult>>> _middleware = new();
    private readonly MessageHandler _handler;

    public MessagePipeline(MessageHandler handler)
    {
        _handler = handler;
    }

    public IMessagePipeline AddMiddleware(Func<MessageContext, Func<MessageContext, Task<MessageResult>>, Task<MessageResult>> middleware)
    {
        _middleware.Add(middleware);
        return this;
    }

    public async Task<MessageResult> ProcessAsync(MessageContext context)
    {
        // 构建中间件管道
        var pipeline = _handler;
        if (_middleware.Any())
        {
            for (int i = _middleware.Count - 1; i >= 0; i--)
            {
                var currentMiddleware = _middleware[i];
                var next = pipeline;
                pipeline = async (ctx) => await currentMiddleware(ctx, next);
            }
        }

        return await pipeline(context);
    }
}

// 共振适配器实现
public class ResonanceAdapter : IResonanceAdapter
{
    private readonly MessageOptions _options;
    private readonly ILogger<ResonanceAdapter>? _logger;
    private readonly ConcurrentDictionary<string, MessagePipeline> _handlers = new();
    private readonly ConcurrentDictionary<string, long> _messageCounts = new();
    private readonly SemaphoreSlim _queueSemaphore;
    private long _totalMessages = 0;
    private long _successfulMessages = 0;
    private long _failedMessages = 0;
    private long _retriedMessages = 0;
    private double _totalProcessingTimeMs = 0;
    private bool _isRunning = false;
    private CancellationTokenSource _cts = new();

    public ResonanceAdapter(IOptions<MessageOptions> options, ILogger<ResonanceAdapter>? logger = null)
    {
        _options = options.Value;
        _logger = logger;
        _queueSemaphore = new SemaphoreSlim(_options.QueueCapacity);
    }

    // 发送消息
    public async Task<MessageResult> SendAsync(MessageContext context)
    {
        if (!_isRunning)
        {
            throw new InvalidOperationException("ResonanceAdapter is not running.");
        }

        Interlocked.Increment(ref _totalMessages);
        var startTime = DateTime.UtcNow;

        try
        {
            // 等待队列容量
            await _queueSemaphore.WaitAsync(context.CancellationToken);

            try
            {
                // 查找消息处理器
                if (_handlers.TryGetValue(context.Type, out var pipeline))
                {
                    // 处理消息
                    var result = await ProcessWithRetryAsync(pipeline, context);
                    
                    if (result.Success)
                    {
                        Interlocked.Increment(ref _successfulMessages);
                    }
                    else
                    {
                        Interlocked.Increment(ref _failedMessages);
                    }

                    return result;
                }
                else
                {
                    throw new InvalidOperationException($"未找到消息类型的处理器: {context.Type}");
                }
            }
            finally
            {
                _queueSemaphore.Release();
            }
        }
        catch (Exception ex)
        {
            Interlocked.Increment(ref _failedMessages);
            _logger?.LogError(ex, "[ResonanceAdapter] 消息发送失败: {MessageType} {MessageId}", context.Type, context.MessageId);
            
            return new MessageResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                ExecutionTime = DateTime.UtcNow - startTime
            };
        }
        finally
        {
            var processingTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
            Interlocked.Add(ref _totalProcessingTimeMs, processingTimeMs);
            _messageCounts.AddOrUpdate(context.Type, 1, (_, count) => count + 1);
        }
    }

    // 发送消息（泛型）
    public async Task<MessageResult> SendAsync<T>(string destination, T message, string source = "")
    {
        var context = new MessageContext
        {
            Source = source,
            Destination = destination,
            Type = typeof(T).FullName ?? typeof(T).Name,
            Body = message
        };

        return await SendAsync(context);
    }

    // 注册消息处理器
    public void RegisterHandler(string messageType, MessageHandler handler)
    {
        RegisterHandler(messageType, handler, Array.Empty<Func<MessageContext, Func<MessageContext, Task<MessageResult>>, Task<MessageResult>>>());
    }

    // 注册消息处理器（泛型）
    public void RegisterHandler<T>(Func<MessageContext, T, Task<MessageResult>> handler)
    {
        var messageType = typeof(T).FullName ?? typeof(T).Name;
        
        RegisterHandler(messageType, async (context) =>
        {
            if (context.Body is T typedMessage)
            {
                return await handler(context, typedMessage);
            }
            else
            {
                throw new InvalidOperationException($"消息体类型不匹配: 期望 {typeof(T).Name}, 实际 {context.Body?.GetType().Name}");
            }
        });
    }

    // 注册消息处理器（带中间件）
    public void RegisterHandler(string messageType, MessageHandler handler, params Func<MessageContext, Func<MessageContext, Task<MessageResult>>, Task<MessageResult>>[] middleware)
    {
        if (string.IsNullOrEmpty(messageType))
            throw new ArgumentNullException(nameof(messageType));
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var pipeline = new MessagePipeline(handler);
        foreach (var m in middleware)
        {
            pipeline.AddMiddleware(m);
        }

        _handlers[messageType] = pipeline;
        _logger?.LogInformation("[ResonanceAdapter] 注册消息处理器: {MessageType}", messageType);
    }

    // 取消注册消息处理器
    public void UnregisterHandler(string messageType)
    {
        if (_handlers.TryRemove(messageType, out _))
        {
            _logger?.LogInformation("[ResonanceAdapter] 取消注册消息处理器: {MessageType}", messageType);
        }
    }

    // 获取所有注册的处理器
    public IEnumerable<string> GetRegisteredHandlers()
    {
        return _handlers.Keys;
    }

    // 启动适配器
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_isRunning)
        {
            return;
        }

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _isRunning = true;
        
        _logger?.LogInformation("[ResonanceAdapter] 启动成功");
        
        // 可以在这里启动后台任务，例如定期清理、监控等
        await Task.CompletedTask;
    }

    // 停止适配器
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (!_isRunning)
        {
            return;
        }

        _cts.Cancel();
        _isRunning = false;
        
        // 等待队列处理完成
        await Task.Delay(100);
        
        _logger?.LogInformation("[ResonanceAdapter] 停止成功");
    }

    // 获取消息处理统计
    public MessageStats GetStats()
    {
        var activeHandlers = _handlers.Count;
        var queueDepth = _options.QueueCapacity - _queueSemaphore.CurrentCount;
        var averageProcessingTimeMs = _totalMessages > 0 ? _totalProcessingTimeMs / _totalMessages : 0;

        return new MessageStats
        {
            TotalMessages = _totalMessages,
            SuccessfulMessages = _successfulMessages,
            FailedMessages = _failedMessages,
            RetriedMessages = _retriedMessages,
            AverageProcessingTimeMs = averageProcessingTimeMs,
            ActiveHandlers = activeHandlers,
            QueueDepth = queueDepth
        };
    }

    // 带重试的消息处理
    private async Task<MessageResult> ProcessWithRetryAsync(MessagePipeline pipeline, MessageContext context)
    {
        int retryCount = 0;
        Exception? lastException = null;

        while (retryCount <= _options.RetryCount)
        {
            try
            {
                return await pipeline.ProcessAsync(context);
            }
            catch (Exception ex)
            {
                lastException = ex;
                retryCount++;
                
                if (retryCount <= _options.RetryCount)
                {
                    Interlocked.Increment(ref _retriedMessages);
                    _logger?.LogWarning(ex, "[ResonanceAdapter] 消息处理失败，正在重试 ({RetryCount}/{MaxRetries}): {MessageType} {MessageId}", 
                        retryCount, _options.RetryCount, context.Type, context.MessageId);
                    
                    await Task.Delay(_options.RetryDelayMs * retryCount, context.CancellationToken);
                }
            }
        }

        throw lastException ?? new InvalidOperationException("消息处理失败，达到最大重试次数");
    }
}

// 消息中间件
public static class MessageMiddleware
{
    // 日志中间件
    public static Func<MessageContext, Func<MessageContext, Task<MessageResult>>, Task<MessageResult>> Logging
    {
        get
        {
            return async (context, next) =>
            {
                Console.WriteLine($"[Middleware] 处理消息: {context.Type} {context.MessageId}");
                var result = await next(context);
                Console.WriteLine($"[Middleware] 消息处理完成: {context.Type} {context.MessageId} - {(result.Success ? "成功" : "失败")}");
                return result;
            };
        }
    }

    // 超时中间件
    public static Func<MessageContext, Func<MessageContext, Task<MessageResult>>, Task<MessageResult>> Timeout(int timeoutMs)
    {
        return async (context, next) =>
        {
            using var timeoutCts = new CancellationTokenSource(timeoutMs);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken, timeoutCts.Token);
            
            var timeoutContext = new MessageContext
            {
                MessageId = context.MessageId,
                Source = context.Source,
                Destination = context.Destination,
                Type = context.Type,
                Headers = context.Headers,
                Body = context.Body,
                Timestamp = context.Timestamp,
                CancellationToken = linkedCts.Token
            };

            try
            {
                return await next(timeoutContext);
            }
            catch (OperationCanceledException)
            {
                return new MessageResult
                {
                    Success = false,
                    ErrorMessage = "消息处理超时",
                    ExecutionTime = TimeSpan.FromMilliseconds(timeoutMs)
                };
            }
        };
    }
}

// 依赖注入扩展
public static class ResonanceAdapterExtensions
{
    public static IServiceCollection AddResonanceAdapter(this IServiceCollection services)
    {
        return services.AddResonanceAdapter(options => { });
    }

    public static IServiceCollection AddResonanceAdapter(this IServiceCollection services, Action<MessageOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<IResonanceAdapter, ResonanceAdapter>();
        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // 注册共振适配器服务
        services.AddResonanceAdapter(options =>
        {
            options.MaxMessageSize = 1024 * 1024;
            options.QueueCapacity = 1000;
            options.WorkerCount = Environment.ProcessorCount;
            options.RetryCount = 3;
            options.RetryDelayMs = 100;
            options.EnableDeadLetterQueue = true;
        });

        var serviceProvider = services.BuildServiceProvider();

        // 获取共振适配器服务
        var resonanceAdapter = serviceProvider.GetRequiredService<IResonanceAdapter>();

        Console.WriteLine("Resonance Adapter Demo");
        Console.WriteLine("=" + new string('=', 50));

        try
        {
            // 启动适配器
            await resonanceAdapter.StartAsync();
            Console.WriteLine("✓ Resonance Adapter 启动成功");

            // 定义消息类型
            record UserMessage(string Name, int Age);
            record OrderMessage(Guid OrderId, decimal Amount, string Status);

            // 注册消息处理器
            resonanceAdapter.RegisterHandler<UserMessage>(async (context, message) =>
            {
                Console.WriteLine($"[Handler] 处理用户消息: {message.Name}, {message.Age}");
                return new MessageResult
                {
                    Success = true,
                    Result = new { Processed = true, User = message.Name },
                    ExecutionTime = TimeSpan.FromMilliseconds(50)
                };
            });

            resonanceAdapter.RegisterHandler<OrderMessage>(async (context, message) =>
            {
                Console.WriteLine($"[Handler] 处理订单消息: {message.OrderId}, {message.Amount}, {message.Status}");
                return new MessageResult
                {
                    Success = true,
                    Result = new { Processed = true, OrderId = message.OrderId },
                    ExecutionTime = TimeSpan.FromMilliseconds(30)
                };
            });

            // 注册带中间件的消息处理器
            resonanceAdapter.RegisterHandler("System.Heartbeat", async (context) =>
            {
                Console.WriteLine($"[Handler] 处理心跳消息: {context.MessageId}");
                return new MessageResult
                {
                    Success = true,
                    Result = new { Status = "Alive", Timestamp = DateTime.UtcNow },
                    ExecutionTime = TimeSpan.FromMilliseconds(10)
                };
            },
            MessageMiddleware.Logging,
            MessageMiddleware.Timeout(1000)
            );

            Console.WriteLine("\n测试消息发送...");

            // 测试发送用户消息
            var userResult = await resonanceAdapter.SendAsync("user-service", new UserMessage("张三", 30), "demo-client");
            Console.WriteLine($"✓ 用户消息: {userResult.Success} - {userResult.Result}");

            // 测试发送订单消息
            var orderResult = await resonanceAdapter.SendAsync("order-service", new OrderMessage(Guid.NewGuid(), 199.99m, "Pending"), "demo-client");
            Console.WriteLine($"✓ 订单消息: {orderResult.Success} - {orderResult.Result}");

            // 测试发送心跳消息
            var heartbeatContext = new MessageContext
            {
                Source = "demo-client",
                Destination = "system-service",
                Type = "System.Heartbeat",
                Body = new { Timestamp = DateTime.UtcNow }
            };
            var heartbeatResult = await resonanceAdapter.SendAsync(heartbeatContext);
            Console.WriteLine($"✓ 心跳消息: {heartbeatResult.Success} - {heartbeatResult.Result}");

            // 获取消息统计
            var stats = resonanceAdapter.GetStats();
            Console.WriteLine("\n消息处理统计:");
            Console.WriteLine($"总消息数: {stats.TotalMessages}");
            Console.WriteLine($"成功消息数: {stats.SuccessfulMessages}");
            Console.WriteLine($"失败消息数: {stats.FailedMessages}");
            Console.WriteLine($"重试消息数: {stats.RetriedMessages}");
            Console.WriteLine($"平均处理时间: {stats.AverageProcessingTimeMs:F2}ms");
            Console.WriteLine($"活跃处理器数: {stats.ActiveHandlers}");
            Console.WriteLine($"队列深度: {stats.QueueDepth}");

            // 获取注册的处理器
            var handlers = resonanceAdapter.GetRegisteredHandlers();
            Console.WriteLine("\n注册的消息处理器:");
            foreach (var handler in handlers)
            {
                Console.WriteLine($"- {handler}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            // 停止适配器
            await resonanceAdapter.StopAsync();
            Console.WriteLine("✓ Resonance Adapter 停止成功");

            // 释放资源
            if (serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}