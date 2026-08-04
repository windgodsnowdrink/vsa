# Scrutor 技能使用示例文档

## 1. 基础使用示例

### 1.1 扫描程序集并注册服务

**功能说明**：扫描程序集并注册符合条件的服务。

**命令示例**：
```bash
# 扫描当前目录下的所有程序集，注册所有以 Service 结尾的类型
scrutor_core scan --assembly "." --pattern "*Service" --lifetime "scoped"
```

**执行结果**：
```
成功扫描 5 个程序集
成功注册 12 个服务
- IUserService -> UserService (Scoped)
- IProductService -> ProductService (Scoped)
- IOrderService -> OrderService (Scoped)
- UserService -> UserService (Scoped)
- ProductService -> ProductService (Scoped)
- OrderService -> OrderService (Scoped)
- IEmailService -> EmailService (Scoped)
- EmailService -> EmailService (Scoped)
- ISmsService -> SmsService (Scoped)
- SmsService -> SmsService (Scoped)
- ILoggerService -> LoggerService (Scoped)
- LoggerService -> LoggerService (Scoped)
```

### 1.2 扫描指定程序集

**功能说明**：扫描指定的程序集并注册服务。

**命令示例**：
```bash
# 扫描指定的程序集，注册所有以 Repository 结尾的类型
scrutor_core scan --assembly "MyApp.Services.dll" --pattern "*Repository" --lifetime "singleton"
```

**执行结果**：
```
成功扫描 1 个程序集
成功注册 8 个服务
- IUserRepository -> UserRepository (Singleton)
- IProductRepository -> ProductRepository (Singleton)
- IOrderRepository -> OrderRepository (Singleton)
- UserRepository -> UserRepository (Singleton)
- ProductRepository -> ProductRepository (Singleton)
- OrderRepository -> OrderRepository (Singleton)
- ICacheRepository -> CacheRepository (Singleton)
- CacheRepository -> CacheRepository (Singleton)
```

### 1.3 为服务添加装饰器

**功能说明**：为已注册的服务添加装饰器。

**命令示例**：
```bash
# 为 IUserService 添加日志装饰器
scrutor_core decorate --service "IUserService" --decorator "UserServiceLoggingDecorator" --lifetime "scoped"
```

**执行结果**：
```
成功为服务 IUserService 添加装饰器 UserServiceLoggingDecorator
```

### 1.4 列出已注册的服务

**功能说明**：列出所有已注册的服务。

**命令示例**：
```bash
# 以文本格式列出服务
scrutor_core list --format "text"
```

**执行结果**：
```
已注册的服务 (20):
--------------------------------------------------------------------------------------------
| 服务类型                                 | 实现类型                                 | 生命周期       |
--------------------------------------------------------------------------------------------
| IUserService                            | UserServiceLoggingDecorator              | Scoped        |
| IProductService                         | ProductService                           | Scoped        |
| IOrderService                           | OrderService                             | Scoped        |
| UserService                             | UserService                              | Scoped        |
| ProductService                          | ProductService                           | Scoped        |
| OrderService                            | OrderService                             | Scoped        |
| IEmailService                           | EmailService                             | Scoped        |
| EmailService                            | EmailService                             | Scoped        |
| ISmsService                             | SmsService                               | Scoped        |
| SmsService                              | SmsService                               | Scoped        |
| ILoggerService                          | LoggerService                            | Scoped        |
| LoggerService                           | LoggerService                            | Scoped        |
| IUserRepository                         | UserRepository                           | Singleton     |
| IProductRepository                      | ProductRepository                        | Singleton     |
| IOrderRepository                        | OrderRepository                          | Singleton     |
| UserRepository                          | UserRepository                           | Singleton     |
| ProductRepository                       | ProductRepository                        | Singleton     |
| OrderRepository                         | OrderRepository                          | Singleton     |
| ICacheRepository                        | CacheRepository                          | Singleton     |
| CacheRepository                         | CacheRepository                          | Singleton     |
--------------------------------------------------------------------------------------------
```

### 1.5 以 JSON 格式列出服务

**功能说明**：以 JSON 格式列出所有已注册的服务。

**命令示例**：
```bash
# 以 JSON 格式列出服务
scrutor_core list --format "json"
```

