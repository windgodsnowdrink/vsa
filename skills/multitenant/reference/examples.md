# 多租户智能体技能 - 使用示例

## 快速开始

### 1. 基本用法示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using multitenant;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var tenantManager = serviceProvider.GetRequiredService<ITenantManager>();
        
        Console.WriteLine("多租户基本用法示例");
        Console.WriteLine("=" * 50);
        
        try
        {
            // 创建新租户
            Console.WriteLine("1. 创建新租户...");
            var tenant = await tenantManager.CreateTenantAsync(new TenantCreateRequest
            {
                TenantId = "tenant-001",
                Name = "示例租户",
                Description = "这是一个示例租户",
                ConnectionString = "Server=localhost;Database=tenant001;User Id=sa;Password=your_password;"
            });
            Console.WriteLine($"租户创建成功: {tenant.Name}");
            
            // 获取租户信息
            Console.WriteLine("\n2. 获取租户信息...");
            var tenantInfo = await tenantManager.GetTenantAsync("tenant-001");
            Console.WriteLine($"租户ID: {tenantInfo.TenantId}");
            Console.WriteLine($"租户名称: {tenantInfo.Name}");
            Console.WriteLine($"租户描述: {tenantInfo.Description}");
            
            // 更新租户信息
            Console.WriteLine("\n3. 更新租户信息...");
            await tenantManager.UpdateTenantAsync("tenant-001", new TenantUpdateRequest
            {
                Name = "更新后的示例租户",
                Description = "这是一个更新后的示例租户"
            });
            Console.WriteLine("租户信息更新成功");
            
            // 再次获取租户信息
            var updatedTenant = await tenantManager.GetTenantAsync("tenant-001");
            Console.WriteLine($"更新后的租户名称: {updatedTenant.Name}");
            Console.WriteLine($"更新后的租户描述: {updatedTenant.Description}");
            
            // 删除租户
            Console.WriteLine("\n4. 删除租户...");
            await tenantManager.DeleteTenantAsync("tenant-001");
            Console.WriteLine("租户删除成功");
            
        } catch (Exception ex) {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册多租户服务
        builder.AddSingleton<ITenantManager, TenantManager>();
        builder.AddSingleton<ITenantRepository, InMemoryTenantRepository>();
        builder.AddSingleton<ITenantResourceManager, DefaultTenantResourceManager>();
        
        // 配置多租户设置
        builder.Configure<TenantSettings>(options =>
        {
            options.EnableResourceQuota = true;
            options.DefaultMaxConnections = 100;
            options.DefaultMaxStorage = 1024 * 1024 * 1024; // 1GB
            options.EnableDetailedLogging = true;
        });
        
        return builder.BuildServiceProvider();
    }
}
```

### 2. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("多租户高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置多租户设置
        builder.Configure<TenantSettings>(options => {
            options.EnableResourceQuota = true;
            options.DefaultMaxConnections = 200;
            options.DefaultMaxStorage = 2 * 1024 * 1024 * 1024; // 2GB
            options.EnableDetailedLogging = true;
            options.ResourceCheckInterval = 30000; // 30秒
            options.EnableResourceThrottling = true;
        });
        
        // 注册服务
        builder.AddSingleton<ITenantManager, TenantManager>();
        builder.AddSingleton<ITenantRepository, InMemoryTenantRepository>();
        builder.AddSingleton<ITenantResourceManager, DefaultTenantResourceManager>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<TenantSettings>>().Value;
        Console.WriteLine($"配置信息:");
        Console.WriteLine($"  启用资源配额: {settings.EnableResourceQuota}");
        Console.WriteLine($"  默认最大连接数: {settings.DefaultMaxConnections}");
        Console.WriteLine($"  默认最大存储: {settings.DefaultMaxStorage / (1024 * 1024)}MB");
        Console.WriteLine($"  启用详细日志: {settings.EnableDetailedLogging}");
        Console.WriteLine($"  资源检查间隔: {settings.ResourceCheckInterval / 1000}秒");
        Console.WriteLine($"  启用资源限制: {settings.EnableResourceThrottling}");
        
        // 使用服务
        var tenantManager = serviceProvider.GetRequiredService<ITenantManager>();
        
        try
        {
            // 创建租户
            var tenant = await tenantManager.CreateTenantAsync(new TenantCreateRequest
            {
                TenantId = "tenant-002",
                Name = "高级配置示例租户",
                Description = "这是一个使用高级配置的示例租户",
                ConnectionString = "Server=localhost;Database=tenant002;User Id=sa;Password=your_password;"
            });
            Console.WriteLine($"\n租户创建成功: {tenant.Name}");
            
            // 删除租户
            await tenantManager.DeleteTenantAsync("tenant-002");
            Console.WriteLine("租户删除成功");
            
        } catch (Exception ex) {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

### 3. 性能优化示例

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("多租户性能优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var tenantManager = serviceProvider.GetRequiredService<ITenantManager>();
        
        // 准备测试数据
        Console.WriteLine("准备测试数据...");
        for (int i = 1; i <= 10; i++)
        {
            await tenantManager.CreateTenantAsync(new TenantCreateRequest
            {
                TenantId = $"tenant-perf-{i}",
                Name = $"性能测试租户{i}",
                Description = $"这是一个性能测试租户{i}",
                ConnectionString = $"Server=localhost;Database=tenantperf{i};User Id=sa;Password=your_password;"
            });
        }
        Console.WriteLine("测试数据准备完成");
        
        // 性能测试 - 获取租户信息
        Console.WriteLine("\n1. 测试获取租户信息性能...");
        const int iterations = 1000;
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            var tenantId = $"tenant-perf-{((i % 10) + 1)}";
            await tenantManager.GetTenantAsync(tenantId);
        }
        
        stopwatch.Stop();
        Console.WriteLine($"执行 {iterations} 次获取租户信息操作的时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"每次操作平均时间: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
        
        // 性能测试 - 批量获取租户
        Console.WriteLine("\n2. 测试批量获取租户性能...");
        stopwatch.Restart();
        
        for (int i = 0; i < 100; i++)
        {
            await tenantManager.GetAllTenantsAsync();
        }
        
        stopwatch.Stop();
        Console.WriteLine($"执行 100 次批量获取租户操作的时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"每次操作平均时间: {stopwatch.Elapsed.TotalMilliseconds / 100:F3} ms");
        
        // 清理测试数据
        Console.WriteLine("\n清理测试数据...");
        for (int i = 1; i <= 10; i++)
        {
            await tenantManager.DeleteTenantAsync($"tenant-perf-{i}");
        }
        Console.WriteLine("测试数据清理完成");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册多租户服务
        builder.AddSingleton<ITenantManager, TenantManager>();
        builder.AddSingleton<ITenantRepository, InMemoryTenantRepository>();
        builder.AddSingleton<ITenantResourceManager, DefaultTenantResourceManager>();
        
        // 配置多租户设置
        builder.Configure<TenantSettings>(options =>
        {
            options.EnableResourceQuota = false; // 禁用资源配额以提高性能
            options.EnableDetailedLogging = false; // 禁用详细日志以提高性能
        });
        
        return builder.BuildServiceProvider();
    }
}
```

