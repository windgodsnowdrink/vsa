# LiquidState 使用示例文档

## 1. 基本用法

### 1.1 创建状态机

**功能说明**：创建一个新的状态机实例

**命令行用法**：

```bash
# 使用完整命令
liquidstate_aot.exe create "system"

# 使用别名
liquidstate_aot.exe c "system"
```

**输出示例**：

```
状态机 'system' 已创建并注册
```

### 1.2 启动状态机

**功能说明**：启动状态机，设置初始状态

**命令行用法**：

```bash
# 使用完整命令
liquidstate_aot.exe start "system" "Idle"

# 使用别名
liquidstate_aot.exe s "system" "Idle"
```

**输出示例**：

```
状态机 'system' 已启动，初始状态: Idle
```

### 1.3 触发事件

**功能说明**：向状态机触发事件，执行状态转换

**命令行用法**：

```bash
# 使用完整命令
liquidstate_aot.exe fire "system" "Start"

# 使用别名
liquidstate_aot.exe f "system" "Start"
```

**输出示例**：

```
启动系统...
系统已启动
事件 'Start' 已成功触发
当前状态: Running
```

### 1.4 停止状态机

**功能说明**：停止状态机运行

**命令行用法**：

```bash
# 使用完整命令
liquidstate_aot.exe stop "system"

# 使用别名
liquidstate_aot.exe st "system"
```

**输出示例**：

```
状态机 'system' 已停止
```

### 1.5 列出状态机

**功能说明**：列出所有已注册的状态机

**命令行用法**：

```bash
# 使用完整命令
liquidstate_aot.exe list

# 使用别名
liquidstate_aot.exe l
```

**输出示例**：

```
已注册的状态机:
- system
- user
- order
```

### 1.6 移除状态机

**功能说明**：移除指定的状态机实例

**命令行用法**：

```bash
# 使用完整命令
liquidstate_aot.exe remove "system"

# 使用别名
liquidstate_aot.exe r "system"
```

**输出示例**：

```
状态机 'system' 已移除
```

### 1.7 显示帮助信息

**功能说明**：显示命令行帮助信息

**命令行用法**：

```bash
# 使用完整命令
liquidstate_aot.exe help

# 使用别名
liquidstate_aot.exe h
```

**输出示例**：

```
LiquidState AOT - 基于AOT编译的状态机工具

Usage: liquidstate_aot [command]

Commands:
  create (c)          创建状态机
  start (s)           启动状态机
  fire (f)            触发状态机事件
  stop (st)           停止状态机
  list (l)            列出所有状态机
  remove (r)          移除状态机
  help (h)            显示帮助信息

Options:
  --version           Show version information
  -?, -h, --help      Show help and usage information
```

## 2. 高级用法

### 2.1 编程方式使用

**功能说明**：在代码中使用LiquidState状态机

**示例代码**：

```csharp
using LiquidStateAot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// 构建服务容器
var services = new ServiceCollection();
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});
services.AddMemoryCache();
services.AddSingleton<StateMachineService>();

var serviceProvider = services.BuildServiceProvider();
var stateMachineService = serviceProvider.GetRequiredService<StateMachineService>();

// 创建状态机构建器
var builder = stateMachineService.CreateBuilder<string, string>();

// 配置状态转换
builder.AddTransition("Idle", "Start", "Running", async () =>
{
    Console.WriteLine("启动系统...");
    await Task.Delay(1000);
    Console.WriteLine("系统已启动");
});

builder.AddTransition("Running", "Stop", "Idle", async () =>
{
    Console.WriteLine("停止系统...");
    await Task.Delay(1000);
    Console.WriteLine("系统已停止");
});

builder.AddTransition("Running", "Pause", "Paused", async () =>
{
    Console.WriteLine("暂停系统...");
    await Task.Delay(500);
    Console.WriteLine("系统已暂停");
});

builder.AddTransition("Paused", "Resume", "Running", async () =>
{
    Console.WriteLine("恢复系统...");
    await Task.Delay(500);
    Console.WriteLine("系统已恢复");
});

// 添加状态进入和退出动作
builder.AddEntryAction("Running", async () =>
{
    Console.WriteLine("进入运行状态");
});

builder.AddExitAction("Running", async () =>
{
    Console.WriteLine("退出运行状态");
});

// 构建并注册状态机
var stateMachine = builder.Build(serviceProvider);
stateMachineService.RegisterStateMachine("system", stateMachine);

// 启动状态机
stateMachine.Start("Idle");

// 触发事件
await stateMachine.FireAsync("Start");
Console.WriteLine($"当前状态: {stateMachine.CurrentState}");

await stateMachine.FireAsync("Pause");
Console.WriteLine($"当前状态: {stateMachine.CurrentState}");

await stateMachine.FireAsync("Resume");
Console.WriteLine($"当前状态: {stateMachine.CurrentState}");

await stateMachine.FireAsync("Stop");
Console.WriteLine($"当前状态: {stateMachine.CurrentState}");

// 停止状态机
stateMachine.Stop();
```

**输出示例**：

```
状态机已启动，初始状态: Idle
启动系统...
系统已启动
进入运行状态
当前状态: Running
退出运行状态
暂停系统...
系统已暂停
当前状态: Paused
恢复系统...
系统已恢复
进入运行状态
当前状态: Running
退出运行状态
停止系统...
系统已停止
当前状态: Idle
状态机已停止，最终状态: Idle
```

### 2.2 使用条件转换

**功能说明**：使用条件判断控制状态转换

**示例代码**：

```csharp
// 创建带条件的状态机
var builder = stateMachineService.CreateBuilder<string, string>();

// 定义条件变量
bool canTransition = true;

// 添加带条件的转换
builder.AddTransition("Idle", "Start", "Running", async () =>
{
    Console.WriteLine("启动系统...");
    await Task.Delay(1000);
    Console.WriteLine("系统已启动");
}, () => canTransition); // 条件判断

// 构建并启动状态机
var stateMachine = builder.Build(serviceProvider);
stateMachine.Start("Idle");

// 触发事件（条件满足）
Console.WriteLine("条件满足时触发事件:");
await stateMachine.FireAsync("Start");
Console.WriteLine($"当前状态: {stateMachine.CurrentState}");

// 修改条件
canTransition = false;

// 重置状态机
stateMachine.Reset("Idle");

// 触发事件（条件不满足）
Console.WriteLine("条件不满足时触发事件:");
await stateMachine.FireAsync("Start");
Console.WriteLine($"当前状态: {stateMachine.CurrentState}");
```

**输出示例**：

```
状态机已启动，初始状态: Idle
条件满足时触发事件:
启动系统...
系统已启动
当前状态: Running
状态机已重置，新的初始状态: Idle
条件不满足时触发事件:
无匹配的状态转换: 当前状态=Idle, 事件=Start
当前状态: Idle
```

### 2.3 错误处理

**功能说明**：处理状态机中的错误

**示例代码**：

```csharp
// 创建带错误处理的状态机
var builder = stateMachineService.CreateBuilder<string, string>();

// 添加可能出错的转换
builder.AddTransition("Idle", "Start", "Running", async () =>
{
    Console.WriteLine("启动系统...");
    await Task.Delay(500);
    // 模拟错误
    throw new Exception("启动失败: 系统资源不足");
});

// 设置错误处理程序
builder.WithErrorHandler(ex =>
{
    Console.WriteLine($"错误处理: {ex.Message}");
});

// 构建并启动状态机
var stateMachine = builder.Build(serviceProvider);
stateMachine.Start("Idle");

// 触发事件
await stateMachine.FireAsync("Start");
Console.WriteLine($"当前状态: {stateMachine.CurrentState}");
```

**输出示例**：

```
状态机已启动，初始状态: Idle
启动系统...
错误处理: 启动失败: 系统资源不足
当前状态: Idle
```

### 2.4 管理多个状态机

**功能说明**：创建和管理多个状态机实例

**示例代码**：

```csharp
// 创建第一个状态机（系统状态机）
var systemBuilder = stateMachineService.CreateBuilder<string, string>();
systemBuilder.AddTransition("Idle", "Start", "Running");
systemBuilder.AddTransition("Running", "Stop", "Idle");
var systemStateMachine = systemBuilder.Build(serviceProvider);
stateMachineService.RegisterStateMachine("system", systemStateMachine);

// 创建第二个状态机（用户状态机）
var userBuilder = stateMachineService.CreateBuilder<string, string>();
userBuilder.AddTransition("Offline", "Login", "Online");
userBuilder.AddTransition("Online", "Logout", "Offline");
var userStateMachine = userBuilder.Build(serviceProvider);
stateMachineService.RegisterStateMachine("user", userStateMachine);

// 列出所有状态机
Console.WriteLine("已注册的状态机:");
foreach (var name in stateMachineService.GetAllStateMachineNames())
{
    Console.WriteLine($"- {name}");
}

// 启动系统状态机
var systemSM = stateMachineService.GetStateMachine<string, string>("system");
systemSM.Start("Idle");
await systemSM.FireAsync("Start");
Console.WriteLine($"系统状态机当前状态: {systemSM.CurrentState}");

// 启动用户状态机
var userSM = stateMachineService.GetStateMachine<string, string>("user");
userSM.Start("Offline");
await userSM.FireAsync("Login");
Console.WriteLine($"用户状态机当前状态: {userSM.CurrentState}");

// 移除系统状态机
stateMachineService.RemoveStateMachine("system");

// 列出剩余的状态机
Console.WriteLine("移除后剩余的状态机:");
foreach (var name in stateMachineService.GetAllStateMachineNames())
{
    Console.WriteLine($"- {name}");
}
```

**输出示例**：

