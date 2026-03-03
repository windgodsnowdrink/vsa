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

namespace Cscore.AOT
{
    /// <summary>
    /// Cscore配置选项
    /// </summary>
    public class CscoreOptions
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
        /// 音频采样率
        /// </summary>
        public int SampleRate { get; set; } = 44100;
        
        /// <summary>
        /// 音频通道数
        /// </summary>
        public int ChannelCount { get; set; } = 2;
        
        /// <summary>
        /// 音频位深度
        /// </summary>
        public int BitsPerSample { get; set; } = 16;
    }
    
    /// <summary>
    /// Cscore服务接口
    /// 定义了Cscore的核心功能
    /// </summary>
    public interface ICscoreService
    {
        /// <summary>
        /// 处理音频数据
        /// </summary>
        /// <param name="audioData">音频数据</param>
        /// <param name="options">处理选项</param>
        /// <returns>处理结果</returns>
        Task<CscoreResult> ProcessAudioAsync(byte[] audioData, AudioProcessingOptions options);
        
        /// <summary>
        /// 批量处理音频数据
        /// </summary>
        /// <param name="audioDataList">音频数据列表</param>
        /// <param name="options">处理选项</param>
        /// <returns>处理结果列表</returns>
        Task<IEnumerable<CscoreResult>> ProcessAudioBatchAsync(IEnumerable<byte[]> audioDataList, AudioProcessingOptions options);
        
        /// <summary>
        /// 获取音频设备列表
        /// </summary>
        /// <returns>设备列表</returns>
        Task<IEnumerable<AudioDevice>> GetAudioDevicesAsync();
        
        /// <summary>
        /// 获取Cscore状态
        /// </summary>
        /// <returns>状态信息</returns>
        Task<CscoreStatus> GetStatusAsync();
        
        /// <summary>
        /// 重置Cscore状态
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> ResetStatusAsync();
    }
    
    /// <summary>
    /// 音频处理选项
    /// </summary>
    public class AudioProcessingOptions
    {
        /// <summary>
        /// 处理类型
        /// </summary>
        public string ProcessingType { get; set; } = "default";
        
        /// <summary>
        /// 音量增益（分贝）
        /// </summary>
        public float VolumeGain { get; set; } = 0;
        
        /// <summary>
        /// 采样率转换
        /// </summary>
        public int? TargetSampleRate { get; set; }
        
        /// <summary>
        /// 通道数转换
        /// </summary>
        public int? TargetChannelCount { get; set; }
        
        /// <summary>
        /// 是否启用降噪
        /// </summary>
        public bool EnableNoiseReduction { get; set; } = false;
        
        /// <summary>
        /// 降噪强度
        /// </summary>
        public float NoiseReductionLevel { get; set; } = 0.5f;
    }
    
    /// <summary>
    /// Cscore操作结果
    /// </summary>
    public class CscoreResult
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 处理后的音频数据
        /// </summary>
        public byte[]? ProcessedAudioData { get; set; }
        
        /// <summary>
        /// 错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// 执行时间（毫秒）
        /// </summary>
        public long ExecutionTimeMs { get; set; }
        
        /// <summary>
        /// 处理前后音频数据大小
        /// </summary>
        public AudioSizeInfo? SizeInfo { get; set; }
    }
    
    /// <summary>
    /// 音频大小信息
    /// </summary>
    public class AudioSizeInfo
    {
        /// <summary>
        /// 原始大小（字节）
        /// </summary>
        public long OriginalSize { get; set; }
        
        /// <summary>
        /// 处理后大小（字节）
        /// </summary>
        public long ProcessedSize { get; set; }
        
        /// <summary>
        /// 压缩率（百分比）
        /// </summary>
        public double CompressionRatio { get; set; }
    }
    
    /// <summary>
    /// 音频设备信息
    /// </summary>
    public class AudioDevice
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;
        
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; } = string.Empty;
        
        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceType DeviceType { get; set; } = DeviceType.Unknown;
        
        /// <summary>
        /// 支持的采样率
        /// </summary>
        public IEnumerable<int> SupportedSampleRates { get; set; } = new List<int>();
        
        /// <summary>
        /// 支持的通道数
        /// </summary>
        public IEnumerable<int> SupportedChannelCounts { get; set; } = new List<int>();
    }
    
    /// <summary>
    /// 设备类型
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// 未知设备
        /// </summary>
        Unknown,
        /// <summary>
        /// 输入设备（麦克风）
        /// </summary>
        Input,
        /// <summary>
        /// 输出设备（扬声器）
        /// </summary>
        Output,
        /// <summary>
        /// 输入输出设备
        /// </summary>
        InputOutput
    }
    
    /// <summary>
    /// Cscore状态信息
    /// </summary>
    public class CscoreStatus
    {
        /// <summary>
        /// 服务是否正常运行
        /// </summary>
        public bool IsRunning { get; set; }
        
        /// <summary>
        /// 已处理的音频文件数
        /// </summary>
        public long ProcessedFiles { get; set; }
        
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
        /// 平均处理时间（毫秒）
        /// </summary>
        public double AverageProcessingTimeMs { get; set; }
        
        /// <summary>
        /// 服务启动时间
        /// </summary>
        public DateTime StartTime { get; set; }
    }
    
    /// <summary>
    /// Cscore服务实现
    /// 基于.NET 10 AOT架构，提供高性能音频处理功能
    /// </summary>
    public class CscoreService : ICscoreService
    {
        private readonly ILogger<CscoreService> _logger;
        private readonly CscoreOptions _options;
        private readonly Dictionary<string, CscoreResult> _cache = new Dictionary<string, CscoreResult>();
        private long _processedFiles = 0;
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
        public CscoreService(ILogger<CscoreService> logger, IOptions<CscoreOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            
            _logger.LogInformation("CscoreService初始化成功，配置选项：SampleRate={SampleRate}, ChannelCount={ChannelCount}, BitsPerSample={BitsPerSample}",
                _options.SampleRate, _options.ChannelCount, _options.BitsPerSample);
        }
        
        /// <summary>
        /// 处理音频数据
        /// </summary>
        public async Task<CscoreResult> ProcessAudioAsync(byte[] audioData, AudioProcessingOptions options)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new CscoreResult();
            
            try
            {
                Interlocked.Increment(ref _processedFiles);
                
                // 生成缓存键
                var cacheKey = GenerateCacheKey(audioData, options);
                
                // 检查缓存
                if (_options.EnableCache && _cache.TryGetValue(cacheKey, out var cachedResult))
                {
                    Interlocked.Increment(ref _cacheHits);
                    _logger.LogDebug("Cache hit for audio processing, CacheKey: {CacheKey}", cacheKey);
                    return cachedResult;
                }
                
                if (_options.EnableCache)
                {
                    Interlocked.Increment(ref _cacheMisses);
                }
                
                _logger.LogInformation("开始处理音频数据，大小: {Size} bytes, 处理类型: {ProcessingType}", 
                    audioData.Length, options.ProcessingType);
                
                // 执行实际音频处理
                await Task.Delay(100); // 模拟音频处理延迟
                
                // 根据处理类型执行不同的逻辑
                byte[] processedData;
                switch (options.ProcessingType.ToLower())
                {
                    case "volume":
                        processedData = await ApplyVolumeAsync(audioData, options.VolumeGain);
                        break;
                    case "resample":
                        processedData = await ResampleAsync(audioData, options.TargetSampleRate ?? _options.SampleRate);
                        break;
                    case "noise_reduction":
                        processedData = await ApplyNoiseReductionAsync(audioData, options.NoiseReductionLevel);
                        break;
                    default:
                        processedData = await DefaultProcessingAsync(audioData);
                        break;
                }
                
                // 计算大小信息
                var sizeInfo = new AudioSizeInfo
                {
                    OriginalSize = audioData.Length,
                    ProcessedSize = processedData.Length,
                    CompressionRatio = audioData.Length > 0 ? ((double)(audioData.Length - processedData.Length) / audioData.Length) * 100 : 0
                };
                
                // 设置结果
                result.Success = true;
                result.ProcessedAudioData = processedData;
                result.SizeInfo = sizeInfo;
                
                Interlocked.Increment(ref _successfulFiles);
                
                // 缓存结果
                if (_options.EnableCache)
                {
                    AddToCache(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedFiles);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "处理音频数据失败");
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }
            
            _logger.LogInformation("音频数据处理完成，结果: {Success}, 执行时间: {ExecutionTimeMs}ms",
                result.Success, result.ExecutionTimeMs);
            
            return result;
        }
        
        /// <summary>
        /// 批量处理音频数据
        /// </summary>
        public async Task<IEnumerable<CscoreResult>> ProcessAudioBatchAsync(IEnumerable<byte[]> audioDataList, AudioProcessingOptions options)
        {
            _logger.LogInformation("开始批量处理音频数据，文件数量: {Count}", audioDataList.Count());
            
            var tasks = audioDataList.Select(audioData => ProcessAudioAsync(audioData, options));
            var results = await Task.WhenAll(tasks);
            
            _logger.LogInformation("批量处理音频数据完成，总文件数: {Total}, 成功: {Success}, 失败: {Failed}",
                results.Length, results.Count(r => r.Success), results.Count(r => !r.Success));
            
            return results;
        }
        
        /// <summary>
        /// 获取音频设备列表
        /// </summary>
        public async Task<IEnumerable<AudioDevice>> GetAudioDevicesAsync()
        {
            await Task.Delay(50); // 模拟异步操作
            
            // 模拟音频设备列表
            var devices = new List<AudioDevice>
            {
                new AudioDevice
                {
                    DeviceId = "device_1",
                    DeviceName = "麦克风 (Realtek Audio)",
                    DeviceType = DeviceType.Input,
                    SupportedSampleRates = new List<int> { 44100, 48000, 96000 },
                    SupportedChannelCounts = new List<int> { 1, 2 }
                },
                new AudioDevice
                {
                    DeviceId = "device_2",
                    DeviceName = "扬声器 (Realtek Audio)",
                    DeviceType = DeviceType.Output,
                    SupportedSampleRates = new List<int> { 44100, 48000, 96000, 192000 },
                    SupportedChannelCounts = new List<int> { 2, 4, 6, 8 }
                },
                new AudioDevice
                {
                    DeviceId = "device_3",
                    DeviceName = "USB 音频设备",
                    DeviceType = DeviceType.InputOutput,
                    SupportedSampleRates = new List<int> { 44100, 48000 },
                    SupportedChannelCounts = new List<int> { 1, 2 }
                }
            };
            
            _logger.LogInformation("获取音频设备列表成功，设备数量: {Count}", devices.Count);
            return devices;
        }
        
        /// <summary>
        /// 获取Cscore状态
        /// </summary>
        public async Task<CscoreStatus> GetStatusAsync()
        {
            await Task.CompletedTask; // 模拟异步操作
            
            var totalCacheAccesses = _cacheHits + _cacheMisses;
            var cacheHitRate = totalCacheAccesses > 0 ? (double)_cacheHits / totalCacheAccesses * 100 : 0;
            
            var status = new CscoreStatus
            {
                IsRunning = true,
                ProcessedFiles = _processedFiles,
                SuccessfulFiles = _successfulFiles,
                FailedFiles = _failedFiles,
                CacheHitRate = Math.Round(cacheHitRate, 2),
                AverageProcessingTimeMs = 0, // 简化实现，实际应计算平均值
                StartTime = _startTime
            };
            
            _logger.LogDebug("获取Cscore状态: {@Status}", status);
            
            return status;
        }
        
        /// <summary>
        /// 重置Cscore状态
        /// </summary>
        public async Task<bool> ResetStatusAsync()
        {
            await Task.CompletedTask; // 模拟异步操作
            
            try
            {
                Interlocked.Exchange(ref _processedFiles, 0);
                Interlocked.Exchange(ref _successfulFiles, 0);
                Interlocked.Exchange(ref _failedFiles, 0);
                Interlocked.Exchange(ref _cacheHits, 0);
                Interlocked.Exchange(ref _cacheMisses, 0);
                
                lock (_cache)
                {
                    _cache.Clear();
                }
                
                _logger.LogInformation("Cscore状态已重置");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "重置Cscore状态失败");
                return false;
            }
        }
        
        /// <summary>
        /// 默认音频处理
        /// </summary>
        private async Task<byte[]> DefaultProcessingAsync(byte[] audioData)
        {
            await Task.Delay(50); // 模拟处理延迟
            return audioData; // 默认返回原数据
        }
        
        /// <summary>
        /// 应用音量增益
        /// </summary>
        private async Task<byte[]> ApplyVolumeAsync(byte[] audioData, float gainDb)
        {
            await Task.Delay(70); // 模拟处理延迟
            
            // 模拟音量增益处理
            // 实际实现中，这里应该对音频数据进行音量调整
            return audioData;
        }
        
        /// <summary>
        /// 重采样
        /// </summary>
        private async Task<byte[]> ResampleAsync(byte[] audioData, int targetSampleRate)
        {
            await Task.Delay(150); // 模拟处理延迟
            
            // 模拟重采样处理
            // 实际实现中，这里应该对音频数据进行重采样
            return audioData;
        }
        
        /// <summary>
        /// 应用降噪
        /// </summary>
        private async Task<byte[]> ApplyNoiseReductionAsync(byte[] audioData, float level)
        {
            await Task.Delay(200); // 模拟处理延迟
            
            // 模拟降噪处理
            // 实际实现中，这里应该对音频数据进行降噪
            return audioData;
        }
        
        /// <summary>
        /// 生成缓存键
        /// </summary>
        private string GenerateCacheKey(byte[] audioData, AudioProcessingOptions options)
        {
            var hash = GetHashCode(audioData);
            return $"{hash}:{options.ProcessingType}:{options.VolumeGain}:{options.TargetSampleRate}:{options.EnableNoiseReduction}:{options.NoiseReductionLevel}";
        }
        
        /// <summary>
        /// 获取字节数组的哈希码
        /// </summary>
        private int GetHashCode(byte[] data)
        {
            unchecked
            {
                int hash = 17;
                foreach (byte b in data)
                {
                    hash = hash * 23 + b;
                }
                return hash;
            }
        }
        
        /// <summary>
        /// 添加到缓存
        /// </summary>
        private void AddToCache(string cacheKey, CscoreResult result)
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
    /// Cscore AOT执行引擎
    /// 管理Cscore音频处理的执行
    /// </summary>
    public class CscoreAotEngine
    {
        private readonly ILogger<CscoreAotEngine> _logger;
        private readonly ICscoreService _cscoreService;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="cscoreService">Cscore服务</param>
        public CscoreAotEngine(ILogger<CscoreAotEngine> logger, ICscoreService cscoreService)
        {
            _logger = logger;
            _cscoreService = cscoreService;
            
            _logger.LogInformation("CscoreAotEngine初始化成功");
        }
        
        /// <summary>
        /// 处理音频数据
        /// </summary>
        /// <param name="audioData">音频数据</param>
        /// <param name="options">处理选项</param>
        /// <returns>处理结果</returns>
        public async Task<CscoreResult> ProcessAudioAsync(byte[] audioData, AudioProcessingOptions options)
        {
            return await _cscoreService.ProcessAudioAsync(audioData, options);
        }
        
        /// <summary>
        /// 批量处理音频数据
        /// </summary>
        /// <param name="audioDataList">音频数据列表</param>
        /// <param name="options">处理选项</param>
        /// <returns>处理结果列表</returns>
        public async Task<IEnumerable<CscoreResult>> ProcessAudioBatchAsync(IEnumerable<byte[]> audioDataList, AudioProcessingOptions options)
        {
            return await _cscoreService.ProcessAudioBatchAsync(audioDataList, options);
        }
        
        /// <summary>
        /// 获取音频设备列表
        /// </summary>
        /// <returns>设备列表</returns>
        public async Task<IEnumerable<AudioDevice>> GetAudioDevicesAsync()
        {
            return await _cscoreService.GetAudioDevicesAsync();
        }
        
        /// <summary>
        /// 获取Cscore状态
        /// </summary>
        /// <returns>状态信息</returns>
        public async Task<CscoreStatus> GetStatusAsync()
        {
            return await _cscoreService.GetStatusAsync();
        }
        
        /// <summary>
        /// 重置Cscore状态
        /// </summary>
        /// <returns>操作结果</returns>
        public async Task<bool> ResetStatusAsync()
        {
            return await _cscoreService.ResetStatusAsync();
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
            
            // 配置Cscore选项
            builder.Configuration.AddJsonFile("cscore_aot.setting.json", optional: true);
            builder.Services.Configure<CscoreOptions>(builder.Configuration.GetSection("Cscore"));
            
            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);
            
            // 注册服务
            builder.Services.AddSingleton<ICscoreService, CscoreService>();
            builder.Services.AddSingleton<CscoreAotEngine>();
            
            // 构建主机
            var host = builder.Build();
            var serviceProvider = host.Services;
            
            // 获取引擎实例
            var engine = serviceProvider.GetRequiredService<CscoreAotEngine>();
            
            // 解析命令行参数
            if (args.Length < 1)
            {
                Console.WriteLine("用法:");
                Console.WriteLine("  cscore_aot.exe <command> [arguments]");
                Console.WriteLine("  ");
                Console.WriteLine("命令:");
                Console.WriteLine("  process <inputfile> <outputfile> [processingtype]	处理音频文件");
                Console.WriteLine("  devices	获取音频设备列表");
                Console.WriteLine("  status	获取Cscore状态");
                Console.WriteLine("  reset	重置Cscore状态");
                Console.WriteLine("  ");
                Console.WriteLine("示例:");
                Console.WriteLine("  cscore_aot.exe process input.wav output.wav volume");
                Console.WriteLine("  cscore_aot.exe process input.wav output.wav noise_reduction");
                Console.WriteLine("  cscore_aot.exe devices");
                return 1;
            }
            
            try
            {
                string command = args[0].ToLower();
                
                switch (command)
                {
                    case "process":
                        if (args.Length < 3)
                        {
                            Console.WriteLine("缺少参数: inputfile outputfile [processingtype]");
                            return 1;
                        }
                        
                        // 解析参数
                        string inputFile = args[1];
                        string outputFile = args[2];
                        string processingType = args.Length > 3 ? args[3] : "default";
                        
                        // 读取输入文件
                        if (!File.Exists(inputFile))
                        {
                            Console.WriteLine($"输入文件不存在: {inputFile}");
                            return 1;
                        }
                        
                        byte[] audioData = await File.ReadAllBytesAsync(inputFile);
                        
                        // 执行音频处理
                        var options = new AudioProcessingOptions
                        {
                            ProcessingType = processingType
                        };
                        
                        var result = await engine.ProcessAudioAsync(audioData, options);
                        
                        if (result.Success && result.ProcessedAudioData != null)
                        {
                            // 保存处理后的文件
                            await File.WriteAllBytesAsync(outputFile, result.ProcessedAudioData);
                            Console.WriteLine($"音频处理成功，输出文件: {outputFile}");
                            Console.WriteLine($"执行时间: {result.ExecutionTimeMs}ms");
                            if (result.SizeInfo != null)
                            {
                                Console.WriteLine($"原始大小: {result.SizeInfo.OriginalSize} bytes");
                                Console.WriteLine($"处理后大小: {result.SizeInfo.ProcessedSize} bytes");
                                Console.WriteLine($"压缩率: {result.SizeInfo.CompressionRatio:F2}%");
                            }
                            return 0;
                        }
                        else
                        {
                            Console.WriteLine($"音频处理失败: {result.ErrorMessage}");
                            return 1;
                        }
                        
                    case "devices":
                        var devices = await engine.GetAudioDevicesAsync();
                        Console.WriteLine("音频设备列表:");
                        foreach (var device in devices)
                        {
                            Console.WriteLine($"- {device.DeviceName} ({device.DeviceType})");
                            Console.WriteLine($"  ID: {device.DeviceId}");
                            Console.WriteLine($"  支持的采样率: {string.Join(", ", device.SupportedSampleRates)}");
                            Console.WriteLine($"  支持的通道数: {string.Join(", ", device.SupportedChannelCounts)}");
                        }
                        return 0;
                        
                    case "status":
                        var status = await engine.GetStatusAsync();
                        Console.WriteLine("Cscore状态:");
                        Console.WriteLine($"  运行状态: {(status.IsRunning ? "正常" : "异常")}");
                        Console.WriteLine($"  已处理文件数: {status.ProcessedFiles}");
                        Console.WriteLine($"  成功文件数: {status.SuccessfulFiles}");
                        Console.WriteLine($"  失败文件数: {status.FailedFiles}");
                        Console.WriteLine($"  缓存命中率: {status.CacheHitRate}%");
                        Console.WriteLine($"  平均处理时间: {status.AverageProcessingTimeMs}ms");
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