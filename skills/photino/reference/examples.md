# Photino 技能使用示例

## 1. 基础示例

### 1.1 最小化应用

一个最基本的 Photino 应用，演示了如何创建和显示一个简单的窗口。

#### 代码示例

```csharp
#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Photino.NET@2.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:property TargetFramework=net10.0
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true
#:property TrimMode=partial
#:property Optimize=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino;

class Program
{
    static async Task Main(string[] args)
    {
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置日志
        services.AddLogging(builder => builder.AddConsole());
        
        // 注册 Photino 服务
        services.AddPhotinoServices(options =>
        {
            options.Title = "最小化 Photino 应用";
            options.Width = 800;
            options.Height = 600;
            options.StartUrl = "wwwroot/index.html";
        });
        
        // 构建服务提供程序
        using var serviceProvider = services.BuildServiceProvider();
        
        // 获取 Photino 服务
        var photinoService = serviceProvider.GetRequiredService<IPhotinoService>();
        
        // 创建并显示窗口
        var window = await photinoService.CreateWindowAsync();
        await window.ShowAsync();
        
        // 等待用户输入
        Console.WriteLine("按任意键退出...");
        Console.ReadKey();
        
        // 关闭窗口
        await window.CloseAsync();
    }
}
```

#### HTML 示例

```html
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>最小化 Photino 应用</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 20px;
            background-color: #f0f0f0;
        }
        h1 {
            color: #333;
        }
        p {
            color: #666;
        }
    </style>
</head>
<body>
    <h1>欢迎使用 Photino！</h1>
    <p>这是一个最小化的 Photino 应用示例。</p>
    <p>你可以使用 HTML、CSS 和 JavaScript 构建桌面应用界面。</p>
</body>
</html>
```

### 1.2 多窗口应用

演示如何创建和管理多个窗口。

#### 代码示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino;

class Program
{
    static async Task Main(string[] args)
    {
        // 构建服务容器
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddPhotinoServices(options =>
        {
            options.Title = "多窗口应用";
            options.Width = 800;
            options.Height = 600;
            options.StartUrl = "wwwroot/index.html";
            options.MaxWindows = 3; // 限制最大窗口数
        });
        
        using var serviceProvider = services.BuildServiceProvider();
        var photinoService = serviceProvider.GetRequiredService<IPhotinoService>();
        
        try
        {
            // 创建主窗口
            Console.WriteLine("创建主窗口...");
            var mainWindow = await photinoService.CreateWindowAsync();
            await mainWindow.SetTitleAsync("主窗口");
            await mainWindow.ShowAsync();
            
            // 创建第二个窗口
            Console.WriteLine("创建第二个窗口...");
            var secondWindow = await photinoService.CreateWindowAsync();
            await secondWindow.SetTitleAsync("第二个窗口");
            await secondWindow.NavigateToAsync("wwwroot/second.html");
            await secondWindow.ShowAsync();
            
            // 创建第三个窗口
            Console.WriteLine("创建第三个窗口...");
            var thirdWindow = await photinoService.CreateWindowAsync();
            await thirdWindow.SetTitleAsync("第三个窗口");
            await thirdWindow.NavigateToAsync("wwwroot/third.html");
            await thirdWindow.ShowAsync();
            
            // 尝试创建第四个窗口（应该失败）
            try
            {
                Console.WriteLine("尝试创建第四个窗口...");
                var fourthWindow = await photinoService.CreateWindowAsync();
                await fourthWindow.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建第四个窗口失败: {ex.Message}");
            }
            
            // 等待用户输入
            Console.WriteLine("按任意键关闭所有窗口...");
            Console.ReadKey();
            
            // 关闭所有窗口
            await photinoService.CloseAllWindowsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 2. 高级示例

### 2.1 消息通信示例

演示 .NET 和 Web 端之间的双向通信。

#### .NET 代码

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddPhotinoServices(options =>
        {
            options.Title = "消息通信示例";
            options.Width = 800;
            options.Height = 600;
            options.StartUrl = "wwwroot/index.html";
        });
        
        using var serviceProvider = services.BuildServiceProvider();
        var photinoService = serviceProvider.GetRequiredService<IPhotinoService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        var window = await photinoService.CreateWindowAsync();
        await window.ShowAsync();
        
        // 发送消息到 Web 端
        await Task.Delay(1000); // 等待页面加载
        await photinoService.SendMessageAsync("Hello from .NET!");
        
        // 接收来自 Web 端的消息
        Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    var message = await photinoService.ReceiveMessageAsync();
                    logger.LogInformation($"收到来自 Web 端的消息: {message}");
                    
                    // 回复消息
                    await photinoService.SendMessageAsync($"已收到消息: {message}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "接收消息失败");
                    break;
                }
            }
        });
        
        Console.WriteLine("按任意键退出...");
        Console.ReadKey();
        await window.CloseAsync();
    }
}
```

#### Web 端代码

```html
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>消息通信示例</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        #messages { border: 1px solid #ccc; padding: 10px; height: 200px; overflow-y: scroll; margin-bottom: 10px; }
        #input { width: 80%; padding: 5px; }
        #send { padding: 5px 10px; }
    </style>
