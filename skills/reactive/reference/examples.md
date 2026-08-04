# Reactive 技能使用示例

## 快速开始

### 1. 基本用法示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Reactive 基本用法示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var reactiveService = serviceProvider.GetRequiredService<IReactiveService>();
        
        // 执行异步操作
        var result = await reactiveService.DoSomethingAsync();
        Console.WriteLine($"执行结果: {result}");
        
        // 创建可观察对象
        var observable = reactiveService.CreateObservable(() => DateTime.Now);
        
        // 订阅事件
        Console.WriteLine("订阅时间事件...");
        reactiveService.SubscribeToEvents(observable, time => {
            Console.WriteLine($"当前时间: {time}");
        });
        
        // 等待一段时间，观察事件输出
        await Task.Delay(2000);
        
        Console.WriteLine("\n基本用法示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置日志
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册 Reactive 服务
        builder.AddReactiveServices(options => {
            options.EnableMemoryPooling = true;
            options.MaxPoolSize = 1000;
            options.EnableBatchProcessing = false;
        });
        
        return builder.BuildServiceProvider();
    }
}
```

### 2. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Reactive 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置日志
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Debug);
        });
        
        // 配置 Reactive 选项
        builder.AddReactiveServices(options => {
            options.EnableMemoryPooling = true;
            options.MaxPoolSize = 2000;
            options.EnableBatchProcessing = true;
            options.BatchSize = 100;
            options.BatchTimeout = 100;
            options.EnableDebugLogging = true;
            options.EnableSchedulerOptimization = true;
            options.EnableSubscriptionTracking = true;
        });
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<ReactiveOptions>>().Value;
        Console.WriteLine($"配置信息: ");
        Console.WriteLine($"  内存池启用: {settings.EnableMemoryPooling}");
        Console.WriteLine($"  最大池大小: {settings.MaxPoolSize}");
        Console.WriteLine($"  批处理启用: {settings.EnableBatchProcessing}");
        Console.WriteLine($"  批处理大小: {settings.BatchSize}");
        Console.WriteLine($"  批处理超时: {settings.BatchTimeout}ms");
        Console.WriteLine($"  调试日志启用: {settings.EnableDebugLogging}");
        Console.WriteLine($"  调度器优化启用: {settings.EnableSchedulerOptimization}");
        Console.WriteLine($"  订阅跟踪启用: {settings.EnableSubscriptionTracking}");
        
        // 使用服务
        var reactiveService = serviceProvider.GetRequiredService<IReactiveService>();
        var result = await reactiveService.DoSomethingAsync();
        Console.WriteLine($"\n执行结果: {result}");
        
        Console.WriteLine("\n高级配置示例完成！");
    }
}
```

