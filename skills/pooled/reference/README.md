# pooled - 参考文档

## 概述

pooled是一个基于.NET 10的高性能内存池化系统，专为.NET开发者设计，提供了内存池化集合（List、Dictionary、HashSet）以减少内存分配和GC压力，支持AOT编译和.NET 10。

## 核心组件

### 1. 内存池化集合服务 (Pooled Collections Service)
- **位置**: scripts/collections_pooled_integration.cs
- **功能**: 提供内存池化集合的核心实现，包括PooledList、PooledDictionary、PooledSet等类型
- **特性**: 
  - 零分配操作
  - 内存重用
  - 线程安全
  - 自动清理未使用的池化对象
  - 线程本地存储优化

### 2. 池化集合工厂 (Pooled Collections Factory)
- **位置**: scripts/collections_pooled_integration.cs
- **功能**: 管理池化集合的创建和回收
- **特性**: 
  - 按需创建池化集合
  - 自动回收未使用的集合
  - 线程安全的工厂方法

### 3. 依赖注入扩展 (Dependency Injection Extensions)
- **位置**: scripts/collections_pooled_integration.cs
- **功能**: 提供DI集成，便于在应用中使用池化集合
- **特性**: 
  - 简单的服务注册
  - 配置选项支持
  - 生命周期管理

## 使用示例

### 基本使用

```csharp
// 注册服务
builder.Services.AddPooledCollections(options =>
{
    options.EnableMemoryPooling = true;
    options.EnableZeroAllocation = true;
    options.MemoryPoolSize = 1024;
});

// 在需要的地方注入
public class MyService
{
    private readonly IPooledListFactory _listFactory;
    private readonly IPooledDictionaryFactory _dictionaryFactory;

    public MyService(IPooledListFactory listFactory, IPooledDictionaryFactory dictionaryFactory)
    {
        _listFactory = listFactory;
        _dictionaryFactory = dictionaryFactory;
    }

    public void ProcessData()
    {
        // 使用池化List
        using var pooledList = _listFactory.Create<int>();
        var list = pooledList.Value;
        
        // 添加数据
        for (int i = 0; i < 1000; i++)
        {
            list.Add(i);
        }
        
        // 使用池化Dictionary
        using var pooledDictionary = _dictionaryFactory.Create<int, string>();
        var dictionary = pooledDictionary.Value;
        
        // 添加数据
        for (int i = 0; i < 1000; i++)
        {
            dictionary[i] = $"Value {i}";
        }
        
        // 处理数据...
    }
}
```

### 高级配置

```csharp
// 配置池化集合选项
var pooledOptions = new PooledCollectionsOptions
{
    EnableMemoryPooling = true,
    EnableZeroAllocation = true,
    EnableThreadLocalStorage = true,
    MemoryPoolSize = 2048,
    ThreadLocalCacheSize = 512,
    DefaultInitialCapacity = 16,
    MaximumPoolSize = 1024,
    EnableStrictMode = true,
    EnableAutoClear = true,
    ClearInterval = 5000,
    EnablePerformanceMetrics = true
};

// 注册服务并应用配置
builder.Services.AddPooledCollections(options =>
{
    options.EnableMemoryPooling = pooledOptions.EnableMemoryPooling;
    options.EnableZeroAllocation = pooledOptions.EnableZeroAllocation;
    options.EnableThreadLocalStorage = pooledOptions.EnableThreadLocalStorage;
    options.MemoryPoolSize = pooledOptions.MemoryPoolSize;
    options.ThreadLocalCacheSize = pooledOptions.ThreadLocalCacheSize;
    options.DefaultInitialCapacity = pooledOptions.DefaultInitialCapacity;
    options.MaximumPoolSize = pooledOptions.MaximumPoolSize;
    options.EnableStrictMode = pooledOptions.EnableStrictMode;
    options.EnableAutoClear = pooledOptions.EnableAutoClear;
    options.ClearInterval = pooledOptions.ClearInterval;
    options.EnablePerformanceMetrics = pooledOptions.EnablePerformanceMetrics;
});
```

## 配置选项

### 池化集合配置

```json
{
  "PooledCollections": {
    "EnableMemoryPooling": true,          // 启用内存池化
    "EnableZeroAllocation": true,         // 启用零分配操作
    "EnableThreadLocalStorage": true,     // 启用线程本地存储
    "MemoryPoolSize": 1024,               // 内存池大小
    "ThreadLocalCacheSize": 256,          // 线程本地缓存大小
    "DefaultInitialCapacity": 16,         // 默认初始容量
    "MaximumPoolSize": 1024,              // 最大池大小
    "EnableStrictMode": false,            // 启用严格模式
    "EnableAutoClear": true,              // 启用自动清理
    "ClearInterval": 5000,                // 清理间隔（毫秒）
    "EnablePerformanceMetrics": true      // 启用性能指标
  }
}
```

## 性能优化

1. **内存池化**: 启用内存池化以减少GC压力
2. **零分配操作**: 使用零分配方法避免内存分配
3. **线程本地存储**: 启用线程本地存储以提高访问速度
4. **批量处理**: 批量处理数据以提高效率
5. **适当的初始容量**: 根据实际需求设置适当的初始容量
6. **自动清理**: 启用自动清理以释放未使用的资源

