# bogus - 参考文档

## 概述

bogus 是一个基于 .NET 10 的高性能 Bogus 模拟数据生成系统，专为 .NET 开发者设计，支持 AOT（提前编译）编译，提供极致的模拟数据生成性能。

## 核心组件

### 1. Bogus 服务 (Bogus Service)
- **位置**: scripts/bogus_integration.cs, scripts/bogus_advanced_integration.cs
- **功能**: 核心模拟数据生成逻辑处理
- **特性**: 
  - 高性能模拟数据生成
  - 支持多种数据类型
  - 支持自定义数据模板
  - 支持批量数据生成
  - 支持 AOT 编译优化
  - 错误处理和日志记录
  - 支持多种数据格式输出

### 2. 数据生成器 (Data Generator)
- **功能**: 负责具体的数据生成逻辑
- **支持的数据类型**: 
  - 字符串（姓名、邮箱、地址、电话号码等）
  - 数字（整数、小数、百分比等）
  - 日期和时间
  - GUID
  - 布尔值
  - 枚举类型
  - 自定义对象

### 3. 数据模板引擎 (Data Template Engine)
- **功能**: 支持自定义数据生成模板
- **特性**: 
  - 支持 Lambda 表达式定义数据生成规则
  - 支持条件数据生成
  - 支持关联数据生成
  - 支持数据验证

## 使用示例

### 基本用法

```csharp
using Bogus;
using Microsoft.Extensions.DependencyInjection;

// 创建服务容器
var services = new ServiceCollection();

// 注册 Bogus 服务
services.AddSingleton<Faker, Faker>();
services.AddSingleton<IFakerService, FakerService>();

var serviceProvider = services.BuildServiceProvider();

// 获取 Bogus 服务
var fakerService = serviceProvider.GetRequiredService<IFakerService>();

// 生成单个模拟用户
var user = fakerService.GenerateUser();
Console.WriteLine($"生成的用户: {user.Name}, {user.Email}, {user.PhoneNumber}");
```

### 高级配置

```csharp
// 配置自定义 Bogus 设置
var settings = new BogusSettings {
    Locale = "zh_CN",           // 使用中文数据
    Seed = 12345,                // 固定种子，生成可重现的数据
    EnableCache = true,          // 启用缓存
    CacheSize = 1000,            // 缓存大小
    EnableDetailedLogging = false // 启用详细日志
};

// 注册自定义设置
builder.Services.Configure<BogusSettings>(options => {
    options.Locale = settings.Locale;
    options.Seed = settings.Seed;
    options.EnableCache = settings.EnableCache;
    options.CacheSize = settings.CacheSize;
    options.EnableDetailedLogging = settings.EnableDetailedLogging;
});
```

## 配置选项

### Bogus 配置

```json
{
  "BogusSettings": {
    "Locale": "zh_CN",           // 数据语言（zh_CN, en_US, ja_JP 等）
    "Seed": 12345,                // 随机种子
    "EnableCache": true,          // 启用缓存
    "CacheSize": 1000,            // 缓存大小
    "EnableDetailedLogging": false, // 启用详细日志
    "BatchSize": 1000,            // 批量生成大小
    "MaxRetryCount": 5,           // 最大重试次数
    "TimeoutMilliseconds": 30000  // 超时时间（毫秒）
  }
}
```

## 性能优化

1. **启用缓存**: 启用缓存可以提高重复数据生成的性能
2. **异步编程**: 使用异步 API 避免阻塞主线程
3. **批量处理**: 批量生成数据以提高效率
4. **固定种子**: 使用固定种子生成可重现的数据，便于调试和测试
5. **AOT 编译**: 启用 AOT 编译以获得最佳性能
6. **合理的批量大小**: 根据内存情况调整批量生成大小
7. **避免不必要的验证**: 对于性能敏感场景，可适当减少数据验证
8. **使用高性能数据结构**: 使用数组等高性能数据结构存储生成的数据
9. **并发生成**: 对于大量数据生成，考虑使用并发生成
10. **优化数据模板**: 优化自定义数据模板，减少不必要的计算

## AOT 编译优化

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

### AOT 兼容注意事项

1. **使用 AOT 兼容的 Bogus 版本**: 确保使用的 Bogus 版本支持 AOT 编译
2. **避免反射**: 避免在数据生成过程中使用反射
3. **资源加载**: 确保所有资源在 AOT 编译时能被正确处理
4. **动态代码生成**: 避免使用动态代码生成技术
5. **使用 Source Generator**: 对于需要动态生成代码的场景，考虑使用 Source Generator
6. **测试验证**: 在 AOT 编译后进行充分测试

## 故障排除

### 常见问题

1. **数据生成性能不佳**
   - 启用缓存
   - 调整批量生成大小
   - 考虑使用 AOT 编译
   - 优化数据生成模板

2. **生成的数据不符合预期**
   - 检查数据生成模板
   - 确保使用了正确的种子
   - 检查数据验证规则

