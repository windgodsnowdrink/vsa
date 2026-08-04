#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.Extensions.Http@10.0.0
#:package Yarp.ReverseProxy@2.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property EnableUnsafeBinaryFormatterSerialization=false

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Model;

namespace Gateway.AOT
{
    /// <summary>
    /// 网关命令类型枚举
    /// </summary>
    public enum GatewayCommandType { Start, Stop, Restart, Status, AddRoute, RemoveRoute, ListRoutes, VersionInfo, Help }

    /// <summary>
    /// 网关选项配置
    /// </summary>
    public class GatewayOptions
    {
        /// <summary>
        /// 工作目录
        /// </summary>
        public string WorkingDirectory { get; set; } = Environment.CurrentDirectory;
        
        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
        
        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;
        
        /// <summary>
        /// 请求超时时间（毫秒）
        /// </summary>
        public int RequestTimeoutMs { get; set; } = 30000;
        
        /// <summary>
        /// 网关服务器地址
        /// </summary>
        public string ServerAddress { get; set; } = "localhost";
        
        /// <summary>
        /// 网关服务器端口
        /// </summary>
        public int ServerPort { get; set; } = 8080;
        
        /// <summary>
        /// 是否启用 SSL
        /// </summary>
        public bool EnableSsl { get; set; } = false;
        
        /// <summary>
        /// 是否启用压缩
        /// </summary>
        public bool EnableCompression { get; set; } = true;
        
        /// <summary>
        /// 是否启用电路 breaker
        /// </summary>
        public bool EnableCircuitBreaker { get; set; } = true;
        
        /// <summary>
        /// 电路 breaker 失败阈值
        /// </summary>
        public int CircuitBreakerFailureThreshold { get; set; } = 5;
        
        /// <summary>
        /// 电路 breaker 重置时间（毫秒）
        /// </summary>
        public int CircuitBreakerResetTimeMs { get; set; } = 30000;
    }

    /// <summary>
    /// 路由配置
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// 路由 ID
        /// </summary>
        public string RouteId { get; set; } = string.Empty;
        
        /// <summary>
        /// 匹配路径
        /// </summary>
        public string MatchPath { get; set; } = string.Empty;
        
        /// <summary>
        /// 目标集群
        /// </summary>
        public string ClusterId { get; set; } = string.Empty;
        
