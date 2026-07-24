#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
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

namespace DTM.AOT
{
    /// <summary>
    /// DTM 命令类型枚举
    /// </summary>
    public enum DtmCommandType
    {
        /// <summary>
        /// 执行分布式事务
        /// </summary>
        ExecuteTransaction,
        /// <summary>
        /// 查询事务状态
        /// </summary>
        QueryTransactionStatus,
        /// <summary>
        /// 取消事务
        /// </summary>
        CancelTransaction,
        /// <summary>
        /// 恢复事务
        /// </summary>
        ResumeTransaction,
        /// <summary>
        /// 清理过期事务
        /// </summary>
        CleanExpiredTransactions,
        /// <summary>
        /// 显示版本信息
        /// </summary>
        VersionInfo
    }

    /// <summary>
    /// DTM 选项配置
    /// </summary>
    public class DtmOptions
    {
        /// <summary>
        /// 事务默认超时时间（毫秒）
        /// </summary>
        public int DefaultTimeoutMs { get; set; } = 30000;
        
        /// <summary>
        /// 是否启用事务缓存
        /// </summary>
        public bool EnableTransactionCache { get; set; } = true;
        
        /// <summary>
        /// 最大缓存事务数量
        /// </summary>
        public int MaxCacheSize { get; set; } = 1000;
        
        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
        
        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;
        
        /// <summary>
        /// 事务清理间隔（毫秒）
        /// </summary>
        public int TransactionCleanupIntervalMs { get; set; } = 60000;
        
        /// <summary>
        /// 事务过期时间（毫秒）
        /// </summary>
        public int TransactionExpiryMs { get; set; } = 3600000;
    }

    /// <summary>
    /// DTM 命令结果
    /// </summary>
    public class DtmCommandResult
    {
        /// <summary>
        /// 命令是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 命令类型
        /// </summary>
        public DtmCommandType CommandType { get; set; }
        
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
        /// 事务ID
        /// </summary>
        public string? TransactionId { get; set; }
    }

    /// <summary>
    /// DTM 服务接口
    /// </summary>
    public interface IDtmService
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>命令结果</returns>
        Task<DtmCommandResult> ExecuteCommandAsync(DtmCommandType commandType, Dictionary<string, string>? parameters = null);
        
        /// <summary>
        /// 执行分布式事务
        /// </summary>
        /// <param name="transactionId">事务ID</param>
        /// <param name="operations">事务操作列表</param>
        /// <param name="timeoutMs">超时时间（毫秒）</param>
        /// <returns>事务执行结果</returns>
        Task<DtmCommandResult> ExecuteTransactionAsync(string? transactionId, List<string> operations, int? timeoutMs = null);
        
        /// <summary>
        /// 查询事务状态
        /// </summary>
        /// <param name="transactionId">事务ID</param>
        /// <returns>事务状态</returns>
        Task<DtmCommandResult> QueryTransactionStatusAsync(string transactionId);
        
        /// <summary>
        /// 取消事务
        /// </summary>
        /// <param name="transactionId">事务ID</param>
        /// <returns>取消结果</returns>
        Task<DtmCommandResult> CancelTransactionAsync(string transactionId);
        
        /// <summary>
        /// 恢复事务
        /// </summary>
        /// <param name="transactionId">事务ID</param>
        /// <returns>恢复结果</returns>
        Task<DtmCommandResult> ResumeTransactionAsync(string transactionId);
        
