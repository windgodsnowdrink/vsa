# MiniAPI - 参考文档

## 概述

MiniAPI 是基于 .NET 10 的高性能 API 开发框架，专为 .NET 开发者设计。它提供了一系列 API 开发功能，包括路由、中间件、依赖注入、异步编程等，帮助开发者快速构建高性能的 API 服务。

## 核心组件

### 1. WebApplication
- **位置**: scripts/miniapi_impl.cs
- **功能**: 核心应用程序构建器
- **特性**: 
  - 提供简洁的 API 构建语法
  - 支持依赖注入
  - 集成中间件管道
  - 高性能路由系统

### 2. 路由系统
- **位置**: scripts/miniapi_impl.cs
- **功能**: 处理 HTTP 请求路由
- **特性**: 
  - 支持 RESTful API 路由
  - 支持参数绑定
  - 支持路由约束
  - 高性能路由匹配

### 3. 中间件
- **位置**: scripts/miniapi_impl.cs
- **功能**: 处理 HTTP 请求和响应
- **特性**: 
  - 支持内置中间件
  - 支持自定义中间件
  - 中间件管道配置
  - 错误处理中间件

### 4. 依赖注入
- **位置**: scripts/miniapi_impl.cs
- **功能**: 管理服务依赖
- **特性**: 
  - 集成 Microsoft.Extensions.DependencyInjection
  - 支持多种生命周期
  - 支持配置绑定
  - 支持服务装饰器

### 5. 健康检查
- **位置**: scripts/miniapi_impl.cs
- **功能**: 提供健康检查端点
- **特性**: 
  - 内置健康检查中间件
  - 支持自定义健康检查
  - 支持健康检查 UI
  - 集成监控系统

## 使用示例

### 基本用法

```csharp
// 创建 MiniAPI 应用
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// 定义 API 端点
app.MapGet("/", () => "Hello World!")
   .WithName("GetHello")
   .WithOpenApi();

app.Run();
```

### 高级配置

```csharp
// 创建 MiniAPI 应用
var builder = WebApplication.CreateBuilder(args);

// 配置应用设置
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// 添加服务
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Todo API",
        Description = "A simple Todo API built with MiniAPI"
    });
});

// 配置 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 配置中间件
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// 定义 API 端点
app.MapGet("/", () => "Todo API")
   .WithName("GetRoot")
   .WithOpenApi();

app.Run();
```

## 配置选项

### 应用配置

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TodoDb;Trusted_Connection=True;"
  },
  "TodoApi": {
    "PageSize": 10,
    "EnableCaching": true,
    "CacheDuration": "00:05:00"
  }
}
```

### 路由配置

```csharp
// 路由参数约束
app.MapGet("/todos/{id:int}", (int id) => Results.Ok($"Todo {id}"));

// 路由参数转换
app.MapGet("/todos/{id:guid}", (Guid id) => Results.Ok($"Todo {id}"));

// 可选路由参数
app.MapGet("/todos/{id?}", (int? id) =>
{
    if (id is null)
    {
        return Results.Ok("All todos");
    }
    return Results.Ok($"Todo {id}");
});
```

## 性能优化

1. **内存分配优化**：减少不必要的内存分配
2. **GC 压力优化**：减少 GC 触发次数
3. **并发优化**：使用线程安全的代码
4. **批处理优化**：批量处理请求提高效率
5. **缓存使用**：合理使用缓存提高性能
6. **网络传输优化**：优化网络传输中的数据处理
7. **中间件优化**：减少不必要的中间件
8. **路由优化**：使用高效的路由设计
9. **连接池**：使用连接池管理数据库连接
10. **异步编程**：使用异步 API 避免阻塞

## 故障排除

### 常见问题

1. **启动失败**
   - 检查配置文件
   - 验证端口是否被占用
   - 检查依赖项是否正确安装
   - 查看日志信息

2. **路由未找到**
   - 检查路由定义
   - 验证 HTTP 方法是否正确
   - 检查路由参数约束
   - 启用详细日志

3. **依赖注入失败**
   - 检查服务注册
   - 验证服务生命周期
   - 检查构造函数参数
   - 查看依赖注入错误日志

4. **性能问题**
   - 启用缓存
   - 优化数据库查询
   - 减少中间件使用
   - 增加服务器资源

## 扩展开发

### 添加自定义中间件

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
        // 处理请求前的逻辑
        Console.WriteLine("Request incoming...");

        // 调用下一个中间件
        await _next(context);

        // 处理响应后的逻辑
        Console.WriteLine("Response outgoing...");
    }
}

// 中间件扩展方法
public static class CustomMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomMiddleware>();
    }
}

// 使用中间件
app.UseCustomMiddleware();
```

### 添加自定义健康检查

```csharp
// 自定义健康检查
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly IDbConnection _connection;

    public DatabaseHealthCheck(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _connection.OpenAsync(cancellationToken);
            return HealthCheckResult.Healthy("Database connection is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database connection failed", ex);
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
}

// 注册健康检查
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");

// 配置健康检查端点
app.MapHealthChecks("/health");
app.MapHealthChecksUI();
```

## AOT 编译优化

### AOT 编译配置

```yaml
#:property PublishAot=true
#:property IncludeNativeLibrariesForSelfExtract=true
#:property EnableCppCodeGen=true
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
#:property RuntimeIdentifier=linux-x64
#:property RuntimeIdentifier=osx-x64
```

### AOT 编译最佳实践

1. **避免反射**：使用静态分析可检测的代码
2. **避免动态类型**：使用强类型
3. **避免运行时代码生成**：使用预编译代码
4. **优化内存使用**：使用 Span<T> 和 Memory<T>
5. **减少依赖**：最小化依赖项
6. **使用值类型**：减少 GC 压力
7. **避免大对象分配**：避免分配大于 85KB 的对象
8. **使用对象池**：对于频繁创建和销毁的对象，使用对象池

## 部署说明

### 部署步骤

1. **编译**：使用 .NET 10 SDK 编译代码
2. **打包**：打包为单文件可执行文件
3. **部署**：部署到目标环境
4. **配置**：配置环境变量和配置文件
5. **启动**：启动服务

### 环境要求

- .NET 10 运行时或更高版本
- 足够的内存和磁盘空间
- 支持 AOT 编译的操作系统
- 网络连接（用于 API 访问）

### 容器化部署

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out --self-contained true --runtime linux-x64 -p:PublishAot=true

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/out ./

EXPOSE 80

ENTRYPOINT ["./miniapi_impl"]
```

## 监控和维护

### 监控指标

1. **请求量**：API 请求数量
2. **响应时间**：API 响应时间
3. **错误率**：API 错误率
4. **内存使用**：内存使用情况
5. **CPU 使用**：CPU 使用情况
6. **网络流量**：网络流量情况
7. **健康状态**：服务健康状态

### 维护建议

1. **定期检查**：定期检查服务状态
2. **优化配置**：根据实际使用情况优化配置
3. **更新依赖**：定期更新依赖项
4. **性能测试**：定期进行性能测试
5. **安全审计**：定期进行安全审计
6. **备份**：定期备份配置和数据
7. **监控**：建立完善的监控系统
8. **告警**：设置合理的告警阈值

## 总结

MiniAPI 智能体技能提供了一套完整的 API 开发解决方案，包括路由、中间件、依赖注入、异步编程等功能。它基于 .NET 10 构建，支持 AOT 编译，可以帮助 .NET 开发者更高效地构建高性能的 API 服务，提高应用程序性能，确保服务的可靠性和可扩展性，实现更好的用户体验。