```
状态机已注册: system
状态机已注册: user
已注册的状态机:
- system
- user
状态机已启动，初始状态: Idle
执行状态转换: Idle -> Running (事件: Start
系统状态机当前状态: Running
状态机已启动，初始状态: Offline
执行状态转换: Offline -> Online (事件: Login
用户状态机当前状态: Online
状态机已移除: system
移除后剩余的状态机:
- user
```

### 2.5 自定义状态和事件类型

**功能说明**：使用自定义类型作为状态和事件

**示例代码**：

```csharp
// 定义自定义状态和事件枚举
enum OrderState { Created, Processing, Shipped, Delivered, Cancelled }
enum OrderEvent { Process, Ship, Deliver, Cancel }

// 创建使用自定义类型的状态机
var builder = stateMachineService.CreateBuilder<OrderState, OrderEvent>();

// 添加状态转换
builder.AddTransition(OrderState.Created, OrderEvent.Process, OrderState.Processing, async () =>
{
    Console.WriteLine("订单开始处理...");
    await Task.Delay(500);
    Console.WriteLine("订单处理中");
});

builder.AddTransition(OrderState.Processing, OrderEvent.Ship, OrderState.Shipped, async () =>
{
    Console.WriteLine("订单开始发货...");
    await Task.Delay(500);
    Console.WriteLine("订单已发货");
});

builder.AddTransition(OrderState.Shipped, OrderEvent.Deliver, OrderState.Delivered, async () =>
{
    Console.WriteLine("订单开始配送...");
    await Task.Delay(500);
    Console.WriteLine("订单已送达");
});

builder.AddTransition(OrderState.Created, OrderEvent.Cancel, OrderState.Cancelled, async () =>
{
    Console.WriteLine("订单已取消");
});

builder.AddTransition(OrderState.Processing, OrderEvent.Cancel, OrderState.Cancelled, async () =>
{
    Console.WriteLine("订单已取消");
});

// 构建并启动状态机
var orderStateMachine = builder.Build(serviceProvider);
orderStateMachine.Start(OrderState.Created);

// 触发事件
await orderStateMachine.FireAsync(OrderEvent.Process);
Console.WriteLine($"当前状态: {orderStateMachine.CurrentState}");

await orderStateMachine.FireAsync(OrderEvent.Ship);
Console.WriteLine($"当前状态: {orderStateMachine.CurrentState}");

await orderStateMachine.FireAsync(OrderEvent.Deliver);
Console.WriteLine($"当前状态: {orderStateMachine.CurrentState}");
```

**输出示例**：

```
状态机已启动，初始状态: Created
订单开始处理...
订单处理中
当前状态: Processing
订单开始发货...
订单已发货
当前状态: Shipped
订单开始配送...
订单已送达
当前状态: Delivered
```

## 3. 命令行使用技巧

### 3.1 使用别名

**功能说明**：使用命令别名提高输入效率

**示例**：

```bash
# 使用完整命令
liquidstate_aot.exe create "system"
liquidstate_aot.exe start "system" "Idle"
liquidstate_aot.exe fire "system" "Start"
liquidstate_aot.exe list
liquidstate_aot.exe stop "system"
liquidstate_aot.exe remove "system"

# 使用别名（更简洁）
liquidstate_aot.exe c "system"
liquidstate_aot.exe s "system" "Idle"
liquidstate_aot.exe f "system" "Start"
liquidstate_aot.exe l
liquidstate_aot.exe st "system"
liquidstate_aot.exe r "system"
```

### 3.2 重定向输出到文件

**功能说明**：将命令输出重定向到文件，便于保存和分析

**示例**：

```bash
# 重定向标准输出到文件
liquidstate_aot.exe list > state_machines.txt

# 重定向标准输出和错误输出到文件
liquidstate_aot.exe fire "system" "Start" > output.txt 2>&1

# 追加输出到文件
liquidstate_aot.exe fire "system" "Stop" >> output.txt
```

### 3.3 使用管道传递参数

**功能说明**：使用管道从其他命令接收输入

**示例**：

```powershell
# 使用 Get-Content 读取状态机名称并操作
Get-Content state_machines.txt | ForEach-Object {
    liquidstate_aot.exe start $_ "Idle"
}

# 使用变量存储状态机名称
$machineName = "system"
liquidstate_aot.exe create $machineName
liquidstate_aot.exe start $machineName "Idle"
```

## 4. 编程集成

### 4.1 基本集成

**功能说明**：在 C# 代码中集成 LiquidState

**示例代码**：

```csharp
using LiquidStateAot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// 构建服务容器
var services = new ServiceCollection();
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});
services.AddMemoryCache();
services.AddSingleton<StateMachineService>();

var serviceProvider = services.BuildServiceProvider();
var stateMachineService = serviceProvider.GetRequiredService<StateMachineService>();

// 创建和使用状态机
var builder = stateMachineService.CreateBuilder<string, string>();

builder.AddTransition("Idle", "Start", "Running");
builder.AddTransition("Running", "Stop", "Idle");

var stateMachine = builder.Build(serviceProvider);
stateMachineService.RegisterStateMachine("system", stateMachine);

stateMachine.Start("Idle");
await stateMachine.FireAsync("Start");
Console.WriteLine($"当前状态: {stateMachine.CurrentState}");

stateMachine.Stop();
```

### 4.2 集成到 ASP.NET Core 应用

**功能说明**：在 ASP.NET Core 应用中使用 LiquidState

**示例代码**：

```csharp
using LiquidStateAot;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 添加状态机服务
builder.Services.AddLogging();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<StateMachineService>();

var app = builder.Build();

// 获取状态机服务
var stateMachineService = app.Services.GetRequiredService<StateMachineService>();

// 创建默认状态机
var defaultBuilder = stateMachineService.CreateBuilder<string, string>();
defaultBuilder.AddTransition("Idle", "Start", "Running");
defaultBuilder.AddTransition("Running", "Stop", "Idle");
var defaultStateMachine = defaultBuilder.Build(app.Services);
stateMachineService.RegisterStateMachine("default", defaultStateMachine);

// 状态机操作端点
app.MapPost("/state-machine/create", async (HttpContext context, StateMachineService smService) =>
{
    var body = await context.Request.ReadFromJsonAsync<CreateRequest>();
    if (body == null || string.IsNullOrEmpty(body.Name))
    {
        return Results.BadRequest("请提供状态机名称");
    }

    try
    {
        // 创建状态机
        var builder = smService.CreateBuilder<string, string>();
        builder.AddTransition("Idle", "Start", "Running");
        builder.AddTransition("Running", "Stop", "Idle");
        var stateMachine = builder.Build(app.Services);
        smService.RegisterStateMachine(body.Name, stateMachine);

        return Results.Ok(new { message = $"状态机 '{body.Name}' 已创建" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/state-machine/start", async (HttpContext context, StateMachineService smService) =>
{
    var body = await context.Request.ReadFromJsonAsync<StartRequest>();
    if (body == null || string.IsNullOrEmpty(body.Name) || string.IsNullOrEmpty(body.InitialState))
    {
        return Results.BadRequest("请提供状态机名称和初始状态");
    }

    try
    {
        var stateMachine = smService.GetStateMachine<string, string>(body.Name);
        stateMachine.Start(body.InitialState);
        return Results.Ok(new { message = $"状态机 '{body.Name}' 已启动" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/state-machine/fire", async (HttpContext context, StateMachineService smService) =>
{
    var body = await context.Request.ReadFromJsonAsync<FireRequest>();
    if (body == null || string.IsNullOrEmpty(body.Name) || string.IsNullOrEmpty(body.Event))
    {
        return Results.BadRequest("请提供状态机名称和事件");
    }

    try
    {
        var stateMachine = smService.GetStateMachine<string, string>(body.Name);
        var success = await stateMachine.FireAsync(body.Event);
        var currentState = stateMachine.CurrentState;
        return Results.Ok(new { success, currentState });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/state-machine/list", (StateMachineService smService) =>
{
    var names = smService.GetAllStateMachineNames();
    return Results.Ok(new { stateMachines = names });
});

app.MapPost("/state-machine/stop", async (HttpContext context, StateMachineService smService) =>
{
    var body = await context.Request.ReadFromJsonAsync<StopRequest>();
    if (body == null || string.IsNullOrEmpty(body.Name))
    {
        return Results.BadRequest("请提供状态机名称");
    }

    try
    {
        var stateMachine = smService.GetStateMachine<string, string>(body.Name);
        stateMachine.Stop();
        return Results.Ok(new { message = $"状态机 '{body.Name}' 已停止" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/state-machine/remove", async (HttpContext context, StateMachineService smService) =>
{
    var body = await context.Request.ReadFromJsonAsync<RemoveRequest>();
    if (body == null || string.IsNullOrEmpty(body.Name))
    {
        return Results.BadRequest("请提供状态机名称");
    }

    try
    {
        var success = smService.RemoveStateMachine(body.Name);
        return Results.Ok(new { success, message = $"状态机 '{body.Name}' 已移除" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();

// 请求模型
public class CreateRequest
{
    public string Name { get; set; }
}

public class StartRequest
{
    public string Name { get; set; }
    public string InitialState { get; set; }
}

public class FireRequest
{
    public string Name { get; set; }
    public string Event { get; set; }
}

public class StopRequest
{
    public string Name { get; set; }
}

public class RemoveRequest
{
    public string Name { get; set; }
}
```

### 4.3 集成到控制台应用

**功能说明**：在控制台应用中使用 LiquidState

**示例代码**：

