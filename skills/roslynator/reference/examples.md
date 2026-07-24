# Roslynator 使用示例

## 1. 命令行使用示例

### 1.1 分析命令示例

#### 1.1.1 分析单个文件

```bash
# 分析单个 C# 文件，严重程度设为 warning，输出报告到 analysis-report.json
roslynator_analyzer.exe analyze --path "Program.cs" --severity warning --output analysis-report.json
```

**示例输出**：

```
开始分析: Program.cs
严重程度过滤: warning
分析完成! 报告已保存到: analysis-report.json
找到 5 个问题
```

#### 1.1.2 分析整个项目

```bash
# 分析整个项目，严重程度设为 error，输出报告到 project-analysis.json
roslynator_analyzer.exe analyze --path "MyProject.csproj" --severity error --output project-analysis.json
```

**示例输出**：

```
开始分析: MyProject.csproj
严重程度过滤: error
分析完成! 报告已保存到: project-analysis.json
找到 2 个问题
```

#### 1.1.3 分析整个解决方案

```bash
# 分析整个解决方案，使用默认严重程度和输出路径
roslynator_analyzer.exe analyze --path "MySolution.sln"
```

**示例输出**：

```
开始分析: MySolution.sln
严重程度过滤: warning
分析完成! 报告已保存到: analysis-report.json
找到 15 个问题
```

### 1.2 重构命令示例

#### 1.2.1 重构单个文件

```bash
# 重构单个 C# 文件，执行所有重构类型，输出报告到 refactoring-report.json
roslynator_refactor.exe refactor --path "Program.cs" --refactoring-type all --output refactoring-report.json
```

**示例输出**：

```
开始重构: Program.cs
重构类型: all
重构完成! 报告已保存到: refactoring-report.json
执行了 8 个重构操作
```

#### 1.2.2 重构特定类型

```bash
# 重构单个 C# 文件，只执行表达式体成员重构
roslynator_refactor.exe refactor --path "Program.cs" --refactoring-type expression-bodied --output expression-bodied-report.json
```

**示例输出**：

```
开始重构: Program.cs
重构类型: expression-bodied
重构完成! 报告已保存到: expression-bodied-report.json
执行了 3 个重构操作
```

#### 1.2.3 重构整个项目

```bash
# 重构整个项目，只执行字符串插值重构
roslynator_refactor.exe refactor --path "MyProject.csproj" --refactoring-type string-interpolation --output string-interpolation-report.json
```

**示例输出**：

```
开始重构: MyProject.csproj
重构类型: string-interpolation
重构完成! 报告已保存到: string-interpolation-report.json
执行了 12 个重构操作
```

## 2. 编程接口使用示例

### 2.1 分析接口使用示例

#### 2.1.1 基本分析示例

```csharp
using Roslynator.Analyzer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // 配置依赖注入
        var serviceProvider = new ServiceCollection()
            .AddSingleton<ICodeAnalyzer, CodeAnalyzer>()
            .AddSingleton<IAnalysisEngine, AnalysisEngine>()
            .AddSingleton<IProjectLoader, ProjectLoader>()
            .BuildServiceProvider();
        
        // 获取分析服务
        var analyzer = serviceProvider.GetRequiredService<ICodeAnalyzer>();
        
        // 配置分析选项
        var options = new AnalyzerOptions { Severity = "warning" };
        
        // 执行分析
        var result = await analyzer.AnalyzeAsync(
            "MyProject.csproj",
            options,
            CancellationToken.None
        );
        
        // 处理分析结果
        Console.WriteLine($"分析完成，找到 {result.Diagnostics.Count} 个问题");
        foreach (var diagnostic in result.Diagnostics)
        {
            Console.WriteLine($"[{diagnostic.Severity}] {diagnostic.FilePath}:{diagnostic.LineNumber} - {diagnostic.Message}");
        }
    }
}
```

#### 2.1.2 自定义分析规则示例

