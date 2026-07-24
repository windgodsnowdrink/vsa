# Oqtane 技能使用示例

## 概述

本文档提供了 Oqtane 技能的各种使用示例，包括基本用法、高级配置、性能优化、错误处理等。这些示例旨在帮助您快速上手 Oqtane 功能，并了解其在不同场景下的应用。

## 示例 1: 基本使用

### 功能说明

演示 Oqtane 技能的基本使用方法，包括服务注册、模块管理和主题管理。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Oqtane 基本使用示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 Oqtane 服务
        services.AddOqtaneServices(options =>
        {
            options.Enabled = true;
            options.SiteName = "示例站点";
            options.DefaultLanguage = "zh-CN";
            options.EnableModules = true;
            options.EnableThemes = true;
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var oqtaneService = serviceProvider.GetRequiredService<IOqtaneService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 示例 1: 获取已安装的模块
            Console.WriteLine("示例 1: 获取已安装的模块");
            var modules = await oqtaneService.GetModulesAsync();
            Console.WriteLine($"已安装的模块数量: {modules.Count()}");
            foreach (var module in modules)
            {
                Console.WriteLine($"- {module.Name} ({module.Version})");
            }

            // 示例 2: 安装新模块
            Console.WriteLine("\n示例 2: 安装新模块");
            var newModule = await oqtaneService.InstallModuleAsync("Blog", "1.0.0");
            Console.WriteLine($"模块安装成功: {newModule.Name} v{newModule.Version}");

            // 示例 3: 获取已安装的主题
            Console.WriteLine("\n示例 3: 获取已安装的主题");
            var themes = await oqtaneService.GetThemesAsync();
            Console.WriteLine($"已安装的主题数量: {themes.Count()}");
            foreach (var theme in themes)
            {
                Console.WriteLine($"- {theme.Name} ({theme.Version}) {(theme.IsDefault ? "[默认]" : "")}");
            }

            // 示例 4: 安装新主题
            Console.WriteLine("\n示例 4: 安装新主题");
            var newTheme = await oqtaneService.InstallThemeAsync("Modern", "1.0.0");
            Console.WriteLine($"主题安装成功: {newTheme.Name} v{newTheme.Version}");

            // 示例 5: 设置默认主题
            Console.WriteLine("\n示例 5: 设置默认主题");
            var setDefaultResult = await oqtaneService.SetDefaultThemeAsync("Modern");
            Console.WriteLine($"设置默认主题: {(setDefaultResult ? "成功" : "失败")}");

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

演示 Oqtane 技能的高级配置选项，包括用户管理、配置管理和部署工具。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Oqtane 高级配置示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 Oqtane 服务并配置所有选项
        services.AddOqtaneServices(options =>
        {
            options.Enabled = true;
            options.SiteName = "高级配置示例";
            options.DefaultLanguage = "zh-CN";
            options.EnableModules = true;
            options.EnableThemes = true;
            options.EnableUsers = true;
            options.EnableConfiguration = true;
            options.EnableDeployment = true;
            options.EnableParallelProcessing = true;
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var oqtaneService = serviceProvider.GetRequiredService<IOqtaneService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 示例 1: 用户管理
            Console.WriteLine("示例 1: 用户管理");
            
            // 创建用户
            var user = await oqtaneService.CreateUserAsync("testuser", "test@example.com", "Password123!");
            Console.WriteLine($"用户创建成功: {user.Username}");
            
            // 添加用户到角色
            var addRoleResult = await oqtaneService.AddUserToRoleAsync("testuser", "Users");
            Console.WriteLine($"添加用户到角色: {(addRoleResult ? "成功" : "失败")}");
            
            // 获取所有用户
            var users = await oqtaneService.GetUsersAsync();
            Console.WriteLine($"用户数量: {users.Count()}");
            foreach (var u in users)
            {
                Console.WriteLine($"- {u.Username} ({u.Email}) - 角色: {string.Join(", ", u.Roles)}");
            }

            // 示例 2: 配置管理
            Console.WriteLine("\n示例 2: 配置管理");
            
            // 获取当前配置
            var config = await oqtaneService.GetConfigurationAsync();
            Console.WriteLine($"当前站点名称: {config.SiteName}");
            Console.WriteLine($"当前默认语言: {config.DefaultLanguage}");
            
            // 更新配置
            var newConfig = new OqtaneConfiguration
            {
                SiteName = "更新后的站点",
                DefaultLanguage = "zh-CN",
                ConnectionString = config.ConnectionString,
                EnableSsl = config.EnableSsl,
                Port = config.Port,
                AdminEmail = config.AdminEmail,
                EnableLogging = config.EnableLogging,
                LogLevel = config.LogLevel
            };
            var updateConfigResult = await oqtaneService.UpdateConfigurationAsync(newConfig);
            Console.WriteLine($"更新配置: {(updateConfigResult ? "成功" : "失败")}");
            
            // 验证配置更新
            var updatedConfig = await oqtaneService.GetConfigurationAsync();
            Console.WriteLine($"更新后的站点名称: {updatedConfig.SiteName}");

            // 示例 3: 部署工具
            Console.WriteLine("\n示例 3: 部署工具");
            
            // 打包应用
            var outputPath = Environment.CurrentDirectory;
            var packageResult = await oqtaneService.PackageApplicationAsync(outputPath);
            Console.WriteLine($"打包应用: {(packageResult ? "成功" : "失败")}");

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

## 示例 3: 性能优化

### 功能说明

演示 Oqtane 技能的性能优化技术，包括并行处理和缓存。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
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

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Oqtane 性能优化示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 Oqtane 服务并配置性能优化选项
        services.AddOqtaneServices(options =>
        {
            options.Enabled = true;
            options.SiteName = "性能优化示例";
            options.DefaultLanguage = "zh-CN";
            options.EnableModules = true;
            options.EnableThemes = true;
            options.EnableUsers = true;
            options.EnableConfiguration = true;
            options.EnableDeployment = true;
            options.EnableParallelProcessing = true;
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var oqtaneService = serviceProvider.GetRequiredService<IOqtaneService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 示例 1: 并行安装多个模块
            Console.WriteLine("示例 1: 并行安装多个模块");
            var stopwatch = Stopwatch.StartNew();
            
            var moduleNames = new[] { "Blog", "Forum", "Gallery", "Calendar", "News" };
            var moduleTasks = new List<Task>();
            
            // 使用数据流进行并行处理
            var transformBlock = new TransformBlock<string, string>(
                async moduleName =>
                {
                    await oqtaneService.InstallModuleAsync(moduleName, "1.0.0");
                    return moduleName;
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                }
            );
            
            var actionBlock = new ActionBlock<string>(
                moduleName => Console.WriteLine($"模块安装完成: {moduleName}")
            );
            
            transformBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });
            
            // 发布所有模块名称
            foreach (var moduleName in moduleNames)
            {
                await transformBlock.SendAsync(moduleName);
            }
            
            transformBlock.Complete();
            await actionBlock.Completion;
            
            stopwatch.Stop();
            Console.WriteLine($"并行安装 {moduleNames.Length} 个模块耗时: {stopwatch.ElapsedMilliseconds}ms");

            // 示例 2: 性能测试
            Console.WriteLine("\n示例 2: 性能测试");
            stopwatch.Restart();
            
            // 执行多次操作以测试性能
            const int iterations = 100;
            for (int i = 0; i < iterations; i++)
            {
                // 模拟获取模块列表的操作
                await oqtaneService.GetModulesAsync();
            }
            
            stopwatch.Stop();
            Console.WriteLine($"执行 {iterations} 次获取模块操作耗时: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"平均每次操作耗时: {stopwatch.ElapsedMilliseconds / (double)iterations:F2}ms");

            // 示例 3: 内存使用测试
            Console.WriteLine("\n示例 3: 内存使用测试");
            
            // 获取当前内存使用情况
            var process = Process.GetCurrentProcess();
            var memoryBefore = process.WorkingSet64 / 1024 / 1024; // 转换为 MB
            Console.WriteLine($"操作前内存使用: {memoryBefore} MB");
            
            // 执行大量操作
            for (int i = 0; i < 1000; i++)
            {
                await oqtaneService.GetModulesAsync();
            }
            
            // 获取操作后的内存使用情况
            process.Refresh();
            var memoryAfter = process.WorkingSet64 / 1024 / 1024; // 转换为 MB
            Console.WriteLine($"操作后内存使用: {memoryAfter} MB");
            Console.WriteLine($"内存使用变化: {memoryAfter - memoryBefore} MB");

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

## 示例 4: 错误处理

### 功能说明

演示 Oqtane 技能的错误处理机制，包括异常捕获、错误日志记录和优雅错误处理。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Oqtane 错误处理示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 Oqtane 服务
        services.AddOqtaneServices(options =>
        {
            options.Enabled = true;
            options.SiteName = "错误处理示例";
            options.DefaultLanguage = "zh-CN";
            options.EnableModules = true;
            options.EnableThemes = true;
            options.EnableUsers = true;
            options.EnableConfiguration = true;
            options.EnableDeployment = true;
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var oqtaneService = serviceProvider.GetRequiredService<IOqtaneService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 测试 1: 正常操作
            Console.WriteLine("测试 1: 正常操作");
            try
            {
                var modules = await oqtaneService.GetModulesAsync();
                Console.WriteLine($"获取模块成功，数量: {modules.Count()}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取模块失败");
                Console.WriteLine($"获取模块失败: {ex.Message}");
            }

            // 测试 2: 重复安装模块
            Console.WriteLine("\n测试 2: 重复安装模块");
            try
            {
                // 先安装一个模块
                await oqtaneService.InstallModuleAsync("TestModule", "1.0.0");
                Console.WriteLine("第一次安装模块成功");
                
                // 尝试再次安装同一个模块
                await oqtaneService.InstallModuleAsync("TestModule", "1.0.0");
                Console.WriteLine("第二次安装模块成功");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "安装模块失败");
                Console.WriteLine($"安装模块失败: {ex.Message}");
            }

            // 测试 3: 卸载不存在的模块
            Console.WriteLine("\n测试 3: 卸载不存在的模块");
            try
            {
                var result = await oqtaneService.UninstallModuleAsync("NonExistentModule");
                Console.WriteLine($"卸载模块结果: {(result ? "成功" : "失败")}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "卸载模块失败");
                Console.WriteLine($"卸载模块失败: {ex.Message}");
            }

            // 测试 4: 创建用户时的密码策略错误
            Console.WriteLine("\n测试 4: 创建用户时的密码策略错误");
            try
            {
                // 使用弱密码
                var user = await oqtaneService.CreateUserAsync("weakuser", "weak@example.com", "123456");
                Console.WriteLine($"用户创建成功: {user.Username}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "创建用户失败");
                Console.WriteLine($"创建用户失败: {ex.Message}");
            }

            // 测试 5: 部署到无效服务器
            Console.WriteLine("\n测试 5: 部署到无效服务器");
            try
            {
                var result = await oqtaneService.DeployApplicationAsync("invalid-server", "username", "password");
                Console.WriteLine($"部署应用结果: {(result ? "成功" : "失败")}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "部署应用失败");
                Console.WriteLine($"部署应用失败: {ex.Message}");
            }

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

## 示例 5: 自定义 Oqtane 服务

### 功能说明

演示如何创建自定义的 Oqtane 服务，扩展基本功能以满足特定需求。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

// 自定义配置选项
public class CustomOqtaneOptions
{
    public bool Enabled { get; set; } = true;
    public string CustomSetting { get; set; } = "Default Value";
    public int CustomTimeout { get; set; } = 30;
}

// 自定义 Oqtane 服务接口
public interface ICustomOqtaneService
{
    Task<string> GetCustomSettingAsync();
    Task<bool> UpdateCustomSettingAsync(string value);
    Task<int> GetCustomTimeoutAsync();
    Task<bool> UpdateCustomTimeoutAsync(int timeout);
    Task<string> GetCombinedSettingsAsync();
}

// 自定义 Oqtane 服务实现
public class CustomOqtaneService : ICustomOqtaneService
{
    private readonly CustomOqtaneOptions _options;
    private readonly IOqtaneService _oqtaneService;
    private readonly ILogger<CustomOqtaneService> _logger;

    public CustomOqtaneService(
        IOptions<CustomOqtaneOptions> options,
        IOqtaneService oqtaneService,
        ILogger<CustomOqtaneService> logger)
    {
        _options = options.Value;
        _oqtaneService = oqtaneService;
        _logger = logger;
    }

    public async Task<string> GetCustomSettingAsync()
    {
        _logger.LogInformation("Getting custom setting");
        await Task.Delay(100);
        return _options.CustomSetting;
    }

    public async Task<bool> UpdateCustomSettingAsync(string value)
    {
        _logger.LogInformation("Updating custom setting to: {Value}", value);
        await Task.Delay(100);
        // 注意：在实际应用中，这里应该更新配置存储
        return true;
    }

    public async Task<int> GetCustomTimeoutAsync()
    {
        _logger.LogInformation("Getting custom timeout");
        await Task.Delay(100);
        return _options.CustomTimeout;
    }

    public async Task<bool> UpdateCustomTimeoutAsync(int timeout)
    {
        _logger.LogInformation("Updating custom timeout to: {Timeout}", timeout);
        await Task.Delay(100);
        // 注意：在实际应用中，这里应该更新配置存储
        return true;
    }

    public async Task<string> GetCombinedSettingsAsync()
    {
        _logger.LogInformation("Getting combined settings");
        
        // 获取 Oqtane 配置
        var config = await _oqtaneService.GetConfigurationAsync();
        
        // 获取自定义设置
        var customSetting = await GetCustomSettingAsync();
        var customTimeout = await GetCustomTimeoutAsync();
        
        return $"SiteName: {config.SiteName}, CustomSetting: {customSetting}, CustomTimeout: {customTimeout}";
    }
}

// 依赖注入扩展
public static class CustomOqtaneServiceCollectionExtensions
{
    public static IServiceCollection AddCustomOqtaneServices(this IServiceCollection services, Action<CustomOqtaneOptions> configureOptions = null)
    {
        // 配置选项
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<CustomOqtaneOptions>(options => { });
        }

        // 注册自定义服务
        services.AddSingleton<ICustomOqtaneService, CustomOqtaneService>();

        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Oqtane 自定义服务示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 Oqtane 服务
        services.AddOqtaneServices(options =>
        {
            options.Enabled = true;
            options.SiteName = "自定义服务示例";
            options.DefaultLanguage = "zh-CN";
        });

        // 注册自定义 Oqtane 服务
        services.AddCustomOqtaneServices(options =>
        {
            options.Enabled = true;
            options.CustomSetting = "自定义值";
            options.CustomTimeout = 60;
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var customOqtaneService = serviceProvider.GetRequiredService<ICustomOqtaneService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 示例 1: 获取自定义设置
            Console.WriteLine("示例 1: 获取自定义设置");
            var customSetting = await customOqtaneService.GetCustomSettingAsync();
            Console.WriteLine($"自定义设置值: {customSetting}");

            // 示例 2: 更新自定义设置
            Console.WriteLine("\n示例 2: 更新自定义设置");
            var updateSettingResult = await customOqtaneService.UpdateCustomSettingAsync("新的自定义值");
            Console.WriteLine($"更新自定义设置: {(updateSettingResult ? "成功" : "失败")}");

            // 示例 3: 获取自定义超时
            Console.WriteLine("\n示例 3: 获取自定义超时");
            var customTimeout = await customOqtaneService.GetCustomTimeoutAsync();
            Console.WriteLine($"自定义超时值: {customTimeout}");

            // 示例 4: 更新自定义超时
            Console.WriteLine("\n示例 4: 更新自定义超时");
            var updateTimeoutResult = await customOqtaneService.UpdateCustomTimeoutAsync(120);
            Console.WriteLine($"更新自定义超时: {(updateTimeoutResult ? "成功" : "失败")}");

            // 示例 5: 获取组合设置
            Console.WriteLine("\n示例 5: 获取组合设置");
            var combinedSettings = await customOqtaneService.GetCombinedSettingsAsync();
            Console.WriteLine($"组合设置: {combinedSettings}");

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

## 示例 6: 实际应用场景

### 功能说明

演示 Oqtane 技能在实际应用场景中的使用，如内容管理系统、企业门户网站和电子商务网站等。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Oqtane 实际应用场景示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 Oqtane 服务
        services.AddOqtaneServices(options =>
        {
            options.Enabled = true;
            options.SiteName = "实际应用示例";
            options.DefaultLanguage = "zh-CN";
            options.EnableModules = true;
            options.EnableThemes = true;
            options.EnableUsers = true;
            options.EnableConfiguration = true;
            options.EnableDeployment = true;
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var oqtaneService = serviceProvider.GetRequiredService<IOqtaneService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 场景 1: 内容管理系统 (CMS)
            Console.WriteLine("场景 1: 内容管理系统 (CMS)");
            
            // 安装 CMS 相关模块
            await oqtaneService.InstallModuleAsync("Blog", "1.0.0");
            await oqtaneService.InstallModuleAsync("News", "1.0.0");
            await oqtaneService.InstallModuleAsync("Pages", "1.0.0");
            await oqtaneService.InstallModuleAsync("Media", "1.0.0");
            
            // 安装适合 CMS 的主题
            await oqtaneService.InstallThemeAsync("CMSTheme", "1.0.0");
            await oqtaneService.SetDefaultThemeAsync("CMSTheme");
            
            // 创建 CMS 管理员用户
            await oqtaneService.CreateUserAsync("cmsadmin", "cmsadmin@example.com", "CmsAdmin123!");
            await oqtaneService.AddUserToRoleAsync("cmsadmin", "Administrators");
            
            Console.WriteLine("内容管理系统配置完成");

            // 场景 2: 企业门户网站
            Console.WriteLine("\n场景 2: 企业门户网站");
            
            // 安装企业门户相关模块
            await oqtaneService.InstallModuleAsync("About", "1.0.0");
            await oqtaneService.InstallModuleAsync("Contact", "1.0.0");
            await oqtaneService.InstallModuleAsync("Services", "1.0.0");
            await oqtaneService.InstallModuleAsync("Careers", "1.0.0");
            
            // 安装适合企业门户的主题
            await oqtaneService.InstallThemeAsync("CorporateTheme", "1.0.0");
            await oqtaneService.SetDefaultThemeAsync("CorporateTheme");
            
            // 创建企业门户管理员用户
            await oqtaneService.CreateUserAsync("portaladmin", "portaladmin@example.com", "PortalAdmin123!");
            await oqtaneService.AddUserToRoleAsync("portaladmin", "Administrators");
            
            Console.WriteLine("企业门户网站配置完成");

            // 场景 3: 电子商务网站
            Console.WriteLine("\n场景 3: 电子商务网站");
            
            // 安装电子商务相关模块
            await oqtaneService.InstallModuleAsync("Products", "1.0.0");
            await oqtaneService.InstallModuleAsync("ShoppingCart", "1.0.0");
            await oqtaneService.InstallModuleAsync("Checkout", "1.0.0");
            await oqtaneService.InstallModuleAsync("Payment", "1.0.0");
            
            // 安装适合电子商务的主题
            await oqtaneService.InstallThemeAsync("ECommerceTheme", "1.0.0");
            await oqtaneService.SetDefaultThemeAsync("ECommerceTheme");
            
            // 创建电子商务管理员用户
            await oqtaneService.CreateUserAsync("ecomadmin", "ecomadmin@example.com", "EcomAdmin123!");
            await oqtaneService.AddUserToRoleAsync("ecomadmin", "Administrators");
            
            Console.WriteLine("电子商务网站配置完成");

            // 场景 4: 教育网站
            Console.WriteLine("\n场景 4: 教育网站");
            
            // 安装教育相关模块
            await oqtaneService.InstallModuleAsync("Courses", "1.0.0");
            await oqtaneService.InstallModuleAsync("Teachers", "1.0.0");
            await oqtaneService.InstallModuleAsync("Students", "1.0.0");
            await oqtaneService.InstallModuleAsync("Events", "1.0.0");
            
            // 安装适合教育网站的主题
            await oqtaneService.InstallThemeAsync("EducationTheme", "1.0.0");
            await oqtaneService.SetDefaultThemeAsync("EducationTheme");
            
            // 创建教育网站管理员用户
            await oqtaneService.CreateUserAsync("eduadmin", "eduadmin@example.com", "EduAdmin123!");
            await oqtaneService.AddUserToRoleAsync("eduadmin", "Administrators");
            
            Console.WriteLine("教育网站配置完成");

            // 场景 5: 社区网站
            Console.WriteLine("\n场景 5: 社区网站");
            
            // 安装社区相关模块
            await oqtaneService.InstallModuleAsync("Forum", "1.0.0");
            await oqtaneService.InstallModuleAsync("Chat", "1.0.0");
            await oqtaneService.InstallModuleAsync("Groups", "1.0.0");
            await oqtaneService.InstallModuleAsync("Events", "1.0.0");
            
            // 安装适合社区网站的主题
            await oqtaneService.InstallThemeAsync("CommunityTheme", "1.0.0");
            await oqtaneService.SetDefaultThemeAsync("CommunityTheme");
            
            // 创建社区网站管理员用户
            await oqtaneService.CreateUserAsync("communityadmin", "communityadmin@example.com", "CommunityAdmin123!");
            await oqtaneService.AddUserToRoleAsync("communityadmin", "Administrators");
            
            Console.WriteLine("社区网站配置完成");

            // 场景 6: 个人博客
            Console.WriteLine("\n场景 6: 个人博客");
            
            // 安装博客相关模块
            await oqtaneService.InstallModuleAsync("Blog", "1.0.0");
            await oqtaneService.InstallModuleAsync("Comments", "1.0.0");
            await oqtaneService.InstallModuleAsync("Analytics", "1.0.0");
            
            // 安装适合个人博客的主题
            await oqtaneService.InstallThemeAsync("BlogTheme", "1.0.0");
            await oqtaneService.SetDefaultThemeAsync("BlogTheme");
            
            // 创建博客管理员用户
            await oqtaneService.CreateUserAsync("blogadmin", "blogadmin@example.com", "BlogAdmin123!");
            await oqtaneService.AddUserToRoleAsync("blogadmin", "Administrators");
            
            Console.WriteLine("个人博客配置完成");

            // 查看最终状态
            Console.WriteLine("\n最终状态");
            var modules = await oqtaneService.GetModulesAsync();
            var themes = await oqtaneService.GetThemesAsync();
            var users = await oqtaneService.GetUsersAsync();
            
            Console.WriteLine($"已安装模块数量: {modules.Count()}");
            Console.WriteLine($"已安装主题数量: {themes.Count()}");
            Console.WriteLine($"已创建用户数量: {users.Count()}");

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

## 实际应用场景

### 场景 1: 内容管理系统 (CMS)

**功能说明**：构建一个完整的内容管理系统，用于管理网站内容。

**应用示例**：
- 安装博客、新闻、页面和媒体模块
- 配置适合 CMS 的主题
- 创建内容管理员用户
- 设置内容发布工作流

### 场景 2: 企业门户网站

**功能说明**：构建企业门户网站，展示公司信息和服务。

**应用示例**：
- 安装关于我们、联系我们、服务和招聘模块
- 配置专业的企业主题
- 创建企业管理员用户
- 集成企业社交媒体账号

### 场景 3: 电子商务网站

**功能说明**：构建电子商务网站，用于在线销售产品。

**应用示例**：
- 安装产品、购物车、结账和支付模块
- 配置适合电子商务的主题
- 创建电子商务管理员用户
- 集成支付网关和物流服务

### 场景 4: 教育网站

**功能说明**：构建教育网站，用于展示课程和学校信息。

**应用示例**：
- 安装课程、教师、学生和事件模块
- 配置适合教育机构的主题
- 创建教育管理员用户
- 集成学习管理系统 (LMS)

### 场景 5: 社区网站

**功能说明**：构建社区网站，用于用户交流和互动。

**应用示例**：
- 安装论坛、聊天、群组和事件模块
- 配置适合社区的主题
- 创建社区管理员用户
- 设置用户等级和权限系统

### 场景 6: 个人博客

**功能说明**：构建个人博客网站，用于分享个人观点和内容。

**应用示例**：
- 安装博客、评论和分析模块
- 配置适合个人博客的主题
- 创建博客管理员用户
- 集成内容分发网络 (CDN) 和搜索引擎优化 (SEO) 工具
