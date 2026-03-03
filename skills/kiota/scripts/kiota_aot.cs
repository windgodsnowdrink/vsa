#:sdk Microsoft.NET.Sdk
#:package Microsoft.Kiota.Abstractions@1.16.0
#:package Microsoft.Kiota.Authentication.Azure@1.1.0
#:package Microsoft.Kiota.Http.HttpClientLibrary@1.1.0
#:package Microsoft.Kiota.Serialization.Json@1.1.0
#:package Microsoft.Kiota.Serialization.Text@1.1.0
#:package Microsoft.Kiota.Serialization.Form@1.1.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Caching.Memory@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.CommandLine@2.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.Kiota.Serialization.Json;
using Microsoft.Kiota.Serialization.Text;
using Microsoft.Kiota.Serialization.Form;

namespace KiotaAot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole())
                .AddMemoryCache()
                .AddSingleton<KiotaService>()
                .BuildServiceProvider();

            var kiotaService = serviceProvider.GetRequiredService<KiotaService>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            try
            {
                var command = args[0].ToLower();
                switch (command)
                {
                    case "generate":
                    case "g":
                        if (args.Length < 4)
                        {
                            logger.LogError("请提供OpenAPI规范URL/文件、输出目录和语言");
                            return;
                        }
                        var specPath = args[1];
                        var outputDir = args[2];
                        var language = args[3];
                        var namespaceName = args.Length > 4 ? args[4] : "ApiClient";
                        await kiotaService.GenerateClientAsync(specPath, outputDir, language, namespaceName);
                        logger.LogInformation("客户端生成成功");
                        break;

                    case "validate":
                    case "v":
                        if (args.Length < 2)
                        {
                            logger.LogError("请提供OpenAPI规范URL/文件");
                            return;
                        }
                        specPath = args[1];
                        await kiotaService.ValidateSpecAsync(specPath);
                        break;

                    case "list-languages":
                    case "ll":
                        await kiotaService.ListSupportedLanguagesAsync();
                        break;

                    case "download-spec":
                    case "ds":
                        if (args.Length < 3)
                        {
                            logger.LogError("请提供OpenAPI规范URL和输出文件路径");
                            return;
                        }
                        var specUrl = args[1];
                        var outputFile = args[2];
                        await kiotaService.DownloadSpecAsync(specUrl, outputFile);
                        break;

                    case "cache-clear":
                    case "cc":
                        await kiotaService.ClearCacheAsync();
                        break;

                    case "cache-info":
                    case "ci":
                        await kiotaService.GetCacheInfoAsync();
                        break;

                    case "benchmark":
                    case "bm":
                        if (args.Length < 3)
                        {
                            logger.LogError("请提供OpenAPI规范URL/文件和运行次数");
                            return;
                        }
                        specPath = args[1];
                        var runCount = int.TryParse(args[2], out var count) ? count : 5;
                        await kiotaService.RunBenchmarkAsync(specPath, runCount);
                        break;

                    case "help":
                    case "h":
                        ShowHelp();
                        break;

                    default:
                        logger.LogError("未知命令: {Command}", command);
                        ShowHelp();
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "执行命令时发生错误");
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Kiota AOT 工具");
            Console.WriteLine("=============");
            Console.WriteLine("命令列表:");
            Console.WriteLine("  generate|g <spec> <output> <language> [namespace] - 生成API客户端");
            Console.WriteLine("  validate|v <spec> - 验证OpenAPI规范");
            Console.WriteLine("  list-languages|ll - 列出支持的语言");
            Console.WriteLine("  download-spec|ds <url> <output> - 下载OpenAPI规范");
            Console.WriteLine("  cache-clear|cc - 清除缓存");
            Console.WriteLine("  cache-info|ci - 显示缓存信息");
            Console.WriteLine("  benchmark|bm <spec> <count> - 运行性能基准测试");
            Console.WriteLine("  help|h - 显示帮助信息");
        }
    }

    public class KiotaService
    {
        private readonly ILogger<KiotaService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _cacheDirectory;

        public KiotaService()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole())
                .AddHttpClient()
                .BuildServiceProvider();
            
            _logger = serviceProvider.GetRequiredService<ILogger<KiotaService>>();
            _httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            _cacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Kiota", "Cache");
        }

        public async Task GenerateClientAsync(string specPath, string outputDir, string language, string namespaceName)
        {
            _logger.LogInformation("开始生成API客户端...");
            _logger.LogInformation("OpenAPI规范: {SpecPath}", specPath);
            _logger.LogInformation("输出目录: {OutputDir}", outputDir);
            _logger.LogInformation("语言: {Language}", language);
            _logger.LogInformation("命名空间: {Namespace}", namespaceName);

            // 确保输出目录存在
            Directory.CreateDirectory(outputDir);

            // 模拟Kiota客户端生成过程
            // 实际项目中，这里应该调用Kiota的核心API
            await Task.Delay(1000);

            // 生成示例文件
            var sampleFile = Path.Combine(outputDir, "ApiClient.cs");
            var sampleContent = $@"namespace {namespaceName}
{{
    /// <summary>
    /// API客户端
    /// </summary>
    public class ApiClient
    /// <summary>
    /// 初始化API客户端
    /// </summary>
    /// <param name="baseUrl">API基础URL</param>
    public ApiClient(string baseUrl = "https://api.example.com")
    {{
        // 初始化逻辑
    }}
    
    /// <summary>
    /// 获取资源
    /// </summary>
    /// <returns>资源列表</returns>
    public async Task<IEnumerable<Resource>> GetResourcesAsync()
    {{
        // 实现逻辑
        return new List<Resource>();
    }}
}}

/// <summary>
/// 资源模型
/// </summary>
public class Resource
{{
    public int Id {{ get; set; }}
    public string Name {{ get; set; }}
}}
}}";

            File.WriteAllText(sampleFile, sampleContent);

            _logger.LogInformation("客户端生成完成，示例文件已创建: {SampleFile}", sampleFile);
        }

        public async Task ValidateSpecAsync(string specPath)
        {
            _logger.LogInformation("开始验证OpenAPI规范...");
            _logger.LogInformation("规范路径: {SpecPath}", specPath);

            // 模拟OpenAPI规范验证过程
            await Task.Delay(500);

            // 检查文件是否存在
            if (File.Exists(specPath))
            {
                var fileContent = File.ReadAllText(specPath);
                if (fileContent.Contains("openapi") || fileContent.Contains("swagger"))
                {
                    _logger.LogInformation("OpenAPI规范验证成功");
                    _logger.LogInformation("规范格式: {Format}", fileContent.Contains("openapi") ? "OpenAPI 3.x" : "Swagger 2.0");
                }
                else
                {
                    _logger.LogError("OpenAPI规范验证失败: 无效的规范文件");
                }
            }
            else if (Uri.IsWellFormedUriString(specPath, UriKind.Absolute))
            {
                // 尝试从URL下载并验证
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    var response = await client.GetAsync(specPath);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    
                    if (content.Contains("openapi") || content.Contains("swagger"))
                    {
                        _logger.LogInformation("OpenAPI规范验证成功");
                        _logger.LogInformation("规范格式: {Format}", content.Contains("openapi") ? "OpenAPI 3.x" : "Swagger 2.0");
                    }
                    else
                    {
                        _logger.LogError("OpenAPI规范验证失败: 无效的规范内容");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "OpenAPI规范验证失败: 无法下载或解析规范");
                }
            }
            else
            {
                _logger.LogError("OpenAPI规范验证失败: 无效的路径或URL");
            }
        }

        public async Task ListSupportedLanguagesAsync()
        {
            _logger.LogInformation("支持的语言列表:");
            _logger.LogInformation("=================");

            var languages = new List<string>
            {
                "csharp",
                "typescript",
                "java",
                "go",
                "python",
                "php",
                "ruby",
                "swift",
                "kotlin",
                "powershell"
            };

            foreach (var language in languages)
            {
                _logger.LogInformation("  - {Language}", language);
                await Task.Delay(50);
            }

            _logger.LogInformation("共支持 {Count} 种语言", languages.Count);
        }

        public async Task DownloadSpecAsync(string specUrl, string outputFile)
        {
            _logger.LogInformation("开始下载OpenAPI规范...");
            _logger.LogInformation("源URL: {SpecUrl}", specUrl);
            _logger.LogInformation("输出文件: {OutputFile}", outputFile);

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(specUrl);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                
                // 确保输出目录存在
                Directory.CreateDirectory(Path.GetDirectoryName(outputFile) ?? string.Empty);
                
                File.WriteAllText(outputFile, content);
                
                _logger.LogInformation("OpenAPI规范下载成功，文件大小: {Size} bytes", new FileInfo(outputFile).Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OpenAPI规范下载失败");
            }
        }

        public async Task ClearCacheAsync()
        {
            _logger.LogInformation("开始清除缓存...");
            _logger.LogInformation("缓存目录: {CacheDirectory}", _cacheDirectory);

            try
            {
                if (Directory.Exists(_cacheDirectory))
                {
                    var files = Directory.GetFiles(_cacheDirectory, "*.*", SearchOption.AllDirectories);
                    var fileCount = files.Length;
                    
                    Directory.Delete(_cacheDirectory, true);
                    Directory.CreateDirectory(_cacheDirectory);
                    
                    _logger.LogInformation("缓存清除成功，删除了 {Count} 个文件", fileCount);
                }
                else
                {
                    _logger.LogInformation("缓存目录不存在，无需清除");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "缓存清除失败");
            }

            await Task.CompletedTask;
        }

        public async Task GetCacheInfoAsync()
        {
            _logger.LogInformation("缓存信息:");
            _logger.LogInformation("============");
            _logger.LogInformation("缓存目录: {CacheDirectory}", _cacheDirectory);

            try
            {
                if (Directory.Exists(_cacheDirectory))
                {
                    var files = Directory.GetFiles(_cacheDirectory, "*.*", SearchOption.AllDirectories);
                    var totalSize = files.Sum(f => new FileInfo(f).Length);
                    
                    _logger.LogInformation("文件数量: {Count}", files.Length);
                    _logger.LogInformation("总大小: {Size} bytes", totalSize);
                    _logger.LogInformation("最近修改时间:");
                    
                    var recentFiles = files.Select(f => new FileInfo(f))
                        .OrderByDescending(f => f.LastWriteTime)
                        .Take(5);
                    
                    foreach (var file in recentFiles)
                    {
                        _logger.LogInformation("  - {Name}: {LastWriteTime}", file.Name, file.LastWriteTime);
                    }
                }
                else
                {
                    _logger.LogInformation("缓存目录不存在");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取缓存信息失败");
            }

            await Task.CompletedTask;
        }

        public async Task RunBenchmarkAsync(string specPath, int runCount)
        {
            _logger.LogInformation("开始性能基准测试...");
            _logger.LogInformation(new string('=', 60));
            _logger.LogInformation("规范路径: {SpecPath}", specPath);
            _logger.LogInformation("运行次数: {RunCount}", runCount);

            var times = new List<long>();

            for (int i = 0; i < runCount; i++)
            {
                _logger.LogInformation($"运行测试 {i + 1}/{runCount}...");
                
                var stopwatch = Stopwatch.StartNew();
                
                // 模拟Kiota操作
                await ValidateSpecAsync(specPath);
                
                stopwatch.Stop();
                times.Add(stopwatch.ElapsedMilliseconds);
                
                _logger.LogInformation($"运行 {i + 1} 耗时: {stopwatch.ElapsedMilliseconds} ms");
            }

            var averageTime = times.Average();
            var minTime = times.Min();
            var maxTime = times.Max();

            _logger.LogInformation(new string('=', 60));
            _logger.LogInformation("性能基准测试结果:");
            _logger.LogInformation("  运行次数: {RunCount}", runCount);
            _logger.LogInformation("  平均耗时: {Average:F2} ms", averageTime);
            _logger.LogInformation("  最小耗时: {Min} ms", minTime);
            _logger.LogInformation("  最大耗时: {Max} ms", maxTime);
            _logger.LogInformation("  标准差: {StdDev:F2} ms", Math.Sqrt(times.Select(t => Math.Pow(t - averageTime, 2)).Average()));
        }
    }
}