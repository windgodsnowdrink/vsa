#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.AspNetCore.Http.Abstractions@10.0.0
#:package Microsoft.AspNetCore.Routing@10.0.0
#:package Microsoft.AspNetCore.OpenApi@10.0.0
#:package Swashbuckle.AspNetCore@6.4.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property IncludeNativeLibrariesForSelfExtract=true
#:property EnableCppCodeGen=true
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
#:property RuntimeIdentifier=linux-x64
#:property RuntimeIdentifier=osx-x64

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.OpenApi.Models;

// MiniAPI 应用程序
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // 配置日志
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options =>
        {
            options.FormatterName = ConsoleFormatterNames.Json;
        });
        
        // 添加服务
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "MiniAPI Demo",
                Description = "A simple MiniAPI demo built with .NET 10"
            });
        });
        
        // 添加依赖注入服务
        builder.Services.AddScoped<TodoService>();
        builder.Services.AddScoped<WeatherForecastService>();
        
        // 添加健康检查
        builder.Services.AddHealthChecks()
            .AddCheck<MemoryHealthCheck>("memory")
            .AddCheck<DiskHealthCheck>("disk");
        
        var app = builder.Build();
        
        // 配置中间件
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        
        // 注册 MiniAPI 端点
        RegisterEndpoints(app);
        
        app.Run();
    }
    
    private static void RegisterEndpoints(WebApplication app)
    {
        // 健康检查端点
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
        
        // 根端点
        app.MapGet("/", () => "Hello MiniAPI!")
            .WithName("GetRoot")
            .WithOpenApi();
        
        // Todo API 端点
        var todoGroup = app.MapGroup("/api/todos")
            .WithTags("Todos")
            .WithOpenApi();
        
        todoGroup.MapGet("", async (TodoService service) =>
        {
            var todos = await service.GetAllAsync();
            return Results.Ok(todos);
        })
        .WithName("GetAllTodos");
        
        todoGroup.MapGet("/{id}", async (int id, TodoService service) =>
        {
            var todo = await service.GetByIdAsync(id);
            return todo is not null ? Results.Ok(todo) : Results.NotFound();
        })
        .WithName("GetTodoById");
        
        todoGroup.MapPost("", async (TodoDto dto, TodoService service) =>
        {
            var createdTodo = await service.CreateAsync(dto);
            return Results.Created($"/api/todos/{createdTodo.Id}", createdTodo);
        })
        .WithName("CreateTodo");
        
        todoGroup.MapPut("/{id}", async (int id, TodoDto dto, TodoService service) =>
        {
            var updatedTodo = await service.UpdateAsync(id, dto);
            return updatedTodo is not null ? Results.Ok(updatedTodo) : Results.NotFound();
        })
        .WithName("UpdateTodo");
        
        todoGroup.MapDelete("/{id}", async (int id, TodoService service) =>
        {
            var success = await service.DeleteAsync(id);
            return success ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteTodo");
        
        // 天气预测端点
        app.MapGet("/api/weatherforecast", async (WeatherForecastService service) =>
        {
            var forecasts = await service.GetForecastAsync();
            return Results.Ok(forecasts);
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi();
        
        // 性能优化示例端点
        app.MapGet("/api/performance/memory", () =>
        {
            // 使用内存池减少内存分配
            using var memory = MemoryPool<byte>.Shared.Rent(1024 * 1024); // 1MB
            var buffer = memory.Memory;
            
            // 使用 Span<byte> 进行零拷贝操作
            Span<byte> span = buffer.Span;
            // 填充数据
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = (byte)(i % 255);
            }
            
            return Results.Ok(new { Message = "Memory optimized operation completed", BytesProcessed = span.Length });
        })
        .WithName("MemoryOptimization")
        .WithOpenApi();
        
        // 并发示例端点
        var cache = new ConcurrentDictionary<int, string>();
        
        app.MapGet("/api/performance/concurrent", async (int id) =>
        {
            // 使用 ConcurrentDictionary 进行线程安全操作
            var result = await Task.Run(() =>
            {
                return cache.GetOrAdd(id, key =>
                {
                    // 模拟耗时操作
                    Task.Delay(100).Wait();
                    return $"Cached value for {key} generated at {DateTime.Now}";
                });
            });

            return Results.Ok(new { Value = result, CacheSize = cache.Count });
        })
        .WithName("ConcurrentOperation")
        .WithOpenApi();
    }
}

// Todo 数据模型
public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Completed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// Todo DTO
public class TodoDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Completed { get; set; }
}

// Todo 服务
public class TodoService
{
    private static readonly List<Todo> _todos = new List<Todo>
    {
        new Todo { Id = 1, Title = "学习 MiniAPI", Description = "了解 .NET 10 MiniAPI 特性", Completed = false },
        new Todo { Id = 2, Title = "构建 API", Description = "使用 MiniAPI 构建 RESTful API", Completed = false },
        new Todo { Id = 3, Title = "性能优化", Description = "优化 MiniAPI 性能", Completed = false }
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

    public Task<Todo> CreateAsync(TodoDto dto)
    {
        var todo = new Todo
        {
            Id = _todos.Max(t => t.Id) + 1,
            Title = dto.Title,
            Description = dto.Description,
            Completed = dto.Completed
        };
        
        _todos.Add(todo);
        return Task.FromResult(todo);
    }

    public Task<Todo> UpdateAsync(int id, TodoDto dto)
    {
        var existingTodo = _todos.FirstOrDefault(t => t.Id == id);
        if (existingTodo is null)
        {
            return Task.FromResult<Todo>(null);
        }

        existingTodo.Title = dto.Title;
        existingTodo.Description = dto.Description;
        existingTodo.Completed = dto.Completed;

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

// MiniAPI 扩展方法
public static class MiniApiExtensions
{
    /// <summary>
    /// 映射 CRUD 端点
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TDto">DTO 类型</typeparam>
    /// <param name="app">Web 应用程序</param>
    /// <param name="routePrefix">路由前缀</param>
    /// <param name="tags">OpenAPI 标签</param>
    public static void MapCrudEndpoints<TService, TEntity, TDto>(
        this WebApplication app,
        string routePrefix,
        string tags)
        where TService : class
        where TEntity : class
        where TDto : class
    {
        var group = app.MapGroup($"/api/{routePrefix}")
            .WithTags(tags)
            .WithOpenApi();
        
        // 这里可以根据实际需要实现通用的 CRUD 端点
        // 例如：GetAll, GetById, Create, Update, Delete
    }
}
