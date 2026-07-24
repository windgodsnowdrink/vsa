# Reactive 技能参考文档

## 技能概述

Reactive 技能是一个基于 .NET 10 的高性能响应式系统，专为 .NET 开发者设计，提供了完整的响应式编程框架和工具集。

- **基于 ReactiveUI 和 System.Reactive**：集成了成熟的响应式编程库，提供强大的响应式功能
- **支持 AOT 编译**：优化运行时性能，减少启动时间和内存占用
- **内存管理优化**：实现对象池，减少垃圾回收，提高性能
- **依赖注入集成**：与 .NET 标准依赖注入框架无缝集成
- **配置管理**：提供灵活的配置选项，支持运行时调整

## 核心组件

### 1. ReactiveService
- **位置**：scripts/rx_reactiveui_mvvm.cs
- **功能**：核心业务逻辑处理和响应式操作
- **特性**：
  - 响应式编程实现
  - 异步操作支持
  - 性能优化
  - 错误处理
  - 日志记录

### 2. ReactiveOptions
- **位置**：scripts/rx_reactiveui_mvvm.cs
- **功能**：配置选项管理
- **特性**：
  - 内存池配置
  - 批处理配置
  - 调试日志配置
  - 调度器优化配置

### 3. ReactiveViewModelBase
- **位置**：scripts/rx_reactiveui_mvvm.cs
- **功能**：响应式 ViewModel 基类
- **特性**：
  - 属性变更通知
  - 命令支持
  - 生命周期管理
  - 依赖注入集成

### 4. ObjectPool
- **位置**：scripts/rx_reactiveui_mvvm.cs
- **功能**：对象池实现，用于内存管理优化
- **特性**：
  - 线程安全
  - 最大池大小限制
  - 自动回收
  - 高性能

## 快速开始

### 1. 安装依赖

在项目中添加以下包引用：

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
  <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.0" />
  <PackageReference Include="Microsoft.Extensions.Options" Version="10.0.0" />
  <PackageReference Include="ReactiveUI" Version="18.3.1" />
  <PackageReference Include="System.Reactive" Version="6.0.0" />
  <PackageReference Include="System.Threading.Channels" Version="8.0.0" />
  <PackageReference Include="System.Runtime.CompilerServices.Unsafe" Version="6.0.0" />
</ItemGroup>
```

### 2. 注册服务

在 `Program.cs` 中注册 Reactive 服务：

```csharp
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 注册 Reactive 服务
builder.Services.AddReactiveServices(options => {
    options.EnableMemoryPooling = true;
    options.MaxPoolSize = 1000;
    options.EnableBatchProcessing = false;
});

var app = builder.Build();
// ...
app.Run();
```

### 3. 使用 Reactive 服务

在需要使用 Reactive 服务的地方注入 `IReactiveService`：

```csharp
using System.Reactive.Linq;

public class MyService
{
    private readonly IReactiveService _reactiveService;

    public MyService(IReactiveService reactiveService)
    {
        _reactiveService = reactiveService;
    }

    public async Task DoSomethingAsync()
    {
        // 执行异步操作
        var result = await _reactiveService.DoSomethingAsync();
        Console.WriteLine(result);

        // 创建可观察对象
        var observable = _reactiveService.CreateObservable(() => "Hello, Reactive!");

        // 订阅事件
        _reactiveService.SubscribeToEvents(observable, Console.WriteLine);
    }
}
```

## 目录结构

```
reactive/
├── index.yaml          # 技能元数据
├── SKILL.md            # 技能文档
├── scripts/            # 脚本目录
│   ├── rx_reactiveui_mvvm.cs                # 主脚本文件
│   ├── rx_reactiveui_mvvm.setting.json      # 配置文件
│   └── rx_reactiveui_mvvm.run.json          # 运行配置文件
└── reference/          # 参考文档
    ├── README.md       # 参考文档
    └── examples.md     # 示例文档
```

## 主要功能

### 1. 响应式编程

- **Observable**：创建和管理可观察序列
- **Subscription**：订阅和处理事件
- **Operators**：使用丰富的操作符处理数据流
- **Scheduler**：控制事件执行的线程和时机

### 2. MVVM 模式

- **ReactiveViewModelBase**：响应式 ViewModel 基类
- **ReactiveCommand**：响应式命令，支持异步操作
- **WhenAnyValue**：监听属性变化
- **Bind**：属性绑定

### 3. 依赖注入

- **AddReactiveServices**：注册 Reactive 服务
- **IOptions<ReactiveOptions>**：配置选项注入
- **Scoped**：作用域服务

### 4. 内存管理

- **ObjectPool**：对象池实现
- **CompositeDisposable**：组合 disposable
- **ThreadLocal**：线程本地存储

### 5. 配置管理

- **ReactiveOptions**：配置选项类
- **appsettings.json**：配置文件
- **Environment Variables**：环境变量

## AOT 编译配置

### 项目配置

在 `.csproj` 文件中添加以下配置：

```xml
<PropertyGroup>
  <TargetFramework>net11.0</TargetFramework>
  <PublishAot>true</PublishAot>
  <TrimMode>partial</TrimMode>
  <ReadyToRun>true</ReadyToRun>
  <TieredCompilation>true</TieredCompilation>
  <Optimize>true</Optimize>
  <LangVersion>preview</LangVersion>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>
