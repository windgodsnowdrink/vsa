# auth - 参考文档

## 概述

auth 是一个基于 .NET 10 的高性能认证授权系统，专为 .NET 开发者设计，提供完整的身份验证、授权管理、单点登录、权限控制等功能，支持 AOT 编译优化，适用于各种规模的应用程序。

## 核心组件

### 1. 认证服务 (Auth Service)
- **位置**: scripts/ 目录下的 .cs 文件
- **功能**: 认证授权的核心业务逻辑处理
- **特性**: 
  - 核心认证授权功能实现
  - 高性能设计和优化
  - 完善的错误处理机制
  - 详细的日志记录
  - 支持 AOT 编译
  - 模块化架构设计

### 2. 身份验证提供程序 (Authentication Provider)
- **功能**: 提供多种身份验证方式
- **支持的认证方式**: 
  - JWT (JSON Web Token)
  - OAuth 2.0
  - OpenID Connect
  - 用户名/密码
  - 证书认证
  - 多因素认证

### 3. 授权管理器 (Authorization Manager)
- **功能**: 提供授权管理和访问控制
- **支持的授权模型**: 
  - 基于角色的访问控制 (RBAC)
  - 基于属性的访问控制 (ABAC)
  - 基于策略的授权
  - 基于声明的授权

### 4. 令牌服务 (Token Service)
- **功能**: 提供令牌的生成、验证和管理
- **支持的令牌类型**: 
  - JWT (JSON Web Token)
  - Refresh Token
  - Access Token
  - ID Token

### 5. SSO 集成服务 (SSO Integration Service)
- **功能**: 提供与各种 SSO 解决方案的集成
- **支持的 SSO 解决方案**: 
  - Keycloak
  - OpenIddict
  - TheIdServer
  - CASBIN
  - 自定义 SSO 实现

### 6. 权限服务 (Permission Service)
- **功能**: 提供细粒度的权限管理
- **特性**: 
  - 支持资源级权限控制
  - 支持操作级权限控制
  - 支持动态权限分配
  - 支持权限继承

## 功能特性

### 1. 身份验证
- 支持多种身份验证方式
- 支持自定义身份验证提供程序
- 支持多因素认证
- 支持社交登录集成
- 支持联合身份验证

### 2. 授权管理
- 支持多种授权模型
- 支持自定义授权策略
- 支持基于资源的授权
- 支持基于属性的授权
- 支持授权规则的动态更新

### 3. 单点登录 (SSO)
- 支持多种 SSO 协议
- 支持与主流 SSO 解决方案集成
- 支持跨域 SSO
- 支持集中式身份管理

### 4. 令牌管理
- 支持 JWT 令牌的生成和验证
- 支持令牌刷新机制
- 支持令牌撤销
- 支持令牌过期管理
- 支持令牌签名和加密

### 5. API 保护
- 提供 API 授权和保护机制
- 支持基于角色的 API 访问控制
- 支持基于策略的 API 访问控制
- 支持 API 密钥认证
- 支持 OAuth 2.0 保护 API

### 6. 多因素认证
- 支持多种多因素认证方式
- 支持短信验证码
- 支持邮件验证码
- 支持 TOTP (基于时间的一次性密码)
- 支持硬件令牌

### 7. 审计日志
- 支持记录认证授权事件
- 支持审计日志查询和分析
- 支持审计日志导出
- 支持审计日志加密

### 8. AOT 编译支持
- 支持将认证应用编译为本机代码
- 提高启动速度和运行性能
- 减少内存占用
- 支持多种平台和架构

## 使用示例

### 基础使用

```csharp
// 获取认证服务
var authService = serviceProvider.GetRequiredService<IAuthService>();

// 验证用户凭据
var result = await authService.ValidateCredentialsAsync("username", "password");
if (result.IsValid) {
    // 生成 JWT 令牌
    var token = await authService.GenerateTokenAsync(result.User);
    Console.WriteLine($"JWT 令牌: {token}");
}
```

### 高级配置

```csharp
// 配置认证服务
var builder = WebApplication.CreateBuilder();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}) .AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "https://auth.example.com",
        ValidAudience = "https://api.example.com",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKey"))
    };
    
    // 配置令牌刷新
    options.Events = new JwtBearerEvents {
        OnTokenValidated = async context => {
            var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
            var principal = context.Principal;
            if (await tokenService.ShouldRefreshTokenAsync(principal)) {
                var newToken = await tokenService.RefreshTokenAsync(principal);
                context.Response.Headers.Append("X-Refresh-Token", newToken);
            }
        }
    };
});

// 配置授权服务
builder.Services.AddAuthorization(options => {
    options.AddPolicy("AdminPolicy", policy => 
        policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => 
        policy.RequireRole("User"));
    options.AddPolicy("AgePolicy", policy => 
        policy.RequireClaim(ClaimTypes.DateOfBirth, "1990-01-01", DateTime.Now.AddYears(-18).ToString("yyyy-MM-dd")));
});
```