        /// <summary>
        /// 目标地址
        /// </summary>
        public string DestinationAddress { get; set; } = string.Empty;
    }

    /// <summary>
    /// 网关命令结果
    /// </summary>
    public class GatewayCommandResult
    {
        /// <summary>
        /// 命令是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 执行时间（毫秒）
        /// </summary>
        public long ExecutionTimeMs { get; set; }
        
        /// <summary>
        /// 错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// 结果数据
        /// </summary>
        public List<string> Results { get; set; } = new List<string>();
        
        /// <summary>
        /// 路由列表
        /// </summary>
        public List<RouteConfig>? Routes { get; set; }
    }

    /// <summary>
    /// 网关服务接口
    /// </summary>
    public interface IGatewayService
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>命令结果</returns>
        Task<GatewayCommandResult> ExecuteCommandAsync(GatewayCommandType commandType, Dictionary<string, string>? parameters = null);
        
        /// <summary>
        /// 启动网关
        /// </summary>
        /// <returns>启动结果</returns>
        Task<GatewayCommandResult> StartAsync();
        
        /// <summary>
        /// 停止网关
        /// </summary>
        /// <returns>停止结果</returns>
        Task<GatewayCommandResult> StopAsync();
        
        /// <summary>
        /// 重启网关
        /// </summary>
        /// <returns>重启结果</returns>
        Task<GatewayCommandResult> RestartAsync();
        
        /// <summary>
        /// 获取网关状态
        /// </summary>
        /// <returns>状态结果</returns>
        Task<GatewayCommandResult> GetStatusAsync();
        
        /// <summary>
        /// 添加路由
        /// </summary>
        /// <param name="routeId">路由 ID</param>
        /// <param name="matchPath">匹配路径</param>
        /// <param name="clusterId">集群 ID</param>
        /// <param name="destinationAddress">目标地址</param>
        /// <returns>添加结果</returns>
        Task<GatewayCommandResult> AddRouteAsync(string routeId, string matchPath, string clusterId, string destinationAddress);
        
        /// <summary>
        /// 删除路由
        /// </summary>
        /// <param name="routeId">路由 ID</param>
        /// <returns>删除结果</returns>
        Task<GatewayCommandResult> RemoveRouteAsync(string routeId);
        
        /// <summary>
        /// 列出所有路由
        /// </summary>
        /// <returns>路由列表</returns>
        Task<GatewayCommandResult> ListRoutesAsync();
        
        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        Task<GatewayCommandResult> GetVersionInfoAsync();
    }

    /// <summary>
    /// 网关服务实现
    /// </summary>
    public class GatewayService : IGatewayService
    {
        private readonly GatewayOptions _options;
        private readonly ILogger<GatewayService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private bool _isRunning = false;
        private List<RouteConfig> _routes = new List<RouteConfig>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">网关选项</param>
        /// <param name="logger">日志记录器</param>
        /// <param name="httpClientFactory">HTTP 客户端工厂</param>
        public GatewayService(IOptions<GatewayOptions> options, ILogger<GatewayService> logger, IHttpClientFactory httpClientFactory)
        {
            _options = options.Value;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            
            // 初始化示例路由
            InitializeSampleRoutes();
        }

        /// <summary>
        /// 初始化示例路由
        /// </summary>
        private void InitializeSampleRoutes()
        {
            _routes.Add(new RouteConfig
            {
                RouteId = "api_route",
                MatchPath = "/api/{**remainder}",
                ClusterId = "api_cluster",
                DestinationAddress = "http://localhost:5000"
            });
            
            _routes.Add(new RouteConfig
            {
                RouteId = "web_route",
                MatchPath = "/web/{**remainder}",
                ClusterId = "web_cluster",
                DestinationAddress = "http://localhost:5001"
            });
        }

        /// <inheritdoc/>
        public async Task<GatewayCommandResult> ExecuteCommandAsync(GatewayCommandType commandType, Dictionary<string, string>? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new GatewayCommandResult();

            try
            {
                switch (commandType)
                {
                    case GatewayCommandType.Start:
                        result = await StartAsync();
                        break;
                    
                    case GatewayCommandType.Stop:
                        result = await StopAsync();
                        break;
                    
                    case GatewayCommandType.Restart:
                        result = await RestartAsync();
                        break;
                    
                    case GatewayCommandType.Status:
                        result = await GetStatusAsync();
                        break;
                    
                    case GatewayCommandType.AddRoute:
                        if (parameters?.ContainsKey("routeId") == true && parameters?.ContainsKey("matchPath") == true && 
                            parameters?.ContainsKey("clusterId") == true && parameters?.ContainsKey("destinationAddress") == true)
                        {
                            string routeId = parameters["routeId"];
                            string matchPath = parameters["matchPath"];
                            string clusterId = parameters["clusterId"];
                            string destinationAddress = parameters["destinationAddress"];
                            result = await AddRouteAsync(routeId, matchPath, clusterId, destinationAddress);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "缺少必要的参数: routeId, matchPath, clusterId 和 destinationAddress";
                        }
                        break;
                    
                    case GatewayCommandType.RemoveRoute:
                        if (parameters?.ContainsKey("routeId") == true)
                        {
                            string routeId = parameters["routeId"];
                            result = await RemoveRouteAsync(routeId);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "缺少必要的参数: routeId";
                        }
                        break;
                    
                    case GatewayCommandType.ListRoutes:
                        result = await ListRoutesAsync();
                        break;
                    
                    case GatewayCommandType.VersionInfo:
                        result = await GetVersionInfoAsync();
                        break;
                    
                    default:
                        result.Success = false;
                        result.ErrorMessage = "未知命令类型";
                        break;
                }
            }
            catch (Exception ex)
            {
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
        public async Task<GatewayCommandResult> StartAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new GatewayCommandResult();

            try
            {
                _logger.LogInformation("启动网关服务");
                
                // 检查是否已运行
                if (_isRunning)
                {
                    result.Success = false;
                    result.ErrorMessage = "网关服务已在运行";
                    return result;
                }
                
                // 模拟启动过程
                await Task.Delay(1000);
                
                // 启动网关
                _isRunning = true;
                
                result.Success = true;
                result.Results.Add($"成功启动网关服务");
                result.Results.Add($"服务器地址: {(EnableSsl ? "https" : "http")}://{_options.ServerAddress}:{_options.ServerPort}");
                result.Results.Add($"启用压缩: {_options.EnableCompression}");
                result.Results.Add($"启用电路 breaker: {_options.EnableCircuitBreaker}");
                result.Results.Add($"启动时间: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "启动网关服务时出错");
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
        public async Task<GatewayCommandResult> StopAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new GatewayCommandResult();

            try
            {
                _logger.LogInformation("停止网关服务");
                
                // 检查是否未运行
                if (!_isRunning)
                {
                    result.Success = false;
                    result.ErrorMessage = "网关服务未运行";
                    return result;
                }
                
                // 模拟停止过程
                await Task.Delay(500);
                
                // 停止网关
                _isRunning = false;
                
                result.Success = true;
                result.Results.Add($"成功停止网关服务");
                result.Results.Add($"停止时间: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "停止网关服务时出错");
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
        public async Task<GatewayCommandResult> RestartAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new GatewayCommandResult();

            try
            {
                _logger.LogInformation("重启网关服务");
                
                // 先停止
                var stopResult = await StopAsync();
                if (!stopResult.Success)
                {
                    // 如果未运行，直接启动
                    if (stopResult.ErrorMessage == "网关服务未运行")
                    {
                        var startResult = await StartAsync();
                        result.Success = startResult.Success;
                        result.ErrorMessage = startResult.ErrorMessage;
                        result.Results.AddRange(startResult.Results);
                    }
                    else
                    {
                        result.Success = false;
                        result.ErrorMessage = stopResult.ErrorMessage;
                    }
                    return result;
                }
                
                // 再启动
                var startResult = await StartAsync();
                result.Success = startResult.Success;
                result.ErrorMessage = startResult.ErrorMessage;
                result.Results.AddRange(startResult.Results);
                result.Results.Insert(0, "成功重启网关服务");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "重启网关服务时出错");
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
        public async Task<GatewayCommandResult> GetStatusAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new GatewayCommandResult();

            try
            {
                _logger.LogInformation("获取网关服务状态");
                
                // 模拟获取状态
                await Task.Delay(100);
                
                result.Success = true;
                result.Results.Add($"网关服务状态: {(IsRunning ? "运行中" : "已停止")}");
                result.Results.Add($"服务器地址: {(EnableSsl ? "https" : "http")}://{_options.ServerAddress}:{_options.ServerPort}");
                result.Results.Add($"启用压缩: {_options.EnableCompression}");
                result.Results.Add($"启用电路 breaker: {_options.EnableCircuitBreaker}");
                result.Results.Add($"路由数量: {_routes.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取网关服务状态时出错");
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
        public async Task<GatewayCommandResult> AddRouteAsync(string routeId, string matchPath, string clusterId, string destinationAddress)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new GatewayCommandResult();

            try
            {
                _logger.LogInformation("添加路由: {RouteId}", routeId);
                
                // 检查路由是否已存在
                if (_routes.Any(r => r.RouteId == routeId))
                {
                    result.Success = false;
                    result.ErrorMessage = "路由已存在";
                    return result;
                }
                
                // 模拟添加过程
                await Task.Delay(100);
                
                // 添加路由
                var route = new RouteConfig
                {
                    RouteId = routeId,
                    MatchPath = matchPath,
                    ClusterId = clusterId,
                    DestinationAddress = destinationAddress
                };
                _routes.Add(route);
                
                result.Success = true;
                result.Results.Add($"成功添加路由: {routeId}");
                result.Results.Add($"匹配路径: {matchPath}");
                result.Results.Add($"集群 ID: {clusterId}");
                result.Results.Add($"目标地址: {destinationAddress}");
                result.Results.Add($"添加时间: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加路由时出错");
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
        public async Task<GatewayCommandResult> RemoveRouteAsync(string routeId)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new GatewayCommandResult();

            try
            {
                _logger.LogInformation("删除路由: {RouteId}", routeId);
                
                // 检查路由是否存在
                var route = _routes.FirstOrDefault(r => r.RouteId == routeId);
                if (route == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "路由不存在";
                    return result;
                }
                
                // 模拟删除过程
                await Task.Delay(100);
                
                // 删除路由
                _routes.Remove(route);
                
                result.Success = true;
                result.Results.Add($"成功删除路由: {routeId}");
                result.Results.Add($"匹配路径: {route.MatchPath}");
                result.Results.Add($"集群 ID: {route.ClusterId}");
                result.Results.Add($"目标地址: {route.DestinationAddress}");
                result.Results.Add($"删除时间: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除路由时出错");
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
        public async Task<GatewayCommandResult> ListRoutesAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new GatewayCommandResult();

            try
            {
                _logger.LogInformation("列出所有路由");
                
                // 模拟获取路由列表
                await Task.Delay(100);
                
                result.Success = true;
                result.Results.Add($"成功获取路由列表");
                result.Results.Add($"路由数量: {_routes.Count}");
                result.Routes = _routes;
                
                foreach (var route in _routes)
                {
                    result.Results.Add($"- {route.RouteId}: {route.MatchPath} -> {route.DestinationAddress}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "列出路由时出错");
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
        public async Task<GatewayCommandResult> GetVersionInfoAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new GatewayCommandResult();

            try
            {
                _logger.LogInformation("获取网关版本信息");
                
                // 模拟获取版本信息
                await Task.Delay(100);
                
                result.Success = true;
                result.Results.Add("Gateway AOT Engine");
                result.Results.Add($"版本: 1.0.0");
                result.Results.Add($".NET 版本: {Environment.Version}");
                result.Results.Add($"操作系统: {Environment.OSVersion}");
                result.Results.Add($"架构: {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}");
                result.Results.Add($"AOT 编译: {AppContext.TryGetSwitch("PublishAot", out bool isAot) && isAot}");
                result.Results.Add($"服务器地址: {(EnableSsl ? "https" : "http")}://{_options.ServerAddress}:{_options.ServerPort}");
                result.Results.Add($"启用 SSL: {_options.EnableSsl}");
                result.Results.Add($"工作目录: {_options.WorkingDirectory}");
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

            return result;
        }

        /// <summary>
        /// 是否启用 SSL
        /// </summary>
        private bool EnableSsl => _options.EnableSsl;

        /// <summary>
        /// 网关是否运行中
        /// </summary>
        public bool IsRunning => _isRunning;
    }

    /// <summary>
    /// 网关 AOT 引擎
    /// </summary>
    public class GatewayAotEngine
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<GatewayAotEngine> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="logger">日志记录器</param>
        public GatewayAotEngine(IServiceProvider serviceProvider, ILogger<GatewayAotEngine> logger)
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
            _logger.LogInformation("Gateway AOT Engine 启动，参数: {Args}", string.Join(" ", args));
            
            if (args.Length == 0)
            {
                ShowHelp();
                return 0;
            }

            var command = args[0].ToLower();
            var gatewayService = _serviceProvider.GetRequiredService<IGatewayService>();
            GatewayCommandResult? result = null;

            try
            {
                switch (command)
                {
                    case "start":
                        result = await gatewayService.StartAsync();
                        break;
                    
                    case "stop":
                        result = await gatewayService.StopAsync();
                        break;
                    
                    case "restart":
                        result = await gatewayService.RestartAsync();
                        break;
                    
                    case "status":
                        result = await gatewayService.GetStatusAsync();
                        break;
                    
                    case "add":
                    case "addroute":
                        if (args.Length < 5)
                        {
                            Console.WriteLine("错误: 需要提供 routeId, matchPath, clusterId 和 destinationAddress");
                            return 1;
                        }
                        string routeId = args[1];
                        string matchPath = args[2];
                        string clusterId = args[3];
                        string destinationAddress = args[4];
                        result = await gatewayService.AddRouteAsync(routeId, matchPath, clusterId, destinationAddress);
                        break;
                    
                    case "remove":
                    case "removeroute":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("错误: 需要提供 routeId");
                            return 1;
                        }
                        string removeRouteId = args[1];
                        result = await gatewayService.RemoveRouteAsync(removeRouteId);
                        break;
                    
                    case "list":
                    case "listroutes":
                        result = await gatewayService.ListRoutesAsync();
                        break;
                    
                    case "version":
                    case "info":
                        result = await gatewayService.GetVersionInfoAsync();
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
                
                if (result.Routes != null && result.Routes.Any())
                {
                    Console.WriteLine($"\n路由详情:");
                    foreach (var route in result.Routes)
                    {
                        Console.WriteLine($"  {route.RouteId}:");
                        Console.WriteLine($"    匹配路径: {route.MatchPath}");
                        Console.WriteLine($"    集群 ID: {route.ClusterId}");
                        Console.WriteLine($"    目标地址: {route.DestinationAddress}");
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
            Console.WriteLine("Gateway AOT Engine - .NET 10 AOT 编译的网关引擎");
            Console.WriteLine();
            Console.WriteLine("用法: gateway_aot <命令> [参数]");
            Console.WriteLine();
            Console.WriteLine("命令:");
            Console.WriteLine("  start                                  启动网关服务");
            Console.WriteLine("  stop                                   停止网关服务");
            Console.WriteLine("  restart                                重启网关服务");
            Console.WriteLine("  status                                 获取网关状态");
            Console.WriteLine("  add|addroute <routeId> <matchPath> <clusterId> <destinationAddress>  添加路由");
            Console.WriteLine("  remove|removeroute <routeId>           删除路由");
            Console.WriteLine("  list|listroutes                        列出所有路由");
            Console.WriteLine("  version|info                           显示版本信息");
            Console.WriteLine("  help|--help|-h                         显示帮助信息");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine("  gateway_aot start");
            Console.WriteLine("  gateway_aot add api_route /api/{**remainder} api_cluster http://localhost:5000");
            Console.WriteLine("  gateway_aot list");
            Console.WriteLine("  gateway_aot stop");
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
            // 创建主机
            var builder = Host.CreateApplicationBuilder(args);
            
            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            
            // 配置服务
            builder.Services.Configure<GatewayOptions>(builder.Configuration.GetSection("Gateway"));
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<IGatewayService, GatewayService>();
            builder.Services.AddSingleton<GatewayAotEngine>();
            
            // 构建主机
            using var host = builder.Build();
            
            // 获取引擎实例
            var engine = host.Services.GetRequiredService<GatewayAotEngine>();
            
            // 执行命令
            return await engine.ExecuteCommandLineAsync(args);
        }
    }
}
