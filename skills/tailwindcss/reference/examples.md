# TailwindCSS 技能使用示例

## 1. 核心 TailwindCSS 功能示例

### 1.1 基本构建命令

#### 输入 CSS 文件

```css
/* input.css */
@tailwind base;
@tailwind components;
@tailwind utilities;

@layer components {
    .btn {
        @apply px-4 py-2 rounded font-medium;
    }
    
    .btn-primary {
        @apply bg-blue-500 text-white hover:bg-blue-600;
    }
    
    .btn-secondary {
        @apply bg-gray-500 text-white hover:bg-gray-600;
    }
}
```

#### 构建命令

```bash
# 构建 CSS 文件
dotnet run -- build input.css output.css
```

#### 输出结果

```
Building TailwindCSS from input.css to output.css
Build completed successfully
```

### 1.2 监视模式

#### 监视命令

```bash
# 启动监视模式
dotnet run -- watch input.css output.css
```

#### 输出结果

```
Starting watch mode for input.css to output.css
Building TailwindCSS from input.css to output.css
Build completed successfully
Watch mode started. Press Ctrl+C to exit.
```

### 1.3 清理未使用的 CSS

#### 清理命令

```bash
# 清理未使用的 CSS
dotnet run -- purge input.css output.css --content **/*.html,**/*.cshtml,**/*.razor,**/*.js
```

#### 输出结果

```
Purging unused CSS from input.css to output.css
Building TailwindCSS from input.css to output.css
CSS purged successfully
```

### 1.4 优化 CSS

#### 优化命令

```bash
# 优化 CSS 文件
dotnet run -- optimize input.css output.css
```

#### 输出结果

```
Optimizing CSS from input.css to output.css
Building TailwindCSS from input.css to output.css
CSS optimized successfully
```

### 1.5 初始化配置

#### 初始化命令

```bash
# 初始化配置
dotnet run -- init
```

#### 输出结果

```
Initializing TailwindCSS configuration
Configuration initialized at tailwind.config.json
```

## 2. Scrutor 用法示例

### 2.1 基本扫描注册

```csharp
// 基本扫描注册
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses()
    .AsSelf()
    .WithTransientLifetime()
);
```

### 2.2 按约定注册

```csharp
// 按约定注册服务
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Provider")))
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
);
```

### 2.3 按命名空间注册

```csharp
// 按命名空间注册
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.InNamespaces("TailwindCSS.Services", "TailwindCSS.Repositories"))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);
```

### 2.4 按属性注册

```csharp
// 定义注册属性
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class RegisterServiceAttribute : Attribute
{}

// 标记服务类
[RegisterService]
public class ExampleService
{}

// 按属性注册
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.WithAttribute<RegisterServiceAttribute>())
    .AsImplementedInterfaces()
    .WithTransientLifetime()
);
```

### 2.5 装饰器模式

```csharp
// 添加基础服务
services.AddTransient<ITailwindService, TailwindService>();

// 添加装饰器
services.Decorate<ITailwindService, TailwindServiceLoggingDecorator>();
services.Decorate<ITailwindService, TailwindServiceCachingDecorator>();
```

### 2.6 多重装饰器

```csharp
// 添加基础服务
services.AddTransient<ITemplateService, TemplateService>();

// 添加多个装饰器
services.Decorate<ITemplateService, TemplateServiceLoggingDecorator>();
services.Decorate<ITemplateService, TemplateServiceCachingDecorator>();
services.Decorate<ITemplateService, TemplateServiceValidationDecorator>();
```

### 2.7 开放泛型

```csharp
// 注册开放泛型服务
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(type => type.IsGenericTypeDefinition))
    .AsImplementedInterfaces()
    .WithTransientLifetime()
);
```

### 2.8 自定义注册规则

```csharp
// 自定义注册规则
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(type => 
        type.GetInterfaces().Any(i => i.Name.StartsWith("I")) &&
        !type.IsAbstract &&
        !type.IsInterface
    ))
    .As(t => t.GetInterfaces().Where(i => i.Name == "I" + t.Name))
    .WithScopedLifetime()
);
```

### 2.9 组合多种注册方式

```csharp
// 组合多种注册方式
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    // 注册服务
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
    // 注册仓库
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
    // 注册单例
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Singleton")))
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
);
```

