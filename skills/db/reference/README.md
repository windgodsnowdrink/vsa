# db - 参考文档

## 概述

db 是基于 .NET 10 构建的高性能数据库技能，专为 .NET 开发者设计，提供全面的数据库功能支持。该技能支持 AOT（预编译）编译，能够提供极致的性能表现和启动速度。

## 核心组件

### 1. DatabaseService（数据库服务）
- **位置**: scripts/db_aot.cs
- **功能**: 数据库核心业务逻辑处理
- **特性**: 
  - 支持多种数据库类型（SQLite、MySQL、PostgreSQL）
  - 全面的数据库操作支持
  - 高性能 AOT 编译
  - 优化的连接池管理
  - 完善的错误处理
  - 详细的日志记录

### 2. DatabaseAotEngine（数据库 AOT 引擎）
- **位置**: scripts/db_aot.cs
- **功能**: 管理数据库功能调用的引擎
- **特性**: 
  - 简化的 API 调用
  - 统一的错误处理
  - 性能监控
  - 状态管理

## 核心接口

### IDatabaseService
数据库服务的核心接口，定义了所有数据库操作方法：

| 方法名 | 描述 | 参数 | 返回值 |
|--------|------|------|--------|
| ExecuteScalarAsync<T> | 执行查询并返回单个结果 | sql: string, parameters: object? | DatabaseResult<T> |
| ExecuteQueryAsync<T> | 执行查询并返回结果集 | sql: string, parameters: object? | DatabaseResult<List<T>> |
| ExecuteNonQueryAsync | 执行非查询操作 | sql: string, parameters: object? | DatabaseResult<int> |
| ExecuteStoredProcedureAsync<T> | 执行存储过程 | procedureName: string, parameters: object? | DatabaseResult<List<T>> |
| ExecuteTransactionAsync | 执行事务中的多个操作 | actions: List<Func<IDbConnection, Task<bool>>> | DatabaseResult |
| BulkInsertAsync<T> | 批量插入数据 | tableName: string, data: List<T> | DatabaseResult<int> |
| GetConnectionAsync | 获取数据库连接 | 无 | Task<DbConnection> |
| GetStatusAsync | 获取数据库状态 | 无 | Task<DatabaseStatus> |
| ResetStatusAsync | 重置数据库状态 | 无 | Task<bool> |

## 数据结构

### DatabaseOptions（数据库配置选项）
```csharp
public class DatabaseOptions
{
    public DatabaseType DatabaseType { get; set; } = DatabaseType.Sqlite;
    public string ConnectionString { get; set; } = "Data Source=app.db";
    public bool EnableConnectionPooling { get; set; } = true;
    public int MaxPoolSize { get; set; } = 100;
    public int ConnectionTimeout { get; set; } = 30;
    public int CommandTimeout { get; set; } = 30;
    public bool EnableDetailedLogging { get; set; } = false;
    public bool EnablePerformanceMonitoring { get; set; } = true;
    public int BatchSize { get; set; } = 1000;
    public bool EnableTransactions { get; set; } = true;
}
```

### DatabaseResult（数据库操作结果）
```csharp
public class DatabaseResult
{
    public bool Success { get; set; }
    public object? ResultData { get; set; }
    public string? ErrorMessage { get; set; }
    public long ExecutionTimeMs { get; set; }
    public string? OperationType { get; set; }
    public int RowsAffected { get; set; }
}
```

### DatabaseStatus（数据库状态）
```csharp
public class DatabaseStatus
{
    public bool IsRunning { get; set; }
    public long ProcessedRequests { get; set; }
    public long SuccessfulRequests { get; set; }
    public long FailedRequests { get; set; }
    public double AverageExecutionTimeMs { get; set; }
    public DateTime StartTime { get; set; }
    public DatabaseType DatabaseType { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public int ActiveConnections { get; set; }
    public int MaxPoolSize { get; set; }
}
```

## 配置选项

### 数据库配置（db_aot.setting.json）

