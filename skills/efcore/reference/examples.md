# efcore - 使用示例

## 快速开始

### 1. 基本数据库操作示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EFCoreAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 构建主机和服务容器
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("efcore_aot.setting.json", optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // 配置 EFCore 选项
                    services.Configure<EFCore.AOT.EFCoreOptions>(context.Configuration.GetSection("EFCore"));
                    
                    // 注册 EFCore 服务
                    services.AddEFCore();
                })
                .Build();

            Console.WriteLine("EFCore 基本数据库操作示例");
            Console.WriteLine("=" * 50);
            
            // 获取 EFCore 服务实例
            var efcoreService = host.Services.GetRequiredService<EFCore.AOT.IEFCoreService>();
            
            // 1. 执行数据库迁移
            Console.WriteLine("\n1. 执行数据库迁移");
            var migrateResult = await efcoreService.MigrateDatabaseAsync();
            if (migrateResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {migrateResult.ExecutionTimeMs}ms)");
                foreach (var result in migrateResult.Results)
                {
                    Console.WriteLine($"  {result}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {migrateResult.ErrorMessage}");
                return;
            }
            
            // 2. 插入示例数据
            Console.WriteLine("\n2. 插入示例数据");
            var insertResult = await efcoreService.InsertSampleDataAsync(3);
            if (insertResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {insertResult.ExecutionTimeMs}ms)");
                Console.WriteLine($"受影响行数: {insertResult.RowsAffected}");
            }
            else
            {
                Console.WriteLine($"失败: {insertResult.ErrorMessage}");
            }
            
            // 3. 查询用户数据
            Console.WriteLine("\n3. 查询用户数据");
            var userResult = await efcoreService.QueryDataAsync("users");
            if (userResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {userResult.ExecutionTimeMs}ms)");
                foreach (var result in userResult.Results)
                {
                    Console.WriteLine($"  {result}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {userResult.ErrorMessage}");
            }
            
            // 4. 查询订单数据
            Console.WriteLine("\n4. 查询订单数据");
            var orderResult = await efcoreService.QueryDataAsync("orders");
            if (orderResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {orderResult.ExecutionTimeMs}ms)");
                foreach (var result in orderResult.Results)
                {
                    Console.WriteLine($"  {result}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {orderResult.ErrorMessage}");
            }
            
            await host.RunAsync();
        }
    }
}
```

### 2. 高级配置示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace EFCoreAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("EFCore 高级配置示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务容器
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // 手动配置 EFCore 选项
                    services.Configure<EFCore.AOT.EFCoreOptions>(options => {
                        options.ConnectionString = "InMemoryDatabase=efcore-custom-db";
                        options.ProviderType = "InMemory";
                        options.EnableDetailedLogging = true;
                        options.EnablePerformanceMonitoring = true;
                        options.QueryTimeoutSeconds = 60;
                        options.BatchSize = 200;
                    });
                    
                    // 注册 EFCore 服务
                    services.AddEFCore();
                })
                .Build();
            
            // 获取配置信息
            var efcoreOptions = host.Services.GetRequiredService<IOptions<EFCore.AOT.EFCoreOptions>>().Value;
            Console.WriteLine("\n当前 EFCore 配置:");
            Console.WriteLine($"  连接字符串: {efcoreOptions.ConnectionString}");
            Console.WriteLine($"  数据库提供程序: {efcoreOptions.ProviderType}");
            Console.WriteLine($"  启用详细日志: {efcoreOptions.EnableDetailedLogging}");
            Console.WriteLine($"  启用性能监控: {efcoreOptions.EnablePerformanceMonitoring}");
            Console.WriteLine($"  查询超时: {efcoreOptions.QueryTimeoutSeconds}秒");
            Console.WriteLine($"  批量大小: {efcoreOptions.BatchSize}");
            
            // 获取 EFCore 服务实例
            var efcoreService = host.Services.GetRequiredService<EFCore.AOT.IEFCoreService>();
            
            // 执行操作
            Console.WriteLine("\n执行数据库创建操作");
            var createResult = await efcoreService.CreateDatabaseAsync();
            if (createResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {createResult.ExecutionTimeMs}ms)");
            }
            else
            {
                Console.WriteLine($"失败: {createResult.ErrorMessage}");
            }
            
            await host.RunAsync();
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
using Microsoft.Extensions.Hosting;

namespace EFCoreAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("EFCore 性能优化示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务容器
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // 配置 EFCore 选项，优化性能
                    services.Configure<EFCore.AOT.EFCoreOptions>(options => {
                        options.EnableDetailedLogging = false; // 生产环境关闭详细日志
                        options.QueryTimeoutSeconds = 30;
                        options.BatchSize = 500; // 增大批量处理大小
                    });
                    
                    // 注册 EFCore 服务
                    services.AddEFCore();
                })
                .Build();
            
            // 获取 EFCore 服务实例
            var efcoreService = host.Services.GetRequiredService<EFCore.AOT.IEFCoreService>();
            
            // 1. 执行数据库迁移
            await efcoreService.MigrateDatabaseAsync();
            
            // 2. 性能测试：插入大量数据
            Console.WriteLine("\n2. 性能测试：插入数据");
            
            // 测试不同数量级的数据插入
            int[] testCounts = { 1, 5, 10, 50 };
            foreach (var count in testCounts)
            {
                var stopwatch = Stopwatch.StartNew();
                
                // 插入数据
                var result = await efcoreService.InsertSampleDataAsync(count);
                
                stopwatch.Stop();
                
                if (result.Success)
                {
                    Console.WriteLine($"  插入 {count} 条记录: {stopwatch.ElapsedMilliseconds}ms");
                    Console.WriteLine($"  每行平均: {stopwatch.ElapsedMilliseconds / (double)result.RowsAffected:F2}ms");
                }
                else
                {
                    Console.WriteLine($"  插入 {count} 条记录: 失败");
                }
            }
            
            // 3. 性能测试：查询数据
            Console.WriteLine("\n3. 性能测试：查询数据");
            
            var queryStopwatch = Stopwatch.StartNew();
            var queryResult = await efcoreService.QueryDataAsync("users");
            queryStopwatch.Stop();
            
            if (queryResult.Success)
            {
                Console.WriteLine($"  查询 {queryResult.RowsAffected} 条用户记录: {queryStopwatch.ElapsedMilliseconds}ms");
            }
            
            await host.RunAsync();
        }
    }
}
```

