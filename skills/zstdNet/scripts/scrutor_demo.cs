#:sdk Microsoft.NET.Sdk
#:package ZstdSharp@0.8.0
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:package Scrutor@4.2.2
#:package Microsoft.Extensions.Logging@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZstdSharp;

namespace ZstdNet.ScrutorDemo
{
    /// <summary>
    /// 基础压缩服务接口
    /// </summary>
    public interface ICompressionService
    {
        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="data">要压缩的数据</param>
        /// <returns>压缩后的数据</returns>
        byte[] Compress(byte[] data);
        
        /// <summary>
        /// 解压缩数据
        /// </summary>
        /// <param name="compressedData">压缩的数据</param>
        /// <returns>解压缩后的数据</returns>
        byte[] Decompress(byte[] data);
    }

    /// <summary>
    /// Zstd 压缩服务实现
    /// </summary>
    public class ZstdCompressionService : ICompressionService
    {
        private readonly ILogger<ZstdCompressionService> _logger;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        public ZstdCompressionService(ILogger<ZstdCompressionService> logger = null)
        {
            _logger = logger;
        }
        
        /// <inheritdoc/>
        public byte[] Compress(byte[] data)
        {
            if (data == null || data.Length == 0)
                return Array.Empty<byte>();
            
            try
            {
                _logger?.LogInformation($"压缩数据，大小: {data.Length} 字节");
                using var compressor = new Compressor(3);
                var result = compressor.Wrap(data);
                _logger?.LogInformation($"压缩完成，压缩后大小: {result.Length} 字节，压缩率: {(double)result.Length / data.Length:P2}");
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "压缩数据时发生错误");
                throw;
            }
        }
        
