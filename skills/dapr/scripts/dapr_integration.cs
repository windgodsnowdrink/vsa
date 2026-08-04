#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Dapr.AspNetCore@1.12.0
#:package Dapr.Client@1.12.0
#:property TargetFramework=net10.0
#:property Nullable=enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;

namespace Dapr.Integration
{
    /// <summary>
    /// Dapr配置选项
    /// </summary>
    public class DaprOptions
    {
        /// <summary>
        /// 状态存储名称
        /// </summary>
        public string StateStoreName { get; set; } = "statestore";

        /// <summary>
        /// 发布订阅组件名称
        /// </summary>
        public string PubSubName { get; set; } = "pubsub";

        /// <summary>
        /// 支持的状态存储
        /// </summary>
        public string[] SupportedStateStores { get; set; } = new[] { "redis", "cosmosdb" };

        /// <summary>
        /// 支持的消息代理
        /// </summary>
        public string[] SupportedMessageBrokers { get; set; } = new[] { "rabbitmq", "kafka" };

        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// 断路器阈值
        /// </summary>
        public int CircuitBreakerThreshold { get; set; } = 5;
    }

    /// <summary>
    /// Dapr健康检查响应
    /// </summary>
    public class DaprHealthCheckResponse
    {
        /// <summary>
        /// 组件名称
        /// </summary>
        public string ComponentName { get; set; }

        /// <summary>
        /// 健康状态
        /// </summary>
        public bool IsHealthy { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Dapr服务接口
    /// </summary>
    public interface IDaprService
    {
        /// <summary>
        /// 保存状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="storeName">状态存储名称</param>
        /// <param name="key">状态键</param>
        /// <param name="value">状态值</param>
        /// <param name="etag">ETag</param>
        /// <param name="options">状态选项</param>
        /// <returns>任务</returns>
        Task SaveStateAsync<T>(string storeName, string key, T value, string etag = null, StateOptions options = null);

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="storeName">状态存储名称</param>
        /// <param name="key">状态键</param>
        /// <param name="consistencyMode">一致性模式</param>
        /// <returns>状态值</returns>
        Task<T> GetStateAsync<T>(string storeName, string key, ConsistencyMode? consistencyMode = null);

        /// <summary>
        /// 删除状态
        /// </summary>
        /// <param name="storeName">状态存储名称</param>
        /// <param name="key">状态键</param>
        /// <returns>任务</returns>
        Task DeleteStateAsync(string storeName, string key);

        /// <summary>
        /// 尝试保存状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="storeName">状态存储名称</param>
        /// <param name="key">状态键</param>
        /// <param name="value">状态值</param>
        /// <param name="etag">ETag</param>
        /// <returns>是否保存成功</returns>
        Task<bool> TrySaveStateAsync<T>(string storeName, string key, T value, string etag = null);

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="pubsubName">发布订阅组件名称</param>
        /// <param name="topic">主题</param>
        /// <param name="data">消息数据</param>
        /// <returns>任务</returns>
        Task PublishAsync<T>(string pubsubName, string topic, T data);

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="pubsubName">发布订阅组件名称</param>
        /// <param name="topic">主题</param>
        /// <param name="handler">消息处理函数</param>
        /// <returns>任务</returns>
        Task SubscribeAsync<T>(string pubsubName, string topic, Func<T, Task> handler);

        /// <summary>
        /// 调用服务
        /// </summary>
        /// <typeparam name="TRequest">请求类型</typeparam>
        /// <typeparam name="TResponse">响应类型</typeparam>
        /// <param name="appId">应用ID</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="request">请求数据</param>
        /// <param name="httpMethod">HTTP方法</param>
        /// <returns>响应数据</returns>
        Task<TResponse> InvokeMethodAsync<TRequest, TResponse>(string appId, string methodName, TRequest request, HttpMethod httpMethod = null);

        /// <summary>
        /// 调用绑定
        /// </summary>
        /// <param name="bindingName">绑定名称</param>
        /// <param name="operation">操作</param>
        /// <param name="data">数据</param>
        /// <param name="metadata">元数据</param>
        /// <returns>任务</returns>
        Task InvokeBindingAsync(string bindingName, string operation, object data, Dictionary<string, string> metadata = null);

        /// <summary>
        /// 检查健康状态
        /// </summary>
        /// <returns>健康检查响应列表</returns>
        Task<IEnumerable<DaprHealthCheckResponse>> CheckHealthAsync();
    }

    /// <summary>
    /// Dapr服务实现
    /// </summary>
    public class DaprService : IDaprService
    {
        /// <summary>
        /// Dapr客户端
        /// </summary>
        private readonly DaprClient _daprClient;

        /// <summary>
        /// 日志记录器
        /// </summary>
        private readonly ILogger<DaprService> _logger;

        /// <summary>
        /// 重试策略
        /// </summary>
        private readonly AsyncRetryPolicy _retryPolicy;

        /// <summary>
        /// 断路器策略
        /// </summary>
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="daprClient">Dapr客户端</param>
        /// <param name="logger">日志记录器</param>
        /// <param name="options">Dapr选项</param>
        public DaprService(DaprClient daprClient, ILogger<DaprService> logger, IOptions<DaprOptions> options)
        {
            _daprClient = daprClient;
            _logger = logger;

            // 配置重试策略
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: options.Value.RetryCount,
                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception, "Dapr操作重试 {RetryCount}，等待 {TimeSpan}ms", retryCount, timeSpan.TotalMilliseconds);
                    });

