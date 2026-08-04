using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;

// 防止 AOT 编译时裁剪必要的类型
[assembly: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(object))]
[assembly: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Console))]
[assembly: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonSerializer))]

namespace Serializer.Generator
{
    // 命令行选项
    public class Options
    {
        [Value(0, MetaName = "command", Required = true, HelpText = "命令名称: schema, converter, config")]
        public string Command { get; set; }

        [Option('t', "type", Required = false, HelpText = "目标类型")]
        public string Type { get; set; }

        [Option('s', "source", Required = false, HelpText = "源类型")]
        public string Source { get; set; }

        [Option('t', "target", Required = false, HelpText = "目标类型")]
        public string Target { get; set; }

        [Option('o', "output", Required = false, HelpText = "输出文件路径")]
        public string Output { get; set; }

        [Option('f', "format", Required = false, HelpText = "序列化格式: json, xml, yaml, proto")]
        public string Format { get; set; } = "json";

        [Option('n', "namespace", Required = false, HelpText = "生成代码的命名空间")]
        public string Namespace { get; set; } = "Serializer.Generators";

        [Option('c', "className", Required = false, HelpText = "生成类的名称")]
        public string ClassName { get; set; }
    }

    // 代码生成器接口
    public interface ICodeGenerator
    {
        Task<string> GenerateSchemaAsync(Type type, string format, CancellationToken cancellationToken = default);
        Task<string> GenerateConverterAsync(Type sourceType, Type targetType, CancellationToken cancellationToken = default);
        Task<string> GenerateSerializerConfigAsync(Type type, string format, CancellationToken cancellationToken = default);
    }

    // 模板管理器接口
    public interface ITemplateManager
    {
        string GetTemplate(string name);
        string RenderTemplate(string template, Dictionary<string, object> parameters);
    }

    // 模板管理器实现
    public class TemplateManager : ITemplateManager
    {
        private readonly Dictionary<string, string> _templates;

        public TemplateManager()
        {
            _templates = new Dictionary<string, string>
            {
                {
                    "JsonSchema",
                    @"{
  \"$schema\": \"http://json-schema.org/draft-07/schema#\",
  \"title\": \"{{TypeName}}\",
  \"type\": \"object\",
  \"properties\": {
{{Properties}}
  },
  \"required\": [{{RequiredProperties}}]
}"
                },
                {
                    "TypeConverter",
                    @"using System;

namespace {{Namespace}}
{
    public class {{ClassName}}
    {
        public static {{TargetType}} Convert({{SourceType}} source)
        {
            if (source == null)
            {
                return null;
            }

            return new {{TargetType}}
            {
{{PropertiesMapping}}
            };
        }

        public static {{SourceType}} ConvertBack({{TargetType}} target)
        {
            if (target == null)
            {
                return null;
            }

            return new {{SourceType}}
            {
{{ReversePropertiesMapping}}
            };
        }
    }
}"
                },
                {
                    "SerializerConfig",
                    @"using System.Text.Json;
using System.Text.Json.Serialization;

namespace {{Namespace}}
{
    public static class {{ClassName}}
    {
        public static JsonSerializerOptions CreateJsonSerializerOptions()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IgnoreNullValues = false,
                MaxDepth = 64,
                IncludeFields = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
{{Converters}}
                }
            };

            return options;
        }

