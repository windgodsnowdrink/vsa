# OpenAPI 技能使用示例

## 概述

本文档提供了 OpenAPI 技能的各种使用示例，包括基本用法、高级配置、性能优化、错误处理等。这些示例旨在帮助您快速上手 OpenAPI 功能，并了解其在不同场景下的应用。

## 示例 1: 基本使用

### 功能说明

演示 OpenAPI 技能的基本使用方法，包括服务注册、OpenAPI 规范生成和验证。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Swashbuckle.AspNetCore@6.5.0
#:package Microsoft.OpenApi@1.6.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

// 配置选项
public class OpenAPIOptions
{
    public bool Enabled { get; set; } = true;
    public string ApiTitle { get; set; } = "API";
    public string ApiVersion { get; set; } = "v1";
}

// OpenAPI 服务接口
public interface IOpenAPIService
{
    Task<string> GenerateOpenApiJsonAsync();
    Task<bool> ValidateOpenApiSpecAsync(string openApiJson);
}

// OpenAPI 服务实现
public class OpenAPIService : IOpenAPIService
{
    private readonly OpenAPIOptions _options;
    private readonly ILogger<OpenAPIService> _logger;

    public OpenAPIService(Microsoft.Extensions.Options.IOptions<OpenAPIOptions> options, ILogger<OpenAPIService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GenerateOpenApiJsonAsync()
    {
        _logger.LogInformation("生成 OpenAPI 规范");
        await Task.Delay(500); // 模拟生成过程
        return $"{{\"openapi\":\"3.0.0\",\"info\":{{\"title\":\"{_options.ApiTitle}\",\"version\":\"{_options.ApiVersion}\"}},\"paths\":{{}}}}";
    }

    public async Task<bool> ValidateOpenApiSpecAsync(string openApiJson)
    {
        _logger.LogInformation("验证 OpenAPI 规范");
        await Task.Delay(300); // 模拟验证过程
        return !string.IsNullOrEmpty(openApiJson);
    }
}

// 依赖注入扩展
public static class OpenAPIServiceCollectionExtensions
{
    public static IServiceCollection AddOpenAPIServices(this IServiceCollection services, Action<OpenAPIOptions> configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<OpenAPIOptions>(options => { });
        }

