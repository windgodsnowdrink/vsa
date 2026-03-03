# NRules 技能参考文档

## 概述

NRules 技能是一个基于 .NET 10 的规则引擎集成解决方案，提供了完整的规则定义、执行和管理功能。本文档详细介绍了 NRules 技能的核心组件、使用方法、配置选项和部署指南。

## 核心组件

### 规则引擎服务

#### IRuleEngineService

规则引擎的主服务接口，用于创建规则会话。

```csharp
public interface IRuleEngineService
{
    IRuleSession CreateSession();
}
```

#### IRuleRepository

规则仓储，用于注册和管理规则。

```csharp
public interface IRuleRepository
{
    void RegisterRulesFromAssembly(Assembly assembly);
    void RegisterRule(Type ruleType);
    RuleFactory CreateRuleFactory();
}
```

#### IRuleSession

规则会话，用于插入事实和执行规则。

```csharp
public interface IRuleSession
{
    void Insert(object fact);
    void Update(object fact);
    void Delete(object fact);
    void Fire();
}
```

### 规则定义

#### Rule

规则基类，所有自定义规则都继承自此类。

```csharp
public abstract class Rule
{
    public abstract void Define();
}
```

#### RuleDefinition

规则定义，包含规则类型、条件和动作。

```csharp
public class RuleDefinition
{
    public Type RuleType { get; set; }
    public Action<IContext, object> Action { get; set; }
    public Func<object, bool> Condition { get; set; }
}
```

### 配置选项

#### NRulesOptions

规则引擎配置选项，包括缓存设置、超时设置等。

| 属性 | 类型 | 默认值 | 描述 |
|------|------|--------|------|
| EnableCache | bool | true | 是否启用规则缓存 |
| CacheSize | int | 1000 | 缓存大小 |
| Timeout | TimeSpan | 30秒 | 规则执行超时时间 |
| EnableDetailedLogging | bool | false | 是否启用详细日志 |
| DefaultTimeZone | string | "UTC" | 默认时区 |

## 使用示例

### 基本使用

```csharp
// 注册服务
builder.Services.AddNRulesServices();

// 配置选项
builder.Services.Configure<NRulesOptions>(options => {
    options.EnableCache = true;
    options.CacheSize = 1000;
});

// 获取服务
var ruleEngineService = serviceProvider.GetRequiredService<IRuleEngineService>();
var ruleRepository = serviceProvider.GetRequiredService<IRuleRepository>();

// 注册规则
ruleRepository.RegisterRulesFromAssembly(typeof(OrderRules).Assembly);

// 创建会话
var session = ruleEngineService.CreateSession();

// 插入事实
var order = new Order { Id = 1, Amount = 6000, Status = OrderStatus.New };
session.Insert(order);

// 执行规则
session.Fire();

// 查看结果
Console.WriteLine($"订单状态: {order.Status}");
Console.WriteLine($"折扣: {order.Discount:P}");
```

### 高级配置

```csharp
// 高级配置
builder.Services.AddNRulesServices(options => {
    options.EnableCache = true;
    options.CacheSize = 5000;
    options.Timeout = TimeSpan.FromMinutes(1);
    options.EnableDetailedLogging = true;
    options.DefaultTimeZone = "Asia/Shanghai";
});
```

### 规则定义示例

```csharp
public class OrderRules
{
    public class HighValueOrderRule : Rule
    {
        public override void Define()
        {
            // 规则定义
        }
    }

    public class PriorityOrderRule : Rule
    {
        public override void Define()
        {
            // 规则定义
        }
    }
}
```

## 性能优化

### 规则缓存

启用规则缓存可以提高规则执行性能，特别是在规则数量较多的情况下。

```csharp
options.EnableCache = true;
options.CacheSize = 1000;
```

### 批处理

对于大量事实的处理，可以使用批处理方式减少规则执行次数。

```csharp
// 批量插入事实
foreach (var order in orders)
{
    session.Insert(order);
}

// 一次性执行规则
session.Fire();
```

### 内存优化

使用 `Span` 和 `Memory` 等技术减少内存分配，提高性能。

## 错误处理

### 规则执行异常

```csharp
try
{
    session.Fire();
}
catch (Exception ex)
{
    logger.LogError(ex, "规则执行失败");
    // 处理异常
}
```

### 规则定义验证

在注册规则时，应该验证规则定义的正确性，避免运行时错误。

## 部署指南

### AOT 编译

NRules 技能支持 AOT 编译，可以显著提高运行时性能和减少内存使用。

```bash
# 发布为 AOT 编译的单文件应用
dotnet publish --configuration Release --output ./publish --runtime win-x64 --self-contained true -p:PublishAot=true
```

### 支持的运行时

- win-x64
- linux-x64
- osx-x64

### 容器化部署

#### Dockerfile 示例

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish -r linux-x64 --self-contained true -p:PublishAot=true

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["./nrules_app"]
```

### 环境变量

| 环境变量 | 描述 | 默认值 |
|---------|------|--------|
| DOTNET_ENVIRONMENT | 运行环境 | Development |
| DOTNET_PUBLISH_AOT | 是否启用 AOT | 1 |
| DOTNET_TieredCompilation | 是否启用分层编译 | 1 |
| DOTNET_ReadyToRun | 是否启用 ReadyToRun | 1 |
| LOG_LEVEL | 日志级别 | Information |

## 最佳实践

### 规则设计

1. **单一职责**：每个规则只负责一个具体的业务逻辑
2. **命名规范**：使用清晰、描述性的规则名称
3. **规则分组**：将相关的规则组织在一起
4. **性能考虑**：避免在规则中执行耗时操作
5. **可测试性**：确保规则可以单独测试

### 规则管理

1. **版本控制**：对规则进行版本控制
2. **规则监控**：监控规则执行情况和性能
3. **规则审计**：记录规则执行历史
4. **规则优化**：定期分析和优化规则

### 性能优化

1. **启用缓存**：启用规则缓存提高性能
2. **批量处理**：使用批处理减少规则执行次数
3. **内存管理**：使用对象池和内存优化技术
4. **并行执行**：对于独立规则考虑并行执行
5. **规则优先级**：合理设置规则优先级

## 故障排除

### 常见问题

1. **规则不执行**
   - 检查规则是否正确注册
   - 检查事实是否正确插入
   - 检查规则条件是否满足

2. **规则执行性能差**
   - 启用规则缓存
   - 优化规则条件
   - 减少规则数量
   - 使用批处理

3. **内存使用高**
   - 启用 AOT 编译
   - 优化内存管理
   - 减少事实数量

4. **规则执行超时**
   - 增加超时设置
   - 优化规则执行时间
   - 分解复杂规则

## 参考资源

- [NRules 官方文档](https://nrules.net/)
- [NRules GitHub 仓库](https://github.com/nrules/nrules)
- [.NET 依赖注入文档](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [.NET 配置文档](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration)
- [.NET AOT 文档](https://docs.microsoft.com/en-us/dotnet/core/deploying/native-aot)

## 版本历史

| 版本 | 日期 | 变更说明 |
|------|------|----------|
| 1.0.0 | 2026-01-24 | 初始版本，支持 AOT 编译和完整的规则引擎功能 |