</head>
<body>
    <h1>消息通信示例</h1>
    <div id="messages"></div>
    <input type="text" id="input" placeholder="输入消息...">
    <button id="send">发送</button>
    
    <script>
        // 模拟消息接收
        function receiveMessage(message) {
            const messagesDiv = document.getElementById('messages');
            messagesDiv.innerHTML += `<p><strong>收到:</strong> ${message}</p>`;
            messagesDiv.scrollTop = messagesDiv.scrollHeight;
        }
        
        // 发送消息
        document.getElementById('send').addEventListener('click', function() {
            const input = document.getElementById('input');
            const message = input.value;
            if (message) {
                const messagesDiv = document.getElementById('messages');
                messagesDiv.innerHTML += `<p><strong>发送:</strong> ${message}</p>`;
                messagesDiv.scrollTop = messagesDiv.scrollHeight;
                
                // 模拟发送到 .NET
                console.log('发送消息到 .NET:', message);
                input.value = '';
                
                // 模拟 .NET 回复
                setTimeout(() => {
                    receiveMessage(`已收到消息: ${message}`);
                }, 500);
            }
        });
        
        // 初始消息
        receiveMessage('连接已建立');
        
        // 模拟接收 .NET 消息
        setTimeout(() => {
            receiveMessage('Hello from .NET!');
        }, 2000);
    </script>
</body>
</html>
```

### 2.2 插件系统示例

演示如何创建和使用 Photino 插件。

#### 插件定义

```csharp
// 数学计算插件
public class MathPlugin : IPhotinoPlugin
{
    public string Name => "MathPlugin";
    public string Version => "1.0.0";

    public Task InitializeAsync(IPhotinoService photinoService)
    {
        Console.WriteLine("MathPlugin 初始化成功");
        return Task.CompletedTask;
    }

    public Task<object> ExecuteAsync(string command, params object[] parameters)
    {
        switch (command)
        {
            case "add":
                if (parameters.Length >= 2 && parameters[0] is double a && parameters[1] is double b)
                {
                    return Task.FromResult<object>(a + b);
                }
                break;
            case "subtract":
                if (parameters.Length >= 2 && parameters[0] is double a && parameters[1] is double b)
                {
                    return Task.FromResult<object>(a - b);
                }
                break;
            case "multiply":
                if (parameters.Length >= 2 && parameters[0] is double a && parameters[1] is double b)
                {
                    return Task.FromResult<object>(a * b);
                }
                break;
            case "divide":
                if (parameters.Length >= 2 && parameters[0] is double a && parameters[1] is double b && b != 0)
                {
                    return Task.FromResult<object>(a / b);
                }
                break;
        }
        return Task.FromResult<object>("命令执行失败");
    }
}

// 文件操作插件
public class FilePlugin : IPhotinoPlugin
{
    public string Name => "FilePlugin";
    public string Version => "1.0.0";

    public Task InitializeAsync(IPhotinoService photinoService)
    {
        Console.WriteLine("FilePlugin 初始化成功");
        return Task.CompletedTask;
    }

