# elsa - 使用示例

## 快速开始

### 1. 基本工作流管理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Elsa.AOT;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务容器
        var serviceProvider = BuildServiceProvider();
        var elsaService = serviceProvider.GetRequiredService<IElsaService>();
        
        Console.WriteLine("Elsa 基本工作流管理示例");
        Console.WriteLine("=" * 50);
        
        // 启动工作流
        Console.WriteLine("1. 启动工作流...");
        var startResult = await elsaService.StartWorkflowAsync();
        Console.WriteLine($"   工作流已启动，实例 ID: {startResult.WorkflowInstanceId}");
        Console.WriteLine($"   执行时间: {startResult.ExecutionTimeMs} ms");
        
        // 查询工作流状态
        Console.WriteLine($"\n2. 查询工作流状态 (实例 ID: {startResult.WorkflowInstanceId})...");
        var statusResult = await elsaService.QueryWorkflowStatusAsync(startResult.WorkflowInstanceId!);
        Console.WriteLine($"   状态查询成功，执行时间: {statusResult.ExecutionTimeMs} ms");
        foreach (var result in statusResult.Results)
        {
            Console.WriteLine($"   {result}");
        }
        
        // 列出所有工作流实例
        Console.WriteLine("\n3. 列出所有工作流实例...");
        var listResult = await elsaService.ListWorkflowInstancesAsync();
        Console.WriteLine($"   实例列表查询成功，找到 {listResult.WorkflowInstances?.Count} 个实例");
        foreach (var result in listResult.Results)
        {
            Console.WriteLine($"   {result}");
        }
        
        // 终止工作流
        Console.WriteLine($"\n4. 终止工作流 (实例 ID: {startResult.WorkflowInstanceId})...");
        var terminateResult = await elsaService.TerminateWorkflowAsync(startResult.WorkflowInstanceId!);
        Console.WriteLine($"   工作流已终止，执行时间: {terminateResult.ExecutionTimeMs} ms");
        
        Console.WriteLine("\n工作流管理示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        // 注册 Elsa AOT 服务
        builder.AddElsaAot();
        return builder.BuildServiceProvider();
    }
}
```

### 2. AOT 编译配置示例

```csharp
// 在项目文件中添加以下 AOT 编译配置
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true              // 启用 AOT 编译
#:property InvariantGlobalization=true  // 使用不变全球化模式
#:property EnableCompilationRelaxations=true // 启用编译优化
#:property PublishReadyToRun=true       // 启用 ReadyToRun 编译
```

### 3. 工作流生命周期管理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Elsa.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Elsa 工作流生命周期管理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var elsaService = serviceProvider.GetRequiredService<IElsaService>();
        
        // 启动工作流
        var startResult = await elsaService.StartWorkflowAsync();
        var workflowId = startResult.WorkflowInstanceId!;
        Console.WriteLine($"1. 工作流已启动，实例 ID: {workflowId}");
        
        // 暂停工作流
        var suspendResult = await elsaService.SuspendWorkflowAsync(workflowId);
        Console.WriteLine($"2. 工作流已暂停，执行时间: {suspendResult.ExecutionTimeMs} ms");
        
        // 恢复工作流
        var resumeResult = await elsaService.ResumeWorkflowAsync(workflowId);
        Console.WriteLine($"3. 工作流已恢复，执行时间: {resumeResult.ExecutionTimeMs} ms");
        
        // 终止工作流
        var terminateResult = await elsaService.TerminateWorkflowAsync(workflowId);
        Console.WriteLine($"4. 工作流已终止，执行时间: {terminateResult.ExecutionTimeMs} ms");
        
        Console.WriteLine("\n工作流生命周期管理示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddElsaAot();
        return builder.BuildServiceProvider();
    }
}
```

### 4. 命令行工具使用示例

```bash
# 显示帮助信息
dotnet run --project elsa_aot.cs -- help

# 启动工作流
dotnet run --project elsa_aot.cs -- start

# 启动指定定义的工作流
dotnet run --project elsa_aot.cs -- start my-custom-workflow

# 暂停工作流
dotnet run --project elsa_aot.cs -- suspend 12345678-1234-1234-1234-1234567890ab

# 恢复工作流
dotnet run --project elsa_aot.cs -- resume 12345678-1234-1234-1234-1234567890ab

# 终止工作流
dotnet run --project elsa_aot.cs -- terminate 12345678-1234-1234-1234-1234567890ab

# 查询工作流状态
dotnet run --project elsa_aot.cs -- status 12345678-1234-1234-1234-1234567890ab

# 列出所有运行中的工作流
dotnet run --project elsa_aot.cs -- list running

# 列出工作流实例，使用自定义分页
dotnet run --project elsa_aot.cs -- list 20 1

# 显示版本信息
dotnet run --project elsa_aot.cs -- version
```

