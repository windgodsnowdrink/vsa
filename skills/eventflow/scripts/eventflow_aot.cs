#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
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

namespace EventFlow.AOT
{
    /// <summary>
    /// EventFlow 命令类型枚举
    /// </summary>
    public enum EventFlowCommandType { CreateEvent, GetEvent, ListEvents, PublishEvent, SubscribeEvent, VersionInfo }

    /// <summary>
    /// EventFlow 选项配置
    /// </summary>
    public class EventFlowOptions
    {
        /// <summary>
        /// 事件存储类型
        /// </summary>
        public string EventStoreType { get; set; } = "InMemory";
        
        /// <summary>
        /// 事件存储连接字符串
        /// </summary>
        public string EventStoreConnectionString { get; set; } = "";
        
        /// <summary>
        /// 事件总线类型
        /// </summary>
        public string EventBusType { get; set; } = "InMemory";
        
        /// <summary>
        /// 事件总线连接字符串
        /// </summary>
        public string EventBusConnectionString { get; set; } = "";
        
        /// <summary>
        /// 请求超时时间（毫秒）
        /// </summary>
        public int RequestTimeoutMs { get; set; } = 5000;
        
        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
        
        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;
        
        /// <summary>
        /// 最大事件存储数量
        /// </summary>
        public int MaxEventStoreCount { get; set; } = 10000;
        
        /// <summary>
        /// 是否启用事件压缩
        /// </summary>
        public bool EnableEventCompression { get; set; } = false;
        
        /// <summary>
        /// 是否启用事件加密
        /// </summary>
        public bool EnableEventEncryption { get; set; } = false;
    }

    /// <summary>
    /// 事件数据模型
    /// </summary>
    public class EventData
    {
        /// <summary>
        /// 事件ID
        /// </summary>
        public string EventId { get; set; } = string.Empty;
        
        /// <summary>
        /// 事件类型
        /// </summary>
        public string EventType { get; set; } = string.Empty;
        
        /// <summary>
        /// 事件数据
        /// </summary>
        public string EventDataContent { get; set; } = string.Empty;
        
        /// <summary>
        /// 事件来源
        /// </summary>
        public string Source { get; set; } = string.Empty;
        
        /// <summary>
        /// 事件时间
        /// </summary>
        public DateTime EventTime { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; set; } = 1;
        
        /// <summary>
        /// 相关聚合根ID
        /// </summary>
        public string AggregateId { get; set; } = string.Empty;
        
        /// <summary>
        /// 相关聚合根类型
        /// </summary>
        public string AggregateType { get; set; } = string.Empty;
    }

    /// <summary>
    /// EventFlow 命令结果
    /// </summary>
    public class EventFlowCommandResult
    {
        /// <summary>
        /// 命令是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 命令类型
        /// </summary>
        public EventFlowCommandType CommandType { get; set; }
        
        /// <summary>
        /// 结果数据
        /// </summary>
        public List<string> Results { get; set; } = new List<string>();
        
        /// <summary>
        /// 执行时间（毫秒）
        /// </summary>
        public long ExecutionTimeMs { get; set; }
        
        /// <summary>
        /// 错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// 事件列表
        /// </summary>
        public List<EventData>? Events { get; set; }
        
        /// <summary>
        /// 事件ID
        /// </summary>
        public string? EventId { get; set; }
        
        /// <summary>
        /// 事件类型
        /// </summary>
        public string? EventType { get; set; }
    }

    /// <summary>
    /// EventFlow 服务接口
    /// </summary>
    public interface IEventFlowService
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>命令结果</returns>
        Task<EventFlowCommandResult> ExecuteCommandAsync(EventFlowCommandType commandType, Dictionary<string, string>? parameters = null);
        
        /// <summary>
        /// 创建事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="eventData">事件数据</param>
        /// <param name="source">事件来源</param>
        /// <returns>操作结果</returns>
        Task<EventFlowCommandResult> CreateEventAsync(string eventType, string eventData, string source = "EventFlowAotEngine");
        
        /// <summary>
        /// 获取事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <returns>操作结果</returns>
        Task<EventFlowCommandResult> GetEventAsync(string eventId);
        
