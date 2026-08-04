# Identity - 使用示例

## Identity AOT 引擎示例

Identity AOT 引擎是一个基于 .NET 10 AOT 编译的高性能身份认证解决方案，以下是详细的使用示例。

## 快速开始

### 1. 基本使用示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Identity AOT 引擎 - 基本使用示例");
        Console.WriteLine("=" * 60);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var identityService = serviceProvider.GetRequiredService<IdentityService>();
        
        // 生成访问令牌
        Console.WriteLine("1. 生成访问令牌");
        var tokenResult = await identityService.GenerateTokenAsync("admin", "admin");
        Console.WriteLine($"访问令牌: {tokenResult.AccessToken.Substring(0, 50)}...");
        Console.WriteLine($"刷新令牌: {tokenResult.RefreshToken.Substring(0, 50)}...");
        Console.WriteLine($"过期时间: {tokenResult.ExpiresAt}");
        
        // 验证令牌
        Console.WriteLine("\n2. 验证访问令牌");
        var validationResult = await identityService.ValidateTokenAsync(tokenResult.AccessToken);
        Console.WriteLine($"验证状态: {(validationResult.Valid ? "有效" : "无效"}");
        if (validationResult.Valid)
        {
            Console.WriteLine($"用户名: {validationResult.Username}");
            Console.WriteLine($"角色: {validationResult.Role}");
        }
        
        // 解码令牌
        Console.WriteLine("\n3. 解码访问令牌");
        var decodeResult = await identityService.DecodeTokenAsync(tokenResult.AccessToken);
        Console.WriteLine($"解码状态: {(decodeResult.Success ? "成功" : "失败"}");
        if (decodeResult.Success)
        {
            Console.WriteLine($"用户名: {decodeResult.Username}");
            Console.WriteLine($"角色: {decodeResult.Role}");
            Console.WriteLine($"发行者: {decodeResult.Issuer}");
            Console.WriteLine($"受众: {decodeResult.Audience}");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        // 配置身份认证设置
        services.Configure<IdentitySettings>(options => {
            options.Issuer = "https://identity.example.com";
            options.Audience = "https://api.example.com";
            options.Key = "your-secret-key-here-change-in-production";
            options.TokenExpiry = TimeSpan.FromHours(1);
            options.RefreshTokenExpiry = TimeSpan.FromDays(7);
            options.EnableTokenRevocation = true;
            options.EnableCaching = true;
            options.CacheSize = 1000;
            options.CacheExpiry = TimeSpan.FromHours(1);
        });
        
        // 注册服务
        services.AddSingleton<IdentityService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        return services.BuildServiceProvider();
    }
}
```

### 2. 高级配置示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Identity AOT 引擎 - 高级配置示例");
        Console.WriteLine("=" * 60);
        
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置身份认证设置
        services.Configure<IdentitySettings>(options => {
            options.Issuer = "https://identity.example.com";
            options.Audience = "https://api.example.com";
            options.Key = "your-secret-key-here-change-in-production";
            options.TokenExpiry = TimeSpan.FromHours(2);
            options.RefreshTokenExpiry = TimeSpan.FromDays(14);
            options.EnableTokenRevocation = true;
            options.EnableCaching = true;
            options.CacheSize = 2000;
            options.CacheExpiry = TimeSpan.FromHours(2);
            options.Permissions = new Dictionary<string, List<string>> {
                { "admin", new List<string> { "read", "write", "delete", "admin" } },
                { "user", new List<string> { "read", "write" } },
                { "guest", new List<string> { "read" } },
                { "manager", new List<string> { "read", "write", "manage" } }
            };
        });
        
        // 注册服务
        services.AddSingleton<IdentityService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        
        var serviceProvider = services.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<IdentitySettings>>().Value;
        Console.WriteLine("配置信息:");
        Console.WriteLine($"发行者: {settings.Issuer}");
        Console.WriteLine($"受众: {settings.Audience}");
        Console.WriteLine($"令牌过期: {settings.TokenExpiry}");
        Console.WriteLine($"刷新令牌过期: {settings.RefreshTokenExpiry}");
        Console.WriteLine($"启用缓存: {settings.EnableCaching}");
        Console.WriteLine($"缓存大小: {settings.CacheSize}");
        Console.WriteLine($"权限配置: {settings.Permissions.Count} 个角色");
        
        // 使用服务
        var identityService = serviceProvider.GetRequiredService<IdentityService>();
        
        // 生成带自定义声明的令牌
        Console.WriteLine("\n生成带自定义声明的令牌");
        var claims = new List<System.Security.Claims.Claim> {
            new System.Security.Claims.Claim("department", "IT"),
            new System.Security.Claims.Claim("location", "Beijing"),
            new System.Security.Claims.Claim("email", "admin@example.com")
        };
        
        var customTokenResult = await identityService.GenerateTokenAsync("admin", "admin", claims);
        Console.WriteLine($"访问令牌: {customTokenResult.AccessToken.Substring(0, 50)}...");
        
        // 解码令牌查看自定义声明
        var decodeResult = await identityService.DecodeTokenAsync(customTokenResult.AccessToken);
        if (decodeResult.Success)
        {
            Console.WriteLine("\n令牌声明:");
            foreach (var claim in decodeResult.Claims)
            {
                Console.WriteLine($"  {claim.Type}: {claim.Value}");
            }
        }
    }
}
```

