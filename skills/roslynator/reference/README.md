# Roslynator 技术参考文档

## 1. 架构概述

Roslynator 是一个基于 .NET 10 和 AOT 编译的代码分析与重构工具，采用模块化架构设计，主要由以下组件组成：

### 1.1 核心组件

- **代码分析引擎**：基于 Roslyn 编译器，负责分析代码质量、检测潜在问题
- **代码重构引擎**：基于 Roslyn 编译器，负责执行代码自动重构操作
- **命令行接口**：使用 System.CommandLine 库，提供用户交互界面
- **依赖注入容器**：使用 Microsoft.Extensions.DependencyInjection，管理服务生命周期
- **报告生成器**：生成结构化的分析和重构报告

### 1.2 架构层次

```
┌─────────────────────────────────────────────────────────┐
│                       命令行接口                        │
├─────────────────────────────────────────────────────────┤
│                     服务层 (Services)                  │
│ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌───────┐ │
│ │ 分析服务    │ │ 重构服务    │ │ 项目加载器  │ │ 报告  │ │
│ └─────────────┘ └─────────────┘ └─────────────┘ └───────┘ │
├─────────────────────────────────────────────────────────┤
│                    核心引擎 (Engines)                  │
│ ┌─────────────┐ ┌────────────────────────────────────┐ │
│ │ 分析引擎    │ │ 重构引擎                           │ │
│ │             │ │ ┌─────────┐ ┌─────────┐ ┌─────────┐ │ │
│ │             │ │ │表达式体 │ │字符串插值│ │模式匹配 │ │ │
│ │             │ │ └─────────┘ └─────────┘ └─────────┘ │ │
│ └─────────────┘ └────────────────────────────────────┘ │
├─────────────────────────────────────────────────────────┤
│                     Roslyn 库                         │
├─────────────────────────────────────────────────────────┤
│                    .NET 10 运行时                      │
└─────────────────────────────────────────────────────────┘
```

### 1.3 数据流

1. **输入**：用户通过命令行指定要分析或重构的文件、项目或解决方案路径
2. **处理**：
   - 项目加载器加载目标文件/项目/解决方案
   - 分析引擎或重构引擎执行相应操作
   - 重构引擎使用语法重写器修改代码
3. **输出**：
   - 分析结果或重构操作记录
   - 生成结构化报告
   - （重构时）修改原始文件

## 2. API 详细说明

### 2.1 核心接口

#### ICodeAnalyzer

```csharp
public interface ICodeAnalyzer
{
    Task<AnalysisResult> AnalyzeAsync(string path, AnalyzerOptions options, CancellationToken cancellationToken);
}
```

- **参数**：
  - `path`：要分析的文件、项目或解决方案路径
  - `options`：分析选项，包括严重程度过滤等
  - `cancellationToken`：取消令牌
- **返回值**：包含分析结果的 `AnalysisResult` 对象

#### IRefactoringProvider

```csharp
public interface IRefactoringProvider
{
    Task<RefactoringResult> RefactorAsync(string path, RefactoringOptions options, CancellationToken cancellationToken);
}
```

- **参数**：
  - `path`：要重构的文件、项目或解决方案路径
  - `options`：重构选项，包括重构类型等
  - `cancellationToken`：取消令牌
- **返回值**：包含重构结果的 `RefactoringResult` 对象

#### IAnalysisEngine

```csharp
public interface IAnalysisEngine
{
    Task<List<DiagnosticInfo>> AnalyzeFileAsync(string filePath, AnalyzerOptions options, CancellationToken cancellationToken);
    Task<List<DiagnosticInfo>> AnalyzeProjectAsync(Project project, AnalyzerOptions options, CancellationToken cancellationToken);
    Task<List<DiagnosticInfo>> AnalyzeSolutionAsync(Solution solution, AnalyzerOptions options, CancellationToken cancellationToken);
}
```

- **方法**：
  - `AnalyzeFileAsync`：分析单个文件
  - `AnalyzeProjectAsync`：分析整个项目
  - `AnalyzeSolutionAsync`：分析整个解决方案

#### IRefactoringEngine

```csharp
public interface IRefactoringEngine
{
    Task<List<RefactoringInfo>> RefactorFileAsync(string filePath, RefactoringOptions options, CancellationToken cancellationToken);
    Task<List<RefactoringInfo>> RefactorProjectAsync(Project project, RefactoringOptions options, CancellationToken cancellationToken);
    Task<List<RefactoringInfo>> RefactorSolutionAsync(Solution solution, RefactoringOptions options, CancellationToken cancellationToken);
}
```

- **方法**：
  - `RefactorFileAsync`：重构单个文件
  - `RefactorProjectAsync`：重构整个项目
  - `RefactorSolutionAsync`：重构整个解决方案

#### IProjectLoader