### 2.10 装饰器链

```csharp
// 装饰器链示例
services.AddTransient<IScrutorDemoService, ScrutorDemoService>();
services.Decorate<IScrutorDemoService, ScrutorDemoServiceLoggingDecorator>();
```

## 3. 高级 DI 使用示例

### 3.1 依赖注入容器配置

```csharp
// 配置依赖注入容器
var services = new ServiceCollection();

// 添加日志
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

// 添加缓存
services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024 * 1024 * 100; // 100MB
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
});

// 添加文件系统
services.AddSingleton<IFileSystem, FileSystem>();

// 添加主题服务
services.AddSingleton<ITailwindThemeService, TailwindThemeService>();

// 添加插件服务
services.AddSingleton<ITailwindPluginService, TailwindPluginService>();

// 添加构建步骤
services.AddTransient<IBuildStep, ParseDirectivesStep>();
services.AddTransient<IBuildStep, ProcessLayersStep>();
services.AddTransient<IBuildStep, ApplyDirectivesStep>();

// 添加核心服务
services.AddTransient<ITailwindService, TailwindService>();

// 添加装饰器
services.Decorate<ITailwindService, TailwindServiceLoggingDecorator>();
services.Decorate<ITailwindService, TailwindServiceCachingDecorator>();

// 构建服务提供器
var serviceProvider = services.BuildServiceProvider();
```

### 3.2 装饰器模式应用

#### 日志装饰器

```csharp
public class TailwindServiceLoggingDecorator : ITailwindService
{
    private readonly ITailwindService _inner;
    private readonly ILogger<TailwindServiceLoggingDecorator> _logger;

    public TailwindServiceLoggingDecorator(ITailwindService inner, ILogger<TailwindServiceLoggingDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<string> BuildAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[Decorator] Building TailwindCSS");
        var result = await _inner.BuildAsync(inputPath, outputPath, cancellationToken);
        _logger.LogInformation("[Decorator] Build completed");
        return result;
    }

    // 其他方法实现...
}
```

#### 缓存装饰器

```csharp
public class TailwindServiceCachingDecorator : ITailwindService
{
    private readonly ITailwindService _inner;
    private readonly IMemoryCache _cache;

    public TailwindServiceCachingDecorator(ITailwindService inner, IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<string> BuildAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"build:{inputPath}:{outputPath}";
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(10);
            return await _inner.BuildAsync(inputPath, outputPath, cancellationToken);
        });
    }

    // 其他方法实现...
}
```

#### 验证装饰器

```csharp
public class TemplateServiceValidationDecorator : ITemplateService
{
    private readonly ITemplateService _inner;
    private readonly ILogger<TemplateServiceValidationDecorator> _logger;

    public TemplateServiceValidationDecorator(ITemplateService inner, ILogger<TemplateServiceValidationDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public Task<string> GetTemplateAsync(string templateName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(templateName))
        {
            throw new ArgumentNullException(nameof(templateName));
        }
        return _inner.GetTemplateAsync(templateName, cancellationToken);
    }

    // 其他方法实现...
}
```

## 4. 主题管理示例

### 4.1 添加自定义主题

```csharp
// 添加自定义主题
var themeService = serviceProvider.GetRequiredService<ITailwindThemeService>();

var customTheme = new ThemeConfiguration
{
    Colors = new Dictionary<string, Dictionary<string, string>>
    {
        { "primary", new Dictionary<string, string>
            {
                { "50", "#f0fdf4" },
                { "100", "#dcfce7" },
                { "200", "#bbf7d0" },
                { "300", "#86efac" },
                { "400", "#4ade80" },
                { "500", "#22c55e" },
                { "600", "#16a34a" },
                { "700", "#15803d" },
                { "800", "#166534" },
                { "900", "#14532d" }
            }
        }
    },
    FontFamily = new Dictionary<string, List<string>>
    {
        { "sans", new List<string> { "Inter", "system-ui", "sans-serif" } },
        { "serif", new List<string> { "Georgia", "Cambria", "serif" } },
        { "mono", new List<string> { "Fira Code", "Consolas", "monospace" } }
    }
};

themeService.AddTheme("custom", customTheme);
themeService.SetActiveTheme("custom");

var activeTheme = themeService.GetActiveTheme();
Console.WriteLine("Active theme: custom");
```

