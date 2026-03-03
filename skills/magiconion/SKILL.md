# magiconion 智能体技能 - magiconion 技能

## 技能概述

基于 .NET 10 的高性能 magiconion 技能，为 .NET 开发者提供强大的 magiconion 功能。

## 快速开始指南

### 安装依赖

在主应用的运行文件中添加以下依赖：

`yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package System.Threading.Channels@8.0.0
`

### 注册服务

在主应用中注册 magiconion 服务：

`csharp
// 注册 magiconion 服务
builder.Services.AddSingleton<IMagiconionProcessor, MagiconionProcessor>();
builder.Services.AddSingleton<IMagiconionService, MagiconionService>();
`

### 使用示例

`csharp
// 获取 magiconion 服务
var magiconionService = serviceProvider.GetRequiredService<IMagiconionService>();

// 使用 magiconion 功能
var result = await magiconionService.ProcessAsync(data);
Console.WriteLine($"处理结果: {result}");
`

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

### Channel 事件处理

magiconion 技能使用 Threading.Channels 进行高效的异步事件队列处理，支持背压控制：

```csharp
// 创建事件通道
var eventChannel = Channel.CreateBounded<MagiconionEvent>(
    new BoundedChannelOptions(10_000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true,
        FullMode = BoundedChannelFullMode.DropOldest
    });

// 处理事件
async Task ProcessEventsAsync()
{
    await foreach (var @event in eventChannel.Reader.ReadAllAsync())
    {
        await HandleEventAsync(@event);
    }
}

// 启动事件处理任务
_ = Task.Run(ProcessEventsAsync);
```

### 执行脚本和工作流

magiconion 技能提供了多个执行脚本，支持不同场景的使用：

1. **基础脚本**：`magiconion_chat.cs` - 提供基本的聊天功能
2. **集成脚本**：`magiconion_integration.cs` - 提供系统集成功能
3. **网格脚本**：`magiconion_mesh.cs` - 提供网格计算功能
4. **完整网格**：`magiconion_mesh_full.cs` - 提供完整的网格计算功能
5. **可观测性**：`magiconion_observability.cs` - 提供系统可观测性功能
6. **持久化**：`magiconion_persistence.cs` - 提供数据持久化功能
7. **完整持久化**：`magiconion_persistence_full.cs` - 提供完整的数据持久化功能
8. **实时处理**：`magiconion_realtime.cs` - 提供实时数据处理功能
9. **完整实时**：`magiconion_realtime_full.cs` - 提供完整的实时数据处理功能
10. **待办事项**：`magiconion_todo.cs` - 提供待办事项管理功能
11. **追踪**：`magiconion_tracing.cs` - 提供系统追踪功能
12. **完整追踪**：`magiconion_tracing_full.cs` - 提供完整的系统追踪功能

## 导航地图

`
magiconion/
????? index.yaml                   # 元数据索引描述
????? SKILL.md                    # 技能入口点（当前文件）
????? reference/                  # 参考文件
??  ????? README.md              # 完整功能描述
??  ????? examples.md            # 使用示例
????? scripts/                    # 脚本和工具
    ????? magiconion_chat.cs     # 基本聊天功能实现
    ????? magiconion_chat.run.json  # 聊天功能运行配置
    ????? magiconion_chat.setting.json  # 聊天功能设置文件
    ????? magiconion_integration.cs  # 系统集成功能实现
    ????? magiconion_integration.run.json  # 集成功能运行配置
    ????? magiconion_integration.setting.json  # 集成功能设置文件
    ????? magiconion_mesh.cs  # 网格计算功能实现
    ????? magiconion_mesh.run.json  # 网格计算运行配置
    ????? magiconion_mesh.setting.json  # 网格计算设置文件
    ????? magiconion_mesh_full.cs  # 完整网格计算功能实现
    ????? magiconion_mesh_full.run.json  # 完整网格计算运行配置
    ????? magiconion_mesh_full.setting.json  # 完整网格计算设置文件
    ????? magiconion_observability.cs  # 可观测性功能实现
    ????? magiconion_observability.run.json  # 可观测性运行配置
    ????? magiconion_observability.setting.json  # 可观测性设置文件
    ????? magiconion_persistence.cs  # 数据持久化功能实现
    ????? magiconion_persistence.run.json  # 持久化运行配置
    ????? magiconion_persistence.setting.json  # 持久化设置文件
    ????? magiconion_persistence_full.cs  # 完整数据持久化功能实现
    ????? magiconion_persistence_full.run.json  # 完整持久化运行配置
    ????? magiconion_persistence_full.setting.json  # 完整持久化设置文件
    ????? magiconion_realtime.cs  # 实时数据处理功能实现
    ????? magiconion_realtime.run.json  # 实时处理运行配置
    ????? magiconion_realtime.setting.json  # 实时处理设置文件
    ????? magiconion_realtime_full.cs  # 完整实时数据处理功能实现
    ????? magiconion_realtime_full.run.json  # 完整实时处理运行配置
    ????? magiconion_realtime_full.setting.json  # 完整实时处理设置文件
    ????? magiconion_todo.cs  # 待办事项管理功能实现
    ????? magiconion_todo.run.json  # 待办事项运行配置
    ????? magiconion_todo.setting.json  # 待办事项设置文件
    ????? magiconion_tracing.cs  # 系统追踪功能实现
    ????? magiconion_tracing.run.json  # 追踪运行配置
    ????? magiconion_tracing.setting.json  # 追踪设置文件
    ????? magiconion_tracing_full.cs  # 完整系统追踪功能实现
    ????? magiconion_tracing_full.run.json  # 完整追踪运行配置
    ????? magiconion_tracing_full.setting.json  # 完整追踪设置文件
`

## 主要功能

1. **核心功能 1**：magiconion 核心功能描述
2. **核心功能 2**：magiconion 核心功能描述
3. **核心功能 3**：magiconion 核心功能描述
4. **高性能设计**：优化的性能实现，使用 Threading.Channels 和对象池
5. **易用 API**：简单直观的 API 设计
6. **可扩展架构**：支持自定义扩展

## 扩展说明

本技能提供了完整的 magiconion 解决方案，您可以根据需要进行扩展：

1. **自定义实现**：实现 IMagiconionProcessor 接口
2. **扩展功能**：添加新的 magiconion 功能
3. **系统集成**：与其他系统集成
4. **性能优化**：针对特定场景优化性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：正确处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **AOT 编译**：使用 AOT 编译提升启动速度和运行性能
7. **内存优化**：使用对象池和 Span 零拷贝技术优化内存使用
