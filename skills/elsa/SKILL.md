# elsa Agent Skill - elsa 技能

## 技能概述

基于 .NET 10 的高性能 elsa 技能，为 .NET 开发者提供强大的工作流管理功能，支持 AOT 编译以实现极致性能。

## 快速入门指南

### 安装依赖

在主应用程序的 runfile 中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
```

### 注册服务

在主应用程序中注册 elsa 服务：

```csharp
// 注册 elsa 服务
builder.Services.AddElsaAot();
```

### 使用示例

```csharp
// 获取 elsa 服务
var elsaService = serviceProvider.GetRequiredService<IElsaService>();

// 启动工作流
var result = await elsaService.StartWorkflowAsync();
Console.WriteLine($"工作流已启动，实例 ID: {result.WorkflowInstanceId}");

// 查询工作流状态
var statusResult = await elsaService.QueryWorkflowStatusAsync(result.WorkflowInstanceId);
Console.WriteLine($"工作流状态: {statusResult.Results[1]}");
```

## 导航地图

```
elsa/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── elsa_aot.cs            # Elsa AOT 核心实现
    ├── elsa_aot.run.json      # 运行配置
    ├── elsa_aot.setting.json  # 应用程序设置
    ├── elsa_workflow_integration.cs            # 工作流集成实现
    ├── elsa_workflow_integration.run.json      # 工作流集成运行配置
    └── elsa_workflow_integration.setting.json  # 工作流集成应用程序设置
```

## 主要功能

1. **工作流管理**：支持工作流的启动、暂停、恢复、终止等完整生命周期管理
2. **工作流状态查询**：提供详细的工作流状态查询和实例列表功能
3. **AOT 编译支持**：基于 .NET 10 AOT 编译，提供极致的性能和启动速度
4. **高性能设计**：优化的内存使用和并发支持，适合高负载场景
5. **易用的 API**：简单直观的 API 设计，便于集成到各种应用程序
6. **可扩展架构**：支持自定义扩展和集成

## 扩展说明

本技能提供了完整的 elsa 工作流管理解决方案，您可以根据需要进行扩展：

1. **自定义服务实现**：实现 `IElsaService` 接口来扩展或替换默认功能
2. **添加新命令**：扩展 `ElsaCommandType` 枚举以添加新的工作流命令
3. **集成其他系统**：将工作流管理与其他业务系统集成
4. **性能优化**：针对特定场景进行性能优化

## 最佳实践

1. **依赖注入**：使用依赖注入来管理服务，便于测试和扩展
2. **异步编程**：优先使用异步 API，避免阻塞主线程
3. **错误处理**：正确处理异常情况，确保系统稳定性
4. **日志记录**：添加适当的日志记录，便于调试和监控
5. **性能监控**：监控关键性能指标，及时发现和解决性能问题
6. **AOT 编译**：使用 AOT 编译发布模式，获得最佳性能
7. **配置管理**：使用 `Options` 模式管理配置，便于环境切换

## AOT 编译说明

本技能支持 .NET 10 AOT 编译，通过以下特性实现极致性能：

- **PublishAot=true**：启用 AOT 编译
- **InvariantGlobalization=true**：使用不变全球化模式，减少包大小
- **EnableCompilationRelaxations=true**：启用编译优化
- **PublishReadyToRun=true**：启用 ReadyToRun 编译，加速启动

## 命令行使用

使用以下命令行参数运行 elsa_aot：

```bash
# 显示帮助信息
dotnet run --project elsa_aot.cs -- help

# 启动新工作流
dotnet run --project elsa_aot.cs -- start

# 暂停工作流
dotnet run --project elsa_aot.cs -- suspend <instance-id>

# 恢复工作流
dotnet run --project elsa_aot.cs -- resume <instance-id>

# 终止工作流
dotnet run --project elsa_aot.cs -- terminate <instance-id>

# 查询工作流状态
dotnet run --project elsa_aot.cs -- status <instance-id>

# 列出工作流实例
dotnet run --project elsa_aot.cs -- list

# 显示版本信息
dotnet run --project elsa_aot.cs -- version
```