```csharp
using Roslynator.Analyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

public class CustomSyntaxAnalyzer : SyntaxAnalyzer
{
    public CustomSyntaxAnalyzer(SemanticModel semanticModel, AnalyzerOptions options)
        : base(semanticModel, options)
    {}
    
    // 重写方法声明访问器，添加自定义规则
    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        base.VisitMethodDeclaration(node);
        
        // 检查方法参数数量
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
        
        // 检查方法长度
        var body = node.Body;
        if (body != null)
        {
            var lineCount = body.GetText().Lines.Count;
            if (lineCount > 50)
            {
                Diagnostics.Add(new DiagnosticInfo
                {
                    Id = "CUSTOM002",
                    Message = "方法过长，建议拆分为多个小方法",
                    Severity = "warning",
                    FilePath = node.SyntaxTree.FilePath,
                    LineNumber = node.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    ColumnNumber = node.Identifier.GetLocation().GetLineSpan().StartLinePosition.Character + 1
                });
            }
        }
    }
}

// 使用自定义分析器
public class CustomAnalysisEngine : AnalysisEngine
{
    public override async Task<List<DiagnosticInfo>> AnalyzeFileAsync(string filePath, AnalyzerOptions options, CancellationToken cancellationToken)
    {
        var diagnostics = new List<DiagnosticInfo>();
        
        try
        {
            // 读取文件内容
            var code = await File.ReadAllTextAsync(filePath, cancellationToken);
            
            // 解析语法树
            var syntaxTree = CSharpSyntaxTree.ParseText(code, path: filePath);
            
            // 创建编译单元
            var compilation = CSharpCompilation.Create(
                Path.GetFileNameWithoutExtension(filePath),
                syntaxTrees: new[] { syntaxTree },
                references: GetMetadataReferences(),
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );
            
            // 获取语义模型
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            
            // 使用自定义分析器
            var syntaxAnalyzer = new CustomSyntaxAnalyzer(semanticModel, options);
            var root = await syntaxTree.GetRootAsync(cancellationToken);
            syntaxAnalyzer.Visit(root);
            
            diagnostics.AddRange(syntaxAnalyzer.Diagnostics);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"分析文件 {filePath} 时发生错误: {ex.Message}");
        }
        
        return diagnostics;
    }
}
```

### 2.2 重构接口使用示例

#### 2.2.1 基本重构示例

```csharp
using Roslynator.Refactor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // 配置依赖注入
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IRefactoringProvider, RefactoringProvider>()
            .AddSingleton<IRefactoringEngine, RefactoringEngine>()
            .AddSingleton<IProjectLoader, ProjectLoader>()
            .BuildServiceProvider();
        
        // 获取重构服务
        var refactoringProvider = serviceProvider.GetRequiredService<IRefactoringProvider>();
        
        // 配置重构选项
        var options = new RefactoringOptions { RefactoringType = "all" };
        
        // 执行重构
        var result = await refactoringProvider.RefactorAsync(
            "Program.cs",
            options,
            CancellationToken.None
        );
        
        // 处理重构结果
        Console.WriteLine($"重构完成，执行了 {result.Refactorings.Count} 个重构操作");
        foreach (var refactoring in result.Refactorings)
        {
            Console.WriteLine($"[{refactoring.RefactoringType}] {refactoring.FilePath}:{refactoring.LineNumber} - {refactoring.Description}");
        }
    }
}
```

#### 2.2.2 自定义重构类型示例

