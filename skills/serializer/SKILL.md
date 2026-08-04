# Serializer 技能文档

## 1. 技能概述

Serializer 是一个强大的序列化工具，支持多种序列化格式，提供代码生成功能，帮助开发者快速实现数据序列化和反序列化操作。

### 1.1 主要功能

- **多格式支持**：支持 JSON、XML、YAML、Protobuf 等多种序列化格式
- **高性能**：针对不同场景优化序列化性能，支持 AOT 编译
- **代码生成**：自动生成序列化相关的代码，如类型转换器、序列化模式等
- **灵活配置**：提供丰富的配置选项，满足不同场景的需求
- **依赖注入**：集成 Microsoft.Extensions.DependencyInjection，支持依赖注入
- **跨平台**：支持 Windows、Linux、macOS 等多种平台

### 1.2 适用场景

- **API 开发**：序列化和反序列化 API 请求/响应数据
- **配置管理**：读取和写入配置文件
- **数据存储**：将对象序列化为持久化格式
- **网络通信**：在网络传输中序列化数据
- **微服务**：在微服务之间传递数据
- **性能测试**：测试不同序列化格式的性能

## 2. 快速开始

### 2.1 安装

1. 确保已安装 .NET 10 SDK
2. 克隆或下载本技能到本地
3. 进入 `serializer/scripts` 目录

### 2.2 编译

使用以下命令编译脚本：

```bash
# 编译 serializer_core.cs
dotnet publish serializer_core.cs -c Release -o publish --self-contained true --publish-single-file true -r win-x64 -p:PublishAot=true -p:TrimMode=partial

# 编译 serializer_generator.cs
dotnet publish serializer_generator.cs -c Release -o publish --self-contained true --publish-single-file true -r win-x64 -p:PublishAot=true -p:TrimMode=partial
```

### 2.3 运行

```bash
# 运行 JSON 序列化
./serializer_core.exe json serialize --input "{\"name\": \"John\", \"age\": 30}" --output output.json

# 运行 JSON 反序列化
./serializer_core.exe json deserialize --input input.json --type "System.Collections.Generic.Dictionary`2[[System.String],[System.Object]]"

# 运行性能测试
./serializer_core.exe test performance --type "System.Collections.Generic.List`1[[System.String]]" --iterations 10000 --formats "json,xml,yaml"
```

## 3. 核心功能

### 3.1 序列化和反序列化

#### JSON 序列化

- 使用 `System.Text.Json` 和 `Newtonsoft.Json` 提供高性能 JSON 序列化
- 支持自定义序列化选项，如缩进、忽略空值等
- 支持循环引用处理

#### XML 序列化

- 使用 `System.Xml.Serialization` 提供 XML 序列化
- 支持自定义 XML 命名空间和属性

#### YAML 序列化

- 使用 `YamlDotNet` 提供 YAML 序列化
- 支持 YAML 1.2 规范

#### Protobuf 序列化

- 使用 `protobuf-net` 提供 Protobuf 序列化
- 支持二进制格式，适用于高性能场景

### 3.2 代码生成

- **类型转换器**：生成不同类型之间的转换代码
- **序列化模式**：生成 JSON Schema、XML Schema 等
- **序列化配置**：生成序列化器配置代码

### 3.3 性能测试

- 测试不同序列化格式的性能
- 生成详细的性能报告
- 支持自定义测试类型和迭代次数

### 3.4 依赖注入

- 集成 Microsoft.Extensions.DependencyInjection
- 提供序列化器的依赖注入扩展方法
- 支持在 ASP.NET Core 应用中使用

## 4. API 参考

### 4.1 命令行接口

#### json 命令

- `json serialize`：序列化对象为 JSON
- `json deserialize`：反序列化 JSON 为对象

#### xml 命令

- `xml serialize`：序列化对象为 XML
- `xml deserialize`：反序列化 XML 为对象

#### yaml 命令

- `yaml serialize`：序列化对象为 YAML
- `yaml deserialize`：反序列化 YAML 为对象

#### protobuf 命令

- `protobuf serialize`：序列化对象为 Protobuf
- `protobuf deserialize`：反序列化 Protobuf 为对象

#### generate 命令

- `generate schema`：生成序列化模式（如 JSON Schema）
- `generate converter`：生成类型转换器

#### test 命令

- `test performance`：测试序列化性能

### 4.2 核心接口

#### ISerializer

```csharp
public interface ISerializer
{
    Task<string> SerializeAsync<T>(T value, CancellationToken cancellationToken = default);
    Task<T> DeserializeAsync<T>(string input, CancellationToken cancellationToken = default);
    Task<byte[]> SerializeToBytesAsync<T>(T value, CancellationToken cancellationToken = default);
    Task<T> DeserializeFromBytesAsync<T>(byte[] input, CancellationToken cancellationToken = default);
}
```

#### ISerializerFactory

```csharp
public interface ISerializerFactory
{
    ISerializer Create(string format);
    IEnumerable<string> GetSupportedFormats();
}
```

#### ICodeGenerator

```csharp
public interface ICodeGenerator
{
    Task<string> GenerateSchemaAsync(Type type, string format, CancellationToken cancellationToken = default);
    Task<string> GenerateConverterAsync(Type sourceType, Type targetType, CancellationToken cancellationToken = default);
    Task<string> GenerateSerializerConfigAsync(Type type, string format, CancellationToken cancellationToken = default);
}
```

#### IPerformanceTester

```csharp
public interface IPerformanceTester
{
    Task<PerformanceResult> TestPerformanceAsync<T>(int iterations, params string[] formats);
    Task<PerformanceResult> TestPerformanceAsync(Type type, int iterations, params string[] formats);
}
```

### 4.3 配置选项

#### 序列化配置

| 选项 | 描述 | 默认值 |
|------|------|--------|
| Indented | 是否缩进输出 | true |
| IgnoreNullValues | 是否忽略空值 | false |
| MaxDepth | 最大序列化深度 | 64 |
| IncludeFields | 是否包含字段 | false |
| DateTimeFormat | 日期时间格式 | "yyyy-MM-ddTHH:mm:ss.fffffffK" |

#### 性能测试配置

| 选项 | 描述 | 默认值 |
|------|------|--------|
| WarmupIterations | 预热迭代次数 | 1000 |
| TestIterations | 测试迭代次数 | 10000 |
| ConcurrentThreads | 并发线程数 | 1 |
| MemoryMeasurement | 是否测量内存使用 | false |

## 5. AOT 编译

### 5.1 配置

Serializer 技能支持 AOT 编译，通过以下配置启用：

- `PublishAot=true`：启用 AOT 编译
- `TrimMode=partial`：部分裁剪，保留必要的类型
- `SelfContained=true`：自包含发布
- `PublishSingleFile=true`：发布为单个文件

### 5.2 优势

- **启动速度快**：AOT 编译的代码启动更快
- **内存使用少**：裁剪后的代码内存使用更少
- **部署简单**：单个可执行文件，无需安装 .NET 运行时
- **安全性高**：减少了可攻击面

### 5.3 注意事项

- **反射限制**：AOT 编译对反射有一定限制，需要确保所有使用反射的类型都被正确保留
- **动态类型**：动态类型在 AOT 编译中可能无法正常工作
- **序列化选项**：某些序列化选项可能在 AOT 编译中不可用

## 6. 示例

### 6.1 基本序列化

```csharp
// 创建序列化器工厂
var factory = new SerializerFactory();