```csharp
public interface IProjectLoader
{
    Task<Project> LoadProjectAsync(string projectPath, CancellationToken cancellationToken);
    Task<Solution> LoadSolutionAsync(string solutionPath, CancellationToken cancellationToken);
}
```

- **方法**：
  - `LoadProjectAsync`：加载 .csproj 项目文件
  - `LoadSolutionAsync`：加载 .sln 解决方案文件

### 2.2 数据结构

#### AnalysisResult

```csharp
public class AnalysisResult
{
    public string Path { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<DiagnosticInfo> Diagnostics { get; set; } = new List<DiagnosticInfo>();
}
```

- **属性**：
  - `Path`：分析路径
  - `StartTime`：开始时间
  - `EndTime`：结束时间
  - `Diagnostics`：诊断信息列表

#### RefactoringResult

```csharp
public class RefactoringResult
{
    public string Path { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<RefactoringInfo> Refactorings { get; set; } = new List<RefactoringInfo>();
}
```

- **属性**：
  - `Path`：重构路径
  - `StartTime`：开始时间
  - `EndTime`：结束时间
  - `Refactorings`：重构操作列表

#### DiagnosticInfo

```csharp
public class DiagnosticInfo
{
    public string Id { get; set; }
    public string Message { get; set; }
    public string Severity { get; set; }
    public string FilePath { get; set; }
    public int LineNumber { get; set; }
    public int ColumnNumber { get; set; }
}
```

- **属性**：
  - `Id`：诊断 ID
  - `Message`：诊断消息
  - `Severity`：严重程度
  - `FilePath`：文件路径
  - `LineNumber`：行号
  - `ColumnNumber`：列号

#### RefactoringInfo

```csharp
public class RefactoringInfo
{
    public string RefactoringType { get; set; }
    public string Description { get; set; }
    public string FilePath { get; set; }
    public int LineNumber { get; set; }
    public int ColumnNumber { get; set; }
    public string OldCode { get; set; }
    public string NewCode { get; set; }
}
```

- **属性**：
  - `RefactoringType`：重构类型
  - `Description`：重构描述
  - `FilePath`：文件路径
  - `LineNumber`：行号
  - `ColumnNumber`：列号
  - `OldCode`：重构前代码
  - `NewCode`：重构后代码

## 3. 配置指南

### 3.1 项目配置

#### index.yaml 配置

`index.yaml` 文件用于配置技能的基本信息、依赖项、功能和 AOT 编译选项：

```yaml
# 基本信息
name: Roslynator
version: 1.0.0
author: 大佬
description: 基于 .NET 10 和 AOT 编译的代码分析与重构工具

# 依赖项
dependencies:
  - name: Microsoft.CodeAnalysis.CSharp
    version: 4.10.0

# 功能配置
features:
  - name: code-analysis
    description: 代码质量分析

# AOT 编译配置
aot:
  enabled: true
  options:
    - name: PublishAot
      value: true
    - name: TrimMode
      value: partial
```

#### .setting.json 配置

每个 .cs 文件都有对应的 .setting.json 文件，用于配置编译选项和依赖项：

```json
{
  "compilationOptions": {
    "targetFramework": "net10.0",
    "langVersion": "preview",
    "publishAot": true,
    "trimMode": "partial",
    "selfContained": true,
    "publishSingleFile": true,
    "runtimeIdentifier": "win-x64"
  },
  "dependencies": {
    "Microsoft.CodeAnalysis.CSharp": "4.10.0",
    "System.CommandLine": "2.0.0"
  }
}
```

#### .run.json 配置

每个 .cs 文件都有对应的 .run.json 文件，用于配置运行时参数和环境变量：

```json
{
  "command": "roslynator_analyzer.exe",
  "args": [
    "analyze",
    "--path", "${workspaceFolder}",
    "--severity", "warning",
    "--output", "analysis-report.json"
  ],
  "env": {
    "DOTNET_ENVIRONMENT": "Production"
  }
}
```

### 3.2 命令行参数

#### 分析命令参数

| 参数 | 类型 | 描述 | 默认值 | 必需 |
|------|------|------|--------|------|
| `--path` | string | 要分析的文件、项目或解决方案路径 | - | 是 |
| `--severity` | string | 分析结果的严重程度过滤 | warning | 否 |
| `--output` | string | 分析报告输出路径 | analysis-report.json | 否 |

#### 重构命令参数

| 参数 | 类型 | 描述 | 默认值 | 必需 |
|------|------|------|--------|------|
| `--path` | string | 要重构的文件、项目或解决方案路径 | - | 是 |
| `--refactoring-type` | string | 重构类型 | all | 否 |
| `--output` | string | 重构报告输出路径 | refactoring-report.json | 否 |

## 4. AOT 编译指南

### 4.1 AOT 编译优势