**执行结果**：
```json
[
  {
    "ServiceType": "IUserService",
    "ImplementationType": "UserServiceLoggingDecorator",
    "Lifetime": "Scoped"
  },
  {
    "ServiceType": "IProductService",
    "ImplementationType": "ProductService",
    "Lifetime": "Scoped"
  },
  {
    "ServiceType": "IOrderService",
    "ImplementationType": "OrderService",
    "Lifetime": "Scoped"
  }
  // 其他服务...
]
```

## 2. 高级使用示例

### 2.1 生成依赖注入配置代码

**功能说明**：生成依赖注入配置代码。

**命令示例**：
```bash
# 生成依赖注入配置代码
scrutor_generator generate --output "DependencyInjection.cs" --namespace "MyApp.DependencyInjection" --class "DependencyInjection"
```

**执行结果**：
```
成功生成代码到: DependencyInjection.cs
生成的代码行数: 35
```

**生成的代码**：
```csharp
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.DependencyInjection
{
    /// <summary>
    /// 依赖注入配置
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// 添加依赖项
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            // 注册服务
            // 示例：
            // services.AddScoped<IService, Service>();
            // services.AddSingleton<IRepository, Repository>();
            // services.AddTransient<IFactory, Factory>();
            
            // 使用 Scrutor 扫描程序集
            // services.Scan(scan => scan
            //     .FromAssemblyOf<Program>()
            //     .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
            //     .AsImplementedInterfaces()
            //     .WithScopedLifetime()
            // );
            
            // 添加装饰器
            // services.Decorate<IService, ServiceDecorator>();
            
            return services;
        }
        
        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="assemblyPattern">程序集匹配模式</param>
        /// <param name="typePattern">类型匹配模式</param>
        /// <param name="lifetime">服务生命周期</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection ConfigureServices(this IServiceCollection services, string assemblyPattern = "*", string typePattern = "*Service", ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            // 注册服务
            // 示例：
            // services.Scan(scan => scan
            //     .FromAssembliesOfType<IService>()
            //     .AddClasses(classes => classes.Where(type => type.Name.Contains(typePattern)))
            //     .AsImplementedInterfaces()
            //     .WithLifetime(lifetime)
            // );
            
            return services;
        }
    }
}
```

### 2.2 生成服务集合配置代码

**功能说明**：生成服务集合配置代码。

**命令示例**：
```bash
# 生成服务集合配置代码
scrutor_generator generate --output "ServiceCollectionConfig.cs" --namespace "MyApp.Configuration" --class "ServiceCollectionConfig"
```

**执行结果**：
```
成功生成代码到: ServiceCollectionConfig.cs
生成的代码行数: 30
```

**生成的代码**：
```csharp
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Configuration
{
    /// <summary>
    /// 服务集合配置
    /// </summary>
    public class ServiceCollectionConfig
    {
        /// <summary>
        /// 配置服务
        /// </summary>
        /// <returns>服务集合</returns>
        public static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();
            
            // 注册服务
            // 示例：
            // services.AddScoped<IService, Service>();
            // services.AddSingleton<IRepository, Repository>();
            // services.AddTransient<IFactory, Factory>();
            
            // 使用 Scrutor 扫描程序集
            // services.Scan(scan => scan
            //     .FromApplicationDependencies()
            //     .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
            //     .AsImplementedInterfaces()
            //     .WithScopedLifetime()
            // );
            
            // 添加装饰器
            // services.Decorate<IService, ServiceDecorator>();
            
            return services;
        }
        
        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="assemblyPattern">程序集匹配模式</param>
        /// <param name="typePattern">类型匹配模式</param>
        /// <param name="lifetime">服务生命周期</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection ConfigureServices(string assemblyPattern = "*", string typePattern = "*Service", ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var services = new ServiceCollection();
            
            // 注册服务
            // 示例：
            // services.Scan(scan => scan
            //     .FromAssembliesInPath(Directory.GetCurrentDirectory())
            //     .AddClasses(classes => classes.Where(type => type.Name.Contains(typePattern)))
            //     .AsImplementedInterfaces()
            //     .WithLifetime(lifetime)
            // );
            
            return services;
        }
    }
}
```

### 2.3 生成 Web 应用配置代码

