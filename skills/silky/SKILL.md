# Silky 技能 - Silky 微服务框架

## 技能概述

基于 .NET 10 的高性能微服务框架 Silky 技能，为 .NET 开发者提供强大的微服务开发能力。Silky 是一个轻量级、高性能、模块化的微服务框架，专注于简化微服务开发和提高开发效率。

## 快速开始指南

### 安装依赖

在主应用程序的 runfile 中添加以下依赖：

```yaml
#:package Silky.Core@3.3.0
#:package Silky.Http.Core@3.3.0
#:package Silky.Rpc@3.3.0
#:package Silky.Registry@3.3.0
#:package Silky.Swagger@3.3.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
```

### 注册服务

在主应用程序中注册 Silky 服务：

```csharp
// 注册 Silky 服务
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSilkyServices(builder.Configuration, options =>
{
    options.AddRegistryCenter();
    options.AddSwaggerDocument();
    options.AddMessagePackSerializer();
});

// 构建应用
var app = builder.Build();

// 配置中间件
app.UseSilkyWebHost();
app.Run();
```

### 使用示例

```csharp
// 定义服务接口
[ServiceRoute]
public interface IUserService
{
    [HttpGet("users")]
    Task<List<UserDto>> GetUsersAsync();
    
    [HttpGet("users/{id}")]
    Task<UserDto> GetUserByIdAsync(long id);
    
    [HttpPost("users")]
    Task<long> CreateUserAsync(UserDto user);
    
    [HttpPut("users/{id}")]
    Task<bool> UpdateUserAsync(long id, UserDto user);
    
    [HttpDelete("users/{id}")]
    Task<bool> DeleteUserAsync(long id);
}

// 实现服务
public class UserService : IUserService
{
    private static readonly List<UserDto> _users = new()
    {
        new UserDto { Id = 1, Name = "张三", Age = 25 },
        new UserDto { Id = 2, Name = "李四", Age = 30 }
    };
    
    public Task<List<UserDto>> GetUsersAsync()
    {
        return Task.FromResult(_users);
    }
    
    public Task<UserDto> GetUserByIdAsync(long id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user);
    }
    
    public Task<long> CreateUserAsync(UserDto user)
    {
        user.Id = _users.Max(u => u.Id) + 1;
        _users.Add(user);
        return Task.FromResult(user.Id);
    }
    
    public Task<bool> UpdateUserAsync(long id, UserDto user)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == id);
        if (existingUser != null)
        {
            existingUser.Name = user.Name;
            existingUser.Age = user.Age;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
    
    public Task<bool> DeleteUserAsync(long id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user != null)
        {
            _users.Remove(user);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}

// 数据传输对象
public class UserDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}
```

## 导航地图

```
silky/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── silky_core.cs          # silky 核心实现
    ├── silky_generator.cs     # silky 代码生成实现
    ├── silky_core.setting.json  # 编译配置
    ├── silky_core.run.json  # 运行配置
    ├── silky_generator.setting.json  # 编译配置
    └── silky_generator.run.json  # 运行配置
```

## 核心功能

1. **微服务框架**：提供完整的微服务开发框架，支持服务注册与发现、负载均衡、熔断与限流等功能
2. **RPC 调用**：基于 .NET Core 的高性能 RPC 框架，支持多种序列化方式
3. **API 网关**：内置 API 网关，支持请求路由、负载均衡、认证授权等功能
4. **分布式配置**：支持分布式配置管理，实现配置的集中管理和动态更新
5. **服务治理**：提供服务健康检查、服务降级、服务熔断等服务治理功能
6. **代码生成**：支持自动生成 API 接口、实现代码、数据传输对象等代码
7. **AOT 编译**：支持 AOT 编译，提供更高的性能和更小的部署包

## API 参考

### 服务注册与发现

```csharp
// 注册服务中心
builder.Services.AddRegistryCenter();

// 配置服务注册选项
builder.Services.Configure<RegistryOptions>(options =>
{
    options.RegistryCenterType = RegistryCenterType.Zookeeper;
    options.ConnectionString = "localhost:2181";
});
```

### RPC 调用

```csharp
// 配置 RPC 选项
builder.Services.Configure<RpcOptions>(options =>
{
    options.TimeoutMilliseconds = 3000;
    options.RetryCount = 3;
});

// 使用 RPC 调用其他服务
public class OrderService : IOrderService
{
    private readonly IUserService _userService;
    
    public OrderService(IUserService userService)
    {
        _userService = userService;
    }
    
    public async Task<OrderDto> CreateOrderAsync(CreateOrderInput input)
    {
        // 调用用户服务获取用户信息
        var user = await _userService.GetUserByIdAsync(input.UserId);
        if (user == null)
        {
            throw new Exception("用户不存在");
        }
        
        // 创建订单
        var order = new OrderDto
        {
            Id = 1,
            UserId = input.UserId,
            UserName = user.Name,
            Amount = input.Amount,
            Status = "Created"
        };
        
        return order;
    }
}
```

### API 网关

```csharp
// 配置 API 网关
builder.Services.AddSilkyGateway();

// 配置路由选项
builder.Services.Configure<GatewayOptions>(options =>
{
    options.RouteCacheExpireTime = 30;
});
```

### 分布式配置

```csharp
// 配置分布式配置
builder.Services.AddDistributedConfiguration();

// 配置配置中心选项
builder.Services.Configure<ConfigurationOptions>(options =>
{
    options.ConfigCenterType = ConfigCenterType.Consul;
    options.ConnectionString = "localhost:8500";
});
```

