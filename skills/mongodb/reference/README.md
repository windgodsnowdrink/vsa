# MongoDB - 参考文档

## 概述

MongoDB 是基于 .NET 10 的高性能对象文档数据库系统，专为 .NET 开发者设计。它提供了一系列强大的数据库操作功能，包括文档增删改查、聚合查询、事务支持、索引管理等，帮助开发者快速构建高性能的数据存储应用。

## 核心组件

### 1. MongoDB 客户端
- **位置**: scripts/mongodb_integration.cs
- **功能**: MongoDB 客户端核心实现
- **特性**: 
  - 基于 MongoDB.Driver
  - 支持所有 MongoDB API
  - 高性能实现
  - 异步编程模型
  - 连接池管理

### 2. 依赖注入服务
- **位置**: scripts/mongodb_integration.cs
- **功能**: 提供依赖注入集成
- **特性**: 
  - 集成 Microsoft.Extensions.DependencyInjection
  - 支持配置绑定
  - 支持服务生命周期管理

### 3. 文档操作
- **位置**: scripts/mongodb_integration.cs
- **功能**: 管理 MongoDB 文档
- **特性**: 
  - 插入文档
  - 查询文档
  - 更新文档
  - 删除文档
  - 批量操作

### 4. 聚合查询
- **位置**: scripts/mongodb_integration.cs
- **功能**: 执行 MongoDB 聚合查询
- **特性**: 
  - 支持复杂的聚合管道
  - 支持分组、排序、过滤等操作
  - 支持地理空间查询
  - 支持全文搜索

### 5. 事务管理
- **位置**: scripts/mongodb_integration.cs
- **功能**: 管理 MongoDB 事务
- **特性**: 
  - 支持多文档事务
  - 支持事务回滚
  - 支持事务超时设置

### 6. 索引管理
- **位置**: scripts/mongodb_integration.cs
- **功能**: 管理 MongoDB 索引
- **特性**: 
  - 创建索引
  - 删除索引
  - 列出索引
  - 优化索引

## 使用示例

### 基本用法

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

### 高级配置

```csharp
// 配置 MongoDB 服务
builder.Services.AddMongoDB(options => {
    options.ConnectionString = "mongodb://localhost:27017";
    options.DatabaseName = "myDatabase";
    options.ConnectTimeout = TimeSpan.FromSeconds(30);
    options.ServerSelectionTimeout = TimeSpan.FromSeconds(30);
    options.MaxConnectionPoolSize = 100;
    options.MinConnectionPoolSize = 10;
    options.SocketTimeout = TimeSpan.FromMinutes(2);
    options.WaitQueueTimeout = TimeSpan.FromSeconds(2);
});

// 注册自定义存储库
builder.Services.AddSingleton<IMongoRepository, MongoRepository>();

// 获取配置的 MongoDB 客户端
var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();

// 获取数据库
var database = mongoClient.GetDatabase("myDatabase");

// 获取集合
var collection = database.GetCollection<User>("users");

// 执行复杂查询
var users = await collection.Find(x => x.Age > 25)
    .SortBy(x => x.Name)
    .Skip(0)
    .Limit(10)
    .ToListAsync();

foreach (var user in users)
{
    Console.WriteLine($"用户: {user.Name}, 年龄: {user.Age}");
}
```

### 聚合查询示例

```csharp
// 执行聚合查询
var pipeline = new BsonDocument[] {
    new BsonDocument("$match", new BsonDocument("age", new BsonDocument("$gt", 25))),
    new BsonDocument("$group", new BsonDocument {
        { "_id", "$age" },
        { "count", new BsonDocument("$sum", 1) }
    }),
    new BsonDocument("$sort", new BsonDocument("_id", 1))
};

var result = await collection.Aggregate<BsonDocument>(pipeline).ToListAsync();

foreach (var item in result)
{
    Console.WriteLine($"年龄: {item["_id"]}, 人数: {item["count"]}");
}
```

### 事务示例

```csharp
// 执行事务
using var session = await mongoClient.StartSessionAsync();
try
{
    session.StartTransaction();
    
    // 在事务中执行操作
    var userCollection = database.GetCollection<User>("users", session);
    var orderCollection = database.GetCollection<Order>("orders", session);
    
    // 创建用户
    var user = new User {
        Id = ObjectId.GenerateNewId(),
        Name = "李四",
        Email = "lisi@example.com"
    };
    await userCollection.InsertOneAsync(user);
    
    // 创建订单
    var order = new Order {
        Id = ObjectId.GenerateNewId(),
        UserId = user.Id,
        Amount = 100.0
    };
    await orderCollection.InsertOneAsync(order);
    
    // 提交事务
    await session.CommitTransactionAsync();
    Console.WriteLine("事务执行成功");
}
catch (Exception ex)
{
    // 回滚事务
    await session.AbortTransactionAsync();
    Console.WriteLine($"事务执行失败: {ex.Message}");
}
```