```

### 运行时配置

在 `appsettings.json` 文件中添加以下配置：

```json
{
  "Reactive": {
    "EnableMemoryPooling": true,
    "MaxPoolSize": 1000,
    "EnableBatchProcessing": false,
    "BatchSize": 100,
    "BatchTimeout": 100,
    "EnableDebugLogging": false,
    "EnableSchedulerOptimization": true,
    "EnableSubscriptionTracking": false
  }
}
```

## 性能优化

1. **启用内存池**：通过 `EnableMemoryPooling` 配置启用内存池，减少垃圾回收
2. **使用异步编程**：使用 async/await 模式，避免阻塞
3. **批处理**：对于大量操作，使用批处理模式提高效率
4. **对象池**：使用 `ObjectPool` 复用对象，减少内存分配
5. **调度器优化**：使用合适的调度器，控制事件执行的线程
6. **订阅管理**：及时取消订阅，避免内存泄漏

## 故障排除

### 常见问题

1. **内存泄漏**
   - 检查是否及时取消订阅
   - 检查是否正确使用 `Dispose` 方法
   - 启用订阅跟踪，查看是否有未释放的订阅

2. **性能问题**
   - 启用内存池
   - 优化批处理大小
   - 检查调度器使用是否合理
   - 启用调试日志，查看性能瓶颈

3. **AOT 编译错误**
   - 确保所有依赖项支持 AOT 编译
   - 调整 `TrimMode` 为 `partial` 或 `full`
   - 检查是否使用了反射或动态类型

4. **依赖注入错误**
   - 确保所有服务都已正确注册
   - 检查依赖项的生命周期是否正确
   - 检查是否存在循环依赖

## 最佳实践

1. **使用 MVVM 模式**：遵循 MVVM 模式，分离 UI 和业务逻辑
2. **使用响应式命令**：使用 `ReactiveCommand` 处理用户操作
3. **使用 WhenAnyValue**：使用 `WhenAnyValue` 监听属性变化，避免手动事件处理
4. **及时取消订阅**：使用 `DisposeWith` 或 `CompositeDisposable` 管理订阅生命周期
5. **使用对象池**：对于频繁创建和销毁的对象，使用 `ObjectPool` 提高性能
6. **配置优化**：根据实际场景调整配置选项，如内存池大小、批处理大小等
7. **日志记录**：适当添加日志，便于排查问题
8. **异常处理**：合理处理异常，避免应用崩溃

## 扩展开发

### 添加自定义功能

1. **继承 ReactiveViewModelBase**：创建自定义 ViewModel

```csharp
public class CustomViewModel : ReactiveViewModelBase
{
    public CustomViewModel(ObjectPool<CompositeDisposable> objectPool)
        : base(objectPool)
    {
        // 初始化
    }

    // 属性
    private string _customProperty;
    public string CustomProperty
    {
        get => _customProperty;
        set => this.RaiseAndSetIfChanged(ref _customProperty, value);
    }

    // 命令
    private ReactiveCommand<Unit, Unit> _customCommand;
    public ReactiveCommand<Unit, Unit> CustomCommand => _customCommand ??= ReactiveCommand.CreateFromTask(async () => {
        // 命令逻辑
    });
}
```

2. **扩展 IReactiveService**：创建自定义服务

```csharp
public class CustomReactiveService : ReactiveService
{
    public CustomReactiveService(IOptions<ReactiveOptions> options, ILogger<ReactiveService> logger, ObjectPool<CompositeDisposable> objectPool)
        : base(options, logger, objectPool)
    {
    }

    public async Task<string> CustomOperationAsync()
    {
        // 自定义操作逻辑
        return "Custom operation result";
    }
}
```

3. **注册自定义服务**：

```csharp
builder.Services.AddReactiveServices(options => {
    options.EnableMemoryPooling = true;
    options.MaxPoolSize = 1000;
});

builder.Services.AddScoped<IReactiveService, CustomReactiveService>();
builder.Services.AddTransient<CustomViewModel>();
```

## 示例代码

### 基本用法

```csharp
// 获取 Reactive 服务
var reactiveService = serviceProvider.GetRequiredService<IReactiveService>();

// 执行异步操作
var result = await reactiveService.DoSomethingAsync();
Console.WriteLine(result);

// 创建可观察对象
var observable = reactiveService.CreateObservable(() => DateTime.Now);

// 订阅事件
reactiveService.SubscribeToEvents(observable, time => {
    Console.WriteLine($"Current time: {time}");
});

// 执行带参数的异步操作
var data = await reactiveService.ExecuteAsync(async () => {
    // 模拟异步操作
    await Task.Delay(100);
    return "Hello, Reactive!";
});
Console.WriteLine(data);
```

### ViewModel 用法

```csharp
// 创建 ViewModel
var viewModel = new UserProfileViewModel(objectPool);

// 设置属性
viewModel.Name = "张三";
viewModel.Age = 30;

// 执行命令
if (viewModel.SaveCommand.CanExecute(null))
{
    await viewModel.SaveCommand.ExecuteAsync(null);
}

// 监听属性变化
viewModel.WhenAnyValue(x => x.Name, x => x.Age)
    .Subscribe(values => {
        var (name, age) = values;
        Console.WriteLine($"Name: {name}, Age: {age}");
    });
```

### 高级配置

```csharp
// 配置 Reactive 服务
builder.Services.AddReactiveServices(options => {
    options.EnableMemoryPooling = true;
    options.MaxPoolSize = 1000;
    options.EnableBatchProcessing = true;
    options.BatchSize = 100;
    options.BatchTimeout = 100;
    options.EnableDebugLogging = true;
    options.EnableSchedulerOptimization = true;
    options.EnableSubscriptionTracking = true;
});

// 配置日志
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```
