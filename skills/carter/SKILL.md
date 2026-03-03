# carter Agent Skill - Carter 技能

## 技能概述

基于 .NET 10 的高性能 Carter 技能，为 .NET 开发者提供强大的现代化 API 框架功能，支持 AOT（提前编译）编译，适用于构建高性能、可扩展的 Web API 和微服务。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Carter@8.0.0
#:package Swashbuckle.AspNetCore@7.0.0
#:package Microsoft.Extensions.Aot@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
```

### 注册服务

在您的主应用程序中注册 Carter 服务：

```csharp
// 配置 Web 应用程序
var builder = WebApplication.CreateBuilder(args);

// 注册 Carter 服务
builder.Services.AddCarter();

// 配置 OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AOT 优化配置
builder.Services.Configure<AotSettings>(options => {
    options.EnableAotOptimization = true;
    options.EnableTrimOptimization = true;
});

var app = builder.Build();

// 启用 Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 配置路由和中间件
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// 映射 Carter 端点
app.MapCarter();

await app.RunAsync();
```

### 创建 API 模块

```csharp
// 定义 API 模块
public class ProductModule : CarterModule
{
    public ProductModule() : base("api/products")
    {
        // 配置模块
    }
    
    // 注册路由
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // GET: /api/products
        app.MapGet("", async (IProductService productService) =>
        {
            var products = await productService.GetAllAsync();
            return Results.Ok(products);
        })
        .WithName("GetAllProducts")
        .WithTags("Products")
        .Produces<List<Product>>(StatusCodes.Status200OK);
        
        // GET: /api/products/{id}
        app.MapGet("/{id:int}", async (int id, IProductService productService) =>
        {
            var product = await productService.GetByIdAsync(id);
            return product is not null ? Results.Ok(product) : Results.NotFound();
        })
        .WithName("GetProductById")
        .WithTags("Products")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
        
        // POST: /api/products
        app.MapPost("", async (Product product, IProductService productService) =>
        {
            var createdProduct = await productService.CreateAsync(product);
            return Results.CreatedAtRoute("GetProductById", new { id = createdProduct.Id }, createdProduct);
        })
        .WithName("CreateProduct")
        .WithTags("Products")
        .Accepts<Product>("application/json")
        .Produces<Product>(StatusCodes.Status201Created);
        
        // PUT: /api/products/{id}
        app.MapPut("/{id:int}", async (int id, Product product, IProductService productService) =>
        {
            var updatedProduct = await productService.UpdateAsync(id, product);
            return updatedProduct is not null ? Results.Ok(updatedProduct) : Results.NotFound();
        })
        .WithName("UpdateProduct")
        .WithTags("Products")
        .Accepts<Product>("application/json")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
        
        // DELETE: /api/products/{id}
        app.MapDelete("/{id:int}", async (int id, IProductService productService) =>
        {
            var deleted = await productService.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteProduct")
        .WithTags("Products")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}

// 产品模型
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// 产品服务接口
public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<Product?> UpdateAsync(int id, Product product);
    Task<bool> DeleteAsync(int id);
}

// 产品服务实现
public class ProductService : IProductService
{
    private static readonly List<Product> _products = new();
    
    public Task<List<Product>> GetAllAsync()
    {
        return Task.FromResult(_products.ToList());
    }
    
    public Task<Product?> GetByIdAsync(int id)
    {
        return Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
    }
    
    public Task<Product> CreateAsync(Product product)
    {
        product.Id = _products.Count + 1;
        _products.Add(product);
        return Task.FromResult(product);
    }
    
    public Task<Product?> UpdateAsync(int id, Product product)
    {
        var existingProduct = _products.FirstOrDefault(p => p.Id == id);
        if (existingProduct is null) return Task.FromResult<Product?>(null);
        
        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.IsActive = product.IsActive;
        
        return Task.FromResult(existingProduct);
    }
    
