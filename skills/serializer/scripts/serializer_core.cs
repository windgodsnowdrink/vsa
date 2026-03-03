using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

// 防止 AOT 编译时裁剪必要的类型
[assembly: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(object))]
[assembly: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Console))]
[assembly: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonSerializer))]
[assembly: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Newtonsoft.Json.JsonSerializer))]
[assembly: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(YamlDotNet.Serialization.Serializer))]

namespace Serializer
{
    // 命令行选项
    public class Options
    {
        [Value(0, MetaName = "command", Required = true, HelpText = "命令名称: json, xml, yaml, protobuf, generate, test")]
        public string Command { get; set; }

        [Value(1, MetaName = "subcommand", Required = false, HelpText = "子命令: serialize, deserialize, schema, converter, performance")]
        public string Subcommand { get; set; }

        [Option('i', "input", Required = false, HelpText = "输入文件路径或内容")]
        public string Input { get; set; }

        [Option('o', "output", Required = false, HelpText = "输出文件路径")]
        public string Output { get; set; }

        [Option('t', "type", Required = false, HelpText = "目标类型")]
        public string Type { get; set; }

        [Option('s', "source", Required = false, HelpText = "源类型")]
        public string Source { get; set; }

        [Option('t', "target", Required = false, HelpText = "目标类型")]
        public string Target { get; set; }

        [Option('f', "formats", Required = false, HelpText = "测试的序列化格式 (json,xml,yaml,proto)")]
        public string Formats { get; set; }

        [Option('n', "iterations", Required = false, HelpText = "迭代次数")]
        public int Iterations { get; set; } = 10000;
    }

    // 序列化器接口
    public interface ISerializer
    {
        Task<string> SerializeAsync<T>(T value, CancellationToken cancellationToken = default);
        Task<T> DeserializeAsync<T>(string input, CancellationToken cancellationToken = default);
        Task<byte[]> SerializeToBytesAsync<T>(T value, CancellationToken cancellationToken = default);
        Task<T> DeserializeFromBytesAsync<T>(byte[] input, CancellationToken cancellationToken = default);
    }

    // 序列化器工厂接口
    public interface ISerializerFactory
    {
        ISerializer Create(string format);
        IEnumerable<string> GetSupportedFormats();
    }

    // 性能测试结果
    public class PerformanceResult
    {
        public List<string> Formats { get; set; } = new List<string>();
        public Dictionary<string, long> ExecutionTimes { get; set; } = new Dictionary<string, long>();
        public Dictionary<string, long> MemoryUsage { get; set; } = new Dictionary<string, long>();

        public double GetAverageTime(string format)
        {
            if (ExecutionTimes.TryGetValue(format, out var time))
            {
                return time / 1000000.0; // 转换为毫秒
            }
            return 0;
        }

        public long GetMemoryUsage(string format)
        {
            MemoryUsage.TryGetValue(format, out var memory);
            return memory;
        }
    }

    // 性能测试器接口
    public interface IPerformanceTester
    {
        Task<PerformanceResult> TestPerformanceAsync<T>(int iterations, params string[] formats);
        Task<PerformanceResult> TestPerformanceAsync(Type type, int iterations, params string[] formats);
    }

    // JSON 序列化器实现
    public class JsonSerializerImpl : ISerializer
    {
        private readonly JsonSerializerOptions _options;

