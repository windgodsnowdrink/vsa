# btree Agent Skill - btree 技能

## 技能概述

基于 .NET 10 的高性能 B-Tree 数据结构技能，为 .NET 开发者提供强大的索引和数据存储功能，支持 AOT（提前编译）编译，适用于构建高性能、可扩展的数据存储系统。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package ZoneTree@1.0.0
```

### 注册服务

```csharp
// 注册 B-Tree 服务
builder.Services.AddSingleton<IBTreeService, BTreeService>();
builder.Services.AddSingleton<IBTreeFactory, BTreeFactory>();
builder.Services.AddSingleton<IZoneTreeService, ZoneTreeService>();
```

### 使用示例

```csharp
// 创建 B-Tree
var btreeFactory = serviceProvider.GetRequiredService<IBTreeFactory>();
var btree = btreeFactory.Create<int, string>();

// 插入数据
await btree.InsertAsync(1, "Hello");
await btree.InsertAsync(2, "World");
await btree.InsertAsync(3, "B-Tree");

// 查询数据
var result = await btree.SearchAsync(2);
Console.WriteLine($"查询结果: {result}");

// 范围查询
var rangeResult = await btree.RangeSearchAsync(1, 3);
foreach (var item in rangeResult)
{
    Console.WriteLine($"范围查询: {item.Key} => {item.Value}");
}
```

## 导航地图

```
btree/
├── index.yaml                           # 元数据索引描述
├── SKILL.md                            # 技能入口点（当前文件）
├── reference/                          # 参考文件
│   ├── README.md                      # 完整功能描述
│   └── examples.md                    # 使用示例
├── scripts/                            # 脚本和工具
    ├── btree_integration.cs           # B-Tree 集成示例
    ├── btree_integration.run.json     # 运行配置
    ├── btree_integration.setting.json # 设置文件
    ├── zonetree_integration.cs        # ZoneTree 集成示例
    ├── zonetree_integration.run.json  # 运行配置
    └── zonetree_integration.setting.json # 设置文件
```

## 主要功能

1. **B-Tree 和 B+Tree 实现**: 提供高性能的 B-Tree 和 B+Tree 数据结构实现
2. **支持 AOT 编译优化**: 支持将应用编译为本机代码，提高运行时性能
3. **高性能数据操作**: 支持高性能的数据插入、删除和搜索操作
4. **事务处理**: 支持事务性数据操作，确保数据一致性
5. **并发访问**: 支持并发访问，充分利用多核 CPU
6. **多种数据类型**: 支持多种数据类型的存储和检索
7. **持久化存储**: 支持将数据持久化到磁盘
8. **范围查询**: 支持高效的范围查询操作
9. **排序支持**: 内置排序功能
10. **内存优化**: 优化内存使用，减少内存占用

## 扩展说明

此技能提供完整的 B-Tree 解决方案，您可以根据需要进行扩展：

1. **自定义数据类型**: 支持自定义数据类型的存储和检索
2. **自定义比较器**: 支持自定义比较器，实现自定义排序规则
3. **扩展持久化存储**: 支持扩展到不同的持久化存储后端
4. **添加新的索引类型**: 支持添加新的索引类型
5. **优化性能**: 根据特定场景优化性能
6. **集成其他系统**: 与其他系统集成，如数据库、消息中间件等

## 最佳实践

1. **使用依赖注入**: 始终使用依赖注入管理 B-Tree 服务
2. **采用异步 API**: 优先使用异步 API 避免阻塞主线程
3. **合理设置树的阶数**: 根据实际情况设置 B-Tree 的阶数
4. **使用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译
5. **添加适当的日志**: 添加详细的日志记录，便于调试和监控
6. **监控性能**: 定期监控系统性能，及时发现瓶颈
7. **使用事务**: 对于关键操作，使用事务确保数据一致性
8. **优化内存使用**: 根据实际情况优化内存使用
9. **测试性能**: 定期测试系统性能，确保满足需求
10. **遵循 B-Tree 最佳实践**: 遵循 B-Tree 数据结构的最佳实践

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

1. **使用 AOT 兼容的库**: 确保使用的 B-Tree 库支持 AOT 编译
2. **避免反射**: 避免在 B-Tree 操作中使用反射
3. **资源加载**: 确保所有资源在 AOT 编译时能被正确处理
4. **动态代码生成**: 避免使用动态代码生成技术
5. **测试验证**: 在 AOT 编译后进行充分测试
6. **性能比较**: 比较 JIT 和 AOT 编译后的性能差异

## 与其他系统集成

### 与 ASP.NET Core 集成

```csharp
// 在 Startup.cs 中配置
public void ConfigureServices(IServiceCollection services)
{
    // 注册 B-Tree 服务
    services.AddSingleton<IBTreeService, BTreeService>();
    services.AddSingleton<IBTreeFactory, BTreeFactory>();
    
    // 其他服务配置
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
}

// 在控制器中使用
[ApiController]
[Route("[controller]")]
public class DataController : ControllerBase
{
    private readonly IBTreeFactory _btreeFactory;
    
    public DataController(IBTreeFactory btreeFactory)
    {
        _btreeFactory = btreeFactory;
    }
    
    [HttpPost]
    public async Task<IActionResult> Insert([FromBody] KeyValuePair<int, string> data)
    {
        var btree = _btreeFactory.Create<int, string>();
        await btree.InsertAsync(data.Key, data.Value);
        return Ok();
    }
    
    [HttpGet("{key}")]
    public async Task<IActionResult> Get(int key)
    {
        var btree = _btreeFactory.Create<int, string>();
        var value = await btree.SearchAsync(key);
        if (value == null)
        {
            return NotFound();
        }
        return Ok(value);
    }
}
```

### 与数据库集成

```csharp
// 与 EF Core 集成
public class BTreeRepository<TKey, TValue> : IBTreeRepository<TKey, TValue>
{
    private readonly IBTree<TKey, TValue> _btree;
    private readonly AppDbContext _context;
    
    public BTreeRepository(IBTree<TKey, TValue> btree, AppDbContext context)
    {
        _btree = btree;
        _context = context;
    }
    
    public async Task InsertAsync(TKey key, TValue value)
    {
        // 插入到 B-Tree
        await _btree.InsertAsync(key, value);
        
        // 同时插入到数据库
        await _context.BTreeItems.AddAsync(new BTreeItem<TKey, TValue> { Key = key, Value = value });
        await _context.SaveChangesAsync();
    }
    
    public async Task<TValue> GetAsync(TKey key)
    {
        // 先从 B-Tree 查找
        var value = await _btree.SearchAsync(key);
        if (value != null)
        {
            return value;
        }
        
        // 如果 B-Tree 中没有，从数据库查找
        var item = await _context.BTreeItems.FindAsync(key);
        if (item != null)
        {
            // 将结果放入 B-Tree 缓存
            await _btree.InsertAsync(item.Key, item.Value);
            return item.Value;
        }
        
        return default;
    }
}
