# encrypt - 使用示例

## 快速开始

### 1. 基本加密解密示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Encrypt.AOT;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务容器
        var serviceProvider = BuildServiceProvider();
        var encryptService = serviceProvider.GetRequiredService<IEncryptService>();
        
        Console.WriteLine("加密解密基本示例");
        Console.WriteLine("=" * 50);
        
        // 加密数据
        Console.WriteLine("1. 加密数据 'Hello World'...");
        var encryptResult = await encryptService.EncryptAsync("Hello World");
        Console.WriteLine($"   加密成功，密文: {encryptResult.EncryptedData}");
        Console.WriteLine($"   密钥: {encryptResult.GeneratedKey}");
        Console.WriteLine($"   执行时间: {encryptResult.ExecutionTimeMs} ms");
        
        // 解密数据
        Console.WriteLine($"\n2. 解密数据...");
        var decryptResult = await encryptService.DecryptAsync(encryptResult.EncryptedData!, encryptResult.GeneratedKey!);
        Console.WriteLine($"   解密成功，明文: {decryptResult.DecryptedData}");
        Console.WriteLine($"   执行时间: {decryptResult.ExecutionTimeMs} ms");
        
        Console.WriteLine("\n加密解密示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        // 注册加密服务
        builder.AddEncryptAot();
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
using Encrypt.AOT;

public class Program
{
    public static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddEncryptAot();
        
        var serviceProvider = services.BuildServiceProvider();
        var engine = serviceProvider.GetRequiredService<EncryptAotEngine>();
        await engine.ExecuteCommandLineAsync(args);
    }
}
```

### 3. 密钥生成示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Encrypt.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("密钥生成示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var encryptService = serviceProvider.GetRequiredService<IEncryptService>();
        
        // 生成 AES-GCM 密钥
        Console.WriteLine("1. 生成 AES-GCM 256 位密钥...");
        var aesKeyResult = await encryptService.GenerateKeyAsync("AES-GCM", 256);
        Console.WriteLine($"   密钥生成成功，密钥: {aesKeyResult.GeneratedKey}");
        Console.WriteLine($"   执行时间: {aesKeyResult.ExecutionTimeMs} ms");
        
        // 生成 RSA 密钥
        Console.WriteLine("\n2. 生成 RSA 4096 位密钥...");
        var rsaKeyResult = await encryptService.GenerateKeyAsync("RSA", 4096);
        Console.WriteLine($"   RSA 密钥生成成功，密钥长度: {rsaKeyResult.GeneratedKey?.Length} 字符");
        Console.WriteLine($"   执行时间: {rsaKeyResult.ExecutionTimeMs} ms");
        
        Console.WriteLine("\n密钥生成示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddEncryptAot();
        return builder.BuildServiceProvider();
    }
}
```

### 4. 哈希计算与验证示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Encrypt.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("哈希计算与验证示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var encryptService = serviceProvider.GetRequiredService<IEncryptService>();
        
        // 生成哈希值
        Console.WriteLine("1. 计算 'Hello World' 的 SHA256 哈希值...");
        var hashResult = await encryptService.GenerateHashAsync("Hello World", "SHA256");
        Console.WriteLine($"   哈希值生成成功: {hashResult.GeneratedHash}");
        Console.WriteLine($"   执行时间: {hashResult.ExecutionTimeMs} ms");
        
        // 验证哈希值
        Console.WriteLine($"\n2. 验证哈希值...");
        var validateResult = await encryptService.ValidateHashAsync("Hello World", hashResult.GeneratedHash!, "SHA256");
        Console.WriteLine($"   哈希验证结果: {(validateResult.HashValid == true ? "有效" : "无效")}");
        Console.WriteLine($"   执行时间: {validateResult.ExecutionTimeMs} ms");
        
        // 验证错误哈希值
        Console.WriteLine($"\n3. 验证错误的哈希值...");
        var invalidValidateResult = await encryptService.ValidateHashAsync("Hello World", "invalid-hash", "SHA256");
        Console.WriteLine($"   哈希验证结果: {(invalidValidateResult.HashValid == true ? "有效" : "无效")}");
        Console.WriteLine($"   执行时间: {invalidValidateResult.ExecutionTimeMs} ms");
        
        Console.WriteLine("\n哈希计算与验证示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddEncryptAot();
        return builder.BuildServiceProvider();
    }
}
```

### 5. 命令行工具使用示例

```bash
# 显示帮助信息
dotnet run --project encrypt_aot.cs -- help

# 加密数据
dotnet run --project encrypt_aot.cs -- encrypt "Hello World"

# 解密数据 (使用上面命令生成的密文和密钥)
dotnet run --project encrypt_aot.cs -- decrypt <encryptedData> <key>

# 生成 AES-GCM 256 位密钥
dotnet run --project encrypt_aot.cs -- genkey AES-GCM 256

# 生成 RSA 4096 位密钥
dotnet run --project encrypt_aot.cs -- genkey RSA 4096

# 生成 SHA256 哈希值
dotnet run --project encrypt_aot.cs -- genhash "Hello World" SHA256

# 验证哈希值
dotnet run --project encrypt_aot.cs -- valhash "Hello World" <hash> SHA256

