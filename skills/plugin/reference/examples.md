# 插件系统示例文档

## 1. 基础插件示例

### 1.1 简单插件

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class SimplePlugin : IPlugin
{
    private readonly ILogger<SimplePlugin> _logger;

    public SimplePlugin(ILogger<SimplePlugin> logger)
    {
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("初始化简单插件");
        await Task.CompletedTask;
    }

    public async Task<object> ExecuteAsync(object context)
    {
        _logger.LogInformation($"执行简单插件，上下文: {context}");
        await Task.Delay(50);
        return $"简单插件执行结果: {DateTime.Now}";
    }

    public async Task ShutdownAsync()
    {
        _logger.LogInformation("关闭简单插件");
        await Task.CompletedTask;
    }
}
```

### 1.2 带依赖注入的插件

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

public class ConfigurablePlugin : IPlugin
{
    private readonly ILogger<ConfigurablePlugin> _logger;
    private readonly IConfiguration _configuration;
    private string _pluginName;

    public ConfigurablePlugin(ILogger<ConfigurablePlugin> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InitializeAsync()
    {
        _pluginName = _configuration.GetValue<string>("PluginName", "默认插件");
        _logger.LogInformation($"初始化配置插件: {_pluginName}");
        await Task.CompletedTask;
    }

    public async Task<object> ExecuteAsync(object context)
    {
        _logger.LogInformation($"执行配置插件 {_pluginName}，上下文: {context}");
        await Task.Delay(100);
        return $"{_pluginName} 执行结果: {DateTime.Now}";
    }

    public async Task ShutdownAsync()
    {
        _logger.LogInformation($"关闭配置插件: {_pluginName}");
        await Task.CompletedTask;
    }
}
```

## 2. AOP 插件示例

### 2.1 带性能监控的插件

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plugin.AOP;

public class PerformancePlugin : IPlugin
{
    private readonly ILogger<PerformancePlugin> _logger;

    public PerformancePlugin(ILogger<PerformancePlugin> logger)
    {
        _logger = logger;
    }

    [PluginInterceptor]
    [PerformanceMonitor]
    public async Task InitializeAsync()
    {
        _logger.LogInformation("初始化性能监控插件");
        await Task.Delay(200);
    }

    [PluginInterceptor]
    [PerformanceMonitor]
    public async Task<object> ExecuteAsync(object context)
    {
        _logger.LogInformation($"执行性能监控插件，上下文: {context}");
        // 模拟耗时操作
        await Task.Delay(500);
        return $"性能监控插件执行结果: {DateTime.Now}";
    }

    [PluginInterceptor]
    [PerformanceMonitor]
    public async Task ShutdownAsync()
    {
        _logger.LogInformation("关闭性能监控插件");
        await Task.Delay(100);
    }
}
```

### 2.2 带缓存的插件

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plugin.AOP;

public class CachedPlugin : IPlugin
{
    private readonly ILogger<CachedPlugin> _logger;

    public CachedPlugin(ILogger<CachedPlugin> logger)
    {
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("初始化缓存插件");
        await Task.CompletedTask;
    }

    [Cache(300)] // 缓存5分钟
    public async Task<object> ExecuteAsync(object context)
    {
        _logger.LogInformation($"执行缓存插件，上下文: {context}");
        // 模拟耗时操作
        await Task.Delay(1000);
        return $"缓存插件执行结果: {DateTime.Now}";
    }

    public async Task ShutdownAsync()
    {
        _logger.LogInformation("关闭缓存插件");
        await Task.CompletedTask;
    }
}
```

### 2.3 带事务的插件

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plugin.AOP;

public class TransactionalPlugin : IPlugin
{
    private readonly ILogger<TransactionalPlugin> _logger;

    public TransactionalPlugin(ILogger<TransactionalPlugin> logger)
    {
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("初始化事务插件");
        await Task.CompletedTask;
    }

    [Transaction]
    public async Task<object> ExecuteAsync(object context)
    {
        _logger.LogInformation($"执行事务插件，上下文: {context}");
        // 模拟事务操作
        await Task.Delay(300);
        
        // 模拟异常
        if (context?.ToString()?.Contains("error") ?? false)
        {
            throw new Exception("模拟事务失败");
        }
        
        return $"事务插件执行结果: {DateTime.Now}";
    }