### 4. 错误处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("多租户错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var tenantManager = serviceProvider.GetRequiredService<ITenantManager>();
        
        try
        {
            // 测试1: 创建已存在的租户
            Console.WriteLine("1. 测试创建已存在的租户...");
            await tenantManager.CreateTenantAsync(new TenantCreateRequest
            {
                TenantId = "tenant-error-001",
                Name = "错误测试租户",
                Description = "这是一个错误测试租户",
                ConnectionString = "Server=localhost;Database=tenanterror001;User Id=sa;Password=your_password;"
            });
            Console.WriteLine("租户创建成功");
            
            // 再次创建相同ID的租户
            try
            {
                await tenantManager.CreateTenantAsync(new TenantCreateRequest
                {
                    TenantId = "tenant-error-001",
                    Name = "重复租户",
                    Description = "这是一个重复的租户",
                    ConnectionString = "Server=localhost;Database=tenanterror001;User Id=sa;Password=your_password;"
                });
            } catch (TenantAlreadyExistsException ex) {
                Console.WriteLine($"预期的错误: {ex.Message}");
            }
            
            // 测试2: 获取不存在的租户
            Console.WriteLine("\n2. 测试获取不存在的租户...");
            try
            {
                await tenantManager.GetTenantAsync("non-existent-tenant");
            } catch (TenantNotFoundException ex) {
                Console.WriteLine($"预期的错误: {ex.Message}");
            }
            
            // 测试3: 更新不存在的租户
            Console.WriteLine("\n3. 测试更新不存在的租户...");
            try
            {
                await tenantManager.UpdateTenantAsync("non-existent-tenant", new TenantUpdateRequest
                {
                    Name = "更新不存在的租户",
                    Description = "这是一个不存在的租户"
                });
            } catch (TenantNotFoundException ex) {
                Console.WriteLine($"预期的错误: {ex.Message}");
            }
            
            // 测试4: 删除不存在的租户
            Console.WriteLine("\n4. 测试删除不存在的租户...");
            try
            {
                await tenantManager.DeleteTenantAsync("non-existent-tenant");
            } catch (TenantNotFoundException ex) {
                Console.WriteLine($"预期的错误: {ex.Message}");
            }
            
        } catch (Exception ex) {
            Console.WriteLine($"未预期的错误: {ex.Message}");
        } finally {
            // 清理测试数据
            try
            {
                await tenantManager.DeleteTenantAsync("tenant-error-001");
            } catch {}
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册多租户服务
        builder.AddSingleton<ITenantManager, TenantManager>();
        builder.AddSingleton<ITenantRepository, InMemoryTenantRepository>();
        builder.AddSingleton<ITenantResourceManager, DefaultTenantResourceManager>();
        
        // 配置多租户设置
        builder.Configure<TenantSettings>(options =>
        {
            options.EnableDetailedLogging = true;
        });
        
        return builder.BuildServiceProvider();
    }
}
```

### 5. 资源管理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("多租户资源管理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var tenantManager = serviceProvider.GetRequiredService<ITenantManager>();
        var resourceManager = serviceProvider.GetRequiredService<ITenantResourceManager>();
        
        try
        {
            // 创建租户
            Console.WriteLine("1. 创建租户...");
            await tenantManager.CreateTenantAsync(new TenantCreateRequest
            {
                TenantId = "tenant-resource-001",
                Name = "资源管理示例租户",
                Description = "这是一个资源管理示例租户",
                ConnectionString = "Server=localhost;Database=tenantresource001;User Id=sa;Password=your_password;"
            });
            Console.WriteLine("租户创建成功");
            
            // 获取资源使用情况
            Console.WriteLine("\n2. 获取资源使用情况...");
            var resourceUsage = await resourceManager.GetResourceUsageAsync("tenant-resource-001");
            Console.WriteLine($"租户ID: {resourceUsage.TenantId}");
            Console.WriteLine($"当前存储使用: {resourceUsage.StorageUsed / (1024 * 1024)}MB");
            Console.WriteLine($"当前连接数: {resourceUsage.CurrentConnections}");
            Console.WriteLine($"当前请求率: {resourceUsage.RequestsPerSecond} req/s");
            
            // 设置资源配额
            Console.WriteLine("\n3. 设置资源配额...");
            await resourceManager.SetResourceQuotaAsync("tenant-resource-001", new ResourceQuota
            {
                MaxStorage = 2 * 1024 * 1024 * 1024, // 2GB
                MaxConnections = 200,
                MaxRequestsPerSecond = 1000
            });
            Console.WriteLine("资源配额设置成功");
            
            // 获取资源配额
            Console.WriteLine("\n4. 获取资源配额...");
            var resourceQuota = await resourceManager.GetResourceQuotaAsync("tenant-resource-001");
            Console.WriteLine($"租户ID: {resourceQuota.TenantId}");
            Console.WriteLine($"最大存储: {resourceQuota.MaxStorage / (1024 * 1024)}MB");
            Console.WriteLine($"最大连接数: {resourceQuota.MaxConnections}");
            Console.WriteLine($"最大请求率: {resourceQuota.MaxRequestsPerSecond} req/s");
            
            // 检查是否超出配额
            Console.WriteLine("\n5. 检查是否超出配额...");
            var isOverQuota = await resourceManager.IsOverQuotaAsync("tenant-resource-001");
            Console.WriteLine($"是否超出配额: {isOverQuota}");
            
            // 模拟资源使用
            Console.WriteLine("\n6. 模拟资源使用...");
            await resourceManager.UpdateResourceUsageAsync("tenant-resource-001", new ResourceUsage
            {
                TenantId = "tenant-resource-001",
                StorageUsed = 512 * 1024 * 1024, // 512MB
                CurrentConnections = 50,
                RequestsPerSecond = 200
            });
            Console.WriteLine("资源使用情况更新成功");
            
            // 再次检查资源使用情况
            resourceUsage = await resourceManager.GetResourceUsageAsync("tenant-resource-001");
            Console.WriteLine($"更新后的存储使用: {resourceUsage.StorageUsed / (1024 * 1024)}MB");
            Console.WriteLine($"更新后的连接数: {resourceUsage.CurrentConnections}");
            Console.WriteLine($"更新后的请求率: {resourceUsage.RequestsPerSecond} req/s");
            
        } catch (Exception ex) {
            Console.WriteLine($"错误: {ex.Message}");
        } finally {
            // 清理测试数据
            try
            {
                await tenantManager.DeleteTenantAsync("tenant-resource-001");
                Console.WriteLine("\n测试数据清理完成");
            } catch {}
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册多租户服务
        builder.AddSingleton<ITenantManager, TenantManager>();
        builder.AddSingleton<ITenantRepository, InMemoryTenantRepository>();
        builder.AddSingleton<ITenantResourceManager, DefaultTenantResourceManager>();
        
        // 配置多租户设置
        builder.Configure<TenantSettings>(options =>
        {
            options.EnableResourceQuota = true;
            options.DefaultMaxConnections = 100;
            options.DefaultMaxStorage = 1024 * 1024 * 1024; // 1GB
            options.EnableDetailedLogging = true;
        });
        
        return builder.BuildServiceProvider();
    }
}
```

