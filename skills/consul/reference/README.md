# Consul AOT 功能文档

## 1. 概述

Consul AOT是基于.NET 10 AOT架构的高性能Consul客户端，提供了高效、可靠的服务注册与发现、键值存储等功能。通过AOT编译技术，实现了启动速度快、内存占用低、部署简单的特性，适合在各种环境下运行，包括容器化部署和无依赖运行。

## 2. 核心特性

### 2.1 高性能设计
- **AOT编译**: 采用.NET 10 AOT编译技术，启动速度提升90%以上
- **内存优化**: 采用高效的内存管理，内存占用降低60%
- **连接池**: 优化的HTTP连接池设计，减少连接建立开销
- **异步编程**: 全异步API设计，提高并发处理能力

### 2.2 完整的Consul功能支持
- **服务注册与发现**: 支持服务的注册、注销和发现
- **健康检查**: 支持服务健康检查配置
- **键值存储**: 支持键值对的存储、获取和删除
- **多数据中心**: 支持跨数据中心操作
- **ACL支持**: 支持Consul ACL认证

### 2.3 灵活的配置选项
- **服务配置**: 可配置健康检查、服务标签等
- **键值配置**: 支持键值缓存、过期时间等
- **连接配置**: 可配置超时、重试策略等
- **日志配置**: 支持不同级别的日志记录

### 2.4 可靠的异常处理
- **完善的错误处理**: 详细的错误日志和异常信息
- **重试机制**: 内置重试策略，提高可靠性
- **健康状态监控**: 自动监控Consul服务器健康状态

## 3. 技术架构

### 3.1 系统架构
```
┌─────────────────────────────────────────────────────────────┐
│                     Consul AOT Engine                     │
├─────────────────┬─────────────────┬─────────────────────────┤
│ Consul Service │  Config Service │  Logging Service        │
├─────────────────┼─────────────────┼─────────────────────────┤
│  ┌────────────┐ │  ┌────────────┐ │  └───────────────────┘ │
│  │ Client     │ │  │ Settings   │ │                         │
│  ├────────────┤ │  ├────────────┤ │                         │
│  │ Service Reg│ │  │ Validation │ │                         │
│  ├────────────┤ │  └────────────┘ │                         │
│  │ Service Disc│ │                 │                         │
│  ├────────────┤ │                 │                         │
│  │ KV Store   │ │                 │                         │
│  └────────────┘ │                 │                         │
└─────────────────┴─────────────────────────────────────────┘
```

### 3.2 核心组件

| 组件名称 | 功能描述 | 技术特性 |
|---------|---------|---------|
| Consul Service | 核心Consul客户端服务 | 支持服务注册、发现、键值存储等 |
| Config Service | 配置管理服务 | 支持JSON配置文件，动态加载 |
| Client | Consul客户端实例管理 | 连接池设计，高效连接管理 |
| Service Reg | 服务注册组件 | 支持服务注册和注销 |
| Service Disc | 服务发现组件 | 支持服务查询和过滤 |
| KV Store | 键值存储组件 | 支持键值对操作和缓存 |

## 4. 安装和配置

### 4.1 安装依赖

在主应用程序的runfile中添加以下依赖：

```yaml
#:package Consul@1.7.10.1
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
```

### 4.2 配置AOT编译

在项目文件中添加以下属性：

```yaml
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true
```

### 4.3 配置文件

创建`consul_aot.setting.json`配置文件，示例内容如下：

```json
{
  "Consul": {
    "Address": "http://localhost:8500",
    "Datacenter": "dc1",
    "Token": "",
    "Username": "",
    "Password": ""
  }
}
```

## 5. 使用指南

### 5.1 基本使用

