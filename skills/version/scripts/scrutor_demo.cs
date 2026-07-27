#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Scrutor@4.2.2
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using VersionSkill;

namespace ScrutorDemo
{
    /// <summary>
    /// 日志服务接口
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="message">日志消息</param>
        void Info(string message);

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="exception">异常对象</param>
        void Error(string message, Exception exception = null);
    }

    /// <summary>
    /// 控制台日志服务实现
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="message">日志消息</param>
        public void Info(string message)
        {
            Console.WriteLine($"[INFO] {message}");
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="exception">异常对象</param>
        public void Error(string message, Exception exception = null)
        {
            Console.WriteLine($"[ERROR] {message}");
            if (exception != null)
            {
                Console.WriteLine($"[ERROR] {exception.Message}");
            }
        }
    }

    /// <summary>
    /// 版本服务装饰器接口
    /// </summary>
    public interface IVersionServiceDecorator : IVersionService
    {}

    /// <summary>
    /// 版本服务日志装饰器
    /// </summary>
    public class LoggingVersionServiceDecorator : IVersionServiceDecorator
    {
        private readonly IVersionService _innerService;
        private readonly ILogger _logger;

        /// <summary>
        /// 初始化 <see cref="LoggingVersionServiceDecorator"/> 类的新实例
        /// </summary>
        /// <param name="innerService">内部版本服务</param>
        /// <param name="logger">日志服务</param>
        public LoggingVersionServiceDecorator(IVersionService innerService, ILogger logger)
        {
            _innerService = innerService ?? throw new ArgumentNullException(nameof(innerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 解析版本字符串
        /// </summary>
        /// <param name="versionString">版本字符串</param>
        /// <returns>语义化版本对象</returns>
        public SemanticVersion Parse(string versionString)
        {
            _logger.Info($"解析版本字符串: {versionString}");
            try
            {
                var result = _innerService.Parse(versionString);
                _logger.Info($"解析结果: {result}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"解析版本字符串失败: {versionString}", ex);
                throw;
            }
        }

        /// <summary>
        /// 尝试解析版本字符串
        /// </summary>
        /// <param name="versionString">版本字符串</param>
        /// <param name="version">语义化版本对象</param>
        /// <returns>解析是否成功</returns>
        public bool TryParse(string versionString, out SemanticVersion version)
        {
            _logger.Info($"尝试解析版本字符串: {versionString}");
            var result = _innerService.TryParse(versionString, out version);
            _logger.Info($"尝试解析结果: {result}, 版本: {version}");
            return result;
        }

        /// <summary>
        /// 检查版本字符串是否有效
        /// </summary>
        /// <param name="versionString">版本字符串</param>
        /// <returns>版本字符串是否有效</returns>
        public bool IsValidVersion(string versionString)
        {
            var result = _innerService.IsValidVersion(versionString);
            _logger.Info($"检查版本字符串 '{versionString}' 是否有效: {result}");
            return result;
        }

        /// <summary>
        /// 递增主版本号
        /// </summary>
        /// <param name="version">版本对象</param>
        /// <returns>新的版本对象</returns>
        public SemanticVersion IncrementMajor(SemanticVersion version)
        {
            _logger.Info($"递增主版本号: {version}");
            var result = _innerService.IncrementMajor(version);
            _logger.Info($"递增结果: {result}");
            return result;
        }

        /// <summary>
        /// 递增次版本号
        /// </summary>
        /// <param name="version">版本对象</param>
        /// <returns>新的版本对象</returns>
        public SemanticVersion IncrementMinor(SemanticVersion version)
        {
            _logger.Info($"递增次版本号: {version}");
            var result = _innerService.IncrementMinor(version);
            _logger.Info($"递增结果: {result}");
            return result;
        }

        /// <summary>
        /// 递增补丁版本号
        /// </summary>
        /// <param name="version">版本对象</param>
        /// <returns>新的版本对象</returns>
        public SemanticVersion IncrementPatch(SemanticVersion version)
        {
            _logger.Info($"递增补丁版本号: {version}");
            var result = _innerService.IncrementPatch(version);
            _logger.Info($"递增结果: {result}");
            return result;
        }

        /// <summary>
        /// 解析版本范围字符串
        /// </summary>
        /// <param name="rangeString">版本范围字符串</param>
        /// <returns>版本范围对象</returns>
        public VersionRange ParseRange(string rangeString)
        {
            _logger.Info($"解析版本范围字符串: {rangeString}");
            try
            {
                var result = _innerService.ParseRange(rangeString);
                _logger.Info($"解析结果: {result}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"解析版本范围字符串失败: {rangeString}", ex);
                throw;
            }
        }

        /// <summary>
        /// 检查版本是否在范围内
        /// </summary>
        /// <param name="version">版本对象</param>
        /// <param name="range">版本范围对象</param>
        /// <returns>版本是否在范围内</returns>
        public bool IsInRange(SemanticVersion version, VersionRange range)
        {
            var result = _innerService.IsInRange(version, range);
            _logger.Info($"检查版本 {version} 是否在范围 {range} 内: {result}");
            return result;
        }
    }

    /// <summary>
    /// 版本服务缓存装饰器
    /// </summary>
    public class CachingVersionServiceDecorator : IVersionServiceDecorator
    {
        private readonly IVersionService _innerService;
        private readonly Dictionary<string, SemanticVersion> _versionCache;
        private readonly Dictionary<string, VersionRange> _rangeCache;
        private readonly ILogger _logger;

        /// <summary>
        /// 初始化 <see cref="CachingVersionServiceDecorator"/> 类的新实例
        /// </summary>
        /// <param name="innerService">内部版本服务</param>
        /// <param name="logger">日志服务</param>
        public CachingVersionServiceDecorator(IVersionService innerService, ILogger logger)
        {
            _innerService = innerService ?? throw new ArgumentNullException(nameof(innerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _versionCache = new Dictionary<string, SemanticVersion>();
            _rangeCache = new Dictionary<string, VersionRange>();
        }

        /// <summary>
        /// 解析版本字符串
        /// </summary>
        /// <param name="versionString">版本字符串</param>
        /// <returns>语义化版本对象</returns>
        public SemanticVersion Parse(string versionString)
        {
            if (_versionCache.TryGetValue(versionString, out var cachedVersion))
            {
                _logger.Info($"从缓存获取版本: {versionString} -> {cachedVersion}");
                return cachedVersion;
            }

            var version = _innerService.Parse(versionString);
            _versionCache[versionString] = version;
            _logger.Info($"缓存版本: {versionString} -> {version}");
            return version;
        }

        /// <summary>
        /// 尝试解析版本字符串
        /// </summary>
        /// <param name="versionString">版本字符串</param>
        /// <param name="version">语义化版本对象</param>
        /// <returns>解析是否成功</returns>
        public bool TryParse(string versionString, out SemanticVersion version)
        {
            if (_versionCache.TryGetValue(versionString, out version))
            {
                _logger.Info($"从缓存获取版本: {versionString} -> {version}");
                return true;
            }

            var success = _innerService.TryParse(versionString, out version);
            if (success && version != null)
            {
                _versionCache[versionString] = version;
                _logger.Info($"缓存版本: {versionString} -> {version}");
            }
            return success;
        }

        /// <summary>
        /// 检查版本字符串是否有效
        /// </summary>
        /// <param name="versionString">版本字符串</param>
        /// <returns>版本字符串是否有效</returns>
        public bool IsValidVersion(string versionString)
        {
            return _innerService.IsValidVersion(versionString);
        }

        /// <summary>
        /// 递增主版本号
        /// </summary>
        /// <param name="version">版本对象</param>
        /// <returns>新的版本对象</returns>
        public SemanticVersion IncrementMajor(SemanticVersion version)
        {
            return _innerService.IncrementMajor(version);
        }

        /// <summary>
        /// 递增次版本号
        /// </summary>
        /// <param name="version">版本对象</param>
        /// <returns>新的版本对象</returns>
        public SemanticVersion IncrementMinor(SemanticVersion version)
        {
            return _innerService.IncrementMinor(version);
        }

        /// <summary>
        /// 递增补丁版本号
        /// </summary>
        /// <param name="version">版本对象</param>
        /// <returns>新的版本对象</returns>
        public SemanticVersion IncrementPatch(SemanticVersion version)
        {
            return _innerService.IncrementPatch(version);
        }

        /// <summary>
        /// 解析版本范围字符串
        /// </summary>
        /// <param name="rangeString">版本范围字符串</param>
        /// <returns>版本范围对象</returns>
        public VersionRange ParseRange(string rangeString)
        {
            if (_rangeCache.TryGetValue(rangeString, out var cachedRange))
            {
                _logger.Info($"从缓存获取版本范围: {rangeString} -> {cachedRange}");
                return cachedRange;
            }

            var range = _innerService.ParseRange(rangeString);
            _rangeCache[rangeString] = range;
            _logger.Info($"缓存版本范围: {rangeString} -> {range}");
            return range;
        }

        /// <summary>
        /// 检查版本是否在范围内
        /// </summary>
        /// <param name="version">版本对象</param>
        /// <param name="range">版本范围对象</param>
        /// <returns>版本是否在范围内</returns>
        public bool IsInRange(SemanticVersion version, VersionRange range)
        {
            return _innerService.IsInRange(version, range);
        }
    }

    /// <summary>
    /// 泛型服务接口
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    public interface IGenericService<T>
    {
        /// <summary>
        /// 获取服务名称
        /// </summary>
        /// <returns>服务名称</returns>
        string GetServiceName();

        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>处理结果</returns>
        string ProcessData(T data);
    }

    /// <summary>
    /// 泛型服务实现
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    public class GenericService<T> : IGenericService<T>
    {
        /// <summary>
        /// 获取服务名称
        /// </summary>
        /// <returns>服务名称</returns>
        public string GetServiceName()
        {
            return $"GenericService<{typeof(T).Name}>";
        }

        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>处理结果</returns>
        public string ProcessData(T data)
        {
            return $"Processed {typeof(T).Name}: {data}";
        }
    }

    /// <summary>
    /// 主程序类
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 主方法
        /// </summary>
        /// <param name="args">命令行参数</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Scrutor 用法演示");
            Console.WriteLine("==================");

            // 示例 1: 基本自动服务注册
            Console.WriteLine("\n1. 基本自动服务注册:");
            DemoBasicRegistration();

            // 示例 2: 装饰器模式
            Console.WriteLine("\n2. 装饰器模式:");
            DemoDecoratorPattern();

            // 示例 3: 泛型服务注册
            Console.WriteLine("\n3. 泛型服务注册:");
            DemoGenericRegistration();

            // 示例 4: 高级服务注册
            Console.WriteLine("\n4. 高级服务注册:");
            DemoAdvancedRegistration();

            Console.WriteLine("\n演示完成！");
        }

        /// <summary>
        /// 演示基本自动服务注册
        /// </summary>
        private static void DemoBasicRegistration()
        {
            var services = new ServiceCollection();

            // 使用 Scrutor 自动注册服务
            services.Scan(scan => scan
                .FromAssembliesOf(typeof(ILogger), typeof(IVersionService))
                .AddClasses(classes => classes.AssignableTo<ILogger>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo<IVersionService>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

            var serviceProvider = services.BuildServiceProvider();

            // 获取服务
            var logger = serviceProvider.GetRequiredService<ILogger>();
            var versionService = serviceProvider.GetRequiredService<IVersionService>();

            // 使用服务
            logger.Info("基本自动服务注册演示");
            var version = versionService.Parse("1.0.0");
            logger.Info($"解析版本: {version}");
        }

        /// <summary>
        /// 演示装饰器模式
        /// </summary>
        private static void DemoDecoratorPattern()
        {
            var services = new ServiceCollection();

            // 注册基础服务
            services.AddSingleton<ILogger, ConsoleLogger>();
            services.AddSingleton<IVersionService, VersionService>();

            // 使用 Scrutor 添加装饰器
            // 注意：装饰器的添加顺序很重要，后添加的装饰器会包装先添加的装饰器
            services.Decorate<IVersionService, CachingVersionServiceDecorator>();
            services.Decorate<IVersionService, LoggingVersionServiceDecorator>();

            var serviceProvider = services.BuildServiceProvider();

            // 获取服务（实际上是被装饰后的服务）
            var versionService = serviceProvider.GetRequiredService<IVersionService>();

            // 使用服务
            Console.WriteLine("使用装饰器模式解析版本:");
            var version1 = versionService.Parse("1.0.0");
            Console.WriteLine("再次解析相同版本（应该从缓存获取）:");
            var version2 = versionService.Parse("1.0.0");
            Console.WriteLine("解析版本范围:");
            var range = versionService.ParseRange(">=1.0.0 <2.0.0");
            Console.WriteLine("再次解析相同版本范围（应该从缓存获取）:");
            var range2 = versionService.ParseRange(">=1.0.0 <2.0.0");
        }

        /// <summary>
        /// 演示泛型服务注册
        /// </summary>
        private static void DemoGenericRegistration()
        {
            var services = new ServiceCollection();

            // 使用 Scrutor 注册泛型服务
            services.Scan(scan => scan
                .FromAssemblyOf(typeof(IGenericService<>))
                .AddClasses(classes => classes.AssignableTo(typeof(IGenericService<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            var serviceProvider = services.BuildServiceProvider();

            // 获取泛型服务
            var stringService = serviceProvider.GetRequiredService<IGenericService<string>>();
            var intService = serviceProvider.GetRequiredService<IGenericService<int>>();
            var boolService = serviceProvider.GetRequiredService<IGenericService<bool>>();

            // 使用服务
            Console.WriteLine($"String 服务名称: {stringService.GetServiceName()}");
            Console.WriteLine($"String 服务处理结果: {stringService.ProcessData("Hello, World!")}");
            Console.WriteLine($"Int 服务名称: {intService.GetServiceName()}");
            Console.WriteLine($"Int 服务处理结果: {intService.ProcessData(42)}");
            Console.WriteLine($"Bool 服务名称: {boolService.GetServiceName()}");
            Console.WriteLine($"Bool 服务处理结果: {boolService.ProcessData(true)}");
        }

        /// <summary>
        /// 演示高级服务注册
        /// </summary>
        private static void DemoAdvancedRegistration()
        {
            var services = new ServiceCollection();

            // 注册基础服务
            services.AddSingleton<ILogger, ConsoleLogger>();

            // 使用 Scrutor 高级注册
            services.Scan(scan => scan
                .FromAssembliesOf(typeof(IVersionService), typeof(IGenericService<>))
                // 注册版本服务
                .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                // 注册泛型服务
                .AddClasses(classes => classes.AssignableTo(typeof(IGenericService<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            // 添加装饰器
            services.Decorate<IVersionService, LoggingVersionServiceDecorator>();

            var serviceProvider = services.BuildServiceProvider();

            // 获取服务
            var versionService = serviceProvider.GetRequiredService<IVersionService>();
            var genericService = serviceProvider.GetRequiredService<IGenericService<string>>();

            // 使用服务
            Console.WriteLine("高级服务注册演示:");
            var version = versionService.Parse("2.0.0-alpha.1");
            Console.WriteLine($"解析预发布版本: {version}");
            var range = versionService.ParseRange("^1.0.0");
            Console.WriteLine($"解析插入符号范围: {range}");
            var isInRange = versionService.IsInRange(version, range);
            Console.WriteLine($"版本 {version} 是否在范围 {range} 内: {isInRange}");
            Console.WriteLine($"泛型服务处理结果: {genericService.ProcessData("Test data")}");
        }
    }
}