## 配置选项

### MongoDB 客户端配置

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017", // MongoDB 连接字符串
    "DatabaseName": "myDatabase",                  // 数据库名称
    "ConnectTimeout": "00:00:30",                 // 连接超时时间
    "ServerSelectionTimeout": "00:00:30",         // 服务器选择超时时间
    "MaxConnectionPoolSize": 100,                 // 最大连接池大小
    "MinConnectionPoolSize": 10,                  // 最小连接池大小
    "SocketTimeout": "00:02:00",                 // Socket 超时时间
    "WaitQueueTimeout": "00:00:02",              // 等待队列超时时间
    "EnableDetailedLogging": false                // 启用详细日志
  }
}
```

### 依赖注入配置

```csharp
// 基本配置
builder.Services.AddMongoDB(options => {
    options.ConnectionString = Configuration["MongoDB:ConnectionString"];
    options.DatabaseName = Configuration["MongoDB:DatabaseName"];
});

// 高级配置
builder.Services.AddMongoDB((sp, options) => {
    var configuration = sp.GetRequiredService<IConfiguration>();
    options.ConnectionString = configuration["MongoDB:ConnectionString"];
    options.DatabaseName = configuration["MongoDB:DatabaseName"];
    options.ConnectTimeout = TimeSpan.Parse(configuration["MongoDB:ConnectTimeout"]);
    options.ServerSelectionTimeout = TimeSpan.Parse(configuration["MongoDB:ServerSelectionTimeout"]);
    options.MaxConnectionPoolSize = int.Parse(configuration["MongoDB:MaxConnectionPoolSize"]);
    options.MinConnectionPoolSize = int.Parse(configuration["MongoDB:MinConnectionPoolSize"]);
    options.SocketTimeout = TimeSpan.Parse(configuration["MongoDB:SocketTimeout"]);
    options.WaitQueueTimeout = TimeSpan.Parse(configuration["MongoDB:WaitQueueTimeout"]);
});
```

## 性能优化

1. **连接池配置**：根据应用程序需求优化连接池大小
2. **索引优化**：根据查询模式创建合适的索引
3. **批量操作**：对于大量数据操作，使用批量操作减少网络往返次数
4. **投影查询**：只查询需要的字段，减少数据传输量
5. **缓存使用**：合理使用缓存，减少重复查询
6. **异步编程**：使用异步 API，避免阻塞主线程
7. **查询优化**：编写高效的查询，避免全表扫描
8. **分片策略**：对于大数据集，使用 MongoDB 分片

## 故障排除

### 常见问题

1. **连接失败**
   - 检查配置文件中的连接字符串
   - 验证网络连接是否正常
   - 检查 MongoDB 服务是否运行
   - 查看日志信息

2. **查询性能问题**
   - 检查是否有合适的索引
   - 优化查询条件
   - 考虑使用投影查询
   - 监控查询执行计划

3. **写入性能问题**
   - 考虑使用批量写入
   - 优化文档大小
   - 检查磁盘 I/O 性能
   - 考虑使用写入关注级别

4. **内存使用过高**
   - 检查连接池大小
   - 优化查询，避免加载过多数据
   - 考虑使用游标分页
   - 监控 MongoDB 服务器内存使用

5. **事务失败**
   - 检查事务超时设置
   - 验证操作是否在同一个会话中执行
   - 检查是否超出了事务大小限制
   - 查看 MongoDB 服务器日志

## 扩展开发

### 自定义存储库

```csharp
public class CustomMongoRepository : IMongoRepository
{
    private readonly IMongoCollection<User> _userCollection;
    private readonly ILogger<CustomMongoRepository> _logger;

    public CustomMongoRepository(IMongoDatabase database, ILogger<CustomMongoRepository> logger)
    {
        _userCollection = database.GetCollection<User>("users");
        _logger = logger;
    }

    public async Task InsertAsync(User user)
    {
        try
        {
            _logger.LogInformation("开始插入用户: {UserName}", user.Name);
            await _userCollection.InsertOneAsync(user);
            _logger.LogInformation("用户插入成功: {UserName}", user.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "用户插入失败: {UserName}", user.Name);
            throw;
        }
    }

