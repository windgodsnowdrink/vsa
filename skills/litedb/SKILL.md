# LiteDB 技能

## 概述

LiteDB 是一个轻量级的 .NET NoSQL 嵌入式数据库，专为简单应用程序设计。本技能提供了一个 AOT 编译的 LiteDB 工具，支持各种数据库操作，包括集合管理、文档 CRUD 操作、备份和压缩等功能。

## 功能特性

- **轻量级嵌入式数据库**：无需单独的数据库服务器，直接存储为单个文件
- **NoSQL 文档存储**：使用 JSON 格式存储数据，灵活的数据结构
- **完整的 CRUD 操作**：支持文档的创建、读取、更新和删除
- **集合管理**：创建和删除集合，列出所有集合
- **数据库维护**：备份和压缩数据库
- **高性能**：AOT 编译，单文件部署，启动快速
- **跨平台支持**：支持 Windows、Linux 和 macOS
- **依赖注入**：使用 Microsoft.Extensions.DependencyInjection 进行依赖管理
- **内存缓存**：提高频繁操作的性能
- **详细的日志记录**：使用 Microsoft.Extensions.Logging 提供日志功能

## 技术架构

- **框架**：.NET 10.0
- **编译方式**：AOT (Ahead-of-Time) 编译
- **部署方式**：单文件、自包含部署
- **依赖库**：
  - LiteDB 5.0.16
  - System.CommandLine 2.0.0
  - Microsoft.Extensions.DependencyInjection 10.0.0
  - Microsoft.Extensions.Caching.Memory 10.0.0
  - Microsoft.Extensions.Logging 10.0.0
  - Microsoft.Extensions.Options 10.0.0

## 命令列表

| 命令 | 描述 | 参数 |
|------|------|------|
| `create-collection` | 创建集合 | `--database` (数据库路径), `--collection` (集合名称) |
| `insert` | 插入文档 | `--database` (数据库路径), `--collection` (集合名称), `--document` (文档内容) |
| `query` | 查询文档 | `--database` (数据库路径), `--collection` (集合名称), `--query` (查询条件，可选) |
| `update` | 更新文档 | `--database` (数据库路径), `--collection` (集合名称), `--id` (文档ID), `--document` (文档内容) |
| `delete` | 删除文档 | `--database` (数据库路径), `--collection` (集合名称), `--id` (文档ID) |
| `drop-collection` | 删除集合 | `--database` (数据库路径), `--collection` (集合名称) |
| `list-collections` | 列出所有集合 | `--database` (数据库路径) |
| `backup` | 备份数据库 | `--database` (数据库路径), `--backup` (备份路径) |
| `compact` | 压缩数据库 | `--database` (数据库路径) |
| `info` | 获取数据库信息 | `--database` (数据库路径) |

## 使用示例

### 创建集合
```bash
litedb_aot.exe create-collection --database "data.db" --collection "users"
```

### 插入文档
```bash
litedb_aot.exe insert --database "data.db" --collection "users" --document '{"name": "张三", "age": 30, "email": "zhangsan@example.com"}'
```

### 查询文档
```bash
# 查询所有文档
litedb_aot.exe query --database "data.db" --collection "users"

# 使用查询条件
litedb_aot.exe query --database "data.db" --collection "users" --query "$.age > 25"
```

### 更新文档
```bash
litedb_aot.exe update --database "data.db" --collection "users" --id "5f8d0d5a-1234-4567-89ab-cdef01234567" --document '{"name": "张三", "age": 31, "email": "zhangsan@example.com"}'
```

### 删除文档
```bash
litedb_aot.exe delete --database "data.db" --collection "users" --id "5f8d0d5a-1234-4567-89ab-cdef01234567"
```

### 列出所有集合
```bash
litedb_aot.exe list-collections --database "data.db"
```

### 备份数据库
```bash
litedb_aot.exe backup --database "data.db" --backup "backup/data_backup.db"
```

### 压缩数据库
```bash
litedb_aot.exe compact --database "data.db"
```

### 获取数据库信息
```bash
litedb_aot.exe info --database "data.db"
```

## 配置选项

### 环境变量

| 环境变量 | 默认值 | 描述 |
|---------|--------|------|
| `LITEDB_LOG_LEVEL` | `Information` | 日志级别 |
| `LITEDB_CACHE_SIZE` | `1024` | 缓存大小 (KB) |
| `LITEDB_TIMEOUT` | `60000` | 超时时间 (毫秒) |
| `LITEDB_JOURNAL` | `true` | 是否启用日志 |

### 数据库连接选项

- **模式**：独占模式
- **日志**：启用
- **缓存大小**：1024 KB
- **超时**：1 分钟

## 性能优化

- **内存缓存**：使用 Microsoft.Extensions.Caching.Memory 缓存数据库实例
- **异步操作**：所有操作都支持异步执行
- **连接池**：维护数据库连接池，减少连接开销
- **AOT 编译**：提前编译为本地代码，提高启动速度和运行性能
- **单文件部署**：减少文件数量，提高部署效率

## 限制和注意事项

- LiteDB 适用于中小型应用程序，不建议用于大型生产环境
- 单个数据库文件大小建议不超过 1GB
- 在高并发场景下可能需要额外的同步措施
- 备份操作会创建数据库的完整副本，可能需要较多磁盘空间

## 故障排除

### 常见问题

1. **数据库文件被占用**
   - 确保没有其他进程正在使用该数据库文件
   - 检查是否有未关闭的连接

2. **文档插入失败**
   - 检查文档格式是否正确（有效的 JSON）
   - 确保集合存在

3. **查询返回空结果**
   - 检查查询条件是否正确
   - 确认集合中存在符合条件的文档

4. **备份失败**
   - 确保备份目录存在且可写
   - 检查磁盘空间是否充足

### 日志查看

运行工具时，日志会输出到控制台，可根据日志级别调整输出详细程度。

## 总结

LiteDB 技能提供了一个轻量级、高性能的嵌入式数据库解决方案，适用于各种小型应用程序和原型开发。通过 AOT 编译和单文件部署，它具有快速启动、低资源消耗的特点，同时提供了完整的数据库操作功能。

---

**版本**：1.0.0
**发布日期**：2026-01-22
**作者**：NET 专家