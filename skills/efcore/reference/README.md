# efcore - 参考文档

## 概述

efcore 是基于 .NET 10 AOT 架构的高性能数据库访问系统，专为 .NET 开发者设计，提供强大的 ORM 映射和数据库操作功能。

## 核心组件

### 1. EFCoreService（EFCore 服务）
- **位置**: scripts/efcore_aot.cs
- **功能**: EFCore 核心业务逻辑处理
- **特性**: 
  - 基于 .NET 10 AOT 编译，高性能
  - 数据库迁移和管理
  - 数据查询和操作
  - 完善的错误处理和日志记录

### 2. EFCoreAotContext（EFCore 上下文）
- **位置**: scripts/efcore_aot.cs
- **功能**: 数据库上下文，管理实体和数据库连接
- **特性**: 
  - 支持多种数据库提供程序
  - 实体关系映射
  - 配置灵活

### 3. EFCoreAotEngine（EFCore AOT 引擎）
- **位置**: scripts/efcore_aot.cs
- **功能**: 管理 EFCore 功能调用的引擎，提供命令行接口
- **特性**: 
  - 简化的 API 调用
  - 统一的错误处理
  - 支持命令行操作

## 核心接口

### IEFCoreService
EFCore 服务的核心接口，定义了所有 EFCore 操作方法：

| 方法名 | 描述 | 参数 | 返回值 |
|--------|------|------|--------|
| ExecuteCommandAsync | 执行 EFCore 命令 | commandType: EFCoreCommandType, parameters: Dictionary<string, string>? | Task<EFCoreCommandResult> |
| MigrateDatabaseAsync | 执行数据库迁移 | 无 | Task<EFCoreCommandResult> |
| CreateDatabaseAsync | 创建数据库 | 无 | Task<EFCoreCommandResult> |
| DropDatabaseAsync | 删除数据库 | 无 | Task<EFCoreCommandResult> |
| QueryDataAsync | 查询数据 | queryType: string, filter: string? | Task<EFCoreCommandResult> |
| InsertSampleDataAsync | 插入示例数据 | count: int | Task<EFCoreCommandResult> |
| GetVersionInfoAsync | 获取版本信息 | 无 | Task<EFCoreCommandResult> |

## 数据结构

### EFCoreCommandType（EFCore 命令类型）
```csharp
public enum EFCoreCommandType
{
    Migrate,           // 数据库迁移
    CreateDatabase,    // 创建数据库
    DropDatabase,      // 删除数据库
    Query,             // 查询数据
    Insert,            // 插入数据
    Update,            // 更新数据
    Delete,            // 删除数据
    VersionInfo        // 显示版本信息
}
```

### EFCoreOptions（EFCore 配置选项）
```csharp
public class EFCoreOptions
{
    public string ConnectionString { get; set; } = "InMemoryDatabase=efcore-aot-db";        
    public string ProviderType { get; set; } = "InMemory";        
    public bool EnableDetailedLogging { get; set; } = false;
    public bool EnablePerformanceMonitoring { get; set; } = true;
    public int QueryTimeoutSeconds { get; set; } = 30;
    public int BatchSize { get; set; } = 100;
}
```

### EFCoreCommandResult（EFCore 命令结果）
```csharp
public class EFCoreCommandResult
{
    public bool Success { get; set; }
    public EFCoreCommandType CommandType { get; set; }
    public List<string> Results { get; set; } = new List<string>();
    public long ExecutionTimeMs { get; set; }
    public string? ErrorMessage { get; set; }
    public int RowsAffected { get; set; }
}
```

## 配置选项

### EFCore 配置（efcore_aot.setting.json）

```json
{
  "EFCore": {
    "ConnectionString": "InMemoryDatabase=efcore-aot-db",
    "ProviderType": "InMemory",
    "EnableDetailedLogging": false,
    "EnablePerformanceMonitoring": true,
    "QueryTimeoutSeconds": 30,
    "BatchSize": 100
  }
}
```

## 运行配置（efcore_aot.run.json）

