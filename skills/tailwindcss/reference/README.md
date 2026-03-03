# TailwindCSS 技能参考文档

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

## 2. 目录结构

TailwindCSS 技能采用清晰的目录结构，便于维护和扩展：

```
tailwindcss/
├── index.yaml            # 技能元数据文件
├── SKILL.md              # 技能文档
├── scripts/             # 执行脚本
│   ├── tailwind_core.cs              # 核心 TailwindCSS 功能
│   ├── tailwind_core.setting.json    # 核心文件编译配置
│   ├── tailwind_core.run.json        # 核心文件运行配置
│   ├── tailwind_generator.cs         # 代码生成功能
│   ├── tailwind_generator.setting.json    # 生成器编译配置
│   └── tailwind_generator.run.json        # 生成器运行配置
└── reference/           # 参考文档
    ├── README.md         # 参考文档
    └── examples.md       # 使用示例
```

### 目录说明

- **tailwindcss/**：技能根目录
  - **index.yaml**：技能元数据文件，包含 AOT 编译选项、依赖、功能特性、CLI 命令和文件路径等配置
  - **SKILL.md**：技能文档，包含概述、快速开始、核心功能、API 参考等详细内容
  - **scripts/**：执行脚本目录，包含所有 .NET 10 单文件执行脚本
    - **tailwind_core.cs**：核心 TailwindCSS 功能实现，包含配置管理、构建系统、监视模式等
    - **tailwind_core.setting.json**：核心文件编译配置，包含目标框架、依赖项等
    - **tailwind_core.run.json**：核心文件运行配置，包含各种命令的参数和环境变量
    - **tailwind_generator.cs**：代码生成功能实现，包含 Scrutor 用法示例
    - **tailwind_generator.setting.json**：生成器编译配置
    - **tailwind_generator.run.json**：生成器运行配置
  - **reference/**：参考文档目录
    - **README.md**：参考文档，包含技能概述、目录结构、核心功能等
    - **examples.md**：使用示例，包含详细的用法示例和 Scrutor 各种用法 demo

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
- **插件生成**：生成自定义插件

### 3.6 性能优化

性能优化是 TailwindCSS 技能的重要特性：

- **内存缓存**：缓存编译结果，提高重复构建速度
- **并行处理**：使用多线程加速构建过程
- **AOT 编译**：预先编译为本地代码，提高运行时性能
- **代码压缩**：优化输出的 CSS 文件大小

## 4. 技术栈

### 4.1 核心技术

| 技术 | 版本 | 用途 |
|------|------|------|
| .NET | 10.0 | 核心运行时 |
| C# | 12.0 | 编程语言 |
| Scrutor | 4.2.0 | 高级依赖注入 |
| System.CommandLine | 2.0.0 | CLI 框架 |
| Microsoft.Extensions.DependencyInjection | 8.0.0 | 依赖注入容器 |
| Microsoft.Extensions.Logging | 8.0.0 | 日志系统 |
| Microsoft.Extensions.Caching.Memory | 8.0.0 | 内存缓存 |
| System.IO.Abstractions | 17.0.21 | 文件系统抽象 |
| Newtonsoft.Json | 13.0.3 | JSON 处理 |

### 4.2 编译配置

TailwindCSS 技能使用 AOT 编译技术，配置如下：

- **PublishAot**：true - 启用 AOT 编译
- **TrimMode**：partial - 部分修剪未使用的代码
- **SelfContained**：true - 自包含部署
- **PublishSingleFile**：true - 发布为单文件应用
- **TargetFramework**：net10.0 - 目标框架

### 4.3 依赖项

TailwindCSS 技能依赖以下 NuGet 包：

- **Scrutor**：用于高级依赖注入和装饰器模式
- **System.CommandLine**：用于构建 CLI 命令
- **Microsoft.Extensions.DependencyInjection**：依赖注入容器
- **Microsoft.Extensions.Logging**：日志系统
- **Microsoft.Extensions.Caching.Memory**：内存缓存
- **System.IO.Abstractions**：文件系统抽象
- **Newtonsoft.Json**：JSON 处理

## 5. CLI 命令

### 5.1 核心命令

| 命令 | 描述 | 参数 | 示例 |
|------|------|------|------|
| build | 构建 TailwindCSS | input: 输入文件路径<br>output: 输出文件路径 | `build input.css output.css` |
| watch | 监视文件变化并构建 | input: 输入文件路径<br>output: 输出文件路径 | `watch input.css output.css` |
| init | 初始化 TailwindCSS 配置 | --output: 输出配置文件路径 | `init --output tailwind.config.json` |
| purge | 清理未使用的 CSS | input: 输入文件路径<br>output: 输出文件路径<br>--content: 内容文件路径 | `purge input.css output.css --content **/*.html` |
| optimize | 优化 CSS 文件 | input: 输入文件路径<br>output: 输出文件路径 | `optimize input.css output.css` |

### 5.2 生成器命令

| 命令 | 描述 | 参数 | 示例 |
|------|------|------|------|
| component | 生成组件 | name: 组件名称<br>--template: 模板名称<br>--output: 输出文件路径 | `component Button --template component_button` |
| config | 生成配置 | type: 配置类型<br>--output: 输出文件路径 | `config tailwind` |
| style | 生成样式 | type: 样式类型<br>--output: 输出文件路径 | `style utilities` |
| plugin | 生成插件 | name: 插件名称<br>--output: 输出文件路径 | `plugin CustomPlugin` |
| scrutor | 演示 Scrutor 用法 | 无 | `scrutor` |
| templates | 列出可用模板 | 无 | `templates` |

## 6. API 参考

### 6.1 核心接口

#### ITailwindService

```csharp
public interface ITailwindService
{
    Task<string> BuildAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default);
    Task WatchAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default);
    Task InitializeAsync(string configPath = null, CancellationToken cancellationToken = default);
    Task<string> PurgeAsync(string inputPath, string outputPath, IEnumerable<string> contentPaths, CancellationToken cancellationToken = default);
    Task<string> OptimizeAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default);
}
```

#### ITailwindThemeService

```csharp
public interface ITailwindThemeService
{
    IEnumerable<string> GetThemes();
    ThemeConfiguration GetTheme(string name);
    void AddTheme(string name, ThemeConfiguration configuration);
    void SetActiveTheme(string name);
    ThemeConfiguration GetActiveTheme();
}
```

#### ITailwindPluginService

```csharp
public interface ITailwindPluginService
{
    Task LoadPluginsAsync(CancellationToken cancellationToken = default);
    IEnumerable<IPlugin> GetPlugins();
    void AddPlugin(IPlugin plugin);
    void RemovePlugin(string name);
    Task ExecutePluginsAsync(string input, CancellationToken cancellationToken = default);
}
```

#### ICodeGeneratorService

```csharp
public interface ICodeGeneratorService
{
    Task<string> GenerateComponentAsync(string componentName, string templateName, Dictionary<string, object> parameters, CancellationToken cancellationToken = default);
    Task<string> GenerateConfigurationAsync(string configType, Dictionary<string, object> parameters, CancellationToken cancellationToken = default);
    Task<string> GenerateStyleAsync(string styleType, Dictionary<string, object> parameters, CancellationToken cancellationToken = default);
    Task<string> GeneratePluginAsync(string pluginName, Dictionary<string, object> parameters, CancellationToken cancellationToken = default);
}
```

#### ITemplateService

```csharp
public interface ITemplateService
{
    Task<string> GetTemplateAsync(string templateName, CancellationToken cancellationToken = default);
    Task<string> RenderTemplateAsync(string templateName, Dictionary<string, object> parameters, CancellationToken cancellationToken = default);
    void RegisterTemplate(string templateName, string templateContent);
    IEnumerable<string> GetAvailableTemplates();
}
```

## 7. 故障排除

### 7.1 常见问题

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

#### 问题：代码生成失败

**解决方案**：
- 检查模板名称是否存在
- 确保参数格式正确
- 检查输出目录权限

### 7.2 错误代码

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
| TW009 | 模板不存在 | 检查模板名称是否正确 |
| TW010 | 代码生成失败 | 检查参数格式和输出权限 |

### 7.3 调试技巧

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

#### 测试生成器

```bash
# 测试组件生成
dotnet run --project tailwind_generator.csproj component Test --template component_button
```

## 8. 性能优化

### 8.1 内存缓存

TailwindCSS 技能使用内存缓存来提高性能：

- **构建结果缓存**：缓存编译结果，避免重复编译
- **模板缓存**：缓存模板内容，提高模板渲染速度
- **配置缓存**：缓存配置文件，减少文件 I/O

### 8.2 并行处理

TailwindCSS 技能使用并行处理来加速构建过程：

- **多线程构建**：使用多个线程同时处理不同的构建步骤
- **并行插件执行**：并行执行插件，提高插件处理速度
- **并行文件扫描**：并行扫描文件，提高内容检测速度

### 8.3 AOT 编译

AOT 编译可以显著提高 TailwindCSS 技能的性能：

- **启动速度快**：AOT 编译的应用启动速度比 JIT 编译快得多
- **运行时性能好**：避免了运行时编译的开销
- **内存使用少**：不需要加载和编译 IL 代码
- **部署简单**：单文件部署，不需要安装 .NET 运行时

### 8.4 其他优化

- **代码压缩**：优化输出的 CSS 文件大小
- **增量构建**：只重新构建变化的部分
- **文件系统抽象**：减少文件 I/O 开销
- **日志级别控制**：在生产环境中使用较低的日志级别

## 9. 扩展和定制

### 9.1 自定义主题

TailwindCSS 技能支持自定义主题：

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
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
        mono: ['Fira Code', 'monospace'],
      },
    },
  },
};
```