### 3. 用户认证与授权示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Identity AOT 引擎 - 用户认证与授权示例");
        Console.WriteLine("=" * 60);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var identityService = serviceProvider.GetRequiredService<IdentityService>();
        
        // 用户认证
        Console.WriteLine("1. 用户认证");
        var authResult = await identityService.AuthenticateUserAsync("admin", "admin123");
        Console.WriteLine($"认证状态: {(authResult.Success ? "成功" : "失败"}");
        if (authResult.Success)
        {
            Console.WriteLine($"用户名: {authResult.Username}");
            Console.WriteLine($"角色: {authResult.Role}");
            Console.WriteLine($"访问令牌: {authResult.AccessToken.Substring(0, 50)}...");
        }
        
        // 用户授权
        Console.WriteLine("\n2. 用户授权");
        if (authResult.Success)
        {
            // 测试管理员权限
            var adminAuthResult = await identityService.AuthorizeUserAsync(authResult.AccessToken, "admin");
            Console.WriteLine($"管理员权限: {(adminAuthResult.Success ? "授权通过" : "授权失败"}");
            
            // 测试普通用户权限
            var userAuthResult = await identityService.AuthorizeUserAsync(authResult.AccessToken, "write");
            Console.WriteLine($"写权限: {(userAuthResult.Success ? "授权通过" : "授权失败"}");
        }
        
        // 测试失败的认证
        Console.WriteLine("\n3. 失败的认证尝试");
        var failedAuthResult = await identityService.AuthenticateUserAsync("admin", "wrongpassword");
        Console.WriteLine($"认证状态: {(failedAuthResult.Success ? "成功" : "失败"}");
        if (!failedAuthResult.Success)
        {
            Console.WriteLine($"错误信息: {failedAuthResult.Error}");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.Configure<IdentitySettings>(options => {
            options.Issuer = "https://identity.example.com";
            options.Audience = "https://api.example.com";
            options.Key = "your-secret-key-here-change-in-production";
            options.TokenExpiry = TimeSpan.FromHours(1);
            options.RefreshTokenExpiry = TimeSpan.FromDays(7);
            options.EnableTokenRevocation = true;
            options.EnableCaching = true;
            options.Permissions = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> {
                { "admin", new System.Collections.Generic.List<string> { "read", "write", "delete", "admin" } },
                { "user", new System.Collections.Generic.List<string> { "read", "write" } },
                { "guest", new System.Collections.Generic.List<string> { "read" } }
            };
        });
        
        services.AddSingleton<IdentityService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        return services.BuildServiceProvider();
    }
}
```

### 4. 令牌管理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Identity AOT 引擎 - 令牌管理示例");
        Console.WriteLine("=" * 60);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var identityService = serviceProvider.GetRequiredService<IdentityService>();
        
        // 生成令牌
        Console.WriteLine("1. 生成访问令牌");
        var tokenResult = await identityService.GenerateTokenAsync("user", "user");
        var accessToken = tokenResult.AccessToken;
        var refreshToken = tokenResult.RefreshToken;
        Console.WriteLine($"访问令牌: {accessToken.Substring(0, 50)}...");
        Console.WriteLine($"刷新令牌: {refreshToken.Substring(0, 50)}...");
        
        // 验证令牌
        Console.WriteLine("\n2. 验证访问令牌");
        var validationResult = await identityService.ValidateTokenAsync(accessToken);
        Console.WriteLine($"验证状态: {(validationResult.Valid ? "有效" : "无效"}");
        
        // 撤销令牌
        Console.WriteLine("\n3. 撤销访问令牌");
        var revokeResult = await identityService.RevokeTokenAsync(accessToken);
        Console.WriteLine($"撤销状态: {(revokeResult.Success ? "成功" : "失败"}");
        
        // 验证已撤销的令牌
        Console.WriteLine("\n4. 验证已撤销的令牌");
        var revokedValidationResult = await identityService.ValidateTokenAsync(accessToken);
        Console.WriteLine($"验证状态: {(revokedValidationResult.Valid ? "有效" : "无效"}");
        if (!revokedValidationResult.Valid)
        {
            Console.WriteLine($"错误信息: {revokedValidationResult.Error}");
        }
        
        // 刷新令牌
        Console.WriteLine("\n5. 刷新访问令牌");
        var refreshResult = await identityService.RefreshTokenAsync(refreshToken);
        Console.WriteLine($"刷新状态: {(refreshResult.Success ? "成功" : "失败"}");
        if (refreshResult.Success)
        {
            Console.WriteLine($"新访问令牌: {refreshResult.AccessToken.Substring(0, 50)}...");
            Console.WriteLine($"新刷新令牌: {refreshResult.RefreshToken.Substring(0, 50)}...");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.Configure<IdentitySettings>(options => {
            options.Issuer = "https://identity.example.com";
            options.Audience = "https://api.example.com";
            options.Key = "your-secret-key-here-change-in-production";
            options.TokenExpiry = TimeSpan.FromHours(1);
            options.RefreshTokenExpiry = TimeSpan.FromDays(7);
            options.EnableTokenRevocation = true;
            options.EnableCaching = true;
        });
        
        services.AddSingleton<IdentityService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        return services.BuildServiceProvider();
    }
}
```

