# FluentValidation - 使用示例

## 快速开始

### 1. 基本使用示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FluentValidation.AOT;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var host = CreateHostBuilder().Build();
        var validationService = host.Services.GetRequiredService<IFluentValidationService>();
        
        Console.WriteLine("FluentValidation 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 使用 FluentValidation 功能
        Console.WriteLine("1. 验证有效的测试模型");
        var validModel = new TestModel
        {
            Name = "张三",
            Age = 25,
            Email = "zhangsan@example.com",
            Phone = "13800138000",
            Password = "password123",
            ConfirmPassword = "password123"
        };
        var validResult = await validationService.ValidateTestModelAsync(validModel);
        Console.WriteLine($"验证结果: {(validResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"验证状态: {(validResult.ValidationResult?.IsValid ? "通过" : "失败")}");
        Console.WriteLine($"执行时间: {validResult.ExecutionTimeMs} ms");
        
        Console.WriteLine("\n2. 验证无效的测试模型");
        var invalidModel = new TestModel
        {
            Name = "", // 空姓名
            Age = 0, // 无效年龄
            Email = "invalid-email", // 无效邮箱
            Phone = "123", // 无效手机号
            Password = "123", // 密码太短
            ConfirmPassword = "456" // 密码不一致
        };
        var invalidResult = await validationService.ValidateTestModelAsync(invalidModel);
        Console.WriteLine($"验证结果: {(invalidResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"验证状态: {(invalidResult.ValidationResult?.IsValid ? "通过" : "失败")}");
        Console.WriteLine($"执行时间: {invalidResult.ExecutionTimeMs} ms");
        
        if (!invalidResult.ValidationResult?.IsValid ?? false && invalidResult.ValidationResult?.Errors.Any() == true)
        {
            Console.WriteLine("\n验证错误:");
            foreach (var error in invalidResult.ValidationResult.Errors)
            {
                Console.WriteLine($"  {error.PropertyName}: {error.ErrorMessage}");
            }
        }
        
        Console.WriteLine("\n3. 获取验证器信息");
        var validatorsResult = await validationService.GetValidatorsAsync();
        Console.WriteLine($"获取验证器信息结果: {(validatorsResult.Success ? "成功" : "失败")}");
        foreach (var item in validatorsResult.Results)
        {
            Console.WriteLine($"- {item}");
        }
        
        Console.WriteLine("\n4. 获取版本信息");
        var versionResult = await validationService.GetVersionInfoAsync();
        Console.WriteLine($"获取版本信息结果: {(versionResult.Success ? "成功" : "失败")}");
        foreach (var item in versionResult.Results)
        {
            Console.WriteLine($"- {item}");
        }
    }
    
    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) => {
                // 配置 FluentValidation 选项
                services.Configure<FluentValidationOptions>(options => {
                    options.WorkingDirectory = Environment.CurrentDirectory;
                    options.EnableVersioning = false;
                    options.EnablePerformanceMonitoring = true;
                });
                
                // 注册服务
                services.AddSingleton<IValidator<TestModel>, TestModelValidator>();
                services.AddSingleton<IFluentValidationService, FluentValidationService>();
                services.AddSingleton<FluentValidationAotEngine>();
            });
    }
}
`

### 2. 高级配置示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using FluentValidation.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("FluentValidation 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) => {
                // 配置 FluentValidation 设置
                services.Configure<FluentValidationOptions>(options => {
                    options.WorkingDirectory = Environment.CurrentDirectory;
                    options.EnableDetailedLogging = true;
                    options.EnablePerformanceMonitoring = true;
                    options.RequestTimeoutMs = 60000; // 60秒超时
                    options.EnableCache = true;
                    options.CacheSize = 2000;
                });
                
                // 注册服务
                services.AddSingleton<IValidator<TestModel>, TestModelValidator>();
                services.AddSingleton<IFluentValidationService, FluentValidationService>();
                services.AddSingleton<FluentValidationAotEngine>();
            })
            .Build();
        
        // 获取配置
        var fluentValidationOptions = host.Services.GetRequiredService<IOptions<FluentValidationOptions>>().Value;
        Console.WriteLine("配置信息:");
        Console.WriteLine($"- 工作目录: {fluentValidationOptions.WorkingDirectory}");
        Console.WriteLine($"- 启用详细日志: {fluentValidationOptions.EnableDetailedLogging}");
        Console.WriteLine($"- 启用性能监控: {fluentValidationOptions.EnablePerformanceMonitoring}");
        Console.WriteLine($"- 请求超时时间: {fluentValidationOptions.RequestTimeoutMs} ms");
        Console.WriteLine($"- 启用缓存: {fluentValidationOptions.EnableCache}");
        Console.WriteLine($"- 缓存大小: {fluentValidationOptions.CacheSize}");
        
        // 使用服务
        var validationService = host.Services.GetRequiredService<IFluentValidationService>();
        
        // 验证测试模型
        var testModel = new TestModel
        {
            Name = "李四",
            Age = 30,
            Email = "lisi@example.com",
            Phone = "13900139000",
            Password = "securepass123",
            ConfirmPassword = "securepass123"
        };
        var result = await validationService.ValidateTestModelAsync(testModel);
        Console.WriteLine($"\n验证结果: {(result.Success ? "成功" : "失败")}");
        Console.WriteLine($"验证状态: {(result.ValidationResult?.IsValid ? "通过" : "失败")}");
        Console.WriteLine($"执行时间: {result.ExecutionTimeMs} ms");
        
        if (result.Success)
        {
            Console.WriteLine("\n验证成功的模型信息:");
            foreach (var item in result.Results)
            {
                Console.WriteLine($"- {item}");
            }
        }
    }
}
`

