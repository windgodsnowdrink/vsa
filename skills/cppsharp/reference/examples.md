# CppSharp AOT 使用示例

## 1. 基本使用示例

### 1.1 生成简单绑定

```csharp
// 生成简单绑定
var builder = Host.CreateApplicationBuilder();
builder.Services.Configure<CppSharp.AOT.CppSharpOptions>(options =>
{
    options.Namespace = "MyApp.Generated";
    options.GenerateAsyncWrappers = true;
});
builder.Services.AddSingleton<CppSharp.AOT.ICppSharpService, CppSharp.AOT.CppSharpService>();

var host = builder.Build();
var cppSharpService = host.Services.GetRequiredService<CppSharp.AOT.ICppSharpService>();

// 生成绑定
var result = await cppSharpService.GenerateBindingsAsync(
    "simple.h", 
    "SimpleBindings.cs"
);

Console.WriteLine($"绑定生成结果: {(result ? "成功" : "失败")}");
```

### 1.2 使用配置文件

```csharp
// 使用配置文件
var builder = Host.CreateApplicationBuilder();
builder.Configuration.AddJsonFile("cppsharp_aot.setting.json");
builder.Services.Configure<CppSharp.AOT.CppSharpOptions>(builder.Configuration.GetSection("CppSharp"));
builder.Services.AddSingleton<CppSharp.AOT.ICppSharpService, CppSharp.AOT.CppSharpService>();

var host = builder.Build();
var cppSharpService = host.Services.GetRequiredService<CppSharp.AOT.ICppSharpService>();

// 生成绑定
var result = await cppSharpService.GenerateBindingsAsync(
    "simple.h", 
    "SimpleBindings.cs"
);
```

## 2. 命令行使用示例

### 2.1 基本命令

```bash
# 生成绑定
cppsharp_aot.exe generate input.h output.cs

# 指定命名空间
cppsharp_aot.exe generate input.h output.cs MyNamespace
```

### 2.2 高级命令

```bash
# 使用自定义配置文件
cppsharp_aot.exe generate input.h output.cs --config custom_config.json

# 生成调试信息
cppsharp_aot.exe generate input.h output.cs --debug

# 禁用异步包装器
cppsharp_aot.exe generate input.h output.cs --no-async
```

## 3. 分步使用示例

### 3.1 解析C++头文件

```csharp
// 解析C++头文件
var builder = Host.CreateApplicationBuilder();
builder.Services.AddSingleton<CppSharp.AOT.ICppSharpService, CppSharp.AOT.CppSharpService>();
var host = builder.Build();
var cppSharpService = host.Services.GetRequiredService<CppSharp.AOT.ICppSharpService>();

// 解析头文件
var typeInfos = await cppSharpService.ParseHeaderAsync("input.h");

// 遍历类型信息
foreach (var typeInfo in typeInfos)
{
    Console.WriteLine($"找到类型: {typeInfo.Name}");
    foreach (var member in typeInfo.Members)
    {
        Console.WriteLine($"  成员: {member.Name} ({member.Type})");
    }
}
```

### 3.2 生成C#代码

```csharp
// 生成C#代码
var builder = Host.CreateApplicationBuilder();
builder.Services.AddSingleton<CppSharp.AOT.ICppSharpService, CppSharp.AOT.CppSharpService>();
var host = builder.Build();
var cppSharpService = host.Services.GetRequiredService<CppSharp.AOT.ICppSharpService>();

// 解析头文件
var typeInfos = await cppSharpService.ParseHeaderAsync("input.h");

// 生成C#代码
var code = await cppSharpService.GenerateBindingCodeAsync(typeInfos, "MyApp.Generated");

// 输出代码到控制台
Console.WriteLine(code);

// 保存到文件
await File.WriteAllTextAsync("output.cs", code);
```

## 4. 高级使用示例

### 4.1 自定义服务实现

```csharp
// 自定义服务实现
public class CustomCppSharpService : CppSharp.AOT.CppSharpService
{
    public CustomCppSharpService(ILogger<CppSharpService> logger)
        : base(logger)
    {
    }

    // 重写解析逻辑
    public override async Task<List<CppTypeInfo>> ParseHeaderAsync(string headerPath)
    {
        // 自定义解析逻辑
        var typeInfos = await base.ParseHeaderAsync(headerPath);
        // 添加自定义类型处理
        return typeInfos;
    }

    // 重写生成逻辑
    public override async Task<string> GenerateBindingCodeAsync(List<CppTypeInfo> typeInfos, string namespaceName)
    {
        // 自定义生成逻辑
        var code = await base.GenerateBindingCodeAsync(typeInfos, namespaceName);
        // 添加自定义代码生成
        return code;
    }
}

// 使用自定义服务
var builder = Host.CreateApplicationBuilder();
builder.Services.AddSingleton<CppSharp.AOT.ICppSharpService, CustomCppSharpService>();
var host = builder.Build();
```

