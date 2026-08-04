# dtm - 参考文档

## 概述

dtm 是基于 .NET 10 AOT 架构的高性能分布式事务管理系统，专为 .NET 开发者设计。

## 核心组件

### 1. DtmService（DTM 服务）
- **位置**: scripts/dtm_aot.cs
- **功能**: DTM 核心业务逻辑处理
- **特性**: 
  - 基于 .NET 10 AOT 编译，高性能
  - 分布式事务执行和管理
  - 事务状态查询和维护
  - 完善的错误处理和日志记录
  - 事务缓存机制

### 2. DtmAotEngine（DTM AOT 引擎）
- **位置**: scripts/dtm_aot.cs
- **功能**: 管理 DTM 功能调用的引擎，提供简洁的 API 接口
- **特性**: 
  - 简化的 API 调用
  - 统一的错误处理
  - 支持命令行操作
  - 完整的 DTM 功能支持

## 核心接口

### IDtmService
DTM 服务的核心接口，定义了所有 DTM 操作方法：

| 方法名 | 描述 | 参数 | 返回值 |
|--------|------|------|--------|
| ExecuteCommandAsync | 执行 DTM 命令 | commandType: DtmCommandType, parameters: Dictionary<string, string>? | Task<DtmCommandResult> |
| ExecuteTransactionAsync | 执行分布式事务 | transactionId: string?, operations: List<string>, timeoutMs: int? | Task<DtmCommandResult> |
| QueryTransactionStatusAsync | 查询事务状态 | transactionId: string | Task<DtmCommandResult> |
| CancelTransactionAsync | 取消事务 | transactionId: string | Task<DtmCommandResult> |
| ResumeTransactionAsync | 恢复事务 | transactionId: string | Task<DtmCommandResult> |
| CleanExpiredTransactionsAsync | 清理过期事务 | 无 | Task<DtmCommandResult> |
| GetVersionInfoAsync | 获取版本信息 | 无 | Task<DtmCommandResult> |

## 数据结构

### DtmCommandType（DTM 命令类型）
```csharp
public enum DtmCommandType
{
    ExecuteTransaction,  // 执行分布式事务
    QueryTransactionStatus,  // 查询事务状态
    CancelTransaction,  // 取消事务
    ResumeTransaction,  // 恢复事务
    CleanExpiredTransactions,  // 清理过期事务
    VersionInfo  // 显示版本信息
}
```

### DtmCommandResult（DTM 命令结果）
```csharp
public class DtmCommandResult
{
    public bool Success { get; set; }              // 命令是否成功
    public DtmCommandType CommandType { get; set; } // 命令类型
    public List<string> Results { get; set; }       // 结果数据
    public long ExecutionTimeMs { get; set; }       // 执行时间（毫秒）
    public string? ErrorMessage { get; set; }       // 错误信息
    public string? TransactionId { get; set; }      // 事务ID
}
```

### DtmOptions（DTM 配置选项）
```csharp
public class DtmOptions
{
    public int DefaultTimeoutMs { get; set; } = 30000;  // 默认超时时间（毫秒）
    public bool EnableTransactionCache { get; set; } = true;  // 是否启用事务缓存
    public int MaxCacheSize { get; set; } = 1000;  // 最大缓存事务数量
    public bool EnableDetailedLogging { get; set; } = false;  // 是否启用详细日志
    public bool EnablePerformanceMonitoring { get; set; } = true;  // 是否启用性能监控
    public int TransactionCleanupIntervalMs { get; set; } = 60000;  // 事务清理间隔（毫秒）
    public int TransactionExpiryMs { get; set; } = 3600000;  // 事务过期时间（毫秒）
}
```

## 配置选项

### DTM 配置（dtm_aot.setting.json）

```json
{
  "Dtm": {
    "DefaultTimeoutMs": 30000,                      // 默认超时时间（毫秒）
    "EnableTransactionCache": true,                // 是否启用事务缓存
    "MaxCacheSize": 1000,                         // 最大缓存事务数量
    "EnableDetailedLogging": false,                // 是否启用详细日志
    "EnablePerformanceMonitoring": true,           // 是否启用性能监控
    "TransactionCleanupIntervalMs": 60000,         // 事务清理间隔（毫秒）
    "TransactionExpiryMs": 3600000                // 事务过期时间（毫秒）
  }
}
```

## 运行配置（dtm_aot.run.json）

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
    "Microsoft.Extensions.Options": "10.0.0"
  }
}
```

## 性能优化

1. **启用 AOT 编译**：AOT 编译可以提供极致的启动速度和运行性能
2. **异步编程**：使用异步 API 可以提高系统的并发处理能力
3. **合理配置超时时间**：根据事务复杂度调整超时时间
4. **启用事务缓存**：缓存可以提高重复事务的处理速度
5. **定期清理缓存**：定期清理过期事务，释放系统资源
6. **日志级别控制**：在生产环境中，将日志级别设置为 Information 或更高，减少日志开销

## 故障排除

### 常见问题

1. **事务执行失败**
   - 检查事务操作是否正确
   - 验证事务 ID 是否有效
   - 查看日志获取详细错误信息
   - 检查事务执行超时设置

2. **事务状态查询失败**
   - 检查事务 ID 是否正确
   - 验证事务是否已过期
   - 查看日志获取详细错误信息

3. **命令执行超时**
   - 调整超时时间配置
   - 优化事务操作，减少执行时间
   - 考虑将复杂事务拆分为多个简单事务

4. **服务异常**
   - 查看日志获取详细错误信息
   - 检查配置文件是否正确
   - 重启应用程序

5. **缓存问题**
   - 清理过期事务（使用 clean 命令）
   - 调整缓存大小配置
   - 检查缓存目录权限

## 扩展开发

### 自定义 DTM 服务

1. 实现 IDtmService 接口
2. 重写需要自定义的方法
3. 注册自定义服务

```csharp
builder.Services.AddSingleton<IDtmService, CustomDtmService>();
```

### 扩展命令类型

1. 在 DtmCommandType 枚举中添加新的命令类型
2. 在 ExecuteCommandAsync 方法中添加相应的处理逻辑
3. 添加对应的便捷方法

## AOT 编译注意事项

1. **依赖项**：确保所有依赖项都支持 AOT 编译
2. **反射**：避免在运行时使用反射，或使用 AOT 友好的反射方式
3. **动态类型**：谨慎使用 dynamic 类型，可能会影响 AOT 编译效果
4. **配置文件**：AOT 编译后，配置文件路径可能需要调整
5. **测试**：在 AOT 模式下进行充分测试，确保所有功能正常工作

## 命令行工具

该技能提供了命令行工具，支持以下命令：

- `execute <operations> [transactionId]`: 执行分布式事务
- `query <transactionId>`: 查询事务状态
- `cancel <transactionId>`: 取消事务
- `resume <transactionId>`: 恢复事务
- `clean`: 清理过期事务
- `version`: 显示版本信息
- `help, --help, -h`: 显示帮助信息

使用示例：
```bash
dtm_aot.exe execute 'op1,op2,op3'              # 执行分布式事务
dtm_aot.exe execute 'op1,op2' tx123            # 使用指定ID执行事务
dtm_aot.exe query tx123                        # 查询事务状态
dtm_aot.exe cancel tx123                       # 取消事务
dtm_aot.exe resume tx123                       # 恢复事务
dtm_aot.exe clean                              # 清理过期事务
dtm_aot.exe version                            # 显示版本信息
```

