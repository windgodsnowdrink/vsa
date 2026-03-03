# dotnetscript - 参考文档

## 概述

dotnetscript 是基于 .NET 10 AOT 架构的高性能 DotNetScript 技能，专为 .NET 开发者设计，提供强大的脚本执行和代码编译功能。

## 核心组件

### 1. DotNetScriptService（DotNetScript 服务）
- **位置**: scripts/dotnetscript_aot.cs
- **功能**: DotNetScript 核心服务，负责脚本执行、编译和管理
- **特性**: 
  - 基于 .NET 10 AOT 编译，高性能
  - 支持脚本文件和内联脚本执行
  - 脚本编译和语法检查
  - 脚本文件列表管理
  - 编译缓存清理
  - 详细的错误处理和日志记录

### 2. DotNetScriptAotEngine（DotNetScript AOT 引擎）
- **位置**: scripts/dotnetscript_aot.cs
- **功能**: 管理 DotNetScript 功能调用的引擎，提供简洁的 API 接口
- **特性**: 
  - 简化的 API 调用
  - 统一的错误处理
  - 支持命令行操作
  - 完整的 DotNetScript 功能支持

## 核心接口

### IDotNetScriptService
DotNetScript 服务的核心接口，定义了所有 DotNetScript 操作方法：

| 方法名 | 描述 | 参数 | 返回值 |
|--------|------|------|--------|
| ExecuteCommandAsync | 执行 DotNetScript 命令 | commandType: DotNetScriptCommandType, parameters: Dictionary<string, string>? | Task<DotNetScriptCommandResult> |
| ExecuteScriptAsync | 执行脚本 | scriptContent: string, scriptPath: string?, timeoutMs: int? | Task<DotNetScriptCommandResult> |
| ExecuteScriptFromFileAsync | 从文件执行脚本 | scriptPath: string, timeoutMs: int? | Task<DotNetScriptCommandResult> |
| CompileScriptAsync | 编译脚本 | scriptContent: string, scriptPath: string? | Task<DotNetScriptCommandResult> |
| ListScriptsAsync | 列出脚本文件 | directory: string, recursive: bool | Task<DotNetScriptCommandResult> |
| CleanCacheAsync | 清理编译缓存 | 无 | Task<DotNetScriptCommandResult> |
| GetVersionInfoAsync | 获取版本信息 | 无 | Task<DotNetScriptCommandResult> |

## 数据结构

### DotNetScriptCommandType（DotNetScript 命令类型）
```csharp
public enum DotNetScriptCommandType
{
    ExecuteScript,  // 执行脚本
    CompileScript,  // 编译脚本
    ListScripts,    // 列出脚本
    CleanCache,     // 清理缓存
    VersionInfo     // 版本信息
}
```

### DotNetScriptCommandResult（DotNetScript 命令结果）
```csharp
public class DotNetScriptCommandResult
{
    public bool Success { get; set; }              // 命令是否成功
    public DotNetScriptCommandType CommandType { get; set; } // 命令类型
    public List<string> Results { get; set; }       // 结果数据
    public long ExecutionTimeMs { get; set; }       // 执行时间（毫秒）
    public string? ErrorMessage { get; set; }       // 错误信息
    public string? Output { get; set; }            // 输出内容
}
```

### DotNetScriptOptions（DotNetScript 配置选项）
```csharp
public class DotNetScriptOptions
{
    public int DefaultTimeoutMs { get; set; } = 30000;  // 默认超时时间（毫秒）
    public bool EnableCompilationCache { get; set; } = true;  // 是否启用编译缓存
    public string CacheDirectory { get; set; } = Path.Combine(Path.GetTempPath(), "dotnetscript-aot-cache"); // 缓存目录
    public bool EnableDetailedLogging { get; set; } = false;  // 是否启用详细日志
    public bool EnablePerformanceMonitoring { get; set; } = true;  // 是否启用性能监控
    public string DefaultScriptExtension { get; set; } = ".csx";  // 默认脚本扩展名
    public List<string> SupportedExtensions { get; set; } = new List<string> { ".csx", ".cs" };  // 支持的扩展名列表
}
```

## 配置选项

### DotNetScript 配置（dotnetscript_aot.setting.json）

```json
{
  "DotNetScript": {
    "DefaultTimeoutMs": 30000,                      // 默认超时时间（毫秒）
    "EnableCompilationCache": true,                // 是否启用编译缓存
    "CacheDirectory": "dotnetscript-aot-cache",    // 缓存目录
    "EnableDetailedLogging": false,                // 是否启用详细日志
    "EnablePerformanceMonitoring": true,           // 是否启用性能监控
    "DefaultScriptExtension": ".csx",             // 默认脚本扩展名
    "SupportedExtensions": [".csx", ".cs"]     // 支持的扩展名列表
  }
}
```

