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

namespace CsharpFlink.AOT
{
    /// <summary>
    /// CsharpFlink配置选项
    /// </summary>
    public class CsharpFlinkOptions
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
        /// 并行度
        /// </summary>
        public int Parallelism { get; set; } = Environment.ProcessorCount;
        
        /// <summary>
        /// 检查点间隔
        /// </summary>
        public TimeSpan CheckpointInterval { get; set; } = TimeSpan.FromSeconds(10);
        
        /// <summary>
        /// 状态保留时间
        /// </summary>
        public TimeSpan StateRetentionTime { get; set; } = TimeSpan.FromHours(24);
    }
    
    /// <summary>
    /// CsharpFlink服务接口
    /// 定义了CsharpFlink的核心功能
    /// </summary>
    public interface ICsharpFlinkService
    {
        /// <summary>
        /// 执行Flink作业
        /// </summary>
        /// <param name="jobConfig">作业配置</param>
        /// <returns>执行结果</returns>
        Task<CsharpFlinkResult> ExecuteJobAsync(FlinkJobConfig jobConfig);
        
        /// <summary>
        /// 批量执行Flink作业
        /// </summary>
        /// <param name="jobConfigs">作业配置列表</param>
        /// <returns>执行结果列表</returns>
        Task<IEnumerable<CsharpFlinkResult>> ExecuteJobBatchAsync(IEnumerable<FlinkJobConfig> jobConfigs);
        
        /// <summary>
        /// 获取作业状态
        /// </summary>
        /// <param name="jobId">作业ID</param>
        /// <returns>作业状态</returns>
        Task<JobStatus> GetJobStatusAsync(string jobId);
        
        /// <summary>
        /// 停止作业
        /// </summary>
        /// <param name="jobId">作业ID</param>
        /// <returns>操作结果</returns>
        Task<bool> StopJobAsync(string jobId);
        
        /// <summary>
        /// 获取CsharpFlink状态
        /// </summary>
        /// <returns>状态信息</returns>
        Task<CsharpFlinkStatus> GetStatusAsync();
        
        /// <summary>
        /// 重置CsharpFlink状态
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> ResetStatusAsync();
    }
    
    /// <summary>
    /// Flink作业配置
    /// </summary>
    public class FlinkJobConfig
    {
        /// <summary>
        /// 作业ID
        /// </summary>
        public string JobId { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// 作业名称
        /// </summary>
        public string JobName { get; set; } = string.Empty;
        
        /// <summary>
        /// 作业类型
        /// </summary>
        public string JobType { get; set; } = "streaming";
        
        /// <summary>
        /// 并行度
        /// </summary>
        public int Parallelism { get; set; } = Environment.ProcessorCount;
        
        /// <summary>
        /// 作业配置参数
        /// </summary>
        public Dictionary<string, string> ConfigParams { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// 输入数据源
        /// </summary>
        public string InputSource { get; set; } = string.Empty;
        
        /// <summary>
        /// 输出目的地
        /// </summary>
        public string OutputSink { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// CsharpFlink操作结果
    /// </summary>
    public class CsharpFlinkResult
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 作业ID
        /// </summary>
        public string JobId { get; set; } = string.Empty;
        
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
        /// 作业状态
        /// </summary>
        public JobStatus JobStatus { get; set; } = JobStatus.Unknown;
    }
    
    /// <summary>
    /// 作业状态
    /// </summary>
    public enum JobStatus
    {
        /// <summary>
        /// 未知状态
        /// </summary>
        Unknown,
        /// <summary>
        /// 已提交
        /// </summary>
        Submitted,
        /// <summary>
        /// 运行中
        /// </summary>
        Running,
        /// <summary>
        /// 已完成
        /// </summary>
        Completed,
        /// <summary>
        /// 已失败
        /// </summary>
        Failed,
        /// <summary>
        /// 已取消
        /// </summary>
        Cancelled,
        /// <summary>
        /// 已暂停
        /// </summary>
        Paused
    }
    
    /// <summary>
    /// CsharpFlink状态信息
    /// </summary>
    public class CsharpFlinkStatus
    {
        /// <summary>
        /// 服务是否正常运行
        /// </summary>
        public bool IsRunning { get; set; }
        
        /// <summary>
        /// 正在运行的作业数
        /// </summary>
        public int RunningJobs { get; set; }
        
        /// <summary>
        /// 已完成的作业数
        /// </summary>
        public long CompletedJobs { get; set; }
        
        /// <summary>
        /// 已失败的作业数
        /// </summary>
        public long FailedJobs { get; set; }
        
        /// <summary>
        /// 缓存命中率（百分比）
        /// </summary>
        public double CacheHitRate { get; set; }
        
        /// <summary>
        /// 平均作业执行时间（毫秒）
        /// </summary>
        public double AverageJobTimeMs { get; set; }
        
        /// <summary>
        /// 服务启动时间
        /// </summary>
        public DateTime StartTime { get; set; }
    }
    
    /// <summary>
    /// CsharpFlink服务实现
    /// 基于.NET 10 AOT架构，提供高性能Flink作业执行功能
    /// </summary>
    public class CsharpFlinkService : ICsharpFlinkService
    {
        private readonly ILogger<CsharpFlinkService> _logger;
        private readonly CsharpFlinkOptions _options;
        private readonly Dictionary<string, CsharpFlinkResult> _cache = new Dictionary<string, CsharpFlinkResult>();
        private long _completedJobs = 0;
        private long _failedJobs = 0;
        private long _cacheHits = 0;
        private long _cacheMisses = 0;
        private readonly DateTime _startTime = DateTime.UtcNow;
        private readonly Dictionary<string, JobStatus> _runningJobs = new Dictionary<string, JobStatus>();
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="options">配置选项</param>
        public CsharpFlinkService(ILogger<CsharpFlinkService> logger, IOptions<CsharpFlinkOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            
            _logger.LogInformation("CsharpFlinkService初始化成功，配置选项：Parallelism={Parallelism}, CheckpointInterval={CheckpointInterval}, StateRetentionTime={StateRetentionTime}",
                _options.Parallelism, _options.CheckpointInterval, _options.StateRetentionTime);
        }
        
        /// <summary>
        /// 执行Flink作业
        /// </summary>
        public async Task<CsharpFlinkResult> ExecuteJobAsync(FlinkJobConfig jobConfig)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new CsharpFlinkResult { JobId = jobConfig.JobId };
            
            try
            {
                // 生成缓存键
                var cacheKey = GenerateCacheKey(jobConfig);
                
                // 检查缓存
                if (_options.EnableCache && _cache.TryGetValue(cacheKey, out var cachedResult))
                {
                    Interlocked.Increment(ref _cacheHits);
                    _logger.LogDebug("Cache hit for Flink job, CacheKey: {CacheKey}", cacheKey);
                    return cachedResult;
                }
                
                if (_options.EnableCache)
                {
                    Interlocked.Increment(ref _cacheMisses);
                }
                
                _logger.LogInformation("开始执行Flink作业，JobId: {JobId}, JobName: {JobName}, JobType: {JobType}",
                    jobConfig.JobId, jobConfig.JobName, jobConfig.JobType);
                
                // 标记作业为运行中
                lock (_runningJobs)
                {
                    _runningJobs[jobConfig.JobId] = JobStatus.Running;
                }
                
                // 执行实际作业逻辑（模拟）
                await Task.Delay(500); // 模拟作业执行延迟
                
                // 根据作业类型执行不同的逻辑
                object? resultData = null;
                switch (jobConfig.JobType.ToLower())
                {
                    case "streaming":
                        resultData = await ExecuteStreamingJobAsync(jobConfig);
                        break;
                    case "batch":
                        resultData = await ExecuteBatchJobAsync(jobConfig);
                        break;
                    case "sql":
                        resultData = await ExecuteSqlJobAsync(jobConfig);
                        break;
                    default:
                        resultData = await ExecuteDefaultJobAsync(jobConfig);
                        break;
                }
                
                // 设置结果
                result.Success = true;
                result.ResultData = resultData;
                result.JobStatus = JobStatus.Completed;
                
                Interlocked.Increment(ref _completedJobs);
                
                // 移除作业
                lock (_runningJobs)
                {
                    _runningJobs.Remove(jobConfig.JobId);
                }
                
                // 缓存结果
                if (_options.EnableCache)
                {
                    AddToCache(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedJobs);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.JobStatus = JobStatus.Failed;
                
                // 移除作业
                lock (_runningJobs)
                {
                    _runningJobs.Remove(jobConfig.JobId);
                }
                
                _logger.LogError(ex, "执行Flink作业失败，JobId: {JobId}", jobConfig.JobId);
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }
            
            _logger.LogInformation("Flink作业执行完成，JobId: {JobId}, 结果: {Success}, 执行时间: {ExecutionTimeMs}ms",
                result.JobId, result.Success, result.ExecutionTimeMs);
            
            return result;
        }
        
        /// <summary>
        /// 批量执行Flink作业
        /// </summary>
        public async Task<IEnumerable<CsharpFlinkResult>> ExecuteJobBatchAsync(IEnumerable<FlinkJobConfig> jobConfigs)
        {
            _logger.LogInformation("开始批量执行Flink作业，作业数量: {Count}", jobConfigs.Count());
            
            var tasks = jobConfigs.Select(jobConfig => ExecuteJobAsync(jobConfig));
            var results = await Task.WhenAll(tasks);
            
            _logger.LogInformation("批量执行Flink作业完成，总作业数: {Total}, 成功: {Success}, 失败: {Failed}",
                results.Length, results.Count(r => r.Success), results.Count(r => !r.Success));
            
            return results;
        }
        
        /// <summary>
        /// 获取作业状态
        /// </summary>
        public async Task<JobStatus> GetJobStatusAsync(string jobId)
        {
            await Task.CompletedTask; // 模拟异步操作
            
            lock (_runningJobs)
            {
                if (_runningJobs.TryGetValue(jobId, out var status))
                {
                    return status;
                }
            }
            
            return JobStatus.Unknown;
        }
        
        /// <summary>
        /// 停止作业
        /// </summary>
        public async Task<bool> StopJobAsync(string jobId)
        {
            await Task.Delay(200); // 模拟异步操作
            
            try
            {
                lock (_runningJobs)
                {
                    if (_runningJobs.ContainsKey(jobId))
                    {
                        _runningJobs[jobId] = JobStatus.Cancelled;
                        _runningJobs.Remove(jobId);
                    }
                }
                
                _logger.LogInformation("停止Flink作业成功，JobId: {JobId}", jobId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "停止Flink作业失败，JobId: {JobId}", jobId);
                return false;
            }
        }
        
        /// <summary>
        /// 获取CsharpFlink状态
        /// </summary>
        public async Task<CsharpFlinkStatus> GetStatusAsync()
        {
            await Task.CompletedTask; // 模拟异步操作
            
            int runningJobsCount;
            lock (_runningJobs)
            {
                runningJobsCount = _runningJobs.Count;
            }
            
            var totalCacheAccesses = _cacheHits + _cacheMisses;
            var cacheHitRate = totalCacheAccesses > 0 ? (double)_cacheHits / totalCacheAccesses * 100 : 0;
            
            var status = new CsharpFlinkStatus
            {
                IsRunning = true,
                RunningJobs = runningJobsCount,
                CompletedJobs = _completedJobs,
                FailedJobs = _failedJobs,
                CacheHitRate = Math.Round(cacheHitRate, 2),
                AverageJobTimeMs = 0, // 简化实现，实际应计算平均值
                StartTime = _startTime
            };
            
            _logger.LogDebug("获取CsharpFlink状态: {@Status}", status);
            
            return status;
        }
        
        /// <summary>
        /// 重置CsharpFlink状态
        /// </summary>
        public async Task<bool> ResetStatusAsync()
        {
            await Task.CompletedTask; // 模拟异步操作
            
            try
            {
                Interlocked.Exchange(ref _completedJobs, 0);
                Interlocked.Exchange(ref _failedJobs, 0);
                Interlocked.Exchange(ref _cacheHits, 0);
                Interlocked.Exchange(ref _cacheMisses, 0);
                
                lock (_runningJobs)
                {
                    _runningJobs.Clear();
                }
                
                lock (_cache)
                {
                    _cache.Clear();
                }
                
                _logger.LogInformation("CsharpFlink状态已重置");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "重置CsharpFlink状态失败");
                return false;
            }
        }
        
        /// <summary>
        /// 执行流处理作业
        /// </summary>
        private async Task<object> ExecuteStreamingJobAsync(FlinkJobConfig jobConfig)
        {
            await Task.Delay(300); // 模拟处理延迟
            return new { JobType = "Streaming", Message = "流处理作业执行成功", InputSource = jobConfig.InputSource, OutputSink = jobConfig.OutputSink };
        }
        
        /// <summary>
        /// 执行批处理作业
        /// </summary>
        private async Task<object> ExecuteBatchJobAsync(FlinkJobConfig jobConfig)
        {
            await Task.Delay(600); // 模拟处理延迟
            return new { JobType = "Batch", Message = "批处理作业执行成功", InputSource = jobConfig.InputSource, OutputSink = jobConfig.OutputSink };
        }
        
        /// <summary>
        /// 执行SQL作业
        /// </summary>
        private async Task<object> ExecuteSqlJobAsync(FlinkJobConfig jobConfig)
        {
            await Task.Delay(400); // 模拟处理延迟
            return new { JobType = "SQL", Message = "SQL作业执行成功", InputSource = jobConfig.InputSource, OutputSink = jobConfig.OutputSink };
        }
        
        /// <summary>
        /// 执行默认作业
        /// </summary>
        private async Task<object> ExecuteDefaultJobAsync(FlinkJobConfig jobConfig)
        {
            await Task.Delay(200); // 模拟处理延迟
            return new { JobType = "Default", Message = "默认作业执行成功", InputSource = jobConfig.InputSource, OutputSink = jobConfig.OutputSink };
        }
        
        /// <summary>
        /// 生成缓存键
        /// </summary>
        private string GenerateCacheKey(FlinkJobConfig jobConfig)
        {
            var hash = GetHashCode($"{jobConfig.JobType}:{jobConfig.InputSource}:{jobConfig.OutputSink}:{string.Join(":", jobConfig.ConfigParams)}");
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
        private void AddToCache(string cacheKey, CsharpFlinkResult result)
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
    /// CsharpFlink AOT执行引擎
    /// 管理CsharpFlink作业的执行
    /// </summary>
    public class CsharpFlinkAotEngine
    {
        private readonly ILogger<CsharpFlinkAotEngine> _logger;
        private readonly ICsharpFlinkService _csharpFlinkService;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="csharpFlinkService">CsharpFlink服务</param>
        public CsharpFlinkAotEngine(ILogger<CsharpFlinkAotEngine> logger, ICsharpFlinkService csharpFlinkService)
        {
            _logger = logger;
            _csharpFlinkService = csharpFlinkService;
            
            _logger.LogInformation("CsharpFlinkAotEngine初始化成功");
        }
        
        /// <summary>
        /// 执行Flink作业
        /// </summary>
        /// <param name="jobConfig">作业配置</param>
        /// <returns>执行结果</returns>
        public async Task<CsharpFlinkResult> ExecuteJobAsync(FlinkJobConfig jobConfig)
        {
            return await _csharpFlinkService.ExecuteJobAsync(jobConfig);
        }
        
        /// <summary>
        /// 批量执行Flink作业
        /// </summary>
        /// <param name="jobConfigs">作业配置列表</param>
        /// <returns>执行结果列表</returns>
        public async Task<IEnumerable<CsharpFlinkResult>> ExecuteJobBatchAsync(IEnumerable<FlinkJobConfig> jobConfigs)
        {
            return await _csharpFlinkService.ExecuteJobBatchAsync(jobConfigs);
        }
        
        /// <summary>
        /// 获取作业状态
        /// </summary>
        /// <param name="jobId">作业ID</param>
        /// <returns>作业状态</returns>
        public async Task<JobStatus> GetJobStatusAsync(string jobId)
        {
            return await _csharpFlinkService.GetJobStatusAsync(jobId);
        }
        
        /// <summary>
        /// 停止作业
        /// </summary>
        /// <param name="jobId">作业ID</param>
        /// <returns>操作结果</returns>
        public async Task<bool> StopJobAsync(string jobId)
        {
            return await _csharpFlinkService.StopJobAsync(jobId);
        }
        
        /// <summary>
        /// 获取CsharpFlink状态
        /// </summary>
        /// <returns>状态信息</returns>
        public async Task<CsharpFlinkStatus> GetStatusAsync()
        {
            return await _csharpFlinkService.GetStatusAsync();
        }
        
        /// <summary>
        /// 重置CsharpFlink状态
        /// </summary>
        /// <returns>操作结果</returns>
        public async Task<bool> ResetStatusAsync()
        {
            return await _csharpFlinkService.ResetStatusAsync();
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
            
            // 配置CsharpFlink选项
            builder.Configuration.AddJsonFile("csharpflink_aot.setting.json", optional: true);
            builder.Services.Configure<CsharpFlinkOptions>(builder.Configuration.GetSection("CsharpFlink"));
            
            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);
            
            // 注册服务
            builder.Services.AddSingleton<ICsharpFlinkService, CsharpFlinkService>();
            builder.Services.AddSingleton<CsharpFlinkAotEngine>();
            
            // 构建主机
            var host = builder.Build();
            var serviceProvider = host.Services;
            
            // 获取引擎实例
            var engine = serviceProvider.GetRequiredService<CsharpFlinkAotEngine>();
            
            // 解析命令行参数
            if (args.Length < 1)
            {
                Console.WriteLine("用法:");
                Console.WriteLine("  csharpflink_aot.exe <command> [arguments]");
                Console.WriteLine("  ");
                Console.WriteLine("命令:");
                Console.WriteLine("  run <jobtype> <input> <output> [name]	执行Flink作业");
                Console.WriteLine("  status <jobid>	获取作业状态");
                Console.WriteLine("  stop <jobid>	停止作业");
                Console.WriteLine("  service-status	获取服务状态");
                Console.WriteLine("  reset	重置服务状态");
                Console.WriteLine("  ");
                Console.WriteLine("示例:");
                Console.WriteLine("  csharpflink_aot.exe run streaming input.txt output.txt MyJob");
                Console.WriteLine("  csharpflink_aot.exe run batch input.csv output.csv BatchJob");
                Console.WriteLine("  csharpflink_aot.exe status job123");
                return 1;
            }
            
            try
            {
                string command = args[0].ToLower();
                
                switch (command)
                {
                    case "run":
                        if (args.Length < 4)
                        {
                            Console.WriteLine("缺少参数: <jobtype> <input> <output> [name]");
                            return 1;
                        }
                        
                        // 解析参数
                        string jobType = args[1];
                        string input = args[2];
                        string output = args[3];
                        string name = args.Length > 4 ? args[4] : "FlinkJob";
                        
                        // 创建作业配置
                        var jobConfig = new FlinkJobConfig
                        {
                            JobName = name,
                            JobType = jobType,
                            InputSource = input,
                            OutputSink = output
                        };
                        
                        // 执行作业
                        var result = await engine.ExecuteJobAsync(jobConfig);
                        
                        if (result.Success)
                        {
                            Console.WriteLine($"作业执行成功，JobId: {result.JobId}");
                            Console.WriteLine($"执行时间: {result.ExecutionTimeMs}ms");
                            Console.WriteLine($"状态: {result.JobStatus}");
                            if (result.ResultData != null)
                            {
                                Console.WriteLine($"结果: {Newtonsoft.Json.JsonConvert.SerializeObject(result.ResultData)}");
                            }
                            return 0;
                        }
                        else
                        {
                            Console.WriteLine($"作业执行失败: {result.ErrorMessage}");
                            return 1;
                        }
                        
                    case "status":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("缺少参数: <jobid>");
                            return 1;
                        }
                        
                        string jobId = args[1];
                        var jobStatus = await engine.GetJobStatusAsync(jobId);
                        Console.WriteLine($"作业 {jobId} 状态: {jobStatus}");
                        return 0;
                        
                    case "stop":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("缺少参数: <jobid>");
                            return 1;
                        }
                        
                        jobId = args[1];
                        var stopResult = await engine.StopJobAsync(jobId);
                        Console.WriteLine($"停止作业 {jobId}: {(stopResult ? "成功" : "失败")}");
                        return stopResult ? 0 : 1;
                        
                    case "service-status":
                        var serviceStatus = await engine.GetStatusAsync();
                        Console.WriteLine("服务状态:");
                        Console.WriteLine($"  运行状态: {(serviceStatus.IsRunning ? "正常" : "异常")}");
                        Console.WriteLine($"  正在运行的作业数: {serviceStatus.RunningJobs}");
                        Console.WriteLine($"  已完成的作业数: {serviceStatus.CompletedJobs}");
                        Console.WriteLine($"  已失败的作业数: {serviceStatus.FailedJobs}");
                        Console.WriteLine($"  缓存命中率: {serviceStatus.CacheHitRate}%");
                        Console.WriteLine($"  平均作业执行时间: {serviceStatus.AverageJobTimeMs}ms");
                        Console.WriteLine($"  启动时间: {serviceStatus.StartTime.ToLocalTime()}");
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