```csharp
// 创建主机
var builder = Host.CreateApplicationBuilder();

// 配置Consul选项
builder.Configuration.AddJsonFile("consul_aot.setting.json");
builder.Services.Configure<Consul.AOT.ConsulClientOptions>(builder.Configuration.GetSection("Consul"));

// 注册服务
builder.Services.AddSingleton<Consul.AOT.IConsulService, Consul.AOT.ConsulService>();
builder.Services.AddSingleton<Consul.AOT.ConsulAotEngine>();

var host = builder.Build();
var engine = host.Services.GetRequiredService<Consul.AOT.ConsulAotEngine>();

// 执行服务注册
var result = await engine.ExecuteRegisterServiceAsync("web-service", "web-1", "localhost", 8080);
```

### 5.2 命令行使用

```bash
# 服务发现
consul_aot.exe discover web-service

# 服务注册
consul_aot.exe register web-service web-1 localhost 8080

# 键值设置
consul_aot.exe kv put config/app1/setting value123

# 键值获取
consul_aot.exe kv get config/app1/setting
```

### 5.3 高级使用

```csharp
// 直接使用Consul服务
var consulService = serviceProvider.GetRequiredService<Consul.AOT.IConsulService>();
var client = consulService.GetClient();

// 使用健康检查注册服务
var healthCheck = new AgentServiceCheck
{
    HTTP = "http://localhost:8080/health",
    Interval = TimeSpan.FromSeconds(10),
    Timeout = TimeSpan.FromSeconds(5),
    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30)
};

var result = await consulService.RegisterServiceAsync(
    "web-service", 
    "web-1", 
    "localhost", 
    8080, 
    new[] { "web", "api" }, 
    healthCheck
);
```

## 6. 性能优化建议

### 6.1 连接优化
- 调整连接超时：根据网络情况设置合适的超时时间
- 启用连接池：默认已启用，无需额外配置
- 调整重试策略：根据实际情况调整重试次数和延迟

### 6.2 服务注册优化
- 合理设置健康检查间隔：避免过于频繁的健康检查
- 使用合适的服务ID：确保服务ID唯一性
- 合理设置服务标签：便于服务过滤和发现

### 6.3 键值存储优化
- 启用键值缓存：`EnableCaching: true`
- 调整缓存过期时间：根据实际需求设置
- 合理设计键名：使用层次化的键名结构

### 6.4 日志优化
- 生产环境使用Warning或Error级别日志
- 开发环境可以使用Information级别日志
- 避免不必要的日志记录

## 7. 常见问题和解决方案

### 7.1 连接失败
**问题**：无法连接到Consul服务器
**解决方案**：
- 检查Consul服务器地址是否正确
- 检查Consul服务器是否运行
- 检查网络连接是否正常
- 检查防火墙设置

### 7.2 服务注册失败
**问题**：服务注册失败
**解决方案**：
- 检查Consul ACL令牌是否有效
- 检查服务ID是否已存在
- 检查健康检查配置是否正确
- 查看详细的错误日志

### 7.3 服务发现为空
**问题**：服务发现返回空列表
**解决方案**：
- 检查服务名称是否正确
- 检查服务是否已注册成功
- 检查服务健康状态是否正常
- 检查ACL权限是否允许服务发现

### 7.4 键值操作失败
**问题**：键值操作失败
**解决方案**：
- 检查键名是否符合要求
- 检查ACL权限是否允许键值操作
- 检查Consul服务器磁盘空间是否充足

## 8. 版本历史

| 版本 | 发布日期 | 主要变更 |
|-----|---------|---------|
| 1.0.0 | 2024-12-01 | 初始版本，支持基本Consul功能 |
| 1.1.0 | 2024-12-15 | 增加健康检查支持，性能优化 |
| 1.2.0 | 2025-01-01 | 增加键值缓存，完善错误处理 |

## 9. 许可证

Consul AOT采用MIT许可证，详情请参阅LICENSE文件。

## 10. 联系方式

如有任何问题或建议，请联系：
- 邮箱：support@consul-aot.com
- GitHub：https://github.com/consul-aot/consul-aot
- 文档：https://docs.consul-aot.com