    public Task<bool> DeleteAsync(int id)
    {
        var count = _products.RemoveAll(p => p.Id == id);
        return Task.FromResult(count > 0);
    }
}
```

### 使用示例

```csharp
// 注册服务
builder.Services.AddSingleton<IProductService, ProductService>();

// 创建应用
var app = builder.Build();

// 映射 Carter 端点
app.MapCarter();

await app.RunAsync();
```

## 导航地图

```
carter/
├── index.yaml                           # 元数据索引描述
├── SKILL.md                            # 技能入口点（当前文件）
├── reference/                          # 参考文件
│   ├── README.md                      # 完整功能描述
│   └── examples.md                    # 使用示例
├── scripts/                            # 脚本和工具
    ├── carter_integration.cs           # Carter 集成示例
    ├── carter_integration.run.json      # 运行配置
    ├── carter_integration.setting.json  # 设置文件
    ├── cater_otel_integration.cs        # Carter + OpenTelemetry 集成示例
    ├── cater_otel_integration.run.json   # 运行配置
    └── cater_otel_integration.setting.json # 设置文件
```

## 主要功能

1. **现代化 API 框架**: 基于 .NET 10 构建的轻量级、高性能 API 框架
2. **AOT 编译支持**: 支持将应用编译为本机代码，提高运行时性能和启动速度
3. **模块化设计**: 基于 CarterModule 的模块化架构，便于组织和维护 API
4. **强大的路由系统**: 支持 RESTful 路由、参数绑定、模型验证等
5. **中间件支持**: 与 ASP.NET Core 中间件生态系统无缝集成
6. **依赖注入**: 原生支持 .NET 依赖注入容器
7. **异步编程模型**: 基于异步/等待模式，提高并发处理能力
8. **OpenAPI/Swagger 支持**: 自动生成 API 文档
9. **内容协商**: 支持 JSON、XML 等多种格式
10. **认证和授权**: 与 ASP.NET Core 认证授权系统集成
11. **过滤器支持**: 支持动作过滤器、结果过滤器等
12. **健康检查**: 内置健康检查端点

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

<!-- 添加 AOT 兼容的依赖 -->
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Aot" Version="10.0.0" />
  <PackageReference Include="Carter" Version="8.0.0" />
  <PackageReference Include="Microsoft.AspNetCore.Http" Version="10.0.0" />
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

### AOT 兼容性注意事项

1. **使用 AOT 兼容的库**: 确保使用的 Carter 版本和依赖库支持 AOT 编译
2. **避免反射**: 避免在 API 处理中使用反射
3. **资源处理**: 确保所有资源在 AOT 编译时能被正确处理
4. **动态代码生成**: 避免使用动态代码生成技术
5. **使用值类型**: 优先使用值类型而非引用类型，减少内存分配
6. **测试验证**: 在 AOT 编译后进行充分测试
7. **避免使用动态代理**: 避免使用 Castle DynamicProxy 等动态代理库
8. **使用 AOT 兼容的序列化**: 优先使用 System.Text.Json 等 AOT 兼容的序列化库

## 与其他系统集成

### 与 EF Core 集成

```csharp
// 配置 EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 注册存储库
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

// 在 API 模块中使用
public class ProductModule : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("", async (IProductService productService) =>
        {
            var products = await productService.GetAllAsync();
            return Results.Ok(products);
        });
    }
}
```

### 与 OpenTelemetry 集成

```csharp
// 配置 OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation()
               .AddSqlClientInstrumentation()
               .AddJaegerExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation()
               .AddProcessInstrumentation()
               .AddPrometheusExporter();
    });

// 启用 Prometheus 端点
app.MapPrometheusScrapingEndpoint();
```

### 与 GraphQL 集成

```csharp
// 配置 GraphQL
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()
    .AddType<ProductType>()
    .AddInMemorySubscriptions();