```csharp
using LiquidStateAot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        // 构建服务容器
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        services.AddMemoryCache();
        services.AddSingleton<StateMachineService>();

        var serviceProvider = services.BuildServiceProvider();
        var stateMachineService = serviceProvider.GetRequiredService<StateMachineService>();

        // 主菜单
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("=== LiquidState 状态机管理工具 ===");
            Console.WriteLine("1. 创建状态机");
            Console.WriteLine("2. 启动状态机");
            Console.WriteLine("3. 触发事件");
            Console.WriteLine("4. 停止状态机");
            Console.WriteLine("5. 列出状态机");
            Console.WriteLine("6. 移除状态机");
            Console.WriteLine("7. 退出");
            Console.Write("请选择操作: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    await CreateStateMachine(stateMachineService, serviceProvider);
                    break;
                case "2":
                    await StartStateMachine(stateMachineService);
                    break;
                case "3":
                    await FireEvent(stateMachineService);
                    break;
                case "4":
                    await StopStateMachine(stateMachineService);
                    break;
                case "5":
                    ListStateMachines(stateMachineService);
                    break;
                case "6":
                    await RemoveStateMachine(stateMachineService);
                    break;
                case "7":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("无效选择，请重新输入。");
                    break;
            }

            if (!exit)
            {
                Console.WriteLine("按任意键继续...");
                Console.ReadKey();
            }
        }
    }

    static async Task CreateStateMachine(StateMachineService stateMachineService, IServiceProvider serviceProvider)
    {
        Console.Write("请输入状态机名称: ");
        var name = Console.ReadLine();

        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("状态机名称不能为空。");
            return;
        }

        try
        {
            // 创建状态机
            var builder = stateMachineService.CreateBuilder<string, string>();
            builder.AddTransition("Idle", "Start", "Running");
            builder.AddTransition("Running", "Stop", "Idle");
            var stateMachine = builder.Build(serviceProvider);
            stateMachineService.RegisterStateMachine(name, stateMachine);

            Console.WriteLine($"状态机 '{name}' 已创建。");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"创建状态机失败: {ex.Message}");
        }
    }

    static async Task StartStateMachine(StateMachineService stateMachineService)
    {
        Console.Write("请输入状态机名称: ");
        var name = Console.ReadLine();
        Console.Write("请输入初始状态: ");
        var initialState = Console.ReadLine();

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(initialState))
        {
            Console.WriteLine("状态机名称和初始状态不能为空。");
            return;
        }

        try
        {
            var stateMachine = stateMachineService.GetStateMachine<string, string>(name);
            stateMachine.Start(initialState);
            Console.WriteLine($"状态机 '{name}' 已启动，初始状态: {initialState}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"启动状态机失败: {ex.Message}");
        }
    }

    static async Task FireEvent(StateMachineService stateMachineService)
    {
        Console.Write("请输入状态机名称: ");
        var name = Console.ReadLine();
        Console.Write("请输入事件: ");
        var @event = Console.ReadLine();

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(@event))
        {
            Console.WriteLine("状态机名称和事件不能为空。");
            return;
        }

        try
        {
            var stateMachine = stateMachineService.GetStateMachine<string, string>(name);
            var success = await stateMachine.FireAsync(@event);
            Console.WriteLine($"事件 '{@event}' 触发{(success ? "成功" : "失败" )}");
            Console.WriteLine($"当前状态: {stateMachine.CurrentState}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"触发事件失败: {ex.Message}");
        }
    }

    static async Task StopStateMachine(StateMachineService stateMachineService)
    {
        Console.Write("请输入状态机名称: ");
        var name = Console.ReadLine();

        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("状态机名称不能为空。");
            return;
        }

        try
        {
            var stateMachine = stateMachineService.GetStateMachine<string, string>(name);
            stateMachine.Stop();
            Console.WriteLine($"状态机 '{name}' 已停止。");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"停止状态机失败: {ex.Message}");
        }
    }

    static void ListStateMachines(StateMachineService stateMachineService)
    {
        var names = stateMachineService.GetAllStateMachineNames();
        Console.WriteLine("已注册的状态机:");
        foreach (var name in names)
        {
            Console.WriteLine($"- {name}");
        }
    }

    static async Task RemoveStateMachine(StateMachineService stateMachineService)
    {
        Console.Write("请输入状态机名称: ");
        var name = Console.ReadLine();

        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("状态机名称不能为空。");
            return;
        }

        try
        {
            var success = stateMachineService.RemoveStateMachine(name);
            Console.WriteLine($"状态机 '{name}' {(success ? "已移除" : "不存在" )}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"移除状态机失败: {ex.Message}");
        }
    }
}
```

## 5. 使用场景

### 5.1 工作流管理

**场景**：使用状态机管理业务工作流

**示例**：

```csharp
// 定义订单状态和事件
enum OrderStatus { Created, Processing, Shipped, Delivered, Cancelled }
enum OrderAction { Process, Ship, Deliver, Cancel }

// 创建订单状态机
var builder = stateMachineService.CreateBuilder<OrderStatus, OrderAction>();

// 配置状态转换
builder.AddTransition(OrderStatus.Created, OrderAction.Process, OrderStatus.Processing, async () =>
{
    Console.WriteLine("订单开始处理...");
    await Task.Delay(1000);
    Console.WriteLine("订单处理中");
});

builder.AddTransition(OrderStatus.Processing, OrderAction.Ship, OrderStatus.Shipped, async () =>
{
    Console.WriteLine("订单开始发货...");
    await Task.Delay(1000);
    Console.WriteLine("订单已发货");
});

builder.AddTransition(OrderStatus.Shipped, OrderAction.Deliver, OrderStatus.Delivered, async () =>
{
    Console.WriteLine("订单开始配送...");
    await Task.Delay(1000);
    Console.WriteLine("订单已送达");
});

builder.AddTransition(OrderStatus.Created, OrderAction.Cancel, OrderStatus.Cancelled, async () =>
{
    Console.WriteLine("订单已取消");
});

builder.AddTransition(OrderStatus.Processing, OrderAction.Cancel, OrderStatus.Cancelled, async () =>
{
    Console.WriteLine("订单已取消");
});

// 构建并使用状态机
var orderStateMachine = builder.Build(serviceProvider);
orderStateMachine.Start(OrderStatus.Created);

// 模拟订单处理流程
await orderStateMachine.FireAsync(OrderAction.Process);
await orderStateMachine.FireAsync(OrderAction.Ship);
await orderStateMachine.FireAsync(OrderAction.Deliver);
```

### 5.2 设备状态管理

**场景**：使用状态机管理设备状态

**示例**：

```csharp
// 定义设备状态和事件
enum DeviceState { Offline, Online, Busy, Error }
enum DeviceEvent { Connect, Disconnect, StartTask, CompleteTask, ReportError, Reset }

// 创建设备状态机
var builder = stateMachineService.CreateBuilder<DeviceState, DeviceEvent>();

// 配置状态转换
builder.AddTransition(DeviceState.Offline, DeviceEvent.Connect, DeviceState.Online, async () =>
{
    Console.WriteLine("设备已连接");
});

builder.AddTransition(DeviceState.Online, DeviceEvent.Disconnect, DeviceState.Offline, async () =>
{
    Console.WriteLine("设备已断开连接");
});

builder.AddTransition(DeviceState.Online, DeviceEvent.StartTask, DeviceState.Busy, async () =>
{
    Console.WriteLine("设备开始执行任务");
});

builder.AddTransition(DeviceState.Busy, DeviceEvent.CompleteTask, DeviceState.Online, async () =>
{
    Console.WriteLine("设备任务已完成");
});

builder.AddTransition(DeviceState.Busy, DeviceEvent.ReportError, DeviceState.Error, async () =>
{
    Console.WriteLine("设备报告错误");
});

builder.AddTransition(DeviceState.Error, DeviceEvent.Reset, DeviceState.Online, async () =>
{
    Console.WriteLine("设备已重置");
});

// 构建并使用状态机
var deviceStateMachine = builder.Build(serviceProvider);
deviceStateMachine.Start(DeviceState.Offline);

// 模拟设备操作流程
await deviceStateMachine.FireAsync(DeviceEvent.Connect);
await deviceStateMachine.FireAsync(DeviceEvent.StartTask);
await deviceStateMachine.FireAsync(DeviceEvent.CompleteTask);
await deviceStateMachine.FireAsync(DeviceEvent.Disconnect);
```

### 5.3 游戏状态管理

**场景**：使用状态机管理游戏状态

**示例**：

```csharp
// 定义游戏状态和事件
enum GameState { Menu, Loading, Playing, Paused, GameOver }
enum GameEvent { StartGame, LoadComplete, Pause, Resume, GameOver, Restart, ExitToMenu }

// 创建游戏状态机
var builder = stateMachineService.CreateBuilder<GameState, GameEvent>();

// 配置状态转换
builder.AddTransition(GameState.Menu, GameEvent.StartGame, GameState.Loading, async () =>
{
    Console.WriteLine("开始加载游戏...");
    await Task.Delay(2000);
    Console.WriteLine("游戏加载中");
});

builder.AddTransition(GameState.Loading, GameEvent.LoadComplete, GameState.Playing, async () =>
{
    Console.WriteLine("游戏加载完成");
    Console.WriteLine("游戏开始");
});

builder.AddTransition(GameState.Playing, GameEvent.Pause, GameState.Paused, async () =>
{
    Console.WriteLine("游戏已暂停");
});

builder.AddTransition(GameState.Paused, GameEvent.Resume, GameState.Playing, async () =>
{
    Console.WriteLine("游戏已恢复");
});

builder.AddTransition(GameState.Playing, GameEvent.GameOver, GameState.GameOver, async () =>
{
    Console.WriteLine("游戏结束");
});

builder.AddTransition(GameState.GameOver, GameEvent.Restart, GameState.Loading, async () =>
{
    Console.WriteLine("重新开始游戏");
});

builder.AddTransition(GameState.GameOver, GameEvent.ExitToMenu, GameState.Menu, async () =>
{
    Console.WriteLine("返回主菜单");
});

builder.AddTransition(GameState.Paused, GameEvent.ExitToMenu, GameState.Menu, async () =>
{
    Console.WriteLine("返回主菜单");
});

// 构建并使用状态机
var gameStateMachine = builder.Build(serviceProvider);
gameStateMachine.Start(GameState.Menu);

// 模拟游戏流程
await gameStateMachine.FireAsync(GameEvent.StartGame);
await gameStateMachine.FireAsync(GameEvent.LoadComplete);
await gameStateMachine.FireAsync(GameEvent.Pause);
await gameStateMachine.FireAsync(GameEvent.Resume);
await gameStateMachine.FireAsync(GameEvent.GameOver);
await gameStateMachine.FireAsync(GameEvent.ExitToMenu);
```

