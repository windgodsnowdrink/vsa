# bogus Agent Skill - bogus 技能

## 技能概述

基于 .NET 10 的高性能 Bogus 模拟数据生成技能，为 .NET 开发者提供强大的模拟数据生成功能，支持 AOT 编译优化，适用于各种测试、演示和开发场景。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:package Bogus@35.1.2
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package System.Text.Json@10.0.0
```

### 注册服务

在您的主应用程序中注册 Bogus 服务：

```csharp
// 注册 Bogus 服务
builder.Services.AddSingleton<IFakerService, FakerService>();
builder.Services.AddSingleton<IBogusGenerator, BogusGenerator>();
```

### 使用示例

```csharp
// 获取 Bogus 服务
var fakerService = serviceProvider.GetRequiredService<IFakerService>();

// 生成单个模拟用户
var user = fakerService.GenerateUser();
Console.WriteLine($"生成的用户: {user.Name}, {user.Email}");

// 批量生成模拟用户（100个）
var users = fakerService.GenerateUsers(100);
Console.WriteLine($"批量生成 {users.Count} 个用户");
```

## 导航地图

```
bogus/
├── index.yaml                           # 元数据索引描述
├── SKILL.md                            # 技能入口点（当前文件）
├── reference/                          # 参考文件
│   ├── README.md                      # 完整功能描述
│   └── examples.md                    # 使用示例
├── scripts/                            # 脚本和工具
    ├── bogus_integration.cs            # Bogus 集成示例
    ├── bogus_integration.run.json      # 运行配置
    ├── bogus_integration.setting.json  # 设置文件
    ├── bogus_advanced_integration.cs   # Bogus 高级集成示例
    ├── bogus_advanced_integration.run.json      # 运行配置
    └── bogus_advanced_integration.setting.json  # 设置文件
```

## 主要功能

1. **高性能模拟数据生成**: 基于 Bogus 库的高性能模拟数据生成
2. **支持多种数据类型**: 支持字符串、数字、日期、GUID、地址、姓名等多种数据类型
3. **支持自定义数据模板**: 支持根据业务需求自定义数据生成模板
4. **支持批量数据生成**: 支持一次性生成大量模拟数据
5. **支持 AOT 编译优化**: 支持将应用编译为本机代码，提高数据生成性能
6. **支持多种数据格式输出**: 支持 JSON、XML、CSV 等多种数据格式输出
7. **支持多语言数据生成**: 支持生成不同语言的模拟数据
8. **支持关联数据生成**: 支持生成具有关联关系的模拟数据
9. **支持数据验证**: 支持生成符合验证规则的模拟数据
10. **支持随机数据生成**: 支持生成具有随机性的模拟数据
11. **支持种子数据生成**: 支持基于种子生成可重现的模拟数据
12. **支持自定义数据提供者**: 支持自定义数据生成规则和提供者

## 扩展说明

此技能提供完整的 Bogus 模拟数据生成解决方案，您可以根据需要进行扩展：

1. **自定义数据模板**: 编写自定义数据生成模板和规则
2. **扩展数据类型**: 添加新的数据类型支持
3. **集成新的数据源**: 与外部数据源集成，生成更真实的模拟数据
4. **添加新的输出格式**: 支持更多数据输出格式
5. **优化性能**: 根据特定场景优化数据生成性能
6. **添加数据验证**: 为生成的数据添加验证规则

## 最佳实践

1. **依赖注入**: 使用依赖注入管理 Bogus 服务
2. **异步编程**: 优先使用异步 API 避免阻塞主线程
3. **批量生成**: 批量生成数据以提高性能
4. **合理使用种子**: 使用固定种子生成可重现的测试数据
5. **自定义模板**: 根据业务需求自定义数据生成模板
6. **数据验证**: 对生成的数据进行验证，确保数据质量
7. **内存管理**: 生成大量数据时注意内存管理
8. **AOT 编译**: 对于性能敏感场景，使用 AOT 编译提高性能
9. **错误处理**: 适当处理数据生成过程中的异常
10. **日志记录**: 添加适当的日志记录，便于调试和监控

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

1. **使用 AOT 兼容的 Bogus 版本**: 确保使用的 Bogus 版本支持 AOT 编译
2. **避免反射**: 避免在数据生成过程中使用反射，或使用 Source Generator 替代
3. **资源加载**: 确保所有资源在 AOT 编译时能被正确处理
4. **动态代码生成**: 避免使用动态代码生成技术
5. **测试验证**: 在 AOT 编译后进行充分测试
6. **性能优化**: AOT 编译可以提高数据生成性能，减少启动时间
7. **内存使用**: AOT 编译可以减少运行时内存占用

## 与其他系统集成

### 与 ASP.NET Core 集成

```csharp
// ASP.NET Core 应用中集成 Bogus
[ApiController]
[Route("[controller]")]
public class FakeDataController : ControllerBase
{
    private readonly IFakerService _fakerService;
    
    public FakeDataController(IFakerService fakerService)
    {
        _fakerService = fakerService;
    }
    
    [HttpGet("users")]
    public IActionResult GetFakeUsers([FromQuery] int count = 10)
    {
        // 生成指定数量的模拟用户
        var users = _fakerService.GenerateUsers(count);
        return Ok(users);
    }
    
    [HttpGet("products")]
    public IActionResult GetFakeProducts([FromQuery] int count = 10)
    {
        // 生成指定数量的模拟产品
        var products = _fakerService.GenerateProducts(count);
        return Ok(products);
    }
}
```

### 与数据库集成

```csharp
// 与数据库集成，生成测试数据
public class TestDataGenerator
{
    private readonly IFakerService _fakerService;
    private readonly ApplicationDbContext _dbContext;
    
    public TestDataGenerator(IFakerService fakerService, ApplicationDbContext dbContext)
    {
        _fakerService = fakerService;
        _dbContext = dbContext;
    }
    
    public async Task GenerateTestDataAsync(int userCount, int productCount)
    {
        // 生成模拟用户
        var users = _fakerService.GenerateUsers(userCount);
        await _dbContext.Users.AddRangeAsync(users);
        
        // 生成模拟产品
        var products = _fakerService.GenerateProducts(productCount);
        await _dbContext.Products.AddRangeAsync(products);
        
        // 保存到数据库
        await _dbContext.SaveChangesAsync();
    }
}