### 4.2 主题切换

```csharp
// 主题切换示例
var themeService = serviceProvider.GetRequiredService<ITailwindThemeService>();

// 列出所有主题
var themes = themeService.GetThemes();
Console.WriteLine("Available themes:");
foreach (var theme in themes)
{
    Console.WriteLine($"- {theme}");
}

// 切换主题
themeService.SetActiveTheme("default");
var activeTheme = themeService.GetActiveTheme();
Console.WriteLine("Switched to default theme");
```

## 5. 插件系统示例

### 5.1 创建自定义插件

```csharp
// 创建自定义插件
public class MyCustomPlugin : IPlugin
{
    public string Name => "MyCustomPlugin";
    
    public Task ExecuteAsync(string input, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Executing MyCustomPlugin...");
        // 插件逻辑：添加自定义样式
        return Task.CompletedTask;
    }
    
    public void Configure(PluginConfiguration configuration)
    {
        Console.WriteLine("Configuring MyCustomPlugin...");
        // 插件配置
    }
}
```

### 5.2 注册和使用插件

```csharp
// 注册和使用插件
var pluginService = serviceProvider.GetRequiredService<ITailwindPluginService>();

// 添加自定义插件
pluginService.AddPlugin(new MyCustomPlugin());

// 加载所有插件
await pluginService.LoadPluginsAsync();

// 获取所有插件
var plugins = pluginService.GetPlugins();
Console.WriteLine("Loaded plugins:");
foreach (var plugin in plugins)
{
    Console.WriteLine($"- {plugin.Name}");
}

// 执行插件
await pluginService.ExecutePluginsAsync("input content");
```

## 6. 构建步骤扩展示例

### 6.1 创建自定义构建步骤

```csharp
// 创建自定义构建步骤
public class CustomBuildStep : IBuildStep
{
    public string Name => "CustomBuildStep";
    
    public Task<string> ExecuteAsync(string input, BuildContext context, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Executing CustomBuildStep...");
        // 自定义构建逻辑：处理输入内容
        var result = input + "\n/* Custom build step added this comment */";
        return Task.FromResult(result);
    }
}
```

### 6.2 注册构建步骤

```csharp
// 注册自定义构建步骤
services.AddTransient<IBuildStep, CustomBuildStep>();

// 构建服务提供器
var serviceProvider = services.BuildServiceProvider();

// 获取所有构建步骤
var buildSteps = serviceProvider.GetRequiredService<IEnumerable<IBuildStep>>();
Console.WriteLine("Registered build steps:");
foreach (var step in buildSteps)
{
    Console.WriteLine($"- {step.Name}");
}
```

## 7. 模板系统示例

### 7.1 渲染模板

```csharp
// 渲染模板示例
var templateService = serviceProvider.GetRequiredService<ITemplateService>();

// 准备参数
var parameters = new Dictionary<string, object>
{
    { "ComponentName", "Button" },
    { "ComponentNameLower", "button" },
    { "ComponentNameCamel", "button" }
};

// 渲染模板
var result = await templateService.RenderTemplateAsync("component_button", parameters);
Console.WriteLine("Rendered template:");
Console.WriteLine(result);
```

### 7.2 注册自定义模板

```csharp
// 注册自定义模板
var templateService = serviceProvider.GetRequiredService<ITemplateService>();

// 注册自定义模板
templateService.RegisterTemplate("custom_card", @"/* Custom Card Component */
@layer components {
  .card-custom {
    @apply bg-white rounded-lg shadow-md p-6 border border-gray-200;
  }
  
  .card-custom-header {
    @apply text-xl font-bold mb-4;
  }
  
  .card-custom-body {
    @apply text-gray-600;
  }
}");

// 获取可用模板
var templates = templateService.GetAvailableTemplates();
Console.WriteLine("Available templates:");
foreach (var template in templates)
{
    Console.WriteLine($"- {template}");
}
```

## 8. 代码生成示例

### 8.1 生成组件

```bash
# 生成组件
dotnet run --project tailwind_generator.csproj component Button --template component_button --output components/Button.css
```

#### 输出结果

```
Generating component Button using template component_button
Component Button generated successfully
Component generated to: components/Button.css
```

### 8.2 生成配置

```bash
# 生成配置
dotnet run --project tailwind_generator.csproj config tailwind --output tailwind.config.json
```

