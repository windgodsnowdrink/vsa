# encrypt - 参考文档

## 概述

encrypt 是基于 .NET 10 的高性能加密系统，专为 .NET 开发者设计，支持 AOT 编译以实现极致性能。

## 核心组件

### 1. IEncryptService（加密服务）
- **位置**: scripts/encrypt_aot.cs
- **功能**: 核心加密业务逻辑处理
- **特性**: 
  - 支持多种加密算法
  - 高性能优化设计
  - 完善的错误处理
  - 详细的日志记录
  - AOT 编译支持

### 2. EncryptAotEngine（加密 AOT 引擎）
- **位置**: scripts/encrypt_aot.cs
- **功能**: 命令行界面和引擎管理
- **特性**: 
  - 支持多种命令行操作
  - 高性能启动
  - 完善的参数处理

## 使用示例

### 基本用法

```csharp
// 获取加密服务
var encryptService = serviceProvider.GetRequiredService<IEncryptService>();

// 加密数据
var result = await encryptService.EncryptAsync("Hello World");
Console.WriteLine($"加密成功，密文: {result.EncryptedData}");
Console.WriteLine($"密钥: {result.GeneratedKey}");
```

### 高级配置

```csharp
// 配置加密选项
builder.Services.Configure<EncryptOptions>(options => {
    options.DefaultAlgorithm = "AES-GCM";
    options.AesKeySize = 256;
    options.DefaultHashAlgorithm = "SHA256";
    options.EnableKeyCache = true;
    options.MaxCacheSize = 1000;
    options.EnableDetailedLogging = true;
});
```

## 配置选项

### 加密配置

```json
{
  "Encrypt": {
    "DefaultAlgorithm": "AES-GCM",          // 默认加密算法
    "AesKeySize": 256,                       // AES 密钥大小（位）
    "RsaKeySize": 4096,                      // RSA 密钥大小（位）
    "DefaultHashAlgorithm": "SHA256",        // 默认哈希算法
    "EnableKeyCache": true,                  // 启用密钥缓存
    "KeyCacheSize": 1000,                    // 密钥缓存大小
    "EnableDetailedLogging": false,          // 启用详细日志
    "EnablePerformanceMonitoring": true      // 启用性能监控
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Encrypt.AOT": "Information"
    }
  }
}
```

## 性能优化

1. **AOT 编译**: 使用 `PublishAot=true` 获得最佳性能
2. **缓存使用**: 启用密钥缓存以提高性能
3. **异步编程**: 使用异步 API 避免阻塞
4. **内存优化**: 配置适当的缓存大小
5. **并发支持**: 优化的锁机制和并发处理

## 故障排除

### 常见问题

1. **加密失败**
   - 检查密钥是否正确
   - 验证算法是否支持
   - 查看日志以获取详细错误信息

2. **解密失败**
   - 检查密文是否完整
   - 验证密钥是否正确
   - 确保使用相同的算法

3. **性能问题**
   - 启用密钥缓存
   - 考虑使用 AOT 编译
   - 优化算法选择

4. **命令行参数错误**
   - 使用 `--help` 查看可用命令
   - 检查参数格式是否正确

## 扩展开发

### 添加自定义加密算法

```csharp
// 自定义加密服务实现
public class CustomEncryptService : IEncryptService
{
    private readonly EncryptOptions _options;
    private readonly ILogger<CustomEncryptService> _logger;
    
    public CustomEncryptService(IOptions<EncryptOptions> options, ILogger<CustomEncryptService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }
    
    // 实现接口方法...
    public async Task<EncryptCommandResult> EncryptAsync(string data, string? key = null, string? algorithm = null)
    {
        _logger.LogInformation("使用自定义算法加密数据");
        
        // 自定义加密逻辑
        return new EncryptCommandResult
        {
            Success = true,
            CommandType = EncryptCommandType.Encrypt,
            Results = new List<string> { "使用自定义算法加密成功" },
            EncryptedData = "custom-encrypted-data",
            GeneratedKey = "custom-key"
        };
    }
    
    // 实现其他接口方法...
    public async Task<EncryptCommandResult> DecryptAsync(string encryptedData, string key, string? algorithm = null)
    {
        // 自定义解密逻辑
        return new EncryptCommandResult
        {
            Success = true,
            CommandType = EncryptCommandType.Decrypt,
            Results = new List<string> { "使用自定义算法解密成功" },
            DecryptedData = "Hello World"
        };
    }
    
    // 其他方法实现...
}

// 扩展方法
public static class CustomEncryptExtensions
{
    public static IServiceCollection AddCustomEncryptAot(this IServiceCollection services)
    {
        services.AddLogging();
        services.AddOptions<EncryptOptions>();
        // 注册自定义加密服务
        services.AddSingleton<IEncryptService, CustomEncryptService>();
        services.AddSingleton<EncryptAotEngine>();
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
#:property TargetFramework=net10.0
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
| encrypt    | 加密数据           | <data> [key] [algorithm]      |
| decrypt    | 解密数据           | <encryptedData> <key> [algorithm] |
| generatekey|genkey | 生成密钥        | [algorithm] [keySize]         |
| generatehash|genhash | 生成哈希值    | <data> [algorithm]            |
| validatehash|valhash | 验证哈希值    | <data> <hash> [algorithm]     |
| version    | 显示版本信息         | 无                            |
| help       | 显示帮助信息         | 无                            |

### 命令示例

```bash
# 加密数据
dotnet run --project encrypt_aot.cs -- encrypt "Hello World"

# 解密数据
dotnet run --project encrypt_aot.cs -- decrypt <encryptedData> <key>

# 生成 AES-GCM 密钥
dotnet run --project encrypt_aot.cs -- genkey AES-GCM 256

# 生成 RSA 密钥
dotnet run --project encrypt_aot.cs -- genkey RSA 4096

# 生成 SHA256 哈希值
dotnet run --project encrypt_aot.cs -- genhash "Hello World" SHA256

# 验证哈希值
dotnet run --project encrypt_aot.cs -- valhash "Hello World" <hash> SHA256

# 显示版本信息
dotnet run --project encrypt_aot.cs -- version
```