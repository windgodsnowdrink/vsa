# elsa - 参考文档

## 概述

elsa 是基于 .NET 10 的高性能工作流管理系统，专为 .NET 开发者设计，支持 AOT 编译以实现极致性能。

## 核心组件

### 1. IElsaService（Elsa 服务）
- **位置**: scripts/elsa_aot.cs
- **功能**: 核心业务逻辑处理
- **特性**: 
  - 工作流完整生命周期管理
  - 高性能优化设计
  - 完善的错误处理
  - 详细的日志记录
  - AOT 编译支持

### 2. ElsaAotEngine（Elsa AOT 引擎）
- **位置**: scripts/elsa_aot.cs
- **功能**: 命令行界面和引擎管理
- **特性**: 
  - 支持多种命令行操作
  - 高性能启动
  - 完善的参数处理

## 使用示例

### 基本用法

```csharp
// 获取 Elsa 服务
var elsaService = serviceProvider.GetRequiredService<IElsaService>();

// 启动工作流
var result = await elsaService.StartWorkflowAsync();
Console.WriteLine($"工作流已启动，实例 ID: {result.WorkflowInstanceId}");
```

### 高级配置

```csharp
// 配置 Elsa 选项
builder.Services.Configure<ElsaOptions>(options => {
    options.EnableWorkflowCache = true;
    options.MaxCacheSize = 1000;
    options.DefaultTimeoutMs = 30000;
    options.EnableDetailedLogging = true;
});
```

## 配置选项

### Elsa 配置

```json
{
  "Elsa": {
    "DefaultTimeoutMs": 30000,          // 默认超时时间（毫秒）
    "EnableWorkflowCache": true,         // 启用工作流缓存
    "MaxCacheSize": 1000,                // 最大缓存大小
    "EnableDetailedLogging": false,      // 启用详细日志
    "EnablePerformanceMonitoring": true,  // 启用性能监控
    "StorageType": "Memory",            // 存储类型
    "StorageConnectionString": "MemoryStore" // 存储连接字符串
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Elsa.AOT": "Information"
    }
  }
}
```

## 性能优化

1. **AOT 编译**: 使用 `PublishAot=true` 获得最佳性能
2. **缓存使用**: 启用工作流缓存以提高性能
3. **异步编程**: 使用异步 API 避免阻塞
4. **内存优化**: 配置适当的缓存大小
5. **并发支持**: 优化的锁机制和并发处理

## 故障排除

### 常见问题

1. **工作流启动失败**
   - 检查配置文件中的超时设置
   - 查看日志以获取详细错误信息
   - 确保服务已正确注册

2. **性能问题**
   - 启用工作流缓存
   - 优化存储连接
   - 考虑使用 AOT 编译

3. **命令行参数错误**
   - 使用 `--help` 查看可用命令
   - 检查参数格式是否正确

## 扩展开发

### 添加自定义功能

```csharp
// 实现自定义 Elsa 服务
public class CustomElsaService : IElsaService
{
    // 实现 IElsaService 接口的方法
    public async Task<ElsaCommandResult> StartWorkflowAsync(string? workflowDefinitionId = null, Dictionary<string, object>? inputData = null, int? timeoutMs = null)
    {
        // 自定义工作流启动逻辑
        return new ElsaCommandResult
        {
            Success = true,
            CommandType = ElsaCommandType.StartWorkflow,
            Results = new List<string> { "自定义工作流已启动" }
        };
    }
    
    // 实现其他接口方法...
}

// 注册自定义服务
builder.Services.AddSingleton<IElsaService, CustomElsaService>();
```

## AOT 编译说明

### 编译选项

```csharp
#:property PublishAot=true              // 启用 AOT 编译
#:property InvariantGlobalization=true  // 使用不变全球化模式
#:property EnableCompilationRelaxations=true // 启用编译优化
#:property PublishReadyToRun=true       // 启用 ReadyToRun 编译
```

### AOT 优势

1. **快速启动**: 减少应用程序启动时间
2. **更小的内存占用**: 优化的内存使用
3. **更高的性能**: 本地代码执行，减少运行时开销
4. **更好的安全性**: 减少攻击面，提高安全性

### AOT 注意事项

1. **反射使用**: 避免在运行时使用反射，或确保反射目标已在编译时已知
2. **动态代码生成**: 避免使用动态代码生成，如 `System.Reflection.Emit`
3. **序列化**: 确保所有需要序列化的类型都已正确配置
4. **配置文件**: 确保所有配置文件都能在 AOT 编译后正确加载

## 命令行参考

### 可用命令

| 命令       | 描述                 | 参数                          |
|------------|----------------------|-------------------------------|
| start      | 启动工作流           | [definitionId] (可选)         |
| suspend    | 暂停工作流           | instanceId (必需)             |
| resume     | 恢复工作流           | instanceId (必需)             |
| terminate  | 终止工作流           | instanceId (必需)             |
| status     | 查询工作流状态       | instanceId (必需)             |
| list       | 列出工作流实例       | [status] [pageSize] [pageIndex] (可选) |
| version    | 显示版本信息         | 无                            |
| help       | 显示帮助信息         | 无                            |

### 命令示例

```bash
# 启动默认工作流
dotnet run --project elsa_aot.cs -- start

# 启动指定定义的工作流
dotnet run --project elsa_aot.cs -- start my-workflow-definition

# 暂停工作流
dotnet run --project elsa_aot.cs -- suspend 12345678-1234-1234-1234-1234567890ab

# 查询工作流状态
dotnet run --project elsa_aot.cs -- status 12345678-1234-1234-1234-1234567890ab

# 列出所有运行中的工作流
dotnet run --project elsa_aot.cs -- list running

# 列出工作流实例，使用自定义分页
dotnet run --project elsa_aot.cs -- list 20 1

# 显示版本信息
dotnet run --project elsa_aot.cs -- version

# 显示帮助信息
dotnet run --project elsa_aot.cs -- help
```