### 4.2 批量生成绑定

```csharp
// 批量生成绑定
var builder = Host.CreateApplicationBuilder();
builder.Services.AddSingleton<CppSharp.AOT.ICppSharpService, CppSharp.AOT.CppSharpService>();
var host = builder.Build();
var cppSharpService = host.Services.GetRequiredService<CppSharp.AOT.ICppSharpService>();

// 批量生成配置
var bindings = new List<(string Input, string Output, string Namespace)>
{
    ("input1.h", "output1.cs", "MyApp.Generated1"),
    ("input2.h", "output2.cs", "MyApp.Generated2"),
    ("input3.h", "output3.cs", "MyApp.Generated3")
};

// 批量生成
foreach (var (input, output, ns) in bindings)
{
    Console.WriteLine($"正在生成: {input} -> {output}");
    var result = await cppSharpService.GenerateBindingsAsync(input, output, new CppSharp.AOT.CppSharpOptions { Namespace = ns });
    Console.WriteLine($"结果: {(result ? "成功" : "失败")}");
}
```

### 4.3 使用CppSharpAotEngine

```csharp
// 使用CppSharpAotEngine
var builder = Host.CreateApplicationBuilder();
builder.Configuration.AddJsonFile("cppsharp_aot.setting.json");
builder.Services.Configure<CppSharp.AOT.CppSharpOptions>(builder.Configuration.GetSection("CppSharp"));
builder.Services.AddSingleton<CppSharp.AOT.ICppSharpService, CppSharp.AOT.CppSharpService>();
builder.Services.AddSingleton<CppSharp.AOT.CppSharpAotEngine>();

var host = builder.Build();
var engine = host.Services.GetRequiredService<CppSharp.AOT.CppSharpAotEngine>();
var options = host.Services.GetRequiredService<IOptions<CppSharp.AOT.CppSharpOptions>>().Value;

// 使用引擎生成绑定
var result = await engine.ExecuteGenerateBindingsAsync(
    "input.h", 
    "output.cs",
    options
);

Console.WriteLine($"生成结果: {(result ? "成功" : "失败")}");
```

## 5. 配置文件示例

### 5.1 基本配置

```json
{
  "CppSharp": {
    "InputPath": "",
    "OutputPath": "",
    "GenerateDebugInfo": false,
    "Namespace": "CppSharp.Generated",
    "GenerateAsyncWrappers": true,
    "GenerateEventWrappers": true
  }
}
```

### 5.2 高级配置

```json
{
  "CppSharp": {
    "InputPath": "headers",
    "OutputPath": "generated",
    "GenerateDebugInfo": true,
    "Namespace": "MyApp.Generated",
    "GenerateAsyncWrappers": true,
    "GenerateEventWrappers": true,
    "EnableParallelProcessing": true,
    "MaxRecursionDepth": 10,
    "EnableCaching": true,
    "MaxCacheSize": 1000
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "CppSharp.AOT": "Debug"
    }
  }
}
```

## 6. 真实应用场景示例

### 6.1 C++库封装

```csharp
// C++库封装示例
var builder = Host.CreateApplicationBuilder();
builder.Services.Configure<CppSharp.AOT.CppSharpOptions>(options =>
{
    options.Namespace = "MyLibrary.Cpp"; // 生成到指定命名空间
    options.GenerateAsyncWrappers = true; // 生成异步包装器
    options.GenerateDebugInfo = false; // 禁用调试信息（发布版本）
    options.GenerateEventWrappers = true; // 生成事件包装器
});
builder.Services.AddSingleton<CppSharp.AOT.ICppSharpService, CppSharp.AOT.CppSharpService>();

var host = builder.Build();
var cppSharpService = host.Services.GetRequiredService<CppSharp.AOT.ICppSharpService>();

// 遍历所有C++头文件
var headerFiles = Directory.GetFiles("libheaders", "*.h", SearchOption.AllDirectories);
foreach (var headerFile in headerFiles)
{
    // 生成对应的C#文件
    var outputFile = Path.Combine("src", "Generated", Path.ChangeExtension(Path.GetFileName(headerFile), ".cs"));
    Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
    
    Console.WriteLine($"正在生成: {headerFile} -> {outputFile}");
    var result = await cppSharpService.GenerateBindingsAsync(headerFile, outputFile);
    Console.WriteLine($"结果: {(result ? "成功" : "失败")}");
}
```

### 6.2 跨语言交互

