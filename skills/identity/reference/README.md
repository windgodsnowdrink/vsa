# Identity - 参考文档

## 概述

Identity 是一个基于 .NET 10 的高性能身份认证系统，专为 .NET 开发者设计，提供完整的 JWT 令牌管理和用户认证授权功能。

## Identity AOT 引擎

Identity AOT 引擎是一个基于 .NET 10 AOT 编译的高性能身份认证解决方案，提供以下特性：

- **AOT 编译优化**：使用 .NET 10 的 AOT 编译技术，减少启动时间和内存占用
- **完整的 JWT 功能**：支持令牌生成、验证、解码、撤销和刷新
- **用户认证与授权**：提供用户登录认证和基于角色的权限控制
- **高性能设计**：优化的内存管理和并发处理
- **完善的错误处理**：详细的错误信息和日志记录
- **灵活的配置选项**：支持自定义配置和环境变量

## 核心组件

### 1. IdentityService
- **位置**：scripts/identity_aot.cs
- **功能**：核心身份认证业务逻辑处理
- **特性**：
  - JWT 令牌生成与验证
  - 用户认证与授权
  - 令牌撤销与刷新
  - 内存缓存管理
  - 性能基准测试
  - 错误处理和日志记录

### 2. IdentitySettings
- **位置**：scripts/identity_aot.cs
- **功能**：身份认证服务配置选项
- **特性**：
  - 发行者和受众配置
  - 令牌过期时间设置
  - 缓存配置
  - 权限配置
  - 令牌撤销设置

## 使用示例

### 基本使用

```csharp
// 获取身份认证服务
var identityService = serviceProvider.GetRequiredService<IdentityService>();

// 生成访问令牌
var tokenResult = await identityService.GenerateTokenAsync("admin", "admin");
Console.WriteLine($"访问令牌: {tokenResult.AccessToken}");
Console.WriteLine($"刷新令牌: {tokenResult.RefreshToken}");

// 验证令牌
var validationResult = await identityService.ValidateTokenAsync(tokenResult.AccessToken);
Console.WriteLine($"验证结果: {validationResult.Valid}");
```

### 高级配置

```csharp
// 配置身份认证服务
var settings = new IdentitySettings {
    Issuer = "https://identity.example.com",
    Audience = "https://api.example.com",
    Key = "your-secret-key-here-change-in-production",
    TokenExpiry = TimeSpan.FromHours(1),
    RefreshTokenExpiry = TimeSpan.FromDays(7),
    EnableTokenRevocation = true,
    EnableCaching = true,
    CacheSize = 1000,
    CacheExpiry = TimeSpan.FromHours(1),
    Permissions = new Dictionary<string, List<string>> {
        { "admin", new List<string> { "read", "write", "delete", "admin" } },
        { "user", new List<string> { "read", "write" } },
        { "guest", new List<string> { "read" } }
    }
};

builder.Services.Configure<IdentitySettings>(options => {
    options.Issuer = settings.Issuer;
    options.Audience = settings.Audience;
    options.Key = settings.Key;
    options.TokenExpiry = settings.TokenExpiry;
    options.RefreshTokenExpiry = settings.RefreshTokenExpiry;
    options.EnableTokenRevocation = settings.EnableTokenRevocation;
    options.EnableCaching = settings.EnableCaching;
    options.CacheSize = settings.CacheSize;
    options.CacheExpiry = settings.CacheExpiry;
    options.Permissions = settings.Permissions;
});
```

### 命令行使用

```bash
# 生成访问令牌
identity_aot.exe generate admin admin

# 验证访问令牌
identity_aot.exe validate <token>

# 解码访问令牌
identity_aot.exe decode <token>

# 撤销访问令牌
identity_aot.exe revoke <token>

# 刷新访问令牌
identity_aot.exe refresh <refresh_token>

# 认证用户
identity_aot.exe auth admin admin123

# 授权用户
identity_aot.exe authorize <token> read

# 运行基准测试
identity_aot.exe benchmark 1000

# 显示配置信息
identity_aot.exe config

# 显示帮助信息
identity_aot.exe help
```

