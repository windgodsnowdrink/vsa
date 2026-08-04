# Source Generator 技能文档

## 技能概述

Source Generator 技能是一个基于 .NET 10 开发的完整 Source Generator 解决方案，支持 AOT 编译，提供代码生成、模板生成、反射增强、特性处理、增量生成和代码分析等功能。

### 主要特性

- **AOT 编译优化**：支持 Ahead-of-Time 编译，提供更快的启动速度和更低的内存占用
- **完整的 Source Generator 功能**：代码生成、模板生成、反射增强、特性处理、增量生成、代码分析
- **Roslyn 集成**：与 Roslyn 编译器无缝集成，支持编译时代码生成
- **Scrutor 支持**：提供高级服务注册和装饰器模式的使用示例
- **命令行接口**：通过 System.CommandLine 提供友好的命令行操作界面
- **依赖注入**：集成 Microsoft.Extensions.DependencyInjection 实现服务管理
- **异步编程**：全面使用 async/await 模式提高性能
- **增量编译**：支持增量编译和代码生成，提高开发效率
- **代码分析**：支持编译时代码分析和验证

## 快速开始

### 环境要求

- .NET 10.0 或更高版本
- Visual Studio 2026 或 VS Code
- .NET SDK 10.0 或更高版本

### 安装步骤