- **性能提升**：减少运行时 JIT 编译开销，提高启动速度和执行性能
- **部署简化**：自包含部署，无需目标机器安装 .NET 运行时
- **安全性增强**：减少可攻击面，提高应用安全性
- **体积优化**：通过裁剪未使用代码，减少应用体积

### 4.2 AOT 配置选项

| 选项 | 描述 | 建议值 |
|------|------|--------|
| `PublishAot` | 启用 AOT 编译 | `true` |
| `TrimMode` | 裁剪模式 | `partial` |
| `SelfContained` | 自包含部署 | `true` |
| `PublishSingleFile` | 发布为单文件 | `true` |
| `RuntimeIdentifier` | 运行时标识符 | `win-x64` |

### 4.3 AOT 编译命令

```bash
# 发布分析工具
dotnet publish roslynator_analyzer.csproj -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial

# 发布重构工具
dotnet publish roslynator_refactor.csproj -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial
```

### 4.4 AOT 兼容性考虑

- **反射使用**：AOT 编译会裁剪未使用的代码，使用反射时需要特别注意
- **动态类型**：减少使用 dynamic 类型，可能会影响 AOT 编译效果
- **序列化**：确保序列化/反序列化操作兼容 AOT 编译
- **测试**：在 AOT 编译后进行充分测试，确保功能正常

## 5. 技术实现细节

### 5.1 代码分析实现

代码分析使用 Roslyn 的语法树和语义模型进行深度分析：

1. **语法树解析**：使用 `CSharpSyntaxTree.ParseText()` 解析代码文本
2. **编译单元创建**：使用 `CSharpCompilation.Create()` 创建编译单元
3. **语义模型获取**：使用 `compilation.GetSemanticModel()` 获取语义模型
4. **语法访问**：使用 `CSharpSyntaxWalker` 遍历语法树节点
5. **问题检测**：在访问过程中检测代码质量问题

### 5.2 代码重构实现

代码重构使用 Roslyn 的语法重写器进行代码修改：

1. **语法树解析**：与分析过程相同
2. **语法重写**：使用 `CSharpSyntaxRewriter` 重写语法树节点
3. **代码生成**：生成重构后的代码
4. **文件更新**：保存修改后的代码到文件

### 5.3 重构类型实现

#### 表达式体成员重构

将简单的方法和属性转换为表达式体形式：

```csharp
// 重构前
public int GetValue()
{
    return _value;
}

// 重构后
public int GetValue() => _value;
```

#### 字符串插值重构

将字符串拼接转换为字符串插值：

```csharp
// 重构前
string message = "Hello, " + name + "!";

// 重构后
string message = $"Hello, {name}!";
```

#### 模式匹配重构

使用现代 C# 模式匹配简化代码：

```csharp
// 重构前
if (obj is string && obj != null)
{
    string str = (string)obj;
    Console.WriteLine(str);
}

// 重构后
if (obj is string str)
{
    Console.WriteLine(str);
}
```

## 6. 扩展和定制

### 6.1 自定义分析规则

可以通过继承 `SyntaxAnalyzer` 类来添加自定义分析规则：

```csharp
public class CustomSyntaxAnalyzer : SyntaxAnalyzer
{
    public CustomSyntaxAnalyzer(SemanticModel semanticModel, AnalyzerOptions options)
        : base(semanticModel, options)
    {}
    
    // 重写访问方法，添加自定义规则
    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        base.VisitMethodDeclaration(node);
        
        // 添加自定义规则
        if (node.ParameterList.Parameters.Count > 5)
        {
            Diagnostics.Add(new DiagnosticInfo
            {
                Id = "CUSTOM001",
                Message = "方法参数过多，建议不超过 5 个",
                Severity = "warning",
                FilePath = node.SyntaxTree.FilePath,
                LineNumber = node.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                ColumnNumber = node.Identifier.GetLocation().GetLineSpan().StartLinePosition.Character + 1
            });
        }
    }
}
```

### 6.2 自定义重构类型

可以通过创建新的 `CSharpSyntaxRewriter` 子类来添加自定义重构类型：

```csharp
public class CustomRefactoringRewriter : CSharpSyntaxRewriter
{
    private readonly SemanticModel _semanticModel;
    private readonly List<RefactoringInfo> _refactorings;
    private readonly string _filePath;
    
    public CustomRefactoringRewriter(SemanticModel semanticModel, List<RefactoringInfo> refactorings, string filePath)
    {
        _semanticModel = semanticModel;
        _refactorings = refactorings;
        _filePath = filePath;
    }
    
    // 实现自定义重构逻辑
}
```

### 6.3 服务扩展

可以通过依赖注入容器扩展服务：

```csharp
var services = new ServiceCollection();

// 注册默认服务
services.AddSingleton<ICodeAnalyzer, CodeAnalyzer>();

// 注册自定义服务
services.AddSingleton<ICustomService, CustomService>();

var serviceProvider = services.BuildServiceProvider();
```

