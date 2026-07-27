# TailwindCSS 技能文档

## 1. 技能概述

TailwindCSS 技能是一个基于 .NET 10 的全功能 TailwindCSS 实现，提供了完整的 CSS 框架功能，支持 AOT 编译和 Scrutor 高级依赖注入。

### 主要特性
- **完整的 TailwindCSS 功能**：包括配置管理、构建系统、监视模式等
- **高级依赖注入**：集成 Scrutor 实现复杂的服务注册和装饰器模式
- **AOT 编译支持**：优化性能，减小应用体积
- **主题管理**：支持多主题和主题继承
- **插件系统**：可扩展的插件架构
- **代码生成**：基于模板的代码生成功能
- **性能优化**：内存缓存和并行处理

### 技术栈
- **.NET 10**：最新的 .NET 框架
- **C# 12**：现代 C# 语言特性
- **Scrutor**：高级依赖注入库
- **System.CommandLine**：CLI 框架
- **Microsoft.Extensions.DependencyInjection**：依赖注入容器
- **Microsoft.Extensions.Logging**：日志系统
- **Microsoft.Extensions.Caching.Memory**：内存缓存
- **System.IO.Abstractions**：文件系统抽象
- **Newtonsoft.Json**：JSON 处理

## 2. 快速开始

### 2.1 环境要求
- .NET 10 SDK 或更高版本
- Windows、Linux 或 macOS 操作系统
- 支持的终端：PowerShell、bash、zsh 等

### 2.2 安装

```bash
# 克隆技能仓库
git clone https://github.com/dotnet-expert/tailwindcss-skill.git

# 进入技能目录
cd tailwindcss-skill

# 编译技能
dotnet build
```

### 2.3 基本使用

#### 初始化 TailwindCSS 配置

```bash
# 初始化配置
dotnet run -- init
```

#### 构建 CSS

```bash
# 构建 CSS 文件
dotnet run -- build input.css output.css
```

#### 监视模式

```bash
# 启动监视模式
dotnet run -- watch input.css output.css
```

#### 清理未使用的 CSS

```bash
# 清理未使用的 CSS
dotnet run -- purge input.css output.css
```

#### 优化 CSS

```bash
# 优化 CSS 文件
dotnet run -- optimize input.css output.css
```

## 3. 核心功能

### 3.1 配置管理

TailwindCSS 技能支持完整的配置管理系统，包括：

- **配置文件**：支持 `tailwind.config.js` 和 `tailwind.config.json`
- **默认配置**：内置合理的默认配置
- **配置覆盖**：支持通过命令行参数覆盖配置
- **配置验证**：自动验证配置的有效性

### 3.2 构建系统

构建系统是 TailwindCSS 技能的核心，提供：

- **CSS 生成**：根据配置生成完整的 TailwindCSS 样式
- **监视模式**：实时监视文件变化并自动重新构建
- **增量构建**：只重新构建变化的部分，提高性能
- **错误处理**：详细的错误信息和建议

### 3.3 主题管理

主题管理系统支持：

- **多主题**：可以定义和切换多个主题
- **主题继承**：主题可以继承其他主题的设置
- **自定义主题**：完全自定义主题颜色、字体、间距等
- **主题预览**：预览主题效果

### 3.4 插件系统

插件系统提供了扩展 TailwindCSS 功能的能力：

- **内置插件**：包含常用的内置插件
- **自定义插件**：支持开发和使用自定义插件
- **插件配置**：精细控制插件的行为
- **插件依赖**：支持插件之间的依赖关系

### 3.5 代码生成

代码生成功能基于模板系统：

- **组件生成**：生成常见的 UI 组件
- **配置生成**：生成项目配置文件
- **样式生成**：生成自定义样式文件
- **模板系统**：基于 Razor 或其他模板引擎

### 3.6 性能优化

性能优化是 TailwindCSS 技能的重要特性：

- **内存缓存**：缓存编译结果，提高重复构建速度
- **并行处理**：使用多线程加速构建过程
- **AOT 编译**：预先编译为本地代码，提高运行时性能
- **代码压缩**：优化输出的 CSS 文件大小

## 4. API 参考

### 4.1 核心接口

#### ITailwindService

```csharp
public interface ITailwindService
{
    // 构建 TailwindCSS
    Task<string> BuildAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default);
    
    // 监视文件变化
    Task WatchAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default);
    
    // 初始化配置
    Task InitializeAsync(string configPath = null, CancellationToken cancellationToken = default);
    
    // 清理未使用的 CSS
    Task<string> PurgeAsync(string inputPath, string outputPath, IEnumerable<string> contentPaths, CancellationToken cancellationToken = default);
    
    // 优化 CSS
    Task<string> OptimizeAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default);
}
```

#### ITailwindThemeService

```csharp
public interface ITailwindThemeService
{
    // 获取所有主题
    IEnumerable<string> GetThemes();
    
    // 获取主题配置
    ThemeConfiguration GetTheme(string name);
    
    // 添加主题
    void AddTheme(string name, ThemeConfiguration configuration);
    
    // 切换主题
    void SetActiveTheme(string name);
    
    // 获取当前主题
    ThemeConfiguration GetActiveTheme();
}
```

