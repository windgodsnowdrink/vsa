# etcd - 使用示例

## 快速开始

### 1. 基本键值操作示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Etcd.AOT;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务容器
        var serviceProvider = BuildServiceProvider();
        var etcdService = serviceProvider.GetRequiredService<IEtcdService>();
        
        Console.WriteLine("etcd 键值操作基本示例");
        Console.WriteLine("=" * 50);
        
        // 设置键值对
        Console.WriteLine("1. 设置键值对 'test-key' = 'test-value'...");
        var putResult = await etcdService.PutAsync("test-key", "test-value");
        Console.WriteLine($"   设置成功: {putResult.Success}");
        Console.WriteLine($"   执行时间: {putResult.ExecutionTimeMs} ms");
        
        // 获取键值对
        Console.WriteLine($"\n2. 获取键 'test-key'...");
        var getResult = await etcdService.GetAsync("test-key");
        Console.WriteLine($"   获取成功: {getResult.Success}");
        Console.WriteLine($"   执行时间: {getResult.ExecutionTimeMs} ms");
        if (getResult.KeyValues != null && getResult.KeyValues.Count > 0)
        {
            Console.WriteLine($"   值: {getResult.KeyValues[0].Value}");
            Console.WriteLine($"   版本: {getResult.KeyValues[0].Version}");
            Console.WriteLine($"   修改版本: {getResult.KeyValues[0].ModRevision}");
        }
        
        // 删除键值对
        Console.WriteLine($"\n3. 删除键 'test-key'...");
        var deleteResult = await etcdService.DeleteAsync("test-key");
        Console.WriteLine($"   删除成功: {deleteResult.Success}");
        Console.WriteLine($"   执行时间: {deleteResult.ExecutionTimeMs} ms");
        
        Console.WriteLine("\netcd 键值操作示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        // 注册 etcd 服务
        builder.AddEtcdAot();
        return builder.BuildServiceProvider();
    }
}
```

### 2. AOT 编译配置示例

```csharp
// 在项目文件中添加以下 AOT 编译配置
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true              // 启用 AOT 编译
#:property InvariantGlobalization=true  // 使用不变全球化模式
#:property EnableCompilationRelaxations=true // 启用编译优化
#:property PublishReadyToRun=true       // 启用 ReadyToRun 编译

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Etcd.AOT;

public class Program
{
    public static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddEtcdAot();
        
        var serviceProvider = services.BuildServiceProvider();
        var engine = serviceProvider.GetRequiredService<EtcdAotEngine>();
        await engine.ExecuteCommandLineAsync(args);
    }
}
```

### 3. 前缀匹配查询示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Etcd.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("etcd 前缀匹配查询示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var etcdService = serviceProvider.GetRequiredService<IEtcdService>();
        
        // 设置多个带有相同前缀的键值对
        Console.WriteLine("1. 设置多个带有 'user' 前缀的键值对...");
        await etcdService.PutAsync("user:1", "张三");
        await etcdService.PutAsync("user:2", "李四");
        await etcdService.PutAsync("user:3", "王五");
        Console.WriteLine("   设置完成");
        
        // 使用前缀匹配查询
        Console.WriteLine($"\n2. 使用前缀 'user' 查询所有用户...");
        var getResult = await etcdService.GetAsync("user", prefix: true);
        Console.WriteLine($"   查询成功: {getResult.Success}");
        Console.WriteLine($"   执行时间: {getResult.ExecutionTimeMs} ms");
        
        if (getResult.KeyValues != null && getResult.KeyValues.Count > 0)
        {
            Console.WriteLine($"   找到 {getResult.KeyValues.Count} 个用户:");
            foreach (var kv in getResult.KeyValues)
            {
                Console.WriteLine($"   - {kv.Key} = {kv.Value}");
            }
        }
        
        // 清理数据
        Console.WriteLine($"\n3. 清理数据...");
        await etcdService.DeleteAsync("user", prefix: true);
        Console.WriteLine("   清理完成");
        
        Console.WriteLine("\netcd 前缀匹配查询示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddEtcdAot();
        return builder.BuildServiceProvider();
    }
}
```