# 显示版本信息
dotnet run --project encrypt_aot.cs -- version
```

### 6. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Encrypt.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("加密高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置加密选项
        builder.Configure<EncryptOptions>(options => {
            options.DefaultAlgorithm = "AES-GCM";
            options.AesKeySize = 256;
            options.DefaultHashAlgorithm = "SHA256";
            options.EnableKeyCache = true;
            options.KeyCacheSize = 1000;
            options.EnableDetailedLogging = true;
        });
        
        // 注册加密服务
        builder.AddEncryptAot();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var encryptOptions = serviceProvider.GetRequiredService<IOptions<EncryptOptions>>().Value;
        Console.WriteLine($"配置信息:");
        Console.WriteLine($"   默认加密算法: {encryptOptions.DefaultAlgorithm}");
        Console.WriteLine($"   AES 密钥大小: {encryptOptions.AesKeySize} 位");
        Console.WriteLine($"   默认哈希算法: {encryptOptions.DefaultHashAlgorithm}");
        Console.WriteLine($"   启用密钥缓存: {encryptOptions.EnableKeyCache}");
        
        // 使用服务
        var encryptService = serviceProvider.GetRequiredService<IEncryptService>();
        var result = await encryptService.EncryptAsync("Hello World");
        Console.WriteLine($"\n使用配置的默认算法加密数据:");
        Console.WriteLine($"   密文: {result.EncryptedData}");
        Console.WriteLine($"   密钥: {result.GeneratedKey}");
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
using Encrypt.AOT;

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
        
        // 简单的自定义加密逻辑（实际项目中应使用更安全的算法）
        var encryptedData = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        var generatedKey = key ?? Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        
        return new EncryptCommandResult
        {
            Success = true,
            CommandType = EncryptCommandType.Encrypt,
            Results = new List<string> { "使用自定义算法加密成功", $"算法: {algorithm ?? _options.DefaultAlgorithm}" },
            EncryptedData = encryptedData,
            GeneratedKey = generatedKey
        };
    }
    
    public async Task<EncryptCommandResult> DecryptAsync(string encryptedData, string key, string? algorithm = null)
    {
        _logger.LogInformation("使用自定义算法解密数据");
        
        // 简单的自定义解密逻辑
        var decryptedData = Encoding.UTF8.GetString(Convert.FromBase64String(encryptedData));
        
        return new EncryptCommandResult
        {
            Success = true,
            CommandType = EncryptCommandType.Decrypt,
            Results = new List<string> { "使用自定义算法解密成功" },
            DecryptedData = decryptedData
        };
    }
    
    // 其他方法实现
    public async Task<EncryptCommandResult> ExecuteCommandAsync(EncryptCommandType commandType, Dictionary<string, string>? parameters = null)
    {
        // 命令执行逻辑
        return new EncryptCommandResult { Success = false, ErrorMessage = "未实现的命令" };
    }
    
    public async Task<EncryptCommandResult> GenerateKeyAsync(string? algorithm = null, int? keySize = null)
    {
        var key = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        return new EncryptCommandResult
        {
            Success = true,
            CommandType = EncryptCommandType.GenerateKey,
            GeneratedKey = key,
            Results = new List<string> { "自定义密钥生成成功" }
        };
    }
    
    public async Task<EncryptCommandResult> GenerateHashAsync(string data, string? algorithm = null)
    {
        var hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        return new EncryptCommandResult
        {
            Success = true,
            CommandType = EncryptCommandType.GenerateHash,
            GeneratedHash = hash,
            Results = new List<string> { "自定义哈希生成成功" }
        };
    }
    
    public async Task<EncryptCommandResult> ValidateHashAsync(string data, string hash, string? algorithm = null)
    {
        var generatedHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        bool isValid = generatedHash == hash;
        
        return new EncryptCommandResult
        {
            Success = true,
            CommandType = EncryptCommandType.ValidateHash,
            HashValid = isValid,
            Results = new List<string> { $"哈希验证结果: {(isValid ? "有效" : "无效")}" }
        };
    }
    
    public async Task<EncryptCommandResult> GetVersionInfoAsync()
    {
        return new EncryptCommandResult
        {
            Success = true,
            CommandType = EncryptCommandType.VersionInfo,
            Results = new List<string> { "自定义加密服务 v1.0.0" }
        };
    }
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

// 使用示例
public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("自定义加密服务示例");
        Console.WriteLine("=" * 50);
        
        var builder = new ServiceCollection();
        builder.AddCustomEncryptAot(); // 使用自定义扩展方法
        
        var serviceProvider = builder.BuildServiceProvider();
        var encryptService = serviceProvider.GetRequiredService<IEncryptService>();
        
        // 使用自定义服务加密数据
        var result = await encryptService.EncryptAsync("Hello World");
        Console.WriteLine($"自定义加密服务结果: {result.Success}");
        foreach (var res in result.Results)
        {
            Console.WriteLine($"   {res}");
        }
        Console.WriteLine($"   密文: {result.EncryptedData}");
        
        // 解密数据
        var decryptResult = await encryptService.DecryptAsync(result.EncryptedData!, result.GeneratedKey!);
        Console.WriteLine($"\n自定义解密服务结果: {decryptResult.Success}");
        Console.WriteLine($"   明文: {decryptResult.DecryptedData}");
    }
}
```

## 总结

以上示例展示了 encrypt 加密技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速入门基本的加密解密操作
2. 配置高级选项以优化性能
3. 生成和管理密钥
4. 计算和验证哈希值
5. 使用命令行工具进行加密操作
6. 扩展和自定义加密服务

该系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适合各种规模和复杂度的项目。

### AOT 编译优势

使用 AOT 编译的加密系统具有以下优势：

1. **快速启动**：AOT 编译的应用程序启动时间比 JIT 编译快数倍
2. **更小的内存占用**：优化的内存使用，减少运行时开销
3. **更高的性能**：本地代码执行，减少运行时编译成本
4. **更好的安全性**：减少攻击面，提高系统安全性
5. **部署简单**：无需依赖 .NET 运行时，单文件部署

通过这些示例，您可以快速掌握 encrypt AOT 加密系统的使用，并根据自己的需求进行扩展和定制。