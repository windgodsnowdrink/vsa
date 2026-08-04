# Source Generator 技能参考文档

## 1. 架构概述

Source Generator 技能采用模块化架构设计，基于 .NET 10 和 AOT 编译技术，提供高性能的代码生成功能。

### 1.1 核心组件

- **SourceGeneratorService**: 核心代码生成服务，处理代码分析和生成逻辑
- **TemplateService**: 模板管理服务，处理模板的加载、解析和渲染
- **AttributeService**: 属性处理服务，处理自定义属性的分析和处理
- **CodeAnalysisService**: 代码分析服务，提供编译时代码分析功能
- **ReflectionService**: 反射信息生成服务，提供编译时反射信息
- **CodeGeneratorService**: 代码生成服务，处理具体的代码生成任务
- **GeneratorRegistry**: 生成器注册表，管理生成器的注册和发现

### 1.2 数据流

1. **输入阶段**: 接收源代码文件和配置参数
2. **分析阶段**: 分析源代码结构、属性和元数据
3. **生成阶段**: 根据模板和分析结果生成代码
4. **输出阶段**: 将生成的代码写入指定位置
5. **验证阶段**: 验证生成的代码是否正确

## 2. 核心 API 参考

### 2.1 ISourceGeneratorService

```csharp
public interface ISourceGeneratorService
{
    Task GenerateCodeAsync(string projectPath, string outputPath);
    Task GenerateReflectionInfoAsync(string assemblyPath, string outputPath);
    Task AnalyzeCodeAsync(string projectPath, string outputPath);
}
```

**参数说明**:
- `projectPath`: 项目文件路径
- `outputPath`: 输出文件路径
- `assemblyPath`: 程序集文件路径

**返回值**:
- `Task`: 表示异步操作的任务

### 2.2 ITemplateService

```csharp
public interface ITemplateService
{
    Task<string?> GetTemplateAsync(string name);
    Task<IEnumerable<string>> GetTemplateNamesAsync();
    string ProcessTemplate(string template, Dictionary<string, string> parameters);
    Task SaveTemplateAsync(string name, string content);
    Task DeleteTemplateAsync(string name);
}
```

**参数说明**:
- `name`: 模板名称
- `template`: 模板内容
- `parameters`: 模板参数
- `content`: 模板内容

**返回值**:
- `string?`: 模板内容，不存在则返回 null
- `IEnumerable<string>`: 模板名称列表
- `string`: 处理后的模板内容
- `Task`: 表示异步操作的任务

### 2.3 IAttributeService

```csharp
public interface IAttributeService
{
    Task<IEnumerable<AttributeInfo>> GetAttributesAsync(string assemblyPath, string attributeName);
    Task ProcessAttributesAsync(string assemblyPath, Action<AttributeInfo> processor);
}
```

**参数说明**:
- `assemblyPath`: 程序集文件路径
- `attributeName`: 属性名称
- `processor`: 属性处理器

**返回值**:
- `IEnumerable<AttributeInfo>`: 属性信息列表
- `Task`: 表示异步操作的任务

### 2.4 ICodeAnalysisService

```csharp
public interface ICodeAnalysisService
{
    Task<CodeAnalysisResult> AnalyzeProjectAsync(string projectPath);
    Task<CodeAnalysisResult> AnalyzeAssemblyAsync(string assemblyPath);
}
```

**参数说明**:
- `projectPath`: 项目文件路径
- `assemblyPath`: 程序集文件路径

**返回值**:
- `Task<CodeAnalysisResult>`: 代码分析结果

### 2.5 IReflectionService

```csharp
public interface IReflectionService
{
    Task GenerateReflectionInfoAsync(string assemblyPath, string outputPath);
    Task<ReflectionInfo> GetReflectionInfoAsync(string assemblyPath);
}
```

**参数说明**:
- `assemblyPath`: 程序集文件路径
- `outputPath`: 输出文件路径

**返回值**:
- `Task`: 表示异步操作的任务
- `Task<ReflectionInfo>`: 反射信息

### 2.6 ICodeGeneratorService

