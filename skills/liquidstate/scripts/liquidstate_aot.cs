#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Caching.Memory@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.CommandLine@2.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
#:property Optimize=true
#:property PublishTrimmed=true
#:property PublishAot=true
#:property IncludeNativeLibrariesForSelfExtract=true
#:property EnableCppCodeGen=true

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LiquidStateAot
{
    /// <summary>
    /// 状态机接口
    /// </summary>
    public interface IStateMachine<TState, TEvent>
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        TState CurrentState { get; }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="@event">事件</param>
        /// <returns>是否成功触发</returns>
        Task<bool> FireAsync(TEvent @event);

        /// <summary>
        /// 启动状态机
        /// </summary>
        /// <param name="initialState">初始状态</param>
        void Start(TState initialState);

        /// <summary>
        /// 停止状态机
        /// </summary>
        void Stop();

        /// <summary>
        /// 重置状态机
        /// </summary>
        /// <param name="initialState">新的初始状态</param>
        void Reset(TState initialState);
    }

    /// <summary>
    /// 状态转换操作
    /// </summary>
    public class TransitionAction<TState, TEvent>
    {
        /// <summary>
        /// 源状态
        /// </summary>
        public TState SourceState { get; set; }

        /// <summary>
        /// 目标状态
        /// </summary>
        public TState TargetState { get; set; }

        /// <summary>
        /// 触发事件
        /// </summary>
        public TEvent Event { get; set; }

        /// <summary>
        /// 转换动作
        /// </summary>
        public Func<Task> Action { get; set; }

        /// <summary>
        /// 转换条件
        /// </summary>
        public Func<bool> Condition { get; set; }
    }

    /// <summary>
    /// 状态机配置
    /// </summary>
    public class StateMachineConfig<TState, TEvent>
    {
        /// <summary>
        /// 状态转换配置
        /// </summary>
        public List<TransitionAction<TState, TEvent>> Transitions { get; set; } = new();

        /// <summary>
        /// 状态进入动作
        /// </summary>
        public Dictionary<TState, Func<Task>> EntryActions { get; set; } = new();

        /// <summary>
        /// 状态退出动作
        /// </summary>
        public Dictionary<TState, Func<Task>> ExitActions { get; set; } = new();

        /// <summary>
        /// 全局错误处理
        /// </summary>
        public Action<Exception> ErrorHandler { get; set; }

        /// <summary>
        /// 是否启用日志
        /// </summary>
        public bool EnableLogging { get; set; } = true;

        /// <summary>
        /// 最大并发事件数
        /// </summary>
        public int MaxConcurrentEvents { get; set; } = 1;

        /// <summary>
        /// 事件队列大小
        /// </summary>
        public int EventQueueSize { get; set; } = 100;

        /// <summary>
        /// 是否启用Channel事件处理
        /// </summary>
        public bool EnableChannelProcessing { get; set; } = true;

        /// <summary>
        /// Channel满时的行为
        /// </summary>
        public BoundedChannelFullMode ChannelFullMode { get; set; } = BoundedChannelFullMode.Wait;
    }

    /// <summary>
    /// 状态机实现
    /// </summary>
    public class StateMachine<TState, TEvent> : IStateMachine<TState, TEvent>
    {
        private readonly StateMachineConfig<TState, TEvent> _config;
        private readonly ILogger<StateMachine<TState, TEvent>> _logger;
        private readonly IMemoryCache _cache;
        private TState _currentState;
        private bool _isRunning;
        private readonly object _lock = new();
        private Channel<TEvent> _eventChannel;
        private Task _processingTask;
        private CancellationTokenSource _cts;

        /// <summary>
        /// 当前状态
        /// </summary>
        public TState CurrentState => _currentState;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config">状态机配置</param>
        /// <param name="logger">日志记录器</param>
        /// <param name="cache">内存缓存</param>
        public StateMachine(StateMachineConfig<TState, TEvent> config, ILogger<StateMachine<TState, TEvent>> logger, IMemoryCache cache)
        {
            _config = config;
            _logger = logger;
            _cache = cache;
            
            if (_config.EnableChannelProcessing)
            {
                InitializeChannel();
            }
        }

        /// <summary>
        /// 初始化事件通道
        /// </summary>
        private void InitializeChannel()
        {
            var options = new BoundedChannelOptions(_config.EventQueueSize)
            {
                FullMode = _config.ChannelFullMode,
                SingleReader = _config.MaxConcurrentEvents == 1,
                SingleWriter = false
            };
            
            _eventChannel = Channel.CreateBounded<TEvent>(options);
            _cts = new CancellationTokenSource();
            _processingTask = ProcessEventsAsync(_cts.Token);
        }

        /// <summary>
        /// 处理事件通道中的事件
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        private async Task ProcessEventsAsync(CancellationToken cancellationToken)
        {
            try
            {
                await foreach (var @event in _eventChannel.Reader.ReadAllAsync(cancellationToken))
                {
                    await ProcessEventAsync(@event);
                }
            }
            catch (OperationCanceledException)
            {
                // 正常取消
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        /// <summary>
        /// 启动状态机
        /// </summary>
        /// <param name="initialState">初始状态</param>
        public void Start(TState initialState)
        {
            lock (_lock)
            {
                if (_isRunning)
                {
                    _logger.LogWarning("状态机已经在运行中");
                    return;
                }

                _currentState = initialState;
                _isRunning = true;

                if (_config.EnableLogging)
                {
                    _logger.LogInformation($"状态机已启动，初始状态: {initialState}");
                }

                // 执行初始状态的进入动作
                if (_config.EntryActions.TryGetValue(initialState, out var entryAction))
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await entryAction();
                        }
                        catch (Exception ex)
                        {
                            HandleError(ex);
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 停止状态机
        /// </summary>
        public void Stop()
        {
            lock (_lock)
            {
                if (!_isRunning)
                {
                    _logger.LogWarning("状态机已经停止");
                    return;
                }

                _isRunning = false;

                // 清理Channel资源
                if (_config.EnableChannelProcessing && _cts != null)
                {
                    try
                    {
                        _cts.Cancel();
                        _cts.Dispose();
                        _eventChannel.Writer.Complete();
                        _processingTask?.Wait(1000);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "清理Channel资源失败");
                    }
                }

                if (_config.EnableLogging)
                {
                    _logger.LogInformation($"状态机已停止，最终状态: {_currentState}");
                }
            }
        }

        /// <summary>
        /// 重置状态机
        /// </summary>
        /// <param name="initialState">新的初始状态</param>
        public void Reset(TState initialState)
        {
            lock (_lock)
            {
                if (_isRunning)
                {
                    // 执行当前状态的退出动作
                    if (_config.ExitActions.TryGetValue(_currentState, out var exitAction))
                    {
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await exitAction();
                            }
                            catch (Exception ex)
                            {
                                HandleError(ex);
                            }
                        });
                    }
                }

                _currentState = initialState;
                _isRunning = true;

                if (_config.EnableLogging)
                {
                    _logger.LogInformation($"状态机已重置，新的初始状态: {initialState}");
                }

                // 执行新初始状态的进入动作
                if (_config.EntryActions.TryGetValue(initialState, out var entryAction))
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await entryAction();
                        }
                        catch (Exception ex)
                        {
                            HandleError(ex);
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="@event">事件</param>
        /// <returns>是否成功触发</returns>
        public async Task<bool> FireAsync(TEvent @event)
        {
            if (!_isRunning)
            {
                _logger.LogWarning("状态机未运行，无法触发事件");
                return false;
            }

            if (_config.EnableChannelProcessing && _eventChannel != null)
            {
                // 使用Channel处理事件
                await _eventChannel.Writer.WriteAsync(@event);
                return true;
            }
            else
            {
                // 直接处理事件
                return await ProcessEventAsync(@event);
            }
        }

        /// <summary>
        /// 处理单个事件
        /// </summary>
        /// <param name="@event">事件</param>
        /// <returns>是否成功处理</returns>
        private async Task<bool> ProcessEventAsync(TEvent @event)
        {
            lock (_lock)
            {
                var transitions = _config.Transitions
                    .Where(t => EqualityComparer<TState>.Default.Equals(t.SourceState, _currentState))
                    .Where(t => EqualityComparer<TEvent>.Default.Equals(t.Event, @event))
                    .Where(t => t.Condition == null || t.Condition())
                    .ToList();

                if (transitions.Count == 0)
                {
                    _logger.LogInformation($"无匹配的状态转换: 当前状态={_currentState}, 事件={@event}");
                    return false;
                }

                // 处理第一个匹配的转换
                var transition = transitions[0];
                var targetState = transition.TargetState;

                _logger.LogInformation($"执行状态转换: {_currentState} -> {targetState} (事件: {@event}");

                // 执行当前状态的退出动作
                if (_config.ExitActions.TryGetValue(_currentState, out var exitAction))
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await exitAction();
                        }
                        catch (Exception ex)
                        {
                            HandleError(ex);
                        }
                    });
                }

                // 执行转换动作
                if (transition.Action != null)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await transition.Action();
                        }
                        catch (Exception ex)
                        {
                            HandleError(ex);
                        }
                    });
                }

                // 更新当前状态
                _currentState = targetState;

                // 执行目标状态的进入动作
                if (_config.EntryActions.TryGetValue(targetState, out var entryAction))
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await entryAction();
                        }
                        catch (Exception ex)
                        {
                            HandleError(ex);
                        }
                    });
                }

                return true;
            }
        }

        /// <summary>
        /// 处理错误
        /// </summary>
        /// <param name="ex">异常</param>
        private void HandleError(Exception ex)
        {
            if (_config.ErrorHandler != null)
            {
                try
                {
                    _config.ErrorHandler(ex);
                }
                catch (Exception handlerEx)
                {
                    _logger.LogError(handlerEx, "错误处理程序执行失败");
                }
            }
            else
            {
                _logger.LogError(ex, "状态机操作执行失败");
            }
        }
    }

    /// <summary>
    /// 状态机构建器
    /// </summary>
    public class StateMachineBuilder<TState, TEvent>
    {
        private readonly StateMachineConfig<TState, TEvent> _config = new();

        /// <summary>
        /// 添加状态转换
        /// </summary>
        /// <param name="sourceState">源状态</param>
        /// <param name="@event">事件</param>
        /// <param name="targetState">目标状态</param>
        /// <param name="action">转换动作</param>
        /// <param name="condition">转换条件</param>
        /// <returns>构建器实例</returns>
        public StateMachineBuilder<TState, TEvent> AddTransition(TState sourceState, TEvent @event, TState targetState, Func<Task> action = null, Func<bool> condition = null)
        {
            _config.Transitions.Add(new TransitionAction<TState, TEvent>
            {
                SourceState = sourceState,
                Event = @event,
                TargetState = targetState,
                Action = action,
                Condition = condition
            });

            return this;
        }

        /// <summary>
        /// 添加状态进入动作
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="action">进入动作</param>
        /// <returns>构建器实例</returns>
        public StateMachineBuilder<TState, TEvent> AddEntryAction(TState state, Func<Task> action)
        {
            _config.EntryActions[state] = action;
            return this;
        }

        /// <summary>
        /// 添加状态退出动作
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="action">退出动作</param>
        /// <returns>构建器实例</returns>
        public StateMachineBuilder<TState, TEvent> AddExitAction(TState state, Func<Task> action)
        {
            _config.ExitActions[state] = action;
            return this;
        }

        /// <summary>
        /// 设置错误处理程序
        /// </summary>
        /// <param name="errorHandler">错误处理程序</param>
        /// <returns>构建器实例</returns>
        public StateMachineBuilder<TState, TEvent> WithErrorHandler(Action<Exception> errorHandler)
        {
            _config.ErrorHandler = errorHandler;
            return this;
        }

        /// <summary>
        /// 启用或禁用日志
        /// </summary>
        /// <param name="enabled">是否启用</param>
        /// <returns>构建器实例</returns>
        public StateMachineBuilder<TState, TEvent> WithLogging(bool enabled)
        {
            _config.EnableLogging = enabled;
            return this;
        }

        /// <summary>
        /// 设置最大并发事件数
        /// </summary>
        /// <param name="maxConcurrentEvents">最大并发事件数</param>
        /// <returns>构建器实例</returns>
        public StateMachineBuilder<TState, TEvent> WithMaxConcurrentEvents(int maxConcurrentEvents)
        {
            _config.MaxConcurrentEvents = maxConcurrentEvents;
            return this;
        }

        /// <summary>
        /// 设置事件队列大小
        /// </summary>
        /// <param name="queueSize">队列大小</param>
        /// <returns>构建器实例</returns>
        public StateMachineBuilder<TState, TEvent> WithEventQueueSize(int queueSize)
        {
            _config.EventQueueSize = queueSize;
            return this;
        }

        /// <summary>
        /// 构建状态机配置
        /// </summary>
        /// <returns>状态机配置</returns>
        public StateMachineConfig<TState, TEvent> BuildConfig()
        {
            return _config;
        }

        /// <summary>
        /// 构建状态机
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <returns>状态机实例</returns>
        public IStateMachine<TState, TEvent> Build(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<StateMachine<TState, TEvent>>>();
            var cache = serviceProvider.GetRequiredService<IMemoryCache>();
            return new StateMachine<TState, TEvent>(_config, logger, cache);
        }
    }

    /// <summary>
    /// 状态机服务
    /// </summary>
    public class StateMachineService
    {
        private readonly ILogger<StateMachineService> _logger;
        private readonly IMemoryCache _cache;
        private readonly Dictionary<string, object> _stateMachines = new();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="cache">内存缓存</param>
        public StateMachineService(ILogger<StateMachineService> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        /// <summary>
        /// 创建状态机构建器
        /// </summary>
        /// <typeparam name="TState">状态类型</typeparam>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <returns>状态机构建器</returns>
        public StateMachineBuilder<TState, TEvent> CreateBuilder<TState, TEvent>()
        {
            return new StateMachineBuilder<TState, TEvent>();
        }

        /// <summary>
        /// 注册状态机
        /// </summary>
        /// <typeparam name="TState">状态类型</typeparam>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="name">状态机名称</param>
        /// <param name="stateMachine">状态机实例</param>
        public void RegisterStateMachine<TState, TEvent>(string name, IStateMachine<TState, TEvent> stateMachine)
        {
            _stateMachines[name] = stateMachine;
            _logger.LogInformation($"状态机已注册: {name}");
        }

        /// <summary>
        /// 获取状态机
        /// </summary>
        /// <typeparam name="TState">状态类型</typeparam>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="name">状态机名称</param>
        /// <returns>状态机实例</returns>
        public IStateMachine<TState, TEvent> GetStateMachine<TState, TEvent>(string name)
        {
            if (_stateMachines.TryGetValue(name, out var stateMachine))
            {
                return (IStateMachine<TState, TEvent>)stateMachine;
            }

            _logger.LogError($"状态机未找到: {name}");
            throw new KeyNotFoundException($"状态机未找到: {name}");
        }

        /// <summary>
        /// 移除状态机
        /// </summary>
        /// <param name="name">状态机名称</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveStateMachine(string name)
        {
            var removed = _stateMachines.Remove(name);
            if (removed)
            {
                _logger.LogInformation($"状态机已移除: {name}");
            }
            return removed;
        }

        /// <summary>
        /// 获取所有状态机名称
        /// </summary>
        /// <returns>状态机名称列表</returns>
        public IEnumerable<string> GetAllStateMachineNames()
        {
            return _stateMachines.Keys;
        }

        /// <summary>
        /// 启动所有状态机
        /// </summary>
        /// <typeparam name="TState">状态类型</typeparam>
        /// <param name="initialState">初始状态</param>
        public void StartAllStateMachines<TState>(TState initialState)
        {
            foreach (var kvp in _stateMachines)
            {
                if (kvp.Value is IStateMachine<TState, object> stateMachine)
                {
                    stateMachine.Start(initialState);
                }
            }
        }

        /// <summary>
        /// 停止所有状态机
        /// </summary>
        public void StopAllStateMachines()
        {
            foreach (var kvp in _stateMachines)
            {
                if (kvp.Value is dynamic stateMachine)
                {
                    stateMachine.Stop();
                }
            }
        }
    }

    /// <summary>
    /// 主程序类
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
            // 创建命令行根命令
            var rootCommand = new RootCommand("LiquidState AOT - 基于AOT编译的状态机工具");

            // 创建服务提供者
            var serviceProvider = BuildServiceProvider();
            var stateMachineService = serviceProvider.GetRequiredService<StateMachineService>();

            // 创建状态机命令
            var createCommand = new Command("create", "创建状态机")
            {
                new Argument<string>("name", "状态机名称")
            };
            createCommand.AddAlias("c");
            createCommand.Handler = CommandHandler.Create<string>(async (name) =>
            {
                try
                {
                    // 创建一个简单的状态机示例
                    var builder = stateMachineService.CreateBuilder<string, string>();
                    
                    // 配置状态转换
                    builder.AddTransition("Idle", "Start", "Running", async () =>
                    {
                        Console.WriteLine("启动系统...");
                        await Task.Delay(1000);
                        Console.WriteLine("系统已启动");
                    });
                    
                    builder.AddTransition("Running", "Stop", "Idle", async () =>
                    {
                        Console.WriteLine("停止系统...");
                        await Task.Delay(1000);
                        Console.WriteLine("系统已停止");
                    });
                    
                    builder.AddTransition("Running", "Pause", "Paused", async () =>
                    {
                        Console.WriteLine("暂停系统...");
                        await Task.Delay(500);
                        Console.WriteLine("系统已暂停");
                    });
                    
                    builder.AddTransition("Paused", "Resume", "Running", async () =>
                    {
                        Console.WriteLine("恢复系统...");
                        await Task.Delay(500);
                        Console.WriteLine("系统已恢复");
                    });
                    
                    // 添加状态动作
                    builder.AddEntryAction("Running", async () =>
                    {
                        Console.WriteLine("进入运行状态");
                    });
                    
                    builder.AddExitAction("Running", async () =>
                    {
                        Console.WriteLine("退出运行状态");
                    });
                    
                    // 构建并注册状态机
                    var stateMachine = builder.Build(serviceProvider);
                    stateMachineService.RegisterStateMachine(name, stateMachine);
                    
                    Console.WriteLine($"状态机 '{name}' 已创建并注册");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"错误: {ex.Message}");
                    return 1;
                }
                return 0;
            });

            // 启动状态机命令
            var startCommand = new Command("start", "启动状态机")
            {
                new Argument<string>("name", "状态机名称"),
                new Argument<string>("initialState", "初始状态")
            };
            startCommand.AddAlias("s");
            startCommand.Handler = CommandHandler.Create<string, string>(async (name, initialState) =>
            {
                try
                {
                    var stateMachine = stateMachineService.GetStateMachine<string, string>(name);
                    stateMachine.Start(initialState);
                    Console.WriteLine($"状态机 '{name}' 已启动，初始状态: {initialState}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"错误: {ex.Message}");
                    return 1;
                }
                return 0;
            });

            // 触发事件命令
            var fireCommand = new Command("fire", "触发状态机事件")
            {
                new Argument<string>("name", "状态机名称"),
                new Argument<string>("event", "事件")
            };
            fireCommand.AddAlias("f");
            fireCommand.Handler = CommandHandler.Create<string, string>(async (name, @event) =>
            {
                try
                {
                    var stateMachine = stateMachineService.GetStateMachine<string, string>(name);
                    var success = await stateMachine.FireAsync(@event);
                    if (success)
                    {
                        Console.WriteLine($"事件 '{@event}' 已成功触发");
                        Console.WriteLine($"当前状态: {stateMachine.CurrentState}");
                    }
                    else
                    {
                        Console.WriteLine($"事件 '{@event}' 触发失败");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"错误: {ex.Message}");
                    return 1;
                }
                return 0;
            });

            // 停止状态机命令
            var stopCommand = new Command("stop", "停止状态机")
            {
                new Argument<string>("name", "状态机名称")
            };
            stopCommand.AddAlias("st");
            stopCommand.Handler = CommandHandler.Create<string>(async (name) =>
            {
                try
                {
                    var stateMachine = stateMachineService.GetStateMachine<string, string>(name);
                    stateMachine.Stop();
                    Console.WriteLine($"状态机 '{name}' 已停止");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"错误: {ex.Message}");
                    return 1;
                }
                return 0;
            });

            // 列出状态机命令
            var listCommand = new Command("list", "列出所有状态机");
            listCommand.AddAlias("l");
            listCommand.Handler = CommandHandler.Create(async () =>
            {
                try
                {
                    var names = stateMachineService.GetAllStateMachineNames();
                    Console.WriteLine("已注册的状态机:");
                    foreach (var name in names)
                    {
                        Console.WriteLine($"- {name}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"错误: {ex.Message}");
                    return 1;
                }
                return 0;
            });

            // 移除状态机命令
            var removeCommand = new Command("remove", "移除状态机")
            {
                new Argument<string>("name", "状态机名称")
            };
            removeCommand.AddAlias("r");
            removeCommand.Handler = CommandHandler.Create<string>(async (name) =>
            {
                try
                {
                    var success = stateMachineService.RemoveStateMachine(name);
                    if (success)
                    {
                        Console.WriteLine($"状态机 '{name}' 已移除");
                    }
                    else
                    {
                        Console.WriteLine($"状态机 '{name}' 不存在");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"错误: {ex.Message}");
                    return 1;
                }
                return 0;
            });

            // 显示帮助信息命令
            var helpCommand = new Command("help", "显示帮助信息");
            helpCommand.AddAlias("h");
            helpCommand.Handler = CommandHandler.Create(() =>
            {
                rootCommand.Invoke("--help");
                return 0;
            });

            // 添加命令到根命令
            rootCommand.AddCommand(createCommand);
            rootCommand.AddCommand(startCommand);
            rootCommand.AddCommand(fireCommand);
            rootCommand.AddCommand(stopCommand);
            rootCommand.AddCommand(listCommand);
            rootCommand.AddCommand(removeCommand);
            rootCommand.AddCommand(helpCommand);

            // 执行命令
            return await rootCommand.InvokeAsync(args);
        }

        /// <summary>
        /// 构建服务提供者
        /// </summary>
        /// <returns>服务提供者</returns>
        private static ServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            // 添加日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // 添加内存缓存
            services.AddMemoryCache(options =>
            {
                options.SizeLimit = 1024 * 1024;
                options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
            });

            // 添加状态机服务
            services.AddSingleton<StateMachineService>();

            return services.BuildServiceProvider();
        }
    }
}