**功能说明**：生成 Web 应用配置代码。

**命令示例**：
```bash
# 生成 Web 应用配置代码
scrutor_generator generate --output "WebApplicationConfig.cs" --namespace "MyApp.Configuration" --class "WebApplicationConfig"
```

**执行结果**：
```
成功生成代码到: WebApplicationConfig.cs
生成的代码行数: 50
```

**生成的代码**：
```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MyApp.Configuration
{
    /// <summary>
    /// Web 应用配置
    /// </summary>
    public class WebApplicationConfig
    {
        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="builder">Web 应用构建器</param>
        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            // 注册服务
            // 示例：
            // builder.Services.AddScoped<IService, Service>();
            // builder.Services.AddSingleton<IRepository, Repository>();
            // builder.Services.AddTransient<IFactory, Factory>();
            
            // 使用 Scrutor 扫描程序集
            // builder.Services.Scan(scan => scan
            //     .FromAssemblyOf<Program>()
            //     .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
            //     .AsImplementedInterfaces()
            //     .WithScopedLifetime()
            // );
            
            // 添加装饰器
            // builder.Services.Decorate<IService, ServiceDecorator>();
            
            // 配置其他服务
            // builder.Services.AddControllers();
            // builder.Services.AddSwaggerGen();
            // builder.Services.AddAuthentication();
            // builder.Services.AddAuthorization();
        }
        
        /// <summary>
        /// 配置管道
        /// </summary>
        /// <param name="app">Web 应用</param>
        public static void ConfigurePipeline(WebApplication app)
        {
            // 配置中间件
            // if (app.Environment.IsDevelopment())
            // {
            //     app.UseSwagger();
            //     app.UseSwaggerUI();
            // }
            
            // app.UseHttpsRedirection();
            // app.UseAuthentication();
            // app.UseAuthorization();
            // app.MapControllers();
        }
        
        /// <summary>
        /// 构建 Web 应用
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>Web 应用</returns>
        public static WebApplication BuildWebApplication(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // 配置服务
            ConfigureServices(builder);
            
            var app = builder.Build();
            
            // 配置管道
            ConfigurePipeline(app);
            
            return app;
        }
    }
}
```

### 2.4 列出可用的代码模板

**功能说明**：列出所有可用的代码模板。

**命令示例**：
```bash
# 列出可用的代码模板
scrutor_generator template list
```

**执行结果**：
```
可用的代码模板:
--------------------------------------------------------------------------------
模板: dependency-injection
描述: 依赖注入配置代码模板
类型: dependency-injection
--------------------------------------------------------------------------------
模板: service-collection
描述: 服务集合配置代码模板
类型: service-collection
--------------------------------------------------------------------------------
模板: web-application
描述: Web 应用配置代码模板
类型: web-application
--------------------------------------------------------------------------------
```

## 2. 高级使用示例

### 2.1 多层装饰器

**功能说明**：为服务添加多个装饰器，形成装饰器链。

**命令示例**：
```bash
# 为 IUserService 添加日志装饰器
scrutor_core decorate --service "IUserService" --decorator "UserServiceLoggingDecorator" --lifetime "scoped"

# 为 IUserService 添加缓存装饰器
scrutor_core decorate --service "IUserService" --decorator "UserServiceCachingDecorator" --lifetime "scoped"

# 为 IUserService 添加事务装饰器
scrutor_core decorate --service "IUserService" --decorator "UserServiceTransactionDecorator" --lifetime "scoped"
```

**执行结果**：
```
成功为服务 IUserService 添加装饰器 UserServiceLoggingDecorator
成功为服务 IUserService 添加装饰器 UserServiceCachingDecorator
成功为服务 IUserService 添加装饰器 UserServiceTransactionDecorator
```

### 2.2 批量扫描和注册

**功能说明**：批量扫描多个程序集并注册服务。

**示例脚本**：
```powershell
# 批量扫描多个程序集
$assemblies = @(".\MyApp.Services.dll", ".\MyApp.Repositories.dll", ".\MyApp.Infrastructure.dll")

foreach ($assembly in $assemblies) {
    Write-Host "扫描程序集: $assembly"
    $command = "scrutor_core scan --assembly `"$assembly`" --pattern `"*Service`" --lifetime `"scoped`""
    Invoke-Expression $command
}
```