    public Task<object> ExecuteAsync(string command, params object[] parameters)
    {
        switch (command)
        {
            case "read":
                if (parameters.Length >= 1 && parameters[0] is string path)
                {
                    try
                    {
                        if (File.Exists(path))
                        {
                            return Task.FromResult<object>(File.ReadAllText(path));
                        }
                        return Task.FromResult<object>("文件不存在");
                    }
                    catch (Exception ex)
                    {
                        return Task.FromResult<object>($"读取失败: {ex.Message}");
                    }
                }
                break;
            case "write":
                if (parameters.Length >= 2 && parameters[0] is string path && parameters[1] is string content)
                {
                    try
                    {
                        File.WriteAllText(path, content);
                        return Task.FromResult<object>("写入成功");
                    }
                    catch (Exception ex)
                    {
                        return Task.FromResult<object>($"写入失败: {ex.Message}");
                    }
                }
                break;
        }
        return Task.FromResult<object>("命令执行失败");
    }
}
```

#### 使用插件

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddPhotinoServices(options =>
        {
            options.Title = "插件系统示例";
            options.Width = 800;
            options.Height = 600;
            options.StartUrl = "wwwroot/index.html";
        });
        
        using var serviceProvider = services.BuildServiceProvider();
        var photinoService = serviceProvider.GetRequiredService<IPhotinoService>();
        
        // 注册插件
        await photinoService.RegisterPluginAsync<MathPlugin>();
        await photinoService.RegisterPluginAsync<FilePlugin>();
        
        var window = await photinoService.CreateWindowAsync();
        await window.ShowAsync();
        
        // 测试数学插件
        Console.WriteLine("测试数学插件...");
        var mathPlugin = new MathPlugin();
        await mathPlugin.InitializeAsync(photinoService);
        
        var addResult = await mathPlugin.ExecuteAsync("add", 10, 5);
        Console.WriteLine($"10 + 5 = {addResult}");
        
        var multiplyResult = await mathPlugin.ExecuteAsync("multiply", 10, 5);
        Console.WriteLine($"10 * 5 = {multiplyResult}");
        
        // 测试文件插件
        Console.WriteLine("测试文件插件...");
        var filePlugin = new FilePlugin();
        await filePlugin.InitializeAsync(photinoService);
        
        var writeResult = await filePlugin.ExecuteAsync("write", "test.txt", "Hello, Photino!");
        Console.WriteLine($"写入文件: {writeResult}");
        
        var readResult = await filePlugin.ExecuteAsync("read", "test.txt");
        Console.WriteLine($"读取文件: {readResult}");
        
        Console.WriteLine("按任意键退出...");
        Console.ReadKey();
        await window.CloseAsync();
    }
}
```

### 2.3 本地存储示例

演示如何使用 Photino 的本地存储功能。

#### 代码示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddPhotinoServices(options =>
        {
            options.Title = "本地存储示例";
            options.Width = 800;
            options.Height = 600;
            options.StartUrl = "wwwroot/index.html";
            options.LocalStoragePath = "app_data.db";
        });
        
        using var serviceProvider = services.BuildServiceProvider();
        var photinoService = serviceProvider.GetRequiredService<IPhotinoService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        var window = await photinoService.CreateWindowAsync();
        await window.ShowAsync();
        
        // 获取本地存储
        var localStorage = await photinoService.GetLocalStorageAsync();
        
        // 存储数据
        Console.WriteLine("存储用户数据...");
        await localStorage.SetAsync("username", "张三");
        await localStorage.SetAsync("email", "zhangsan@example.com");
        await localStorage.SetAsync("age", 30);
        await localStorage.SetAsync("isAdmin", true);
        
        // 获取数据
        Console.WriteLine("获取用户数据...");
        var username = await localStorage.GetAsync<string>("username");
        var email = await localStorage.GetAsync<string>("email");
        var age = await localStorage.GetAsync<int>("age");
        var isAdmin = await localStorage.GetAsync<bool>("isAdmin");
        
        Console.WriteLine($"用户名: {username}");
        Console.WriteLine($"邮箱: {email}");
        Console.WriteLine($"年龄: {age}");
        Console.WriteLine($"是否管理员: {isAdmin}");
        
        // 删除数据
        Console.WriteLine("删除邮箱数据...");
        await localStorage.DeleteAsync("email");
        
        // 检查数据是否存在
        var emailAfterDelete = await localStorage.GetAsync<string>("email");
        Console.WriteLine($"删除后邮箱: {(emailAfterDelete == null ? "不存在" : emailAfterDelete)}");
        
        // 清空存储
        Console.WriteLine("清空所有数据...");
        await localStorage.ClearAsync();
        
        // 检查数据是否存在
        var usernameAfterClear = await localStorage.GetAsync<string>("username");
        Console.WriteLine($"清空后用户名: {(usernameAfterClear == null ? "不存在" : usernameAfterClear)}");
        
        Console.WriteLine("按任意键退出...");
        Console.ReadKey();
        await window.CloseAsync();
    }
}
```

## 3. 高级场景

### 3.1 主题切换示例

演示如何实现应用主题切换功能。

#### 代码示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddPhotinoServices(options =>
        {
            options.Title = "主题切换示例";
            options.Width = 800;
            options.Height = 600;
            options.StartUrl = "wwwroot/index.html";
            options.DefaultTheme = "light";
        });
        
        using var serviceProvider = services.BuildServiceProvider();
        var photinoService = serviceProvider.GetRequiredService<IPhotinoService>();
        
        var window = await photinoService.CreateWindowAsync();
        await window.ShowAsync();
        
        // 模拟主题切换
        Console.WriteLine("当前主题: light");
        Console.WriteLine("按 'd' 切换到深色主题，按 'l' 切换到浅色主题，按任意其他键退出...");
        
        while (true)
        {
            var key = Console.ReadKey(true);
            
            if (key.Key == ConsoleKey.D)
            {
                Console.WriteLine("切换到深色主题...");
                await photinoService.SetThemeAsync("dark");
            }
            else if (key.Key == ConsoleKey.L)
            {
                Console.WriteLine("切换到浅色主题...");
                await photinoService.SetThemeAsync("light");
            }
            else
            {
                break;
            }
        }
        
        await window.CloseAsync();
    }
}
```