```json
{
  "Database": {
    "DatabaseType": "Sqlite",              // 数据库类型：Sqlite、MySql、PostgreSql
    "ConnectionString": "Data Source=app.db", // 连接字符串
    "EnableConnectionPooling": true,       // 是否启用连接池
    "MaxPoolSize": 100,                    // 最大连接池大小
    "ConnectionTimeout": 30,               // 连接超时时间（秒）
    "CommandTimeout": 30,                  // 命令超时时间（秒）
    "EnableDetailedLogging": false,        // 是否启用详细日志
    "EnablePerformanceMonitoring": true,   // 是否启用性能监控
    "BatchSize": 1000,                     // 批量操作大小
    "EnableTransactions": true             // 是否启用事务支持
  }
}
```

## 运行配置（db_aot.run.json）

```json
{
  "$schema": "https://dot.net/v1/dotnet.run.schema.json",
  "framework": "net10.0",
  "options": {
    "PublishAot": true,                   // 启用 AOT 编译
    "InvariantGlobalization": true,       // 启用不变全球化
    "EnableCompilationRelaxations": true, // 启用编译松弛
    "PublishReadyToRun": true,            // 启用 ReadyToRun
    "LangVersion": "preview",            // 语言版本
    "Nullable": true,                     // 启用可空引用类型
    "ImplicitUsings": true                // 启用隐式 using
  },
  "dependencies": {
    "Microsoft.Extensions.DependencyInjection": "10.0.0",
    "Microsoft.Extensions.Hosting": "10.0.0",
    "Microsoft.Extensions.Logging": "10.0.0",
    "Microsoft.Extensions.Options": "10.0.0",
    "Microsoft.Data.Sqlite": "8.0.0",
    "Dapper": "2.1.35",
    "MySqlConnector": "2.3.5",
    "Npgsql": "8.0.3"
  }
}
```

## 性能优化

1. **AOT 编译**: 启用 PublishAot=true，获得极致的启动速度和运行性能
2. **连接池管理**: 启用连接池，合理设置最大连接数
3. **异步编程**: 使用 async/await 模式，避免阻塞主线程
4. **批量操作**: 对于大量数据操作，使用 BulkInsertAsync 方法
5. **适当的超时设置**: 根据实际情况调整连接超时和命令超时
6. **日志级别**: 在生产环境中，将日志级别设置为 Information 或更高，减少日志开销

## 故障排除

### 常见问题

1. **连接失败**
   - 检查连接字符串是否正确
   - 验证数据库服务是否正在运行
   - 检查网络连接（对于远程数据库）
   - 查看日志信息获取详细错误

2. **性能问题**
   - 启用连接池
   - 优化查询语句
   - 使用批量操作
   - 增加连接池大小（根据系统负载）
   - 启用 AOT 编译

3. **事务失败**
   - 检查事务中的每个操作是否正确
   - 确保数据库支持事务
   - 查看日志获取详细错误信息

## 扩展开发

### 添加自定义数据库类型支持

1. 在 DatabaseType 枚举中添加新的数据库类型
2. 在 CreateConnection 方法中添加新的数据库连接创建逻辑
3. 添加相应的数据库驱动依赖

### 扩展数据库操作方法

1. 在 IDatabaseService 接口中定义新的方法
2. 在 DatabaseService 类中实现该方法
3. 在 DatabaseAotEngine 类中添加对应的调用方法

## AOT 编译注意事项

1. **反射使用**: 避免在运行时使用反射，或使用 AOT 友好的反射方式
2. **动态类型**: 谨慎使用 dynamic 类型，可能会影响 AOT 编译效果
3. **依赖项**: 确保所有依赖项都支持 AOT 编译
4. **配置文件**: AOT 编译后，配置文件路径可能需要调整
5. **测试**: 在 AOT 模式下进行充分测试，确保所有功能正常工作

## 命令行工具

该技能提供了命令行工具，支持以下命令：

- `status`: 查看数据库服务状态
- `reset`: 重置数据库服务状态
- `demo`: 运行数据库演示程序

使用示例：
```
db_aot.exe status
db_aot.exe reset
db_aot.exe demo
```

## 版本历史

| 版本 | 日期 | 描述 |
|------|------|------|
| 1.0.0 | 2026-01-03 | 初始版本，基于 .NET 10 AOT 架构 |

## 许可证

MIT License