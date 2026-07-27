# Carter - 使用示例

## 快速开始

### 1. 基础Carter使用示例

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

// 定义API模块
public class HelloWorldModule : CarterModule
{
    public HelloWorldModule() : base("api")
    {
        // 配置模块
    }
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // GET: /api/hello
        app.MapGet("/hello", () => Results.Ok(new { Message = "Hello, Carter!" }))
           .WithName("GetHello")
           .WithTags("Hello")
           .Produces<object>(StatusCodes.Status200OK);
        
        // GET: /api/hello/{name}
        app.MapGet("/hello/{name}", (string name) => Results.Ok(new { Message = $"Hello, {name}!" }))
           .WithName("GetHelloByName")
           .WithTags("Hello")
           .Produces<object>(StatusCodes.Status200OK);
           .Produces(StatusCodes.Status400BadRequest);
    }
}

// 主程序
var builder = WebApplication.CreateBuilder(args);

// 注册Carter服务
builder.Services.AddCarter();

var app = builder.Build();

// 映射Carter端点
app.MapCarter();

await app.RunAsync();
```

### 2. AOT编译示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Aot@10.0.0
#:package Carter@8.0.0
#:package Swashbuckle.AspNetCore@7.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=Full
#:property PublishReadyToRun=true
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Carter;

// AOT安全的设置
public class AotSettings
{
    public bool EnableAotOptimization { get; set; } = true;
    public bool EnableTrimOptimization { get; set; } = true;
}

// AOT安全的产品模型
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// AOT安全的产品服务接口
public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<Product?> UpdateAsync(int id, Product product);
    Task<bool> DeleteAsync(int id);
}

// AOT安全的产品服务实现
public class ProductService : IProductService
{
    private readonly Dictionary<int, Product> _products = new();
    private int _nextId = 1;
    
    public ProductService()
    {
        // 添加初始数据
        _products.Add(_nextId++, new Product { Name = "产品1", Price = 10.99m, IsActive = true });
        _products.Add(_nextId++, new Product { Name = "产品2", Price = 19.99m, IsActive = true });
    }
    
    public Task<List<Product>> GetAllAsync()
    {
        return Task.FromResult(_products.Values.ToList());
    }
    
    public Task<Product?> GetByIdAsync(int id)
    {
        return Task.FromResult(_products.TryGetValue(id, out var product) ? product : null);
    }
    
    public Task<Product> CreateAsync(Product product)
    {
        product.Id = _nextId++;
        _products[product.Id] = product;
        return Task.FromResult(product);
    }
    
    public Task<Product?> UpdateAsync(int id, Product product)
    {
        if (!_products.TryGetValue(id, out var existingProduct))
        {
            return Task.FromResult<Product?>(null);
        }
        
        product.Id = id;
        _products[id] = product;
        return Task.FromResult(product);
    }
    
    public Task<bool> DeleteAsync(int id)
    {
        return Task.FromResult(_products.Remove(id));
    }
}

// AOT安全的API模块
public class ProductModule : CarterModule
{
    public ProductModule() : base("api/products")
    {
        // 配置模块
    }
    
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
        .Produces<Product>(StatusCodes.Status201Created)
        .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest);
        
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
        .Produces(StatusCodes.Status404NotFound)
        .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest);
        
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

// 主程序
var builder = WebApplication.CreateBuilder(args);

// 配置AOT设置
builder.Services.Configure<AotSettings>(options => {
    options.EnableAotOptimization = true;
    options.EnableTrimOptimization = true;
});

// 注册Carter服务
builder.Services.AddCarter();

// 注册产品服务
builder.Services.AddSingleton<IProductService, ProductService>();

// 配置Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 启用Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 配置中间件
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// 映射Carter端点
app.MapCarter();

await app.RunAsync();
```

### 3. 模块化API设计示例

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

// 产品相关模型
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// 订单相关模型
public class Order
{
    public int Id { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class OrderItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

// 产品API模块
public class ProductModule : CarterModule
{
    public ProductModule() : base("api/products")
    {
    }
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("", () => Results.Ok(new List<Product> {
            new Product { Id = 1, Name = "产品1", Price = 10.99m },
            new Product { Id = 2, Name = "产品2", Price = 19.99m }
        }));
    }
}

// 订单API模块
public class OrderModule : CarterModule
{
    public OrderModule() : base("api/orders")
    {
    }
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("", () => Results.Ok(new List<Order> {
            new Order {
                Id = 1,
                Items = new List<OrderItem> {
                    new OrderItem { ProductId = 1, Quantity = 2, UnitPrice = 10.99m }
                },
                Total = 21.98m
            }
        }));
        
        app.MapPost("", (Order order) => Results.Created($"/api/orders/{1}", order));
    }
}

// 健康检查模块
public class HealthModule : CarterModule
{
    public HealthModule() : base("health")
    {
    }
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));
    }
}

// 主程序
var builder = WebApplication.CreateBuilder(args);

// 注册Carter服务
builder.Services.AddCarter(options =>
{
    // 显式注册模块
    options.WithModule<ProductModule>();
    options.WithModule<OrderModule>();
    options.WithModule<HealthModule>();
});

var app = builder.Build();

