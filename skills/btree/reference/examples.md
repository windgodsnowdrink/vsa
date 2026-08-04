# btree - 使用示例

## 快速入门

### 1. 基本使用示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using btree;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务容器
        var serviceProvider = BuildServiceProvider();
        var btreeFactory = serviceProvider.GetRequiredService<IBTreeFactory>();
        
        Console.WriteLine("B-Tree 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 创建 B-Tree 实例
        var btree = btreeFactory.Create<int, string>();
        
        // 插入数据
        await btree.InsertAsync(1, "苹果");
        await btree.InsertAsync(2, "香蕉");
        await btree.InsertAsync(3, "橙子");
        await btree.InsertAsync(4, "葡萄");
        await btree.InsertAsync(5, "西瓜");
        
        Console.WriteLine("数据插入完成");
        
        // 查询数据
        var result = await btree.SearchAsync(3);
        Console.WriteLine($"查询结果 (键 3): {result}");
        
        // 范围查询
        Console.WriteLine("\n范围查询结果 (1-5):");
        var rangeResult = await btree.RangeSearchAsync(1, 5);
        await foreach (var item in rangeResult)
        {
            Console.WriteLine($"  {item.Key}: {item.Value}");
        }
        
        // 删除数据
        await btree.DeleteAsync(4);
        Console.WriteLine("\n删除键 4 后的数据:");
        
        rangeResult = await btree.RangeSearchAsync(1, 5);
        await foreach (var item in rangeResult)
        {
            Console.WriteLine($"  {item.Key}: {item.Value}");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<IBTreeService, BTreeService>();
        builder.AddSingleton<IBTreeFactory, BTreeFactory>();
        builder.AddSingleton<IZoneTreeService, ZoneTreeService>();
        return builder.BuildServiceProvider();
    }
}
```

### 2. 高级配置示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using btree;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("B-Tree 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 B-Tree 设置
        builder.Configure<BTreeOptions>(options => {
            options.EnableAotOptimization = true; // 启用 AOT 优化
            options.Order = 10; // B-Tree 阶数
            options.EnableConcurrency = true; // 启用并发支持
            options.EnableTransaction = true; // 启用事务支持
            options.CacheSize = 2000; // 缓存大小
            options.EnableLogging = true; // 启用日志记录
        });
        
        // 注册服务
        builder.AddSingleton<IBTreeFactory, BTreeFactory>();
        builder.AddSingleton<IZoneTreeService, ZoneTreeService>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<BTreeOptions>>().Value;
        Console.WriteLine($"B-Tree 配置:");
        Console.WriteLine($"  启用 AOT 优化: {settings.EnableAotOptimization}");
        Console.WriteLine($"  B-Tree 阶数: {settings.Order}");
        Console.WriteLine($"  启用并发支持: {settings.EnableConcurrency}");
        Console.WriteLine($"  启用事务支持: {settings.EnableTransaction}");
        Console.WriteLine($"  缓存大小: {settings.CacheSize}");
        Console.WriteLine($"  启用日志记录: {settings.EnableLogging}");
        
        // 使用服务
        var btreeFactory = serviceProvider.GetRequiredService<IBTreeFactory>();
        var btree = btreeFactory.Create<string, int>(settings);
        
        // 插入数据
        await btree.InsertAsync("apple", 1);
        await btree.InsertAsync("banana", 2);
        await btree.InsertAsync("orange", 3);
        
        var value = await btree.SearchAsync("banana");
        Console.WriteLine($"\n查询结果 (键 'banana'): {value}");
    }
}
```

### 3. AOT 编译示例

```csharp
// 这是一个支持 AOT 编译的 B-Tree 示例
// 项目文件需要包含 AOT 配置

#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package ZoneTree@1.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property PublishAot=true
#:property TrimMode=Full
#:property PublishReadyToRun=true
#:property PublishSingleFile=true
#:property SelfContained=true

using System;
using Microsoft.Extensions.DependencyInjection;
using btree;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("B-Tree AOT 编译示例");
        Console.WriteLine("=" * 50);
        
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var btreeFactory = serviceProvider.GetRequiredService<IBTreeFactory>();
        
        // 创建 B-Tree 实例 (支持 AOT)
        var btree = btreeFactory.Create<int, string>();
        
        // 性能测试
        const int iterations = 10000;
        var stopwatch = Stopwatch.StartNew();
        
        Console.WriteLine($"\n执行 {iterations} 次插入操作...");
        
        // 批量插入数据
        for (int i = 0; i < iterations; i++)
        {
            await btree.InsertAsync(i, $"数据 {i}
