#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:package Microsoft.Extensions.Configuration@9.0.0
#:package Microsoft.Extensions.Configuration.Json@9.0.0
#:package Microsoft.Extensions.Logging@9.0.0
#:package Wolverine@1.0.0
#:package Wolverine.RabbitMQ@1.0.0
#:package Marten@7.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.ErrorHandling;
using Wolverine.Marten;
using Wolverine.RabbitMQ;

namespace WolverineSkill
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Wolverine 技能核心功能演示");
            Console.WriteLine("==============================");

            // 构建主机
            var host = CreateHostBuilder(args).Build();

            // 启动主机
            await host.StartAsync();

            try
            {
                // 获取 Wolverine 消息总线
                var messageBus = host.Services.GetRequiredService<IMessageBus>();

                // 演示命令处理
                await DemoCommandProcessing(messageBus);

                // 演示事件处理
                await DemoEventProcessing(messageBus);

                // 演示消息持久化
                await DemoMessagePersistence(messageBus);

                // 演示重试机制
                await DemoRetryPolicy(messageBus);

                // 演示异常处理
                await DemoExceptionHandling(messageBus);

                // 演示性能测试
                await DemoPerformanceTesting(messageBus);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"演示过程中发生异常: {ex.Message}");
            }
            finally
            {
                // 停止主机
                await host.StopAsync();
            }

            Console.WriteLine("\nWolverine 技能核心功能演示完成");
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // 配置 Wolverine
                    services.AddWolverine(options =>
                    {
                        // 配置消息存储（使用内存存储，生产环境可配置为 Marten 或其他持久化存储）
                        options.PersistMessagesWith<InMemoryMessageStore>();

                        // 配置重试策略
                        options.DefaultErrorHandlingPolicy = PolicyChain
                            .Handle<InvalidOperationException>()
                            .RetryWithBackoff(3, 500, 2000);

                        // 配置命令和事件处理
                        options.Discovery.IncludeAssembly(typeof(Program).Assembly);

                        // 配置性能优化
                        options.Advanced.ScheduledMessagePollingTime = TimeSpan.FromSeconds(1);
                        options.Advanced.MaximumEnvelopeRetryStorage = 1000;
                    });

                    // 可选：配置 Marten 作为事件存储
                    // services.AddMarten(options =>
                    // {
                    //     options.Connection("connection string");
                    //     options.Schema.For<OrderCreatedEvent>();
                    //     options.Schema.For<OrderUpdatedEvent>();
                    // });

                    // 可选：配置 RabbitMQ 集成
                    // services.AddWolverineRabbitMq(options =>
                    // {
                    //     options.HostName = "localhost";
                    //     options.UserName = "guest";
                    //     options.Password = "guest";
                    // });
                });

        private static async Task DemoCommandProcessing(IMessageBus messageBus)
        {
            Console.WriteLine("\n=== 演示命令处理 ===");

            // 创建订单命令
            var createOrderCommand = new CreateOrderCommand
            {
                OrderId = Guid.NewGuid(),
                CustomerId = 12345,
                Amount = 99.99m,
                Items = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 2, Price = 49.99m } }
            };

            // 发送命令并等待结果
            var result = await messageBus.InvokeAsync<OrderResult>(createOrderCommand);
            Console.WriteLine($"命令处理结果: {result.Success}");
            if (result.Success)
            {
                Console.WriteLine($"订单创建成功，订单ID: {result.OrderId}");
            }

            // 更新订单命令
            var updateOrderCommand = new UpdateOrderCommand
            {
                OrderId = createOrderCommand.OrderId,
                Amount = 149.99m,
                Items = new List<OrderItem> 
                {
                    new OrderItem { ProductId = 1, Quantity = 2, Price = 49.99m },
                    new OrderItem { ProductId = 2, Quantity = 1, Price = 50.00m }
                }
            };

            // 发送命令并等待结果
            var updateResult = await messageBus.InvokeAsync<OrderResult>(updateOrderCommand);
            Console.WriteLine($"命令处理结果: {updateResult.Success}");
            if (updateResult.Success)
            {
                Console.WriteLine($"订单更新成功，订单ID: {updateResult.OrderId}");
            }
        }

        private static async Task DemoEventProcessing(IMessageBus messageBus)
        {
            Console.WriteLine("\n=== 演示事件处理 ===");

            // 发布订单创建事件
            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = Guid.NewGuid(),
                CustomerId = 67890,
                Amount = 199.99m,
                CreatedAt = DateTime.UtcNow
            };

            // 发布事件
            await messageBus.PublishAsync(orderCreatedEvent);
            Console.WriteLine($"订单创建事件已发布，订单ID: {orderCreatedEvent.OrderId}");

            // 等待事件处理完成
            await Task.Delay(1000);

            // 发布订单更新事件
            var orderUpdatedEvent = new OrderUpdatedEvent
            {
                OrderId = orderCreatedEvent.OrderId,
                Amount = 249.99m,
                UpdatedAt = DateTime.UtcNow
            };

            // 发布事件
            await messageBus.PublishAsync(orderUpdatedEvent);
            Console.WriteLine($"订单更新事件已发布，订单ID: {orderUpdatedEvent.OrderId}");

            // 等待事件处理完成
            await Task.Delay(1000);
        }

        private static async Task DemoMessagePersistence(IMessageBus messageBus)
        {
            Console.WriteLine("\n=== 演示消息持久化 ===");

            // 创建一个需要持久化的命令
            var persistedCommand = new CreateOrderCommand
            {
                OrderId = Guid.NewGuid(),
                CustomerId = 54321,
                Amount = 299.99m,
                Items = new List<OrderItem> { new OrderItem { ProductId = 3, Quantity = 1, Price = 299.99m } }
            };

            // 发送需要持久化的命令
            var result = await messageBus.InvokeAsync<OrderResult>(persistedCommand);
            Console.WriteLine($"持久化命令处理结果: {result.Success}");
            if (result.Success)
            {
                Console.WriteLine($"订单创建成功并持久化，订单ID: {result.OrderId}");
            }
        }

        private static async Task DemoRetryPolicy(IMessageBus messageBus)
        {
            Console.WriteLine("\n=== 演示重试机制 ===");

            // 发送一个会失败但会重试的命令
            var retryCommand = new FailingCommand
            {
                CommandId = Guid.NewGuid(),
                AttemptCount = 0
            };

            try
            {
                var result = await messageBus.InvokeAsync<RetryResult>(retryCommand);
                Console.WriteLine($"重试命令处理结果: {result.Success}");
                if (result.Success)
                {
                    Console.WriteLine($"命令最终处理成功，尝试次数: {result.AttemptCount}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"命令处理最终失败: {ex.Message}");
            }
        }

        private static async Task DemoExceptionHandling(IMessageBus messageBus)
        {
            Console.WriteLine("\n=== 演示异常处理 ===");

            // 发送一个会立即失败的命令
            var exceptionCommand = new ExceptionCommand
            {
                CommandId = Guid.NewGuid()
            };

            try
            {
                var result = await messageBus.InvokeAsync<ExceptionResult>(exceptionCommand);
                Console.WriteLine($"异常命令处理结果: {result.Success}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"捕获到预期异常: {ex.Message}");
            }
        }

        private static async Task DemoPerformanceTesting(IMessageBus messageBus)
        {
            Console.WriteLine("\n=== 演示性能测试 ===");

            const int messageCount = 1000;
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            // 并行发送多条消息
            var tasks = Enumerable.Range(0, messageCount)
                .Select(i =>
                {
                    var command = new CreateOrderCommand
                    {
                        OrderId = Guid.NewGuid(),
                        CustomerId = i,
                        Amount = 100.00m + i,
                        Items = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 1, Price = 100.00m + i } }
                    };
                    return messageBus.InvokeAsync<OrderResult>(command);
                })
                .ToList();

            await Task.WhenAll(tasks);

            stopwatch.Stop();

            var successfulMessages = tasks.Count(t => t.Result.Success);
            Console.WriteLine($"性能测试完成: 发送 {messageCount} 条消息，成功 {successfulMessages} 条");
            Console.WriteLine($"总耗时: {stopwatch.ElapsedMilliseconds} 毫秒");
            Console.WriteLine($"平均每秒处理: {messageCount * 1000 / stopwatch.ElapsedMilliseconds} 条消息");
        }
    }

    // 命令和事件定义
    public class CreateOrderCommand
    {
        public Guid OrderId { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class UpdateOrderCommand
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class OrderUpdatedEvent
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class FailingCommand
    {
        public Guid CommandId { get; set; }
        public int AttemptCount { get; set; }
    }

    public class ExceptionCommand
    {
        public Guid CommandId { get; set; }
    }

    // 结果类型
    public class OrderResult
    {
        public bool Success { get; set; }
        public Guid OrderId { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class RetryResult
    {
        public bool Success { get; set; }
        public int AttemptCount { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class ExceptionResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    // 命令处理器实现
    public class CreateOrderCommandHandler
    {
        [WolverineHandler]
        public async Task<OrderResult> HandleAsync(CreateOrderCommand command, IMessageBus messageBus, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken); // 模拟处理时间
            Console.WriteLine($"创建订单: {command.OrderId}, 客户: {command.CustomerId}, 金额: {command.Amount}");
            
            // 发布订单创建事件
            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = command.OrderId,
                CustomerId = command.CustomerId,
                Amount = command.Amount,
                CreatedAt = DateTime.UtcNow
            };
            
            // 发布事件
            await messageBus.PublishAsync(orderCreatedEvent);
            
            return new OrderResult { Success = true, OrderId = command.OrderId };
        }
    }

    public class UpdateOrderCommandHandler
    {
        [WolverineHandler]
        public async Task<OrderResult> HandleAsync(UpdateOrderCommand command, IMessageBus messageBus, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken); // 模拟处理时间
            Console.WriteLine($"更新订单: {command.OrderId}, 新金额: {command.Amount}");
            
            // 发布订单更新事件
            var orderUpdatedEvent = new OrderUpdatedEvent
            {
                OrderId = command.OrderId,
                Amount = command.Amount,
                UpdatedAt = DateTime.UtcNow
            };
            
            // 发布事件
            await messageBus.PublishAsync(orderUpdatedEvent);
            
            return new OrderResult { Success = true, OrderId = command.OrderId };
        }
    }

    public class FailingCommandHandler
    {
        [WolverineHandler]
        [RetryNow(3)] // 配置重试策略
        public async Task<RetryResult> HandleAsync(FailingCommand command, CancellationToken cancellationToken = default)
        {
            command.AttemptCount++;
            await Task.Delay(100, cancellationToken); // 模拟处理时间

            if (command.AttemptCount < 3)
            {
                Console.WriteLine($"命令处理失败 (尝试 {command.AttemptCount}/3): {command.CommandId}");
                throw new InvalidOperationException("模拟命令处理失败");
            }

            Console.WriteLine($"命令处理成功 (尝试 {command.AttemptCount}/3): {command.CommandId}");
            return new RetryResult { Success = true, AttemptCount = command.AttemptCount };
        }
    }

    public class ExceptionCommandHandler
    {
        [WolverineHandler]
        public Task<ExceptionResult> HandleAsync(ExceptionCommand command, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("命令处理发生异常");
        }
    }

    // 事件处理器实现
    public class OrderCreatedEventHandler
    {
        [WolverineHandler]
        public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            await Task.Delay(50, cancellationToken); // 模拟处理时间
            Console.WriteLine($"处理订单创建事件: {@event.OrderId}, 金额: {@event.Amount}");
        }
    }

    public class OrderUpdatedEventHandler
    {
        [WolverineHandler]
        public async Task HandleAsync(OrderUpdatedEvent @event, CancellationToken cancellationToken = default)
        {
            await Task.Delay(50, cancellationToken); // 模拟处理时间
            Console.WriteLine($"处理订单更新事件: {@event.OrderId}, 新金额: {@event.Amount}");
        }
    }

    public class EmailNotificationHandler
    {
        [WolverineHandler]
        public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken); // 模拟处理时间
            Console.WriteLine($"发送订单创建邮件通知: 客户 {@event.CustomerId}, 订单 {@event.OrderId}");
        }
    }

    // 内存消息存储（用于演示）
    public class InMemoryMessageStore : IMessageStore
    {
        private readonly Dictionary<string, string> _messages = new();

        public Task DeleteAsync(string messageId, CancellationToken cancellationToken = default)
        {
            _messages.Remove(messageId);
            return Task.CompletedTask;
        }

        public Task<string?> LoadAsync(string messageId, CancellationToken cancellationToken = default)
        {
            _messages.TryGetValue(messageId, out var message);
            return Task.FromResult(message);
        }

        public Task StoreAsync(string messageId, string message, CancellationToken cancellationToken = default)
        {
            _messages[messageId] = message;
            return Task.CompletedTask;
        }
    }

    // 内存消息存储接口
    public interface IMessageStore
    {
        Task StoreAsync(string messageId, string message, CancellationToken cancellationToken = default);
        Task<string?> LoadAsync(string messageId, CancellationToken cancellationToken = default);
        Task DeleteAsync(string messageId, CancellationToken cancellationToken = default);
    }
}