### 9.2 自定义插件

TailwindCSS 技能支持自定义插件：

```csharp
public class MyCustomPlugin : IPlugin
{
    public string Name => "MyCustomPlugin";
    
    public Task ExecuteAsync(string input, CancellationToken cancellationToken = default)
    {
        // 插件逻辑
        return Task.CompletedTask;
    }
    
    public void Configure(PluginConfiguration configuration)
    {
        // 插件配置
    }
}
```

### 9.3 自定义模板

TailwindCSS 技能支持自定义模板：

```csharp
var templateService = serviceProvider.GetRequiredService<ITemplateService>();
templateService.RegisterTemplate("custom_template", @"/* Custom Template */
@layer components {
  .custom-class {
    @apply px-4 py-2 rounded;
  }
}");
```

### 9.4 自定义构建步骤

TailwindCSS 技能支持自定义构建步骤：

```csharp
public class CustomBuildStep : IBuildStep
{
    public string Name => "CustomBuildStep";
    
    public Task<string> ExecuteAsync(string input, BuildContext context, CancellationToken cancellationToken = default)
    {
        // 自定义构建逻辑
        return Task.FromResult(input);
    }
}
```

## 10. 部署和分发

### 10.1 构建和发布

#### 构建 Debug 版本

```bash
# 构建 Debug 版本
dotnet build -c Debug
```