### 5. 性能基准测试示例

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Identity AOT 引擎 - 性能基准测试示例");
        Console.WriteLine("=" * 60);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var identityService = serviceProvider.GetRequiredService<IdentityService>();
        
        // 运行基准测试
        int iterations = 1000;
        Console.WriteLine($"运行 {iterations} 次迭代的基准测试...");
        
        var stopwatch = Stopwatch.StartNew();
        int successes = 0;
        int failures = 0;
        
        for (int i = 0; i < iterations; i++)
        {
            try
            {
                // 生成令牌
                var tokenResult = await identityService.GenerateTokenAsync($"user{i}", "user");
                
                // 验证令牌
                var validationResult = await identityService.ValidateTokenAsync(tokenResult.AccessToken);
                
                if (validationResult.Valid)
                {
                    successes++;
                }
                else
                {
                    failures++;
                }
            }
            catch
            {
                failures++;
            }
        }
        
        stopwatch.Stop();
        var elapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
        var operationsPerSecond = iterations / (stopwatch.Elapsed.TotalSeconds);
        
        Console.WriteLine("\n基准测试结果:");
        Console.WriteLine($"总用时: {elapsedMilliseconds:F3} ms");
        Console.WriteLine($"成功: {successes}");
        Console.WriteLine($"失败: {failures}");
        Console.WriteLine($"每秒操作数: {operationsPerSecond:F2} ops/s");
        Console.WriteLine($"平均每操作: {elapsedMilliseconds / iterations:F3} ms");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.Configure<IdentitySettings>(options => {
            options.Issuer = "https://identity.example.com";
            options.Audience = "https://api.example.com";
            options.Key = "your-secret-key-here-change-in-production";
            options.TokenExpiry = TimeSpan.FromHours(1);
            options.RefreshTokenExpiry = TimeSpan.FromDays(7);
            options.EnableTokenRevocation = false; // 禁用撤销以提高性能
            options.EnableCaching = true;
            options.CacheSize = 5000;
        });
        
        services.AddSingleton<IdentityService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Error); // 减少日志输出以提高性能
        });
        
        return services.BuildServiceProvider();
    }
}
```

## 命令行工具使用

Identity AOT 引擎提供了功能强大的命令行工具，以下是常用命令的使用示例：

### 1. 生成访问令牌

```bash
# 基本用法
identity_aot.exe generate admin admin

