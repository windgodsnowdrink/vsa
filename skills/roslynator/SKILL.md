# Roslynator 技能文档

## 技能概述

Roslynator 是一个基于 .NET 10 和 AOT 编译的代码分析与重构工具，旨在帮助开发者提高代码质量、优化性能并简化代码重构过程。该工具利用 Roslyn 编译器的强大能力，提供了全面的代码分析和自动重构功能。

### 核心特性

- **代码质量分析**：检测命名规范、空值检查、性能问题等代码质量问题
- **代码自动重构**：支持表达式体成员、字符串插值、模式匹配等代码优化
- **多范围支持**：分析和重构单个文件、整个项目或解决方案
- **详细报告**：生成结构化的分析和重构报告
- **AOT 编译**：采用 AOT 编译技术，提高运行性能和减少部署依赖

### 适用场景

- 代码审查和质量保证
- 项目重构和技术债务清理
- 代码风格统一和规范检查
- 性能优化和最佳实践应用

## 快速开始

### 环境要求

- .NET 10 SDK 或更高版本
- Windows 10/11 (x64)
- Visual Studio 2022 或更高版本（可选）

### 安装和使用

1. **克隆或下载**：获取 Roslynator 技能包
2. **构建项目**：使用 .NET CLI 构建项目
   ```bash
   dotnet build
   ```
3. **发布 AOT**：发布为 AOT 编译的单文件可执行文件
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial
   ```
4. **运行分析**：分析代码质量
   ```bash
   roslynator_analyzer.exe analyze --path "YourProject.csproj" --severity warning --output analysis-report.json
   ```
5. **执行重构**：自动重构代码
   ```bash
   roslynator_refactor.exe refactor --path "YourProject.csproj" --refactoring-type all --output refactoring-report.json
   ```

## 核心功能

### 代码分析

Roslynator 提供了全面的代码质量分析功能，包括但不限于：

- **命名规范检查**：确保类、方法、变量等命名符合 C# 编码规范
- **空值检查**：检测可能的空引用异常
- **性能问题**：识别潜在的性能瓶颈，如字符串拼接、不必要的装箱等
- **代码风格**：检查代码缩进、括号使用、注释规范等
- **最佳实践**：应用 .NET 开发最佳实践

#### 分析范围

- **单文件**：分析单个 C# 源文件
- **项目**：分析整个 .csproj 项目
- **解决方案**：分析整个 .sln 解决方案

#### 分析报告

分析完成后，Roslynator 会生成详细的 JSON 格式报告，包含：
- 问题类型和严重程度
- 问题位置（文件、行号、列号）
- 问题描述和修复建议
- 问题统计信息

### 代码重构

Roslynator 支持多种代码自动重构操作，帮助开发者编写更简洁、更高效的代码：

- **表达式体成员**：将简单方法和属性转换为表达式体形式
- **字符串插值**：将字符串拼接转换为字符串插值
- **模式匹配**：使用现代 C# 模式匹配简化代码
- **空值处理**：优化空值检查逻辑
- **使用声明**：将显式 Dispose 调用转换为 using 声明
- **异步优化**：优化异步代码结构

#### 重构类型

| 重构类型 | 描述 |
|---------|------|
| all | 执行所有支持的重构 |
| expression-bodied | 表达式体成员优化 |
| string-interpolation | 字符串插值优化 |
| pattern-matching | 模式匹配优化 |
| null-check | 空值检查优化 |
| using-declaration | using 声明优化 |
| async-optimization | 异步代码优化 |

#### 重构报告

重构完成后，Roslynator 会生成详细的 JSON 格式报告，包含：
- 执行的重构操作
- 重构位置（文件、行号、列号）
- 重构前后的代码对比
- 重构统计信息

## 命令行接口

### 分析命令

```bash
roslynator_analyzer.exe analyze [选项]
```

#### 选项

| 选项 | 描述 | 默认值 |
|------|------|--------|
| --path | 要分析的文件、项目或解决方案路径 | 无（必需） |
| --severity | 分析结果的严重程度过滤 | warning |
| --output | 分析报告输出路径 | analysis-report.json |
| --help | 显示帮助信息 | 无 |

### 重构命令

```bash
roslynator_refactor.exe refactor [选项]
```

#### 选项

| 选项 | 描述 | 默认值 |
|------|------|--------|
| --path | 要重构的文件、项目或解决方案路径 | 无（必需） |
| --refactoring-type | 重构类型 | all |
| --output | 重构报告输出路径 | refactoring-report.json |
| --help | 显示帮助信息 | 无 |

## API 参考

### 核心接口

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

### 数据结构

#### AnalysisResult

包含代码分析结果，包括问题列表、统计信息等。

#### RefactoringResult

包含代码重构结果，包括执行的重构操作、代码变更等。

#### AnalyzerOptions

分析选项，包括严重程度过滤、规则配置等。

#### RefactoringOptions

重构选项，包括重构类型、是否预览等。

## AOT 编译指南

### AOT 编译优势

- **性能提升**：减少运行时 JIT 编译开销，提高启动速度和执行性能
- **部署简化**：自包含部署，无需目标机器安装 .NET 运行时
- **安全性增强**：减少可攻击面，提高应用安全性
- **体积优化**：通过裁剪未使用代码，减少应用体积

### AOT 配置

在项目文件中配置 AOT 编译选项：

```xml
<PropertyGroup>
  <TargetFramework>net11.0</TargetFramework>
  <PublishAot>true</PublishAot>
  <TrimMode>partial</TrimMode>
  <SelfContained>true</SelfContained>
  <PublishSingleFile>true</PublishSingleFile>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