#### Web 端代码

```html
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>主题切换示例</title>
    <style>
        /* 浅色主题 */
        body.light {
            background-color: #ffffff;
            color: #333333;
        }
        
        /* 深色主题 */
        body.dark {
            background-color: #333333;
            color: #ffffff;
        }
        
        body {
            font-family: Arial, sans-serif;
            margin: 20px;
            transition: background-color 0.3s, color 0.3s;
        }
        
        .theme-toggle {
            padding: 10px 20px;
            background-color: #007bff;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
        }
        
        .theme-toggle:hover {
            background-color: #0069d9;
        }
    </style>
</head>
<body class="light">
    <h1>主题切换示例</h1>
    <p>当前主题: <span id="current-theme">浅色</span></p>
    <button class="theme-toggle" onclick="toggleTheme()">切换主题</button>
    
    <script>
        let currentTheme = 'light';
        
        function toggleTheme() {
            currentTheme = currentTheme === 'light' ? 'dark' : 'light';
            document.body.className = currentTheme;
            document.getElementById('current-theme').textContent = currentTheme === 'light' ? '浅色' : '深色';
            
            // 通知 .NET 主题已更改
            console.log(`主题已切换到: ${currentTheme}`);
        }
        
        // 监听来自 .NET 的主题更改
        function setTheme(theme) {
            currentTheme = theme;
            document.body.className = theme;
            document.getElementById('current-theme').textContent = theme === 'light' ? '浅色' : '深色';
            console.log(`主题已设置为: ${theme}`);
        }
    </script>
</body>
</html>
```

### 3.2 自动更新示例

演示如何实现应用自动更新功能。

