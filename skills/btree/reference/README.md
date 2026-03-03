# btree - 参考文档

## 概述

btree 是一个基于 .NET 10 的高性能 B-Tree 数据结构系统，专为 .NET 开发者设计，支持 AOT（提前编译）编译，提供强大的索引和数据存储功能。

## 核心组件

### 1. B-Tree 数据结构
- **位置**: 应用程序中的 B-Tree 类
- **功能**: 提供高性能的 B-Tree 和 B+Tree 数据结构实现
- **特性**: 
  - 支持 AOT 编译优化
  - 高性能数据插入、删除和搜索
  - 支持并发访问
  - 支持多种数据类型
  - 支持持久化存储
  - 支持范围查询

### 2. B-Tree 服务
- **位置**: 应用程序中的 B-Tree 服务类
- **功能**: 管理 B-Tree 实例和操作
- **特性**: 
  - 支持 B-Tree 实例创建和管理
  - 支持事务处理
  - 支持监控和统计
  - 支持配置管理

### 3. ZoneTree 集成
- **位置**: scripts/zonetree_integration.cs
- **功能**: 与 ZoneTree 库的集成
- **特性**: 
  - 支持持久化 B-Tree
  - 支持高并发访问
  - 支持事务处理
  - 支持 AOT 编译优化

## 使用示例

### 基本 B-Tree 使用

```csharp
// 创建 B-Tree 实例
var btree = new BTree<int, string>(order: 5);

// 插入数据
await btree.InsertAsync(1, "苹果");
await btree.InsertAsync(2, "香蕉");
await btree.InsertAsync(3, "橙子");
await btree.InsertAsync(4, "葡萄");
await btree.InsertAsync(5, "西瓜");

// 搜索数据
var result = await btree.SearchAsync(3);
Console.WriteLine($"搜索结果: {result}");

// 删除数据
await btree.DeleteAsync(4);

// 范围查询
var rangeResult = await btree.RangeSearchAsync(1, 5);
foreach (var item in rangeResult)
{
    Console.WriteLine($"{item.Key}: {item.Value}");
}
```

### 高级配置

```csharp
// 配置 B-Tree 选项
var options = new BTreeOptions
{
    Order = 10,                   // B-Tree 阶数
    EnableAotOptimization = true,  // 启用 AOT 优化
    EnableConcurrency = true,      // 启用并发支持
    EnableTransaction = true,      // 启用事务支持
    CacheSize = 1000               // 缓存大小
};

// 使用配置创建 B-Tree
var btree = new BTree<int, string>(options);
```

## 配置选项

### B-Tree 配置

```json
{
  "BTreeSettings": {
    "EnableAot": true,             // 启用 AOT 编译优化
    "Order": 10,                   // B-Tree 阶数
    "EnableConcurrency": true,      // 启用并发支持
    "EnableTransaction": true,      // 启用事务支持
    "CacheSize": 1000,              // 缓存大小
    "EnableLogging": false,         // 启用日志记录
    "EnableMetrics": true,          // 启用指标收集
    "PersistToDisk": false,         // 启用持久化到磁盘
    "MaxDegreeOfParallelism": 4     // 最大并行度
  }
}
```

### ZoneTree 配置

```json
{
  "ZoneTreeSettings": {
    "EnableAot": true,             // 启用 AOT 编译优化
    "DataDirectory": "./data",    // 数据存储目录
    "SegmentSize": 1048576,        // 段大小（1MB）
    "MaxRecordSize": 1024,         // 最大记录大小
    "EnableCompression": true,     // 启用压缩
    "EnableChecksum": true,        // 启用校验和
    "MaxCacheSize": 1073741824     // 最大缓存大小（1GB）
  }
}
```

## 性能优化

1. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译以获得最佳性能
2. **选择合适的阶数**: 根据实际数据量选择合适的 B-Tree 阶数
3. **使用异步 API**: 优先使用异步 API 避免阻塞主线程
4. **启用并发支持**: 对于高并发场景，启用并发支持
5. **优化缓存配置**: 根据实际情况调整缓存大小
6. **使用范围查询**: 对于范围查询，使用 B-Tree 的范围查询功能
7. **避免频繁插入和删除**: 尽量减少频繁的插入和删除操作
8. **使用批量操作**: 对于大量数据，考虑使用批量操作
9. **优化数据结构**: 确保键和值的数据结构高效
10. **监控性能**: 定期监控 B-Tree 性能，及时调整配置

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

<!-- 添加 AOT 兼容的依赖 -->
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Aot" Version="10.0.0" />
</ItemGroup>
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

1. **使用 AOT 兼容的库**: 确保使用的 B-Tree 库支持 AOT 编译
2. **避免反射**: 避免在 B-Tree 操作中使用反射
3. **资源加载**: 确保所有资源在 AOT 编译时能被正确处理
4. **动态代码生成**: 避免使用动态代码生成技术
5. **泛型类型**: 限制使用复杂的泛型类型
6. **序列化**: 确保所有需要序列化的类型都具有公共无参数构造函数
7. **测试验证**: 在 AOT 编译后进行充分测试

## 故障排除

### 常见问题

1. **插入性能问题**
   - 检查 B-Tree 阶数是否合适
   - 考虑启用并发支持
   - 考虑使用批量插入
   - 考虑使用 AOT 编译

2. **搜索性能问题**
   - 检查 B-Tree 阶数是否合适
   - 考虑增加缓存大小
   - 考虑使用 AOT 编译
   - 检查键的比较器是否高效