// 获取 JSON 序列化器
var jsonSerializer = factory.Create("json");

// 序列化对象
var person = new { Name = "John", Age = 30 };
var json = await jsonSerializer.SerializeAsync(person);
Console.WriteLine(json);

// 反序列化对象
var deserializedPerson = await jsonSerializer.DeserializeAsync<dynamic>(json);
Console.WriteLine($"Name: {deserializedPerson.Name}, Age: {deserializedPerson.Age}");
```

### 6.2 自定义序列化选项

```csharp
// 创建自定义序列化选项
var options = new JsonSerializerOptions
{
    WriteIndented = true,
    IgnoreNullValues = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

// 创建序列化器
var jsonSerializer = new JsonSerializerImpl(options);

// 序列化对象
var person = new { FirstName = "John", LastName = "Doe", Age = 30, Address = (string)null };
var json = await jsonSerializer.SerializeAsync(person);
Console.WriteLine(json);
```

### 6.3 性能测试

```csharp
// 创建性能测试器
var tester = new PerformanceTester(new SerializerFactory());

// 测试性能
var result = await tester.TestPerformanceAsync<List<string>>(
    iterations: 10000,
    formats: new[] { "json", "xml", "yaml" }
);

// 输出结果
Console.WriteLine("Performance Test Results:");
foreach (var format in result.Formats)
{
    Console.WriteLine($"{format}: {result.GetAverageTime(format):F4} ms");
}
```

### 6.4 代码生成

```csharp
// 创建代码生成器
var generator = new CodeGenerator();

// 生成 JSON Schema
var schema = await generator.GenerateSchemaAsync(typeof(Person), "json");
Console.WriteLine(schema);

// 生成类型转换器
var converter = await generator.GenerateConverterAsync(typeof(Person), typeof(PersonDto));
Console.WriteLine(converter);
```

## 7. 高级功能

### 7.1 自定义序列化器

可以通过实现 `ISerializer` 接口来创建自定义序列化器：

```csharp
public class CustomSerializer : ISerializer
{
    public Task<string> SerializeAsync<T>(T value, CancellationToken cancellationToken = default)
    {
        // 实现自定义序列化逻辑
        return Task.FromResult(JsonSerializer.Serialize(value));
    }

    public Task<T> DeserializeAsync<T>(string input, CancellationToken cancellationToken = default)
    {
        // 实现自定义反序列化逻辑
        return Task.FromResult(JsonSerializer.Deserialize<T>(input));
    }

    public Task<byte[]> SerializeToBytesAsync<T>(T value, CancellationToken cancellationToken = default)
    {
        // 实现自定义字节序列化逻辑
        var json = JsonSerializer.Serialize(value);
        return Task.FromResult(Encoding.UTF8.GetBytes(json));
    }

    public Task<T> DeserializeFromBytesAsync<T>(byte[] input, CancellationToken cancellationToken = default)
    {
        // 实现自定义字节反序列化逻辑
        var json = Encoding.UTF8.GetString(input);
        return Task.FromResult(JsonSerializer.Deserialize<T>(json));
    }
}
```

### 7.2 依赖注入集成

在 ASP.NET Core 应用中集成：

```csharp
var builder = WebApplication.CreateBuilder(args);

// 添加序列化器
builder.Services.AddSerializer();

// 配置序列化选项
builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.WriteIndented = true;
    options.IgnoreNullValues = true;
});

