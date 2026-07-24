# 多租户智能体技能 - 参考文档

## 概述

多租户智能体技能是基于.NET 10的高性能多租户系统，专为.NET开发者设计。它提供了完整的多租户功能，包括租户隔离、资源管理、配额控制和生命周期管理等核心特性，采用AOT编译优化，提供高性能、高可扩展的多租户架构。

## 核心组件

### 1. ITenantManager (租户管理服务)
- **位置**: scripts/multitenant_integration.cs
- **功能**: 租户的核心业务逻辑处理
- **特性**: 
  - 租户的创建、更新、删除操作
  - 租户信息的获取和管理
  - 租户资源使用情况的监控
  - 错误处理和日志记录
  - 性能优化和缓存管理

### 2. ITenantRepository (租户存储服务)
- **位置**: scripts/multitenant_integration.cs
- **功能**: 租户数据的存储和检索
- **特性**: 
  - 租户数据的持久化存储
  - 租户信息的快速检索
  - 支持不同的存储介质
  - 数据一致性保证

### 3. ITenantResourceManager (租户资源管理服务)
- **位置**: scripts/multitenant_quota_management.cs
- **功能**: 租户资源的管理和监控
- **特性**: 
  - 租户资源使用情况的监控
  - 资源配额的设置和检查
  - 资源使用的限制和控制
  - 资源使用情况的统计和报告

### 4. TenantLifecycleManager (租户生命周期管理服务)
- **位置**: scripts/multitenant_lifecycle.cs
- **功能**: 租户生命周期的管理
- **特性**: 
  - 租户的创建和初始化
  - 租户的更新和配置
  - 租户的删除和清理
  - 租户状态的监控和管理

## 使用示例

### 基本用法

```csharp
// 获取租户管理服务
var tenantManager = serviceProvider.GetRequiredService<ITenantManager>();

// 创建新租户
var tenant = await tenantManager.CreateTenantAsync(new TenantCreateRequest
{
    TenantId = "tenant-001",
    Name = "示例租户",
    Description = "这是一个示例租户",
    ConnectionString = "Server=localhost;Database=tenant001;User Id=sa;Password=your_password;"
});

// 获取租户信息
var tenantInfo = await tenantManager.GetTenantAsync("tenant-001");
Console.WriteLine($"租户名称: {tenantInfo.Name}");

// 删除租户
await tenantManager.DeleteTenantAsync("tenant-001");
```

### 高级配置

```csharp
// 配置租户设置
var tenantSettings = new TenantSettings
{
    EnableResourceQuota = true,
    DefaultMaxConnections = 100,
    DefaultMaxStorage = 1024 * 1024 * 1024, // 1GB
    EnableDetailedLogging = true,
    ResourceCheckInterval = 60000, // 60秒
    EnableResourceThrottling = true
};

builder.Services.Configure<TenantSettings>(options =>
{
    options.EnableResourceQuota = tenantSettings.EnableResourceQuota;
    options.DefaultMaxConnections = tenantSettings.DefaultMaxConnections;
    options.DefaultMaxStorage = tenantSettings.DefaultMaxStorage;
    options.EnableDetailedLogging = tenantSettings.EnableDetailedLogging;
    options.ResourceCheckInterval = tenantSettings.ResourceCheckInterval;
    options.EnableResourceThrottling = tenantSettings.EnableResourceThrottling;
});
```

### 资源管理示例

```csharp
// 获取租户资源管理服务
var resourceManager = serviceProvider.GetRequiredService<ITenantResourceManager>();

// 检查租户资源使用情况
var resourceUsage = await resourceManager.GetResourceUsageAsync("tenant-001");
Console.WriteLine($"当前存储使用: {resourceUsage.StorageUsed / (1024 * 1024)}MB");
Console.WriteLine($"当前连接数: {resourceUsage.CurrentConnections}");

// 设置租户资源配额
await resourceManager.SetResourceQuotaAsync("tenant-001", new ResourceQuota
{
    MaxStorage = 2 * 1024 * 1024 * 1024, // 2GB
    MaxConnections = 200,
    MaxRequestsPerSecond = 1000
});

// 检查租户是否超出资源配额
var isOverQuota = await resourceManager.IsOverQuotaAsync("tenant-001");
if (isOverQuota)
{
    Console.WriteLine("租户已超出资源配额");
}
```

## 配置选项

### TenantSettings 配置