3. **内存占用过高**
   - 减少缓存大小
   - 考虑使用持久化存储
   - 优化键和值的数据结构
   - 考虑使用压缩

4. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 检查是否使用了反射等不兼容的特性
   - 查看详细的编译日志
   - 确保所有依赖都支持 AOT 编译

5. **并发访问问题**
   - 确保启用了并发支持
   - 检查锁的使用是否合理
   - 考虑使用更细粒度的锁

6. **持久化存储问题**
   - 检查磁盘空间是否充足
   - 检查文件权限
   - 查看持久化日志
   - 考虑使用更可靠的存储方案

## 扩展开发

### 自定义比较器

```csharp
// 自定义字符串比较器
public class CaseInsensitiveStringComparer : IComparer<string>
{
    public int Compare(string x, string y)
    {
        return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
    }
}

// 使用自定义比较器创建 B-Tree
var btree = new BTree<string, int>(new CaseInsensitiveStringComparer(), order: 5);
```

### 自定义数据类型

```csharp
// 自定义数据类型
public class Person : IComparable<Person>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    
    public int CompareTo(Person other)
    {
        if (other == null) return 1;
        return Id.CompareTo(other.Id);
    }
}

// 使用自定义数据类型创建 B-Tree
var btree = new BTree<Person, string>(order: 5);

// 插入数据
await btree.InsertAsync(
    new Person { Id = 1, Name = "张三", Age = 30 },
    "工程师");
```

### 扩展 B-Tree 功能

```csharp
// 扩展 B-Tree 类
public class ExtendedBTree<TKey, TValue> : BTree<TKey, TValue>
    where TKey : IComparable<TKey>
{
    public ExtendedBTree(int order) : base(order) { }
    
    // 添加自定义方法
    public async Task<int> CountGreaterThanAsync(TKey key)
    {
        var count = 0;
        var enumerator = await RangeSearchAsync(key, default, includeLower: false);
        await foreach (var item in enumerator)
        {
            count++;
        }
        return count;
    }
    
    public async Task<int> CountLessThanAsync(TKey key)
    {
        var count = 0;
        var enumerator = await RangeSearchAsync(default, key, includeUpper: false);
        await foreach (var item in enumerator)
        {
            count++;
        }
        return count;
    }
}

// 使用扩展 B-Tree
var extendedBTree = new ExtendedBTree<int, string>(order: 5);
var count = await extendedBTree.CountGreaterThanAsync(5);
```

## 最佳实践

1. **使用依赖注入**: 始终使用依赖注入管理 B-Tree 服务
2. **采用异步 API**: 优先使用异步 API 避免阻塞主线程
3. **合理设置阶数**: 根据实际数据量选择合适的 B-Tree 阶数
4. **使用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译
5. **添加适当的日志**: 添加详细的日志记录，便于调试和监控
6. **监控性能**: 定期监控 B-Tree 性能，及时调整配置
7. **使用事务**: 对于关键操作，使用事务确保数据一致性
8. **优化内存使用**: 根据实际情况优化内存使用
9. **测试性能**: 定期测试 B-Tree 性能，确保满足需求
10. **遵循 B-Tree 最佳实践**: 遵循 B-Tree 数据结构的最佳实践

## 与其他系统集成

### 与 ASP.NET Core 集成

```csharp
// 在 Program.cs 中配置
var builder = WebApplication.CreateBuilder(args);

// 注册 B-Tree 服务
builder.Services.AddSingleton<IBTreeFactory, BTreeFactory>();
builder.Services.AddSingleton<IZoneTreeService, ZoneTreeService>();

// 注册 API 服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 配置中间件
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### 与缓存系统集成

```csharp
// 与 Redis 集成
public class RedisBTreeCache<TKey, TValue> : IBTreeCache<TKey, TValue>
{
    private readonly IDistributedCache _cache;
    private readonly IBTree<TKey, TValue> _btree;
    
    public RedisBTreeCache(IDistributedCache cache, IBTree<TKey, TValue> btree)
    {
        _cache = cache;
        _btree = btree;
    }
    
    public async Task<TValue> GetAsync(TKey key)
    {
        // 先从 Redis 缓存获取
        var cacheKey = $"btree:{key}";
        var cachedValue = await _cache.GetStringAsync(cacheKey);
        
        if (!string.IsNullOrEmpty(cachedValue))
        {
            return JsonSerializer.Deserialize<TValue>(cachedValue);
        }
        
        // 从 B-Tree 获取
        var value = await _btree.SearchAsync(key);
        if (value != null)
        {
            // 放入 Redis 缓存
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(value));
        }
        
        return value;
    }
    
    // 其他方法实现...
}
```

## 部署建议

1. **容器化部署**: 使用 Docker 容器化应用，便于部署和扩展
2. **使用 Kubernetes**: 对于分布式应用，使用 Kubernetes 进行编排
3. **使用 CI/CD 流程**: 实现自动化构建、测试和部署
4. **使用环境变量**: 使用环境变量配置应用，便于不同环境部署
5. **实施监控**: 实施监控和告警，及时发现问题
6. **备份策略**: 对于持久化存储，实施适当的备份策略
7. **使用 AOT 编译**: 对于生产环境，考虑使用 AOT 编译提高性能
8. **优化资源配置**: 根据实际负载优化资源配置

## 总结

B-Tree 技能提供了一套完整的 .NET 10 B-Tree 解决方案，支持 AOT 编译优化，适用于构建高性能、可扩展的数据存储系统。通过遵循最佳实践和合理配置，可以构建出可靠、高效的 B-Tree 应用。