```csharp
using Roslynator.Refactor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

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
    
    // 重写变量声明，添加 readonly 修饰符
    public override SyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node)
    {
        // 检查是否可以添加 readonly 修饰符
        if (CanAddReadonly(node))
        {
            var oldCode = node.ToString();
            var newNode = node.WithModifiers(
                node.Modifiers.Add(SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword))
            );
            var newCode = newNode.ToString();
            
            // 添加重构信息
            _refactorings.Add(new RefactoringInfo
            {
                RefactoringType = "readonly-modifier",
                Description = "为变量添加 readonly 修饰符",
                FilePath = _filePath,
                LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                ColumnNumber = node.GetLocation().GetLineSpan().StartLinePosition.Character + 1,
                OldCode = oldCode,
                NewCode = newCode
            });
            
            return newNode;
        }
        
        return base.VisitVariableDeclaration(node);
    }
    
    private bool CanAddReadonly(VariableDeclarationSyntax node)
    {
        // 检查变量是否已经是 readonly
        if (node.Modifiers.Any(m => m.IsKind(SyntaxKind.ReadOnlyKeyword)))
            return false;
        
        // 检查变量是否在声明后被修改
        // 这里简化处理，实际需要更复杂的分析
        return true;
    }
}

// 使用自定义重写器
public class CustomRefactoringEngine : RefactoringEngine
{
    public override async Task<List<RefactoringInfo>> RefactorFileAsync(string filePath, RefactoringOptions options, CancellationToken cancellationToken)
    {
        var refactorings = new List<RefactoringInfo>();
        
        try
        {
            // 读取文件内容
            var code = await File.ReadAllTextAsync(filePath, cancellationToken);
            
            // 解析语法树
            var syntaxTree = CSharpSyntaxTree.ParseText(code, path: filePath);
            
            // 创建编译单元
            var compilation = CSharpCompilation.Create(
                Path.GetFileNameWithoutExtension(filePath),
                syntaxTrees: new[] { syntaxTree },
                references: GetMetadataReferences(),
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );
            
            // 获取语义模型
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            
            // 执行自定义重构
            var root = await syntaxTree.GetRootAsync(cancellationToken);
            var refactoredRoot = root;
            
            if (options.RefactoringType == "all" || options.RefactoringType == "readonly-modifier")
            {
                var readonlyRewriter = new CustomRefactoringRewriter(semanticModel, refactorings, filePath);
                refactoredRoot = readonlyRewriter.Visit(refactoredRoot);
            }
            
            // 如果有重构操作，保存修改后的文件
            if (!refactoredRoot.IsEquivalentTo(root))
            {
                var newCode = refactoredRoot.ToFullString();
                await File.WriteAllTextAsync(filePath, newCode, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"重构文件 {filePath} 时发生错误: {ex.Message}");
        }
        
        return refactorings;
    }
}
```

## 3. 配置文件示例

### 3.1 .setting.json 配置示例

#### 3.1.1 分析工具配置

```json
{
  "compilationOptions": {
    "targetFramework": "net11.0",
    "langVersion": "preview",
    "publishAot": true,
    "trimMode": "partial",
    "selfContained": true,
    "publishSingleFile": true,
    "runtimeIdentifier": "win-x64",
    "nullable": "enable",
    "implicitUsings": "enable"
  },
  "dependencies": {
    "Microsoft.CodeAnalysis.CSharp": "4.10.0",
    "Microsoft.CodeAnalysis.Common": "4.10.0",
    "System.CommandLine": "2.0.0",
    "Microsoft.Extensions.DependencyInjection": "8.0.0"
  },
  "sdk": "Microsoft.NET.Sdk"
}
```

#### 3.1.2 重构工具配置

```json
{
  "compilationOptions": {
    "targetFramework": "net11.0",
    "langVersion": "preview",
    "publishAot": true,
    "trimMode": "partial",
    "selfContained": true,
    "publishSingleFile": true,
    "runtimeIdentifier": "win-x64",
    "nullable": "enable",
    "implicitUsings": "enable"
  },
  "dependencies": {
    "Microsoft.CodeAnalysis.CSharp": "4.10.0",
    "Microsoft.CodeAnalysis.Common": "4.10.0",
    "System.CommandLine": "2.0.0",
    "Microsoft.Extensions.DependencyInjection": "8.0.0"
  },
  "sdk": "Microsoft.NET.Sdk"
}
```

### 3.2 .run.json 配置示例

#### 3.2.1 分析工具运行配置

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
    "DOTNET_ENVIRONMENT": "Production",
    "DOTNET_CLI_TELEMETRY_OPTOUT": "1"
  },
  "workingDirectory": "${workspaceFolder}",
  "problemMatcher": [],
  "label": "Run Roslynator Analyzer"
}
```

#### 3.2.2 重构工具运行配置

```json
{
  "command": "roslynator_refactor.exe",
  "args": [
    "refactor",
    "--path", "${workspaceFolder}",
    "--refactoring-type", "all",
    "--output", "refactoring-report.json"
  ],
  "env": {
    "DOTNET_ENVIRONMENT": "Production",
    "DOTNET_CLI_TELEMETRY_OPTOUT": "1"
  },
  "workingDirectory": "${workspaceFolder}",
  "problemMatcher": [],
  "label": "Run Roslynator Refactor"
}
```

### 3.3 index.yaml 配置示例

```yaml
# Roslynator 技能配置文件
# 基于 .NET 10 和 AOT 架构的代码分析与重构工具

