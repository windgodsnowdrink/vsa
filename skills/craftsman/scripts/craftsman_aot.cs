#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Newtonsoft.Json@13.0.3
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

namespace Craftsman.AOT
{
    /// <summary>
    /// Craftsman配置选项
    /// </summary>
    public class CraftsmanOptions
    {
        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public bool EnableCache { get; set; } = true;
        
        /// <summary>
        /// 缓存大小
        /// </summary>
        public int CacheSize { get; set; } = 1000;
        
        /// <summary>
        /// 超时时间
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
        
        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
        
        /// <summary>
        /// 工作线程数
        /// </summary>
        public int WorkerCount { get; set; } = Environment.ProcessorCount;
        
        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; } = 3;
        
        /// <summary>
        /// 重试间隔
        /// </summary>
        public TimeSpan RetryInterval { get; set; } = TimeSpan.FromMilliseconds(500);
    }
    
    /// <summary>
    /// Craftsman服务接口
    /// 定义了Craftsman的核心功能
    /// </summary>
    public interface ICraftsmanService
    {
        /// <summary>
        /// 执行Craftsman操作
        /// </summary>
        /// <param name="input">输入参数</param>
        /// <returns>操作结果</returns>
        Task<CraftsmanResult> ExecuteAsync(CraftsmanInput input);
        
        /// <summary>
        /// 批量执行Craftsman操作
        /// </summary>
        /// <param name="inputs">输入参数列表</param>
        /// <returns>操作结果列表</returns>
        Task<IEnumerable<CraftsmanResult>> ExecuteBatchAsync(IEnumerable<CraftsmanInput> inputs);
        
        /// <summary>
        /// 获取Craftsman状态
        /// </summary>
        /// <returns>状态信息</returns>
        Task<CraftsmanStatus> GetStatusAsync();
        
        /// <summary>
        /// 重置Craftsman状态
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> ResetStatusAsync();
    }
    
    /// <summary>
    /// Craftsman输入参数
    /// </summary>
    public class CraftsmanInput
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public string OperationType { get; set; } = string.Empty;
        
        /// <summary>
        /// 操作参数
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public bool EnableCache { get; set; } = true;
    }
    
    /// <summary>
    /// Craftsman操作结果
    /// </summary>
    public class CraftsmanResult
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 操作结果数据
        /// </summary>
        public object? Data { get; set; }
        
        /// <summary>
        /// 错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// 执行时间（毫秒）
        /// </summary>
        public long ExecutionTimeMs { get; set; }
        
        /// <summary>
        /// 操作ID
        /// </summary>
        public string OperationId { get; set; } = Guid.NewGuid().ToString();
    }
    
    /// <summary>
    /// Craftsman状态信息
    /// </summary>
    public class CraftsmanStatus
    {
        /// <summary>
        /// 服务是否正常运行
        /// </summary>
        public bool IsRunning { get; set; }
        
        /// <summary>
        /// 已处理的操作数
        /// </summary>
        public long ProcessedOperations { get; set; }
        
        /// <summary>
        /// 成功的操作数
        /// </summary>
        public long SuccessfulOperations { get; set; }
        
        /// <summary>
        /// 失败的操作数
        /// </summary>
        public long FailedOperations { get; set; }
        
        /// <summary>
        /// 缓存命中率（百分比）
        /// </summary>
        public double CacheHitRate { get; set; }
        
        /// <summary>
        /// 平均执行时间（毫秒）
        /// </summary>
        public double AverageExecutionTimeMs { get; set; }
        
        /// <summary>
        /// 服务启动时间
        /// </summary>
        public DateTime StartTime { get; set; }
    }
    
    /// <summary>
    /// Craftsman服务实现
    /// 基于.NET 10 AOT架构，提供高性能Craftsman功能
    /// </summary>
    public class CraftsmanService : ICraftsmanService
    {
        private readonly ILogger<CraftsmanService> _logger;
        private readonly CraftsmanOptions _options;
        private readonly Dictionary<string, CraftsmanResult> _cache = new Dictionary<string, CraftsmanResult>();
        private long _processedOperations = 0;
        private long _successfulOperations = 0;
        private long _failedOperations = 0;
        private long _cacheHits = 0;
        private long _cacheMisses = 0;
        private readonly DateTime _startTime = DateTime.UtcNow;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="options">配置选项</param>
        public CraftsmanService(ILogger<CraftsmanService> logger, IOptions<CraftsmanOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            
            _logger.LogInformation("CraftsmanService初始化成功，配置选项：EnableCache={EnableCache}, CacheSize={CacheSize}, Timeout={Timeout}",
                _options.EnableCache, _options.CacheSize, _options.Timeout);
        }
        
        /// <summary>
        /// 执行Craftsman操作
        /// </summary>
        public async Task<CraftsmanResult> ExecuteAsync(CraftsmanInput input)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new CraftsmanResult();
            
            try
            {
                Interlocked.Increment(ref _processedOperations);
                
                // 生成缓存键
                var cacheKey = GenerateCacheKey(input);
                
                // 检查缓存
                if (_options.EnableCache && input.EnableCache && _cache.TryGetValue(cacheKey, out var cachedResult))
                {
                    Interlocked.Increment(ref _cacheHits);
                    _logger.LogDebug("Cache hit for operation: {OperationType}, CacheKey: {CacheKey}", input.OperationType, cacheKey);
                    return cachedResult;
                }
                
                if (_options.EnableCache && input.EnableCache)
                {
                    Interlocked.Increment(ref _cacheMisses);
                }
                
                _logger.LogInformation("开始执行Craftsman操作: {OperationType}, 参数: {@Parameters}", input.OperationType, input.Parameters);
                
                // 执行实际操作
                await Task.Delay(100); // 模拟操作延迟
                
                // 根据操作类型执行不同的逻辑
                switch (input.OperationType.ToLower())
                {
                    case "generate":
                        result = await GenerateAsync(input);
                        break;
                    case "build":
                        result = await BuildAsync(input);
                        break;
                    case "deploy":
                        result = await DeployAsync(input);
                        break;
                    case "test":
                        result = await TestAsync(input);
                        break;
                    default:
                        result.Success = false;
                        result.ErrorMessage = $"未知的操作类型: {input.OperationType}";
                        break;
                }
                
                if (result.Success)
                {
                    Interlocked.Increment(ref _successfulOperations);
                    
                    // 缓存结果
                    if (_options.EnableCache && input.EnableCache)
                    {
                        AddToCache(cacheKey, result);
                    }
                }
                else
                {
                    Interlocked.Increment(ref _failedOperations);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedOperations);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "执行Craftsman操作失败: {OperationType}", input.OperationType);
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }
            
            _logger.LogInformation("Craftsman操作执行完成: {OperationType}, 结果: {Success}, 执行时间: {ExecutionTimeMs}ms",
                input.OperationType, result.Success, result.ExecutionTimeMs);
            
            return result;
        }
        
        /// <summary>
        /// 批量执行Craftsman操作
        /// </summary>
        public async Task<IEnumerable<CraftsmanResult>> ExecuteBatchAsync(IEnumerable<CraftsmanInput> inputs)
        {
            _logger.LogInformation("开始批量执行Craftsman操作，操作数量: {Count}", inputs.Count());
            
            var tasks = inputs.Select(input => ExecuteAsync(input));
            var results = await Task.WhenAll(tasks);
            
            _logger.LogInformation("批量执行Craftsman操作完成，总操作数: {Total}, 成功: {Success}, 失败: {Failed}",
                results.Length, results.Count(r => r.Success), results.Count(r => !r.Success));
            
            return results;
        }
        
        /// <summary>
        /// 获取Craftsman状态
        /// </summary>
        public async Task<CraftsmanStatus> GetStatusAsync()
        {
            await Task.CompletedTask; // 模拟异步操作
            
            var totalCacheAccesses = _cacheHits + _cacheMisses;
            var cacheHitRate = totalCacheAccesses > 0 ? (double)_cacheHits / totalCacheAccesses * 100 : 0;
            
            var status = new CraftsmanStatus
            {
                IsRunning = true,
                ProcessedOperations = _processedOperations,
                SuccessfulOperations = _successfulOperations,
                FailedOperations = _failedOperations,
                CacheHitRate = Math.Round(cacheHitRate, 2),
                AverageExecutionTimeMs = 0, // 简化实现，实际应计算平均值
                StartTime = _startTime
            };
            
            _logger.LogDebug("获取Craftsman状态: {@Status}", status);
            
            return status;
        }
        
        /// <summary>
        /// 重置Craftsman状态
        /// </summary>
        public async Task<bool> ResetStatusAsync()
        {
            await Task.CompletedTask; // 模拟异步操作
            
            try
            {
                Interlocked.Exchange(ref _processedOperations, 0);
                Interlocked.Exchange(ref _successfulOperations, 0);
                Interlocked.Exchange(ref _failedOperations, 0);
                Interlocked.Exchange(ref _cacheHits, 0);
                Interlocked.Exchange(ref _cacheMisses, 0);
                
                lock (_cache)
                {
                    _cache.Clear();
                }
                
                _logger.LogInformation("Craftsman状态已重置");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "重置Craftsman状态失败");
                return false;
            }
        }
        
        /// <summary>
        /// 生成操作
        /// </summary>
        private async Task<CraftsmanResult> GenerateAsync(CraftsmanInput input)
        {
            await Task.Delay(150); // 模拟操作延迟
            
            return new CraftsmanResult
            {
                Success = true,
                Data = new { GeneratedFile = "output.txt", Size = 1024, Checksum = "abc123" }
            };
        }
        
        /// <summary>
        /// 构建操作
        /// </summary>
        private async Task<CraftsmanResult> BuildAsync(CraftsmanInput input)
        {
            await Task.Delay(200); // 模拟操作延迟
            
            return new CraftsmanResult
            {
                Success = true,
                Data = new { BuildResult = "success", Artifact = "app.zip", Size = 1024 * 1024 }
            };
        }
        
        /// <summary>
        /// 部署操作
        /// </summary>
        private async Task<CraftsmanResult> DeployAsync(CraftsmanInput input)
        {
            await Task.Delay(250); // 模拟操作延迟
            
            return new CraftsmanResult
            {
                Success = true,
                Data = new { DeploymentId = "deploy-123", Status = "running", Environment = "production" }
            };
        }
        
        /// <summary>
        /// 测试操作
        /// </summary>
        private async Task<CraftsmanResult> TestAsync(CraftsmanInput input)
        {
            await Task.Delay(180); // 模拟操作延迟
            
            return new CraftsmanResult
            {
                Success = true,
                Data = new { TestResult = "passed", TestsRun = 100, FailedTests = 0, Coverage = 95.5 }
            };
        }
        
        /// <summary>
        /// 生成缓存键
        /// </summary>
        private string GenerateCacheKey(CraftsmanInput input)
        {
            var parameterString = string.Join(",", input.Parameters.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}={kv.Value}"));
            return $"{input.OperationType}:{parameterString}";
        }
        
        /// <summary>
        /// 添加到缓存
        /// </summary>
        private void AddToCache(string cacheKey, CraftsmanResult result)
        {
            lock (_cache)
            {
                // 如果缓存已满，移除最旧的项
                if (_cache.Count >= _options.CacheSize)
                {
                    var oldestKey = _cache.Keys.First();
                    _cache.Remove(oldestKey);
                }
                
                _cache[cacheKey] = result;
            }
        }
    }
    
    /// <summary>
    /// Craftsman AOT执行引擎
    /// 管理Craftsman操作的执行
    /// </summary>
    public class CraftsmanAotEngine
    {
        private readonly ILogger<CraftsmanAotEngine> _logger;
        private readonly ICraftsmanService _craftsmanService;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="craftsmanService">Craftsman服务</param>
        public CraftsmanAotEngine(ILogger<CraftsmanAotEngine> logger, ICraftsmanService craftsmanService)
        {
            _logger = logger;
            _craftsmanService = craftsmanService;
            
            _logger.LogInformation("CraftsmanAotEngine初始化成功");
        }
        
        /// <summary>
        /// 执行Craftsman操作
        /// </summary>
        /// <param name="input">输入参数</param>
        /// <returns>操作结果</returns>
        public async Task<CraftsmanResult> ExecuteAsync(CraftsmanInput input)
        {
            return await _craftsmanService.ExecuteAsync(input);
        }
        
        /// <summary>
        /// 批量执行Craftsman操作
        /// </summary>
        /// <param name="inputs">输入参数列表</param>
        /// <returns>操作结果列表</returns>
        public async Task<IEnumerable<CraftsmanResult>> ExecuteBatchAsync(IEnumerable<CraftsmanInput> inputs)
        {
            return await _craftsmanService.ExecuteBatchAsync(inputs);
        }
        
        /// <summary>
        /// 获取Craftsman状态
        /// </summary>
        /// <returns>状态信息</returns>
        public async Task<CraftsmanStatus> GetStatusAsync()
        {
            return await _craftsmanService.GetStatusAsync();
        }
        
        /// <summary>
        /// 重置Craftsman状态
        /// </summary>
        /// <returns>操作结果</returns>
        public async Task<bool> ResetStatusAsync()
        {
            return await _craftsmanService.ResetStatusAsync();
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
            
            // 配置Craftsman选项
            builder.Configuration.AddJsonFile("craftsman_aot.setting.json", optional: true);
            builder.Services.Configure<CraftsmanOptions>(builder.Configuration.GetSection("Craftsman"));
            
            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);
            
            // 注册服务
            builder.Services.AddSingleton<ICraftsmanService, CraftsmanService>();
            builder.Services.AddSingleton<CraftsmanAotEngine>();
            
            // 构建主机
            var host = builder.Build();
            var serviceProvider = host.Services;
            
            // 获取引擎实例
            var engine = serviceProvider.GetRequiredService<CraftsmanAotEngine>();
            
            // 解析命令行参数
            if (args.Length < 1)
            {
                Console.WriteLine("用法:");
                Console.WriteLine("  craftsman_aot.exe <command> [arguments]");
                Console.WriteLine("  ");
                Console.WriteLine("命令:");
                Console.WriteLine("  execute <operationtype> [key=value...]	执行单个Craftsman操作");
                Console.WriteLine("  status	获取Craftsman状态");
                Console.WriteLine("  reset	重置Craftsman状态");
                Console.WriteLine("  ");
                Console.WriteLine("示例:");
                Console.WriteLine("  craftsman_aot.exe execute generate type=file name=output.txt");
                Console.WriteLine("  craftsman_aot.exe execute build configuration=release");
                Console.WriteLine("  craftsman_aot.exe status");
                return 1;
            }
            
            try
            {
                string command = args[0].ToLower();
                
                switch (command)
                {
                    case "execute":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("缺少操作类型参数");
                            return 1;
                        }
                        
                        // 解析操作类型和参数
                        string operationType = args[1];
                        var parameters = new Dictionary<string, string>();
                        
                        for (int i = 2; i < args.Length; i++)
                        {
                            var parts = args[i].Split('=', 2);
                            if (parts.Length == 2)
                            {
                                parameters[parts[0]] = parts[1];
                            }
                        }
                        
                        // 执行操作
                        var input = new CraftsmanInput
                        {
                            OperationType = operationType,
                            Parameters = parameters
                        };
                        
                        var result = await engine.ExecuteAsync(input);
                        
                        Console.WriteLine($"操作结果: {(result.Success ? "成功" : "失败")}");
                        if (!result.Success)
                        {
                            Console.WriteLine($"错误信息: {result.ErrorMessage}");
                        }
                        else if (result.Data != null)
                        {
                            Console.WriteLine($"结果数据: {Newtonsoft.Json.JsonConvert.SerializeObject(result.Data)}");
                        }
                        Console.WriteLine($"执行时间: {result.ExecutionTimeMs}ms");
                        
                        return result.Success ? 0 : 1;
                        
                    case "status":
                        var status = await engine.GetStatusAsync();
                        Console.WriteLine("Craftsman状态:");
                        Console.WriteLine($"  运行状态: {(status.IsRunning ? "正常" : "异常")}");
                        Console.WriteLine($"  已处理操作数: {status.ProcessedOperations}");
                        Console.WriteLine($"  成功操作数: {status.SuccessfulOperations}");
                        Console.WriteLine($"  失败操作数: {status.FailedOperations}");
                        Console.WriteLine($"  缓存命中率: {status.CacheHitRate}%");
                        Console.WriteLine($"  平均执行时间: {status.AverageExecutionTimeMs}ms");
                        Console.WriteLine($"  启动时间: {status.StartTime.ToLocalTime()}");
                        return 0;
                        
                    case "reset":
                        var resetResult = await engine.ResetStatusAsync();
                        Console.WriteLine($"重置状态: {(resetResult ? "成功" : "失败")}");
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