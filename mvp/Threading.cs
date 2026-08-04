#:sdk Microsoft.NET.Sdk.Web
#:sdk Aspire.AppHost.Sdk@9.4.2
#:package Aspire.Hosting.AppHost@9.4.2
#:package Microsoft.Extensions.Hosting@10.0.0-rc.1.25451.107
#:package System.Threading.Channels@10.0.0-rc.1.25451.107
#:package Microsoft.Extensions.ObjectPool@10.0.0-rc.1.25451.107
#:package Microsoft.Extensions.ObjectPool.DependencyInjection@9.9.0
#:package Swashbuckle.AspNetCore@9.0.4
#:package Swashbuckle.AspNetCore.Swagger@9.0.4
#:package Swashbuckle.AspNetCore.SwaggerGen@9.0.4
#:package Swashbuckle.AspNetCore.SwaggerUI@9.0.4
#:package Scalar.AspNetCore@2.8.0
#:package Ardalis.ListStartupServices@1.1.4
#:package Serilog@4.3.0
#:package Serilog.AspNetCore@9.0.0
#:package Serilog.Enrichers.Environment@3.0.1
#:package Serilog.Enrichers.Process@3.0.0
#:package Serilog.Enrichers.Span@3.1.0
#:package Serilog.Enrichers.Thread@4.0.0
#:package Serilog.Exceptions@8.4.0
#:package Serilog.Extensions.Hosting@9.0.0
#:package Serilog.Extensions.Logging@9.0.2
#:package Serilog.Formatting.Compact@3.0.0
#:package Serilog.Settings.Configuration@9.0.0
#:package Serilog.Sinks.Async@2.1.0
#:package Serilog.Sinks.Console@6.0.0
#:package Serilog.Sinks.EventLog@4.0.0
#:package Serilog.Sinks.Http@9.2.0
#:package Serilog.Sinks.RollingFileAlternate@2.0.9
#:package Serilog.Sinks.Seq@9.0.0
#:package Serilog.Sinks.SpectreConsole@0.3.3
#:package Serilog.Sinks.Trace@4.0.0
#:package ModelContextProtocol@0.3.0-preview.4
#:package ModelContextProtocol.Core@0.3.0-preview.4
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property RollForward=Major
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=True
#:property Platform=Any CPU
#:property PackAsTool=True
#:property PackageType=McpServer
#:property PackageReadmeFile=README.md
#:property PackageId=AOT.SampleMcpServer
#:property PackageVersion=0.0.1-beta
#:property PackageTags=AI; MCP; server; stdio
#:property Description=An MCP server using the MCP C# SDK.

using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Net;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Ardalis.ListStartupServices;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using ModelContextProtocol.Server;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

builder.Logging.ClearProviders();               // 只保留下面的 ConsoleProvider，避免干扰
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);                   // 使用颜色化的控制台日志
builder.Logging.SetMinimumLevel(LogLevel.Trace); // 设为 Trace，最细粒度
builder.Logging.AddFilter("Microsoft.Extensions.Http", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.Extensions.ServiceDiscovery", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.Extensions.Resilience", LogLevel.Trace);
builder.Logging.AddFilter("RestEase.HttpClientFactory", LogLevel.Trace);
builder.Logging.AddFilter("App.ServiceDiscoveryHandler", LogLevel.Trace);
Serilog.Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Error)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "error", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Fatal)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "fatal", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Information)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "info", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Warning)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "warning", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog(Log.Logger, true).ConfigureLogging((context, logging) =>
{
    logging.ClearProviders();
    logging.AddConfiguration(context.Configuration);
    logging.AddSerilog(Log.Logger, true);
});
builder.Services.AddOptions();
builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("X-Pagination", "X-Platform-Type", "X-CSRF-TOKEN-HEADERNAME", "X-CSRF-TOKEN", "X-Forwarded-For", "X-Forwarded-Host", "X-Forwarded-Proto", "X-Forwarded-Prefix", "X-Body-Hash")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
// });

// 测试MCP
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<App.RandomNumberTools>();