### 3. MVVM 模式示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Reactive MVVM 模式示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var objectPool = serviceProvider.GetRequiredService<ObjectPool<CompositeDisposable>>();
        
        // 创建 ViewModel
        var viewModel = new UserProfileViewModel(objectPool);
        
        // 设置属性
        Console.WriteLine("设置 ViewModel 属性...");
        viewModel.Name = "张三";
        viewModel.Age = 30;
        
        Console.WriteLine($"ViewModel 属性: Name={viewModel.Name}, Age={viewModel.Age}");
        
        // 监听属性变化
        Console.WriteLine("\n监听属性变化...");
        viewModel.WhenAnyValue(x => x.Name, x => x.Age)
            .Subscribe(values => {
                var (name, age) = values;
                Console.WriteLine($"属性变化: Name={name}, Age={age}");
            });
        
        // 修改属性，触发变化
        await Task.Delay(1000);
        viewModel.Name = "李四";
        await Task.Delay(1000);
        viewModel.Age = 25;
        
        // 执行命令
        Console.WriteLine("\n执行 SaveCommand...");
        if (viewModel.SaveCommand.CanExecute(null))
        {
            await viewModel.SaveCommand.ExecuteAsync(null);
        }
        
        // 再次执行命令
        await Task.Delay(1000);
        Console.WriteLine("再次执行 SaveCommand...");
        if (viewModel.SaveCommand.CanExecute(null))
        {
            await viewModel.SaveCommand.ExecuteAsync(null);
        }
        
        Console.WriteLine("\nMVVM 模式示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置日志
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册 Reactive 服务
        builder.AddReactiveServices(options => {
            options.EnableMemoryPooling = true;
            options.MaxPoolSize = 1000;
        });
        
        return builder.BuildServiceProvider();
    }
}
```

### 4. 内存管理优化示例

```csharp
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Reactive 内存管理优化示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var reactiveService = serviceProvider.GetRequiredService<IReactiveService>();
        var objectPool = serviceProvider.GetRequiredService<ObjectPool<CompositeDisposable>>();
        
        // 性能测试
        const int iterations = 10000;
        var stopwatch = Stopwatch.StartNew();
        
        Console.WriteLine($"执行 {iterations} 次操作，测试内存管理性能...");
        
        for (int i = 0; i < iterations; i++)
        {
            // 从对象池获取 CompositeDisposable
            var disposable = objectPool.Get();
            
            try
            {
                // 创建可观察对象并订阅
                var observable = reactiveService.CreateObservable(() => i);
                var subscription = observable.Subscribe(value => {
                    // 处理事件
                });
                
                // 添加到 CompositeDisposable
                subscription.DisposeWith(disposable);
                
                // 模拟操作
                await Task.Delay(1);
            }
            finally
            {
                // 清理并返回对象池
                disposable.Clear();
                objectPool.Return(disposable);
            }
        }
        
        stopwatch.Stop();
        Console.WriteLine($"执行 {iterations} 次操作的时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每次操作时间: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
        
        // 测试内存使用
        var process = Process.GetCurrentProcess();
        Console.WriteLine($"内存使用: {process.WorkingSet64 / 1024 / 1024:F2} MB");
        
        Console.WriteLine("\n内存管理优化示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置日志
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Warning);
        });
        
        // 注册 Reactive 服务，启用内存池
        builder.AddReactiveServices(options => {
            options.EnableMemoryPooling = true;
            options.MaxPoolSize = 1000;
        });
        
        return builder.BuildServiceProvider();
    }
}
```

### 5. AOT 编译配置示例

```csharp
// 项目文件 (.csproj) 配置
/*
<Project Sdk="Microsoft.NET.Sdk">
  
  <PropertyGroup>
    <OutputType>Exe</OutputType>
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
  
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Options" Version="10.0.0" />
    <PackageReference Include="ReactiveUI" Version="18.3.1" />
    <PackageReference Include="System.Reactive" Version="6.0.0" />
    <PackageReference Include="System.Threading.Channels" Version="8.0.0" />
    <PackageReference Include="System.Runtime.CompilerServices.Unsafe" Version="6.0.0" />
  </ItemGroup>
  
</Project>
*/

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Reactive AOT 编译配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var reactiveService = serviceProvider.GetRequiredService<IReactiveService>();
        
        // 执行操作，测试 AOT 编译后的性能
        Console.WriteLine("测试 AOT 编译后的性能...");
        
        // 测试基本操作
        var result = await reactiveService.DoSomethingAsync();
        Console.WriteLine($"基本操作结果: {result}");
        
        // 测试异步执行
        var data = await reactiveService.ExecuteAsync(async () => {
            // 模拟异步操作
            await Task.Delay(100);
            return "Hello, AOT!";
        });
        Console.WriteLine($"异步执行结果: {data}");
        
        // 测试可观察对象
        var observable = reactiveService.CreateObservable(() => DateTime.Now);
        reactiveService.SubscribeToEvents(observable, time => {
            Console.WriteLine($"可观察对象事件: {time}");
        });
        
        // 等待一段时间
        await Task.Delay(1000);
        
        Console.WriteLine("\nAOT 编译配置示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置日志
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册 Reactive 服务
        builder.AddReactiveServices(options => {
            options.EnableMemoryPooling = true;
            options.MaxPoolSize = 1000;
            options.EnableBatchProcessing = false;
        });
        
        return builder.BuildServiceProvider();
    }
}
```

### 6. 错误处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Reactive 错误处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var reactiveService = serviceProvider.GetRequiredService<IReactiveService>();
        
        try
        {
            Console.WriteLine("测试正常操作...");
            var result = await reactiveService.DoSomethingAsync();
            Console.WriteLine($"成功: {result}");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"超时错误: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"操作错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用错误: {ex.Message}");
        }
        
        // 测试异步执行中的错误处理
        try
        {
            Console.WriteLine("\n测试异步执行中的错误处理...");
            var result = await reactiveService.ExecuteAsync(async () => {
                // 模拟错误
                throw new InvalidOperationException("测试错误");
            });
            Console.WriteLine($"成功: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"捕获到错误: {ex.Message}");
        }
        
        Console.WriteLine("\n错误处理示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置日志
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册 Reactive 服务
        builder.AddReactiveServices(options => {
            options.EnableMemoryPooling = true;
            options.MaxPoolSize = 1000;
        });
        
        return builder.BuildServiceProvider();
    }
}
```