// 映射Carter端点
app.MapCarter();

await app.RunAsync();
```

### 4. 中间件使用示例

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
using Microsoft.Extensions.Logging;
using Carter;

// 自定义日志中间件
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;
    
    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation($"Request started: {context.Request.Method} {context.Request.Path}");
        
        // 记录请求开始时间
        var startTime = DateTime.UtcNow;
        
        try
        {
            // 执行下一个中间件
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Request error: {context.Request.Method} {context.Request.Path}");
            throw;
        }
        finally
        {
            // 计算请求持续时间
            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation($"Request completed: {context.Request.Method} {context.Request.Path} - {context.Response.StatusCode} in {duration.TotalMilliseconds:F2} ms");
        }
    }
}

// 自定义认证中间件
public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    
    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // 检查认证头
        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            // 简单的认证检查
            if (authHeader.ToString() == "Bearer secret-token")
            {
                // 认证通过，设置用户信息
                context.Items["User"] = new { Id = 1, Name = "测试用户" };
                await _next(context);
                return;
            }
        }
        
        // 认证失败
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Unauthorized");
    }
}

// API模块
public class ProtectedModule : CarterModule
{
    public ProtectedModule() : base("api/protected")
    {
    }
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // 需要认证的路由
        app.MapGet("", (HttpContext context) =>
        {
            var user = context.Items["User"];
            return Results.Ok(new { Message = "这是受保护的API", User = user });
        });
    }
}

// 主程序
var builder = WebApplication.CreateBuilder(args);

// 注册Carter服务
builder.Services.AddCarter();

var app = builder.Build();

// 配置中间件
app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();

// 映射Carter端点
app.MapCarter();

await app.RunAsync();
```

### 5. 错误处理示例

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

// 自定义异常
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string message, Exception inner) : base(message, inner) { }
}

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
    public ValidationException(string message, Exception inner) : base(message, inner) { }
}

// 产品服务
public class ProductService
{
    public Product GetProduct(int id)
    {
        if (id <= 0)
        {
            throw new ValidationException("产品ID必须大于0");
        }
        
        if (id == 1)
        {
            return new Product { Id = 1, Name = "产品1", Price = 10.99m };
        }
        
        throw new NotFoundException($"找不到ID为{id}的产品");
    }
}

// 产品API模块
public class ProductModule : CarterModule
{
    public ProductModule() : base("api/products")
    {
    }
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:int}", (int id, ProductService productService) =>
        {
            try
            {
                var product = productService.GetProduct(id);
                return Results.Ok(product);
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return Results.NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        });
    }
}

// 全局异常处理中间件
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "验证错误");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { Error = ex.Message });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "资源未找到");
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "服务器错误");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { Error = "服务器内部错误" });
        }
    }
}

// 主程序
var builder = WebApplication.CreateBuilder(args);

// 注册服务
builder.Services.AddSingleton<ProductService>();

// 注册Carter服务
builder.Services.AddCarter();

var app = builder.Build();

// 配置中间件
app.UseMiddleware<GlobalExceptionMiddleware>();

// 映射Carter端点
app.MapCarter();

await app.RunAsync();
```

### 6. 与EF Core集成示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Carter@8.0.0
#:package Microsoft.EntityFrameworkCore@8.0.0
#:package Microsoft.EntityFrameworkCore.InMemory@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Carter;

// 数据库上下文
public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 配置模型关系
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne();
        
        // 种子数据
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "产品1", Price = 10.99m },
            new Product { Id = 2, Name = "产品2", Price = 19.99m }
        );
    }
}

// 产品模型
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// 订单模型
public class Order
{
    public int Id { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

// 订单API模块
public class OrderModule : CarterModule
{
    public OrderModule() : base("api/orders")
    {
    }
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // GET: /api/orders
        app.MapGet("", async (AppDbContext db) =>
        {
            var orders = await db.Orders.Include(o => o.Items).ToListAsync();
            return Results.Ok(orders);
        });
        
        // GET: /api/orders/{id}
        app.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var order = await db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
            return order is not null ? Results.Ok(order) : Results.NotFound();
        });
        
        // POST: /api/orders
        app.MapPost("", async (Order order, AppDbContext db) =>
        {
            // 计算订单总额
            order.Total = order.Items.Sum(item => item.Quantity * item.UnitPrice);
            
            db.Orders.Add(order);
            await db.SaveChangesAsync();
            
            return Results.Created($"/api/orders/{order.Id}", order);
        });
    }
}

// 主程序
var builder = WebApplication.CreateBuilder(args);

// 配置EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TestDb"));

// 注册Carter服务
builder.Services.AddCarter();

var app = builder.Build();

// 映射Carter端点
app.MapCarter();

await app.RunAsync();
```

## 总结

以上示例演示了Carter技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速开始使用Carter构建API
2. 了解如何配置AOT编译
3. 学习模块化API设计
4. 掌握中间件的使用
5. 实现错误处理
6. 与EF Core等ORM框架集成

这些示例遵循.NET 10最佳实践，具有良好的可扩展性和可维护性，适合各种规模的项目。所有示例都支持AOT编译，可以编译为本机代码以获得更高的性能和更快的启动速度。