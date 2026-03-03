#:sdk Microsoft.NET.Sdk
#:package Silky.Core@3.3.0
#:package Silky.Http.Core@3.3.0
#:package Silky.Rpc@3.3.0
#:package Silky.Registry@3.3.0
#:package Silky.Swagger@3.3.0
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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Silky.Core;
using Silky.Core.Extensions;
using Silky.Core.Modularity;
using Silky.Http.Core;
using Silky.Rpc;
using Silky.Rpc.Extensions;
using Silky.Rpc.Runtime.Server;
using Silky.Registry;
using Silky.Swagger;

namespace SilkySkill
{
    // 服务信息
    public class ServiceInfo
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string Version { get; set; }
        public bool IsHealthy { get; set; }
    }

    // 服务注册与发现服务
    public interface IRegistryService
    {
        Task<List<ServiceInfo>> GetServicesAsync();
        Task<ServiceInfo> GetServiceAsync(string serviceName);
        Task<bool> CheckServiceHealthAsync(string serviceName);
    }

    // 服务注册与发现服务实现
    public class RegistryService : IRegistryService
    {
        private readonly ILogger<RegistryService> _logger;

        public RegistryService(ILogger<RegistryService> logger)
        {
            _logger = logger;
        }

        public Task<List<ServiceInfo>> GetServicesAsync()
        {
            // 模拟获取服务列表
            var services = new List<ServiceInfo>
            {
                new ServiceInfo
                {
                    Name = "UserService",
                    Address = "127.0.0.1",
                    Port = 8001,
                    Version = "1.0.0",
                    IsHealthy = true
                },
                new ServiceInfo
                {
                    Name = "OrderService",
                    Address = "127.0.0.1",
                    Port = 8002,
                    Version = "1.0.0",
                    IsHealthy = true
                },
                new ServiceInfo
                {
                    Name = "ProductService",
                    Address = "127.0.0.1",
                    Port = 8003,
                    Version = "1.0.0",
                    IsHealthy = false
                }
            };

            _logger.LogInformation($"获取到 {services.Count} 个服务");
            return Task.FromResult(services);
        }

        public async Task<ServiceInfo> GetServiceAsync(string serviceName)
        {
            var services = await GetServicesAsync();
            var service = services.FirstOrDefault(s => s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
            
            if (service == null)
            {
                _logger.LogWarning($"未找到服务: {serviceName}");
            }
            else
            {
                _logger.LogInformation($"找到服务: {service.Name}, 地址: {service.Address}:{service.Port}");
            }
            
            return service;
        }

        public async Task<bool> CheckServiceHealthAsync(string serviceName)
        {
            var service = await GetServiceAsync(serviceName);
            var isHealthy = service?.IsHealthy ?? false;
            
            _logger.LogInformation($"服务 {serviceName} 健康状态: {isHealthy}");
            return isHealthy;
        }
    }

    // 项目创建服务
    public interface IProjectCreatorService
    {
        Task<bool> CreateServiceProjectAsync(string name, string outputPath);
        Task<bool> CreateGatewayProjectAsync(string name, string outputPath);
        Task<bool> CreateModuleProjectAsync(string name, string outputPath);
    }

    // 项目创建服务实现
    public class ProjectCreatorService : IProjectCreatorService
    {
        private readonly ILogger<ProjectCreatorService> _logger;

        public ProjectCreatorService(ILogger<ProjectCreatorService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> CreateServiceProjectAsync(string name, string outputPath)
        {
            try
            {
                _logger.LogInformation($"创建服务项目: {name}, 输出路径: {outputPath}");
                
                // 确保输出目录存在
                var projectPath = Path.Combine(outputPath, name);
                Directory.CreateDirectory(projectPath);
                
                // 创建项目文件
                await CreateProjectFileAsync(projectPath, name, "service");
                
                // 创建服务接口
                await CreateServiceInterfaceAsync(projectPath, name);
                
                // 创建服务实现
                await CreateServiceImplementationAsync(projectPath, name);
                
                // 创建数据传输对象
                await CreateDtoAsync(projectPath, name);
                
                // 创建启动文件
                await CreateStartupFileAsync(projectPath, name);
                
                _logger.LogInformation($"服务项目 {name} 创建成功");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"创建服务项目 {name} 失败");
                return false;
            }
        }

        public async Task<bool> CreateGatewayProjectAsync(string name, string outputPath)
        {
            try
            {
                _logger.LogInformation($"创建网关项目: {name}, 输出路径: {outputPath}");
                
                // 确保输出目录存在
                var projectPath = Path.Combine(outputPath, name);
                Directory.CreateDirectory(projectPath);
                
                // 创建项目文件
                await CreateProjectFileAsync(projectPath, name, "gateway");
                
                // 创建启动文件
                await CreateGatewayStartupFileAsync(projectPath, name);
                
                _logger.LogInformation($"网关项目 {name} 创建成功");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"创建网关项目 {name} 失败");
                return false;
            }
        }

        public async Task<bool> CreateModuleProjectAsync(string name, string outputPath)
        {
            try
            {
                _logger.LogInformation($"创建模块项目: {name}, 输出路径: {outputPath}");
                
                // 确保输出目录存在
                var projectPath = Path.Combine(outputPath, name);
                Directory.CreateDirectory(projectPath);
                
                // 创建项目文件
                await CreateProjectFileAsync(projectPath, name, "module");
                
                // 创建模块类
                await CreateModuleClassAsync(projectPath, name);
                
                _logger.LogInformation($"模块项目 {name} 创建成功");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"创建模块项目 {name} 失败");
                return false;
            }
        }

        private async Task CreateProjectFileAsync(string projectPath, string name, string type)
        {
            var projectFileContent = $"<Project Sdk=\"Microsoft.NET.Sdk.Web\">\n\n  <PropertyGroup>\n    <TargetFramework>net10.0</TargetFramework>\n    <Nullable>enable</Nullable>\n    <ImplicitUsings>enable</ImplicitUsings>\n  </PropertyGroup>\n\n  <ItemGroup>\n    <PackageReference Include=\"Silky.Core\" Version=\"3.3.0\" />\n    <PackageReference Include=\"Silky.Http.Core\" Version=\"3.3.0\" />\n    <PackageReference Include=\"Silky.Rpc\" Version=\"3.3.0\" />\n    <PackageReference Include=\"Silky.Registry\" Version=\"3.3.0\" />\n    <PackageReference Include=\"Silky.Swagger\" Version=\"3.3.0\" />\n  </ItemGroup>\n\n</Project>";
            
            var projectFilePath = Path.Combine(projectPath, $"{name}.csproj");
            await File.WriteAllTextAsync(projectFilePath, projectFileContent);
        }

        private async Task CreateServiceInterfaceAsync(string projectPath, string name)
        {
            var interfaceContent = $"using System.Collections.Generic;\nusing System.Threading.Tasks;\nusing Silky.Http.Core.Attributes;\nusing Silky.Rpc.Routing;\n\nnamespace {name}.Services\n{{\n    [ServiceRoute]\n    public interface I{name}Service\n    {{\n        [HttpGet(\"{name.ToLower()}\")]\n        Task<List<{name}Dto>> Get{name}sAsync();\n        \n        [HttpGet(\"{name.ToLower()}/{id}\")]\n        Task<{name}Dto> Get{name}ByIdAsync(long id);\n        \n        [HttpPost(\"{name.ToLower()}\")]\n        Task<long> Create{name}Async({name}Dto {name.ToLower()});\n        \n        [HttpPut(\"{name.ToLower()}/{id}\")]\n        Task<bool> Update{name}Async(long id, {name}Dto {name.ToLower()});\n        \n        [HttpDelete(\"{name.ToLower()}/{id}\")]\n        Task<bool> Delete{name}Async(long id);\n    }}\n}}";
            
            var interfacePath = Path.Combine(projectPath, "Services", "Interfaces");
            Directory.CreateDirectory(interfacePath);
            var interfaceFilePath = Path.Combine(interfacePath, $"I{name}Service.cs");
            await File.WriteAllTextAsync(interfaceFilePath, interfaceContent);
        }

        private async Task CreateServiceImplementationAsync(string projectPath, string name)
        {
            var implementationContent = $"using System.Collections.Generic;\nusing System.Linq;\nusing System.Threading.Tasks;\n\nnamespace {name}.Services.Impl\n{{\n    public class {name}Service : I{name}Service\n    {{\n        private static readonly List<{name}Dto> _{name.ToLower()}s = new()\n        {{\n            new {name}Dto {{ Id = 1, Name = \"测试{name}\" }}\n        }};\n        \n        public Task<List<{name}Dto>> Get{name}sAsync()\n        {{\n            return Task.FromResult(_{name.ToLower()}s);\n        }}\n        \n        public Task<{name}Dto> Get{name}ByIdAsync(long id)\n        {{\n            var {name.ToLower()} = _{name.ToLower()}s.FirstOrDefault(u => u.Id == id);\n            return Task.FromResult({name.ToLower()});\n        }}\n        \n        public Task<long> Create{name}Async({name}Dto {name.ToLower()})\n        {{\n            {name.ToLower()}.Id = _{name.ToLower()}s.Max(u => u.Id) + 1;\n            _{name.ToLower()}s.Add({name.ToLower()});\n            return Task.FromResult({name.ToLower()}.Id);\n        }}\n        \n        public Task<bool> Update{name}Async(long id, {name}Dto {name.ToLower()})\n        {{\n            var existing{name} = _{name.ToLower()}s.FirstOrDefault(u => u.Id == id);\n            if (existing{name} != null)\n            {{\n                existing{name}.Name = {name.ToLower()}.Name;\n                return Task.FromResult(true);\n            }}\n            return Task.FromResult(false);\n        }}\n        \n        public Task<bool> Delete{name}Async(long id)\n        {{\n            var {name.ToLower()} = _{name.ToLower()}s.FirstOrDefault(u => u.Id == id);\n            if ({name.ToLower()} != null)\n            {{\n                _{name.ToLower()}s.Remove({name.ToLower()});\n                return Task.FromResult(true);\n            }}\n            return Task.FromResult(false);\n        }}\n    }}\n}}";
            
            var implementationPath = Path.Combine(projectPath, "Services", "Impl");
            Directory.CreateDirectory(implementationPath);
            var implementationFilePath = Path.Combine(implementationPath, $"{name}Service.cs");
            await File.WriteAllTextAsync(implementationFilePath, implementationContent);
        }

        private async Task CreateDtoAsync(string projectPath, string name)
        {
            var dtoContent = $"namespace {name}.Services\n{{\n    public class {name}Dto\n    {{\n        public long Id {{ get; set; }}\n        public string Name {{ get; set; }}\n    }}\n}}";
            
            var dtoPath = Path.Combine(projectPath, "Services");
            Directory.CreateDirectory(dtoPath);
            var dtoFilePath = Path.Combine(dtoPath, $"{name}Dto.cs");
            await File.WriteAllTextAsync(dtoFilePath, dtoContent);
        }

        private async Task CreateStartupFileAsync(string projectPath, string name)
        {
            var startupContent = $"using Microsoft.AspNetCore.Builder;\nusing Microsoft.Extensions.Configuration;\nusing Microsoft.Extensions.DependencyInjection;\n\nnamespace {name}\n{{\n    public class Program\n    {{\n        public static void Main(string[] args)\n        {{\n            var builder = WebApplication.CreateBuilder(args);\n            \n            // 注册 Silky 服务\n            builder.Services.AddSilkyServices(builder.Configuration, options =>\n            {{\n                options.AddRegistryCenter();\n                options.AddSwaggerDocument();\n                options.AddMessagePackSerializer();\n            }});\n            \n            var app = builder.Build();\n            \n            // 配置中间件\n            app.UseSilkyWebHost();\n            app.Run();\n        }}\n    }}\n}}";
            
            var startupFilePath = Path.Combine(projectPath, "Program.cs");
            await File.WriteAllTextAsync(startupFilePath, startupContent);
        }

        private async Task CreateGatewayStartupFileAsync(string projectPath, string name)
        {
            var startupContent = $"using Microsoft.AspNetCore.Builder;\nusing Microsoft.Extensions.Configuration;\nusing Microsoft.Extensions.DependencyInjection;\n\nnamespace {name}\n{{\n    public class Program\n    {{\n        public static void Main(string[] args)\n        {{\n            var builder = WebApplication.CreateBuilder(args);\n            \n            // 注册 Silky 网关服务\n            builder.Services.AddSilkyGateway(builder.Configuration, options =>\n            {{\n                options.AddRegistryCenter();\n                options.AddSwaggerDocument();\n            }});\n            \n            var app = builder.Build();\n            \n            // 配置中间件\n            app.UseSilkyWebHost();\n            app.Run();\n        }}\n    }}\n}}";
            
            var startupFilePath = Path.Combine(projectPath, "Program.cs");
            await File.WriteAllTextAsync(startupFilePath, startupContent);
        }

        private async Task CreateModuleClassAsync(string projectPath, string name)
        {
            var moduleContent = $"using Silky.Core.Modularity;\n\nnamespace {name}\n{{\n    [DependsOn(typeof(SilkyHttpCoreModule))]\n    [DependsOn(typeof(SilkyRpcModule))]\n    public class {name}Module : SilkyModule\n    {{\n        public override void ConfigureServices(ServiceConfigurationContext context)\n        {{\n            // 配置服务\n        }}\n        \n        public override void Configure(ApplicationConfigureContext context)\n        {{\n            // 配置应用\n        }}\n    }}\n}}";
            
            var moduleFilePath = Path.Combine(projectPath, $"{name}Module.cs");
            await File.WriteAllTextAsync(moduleFilePath, moduleContent);
        }
    }

    // 项目构建服务
    public interface IProjectBuilderService
    {
        Task<bool> BuildProjectAsync(string projectPath, string configuration);
    }

    // 项目构建服务实现
    public class ProjectBuilderService : IProjectBuilderService
    {
        private readonly ILogger<ProjectBuilderService> _logger;

        public ProjectBuilderService(ILogger<ProjectBuilderService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> BuildProjectAsync(string projectPath, string configuration)
        {
            try
            {
                _logger.LogInformation($"构建项目: {projectPath}, 配置: {configuration}");
                
                // 模拟构建过程
                await Task.Delay(2000);
                
                _logger.LogInformation($"项目构建成功");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"项目构建失败");
                return false;
            }
        }
    }

    // 项目启动服务
    public interface IProjectRunnerService
    {
        Task<bool> RunProjectAsync(string projectPath, string environment);
    }

    // 项目启动服务实现
    public class ProjectRunnerService : IProjectRunnerService
    {
        private readonly ILogger<ProjectRunnerService> _logger;

        public ProjectRunnerService(ILogger<ProjectRunnerService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> RunProjectAsync(string projectPath, string environment)
        {
            try
            {
                _logger.LogInformation($"启动项目: {projectPath}, 环境: {environment}");
                
                // 模拟启动过程
                await Task.Delay(1000);
                
                _logger.LogInformation($"项目启动成功");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"项目启动失败");
                return false;
            }
        }
    }

    // 命令行工具
    public class SilkyCli
    {
        private readonly IProjectCreatorService _projectCreatorService;
        private readonly IProjectBuilderService _projectBuilderService;
        private readonly IProjectRunnerService _projectRunnerService;
        private readonly IRegistryService _registryService;
        private readonly ILogger<SilkyCli> _logger;

        public SilkyCli(
            IProjectCreatorService projectCreatorService,
            IProjectBuilderService projectBuilderService,
            IProjectRunnerService projectRunnerService,
            IRegistryService registryService,
            ILogger<SilkyCli> logger)
        {
            _projectCreatorService = projectCreatorService;
            _projectBuilderService = projectBuilderService;
            _projectRunnerService = projectRunnerService;
            _registryService = registryService;
            _logger = logger;
        }

        public async Task<int> RunAsync(string[] args)
        {
            // 创建根命令
            var rootCommand = new RootCommand("Silky 命令行工具");

            // 创建命令
            var createCommand = new Command("create", "创建 silky 项目");
            var typeOption = new Option<string>("--type", "项目类型（service/gateway/module）");
            var nameOption = new Option<string>("--name", "项目名称");
            var outputOption = new Option<string>("--output", "输出目录");

            createCommand.AddOption(typeOption);
            createCommand.AddOption(nameOption);
            createCommand.AddOption(outputOption);

            createCommand.Handler = CommandHandler.Create<string, string, string>(async (type, name, output) =>
            {
                bool success = false;
                
                switch (type?.ToLower())
                {
                    case "service":
                        success = await _projectCreatorService.CreateServiceProjectAsync(name, output);
                        break;
                    case "gateway":
                        success = await _projectCreatorService.CreateGatewayProjectAsync(name, output);
                        break;
                    case "module":
                        success = await _projectCreatorService.CreateModuleProjectAsync(name, output);
                        break;
                    default:
                        Console.WriteLine($"无效的项目类型: {type}，支持的类型: service, gateway, module");
                        return;
                }
                
                if (success)
                {
                    Console.WriteLine($"项目创建成功: {name}");
                }
                else
                {
                    Console.WriteLine($"项目创建失败: {name}");
                }
            });

            // 启动命令
            var startCommand = new Command("start", "启动 silky 服务");
            var projectOption = new Option<string>("--project", "项目路径");
            var environmentOption = new Option<string>("--environment", "环境变量");

            startCommand.AddOption(projectOption);
            startCommand.AddOption(environmentOption);

            startCommand.Handler = CommandHandler.Create<string, string>(async (project, environment) =>
            {
                var success = await _projectRunnerService.RunProjectAsync(project, environment);
                if (success)
                {
                    Console.WriteLine($"项目启动成功: {project}");
                }
                else
                {
                    Console.WriteLine($"项目启动失败: {project}");
                }
            });

            // 构建命令
            var buildCommand = new Command("build", "构建 silky 项目");
            var buildProjectOption = new Option<string>("--project", "项目路径");
            var configurationOption = new Option<string>("--configuration", "构建配置");

            buildCommand.AddOption(buildProjectOption);
            buildCommand.AddOption(configurationOption);

            buildCommand.Handler = CommandHandler.Create<string, string>(async (project, configuration) =>
            {
                var success = await _projectBuilderService.BuildProjectAsync(project, configuration);
                if (success)
                {
                    Console.WriteLine($"项目构建成功: {project}");
                }
                else
                {
                    Console.WriteLine($"项目构建失败: {project}");
                }
            });

            // 注册命令
            var registryCommand = new Command("registry", "服务注册管理");
            var listOption = new Option<bool>("--list", "列出所有注册的服务");
            var healthOption = new Option<string>("--health", "检查服务健康状态");

            registryCommand.AddOption(listOption);
            registryCommand.AddOption(healthOption);

            registryCommand.Handler = CommandHandler.Create<bool, string>(async (list, health) =>
            {
                if (list)
                {
                    var services = await _registryService.GetServicesAsync();
                    Console.WriteLine("注册的服务:");
                    foreach (var service in services)
                    {
                        Console.WriteLine($"  名称: {service.Name}, 地址: {service.Address}:{service.Port}, 版本: {service.Version}, 健康状态: {(service.IsHealthy ? "健康" : "不健康")}");
                    }
                }
                else if (!string.IsNullOrEmpty(health))
                {
                    var isHealthy = await _registryService.CheckServiceHealthAsync(health);
                    Console.WriteLine($"服务 {health} 健康状态: {(isHealthy ? "健康" : "不健康")}");
                }
                else
                {
                    Console.WriteLine("请指定 --list 或 --health <serviceName>");
                }
            });

            // 添加命令到根命令
            rootCommand.AddCommand(createCommand);
            rootCommand.AddCommand(startCommand);
            rootCommand.AddCommand(buildCommand);
            rootCommand.AddCommand(registryCommand);

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
                .AddSingleton<IProjectCreatorService, ProjectCreatorService>()
                .AddSingleton<IProjectBuilderService, ProjectBuilderService>()
                .AddSingleton<IProjectRunnerService, ProjectRunnerService>()
                .AddSingleton<IRegistryService, RegistryService>()
                .AddSingleton<SilkyCli>()
                .BuildServiceProvider();

            // 获取命令行工具
            var cli = serviceProvider.GetRequiredService<SilkyCli>();

            // 运行命令
            return await cli.RunAsync(args);
        }
    }
}
