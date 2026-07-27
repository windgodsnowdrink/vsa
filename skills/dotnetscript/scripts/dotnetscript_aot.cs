#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.CodeAnalysis.CSharp.Scripting@4.10.0
#:package Microsoft.CodeAnalysis.Common@4.10.0
#:package Microsoft.CodeAnalysis.CSharp@4.10.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotNetScript.AOT
{
    /// <summary>
    /// DotNetScript 命令类型枚举
    /// </summary>
    public enum DotNetScriptCommandType
    {
        /// <summary>
        /// 执行脚本
        /// </summary>
        ExecuteScript,
        /// <summary>
        /// 编译脚本
        /// </summary>
        CompileScript,
        /// <summary>
        /// 列出可用的脚本
        /// </summary>
        ListScripts,
        /// <summary>
        /// 清理编译缓存
        /// </summary>
        CleanCache,
        /// <summary>
        /// 显示版本信息
        /// </summary>
        VersionInfo
    }

    /// <summary>
    /// DotNetScript 选项配置
    /// </summary>
    public class DotNetScriptOptions
    {
        /// <summary>
        /// 脚本默认超时时间（毫秒）
        /// </summary>
        public int DefaultTimeoutMs { get; set; } = 30000;
        
        /// <summary>
        /// 是否启用编译缓存
        /// </summary>
        public bool EnableCompilationCache { get; set; } = true;
        
        /// <summary>
        /// 缓存目录
        /// </summary>
        public string CacheDirectory { get; set; } = Path.Combine(Path.GetTempPath(), "dotnetscript-aot-cache");
        
        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
        
        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;
        
        /// <summary>
        /// 默认脚本文件扩展名
        /// </summary>
        public string DefaultScriptExtension { get; set; } = ".csx";
        
        /// <summary>
        /// 支持的脚本文件扩展名列表
        /// </summary>
        public List<string> SupportedExtensions { get; set; } = new List<string> { ".csx", ".cs" };
    }

    /// <summary>
    /// DotNetScript 命令结果
    /// </summary>
    public class DotNetScriptCommandResult
    {
        /// <summary>
        /// 命令是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 命令类型
        /// </summary>
        public DotNetScriptCommandType CommandType { get; set; }
        
        /// <summary>
        /// 结果数据
        /// </summary>
        public List<string> Results { get; set; } = new List<string>();
        
        /// <summary>
        /// 执行时间（毫秒）
        /// </summary>
        public long ExecutionTimeMs { get; set; }
        
        /// <summary>
        /// 错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// 输出内容
        /// </summary>
        public string? Output { get; set; }
    }

    /// <summary>
    /// DotNetScript 服务接口
    /// </summary>
    public interface IDotNetScriptService
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>命令结果</returns>
        Task<DotNetScriptCommandResult> ExecuteCommandAsync(DotNetScriptCommandType commandType, Dictionary<string, string>? parameters = null);
        
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="scriptContent">脚本内容</param>
        /// <param name="scriptPath">脚本路径</param>
        /// <param name="timeoutMs">超时时间（毫秒）</param>
        /// <returns>脚本执行结果</returns>
        Task<DotNetScriptCommandResult> ExecuteScriptAsync(string scriptContent, string? scriptPath = null, int? timeoutMs = null);
        
        /// <summary>
        /// 从文件执行脚本
        /// </summary>
        /// <param name="scriptPath">脚本文件路径</param>
        /// <param name="timeoutMs">超时时间（毫秒）</param>
        /// <returns>脚本执行结果</returns>
        Task<DotNetScriptCommandResult> ExecuteScriptFromFileAsync(string scriptPath, int? timeoutMs = null);
        
        /// <summary>
        /// 编译脚本
        /// </summary>
        /// <param name="scriptContent">脚本内容</param>
        /// <param name="scriptPath">脚本路径</param>
        /// <returns>编译结果</returns>
        Task<DotNetScriptCommandResult> CompileScriptAsync(string scriptContent, string? scriptPath = null);
        
        /// <summary>
        /// 列出可用的脚本
        /// </summary>
        /// <param name="directory">目录路径</param>
        /// <param name="recursive">是否递归搜索</param>
        /// <returns>脚本列表</returns>
        Task<DotNetScriptCommandResult> ListScriptsAsync(string directory, bool recursive = false);
        
        /// <summary>
        /// 清理编译缓存
        /// </summary>
        /// <returns>清理结果</returns>
        Task<DotNetScriptCommandResult> CleanCacheAsync();
        
        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        Task<DotNetScriptCommandResult> GetVersionInfoAsync();
    }

    /// <summary>
    /// DotNetScript 服务实现
    /// </summary>
    public class DotNetScriptService : IDotNetScriptService
    {
        private readonly DotNetScriptOptions _options;
        private readonly ILogger<DotNetScriptService> _logger;
        private readonly ScriptOptions _scriptOptions;
        private readonly Dictionary<string, Assembly> _referencedAssemblies = new Dictionary<string, Assembly>();
        private readonly object _cacheLock = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">DotNetScript 选项</param>
        /// <param name="logger">日志记录器</param>
        public DotNetScriptService(IOptions<DotNetScriptOptions> options, ILogger<DotNetScriptService> logger)
        {
            _options = options.Value;
            _logger = logger;
            
            // 初始化脚本选项
            _scriptOptions = ScriptOptions.Default
                .AddReferences(Assembly.GetExecutingAssembly())
                .AddReferences(typeof(System.Collections.Generic.List<>).Assembly)
                .AddReferences(typeof(System.Threading.Tasks.Task<>).Assembly)
                .AddReferences(typeof(System.IO.File).Assembly)
                .AddReferences(typeof(System.Console).Assembly)
                .AddImports(
                    "System",
                    "System.Collections.Generic",
                    "System.Linq",
                    "System.IO",
                    "System.Threading.Tasks",
                    "System.Text"
                );
            
            // 确保缓存目录存在
            if (_options.EnableCompilationCache && !Directory.Exists(_options.CacheDirectory))
            {
                Directory.CreateDirectory(_options.CacheDirectory);
            }
        }

        /// <inheritdoc/>
        public async Task<DotNetScriptCommandResult> ExecuteCommandAsync(DotNetScriptCommandType commandType, Dictionary<string, string>? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DotNetScriptCommandResult
            {
                CommandType = commandType
            };

            try
            {
                switch (commandType)
                {
                    case DotNetScriptCommandType.ExecuteScript:
                        if (parameters?.ContainsKey("scriptContent") == true)
                        {
                            string scriptContent = parameters["scriptContent"];
                            string? scriptPath = parameters?.ContainsKey("scriptPath") == true ? parameters["scriptPath"] : null;
                            int? timeoutMs = parameters?.ContainsKey("timeoutMs") == true ? int.Parse(parameters["timeoutMs"]) : null;
                            result = await ExecuteScriptAsync(scriptContent, scriptPath, timeoutMs);
                        }
                        else if (parameters?.ContainsKey("scriptPath") == true)
                        {
                            string scriptPath = parameters["scriptPath"];
                            int? timeoutMs = parameters?.ContainsKey("timeoutMs") == true ? int.Parse(parameters["timeoutMs"]) : null;
                            result = await ExecuteScriptFromFileAsync(scriptPath, timeoutMs);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "Either scriptContent or scriptPath parameter is required";
                        }
                        break;
                    
                    case DotNetScriptCommandType.CompileScript:
                        if (parameters?.ContainsKey("scriptContent") == true)
                        {
                            string scriptContent = parameters["scriptContent"];
                            string? scriptPath = parameters?.ContainsKey("scriptPath") == true ? parameters["scriptPath"] : null;
                            result = await CompileScriptAsync(scriptContent, scriptPath);
                        }
                        else if (parameters?.ContainsKey("scriptPath") == true)
                        {
                            string scriptPath = parameters["scriptPath"];
                            string scriptContent = await File.ReadAllTextAsync(scriptPath);
                            result = await CompileScriptAsync(scriptContent, scriptPath);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "Either scriptContent or scriptPath parameter is required";
                        }
                        break;
                    
                    case DotNetScriptCommandType.ListScripts:
                        string directory = parameters?.ContainsKey("directory") == true ? parameters["directory"] : ".";
                        bool recursive = parameters?.ContainsKey("recursive") == true && bool.Parse(parameters["recursive"]);
                        result = await ListScriptsAsync(directory, recursive);
                        break;
                    
                    case DotNetScriptCommandType.CleanCache:
                        result = await CleanCacheAsync();
                        break;
                    
                    case DotNetScriptCommandType.VersionInfo:
                        result = await GetVersionInfoAsync();
                        break;
                    
                    default:
                        result.Success = false;
                        result.ErrorMessage = $"Unknown command type: {commandType}";
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing command: {CommandType}", commandType);
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<DotNetScriptCommandResult> ExecuteScriptAsync(string scriptContent, string? scriptPath = null, int? timeoutMs = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DotNetScriptCommandResult
            {
                CommandType = DotNetScriptCommandType.ExecuteScript
            };

            try
            {
                _logger.LogInformation("Executing script, path: {ScriptPath}", scriptPath);
                
                // 创建字符串编写器来捕获输出
                using var outputWriter = new StringWriter();
                var originalOutput = Console.Out;
                Console.SetOut(outputWriter);
                
                try
                {
                    // 执行脚本
                    var scriptState = await CSharpScript.RunAsync(
                        scriptContent,
                        _scriptOptions,
                        globals: null,
                        cancellationToken: new System.Threading.CancellationTokenSource(timeoutMs ?? _options.DefaultTimeoutMs).Token
                    );
                    
                    result.Success = true;
                    result.Output = outputWriter.ToString().Trim();
                    
                    if (scriptState.ReturnValue != null)
                    {
                        result.Results.Add(scriptState.ReturnValue.ToString() ?? string.Empty);
                    }
                }
                finally
                {
                    // 恢复原始输出
                    Console.SetOut(originalOutput);
                }
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                result.Success = false;
                result.ErrorMessage = $"Script execution timed out after {timeoutMs ?? _options.DefaultTimeoutMs} ms";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing script, path: {ScriptPath}", scriptPath);
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<DotNetScriptCommandResult> ExecuteScriptFromFileAsync(string scriptPath, int? timeoutMs = null)
        {
            if (!File.Exists(scriptPath))
            {
                return new DotNetScriptCommandResult
                {
                    CommandType = DotNetScriptCommandType.ExecuteScript,
                    Success = false,
                    ErrorMessage = $"Script file not found: {scriptPath}"
                };
            }
            
            string scriptContent = await File.ReadAllTextAsync(scriptPath);
            return await ExecuteScriptAsync(scriptContent, scriptPath, timeoutMs);
        }

        /// <inheritdoc/>
        public async Task<DotNetScriptCommandResult> CompileScriptAsync(string scriptContent, string? scriptPath = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DotNetScriptCommandResult
            {
                CommandType = DotNetScriptCommandType.CompileScript
            };

            try
            {
                _logger.LogInformation("Compiling script, path: {ScriptPath}", scriptPath);
                
                // 编译脚本但不执行
                var script = CSharpScript.Create(
                    scriptContent,
                    _scriptOptions,
                    globalsType: null
                );
                
                var compilation = script.GetCompilation();
                var diagnostics = compilation.GetDiagnostics();
                
                var errors = diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error).ToList();
                if (errors.Any())
                {
                    result.Success = false;
                    result.ErrorMessage = string.Join(Environment.NewLine, errors.Select(e => $"{e.Id}: {e.GetMessage()}"));
                }
                else
                {
                    result.Success = true;
                    result.Results.Add($"Script compiled successfully");
                    
                    var warnings = diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Warning).ToList();
                    if (warnings.Any())
                    {
                        result.Results.Add($"Warnings: {warnings.Count}");
                        if (_options.EnableDetailedLogging)
                        {
                            foreach (var warning in warnings)
                            {
                                result.Results.Add($"  {warning.Id}: {warning.GetMessage()}");
                            }
                        }
                    }
                    else
                    {
                        result.Results.Add("No warnings");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error compiling script, path: {ScriptPath}", scriptPath);
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<DotNetScriptCommandResult> ListScriptsAsync(string directory, bool recursive = false)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DotNetScriptCommandResult
            {
                CommandType = DotNetScriptCommandType.ListScripts
            };

            try
            {
                _logger.LogInformation("Listing scripts, directory: {Directory}, recursive: {Recursive}", directory, recursive);
                
                if (!Directory.Exists(directory))
                {
                    result.Success = false;
                    result.ErrorMessage = $"Directory not found: {directory}";
                    return result;
                }
                
                // 搜索脚本文件
                var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                var scripts = new List<string>();
                
                foreach (var extension in _options.SupportedExtensions)
                {
                    var files = Directory.GetFiles(directory, "*" + extension, searchOption);
                    scripts.AddRange(files);
                }
                
                // 按名称排序
                scripts.Sort();
                
                result.Success = true;
                result.Results.AddRange(scripts);
                result.Results.Insert(0, $"Found {scripts.Count} script(s):");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing scripts, directory: {Directory}", directory);
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<DotNetScriptCommandResult> CleanCacheAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DotNetScriptCommandResult
            {
                CommandType = DotNetScriptCommandType.CleanCache
            };

            try
            {
                _logger.LogInformation("Cleaning compilation cache, directory: {CacheDirectory}", _options.CacheDirectory);
                
                if (Directory.Exists(_options.CacheDirectory))
                {
                    Directory.Delete(_options.CacheDirectory, recursive: true);
                    Directory.CreateDirectory(_options.CacheDirectory);
                    result.Success = true;
                    result.Results.Add("Cache cleaned successfully");
                }
                else
                {
                    result.Success = true;
                    result.Results.Add("Cache directory does not exist, nothing to clean");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning cache");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<DotNetScriptCommandResult> GetVersionInfoAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DotNetScriptCommandResult
            {
                CommandType = DotNetScriptCommandType.VersionInfo
            };

            try
            {
                _logger.LogInformation("Getting version info");
                
                result.Success = true;
                result.Results.Add("DotNetScript AOT Engine");
                result.Results.Add($"Version: 1.0.0");
                result.Results.Add($".NET Version: {Environment.Version}");
                result.Results.Add($"OS: {Environment.OSVersion}");
                result.Results.Add($"Architecture: {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}");
                result.Results.Add($"AOT Compiled: {AppContext.TryGetSwitch("PublishAot", out bool isAot) && isAot}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting version info");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return await Task.FromResult(result);
        }
    }

    /// <summary>
    /// DotNetScript AOT 引擎
    /// </summary>
    public class DotNetScriptAotEngine
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DotNetScriptAotEngine> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="logger">日志记录器</param>
        public DotNetScriptAotEngine(IServiceProvider serviceProvider, ILogger<DotNetScriptAotEngine> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// 执行命令行操作
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出码</returns>
        public async Task<int> ExecuteCommandLineAsync(string[] args)
        {
            _logger.LogInformation("DotNetScript AOT Engine starting with args: {Args}", string.Join(" ", args));
            
            if (args.Length == 0)
            {
                ShowHelp();
                return 0;
            }

            var command = args[0].ToLower();
            var scriptService = _serviceProvider.GetRequiredService<IDotNetScriptService>();
            DotNetScriptCommandResult? result = null;

            try
            {
                switch (command)
                {
                    case "run":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Error: Script path or content is required");
                            return 1;
                        }
                        
                        string scriptPathOrContent = args[1];
                        int? timeoutMs = null;
                        
                        // 解析参数
                        for (int i = 2; i < args.Length; i++)
                        {
                            if (args[i].StartsWith("--timeout"))
                            {
                                if (i + 1 < args.Length && int.TryParse(args[i + 1], out int timeout))
                                {
                                    timeoutMs = timeout;
                                    i++;
                                }
                            }
                        }
                        
                        // 判断是文件路径还是脚本内容
                        if (File.Exists(scriptPathOrContent))
                        {
                            result = await scriptService.ExecuteScriptFromFileAsync(scriptPathOrContent, timeoutMs);
                        }
                        else
                        {
                            // 直接执行脚本内容
                            result = await scriptService.ExecuteScriptAsync(scriptPathOrContent, null, timeoutMs);
                        }
                        break;
                    
                    case "compile":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Error: Script path or content is required");
                            return 1;
                        }
                        
                        string compilePathOrContent = args[1];
                        if (File.Exists(compilePathOrContent))
                        {
                            string content = await File.ReadAllTextAsync(compilePathOrContent);
                            result = await scriptService.CompileScriptAsync(content, compilePathOrContent);
                        }
                        else
                        {
                            result = await scriptService.CompileScriptAsync(compilePathOrContent);
                        }
                        break;
                    
                    case "list":
                        string directory = args.Length > 1 ? args[1] : ".";
                        bool recursive = args.Contains("--recursive") || args.Contains("-r");
                        result = await scriptService.ListScriptsAsync(directory, recursive);
                        break;
                    
                    case "clean":
                        result = await scriptService.CleanCacheAsync();
                        break;
                    
                    case "version":
                        result = await scriptService.GetVersionInfoAsync();
                        break;
                    
                    case "help":
                    case "--help":
                    case "-h":
                        ShowHelp();
                        return 0;
                    
                    default:
                        Console.WriteLine($"Error: Unknown command '{command}'");
                        ShowHelp();
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                _logger.LogError(ex, "Error executing command: {Command}", command);
                return 1;
            }

            if (result != null)
            {
                DisplayResult(result);
                return result.Success ? 0 : 1;
            }

            return 0;
        }

        private void ShowHelp()
        {
            Console.WriteLine("DotNetScript AOT Command Line Tool");
            Console.WriteLine("=================================");
            Console.WriteLine("Usage: dotnetscript_aot <command> [options]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  run <script> [options]      Run a script file or inline script");
            Console.WriteLine("  compile <script> [options]   Compile a script without executing it");
            Console.WriteLine("  list [directory] [options]   List script files in directory");
            Console.WriteLine("  clean                       Clean compilation cache");
            Console.WriteLine("  version                     Show version information");
            Console.WriteLine("  help, --help, -h            Show this help message");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --timeout <ms>              Script execution timeout in milliseconds");
            Console.WriteLine("  --recursive, -r             Recursively list scripts");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnetscript_aot run script.csx              Run a script file");
            Console.WriteLine("  dotnetscript_aot run \"Console.WriteLine(123);\"  Run inline script");
            Console.WriteLine("  dotnetscript_aot compile script.csx         Compile a script");
            Console.WriteLine("  dotnetscript_aot list scripts/ -r           List all scripts recursively");
            Console.WriteLine("  dotnetscript_aot clean                      Clean cache");
            Console.WriteLine("  dotnetscript_aot version                   Show version");
        }

        private void DisplayResult(DotNetScriptCommandResult result)
        {
            Console.WriteLine($"Command: {result.CommandType}");
            Console.WriteLine($"Status: {(result.Success ? "Success" : "Failed")}");
            Console.WriteLine($"Time: {result.ExecutionTimeMs} ms");
            
            if (!string.IsNullOrEmpty(result.Output))
            {
                Console.WriteLine();
                Console.WriteLine("Output:");
                Console.WriteLine(result.Output);
            }
            
            if (!result.Success && !string.IsNullOrEmpty(result.ErrorMessage))
            {
                Console.WriteLine();
                Console.WriteLine($"Error: {result.ErrorMessage}");
            }
            else if (result.Results.Any())
            {
                Console.WriteLine();
                Console.WriteLine("Results:");
                foreach (var item in result.Results)
                {
                    Console.WriteLine($"  {item}");
                }
            }
        }
    }
}

// 扩展方法
public static class DotNetScriptServiceExtensions
{
    /// <summary>
    /// 添加 DotNetScript 服务到依赖注入容器
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDotNetScript(this IServiceCollection services)
    {
        services.AddSingleton<DotNetScript.AOT.IDotNetScriptService, DotNetScript.AOT.DotNetScriptService>();
        services.AddSingleton<DotNetScript.AOT.DotNetScriptAotEngine>();
        return services;
    }
}

// 主程序
class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("dotnetscript_aot.setting.json", optional: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<DotNetScript.AOT.DotNetScriptOptions>(context.Configuration.GetSection("DotNetScript"));
                services.AddDotNetScript();
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .Build();

        var engine = host.Services.GetRequiredService<DotNetScript.AOT.DotNetScriptAotEngine>();
        var exitCode = await engine.ExecuteCommandLineAsync(args);
        Environment.Exit(exitCode);
    }
}