## 配置选项

### IdentitySettings 配置

```json
{
  "IdentitySettings": {
    "Issuer": "https://identity.example.com",
    "Audience": "https://api.example.com",
    "Key": "your-secret-key-here-change-in-production",
    "TokenExpiry": "01:00:00",
    "RefreshTokenExpiry": "7.00:00:00",
    "EnableTokenRevocation": true,
    "EnableCaching": true,
    "CacheSize": 1000,
    "CacheExpiry": "01:00:00",
    "Permissions": {
      "admin": ["read", "write", "delete", "admin"],
      "user": ["read", "write"],
      "guest": ["read"]
    }
  }
}
```

### AOT 编译配置

**identity_aot.setting.json**：

```json
{
  "compilationOptions": {
    "targetFramework": "net10.0",
    "publishAot": true,
    "trimMode": "partial",
    "runtimeIdentifier": "win-x64"
  },
  "dependencies": {
    "Microsoft.Extensions.DependencyInjection": "10.0.0",
    "Microsoft.Extensions.Logging": "10.0.0",
    "System.IdentityModel.Tokens.Jwt": "7.0.0"
  }
}
```

### 运行配置

**identity_aot.run.json**：

```json
{
  "profiles": {
    "Generate Token": {
      "commandLineArgs": "generate admin admin",
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Development"
      }
    }
  }
}
```

## 性能优化

1. **启用缓存**：启用内存缓存以提高性能
2. **异步编程**：使用异步 API 避免阻塞
3. **合理配置缓存大小**：根据实际需求调整缓存大小
4. **使用 AOT 编译**：利用 .NET 10 的 AOT 编译技术提高性能
5. **优化密钥长度**：选择合适的密钥长度，平衡安全性和性能

## 故障排除

### 常见问题

1. **令牌验证失败**
   - 检查密钥是否正确
   - 验证令牌是否过期
   - 检查发行者和受众配置

2. **性能问题**
   - 启用缓存
   - 调整缓存大小
   - 优化配置参数

3. **编译错误**
   - 检查依赖版本是否正确
   - 验证 .NET 10 安装是否完整
   - 检查 AOT 编译配置

## 扩展开发

### 添加自定义功能

```csharp
public class CustomIdentityService : IdentityService
{
    public CustomIdentityService(ILogger<IdentityService> logger, IOptions<IdentitySettings> options) 
        : base(logger, options)
    {
    }

    // 重写或扩展现有方法
    public async Task<CustomResult> CustomMethodAsync()
    {
        // 实现自定义逻辑
        return new CustomResult();
    }
}

// 注册自定义服务
builder.Services.AddSingleton<CustomIdentityService>();
```

### 集成现有系统

```csharp
// 集成现有用户系统
public class IntegratedIdentityService : IdentityService
{
    private readonly IUserRepository _userRepository;

    public IntegratedIdentityService(ILogger<IdentityService> logger, IOptions<IdentitySettings> options, IUserRepository userRepository) 
        : base(logger, options)
    {
        _userRepository = userRepository;
    }

    // 重写认证方法，使用现有用户系统
    public override async Task<AuthenticationResult> AuthenticateUserAsync(string username, string password)
    {
        // 从现有系统验证用户
        var user = await _userRepository.GetUserAsync(username, password);
        if (user == null)
        {
            return new AuthenticationResult {
                Success = false,
                Error = "用户名或密码错误"
            };
        }

        // 生成令牌
        var tokenResult = await GenerateTokenAsync(user.Username, user.Role);
        return new AuthenticationResult {
            Success = true,
            Username = user.Username,
            Role = user.Role,
            AccessToken = tokenResult.AccessToken,
            RefreshToken = tokenResult.RefreshToken
        };
    }
}
```
