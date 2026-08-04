# Util - 使用示例

## 快速开始

### 1. 基本使用示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Util.Services;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Util 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 初始化服务容器
        var serviceProvider = BuildServiceProvider();
        var utilService = serviceProvider.GetRequiredService<IUtilService>();
        
        // 使用文件工具
        Console.WriteLine("\n1. 文件工具示例:");
        var fileExists = await utilService.FileUtil.ExistsAsync("appsettings.json");
        Console.WriteLine($"文件是否存在: {fileExists}");
        
        // 使用加密工具
        Console.WriteLine("\n2. 加密工具示例:");
        var originalText = "Hello, Util!";
        var encrypted = await utilService.CryptoUtil.EncryptAsync(originalText);
        var decrypted = await utilService.CryptoUtil.DecryptAsync(encrypted);
        Console.WriteLine($"原始文本: {originalText}");
        Console.WriteLine($"加密结果: {encrypted}");
        Console.WriteLine($"解密结果: {decrypted}");
        
        // 使用序列化工具
        Console.WriteLine("\n3. 序列化工具示例:");
        var user = new User { Id = 1, Name = "张三", Email = "zhangsan@example.com" };
        var json = await utilService.SerializationUtil.SerializeToJsonAsync(user);
        var deserializedUser = await utilService.SerializationUtil.DeserializeFromJsonAsync<User>(json);
        Console.WriteLine($"序列化结果: {json}");
        Console.WriteLine($"反序列化结果: Id={deserializedUser.Id}, Name={deserializedUser.Name}, Email={deserializedUser.Email}");
        
        // 使用时间工具
        Console.WriteLine("\n4. 时间工具示例:");
        var now = DateTime.Now;
        var formattedDate = utilService.TimeUtil.FormatDateTime(now, "yyyy-MM-dd HH:mm:ss");
        var timestamp = utilService.TimeUtil.ToTimestamp(now);
        var dateFromTimestamp = utilService.TimeUtil.FromTimestamp(timestamp);
        Console.WriteLine($"当前时间: {formattedDate}");
        Console.WriteLine($"时间戳: {timestamp}");
        Console.WriteLine($"从时间戳转换: {utilService.TimeUtil.FormatDateTime(dateFromTimestamp, "yyyy-MM-dd HH:mm:ss")}");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册Util服务
        builder.AddUtilServices();
        builder.AddSingleton<IUtilService, DefaultUtilService>();
        
        return builder.BuildServiceProvider();
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
```

### 2. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Util.Services;
using Util.Settings;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Util 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置Util设置
        builder.Configure<UtilSettings>(options => {
            options.EnableCache = true;
            options.CacheSize = 2000;
            options.DefaultTimeout = 60;
            options.EnableDetailedLogging = true;
            options.MaxRetryAttempts = 3;
            options.EnableCompression = true;
        });
        
        // 配置文件工具设置
        builder.Configure<FileUtilSettings>(options => {
            options.BufferSize = 16384;
            options.MaxFileSize = 209715200; // 200MB
            options.EnableFileWatch = true;
            options.WatchInterval = 2000;
        });
        
        // 配置加密工具设置
        builder.Configure<CryptoUtilSettings>(options => {
            options.AesKeySize = 256;
            options.RsaKeySize = 2048;
            options.HashAlgorithm = "SHA256";
            options.KeyStoragePath = "keys/";
            options.EnableKeyRotation = false;
        });
        
        // 注册服务
        builder.AddUtilServices();
        builder.AddSingleton<IUtilService, DefaultUtilService>();
        builder.AddSingleton<IFileUtil, DefaultFileUtil>();
        builder.AddSingleton<ICryptoUtil, DefaultCryptoUtil>();
        builder.AddSingleton<ISerializationUtil, DefaultSerializationUtil>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var utilSettings = serviceProvider.GetRequiredService<IOptions<UtilSettings>>().Value;
        var fileSettings = serviceProvider.GetRequiredService<IOptions<FileUtilSettings>>().Value;
        var cryptoSettings = serviceProvider.GetRequiredService<IOptions<CryptoUtilSettings>>().Value;
        
        Console.WriteLine("\n配置信息:");
        Console.WriteLine($"Util设置: 缓存={utilSettings.EnableCache}, 缓存大小={utilSettings.CacheSize}");
        Console.WriteLine($"文件工具设置: 缓冲区大小={fileSettings.BufferSize}, 最大文件大小={fileSettings.MaxFileSize}");
        Console.WriteLine($"加密工具设置: AES密钥大小={cryptoSettings.AesKeySize}, RSA密钥大小={cryptoSettings.RsaKeySize}");
        
        // 使用服务
        var utilService = serviceProvider.GetRequiredService<IUtilService>();
        
        // 测试加密功能
        var testText = "测试高级配置加密功能";
        var encrypted = await utilService.CryptoUtil.EncryptAsync(testText);
        var decrypted = await utilService.CryptoUtil.DecryptAsync(encrypted);
        
        Console.WriteLine("\n加密测试结果:");
        Console.WriteLine($"原始文本: {testText}");
        Console.WriteLine($"解密结果: {decrypted}");
        Console.WriteLine($"加密成功: {testText == decrypted}");
    }
}
```