    public async Task ShutdownAsync()
    {
        _logger.LogInformation("关闭事务插件");
        await Task.CompletedTask;
    }
}
```

## 3. 动态代码生成示例

### 3.1 简单动态插件

```csharp
// 生成动态插件代码
var pluginCode = @"
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plugin.Core;

namespace DynamicPlugins
{
    public class SimpleDynamicPlugin : IPlugin
    {
        private readonly ILogger<SimpleDynamicPlugin> _logger;

        public SimpleDynamicPlugin(ILogger<SimpleDynamicPlugin> logger)
        {
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("初始化简单动态插件");
            await Task.CompletedTask;
        }

        public async Task<object> ExecuteAsync(object context)
        {
            _logger.LogInformation($"执行简单动态插件，上下文: {context}");
            await Task.Delay(100);
            return $"简单动态插件执行结果: {DateTime.Now}";
        }

        public async Task ShutdownAsync()
        {
            _logger.LogInformation("关闭简单动态插件");
            await Task.CompletedTask;
        }
    }
}
";

// 编译并加载动态插件
var natashaService = serviceProvider.GetRequiredService<INatashaIntegrationService>();
var pluginType = await natashaService.GenerateTypeAsync("SimpleDynamicPlugin", pluginCode);
var plugin = (IPlugin)Activator.CreateInstance(pluginType, serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(pluginType));

// 执行插件
await plugin.InitializeAsync();
var result = await plugin.ExecuteAsync("测试参数");
Console.WriteLine($"动态插件执行结果: {result}");
await plugin.ShutdownAsync();
```

### 3.2 带业务逻辑的动态插件

```csharp
// 生成带业务逻辑的动态插件
var businessPluginCode = @"
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plugin.Core;

namespace DynamicPlugins
{
    public class BusinessDynamicPlugin : IPlugin
    {
        private readonly ILogger<BusinessDynamicPlugin> _logger;
        private int _executionCount;

        public BusinessDynamicPlugin(ILogger<BusinessDynamicPlugin> logger)
        {
            _logger = logger;
            _executionCount = 0;
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("初始化业务动态插件");
            await Task.CompletedTask;
        }

        public async Task<object> ExecuteAsync(object context)
        {
            _executionCount++;
            _logger.LogInformation($"执行业务动态插件 (第 {_executionCount} 次)，上下文: {context}");
            
            // 模拟业务逻辑
            await Task.Delay(200);
            
            // 业务处理
            var input = context?.ToString() ?? "";
            var result = ProcessBusinessLogic(input);
            
            return $"业务动态插件执行结果 (第 {_executionCount} 次): {result}";
        }

        private string ProcessBusinessLogic(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "空输入";
            }
            else if (input.Contains("hello"))
            {
                return "Hello, World!";
            }
            else if (input.Contains("time"))
            {
                return DateTime.Now.ToString();
            }
            else
            {
                return $"处理输入: {input}";
            }
        }

        public async Task ShutdownAsync()
        {
            _logger.LogInformation($"关闭业务动态插件，共执行 {_executionCount} 次");
            await Task.CompletedTask;
        }
    }
}
";

// 编译并加载动态插件
var pluginType = await natashaService.GenerateTypeAsync("BusinessDynamicPlugin", businessPluginCode);
var plugin = (IPlugin)Activator.CreateInstance(pluginType, serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(pluginType));

// 执行插件
await plugin.InitializeAsync();
var result1 = await plugin.ExecuteAsync("hello");
var result2 = await plugin.ExecuteAsync("time");
var result3 = await plugin.ExecuteAsync("test");
Console.WriteLine($"结果1: {result1}");
Console.WriteLine($"结果2: {result2}");
Console.WriteLine($"结果3: {result3}");
await plugin.ShutdownAsync();
```

## 4. 高级插件示例

### 4.1 带配置选项的插件

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class OptionsPlugin : IPlugin
{
    private readonly ILogger<OptionsPlugin> _logger;
    private readonly PluginOptions _options;

    public OptionsPlugin(ILogger<OptionsPlugin> logger, IOptions<PluginOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation($"初始化选项插件: {_options.PluginName}");
        _logger.LogInformation($"插件描述: {_options.Description}");
        _logger.LogInformation($"执行超时: {_options.ExecutionTimeoutMs}ms");
        await Task.CompletedTask;
    }

    public async Task<object> ExecuteAsync(object context)
    {
        _logger.LogInformation($"执行选项插件 {_options.PluginName}，上下文: {context}");
        
        // 模拟执行
        await Task.Delay(Math.Min(500, _options.ExecutionTimeoutMs / 2));
        
        return $"{_options.PluginName} 执行结果: {DateTime.Now}";
    }

    public async Task ShutdownAsync()
    {
        _logger.LogInformation($"关闭选项插件: {_options.PluginName}");
        await Task.CompletedTask;
    }
}

// 插件选项类
public class PluginOptions
{
    public string PluginName { get; set; } = "默认插件";
    public string Description { get; set; } = "默认描述";
    public int ExecutionTimeoutMs { get; set; } = 1000;
}

// 注册选项
// services.Configure<PluginOptions>(configuration.GetSection("PluginOptions"));
```