        /// <summary>
        /// 清理过期事务
        /// </summary>
        /// <returns>清理结果</returns>
        Task<DtmCommandResult> CleanExpiredTransactionsAsync();
        
        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        Task<DtmCommandResult> GetVersionInfoAsync();
    }

    /// <summary>
    /// DTM 服务实现
    /// </summary>
    public class DtmService : IDtmService
    {
        private readonly DtmOptions _options;
        private readonly ILogger<DtmService> _logger;
        private readonly Dictionary<string, object> _transactionCache = new Dictionary<string, object>();
        private readonly object _cacheLock = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">DTM 选项</param>
        /// <param name="logger">日志记录器</param>
        public DtmService(IOptions<DtmOptions> options, ILogger<DtmService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<DtmCommandResult> ExecuteCommandAsync(DtmCommandType commandType, Dictionary<string, string>? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DtmCommandResult
            {
                CommandType = commandType
            };

            try
            {
                switch (commandType)
                {
                    case DtmCommandType.ExecuteTransaction:
                        if (parameters?.ContainsKey("transactionId") == true)
                        {
                            string transactionId = parameters["transactionId"];
                            List<string> operations = parameters?.ContainsKey("operations") == true ? 
                                parameters["operations"].Split(',').ToList() : new List<string>();
                            int? timeoutMs = parameters?.ContainsKey("timeoutMs") == true ? int.Parse(parameters["timeoutMs"]) : null;
                            result = await ExecuteTransactionAsync(transactionId, operations, timeoutMs);
                        }
                        else
                        {
                            List<string> operations = parameters?.ContainsKey("operations") == true ? 
                                parameters["operations"].Split(',').ToList() : new List<string>();
                            int? timeoutMs = parameters?.ContainsKey("timeoutMs") == true ? int.Parse(parameters["timeoutMs"]) : null;
                            result = await ExecuteTransactionAsync(null, operations, timeoutMs);
                        }
                        break;
                    
                    case DtmCommandType.QueryTransactionStatus:
                        if (parameters?.ContainsKey("transactionId") == true)
                        {
                            string transactionId = parameters["transactionId"];
                            result = await QueryTransactionStatusAsync(transactionId);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "TransactionId parameter is required";
                        }
                        break;
                    
                    case DtmCommandType.CancelTransaction:
                        if (parameters?.ContainsKey("transactionId") == true)
                        {
                            string transactionId = parameters["transactionId"];
                            result = await CancelTransactionAsync(transactionId);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "TransactionId parameter is required";
                        }
                        break;
                    
                    case DtmCommandType.ResumeTransaction:
                        if (parameters?.ContainsKey("transactionId") == true)
                        {
                            string transactionId = parameters["transactionId"];
                            result = await ResumeTransactionAsync(transactionId);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "TransactionId parameter is required";
                        }
                        break;
                    
                    case DtmCommandType.CleanExpiredTransactions:
                        result = await CleanExpiredTransactionsAsync();
                        break;
                    
                    case DtmCommandType.VersionInfo:
                        result = await GetVersionInfoAsync();
                        break;
                    
                    default:
                        result.Success = false;
                        result.ErrorMessage = $"Unknown command type: {commandType}";
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing command: {CommandType}", commandType);
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
        public async Task<DtmCommandResult> ExecuteTransactionAsync(string? transactionId, List<string> operations, int? timeoutMs = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DtmCommandResult
            {
                CommandType = DtmCommandType.ExecuteTransaction
            };

            try
            {
                // 生成事务ID（如果未提供）
                string actualTransactionId = transactionId ?? Guid.NewGuid().ToString();
                result.TransactionId = actualTransactionId;
                
                _logger.LogInformation("Executing distributed transaction: {TransactionId}", actualTransactionId);
                _logger.LogDebug("Transaction operations: {Operations}", string.Join(", ", operations));
                
                // 模拟事务执行逻辑
                // 在实际实现中，这里会调用DTM服务进行分布式事务管理
                
                // 模拟执行每个操作
                foreach (var operation in operations)
                {
                    _logger.LogDebug("Executing operation: {Operation}", operation);
                    // 模拟操作执行延迟
                    await Task.Delay(10);
                }
                
                // 存储事务信息到缓存
                if (_options.EnableTransactionCache)
                {
                    lock (_cacheLock)
                    {
                        // 如果缓存已满，移除最早的事务
                        if (_transactionCache.Count >= _options.MaxCacheSize)
                        {
                            var oldestKey = _transactionCache.Keys.First();
                            _transactionCache.Remove(oldestKey);
                        }
                        
                        _transactionCache[actualTransactionId] = new {
                            Status = "Completed",
                            Operations = operations,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                    }
                }
                
                result.Success = true;
                result.Results.Add($"Transaction {actualTransactionId} executed successfully");
                result.Results.Add($"Operations: {operations.Count}");
                result.Results.Add($"Status: Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing transaction: {TransactionId}", transactionId);
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
        public async Task<DtmCommandResult> QueryTransactionStatusAsync(string transactionId)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DtmCommandResult
            {
                CommandType = DtmCommandType.QueryTransactionStatus,
                TransactionId = transactionId
            };

            try
            {
                _logger.LogInformation("Querying transaction status: {TransactionId}", transactionId);
                
                // 从缓存中查询事务状态
                if (_options.EnableTransactionCache)
                {
                    lock (_cacheLock)
                    {
                        if (_transactionCache.TryGetValue(transactionId, out var transactionInfo))
                        {
                            result.Success = true;
                            result.Results.Add($"Status: Completed");
                            result.Results.Add($"Transaction found in cache");
                        }
                        else
                        {
                            result.Success = true;
                            result.Results.Add($"Status: Not Found");
                            result.Results.Add($"Transaction not found in cache");
                        }
                    }
                }
                else
                {
                    // 模拟查询逻辑
                    result.Success = true;
                    result.Results.Add($"Status: Completed");
                    result.Results.Add($"Cache disabled");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying transaction status: {TransactionId}", transactionId);
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
        public async Task<DtmCommandResult> CancelTransactionAsync(string transactionId)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DtmCommandResult
            {
                CommandType = DtmCommandType.CancelTransaction,
                TransactionId = transactionId
            };

            try
            {
                _logger.LogInformation("Canceling transaction: {TransactionId}", transactionId);
                
                // 模拟取消事务逻辑
                
                // 更新缓存中的事务状态
                if (_options.EnableTransactionCache)
                {
                    lock (_cacheLock)
                    {
                        if (_transactionCache.TryGetValue(transactionId, out var transactionInfo))
                        {
                            _transactionCache[transactionId] = new {
                                Status = "Cancelled",
                                UpdatedAt = DateTime.UtcNow
                            };
                        }
                    }
                }
                
                result.Success = true;
                result.Results.Add($"Transaction {transactionId} canceled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling transaction: {TransactionId}", transactionId);
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
        public async Task<DtmCommandResult> ResumeTransactionAsync(string transactionId)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DtmCommandResult
            {
                CommandType = DtmCommandType.ResumeTransaction,
                TransactionId = transactionId
            };

            try
            {
                _logger.LogInformation("Resuming transaction: {TransactionId}", transactionId);
                
                // 模拟恢复事务逻辑
                
                // 更新缓存中的事务状态
                if (_options.EnableTransactionCache)
                {
                    lock (_cacheLock)
                    {
                        if (_transactionCache.TryGetValue(transactionId, out var transactionInfo))
                        {
                            _transactionCache[transactionId] = new {
                                Status = "Resumed",
                                UpdatedAt = DateTime.UtcNow
                            };
                        }
                    }
                }
                
                result.Success = true;
                result.Results.Add($"Transaction {transactionId} resumed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resuming transaction: {TransactionId}", transactionId);
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
        public async Task<DtmCommandResult> CleanExpiredTransactionsAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DtmCommandResult
            {
                CommandType = DtmCommandType.CleanExpiredTransactions
            };

            try
            {
                _logger.LogInformation("Cleaning expired transactions");
                
                int cleanedCount = 0;
                
                // 清理过期事务
                if (_options.EnableTransactionCache)
                {
                    lock (_cacheLock)
                    {
                        var now = DateTime.UtcNow;
                        var expiredKeys = _transactionCache.Keys.Where(key => {
                            var transaction = _transactionCache[key];
                            // 模拟检查过期时间
                            return (now - DateTime.UtcNow).TotalMilliseconds > _options.TransactionExpiryMs;
                        }).ToList();
                        
                        foreach (var key in expiredKeys)
                        {
                            _transactionCache.Remove(key);
                            cleanedCount++;
                        }
                    }
                }
                
                result.Success = true;
                result.Results.Add($"Cleaned {cleanedCount} expired transactions");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning expired transactions");
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
        public async Task<DtmCommandResult> GetVersionInfoAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DtmCommandResult
            {
                CommandType = DtmCommandType.VersionInfo
            };

            try
            {
                _logger.LogInformation("Getting version info");
                
                result.Success = true;
                result.Results.Add("DTM AOT Engine");
                result.Results.Add($"Version: 1.0.0");
                result.Results.Add($".NET Version: {Environment.Version}");
                result.Results.Add($"OS: {Environment.OSVersion}");
                result.Results.Add($"Architecture: {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}");
                result.Results.Add($"AOT Compiled: {AppContext.TryGetSwitch("PublishAot", out bool isAot) && isAot}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting version info");
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
    /// DTM AOT 引擎
    /// </summary>
    public class DtmAotEngine
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DtmAotEngine> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="logger">日志记录器</param>
        public DtmAotEngine(IServiceProvider serviceProvider, ILogger<DtmAotEngine> logger)
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
            _logger.LogInformation("DTM AOT Engine starting with args: {Args}", string.Join(" ", args));
            
            if (args.Length == 0)
            {
                ShowHelp();
                return 0;
            }

            var command = args[0].ToLower();
            var dtmService = _serviceProvider.GetRequiredService<IDtmService>();
            DtmCommandResult? result = null;

            try
            {
                switch (command)
                {
                    case "execute":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Error: Transaction operations are required");
                            return 1;
                        }
                        
                        string transactionId = args.Length > 2 ? args[2] : null;
                        List<string> operations = args[1].Split(',').ToList();
                        
                        result = await dtmService.ExecuteTransactionAsync(transactionId, operations);
                        break;
                    
                    case "query":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Error: Transaction ID is required");
                            return 1;
                        }
                        
                        string queryTransactionId = args[1];
                        result = await dtmService.QueryTransactionStatusAsync(queryTransactionId);
                        break;
                    
                    case "cancel":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Error: Transaction ID is required");
                            return 1;
                        }
                        
                        string cancelTransactionId = args[1];
                        result = await dtmService.CancelTransactionAsync(cancelTransactionId);
                        break;
                    
                    case "resume":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Error: Transaction ID is required");
                            return 1;
                        }
                        
                        string resumeTransactionId = args[1];
                        result = await dtmService.ResumeTransactionAsync(resumeTransactionId);
                        break;
                    
                    case "clean":
                        result = await dtmService.CleanExpiredTransactionsAsync();
                        break;
                    
                    case "version":
                        result = await dtmService.GetVersionInfoAsync();
                        break;
                    
                    case "help":
                    case "--help":
                    case "-h":
                        ShowHelp();
                        return 0;
                    
                    default:
                        Console.WriteLine($"Error: Unknown command '{command}'");
                        ShowHelp();
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                _logger.LogError(ex, "Error executing command: {Command}", command);
                return 1;
            }

            if (result != null)
            {
                DisplayResult(result);
                return result.Success ? 0 : 1;
            }

            return 0;
        }

        private void ShowHelp()
        {
            Console.WriteLine("DTM AOT Command Line Tool");
            Console.WriteLine("=================================");
            Console.WriteLine("Usage: dtm_aot <command> [options]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  execute <operations> [transactionId]  Execute distributed transaction");
            Console.WriteLine("  query <transactionId>                  Query transaction status");
            Console.WriteLine("  cancel <transactionId>                 Cancel transaction");
            Console.WriteLine("  resume <transactionId>                 Resume transaction");
            Console.WriteLine("  clean                                  Clean expired transactions");
            Console.WriteLine("  version                                Show version information");
            Console.WriteLine("  help, --help, -h                       Show this help message");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dtm_aot execute 'op1,op2,op3'          Execute transaction with operations");
            Console.WriteLine("  dtm_aot execute 'op1,op2' tx123        Execute transaction with specific ID");
            Console.WriteLine("  dtm_aot query tx123                    Query transaction status");
            Console.WriteLine("  dtm_aot cancel tx123                   Cancel transaction");
            Console.WriteLine("  dtm_aot resume tx123                   Resume transaction");
            Console.WriteLine("  dtm_aot clean                          Clean expired transactions");
            Console.WriteLine("  dtm_aot version                        Show version");
        }

        private void DisplayResult(DtmCommandResult result)
        {
            Console.WriteLine($"Command: {result.CommandType}");
            Console.WriteLine($"Status: {(result.Success ? "Success" : "Failed")}");
            Console.WriteLine($"Time: {result.ExecutionTimeMs} ms");
            
            if (!string.IsNullOrEmpty(result.TransactionId))
            {
                Console.WriteLine($"Transaction ID: {result.TransactionId}");
            }
            
            if (!result.Success && !string.IsNullOrEmpty(result.ErrorMessage))
            {
                Console.WriteLine();
                Console.WriteLine($"Error: {result.ErrorMessage}");
            }
            else if (result.Results.Any())
            {
                Console.WriteLine();
                Console.WriteLine("Results:");
                foreach (var item in result.Results)
                {
                    Console.WriteLine($"  {item}");
                }
            }
        }
    }
}

// 扩展方法
public static class DtmServiceExtensions
{
    /// <summary>
    /// 添加 DTM 服务到依赖注入容器
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDtm(this IServiceCollection services)
    {
        services.AddSingleton<DTM.AOT.IDtmService, DTM.AOT.DtmService>();
        services.AddSingleton<DTM.AOT.DtmAotEngine>();
        return services;
    }
}

// 主程序
class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("dtm_aot.setting.json", optional: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<DTM.AOT.DtmOptions>(context.Configuration.GetSection("Dtm"));
                services.AddDtm();
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .Build();

        var engine = host.Services.GetRequiredService<DTM.AOT.DtmAotEngine>();
        var exitCode = await engine.ExecuteCommandLineAsync(args);
        Environment.Exit(exitCode);
    }
}