name: Roslynator
version: 1.0.0
author: 大佬
description: 基于 .NET 10 和 AOT 编译的代码分析与重构工具，支持代码质量检查、性能优化和代码重构
language: zh-CN

# 依赖项配置
dependencies:
  - name: Microsoft.CodeAnalysis.CSharp
    version: 4.10.0
    description: C# 代码分析库
  - name: Microsoft.CodeAnalysis.Common
    version: 4.10.0
    description: 代码分析通用库
  - name: System.CommandLine
    version: 2.0.0
    description: 命令行参数解析库
  - name: Microsoft.Extensions.DependencyInjection
    version: 8.0.0
    description: 依赖注入库

# 功能配置
features:
  - name: code-analysis
    description: 代码质量分析，支持检测命名规范、空值检查、性能问题等
    commands:
      - name: analyze
        description: 分析代码质量
        parameters:
          - name: path
            type: string
            description: 要分析的文件、项目或解决方案路径
          - name: severity
            type: string
            description: 分析结果的严重程度过滤
            default: warning
          - name: output
            type: string
            description: 分析报告输出路径
            default: analysis-report.json

  - name: code-refactoring
    description: 代码自动重构，支持表达式体成员、字符串插值、模式匹配等优化
    commands:
      - name: refactor
        description: 执行代码重构
        parameters:
          - name: path
            type: string
            description: 要重构的文件、项目或解决方案路径
          - name: refactoring-type
            type: string
            description: 重构类型
            default: all
          - name: output
            type: string
            description: 重构报告输出路径
            default: refactoring-report.json

# AOT 编译配置
aot:
  enabled: true
  options:
    - name: PublishAot
      value: true
      description: 启用 AOT 编译
    - name: TrimMode
      value: partial
      description: 裁剪模式
    - name: SelfContained
      value: true
      description: 自包含部署
    - name: PublishSingleFile
      value: true
      description: 发布为单文件
    - name: RuntimeIdentifier
      value: win-x64
      description: 运行时标识符
```

## 4. CI/CD 集成示例

### 4.1 GitHub Actions 集成

#### 4.1.1 代码分析工作流

```yaml
name: Code Quality Analysis

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  analyze:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      
      - name: Build project
        run: dotnet build --configuration Release
      
      - name: Download Roslynator Analyzer
        run: |
          Invoke-WebRequest -Uri "https://github.com/your-repo/roslynator/releases/latest/download/roslynator_analyzer.exe" -OutFile "roslynator_analyzer.exe"
          chmod +x roslynator_analyzer.exe
      
      - name: Run code analysis
        run: ./roslynator_analyzer.exe analyze --path "${{ github.workspace }}" --severity warning --output analysis-report.json
      
      - name: Upload analysis report
        uses: actions/upload-artifact@v3
        with:
          name: analysis-report
          path: analysis-report.json
      
      - name: Check for errors
        run: |
          $report = Get-Content analysis-report.json | ConvertFrom-Json
          $errorCount = ($report.Diagnostics | Where-Object { $_.Severity -eq "error" }).Count
          if ($errorCount -gt 0) {
            Write-Error "Found $errorCount errors during analysis"
            exit 1
          }
```

#### 4.1.2 代码重构工作流

```yaml
name: Code Refactoring

on:
  schedule:
    - cron: '0 0 * * 0'  # 每周日运行