```csharp
public interface ICodeGeneratorService
{
    Task<string> GenerateCodeAsync(string templateName, Dictionary<string, string> parameters);
    Task GenerateFileAsync(string templateName, string outputPath, Dictionary<string, string> parameters);
    Task<IEnumerable<string>> GetAvailableTemplatesAsync();
}
```

**参数说明**:
- `templateName`: 模板名称
- `parameters`: 模板参数
- `outputPath`: 输出文件路径

**返回值**:
- `Task<string>`: 生成的代码
- `Task`: 表示异步操作的任务
- `Task<IEnumerable<string>>`: 可用模板列表

### 2.7 IGeneratorRegistry

```csharp
public interface IGeneratorRegistry
{
    void RegisterGenerators(IServiceCollection services);
    void RegisterDecorators(IServiceCollection services);
    void RegisterExtensions(IServiceCollection services);
}
```

**参数说明**:
- `services`: 服务集合

**返回值**:
- 无

## 3. 配置选项

### 3.1 编译配置

在 `.setting.json` 文件中配置编译选项：

```json
{
  "compilationOptions": {
    "targetFramework": "net10.0",
    "langVersion": "preview",
    "nullable": "enable",
    "implicitUsings": "enable",
    "publishOptions": {
      "PublishAot": true,
      "TrimMode": "partial",
      "SelfContained": true,
      "PublishSingleFile": true,
      "PublishReadyToRun": true,
      "RuntimeIdentifier": "win-x64"
    }
  }
}
```

### 3.2 运行配置

在 `.run.json` 文件中配置运行选项：

```json
{
  "profiles": {
    "Generate Code": {
      "commandName": "Project",
      "commandLineArgs": "generate --output ./output --namespace MyProject.Generators",
      "workingDirectory": "$(ProjectDir)",
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Development",
        "SOURCEGENERATOR_OUTPUT_PATH": "./output"
      }
    }
  }
}
```

### 3.3 环境变量

| 环境变量 | 描述 | 默认值 |
|---------|------|-------|
| `DOTNET_ENVIRONMENT` | .NET 环境 | `Production` |
| `SOURCEGENERATOR_OUTPUT_PATH` | 输出路径 | `./output` |
| `SOURCEGENERATOR_TEMPLATES_PATH` | 模板路径 | `./templates` |
| `SOURCEGENERATOR_LOG_LEVEL` | 日志级别 | `Information` |
| `SCRUTOR_DEMO_ENABLED` | Scrutor 演示模式 | `false` |

## 4. CLI 命令参考

### 4.1 核心命令

#### `generate`

生成代码从模板

**参数**:
- `--template`: 模板名称（必需）
- `--output`: 输出文件路径（必需）
- `--parameters`: 模板参数，格式为 key=value

**示例**:
```bash
sourcegenerator_generator.exe generate --template ClassGenerator --output ./Generated/MyClass.cs --parameters namespace=MyProject className=MyClass properties=Id:int,Name:string
```

#### `list`

列出可用模板

**示例**:
```bash
sourcegenerator_generator.exe list
```

#### `scrutor-demo`

演示 Scrutor 用法

**示例**:
```bash
sourcegenerator_generator.exe scrutor-demo
```

### 4.2 高级命令

#### `generate-reflection`

生成反射信息

**参数**:
- `--assembly`: 程序集文件路径（必需）
- `--output`: 输出文件路径（必需）

**示例**:
```bash
sourcegenerator_core.exe generate-reflection --assembly ./bin/Debug/net10.0/MyProject.dll --output ./reflection-info.json
```

#### `analyze`

分析代码

**参数**:
- `--project`: 项目文件路径（必需）
- `--output`: 输出文件路径（必需）

**示例**:
```bash
sourcegenerator_core.exe analyze --project ./MyProject.csproj --output ./analysis-result.json
```

#### `template generate`

生成模板

**参数**:
- `--name`: 模板名称（必需）
- `--output`: 输出路径（必需）

**示例**:
```bash
sourcegenerator_core.exe template generate --name MyTemplate --output ./templates
```

