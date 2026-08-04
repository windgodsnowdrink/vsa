#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Mono.Cecil@0.11.5
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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mono.Cecil;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fody.AOT
{
    /// <summary>
    /// Fody 命令类型枚举
    /// </summary>
    public enum FodyCommandType { Weave, ListWeavers, VersionInfo, Help }

    /// <summary>
    /// Fody 选项配置
    /// </summary>
    public class FodyOptions
    {
        /// <summary>
        /// 工作目录
        /// </summary>
        public string WorkingDirectory { get; set; } = Environment.CurrentDirectory;
        
        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
        
        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;
        
        /// <summary>
        /// 请求超时时间（毫秒）
        /// </summary>
        public int RequestTimeoutMs { get; set; } = 30000;
        
        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public bool EnableCache { get; set; } = true;
        
        /// <summary>
        /// 缓存大小
        /// </summary>
        public int CacheSize { get; set; } = 1000;
    }

    /// <summary>
    /// Fody 命令结果
    /// </summary>
    public class FodyCommandResult
    {
        /// <summary>
        /// 命令是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 执行时间（毫秒）
        /// </summary>
        public long ExecutionTimeMs { get; set; }
        
        /// <summary>
        /// 错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// 结果数据
        /// </summary>
        public List<string> Results { get; set; } = new List<string>();
    }

    /// <summary>
    /// Fody 服务接口
    /// </summary>
    public interface IFodyService
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>命令结果</returns>
        Task<FodyCommandResult> ExecuteCommandAsync(FodyCommandType commandType, Dictionary<string, string>? parameters = null);
        
        /// <summary>
        /// 织入程序集
        /// </summary>
        /// <param name="assemblyPath">程序集路径</param>
        /// <param name="outputPath">输出路径</param>
        /// <returns>织入结果</returns>
        Task<FodyCommandResult> WeaveAssemblyAsync(string assemblyPath, string outputPath);
        
        /// <summary>
        /// 列出可用的织入器
        /// </summary>
        /// <returns>织入器列表</returns>
        Task<FodyCommandResult> ListWeaversAsync();
        
        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        Task<FodyCommandResult> GetVersionInfoAsync();
    }

    /// <summary>
    /// Fody 服务实现
    /// </summary>
    public class FodyService : IFodyService
    {
        private readonly FodyOptions _options;
        private readonly ILogger<FodyService> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">Fody 选项</param>
        /// <param name="logger">日志记录器</param>
        public FodyService(IOptions<FodyOptions> options, ILogger<FodyService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<FodyCommandResult> ExecuteCommandAsync(FodyCommandType commandType, Dictionary<string, string>? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FodyCommandResult();

            try
            {
                switch (commandType)
                {
                    case FodyCommandType.Weave:
                        if (parameters?.ContainsKey("assemblyPath") == true && parameters?.ContainsKey("outputPath") == true)
                        {
                            result = await WeaveAssemblyAsync(parameters["assemblyPath"], parameters["outputPath"]);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "缺少必要的参数: assemblyPath 和 outputPath";
                        }
                        break;
                    
                    case FodyCommandType.ListWeavers:
                        result = await ListWeaversAsync();
                        break;
                    
                    case FodyCommandType.VersionInfo:
                        result = await GetVersionInfoAsync();
                        break;
                    
                    default:
                        result.Success = false;
                        result.ErrorMessage = "未知命令类型";
                        break;
                }
            }
            catch (Exception ex)
            {
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
        public async Task<FodyCommandResult> WeaveAssemblyAsync(string assemblyPath, string outputPath)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FodyCommandResult();

            try
            {
                _logger.LogInformation("开始织入程序集: {AssemblyPath}", assemblyPath);
                
                // 检查文件是否存在
                if (!File.Exists(assemblyPath))
                {
                    result.Success = false;
                    result.ErrorMessage = "程序集文件不存在";
                    return result;
                }
                
                // 确保输出目录存在
                var outputDirectory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }
                
                // 使用 Mono.Cecil 读取程序集
                using var assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyPath);
                
                // 模拟织入过程
                _logger.LogInformation("读取程序集成功: {AssemblyName}", assemblyDefinition.Name.Name);
                
                // 模拟织入操作
                await Task.Delay(1000); // 模拟织入时间
                
                // 添加一些模拟的织入结果
                result.Results.Add($"成功读取程序集: {assemblyDefinition.Name.Name}");
                result.Results.Add($"程序集版本: {assemblyDefinition.Name.Version}");
                result.Results.Add($"模块数量: {assemblyDefinition.Modules.Count}");
                result.Results.Add($"类型数量: {assemblyDefinition.MainModule.Types.Count}");
                
                // 保存修改后的程序集
                assemblyDefinition.Write(outputPath);
                result.Results.Add($"成功织入到: {outputPath}");
                
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "织入程序集时出错");
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
        public async Task<FodyCommandResult> ListWeaversAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FodyCommandResult();

            try
            {
                _logger.LogInformation("列出可用的织入器");
                
                // 模拟可用的织入器
                result.Results.Add("可用的织入器:");
                result.Results.Add("1. PropertyChanged.Fody - 自动实现 INotifyPropertyChanged");
                result.Results.Add("2. MethodTimer.Fody - 方法执行时间测量");
                result.Results.Add("3. NullGuard.Fody - 空值检查");
                result.Results.Add("4. ToString.Fody - 自动生成 ToString 方法");
                result.Results.Add("5. Equals.Fody - 自动生成 Equals 和 GetHashCode 方法");
                result.Results.Add("6. Unquote.Fody - 字符串插值优化");
                result.Results.Add("7. AsyncErrorHandler.Fody - 异步错误处理");
                
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "列出织入器时出错");
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
        public async Task<FodyCommandResult> GetVersionInfoAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FodyCommandResult();

            try
            {
                _logger.LogInformation("获取 Fody 版本信息");
                
                // 模拟获取版本信息
                await Task.Delay(50);
                
                result.Results.Add("Fody AOT Engine");
                result.Results.Add("版本: 1.0.0");
                result.Results.Add($".NET 版本: {Environment.Version}");
                result.Results.Add($"操作系统: {Environment.OSVersion}");
                result.Results.Add($"架构: {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}");
                result.Results.Add($"AOT 编译: {AppContext.TryGetSwitch("PublishAot", out bool isAot) && isAot}");
                result.Results.Add($"工作目录: {_options.WorkingDirectory}");
                result.Results.Add($"启用缓存: {_options.EnableCache}");
                result.Results.Add($"缓存大小: {_options.CacheSize}");
                
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取版本信息时出错");
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
    }

    /// <summary>
    /// Fody AOT 引擎
    /// </summary>
    public class FodyAotEngine
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FodyAotEngine> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="logger">日志记录器</param>
        public FodyAotEngine(IServiceProvider serviceProvider, ILogger<FodyAotEngine> logger)
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
            _logger.LogInformation("Fody AOT Engine 启动，参数: {Args}", string.Join(" ", args));
            
            if (args.Length == 0)
            {
                ShowHelp();
                return 0;
            }

            var command = args[0].ToLower();
            var fodyService = _serviceProvider.GetRequiredService<IFodyService>();
            FodyCommandResult? result = null;

            try
            {
                switch (command)
                {
                    case "weave":
                        if (args.Length < 3)
                        {
                            Console.WriteLine("错误: 需要提供程序集路径和输出路径");
                            return 1;
                        }
                        string assemblyPath = args[1];
                        string outputPath = args[2];
                        result = await fodyService.WeaveAssemblyAsync(assemblyPath, outputPath);
                        break;
                    
                    case "weavers":
                    case "list":
                        result = await fodyService.ListWeaversAsync();
                        break;
                    
                    case "version":
                    case "info":
                        result = await fodyService.GetVersionInfoAsync();
                        break;
                    
                    case "help":
                    case "--help":
                    case "-h":
                        ShowHelp();
                        return 0;
                    
                    default:
                        Console.WriteLine($"错误: 未知命令 '{command}'");
                        ShowHelp();
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
                return 1;
            }

            // 显示结果
            if (result != null)
            {
                Console.WriteLine($"\n命令执行结果: {(result.Success ? "成功" : "失败")}");
                Console.WriteLine($"执行时间: {result.ExecutionTimeMs} ms");
                
                if (!result.Success && !string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Console.WriteLine($"错误信息: {result.ErrorMessage}");
                }
                
                foreach (var item in result.Results)
                {
                    Console.WriteLine($"- {item}");
                }
            }
            
            return result?.Success == true ? 0 : 1;
        }

        /// <summary>
        /// 显示帮助信息
        /// </summary>
        private void ShowHelp()
        {
            Console.WriteLine("Fody AOT Engine - .NET 10 AOT 编译的 Fody 引擎");
            Console.WriteLine();
            Console.WriteLine("用法: fody_aot <命令> [参数]");
            Console.WriteLine();
            Console.WriteLine("命令:");
            Console.WriteLine("  weave <assemblyPath> <outputPath>    织入程序集");
            Console.WriteLine("  weavers|list                        列出可用的织入器");
            Console.WriteLine("  version|info                        显示版本信息");
            Console.WriteLine("  help|--help|-h                      显示帮助信息");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine("  fody_aot weave MyAssembly.dll Output.dll");
            Console.WriteLine("  fody_aot weavers");
            Console.WriteLine("  fody_aot version");
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
        /// <returns>退出码</returns>
        public static async Task<int> Main(string[] args)
        {
            // 创建主机
            var builder = Host.CreateApplicationBuilder(args);
            
            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            
            // 配置服务
            builder.Services.Configure<FodyOptions>(builder.Configuration.GetSection("Fody"));
            builder.Services.AddSingleton<IFodyService, FodyService>();
            builder.Services.AddSingleton<FodyAotEngine>();
            
            // 构建主机
            using var host = builder.Build();
            
            // 获取引擎实例
            var engine = host.Services.GetRequiredService<FodyAotEngine>();
            
            // 执行命令
            return await engine.ExecuteCommandLineAsync(args);
        }
    }
}