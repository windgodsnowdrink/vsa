# Carter - 参考文档

## 概述

Carter是一个基于.NET 10的高性能、模块化API框架，为.NET开发者提供了一种简单、优雅的方式来构建现代化的Web API和微服务。Carter遵循了ASP.NET Core的设计原则，同时提供了更简洁、更模块化的API开发体验。

## 核心组件

### 1. CarterModule

- **功能**: 定义API模块，组织相关路由和业务逻辑
- **特性**: 
  - 支持模块前缀路由
  - 便于组织和维护API
  - 支持依赖注入
  - 支持中间件和过滤器
- **使用示例**: 
  ```csharp
  public class ProductModule : CarterModule
  {
      public ProductModule() : base("api/products")
      {
          // 配置模块
      }
      
      public override void AddRoutes(IEndpointRouteBuilder app)
      {
          // 注册路由
      }
  }
  ```

### 2. 路由系统

- **功能**: 处理HTTP请求路由，支持RESTful API设计
- **特性**: 
  - 支持各种HTTP方法（GET、POST、PUT、DELETE等）
  - 支持路由参数绑定
  - 支持模型验证
  - 支持路由约束
- **使用示例**: 
  ```csharp
  app.MapGet("/api/products/{id:int}", async (int id, IProductService productService) =>
  {
      var product = await productService.GetByIdAsync(id);
      return product is not null ? Results.Ok(product) : Results.NotFound();
  });
  ```

### 3. 中间件系统

- **功能**: 处理HTTP请求和响应的中间件
- **特性**: 
  - 与ASP.NET Core中间件生态系统兼容
  - 支持自定义中间件
  - 支持中间件排序
- **使用示例**: 
  ```csharp
  app.UseMiddleware<CustomMiddleware>();
  ```

### 4. 依赖注入

- **功能**: 管理服务和依赖关系
- **特性**: 
  - 与.NET依赖注入容器兼容
  - 支持构造函数注入
  - 支持多种服务生命周期（单例、作用域、瞬时）
- **使用示例**: 
  ```csharp
  builder.Services.AddSingleton<IProductService, ProductService>();
  ```

### 5. 模型绑定和验证

- **功能**: 将HTTP请求数据绑定到模型并验证
- **特性**: 
  - 支持JSON、表单数据等多种格式
  - 支持数据注解验证
  - 支持自定义验证
- **使用示例**: 
  ```csharp
  app.MapPost("/api/products", async (Product product, IProductService productService)
      => Results.Created($"/api/products/{product.Id}", await productService.CreateAsync(product)))
      .Accepts<Product>("application/json")
      .Produces<Product>(StatusCodes.Status201Created)
      .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest);
  ```

## 使用示例

### 基础使用

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Carter@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Extensions.DependencyInjection;
using Carter;

// 定义产品模型
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// 定义产品服务
public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
}

// 实现产品服务
public class ProductService : IProductService
{
    private static readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "产品1", Price = 10.99m },
        new Product { Id = 2, Name = "产品2", Price = 19.99m }
    };
    
    public Task<List<Product>> GetAllAsync() => Task.FromResult(_products);
    
    public Task<Product?> GetByIdAsync(int id) => Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
    
    public Task<Product> CreateAsync(Product product)
    {
        product.Id = _products.Count + 1;
        _products.Add(product);
        return Task.FromResult(product);
    }
}

// 定义API模块
public class ProductModule : CarterModule
{
    public ProductModule() : base("api/products")
    {
    }
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // GET: /api/products
        app.MapGet("", async (IProductService productService) =>
        {
            var products = await productService.GetAllAsync();
            return Results.Ok(products);
        });
        
        // GET: /api/products/{id}
        app.MapGet("/{id:int}", async (int id, IProductService productService) =>
        {
            var product = await productService.GetByIdAsync(id);
            return product is not null ? Results.Ok(product) : Results.NotFound();
        });
        
