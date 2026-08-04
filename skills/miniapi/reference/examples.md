# MiniAPI - 使用示例

## 快速开始

### 1. 基本使用示例

```csharp
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.MapGet("/", () => "Hello World!");

        app.Run();
    }
}
```

### 2. Todo API 示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // 添加依赖注入服务
        builder.Services.AddScoped<ITodoService, TodoService>();
        
        var app = builder.Build();

        // 定义 API 端点
        app.MapGet("/todos", async (ITodoService todoService) =>
        {
            var todos = await todoService.GetAllAsync();
            return Results.Ok(todos);
        });

        app.MapGet("/todos/{id}", async (int id, ITodoService todoService) =>
        {
            var todo = await todoService.GetByIdAsync(id);
            return todo is not null ? Results.Ok(todo) : Results.NotFound();
        });

        app.MapPost("/todos", async (Todo todo, ITodoService todoService) =>
        {
            var createdTodo = await todoService.CreateAsync(todo);
            return Results.Created($"/todos/{createdTodo.Id}", createdTodo);
        });

        app.MapPut("/todos/{id}", async (int id, Todo todo, ITodoService todoService) =>
        {
            var updatedTodo = await todoService.UpdateAsync(id, todo);
            return updatedTodo is not null ? Results.Ok(updatedTodo) : Results.NotFound();
        });

        app.MapDelete("/todos/{id}", async (int id, ITodoService todoService) =>
        {
            var success = await todoService.DeleteAsync(id);
            return success ? Results.NoContent() : Results.NotFound();
        });

        app.Run();
    }
}

// 数据模型
public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Completed { get; set; }
}

// 服务接口
public interface ITodoService
{
    Task<IEnumerable<Todo>> GetAllAsync();
    Task<Todo> GetByIdAsync(int id);
    Task<Todo> CreateAsync(Todo todo);
    Task<Todo> UpdateAsync(int id, Todo todo);
    Task<bool> DeleteAsync(int id);
}

// 服务实现
public class TodoService : ITodoService
{
    private static List<Todo> _todos = new List<Todo>
    {
        new Todo { Id = 1, Title = "学习 MiniAPI", Description = "了解 .NET 10 MiniAPI 特性", Completed = false },
        new Todo { Id = 2, Title = "构建 API", Description = "使用 MiniAPI 构建 RESTful API", Completed = false }
    };

    public Task<IEnumerable<Todo>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Todo>>(_todos);
    }

    public Task<Todo> GetByIdAsync(int id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(todo);
    }

    public Task<Todo> CreateAsync(Todo todo)
    {
        todo.Id = _todos.Max(t => t.Id) + 1;
        _todos.Add(todo);
        return Task.FromResult(todo);
    }

    public Task<Todo> UpdateAsync(int id, Todo todo)
    {
        var existingTodo = _todos.FirstOrDefault(t => t.Id == id);
        if (existingTodo is null)
        {
            return Task.FromResult<Todo>(null);
        }

        existingTodo.Title = todo.Title;
        existingTodo.Description = todo.Description;
        existingTodo.Completed = todo.Completed;

        return Task.FromResult(existingTodo);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        if (todo is null)
        {
            return Task.FromResult(false);
        }

        _todos.Remove(todo);
        return Task.FromResult(true);
    }
}
```

### 3. 高级配置示例

```csharp
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // 配置应用设置
        builder.Configuration
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
        
        // 添加服务
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
        
        // 添加自定义服务
        builder.Services.AddScoped<WeatherForecastService>();
        
        var app = builder.Build();
        
        // 配置中间件
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        
        // 定义 API 端点
        app.MapGet("/weatherforecast", async (WeatherForecastService service) =>
        {
            var forecasts = await service.GetForecastAsync();
            return Results.Ok(forecasts);
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi();
        
        app.Run();
    }
}

// 天气预测服务
public class WeatherForecastService
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public Task<IEnumerable<WeatherForecast>> GetForecastAsync()
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        
        return Task.FromResult<IEnumerable<WeatherForecast>>(forecast);
    }
}

