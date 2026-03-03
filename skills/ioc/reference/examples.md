# IOC 技能使用示例

## 快速开始

### 1. 基本使用示例

#### 1.1 服务注册示例

```bash
# 注册一个 transient 生命周期的服务
ioc_aot register System.String transient

# 注册一个 singleton 生命周期的服务
ioc_aot register System.Collections.Generic.List`1[[System.String]] singleton

# 注册一个 scoped 生命周期的服务
ioc_aot register System.IO.MemoryStream scoped
```

**预期输出**：
```
注册服务: System.String, 生命周期: transient
服务注册完成! 用时: 12.345 ms
```

#### 1.2 服务解析示例

```bash
# 解析已注册的服务
ioc_aot resolve System.String

# 解析不同类型的服务
ioc_aot resolve System.Collections.Generic.List`1[[System.String]]
```

**预期输出**：
```
解析服务: System.String
服务解析完成! 用时: 5.678 ms
解析结果: System.String
```

#### 1.3 容器构建示例

```bash
# 构建 IOC 容器
ioc_aot build
```

**预期输出**：
```
构建容器...
容器构建完成! 用时: 8.901 ms
注册的服务数量: 3
```

### 2. 配置管理示例

#### 2.1 查看当前配置

```bash
# 显示 IOC 容器配置
ioc_aot config
```

**预期输出**：
```
IOC 配置:
默认生命周期: transient
自动装配: True
循环依赖检测: True
日志记录: True
最大注册深度: 10
基准测试: True
```

#### 2.2 通过环境变量配置

```powershell
# 设置环境变量覆盖默认配置
$env:IOC_DEFAULT_LIFETIME = "scoped"
$env:IOC_ENABLE_AUTO_WIRING = "true"
$env:IOC_MAX_REGISTRATION_DEPTH = "20"

# 查看更新后的配置
ioc_aot config
```

**预期输出**：
```
IOC 配置:
默认生命周期: scoped
自动装配: True
循环依赖检测: True
日志记录: True
最大注册深度: 20
基准测试: True
```

### 3. 性能基准测试示例

#### 3.1 运行基准测试

```bash
# 运行 1000 次迭代的基准测试
ioc_aot benchmark 1000

# 运行 5000 次迭代的基准测试
ioc_aot benchmark 5000
```

**预期输出**：
```
运行基准测试，迭代次数: 1000
基准测试完成! 平均解析时间: 0.123 ms
每秒操作数: 8130.08 ops/s
```

#### 3.2 性能优化建议

根据基准测试结果，可以采取以下优化措施：

1. **对于频繁使用的服务**：使用 singleton 生命周期
2. **对于轻量级服务**：使用 transient 生命周期
3. **对于请求范围的服务**：使用 scoped 生命周期
4. **批量注册**：减少容器构建次数

### 4. 高级使用示例

#### 4.1 完整工作流示例

```bash
# 步骤 1: 注册多个服务
ioc_aot register System.String singleton
ioc_aot register System.Int32 transient
ioc_aot register System.DateTime scoped

# 步骤 2: 构建容器
ioc_aot build

# 步骤 3: 解析服务
ioc_aot resolve System.String
ioc_aot resolve System.Int32
ioc_aot resolve System.DateTime

# 步骤 4: 运行基准测试
ioc_aot benchmark 2000
```

**预期输出**：
```
注册服务: System.String, 生命周期: singleton
服务注册完成! 用时: 10.123 ms

注册服务: System.Int32, 生命周期: transient
服务注册完成! 用时: 8.456 ms

注册服务: System.DateTime, 生命周期: scoped
服务注册完成! 用时: 9.789 ms

构建容器...
容器构建完成! 用时: 15.321 ms
注册的服务数量: 3

解析服务: System.String
服务解析完成! 用时: 4.567 ms
解析结果: System.String

解析服务: System.Int32
服务解析完成! 用时: 3.210 ms
解析结果: System.Int32

解析服务: System.DateTime
服务解析完成! 用时: 4.890 ms
解析结果: System.DateTime

运行基准测试，迭代次数: 2000
基准测试完成! 平均解析时间: 0.098 ms
每秒操作数: 10204.08 ops/s
```

#### 4.2 自定义服务示例

假设我们有以下自定义服务：

```csharp
// MyService.cs
public interface IMyService
{
    string GetMessage();
}

public class MyService : IMyService
{
    public string GetMessage() => "Hello from MyService!";
}
```

**使用示例**：

```bash
# 注册自定义服务
ioc_aot register MyService singleton
ioc_aot register IMyService,MyService singleton

# 构建容器
ioc_aot build

# 解析服务
ioc_aot resolve MyService
ioc_aot resolve IMyService

# 运行基准测试
ioc_aot benchmark 1000
```

