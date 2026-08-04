#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Collections.Concurrent@8.0.0
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
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// 路由选项
public class RouterOptions
{
    public string RoutePrefix { get; set; } = "/api";
    public int MaxConcurrentRequests { get; set; } = 100;
    public bool EnableTelemetry { get; set; } = true;
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
}

// 路由上下文
public class RouteContext
{
    public HttpMethod HttpMethod { get; set; }
    public string Path { get; set; }
    public Dictionary<string, string> RouteParameters { get; set; } = new();
    public Dictionary<string, string> QueryParameters { get; set; } = new();
    public Dictionary<string, string> Headers { get; set; } = new();
    public object Body { get; set; }
    public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
}

// 路由信息
public class RouteInfo
{
    public string HttpMethod { get; set; }
    public string Path { get; set; }
    public int MiddlewareCount { get; set; }
}

// 动态路由接口
public interface IDynamicRouter
{
    // 注册路由
    void RegisterRoute(string httpMethod, string path, Func<RouteContext, Task<object>> handler);
    
    // 注册路由（带中间件）
    void RegisterRoute(string httpMethod, string path, Func<RouteContext, Task<object>> handler, params Func<RouteContext, Func<RouteContext, Task<object>>, Task<object>>[] middleware);
    
    // 匹配和执行路由
    Task<object> MatchAndExecuteAsync(RouteContext context);
    
    // 获取所有注册的路由
    IEnumerable<RouteInfo> GetRoutes();
    
    // 移除路由
    void RemoveRoute(string httpMethod, string path);
    
    // 清空所有路由
    void ClearRoutes();
    
    // 获取路由执行统计
    RouteExecutionStats GetStats();
}

// 路由执行统计
public class RouteExecutionStats
{
    public long TotalRequests { get; set; }
    public long SuccessfulRequests { get; set; }
    public long FailedRequests { get; set; }
    public double AverageExecutionTimeMs { get; set; }
    public int ActiveRoutes { get; set; }
}

// 路由节点
internal class RouteNode
{
    public string Segment { get; set; }
    public bool IsParameter { get; set; }
    public string ParameterName { get; set; }
    public Dictionary<string, Func<RouteContext, Task<object>>> Handlers { get; set; } = new();
    public List<Func<RouteContext, Func<RouteContext, Task<object>>, Task<object>>> Middleware { get; set; } = new();
    public Dictionary<string, RouteNode> Children { get; set; } = new();
}

// 动态路由实现
public class DynamicRouter : IDynamicRouter
{
    private readonly RouteNode _rootNode = new() { Segment = string.Empty };
    private readonly RouterOptions _options;
    private readonly ILogger<DynamicRouter>? _logger;
    private readonly ConcurrentDictionary<string, long> _executionTimes = new();
    private long _totalRequests = 0;
    private long _successfulRequests = 0;
    private long _failedRequests = 0;
    private double _totalExecutionTimeMs = 0;

    public DynamicRouter(IOptions<RouterOptions> options, ILogger<DynamicRouter>? logger = null)
    {
        _options = options.Value;
        _logger = logger;
    }

    // 注册路由
    public void RegisterRoute(string httpMethod, string path, Func<RouteContext, Task<object>> handler)
    {
        RegisterRoute(httpMethod, path, handler, Array.Empty<Func<RouteContext, Func<RouteContext, Task<object>>, Task<object>>>());
    }

