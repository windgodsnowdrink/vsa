# MongoDB 智能体技能

## 技能概述

基于 .NET 10 的高性能 MongoDB 技能，采用 AOT 编译优化，为 .NET 开发者提供强大的 MongoDB 数据库操作功能，支持高性能、高并发的数据存储和查询。

## 快速开始指南

### 安装依赖

在主应用程序的 runfile 中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package MongoDB.Driver@2.28.0
#:package MongoDB.Bson@2.28.0
```

### 注册服务

在主应用程序中注册 MongoDB 服务：

```csharp
// 注册 MongoDB 服务
builder.Services.AddMongoDB("mongodb://localhost:27017", "myDatabase");

// 注册 MongoDB 存储服务
builder.Services.AddSingleton<IMongoRepository, MongoRepository>();
```

### 使用示例

```csharp
// 获取 MongoDB 存储服务
var repository = serviceProvider.GetRequiredService<IMongoRepository>();

// 创建文档
var user = new User {
    Id = ObjectId.GenerateNewId(),
    Name = "张三",
    Email = "zhangsan@example.com",
    Age = 30
};

// 插入文档
await repository.InsertAsync(user);
Console.WriteLine($"用户 {user.Name} 创建成功");

// 查询文档
var foundUser = await repository.FindByIdAsync(user.Id);
Console.WriteLine($"找到用户: {foundUser.Name}");

// 更新文档
foundUser.Age = 31;
await repository.UpdateAsync(foundUser);
Console.WriteLine($"用户 {foundUser.Name} 更新成功");

// 删除文档
await repository.DeleteAsync(user.Id);
Console.WriteLine($"用户 {user.Name} 删除成功");
```

## AOT 架构执行

### AOT 编译配置

```yaml
#:property PublishAot=true
#:property IncludeNativeLibrariesForSelfExtract=true
#:property EnableCppCodeGen=true
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
#:property RuntimeIdentifier=linux-x64
#:property RuntimeIdentifier=osx-x64
```

### 执行流程

1. **编译阶段**：使用 .NET 10 的 AOT 编译功能将代码编译为本地机器码，提高运行性能
2. **打包阶段**：将编译后的代码打包为单文件可执行文件，包含所有依赖项
3. **部署阶段**：将打包后的可执行文件部署到目标环境，无需安装 .NET 运行时
4. **运行阶段**：执行单文件可执行文件，处理 MongoDB 数据库操作，享受 AOT 编译带来的性能优势

### AOT 架构优势

1. **启动速度快**：AOT 编译消除了 JIT 编译开销，启动时间显著缩短
2. **内存占用低**：减少了运行时编译所需的内存，降低了内存使用
3. **执行效率高**：本地机器码执行效率更高，特别是对于计算密集型操作
4. **部署简单**：单文件可执行文件，无需依赖外部运行时
5. **安全性强**：减少了运行时攻击面，提高了应用程序安全性

## 导航地图

```
mongodb/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── *.cs                    # MongoDB 核心实现
    ├── *.run.json              # 运行配置
    └── *.setting.json          # 设置文件
```

## 主要功能

1. **高性能 MongoDB 数据库操作**：基于 MongoDB.Driver 的高性能实现，支持 AOT 编译优化
2. **文档增删改查**：支持 MongoDB 文档的创建、读取、更新和删除操作
3. **聚合查询和分析**：支持复杂的聚合管道查询和数据分析
4. **事务操作**：支持 MongoDB 事务，确保数据一致性
5. **索引管理**：支持创建和管理 MongoDB 索引，提高查询性能
6. **连接池管理**：优化 MongoDB 连接池配置，提高并发性能
7. **批量操作**：支持批量插入、更新和删除操作，提高处理效率
8. **地理位置查询**：支持 MongoDB 地理位置索引和查询
9. **全文搜索**：支持 MongoDB 全文搜索功能
10. **变更流**：支持 MongoDB 变更流，实现实时数据变更监听

## 扩展说明

本技能提供了完整的 MongoDB 解决方案，您可以根据需要进行扩展：

1. **自定义存储库**：实现自定义的 MongoDB 存储库，添加特定业务逻辑
2. **扩展功能**：添加新的 MongoDB 功能，如加密、压缩等
3. **与其他系统集成**：与缓存、消息队列等系统集成，实现数据流转
4. **性能优化**：针对特定场景优化性能，如大文档处理、高频查询等

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务，提高代码可测试性和可维护性
2. **异步编程**：优先使用异步 API 避免阻塞，提高应用程序响应速度
3. **错误处理**：正确处理异常情况，实现重试机制和故障转移
4. **日志记录**：添加适当的日志记录，便于问题排查和性能分析
5. **性能监控**：监控关键性能指标，如查询延迟、写入吞吐量等
6. **连接池配置**：根据应用程序需求优化连接池配置
7. **索引优化**：根据查询模式创建合适的索引，提高查询性能
8. **批量操作**：对于大量数据操作，使用批量操作减少网络往返次数
9. **事务使用**：合理使用事务，避免过度使用影响性能
10. **数据建模**：根据应用程序需求设计合理的数据模型

## 性能优化建议

1. **内存分配优化**：减少不必要的内存分配，使用 Span<T> 和 Memory<T> 进行零拷贝操作
2. **GC 压力优化**：减少 GC 触发次数，使用对象池复用频繁创建的对象
3. **并发优化**：使用多线程并行处理，提高处理效率
4. **批处理优化**：批量处理请求，减少网络往返次数
5. **缓存使用**：合理使用缓存，减少重复查询
6. **网络传输优化**：优化网络传输中的数据处理，使用压缩等技术
7. **查询优化**：编写高效的查询，避免全表扫描
8. **索引使用**：合理使用索引，避免过度索引

## AOT 编译最佳实践

1. **避免反射**：使用静态分析可检测的代码，减少反射使用
2. **避免动态类型**：使用强类型，提高编译时类型检查
3. **避免运行时代码生成**：使用预编译代码，减少运行时开销
4. **优化内存使用**：使用 Span<T> 和 Memory<T>，减少内存拷贝
5. **减少依赖**：最小化依赖项，减少编译时间和可执行文件大小
6. **使用值类型**：减少 GC 压力，提高内存访问效率
7. **避免大对象分配**：避免分配大于 85KB 的对象，减少大对象堆使用
8. **使用对象池**：对于频繁创建和销毁的对象，使用对象池提高性能