### 5. 错误处理示例

#### 5.1 服务注册错误

```bash
# 尝试注册不存在的类型
ioc_aot register Non.Existent.Type singleton
```

**预期输出**：
```
错误: 类型不存在: Non.Existent.Type
```

#### 5.2 服务解析错误

```bash
# 尝试解析未注册的服务
ioc_aot resolve Non.Existent.Type
```

**预期输出**：
```
错误: 类型不存在: Non.Existent.Type
```

### 6. 命令别名使用示例

```bash
# 使用命令别名

# 注册服务 (r = register)
ioc_aot r System.String singleton

# 解析服务 (res = resolve)
ioc_aot res System.String

# 构建容器 (b = build)
ioc_aot b

# 显示配置 (co = config)
ioc_aot co

# 运行基准测试 (bm = benchmark)
ioc_aot bm 1000

# 显示帮助 (h = help)
ioc_aot h
```

**预期输出**：
```
注册服务: System.String, 生命周期: singleton
服务注册完成! 用时: 11.234 ms

解析服务: System.String
服务解析完成! 用时: 4.567 ms
解析结果: System.String

构建容器...
容器构建完成! 用时: 7.890 ms
注册的服务数量: 1

IOC 配置:
默认生命周期: transient
自动装配: True
循环依赖检测: True
日志记录: True
最大注册深度: 10
基准测试: True

运行基准测试，迭代次数: 1000
基准测试完成! 平均解析时间: 0.112 ms
每秒操作数: 8928.57 ops/s

IOC AOT 容器 命令帮助:
============================================================
register (r)     - 注册服务
resolve (res)    - 解析服务
build (b)        - 构建容器
config (co)      - 显示配置
benchmark (bm)   - 运行基准测试
help (h, ?)      - 显示帮助信息
```

## 实际应用场景示例

### 1. 控制台应用程序

```csharp
// Program.cs
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("控制台应用程序 IOC 示例");
        Console.WriteLine("=" * 60);
        
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 注册服务
        services.AddTransient<ILogger, ConsoleLogger>();
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<IConfiguration, AppConfiguration>();
        
        var serviceProvider = services.BuildServiceProvider();
        
        // 解析并使用服务
        using (var scope = serviceProvider.CreateScope())
        {
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
            
            logger.Log("开始处理用户数据...");
            var users = await userService.GetUsersAsync();
            logger.Log($"获取到 {users.Count} 个用户");
        }
    }
}

// 服务接口和实现
public interface ILogger { void Log(string message); }
public class ConsoleLogger : ILogger { public void Log(string message) => Console.WriteLine($"[LOG] {message}"); }

public interface IUserService { Task<List<User>> GetUsersAsync(); }
public class UserService : IUserService
{
    private readonly ILogger _logger;
    public UserService(ILogger logger) => _logger = logger;
    public async Task<List<User>> GetUsersAsync()
    {
        _logger.Log("从数据库获取用户...");
        await Task.Delay(100); // 模拟异步操作
        return new List<User> { new User { Id = 1, Name = "张三" }, new User { Id = 2, Name = "李四" } };
    }
}

public class User { public int Id { get; set; } public string Name { get; set; } }
public interface IConfiguration { string GetSetting(string key); }
public class AppConfiguration : IConfiguration { public string GetSetting(string key) => "Value";
}
```

### 2. Web 应用程序

```csharp
// Startup.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // 注册服务
        services.AddTransient<ILogger, FileLogger>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddSingleton<IConfiguration, AppConfiguration>();
        services.AddSingleton<ICacheService, MemoryCacheService>();
        
        // 注册控制器
        services.AddControllers();
    }
    
    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints => {
            endpoints.MapControllers();
        });
    }
}

// UserController.cs
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger _logger;
    
    public UserController(IUserService userService, ILogger logger)
    {
        _userService = userService;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        _logger.Log("获取用户列表");
        var users = await _userService.GetUsersAsync();
        return Ok(users);
    }
}
```

## 总结

通过以上示例，您可以了解如何：

1. **快速开始**：使用基本命令进行服务注册和解析
2. **配置管理**：通过环境变量和配置文件管理容器配置
3. **性能优化**：运行基准测试评估容器性能
4. **高级使用**：注册和解析自定义服务
5. **错误处理**：处理常见的错误情况
6. **命令别名**：使用命令别名简化操作
7. **实际应用**：在控制台和 Web 应用程序中使用 IOC 容器

IOC 技能设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。
