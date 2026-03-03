# benchmark Agent Skill - benchmark 技能

## 技能概述

基于 .NET 10 的高性能基准测试技能，为 .NET 开发者提供强大的基准测试功能，包括性能测试、压力测试、负载测试、性能分析等核心功能，支持 AOT 编译优化，适用于各种性能敏感场景。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:package BenchmarkDotNet@0.14.0
#:package BenchmarkDotNet.Annotations@0.14.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package System.Memory@4.5.5
#:package System.Buffers@4.5.1
```

### 创建基准测试

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

public class Program
{
    public static void Main(string[] args)
    {
        // 运行基准测试
        var summary = BenchmarkRunner.Run<MyBenchmark>();
        Console.WriteLine(summary);
    }
}

[MemoryDiagnoser]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class MyBenchmark
{
    private readonly string _testString = "这是一个基准测试字符串，用于测试不同的字符串操作性能。";
    private readonly int _iterations = 1000;
    
    [Benchmark]
    public string StringConcat()
    {
        string result = "";
        for (int i = 0; i < _iterations; i++)
        {
            result += _testString;
        }
        return result;
    }
    
    [Benchmark]
    public string StringBuilder()
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < _iterations; i++)
        {
            sb.Append(_testString);
        }
        return sb.ToString();
    }
    
    [Benchmark]
    public string StringJoin()
    {
        var list = new List<string>();
        for (int i = 0; i < _iterations; i++)
        {
            list.Add(_testString);
        }
        return string.Join("", list);
    }
}
```

### 运行基准测试

```bash
# 直接运行基准测试
dotnet run --configuration Release

# 使用 AOT 编译运行基准测试
dotnet publish -c Release -r win-x64 --self-contained -p:PublishAot=true
dotnet bin/Release/net10.0/win-x64/publish/MyBenchmark.exe
```

## 导航地图

```
benchmark/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
├── scripts/                    # 脚本和工具
    ├── benchmark_demo.cs          # 基准测试演示
    ├── benchmark_demo.run.json    # 基准测试演示运行配置
    └── benchmark_demo.setting.json # 基准测试演示设置文件
```

## 主要功能

1. **性能基准测试**: 提供高性能的基准测试框架，支持多种基准测试场景
2. **内存分析**: 内置内存诊断和分析功能
3. **CPU 分析**: 提供 CPU 使用率和性能计数器分析
4. **并发测试**: 支持并发和并行基准测试
5. **AOT 编译支持**: 支持将基准测试应用编译为本机代码，提高测试准确性
6. **多框架支持**: 支持在多个 .NET 框架和运行时上运行基准测试
7. **详细报告生成**: 生成详细的基准测试报告，包括图表和统计数据
8. **自定义基准测试**: 支持自定义基准测试场景和配置
9. **性能比较**: 支持不同实现和算法的性能比较
10. **热路径分析**: 分析应用程序的热路径和性能瓶颈
11. **持续性能监控**: 支持持续集成环境中的性能监控
12. **与多种工具集成**: 支持与各种性能分析工具集成

## 扩展说明

此技能提供完整的基准测试解决方案，您可以根据需要进行扩展：

1. **自定义基准测试**: 编写自定义基准测试方法和类
2. **扩展报告生成**: 自定义基准测试报告格式和内容
3. **添加新的诊断器**: 开发新的性能诊断器
4. **集成新的工具**: 与新的性能分析工具集成
5. **支持新的平台**: 扩展到新的平台和架构
6. **添加新的测试场景**: 支持新的基准测试场景和模式

## 最佳实践

1. **使用 Release 配置**: 始终使用 Release 配置运行基准测试，确保优化生效
2. **控制变量**: 在比较不同实现时，确保只有一个变量变化
3. **足够的测试时长**: 确保基准测试运行足够长的时间，减少随机性影响
4. **使用内存诊断**: 启用内存诊断，了解内存使用情况
5. **使用 AOT 编译**: 对于 AOT 相关的基准测试，使用 AOT 编译提高测试准确性
6. **避免副作用**: 确保基准测试方法没有副作用，每次运行都产生相同的结果
7. **使用属性控制**: 使用 BenchmarkDotNet 属性控制测试行为
8. **分析热路径**: 分析基准测试结果，找出性能瓶颈
9. **定期运行测试**: 定期运行基准测试，监控性能变化
10. **与 CI/CD 集成**: 将基准测试集成到持续集成流程中

## AOT 编译支持

### AOT 编译配置

在项目文件中添加以下配置以支持 AOT 编译：

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
  <TrimMode>Full</TrimMode>
  <PublishReadyToRun>true</PublishReadyToRun>
  <PublishSingleFile>true</PublishSingleFile>
  <SelfContained>true</SelfContained>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
</PropertyGroup>
```

### AOT 编译命令

```bash
# 编译为 Windows x64 原生可执行文件
dotnet publish -c Release -r win-x64 --self-contained

# 编译为 Linux x64 原生可执行文件
dotnet publish -c Release -r linux-x64 --self-contained

# 编译为 macOS x64 原生可执行文件
dotnet publish -c Release -r osx-x64 --self-contained
```

### AOT 编译注意事项

1. **使用 AOT 兼容的库**: 确保使用的基准测试相关库支持 AOT 编译
2. **避免反射**: 避免在基准测试中使用反射，或使用 Source Generator 替代
3. **资源加载**: 确保所有资源在 AOT 编译时能被正确处理
4. **动态代码生成**: 避免使用动态代码生成技术
5. **测试验证**: 在 AOT 编译后进行充分测试
6. **基准测试准确性**: AOT 编译可以提高基准测试的准确性，减少 JIT 编译的影响
7. **比较 JIT 和 AOT 性能**: 可以比较 JIT 和 AOT 编译后的性能差异

## 与其他系统集成

### 与 ASP.NET Core 集成

```csharp
// ASP.NET Core 应用中集成基准测试
[ApiController]
[Route("[controller]")]
public class BenchmarkController : ControllerBase
{
    private readonly IBenchmarkService _benchmarkService;
    
    public BenchmarkController(IBenchmarkService benchmarkService)
    {
        _benchmarkService = benchmarkService;
    }
    
    [HttpPost("run")]
    public async Task<IActionResult> RunBenchmark([FromBody] BenchmarkRequest request)
    {
        // 运行基准测试
        var result = await _benchmarkService.RunAsync(request);
        return Ok(result);
    }
}

// 基准测试服务
public interface IBenchmarkService
{
    Task<BenchmarkResult> RunAsync(BenchmarkRequest request);
}

public class BenchmarkService : IBenchmarkService
{
    public async Task<BenchmarkResult> RunAsync(BenchmarkRequest request)
    {
        // 运行基准测试逻辑
        return await Task.Run(() => {
            var summary = BenchmarkRunner.Run<MyBenchmark>();
            return new BenchmarkResult {
                Summary = summary.ToString(),
                Results = ParseResults(summary)
            };
        });
    }
    
    // 其他方法实现...
}
```

### 与依赖注入集成

```csharp
// 与依赖注入集成
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // 注册基准测试服务
        services.AddSingleton<IBenchmarkService, BenchmarkService>();
        services.AddTransient<MyBenchmark>();
        
        // 注册其他服务
        services.AddHttpClient();
        services.AddLogging();
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // 配置应用程序
    }
}
```
