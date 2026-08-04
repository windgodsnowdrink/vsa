# Identity Agent Skill - Identity 技能

## 技能概述

基于 .NET 10 的高性能身份认证技能，为 .NET 开发者提供强大的身份管理功能，包括 JWT 令牌生成与验证、用户认证与授权、令牌撤销与刷新等核心功能。

## Identity AOT 引擎

Identity AOT 引擎是一个基于 .NET 10 AOT 编译的高性能身份认证解决方案，提供以下特性：

- **AOT 编译优化**：使用 .NET 10 的 AOT 编译技术，减少启动时间和内存占用
- **完整的 JWT 功能**：支持令牌生成、验证、解码、撤销和刷新
- **用户认证与授权**：提供用户登录认证和基于角色的权限控制
- **高性能设计**：优化的内存管理和并发处理
- **完善的错误处理**：详细的错误信息和日志记录
- **灵活的配置选项**：支持自定义配置和环境变量

## 快速开始指南

### 安装依赖

在主应用的 runfile 中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.IdentityModel.Tokens.Jwt@7.0.0
#:package Microsoft.IdentityModel.Tokens@7.0.0
#:package Microsoft.Extensions.Caching.Memory@10.0.0
```

### 注册服务

在主应用中注册身份认证服务：

```csharp
// 注册身份认证服务
builder.Services.Configure<IdentitySettings>(options => {
    options.Issuer = "https://identity.example.com";
    options.Audience = "https://api.example.com";
    options.Key = "your-secret-key-here-change-in-production";
    options.TokenExpiry = TimeSpan.FromHours(1);
    options.RefreshTokenExpiry = TimeSpan.FromDays(7);
});
builder.Services.AddSingleton<IdentityService>();
```

### 使用示例

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

## 导航地图

```
identity/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── identity_aot.cs         # Identity AOT 核心实现
    ├── identity_aot.run.json   # 运行配置
    ├── identity_aot.setting.json # 设置文件
    ├── identity_integration.cs # Identity 集成实现
    ├── identity_integration.run.json # 集成运行配置
    └── identity_integration.setting.json # 集成设置文件
```

## 主要功能

1. **JWT 令牌管理**：生成、验证、解码、撤销和刷新 JWT 令牌
2. **用户认证**：基于用户名和密码的用户认证
3. **用户授权**：基于角色的权限控制和授权检查
4. **高性能设计**：AOT 编译优化和内存管理
5. **缓存机制**：内置内存缓存，提高性能
6. **基准测试**：内置性能基准测试功能
7. **易使用的 API**：简洁直观的 API 设计
8. **可扩展架构**：支持自定义扩展和集成

## 扩展说明

本技能提供完整的身份认证解决方案，您可以根据需要进行扩展：

1. **自定义实现**：实现自定义的身份认证逻辑
2. **扩展功能**：添加新的身份认证特性
3. **与其他系统集成**：与现有的用户系统集成
4. **性能优化**：针对特定场景优化性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：正确处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **安全配置**：在生产环境中使用安全的密钥和配置
7. **令牌管理**：合理管理令牌生命周期和撤销
