# boxed Agent Skill - boxed 技能

## 技能概述

基于 .NET 10 的高性能 Boxed 模板技能，为 .NET 开发者提供强大的模板生成功能，支持 AOT（提前编译）编译，适用于 GraphQL、Web API、Orleans 等多种 .NET 项目场景。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package HotChocolate@14.0.0
#:package Swashbuckle.AspNetCore@6.5.0
#:package Orleans.Core@8.0.0
```

### 使用 GraphQL 模板

```csharp
// GraphQL 服务启动示例
using HotChocolate.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 添加 GraphQL 服务
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

var app = builder.Build();

// 配置 GraphQL 端点
app.MapGraphQL();

app.Run();
```

### 使用 Web API 模板

```csharp
// Web API 服务启动示例
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 添加 Web API 服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 配置中间件
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

## 导航地图

```
boxed/
├── index.yaml                           # 元数据索引描述
├── SKILL.md                            # 技能入口点（当前文件）
├── reference/                          # 参考文件
│   ├── README.md                      # 完整功能描述
│   └── examples.md                    # 使用示例
├── scripts/                            # 脚本和工具
    ├── boxed_graphql.cs               # GraphQL 模板示例
    ├── boxed_graphql.run.json         # 运行配置
    ├── boxed_graphql.setting.json     # 设置文件
    ├── boxed_webapi.cs                # Web API 模板示例
    ├── boxed_webapi.run.json          # 运行配置
    ├── boxed_webapi.setting.json      # 设置文件
    ├── boxed_orleans.cs               # Orleans 模板示例
    ├── boxed_orleans.run.json         # 运行配置
    ├── boxed_orleans.setting.json     # 设置文件
    ├── boxed_graphql_production.cs    # GraphQL 生产级模板示例
    ├── boxed_graphql_production.run.json         # 运行配置
    └── boxed_graphql_production.setting.json     # 设置文件
```

## 主要功能

1. **GraphQL 服务模板**: 基于 HotChocolate 的高性能 GraphQL 服务模板
2. **Web API 模板**: 基于 ASP.NET Core 的高性能 Web API 模板
3. **Orleans 分布式应用模板**: 基于 Orleans 的分布式应用模板
4. **支持 AOT 编译优化**: 支持将模板生成的应用编译为本机代码
5. **多种认证方式支持**: JWT、OAuth 2.0、OpenID Connect 等
6. **日志和监控集成**: 支持多种日志框架和监控系统
7. **生产级配置**: 支持环境配置、健康检查、指标收集等
8. **支持多种数据访问**: EF Core、Dapper、MongoDB 等
9. **支持缓存集成**: Redis、MemoryCache 等
10. **支持消息队列集成**: RabbitMQ、Kafka 等
11. **支持微服务架构**: 支持服务发现、负载均衡等
12. **支持自定义模板扩展**: 支持根据业务需求扩展模板

## 扩展说明

此技能提供完整的 Boxed 模板解决方案，您可以根据需要进行扩展：

1. **自定义模板**: 根据业务需求自定义模板生成规则
2. **扩展现有模板**: 扩展现有模板以支持更多功能
3. **集成新的框架**: 与新的 .NET 框架和库集成
4. **优化性能**: 根据特定场景优化模板性能
5. **添加新的模板类型**: 支持新的 .NET 项目类型

## 最佳实践

1. **使用依赖注入**: 始终使用依赖注入管理服务
2. **采用异步编程**: 优先使用异步 API 避免阻塞
3. **实施适当的认证**: 根据应用需求选择合适的认证方式
4. **添加详细的日志**: 添加详细的日志记录，便于调试和监控
5. **实施健康检查**: 添加健康检查端点，便于监控系统状态
6. **使用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译
7. **优化数据库访问**: 优化数据库查询和连接管理
8. **实施缓存策略**: 对于频繁访问的数据，实施适当的缓存策略
9. **实施监控和指标**: 添加监控和指标收集，便于性能分析
10. **遵循 .NET 最佳实践**: 遵循 .NET 10 最佳实践和设计原则

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

1. **使用 AOT 兼容的库**: 确保使用的库支持 AOT 编译
2. **避免反射**: 避免在模板中使用反射，或使用 Source Generator 替代
3. **资源加载**: 确保所有资源在 AOT 编译时能被正确处理
4. **动态代码生成**: 避免使用动态代码生成技术
5. **测试验证**: 在 AOT 编译后进行充分测试
6. **性能比较**: 比较 JIT 和 AOT 编译后的性能差异
7. **内存使用**: 监控 AOT 编译后的内存使用情况

## 与其他系统集成

### 与 GraphQL 客户端集成

```csharp
// GraphQL 客户端集成示例
using StrawberryShake;

[GraphQLClient("https://localhost:5001/graphql")]
public interface IProductClient
{
    [Query(nameof(GetProducts))]
    Task<IOperationResult<IGetProductsResult>> GetProductsAsync();
}
```

### 与 Orleans 客户端集成

```csharp
// Orleans 客户端集成示例
using Orleans;

var client = new ClientBuilder()
    .UseLocalhostClustering()
    .Build();

await client.Connect();

// 获取 Grain 实例
var grain = client.GetGrain<IProductGrain>(productId);
var product = await grain.GetProductAsync();
```