## 6. 性能优化

### 6.1 缓存优化

**功能说明**：调整缓存设置以提高性能

**示例**：

```csharp
// 配置内存缓存
var cacheOptions = new MemoryCacheOptions
{
    SizeLimit = 1024 * 1024, // 1MB
    ExpirationScanFrequency = TimeSpan.FromMinutes(5)
};

// 注册缓存服务
services.AddSingleton<IMemoryCache>(new MemoryCache(cacheOptions));

// 添加状态机服务
services.AddSingleton<StateMachineService>();
```

### 6.2 异步操作优化

**功能说明**：优化异步操作，提高响应速度

**最佳实践**：

1. **使用 async/await**：所有状态转换和动作都使用异步操作
2. **避免阻塞调用**：不使用 Task.Wait() 或 Task.Result
3. **合理使用 Task.Delay**：对于长时间运行的操作，使用 Task.Delay 释放线程
4. **并行处理**：对于独立的操作，使用 Task.WhenAll 并行执行

**示例**：

```csharp
// 并行处理多个状态机
var tasks = new List<Task>();

foreach (var name in stateMachineNames)
{
    tasks.Add(Task.Run(async () =>
    {
        var stateMachine = stateMachineService.GetStateMachine<string, string>(name);
        await stateMachine.FireAsync("Start");
        // 执行其他操作
    }));
}

await Task.WhenAll(tasks);
```

### 6.3 内存管理

**功能说明**：优化内存使用，避免内存泄漏

**最佳实践**：

1. **及时停止状态机**：不再使用的状态机及时停止
2. **移除未使用的状态机**：不再使用的状态机从服务中移除
3. **限制状态机数量**：根据应用需求限制状态机数量
4. **监控内存使用**：定期检查内存使用情况

**示例**：

```csharp
// 监控内存使用
var process = System.Diagnostics.Process.GetCurrentProcess();

Console.WriteLine($"初始内存使用: {process.WorkingSet64 / 1024 / 1024} MB");

// 创建多个状态机
for (int i = 0; i < 100; i++)
{
    var name = $"machine_{i}";
    var builder = stateMachineService.CreateBuilder<string, string>();
    builder.AddTransition("Idle", "Start", "Running");
    var stateMachine = builder.Build(serviceProvider);
    stateMachineService.RegisterStateMachine(name, stateMachine);
}

Console.WriteLine($"创建状态机后内存使用: {process.WorkingSet64 / 1024 / 1024} MB");

// 移除状态机
for (int i = 0; i < 100; i++)
{
    var name = $"machine_{i}";
    stateMachineService.RemoveStateMachine(name);
}

// 强制垃圾回收
GC.Collect();
GC.WaitForPendingFinalizers();

Console.WriteLine($"移除状态机后内存使用: {process.WorkingSet64 / 1024 / 1024} MB");
```

## 7. 错误处理

### 7.1 常见错误及解决方案

| 错误类型 | 错误信息 | 解决方案 |
|---------|---------|----------|
| KeyNotFoundException | 状态机未找到: system | 确保状态机已创建并注册 |
| ArgumentException | 参数无效 | 检查输入参数是否正确 |
| InvalidOperationException | 状态机未运行，无法触发事件 | 确保状态机已启动 |
| Exception | 启动失败: 系统资源不足 | 检查系统资源，实现错误处理 |

### 7.2 错误处理示例

**功能说明**：实现健壮的错误处理策略

**示例代码**：

```csharp
// 实现错误处理
var builder = stateMachineService.CreateBuilder<string, string>();

// 设置错误处理程序
builder.WithErrorHandler(ex =>
{
    Console.WriteLine($"错误处理: {ex.Message}");
    // 可以添加日志记录、报警等逻辑
});

// 添加可能出错的转换
builder.AddTransition("Idle", "Start", "Running", async () =>
{
    try
    {
        Console.WriteLine("启动系统...");
        // 模拟错误
        throw new Exception("启动失败: 系统资源不足");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"转换动作出错: {ex.Message}");
        throw; // 重新抛出异常，让错误处理程序处理
    }
});

// 构建并使用状态机
var stateMachine = builder.Build(serviceProvider);
stateMachine.Start("Idle");

try
{
    await stateMachine.FireAsync("Start");
}
catch (Exception ex)
{
    Console.WriteLine($"触发事件失败: {ex.Message}");
}
```

### 7.3 日志记录

**功能说明**：配置详细的日志记录，便于故障排查

**示例代码**：

```csharp
// 配置详细的日志记录
var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole(options =>
    {
        options.Format = ConsoleLoggerFormat.Systemd;
    });
    builder.AddFile("liquidstate.log", options =>
    {
        options.FileSizeLimit = 10 * 1024 * 1024; // 10MB
        options.MaxRollingFiles = 5;
        options.MinLevel = LogLevel.Trace;
    });
    builder.SetMinimumLevel(LogLevel.Debug);
});

services.AddSingleton(loggerFactory);
services.AddLogging();
services.AddMemoryCache();
services.AddSingleton<StateMachineService>();

// 使用状态机
var stateMachineService = serviceProvider.GetRequiredService<StateMachineService>();
var builder = stateMachineService.CreateBuilder<string, string>();
builder.AddTransition("Idle", "Start", "Running");
var stateMachine = builder.Build(serviceProvider);
stateMachine.Start("Idle");

// 触发事件
await stateMachine.FireAsync("Start");
```

## 8. 总结

LiquidState AOT 是一个功能强大的状态机工具，基于 .NET 10.0 的 AOT 编译架构，提供了全面的状态管理、事件处理和状态转换功能。通过本文档提供的示例和最佳实践，您可以：

1. **快速上手**：使用命令行接口执行各种状态机操作
2. **高级使用**：通过编程方式创建复杂的状态机，支持条件转换、错误处理等高级功能
3. **性能优化**：利用缓存、异步操作和内存管理提高性能
4. **错误处理**：实现健壮的错误处理策略，确保工具稳定运行
5. **集成应用**：将 LiquidState 集成到各种应用场景中，如工作流管理、设备状态管理、游戏状态管理等

LiquidState AOT 工具设计为跨平台、高性能和可扩展的，适用于各种 .NET 项目的状态管理场景。无论您是在命令行中使用，还是在代码中集成，LiquidState AOT 都能为您提供强大的状态机功能支持。

# LiteDB 使用示例文档

## 1. 基本用法

### 1.1 创建集合

**功能说明**：创建一个新的集合

**命令行用法**：

```bash
litedb_aot.exe create-collection --database "data.db" --collection "users"
```

**输出示例**：

```
集合创建成功
```

### 1.2 插入文档

**功能说明**：向集合中插入一个文档

**命令行用法**：

```bash
litedb_aot.exe insert --database "data.db" --collection "users" --document '{"name": "张三", "age": 30, "email": "zhangsan@example.com"}'
```

**输出示例**：

```
文档插入成功，ID: 5f8d0d5a-1234-4567-89ab-cdef01234567
```

### 1.3 查询文档

**功能说明**：查询集合中的文档

**命令行用法**：

```bash
# 查询所有文档
litedb_aot.exe query --database "data.db" --collection "users"

# 使用查询条件
litedb_aot.exe query --database "data.db" --collection "users" --query "$.age > 25"
```

**输出示例**：

```json
[
  {
    "_id": "5f8d0d5a-1234-4567-89ab-cdef01234567",
    "name": "张三",
    "age": 30,
    "email": "zhangsan@example.com"
  }
]
```

### 1.4 更新文档

**功能说明**：更新集合中的文档

**命令行用法**：

```bash
litedb_aot.exe update --database "data.db" --collection "users" --id "5f8d0d5a-1234-4567-89ab-cdef01234567" --document '{"name": "张三", "age": 31, "email": "zhangsan@example.com"}'
```

**输出示例**：

```
文档更新成功
```

### 1.5 删除文档

**功能说明**：从集合中删除文档

**命令行用法**：

```bash
litedb_aot.exe delete --database "data.db" --collection "users" --id "5f8d0d5a-1234-4567-89ab-cdef01234567"
```

**输出示例**：

```
文档删除成功
```

### 1.6 列出所有集合

**功能说明**：列出数据库中的所有集合

**命令行用法**：

```bash
litedb_aot.exe list-collections --database "data.db"
```

**输出示例**：

```json
[
  "users",
  "products",
  "orders"
]
```

### 1.7 删除集合

**功能说明**：删除数据库中的集合

**命令行用法**：

```bash
litedb_aot.exe drop-collection --database "data.db" --collection "users"
```

**输出示例**：

```
集合删除成功
```

### 1.8 备份数据库

**功能说明**：备份数据库文件

**命令行用法**：

```bash
litedb_aot.exe backup --database "data.db" --backup "backup/data_backup.db"
```

**输出示例**：

```
数据库备份成功
```

### 1.9 压缩数据库

**功能说明**：压缩数据库文件，减少文件大小

**命令行用法**：

```bash
litedb_aot.exe compact --database "data.db"
```

**输出示例**：

```
数据库压缩成功
```

### 1.10 获取数据库信息

**功能说明**：获取数据库的详细信息

**命令行用法**：

```bash
litedb_aot.exe info --database "data.db"
```

**输出示例**：

