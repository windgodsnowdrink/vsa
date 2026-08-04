# boxed - 参考文档

## 概述

boxed 是一个基于 .NET 10 的高性能 Boxed 模板系统，专为 .NET 开发者设计，支持 AOT（提前编译）编译，提供 GraphQL、Web API、Orleans 等多种 .NET 模板支持。

## 核心组件

### 1. GraphQL 模板
- **位置**: scripts/boxed_graphql.cs, scripts/boxed_graphql_production.cs
- **功能**: 提供基于 HotChocolate 的高性能 GraphQL 服务模板
- **特性**: 
  - 支持 AOT 编译优化
  - 基于 HotChocolate 14.x
  - 支持查询、变更和订阅
  - 支持批量处理
  - 支持自定义类型和扩展
  - 支持多种认证方式
  - 支持生产级配置

### 2. Web API 模板
- **位置**: scripts/boxed_webapi.cs
- **功能**: 提供基于 ASP.NET Core 的高性能 Web API 模板
- **特性**: 
  - 支持 AOT 编译优化
  - 基于 ASP.NET Core 10
  - 支持控制器和最小 API
  - 支持 Swagger/OpenAPI 文档
  - 支持多种认证方式
  - 支持健康检查和指标收集
  - 支持生产级配置

### 3. Orleans 模板
- **位置**: scripts/boxed_orleans.cs
- **功能**: 提供基于 Orleans 的分布式应用模板
- **特性**: 
  - 支持 AOT 编译优化
  - 基于 Orleans 8.x
  - 支持分布式计算
  - 支持高可用和弹性
  - 支持状态管理
  - 支持多种集群配置
  - 支持生产级监控

## 使用示例

### GraphQL 模板使用示例

```csharp
// GraphQL 服务配置示例
using HotChocolate.AspNetCore;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 配置数据库连接
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 添加 GraphQL 服务
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .AddDbContext<AppDbContext>();

var app = builder.Build();

// 配置 GraphQL 端点
app.MapGraphQL();
app.MapGraphQLSubscriptions();

app.Run();
```

### Web API 模板使用示例

```csharp
// Web API 服务配置示例
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 添加服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

var app = builder.Build();

// 配置中间件
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
```

## 配置选项

### GraphQL 模板配置

```json
{
  "GraphQLSettings": {
    "EnableAot": true,                    // 启用 AOT 编译
    "EnableSubscriptions": true,          // 启用订阅
    "EnableFiltering": true,              // 启用过滤
    "EnableSorting": true,                // 启用排序
    "EnableProjections": true,            // 启用投影
    "EnablePersistedQueries": false,      // 启用持久化查询
    "MaxComplexity": 100,                 // 最大复杂度
    "EnableDetailedErrors": false,        // 启用详细错误信息
    "EnableIntrospection": true           // 启用内省
  }
}
```

### Web API 模板配置

```json
{
  "WebApiSettings": {
    "EnableAot": true,                    // 启用 AOT 编译
    "EnableSwagger": true,                // 启用 Swagger
    "EnableHttpsRedirection": true,       // 启用 HTTPS 重定向
    "EnableCors": true,                   // 启用 CORS
    "CorsOrigins": ["*"],                // CORS 允许的源
    "EnableHealthChecks": true,           // 启用健康检查
    "EnableMetrics": true,                // 启用指标收集
    "MaxRequestBodySize": 1048576,        // 最大请求体大小
    "EnableRequestLogging": false         // 启用请求日志
  }
}
```

## 性能优化

1. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译以获得最佳性能
2. **使用异步 API**: 优先使用异步 API 避免阻塞主线程
3. **优化数据库访问**: 
   - 使用 EF Core 缓存查询
   - 优化查询条件
   - 使用索引
   - 避免 N+1 查询问题
4. **使用缓存**: 
   - 对于频繁访问的数据使用内存缓存
   - 对于分布式场景使用 Redis 缓存
   - 设置合理的缓存过期时间
5. **优化中间件**: 只启用必要的中间件，避免不必要的性能开销
6. **使用 Response Compression**: 启用响应压缩减少网络传输
7. **优化序列化**: 使用 System.Text.Json 并优化序列化配置
8. **使用 Connection Pooling**: 启用数据库连接池
9. **优化并发**: 使用 async/await 和适当的并发模式
10. **使用 CDN**: 对于静态资源使用 CDN

## AOT 编译优化

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

<!-- 添加 AOT 兼容的依赖 -->
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Aot" Version="10.0.0" />
  <PackageReference Include="Microsoft.Extensions.DependencyInjection.Aot" Version="10.0.0" />
</ItemGroup>
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

### AOT 兼容注意事项

