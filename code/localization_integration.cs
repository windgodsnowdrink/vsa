#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.Localization@8.0.0
#:package Westwind.Globalization@7.5.0
#:package Microsoft.Extensions.Caching.Memory@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System;
using System.Globalization;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.ObjectPool;
using Westwind.Globalization;

namespace LocalizationIntegration
{
    public interface ILocalizationService
    {
        string GetString(string name, CultureInfo culture = null);
        void SetCurrentCulture(CultureInfo culture);
        CultureInfo GetCurrentCulture();
        Task LoadJsonResourcesAsync(string cultureName = null); // 新增方法
    }

    public class LocalizationService : ILocalizationService
    {
        private readonly ObjectPool<DbResourceManager> _resourceManagerPool;
        private readonly IMemoryCache _memoryCache;
        private readonly ThreadLocal<CultureInfo> _currentCulture;
        private readonly DbResourceConfiguration _config;
        private readonly ConcurrentDictionary<string, Dictionary<string, string>> _jsonResources;
        private readonly IWebHostEnvironment _env;

        public LocalizationService(
            ObjectPool<DbResourceManager> resourceManagerPool,
            IMemoryCache memoryCache,
            DbResourceConfiguration config,
            IWebHostEnvironment env)
        {
            _resourceManagerPool = resourceManagerPool;
            _memoryCache = memoryCache;
            _config = config;
            _currentCulture = new ThreadLocal<CultureInfo>(() => CultureInfo.CurrentCulture);
            _env = env;
            _jsonResources = new ConcurrentDictionary<string, Dictionary<string, string>>();
        }

        public async Task LoadJsonResourcesAsync(string cultureName = null)
        {
            cultureName ??= _currentCulture.Value.Name;
            var jsonPath = Path.Combine(_env.ContentRootPath, "Resources", $"{cultureName}.json");

            if (!File.Exists(jsonPath))
                return;

            var json = await File.ReadAllTextAsync(jsonPath);
            
            // 使用XPath表达式获取JSON值
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            
            // 方式1：直接通过属性名获取
            var resources = new Dictionary<string, string>();
            foreach (var property in root.EnumerateObject())
            {
                resources[property.Name] = property.Value.GetString();
            }
            
            // 方式2：使用XPath表达式获取嵌套值
            var xpath = "//*[contains(name(),'Greeting')]";
            var greetingElement = root.SelectElement(xpath);
            if (greetingElement != null)
            {
                resources["Greeting"] = greetingElement.GetString();
            }
            
            // 方式3：使用Element获取数组值
            if (root.TryGetProperty("Messages", out var messagesElement))
            {
                foreach (var message in messagesElement.EnumerateArray())
                {
                    if (message.TryGetProperty("Key", out var keyElement) && 
                        message.TryGetProperty("Value", out var valueElement))
                    {
                        resources[keyElement.GetString()] = valueElement.GetString();
                    }
                }
            }
            
            _jsonResources.AddOrUpdate(cultureName, resources, (k, v) => resources);
        }

        public string GetString(string name, CultureInfo culture = null)
        {
            culture ??= _currentCulture.Value;
            
            // 首先尝试从JSON资源获取
            if (_jsonResources.TryGetValue(culture.Name, out var resources) && 
                resources.TryGetValue(name, out var jsonValue))
            {
                return jsonValue;
            }

            // 然后尝试从内存缓存获取
            var cacheKey = $"{name}:{culture.Name}";
            if (_memoryCache.TryGetValue(cacheKey, out string cachedValue))
                return cachedValue;

            // 最后从数据库资源获取
            var resourceManager = _resourceManagerPool.Get();
            try
            {
                var value = resourceManager.GetObject(name, culture) as string;
                _memoryCache.Set(cacheKey, value, new MemoryCacheEntryOptions
                {
                    Size = 1,
                    SlidingExpiration = TimeSpan.FromHours(1),
                    Priority = CacheItemPriority.High
                });
                return value;
            }
            finally
            {
                _resourceManagerPool.Return(resourceManager);
            }
        }

        public void SetCurrentCulture(CultureInfo culture)
        {
            _currentCulture.Value = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        public CultureInfo GetCurrentCulture() => _currentCulture.Value;
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalizationServices(this IServiceCollection services, Action<DbResourceConfiguration> configure = null)
        {
            var config = new DbResourceConfiguration
            {
                ConnectionString = "Server=.;Database=LocalizationDB;Trusted_Connection=True;",
                ResourceTableName = "Localizations",
                ResourceSetColumnName = "ResourceSet",
                AddMissingResources = true,
                ResxExportProjectType = ResxExportProjectType.WebForms
            };
            
            configure?.Invoke(config);
            
            services.AddSingleton(config);
            
            services.AddSingleton<ObjectPool<DbResourceManager>>(sp =>
            {
                var policy = new DefaultPooledObjectPolicy<DbResourceManager>();
                return new DefaultObjectPool<DbResourceManager>(policy, Environment.ProcessorCount * 2);
            });
            
            services.AddMemoryCache(options =>
            {
                options.SizeLimit = 1024 * 1024 * 100; // 100MB
                options.CompactionPercentage = 0.5;
            });
            
            services.AddSingleton<ILocalizationService, LocalizationService>();
            
            // 添加ASP.NET Core本地化服务
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            
            return services;
        }
    }
}

// 在Program.cs中注册服务
builder.Services.AddLocalizationServices(config => 
{
    config.ConnectionString = builder.Configuration.GetConnectionString("LocalizationDB");
});

// 在控制器或服务中使用
var localizationService = serviceProvider.GetRequiredService<ILocalizationService>();

// 设置当前文化
localizationService.SetCurrentCulture(new CultureInfo("zh-CN"));

// 获取本地化字符串
var greeting = localizationService.GetString("Greeting");

// 获取当前文化
var currentCulture = localizationService.GetCurrentCulture();

// JSON资源文件监听服务
public class JsonResourceWatcher : BackgroundService
{
    private readonly ILocalizationService _localizationService;
    private readonly IWebHostEnvironment _env;
    private FileSystemWatcher _watcher;

    public JsonResourceWatcher(ILocalizationService localizationService, IWebHostEnvironment env)
    {
        _localizationService = localizationService;
        _env = env;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var resourcesPath = Path.Combine(_env.ContentRootPath, "Resources");
        Directory.CreateDirectory(resourcesPath);

        _watcher = new FileSystemWatcher(resourcesPath, "*.json")
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
            EnableRaisingEvents = true
        };

        _watcher.Changed += async (sender, e) => 
        {
            var cultureName = Path.GetFileNameWithoutExtension(e.Name);
            await _localizationService.LoadJsonResourcesAsync(cultureName);
        };

        // 初始加载所有JSON资源
        foreach (var file in Directory.GetFiles(resourcesPath, "*.json"))
        {
            var cultureName = Path.GetFileNameWithoutExtension(file);
            await _localizationService.LoadJsonResourcesAsync(cultureName);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}

/*
{
  "Greeting": "你好",
  "Welcome": "欢迎"
}

// 获取本地化字符串 - 会优先从JSON文件读取
var greeting = localizationService.GetString("Greeting");

// 手动重新加载JSON资源
await localizationService.LoadJsonResourcesAsync("zh-CN");
*/