```json
{
  "DatabasePath": "data.db",
  "FileSize": 1048576,
  "CollectionCount": 3,
  "Collections": [
    "users",
    "products",
    "orders"
  ]
}
```

## 2. 高级用法

### 2.1 编程方式使用

**功能说明**：在代码中使用LiteDB

**示例代码**：

```csharp
using LiteDBAot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// 构建服务容器
var services = new ServiceCollection();
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});
services.AddMemoryCache();
services.AddSingleton<ILiteDBService, LiteDBService>();

var serviceProvider = services.BuildServiceProvider();
var liteDBService = serviceProvider.GetRequiredService<ILiteDBService>();

// 数据库路径
string databasePath = "data.db";
string collectionName = "users";

// 创建集合
Console.WriteLine("创建集合...");
bool createResult = await liteDBService.CreateCollectionAsync(databasePath, collectionName);
Console.WriteLine(createResult ? "集合创建成功" : "集合创建失败");

// 插入文档
Console.WriteLine("\n插入文档...");
string document = '{"name": "李四", "age": 28, "email": "lisi@example.com"}';
string insertResult = await liteDBService.InsertDocumentAsync(databasePath, collectionName, document);
Console.WriteLine($"文档插入成功，ID: {insertResult}");

// 查询文档
Console.WriteLine("\n查询文档...");
string queryResult = await liteDBService.QueryDocumentsAsync(databasePath, collectionName);
Console.WriteLine(queryResult);

// 更新文档
Console.WriteLine("\n更新文档...");
string updateDocument = '{"name": "李四", "age": 29, "email": "lisi@example.com"}';
bool updateResult = await liteDBService.UpdateDocumentAsync(databasePath, collectionName, insertResult, updateDocument);
Console.WriteLine(updateResult ? "文档更新成功" : "文档更新失败");

// 查询更新后的文档
Console.WriteLine("\n查询更新后的文档...");
string updatedQueryResult = await liteDBService.QueryDocumentsAsync(databasePath, collectionName);
Console.WriteLine(updatedQueryResult);

// 列出所有集合
Console.WriteLine("\n列出所有集合...");
string collectionsResult = await liteDBService.ListCollectionsAsync(databasePath);
Console.WriteLine(collectionsResult);

// 获取数据库信息
Console.WriteLine("\n获取数据库信息...");
string infoResult = await liteDBService.GetDatabaseInfoAsync(databasePath);
Console.WriteLine(infoResult);

// 备份数据库
Console.WriteLine("\n备份数据库...");
string backupPath = "backup/data_backup.db";
bool backupResult = await liteDBService.BackupDatabaseAsync(databasePath, backupPath);
Console.WriteLine(backupResult ? "数据库备份成功" : "数据库备份失败");

// 压缩数据库
Console.WriteLine("\n压缩数据库...");
bool compactResult = await liteDBService.CompactDatabaseAsync(databasePath);
Console.WriteLine(compactResult ? "数据库压缩成功" : "数据库压缩失败");

// 删除文档
Console.WriteLine("\n删除文档...");
bool deleteResult = await liteDBService.DeleteDocumentAsync(databasePath, collectionName, insertResult);
Console.WriteLine(deleteResult ? "文档删除成功" : "文档删除失败");

// 删除集合
Console.WriteLine("\n删除集合...");
bool dropResult = await liteDBService.DropCollectionAsync(databasePath, collectionName);
Console.WriteLine(dropResult ? "集合删除成功" : "集合删除失败");
```

**输出示例**：

```
创建集合...
集合创建成功

插入文档...
文档插入成功，ID: 5f8d0d5a-1234-4567-89ab-cdef01234568

查询文档...
[
  {
    "_id": "5f8d0d5a-1234-4567-89ab-cdef01234568",
    "name": "李四",
    "age": 28,
    "email": "lisi@example.com"
  }
]

更新文档...
文档更新成功

查询更新后的文档...
[
  {
    "_id": "5f8d0d5a-1234-4567-89ab-cdef01234568",
    "name": "李四",
    "age": 29,
    "email": "lisi@example.com"
  }
]

列出所有集合...
[
  "users"
]

获取数据库信息...
{
  "DatabasePath": "data.db",
  "FileSize": 4096,
  "CollectionCount": 1,
  "Collections": [
    "users"
  ]
}

备份数据库...
数据库备份成功

压缩数据库...
数据库压缩成功

删除文档...
文档删除成功

删除集合...
集合删除成功
```

### 2.2 使用查询条件

**功能说明**：使用复杂的查询条件

**示例代码**：

```csharp
// 构建服务容器
var services = new ServiceCollection();
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});
services.AddMemoryCache();
services.AddSingleton<ILiteDBService, LiteDBService>();

var serviceProvider = services.BuildServiceProvider();
var liteDBService = serviceProvider.GetRequiredService<ILiteDBService>();

// 数据库路径
string databasePath = "data.db";
string collectionName = "users";

// 创建集合
await liteDBService.CreateCollectionAsync(databasePath, collectionName);

// 插入多个文档
string[] documents = {
    '{"name": "张三", "age": 30, "email": "zhangsan@example.com", "city": "北京"}',
    '{"name": "李四", "age": 25, "email": "lisi@example.com", "city": "上海"}',
    '{"name": "王五", "age": 35, "email": "wangwu@example.com", "city": "北京"}',
    '{"name": "赵六", "age": 28, "email": "zhaoliu@example.com", "city": "广州"}'
};

foreach (var doc in documents)
{
    await liteDBService.InsertDocumentAsync(databasePath, collectionName, doc);
}

// 基本查询（所有文档）
Console.WriteLine("所有文档:");
string allDocs = await liteDBService.QueryDocumentsAsync(databasePath, collectionName);
Console.WriteLine(allDocs);

// 条件查询（年龄大于28）
Console.WriteLine("\n年龄大于28的文档:");
string ageQuery = await liteDBService.QueryDocumentsAsync(databasePath, collectionName, "$.age > 28");
Console.WriteLine(ageQuery);

// 条件查询（城市为北京）
Console.WriteLine("\n城市为北京的文档:");
string cityQuery = await liteDBService.QueryDocumentsAsync(databasePath, collectionName, "$.city = '北京'");
Console.WriteLine(cityQuery);

// 复合条件查询（年龄大于25且城市为北京）
Console.WriteLine("\n年龄大于25且城市为北京的文档:");
string complexQuery = await liteDBService.QueryDocumentsAsync(databasePath, collectionName, "$.age > 25 AND $.city = '北京'");
Console.WriteLine(complexQuery);
```

**输出示例**：

```
所有文档:
[
  {
    "_id": "5f8d0d5a-1234-4567-89ab-cdef01234569",
    "name": "张三",
    "age": 30,
    "email": "zhangsan@example.com",
    "city": "北京"
  },
  {
    "_id": "5f8d0d5a-1234-4567-89ab-cdef0123456a",
    "name": "李四",
    "age": 25,
    "email": "lisi@example.com",
    "city": "上海"
  },
  {
    "_id": "5f8d0d5a-1234-4567-89ab-cdef0123456b",
    "name": "王五",
    "age": 35,
    "email": "wangwu@example.com",
    "city": "北京"
  },
  {
    "_id": "5f8d0d5a-1234-4567-89ab-cdef0123456c",
    "name": "赵六",
    "age": 28,
    "email": "zhaoliu@example.com",
    "city": "广州"
  }
]

年龄大于28的文档:
[
  {
    "_id": "5f8d0d5a-1234-4567-89ab-cdef01234569",
    "name": "张三",
    "age": 30,
    "email": "zhangsan@example.com",
    "city": "北京"
  },
  {
    "_id": "5f8d0d5a-1234-4567-89ab-cdef0123456b",
    "name": "王五",
    "age": 35,
    "email": "wangwu@example.com",
    "city": "北京"
  }
]

城市为北京的文档:
[
  {
    "_id": "5f8d0d5a-1234-4567-89ab-cdef01234569",
    "name": "张三",
    "age": 30,
    "email": "zhangsan@example.com",
    "city": "北京"
  },
  {
    "_id": "5f8d0d5a-1234-4567-89ab-cdef0123456b",
    "name": "王五",
    "age": 35,
    "email": "wangwu@example.com",
    "city": "北京"
  }
]

年龄大于25且城市为北京的文档:
[
  {
    "_id": "5f8d0d5a-1234-4567-89ab-cdef01234569",
    "name": "张三",
    "age": 30,
    "email": "zhangsan@example.com",
    "city": "北京"
  },
  {
    "_id": "5f8d0d5a-1234-4567-89ab-cdef0123456b",
    "name": "王五",
    "age": 35,
    "email": "wangwu@example.com",
    "city": "北京"
  }
]
```

### 2.3 批量操作

**功能说明**：执行批量操作，提高性能

**示例代码**：

```csharp
using LiteDBAot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// 构建服务容器
var services = new ServiceCollection();
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});
services.AddMemoryCache();
services.AddSingleton<ILiteDBService, LiteDBService>();

var serviceProvider = services.BuildServiceProvider();
var liteDBService = serviceProvider.GetRequiredService<ILiteDBService>();

// 数据库路径
string databasePath = "data.db";
string collectionName = "products";

// 创建集合
await liteDBService.CreateCollectionAsync(databasePath, collectionName);

// 批量插入产品文档
Console.WriteLine("批量插入产品文档...");
int productCount = 100;
var tasks = new List<Task<string>>();

for (int i = 1; i <= productCount; i++)
{
    string productDoc = $'{{"name": "产品{i}", "price": {i * 10.5}, "category": "分类{(i % 5) + 1}", "stock": {100 - i}}}';
    tasks.Add(liteDBService.InsertDocumentAsync(databasePath, collectionName, productDoc));
}

// 并行执行所有插入操作
await Task.WhenAll(tasks);
Console.WriteLine($"成功插入 {tasks.Count} 个产品文档");

// 统计产品数量
Console.WriteLine("\n查询所有产品...");
string allProducts = await liteDBService.QueryDocumentsAsync(databasePath, collectionName);
var productsArray = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement[]>(allProducts);
Console.WriteLine($"产品总数: {productsArray.Length}");

// 查询特定分类的产品
Console.WriteLine("\n查询分类1的产品...");
string categoryQuery = await liteDBService.QueryDocumentsAsync(databasePath, collectionName, "$.category = '分类1'");
var categoryProducts = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement[]>(categoryQuery);
Console.WriteLine($"分类1的产品数量: {categoryProducts.Length}");

// 查询库存少于50的产品
Console.WriteLine("\n查询库存少于50的产品...");
string stockQuery = await liteDBService.QueryDocumentsAsync(databasePath, collectionName, "$.stock < 50");
var stockProducts = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement[]>(stockQuery);
Console.WriteLine($"库存少于50的产品数量: {stockProducts.Length}");
```