jobs:
  refactor:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
        with:
          fetch-depth: 0
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      
      - name: Download Roslynator Refactor
        run: |
          Invoke-WebRequest -Uri "https://github.com/your-repo/roslynator/releases/latest/download/roslynator_refactor.exe" -OutFile "roslynator_refactor.exe"
          chmod +x roslynator_refactor.exe
      
      - name: Run code refactoring
        run: ./roslynator_refactor.exe refactor --path "${{ github.workspace }}" --refactoring-type all --output refactoring-report.json
      
      - name: Upload refactoring report
        uses: actions/upload-artifact@v3
        with:
          name: refactoring-report
          path: refactoring-report.json
      
      - name: Commit refactored code
        run: |
          git config --global user.name "github-actions[bot]"
          git config --global user.email "github-actions[bot]@users.noreply.github.com"
          git add .
          git commit -m "Auto-refactor code using Roslynator" || echo "No changes to commit"
          git push
```

### 4.2 Azure DevOps 集成

#### 4.2.1 代码分析管道

```yaml
trigger:
- main
- develop

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
    Invoke-WebRequest -Uri "https://github.com/your-repo/roslynator/releases/latest/download/roslynator_analyzer.exe" -OutFile "roslynator_analyzer.exe"
  displayName: 'Download Roslynator Analyzer'

- script: |
    .\roslynator_analyzer.exe analyze --path "$(Build.SourcesDirectory)" --severity warning --output analysis-report.json
  displayName: 'Run Code Analysis'

- task: PublishBuildArtifacts@1
  inputs:
    PathtoPublish: 'analysis-report.json'
    ArtifactName: 'analysis-report'

- script: |
    $report = Get-Content analysis-report.json | ConvertFrom-Json
    $errorCount = ($report.Diagnostics | Where-Object { $_.Severity -eq "error" }).Count
    if ($errorCount -gt 0) {
      Write-Error "Found $errorCount errors during analysis"
      exit 1
    }
  displayName: 'Check for Errors'
```

#### 4.2.2 代码重构管道

```yaml
schedules:
- cron: "0 0 * * 0"  # 每周日运行
  displayName: Weekly refactoring
  branches:
    include:
    - develop

pool:
  vmImage: 'windows-latest'

steps:
- task: UseDotNet@2
  inputs:
    packageType: 'sdk'
    version: '10.0.x'

- script: |
    Invoke-WebRequest -Uri "https://github.com/your-repo/roslynator/releases/latest/download/roslynator_refactor.exe" -OutFile "roslynator_refactor.exe"
  displayName: 'Download Roslynator Refactor'

- script: |
    .\roslynator_refactor.exe refactor --path "$(Build.SourcesDirectory)" --refactoring-type all --output refactoring-report.json
  displayName: 'Run Code Refactoring'

- task: PublishBuildArtifacts@1
  inputs:
    PathtoPublish: 'refactoring-report.json'
    ArtifactName: 'refactoring-report'

- task: GitToolsGitCommit@1
  inputs:
    commitMessage: 'Auto-refactor code using Roslynator'
    workingDirectory: '$(Build.SourcesDirectory)'

- task: GitToolsGitPush@1
  inputs:
    workingDirectory: '$(Build.SourcesDirectory)'
    remoteName: 'origin'
    branchName: 'develop'
```

## 5. 实际应用示例

### 5.1 示例 1：分析控制台应用程序

#### 5.1.1 目标文件

**Program.cs**：

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var names = new List<string> { "Alice", "Bob", "Charlie" };
            Console.WriteLine("Hello, " + names[0] + "!");
            
            foreach (var name in names)
            {
                Console.WriteLine(GetGreeting(name));
            }
        }
        
        static string GetGreeting(string name)
        {
            if (name == null)
            {
                return "Hello, Guest!";
            }
            else
            {
                return "Hello, " + name + "!";
            }
        }
    }
}
```

#### 5.1.2 分析命令

```bash
roslynator_analyzer.exe analyze --path "Program.cs" --severity warning --output analysis-report.json
```

#### 5.1.3 分析结果

**analysis-report.json**：

