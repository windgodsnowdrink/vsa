# iconpython - 参考文档

## 概述

IconPython 是一个基于 .NET 10 的高性能 Python 执行系统，专为 .NET 开发者设计，提供了强大的 Python 执行功能，包括脚本执行、表达式计算、REPL 环境、模块管理等。

## 核心组件

### 1. PythonService (IconPython 服务)
- **位置**: scripts/iconpython_integration.cs, scripts/iconpython_aot.cs
- **功能**: 核心 Python 执行服务，处理 Python 脚本执行、表达式计算等业务逻辑
- **特性**: 
  - 核心功能实现
  - 性能优化
  - 错误处理
  - 日志记录
  - 支持异步编程

### 2. PythonSettings (IconPython 配置)
- **位置**: scripts/iconpython_aot.cs
- **功能**: IconPython 服务的配置选项
- **特性**: 
  - 基础路径配置
  - 超时设置
  - 调试和跟踪选项
  - 模块搜索路径配置
  - 全局变量配置

### 3. IconPython AOT 核心引擎
- **位置**: scripts/iconpython_aot.cs, scripts/iconpython_aot.setting.json, scripts/iconpython_aot.run.json
- **功能**: 基于 AOT 编译的高性能 Python 执行引擎
- **特性**: 
  - 支持 Python 脚本执行
  - 支持 Python 表达式计算
  - 支持 Python REPL 环境
  - 支持基准测试
  - 支持模块管理
  - 命令行界面

## 使用示例

### 基本使用

```csharp
// 获取 IconPython 服务
var pythonService = serviceProvider.GetRequiredService<PythonService>();

// 执行 Python 脚本
var result = await pythonService.RunScriptAsync("test.py");
Console.WriteLine($"执行状态: {(result.Success ? "成功" : "失败"}");
Console.WriteLine($"输出: {result.Output}");

// 计算 Python 表达式
var evalResult = await pythonService.EvaluateExpressionAsync("1 + 1");
Console.WriteLine($"计算结果: {evalResult}");

// 导入 Python 模块
var module = await pythonService.ImportModuleAsync("math");
Console.WriteLine($"模块导入: {(module != null ? "成功" : "失败"}");
```

### 高级配置

```csharp
// 配置 IconPython 服务
var settings = new PythonSettings {
    BasePath = Environment.CurrentDirectory,
    Timeout = TimeSpan.FromSeconds(30),
    EnableDebug = true,
    EnableTracing = false,
    ModulePaths = new List<string> {
        Path.Combine(Environment.CurrentDirectory, "modules"),
        Path.Combine(Environment.CurrentDirectory, "scripts")
    },
    GlobalVariables = new Dictionary<string, object> {
        { "__name__", "__main__" },
        { "__file__", "iconpython_aot.cs" }
    }
};

builder.Services.Configure<PythonSettings>(options => {
    options.BasePath = settings.BasePath;
    options.Timeout = settings.Timeout;
    options.EnableDebug = settings.EnableDebug;
    options.EnableTracing = settings.EnableTracing;
    options.ModulePaths = settings.ModulePaths;
    options.GlobalVariables = settings.GlobalVariables;
});
```

## 配置选项

### PythonSettings 配置

```json
{
  "PythonSettings": {
    "BasePath": "D:\\Trae\\vsa\\skills\\iconpython\\scripts", // 基础路径
    "Timeout": "00:00:30",        // 超时时间
    "EnableDebug": false,          // 启用调试
    "EnableTracing": false,        // 启用跟踪
    "ModulePaths": [              // 模块搜索路径
      "D:\\Trae\\vsa\\skills\\iconpython\\modules",
      "D:\\Trae\\vsa\\skills\\iconpython\\scripts"
    ],
    "GlobalVariables": {          // 全局变量
      "__name__": "__main__",
      "__file__": "iconpython_aot.cs"
    }
  }
}
```

## 性能优化

1. **缓存使用**: 启用缓存以提高性能
2. **异步编程**: 使用异步 API 避免阻塞
3. **批处理**: 批量处理以提高效率
4. **资源管理**: 合理管理 Python 引擎资源
5. **AOT 编译**: 使用 AOT 编译提高启动速度和运行性能

## 故障排除

### 常见问题

1. **Python 执行失败**
   - 检查脚本文件路径
   - 验证 Python 语法
   - 检查日志信息

2. **性能问题**
   - 启用缓存
   - 优化 Python 代码
   - 增加资源限制

3. **模块导入失败**
   - 检查模块搜索路径
   - 验证模块是否存在
   - 检查模块依赖

## 扩展开发

### 添加自定义功能

```csharp
// 实现自定义 Python 服务
public class CustomPythonService : PythonService
{
    public CustomPythonService(ILogger<PythonService> logger, IOptions<PythonSettings> options) 
        : base(logger, options)
    {
    }
    
    // 添加自定义方法
    public async Task<object> CustomPythonFunctionAsync(string code)
    {
        // 实现自定义 Python 执行逻辑
        return await EvaluateExpressionAsync(code);
    }
}

// 注册自定义服务
builder.Services.AddSingleton<CustomPythonService>();
```

### 扩展 Python 模块

```csharp
// 创建自定义 Python 模块
builder.Services.Configure<PythonSettings>(options => {
    // 添加自定义模块路径
    options.ModulePaths.Add(Path.Combine(Environment.CurrentDirectory, "custom_modules"));
    
    // 添加自定义全局变量
    options.GlobalVariables["custom_function"] = new Func<int, int, int>((a, b) => a + b);
});
```

## IconPython AOT 核心引擎

### IconPython AOT 命令行使用

```bash
# 运行 Python 脚本
iconpython_aot run test.py

# 计算 Python 表达式
iconpython_aot eval 1 + 1

# 启动 REPL 环境
iconpython_aot repl

# 运行基准测试
iconpython_aot benchmark 1000 1 + 1

# 运行 Python 模块
iconpython_aot module math

# 导入 Python 模块
iconpython_aot import math

# 显示配置
iconpython_aot config

# 显示帮助
iconpython_aot help
```

### IconPython AOT 编译配置

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
  <TrimMode>partial</TrimMode>
  <PublishReadyToRun>true</PublishReadyToRun>
  <PublishSingleFile>true</PublishSingleFile>
  <SelfContained>true</SelfContained>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
</PropertyGroup>
```

### IconPython AOT 性能特性

1. **启动速度快**: AOT 编译减少了启动时间
2. **运行性能高**: 原生代码执行提高了运行性能
3. **内存使用少**: 优化的内存管理减少了内存占用
4. **部署简单**: 单文件发布便于部署

IconPython AOT 核心引擎为 .NET 开发者提供了一种高性能、可靠的 Python 执行方案，适合各种 Python 执行场景，特别是需要高性能和低延迟的应用。