#### 代码示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddPhotinoServices(options =>
        {
            options.Title = "自动更新示例";
            options.Width = 800;
            options.Height = 600;
            options.StartUrl = "wwwroot/index.html";
            options.UpdateUrl = "https://api.example.com/updates";
        });
        
        using var serviceProvider = services.BuildServiceProvider();
        var photinoService = serviceProvider.GetRequiredService<IPhotinoService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        var window = await photinoService.CreateWindowAsync();
        await window.ShowAsync();
        
        // 检查更新
        Console.WriteLine("检查更新...");
        var updateResult = await photinoService.CheckForUpdatesAsync();
        
        if (updateResult.HasUpdate)
        {
            Console.WriteLine($"发现新版本: {updateResult.Version}");
            Console.WriteLine($"更新描述: {updateResult.Description}");
            
            // 询问用户是否更新
            Console.WriteLine("是否应用更新? (y/n)");
            var key = Console.ReadKey(true);
            
            if (key.Key == ConsoleKey.Y)
            {
                Console.WriteLine("应用更新...");
                await photinoService.ApplyUpdateAsync();
                Console.WriteLine("更新成功，请重启应用");
            }
            else
            {
                Console.WriteLine("取消更新");
            }
        }
        else
        {
            Console.WriteLine("当前版本已是最新");
        }
        
        Console.WriteLine("按任意键退出...");
        Console.ReadKey();
        await window.CloseAsync();
    }
}
```

### 3.3 性能监控示例

演示如何使用 Photino 的性能监控功能。

#### 代码示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino;
using System.Diagnostics;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddPhotinoServices(options =>
        {
            options.Title = "性能监控示例";
            options.Width = 800;
            options.Height = 600;
            options.StartUrl = "wwwroot/index.html";
            options.EnablePerformanceMetrics = true;
        });
        
        using var serviceProvider = services.BuildServiceProvider();
        var photinoService = serviceProvider.GetRequiredService<IPhotinoService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        var window = await photinoService.CreateWindowAsync();
        await window.ShowAsync();
        
        // 性能测试
        Console.WriteLine("开始性能测试...");
        
        // 测试窗口创建性能
        var stopwatch = Stopwatch.StartNew();
        var testWindow = await photinoService.CreateWindowAsync();
        stopwatch.Stop();
        Console.WriteLine($"窗口创建时间: {stopwatch.ElapsedMilliseconds}ms");
        await testWindow.CloseAsync();
        
        // 测试消息发送性能
        stopwatch.Restart();
        for (int i = 0; i < 100; i++)
        {
            await photinoService.SendMessageAsync($"Test message {i}");
        }
        stopwatch.Stop();
        Console.WriteLine($"发送 100 条消息时间: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"平均每条消息时间: {stopwatch.ElapsedMilliseconds / 100.0}ms");
        
        // 测试 JavaScript 执行性能
        stopwatch.Restart();
        var jsResult = await window.ExecuteJavaScriptAsync("1 + 2 + 3 + 4 + 5");
        stopwatch.Stop();
        Console.WriteLine($"JavaScript 执行时间: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"JavaScript 结果: {jsResult}");
        
        // 测试并行处理性能
        stopwatch.Restart();
        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                await Task.Delay(100); // 模拟工作
            }));
        }
        await Task.WhenAll(tasks);
        stopwatch.Stop();
        Console.WriteLine($"并行处理 10 个任务时间: {stopwatch.ElapsedMilliseconds}ms");
        
        Console.WriteLine("性能测试完成");
        Console.WriteLine("按任意键退出...");
        Console.ReadKey();
        await window.CloseAsync();
    }
}
```

## 4. 部署示例

### 4.1 AOT 编译部署

演示如何使用 AOT 编译部署 Photino 应用。

#### 发布命令

```bash
# Windows 发布
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true -p:ReadyToRun=true -p:TieredCompilation=true -p:TrimMode=partial -p:Optimize=true

# Linux 发布
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true -p:ReadyToRun=true -p:TieredCompilation=true -p:TrimMode=partial -p:Optimize=true

# macOS 发布
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true -p:ReadyToRun=true -p:TieredCompilation=true -p:TrimMode=partial -p:Optimize=true
```

#### 发布配置文件

```xml
<!-- PhotinoApp.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <PublishAot>true</PublishAot>
    <ReadyToRun>true</ReadyToRun>
    <TieredCompilation>true</TieredCompilation>
    <TrimMode>partial</TrimMode>
    <Optimize>true</Optimize>
    <PublishSingleFile>true</PublishSingleFile>
    <SelfContained>true</SelfContained>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Photino.NET" Version="2.0.0" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging.Console" Version="10.0.0" />
  </ItemGroup>
  
  <ItemGroup>
    <None Update="wwwroot\**\*">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>
  
</Project>
```

### 4.2 Docker 部署

演示如何使用 Docker 部署 Photino 应用。

#### Dockerfile

```dockerfile
# 使用 .NET 10 SDK 作为构建镜像
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# 复制项目文件
COPY *.csproj .
RUN dotnet restore

# 复制源代码
COPY . .

# 发布应用
RUN dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true -p:TrimMode=partial -o /app/publish

# 使用 Alpine 作为运行镜像
FROM alpine:3.18 AS runtime
WORKDIR /app

# 安装依赖
RUN apk add --no-cache \
    libgcc \
    libstdc++ \
    icu-libs \
    webkit2gtk \
    gtk+3.0 \
    libsoup \
    libjavascriptcoregtk \
    libxml2 \
    glib \
    cairo \
    pango \
    atk

# 复制发布文件
COPY --from=build /app/publish .

# 设置执行权限
RUN chmod +x ./PhotinoApp

# 运行应用
ENTRYPOINT ["./PhotinoApp"]
```

#### Docker Compose

```yaml
version: '3.8'

services:
  photino-app:
    build: .
    container_name: photino-app
    restart: unless-stopped
    ports:
      - "8080:8080"
    volumes:
      - ./data:/app/data
      - ./logs:/app/logs
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - DOTNET_ENVIRONMENT=Production
      - PHOTINO_DEBUG=false
      - PHOTINO_LOG_LEVEL=Warning
```

