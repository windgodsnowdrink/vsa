#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Channels;
using System.Diagnostics;

namespace Disruptor.AOT
{
    /// <summary>
    /// Disruptor事件类型
    /// </summary>
    public enum DisruptorEventType
    {
        /// <summary>
        /// 普通事件
        /// </summary>
        Normal,
        /// <summary>
        /// 优先级事件
        /// </summary>
        Priority,
        /// <summary>
        /// 紧急事件
        /// </summary>
        Emergency,
        /// <summary>
        /// 系统事件
        /// </summary>
        System
    }
    
    /// <summary>
    /// Disruptor事件数据
    /// </summary>
    public class DisruptorEvent
    {
        /// <summary>
        /// 事件ID
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// 事件类型
        /// </summary>
        public DisruptorEventType Type { get; set; }
        
        /// <summary>
        /// 事件数据
        /// </summary>
        public string? Data { get; set; }
        
        /// <summary>
        /// 事件创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// 事件优先级（0-100，值越大优先级越高）
        /// </summary>
        public int Priority { get; set; }
        
        /// <summary>
        /// 事件来源
        /// </summary>
        public string? Source { get; set; }
        
        /// <summary>
        /// 事件标签
        /// </summary>
        public List<string>? Tags { get; set; }
    }
    
    /// <summary>
    /// Disruptor选项配置
    /// </summary>
    public class DisruptorOptions
    {
        /// <summary>
        /// 环形缓冲区大小（必须是2的幂）
        /// </summary>
        public int RingBufferSize { get; set; } = 4096;
        
        /// <summary>
        /// 消费者数量
        /// </summary>
        public int ConsumerCount { get; set; } = 1;
        
        /// <summary>
        /// 是否启用批量处理
        /// </summary>
        public bool EnableBatching { get; set; } = true;
        
        /// <summary>
        /// 批量处理大小
        /// </summary>
        public int BatchSize { get; set; } = 64;
        
        /// <summary>
        /// 是否启用优先级队列
        /// </summary>
        public bool EnablePriorityQueue { get; set; } = false;
        
        /// <summary>
        /// 是否启用事件跟踪
        /// </summary>
        public bool EnableEventTracking { get; set; } = true;
        
        /// <summary>
        /// 事件超时时间（毫秒）
        /// </summary>
        public int EventTimeoutMs { get; set; } = 5000;
        
        /// <summary>
        /// 最大重试次数
        /// </summary>
        public int MaxRetryCount { get; set; } = 3;
        
        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
        
        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;
    }
    
    /// <summary>
    /// Disruptor事件处理结果
    /// </summary>
    public class DisruptorResult
    {
        /// <summary>
        /// 处理是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 事件ID
        /// </summary>
        public Guid EventId { get; set; }
        
        /// <summary>
        /// 处理结果数据
        /// </summary>
        public object? ResultData { get; set; }
        
