# pooled Agent Skill - 内存池化集合技能

## 技能概述

基于.NET 10的高性能内存池化集合技能，为.NET开发者提供强大的内存池化功能，旨在减少内存分配和垃圾回收压力，提高应用程序性能。

## 快速开始指南

### 安装依赖项

在主应用程序的runfile中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Collections.Immutable@10.0.0
#:package System.Memory@10.0.0
#:package System.Buffers@10.0.0
#:package System.Threading.Tasks.Dataflow@10.0.0
#:package System.IO.Pipelines@10.0.0
```

### 注册服务

在主应用程序中注册内存池化服务：

```csharp
// 注册内存池化服务
builder.Services.AddPooledCollections();
builder.Services.Configure<PooledCollectionsOptions>(options =>
{
    options.EnableMemoryPooling = true;
    options.EnableZeroAllocation = true;
    options.EnableThreadLocalStorage = true;
    options.MemoryPoolSize = 1024;
    options.ThreadLocalCacheSize = 256;
});
```

### 使用示例

```csharp
// 获取内存池化服务
var pooledListFactory = serviceProvider.GetRequiredService<IPooledListFactory>();
var pooledDictionaryFactory = serviceProvider.GetRequiredService<IPooledDictionaryFactory>();

// 使用内存池化List
using (var pooledList = pooledListFactory.Create<string>())
{
    // 添加元素
    pooledList.Add("Item 1");
    pooledList.Add("Item 2");
    pooledList.Add("Item 3");
    
    // 使用List
    foreach (var item in pooledList)
    {
        Console.WriteLine($"Item: {item}");
    }
    
    // 释放回池（using块结束时自动释放）
}

// 使用内存池化Dictionary
using (var pooledDictionary = pooledDictionaryFactory.Create<string, int>())
{
    // 添加键值对
    pooledDictionary["Key1"] = 1;
    pooledDictionary["Key2"] = 2;
    
    // 使用Dictionary
    foreach (var kvp in pooledDictionary)
    {
        Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
    }
    
    // 释放回池（using块结束时自动释放）
}
```

## 导航地图

```
pooled/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── collections_pooled_integration.cs     # 内存池化核心实现
    ├── collections_pooled_integration.run.json  # 运行配置
    └── collections_pooled_integration.setting.json  # 设置文件
```

## 主要特性

1. **内存池化集合类型**：提供List、Dictionary、HashSet等常用集合的内存池化实现
2. **零分配操作**：支持零分配的集合操作，减少GC压力
3. **线程安全实现**：提供线程安全的内存池化集合
4. **高性能并发集合**：优化的并发集合实现，支持高并行度
5. **内存使用监控**：内置内存使用监控和分析功能
6. **自动内存回收**：智能内存回收和池化管理
7. **高性能设计**：优化的性能实现，减少内存开销
8. **易用API**：简单直观的API设计，与标准集合API兼容
9. **可扩展架构**：支持自定义扩展和实现
10. **AOT编译支持**：完全支持AOT编译和裁剪

## 扩展说明

此技能提供了完整的内存池化解决方案，您可以根据需要进行扩展：

1. **自定义实现**：实现IPooledCollection接口创建自定义内存池化集合
2. **扩展特性**：添加新的内存池化功能和集合类型
3. **与其他系统集成**：与其他系统和框架集成
4. **性能优化**：针对特定场景优化性能
5. **自定义内存池**：实现自定义内存池策略

## 最佳实践

1. **依赖注入**：使用依赖注入管理内存池化服务
2. **异步编程**：优先使用异步API避免阻塞
3. **使用using块**：使用using块确保内存池化对象正确释放回池
4. **错误处理**：正确处理异常情况，确保内存池化对象不会泄漏
5. **日志记录**：添加适当的日志记录，便于排查问题
6. **性能监控**：监控关键性能指标，如内存使用和GC频率
7. **批量操作**：对于大量数据，使用批量操作减少内存分配
8. **合理设置池大小**：根据应用程序需求合理设置内存池大小

## AOT编译配置

为了支持AOT编译，请确保在项目文件中添加以下配置：

```xml
<PropertyGroup>
    <PublishAot>true</PublishAot>
    <TrimMode>partial</TrimMode>
    <ReadyToRun>true</ReadyToRun>
    <TieredCompilation>true</TieredCompilation>
    <Optimize>true</Optimize>
</PropertyGroup>
```

## 部署选项

此技能支持多种部署选项：

1. **标准部署**：作为常规.NET应用程序部署
2. **AOT编译部署**：编译为本地代码，提高性能和启动速度
3. **容器化部署**：支持Docker容器部署
4. **自包含应用部署**：包含运行时，无需安装.NET

## 性能优化建议

1. **使用适当的集合类型**：根据具体场景选择合适的内存池化集合类型
2. **避免频繁创建和销毁**：使用内存池化集合避免频繁的内存分配和释放
3. **合理设置初始容量**：根据预期数据量设置合理的初始容量
4. **使用批量操作**：对于大量数据，使用批量操作减少内存分配
5. **监控内存使用**：定期监控内存使用情况，调整内存池配置
6. **使用线程本地存储**：对于高频访问的集合，使用线程本地存储提高性能

## 故障排除

### 常见问题

1. **内存泄漏**：确保所有内存池化对象都正确释放回池
2. **性能下降**：检查内存池大小是否合适，是否有过多的内存分配
3. **线程安全问题**：对于多线程场景，使用线程安全的内存池化集合
4. **AOT编译错误**：确保所有依赖项都支持AOT编译，避免使用反射和动态代码

### 解决方案

1. **使用using块**：始终使用using块管理内存池化对象的生命周期
2. **调整内存池配置**：根据应用程序需求调整内存池大小和其他配置
3. **使用线程安全集合**：对于多线程场景，使用线程安全的内存池化集合
4. **添加内存使用监控**：添加内存使用监控，及时发现和解决内存问题
5. **测试AOT编译**：在部署前测试AOT编译，确保所有功能正常工作