## 7. 故障排除

### 7.1 常见错误及解决方案

| 错误 | 原因 | 解决方案 |
|------|------|----------|
| AOT 编译失败 | 使用了不兼容 AOT 的功能 | 检查项目是否使用了反射、动态类型等不兼容 AOT 的功能 |
| 分析结果不准确 | 项目无法正常构建 | 确保项目能够正常构建，检查依赖项是否完整 |
| 重构操作失败 | 代码结构复杂 | 确保代码可以正常编译，检查重构类型是否支持当前代码结构 |
| 性能问题 | 分析大型项目 | 分批处理大型项目，调整分析范围和规则集 |
| 报告生成失败 | 输出路径权限问题 | 确保输出路径有写入权限，检查路径是否存在 |

### 7.2 日志和调试

- **命令行输出**：工具会在命令行输出执行过程和错误信息
- **报告文件**：分析和重构操作会生成详细的报告文件
- **调试模式**：可以通过设置环境变量 `DOTNET_ENVIRONMENT=Development` 启用调试模式

## 8. 性能优化

### 8.1 分析性能优化

- **增量分析**：只分析变更的文件，减少分析范围
- **并行分析**：使用多线程并行分析多个文件
- **缓存机制**：缓存分析结果，避免重复分析
- **规则过滤**：根据需要启用或禁用特定分析规则

### 8.2 重构性能优化

- **增量重构**：只重构变更的文件，减少重构范围
- **批量处理**：批量执行重构操作，减少文件 I/O 次数
- **预览模式**：在执行重构前预览变更，避免不必要的修改

### 8.3 AOT 性能优化

- **代码裁剪**：使用 `TrimMode=partial` 裁剪未使用代码
- **运行时优化**：启用 .NET 运行时优化选项
- **内存管理**：优化内存使用，减少 GC 压力

## 9. 安全考虑

### 9.1 代码安全

- **输入验证**：验证用户输入的文件路径和参数
- **文件访问**：确保只访问授权的文件和目录
- **异常处理**：妥善处理异常，避免信息泄露
- **代码注入**：防止代码注入攻击

### 9.2 部署安全

- **签名验证**：对发布的可执行文件进行数字签名
- **权限设置**：合理设置文件和目录权限
- **漏洞扫描**：定期扫描依赖项漏洞
- **安全更新**：及时更新依赖项和运行时

## 10. 版本兼容性

### 10.1 .NET 版本兼容

| 功能 | .NET 8.0 | .NET 9.0 | .NET 10.0 |
|------|----------|----------|-----------|
| 代码分析 | ✅ | ✅ | ✅ |
| 代码重构 | ✅ | ✅ | ✅ |
| AOT 编译 | ✅ | ✅ | ✅ |
| 单文件发布 | ✅ | ✅ | ✅ |

### 10.2 Roslyn 版本兼容

| Roslyn 版本 | 支持情况 |
|-------------|----------|
| 4.8.0 | ✅ |
| 4.9.0 | ✅ |
| 4.10.0 | ✅ |
| 4.11.0 | ✅ |

### 10.3 操作系统兼容

| 操作系统 | 支持情况 |
|----------|----------|
| Windows 10 (x64) | ✅ |
| Windows 11 (x64) | ✅ |
| Windows Server 2019 | ✅ |
| Windows Server 2022 | ✅ |

## 11. 总结

Roslynator 是一个功能强大的代码分析与重构工具，基于 .NET 10 和 AOT 编译技术，提供了全面的代码质量检查和自动重构功能。通过模块化架构设计和精心的性能优化，它能够高效地处理从单个文件到大型解决方案的各种代码分析和重构任务。

### 11.1 核心优势

- **基于 Roslyn**：利用 Microsoft 的 Roslyn 编译器技术，提供准确的代码分析和重构
- **AOT 编译**：采用 AOT 编译技术，提高性能和简化部署
- **全面功能**：支持多种代码分析规则和重构类型
- **灵活配置**：通过配置文件和命令行参数提供灵活的配置选项
- **详细报告**：生成结构化的分析和重构报告，便于查看和处理

### 11.2 应用场景

- **代码审查**：在代码审查过程中使用，确保代码质量
- **项目重构**：在项目重构过程中使用，自动执行常见重构操作
- **持续集成**：集成到 CI/CD 管道中，自动检查代码质量
- **学习工具**：作为学习 C# 最佳实践的工具，了解代码优化技巧

Roslynator 不仅是一个实用的开发工具，也是学习 Roslyn 编译器 API 和 .NET AOT 编译技术的优秀示例。通过不断扩展和改进，它可以成为 .NET 开发者的得力助手，帮助他们编写更高质量、更高效的代码。
