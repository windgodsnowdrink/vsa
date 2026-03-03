#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Swashbuckle.AspNetCore@6.5.0
#:package Microsoft.OpenApi@1.6.0
#:package System.Text.Json@10.0.0
#:package System.Net.Http.Json@10.0.0
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

// 配置选项
public class OpenAPIOptions
{
    public bool Enabled { get; set; } = true;
    public bool EnableSwaggerUI { get; set; } = true;
    public bool EnableScalarUI { get; set; } = true;
    public string ApiTitle { get; set; } = "API";
    public string ApiVersion { get; set; } = "v1";
    public string ApiDescription { get; set; } = "API Description";
    public string ContactName { get; set; } = "Contact";
    public string ContactEmail { get; set; } = "contact@example.com";
    public string LicenseName { get; set; } = "License";
    public string LicenseUrl { get; set; } = "https://example.com/license";
    public bool EnableXmlComments { get; set; } = false;
    public string XmlCommentsPath { get; set; } = string.Empty;
    public bool EnableAuthentication { get; set; } = false;
    public string AuthenticationScheme { get; set; } = "Bearer";
    public bool EnableParallelProcessing { get; set; } = true;
    public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
}

// OpenAPI 服务接口
public interface IOpenAPIService
{
    Task<OpenApiDocument> GenerateOpenApiSpecAsync(CancellationToken cancellationToken = default);
    Task<OpenApiValidationResult> ValidateOpenApiSpecAsync(OpenApiDocument openApiDoc, CancellationToken cancellationToken = default);
    Task<string> GenerateOpenApiJsonAsync(CancellationToken cancellationToken = default);
    Task<string> GenerateOpenApiYamlAsync(CancellationToken cancellationToken = default);
    Task<bool> SaveOpenApiSpecAsync(string filePath, CancellationToken cancellationToken = default);
    Task<IEnumerable<OpenApiOperation>> GetApiOperationsAsync(CancellationToken cancellationToken = default);
}

// OpenAPI 验证结果
public class OpenApiValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public List<string> Warnings { get; set; } = new List<string>();
}

// OpenAPI 文档生成器接口
public interface IOpenApiDocumentGenerator
{
    Task<OpenApiDocument> GenerateDocumentAsync(OpenAPIOptions options, CancellationToken cancellationToken = default);
}

// OpenAPI 验证器接口
public interface IOpenApiValidator
{
    Task<OpenApiValidationResult> ValidateAsync(OpenApiDocument openApiDoc, CancellationToken cancellationToken = default);
}

// OpenAPI 文档生成器实现
public class OpenApiDocumentGenerator : IOpenApiDocumentGenerator
{
    private readonly ILogger<OpenApiDocumentGenerator> _logger;

    public OpenApiDocumentGenerator(ILogger<OpenApiDocumentGenerator> logger)
    {
        _logger = logger;
    }