        /// <inheritdoc/>
        public byte[] Decompress(byte[] data)
        {
            if (data == null || data.Length == 0)
                return Array.Empty<byte>();
            
            try
            {
                _logger?.LogInformation($"解压缩数据，大小: {data.Length} 字节");
                using var decompressor = new Decompressor();
                var result = decompressor.Unwrap(data);
                _logger?.LogInformation($"解压缩完成，解压缩后大小: {result.Length} 字节");
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "解压缩数据时发生错误");
                throw;
            }
        }
    }

    /// <summary>
    /// 性能监控装饰器
    /// </summary>
    public class PerformanceMonitoringCompressionDecorator : ICompressionService
    {
        private readonly ICompressionService _innerService;
        private readonly ILogger<PerformanceMonitoringCompressionDecorator> _logger;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="innerService">内部压缩服务</param>
        /// <param name="logger">日志记录器</param>
        public PerformanceMonitoringCompressionDecorator(ICompressionService innerService, ILogger<PerformanceMonitoringCompressionDecorator> logger)
        {
            _innerService = innerService ?? throw new ArgumentNullException(nameof(innerService));
            _logger = logger;
        }
        
        /// <inheritdoc/>
        public byte[] Compress(byte[] data)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var result = _innerService.Compress(data);
                stopwatch.Stop();
                _logger?.LogInformation($"压缩操作耗时: {stopwatch.ElapsedMilliseconds:F2} 毫秒");
                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger?.LogError(ex, $"压缩操作失败，耗时: {stopwatch.ElapsedMilliseconds:F2} 毫秒");
                throw;
            }
        }
        
        /// <inheritdoc/>
        public byte[] Decompress(byte[] data)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var result = _innerService.Decompress(data);
                stopwatch.Stop();
                _logger?.LogInformation($"解压缩操作耗时: {stopwatch.ElapsedMilliseconds:F2} 毫秒");
                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger?.LogError(ex, $"解压缩操作失败，耗时: {stopwatch.ElapsedMilliseconds:F2} 毫秒");
                throw;
            }
        }
    }

    /// <summary>
    /// 缓存装饰器
    /// </summary>
    public class CachingCompressionDecorator : ICompressionService
    {
        private readonly ICompressionService _innerService;
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, byte[]> _cache;
        private readonly ILogger<CachingCompressionDecorator> _logger;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="innerService">内部压缩服务</param>
        /// <param name="logger">日志记录器</param>
        public CachingCompressionDecorator(ICompressionService innerService, ILogger<CachingCompressionDecorator> logger)
        {
            _innerService = innerService ?? throw new ArgumentNullException(nameof(innerService));
            _cache = new System.Collections.Concurrent.ConcurrentDictionary<string, byte[]>();
            _logger = logger;
        }
        
        /// <summary>
        /// 生成缓存键
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>缓存键</returns>
        private string GenerateCacheKey(byte[] data)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hash = sha256.ComputeHash(data);
            return Convert.ToBase64String(hash);
        }
        
        /// <inheritdoc/>
        public byte[] Compress(byte[] data)
        {
            var cacheKey = "compress:" + GenerateCacheKey(data);
            
            if (_cache.TryGetValue(cacheKey, out var cachedResult))
            {
                _logger?.LogInformation("从缓存获取压缩结果");
                return cachedResult;
            }
            
            var result = _innerService.Compress(data);
            _cache.TryAdd(cacheKey, result);
            _logger?.LogInformation("压缩结果已缓存");
            return result;
        }
        
        /// <inheritdoc/>
        public byte[] Decompress(byte[] data)
        {
            var cacheKey = "decompress:" + GenerateCacheKey(data);
            
            if (_cache.TryGetValue(cacheKey, out var cachedResult))
            {
                _logger?.LogInformation("从缓存获取解压缩结果");
                return cachedResult;
            }
            
            var result = _innerService.Decompress(data);
            _cache.TryAdd(cacheKey, result);
            _logger?.LogInformation("解压缩结果已缓存");
            return result;
        }
    }

    /// <summary>
    /// 重试装饰器
    /// </summary>
    public class RetryCompressionDecorator : ICompressionService
    {
        private readonly ICompressionService _innerService;
        private readonly ILogger<RetryCompressionDecorator> _logger;
        private readonly int _maxRetries;
        private readonly TimeSpan _retryDelay;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="innerService">内部压缩服务</param>
        /// <param name="logger">日志记录器</param>
        /// <param name="maxRetries">最大重试次数</param>
        /// <param name="retryDelay">重试延迟</param>
        public RetryCompressionDecorator(ICompressionService innerService, ILogger<RetryCompressionDecorator> logger, int maxRetries = 3, TimeSpan? retryDelay = null)
        {
            _innerService = innerService ?? throw new ArgumentNullException(nameof(innerService));
            _logger = logger;
            _maxRetries = Math.Max(1, maxRetries);
            _retryDelay = retryDelay ?? TimeSpan.FromMilliseconds(100);
        }
        
        /// <inheritdoc/>
        public byte[] Compress(byte[] data)
        {
            int attempts = 0;
            while (true)
            {
                try
                {
                    attempts++;
                    return _innerService.Compress(data);
                }
                catch (Exception ex) when (attempts < _maxRetries)
                {
                    _logger?.LogWarning(ex, $"压缩操作失败，尝试 {attempts}/{_maxRetries}，将在 {_retryDelay.TotalMilliseconds}ms 后重试");
                    Thread.Sleep(_retryDelay);
                }
            }
        }
        
        /// <inheritdoc/>
        public byte[] Decompress(byte[] data)
        {
            int attempts = 0;
            while (true)
            {
                try
                {
                    attempts++;
                    return _innerService.Decompress(data);
                }
                catch (Exception ex) when (attempts < _maxRetries)
                {
                    _logger?.LogWarning(ex, $"解压缩操作失败，尝试 {attempts}/{_maxRetries}，将在 {_retryDelay.TotalMilliseconds}ms 后重试");
                    Thread.Sleep(_retryDelay);
                }
            }
        }
    }

    /// <summary>
    /// 高级压缩服务接口
    /// </summary>
    public interface IAdvancedCompressionService : ICompressionService
    {
        /// <summary>
        /// 压缩数据（指定压缩级别）
        /// </summary>
        /// <param name="data">要压缩的数据</param>
        /// <param name="compressionLevel">压缩级别 (1-19)</param>
        /// <returns>压缩后的数据</returns>
        byte[] Compress(byte[] data, int compressionLevel);
        
        /// <summary>
        /// 使用字典压缩
        /// </summary>
        /// <param name="data">要压缩的数据</param>
        /// <param name="dictionary">压缩字典</param>
        /// <returns>压缩后的数据</returns>
        byte[] CompressWithDictionary(byte[] data, byte[] dictionary);
    }

    /// <summary>
    /// 高级 Zstd 压缩服务实现
    /// </summary>
    public class AdvancedZstdCompressionService : ZstdCompressionService, IAdvancedCompressionService
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        public AdvancedZstdCompressionService(ILogger<AdvancedZstdCompressionService> logger = null) : base(logger)
        {}
        
        /// <inheritdoc/>
        public byte[] Compress(byte[] data, int compressionLevel)
        {
            if (data == null || data.Length == 0)
                return Array.Empty<byte>();
            
            try
            {
                Logger?.LogInformation($"压缩数据（级别: {compressionLevel}），大小: {data.Length} 字节");
                using var compressor = new Compressor(compressionLevel);
                var result = compressor.Wrap(data);
                Logger?.LogInformation($"压缩完成，压缩后大小: {result.Length} 字节，压缩率: {(double)result.Length / data.Length:P2}");
                return result;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "压缩数据时发生错误");
                throw;
            }
        }
        
        /// <inheritdoc/>
        public byte[] CompressWithDictionary(byte[] data, byte[] dictionary)
        {
            if (data == null || data.Length == 0)
                return Array.Empty<byte>();
            
            if (dictionary == null || dictionary.Length == 0)
                return Compress(data);
            
            try
            {
                Logger?.LogInformation($"使用字典压缩数据，数据大小: {data.Length} 字节，字典大小: {dictionary.Length} 字节");
                using var dict = new CompressionDictionary(dictionary);
                using var compressor = new Compressor(3, dict);
                var result = compressor.Wrap(data);
                Logger?.LogInformation($"压缩完成，压缩后大小: {result.Length} 字节，压缩率: {(double)result.Length / data.Length:P2}");
                return result;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "使用字典压缩数据时发生错误");
                throw;
            }
        }
        
        /// <summary>
        /// 内部日志记录器
        /// </summary>
        private ILogger Logger { get; }
    }

    /// <summary>
    /// 标记接口：表示该服务应该被自动注册
    /// </summary>
    public interface IAutoRegister { }

    /// <summary>
    /// 示例服务：实现了 IAutoRegister 接口
    /// </summary>
    public class SampleAutoRegisterService : IAutoRegister
    {
        public string GetMessage() => "这是一个自动注册的服务";
    }

    /// <summary>
    /// 另一个示例服务：实现了 IAutoRegister 接口
    /// </summary>
    public class AnotherAutoRegisterService : IAutoRegister
    {
        public string GetInfo() => "这是另一个自动注册的服务";
    }

    /// <summary>
    /// Scrutor 演示程序
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("=== Scrutor 演示程序 ===");
            
            // 测试基本的 Scrutor 用法
            await TestBasicScrutorUsage();
            
            // 测试装饰器模式
            await TestDecoratorPattern();
            
            // 测试自动注册
            await TestAutoRegistration();
            
            // 测试高级用法
            await TestAdvancedScrutorUsage();
            
            Console.WriteLine("\n=== 演示完成 ===");
        }
        
        /// <summary>
        /// 测试基本的 Scrutor 用法
        /// </summary>
        private static async Task TestBasicScrutorUsage()
        {
            Console.WriteLine("\n1. 测试基本的 Scrutor 用法");
            
            var services = new ServiceCollection();
            
            // 添加日志服务
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            
            // 使用 Scrutor 注册服务
            // 方式 1: 显式注册
            services.AddScoped<ICompressionService, ZstdCompressionService>();
            
            // 构建服务提供者
            using var serviceProvider = services.BuildServiceProvider();
            
            // 获取服务
            var compressionService = serviceProvider.GetRequiredService<ICompressionService>();
            
            // 测试压缩功能
            var testData = System.Text.Encoding.UTF8.GetBytes("这是测试数据，用于测试 Scrutor 的基本用法.重复测试数据以获得更好的压缩效果.这是测试数据，用于测试 Scrutor 的基本用法.重复测试数据以获得更好的压缩效果.");
            Console.WriteLine($"原始数据大小: {testData.Length} 字节");
            
            var compressedData = compressionService.Compress(testData);
            Console.WriteLine($"压缩后大小: {compressedData.Length} 字节");
            
            var decompressedData = compressionService.Decompress(compressedData);
            Console.WriteLine($"解压缩后大小: {decompressedData.Length} 字节");
            
            var decompressedText = System.Text.Encoding.UTF8.GetString(decompressedData);
            Console.WriteLine($"解压缩结果是否正确: {decompressedText == System.Text.Encoding.UTF8.GetString(testData)}");
        }
        
        /// <summary>
        /// 测试装饰器模式
        /// </summary>
        private static async Task TestDecoratorPattern()
        {
            Console.WriteLine("\n2. 测试装饰器模式");
            
            var services = new ServiceCollection();
            
            // 添加日志服务
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            
            // 注册基础服务
            services.AddScoped<ICompressionService, ZstdCompressionService>();
            
            // 使用 Scrutor 添加装饰器
            // 注意：装饰器的添加顺序很重要，先添加的装饰器会被后添加的装饰器包装
            services.Decorate<ICompressionService, CachingCompressionDecorator>();
            services.Decorate<ICompressionService, PerformanceMonitoringCompressionDecorator>();
            services.Decorate<ICompressionService, RetryCompressionDecorator>();
            
            // 构建服务提供者
            using var serviceProvider = services.BuildServiceProvider();
            
            // 获取服务（此时获取到的是被多层装饰器包装后的服务）
            var compressionService = serviceProvider.GetRequiredService<ICompressionService>();
            
            // 测试压缩功能（第一次压缩，应该会执行完整的压缩流程）
            var testData = System.Text.Encoding.UTF8.GetBytes("这是测试数据，用于测试装饰器模式.装饰器模式可以为服务添加额外的功能，而不需要修改原始服务的代码.");
            Console.WriteLine($"\n第一次压缩测试:");
            
            var compressedData1 = compressionService.Compress(testData);
            var decompressedData1 = compressionService.Decompress(compressedData1);
            
            // 测试压缩功能（第二次压缩相同的数据，应该会从缓存中获取）
            Console.WriteLine($"\n第二次压缩测试（相同数据，应该从缓存获取）:");
            var compressedData2 = compressionService.Compress(testData);
            var decompressedData2 = compressionService.Decompress(compressedData2);
            
            // 验证结果
            Console.WriteLine($"\n验证结果:");
            Console.WriteLine($"两次压缩结果是否相同: {compressedData1.SequenceEqual(compressedData2)}");
            Console.WriteLine($"两次解压缩结果是否相同: {decompressedData1.SequenceEqual(decompressedData2)}");
        }
        
        /// <summary>
        /// 测试自动注册
        /// </summary>
        private static async Task TestAutoRegistration()
        {
            Console.WriteLine("\n3. 测试自动注册");
            
            var services = new ServiceCollection();
            
            // 添加日志服务
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            
            // 使用 Scrutor 自动注册服务
            // 方式 1: 按接口注册
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.AssignableTo<ICompressionService>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());
            
            // 方式 2: 按标记接口注册
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.AssignableTo<IAutoRegister>())
                .AsSelf()
                .WithTransientLifetime());
            
            // 构建服务提供者
            using var serviceProvider = services.BuildServiceProvider();
            
            // 测试自动注册的压缩服务
            var compressionService = serviceProvider.GetService<ICompressionService>();
            if (compressionService != null)
            {
                Console.WriteLine("\n测试自动注册的压缩服务:");
                var testData = System.Text.Encoding.UTF8.GetBytes("这是测试数据，用于测试自动注册功能.");
                var compressedData = compressionService.Compress(testData);
                Console.WriteLine($"压缩成功，压缩率: {(double)compressedData.Length / testData.Length:P2}");
            }
            
            // 测试自动注册的其他服务
            var sampleService = serviceProvider.GetService<SampleAutoRegisterService>();
            if (sampleService != null)
            {
                Console.WriteLine($"\n测试 SampleAutoRegisterService: {sampleService.GetMessage()}");
            }
            
            var anotherService = serviceProvider.GetService<AnotherAutoRegisterService>();
            if (anotherService != null)
            {
                Console.WriteLine($"测试 AnotherAutoRegisterService: {anotherService.GetInfo()}");
            }
        }
        
        /// <summary>
        /// 测试高级的 Scrutor 用法
        /// </summary>
        private static async Task TestAdvancedScrutorUsage()
        {
            Console.WriteLine("\n4. 测试高级的 Scrutor 用法");
            
            var services = new ServiceCollection();
            
            // 添加日志服务
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            
            // 高级扫描和注册
            services.Scan(scan => scan
                // 从当前程序集和包含 ZstdCompressionService 的程序集扫描
                .FromAssembliesOf(typeof(Program), typeof(ZstdCompressionService))
                
                // 注册压缩相关的服务
                .AddClasses(classes => classes.Where(c => c.Name.Contains("Compression")))
                .As(t => t.GetInterfaces().FirstOrDefault(i => i.Name.Contains("Compression")) ?? t)
                .WithScopedLifetime()
                
                // 注册其他服务
                .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Service")))
                .AsSelf()
                .WithTransientLifetime());
            
            // 为特定服务添加装饰器
            services.Decorate<ICompressionService>(provider =>
            {
                var innerService = provider.GetRequiredService<ICompressionService>();
                var logger = provider.GetRequiredService<ILogger<PerformanceMonitoringCompressionDecorator>>();
                return new PerformanceMonitoringCompressionDecorator(innerService, logger);
            });
            
            // 构建服务提供者
            using var serviceProvider = services.BuildServiceProvider();
            
            // 测试高级压缩服务
            var advancedService = serviceProvider.GetService<IAdvancedCompressionService>();
            if (advancedService != null)
            {
                Console.WriteLine("\n测试高级压缩服务:");
                
                var testData = System.Text.Encoding.UTF8.GetBytes("这是测试数据，用于测试高级压缩服务.高级压缩服务支持指定压缩级别和使用字典压缩.");
                
                // 测试不同压缩级别
                for (int level = 1; level <= 5; level += 2)
                {
                    var compressedData = advancedService.Compress(testData, level);
                    Console.WriteLine($"压缩级别 {level}: 压缩率 {(double)compressedData.Length / testData.Length:P2}");
                }
                
                // 测试字典压缩
                var dictionaryData = System.Text.Encoding.UTF8.GetBytes("这是测试数据，用于测试高级压缩服务.");
                var compressedWithDict = advancedService.CompressWithDictionary(testData, dictionaryData);
                var compressedWithoutDict = advancedService.Compress(testData);
                
                Console.WriteLine($"\n字典压缩测试:");
                Console.WriteLine($"使用字典压缩率: {(double)compressedWithDict.Length / testData.Length:P2}");
                Console.WriteLine($"不使用字典压缩率: {(double)compressedWithoutDict.Length / testData.Length:P2}");
                Console.WriteLine($"字典压缩效果提升: {((double)compressedWithoutDict.Length / compressedWithDict.Length - 1):P2}");
            }
            
            // 测试服务解析
            Console.WriteLine("\n测试服务解析:");
            var serviceTypes = serviceProvider.GetServices<object>()
                .Select(s => s.GetType().FullName)
                .Distinct()
                .ToList();
            
            Console.WriteLine($"解析到的服务数量: {serviceTypes.Count}");
            foreach (var serviceType in serviceTypes.Take(5)) // 只显示前 5 个
            {
                Console.WriteLine($"- {serviceType}");
            }
            if (serviceTypes.Count > 5)
            {
                Console.WriteLine($"... 还有 {serviceTypes.Count - 5} 个服务");
            }
        }
    }

    /// <summary>
    /// Scrutor 扩展方法
    /// </summary>
    public static class ScrutorExtensions
    {
        /// <summary>
        /// 自动注册所有实现了指定接口的服务
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="lifetime">生命周期</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AutoRegisterServices<TInterface>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            services.Scan(scan => scan
                .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                .AddClasses(classes => classes.AssignableTo<TInterface>())
                .AsImplementedInterfaces()
                .WithLifetime(lifetime));
            
            return services;
        }
        
        /// <summary>
        /// 为服务添加性能监控装饰器
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddPerformanceMonitoring<TService>(this IServiceCollection services)
            where TService : class
        {
            services.Decorate<TService>((provider, inner) =>
            {
                var logger = provider.GetRequiredService<ILogger<PerformanceMonitoringCompressionDecorator>>();
                
                // 注意：这里只是示例，实际使用时需要根据具体的服务类型创建对应的装饰器
                if (inner is ICompressionService compressionService)
                {
                    return (TService)(object)new PerformanceMonitoringCompressionDecorator(compressionService, logger);
                }
                
                return inner;
            });
            
            return services;
        }
    }
}