```json
{
  "Path": "Program.cs",
  "StartTime": "2026-01-24T10:00:00",
  "EndTime": "2026-01-24T10:00:01",
  "Duration": 1.0,
  "Diagnostics": [
    {
      "Id": "PERF001",
      "Message": "使用字符串拼接可能影响性能，建议使用 StringBuilder 或字符串插值",
      "Severity": "info",
      "FilePath": "Program.cs",
      "LineNumber": 12,
      "ColumnNumber": 21
    },
    {
      "Id": "PERF001",
      "Message": "使用字符串拼接可能影响性能，建议使用 StringBuilder 或字符串插值",
      "Severity": "info",
      "FilePath": "Program.cs",
      "LineNumber": 24,
      "ColumnNumber": 24
    },
    {
      "Id": "NAMING004",
      "Message": "变量名 'args' 不符合 camelCase 命名规范",
      "Severity": "warning",
      "FilePath": "Program.cs",
      "LineNumber": 10,
      "ColumnNumber": 20
    }
  ],
  "Summary": {
    "TotalIssues": 3,
    "IssuesBySeverity": {
      "info": 2,
      "warning": 1
    }
  }
}
```

#### 5.1.4 重构命令

```bash
roslynator_refactor.exe refactor --path "Program.cs" --refactoring-type all --output refactoring-report.json
```

#### 5.1.5 重构结果

**refactoring-report.json**：

```json
{
  "Path": "Program.cs",
  "StartTime": "2026-01-24T10:05:00",
  "EndTime": "2026-01-24T10:05:01",
  "Duration": 1.0,
  "Refactorings": [
    {
      "RefactoringType": "string-interpolation",
      "Description": "将字符串拼接转换为字符串插值",
      "FilePath": "Program.cs",
      "LineNumber": 12,
      "ColumnNumber": 21,
      "OldCode": "Console.WriteLine(\"Hello, \" + names[0] + \"!\");",
      "NewCode": "Console.WriteLine($\"Hello, {names[0]}!\");"
    },
    {
      "RefactoringType": "expression-bodied",
      "Description": "将方法 'GetGreeting' 转换为表达式体成员",
      "FilePath": "Program.cs",
      "LineNumber": 18,
      "ColumnNumber": 17,
      "OldCode": "static string GetGreeting(string name)\n        {\n            if (name == null)\n            {\n                return \"Hello, Guest!\";\n            }\n            else\n            {\n                return \"Hello, \" + name + \"!\";\n            }\n        }",
      "NewCode": "static string GetGreeting(string name) => name == null ? \"Hello, Guest!\" : $\"Hello, {name}!\";"
    },
    {
      "RefactoringType": "string-interpolation",
      "Description": "将字符串拼接转换为字符串插值",
      "FilePath": "Program.cs",
      "LineNumber": 24,
      "ColumnNumber": 24,
      "OldCode": "return \"Hello, \" + name + \"!\";",
      "NewCode": "return $\"Hello, {name}!\";"
    }
  ],
  "Summary": {
    "TotalRefactorings": 3,
    "RefactoringsByType": {
      "string-interpolation": 2,
      "expression-bodied": 1
    }
  }
}
```

#### 5.1.6 重构后文件