// 天气预测模型
public class WeatherForecast
{
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string Summary { get; set; }
}
```

### 4. 中间件示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        // 使用自定义中间件
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // 定义 API 端点
        app.MapGet("/", () => "Hello World!");
        app.MapGet("/error", () => throw new Exception("Test error"));

        app.Run();
    }
}

// 请求日志中间件
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 处理请求前
        Console.WriteLine($"[{DateTime.Now}] Request: {context.Request.Method} {context.Request.Path}");

        // 调用下一个中间件
        await _next(context);

        // 处理响应后
        Console.WriteLine($"[{DateTime.Now}] Response: {context.Response.StatusCode}");
    }
}

// 异常处理中间件
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // 处理异常
            Console.WriteLine($"Error: {ex.Message}");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("An internal server error occurred.");
        }
    }
}
```

### 5. 健康检查示例

```csharp
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // 添加健康检查
        builder.Services.AddHealthChecks()
            .AddCheck<MemoryHealthCheck>("memory")
            .AddCheck<DiskHealthCheck>("disk");
        
        var app = builder.Build();
        
        // 配置健康检查端点
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/health/detail", new HealthCheckOptions
        {
            AllowCachingResponses = false,
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description,
                        data = e.Value.Data
                    })
                });
            }
        });
        
        app.MapGet("/", () => "Hello World!");
        
        app.Run();
    }
}

// 内存健康检查
public class MemoryHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var memoryInfo = GC.GetTotalMemory(false);
        var memoryThreshold = 1024 * 1024 * 1024; // 1GB
        
        if (memoryInfo < memoryThreshold)
        {
            return Task.FromResult(HealthCheckResult.Healthy($"Memory usage is normal: {memoryInfo / (1024 * 1024)} MB"));
        }
        
        return Task.FromResult(HealthCheckResult.Degraded($"Memory usage is high: {memoryInfo / (1024 * 1024)} MB"));
    }
}

// 磁盘健康检查
public class DiskHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var driveInfo = DriveInfo.GetDrives()[0];
        var freeSpace = driveInfo.AvailableFreeSpace;
        var totalSpace = driveInfo.TotalSize;
        var freePercentage = (double)freeSpace / totalSpace * 100;
        
        if (freePercentage > 10)
        {
            return Task.FromResult(HealthCheckResult.Healthy($"Disk space is normal: {freePercentage:F1}% free"));
        }
        
        return Task.FromResult(HealthCheckResult.Degraded($"Disk space is low: {freePercentage:F1}% free"));
    }
}
```

## 性能优化示例

### 1. 内存优化示例

```csharp
using System;
using System.Buffers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.MapGet("/memory-optimized", () =>
        {
            // 使用内存池减少内存分配
            using var memory = MemoryPool<byte>.Shared.Rent(1024 * 1024); // 1MB
            var buffer = memory.Memory;
            
            // 使用 Span<byte> 进行零拷贝操作
            Span<byte> span = buffer.Span;
            // 填充数据...
            
            return Results.Ok(new { Message = "Memory optimized operation completed" });
        });

        app.Run();
    }
}
```

### 2. 并发优化示例

```csharp
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

public class Program
{
    private static readonly ConcurrentDictionary<int, string> _cache = new ConcurrentDictionary<int, string>();

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.MapGet("/concurrent", async (int id) =>
        {
            // 使用 ConcurrentDictionary 进行线程安全操作
            var result = await Task.Run(() =>
            {
                return _cache.GetOrAdd(id, key =>
                {
                    // 模拟耗时操作
                    Task.Delay(100).Wait();
                    return $"Cached value for {key}";
                });
            });

            return Results.Ok(new { Value = result });
        });

        app.Run();
    }
}
```

## 总结

以上示例展示了 MiniAPI 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速开始基本操作
2. 构建完整的 RESTful API
3. 配置高级应用设置
4. 实现自定义中间件
5. 添加健康检查
6. 优化内存使用
7. 实现并发操作

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。