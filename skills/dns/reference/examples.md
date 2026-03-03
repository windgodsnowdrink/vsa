# dns - 使用示例

## 快速开始

### 1. 基本 DNS 查询示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DnsAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 构建主机和服务容器
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("dns_aot.setting.json", optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // 配置 DNS 选项
                    services.Configure<DnsOptions>(context.Configuration.GetSection("Dns"));
                    
                    // 注册 DNS 服务
                    services.AddDns();
                })
                .Build();

            Console.WriteLine("DNS 基本查询示例");
            Console.WriteLine("=" * 50);
            
            // 获取 DNS 服务实例
            var dnsService = host.Services.GetRequiredService<IDnsService>();
            
            // 查询 A 记录（IPv4）
            Console.WriteLine("\n1. 查询 A 记录 (IPv4): example.com");
            var aResult = await dnsService.QueryAAsync("example.com");
            if (aResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {aResult.ExecutionTimeMs}ms)");
                foreach (var ip in aResult.Results)
                {
                    Console.WriteLine($"  - {ip}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {aResult.ErrorMessage}");
            }
            
            // 查询 AAAA 记录（IPv6）
            Console.WriteLine("\n2. 查询 AAAA 记录 (IPv6): example.com");
            var aaaaResult = await dnsService.QueryAAAAAsync("example.com");
            if (aaaaResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {aaaaResult.ExecutionTimeMs}ms)");
                foreach (var ip in aaaaResult.Results)
                {
                    Console.WriteLine($"  - {ip}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {aaaaResult.ErrorMessage}");
            }
            
            // 查询 MX 记录（邮件）
            Console.WriteLine("\n3. 查询 MX 记录 (邮件): gmail.com");
            var mxResult = await dnsService.QueryMXAsync("gmail.com");
            if (mxResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {mxResult.ExecutionTimeMs}ms)");
                foreach (var mx in mxResult.Results)
                {
                    Console.WriteLine($"  - {mx}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {mxResult.ErrorMessage}");
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

namespace DnsAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("DNS 高级配置示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务容器
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // 手动配置 DNS 选项
                    services.Configure<DnsOptions>(options => {
                        options.EnableCache = true;
                        options.CacheSize = 2000;
                        options.CacheExpirationSeconds = 600;
                        options.TimeoutMs = 3000;
                        options.DnsServers = new List<string> { "1.1.1.1", "1.0.0.1" };
                        options.EnableDetailedLogging = true;
                        options.UseSystemDns = false;
                    });
                    
                    // 注册 DNS 服务
                    services.AddDns();
                })
                .Build();
            
            // 获取配置
            var settings = host.Services.GetRequiredService<IOptions<DnsOptions>>().Value;
            Console.WriteLine($"\nDNS 配置:");
            Console.WriteLine($"  启用缓存: {settings.EnableCache}");
            Console.WriteLine($"  缓存大小: {settings.CacheSize}");
            Console.WriteLine($"  缓存过期时间: {settings.CacheExpirationSeconds}秒");
            Console.WriteLine($"  查询超时: {settings.TimeoutMs}ms");
            Console.WriteLine($"  DNS 服务器: {string.Join(", ", settings.DnsServers)}");
            Console.WriteLine($"  使用系统 DNS: {settings.UseSystemDns}");
            
            // 使用服务
            var dnsService = host.Services.GetRequiredService<IDnsService>();
            
            Console.WriteLine("\n查询 A 记录: github.com");
            var result = await dnsService.QueryAAsync("github.com");
            if (result.Success)
            {
                Console.WriteLine($"成功 (耗时: {result.ExecutionTimeMs}ms)");
                foreach (var ip in result.Results)
                {
                    Console.WriteLine($"  - {ip}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {result.ErrorMessage}");
            }
            
            await host.RunAsync();
        }
    }
}
```

### 3. 批量查询示例

```csharp
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DnsAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("DNS 批量查询示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务容器
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.Configure<DnsOptions>(context.Configuration.GetSection("Dns"));
                    services.AddDns();
                })
                .Build();
            
            var dnsService = host.Services.GetRequiredService<IDnsService>();
            
            // 准备批量查询任务
            var queries = new List<(string Domain, DnsRecordType RecordType)> {
                ("example.com", DnsRecordType.A),
                ("example.com", DnsRecordType.AAAA),
                ("gmail.com", DnsRecordType.MX),
                ("github.com", DnsRecordType.NS),
                ("google.com", DnsRecordType.TXT),
                ("microsoft.com", DnsRecordType.CNAME)
            };
            
            Console.WriteLine($"\n开始批量查询 {queries.Count} 个记录...");
            
            // 执行批量查询
            var results = await dnsService.BatchQueryAsync(queries);
            
            // 显示结果
            for (int i = 0; i < results.Count; i++)
            {
                var query = queries[i];
                var result = results[i];
                
                Console.WriteLine($"\n{i + 1}. {query.Domain} - {query.RecordType}");
                if (result.Success)
                {
                    Console.WriteLine($"   成功 (耗时: {result.ExecutionTimeMs}ms)");
                    foreach (var item in result.Results)
                    {
                        Console.WriteLine($"     - {item}");
                    }
                }
                else
                {
                    Console.WriteLine($"   失败: {result.ErrorMessage}");
                }
            }
            
            await host.RunAsync();
        }
    }
}
```

### 4. 错误处理示例

```csharp
using System;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DnsAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("DNS 错误处理示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务容器
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.Configure<DnsOptions>(options => {
                        options.TimeoutMs = 2000; // 短超时，便于测试
                        options.DnsServers = new List<string> { "999.999.999.999" }; // 无效 DNS 服务器
                    });
                    services.AddDns();
                })
                .Build();
            
            var dnsService = host.Services.GetRequiredService<IDnsService>();
            
            Console.WriteLine("\n1. 查询无效域名（不存在的域名）");
            try
            {
                var result = await dnsService.QueryAAsync("this-domain-does-not-exist-12345.com");
                if (result.Success)
                {
                    Console.WriteLine($"成功: {string.Join(", ", result.Results)}");
                }
                else
                {
                    Console.WriteLine($"预期失败: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"捕获异常: {ex.GetType().Name}: {ex.Message}");
            }
            
            Console.WriteLine("\n2. 使用无效 DNS 服务器查询");
            try
            {
                var result = await dnsService.QueryAAsync("example.com");
                if (result.Success)
                {
                    Console.WriteLine($"成功: {string.Join(", ", result.Results)}");
                }
                else
                {
                    Console.WriteLine($"预期失败: {result.ErrorMessage}");
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"捕获 Socket 异常: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"捕获其他异常: {ex.GetType().Name}: {ex.Message}");
            }
            
            await host.RunAsync();
        }
    }
}
```

### 5. 命令行工具示例

```bash
# 查询 A 记录（IPv4）
dns_aot.exe a example.com

