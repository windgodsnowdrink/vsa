#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package System.CommandLine@2.0.0
#:package System.Text.Json@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
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

namespace SilkySkill
{
    // 代码生成服务接口
    public interface ICodeGeneratorService
    {
        Task<string> GenerateApiAsync(string serviceName);
        Task<string> GenerateImplAsync(string serviceName);
        Task<string> GenerateDtoAsync(string serviceName);
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

        public Task<string> GenerateApiAsync(string serviceName)
        {
            try
            {
                _logger.LogInformation($"开始生成 {serviceName} 服务的 API 接口代码");
                
                var code = GenerateApiCode(serviceName);
                
                _logger.LogInformation($"{serviceName} 服务的 API 接口代码生成完成");
                return Task.FromResult(code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成 API 接口代码时发生错误");
                throw;
            }
        }

        public Task<string> GenerateImplAsync(string serviceName)
        {
            try
            {
                _logger.LogInformation($"开始生成 {serviceName} 服务的实现代码");
                
                var code = GenerateImplCode(serviceName);
                
                _logger.LogInformation($"{serviceName} 服务的实现代码生成完成");
                return Task.FromResult(code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成实现代码时发生错误");
                throw;
            }
        }

        public Task<string> GenerateDtoAsync(string serviceName)
        {
            try
            {
                _logger.LogInformation($"开始生成 {serviceName} 服务的数据传输对象代码");
                
                var code = GenerateDtoCode(serviceName);
                
                _logger.LogInformation($"{serviceName} 服务的数据传输对象代码生成完成");
                return Task.FromResult(code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成数据传输对象代码时发生错误");
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

        private string GenerateApiCode(string serviceName)
        {
            var code = new StringBuilder();
            
            code.AppendLine($"using System.Collections.Generic;");
            code.AppendLine($"using System.Threading.Tasks;");
            code.AppendLine($"using Silky.Http.Core.Attributes;");
            code.AppendLine($"using Silky.Rpc.Routing;");
            code.AppendLine($"");
            code.AppendLine($"namespace {serviceName}.Services");
            code.AppendLine($"{{");
            code.AppendLine($"    [ServiceRoute]");
            code.AppendLine($"    public interface I{serviceName}Service");
            code.AppendLine($"    {{");
            code.AppendLine($"        [HttpGet(\"{serviceName.ToLower()}\")]");
            code.AppendLine($"        Task<List<{serviceName}Dto>> Get{serviceName}sAsync();");
            code.AppendLine($"        ");
            code.AppendLine($"        [HttpGet(\"{serviceName.ToLower()}/{id}\")]");
            code.AppendLine($"        Task<{serviceName}Dto> Get{serviceName}ByIdAsync(long id);");
            code.AppendLine($"        ");
            code.AppendLine($"        [HttpPost(\"{serviceName.ToLower()}\")]");
            code.AppendLine($"        Task<long> Create{serviceName}Async({serviceName}Dto {serviceName.ToLower()});");
            code.AppendLine($"        ");
            code.AppendLine($"        [HttpPut(\"{serviceName.ToLower()}/{id}\")]");
            code.AppendLine($"        Task<bool> Update{serviceName}Async(long id, {serviceName}Dto {serviceName.ToLower()});");
            code.AppendLine($"        ");
            code.AppendLine($"        [HttpDelete(\"{serviceName.ToLower()}/{id}\")]");
            code.AppendLine($"        Task<bool> Delete{serviceName}Async(long id);");
            code.AppendLine($"    }}");
            code.AppendLine($"}}");
            
            return code.ToString();
        }

        private string GenerateImplCode(string serviceName)
        {
            var code = new StringBuilder();
            
            code.AppendLine($"using System.Collections.Generic;");
            code.AppendLine($"using System.Linq;");
            code.AppendLine($"using System.Threading.Tasks;");
            code.AppendLine($"");
            code.AppendLine($"namespace {serviceName}.Services.Impl");
            code.AppendLine($"{{");
            code.AppendLine($"    public class {serviceName}Service : I{serviceName}Service");
            code.AppendLine($"    {{");
            code.AppendLine($"        private static readonly List<{serviceName}Dto> _{serviceName.ToLower()}s = new()");
            code.AppendLine($"        {{");
            code.AppendLine($"            new {serviceName}Dto {{ Id = 1, Name = \"测试{serviceName}\" }}");
            code.AppendLine($"        }};");
            code.AppendLine($"        ");
            code.AppendLine($"        public Task<List<{serviceName}Dto>> Get{serviceName}sAsync()");
            code.AppendLine($"        {{");
            code.AppendLine($"            return Task.FromResult(_{serviceName.ToLower()}s);");
            code.AppendLine($"        }}");
            code.AppendLine($"        ");
            code.AppendLine($"        public Task<{serviceName}Dto> Get{serviceName}ByIdAsync(long id)");
            code.AppendLine($"        {{");
            code.AppendLine($"            var {serviceName.ToLower()} = _{serviceName.ToLower()}s.FirstOrDefault(u => u.Id == id);");
            code.AppendLine($"            return Task.FromResult({serviceName.ToLower()});");
            code.AppendLine($"        }}");
            code.AppendLine($"        ");
            code.AppendLine($"        public Task<long> Create{serviceName}Async({serviceName}Dto {serviceName.ToLower()})");
            code.AppendLine($"        {{");
            code.AppendLine($"            {serviceName.ToLower()}.Id = _{serviceName.ToLower()}s.Max(u => u.Id) + 1;");
            code.AppendLine($"            _{serviceName.ToLower()}s.Add({serviceName.ToLower()});");
            code.AppendLine($"            return Task.FromResult({serviceName.ToLower()}.Id);");
            code.AppendLine($"        }}");
            code.AppendLine($"        ");
            code.AppendLine($"        public Task<bool> Update{serviceName}Async(long id, {serviceName}Dto {serviceName.ToLower()})");
            code.AppendLine($"        {{");
            code.AppendLine($"            var existing{serviceName} = _{serviceName.ToLower()}s.FirstOrDefault(u => u.Id == id);");
            code.AppendLine($"            if (existing{serviceName} != null)");
            code.AppendLine($"            {{");
            code.AppendLine($"                existing{serviceName}.Name = {serviceName.ToLower()}.Name;");
            code.AppendLine($"                return Task.FromResult(true);");
            code.AppendLine($"            }}");
            code.AppendLine($"            return Task.FromResult(false);");
            code.AppendLine($"        }}");
            code.AppendLine($"        ");
            code.AppendLine($"        public Task<bool> Delete{serviceName}Async(long id)");
            code.AppendLine($"        {{");
            code.AppendLine($"            var {serviceName.ToLower()} = _{serviceName.ToLower()}s.FirstOrDefault(u => u.Id == id);");
            code.AppendLine($"            if ({serviceName.ToLower()} != null)");
            code.AppendLine($"            {{");
            code.AppendLine($"                _{serviceName.ToLower()}s.Remove({serviceName.ToLower()});");
            code.AppendLine($"                return Task.FromResult(true);");
            code.AppendLine($"            }}");
            code.AppendLine($"            return Task.FromResult(false);");
            code.AppendLine($"        }}");
            code.AppendLine($"    }}");
            code.AppendLine($"}}");
            
            return code.ToString();
        }

        private string GenerateDtoCode(string serviceName)
        {
            var code = new StringBuilder();
            
            code.AppendLine($"namespace {serviceName}.Services");
            code.AppendLine($"{{");
            code.AppendLine($"    public class {serviceName}Dto");
            code.AppendLine($"    {{");
            code.AppendLine($"        public long Id {{ get; set; }}");
            code.AppendLine($"        public string Name {{ get; set; }}");
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
            var rootCommand = new RootCommand("Silky 代码生成工具");

            // 生成命令
            var generateCommand = new Command("generate", "生成代码");
            var typeOption = new Option<string>("--type", "生成类型（api/impl/dto）");
            var serviceOption = new Option<string>("--service", "服务名称");
            var outputOption = new Option<string>("--output", "输出目录");

            generateCommand.AddOption(typeOption);
            generateCommand.AddOption(serviceOption);
            generateCommand.AddOption(outputOption);

            generateCommand.Handler = CommandHandler.Create<string, string, string>(async (type, service, output) =>
            {
                string code;
                string fileName;
                
                switch (type?.ToLower())
                {
                    case "api":
                        code = await _codeGeneratorService.GenerateApiAsync(service);
                        fileName = $"I{service}Service.cs";
                        break;
                    case "impl":
                        code = await _codeGeneratorService.GenerateImplAsync(service);
                        fileName = $"{service}Service.cs";
                        break;
                    case "dto":
                        code = await _codeGeneratorService.GenerateDtoAsync(service);
                        fileName = $"{service}Dto.cs";
                        break;
                    default:
                        Console.WriteLine($"无效的生成类型: {type}，支持的类型: api, impl, dto");
                        return;
                }

                if (!string.IsNullOrEmpty(output))
                {
                    var outputPath = Path.Combine(output, fileName);
                    await _codeGeneratorService.SaveGeneratedCodeAsync(code, outputPath);
                    Console.WriteLine($"代码已保存到: {outputPath}");
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