### 4. 错误处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EFCoreAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("EFCore 错误处理示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务容器
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // 注册 EFCore 服务
                    services.AddEFCore();
                })
                .Build();
            
            // 获取 EFCore 服务实例
            var efcoreService = host.Services.GetRequiredService<EFCore.AOT.IEFCoreService>();
            
            // 1. 测试无效的查询类型
            Console.WriteLine("\n1. 测试无效的查询类型");
            try
            {
                var result = await efcoreService.QueryDataAsync("invalid_type");
                Console.WriteLine($"结果: {result.Success}");
                foreach (var message in result.Results)
                {
                    Console.WriteLine($"  {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异常: {ex.Message}");
            }
            
            // 2. 测试数据库操作（先删除再查询）
            Console.WriteLine("\n2. 测试数据库操作（先删除再查询）");
            
            // 删除数据库
            await efcoreService.DropDatabaseAsync();
            
            // 尝试查询（应该失败）
            try
            {
                var result = await efcoreService.QueryDataAsync("users");
                Console.WriteLine($"查询结果: {result.Success}");
                if (!result.Success)
                {
                    Console.WriteLine($"错误信息: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"查询异常: {ex.Message}");
            }
            
            // 恢复数据库
            await efcoreService.CreateDatabaseAsync();
            
            Console.WriteLine("\n错误处理示例完成");
            await host.RunAsync();
        }
    }
}
```

### 5. 命令行工具示例

```bash
# 查看帮助信息
efcore_aot.exe help
efcore_aot.exe --help
efcore_aot.exe -h

# 执行数据库迁移
efcore_aot.exe migrate

# 创建数据库
efcore_aot.exe create-db

# 删除数据库
efcore_aot.exe drop-db

# 查询用户数据
efcore_aot.exe query users

# 查询订单数据
efcore_aot.exe query orders

# 插入示例数据
efcore_aot.exe insert          # 插入1条
efcore_aot.exe insert 3        # 插入3条
efcore_aot.exe insert 10       # 插入10条

# 查看版本信息
efcore_aot.exe version
```

## 总结

以上示例展示了 EFCore 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本的数据库操作
2. 学习高级配置选项
3. 了解性能优化策略
4. 掌握错误处理方法
5. 使用命令行工具进行便捷操作

该系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的数据库应用场景。所有示例都支持 .NET 10 AOT 编译，提供极致的性能表现。

## 最佳实践建议

1. **依赖注入**：始终使用依赖注入管理 EFCore 服务
2. **异步编程**：优先使用异步 API，提高系统并发能力
3. **合理配置**：根据实际需求调整数据库连接和查询设置
4. **错误处理**：妥善处理数据库操作中的异常
5. **日志记录**：在开发环境启用详细日志，生产环境简化日志
6. **性能监控**：定期监控数据库操作性能
7. **批量处理**：对于大量数据操作，使用批量处理提高效率
8. **AOT 优化**：利用 AOT 编译提升应用程序性能和启动速度
