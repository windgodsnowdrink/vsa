#:sdk Microsoft.NET.Sdk
#:package Microsoft.CodeAnalysis.CSharp@4.10.0
#:package Microsoft.CodeAnalysis.Common@4.10.0
#:package System.CommandLine@2.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property SelfContained=true
#:property PublishSingleFile=true
#:property RuntimeIdentifier=win-x64

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Roslynator.Analyzer
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // 配置依赖注入
            var serviceProvider = ConfigureServices();
            
            // 创建命令行根命令
            var rootCommand = new RootCommand("Roslynator 代码分析工具");
            
            // 创建分析命令
            var analyzeCommand = new Command("analyze", "分析代码质量");
            
            // 添加命令参数
            var pathOption = new Option<string>("--path", "要分析的文件、项目或解决方案路径");
            var severityOption = new Option<string>("--severity", () => "warning", "分析结果的严重程度过滤");
            var outputOption = new Option<string>("--output", () => "analysis-report.json", "分析报告输出路径");
            
            analyzeCommand.AddOption(pathOption);
            analyzeCommand.AddOption(severityOption);
            analyzeCommand.AddOption(outputOption);
            
            // 设置命令处理程序
            analyzeCommand.SetHandler(async (context) =>
            {
                var path = context.ParseResult.GetValueForOption(pathOption);
                var severity = context.ParseResult.GetValueForOption(severityOption);
                var output = context.ParseResult.GetValueForOption(outputOption);
                var cancellationToken = context.GetCancellationToken();
                
                await HandleAnalyzeCommand(serviceProvider, path, severity, output, cancellationToken);
            });
            
            // 添加分析命令到根命令
            rootCommand.AddCommand(analyzeCommand);
            
            // 执行命令
            return await rootCommand.InvokeAsync(args);
        }
        
        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            
            // 注册服务
            services.AddSingleton<ICodeAnalyzer, CodeAnalyzer>();
            services.AddSingleton<IAnalysisEngine, AnalysisEngine>();
            services.AddSingleton<IProjectLoader, ProjectLoader>();
            services.AddSingleton<IDiagnosticReporter, DiagnosticReporter>();
            
            return services.BuildServiceProvider();
        }
        
        private static async Task HandleAnalyzeCommand(IServiceProvider serviceProvider, string path, string severity, string output, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("错误: 必须指定要分析的路径");
                return;
            }
            
            if (!File.Exists(path) && !Directory.Exists(path))
            {
                Console.WriteLine($"错误: 路径不存在: {path}");
                return;
            }
            
            Console.WriteLine($"开始分析: {path}");
            Console.WriteLine($"严重程度过滤: {severity}");
            
            try
            {
                var analyzer = serviceProvider.GetRequiredService<ICodeAnalyzer>();
                var options = new AnalyzerOptions { Severity = severity };
                
                var result = await analyzer.AnalyzeAsync(path, options, cancellationToken);
                
                // 生成报告
                await GenerateReport(result, output);
                
                Console.WriteLine($"分析完成! 报告已保存到: {output}");
                Console.WriteLine($"找到 {result.Diagnostics.Count} 个问题");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"分析过程中发生错误: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        private static async Task GenerateReport(AnalysisResult result, string outputPath)
        {
            var report = new AnalysisReport
            {
                Path = result.Path,
                StartTime = result.StartTime,
                EndTime = result.EndTime,
                Duration = (result.EndTime - result.StartTime).TotalSeconds,
                Diagnostics = result.Diagnostics,
                Summary = new AnalysisSummary
                {
                    TotalIssues = result.Diagnostics.Count,
                    IssuesBySeverity = result.Diagnostics.GroupBy(d => d.Severity)
                        .ToDictionary(g => g.Key, g => g.Count())
                }
            };
            
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            var jsonContent = JsonSerializer.Serialize(report, jsonOptions);
            
            await File.WriteAllTextAsync(outputPath, jsonContent);
        }
    }
    
    // 服务接口
    public interface ICodeAnalyzer
    {
        Task<AnalysisResult> AnalyzeAsync(string path, AnalyzerOptions options, CancellationToken cancellationToken);
    }
    
    public interface IAnalysisEngine
    {
        Task<List<DiagnosticInfo>> AnalyzeFileAsync(string filePath, AnalyzerOptions options, CancellationToken cancellationToken);
        Task<List<DiagnosticInfo>> AnalyzeProjectAsync(Project project, AnalyzerOptions options, CancellationToken cancellationToken);
        Task<List<DiagnosticInfo>> AnalyzeSolutionAsync(Solution solution, AnalyzerOptions options, CancellationToken cancellationToken);
    }
    
    public interface IProjectLoader
    {
        Task<Project> LoadProjectAsync(string projectPath, CancellationToken cancellationToken);
        Task<Solution> LoadSolutionAsync(string solutionPath, CancellationToken cancellationToken);
    }
    
    public interface IDiagnosticReporter
    {
        void Report(DiagnosticInfo diagnostic);
    }
    
    // 实现类
    public class CodeAnalyzer : ICodeAnalyzer
    {
        private readonly IAnalysisEngine _analysisEngine;
        private readonly IProjectLoader _projectLoader;
        
        public CodeAnalyzer(IAnalysisEngine analysisEngine, IProjectLoader projectLoader)
        {
            _analysisEngine = analysisEngine;
            _projectLoader = projectLoader;
        }
        
        public async Task<AnalysisResult> AnalyzeAsync(string path, AnalyzerOptions options, CancellationToken cancellationToken)
        {
            var result = new AnalysisResult { Path = path, StartTime = DateTime.Now };
            
            try
            {
                if (File.Exists(path))
                {
                    var extension = Path.GetExtension(path).ToLower();
                    
                    if (extension == ".cs")
                    {
                        // 分析单个 C# 文件
                        result.Diagnostics = await _analysisEngine.AnalyzeFileAsync(path, options, cancellationToken);
                    }
                    else if (extension == ".csproj")
                    {
                        // 分析项目
                        var project = await _projectLoader.LoadProjectAsync(path, cancellationToken);
                        result.Diagnostics = await _analysisEngine.AnalyzeProjectAsync(project, options, cancellationToken);
                    }
                    else if (extension == ".sln")
                    {
                        // 分析解决方案
                        var solution = await _projectLoader.LoadSolutionAsync(path, cancellationToken);
                        result.Diagnostics = await _analysisEngine.AnalyzeSolutionAsync(solution, options, cancellationToken);
                    }
                    else
                    {
                        Console.WriteLine($"不支持的文件类型: {extension}");
                    }
                }
                else if (Directory.Exists(path))
                {
                    // 分析目录中的所有 C# 文件
                    var csFiles = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
                    var allDiagnostics = new List<DiagnosticInfo>();
                    
                    foreach (var file in csFiles)
                    {
                        var fileDiagnostics = await _analysisEngine.AnalyzeFileAsync(file, options, cancellationToken);
                        allDiagnostics.AddRange(fileDiagnostics);
                    }
                    
                    result.Diagnostics = allDiagnostics;
                }
                else
                {
                    Console.WriteLine($"路径不存在: {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"分析过程中发生错误: {ex.Message}");
                result.Diagnostics = new List<DiagnosticInfo>();
            }
            finally
            {
                result.EndTime = DateTime.Now;
            }
            
            return result;
        }
    }
    
    public class AnalysisEngine : IAnalysisEngine
    {
        public async Task<List<DiagnosticInfo>> AnalyzeFileAsync(string filePath, AnalyzerOptions options, CancellationToken cancellationToken)
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
                
                // 分析语法树
                var syntaxAnalyzer = new SyntaxAnalyzer(semanticModel, options);
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
        
        public async Task<List<DiagnosticInfo>> AnalyzeProjectAsync(Project project, AnalyzerOptions options, CancellationToken cancellationToken)
        {
            var diagnostics = new List<DiagnosticInfo>();
            
            try
            {
                // 分析项目中的所有文件
                foreach (var document in project.Documents)
                {
                    if (document.Language == LanguageNames.CSharp)
                    {
                        var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);
                        if (syntaxTree != null)
                        {
                            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
                            if (semanticModel != null)
                            {
                                var syntaxAnalyzer = new SyntaxAnalyzer(semanticModel, options);
                                var root = await syntaxTree.GetRootAsync(cancellationToken);
                                syntaxAnalyzer.Visit(root);
                                
                                diagnostics.AddRange(syntaxAnalyzer.Diagnostics);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"分析项目时发生错误: {ex.Message}");
            }
            
            return diagnostics;
        }
        
        public async Task<List<DiagnosticInfo>> AnalyzeSolutionAsync(Solution solution, AnalyzerOptions options, CancellationToken cancellationToken)
        {
            var diagnostics = new List<DiagnosticInfo>();
            
            try
            {
                // 分析解决方案中的所有项目
                foreach (var projectId in solution.ProjectIds)
                {
                    var project = solution.GetProject(projectId);
                    if (project != null)
                    {
                        var projectDiagnostics = await AnalyzeProjectAsync(project, options, cancellationToken);
                        diagnostics.AddRange(projectDiagnostics);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"分析解决方案时发生错误: {ex.Message}");
            }
            
            return diagnostics;
        }
        
        private IEnumerable<MetadataReference> GetMetadataReferences()
        {
            // 添加基本的元数据引用
            return new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Collections.Generic.List<>).Assembly.Location)
            };
        }
    }
    
    public class ProjectLoader : IProjectLoader
    {
        public async Task<Project> LoadProjectAsync(string projectPath, CancellationToken cancellationToken)
        {
            var workspace = MSBuildWorkspace.Create();
            var project = await workspace.OpenProjectAsync(projectPath, cancellationToken);
            return project;
        }
        
        public async Task<Solution> LoadSolutionAsync(string solutionPath, CancellationToken cancellationToken)
        {
            var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(solutionPath, cancellationToken);
            return solution;
        }
    }
    
    public class DiagnosticReporter : IDiagnosticReporter
    {
        public void Report(DiagnosticInfo diagnostic)
        {
            Console.WriteLine($"[{diagnostic.Severity}] {diagnostic.FilePath}:{diagnostic.LineNumber}:{diagnostic.ColumnNumber} - {diagnostic.Message}");
        }
    }
    
    // 语法分析器
    public class SyntaxAnalyzer : CSharpSyntaxWalker
    {
        private readonly SemanticModel _semanticModel;
        private readonly AnalyzerOptions _options;
        public List<DiagnosticInfo> Diagnostics { get; } = new List<DiagnosticInfo>();
        
        public SyntaxAnalyzer(SemanticModel semanticModel, AnalyzerOptions options)
        {
            _semanticModel = semanticModel;
            _options = options;
        }
        
        // 访问方法声明
        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            base.VisitMethodDeclaration(node);
            
            // 检查方法命名规范
            if (!IsValidMethodName(node.Identifier.Text))
            {
                Diagnostics.Add(new DiagnosticInfo
                {
                    Id = "NAMING001",
                    Message = $"方法名 '{node.Identifier.Text}' 不符合 PascalCase 命名规范",
                    Severity = "warning",
                    FilePath = node.SyntaxTree.FilePath,
                    LineNumber = node.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    ColumnNumber = node.Identifier.GetLocation().GetLineSpan().StartLinePosition.Character + 1
                });
            }
            
            // 检查方法参数命名规范
            foreach (var parameter in node.ParameterList.Parameters)
            {
                if (!IsValidParameterName(parameter.Identifier.Text))
                {
                    Diagnostics.Add(new DiagnosticInfo
                    {
                        Id = "NAMING002",
                        Message = $"参数名 '{parameter.Identifier.Text}' 不符合 camelCase 命名规范",
                        Severity = "warning",
                        FilePath = parameter.SyntaxTree.FilePath,
                        LineNumber = parameter.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                        ColumnNumber = parameter.Identifier.GetLocation().GetLineSpan().StartLinePosition.Character + 1
                    });
                }
            }
            
            // 检查 XML 注释
            if (node.GetLeadingTrivia().All(t => t.Kind() != SyntaxKind.SingleLineDocumentationCommentTrivia && 
                                               t.Kind() != SyntaxKind.MultiLineDocumentationCommentTrivia))
            {
                Diagnostics.Add(new DiagnosticInfo
                {
                    Id = "DOC001",
                    Message = $"方法 '{node.Identifier.Text}' 缺少 XML 注释",
                    Severity = "info",
                    FilePath = node.SyntaxTree.FilePath,
                    LineNumber = node.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    ColumnNumber = node.Identifier.GetLocation().GetLineSpan().StartLinePosition.Character + 1
                });
            }
        }
        
        // 访问类声明
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            base.VisitClassDeclaration(node);
            
            // 检查类命名规范
            if (!IsValidClassName(node.Identifier.Text))
            {
                Diagnostics.Add(new DiagnosticInfo
                {
                    Id = "NAMING003",
                    Message = $"类名 '{node.Identifier.Text}' 不符合 PascalCase 命名规范",
                    Severity = "warning",
                    FilePath = node.SyntaxTree.FilePath,
                    LineNumber = node.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    ColumnNumber = node.Identifier.GetLocation().GetLineSpan().StartLinePosition.Character + 1
                });
            }
        }
        
        // 访问变量声明
        public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            base.VisitVariableDeclaration(node);
            
            // 检查变量命名规范
            foreach (var variable in node.Variables)
            {
                if (!IsValidVariableName(variable.Identifier.Text))
                {
                    Diagnostics.Add(new DiagnosticInfo
                    {
                        Id = "NAMING004",
                        Message = $"变量名 '{variable.Identifier.Text}' 不符合 camelCase 命名规范",
                        Severity = "warning",
                        FilePath = variable.SyntaxTree.FilePath,
                        LineNumber = variable.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                        ColumnNumber = variable.Identifier.GetLocation().GetLineSpan().StartLinePosition.Character + 1
                    });
                }
            }
        }
        
        // 访问属性声明
        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            base.VisitPropertyDeclaration(node);
            
            // 检查属性命名规范
            if (!IsValidPropertyName(node.Identifier.Text))
            {
                Diagnostics.Add(new DiagnosticInfo
                {
                    Id = "NAMING005",
                    Message = $"属性名 '{node.Identifier.Text}' 不符合 PascalCase 命名规范",
                    Severity = "warning",
                    FilePath = node.SyntaxTree.FilePath,
                    LineNumber = node.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    ColumnNumber = node.Identifier.GetLocation().GetLineSpan().StartLinePosition.Character + 1
                });
            }
        }
        
        // 访问方法调用
        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            base.VisitInvocationExpression(node);
            
            // 检查字符串拼接
            if (IsStringConcat(node))
            {
                Diagnostics.Add(new DiagnosticInfo
                {
                    Id = "PERF001",
                    Message = "使用字符串拼接可能影响性能，建议使用 StringBuilder 或字符串插值",
                    Severity = "info",
                    FilePath = node.SyntaxTree.FilePath,
                    LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    ColumnNumber = node.GetLocation().GetLineSpan().StartLinePosition.Character + 1
                });
            }
        }
        
        // 辅助方法
        private bool IsValidMethodName(string name)
        {
            return char.IsUpper(name[0]) && !name.Contains('_');
        }
        
        private bool IsValidParameterName(string name)
        {
            return char.IsLower(name[0]) && !name.Contains('_');
        }
        
        private bool IsValidClassName(string name)
        {
            return char.IsUpper(name[0]) && !name.Contains('_');
        }
        
        private bool IsValidVariableName(string name)
        {
            return char.IsLower(name[0]) && !name.Contains('_');
        }
        
        private bool IsValidPropertyName(string name)
        {
            return char.IsUpper(name[0]) && !name.Contains('_');
        }
        
        private bool IsStringConcat(InvocationExpressionSyntax node)
        {
            var memberAccess = node.Expression as MemberAccessExpressionSyntax;
            if (memberAccess != null)
            {
                var methodName = memberAccess.Name.Identifier.Text;
                var expressionType = _semanticModel.GetTypeInfo(memberAccess.Expression).Type;
                
                return methodName == "Concat" && expressionType?.ToDisplayString() == "string";
            }
            return false;
        }
    }
    
    // 数据结构
    public class AnalysisResult
    {
        public string Path { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<DiagnosticInfo> Diagnostics { get; set; } = new List<DiagnosticInfo>();
    }
    
    public class AnalyzerOptions
    {
        public string Severity { get; set; } = "warning";
        public bool EnableAllRules { get; set; } = true;
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
    
    public class AnalysisReport
    {
        public string Path { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Duration { get; set; }
        public List<DiagnosticInfo> Diagnostics { get; set; } = new List<DiagnosticInfo>();
        public AnalysisSummary Summary { get; set; } = new AnalysisSummary();
    }
    
    public class AnalysisSummary
    {
        public int TotalIssues { get; set; }
        public Dictionary<string, int> IssuesBySeverity { get; set; } = new Dictionary<string, int>();
    }
}