// 映射 GraphQL 端点
app.MapGraphQL();
```

## 性能优化建议

1. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译可以显著提高性能
2. **使用异步 API**: 优先使用异步 API，避免阻塞主线程
3. **优化序列化**: 使用 System.Text.Json 并启用源生成器
4. **使用内存池**: 对于频繁分配的内存，使用 ArrayPool<T> 减少 GC 压力
5. **优化数据库查询**: 使用 EF Core 性能优化、索引优化等
6. **启用响应压缩**: 启用 Gzip 或 Brotli 压缩
7. **使用缓存**: 对频繁访问的数据使用缓存
8. **优化路由**: 避免复杂的路由模板
9. **使用 HTTP/2 或 HTTP/3**: 提高并发连接处理能力
10. **监控性能**: 使用 OpenTelemetry 等工具监控 API 性能

## 故障排除

### 常见问题

1. **API 启动失败**
   - 检查配置文件
   - 验证依赖项版本兼容性
   - 查看详细日志
   - 检查端口占用情况

2. **路由冲突**
   - 检查路由模板是否冲突
   - 确保每个路由有唯一的名称
   - 检查模块前缀是否冲突

3. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 查看详细的编译日志
   - 确保所有依赖都支持 AOT
   - 检查是否使用了反射等不兼容特性
   - 考虑调整 TrimMode

4. **性能问题**
   - 使用性能分析工具定位瓶颈
   - 优化数据库查询
   - 启用缓存
   - 考虑使用 AOT 编译
   - 优化序列化和反序列化

5. **模型验证失败**
   - 检查模型验证规则
   - 查看请求体格式
   - 确保使用正确的 Content-Type

## 扩展开发

### 创建自定义中间件

```csharp
// 自定义中间件
public class CustomMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomMiddleware> _logger;
    
    public CustomMiddleware(RequestDelegate next, ILogger<CustomMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");
        
        // 执行下一个中间件
        await _next(context);
        
        _logger.LogInformation($"Response: {context.Response.StatusCode}");
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

// 使用中间件
app.UseCustomMiddleware();
```

### 创建自定义过滤器

```csharp
// 自定义动作过滤器
public class LoggingActionFilter : IActionFilter
{
    private readonly ILogger<LoggingActionFilter> _logger;
    private Stopwatch _stopwatch;
    
    public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
    {
        _logger = logger;
    }
    
    public void OnActionExecuting(ActionExecutingContext context)
    {
        _stopwatch = Stopwatch.StartNew();
        _logger.LogInformation($"Action executing: {context.ActionDescriptor.DisplayName}");
    }
    
    public void OnActionExecuted(ActionExecutedContext context)
    {
        _stopwatch.Stop();
        _logger.LogInformation($"Action executed: {context.ActionDescriptor.DisplayName} in {_stopwatch.ElapsedMilliseconds} ms");
    }
}

// 注册过滤器
builder.Services.AddScoped<LoggingActionFilter>();
```

## 最佳实践

1. **使用模块化设计**: 按照业务领域组织 API 模块
2. **遵循 RESTful 设计原则**: 使用正确的 HTTP 方法和状态码
3. **实现适当的错误处理**: 提供清晰的错误信息
4. **使用依赖注入**: 避免硬编码依赖
5. **编写单元测试**: 测试 API 逻辑和业务规则
6. **使用 OpenAPI/Swagger**: 自动生成 API 文档
7. **实现认证和授权**: 保护敏感 API
8. **使用日志记录**: 记录关键操作和错误
9. **监控性能**: 使用 OpenTelemetry 等工具监控 API 性能
10. **考虑安全性**: 实现适当的安全措施，如 CORS、CSRF 保护等
11. **使用异步编程**: 提高 API 并发处理能力
12. **考虑可扩展性**: 设计 API 时考虑未来的扩展需求

## 总结

Carter 技能提供了一套完整的现代化 API 框架解决方案，基于 .NET 10 构建，支持 AOT 编译，具有高性能、模块化、可扩展等特点。通过 Carter，开发者可以快速构建高性能、可维护的 Web API 和微服务，适用于各种规模的应用场景。

该技能遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，支持与多种系统集成，如 EF Core、OpenTelemetry、GraphQL 等。同时，提供了详细的性能优化建议和故障排除指南，帮助开发者构建高性能、可靠的 API 服务。