# dotnetscript Agent Skill - DotNetScript 技能

## 技能概述

基于 .NET 10 构建的高性能 DotNetScript 技能，为 .NET 开发者提供强大的脚本执行和代码编译功能支持。该技能采用 AOT（预编译）技术，提供极致的性能表现和启动速度，适用于各种脚本执行场景。

## 快速入门指南

### 安装依赖

在主应用程序的运行文件中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.CodeAnalysis.CSharp.Scripting@4.10.0
#:package Microsoft.CodeAnalysis.Common@4.10.0
#:package Microsoft.CodeAnalysis.CSharp@4.10.0
```

### 注册服务

在主应用程序中注册 DotNetScript 服务：

```csharp
// 配置 DotNetScript 选项
builder.Configuration.AddJsonFile("dotnetscript_aot.setting.json", optional: true);
builder.Services.Configure<DotNetScript.AOT.DotNetScriptOptions>(builder.Configuration.GetSection("DotNetScript"));

// 注册 DotNetScript 服务
builder.Services.AddDotNetScript();
```

### 使用示例

```csharp
// 获取 DotNetScript 引擎实例
var engine = serviceProvider.GetRequiredService<DotNetScript.AOT.DotNetScriptAotEngine>();

// 执行内联脚本
var runResult = await engine.ExecuteCommandLineAsync(new string[] { "run", "Console.WriteLine(\"Hello, DotNetScript!\")" });
Console.WriteLine($"Script execution result: {runResult}");

// 执行脚本文件
var fileResult = await engine.ExecuteCommandLineAsync(new string[] { "run", "script.csx" });
Console.WriteLine($"Script file execution result: {fileResult}");
```

## 导航地图

```
dotnetscript/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── dotnetscript_aot.cs    # DotNetScript 核心实现（AOT）
    ├── dotnetscript_aot.run.json  # 运行配置
    └── dotnetscript_aot.setting.json # 应用设置
```

## 主要功能

1. **高性能 AOT 编译**：基于 .NET 10 AOT 技术，提供极致性能和启动速度
2. **脚本执行**：支持脚本文件和内联脚本的执行
3. **脚本编译**：编译脚本并进行语法检查，不执行代码
4. **脚本管理**：列出指定目录下的脚本文件
5. **缓存管理**：清理编译缓存，释放磁盘空间
6. **版本信息**：获取 DotNetScript 引擎的版本信息
7. **超时控制**：支持设置脚本执行的超时时间

## 扩展说明

该技能提供了完整的 DotNetScript 解决方案，您可以根据需要进行扩展：

1. **自定义脚本服务**：实现 IDotNetScriptService 接口，自定义脚本执行逻辑
2. **扩展命令类型**：添加新的脚本命令类型和处理逻辑
3. **集成其他系统**：与其他系统和框架集成，实现更复杂的脚本功能
4. **性能优化**：针对特定场景优化脚本编译和执行性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务，提高代码可测试性和可维护性
2. **异步编程**：优先使用异步 API，避免阻塞主线程
3. **合理配置**：根据实际需求调整脚本执行超时时间
4. **错误处理**：妥善处理脚本执行中的各种异常情况
5. **日志记录**：适当添加日志记录，便于调试和监控
6. **性能监控**：启用性能监控，实时了解脚本执行性能
7. **缓存管理**：定期清理编译缓存，释放磁盘空间

## AOT 编译说明

该技能支持 .NET 10 AOT 编译，通过预编译将应用程序编译为本地机器代码，提供以下优势：

- **极致的启动速度**：无需 JIT 编译，直接运行本地代码
- **减少内存占用**：更小的运行时占用
- **提高安全性**：减少可攻击面，提高应用程序安全性
- **跨平台支持**：支持多种操作系统和架构

## 命令行工具

该技能提供了命令行工具，支持以下命令：

- `run <script> [options]`: 运行脚本文件或内联脚本
- `compile <script> [options]`: 编译脚本而不执行
- `list [directory] [options]`: 列出目录中的脚本文件
- `clean`: 清理编译缓存
- `version`: 显示版本信息
- `help, --help, -h`: 显示帮助信息

使用示例：
```
dotnetscript_aot.exe run script.csx              Run a script file
dotnetscript_aot.exe run "Console.WriteLine(123);"  Run inline script
dotnetscript_aot.exe compile script.csx         Compile a script
dotnetscript_aot.exe list scripts/ -r           List all scripts recursively
dotnetscript_aot.exe clean                      Clean cache
dotnetscript_aot.exe version                   Show version
```