**输出示例**：

```
批量插入产品文档...
成功插入 100 个产品文档

查询所有产品...
产品总数: 100

查询分类1的产品...
分类1的产品数量: 20

查询库存少于50的产品...
库存少于50的产品数量: 50
```

### 2.4 数据库维护

**功能说明**：执行数据库维护操作

**示例代码**：

```csharp
using LiteDBAot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

// 构建服务容器
var services = new ServiceCollection();
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});
services.AddMemoryCache();
services.AddSingleton<ILiteDBService, LiteDBService>();

var serviceProvider = services.BuildServiceProvider();
var liteDBService = serviceProvider.GetRequiredService<ILiteDBService>();

// 数据库路径
string databasePath = "data.db";
string backupPath = "backup/data_backup.db";

// 确保备份目录存在
var backupDirectory = Path.GetDirectoryName(backupPath);
if (!string.IsNullOrEmpty(backupDirectory) && !Directory.Exists(backupDirectory))
{
    Directory.CreateDirectory(backupDirectory);
    Console.WriteLine($"创建备份目录: {backupDirectory}");
}

// 获取数据库信息
Console.WriteLine("数据库信息:");
string infoResult = await liteDBService.GetDatabaseInfoAsync(databasePath);
Console.WriteLine(infoResult);

// 备份数据库
Console.WriteLine("\n备份数据库...");
bool backupResult = await liteDBService.BackupDatabaseAsync(databasePath, backupPath);
Console.WriteLine(backupResult ? "数据库备份成功" : "数据库备份失败");

// 检查备份文件是否存在
if (File.Exists(backupPath))
{
    var backupFileInfo = new FileInfo(backupPath);
    Console.WriteLine($"备份文件大小: {backupFileInfo.Length} 字节");
}

// 压缩数据库
Console.WriteLine("\n压缩数据库...");
bool compactResult = await liteDBService.CompactDatabaseAsync(databasePath);
Console.WriteLine(compactResult ? "数据库压缩成功" : "数据库压缩失败");

// 获取压缩后的数据库信息
Console.WriteLine("\n压缩后的数据库信息:");
string compactedInfo = await liteDBService.GetDatabaseInfoAsync(databasePath);
Console.WriteLine(compactedInfo);

// 检查压缩后的文件大小
var databaseFileInfo = new FileInfo(databasePath);
Console.WriteLine($"压缩后数据库文件大小: {databaseFileInfo.Length} 字节");
```

**输出示例**：

```
创建备份目录: backup
数据库信息:
{
  "DatabasePath": "data.db",
  "FileSize": 4096,
  "CollectionCount": 2,
  "Collections": [
    "users",
    "products"
  ]
}

备份数据库...
数据库备份成功
备份文件大小: 4096 字节

压缩数据库...
数据库压缩成功

压缩后的数据库信息:
{
  "DatabasePath": "data.db",
  "FileSize": 4096,
  "CollectionCount": 2,
  "Collections": [
    "users",
    "products"
  ]
}

压缩后数据库文件大小: 4096 字节
```

## 3. 命令行使用技巧

### 3.1 使用完整路径

**功能说明**：使用完整路径避免路径解析错误

**示例**：

```bash
# 使用相对路径
litedb_aot.exe create-collection --database "data.db" --collection "users"

# 使用完整路径（更可靠）
litedb_aot.exe create-collection --database "C:\\Data\\litedb\\data.db" --collection "users"
```

### 3.2 重定向输出到文件

**功能说明**：将命令输出重定向到文件，便于保存和分析

**示例**：

```bash
# 重定向标准输出到文件
litedb_aot.exe query --database "data.db" --collection "users" > users.json

# 重定向标准输出和错误输出到文件
litedb_aot.exe insert --database "data.db" --collection "users" --document '{"name": "张三", "age": 30}' > output.txt 2>&1

# 追加输出到文件
litedb_aot.exe info --database "data.db" >> database_info.txt
```

### 3.3 使用管道传递参数

**功能说明**：使用管道从其他命令接收输入

**示例**：

```powershell
# 从文件读取文档并插入
Get-Content document.json | ForEach-Object {
    litedb_aot.exe insert --database "data.db" --collection "users" --document $_
}

# 批量创建集合
@("users", "products", "orders", "customers") | ForEach-Object {
    litedb_aot.exe create-collection --database "data.db" --collection $_
}
```

### 3.4 使用环境变量

**功能说明**：设置环境变量来自定义LiteDB行为

**示例**：

```bash
# 设置日志级别
set LITEDB_LOG_LEVEL=Debug
litedb_aot.exe query --database "data.db" --collection "users"

# 设置缓存大小
set LITEDB_CACHE_SIZE=2048
litedb_aot.exe insert --database "data.db" --collection "users" --document '{"name": "张三", "age": 30}'
```

## 4. 编程集成

### 4.1 基本集成

**功能说明**：在 C# 代码中集成 LiteDB

**示例代码**：

```csharp
using LiteDBAot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// 构建服务容器
var services = new ServiceCollection();
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});
services.AddMemoryCache();
services.AddSingleton<ILiteDBService, LiteDBService>();

var serviceProvider = services.BuildServiceProvider();
var liteDBService = serviceProvider.GetRequiredService<ILiteDBService>();

// 使用LiteDB服务
string databasePath = "app.db";
string collectionName = "settings";

// 创建集合
await liteDBService.CreateCollectionAsync(databasePath, collectionName);

// 插入应用设置
string settingsDoc = '{"appName": "MyApp", "version": "1.0.0", "theme": "dark", "language": "zh-CN"}';
string settingsId = await liteDBService.InsertDocumentAsync(databasePath, collectionName, settingsDoc);

// 查询设置
string settings = await liteDBService.QueryDocumentsAsync(databasePath, collectionName);
Console.WriteLine("应用设置:");
Console.WriteLine(settings);
```

### 4.2 集成到 ASP.NET Core 应用

**功能说明**：在 ASP.NET Core 应用中使用 LiteDB

**示例代码**：

```csharp
using LiteDBAot;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 添加LiteDB服务
builder.Services.AddLogging();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ILiteDBService, LiteDBService>();

var app = builder.Build();

// 获取LiteDB服务
var liteDBService = app.Services.GetRequiredService<ILiteDBService>();

// 数据库路径
string databasePath = "app.db";
string usersCollection = "users";

// 确保集合存在
await liteDBService.CreateCollectionAsync(databasePath, usersCollection);

// 用户操作端点
app.MapGet("/users", async (ILiteDBService dbService) =>
{
    var users = await dbService.QueryDocumentsAsync(databasePath, usersCollection);
    return Results.Content(users, "application/json");
});

app.MapPost("/users", async (HttpRequest request, ILiteDBService dbService) =>
{
    var body = await new StreamReader(request.Body).ReadToEndAsync();
    try
    {
        var id = await dbService.InsertDocumentAsync(databasePath, usersCollection, body);
        return Results.Created($"/users/{id}", new { id, message = "用户创建成功" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/users/{id}", async (string id, ILiteDBService dbService) =>
{
    // 注意：这里简化处理，实际应该使用更精确的查询
    var users = await dbService.QueryDocumentsAsync(databasePath, usersCollection);
    var usersArray = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement[]>(users);
    var user = usersArray.FirstOrDefault(u => u.GetProperty("_id").GetString() == id);
    
    if (user.ValueKind != System.Text.Json.JsonValueKind.Undefined)
    {
        return Results.Content(user.GetRawText(), "application/json");
    }
    else
    {
        return Results.NotFound(new { message = "用户不存在" });
    }
});

app.MapPut("/users/{id}", async (string id, HttpRequest request, ILiteDBService dbService) =>
{
    var body = await new StreamReader(request.Body).ReadToEndAsync();
    try
    {
        var result = await dbService.UpdateDocumentAsync(databasePath, usersCollection, id, body);
        if (result)
        {
            return Results.Ok(new { message = "用户更新成功" });
        }
        else
        {
            return Results.NotFound(new { message = "用户不存在" });
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapDelete("/users/{id}", async (string id, ILiteDBService dbService) =>
{
    try
    {
        var result = await dbService.DeleteDocumentAsync(databasePath, usersCollection, id);
        if (result)
        {
            return Results.Ok(new { message = "用户删除成功" });
        }
        else
        {
            return Results.NotFound(new { message = "用户不存在" });
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();
```

### 4.3 集成到控制台应用

**功能说明**：在控制台应用中使用 LiteDB

**示例代码**：