### 4.2 带事件处理的插件

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

public class EventPlugin : IPlugin, IHostedService
{
    private readonly ILogger<EventPlugin> _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    private int _eventCount;

    public EventPlugin(ILogger<EventPlugin> logger, IHostApplicationLifetime appLifetime)
    {
        _logger = logger;
        _appLifetime = appLifetime;
        _eventCount = 0;
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("初始化事件插件");
        
        // 注册应用生命周期事件
        _appLifetime.ApplicationStarted.Register(OnApplicationStarted);
        _appLifetime.ApplicationStopping.Register(OnApplicationStopping);
        _appLifetime.ApplicationStopped.Register(OnApplicationStopped);
        
        await Task.CompletedTask;
    }

    public async Task<object> ExecuteAsync(object context)
    {
        _eventCount++;
        _logger.LogInformation($"执行事件插件 (第 {_eventCount} 次)，上下文: {context}");
        await Task.Delay(100);
        return $"事件插件执行结果 (第 {_eventCount} 次): {DateTime.Now}";
    }

    public async Task ShutdownAsync()
    {
        _logger.LogInformation($"关闭事件插件，共处理 {_eventCount} 个执行请求");
        await Task.CompletedTask;
    }

    private void OnApplicationStarted()
    {
        _logger.LogInformation("应用启动事件触发");
    }

    private void OnApplicationStopping()
    {
        _logger.LogInformation("应用停止中事件触发");
    }

    private void OnApplicationStopped()
    {
        _logger.LogInformation("应用已停止事件触发");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("启动事件插件服务");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("停止事件插件服务");
        return Task.CompletedTask;
    }
}
```

## 5. 插件服务使用示例

### 5.1 基本插件服务使用

```csharp
// 创建服务集合
var services = new ServiceCollection();

// 添加日志
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});

// 添加配置
services.AddConfiguration();

// 添加插件服务
services.AddPluginService();

// 构建服务提供程序
using var serviceProvider = services.BuildServiceProvider();

// 获取插件服务
var pluginService = serviceProvider.GetRequiredService<IPluginService>();

// 加载插件
var pluginPath = "path/to/plugin.dll";
var pluginInfo = await pluginService.LoadPluginAsync(pluginPath);
Console.WriteLine($"加载插件成功: {pluginInfo.Name}");

// 执行插件
var result = await pluginService.ExecutePluginAsync(pluginPath, "测试参数");
Console.WriteLine($"插件执行结果: {result}");

// 获取已加载插件
var loadedPlugins = await pluginService.GetLoadedPluginsAsync();
Console.WriteLine($"当前加载 {loadedPlugins.Count} 个插件:");
foreach (var p in loadedPlugins)
{
    Console.WriteLine($"- {p.Name} (版本: {p.Version})");
}

// 卸载插件
await pluginService.UnloadPluginAsync(pluginPath);
Console.WriteLine("插件卸载成功");
```

### 5.2 带热重载的插件服务

```csharp
// 创建服务集合
var services = new ServiceCollection();

// 添加日志
services.AddLogging(builder =>
{
    builder.AddConsole();
});

// 添加插件服务（启用热重载）
services.AddPluginService(options =>
{
    options.EnableHotReload = true;
    options.HotReloadIntervalMs = 2000;
    options.PluginDirectory = "plugins";
});

// 构建服务提供程序
using var serviceProvider = services.BuildServiceProvider();

// 获取插件服务
var pluginService = serviceProvider.GetRequiredService<IPluginService>();

// 启动热重载监控
await pluginService.StartHotReloadAsync();
Console.WriteLine("热重载监控已启动");