        public JsonSerializerImpl(JsonSerializerOptions options = null)
        {
            _options = options ?? new JsonSerializerOptions
            {
                WriteIndented = true,
                IgnoreNullValues = false,
                MaxDepth = 64,
                IncludeFields = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<string> SerializeAsync<T>(T value, CancellationToken cancellationToken = default)
        {
            return JsonSerializer.Serialize(value, _options);
        }

        public async Task<T> DeserializeAsync<T>(string input, CancellationToken cancellationToken = default)
        {
            return JsonSerializer.Deserialize<T>(input, _options);
        }

        public async Task<byte[]> SerializeToBytesAsync<T>(T value, CancellationToken cancellationToken = default)
        {
            return JsonSerializer.SerializeToUtf8Bytes(value, _options);
        }

        public async Task<T> DeserializeFromBytesAsync<T>(byte[] input, CancellationToken cancellationToken = default)
        {
            return JsonSerializer.Deserialize<T>(input, _options);
        }
    }

    // Newtonsoft JSON 序列化器实现
    public class NewtonsoftJsonSerializerImpl : ISerializer
    {
        private readonly JsonSerializerSettings _settings;

        public NewtonsoftJsonSerializerImpl(JsonSerializerSettings settings = null)
        {
            _settings = settings ?? new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include,
                MaxDepth = 64,
                TypeNameHandling = TypeNameHandling.None
            };
        }

        public async Task<string> SerializeAsync<T>(T value, CancellationToken cancellationToken = default)
        {
            return JsonConvert.SerializeObject(value, _settings);
        }

        public async Task<T> DeserializeAsync<T>(string input, CancellationToken cancellationToken = default)
        {
            return JsonConvert.DeserializeObject<T>(input, _settings);
        }

        public async Task<byte[]> SerializeToBytesAsync<T>(T value, CancellationToken cancellationToken = default)
        {
            var json = JsonConvert.SerializeObject(value, _settings);
            return Encoding.UTF8.GetBytes(json);
        }

        public async Task<T> DeserializeFromBytesAsync<T>(byte[] input, CancellationToken cancellationToken = default)
        {
            var json = Encoding.UTF8.GetString(input);
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }
    }

    // XML 序列化器实现
    public class XmlSerializerImpl : ISerializer
    {
        public async Task<string> SerializeAsync<T>(T value, CancellationToken cancellationToken = default)
        {
            using var stringWriter = new StringWriter();
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            serializer.Serialize(stringWriter, value);
            return stringWriter.ToString();
        }

        public async Task<T> DeserializeAsync<T>(string input, CancellationToken cancellationToken = default)
        {
            using var stringReader = new StringReader(input);
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stringReader);
        }

        public async Task<byte[]> SerializeToBytesAsync<T>(T value, CancellationToken cancellationToken = default)
        {
            using var memoryStream = new MemoryStream();
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            serializer.Serialize(memoryStream, value);
            return memoryStream.ToArray();
        }

        public async Task<T> DeserializeFromBytesAsync<T>(byte[] input, CancellationToken cancellationToken = default)
        {
            using var memoryStream = new MemoryStream(input);
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(memoryStream);
        }
    }

    // YAML 序列化器实现
    public class YamlSerializerImpl : ISerializer
    {
        private readonly ISerializer _serializer;
        private readonly IDeserializer _deserializer;

        public YamlSerializerImpl()
        {
            _serializer = new YamlDotNet.Serialization.SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            _deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
        }

        public async Task<string> SerializeAsync<T>(T value, CancellationToken cancellationToken = default)
        {
            return _serializer.Serialize(value);
        }

        public async Task<T> DeserializeAsync<T>(string input, CancellationToken cancellationToken = default)
        {
            return _deserializer.Deserialize<T>(input);
        }

        public async Task<byte[]> SerializeToBytesAsync<T>(T value, CancellationToken cancellationToken = default)
        {
            var yaml = _serializer.Serialize(value);
            return Encoding.UTF8.GetBytes(yaml);
        }

        public async Task<T> DeserializeFromBytesAsync<T>(byte[] input, CancellationToken cancellationToken = default)
        {
            var yaml = Encoding.UTF8.GetString(input);
            return _deserializer.Deserialize<T>(yaml);
        }
    }