#### `template list`

列出模板

**示例**:
```bash
sourcegenerator_core.exe template list
```

#### `template validate`

验证模板

**参数**:
- `--path`: 模板路径（必需）

**示例**:
```bash
sourcegenerator_core.exe template validate --path ./templates/MyTemplate.tmpl
```

## 5. 扩展指南

### 5.1 创建自定义生成器

1. **创建生成器类**:

```csharp
[GeneratorAttribute("MyCustomGenerator", "Generates custom code")]
public class MyCustomGenerator
{
    public string GenerateCustomCode(string parameter1, int parameter2)
    {
        // 生成代码逻辑
        return $"// Custom code generated with parameters: {parameter1}, {parameter2}";
    }
}
```

2. **注册生成器**:

```csharp
services.Scan(scan => scan
    .FromAssemblyOf<MyCustomGenerator>()
    .AddClasses(classes => classes.WithAttribute<GeneratorAttribute>())
    .AsSelfWithInterfaces()
    .WithSingletonLifetime()
);
```

### 5.2 创建自定义模板

1. **创建模板文件** (例如: `MyTemplate.tmpl`):

```
namespace {{Namespace}}
{
    public class {{ClassName}}
    {
        {{#each Properties}}
        public {{Type}} {{Name}} { get; set; }
        {{/each}}
    }
}
```

2. **使用模板**:

```csharp
var parameters = new Dictionary<string, string>
{
    { "Namespace", "MyProject" },
    { "ClassName", "MyClass" },
    { "Properties", "Id:int,Name:string" }
};

await generatorService.GenerateFileAsync("MyTemplate", "./Generated/MyClass.cs", parameters);
```

### 5.3 创建自定义装饰器

```csharp
public class MyCodeGeneratorDecorator : ICodeGeneratorService
{
    private readonly ICodeGeneratorService _inner;

    public MyCodeGeneratorDecorator(ICodeGeneratorService inner)
    {
        _inner = inner;
    }

    public async Task<string> GenerateCodeAsync(string templateName, Dictionary<string, string> parameters)
    {
        // 前置处理
        var result = await _inner.GenerateCodeAsync(templateName, parameters);
        // 后置处理
        return result;
    }

    // 实现其他方法...
}
```

**注册装饰器**:

```csharp
services.Decorate<ICodeGeneratorService, MyCodeGeneratorDecorator>();
```

## 6. 性能优化

### 6.1 AOT 编译优化

- **启用 PublishAot**: 提高启动速度和运行时性能
- **使用 TrimMode=partial**: 减少应用程序大小
- **启用 PublishReadyToRun**: 提高启动速度
- **使用 SelfContained**: 简化部署
- **使用 PublishSingleFile**: 减少文件数量

### 6.2 内存优化

- **使用对象池**: 减少内存分配
- **使用 Span<T> 和 Memory<T>**: 减少内存复制
- **避免不必要的字符串操作**: 使用 StringBuilder
- **使用异步编程**: 提高并发性能
- **使用缓存**: 减少重复计算

### 6.3 并行处理

- **使用 Parallel.ForEach**: 并行处理多个文件
- **使用 Task.WhenAll**: 并行执行多个异步操作
- **使用 Channels**: 实现高效的生产者-消费者模式
- **使用 Dataflow**: 实现复杂的数据流处理

## 7. 部署指南

### 7.1 本地部署

1. **编译应用程序**:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true -p:TrimMode=partial
```

2. **运行应用程序**:

```bash
./sourcegenerator_core.exe generate --output ./output --namespace MyProject.Generators
```

### 7.2 CI/CD 集成

**GitHub Actions 示例**:

```yaml
name: Source Generator CI

on: [push, pull_request]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '10.0.x'
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --configuration Release
    - name: Test
      run: dotnet test --configuration Release
    - name: Publish
      run: dotnet publish -c Release -r ubuntu-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true -p:TrimMode=partial
```

### 7.3 Docker 部署

**Dockerfile 示例**:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true -p:TrimMode=partial -o /out

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

ENTRYPOINT ["./sourcegenerator_core"]
```

