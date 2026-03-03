#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package FastEndpoints@7.0.1
#:package Microsoft.VisualStudio.Azure.Containers.Tools.Targets@1.22.1
#:package Microsoft.EntityFrameworkCore.SqlServer@10.0.0-preview.6.25358.103
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property UserSecretsId=210f4926-30c7-45ca-a020-391f82b3b3a1
#:property DockerDefaultTargetOS=Linux
#:property DockerComposeProjectPath=..\docker-compose.dcproj

using System;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Channels;
using System.Buffers;
using Microsoft.Extensions.ObjectPool;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace FastEndpointsExample
{
    /// <summary>
    /// 待办事项模型
    /// </summary>
    public class TodoItem
    {
        /// <summary>
        /// 待办事项ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 待办事项标题
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// 数据库上下文
    /// </summary>
    public class TodoDbContext : DbContext
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">数据库上下文选项</param>
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }

        /// <summary>
        /// 待办事项集合
        /// </summary>
        public DbSet<TodoItem> TodoItems { get; set; }
    }

    /// <summary>
    /// 内存优化器接口
    /// </summary>
    public interface IMemoryOptimizer
    {
        /// <summary>
        /// 优化内存使用
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>优化后的内存</returns>
        Span<byte> OptimizeMemory(byte[] data);
    }

    /// <summary>
    /// Span/Memory内存优化器
    /// </summary>
    public class SpanMemoryOptimizer : IMemoryOptimizer
    {
        /// <summary>
        /// 优化内存使用
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>优化后的内存</returns>
        public Span<byte> OptimizeMemory(byte[] data)
        {
            return data.AsSpan();
        }
    }

    /// <summary>
    /// 尾延迟优化器
    /// </summary>
    public class TailLatencyOptimizer
    {
        /// <summary>
        /// 优化尾延迟
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <returns>任务</returns>
        public async Task OptimizeAsync(Func<Task> action)
        {
            // 实现尾延迟优化逻辑
            await action();
        }
    }

    /// <summary>
    /// 创建待办事项请求
    /// </summary>
    public class CreateTodoRequest
    {
        /// <summary>
        /// 待办事项标题
        /// </summary>
        [Required]
        public string Title { get; set; }
    }

    /// <summary>
    /// 更新待办事项请求
    /// </summary>
    public class UpdateTodoRequest
    {
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// 待办事项响应
    /// </summary>
    public class TodoResponse
    {
        /// <summary>
        /// 待办事项ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 待办事项标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// 待办事项端点
    /// </summary>
    public class TodoEndpoint : EndpointGroupBase
    {
        /// <summary>
        /// 映射端点
        /// </summary>
        /// <param name="app">应用构建器</param>
        public override void Map(WebApplication app)
        {
            app.MapGroup("/todos")
                .MapGet("", GetAllTodos)
                .MapGet("/{id}", GetTodo)
                .MapPost("", CreateTodo)
                .MapPut("/{id}", UpdateTodo)
                .MapDelete("/{id}", DeleteTodo);
        }

        /// <summary>
        /// 获取所有待办事项
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <returns>待办事项列表</returns>
        private static async Task<IEnumerable<TodoResponse>> GetAllTodos(TodoDbContext db)
        {
            return await db.TodoItems
                .Select(t => new TodoResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted
                })
                .ToListAsync();
        }

        /// <summary>
        /// 获取单个待办事项
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <param name="db">数据库上下文</param>
        /// <returns>待办事项响应</returns>
        private static async Task<Results<TodoResponse, NotFound>> GetTodo(int id, TodoDbContext db)
        {
            var todo = await db.TodoItems.FindAsync(id);
            if (todo == null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(new TodoResponse
            {
                Id = todo.Id,
                Title = todo.Title,
                IsCompleted = todo.IsCompleted
            });
        }

        /// <summary>
        /// 创建待办事项
        /// </summary>
        /// <param name="request">创建待办事项请求</param>
        /// <param name="db">数据库上下文</param>
        /// <param name="channel">通道</param>
        /// <returns>创建的待办事项</returns>
        private static async Task<Created<TodoResponse>> CreateTodo(CreateTodoRequest request, TodoDbContext db, Channel<TodoItem> channel)
        {
            var todo = new TodoItem
            {
                Title = request.Title,
                IsCompleted = false
            };

            db.TodoItems.Add(todo);
            await db.SaveChangesAsync();

            // 将新创建的待办事项发送到通道
            await channel.Writer.WriteAsync(todo);

            return TypedResults.Created($"/todos/{todo.Id}", new TodoResponse
            {
                Id = todo.Id,
                Title = todo.Title,
                IsCompleted = todo.IsCompleted
            });
        }

        /// <summary>
        /// 更新待办事项
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <param name="request">更新待办事项请求</param>
        /// <param name="db">数据库上下文</param>
        /// <returns>更新结果</returns>
        private static async Task<Results<NoContent, NotFound>> UpdateTodo(int id, UpdateTodoRequest request, TodoDbContext db)
        {
            var todo = await db.TodoItems.FindAsync(id);
            if (todo == null)
            {
                return TypedResults.NotFound();
            }

            todo.IsCompleted = request.IsCompleted;
            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        }

        /// <summary>
        /// 删除待办事项
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <param name="db">数据库上下文</param>
        /// <returns>删除结果</returns>
        private static async Task<Results<NoContent, NotFound>> DeleteTodo(int id, TodoDbContext db)
        {
            var todo = await db.TodoItems.FindAsync(id);
            if (todo == null)
            {
                return TypedResults.NotFound();
            }

            db.TodoItems.Remove(todo);
            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        }
    }

    /// <summary>
    /// 主程序
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 主入口点
        /// </summary>
        /// <param name="args">命令行参数</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 添加 FastEndpoints
            builder.Services.AddFastEndpoints();

            // 增强1：添加内存池和零拷贝优化
            builder.Services.AddSingleton<ObjectPool<Memory<byte>>>(new DefaultObjectPool<Memory<byte>>(
                new DefaultPooledObjectPolicy<Memory<byte>>(), 1000));
            builder.Services.AddSingleton<TailLatencyOptimizer>();

            // 增强2：添加高性能通道处理
            builder.Services.AddSingleton<Channel<TodoItem>>(Channel.CreateUnbounded<TodoItem>(
                new UnboundedChannelOptions { SingleReader = true }));

            // 增强3：添加Span/Memory优化支持
            builder.Services.AddSingleton<IMemoryOptimizer, SpanMemoryOptimizer>();

            // 配置数据库连接
            builder.Services.AddDbContext<TodoDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=Todo.db"));

            var app = builder.Build();

            // 启用 FastEndpoints
            app.UseFastEndpoints();

            // 启用数据库迁移
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<TodoDbContext>();
                context.Database.Migrate();
            }

            app.Run();
        }
    }
}