**执行结果**：
```
扫描程序集: .\MyApp.Services.dll
成功扫描 1 个程序集
成功注册 8 个服务

扫描程序集: .\MyApp.Repositories.dll
成功扫描 1 个程序集
成功注册 6 个服务

扫描程序集: .\MyApp.Infrastructure.dll
成功扫描 1 个程序集
成功注册 4 个服务
```

### 2.3 自定义扫描模式

**功能说明**：使用自定义的扫描模式注册服务。

**命令示例**：
```bash
# 扫描程序集，注册所有实现了 ICommandHandler 接口的类型
scrutor_core scan --assembly "." --pattern "*CommandHandler" --lifetime "transient"
```

**执行结果**：
```
成功扫描 3 个程序集
成功注册 10 个服务
- ICreateUserCommandHandler -> CreateUserCommandHandler (Transient)
- IUpdateUserCommandHandler -> UpdateUserCommandHandler (Transient)
- IDeleteUserCommandHandler -> DeleteUserCommandHandler (Transient)
- CreateUserCommandHandler -> CreateUserCommandHandler (Transient)
- UpdateUserCommandHandler -> UpdateUserCommandHandler (Transient)
- DeleteUserCommandHandler -> DeleteUserCommandHandler (Transient)
- ICreateProductCommandHandler -> CreateProductCommandHandler (Transient)
- CreateProductCommandHandler -> CreateProductCommandHandler (Transient)
- IUpdateProductCommandHandler -> UpdateProductCommandHandler (Transient)
- UpdateProductCommandHandler -> UpdateProductCommandHandler (Transient)
```

## 3. 实际应用场景示例

### 3.1 Web 应用集成

**场景描述**：在 ASP.NET Core Web 应用中集成 Scrutor，自动扫描和注册服务。

**示例代码**：

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyApp.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 使用 Scrutor 生成的扩展方法注册服务
builder.Services.AddDependencies();

// 配置其他服务
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

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

app.Run();
```

**依赖注入配置** (DependencyInjection.cs)：

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            // 使用 Scrutor 扫描程序集
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            );
            
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
            );
            
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("CommandHandler")))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );
            
            // 添加装饰器
            services.Decorate<IUserService, UserServiceLoggingDecorator>();
            services.Decorate<IUserService, UserServiceCachingDecorator>();
            
            return services;
        }
    }
}
```

### 3.2 控制台应用集成

**场景描述**：在控制台应用中集成 Scrutor，自动扫描和注册服务。

**示例代码**：

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyApp.Configuration;

// 使用 Scrutor 生成的服务集合配置
var services = ServiceCollectionConfig.ConfigureServices();

// 构建服务提供者
var serviceProvider = services.BuildServiceProvider();

// 解析服务
var userService = serviceProvider.GetRequiredService<IUserService>();
var productService = serviceProvider.GetRequiredService<IProductService>();

// 使用服务
await userService.CreateUserAsync(new User { Name = "John Doe", Email = "john@example.com" });
await productService.CreateProductAsync(new Product { Name = "Product 1", Price = 99.99 });

Console.WriteLine("操作完成!");
```

**服务集合配置** (ServiceCollectionConfig.cs)：

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Configuration
{
    public class ServiceCollectionConfig
    {
        public static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();
            
            // 使用 Scrutor 扫描程序集
            services.Scan(scan => scan
                .FromApplicationDependencies()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            );
            
            services.Scan(scan => scan
                .FromApplicationDependencies()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
            );
            
            return services;
        }
    }
}
```

### 3.3 工作器服务集成

**场景描述**：在 .NET 工作器服务中集成 Scrutor，自动扫描和注册服务。

**示例代码**：

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyApp.DependencyInjection;

var builder = Host.CreateDefaultBuilder(args);

// 配置服务
builder.ConfigureServices((hostContext, services) =>
{
    // 使用 Scrutor 生成的扩展方法注册服务
    services.AddDependencies();
    
    // 添加工作器服务
    services.AddHostedService<Worker>();
});