            // 配置断路器策略
            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: options.Value.CircuitBreakerThreshold,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (exception, breakDuration) =>
                    {
                        _logger.LogError(exception, "Dapr断路器打开，持续 {BreakDuration}s", breakDuration.TotalSeconds);
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation("Dapr断路器关闭");
                    });
        }

        /// <summary>
        /// 保存状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="storeName">状态存储名称</param>
        /// <param name="key">状态键</param>
        /// <param name="value">状态值</param>
        /// <param name="etag">ETag</param>
        /// <param name="options">状态选项</param>
        /// <returns>任务</returns>
        public async Task SaveStateAsync<T>(string storeName, string key, T value, string etag = null, StateOptions options = null)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogInformation("保存状态，存储: {StoreName}, 键: {Key}", storeName, key);
                    await _daprClient.SaveStateAsync(storeName, key, value, etag, options);
                    _logger.LogInformation("保存状态成功，存储: {StoreName}, 键: {Key}", storeName, key);
                });
            });
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="storeName">状态存储名称</param>
        /// <param name="key">状态键</param>
        /// <param name="consistencyMode">一致性模式</param>
        /// <returns>状态值</returns>
        public async Task<T> GetStateAsync<T>(string storeName, string key, ConsistencyMode? consistencyMode = null)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogInformation("获取状态，存储: {StoreName}, 键: {Key}", storeName, key);
                    var result = await _daprClient.GetStateAsync<T>(storeName, key, consistencyMode);
                    _logger.LogInformation("获取状态成功，存储: {StoreName}, 键: {Key}", storeName, key);
                    return result;
                });
            });
        }

        /// <summary>
        /// 删除状态
        /// </summary>
        /// <param name="storeName">状态存储名称</param>
        /// <param name="key">状态键</param>
        /// <returns>任务</returns>
        public async Task DeleteStateAsync(string storeName, string key)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogInformation("删除状态，存储: {StoreName}, 键: {Key}", storeName, key);
                    await _daprClient.DeleteStateAsync(storeName, key);
                    _logger.LogInformation("删除状态成功，存储: {StoreName}, 键: {Key}", storeName, key);
                });
            });
        }

        /// <summary>
        /// 尝试保存状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="storeName">状态存储名称</param>
        /// <param name="key">状态键</param>
        /// <param name="value">状态值</param>
        /// <param name="etag">ETag</param>
        /// <returns>是否保存成功</returns>
        public async Task<bool> TrySaveStateAsync<T>(string storeName, string key, T value, string etag = null)
        {
            try
            {
                await SaveStateAsync(storeName, key, value, etag);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "尝试保存状态失败，存储: {StoreName}, 键: {Key}", storeName, key);
                return false;
            }
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="pubsubName">发布订阅组件名称</param>
        /// <param name="topic">主题</param>
        /// <param name="data">消息数据</param>
        /// <returns>任务</returns>
        public async Task PublishAsync<T>(string pubsubName, string topic, T data)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogInformation("发布消息，发布订阅: {PubSubName}, 主题: {Topic}", pubsubName, topic);
                    await _daprClient.PublishEventAsync(pubsubName, topic, data);
                    _logger.LogInformation("发布消息成功，发布订阅: {PubSubName}, 主题: {Topic}", pubsubName, topic);
                });
            });
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="pubsubName">发布订阅组件名称</param>
        /// <param name="topic">主题</param>
        /// <param name="handler">消息处理函数</param>
        /// <returns>任务</returns>
        public async Task SubscribeAsync<T>(string pubsubName, string topic, Func<T, Task> handler)
        {
            // 这里只是一个示例实现
            // 实际的订阅逻辑应该在控制器或端点中使用Dapr的订阅特性
            _logger.LogInformation("订阅消息，发布订阅: {PubSubName}, 主题: {Topic}", pubsubName, topic);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 调用服务
        /// </summary>
        /// <typeparam name="TRequest">请求类型</typeparam>
        /// <typeparam name="TResponse">响应类型</typeparam>
        /// <param name="appId">应用ID</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="request">请求数据</param>
        /// <param name="httpMethod">HTTP方法</param>
        /// <returns>响应数据</returns>
        public async Task<TResponse> InvokeMethodAsync<TRequest, TResponse>(string appId, string methodName, TRequest request, HttpMethod httpMethod = null)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogInformation("调用服务，应用ID: {AppId}, 方法: {MethodName}", appId, methodName);
                    var response = await _daprClient.InvokeMethodAsync<TRequest, TResponse>(
                        httpMethod ?? HttpMethod.Post,
                        appId,
                        methodName,
                        request);
                    _logger.LogInformation("调用服务成功，应用ID: {AppId}, 方法: {MethodName}", appId, methodName);
                    return response;
                });
            });
        }

        /// <summary>
        /// 调用绑定
        /// </summary>
        /// <param name="bindingName">绑定名称</param>
        /// <param name="operation">操作</param>
        /// <param name="data">数据</param>
        /// <param name="metadata">元数据</param>
        /// <returns>任务</returns>
        public async Task InvokeBindingAsync(string bindingName, string operation, object data, Dictionary<string, string> metadata = null)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogInformation("调用绑定，绑定名称: {BindingName}, 操作: {Operation}", bindingName, operation);
                    await _daprClient.InvokeBindingAsync(bindingName, operation, data, metadata);
                    _logger.LogInformation("调用绑定成功，绑定名称: {BindingName}, 操作: {Operation}", bindingName, operation);
                });
            });
        }

        /// <summary>
        /// 检查健康状态
        /// </summary>
        /// <returns>健康检查响应列表</returns>
        public async Task<IEnumerable<DaprHealthCheckResponse>> CheckHealthAsync()
        {
            var responses = new List<DaprHealthCheckResponse>();

            // 检查状态存储
            responses.AddRange(await CheckStateStoresHealthAsync());

            // 检查消息代理
            responses.AddRange(await CheckMessageBrokersHealthAsync());

            return responses;
        }

        /// <summary>
        /// 检查状态存储健康
        /// </summary>
        /// <returns>健康检查响应列表</returns>
        private async Task<IEnumerable<DaprHealthCheckResponse>> CheckStateStoresHealthAsync()
        {
            var responses = new List<DaprHealthCheckResponse>();

            // 检查默认状态存储
            try
            {
                await _daprClient.GetStateAsync<string>("statestore", "health-check");
                responses.Add(new DaprHealthCheckResponse
                {
                    ComponentName = "statestore",
                    IsHealthy = true
                });
            }
            catch (Exception ex)
            {
                responses.Add(new DaprHealthCheckResponse
                {
                    ComponentName = "statestore",
                    IsHealthy = false,
                    ErrorMessage = ex.Message
                });
            }

            return responses;
        }

        /// <summary>
        /// 检查消息代理健康
        /// </summary>
        /// <returns>健康检查响应列表</returns>
        private async Task<IEnumerable<DaprHealthCheckResponse>> CheckMessageBrokersHealthAsync()
        {
            var responses = new List<DaprHealthCheckResponse>();

            // 检查默认消息代理
            try
            {
                await _daprClient.PublishEventAsync("pubsub", "health-check", new { timestamp = DateTime.UtcNow });
                responses.Add(new DaprHealthCheckResponse
                {
                    ComponentName = "pubsub",
                    IsHealthy = true
                });
            }
            catch (Exception ex)
            {
                responses.Add(new DaprHealthCheckResponse
                {
                    ComponentName = "pubsub",
                    IsHealthy = false,
                    ErrorMessage = ex.Message
                });
            }

            return responses;
        }
    }

    /// <summary>
    /// Dapr服务扩展
    /// </summary>
    public static class DaprServiceExtensions
    {
        /// <summary>
        /// 添加Dapr服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddDaprService(this IServiceCollection services, Action<DaprOptions> configureOptions = null)
        {
            // 配置Dapr选项
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }

            // 注册Dapr客户端
            services.AddDaprClient();

            // 注册Dapr服务
            services.AddSingleton<IDaprService, DaprService>();

            return services;
        }
    }
}