# 带自定义声明
identity_aot.exe generate user user department:IT location:Beijing
```

### 2. 验证访问令牌

```bash
# 验证令牌
identity_aot.exe validate eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 3. 解码访问令牌

```bash
# 解码令牌
identity_aot.exe decode eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 4. 撤销访问令牌

```bash
# 撤销令牌
identity_aot.exe revoke eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 5. 刷新访问令牌

```bash
# 刷新令牌
identity_aot.exe refresh CfDJ8...
```

### 6. 认证用户

```bash
# 认证用户
identity_aot.exe auth admin admin123
```

### 7. 授权用户

```bash
# 授权用户
identity_aot.exe authorize eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9... write
```

### 8. 运行基准测试

```bash
# 运行基准测试
identity_aot.exe benchmark 1000

# 运行更大型的基准测试
identity_aot.exe benchmark 10000
```

### 9. 显示配置信息

```bash
# 显示配置
identity_aot.exe config
```

### 10. 显示帮助信息

```bash
# 显示帮助
identity_aot.exe help

# 简写形式
identity_aot.exe h
identity_aot.exe ?
```

## 集成示例

### ASP.NET Core 集成

```csharp
// Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 配置 JWT 认证
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://identity.example.com",
            ValidAudience = "https://api.example.com",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key-here-change-in-production"))
        };
    });

// 注册 Identity 服务
builder.Services.Configure<IdentitySettings>(options => {
    options.Issuer = "https://identity.example.com";
    options.Audience = "https://api.example.com";
    options.Key = "your-secret-key-here-change-in-production";
    options.TokenExpiry = TimeSpan.FromHours(1);
    options.RefreshTokenExpiry = TimeSpan.FromDays(7);
});
builder.Services.AddSingleton<IdentityService>();

// 添加授权
builder.Services.AddAuthorization();

var app = builder.Build();

// 使用认证和授权
app.UseAuthentication();
app.UseAuthorization();

// 示例 API 端点
app.MapGet("/api/protected", [Authorize] () => {
    return Results.Ok(new { message = "受保护的资源" });
});

app.MapPost("/api/login", async (LoginRequest request, IdentityService identityService) => {
    var authResult = await identityService.AuthenticateUserAsync(request.Username, request.Password);
    if (authResult.Success)
    {
        return Results.Ok(new {
            accessToken = authResult.AccessToken,
            refreshToken = authResult.RefreshToken
        });
    }
    return Results.Unauthorized();
});

app.Run();

record LoginRequest(string Username, string Password);
```

## 总结

以上示例展示了 Identity AOT 引擎的主要功能和使用方法，通过这些示例，您可以：

1. 快速上手基本操作
2. 配置高级选项
3. 实现用户认证与授权
4. 管理 JWT 令牌生命周期
5. 运行性能基准测试
6. 与 ASP.NET Core 集成

Identity AOT 引擎采用 .NET 10 最佳实践设计，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。通过 AOT 编译优化，它提供了卓越的性能和可靠性，是构建现代身份认证系统的理想选择。
