#:sdk Microsoft.NET.Sdk.Web
#:package Masuit.Tools@5.0.0
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Masuit.Tools;
using Masuit.Tools.AspNetCore.Extensions;
using Masuit.Tools.Core.Net;
using Masuit.Tools.DateTimeExt;
using Masuit.Tools.Files;
using Masuit.Tools.Logging;
using Masuit.Tools.Mime;
using Masuit.Tools.NoSQL;
using Masuit.Tools.Security;
using Masuit.Tools.Systems;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace MasuitToolsIntegration
{
    /// <summary>
    /// Masuit.Tools配置选项
    /// </summary>
    public class MasuitToolsOptions
    {
        public bool EnableRedisCache { get; set; } = false;
        public string RedisConnectionString { get; set; } = string.Empty;
        public bool EnableRSAEncryption { get; set; } = false;
        public string RSAPublicKey { get; set; } = string.Empty;
        public string RSAPrivateKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// Masuit.Tools服务接口
    /// </summary>
    public interface IMasuitToolsService
    {
        /// <summary>
        /// 获取Redis客户端
        /// </summary>
        RedisClient Redis { get; }

        /// <summary>
        /// 获取RSA加密服务
        /// </summary>
        RSACryptoService RSA { get; }

        /// <summary>
        /// 获取文件工具服务
        /// </summary>
        FileUtils FileUtils { get; }
    }

    /// <summary>
    /// Masuit.Tools服务实现
    /// </summary>
    public class MasuitToolsService : IMasuitToolsService
    {
        private readonly MasuitToolsOptions _options;
        private readonly ILogger<MasuitToolsService> _logger;
        private RedisClient _redisClient;
        private RSACryptoService _rsaService;

        public MasuitToolsService(IOptions<MasuitToolsOptions> options, ILogger<MasuitToolsService> logger)
        {
            _options = options.Value;
            _logger = logger;

            if (_options.EnableRedisCache && !string.IsNullOrEmpty(_options.RedisConnectionString))
            {
                _redisClient = new RedisClient(_options.RedisConnectionString);
                _logger.LogInformation("Redis client initialized");
            }

            if (_options.EnableRSAEncryption)
            {
                _rsaService = new RSACryptoService(_options.RSAPublicKey, _options.RSAPrivateKey);
                _logger.LogInformation("RSA crypto service initialized");
            }
        }

        public RedisClient Redis => _redisClient ?? throw new InvalidOperationException("Redis is not enabled");

        public RSACryptoService RSA => _rsaService ?? throw new InvalidOperationException("RSA is not enabled");

        public FileUtils FileUtils => new FileUtils();
    }

    /// <summary>
    /// DI扩展方法
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加Masuit.Tools服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddMasuitTools(this IServiceCollection services, Action<MasuitToolsOptions> configure)
        {
            services.Configure(configure);
            services.TryAddSingleton<IMasuitToolsService, MasuitToolsService>();
            
            // 注册常用工具类
            services.TryAddSingleton<DateTimeExtensions>();
            services.TryAddSingleton<MimeMapper>();
            services.TryAddSingleton<LogManager>();
            services.TryAddSingleton<SnowFlake>();
            services.TryAddSingleton<ObjectPool>();
            
            return services;
        }
    }

    /// <summary>
    /// 示例用法
    /// </summary>
    public static class ExampleUsage
    {
        public static void Demo()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            
            // 配置Masuit.Tools
            services.AddMasuitTools(options =>
            {
                options.EnableRedisCache = true;
                options.RedisConnectionString = "localhost:6379";
                options.EnableRSAEncryption = true;
                options.RSAPublicKey = "public_key_here";
                options.RSAPrivateKey = "private_key_here";
            });
            
            var provider = services.BuildServiceProvider();
            var masuitTools = provider.GetRequiredService<IMasuitToolsService>();
            
            // 使用Redis
            masuitTools.Redis.Set("key", "value");
            var value = masuitTools.Redis.Get<string>("key");
            
            // 使用RSA加密
            var encrypted = masuitTools.RSA.Encrypt("data to encrypt");
            var decrypted = masuitTools.RSA.Decrypt(encrypted);
            
            // 使用文件工具
            var fileInfo = masuitTools.FileUtils.GetFileInfo("path/to/file");
            
            // 使用其他工具类
            var dateUtils = provider.GetRequiredService<DateTimeExtensions>();
            var now = dateUtils.Now;
            
            var mimeMapper = provider.GetRequiredService<MimeMapper>();
            var mimeType = mimeMapper.GetMimeFromExtension(".txt");
        }
    }
}