#### ITailwindPluginService

```csharp
public interface ITailwindPluginService
{
    // 加载所有插件
    Task LoadPluginsAsync(CancellationToken cancellationToken = default);
    
    // 获取所有插件
    IEnumerable<IPlugin> GetPlugins();
    
    // 添加插件
    void AddPlugin(IPlugin plugin);
    
    // 移除插件
    void RemovePlugin(string name);
    
    // 执行插件
    Task ExecutePluginsAsync(string input, CancellationToken cancellationToken = default);
}
```

### 4.2 命令行接口

#### 构建命令

```bash
# 构建 TailwindCSS
dotnet run -- build [options] <input> <output>

# 选项
--config <path>        指定配置文件路径
--minify               最小化输出
--watch                监视文件变化
--purge                清理未使用的 CSS
--content <paths>      指定内容文件路径，用于清理
```

#### 监视命令

```bash
# 监视文件变化
dotnet run -- watch [options] <input> <output>

# 选项
--config <path>        指定配置文件路径
--minify               最小化输出
--content <paths>      指定内容文件路径，用于清理
```

#### 初始化命令

```bash
# 初始化 TailwindCSS 配置
dotnet run -- init [options]

# 选项
--output <path>        指定输出配置文件路径
--theme <name>         指定默认主题
```

#### 清理命令

```bash
# 清理未使用的 CSS
dotnet run -- purge [options] <input> <output>

# 选项
--config <path>        指定配置文件路径
--content <paths>      指定内容文件路径，用于清理
--minify               最小化输出
```

#### 优化命令

```bash
# 优化 CSS 文件
dotnet run -- optimize [options] <input> <output>

# 选项
--minify               最小化输出
--source-map           生成源映射
```

## 5. AOT 编译

### 5.1 AOT 编译配置

TailwindCSS 技能支持 AOT 编译，通过以下配置实现：

```yaml
# AOT 编译配置
aot:
  enabled: true
  publish_aot: true
  trim_mode: partial
  self_contained: true
  publish_single_file: true
  target_framework: net11.0
```

### 5.2 编译为 AOT

```bash
# 发布为 AOT 编译的单文件应用
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishAot=true -p:TrimMode=partial -p:PublishSingleFile=true
```

### 5.3 AOT 编译的优势

- **启动速度快**：AOT 编译的应用启动速度比 JIT 编译快得多
- **运行时性能好**：避免了运行时编译的开销
- **内存使用少**：不需要加载和编译 IL 代码
- **部署简单**：单文件部署，不需要安装 .NET 运行时
- **安全性高**：难以反编译，保护知识产权

### 5.4 注意事项

- **编译时间长**：AOT 编译需要更长的时间
- **调试难度大**：AOT 编译的代码调试难度增加
- **反射限制**：某些反射功能可能受到限制
- **平台特定**：需要为每个目标平台单独编译

## 6. 高级用法

### 6.1 自定义主题

创建自定义主题配置：

```js
// tailwind.config.js
module.exports = {
  theme: {
    extend: {
      colors: {
        primary: '#3b82f6',
        secondary: '#10b981',
        accent: '#8b5cf6',
        neutral: '#6b7280',
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
        mono: ['Fira Code', 'monospace'],
      },
      spacing: {
        '128': '32rem',
        '144': '36rem',
      },
    },
  },
};
```

### 6.2 自定义插件

创建自定义插件：

```csharp
public class MyCustomPlugin : IPlugin
{
    public string Name => "MyCustomPlugin";
    
    public Task ExecuteAsync(string input, CancellationToken cancellationToken = default)
    {
        // 插件逻辑
        Console.WriteLine("Executing custom plugin...");
        return Task.CompletedTask;
    }
    
    public void Configure(PluginConfiguration configuration)
    {
        // 插件配置
    }
}
```

注册自定义插件：

```csharp
services.AddSingleton<IPlugin, MyCustomPlugin>();
```

### 6.3 高级依赖注入

使用 Scrutor 实现高级依赖注入：

```csharp
// 注册所有实现了特定接口的服务
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);

// 使用装饰器模式
services.Decorate<ITailwindService, TailwindServiceLoggingDecorator>();
services.Decorate<ITailwindService, TailwindServiceCachingDecorator>();
```

### 6.4 性能优化

#### 内存缓存

```csharp
// 配置内存缓存
services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024 * 1024 * 100; // 100MB
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
});

// 使用内存缓存
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
    
    // 其他方法...
}
```

#### 并行处理

```csharp
// 并行处理多个文件
public async Task BuildMultipleAsync(IEnumerable<(string Input, string Output)> files, CancellationToken cancellationToken = default)
{
    var tasks = files.Select(file => BuildAsync(file.Input, file.Output, cancellationToken));
    await Task.WhenAll(tasks);
}
```

## 7. 示例