        /// <summary>
        /// 列出事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="limit">限制数量</param>
        /// <returns>操作结果</returns>
        Task<EventFlowCommandResult> ListEventsAsync(string? eventType = null, int limit = 10);
        
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="eventData">事件数据</param>
        /// <param name="source">事件来源</param>
        /// <returns>操作结果</returns>
        Task<EventFlowCommandResult> PublishEventAsync(string eventType, string eventData, string source = "EventFlowAotEngine");
        
        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="subscriptionId">订阅ID</param>
        /// <returns>操作结果</returns>
        Task<EventFlowCommandResult> SubscribeEventAsync(string eventType, string subscriptionId);
        
        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        Task<EventFlowCommandResult> GetVersionInfoAsync();
    }

    /// <summary>
    /// EventFlow 服务实现
    /// </summary>
    public class EventFlowService : IEventFlowService
    {
        private readonly EventFlowOptions _options;
        private readonly ILogger<EventFlowService> _logger;
        private readonly Dictionary<string, List<EventData>> _eventStore = new Dictionary<string, List<EventData>>();
        private readonly Dictionary<string, List<string>> _subscriptions = new Dictionary<string, List<string>>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">EventFlow 选项</param>
        /// <param name="logger">日志记录器</param>
        public EventFlowService(IOptions<EventFlowOptions> options, ILogger<EventFlowService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<EventFlowCommandResult> ExecuteCommandAsync(EventFlowCommandType commandType, Dictionary<string, string>? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EventFlowCommandResult
            {
                CommandType = commandType
            };

            try
            {
                switch (commandType)
                {
                    case EventFlowCommandType.CreateEvent:
                        if (parameters?.ContainsKey("eventType") == true && parameters?.ContainsKey("eventData") == true)
                        {
                            string eventType = parameters["eventType"];
                            string eventData = parameters["eventData"];
                            string source = parameters?.ContainsKey("source") == true ? parameters["source"] : "EventFlowAotEngine";
                            result = await CreateEventAsync(eventType, eventData, source);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "EventType and EventData parameters are required";
                        }
                        break;
                    
                    case EventFlowCommandType.GetEvent:
                        if (parameters?.ContainsKey("eventId") == true)
                        {
                            string eventId = parameters["eventId"];
                            result = await GetEventAsync(eventId);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "EventId parameter is required";
                        }
                        break;
                    
                    case EventFlowCommandType.ListEvents:
                        string? listEventType = parameters?.ContainsKey("eventType") == true ? parameters["eventType"] : null;
                        int limit = parameters?.ContainsKey("limit") == true ? int.Parse(parameters["limit"]) : 10;
                        result = await ListEventsAsync(listEventType, limit);
                        break;
                    
                    case EventFlowCommandType.PublishEvent:
                        if (parameters?.ContainsKey("eventType") == true && parameters?.ContainsKey("eventData") == true)
                        {
                            string eventType = parameters["eventType"];
                            string eventData = parameters["eventData"];
                            string source = parameters?.ContainsKey("source") == true ? parameters["source"] : "EventFlowAotEngine";
                            result = await PublishEventAsync(eventType, eventData, source);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "EventType and EventData parameters are required";
                        }
                        break;
                    
                    case EventFlowCommandType.SubscribeEvent:
                        if (parameters?.ContainsKey("eventType") == true && parameters?.ContainsKey("subscriptionId") == true)
                        {
                            string eventType = parameters["eventType"];
                            string subscriptionId = parameters["subscriptionId"];
                            result = await SubscribeEventAsync(eventType, subscriptionId);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "EventType and SubscriptionId parameters are required";
                        }
                        break;
                    
                    case EventFlowCommandType.VersionInfo:
                        result = await GetVersionInfoAsync();
                        break;
                    
                    default:
                        result.Success = false;
                        result.ErrorMessage = $"未知命令类型: {commandType}";
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "执行命令时出错: {CommandType}", commandType);
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<EventFlowCommandResult> CreateEventAsync(string eventType, string eventData, string source = "EventFlowAotEngine")
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EventFlowCommandResult
            {
                CommandType = EventFlowCommandType.CreateEvent,
                EventType = eventType
            };

            try
            {
                _logger.LogInformation("创建事件: {EventType}，来源: {Source}", eventType, source);
                
                // 创建事件
                var eventId = Guid.NewGuid().ToString();
                var eventDataItem = new EventData
                {
                    EventId = eventId,
                    EventType = eventType,
                    EventDataContent = eventData,
                    Source = source,
                    EventTime = DateTime.UtcNow,
                    Version = 1
                };
                
                // 存储事件
                if (!_eventStore.ContainsKey(eventType))
                {
                    _eventStore[eventType] = new List<EventData>();
                }
                
                _eventStore[eventType].Add(eventDataItem);
                
                // 模拟成功结果
                result.Success = true;
                result.EventId = eventId;
                result.Results.Add($"成功创建事件: {eventId}");
                result.Results.Add($"事件类型: {eventType}");
                result.Results.Add($"事件来源: {source}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建事件时出错: {EventType}", eventType);
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<EventFlowCommandResult> GetEventAsync(string eventId)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EventFlowCommandResult
            {
                CommandType = EventFlowCommandType.GetEvent,
                EventId = eventId
            };

            try
            {
                _logger.LogInformation("获取事件: {EventId}", eventId);
                
                // 查找事件
                EventData? foundEvent = null;
                foreach (var events in _eventStore.Values)
                {
                    foundEvent = events.FirstOrDefault(e => e.EventId == eventId);
                    if (foundEvent != null)
                    {
                        break;
                    }
                }
                
                if (foundEvent != null)
                {
                    result.Success = true;
                    result.Events = new List<EventData> { foundEvent };
                    result.Results.Add($"成功获取事件: {eventId}");
                    result.Results.Add($"事件类型: {foundEvent.EventType}");
                    result.Results.Add($"事件时间: {foundEvent.EventTime}");
                    result.Results.Add($"事件来源: {foundEvent.Source}");
                }
                else
                {
                    result.Success = false;
                    result.ErrorMessage = $"未找到事件: {eventId}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取事件时出错: {EventId}", eventId);
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<EventFlowCommandResult> ListEventsAsync(string? eventType = null, int limit = 10)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EventFlowCommandResult
            {
                CommandType = EventFlowCommandType.ListEvents
            };

            try
            {
                _logger.LogInformation("列出事件，类型: {EventType}，限制: {Limit}", eventType ?? "所有", limit);
                
                List<EventData> events = new List<EventData>();
                
                if (eventType != null && _eventStore.ContainsKey(eventType))
                {
                    // 按类型列出事件
                    events = _eventStore[eventType].OrderByDescending(e => e.EventTime).Take(limit).ToList();
                }
                else if (eventType == null)
                {
                    // 列出所有事件
                    events = _eventStore.Values.SelectMany(e => e).OrderByDescending(e => e.EventTime).Take(limit).ToList();
                }
                
                result.Success = true;
                result.Events = events;
                result.Results.Add($"成功获取 {events.Count} 个事件");
                if (eventType != null)
                {
                    result.Results.Add($"事件类型: {eventType}");
                }
                result.Results.Add($"限制数量: {limit}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "列出事件时出错，类型: {EventType}", eventType ?? "所有");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<EventFlowCommandResult> PublishEventAsync(string eventType, string eventData, string source = "EventFlowAotEngine")
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EventFlowCommandResult
            {
                CommandType = EventFlowCommandType.PublishEvent,
                EventType = eventType
            };

            try
            {
                _logger.LogInformation("发布事件: {EventType}，来源: {Source}", eventType, source);
                
                // 先创建事件
                var createResult = await CreateEventAsync(eventType, eventData, source);
                if (!createResult.Success)
                {
                    return createResult;
                }
                
                // 模拟发布事件
                // 实际实现中应使用事件总线发布事件
                
                // 检查订阅
                if (_subscriptions.ContainsKey(eventType))
                {
                    var subscriptionCount = _subscriptions[eventType].Count;
                    result.Results.Add($"事件已发布到 {subscriptionCount} 个订阅者");
                }
                
                result.Success = true;
                result.EventId = createResult.EventId;
                result.Results.Add($"成功发布事件: {createResult.EventId}");
                result.Results.Add($"事件类型: {eventType}");
                result.Results.Add($"事件来源: {source}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "发布事件时出错: {EventType}", eventType);
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<EventFlowCommandResult> SubscribeEventAsync(string eventType, string subscriptionId)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EventFlowCommandResult
            {
                CommandType = EventFlowCommandType.SubscribeEvent,
                EventType = eventType
            };

            try
            {
                _logger.LogInformation("订阅事件: {EventType}，订阅ID: {SubscriptionId}", eventType, subscriptionId);
                
                // 添加订阅
                if (!_subscriptions.ContainsKey(eventType))
                {
                    _subscriptions[eventType] = new List<string>();
                }
                
                if (!_subscriptions[eventType].Contains(subscriptionId))
                {
                    _subscriptions[eventType].Add(subscriptionId);
                    result.Results.Add($"成功订阅事件: {eventType}");
                    result.Results.Add($"订阅ID: {subscriptionId}");
                }
                else
                {
                    result.Results.Add($"已经订阅了事件: {eventType}");
                    result.Results.Add($"订阅ID: {subscriptionId}");
                }
                
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "订阅事件时出错: {EventType}", eventType);
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<EventFlowCommandResult> GetVersionInfoAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EventFlowCommandResult
            {
                CommandType = EventFlowCommandType.VersionInfo
            };

            try
            {
                _logger.LogInformation("获取 EventFlow 版本信息");
                
                // 模拟成功结果
                result.Success = true;
                result.Results.Add("EventFlow AOT Engine");
                result.Results.Add($"版本: 1.0.0");
                result.Results.Add($".NET 版本: {Environment.Version}");
                result.Results.Add($"操作系统: {Environment.OSVersion}");
                result.Results.Add($"架构: {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}");
                result.Results.Add($"AOT 编译: {AppContext.TryGetSwitch("PublishAot", out bool isAot) && isAot}");
                result.Results.Add($"事件存储类型: {_options.EventStoreType}");
                result.Results.Add($"事件总线类型: {_options.EventBusType}");
                result.Results.Add($"最大事件存储数量: {_options.MaxEventStoreCount}");
                result.Results.Add($"启用事件压缩: {_options.EnableEventCompression}");
                result.Results.Add($"启用事件加密: {_options.EnableEventEncryption}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取版本信息时出错");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return await Task.FromResult(result);
        }
    }

    /// <summary>
    /// EventFlow AOT 引擎
    /// </summary>
    public class EventFlowAotEngine
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventFlowAotEngine> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="logger">日志记录器</param>
        public EventFlowAotEngine(IServiceProvider serviceProvider, ILogger<EventFlowAotEngine> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// 执行命令行操作
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出码</returns>
        public async Task<int> ExecuteCommandLineAsync(string[] args)
        {
            _logger.LogInformation("EventFlow AOT Engine 启动，参数: {Args}", string.Join(" ", args));
            
            if (args.Length == 0)
            {
                ShowHelp();
                return 0;
            }

            var command = args[0].ToLower();
            var eventFlowService = _serviceProvider.GetRequiredService<IEventFlowService>();
            EventFlowCommandResult? result = null;

            try
            {
                switch (command)
                {
                    case "create":
                    case "createevent":
                        if (args.Length < 3)
                        {
                            Console.WriteLine("错误: 需要提供事件类型和事件数据");
                            return 1;
                        }
                        string eventType = args[1];
                        string eventData = args[2];
                        string source = args.Length > 3 ? args[3] : "EventFlowAotEngine";
                        result = await eventFlowService.CreateEventAsync(eventType, eventData, source);
                        break;
                    
                    case "get":
                    case "getevent":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("错误: 需要提供事件ID");
                            return 1;
                        }
                        string eventId = args[1];
                        result = await eventFlowService.GetEventAsync(eventId);
                        break;
                    
                    case "list":
                    case "listevents":
                        string? listEventType = args.Length > 1 ? args[1] : null;
                        int limit = args.Length > 2 ? int.Parse(args[2]) : 10;
                        result = await eventFlowService.ListEventsAsync(listEventType, limit);
                        break;
                    
                    case "publish":
                    case "publishevent":
                        if (args.Length < 3)
                        {
                            Console.WriteLine("错误: 需要提供事件类型和事件数据");
                            return 1;
                        }
                        string publishEventType = args[1];
                        string publishEventData = args[2];
                        string publishSource = args.Length > 3 ? args[3] : "EventFlowAotEngine";
                        result = await eventFlowService.PublishEventAsync(publishEventType, publishEventData, publishSource);
                        break;
                    
                    case "subscribe":
                    case "subscribeevent":
                        if (args.Length < 3)
                        {
                            Console.WriteLine("错误: 需要提供事件类型和订阅ID");
                            return 1;
                        }
                        string subscribeEventType = args[1];
                        string subscriptionId = args[2];
                        result = await eventFlowService.SubscribeEventAsync(subscribeEventType, subscriptionId);
                        break;
                    
                    case "version":
                        result = await eventFlowService.GetVersionInfoAsync();
                        break;
                    
                    case "help":
                    case "--help":
                    case "-h":
                        ShowHelp();
                        return 0;
                    
                    default:
                        Console.WriteLine($"错误: 未知命令 '{command}'");
                        ShowHelp();
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
                return 1;
            }

            // 显示结果
            if (result != null)
            {
                Console.WriteLine($"\n命令执行结果: {(result.Success ? "成功" : "失败")}");
                Console.WriteLine($"执行时间: {result.ExecutionTimeMs} ms");
                
                if (!result.Success && !string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Console.WriteLine($"错误信息: {result.ErrorMessage}");
                }
                
                foreach (var item in result.Results)
                {
                    Console.WriteLine($"- {item}");
                }
                
                if (result.Events != null && result.Events.Any())
                {
                    Console.WriteLine($"\n事件详情:");
                    foreach (var evt in result.Events)
                    {
                        Console.WriteLine($"  事件ID: {evt.EventId}");
                        Console.WriteLine($"  事件类型: {evt.EventType}");
                        Console.WriteLine($"  事件时间: {evt.EventTime:yyyy-MM-dd HH:mm:ss}");
                        Console.WriteLine($"  事件来源: {evt.Source}");
                        Console.WriteLine($"  版本: {evt.Version}");
                        Console.WriteLine($"  事件数据: {evt.EventDataContent.Substring(0, Math.Min(50, evt.EventDataContent.Length))}{(evt.EventDataContent.Length > 50 ? "..." : "")}");
                        Console.WriteLine();
                    }
                }
            }
            
            return result?.Success == true ? 0 : 1;
        }
        
        /// <summary>
        /// 显示帮助信息
        /// </summary>
        private void ShowHelp()
        {
            Console.WriteLine("EventFlow AOT Engine - .NET 10 AOT 编译的事件流引擎");
            Console.WriteLine();
            Console.WriteLine("用法: eventflow_aot <命令> [参数]");
            Console.WriteLine();
            Console.WriteLine("命令:");
            Console.WriteLine("  createevent <eventType> <eventData> [source]   创建事件");
            Console.WriteLine("  getevent <eventId>                             获取事件");
            Console.WriteLine("  listevents [eventType] [limit]                 列出事件");
            Console.WriteLine("  publishevent <eventType> <eventData> [source]  发布事件");
            Console.WriteLine("  subscribeevent <eventType> <subscriptionId>    订阅事件");
            Console.WriteLine("  version                                        显示版本信息");
            Console.WriteLine("  help                                           显示帮助信息");
            Console.WriteLine();
            Console.WriteLine("别名:");
            Console.WriteLine("  createevent -> create");
            Console.WriteLine("  getevent -> get");
            Console.WriteLine("  listevents -> list");
            Console.WriteLine("  publishevent -> publish");
            Console.WriteLine("  subscribeevent -> subscribe");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine("  eventflow_aot create UserCreated '{\"userId\":\"123\",\"name\":\"测试用户\"}'");
            Console.WriteLine("  eventflow_aot get 550e8400-e29b-41d4-a716-446655440000");
            Console.WriteLine("  eventflow_aot list UserCreated 5");
            Console.WriteLine("  eventflow_aot publish OrderPlaced '{\"orderId\":\"456\",\"amount\":100}'");
            Console.WriteLine("  eventflow_aot subscribe UserCreated Subscriber1");
            Console.WriteLine();
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
        /// <returns>退出码</returns>
        public static async Task<int> Main(string[] args)
        {
            // 创建主机构建器
            var builder = Host.CreateApplicationBuilder(args);
            
            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            
            // 配置选项
            builder.Services.Configure<EventFlowOptions>(builder.Configuration.GetSection("EventFlow"));
            
            // 注册服务
            builder.Services.AddSingleton<IEventFlowService, EventFlowService>();
            builder.Services.AddSingleton<EventFlowAotEngine>();
            
            // 构建主机
            var host = builder.Build();
            
            // 获取引擎实例
            var engine = host.Services.GetRequiredService<EventFlowAotEngine>();
            
            // 执行命令行操作
            return await engine.ExecuteCommandLineAsync(args);
        }
    }
}