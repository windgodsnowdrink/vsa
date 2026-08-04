# LiquidState Agent Skill - 状态机技能

## 技能概述

基于.NET 10的高性能状态机工具，提供强大的状态机功能，支持AOT编译，适用于各种复杂的状态管理场景。

## 快速开始指南

### 安装依赖

在主应用程序的runfile中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Caching.Memory@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.CommandLine@2.0.0
#:package System.Threading.Channels@8.0.0
```

### AOT架构执行说明

本技能支持AOT（Ahead-of-Time）编译，提供更高的性能和更小的部署体积：

#### AOT编译配置

```yaml
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
#:property Optimize=true
#:property PublishTrimmed=true
#:property PublishAot=true
#:property IncludeNativeLibrariesForSelfExtract=true
#:property EnableCppCodeGen=true
```

#### AOT优势

1. **启动速度快**：AOT编译消除了JIT编译开销，启动速度显著提升
2. **运行时性能高**：预编译的本机代码执行效率更高
3. **内存占用小**：无需携带完整的.NET运行时，部署体积更小
4. **安全性强**：减少了JIT编译的攻击面
5. **可预测性好**：避免了运行时JIT编译的性能波动

#### Channel事件处理

本技能使用System.Threading.Channels实现高效的事件处理：

1. **异步事件处理**：使用Channel实现异步事件队列
2. **背压处理**：支持多种Channel满时的处理策略
3. **并发优化**：根据配置自动优化并发处理
4. **内存安全**：避免了传统队列的线程安全问题

#### 执行脚本

- **主脚本**：`scripts/liquidstate_aot.cs` - 状态机核心实现，支持AOT编译
- **配置文件**：`scripts/liquidstate_aot.setting.json` - 详细配置选项
- **运行环境**：`scripts/liquidstate_aot.run.json` - 运行时环境配置

#### 执行流程

1. **编译阶段**：使用AOT编译生成单文件可执行文件
2. **部署阶段**：将编译后的文件部署到目标环境
3. **运行阶段**：执行编译后的可执行文件，处理状态机操作
4. **监控阶段**：通过日志和监控系统跟踪状态机运行状态

### 注册服务

在主应用程序中注册状态机服务：

```csharp
// 注册状态机服务
builder.Services.AddSingleton<StateMachineService>();
builder.Services.AddMemoryCache();
builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});
```

### 使用示例

```csharp
// 获取状态机服务
var stateMachineService = serviceProvider.GetRequiredService<StateMachineService>();

// 创建状态机构建器
var builder = stateMachineService.CreateBuilder<string, string>();

// 配置状态转换
builder.AddTransition("Idle", "Start", "Running", async () =>
{
    Console.WriteLine("启动系统...");
    await Task.Delay(1000);
    Console.WriteLine("系统已启动");
});

// 构建并注册状态机
var stateMachine = builder.Build(serviceProvider);
stateMachineService.RegisterStateMachine("system", stateMachine);

// 启动状态机
stateMachine.Start("Idle");

// 触发事件
await stateMachine.FireAsync("Start");
Console.WriteLine($"当前状态: {stateMachine.CurrentState}");
```

## 导航地图

```
liquidstate/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── liquidstate_aot.cs     # 状态机核心实现
    ├── liquidstate_aot.run.json  # 运行配置
    └── liquidstate_aot.setting.json  # 设置文件
```

## 主要功能

1. **状态机核心功能**：完整的状态机实现，支持状态转换、事件触发
2. **流畅的API设计**：使用构建器模式，提供直观的配置方式
3. **异步操作支持**：所有状态转换和动作都支持异步操作
4. **依赖注入集成**：与Microsoft.Extensions.DependencyInjection无缝集成
5. **内存缓存支持**：使用Microsoft.Extensions.Caching.Memory提高性能
6. **日志记录**：集成Microsoft.Extensions.Logging，提供详细的运行日志
7. **命令行界面**：提供完整的命令行工具，支持创建、启动、触发事件等操作
8. **高性能设计**：优化的性能实现，支持AOT编译
9. **跨平台支持**：支持Windows、Linux、macOS等多个平台

## 扩展说明

本技能提供了完整的状态机解决方案，您可以根据需要进行扩展：

1. **自定义状态和事件类型**：支持任意类型的状态和事件
2. **添加自定义转换动作**：为状态转换添加自定义业务逻辑
3. **实现条件转换**：基于条件判断的状态转换
4. **集成其他系统**：与其他系统和服务集成
5. **性能优化**：针对特定场景进行性能优化

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务，提高代码可测试性
2. **异步编程**：优先使用异步API，避免阻塞主线程
3. **错误处理**：正确处理异常情况，确保状态机稳定运行
4. **日志记录**：添加适当的日志，便于调试和监控
5. **状态设计**：合理设计状态和转换，避免复杂的状态关系
6. **性能监控**：监控关键性能指标，及时发现问题
7. **配置管理**：使用配置文件管理状态机设置，提高灵活性