    // Protobuf 序列化器实现
    public class ProtobufSerializerImpl : ISerializer
    {
        public async Task<string> SerializeAsync<T>(T value, CancellationToken cancellationToken = default)
        {
            using var memoryStream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(memoryStream, value);
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public async Task<T> DeserializeAsync<T>(string input, CancellationToken cancellationToken = default)
        {
            var bytes = Convert.FromBase64String(input);
            using var memoryStream = new MemoryStream(bytes);
            return ProtoBuf.Serializer.Deserialize<T>(memoryStream);
        }

        public async Task<byte[]> SerializeToBytesAsync<T>(T value, CancellationToken cancellationToken = default)
        {
            using var memoryStream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(memoryStream, value);
            return memoryStream.ToArray();
        }

        public async Task<T> DeserializeFromBytesAsync<T>(byte[] input, CancellationToken cancellationToken = default)
        {
            using var memoryStream = new MemoryStream(input);
            return ProtoBuf.Serializer.Deserialize<T>(memoryStream);
        }
    }

    // 序列化器工厂实现
    public class SerializerFactory : ISerializerFactory
    {
        private readonly Dictionary<string, Func<ISerializer>> _serializers;

        public SerializerFactory()
        {
            _serializers = new Dictionary<string, Func<ISerializer>>(StringComparer.OrdinalIgnoreCase)
            {
                { "json", () => new JsonSerializerImpl() },
                { "newtonsoft", () => new NewtonsoftJsonSerializerImpl() },
                { "xml", () => new XmlSerializerImpl() },
                { "yaml", () => new YamlSerializerImpl() },
                { "proto", () => new ProtobufSerializerImpl() },
                { "protobuf", () => new ProtobufSerializerImpl() }
            };
        }

        public ISerializer Create(string format)
        {
            if (_serializers.TryGetValue(format, out var factory))
            {
                return factory();
            }
            throw new NotSupportedException($"不支持的序列化格式: {format}");
        }

        public IEnumerable<string> GetSupportedFormats()
        {
            return _serializers.Keys;
        }
    }

    // 性能测试器实现
    public class PerformanceTester : IPerformanceTester
    {
        private readonly ISerializerFactory _factory;

        public PerformanceTester(ISerializerFactory factory)
        {
            _factory = factory;
        }

        public async Task<PerformanceResult> TestPerformanceAsync<T>(int iterations, params string[] formats)
        {
            return await TestPerformanceAsync(typeof(T), iterations, formats);
        }

        public async Task<PerformanceResult> TestPerformanceAsync(Type type, int iterations, params string[] formats)
        {
            var result = new PerformanceResult();
            var testData = CreateTestData(type);

            foreach (var format in formats)
            {
                try
                {
                    var serializer = _factory.Create(format);
                    var serializeMethod = GetSerializeMethod(serializer, type);
                    var deserializeMethod = GetDeserializeMethod(serializer, type);

                    // 预热
                    for (int i = 0; i < 1000; i++)
                    {
                        await serializeMethod.Invoke(serializer, new object[] { testData, CancellationToken.None });
                    }

                    // 测试序列化性能
                    var startTime = DateTime.Now.Ticks;
                    for (int i = 0; i < iterations; i++)
                    {
                        await serializeMethod.Invoke(serializer, new object[] { testData, CancellationToken.None });
                    }
                    var endTime = DateTime.Now.Ticks;
                    var serializeTime = endTime - startTime;

                    // 序列化一次获取结果用于反序列化测试
                    var serializedData = await serializeMethod.Invoke(serializer, new object[] { testData, CancellationToken.None });

                    // 测试反序列化性能
                    startTime = DateTime.Now.Ticks;
                    for (int i = 0; i < iterations; i++)
                    {
                        await deserializeMethod.Invoke(serializer, new object[] { serializedData, CancellationToken.None });
                    }
                    endTime = DateTime.Now.Ticks;
                    var deserializeTime = endTime - startTime;

                    result.Formats.Add(format);
                    result.ExecutionTimes[format] = serializeTime + deserializeTime;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"测试 {format} 时出错: {ex.Message}");
                }
            }

