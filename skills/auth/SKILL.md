# auth Agent Skill - auth 技能

## 技能概述

基于 .NET 10 的高性能认证授权技能，为 .NET 开发者提供强大的认证授权功能，包括身份验证、授权管理、单点登录、权限控制等核心功能，支持 AOT 编译优化。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.AspNetCore.Authentication.JwtBearer@10.0.0
#:package Microsoft.AspNetCore.Authorization@10.0.0
```

### 注册服务

在您的主应用程序中注册认证授权服务：

```csharp
// 注册认证授权服务
var builder = WebApplication.CreateBuilder();

// 添加认证服务
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
});

// 添加授权服务
builder.Services.AddAuthorization(options => {
    options.AddPolicy("AdminPolicy", policy => 
        policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => 
        policy.RequireRole("User"));
});

var app = builder.Build();

// 使用认证授权中间件
app.UseAuthentication();
app.UseAuthorization();

app.Run();
```

### 使用示例

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

// 验证令牌
var isValid = await authService.ValidateTokenAsync("your-jwt-token");
Console.WriteLine($"令牌是否有效: {isValid}");
```

## 导航地图

```
auth/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
├── scripts/                    # 脚本和工具
    ├── abac_attribute_based_access.cs          # ABAC 基于属性的访问控制
    ├── abac_attribute_based_access.run.json    # ABAC 运行配置
    ├── abac_attribute_based_access.setting.json # ABAC 设置文件
    ├── auth_integration.cs                     # 认证集成
    ├── auth_integration.run.json               # 认证集成运行配置
    ├── auth_integration.setting.json           # 认证集成设置文件
    ├── casbin_integration.cs                   # Casbin 集成
    ├── casbin_integration.run.json             # Casbin 集成运行配置
    ├── casbin_integration.setting.json         # Casbin 集成设置文件
    ├── keycloak_integration.cs                 # Keycloak 集成
    ├── keycloak_integration.run.json           # Keycloak 集成运行配置
    ├── keycloak_integration.setting.json       # Keycloak 集成设置文件
    ├── keycloak_service.cs                     # Keycloak 服务
    ├── keycloak_service.run.json               # Keycloak 服务运行配置
    ├── keycloak_service.setting.json           # Keycloak 服务设置文件
    ├── openauth_integration.cs                 # OpenAuth 集成
    ├── openauth_integration.run.json           # OpenAuth 集成运行配置
    ├── openauth_integration.setting.json       # OpenAuth 集成设置文件
    ├── openiddict_service.cs                   # OpenIddict 服务
    ├── openiddict_service.run.json             # OpenIddict 服务运行配置
    ├── openiddict_service.setting.json         # OpenIddict 服务设置文件
    ├── permission_service.cs                   # 权限服务
    ├── permission_service.run.json             # 权限服务运行配置
    ├── permission_service.setting.json         # 权限服务设置文件
    ├── sso_integration.cs                      # SSO 集成
    ├── sso_integration.run.json                # SSO 集成运行配置
    ├── sso_integration.setting.json            # SSO 集成设置文件
    ├── sso_microservice.cs                     # SSO 微服务
    ├── sso_microservice.run.json               # SSO 微服务运行配置
    ├── sso_microservice.setting.json           # SSO 微服务设置文件
    ├── sso_service.cs                          # SSO 服务
    ├── sso_service.run.json                    # SSO 服务运行配置
    ├── sso_service.setting.json                # SSO 服务设置文件
    ├── theidserver_integration.cs              # TheIdServer 集成
    ├── theidserver_integration.run.json        # TheIdServer 集成运行配置
    └── theidserver_integration.setting.json    # TheIdServer 集成设置文件
```

## 主要功能

1. **身份验证**: 支持多种身份验证方式，包括 JWT、OAuth 2.0、OpenID Connect 等
2. **授权管理**: 支持基于角色的访问控制（RBAC）和基于属性的访问控制（ABAC）
3. **单点登录 (SSO)**: 支持多种 SSO 解决方案集成
4. **权限控制**: 细粒度的权限管理和访问控制
5. **令牌管理**: 支持 JWT 令牌的生成、验证和刷新
6. **多因素认证**: 支持多种多因素认证方式
7. **API 保护**: 提供 API 保护和授权机制
8. **高性能设计**: 优化的性能实现，支持高并发场景
9. **AOT 编译优化**: 支持将认证应用编译为本机代码，提高启动速度和运行性能
10. **易于使用的 API**: 简洁直观的 API 设计，降低开发复杂度
11. **可扩展架构**: 支持自定义扩展和插件开发
12. **与多种框架集成**: 支持与 ASP.NET Core、Minimal API 等框架集成

## 扩展说明

此技能提供完整的认证授权解决方案，您可以根据需要进行扩展：

1. **自定义身份验证提供程序**: 实现自定义身份验证逻辑
2. **扩展授权策略**: 添加自定义授权策略和规则
3. **集成新的 SSO 解决方案**: 集成其他 SSO 提供商
4. **添加新的令牌类型**: 支持新的令牌格式和标准
5. **性能优化**: 针对特定场景优化认证授权性能
6. **添加审计日志**: 记录认证授权事件和操作

## 最佳实践

1. **使用依赖注入**: 使用依赖注入管理认证授权服务，提高代码的可测试性和可维护性
2. **异步编程**: 优先使用异步 API 进行认证授权操作，避免阻塞主线程
3. **合理设计授权策略**: 根据业务需求设计合理的授权策略，避免过度授权
4. **安全存储凭据**: 确保敏感信息如密码、密钥等安全存储
5. **定期更新密钥**: 定期更新加密密钥和令牌签名密钥
6. **监控认证性能**: 监控认证授权性能，及时发现和解决性能瓶颈
7. **使用 AOT 编译**: 对于性能敏感的认证应用，考虑使用 AOT 编译优化
8. **实施多因素认证**: 对于敏感操作，实施多因素认证
9. **记录审计日志**: 记录认证授权事件，便于审计和监控
10. **测试认证功能**: 充分测试认证授权功能，确保安全性和可靠性

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
```

### AOT 编译注意事项

1. **使用 AOT 兼容的库**: 确保使用的认证相关库支持 AOT 编译
2. **避免反射**: 避免在认证处理中使用反射，或使用 Source Generator 替代
3. **资源加载**: 确保所有认证资源都能在 AOT 编译时被正确处理
4. **动态代码生成**: 避免使用动态代码生成，如 System.Reflection.Emit
5. **测试验证**: 在 AOT 编译后进行充分的测试，确保认证功能正常工作
6. **性能优化**: AOT 编译可以显著提高认证应用的启动速度和运行性能
7. **内存优化**: AOT 编译可以减少认证应用的内存占用

## 与其他系统集成

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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
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

// 定义受保护的 API 端点
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