        // POST: /api/products
        app.MapPost("", async (Product product, IProductService productService) =>
        {
            var createdProduct = await productService.CreateAsync(product);
            return Results.Created($"/api/products/{createdProduct.Id}", createdProduct);
        });
    }
}

// 主程序
var builder = WebApplication.CreateBuilder(args);

// 注册Carter服务
builder.Services.AddCarter();

// 注册产品服务
builder.Services.AddSingleton<IProductService, ProductService>();

var app = builder.Build();

// 映射Carter端点
app.MapCarter();

await app.RunAsync();
```

### 高级配置

```csharp
// 配置Carter服务
builder.Services.AddCarter(options =>
{
    // 注册所有CarterModule
    options.WithModule<ProductModule>();
    options.WithModule<OrderModule>();
    
    // 配置中间件
    options.WithDefaultModuleBindingMiddleware((builder) =>
    {
        builder.UseMiddleware<AuthenticationMiddleware>();
        builder.UseMiddleware<AuthorizationMiddleware>();
    });
    
    // 配置模型绑定
    options.WithModelBinder<CustomModelBinder>();
    
    // 配置过滤器
    options.WithActionFilter<LoggingActionFilter>();
});

// 配置应用
var app = builder.Build();

// 配置中间件
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// 映射Carter端点
app.MapCarter();

await app.RunAsync();
```

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
7. **使用 AOT 兼容的序列化**: 优先使用 System.Text.Json 等 AOT 兼容的序列化库

## 配置选项

### Carter 配置

| 配置项 | 类型 | 默认值 | 描述 |
|--------|------|--------|------|
| EnableAotOptimization | bool | false | 启用 AOT 优化 |
| EnableTrimOptimization | bool | false | 启用修剪优化 |
| EnableDetailedLogging | bool | false | 启用详细日志记录 |
| ModuleDiscoveryMode | enum | All | 模块发现模式（All、Manual） |

### 应用配置示例

```json
{
  "Carter": {
    "EnableAotOptimization": true,
    "EnableTrimOptimization": true,
    "EnableDetailedLogging": false,
    "ModuleDiscoveryMode": "All"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:5000"
      },
      "Https": {
        "Url": "https://localhost:5001"
      }
    }
  }
}
```

## 性能优化

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
   - 检查配置文件是否正确
   - 验证依赖项版本兼容性
   - 查看详细日志
   - 检查端口是否被占用

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

6. **依赖注入错误**
   - 检查服务注册是否正确
   - 验证依赖关系是否循环
   - 检查服务生命周期是否正确

## 扩展开发

### 创建自定义中间件

```csharp
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

### 创建自定义模型绑定器

```csharp
public class CustomModelBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }
        
        // 自定义模型绑定逻辑
        var modelName = bindingContext.ModelName;
        var valueProviderResult = await bindingContext.ValueProvider.GetValueAsync(modelName);
        
        if (valueProviderResult == ValueProviderResult.None)
        {
            return;
        }
        
        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
        
        var value = valueProviderResult.FirstValue;
        
        if (string.IsNullOrEmpty(value))
        {
            return;
        }
        
        // 解析值到模型
        var model = ParseValueToModel(value);
        
        bindingContext.Result = ModelBindingResult.Success(model);
    }
    
    private object ParseValueToModel(string value)
    {
        // 解析逻辑
        return new CustomModel { Value = value };
    }
}
```

### 创建自定义过滤器

```csharp
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

## 与其他系统集成

### 与 EF Core 集成

```csharp
// 配置 EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 注册存储库模式
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

// 在 API 模块中使用
public class ProductModule : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products", async (IProductService productService) =>
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

Carter 是一个基于 .NET 10 的现代化 API 框架，具有高性能、模块化、可扩展等特点。通过 Carter，开发者可以快速构建高性能、可维护的 Web API 和微服务，适用于各种规模的应用场景。

本参考文档提供了 Carter 的核心组件、使用示例、配置选项、性能优化建议、故障排除指南和扩展开发方法，帮助开发者充分利用 Carter 框架的优势，构建高质量的 API 服务。