**构建和运行**:

```bash
docker build -t sourcegenerator .
docker run --rm -v "$(pwd)/output:/app/output" sourcegenerator generate --output /app/output --namespace MyProject.Generators
```

## 8. 监控和日志

### 8.1 日志配置

**配置日志级别**:

```json
{
  "environmentVariables": {
    "SOURCEGENERATOR_LOG_LEVEL": "Debug"
  }
}
```

### 8.2 日志输出

| 日志级别 | 描述 |
|---------|------|
| `Trace` | 最详细的日志，包含所有信息 |
| `Debug` | 调试信息，包含详细的执行过程 |
| `Information` | 常规信息，包含执行状态 |
| `Warning` | 警告信息，包含潜在问题 |
| `Error` | 错误信息，包含执行错误 |
| `Critical` | 严重错误信息，包含系统故障 |

### 8.3 性能监控

**关键指标**:
- **代码生成时间**: 生成代码所需的时间
- **内存使用**: 执行过程中的内存使用情况
- **CPU 使用率**: 执行过程中的 CPU 使用情况
- **文件 I/O 操作**: 文件读写操作的次数和时间
- **模板渲染时间**: 模板渲染所需的时间

## 9. 安全考虑

### 9.1 输入验证

- **验证模板名称**: 确保模板名称合法
- **验证输出路径**: 确保输出路径在允许的范围内
- **验证参数**: 确保参数值合法
- **防止路径遍历**: 防止通过参数访问系统文件

### 9.2 代码安全

- **防止代码注入**: 确保生成的代码不包含恶意内容
- **验证生成的代码**: 确保生成的代码符合安全标准
- **限制模板权限**: 限制模板可以执行的操作

### 9.3 权限管理

- **文件系统权限**: 确保应用程序有适当的文件系统权限
- **网络权限**: 确保应用程序有适当的网络权限
- **环境变量安全**: 确保环境变量不包含敏感信息

## 10. 故障排除

### 10.1 常见问题

| 问题 | 原因 | 解决方案 |
|-----|------|--------|
| 模板未找到 | 模板路径配置错误 | 检查 `SOURCEGENERATOR_TEMPLATES_PATH` 环境变量 |
| 代码生成失败 | 模板语法错误 | 检查模板文件语法 |
| 权限被拒绝 | 文件系统权限不足 | 确保应用程序有适当的权限 |
| 内存不足 | 处理大型项目 | 增加系统内存或分批处理 |
| 编译错误 | 生成的代码有语法错误 | 检查生成的代码语法 |

### 10.2 诊断工具

- **日志分析**: 检查应用程序日志
- **性能分析**: 使用 dotnet-trace 分析性能
- **内存分析**: 使用 dotnet-dump 分析内存
- **代码分析**: 使用 Roslyn 分析器分析代码

### 10.3 调试技巧

1. **启用详细日志**:

```bash
export SOURCEGENERATOR_LOG_LEVEL=Debug
./sourcegenerator_core.exe generate --output ./output
```

2. **使用调试器**:

```bash
dotnet build -c Debug
dotnet debug ./bin/Debug/net10.0/sourcegenerator_core.dll generate --output ./output
```

3. **检查生成的代码**:

```bash
cat ./output/GeneratedCode.cs
```

## 11. 总结

Source Generator 技能是一个功能强大的代码生成工具，基于 .NET 10 和 AOT 编译技术，提供高性能、可扩展的代码生成功能。通过本文档，您应该能够：

1. **理解** Source Generator 技能的架构和核心组件
2. **使用** 核心 API 和 CLI 命令
3. **配置** 编译和运行选项
4. **扩展** 创建自定义生成器和模板
5. **优化** 性能和内存使用
6. **部署** 在不同环境中部署应用程序
7. **监控** 应用程序的运行状态
8. **排除** 常见问题和故障

Source Generator 技能为您的开发工作流程提供了强大的代码生成能力，帮助您减少重复代码，提高开发效率，确保代码质量。