### 3. 性能优化示例

`csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FluentValidation.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("FluentValidation 性能优化示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) => {
                // 配置 FluentValidation 选项
                services.Configure<FluentValidationOptions>(options => {
                    options.WorkingDirectory = Environment.CurrentDirectory;
                    options.EnablePerformanceMonitoring = true;
                    options.EnableCache = true; // 启用缓存以提高性能
                    options.CacheSize = 2000;
                });
                
                // 注册服务
                services.AddSingleton<IValidator<TestModel>, TestModelValidator>();
                services.AddSingleton<IFluentValidationService, FluentValidationService>();
                services.AddSingleton<FluentValidationAotEngine>();
            })
            .Build();
        
        var validationService = host.Services.GetRequiredService<IFluentValidationService>();
        
        // 性能测试
        const int iterations = 1000;
        var totalTime = 0L;
        var validModels = 0;
        var invalidModels = 0;
        
        Console.WriteLine($"测试 {iterations} 次验证操作的性能...");
        
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            // 交替验证有效和无效的模型
            TestModel model;
            if (i % 2 == 0)
            {
                // 有效模型
                model = new TestModel
                {
                    Name = $"测试用户{i}",
                    Age = 20 + (i % 50),
                    Email = $"user{i}@example.com",
                    Phone = "13800138000",
                    Password = "password123",
                    ConfirmPassword = "password123"
                };
                validModels++;
            }
            else
            {
                // 无效模型
                model = new TestModel
                {
                    Name = i % 3 == 0 ? "" : $"测试用户{i}",
                    Age = i % 3 == 0 ? 0 : 20 + (i % 50),
                    Email = i % 3 == 0 ? "invalid-email" : $"user{i}@example.com",
                    Phone = "123",
                    Password = "123",
                    ConfirmPassword = "456"
                };
                invalidModels++;
            }
            
            var result = await validationService.ValidateTestModelAsync(model);
            if (result.Success)
            {
                totalTime += result.ExecutionTimeMs;
            }
            
            // 每100次迭代显示进度
            if ((i + 1) % 100 == 0)
            {
                Console.WriteLine($"已完成 {i + 1}/{iterations} 次迭代");
            }
        }
        
        stopwatch.Stop();
        
        Console.WriteLine("\n性能测试结果:");
        Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"验证操作总时间: {totalTime:F3} ms");
        Console.WriteLine($"平均每次验证时间: {totalTime / (double)iterations:F3} ms");
        Console.WriteLine($"验证有效模型数量: {validModels}");
        Console.WriteLine($"验证无效模型数量: {invalidModels}");
        Console.WriteLine($"每秒可处理验证次数: {iterations / stopwatch.Elapsed.TotalSeconds:F2}");
    }
}
`

