#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true

using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CppSharp.AOT
{
    /// <summary>
    /// CppSharp AOT配置选项
    /// </summary>
    public class CppSharpOptions
    {
        /// <summary>
        /// 输入C++文件路径
        /// </summary>
        public string InputPath { get; set; } = string.Empty;

        /// <summary>
        /// 输出C#文件路径
        /// </summary>
        public string OutputPath { get; set; } = string.Empty;

        /// <summary>
        /// 是否生成调试信息
        /// </summary>
        public bool GenerateDebugInfo { get; set; } = false;

        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespace { get; set; } = "CppSharp.Generated";

        /// <summary>
        /// 是否生成异步包装器
        /// </summary>
        public bool GenerateAsyncWrappers { get; set; } = true;

        /// <summary>
        /// 是否生成事件包装器
        /// </summary>
        public bool GenerateEventWrappers { get; set; } = true;
    }

    /// <summary>
    /// C++类型信息
    /// </summary>
    public class CppTypeInfo
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 类型别名
        /// </summary>
        public string Alias { get; set; } = string.Empty;

        /// <summary>
        /// 是否为结构体
        /// </summary>
        public bool IsStruct { get; set; } = false;

        /// <summary>
        /// 是否为类
        /// </summary>
        public bool IsClass { get; set; } = false;

        /// <summary>
        /// 是否为枚举
        /// </summary>
        public bool IsEnum { get; set; } = false;

        /// <summary>
        /// 是否为接口
        /// </summary>
        public bool IsInterface { get; set; } = false;

        /// <summary>
        /// 成员列表
        /// </summary>
        public List<CppMemberInfo> Members { get; set; } = new List<CppMemberInfo>();
    }

    /// <summary>
    /// C++成员信息
    /// </summary>
    public class CppMemberInfo
    {
        /// <summary>
        /// 成员名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 成员类型
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 是否为方法
        /// </summary>
        public bool IsMethod { get; set; } = false;

        /// <summary>
        /// 是否为属性
        /// </summary>
        public bool IsProperty { get; set; } = false;

        /// <summary>
        /// 是否为字段
        /// </summary>
        public bool IsField { get; set; } = false;

        /// <summary>
        /// 参数列表
        /// </summary>
        public List<CppParameterInfo> Parameters { get; set; } = new List<CppParameterInfo>();
    }

    /// <summary>
    /// C++参数信息
    /// </summary>
    public class CppParameterInfo
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 参数类型
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 是否为输出参数
        /// </summary>
        public bool IsOut { get; set; } = false;

        /// <summary>
        /// 是否为引用参数
        /// </summary>
        public bool IsRef { get; set; } = false;
    }

    /// <summary>
    /// CppSharp服务接口
    /// 定义了C++和C#交互的核心功能
    /// </summary>
    public interface ICppSharpService
    {
        /// <summary>
        /// 解析C++头文件
        /// </summary>
        /// <param name="headerPath">头文件路径</param>
        /// <returns>解析结果</returns>
        Task<List<CppTypeInfo>> ParseHeaderAsync(string headerPath);

        /// <summary>
        /// 生成C#绑定代码
        /// </summary>
        /// <param name="typeInfos">类型信息列表</param>
        /// <param name="namespaceName">命名空间</param>
        /// <returns>生成的C#代码</returns>
        Task<string> GenerateBindingCodeAsync(List<CppTypeInfo> typeInfos, string namespaceName);

        /// <summary>
        /// 保存生成的代码到文件
        /// </summary>
        /// <param name="code">生成的代码</param>
        /// <param name="outputPath">输出文件路径</param>
        /// <returns>保存结果</returns>
        Task<bool> SaveGeneratedCodeAsync(string code, string outputPath);

        /// <summary>
        /// 执行完整的绑定生成流程
        /// </summary>
        /// <param name="inputPath">输入头文件路径</param>
        /// <param name="outputPath">输出C#文件路径</param>
        /// <param name="options">生成选项</param>
        /// <returns>生成结果</returns>
        Task<bool> GenerateBindingsAsync(string inputPath, string outputPath, CppSharpOptions options = null);
    }

    /// <summary>
    /// CppSharp服务实现
    /// 基于.NET 10 AOT架构，提供高性能C++和C#交互功能
    /// </summary>
    public class CppSharpService : ICppSharpService
    {
        private readonly ILogger<CppSharpService> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        public CppSharpService(ILogger<CppSharpService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 解析C++头文件
        /// </summary>
        public async Task<List<CppTypeInfo>> ParseHeaderAsync(string headerPath)
        {
            try
            {
                _logger.LogInformation($"开始解析C++头文件: {headerPath}");
                
                // 检查文件是否存在
                if (!File.Exists(headerPath))
                {
                    _logger.LogError($"头文件不存在: {headerPath}");
                    return new List<CppTypeInfo>();
                }

                // 读取文件内容
                var content = await File.ReadAllTextAsync(headerPath);
                
                // 这里是简化的解析逻辑，实际项目中会使用更复杂的解析器
                var typeInfos = new List<CppTypeInfo>();
                
                // 示例：添加一些模拟的类型信息
                typeInfos.Add(new CppTypeInfo
                {
                    Name = "MyClass",
                    IsClass = true,
                    Members = new List<CppMemberInfo>
                    {
                        new CppMemberInfo
                        {
                            Name = "DoSomething",
                            Type = "void",
                            IsMethod = true,
                            Parameters = new List<CppParameterInfo>
                            {
                                new CppParameterInfo { Name = "param1", Type = "int" },
                                new CppParameterInfo { Name = "param2", Type = "const char*" }
                            }
                        },
                        new CppMemberInfo
                        {
                            Name = "GetValue",
                            Type = "int",
                            IsMethod = true,
                            Parameters = new List<CppParameterInfo>()
                        },
                        new CppMemberInfo
                        {
                            Name = "Value",
                            Type = "int",
                            IsProperty = true
                        }
                    }
                });

                typeInfos.Add(new CppTypeInfo
                {
                    Name = "MyStruct",
                    IsStruct = true,
                    Members = new List<CppMemberInfo>
                    {
                        new CppMemberInfo { Name = "x", Type = "float", IsField = true },
                        new CppMemberInfo { Name = "y", Type = "float", IsField = true }
                    }
                });

                typeInfos.Add(new CppTypeInfo
                {
                    Name = "MyEnum",
                    IsEnum = true,
                    Members = new List<CppMemberInfo>
                    {
                        new CppMemberInfo { Name = "Value1", Type = "0" },
                        new CppMemberInfo { Name = "Value2", Type = "1" },
                        new CppMemberInfo { Name = "Value3", Type = "2" }
                    }
                });

                _logger.LogInformation($"解析完成，找到 {typeInfos.Count} 个类型");
                return typeInfos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"解析C++头文件失败: {headerPath}");
                return new List<CppTypeInfo>();
            }
        }

        /// <summary>
        /// 生成C#绑定代码
        /// </summary>
        public async Task<string> GenerateBindingCodeAsync(List<CppTypeInfo> typeInfos, string namespaceName)
        {
            try
            {
                _logger.LogInformation($"开始生成C#绑定代码，命名空间: {namespaceName}");
                
                var sb = new System.Text.StringBuilder();
                
                // 生成命名空间
                sb.AppendLine($"namespace {namespaceName}");
                sb.AppendLine("{");
                sb.AppendLine();

                // 生成类型绑定
                foreach (var typeInfo in typeInfos)
                {
                    // 生成注释
                    sb.AppendLine($"    /// <summary>");
                    sb.AppendLine($"    /// {typeInfo.Name} 的C#绑定");
                    sb.AppendLine($"    /// </summary>");
                    
                    // 生成类型定义
                    if (typeInfo.IsClass)
                    {
                        sb.AppendLine($"    public class {typeInfo.Name}");
                    }
                    else if (typeInfo.IsStruct)
                    {
                        sb.AppendLine($"    public struct {typeInfo.Name}");
                    }
                    else if (typeInfo.IsEnum)
                    {
                        sb.AppendLine($"    public enum {typeInfo.Name}");
                    }
                    else if (typeInfo.IsInterface)
                    {
                        sb.AppendLine($"    public interface {typeInfo.Name}");
                    }
                    
                    sb.AppendLine("    {");
                    
                    // 生成成员
                    foreach (var member in typeInfo.Members)
                    {
                        sb.AppendLine();
                        sb.AppendLine($"        /// <summary>");
                        sb.AppendLine($"        /// {member.Name} 的C#绑定");
                        sb.AppendLine($"        /// </summary>");
                        
                        if (typeInfo.IsEnum)
                        {
                            // 枚举成员
                            sb.AppendLine($"        {member.Name} = {member.Type},");
                        }
                        else if (member.IsField)
                        {
                            // 字段
                            sb.AppendLine($"        public {member.Type} {member.Name};");
                        }
                        else if (member.IsProperty)
                        {
                            // 属性
                            sb.AppendLine($"        public {member.Type} {member.Name} {{ get; set; }}");
                        }
                        else if (member.IsMethod)
                        {
                            // 方法
                            var paramStr = string.Join(", ", member.Parameters.Select(p => $"{p.Type} {p.Name}"));
                            sb.AppendLine($"        public {member.Type} {member.Name}({paramStr})");
                            sb.AppendLine("        {");
                            sb.AppendLine("            // TODO: 实现C++方法调用");
                            sb.AppendLine("        }");
                        }
                    }
                    
                    sb.AppendLine("    }");
                    sb.AppendLine();
                }
                
                // 关闭命名空间
                sb.AppendLine("}");
                
                _logger.LogInformation("C#绑定代码生成完成");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成C#绑定代码失败");
                return string.Empty;
            }
        }

        /// <summary>
        /// 保存生成的代码到文件
        /// </summary>
        public async Task<bool> SaveGeneratedCodeAsync(string code, string outputPath)
        {
            try
            {
                _logger.LogInformation($"保存生成的代码到文件: {outputPath}");
                
                // 确保输出目录存在
                var outputDir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }
                
                // 写入文件
                await File.WriteAllTextAsync(outputPath, code);
                
                _logger.LogInformation("代码保存成功");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"保存代码失败: {outputPath}");
                return false;
            }
        }

        /// <summary>
        /// 执行完整的绑定生成流程
        /// </summary>
        public async Task<bool> GenerateBindingsAsync(string inputPath, string outputPath, CppSharpOptions options = null)
        {
            try
            {
                _logger.LogInformation($"开始执行完整的绑定生成流程");
                
                // 使用默认选项如果未提供
                options ??= new CppSharpOptions();
                
                // 解析头文件
                var typeInfos = await ParseHeaderAsync(inputPath);
                if (typeInfos.Count == 0)
                {
                    _logger.LogError("解析头文件失败，未找到任何类型");
                    return false;
                }
                
                // 生成绑定代码
                var code = await GenerateBindingCodeAsync(typeInfos, options.Namespace);
                if (string.IsNullOrEmpty(code))
                {
                    _logger.LogError("生成绑定代码失败");
                    return false;
                }
                
                // 保存生成的代码
                var result = await SaveGeneratedCodeAsync(code, outputPath);
                
                if (result)
                {
                    _logger.LogInformation("绑定生成流程执行完成");
                }
                else
                {
                    _logger.LogError("绑定生成流程执行失败");
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "执行绑定生成流程时发生错误");
                return false;
            }
        }
    }

    /// <summary>
    /// CppSharp AOT执行引擎
    /// 管理C++和C#交互的执行
    /// </summary>
    public class CppSharpAotEngine
    {
        private readonly ILogger<CppSharpAotEngine> _logger;
        private readonly ICppSharpService _cppSharpService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="cppSharpService">CppSharp服务</param>
        public CppSharpAotEngine(ILogger<CppSharpAotEngine> logger, ICppSharpService cppSharpService)
        {
            _logger = logger;
            _cppSharpService = cppSharpService;
        }

        /// <summary>
        /// 执行绑定生成
        /// </summary>
        /// <param name="inputPath">输入头文件路径</param>
        /// <param name="outputPath">输出C#文件路径</param>
        /// <param name="options">生成选项</param>
        /// <returns>执行结果</returns>
        public async Task<bool> ExecuteGenerateBindingsAsync(string inputPath, string outputPath, CppSharpOptions options = null)
        {
            return await _cppSharpService.GenerateBindingsAsync(inputPath, outputPath, options);
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

            // 配置CppSharp选项
            builder.Configuration.AddJsonFile("cppsharp_aot.setting.json", optional: true);
            builder.Services.Configure<CppSharpOptions>(builder.Configuration.GetSection("CppSharp"));

            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);

            // 注册服务
            builder.Services.AddSingleton<ICppSharpService, CppSharpService>();
            builder.Services.AddSingleton<CppSharpAotEngine>();

            // 构建主机
            var host = builder.Build();
            var serviceProvider = host.Services;

            // 获取引擎实例
            var engine = serviceProvider.GetRequiredService<CppSharpAotEngine>();
            var options = serviceProvider.GetRequiredService<IOptions<CppSharpOptions>>().Value;

            // 解析命令行参数
            if (args.Length < 2)
            {
                Console.WriteLine("用法:");
                Console.WriteLine("  cppsharp_aot.exe generate <inputheader> <outputcs> [namespace]");
                return 1;
            }

            try
            {
                string command = args[0].ToLower();
                string inputPath = args[1];
                string outputPath = args[2];
                string namespaceName = args.Length > 3 ? args[3] : options.Namespace;

                switch (command)
                {
                    case "generate":
                        // 使用指定的命名空间
                        var customOptions = new CppSharpOptions
                        {
                            Namespace = namespaceName,
                            GenerateDebugInfo = options.GenerateDebugInfo,
                            GenerateAsyncWrappers = options.GenerateAsyncWrappers,
                            GenerateEventWrappers = options.GenerateEventWrappers
                        };

                        var result = await engine.ExecuteGenerateBindingsAsync(inputPath, outputPath, customOptions);
                        Console.WriteLine($"绑定生成结果: {(result ? "成功" : "失败")}");
                        return result ? 0 : 1;
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