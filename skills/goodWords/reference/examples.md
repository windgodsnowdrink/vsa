# GoodWords - 使用示例

## 快速开始

### 1. 基本使用示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GoodWords.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("GoodWords 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 创建主机
        var builder = Host.CreateApplicationBuilder();
        
        // 配置服务
        builder.Services.Configure<GoodWordsOptions>(builder.Configuration.GetSection("GoodWords"));
        builder.Services.AddSingleton<IGoodWordsService, GoodWordsService>();
        builder.Services.AddSingleton<GoodWordsAotEngine>();
        
        // 构建主机
        using var host = builder.Build();
        
        // 获取 GoodWords 服务
        var goodWordsService = host.Services.GetRequiredService<IGoodWordsService>();
        
        // 生成好词好句
        Console.WriteLine("1. 生成好词好句...");
        var generateResult = await goodWordsService.GenerateAsync("love", 5, "zh");
        Console.WriteLine($"   生成结果: {(generateResult.Success ? "成功" : "失败")}");
        if (!generateResult.Success)
        {
            Console.WriteLine($"   错误信息: {generateResult.ErrorMessage}");
            return;
        }
        
        // 分类好词好句
        Console.WriteLine("2. 分类好词好句...");
        var classifyResult = await goodWordsService.ClassifyAsync("生活是美好的");
        Console.WriteLine($"   分类结果: {(classifyResult.Success ? "成功" : "失败")}");
        if (!classifyResult.Success)
        {
            Console.WriteLine($"   错误信息: {classifyResult.ErrorMessage}");
            return;
        }
        
        // 搜索好词好句
        Console.WriteLine("3. 搜索好词好句...");
        var searchResult = await goodWordsService.SearchAsync("爱情");
        Console.WriteLine($"   搜索结果: {(searchResult.Success ? "成功" : "失败")}");
        if (!searchResult.Success)
        {
            Console.WriteLine($"   错误信息: {searchResult.ErrorMessage}");
            return;
        }
        
        // 列出类别
        Console.WriteLine("4. 列出类别...");
        var listCategoriesResult = await goodWordsService.ListCategoriesAsync();
        Console.WriteLine($"   列出类别结果: {(listCategoriesResult.Success ? "成功" : "失败")}");
        if (!listCategoriesResult.Success)
        {
            Console.WriteLine($"   错误信息: {listCategoriesResult.ErrorMessage}");
            return;
        }
        
        // 获取版本信息
        Console.WriteLine("5. 获取版本信息...");
        var versionResult = await goodWordsService.GetVersionInfoAsync();
        Console.WriteLine($"   获取版本信息结果: {(versionResult.Success ? "成功" : "失败")}");
        if (!versionResult.Success)
        {
            Console.WriteLine($"   错误信息: {versionResult.ErrorMessage}");
            return;
        }
        
        Console.WriteLine("\n示例完成！");
    }
}
```

### 2. 命令行使用示例

GoodWords AOT 引擎支持通过命令行执行各种操作，以下是命令行使用示例：

#### 生成好词好句
```bash
# 使用命令行生成好词好句
goodWords_aot generate love 5 zh

# 输出示例
命令执行结果: 成功
执行时间: 550 ms
- 成功生成好词好句
- 类别: love
- 数量: 5
- 语言: zh
- 爱情是美好的礼物。
- 爱如阳光，温暖人心。
- 爱情是心灵最好的共鸣。
- 爱情是珍贵的财富。
- 爱如花朵，需要精心呵护。

好词好句详情:
  内容: 爱情是美好的礼物。
  类别: love
  标签: 爱情, 浪漫, 情感
  来源: AI 生成
  创建时间: 2024-01-19 12:00:00

  内容: 爱如阳光，温暖人心。
  类别: love
  标签: 爱情, 浪漫, 心灵
  来源: AI 生成
  创建时间: 2024-01-19 12:00:00
```

#### 分类好词好句
```bash
# 使用命令行分类好词好句
goodWords_aot classify 生活是美好的

# 输出示例
命令执行结果: 成功
执行时间: 320 ms
- 成功分类好词好句
- 内容: 生活是美好的
- 类别: general
- 标签: 生活, 感悟

好词好句详情:
  内容: 生活是美好的
  类别: general
  标签: 生活, 感悟
  来源: 用户输入
  创建时间: 2024-01-19 12:00:00
```

#### 搜索好词好句
```bash
# 使用命令行搜索好词好句
goodWords_aot search 爱情

# 输出示例
命令执行结果: 成功
执行时间: 220 ms
- 成功搜索好词好句
- 关键词: 爱情
- 类别: 全部
- 找到: 1 条结果
- 爱情是心灵的共鸣，是灵魂的契合。 (类别: love)

好词好句详情:
  内容: 爱情是心灵的共鸣，是灵魂的契合。
  类别: love
  标签: 爱情, 心灵, 共鸣
  来源: 未知
  创建时间: 2024-01-18 12:00:00
```

#### 列出类别
```bash
# 使用命令行列出类别
goodWords_aot list

# 输出示例
命令执行结果: 成功
执行时间: 120 ms
- 成功获取类别列表
- 类别数量: 10
- general
- love
- friendship
- family
- nature
- inspiration
- wisdom
- success
- life
- happiness

类别详情:
  - general
  - love
  - friendship
  - family
  - nature
  - inspiration
  - wisdom
  - success
  - life
  - happiness
```

#### 显示版本信息
```bash
# 使用命令行显示版本信息
goodWords_aot version