#### 输出结果

```
Generating configuration of type tailwind
Configuration tailwind generated successfully
```

### 8.3 生成样式

```bash
# 生成样式
dotnet run --project tailwind_generator.csproj style utilities --output styles/utilities.css
```

#### 输出结果

```
Generating style of type utilities
Style utilities generated successfully
```

### 8.4 生成插件

```bash
# 生成插件
dotnet run --project tailwind_generator.csproj plugin CustomPlugin --output plugins/CustomPlugin.cs
```

#### 输出结果

```
Generating plugin CustomPlugin
Plugin CustomPlugin generated successfully
Plugin generated to: plugins/CustomPlugin.cs
```

## 9. 性能优化示例

### 9.1 内存缓存配置

```csharp
// 优化内存缓存配置
services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024 * 1024 * 100; // 100MB
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
    options.CompactionPercentage = 0.1; // 10%
});

// 使用缓存装饰器
services.AddTransient<ITailwindService, TailwindService>();
services.Decorate<ITailwindService, TailwindServiceCachingDecorator>();
```

### 9.2 并行处理

```csharp
// 并行处理多个文件
public async Task BuildMultipleAsync(IEnumerable<(string Input, string Output)> files, CancellationToken cancellationToken = default)
{
    var tailwindService = serviceProvider.GetRequiredService<ITailwindService>();
    
    var tasks = files.Select(file => tailwindService.BuildAsync(file.Input, file.Output, cancellationToken));
    await Task.WhenAll(tasks);
    
    Console.WriteLine($"Built {files.Count()} files in parallel");
}

// 使用示例
var files = new List<(string, string)>
{
    ("input1.css", "output1.css"),
    ("input2.css", "output2.css"),
    ("input3.css", "output3.css")
};

await BuildMultipleAsync(files);
```

### 9.3 AOT 编译优化

```bash
# 优化 AOT 编译
dotnet publish -c Release -r win-x64 \
    --self-contained true \
    -p:PublishAot=true \
    -p:TrimMode=partial \
    -p:PublishSingleFile=true \
    -p:EnableCompressionInSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true
```

## 10. 故障排除示例

### 10.1 启用详细日志

```bash
# 启用详细日志
dotnet run --verbosity detailed -- build input.css output.css
```

#### 输出结果

```
[11:30:00 INF] Building TailwindCSS from input.css to output.css
[11:30:00 DBG] Loading configuration from tailwind.config.json
[11:30:00 DBG] Executing build step: ParseDirectives
[11:30:00 DBG] Executing build step: ProcessLayers
[11:30:00 DBG] Executing build step: ApplyDirectives
[11:30:00 DBG] Executing plugin: DefaultPlugin
[11:30:00 INF] Build completed successfully
```

### 10.2 检查缓存状态

```bash
# 清除缓存并重新构建
dotnet run -- clear-cache && dotnet run -- build input.css output.css
```

#### 输出结果

```
Cache cleared successfully
Building TailwindCSS from input.css to output.css
Build completed successfully
```

### 10.3 分析性能

```bash
# 启用性能分析
dotnet run --profile -- build input.css output.css
```

#### 输出结果

```
Profiling enabled
Building TailwindCSS from input.css to output.css
[Profile] Build time: 120ms
[Profile] Cache hits: 0
[Profile] Cache misses: 3
[Profile] Plugin execution time: 10ms
Build completed successfully
```

## 11. 完整集成示例

### 11.1 基本集成

```csharp
// 基本集成示例
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TailwindCSS;

class Program
{
    static async Task Main(string[] args)
    {
        // 配置依赖注入
        var services = new ServiceCollection();
        // 添加服务...
        var serviceProvider = services.BuildServiceProvider();

        // 获取 Tailwind 服务
        var tailwindService = serviceProvider.GetRequiredService<ITailwindService>();

        // 构建 CSS
        await tailwindService.BuildAsync("input.css", "output.css");

        Console.WriteLine("TailwindCSS build completed!");
    }
}
```

### 11.2 高级集成

