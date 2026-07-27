# aspire - 使用示例

## 快速入门

### 1. 基本 aspire 应用开发示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Aspire.Hosting@8.0.0
#:package Aspire.Dashboard@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("基本 aspire 应用开发示例");
        Console.WriteLine("=" * 50);
        
        try
        {
            // 创建分布式应用构建器
            var builder = DistributedApplication.CreateBuilder(args);
            
            // 添加简单的 HTTP 服务
            var apiService = builder.AddProject<Projects.MyApi>("api")
                .WithHttpEndpoint(port: 5000, isDefault: true)
                .WithHealthChecks();
            
            // 配置服务环境变量
            apiService.WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");
            
            // 构建应用
            var app = builder.Build();
            
            Console.WriteLine("启动 aspire 应用...");
            Console.WriteLine("API 服务地址: http://localhost:5000");
            Console.WriteLine("aspire 仪表板: http://localhost:18888");
            
            // 运行应用
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"应用启动失败: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}

// 项目引用（实际项目中会自动生成）
namespace Projects
{
    public record MyApi { }
}
```

### 2. 带数据库的 aspire 应用示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Aspire.Hosting@8.0.0
#:package Aspire.Dashboard@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("带数据库的 aspire 应用示例");
        Console.WriteLine("=" * 50);
        
        // 创建分布式应用构建器
        var builder = DistributedApplication.CreateBuilder(args);
        
        // 添加 PostgreSQL 数据库服务
        var postgres = builder.AddPostgres("postgres")
            .WithPassword("postgres")
            .WithDatabase("mydb")
            .WithPersistence();
        
        // 添加 MySQL 数据库服务
        var mysql = builder.AddMySql("mysql")
            .WithPassword("mysql")
            .WithDatabase("mydb")
            .WithPersistence();
        
        // 添加使用 PostgreSQL 的 API 服务
        var postgresApi = builder.AddProject<Projects.PostgresApi>("postgres-api")
            .WithReference(postgres)
            .WithHttpEndpoint();
        
        // 添加使用 MySQL 的 API 服务
        var mysqlApi = builder.AddProject<Projects.MySqlApi>("mysql-api")
            .WithReference(mysql)
            .WithHttpEndpoint();
        
        // 添加前端服务
        var frontend = builder.AddProject<Projects.Frontend>("frontend")
            .WithReference(postgresApi)
            .WithReference(mysqlApi)
            .WithHttpEndpoint(isDefault: true);
        
        // 构建并运行应用
        var app = builder.Build();
        await app.RunAsync();
    }
}

// 项目引用（实际项目中会自动生成）
namespace Projects
{
    public record PostgresApi { }
    public record MySqlApi { }
    public record Frontend { }
}
```

### 3. 带 Redis 缓存的 aspire 应用示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Aspire.Hosting@8.0.0
#:package Aspire.Dashboard@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("带 Redis 缓存的 aspire 应用示例");
        Console.WriteLine("=" * 50);
        
        // 创建分布式应用构建器
        var builder = DistributedApplication.CreateBuilder(args);
        
        // 添加 Redis 缓存服务
        var redis = builder.AddRedis("redis")
            .WithRedisConfiguration("Cache", "0")
            .WithPersistence()
            .WithMemoryLimit("512MB")
            .WithCpuLimit(0.5);
        
        // 添加 API 服务并引用 Redis
        var api = builder.AddProject<Projects.CacheApi>("cache-api")
            .WithReference(redis)
            .WithHttpEndpoint()
            .WithReplicas(2);
        
        // 添加后台工作服务并引用 Redis
        var worker = builder.AddProject<Projects.CacheWorker>("cache-worker")
            .WithReference(redis);
        
        // 构建并运行应用
        var app = builder.Build();
        await app.RunAsync();
    }
}

