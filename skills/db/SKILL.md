# db Agent Skill - 数据库技能

## 技能概述

基于 .NET 10 构建的高性能数据库技能，为 .NET 开发者提供强大的数据库功能支持。该技能支持 AOT（预编译）编译，提供极致的性能表现和启动速度。

## 快速入门指南

### 安装依赖

在主应用程序的运行文件中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.Data.Sqlite@8.0.0
#:package Dapper@2.1.35
#:package MySqlConnector@2.3.5
#:package Npgsql@8.0.3
```

### 注册服务

在主应用程序中注册数据库服务：

```csharp
// 配置数据库选项
builder.Configuration.AddJsonFile("db_aot.setting.json", optional: true);
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));

// 注册数据库服务
builder.Services.AddDatabase();
```

### 使用示例

```csharp
// 获取数据库引擎实例
var engine = serviceProvider.GetRequiredService<DatabaseAotEngine>();

// 执行查询操作
var result = await engine.ExecuteQueryAsync<dynamic>("SELECT * FROM TestTable");
if (result.Success && result.ResultData != null)
{
    Console.WriteLine($"查询到 {result.ResultData.Count} 条记录");
    foreach (var record in result.ResultData)
    {
        Console.WriteLine($"- Id: {record.Id}, Name: {record.Name}");
    }
}
```

## 导航地图

```
db/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── db_aot.cs              # 数据库核心实现（AOT）
    ├── db_aot.run.json        # 运行配置
    └── db_aot.setting.json    # 应用设置
```

## 主要功能

1. **多数据库支持**：支持 SQLite、MySQL、PostgreSQL 等多种数据库
2. **高性能 AOT 编译**：基于 .NET 10 AOT 技术，提供极致性能和启动速度
3. **全面的数据库操作**：支持查询、插入、更新、删除、事务、存储过程等
4. **批量操作支持**：高效的批量插入和更新功能
5. **连接池管理**：优化的连接池实现，提高并发性能
6. **详细的状态监控**：实时监控数据库服务状态和性能指标
7. **灵活的配置选项**：支持通过配置文件自定义各种数据库参数
8. **完善的错误处理**：详细的错误信息和日志记录

## 扩展说明

该技能提供了完整的数据库解决方案，您可以根据需要进行扩展：

1. **自定义数据库类型**：添加对其他数据库类型的支持
2. **扩展数据库操作**：添加自定义的数据库操作方法
3. **性能优化**：针对特定场景优化数据库操作性能
4. **集成其他系统**：与其他系统和框架集成

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务，提高代码可测试性和可维护性
2. **异步编程**：优先使用异步 API，避免阻塞主线程
3. **适当的超时设置**：根据实际情况调整连接超时和命令超时
4. **连接池配置**：根据系统负载调整连接池大小
5. **日志记录**：添加适当的日志记录，便于调试和监控
6. **事务管理**：合理使用事务，确保数据一致性
7. **批量操作**：对于大量数据操作，使用批量操作提高性能
8. **错误处理**：妥善处理异常情况，提供友好的错误信息

## AOT 编译说明

该技能支持 .NET 10 AOT 编译，通过预编译将应用程序编译为本地机器代码，提供以下优势：

- **极致的启动速度**：无需 JIT 编译，直接运行本地代码
- **减少内存占用**：更小的运行时占用
- **提高安全性**：减少可攻击面，提高应用程序安全性
- **跨平台支持**：支持多种操作系统和架构

## 命令行工具

该技能提供了命令行工具，支持以下命令：

- `status`：查看数据库服务状态
- `reset`：重置数据库服务状态
- `demo`：运行数据库演示程序

使用示例：
```
db_aot.exe status
db_aot.exe reset
db_aot.exe demo
```