## 配置选项

### 认证服务配置

```json
{
  "Auth": {
    "Jwt": {
      "Issuer": "https://auth.example.com",
      "Audience": "https://api.example.com",
      "Key": "YourSecretKey",
      "ExpirationMinutes": 60,
      "RefreshTokenExpirationDays": 7
    },
    "MultiFactorAuthentication": {
      "Enabled": true,
      "Methods": ["Sms", "Email", "TOTP"],
      "SmsProvider": "Twilio",
      "EmailProvider": "SendGrid"
    },
    "Cache": {
      "Enabled": true,
      "Size": 10000,
      "ExpirationMinutes": 30
    },
    "Logging": {
      "Enabled": true,
      "Detailed": false
    }
  }
}
```

## 性能优化

### 1. 缓存使用
- 启用认证缓存以提高性能
- 配置合理的缓存大小和过期时间
- 定期清理过期缓存
- 考虑使用分布式缓存以支持多实例部署

### 2. 异步编程
- 使用异步 API 避免阻塞主线程
- 充分利用 .NET 10 的异步优化
- 避免在认证授权处理中使用同步操作

### 3. 批量处理
- 使用批量处理减少数据库查询次数
- 批量生成和验证令牌
- 批量更新用户权限

### 4. 连接池管理
- 使用连接池管理数据库连接
- 配置合理的连接池大小
- 定期清理空闲连接

### 5. AOT 编译优化
- 启用 AOT 编译以提高性能
- 使用 AOT 兼容的库和 API
- 避免使用反射等 AOT 不友好的特性
- 考虑使用 Source Generator 替代反射

### 6. 内存优化
- 减少对象创建和内存分配
- 使用对象池管理频繁创建的对象
- 考虑使用 Span<T> 和 Memory<T> 优化内存使用
- 避免不必要的内存拷贝

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

### AOT 编译注意事项

1. **使用 AOT 兼容的库**：确保所有依赖库都支持 AOT 编译
2. **避免反射**：尽量避免使用反射，或使用 Source Generator 替代
3. **资源加载**：确保所有资源在 AOT 编译时能被正确处理
4. **动态代码生成**：避免使用动态代码生成技术
5. **测试验证**：在 AOT 编译后进行充分测试
6. **配置 AOT 特定选项**：根据需要配置 AOT 特定的编译器选项
7. **考虑使用 TrimMode=Full**：对于需要最小化大小的应用，考虑使用 Full 修剪模式

## 与其他框架集成

### 与 ASP.NET Core 集成

```csharp
var builder = WebApplication.CreateBuilder(args);

// 添加认证服务
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Auth:Jwt:Issuer"],
            ValidAudience = builder.Configuration["Auth:Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Auth:Jwt:Key"]))
        };
    });

// 添加授权服务
builder.Services.AddAuthorization();

// 添加自定义认证服务
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// 使用认证授权中间件
app.UseAuthentication();
app.UseAuthorization();

// 定义 API 端点
app.MapGet("/api/protected", [Authorize] () => {
    return Results.Ok("This is a protected endpoint");
});

app.Run();
```

### 与 Minimal API 集成

```csharp
var builder = WebApplication.CreateBuilder(args);

// 添加认证服务
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://auth.example.com",
            ValidAudience = "https://api.example.com",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKey"))
        };
    });

// 添加授权服务
builder.Services.AddAuthorization();

// 添加自定义认证服务
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// 使用认证授权中间件
app.UseAuthentication();
app.UseAuthorization();

// 定义 Minimal API 端点
app.MapPost("/api/login", async (IAuthService authService, LoginRequest request) => {
    var result = await authService.ValidateCredentialsAsync(request.Username, request.Password);
    if (result.IsValid) {
        var token = await authService.GenerateTokenAsync(result.User);
        return Results.Ok(new { Token = token });
    }
    return Results.Unauthorized();
});

app.MapGet("/api/user", [Authorize] (ClaimsPrincipal user) => {
    return Results.Ok(new {
        UserId = user.FindFirstValue(ClaimTypes.NameIdentifier),
        Username = user.FindFirstValue(ClaimTypes.Name),
        Roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList()
    });
});

app.Run();
```

### 与 SSO 解决方案集成

#### Keycloak 集成

```csharp
var builder = WebApplication.CreateBuilder(args);

// 添加 Keycloak 认证服务
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
}) .AddCookie() .AddOpenIdConnect(options => {
    options.Authority = "https://keycloak.example.com/realms/your-realm";
    options.ClientId = "your-client-id";
    options.ClientSecret = "your-client-secret";
    options.ResponseType = "code";
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.SaveTokens = true;
});

// 添加授权服务
builder.Services.AddAuthorization();

var app = builder.Build();

// 使用认证授权中间件
app.UseAuthentication();
app.UseAuthorization();

// 定义受保护的端点
app.MapGet("/api/protected", [Authorize] () => {
    return Results.Ok("This is a protected endpoint using Keycloak");
});

app.Run();
```

