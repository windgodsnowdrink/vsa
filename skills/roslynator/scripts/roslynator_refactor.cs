#:sdk Microsoft.NET.Sdk
#:package Microsoft.CodeAnalysis.CSharp@4.10.0
#:package Microsoft.CodeAnalysis.Common@4.10.0
#:package System.CommandLine@2.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
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
using Microsoft.Extensions.DependencyInjection;

namespace Roslynator.Refactor
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // 配置依赖注入
            var serviceProvider = ConfigureServices();
            
            // 创建命令行根命令
            var rootCommand = new RootCommand("Roslynator 代码重构工具");
            
            // 创建重构命令
            var refactorCommand = new Command("refactor", "执行代码重构");
            
            // 添加命令参数
            var pathOption = new Option<string>("--path", "要重构的文件、项目或解决方案路径");
            var refactoringTypeOption = new Option<string>("--refactoring-type", () => "all", "重构类型");
            var outputOption = new Option<string>("--output", () => "refactoring-report.json", "重构报告输出路径");
            
            refactorCommand.AddOption(pathOption);
            refactorCommand.AddOption(refactoringTypeOption);
            refactorCommand.AddOption(outputOption);
            
            // 设置命令处理程序
            refactorCommand.SetHandler(async (context) =>
            {
                var path = context.ParseResult.GetValueForOption(pathOption);
                var refactoringType = context.ParseResult.GetValueForOption(refactoringTypeOption);
                var output = context.ParseResult.GetValueForOption(outputOption);
                var cancellationToken = context.GetCancellationToken();
                
                await HandleRefactorCommand(serviceProvider, path, refactoringType, output, cancellationToken);
            });
            
            // 添加重构命令到根命令
            rootCommand.AddCommand(refactorCommand);
            
            // 执行命令
            return await rootCommand.InvokeAsync(args);
        }
        
        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            
            // 注册服务
            services.AddSingleton<IRefactoringProvider, RefactoringProvider>();
            services.AddSingleton<IRefactoringEngine, RefactoringEngine>();
            services.AddSingleton<IProjectLoader, ProjectLoader>();
            services.AddSingleton<IRefactoringReporter, RefactoringReporter>();
            
            return services.BuildServiceProvider();
        }
        
        private static async Task HandleRefactorCommand(IServiceProvider serviceProvider, string path, string refactoringType, string output, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("错误: 必须指定要重构的路径");
                return;
            }
            
            if (!File.Exists(path) && !Directory.Exists(path))
            {
                Console.WriteLine($"错误: 路径不存在: {path}");
                return;
            }
            
            Console.WriteLine($"开始重构: {path}");
            Console.WriteLine($"重构类型: {refactoringType}");
            
            try
            {
                var refactoringProvider = serviceProvider.GetRequiredService<IRefactoringProvider>();
                var options = new RefactoringOptions { RefactoringType = refactoringType };
                
                var result = await refactoringProvider.RefactorAsync(path, options, cancellationToken);
                
                // 生成报告
                await GenerateReport(result, output);
                
                Console.WriteLine($"重构完成! 报告已保存到: {output}");
                Console.WriteLine($"执行了 {result.Refactorings.Count} 个重构操作");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"重构过程中发生错误: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        private static async Task GenerateReport(RefactoringResult result, string outputPath)
        {
            var report = new RefactoringReport
            {
                Path = result.Path,
                StartTime = result.StartTime,
                EndTime = result.EndTime,
                Duration = (result.EndTime - result.StartTime).TotalSeconds,
                Refactorings = result.Refactorings,
                Summary = new RefactoringSummary
                {
                    TotalRefactorings = result.Refactorings.Count,
                    RefactoringsByType = result.Refactorings.GroupBy(r => r.RefactoringType)
                        .ToDictionary(g => g.Key, g => g.Count())
                }
            };
            
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            var jsonContent = JsonSerializer.Serialize(report, jsonOptions);
            
            await File.WriteAllTextAsync(outputPath, jsonContent);
        }
    }
    
    // 服务接口
    public interface IRefactoringProvider
    {
        Task<RefactoringResult> RefactorAsync(string path, RefactoringOptions options, CancellationToken cancellationToken);
    }
    
    public interface IRefactoringEngine
    {
        Task<List<RefactoringInfo>> RefactorFileAsync(string filePath, RefactoringOptions options, CancellationToken cancellationToken);
        Task<List<RefactoringInfo>> RefactorProjectAsync(Project project, RefactoringOptions options, CancellationToken cancellationToken);
        Task<List<RefactoringInfo>> RefactorSolutionAsync(Solution solution, RefactoringOptions options, CancellationToken cancellationToken);
    }
    
    public interface IProjectLoader
    {
        Task<Project> LoadProjectAsync(string projectPath, CancellationToken cancellationToken);
        Task<Solution> LoadSolutionAsync(string solutionPath, CancellationToken cancellationToken);
    }
    
    public interface IRefactoringReporter
    {
        void Report(RefactoringInfo refactoring);
    }
    
    // 实现类
    public class RefactoringProvider : IRefactoringProvider
    {
        private readonly IRefactoringEngine _refactoringEngine;
        private readonly IProjectLoader _projectLoader;
        
        public RefactoringProvider(IRefactoringEngine refactoringEngine, IProjectLoader projectLoader)
        {
            _refactoringEngine = refactoringEngine;
            _projectLoader = projectLoader;
        }
        
        public async Task<RefactoringResult> RefactorAsync(string path, RefactoringOptions options, CancellationToken cancellationToken)
        {
            var result = new RefactoringResult { Path = path, StartTime = DateTime.Now };
            
            try
            {
                if (File.Exists(path))
                {
                    var extension = Path.GetExtension(path).ToLower();
                    
                    if (extension == ".cs")
                    {
                        // 重构单个 C# 文件
                        result.Refactorings = await _refactoringEngine.RefactorFileAsync(path, options, cancellationToken);
                    }
                    else if (extension == ".csproj")
                    {
                        // 重构项目
                        var project = await _projectLoader.LoadProjectAsync(path, cancellationToken);
                        result.Refactorings = await _refactoringEngine.RefactorProjectAsync(project, options, cancellationToken);
                    }
                    else if (extension == ".sln")
                    {
                        // 重构解决方案
                        var solution = await _projectLoader.LoadSolutionAsync(path, cancellationToken);
                        result.Refactorings = await _refactoringEngine.RefactorSolutionAsync(solution, options, cancellationToken);
                    }
                    else
                    {
                        Console.WriteLine($"不支持的文件类型: {extension}");
                    }
                }
                else if (Directory.Exists(path))
                {
                    // 重构目录中的所有 C# 文件
                    var csFiles = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
                    var allRefactorings = new List<RefactoringInfo>();
                    
                    foreach (var file in csFiles)
                    {
                        var fileRefactorings = await _refactoringEngine.RefactorFileAsync(file, options, cancellationToken);
                        allRefactorings.AddRange(fileRefactorings);
                    }
                    
                    result.Refactorings = allRefactorings;
                }
                else
                {
                    Console.WriteLine($"路径不存在: {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"重构过程中发生错误: {ex.Message}");
                result.Refactorings = new List<RefactoringInfo>();
            }
            finally
            {
                result.EndTime = DateTime.Now;
            }
            
            return result;
        }
    }
    
    public class RefactoringEngine : IRefactoringEngine
    {
        public async Task<List<RefactoringInfo>> RefactorFileAsync(string filePath, RefactoringOptions options, CancellationToken cancellationToken)
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
                
                // 执行重构
                var root = await syntaxTree.GetRootAsync(cancellationToken);
                var refactoredRoot = root;
                
                // 根据重构类型执行不同的重构
                if (options.RefactoringType == "all" || options.RefactoringType == "expression-bodied")
                {
                    var expressionBodyRewriter = new ExpressionBodyRewriter(semanticModel, refactorings, filePath);
                    refactoredRoot = expressionBodyRewriter.Visit(refactoredRoot);
                }
                
                if (options.RefactoringType == "all" || options.RefactoringType == "string-interpolation")
                {
                    var stringInterpolationRewriter = new StringInterpolationRewriter(semanticModel, refactorings, filePath);
                    refactoredRoot = stringInterpolationRewriter.Visit(refactoredRoot);
                }
                
                if (options.RefactoringType == "all" || options.RefactoringType == "pattern-matching")
                {
                    var patternMatchingRewriter = new PatternMatchingRewriter(semanticModel, refactorings, filePath);
                    refactoredRoot = patternMatchingRewriter.Visit(refactoredRoot);
                }
                
                if (options.RefactoringType == "all" || options.RefactoringType == "null-check")
                {
                    var nullCheckRewriter = new NullCheckRewriter(semanticModel, refactorings, filePath);
                    refactoredRoot = nullCheckRewriter.Visit(refactoredRoot);
                }
                
                if (options.RefactoringType == "all" || options.RefactoringType == "using-declaration")
                {
                    var usingDeclarationRewriter = new UsingDeclarationRewriter(semanticModel, refactorings, filePath);
                    refactoredRoot = usingDeclarationRewriter.Visit(refactoredRoot);
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
        
        public async Task<List<RefactoringInfo>> RefactorProjectAsync(Project project, RefactoringOptions options, CancellationToken cancellationToken)
        {
            var refactorings = new List<RefactoringInfo>();
            
            try
            {
                // 重构项目中的所有文件
                foreach (var document in project.Documents)
                {
                    if (document.Language == LanguageNames.CSharp)
                    {
                        var filePath = document.FilePath;
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            var fileRefactorings = await RefactorFileAsync(filePath, options, cancellationToken);
                            refactorings.AddRange(fileRefactorings);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"重构项目时发生错误: {ex.Message}");
            }
            
            return refactorings;
        }
        
        public async Task<List<RefactoringInfo>> RefactorSolutionAsync(Solution solution, RefactoringOptions options, CancellationToken cancellationToken)
        {
            var refactorings = new List<RefactoringInfo>();
            
            try
            {
                // 重构解决方案中的所有项目
                foreach (var projectId in solution.ProjectIds)
                {
                    var project = solution.GetProject(projectId);
                    if (project != null)
                    {
                        var projectRefactorings = await RefactorProjectAsync(project, options, cancellationToken);
                        refactorings.AddRange(projectRefactorings);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"重构解决方案时发生错误: {ex.Message}");
            }
            
            return refactorings;
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
    
    public class RefactoringReporter : IRefactoringReporter
    {
        public void Report(RefactoringInfo refactoring)
        {
            Console.WriteLine($"[{refactoring.RefactoringType}] {refactoring.FilePath}:{refactoring.LineNumber}:{refactoring.ColumnNumber} - {refactoring.Description}");
        }
    }
    
    // 重构重写器
    public class ExpressionBodyRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel _semanticModel;
        private readonly List<RefactoringInfo> _refactorings;
        private readonly string _filePath;
        
        public ExpressionBodyRewriter(SemanticModel semanticModel, List<RefactoringInfo> refactorings, string filePath)
        {
            _semanticModel = semanticModel;
            _refactorings = refactorings;
            _filePath = filePath;
        }
        
        // 重写方法声明
        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            // 检查是否可以转换为表达式体成员
            if (CanConvertToExpressionBody(node))
            {
                var oldCode = node.ToString();
                var expressionBody = SyntaxFactory.ArrowExpressionClause(node.Body!.Statements[0].DescendantNodes().OfType<ReturnStatementSyntax>().First().Expression);
                var newNode = node.WithBody(null).WithExpressionBody(expressionBody).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                var newCode = newNode.ToString();
                
                // 添加重构信息
                _refactorings.Add(new RefactoringInfo
                {
                    RefactoringType = "expression-bodied",
                    Description = $"将方法 '{node.Identifier.Text}' 转换为表达式体成员",
                    FilePath = _filePath,
                    LineNumber = node.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    ColumnNumber = node.Identifier.GetLocation().GetLineSpan().StartLinePosition.Character + 1,
                    OldCode = oldCode,
                    NewCode = newCode
                });
                
                return newNode;
            }
            
            return base.VisitMethodDeclaration(node);
        }
        
        // 重写属性声明
        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            // 检查是否可以转换为表达式体成员
            if (CanConvertToExpressionBody(node))
            {
                var oldCode = node.ToString();
                var expressionBody = SyntaxFactory.ArrowExpressionClause(node.AccessorList!.Accessors[0].Body!.Statements[0].DescendantNodes().OfType<ReturnStatementSyntax>().First().Expression);
                var newNode = node.WithAccessorList(null).WithExpressionBody(expressionBody).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                var newCode = newNode.ToString();
                
                // 添加重构信息
                _refactorings.Add(new RefactoringInfo
                {
                    RefactoringType = "expression-bodied",
                    Description = $"将属性 '{node.Identifier.Text}' 转换为表达式体成员",
                    FilePath = _filePath,
                    LineNumber = node.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    ColumnNumber = node.Identifier.GetLocation().GetLineSpan().StartLinePosition.Character + 1,
                    OldCode = oldCode,
                    NewCode = newCode
                });
                
                return newNode;
            }
            
            return base.VisitPropertyDeclaration(node);
        }
        
        // 辅助方法
        private bool CanConvertToExpressionBody(MethodDeclarationSyntax node)
        {
            // 检查方法是否有单个返回语句
            return node.Body != null && 
                   node.Body.Statements.Count == 1 && 
                   node.Body.Statements[0] is ReturnStatementSyntax && 
                   !node.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword);
        }
        
        private bool CanConvertToExpressionBody(PropertyDeclarationSyntax node)
        {
            // 检查属性是否有单个 getter 且包含单个返回语句
            return node.AccessorList != null && 
                   node.AccessorList.Accessors.Count == 1 && 
                   node.AccessorList.Accessors[0].IsKind(SyntaxKind.GetAccessorDeclaration) && 
                   node.AccessorList.Accessors[0].Body != null && 
                   node.AccessorList.Accessors[0].Body.Statements.Count == 1 && 
                   node.AccessorList.Accessors[0].Body.Statements[0] is ReturnStatementSyntax;
        }
    }
    
    public class StringInterpolationRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel _semanticModel;
        private readonly List<RefactoringInfo> _refactorings;
        private readonly string _filePath;
        
        public StringInterpolationRewriter(SemanticModel semanticModel, List<RefactoringInfo> refactorings, string filePath)
        {
            _semanticModel = semanticModel;
            _refactorings = refactorings;
            _filePath = filePath;
        }
        
        // 重写二元表达式
        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            // 检查是否是字符串拼接
            if (IsStringConcat(node))
            {
                var oldCode = node.ToString();
                var newNode = ConvertToStringInterpolation(node);
                var newCode = newNode.ToString();
                
                // 添加重构信息
                _refactorings.Add(new RefactoringInfo
                {
                    RefactoringType = "string-interpolation",
                    Description = "将字符串拼接转换为字符串插值",
                    FilePath = _filePath,
                    LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    ColumnNumber = node.GetLocation().GetLineSpan().StartLinePosition.Character + 1,
                    OldCode = oldCode,
                    NewCode = newCode
                });
                
                return newNode;
            }
            
            return base.VisitBinaryExpression(node);
        }
        
        // 辅助方法
        private bool IsStringConcat(BinaryExpressionSyntax node)
        {
            // 检查是否是字符串拼接表达式
            if (node.Kind() != SyntaxKind.AddExpression)
                return false;
            
            // 检查表达式类型是否为字符串
            var leftType = _semanticModel.GetTypeInfo(node.Left).Type;
            var rightType = _semanticModel.GetTypeInfo(node.Right).Type;
            
            return (leftType?.ToDisplayString() == "string" || rightType?.ToDisplayString() == "string");
        }
        
        private SyntaxNode ConvertToStringInterpolation(BinaryExpressionSyntax node)
        {
            // 构建插值字符串内容
            var contents = new List<InterpolationSyntax>();
            var stringBuilder = new System.Text.StringBuilder();
            
            // 递归处理表达式
            ProcessExpression(node, stringBuilder, contents);
            
            // 创建插值字符串
            var interpolation = SyntaxFactory.InterpolatedStringExpression(
                SyntaxFactory.Token(SyntaxKind.InterpolatedStringStartToken),
                SyntaxFactory.List(contents),
                SyntaxFactory.Token(SyntaxKind.InterpolatedStringEndToken)
            );
            
            return interpolation;
        }
        
        private void ProcessExpression(ExpressionSyntax expression, System.Text.StringBuilder currentString, List<InterpolationSyntax> interpolations)
        {
            if (expression is BinaryExpressionSyntax binaryExpr && binaryExpr.Kind() == SyntaxKind.AddExpression)
            {
                // 处理左侧表达式
                ProcessExpression(binaryExpr.Left, currentString, interpolations);
                
                // 处理右侧表达式
                ProcessExpression(binaryExpr.Right, currentString, interpolations);
            }
            else if (expression is LiteralExpressionSyntax literalExpr && literalExpr.Kind() == SyntaxKind.StringLiteralExpression)
            {
                // 添加字符串字面量
                currentString.Append(literalExpr.Token.ValueText);
            }
            else
            {
                // 添加当前字符串作为字面量内容
                if (currentString.Length > 0)
                {
                    interpolations.Add(SyntaxFactory.Interpolation(
                        SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(currentString.ToString()))
                    ));
                    currentString.Clear();
                }
                
                // 添加表达式作为插值
                interpolations.Add(SyntaxFactory.Interpolation(expression));
            }
        }
    }
    
    public class PatternMatchingRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel _semanticModel;
        private readonly List<RefactoringInfo> _refactorings;
        private readonly string _filePath;
        
        public PatternMatchingRewriter(SemanticModel semanticModel, List<RefactoringInfo> refactorings, string filePath)
        {
            _semanticModel = semanticModel;
            _refactorings = refactorings;
            _filePath = filePath;
        }
        
        // 重写 if 语句
        public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
        {
            // 检查是否可以转换为模式匹配
            if (CanConvertToPatternMatching(node))
            {
                var oldCode = node.ToString();
                var newNode = ConvertToPatternMatching(node);
                var newCode = newNode.ToString();
                
                // 添加重构信息
                _refactorings.Add(new RefactoringInfo
                {
                    RefactoringType = "pattern-matching",
                    Description = "将类型检查转换为模式匹配",
                    FilePath = _filePath,
                    LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    ColumnNumber = node.GetLocation().GetLineSpan().StartLinePosition.Character + 1,
                    OldCode = oldCode,
                    NewCode = newCode
                });
                
                return newNode;
            }
            
            return base.VisitIfStatement(node);
        }
        
        // 辅助方法
        private bool CanConvertToPatternMatching(IfStatementSyntax node)
        {
            // 检查是否是类型检查
            if (node.Condition is BinaryExpressionSyntax binaryExpr && binaryExpr.Kind() == SyntaxKind.EqualsExpression)
            {
                var left = binaryExpr.Left;
                var right = binaryExpr.Right;
                
                // 检查是否是 typeof 表达式
                if (right is TypeOfExpressionSyntax)
                {
                    // 检查左侧是否是 is 表达式
                    if (left is BinaryExpressionSyntax isExpr && isExpr.Kind() == SyntaxKind.IsExpression)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
        
        private SyntaxNode ConvertToPatternMatching(IfStatementSyntax node)
        {
            // 提取表达式和类型
            var binaryExpr = (BinaryExpressionSyntax)node.Condition;
            var isExpr = (BinaryExpressionSyntax)binaryExpr.Left;
            var expression = isExpr.Left;
            var type = ((TypeOfExpressionSyntax)binaryExpr.Right).Type;
            
            // 创建模式匹配表达式
            var patternExpr = SyntaxFactory.IsPatternExpression(
                expression,
                SyntaxFactory.DeclarationPattern(
                    type,
                    SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("temp"))
                )
            );
            
            // 创建新的 if 语句
            var newIfStatement = node.WithCondition(patternExpr);
            
            // 替换语句中的类型转换
            var statementRewriter = new TypeCastRewriter(expression, type);
            var newStatement = statementRewriter.Visit(newIfStatement.Statement);
            
            return newIfStatement.WithStatement(newStatement);
        }
        
        private class TypeCastRewriter : CSharpSyntaxRewriter
        {
            private readonly ExpressionSyntax _expression;
            private readonly TypeSyntax _type;
            
            public TypeCastRewriter(ExpressionSyntax expression, TypeSyntax type)
            {
                _expression = expression;
                _type = type;
            }
            
            public override SyntaxNode VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
            {
                // 检查是否是类型转换
                if (node.Parent is CastExpressionSyntax castExpr && castExpr.Type.IsEquivalentTo(_type))
                {
                    if (castExpr.Expression.IsEquivalentTo(_expression))
                    {
                        // 替换为临时变量
                        return SyntaxFactory.IdentifierName("temp");
                    }
                }
                
                return base.VisitParenthesizedExpression(node);
            }
        }
    }
    
    public class NullCheckRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel _semanticModel;
        private readonly List<RefactoringInfo> _refactorings;
        private readonly string _filePath;
        
        public NullCheckRewriter(SemanticModel semanticModel, List<RefactoringInfo> refactorings, string filePath)
        {
            _semanticModel = semanticModel;
            _refactorings = refactorings;
            _filePath = filePath;
        }
        
        // 重写条件表达式
        public override SyntaxNode VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            // 检查是否是空值检查
            if (IsNullCheck(node))
            {
                var oldCode = node.ToString();
                var newNode = ConvertToNullCoalescing(node);
                var newCode = newNode.ToString();
                
                // 添加重构信息
                _refactorings.Add(new RefactoringInfo
                {
                    RefactoringType = "null-check",
                    Description = "将空值检查转换为空值合并运算符",
                    FilePath = _filePath,
                    LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    ColumnNumber = node.GetLocation().GetLineSpan().StartLinePosition.Character + 1,
                    OldCode = oldCode,
                    NewCode = newCode
                });
                
                return newNode;
            }
            
            return base.VisitConditionalExpression(node);
        }
        
        // 辅助方法
        private bool IsNullCheck(ConditionalExpressionSyntax node)
        {
            // 检查是否是空值比较
            if (node.Condition is BinaryExpressionSyntax binaryExpr)
            {
                if (binaryExpr.Kind() == SyntaxKind.NotEqualsExpression || binaryExpr.Kind() == SyntaxKind.EqualsExpression)
                {
                    // 检查是否与 null 比较
                    if ((binaryExpr.Left is LiteralExpressionSyntax leftLiteral && leftLiteral.Kind() == SyntaxKind.NullLiteralExpression) ||
                        (binaryExpr.Right is LiteralExpressionSyntax rightLiteral && rightLiteral.Kind() == SyntaxKind.NullLiteralExpression))
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
        
        private SyntaxNode ConvertToNullCoalescing(ConditionalExpressionSyntax node)
        {
            var binaryExpr = (BinaryExpressionSyntax)node.Condition;
            ExpressionSyntax expression = null;
            ExpressionSyntax whenNotNull = null;
            ExpressionSyntax whenNull = null;
            
            // 提取表达式和值
            if (binaryExpr.Left is LiteralExpressionSyntax && binaryExpr.Left.Kind() == SyntaxKind.NullLiteralExpression)
            {
                expression = binaryExpr.Right;
                whenNotNull = binaryExpr.Kind() == SyntaxKind.NotEqualsExpression ? node.WhenTrue : node.WhenFalse;
                whenNull = binaryExpr.Kind() == SyntaxKind.NotEqualsExpression ? node.WhenFalse : node.WhenTrue;
            }
            else if (binaryExpr.Right is LiteralExpressionSyntax && binaryExpr.Right.Kind() == SyntaxKind.NullLiteralExpression)
            {
                expression = binaryExpr.Left;
                whenNotNull = binaryExpr.Kind() == SyntaxKind.NotEqualsExpression ? node.WhenTrue : node.WhenFalse;
                whenNull = binaryExpr.Kind() == SyntaxKind.NotEqualsExpression ? node.WhenFalse : node.WhenTrue;
            }
            
            // 创建空值合并表达式
            if (expression != null)
            {
                return SyntaxFactory.BinaryExpression(
                    SyntaxKind.CoalesceExpression,
                    expression,
                    whenNull
                );
            }
            
            return node;
        }
    }
    
    public class UsingDeclarationRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel _semanticModel;
        private readonly List<RefactoringInfo> _refactorings;
        private readonly string _filePath;
        
        public UsingDeclarationRewriter(SemanticModel semanticModel, List<RefactoringInfo> refactorings, string filePath)
        {
            _semanticModel = semanticModel;
            _refactorings = refactorings;
            _filePath = filePath;
        }
        
        // 重写 using 语句
        public override SyntaxNode VisitUsingStatement(UsingStatementSyntax node)
        {
            // 检查是否可以转换为 using 声明
            if (CanConvertToUsingDeclaration(node))
            {
                var oldCode = node.ToString();
                var newNode = ConvertToUsingDeclaration(node);
                var newCode = newNode.ToString();
                
                // 添加重构信息
                _refactorings.Add(new RefactoringInfo
                {
                    RefactoringType = "using-declaration",
                    Description = "将 using 语句转换为 using 声明",
                    FilePath = _filePath,
                    LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    ColumnNumber = node.GetLocation().GetLineSpan().StartLinePosition.Character + 1,
                    OldCode = oldCode,
                    NewCode = newCode
                });
                
                return newNode;
            }
            
            return base.VisitUsingStatement(node);
        }
        
        // 辅助方法
        private bool CanConvertToUsingDeclaration(UsingStatementSyntax node)
        {
            // 检查是否是简单的 using 语句
            return node.Declaration != null && node.Statement is BlockSyntax block && block.Statements.Count == 1;
        }
        
        private SyntaxNode ConvertToUsingDeclaration(UsingStatementSyntax node)
        {
            // 创建 using 声明
            var usingDeclaration = SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.UsingKeyword)),
                node.Declaration
            );
            
            // 获取块中的语句
            var block = (BlockSyntax)node.Statement;
            var statement = block.Statements[0];
            
            // 创建语句列表
            var statements = SyntaxFactory.List<StatementSyntax>(new[] { usingDeclaration, statement });
            
            // 创建新的块
            return SyntaxFactory.Block(statements);
        }
    }
    
    // 数据结构
    public class RefactoringResult
    {
        public string Path { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<RefactoringInfo> Refactorings { get; set; } = new List<RefactoringInfo>();
    }
    
    public class RefactoringOptions
    {
        public string RefactoringType { get; set; } = "all";
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
    
    public class RefactoringReport
    {
        public string Path { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Duration { get; set; }
        public List<RefactoringInfo> Refactorings { get; set; } = new List<RefactoringInfo>();
        public RefactoringSummary Summary { get; set; } = new RefactoringSummary();
    }
    
    public class RefactoringSummary
    {
        public int TotalRefactorings { get; set; }
        public Dictionary<string, int> RefactoringsByType { get; set; } = new Dictionary<string, int>();
    }
}