# 输出示例
命令执行结果: 成功
执行时间: 110 ms
- GoodWords AOT Engine
- 版本: 1.0.0
- .NET 版本: 10.0.0
- 操作系统: Microsoft Windows 10.0.19045
- 架构: X64
- AOT 编译: True
- 默认类别: general
- 默认语言: zh
- 工作目录: d:\Trae\vsa\skills\goodWords
```

### 3. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using GoodWords.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("GoodWords 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 创建配置
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "GoodWords:DefaultCategory", "general" },
                { "GoodWords:DefaultLanguage", "zh" },
                { "GoodWords:EnableCache", "true" },
                { "GoodWords:CacheSize", "1000" },
                { "GoodWords:RequestTimeoutMs", "30000" },
                { "GoodWords:EnableDetailedLogging", "true" },
                { "GoodWords:EnablePerformanceMonitoring", "true" }
            })
            .Build();
        
        // 创建主机
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddConfiguration(configuration);
        
        // 配置服务
        builder.Services.Configure<GoodWordsOptions>(builder.Configuration.GetSection("GoodWords"));
        builder.Services.AddSingleton<IGoodWordsService, GoodWordsService>();
        builder.Services.AddSingleton<GoodWordsAotEngine>();
        
        // 构建主机
        using var host = builder.Build();
        
        // 获取配置
        var options = host.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<GoodWordsOptions>>().Value;
        Console.WriteLine("配置信息:");
        Console.WriteLine($"- 默认类别: {options.DefaultCategory}");
        Console.WriteLine($"- 默认语言: {options.DefaultLanguage}");
        Console.WriteLine($"- 启用缓存: {options.EnableCache}");
        Console.WriteLine($"- 缓存大小: {options.CacheSize}");
        Console.WriteLine($"- 请求超时: {options.RequestTimeoutMs} ms");
        Console.WriteLine($"- 启用详细日志: {options.EnableDetailedLogging}");
        Console.WriteLine($"- 启用性能监控: {options.EnablePerformanceMonitoring}");
        
        // 获取 GoodWords 服务
        var goodWordsService = host.Services.GetRequiredService<IGoodWordsService>();
        
        // 使用服务
        Console.WriteLine("\n使用 GoodWords 服务...");
        var versionResult = await goodWordsService.GetVersionInfoAsync();
        if (versionResult.Success)
        {
            Console.WriteLine("版本信息获取成功:");
            foreach (var item in versionResult.Results)
            {
                Console.WriteLine($"- {item}");
            }
        }
    }
}
```

### 4. 性能优化示例

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GoodWords.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("GoodWords 性能优化示例");
        Console.WriteLine("=" * 50);
        
        // 创建主机
        var builder = Host.CreateApplicationBuilder();
        
        // 配置服务
        builder.Services.Configure<GoodWordsOptions>(options => {
            options.EnableCache = true;
            options.CacheSize = 1000;
            options.EnablePerformanceMonitoring = true;
        });
        builder.Services.AddSingleton<IGoodWordsService, GoodWordsService>();
        builder.Services.AddSingleton<GoodWordsAotEngine>();
        
        // 构建主机
        using var host = builder.Build();
        
        // 获取 GoodWords 服务
        var goodWordsService = host.Services.GetRequiredService<IGoodWordsService>();
        
        // 性能测试
        const int iterations = 100;
        var totalTime = 0L;
        
        Console.WriteLine($"执行 {iterations} 次生成操作测试...");
        
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            var result = await goodWordsService.GenerateAsync("general", 1, "zh");
            if (!result.Success)
            {
                Console.WriteLine($"测试失败: {result.ErrorMessage}");
                break;
            }
            totalTime += result.ExecutionTimeMs;
        }
        
        stopwatch.Stop();
        
        Console.WriteLine($"测试完成！");
        Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每次操作时间: {totalTime / (double)iterations:F3} ms");
        Console.WriteLine($"AOT 编译提升性能: 启动速度快，内存占用低");
    }
}
```

### 5. 错误处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GoodWords.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("GoodWords 错误处理示例");
        Console.WriteLine("=" * 50);
        
        // 创建主机
        var builder = Host.CreateApplicationBuilder();
        
        // 配置服务
        builder.Services.Configure<GoodWordsOptions>(builder.Configuration.GetSection("GoodWords"));
        builder.Services.AddSingleton<IGoodWordsService, GoodWordsService>();
        builder.Services.AddSingleton<GoodWordsAotEngine>();
        
        // 构建主机
        using var host = builder.Build();
        
        // 获取 GoodWords 服务
        var goodWordsService = host.Services.GetRequiredService<IGoodWordsService>();
        
        // 测试错误处理
        Console.WriteLine("1. 测试生成不存在的类别...");
        var generateResult = await goodWordsService.GenerateAsync("nonexistent", 5, "zh");
        Console.WriteLine($"   生成结果: {(generateResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"   错误信息: {generateResult.ErrorMessage}");
        
        Console.WriteLine("2. 测试空内容分类...");
        var classifyResult = await goodWordsService.ClassifyAsync("");
        Console.WriteLine($"   分类结果: {(classifyResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"   错误信息: {classifyResult.ErrorMessage}");
        
        Console.WriteLine("3. 测试空关键词搜索...");
        var searchResult = await goodWordsService.SearchAsync("");
        Console.WriteLine($"   搜索结果: {(searchResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"   错误信息: {searchResult.ErrorMessage}");
        
        Console.WriteLine("\n错误处理测试完成！");
    }
}
```

## 总结

以上示例展示了 GoodWords AOT 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本操作
2. 配置高级选项
3. 优化性能
4. 处理错误情况
5. 使用命令行工具执行操作

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。通过 AOT 编译，GoodWords 引擎获得了更快的启动速度和更低的内存占用，同时保持了完整的功能和良好的用户体验。