**Program.cs**：

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var names = new List<string> { "Alice", "Bob", "Charlie" };
            Console.WriteLine($"Hello, {names[0]}!");
            
            foreach (var name in names)
            {
                Console.WriteLine(GetGreeting(name));
            }
        }
        
        static string GetGreeting(string name) => name == null ? "Hello, Guest!" : $"Hello, {name}!";
    }
}
```

### 5.2 示例 2：重构 ASP.NET Core 控制器

#### 5.2.1 目标文件

**WeatherForecastController.cs**：

```csharp
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            var forecasts = new List<WeatherForecast>();
            
            for (int i = 0; i < 5; i++)
            {
                var forecast = new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(i),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                };
                
                forecasts.Add(forecast);
            }
            
            return forecasts;
        }
    }

    public class WeatherForecast
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        public string Summary { get; set; }
    }
}
```

#### 5.2.2 重构命令

```bash
roslynator_refactor.exe refactor --path "WeatherForecastController.cs" --refactoring-type all --output refactoring-report.json
```

#### 5.2.3 重构结果

**refactoring-report.json**：

```json
{
  "Path": "WeatherForecastController.cs",
  "StartTime": "2026-01-24T10:10:00",
  "EndTime": "2026-01-24T10:10:01",
  "Duration": 1.0,
  "Refactorings": [
    {
      "RefactoringType": "expression-bodied",
      "Description": "将方法 'Get' 转换为表达式体成员",
      "FilePath": "WeatherForecastController.cs",
      "LineNumber": 18,
      "ColumnNumber": 26,
      "OldCode": "public IEnumerable<WeatherForecast> Get()\n        {\n            var rng = new Random();\n            var forecasts = new List<WeatherForecast>();\n            \n            for (int i = 0; i < 5; i++)\n            {\n                var forecast = new WeatherForecast\n                {\n                    Date = DateTime.Now.AddDays(i),\n                    TemperatureC = rng.Next(-20, 55),\n                    Summary = Summaries[rng.Next(Summaries.Length)]\n                };\n                \n                forecasts.Add(forecast);\n            }\n            \n            return forecasts;\n        }",
      "NewCode": "public IEnumerable<WeatherForecast> Get()\n        {\n            var rng = new Random();\n            return Enumerable.Range(0, 5).Select(i => new WeatherForecast\n            {\n                Date = DateTime.Now.AddDays(i),\n                TemperatureC = rng.Next(-20, 55),\n                Summary = Summaries[rng.Next(Summaries.Length)]\n            });\n        }"
    }
  ],
  "Summary": {
    "TotalRefactorings": 1,
    "RefactoringsByType": {
      "expression-bodied": 1
    }
  }
}
```

#### 5.2.4 重构后文件

**WeatherForecastController.cs**：

```csharp
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(0, 5).Select(i => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(i),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }
    }

    public class WeatherForecast
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        public string Summary { get; set; }
    }
}
```

## 6. 高级使用示例

### 6.1 自定义分析规则

#### 6.1.1 目标

创建一个自定义分析规则，检测方法参数数量是否超过 5 个。

#### 6.1.2 实现

**CustomAnalyzer.cs**：

```csharp
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

public class CustomAnalyzer : CSharpSyntaxWalker
{
    public List<DiagnosticInfo> Diagnostics { get; } = new List<DiagnosticInfo>();
    private readonly SemanticModel _semanticModel;
    
    public CustomAnalyzer(SemanticModel semanticModel)
    {
        _semanticModel = semanticModel;
    }
    
    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        base.VisitMethodDeclaration(node);
        
