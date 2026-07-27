#:sdk Microsoft.NET.Sdk
#:package Scrutor@4.2.0
#:package System.CommandLine@2.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Microsoft.Extensions.Logging@8.0.0
#:package Microsoft.Extensions.Caching.Memory@8.0.0
#:package System.IO.Abstractions@17.0.21
#:package Newtonsoft.Json@13.0.3
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property SelfContained=true
#:property PublishSingleFile=true

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System.IO.Abstractions;
using Newtonsoft.Json;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TailwindCSS
{
    // 核心接口
    public interface ITailwindService
    {
        Task<string> BuildAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default);
        Task WatchAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default);
        Task InitializeAsync(string configPath = null, CancellationToken cancellationToken = default);
        Task<string> PurgeAsync(string inputPath, string outputPath, IEnumerable<string> contentPaths, CancellationToken cancellationToken = default);
        Task<string> OptimizeAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default);
    }

    public interface ITailwindThemeService
    {
        IEnumerable<string> GetThemes();
        ThemeConfiguration GetTheme(string name);
        void AddTheme(string name, ThemeConfiguration configuration);
        void SetActiveTheme(string name);
        ThemeConfiguration GetActiveTheme();
    }

    public interface ITailwindPluginService
    {
        Task LoadPluginsAsync(CancellationToken cancellationToken = default);
        IEnumerable<IPlugin> GetPlugins();
        void AddPlugin(IPlugin plugin);
        void RemovePlugin(string name);
        Task ExecutePluginsAsync(string input, CancellationToken cancellationToken = default);
    }

    public interface IBuildStep
    {
        string Name { get; }
        Task<string> ExecuteAsync(string input, BuildContext context, CancellationToken cancellationToken = default);
    }

    public interface IPlugin
    {
        string Name { get; }
        Task ExecuteAsync(string input, CancellationToken cancellationToken = default);
        void Configure(PluginConfiguration configuration);
    }

    // 配置类
    public class TailwindConfiguration
    {
        public ThemeConfiguration Theme { get; set; } = new ThemeConfiguration();
        public List<string> Content { get; set; } = new List<string> { "**/*.html", "**/*.cshtml", "**/*.razor", "**/*.js", "**/*.ts" };
        public List<string> Plugins { get; set; } = new List<string>();
        public bool Purge { get; set; } = false;
        public bool Minify { get; set; } = true;
        public string Separator { get; set; } = ":";
    }

    public class ThemeConfiguration
    {
        public Dictionary<string, Dictionary<string, string>> Colors { get; set; } = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<string, List<string>> FontFamily { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, string> Spacing { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> FontSize { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> BorderRadius { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> BoxShadow { get; set; } = new Dictionary<string, string>();
    }

    public class PluginConfiguration
    {
        public Dictionary<string, object> Settings { get; set; } = new Dictionary<string, object>();
    }

    public class BuildContext
    {
        public TailwindConfiguration Configuration { get; set; }
        public IFileSystem FileSystem { get; set; }
        public IMemoryCache Cache { get; set; }
        public ILogger Logger { get; set; }
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
    }

    // 实现类
    public class TailwindService : ITailwindService
    {
        private readonly ITailwindThemeService _themeService;
        private readonly ITailwindPluginService _pluginService;
        private readonly IFileSystem _fileSystem;
        private readonly IMemoryCache _cache;
        private readonly ILogger<TailwindService> _logger;
        private readonly IEnumerable<IBuildStep> _buildSteps;

        public TailwindService(
            ITailwindThemeService themeService,
            ITailwindPluginService pluginService,
            IFileSystem fileSystem,
            IMemoryCache cache,
            ILogger<TailwindService> logger,
            IEnumerable<IBuildStep> buildSteps)
        {
            _themeService = themeService;
            _pluginService = pluginService;
            _fileSystem = fileSystem;
            _cache = cache;
            _logger = logger;
            _buildSteps = buildSteps;
        }

        public async Task<string> BuildAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Building TailwindCSS from {Input} to {Output}", inputPath, outputPath);

            // 检查文件存在
            if (!_fileSystem.File.Exists(inputPath))
            {
                throw new FileNotFoundException($"Input file not found: {inputPath}");
            }

            // 创建输出目录
            var outputDir = _fileSystem.Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outputDir) && !_fileSystem.Directory.Exists(outputDir))
            {
                _fileSystem.Directory.CreateDirectory(outputDir);
            }

            // 读取配置
            var config = await LoadConfigurationAsync(cancellationToken);

            // 构建上下文
            var context = new BuildContext
            {
                Configuration = config,
                FileSystem = _fileSystem,
                Cache = _cache,
                Logger = _logger,
                InputPath = inputPath,
                OutputPath = outputPath
            };

            // 读取输入文件
            var inputContent = _fileSystem.File.ReadAllText(inputPath, Encoding.UTF8);

            // 执行构建步骤
            var result = inputContent;
            foreach (var step in _buildSteps)
            {
                _logger.LogDebug("Executing build step: {Step}", step.Name);
                result = await step.ExecuteAsync(result, context, cancellationToken);
            }

            // 执行插件
            await _pluginService.ExecutePluginsAsync(result, cancellationToken);

            // 写入输出文件
            _fileSystem.File.WriteAllText(outputPath, result, Encoding.UTF8);

            _logger.LogInformation("Build completed successfully");
            return result;
        }

        public async Task WatchAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting watch mode for {Input} to {Output}", inputPath, outputPath);

            var inputDir = _fileSystem.Path.GetDirectoryName(inputPath);
            if (string.IsNullOrEmpty(inputDir))
            {
                inputDir = _fileSystem.Directory.GetCurrentDirectory();
            }

            // 初始构建
            await BuildAsync(inputPath, outputPath, cancellationToken);

            // 监视文件变化
            using var watcher = new FileSystemWatcher(inputDir)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Filter = "*.css",
                IncludeSubdirectories = true
            };

            var changeTaskCompletionSource = new TaskCompletionSource<bool>();
            watcher.Changed += async (sender, e) =>
            {
                if (e.FullPath.Equals(inputPath, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("File changed: {File}", e.FullPath);
                    try
                    {
                        await BuildAsync(inputPath, outputPath, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error during watch build");
                    }
                }
            };

            watcher.Error += (sender, e) =>
            {
                _logger.LogError(e.GetException(), "FileSystemWatcher error");
                changeTaskCompletionSource.TrySetResult(false);
            };

            watcher.EnableRaisingEvents = true;
            _logger.LogInformation("Watch mode started. Press Ctrl+C to exit.");

            try
            {
                await Task.WhenAny(changeTaskCompletionSource.Task, Task.Delay(Timeout.Infinite, cancellationToken));
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Watch mode cancelled");
            }
        }

        public async Task InitializeAsync(string configPath = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Initializing TailwindCSS configuration");

            if (string.IsNullOrEmpty(configPath))
            {
                configPath = _fileSystem.Path.Combine(_fileSystem.Directory.GetCurrentDirectory(), "tailwind.config.json");
            }

            // 创建默认配置
            var defaultConfig = new TailwindConfiguration
            {
                Theme = new ThemeConfiguration
                {
                    Colors = new Dictionary<string, Dictionary<string, string>>
                    {
                        { "primary", new Dictionary<string, string>
                            {
                                { "50", "#eff6ff" },
                                { "100", "#dbeafe" },
                                { "200", "#bfdbfe" },
                                { "300", "#93c5fd" },
                                { "400", "#60a5fa" },
                                { "500", "#3b82f6" },
                                { "600", "#2563eb" },
                                { "700", "#1d4ed8" },
                                { "800", "#1e40af" },
                                { "900", "#1e3a8a" }
                            }
                        }
                    },
                    FontFamily = new Dictionary<string, List<string>>
                    {
                        { "sans", new List<string> { "Inter", "system-ui", "sans-serif" } },
                        { "mono", new List<string> { "Fira Code", "monospace" } }
                    }
                }
            };

            // 写入配置文件
            var configJson = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
            _fileSystem.File.WriteAllText(configPath, configJson, Encoding.UTF8);

            _logger.LogInformation("Configuration initialized at {Path}", configPath);
        }

        public async Task<string> PurgeAsync(string inputPath, string outputPath, IEnumerable<string> contentPaths, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Purging unused CSS from {Input} to {Output}", inputPath, outputPath);

            // 执行构建
            var cssContent = await BuildAsync(inputPath, outputPath, cancellationToken);

            // 收集使用的类
            var usedClasses = new HashSet<string>();
            foreach (var contentPath in contentPaths)
            {
                var files = _fileSystem.Directory.GetFiles(_fileSystem.Directory.GetCurrentDirectory(), contentPath, SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var content = _fileSystem.File.ReadAllText(file, Encoding.UTF8);
                    // 简单的类名提取
                    var matches = System.Text.RegularExpressions.Regex.Matches(content, @"\b[a-z][a-z0-9-]*\b");
                    foreach (var match in matches)
                    {
                        usedClasses.Add(match.ToString());
                    }
                }
            }

            // 清理未使用的 CSS
            var purgedContent = PurgeCss(cssContent, usedClasses);

            // 写入输出文件
            _fileSystem.File.WriteAllText(outputPath, purgedContent, Encoding.UTF8);

            _logger.LogInformation("CSS purged successfully");
            return purgedContent;
        }

        public async Task<string> OptimizeAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Optimizing CSS from {Input} to {Output}", inputPath, outputPath);

            // 执行构建
            var cssContent = await BuildAsync(inputPath, outputPath, cancellationToken);

            // 优化 CSS
            var optimizedContent = OptimizeCss(cssContent);

            // 写入输出文件
            _fileSystem.File.WriteAllText(outputPath, optimizedContent, Encoding.UTF8);

            _logger.LogInformation("CSS optimized successfully");
            return optimizedContent;
        }

        private async Task<TailwindConfiguration> LoadConfigurationAsync(CancellationToken cancellationToken = default)
        {
            var configPath = _fileSystem.Path.Combine(_fileSystem.Directory.GetCurrentDirectory(), "tailwind.config.json");
            if (_fileSystem.File.Exists(configPath))
            {
                _logger.LogDebug("Loading configuration from {Path}", configPath);
                var configJson = _fileSystem.File.ReadAllText(configPath, Encoding.UTF8);
                return JsonConvert.DeserializeObject<TailwindConfiguration>(configJson);
            }

            _logger.LogDebug("Configuration file not found, using default configuration");
            return new TailwindConfiguration();
        }

        private string PurgeCss(string cssContent, HashSet<string> usedClasses)
        {
            // 简单的 CSS 清理实现
            var lines = cssContent.Split('\n');
            var result = new StringBuilder();
            var inUsedRule = false;

            foreach (var line in lines)
            {
                if (line.Trim().StartsWith('.'))
                {
                    var selector = line.Trim().Split('{')[0].Trim();
                    var className = selector.Substring(1); // 移除点号
                    inUsedRule = usedClasses.Contains(className);
                }

                if (inUsedRule || !line.Trim().StartsWith('.'))
                {
                    result.AppendLine(line);
                }
            }

            return result.ToString();
        }

        private string OptimizeCss(string cssContent)
        {
            // 简单的 CSS 优化实现
            return cssContent
                .Replace("  ", " ")
                .Replace("\n", "")
                .Replace("}", "}\n")
                .Trim();
        }
    }

    public class TailwindThemeService : ITailwindThemeService
    {
        private readonly Dictionary<string, ThemeConfiguration> _themes = new Dictionary<string, ThemeConfiguration>();
        private string _activeTheme = "default";

        public TailwindThemeService()
        {
            // 添加默认主题
            _themes["default"] = new ThemeConfiguration
            {
                Colors = new Dictionary<string, Dictionary<string, string>>
                {
                    { "primary", new Dictionary<string, string>
                        {
                            { "50", "#eff6ff" },
                            { "100", "#dbeafe" },
                            { "200", "#bfdbfe" },
                            { "300", "#93c5fd" },
                            { "400", "#60a5fa" },
                            { "500", "#3b82f6" },
                            { "600", "#2563eb" },
                            { "700", "#1d4ed8" },
                            { "800", "#1e40af" },
                            { "900", "#1e3a8a" }
                        }
                    },
                    { "gray", new Dictionary<string, string>
                        {
                            { "50", "#f9fafb" },
                            { "100", "#f3f4f6" },
                            { "200", "#e5e7eb" },
                            { "300", "#d1d5db" },
                            { "400", "#9ca3af" },
                            { "500", "#6b7280" },
                            { "600", "#4b5563" },
                            { "700", "#374151" },
                            { "800", "#1f2937" },
                            { "900", "#111827" }
                        }
                    }
                },
                FontFamily = new Dictionary<string, List<string>>
                {
                    { "sans", new List<string> { "Inter", "system-ui", "sans-serif" } },
                    { "serif", new List<string> { "Georgia", "Cambria", "serif" } },
                    { "mono", new List<string> { "Fira Code", "Consolas", "monospace" } }
                },
                Spacing = new Dictionary<string, string>
                {
                    { "0", "0" },
                    { "1", "0.25rem" },
                    { "2", "0.5rem" },
                    { "3", "0.75rem" },
                    { "4", "1rem" },
                    { "5", "1.25rem" },
                    { "6", "1.5rem" },
                    { "8", "2rem" },
                    { "10", "2.5rem" },
                    { "12", "3rem" },
                    { "16", "4rem" },
                    { "20", "5rem" },
                    { "24", "6rem" },
                    { "32", "8rem" }
                }
            };
        }

        public IEnumerable<string> GetThemes()
        {
            return _themes.Keys;
        }

        public ThemeConfiguration GetTheme(string name)
        {
            if (_themes.TryGetValue(name, out var theme))
            {
                return theme;
            }
            throw new KeyNotFoundException($"Theme not found: {name}");
        }

        public void AddTheme(string name, ThemeConfiguration configuration)
        {
            _themes[name] = configuration;
        }

        public void SetActiveTheme(string name)
        {
            if (_themes.ContainsKey(name))
            {
                _activeTheme = name;
            }
            else
            {
                throw new KeyNotFoundException($"Theme not found: {name}");
            }
        }

        public ThemeConfiguration GetActiveTheme()
        {
            return GetTheme(_activeTheme);
        }
    }

    public class TailwindPluginService : ITailwindPluginService
    {
        private readonly List<IPlugin> _plugins = new List<IPlugin>();
        private readonly ILogger<TailwindPluginService> _logger;

        public TailwindPluginService(ILogger<TailwindPluginService> logger)
        {
            _logger = logger;
        }

        public async Task LoadPluginsAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Loading plugins");

            // 加载内置插件
            _plugins.Add(new DefaultPlugin());

            // 可以在这里加载外部插件
        }

        public IEnumerable<IPlugin> GetPlugins()
        {
            return _plugins;
        }

        public void AddPlugin(IPlugin plugin)
        {
            _plugins.Add(plugin);
        }

        public void RemovePlugin(string name)
        {
            _plugins.RemoveAll(p => p.Name == name);
        }

        public async Task ExecutePluginsAsync(string input, CancellationToken cancellationToken = default)
        {
            foreach (var plugin in _plugins)
            {
                _logger.LogDebug("Executing plugin: {Plugin}", plugin.Name);
                await plugin.ExecuteAsync(input, cancellationToken);
            }
        }
    }

    // 构建步骤
    public class ParseDirectivesStep : IBuildStep
    {
        public string Name => "ParseDirectives";

        public Task<string> ExecuteAsync(string input, BuildContext context, CancellationToken cancellationToken = default)
        {
            // 解析 @tailwind 指令
            var result = input
                .Replace("@tailwind base;", GenerateBaseStyles())
                .Replace("@tailwind components;", GenerateComponentsStyles())
                .Replace("@tailwind utilities;", GenerateUtilitiesStyles());

            return Task.FromResult(result);
        }

        private string GenerateBaseStyles()
        {
            return "/* Base styles */\nbody { margin: 0; font-family: system-ui, sans-serif; }\n";
        }

        private string GenerateComponentsStyles()
        {
            return "/* Components styles */\n";
        }

        private string GenerateUtilitiesStyles()
        {
            return "/* Utilities styles */\n";
        }
    }

    public class ProcessLayersStep : IBuildStep
    {
        public string Name => "ProcessLayers";

        public Task<string> ExecuteAsync(string input, BuildContext context, CancellationToken cancellationToken = default)
        {
            // 处理 @layer 指令
            var result = input;
            // 简单的层处理
            return Task.FromResult(result);
        }
    }

    public class ApplyDirectivesStep : IBuildStep
    {
        public string Name => "ApplyDirectives";

        public Task<string> ExecuteAsync(string input, BuildContext context, CancellationToken cancellationToken = default)
        {
            // 处理 @apply 指令
            var result = input;
            // 简单的 @apply 处理
            return Task.FromResult(result);
        }
    }

    // 插件
    public class DefaultPlugin : IPlugin
    {
        public string Name => "DefaultPlugin";

        public Task ExecuteAsync(string input, CancellationToken cancellationToken = default)
        {
            // 默认插件逻辑
            return Task.CompletedTask;
        }

        public void Configure(PluginConfiguration configuration)
        {
            // 插件配置
        }
    }

    // 装饰器
    public class TailwindServiceLoggingDecorator : ITailwindService
    {
        private readonly ITailwindService _inner;
        private readonly ILogger<TailwindServiceLoggingDecorator> _logger;

        public TailwindServiceLoggingDecorator(ITailwindService inner, ILogger<TailwindServiceLoggingDecorator> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task<string> BuildAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[Decorator] Building TailwindCSS");
            var result = await _inner.BuildAsync(inputPath, outputPath, cancellationToken);
            _logger.LogInformation("[Decorator] Build completed");
            return result;
        }

        public async Task WatchAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[Decorator] Starting watch mode");
            await _inner.WatchAsync(inputPath, outputPath, cancellationToken);
            _logger.LogInformation("[Decorator] Watch mode stopped");
        }

        public async Task InitializeAsync(string configPath = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[Decorator] Initializing configuration");
            await _inner.InitializeAsync(configPath, cancellationToken);
            _logger.LogInformation("[Decorator] Configuration initialized");
        }

        public async Task<string> PurgeAsync(string inputPath, string outputPath, IEnumerable<string> contentPaths, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[Decorator] Purging CSS");
            var result = await _inner.PurgeAsync(inputPath, outputPath, contentPaths, cancellationToken);
            _logger.LogInformation("[Decorator] CSS purged");
            return result;
        }

        public async Task<string> OptimizeAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[Decorator] Optimizing CSS");
            var result = await _inner.OptimizeAsync(inputPath, outputPath, cancellationToken);
            _logger.LogInformation("[Decorator] CSS optimized");
            return result;
        }
    }

    public class TailwindServiceCachingDecorator : ITailwindService
    {
        private readonly ITailwindService _inner;
        private readonly IMemoryCache _cache;

        public TailwindServiceCachingDecorator(ITailwindService inner, IMemoryCache cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public async Task<string> BuildAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"build:{inputPath}:{outputPath}";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                return await _inner.BuildAsync(inputPath, outputPath, cancellationToken);
            });
        }

        public async Task WatchAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default)
        {
            // 监视模式不使用缓存
            await _inner.WatchAsync(inputPath, outputPath, cancellationToken);
        }

        public async Task InitializeAsync(string configPath = null, CancellationToken cancellationToken = default)
        {
            // 初始化不使用缓存
            await _inner.InitializeAsync(configPath, cancellationToken);
        }

        public async Task<string> PurgeAsync(string inputPath, string outputPath, IEnumerable<string> contentPaths, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"purge:{inputPath}:{outputPath}:{string.Join(',', contentPaths)}";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                return await _inner.PurgeAsync(inputPath, outputPath, contentPaths, cancellationToken);
            });
        }

        public async Task<string> OptimizeAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"optimize:{inputPath}:{outputPath}";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                return await _inner.OptimizeAsync(inputPath, outputPath, cancellationToken);
            });
        }
    }

    // 主程序
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            // 设置依赖注入
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();

            // 创建命令行根命令
            var rootCommand = new RootCommand("TailwindCSS skill for .NET");

            // 构建命令
            var buildCommand = new Command("build", "Build TailwindCSS")
            {
                new Argument<string>("input", "Input CSS file path"),
                new Argument<string>("output", "Output CSS file path")
            };
            buildCommand.Handler = CommandHandler.Create<string, string, CancellationToken>(async (input, output, cancellationToken) =>
            {
                var tailwindService = serviceProvider.GetRequiredService<ITailwindService>();
                await tailwindService.BuildAsync(input, output, cancellationToken);
                return 0;
            });

            // 监视命令
            var watchCommand = new Command("watch", "Watch for changes and build")
            {
                new Argument<string>("input", "Input CSS file path"),
                new Argument<string>("output", "Output CSS file path")
            };
            watchCommand.Handler = CommandHandler.Create<string, string, CancellationToken>(async (input, output, cancellationToken) =>
            {
                var tailwindService = serviceProvider.GetRequiredService<ITailwindService>();
                await tailwindService.WatchAsync(input, output, cancellationToken);
                return 0;
            });

            // 初始化命令
            var initCommand = new Command("init", "Initialize TailwindCSS configuration")
            {
                new Option<string>("--output", "Output configuration file path")
            };
            initCommand.Handler = CommandHandler.Create<string, CancellationToken>(async (output, cancellationToken) =>
            {
                var tailwindService = serviceProvider.GetRequiredService<ITailwindService>();
                await tailwindService.InitializeAsync(output, cancellationToken);
                return 0;
            });

            // 清理命令
            var purgeCommand = new Command("purge", "Purge unused CSS")
            {
                new Argument<string>("input", "Input CSS file path"),
                new Argument<string>("output", "Output CSS file path"),
                new Option<IEnumerable<string>>("--content", "Content files to scan for used classes")
            };
            purgeCommand.Handler = CommandHandler.Create<string, string, IEnumerable<string>, CancellationToken>(async (input, output, content, cancellationToken) =>
            {
                var tailwindService = serviceProvider.GetRequiredService<ITailwindService>();
                if (content == null)
                {
                    content = new List<string> { "**/*.html", "**/*.cshtml", "**/*.razor", "**/*.js" };
                }
                await tailwindService.PurgeAsync(input, output, content, cancellationToken);
                return 0;
            });

            // 优化命令
            var optimizeCommand = new Command("optimize", "Optimize CSS")
            {
                new Argument<string>("input", "Input CSS file path"),
                new Argument<string>("output", "Output CSS file path")
            };
            optimizeCommand.Handler = CommandHandler.Create<string, string, CancellationToken>(async (input, output, cancellationToken) =>
            {
                var tailwindService = serviceProvider.GetRequiredService<ITailwindService>();
                await tailwindService.OptimizeAsync(input, output, cancellationToken);
                return 0;
            });

            // 添加命令到根命令
            rootCommand.AddCommand(buildCommand);
            rootCommand.AddCommand(watchCommand);
            rootCommand.AddCommand(initCommand);
            rootCommand.AddCommand(purgeCommand);
            rootCommand.AddCommand(optimizeCommand);

            // 执行命令
            return await rootCommand.InvokeAsync(args);
        }

        private static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            // 添加日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // 添加缓存
            services.AddMemoryCache(options =>
            {
                options.SizeLimit = 1024 * 1024 * 100; // 100MB
                options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
            });

            // 添加文件系统
            services.AddSingleton<IFileSystem, FileSystem>();

            // 添加主题服务
            services.AddSingleton<ITailwindThemeService, TailwindThemeService>();

            // 添加插件服务
            services.AddSingleton<ITailwindPluginService, TailwindPluginService>();

            // 添加构建步骤
            services.AddTransient<IBuildStep, ParseDirectivesStep>();
            services.AddTransient<IBuildStep, ProcessLayersStep>();
            services.AddTransient<IBuildStep, ApplyDirectivesStep>();

            // 添加核心服务
            services.AddTransient<ITailwindService, TailwindService>();

            // 添加装饰器
            services.Decorate<ITailwindService, TailwindServiceLoggingDecorator>();
            services.Decorate<ITailwindService, TailwindServiceCachingDecorator>();

            return services;
        }
    }
}