var host = builder.Build();
await host.RunAsync();

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IUserService _userService;
    private readonly IProductService _productService;

    public Worker(ILogger<Worker> logger, IUserService userService, IProductService productService)
    {
        _logger = logger;
        _userService = userService;
        _productService = productService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            
            // 使用服务
            await _userService.SyncUsersAsync();
            await _productService.SyncProductsAsync();
            
            await Task.Delay(10000, stoppingToken);
        }
    }
}
```

## 4. 常见问题解决方案

### 4.1 程序集扫描失败

**问题描述**：程序集扫描失败，显示无法加载程序集。

**解决方案**：

1. 检查程序集路径是否正确：
   ```bash
   # 检查路径是否存在
   ls -la "MyApp.Services.dll"
   ```

2. 检查程序集是否损坏：
   ```bash
   # 使用 ILDASM 检查程序集
   ildasm /all /text MyApp.Services.dll > MyApp.Services.il
   ```

3. 检查依赖项是否缺失：
   ```bash
   # 使用 dotnet 命令检查依赖项
   dotnet list MyApp.Services.dll package
   ```

### 4.2 服务注册失败

**问题描述**：服务注册失败，显示类型不存在或不匹配。

**解决方案**：

1. 检查类型是否存在：
   ```bash
   # 使用反射检查类型
   dotnet reflection MyApp.Services.dll | grep "UserService"
   ```

2. 检查类型匹配模式是否正确：
   ```bash
   # 调整匹配模式
   scrutor_core scan --assembly "." --pattern "User*" --lifetime "scoped"
   ```

3. 检查循环依赖：
   ```bash
   # 检查服务之间的依赖关系
   dotnet dependencyscope MyApp.Services.dll
   ```

### 4.3 装饰器添加失败

**问题描述**：装饰器添加失败，显示装饰器不实现服务接口。

**解决方案**：

1. 检查装饰器是否实现了服务接口：
   ```csharp
   // 确保装饰器实现了服务接口
   public class UserServiceDecorator : IUserService
   {
       private readonly IUserService _decorated;
       
       public UserServiceDecorator(IUserService decorated)
       {
           _decorated = decorated;
       }
       
       // 实现接口方法
   }
   ```

2. 检查装饰器类型名称是否正确：
   ```bash
   # 确保装饰器类型名称正确
   scrutor_core decorate --service "IUserService" --decorator "MyApp.Services.Decorators.UserServiceDecorator" --lifetime "scoped"
   ```

### 4.4 代码生成失败

**问题描述**：代码生成失败，显示输出路径不存在。

**解决方案**：

1. 检查输出路径是否存在：
   ```bash
   # 创建输出目录
   mkdir -p "generated"
   ```

2. 检查权限是否足够：
   ```bash
   # 检查权限
   ls -la "generated"
   ```

3. 检查配置是否正确：
   ```bash
   # 确保配置正确
   scrutor_generator generate --output "generated/DependencyInjection.cs" --namespace "MyApp.DependencyInjection" --class "DependencyInjection"
   ```

## 5. 性能优化示例

### 5.1 优化程序集扫描

**功能说明**：优化程序集扫描，提高扫描速度。

**示例代码**：

```csharp
// 优化后的程序集扫描
services.Scan(scan => scan
    // 只扫描必要的程序集
    .FromAssemblyOf<UserService>()
    .FromAssemblyOf<ProductService>()
    // 使用具体的匹配模式
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    // 只注册为实现的接口
    .AsImplementedInterfaces()
    // 使用合适的生命周期
    .WithScopedLifetime()
);
```

### 5.2 优化服务注册

**功能说明**：优化服务注册，减少注册次数。

**示例代码**：

```csharp
// 批量注册服务
services.Scan(scan => scan
    .FromApplicationDependencies()
    // 批量添加类
    .AddClasses(classes => classes
        .Where(type => type.Name.EndsWith("Service") || 
                      type.Name.EndsWith("Repository") || 
                      type.Name.EndsWith("CommandHandler"))
    )
    // 批量注册
    .AsImplementedInterfaces()
    // 根据类型设置生命周期
    .WithLifetime(ServiceLifetime.Scoped)
);
```

### 5.3 优化装饰器链

**功能说明**：优化装饰器链，减少装饰器的数量和复杂度。

**示例代码**：

```csharp
// 优化装饰器链
// 1. 创建一个复合装饰器，包含多个横切关注点
public class UserServiceCompositeDecorator : IUserService
{
    private readonly IUserService _decorated;
    private readonly ILogger<UserServiceCompositeDecorator> _logger;
    private readonly ICacheService _cache;
    private readonly ITransactionService _transaction;

