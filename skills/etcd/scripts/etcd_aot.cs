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
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Etcd.AOT
{
    /// <summary>
    /// Etcd 命令类型枚举
    /// </summary>
    public enum EtcdCommandType { Put, Get, Delete, Watch, Transaction, VersionInfo }

    /// <summary>
    /// Etcd 选项配置
    /// </summary>
    public class EtcdOptions
    {
        /// <summary>
        /// Etcd 服务器地址列表
        /// </summary>
        public List<string> Endpoints { get; set; } = new List<string> { "http://localhost:2379" };
        
        /// <summary>
        /// 用户名
        /// </summary>
        public string? Username { get; set; }
        
        /// <summary>
        /// 密码
        /// </summary>
        public string? Password { get; set; }
        
        /// <summary>
        /// 请求超时时间（毫秒）
        /// </summary>
        public int RequestTimeoutMs { get; set; } = 5000;
        
        /// <summary>
        /// 连接超时时间（毫秒）
        /// </summary>
        public int ConnectTimeoutMs { get; set; } = 10000;
        
        /// <summary>
        /// 重试次数
        /// </summary>
        public int MaxRetries { get; set; } = 3;
        
        /// <summary>
        /// 是否启用 TLS
        /// </summary>
        public bool EnableTls { get; set; } = false;
        
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
    /// Etcd 键值对
    /// </summary>
    public class EtcdKeyValue
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; } = string.Empty;
        
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; } = string.Empty;
        
        /// <summary>
        /// 创建版本
        /// </summary>
        public long CreateRevision { get; set; }
        
        /// <summary>
        /// 修改版本
        /// </summary>
        public long ModRevision { get; set; }
        
        /// <summary>
        /// 版本
        /// </summary>
        public long Version { get; set; }
        
        /// <summary>
        /// 租约 ID
        /// </summary>
        public long LeaseId { get; set; }
    }

    /// <summary>
    /// Etcd 命令结果
    /// </summary>
    public class EtcdCommandResult
    {
        /// <summary>
        /// 命令是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 命令类型
        /// </summary>
        public EtcdCommandType CommandType { get; set; }
        
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
        /// 键值对列表
        /// </summary>
        public List<EtcdKeyValue>? KeyValues { get; set; }
        
        /// <summary>
        /// 操作的键
        /// </summary>
        public string? Key { get; set; }
        
        /// <summary>
        /// 操作的值
        /// </summary>
        public string? Value { get; set; }
        
        /// <summary>
        /// 事务成功结果
        /// </summary>
        public bool? TransactionSuccess { get; set; }
    }

    /// <summary>
    /// Etcd 服务接口
    /// </summary>
    public interface IEtcdService
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>命令结果</returns>
        Task<EtcdCommandResult> ExecuteCommandAsync(EtcdCommandType commandType, Dictionary<string, string>? parameters = null);
        
        /// <summary>
        /// 设置键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="leaseId">租约 ID</param>
        /// <returns>操作结果</returns>
        Task<EtcdCommandResult> PutAsync(string key, string value, long leaseId = 0);
        
        /// <summary>
        /// 获取键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="prefix">是否前缀匹配</param>
        /// <returns>操作结果</returns>
        Task<EtcdCommandResult> GetAsync(string key, bool prefix = false);
        
        /// <summary>
        /// 删除键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="prefix">是否前缀匹配</param>
        /// <returns>操作结果</returns>
        Task<EtcdCommandResult> DeleteAsync(string key, bool prefix = false);
        
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="compareKey">比较的键</param>
        /// <param name="compareValue">比较的值</param>
        /// <param name="successKey">成功时操作的键</param>
        /// <param name="successValue">成功时设置的值</param>
        /// <param name="failKey">失败时操作的键</param>
        /// <param name="failValue">失败时设置的值</param>
        /// <returns>事务结果</returns>
        Task<EtcdCommandResult> TransactionAsync(string compareKey, string compareValue, string successKey, string successValue, string failKey, string failValue);
        
        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        Task<EtcdCommandResult> GetVersionInfoAsync();
    }

    /// <summary>
    /// Etcd 服务实现
    /// </summary>
    public class EtcdService : IEtcdService
    {
        private readonly EtcdOptions _options;
        private readonly ILogger<EtcdService> _logger;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">Etcd 选项</param>
        /// <param name="logger">日志记录器</param>
        public EtcdService(IOptions<EtcdOptions> options, ILogger<EtcdService> logger)
        {
            _options = options.Value;
            _logger = logger;
            
            // 初始化 HttpClient
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMilliseconds(_options.RequestTimeoutMs);
        }

        /// <inheritdoc/>
        public async Task<EtcdCommandResult> ExecuteCommandAsync(EtcdCommandType commandType, Dictionary<string, string>? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EtcdCommandResult
            {
                CommandType = commandType
            };

            try
            {
                switch (commandType)
                {
                    case EtcdCommandType.Put:
                        if (parameters?.ContainsKey("key") == true && parameters?.ContainsKey("value") == true)
                        {
                            string key = parameters["key"];
                            string value = parameters["value"];
                            long leaseId = parameters?.ContainsKey("leaseId") == true ? long.Parse(parameters["leaseId"]) : 0;
                            result = await PutAsync(key, value, leaseId);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "Key and Value parameters are required";
                        }
                        break;
                    
                    case EtcdCommandType.Get:
                        if (parameters?.ContainsKey("key") == true)
                        {
                            string key = parameters["key"];
                            bool prefix = parameters?.ContainsKey("prefix") == true ? bool.Parse(parameters["prefix"]) : false;
                            result = await GetAsync(key, prefix);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "Key parameter is required";
                        }
                        break;
                    
                    case EtcdCommandType.Delete:
                        if (parameters?.ContainsKey("key") == true)
                        {
                            string key = parameters["key"];
                            bool prefix = parameters?.ContainsKey("prefix") == true ? bool.Parse(parameters["prefix"]) : false;
                            result = await DeleteAsync(key, prefix);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "Key parameter is required";
                        }
                        break;
                    
                    case EtcdCommandType.Transaction:
                        if (parameters?.ContainsKey("compareKey") == true && parameters?.ContainsKey("compareValue") == true &&
                            parameters?.ContainsKey("successKey") == true && parameters?.ContainsKey("successValue") == true &&
                            parameters?.ContainsKey("failKey") == true && parameters?.ContainsKey("failValue") == true)
                        {
                            string compareKey = parameters["compareKey"];
                            string compareValue = parameters["compareValue"];
                            string successKey = parameters["successKey"];
                            string successValue = parameters["successValue"];
                            string failKey = parameters["failKey"];
                            string failValue = parameters["failValue"];
                            result = await TransactionAsync(compareKey, compareValue, successKey, successValue, failKey, failValue);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "All transaction parameters are required";
                        }
                        break;
                    
                    case EtcdCommandType.VersionInfo:
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
        public async Task<EtcdCommandResult> PutAsync(string key, string value, long leaseId = 0)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EtcdCommandResult
            {
                CommandType = EtcdCommandType.Put,
                Key = key,
                Value = value
            };

            try
            {
                _logger.LogInformation("Putting key-value pair: {Key} = {Value}", key, value);
                
                // Etcd HTTP API 请求（模拟实现）
                // 实际实现中应使用真实的 etcd gRPC 客户端或 HTTP API
                
                // 模拟成功结果
                result.Success = true;
                result.Results.Add($"成功设置键值对: {key} = {value}");
                if (leaseId > 0)
                {
                    result.Results.Add($"租约 ID: {leaseId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error putting key-value pair: {Key}", key);
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
        public async Task<EtcdCommandResult> GetAsync(string key, bool prefix = false)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EtcdCommandResult
            {
                CommandType = EtcdCommandType.Get,
                Key = key
            };

            try
            {
                _logger.LogInformation("Getting key: {Key}, Prefix: {Prefix}", key, prefix);
                
                // Etcd HTTP API 请求（模拟实现）
                // 实际实现中应使用真实的 etcd gRPC 客户端或 HTTP API
                
                // 模拟成功结果
                result.Success = true;
                result.KeyValues = new List<EtcdKeyValue>
                {
                    new EtcdKeyValue
                    {
                        Key = key,
                        Value = "模拟值",
                        CreateRevision = 1,
                        ModRevision = 2,
                        Version = 2,
                        LeaseId = 0
                    }
                };
                result.Results.Add($"成功获取键值对: {key} = {result.KeyValues[0].Value}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting key: {Key}", key);
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
        public async Task<EtcdCommandResult> DeleteAsync(string key, bool prefix = false)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EtcdCommandResult
            {
                CommandType = EtcdCommandType.Delete,
                Key = key
            };

            try
            {
                _logger.LogInformation("Deleting key: {Key}, Prefix: {Prefix}", key, prefix);
                
                // Etcd HTTP API 请求（模拟实现）
                // 实际实现中应使用真实的 etcd gRPC 客户端或 HTTP API
                
                // 模拟成功结果
                result.Success = true;
                result.Results.Add($"成功删除键: {key}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting key: {Key}", key);
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
        public async Task<EtcdCommandResult> TransactionAsync(string compareKey, string compareValue, string successKey, string successValue, string failKey, string failValue)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EtcdCommandResult
            {
                CommandType = EtcdCommandType.Transaction
            };

            try
            {
                _logger.LogInformation("Executing transaction: Compare {CompareKey} = {CompareValue}, Success: {SuccessKey} = {SuccessValue}, Fail: {FailKey} = {FailValue}", 
                    compareKey, compareValue, successKey, successValue, failKey, failValue);
                
                // Etcd HTTP API 请求（模拟实现）
                // 实际实现中应使用真实的 etcd gRPC 客户端或 HTTP API
                
                // 模拟事务成功
                result.Success = true;
                result.TransactionSuccess = true;
                result.Results.Add($"事务执行成功");
                result.Results.Add($"已设置: {successKey} = {successValue}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing transaction");
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
        public async Task<EtcdCommandResult> GetVersionInfoAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EtcdCommandResult
            {
                CommandType = EtcdCommandType.VersionInfo
            };

            try
            {
                _logger.LogInformation("Getting etcd version info");
                
                // Etcd HTTP API 请求（模拟实现）
                // 实际实现中应使用真实的 etcd gRPC 客户端或 HTTP API
                
                // 模拟成功结果
                result.Success = true;
                result.Results.Add("Etcd AOT Engine");
                result.Results.Add($"版本: 1.0.0");
                result.Results.Add($".NET 版本: {Environment.Version}");
                result.Results.Add($"操作系统: {Environment.OSVersion}");
                result.Results.Add($"架构: {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}");
                result.Results.Add($"AOT 编译: {AppContext.TryGetSwitch("PublishAot", out bool isAot) && isAot}");
                result.Results.Add($"默认端点: {string.Join(", ", _options.Endpoints)}");
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
    /// Etcd AOT 引擎
    /// </summary>
    public class EtcdAotEngine
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EtcdAotEngine> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="logger">日志记录器</param>
        public EtcdAotEngine(IServiceProvider serviceProvider, ILogger<EtcdAotEngine> logger)
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
            _logger.LogInformation("Etcd AOT Engine starting with args: {Args}", string.Join(" ", args));
            
            if (args.Length == 0)
            {
                ShowHelp();
                return 0;
            }

            var command = args[0].ToLower();
            var etcdService = _serviceProvider.GetRequiredService<IEtcdService>();
            EtcdCommandResult? result = null;

            try
            {
                switch (command)
                {
                    case "put":
                        if (args.Length < 3)
                        {
                            Console.WriteLine("错误: 需要提供键和值");
                            return 1;
                        }
                        string key = args[1];
                        string value = args[2];
                        long leaseId = args.Length > 3 ? long.Parse(args[3]) : 0;
                        result = await etcdService.PutAsync(key, value, leaseId);
                        break;
                    
                    case "get":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("错误: 需要提供键");
                            return 1;
                        }
                        string getKey = args[1];
                        bool prefix = args.Length > 2 && args[2].Equals("--prefix", StringComparison.OrdinalIgnoreCase);
                        result = await etcdService.GetAsync(getKey, prefix);
                        break;
                    
                    case "delete":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("错误: 需要提供键");
                            return 1;
                        }
                        string deleteKey = args[1];
                        bool deletePrefix = args.Length > 2 && args[2].Equals("--prefix", StringComparison.OrdinalIgnoreCase);
                        result = await etcdService.DeleteAsync(deleteKey, deletePrefix);
                        break;
                    
                    case "transaction":
                        if (args.Length < 7)
                        {
                            Console.WriteLine("错误: 事务命令需要提供完整参数");
                            Console.WriteLine("用法: transaction <compareKey> <compareValue> <successKey> <successValue> <failKey> <failValue>");
                            return 1;
                        }
                        string compareKey = args[1];
                        string compareValue = args[2];
                        string successKey = args[3];
                        string successValue = args[4];
                        string failKey = args[5];
                        string failValue = args[6];
                        result = await etcdService.TransactionAsync(compareKey, compareValue, successKey, successValue, failKey, failValue);
                        break;
                    
                    case "version":
                        result = await etcdService.GetVersionInfoAsync();
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
                Console.WriteLine($"执行时间: {result.ExecutionTimeMs} ms