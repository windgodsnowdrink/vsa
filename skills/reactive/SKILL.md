# Reactive Agent Skill - Reactive 技能

## 技能概述

基于 .NET 10 的高性能 Reactive 技能，为 .NET 开发者提供强大的响应式编程功能，支持 ReactiveUI 集成、MVVM 模式、响应式命令和属性、事件处理等特性。

## 快速开始

### 安装依赖

在主应用程序的运行文件中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package ReactiveUI@18.3.1
#:package System.Reactive@6.0.0
#:package System.Threading.Channels@8.0.0
```

### 注册服务

在主应用程序中注册 Reactive 服务：

```csharp
// 注册 Reactive 服务
builder.Services.AddReactiveServices();

// 配置 Reactive 选项
builder.Services.Configure<ReactiveOptions>(options => {
    options.EnableMemoryPooling = true;
    options.MaxPoolSize = 1000;
    options.EnableBatchProcessing = false;
});
```

### 使用示例

```csharp
// 获取 Reactive 服务
var reactiveService = serviceProvider.GetRequiredService<IReactiveService>();

// 使用 Reactive 功能
var result = await reactiveService.DoSomethingAsync();
Console.WriteLine($"结果: {result}");

// 创建 ViewModel
var viewModel = new UserProfileViewModel(serviceProvider.GetRequiredService<ObjectPool<CompositeDisposable>>());

// 绑定属性
viewModel.Name = "张三";
viewModel.Age = 30;

// 执行命令
if (viewModel.SaveCommand.CanExecute(null)) {
    await viewModel.SaveCommand.ExecuteAsync(null);
}
```

## 目录结构

```
reactive/
????? index.yaml                   # 元数据索引描述
????? SKILL.md                    # 技能入口点（当前文件）
????? reference/                  # 参考文件
??  ????? README.md              # 完整功能描述
??  ????? examples.md            # 使用示例
????? scripts/                    # 脚本和工具
    ????? rx_reactiveui_mvvm.cs     # Reactive 核心实现
    ????? rx_reactiveui_mvvm.setting.json  # 设置文件
    ????? rx_reactiveui_mvvm.run.json  # 运行配置
```

## 主要功能

1. **ReactiveUI 集成**：提供与 ReactiveUI 的完整集成，支持响应式编程模式
2. **MVVM 模式支持**：实现完整的 MVVM 模式，包括 ViewModel 基类和命令绑定
3. **响应式命令和属性**：使用 ReactiveCommand 和 ReactiveObject 实现响应式操作
4. **事件处理**：使用 Rx 进行事件处理和流操作
5. **数据绑定和验证**：支持数据绑定和验证功能
6. **异步编程支持**：完整的异步编程模型
7. **内存管理优化**：使用对象池和其他技术优化内存使用
8. **高性能设计**：优化的性能实现，支持高并发场景

## 扩展说明

本技能提供了完整的 Reactive 解决方案，您可以根据需要进行扩展：

1. **自定义实现**：实现 IReactiveService 接口
2. **扩展功能**：添加新的 Reactive 功能
3. **与其他系统集成**：与其他系统集成
4. **性能优化**：针对特定场景优化性能

## AOT 编译配置

### 项目配置

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <PublishAot>true</PublishAot>
    <TrimMode>partial</TrimMode>
    <ReadyToRun>true</ReadyToRun>
    <TieredCompilation>true</TieredCompilation>
    <Optimize>true</Optimize>
    <LangVersion>preview</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

### 编译命令

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

## 故障排除

### 常见问题

1. **依赖注入错误**
   - 确保正确注册了所有服务
   - 检查依赖项版本是否匹配

2. **内存泄漏**
   - 确保正确处理 IDisposable 对象
   - 使用 CompositeDisposable 管理订阅

3. **性能问题**
   - 启用内存池
   - 优化订阅和取消订阅
   - 使用适当的调度器

4. **AOT 编译错误**
   - 避免使用反射
   - 确保所有依赖项支持 AOT
   - 配置正确的 TrimMode

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务生命周期
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：正确处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **内存管理**：使用对象池和 CompositeDisposable 管理资源
7. **响应式编程**：遵循 Reactive 编程最佳实践
8. **测试覆盖**：编写充分的单元测试和集成测试

## 性能优化技巧

1. **使用对象池**：减少内存分配和垃圾收集
2. **合理使用订阅**：及时取消不需要的订阅
3. **选择合适的调度器**：根据操作类型选择适当的调度器
4. **批处理操作**：对批量操作使用合适的批处理策略
5. **避免过度反应**：使用 Throttle 或 Debounce 控制事件频率
6. **优化数据流**：合理设计数据流，避免不必要的转换

## 支持的平台

- **Windows**：支持 Windows 10 及以上版本
- **Linux**：支持主流 Linux 发行版
- **macOS**：支持 macOS 10.15 及以上版本

## 运行时要求

- **.NET**：10.0 或更高版本
- **ReactiveUI**：18.3.1 或更高版本
- **System.Reactive**：6.0.0 或更高版本

## 总结

Reactive 技能是一个功能完整、性能优化的响应式编程解决方案，基于 .NET 10 和 ReactiveUI，提供了丰富的响应式编程功能和灵活的配置选项。通过合理配置和使用这些功能，可以构建高性能、可靠的响应式应用程序，满足各种业务场景的需求。