#### 构建 Release 版本

```bash
# 构建 Release 版本
dotnet build -c Release
```

#### 发布为 AOT 编译的单文件应用

```bash
# 发布为 AOT 编译的单文件应用
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishAot=true -p:TrimMode=partial -p:PublishSingleFile=true
```

### 10.2 分发

TailwindCSS 技能可以分发为：

- **单文件可执行文件**：适合直接运行
- **NuGet 包**：适合集成到其他项目
- **Docker 容器**：适合容器化部署

### 10.3 环境要求

TailwindCSS 技能的环境要求：

- **.NET 10 SDK**：用于构建和开发
- **.NET 10 Runtime**：用于运行（如果不使用自包含部署）
- **操作系统**：Windows、Linux 或 macOS
- **文件系统权限**：读取输入文件和写入输出文件的权限

## 11. 开发指南

### 11.1 开发环境设置

1. **安装 .NET 10 SDK**
   - 从 [Microsoft 官网](https://dotnet.microsoft.com/download/dotnet/10.0) 下载并安装 .NET 10 SDK

2. **克隆仓库**
   ```bash
   git clone https://github.com/dotnet-expert/tailwindcss-skill.git
   cd tailwindcss-skill
   ```

3. **安装依赖**
   ```bash
   dotnet restore
   ```

4. **构建项目**
   ```bash
   dotnet build
   ```

### 11.2 代码规范

TailwindCSS 技能遵循以下代码规范：

- **命名约定**：使用 PascalCase 命名类和方法，使用 camelCase 命名变量
- **缩进**：使用 4 个空格缩进
- **换行**：每行不超过 120 个字符
- **注释**：为公共接口和复杂逻辑添加注释
- **异常处理**：使用 try-catch 处理异常，提供详细的错误信息
- **日志记录**：使用 Microsoft.Extensions.Logging 记录日志，根据不同级别记录不同详细程度的信息

### 11.3 测试

TailwindCSS 技能使用以下测试方法：

- **单元测试**：测试单个功能和方法
- **集成测试**：测试多个组件的集成
- **端到端测试**：测试完整的构建流程

### 11.4 贡献

贡献 TailwindCSS 技能的步骤：

1. **Fork 仓库**
2. **创建分支**
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **实现功能**
4. **运行测试**
   ```bash
   dotnet test
   ```
5. **提交更改**
   ```bash
   git add .
   git commit -m "Add your feature description"
   ```
6. **推送到远程**
   ```bash
   git push origin feature/your-feature-name
   ```
7. **创建 Pull Request**

## 12. 许可证

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

## 13. 联系我们

- **GitHub**：[https://github.com/dotnet-expert/tailwindcss-skill](https://github.com/dotnet-expert/tailwindcss-skill)
- **Discord**：[https://discord.gg/dotnet](https://discord.gg/dotnet)
- **Email**：expert@dotnet.com

---

**TailwindCSS 技能** - 为 .NET 开发者提供强大的 CSS 工具链
