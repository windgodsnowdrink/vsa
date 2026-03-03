# dtm Agent Skill - dtm 技能

## 技能概述

基于 .NET 10 构建的高性能 dtm 技能，为 .NET 开发者提供强大的分布式事务管理功能支持。该技能采用 AOT（预编译）技术，提供极致的性能表现和启动速度，适用于各种分布式事务场景。

## 快速入门指南

### 安装依赖

在主应用程序的运行文件中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
```

### 注册服务

在主应用程序中注册 dtm 服务：

```csharp
// 配置 dtm 选项
builder.Configuration.AddJsonFile("dtm_aot.setting.json", optional: true);
builder.Services.Configure<DTM.AOT.DtmOptions>(builder.Configuration.GetSection("Dtm"));

// 注册 dtm 服务
builder.Services.AddDtm();
```

### 使用示例

```csharp
// 获取 dtm 服务实例
var dtmService = serviceProvider.GetRequiredService<DTM.AOT.IDtmService>();

// 执行分布式事务
var operations = new List<string> { "operation1", "operation2", "operation3" };
var runResult = await dtmService.ExecuteTransactionAsync(null, operations);
Console.WriteLine($"Transaction result: {runResult.Success}, ID: {runResult.TransactionId}");

// 查询事务状态
var queryResult = await dtmService.QueryTransactionStatusAsync(runResult.TransactionId);
Console.WriteLine($"Transaction status: {string.Join(", ", queryResult.Results)}");
```

## 导航地图

```
dtm/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── dtm_aot.cs             # dtm 核心实现（AOT）
    ├── dtm_aot.run.json       # 运行配置
    ├── dtm_aot.setting.json   # 应用设置
    └── dtm_demo.cs            # dtm 示例脚本
```

## 主要功能

1. **分布式事务执行**：支持执行包含多个操作的分布式事务
2. **事务状态查询**：实时查询事务的执行状态
3. **事务取消和恢复**：支持取消正在执行的事务或恢复已取消的事务
4. **过期事务清理**：自动清理过期的事务记录
5. **版本信息查询**：获取 dtm 引擎的版本信息
6. **高性能设计**：基于 AOT 编译的优化性能实现
7. **易用 API**：简单直观的 API 设计，易于集成
8. **可扩展架构**：支持自定义扩展和功能增强

## 扩展说明

该技能提供了完整的 dtm 分布式事务解决方案，您可以根据需要进行扩展：

1. **自定义事务实现**：实现 IDtmService 接口，自定义事务处理逻辑
2. **扩展命令类型**：添加新的事务命令类型和处理逻辑
3. **集成其他系统**：与其他系统和框架集成，实现更复杂的分布式事务场景
4. **性能优化**：针对特定场景优化事务执行性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务，提高代码可测试性和可维护性
2. **异步编程**：优先使用异步 API，避免阻塞主线程
3. **合理配置**：根据实际需求调整事务超时时间和缓存设置
4. **错误处理**：妥善处理事务执行中的各种异常情况
5. **日志记录**：适当添加日志记录，便于调试和监控
6. **性能监控**：启用性能监控，实时了解事务执行性能
7. **定期清理**：定期清理过期事务，释放系统资源

## AOT 编译说明

该技能支持 .NET 10 AOT 编译，通过预编译将应用程序编译为本地机器代码，提供以下优势：

- **极致的启动速度**：无需 JIT 编译，直接运行本地代码
- **减少内存占用**：更小的运行时占用
- **提高安全性**：减少可攻击面，提高应用程序安全性
- **跨平台支持**：支持多种操作系统和架构

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
```
dtm_aot.exe execute 'op1,op2,op3'              执行分布式事务

dtm_aot.exe execute 'op1,op2' tx123        使用指定事务ID执行事务
dtm_aot.exe query tx123                    查询事务状态
dtm_aot.exe cancel tx123                   取消事务
dtm_aot.exe resume tx123                   恢复事务
dtm_aot.exe clean                          清理过期事务
dtm_aot.exe version                        显示版本信息
```