```csharp
using LiteDBAot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        // 构建服务容器
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        services.AddMemoryCache();
        services.AddSingleton<ILiteDBService, LiteDBService>();

        var serviceProvider = services.BuildServiceProvider();
        var liteDBService = serviceProvider.GetRequiredService<ILiteDBService>();

        // 主菜单
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("=== LiteDB 数据库管理工具 ===");
            Console.WriteLine("1. 创建集合");
            Console.WriteLine("2. 插入文档");
            Console.WriteLine("3. 查询文档");
            Console.WriteLine("4. 更新文档");
            Console.WriteLine("5. 删除文档");
            Console.WriteLine("6. 列出集合");
            Console.WriteLine("7. 删除集合");
            Console.WriteLine("8. 备份数据库");
            Console.WriteLine("9. 压缩数据库");
            Console.WriteLine("10. 数据库信息");
            Console.WriteLine("11. 退出");
            Console.Write("请选择操作: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    await CreateCollection(liteDBService);
                    break;
                case "2":
                    await InsertDocument(liteDBService);
                    break;
                case "3":
                    await QueryDocuments(liteDBService);
                    break;
                case "4":
                    await UpdateDocument(liteDBService);
                    break;
                case "5":
                    await DeleteDocument(liteDBService);
                    break;
                case "6":
                    await ListCollections(liteDBService);
                    break;
                case "7":
                    await DropCollection(liteDBService);
                    break;
                case "8":
                    await BackupDatabase(liteDBService);
                    break;
                case "9":
                    await CompactDatabase(liteDBService);
                    break;
                case "10":
                    await GetDatabaseInfo(liteDBService);
                    break;
                case "11":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("无效选择，请重新输入。");
                    break;
            }

            if (!exit)
            {
                Console.WriteLine("按任意键继续...");
                Console.ReadKey();
            }
        }
    }

    static async Task CreateCollection(ILiteDBService service)
    {
        Console.Write("请输入数据库路径: ");
        string database = Console.ReadLine();
        Console.Write("请输入集合名称: ");
        string collection = Console.ReadLine();

        if (string.IsNullOrEmpty(database) || string.IsNullOrEmpty(collection))
        {
            Console.WriteLine("数据库路径和集合名称不能为空。");
            return;
        }

        try
        {
            bool result = await service.CreateCollectionAsync(database, collection);
            Console.WriteLine(result ? "集合创建成功" : "集合创建失败");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"创建集合失败: {ex.Message}");
        }
    }

    static async Task InsertDocument(ILiteDBService service)
    {
        Console.Write("请输入数据库路径: ");
        string database = Console.ReadLine();
        Console.Write("请输入集合名称: ");
        string collection = Console.ReadLine();
        Console.Write("请输入文档内容 (JSON): ");
        string document = Console.ReadLine();

        if (string.IsNullOrEmpty(database) || string.IsNullOrEmpty(collection) || string.IsNullOrEmpty(document))
        {
            Console.WriteLine("数据库路径、集合名称和文档内容不能为空。");
            return;
        }

        try
        {
            string result = await service.InsertDocumentAsync(database, collection, document);
            Console.WriteLine($"文档插入成功，ID: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"插入文档失败: {ex.Message}");
        }
    }

    static async Task QueryDocuments(ILiteDBService service)
    {
        Console.Write("请输入数据库路径: ");
        string database = Console.ReadLine();
        Console.Write("请输入集合名称: ");
        string collection = Console.ReadLine();
        Console.Write("请输入查询条件 (可选): ");
        string query = Console.ReadLine();

        if (string.IsNullOrEmpty(database) || string.IsNullOrEmpty(collection))
        {
            Console.WriteLine("数据库路径和集合名称不能为空。");
            return;
        }

        try
        {
            string result = await service.QueryDocumentsAsync(database, collection, string.IsNullOrEmpty(query) ? null : query);
            Console.WriteLine("查询结果:");
            Console.WriteLine(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"查询文档失败: {ex.Message}");
        }
    }

    static async Task UpdateDocument(ILiteDBService service)
    {
        Console.Write("请输入数据库路径: ");
        string database = Console.ReadLine();
        Console.Write("请输入集合名称: ");
        string collection = Console.ReadLine();
        Console.Write("请输入文档ID: ");
        string id = Console.ReadLine();
        Console.Write("请输入更新后的文档内容 (JSON): ");
        string document = Console.ReadLine();

        if (string.IsNullOrEmpty(database) || string.IsNullOrEmpty(collection) || string.IsNullOrEmpty(id) || string.IsNullOrEmpty(document))
        {
            Console.WriteLine("所有字段都不能为空。");
            return;
        }

        try
        {
            bool result = await service.UpdateDocumentAsync(database, collection, id, document);
            Console.WriteLine(result ? "文档更新成功" : "文档更新失败");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"更新文档失败: {ex.Message}");
        }
    }

    static async Task DeleteDocument(ILiteDBService service)
    {
        Console.Write("请输入数据库路径: ");
        string database = Console.ReadLine();
        Console.Write("请输入集合名称: ");
        string collection = Console.ReadLine();
        Console.Write("请输入文档ID: ");
        string id = Console.ReadLine();

        if (string.IsNullOrEmpty(database) || string.IsNullOrEmpty(collection) || string.IsNullOrEmpty(id))
        {
            Console.WriteLine("数据库路径、集合名称和文档ID不能为空。");
            return;
        }

        try
        {
            bool result = await service.DeleteDocumentAsync(database, collection, id);
            Console.WriteLine(result ? "文档删除成功" : "文档删除失败");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"删除文档失败: {ex.Message}");
        }
    }

    static async Task ListCollections(ILiteDBService service)
    {
        Console.Write("请输入数据库路径: ");
        string database = Console.ReadLine();

        if (string.IsNullOrEmpty(database))
        {
            Console.WriteLine("数据库路径不能为空。");
            return;
        }

        try
        {
            string result = await service.ListCollectionsAsync(database);
            Console.WriteLine("集合列表:");
            Console.WriteLine(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"列出集合失败: {ex.Message}");
        }
    }

    static async Task DropCollection(ILiteDBService service)
    {
        Console.Write("请输入数据库路径: ");
        string database = Console.ReadLine();
        Console.Write("请输入集合名称: ");
        string collection = Console.ReadLine();

        if (string.IsNullOrEmpty(database) || string.IsNullOrEmpty(collection))
        {
            Console.WriteLine("数据库路径和集合名称不能为空。");
            return;
        }

        try
        {
            bool result = await service.DropCollectionAsync(database, collection);
            Console.WriteLine(result ? "集合删除成功" : "集合删除失败");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"删除集合失败: {ex.Message}");
        }
    }

    static async Task BackupDatabase(ILiteDBService service)
    {
        Console.Write("请输入数据库路径: ");
        string database = Console.ReadLine();
        Console.Write("请输入备份路径: ");
        string backup = Console.ReadLine();

        if (string.IsNullOrEmpty(database) || string.IsNullOrEmpty(backup))
        {
            Console.WriteLine("数据库路径和备份路径不能为空。");
            return;
        }

        try
        {
            bool result = await service.BackupDatabaseAsync(database, backup);
            Console.WriteLine(result ? "数据库备份成功" : "数据库备份失败");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"备份数据库失败: {ex.Message}");
        }
    }

    static async Task CompactDatabase(ILiteDBService service)
    {
        Console.Write("请输入数据库路径: ");
        string database = Console.ReadLine();

        if (string.IsNullOrEmpty(database))
        {
            Console.WriteLine("数据库路径不能为空。");
            return;
        }

        try
        {
            bool result = await service.CompactDatabaseAsync(database);
            Console.WriteLine(result ? "数据库压缩成功" : "数据库压缩失败");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"压缩数据库失败: {ex.Message}");
        }
    }

    static async Task GetDatabaseInfo(ILiteDBService service)
    {
        Console.Write("请输入数据库路径: ");
        string database = Console.ReadLine();

        if (string.IsNullOrEmpty(database))
        {
            Console.WriteLine("数据库路径不能为空。");
            return;
        }

        try
        {
            string result = await service.GetDatabaseInfoAsync(database);
            Console.WriteLine("数据库信息:");
            Console.WriteLine(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"获取数据库信息失败: {ex.Message}");
        }
    }
}
```

## 5. 使用场景

### 5.1 配置存储

**场景**：使用LiteDB存储应用程序配置

**示例**：

```csharp
// 存储配置
string configDocument = '{"appName": "MyApplication", "version": "1.0.0", "settings": {"theme": "dark", "language": "zh-CN", "autoSave": true, "maxItems": 100}}';
await liteDBService.InsertDocumentAsync("config.db", "app_config", configDocument);

// 读取配置
string config = await liteDBService.QueryDocumentsAsync("config.db", "app_config");
var configObject = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(config);
string theme = configObject.GetProperty("settings").GetProperty("theme").GetString();
Console.WriteLine($"当前主题: {theme}");

// 更新配置
string updatedConfig = '{"appName": "MyApplication", "version": "1.0.1", "settings": {"theme": "light", "language": "zh-CN", "autoSave": true, "maxItems": 200}}';
await liteDBService.UpdateDocumentAsync("config.db", "app_config", configObject.GetProperty("_id").GetString(), updatedConfig);
```

### 5.2 本地缓存

**场景**：使用LiteDB作为本地缓存

**示例**：

```csharp
// 缓存API响应
string apiResponse = await GetApiResponseAsync("https://api.example.com/data");
string cacheKey = "api_data_" + DateTime.UtcNow.ToString("yyyyMMddHHmm");
string cacheDocument = $'{"key": "{cacheKey}", "data": {apiResponse}, "timestamp": "{DateTime.UtcNow}", "expires": "{DateTime.UtcNow.AddHours(1)}"}';
await liteDBService.InsertDocumentAsync("cache.db", "api_cache", cacheDocument);

// 检查缓存
string cachedData = await liteDBService.QueryDocumentsAsync("cache.db", "api_cache", $"$.key = '{cacheKey}'");
if (!string.IsNullOrEmpty(cachedData) && cachedData != "[]")
{
    Console.WriteLine("从缓存读取数据:");
    Console.WriteLine(cachedData);
}
else
{
    Console.WriteLine("缓存未命中，从API获取数据...");
    // 从API获取数据并缓存
}

// 清理过期缓存
string expiredQuery = await liteDBService.QueryDocumentsAsync("cache.db", "api_cache", $"$.expires < '{DateTime.UtcNow}'");
var expiredItems = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement[]>(expiredQuery);
foreach (var item in expiredItems)
{
    await liteDBService.DeleteDocumentAsync("cache.db", "api_cache", item.GetProperty("_id").GetString());
}
Console.WriteLine($"清理了 {expiredItems.Length} 个过期缓存项");
```