        /// <summary>
        /// 错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// 执行时间（毫秒）
        /// </summary>
        public long ExecutionTimeMs { get; set; }
        
        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; }
    }
    
    /// <summary>
    /// Disruptor状态信息
    /// </summary>
    public class DisruptorStatus
    {
        /// <summary>
        /// 服务是否正常运行
        /// </summary>
        public bool IsRunning { get; set; }
        
        /// <summary>
        /// 已处理的事件数
        /// </summary>
        public long ProcessedEvents { get; set; }
        
        /// <summary>
        /// 成功处理的事件数
        /// </summary>
        public long SuccessfulEvents { get; set; }
        
        /// <summary>
        /// 失败处理的事件数
        /// </summary>
        public long FailedEvents { get; set; }
        
        /// <summary>
        /// 平均处理时间（毫秒）
        /// </summary>
        public double AverageProcessingTimeMs { get; set; }
        
        /// <summary>
        /// 当前环形缓冲区使用率（%）
        /// </summary>
        public double RingBufferUsage { get; set; }
        
        /// <summary>
        /// 服务启动时间
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// 当前活跃消费者数量
        /// </summary>
        public int ActiveConsumers { get; set; }
        
        /// <summary>
        /// 环形缓冲区大小
        /// </summary>
        public int RingBufferSize { get; set; }
    }
    
    /// <summary>
    /// Disruptor服务接口
    /// </summary>
    public interface IDisruptorService
    {
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        /// <returns>事件处理结果</returns>
        Task<DisruptorResult> PublishEventAsync(DisruptorEvent eventData);
        
        /// <summary>
        /// 批量发布事件
        /// </summary>
        /// <param name="events">事件列表</param>
        /// <returns>批量处理结果</returns>
        Task<List<DisruptorResult>> PublishEventsAsync(List<DisruptorEvent> events);
        
        /// <summary>
        /// 发布优先级事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        /// <param name="priority">优先级（0-100）</param>
        /// <returns>事件处理结果</returns>
        Task<DisruptorResult> PublishPriorityEventAsync(DisruptorEvent eventData, int priority = 100);
        
        /// <summary>
        /// 获取Disruptor状态
        /// </summary>
        /// <returns>状态信息</returns>
        Task<DisruptorStatus> GetStatusAsync();
        
        /// <summary>
        /// 重置Disruptor状态
        /// </summary>
        /// <returns>重置结果</returns>
        Task<bool> ResetStatusAsync();
        
        /// <summary>
        /// 启动Disruptor服务
        /// </summary>
        /// <returns>启动结果</returns>
        Task<bool> StartAsync();
        
        /// <summary>
        /// 停止Disruptor服务
        /// </summary>
        /// <returns>停止结果</returns>
        Task<bool> StopAsync();
    }
    
    /// <summary>
    /// Disruptor服务实现
    /// </summary>
    public class DisruptorService : IDisruptorService
    {
        private readonly ILogger<DisruptorService> _logger;
        private readonly DisruptorOptions _options;
        private readonly Channel<DisruptorEvent> _eventChannel;
        private readonly CancellationTokenSource _cts;
        private readonly List<Task> _consumerTasks;
        private long _processedEvents;
        private long _successfulEvents;
        private long _failedEvents;
        private long _totalProcessingTime;
        private readonly DateTime _startTime;
        private bool _isRunning;
        private readonly SemaphoreSlim _semaphore;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="options">配置选项</param>
        public DisruptorService(ILogger<DisruptorService> logger, IOptions<DisruptorOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _cts = new CancellationTokenSource();
            _consumerTasks = new List<Task>();
            _startTime = DateTime.UtcNow;
            _semaphore = new SemaphoreSlim(1, 1);
            
            // 确保环形缓冲区大小是2的幂
            _options.RingBufferSize = GetNextPowerOfTwo(_options.RingBufferSize);
            
            // 创建通道
            _eventChannel = Channel.CreateBounded<DisruptorEvent>(new BoundedChannelOptions(_options.RingBufferSize)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = _options.ConsumerCount == 1,
                SingleWriter = false
            });
            
            _logger.LogInformation("DisruptorService初始化成功，配置选项：RingBufferSize={RingBufferSize}, ConsumerCount={ConsumerCount}, EnableBatching={EnableBatching}",
                _options.RingBufferSize, _options.ConsumerCount, _options.EnableBatching);
        }
        
        /// <summary>
        /// 获取下一个2的幂
        /// </summary>
        private int GetNextPowerOfTwo(int value)
        {
            if (value <= 0) return 1;
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
        }
        
        /// <summary>
        /// 发布事件
        /// </summary>
        public async Task<DisruptorResult> PublishEventAsync(DisruptorEvent eventData)
        {
            if (!_isRunning)
            {
                return new DisruptorResult
                {
                    Success = false,
                    EventId = eventData.Id,
                    ErrorMessage = "Disruptor服务未运行"
                };
            }
            
            var result = new DisruptorResult
            {
                EventId = eventData.Id,
                Success = true
            };
            
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                await _eventChannel.Writer.WriteAsync(eventData, _cts.Token);
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                
                _logger.LogDebug("事件发布成功：EventId={EventId}, Type={EventType}", eventData.Id, eventData.Type);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                
                _logger.LogError(ex, "事件发布失败：EventId={EventId}", eventData.Id);
            }
            finally
            {
                stopwatch.Stop();
            }
            
            return result;
        }
        
        /// <summary>
        /// 批量发布事件
        /// </summary>
        public async Task<List<DisruptorResult>> PublishEventsAsync(List<DisruptorEvent> events)
        {
            var results = new List<DisruptorResult>();
            
            foreach (var evnt in events)
            {
                var result = await PublishEventAsync(evnt);
                results.Add(result);
            }
            
            return results;
        }
        
        /// <summary>
        /// 发布优先级事件
        /// </summary>
        public async Task<DisruptorResult> PublishPriorityEventAsync(DisruptorEvent eventData, int priority = 100)
        {
            eventData.Priority = Math.Clamp(priority, 0, 100);
            eventData.Type = DisruptorEventType.Priority;
            
            return await PublishEventAsync(eventData);
        }
        
        /// <summary>
        /// 获取Disruptor状态
        /// </summary>
        public async Task<DisruptorStatus> GetStatusAsync()
        {
            await Task.CompletedTask;
            
            var processed = Interlocked.Read(ref _processedEvents);
            var successful = Interlocked.Read(ref _successfulEvents);
            var failed = Interlocked.Read(ref _failedEvents);
            var totalTime = Interlocked.Read(ref _totalProcessingTime);
            
            var averageTime = processed > 0 ? (double)totalTime / processed : 0;
            
            var status = new DisruptorStatus
            {
                IsRunning = _isRunning,
                ProcessedEvents = processed,
                SuccessfulEvents = successful,
                FailedEvents = failed,
                AverageProcessingTimeMs = Math.Round(averageTime, 2),
                RingBufferUsage = CalculateRingBufferUsage(),
                StartTime = _startTime,
                ActiveConsumers = _consumerTasks.Count(t => !t.IsCompleted),
                RingBufferSize = _options.RingBufferSize
            };
            
            return status;
        }
        
        /// <summary>
        /// 计算环形缓冲区使用率
        /// </summary>
        private double CalculateRingBufferUsage()
        {
            if (_eventChannel.Reader.Count == 0) return 0;
            return (double)_eventChannel.Reader.Count / _options.RingBufferSize * 100;
        }
        
        /// <summary>
        /// 重置Disruptor状态
        /// </summary>
        public async Task<bool> ResetStatusAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                Interlocked.Exchange(ref _processedEvents, 0);
                Interlocked.Exchange(ref _successfulEvents, 0);
                Interlocked.Exchange(ref _failedEvents, 0);
                Interlocked.Exchange(ref _totalProcessingTime, 0);
                
                _logger.LogInformation("Disruptor状态已重置");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "重置Disruptor状态失败");
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        /// <summary>
        /// 启动Disruptor服务
        /// </summary>
        public async Task<bool> StartAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_isRunning)
                {
                    _logger.LogWarning("Disruptor服务已在运行");
                    return true;
                }
                
                // 创建消费者任务
                for (int i = 0; i < _options.ConsumerCount; i++)
                {
                    var consumerId = i + 1;
                    _consumerTasks.Add(Task.Run(() => ConsumerLoopAsync(consumerId)));
                }
                
                _isRunning = true;
                _logger.LogInformation("Disruptor服务启动成功，消费者数量：{ConsumerCount}", _options.ConsumerCount);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "启动Disruptor服务失败");
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        /// <summary>
        /// 停止Disruptor服务
        /// </summary>
        public async Task<bool> StopAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!_isRunning)
                {
                    _logger.LogWarning("Disruptor服务已停止");
                    return true;
                }
                
                _cts.Cancel();
                
                // 等待所有消费者任务完成
                await Task.WhenAll(_consumerTasks.Where(t => !t.IsCompleted));
                
                _eventChannel.Writer.Complete();
                
                _isRunning = false;
                _logger.LogInformation("Disruptor服务停止成功");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "停止Disruptor服务失败");
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        /// <summary>
        /// 消费者循环
        /// </summary>
        private async Task ConsumerLoopAsync(int consumerId)
        {
            _logger.LogInformation("消费者启动：ConsumerId={ConsumerId}", consumerId);
            
            try
            {
                if (_options.EnableBatching)
                {
                    await BatchConsumeAsync(consumerId);
                }
                else
                {
                    await SingleConsumeAsync(consumerId);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("消费者取消：ConsumerId={ConsumerId}", consumerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "消费者异常：ConsumerId={ConsumerId}", consumerId);
            }
            finally
            {
                _logger.LogInformation("消费者停止：ConsumerId={ConsumerId}", consumerId);
            }
        }
        
        /// <summary>
        /// 单个消费
        /// </summary>
        private async Task SingleConsumeAsync(int consumerId)
        {
            await foreach (var evnt in _eventChannel.Reader.ReadAllAsync(_cts.Token))
            {
                await ProcessEventAsync(evnt, consumerId);
            }
        }
        
        /// <summary>
        /// 批量消费
        /// </summary>
        private async Task BatchConsumeAsync(int consumerId)
        {
            var batch = new List<DisruptorEvent>();
            
            while (!_cts.Token.IsCancellationRequested)
            {
                batch.Clear();
                
                // 尝试读取一批事件
                for (int i = 0; i < _options.BatchSize; i++)
                {
                    if (_eventChannel.Reader.TryRead(out var evnt))
                    {
                        batch.Add(evnt);
                    }
                    else
                    {
                        break;
                    }
                }
                
                if (batch.Count > 0)
                {
                    _logger.LogDebug("批量消费事件：ConsumerId={ConsumerId}, BatchSize={BatchSize}", consumerId, batch.Count);
                    
                    // 并行处理批量事件
                    await Task.WhenAll(batch.Select(evnt => ProcessEventAsync(evnt, consumerId)));
                }
                else
                {
                    // 没有事件，短暂等待
                    await Task.Delay(10, _cts.Token);
                }
            }
        }
        
        /// <summary>
        /// 处理事件
        /// </summary>
        private async Task ProcessEventAsync(DisruptorEvent evnt, int consumerId)
        {
            var stopwatch = Stopwatch.StartNew();
            int retryCount = 0;
            bool success = false;
            
            try
            {
                // 模拟事件处理
                await SimulateEventProcessingAsync(evnt);
                success = true;
                
                _logger.LogDebug("事件处理成功：ConsumerId={ConsumerId}, EventId={EventId}, Type={EventType}, Priority={Priority}",
                    consumerId, evnt.Id, evnt.Type, evnt.Priority);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "事件处理失败：ConsumerId={ConsumerId}, EventId={EventId}", consumerId, evnt.Id);
                
                // 重试逻辑
                while (retryCount < _options.MaxRetryCount)
                {
                    retryCount++;
                    _logger.LogWarning("事件重试：ConsumerId={ConsumerId}, EventId={EventId}, RetryCount={RetryCount}",
                        consumerId, evnt.Id, retryCount);
                    
                    try
                    {
                        await SimulateEventProcessingAsync(evnt);
                        success = true;
                        break;
                    }
                    catch (Exception retryEx)
                    {
                        _logger.LogError(retryEx, "事件重试失败：ConsumerId={ConsumerId}, EventId={EventId}, RetryCount={RetryCount}",
                            consumerId, evnt.Id, retryCount);
                    }
                }
            }
            finally
            {
                stopwatch.Stop();
                
                // 更新统计信息
                Interlocked.Increment(ref _processedEvents);
                if (success)
                {
                    Interlocked.Increment(ref _successfulEvents);
                }
                else
                {
                    Interlocked.Increment(ref _failedEvents);
                }
                Interlocked.Add(ref _totalProcessingTime, stopwatch.ElapsedMilliseconds);
            }
        }
        
        /// <summary>
        /// 模拟事件处理
        /// </summary>
        private async Task SimulateEventProcessingAsync(DisruptorEvent evnt)
        {
            // 模拟事件处理延迟
            await Task.Delay(10, _cts.Token);
            
            // 可以根据事件类型执行不同的处理逻辑
            switch (evnt.Type)
            {
                case DisruptorEventType.Normal:
                    // 普通事件处理
                    break;
                case DisruptorEventType.Priority:
                    // 优先级事件处理
                    break;
                case DisruptorEventType.Emergency:
                    // 紧急事件处理
                    break;
                case DisruptorEventType.System:
                    // 系统事件处理
                    break;
            }
        }
    }
    
    /// <summary>
    /// Disruptor AOT引擎
    /// </summary>
    public class DisruptorAotEngine
    {
        private readonly ILogger<DisruptorAotEngine> _logger;
        private readonly IDisruptorService _disruptorService;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="disruptorService">Disruptor服务</param>
        public DisruptorAotEngine(ILogger<DisruptorAotEngine> logger, IDisruptorService disruptorService)
        {
            _logger = logger;
            _disruptorService = disruptorService;
            
            _logger.LogInformation("DisruptorAotEngine初始化成功");
        }
        
        /// <summary>
        /// 发布事件
        /// </summary>
        public async Task<DisruptorResult> PublishEventAsync(DisruptorEvent eventData)
        {
            return await _disruptorService.PublishEventAsync(eventData);
        }
        
        /// <summary>
        /// 批量发布事件
        /// </summary>
        public async Task<List<DisruptorResult>> PublishEventsAsync(List<DisruptorEvent> events)
        {
            return await _disruptorService.PublishEventsAsync(events);
        }
        
        /// <summary>
        /// 发布优先级事件
        /// </summary>
        public async Task<DisruptorResult> PublishPriorityEventAsync(DisruptorEvent eventData, int priority = 100)
        {
            return await _disruptorService.PublishPriorityEventAsync(eventData, priority);
        }
        
        /// <summary>
        /// 获取Disruptor状态
        /// </summary>
        public async Task<DisruptorStatus> GetStatusAsync()
        {
            return await _disruptorService.GetStatusAsync();
        }
        
        /// <summary>
        /// 重置Disruptor状态
        /// </summary>
        public async Task<bool> ResetStatusAsync()
        {
            return await _disruptorService.ResetStatusAsync();
        }
        
        /// <summary>
        /// 启动Disruptor服务
        /// </summary>
        public async Task<bool> StartAsync()
        {
            return await _disruptorService.StartAsync();
        }
        
        /// <summary>
        /// 停止Disruptor服务
        /// </summary>
        public async Task<bool> StopAsync()
        {
            return await _disruptorService.StopAsync();
        }
    }
    
    /// <summary>
    /// Disruptor扩展
    /// </summary>
    public static class DisruptorExtensions
    {
        /// <summary>
        /// 注册Disruptor服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddDisruptor(this IServiceCollection services)
        {
            services.AddSingleton<IDisruptorService, DisruptorService>();
            services.AddSingleton<DisruptorAotEngine>();
            
            return services;
        }
        
        /// <summary>
        /// 注册Disruptor服务并配置选项
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configureOptions">配置选项的委托</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddDisruptor(this IServiceCollection services, Action<DisruptorOptions> configureOptions)
        {
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }
            
            services.Configure(configureOptions);
            services.AddDisruptor();
            
            return services;
        }
    }
    
    /// <summary>
    /// 主程序
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 主入口点
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码</returns>
        public static async Task<int> Main(string[] args)
        {
            // 构建主机
            var builder = Host.CreateApplicationBuilder(args);
            
            // 配置Disruptor选项
            builder.Configuration.AddJsonFile("disruptor_aot.setting.json", optional: true);
            builder.Services.Configure<DisruptorOptions>(builder.Configuration.GetSection("Disruptor"));
            
            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);
            
            // 注册服务
            builder.Services.AddDisruptor();
            
            // 构建主机
            var host = builder.Build();
            var serviceProvider = host.Services;
            
            // 获取引擎实例
            var engine = serviceProvider.GetRequiredService<DisruptorAotEngine>();
            
            // 解析命令行参数
            if (args.Length < 1)
            {
                Console.WriteLine("用法:");
                Console.WriteLine("  disruptor_aot.exe <command> [arguments]");
                Console.WriteLine("  ");
                Console.WriteLine("命令:");
                Console.WriteLine("  start    启动Disruptor服务");
                Console.WriteLine("  stop     停止Disruptor服务");
                Console.WriteLine("  status   获取Disruptor服务状态");
                Console.WriteLine("  reset    重置Disruptor服务状态");
                Console.WriteLine("  demo     运行Disruptor演示");
                Console.WriteLine("  ");
                Console.WriteLine("示例:");
                Console.WriteLine("  disruptor_aot.exe start");
                Console.WriteLine("  disruptor_aot.exe status");
                Console.WriteLine("  disruptor_aot.exe demo");
                return 1;
            }
            
            try
            {
                string command = args[0].ToLower();
                
                switch (command)
                {
                    case "start":
                        Console.WriteLine("启动Disruptor服务...");
                        var startResult = await engine.StartAsync();
                        Console.WriteLine($"启动结果: {(startResult ? "成功" : "失败"}");
                        return startResult ? 0 : 1;
                        
                    case "stop":
                        Console.WriteLine("停止Disruptor服务...");
                        var stopResult = await engine.StopAsync();
                        Console.WriteLine($"停止结果: {(stopResult ? "成功" : "失败"}");
                        return stopResult ? 0 : 1;
                        
                    case "status":
                        var status = await engine.GetStatusAsync();
                        Console.WriteLine("Disruptor服务状态:");
                        Console.WriteLine($"  运行状态: {status.IsRunning ? "正常" : "异常"}");
                        Console.WriteLine($"  已处理事件: {status.ProcessedEvents}");
                        Console.WriteLine($"  成功事件: {status.SuccessfulEvents}");
                        Console.WriteLine($"  失败事件: {status.FailedEvents}");
                        Console.WriteLine($"  平均处理时间: {status.AverageProcessingTimeMs} ms");
                        Console.WriteLine($"  环形缓冲区使用率: {status.RingBufferUsage:F2}%");
                        Console.WriteLine($"  活跃消费者: {status.ActiveConsumers}");
                        Console.WriteLine($"  环形缓冲区大小: {status.RingBufferSize}");
                        Console.WriteLine($"  服务启动时间: {status.StartTime.ToLocalTime()}");
                        return 0;
                        
                    case "reset":
                        var resetResult = await engine.ResetStatusAsync();
                        Console.WriteLine($"重置服务状态: {(resetResult ? "成功" : "失败"}");
                        return resetResult ? 0 : 1;
                        
                    case "demo":
                        Console.WriteLine("运行Disruptor演示...");
                        
                        // 启动服务
                        await engine.StartAsync();
                        
                        // 发布测试事件
                        Console.WriteLine("\n1. 发布普通事件...");
                        var normalEvent = new DisruptorEvent
                        {
                            Id = Guid.NewGuid(),
                            Type = DisruptorEventType.Normal,
                            Data = "普通事件数据",
                            CreatedAt = DateTime.UtcNow,
                            Source = "DemoApp",
                            Tags = new List<string> { "demo", "normal" }
                        };
                        var normalResult = await engine.PublishEventAsync(normalEvent);
                        Console.WriteLine($"   结果: {normalResult.Success ? "成功" : "失败"}");
                        
                        // 发布优先级事件
                        Console.WriteLine("\n2. 发布优先级事件...");
                        var priorityEvent = new DisruptorEvent
                        {
                            Id = Guid.NewGuid(),
                            Type = DisruptorEventType.Priority,
                            Data = "优先级事件数据",
                            CreatedAt = DateTime.UtcNow,
                            Source = "DemoApp",
                            Tags = new List<string> { "demo", "priority" }
                        };
                        var priorityResult = await engine.PublishPriorityEventAsync(priorityEvent, 90);
                        Console.WriteLine($"   结果: {priorityResult.Success ? "成功" : "失败"}");
                        
                        // 批量发布事件
                        Console.WriteLine("\n3. 批量发布事件...");
                        var batchEvents = new List<DisruptorEvent>();
                        for (int i = 0; i < 10; i++)
                        {
                            batchEvents.Add(new DisruptorEvent
                            {
                                Id = Guid.NewGuid(),
                                Type = DisruptorEventType.Normal,
                                Data = $"批量事件{i}",
                                CreatedAt = DateTime.UtcNow,
                                Source = "DemoApp",
                                Tags = new List<string> { "demo", "batch" }
                            });
                        }
                        var batchResults = await engine.PublishEventsAsync(batchEvents);
                        var successfulBatch = batchResults.Count(r => r.Success);
                        Console.WriteLine($"   批量发布结果: 成功 {successfulBatch}/{batchEvents.Count}");
                        
                        // 查看状态
                        Console.WriteLine("\n4. 查看服务状态...");
                        var demoStatus = await engine.GetStatusAsync();
                        Console.WriteLine($"   已处理事件: {demoStatus.ProcessedEvents}");
                        Console.WriteLine($"   成功事件: {demoStatus.SuccessfulEvents}");
                        Console.WriteLine($"   环形缓冲区使用率: {demoStatus.RingBufferUsage:F2}%");
                        
                        // 停止服务
                        await engine.StopAsync();
                        
                        Console.WriteLine("\n演示完成！");
                        return 0;
                        
                    default:
                        Console.WriteLine($"未知命令: {command}");
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
                return 1;
            }
        }
    }
}"}