### 服务治理

```csharp
// 配置服务治理选项
builder.Services.Configure<GovernanceOptions>(options =>
{
    options.TimeoutMilliseconds = 3000;
    options.RetryCount = 3;
    options.CircuitBreakerFallbackPolicy = CircuitBreakerFallbackPolicy.FailFast;
});
```

## AOT 编译

Silky 技能支持 AOT（Ahead-of-Time）编译，提供以下优势：

1. **启动速度快**：预编译代码，减少运行时 JIT 编译开销
2. **运行时性能高**：优化的机器代码执行效率更高
3. **内存占用小**：通过裁剪未使用的代码，减少应用程序体积
4. **部署简单**：生成单个可执行文件，无需安装 .NET 运行时

### AOT 编译配置

```json
{
  "compilationOptions": {
    "targetFramework": "net10.0",
    "langVersion": "preview",
    "nullable": true,
    "implicitUsings": true
  },
  "publishOptions": {
    "publishAot": true,
    "trimMode": "partial",
    "selfContained": true,
    "publishSingleFile": true,
    "runtimeIdentifier": "win-x64"
  }
}
```

## 使用示例

### 示例 1：创建微服务项目

```bash
# 创建服务项目
silky_core create --type service --name UserService --output ./services

# 创建网关项目
silky_core create --type gateway --name ApiGateway --output ./gateway

# 创建模块项目
silky_core create --type module --name OrderModule --output ./modules
```

### 示例 2：启动微服务

```bash
# 启动用户服务
silky_core start --project ./services/UserService --environment Development

# 启动订单服务
silky_core start --project ./services/OrderService --environment Development

# 启动 API 网关
silky_core start --project ./gateway/ApiGateway --environment Development
```

### 示例 3：服务注册管理

```bash
# 列出所有注册的服务
silky_core registry --list

# 检查服务健康状态
silky_core registry --health
```

### 示例 4：代码生成

```bash
# 生成 API 接口代码
silky_core generate --type api --service UserService --output ./services/UserService/Interfaces

# 生成实现代码
silky_core generate --type impl --service UserService --output ./services/UserService/Impl

# 生成数据传输对象代码
silky_core generate --type dto --service UserService --output ./services/UserService/Dtos
```

## 扩展说明

Silky 技能提供了丰富的扩展点，您可以根据需要进行扩展：

1. **自定义服务注册中心**：实现 `IRegistryCenter` 接口，自定义服务注册中心
2. **自定义负载均衡策略**：实现 `ILoadBalancer` 接口，自定义负载均衡策略
3. **自定义序列化方式**：实现 `ISerializer` 接口，自定义序列化方式
4. **自定义服务治理策略**：实现 `IServiceGovernance` 接口，自定义服务治理策略
5. **自定义 API 过滤器**：实现 `IGatewayFilter` 接口，自定义 API 网关过滤器

## 最佳实践

1. **服务设计**：遵循微服务设计原则，将业务逻辑拆分为独立的服务
2. **接口设计**：使用 RESTful API 设计风格，定义清晰的接口规范
3. **依赖注入**：充分利用依赖注入，提高代码的可测试性和可维护性
4. **配置管理**：使用分布式配置中心，实现配置的集中管理和动态更新
5. **服务治理**：合理配置服务治理策略，提高系统的可靠性和稳定性
6. **监控告警**：集成监控系统，实现服务的实时监控和告警
7. **CI/CD**：建立持续集成和持续部署流程，提高开发和部署效率
8. **AOT 编译**：对于性能要求高的场景，使用 AOT 编译提高性能

## 故障排除

1. **服务注册失败**：检查服务注册中心是否正常运行，检查网络连接是否正常
2. **RPC 调用失败**：检查目标服务是否正常运行，检查网络连接是否正常，检查服务接口定义是否一致
3. **API 网关路由失败**：检查路由配置是否正确，检查目标服务是否正常运行
4. **分布式配置更新失败**：检查配置中心是否正常运行，检查配置权限是否正确
5. **服务健康检查失败**：检查服务依赖是否正常，检查服务自身是否正常运行
6. **AOT 编译错误**：检查代码是否符合 AOT 编译要求，检查依赖项是否支持 AOT 编译

## 常见问题

### Q: 如何选择服务注册中心？
A: Silky 支持 Zookeeper、Consul、Nacos 等多种服务注册中心，您可以根据自己的技术栈和需求选择合适的服务注册中心。

### Q: 如何配置服务熔断策略？
A: 您可以通过配置 `GovernanceOptions` 来配置服务熔断策略，包括熔断阈值、熔断时间窗口等参数。

### Q: 如何实现服务降级？
A: 您可以通过实现 `IFallbackHandler` 接口来实现服务降级逻辑，当服务调用失败时，会自动调用降级逻辑。

### Q: 如何优化 RPC 调用性能？
A: 您可以通过以下方式优化 RPC 调用性能：
1. 使用更高效的序列化方式，如 MessagePack
2. 合理配置超时时间和重试策略
3. 使用连接池管理网络连接
4. 对于高频调用，考虑使用缓存

### Q: 如何实现分布式事务？
A: Silky 支持基于 TCC（Try-Confirm-Cancel）模式的分布式事务，您可以通过添加 `[Transaction]` 特性来启用分布式事务。

### Q: 如何集成 Swagger？
A: Silky 内置了 Swagger 支持，您可以通过添加 `AddSwaggerDocument` 选项来启用 Swagger，然后通过访问 `/swagger` 路径查看 API 文档。