```csharp
// 跨语言交互示例
var builder = Host.CreateApplicationBuilder();
builder.Services.AddSingleton<CppSharp.AOT.ICppSharpService, CppSharp.AOT.CppSharpService>();
var host = builder.Build();
var cppSharpService = host.Services.GetRequiredService<CppSharp.AOT.ICppSharpService>();

// 生成C#绑定
await cppSharpService.GenerateBindingsAsync(
    "native_lib.h", 
    "NativeLib.cs",
    new CppSharp.AOT.CppSharpOptions { Namespace = "NativeLib" }
);

// 使用生成的绑定（假设生成的代码已经编译）
// var nativeLib = new NativeLib.MyClass();
// var result = nativeLib.DoSomething(42, "test");
```

### 6.3 高性能计算

```csharp
// 高性能计算示例
var builder = Host.CreateApplicationBuilder();
builder.Services.AddSingleton<CppSharp.AOT.ICppSharpService, CppSharp.AOT.CppSharpService>();
var host = builder.Build();
var cppSharpService = host.Services.GetRequiredService<CppSharp.AOT.ICppSharpService>();

// 生成高性能计算库的C#绑定
await cppSharpService.GenerateBindingsAsync(
    "high_performance_lib.h", 
    "HighPerformanceLib.cs",
    new CppSharp.AOT.CppSharpOptions 
    { 
        Namespace = "HighPerformanceLib",
        GenerateAsyncWrappers = false, // 禁用异步包装器，提高性能
        GenerateDebugInfo = false // 禁用调试信息，减小体积
    }
);

// 使用生成的绑定进行高性能计算
// var result = HighPerformanceLib.Calculator.Compute(data, size);
```

## 7. 测试和验证示例

### 7.1 测试生成的代码

```csharp
// 测试生成的代码
var builder = Host.CreateApplicationBuilder();
builder.Services.AddSingleton<CppSharp.AOT.ICppSharpService, CppSharp.AOT.CppSharpService>();
var host = builder.Build();
var cppSharpService = host.Services.GetRequiredService<CppSharp.AOT.ICppSharpService>();

// 生成测试绑定
var result = await cppSharpService.GenerateBindingsAsync(
    "test.h", 
    "TestBindings.cs"
);

if (result)
{
    // 编译生成的代码
    var compiler = new CSharpCompiler();
    var compilationResult = await compiler.CompileAsync(
        "TestBindings.cs",
        references: new[] { "System.dll", "System.Core.dll" }
    );
    
    if (compilationResult.Success)
    {
        Console.WriteLine("生成的代码编译成功");
    }
    else
    {
        Console.WriteLine("生成的代码编译失败:");
        foreach (var error in compilationResult.Errors)
        {
            Console.WriteLine($"  {error.Line}: {error.Message}");
        }
    }
}
```

### 7.2 验证生成结果

```csharp
// 验证生成结果
var builder = Host.CreateApplicationBuilder();
builder.Services.AddSingleton<CppSharp.AOT.ICppSharpService, CppSharp.AOT.CppSharpService>();
var host = builder.Build();
var cppSharpService = host.Services.GetRequiredService<CppSharp.AOT.ICppSharpService>();

// 解析头文件
var typeInfos = await cppSharpService.ParseHeaderAsync("input.h");

// 验证解析结果
Console.WriteLine($"解析到 {typeInfos.Count} 个类型:");
foreach (var typeInfo in typeInfos)
{
    Console.WriteLine($"  - {typeInfo.Name} ({(typeInfo.IsClass ? "Class" : typeInfo.IsStruct ? "Struct" : typeInfo.IsEnum ? "Enum" : typeInfo.IsInterface ? "Interface" : "Unknown")})");
    Console.WriteLine($"    成员数: {typeInfo.Members.Count}");
    
    // 验证成员
    var methodCount = typeInfo.Members.Count(m => m.IsMethod);
    var propertyCount = typeInfo.Members.Count(m => m.IsProperty);
    var fieldCount = typeInfo.Members.Count(m => m.IsField);
    
    Console.WriteLine($"    方法: {methodCount}, 属性: {propertyCount}, 字段: {fieldCount}");
}

// 生成代码并验证
var code = await cppSharpService.GenerateBindingCodeAsync(typeInfos, "Test");
Console.WriteLine($"生成的代码行数: {code.Split('\n').Length}");

// 验证代码质量
var syntaxTree = CSharpSyntaxTree.ParseText(code);
var diagnostics = syntaxTree.GetDiagnostics();
if (diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
{
    Console.WriteLine("生成的代码存在语法错误:");
    foreach (var diagnostic in diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
    {
        Console.WriteLine($"  {diagnostic.Location.GetLineSpan().StartLinePosition.Line + 1}: {diagnostic.GetMessage()}");
    }
}
else
{
    Console.WriteLine("生成的代码语法正确");
}
```