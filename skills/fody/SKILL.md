# Fody Agent Skill - Fody 技能

## 技能概述

基于 .NET 10 AOT 编译的高性能 Fody 技能，为 .NET 开发者提供强大的代码织入功能。

## 快速开始指南

### 安装依赖

在主应用程序的 runfile 中添加以下依赖：

`yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Mono.Cecil@0.11.5
`

### 注册服务

在主应用程序中注册 Fody 服务：

`csharp
// 注册 Fody 服务
builder.Services.Configure<FodyOptions>(builder.Configuration.GetSection("Fody"));
builder.Services.AddSingleton<IFodyService, FodyService>();
builder.Services.AddSingleton<FodyAotEngine>();
`

### 使用示例

`csharp
// 获取 Fody 服务
var fodyService = serviceProvider.GetRequiredService<IFodyService>();

// 织入程序集
var result = await fodyService.WeaveAssemblyAsync("MyAssembly.dll", "Output.dll");
Console.WriteLine($"织入结果: {(result.Success ? "成功" : "失败")}");
Console.WriteLine($"执行时间: {result.ExecutionTimeMs} ms");

// 列出可用的织入器
var weaversResult = await fodyService.ListWeaversAsync();
Console.WriteLine("\n可用的织入器:");
foreach (var item in weaversResult.Results)
{
    Console.WriteLine($"- {item}");
}
`

## 导航地图

`
fody/
????? index.yaml                   # 元数据索引描述
????? SKILL.md                    # 技能入口点（当前文件）
????? reference/                  # 参考文件
??  ????? README.md              # 完整功能描述
??  ????? examples.md            # 使用示例
????? scripts/                    # 脚本和工具
    ????? fody_aot.cs             # Fody AOT 核心实现
    ????? fody_aot.run.json       # 运行配置
    ????? fody_aot.setting.json   # 设置文件
    ????? fody_integration.cs     # Fody 集成
`

## 主要功能

1. **程序集织入**：支持对 .NET 程序集进行织入操作，添加额外的功能
2. **织入器管理**：提供可用织入器的列表，包括 PropertyChanged、MethodTimer、NullGuard 等
3. **AOT 编译优化**：使用 .NET 10 的 AOT 编译功能，减少启动时间和内存占用
4. **详细的执行结果**：返回详细的织入结果，包括执行时间、错误信息等
5. **易用的命令行接口**：支持多种命令别名，方便使用
6. **依赖注入**：基于 Microsoft.Extensions.DependencyInjection 的服务管理
7. **异步编程**：使用 Task-based 异步模式，支持高并发操作
8. **详细的错误处理**：完善的错误捕获和日志记录

## 扩展说明

此技能提供了完整的 Fody 解决方案，您可以根据需要进行扩展：

1. **自定义织入器**：实现自定义的织入器，添加特定的功能
2. **扩展服务**：继承 FodyService，添加新的织入功能
3. **与其他系统集成**：将 Fody 与其他构建工具或系统集成
4. **性能优化**：针对特定场景优化织入性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理 Fody 服务
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：正确处理织入过程中的错误情况
4. **日志记录**：添加适当的日志记录，便于调试和监控
5. **性能监控**：监控织入性能，优化织入过程
6. **AOT 优化**：利用 AOT 编译提高 Fody 引擎的性能
7. **模块化设计**：将织入逻辑模块化，便于维护和扩展
