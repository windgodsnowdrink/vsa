# etcd - 参考文档

## 概述

etcd 是基于 .NET 10 的高性能 etcd 系统，专为 .NET 开发者设计，支持 AOT 编译以实现极致性能。

## 核心组件

### 1. IEtcdService（etcd 服务）
- **位置**: scripts/etcd_aot.cs
- **功能**: 核心 etcd 业务逻辑处理
- **特性**: 
  - 支持多种 etcd 操作
  - 高性能优化设计
  - 完善的错误处理
  - 详细的日志记录
  - AOT 编译支持

### 2. EtcdAotEngine（etcd AOT 引擎）
- **位置**: scripts/etcd_aot.cs
- **功能**: 命令行界面和引擎管理
- **特性**: 
  - 支持多种命令行操作
  - 高性能启动
  - 完善的参数处理

## 使用示例

### 基本用法

```csharp
// 获取 etcd 服务
var etcdService = serviceProvider.GetRequiredService<IEtcdService>();

// 设置键值对
var putResult = await etcdService.PutAsync("test-key", "test-value");
Console.WriteLine($"设置成功: {putResult.Success}");

// 获取键值对
var getResult = await etcdService.GetAsync("test-key");
Console.WriteLine($"获取成功: {getResult.Success}");
if (getResult.KeyValues != null && getResult.KeyValues.Count > 0)
{
    Console.WriteLine($"值: {getResult.KeyValues[0].Value}");
}
```

### 高级配置

```csharp
// 配置 etcd 选项
builder.Services.Configure<EtcdOptions>(options => {
    options.Endpoints = new List<string> { "http://localhost:2379", "http://localhost:2380" };
    options.RequestTimeoutMs = 10000;
    options.ConnectTimeoutMs = 20000;
    options.MaxRetries = 5;
    options.EnableDetailedLogging = true;
});
```

## 配置选项

### etcd 配置

```json
{
  "Etcd": {
    "Endpoints": ["http://localhost:2379"],          // etcd 服务器地址列表
    "Username": null,                               // 用户名
    "Password": null,                               // 密码
    "RequestTimeoutMs": 5000,                        // 请求超时时间（毫秒）
    "ConnectTimeoutMs": 10000,                      // 连接超时时间（毫秒）
    "MaxRetries": 3,                                 // 重试次数
    "EnableTls": false,                              // 是否启用 TLS
    "EnableDetailedLogging": false,                  // 是否启用详细日志
    "EnablePerformanceMonitoring": true              // 是否启用性能监控
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Etcd.AOT": "Information"
    }
  }
}
```

## 性能优化

1. **AOT 编译**: 使用 `PublishAot=true` 获得最佳性能
2. **连接池**: 优化的连接管理，减少连接创建开销
3. **异步编程**: 使用异步 API 避免阻塞
4. **内存优化**: 配置适当的超时和重试策略
5. **并发支持**: 优化的锁机制和并发处理

## 故障排除

### 常见问题

1. **连接失败**
   - 检查 etcd 服务器地址是否正确
   - 验证 etcd 服务是否正在运行
   - 查看日志以获取详细错误信息

2. **操作超时**
   - 增加请求超时时间
   - 检查网络连接
   - 优化 etcd 服务器性能

3. **事务失败**
   - 检查事务条件是否满足
   - 验证键值对是否存在
   - 查看日志以获取详细错误信息

4. **命令行参数错误**
   - 使用 `--help` 查看可用命令
   - 检查参数格式是否正确

## 扩展开发

### 添加自定义功能

```csharp
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
        
        // 自定义实现逻辑
        return new EtcdCommandResult
        {
            Success = true,
            CommandType = EtcdCommandType.Put,
            Key = key,
            Value = value,
            Results = new List<string> { $"自定义实现成功设置键值对: {key} = {value}" }
        };
    }
    
    // 实现其他接口方法...
    public async Task<EtcdCommandResult> GetAsync(string key, bool prefix = false)
    {
        _logger.LogInformation("使用自定义实现获取键: {Key}", key);
        
        // 自定义实现逻辑
        return new EtcdCommandResult
        {
            Success = true,
            CommandType = EtcdCommandType.Get,
            Key = key,
            KeyValues = new List<EtcdKeyValue>
            {
                new EtcdKeyValue
                {
                    Key = key,
                    Value = "自定义实现返回的值",
                    CreateRevision = 1,
                    ModRevision = 2,
                    Version = 2,
                    LeaseId = 0
                }
            },
            Results = new List<string> { $"自定义实现成功获取键: {key}" }
        };
    }
    
    // 其他方法实现...
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
```

## AOT 编译说明

### 编译选项

```csharp
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
```

### AOT 优势

1. **快速启动**: AOT 编译的应用程序启动时间比 JIT 编译快数倍
2. **更小的内存占用**: 优化的内存使用，减少运行时开销
3. **更高的性能**: 本地代码执行，减少运行时编译成本
4. **更好的安全性**: 减少攻击面，提高安全性
5. **部署简单**: 无需依赖 .NET 运行时，单文件部署

### AOT 注意事项

1. **反射使用**: 避免在运行时使用反射，或确保反射目标已在编译时已知
2. **动态代码生成**: 避免使用动态代码生成，如 `System.Reflection.Emit`
3. **序列化**: 确保所有需要序列化的类型都已正确配置
4. **配置文件**: 确保所有配置文件都能在 AOT 编译后正确加载

## 命令行参考

### 可用命令

| 命令       | 描述                 | 参数                          |
|------------|----------------------|-------------------------------|
| put        | 设置键值对           | <key> <value> [leaseId]        |
| get        | 获取键值对           | <key> [--prefix]              |
| delete     | 删除键值对           | <key> [--prefix]              |
| transaction| 执行事务             | <compareKey> <compareValue> <successKey> <successValue> <failKey> <failValue> |
| version    | 显示版本信息         | 无                            |
| help       | 显示帮助信息         | 无                            |

### 命令示例

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