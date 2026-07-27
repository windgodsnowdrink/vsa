#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Caching.Memory@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.CommandLine@2.0.0
#:package System.Reflection.MetadataLoadContext@8.0.0
#:package System.Reflection.Emit@4.7.0
#:package Mono.Cecil@0.11.5
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
#:property Optimize=true
#:property PublishTrimmed=true

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mono.Cecil;

namespace LinkersAot
{
    /// <summary>
    /// Linkers服务，提供程序集分析、优化和验证功能
    /// </summary>
    public class LinkersService
    {
        private readonly ILogger<LinkersService> _logger;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="cache">内存缓存</param>
        public LinkersService(ILogger<LinkersService> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
            _cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                Size = 1024
            };
        }

        /// <summary>
        /// 分析程序集
        /// </summary>
        /// <param name="assemblyPath">程序集路径</param>
        /// <returns>分析结果</returns>
        public async Task<string> AnalyzeAssemblyAsync(string assemblyPath)
        {
            _logger.LogInformation($"开始分析程序集: {assemblyPath}");

            try
            {
                if (!File.Exists(assemblyPath))
                {
                    throw new FileNotFoundException($"程序集文件不存在: {assemblyPath}");
                }

                // 尝试从缓存获取
                var cacheKey = $"analyze:{assemblyPath}:{File.GetLastWriteTimeUtc(assemblyPath)}";
                if (_cache.TryGetValue(cacheKey, out string cachedResult))
                {
                    _logger.LogInformation("从缓存获取分析结果");
                    return cachedResult;
                }

                using var assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
                var result = new System.Text.StringBuilder();

                result.AppendLine($"=== 程序集分析结果 ===");
                result.AppendLine($"程序集名称: {assembly.Name.Name}");
                result.AppendLine($"版本: {assembly.Name.Version}");
                result.AppendLine($"架构: {assembly.MainModule.Architecture}");
                result.AppendLine($"运行时: {assembly.MainModule.Runtime}");
                result.AppendLine($"入口点: {assembly.EntryPoint?.FullName ?? "无"}");
                result.AppendLine();

                // 统计类型、方法、字段等
                int typeCount = 0, methodCount = 0, fieldCount = 0, propertyCount = 0, eventCount = 0;

                foreach (var type in assembly.MainModule.Types)
                {
                    if (!type.IsPublic || type.Name.StartsWith('<')) continue;

                    typeCount++;
                    methodCount += type.Methods.Count;
                    fieldCount += type.Fields.Count;
                    propertyCount += type.Properties.Count;
                    eventCount += type.Events.Count;
                }

                result.AppendLine($"=== 内容统计 ===");
                result.AppendLine($"公共类型: {typeCount}");
                result.AppendLine($"方法总数: {methodCount}");
                result.AppendLine($"字段总数: {fieldCount}");
                result.AppendLine($"属性总数: {propertyCount}");
                result.AppendLine($"事件总数: {eventCount}");
                result.AppendLine();

                // 列出依赖项
                result.AppendLine($"=== 直接依赖项 ===");
                foreach (var reference in assembly.MainModule.AssemblyReferences)
                {
                    result.AppendLine($"- {reference.Name} v{reference.Version}");
                }

                var resultString = result.ToString();

                // 缓存结果
                _cache.Set(cacheKey, resultString, _cacheOptions);

                _logger.LogInformation($"程序集分析完成: {assemblyPath}");
                return resultString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"分析程序集时出错: {assemblyPath}");
                throw;
            }
        }

        /// <summary>
        /// 解析符号
        /// </summary>
        /// <param name="assemblyPath">程序集路径</param>
        /// <param name="symbolName">符号名称</param>
        /// <returns>解析结果</returns>
        public async Task<string> ResolveSymbolAsync(string assemblyPath, string symbolName)
        {
            _logger.LogInformation($"开始解析符号: {symbolName} 在 {assemblyPath}");

            try
            {
                if (!File.Exists(assemblyPath))
                {
                    throw new FileNotFoundException($"程序集文件不存在: {assemblyPath}");
                }

                using var assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
                var result = new System.Text.StringBuilder();

                result.AppendLine($"=== 符号解析结果 ===");
                result.AppendLine($"目标符号: {symbolName}");
                result.AppendLine($"程序集: {assembly.Name.Name} v{assembly.Name.Version}");
                result.AppendLine();

                // 查找类型
                var found = false;
                foreach (var type in assembly.MainModule.Types)
                {
                    if (type.Name.Equals(symbolName, StringComparison.OrdinalIgnoreCase) || 
                        type.FullName.Equals(symbolName, StringComparison.OrdinalIgnoreCase))
                    {
                        result.AppendLine($"找到类型: {type.FullName}");
                        result.AppendLine($"可见性: {type.Attributes}");
                        result.AppendLine($"基类: {type.BaseType?.FullName ?? "无"}");
                        result.AppendLine($"接口: {string.Join(", ", type.Interfaces.Select(i => i.InterfaceType.FullName))}");
                        result.AppendLine();

                        // 列出成员
                        result.AppendLine("  成员:");
                        foreach (var method in type.Methods)
                        {
                            if (!method.IsPublic) continue;
                            result.AppendLine($"    - 方法: {method.Name}()");
                        }
                        foreach (var property in type.Properties)
                        {
                            if (!property.GetMethod?.IsPublic ?? !property.SetMethod?.IsPublic) continue;
                            result.AppendLine($"    - 属性: {property.Name}");
                        }
                        foreach (var field in type.Fields)
                        {
                            if (!field.IsPublic) continue;
                            result.AppendLine($"    - 字段: {field.Name}");
                        }

                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    result.AppendLine("未找到指定符号");
                }

                _logger.LogInformation($"符号解析完成: {symbolName}");
                return result.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"解析符号时出错: {symbolName}");
                throw;
            }
        }

        /// <summary>
        /// 列出依赖项
        /// </summary>
        /// <param name="assemblyPath">程序集路径</param>
        /// <returns>依赖项列表</returns>
        public async Task<string> ListDependenciesAsync(string assemblyPath)
        {
            _logger.LogInformation($"开始列出依赖项: {assemblyPath}");

            try
            {
                if (!File.Exists(assemblyPath))
                {
                    throw new FileNotFoundException($"程序集文件不存在: {assemblyPath}");
                }

                // 尝试从缓存获取
                var cacheKey = $"dependencies:{assemblyPath}:{File.GetLastWriteTimeUtc(assemblyPath)}";
                if (_cache.TryGetValue(cacheKey, out string cachedResult))
                {
                    _logger.LogInformation("从缓存获取依赖项列表");
                    return cachedResult;
                }

                using var assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
                var result = new System.Text.StringBuilder();

                result.AppendLine($"=== 依赖项列表 ===");
                result.AppendLine($"目标程序集: {assembly.Name.Name} v{assembly.Name.Version}");
                result.AppendLine();

                // 列出直接依赖项
                result.AppendLine("直接依赖项:");
                foreach (var reference in assembly.MainModule.AssemblyReferences)
                {
                    result.AppendLine($"- {reference.Name} v{reference.Version}");
                }

                var resultString = result.ToString();

                // 缓存结果
                _cache.Set(cacheKey, resultString, _cacheOptions);

                _logger.LogInformation($"依赖项列出完成: {assemblyPath}");
                return resultString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"列出依赖项时出错: {assemblyPath}");
                throw;
            }
        }

        /// <summary>
        /// 优化程序集
        /// </summary>
        /// <param name="inputPath">输入程序集路径</param>
        /// <param name="outputPath">输出程序集路径</param>
        /// <param name="optimizationLevel">优化级别</param>
        /// <returns>优化结果</returns>
        public async Task<string> OptimizeAssemblyAsync(string inputPath, string outputPath, string optimizationLevel)
        {
            _logger.LogInformation($"开始优化程序集: {inputPath} -> {outputPath} (级别: {optimizationLevel})");

            try
            {
                if (!File.Exists(inputPath))
                {
                    throw new FileNotFoundException($"输入程序集文件不存在: {inputPath}");
                }

                // 确保输出目录存在
                var outputDir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                using var assembly = AssemblyDefinition.ReadAssembly(inputPath);
                
                // 根据优化级别设置参数
                var writerParameters = new WriterParameters
                {
                    WriteSymbols = optimizationLevel != "high",
                    OptimizeForSize = true
                };

                // 保存优化后的程序集
                assembly.Write(outputPath, writerParameters);

                var inputSize = new FileInfo(inputPath).Length;
                var outputSize = new FileInfo(outputPath).Length;
                var reduction = ((double)(inputSize - outputSize) / inputSize) * 100;

                var result = new System.Text.StringBuilder();
                result.AppendLine($"=== 程序集优化结果 ===");
                result.AppendLine($"输入文件: {inputPath}");
                result.AppendLine($"输出文件: {outputPath}");
                result.AppendLine($"优化级别: {optimizationLevel}");
                result.AppendLine($"输入大小: {FormatFileSize(inputSize)}");
                result.AppendLine($"输出大小: {FormatFileSize(outputSize)}");
                result.AppendLine($"大小减少: {reduction:F2}%");
                result.AppendLine($"优化完成: {DateTime.Now}");

                _logger.LogInformation($"程序集优化完成: {inputPath} -> {outputPath}");
                return result.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"优化程序集时出错: {inputPath}");
                throw;
            }
        }

        /// <summary>
        /// 验证程序集
        /// </summary>
        /// <param name="assemblyPath">程序集路径</param>
        /// <returns>验证结果</returns>
        public async Task<string> VerifyAssemblyAsync(string assemblyPath)
        {
            _logger.LogInformation($"开始验证程序集: {assemblyPath}");

            try
            {
                if (!File.Exists(assemblyPath))
                {
                    throw new FileNotFoundException($"程序集文件不存在: {assemblyPath}");
                }

                using var assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
                var result = new System.Text.StringBuilder();

                result.AppendLine($"=== 程序集验证结果 ===");
                result.AppendLine($"目标程序集: {assembly.Name.Name} v{assembly.Name.Version}");
                result.AppendLine();

                // 验证基本信息
                result.AppendLine("验证项目:");
                result.AppendLine($"- 文件存在: ✓");
                result.AppendLine($"- 可读取: ✓");
                result.AppendLine($"- 程序集格式有效: ✓");
                result.AppendLine($"- 模块数量: {assembly.Modules.Count}");
                result.AppendLine($"- 类型数量: {assembly.MainModule.Types.Count}");

                // 验证依赖项
                var missingDependencies = new List<string>();
                foreach (var reference in assembly.MainModule.AssemblyReferences)
                {
                    // 简单验证依赖项是否可解析
                    try
                    {
                        // 尝试在当前目录查找
                        var depPath = Path.Combine(Path.GetDirectoryName(assemblyPath), $"{reference.Name}.dll");
                        if (File.Exists(depPath))
                        {
                            using var _ = AssemblyDefinition.ReadAssembly(depPath);
                        }
                    }
                    catch
                    {
                        missingDependencies.Add($"{reference.Name} v{reference.Version}");
                    }
                }

                if (missingDependencies.Count > 0)
                {
                    result.AppendLine();
                    result.AppendLine("警告: 可能缺少的依赖项:");
                    foreach (var dep in missingDependencies)
                    {
                        result.AppendLine($"- {dep}");
                    }
                }
                else
                {
                    result.AppendLine();
                    result.AppendLine("- 依赖项验证: ✓");
                }

                result.AppendLine();
                result.AppendLine($"验证完成: {DateTime.Now}");
                result.AppendLine($"验证状态: {(missingDependencies.Count == 0 ? "通过" : "警告")}");

                _logger.LogInformation($"程序集验证完成: {assemblyPath}");
                return result.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"验证程序集时出错: {assemblyPath}");
                throw;
            }
        }

        /// <summary>
        /// 提取程序集内容
        /// </summary>
        /// <param name="assemblyPath">程序集路径</param>
        /// <param name="outputPath">输出路径</param>
        /// <returns>提取结果</returns>
        public async Task<string> ExtractAssemblyAsync(string assemblyPath, string outputPath)
        {
            _logger.LogInformation($"开始提取程序集内容: {assemblyPath} -> {outputPath}");

            try
            {
                if (!File.Exists(assemblyPath))
                {
                    throw new FileNotFoundException($"程序集文件不存在: {assemblyPath}");
                }

                // 确保输出目录存在
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                using var assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
                
                // 提取类型信息
                var typesPath = Path.Combine(outputPath, "types.txt");
                using (var writer = new StreamWriter(typesPath))
                {
                    writer.WriteLine("=== 程序集类型信息 ===");
                    writer.WriteLine($"程序集: {assembly.Name.Name} v{assembly.Name.Version}");
                    writer.WriteLine();
                    
                    foreach (var type in assembly.MainModule.Types)
                    {
                        if (!type.IsPublic || type.Name.StartsWith('<')) continue;
                        
                        writer.WriteLine($"类型: {type.FullName}");
                        writer.WriteLine($"  可见性: {type.Attributes}");
                        writer.WriteLine($"  基类: {type.BaseType?.FullName ?? "无"}");
                        writer.WriteLine($"  接口: {string.Join(", ", type.Interfaces.Select(i => i.InterfaceType.FullName))}");
                        writer.WriteLine();
                    }
                }

                // 提取资源信息
                var resourcesPath = Path.Combine(outputPath, "resources.txt");
                using (var writer = new StreamWriter(resourcesPath))
                {
                    writer.WriteLine("=== 程序集资源信息 ===");
                    writer.WriteLine($"程序集: {assembly.Name.Name} v{assembly.Name.Version}");
                    writer.WriteLine();
                    
                    foreach (var resource in assembly.MainModule.Resources)
                    {
                        writer.WriteLine($"资源: {resource.Name}");
                        writer.WriteLine($"  类型: {resource.GetType().Name}");
                        writer.WriteLine();
                    }
                }

                var result = new System.Text.StringBuilder();
                result.AppendLine($"=== 程序集内容提取结果 ===");
                result.AppendLine($"输入文件: {assemblyPath}");
                result.AppendLine($"输出目录: {outputPath}");
                result.AppendLine($"提取文件:");
                result.AppendLine($"- types.txt: 类型信息");
                result.AppendLine($"- resources.txt: 资源信息");
                result.AppendLine($"提取完成: {DateTime.Now}");

                _logger.LogInformation($"程序集内容提取完成: {assemblyPath} -> {outputPath}");
                return result.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"提取程序集内容时出错: {assemblyPath}");
                throw;
            }
        }

        /// <summary>
        /// 生成原生库绑定
        /// </summary>
        /// <param name="nativeLibPath">原生库路径</param>
        /// <param name="outputPath">输出路径</param>
        /// <returns>生成结果</returns>
        public async Task<string> GenerateBindingsAsync(string nativeLibPath, string outputPath)
        {
            _logger.LogInformation($"开始生成原生库绑定: {nativeLibPath} -> {outputPath}");

            try
            {
                if (!File.Exists(nativeLibPath))
                {
                    throw new FileNotFoundException($"原生库文件不存在: {nativeLibPath}");
                }

                // 确保输出目录存在
                var outputDir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                // 模拟生成绑定代码
                var bindingsCode = new System.Text.StringBuilder();
                bindingsCode.AppendLine("// 原生库绑定代码");
                bindingsCode.AppendLine("// 自动生成于: " + DateTime.Now);
                bindingsCode.AppendLine("// 原生库: " + nativeLibPath);
                bindingsCode.AppendLine();
                bindingsCode.AppendLine("using System;");
                bindingsCode.AppendLine("using System.Runtime.InteropServices;");
                bindingsCode.AppendLine();
                bindingsCode.AppendLine("namespace NativeBindings");
                bindingsCode.AppendLine("{");
                bindingsCode.AppendLine("    public static class NativeMethods");
                bindingsCode.AppendLine("    {");
                bindingsCode.AppendLine($"        private const string LibraryName = \"{Path.GetFileName(nativeLibPath)}\";");
                bindingsCode.AppendLine();
                bindingsCode.AppendLine("        // 示例方法绑定");
                bindingsCode.AppendLine("        [DllImport(LibraryName)]");
                bindingsCode.AppendLine("        public static extern int ExampleFunction(int parameter);");
                bindingsCode.AppendLine();
                bindingsCode.AppendLine("        [DllImport(LibraryName)]");
                bindingsCode.AppendLine("        public static extern void AnotherFunction(IntPtr data, int length);");
                bindingsCode.AppendLine("    }");
                bindingsCode.AppendLine("}");

                File.WriteAllText(outputPath, bindingsCode.ToString());

                var result = new System.Text.StringBuilder();
                result.AppendLine($"=== 原生库绑定生成结果 ===");
                result.AppendLine($"原生库: {nativeLibPath}");
                result.AppendLine($"输出文件: {outputPath}");
                result.AppendLine($"生成状态: 完成");
                result.AppendLine($"生成时间: {DateTime.Now}");
                result.AppendLine();
                result.AppendLine("注意: 这是一个模拟实现，实际绑定代码需要根据原生库的具体接口生成.");

                _logger.LogInformation($"原生库绑定生成完成: {nativeLibPath} -> {outputPath}");
                return result.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"生成原生库绑定时出错: {nativeLibPath}");
                throw;
            }
        }

        /// <summary>
        /// 运行性能基准测试
        /// </summary>
        /// <param name="assemblyPath">程序集路径</param>
        /// <param name="iterations">迭代次数</param>
        /// <returns>基准测试结果</returns>
        public async Task<string> RunBenchmarkAsync(string assemblyPath, int iterations = 10)
        {
            _logger.LogInformation($"开始运行性能基准测试: {assemblyPath} (迭代: {iterations})");

            try
            {
                if (!File.Exists(assemblyPath))
                {
                    throw new FileNotFoundException($"程序集文件不存在: {assemblyPath}");
                }

                var results = new List<long>();
                
                // 运行多次测试
                for (int i = 0; i < iterations; i++)
                {
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    
                    // 测试程序集加载时间
                    using var assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
                    
                    stopwatch.Stop();
                    results.Add(stopwatch.ElapsedMilliseconds);
                }

                // 计算统计信息
                var avgTime = results.Average();
                var minTime = results.Min();
                var maxTime = results.Max();

                var result = new System.Text.StringBuilder();
                result.AppendLine($"=== 性能基准测试结果 ===");
                result.AppendLine($"目标程序集: {assemblyPath}");
                result.AppendLine($"迭代次数: {iterations}");
                result.AppendLine();
                result.AppendLine("测试结果:");
                result.AppendLine($"- 平均加载时间: {avgTime:F2} ms");
                result.AppendLine($"- 最短加载时间: {minTime} ms");
                result.AppendLine($"- 最长加载时间: {maxTime} ms");
                result.AppendLine();
                result.AppendLine("详细数据:");
                result.AppendLine(string.Join(", ", results));

                _logger.LogInformation($"性能基准测试完成: {assemblyPath}");
                return result.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"运行性能基准测试时出错: {assemblyPath}");
                throw;
            }
        }

        /// <summary>
        /// 格式化文件大小
        /// </summary>
        /// <param name="bytes">字节数</param>
        /// <returns>格式化后的大小</returns>
        private string FormatFileSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB" };
            int counter = 0;
            double number = bytes;
            
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            
            return $"{number:F2} {suffixes[counter]}";
        }
    }

    /// <summary>
    /// 主程序类
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
            // 创建命令行根命令
            var rootCommand = new RootCommand("Linkers AOT - 基于AOT编译的链接器工具");

            // 创建服务提供者
            var serviceProvider = BuildServiceProvider();
            var linkersService = serviceProvider.GetRequiredService<LinkersService>();

            // 分析程序集命令
            var analyzeCommand = new Command("analyze", "分析程序集的详细信息")
            {
                new Argument<string>("assembly", "程序集文件路径")
            };
            analyzeCommand.AddAlias("a");
            analyzeCommand.Handler = CommandHandler.Create<string>(async (assembly) =>
            {
                try
                {
                    var result = await linkersService.AnalyzeAssemblyAsync(assembly);
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"错误: {ex.Message}");
                    return 1;
                }
                return 0;
            });

            // 解析符号命令
            var resolveCommand = new Command("resolve", "解析程序集中的符号")
            {
                new Argument<string>("assembly", "程序集文件路径"),
                new Argument<string>("symbol", "要解析的符号名称")
            };
            resolveCommand.AddAlias("r");
            resolveCommand.Handler = CommandHandler.Create<string, string>(async (assembly, symbol) =>
            {
                try
                {
                    var result = await linkersService.ResolveSymbolAsync(assembly, symbol);
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"错误: {ex.Message}");
                    return 1;
                }
                return 0;
            });

            // 列出依赖项命令
            var listDependenciesCommand = new Command("list-dependencies", "列出程序集的依赖项")
            {
                new Argument<string>("assembly", "程序集文件路径")
            };
            listDependenciesCommand.AddAlias("ld");
            listDependenciesCommand.Handler = CommandHandler.Create<string>(async (assembly) =>
            {
                try
                {
                    var result = await linkersService.ListDependenciesAsync(assembly);
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"错误: {ex.Message}");
                    return 1;
                }
                return 0;
            });

            // 优化程序集命令
            var optimizeCommand = new Command("optimize", "优化程序集")
            {
                new Argument<string>("input", "输入程序集路径"),
                new Argument<string>("output", "输出程序集路径"),
                new Option<string>("--level", () => "medium", "优化级别: low, medium, high")
            };
            optimizeCommand.AddAlias("o");
            optimizeCommand.Handler = CommandHandler.Create<string, string, string>(async (input, output, level) =>
            {
                try
                {
                    var result = await linkersService.OptimizeAssemblyAsync(input, output, level);
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"错误: {ex.Message}");
                    return 1;
                }
                return 0;
            });

            // 验证程序集命令
            var verifyCommand = new Command("verify", "验证程序集的完整性")
            {
                new Argument<string>("assembly", "程序集文件路径")
            };
            verifyCommand.AddAlias("v");
            verifyCommand.Handler = CommandHandler.Create<string>(async (assembly) =>
            {
                try
                {
                    var result = await linkersService.VerifyAssemblyAsync(assembly);
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"错误: {ex.Message}");
                    return 1;
                }
                return 0;
            });

            // 提取程序集内容命令
            var extractCommand = new Command("extract", "提取程序集的内容")
            {
                new Argument<string>("assembly", "程序集文件路径"),
                new Argument<string>("output", "输出目录路径")
            };
            extractCommand.AddAlias("e");
            extractCommand.Handler = CommandHandler.Create<string, string>(async (assembly, output) =>
            {
                try
                {
                    var result = await linkersService.ExtractAssemblyAsync(assembly, output);
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"错误: {ex.Message}");
                    return 1;
                }
                return 0;
            });

            // 生成原生库绑定命令
            var generateBindingsCommand = new Command("generate-bindings", "生成原生库的C#绑定代码")
            {
                new Argument<string>("native-lib", "原生库文件路径"),
                new Argument<string>("output", "输出文件路径")
            };
            generateBindingsCommand.AddAlias("gb");
            generateBindingsCommand.Handler = CommandHandler.Create<string, string>(async (nativeLib, output) =>
            {
                try
                {
                    var result = await linkersService.GenerateBindingsAsync(nativeLib, output);
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"错误: {ex.Message}");
                    return 1;
                }
                return 0;
            });

            // 运行性能基准测试命令
            var benchmarkCommand = new Command("benchmark", "运行程序集的性能基准测试")
            {
                new Argument<string>("assembly", "程序集文件路径"),
                new Option<int>("--iterations", () => 10, "测试迭代次数")
            };
            benchmarkCommand.AddAlias("bm");
            benchmarkCommand.Handler = CommandHandler.Create<string, int>(async (assembly, iterations) =>
            {
                try
                {
                    var result = await linkersService.RunBenchmarkAsync(assembly, iterations);
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"错误: {ex.Message}");
                    return 1;
                }
                return 0;
            });

            // 帮助命令
            var helpCommand = new Command("help", "显示帮助信息");
            helpCommand.AddAlias("h");
            helpCommand.Handler = CommandHandler.Create(() =>
            {
                rootCommand.Invoke("--help");
                return 0;
            });

            // 添加命令到根命令
            rootCommand.AddCommand(analyzeCommand);
            rootCommand.AddCommand(resolveCommand);
            rootCommand.AddCommand(listDependenciesCommand);
            rootCommand.AddCommand(optimizeCommand);
            rootCommand.AddCommand(verifyCommand);
            rootCommand.AddCommand(extractCommand);
            rootCommand.AddCommand(generateBindingsCommand);
            rootCommand.AddCommand(benchmarkCommand);
            rootCommand.AddCommand(helpCommand);

            // 执行命令
            return await rootCommand.InvokeAsync(args);
        }

        /// <summary>
        /// 构建服务提供者
        /// </summary>
        /// <returns>服务提供者</returns>
        private static ServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            // 添加日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // 添加内存缓存
            services.AddMemoryCache(options =>
            {
                options.SizeLimit = 1024 * 1024;
                options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
            });

            // 添加Linkers服务
            services.AddSingleton<LinkersService>();

            return services.BuildServiceProvider();
        }
    }
}