// 加载插件目录中的所有插件
var plugins = await pluginService.LoadPluginsFromDirectoryAsync("plugins");
Console.WriteLine($"加载 {plugins.Count} 个插件");

// 等待用户输入
Console.WriteLine("按任意键停止...");
Console.ReadKey();

// 停止热重载监控
await pluginService.StopHotReloadAsync();
Console.WriteLine("热重载监控已停止");
```

### 5.3 带性能监控的插件服务

```csharp
// 创建服务集合
var services = new ServiceCollection();

// 添加日志
services.AddLogging(builder =>
{
    builder.AddConsole();
});

// 添加插件服务（启用性能监控）
services.AddPluginService(options =>
{
    options.EnablePerformanceMetrics = true;
});

// 构建服务提供程序
using var serviceProvider = services.BuildServiceProvider();

// 获取插件服务
var pluginService = serviceProvider.GetRequiredService<IPluginService>();

// 加载插件
var pluginPath = "path/to/plugin.dll";
await pluginService.LoadPluginAsync(pluginPath);

// 执行插件多次
for (int i = 0; i < 5; i++)
{
    var result = await pluginService.ExecutePluginAsync(pluginPath, $"测试参数 {i}");
    Console.WriteLine($"执行结果 {i + 1}: {result}");
    await Task.Delay(500);
}

// 获取性能指标
var metrics = pluginService.GetPerformanceMetrics();
Console.WriteLine("\n性能指标:");
foreach (var (key, value) in metrics)
{
    Console.WriteLine($"{key}: {value}");
}

// 卸载插件
await pluginService.UnloadPluginAsync(pluginPath);
```

## 6. 配置示例

### 6.1 插件服务配置（appsettings.json）

```json
{
  "PluginService": {
    "PluginDirectory": "plugins",
    "EnableHotReload": true,
    "HotReloadIntervalMs": 2000,
    "EnableSandbox": false,
    "MaxConcurrentPlugins": 10,
    "EnablePerformanceMetrics": true,
    "PluginInitializationTimeoutMs": 30000,
    "PluginExecutionTimeoutMs": 60000
  }
}
```

### 6.2 插件配置（plugin.json）

```json
{
  "Name": "示例插件",
  "Version": "1.0.0",
  "Description": "这是一个示例插件",
  "Author": "插件作者",
  "Dependencies": [
    "Microsoft.Extensions.Logging",
    "Microsoft.Extensions.Configuration"
  ],
  "Settings": {
    "Timeout": 30000,
    "RetryCount": 3,
    "EnableCache": true
  }
}
```

### 6.3 AOT 编译配置（csproj）

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <PublishAot>true</PublishAot>
    <TrimMode>partial</TrimMode>
    <ReadyToRun>true</ReadyToRun>
    <TieredCompilation>true</TieredCompilation>
    <Optimize>true</Optimize>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="10.0.0" />
    <PackageReference Include="System.Threading.Channels" Version="10.0.0" />
    <PackageReference Include="Natasha" Version="3.0.0" />
    <PackageReference Include="Rougamo" Version="2.0.0" />
  </ItemGroup>

</Project>
```

## 7. 部署示例

### 7.1 Windows 部署脚本

```powershell
# 创建发布目录
New-Item -ItemType Directory -Path "publish" -Force

# 发布应用（AOT 编译）
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true -o "publish"

# 复制插件目录
Copy-Item -Path "plugins" -Destination "publish\plugins" -Recurse -Force

# 复制配置文件
Copy-Item -Path "appsettings.json" -Destination "publish\appsettings.json" -Force

# 启动应用
Set-Location "publish"
.luginService.exe
```

### 7.2 Linux 部署脚本

```bash
#!/bin/bash

# 创建发布目录
mkdir -p publish

# 发布应用（AOT 编译）
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true -o "publish"

# 复制插件目录
cp -r plugins publish/

# 复制配置文件
cp appsettings.json publish/

# 设置执行权限
chmod +x publish/PluginService

# 启动应用
cd publish
./PluginService
```

### 7.3 Docker 部署

#### 7.3.1 Dockerfile

```dockerfile
# 构建阶段
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# 复制项目文件
COPY . .

# 发布应用（AOT 编译）
RUN dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true -o out

# 运行阶段
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0 AS runtime
WORKDIR /app

# 复制发布文件
COPY --from=build /app/out .

# 复制插件目录和配置文件
COPY --from=build /app/plugins ./plugins
COPY --from=build /app/appsettings.json .

# 暴露端口
EXPOSE 5000

# 启动应用
ENTRYPOINT ["./PluginService"]
```