# 查询 AAAA 记录（IPv6）
dns_aot.exe aaaa example.com

# 查询 MX 记录（邮件）
dns_aot.exe mx gmail.com

# 查询 NS 记录（名称服务器）
dns_aot.exe ns github.com

# 查询 CNAME 记录（别名）
dns_aot.exe cname www.example.com

# 查询 TXT 记录（文本）
dns_aot.exe txt google.com

# 通用查询命令
dns_aot.exe query example.com A
dns_aot.exe query gmail.com MX

# 获取 DNS 服务状态
dns_aot.exe status

# 刷新 DNS 缓存
dns_aot.exe flush

# 重置 DNS 服务状态
dns_aot.exe reset

# 运行 DNS 演示
dns_aot.exe demo
```

### 6. 性能优化示例

```csharp
using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DnsAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("DNS 性能优化示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务容器，启用缓存以提高性能
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.Configure<DnsOptions>(options => {
                        options.EnableCache = true; // 启用缓存
                        options.CacheSize = 2000;    // 较大的缓存大小
                        options.CacheExpirationSeconds = 3600; // 较长的缓存过期时间
                        options.TimeoutMs = 2000;
                    });
                    services.AddDns();
                })
                .Build();
            
            var dnsService = host.Services.GetRequiredService<IDnsService>();
            var testDomains = new List<string> {
                "example.com", "github.com", "google.com", "microsoft.com", "amazon.com",
                "facebook.com", "twitter.com", "linkedin.com", "instagram.com", "youtube.com"
            };
            
            Console.WriteLine($"\n测试场景：查询 {testDomains.Count} 个域名，每个域名查询 5 次");
            Console.WriteLine($"配置：缓存启用，大小 {2000}，过期时间 {3600} 秒");
            
            var stopwatch = Stopwatch.StartNew();
            int totalQueries = 0;
            int successfulQueries = 0;
            
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"\n第 {i + 1} 轮查询：");
                var roundStopwatch = Stopwatch.StartNew();
                
                foreach (var domain in testDomains)
                {
                    totalQueries++;
                    var result = await dnsService.QueryAAsync(domain);
                    if (result.Success)
                    {
                        successfulQueries++;
                        Console.Write($"✓");
                    }
                    else
                    {
                        Console.Write($"✗");
                    }
                }
                
                roundStopwatch.Stop();
                Console.WriteLine($"  耗时：{roundStopwatch.ElapsedMilliseconds} ms");
            }
            
            stopwatch.Stop();
            
            Console.WriteLine("\n" + "=" * 50);
            Console.WriteLine($"性能测试结果：");
            Console.WriteLine($"总查询数：{totalQueries}");
            Console.WriteLine($"成功查询数：{successfulQueries}");
            Console.WriteLine($"总耗时：{stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"平均查询耗时：{stopwatch.ElapsedMilliseconds / (double)totalQueries:F2} ms");
            Console.WriteLine($"成功率：{successfulQueries / (double)totalQueries * 100:F1}%");
            
            // 查看缓存效果（第二次及以后查询应该更快）
            Console.WriteLine("\n性能优化效果：");
            Console.WriteLine("1. 启用缓存显著提高了重复查询的速度");
            Console.WriteLine("2. 较大的缓存大小可以存储更多查询结果");
            Console.WriteLine("3. 较长的缓存过期时间减少了网络请求");
            Console.WriteLine("4. AOT 编译提供了极致的启动速度和运行性能");
            
            await host.RunAsync();
        }
    }
}
```

## 总结

以上示例展示了 DNS 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本的 DNS 查询操作
2. 配置高级选项以满足特定需求
3. 使用批量查询提高效率
4. 处理各种错误情况
5. 使用命令行工具进行便捷操作
6. 优化性能以获得更好的查询体验

该系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。所有示例都支持 .NET 10 AOT 编译，提供极致的性能表现。
