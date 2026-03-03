# brighter Agent Skill - brighter 技能

## 技能概述

基于 .NET 10 的高性能 Brighter 命令查询责任分离（CQRS）技能，为 .NET 开发者提供强大的 CQRS 功能，支持 AOT（提前编译）编译，适用于构建高性能、可扩展的分布式系统。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:package Paramore.Brighter@9.0.0
#:package Paramore.Brighter.Extensions.DependencyInjection@9.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
```

### 注册服务

```csharp
// 注册 Brighter 服务
builder.Services.AddBrighter(options =>
{
    options.HandlerLifetime = ServiceLifetime.Scoped;
    options.CommandProcessorLifetime = ServiceLifetime.Scoped;
});

// 注册命令处理器
builder.Services.AddHandlersFromAssemblies(typeof(Program).Assembly);
```

### 使用示例

```csharp
// 创建命令
var command = new CreateTodoCommand
{
    Id = Guid.NewGuid(),
    Title = "完成 Brighter 技能实现",
    Description = "按照 AOT 架构要求实现 Brighter 技能",
    DueDate = DateTime.UtcNow.AddDays(7)
};

// 获取命令处理器
var commandProcessor = serviceProvider.GetRequiredService<IAmACommandProcessor>();

// 发送命令
await commandProcessor.SendAsync(command);
```

## 导航地图

```
brighter/
├── index.yaml                           # 元数据索引描述
├── SKILL.md                            # 技能入口点（当前文件）
├── reference/                          # 参考文件
│   ├── README.md                      # 完整功能描述
│   └── examples.md                    # 使用示例
├── scripts/                            # 脚本和工具
    ├── brighter_integration.cs         # Brighter 集成示例
    ├── brighter_integration.run.json   # 运行配置
    └── brighter_integration.setting.json # 设置文件
```

## 主要功能

1. **命令处理和查询处理分离**: 实现 CQRS 架构，分离命令处理和查询处理
2. **支持多种消息中间件**: 支持 RabbitMQ、Kafka、SQS 等多种消息中间件
3. **支持 AOT 编译优化**: 支持将应用编译为本机代码，提高运行时性能
4. **事务处理支持**: 支持事务性消息处理，确保数据一致性
5. **重试和熔断机制**: 支持命令处理的重试和熔断，提高系统可靠性
6. **异步和同步处理**: 支持异步和同步命令处理模式
7. **多种命令处理器**: 支持多种命令处理器实现
8. **查询处理器**: 支持查询处理器实现
9. **生产级配置**: 支持生产环境的配置和监控
10. **扩展机制**: 支持自定义扩展和插件

## 扩展说明

此技能提供完整的 Brighter CQRS 解决方案，您可以根据需要进行扩展：

1. **自定义命令处理器**: 实现自定义的命令处理器
2. **自定义查询处理器**: 实现自定义的查询处理器
3. **集成新的消息中间件**: 与新的消息中间件集成
4. **添加自定义拦截器**: 添加自定义的命令拦截器
5. **扩展事务支持**: 扩展事务处理机制
6. **优化性能**: 根据特定场景优化性能

## 最佳实践

1. **使用依赖注入**: 始终使用依赖注入管理 Brighter 服务
2. **采用异步编程**: 优先使用异步 API 避免阻塞主线程
3. **合理设计命令和查询**: 遵循 CQRS 原则，合理设计命令和查询
4. **使用事务处理**: 对于关键操作，使用事务确保数据一致性
5. **实施重试机制**: 对于外部依赖，实施适当的重试机制
6. **使用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译
7. **添加适当的日志**: 添加详细的日志记录，便于调试和监控
8. **监控系统性能**: 定期监控系统性能，确保满足需求
9. **测试命令处理器**: 编写单元测试和集成测试
10. **遵循 CQRS 最佳实践**: 遵循 CQRS 架构的最佳实践

## AOT 编译支持

### AOT 编译配置

在项目文件中添加以下配置以支持 AOT 编译：

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
  <TrimMode>Full</TrimMode>
  <PublishReadyToRun>true</PublishReadyToRun>
  <PublishSingleFile>true</PublishSingleFile>
  <SelfContained>true</SelfContained>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
</PropertyGroup>
```

### AOT 编译命令

```bash
# 编译为 Windows x64 原生可执行文件
dotnet publish -c Release -r win-x64 --self-contained

# 编译为 Linux x64 原生可执行文件
dotnet publish -c Release -r linux-x64 --self-contained

# 编译为 macOS x64 原生可执行文件
dotnet publish -c Release -r osx-x64 --self-contained
```

### AOT 编译注意事项

1. **使用 AOT 兼容的 Brighter 版本**: 确保使用的 Brighter 版本支持 AOT 编译
2. **避免反射**: 避免在命令处理器和查询处理器中使用反射
3. **资源加载**: 确保所有资源在 AOT 编译时能被正确处理
4. **动态代码生成**: 避免使用动态代码生成技术
5. **测试验证**: 在 AOT 编译后进行充分测试
6. **性能比较**: 比较 JIT 和 AOT 编译后的性能差异

## 与其他系统集成

### 与 ASP.NET Core 集成

```csharp
// ASP.NET Core 中集成 Brighter
public void ConfigureServices(IServiceCollection services)
{
    // 配置 Brighter
    services.AddBrighter(options =>
    {
        options.Policies = new PolicyRegistry()
            .Register<RetryPolicy>(CommandProcessorRetryPolicy.Get())
            .Register<CircuitBreakerPolicy>(CommandProcessorCircuitBreakerPolicy.Get());
    });
    
    // 注册处理器
    services.AddHandlersFromAssemblies(typeof(Program).Assembly);
    
    // 其他服务配置
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
}
```

### 与消息中间件集成

```csharp
// 与 RabbitMQ 集成
builder.Services.AddBrighter(options =>
{
    options.CommandProcessorLifetime = ServiceLifetime.Scoped;
    options.HandlerLifetime = ServiceLifetime.Scoped;
})
.AddRabbitMqProducer(options =>
{
    options.HostName = "localhost";
    options.Exchange = "brighter.exchange";
    options.VirtualHost = "/";
    options.UserName = "guest";
    options.Password = "guest";
    options.Port = 5672;
});
```