### 5. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Elsa.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Elsa 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 Elsa 选项
        builder.Configure<ElsaOptions>(options => {
            options.EnableWorkflowCache = true;
            options.MaxCacheSize = 2000;
            options.DefaultTimeoutMs = 60000;
            options.EnableDetailedLogging = true;
            options.EnablePerformanceMonitoring = true;
        });
        
        // 注册 Elsa 服务
        builder.AddElsaAot();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var elsaOptions = serviceProvider.GetRequiredService<IOptions<ElsaOptions>>().Value;
        Console.WriteLine($"配置信息:");
        Console.WriteLine($"   启用缓存: {elsaOptions.EnableWorkflowCache}");
        Console.WriteLine($"   缓存大小: {elsaOptions.MaxCacheSize}");
        Console.WriteLine($"   默认超时: {elsaOptions.DefaultTimeoutMs} ms");
        Console.WriteLine($"   详细日志: {elsaOptions.EnableDetailedLogging}");
        
        // 使用服务
        var elsaService = serviceProvider.GetRequiredService<IElsaService>();
        var result = await elsaService.StartWorkflowAsync();
        Console.WriteLine($"\n工作流已启动，实例 ID: {result.WorkflowInstanceId}");
    }
}
```

### 6. 扩展开发示例

```csharp
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Elsa.AOT;

// 自定义 Elsa 服务实现
public class CustomElsaService : IElsaService
{
    private readonly ElsaOptions _options;
    private readonly ILogger<CustomElsaService> _logger;
    
    public CustomElsaService(IOptions<ElsaOptions> options, ILogger<CustomElsaService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }
    
    // 实现接口方法...
    public async Task<ElsaCommandResult> StartWorkflowAsync(string? workflowDefinitionId = null, Dictionary<string, object>? inputData = null, int? timeoutMs = null)
    {
        _logger.LogInformation("自定义工作流启动逻辑执行");
        
        // 自定义工作流启动逻辑
        return new ElsaCommandResult
        {
            Success = true,
            CommandType = ElsaCommandType.StartWorkflow,
            Results = new List<string> { "自定义工作流已启动", "这是一个扩展示例" },
            WorkflowInstanceId = Guid.NewGuid().ToString()
        };
    }
    
    // 实现其他接口方法...
    public async Task<ElsaCommandResult> ExecuteCommandAsync(ElsaCommandType commandType, Dictionary<string, string>? parameters = null)
    {
        return new ElsaCommandResult { Success = false, ErrorMessage = "未实现的命令" };
    }
    
    public async Task<ElsaCommandResult> SuspendWorkflowAsync(string workflowInstanceId) { return new ElsaCommandResult(); }
    public async Task<ElsaCommandResult> ResumeWorkflowAsync(string workflowInstanceId) { return new ElsaCommandResult(); }
    public async Task<ElsaCommandResult> TerminateWorkflowAsync(string workflowInstanceId) { return new ElsaCommandResult(); }
    public async Task<ElsaCommandResult> QueryWorkflowStatusAsync(string workflowInstanceId) { return new ElsaCommandResult(); }
    public async Task<ElsaCommandResult> ListWorkflowInstancesAsync(string? statusFilter = null, int pageSize = 10, int pageIndex = 0) { return new ElsaCommandResult(); }
    public async Task<ElsaCommandResult> GetVersionInfoAsync() { return new ElsaCommandResult(); }
}

// 扩展方法
public static class CustomElsaExtensions
{
    public static IServiceCollection AddCustomElsaAot(this IServiceCollection services)
    {
        services.AddLogging();
        services.AddOptions<ElsaOptions>();
        // 注册自定义 Elsa 服务
        services.AddSingleton<IElsaService, CustomElsaService>();
        services.AddSingleton<ElsaAotEngine>();
        return services;
    }
}

// 使用示例
public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Elsa 扩展开发示例");
        Console.WriteLine("=" * 50);
        
        var builder = new ServiceCollection();
        builder.AddCustomElsaAot(); // 使用自定义扩展方法
        
        var serviceProvider = builder.BuildServiceProvider();
        var elsaService = serviceProvider.GetRequiredService<IElsaService>();
        
        // 使用自定义服务
        var result = await elsaService.StartWorkflowAsync();
        Console.WriteLine($"自定义工作流启动结果: {result.Success}");
        foreach (var message in result.Results)
        {
            Console.WriteLine($"   {message}");
        }
    }
}
```

## 总结

以上示例展示了 Elsa 工作流管理技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速入门基本的工作流管理操作
2. 配置高级选项以优化性能
3. 实现完整的工作流生命周期管理
4. 使用命令行工具进行工作流操作
5. 扩展和自定义 Elsa 服务

该系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

### AOT 编译优势

使用 AOT 编译的 Elsa 工作流管理系统具有以下优势：

1. **快速启动**：AOT 编译的应用程序启动时间比 JIT 编译快数倍
2. **更小的内存占用**：优化的内存使用，减少运行时开销
3. **更高的性能**：本地代码执行，减少运行时编译成本
4. **更好的安全性**：减少攻击面，提高系统安全性
5. **部署简单**：无需依赖 .NET 运行时，单文件部署

通过这些示例，您可以快速掌握 Elsa AOT 工作流管理系统的使用，并根据自己的需求进行扩展和定制。