```csharp
// 高级集成示例
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TailwindCSS;
using TailwindCSS.Generator;

class Program
{
    static async Task Main(string[] args)
    {
        // 配置依赖注入
        var services = new ServiceCollection();

        // 添加日志
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // 添加缓存
        services.AddMemoryCache();

        // 添加文件系统
        services.AddSingleton<IFileSystem, FileSystem>();

        // 添加主题服务
        services.AddSingleton<ITailwindThemeService, TailwindThemeService>();

        // 添加插件服务
        services.AddSingleton<ITailwindPluginService, TailwindPluginService>();

        // 添加构建步骤
        services.AddTransient<IBuildStep, ParseDirectivesStep>();
        services.AddTransient<IBuildStep, ProcessLayersStep>();
        services.AddTransient<IBuildStep, ApplyDirectivesStep>();

        // 添加核心服务
        services.AddTransient<ITailwindService, TailwindService>();

        // 添加装饰器
        services.Decorate<ITailwindService, TailwindServiceLoggingDecorator>();
        services.Decorate<ITailwindService, TailwindServiceCachingDecorator>();

        // 添加模板服务
        services.AddTransient<ITemplateService, TemplateService>();

        // 添加代码生成服务
        services.AddTransient<ICodeGeneratorService, CodeGeneratorService>();

        // 添加 Scrutor 演示服务
        services.AddTransient<IScrutorDemoService, ScrutorDemoService>();

        // 构建服务提供器
        var serviceProvider = services.BuildServiceProvider();

        // 使用 Tailwind 服务
        var tailwindService = serviceProvider.GetRequiredService<ITailwindService>();
        await tailwindService.BuildAsync("input.css", "output.css");

        // 使用代码生成服务
        var generatorService = serviceProvider.GetRequiredService<ICodeGeneratorService>();
        var component = await generatorService.GenerateComponentAsync("Button", "component_button", new());
        Console.WriteLine("Generated component:");
        Console.WriteLine(component);

        // 演示 Scrutor 用法
        var scrutorService = serviceProvider.GetRequiredService<IScrutorDemoService>();
        await scrutorService.DemonstrateScrutorUsageAsync();

        Console.WriteLine("All operations completed successfully!");
    }
}
```

## 12. 最佳实践

### 12.1 项目结构

```
MyProject/
├── tailwind.config.json        # TailwindCSS 配置
├── input.css                   # 输入 CSS 文件
├── output.css                  # 输出 CSS 文件
├── components/                 # 组件目录
│   ├── Button.css              # 生成的按钮组件
│   └── Card.css                # 生成的卡片组件
├── plugins/                    # 插件目录
│   └── CustomPlugin.cs         # 自定义插件
└── scripts/                    # 脚本目录
    └── build.ps1               # 构建脚本
```

### 12.2 构建脚本

```powershell
# build.ps1

# 构建 TailwindCSS
Write-Host "Building TailwindCSS..."
dotnet run --project tailwindcss/scripts/tailwind_core.cs build input.css output.css

# 清理未使用的 CSS
Write-Host "Purging unused CSS..."
dotnet run --project tailwindcss/scripts/tailwind_core.cs purge output.css output.min.css --content **/*.html,**/*.cshtml,**/*.razor,**/*.js

# 优化 CSS
Write-Host "Optimizing CSS..."
dotnet run --project tailwindcss/scripts/tailwind_core.cs optimize output.min.css output.min.css

Write-Host "Build completed successfully!"
```

### 12.3 开发工作流

1. **初始化项目**
   ```bash
   dotnet run --project tailwindcss/scripts/tailwind_core.cs init
   ```

2. **创建输入文件**
   ```css
   /* input.css */
   @tailwind base;
   @tailwind components;
   @tailwind utilities;
   ```

3. **启动监视模式**
   ```bash
   dotnet run --project tailwindcss/scripts/tailwind_core.cs watch input.css output.css
   ```

4. **开发组件**
   ```bash
   dotnet run --project tailwindcss/scripts/tailwind_generator.cs component Button --template component_button --output components/Button.css
   ```

5. **构建生产版本**
   ```bash
   dotnet run --project tailwindcss/scripts/tailwind_core.cs build input.css output.css
   dotnet run --project tailwindcss/scripts/tailwind_core.cs purge output.css output.min.css --content **/*.html
   dotnet run --project tailwindcss/scripts/tailwind_core.cs optimize output.min.css output.min.css
   ```

---

**TailwindCSS 技能** - 为 .NET 开发者提供强大的 CSS 工具链