        // 检查方法参数数量
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

#### 6.1.3 使用

```csharp
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var code = await File.ReadAllTextAsync("Program.cs");
        var syntaxTree = CSharpSyntaxTree.ParseText(code, path: "Program.cs");
        
        var compilation = CSharpCompilation.Create(
            "Test",
            syntaxTrees: new[] { syntaxTree },
            references: new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location)
            },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var analyzer = new CustomAnalyzer(semanticModel);
        var root = await syntaxTree.GetRootAsync();
        analyzer.Visit(root);
        
        foreach (var diagnostic in analyzer.Diagnostics)
        {
            Console.WriteLine($"[{diagnostic.Severity}] {diagnostic.FilePath}:{diagnostic.LineNumber} - {diagnostic.Message}");
        }
    }
}
```

### 6.2 自定义重构规则

#### 6.2.1 目标

创建一个自定义重构规则，将 `List<T>.AddRange()` 调用转换为集合初始化器。

#### 6.2.2 实现

**CustomRefactoringRewriter.cs**：

```csharp
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

public class CustomRefactoringRewriter : CSharpSyntaxRewriter
{
    public List<RefactoringInfo> Refactorings { get; } = new List<RefactoringInfo>();
    private readonly SemanticModel _semanticModel;
    private readonly string _filePath;
    
    public CustomRefactoringRewriter(SemanticModel semanticModel, string filePath)
    {
        _semanticModel = semanticModel;
        _filePath = filePath;
    }
    
    public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
    {
        var newNode = (ObjectCreationExpressionSyntax)base.VisitObjectCreationExpression(node);
        
        // 检查是否是 List<T> 创建
        var typeInfo = _semanticModel.GetTypeInfo(node.Type);
        if (typeInfo.Type != null && typeInfo.Type.Name == "List" && typeInfo.Type.ContainingNamespace.Name == "System.Collections.Generic")
        {
            // 查找后续的 AddRange 调用
            var parent = node.Parent;
            if (parent is VariableDeclarationSyntax variableDecl && variableDecl.Variables.Count == 1)
            {
                var variableName = variableDecl.Variables[0].Identifier.Text;
                var nextStatement = GetNextStatement(variableDecl);
                
                if (nextStatement is ExpressionStatementSyntax exprStmt && exprStmt.Expression is InvocationExpressionSyntax invocation)
                {
                    if (invocation.Expression is MemberAccessExpressionSyntax memberAccess && 
                        memberAccess.Name.Identifier.Text == "AddRange" &&
                        memberAccess.Expression is IdentifierNameSyntax idName &&
                        idName.Identifier.Text == variableName)
                    {
                        // 提取 AddRange 的参数
                        var argument = invocation.ArgumentList.Arguments[0].Expression;
                        
                        // 创建集合初始化器
                        var initializer = SyntaxFactory.InitializerExpression(
                            SyntaxKind.CollectionInitializerExpression,
                            SyntaxFactory.SeparatedList<ExpressionSyntax>(new[] { argument })
                        );
                        
                        // 创建新的对象创建表达式
                        var updatedNode = newNode.WithInitializer(initializer);
                        
                        // 记录重构信息
                        Refactorings.Add(new RefactoringInfo
                        {
                            RefactoringType = "collection-initializer",
                            Description = "将 List<T>.AddRange() 转换为集合初始化器",
                            FilePath = _filePath,
                            LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                            ColumnNumber = node.GetLocation().GetLineSpan().StartLinePosition.Character + 1,
                            OldCode = variableDecl.ToString() + "\n" + nextStatement.ToString(),
                            NewCode = variableDecl.WithDeclaration(variableDecl.Declaration.WithVariables(
                                SyntaxFactory.SeparatedList<VariableDeclaratorSyntax>(new[] {
                                    variableDecl.Declaration.Variables[0].WithInitializer(
                                        SyntaxFactory.EqualsValueClause(updatedNode)
                                    )
                                })
                            )).ToString()
                        });
                        
                        return updatedNode;
                    }
                }
            }
        }
        
        return newNode;
    }
    
    private StatementSyntax GetNextStatement(SyntaxNode node)
    {
        var parent = node.Parent;
        if (parent is BlockSyntax block)
        {
            var index = block.Statements.IndexOf(node as StatementSyntax);
            if (index >= 0 && index < block.Statements.Count - 1)
            {
                return block.Statements[index + 1];
            }
        }
        return null;
    }
}

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

#### 6.2.3 使用

```csharp
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var code = await File.ReadAllTextAsync("Program.cs");
        var syntaxTree = CSharpSyntaxTree.ParseText(code, path: "Program.cs");
        
        var compilation = CSharpCompilation.Create(
            "Test",
            syntaxTrees: new[] { syntaxTree },
            references: new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Collections.Generic.List<>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location)
            },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var rewriter = new CustomRefactoringRewriter(semanticModel, "Program.cs");
        var root = await syntaxTree.GetRootAsync();
        var newRoot = rewriter.Visit(root);
        
        if (!newRoot.IsEquivalentTo(root))
        {
            await File.WriteAllTextAsync("Program.cs", newRoot.ToFullString());
            Console.WriteLine("重构完成!");
            foreach (var refactoring in rewriter.Refactorings)
            {
                Console.WriteLine($"[{refactoring.RefactoringType}] {refactoring.Description}");
            }
        }
        else
        {
            Console.WriteLine("没有需要重构的代码。");
        }
    }
}
```

## 7. 总结

Roslynator 是一个功能强大的代码分析与重构工具，基于 .NET 10 和 AOT 编译技术，提供了全面的代码质量检查和自动重构功能。通过本示例文档，您可以了解如何：

1. **使用命令行工具**：执行代码分析和重构操作
2. **集成到 CI/CD 流程**：在持续集成和持续部署过程中自动检查代码质量
3. **扩展和定制**：创建自定义分析规则和重构类型
4. **应用到实际项目**：在真实场景中使用 Roslynator 提高代码质量

Roslynator 不仅是一个实用的开发工具，也是学习 Roslyn 编译器 API 和 .NET AOT 编译技术的优秀示例。通过不断探索和使用，您可以充分发挥其潜力，编写出更高质量、更高效的代码。
