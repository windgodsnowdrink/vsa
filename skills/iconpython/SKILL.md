# iconpython Agent Skill - IconPython 技能

## 技能概述

基于 .NET 10 的高性能 IconPython 技能，为 .NET 开发者提供强大的 Python 功能，包括 Python 脚本执行、表达式计算、REPL 环境、基准测试等功能。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package IronPython@3.4.1
#:package Microsoft.Scripting@1.3.4
```

### 注册服务

在您的主应用程序中注册 IconPython 服务：

```csharp
// 注册 IconPython 服务
var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton<PythonService>();

var app = builder.Build();
```

### 使用示例

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
```

## 导航地图

```
iconpython/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
├── scripts/                    # 脚本和工具
│   ├── iconpython_integration.cs     # IconPython 集成实现
│   ├── iconpython_integration.run.json  # IconPython 集成运行配置
│   ├── iconpython_integration.setting.json  # IconPython 集成设置文件
│   ├── iconpython_aot.cs     # IconPython AOT 核心引擎
│   ├── iconpython_aot.run.json  # IconPython AOT 运行配置
│   └── iconpython_aot.setting.json  # IconPython AOT 设置文件
```

## 主要功能

1. **Python 脚本执行**: 支持执行 Python 脚本文件
2. **Python 表达式计算**: 支持计算 Python 表达式
3. **Python REPL 环境**: 提供交互式 Python 环境
4. **Python 模块管理**: 支持导入和使用 Python 模块
5. **基准测试**: 支持 Python 代码性能基准测试
6. **IconPython AOT 核心引擎**: 基于 AOT 编译的高性能 Python 引擎
7. **高性能设计**: 优化的性能实现
8. **易于使用的 API**: 简单直观的 API 设计
9. **可扩展架构**: 支持自定义扩展

## IconPython AOT 架构

### IconPython AOT 核心功能

1. **Python 脚本执行**: 支持执行 Python 脚本文件
2. **Python 表达式计算**: 支持计算 Python 表达式
3. **Python REPL 环境**: 提供交互式 Python 环境
4. **基准测试**: 支持 Python 代码性能基准测试
5. **模块管理**: 支持导入和使用 Python 模块
6. **详细日志**: 支持详细的执行日志
7. **命令行界面**: 提供友好的命令行界面，支持命令别名

### IconPython AOT 配置文件

IconPython AOT 架构包含以下配置文件：

- **iconpython_aot.cs**: 核心 IconPython AOT 引擎，实现了完整的 Python 执行功能
- **iconpython_aot.setting.json**: AOT 编译配置，包含依赖项和运行时选项
- **iconpython_aot.run.json**: 运行环境配置，包含不同命令的启动设置

### IconPython AOT 使用示例

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

## 扩展说明

此技能提供完整的 IconPython 解决方案，您可以根据需要进行扩展：

1. **自定义 Python 服务**: 实现自定义的 Python 服务，支持特定的功能
2. **扩展 Python 模块**: 添加自定义的 Python 模块
3. **与其他系统集成**: 与其他系统集成，如数据库、消息队列等
4. **性能优化**: 针对特定场景优化性能

## 最佳实践

1. **依赖注入**: 使用依赖注入管理服务
2. **异步编程**: 优先使用异步 API，避免阻塞
3. **错误处理**: 正确处理异常情况
4. **日志记录**: 添加适当的日志记录
5. **性能监控**: 监控关键性能指标
6. **模块管理**: 合理组织和管理 Python 模块
7. **内存管理**: 注意 Python 引擎的内存使用
8. **超时设置**: 为 Python 执行设置合理的超时时间