```json
{
  "TenantSettings": {
    "EnableResourceQuota": true,          // 启用资源配额
    "DefaultMaxConnections": 100,         // 默认最大连接数
    "DefaultMaxStorage": 1073741824,       // 默认最大存储(1GB)
    "EnableDetailedLogging": false,       // 启用详细日志
    "EnableTenantIsolation": true,        // 启用租户隔离
    "ResourceCheckInterval": 60000,       // 资源检查间隔(毫秒)
    "EnableResourceThrottling": true      // 启用资源限制
  }
}
```

### 运行配置 (run.json)

```json
{
  "name": "multitenant_integration",
  "description": "多租户集成示例",
  "command": "dotnet run --project multitenant_integration.cs",
  "workingDirectory": "d:\\Trae\\vsa\\skills\\multitenant\\scripts",
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "MULTITENANT_ENABLE_DETAILED_LOGGING": "true"
  },
  "options": {
    "publishAot": true,
    "selfContained": true,
    "runtimeIdentifier": "win-x64"
  }
}
```

## 性能优化

1. **缓存使用**: 启用租户信息缓存，减少数据库查询
2. **异步编程**: 使用异步API，避免阻塞主线程
3. **批量处理**: 对于多个租户的操作，使用批量处理减少网络往返
4. **连接池**: 使用连接池管理数据库连接，减少连接建立开销
5. **资源监控**: 定期监控租户资源使用情况，及时发现性能问题
6. **AOT编译**: 使用AOT编译优化，提高运行时性能
7. **内存优化**: 使用零拷贝技术和内存池，减少内存开销
8. **并发处理**: 使用并发集合和异步编程，充分利用多核CPU

## 故障排除

### 常见问题

1. **租户创建失败**
   - 检查租户ID是否已存在
   - 验证连接字符串是否正确
   - 检查数据库权限是否足够
   - 查看详细日志获取错误信息

2. **资源使用过高**
   - 检查租户是否超出资源配额
   - 优化租户应用程序的资源使用
   - 增加租户的资源配额限制
   - 考虑使用资源限制和节流机制

3. **性能下降**
   - 检查缓存是否启用
   - 优化数据库查询
   - 增加服务器资源
   - 考虑使用负载均衡

4. **跨租户访问**
   - 检查租户隔离设置是否启用
   - 验证访问控制是否正确配置
   - 确保租户数据访问逻辑正确

## 扩展开发

### 添加自定义功能

```csharp
// 自定义租户存储实现
public class CustomTenantRepository : ITenantRepository
{
    private readonly Dictionary<string, TenantInfo> _tenants = new();
    private readonly object _lock = new();

    public Task<TenantInfo> GetTenantAsync(string tenantId)
    {
        lock (_lock)
        {
            if (_tenants.TryGetValue(tenantId, out var tenant))
            {
                return Task.FromResult(tenant);
            }
            throw new TenantNotFoundException(tenantId);
        }
    }

    public Task<IEnumerable<TenantInfo>> GetAllTenantsAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(_tenants.Values.AsEnumerable());
        }
    }

    public Task AddTenantAsync(TenantInfo tenant)
    {
        lock (_lock)
        {
            if (_tenants.ContainsKey(tenant.TenantId))
            {
                throw new TenantAlreadyExistsException(tenant.TenantId);
            }
            _tenants[tenant.TenantId] = tenant;
            return Task.CompletedTask;
        }
    }

    public Task UpdateTenantAsync(TenantInfo tenant)
    {
        lock (_lock)
        {
            if (!_tenants.ContainsKey(tenant.TenantId))
            {
                throw new TenantNotFoundException(tenant.TenantId);
            }
            _tenants[tenant.TenantId] = tenant;
            return Task.CompletedTask;
        }
    }

    public Task DeleteTenantAsync(string tenantId)
    {
        lock (_lock)
        {
            if (!_tenants.Remove(tenantId))
            {
                throw new TenantNotFoundException(tenantId);
            }
            return Task.CompletedTask;
        }
    }
}

// 注册自定义存储实现
builder.Services.AddSingleton<ITenantRepository, CustomTenantRepository>();
```

### 扩展资源管理功能

