# Nacos 智能体技能 - 使用示例

## 快速开始

### 1. 基本用法示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using nacos;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var nacosService = serviceProvider.GetRequiredService<INacosService>();
        var configService = serviceProvider.GetRequiredService<INacosConfigService>();
        var namingService = serviceProvider.GetRequiredService<INacosNamingService>();
        
        Console.WriteLine("Nacos 基本用法示例");
        Console.WriteLine("=" * 50);
        
        try
        {
            // 从配置中心获取配置
            Console.WriteLine("1. 从配置中心获取配置...");
            var config = await configService.GetConfigAsync("example-config", "DEFAULT_GROUP");
            Console.WriteLine($"配置内容: {config}");
            
            // 注册服务
            Console.WriteLine("\n2. 注册服务...");
            await namingService.RegisterInstanceAsync("example-service", "127.0.0.1", 8080);
            Console.WriteLine("服务注册成功");
            
            // 发现服务
            Console.WriteLine("\n3. 发现服务...");
            var instances = await namingService.SelectInstancesAsync("example-service", true);
            Console.WriteLine($"发现服务实例数: {instances.Count}");
            foreach (var instance in instances)
            {
                Console.WriteLine($"服务实例: {instance.Ip}:{instance.Port}
