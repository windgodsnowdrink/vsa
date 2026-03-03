# Util - 参考文档

## 概述

Util是基于.NET 10的高性能实用工具支持系统，专为.NET开发者设计的全方位工具集。提供统一的工具接口和高性能实现，支持依赖注入、配置管理、文件操作、加密解密、序列化等企业级功能。

## 核心组件

### 1. UtilService (核心服务)
- **位置**: scripts/util_core.cs
- **功能**: 核心业务逻辑处理
- **特性**: 
  - 核心功能实现
  - 性能优化
  - 错误处理
  - 日志记录
  - 依赖注入支持

### 2. FileUtil (文件工具)
- **功能**: 文件和目录操作
- **特性**:
  - 文件读写（同步/异步）
  - 目录创建和管理
  - 文件监控
  - 路径处理
  - 文件权限管理

### 3. CryptoUtil (加密工具)
- **功能**: 加密和解密操作
- **特性**:
  - 对称加密（AES）
  - 非对称加密（RSA）
  - 哈希计算（MD5, SHA-1, SHA-256）
  - 安全随机数生成
  - 密码哈希

### 4. SerializationUtil (序列化工具)
- **功能**: 对象序列化和反序列化
- **特性**:
  - JSON序列化
  - XML序列化
  - 二进制序列化
  - 自定义序列化器支持

### 5. ConfigUtil (配置工具)
- **功能**: 配置管理
- **特性**:
  - 配置文件读取
  - 环境变量管理
  - 配置验证
  - 配置变更监控

### 6. ReflectionUtil (反射工具)
- **功能**: 类型反射操作
- **特性**:
  - 类型信息获取
  - 动态方法调用
  - 属性操作
  - 构造函数调用

### 7. TimeUtil (时间工具)
- **功能**: 时间相关操作
- **特性**:
  - 时间格式化
  - 时间计算
  - 时区转换
  - 时间戳操作

### 8. NetworkUtil (网络工具)
- **功能**: 网络相关操作
- **特性**:
  - HTTP请求
  - 网络诊断
  - URL处理
  - 网络状态检查

## 使用示例

### 基本使用

```csharp
// 获取Util服务
var utilService = serviceProvider.GetRequiredService<IUtilService>();

// 使用文件工具
var fileExists = await utilService.FileUtil.ExistsAsync("appsettings.json");
if (fileExists)
{
    var content = await utilService.FileUtil.ReadAllTextAsync("appsettings.json");
    Console.WriteLine($"文件内容: {content}");
}

// 使用加密工具
var encrypted = await utilService.CryptoUtil.EncryptAsync("敏感信息");
var decrypted = await utilService.CryptoUtil.DecryptAsync(encrypted);
Console.WriteLine($"解密结果: {decrypted}");

// 使用序列化工具
var data = new { Name = "测试", Value = 123 };
var json = await utilService.SerializationUtil.SerializeToJsonAsync(data);
Console.WriteLine($"JSON: {json}");
```

### 高级配置

```csharp
// 配置Util服务
var utilSettings = new UtilSettings
{
    EnableCache = true,
    CacheSize = 1000,
    DefaultTimeout = 30,
    EnableDetailedLogging = true,
    MaxRetryAttempts = 3,
    EnableCompression = true
};

// 注册服务并配置
builder.Services.AddUtilServices();
builder.Services.Configure<UtilSettings>(options =>
{
    options.EnableCache = utilSettings.EnableCache;
    options.CacheSize = utilSettings.CacheSize;
    options.DefaultTimeout = utilSettings.DefaultTimeout;
    options.EnableDetailedLogging = utilSettings.EnableDetailedLogging;
    options.MaxRetryAttempts = utilSettings.MaxRetryAttempts;
    options.EnableCompression = utilSettings.EnableCompression;
});

// 注册自定义工具实现
builder.Services.AddSingleton<IFileUtil, CustomFileUtil>();
builder.Services.AddSingleton<ICryptoUtil, CustomCryptoUtil>();
```

### Scrutor集成使用

```csharp
// 使用Scrutor自动注册服务
builder.Services.Scan(scan => scan
    .FromAssemblyOf<IUtilService>()
    .AddClasses(classes => classes.AssignableTo<IUtilService>())
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
    .FromAssemblyOf<IFileUtil>()
    .AddClasses(classes => classes.AssignableTo<IFileUtil>())
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
    .FromAssemblyOf<ICryptoUtil>()
    .AddClasses(classes => classes.AssignableTo<ICryptoUtil>())
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
);

// 使用Scrutor的装饰器模式
builder.Services.Decorate<IFileUtil, CachedFileUtil>();
builder.Services.Decorate<ICryptoUtil, LoggingCryptoUtil>();
```

## 配置选项

### UtilSettings 配置

