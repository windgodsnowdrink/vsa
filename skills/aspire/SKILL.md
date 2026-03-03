# aspire Agent Skill - aspire 技能

## 技能概述

基于 .NET 10 的高性能 aspire 技能，为 .NET 开发者提供强大的云原生应用开发和部署功能，支持微服务架构、服务发现、配置管理、健康检查等核心功能。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Aspire.Hosting@8.0.0
#:package Aspire.Dashboard@8.0.0
```

### 注册服务

在您的主应用程序中注册 aspire 服务：

```csharp
// 注册 aspire 服务
var appHost = DistributedApplication.CreateBuilder(args).Build();

// 配置服务
var apiService = appHost.AddProject<Projects.MyApi>("api")
    .WithReplicas(2)
    .WithHealthChecks()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

// 配置数据库
var db = appHost.AddPostgres("postgres")
    .WithPassword("postgres")
    .WithDatabase("mydb");

// 连接服务和数据库
apiService.WithReference(db);

// 运行应用
await appHost.RunAsync();
```

### 使用示例

```csharp
// 创建分布式应用构建器
var builder = DistributedApplication.CreateBuilder(args);

// 添加 Redis 缓存服务
var redis = builder.AddRedis("redis")
    .WithRedisConfiguration("Cache", "0")
    .WithPersistence();

// 添加自定义服务
var myService = builder.AddProject<Projects.MyService>("myservice")
    .WithReference(redis);

// 构建并运行应用
var app = builder.Build();
await app.RunAsync();
```

## 导航地图

```
aspire/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
├── scripts/                    # 脚本和工具
│   ├── aspire_integration.cs               # aspire 集成实现
│   ├── aspire_integration.run.json         # aspire 集成运行配置
│   └── aspire_integration.setting.json     # aspire 集成设置文件
```

## 主要功能

1. **分布式应用开发**: 提供统一的方式来定义、配置和部署分布式应用
2. **服务发现与注册**: 自动管理服务的注册和发现，支持多种服务发现机制
3. **配置管理**: 集中管理应用配置，支持动态更新和环境隔离
4. **健康检查与监控**: 内置健康检查和监控功能，实时监控服务状态
5. **依赖管理**: 简化服务间依赖关系的配置和管理
6. **高可用设计**: 支持服务副本、负载均衡和故障转移
7. **容器化支持**: 无缝集成 Docker 和 Kubernetes，支持容器化部署
8. **AOT 编译优化**: 支持将应用编译为本机代码，提高启动速度和运行性能
9. **易于使用的 API**: 提供简洁直观的 API 设计，降低开发复杂度
10. **可扩展架构**: 支持自定义扩展和插件开发

## 扩展说明

此技能提供完整的 aspire 解决方案，您可以根据需要进行扩展：

1. **自定义服务组件**: 实现自定义的服务组件，扩展 aspire 的功能
2. **集成第三方服务**: 集成其他云服务和第三方工具
3. **自定义健康检查**: 实现自定义的健康检查逻辑
4. **扩展配置源**: 添加新的配置源支持
5. **性能优化**: 针对特定场景优化性能

## 最佳实践

1. **使用依赖注入**: 使用依赖注入管理服务，提高代码的可测试性和可维护性
2. **异步编程**: 优先使用异步 API 进行操作，避免阻塞主线程
3. **适当的服务粒度**: 合理划分服务边界，避免服务过大或过小
4. **配置管理**: 使用 aspire 的配置管理功能，集中管理应用配置
5. **健康检查**: 为每个服务添加适当的健康检查逻辑
6. **监控和日志**: 配置适当的监控和日志记录，便于调试和问题排查
7. **AOT 编译**: 对于性能敏感的服务，考虑使用 AOT 编译优化
8. **容器化部署**: 结合 Docker 和 Kubernetes，实现高效的容器化部署
9. **服务网格**: 考虑使用服务网格技术，增强服务间通信的可靠性和可观测性
10. **持续集成/持续部署**: 结合 CI/CD 流程，实现自动化部署和测试

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

1. **使用 AOT 兼容的库**: 确保所有依赖库都支持 AOT 编译
2. **避免反射**: 避免在运行时使用反射，或使用 Source Generator 替代
3. **资源加载**: 确保所有资源都能在 AOT 编译时被正确处理
4. **动态代码生成**: 避免使用动态代码生成，如 System.Reflection.Emit
5. **测试验证**: 在 AOT 编译后进行充分的测试，确保应用正常运行

## 与其他系统集成

### 与 ASP.NET Core 集成

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// 添加 ASP.NET Core 项目
var api = builder.AddProject<Projects.MyApi>("api")
    .WithHttpsEndpoint()
    .WithHealthChecks();

// 添加数据库
var db = builder.AddSqlServer("sqlserver")
    .WithPassword("P@ssw0rd")
    .WithDatabase("MyDatabase");

// 连接 API 和数据库
api.WithReference(db);

// 运行应用
await builder.Build().RunAsync();
```

### 与容器化服务集成

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// 添加 Redis 容器
var redis = builder.AddContainer("redis")
    .WithImage("redis:7")
    .WithPortBinding(6379, name: "redis")
    .WithVolumeMount("redis-data", "/data");

// 添加自定义容器服务
var myService = builder.AddContainer("myservice")
    .WithImage("mycompany/myservice:latest")
    .WithEnvironment("REDIS_URL", redis.GetEndpoint("redis"))
    .WithHealthCheck("http://localhost:8080/health");

// 运行应用
await builder.Build().RunAsync();
```