    public UserServiceCompositeDecorator(
        IUserService decorated,
        ILogger<UserServiceCompositeDecorator> logger,
        ICacheService cache,
        ITransactionService transaction)
    {
        _decorated = decorated;
        _logger = logger;
        _cache = cache;
        _transaction = transaction;
    }

    // 实现接口方法，包含所有横切关注点
}

// 2. 只添加一个装饰器
scrutor_core decorate --service "IUserService" --decorator "UserServiceCompositeDecorator" --lifetime "scoped"
```

## 5. 性能优化示例

### 5.1 并行扫描程序集

**功能说明**：使用并行扫描提高程序集扫描速度。

**示例脚本**：

```powershell
# 并行扫描多个程序集
$assemblies = @(".\MyApp.Services.dll", ".\MyApp.Repositories.dll", ".\MyApp.Infrastructure.dll")

# 并行执行
$assemblies | ForEach-Object -Parallel {
    Write-Host "扫描程序集: $_"
    $command = "scrutor_core scan --assembly `"$_`" --pattern `"*Service`" --lifetime `"scoped`""
    Invoke-Expression $command
} -ThrottleLimit 3
```

**性能提升**：
- 串行扫描：15 秒
- 并行扫描：5 秒

### 5.2 缓存扫描结果

**功能说明**：缓存扫描结果，避免重复扫描。

**示例代码**：

```csharp
// 缓存扫描结果
public class CachedAssemblyScanner : IAssemblyScanner
{
    private readonly IAssemblyScanner _inner;
    private readonly Dictionary<string, List<ServiceDescriptor>> _cache = new Dictionary<string, List<ServiceDescriptor>>();
    
    public CachedAssemblyScanner(IAssemblyScanner inner)
    {
        _inner = inner;
    }
    
    public async Task<Assembly[]> LoadAssembliesAsync(string path, CancellationToken cancellationToken = default)
    {
        return await _inner.LoadAssembliesAsync(path, cancellationToken);
    }
    
    public async Task<List<ServiceDescriptor>> ScanAssembliesAsync(Assembly[] assemblies, string pattern, string lifetime, CancellationToken cancellationToken = default)
    {
        var key = $"{string.Join(",", assemblies.Select(a => a.FullName))}:{pattern}:{lifetime}";
        
        if (_cache.TryGetValue(key, out var cached))
        {
            return cached;
        }
        
        var result = await _inner.ScanAssembliesAsync(assemblies, pattern, lifetime, cancellationToken);
        _cache[key] = result;
        
        return result;
    }
}
```

### 5.3 优化代码生成

**功能说明**：优化代码生成，减少生成时间和代码体积。

**示例命令**：
```bash
# 生成最小化的代码
scrutor_generator generate --output "DependencyInjection.Minimal.cs" --namespace "MyApp.DependencyInjection" --class "DependencyInjection"
```

**生成的代码**：
```csharp
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(c => c.Where(t => t.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            );
            
            return services;
        }
    }
}
```

## 6. 总结

本示例文档提供了 Scrutor 技能的各种使用场景和示例，包括基础使用、高级使用、实际应用场景、常见问题解决方案和性能优化示例。通过这些示例，您可以更好地理解和使用 Scrutor 技能，为您的应用程序添加可靠的依赖注入功能。

Scrutor 技能支持三种主要功能：
- **程序集扫描**：自动扫描程序集并注册服务
- **服务装饰**：为服务添加装饰器，实现横切关注点
- **代码生成**：生成依赖注入配置代码，简化配置过程

通过合理配置和使用 Scrutor 技能，您可以实现自动化的依赖注入配置，提高应用程序的可维护性和可扩展性。同时，Scrutor 技能支持 AOT 编译，提供了更好的性能和更小的部署包，适合各种规模的应用程序。

希望本示例文档能够帮助您更好地使用 Scrutor 技能，为您的 .NET 应用程序开发提供便利。