### 3. 性能优化示例

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Util.Services;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Util 性能优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var utilService = serviceProvider.GetRequiredService<IUtilService>();
        
        // 性能测试 - 文件操作
        Console.WriteLine("\n1. 文件操作性能测试:");
        await TestFileOperationPerformance(utilService);
        
        // 性能测试 - 加密操作
        Console.WriteLine("\n2. 加密操作性能测试:");
        await TestCryptoOperationPerformance(utilService);
        
        // 性能测试 - 序列化操作
        Console.WriteLine("\n3. 序列化操作性能测试:");
        await TestSerializationPerformance(utilService);
    }
    
    private static async Task TestFileOperationPerformance(IUtilService utilService)
    {
        const int iterations = 100;
        var testContent = "这是一个测试文件内容，用于性能测试。";
        var testFilePath = "test_performance.txt";
        
        // 写入测试
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            await utilService.FileUtil.WriteAllTextAsync(testFilePath, testContent + i);
        }
        stopwatch.Stop();
        Console.WriteLine($"写入 {iterations} 次: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        
        // 读取测试
        stopwatch.Restart();
        for (int i = 0; i < iterations; i++)
        {
            await utilService.FileUtil.ReadAllTextAsync(testFilePath);
        }
        stopwatch.Stop();
        Console.WriteLine($"读取 {iterations} 次: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        
        // 清理
        if (await utilService.FileUtil.ExistsAsync(testFilePath))
        {
            await utilService.FileUtil.DeleteAsync(testFilePath);
        }
    }
    
    private static async Task TestCryptoOperationPerformance(IUtilService utilService)
    {
        const int iterations = 1000;
        var testText = "这是一个用于加密性能测试的文本。";
        
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            var encrypted = await utilService.CryptoUtil.EncryptAsync(testText);
            await utilService.CryptoUtil.DecryptAsync(encrypted);
        }
        stopwatch.Stop();
        Console.WriteLine($"加密解密 {iterations} 次: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
    }
    
    private static async Task TestSerializationPerformance(IUtilService utilService)
    {
        const int iterations = 1000;
        var testObject = new User { Id = 1, Name = "测试用户", Email = "test@example.com", Age = 30 };
        
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            var json = await utilService.SerializationUtil.SerializeToJsonAsync(testObject);
            await utilService.SerializationUtil.DeserializeFromJsonAsync<User>(json);
        }
        stopwatch.Stop();
        Console.WriteLine($"序列化反序列化 {iterations} 次: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddUtilServices();
        builder.AddSingleton<IUtilService, DefaultUtilService>();
        builder.AddSingleton<IFileUtil, DefaultFileUtil>();
        builder.AddSingleton<ICryptoUtil, DefaultCryptoUtil>();
        builder.AddSingleton<ISerializationUtil, DefaultSerializationUtil>();
        
        // 配置性能优化设置
        builder.Configure<UtilSettings>(options => {
            options.EnableCache = true;
            options.CacheSize = 1000;
            options.EnableCompression = true;
        });
        
        return builder.BuildServiceProvider();
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}
```

### 4. 错误处理示例

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Util.Services;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Util 错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var utilService = serviceProvider.GetRequiredService<IUtilService>();
        
        // 文件操作错误处理
        Console.WriteLine("\n1. 文件操作错误处理:");
        await HandleFileOperationErrors(utilService);
        
        // 加密操作错误处理
        Console.WriteLine("\n2. 加密操作错误处理:");
        await HandleCryptoOperationErrors(utilService);
        
        // 序列化操作错误处理
        Console.WriteLine("\n3. 序列化操作错误处理:");
        await HandleSerializationErrors(utilService);
    }
    
    private static async Task HandleFileOperationErrors(IUtilService utilService)
    {
        try
        {
            // 尝试读取不存在的文件
            await utilService.FileUtil.ReadAllTextAsync("non_existent_file.txt");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"文件未找到: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"IO错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用错误: {ex.Message}");
        }
        
        try
        {
            // 尝试写入只读文件
            // 注意: 实际环境中需要创建一个只读文件进行测试
            // await utilService.FileUtil.WriteAllTextAsync("read_only_file.txt", "测试内容");
            Console.WriteLine("写入操作测试: 跳过(需要实际只读文件)");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"访问权限错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"写入错误: {ex.Message}");
        }
    }
    
    private static async Task HandleCryptoOperationErrors(IUtilService utilService)
    {
        try
        {
            // 尝试解密无效的加密文本
            await utilService.CryptoUtil.DecryptAsync("invalid_encrypted_text");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"参数错误: {ex.Message}");
        }
        catch (CryptographicException ex)
        {
            Console.WriteLine($"加密错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用加密错误: {ex.Message}");
        }
    }
    
    private static async Task HandleSerializationErrors(IUtilService utilService)
    {
        try
        {
            // 尝试反序列化无效的JSON
            await utilService.SerializationUtil.DeserializeFromJsonAsync<User>("invalid json");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON错误: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"参数错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用序列化错误: {ex.Message}");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddUtilServices();
        builder.AddSingleton<IUtilService, DefaultUtilService>();
        builder.AddSingleton<IFileUtil, DefaultFileUtil>();
        builder.AddSingleton<ICryptoUtil, DefaultCryptoUtil>();
        builder.AddSingleton<ISerializationUtil, DefaultSerializationUtil>();
        return builder.BuildServiceProvider();
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}

// 用于JSON异常的模拟类
public class JsonException : Exception
{
    public JsonException(string message) : base(message) { }
}

public class CryptographicException : Exception
{
    public CryptographicException(string message) : base(message) { }
}
```

### 5. Scrutor 使用示例

```csharp
using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Util.Services;
using Util.Decorators;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Util Scrutor 使用示例");
        Console.WriteLine("=" * 50);
        
        // 初始化服务容器
        var serviceProvider = BuildServiceProviderWithScrutor();
        
        // 获取服务
        var utilService = serviceProvider.GetRequiredService<IUtilService>();
        var fileUtil = serviceProvider.GetRequiredService<IFileUtil>();
        var cryptoUtil = serviceProvider.GetRequiredService<ICryptoUtil>();
        
        Console.WriteLine("\n1. Scrutor自动注册测试:");
        Console.WriteLine($"Util服务注册成功: {utilService != null}");
        Console.WriteLine($"文件工具注册成功: {fileUtil != null}");
        Console.WriteLine($"加密工具注册成功: {cryptoUtil != null}");
        
        // 测试装饰器模式
        Console.WriteLine("\n2. 装饰器模式测试:");
        
        // 使用文件工具（应该会触发缓存装饰器）
        var testFilePath = "scrutor_test.txt";
        await utilService.FileUtil.WriteAllTextAsync(testFilePath, "测试文件内容");
        
        // 第一次读取（应该从文件读取）
        var content1 = await utilService.FileUtil.ReadAllTextAsync(testFilePath);
        Console.WriteLine($"第一次读取内容: {content1}");
        
        // 第二次读取（应该从缓存读取）
        var content2 = await utilService.FileUtil.ReadAllTextAsync(testFilePath);
        Console.WriteLine($"第二次读取内容: {content2}");
        
        // 使用加密工具（应该会触发日志装饰器）
        var testText = "测试装饰器加密";
        var encrypted = await utilService.CryptoUtil.EncryptAsync(testText);
        var decrypted = await utilService.CryptoUtil.DecryptAsync(encrypted);
        Console.WriteLine($"加密测试结果: {testText == decrypted}");
        
        // 清理
        if (await utilService.FileUtil.ExistsAsync(testFilePath))
        {
            await utilService.FileUtil.DeleteAsync(testFilePath);
        }
        
        Console.WriteLine("\nScrutor使用示例完成！");
    }
    
    private static ServiceProvider BuildServiceProviderWithScrutor()
    {
        var builder = new ServiceCollection();
        
        // 配置设置
        builder.Configure<UtilSettings>(options => {
            options.EnableCache = true;
            options.CacheSize = 1000;
        });
        
        // 使用Scrutor自动注册服务
        builder.Scan(scan => scan
            // 从当前程序集和Util服务程序集扫描
            .FromAssemblies(Assembly.GetExecutingAssembly(), typeof(IUtilService).Assembly)
            
            // 注册Util服务
            .AddClasses(classes => classes.AssignableTo<IUtilService>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
            
            // 注册文件工具
            .AddClasses(classes => classes.AssignableTo<IFileUtil>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
            
            // 注册加密工具
            .AddClasses(classes => classes.AssignableTo<ICryptoUtil>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
            
            // 注册序列化工具
            .AddClasses(classes => classes.AssignableTo<ISerializationUtil>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
            
            // 注册配置工具
            .AddClasses(classes => classes.AssignableTo<IConfigUtil>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
            
            // 注册反射工具
            .AddClasses(classes => classes.AssignableTo<IReflectionUtil>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
            
            // 注册时间工具
            .AddClasses(classes => classes.AssignableTo<ITimeUtil>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
            
            // 注册网络工具
            .AddClasses(classes => classes.AssignableTo<INetworkUtil>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
        );
        
        // 使用Scrutor的装饰器模式
        builder.Decorate<IFileUtil, CachedFileUtil>();
        builder.Decorate<ICryptoUtil, LoggingCryptoUtil>();
        builder.Decorate<ISerializationUtil, CompressedSerializationUtil>();
        
        return builder.BuildServiceProvider();
    }
}

// 装饰器实现示例
public class CachedFileUtil : IFileUtil
{
    private readonly IFileUtil _inner;
    private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();
    
    public CachedFileUtil(IFileUtil inner)
    {
        _inner = inner;
    }
    
    // 实现IFileUtil接口，添加缓存逻辑
    public async Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(path, out var cachedContent))
        {
            Console.WriteLine($"[缓存命中] 读取文件: {path}");
            return cachedContent;
        }
        
        Console.WriteLine($"[从文件读取] 读取文件: {path}");
        var content = await _inner.ReadAllTextAsync(path, cancellationToken);
        _cache[path] = content;
        return content;
    }
    
    // 其他方法实现...
    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        return _inner.ExistsAsync(path, cancellationToken);
    }
    
    public Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default)
    {
        // 写入时更新缓存
        _cache[path] = content;
        return _inner.WriteAllTextAsync(path, content, cancellationToken);
    }
    
    public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        // 删除时移除缓存
        _cache.Remove(path);
        return _inner.DeleteAsync(path, cancellationToken);
    }
    
    // 其他方法实现...
    public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
    {
        return _inner.ReadAllBytesAsync(path, cancellationToken);
    }
    
    public Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
    {
        return _inner.WriteAllBytesAsync(path, bytes, cancellationToken);
    }
    
    public Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default)
    {
        return _inner.OpenReadAsync(path, cancellationToken);
    }
    
    public Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default)
    {
        return _inner.OpenWriteAsync(path, cancellationToken);
    }
}

public class LoggingCryptoUtil : ICryptoUtil
{
    private readonly ICryptoUtil _inner;
    
    public LoggingCryptoUtil(ICryptoUtil inner)
    {
        _inner = inner;
    }
    
    public async Task<string> EncryptAsync(string plainText, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[加密操作] 加密文本长度: {plainText.Length}");
        var result = await _inner.EncryptAsync(plainText, cancellationToken);
        Console.WriteLine($"[加密操作] 加密结果长度: {result.Length}");
        return result;
    }
    
    public async Task<string> DecryptAsync(string encryptedText, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[解密操作] 解密文本长度: {encryptedText.Length}");
        var result = await _inner.DecryptAsync(encryptedText, cancellationToken);
        Console.WriteLine($"[解密操作] 解密结果长度: {result.Length}");
        return result;
    }
    
    // 其他方法实现...
    public Task<string> ComputeHashAsync(string input, CancellationToken cancellationToken = default)
    {
        return _inner.ComputeHashAsync(input, cancellationToken);
    }
    
    public Task<bool> VerifyHashAsync(string input, string hash, CancellationToken cancellationToken = default)
    {
        return _inner.VerifyHashAsync(input, hash, cancellationToken);
    }
}

public class CompressedSerializationUtil : ISerializationUtil
{
    private readonly ISerializationUtil _inner;
    
    public CompressedSerializationUtil(ISerializationUtil inner)
    {
        _inner = inner;
    }
    
    // 实现ISerializationUtil接口，添加压缩逻辑
    public Task<string> SerializeToJsonAsync(object value, CancellationToken cancellationToken = default)
    {
        return _inner.SerializeToJsonAsync(value, cancellationToken);
    }
    
    public Task<T> DeserializeFromJsonAsync<T>(string json, CancellationToken cancellationToken = default)
    {
        return _inner.DeserializeFromJsonAsync<T>(json, cancellationToken);
    }
    
    // 其他方法实现...
    public Task<string> SerializeToXmlAsync(object value, CancellationToken cancellationToken = default)
    {
        return _inner.SerializeToXmlAsync(value, cancellationToken);
    }
    
    public Task<T> DeserializeFromXmlAsync<T>(string xml, CancellationToken cancellationToken = default)
    {
        return _inner.DeserializeFromXmlAsync<T>(xml, cancellationToken);
    }
}

// 简化的接口定义
public interface IFileUtil
{
    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);
    Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default);
    Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default);
    Task DeleteAsync(string path, CancellationToken cancellationToken = default);
    Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);
    Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default);
    Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default);
    Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default);
}

public interface ICryptoUtil
{
    Task<string> EncryptAsync(string plainText, CancellationToken cancellationToken = default);
    Task<string> DecryptAsync(string encryptedText, CancellationToken cancellationToken = default);
    Task<string> ComputeHashAsync(string input, CancellationToken cancellationToken = default);
    Task<bool> VerifyHashAsync(string input, string hash, CancellationToken cancellationToken = default);
}

public interface ISerializationUtil
{
    Task<string> SerializeToJsonAsync(object value, CancellationToken cancellationToken = default);
    Task<T> DeserializeFromJsonAsync<T>(string json, CancellationToken cancellationToken = default);
    Task<string> SerializeToXmlAsync(object value, CancellationToken cancellationToken = default);
    Task<T> DeserializeFromXmlAsync<T>(string xml, CancellationToken cancellationToken = default);
}

public interface IConfigUtil { }
public interface IReflectionUtil { }
public interface ITimeUtil { }
public interface INetworkUtil { }

public class DefaultFileUtil : IFileUtil { /* 实现 */ }
public class DefaultCryptoUtil : ICryptoUtil { /* 实现 */ }
public class DefaultSerializationUtil : ISerializationUtil { /* 实现 */ }
public class DefaultConfigUtil : IConfigUtil { }
public class DefaultReflectionUtil : IReflectionUtil { }
public class DefaultTimeUtil : ITimeUtil { }
public class DefaultNetworkUtil : INetworkUtil { }
public class DefaultUtilService : IUtilService 
{
    public IFileUtil FileUtil { get; }
    public ICryptoUtil CryptoUtil { get; }
    public ISerializationUtil SerializationUtil { get; }
    public IConfigUtil ConfigUtil { get; }
    public IReflectionUtil ReflectionUtil { get; }
    public ITimeUtil TimeUtil { get; }
    public INetworkUtil NetworkUtil { get; }
    
    public DefaultUtilService(IFileUtil fileUtil, ICryptoUtil cryptoUtil, ISerializationUtil serializationUtil, 
                             IConfigUtil configUtil, IReflectionUtil reflectionUtil, 
                             ITimeUtil timeUtil, INetworkUtil networkUtil)
    {
        FileUtil = fileUtil;
        CryptoUtil = cryptoUtil;
        SerializationUtil = serializationUtil;
        ConfigUtil = configUtil;
        ReflectionUtil = reflectionUtil;
        TimeUtil = timeUtil;
        NetworkUtil = networkUtil;
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUtilServices(this IServiceCollection services)
    {
        // 注册默认实现
        services.AddSingleton<IFileUtil, DefaultFileUtil>();
        services.AddSingleton<ICryptoUtil, DefaultCryptoUtil>();
        services.AddSingleton<ISerializationUtil, DefaultSerializationUtil>();
        services.AddSingleton<IConfigUtil, DefaultConfigUtil>();
        services.AddSingleton<IReflectionUtil, DefaultReflectionUtil>();
        services.AddSingleton<ITimeUtil, DefaultTimeUtil>();
        services.AddSingleton<INetworkUtil, DefaultNetworkUtil>();
        services.AddSingleton<IUtilService, DefaultUtilService>();
        return services;
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}
```

### 6. 各种工具具体使用示例

#### 6.1 文件工具详细示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Util.Services;

public class FileUtilExample
{
    public static async Task Run()
    {
        Console.WriteLine("文件工具详细示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var fileUtil = serviceProvider.GetRequiredService<IFileUtil>();
        
        var testDir = "test_files";
        var testFile = Path.Combine(testDir, "example.txt");
        
        try
        {
            // 创建目录
            if (!await fileUtil.DirectoryExistsAsync(testDir))
            {
                await fileUtil.CreateDirectoryAsync(testDir);
                Console.WriteLine($"创建目录: {testDir}");
            }
            
            // 写入文件
            var content = "Hello, FileUtil!\n这是一个测试文件。\n包含多行内容。";
            await fileUtil.WriteAllTextAsync(testFile, content);
            Console.WriteLine($"写入文件: {testFile}");
            
            // 读取文件
            var readContent = await fileUtil.ReadAllTextAsync(testFile);
            Console.WriteLine("\n读取文件内容:");
            Console.WriteLine(readContent);
            
            // 检查文件存在
            var exists = await fileUtil.ExistsAsync(testFile);
            Console.WriteLine($"\n文件是否存在: {exists}");
            
            // 获取文件信息
            var fileInfo = await fileUtil.GetFileInfoAsync(testFile);
            Console.WriteLine($"文件大小: {fileInfo.Length} 字节");
            Console.WriteLine($"创建时间: {fileInfo.CreationTime}");
            Console.WriteLine($"最后修改时间: {fileInfo.LastWriteTime}");
            
            // 复制文件
            var copyFile = Path.Combine(testDir, "example_copy.txt");
            await fileUtil.CopyAsync(testFile, copyFile);
            Console.WriteLine($"\n复制文件到: {copyFile}");
            
            // 移动文件
            var moveFile = Path.Combine(testDir, "example_moved.txt");
            await fileUtil.MoveAsync(copyFile, moveFile);
            Console.WriteLine($"移动文件到: {moveFile}");
            
            // 枚举目录中的文件
            var files = await fileUtil.EnumerateFilesAsync(testDir);
            Console.WriteLine("\n目录中的文件:");
            foreach (var file in files)
            {
                Console.WriteLine($"- {file}");
            }
            
            // 删除文件
            await fileUtil.DeleteAsync(testFile);
            await fileUtil.DeleteAsync(moveFile);
            Console.WriteLine("\n删除测试文件");
            
            // 删除目录
            if (await fileUtil.DirectoryExistsAsync(testDir))
            {
                await fileUtil.DeleteDirectoryAsync(testDir);
                Console.WriteLine($"删除目录: {testDir}");
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        
        Console.WriteLine("\n文件工具示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddUtilServices();
        return builder.BuildServiceProvider();
    }
}

// 扩展IFileUtil接口
public interface IFileUtil
{
    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);
    Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default);
    Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default);
    Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);
    Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default);
    Task DeleteAsync(string path, CancellationToken cancellationToken = default);
    Task CopyAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default);
    Task MoveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default);
    Task<bool> DirectoryExistsAsync(string path, CancellationToken cancellationToken = default);
    Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default);
    Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> EnumerateFilesAsync(string path, CancellationToken cancellationToken = default);
    Task<FileInfo> GetFileInfoAsync(string path, CancellationToken cancellationToken = default);
}