### 7.1 基本示例

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
dotnet run -- build input.css output.css
```

#### 输出 CSS 文件

```css
/* 生成的 CSS 文件 */
/* 包含所有 TailwindCSS 类 */
```

### 7.2 高级示例

#### 自定义主题示例

```js
// tailwind.config.js
module.exports = {
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#eff6ff',
          100: '#dbeafe',
          200: '#bfdbfe',
          300: '#93c5fd',
          400: '#60a5fa',
          500: '#3b82f6',
          600: '#2563eb',
          700: '#1d4ed8',
          800: '#1e40af',
          900: '#1e3a8a',
        },
      },
    },
  },
};
```

#### 插件使用示例

```csharp
// 注册和使用插件
var pluginService = serviceProvider.GetRequiredService<ITailwindPluginService>();
await pluginService.LoadPluginsAsync();

// 执行构建
var tailwindService = serviceProvider.GetRequiredService<ITailwindService>();
await tailwindService.BuildAsync("input.css", "output.css");
```

### 7.3 Scrutor 使用示例

#### 基本扫描注册

```csharp
// 基本扫描注册
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses()
    .AsSelf()
    .WithTransientLifetime()
);
```

#### 按约定注册

```csharp
// 按约定注册服务
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithTransientLifetime()
);
```

#### 装饰器模式

```csharp
// 装饰器模式
services.AddTransient<ITailwindService, TailwindService>();
services.Decorate<ITailwindService, TailwindServiceLoggingDecorator>();
services.Decorate<ITailwindService, TailwindServiceCachingDecorator>();
```

#### 开放泛型

```csharp
// 注册开放泛型服务
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(type => type.IsGenericTypeDefinition))
    .AsImplementedInterfaces()
    .WithTransientLifetime()
);
```

## 8. 故障排除

### 8.1 常见问题

#### 问题：构建失败，提示找不到配置文件

**解决方案**：
- 确保在项目根目录运行命令
- 执行 `dotnet run -- init` 初始化配置文件
- 检查配置文件路径是否正确

#### 问题：监视模式不工作

**解决方案**：
- 确保文件系统权限正确
- 检查输入文件路径是否存在
- 确保终端没有被阻塞

#### 问题：AOT 编译失败

**解决方案**：
- 确保使用 .NET 10 SDK
- 检查项目是否有不兼容的反射使用
- 尝试使用 `TrimMode=partial` 而不是 `TrimMode=full`

#### 问题：CSS 输出文件过大

**解决方案**：
- 使用 `purge` 命令清理未使用的 CSS
- 配置 `content` 路径以确保正确检测使用的类
- 考虑使用 `optimize` 命令进一步优化

### 8.2 错误代码

| 错误代码 | 描述 | 解决方案 |
|---------|------|----------|
| TW001 | 配置文件不存在 | 执行 `init` 命令初始化配置 |
| TW002 | 输入文件不存在 | 检查输入文件路径是否正确 |
| TW003 | 输出目录不存在 | 创建输出目录或检查路径权限 |
| TW004 | 配置文件格式错误 | 检查配置文件语法是否正确 |
| TW005 | 插件加载失败 | 检查插件依赖和配置 |
| TW006 | 内存不足 | 增加系统内存或减少缓存大小 |
| TW007 | 编译超时 | 增加超时时间或减少处理的文件大小 |
| TW008 | AOT 编译不支持 | 检查 .NET 版本和项目配置 |

### 8.3 调试技巧

#### 启用详细日志

```bash
# 启用详细日志
dotnet run --verbosity detailed -- build input.css output.css
```

#### 检查缓存状态

```bash
# 清除缓存并重新构建
dotnet run -- clear-cache && dotnet run -- build input.css output.css
```

#### 分析性能

```bash
# 启用性能分析
dotnet run --profile -- build input.css output.css
```

## 9. 贡献指南

### 9.1 开发环境设置

1. **克隆仓库**

```bash
git clone https://github.com/dotnet-expert/tailwindcss-skill.git
cd tailwindcss-skill
```

2. **安装依赖**

```bash
dotnet restore
```

3. **构建项目**

```bash
dotnet build
```

4. **运行测试**

```bash
dotnet test
```

### 9.2 代码规范

- 遵循 .NET 设计规范
- 使用 C# 12 语言特性
- 保持代码风格一致
- 编写详细的文档和注释
- 为新功能添加测试

### 9.3 提交 PR

1. **创建分支**

```bash
git checkout -b feature/your-feature-name
```

2. **提交更改**

```bash
git add .
git commit -m "Add your feature description"
```

3. **推送到远程**

```bash
git push origin feature/your-feature-name
```

4. **创建 PR**

在 GitHub 上创建 Pull Request，描述你的更改和动机。

## 10. 许可证

TailwindCSS 技能使用 MIT 许可证：

```
MIT License

Copyright (c) 2026 NET 专家

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

## 11. 联系我们

- **GitHub**：[https://github.com/dotnet-expert/tailwindcss-skill](https://github.com/dotnet-expert/tailwindcss-skill)
- **Discord**：[https://discord.gg/dotnet](https://discord.gg/dotnet)
- **Email**：expert@dotnet.com

---

**TailwindCSS 技能** - 为 .NET 开发者提供强大的 CSS 工具链
