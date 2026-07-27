#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package System.CommandLine@2.0.0
#:package System.Text.Json@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property SelfContained=true
#:property PublishSingleFile=true
#:property RuntimeIdentifier=win-x64

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SharpPcapSkill
{
    // 代码生成服务接口
    public interface ICodeGeneratorService
    {
        Task<string> GenerateParserAsync(string protocol);
        Task<string> GenerateAnalyzerAsync(string protocol);
        Task SaveGeneratedCodeAsync(string code, string outputPath);
    }

    // 代码生成服务实现
    public class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly ILogger<CodeGeneratorService> _logger;

        public CodeGeneratorService(ILogger<CodeGeneratorService> logger)
        {
            _logger = logger;
        }

        public Task<string> GenerateParserAsync(string protocol)
        {
            try
            {
                _logger.LogInformation($"开始生成 {protocol} 协议解析器代码");
                
                var code = GenerateParserCode(protocol);
                
                _logger.LogInformation($"{protocol} 协议解析器代码生成完成");
                return Task.FromResult(code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成协议解析器代码时发生错误");
                throw;
            }
        }

        public Task<string> GenerateAnalyzerAsync(string protocol)
        {
            try
            {
                _logger.LogInformation($"开始生成 {protocol} 协议分析器代码");
                
                var code = GenerateAnalyzerCode(protocol);
                
                _logger.LogInformation($"{protocol} 协议分析器代码生成完成");
                return Task.FromResult(code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成协议分析器代码时发生错误");
                throw;
            }
        }

        public async Task SaveGeneratedCodeAsync(string code, string outputPath)
        {
            try
            {
                _logger.LogInformation($"保存生成的代码到 {outputPath}");
                
                // 确保目录存在
                var directory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 写入文件
                await File.WriteAllTextAsync(outputPath, code, Encoding.UTF8);
                
                _logger.LogInformation($"代码保存成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存生成的代码时发生错误");
                throw;
            }
        }

        private string GenerateParserCode(string protocol)
        {
            var code = new StringBuilder();
            
            code.AppendLine($"using System;");
            code.AppendLine($"using System.Collections.Generic;");
            code.AppendLine($"using PacketDotNet;");
            code.AppendLine($"");
            code.AppendLine($"namespace SharpPcapSkill.Parsers");
            code.AppendLine($"{{");
            code.AppendLine($"    /// <summary>");
            code.AppendLine($"    /// {protocol} 协议解析器");
            code.AppendLine($"    /// </summary>");
            code.AppendLine($"    public class {protocol}Parser");
            code.AppendLine($"    {{");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 解析 {protocol} 协议数据包");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        /// <param name="packet">数据包</param>");
            code.AppendLine($"        /// <returns>解析结果</returns>");
            code.AppendLine($"        public {protocol}ParseResult Parse(Packet packet)");
            code.AppendLine($"        {{");
            code.AppendLine($"            var result = new {protocol}ParseResult();");
            code.AppendLine($"            ");
            code.AppendLine($"            // 解析 {protocol} 协议头部");
            code.AppendLine($"            // TODO: 实现具体的协议解析逻辑");
            code.AppendLine($"            ");
            code.AppendLine($"            // 示例代码");
            code.AppendLine($"            result.Timestamp = DateTime.Now;");
            code.AppendLine($"            result.Protocol = \"{protocol}\";");
            code.AppendLine($"            result.IsValid = true;");
            code.AppendLine($"            ");
            code.AppendLine($"            return result;");
            code.AppendLine($"        }}");
            code.AppendLine($"        ");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 解析 {protocol} 协议数据");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        /// <param name="data">原始数据</param>");
            code.AppendLine($"        /// <returns>解析结果</returns>");
            code.AppendLine($"        public {protocol}ParseResult Parse(byte[] data)");
            code.AppendLine($"        {{");
            code.AppendLine($"            var packet = Packet.ParsePacket(LinkLayers.Ethernet, data);");
            code.AppendLine($"            return Parse(packet);");
            code.AppendLine($"        }}");
            code.AppendLine($"    }}");
            code.AppendLine($"    ");
            code.AppendLine($"    /// <summary>");
            code.AppendLine($"    /// {protocol} 协议解析结果");
            code.AppendLine($"    /// </summary>");
            code.AppendLine($"    public class {protocol}ParseResult");
            code.AppendLine($"    {{");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 解析时间戳");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        public DateTime Timestamp {{ get; set; }}");
            code.AppendLine($"        ");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 协议类型");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        public string Protocol {{ get; set; }}");
            code.AppendLine($"        ");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 解析是否成功");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        public bool IsValid {{ get; set; }}");
            code.AppendLine($"        ");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 错误信息");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        public string ErrorMessage {{ get; set; }}");
            code.AppendLine($"        ");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 协议特定数据");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        public Dictionary<string, object> ProtocolData {{ get; set; }} = new Dictionary<string, object>();");
            code.AppendLine($"    }}");
            code.AppendLine($"}}");
            
            return code.ToString();
        }

        private string GenerateAnalyzerCode(string protocol)
        {
            var code = new StringBuilder();
            
            code.AppendLine($"using System;");
            code.AppendLine($"using System.Collections.Generic;");
            code.AppendLine($"using System.Linq;");
            code.AppendLine($"using PacketDotNet;");
            code.AppendLine($"using SharpPcap;");
            code.AppendLine($"");
            code.AppendLine($"namespace SharpPcapSkill.Analyzers");
            code.AppendLine($"{{");
            code.AppendLine($"    /// <summary>");
            code.AppendLine($"    /// {protocol} 协议分析器");
            code.AppendLine($"    /// </summary>");
            code.AppendLine($"    public class {protocol}Analyzer");
            code.AppendLine($"    {{");
            code.AppendLine($"        private readonly {protocol}Parser _parser;");
            code.AppendLine($"        ");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 构造函数");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        public {protocol}Analyzer()");
            code.AppendLine($"        {{");
            code.AppendLine($"            _parser = new {protocol}Parser();");
            code.AppendLine($"        }}");
            code.AppendLine($"        ");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 分析 {protocol} 协议数据包");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        /// <param name="packet">数据包</param>");
            code.AppendLine($"        /// <returns>分析结果</returns>");
            code.AppendLine($"        public {protocol}AnalysisResult Analyze(Packet packet)");
            code.AppendLine($"        {{");
            code.AppendLine($"            var result = new {protocol}AnalysisResult();");
            code.AppendLine($"            ");
            code.AppendLine($"            try");
            code.AppendLine($"            {{");
            code.AppendLine($"                // 解析数据包");
            code.AppendLine($"                var parseResult = _parser.Parse(packet);");
            code.AppendLine($"                result.ParseResult = parseResult;");
            code.AppendLine($"                ");
            code.AppendLine($"                // 分析数据包");
            code.AppendLine($"                // TODO: 实现具体的协议分析逻辑");
            code.AppendLine($"                ");
            code.AppendLine($"                // 示例代码");
            code.AppendLine($"                result.AnalysisTime = DateTime.Now;");
            code.AppendLine($"                result.IsAnomalous = false;");
            code.AppendLine($"                result.AnomalyScore = 0.0;");
            code.AppendLine($"                ");
            code.AppendLine($"                // 提取关键信息");
            code.AppendLine($"                var ethernetPacket = packet.Extract<EthernetPacket>();");
            code.AppendLine($"                if (ethernetPacket != null)");
            code.AppendLine($"                {{");
            code.AppendLine($"                    result.SourceAddress = ethernetPacket.SourceHardwareAddress.ToString();");
            code.AppendLine($"                    result.DestinationAddress = ethernetPacket.DestinationHardwareAddress.ToString();");
            code.AppendLine($"                }}");
            code.AppendLine($"                ");
            code.AppendLine($"                var ipPacket = packet.Extract<IpPacket>();");
            code.AppendLine($"                if (ipPacket != null)");
            code.AppendLine($"                {{");
            code.AppendLine($"                    result.SourceIp = ipPacket.SourceAddress.ToString();");
            code.AppendLine($"                    result.DestinationIp = ipPacket.DestinationAddress.ToString();");
            code.AppendLine($"                }}");
            code.AppendLine($"            }}");
            code.AppendLine($"            catch (Exception ex)");
            code.AppendLine($"            {{");
            code.AppendLine($"                result.IsAnomalous = true;");
            code.AppendLine($"                result.ErrorMessage = ex.Message;");
            code.AppendLine($"            }}");
            code.AppendLine($"            ");
            code.AppendLine($"            return result;");
            code.AppendLine($"        }}");
            code.AppendLine($"        ");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 分析 {protocol} 协议数据");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        /// <param name="data">原始数据</param>");
            code.AppendLine($"        /// <returns>分析结果</returns>");
            code.AppendLine($"        public {protocol}AnalysisResult Analyze(byte[] data)");
            code.AppendLine($"        {{");
            code.AppendLine($"            var packet = Packet.ParsePacket(LinkLayers.Ethernet, data);");
            code.AppendLine($"            return Analyze(packet);");
            code.AppendLine($"        }}");
            code.AppendLine($"        ");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 批量分析 {protocol} 协议数据包");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        /// <param name="packets">数据包列表</param>");
            code.AppendLine($"        /// <returns>分析结果列表</returns>");
            code.AppendLine($"        public List<{protocol}AnalysisResult> AnalyzeBatch(IEnumerable<Packet> packets)");
            code.AppendLine($"        {{");
            code.AppendLine($"            return packets.Select(Analyze).ToList();");
            code.AppendLine($"        }}");
            code.AppendLine($"    }}");
            code.AppendLine($"    ");
            code.AppendLine($"    /// <summary>");
            code.AppendLine($"    /// {protocol} 协议分析结果");
            code.AppendLine($"    /// </summary>");
            code.AppendLine($"    public class {protocol}AnalysisResult");
            code.AppendLine($"    {{");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 分析时间戳");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        public DateTime AnalysisTime {{ get; set; }}");
            code.AppendLine($"        ");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 源地址");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        public string SourceAddress {{ get; set; }}");
            code.AppendLine($"        ");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 目标地址");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        public string DestinationAddress {{ get; set; }}");
            code.AppendLine($"        ");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 源 IP 地址");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        public string SourceIp {{ get; set; }}");
            code.AppendLine($"        ");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 目标 IP 地址");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        public string DestinationIp {{ get; set; }}");
            code.AppendLine($"        ");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 解析结果");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        public {protocol}ParseResult ParseResult {{ get; set; }}");
            code.AppendLine($"        ");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 是否异常");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        public bool IsAnomalous {{ get; set; }}");
            code.AppendLine($"        ");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 异常得分");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        public double AnomalyScore {{ get; set; }}");
            code.AppendLine($"        ");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 错误信息");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        public string ErrorMessage {{ get; set; }}");
            code.AppendLine($"        ");
            code.AppendLine($"        /// <summary>");
            code.AppendLine($"        /// 分析结果摘要");
            code.AppendLine($"        /// </summary>");
            code.AppendLine($"        public string Summary {{ get; set; }}");
            code.AppendLine($"    }}");
            code.AppendLine($"}}");
            
            return code.ToString();
        }
    }

    // 命令行工具
    public class CodeGeneratorCli
    {
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly ILogger<CodeGeneratorCli> _logger;

        public CodeGeneratorCli(ICodeGeneratorService codeGeneratorService, ILogger<CodeGeneratorCli> logger)
        {
            _codeGeneratorService = codeGeneratorService;
            _logger = logger;
        }

        public async Task<int> RunAsync(string[] args)
        {
            // 创建根命令
            var rootCommand = new RootCommand("SharpPcap 代码生成工具");

            // 生成命令
            var generateCommand = new Command("generate", "生成代码");
            var typeOption = new Option<string>("--type", "生成类型（parser/analyzer）");
            var protocolOption = new Option<string>("--protocol", "协议类型");
            var outputOption = new Option<string>("--output", "输出文件路径");

            generateCommand.AddOption(typeOption);
            generateCommand.AddOption(protocolOption);
            generateCommand.AddOption(outputOption);

            generateCommand.Handler = CommandHandler.Create<string, string, string>(async (type, protocol, output) =>
            {
                string code;
                
                if (type.Equals("parser", StringComparison.OrdinalIgnoreCase))
                {
                    code = await _codeGeneratorService.GenerateParserAsync(protocol);
                }
                else if (type.Equals("analyzer", StringComparison.OrdinalIgnoreCase))
                {
                    code = await _codeGeneratorService.GenerateAnalyzerAsync(protocol);
                }
                else
                {
                    Console.WriteLine($"无效的生成类型: {type}，支持的类型: parser, analyzer");
                    return;
                }

                if (!string.IsNullOrEmpty(output))
                {
                    await _codeGeneratorService.SaveGeneratedCodeAsync(code, output);
                    Console.WriteLine($"代码已保存到: {output}");
                }
                else
                {
                    Console.WriteLine("生成的代码:");
                    Console.WriteLine(code);
                }
            });

            // 添加命令到根命令
            rootCommand.AddCommand(generateCommand);

            // 执行命令
            return await rootCommand.InvokeAsync(args);
        }
    }

    // 主程序
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            // 创建服务容器
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                })
                .AddSingleton<ICodeGeneratorService, CodeGeneratorService>()
                .AddSingleton<CodeGeneratorCli>()
                .BuildServiceProvider();

            // 获取命令行工具
            var cli = serviceProvider.GetRequiredService<CodeGeneratorCli>();

            // 运行命令
            return await cli.RunAsync(args);
        }
    }
}