    public async Task<User> FindByIdAsync(ObjectId id)
    {
        try
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, id);
            return await _userCollection.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询用户失败: {Id}", id);
            throw;
        }
    }

    // 实现其他方法...
}

// 注册自定义存储库
builder.Services.AddSingleton<IMongoRepository, CustomMongoRepository>();
```

### 批量操作扩展

```csharp
public class MongoBatchOperations
{
    private readonly IMongoClient _mongoClient;

    public MongoBatchOperations(IMongoClient mongoClient)
    {
        _mongoClient = mongoClient;
    }

    public async Task BatchInsertAsync<T>(string databaseName, string collectionName, IEnumerable<T> documents)
    {
        var database = _mongoClient.GetDatabase(databaseName);
        var collection = database.GetCollection<T>(collectionName);
        
        if (documents.Any())
        {
            await collection.InsertManyAsync(documents, new InsertManyOptions {
                IsOrdered = false // 无序插入，提高性能
            });
        }
    }

    public async Task BatchUpdateAsync<T>(string databaseName, string collectionName, IEnumerable<(FilterDefinition<T>, UpdateDefinition<T>)> updates)
    {
        var database = _mongoClient.GetDatabase(databaseName);
        var collection = database.GetCollection<T>(collectionName);
        
        var bulkOps = updates.Select(update => 
            new UpdateOneModel<T>(update.Item1, update.Item2)
        ).ToList();
        
        if (bulkOps.Any())
        {
            await collection.BulkWriteAsync(bulkOps, new BulkWriteOptions {
                IsOrdered = false // 无序执行，提高性能
            });
        }
    }
}
```

## AOT 编译优化

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

### AOT 编译最佳实践

1. **避免反射**：使用静态分析可检测的代码，减少反射使用
2. **避免动态类型**：使用强类型，提高编译时类型检查
3. **避免运行时代码生成**：使用预编译代码，减少运行时开销
4. **优化内存使用**：使用 Span<T> 和 Memory<T>，减少内存拷贝
5. **减少依赖**：最小化依赖项，减少编译时间和可执行文件大小
6. **使用值类型**：减少 GC 压力，提高内存访问效率
7. **避免大对象分配**：避免分配大于 85KB 的对象，减少大对象堆使用
8. **使用对象池**：对于频繁创建和销毁的对象，使用对象池提高性能

## 部署说明

### 部署步骤

1. **编译**：使用 .NET 10 SDK 编译代码
2. **打包**：打包为单文件可执行文件
3. **部署**：部署到目标环境
4. **配置**：配置环境变量和配置文件
5. **启动**：启动服务

### 环境要求

- .NET 10 运行时或更高版本
- MongoDB 4.0 或更高版本
- 足够的内存和磁盘空间
- 支持 AOT 编译的操作系统
- 网络连接（用于访问 MongoDB 服务）

### 配置文件

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "myDatabase",
    "ConnectTimeout": "00:00:30",
    "ServerSelectionTimeout": "00:00:30",
    "MaxConnectionPoolSize": 100,
    "MinConnectionPoolSize": 10
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 监控和维护

### 监控指标

1. **查询性能**：查询执行时间、扫描文档数
2. **写入性能**：写入操作时间、批量写入效率
3. **连接池**：连接池使用情况、等待队列长度
4. **内存使用**：应用程序内存使用、MongoDB 服务器内存使用
5. **CPU 使用**：应用程序 CPU 使用、MongoDB 服务器 CPU 使用
6. **网络流量**：MongoDB 网络流量、应用程序网络流量
7. **错误率**：操作错误率、异常数量

### 维护建议

1. **定期检查**：定期检查 MongoDB 服务状态
2. **优化配置**：根据实际使用情况优化配置
3. **更新依赖**：定期更新 MongoDB.Driver 等依赖项
4. **性能测试**：定期进行性能测试
5. **安全审计**：定期进行安全审计
6. **备份**：定期备份 MongoDB 数据
7. **监控**：建立完善的监控系统
8. **告警**：设置合理的告警阈值

## 总结

MongoDB 智能体技能提供了一套完整的对象文档数据库解决方案，包括文档增删改查、聚合查询、事务支持、索引管理等功能。它基于 .NET 10 构建，支持 AOT 编译，可以帮助 .NET 开发者更高效地构建高性能的数据存储应用，提高应用程序性能，确保数据的安全性和可靠性，实现更好的用户体验。