## 扩展开发

### 添加自定义身份验证提供程序

```csharp
public class CustomAuthenticationProvider : IAuthenticationProvider
{
    public async Task<AuthenticationResult> AuthenticateAsync(AuthenticationContext context)
    {
        // 自定义身份验证逻辑
        // 例如：从数据库验证用户凭据
        // 或：调用外部认证服务
        
        if (await ValidateCredentialsAsync(context.Username, context.Password)) {
            var user = await GetUserAsync(context.Username);
            return new AuthenticationResult {
                IsValid = true,
                User = user,
                Claims = GenerateClaims(user)
            };
        }
        
        return new AuthenticationResult {
            IsValid = false,
            Error = "Invalid credentials"
        };
    }
    
    // 其他方法实现...
}

// 注册自定义身份验证提供程序
builder.Services.AddSingleton<IAuthenticationProvider, CustomAuthenticationProvider>();
```

### 添加自定义授权策略

```csharp
// 定义自定义授权处理程序
public class CustomAuthorizationHandler : AuthorizationHandler<CustomRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRequirement requirement)
    {
        // 自定义授权逻辑
        // 例如：检查用户是否具有特定权限
        // 或：检查用户是否满足特定条件
        
        if (context.User.HasClaim(c => c.Type == "CustomClaim" && c.Value == "RequiredValue")) {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}

// 定义自定义授权需求
public class CustomRequirement : IAuthorizationRequirement
{
    // 需求属性和方法...
}

// 注册自定义授权处理程序和策略
builder.Services.AddSingleton<IAuthorizationHandler, CustomAuthorizationHandler>();
builder.Services.AddAuthorization(options => {
    options.AddPolicy("CustomPolicy", policy => 
        policy.Requirements.Add(new CustomRequirement()));
});
```

## 故障排除

### 常见问题

1. **认证失败**
   - 检查用户凭据是否正确
   - 检查认证服务配置是否正确
   - 检查令牌是否过期
   - 查看应用程序日志获取详细错误信息

2. **授权失败**
   - 检查用户是否具有所需角色或权限
   - 检查授权策略配置是否正确
   - 检查令牌是否包含所需声明
   - 查看应用程序日志获取详细错误信息

3. **SSO 集成问题**
   - 检查 SSO 提供商配置是否正确
   - 检查客户端 ID 和密钥是否正确
   - 检查重定向 URI 是否匹配
   - 检查 SSO 提供商的日志

4. **性能问题**
   - 启用认证缓存
   - 优化数据库查询
   - 考虑使用 AOT 编译
   - 增加资源限制
   - 监控系统性能指标

5. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 检查是否使用了反射等不兼容特性
   - 查看编译错误信息
   - 考虑使用 Source Generator 替代反射
   - 检查 AOT 配置是否正确

## 最佳实践

1. **使用 HTTPS**
   - 始终使用 HTTPS 保护认证授权通信
   - 避免在生产环境中使用 HTTP

2. **安全存储密钥**
   - 使用安全的方式存储认证密钥和凭证
   - 考虑使用 Azure Key Vault 或 AWS Secrets Manager
   - 避免将密钥硬编码到配置文件中

3. **合理设置令牌过期时间**
   - Access Token 过期时间不宜过长（建议 15-60 分钟）
   - Refresh Token 过期时间可以适当延长（建议 7-30 天）
   - 定期轮换令牌签名密钥

4. **实施适当的授权策略**
   - 遵循最小权限原则
   - 实施细粒度的授权控制
   - 定期审查和更新授权策略

5. **记录审计日志**
   - 记录所有认证授权事件
   - 包括成功和失败的认证尝试
   - 记录用户操作和权限变更
   - 定期审查审计日志

6. **使用 AOT 编译优化**
   - 对于性能敏感的认证应用，考虑使用 AOT 编译
   - 在开发环境进行充分测试
   - 监控 AOT 编译后的性能提升

7. **实施多因素认证**
   - 对于敏感操作，实施多因素认证
   - 支持多种多因素认证方式
   - 考虑使用第三方 MFA 服务

8. **定期进行安全审计**
   - 定期进行安全审计和渗透测试
   - 检查认证授权系统的安全性
   - 及时修复发现的安全漏洞

## 版本更新记录

### 版本 1.0.0
- 初始版本发布
- 支持基本的认证授权功能
- 支持 JWT 令牌管理
- 支持多种授权模型
- 支持 SSO 集成
- 支持 AOT 编译
- 支持与 ASP.NET Core 和 Minimal API 集成

## 许可证

MIT License

## 联系方式

- 项目地址：https://github.com/vsa/auth
- 问题反馈：https://github.com/vsa/auth/issues
- 文档地址：https://vsa.github.io/auth/docs

## 贡献指南

欢迎大家贡献代码和文档！请查看 CONTRIBUTING.md 文件了解贡献指南。