public class DefaultFileUtil : IFileUtil
{
    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(File.Exists(path));
    }
    
    public async Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
    {
        return await File.ReadAllTextAsync(path, cancellationToken);
    }
    
    public async Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default)
    {
        await File.WriteAllTextAsync(path, content, cancellationToken);
    }
    
    public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
    {
        return await File.ReadAllBytesAsync(path, cancellationToken);
    }
    
    public async Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
    {
        await File.WriteAllBytesAsync(path, bytes, cancellationToken);
    }
    
    public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        File.Delete(path);
        return Task.CompletedTask;
    }
    
    public Task CopyAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default)
    {
        File.Copy(sourcePath, destinationPath, true);
        return Task.CompletedTask;
    }
    
    public Task MoveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default)
    {
        File.Move(sourcePath, destinationPath, true);
        return Task.CompletedTask;
    }
    
    public Task<bool> DirectoryExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Directory.Exists(path));
    }
    
    public Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(path);
        return Task.CompletedTask;
    }
    
    public Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken = default)
    {
        Directory.Delete(path, true);
        return Task.CompletedTask;
    }
    
    public Task<IEnumerable<string>> EnumerateFilesAsync(string path, CancellationToken cancellationToken = default)
    {
        var files = Directory.EnumerateFiles(path);
        return Task.FromResult(files);
    }
    
    public Task<FileInfo> GetFileInfoAsync(string path, CancellationToken cancellationToken = default)
    {
        var info = new FileInfo(path);
        return Task.FromResult(info);
    }
}