## 故障排除

### 常见问题

1. **内存使用过高**
   - 检查MemoryPoolSize设置
   - 启用自动清理
   - 检查是否正确使用了using语句释放池化对象

2. **性能下降**
   - 启用线程本地存储
   - 调整MemoryPoolSize和ThreadLocalCacheSize
   - 检查是否有内存泄漏

3. **线程安全问题**
   - 确保在多线程环境中正确使用池化集合
   - 避免在不同线程间共享池化对象

4. **AOT编译错误**
   - 确保代码符合AOT编译要求
   - 避免使用反射和动态代码
   - 检查依赖项是否支持AOT

## 扩展开发

### 添加自定义池化集合

```csharp
// 实现自定义池化集合
public class CustomPooledCollection<T> : IPooledObject
{
    private List<T> _items;
    
    public List<T> Value => _items;
    
    public CustomPooledCollection(int initialCapacity = 16)
    {
        _items = new List<T>(initialCapacity);
    }
    
    public void Reset()
    {
        _items.Clear();
    }
    
    public void Dispose()
    {
        // 清理资源
    }
}

// 实现自定义工厂
public class CustomPooledCollectionFactory : ICustomPooledCollectionFactory
{
    private readonly ObjectPool<CustomPooledCollection<T>> _pool;
    
    public CustomPooledCollectionFactory(IOptions<PooledCollectionsOptions> options)
    {
        var poolOptions = new DefaultObjectPoolOptions
        {
            MaximumRetained = options.Value.MaximumPoolSize
        };
        
        _pool = new DefaultObjectPool<CustomPooledCollection<T>>(
            new CustomPooledCollectionPolicy<T>(),
            poolOptions
        );
    }
    
    public PooledObject<CustomPooledCollection<T>> Create<T>()
    {
        var collection = _pool.Get();
        return new PooledObject<CustomPooledCollection<T>>(collection, _pool);
    }
}

// 注册到DI
public static class CustomPooledCollectionExtensions
{
    public static IServiceCollection AddCustomPooledCollection(this IServiceCollection services)
    {
        services.AddSingleton<ICustomPooledCollectionFactory, CustomPooledCollectionFactory>();
        return services;
    }
}
```

## AOT编译配置

### 项目文件配置

```xml
<Project Sdk="Microsoft.NET.Sdk">
  
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <PublishAot>true</PublishAot>
    <TrimMode>partial</TrimMode>
    <ReadyToRun>true</ReadyToRun>
    <TieredCompilation>true</TieredCompilation>
    <Optimize>true</Optimize>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Options" Version="10.0.0" />
    <PackageReference Include="System.Collections.Immutable" Version="10.0.0" />
    <PackageReference Include="System.Memory" Version="10.0.0" />
    <PackageReference Include="System.Buffers" Version="10.0.0" />
  </ItemGroup>
  
</Project>
```

### 发布命令

```bash
# 发布为AOT编译的可执行文件
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishAot=true

# 发布为Linux平台的AOT编译可执行文件
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishAot=true

# 发布为macOS平台的AOT编译可执行文件
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishAot=true
```

## 监控和指标

### 性能指标

pooled系统提供了以下性能指标：

- **pooled.collections.created.count**: 创建的池化集合数量
- **pooled.collections.returned.count**: 返回的池化集合数量
- **pooled.collections.active.count**: 活跃的池化集合数量
- **pooled.collections.memory.usage**: 内存使用量
- **pooled.collections.operation.time.ms**: 操作时间（毫秒）
- **pooled.collections.pool.size**: 池大小

### 健康检查

```csharp
// 添加健康检查
builder.Services.AddHealthChecks()
    .AddCheck<PooledCollectionsHealthCheck>("pooled-collections");

// 配置健康检查端点
app.MapHealthChecks("/health");
```

## 最佳实践

1. **始终使用using语句**: 确保池化对象被正确释放
2. **避免长时间持有池化对象**: 尽快释放池化对象以提高重用率
3. **设置适当的初始容量**: 根据实际需求设置初始容量，避免频繁扩容
4. **启用线程本地存储**: 在多线程环境中启用线程本地存储以提高性能
5. **监控内存使用**: 定期监控内存使用情况，及时调整配置
6. **使用批量操作**: 对于大量数据，使用批量操作以提高效率
7. **避免在池化集合中存储大对象**: 大对象会增加内存使用和GC压力
8. **定期清理**: 启用自动清理以释放未使用的池化对象

## 依赖项

- **Microsoft.Extensions.DependencyInjection** (10.0.0): 依赖注入框架
- **Microsoft.Extensions.Logging** (10.0.0): 日志框架
- **Microsoft.Extensions.Options** (10.0.0): 配置选项框架
- **System.Collections.Immutable** (10.0.0): 不可变集合
- **System.Memory** (10.0.0): 内存管理
- **System.Buffers** (10.0.0): 缓冲区管理
- **System.Threading.Tasks.Dataflow** (10.0.0): 数据流处理
- **System.IO.Pipelines** (10.0.0): 高性能IO管道
