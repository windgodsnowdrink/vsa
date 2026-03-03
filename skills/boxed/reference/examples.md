# boxed - 使用示例

## 快速开始

### 1. GraphQL 模板使用示例

```csharp
using HotChocolate.AspNetCore;
using HotChocolate.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// 实体模型
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public bool InStock { get; set; }
}

// 数据库上下文
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Product> Products { get; set; }
}

// 查询类型
public class Query
{
    // 查询所有产品
    [UseDbContext(typeof(AppDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Product> GetProducts([ScopedService] AppDbContext context) => context.Products;
    
    // 根据 ID 查询产品
    [UseDbContext(typeof(AppDbContext))]
    public Product GetProduct([ScopedService] AppDbContext context, int id) => 
        context.Products.FirstOrDefault(p => p.Id == id);
}

// 变更类型
public class Mutation
{
    // 创建产品
    [UseDbContext(typeof(AppDbContext))]
    public async Task<Product> CreateProduct([ScopedService] AppDbContext context, Product product)
    {
        context.Products.Add(product);
        await context.SaveChangesAsync();
        return product;
    }
}

// 主程序
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // 配置数据库
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("boxed_graphql_db"));
        
        // 添加 GraphQL 服务
        builder.Services.AddGraphQLServer()
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            .AddFiltering()
            .AddSorting()
            .AddProjections()
            .AddInMemorySubscriptions();
        
        var app = builder.Build();
        
        // 配置中间件
        app.UseRouting();
        app.UseWebSockets();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGraphQL();
        });
        
        app.Run();
    }
}
```

### 2. Web API 模板使用示例

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Annotations;

// 实体模型
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime CreatedAt { get; set; }
}

// 数据库上下文
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Customer> Customers { get; set; }
}

// 数据传输对象
public class CustomerDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}

// API 控制器
[ApiController]
[Route("[controller]")]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public CustomersController(AppDbContext context)
    {
        _context = context;
    }
    
    // GET: api/customers
    [HttpGet]
    [SwaggerOperation(Summary = "获取所有客户", Description = "返回系统中所有客户的列表")]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        return await _context.Customers.ToListAsync();
    }
    
    // GET: api/customers/5
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "根据 ID 获取客户", Description = "返回指定 ID 的客户详细信息")]
    public async Task<ActionResult<Customer>> GetCustomer(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
        {
            return NotFound();
        }
        return customer;
    }
    
    // POST: api/customers
    [HttpPost]
    [SwaggerOperation(Summary = "创建新客户", Description = "在系统中创建一个新客户")]
    public async Task<ActionResult<Customer>> PostCustomer(CustomerDto customerDto)
    {
        var customer = new Customer
        {
            Name = customerDto.Name,
            Email = customerDto.Email,
            Phone = customerDto.Phone,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
    }
}

// 主程序
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // 配置数据库
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("boxed_webapi_db"));
        
        // 添加服务
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
        });
        builder.Services.AddHealthChecks();
        
        var app = builder.Build();
        
        // 配置中间件
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/health");
        
        app.Run();
    }
}
```

### 3. AOT 优化的 Web API 示例

```csharp
// 启用 AOT 优化的 Web API 示例
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// 简单的 API 响应模型
public record ApiResponse<T>(bool Success, T Data, string Message = null);

// 高性能服务
public interface IWeatherService
{
    Task<ApiResponse<IEnumerable<string>>> GetWeatherForecast();
}

// 服务实现
public class WeatherService : IWeatherService
{
    // 使用 AggressiveOptimization 提示编译器优化
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public Task<ApiResponse<IEnumerable<string>>> GetWeatherForecast()
    {
        var forecasts = new string[]
        {
            "晴天，温度 25°C",
            "多云，温度 22°C",
            "小雨，温度 18°C",
            "阴天，温度 20°C",
            "晴朗，温度 28°C"
        };
        
        return Task.FromResult(new ApiResponse<IEnumerable<string>>(true, forecasts));
    }
}

// 最小 API 控制器
[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    
    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }
    
    [HttpGet]
    public Task<ApiResponse<IEnumerable<string>>> Get() => _weatherService.GetWeatherForecast();
}

// 主程序
public class Program
{
    // 使用 AggressiveOptimization 提示编译器优化
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // 添加服务
        builder.Services.AddSingleton<IWeatherService, WeatherService>();
        builder.Services.AddControllers();
        
        var app = builder.Build();
        
        // 配置中间件
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        // 添加最小 API 端点
        app.MapGet("/", () => "Boxed Web API with AOT Support");
        
        app.Run();
    }
}
```

### 4. Orleans 分布式应用示例

```csharp
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Threading.Tasks;

// Grain 接口
public interface IHelloGrain : IGrainWithStringKey
{
    Task<string> SayHello(string name);
    Task<int> IncrementCount();
}

// Grain 实现
public class HelloGrain : Grain, IHelloGrain
{
    private int _count = 0;
    
    public Task<string> SayHello(string name)
    {
        return Task.FromResult($"你好，{name}！这是来自 Orleans Grain 的问候。");
    }
    
    public Task<int> IncrementCount()
    {
        _count++;
        return Task.FromResult(_count);
    }
}

// 服务器程序
public class OrleansServer
{
    public static async Task RunServerAsync()
    {
        var host = new SiloHostBuilder()
            .UseLocalhostClustering()
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "boxed-orleans-cluster";
                options.ServiceId = "boxed-orleans-service";
            })
            .Configure<EndpointOptions>(options =>
            {
                options.AdvertisedIPAddress = System.Net.IPAddress.Loopback;
            })
            .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
            .Build();
        
        await host.StartAsync();
        Console.WriteLine("Orleans 服务器已启动");
        Console.WriteLine("按 Enter 键停止服务器...");
        Console.ReadLine();
        
        await host.StopAsync();
    }
}

// 客户端程序
public class OrleansClient
{
    public static async Task RunClientAsync()
    {
        var client = new ClientBuilder()
            .UseLocalhostClustering()
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "boxed-orleans-cluster";
                options.ServiceId = "boxed-orleans-service";
            })
            .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IHelloGrain).Assembly).WithReferences())
            .Build();
        
        await client.Connect();
        Console.WriteLine("已连接到 Orleans 集群");
        
        // 使用 Grain
        var helloGrain = client.GetGrain<IHelloGrain>("user1");
        var response = await helloGrain.SayHello("张三");
        Console.WriteLine($"Grain 响应: {response}");
        
        // 测试计数器
        var count1 = await helloGrain.IncrementCount();
        Console.WriteLine($"计数器值: {count1}");
        
        var count2 = await helloGrain.IncrementCount();
        Console.WriteLine($"计数器值: {count2}");
        
        await client.Close();
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        // 启动服务器
        var serverTask = OrleansServer.RunServerAsync();
        
        // 等待服务器启动
        await Task.Delay(2000);
        
        // 运行客户端
        await OrleansClient.RunClientAsync();
        
        // 等待服务器停止
        await serverTask;
    }
}
```

## 总结

以上示例展示了 Boxed 模板技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速开始使用 GraphQL 模板构建高性能 API
2. 使用 Web API 模板构建 RESTful 服务
3. 实现 AOT 优化的高性能 Web API
4. 构建 Orleans 分布式应用

所有示例都基于 .NET 10，支持 AOT 编译优化，遵循 .NET 最佳实践。您可以根据业务需求选择合适的模板，并根据需要进行扩展和定制。

Boxed 模板技能提供了完整的 .NET 10 模板解决方案，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的 .NET 项目。
