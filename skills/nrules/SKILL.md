# NRules Agent Skill - NRules 技能

## 技能概述

NRules 技能是一个基于 .NET 10 的规则引擎集成解决方案，提供了完整的规则定义、执行和管理功能。该技能采用 AOT（Ahead-of-Time）编译架构，确保高性能的规则执行和低内存占用。

### 主要功能

- **规则引擎集成**：提供完整的 NRules 规则引擎功能
- **AOT 编译支持**：优化运行时性能和内存使用
- **依赖注入**：无缝集成到 .NET 依赖注入系统
- **配置管理**：通过 Options 模式管理规则引擎配置
- **日志集成**：与 Microsoft.Extensions.Logging 集成
- **高性能设计**：支持规则缓存、批处理和内存优化

### 应用场景

- **业务规则管理**：将复杂的业务规则从代码中分离出来
- **决策引擎**：基于规则的自动化决策系统
- **工作流引擎**：基于规则的工作流控制和状态管理
- **事件处理**：基于规则的事件处理和响应
- **数据验证**：复杂的数据验证规则

## 快速入门

### 安装依赖

```bash
# 添加 NuGet 包
dotnet add package NRules
```

### 配置服务

```csharp
// 注册 NRules 服务
builder.Services.AddNRulesServices();

// 配置 NRules 选项
builder.Services.Configure<NRulesOptions>(options => {
    options.EnableCache = true;
    options.CacheSize = 1000;
    options.Timeout = TimeSpan.FromSeconds(30);
});
```

### 定义规则

```csharp
public class HighValueOrderRule : Rule
{
    public override void Define()
    {
        // 规则定义
    }
}
```

### 使用规则引擎

```csharp
// 获取规则引擎服务
var ruleEngineService = serviceProvider.GetRequiredService<IRuleEngineService>();

// 创建规则会话
var session = ruleEngineService.CreateSession();

// 插入事实
session.Insert(order);

// 执行规则
session.Fire();
```

## 核心组件

### 规则引擎服务

- **IRuleEngineService**：规则引擎的主服务接口，用于创建规则会话
- **IRuleRepository**：规则仓储，用于注册和管理规则
- **IRuleSession**：规则会话，用于插入事实和执行规则

### 配置选项

- **NRulesOptions**：规则引擎配置选项，包括缓存设置、超时设置等

### 规则定义

- **Rule**：规则基类，所有自定义规则都继承自此类
- **RuleDefinition**：规则定义，包含规则类型、条件和动作

### 数据模型

- **Order**：订单示例类，用于演示规则引擎的使用
- **OrderStatus**：订单状态枚举
- **OrderPriority**：订单优先级枚举

## 性能特性

- **AOT 编译**：减少启动时间和内存占用
- **规则缓存**：缓存编译后的规则，提高执行性能
- **批处理**：支持批量事实处理，减少规则执行次数
- **内存优化**：使用 Span、Memory 等技术减少内存分配
- **多线程支持**：支持并行规则执行

## 部署指南

### AOT 编译

```bash
# 发布为 AOT 编译的单文件应用
dotnet publish --configuration Release --output ./publish --runtime win-x64 --self-contained true -p:PublishAot=true
```

### 支持的运行时

- win-x64
- linux-x64
- osx-x64

## 参考文档

- [详细文档](reference/README.md)
- [使用示例](reference/examples.md)

## 版本信息

- **技能版本**：1.0.0
- **.NET 版本**：net11.0
- **NRules 版本**：1.0.0

## 许可证

MIT License