#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Newtonsoft.Json@13.0.3
#:package Dapr.Client@1.14.0
#:package Dapr.AspNetCore@1.14.0
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
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using Dapr.Client;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dapr.AOT
{
    /// <summary>
    /// Dapr配置选项
    /// </summary>
    public class DaprOptions
    {
        /// <summary>
        /// 是否启用Dapr客户端
        /// </summary>
        public bool EnableDaprClient { get; set; } = true;
        
        /// <summary>
        /// Dapr主机地址
        /// </summary>
        public string DaprHost { get; set; } = "http://localhost";
        
        /// <summary>
        /// Dapr HTTP端口
        /// </summary>
        public int DaprHttpPort { get; set; } = 3500;
        
        /// <summary>
        /// Dapr gRPC端口
        /// </summary>
        public int DaprGrpcPort { get; set; } = 50001;
        
        /// <summary>
        /// 是否启用跟踪
        /// </summary>
        public bool EnableTracing { get; set; } = true;
        
        /// <summary>
        /// 默认状态存储名称
        /// </summary>
        public string DefaultStateStore { get; set; } = "statestore";
        
        /// <summary>
        /// 默认发布订阅组件名称
        /// </summary>
        public string DefaultPubSubName { get; set; } = "pubsub";
        
        /// <summary>
        /// 默认绑定名称
        /// </summary>
        public string DefaultBindingName { get; set; } = "binding";
        
        /// <summary>
        /// 超时时间
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
        
        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; } = 3;
        
        /// <summary>
        /// 重试间隔
        /// </summary>
        public TimeSpan RetryInterval { get; set; } = TimeSpan.FromMilliseconds(500);
        
        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
    }
    
    /// <summary>
    /// Dapr服务接口
    /// 定义了Dapr的核心功能
    /// </summary>
    public interface IDaprService
    {
        /// <summary>
        /// 保存状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="key">状态键</param>
        /// <param name="value">状态值</param>
        /// <param name="stateStore">状态存储名称</param>
        /// <returns>操作结果</returns>
        Task<DaprResult> SaveStateAsync<T>(string key, T value, string? stateStore = null) where T : class;
        
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="key">状态键</param>
        /// <param name="stateStore">状态存储名称</param>
        /// <returns>状态值</returns>
        Task<DaprResult<T>> GetStateAsync<T>(string key, string? stateStore = null) where T : class;
        
        /// <summary>
        /// 删除状态
        /// </summary>
        /// <param name="key">状态键</param>
        /// <param name="stateStore">状态存储名称</param>
        /// <returns>操作结果</returns>
        Task<DaprResult> DeleteStateAsync(string key, string? stateStore = null);
        
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="topic">主题名称</param>
        /// <param name="data">消息数据</param>
        /// <param name="pubSubName">发布订阅组件名称</param>
        /// <returns>操作结果</returns>
        Task<DaprResult> PublishMessageAsync<T>(string topic, T data, string? pubSubName = null) where T : class;
        
        /// <summary>
        /// 调用服务
        /// </summary>
        /// <typeparam name="TRequest">请求类型</typeparam>
        /// <typeparam name="TResponse">响应类型</typeparam>
        /// <param name="appId">应用ID</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="requestData">请求数据</param>
        /// <returns>服务响应</returns>
        Task<DaprResult<TResponse>> InvokeServiceAsync<TRequest, TResponse>(string appId, string methodName, TRequest requestData) where TRequest : class where TResponse : class;
        
        /// <summary>
        /// 绑定调用
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="bindingName">绑定名称</param>
        /// <param name="operation">操作名称</param>
        /// <param name="data">操作数据</param>
        /// <returns>操作结果</returns>
        Task<DaprResult> InvokeBindingAsync<T>(string bindingName, string operation, T data) where T : class;
        
        /// <summary>
        /// 获取Dapr状态
        /// </summary>
        /// <returns>状态信息</returns>
        Task<DaprStatus> GetStatusAsync();
        
        /// <summary>
        /// 重置Dapr状态
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> ResetStatusAsync();
    }
    
    /// <summary>
    /// Dapr操作结果
    /// </summary>
    public class DaprResult
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 执行结果数据
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
        /// 操作类型
        /// </summary>
        public string? OperationType { get; set; }
    }
    
    /// <summary>
    /// 带泛型结果的Dapr操作结果
    /// </summary>
    /// <typeparam name="T">结果类型</typeparam>
    public class DaprResult<T> : DaprResult
    {
        /// <summary>
        /// 泛型结果数据
        /// </summary>
        public new T? ResultData { get; set; }
    }
    
    /// <summary>
    /// Dapr状态信息
    /// </summary>
    public class DaprStatus
    {
        /// <summary>
        /// 服务是否正常运行
        /// </summary>
        public bool IsRunning { get; set; }
        
        /// <summary>
        /// 已处理的请求数
        /// </summary>
        public long ProcessedRequests { get; set; }
        
        /// <summary>
        /// 成功处理的请求数
        /// </summary>
        public long SuccessfulRequests { get; set; }
        
        /// <summary>
        /// 失败处理的请求数
        /// </summary>
        public long FailedRequests { get; set; }
        
        /// <summary>
        /// 平均执行时间（毫秒）
        /// </summary>
        public double AverageExecutionTimeMs { get; set; }
        
        /// <summary>
        /// 服务启动时间
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// Dapr客户端状态
        /// </summary>
        public bool IsDaprClientEnabled { get; set; }
        
        /// <summary>
        /// Dapr主机地址
        /// </summary>
        public string DaprHost { get; set; } = string.Empty;
        
        /// <summary>
        /// Dapr HTTP端口
        /// </summary>
        public int DaprHttpPort { get; set; }
    }
    
    /// <summary>
    /// Dapr服务实现
    /// 基于.NET 10 AOT架构，提供高性能Dapr功能
    /// </summary>
    public class DaprService : IDaprService
    {
        private readonly ILogger<DaprService> _logger;
        private readonly DaprOptions _options;
        private readonly DaprClient _daprClient;
        private long _processedRequests = 0;
        private long _successfulRequests = 0;
        private long _failedRequests = 0;
        private long _totalExecutionTime = 0;
        private readonly DateTime _startTime = DateTime.UtcNow;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="options">配置选项</param>
        /// <param name="daprClient">Dapr客户端</param>
        public DaprService(ILogger<DaprService> logger, IOptions<DaprOptions> options, DaprClient daprClient)
        {
            _logger = logger;
            _options = options.Value;
            _daprClient = daprClient;
            
            _logger.LogInformation("DaprService初始化成功，配置选项：DaprHost={DaprHost}, DaprHttpPort={DaprHttpPort}, EnableDaprClient={EnableDaprClient}",
                _options.DaprHost, _options.DaprHttpPort, _options.EnableDaprClient);
        }
        
        /// <summary>
        /// 保存状态
        /// </summary>
        public async Task<DaprResult> SaveStateAsync<T>(string key, T value, string? stateStore = null) where T : class
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DaprResult { OperationType = "SaveState" };
            
            try
            {
                Interlocked.Increment(ref _processedRequests);
                
                if (!_options.EnableDaprClient)
                {
                    throw new InvalidOperationException("Dapr客户端已禁用");
                }
                
                var storeName = stateStore ?? _options.DefaultStateStore;
                
                _logger.LogInformation("开始保存状态，键: {Key}, 存储名称: {StoreName}", key, storeName);
                
                await _daprClient.SaveStateAsync(storeName, key, value);
                
                result.Success = true;
                result.ResultData = new { Key = key, StoreName = storeName };
                
                Interlocked.Increment(ref _successfulRequests);
                
                _logger.LogInformation("保存状态成功，键: {Key}, 存储名称: {StoreName}", key, storeName);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedRequests);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "保存状态失败，键: {Key}", key);
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                Interlocked.Add(ref _totalExecutionTime, result.ExecutionTimeMs);
            }
            
            return result;
        }
        
        /// <summary>
        /// 获取状态
        /// </summary>
        public async Task<DaprResult<T>> GetStateAsync<T>(string key, string? stateStore = null) where T : class
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DaprResult<T> { OperationType = "GetState" };
            
            try
            {
                Interlocked.Increment(ref _processedRequests);
                
                if (!_options.EnableDaprClient)
                {
                    throw new InvalidOperationException("Dapr客户端已禁用");
                }
                
                var storeName = stateStore ?? _options.DefaultStateStore;
                
                _logger.LogInformation("开始获取状态，键: {Key}, 存储名称: {StoreName}", key, storeName);
                
                var value = await _daprClient.GetStateAsync<T>(storeName, key);
                
                result.Success = true;
                result.ResultData = value;
                
                Interlocked.Increment(ref _successfulRequests);
                
                _logger.LogInformation("获取状态成功，键: {Key}, 存储名称: {StoreName}", key, storeName);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedRequests);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "获取状态失败，键: {Key}", key);
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                Interlocked.Add(ref _totalExecutionTime, result.ExecutionTimeMs);
            }
            
            return result;
        }
        
        /// <summary>
        /// 删除状态
        /// </summary>
        public async Task<DaprResult> DeleteStateAsync(string key, string? stateStore = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DaprResult { OperationType = "DeleteState" };
            
            try
            {
                Interlocked.Increment(ref _processedRequests);
                
                if (!_options.EnableDaprClient)
                {
                    throw new InvalidOperationException("Dapr客户端已禁用");
                }
                
                var storeName = stateStore ?? _options.DefaultStateStore;
                
                _logger.LogInformation("开始删除状态，键: {Key}, 存储名称: {StoreName}", key, storeName);
                
                await _daprClient.DeleteStateAsync(storeName, key);
                
                result.Success = true;
                result.ResultData = new { Key = key, StoreName = storeName };
                
                Interlocked.Increment(ref _successfulRequests);
                
                _logger.LogInformation("删除状态成功，键: {Key}, 存储名称: {StoreName}", key, storeName);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedRequests);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "删除状态失败，键: {Key}", key);
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                Interlocked.Add(ref _totalExecutionTime, result.ExecutionTimeMs);
            }
            
            return result;
        }
        
        /// <summary>
        /// 发布消息
        /// </summary>
        public async Task<DaprResult> PublishMessageAsync<T>(string topic, T data, string? pubSubName = null) where T : class
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DaprResult { OperationType = "PublishMessage" };
            
            try
            {
                Interlocked.Increment(ref _processedRequests);
                
                if (!_options.EnableDaprClient)
                {
                    throw new InvalidOperationException("Dapr客户端已禁用");
                }
                
                var pubsubName = pubSubName ?? _options.DefaultPubSubName;
                
                _logger.LogInformation("开始发布消息，主题: {Topic}, 发布订阅名称: {PubSubName}", topic, pubsubName);
                
                await _daprClient.PublishEventAsync(pubsubName, topic, data);
                
                result.Success = true;
                result.ResultData = new { Topic = topic, PubSubName = pubsubName };
                
                Interlocked.Increment(ref _successfulRequests);
                
                _logger.LogInformation("发布消息成功，主题: {Topic}, 发布订阅名称: {PubSubName}", topic, pubsubName);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedRequests);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "发布消息失败，主题: {Topic}", topic);
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                Interlocked.Add(ref _totalExecutionTime, result.ExecutionTimeMs);
            }
            
            return result;
        }
        
        /// <summary>
        /// 调用服务
        /// </summary>
        public async Task<DaprResult<TResponse>> InvokeServiceAsync<TRequest, TResponse>(string appId, string methodName, TRequest requestData) where TRequest : class where TResponse : class
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DaprResult<TResponse> { OperationType = "InvokeService" };
            
            try
            {
                Interlocked.Increment(ref _processedRequests);
                
                if (!_options.EnableDaprClient)
                {
                    throw new InvalidOperationException("Dapr客户端已禁用");
                }
                
                _logger.LogInformation("开始调用服务，应用ID: {AppId}, 方法名称: {MethodName}", appId, methodName);
                
                var response = await _daprClient.InvokeMethodAsync<TRequest, TResponse>(HttpMethod.Post, appId, methodName, requestData);
                
                result.Success = true;
                result.ResultData = response;
                
                Interlocked.Increment(ref _successfulRequests);
                
                _logger.LogInformation("调用服务成功，应用ID: {AppId}, 方法名称: {MethodName}", appId, methodName);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedRequests);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "调用服务失败，应用ID: {AppId}, 方法名称: {MethodName}", appId, methodName);
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                Interlocked.Add(ref _totalExecutionTime, result.ExecutionTimeMs);
            }
            
            return result;
        }
        
        /// <summary>
        /// 绑定调用
        /// </summary>
        public async Task<DaprResult> InvokeBindingAsync<T>(string bindingName, string operation, T data) where T : class
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DaprResult { OperationType = "InvokeBinding" };
            
            try
            {
                Interlocked.Increment(ref _processedRequests);
                
                if (!_options.EnableDaprClient)
                {
                    throw new InvalidOperationException("Dapr客户端已禁用");
                }
                
                var name = bindingName ?? _options.DefaultBindingName;
                
                _logger.LogInformation("开始绑定调用，绑定名称: {BindingName}, 操作: {Operation}", name, operation);
                
                await _daprClient.InvokeBindingAsync(name, operation, data);
                
                result.Success = true;
                result.ResultData = new { BindingName = name, Operation = operation };
                
                Interlocked.Increment(ref _successfulRequests);
                
                _logger.LogInformation("绑定调用成功，绑定名称: {BindingName}, 操作: {Operation}", name, operation);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedRequests);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "绑定调用失败，绑定名称: {BindingName}, 操作: {Operation}", bindingName, operation);
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                Interlocked.Add(ref _totalExecutionTime, result.ExecutionTimeMs);
            }
            
            return result;
        }
        
        /// <summary>
        /// 获取Dapr状态
        /// </summary>
        public async Task<DaprStatus> GetStatusAsync()
        {
            await Task.CompletedTask; // 模拟异步操作
            
            var requestCount = Interlocked.Read(ref _processedRequests);
            var averageTime = requestCount > 0 ? (double)Interlocked.Read(ref _totalExecutionTime) / requestCount : 0;
            
            var status = new DaprStatus
            {
                IsRunning = true,
                ProcessedRequests = requestCount,
                SuccessfulRequests = Interlocked.Read(ref _successfulRequests),
                FailedRequests = Interlocked.Read(ref _failedRequests),
                AverageExecutionTimeMs = Math.Round(averageTime, 2),
                StartTime = _startTime,
                IsDaprClientEnabled = _options.EnableDaprClient,
                DaprHost = _options.DaprHost,
                DaprHttpPort = _options.DaprHttpPort
            };
            
            _logger.LogDebug("获取Dapr状态: {@Status}", status);
            
            return status;
        }
        
        /// <summary>
        /// 重置Dapr状态
        /// </summary>
        public async Task<bool> ResetStatusAsync()
        {
            await Task.CompletedTask; // 模拟异步操作
            
            try
            {
                Interlocked.Exchange(ref _processedRequests, 0);
                Interlocked.Exchange(ref _successfulRequests, 0);
                Interlocked.Exchange(ref _failedRequests, 0);
                Interlocked.Exchange(ref _totalExecutionTime, 0);
                
                _logger.LogInformation("Dapr状态已重置");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "重置Dapr状态失败");
                return false;
            }
        }
    }
    
    /// <summary>
    /// Dapr AOT执行引擎
    /// 管理Dapr功能调用
    /// </summary>
    public class DaprAotEngine
    {
        private readonly ILogger<DaprAotEngine> _logger;
        private readonly IDaprService _daprService;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="daprService">Dapr服务</param>
        public DaprAotEngine(ILogger<DaprAotEngine> logger, IDaprService daprService)
        {
            _logger = logger;
            _daprService = daprService;
            
            _logger.LogInformation("DaprAotEngine初始化成功");
        }
        
        /// <summary>
        /// 保存状态
        /// </summary>
        public async Task<DaprResult> SaveStateAsync<T>(string key, T value, string? stateStore = null) where T : class
        {
            return await _daprService.SaveStateAsync(key, value, stateStore);
        }
        
        /// <summary>
        /// 获取状态
        /// </summary>
        public async Task<DaprResult<T>> GetStateAsync<T>(string key, string? stateStore = null) where T : class
        {
            return await _daprService.GetStateAsync<T>(key, stateStore);
        }
        
        /// <summary>
        /// 删除状态
        /// </summary>
        public async Task<DaprResult> DeleteStateAsync(string key, string? stateStore = null)
        {
            return await _daprService.DeleteStateAsync(key, stateStore);
        }
        
        /// <summary>
        /// 发布消息
        /// </summary>
        public async Task<DaprResult> PublishMessageAsync<T>(string topic, T data, string? pubSubName = null) where T : class
        {
            return await _daprService.PublishMessageAsync(topic, data, pubSubName);
        }
        
        /// <summary>
        /// 调用服务
        /// </summary>
        public async Task<DaprResult<TResponse>> InvokeServiceAsync<TRequest, TResponse>(string appId, string methodName, TRequest requestData) where TRequest : class where TResponse : class
        {
            return await _daprService.InvokeServiceAsync<TRequest, TResponse>(appId, methodName, requestData);
        }
        
        /// <summary>
        /// 绑定调用
        /// </summary>
        public async Task<DaprResult> InvokeBindingAsync<T>(string bindingName, string operation, T data) where T : class
        {
            return await _daprService.InvokeBindingAsync(bindingName, operation, data);
        }
        
        /// <summary>
        /// 获取Dapr状态
        /// </summary>
        public async Task<DaprStatus> GetStatusAsync()
        {
            return await _daprService.GetStatusAsync();
        }
        
        /// <summary>
        /// 重置Dapr状态
        /// </summary>
        public async Task<bool> ResetStatusAsync()
        {
            return await _daprService.ResetStatusAsync();
        }
    }
    
    /// <summary>
    /// Dapr客户端扩展
    /// </summary>
    public static class DaprClientExtensions
    {
        /// <summary>
        /// 注册Dapr客户端
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="options">Dapr配置选项</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddDaprClient(this IServiceCollection services, DaprOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            
            services.AddDaprClient(builder =>
            {
                builder.UseHttpEndpoint($"{options.DaprHost}:{options.DaprHttpPort}");
                builder.UseGrpcEndpoint($"{options.DaprHost}:{options.DaprGrpcPort}");
                
                if (options.EnableTracing)
                {
                    builder.UseTracing();
                }
            });
            
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
            
            // 配置Dapr选项
            builder.Configuration.AddJsonFile("dapr_aot.setting.json", optional: true);
            builder.Services.Configure<DaprOptions>(builder.Configuration.GetSection("Dapr"));
            
            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);
            
            // 获取Dapr选项
            var daprOptions = builder.Configuration.GetSection("Dapr").Get<DaprOptions>() ?? new DaprOptions();
            
            // 注册Dapr客户端
            builder.Services.AddDaprClient(daprOptions);
            
            // 注册服务
            builder.Services.AddSingleton<IDaprService, DaprService>();
            builder.Services.AddSingleton<DaprAotEngine>();
            
            // 构建主机
            var host = builder.Build();
            var serviceProvider = host.Services;
            
            // 获取引擎实例
            var engine = serviceProvider.GetRequiredService<DaprAotEngine>();
            
            // 解析命令行参数
            if (args.Length < 1)
            {
                Console.WriteLine("用法:");
                Console.WriteLine("  dapr_aot.exe <command> [arguments]");
                Console.WriteLine("  ");
                Console.WriteLine("命令:");
                Console.WriteLine("  status	获取服务状态");
                Console.WriteLine("  reset	重置服务状态");
                Console.WriteLine("  ");
                Console.WriteLine("示例:");
                Console.WriteLine("  dapr_aot.exe status");
                Console.WriteLine("  dapr_aot.exe reset");
                return 1;
            }
            
            try
            {
                string command = args[0].ToLower();
                
                switch (command)
                {
                    case "status":
                        var status = await engine.GetStatusAsync();
                        Console.WriteLine("Dapr服务状态:");
                        Console.WriteLine($"  运行状态: {(status.IsRunning ? "正常" : "异常"}");
                        Console.WriteLine($"  Dapr客户端启用: {(status.IsDaprClientEnabled ? "是" : "否"}");
                        Console.WriteLine($"  Dapr主机地址: {status.DaprHost}:{status.DaprHttpPort}");
                        Console.WriteLine($"  已处理请求数: {status.ProcessedRequests}");
                        Console.WriteLine($"  成功请求数: {status.SuccessfulRequests}");
                        Console.WriteLine($"  失败请求数: {status.FailedRequests}");
                        Console.WriteLine($"  平均执行时间: {status.AverageExecutionTimeMs}ms");
                        Console.WriteLine($"  启动时间: {status.StartTime.ToLocalTime()}");
                        return 0;
                        
                    case "reset":
                        var resetResult = await engine.ResetStatusAsync();
                        Console.WriteLine($"重置服务状态: {(resetResult ? "成功" : "失败"}");
                        return resetResult ? 0 : 1;
                        
                    default:
                        Console.WriteLine($"未知命令: {command}");
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行错误: {ex.Message}");
                return 1;
            }
        }
    }
}