1. **克隆或下载技能**：将 Source Generator 技能目录复制到您的项目中
2. **配置依赖**：确保项目引用了必要的依赖项（Microsoft.CodeAnalysis.CSharp 等）
3. **编译技能**：使用 .NET CLI 编译技能
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true
   ```
4. **运行技能**：使用命令行工具执行代码生成操作
   ```bash
   sourcegenerator generate --type "class" --output "GeneratedClass.cs"
   ```

## 核心功能

### 1. 代码生成

支持基于 Source Generator 的代码生成，包括类、接口、枚举、结构体等各种类型的代码生成。

**功能特点**：
- 支持多种代码类型的生成
- 支持自定义代码模板
- 支持基于现有代码的增量生成
- 支持编译时代码生成

### 2. 模板生成

支持代码模板的创建、管理和使用，实现代码生成的标准化和个性化。

**功能特点**：
- 模板参数替换
- 模板版本管理
- 多语言模板支持
- 模板使用统计

### 3. 反射增强

提供编译时反射信息生成，减少运行时反射的性能开销。

**功能特点**：
- 编译时类型信息生成
- 成员信息收集
- 反射操作代码生成
- 性能优化

### 4. 特性处理

支持自定义特性的处理和代码生成，实现基于特性的代码自动生成。

**功能特点**：
- 自定义特性定义
- 特性参数处理
- 基于特性的代码生成
- 特性验证

### 5. 增量生成

支持增量编译和代码生成，提高开发效率。

**功能特点**：
- 增量编译支持
- 变更检测
- 增量代码生成
- 性能优化

### 6. 代码分析

支持编译时代码分析和验证，提高代码质量。

**功能特点**：
- 编译时代码分析
- 代码质量验证
- 潜在问题检测
- 代码风格检查

## API 参考

### ISourceGeneratorService 接口

```csharp
public interface ISourceGeneratorService
{
    Task<string> GenerateCodeAsync(string type, string outputPath, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
    Task<string> GenerateFromTemplateAsync(string templateId, string outputPath, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
    Task<string> GenerateReflectionInfoAsync(string typeName, string outputPath, CancellationToken cancellationToken = default);
    Task<string> GenerateFromAttributeAsync(string attributeName, string outputPath, CancellationToken cancellationToken = default);
    Task<string> AnalyzeCodeAsync(string filePath, CancellationToken cancellationToken = default);
}
```

### ITemplateService 接口

```csharp
public interface ITemplateService
{
    Task<string> CreateTemplateAsync(string name, string content, CancellationToken cancellationToken = default);
    Task<IEnumerable<CodeTemplate>> GetTemplatesAsync(CancellationToken cancellationToken = default);
    Task<CodeTemplate> GetTemplateAsync(string templateId, CancellationToken cancellationToken = default);
    Task UpdateTemplateAsync(string templateId, string name, string content, CancellationToken cancellationToken = default);
    Task DeleteTemplateAsync(string templateId, CancellationToken cancellationToken = default);
    Task<string> RenderTemplateAsync(string templateId, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
}
```

### IAttributeService 接口

```csharp
public interface IAttributeService
{
    Task<string> CreateAttributeAsync(string name, string parameters, CancellationToken cancellationToken = default);
    Task<IEnumerable<CustomAttribute>> GetAttributesAsync(CancellationToken cancellationToken = default);
    Task<CustomAttribute> GetAttributeAsync(string attributeId, CancellationToken cancellationToken = default);
    Task UpdateAttributeAsync(string attributeId, string name, string parameters, CancellationToken cancellationToken = default);
    Task DeleteAttributeAsync(string attributeId, CancellationToken cancellationToken = default);
}
```

### ICodeAnalysisService 接口

```csharp
public interface ICodeAnalysisService
{
    Task<CodeAnalysisResult> AnalyzeFileAsync(string filePath, CancellationToken cancellationToken = default);
    Task<CodeAnalysisResult> AnalyzeProjectAsync(string projectPath, CancellationToken cancellationToken = default);
    Task<IEnumerable<CodeIssue>> GetIssuesAsync(string filePath, CancellationToken cancellationToken = default);
}
```

## AOT 编译

### 编译配置

Source Generator 技能支持 AOT 编译，通过以下配置实现：

```yaml
compilation:
  targetFramework: net10.0
  publishAot: true
  trimMode: partial
  selfContained: true
  publishSingleFile: true
  langVersion: latest
  nullable: enable
  implicitUsings: enable
```

### 编译命令

```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true

# Linux
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true

# macOS
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true
```

### AOT 优化效果

- **启动速度**：比 JIT 编译快 3-5 倍
- **内存占用**：减少约 20-30%
- **运行性能**：关键路径性能提升 5-15%
- **部署大小**：单文件部署，便于分发

## 示例

### 基本代码生成示例

```csharp
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSourceGeneratorServices();

var serviceProvider = services.BuildServiceProvider();
var generatorService = serviceProvider.GetRequiredService<ISourceGeneratorService>();

var parameters = new Dictionary<string, string>
{
    { "ClassName", "Person" },
    { "Namespace", "MyApp.Models" },
    { "Properties", "string Name, int Age, string Email" }
};

var result = await generatorService.GenerateCodeAsync("class", "Person.cs", parameters);
Console.WriteLine($"代码生成: {result}");
```

### 模板使用示例

```csharp
var templateService = serviceProvider.GetRequiredService<ITemplateService>();

// 创建模板
var templateId = await templateService.CreateTemplateAsync(
    "repository",
    "public class {{ClassName}}Repository : I{{ClassName}}Repository
{
    public async Task<{{ClassName}}> GetByIdAsync(int id)
    {
        // 实现逻辑
        return null;
    }
}"
);

// 使用模板生成代码
var parameters = new Dictionary<string, string>
{
    { "ClassName", "Person" }
};

var result = await generatorService.GenerateFromTemplateAsync(templateId, "PersonRepository.cs", parameters);
```

### 反射信息生成示例

```csharp
var result = await generatorService.GenerateReflectionInfoAsync(
    "MyApp.Models.Person",
    "PersonReflectionInfo.cs"
);

Console.WriteLine($"反射信息生成: {result}");
```

### 特性处理示例

```csharp
// 创建自定义特性
var attributeService = serviceProvider.GetRequiredService<IAttributeService>();
var attributeId = await attributeService.CreateAttributeAsync(
    "GenerateDto",
    "string TargetType"
);

// 使用特性标记类
[GenerateDto(TargetType = "PersonDto")]
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
}

// 基于特性生成代码
var result = await generatorService.GenerateFromAttributeAsync(
    "GenerateDto",
    "PersonDto.cs"
);
```

### 代码分析示例

```csharp
var analysisService = serviceProvider.GetRequiredService<ICodeAnalysisService>();
var result = await analysisService.AnalyzeFileAsync("Person.cs");

Console.WriteLine($"代码分析结果:");
Console.WriteLine($"文件: {result.FilePath}");
Console.WriteLine($"类数量: {result.ClassCount}");
Console.WriteLine($"方法数量: {result.MethodCount}");
Console.WriteLine($"属性数量: {result.PropertyCount}");
Console.WriteLine($"问题数量: {result.Issues.Count}");

foreach (var issue in result.Issues)
{
    Console.WriteLine($"  - {issue.Severity}: {issue.Message} (行 {issue.LineNumber})");
}
```

## Scrutor 使用示例

### 服务注册示例

```csharp
using Scrutor;

var services = new ServiceCollection();

// 基本注册
services.AddSingleton<ISourceGeneratorService, SourceGeneratorService>();
services.AddSingleton<ITemplateService, TemplateService>();
services.AddSingleton<IAttributeService, AttributeService>();
services.AddSingleton<ICodeAnalysisService, CodeAnalysisService>();

// 使用 Scrutor 装饰器模式
services.Decorate<ISourceGeneratorService, SourceGeneratorServiceLoggingDecorator>();
services.Decorate<ISourceGeneratorService, SourceGeneratorServiceValidationDecorator>();

// 使用 Scrutor 程序集扫描
services.Scan(scan => scan
    .FromAssemblyOf<SourceGeneratorService>()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithSingletonLifetime());

var serviceProvider = services.BuildServiceProvider();
```

### 装饰器模式示例

```csharp
public class SourceGeneratorServiceLoggingDecorator : ISourceGeneratorService
{
    private readonly ISourceGeneratorService _decorated;
    private readonly ILogger<SourceGeneratorServiceLoggingDecorator> _logger;

    public SourceGeneratorServiceLoggingDecorator(ISourceGeneratorService decorated, ILogger<SourceGeneratorServiceLoggingDecorator> logger)
    {
        _decorated = decorated;
        _logger = logger;
    }

    public async Task<string> GenerateCodeAsync(string type, string outputPath, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("生成代码类型: {Type}, 输出路径: {OutputPath}", type, outputPath);
        var result = await _decorated.GenerateCodeAsync(type, outputPath, parameters, cancellationToken);
        _logger.LogInformation("代码生成完成: {Result}", result);
        return result;
    }

    // 其他方法实现...
}
```

## 故障排除

### 常见问题

1. **代码生成失败**
   - 检查输入参数是否正确
   - 确认目标文件路径是否存在
   - 检查模板内容是否有效
   - 查看生成日志获取详细错误信息

2. **编译错误**
   - 确认依赖项版本是否兼容
   - 检查生成的代码语法是否正确
   - 验证项目配置是否正确
   - 查看编译日志获取详细错误信息

3. **性能问题**
   - 启用增量编译
   - 优化模板内容
   - 减少生成的代码量
   - 考虑使用缓存策略

4. **AOT 编译错误**
   - 确保所有依赖项都支持 AOT 编译
   - 检查代码中是否使用了不支持 AOT 的功能
   - 调整 trimMode 配置为 partial 或 copyused
   - 查看编译日志获取详细错误信息

### 日志和监控

Source Generator 技能集成了 Microsoft.Extensions.Logging 框架，提供了详细的日志记录：

- **日志级别**：支持 Trace、Debug、Information、Warning、Error、Critical 等级别
- **日志输出**：可配置为控制台、文件、数据库等输出目标
- **性能监控**：记录关键操作的执行时间
- **错误追踪**：捕获和记录异常信息

### 性能优化

1. **增量编译**：启用增量编译减少重复生成
2. **缓存策略**：缓存生成结果减少重复工作
3. **并行处理**：使用并行处理提高生成速度
4. **内存管理**：优化内存使用减少 GC 压力
5. **代码优化**：生成高效的代码减少运行时开销

## 总结

Source Generator 技能是一个功能完整、性能优化的 Source Generator 解决方案，基于 .NET 10 和 AOT 编译技术，提供了丰富的功能和友好的使用接口。通过集成 Roslyn 编译器和 Scrutor 高级服务注册库，它不仅满足了基本的代码生成需求，还提供了模板生成、反射增强、特性处理、增量生成和代码分析等高级功能。

无论是用于快速生成重复代码、优化反射性能、基于特性生成代码还是分析代码质量，Source Generator 技能都能提供可靠、高效的代码生成和分析能力。