var app = builder.Build();

// 使用序列化器
app.MapGet("/serialize", async (ISerializerFactory factory) =>
{
    var serializer = factory.Create("json");
    var data = new { Message = "Hello, World!" };
    return await serializer.SerializeAsync(data);
});

app.Run();
```

### 7.3 多格式序列化

```csharp
// 创建序列化器工厂
var factory = new SerializerFactory();

// 序列化对象为多种格式
var person = new { Name = "John", Age = 30 };

var json = await factory.Create("json").SerializeAsync(person);
var xml = await factory.Create("xml").SerializeAsync(person);
var yaml = await factory.Create("yaml").SerializeAsync(person);

Console.WriteLine("JSON:");
Console.WriteLine(json);
Console.WriteLine("\nXML:");
Console.WriteLine(xml);
Console.WriteLine("\nYAML:");
Console.WriteLine(yaml);
```

## 8. 故障排除

### 8.1 常见问题

| 问题 | 原因 | 解决方案 |
|------|------|----------|
| 序列化失败 | 类型不支持序列化 | 确保类型是可序列化的，或使用自定义序列化器 |
| 反序列化失败 | JSON/XML/YAML 格式不正确 | 检查输入格式是否正确，或使用错误处理 |
| AOT 编译错误 | 反射类型未被保留 | 在代码中显式引用类型，或使用 `DynamicDependency` 属性 |
| 性能测试缓慢 | 测试类型过大或迭代次数过多 | 减少测试类型大小，或减少迭代次数 |
| 内存使用高 | 序列化大型对象 | 分块处理大型对象，或使用流式序列化 |

### 8.2 日志

Serializer 技能使用 `Microsoft.Extensions.Logging` 提供日志记录，可通过以下方式配置：

```csharp
// 配置日志
var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});

// 创建序列化器工厂
var factory = new SerializerFactory(loggerFactory);
```

## 9. 性能优化

### 9.1 序列化性能

- **使用 Protobuf**：对于性能敏感的场景，使用 Protobuf 序列化
- **缓存序列化器**：重用序列化器实例，避免重复创建
- **配置优化**：根据需要调整序列化选项，如关闭缩进、启用压缩等
- **类型优化**：使用简单类型，避免复杂的对象图

### 9.2 内存使用

- **流式处理**：对于大型对象，使用流式序列化和反序列化
- **对象池**：使用对象池重用序列化过程中的临时对象
- **内存分配**：减少序列化过程中的内存分配，使用 `Span<T>` 和 `Memory<T>`

### 9.3 启动速度

- **AOT 编译**：使用 AOT 编译提高启动速度
- **预热**：在应用启动时预热序列化器
- **延迟加载**：延迟加载不常用的序列化器

## 10. 总结

Serializer 技能是一个功能强大、性能优异的序列化工具，支持多种序列化格式，提供代码生成功能，支持 AOT 编译，适用于各种序列化场景。通过本技能，开发者可以快速实现数据序列化和反序列化操作，提高开发效率，优化应用性能。

### 10.1 关键特性

- **多格式支持**：JSON、XML、YAML、Protobuf
- **高性能**：优化的序列化和反序列化算法
- **代码生成**：自动生成序列化相关代码
- **AOT 编译**：支持 AOT 编译，提高启动速度和运行时性能
- **跨平台**：支持多种平台
- **灵活配置**：丰富的配置选项
- **依赖注入**：集成 Microsoft.Extensions.DependencyInjection

### 10.2 未来规划

- **更多序列化格式**：支持 MessagePack、BSON 等更多序列化格式
- **更多代码生成功能**：生成更多类型的序列化相关代码
- **更高级的性能测试**：提供更详细的性能分析报告
- **更丰富的集成**：与更多框架和库集成
- **更完善的文档**：提供更详细的使用文档和示例

## 11. 参考资料

- [System.Text.Json 文档](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/)
- [Newtonsoft.Json 文档](https://www.newtonsoft.com/json/help/html/Introduction.htm)
- [YamlDotNet 文档](https://aaubry.net/pages/yamldotnet.html)
- [protobuf-net 文档](https://protobuf-net.github.io/protobuf-net/)
- [.NET AOT 文档](https://docs.microsoft.com/en-us/dotnet/core/deploying/native-aot/)
- [ASP.NET Core 序列化](https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-8.0)