```csharp
// 自定义资源管理实现
public class CustomTenantResourceManager : ITenantResourceManager
{
    private readonly Dictionary<string, ResourceUsage> _resourceUsage = new();
    private readonly Dictionary<string, ResourceQuota> _resourceQuotas = new();
    private readonly object _lock = new();

    public Task<ResourceUsage> GetResourceUsageAsync(string tenantId)
    {
        lock (_lock)
        {
            if (_resourceUsage.TryGetValue(tenantId, out var usage))
            {
                return Task.FromResult(usage);
            }
            // 返回默认使用情况
            return Task.FromResult(new ResourceUsage
            {
                TenantId = tenantId,
                StorageUsed = 0,
                CurrentConnections = 0,
                RequestsPerSecond = 0
            });
        }
    }

    public Task<ResourceQuota> GetResourceQuotaAsync(string tenantId)
    {
        lock (_lock)
        {
            if (_resourceQuotas.TryGetValue(tenantId, out var quota))
            {
                return Task.FromResult(quota);
            }
            // 返回默认配额
            return Task.FromResult(new ResourceQuota
            {
                TenantId = tenantId,
                MaxStorage = 1024 * 1024 * 1024, // 1GB
                MaxConnections = 100,
                MaxRequestsPerSecond = 1000
            });
        }
    }

    public Task SetResourceQuotaAsync(string tenantId, ResourceQuota quota)
    {
        lock (_lock)
        {
            _resourceQuotas[tenantId] = quota;
            return Task.CompletedTask;
        }
    }

    public Task<bool> IsOverQuotaAsync(string tenantId)
    {
        lock (_lock)
        {
            var usage = _resourceUsage.TryGetValue(tenantId, out var u) ? u : new ResourceUsage { TenantId = tenantId };
            var quota = _resourceQuotas.TryGetValue(tenantId, out var q) ? q : new ResourceQuota { TenantId = tenantId };

            return Task.FromResult(
                usage.StorageUsed > quota.MaxStorage ||
                usage.CurrentConnections > quota.MaxConnections ||
                usage.RequestsPerSecond > quota.MaxRequestsPerSecond
            );
        }
    }

    public Task UpdateResourceUsageAsync(string tenantId, ResourceUsage usage)
    {
        lock (_lock)
        {
            _resourceUsage[tenantId] = usage;
            return Task.CompletedTask;
        }
    }
}

// 注册自定义资源管理实现
builder.Services.AddSingleton<ITenantResourceManager, CustomTenantResourceManager>();
```

## 部署指南

### AOT编译部署

1. **配置AOT编译选项**
   - 在run.json中设置publishAot=true
   - 选择合适的运行时标识符(runtimeIdentifier)
   - 启用自包含部署(selfContained=true)

2. **编译和发布**
   ```bash
   # 编译AOT版本
   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishAot=true
   
   # 运行编译后的应用
   .\bin\Release\net11.0\win-x64\publish\multitenant_integration.exe
   ```

3. **容器化部署**
   ```dockerfile
   FROM mcr.microsoft.com/dotnet/runtime:10.0-nanoserver-2022 AS base
   WORKDIR /app

   FROM mcr.microsoft.com/dotnet/sdk:10.0-nanoserver-2022 AS build
   WORKDIR /src
   COPY . .
   RUN dotnet publish -c Release -r win-x64 --self-contained true -p:PublishAot=true -o /app/publish

   FROM base AS final
   WORKDIR /app
   COPY --from=build /app/publish .
   ENTRYPOINT ["multitenant_integration.exe"]
   ```

### 微服务部署

1. **服务注册与发现**
   - 使用Consul或Etcd进行服务注册与发现
   - 配置健康检查和负载均衡

2. **配置管理**
   - 使用配置中心管理多租户配置
   - 支持运行时配置更新

3. **监控与告警**
   - 集成Prometheus和Grafana监控
   - 配置资源使用告警

4. **日志管理**
   - 使用ELK或Loki进行日志收集和分析
   - 配置结构化日志格式

## 安全最佳实践

1. **租户隔离**
   - 确保租户数据完全隔离
   - 使用租户ID作为数据分区键
   - 实现严格的访问控制

2. **敏感信息保护**
   - 加密存储连接字符串等敏感信息
   - 使用密钥管理服务管理加密密钥
   - 避免在日志中记录敏感信息

3. **身份验证与授权**
   - 实现租户级别的身份验证
   - 使用基于角色的访问控制(RBAC)
   - 定期轮换访问令牌和密钥

4. **审计日志**
   - 记录所有租户操作的审计日志
   - 包括操作人、操作时间、操作内容等信息
   - 定期备份和分析审计日志

5. **安全更新**
   - 定期更新依赖包和框架版本
   - 及时修复安全漏洞
   - 进行定期安全审计

## 总结

多租户智能体技能提供了完整的多租户解决方案，支持租户隔离、资源管理、配额控制和生命周期管理等核心功能。通过AOT编译优化和高性能设计，它能够在高并发场景下提供卓越的性能。同时，它的可扩展架构使得开发者可以根据具体业务需求进行定制和扩展。

无论是构建SaaS平台、云服务还是企业级应用，多租户智能体技能都能够为您提供可靠、高效的多租户支持，帮助您快速构建和部署多租户应用。