### 4. 错误处理示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FluentValidation.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("FluentValidation 错误处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) => {
                // 配置 FluentValidation 选项
                services.Configure<FluentValidationOptions>(options => {
                    options.WorkingDirectory = Environment.CurrentDirectory;
                    options.EnablePerformanceMonitoring = true;
                });
                
                // 注册服务
                services.AddSingleton<IValidator<TestModel>, TestModelValidator>();
                services.AddSingleton<IFluentValidationService, FluentValidationService>();
                services.AddSingleton<FluentValidationAotEngine>();
            })
            .Build();
        
        var validationService = host.Services.GetRequiredService<IFluentValidationService>();
        
        Console.WriteLine("1. 验证各种错误情况");
        
        // 测试1: 空模型
        Console.WriteLine("\n测试1: 空模型");
        var emptyModel = new TestModel();
        var emptyResult = await validationService.ValidateTestModelAsync(emptyModel);
        Console.WriteLine($"验证结果: {(emptyResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"验证状态: {(emptyResult.ValidationResult?.IsValid ? "通过" : "失败")}");
        Console.WriteLine($"错误数量: {emptyResult.ValidationResult?.Errors.Count ?? 0}");
        
        // 测试2: 部分无效模型
        Console.WriteLine("\n测试2: 部分无效模型");
        var partialModel = new TestModel
        {
            Name = "张三",
            Age = 25,
            Email = "zhangsan@example.com",
            Phone = "13800138000",
            Password = "password123",
            ConfirmPassword = "different-password" // 密码不一致
        };
        var partialResult = await validationService.ValidateTestModelAsync(partialModel);
        Console.WriteLine($"验证结果: {(partialResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"验证状态: {(partialResult.ValidationResult?.IsValid ? "通过" : "失败")}");
        Console.WriteLine($"错误数量: {partialResult.ValidationResult?.Errors.Count ?? 0}");
        if (partialResult.ValidationResult?.Errors.Any() == true)
        {
            foreach (var error in partialResult.ValidationResult.Errors)
            {
                Console.WriteLine($"- {error.PropertyName}: {error.ErrorMessage}");
            }
        }
        
        // 测试3: 使用 JSON 验证
        Console.WriteLine("\n测试3: 使用 JSON 验证");
        var parameters = new System.Collections.Generic.Dictionary<string, string>
        {
            { "model", '{"name":"李四","age":30,"email":"lisi@example.com"}' }
        };
        var jsonResult = await validationService.ExecuteCommandAsync(ValidationCommandType.Validate, parameters);
        Console.WriteLine($"验证结果: {(jsonResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"验证状态: {(jsonResult.ValidationResult?.IsValid ? "通过" : "失败")}");
        
        // 测试4: 正确操作示例
        Console.WriteLine("\n测试4: 正确操作示例");
        var correctModel = new TestModel
        {
            Name = "王五",
            Age = 35,
            Email = "wangwu@example.com",
            Phone = "13900139000",
            Password = "securepass123",
            ConfirmPassword = "securepass123"
        };
        var correctResult = await validationService.ValidateTestModelAsync(correctModel);
        Console.WriteLine($"验证结果: {(correctResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"验证状态: {(correctResult.ValidationResult?.IsValid ? "通过" : "失败")}");
        Console.WriteLine($"执行时间: {correctResult.ExecutionTimeMs} ms");
    }
}
`

### 5. 命令行工具使用示例

`bash
# 显示帮助信息
fluentvalidation_aot help

# 验证 JSON 格式的模型
fluentvalidation_aot validate '{"name":"张三","age":25,"email":"zhangsan@example.com"}'

# 验证测试模型
fluentvalidation_aot validatemodel 张三 25 zhangsan@example.com 13800138000 password123 password123

# 验证无效的测试模型
fluentvalidation_aot validatemodel "" 0 invalid-email 123 123 456

# 获取验证器信息
fluentvalidation_aot validators

# 显示版本信息
fluentvalidation_aot version
`

## 总结

以上示例展示了 FluentValidation 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本验证操作
2. 配置高级选项，如缓存和超时
3. 优化性能，适用于高并发场景
4. 正确处理验证错误情况
5. 使用命令行工具进行验证操作

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

### 关键特性回顾

- **AOT 编译**: 提前编译为本地代码，减少启动时间和内存占用
- **完整的验证功能**: 支持必填字段、长度限制、格式验证等
- **详细的验证结果**: 返回详细的验证错误信息
- **高性能设计**: 支持缓存和异步操作
- **可扩展性**: 易于添加自定义验证器和验证规则
- **命令行支持**: 提供方便的命令行工具

FluentValidation AOT 引擎是一个功能强大、性能优异的验证解决方案，可满足各种验证场景的需求。
