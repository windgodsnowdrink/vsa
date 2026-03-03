# Serializer 技能使用示例

## 1. 基础使用示例

### 1.1 JSON 序列化和反序列化

#### 1.1.1 基本序列化

```csharp
// 创建序列化器工厂
var factory = new SerializerFactory();

// 获取 JSON 序列化器
var jsonSerializer = factory.Create("json");

// 序列化对象
var person = new { Name = "John", Age = 30, Email = "john@example.com" };
var json = await jsonSerializer.SerializeAsync(person);
Console.WriteLine("序列化结果:");
Console.WriteLine(json);

// 反序列化对象
var deserializedPerson = await jsonSerializer.DeserializeAsync<dynamic>(json);
Console.WriteLine("\n反序列化结果:");
Console.WriteLine($"Name: {deserializedPerson.Name}");
Console.WriteLine($"Age: {deserializedPerson.Age}");
Console.WriteLine($"Email: {deserializedPerson.Email}");
```

#### 1.1.2 自定义序列化选项

```csharp
// 创建自定义序列化选项
var options = new JsonSerializerOptions
{
    WriteIndented = true,
    IgnoreNullValues = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    MaxDepth = 32
};

// 创建 JSON 序列化器
var jsonSerializer = new JsonSerializerImpl(options);

// 序列化对象（包含 null 值）
var person = new { Name = "John", Age = 30, Address = (string)null };
var json = await jsonSerializer.SerializeAsync(person);
Console.WriteLine("序列化结果（忽略 null 值）:");
Console.WriteLine(json);
```

### 1.2 XML 序列化和反序列化

```csharp
// 创建序列化器工厂
var factory = new SerializerFactory();

// 获取 XML 序列化器
var xmlSerializer = factory.Create("xml");

// 序列化对象
var person = new { Name = "John", Age = 30, Email = "john@example.com" };
var xml = await xmlSerializer.SerializeAsync(person);
Console.WriteLine("序列化结果:");
Console.WriteLine(xml);

// 反序列化对象
var deserializedPerson = await xmlSerializer.DeserializeAsync<dynamic>(xml);
Console.WriteLine("\n反序列化结果:");
Console.WriteLine($"Name: {deserializedPerson.Name}");
Console.WriteLine($"Age: {deserializedPerson.Age}");
Console.WriteLine($"Email: {deserializedPerson.Email}");
```

### 1.3 YAML 序列化和反序列化

```csharp
// 创建序列化器工厂
var factory = new SerializerFactory();

// 获取 YAML 序列化器
var yamlSerializer = factory.Create("yaml");

// 序列化对象
var person = new { Name = "John", Age = 30, Email = "john@example.com" };
var yaml = await yamlSerializer.SerializeAsync(person);
Console.WriteLine("序列化结果:");
Console.WriteLine(yaml);

// 反序列化对象
var deserializedPerson = await yamlSerializer.DeserializeAsync<dynamic>(yaml);
Console.WriteLine("\n反序列化结果:");
Console.WriteLine($"Name: {deserializedPerson.Name}");
Console.WriteLine($"Age: {deserializedPerson.Age}");
Console.WriteLine($"Email: {deserializedPerson.Email}");
```

### 1.4 Protobuf 序列化和反序列化

```csharp
// 创建序列化器工厂
var factory = new SerializerFactory();

// 获取 Protobuf 序列化器
var protobufSerializer = factory.Create("protobuf");

// 定义一个可序列化的类
[ProtoBuf.ProtoContract]
public class Person
{
    [ProtoBuf.ProtoMember(1)]
    public string Name { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public int Age { get; set; }

    [ProtoBuf.ProtoMember(3)]
    public string Email { get; set; }
}

// 序列化对象
var person = new Person { Name = "John", Age = 30, Email = "john@example.com" };
var protobuf = await protobufSerializer.SerializeAsync(person);
Console.WriteLine("序列化结果 (Base64):");
Console.WriteLine(protobuf);

// 反序列化对象
var deserializedPerson = await protobufSerializer.DeserializeAsync<Person>(protobuf);
Console.WriteLine("\n反序列化结果:");
Console.WriteLine($"Name: {deserializedPerson.Name}");
Console.WriteLine($"Age: {deserializedPerson.Age}");
Console.WriteLine($"Email: {deserializedPerson.Email}");
```

## 2. 高级使用示例

### 2.1 多格式序列化比较

```csharp
// 创建序列化器工厂
var factory = new SerializerFactory();

// 定义测试对象
var person = new {
    Name = "John Doe",
    Age = 30,
    Email = "john.doe@example.com",
    Address = new {
        Street = "123 Main St",
        City = "New York",
        State = "NY",
        ZipCode = "10001"
    },
    PhoneNumbers = new [] {
        "555-1234",
        "555-5678"
    }
};

// 序列化为不同格式
var json = await factory.Create("json").SerializeAsync(person);
var xml = await factory.Create("xml").SerializeAsync(person);
var yaml = await factory.Create("yaml").SerializeAsync(person);

// 输出结果
Console.WriteLine("=== JSON 序列化 ===");
Console.WriteLine(json);
Console.WriteLine("\n=== XML 序列化 ===");
Console.WriteLine(xml);
Console.WriteLine("\n=== YAML 序列化 ===");
Console.WriteLine(yaml);
```

### 2.2 流式序列化和反序列化

```csharp
// 创建 JSON 序列化器
var jsonSerializer = new JsonSerializerImpl();

// 定义测试对象
var largeObject = new {
    Id = 1,
    Name = "Large Object",
    Data = Enumerable.Range(1, 1000).Select(i => new { Index = i, Value = $"Value {i}