3. **AOT 编译失败**
   - 检查 Bogus 版本是否支持 AOT
   - 检查是否使用了不兼容的特性
   - 查看详细的编译日志

4. **内存占用过高**
   - 减少批量生成大小
   - 优化数据结构
   - 及时释放不再使用的数据

5. **数据生成超时**
   - 增加超时时间
   - 减少单次生成的数据量
   - 优化数据生成逻辑

## 扩展开发

### 添加自定义数据生成器

```csharp
// 自定义数据生成器接口
public interface ICustomDataGenerator
{
    T Generate<T>(string template);
    IEnumerable<T> GenerateMany<T>(string template, int count);
}

// 自定义数据生成器实现
public class CustomDataGenerator : ICustomDataGenerator
{
    private readonly Faker _faker;
    
    public CustomDataGenerator(Faker faker)
    {
        _faker = faker;
    }
    
    public T Generate<T>(string template)
    {
        // 实现自定义数据生成逻辑
        // 示例：生成符合模板的自定义对象
        if (typeof(T) == typeof(CustomObject))
        {
            return (T)(object)new CustomObject
            {
                Id = _faker.Random.Guid(),
                Name = _faker.Company.CompanyName(),
                Description = _faker.Lorem.Paragraph(),
                CreatedAt = _faker.Date.Past()
            };
        }
        
        throw new NotSupportedException($"不支持的类型: {typeof(T).Name}");
    }
    
    public IEnumerable<T> GenerateMany<T>(string template, int count)
    {
        // 批量生成自定义数据
        for (int i = 0; i < count; i++)
        {
            yield return Generate<T>(template);
        }
    }
}
```

### 添加自定义数据模板

```csharp
// 自定义数据模板
public class CustomDataTemplate
{
    public static void Configure(Faker faker)
    {
        // 配置自定义数据生成规则
        faker.CustomObject()
            .RuleFor(o => o.Id, f => f.Random.Guid())
            .RuleFor(o => o.Name, f => f.Company.CompanyName())
            .RuleFor(o => o.Description, f => f.Lorem.Paragraph())
            .RuleFor(o => o.CreatedAt, f => f.Date.Past())
            .RuleFor(o => o.IsActive, f => f.Random.Bool())
            .RuleFor(o => o.Price, f => f.Finance.Amount(10, 1000));
    }
}

// 使用自定义数据模板
CustomDataTemplate.Configure(faker);
var customObject = faker.CustomObject();
```

## 最佳实践

1. **使用依赖注入**: 始终使用依赖注入管理 Bogus 服务
2. **合理设置种子**: 对于测试场景，使用固定种子生成可重现的数据
3. **优化批量大小**: 根据内存情况调整批量生成大小
4. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译
5. **使用异步 API**: 对于大量数据生成，使用异步 API 避免阻塞
6. **及时释放资源**: 生成大量数据后，及时释放不再使用的资源
7. **添加适当的日志**: 添加适当的日志记录，便于调试和监控
8. **测试性能**: 定期测试数据生成性能，确保满足需求
9. **优化数据模板**: 优化自定义数据模板，减少不必要的计算
10. **使用缓存**: 对于重复生成的数据，考虑使用缓存

## 与其他系统集成

### 与 ASP.NET Core 集成

```csharp
// 在 Startup.cs 中配置
public void ConfigureServices(IServiceCollection services)
{
    // 配置 Bogus 设置
    services.Configure<BogusSettings>(Configuration.GetSection("BogusSettings"));
    
    // 注册 Bogus 服务
    services.AddSingleton<Faker, Faker>();
    services.AddSingleton<IFakerService, FakerService>();
    services.AddSingleton<IBogusGenerator, BogusGenerator>();
    
    // 其他服务配置
}
```

### 与数据库集成

```csharp
// 数据库测试数据生成器
public class DatabaseTestDataGenerator
{
    private readonly IFakerService _fakerService;
    private readonly ApplicationDbContext _dbContext;
    
    public DatabaseTestDataGenerator(IFakerService fakerService, ApplicationDbContext dbContext)
    {
        _fakerService = fakerService;
        _dbContext = dbContext;
    }
    
    public async Task GenerateTestDataAsync(int userCount, int productCount)
    {
        // 生成模拟用户
        var users = _fakerService.GenerateUsers(userCount);
        await _dbContext.Users.AddRangeAsync(users);
        await _dbContext.SaveChangesAsync();
        
        // 生成模拟产品
        var products = _fakerService.GenerateProducts(productCount);
        await _dbContext.Products.AddRangeAsync(products);
        await _dbContext.SaveChangesAsync();
        
        // 生成模拟订单
        var orders = _fakerService.GenerateOrders(userCount, productCount);
        await _dbContext.Orders.AddRangeAsync(orders);
        await _dbContext.SaveChangesAsync();
    }
}
