# Silky 技能使用示例

## 基础示例

### 示例 1：创建并运行微服务

**步骤 1：创建服务项目**

```bash
# 创建用户服务项目
silky_core create --type service --name UserService --output ./services

# 创建订单服务项目
silky_core create --type service --name OrderService --output ./services

# 创建 API 网关项目
silky_core create --type gateway --name ApiGateway --output ./gateway
```

**步骤 2：定义服务接口**

在 `UserService/Services/Interfaces/IUserService.cs` 中定义接口：

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using Silky.Http.Core.Attributes;
using Silky.Rpc.Routing;

namespace UserService.Services
{
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
}
```

**步骤 3：实现服务**

在 `UserService/Services/Impl/UserService.cs` 中实现服务：

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Services.Impl
{
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
}
```

**步骤 4：定义数据传输对象**

在 `UserService/Services/UserDto.cs` 中定义 DTO：

```csharp
namespace UserService.Services
{
    public class UserDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
```

**步骤 5：配置服务**

在 `UserService/Program.cs` 中配置服务：

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // 注册 Silky 服务
            builder.Services.AddSilkyServices(builder.Configuration, options =>
            {
                options.AddRegistryCenter();
                options.AddSwaggerDocument();
                options.AddMessagePackSerializer();
            });
            
            var app = builder.Build();
            
            // 配置中间件
            app.UseSilkyWebHost();
            app.Run();
        }
    }
}
```

**步骤 6：启动服务**

```bash
# 启动用户服务
silky_core start --project ./services/UserService --environment Development

# 启动订单服务
silky_core start --project ./services/OrderService --environment Development

# 启动 API 网关
silky_core start --project ./gateway/ApiGateway --environment Development
```

**步骤 7：测试服务**

使用 curl 测试 API：

```bash
# 获取所有用户
curl http://localhost:5000/api/users

# 获取指定用户
curl http://localhost:5000/api/users/1

# 创建用户
curl -X POST http://localhost:5000/api/users -H "Content-Type: application/json" -d '{"Name": "王五", "Age": 35}'

# 更新用户
curl -X PUT http://localhost:5000/api/users/1 -H "Content-Type: application/json" -d '{"Name": "张三更新", "Age": 26}'

# 删除用户
curl -X DELETE http://localhost:5000/api/users/1
```

### 示例 2：使用代码生成工具

**步骤 1：生成 API 接口**

```bash
# 生成用户服务 API 接口
silky_generator generate --type api --service ProductService --output ./services/ProductService/Services/Interfaces
```

**步骤 2：生成实现代码**

```bash
# 生成用户服务实现代码
silky_generator generate --type impl --service ProductService --output ./services/ProductService/Services/Impl
```

**步骤 3：生成数据传输对象**

```bash
# 生成用户服务 DTO
silky_generator generate --type dto --service ProductService --output ./services/ProductService/Services
```

## 高级示例

### 示例 3：服务间 RPC 调用

**步骤 1：在订单服务中引用用户服务**

在 `OrderService.csproj` 中添加引用：

```xml
<ItemGroup>
    <ProjectReference Include="../UserService/UserService.csproj" />
</ItemGroup>
```

**步骤 2：在订单服务中调用用户服务**

在 `OrderService/Services/Impl/OrderService.cs` 中：

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Services;

namespace OrderService.Services.Impl
{
    public class OrderService : IOrderService
    {
        private readonly IUserService _userService;
        private static readonly List<OrderDto> _orders = new();
        
        public OrderService(IUserService userService)
        {
            _userService = userService;
        }
        
        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto input)
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
                Id = _orders.Count > 0 ? _orders.Max(o => o.Id) + 1 : 1,
                UserId = input.UserId,
                UserName = user.Name,
                ProductName = input.ProductName,
                Amount = input.Amount,
                Status = "Created"
            };
            
            _orders.Add(order);
            return order;
        }
        
        public Task<List<OrderDto>> GetOrdersAsync()
        {
            return Task.FromResult(_orders);
        }
        
        public Task<OrderDto> GetOrderByIdAsync(long id)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);
            return Task.FromResult(order);
        }
    }
}
```

### 示例 4：配置服务治理

**步骤 1：配置服务治理选项**

在 `Program.cs` 中：

```csharp
builder.Services.Configure<GovernanceOptions>(options =>
{
    // 超时设置
    options.TimeoutMilliseconds = 3000;
    
    // 重试次数
    options.RetryCount = 3;
    
    // 熔断策略
    options.CircuitBreakerFallbackPolicy = CircuitBreakerFallbackPolicy.FailFast;
    
    // 熔断阈值
    options.CircuitBreakerRequestVolumeThreshold = 20;
    
    // 熔断时间窗口
    options.CircuitBreakerSleepWindowMilliseconds = 5000;
    
    // 错误率阈值
    options.CircuitBreakerErrorThresholdPercentage = 50;
    
    // 限流设置
    options.RateLimiter = true;
    options.RateLimitPeriod = 1;
    options.RateLimit = 100;
});
```

