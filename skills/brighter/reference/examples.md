# brighter - 使用示例

## 快速开始

### 1. 基本命令处理示例

```csharp
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

// 定义命令
public record CreateTaskCommand : Command
{
    public CreateTaskCommand(Guid id, string title, string description, DateTime dueDate)
    {
        Id = id;
        Title = title;
        Description = description;
        DueDate = dueDate;
        CommandId = Guid.NewGuid();
        Type = typeof(CreateTaskCommand).Name;
        Version = 1;
    }
    
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public DateTime DueDate { get; init; }
}

// 定义命令处理器
public class CreateTaskCommandHandler : RequestHandlerAsync<CreateTaskCommand>
{
    private readonly IHostEnvironment _environment;
    
    public CreateTaskCommandHandler(IHostEnvironment environment)
    {
        _environment = environment;
    }
    
    public override async Task<CreateTaskCommand> HandleAsync(CreateTaskCommand command)
    {
        // 模拟命令处理逻辑
        Console.WriteLine($"[{_environment.EnvironmentName}] 创建任务: {command.Title}");
        Console.WriteLine($"  描述: {command.Description}");
        Console.WriteLine($"  截止日期: {command.DueDate.ToShortDateString()}");
        
        // 模拟异步操作
        await Task.Delay(100);
        
        return await base.HandleAsync(command);
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Brighter 基本命令处理示例");
        Console.WriteLine("=" * 60);
        
        // 创建主机
        var builder = Host.CreateApplicationBuilder(args);
        
        // 注册 Brighter 服务
        builder.Services.AddBrighter(options =>
        {
            options.HandlerLifetime = ServiceLifetime.Scoped;
            options.CommandProcessorLifetime = ServiceLifetime.Scoped;
        });
        
        // 注册命令处理器
        builder.Services.AddScoped<CreateTaskCommandHandler>();
        
        var host = builder.Build();
        
        // 获取命令处理器
        var commandProcessor = host.Services.GetRequiredService<IAmACommandProcessor>();
        
        // 发送命令
        var command = new CreateTaskCommand(
            Guid.NewGuid(),
            "完成 Brighter 文档",
            "更新 Brighter 技能的中文文档和示例",
            DateTime.UtcNow.AddDays(3));
        
        await commandProcessor.SendAsync(command);
        
        Console.WriteLine("\n命令处理完成!");
        Console.WriteLine("=" * 60);
    }
}
```

### 2. AOT 优化的命令处理示例

```csharp
// AOT 优化的命令处理示例
using System.Runtime.CompilerServices;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// 高性能命令
public record ProcessDataCommand : Command
{
    public ProcessDataCommand(Guid id, byte[] data)
    {
        Id = id;
        Data = data;
        CommandId = Guid.NewGuid();
        Type = typeof(ProcessDataCommand).Name;
        Version = 1;
    }
    
    public Guid Id { get; init; }
    public byte[] Data { get; init; }
}

// AOT 优化的命令处理器
[SkipLocalsInit]
public class ProcessDataCommandHandler : RequestHandlerAsync<ProcessDataCommand>
{
    // 使用 AggressiveOptimization 提示编译器优化
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override async Task<ProcessDataCommand> HandleAsync(ProcessDataCommand command)
    {
        // 高性能数据处理
        var buffer = command.Data;
        int sum = 0;
        
        // 使用 Span 进行零拷贝数据处理
        var span = new Span<byte>(buffer);
        for (int i = 0; i < span.Length; i++)
        {
            sum += span[i];
        }
        
        // 模拟异步操作
        await Task.Delay(50);
        
        Console.WriteLine($"处理数据: 长度={buffer.Length}, 校验和={sum}");
        
        return await base.HandleAsync(command);
    }
}

// AOT 优化的主程序
public class Program
{
    // 使用 AggressiveOptimization 提示编译器优化
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Brighter AOT 优化的命令处理示例");
        Console.WriteLine("=" * 60);
        
        // 创建主机
        var builder = Host.CreateApplicationBuilder(args);
        
        // 注册 Brighter 服务
        builder.Services.AddBrighter(options =>
        {
            options.HandlerLifetime = ServiceLifetime.Transient;
            options.CommandProcessorLifetime = ServiceLifetime.Scoped;
        });
        
        // 注册命令处理器
        builder.Services.AddTransient<ProcessDataCommandHandler>();
        
        var host = builder.Build();
        
        // 获取命令处理器
        var commandProcessor = host.Services.GetRequiredService<IAmACommandProcessor>();
        
        // 生成测试数据
        var random = new Random();
        var data = new byte[1024 * 1024]; // 1MB 测试数据
        random.NextBytes(data);
        
        // 发送命令
        var command = new ProcessDataCommand(Guid.NewGuid(), data);
        
        // 测量执行时间
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await commandProcessor.SendAsync(command);
        stopwatch.Stop();
        
        Console.WriteLine($"\n数据处理完成!");
        Console.WriteLine($"执行时间: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
        Console.WriteLine($"处理速度: {(data.Length / 1024.0 / stopwatch.Elapsed.TotalSeconds):F2} KB/s");
        Console.WriteLine("=" * 60);
    }
}
```

### 3. 查询处理示例