    // 注册路由（带中间件）
    public void RegisterRoute(string httpMethod, string path, Func<RouteContext, Task<object>> handler, params Func<RouteContext, Func<RouteContext, Task<object>>, Task<object>>[] middleware)
    {
        if (string.IsNullOrEmpty(httpMethod))
            throw new ArgumentNullException(nameof(httpMethod));
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        // 移除路径前缀
        if (!string.IsNullOrEmpty(_options.RoutePrefix) && path.StartsWith(_options.RoutePrefix))
        {
            path = path.Substring(_options.RoutePrefix.Length);
        }

        // 标准化路径
        if (!path.StartsWith("/"))
            path = "/" + path;

        // 构建路由树
        var segments = path.Split('/').Where(s => !string.IsNullOrEmpty(s)).ToArray();
        var currentNode = _rootNode;

        foreach (var segment in segments)
        {
            RouteNode childNode;
            if (segment.StartsWith("{"))
            {
                // 参数段
                var parameterName = segment.Substring(1, segment.Length - 2);
                var paramNodeKey = $"{{{parameterName}}}";

                if (!currentNode.Children.TryGetValue("__param__", out childNode))
                {
                    childNode = new RouteNode
                    {
                        Segment = paramNodeKey,
                        IsParameter = true,
                        ParameterName = parameterName
                    };
                    currentNode.Children["__param__"] = childNode;
                }
            }
            else
            {
                // 静态段
                if (!currentNode.Children.TryGetValue(segment, out childNode))
                {
                    childNode = new RouteNode { Segment = segment };
                    currentNode.Children[segment] = childNode;
                }
            }

            currentNode = childNode;
        }

        // 注册处理器和中间件
        currentNode.Handlers[httpMethod.ToUpperInvariant()] = handler;
        if (middleware.Length > 0)
        {
            currentNode.Middleware.AddRange(middleware);
        }

        _logger?.LogInformation("[DynamicRouter] 注册路由: {HttpMethod} {Path}", httpMethod, path);
    }

    // 匹配和执行路由
    public async Task<object> MatchAndExecuteAsync(RouteContext context)
    {
        Interlocked.Increment(ref _totalRequests);
        var startTime = DateTime.UtcNow;

        try
        {
            // 移除路径前缀
            var path = context.Path;
            if (!string.IsNullOrEmpty(_options.RoutePrefix) && path.StartsWith(_options.RoutePrefix))
            {
                path = path.Substring(_options.RoutePrefix.Length);
            }

            // 标准化路径
            if (!path.StartsWith("/"))
                path = "/" + path;

            // 匹配路由
            var segments = path.Split('/').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            var matchResult = MatchRoute(_rootNode, segments, 0, new Dictionary<string, string>());

            if (matchResult == null)
            {
                throw new InvalidOperationException($"未找到匹配的路由: {context.HttpMethod} {context.Path}");
            }

            // 设置路由参数
            foreach (var (key, value) in matchResult.Parameters)
            {
                context.RouteParameters[key] = value;
            }

            // 构建中间件管道
            var handler = matchResult.Handler;
            var middleware = matchResult.Middleware;

            if (middleware.Any())
            {
                // 从后往前构建中间件管道
                var pipeline = handler;
                for (int i = middleware.Count - 1; i >= 0; i--)
                {
                    var currentMiddleware = middleware[i];
                    var next = pipeline;
                    pipeline = async (ctx) => await currentMiddleware(ctx, next);
                }
                handler = pipeline;
            }

            // 执行路由
            var result = await handler(context);
            Interlocked.Increment(ref _successfulRequests);
            return result;
        }
        catch (Exception ex)
        {
            Interlocked.Increment(ref _failedRequests);
            _logger?.LogError(ex, "[DynamicRouter] 路由执行失败: {HttpMethod} {Path}", context.HttpMethod, context.Path);
            throw;
        }
        finally
        {
            var executionTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
            Interlocked.Add(ref _totalExecutionTimeMs, executionTimeMs);
            _executionTimes[context.Path] = (long)executionTimeMs;
        }
    }

