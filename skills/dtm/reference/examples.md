# dtm - 使用示例

## 快速开始

### 1. 基本分布式事务执行示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DtmAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 构建主机和服务容器
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("dtm_aot.setting.json", optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // 配置 DTM 选项
                    services.Configure<DTM.AOT.DtmOptions>(context.Configuration.GetSection("Dtm"));
                    
                    // 注册 DTM 服务
                    services.AddDtm();
                })
                .Build();

            Console.WriteLine("DTM 基本分布式事务执行示例");
            Console.WriteLine("=" * 50);
            
            // 获取 DTM 服务实例
            var dtmService = host.Services.GetRequiredService<DTM.AOT.IDtmService>();
            
            // 1. 执行分布式事务
            Console.WriteLine("\n1. 执行分布式事务");
            List<string> operations = new List<string> {
                "operation1:create_order",
                "operation2:deduct_inventory",
                "operation3:process_payment"
            };
            
            var transactionResult = await dtmService.ExecuteTransactionAsync(null, operations, 10000);
            if (transactionResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {transactionResult.ExecutionTimeMs}ms)");
                Console.WriteLine($"事务ID: {transactionResult.TransactionId}");
                Console.WriteLine($"操作数量: {operations.Count}");
                foreach (var result in transactionResult.Results)
                {
                    Console.WriteLine($"  {result}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {transactionResult.ErrorMessage}");
            }
            
            // 2. 查询事务状态
            Console.WriteLine("\n2. 查询事务状态");
            if (transactionResult.Success && !string.IsNullOrEmpty(transactionResult.TransactionId))
            {
                var statusResult = await dtmService.QueryTransactionStatusAsync(transactionResult.TransactionId);
                if (statusResult.Success)
                {
                    Console.WriteLine($"成功 (耗时: {statusResult.ExecutionTimeMs}ms)");
                    Console.WriteLine($"事务ID: {statusResult.TransactionId}");
                    foreach (var result in statusResult.Results)
                    {
                        Console.WriteLine($"  {result}");
                    }
                }
                else
                {
                    Console.WriteLine($"失败: {statusResult.ErrorMessage}");
                }
            }
            
            await host.RunAsync();
        }
    }
}
```

### 2. 事务取消和恢复示例

```csharp
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DtmAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("DTM 事务取消和恢复示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务容器
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // 配置 DTM 选项
                    services.Configure<DTM.AOT.DtmOptions>(options => {
                        options.DefaultTimeoutMs = 10000;
                        options.EnableDetailedLogging = true;
                    });
                    
                    // 注册 DTM 服务
                    services.AddDtm();
                })
                .Build();
            
            // 获取 DTM 服务实例
            var dtmService = host.Services.GetRequiredService<DTM.AOT.IDtmService>();
            
            // 1. 首先创建一个事务
            Console.WriteLine("\n1. 创建测试事务");
            List<string> operations = new List<string> { "operation1", "operation2" };
            var createResult = await dtmService.ExecuteTransactionAsync("test-tx-123", operations);
            
            if (createResult.Success)
            {
                Console.WriteLine($"事务创建成功，ID: {createResult.TransactionId}");
                
                // 2. 取消事务
                Console.WriteLine("\n2. 取消事务");
                var cancelResult = await dtmService.CancelTransactionAsync("test-tx-123");
                if (cancelResult.Success)
                {
                    Console.WriteLine($"事务取消成功 (耗时: {cancelResult.ExecutionTimeMs}ms)");
                    foreach (var result in cancelResult.Results)
                    {
                        Console.WriteLine($"  {result}");
                    }
                }
                else
                {
                    Console.WriteLine($"事务取消失败: {cancelResult.ErrorMessage}");
                }
                
                // 3. 恢复事务
                Console.WriteLine("\n3. 恢复事务");
                var resumeResult = await dtmService.ResumeTransactionAsync("test-tx-123");
                if (resumeResult.Success)
                {
                    Console.WriteLine($"事务恢复成功 (耗时: {resumeResult.ExecutionTimeMs}ms)");
                    foreach (var result in resumeResult.Results)
                    {
                        Console.WriteLine($"  {result}");
                    }
                }
                else
                {
                    Console.WriteLine($"事务恢复失败: {resumeResult.ErrorMessage}");
                }
                
                // 4. 再次查询事务状态
                Console.WriteLine("\n4. 查询恢复后的事务状态");
                var statusResult = await dtmService.QueryTransactionStatusAsync("test-tx-123");
                if (statusResult.Success)
                {
                    Console.WriteLine($"状态查询成功 (耗时: {statusResult.ExecutionTimeMs}ms)");
                    foreach (var result in statusResult.Results)
                    {
                        Console.WriteLine($"  {result}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"事务创建失败: {createResult.ErrorMessage}");
            }
            
            await host.RunAsync();
        }
    }
}
```

### 3. 事务管理和清理示例

```csharp
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DtmAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("DTM 事务管理和清理示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务容器
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // 注册 DTM 服务
                    services.AddDtm();
                })
                .Build();
            
            // 获取 DTM 服务实例
            var dtmService = host.Services.GetRequiredService<DTM.AOT.IDtmService>();
            
            // 1. 批量创建测试事务
            Console.WriteLine("\n1. 批量创建测试事务");
            int testCount = 5;
            List<string> transactionIds = new List<string>();
            
            for (int i = 0; i < testCount; i++)
            {
                List<string> operations = new List<string> {
                    $"operation1:test-{i}",
                    $"operation2:test-{i}"
                };
                
                var result = await dtmService.ExecuteTransactionAsync(null, operations);
                if (result.Success && !string.IsNullOrEmpty(result.TransactionId))
                {
                    transactionIds.Add(result.TransactionId);
                    Console.WriteLine($"  事务 {i+1}: {result.TransactionId}");
                }
            }
            
            // 2. 查询所有事务状态
            Console.WriteLine($"\n2. 查询 {transactionIds.Count} 个事务状态");
            foreach (var transactionId in transactionIds)
            {
                var result = await dtmService.QueryTransactionStatusAsync(transactionId);
                if (result.Success)
                {
                    Console.WriteLine($"  {transactionId}: 成功");
                }
                else
                {
                    Console.WriteLine($"  {transactionId}: 失败");
                }
            }
            
            // 3. 清理过期事务
            Console.WriteLine("\n3. 清理过期事务");
            var cleanResult = await dtmService.CleanExpiredTransactionsAsync();
            if (cleanResult.Success)
            {
                Console.WriteLine($"清理完成 (耗时: {cleanResult.ExecutionTimeMs}ms)");
                foreach (var result in cleanResult.Results)
                {
                    Console.WriteLine($"  {result}");
                }
            }
            else
            {
                Console.WriteLine($"清理失败: {cleanResult.ErrorMessage}");
            }
            
            // 4. 查询版本信息
            Console.WriteLine("\n4. 查询版本信息");
            var versionResult = await dtmService.GetVersionInfoAsync();
            if (versionResult.Success)
            {
                Console.WriteLine($"版本信息 (耗时: {versionResult.ExecutionTimeMs}ms)");
                foreach (var result in versionResult.Results)
                {
                    Console.WriteLine($"  {result}");
                }
            }
            
            await host.RunAsync();
        }
    }
}
```

### 4. 命令行工具示例

```bash
# 查看帮助信息
dtm_aot.exe help
dtm_aot.exe --help
dtm_aot.exe -h

# 执行分布式事务
dtm_aot.exe execute 'op1,op2,op3'                          # 使用自动生成的事务ID
dtm_aot.exe execute 'op1,op2' tx-2024-01-03-001            # 使用指定的事务ID

# 查询事务状态
dtm_aot.exe query tx-2024-01-03-001

# 取消事务
dtm_aot.exe cancel tx-2024-01-03-001

# 恢复事务
dtm_aot.exe resume tx-2024-01-03-001

# 清理过期事务
dtm_aot.exe clean

# 查看版本信息
dtm_aot.exe version

# 复杂事务示例
dtm_aot.exe execute 'create_order:1001,deduct_inventory:product1:5,process_payment:user1:299.99' e-commerce-tx-001
```

## 总结

以上示例展示了 DTM 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本的分布式事务执行操作
2. 学习事务状态查询和管理
3. 掌握事务取消和恢复的使用
4. 了解过期事务清理机制
5. 使用命令行工具进行便捷操作

该系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的分布式事务场景。所有示例都支持 .NET 10 AOT 编译，提供极致的性能表现。