// 项目引用（实际项目中会自动生成）
namespace Projects
{
    public record CacheApi { }
    public record CacheWorker { }
}
```

### 4. 多服务 aspire 应用示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Aspire.Hosting@8.0.0
#:package Aspire.Dashboard@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Collections.Generic;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("多服务 aspire 应用示例");
        Console.WriteLine("=" * 50);
        
        // 创建分布式应用构建器
        var builder = DistributedApplication.CreateBuilder(args);
        
        // 配置全局设置
        builder.Configuration.AddJsonFile("appsettings.custom.json", optional: true);
        
        // 添加基础服务
        var redis = builder.AddRedis("redis")
            .WithRedisConfiguration("Default", "0")
            .WithPersistence();
        
        var postgres = builder.AddPostgres("postgres")
            .WithPassword("postgres")
            .WithDatabase("mydb");
        
        // 添加微服务
        var userService = builder.AddProject<Projects.UserService>("user-service")
            .WithReference(postgres)
            .WithReference(redis)
            .WithHttpEndpoint();
        
        var productService = builder.AddProject<Projects.ProductService>("product-service")
            .WithReference(postgres)
            .WithReference(redis)
            .WithHttpEndpoint();
        
        var orderService = builder.AddProject<Projects.OrderService>("order-service")
            .WithReference(postgres)
            .WithReference(redis)
            .WithReference(userService)
            .WithReference(productService)
            .WithHttpEndpoint();
        
        var paymentService = builder.AddProject<Projects.PaymentService>("payment-service")
            .WithReference(postgres)
            .WithReference(orderService)
            .WithHttpEndpoint();
        
        // 添加 API 网关
        var gateway = builder.AddProject<Projects.ApiGateway>("gateway")
            .WithReference(userService)
            .WithReference(productService)
            .WithReference(orderService)
            .WithReference(paymentService)
            .WithHttpEndpoint(isDefault: true)
            .WithReplicas(3);
        
        // 添加监控服务
        var monitoring = builder.AddProject<Projects.MonitoringService>("monitoring")
            .WithReference(gateway)
            .WithHttpEndpoint();
        
        // 构建并运行应用
        var app = builder.Build();
        await app.RunAsync();
    }
}

// 项目引用（实际项目中会自动生成）
namespace Projects
{
    public record UserService { }
    public record ProductService { }
    public record OrderService { }
    public record PaymentService { }
    public record ApiGateway { }
    public record MonitoringService { }
}
```

## AOT 编译示例

### AOT 编译的 aspire 应用

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Aspire.Hosting@8.0.0
#:package Aspire.Dashboard@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=Full

using System;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("AOT 编译的 aspire 应用示例");
        Console.WriteLine("=" * 50);
        
        try
        {
            // 创建分布式应用构建器
            var builder = DistributedApplication.CreateBuilder(args);
            
            // 添加 AOT 编译的 API 服务
            var api = builder.AddProject<Projects.AotApi>("aot-api")
                .WithHttpEndpoint()
                .WithHealthChecks();
            
            // 配置 API 服务使用 AOT 编译
            api.WithEnvironment("DOTNET_PUBLISH_AOT", "1");
            
            // 添加 Redis 服务
            var redis = builder.AddRedis("redis");
            api.WithReference(redis);
            
            // 构建并运行应用
            var app = builder.Build();
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"应用启动失败: {ex.Message}");
        }
    }
}

// 项目引用（实际项目中会自动生成）
namespace Projects
{
    public record AotApi { }
}
```

### AOT 编译配置文件 (csproj)

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net11.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    
    <!-- AOT 编译配置 -->
    <PublishAot>true</PublishAot>
    <TrimMode>Full</TrimMode>
    <PublishReadyToRun>true</PublishReadyToRun>
    <PublishSingleFile>true</PublishSingleFile>
    <SelfContained>true</SelfContained>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.0" />
    <PackageReference Include="Aspire.Hosting" Version="8.0.0" />
    <PackageReference Include="Aspire.Dashboard" Version="8.0.0" />
  </ItemGroup>
</Project>
```

### AOT 编译命令

```bash
# 编译 aspire 应用为 Windows x64 原生可执行文件
dotnet publish -c Release -r win-x64 --self-contained

# 编译 aspire 应用为 Linux x64 原生可执行文件
dotnet publish -c Release -r linux-x64 --self-contained

# 运行 AOT 编译后的应用
./bin/Release/net11.0/win-x64/publish/AspireApp.exe
```