#### 7.3.2 Docker Compose

```yaml
version: '3.8'

services:
  plugin-service:
    build: .
    ports:
      - "5000:5000"
    volumes:
      - ./plugins:/app/plugins
      - ./appsettings.json:/app/appsettings.json
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - DOTNET_ENVIRONMENT=Production
```

## 8. 测试示例

### 8.1 单元测试

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

public class PluginTests
{
    [Fact]
    public async Task TestPluginInitialization()
    {
        // 创建服务集合
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddPluginService();
        
        using var serviceProvider = services.BuildServiceProvider();
        var pluginService = serviceProvider.GetRequiredService<IPluginService>();
        
        // 加载插件
        var pluginPath = "path/to/plugin.dll";
        var pluginInfo = await pluginService.LoadPluginAsync(pluginPath);
        
        // 验证插件加载成功
        Assert.NotNull(pluginInfo);
        Assert.Equal("TestPlugin", pluginInfo.Name);
        
        // 卸载插件
        await pluginService.UnloadPluginAsync(pluginPath);
    }
    
    [Fact]
    public async Task TestPluginExecution()
    {
        // 创建服务集合
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddPluginService();
        
        using var serviceProvider = services.BuildServiceProvider();
        var pluginService = serviceProvider.GetRequiredService<IPluginService>();
        
        // 加载插件
        var pluginPath = "path/to/plugin.dll";
        await pluginService.LoadPluginAsync(pluginPath);
        
        // 执行插件
        var result = await pluginService.ExecutePluginAsync(pluginPath, "测试参数");
        
        // 验证执行结果
        Assert.NotNull(result);
        Assert.IsType<string>(result);
        Assert.Contains("执行结果", result.ToString());
        
        // 卸载插件
        await pluginService.UnloadPluginAsync(pluginPath);
    }
    
    [Fact]
    public async Task TestPluginUnload()
    {
        // 创建服务集合
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddPluginService();
        
        using var serviceProvider = services.BuildServiceProvider();
        var pluginService = serviceProvider.GetRequiredService<IPluginService>();
        
        // 加载插件
        var pluginPath = "path/to/plugin.dll";
        await pluginService.LoadPluginAsync(pluginPath);
        
        // 验证插件已加载
        var loadedPlugins = await pluginService.GetLoadedPluginsAsync();
        Assert.Contains(loadedPlugins, p => p.Path == pluginPath);
        
        // 卸载插件
        await pluginService.UnloadPluginAsync(pluginPath);
        
        // 验证插件已卸载
        loadedPlugins = await pluginService.GetLoadedPluginsAsync();
        Assert.DoesNotContain(loadedPlugins, p => p.Path == pluginPath);
    }
}
```

### 8.2 集成测试

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

public class PluginServiceIntegrationTests
{
    [Fact]
    public async Task TestPluginServiceLifecycle()
    {
        // 创建主机
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddPluginService(options =>
                {
                    options.EnablePerformanceMetrics = true;
                });
            })
            .Build();
        
        // 启动主机
        await host.StartAsync();
        
        try
        {
            // 获取插件服务
            var pluginService = host.Services.GetRequiredService<IPluginService>();
            
            // 加载插件
            var pluginPath = "path/to/plugin.dll";
            var pluginInfo = await pluginService.LoadPluginAsync(pluginPath);
            
            // 验证插件加载成功
            Assert.NotNull(pluginInfo);
            
            // 执行插件
            var result = await pluginService.ExecutePluginAsync(pluginPath, "集成测试参数");
            Assert.NotNull(result);
            
            // 获取性能指标
            var metrics = pluginService.GetPerformanceMetrics();
            Assert.NotEmpty(metrics);
            
            // 卸载插件
            await pluginService.UnloadPluginAsync(pluginPath);
        }
        finally
        {
            // 停止主机
            await host.StopAsync();
        }
    }
}
```

## 9. 最佳实践示例

### 9.1 高性能插件

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plugin.AOP;

public class HighPerformancePlugin : IPlugin
{
    private readonly ILogger<HighPerformancePlugin> _logger;
    private int _executionCount;