```json
{
  "UtilSettings": {
    "EnableCache": true,          // 启用缓存
    "CacheSize": 1000,            // 缓存大小
    "DefaultTimeout": 30,          // 默认超时时间(秒)
    "EnableDetailedLogging": false, // 启用详细日志
    "MaxRetryAttempts": 3,        // 最大重试次数
    "EnableCompression": true     // 启用压缩
  }
}
```

### 文件工具配置

```json
{
  "FileUtilSettings": {
    "BufferSize": 8192,            // 缓冲区大小
    "MaxFileSize": 104857600,      // 最大文件大小(100MB)
    "EnableFileWatch": false,      // 启用文件监控
    "WatchInterval": 5000          // 监控间隔(毫秒)
  }
}
```

### 加密工具配置

```json
{
  "CryptoUtilSettings": {
    "AesKeySize": 256,             // AES密钥大小
    "RsaKeySize": 2048,            // RSA密钥大小
    "HashAlgorithm": "SHA256",     // 哈希算法
    "KeyStoragePath": "keys/",     // 密钥存储路径
    "EnableKeyRotation": false     // 启用密钥轮换
  }
}
```

## 性能优化

1. **缓存使用**: 启用缓存以提高性能
2. **异步编程**: 使用异步API避免阻塞
3. **批处理**: 批量处理以提高效率
4. **连接池**: 使用连接池管理资源
5. **内存优化**: 使用Span和Memory减少内存分配
6. **并行处理**: 对CPU密集型任务使用并行处理
7. **延迟加载**: 按需加载资源
8. **资源重用**: 使用对象池重用对象

## 故障排除

### 常见问题

1. **文件操作失败**
   - 检查文件路径是否正确
   - 验证文件权限
   - 检查磁盘空间
   - 查看详细日志

2. **加密操作失败**
   - 检查密钥是否正确
   - 验证加密算法参数
   - 确保密钥长度符合要求

3. **性能问题**
   - 启用缓存
   - 优化文件操作
   - 减少加密强度（如果适用）
   - 使用异步API

4. **配置问题**
   - 检查配置文件格式
   - 验证环境变量
   - 确保配置键名正确

## 扩展开发

### 添加自定义工具实现

```csharp
// 实现自定义文件工具
public class CustomFileUtil : IFileUtil
{
    private readonly ILogger<CustomFileUtil> _logger;
    private readonly FileUtilSettings _settings;

    public CustomFileUtil(ILogger<CustomFileUtil> logger, IOptions<FileUtilSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Reading file: {Path}", path);
        // 实现自定义读取逻辑
        return await File.ReadAllTextAsync(path, cancellationToken);
    }

    // 实现其他方法...
}

// 注册自定义实现
builder.Services.AddSingleton<IFileUtil, CustomFileUtil>();
```

### 使用Scrutor自动注册

```csharp
// 自动注册所有实现了IUtilService接口的服务
builder.Services.Scan(scan => scan
    .FromAssembliesOf(typeof(IUtilService), typeof(IFileUtil), typeof(ICryptoUtil))
    .AddClasses(classes => classes.Where(type => 
        type.GetInterfaces().Any(i => 
            i.Name.StartsWith("I") && i.Name.EndsWith("Util")
        )
    ))
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
);

// 注册装饰器
builder.Services.Decorate<IFileUtil, CachedFileUtil>();
builder.Services.Decorate<ICryptoUtil, LoggingCryptoUtil>();
builder.Services.Decorate<ISerializationUtil, CompressedSerializationUtil>();
```

## 最佳实践

1. **依赖注入**: 始终使用依赖注入获取服务
2. **异步优先**: 优先使用异步API
3. **错误处理**: 正确处理所有异常
4. **日志记录**: 在关键操作中添加日志
5. **资源管理**: 使用using语句管理可释放资源
6. **配置验证**: 验证配置参数的有效性
7. **性能监控**: 监控关键操作的性能
8. **安全考虑**: 对敏感操作进行适当的安全处理

## 版本兼容性

| .NET版本 | 支持状态 | 备注 |
|---------|---------|------|
| .NET 10 | ✅ 完全支持 | 推荐版本 |
| .NET 9 | ✅ 完全支持 | 无特殊要求 |
| .NET 8 | ✅ 基本支持 | 部分高级功能受限 |
| .NET 7 | ❌ 不支持 | 最低要求 .NET 8 |

## 限制和注意事项

1. **文件操作限制**：
   - 大文件操作可能需要更多内存
   - 网络文件操作依赖网络连接
   - 文件监控可能会增加系统负载

2. **加密注意事项**：
   - 加密密钥需要安全存储
   - 高强度加密可能影响性能
   - 密钥管理需要额外考虑

3. **序列化限制**：
   - 复杂对象序列化可能较慢
   - 循环引用可能导致问题

4. **反射注意事项**：
   - 频繁的反射操作可能影响性能
   - AOT环境下反射受限

5. **使用建议**：
   - 只在必要时使用加密功能
   - 合理设置缓存大小
   - 定期清理不再使用的资源
   - 监控工具使用的性能和内存占用
   - 对关键操作进行单元测试