### 4. 事务处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Etcd.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("etcd 事务处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var etcdService = serviceProvider.GetRequiredService<IEtcdService>();
        
        // 初始化测试数据
        Console.WriteLine("1. 初始化测试数据...");
        await etcdService.PutAsync("transaction-key", "initial-value");
        Console.WriteLine("   初始化完成");
        
        // 执行事务
        Console.WriteLine($"\n2. 执行事务...");
        Console.WriteLine("   事务条件: 如果 'transaction-key' = 'initial-value'");
        Console.WriteLine("   成功操作: 设置 'transaction-key' = 'updated-value'");
        Console.WriteLine("   失败操作: 设置 'transaction-key' = 'failed-value'");
        
        var transactionResult = await etcdService.TransactionAsync(
            "transaction-key",           // 比较的键
            "initial-value",             // 比较的值
            "transaction-key",           // 成功时操作的键
            "updated-value",             // 成功时设置的值
            "transaction-key",           // 失败时操作的键
            "failed-value"               // 失败时设置的值
        );
        
        Console.WriteLine($"   事务执行成功: {transactionResult.Success}");
        Console.WriteLine($"   事务条件满足: {transactionResult.TransactionSuccess}");
        Console.WriteLine($"   执行时间: {transactionResult.ExecutionTimeMs} ms");
        
        // 验证结果
        Console.WriteLine($"\n3. 验证事务结果...");
        var getResult = await etcdService.GetAsync("transaction-key");
        if (getResult.KeyValues != null && getResult.KeyValues.Count > 0)
        {
            Console.WriteLine($"   当前值: {getResult.KeyValues[0].Value}");
        }
        
        // 清理数据
        Console.WriteLine($"\n4. 清理数据...");
        await etcdService.DeleteAsync("transaction-key");
        Console.WriteLine("   清理完成");
        
        Console.WriteLine("\netcd 事务处理示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddEtcdAot();
        return builder.BuildServiceProvider();
    }
}
```

### 5. 命令行工具使用示例

```bash
# 显示帮助信息
dotnet run --project etcd_aot.cs -- help

# 设置键值对
dotnet run --project etcd_aot.cs -- put test-key test-value

# 获取键值对
dotnet run --project etcd_aot.cs -- get test-key

# 获取前缀匹配的键值对
dotnet run --project etcd_aot.cs -- get test --prefix

# 删除键值对
dotnet run --project etcd_aot.cs -- delete test-key

# 执行事务
dotnet run --project etcd_aot.cs -- transaction compare-key compare-value success-key success-value fail-key fail-value

# 显示版本信息
dotnet run --project etcd_aot.cs -- version
```

### 6. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Etcd.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("etcd 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 etcd 选项
        builder.Configure<EtcdOptions>(options => {
            // 配置多个 etcd 服务器端点
            options.Endpoints = new List<string> { 
                "http://localhost:2379", 
                "http://localhost:2380",
                "http://localhost:2381"
            };
            
            // 配置超时和重试
            options.RequestTimeoutMs = 10000;
            options.ConnectTimeoutMs = 20000;
            options.MaxRetries = 5;
            
            // 启用详细日志
            options.EnableDetailedLogging = true;
            
            // 配置认证信息（如果需要）
            // options.Username = "username";
            // options.Password = "password";
            // options.EnableTls = true;
        });
        
        // 注册 etcd 服务
        builder.AddEtcdAot();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var etcdOptions = serviceProvider.GetRequiredService<IOptions<EtcdOptions>>().Value;
        Console.WriteLine($"配置信息:");
        Console.WriteLine($"   服务器端点: {string.Join(", ", etcdOptions.Endpoints)}");
        Console.WriteLine($"   请求超时: {etcdOptions.RequestTimeoutMs} ms");
        Console.WriteLine($"   连接超时: {etcdOptions.ConnectTimeoutMs} ms");
        Console.WriteLine($"   最大重试次数: {etcdOptions.MaxRetries}");
        Console.WriteLine($"   启用 TLS: {etcdOptions.EnableTls}");
        
        // 使用服务
        var etcdService = serviceProvider.GetRequiredService<IEtcdService>();
        var result = await etcdService.GetVersionInfoAsync();
        Console.WriteLine($"\n版本信息:");
        foreach (var res in result.Results)
        {
            Console.WriteLine($"   {res}");
        }
    }
}
```

### 7. 扩展开发示例

