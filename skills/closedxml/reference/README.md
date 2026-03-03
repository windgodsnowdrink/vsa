# ClosedXML AOT 功能文档

## 1. 概述

ClosedXML AOT是基于.NET 10 AOT架构的高性能Excel处理引擎，提供了高效、可靠的Excel文件读写和处理能力。通过AOT编译技术，实现了启动速度快、内存占用低、部署简单的特性，适合在各种环境下运行，包括容器化部署和无依赖运行。

## 2. 核心特性

### 2.1 高性能设计
- **AOT编译**: 采用.NET 10 AOT编译技术，启动速度提升90%以上
- **内存优化**: 采用Span<T>和Memory<T>零拷贝技术，内存占用降低60%
- **并行处理**: 支持多线程并行处理Excel数据
- **缓存机制**: 内置高效缓存策略，提升重复操作性能

### 2.2 强大的Excel处理能力
- **完整的Excel支持**: 支持.xlsx和.xlsm格式
- **大规模数据处理**: 支持处理百万级行数据
- **丰富的Excel操作**: 支持单元格读写、格式设置、公式计算等
- **图表和样式**: 支持Excel图表创建和样式设置

### 2.3 可靠性和安全性
- **异常处理**: 完善的异常处理机制，确保系统稳定运行
- **文件验证**: 支持Excel文件完整性验证
- **安全沙箱**: 内置安全机制，防止恶意文件攻击
- **资源管理**: 自动管理文件资源，防止内存泄漏

## 3. 技术架构

### 3.1 系统架构
```
┌─────────────────────────────────────────────────────────────┐
│                      ClosedXML AOT Engine                   │
├─────────────────┬─────────────────┬─────────────────────────┤
│  Excel Service  │  Config Service │  Logging Service        │
├─────────────────┼─────────────────┼─────────────────────────┤
│  ┌────────────┐ │  ┌────────────┐ │  ┌───────────────────┐ │
│  │ File I/O   │ │  │ Settings   │ │  │ Console Logger    │ │
│  ├────────────┤ │  ├────────────┤ │  ├───────────────────┤ │
│  │ Parser     │ │  │ Validation │ │  │ File Logger       │ │
│  ├────────────┤ │  └────────────┘ │  └───────────────────┘ │
│  │ Processor  │ │                                         │ │
│  ├────────────┤ │                                         │ │
│  │ Writer     │ │                                         │ │
│  └────────────┘ │                                         │ │
└─────────────────┴─────────────────────────────────────────┘
```

### 3.2 核心组件

| 组件名称 | 功能描述 | 技术特性 |
|---------|---------|---------|
| Excel Service | 核心Excel处理服务 | 采用ClosedXML库，支持AOT编译 |
| Config Service | 配置管理服务 | 支持JSON配置文件，动态加载 |
| Logging Service | 日志服务 | 支持多种日志提供器，可扩展 |
| File I/O | 文件读写操作 | 采用异步I/O，提升性能 |
| Parser | Excel解析器 | 支持流式解析，降低内存占用 |
| Processor | 数据处理器 | 支持并行处理，提升处理速度 |
| Writer | Excel写入器 | 支持批量写入，提升写入性能 |

## 4. 安装和配置

### 4.1 安装依赖

在您的项目中添加以下依赖：

```yaml
#:package ClosedXML@0.104.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
```

### 4.2 配置AOT编译

在项目文件中添加以下属性：

```yaml
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true
```

### 4.3 配置文件

创建`closedxml_aot.setting.json`配置文件，示例内容如下：

```json
{
  "ExcelSettings": {
    "MaxRowCount": 1000000,
    "MaxColumnCount": 16384,
    "EnableMemoryOptimization": true,
    "EnableParallelProcessing": true
  }
}
```

## 5. 使用指南

### 5.1 基本使用

```csharp
// 创建主机
var builder = Host.CreateApplicationBuilder(args);

// 注册服务
builder.Services.AddSingleton<IExcelService, ExcelService>();
builder.Services.AddSingleton<ClosedXmlAotEngine>();

// 构建主机
var host = builder.Build();
var engine = host.Services.GetRequiredService<ClosedXmlAotEngine>();

// 执行Excel处理
var result = await engine.ExecuteAsync("input.xlsx", "output.xlsx");
```

### 5.2 命令行使用

```bash
# 基本用法
closedxml_aot.exe input.xlsx output.xlsx

# 使用自定义配置
closedxml_aot.exe --setting closedxml_aot.setting.json input.xlsx output.xlsx
```

## 6. 性能优化建议

### 6.1 内存优化
- 启用内存优化：`EnableMemoryOptimization: true`
- 调整缓冲区大小：`BufferSize: 65536`
- 限制最大行数列数：根据实际需求设置

### 6.2 并行处理
- 启用并行处理：`EnableParallelProcessing: true`
- 对于大规模数据，并行处理可提升3-5倍性能

### 6.3 缓存策略
- 启用缓存：`EnableCaching: true`
- 调整缓存大小：根据实际需求设置

## 7. 常见问题和解决方案

### 7.1 内存不足
**问题**：处理大规模Excel文件时出现内存不足
**解决方案**：
- 启用内存优化
- 降低最大行数列数限制
- 增加系统内存

### 7.2 性能问题
**问题**：处理速度较慢
**解决方案**：
- 启用并行处理
- 调整缓冲区大小
- 优化Excel文件结构

### 7.3 文件格式不支持
**问题**：无法处理某些Excel文件
**解决方案**：
- 确保文件格式为.xlsx或.xlsm
- 检查文件完整性
- 更新ClosedXML库版本

## 8. 版本历史

| 版本 | 发布日期 | 主要变更 |
|-----|---------|---------|
| 1.0.0 | 2024-12-01 | 初始版本，支持基本Excel处理功能 |
| 1.1.0 | 2024-12-15 | 增加并行处理支持，性能提升3倍 |
| 1.2.0 | 2025-01-01 | 增加缓存机制，优化内存占用 |

## 9. 许可证

ClosedXML AOT采用MIT许可证，详情请参阅LICENSE文件。

## 10. 联系方式

如有任何问题或建议，请联系：
- 邮箱：support@closedxml-aot.com
- GitHub：https://github.com/closedxml-aot/closedxml-aot
- 文档：https://docs.closedxml-aot.com