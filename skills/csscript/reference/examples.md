# Csscript AOT - 使用示例

## 快速开始

### 1. 基本用法示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Csscript.AOT;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务容器
        var builder = Host.CreateApplicationBuilder();
        
        // 配置Csscript选项
        builder.Services.Configure<CsscriptOptions>(options => {
            options.EnableCache = true;
            options.CacheSize = 1000;
            options.AllowExternalScripts = false;
        });
        
        // 注册服务
        builder.Services.AddSingleton<ICsscriptService, CsscriptService>();
        builder.Services.AddSingleton<CsscriptAotEngine>();
        
        var host = builder.Build();
        var serviceProvider = host.Services;
        
        Console.WriteLine("Csscript AOT 基本用法示例");
        Console.WriteLine("=" * 60);
        
        // 使用Csscript AOT引擎
        var engine = serviceProvider.GetRequiredService<CsscriptAotEngine>();
        
        // 创建脚本代码
        string scriptCode = @"Console.WriteLine(""Hello from CSScript!"); 
return new { 
    Result = ""Success"", 
    Message = ""脚本执行成功"", 
    Timestamp = DateTime.Now 
};";
        
        // 执行脚本
        var result = await engine.ExecuteScriptAsync(scriptCode);
        Console.WriteLine($"脚本执行结果: 成功={result.Success}, 时间={result.ExecutionTimeMs}ms");
        
        if (result.Success && result.ResultData != null)
        {
            Console.WriteLine($"结果数据: {Newtonsoft.Json.JsonConvert.SerializeObject(result.ResultData)}");
        }
        
        // 获取服务状态
        var status = await engine.GetStatusAsync();
        Console.WriteLine($"\n服务状态: 运行中={status.IsRunning}, 已执行脚本={status.ExecutedScripts}");
    }
}
```

### 2. 脚本文件执行示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Csscript.AOT;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务容器
        var builder = Host.CreateApplicationBuilder();
        
        // 配置Csscript选项，允许执行外部脚本
        builder.Services.Configure<CsscriptOptions>(options => {
            options.EnableCache = true;
            options.AllowExternalScripts = true;
            options.ScriptExecutionTimeout = TimeSpan.FromSeconds(30);
        });
        
        // 注册服务
        builder.Services.AddSingleton<ICsscriptService, CsscriptService>();
        builder.Services.AddSingleton<CsscriptAotEngine>();
        
        var host = builder.Build();
        var engine = host.Services.GetRequiredService<CsscriptAotEngine>();
        
        Console.WriteLine("Csscript AOT 脚本文件执行示例");
        Console.WriteLine("=" * 60);
        
        // 准备测试脚本文件
        string scriptFilePath = "test_script.cs";
        string scriptContent = @"using System;

public class TestScript
{
    public static object Run()
    {
        return new {
            Success = true,
            Message = "外部脚本执行成功",
            Date = DateTime.Now,
            Environment = Environment.OSVersion.ToString()
        };
    }
}";
        
        // 写入测试脚本文件
        await File.WriteAllTextAsync(scriptFilePath, scriptContent);
        Console.WriteLine($"已创建测试脚本文件: {scriptFilePath}");
        
        try
        {
            // 执行脚本文件
            var result = await engine.ExecuteScriptFileAsync(scriptFilePath);
            Console.WriteLine($"\n脚本文件执行结果: 成功={result.Success}, 时间={result.ExecutionTimeMs}ms");
            
            if (result.Success && result.ResultData != null)
            {
                Console.WriteLine($"结果数据: {Newtonsoft.Json.JsonConvert.SerializeObject(result.ResultData)}");
            }
        }
        finally
        {
            // 清理测试文件
            if (File.Exists(scriptFilePath))
            {
                File.Delete(scriptFilePath);
                Console.WriteLine($"\n已删除测试脚本文件: {scriptFilePath}");
            }
        }
    }
}
```

### 3. 批量脚本执行示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Csscript.AOT;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务容器
        var builder = Host.CreateApplicationBuilder();
        
        // 配置Csscript选项
        builder.Services.Configure<CsscriptOptions>(options => {
            options.EnableCache = true;
            options.CacheSize = 2000;
            options.WorkerCount = Environment.ProcessorCount;
        });
        
        // 注册服务
        builder.Services.AddSingleton<ICsscriptService, CsscriptService>();
        builder.Services.AddSingleton<CsscriptAotEngine>();
        
        var host = builder.Build();
        var engine = host.Services.GetRequiredService<CsscriptAotEngine>();
        
        Console.WriteLine("Csscript AOT 批量脚本执行示例");
        Console.WriteLine("=" * 60);
        
        // 准备批量执行请求
        var requests = new List<ScriptExecutionRequest>();
        
        for (int i = 1; i <= 5; i++)
        {
            requests.Add(new ScriptExecutionRequest
            {
                RequestId = $"Request-{i}