public static class Path
{
    public static string Combine(params string[] paths)
    {
        return System.IO.Path.Combine(paths);
    }
}

public class FileInfo
{
    public long Length { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime LastWriteTime { get; set; }
    
    public FileInfo(string path)
    {
        var info = new System.IO.FileInfo(path);
        Length = info.Length;
        CreationTime = info.CreationTime;
        LastWriteTime = info.LastWriteTime;
    }
}

public static class File
{
    public static bool Exists(string path) => System.IO.File.Exists(path);
    public static Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default) => System.IO.File.ReadAllTextAsync(path, cancellationToken);
    public static Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default) => System.IO.File.WriteAllTextAsync(path, content, cancellationToken);
    public static Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default) => System.IO.File.ReadAllBytesAsync(path, cancellationToken);
    public static Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default) => System.IO.File.WriteAllBytesAsync(path, bytes, cancellationToken);
    public static void Delete(string path) => System.IO.File.Delete(path);
    public static void Copy(string sourcePath, string destinationPath, bool overwrite) => System.IO.File.Copy(sourcePath, destinationPath, overwrite);
    public static void Move(string sourcePath, string destinationPath, bool overwrite) 
    {
        if (System.IO.File.Exists(destinationPath))
        {
            System.IO.File.Delete(destinationPath);
        }
        System.IO.File.Move(sourcePath, destinationPath);
    }
}