```csharp
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// 定义查询
public record GetTasksQuery : Query<GetTasksQueryResult>
{
    public GetTasksQuery(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
        QueryId = Guid.NewGuid();
        Type = typeof(GetTasksQuery).Name;
        Version = 1;
    }
    
    public int Page { get; init; }
    public int PageSize { get; init; }
}

// 定义查询结果
public record GetTasksQueryResult(List<TaskDto> Tasks, int TotalCount, int Page, int PageSize);

// 定义任务数据传输对象
public record TaskDto(Guid Id, string Title, string Description, DateTime DueDate, bool IsCompleted);

// 定义查询处理器
public class GetTasksQueryHandler : RequestHandlerAsync<GetTasksQuery, GetTasksQueryResult>
{
    public override async Task<GetTasksQueryResult> HandleAsync(GetTasksQuery query)
    {
        // 模拟查询数据
        var tasks = new List<TaskDto>();
        
        // 生成模拟数据
        for (int i = 0; i < query.PageSize; i++)
        {
            var id = Guid.NewGuid();
            tasks.Add(new TaskDto(
                id,
                $"任务 {((query.Page - 1) * query.PageSize) + i + 1}",
                $"这是任务 {(query.Page - 1) * query.PageSize + i + 1} 的描述",
                DateTime.UtcNow.AddDays(i + 1),
                i % 3 == 0 // 每3个任务中有1个已完成
            ));
        }
        
        // 模拟异步操作
        await Task.Delay(150);
        
        return new GetTasksQueryResult(tasks, 100, query.Page, query.PageSize);
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Brighter 查询处理示例");
        Console.WriteLine("=" * 60);
        
        // 创建主机
        var builder = Host.CreateApplicationBuilder(args);
        
        // 注册 Brighter 服务
        builder.Services.AddBrighter(options =>
        {
            options.HandlerLifetime = ServiceLifetime.Scoped;
            options.QueryProcessorLifetime = ServiceLifetime.Scoped;
        });
        
        // 注册查询处理器
        builder.Services.AddScoped<GetTasksQueryHandler>();
        
        var host = builder.Build();
        
        // 获取查询处理器
        var queryProcessor = host.Services.GetRequiredService<IAmAQueryProcessor>();
        
        // 发送查询
        var query = new GetTasksQuery(page: 1, pageSize: 5);
        var result = await queryProcessor.SendAsync(query);
        
        Console.WriteLine($"查询结果: 共 {result.TotalCount} 个任务");
        Console.WriteLine($"第 {result.Page} 页, 每页 {result.PageSize} 个");
        Console.WriteLine();
        
        foreach (var task in result.Tasks)
        {
            var status = task.IsCompleted ? "已完成" : "未完成";
            Console.WriteLine($"{task.Id}: {task.Title} - {status}");
            Console.WriteLine($"  描述: {task.Description}");
            Console.WriteLine($"  截止日期: {task.DueDate.ToShortDateString()}");
            Console.WriteLine();
        }
        
        Console.WriteLine("=" * 60);
    }
}
```

### 4. 与 ASP.NET Core 集成示例

```csharp
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// 定义命令
public record CreateUserCommand : Command
{
    public CreateUserCommand(Guid id, string name, string email, string password)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
        CommandId = Guid.NewGuid();
        Type = typeof(CreateUserCommand).Name;
        Version = 1;
    }
    
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
}

// 定义命令处理器
public class CreateUserCommandHandler : RequestHandlerAsync<CreateUserCommand>
{
    public override async Task<CreateUserCommand> HandleAsync(CreateUserCommand command)
    {
        // 模拟用户创建逻辑
        Console.WriteLine($"创建用户: {command.Name}");
        Console.WriteLine($"  邮箱: {command.Email}");
        
        // 模拟异步操作
        await Task.Delay(200);
        
        return await base.HandleAsync(command);
    }
}

// API 控制器
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IAmACommandProcessor _commandProcessor;
    
    public UsersController(IAmACommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(
            Guid.NewGuid(),
            request.Name,
            request.Email,
            request.Password
        );
        
        await _commandProcessor.SendAsync(command);
        
        return CreatedAtAction(nameof(Get), new { id = command.Id }, new { id = command.Id, name = command.Name, email = command.Email });
    }
    
    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        // 模拟获取用户
        return Ok(new { id, name = "测试用户", email = "test@example.com" });
    }
}

// 请求模型
public class CreateUserRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

// 主程序
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Brighter 与 ASP.NET Core 集成示例");
        Console.WriteLine("=" * 60);
        
        // 创建 Web 应用
        var builder = WebApplication.CreateBuilder(args);
        
        // 注册 Brighter 服务
        builder.Services.AddBrighter(options =>
        {
            options.HandlerLifetime = ServiceLifetime.Scoped;
            options.CommandProcessorLifetime = ServiceLifetime.Scoped;
        });
        
        // 注册命令处理器
        builder.Services.AddScoped<CreateUserCommandHandler>();
        
        // 注册 API 服务
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        var app = builder.Build();
        
        // 配置中间件
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        // 运行应用
        app.Run();
    }
}
```

## 总结

以上示例展示了 Brighter 技能的主要功能和使用方法。通过这些示例，您可以：

1. **快速开始使用 Brighter**: 实现基本的命令处理
2. **使用 AOT 优化**: 实现高性能的命令处理，支持 AOT 编译
3. **实现查询处理**: 实现 CQRS 架构中的查询处理
4. **与 ASP.NET Core 集成**: 在 Web API 中使用 Brighter

所有示例都基于 .NET 10，支持 AOT 编译优化，遵循 .NET 最佳实践。您可以根据业务需求选择合适的示例，并根据需要进行扩展和定制。

Brighter 技能提供了完整的 CQRS 解决方案，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的 .NET 项目。
