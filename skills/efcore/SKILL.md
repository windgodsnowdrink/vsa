# efcore Agent Skill - efcore 技能

## 技能概述

基于 .NET 10 构建的高性能 efcore 技能，为 .NET 开发者提供强大的数据库访问和 ORM 映射功能支持。该技能采用 AOT（预编译）技术，提供极致的性能表现和启动速度，适用于各种数据库访问场景。

## 快速入门指南

### 安装依赖

在主应用程序的运行文件中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.EntityFrameworkCore@10.0.0
#:package Microsoft.EntityFrameworkCore.InMemory@10.0.0
#:package Microsoft.EntityFrameworkCore.Relational@10.0.0
```

### 注册服务

在主应用程序中注册 efcore 服务：

```csharp
// 配置 efcore 选项
builder.Configuration.AddJsonFile("efcore_aot.setting.json", optional: true);
builder.Services.Configure<EFCore.AOT.EFCoreOptions>(builder.Configuration.GetSection("EFCore"));

// 注册 efcore 服务
builder.Services.AddEFCore();
```

### 使用示例

```csharp
// 获取 efcore 服务实例
var efcoreService = serviceProvider.GetRequiredService<EFCore.AOT.IEFCoreService>();

// 执行数据库迁移
var migrateResult = await efcoreService.MigrateDatabaseAsync();
Console.WriteLine($"Migration result: {migrateResult.Success}");

// 插入示例数据
var insertResult = await efcoreService.InsertSampleDataAsync(2);
Console.WriteLine($"Insert result: {insertResult.Success}, Rows: {insertResult.RowsAffected}");

// 查询数据
var queryResult = await efcoreService.QueryDataAsync("users");
Console.WriteLine($"Query result: {queryResult.Success}, Users: {queryResult.RowsAffected}");
```

## 导航地图

```
efcore/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── efcore_aot.cs          # efcore 核心实现（AOT）
    ├── efcore_aot.run.json    # 运行配置
    ├── efcore_aot.setting.json # 应用设置
    └── other integration files # 其他集成脚本
```

## 主要功能

1. **数据库迁移**：执行数据库架构迁移
2. **数据库管理**：创建和删除数据库
3. **数据查询**：支持多种查询类型
4. **数据操作**：插入、更新和删除数据
5. **版本信息**：获取 efcore 引擎版本信息
6. **高性能设计**：基于 AOT 编译的优化性能实现
7. **易用 API**：简单直观的 API 设计，易于集成
8. **多数据库支持**：支持多种数据库提供程序

## 扩展说明

该技能提供了完整的 efcore 数据库访问解决方案，您可以根据需要进行扩展：

1. **自定义实体**：添加自定义实体类和关系映射
2. **扩展命令**：添加新的数据操作命令
3. **支持新数据库**：添加对新数据库提供程序的支持
4. **性能优化**：针对特定场景优化查询和数据操作

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务，提高代码可测试性和可维护性
2. **异步编程**：优先使用异步 API，避免阻塞主线程
3. **合理配置**：根据实际需求调整数据库连接和查询超时设置
4. **错误处理**：妥善处理数据库操作中的各种异常情况
5. **日志记录**：适当添加日志记录，便于调试和监控
6. **性能监控**：启用性能监控，实时了解数据库操作性能
7. **批量操作**：对于大量数据操作，使用批量处理提高效率

## AOT 编译说明

该技能支持 .NET 10 AOT 编译，通过预编译将应用程序编译为本地机器代码，提供以下优势：

- **极致的启动速度**：无需 JIT 编译，直接运行本地代码
- **减少内存占用**：更小的运行时占用
- **提高安全性**：减少可攻击面，提高应用程序安全性
- **跨平台支持**：支持多种操作系统和架构

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
```
efcore_aot.exe migrate          执行数据库迁移
efcore_aot.exe create-db        创建数据库
efcore_aot.exe query users      查询用户数据
efcore_aot.exe insert 5         插入5条示例数据
efcore_aot.exe version          显示版本信息
efcore_aot.exe help             显示帮助信息
```