    // 匹配路由
    private (Func<RouteContext, Task<object>> Handler, Dictionary<string, string> Parameters, List<Func<RouteContext, Func<RouteContext, Task<object>>, Task<object>>> Middleware)? MatchRoute(
        RouteNode node, string[] segments, int index, Dictionary<string, string> parameters)
    {
        if (index == segments.Length)
        {
            // 找到匹配的节点，检查是否有对应HTTP方法的处理器
            if (node.Handlers.TryGetValue(node.HttpMethod?.ToUpperInvariant() ?? string.Empty, out var handler))
            {
                return (handler, parameters, node.Middleware);
            }
            return null;
        }

        var segment = segments[index];

        // 尝试匹配静态段
        if (node.Children.TryGetValue(segment, out var staticChild))
        {
            var result = MatchRoute(staticChild, segments, index + 1, parameters);
            if (result != null)
            {
                return result;
            }
        }

        // 尝试匹配参数段
        if (node.Children.TryGetValue("__param__", out var paramChild))
        {
            var newParameters = new Dictionary<string, string>(parameters)
            {
                [paramChild.ParameterName] = segment
            };
            var result = MatchRoute(paramChild, segments, index + 1, newParameters);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    // 获取所有注册的路由
    public IEnumerable<RouteInfo> GetRoutes()
    {
        var routes = new List<RouteInfo>();
        CollectRoutes(_rootNode, string.Empty, routes);
        return routes;
    }

    // 收集路由
    private void CollectRoutes(RouteNode node, string currentPath, List<RouteInfo> routes)
    {
        if (node.Handlers.Any())
        {
            foreach (var method in node.Handlers.Keys)
            {
                routes.Add(new RouteInfo
                {
                    HttpMethod = method,
                    Path = currentPath,
                    MiddlewareCount = node.Middleware.Count
                });
            }
        }

        foreach (var (segment, childNode) in node.Children)
        {
            var newPath = currentPath + "/" + (childNode.IsParameter ? childNode.ParameterName : segment);
            CollectRoutes(childNode, newPath, routes);
        }
    }

    // 移除路由
    public void RemoveRoute(string httpMethod, string path)
    {
        // 移除路径前缀
        if (!string.IsNullOrEmpty(_options.RoutePrefix) && path.StartsWith(_options.RoutePrefix))
        {
            path = path.Substring(_options.RoutePrefix.Length);
        }

        // 标准化路径
        if (!path.StartsWith("/"))
            path = "/" + path;

        // 查找并移除路由
        var segments = path.Split('/').Where(s => !string.IsNullOrEmpty(s)).ToArray();
        RemoveRoute(_rootNode, segments, 0, httpMethod.ToUpperInvariant());

        _logger?.LogInformation("[DynamicRouter] 移除路由: {HttpMethod} {Path}", httpMethod, path);
    }

    // 移除路由
    private bool RemoveRoute(RouteNode node, string[] segments, int index, string httpMethod)
    {
        if (index == segments.Length)
        {
            // 找到匹配的节点，移除对应HTTP方法的处理器
            return node.Handlers.Remove(httpMethod);
        }

        var segment = segments[index];
        RouteNode childNode;

        if (segment.StartsWith("{"))
        {
            // 参数段
            if (!node.Children.TryGetValue("__param__", out childNode))
                return false;
        }
        else
        {
            // 静态段
            if (!node.Children.TryGetValue(segment, out childNode))
                return false;
        }

        var removed = RemoveRoute(childNode, segments, index + 1, httpMethod);
        
        // 如果子节点没有处理器且没有子节点，移除子节点
        if (removed && !childNode.Handlers.Any() && !childNode.Children.Any())
        {
            if (segment.StartsWith("{"))
                node.Children.Remove("__param__");
            else
                node.Children.Remove(segment);
        }

        return removed;
    }

    // 清空所有路由
    public void ClearRoutes()
    {
        _rootNode.Children.Clear();
        _executionTimes.Clear();
        Interlocked.Exchange(ref _totalRequests, 0);
        Interlocked.Exchange(ref _successfulRequests, 0);
        Interlocked.Exchange(ref _failedRequests, 0);
        Interlocked.Exchange(ref _totalExecutionTimeMs, 0);

        _logger?.LogInformation("[DynamicRouter] 清空所有路由");
    }

    // 获取路由执行统计
    public RouteExecutionStats GetStats()
    {
        var activeRoutes = GetRoutes().Count();
        var averageExecutionTimeMs = _totalRequests > 0 ? _totalExecutionTimeMs / _totalRequests : 0;

        return new RouteExecutionStats
        {
            TotalRequests = _totalRequests,
            SuccessfulRequests = _successfulRequests,
            FailedRequests = _failedRequests,
            AverageExecutionTimeMs = averageExecutionTimeMs,
            ActiveRoutes = activeRoutes
        };
    }
}

// 依赖注入扩展
public static class DynamicRouterExtensions
{
    public static IServiceCollection AddDynamicRouter(this IServiceCollection services)
    {
        return services.AddDynamicRouter(options => { });
    }

    public static IServiceCollection AddDynamicRouter(this IServiceCollection services, Action<RouterOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<IDynamicRouter, DynamicRouter>();
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

        // 注册动态路由服务
        services.AddDynamicRouter(options =>
        {
            options.RoutePrefix = "/api";
            options.MaxConcurrentRequests = 100;
            options.EnableTelemetry = true;
        });

        var serviceProvider = services.BuildServiceProvider();

        // 获取动态路由服务
        var dynamicRouter = serviceProvider.GetRequiredService<IDynamicRouter>();

        Console.WriteLine("Dynamic Router Demo");
        Console.WriteLine("=" + new string('=', 50));

        try
        {
            // 注册路由
            dynamicRouter.RegisterRoute("GET", "/users/{id}", async (context) =>
            {
                var id = context.RouteParameters["id"];
                return new { UserId = id, Name = "Test User" };
            });

            dynamicRouter.RegisterRoute("POST", "/users", async (context) =>
            {
                return new { Id = Guid.NewGuid(), Name = "New User", Created = DateTime.UtcNow };
            });

            // 注册带中间件的路由
            dynamicRouter.RegisterRoute("GET", "/protected", async (context) =>
            {
                return new { Protected = "Resource", User = "Authenticated" };
            },
            async (ctx, next) =>
            {
                // 简单的认证中间件
                Console.WriteLine("[Middleware] 认证检查");
                // 模拟认证通过
                return await next(ctx);
            });

            // 测试路由
            Console.WriteLine("\n测试路由匹配...");

            // 测试 GET /api/users/123
            var getContext = new RouteContext
            {
                HttpMethod = HttpMethod.Get,
                Path = "/api/users/123"
            };
            var getResult = await dynamicRouter.MatchAndExecuteAsync(getContext);
            Console.WriteLine($"✓ GET /api/users/123: {getResult}");

            // 测试 POST /api/users
            var postContext = new RouteContext
            {
                HttpMethod = HttpMethod.Post,
                Path = "/api/users"
            };
            var postResult = await dynamicRouter.MatchAndExecuteAsync(postContext);
            Console.WriteLine($"✓ POST /api/users: {postResult}");

            // 测试带中间件的路由
            var protectedContext = new RouteContext
            {
                HttpMethod = HttpMethod.Get,
                Path = "/api/protected"
            };
            var protectedResult = await dynamicRouter.MatchAndExecuteAsync(protectedContext);
            Console.WriteLine($"✓ GET /api/protected: {protectedResult}");

            // 获取路由统计
            var stats = dynamicRouter.GetStats();
            Console.WriteLine("\n路由执行统计:");
            Console.WriteLine($"总请求数: {stats.TotalRequests}");
            Console.WriteLine($"成功请求数: {stats.SuccessfulRequests}");
            Console.WriteLine($"失败请求数: {stats.FailedRequests}");
            Console.WriteLine($"平均执行时间: {stats.AverageExecutionTimeMs:F2}ms");
            Console.WriteLine($"活跃路由数: {stats.ActiveRoutes}");

            // 获取所有注册的路由
            var routes = dynamicRouter.GetRoutes();
            Console.WriteLine("\n注册的路由:");
            foreach (var route in routes)
            {
                Console.WriteLine($"- {route.HttpMethod} {route.Path} (中间件: {route.MiddlewareCount})");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            // 释放资源
            if (serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}