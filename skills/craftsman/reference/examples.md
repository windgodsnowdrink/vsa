# Craftsman AOT 使用示例

## 1. 基本使用示例

### 1.1 执行单个操作

```csharp
// 执行单个Craftsman操作
var builder = Host.CreateApplicationBuilder();
builder.Configuration.AddJsonFile("craftsman_aot.setting.json");
builder.Services.Configure<Craftsman.AOT.CraftsmanOptions>(builder.Configuration.GetSection("Craftsman"));
builder.Services.AddSingleton<Craftsman.AOT.ICraftsmanService, Craftsman.AOT.CraftsmanService>();
builder.Services.AddSingleton<Craftsman.AOT.CraftsmanAotEngine>();

var host = builder.Build();
var engine = host.Services.GetRequiredService<Craftsman.AOT.CraftsmanAotEngine>();

// 创建输入参数
var input = new Craftsman.AOT.CraftsmanInput
{
    OperationType = "generate",
    Parameters = new Dictionary<string, string>
    {
        { "type", "file" },
        { "name", "output.txt" },
        { "content", "Hello, Craftsman!" }
    }
};

// 执行操作
var result = await engine.ExecuteAsync(input);

// 处理结果
Console.WriteLine($"操作结果: {(result.Success ? "成功" : "失败")}");
if (result.Success && result.Data != null)
{
    Console.WriteLine($"结果数据: {Newtonsoft.Json.JsonConvert.SerializeObject(result.Data)}");
}
else if (!result.Success)
{
    Console.WriteLine($"错误信息: {result.ErrorMessage}");
}
Console.WriteLine($"执行时间: {result.ExecutionTimeMs}ms");
```

### 1.2 使用代码配置选项

```csharp
// 使用代码配置选项
var builder = Host.CreateApplicationBuilder();
builder.Services.Configure<Craftsman.AOT.CraftsmanOptions>(options =>
{
    options.EnableCache = true;
    options.CacheSize = 500;
    options.Timeout = TimeSpan.FromSeconds(15);
    options.EnableDetailedLogging = true;
});
builder.Services.AddSingleton<Craftsman.AOT.ICraftsmanService, Craftsman.AOT.CraftsmanService>();
builder.Services.AddSingleton<Craftsman.AOT.CraftsmanAotEngine>();

var host = builder.Build();
var engine = host.Services.GetRequiredService<Craftsman.AOT.CraftsmanAotEngine>();

// 执行操作
var result = await engine.ExecuteAsync(new Craftsman.AOT.CraftsmanInput
{
    OperationType = "build",
    Parameters = new Dictionary<string, string>
    {
        { "configuration", "release" },
        { "target", "net11.0" }
    }
});
```

## 2. 命令行使用示例

### 2.1 基本命令

```bash
# 执行生成操作
craftsman_aot.exe execute generate type=file name=output.txt content=Hello

# 执行构建操作
craftsman_aot.exe execute build configuration=release target=net11.0

# 执行部署操作
craftsman_aot.exe execute deploy environment=production

# 执行测试操作
craftsman_aot.exe execute test framework=xunit
```

### 2.2 获取状态

```bash
# 获取Craftsman状态
craftsman_aot.exe status

# 输出示例:
# Craftsman状态:
#   运行状态: 正常
#   已处理操作数: 100
#   成功操作数: 95
#   失败操作数: 5
#   缓存命中率: 85.00%
#   平均执行时间: 120.5ms
#   启动时间: 2026-01-03 14:30:00
```

### 2.3 重置状态

```bash
# 重置Craftsman状态
craftsman_aot.exe reset

# 输出示例:
# 重置状态: 成功
```

## 3. 批量执行示例

### 3.1 批量执行多个操作

```csharp
// 批量执行多个Craftsman操作
var builder = Host.CreateApplicationBuilder();
builder.Services.AddSingleton<Craftsman.AOT.ICraftsmanService, Craftsman.AOT.CraftsmanService>();
builder.Services.AddSingleton<Craftsman.AOT.CraftsmanAotEngine>();

var host = builder.Build();
var engine = host.Services.GetRequiredService<Craftsman.AOT.CraftsmanAotEngine>();

// 创建多个输入参数
var inputs = new List<Craftsman.AOT.CraftsmanInput>
{
    new Craftsman.AOT.CraftsmanInput
    {
        OperationType = "generate",
        Parameters = new Dictionary<string, string> { { "type", "file" }, { "name", "file1.txt" } }
    },
    new Craftsman.AOT.CraftsmanInput
    {
        OperationType = "generate",
        Parameters = new Dictionary<string, string> { { "type", "file" }, { "name", "file2.txt" } }
    },
    new Craftsman.AOT.CraftsmanInput
    {
        OperationType = "build",
        Parameters = new Dictionary<string, string> { { "configuration", "debug" } }
    },
    new Craftsman.AOT.CraftsmanInput
    {
        OperationType = "test",
        Parameters = new Dictionary<string, string> { { "framework", "nunit" } }
    }
};

// 批量执行
var results = await engine.ExecuteBatchAsync(inputs);

// 处理结果
int successCount = 0;
int failedCount = 0;

foreach (var result in results)
{
    if (result.Success)
    {
        successCount++;
    }
    else
    {
        failedCount++;
    }
}

Console.WriteLine($"批量执行完成: 成功 {successCount} 个, 失败 {failedCount} 个");
```

### 3.2 并行批量执行

```csharp
// 并行批量执行
var builder = Host.CreateApplicationBuilder();
builder.Services.Configure<Craftsman.AOT.CraftsmanOptions>(options =>
{
    options.WorkerCount = Environment.ProcessorCount; // 使用所有CPU核心
});
builder.Services.AddSingleton<Craftsman.AOT.ICraftsmanService, Craftsman.AOT.CraftsmanService>();
builder.Services.AddSingleton<Craftsman.AOT.CraftsmanAotEngine>();

var host = builder.Build();
var engine = host.Services.GetRequiredService<Craftsman.AOT.CraftsmanAotEngine>();

// 生成大量输入参数
var inputs = Enumerable.Range(1, 100).Select(i => new Craftsman.AOT.CraftsmanInput
{
    OperationType = "generate",
    Parameters = new Dictionary<string, string>
    {
        { "type", "file" },
        { "name", $"file{i}.txt