    public async Task<OpenApiDocument> GenerateDocumentAsync(OpenAPIOptions options, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("开始生成 OpenAPI 文档");
        
        var document = new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Title = options.ApiTitle,
                Version = options.ApiVersion,
                Description = options.ApiDescription,
                Contact = new OpenApiContact
                {
                    Name = options.ContactName,
                    Email = options.ContactEmail
                },
                License = new OpenApiLicense
                {
                    Name = options.LicenseName,
                    Url = new Uri(options.LicenseUrl)
                }
            },
            Paths = new OpenApiPaths(),
            Components = new OpenApiComponents(),
            Tags = new List<OpenApiTag>()
        };

        // 模拟添加一些路径和操作
        AddSamplePaths(document);

        _logger.LogInformation("OpenAPI 文档生成完成");
        return await Task.FromResult(document);
    }

    private void AddSamplePaths(OpenApiDocument document)
    {
        // 添加示例路径：/api/users
        var usersPath = new OpenApiPathItem();
        usersPath.AddOperation(OperationType.Get, new OpenApiOperation
        {
            Summary = "获取用户列表",
            Description = "获取所有用户的列表",
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Description = "成功",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "array",
                                Items = new OpenApiSchema
                                {
                                    Type = "object",
                                    Properties = new Dictionary<string, OpenApiSchema>
                                    {
                                        ["id"] = new OpenApiSchema { Type = "integer", Format = "int32" },
                                        ["name"] = new OpenApiSchema { Type = "string" },
                                        ["email"] = new OpenApiSchema { Type = "string", Format = "email" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });

        usersPath.AddOperation(OperationType.Post, new OpenApiOperation
        {
            Summary = "创建用户",
            Description = "创建一个新用户",
            RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["name"] = new OpenApiSchema { Type = "string" },
                                ["email"] = new OpenApiSchema { Type = "string", Format = "email" }
                            },
                            Required = new HashSet<string> { "name", "email" }
                        }
                    }
                }
            },
            Responses = new OpenApiResponses
            {
                ["201"] = new OpenApiResponse
                {
                    Description = "创建成功",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["id"] = new OpenApiSchema { Type = "integer", Format = "int32" },
                                    ["name"] = new OpenApiSchema { Type = "string" },
                                    ["email"] = new OpenApiSchema { Type = "string", Format = "email" }
                                }
                            }
                        }
                    }
                }
            }
        });

        document.Paths["/api/users"] = usersPath;

        // 添加示例路径：/api/users/{id}
        var userByIdPath = new OpenApiPathItem();
        userByIdPath.AddOperation(OperationType.Get, new OpenApiOperation
        {
            Summary = "获取用户详情",
            Description = "根据 ID 获取用户详情",
            Parameters = new List<OpenApiParameter>
            {
                new OpenApiParameter
                {
                    Name = "id",
                    In = ParameterLocation.Path,
                    Required = true,
                    Schema = new OpenApiSchema { Type = "integer", Format = "int32" }
                }
            },
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Description = "成功",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["id"] = new OpenApiSchema { Type = "integer", Format = "int32" },
                                    ["name"] = new OpenApiSchema { Type = "string" },
                                    ["email"] = new OpenApiSchema { Type = "string", Format = "email" }
                                }
                            }
                        }
                    }
                },
                ["404"] = new OpenApiResponse
                {
                    Description = "用户不存在"
                }
            }
        });

        document.Paths["/api/users/{id}"] = userByIdPath;
    }
}

// OpenAPI 验证器实现
public class OpenApiValidator : IOpenApiValidator
{
    private readonly ILogger<OpenApiValidator> _logger;

    public OpenApiValidator(ILogger<OpenApiValidator> logger)
    {
        _logger = logger;
    }

    public async Task<OpenApiValidationResult> ValidateAsync(OpenApiDocument openApiDoc, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("开始验证 OpenAPI 文档");
        
        var result = new OpenApiValidationResult { IsValid = true };

        // 验证文档信息
        if (openApiDoc.Info == null)
        {
            result.IsValid = false;
            result.Errors.Add("文档信息不能为空");
        }
        else
        {
            if (string.IsNullOrEmpty(openApiDoc.Info.Title))
            {
                result.Warnings.Add("文档标题为空");
            }
            if (string.IsNullOrEmpty(openApiDoc.Info.Version))
            {
                result.Errors.Add("文档版本不能为空");
                result.IsValid = false;
            }
        }

        // 验证路径
        if (openApiDoc.Paths == null || openApiDoc.Paths.Count == 0)
        {
            result.Warnings.Add("没有定义任何路径");
        }

        // 验证组件
        if (openApiDoc.Components == null)
        {
            result.Warnings.Add("没有定义组件");
        }

        _logger.LogInformation("OpenAPI 文档验证完成，结果: {IsValid}", result.IsValid);
        return await Task.FromResult(result);
    }
}

// OpenAPI 服务实现
public class OpenAPIService : IOpenAPIService
{
    private readonly IOpenApiDocumentGenerator _documentGenerator;
    private readonly IOpenApiValidator _validator;
    private readonly OpenAPIOptions _options;
    private readonly ILogger<OpenAPIService> _logger;