</PropertyGroup>
```

### AOT 注意事项

1. **反射使用**：AOT 编译会裁剪未使用的代码，使用反射时需要特别注意
2. **动态类型**：减少使用 dynamic 类型，可能会影响 AOT 编译效果
3. **序列化**：确保序列化/反序列化操作兼容 AOT 编译
4. **测试**：在 AOT 编译后进行充分测试，确保功能正常

## 示例用法

### 示例 1：分析单个文件

```bash
roslynator_analyzer.exe analyze --path "Program.cs" --severity error --output analysis-report.json
```

### 示例 2：分析整个项目

```bash
roslynator_analyzer.exe analyze --path "MyProject.csproj" --severity warning --output project-analysis.json
```

### 示例 3：执行特定类型重构

```bash
roslynator_refactor.exe refactor --path "Program.cs" --refactoring-type expression-bodied --output refactoring-report.json
```

### 示例 4：重构整个解决方案

```bash
roslynator_refactor.exe refactor --path "MySolution.sln" --refactoring-type all --output solution-refactoring.json
```

## 常见问题

### 1. AOT 编译失败

**问题**：发布 AOT 时出现编译错误

**解决方案**：
- 检查项目是否使用了不兼容 AOT 的功能
- 确保所有依赖项支持 AOT 编译
- 调整 TrimMode 为 partial 或 copyused

### 2. 分析结果不准确

**问题**：代码分析结果与预期不符

**解决方案**：
- 检查分析选项和严重程度设置
- 确保项目能够正常构建
- 验证 Roslyn 版本兼容性

### 3. 重构操作失败

**问题**：重构操作无法执行或产生错误

**解决方案**：
- 确保代码可以正常编译
- 检查重构类型是否支持当前代码结构
- 备份原始代码后再执行重构

### 4. 性能问题

**问题**：分析或重构大型项目时性能较慢

**解决方案**：
- 分批处理大型项目
- 调整分析范围和规则集
- 使用增量分析，只分析变更的文件

## 高级配置

### 自定义规则

可以通过配置文件自定义分析规则和严重程度：

```json
{
  "rules": {
    "naming": {
      "severity": "warning",
      "enabled": true
    },
    "null-check": {
      "severity": "error",
      "enabled": true
    }
  }
}
```

### 集成到 CI/CD

Roslynator 可以集成到 CI/CD 管道中，自动执行代码分析和重构：

#### GitHub Actions 示例

```yaml
name: Code Quality

on: [push, pull_request]

jobs:
  analyze:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      - name: Build
        run: dotnet build --configuration Release
      - name: Analyze
        run: roslynator_analyzer.exe analyze --path "YourProject.csproj" --severity warning --output analysis-report.json
      - name: Upload report
        uses: actions/upload-artifact@v3
        with:
          name: analysis-report
          path: analysis-report.json
```

#### Azure DevOps 示例

```yaml
trigger:
- main

pool:
  vmImage: 'windows-latest'

steps:
- task: UseDotNet@2
  inputs:
    packageType: 'sdk'
    version: '10.0.x'

- task: DotNetCoreCLI@2
  inputs:
    command: 'build'
    arguments: '--configuration Release'

- script: |
    roslynator_analyzer.exe analyze --path "YourProject.csproj" --severity warning --output analysis-report.json
  displayName: 'Run Code Analysis'

- task: PublishBuildArtifacts@1
  inputs:
    PathtoPublish: 'analysis-report.json'
    ArtifactName: 'analysis-report'
```

## 贡献指南

我们欢迎社区贡献，包括但不限于：

- **功能请求**：提出新功能或改进建议
- **错误报告**：报告使用过程中遇到的错误
- **代码贡献**：提交代码修复或功能实现
- **文档改进**：改进文档质量和完整性

### 开发环境设置

1. **克隆仓库**：`git clone https://github.com/your-repo/roslynator.git`
2. **安装依赖**：`dotnet restore`
3. **构建项目**：`dotnet build`
4. **运行测试**：`dotnet test`

### 提交代码

1. **创建分支**：`git checkout -b feature/your-feature`
2. **提交更改**：`git commit -m "Add your feature"`
3. **推送分支**：`git push origin feature/your-feature`
4. **创建 PR**：在 GitHub 上创建 Pull Request

## 许可证

Roslynator 技能使用 MIT 许可证，详情请参阅 LICENSE 文件。

## 联系方式

- **作者**：大佬
- **邮箱**：your-email@example.com
- **GitHub**：https://github.com/your-repo/roslynator

---

**版本历史**

- v1.0.0 (2026-01-24)：初始版本，支持代码分析和重构功能，基于 .NET 10 和 AOT 编译