public static class Directory
{
    public static bool Exists(string path) => System.IO.Directory.Exists(path);
    public static void CreateDirectory(string path) => System.IO.Directory.CreateDirectory(path);
    public static void Delete(string path, bool recursive) => System.IO.Directory.Delete(path, recursive);
    public static IEnumerable<string> EnumerateFiles(string path) => System.IO.Directory.EnumerateFiles(path);
}
```

#### 6.2 加密工具详细示例

```csharp
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class CryptoUtilExample
{
    public static async Task Run()
    {
        Console.WriteLine("加密工具详细示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var cryptoUtil = serviceProvider.GetRequiredService<ICryptoUtil>();
        
        // 测试数据
        var testText = "这是一个测试文本，用于加密工具示例。";
        var password = "MySecurePassword123!";
        
        Console.WriteLine($"原始文本: {testText}");
        
        // 1. 对称加密 (AES)
        Console.WriteLine("\n1. AES对称加密测试:");
        var aesEncrypted = await cryptoUtil.EncryptAsync(testText);
        Console.WriteLine($"加密结果: {aesEncrypted}");
        
        var aesDecrypted = await cryptoUtil.DecryptAsync(aesEncrypted);
        Console.WriteLine($"解密结果: {aesDecrypted}");
        Console.WriteLine($"加密/解密成功: {testText == aesDecrypted}");
        
        // 2. 哈希计算
        Console.WriteLine("\n2. 哈希计算测试:");
        var md5Hash = await cryptoUtil.ComputeHashAsync(testText, "MD5");
        var sha1Hash = await cryptoUtil.ComputeHashAsync(testText, "SHA1");
        var sha256Hash = await cryptoUtil.ComputeHashAsync(testText, "SHA256");
        
        Console.WriteLine($"MD5哈希: {md5Hash}");
        Console.WriteLine($"SHA1哈希: {sha1Hash}");
        Console.WriteLine($"SHA256哈希: {sha256Hash}");
        
        // 3. 密码哈希
        Console.WriteLine("\n3. 密码哈希测试:");
        var passwordHash = await cryptoUtil.ComputePasswordHashAsync(password);
        Console.WriteLine($"密码哈希: {passwordHash}");
        
        var passwordValid = await cryptoUtil.VerifyPasswordHashAsync(password, passwordHash);
        var passwordInvalid = await cryptoUtil.VerifyPasswordHashAsync("WrongPassword", passwordHash);
        
        Console.WriteLine($"密码验证(正确): {passwordValid}");
        Console.WriteLine($"密码验证(错误): {passwordInvalid}");
        
        // 4. 随机数生成
        Console.WriteLine("\n4. 随机数生成测试:");
        var randomString = await cryptoUtil.GenerateRandomStringAsync(16);
        var randomNumber = await cryptoUtil.GenerateRandomNumberAsync(1000, 9999);
        
        Console.WriteLine($"随机字符串(16位): {randomString}");
        Console.WriteLine($"随机数字(1000-9999): {randomNumber}");
        
        Console.WriteLine("\n加密工具示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<ICryptoUtil, DefaultCryptoUtil>();
        return builder.BuildServiceProvider();
    }
}

public interface ICryptoUtil
{
    Task<string> EncryptAsync(string plainText, string key = null, CancellationToken cancellationToken = default);
    Task<string> DecryptAsync(string encryptedText, string key = null, CancellationToken cancellationToken = default);
    Task<string> ComputeHashAsync(string input, string algorithm = "SHA256", CancellationToken cancellationToken = default);
    Task<bool> VerifyHashAsync(string input, string hash, string algorithm = "SHA256", CancellationToken cancellationToken = default);
    Task<string> ComputePasswordHashAsync(string password, CancellationToken cancellationToken = default);
    Task<bool> VerifyPasswordHashAsync(string password, string hash, CancellationToken cancellationToken = default);
    Task<string> GenerateRandomStringAsync(int length, CancellationToken cancellationToken = default);
    Task<int> GenerateRandomNumberAsync(int min, int max, CancellationToken cancellationToken = default);
}

public class DefaultCryptoUtil : ICryptoUtil
{
    private const string DefaultKey = "UtilDefaultEncryptionKey1234567890";
    private readonly byte[] _key;
    private readonly byte[] _iv;
    
    public DefaultCryptoUtil()
    {
        using var sha256 = SHA256.Create();
        _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(DefaultKey));
        _iv = new byte[16]; // 简化处理，实际应用中应该使用随机IV
    }
    
    public async Task<string> EncryptAsync(string plainText, string key = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var keyBytes = string.IsNullOrEmpty(key) ? _key : SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key));
        
        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        
        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using var swEncrypt = new StreamWriter(csEncrypt);
        
        await swEncrypt.WriteAsync(plainText);
        await swEncrypt.FlushAsync();
        csEncrypt.FlushFinalBlock();
        
        var encrypted = msEncrypt.ToArray();
        return Convert.ToBase64String(encrypted);
    }
    
    public async Task<string> DecryptAsync(string encryptedText, string key = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var keyBytes = string.IsNullOrEmpty(key) ? _key : SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key));
        var cipherText = Convert.FromBase64String(encryptedText);
        
        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        
        using var decryptor = aes.CreateDecryptor();
        using var msDecrypt = new MemoryStream(cipherText);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        
        return await srDecrypt.ReadToEndAsync();
    }
    
    public Task<string> ComputeHashAsync(string input, string algorithm = "SHA256", CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        HashAlgorithm hashAlgorithm;
        switch (algorithm.ToUpper())
        {
            case "MD5":
                hashAlgorithm = MD5.Create();
                break;
            case "SHA1":
                hashAlgorithm = SHA1.Create();
                break;
            case "SHA256":
            default:
                hashAlgorithm = SHA256.Create();
                break;
        }
        
        using (hashAlgorithm)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = hashAlgorithm.ComputeHash(inputBytes);
            return Task.FromResult(BitConverter.ToString(hashBytes).Replace("-", "").ToLower());
        }
    }
    
    public Task<bool> VerifyHashAsync(string input, string hash, string algorithm = "SHA256", CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ComputeHashAsync(input, algorithm, cancellationToken).ContinueWith(t => t.Result.Equals(hash, StringComparison.OrdinalIgnoreCase));
    }
    
    public Task<string> ComputePasswordHashAsync(string password, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var salt = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);
        
        var hashBytes = new byte[48];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);
        
        return Task.FromResult(Convert.ToBase64String(hashBytes));
    }
    
    public Task<bool> VerifyPasswordHashAsync(string password, string hash, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var hashBytes = Convert.FromBase64String(hash);
        var salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);
        
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        var computedHash = pbkdf2.GetBytes(32);
        
        for (int i = 0; i < 32; i++)
        {
            if (hashBytes[i + 16] != computedHash[i])
                return Task.FromResult(false);
        }
        
        return Task.FromResult(true);
    }
    
    public Task<string> GenerateRandomStringAsync(int length, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringBuilder = new StringBuilder(length);
        var random = new Random();
        
        for (int i = 0; i < length; i++)
        {
            stringBuilder.Append(chars[random.Next(chars.Length)]);
        }
        
        return Task.FromResult(stringBuilder.ToString());
    }
    
    public Task<int> GenerateRandomNumberAsync(int min, int max, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var random = new Random();
        return Task.FromResult(random.Next(min, max + 1));
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUtilServices(this IServiceCollection services)
    {
        services.AddSingleton<ICryptoUtil, DefaultCryptoUtil>();
        return services;
    }
}
```

## 总结

以上示例展示了Util技能的主要功能和使用方法，包括：

1. **基本操作**：快速上手Util服务的核心功能
2. **高级配置**：详细的配置选项和自定义设置
3. **性能优化**：各种性能优化策略和测试方法
4. **错误处理**：全面的异常处理和故障排除
5. **Scrutor集成**：自动服务注册和装饰器模式的使用
6. **工具详细示例**：各种工具的具体使用方法

系统设计遵循.NET 10最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

通过这些示例，您可以快速掌握Util技能的使用方法，并根据实际项目需求进行扩展和定制。