        public static T SerializeDeserialize<T>(T value)
        {
            var options = CreateJsonSerializerOptions();
            var json = JsonSerializer.Serialize(value, options);
            return JsonSerializer.Deserialize<T>(json, options);
        }
    }
}"
                }
            };
        }

        public string GetTemplate(string name)
        {
            if (_templates.TryGetValue(name, out var template))
            {
                return template;
            }
            throw new NotSupportedException($"不支持的模板: {name}");
        }

        public string RenderTemplate(string template, Dictionary<string, object> parameters)
        {
            string result = template;
            foreach (var parameter in parameters)
            {
                result = result.Replace($"{{{{{parameter.Key}}}}}", parameter.Value?.ToString() ?? string.Empty);
            }
            return result;
        }
    }

    // 代码生成器实现
    public class CodeGenerator : ICodeGenerator
    {
        private readonly ITemplateManager _templateManager;

        public CodeGenerator(ITemplateManager templateManager)
        {
            _templateManager = templateManager;
        }

        public async Task<string> GenerateSchemaAsync(Type type, string format, CancellationToken cancellationToken = default)
        {
            switch (format.ToLower())
            {
                case "json":
                    return await GenerateJsonSchemaAsync(type, cancellationToken);
                case "xml":
                    return await GenerateXmlSchemaAsync(type, cancellationToken);
                case "yaml":
                    return await GenerateYamlSchemaAsync(type, cancellationToken);
                default:
                    throw new NotSupportedException($"不支持的格式: {format}");
            }
        }

        public async Task<string> GenerateConverterAsync(Type sourceType, Type targetType, CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Namespace", "Serializer.Generators" },
                { "ClassName", $"{sourceType.Name}To{targetType.Name}Converter" },
                { "SourceType", sourceType.FullName },
                { "TargetType", targetType.FullName },
                { "PropertiesMapping", GeneratePropertiesMapping(sourceType, targetType) },
                { "ReversePropertiesMapping", GeneratePropertiesMapping(targetType, sourceType) }
            };

            var template = _templateManager.GetTemplate("TypeConverter");
            return _templateManager.RenderTemplate(template, parameters);
        }

        public async Task<string> GenerateSerializerConfigAsync(Type type, string format, CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Namespace", "Serializer.Generators" },
                { "ClassName", $"{type.Name}SerializerConfig" },
                { "Converters", GenerateConverters(type) }
            };

            var template = _templateManager.GetTemplate("SerializerConfig");
            return _templateManager.RenderTemplate(template, parameters);
        }

        private async Task<string> GenerateJsonSchemaAsync(Type type, CancellationToken cancellationToken = default)
        {
            var properties = new StringBuilder();
            var requiredProperties = new List<string>();

            foreach (var property in type.GetProperties())
            {
                var propertyName = property.Name;
                var propertyType = property.PropertyType;
                var jsonType = GetJsonType(propertyType);

                properties.AppendLine($"    \"{propertyName}\": {{");
                properties.AppendLine($"      \"type\": \"{jsonType}\"");

                // 检查是否为必填属性
                if (!IsNullable(propertyType))
                {
                    requiredProperties.Add($"\"{propertyName}\");
                }

                properties.AppendLine("    },");
            }

            // 移除最后一个逗号
            if (properties.Length > 0)
            {
                properties.Length -= 2;
                properties.AppendLine();
            }

            var parameters = new Dictionary<string, object>
            {
                { "TypeName", type.Name },
                { "Properties", properties.ToString() },
                { "RequiredProperties", string.Join(", ", requiredProperties) }
            };

            var template = _templateManager.GetTemplate("JsonSchema");
            return _templateManager.RenderTemplate(template, parameters);
        }

        private async Task<string> GenerateXmlSchemaAsync(Type type, CancellationToken cancellationToken = default)
        {
            var schema = new StringBuilder();
            schema.AppendLine($"<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            schema.AppendLine($"<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">");
            schema.AppendLine($"  <xs:element name=\"{type.Name}\" type=\"{type.Name}Type\" />");
            schema.AppendLine($"  <xs:complexType name=\"{type.Name}Type\">");
            schema.AppendLine($"    <xs:sequence>");

            foreach (var property in type.GetProperties())
            {
                var propertyName = property.Name;
                var propertyType = property.PropertyType;
                var xmlType = GetXmlType(propertyType);

                schema.AppendLine($"      <xs:element name=\"{propertyName}\" type=\"{xmlType}\" />");
            }

            schema.AppendLine($"    </xs:sequence>");
            schema.AppendLine($"  </xs:complexType>");
            schema.AppendLine($"</xs:schema>");

            return schema.ToString();
        }

        private async Task<string> GenerateYamlSchemaAsync(Type type, CancellationToken cancellationToken = default)
        {
            var schema = new StringBuilder();
            schema.AppendLine($"# YAML Schema for {type.Name}");
            schema.AppendLine($"type: object");
            schema.AppendLine($"properties:");

            foreach (var property in type.GetProperties())
            {
                var propertyName = property.Name;
                var propertyType = property.PropertyType;
                var yamlType = GetYamlType(propertyType);

                schema.AppendLine($"  {propertyName}:");
                schema.AppendLine($"    type: {yamlType}");
            }

            return schema.ToString();
        }

        private string GetJsonType(Type type)
        {
            if (type == typeof(string))
                return "string";
            if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte))
                return "integer";
            if (type == typeof(double) || type == typeof(float) || type == typeof(decimal))
                return "number";
            if (type == typeof(bool))
                return "boolean";
            if (type == typeof(DateTime))
                return "string";
            if (type.IsArray || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                return "array";
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                return "object";
            return "object";
        }

        private string GetXmlType(Type type)
        {
            if (type == typeof(string))
                return "xs:string";
            if (type == typeof(int))
                return "xs:int";
            if (type == typeof(long))
                return "xs:long";
            if (type == typeof(short))
                return "xs:short";
            if (type == typeof(byte))
                return "xs:byte";
            if (type == typeof(double))
                return "xs:double";
            if (type == typeof(float))
                return "xs:float";
            if (type == typeof(decimal))
                return "xs:decimal";
            if (type == typeof(bool))
                return "xs:boolean";
            if (type == typeof(DateTime))
                return "xs:dateTime";
            return type.Name;
        }

        private string GetYamlType(Type type)
        {
            if (type == typeof(string))
                return "string";
            if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte))
                return "integer";
            if (type == typeof(double) || type == typeof(float) || type == typeof(decimal))
                return "number";
            if (type == typeof(bool))
                return "boolean";
            if (type == typeof(DateTime))
                return "timestamp";
            if (type.IsArray || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                return "array";
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                return "object";
            return "object";
        }

        private bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private string GeneratePropertiesMapping(Type sourceType, Type targetType)
        {
            var mapping = new StringBuilder();
            var sourceProperties = sourceType.GetProperties();
            var targetProperties = targetType.GetProperties();

            foreach (var targetProperty in targetProperties)
            {
                var sourceProperty = sourceProperties.FirstOrDefault(p => p.Name == targetProperty.Name);
                if (sourceProperty != null)
                {
                    mapping.AppendLine($"                {targetProperty.Name} = source.{sourceProperty.Name},");
                }
            }

            return mapping.ToString();
        }

        private string GenerateConverters(Type type)
        {
            var converters = new StringBuilder();
            // 可以根据需要添加自定义转换器
            return converters.ToString();
        }
    }

    // 主程序
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            // 解析命令行参数
            return await Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    async (Options opts) => await RunCommand(opts),
                    errors => Task.FromResult(1)
                );
        }

        private static async Task<int> RunCommand(Options opts)
        {
            try
            {
                var templateManager = new TemplateManager();
                var generator = new CodeGenerator(templateManager);

                switch (opts.Command.ToLower())
                {
                    case "schema":
                        await RunSchemaCommand(opts, generator);
                        break;
                    case "converter":
                        await RunConverterCommand(opts, generator);
                        break;
                    case "config":
                        await RunConfigCommand(opts, generator);
                        break;
                    default:
                        Console.WriteLine($"未知命令: {opts.Command}");
                        return 1;
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return 1;
            }
        }

        private static async Task RunSchemaCommand(Options opts, ICodeGenerator generator)
        {
            if (string.IsNullOrEmpty(opts.Type))
            {
                Console.WriteLine("请提供目标类型");
                return;
            }

            // 获取目标类型
            var type = Type.GetType(opts.Type);
            if (type == null)
            {
                Console.WriteLine($"无法找到类型: {opts.Type}");
                return;
            }

            // 生成模式
            var schema = await generator.GenerateSchemaAsync(type, opts.Format);

            // 输出结果
            if (!string.IsNullOrEmpty(opts.Output))
            {
                File.WriteAllText(opts.Output, schema);
                Console.WriteLine($"模式已保存到: {opts.Output}");
            }
            else
            {
                Console.WriteLine(schema);
            }
        }

        private static async Task RunConverterCommand(Options opts, ICodeGenerator generator)
        {
            if (string.IsNullOrEmpty(opts.Source))
            {
                Console.WriteLine("请提供源类型");
                return;
            }

            if (string.IsNullOrEmpty(opts.Target))
            {
                Console.WriteLine("请提供目标类型");
                return;
            }

            // 获取源类型和目标类型
            var sourceType = Type.GetType(opts.Source);
            if (sourceType == null)
            {
                Console.WriteLine($"无法找到源类型: {opts.Source}");
                return;
            }

            var targetType = Type.GetType(opts.Target);
            if (targetType == null)
            {
                Console.WriteLine($"无法找到目标类型: {opts.Target}");
                return;
            }

            // 生成转换器
            var converter = await generator.GenerateConverterAsync(sourceType, targetType);

            // 输出结果
            if (!string.IsNullOrEmpty(opts.Output))
            {
                File.WriteAllText(opts.Output, converter);
                Console.WriteLine($"转换器已保存到: {opts.Output}");
            }
            else
            {
                Console.WriteLine(converter);
            }
        }

        private static async Task RunConfigCommand(Options opts, ICodeGenerator generator)
        {
            if (string.IsNullOrEmpty(opts.Type))
            {
                Console.WriteLine("请提供目标类型");
                return;
            }

            // 获取目标类型
            var type = Type.GetType(opts.Type);
            if (type == null)
            {
                Console.WriteLine($"无法找到类型: {opts.Type}");
                return;
            }

            // 生成序列化器配置
            var config = await generator.GenerateSerializerConfigAsync(type, opts.Format);

            // 输出结果
            if (!string.IsNullOrEmpty(opts.Output))
            {
                File.WriteAllText(opts.Output, config);
                Console.WriteLine($"序列化器配置已保存到: {opts.Output}");
            }
            else
            {
                Console.WriteLine(config);
            }
        }
    }
}