builder.Services.Configure<Ardalis.ListStartupServices.ServiceConfig>(config =>
{
    config.Services = [.. builder.Services];
    config.Path = "/services";
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapScalarApiReference(); // scalar
}
else
{
    app.UseExceptionHandler("/Error");
}
app.UseSerilogRequestLogging(opt =>
{
    opt.IncludeQueryInRequestPath = true;
});
app.UseRouting();
app.UseCors();
// app.UseAuthorization();
// app.UseSwagger();
// app.UseSwaggerUI(options =>
// {
//     options.SwaggerEndpoint("/swagger/v1/swagger.json", "Robot API V1.1.0.0");
// });
app.UseShowAllServicesMiddleware();
app.MapGet("/", () => "Mcp Agent Integration!");
app.UseEndpoints(options =>
{
    options.MapControllers();
});

await app.RunAsync();

namespace App
{
    public partial class Program;

    // 1. 基础服务类 - 演示正确的线程同步使用
    public class ThreadSafeDataService
    {
        private readonly AsyncReaderWriterLock _readerWriterLock;
        private readonly Dictionary<int, User> _users = new Dictionary<int, User>();
        private readonly AsyncQueue<User> _userUpdateQueue;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ThreadSafeDataService()
        {
            // 初始化异步读写锁，用于保护共享数据的并发访问
            _readerWriterLock = new AsyncReaderWriterLock();

            // 初始化异步队列，用于处理用户更新请求
            _userUpdateQueue = new AsyncQueue<User>();

            // 初始化取消令牌源
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// 异步获取用户信息 - 使用读锁确保线程安全
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>用户信息</returns>
        public async Task<User> GetUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            // 获取读锁 - 允许多个读操作同时进行，但阻止写操作
            using var readLock = await _readerWriterLock.ReadLockAsync(cancellationToken);

            if (_users.TryGetValue(userId, out var user))
            {
                return user;
            }

            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        /// <summary>
        /// 异步更新用户信息 - 使用写锁确保独占访问
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        public async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            // 获取写锁 - 独占访问，阻止所有读写操作
            using var writeLock = await _readerWriterLock.WriteLockAsync(cancellationToken);

            _users[user.Id] = user;

            // 释放锁后通知等待的读者
            await writeLock.ReleaseAsync(); // 提前释放锁以提高并发性能
        }

        /// <summary>
        /// 批量更新用户 - 使用升级锁支持从读到写的升级
        /// </summary>
        /// <param name="users">用户集合</param>
        /// <param name="cancellationToken">取消令牌</param>
        public async Task BatchUpdateUsersAsync(IEnumerable<User> users, CancellationToken cancellationToken = default)
        {
            // 先获取读锁
            using var readLock = await _readerWriterLock.ReadLockAsync(cancellationToken);

            // 检查是否需要更新
            var existingUserCount = _users.Count;

            // 如果需要更新，则升级到写锁
            using var upgradeLock = await readLock.UpgradeAsync(cancellationToken);

            // 执行批量更新操作
            foreach (var user in users)
            {
                _users[user.Id] = user;
            }
        }

        /// <summary>
        /// 后台处理用户更新队列
        /// </summary>
        public async Task ProcessUserUpdatesAsync()
        {
            var cancellationToken = _cancellationTokenSource.Token;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // 异步等待队列中的项目，避免阻塞线程
                    var user = await _userUpdateQueue.DequeueAsync(cancellationToken);

                    // 处理用户更新
                    await UpdateUserAsync(user, cancellationToken);
                    Console.WriteLine($"Processed user update for {user.Name}");
                }
            }
            catch (OperationCanceledException)
            {
                // 正常取消情况下的处理
                Console.WriteLine("User update processing cancelled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing user updates: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _readerWriterLock.Dispose();
        }
    }

    // 2. 用户实体类
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime LastModified { get; set; }
    }

    // 3. 异步并发处理器 - 演示JoinableTaskFactory的使用
    public class AsyncConcurrencyProcessor
    {
        private readonly JoinableTaskContext _joinableTaskContext;
        private readonly JoinableTaskFactory _joinableTaskFactory;

        public AsyncConcurrencyProcessor()
        {
            // 创建JoinableTaskContext，用于管理异步任务的线程上下文
            _joinableTaskContext = new JoinableTaskContext();

            // 获取JoinableTaskFactory，用于创建和管理异步任务
            _joinableTaskFactory = _joinableTaskContext.Factory;
        }

        /// <summary>
        /// 处理并发业务请求 - 使用JoinableTask确保正确的线程上下文
        /// </summary>
        /// <param name="requests">请求集合</param>
        /// <returns>处理结果</returns>
        public async Task<Dictionary<int, ProcessingResult>> ProcessConcurrentRequestsAsync(
            IEnumerable<BusinessRequest> requests)
        {
            var results = new Dictionary<int, ProcessingResult>();

            // 创建并发任务列表
            var tasks = requests.Select(request =>
                _joinableTaskFactory.RunAsync(async () =>
                {
                    try
                    {
                        // 模拟业务处理
                        var result = await ProcessBusinessRequestAsync(request);
                        return new { RequestId = request.Id, Result = result };
                    }
                    catch (Exception ex)
                    {
                        return new { RequestId = request.Id, Result = new ProcessingResult { Success = false, ErrorMessage = ex.Message } };
                    }
                })).ToArray();

            // 等待所有任务完成
            var completedTasks = await Task.WhenAll(tasks.Select(t => t.Task));

            // 收集结果
            foreach (var completedTask in completedTasks)
            {
                results[completedTask.RequestId] = completedTask.Result;
            }

            return results;
        }

        /// <summary>
        /// 处理单个业务请求
        /// </summary>
        /// <param name="request">业务请求</param>
        /// <returns>处理结果</returns>
        private async Task<ProcessingResult> ProcessBusinessRequestAsync(BusinessRequest request)
        {
            // 模拟一些异步操作
            await Task.Delay(100);

            // 模拟需要回主线程的操作（如UI更新）
            await _joinableTaskFactory.SwitchToMainThreadAsync();

            // 在主线程执行的操作（如果是UI应用程序）
            // ... UI更新逻辑

            // 切换回线程池继续执行
            if (_joinableTaskFactory.Context.IsOnMainThread)
            {
                await TaskScheduler.Default.SwitchTo();
            }

            // 继续处理其他逻辑
            await Task.Delay(50);

            return new ProcessingResult
            {
                Success = true,
                ProcessedAt = DateTime.Now,
                RequestId = request.Id
            };
        }

        public void Dispose()
        {
            _joinableTaskContext.Dispose();
        }
    }

    // 4. 业务请求和处理结果类
    public class BusinessRequest
    {
        public int Id { get; set; }
        public string Operation { get; set; }
        public object Data { get; set; }
    }

    public class ProcessingResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int RequestId { get; set; }
        public DateTime ProcessedAt { get; set; }
    }

    // 5. 线程池优化服务 - 演示AwaitExtensions的使用
    public class ThreadPoolOptimizationDemo
    {
        private readonly AsyncSemaphore _asyncSemaphore;
        private readonly AsyncCountdownEvent _countdownEvent;

        public ThreadPoolOptimizationDemo()
        {
            // 初始化信号量，限制并发访问数量
            _asyncSemaphore = new AsyncSemaphore(initialCount: 10); // 最多允许10个并发操作

            // 初始化倒计时事件
            _countdownEvent = new AsyncCountdownEvent(5); // 等待5个操作完成
        }

        /// <summary>
        /// 优化的并发处理方法 - 使用异步信号量控制并发度
        /// </summary>
        /// <param name="operations">操作集合</param>
        /// <returns>处理结果</returns>
        public async Task<List<string>> ProcessOperationsOptimizedAsync(
            IEnumerable<Func<Task<string>>> operations)
        {
            var results = new List<string>();
            var tasks = new List<Task<string>>();

            foreach (var operation in operations)
            {
                // 异步等待信号量许可
                var semaphoreReleaser = await _asyncSemaphore.EnterAsync();

                // 创建封装的异步任务
                tasks.Add(WrapOperationWithSemaphore(operation, semaphoreReleaser));
            }

            try
            {
                // 等待所有操作完成
                var completedResults = await Task.WhenAll(tasks);
                results.AddRange(completedResults);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Some operations failed: {ex.Message}");
                // 可以根据需要处理部分失败的情况
            }

            return results;
        }

        /// <summary>
        /// 封装带有信号量释放的操作
        /// </summary>
        private async Task<string> WrapOperationWithSemaphore(
            Func<Task<string>> operation,
            AsyncSemaphore.Releaser semaphoreReleaser)
        {
            try
            {
                return await operation();
            }
            finally
            {
                // 确保信号量许可被正确归还
                semaphoreReleaser.Dispose();
            }
        }

        /// <summary>
        /// 使用倒计时事件的批量操作示例
        /// </summary>
        public async Task CoordinateBatchOperationsAsync()
        {
            var countdown = new AsyncCountdownEvent(3);

            // 启动三个并发操作
            var task1 = ProcessOperationWithCountdown("Operation1", countdown);
            var task2 = ProcessOperationWithCountdown("Operation2", countdown);
            var task3 = ProcessOperationWithCountdown("Operation3", countdown);

            // 等待所有操作完成
            await countdown.WaitAsync();

            Console.WriteLine("All batch operations completed");
        }

        private async Task ProcessOperationWithCountdown(string operationName, AsyncCountdownEvent countdown)
        {
            // 模拟处理时间
            await Task.Delay(new Random().Next(1000, 3000));

            Console.WriteLine($"{operationName} completed");

            // 减少计数
            countdown.Signal();
        }

        public void Dispose()
        {
            _asyncSemaphore.Dispose();
            _countdownEvent.Dispose();
        }
    }

    // 6. 异步集合类 - 线程安全的集合操作
    public class AsyncCollectionService
    {
        private readonly AsyncQueue<int> _numberQueue;
        private readonly AsyncCollection<int> _asyncCollection;

        public AsyncCollectionService()
        {
            _numberQueue = new AsyncQueue<int>();
            _asyncCollection = new AsyncCollection<int>();
        }

        /// <summary>
        /// 异步处理队列项目
        /// </summary>
        public async Task QueueProcessingDemoAsync()
        {
            // 启动消费者任务
            var consumerTask = ProcessQueueItemsAsync();

            // 生成队列项目
            for (int i = 1; i <= 10; i++)
            {
                _numberQueue.TryEnqueue(i);
            }

            // 等待处理完成
            await consumerTask;
        }

        private async Task ProcessQueueItemsAsync()
        {
            try
            {
                while (true)
                {
                    var item = await _numberQueue.DequeueAsync();
                    Console.WriteLine($"Processing item: {item}");
                    await Task.Delay(100); // 模拟处理时间
                }
            }
            catch (OperationCanceledException)
            {
                // 队列处理正常结束
            }
        }

        /// <summary>
        /// 异步集合操作示例
        /// </summary>
        public async Task CollectionOperationsDemoAsync()
        {
            // 添加项目到异步集合
            await _asyncCollection.AddAsync(1);
            await _asyncCollection.AddAsync(2);
            await _asyncCollection.AddAsync(3);

            // 获取所有项目
            var allItems = await _asyncCollection.GetAllItemsAsync();
            Console.WriteLine($"Collection items: {string.Join(", ", allItems)}");

            // 移除项目
            await _asyncCollection.RemoveAsync(2);

            // 检查集合内容
            var remainingItems = await _asyncCollection.GetAllItemsAsync();
            Console.WriteLine($"Remaining items: {string.Join(", ", remainingItems)}");
        }
    }

    // 7. 死锁预防服务 - 演示如何避免死锁
    public class DeadlockPreventionService
    {
        private readonly AsyncReaderWriterLock _lock1 = new AsyncReaderWriterLock();
        private readonly AsyncReaderWriterLock _lock2 = new AsyncReaderWriterLock();
        private readonly JoinableTaskFactory _joinableTaskFactory;

        private readonly Dictionary<string, object> _data1 = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _data2 = new Dictionary<string, object>();

        public DeadlockPreventionService(JoinableTaskContext context)
        {
            _joinableTaskFactory = context.Factory;
        }

        /// <summary>
        /// 安全的双重锁操作 - 使用正确的锁获取顺序避免死锁
        /// </summary>
        public async Task SafeDoubleLockOperationAsync(CancellationToken cancellationToken = default)
        {
            // 总是按照相同顺序获取锁以避免死锁
            using var lock1Write = await _lock1.WriteLockAsync(cancellationToken);
            using var lock2Write = await _lock2.WriteLockAsync(cancellationToken);

            // 执行需要两个锁的操作
            await PerformCriticalOperationAsync();
        }

        /// <summary>
        /// 使用JoinableTask的死锁安全操作
        /// </summary>
        public async Task SafeOnUiThreadOperationAsync()
        {
            // 不要在主线程上直接等待同步上下文相关的任务
            await _joinableTaskFactory.RunAsync(async () =>
            {
                // 在JoinableTask上下文中执行，避免死锁
                await Task.Delay(1000);

                // 切换到主线程（如果需要）
                await _joinableTaskFactory.SwitchToMainThreadAsync();

                // 执行UI相关操作
                // PerformUIOperation();

                // 切换回后台线程
                await TaskScheduler.Default.SwitchTo();

                // 继续后台处理
                await Task.Delay(500);
            });
        }

        private async Task PerformCriticalOperationAsync()
        {
            // 模拟关键操作
            await Task.Delay(200);
            _data1["lastModified"] = DateTime.Now;
            _data2["lastModified"] = DateTime.Now;
        }
    }

    // 8. 性能监控和诊断服务
    public class ThreadingPerformanceMonitor
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        /// <summary>
        /// 监控异步操作性能
        /// </summary>
        public async Task<double> MonitorOperationPerformanceAsync(Func<Task> operation)
        {
            _stopwatch.Restart();

            await operation();

            _stopwatch.Stop();
            return _stopwatch.Elapsed.TotalMilliseconds;
        }

        /// <summary>
        /// 监控并发操作的性能影响
        /// </summary>
        public async Task MonitorConcurrencyImpactAsync(
            Func<Task> operation,
            int concurrencyLevel)
        {
            var tasks = new List<Task<double>>();

            for (int i = 0; i < concurrencyLevel; i++)
            {
                tasks.Add(MonitorOperationPerformanceAsync(operation));
            }

            var results = await Task.WhenAll(tasks);

            Console.WriteLine($"Concurrency level: {concurrencyLevel}");
            Console.WriteLine($"Average execution time: {results.Average():F2}ms");
            Console.WriteLine($"Min execution time: {results.Min():F2}ms");
            Console.WriteLine($"Max execution time: {results.Max():F2}ms");
        }
    }

    // 9. 主程序演示类
    public class ThreadingDemoProgram
    {
        private readonly ThreadSafeDataService _dataService;
        private readonly AsyncConcurrencyProcessor _concurrencyProcessor;
        private readonly ThreadPoolOptimizationDemo _optimizationDemo;
        private readonly AsyncCollectionService _collectionService;
        private readonly DeadlockPreventionService _deadlockService;
        private readonly ThreadingPerformanceMonitor _monitor;

        public ThreadingDemoProgram()
        {
            _dataService = new ThreadSafeDataService();
            _concurrencyProcessor = new AsyncConcurrencyProcessor();
            _optimizationDemo = new ThreadPoolOptimizationDemo();
            _collectionService = new AsyncCollectionService();
            _deadlockService = new DeadlockPreventionService(_concurrencyProcessor._joinableTaskContext);
            _monitor = new ThreadingPerformanceMonitor();
        }

        /// <summary>
        /// 运行完整的生产级线程演示
        /// </summary>
        public async Task RunDemoAsync()
        {
            Console.WriteLine("=== Microsoft.VisualStudio.Threading Production Demo ===");

            #region 线程安全数据服务演示

            Console.WriteLine("\n1. Thread Safe Data Service Demo:");

            // 添加测试用户
            var testUser = new User { Id = 1, Name = "Test User", Email = "test@example.com" };
            await _dataService.UpdateUserAsync(testUser);

            // 获取用户信息
            var retrievedUser = await _dataService.GetUserAsync(1);
            Console.WriteLine($"Retrieved user: {retrievedUser.Name}");

            // 批量更新演示
            var batchUsers = new[]
            {
            new User { Id = 2, Name = "User2", Email = "user2@example.com" },
            new User { Id = 3, Name = "User3", Email = "user3@example.com" }
        };
            await _dataService.BatchUpdateUsersAsync(batchUsers);
            Console.WriteLine("Batch users updated successfully");

            #endregion

            #region 并发处理演示

            Console.WriteLine("\n2. Async Concurrency Processing Demo:");

            var requests = Enumerable.Range(1, 5).Select(i => new BusinessRequest
            {
                Id = i,
                Operation = $"Operation_{i}",
                Data = $"Data_{i}"
            });

            var processingResults = await _concurrencyProcessor.ProcessConcurrentRequestsAsync(requests);
            Console.WriteLine($"Processed {processingResults.Count} requests");

            foreach (var result in processingResults)
            {
                Console.WriteLine($"Request {result.Key}: Success = {result.Value.Success}");
            }

            #endregion

            #region 线程池优化演示

            Console.WriteLine("\n3. ThreadPool Optimization Demo:");

            var operations = Enumerable.Range(1, 15).Select(i => new Func<Task<string>>(async () =>
            {
                await Task.Delay(500);
                return $"Operation {i} completed";
            }));

            var optimizedResults = await _optimizationDemo.ProcessOperationsOptimizedAsync(operations);
            Console.WriteLine($"Optimized processing completed for {optimizedResults.Count} operations");

            #endregion

            #region 异步集合演示

            Console.WriteLine("\n4. Async Collection Demo:");
            await _collectionService.QueueProcessingDemoAsync();
            await _collectionService.CollectionOperationsDemoAsync();

            #endregion

            #region 性能监控演示

            Console.WriteLine("\n5. Performance Monitoring Demo:");
            await _monitor.MonitorConcurrencyImpactAsync(
                () => Task.Delay(1000),
                5);

            #endregion

            Console.WriteLine("\n=== Demo Complete ===");
            Dispose();
        }

        private void Dispose()
        {
            _dataService.Dispose();
            _concurrencyProcessor.Dispose();
            _optimizationDemo.Dispose();
        }
    }

    // 10. ASP.NET Core集成配置类
    public class ThreadingConfiguration
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            // 注册JoinableTaskContext（需要单例）
            services.AddSingleton<JoinableTaskContext>(provider =>
            {
                return new JoinableTaskContext();
            });

            // 注册JoinableTaskFactory
            services.AddSingleton<JoinableTaskFactory>(provider =>
            {
                var context = provider.GetRequiredService<JoinableTaskContext>();
                return context.Factory;
            });

            // 注册其他线程安全服务
            services.AddTransient<ThreadSafeDataService>();
            services.AddTransient<AsyncConcurrencyProcessor>();
            services.AddTransient<ThreadPoolOptimizationDemo>();
            services.AddTransient<DeadlockPreventionService>();
        }
    }

    // 11. 高级异步同步原语使用示例
    public class AdvancedThreadingDemo
    {
        private readonly AsyncLazy<string> _lazyValue;
        private readonly AsyncAutoResetEvent _autoResetEvent;
        private readonly AsyncManualResetEvent _manualResetEvent;

        public AdvancedThreadingDemo()
        {
            // 初始化异步延迟加载
            _lazyValue = new AsyncLazy<string>(async () =>
            {
                await Task.Delay(1000); // 模拟初始化成本
                return "Expensive resource loaded asynchronously";
            });

            // 初始化异步自动重置事件
            _autoResetEvent = new AsyncAutoResetEvent();

            // 初始化异步手动重置事件
            _manualResetEvent = new AsyncManualResetEvent();
        }

        /// <summary>
        /// 异步延迟加载演示
        /// </summary>
        public async Task<string> GetLazyValueAsync()
        {
            // 第一次调用时会执行初始化委托
            // 后续调用会直接返回缓存的值
            return await _lazyValue.GetValueAsync();
        }

        /// <summary>
        /// 异步事件同步演示
        /// </summary>
        public async Task EventSynchronizationDemoAsync()
        {
            // 启动等待任务
            var waiterTask = WaitForEventAsync();

            // 设置事件信号
            await Task.Delay(2000);
            _autoResetEvent.Set();

            await waiterTask;
        }

        private async Task WaitForEventAsync()
        {
            Console.WriteLine("Waiting for auto reset event...");
            await _autoResetEvent.WaitAsync();
            Console.WriteLine("Auto reset event received!");

            Console.WriteLine("Waiting for manual reset event...");
            await _manualResetEvent.WaitAsync();
            Console.WriteLine("Manual reset event received!");
        }

        public void SetManualEvent()
        {
            _manualResetEvent.Set();
        }

        public void Dispose()
        {
            _autoResetEvent.Dispose();
            _manualResetEvent.Dispose();
        }
    }

    // 12. 程序入口点
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var demo = new ThreadingDemoProgram();
                await demo.RunDemoAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Demo failed with error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }

    // 13. 生产级最佳实践配置
    public class ProductionThreadConfiguration
    {
        /// <summary>
        /// 获取生产环境优化的JoinableTaskContext
        /// </summary>
        public static JoinableTaskContext CreateProductionContext()
        {
            // 在生产环境中配置适当的选项
            var context = new JoinableTaskContext();

            // 注意：实际的生产配置选项可能需要根据具体版本调整
            // 以下是一些常见的生产级建议：

            // 1. 适当设置最大并发度
            // 2. 配置合理的超时机制
            // 3. 启用性能监控（如果需要）
            // 4. 预热线程池

            return context;
        }

        /// <summary>
        /// 创建生产环境的异步读写锁配置
        /// </summary>
        public static void ConfigureProductionLocks(IServiceCollection services)
        {
            // 注册多种锁实例以供不同场景使用
            services.AddSingleton<AsyncReaderWriterLock>(provider =>
            {
                var lockInstance = new AsyncReaderWriterLock();

                // 生产环境下可以配置锁超时
                // lockInstance.DefaultTimeout = TimeSpan.FromSeconds(30);

                return lockInstance;
            });

            // 注册信号量用于控制资源访问
            services.AddSingleton<AsyncSemaphore>(provider =>
                new AsyncSemaphore(initialCount: 50)); // 根据实际需求设置并发度
        }
    }

    // 核心组件说明
    // AsyncReaderWriterLock - 异步读写锁，支持读写分离
    // JoinableTaskFactory - 管理异步任务的线程上下文
    // AsyncSemaphore - 异步信号量，控制并发访问
    // AsyncQueue - 线程安全的异步队列
    // AsyncLazy - 异步延迟加载
    // AsyncAutoResetEvent/AsyncManualResetEvent - 异步事件同步原语

    public void Test()
    {
        // 死锁预防最佳实践
        // 可以使用JoinableTaskFactory.RunAsync避免死锁
        await joinableTaskFactory.RunAsync(async () =>
        {
            await Task.Delay(1000);
            // 需要UI线程时安全切换
            await joinableTaskFactory.SwitchToMainThreadAsync();
            // UI操作...
            // 切换回后台线程继续处理
            await TaskScheduler.Default.SwitchTo();
        });

        // 线程安全集合操作
        // 使用AsyncQueue进行线程安全的队列操作
        var queue = new AsyncQueue<string>();
        queue.TryEnqueue("item");

        // 异步等待队列项目，避免阻塞
        var item = await queue.DequeueAsync();
    }

    // 性能优化要点
    // 限制并发度: 使用AsyncSemaphore控制并发数量
    // 减少锁竞争: 合理使用读写锁分离读写操作
    // 避免阻塞: 异步等待而不是同步阻塞
    // 正确上下文切换: 使用SwitchToMainThreadAsync和TaskScheduler.Default.SwitchTo()
    
    // 生产部署建议
    // 适当的超时配置: 防止无限等待
    // 监控和诊断: 启用性能监控和日志记录
    // 资源管理: 正确处置所有同步原语
    // 错误处理: 实现健壮的异常处理机制
}