```csharp
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Etcd.AOT;

// 自定义 etcd 服务实现
public class CustomEtcdService : IEtcdService
{
    private readonly EtcdOptions _options;
    private readonly ILogger<CustomEtcdService> _logger;
    
    public CustomEtcdService(IOptions<EtcdOptions> options, ILogger<CustomEtcdService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }
    
    // 实现接口方法...
    public async Task<EtcdCommandResult> PutAsync(string key, string value, long leaseId = 0)
    {
        _logger.LogInformation("使用自定义实现设置键值对: {Key} = {Value}", key, value);
        
        // 简单的自定义实现（实际项目中应使用真实的 etcd 客户端）
        return new EtcdCommandResult
        {
            Success = true,
            CommandType = EtcdCommandType.Put,
            Key = key,
            Value = value,
            Results = new List<string> { 
                $"自定义实现成功设置键值对: {key} = {value}",
                $"服务器端点: {string.Join(", ", _options.Endpoints)}"
            }
        };
    }
    
    public async Task<EtcdCommandResult> GetAsync(string key, bool prefix = false)
    {
        _logger.LogInformation("使用自定义实现获取键: {Key}, 前缀匹配: {Prefix}", key, prefix);
        
        // 简单的自定义实现
        var keyValues = new List<EtcdKeyValue>();
        if (prefix)
        {
            // 模拟前缀匹配结果
            keyValues.Add(new EtcdKeyValue { Key = $"{key}:1", Value = "值1", Version = 1, ModRevision = 1 });
            keyValues.Add(new EtcdKeyValue { Key = $"{key}:2", Value = "值2", Version = 1, ModRevision = 2 });
        }
        else
        {
            // 模拟单个键值对结果
            keyValues.Add(new EtcdKeyValue { Key = key, Value = "自定义值", Version = 1, ModRevision = 1 });
        }
        
        return new EtcdCommandResult
        {
            Success = true,
            CommandType = EtcdCommandType.Get,
            Key = key,
            KeyValues = keyValues,
            Results = new List<string> { $"自定义实现成功获取键: {key}" }
        };
    }
    
    // 其他方法实现
    public async Task<EtcdCommandResult> DeleteAsync(string key, bool prefix = false)
    {
        _logger.LogInformation("使用自定义实现删除键: {Key}", key);
        return new EtcdCommandResult
        {
            Success = true,
            CommandType = EtcdCommandType.Delete,
            Key = key,
            Results = new List<string> { $"自定义实现成功删除键: {key}" }
        };
    }
    
    public async Task<EtcdCommandResult> ExecuteCommandAsync(EtcdCommandType commandType, Dictionary<string, string>? parameters = null)
    {
        return new EtcdCommandResult { Success = false, ErrorMessage = "未实现的命令" };
    }
    
    public async Task<EtcdCommandResult> TransactionAsync(string compareKey, string compareValue, string successKey, string successValue, string failKey, string failValue)
    {
        return new EtcdCommandResult
        {
            Success = true,
            CommandType = EtcdCommandType.Transaction,
            TransactionSuccess = true,
            Results = new List<string> { "自定义事务执行成功" }
        };
    }
    
    public async Task<EtcdCommandResult> GetVersionInfoAsync()
    {
        return new EtcdCommandResult
        {
            Success = true,
            CommandType = EtcdCommandType.VersionInfo,
            Results = new List<string> { "自定义 etcd 服务 v1.0.0" }
        };
    }
}

// 扩展方法
public static class CustomEtcdExtensions
{
    public static IServiceCollection AddCustomEtcdAot(this IServiceCollection services)
    {
        services.AddLogging();
        services.AddOptions<EtcdOptions>();
        // 注册自定义 etcd 服务
        services.AddSingleton<IEtcdService, CustomEtcdService>();
        services.AddSingleton<EtcdAotEngine>();
        return services;
    }
}

// 使用示例
public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("自定义 etcd 服务示例");
        Console.WriteLine("=" * 50);
        
        var builder = new ServiceCollection();
        builder.AddCustomEtcdAot(); // 使用自定义扩展方法
        
        var serviceProvider = builder.BuildServiceProvider();
        var etcdService = serviceProvider.GetRequiredService<IEtcdService>();
        
        // 使用自定义服务设置键值对
        var result = await etcdService.PutAsync("custom-key", "custom-value");
        Console.WriteLine($"自定义 etcd 服务结果: {result.Success}");
        foreach (var res in result.Results)
        {
            Console.WriteLine($"   {res}");
        }
        
        // 使用自定义服务获取键值对
        var getResult = await etcdService.GetAsync("custom-key");
        Console.WriteLine($"\n自定义 etcd 服务获取结果: {getResult.Success}");
        if (getResult.KeyValues != null && getResult.KeyValues.Count > 0)
        {
            Console.WriteLine($"   值: {getResult.KeyValues[0].Value}");
        }
    }
}
```

## 总结

以上示例展示了 etcd 加密技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速入门基本的键值操作
2. 配置高级选项以优化性能
3. 使用前缀匹配查询
4. 执行事务操作
5. 使用命令行工具进行 etcd 操作
6. 扩展和自定义 etcd 服务

该系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适合各种规模和复杂度的项目。

### AOT 编译优势

使用 AOT 编译的 etcd 系统具有以下优势：

1. **快速启动**：AOT 编译的应用程序启动时间比 JIT 编译快数倍
2. **更小的内存占用**：优化的内存使用，减少运行时开销
3. **更高的性能**：本地代码执行，减少运行时编译成本
4. **更好的安全性**：减少攻击面，提高系统安全性
5. **部署简单**：无需依赖 .NET 运行时，单文件部署

通过这些示例，您可以快速掌握 etcd AOT 系统的使用，并根据自己的需求进行扩展和定制。