## 5. 实用工具示例

### 5.1 日志工具

演示如何使用 Photino 的日志功能。

#### 代码示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        
        // 配置详细日志
        services.AddLogging(builder =>
        {
            builder.AddConsole(options =>
            {
                options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
            })
            .SetMinimumLevel(LogLevel.Debug);
        });
        
        services.AddPhotinoServices(options =>
        {
            options.Title = "日志工具示例";
            options.Width = 800;
            options.Height = 600;
            options.StartUrl = "wwwroot/index.html";
        });
        
        using var serviceProvider = services.BuildServiceProvider();
        var photinoService = serviceProvider.GetRequiredService<IPhotinoService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("应用启动");
        
        try
        {
            var window = await photinoService.CreateWindowAsync();
            logger.LogDebug("窗口创建成功");
            
            await window.ShowAsync();
            logger.LogInformation("窗口显示成功");
            
            await window.ExecuteJavaScriptAsync("console.log('Hello from JavaScript')");
            logger.LogDebug("JavaScript 执行成功");
            
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
            
            await window.CloseAsync();
            logger.LogInformation("窗口关闭成功");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "应用错误");
        }
        
        logger.LogInformation("应用退出");
    }
}
```

### 5.2 快捷键工具

演示如何使用 Photino 的快捷键功能。

#### 代码示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddPhotinoServices(options =>
        {
            options.Title = "快捷键工具示例";
            options.Width = 800;
            options.Height = 600;
            options.StartUrl = "wwwroot/index.html";
        });
        
        using var serviceProvider = services.BuildServiceProvider();
        var photinoService = serviceProvider.GetRequiredService<IPhotinoService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        var window = await photinoService.CreateWindowAsync();
        await window.ShowAsync();
        
        // 注册快捷键
        await photinoService.RegisterShortcutAsync("Ctrl+S", () =>
        {
            logger.LogInformation("触发快捷键 Ctrl+S - 保存操作");
        });
        
        await photinoService.RegisterShortcutAsync("Ctrl+O", () =>
        {
            logger.LogInformation("触发快捷键 Ctrl+O - 打开操作");
        });
        
        await photinoService.RegisterShortcutAsync("Ctrl+N", () =>
        {
            logger.LogInformation("触发快捷键 Ctrl+N - 新建操作");
        });
        
        await photinoService.RegisterShortcutAsync("Ctrl+Q", () =>
        {
            logger.LogInformation("触发快捷键 Ctrl+Q - 退出操作");
            // 这里可以添加退出逻辑
        });
        
        Console.WriteLine("已注册快捷键:");
        Console.WriteLine("Ctrl+S - 保存");
        Console.WriteLine("Ctrl+O - 打开");
        Console.WriteLine("Ctrl+N - 新建");
        Console.WriteLine("Ctrl+Q - 退出");
        Console.WriteLine("按任意键退出...");
        Console.ReadKey();
        
        await window.CloseAsync();
    }
}
```

## 6. 总结

本示例文档提供了 Photino 技能的各种使用场景和代码示例，包括：

- **基础示例**：最小化应用、多窗口应用
- **高级示例**：消息通信、插件系统、本地存储
- **高级场景**：主题切换、自动更新、性能监控
- **部署示例**：AOT 编译部署、Docker 部署
- **实用工具**：日志工具、快捷键工具

这些示例展示了 Photino 技能的核心功能和使用方法，开发者可以根据自己的需求参考这些示例进行开发。

### 最佳实践

1. **使用依赖注入**：通过 Microsoft.Extensions.DependencyInjection 进行依赖注入，提高代码可维护性
2. **采用异步编程**：使用 async/await 模式处理异步操作，提高应用响应速度
3. **启用 AOT 编译**：通过 AOT 编译提高应用启动速度和运行性能
4. **合理使用插件**：将功能模块化，通过插件系统集成，提高代码可扩展性
5. **优化性能**：使用零拷贝、线程本地缓存等技术优化应用性能
6. **加强错误处理**：完善的错误处理和日志记录，提高应用稳定性
7. **跨平台测试**：在不同平台上测试应用，确保跨平台兼容性

通过这些示例和最佳实践，开发者可以快速上手 Photino 技能，构建高性能、跨平台的桌面应用程序。