1. **使用 AOT 兼容的库**: 确保使用的所有库都支持 AOT 编译
2. **避免反射**: 避免使用反射，或使用 Source Generator 替代
3. **资源加载**: 确保所有资源在 AOT 编译时能被正确处理
4. **动态代码生成**: 避免使用动态代码生成技术
5. **泛型类型**: 限制使用复杂的泛型类型
6. **序列化**: 确保所有需要序列化的类型都具有公共无参数构造函数
7. **测试验证**: 在 AOT 编译后进行充分测试

## 故障排除

### 常见问题

1. **GraphQL 服务启动失败**
   - 检查 HotChocolate 版本是否兼容
   - 检查 GraphQL 类型定义是否正确
   - 检查数据库连接是否配置正确
   - 查看日志获取详细错误信息

2. **Web API Swagger 无法访问**
   - 检查 Swagger 配置是否正确
   - 检查中间件顺序是否正确
   - 检查是否启用了正确的端点

3. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 检查是否使用了反射等不兼容的特性
   - 查看详细的编译日志
   - 确保所有依赖都支持 AOT 编译

4. **性能问题**
   - 启用性能监控
   - 检查数据库查询
   - 检查缓存配置
   - 考虑使用 AOT 编译

5. **Orleans 集群连接问题**
   - 检查集群配置
   - 检查网络连接
   - 检查防火墙设置
   - 查看 Orleans 日志

## 扩展开发

### 扩展 GraphQL 模板

```csharp
// 自定义 GraphQL 类型
public class CustomGraphType : ObjectType<CustomModel>
{
    protected override void Configure(IObjectTypeDescriptor<CustomModel> descriptor)
    {
        descriptor.Field(f => f.Id).Type<NonNullType<IntType>>();
        descriptor.Field(f => f.Name).Type<StringType>();
        descriptor.Field(f => f.Value).Type<FloatType>();
        
        // 添加自定义解析器
        descriptor.Field("customField")
            .Type<StringType>()
            .Resolve(context =>
            {
                var customService = context.Service<IcustomService>();
                return customService.GetCustomValue(context.Parent<CustomModel>());
            });
    }
}

// 将自定义类型添加到 GraphQL 服务器
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddType<CustomGraphType>();
```

### 扩展 Web API 模板

```csharp
// 自定义中间件
public class CustomMiddleware
{
    private readonly RequestDelegate _next;
    
    public CustomMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // 中间件逻辑
        await _next(context);
    }
}

// 扩展方法
public static class CustomMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomMiddleware>();
    }
}

// 使用自定义中间件
app.UseCustomMiddleware();
```

## 最佳实践

1. **使用依赖注入**: 始终使用依赖注入管理服务
2. **采用分层架构**: 分离关注点，提高代码的可维护性
3. **实施适当的认证和授权**: 根据应用需求选择合适的认证方式
4. **添加详细的日志**: 使用结构化日志，便于调试和监控
5. **实施健康检查**: 添加健康检查端点，便于监控系统状态
6. **使用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译
7. **优化数据库访问**: 使用 EF Core 缓存、索引等优化查询性能
8. **实施缓存策略**: 对于频繁访问的数据，实施适当的缓存策略
9. **使用异步编程**: 优先使用异步 API 避免阻塞主线程
10. **遵循 RESTful 设计原则**: 对于 Web API，遵循 RESTful 设计原则
11. **使用 GraphQL 最佳实践**: 对于 GraphQL，使用分页、批量处理等最佳实践
12. **定期进行性能测试**: 定期测试应用性能，确保满足需求

## 与其他系统集成

### 与身份认证系统集成

```csharp
// 与 Azure AD 集成
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

app.UseAuthentication();
app.UseAuthorization();
```

### 与监控系统集成

```csharp
// 与 Prometheus 集成
using Prometheus;

app.UseHttpMetrics();
app.MapMetrics();
```

### 与消息队列集成

```csharp
// 与 RabbitMQ 集成
using MassTransit;

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");
    });
});
```

## 部署建议

1. **容器化部署**: 使用 Docker 容器化应用，便于部署和扩展
2. **使用 Kubernetes**: 对于分布式应用，使用 Kubernetes 进行编排
3. **使用 CI/CD 流程**: 实现自动化构建、测试和部署
4. **使用环境变量**: 使用环境变量配置应用，便于不同环境部署
5. **实施蓝绿部署**: 减少部署风险
6. **使用 AOT 编译**: 对于生产环境，考虑使用 AOT 编译提高性能
7. **实施监控和告警**: 监控应用性能和健康状态，及时发现问题
8. **使用日志聚合**: 使用 ELK Stack 或其他日志聚合工具

## 总结

Boxed 模板技能提供了一套完整的 .NET 10 模板解决方案，支持 GraphQL、Web API、Orleans 等多种应用场景，并支持 AOT 编译优化。通过遵循最佳实践和合理配置，可以构建高性能、可扩展、可维护的 .NET 应用。
