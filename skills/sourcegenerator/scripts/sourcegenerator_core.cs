#:sdk Microsoft.NET.Sdk
#:package Microsoft.CodeAnalysis.Analyzers@4.10.0
#:package Microsoft.CodeAnalysis.CSharp@4.10.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Scrutor@4.0.0
#:package System.CommandLine@2.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Logging.Console@10.0.0
#:package Newtonsoft.Json@13.0.3
#:property LangVersion=latest
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property SelfContained=true
#:property PublishSingleFile=true

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace SourceGeneratorSkill
{
    // 数据模型
    public class CodeTemplate
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CustomAttribute
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Parameters { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CodeAnalysisResult
    {
        public string FilePath { get; set; }
        public int ClassCount { get; set; }
        public int MethodCount { get; set; }
        public int PropertyCount { get; set; }
        public List<CodeIssue> Issues { get; set; } = new List<CodeIssue>();
        public DateTime AnalyzedAt { get; set; }
    }

    public class CodeIssue
    {
        public string Severity { get; set; }
        public string Message { get; set; }
        public int LineNumber { get; set; }
        public string Code { get; set; }
    }

    public class SourceGeneratorOptions
    {
        public string TemplateDirectory { get; set; } = "./templates";
        public string AttributeDirectory { get; set; } = "./attributes";
        public string OutputDirectory { get; set; } = "./generated";
        public bool EnableIncrementalGeneration { get; set; } = true;
        public bool EnableCache { get; set; } = true;
        public int CacheExpirationMinutes { get; set; } = 60;
    }

    // 核心接口
    public interface ISourceGeneratorService
    {
        Task<string> GenerateCodeAsync(string type, string outputPath, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
        Task<string> GenerateFromTemplateAsync(string templateId, string outputPath, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
        Task<string> GenerateReflectionInfoAsync(string typeName, string outputPath, CancellationToken cancellationToken = default);
        Task<string> GenerateFromAttributeAsync(string attributeName, string outputPath, CancellationToken cancellationToken = default);
        Task<string> AnalyzeCodeAsync(string filePath, CancellationToken cancellationToken = default);
    }

    public interface ITemplateService
    {
        Task<string> CreateTemplateAsync(string name, string content, CancellationToken cancellationToken = default);
        Task<IEnumerable<CodeTemplate>> GetTemplatesAsync(CancellationToken cancellationToken = default);
        Task<CodeTemplate> GetTemplateAsync(string templateId, CancellationToken cancellationToken = default);
        Task UpdateTemplateAsync(string templateId, string name, string content, CancellationToken cancellationToken = default);
        Task DeleteTemplateAsync(string templateId, CancellationToken cancellationToken = default);
        Task<string> RenderTemplateAsync(string templateId, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
    }

    public interface IAttributeService
    {
        Task<string> CreateAttributeAsync(string name, string parameters, CancellationToken cancellationToken = default);
        Task<IEnumerable<CustomAttribute>> GetAttributesAsync(CancellationToken cancellationToken = default);
        Task<CustomAttribute> GetAttributeAsync(string attributeId, CancellationToken cancellationToken = default);
        Task UpdateAttributeAsync(string attributeId, string name, string parameters, CancellationToken cancellationToken = default);
        Task DeleteAttributeAsync(string attributeId, CancellationToken cancellationToken = default);
    }

    public interface ICodeAnalysisService
    {
        Task<CodeAnalysisResult> AnalyzeFileAsync(string filePath, CancellationToken cancellationToken = default);
        Task<CodeAnalysisResult> AnalyzeProjectAsync(string projectPath, CancellationToken cancellationToken = default);
        Task<IEnumerable<CodeIssue>> GetIssuesAsync(string filePath, CancellationToken cancellationToken = default);
    }

    // 实现类
    public class TemplateService : ITemplateService
    {
        private readonly SourceGeneratorOptions _options;
        private readonly ILogger<TemplateService> _logger;
        private readonly Dictionary<string, CodeTemplate> _templates;

        public TemplateService(IOptions<SourceGeneratorOptions> options, ILogger<TemplateService> logger)
        {
            _options = options.Value;
            _logger = logger;
            _templates = new Dictionary<string, CodeTemplate>();
            
            // 初始化模板存储目录
            Directory.CreateDirectory(_options.TemplateDirectory);
            LoadTemplates();
        }

        private void LoadTemplates()
        {
            try
            {
                var templateFiles = Directory.GetFiles(_options.TemplateDirectory, "*.json");
                foreach (var file in templateFiles)
                {
                    var content = File.ReadAllText(file);
                    var template = JsonConvert.DeserializeObject<CodeTemplate>(content);
                    if (template != null)
                    {
                        _templates[template.Id] = template;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加载模板失败");
            }
        }

        private void SaveTemplate(CodeTemplate template)
        {
            try
            {
                var filePath = Path.Combine(_options.TemplateDirectory, $"{template.Id}.json");
                var content = JsonConvert.SerializeObject(template, Formatting.Indented);
                File.WriteAllText(filePath, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存模板失败: {TemplateId}", template.Id);
            }
        }

        public Task<string> CreateTemplateAsync(string name, string content, CancellationToken cancellationToken = default)
        {
            var templateId = Guid.NewGuid().ToString();
            var template = new CodeTemplate
            {
                Id = templateId,
                Name = name,
                Content = content,
                CreatedAt = DateTime.Now
            };

            _templates[templateId] = template;
            SaveTemplate(template);
            
            _logger.LogInformation("创建模板: {TemplateName} (ID: {TemplateId})", name, templateId);
            return Task.FromResult(templateId);
        }

        public Task<IEnumerable<CodeTemplate>> GetTemplatesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<CodeTemplate>>(_templates.Values);
        }

        public Task<CodeTemplate> GetTemplateAsync(string templateId, CancellationToken cancellationToken = default)
        {
            if (_templates.TryGetValue(templateId, out var template))
            {
                return Task.FromResult(template);
            }
            throw new KeyNotFoundException($"模板不存在: {templateId}");
        }

        public Task UpdateTemplateAsync(string templateId, string name, string content, CancellationToken cancellationToken = default)
        {
            if (_templates.TryGetValue(templateId, out var template))
            {
                template.Name = name;
                template.Content = content;
                template.UpdatedAt = DateTime.Now;
                SaveTemplate(template);
                
                _logger.LogInformation("更新模板: {TemplateId}", templateId);
            }
            else
            {
                throw new KeyNotFoundException($"模板不存在: {templateId}");
            }
            return Task.CompletedTask;
        }

        public Task DeleteTemplateAsync(string templateId, CancellationToken cancellationToken = default)
        {
            if (_templates.Remove(templateId))
            {
                var filePath = Path.Combine(_options.TemplateDirectory, $"{templateId}.json");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                _logger.LogInformation("删除模板: {TemplateId}", templateId);
            }
            else
            {
                throw new KeyNotFoundException($"模板不存在: {templateId}");
            }
            return Task.CompletedTask;
        }

        public async Task<string> RenderTemplateAsync(string templateId, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
        {
            var template = await GetTemplateAsync(templateId, cancellationToken);
            var content = template.Content;
            
            foreach (var param in parameters)
            {
                content = content.Replace($"{{{{{param.Key}}}}}", param.Value);
            }
            
            return content;
        }
    }

    public class AttributeService : IAttributeService
    {
        private readonly SourceGeneratorOptions _options;
        private readonly ILogger<AttributeService> _logger;
        private readonly Dictionary<string, CustomAttribute> _attributes;

        public AttributeService(IOptions<SourceGeneratorOptions> options, ILogger<AttributeService> logger)
        {
            _options = options.Value;
            _logger = logger;
            _attributes = new Dictionary<string, CustomAttribute>();
            
            // 初始化特性存储目录
            Directory.CreateDirectory(_options.AttributeDirectory);
            LoadAttributes();
        }

        private void LoadAttributes()
        {
            try
            {
                var attributeFiles = Directory.GetFiles(_options.AttributeDirectory, "*.json");
                foreach (var file in attributeFiles)
                {
                    var content = File.ReadAllText(file);
                    var attribute = JsonConvert.DeserializeObject<CustomAttribute>(content);
                    if (attribute != null)
                    {
                        _attributes[attribute.Id] = attribute;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加载特性失败");
            }
        }

        private void SaveAttribute(CustomAttribute attribute)
        {
            try
            {
                var filePath = Path.Combine(_options.AttributeDirectory, $"{attribute.Id}.json");
                var content = JsonConvert.SerializeObject(attribute, Formatting.Indented);
                File.WriteAllText(filePath, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存特性失败: {AttributeId}", attribute.Id);
            }
        }

        public Task<string> CreateAttributeAsync(string name, string parameters, CancellationToken cancellationToken = default)
        {
            var attributeId = Guid.NewGuid().ToString();
            var attribute = new CustomAttribute
            {
                Id = attributeId,
                Name = name,
                Parameters = parameters,
                CreatedAt = DateTime.Now
            };

            _attributes[attributeId] = attribute;
            SaveAttribute(attribute);
            
            _logger.LogInformation("创建特性: {AttributeName} (ID: {AttributeId})", name, attributeId);
            return Task.FromResult(attributeId);
        }

        public Task<IEnumerable<CustomAttribute>> GetAttributesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<CustomAttribute>>(_attributes.Values);
        }

        public Task<CustomAttribute> GetAttributeAsync(string attributeId, CancellationToken cancellationToken = default)
        {
            if (_attributes.TryGetValue(attributeId, out var attribute))
            {
                return Task.FromResult(attribute);
            }
            throw new KeyNotFoundException($"特性不存在: {attributeId}");
        }

        public Task UpdateAttributeAsync(string attributeId, string name, string parameters, CancellationToken cancellationToken = default)
        {
            if (_attributes.TryGetValue(attributeId, out var attribute))
            {
                attribute.Name = name;
                attribute.Parameters = parameters;
                attribute.UpdatedAt = DateTime.Now;
                SaveAttribute(attribute);
                
                _logger.LogInformation("更新特性: {AttributeId}", attributeId);
            }
            else
            {
                throw new KeyNotFoundException($"特性不存在: {attributeId}");
            }
            return Task.CompletedTask;
        }

        public Task DeleteAttributeAsync(string attributeId, CancellationToken cancellationToken = default)
        {
            if (_attributes.Remove(attributeId))
            {
                var filePath = Path.Combine(_options.AttributeDirectory, $"{attributeId}.json");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                _logger.LogInformation("删除特性: {AttributeId}", attributeId);
            }
            else
            {
                throw new KeyNotFoundException($"特性不存在: {attributeId}");
            }
            return Task.CompletedTask;
        }
    }

    public class CodeAnalysisService : ICodeAnalysisService
    {
        private readonly ILogger<CodeAnalysisService> _logger;

        public CodeAnalysisService(ILogger<CodeAnalysisService> logger)
        {
            _logger = logger;
        }

        public async Task<CodeAnalysisResult> AnalyzeFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("分析文件: {FilePath}", filePath);
                
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"文件不存在: {filePath}");
                }

                var content = await File.ReadAllTextAsync(filePath, cancellationToken);
                var syntaxTree = CSharpSyntaxTree.ParseText(content);
                var compilation = CSharpCompilation.Create("AnalysisCompilation")
                    .AddSyntaxTrees(syntaxTree)
                    .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

                var result = new CodeAnalysisResult
                {
                    FilePath = filePath,
                    AnalyzedAt = DateTime.Now,
                    Issues = new List<CodeIssue>()
                };

                // 分析语法树
                var root = await syntaxTree.GetRootAsync(cancellationToken);
                
                // 统计类数量
                result.ClassCount = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Count();
                
                // 统计方法数量
                result.MethodCount = root.DescendantNodes().OfType<MethodDeclarationSyntax>().Count();
                
                // 统计属性数量
                result.PropertyCount = root.DescendantNodes().OfType<PropertyDeclarationSyntax>().Count();

                // 简单的代码分析
                AnalyzeSyntax(root, result);

                _logger.LogInformation("文件分析完成: {FilePath}, 发现 {IssueCount} 个问题", filePath, result.Issues.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "分析文件失败: {FilePath}", filePath);
                throw;
            }
        }

        private void AnalyzeSyntax(SyntaxNode root, CodeAnalysisResult result)
        {
            // 查找潜在问题
            var issues = new List<CodeIssue>();

            // 检查未使用的变量
            foreach (var variable in root.DescendantNodes().OfType<VariableDeclarationSyntax>())
            {
                // 简单的未使用变量检查
                // 实际实现中可以使用更复杂的分析
            }

            // 检查空方法体
            foreach (var method in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
            {
                if (method.Body?.Statements.Count == 0)
                {
                    issues.Add(new CodeIssue
                    {
                        Severity = "Warning",
                        Message = "空方法体",
                        LineNumber = method.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                        Code = "EmptyMethodBody"
                    });
                }
            }

            // 检查公共字段
            foreach (var field in root.DescendantNodes().OfType<FieldDeclarationSyntax>())
            {
                if (field.Modifiers.Any(mod => mod.IsKind(SyntaxKind.PublicKeyword)))
                {
                    issues.Add(new CodeIssue
                    {
                        Severity = "Warning",
                        Message = "公共字段应改为属性",
                        LineNumber = field.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                        Code = "PublicField"
                    });
                }
            }

            result.Issues.AddRange(issues);
        }

        public Task<CodeAnalysisResult> AnalyzeProjectAsync(string projectPath, CancellationToken cancellationToken = default)
        {
            // 实现项目分析逻辑
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CodeIssue>> GetIssuesAsync(string filePath, CancellationToken cancellationToken = default)
        {
            var result = await AnalyzeFileAsync(filePath, cancellationToken);
            return result.Issues;
        }
    }

    public class SourceGeneratorService : ISourceGeneratorService
    {
        private readonly ITemplateService _templateService;
        private readonly IAttributeService _attributeService;
        private readonly ICodeAnalysisService _codeAnalysisService;
        private readonly SourceGeneratorOptions _options;
        private readonly ILogger<SourceGeneratorService> _logger;

        public SourceGeneratorService(
            ITemplateService templateService,
            IAttributeService attributeService,
            ICodeAnalysisService codeAnalysisService,
            IOptions<SourceGeneratorOptions> options,
            ILogger<SourceGeneratorService> logger)
        {
            _templateService = templateService;
            _attributeService = attributeService;
            _codeAnalysisService = codeAnalysisService;
            _options = options.Value;
            _logger = logger;
            
            // 初始化输出目录
            Directory.CreateDirectory(_options.OutputDirectory);
        }

        public async Task<string> GenerateCodeAsync(string type, string outputPath, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("生成代码类型: {Type}, 输出路径: {OutputPath}", type, outputPath);
            
            try
            {
                string code;
                switch (type.ToLower())
                {
                    case "class":
                        code = GenerateClassCode(parameters);
                        break;
                    case "interface":
                        code = GenerateInterfaceCode(parameters);
                        break;
                    case "enum":
                        code = GenerateEnumCode(parameters);
                        break;
                    case "struct":
                        code = GenerateStructCode(parameters);
                        break;
                    default:
                        throw new ArgumentException($"不支持的代码类型: {type}");
                }

                // 确保输出目录存在
                var directory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 写入文件
                await File.WriteAllTextAsync(outputPath, code, cancellationToken);
                
                _logger.LogInformation("代码生成完成: {OutputPath}", outputPath);
                return outputPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成代码失败: {Type}", type);
                throw;
            }
        }

        private string GenerateClassCode(Dictionary<string, string> parameters)
        {
            var className = parameters.GetValueOrDefault("ClassName", "GeneratedClass");
            var @namespace = parameters.GetValueOrDefault("Namespace", "Generated");
            var properties = parameters.GetValueOrDefault("Properties", "");
            
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {className}");
            sb.AppendLine("    {");
            
            // 生成属性
            if (!string.IsNullOrEmpty(properties))
            {
                var propertyList = properties.Split(',');
                foreach (var property in propertyList)
                {
                    var parts = property.Trim().Split(' ');
                    if (parts.Length >= 2)
                    {
                        var typeName = parts[0];
                        var propertyName = parts[1];
                        sb.AppendLine($"        public {typeName} {propertyName} {{ get; set; }}");
                    }
                }
            }
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }

        private string GenerateInterfaceCode(Dictionary<string, string> parameters)
        {
            var interfaceName = parameters.GetValueOrDefault("InterfaceName", "IGeneratedInterface");
            var @namespace = parameters.GetValueOrDefault("Namespace", "Generated");
            var methods = parameters.GetValueOrDefault("Methods", "");
            
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public interface {interfaceName}");
            sb.AppendLine("    {");
            
            // 生成方法
            if (!string.IsNullOrEmpty(methods))
            {
                var methodList = methods.Split(',');
                foreach (var method in methodList)
                {
                    sb.AppendLine($"        {method.Trim()};");
                }
            }
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }

        private string GenerateEnumCode(Dictionary<string, string> parameters)
        {
            var enumName = parameters.GetValueOrDefault("EnumName", "GeneratedEnum");
            var @namespace = parameters.GetValueOrDefault("Namespace", "Generated");
            var values = parameters.GetValueOrDefault("Values", "");
            
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public enum {enumName}");
            sb.AppendLine("    {");
            
            // 生成枚举值
            if (!string.IsNullOrEmpty(values))
            {
                var valueList = values.Split(',');
                for (int i = 0; i < valueList.Length; i++)
                {
                    var value = valueList[i].Trim();
                    sb.AppendLine($"        {value}{(i < valueList.Length - 1 ? "," : "")}");
                }
            }
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }

        private string GenerateStructCode(Dictionary<string, string> parameters)
        {
            var structName = parameters.GetValueOrDefault("StructName", "GeneratedStruct");
            var @namespace = parameters.GetValueOrDefault("Namespace", "Generated");
            var fields = parameters.GetValueOrDefault("Fields", "");
            
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public struct {structName}");
            sb.AppendLine("    {");
            
            // 生成字段
            if (!string.IsNullOrEmpty(fields))
            {
                var fieldList = fields.Split(',');
                foreach (var field in fieldList)
                {
                    var parts = field.Trim().Split(' ');
                    if (parts.Length >= 2)
                    {
                        var typeName = parts[0];
                        var fieldName = parts[1];
                        sb.AppendLine($"        public {typeName} {fieldName};");
                    }
                }
            }
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }

        public async Task<string> GenerateFromTemplateAsync(string templateId, string outputPath, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("从模板生成代码: {TemplateId}, 输出路径: {OutputPath}", templateId, outputPath);
            
            try
            {
                // 渲染模板
                var code = await _templateService.RenderTemplateAsync(templateId, parameters, cancellationToken);
                
                // 确保输出目录存在
                var directory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 写入文件
                await File.WriteAllTextAsync(outputPath, code, cancellationToken);
                
                _logger.LogInformation("模板代码生成完成: {OutputPath}", outputPath);
                return outputPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "从模板生成代码失败: {TemplateId}", templateId);
                throw;
            }
        }

        public Task<string> GenerateReflectionInfoAsync(string typeName, string outputPath, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("生成反射信息: {TypeName}, 输出路径: {OutputPath}", typeName, outputPath);
            
            try
            {
                // 简单的反射信息生成
                // 实际实现中可以使用更复杂的反射分析
                var code = $@"namespace ReflectionInfo
{{
    public static class {typeName}ReflectionInfo
    {{
        public static string TypeName => ""{typeName}"