    public HighPerformancePlugin(ILogger<HighPerformancePlugin> logger)
    {
        _logger = logger;
        _executionCount = 0;
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("初始化高性能插件");
        await Task.CompletedTask;
    }

    [Cache(600)] // 缓存10分钟
    public async Task<object> ExecuteAsync(object context)
    {
        _executionCount++;
        
        // 仅在缓存未命中时记录日志
        if (_executionCount % 10 == 1) // 每10次执行记录一次
        {
            _logger.LogInformation($"执行高性能插件 (第 {_executionCount} 次)，上下文: {context}");
        }
        
        // 高性能处理
        await Task.Yield(); // 避免线程阻塞
        
        // 快速处理逻辑
        var result = ProcessInput(context);
        
        return result;
    }

    private object ProcessInput(object context)
    {
        // 快速处理逻辑
        if (context == null)
        {
            return "空输入";
        }
        
        var input = context.ToString();
        if (string.IsNullOrEmpty(input))
        {
            return "空字符串";
        }
        
        // 简单处理
        return $"处理结果: {input.ToUpper()}";
    }

    public async Task ShutdownAsync()
    {
        _logger.LogInformation($"关闭高性能插件，共执行 {_executionCount} 次");
        await Task.CompletedTask;
    }
}
```

### 9.2 安全插件

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;

public class SecurePlugin : IPlugin
{
    private readonly ILogger<SecurePlugin> _logger;
    private readonly string _allowedDirectory;

    public SecurePlugin(ILogger<SecurePlugin> logger)
    {
        _logger = logger;
        _allowedDirectory = Path.Combine(AppContext.BaseDirectory, "data");
        
        // 确保允许的目录存在
        Directory.CreateDirectory(_allowedDirectory);
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("初始化安全插件");
        await Task.CompletedTask;
    }

    public async Task<object> ExecuteAsync(object context)
    {
        _logger.LogInformation("执行安全插件");
        
        try
        {
            var input = context?.ToString();
            if (string.IsNullOrEmpty(input))
            {
                return "空输入";
            }
            
            // 安全处理：验证路径
            if (input.Contains(".."))
            {
                throw new SecurityException("非法路径访问");
            }
            
            // 安全处理：限制文件操作范围
            var fileName = Path.GetFileName(input);
            var filePath = Path.Combine(_allowedDirectory, fileName);
            
            // 执行安全操作
            var result = await ProcessFileAsync(filePath);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "安全插件执行异常");
            return $"错误: {ex.Message}";
        }
    }

    private async Task<string> ProcessFileAsync(string filePath)
    {
        // 安全的文件操作
        if (File.Exists(filePath))
        {
            var content = await File.ReadAllTextAsync(filePath);
            return $"文件内容: {content.Substring(0, Math.Min(100, content.Length))}...";
        }
        else
        {
            await File.WriteAllTextAsync(filePath, $"创建于: {DateTime.Now}");
            return "文件已创建";
        }
    }

    public async Task ShutdownAsync()
    {
        _logger.LogInformation("关闭安全插件");
        await Task.CompletedTask;
    }
}
```

### 9.3 可维护插件

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

public class MaintainablePlugin : IPlugin
{
    private readonly ILogger<MaintainablePlugin> _logger;
    private readonly IConfiguration _configuration;
    private PluginConfig _config;

    public MaintainablePlugin(ILogger<MaintainablePlugin> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("初始化可维护插件");
        
        // 加载配置
        _config = new PluginConfig();
        _configuration.GetSection("MaintainablePlugin").Bind(_config);
        
        _logger.LogInformation($"插件配置: 超时={_config.TimeoutMs}ms, 重试={_config.RetryCount}");
        
        await Task.CompletedTask;
    }

    public async Task<object> ExecuteAsync(object context)
    {
        _logger.LogInformation("执行可维护插件");
        
        // 使用配置
        for (int i = 0; i < _config.RetryCount; i++)
        {
            try
            {
                return await ProcessWithContextAsync(context);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"执行失败，重试 {i + 1}/{_config.RetryCount}");
                await Task.Delay(100);
            }
        }
        