### 5.3 小型应用数据存储

**场景**：为小型应用存储数据

**示例**：

```csharp
// 存储用户数据
string userDocument = '{"username": "admin", "email": "admin@example.com", "fullName": "管理员", "role": "admin", "lastLogin": "2026-01-22T10:00:00"}';
await liteDBService.InsertDocumentAsync("app.db", "users", userDocument);

// 存储产品数据
string productDocument = '{"name": "产品A", "price": 99.99, "description": "这是一个测试产品", "category": "电子产品", "stock": 100, "createdAt": "2026-01-22T09:00:00"}';
await liteDBService.InsertDocumentAsync("app.db", "products", productDocument);

// 存储订单数据
string orderDocument = '{"userId": "5f8d0d5a-1234-4567-89ab-cdef01234567", "items": [{"productId": "5f8d0d5a-1234-4567-89ab-cdef01234568", "quantity": 2, "price": 99.99}], "totalAmount": 199.98, "status": "pending", "createdAt": "2026-01-22T11:00:00"}';
await liteDBService.InsertDocumentAsync("app.db", "orders", orderDocument);

// 查询订单
string orders = await liteDBService.QueryDocumentsAsync("app.db", "orders");
Console.WriteLine("所有订单:");
Console.WriteLine(orders);
```

### 5.4 日志存储

**场景**：使用LiteDB存储应用程序日志

**示例**：

```csharp
// 存储日志条目
void Log(string level, string message, string category = "General")
{
    string logDocument = $'{"timestamp": "{DateTime.UtcNow}", "level": "{level}", "category": "{category}", "message": "{message}", "machine": "{Environment.MachineName}", "user": "{Environment.UserName}"}';
    liteDBService.InsertDocumentAsync("logs.db", "app_logs", logDocument).Wait();
}

// 记录不同级别的日志
Log("Information", "应用程序启动");
Log("Warning", "配置文件未找到，使用默认配置");
Log("Error", "数据库连接失败", "Database");
Log("Information", "用户登录成功", "Authentication");

// 查询日志
string errorLogs = await liteDBService.QueryDocumentsAsync("logs.db", "app_logs", "$.level = 'Error'");
Console.WriteLine("错误日志:");
Console.WriteLine(errorLogs);

// 查询特定类别的日志
string authLogs = await liteDBService.QueryDocumentsAsync("logs.db", "app_logs", "$.category = 'Authentication'");
Console.WriteLine("认证日志:");
Console.WriteLine(authLogs);

// 查询最近的日志
string recentLogs = await liteDBService.QueryDocumentsAsync("logs.db", "app_logs");
var logsArray = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement[]>(recentLogs);
var sortedLogs = logsArray.OrderByDescending(l => l.GetProperty("timestamp").GetString()).Take(10);
Console.WriteLine("最近10条日志:");
foreach (var log in sortedLogs)
{
    Console.WriteLine($"{log.GetProperty("timestamp").GetString()} [{log.GetProperty("level").GetString()}] {log.GetProperty("message").GetString()}");
}
```

## 6. 性能优化

### 6.1 缓存优化

**功能说明**：调整缓存设置以提高性能

**示例**：

```csharp
// 配置内存缓存
var cacheOptions = new MemoryCacheOptions
{
    SizeLimit = 1024 * 1024, // 1MB
    ExpirationScanFrequency = TimeSpan.FromMinutes(5)
};

// 注册缓存服务
services.AddSingleton<IMemoryCache>(new MemoryCache(cacheOptions));

// 添加LiteDB服务
services.AddSingleton<ILiteDBService, LiteDBService>();
```

### 6.2 异步操作优化

**功能说明**：优化异步操作，提高响应速度

**最佳实践**：

1. **使用 async/await**：所有数据库操作都使用异步执行
2. **避免阻塞调用**：不使用 Task.Wait() 或 Task.Result
3. **并行处理**：对于多个独立操作，使用 Task.WhenAll 并行执行
4. **批量操作**：对于大量数据，使用批量处理减少数据库连接开销

**示例**：

```csharp
// 并行插入多个文档
var tasks = new List<Task<string>>();
for (int i = 0; i < 100; i++)
{
    string doc = $'{"name": "Item{i}", "value": {i}}';
    tasks.Add(liteDBService.InsertDocumentAsync("data.db", "items", doc));
}

// 等待所有插入操作完成
await Task.WhenAll(tasks);
Console.WriteLine($"成功插入 {tasks.Count} 个文档");
```

### 6.3 数据库优化

**功能说明**：优化数据库设计和操作

**最佳实践**：

1. **合理设计集合**：根据数据类型和访问模式设计集合
2. **使用索引**：为频繁查询的字段添加索引
3. **定期压缩**：定期执行数据库压缩操作
4. **避免过度查询**：减少不必要的查询操作
5. **使用适当的查询条件**：使用精确的查询条件减少数据扫描

**示例**：

```csharp
// 定期压缩数据库
if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
{
    await liteDBService.CompactDatabaseAsync("data.db");
    Console.WriteLine("数据库已压缩");
}

// 优化查询
// 好的做法：使用精确的查询条件
string specificQuery = await liteDBService.QueryDocumentsAsync("data.db", "users", "$.email = 'zhangsan@example.com'");

// 避免：查询所有文档然后在内存中过滤
string allUsers = await liteDBService.QueryDocumentsAsync("data.db", "users");
var usersArray = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement[]>(allUsers);
var filteredUsers = usersArray.Where(u => u.GetProperty("email").GetString() == "zhangsan@example.com");
```

## 7. 错误处理

### 7.1 常见错误及解决方案

| 错误类型 | 错误信息 | 解决方案 |
|---------|---------|----------|
| FileNotFoundException | 数据库文件不存在 | 确保数据库路径正确，LiteDB会自动创建不存在的数据库文件 |
| IOException | 数据库文件被占用 | 确保没有其他进程正在使用该数据库文件 |
| LiteException | 文档格式无效 | 确保输入的JSON文档格式正确 |
| UnauthorizedAccessException | 权限不足 | 确保有足够的权限访问数据库文件和目录 |
| OutOfMemoryException | 内存不足 | 减少缓存大小，分批处理大量数据 |

### 7.2 错误处理示例

**功能说明**：实现健壮的错误处理策略

**示例代码**：

```csharp
try
{
    // 尝试插入文档
    string result = await liteDBService.InsertDocumentAsync("data.db", "users", "{invalid json}");
    Console.WriteLine($"文档插入成功，ID: {result}");
}
catch (Exception ex)
{
    Console.WriteLine($"插入文档失败: {ex.Message}");
    // 可以添加日志记录、报警等逻辑
}

// 更好的做法：验证输入
string document = GetUserInput();
if (IsValidJson(document))
{
    try
    {
        string result = await liteDBService.InsertDocumentAsync("data.db", "users", document);
        Console.WriteLine($"文档插入成功，ID: {result}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"插入文档失败: {ex.Message}");
    }
}
else
{
    Console.WriteLine("无效的JSON格式，请检查输入。");
}

// 验证JSON函数
bool IsValidJson(string json)
{
    try
    {
        System.Text.Json.JsonDocument.Parse(json);
        return true;
    }
    catch
    {
        return false;
    }
}
```

### 7.3 日志记录

**功能说明**：配置详细的日志记录，便于故障排查

**示例代码**：

```csharp
// 配置详细的日志记录
var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole(options =>
    {
        options.Format = ConsoleLoggerFormat.Systemd;
    });
    builder.AddFile("litedb.log", options =>
    {
        options.FileSizeLimit = 10 * 1024 * 1024; // 10MB
        options.MaxRollingFiles = 5;
        options.MinLevel = LogLevel.Trace;
    });
    builder.SetMinimumLevel(LogLevel.Debug);
});

services.AddSingleton(loggerFactory);
services.AddLogging();
services.AddMemoryCache();
services.AddSingleton<ILiteDBService, LiteDBService>();

// 使用服务
var liteDBService = serviceProvider.GetRequiredService<ILiteDBService>();
try
{
    await liteDBService.CreateCollectionAsync("data.db", "users");
    Console.WriteLine("集合创建成功");
}
catch (Exception ex)
{
    Console.WriteLine($"操作失败: {ex.Message}");
    // 详细错误信息已记录到日志文件
}
```

## 8. 总结

LiteDB AOT 是一个功能强大的嵌入式数据库工具，基于 .NET 10.0 的 AOT 编译架构，提供了全面的数据库操作功能，包括集合管理、文档 CRUD 操作、备份和压缩等。通过本文档提供的示例和最佳实践，您可以：

1. **快速上手**：使用命令行接口执行各种数据库操作
2. **高级使用**：通过编程方式创建复杂的数据库应用，支持条件查询、批量操作等高级功能
3. **性能优化**：利用缓存、异步操作和数据库优化提高性能
4. **错误处理**：实现健壮的错误处理策略，确保工具稳定运行
5. **集成应用**：将 LiteDB 集成到各种应用场景中，如配置存储、本地缓存、小型应用数据存储和日志存储等

LiteDB AOT 工具设计为跨平台、高性能和可扩展的，适用于各种 .NET 项目的本地数据存储场景。无论您是在命令行中使用，还是在代码中集成，LiteDB AOT 都能为您提供轻量级、高效的数据库功能支持。