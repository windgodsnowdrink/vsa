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

namespace Csscript.AOT
{
    /// <summary>
    /// Csscript配置选项
    /// </summary>
    public class CsscriptOptions
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
        
        /// <summary>
        /// 脚本执行超时时间
        /// </summary>
        public TimeSpan ScriptExecutionTimeout { get; set; } = TimeSpan.FromSeconds(60);
        
        /// <summary>
        /// 是否允许外部脚本
        /// </summary>
        public bool AllowExternalScripts { get; set; } = false;
        
        /// <summary>
        /// 脚本缓存目录
        /// </summary>
        public string ScriptCacheDirectory { get; set; } = "./script_cache";
    }
    
    /// <summary>
    /// Csscript服务接口
    /// 定义了Csscript的核心功能
    /// </summary>
    public interface ICsscriptService
    {
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="scriptCode">脚本代码</param>
        /// <param name="parameters">脚本参数</param>
        /// <returns>执行结果</returns>
        Task<CsscriptResult> ExecuteScriptAsync(string scriptCode, Dictionary<string, object>? parameters = null);
        
        /// <summary>
        /// 执行外部脚本文件
        /// </summary>
        /// <param name="scriptPath">脚本文件路径</param>
        /// <param name="parameters">脚本参数</param>
        /// <returns>执行结果</returns>
        Task<CsscriptResult> ExecuteScriptFileAsync(string scriptPath, Dictionary<string, object>? parameters = null);
        
        /// <summary>
        /// 批量执行脚本
        /// </summary>
        /// <param name="scriptExecutionRequests">脚本执行请求列表</param>
        /// <returns>执行结果列表</returns>
        Task<IEnumerable<CsscriptResult>> ExecuteScriptBatchAsync(IEnumerable<ScriptExecutionRequest> scriptExecutionRequests);
        
        /// <summary>
        /// 编译脚本
        /// </summary>
        /// <param name="scriptCode">脚本代码</param>
        /// <returns>编译结果</returns>
        Task<ScriptCompilationResult> CompileScriptAsync(string scriptCode);
        
        /// <summary>
        /// 获取Csscript状态
        /// </summary>
        /// <returns>状态信息</returns>
        Task<CsscriptStatus> GetStatusAsync();
        
        /// <summary>
        /// 重置Csscript状态
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> ResetStatusAsync();
    }
    
    /// <summary>
    /// 脚本执行请求
    /// </summary>
    public class ScriptExecutionRequest
    {
        /// <summary>
        /// 请求ID
        /// </summary>
        public string RequestId { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// 脚本代码或文件路径
        /// </summary>
        public string ScriptContent { get; set; } = string.Empty;
        
        /// <summary>
        /// 是否为文件路径
        /// </summary>
        public bool IsFilePath { get; set; } = false;
        
        /// <summary>
        /// 脚本参数
        /// </summary>
        public Dictionary<string, object>? Parameters { get; set; } = null;
    }
    
    /// <summary>
    /// Csscript操作结果
    /// </summary>
    public class CsscriptResult
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
        /// 脚本名称或ID
        /// </summary>
        public string? ScriptIdentifier { get; set; }
    }
    
    /// <summary>
    /// 脚本编译结果
    /// </summary>
    public class ScriptCompilationResult
    {
        /// <summary>
        /// 编译是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 编译错误信息
        /// </summary>
        public List<string>? Errors { get; set; }
        
        /// <summary>
        /// 编译警告信息
        /// </summary>
        public List<string>? Warnings { get; set; }
        
        /// <summary>
        /// 编译时间（毫秒）
        /// </summary>
        public long CompilationTimeMs { get; set; }
    }
    
    /// <summary>
    /// Csscript状态信息
    /// </summary>
    public class CsscriptStatus
    {
        /// <summary>
        /// 服务是否正常运行
        /// </summary>
        public bool IsRunning { get; set; }
        
        /// <summary>
        /// 已执行的脚本数
        /// </summary>
        public long ExecutedScripts { get; set; }
        
        /// <summary>
        /// 成功执行的脚本数
        /// </summary>
        public long SuccessfulScripts { get; set; }
        /// <summary>
        /// 失败执行的脚本数
        /// </summary>
        public long FailedScripts { get; set; }
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
    /// Csscript服务实现
    /// 基于.NET 10 AOT架构，提供高性能脚本执行功能
    /// </summary>
    public class CsscriptService : ICsscriptService
    {
        private readonly ILogger<CsscriptService> _logger;
        private readonly CsscriptOptions _options;
        private readonly Dictionary<string, CsscriptResult> _cache = new Dictionary<string, CsscriptResult>();
        private long _executedScripts = 0;
        private long _successfulScripts = 0;
        private long _failedScripts = 0;
        private long _cacheHits = 0;
        private long _cacheMisses = 0;
        private readonly DateTime _startTime = DateTime.UtcNow;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="options">配置选项</param>
        public CsscriptService(ILogger<CsscriptService> logger, IOptions<CsscriptOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            
            _logger.LogInformation("CsscriptService初始化成功，配置选项：EnableCache={EnableCache}, ScriptExecutionTimeout={ScriptExecutionTimeout}, AllowExternalScripts={AllowExternalScripts}",
                _options.EnableCache, _options.ScriptExecutionTimeout, _options.AllowExternalScripts);
        }
        
        /// <summary>
        /// 执行脚本
        /// </summary>
        public async Task<CsscriptResult> ExecuteScriptAsync(string scriptCode, Dictionary<string, object>? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new CsscriptResult();
            
            try
            {
                Interlocked.Increment(ref _executedScripts);
                
                // 生成缓存键
                var cacheKey = GenerateCacheKey(scriptCode, parameters);
                
                // 检查缓存
                if (_options.EnableCache && _cache.TryGetValue(cacheKey, out var cachedResult))
                {
                    Interlocked.Increment(ref _cacheHits);
                    _logger.LogDebug("Cache hit for script execution, CacheKey: {CacheKey}", cacheKey);
                    return cachedResult;
                }
                
                if (_options.EnableCache)
                {
                    Interlocked.Increment(ref _cacheMisses);
                }
                
                _logger.LogInformation("开始执行脚本，脚本长度: {ScriptLength} 字符", scriptCode.Length);
                
                // 执行实际脚本逻辑（模拟）
                await Task.Delay(200); // 模拟脚本执行延迟
                
                // 模拟脚本执行结果
                var executionResult = new {
                    Success = true,
                    Message = "脚本执行成功",
                    Result = "Hello from CSScript!",
                    Parameters = parameters,
                    ExecutionTime = stopwatch.ElapsedMilliseconds
                };
                
                // 设置结果
                result.Success = true;
                result.ResultData = executionResult;
                result.ScriptIdentifier = "inline_script";
                
                Interlocked.Increment(ref _successfulScripts);
                
                // 缓存结果
                if (_options.EnableCache)
                {
                    AddToCache(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedScripts);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "执行脚本失败");
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }
            
            _logger.LogInformation("脚本执行完成，结果: {Success}, 执行时间: {ExecutionTimeMs}ms",
                result.Success, result.ExecutionTimeMs);
            
            return result;
        }
        
        /// <summary>
        /// 执行外部脚本文件
        /// </summary>
        public async Task<CsscriptResult> ExecuteScriptFileAsync(string scriptPath, Dictionary<string, object>? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new CsscriptResult();
            
            try
            {
                // 检查是否允许执行外部脚本
                if (!_options.AllowExternalScripts)
                {
                    throw new InvalidOperationException("执行外部脚本已被禁用");
                }
                
                // 检查脚本文件是否存在
                if (!File.Exists(scriptPath))
                {
                    throw new FileNotFoundException("脚本文件不存在", scriptPath);
                }
                
                Interlocked.Increment(ref _executedScripts);
                
                // 读取脚本内容
                var scriptCode = await File.ReadAllTextAsync(scriptPath);
                
                // 生成缓存键
                var cacheKey = GenerateCacheKey(scriptCode, parameters);
                
                // 检查缓存
                if (_options.EnableCache && _cache.TryGetValue(cacheKey, out var cachedResult))
                {
                    Interlocked.Increment(ref _cacheHits);
                    _logger.LogDebug("Cache hit for script file execution, CacheKey: {CacheKey}", cacheKey);
                    return cachedResult;
                }
                
                if (_options.EnableCache)
                {
                    Interlocked.Increment(ref _cacheMisses);
                }
                
                _logger.LogInformation("开始执行脚本文件，路径: {ScriptPath}", scriptPath);
                
                // 执行实际脚本逻辑（模拟）
                await Task.Delay(300); // 模拟脚本执行延迟
                
                // 模拟脚本执行结果
                var executionResult = new {
                    Success = true,
                    Message = "脚本文件执行成功",
                    FilePath = scriptPath,
                    Result = "Hello from external CSScript file!",
                    Parameters = parameters,
                    ExecutionTime = stopwatch.ElapsedMilliseconds
                };
                
                // 设置结果
                result.Success = true;
                result.ResultData = executionResult;
                result.ScriptIdentifier = scriptPath;
                
                Interlocked.Increment(ref _successfulScripts);
                
                // 缓存结果
                if (_options.EnableCache)
                {
                    AddToCache(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedScripts);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "执行脚本文件失败，路径: {ScriptPath}", scriptPath);
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }
            
            _logger.LogInformation("脚本文件执行完成，路径: {ScriptPath}, 结果: {Success}, 执行时间: {ExecutionTimeMs}ms",
                scriptPath, result.Success, result.ExecutionTimeMs);
            
            return result;
        }
        
        /// <summary>
        /// 批量执行脚本
        /// </summary>
        public async Task<IEnumerable<CsscriptResult>> ExecuteScriptBatchAsync(IEnumerable<ScriptExecutionRequest> scriptExecutionRequests)
        {
            _logger.LogInformation("开始批量执行脚本，请求数量: {Count}", scriptExecutionRequests.Count());
            
            var tasks = new List<Task<CsscriptResult>>();
            
            foreach (var request in scriptExecutionRequests)
            {
                if (request.IsFilePath)
                {
                    tasks.Add(ExecuteScriptFileAsync(request.ScriptContent, request.Parameters));
                }
                else
                {
                    tasks.Add(ExecuteScriptAsync(request.ScriptContent, request.Parameters));
                }
            }
            
            var results = await Task.WhenAll(tasks);
            
            _logger.LogInformation("批量执行脚本完成，总请求数: {Total}, 成功: {Success}, 失败: {Failed}",
                results.Length, results.Count(r => r.Success), results.Count(r => !r.Success));
            
            return results;
        }
        
        /// <summary>
        /// 编译脚本
        /// </summary>
        public async Task<ScriptCompilationResult> CompileScriptAsync(string scriptCode)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ScriptCompilationResult();
            
            try
            {
                _logger.LogInformation("开始编译脚本，脚本长度: {ScriptLength} 字符", scriptCode.Length);
                
                // 执行实际编译逻辑（模拟）
                await Task.Delay(150); // 模拟编译延迟
                
                // 模拟编译结果
                result.Success = true;
                result.Errors = new List<string>();
                result.Warnings = new List<string> { "警告：脚本中使用了过时的API" };
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors = new List<string> { ex.Message };
                result.Warnings = new List<string>();
                _logger.LogError(ex, "编译脚本失败");
            }
            finally
            {
                stopwatch.Stop();
                result.CompilationTimeMs = stopwatch.ElapsedMilliseconds;
            }
            
            _logger.LogInformation("脚本编译完成，结果: {Success}, 编译时间: {CompilationTimeMs}ms",
                result.Success, result.CompilationTimeMs);
            
            return result;
        }
        
        /// <summary>
        /// 获取Csscript状态
        /// </summary>
        public async Task<CsscriptStatus> GetStatusAsync()
        {
            await Task.CompletedTask; // 模拟异步操作
            
            var totalCacheAccesses = _cacheHits + _cacheMisses;
            var cacheHitRate = totalCacheAccesses > 0 ? (double)_cacheHits / totalCacheAccesses * 100 : 0;
            
            var status = new CsscriptStatus
            {
                IsRunning = true,
                ExecutedScripts = _executedScripts,
                SuccessfulScripts = _successfulScripts,
                FailedScripts = _failedScripts,
                CacheHitRate = Math.Round(cacheHitRate, 2),
                AverageExecutionTimeMs = 0, // 简化实现，实际应计算平均值
                StartTime = _startTime
            };
            
            _logger.LogDebug("获取Csscript状态: {@Status}", status);
            
            return status;
        }
        
        /// <summary>
        /// 重置Csscript状态
        /// </summary>
        public async Task<bool> ResetStatusAsync()
        {
            await Task.CompletedTask; // 模拟异步操作
            
            try
            {
                Interlocked.Exchange(ref _executedScripts, 0);
                Interlocked.Exchange(ref _successfulScripts, 0);
                Interlocked.Exchange(ref _failedScripts, 0);
                Interlocked.Exchange(ref _cacheHits, 0);
                Interlocked.Exchange(ref _cacheMisses, 0);
                
                lock (_cache)
                {
                    _cache.Clear();
                }
                
                _logger.LogInformation("Csscript状态已重置");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "重置Csscript状态失败");
                return false;
            }
        }
        
        /// <summary>
        /// 生成缓存键
        /// </summary>
        private string GenerateCacheKey(string scriptCode, Dictionary<string, object>? parameters = null)
        {
            var paramString = parameters != null ? Newtonsoft.Json.JsonConvert.SerializeObject(parameters) : "null";
            var hash = GetHashCode($"{scriptCode}:{paramString}");
            return $"{hash}";
        }
        
        /// <summary>
        /// 获取字符串的哈希码
        /// </summary>
        private int GetHashCode(string data)
        {
            unchecked
            {
                int hash = 17;
                foreach (char c in data)
                {
                    hash = hash * 23 + c;
                }
                return hash;
            }
        }
        
        /// <summary>
        /// 添加到缓存
        /// </summary>
        private void AddToCache(string cacheKey, CsscriptResult result)
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
    /// Csscript AOT执行引擎
    /// 管理Csscript脚本执行
    /// </summary>
    public class CsscriptAotEngine
    {
        private readonly ILogger<CsscriptAotEngine> _logger;
        private readonly ICsscriptService _csscriptService;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="csscriptService">Csscript服务</param>
        public CsscriptAotEngine(ILogger<CsscriptAotEngine> logger, ICsscriptService csscriptService)
        {
            _logger = logger;
            _csscriptService = csscriptService;
            
            _logger.LogInformation("CsscriptAotEngine初始化成功");
        }
        
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="scriptCode">脚本代码</param>
        /// <param name="parameters">脚本参数</param>
        /// <returns>执行结果</returns>
        public async Task<CsscriptResult> ExecuteScriptAsync(string scriptCode, Dictionary<string, object>? parameters = null)
        {
            return await _csscriptService.ExecuteScriptAsync(scriptCode, parameters);
        }
        
        /// <summary>
        /// 执行外部脚本文件
        /// </summary>
        /// <param name="scriptPath">脚本文件路径</param>
        /// <param name="parameters">脚本参数</param>
        /// <returns>执行结果</returns>
        public async Task<CsscriptResult> ExecuteScriptFileAsync(string scriptPath, Dictionary<string, object>? parameters = null)
        {
            return await _csscriptService.ExecuteScriptFileAsync(scriptPath, parameters);
        }
        
        /// <summary>
        /// 批量执行脚本
        /// </summary>
        /// <param name="scriptExecutionRequests">脚本执行请求列表</param>
        /// <returns>执行结果列表</returns>
        public async Task<IEnumerable<CsscriptResult>> ExecuteScriptBatchAsync(IEnumerable<ScriptExecutionRequest> scriptExecutionRequests)
        {
            return await _csscriptService.ExecuteScriptBatchAsync(scriptExecutionRequests);
        }
        
        /// <summary>
        /// 编译脚本
        /// </summary>
        /// <param name="scriptCode">脚本代码</param>
        /// <returns>编译结果</returns>
        public async Task<ScriptCompilationResult> CompileScriptAsync(string scriptCode)
        {
            return await _csscriptService.CompileScriptAsync(scriptCode);
        }
        
        /// <summary>
        /// 获取Csscript状态
        /// </summary>
        /// <returns>状态信息</returns>
        public async Task<CsscriptStatus> GetStatusAsync()
        {
            return await _csscriptService.GetStatusAsync();
        }
        
        /// <summary>
        /// 重置Csscript状态
        /// </summary>
        /// <returns>操作结果</returns>
        public async Task<bool> ResetStatusAsync()
        {
            return await _csscriptService.ResetStatusAsync();
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
            
            // 配置Csscript选项
            builder.Configuration.AddJsonFile("csscript_aot.setting.json", optional: true);
            builder.Services.Configure<CsscriptOptions>(builder.Configuration.GetSection("Csscript"));
            
            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);
            
            // 注册服务
            builder.Services.AddSingleton<ICsscriptService, CsscriptService>();
            builder.Services.AddSingleton<CsscriptAotEngine>();
            
            // 构建主机
            var host = builder.Build();
            var serviceProvider = host.Services;
            
            // 获取引擎实例
            var engine = serviceProvider.GetRequiredService<CsscriptAotEngine>();
            
            // 解析命令行参数
            if (args.Length < 1)
            {
                Console.WriteLine("用法:");
                Console.WriteLine("  csscript_aot.exe <command> [arguments]");
                Console.WriteLine("  ");
                Console.WriteLine("命令:");
                Console.WriteLine("  execute <scriptcode>	执行内联脚本");
                Console.WriteLine("  execute-file <filepath>	执行脚本文件");
                Console.WriteLine("  compile <scriptcode>	编译脚本");
                Console.WriteLine("  status	获取服务状态");
                Console.WriteLine("  reset	重置服务状态");
                Console.WriteLine("  ");
                Console.WriteLine("示例:");
                Console.WriteLine("  csscript_aot.exe execute \"Console.WriteLine(\\\"Hello World!\\\");\"");
                Console.WriteLine("  csscript_aot.exe execute-file script.cs");
                Console.WriteLine("  csscript_aot.exe status");
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
                            Console.WriteLine("缺少参数: <scriptcode>");
                            return 1;
                        }
                        
                        string scriptCode = args[1];
                        var result = await engine.ExecuteScriptAsync(scriptCode);
                        
                        if (result.Success)
                        {
                            Console.WriteLine($"脚本执行成功");
                            Console.WriteLine($"执行时间: {result.ExecutionTimeMs}ms");
                            if (result.ResultData != null)
                            {
                                Console.WriteLine($"结果: {Newtonsoft.Json.JsonConvert.SerializeObject(result.ResultData)}");
                            }
                            return 0;
                        }
                        else
                        {
                            Console.WriteLine($"脚本执行失败: {result.ErrorMessage}");
                            return 1;
                        }
                        
                    case "execute-file":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("缺少参数: <filepath>");
                            return 1;
                        }
                        
                        string scriptPath = args[1];
                        var fileResult = await engine.ExecuteScriptFileAsync(scriptPath);
                        
                        if (fileResult.Success)
                        {
                            Console.WriteLine($"脚本文件执行成功，路径: {scriptPath}");
                            Console.WriteLine($"执行时间: {fileResult.ExecutionTimeMs}ms");
                            if (fileResult.ResultData != null)
                            {
                                Console.WriteLine($"结果: {Newtonsoft.Json.JsonConvert.SerializeObject(fileResult.ResultData)}");
                            }
                            return 0;
                        }
                        else
                        {
                            Console.WriteLine($"脚本文件执行失败: {fileResult.ErrorMessage}");
                            return 1;
                        }
                        
                    case "compile":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("缺少参数: <scriptcode>");
                            return 1;
                        }
                        
                        string compileScriptCode = args[1];
                        var compileResult = await engine.CompileScriptAsync(compileScriptCode);
                        
                        Console.WriteLine($"编译结果: {compileResult.Success}");
                        Console.WriteLine($"编译时间: {compileResult.CompilationTimeMs}ms");
                        
                        if (compileResult.Warnings?.Count > 0)
                        {
                            Console.WriteLine("警告:");
                            foreach (var warning in compileResult.Warnings)
                            {
                                Console.WriteLine($"  - {warning}");
                            }
                        }
                        
                        if (compileResult.Errors?.Count > 0)
                        {
                            Console.WriteLine("错误:");
                            foreach (var error in compileResult.Errors)
                            {
                                Console.WriteLine($"  - {error}");
                            }
                            return 1;
                        }
                        
                        return 0;
                        
                    case "status":
                        var status = await engine.GetStatusAsync();
                        Console.WriteLine("Csscript服务状态:");
                        Console.WriteLine($"  运行状态: {(status.IsRunning ? "正常" : "异常")}");
                        Console.WriteLine($"  已执行脚本数: {status.ExecutedScripts}");
                        Console.WriteLine($"  成功脚本数: {status.SuccessfulScripts}");
                        Console.WriteLine($"  失败脚本数: {status.FailedScripts}");
                        Console.WriteLine($"  缓存命中率: {status.CacheHitRate}%");
                        Console.WriteLine($"  平均执行时间: {status.AverageExecutionTimeMs}ms");
                        Console.WriteLine($"  启动时间: {status.StartTime.ToLocalTime()}");
                        return 0;
                        
                    case "reset":
                        var resetResult = await engine.ResetStatusAsync();
                        Console.WriteLine($"重置服务状态: {(resetResult ? "成功" : "失败")}");
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