        services.AddSingleton<IOpenAPIService, OpenAPIService>();
        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("OpenAPI 基本使用示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 OpenAPI 服务
        services.AddOpenAPIServices(options =>
        {
            options.Enabled = true;
            options.ApiTitle = "示例 API";
            options.ApiVersion = "v1";
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var openApiService = serviceProvider.GetRequiredService<IOpenAPIService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 生成 OpenAPI 规范
            Console.WriteLine("生成 OpenAPI 规范...");
            var openApiJson = await openApiService.GenerateOpenApiJsonAsync();
            Console.WriteLine("OpenAPI 规范生成成功");
            Console.WriteLine($"规范内容: {openApiJson}");

            // 验证 OpenAPI 规范
            Console.WriteLine("\n验证 OpenAPI 规范...");
            var isValid = await openApiService.ValidateOpenApiSpecAsync(openApiJson);
            Console.WriteLine($"OpenAPI 规范验证: {(isValid ? "有效" : "无效")}");

            Console.WriteLine("\n示例执行完成！");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "执行示例时发生错误");
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 示例 2: 高级配置

### 功能说明

演示 OpenAPI 技能的高级配置选项，包括 Swagger UI 配置、Scalar UI 配置和并行处理配置等。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Swashbuckle.AspNetCore@6.5.0
#:package Microsoft.OpenApi@1.6.0
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

// 高级配置选项
public class OpenAPIOptions
{
    public bool Enabled { get; set; } = true;
    public string ApiTitle { get; set; } = "API";
    public string ApiVersion { get; set; } = "v1";
    public bool EnableSwaggerUI { get; set; } = true;
    public bool EnableScalarUI { get; set; } = true;
    public string SwaggerRoutePrefix { get; set; } = "swagger";
    public string ScalarRoutePrefix { get; set; } = "api-reference";
    public bool EnableParallelProcessing { get; set; } = true;
    public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
}

// OpenAPI 服务接口
public interface IOpenAPIService
{
    Task<string> GenerateOpenApiJsonAsync();
    Task<string> GenerateOpenApiYamlAsync();
    Task<bool> SaveOpenApiSpecAsync(string filePath);
}

// OpenAPI 服务实现
public class OpenAPIService : IOpenAPIService
{
    private readonly OpenAPIOptions _options;
    private readonly ILogger<OpenAPIService> _logger;

    public OpenAPIService(Microsoft.Extensions.Options.IOptions<OpenAPIOptions> options, ILogger<OpenAPIService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GenerateOpenApiJsonAsync()
    {
        _logger.LogInformation("生成 OpenAPI JSON 规范");
        await Task.Delay(500);
        return $"{{\"openapi\":\"3.0.0\",\"info\":{{\"title\":\"{_options.ApiTitle}\",\"version\":\"{_options.ApiVersion}\"}},\"paths\":{{}}}}";
    }

    public async Task<string> GenerateOpenApiYamlAsync()
    {
        _logger.LogInformation("生成 OpenAPI YAML 规范");
        await Task.Delay(500);
        return $"openapi: 3.0.0\ninfo:\n  title: {_options.ApiTitle}\n  version: {_options.ApiVersion}\npaths: {{}}";
    }

    public async Task<bool> SaveOpenApiSpecAsync(string filePath)
    {
        try
        {
            _logger.LogInformation("保存 OpenAPI 规范到文件: {FilePath}", filePath);
            
            string content;
            if (filePath.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase) || 
                filePath.EndsWith(".yml", StringComparison.OrdinalIgnoreCase))
            {
                content = await GenerateOpenApiYamlAsync();
            }
            else
            {
                content = await GenerateOpenApiJsonAsync();
            }

            // 确保目录存在
            var directory = System.IO.Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            await System.IO.File.WriteAllTextAsync(filePath, content);
            _logger.LogInformation("OpenAPI 规范已保存到: {FilePath}", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存 OpenAPI 规范失败");
            return false;
        }
    }
}

// 依赖注入扩展
public static class OpenAPIServiceCollectionExtensions
{
    public static IServiceCollection AddOpenAPIServices(this IServiceCollection services, Action<OpenAPIOptions> configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<OpenAPIOptions>(options => { });
        }

        services.AddSingleton<IOpenAPIService, OpenAPIService>();
        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("OpenAPI 高级配置示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 OpenAPI 服务并配置高级选项
        services.AddOpenAPIServices(options =>
        {
            options.Enabled = true;
            options.ApiTitle = "高级 API";
            options.ApiVersion = "v1";
            options.EnableSwaggerUI = true;
            options.EnableScalarUI = true;
            options.SwaggerRoutePrefix = "swagger";
            options.ScalarRoutePrefix = "api-reference";
            options.EnableParallelProcessing = true;
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var openApiService = serviceProvider.GetRequiredService<IOpenAPIService>();

        try
        {
            // 生成 JSON 格式规范
            Console.WriteLine("生成 JSON 格式规范...");
            var jsonSpec = await openApiService.GenerateOpenApiJsonAsync();
            Console.WriteLine("JSON 规范生成成功");

            // 生成 YAML 格式规范
            Console.WriteLine("\n生成 YAML 格式规范...");
            var yamlSpec = await openApiService.GenerateOpenApiYamlAsync();
            Console.WriteLine("YAML 规范生成成功");

            // 保存规范到文件
            Console.WriteLine("\n保存规范到文件...");
            var jsonSaveResult = await openApiService.SaveOpenApiSpecAsync("openapi.json");
            var yamlSaveResult = await openApiService.SaveOpenApiSpecAsync("openapi.yaml");
            Console.WriteLine($"JSON 文件保存: {(jsonSaveResult ? "成功" : "失败")}");
            Console.WriteLine($"YAML 文件保存: {(yamlSaveResult ? "成功" : "失败")}");

            Console.WriteLine("\n示例执行完成！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 示例 3: 性能优化

### 功能说明

演示 OpenAPI 技能的性能优化技术，包括并行处理、缓存和内存优化等。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Swashbuckle.AspNetCore@6.5.0
#:package Microsoft.OpenApi@1.6.0
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

// 性能优化配置选项
public class OpenAPIOptions
{
    public bool Enabled { get; set; } = true;
    public string ApiTitle { get; set; } = "API";
    public string ApiVersion { get; set; } = "v1";
    public bool EnableParallelProcessing { get; set; } = true;
    public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
    public bool EnableCaching { get; set; } = true;
    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(5);
}

// OpenAPI 服务接口
public interface IOpenAPIService
{
    Task<IEnumerable<string>> GenerateMultipleSpecsAsync(IEnumerable<string> apiVersions);
    Task<long> MeasurePerformanceAsync(int iterations);
}

// OpenAPI 服务实现
public class OpenAPIService : IOpenAPIService
{
    private readonly OpenAPIOptions _options;
    private readonly ILogger<OpenAPIService> _logger;
    private string _cachedSpec;
    private DateTime _cacheTimestamp;

    public OpenAPIService(Microsoft.Extensions.Options.IOptions<OpenAPIOptions> options, ILogger<OpenAPIService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IEnumerable<string>> GenerateMultipleSpecsAsync(IEnumerable<string> apiVersions)
    {
        _logger.LogInformation("开始生成多个 OpenAPI 规范");
        var stopwatch = Stopwatch.StartNew();

        IEnumerable<string> results;

        if (_options.EnableParallelProcessing)
        {
            // 使用数据流进行并行处理
            var transformBlock = new TransformBlock<string, string>(
                async version => await GenerateSpecForVersionAsync(version),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = _options.MaxDegreeOfParallelism
                }
            );

            var actionBlock = new ActionBlock<string>(spec => { });
            var bufferBlock = new BufferBlock<string>();

            transformBlock.LinkTo(bufferBlock, new DataflowLinkOptions { PropagateCompletion = true });

            // 发布所有 API 版本
            foreach (var version in apiVersions)
            {
                await transformBlock.SendAsync(version);
            }

            transformBlock.Complete();
            await bufferBlock.Completion;

            // 收集结果
            var specs = new List<string>();
            while (bufferBlock.TryReceive(out var spec))
            {
                specs.Add(spec);
            }
            results = specs;
        }
        else
        {
            // 串行处理
            results = await Task.WhenAll(apiVersions.Select(version => GenerateSpecForVersionAsync(version)));
        }

        stopwatch.Stop();
        _logger.LogInformation("生成多个 OpenAPI 规范完成，耗时: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
        return results;
    }

    public async Task<long> MeasurePerformanceAsync(int iterations)
    {
        _logger.LogInformation("开始性能测试，迭代次数: {Iterations}", iterations);
        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            await GenerateSpecForVersionAsync("v1");
        }

        stopwatch.Stop();
        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        _logger.LogInformation("性能测试完成，总耗时: {ElapsedMilliseconds}ms, 平均每次: {AverageMilliseconds}ms", 
            elapsedMilliseconds, elapsedMilliseconds / (double)iterations);
        return elapsedMilliseconds;
    }

    private async Task<string> GenerateSpecForVersionAsync(string version)
    {
        // 检查缓存
        if (_options.EnableCaching && !string.IsNullOrEmpty(_cachedSpec) && 
            (DateTime.Now - _cacheTimestamp) < _options.CacheDuration)
        {
            _logger.LogInformation("使用缓存的 OpenAPI 规范");
            return _cachedSpec;
        }

        _logger.LogInformation("生成 OpenAPI 规范，版本: {Version}", version);
        await Task.Delay(100); // 模拟生成过程

        var spec = $"{{\"openapi\":\"3.0.0\",\"info\":{{\"title\":\"{_options.ApiTitle}\",\"version\":\"{version}\"}},\"paths\":{{}}}}";

        // 更新缓存
        if (_options.EnableCaching)
        {
            _cachedSpec = spec;
            _cacheTimestamp = DateTime.Now;
        }

        return spec;
    }
}

// 依赖注入扩展
public static class OpenAPIServiceCollectionExtensions
{
    public static IServiceCollection AddOpenAPIServices(this IServiceCollection services, Action<OpenAPIOptions> configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<OpenAPIOptions>(options => { });
        }

        services.AddSingleton<IOpenAPIService, OpenAPIService>();
        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("OpenAPI 性能优化示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 OpenAPI 服务并配置性能优化选项
        services.AddOpenAPIServices(options =>
        {
            options.Enabled = true;
            options.ApiTitle = "性能优化 API";
            options.ApiVersion = "v1";
            options.EnableParallelProcessing = true;
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;
            options.EnableCaching = true;
            options.CacheDuration = TimeSpan.FromMinutes(5);
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var openApiService = serviceProvider.GetRequiredService<IOpenAPIService>();

        try
        {
            // 测试 1: 并行生成多个规范
            Console.WriteLine("测试 1: 并行生成多个规范");
            var apiVersions = new[] { "v1", "v2", "v3", "v4", "v5" };
            var specs = await openApiService.GenerateMultipleSpecsAsync(apiVersions);
            Console.WriteLine($"生成了 {specs.Count()} 个 OpenAPI 规范");

            // 测试 2: 性能测试
            Console.WriteLine("\n测试 2: 性能测试");
            var iterations = 1000;
            var elapsedMilliseconds = await openApiService.MeasurePerformanceAsync(iterations);
            Console.WriteLine($"处理 {iterations} 次耗时: {elapsedMilliseconds}ms");
            Console.WriteLine($"平均每次耗时: {elapsedMilliseconds / (double)iterations:F2}ms");

            Console.WriteLine("\n示例执行完成！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 示例 4: 错误处理

### 功能说明

演示 OpenAPI 技能的错误处理机制，包括异常捕获、错误日志记录和优雅错误处理。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Swashbuckle.AspNetCore@6.5.0
#:package Microsoft.OpenApi@1.6.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

// 配置选项
public class OpenAPIOptions
{
    public bool Enabled { get; set; } = true;
    public string ApiTitle { get; set; } = "API";
    public string ApiVersion { get; set; } = "v1";
}

// OpenAPI 服务接口
public interface IOpenAPIService
{
    Task<string> GenerateOpenApiJsonAsync();
    Task<bool> SaveOpenApiSpecAsync(string filePath);
}

// OpenAPI 服务实现
public class OpenAPIService : IOpenAPIService
{
    private readonly OpenAPIOptions _options;
    private readonly ILogger<OpenAPIService> _logger;

    public OpenAPIService(Microsoft.Extensions.Options.IOptions<OpenAPIOptions> options, ILogger<OpenAPIService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GenerateOpenApiJsonAsync()
    {
        try
        {
            if (!_options.Enabled)
            {
                throw new InvalidOperationException("OpenAPI 服务已禁用");
            }

            _logger.LogInformation("生成 OpenAPI JSON 规范");
            await Task.Delay(500);

            // 模拟偶尔的错误
            if (DateTime.Now.Millisecond % 10 == 0)
            {
                throw new Exception("模拟生成过程中的错误");
            }

            return $"{{\"openapi\":\"3.0.0\",\"info\":{{\"title\":\"{_options.ApiTitle}\",\"version\":\"{_options.ApiVersion}\"}},\"paths\":{{}}}}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成 OpenAPI JSON 规范失败");
            throw;
        }
    }

    public async Task<bool> SaveOpenApiSpecAsync(string filePath)
    {
        try
        {
            _logger.LogInformation("保存 OpenAPI 规范到文件: {FilePath}", filePath);

            // 验证文件路径
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "文件路径不能为空");
            }

            // 模拟路径错误
            if (filePath.Contains("invalid"))
            {
                throw new IOException("无效的文件路径");
            }

            var spec = await GenerateOpenApiJsonAsync();

            // 确保目录存在
            var directory = System.IO.Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !System.IO.Directory.Exists(directory))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(directory);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "创建目录失败: {Directory}", directory);
                    throw new IOException($"创建目录失败: {directory}", ex);
                }
            }

            await System.IO.File.WriteAllTextAsync(filePath, spec);
            _logger.LogInformation("OpenAPI 规范已保存到: {FilePath}", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存 OpenAPI 规范失败");
            return false;
        }
    }
}

// 依赖注入扩展
public static class OpenAPIServiceCollectionExtensions
{
    public static IServiceCollection AddOpenAPIServices(this IServiceCollection services, Action<OpenAPIOptions> configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<OpenAPIOptions>(options => { });
        }

        services.AddSingleton<IOpenAPIService, OpenAPIService>();
        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("OpenAPI 错误处理示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 OpenAPI 服务
        services.AddOpenAPIServices(options =>
        {
            options.Enabled = true;
            options.ApiTitle = "错误处理 API";
            options.ApiVersion = "v1";
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var openApiService = serviceProvider.GetRequiredService<IOpenAPIService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 测试 1: 正常生成规范
            Console.WriteLine("测试 1: 正常生成规范");
            try
            {
                var jsonSpec = await openApiService.GenerateOpenApiJsonAsync();
                Console.WriteLine("生成规范成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"生成规范失败: {ex.Message}");
            }

            // 测试 2: 保存到有效路径
            Console.WriteLine("\n测试 2: 保存到有效路径");
            var validPath = "openapi.json";
            var validSaveResult = await openApiService.SaveOpenApiSpecAsync(validPath);
            Console.WriteLine($"保存到有效路径: {(validSaveResult ? "成功" : "失败")}");

            // 测试 3: 保存到无效路径
            Console.WriteLine("\n测试 3: 保存到无效路径");
            var invalidPath = "invalid/path/openapi.json";
            var invalidSaveResult = await openApiService.SaveOpenApiSpecAsync(invalidPath);
            Console.WriteLine($"保存到无效路径: {(invalidSaveResult ? "成功" : "失败")}");

            // 测试 4: 保存到空路径
            Console.WriteLine("\n测试 4: 保存到空路径");
            var emptyPath = "";
            var emptySaveResult = await openApiService.SaveOpenApiSpecAsync(emptyPath);
            Console.WriteLine($"保存到空路径: {(emptySaveResult ? "成功" : "失败")}");

            Console.WriteLine("\n示例执行完成！");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "执行示例时发生错误");
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 示例 5: 自定义 OpenAPI 服务

### 功能说明

演示如何创建自定义的 OpenAPI 服务，扩展基本功能以满足特定需求。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Swashbuckle.AspNetCore@6.5.0
#:package Microsoft.OpenApi@1.6.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// 自定义配置选项
public class CustomOpenAPIOptions
{
    public bool Enabled { get; set; } = true;
    public string ApiTitle { get; set; } = "API";
    public string ApiVersion { get; set; } = "v1";
    public bool IncludeSecurityDefinitions { get; set; } = true;
    public bool IncludeExamples { get; set; } = true;
    public List<string> SupportedFormats { get; set; } = new List<string> { "json", "yaml" };
}

// 自定义 OpenAPI 服务接口
public interface ICustomOpenAPIService
{
    Task<string> GenerateOpenApiSpecAsync(string format);
    Task<string> GenerateOpenApiSpecWithSecurityAsync(string format);
    Task<string> GenerateOpenApiSpecWithExamplesAsync(string format);
}

// 自定义 OpenAPI 服务实现
public class CustomOpenAPIService : ICustomOpenAPIService
{
    private readonly CustomOpenAPIOptions _options;
    private readonly ILogger<CustomOpenAPIService> _logger;

    public CustomOpenAPIService(Microsoft.Extensions.Options.IOptions<CustomOpenAPIOptions> options, ILogger<CustomOpenAPIService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GenerateOpenApiSpecAsync(string format)
    {
        _logger.LogInformation("生成自定义 OpenAPI 规范，格式: {Format}", format);
        await Task.Delay(500);

        if (!_options.SupportedFormats.Contains(format.ToLower()))
        {
            throw new NotSupportedException($"不支持的格式: {format}");
        }

        if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
        {
            return GenerateJsonSpec();
        }
        else
        {
            return GenerateYamlSpec();
        }
    }

    public async Task<string> GenerateOpenApiSpecWithSecurityAsync(string format)
    {
        _logger.LogInformation("生成带安全定义的 OpenAPI 规范，格式: {Format}", format);
        await Task.Delay(500);

        if (!_options.IncludeSecurityDefinitions)
        {
            return await GenerateOpenApiSpecAsync(format);
        }

        if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
        {
            return GenerateJsonSpecWithSecurity();
        }
        else
        {
            return GenerateYamlSpecWithSecurity();
        }
    }

    public async Task<string> GenerateOpenApiSpecWithExamplesAsync(string format)
    {
        _logger.LogInformation("生成带示例的 OpenAPI 规范，格式: {Format}", format);
        await Task.Delay(500);

        if (!_options.IncludeExamples)
        {
            return await GenerateOpenApiSpecAsync(format);
        }

        if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
        {
            return GenerateJsonSpecWithExamples();
        }
        else
        {
            return GenerateYamlSpecWithExamples();
        }
    }

    private string GenerateJsonSpec()
    {
        return $"{{\"openapi\":\"3.0.0\",\"info\":{{\"title\":\"{_options.ApiTitle}\",\"version\":\"{_options.ApiVersion}\"}},\"paths\":{{}}}}";
    }

    private string GenerateYamlSpec()
    {
        return $"openapi: 3.0.0\ninfo:\n  title: {_options.ApiTitle}\n  version: {_options.ApiVersion}\npaths: {{}}";
    }

    private string GenerateJsonSpecWithSecurity()
    {
        return $"{{\"openapi\":\"3.0.0\",\"info\":{{\"title\":\"{_options.ApiTitle}\",\"version\":\"{_options.ApiVersion}\"}},\"paths\":{{}},\"components\":{{\"securitySchemes\":{{\"Bearer\":{{\"type\":\"http\",\"scheme\":\"bearer\",\"bearerFormat\":\"JWT\"}}}}},\"security\":[{{\"Bearer\":[]}}]}}";
    }

    private string GenerateYamlSpecWithSecurity()
    {
        return $"openapi: 3.0.0\ninfo:\n  title: {_options.ApiTitle}\n  version: {_options.ApiVersion}\npaths: {{}}\ncomponents:\n  securitySchemes:\n    Bearer:\n      type: http\n      scheme: bearer\n      bearerFormat: JWT\nsecurity:\n  - Bearer: []";
    }

    private string GenerateJsonSpecWithExamples()
    {
        return $"{{\"openapi\":\"3.0.0\",\"info\":{{\"title\":\"{_options.ApiTitle}\",\"version\":\"{_options.ApiVersion}\"}},\"paths\":{{\"/api/users\":{{\"get\":{{\"summary\":\"获取用户列表\",\"responses\":{{\"200\":{{\"description\":\"成功\",\"content\":{{\"application/json\":{{\"examples\":{{\"example1\":{{\"value\":[{{\"id\":1,\"name\":\"用户1\"}},{{\"id\":2,\"name\":\"用户2\"}}]}}}}}}}}}}}}}}}}}}";
    }

    private string GenerateYamlSpecWithExamples()
    {
        return $"openapi: 3.0.0\ninfo:\n  title: {_options.ApiTitle}\n  version: {_options.ApiVersion}\npaths:\n  /api/users:\n    get:\n      summary: 获取用户列表\n      responses:\n        '200':\n          description: 成功\n          content:\n            application/json:\n              examples:\n                example1:\n                  value:\n                    - id: 1\n                      name: 用户1\n                    - id: 2\n                      name: 用户2";
    }
}

// 依赖注入扩展
public static class CustomOpenAPIServiceCollectionExtensions
{
    public static IServiceCollection AddCustomOpenAPIServices(this IServiceCollection services, Action<CustomOpenAPIOptions> configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<CustomOpenAPIOptions>(options => { });
        }

        services.AddSingleton<ICustomOpenAPIService, CustomOpenAPIService>();
        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("OpenAPI 自定义服务示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册自定义 OpenAPI 服务
        services.AddCustomOpenAPIServices(options =>
        {
            options.Enabled = true;
            options.ApiTitle = "自定义 API";
            options.ApiVersion = "v1";
            options.IncludeSecurityDefinitions = true;
            options.IncludeExamples = true;
            options.SupportedFormats = new List<string> { "json", "yaml" };
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var customOpenApiService = serviceProvider.GetRequiredService<ICustomOpenAPIService>();

        try
        {
            // 测试 1: 生成 JSON 格式规范
            Console.WriteLine("测试 1: 生成 JSON 格式规范");
            var jsonSpec = await customOpenApiService.GenerateOpenApiSpecAsync("json");
            Console.WriteLine("JSON 规范生成成功");

            // 测试 2: 生成 YAML 格式规范
            Console.WriteLine("\n测试 2: 生成 YAML 格式规范");
            var yamlSpec = await customOpenApiService.GenerateOpenApiSpecAsync("yaml");
            Console.WriteLine("YAML 规范生成成功");

            // 测试 3: 生成带安全定义的规范
            Console.WriteLine("\n测试 3: 生成带安全定义的规范");
            var securitySpec = await customOpenApiService.GenerateOpenApiSpecWithSecurityAsync("json");
            Console.WriteLine("带安全定义的规范生成成功");

            // 测试 4: 生成带示例的规范
            Console.WriteLine("\n测试 4: 生成带示例的规范");
            var examplesSpec = await customOpenApiService.GenerateOpenApiSpecWithExamplesAsync("yaml");
            Console.WriteLine("带示例的规范生成成功");

            Console.WriteLine("\n示例执行完成！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 示例 6: 实际应用场景

### 功能说明

演示 OpenAPI 技能在实际应用场景中的使用，如 API 文档生成、微服务架构和云部署等。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Swashbuckle.AspNetCore@6.5.0
#:package Microsoft.OpenApi@1.6.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// 应用场景配置选项
public class OpenAPIApplicationOptions
{
    public bool Enabled { get; set; } = true;
    public string ApiTitle { get; set; } = "API";
    public string ApiVersion { get; set; } = "v1";
    public bool EnableSwaggerUI { get; set; } = true;
    public bool EnableScalarUI { get; set; } = true;
    public string Environment { get; set; } = "Development";
    public bool EnableCloudDeployment { get; set; } = false;
    public string CloudProvider { get; set; } = "Azure";
}

// API 文档服务接口
public interface IApiDocumentationService
{
    Task<bool> GenerateApiDocumentationAsync(string outputPath);
    Task<bool> DeployApiDocumentationAsync(string environment);
    Task<bool> GenerateClientSdkAsync(string language, string outputPath);
}

// API 文档服务实现
public class ApiDocumentationService : IApiDocumentationService
{
    private readonly OpenAPIApplicationOptions _options;
    private readonly ILogger<ApiDocumentationService> _logger;

    public ApiDocumentationService(Microsoft.Extensions.Options.IOptions<OpenAPIApplicationOptions> options, ILogger<ApiDocumentationService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> GenerateApiDocumentationAsync(string outputPath)
    {
        try
        {
            _logger.LogInformation("生成 API 文档到: {OutputPath}", outputPath);
            await Task.Delay(1000);

            // 确保目录存在
            var directory = System.IO.Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            // 生成 OpenAPI 规范
            var openApiSpec = GenerateOpenApiSpec();
            await System.IO.File.WriteAllTextAsync(outputPath, openApiSpec);

            _logger.LogInformation("API 文档生成完成");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成 API 文档失败");
            return false;
        }
    }

    public async Task<bool> DeployApiDocumentationAsync(string environment)
    {
        try
        {
            _logger.LogInformation("部署 API 文档到环境: {Environment}", environment);
            await Task.Delay(1500);

            if (_options.EnableCloudDeployment)
            {
                _logger.LogInformation("部署到云平台: {CloudProvider}", _options.CloudProvider);
                // 模拟云部署过程
                await Task.Delay(1000);
            }

            _logger.LogInformation("API 文档部署完成");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "部署 API 文档失败");
            return false;
        }
    }

    public async Task<bool> GenerateClientSdkAsync(string language, string outputPath)
    {
        try
        {
            _logger.LogInformation("生成客户端 SDK，语言: {Language}, 输出路径: {OutputPath}", language, outputPath);
            await Task.Delay(2000);

            // 确保目录存在
            var directory = System.IO.Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            // 模拟生成客户端 SDK
            var sdkContent = GenerateClientSdkContent(language);
            await System.IO.File.WriteAllTextAsync(outputPath, sdkContent);

            _logger.LogInformation("客户端 SDK 生成完成");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成客户端 SDK 失败");
            return false;
        }
    }

    private string GenerateOpenApiSpec()
    {
        return $"{{\"openapi\":\"3.0.0\",\"info\":{{\"title\":\"{_options.ApiTitle}\",\"version\":\"{_options.ApiVersion}\"}},\"paths\":{{\"/api/users\":{{\"get\":{{\"summary\":\"获取用户列表\",\"responses\":{{\"200\":{{\"description\":\"成功\"}}}}}},\"post\":{{\"summary\":\"创建用户\",\"responses\":{{\"201\":{{\"description\":\"创建成功\"}}}}}}},\"/api/users/{{id}}\":{{\"get\":{{\"summary\":\"获取用户详情\",\"responses\":{{\"200\":{{\"description\":\"成功\"}}}}}},\"put\":{{\"summary\":\"更新用户\",\"responses\":{{\"200\":{{\"description\":\"更新成功\"}}}}}},\"delete\":{{\"summary\":\"删除用户\",\"responses\":{{\"204\":{{\"description\":\"删除成功\"}}}}}}}}}}}}";
    }

    private string GenerateClientSdkContent(string language)
    {
        switch (language.ToLower())
        {
            case "csharp":
                return GenerateCSharpSdk();
            case "typescript":
                return GenerateTypeScriptSdk();
            case "python":
                return GeneratePythonSdk();
            default:
                return $"// 客户端 SDK for {language}\n// 生成时间: {DateTime.Now}";
        }
    }

    private string GenerateCSharpSdk()
    {
        return $"// C# 客户端 SDK\n// 生成时间: {DateTime.Now}\n\nnamespace ApiClient\n{{\n    public class UserClient\n    {{\n        private readonly HttpClient _httpClient;\n\n        public UserClient(HttpClient httpClient)\n        {{\n            _httpClient = httpClient;\n        }}\n\n        public async Task<List<User>> GetUsersAsync()\n        {{\n            var response = await _httpClient.GetAsync(\"/api/users\");\n            response.EnsureSuccessStatusCode();\n            return await response.Content.ReadFromJsonAsync<List<User>>();\n        }}\n\n        public async Task<User> GetUserAsync(int id)\n        {{\n            var response = await _httpClient.GetAsync($\"/api/users/{{id}}\");\n            response.EnsureSuccessStatusCode();\n            return await response.Content.ReadFromJsonAsync<User>();\n        }}\n    }}\n\n    public class User\n    {{\n        public int Id {{ get; set; }}\n        public string Name {{ get; set; }}\n        public string Email {{ get; set; }}\n    }}\n}}";
    }

    private string GenerateTypeScriptSdk()
    {
        return $"// TypeScript 客户端 SDK\n// 生成时间: {DateTime.Now}\n\nexport class UserClient {{
  private baseUrl: string;
\n  constructor(baseUrl: string = '/api') {{
    this.baseUrl = baseUrl;
  }}\n\n  async getUsers(): Promise<User[]> {{
    const response = await fetch(`${{this.baseUrl}}/users`);
    if (!response.ok) {{
      throw new Error(`HTTP error! status: ${response.status}`);
    }}
    return await response.json();
  }}\n\n  async getUser(id: number): Promise<User> {{
    const response = await fetch(`${{this.baseUrl}}/users/${{id}}`);
    if (!response.ok) {{
      throw new Error(`HTTP error! status: ${response.status}`);
    }}
    return await response.json();
  }}\n}}\n\nexport interface User {{
  id: number;
  name: string;
  email: string;
}}";
    }

    private string GeneratePythonSdk()
    {
        return $"# Python 客户端 SDK
# 生成时间: {DateTime.Now}

import requests

class UserClient:
    def __init__(self, base_url='/api'):
        self.base_url = base_url
    
    def get_users(self):
        response = requests.get(f'{self.base_url}/users')
        response.raise_for_status()
        return response.json()
    
    def get_user(self, user_id):
        response = requests.get(f'{self.base_url}/users/{user_id}')
        response.raise_for_status()
        return response.json()

class User:
    def __init__(self, id, name, email):
        self.id = id
        self.name = name
        self.email = email";
    }
}

// 依赖注入扩展
public static class ApiDocumentationServiceCollectionExtensions
{
    public static IServiceCollection AddApiDocumentationServices(this IServiceCollection services, Action<OpenAPIApplicationOptions> configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<OpenAPIApplicationOptions>(options => { });
        }

        services.AddSingleton<IApiDocumentationService, ApiDocumentationService>();
        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("OpenAPI 实际应用场景示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 API 文档服务
        services.AddApiDocumentationServices(options =>
        {
            options.Enabled = true;
            options.ApiTitle = "实际应用 API";
            options.ApiVersion = "v1";
            options.EnableSwaggerUI = true;
            options.EnableScalarUI = true;
            options.Environment = "Production";
            options.EnableCloudDeployment = true;
            options.CloudProvider = "Azure";
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var apiDocumentationService = serviceProvider.GetRequiredService<IApiDocumentationService>();

        try
        {
            // 测试 1: 生成 API 文档
            Console.WriteLine("测试 1: 生成 API 文档");
            var docPath = "docs/openapi.json";
            var docResult = await apiDocumentationService.GenerateApiDocumentationAsync(docPath);
            Console.WriteLine($"生成 API 文档: {(docResult ? "成功" : "失败")}");

            // 测试 2: 部署 API 文档
            Console.WriteLine("\n测试 2: 部署 API 文档");
            var deployResult = await apiDocumentationService.DeployApiDocumentationAsync("Production");
            Console.WriteLine($"部署 API 文档: {(deployResult ? "成功" : "失败")}");

            // 测试 3: 生成 C# 客户端 SDK
            Console.WriteLine("\n测试 3: 生成 C# 客户端 SDK");
            var csharpSdkPath = "sdk/csharp/ApiClient.cs";
            var csharpSdkResult = await apiDocumentationService.GenerateClientSdkAsync("csharp", csharpSdkPath);
            Console.WriteLine($"生成 C# 客户端 SDK: {(csharpSdkResult ? "成功" : "失败")}");

            // 测试 4: 生成 TypeScript 客户端 SDK
            Console.WriteLine("\n测试 4: 生成 TypeScript 客户端 SDK");
            var tsSdkPath = "sdk/typescript/api-client.ts";
            var tsSdkResult = await apiDocumentationService.GenerateClientSdkAsync("typescript", tsSdkPath);
            Console.WriteLine($"生成 TypeScript 客户端 SDK: {(tsSdkResult ? "成功" : "失败")}");

            Console.WriteLine("\n示例执行完成！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 实际应用场景

### 场景 1: API 文档自动化

**功能说明**：在 CI/CD 流程中自动生成和部署 API 文档。

**应用示例**：
- 在代码提交时自动生成 OpenAPI 规范
- 将生成的文档部署到内部文档服务器
- 通知开发团队文档已更新

### 场景 2: 微服务架构

**功能说明**：在微服务架构中统一管理 API 文档。

**应用示例**：
- 为每个微服务生成单独的 OpenAPI 规范
- 使用 API 网关聚合所有微服务的文档
- 提供统一的 API 文档门户

### 场景 3: 客户端 SDK 生成

**功能说明**：基于 OpenAPI 规范自动生成多语言客户端 SDK。

**应用示例**：
- 为前端团队生成 TypeScript SDK
- 为移动应用团队生成 Swift 和 Kotlin SDK
- 为后端团队生成 C# 和 Java SDK

### 场景 4: API 版本管理

**功能说明**：管理 API 的多个版本，确保向后兼容性。

**应用示例**：
- 为每个 API 版本生成单独的 OpenAPI 规范
- 提供版本切换功能
- 记录 API 变更历史

### 场景 5: 云部署集成

**功能说明**：将 API 文档部署到云平台，提供全球访问。

**应用示例**：
- 部署到 Azure API Management
- 部署到 AWS API Gateway
- 部署到 Google Cloud Endpoints

### 场景 6: API 测试自动化

**功能说明**：基于 OpenAPI 规范自动生成测试用例。

**应用示例**：
- 生成 API 功能测试用例
- 生成 API 性能测试用例
- 集成到 CI/CD 流程中

## 总结

OpenAPI 技能提供了强大的 API 文档生成和管理功能，可以帮助您在各种场景下实现 API 的标准化和自动化。通过本文档的示例，您应该已经了解了 OpenAPI 技能的基本使用方法、高级配置、性能优化、错误处理、自定义扩展和实际应用场景等方面的知识。

在实际应用中，您可以根据具体需求选择合适的使用方式，并结合 AOT 编译等技术优化性能。OpenAPI 技能的灵活性和扩展性使其成为构建 API 文档系统的理想选择。