    public OpenAPIService(
        IOpenApiDocumentGenerator documentGenerator,
        IOpenApiValidator validator,
        IOptions<OpenAPIOptions> options,
        ILogger<OpenAPIService> logger)
    {
        _documentGenerator = documentGenerator;
        _validator = validator;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<OpenApiDocument> GenerateOpenApiSpecAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            throw new InvalidOperationException("OpenAPI 服务已禁用");
        }

        _logger.LogInformation("开始生成 OpenAPI 规范");
        var document = await _documentGenerator.GenerateDocumentAsync(_options, cancellationToken);
        _logger.LogInformation("OpenAPI 规范生成完成");
        return document;
    }

    public async Task<OpenApiValidationResult> ValidateOpenApiSpecAsync(OpenApiDocument openApiDoc, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            throw new InvalidOperationException("OpenAPI 服务已禁用");
        }

        _logger.LogInformation("开始验证 OpenAPI 规范");
        var result = await _validator.ValidateAsync(openApiDoc, cancellationToken);
        _logger.LogInformation("OpenAPI 规范验证完成");
        return result;
    }

    public async Task<string> GenerateOpenApiJsonAsync(CancellationToken cancellationToken = default)
    {
        var document = await GenerateOpenApiSpecAsync(cancellationToken);
        return await SerializeToJsonAsync(document, cancellationToken);
    }

    public async Task<string> GenerateOpenApiYamlAsync(CancellationToken cancellationToken = default)
    {
        var document = await GenerateOpenApiSpecAsync(cancellationToken);
        return await SerializeToYamlAsync(document, cancellationToken);
    }

    public async Task<bool> SaveOpenApiSpecAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            var document = await GenerateOpenApiSpecAsync(cancellationToken);
            string content;

            if (filePath.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase) || 
                filePath.EndsWith(".yml", StringComparison.OrdinalIgnoreCase))
            {
                content = await SerializeToYamlAsync(document, cancellationToken);
            }
            else
            {
                content = await SerializeToJsonAsync(document, cancellationToken);
            }

            // 确保目录存在
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(filePath, content, cancellationToken);
            _logger.LogInformation("OpenAPI 规范已保存到: {FilePath}", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存 OpenAPI 规范失败");
            return false;
        }
    }

    public async Task<IEnumerable<OpenApiOperation>> GetApiOperationsAsync(CancellationToken cancellationToken = default)
    {
        var document = await GenerateOpenApiSpecAsync(cancellationToken);
        var operations = new List<OpenApiOperation>();

        if (document.Paths != null)
        {
            foreach (var path in document.Paths)
            {
                foreach (var operation in path.Value.Operations)
                {
                    operations.Add(operation.Value);
                }
            }
        }

        return operations;
    }

    private async Task<string> SerializeToJsonAsync(OpenApiDocument document, CancellationToken cancellationToken = default)
    {
        // 模拟 JSON 序列化
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // 注意：实际项目中，应该使用专门的 OpenAPI 序列化库
        // 这里只是模拟实现
        var json = JsonSerializer.Serialize(new
        {
            openapi = "3.0.0",
            info = new
            {
                title = document.Info.Title,
                version = document.Info.Version,
                description = document.Info.Description
            },
            paths = document.Paths.ToDictionary(
                p => p.Key,
                p => new { get = p.Value.Operations.ContainsKey(OperationType.Get) ? "operation" : null }
            )
        }, jsonOptions);

        return await Task.FromResult(json);
    }

    private async Task<string> SerializeToYamlAsync(OpenApiDocument document, CancellationToken cancellationToken = default)
    {
        // 模拟 YAML 序列化
        var yaml = new StringBuilder();
        yaml.AppendLine($"openapi: 3.0.0");
        yaml.AppendLine($"info:");
        yaml.AppendLine($"  title: {document.Info.Title}");
        yaml.AppendLine($"  version: {document.Info.Version}");
        yaml.AppendLine($"  description: {document.Info.Description}");
        yaml.AppendLine($"paths:");

        foreach (var path in document.Paths)
        {
            yaml.AppendLine($"  {path.Key}:");
            if (path.Value.Operations.ContainsKey(OperationType.Get))
            {
                yaml.AppendLine($"    get:");
                yaml.AppendLine($"      summary: {path.Value.Operations[OperationType.Get].Summary}");
            }
        }

        return await Task.FromResult(yaml.ToString());
    }
}