```json
{
  "$schema": "https://dot.net/v1/dotnet.run.schema.json",
  "framework": "net10.0",
  "options": {
    "PublishAot": true,
    "InvariantGlobalization": true,
    "EnableCompilationRelaxations": true,
    "PublishReadyToRun": true,
    "LangVersion": "preview",
    "Nullable": true,
    "ImplicitUsings": true
  },
  "dependencies": {
    "Microsoft.Extensions.DependencyInjection": "10.0.0",
    "Microsoft.Extensions.Hosting": "10.0.0",
    "Microsoft.Extensions.Logging": "10.0.0",
    "Microsoft.Extensions.Options": "10.0.0",
    "Microsoft.EntityFrameworkCore": "10.0.0",
    "Microsoft.EntityFrameworkCore.InMemory": "10.0.0",
    "Microsoft.EntityFrameworkCore.Relational": "10.0.0"
  }
}
```

## 性能优化

1. **启用 AOT 编译**：AOT 编译可以提供极致的启动速度和运行性能
2. **异步编程**：使用异步 API 可以提高系统的并发处理能力
3. **合理配置超时时间**：根据查询复杂度调整超时时间
4. **批量处理**：对于大量数据操作，使用批量处理提高效率
5. **连接池管理**：优化数据库连接池配置
6. **日志级别控制**：在生产环境中，将日志级别设置为 Information 或更高，减少日志开销
7. **索引优化**：为频繁查询的字段添加索引

## 故障排除

### 常见问题

1. **数据库连接失败**
   - 检查连接字符串配置
   - 验证数据库服务是否正常运行
   - 检查网络连接
   - 查看日志获取详细错误信息

2. **查询执行失败**
   - 检查实体关系配置
   - 验证 SQL 语法
   - 查看详细的错误信息
   - 检查数据库权限

3. **命令执行超时**
   - 调整超时时间配置
   - 优化查询，减少执行时间
   - 考虑将复杂查询拆分为多个简单查询

4. **服务异常**
   - 查看日志获取详细错误信息
   - 检查配置文件是否正确
   - 验证依赖项版本
   - 重启应用程序

5. **AOT 编译问题**
   - 确保所有依赖项都支持 AOT 编译
   - 检查是否使用了 AOT 不兼容的功能
   - 查看详细的编译错误信息

## 扩展开发

### 自定义实体和上下文

1. 定义自定义实体类
2. 扩展 EFCoreAotContext 类
3. 配置实体关系
4. 更新服务注册

```csharp
// 自定义实体
[Table("Products")]
public class Product
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// 扩展上下文
public class CustomEFCoreContext : EFCoreAOTContext
{
    public CustomEFCoreContext(IOptions<EFCoreOptions> options) : base(options)
    { }
    
    public DbSet<Product> Products { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // 配置自定义实体
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Name)
            .IsUnique();
    }
}
```

### 扩展服务功能

1. 实现 IEFCoreService 接口
2. 重写需要自定义的方法
3. 注册自定义服务

```csharp
builder.Services.AddSingleton<IEFCoreService, CustomEFCoreService>();
```

## AOT 编译注意事项

1. **依赖项**：确保所有依赖项都支持 AOT 编译
2. **反射**：避免在运行时使用反射，或使用 AOT 友好的反射方式
3. **动态类型**：谨慎使用 dynamic 类型，可能会影响 AOT 编译效果
4. **配置文件**：AOT 编译后，配置文件路径可能需要调整
5. **测试**：在 AOT 模式下进行充分测试，确保所有功能正常工作
6. **EFCore 兼容性**：确保使用的 EFCore 版本支持 AOT 编译

## 命令行工具

该技能提供了命令行工具，支持以下命令：

- `migrate`: 执行数据库迁移
- `create-db`: 创建数据库
- `drop-db`: 删除数据库
- `query <type>`: 查询数据（type: users, orders）
- `insert [count]`: 插入示例数据
- `version`: 显示版本信息
- `help, --help, -h`: 显示帮助信息

使用示例：
```bash
efcore_aot.exe migrate                # 执行数据库迁移
efcore_aot.exe create-db              # 创建数据库
efcore_aot.exe query users            # 查询用户数据
efcore_aot.exe insert 10              # 插入10条示例数据
efcore_aot.exe version                # 显示版本信息
```

