#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Newtonsoft.Json@13.0.3
#:package CsvHelper@32.0.1
#:package CsvHelper.Schema@32.0.1
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
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CsvHelper.AOT
{
    /// <summary>
    /// CsvHelper配置选项
    /// </summary>
    public class CsvHelperOptions
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
        /// 默认字符编码
        /// </summary>
        public string DefaultEncoding { get; set; } = "utf-8";
        
        /// <summary>
        /// 默认分隔符
        /// </summary>
        public char DefaultDelimiter { get; set; } = ',';
        
        /// <summary>
        /// 是否包含标题行
        /// </summary>
        public bool HasHeaderRecord { get; set; } = true;
        
        /// <summary>
        /// 是否忽略空行
        /// </summary>
        public bool IgnoreBlankLines { get; set; } = true;
        
        /// <summary>
        /// 是否跳过首行
        /// </summary>
        public bool SkipFirstRecord { get; set; } = false;
    }
    
    /// <summary>
    /// 记录映射配置
    /// </summary>
    /// <typeparam name="T">记录类型</typeparam>
    public class CsvRecordMap<T> : ClassMap<T> where T : class
    {
        public CsvRecordMap()
        {
            AutoMap(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
    
    /// <summary>
    /// CsvHelper服务接口
    /// 定义了CsvHelper的核心功能
    /// </summary>
    public interface ICsvHelperService
    {
        /// <summary>
        /// 将对象列表导出为CSV字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="records">对象列表</param>
        /// <param name="configuration">CSV配置选项</param>
        /// <returns>CSV字符串</returns>
        Task<string> ExportToCsvStringAsync<T>(IEnumerable<T> records, CsvConfiguration? configuration = null) where T : class;
        
        /// <summary>
        /// 将对象列表导出为CSV文件
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="records">对象列表</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="configuration">CSV配置选项</param>
        /// <returns>导出结果</returns>
        Task<CsvHelperResult> ExportToCsvFileAsync<T>(IEnumerable<T> records, string filePath, CsvConfiguration? configuration = null) where T : class;
        
        /// <summary>
        /// 从CSV字符串导入对象列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="csvContent">CSV字符串</param>
        /// <param name="configuration">CSV配置选项</param>
        /// <returns>对象列表</returns>
        Task<List<T>> ImportFromCsvStringAsync<T>(string csvContent, CsvConfiguration? configuration = null) where T : class;
        
        /// <summary>
        /// 从CSV文件导入对象列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="filePath">文件路径</param>
        /// <param name="configuration">CSV配置选项</param>
        /// <returns>导入结果</returns>
        Task<CsvHelperResult<List<T>>> ImportFromCsvFileAsync<T>(string filePath, CsvConfiguration? configuration = null) where T : class;
        
        /// <summary>
        /// 批量导出多个对象列表为CSV文件
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="exportRequests">导出请求列表</param>
        /// <returns>批量导出结果</returns>
        Task<IEnumerable<CsvHelperResult>> ExportBatchAsync<T>(IEnumerable<CsvExportRequest<T>> exportRequests) where T : class;
        
        /// <summary>
        /// 获取CsvHelper状态
        /// </summary>
        /// <returns>状态信息</returns>
        Task<CsvHelperStatus> GetStatusAsync();
        
        /// <summary>
        /// 重置CsvHelper状态
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> ResetStatusAsync();
    }
    
    /// <summary>
    /// CSV导出请求
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public class CsvExportRequest<T> where T : class
    {
        /// <summary>
        /// 请求ID
        /// </summary>
        public string RequestId { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// 对象列表
        /// </summary>
        public IEnumerable<T> Records { get; set; } = Enumerable.Empty<T>();
        
        /// <summary>
        /// 文件路径（如果导出到文件）
        /// </summary>
        public string? FilePath { get; set; } = null;
        
        /// <summary>
        /// CSV配置选项
        /// </summary>
        public CsvConfiguration? Configuration { get; set; } = null;
    }
    
    /// <summary>
    /// CsvHelper操作结果
    /// </summary>
    public class CsvHelperResult
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
        /// 记录数量
        /// </summary>
        public int RecordCount { get; set; }
        
        /// <summary>
        /// 操作类型
        /// </summary>
        public string? OperationType { get; set; }
    }
    
    /// <summary>
    /// 带泛型结果的CsvHelper操作结果
    /// </summary>
    /// <typeparam name="T">结果类型</typeparam>
    public class CsvHelperResult<T> : CsvHelperResult
    {
        /// <summary>
        /// 泛型结果数据
        /// </summary>
        public new T? ResultData { get; set; }
    }
    
    /// <summary>
    /// CsvHelper状态信息
    /// </summary>
    public class CsvHelperStatus
    {
        /// <summary>
        /// 服务是否正常运行
        /// </summary>
        public bool IsRunning { get; set; }
        
        /// <summary>
        /// 已处理的CSV文件数
        /// </summary>
        public long ProcessedFiles { get; set; }
        
        /// <summary>
        /// 已处理的记录数
        /// </summary>
        public long ProcessedRecords { get; set; }
        
        /// <summary>
        /// 成功处理的文件数
        /// </summary>
        public long SuccessfulFiles { get; set; }
        
        /// <summary>
        /// 失败处理的文件数
        /// </summary>
        public long FailedFiles { get; set; }
        
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
    /// CsvHelper服务实现
    /// 基于.NET 10 AOT架构，提供高性能CSV处理功能
    /// </summary>
    public class CsvHelperService : ICsvHelperService
    {
        private readonly ILogger<CsvHelperService> _logger;
        private readonly CsvHelperOptions _options;
        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();
        private long _processedFiles = 0;
        private long _processedRecords = 0;
        private long _successfulFiles = 0;
        private long _failedFiles = 0;
        private long _cacheHits = 0;
        private long _cacheMisses = 0;
        private readonly DateTime _startTime = DateTime.UtcNow;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="options">配置选项</param>
        public CsvHelperService(ILogger<CsvHelperService> logger, IOptions<CsvHelperOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            
            _logger.LogInformation("CsvHelperService初始化成功，配置选项：DefaultDelimiter={DefaultDelimiter}, HasHeaderRecord={HasHeaderRecord}, IgnoreBlankLines={IgnoreBlankLines}",
                _options.DefaultDelimiter, _options.HasHeaderRecord, _options.IgnoreBlankLines);
        }
        
        /// <summary>
        /// 将对象列表导出为CSV字符串
        /// </summary>
        public async Task<string> ExportToCsvStringAsync<T>(IEnumerable<T> records, CsvConfiguration? configuration = null) where T : class
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            try
            {
                // 使用默认配置或传入的配置
                var csvConfig = configuration ?? CreateDefaultConfiguration();
                
                // 生成缓存键
                var cacheKey = GenerateCacheKey<T>(records, csvConfig);
                
                // 检查缓存
                if (_options.EnableCache && _cache.TryGetValue(cacheKey, out var cachedResult))
                {
                    Interlocked.Increment(ref _cacheHits);
                    _logger.LogDebug("Cache hit for CSV export, CacheKey: {CacheKey}", cacheKey);
                    return cachedResult;
                }
                
                if (_options.EnableCache)
                {
                    Interlocked.Increment(ref _cacheMisses);
                }
                
                _logger.LogInformation("开始导出CSV，记录数量: {RecordCount}", records.Count());
                
                using (var writer = new StringWriter())
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    // 注册映射
                    csv.Context.RegisterClassMap<CsvRecordMap<T>>();
                    
                    // 写入记录
                    await csv.WriteRecordsAsync(records);
                    await writer.FlushAsync();
                    
                    var result = writer.ToString();
                    
                    // 缓存结果
                    if (_options.EnableCache)
                    {
                        AddToCache(cacheKey, result);
                    }
                    
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "导出CSV字符串失败");
                throw;
            }
            finally
            {
                stopwatch.Stop();
            }
        }
        
        /// <summary>
        /// 将对象列表导出为CSV文件
        /// </summary>
        public async Task<CsvHelperResult> ExportToCsvFileAsync<T>(IEnumerable<T> records, string filePath, CsvConfiguration? configuration = null) where T : class
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new CsvHelperResult();
            
            try
            {
                Interlocked.Increment(ref _processedFiles);
                
                // 使用默认配置或传入的配置
                var csvConfig = configuration ?? CreateDefaultConfiguration();
                
                var recordCount = records.Count();
                
                _logger.LogInformation("开始导出CSV文件，路径: {FilePath}, 记录数量: {RecordCount}", filePath, recordCount);
                
                // 确保目录存在
                var directoryPath = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                
                using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    // 注册映射
                    csv.Context.RegisterClassMap<CsvRecordMap<T>>();
                    
                    // 写入记录
                    await csv.WriteRecordsAsync(records);
                    await writer.FlushAsync();
                }
                
                // 设置结果
                result.Success = true;
                result.RecordCount = recordCount;
                result.OperationType = "ExportToFile";
                result.ResultData = new { FilePath = filePath, RecordCount = recordCount };
                
                Interlocked.Increment(ref _successfulFiles);
                Interlocked.Add(ref _processedRecords, recordCount);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedFiles);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.OperationType = "ExportToFile";
                _logger.LogError(ex, "导出CSV文件失败，路径: {FilePath}", filePath);
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }
            
            _logger.LogInformation("导出CSV文件完成，路径: {FilePath}, 结果: {Success}, 执行时间: {ExecutionTimeMs}ms",
                filePath, result.Success, result.ExecutionTimeMs);
            
            return result;
        }
        
        /// <summary>
        /// 从CSV字符串导入对象列表
        /// </summary>
        public async Task<List<T>> ImportFromCsvStringAsync<T>(string csvContent, CsvConfiguration? configuration = null) where T : class
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            try
            {
                // 使用默认配置或传入的配置
                var csvConfig = configuration ?? CreateDefaultConfiguration();
                
                _logger.LogInformation("开始从CSV字符串导入，内容长度: {ContentLength} 字符", csvContent.Length);
                
                using (var reader = new StringReader(csvContent))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    // 注册映射
                    csv.Context.RegisterClassMap<CsvRecordMap<T>>();
                    
                    // 读取记录
                    var records = await csv.GetRecordsAsync<T>().ToListAsync();
                    
                    _logger.LogInformation("从CSV字符串导入完成，读取记录数: {RecordCount}", records.Count);
                    
                    return records;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "从CSV字符串导入失败");
                throw;
            }
            finally
            {
                stopwatch.Stop();
            }
        }
        
        /// <summary>
        /// 从CSV文件导入对象列表
        /// </summary>
        public async Task<CsvHelperResult<List<T>>> ImportFromCsvFileAsync<T>(string filePath, CsvConfiguration? configuration = null) where T : class
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new CsvHelperResult<List<T>>();
            
            try
            {
                Interlocked.Increment(ref _processedFiles);
                
                // 检查文件是否存在
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("CSV文件不存在", filePath);
                }
                
                // 使用默认配置或传入的配置
                var csvConfig = configuration ?? CreateDefaultConfiguration();
                
                _logger.LogInformation("开始从CSV文件导入，路径: {FilePath}", filePath);
                
                using (var reader = new StreamReader(filePath, System.Text.Encoding.UTF8))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    // 注册映射
                    csv.Context.RegisterClassMap<CsvRecordMap<T>>();
                    
                    // 读取记录
                    var records = await csv.GetRecordsAsync<T>().ToListAsync();
                    
                    // 设置结果
                    result.Success = true;
                    result.RecordCount = records.Count;
                    result.OperationType = "ImportFromFile";
                    result.ResultData = records;
                    
                    Interlocked.Increment(ref _successfulFiles);
                    Interlocked.Add(ref _processedRecords, records.Count);
                    
                    _logger.LogInformation("从CSV文件导入完成，路径: {FilePath}, 读取记录数: {RecordCount}", filePath, records.Count);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedFiles);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.OperationType = "ImportFromFile";
                _logger.LogError(ex, "从CSV文件导入失败，路径: {FilePath}", filePath);
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }
            
            return result;
        }
        
        /// <summary>
        /// 批量导出多个对象列表为CSV文件
        /// </summary>
        public async Task<IEnumerable<CsvHelperResult>> ExportBatchAsync<T>(IEnumerable<CsvExportRequest<T>> exportRequests) where T : class
        {
            _logger.LogInformation("开始批量导出CSV，请求数量: {Count}", exportRequests.Count());
            
            var tasks = new List<Task<CsvHelperResult>>();
            
            foreach (var request in exportRequests)
            {
                if (!string.IsNullOrEmpty(request.FilePath))
                {
                    tasks.Add(ExportToCsvFileAsync(request.Records, request.FilePath, request.Configuration));
                }
            }
            
            var results = await Task.WhenAll(tasks);
            
            _logger.LogInformation("批量导出CSV完成，总请求数: {Total}, 成功: {Success}, 失败: {Failed}",
                results.Length, results.Count(r => r.Success), results.Count(r => !r.Success));
            
            return results;
        }
        
        /// <summary>
        /// 获取CsvHelper状态
        /// </summary>
        public async Task<CsvHelperStatus> GetStatusAsync()
        {
            await Task.CompletedTask; // 模拟异步操作
            
            var totalCacheAccesses = _cacheHits + _cacheMisses;
            var cacheHitRate = totalCacheAccesses > 0 ? (double)_cacheHits / totalCacheAccesses * 100 : 0;
            
            var status = new CsvHelperStatus
            {
                IsRunning = true,
                ProcessedFiles = _processedFiles,
                ProcessedRecords = _processedRecords,
                SuccessfulFiles = _successfulFiles,
                FailedFiles = _failedFiles,
                CacheHitRate = Math.Round(cacheHitRate, 2),
                AverageExecutionTimeMs = 0, // 简化实现，实际应计算平均值
                StartTime = _startTime
            };
            
            _logger.LogDebug("获取CsvHelper状态: {@Status}", status);
            
            return status;
        }
        
        /// <summary>
        /// 重置CsvHelper状态
        /// </summary>
        public async Task<bool> ResetStatusAsync()
        {
            await Task.CompletedTask; // 模拟异步操作
            
            try
            {
                Interlocked.Exchange(ref _processedFiles, 0);
                Interlocked.Exchange(ref _processedRecords, 0);
                Interlocked.Exchange(ref _successfulFiles, 0);
                Interlocked.Exchange(ref _failedFiles, 0);
                Interlocked.Exchange(ref _cacheHits, 0);
                Interlocked.Exchange(ref _cacheMisses, 0);
                
                lock (_cache)
                {
                    _cache.Clear();
                }
                
                _logger.LogInformation("CsvHelper状态已重置");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "重置CsvHelper状态失败");
                return false;
            }
        }
        
        /// <summary>
        /// 创建默认CSV配置
        /// </summary>
        private CsvConfiguration CreateDefaultConfiguration()
        {
            return new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            {
                Delimiter = _options.DefaultDelimiter.ToString(),
                HasHeaderRecord = _options.HasHeaderRecord,
                IgnoreBlankLines = _options.IgnoreBlankLines,
                SkipEmptyRecords = true,
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
                HeaderValidated = null
            };
        }
        
        /// <summary>
        /// 生成缓存键
        /// </summary>
        private string GenerateCacheKey<T>(IEnumerable<T> records, CsvConfiguration configuration) where T : class
        {
            var recordCount = records.Count();
            var recordType = typeof(T).FullName;
            var configHash = GetHashCode(configuration.ToString());
            
            return $"csv_export:{recordType}:{recordCount}:{configHash}";
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
        private void AddToCache(string cacheKey, string result)
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
    /// CsvHelper AOT执行引擎
    /// 管理CsvHelper CSV处理
    /// </summary>
    public class CsvHelperAotEngine
    {
        private readonly ILogger<CsvHelperAotEngine> _logger;
        private readonly ICsvHelperService _csvHelperService;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="csvHelperService">CsvHelper服务</param>
        public CsvHelperAotEngine(ILogger<CsvHelperAotEngine> logger, ICsvHelperService csvHelperService)
        {
            _logger = logger;
            _csvHelperService = csvHelperService;
            
            _logger.LogInformation("CsvHelperAotEngine初始化成功");
        }
        
        /// <summary>
        /// 将对象列表导出为CSV字符串
        /// </summary>
        public async Task<string> ExportToCsvStringAsync<T>(IEnumerable<T> records, CsvConfiguration? configuration = null) where T : class
        {
            return await _csvHelperService.ExportToCsvStringAsync(records, configuration);
        }
        
        /// <summary>
        /// 将对象列表导出为CSV文件
        /// </summary>
        public async Task<CsvHelperResult> ExportToCsvFileAsync<T>(IEnumerable<T> records, string filePath, CsvConfiguration? configuration = null) where T : class
        {
            return await _csvHelperService.ExportToCsvFileAsync(records, filePath, configuration);
        }
        
        /// <summary>
        /// 从CSV字符串导入对象列表
        /// </summary>
        public async Task<List<T>> ImportFromCsvStringAsync<T>(string csvContent, CsvConfiguration? configuration = null) where T : class
        {
            return await _csvHelperService.ImportFromCsvStringAsync(csvContent, configuration);
        }
        
        /// <summary>
        /// 从CSV文件导入对象列表
        /// </summary>
        public async Task<CsvHelperResult<List<T>>> ImportFromCsvFileAsync<T>(string filePath, CsvConfiguration? configuration = null) where T : class
        {
            return await _csvHelperService.ImportFromCsvFileAsync(filePath, configuration);
        }
        
        /// <summary>
        /// 批量导出多个对象列表为CSV文件
        /// </summary>
        public async Task<IEnumerable<CsvHelperResult>> ExportBatchAsync<T>(IEnumerable<CsvExportRequest<T>> exportRequests) where T : class
        {
            return await _csvHelperService.ExportBatchAsync(exportRequests);
        }
        
        /// <summary>
        /// 获取CsvHelper状态
        /// </summary>
        public async Task<CsvHelperStatus> GetStatusAsync()
        {
            return await _csvHelperService.GetStatusAsync();
        }
        
        /// <summary>
        /// 重置CsvHelper状态
        /// </summary>
        public async Task<bool> ResetStatusAsync()
        {
            return await _csvHelperService.ResetStatusAsync();
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
            
            // 配置CsvHelper选项
            builder.Configuration.AddJsonFile("csvhelper_aot.setting.json", optional: true);
            builder.Services.Configure<CsvHelperOptions>(builder.Configuration.GetSection("CsvHelper"));
            
            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);
            
            // 注册服务
            builder.Services.AddSingleton<ICsvHelperService, CsvHelperService>();
            builder.Services.AddSingleton<CsvHelperAotEngine>();
            
            // 构建主机
            var host = builder.Build();
            var serviceProvider = host.Services;
            
            // 获取引擎实例
            var engine = serviceProvider.GetRequiredService<CsvHelperAotEngine>();
            
            // 解析命令行参数
            if (args.Length < 1)
            {
                Console.WriteLine("用法:");
                Console.WriteLine("  csvhelper_aot.exe <command> [arguments]");
                Console.WriteLine("  ");
                Console.WriteLine("命令:");
                Console.WriteLine("  status	获取服务状态");
                Console.WriteLine("  reset	重置服务状态");
                Console.WriteLine("  ");
                Console.WriteLine("示例:");
                Console.WriteLine("  csvhelper_aot.exe status");
                Console.WriteLine("  csvhelper_aot.exe reset");
                return 1;
            }
            
            try
            {
                string command = args[0].ToLower();
                
                switch (command)
                {
                    case "status":
                        var status = await engine.GetStatusAsync();
                        Console.WriteLine("CsvHelper服务状态:");
                        Console.WriteLine($"  运行状态: {(status.IsRunning ? "正常" : "异常"}");
                        Console.WriteLine($"  已处理文件数: {status.ProcessedFiles}");
                        Console.WriteLine($"  已处理记录数: {status.ProcessedRecords}");
                        Console.WriteLine($"  成功文件数: {status.SuccessfulFiles}");
                        Console.WriteLine($"  失败文件数: {status.FailedFiles}");
                        Console.WriteLine($"  缓存命中率: {status.CacheHitRate}%");
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