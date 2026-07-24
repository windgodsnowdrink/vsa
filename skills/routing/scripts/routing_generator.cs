#:sdk Microsoft.NET.Sdk
#:package System.CommandLine@2.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Microsoft.Extensions.Configuration@8.0.0
#:package System.Text.Json@8.0.0
#:package Newtonsoft.Json@13.0.3
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
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Routing.Generator
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // 配置依赖注入
            var serviceProvider = ConfigureServices();
            
            // 创建命令行根命令
            var rootCommand = new RootCommand("Routing 代码生成工具");
            
            // 创建生成命令
            var generateCommand = new Command("generate", "生成路由代码");
            var frameworkOption = new Option<string>("--framework", () => "aspnetcore", "目标框架");
            var inputOption = new Option<string>("--input", "输入文件路径");
            var outputOption = new Option<string>("--output", "输出文件路径");
            var formatOption = new Option<string>("--format", () => "csharp", "输出格式");
            generateCommand.AddOption(frameworkOption);
            generateCommand.AddOption(inputOption);
            generateCommand.AddOption(outputOption);
            generateCommand.AddOption(formatOption);
            generateCommand.SetHandler(async (context) =>
            {
                var framework = context.ParseResult.GetValueForOption(frameworkOption);
                var input = context.ParseResult.GetValueForOption(inputOption);
                var output = context.ParseResult.GetValueForOption(outputOption);
                var format = context.ParseResult.GetValueForOption(formatOption);
                var cancellationToken = context.GetCancellationToken();
                
                await HandleGenerateCommand(serviceProvider, framework, input, output, format, cancellationToken);
            });
            
            // 创建模板命令
            var templateCommand = new Command("template", "管理代码模板");
            var templateSubCommand = new Command("list", "列出可用模板");
            templateCommand.AddCommand(templateSubCommand);
            templateSubCommand.SetHandler(async (context) =>
            {
                await HandleListTemplatesCommand(serviceProvider, context.GetCancellationToken());
            });
            
            // 创建验证命令
            var validateCommand = new Command("validate", "验证路由配置");
            var validateInputOption = new Option<string>("--input", "输入文件路径");
            validateCommand.AddOption(validateInputOption);
            validateCommand.SetHandler(async (context) =>
            {
                var input = context.ParseResult.GetValueForOption(validateInputOption);
                var cancellationToken = context.GetCancellationToken();
                
                await HandleValidateCommand(serviceProvider, input, cancellationToken);
            });
            
            // 添加命令到根命令
            rootCommand.AddCommand(generateCommand);
            rootCommand.AddCommand(templateCommand);
            rootCommand.AddCommand(validateCommand);
            
            // 执行命令
            return await rootCommand.InvokeAsync(args);
        }
        
        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            
            // 配置配置管理
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            
            services.AddSingleton<IConfiguration>(configuration);
            
            // 注册服务
            services.AddSingleton<IRouteGenerator, RouteGenerator>();
            services.AddSingleton<ITemplateManager, TemplateManager>();
            services.AddSingleton<IConfigurationValidator, ConfigurationValidator>();
            
            // 注册代码生成器
            services.AddSingleton<ICodeGenerator, AspNetCoreGenerator>();
            services.AddSingleton<ICodeGenerator, FastEndpointsGenerator>();
            services.AddSingleton<ICodeGenerator, MinimalApiGenerator>();
            
            return services.BuildServiceProvider();
        }
        
        private static async Task HandleGenerateCommand(IServiceProvider serviceProvider, string framework, string input, string output, string format, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("错误: 必须指定输入文件路径");
                return;
            }
            
            if (!File.Exists(input))
            {
                Console.WriteLine($"错误: 输入文件不存在: {input}");
                return;
            }
            
            if (string.IsNullOrEmpty(output))
            {
                Console.WriteLine("错误: 必须指定输出文件路径");
                return;
            }
            
            try
            {
                var routeGenerator = serviceProvider.GetRequiredService<IRouteGenerator>();
                var code = await routeGenerator.GenerateAsync(framework, input, output, format, cancellationToken);
                
                Console.WriteLine($"成功生成路由代码到: {output}");
                Console.WriteLine($"生成的代码行数: {code.Split('\n').Length}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"生成代码时发生错误: {ex.Message}");
            }
        }
        
        private static async Task HandleListTemplatesCommand(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            try
            {
                var templateManager = serviceProvider.GetRequiredService<ITemplateManager>();
                var templates = await templateManager.GetTemplatesAsync(cancellationToken);
                
                Console.WriteLine("可用的代码模板:");
                Console.WriteLine("-" + new string('-', 80) + "-");
                
                foreach (var template in templates)
                {
                    Console.WriteLine($"模板: {template.Name}");
                    Console.WriteLine($"描述: {template.Description}");
                    Console.WriteLine($"框架: {template.Framework}");
                    Console.WriteLine($"格式: {template.Format}");
                    Console.WriteLine("-" + new string('-', 80) + "-");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"列出模板时发生错误: {ex.Message}");
            }
        }
        
        private static async Task HandleValidateCommand(IServiceProvider serviceProvider, string input, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("错误: 必须指定输入文件路径");
                return;
            }
            
            if (!File.Exists(input))
            {
                Console.WriteLine($"错误: 输入文件不存在: {input}");
                return;
            }
            
            try
            {
                var validator = serviceProvider.GetRequiredService<IConfigurationValidator>();
                var result = await validator.ValidateAsync(input, cancellationToken);
                
                if (result.IsValid)
                {
                    Console.WriteLine("配置验证成功!");
                    Console.WriteLine($"找到 {result.Routes.Count} 个路由");
                    Console.WriteLine($"找到 {result.Middlewares.Count} 个中间件");
                }
                else
                {
                    Console.WriteLine("配置验证失败:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"- {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"验证配置时发生错误: {ex.Message}");
            }
        }
    }
    
    // 服务接口
    public interface IRouteGenerator
    {
        Task<string> GenerateAsync(string framework, string inputPath, string outputPath, string format, CancellationToken cancellationToken = default);
    }
    
    public interface ITemplateManager
    {
        Task<IEnumerable<TemplateInfo>> GetTemplatesAsync(CancellationToken cancellationToken = default);
        Task<string> GetTemplateAsync(string name, CancellationToken cancellationToken = default);
    }
    
    public interface IConfigurationValidator
    {
        Task<ValidationResult> ValidateAsync(string inputPath, CancellationToken cancellationToken = default);
    }
    
    public interface ICodeGenerator
    {
        string Framework { get; }
        string Generate(IEnumerable<RouteConfig> routes, IEnumerable<MiddlewareConfig> middlewares);
    }
    
    // 数据结构
    public class TemplateInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Framework { get; set; }
        public string Format { get; set; }
    }
    
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<RouteConfig> Routes { get; set; } = new List<RouteConfig>();
        public List<MiddlewareConfig> Middlewares { get; set; } = new List<MiddlewareConfig>();
    }
    
    public class RouteConfig
    {
        public string Path { get; set; }
        public string Method { get; set; }
        public string Handler { get; set; }
        public List<RouteParameter> Parameters { get; set; } = new List<RouteParameter>();
        public List<MiddlewareConfig> Middlewares { get; set; } = new List<MiddlewareConfig>();
    }
    
    public class MiddlewareConfig
    {
        public string Name { get; set; }
        public string Handler { get; set; }
        public int Order { get; set; }
    }
    
    public class RouteParameter
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }
        public bool Required { get; set; }
        public string DefaultValue { get; set; }
    }
    
    // 实现类
    public class RouteGenerator : IRouteGenerator
    {
        private readonly IEnumerable<ICodeGenerator> _codeGenerators;
        private readonly ITemplateManager _templateManager;
        
        public RouteGenerator(IEnumerable<ICodeGenerator> codeGenerators, ITemplateManager templateManager)
        {
            _codeGenerators = codeGenerators;
            _templateManager = templateManager;
        }
        
        public async Task<string> GenerateAsync(string framework, string inputPath, string outputPath, string format, CancellationToken cancellationToken = default)
        {
            // 读取输入文件
            var inputContent = await File.ReadAllTextAsync(inputPath, cancellationToken);
            
            // 解析配置
            var config = ParseConfiguration(inputContent);
            
            // 选择代码生成器
            var generator = _codeGenerators.FirstOrDefault(g => g.Framework.Equals(framework, StringComparison.OrdinalIgnoreCase));
            if (generator == null)
            {
                throw new InvalidOperationException($"不支持的框架: {framework}");
            }
            
            // 生成代码
            var code = generator.Generate(config.Routes, config.Middlewares);
            
            // 写入输出文件
            await File.WriteAllTextAsync(outputPath, code, cancellationToken);
            
            return code;
        }
        
        private (List<RouteConfig> Routes, List<MiddlewareConfig> Middlewares) ParseConfiguration(string content)
        {
            // 尝试 JSON 解析
            try
            {
                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var config = JsonSerializer.Deserialize<Configuration>(content, options);
                return (config.Routes, config.Middlewares);
            }
            catch
            {
                // 尝试 YAML 解析（简化版）
                return ParseYamlConfiguration(content);
            }
        }
        
        private (List<RouteConfig> Routes, List<MiddlewareConfig> Middlewares) ParseYamlConfiguration(string content)
        {
            var routes = new List<RouteConfig>();
            var middlewares = new List<MiddlewareConfig>();
            
            // 这里实现简化的 YAML 解析
            // 实际项目中应该使用专业的 YAML 解析库
            
            return (routes, middlewares);
        }
        
        private class Configuration
        {
            public List<RouteConfig> Routes { get; set; } = new List<RouteConfig>();
            public List<MiddlewareConfig> Middlewares { get; set; } = new List<MiddlewareConfig>();
        }
    }
    
    public class TemplateManager : ITemplateManager
    {
        public Task<IEnumerable<TemplateInfo>> GetTemplatesAsync(CancellationToken cancellationToken = default)
        {
            var templates = new List<TemplateInfo>
            {
                new TemplateInfo
                {
                    Name = "aspnetcore-controller",
                    Description = "ASP.NET Core Controller 模板",
                    Framework = "aspnetcore",
                    Format = "csharp"
                },
                new TemplateInfo
                {
                    Name = "fastendpoints-endpoint",
                    Description = "FastEndpoints Endpoint 模板",
                    Framework = "fastendpoints",
                    Format = "csharp"
                },
                new TemplateInfo
                {
                    Name = "minimalapi-endpoint",
                    Description = "Minimal API 端点模板",
                    Framework = "minimalapi",
                    Format = "csharp"
                }
            };
            
            return Task.FromResult<IEnumerable<TemplateInfo>>(templates);
        }
        
        public Task<string> GetTemplateAsync(string name, CancellationToken cancellationToken = default)
        {
            // 根据模板名称返回对应的模板内容
            // 实际项目中应该从文件或数据库加载模板
            var templateContent = "";
            
            switch (name)
            {
                case "aspnetcore-controller":
                    templateContent = GetAspNetCoreControllerTemplate();
                    break;
                case "fastendpoints-endpoint":
                    templateContent = GetFastEndpointsEndpointTemplate();
                    break;
                case "minimalapi-endpoint":
                    templateContent = GetMinimalApiEndpointTemplate();
                    break;
            }
            
            return Task.FromResult(templateContent);
        }
        
        private string GetAspNetCoreControllerTemplate()
        {
            return @"using Microsoft.AspNetCore.Mvc;

namespace {{Namespace}}
{
    [ApiController]
    [Route(""{{RoutePrefix}}"")]
    public class {{ControllerName}}Controller : ControllerBase
    {
        {{Actions}}
    }
}";
        }
        
        private string GetFastEndpointsEndpointTemplate()
        {
            return @"using FastEndpoints;

namespace {{Namespace}}
{
    public class {{EndpointName}}Endpoint : Endpoint<{{RequestType}}, {{ResponseType}}>
    {
        public override void Configure()
        {
            {{Configuration}}
        }
        
        public override async Task HandleAsync({{RequestType}} req, CancellationToken ct)
        {
            {{Handler}}
        }
    }
}";
        }
        
        private string GetMinimalApiEndpointTemplate()
        {
            return @"using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace {{Namespace}}
{
    public static class {{EndpointName}}Extensions
    {
        public static IEndpointRouteBuilder Map{{EndpointName}}(this IEndpointRouteBuilder app)
        {
            {{MapEndpoints}}
            return app;
        }
    }
}";
        }
    }
    
    public class ConfigurationValidator : IConfigurationValidator
    {
        public Task<ValidationResult> ValidateAsync(string inputPath, CancellationToken cancellationToken = default)
        {
            var result = new ValidationResult();
            
            try
            {
                var content = File.ReadAllText(inputPath);
                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var config = JsonSerializer.Deserialize<Configuration>(content, options);
                
                // 验证路由
                foreach (var route in config.Routes)
                {
                    if (string.IsNullOrEmpty(route.Path))
                    {
                        result.Errors.Add($"路由缺少路径");
                    }
                    
                    if (string.IsNullOrEmpty(route.Method))
                    {
                        result.Errors.Add($"路由缺少方法: {route.Path}");
                    }
                    
                    if (string.IsNullOrEmpty(route.Handler))
                    {
                        result.Errors.Add($"路由缺少处理程序: {route.Method} {route.Path}");
                    }
                }
                
                result.Routes = config.Routes;
                result.Middlewares = config.Middlewares;
                result.IsValid = result.Errors.Count == 0;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"配置解析错误: {ex.Message}");
                result.IsValid = false;
            }
            
            return Task.FromResult(result);
        }
        
        private class Configuration
        {
            public List<RouteConfig> Routes { get; set; } = new List<RouteConfig>();
            public List<MiddlewareConfig> Middlewares { get; set; } = new List<MiddlewareConfig>();
        }
    }
    
    // 代码生成器实现
    public class AspNetCoreGenerator : ICodeGenerator
    {
        public string Framework => "aspnetcore";
        
        public string Generate(IEnumerable<RouteConfig> routes, IEnumerable<MiddlewareConfig> middlewares)
        {
            var sb = new StringBuilder();
            
            // 生成 using 语句
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("using Microsoft.AspNetCore.Http;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            
            // 生成命名空间
            sb.AppendLine("namespace Routing.Generated");
            sb.AppendLine("{");
            
            // 按控制器分组路由
            var routesByController = routes.GroupBy(r => GetControllerName(r.Handler));
            
            foreach (var group in routesByController)
            {
                var controllerName = group.Key;
                var routePrefix = GetRoutePrefix(group.First().Path);
                
                // 生成控制器类
                sb.AppendLine($"    [ApiController]");
                sb.AppendLine($"    [Route(\"{routePrefix}\")]");
                sb.AppendLine($"    public class {controllerName}Controller : ControllerBase");
                sb.AppendLine("    {");
                
                // 生成操作方法
                foreach (var route in group)
                {
                    var actionName = GetActionName(route.Handler);
                    var httpMethodAttribute = GetHttpMethodAttribute(route.Method);
                    var routeAttribute = GetRouteAttribute(route.Path, routePrefix);
                    var parameters = GetActionParameters(route.Parameters);
                    var returnType = GetReturnType(route.Handler);
                    
                    sb.AppendLine($"        {httpMethodAttribute}");
                    if (!string.IsNullOrEmpty(routeAttribute))
                    {
                        sb.AppendLine($"        {routeAttribute}");
                    }
                    sb.AppendLine($"        public async Task<{returnType}> {actionName}({parameters})");
                    sb.AppendLine("        {");
                    sb.AppendLine($"            // 实现路由处理逻辑");
                    sb.AppendLine($"            // 处理程序: {route.Handler}");
                    sb.AppendLine("            return Ok();");
                    sb.AppendLine("        }");
                    sb.AppendLine();
                }
                
                sb.AppendLine("    }");
                sb.AppendLine();
            }
            
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        private string GetControllerName(string handler)
        {
            // 从处理程序中提取控制器名称
            // 简化实现，实际项目中应该更复杂
            var parts = handler.Split('.');
            return parts.Last().Replace("Controller", "") + "Controller";
        }
        
        private string GetRoutePrefix(string path)
        {
            // 从路径中提取路由前缀
            var parts = path.Split('/');
            if (parts.Length > 1)
            {
                return parts[1];
            }
            return "";
        }
        
        private string GetActionName(string handler)
        {
            // 从处理程序中提取操作名称
            var parts = handler.Split('.');
            return parts.Last();
        }
        
        private string GetHttpMethodAttribute(string method)
        {
            switch (method.ToUpper())
            {
                case "GET":
                    return "[HttpGet]";
                case "POST":
                    return "[HttpPost]";
                case "PUT":
                    return "[HttpPut]";
                case "DELETE":
                    return "[HttpDelete]";
                case "PATCH":
                    return "[HttpPatch]";
                default:
                    return "[HttpGet]";
            }
        }
        
        private string GetRouteAttribute(string path, string prefix)
        {
            if (path.StartsWith($"/{prefix}") && prefix != "")
            {
                var route = path.Substring(prefix.Length + 1);
                if (!string.IsNullOrEmpty(route))
                {
                    return $"[Route(\"{route}\")]";
                }
            }
            return "";
        }
        
        private string GetActionParameters(List<RouteParameter> parameters)
        {
            if (!parameters.Any())
            {
                return "";
            }
            
            var paramList = new List<string>();
            foreach (var param in parameters)
            {
                var sourceAttribute = param.Source switch
                {
                    "path" => "[FromRoute]",
                    "query" => "[FromQuery]",
                    "form" => "[FromForm]",
                    "body" => "[FromBody]",
                    _ => ""
                };
                
                var paramString = $"{sourceAttribute} {param.Type} {param.Name}";
                if (!param.Required && !string.IsNullOrEmpty(param.DefaultValue))
                {
                    paramString += $" = {param.DefaultValue}";
                }
                
                paramList.Add(paramString);
            }
            
            return string.Join(", ", paramList);
        }
        
        private string GetReturnType(string handler)
        {
            // 简化实现，实际项目中应该从处理程序中提取返回类型
            return "IActionResult";
        }
    }
    
    public class FastEndpointsGenerator : ICodeGenerator
    {
        public string Framework => "fastendpoints";
        
        public string Generate(IEnumerable<RouteConfig> routes, IEnumerable<MiddlewareConfig> middlewares)
        {
            var sb = new StringBuilder();
            
            // 生成 using 语句
            sb.AppendLine("using FastEndpoints;");
            sb.AppendLine("using Microsoft.AspNetCore.Http;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            
            // 生成命名空间
            sb.AppendLine("namespace Routing.Generated");
            sb.AppendLine("{");
            
            // 生成每个路由的端点
            foreach (var route in routes)
            {
                var endpointName = GetEndpointName(route.Handler);
                var requestType = GetRequestType(route.Parameters);
                var responseType = "IResult";
                
                // 生成请求类
                if (!string.IsNullOrEmpty(requestType))
                {
                    sb.AppendLine($"    public class {endpointName}Request");
                    sb.AppendLine("    {");
                    foreach (var param in route.Parameters)
                    {
                        if (param.Source != "path")
                        {
                            sb.AppendLine($"        public {param.Type} {param.Name} {{ get; set; }}");
                        }
                    }
                    sb.AppendLine("    }");
                    sb.AppendLine();
                }
                
                // 生成端点类
                sb.AppendLine($"    public class {endpointName}Endpoint : Endpoint<{requestType}, {responseType}>");
                sb.AppendLine("    {");
                sb.AppendLine("        public override void Configure()");
                sb.AppendLine("        {");
                sb.AppendLine($"            Verbs(new[] {{ \"{route.Method}\" }});");
                sb.AppendLine($"            Routes(\"{route.Path}\");");
                
                // 添加中间件
                foreach (var middleware in route.Middlewares)
                {
                    sb.AppendLine($"            Middleware<{middleware.Handler}>();");
                }
                
                sb.AppendLine("        }");
                sb.AppendLine();
                sb.AppendLine("        public override async Task HandleAsync({requestType} req, CancellationToken ct)");
                sb.AppendLine("        {");
                sb.AppendLine($"            // 实现路由处理逻辑");
                sb.AppendLine($"            // 处理程序: {route.Handler}");
                sb.AppendLine("            await SendAsync(Results.Ok());");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine();
            }
            
            // 生成中间件
            foreach (var middleware in middlewares)
            {
                var middlewareName = GetMiddlewareName(middleware.Handler);
                
                sb.AppendLine($"    public class {middlewareName} : IMiddleware");
                sb.AppendLine("    {");
                sb.AppendLine("        public async Task InvokeAsync(HttpContext context, RequestDelegate next)");
                sb.AppendLine("        {");
                sb.AppendLine($"            // 实现中间件逻辑");
                sb.AppendLine($"            // 中间件: {middleware.Name}");
                sb.AppendLine("            await next(context);");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine();
            }
            
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        private string GetEndpointName(string handler)
        {
            var parts = handler.Split('.');
            return parts.Last();
        }
        
        private string GetRequestType(List<RouteParameter> parameters)
        {
            if (parameters.Any())
            {
                return "object";
            }
            return "EmptyRequest";
        }
        
        private string GetMiddlewareName(string handler)
        {
            var parts = handler.Split('.');
            return parts.Last();
        }
    }
    
    public class MinimalApiGenerator : ICodeGenerator
    {
        public string Framework => "minimalapi";
        
        public string Generate(IEnumerable<RouteConfig> routes, IEnumerable<MiddlewareConfig> middlewares)
        {
            var sb = new StringBuilder();
            
            // 生成 using 语句
            sb.AppendLine("using Microsoft.AspNetCore.Builder;");
            sb.AppendLine("using Microsoft.AspNetCore.Http;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            
            // 生成命名空间
            sb.AppendLine("namespace Routing.Generated");
            sb.AppendLine("{");
            sb.AppendLine("    public static class EndpointExtensions");
            sb.AppendLine("    {");
            sb.AppendLine("        public static IEndpointRouteBuilder MapGeneratedEndpoints(this IEndpointRouteBuilder app)");
            sb.AppendLine("        {");
            
            // 生成全局中间件
            foreach (var middleware in middlewares)
            {
                sb.AppendLine($"            // 注册全局中间件: {middleware.Name}");
                sb.AppendLine($"            app.UseMiddleware<{middleware.Handler}>();");
                sb.AppendLine();
            }
            
            // 生成路由
            foreach (var route in routes)
            {
                var endpointName = GetEndpointName(route.Handler);
                var parameters = GetEndpointParameters(route.Parameters);
                var parameterNames = GetParameterNames(route.Parameters);
                
                sb.AppendLine($"            // {route.Method} {route.Path}");
                sb.AppendLine($"            app.Map{route.Method}({route.Path}", async (HttpContext context{parameters}) =>");
                sb.AppendLine("            {");
                
                // 添加路由中间件
                foreach (var middleware in route.Middlewares)
                {
                    sb.AppendLine($"                // 路由中间件: {middleware.Name}");
                }
                
                sb.AppendLine($"                // 实现路由处理逻辑");
                sb.AppendLine($"                // 处理程序: {route.Handler}");
                sb.AppendLine("                return Results.Ok();");
                sb.AppendLine("            });");
                sb.AppendLine();
            }
            
            sb.AppendLine("            return app;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        private string GetEndpointName(string handler)
        {
            var parts = handler.Split('.');
            return parts.Last();
        }
        
        private string GetEndpointParameters(List<RouteParameter> parameters)
        {
            if (!parameters.Any())
            {
                return "";
            }
            
            var paramList = new List<string>();
            foreach (var param in parameters)
            {
                var paramString = $", {param.Type} {param.Name}";
                if (param.Source == "path")
                {
                    paramString += $" = Route.{param.Name}";
                }
                else if (param.Source == "query")
                {
                    paramString += $" = Query.{param.Name}";
                }
                
                paramList.Add(paramString);
            }
            
            return string.Join("", paramList);
        }
        
        private string GetParameterNames(List<RouteParameter> parameters)
        {
            if (!parameters.Any())
            {
                return "";
            }
            
            var names = parameters.Select(p => p.Name).ToList();
            return string.Join(", ", names);
        }
    }
}