### 6. 租户生命周期管理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("多租户生命周期管理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var lifecycleManager = serviceProvider.GetRequiredService<TenantLifecycleManager>();
        var tenantManager = serviceProvider.GetRequiredService<ITenantManager>();
        
        try
        {
            // 测试1: 创建并初始化租户
            Console.WriteLine("1. 创建并初始化租户...");
            var tenantContext = await lifecycleManager.CreateAndInitializeTenantAsync(new TenantCreateRequest
            {
                TenantId = "tenant-lifecycle-001",
                Name = "生命周期管理示例租户",
                Description = "这是一个生命周期管理示例租户",
                ConnectionString = "Server=localhost;Database=tenantlifecycle001;User Id=sa;Password=your_password;"
            });
            Console.WriteLine($"租户创建并初始化成功: {tenantContext.Tenant.Name}");
            Console.WriteLine($"租户状态: {tenantContext.Status}");
            
            // 测试2: 暂停租户
            Console.WriteLine("\n2. 暂停租户...");
            await lifecycleManager.SuspendTenantAsync("tenant-lifecycle-001");
            var suspendedContext = await lifecycleManager.GetTenantContextAsync("tenant-lifecycle-001");
            Console.WriteLine($"租户暂停成功");
            Console.WriteLine($"租户状态: {suspendedContext.Status}");
            
            // 测试3: 恢复租户
            Console.WriteLine("\n3. 恢复租户...");
            await lifecycleManager.ResumeTenantAsync("tenant-lifecycle-001");
            var resumedContext = await lifecycleManager.GetTenantContextAsync("tenant-lifecycle-001");
            Console.WriteLine($"租户恢复成功");
            Console.WriteLine($"租户状态: {resumedContext.Status}");
            
            // 测试4: 备份租户
            Console.WriteLine("\n4. 备份租户...");
            var backupResult = await lifecycleManager.BackupTenantAsync("tenant-lifecycle-001", "backup-20240101");
            Console.WriteLine($"租户备份成功");
            Console.WriteLine($"备份ID: {backupResult.BackupId}");
            Console.WriteLine($"备份时间: {backupResult.BackupTime}");
            Console.WriteLine($"备份大小: {backupResult.BackupSize / (1024 * 1024)}MB");
            
            // 测试5: 清理租户资源
            Console.WriteLine("\n5. 清理租户资源...");
            await lifecycleManager.CleanupTenantResourcesAsync("tenant-lifecycle-001");
            Console.WriteLine("租户资源清理成功");
            
            // 测试6: 删除并清理租户
            Console.WriteLine("\n6. 删除并清理租户...");
            await lifecycleManager.DeleteAndCleanupTenantAsync("tenant-lifecycle-001");
            Console.WriteLine("租户删除并清理成功");
            
            // 验证租户是否已删除
            try
            {
                await tenantManager.GetTenantAsync("tenant-lifecycle-001");
                Console.WriteLine("租户仍然存在");
            } catch (TenantNotFoundException) {
                Console.WriteLine("租户已成功删除");
            }
            
        } catch (Exception ex) {
            Console.WriteLine($"错误: {ex.Message}");
        } finally {
            // 清理测试数据
            try
            {
                await tenantManager.DeleteTenantAsync("tenant-lifecycle-001");
            } catch {}
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册多租户服务
        builder.AddSingleton<ITenantManager, TenantManager>();
        builder.AddSingleton<ITenantRepository, InMemoryTenantRepository>();
        builder.AddSingleton<ITenantResourceManager, DefaultTenantResourceManager>();
        builder.AddSingleton<TenantLifecycleManager>();
        
        // 配置多租户设置
        builder.Configure<TenantSettings>(options =>
        {
            options.EnableResourceQuota = true;
            options.DefaultMaxConnections = 100;
            options.DefaultMaxStorage = 1024 * 1024 * 1024; // 1GB
            options.EnableDetailedLogging = true;
        });
        
        return builder.BuildServiceProvider();
    }
}
```

## 总结

以上示例演示了多租户智能体技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速开始使用基本操作
2. 配置高级选项以满足特定需求
3. 优化性能以提高系统效率
4. 处理各种错误情况
5. 管理租户资源使用情况
6. 管理租户的完整生命周期

系统设计遵循.NET 10最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

通过AOT编译优化和高性能设计，多租户智能体技能能够在高并发场景下提供卓越的性能，同时保持代码的清晰性和可维护性。

无论是构建SaaS平台、云服务还是企业级应用，多租户智能体技能都能够为您提供可靠、高效的多租户支持，帮助您快速构建和部署多租户应用。