### 7. 高级响应式操作示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Reactive 高级响应式操作示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var reactiveService = serviceProvider.GetRequiredService<IReactiveService>();
        
        // 示例 1: 组合多个可观察对象
        Console.WriteLine("示例 1: 组合多个可观察对象");
        
        var observable1 = reactiveService.CreateObservable(() => "Hello");
        var observable2 = reactiveService.CreateObservable(() => "World");
        
        var combinedObservable = Observable.CombineLatest(
            observable1,
            observable2,
            (value1, value2) => $"{value1} {value2}"
        );
        
        reactiveService.SubscribeToEvents(combinedObservable, Console.WriteLine);
        
        // 示例 2: 使用操作符
        Console.WriteLine("\n示例 2: 使用操作符");
        
        var numbersObservable = Observable.Range(1, 10);
        var filteredObservable = numbersObservable
            .Where(x => x % 2 == 0)  // 过滤偶数
            .Select(x => x * 2)       // 乘以 2
            .Take(3);                 // 只取前 3 个
        
        reactiveService.SubscribeToEvents(filteredObservable, value => {
            Console.WriteLine($"过滤后的值: {value}");
        });
        
        // 示例 3: 超时处理
        Console.WriteLine("\n示例 3: 超时处理");
        
        var timeoutObservable = Observable.Create<int>(async observer => {
            // 模拟长时间操作
            await Task.Delay(2000);
            observer.OnNext(42);
            observer.OnCompleted();
            return Disposable.Empty;
        });
        
        var timeoutHandledObservable = timeoutObservable
            .Timeout(TimeSpan.FromSeconds(1))
            .Catch(Observable.Return(-1)); // 超时返回 -1
        
        reactiveService.SubscribeToEvents(timeoutHandledObservable, value => {
            Console.WriteLine($"超时处理结果: {value}");
        });
        
        // 等待所有操作完成
        await Task.Delay(3000);
        
        Console.WriteLine("\n高级响应式操作示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置日志
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册 Reactive 服务
        builder.AddReactiveServices(options => {
            options.EnableMemoryPooling = true;
            options.MaxPoolSize = 1000;
        });
        
        return builder.BuildServiceProvider();
    }
}
```

## 示例总结

以上示例展示了 Reactive 技能的主要功能和使用方法。通过这些示例，您可以：

1. **快速开始**：了解基本操作和服务注册
2. **高级配置**：根据实际需求调整配置选项
3. **MVVM 模式**：使用响应式 ViewModel 处理 UI 逻辑
4. **内存管理**：优化内存使用，减少垃圾回收
5. **AOT 编译**：配置 AOT 编译，提高性能
6. **错误处理**：妥善处理各种错误情况
7. **高级响应式操作**：使用丰富的响应式操作符处理数据流

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

## 运行示例

要运行这些示例，您需要：

1. 确保安装了 .NET 10 SDK
2. 创建一个新的 .NET 10 控制台项目
3. 添加必要的包引用
4. 将示例代码复制到项目中
5. 运行项目

例如，使用以下命令创建项目并运行：

```bash
# 创建项目
dotnet new console -n ReactiveExample
cd ReactiveExample

# 添加包引用
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Logging
dotnet add package Microsoft.Extensions.Logging.Console
dotnet add package Microsoft.Extensions.Options
dotnet add package ReactiveUI
dotnet add package System.Reactive
dotnet add package System.Threading.Channels
dotnet add package System.Runtime.CompilerServices.Unsafe

# 复制示例代码到 Program.cs
# ...

# 运行项目
dotnet run

# 发布为 AOT 编译
dotnet publish -c Release -r win-x64 --self-contained true
```

通过这些示例，您可以充分了解 Reactive 技能的功能和用法，为您的项目提供高性能、响应式的解决方案。
