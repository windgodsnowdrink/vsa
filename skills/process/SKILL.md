# process Agent Skill - 进程管理技能

## 技能概述

基于.NET 10的高性能进程管理技能，为.NET开发者提供强大的进程管理功能，旨在简化进程操作，提高应用程序的可靠性和性能。

## 快速开始指南

### 安装依赖项

在主应用程序的runfile中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Diagnostics.Process@10.0.0
#:package System.Threading.Tasks.Dataflow@10.0.0
#:package System.IO.Pipelines@10.0.0
#:package System.Threading.Channels@10.0.0
```

### 注册服务

在主应用程序中注册进程管理服务：

```csharp
// 注册进程管理服务
builder.Services.AddProcessManagement();
builder.Services.Configure<ProcessManagementOptions>(options =>
{
    options.EnableProcessPooling = true;
    options.MaxProcesses = 10;
    options.ProcessStartTimeout = TimeSpan.FromSeconds(30);
    options.ProcessIdleTimeout = TimeSpan.FromMinutes(5);
    options.EnableProcessMonitoring = true;
});
```

### 使用示例

```csharp
// 获取进程管理服务
var processManager = serviceProvider.GetRequiredService<IProcessManager>();
var processPool = serviceProvider.GetRequiredService<IProcessPool>();

// 启动单个进程
var processInfo = new ProcessStartInfo
{
    FileName = "dotnet",
    Arguments = "myapp.dll",
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    UseShellExecute = false,
    CreateNoWindow = true
};

using (var process = await processManager.StartProcessAsync(processInfo))
{
    // 读取进程输出
    var output = await process.StandardOutput.ReadToEndAsync();
    Console.WriteLine($"Process output: {output}");
    
    // 等待进程完成
    await process.WaitForExitAsync();
    Console.WriteLine($"Process exited with code: {process.ExitCode}");
}

// 使用进程池
using (var pooledProcess = await processPool.GetProcessAsync())
{
    // 执行任务
    var result = await pooledProcess.ExecuteAsync("echo", "Hello from pooled process");
    Console.WriteLine($"Pooled process result: {result}");
}
```

## 导航地图

```
process/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── processx_integration.cs     # 进程管理核心实现
    ├── processx_integration.run.json  # 运行配置
    └── processx_integration.setting.json  # 设置文件
```

## 主要功能

1. **进程启动和管理**：支持启动、监控和管理进程
2. **进程池化**：提供进程池功能，减少进程启动开销
3. **进程监控**：实时监控进程状态和资源使用情况
4. **进程间通信**：支持进程间的安全通信
5. **进程生命周期管理**：自动化管理进程的创建、使用和销毁
6. **资源监控和限制**：监控和限制进程的资源使用
7. **高性能设计**：优化的性能实现，支持高并发场景
8. **易于使用的API**：简单直观的API设计
9. **可扩展架构**：支持自定义扩展

## 扩展说明

本技能提供了完整的进程管理解决方案，您可以根据需要进行扩展：

1. **自定义实现**：实现IProcessManager接口
2. **扩展功能**：添加新的进程管理功能
3. **与其他系统集成**：与其他系统集成
4. **性能优化**：针对特定场景优化性能

## AOT编译配置

### 项目文件配置

```xml
<Project Sdk="Microsoft.NET.Sdk">
  
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <PublishAot>true</PublishAot>
    <TrimMode>partial</TrimMode>
    <ReadyToRun>true</ReadyToRun>
    <TieredCompilation>true</TieredCompilation>
    <Optimize>true</Optimize>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Options" Version="10.0.0" />
    <PackageReference Include="System.Diagnostics.Process" Version="10.0.0" />
    <PackageReference Include="System.Threading.Tasks.Dataflow" Version="10.0.0" />
    <PackageReference Include="System.IO.Pipelines" Version="10.0.0" />
    <PackageReference Include="System.Threading.Channels" Version="10.0.0" />
  </ItemGroup>
  
</Project>
```

### 部署选项

1. **AOT编译部署**：使用`dotnet publish -c Release -r win-x64 --self-contained true -p:PublishAot=true`命令发布
2. **容器化部署**：支持Docker容器部署
3. **跨平台部署**：支持Windows、Linux、macOS平台

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步API避免阻塞
3. **错误处理**：正确处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **资源管理**：正确管理进程资源，避免泄漏
7. **进程池使用**：对于频繁启动的进程，使用进程池
8. **超时设置**：为进程操作设置合理的超时时间
9. **安全考虑**：注意进程权限和安全问题

## 故障排除

### 常见问题

1. **进程启动失败**
   - 检查可执行文件路径是否正确
   - 检查权限是否足够
   - 检查依赖项是否存在

2. **进程卡住**
   - 检查是否有死锁
   - 检查输入/输出流是否正确处理
   - 确保设置了适当的超时时间

3. **资源使用过高**
   - 检查进程池大小是否合理
   - 监控进程的内存和CPU使用
   - 考虑使用资源限制

4. **AOT编译错误**
   - 确保代码符合AOT编译要求
   - 避免使用反射和动态代码
   - 检查依赖项是否支持AOT