// 依赖注入扩展
public static class OpenAPIServiceCollectionExtensions
{
    public static IServiceCollection AddOpenAPIServices(this IServiceCollection services, Action<OpenAPIOptions> configureOptions = null)
    {
        // 配置选项
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<OpenAPIOptions>(options => { });
        }

        // 注册服务
        services.AddSingleton<IOpenApiDocumentGenerator, OpenApiDocumentGenerator>();
        services.AddSingleton<IOpenApiValidator, OpenApiValidator>();
        services.AddSingleton<IOpenAPIService, OpenAPIService>();

        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("OpenAPI 服务示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 OpenAPI 服务
        services.AddOpenAPIServices(options =>
        {
            options.Enabled = true;
            options.EnableSwaggerUI = true;
            options.EnableScalarUI = true;
            options.ApiTitle = "示例 API";
            options.ApiVersion = "v1";
            options.ApiDescription = "这是一个示例 API 文档";
            options.ContactName = "开发者";
            options.ContactEmail = "developer@example.com";
            options.LicenseName = "MIT";
            options.LicenseUrl = "https://opensource.org/licenses/MIT";
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var openApiService = serviceProvider.GetRequiredService<IOpenAPIService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 示例 1: 生成 OpenAPI 规范
            Console.WriteLine("示例 1: 生成 OpenAPI 规范");
            var openApiSpec = await openApiService.GenerateOpenApiSpecAsync();
            Console.WriteLine($"OpenAPI 规范生成成功，标题: {openApiSpec.Info.Title}, 版本: {openApiSpec.Info.Version}");
            Console.WriteLine($"定义的路径数量: {openApiSpec.Paths.Count}");

            // 示例 2: 验证 OpenAPI 规范
            Console.WriteLine("\n示例 2: 验证 OpenAPI 规范");
            var validationResult = await openApiService.ValidateOpenApiSpecAsync(openApiSpec);
            Console.WriteLine($"OpenAPI 规范验证: {(validationResult.IsValid ? "有效" : "无效")}");
            if (!validationResult.IsValid)
            {
                Console.WriteLine("错误:");
                foreach (var error in validationResult.Errors)
                {
                    Console.WriteLine($"- {error}");
                }
            }
            if (validationResult.Warnings.Count > 0)
            {
                Console.WriteLine("警告:");
                foreach (var warning in validationResult.Warnings)
                {
                    Console.WriteLine($"- {warning}");
                }
            }

            // 示例 3: 生成 OpenAPI JSON
            Console.WriteLine("\n示例 3: 生成 OpenAPI JSON");
            var jsonSpec = await openApiService.GenerateOpenApiJsonAsync();
            Console.WriteLine("OpenAPI JSON 生成成功，前 500 个字符:");
            Console.WriteLine(jsonSpec.Substring(0, Math.Min(500, jsonSpec.Length)));
            Console.WriteLine("...");

            // 示例 4: 保存 OpenAPI 规范
            Console.WriteLine("\n示例 4: 保存 OpenAPI 规范");
            var savePath = Path.Combine(Directory.GetCurrentDirectory(), "openapi-spec.json");
            var saveResult = await openApiService.SaveOpenApiSpecAsync(savePath);
            Console.WriteLine($"保存 OpenAPI 规范到 {savePath}: {(saveResult ? "成功" : "失败")}");

            // 示例 5: 获取 API 操作
            Console.WriteLine("\n示例 5: 获取 API 操作");
            var operations = await openApiService.GetApiOperationsAsync();
            Console.WriteLine($"获取到 {operations.Count()} 个 API 操作:");
            foreach (var operation in operations)
            {
                Console.WriteLine($"- {operation.Summary}");
            }

            Console.WriteLine("\n所有示例执行完成！");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "执行示例时发生错误");
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