            return result;
        }

        private object CreateTestData(Type type)
        {
            if (type == typeof(string))
            {
                return "This is a test string for serialization performance testing.";
            }
            else if (type == typeof(int))
            {
                return 42;
            }
            else if (type == typeof(double))
            {
                return 3.14159;
            }
            else if (type == typeof(bool))
            {
                return true;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elementType = type.GetGenericArguments()[0];
                var list = Activator.CreateInstance(type) as System.Collections.IList;
                for (int i = 0; i < 100; i++)
                {
                    list.Add(CreateTestData(elementType));
                }
                return list;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var keyType = type.GetGenericArguments()[0];
                var valueType = type.GetGenericArguments()[1];
                var dict = Activator.CreateInstance(type) as System.Collections.IDictionary;
                for (int i = 0; i < 50; i++)
                {
                    dict.Add(CreateTestData(keyType), CreateTestData(valueType));
                }
                return dict;
            }
            else
            {
                // 对于其他类型，返回默认值
                return Activator.CreateInstance(type);
            }
        }

        private MethodInfo GetSerializeMethod(ISerializer serializer, Type type)
        {
            var method = typeof(ISerializer).GetMethod("SerializeAsync").MakeGenericMethod(type);
            return method;
        }

        private MethodInfo GetDeserializeMethod(ISerializer serializer, Type type)
        {
            var method = typeof(ISerializer).GetMethod("DeserializeAsync").MakeGenericMethod(type);
            return method;
        }
    }

    // 序列化器扩展
    public static class SerializerExtensions
    {
        public static IServiceCollection AddSerializer(this IServiceCollection services)
        {
            services.AddSingleton<ISerializerFactory, SerializerFactory>();
            services.AddSingleton<IPerformanceTester, PerformanceTester>();
            return services;
        }

