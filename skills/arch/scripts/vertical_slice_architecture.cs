#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.App.Ref@10.0.0-preview.4
#:package Microsoft.EntityFrameworkCore.SqlServer@10.0.0-preview.4
#:package Microsoft.AspNetCore.Mvc.NewtonsoftJson@10.0.0-preview.4
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property UserSecretsId=210f4926-30c7-45ca-a020-391f82b3b3a1
#:property DockerDefaultTargetOS=Linux
#:property DockerComposeProjectPath=..\docker-compose.dcproj

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VerticalSliceArchitecture
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
    /// 消息总线接口
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="event">事件</param>
        /// <returns>任务</returns>
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;
    }

    /// <summary>
    /// 待办事项创建事件
    /// </summary>
    public class TodoItemCreatedEvent
    {
        /// <summary>
        /// 待办事项ID
        /// </summary>
        public int TodoId { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="todoId">待办事项ID</param>
        public TodoItemCreatedEvent(int todoId)
        {
            TodoId = todoId;
        }
    }

    /// <summary>
    /// 待办事项更新事件
    /// </summary>
    public class TodoItemUpdatedEvent
    {
        /// <summary>
        /// 待办事项ID
        /// </summary>
        public int TodoId { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="todoId">待办事项ID</param>
        public TodoItemUpdatedEvent(int todoId)
        {
            TodoId = todoId;
        }
    }

    /// <summary>
    /// 待办事项删除事件
    /// </summary>
    public class TodoItemDeletedEvent
    {
        /// <summary>
        /// 待办事项ID
        /// </summary>
        public int TodoId { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="todoId">待办事项ID</param>
        public TodoItemDeletedEvent(int todoId)
        {
            TodoId = todoId;
        }
    }

    /// <summary>
    /// 垂直切片架构扩展
    /// </summary>
    public static class TodoEndpoints
    {
        /// <summary>
        /// 映射待办事项端点
        /// </summary>
        /// <param name="app">Web应用</param>
        public static void MapTodoEndpoints(this WebApplication app)
        {
            // 切片：获取所有待办事项
            app.MapGet("/todos", async (TodoDbContext db) =>
            {
                return await db.TodoItems.ToListAsync();
            })
            .Produces<List<TodoItem>>(StatusCodes.Status200OK)
            .WithName("GetAllTodos")
            .WithOpenApi();

            // 切片：创建新的待办事项
            app.MapPost("/todos", async (TodoItem todo, TodoDbContext db, IMessageBus bus)
            {
                db.TodoItems.Add(todo);
                await db.SaveChangesAsync();

                // 发布领域事件
                await bus.PublishAsync(new TodoItemCreatedEvent(todo.Id));

                return Results.Created($"/todos/{todo.Id}", todo);
            })
            .Accepts<TodoItem>("application/json")
            .Produces<TodoItem>(StatusCodes.Status201Created)
            .WithName("CreateTodo")
            .WithOpenApi();

            // 切片：获取单个待办事项
            app.MapGet("/todos/{id}", async (int id, TodoDbContext db)
            {
                return await db.TodoItems.FindAsync(id) is TodoItem todo ? Results.Ok(todo) : Results.NotFound();
            })
            .Produces<TodoItem>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetTodo")
            .WithOpenApi();

            // 切片：更新待办事项
            app.MapPut("/todos/{id}", async (int id, TodoItem inputTodo, TodoDbContext db, IMessageBus bus)
            {
                var todo = await db.TodoItems.FindAsync(id);
                if (todo is null)
                {
                    return Results.NotFound();
                }

                todo.Title = inputTodo.Title;
                todo.IsCompleted = inputTodo.IsCompleted;

                await db.SaveChangesAsync();

                // 发布领域事件
                await bus.PublishAsync(new TodoItemUpdatedEvent(todo.Id));

                return Results.NoContent();
            })
            .Accepts<TodoItem>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("UpdateTodo")
            .WithOpenApi();

            // 切片：删除待办事项
            app.MapDelete("/todos/{id}", async (int id, TodoDbContext db, IMessageBus bus)
            {
                if (await db.TodoItems.FindAsync(id) is TodoItem todo)
                {
                    db.TodoItems.Remove(todo);
                    await db.SaveChangesAsync();

                    // 发布领域事件
                    await bus.PublishAsync(new TodoItemDeletedEvent(todo.Id));

                    return Results.NoContent();
                }

                return Results.NotFound();
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteTodo")
            .WithOpenApi();
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

            // 配置数据库
            builder.Services.AddDbContext<TodoDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 注册消息总线
            builder.Services.AddSingleton<IMessageBus, MessageBus>();

            var app = builder.Build();

            // 映射待办事项端点
            app.MapTodoEndpoints();

            app.Run();
        }
    }

    /// <summary>
    /// 消息总线实现
    /// </summary>
    public class MessageBus : IMessageBus
    {
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="event">事件</param>
        /// <returns>任务</returns>
        public Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
        {
            // 实现消息发布逻辑
            // 这里可以集成RabbitMQ、Kafka等消息队列
            Console.WriteLine($"Published event: {typeof(TEvent).Name}");
            return Task.CompletedTask;
        }
    }
}