### 示例 5：使用分布式配置中心

**步骤 1：配置分布式配置**

在 `Program.cs` 中：

```csharp
// 添加分布式配置
builder.Services.AddDistributedConfiguration();

// 配置配置中心选项
builder.Services.Configure<ConfigurationOptions>(options =>
{
    options.ConfigCenterType = ConfigCenterType.Consul;
    options.ConnectionString = "localhost:8500";
    options.Namespace = "silky";
});
```

**步骤 2：使用配置**

在服务中使用配置：

```csharp
using Microsoft.Extensions.Options;

public class UserService : IUserService
{
    private readonly UserOptions _options;
    
    public UserService(IOptions<UserOptions> options)
    {
        _options = options.Value;
    }
    
    public Task<List<UserDto>> GetUsersAsync()
    {
        // 使用配置
        var pageSize = _options.PageSize;
        // ...
    }
}

public class UserOptions
{
    public int PageSize { get; set; } = 10;
    public int MaxUsers { get; set; } = 1000;
}
```

## 实战场景

### 场景 1：电商系统微服务架构

**架构设计**

- **用户服务**：负责用户管理、认证授权
- **商品服务**：负责商品管理、库存管理
- **订单服务**：负责订单管理、支付处理
- **支付服务**：负责支付集成、交易处理
- **物流服务**：负责物流管理、配送跟踪
- **API 网关**：负责请求路由、负载均衡、认证授权

**服务间调用**

- 订单服务调用用户服务获取用户信息
- 订单服务调用商品服务扣减库存
- 订单服务调用支付服务处理支付
- 订单服务调用物流服务创建物流单

**配置示例**

```csharp
// 订单服务配置
builder.Services.AddSilkyServices(builder.Configuration, options =>
{
    options.AddRegistryCenter();
    options.AddSwaggerDocument();
    options.AddMessagePackSerializer();
    options.AddDistributedTransaction();
});

// 配置服务治理
builder.Services.Configure<GovernanceOptions>(options =>
{
    options.TimeoutMilliseconds = 5000;
    options.RetryCount = 3;
    options.CircuitBreakerFallbackPolicy = CircuitBreakerFallbackPolicy.FailFast;
});
```

### 场景 2：企业管理系统微服务架构

**架构设计**

- **认证服务**：负责用户认证、授权管理
- **用户服务**：负责用户信息管理、组织架构
- **部门服务**：负责部门管理、人员分配
- **考勤服务**：负责考勤管理、请假审批
- **薪资服务**：负责薪资计算、发放管理
- **API 网关**：负责请求路由、负载均衡、认证授权

**服务间调用**

- 考勤服务调用用户服务获取用户信息
- 考勤服务调用部门服务获取部门信息
- 薪资服务调用用户服务获取用户信息
- 薪资服务调用考勤服务获取考勤数据

**配置示例**

```csharp
// 考勤服务配置
builder.Services.AddSilkyServices(builder.Configuration, options =>
{
    options.AddRegistryCenter();
    options.AddSwaggerDocument();
    options.AddMessagePackSerializer();
});

// 配置服务注册
builder.Services.Configure<RegistryOptions>(options =>
{
    options.RegistryCenterType = RegistryCenterType.Consul;
    options.ConnectionString = "localhost:8500";
    options.HeartbeatInterval = 10;
    options.HeartbeatTimeout = 30;
});
```

## 性能优化示例

### 示例 6：使用 AOT 编译提升性能

**步骤 1：配置 AOT 编译**

在 `*.setting.json` 文件中：

```json
{
  "compilationOptions": {
    "targetFramework": "net11.0",
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

**步骤 2：编译项目**

```bash
# 使用 AOT 编译用户服务
dotnet publish ./services/UserService -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:PublishSingleFile=true
```

**步骤 3：运行编译后的服务**

```bash
# 运行编译后的用户服务
./services/UserService/bin/Release/net11.0/win-x64/publish/UserService.exe
```

### 示例 7：使用缓存提升性能

**步骤 1：添加缓存服务**

在 `Program.cs` 中：

```csharp
// 添加缓存服务
builder.Services.AddDistributedMemoryCache();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "silky:";
});
```

**步骤 2：在服务中使用缓存**

在 `UserService/Services/Impl/UserService.cs` 中：

```csharp
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

public class UserService : IUserService
{
    private readonly IDistributedCache _cache;
    private static readonly List<UserDto> _users = new();
    
    public UserService(IDistributedCache cache)
    {
        _cache = cache;
    }
    
    public async Task<List<UserDto>> GetUsersAsync()
    {
        // 尝试从缓存获取
        var cachedUsers = await _cache.GetStringAsync("users");
        if (!string.IsNullOrEmpty(cachedUsers))
        {
            return JsonSerializer.Deserialize<List<UserDto>>(cachedUsers);
        }
        
        // 从数据库获取（这里使用内存模拟）
        var users = _users;
        
        // 缓存结果
        await _cache.SetStringAsync("users", JsonSerializer.Serialize(users), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });
        