        throw new Exception("执行失败，已达到最大重试次数");
    }

    private async Task<object> ProcessWithContextAsync(object context)
    {
        // 业务逻辑
        await Task.Delay(_config.TimeoutMs / 10); // 模拟处理
        return $"可维护插件执行结果: {DateTime.Now}";
    }

    public async Task ShutdownAsync()
    {
        _logger.LogInformation("关闭可维护插件");
        await Task.CompletedTask;
    }

    // 配置类
    private class PluginConfig
    {
        public int TimeoutMs { get; set; } = 5000;
        public int RetryCount { get; set; } = 3;
        public bool EnableLogging { get; set; } = true;
    }
}
```

## 10. 故障排除示例

### 10.1 插件加载失败排查

```csharp
// 创建服务集合
var services = new ServiceCollection();
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});
services.AddPluginService();

using var serviceProvider = services.BuildServiceProvider();
var pluginService = serviceProvider.GetRequiredService<IPluginService>();

try
{
    // 尝试加载插件
    var pluginPath = "path/to/invalid-plugin.dll";
    var pluginInfo = await pluginService.LoadPluginAsync(pluginPath);
    Console.WriteLine("插件加载成功");
}
catch (Exception ex)
{
    Console.WriteLine($"插件加载失败: {ex.Message}");
    Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
    
    // 检查文件是否存在
    var pluginPath = "path/to/invalid-plugin.dll";
    if (!File.Exists(pluginPath))
    {
        Console.WriteLine("错误: 插件文件不存在");
    }
    else
    {
        try
        {
            // 尝试加载程序集
            var assembly = Assembly.LoadFrom(pluginPath);
            Console.WriteLine("程序集加载成功");
            
            // 检查是否包含IPlugin实现
            var pluginTypes = assembly.GetTypes()
                .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);
            
            if (!pluginTypes.Any())
            {
                Console.WriteLine("错误: 插件程序集不包含IPlugin实现");
            }
        }
        catch (Exception loadEx)
        {
            Console.WriteLine($"程序集加载失败: {loadEx.Message}");
        }
    }
}
```

### 10.2 插件执行超时排查

```csharp
// 创建服务集合
var services = new ServiceCollection();
services.AddLogging(builder =>
{
    builder.AddConsole();
});
services.AddPluginService(options =>
{
    options.PluginExecutionTimeoutMs = 2000; // 设置2秒超时
});

using var serviceProvider = services.BuildServiceProvider();
var pluginService = serviceProvider.GetRequiredService<IPluginService>();

// 加载插件
var pluginPath = "path/to/slow-plugin.dll";
await pluginService.LoadPluginAsync(pluginPath);

try
{
    // 执行插件
    var result = await pluginService.ExecutePluginAsync(pluginPath, "测试参数");
    Console.WriteLine($"插件执行成功: {result}");
}
catch (TimeoutException ex)
{
    Console.WriteLine($"插件执行超时: {ex.Message}");
    Console.WriteLine("解决方案:");
    Console.WriteLine("1. 优化插件代码，减少执行时间");
    Console.WriteLine("2. 增加PluginExecutionTimeoutMs配置值");
    Console.WriteLine("3. 检查插件是否有死循环或阻塞操作");
}
catch (Exception ex)
{
    Console.WriteLine($"插件执行异常: {ex.Message}");
}
finally
{
    // 卸载插件
    await pluginService.UnloadPluginAsync(pluginPath);
}
```

## 11. 总结

本示例文档提供了插件系统的各种使用示例，包括：

- 基础插件开发
- AOP插件开发
- 动态代码生成
- 插件服务使用
- 配置和部署
- 测试和最佳实践
- 故障排除

通过这些示例，您可以快速上手插件系统的开发和使用，构建高性能、可维护的插件应用。

## 12. 附录

### 12.1 插件接口定义

```csharp
using System;
using System.Threading.Tasks;

public interface IPlugin
{
    /// <summary>
    /// 初始化插件
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// 执行插件
    /// </summary>
    /// <param name="context">执行上下文</param>
    Task<object> ExecuteAsync(object context);

    /// <summary>
    /// 关闭插件
    /// </summary>
    Task ShutdownAsync();
}
```

### 12.2 插件信息结构

```csharp
public class PluginInfo
{
    /// <summary>
    /// 插件名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 插件版本
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// 插件描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 插件路径
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// 加载时间
    /// </summary>
    public DateTime LoadTime { get; set; }

    /// <summary>
    /// 执行次数
    /// </summary>
    public int ExecutionCount { get; set; }

    /// <summary>
    /// 平均执行时间（毫秒）
    /// </summary>
    public double AverageExecutionTime { get; set; }
}
```