        public static IServiceCollection ConfigureSerializer(this IServiceCollection services, Action<JsonSerializerOptions> configure)
        {
            services.Configure(configure);
            return services;
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
                var factory = new SerializerFactory();
                var tester = new PerformanceTester(factory);

                switch (opts.Command.ToLower())
                {
                    case "json":
                        await RunJsonCommand(opts, factory);
                        break;
                    case "xml":
                        await RunXmlCommand(opts, factory);
                        break;
                    case "yaml":
                        await RunYamlCommand(opts, factory);
                        break;
                    case "protobuf":
                    case "proto":
                        await RunProtobufCommand(opts, factory);
                        break;
                    case "test":
                        await RunTestCommand(opts, tester);
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

        private static async Task RunJsonCommand(Options opts, ISerializerFactory factory)
        {
            var serializer = factory.Create("json");
            await RunSerializeDeserializeCommand(opts, serializer);
        }

        private static async Task RunXmlCommand(Options opts, ISerializerFactory factory)
        {
            var serializer = factory.Create("xml");
            await RunSerializeDeserializeCommand(opts, serializer);
        }

        private static async Task RunYamlCommand(Options opts, ISerializerFactory factory)
        {
            var serializer = factory.Create("yaml");
            await RunSerializeDeserializeCommand(opts, serializer);
        }

        private static async Task RunProtobufCommand(Options opts, ISerializerFactory factory)
        {
            var serializer = factory.Create("protobuf");
            await RunSerializeDeserializeCommand(opts, serializer);
        }

        private static async Task RunSerializeDeserializeCommand(Options opts, ISerializer serializer)
        {
            switch (opts.Subcommand?.ToLower())
            {
                case "serialize":
                    await RunSerializeCommand(opts, serializer);
                    break;
                case "deserialize":
                    await RunDeserializeCommand(opts, serializer);
                    break;
                default:
                    Console.WriteLine($"未知子命令: {opts.Subcommand}");
                    break;
            }
        }

        private static async Task RunSerializeCommand(Options opts, ISerializer serializer)
        {
            if (string.IsNullOrEmpty(opts.Input))
            {
                Console.WriteLine("请提供输入内容或文件路径");
                return;
            }

            // 读取输入
            string input = opts.Input;
            if (File.Exists(opts.Input))
            {
                input = File.ReadAllText(opts.Input);
            }

            // 尝试将输入解析为对象
            object data;
            try
            {
                // 假设输入是 JSON 格式
                data = JsonSerializer.Deserialize<dynamic>(input);
            }
            catch
            {
                // 如果不是 JSON，作为字符串处理
                data = input;
            }

            // 序列化
            var serialized = await serializer.SerializeAsync(data);

            // 输出结果
            if (!string.IsNullOrEmpty(opts.Output))
            {
                File.WriteAllText(opts.Output, serialized);
                Console.WriteLine($"序列化结果已保存到: {opts.Output}");
            }
            else
            {
                Console.WriteLine(serialized);
            }
        }

        private static async Task RunDeserializeCommand(Options opts, ISerializer serializer)
        {
            if (string.IsNullOrEmpty(opts.Input))
            {
                Console.WriteLine("请提供输入内容或文件路径");
                return;
            }

            if (string.IsNullOrEmpty(opts.Type))
            {
                Console.WriteLine("请提供目标类型");
                return;
            }

            // 读取输入
            string input = opts.Input;
            if (File.Exists(opts.Input))
            {
                input = File.ReadAllText(opts.Input);
            }

            // 获取目标类型
            var type = Type.GetType(opts.Type);
            if (type == null)
            {
                Console.WriteLine($"无法找到类型: {opts.Type}");
                return;
            }

            // 反序列化
            var deserializeMethod = typeof(ISerializer).GetMethod("DeserializeAsync").MakeGenericMethod(type);
            var result = await deserializeMethod.Invoke(serializer, new object[] { input, CancellationToken.None });

            // 输出结果
            if (!string.IsNullOrEmpty(opts.Output))
            {
                var jsonResult = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(opts.Output, jsonResult);
                Console.WriteLine($"反序列化结果已保存到: {opts.Output}");
            }
            else
            {
                var jsonResult = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(jsonResult);
            }
        }

        private static async Task RunTestCommand(Options opts, IPerformanceTester tester)
        {
            if (opts.Subcommand?.ToLower() != "performance")
            {
                Console.WriteLine($"未知测试子命令: {opts.Subcommand}");
                return;
            }

            if (string.IsNullOrEmpty(opts.Type))
            {
                Console.WriteLine("请提供测试类型");
                return;
            }

            if (string.IsNullOrEmpty(opts.Formats))
            {
                Console.WriteLine("请提供测试的序列化格式");
                return;
            }

            // 获取测试类型
            var type = Type.GetType(opts.Type);
            if (type == null)
            {
                Console.WriteLine($"无法找到类型: {opts.Type}");
                return;
            }

            // 解析测试格式
            var formats = opts.Formats.Split(',').Select(f => f.Trim()).ToArray();

            // 运行性能测试
            Console.WriteLine($"开始测试 {opts.Type} 类型的序列化性能...");
            var result = await tester.TestPerformanceAsync(type, opts.Iterations, formats);

            // 输出结果
            Console.WriteLine("\n性能测试结果:");
            Console.WriteLine("-" + new string('-', 50) + "-");
            Console.WriteLine($"{"格式",-10} {"平均时间 (ms)",-20} {"内存使用 (bytes)",-20}");
            Console.WriteLine("-" + new string('-', 50) + "-");

            foreach (var format in result.Formats)
            {
                var avgTime = result.GetAverageTime(format);
                var memory = result.GetMemoryUsage(format);
                Console.WriteLine($"{format,-10} {avgTime,-20:F4} {memory,-20}");
            }

            Console.WriteLine("-" + new string('-', 50) + "-");
        }
    }
}