### AOT 优化的服务配置

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Aspire.Hosting@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("AOT 优化的服务配置示例");
        Console.WriteLine("=" * 50);
        
        var builder = DistributedApplication.CreateBuilder(args);
        
        // 添加 AOT 优化的服务
        var aotService = builder.AddProject<Projects.AotOptimizedService>("aot-service")
            .WithHttpEndpoint()
            .WithHealthChecks()
            
            // AOT 优化配置
            .WithEnvironment("DOTNET_EnableAOTCompilation", "1")
            .WithEnvironment("DOTNET_TrimMode", "Full")
            .WithEnvironment("DOTNET_RuntimeOptimizations", "1")
            
            // 资源优化配置
            .WithMemoryLimit("256MB")
            .WithCpuLimit(0.5)
            
            // 性能优化配置
            .WithEnvironment("ASPNETCORE_FORWARDHEADERS_ENABLED", "true")
            .WithEnvironment("ASPNETCORE_SERVER_KESTREL__ENDPOINTS__HTTP__PROTOCOLS", "Http1AndHttp2")
            .WithEnvironment("ASPNETCORE_SERVER_KESTREL__Limits__MaxConcurrentConnections", "1000")
            .WithEnvironment("ASPNETCORE_SERVER_KESTREL__Limits__MaxConcurrentUpgradedConnections", "100");
        
        // 构建并运行应用
        var app = builder.Build();
        await app.RunAsync();
    }
}

// 项目引用（实际项目中会自动生成）
namespace Projects
{
    public record AotOptimizedService { }
}
```

## 与其他技术集成示例

### 与 ASP.NET Core Minimal API 集成

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Aspire.Hosting@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // 添加 aspire 服务默认配置
        builder.AddServiceDefaults();
        
        // 配置数据库连接
        var connectionString = builder.Configuration.GetConnectionString("postgres")
            ?? throw new InvalidOperationException("PostgreSQL connection string not found");
        
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        // 添加 Redis 缓存
        builder.Services.AddStackExchangeRedisCache(options =>
            options.Configuration = builder.Configuration.GetConnectionString("redis"));
        
        // 添加健康检查
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>()
            .AddRedis(builder.Configuration.GetConnectionString("redis"));
        
        // 构建应用
        var app = builder.Build();
        
        // 使用 aspire 默认端点
        app.MapDefaultEndpoints();
        
        // 配置 CORS
        app.UseCors(policy =>
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader());
        
        // 定义 API 端点
        app.MapGet("/", () => Results.Ok(new {
            Message = "Hello from ASP.NET Core Minimal API with aspire!",
            Environment = app.Environment.EnvironmentName
        }));
        
        app.MapGet("/api/users", async (AppDbContext db) =>
            await db.Users.ToListAsync());
        
        app.MapGet("/api/users/{id}", async (int id, AppDbContext db) =>
            await db.Users.FindAsync(id) is User user
                ? Results.Ok(user)
                : Results.NotFound());
        
        app.MapPost("/api/users", async (User user, AppDbContext db) =>
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Results.Created($"/api/users/{user.Id}", user);
        });
        
        app.Run();
    }
}

// 数据库上下文
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users => Set<User>();
}

// 用户实体
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

## 总结

上述示例展示了 aspire 技能的主要功能和使用方法，包括：

1. **基本 aspire 应用开发**: 使用声明式 API 定义和部署分布式应用
2. **带数据库的应用**: 集成 PostgreSQL 和 MySQL 数据库服务
3. **带缓存的应用**: 集成 Redis 缓存服务
4. **多服务架构**: 构建复杂的微服务架构
5. **AOT 编译支持**: 将应用编译为本机代码，提高性能
6. **与 ASP.NET Core 集成**: 与 ASP.NET Core Minimal API 无缝集成

所有示例均遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适合各种规模和复杂度的项目。aspire 提供了统一的方式来定义、配置和部署分布式应用，简化了云原生应用的开发和管理。