## 运行配置（dotnetscript_aot.run.json）

```json
{
  "$schema": "https://dot.net/v1/dotnet.run.schema.json",
  "framework": "net10.0",
  "options": {
    "PublishAot": true,                   // 启用 AOT 编译
    "InvariantGlobalization": true,       // 启用不变全球化
    "EnableCompilationRelaxations": true, // 启用编译松弛
    "PublishReadyToRun": true,            // 启用 ReadyToRun
    "LangVersion": "preview",            // 语言版本
    "Nullable": true,                     // 启用可空引用类型
    "ImplicitUsings": true                // 启用隐式 using
  },
  "dependencies": {
    "Microsoft.Extensions.DependencyInjection": "10.0.0",
    "Microsoft.Extensions.Hosting": "10.0.0",
    "Microsoft.Extensions.Logging": "10.0.0",
    "Microsoft.Extensions.Options": "10.0.0",
    "Microsoft.CodeAnalysis.CSharp.Scripting": "4.10.0",
    "Microsoft.CodeAnalysis.Common": "4.10.0",
    "Microsoft.CodeAnalysis.CSharp": "4.10.0"
  }
}
```

## 性能优化

1. **启用 AOT 编译**：AOT 编译可以提供极致的启动速度和运行性能
2. **异步编程**：使用异步 API 可以提高系统的并发处理能力
3. **合理配置超时时间**：根据脚本复杂度调整超时时间
4. **启用编译缓存**：缓存可以提高重复脚本的执行速度
5. **定期清理缓存**：定期清理编译缓存，释放磁盘空间
6. **日志级别控制**：在生产环境中，将日志级别设置为 Information 或更高，减少日志开销

## 故障排除

### 常见问题

1. **脚本执行失败**
   - 检查脚本语法是否正确
   - 验证脚本中引用的程序集是否可用
   - 查看日志获取详细错误信息
   - 检查脚本执行超时设置

2. **编译失败**
   - 检查脚本语法是否正确
   - 验证脚本中使用的命名空间和类型
   - 查看详细的编译错误信息
   - 确保所有依赖项都已正确引用

3. **命令执行超时**
   - 调整超时时间配置
   - 优化脚本代码，减少执行时间
   - 考虑将复杂脚本拆分为多个简单脚本

4. **服务异常**
   - 查看日志获取详细错误信息
   - 检查配置文件是否正确
   - 重启应用程序

5. **缓存问题**
   - 清理编译缓存（使用 clean 命令）
   - 禁用缓存（临时排查问题）
   - 检查缓存目录权限

## 扩展开发

### 自定义 DotNetScript 服务

1. 实现 IDotNetScriptService 接口
2. 重写需要自定义的方法
3. 注册自定义服务

```csharp
builder.Services.AddSingleton<IDotNetScriptService, CustomDotNetScriptService>();
```

### 扩展命令类型

1. 在 DotNetScriptCommandType 枚举中添加新的命令类型
2. 在 ExecuteCommandAsync 方法中添加相应的处理逻辑
3. 添加对应的便捷方法

## AOT 编译注意事项

1. **依赖项**：确保所有依赖项都支持 AOT 编译
2. **反射**：避免在运行时使用反射，或使用 AOT 友好的反射方式
3. **动态类型**：谨慎使用 dynamic 类型，可能会影响 AOT 编译效果
4. **配置文件**：AOT 编译后，配置文件路径可能需要调整
5. **测试**：在 AOT 模式下进行充分测试，确保所有功能正常工作
6. **Microsoft.CodeAnalysis 兼容性**：Microsoft.CodeAnalysis 4.10.0 版本已验证支持 AOT 编译

## 命令行工具

该技能提供了命令行工具，支持以下命令：

- `run <script> [options]`: 运行脚本文件或内联脚本
- `compile <script> [options]`: 编译脚本而不执行
- `list [directory] [options]`: 列出目录中的脚本文件
- `clean`: 清理编译缓存
- `version`: 显示版本信息
- `help, --help, -h`: 显示帮助信息

使用示例：
```bash
dotnetscript_aot.exe run script.csx              # 运行脚本文件
dotnetscript_aot.exe run "Console.WriteLine(123);"  # 运行内联脚本
dotnetscript_aot.exe compile script.csx         # 编译脚本
dotnetscript_aot.exe list scripts/ -r           # 递归列出脚本
dotnetscript_aot.exe clean                      # 清理缓存
dotnetscript_aot.exe version                   # 显示版本
dotnetscript_aot.exe run script.csx --timeout 10000  # 运行脚本并设置超时时间
```

## 版本历史

| 版本 | 日期 | 描述 |
|------|------|------|
| 1.0.0 | 2026-01-03 | 初始版本，基于 .NET 10 AOT 架构 |

## 许可证

MIT License