        return users;
    }
    
    // 其他方法...
}
```

## 常见问题与解决方案

### 问题 1：服务注册失败

**症状**：服务启动后无法在注册中心看到

**解决方案**：

1. 检查注册中心是否正常运行
   ```bash
   # 检查 Consul 是否运行
   curl http://localhost:8500/v1/status/leader
   ```

2. 检查网络连接是否正常
   ```bash
   # 测试网络连接
   ping localhost
   ```

3. 检查服务配置是否正确
   ```csharp
   builder.Services.Configure<RegistryOptions>(options =>
   {
       options.RegistryCenterType = RegistryCenterType.Consul;
       options.ConnectionString = "localhost:8500"; // 确保地址正确
   });
   ```

### 问题 2：RPC 调用失败

**症状**：服务间调用超时或返回错误

**解决方案**：

1. 检查目标服务是否正常运行
   ```bash
   # 检查服务健康状态
   silky_core registry --health UserService
   ```

2. 检查服务接口定义是否一致
   - 确保调用方和服务方的接口定义完全一致
   - 确保参数类型和返回类型一致

3. 检查超时设置是否合理
   ```csharp
   builder.Services.Configure<RpcOptions>(options =>
   {
       options.TimeoutMilliseconds = 5000; // 适当增加超时时间
   });
   ```

### 问题 3：API 网关路由失败

**症状**：通过网关访问服务返回 404 或 500 错误

**解决方案**：

1. 检查路由配置是否正确
   - 确保服务接口使用了 `[ServiceRoute]` 特性
   - 确保方法使用了 `[HttpGet]`、`[HttpPost]` 等特性

2. 检查目标服务是否正常运行
   ```bash
   # 检查服务健康状态
   silky_core registry --health UserService
   ```

3. 检查网关配置是否正确
   ```csharp
   builder.Services.AddSilkyGateway(builder.Configuration, options =>
   {
       options.AddRegistryCenter();
       options.AddSwaggerDocument();
   });
   ```

### 问题 4：AOT 编译错误

**症状**：使用 AOT 编译时出现错误

**解决方案**：

1. 检查代码是否符合 AOT 编译要求
   - 避免使用反射
   - 避免使用动态类型
   - 避免使用 IL 生成

2. 检查依赖项是否支持 AOT 编译
   - 确保所有依赖项都支持 AOT 编译
   - 对于不支持的依赖项，考虑使用替代方案

3. 检查编译配置是否正确
   ```json
   {
     "publishOptions": {
       "publishAot": true,
       "trimMode": "partial", // 使用 partial 模式
       "selfContained": true,
       "publishSingleFile": true,
       "runtimeIdentifier": "win-x64"
     }
   }
   ```

## 最佳实践

1. **服务设计**
   - 遵循微服务设计原则，服务粒度适中
   - 每个服务负责一个业务领域
   - 服务间通过接口通信，避免直接依赖

2. **接口设计**
   - 使用 RESTful API 设计风格
   - 定义清晰的接口规范
   - 使用标准化的 HTTP 方法和状态码

3. **配置管理**
   - 使用分布式配置中心管理配置
   - 配置按环境分离（开发、测试、生产）
   - 敏感配置使用加密存储

4. **服务治理**
   - 合理配置服务治理策略
   - 实现服务健康检查
   - 配置适当的超时和重试策略

5. **监控告警**
   - 集成监控系统（Prometheus + Grafana）
   - 配置关键指标告警
   - 实现分布式追踪（Jaeger + OpenTelemetry）

6. **CI/CD**
   - 建立持续集成和持续部署流程
   - 使用自动化测试确保代码质量
   - 实现蓝绿部署或滚动更新

7. **安全**
   - 使用 HTTPS 加密网络传输
   - 实现细粒度的权限控制
   - 对所有输入进行验证，防止注入攻击

8. **性能**
   - 使用 AOT 编译提升性能
   - 合理使用缓存，减少重复计算
   - 优化数据库查询，使用索引

9. **可扩展性**
   - 设计服务时考虑水平扩展
   - 使用负载均衡分散流量
   - 实现服务自动发现和注册

10. **可维护性**
    - 编写清晰的代码和文档
    - 遵循代码规范和最佳实践
    - 实现自动化测试，确保代码质量

## 总结

Silky 技能提供了完整的微服务开发框架，支持服务注册与发现、RPC 调用、API 网关、分布式配置、服务治理等核心功能。通过本文的示例，您可以快速上手 Silky 技能，构建高性能、可扩展的微服务应用。

在实际应用中，您可以根据具体需求选择合适的配置和架构方案，结合最佳实践，构建高质量的微服务系统。