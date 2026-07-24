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
#:property TargetFramework=net11.0
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;

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

    // 1. 业务实体模型定义
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public int StockCount { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Amount { get; set; }
        public OrderStatus Status { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Processing,
        Completed,
        Cancelled
    }

    // 2. 异步数据提供服务
    public interface IAsyncDataService
    {
        IAsyncEnumerable<Product> GetProductsAsync();
        IAsyncEnumerable<Order> GetOrdersAsync();
        Task<IEnumerable<BusinessError>> GetProcessingErrorsAsync();
    }

    public class AsyncDataService : IAsyncDataService
    {
        private readonly ILogger<AsyncDataService> _logger;
        private readonly List<Product> _products;
        private readonly List<Order> _orders;
        private readonly ConcurrentQueue<BusinessError> _errors = new ConcurrentQueue<BusinessError>();

        public AsyncDataService(ILogger<AsyncDataService> logger)
        {
            _logger = logger;
            _products = GenerateSampleProducts();
            _orders = GenerateSampleOrders();
        }

        /// <summary>
        /// 异步获取产品列表
        /// 模拟从数据库或API获取数据的异步流
        /// </summary>
        public async IAsyncEnumerable<Product> GetProductsAsync()
        {
            _logger.LogInformation("Starting product stream generation");

            foreach (var product in _products)
            {
                // 模拟异步数据检索延迟
                await Task.Delay(10);

                _logger.LogDebug("Yielding product: {ProductName}", product.Name);

                yield return product;
            }

            _logger.LogInformation("Product stream generation completed");
        }

        /// <summary>
        /// 异步获取订单列表
        /// 支持分页和批量加载的异步流
        /// </summary>
        public async IAsyncEnumerable<Order> GetOrdersAsync()
        {
            _logger.LogInformation("Starting order stream generation");

            foreach (var order in _orders)
            {
                // 模拟异步数据检索延迟
                await Task.Delay(5);

                _logger.LogDebug("Yielding order: {OrderId}", order.Id);

                yield return order;
            }

            _logger.LogInformation("Order stream generation completed");
        }

        /// <summary>
        /// 获取业务处理错误队列
        /// 模拟应用运行时产生的错误记录流
        /// </summary>
        public async Task<IEnumerable<BusinessError>> GetProcessingErrorsAsync()
        {
            return _errors;
        }
    }

    // 3. 业务错误模型
    public class BusinessError
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string Operation { get; set; }
        public object ContextData { get; set; }
        public int RetryCount { get; set; }
    }

    // 4. 异步数据处理服务 - 核心业务逻辑
    public class AsyncDataProcessingService
    {
        private readonly IAsyncDataService _dataService;
        private readonly ILogger<AsyncDataProcessingService> _logger;
        private readonly SemaphoreSlim _processingSemaphore = new SemaphoreSlim(3, 3); // 并发限制

        public AsyncDataProcessingService(
            IAsyncDataService dataService,
            ILogger<AsyncDataProcessingService> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        /// <summary>
        /// 处理产品数据流 - 使用System.Interactive.Async进行复杂流处理
        /// </summary>
        public async Task<ProcessingResult<Product>> ProcessProductsAsync(int batchSize = 10)
        {
            _logger.LogInformation("Starting product processing");

            var processedCount = 0;
            var errorCount = 0;
            var batchResults = new List<ProductBatchResult>();

            try
            {
                var products = _dataService.GetProductsAsync();

                // 使用System.Interactive.Async进行分批处理
                // Buffer: 按批次收集元素
                // Where: 过滤有效数据
                // SelectMany: 并发处理每个批次
                // WithCancellation: 支持外部取消
                await products
                    .Buffer(batchSize) // 每batchSize个元素一批
                    .Where(batch => batch != null && batch.Any()) // 过滤空批次
                    .SelectMany(async batch =>
                    {
                        await _processingSemaphore.WaitAsync();
                        try
                        {
                            _logger.LogDebug("Processing batch of {BatchSize} products", batch.Count);

                            // 模拟批次处理
                            var result = await ProcessProductBatchAsync(batch);
                            batchResults.Add(result);

                            return result;
                        }
                        finally
                        {
                            _processingSemaphore.Release();
                        }
                    })
                    .ForEachAsync(batchResult =>
                    {
                        processedCount += batchResult.ProcessedCount;
                        errorCount += batchResult.ErrorCount;

                        _logger.LogInformation("Batch processed - Success: {ProcessedCount}, Errors: {ErrorCount}",
                            batchResult.ProcessedCount, batchResult.ErrorCount);
                    });

                _logger.LogInformation("Product processing completed - Total: {TotalCount}, Errors: {ErrorCount}",
                    processedCount, errorCount);

                return new ProcessingResult<Product>
                {
                    IsSuccess = true,
                    TotalProcessed = processedCount,
                    TotalErrors = errorCount,
                    Results = batchResults
                };
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("Product processing cancelled");
                return new ProcessingResult<Product>
                {
                    IsSuccess = false,
                    Message = "Processing cancelled",
                    TotalProcessed = processedCount,
                    TotalErrors = errorCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during product processing");
                return new ProcessingResult<Product>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    TotalProcessed = processedCount,
                    TotalErrors = errorCount + 1
                };
            }
        }

        /// <summary>
        /// 实时处理订单数据流 - 使用条件控制和流合并
        /// </summary>
        public async Task<RealTimeProcessingResult<Order>> ProcessOrdersRealTimeAsync(
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting real-time order processing");

            var ordersStream = _dataService.GetOrdersAsync();
            var result = new RealTimeProcessingResult<Order>();

            try
            {
                // 使用System.Interactive.Async进行实时流处理

                // TakeUntil: 在取消请求时停止
                // Where: 过滤处理中的订单
                // Delay: 模拟处理延迟
                // Catch: 处理流中的个别错误
                // Retry: 重试失败的流
                await ordersStream
                    .TakeUntil(cancellationToken) // 支持外部取消
                    .Where(order => order.Status != OrderStatus.Cancelled) // 过滤已取消订单
                    .Take(100) // 限制处理数量，生产环境中可以根据需求调整
                    .ToListAsync(cancellationToken) // 收集到列表进行批次处理
                    .ContinueWith(async orders =>
                    {
                        if (orders.IsCompletedSuccessfully)
                        {
                            var processedOrders = await ProcessOrdersConcurrently(orders.Result, cancellationToken);
                            result.ProcessedOrders.AddRange(processedOrders);
                        }
                    })
                    .Unwrap()
                    .WithCancellation(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Real-time order processing cancelled as requested");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Real-time order processing error");
                result.Errors.Add($"Processing error: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// 流合并处理 - 合并多个异步流进行统一处理
        /// </summary>
        public async Task<CombinedProcessingResult> ProcessCombinedDataStreamsAsync(
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting combined data stream processing");

            var productsStream = _dataService.GetProductsAsync();
            var ordersStream = _dataService.GetOrdersAsync();

            var result = new CombinedProcessingResult();

            try
            {
                // 使用Concat: 顺序连接多个流
                var combinedStream = productsStream
                    .Cast<object>() // 转换为object类型以便连接
                    .Concat(ordersStream.Cast<object>());

                await combinedStream
                    .TakeUntil(cancellationToken)
                    .Buffer(50) // 每50个元素一批
                    .Where(buffer => buffer.Any())
                    .SelectMany(async batch => await ProcessBatchAsync(batch, cancellationToken))
                    .ForEachAsync(processedBatch =>
                    {
                        result.TotalProcessed += processedBatch.Count();
                        _logger.LogInformation("Processed combined batch of {BatchSize} items", processedBatch.Count());
                    });
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Combined stream processing cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Combined stream processing error");
            }

            return result;
        }

        /// <summary>
        /// 流条件控制处理 - 使用复杂的流条件逻辑
        /// </summary>
        public async Task<ConditionalProcessingResult> ProcessWithConditionalLogicAsync(
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting conditional processing");

            var products = _dataService.GetProductsAsync();

            var result = new ConditionalProcessingResult();

            try
            {
                // 使用Window: 基于时间窗口的数据处理
                // Skip: 跳过前几个元素
                // TakeLast: 获取最后几个元素
                await products
                    .TakeUntil(cancellationToken)
                    .Window(TimeSpan.FromSeconds(5)) // 时间窗口处理
                    .SelectMany(async window => await window.ToListAsync(cancellationToken))
                    .Where(windowBatch => windowBatch.Any())
                    .Select((batch, index) => new { Batch = batch, Index = index })
                    .Do(async item => await Task.Delay(100, cancellationToken)) // 插入延迟
                    .ForEachAsync(async item =>
                    {
                        var processedBatch = await ProcessConditionalBatchAsync(item.Batch, item.Index, cancellationToken);
                        result.BatchResults.Add(processedBatch);
                    });
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Conditional processing cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Conditional processing error");
            }

            return result;
        }

        #region 辅助处理方法

        private async Task<ProductBatchResult> ProcessProductBatchAsync(List<Product> batch)
        {
            await Task.Delay(100); // 模拟处理延迟

            var processedCount = 0;
            var errorCount = 0;

            foreach (var product in batch)
            {
                try
                {
                    // 模拟产品处理逻辑
                    if (product.Price < 0)
                    {
                        errorCount++;
                        continue;
                    }

                    product.CreatedAt = DateTime.UtcNow;
                    processedCount++;
                }
                catch (Exception ex)
                {
                    errorCount++;
                    _logger.LogError(ex, "Error processing product {ProductId}", product.Id);
                }
            }

            return new ProductBatchResult
            {
                ProcessedCount = processedCount,
                ErrorCount = errorCount,
                BatchSize = batch.Count
            };
        }

        private async Task<List<Order>> ProcessOrdersConcurrently(
            List<Order> orders,
            CancellationToken cancellationToken)
        {
            var semaphore = new SemaphoreSlim(5, 5); // 5个并发处理
            var processedOrders = new List<Order>();

            try
            {
                var tasks = orders.Select(async order =>
                {
                    await semaphore.WaitAsync(cancellationToken);
                    try
                    {
                        await Task.Delay(50, cancellationToken); // 模拟处理
                        order.Status = OrderStatus.Processing;
                        return order;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                processedOrders = (await Task.WhenAll(tasks)).ToList();
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Order concurrent processing cancelled");
            }

            return processedOrders;
        }

        private async Task<IEnumerable<object>> ProcessBatchAsync(
            List<object> batch,
            CancellationToken cancellationToken)
        {
            await Task.Delay(200, cancellationToken);

            // 模拟批处理逻辑
            return batch.Where(item => item != null);
        }

        private async Task<ProcessedBatchResult> ProcessConditionalBatchAsync(
            List<Product> batch,
            int batchIndex,
            CancellationToken cancellationToken)
        {
            await Task.Delay(50, cancellationToken);

            return new ProcessedBatchResult
            {
                BatchIndex = batchIndex,
                ItemCount = batch.Count,
                ProcessedAt = DateTime.UtcNow,
                ProcessedItems = batch
            };
        }

        #endregion

        #region 数据生成辅助方法

        private List<Product> GenerateSampleProducts()
        {
            return Enumerable.Range(1, 100).Select(i => new Product
            {
                Id = i,
                Name = $"Product {i}",
                Price = new Random().Next(10, 1000),
                Category = i % 3 == 0 ? "Electronics" : i % 3 == 1 ? "Clothing" : "Books",
                IsActive = i % 10 != 0, // 10%的产品是不活跃的
                StockCount = new Random().Next(0, 1000),
                CreatedAt = DateTime.UtcNow.AddDays(-new Random().Next(365))
            }).ToList();
        }

        private List<Order> GenerateSampleOrders()
        {
            var statuses = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToArray();
            return Enumerable.Range(1, 200).Select(i => new Order
            {
                Id = i,
                ProductId = new Random().Next(1, 101),
                CustomerName = $"Customer {i}",
                OrderDate = DateTime.UtcNow.AddDays(-new Random().Next(30)),
                Amount = new Random().Next(10, 5000),
                Status = statuses[new Random().Next(statuses.Length)]
            }).ToList();
        }

        #endregion
    }

    // 5. 流处理结果模型类

    /// <summary>
    /// 通用处理结果模型
    /// </summary>
    public class ProcessingResult<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int TotalProcessed { get; set; }
        public int TotalErrors { get; set; }
        public List<ProductBatchResult> Results { get; set; } = new List<ProductBatchResult>();
    }

    /// <summary>
    /// 产品批处理结果
    /// </summary>
    public class ProductBatchResult
    {
        public int ProcessedCount { get; set; }
        public int ErrorCount { get; set; }
        public int BatchSize { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// 实时处理结果模型
    /// </summary>
    public class RealTimeProcessingResult<T>
    {
        public List<T> ProcessedOrders { get; set; } = new List<T>();
        public List<string> Errors { get; set; } = new List<string>();
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime CompletedAt { get; set; }
    }

    /// <summary>
    /// 合并流处理结果模型
    /// </summary>
    public class CombinedProcessingResult
    {
        public int TotalProcessed { get; set; }
        public int TotalErrors { get; set; }
        public List<string> ProcessingSteps { get; set; } = new List<string>();
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime CompletedAt { get; set; }
    }

    /// <summary>
    /// 条件批处理结果模型
    /// </summary>
    public class ProcessedBatchResult
    {
        public int BatchIndex { get; set; }
        public int ItemCount { get; set; }
        public DateTime ProcessedAt { get; set; }
        public List<Product> ProcessedItems { get; set; } = new List<Product>();
    }

    /// <summary>
    /// 条件处理结果模型
    /// </summary>
    public class ConditionalProcessingResult
    {
        public List<ProcessedBatchResult> BatchResults { get; set; } = new List<ProcessedBatchResult>();
        public int TotalBatches { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime CompletedAt { get; set; }
    }

    // 6. 异步数据过滤服务 - 生产级流过滤组件
    public class AsyncDataFilteringService
    {
        private readonly ILogger<AsyncDataFilteringService> _logger;

        public AsyncDataFilteringService(ILogger<AsyncDataFilteringService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 高级产品过滤 - 使用复杂的过滤逻辑
        /// </summary>
        public async Task<FilteredResult<Product>> FilterProductsAdvancedAsync(
            IAsyncEnumerable<Product> products,
            ProductFilterCriteria criteria,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting advanced product filtering with criteria");

            var result = new FilteredResult<Product>();

            try
            {
                // 使用System.Interactive.Async进行链式过滤

                // Where: 条件过滤
                // Distinct: 去重
                // OrderBy: 排序
                // Take: 限量获取
                // Skip: 跳过前几个结果
                var filteredStream = products
                    .Where(p => p.IsActive) // 只选择活跃产品
                    .Where(p => string.IsNullOrEmpty(criteria.Category) ||
                               p.Category.Equals(criteria.Category, StringComparison.OrdinalIgnoreCase)) // 分类过滤
                    .Where(p => p.Price >= criteria.MinPrice && p.Price <= criteria.MaxPrice) // 价格范围过滤
                    .DistinctBy(p => p.Id) // 根据ID去重
                    .OrderBy(p => p.Price) // 按价格排序
                    .ThenBy(p => p.Name) // 按名称排序
                    .Take(criteria.MaxResults) // 限制结果数量
                    .Skip(criteria.SkipCount); // 跳过前几个结果

                // 转换为列表并应用进一步处理
                result.FilteredItems = await filteredStream
                    .TakeUntil(cancellationToken)
                    .Select(p =>
                    {
                        // 数据转换和增强
                        p.Name = p.Name.ToUpper(); // 转换名称为大写
                        return p;
                    })
                    .ToListAsync(cancellationToken);

                result.TotalCount = result.FilteredItems.Count;
                result.IsSuccess = true;

                _logger.LogInformation("Advanced filtering completed - Found {ItemCount} items", result.TotalCount);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Advanced product filtering cancelled");
                result.Message = "Filtering cancelled";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during advanced product filtering");
                result.IsSuccess = false;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 时间窗口聚合处理 - 基于时间的流聚合
        /// </summary>
        public async Task<TimeWindowAggregationResult> AggregateByTimeWindowAsync(
            IAsyncEnumerable<Order> orders,
            TimeSpan windowSize,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting time window aggregation for orders");

            var result = new TimeWindowAggregationResult();

            try
            {
                // 使用GroupBy进行时间窗口分组
                var groupedOrders = await orders
                    .TakeUntil(cancellationToken)
                    .GroupBy(order =>
                        new DateTime(order.OrderDate.Year, order.OrderDate.Month, order.OrderDate.Day,
                                   order.OrderDate.Hour, order.OrderDate.Minute, 0)) // 按分钟分组
                    .ToListAsync(cancellationToken);

                foreach (var group in groupedOrders)
                {
                    var windowResult = new TimeWindowResult
                    {
                        WindowStart = group.Key,
                        WindowEnd = group.Key.Add(windowSize),
                        OrderCount = await group.CountAsync(cancellationToken),
                        TotalAmount = await group.SumAsync(o => o.Amount, cancellationToken),
                        Orders = await group.ToListAsync(cancellationToken)
                    };

                    result.Windows.Add(windowResult);

                    _logger.LogDebug("Aggregated time window {WindowStart} - Orders: {OrderCount}, Amount: {TotalAmount}",
                        windowResult.WindowStart, windowResult.OrderCount, windowResult.TotalAmount);
                }

                result.IsSuccess = true;
                result.TotalWindows = result.Windows.Count;

                _logger.LogInformation("Time window aggregation completed - {WindowCount} windows processed",
                    result.TotalWindows);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Time window aggregation cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during time window aggregation");
                result.IsSuccess = false;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 分组聚合处理 - 复杂的分组和统计
        /// </summary>
        public async Task<Dictionary<string, CategoryStats>> GetCategoryStatisticsAsync(
            IAsyncEnumerable<Product> products,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting category statistics aggregation");

            try
            {
                var categoryStats = await products
                    .TakeUntil(cancellationToken)
                    .Where(p => p.IsActive) // 只统计活跃产品
                    .GroupBy(p => p.Category) // 按分类分组
                    .ToDictionaryAsync(
                        group => group.Key, // 键
                        async group => new CategoryStats
                        {
                            CategoryName = group.Key,
                            ProductCount = await group.CountAsync(cancellationToken),
                            AveragePrice = await group.AverageAsync(p => p.Price, cancellationToken),
                            MinPrice = await group.MinAsync(p => p.Price, cancellationToken),
                            MaxPrice = await group.MaxAsync(p => p.Price, cancellationToken),
                            TotalStock = await group.SumAsync(p => p.StockCount, cancellationToken)
                        },
                        cancellationToken);

                _logger.LogInformation("Category statistics aggregation completed for {CategoryCount} categories",
                    categoryStats.Count);

                return categoryStats;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Category statistics aggregation cancelled");
                return new Dictionary<string, CategoryStats>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during category statistics aggregation");
                throw;
            }
        }
    }

    // 7. 数据过滤模型类
    public class ProductFilterCriteria
    {
        public string Category { get; set; }
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = decimal.MaxValue;
        public int SkipCount { get; set; } = 0;
        public int MaxResults { get; set; } = 100;
        public bool OnlyActive { get; set; } = true;
    }

    public class FilteredResult<T>
    {
        public bool IsSuccess { get; set; }
        public List<T> FilteredItems { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int FilteredCount => FilteredItems?.Count ?? 0;
        public string Message { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }

    public class TimeWindowAggregationResult
    {
        public bool IsSuccess { get; set; }
        public List<TimeWindowResult> Windows { get; set; } = new List<TimeWindowResult>();
        public int TotalWindows { get; set; }
        public string Message { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }

    public class TimeWindowResult
    {
        public DateTime WindowStart { get; set; }
        public DateTime WindowEnd { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
    }

    public class CategoryStats
    {
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int TotalStock { get; set; }
    }

    // 8. 异步流分页服务 - 生产级分页处理
    public class AsyncPagingService
    {
        private readonly ILogger<AsyncPagingService> _logger;

        public AsyncPagingService(ILogger<AsyncPagingService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 异步分页处理 - 使用System.Interactive.Async进行分页
        /// </summary>
        public async Task<PagedResult<T>> GetPagedResultsAsync<T>(
            IAsyncEnumerable<T> source,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting paged results retrieval - Page: {PageNumber}, Size: {PageSize}",
                pageNumber, pageSize);

            var result = new PagedResult<T>();

            try
            {
                // 计算跳过的项数
                var skipCount = (pageNumber - 1) * pageSize;

                // 使用System.Interactive.Async进行分页处理

                // Skip: 跳过前几项
                // Take: 获取指定数量的项
                // CountAsync: 统计总数
                result.Items = await source
                    .Skip(skipCount) // 跳过
                    .Take(pageSize)  // 取页大小
                    .ToListAsync(cancellationToken);

                result.PageNumber = pageNumber;
                result.PageSize = pageSize;
                result.TotalCount = await source.CountAsync(cancellationToken);
                result.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
                result.IsSuccess = true;

                _logger.LogInformation("Paging completed - Page {PageNumber}/{TotalPages}, Items: {ItemCount}",
                    pageNumber, result.TotalPages, result.Items.Count);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Paging operation cancelled");
                result.Message = "Operation cancelled";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during paged results retrieval");
                result.IsSuccess = false;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 流式分页处理 - 逐步加载页面数据
        /// </summary>
        public async IAsyncEnumerable<PagedResult<T>> StreamPagesAsync<T>(
            IAsyncEnumerable<T> source,
            int pageSize,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting streaming paged results - Page Size: {PageSize}", pageSize);

            var pageNumber = 1;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var pagedResult = await GetPagedResultsAsync(source, pageNumber, pageSize, cancellationToken);

                    if (pagedResult.Items.Any())
                    {
                        _logger.LogDebug("Yielding page {PageNumber} with {ItemCount} items",
                            pageNumber, pagedResult.Items.Count);

                        yield return pagedResult;
                        pageNumber++;
                    }
                    else
                    {
                        // 没有更多数据，结束流
                        _logger.LogInformation("Streaming paged results completed - Total pages: {TotalPages}",
                            pageNumber - 1);
                        break;
                    }

                    // 添加小延迟避免过度消耗资源
                    await Task.Delay(10, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Page streaming cancelled on page {PageNumber}", pageNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during page streaming on page {PageNumber}", pageNumber);
            }
        }
    }

    public class PagedResult<T>
    {
        public bool IsSuccess { get; set; }
        public List<T> Items { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string Message { get; set; }
        public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;

        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;
    }

    // 9. 异步流监控和遥测服务
    public interface IAsyncStreamTelemetryService
    {
        Task TrackStreamStatisticsAsync<T>(string streamName, IAsyncEnumerable<T> stream,
            CancellationToken cancellationToken);
        StreamMetrics GetStreamMetrics(string streamName);
        void ResetMetrics(string streamName);
    }

    public class AsyncStreamTelemetryService : IAsyncStreamTelemetryService
    {
        private readonly ILogger<AsyncStreamTelemetryService> _logger;
        private readonly ConcurrentDictionary<string, StreamMetrics> _streamMetrics =
            new ConcurrentDictionary<string, StreamMetrics>();

        public async Task TrackStreamStatisticsAsync<T>(
            string streamName,
            IAsyncEnumerable<T> stream,
            CancellationToken cancellationToken)
        {
            var metrics = _streamMetrics.GetOrAdd(streamName, _ => new StreamMetrics());

            try
            {
                metrics.StartTime = DateTime.UtcNow;

                // 跟踪流的处理统计
                await stream
                    .TakeUntil(cancellationToken)
                    .ForEachAsync(item =>
                    {
                        metrics.ProcessedItems++;

                        // 定期输出统计信息
                        if (metrics.ProcessedItems % 100 == 0)
                        {
                            _logger.LogInformation("STREAM_PROGRESS: {StreamName} - Processed: {ItemCount} items",
                                streamName, metrics.ProcessedItems);
                        }
                    });

                metrics.EndTime = DateTime.UtcNow;
                metrics.IsSuccess = true;
                metrics.Duration = metrics.EndTime - metrics.StartTime;

                _logger.LogInformation("STREAM_COMPLETE: {StreamName} - Total Processed: {ItemCount}, Duration: {Duration}",
                    streamName, metrics.ProcessedItems, metrics.Duration);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("STREAM_CANCELLED: {StreamName} - Processed: {ItemCount} items",
                    streamName, metrics.ProcessedItems);
            }
            catch (Exception ex)
            {
                metrics.IsSuccess = false;
                metrics.Errors++;
                metrics.LastError = ex.Message;
                _logger.LogError(ex, "STREAM_ERROR: {StreamName} - Processed: {ItemCount}, Error: {ErrorMessage}",
                    streamName, metrics.ProcessedItems, ex.Message);
            }
        }

        public StreamMetrics GetStreamMetrics(string streamName)
        {
            return _streamMetrics.TryGetValue(streamName, out var metrics) ? metrics : new StreamMetrics();
        }

        public void ResetMetrics(string streamName)
        {
            _streamMetrics.TryRemove(streamName, out _);
            _logger.LogInformation("Stream metrics reset for {StreamName}", streamName);
        }
    }

    public class StreamMetrics
    {
        public string StreamName { get; set; }
        public long ProcessedItems { get; set; }
        public long Errors { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsSuccess { get; set; }
        public string LastError { get; set; }

        public double ProcessingRate => Duration.TotalSeconds > 0 ?
            ProcessedItems / Duration.TotalSeconds : 0;

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    // 10. 流处理中间件 - 生产环境应用级处理
    public class StreamProcessingMiddleware
    {
        private readonly ILogger<StreamProcessingMiddleware> _logger;

        public StreamProcessingMiddleware(ILogger<StreamProcessingMiddleware> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 处理流并包装在中间件中，添加错误处理和遥测
        /// </summary>
        public async Task<T> ProcessStreamWithMiddlewareAsync<T>(
            IAsyncEnumerable<T> stream,
            Func<T, Task<T>> processor,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Stream processing middleware started");

            try
            {
                // 使用System.Interactive.Async进行流处理

                // Select: 同步转换
                // SelectMany: 异步转换和扩展
                // Catch: 异常处理
                // Finally: 清理操作
                var processedStream = stream
                    .TakeUntil(cancellationToken)
                    .SelectMany(async item =>
                    {
                        try
                        {
                            await Task.Delay(5, cancellationToken); // 模拟处理延迟
                            return await processor(item);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error processing item, skipping");
                            return default(T); // 返回默认值或null
                        }
                    })
                    .Where(item => item != null) // 过滤掉错误处理项
                    .Finally(() =>
                    {
                        _logger.LogInformation("Stream processing completed");
                    });

                // 获取第一个处理成功的结果
                var firstResult = await processedStream.FirstOrDefaultAsync(cancellationToken);

                if (firstResult != null)
                {
                    _logger.LogInformation("First stream item processed successfully");
                }
                else
                {
                    _logger.LogWarning("No items were successfully processed from stream");
                }

                return firstResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stream processing middleware error");
                throw;
            }
        }

        /// <summary>
        /// 流批量处理中间件
        /// </summary>
        public async Task<BatchProcessingResult<T>> ProcessStreamInBatchesAsync<T>(
            IAsyncEnumerable<T> stream,
            Func<List<T>, Task<BatchProcessingResult<T>>> batchProcessor,
            int batchSize = 50,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Batch stream processing middleware started - Batch Size: {BatchSize}", batchSize);

            var result = new BatchProcessingResult<T>();

            try
            {
                await stream
                    .TakeUntil(cancellationToken)
                    .Buffer(batchSize) // 分批
                    .SelectMany(async batch =>
                    {
                        _logger.LogDebug("Processing batch of {BatchSize} items", batch.Count);

                        var batchResult = await batchProcessor(batch.ToList());
                        return batchResult;
                    })
                    .ForEachAsync(batchResult =>
                    {
                        result.ProcessedBatches += batchResult.ProcessedBatches;
                        result.TotalItems += batchResult.TotalItems;
                        result.Errors.AddRange(batchResult.Errors);

                        _logger.LogInformation("Batch processed - Items: {Itemcount}, Errors: {ErrorCount}",
                            batchResult.TotalItems, batchResult.ErrorCount);
                    });

                result.IsSuccess = result.Errors.Count == 0;
                result.ProcessingEnd = DateTime.UtcNow;

                _logger.LogInformation("Batch stream processing completed - Total Items: {TotalItems}, Errors: {ErrorCount}",
                    result.TotalItems, result.ErrorCount);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Batch stream processing error");
                result.IsSuccess = false;
                result.Errors.Add($"Batch processing error: {ex.Message}");
                return result;
            }
        }
    }

    public class BatchProcessingResult<T>
    {
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public int ErrorCount => Errors.Count;
        public int ProcessedBatches { get; set; }
        public int TotalItems { get; set; }
        public List<T> ProcessedItems { get; set; } = new List<T>();
        public DateTime ProcessingStart { get; set; } = DateTime.UtcNow;
        public DateTime ProcessingEnd { get; set; }

        public TimeSpan ProcessingDuration => ProcessingEnd - ProcessingStart;
    }

    // 11. 流式数据转换服务
    public class AsyncStreamTransformService
    {
        private readonly ILogger<AsyncStreamTransformService> _logger;

        public async Task<IAsyncEnumerable<TDestination>> TransformStream<TSource, TDestination>(
            IAsyncEnumerable<TSource> source,
            Func<TSource, TDestination> transformer,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting stream transformation from {SourceType} to {DestinationType}",
                typeof(TSource).Name, typeof(TDestination).Name);

            // 使用System.Interactive.Async进行流式转换
            return source
                .TakeUntil(cancellationToken)
                .Select(transformer) // 同步转换
                .Take(1000) // 限制结果数量
                .Finally(() =>
                {
                    _logger.LogInformation("Stream transformation completed");
                });
        }

        public async Task<IAsyncEnumerable<TDestination>> TransformStreamAsync<TSource, TDestination>(
            IAsyncEnumerable<TSource> source,
            Func<TSource, Task<TDestination>> asyncTransformer,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting async stream transformation from {SourceType} to {DestinationType}",
                typeof(TSource).Name, typeof(TDestination).Name);

            // 使用System.Interactive.Async进行异步流式转换
            return source
                .TakeUntil(cancellationToken)
                .SelectMany(async item =>
                {
                    var transformed = await asyncTransformer(item);
                    return new[] { transformed }.ToAsyncEnumerable(); // 转换为异步流
                })
                .Finally(() =>
                {
                    _logger.LogInformation("Async stream transformation completed");
                });
        }
    }

    // 12. 主程序演示类
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            Console.WriteLine("System.Interactive.Async Production Demo");
            Console.WriteLine("========================================");
            Console.WriteLine();

            #region 服务配置

            // 配置日志服务
            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });

            // 注册核心服务
            builder.Services.AddSingleton<IAsyncDataService, AsyncDataService>();
            builder.Services.AddSingleton<AsyncDataProcessingService>();
            builder.Services.AddSingleton<AsyncDataFilteringService>();
            builder.Services.AddSingleton<AsyncPagingService>();
            builder.Services.AddSingleton<IAsyncStreamTelemetryService, AsyncStreamTelemetryService>();
            builder.Services.AddSingleton<StreamProcessingMiddleware>();
            builder.Services.AddSingleton<AsyncStreamTransformService>();

            #endregion

            var host = builder.Build();
            await RunDemoAsync(host.Services);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task RunDemoAsync(IServiceProvider services)
        {
            var dataService = services.GetRequiredService<IAsyncDataService>();
            var processingService = services.GetRequiredService<AsyncDataProcessingService>();
            var filteringService = services.GetRequiredService<AsyncDataFilteringService>();
            var pagingService = services.GetRequiredService<AsyncPagingService>();
            var telemetryService = services.GetRequiredService<IAsyncStreamTelemetryService>();
            var middleware = services.GetRequiredService<StreamProcessingMiddleware>();
            var transformService = services.GetRequiredService<AsyncStreamTransformService>();

            Console.WriteLine("1. Basic Stream Processing:");
            var products = dataService.GetProductsAsync();
            var processingResult = await processingService.ProcessProductsAsync(20);
            Console.WriteLine($"   Processed {processingResult.TotalProcessed} products with {processingResult.TotalErrors} errors");

            Console.WriteLine("\n2. Advanced Filtering Demo:");
            var filterResult = await filteringService.FilterProductsAdvancedAsync(
                dataService.GetProductsAsync(),
                new ProductFilterCriteria
                {
                    Category = "Electronics",
                    MinPrice = 50,
                    MaxPrice = 500,
                    MaxResults = 10
                });
            Console.WriteLine($"   Filtered {filterResult.TotalCount} electronic products");

            Console.WriteLine("\n3. Paging Demo:");
            var pagedResult = await pagingService.GetPagedResultsAsync(
                dataService.GetProductsAsync(),
                2, 10); // 第2页，每页10个
            Console.WriteLine($"   Retrieved page {pagedResult.PageNumber} with {pagedResult.Items.Count} items");
            Console.WriteLine($"   Total pages: {pagedResult.TotalPages}");

            Console.WriteLine("\n4. Aggregation Demo:");
            var categoryStats = await filteringService.GetCategoryStatisticsAsync(
                dataService.GetProductsAsync());
            foreach (var stat in categoryStats.Take(3))
            {
                Console.WriteLine($"   {stat.Key}: {stat.Value.ProductCount} products, avg price: ${stat.Value.AveragePrice:F2}");
            }

            Console.WriteLine("\n5. Async Transformation Demo:");
            var transformedStream = await transformService.TransformStream(dataService.GetProductsAsync(),
                p => new { p.Name, IsExpensive = p.Price > 100 });
            var transformedList = await transformedStream.Take(5).ToListAsync();
            foreach (var item in transformedList)
            {
                Console.WriteLine($"   {item.Name}: {(item.IsExpensive ? "Expensive" : "Affordable")}");
            }

            Console.WriteLine("\n6. Streaming Pages Demo:");
            var pageStream = pagingService.StreamPagesAsync(dataService.GetOrdersAsync(), 25);
            await pageStream.Take(3).ForEachAsync(page =>
            {
                Console.WriteLine($"   Streamed page {page.PageNumber} with {page.Items.Count} orders");
            });

            Console.WriteLine("\n7. Middleware Processing Demo:");
            var productsStream = dataService.GetProductsAsync();
            var batchResult = await middleware.ProcessStreamInBatchesAsync(
                productsStream,
                async batch =>
                {
                    await Task.Delay(100);
                    return new BatchProcessingResult<Product>
                    {
                        ProcessedBatches = 1,
                        TotalItems = batch.Count,
                        ProcessedItems = batch
                    };
                },
                30);
            Console.WriteLine($"   Middleware processed {batchResult.TotalItems} items in {batchResult.ProcessedBatches} batches");

            Console.WriteLine("\n8. Telemetry Demo:");
            var metricsTask = telemetryService.TrackStreamStatisticsAsync(
                "ProductsTelemetry",
                dataService.GetProductsAsync(),
                CancellationToken.None);
            Console.WriteLine("   Telemetry tracking started...");
            await metricsTask;
            var metrics = telemetryService.GetStreamMetrics("ProductsTelemetry");
            Console.WriteLine($"   Stream processed {metrics.ProcessedItems} items in success");
        }

        public void Test()
        {
            // 1. 数据分批操作
            // Buffer: 将元素收集到缓冲区中
            await stream.Buffer(10)
                .ForEachAsync(batch => Console.WriteLine($"Processing batch of {batch.Count} items"));

            // Window: 基于时间或数量的窗口分组
            await stream.Window(5) // 每5个元素一个窗口
                .SelectMany(window => window.ToListAsync())
                .ForEachAsync(batch => ProcessBatch(batch));

            // 数据转换操作
            // Select: 同步数据转换
            var transformedStream = stream.Select(item => item * 2);

            // SelectMany: 异步数据扩展和转换
            var expandedStream = stream.SelectMany(async item =>
            {
                var result = await ProcessItemAsync(item);
                return new[] { result }.ToAsyncEnumerable();
            });

            // 数据过滤操作
            // Where: 条件过滤
            var filteredStream = stream.Where(item => item > 0);

            // Distinct: 去重
            var uniqueStream = stream.Distinct();

            // DistinctBy: 按特定属性去重
            var uniqueProducts = productsStream.DistinctBy(p => p.Id);

            // 排序和限制操作
            // OrderBy: 排序
            var sortedStream = stream.OrderBy(item => item.Name);

            // Take: 限量获取
            var limitedStream = stream.Take(100);

            // Skip: 跳过元素
            var pagedStream = stream.Skip(50).Take(25);

            // 流控制操作
            // TakeWhile: 满足条件时继续获取
            await stream.TakeWhile(item => item.IsActive)
                .ForEachAsync(item => ProcessActiveItem(item));

            // TakeUntil: 直到取消令牌触发为止
            await stream.TakeUntil(cancellationToken)
                .ForEachAsync(item => ProcessItem(item));

            // Finally: 流处理完成时的清理操作
            await stream.Finally(() => Console.WriteLine("Processing completed"))
                .ForEachAsync(item => ProcessItem(item));

            // 聚合统计操作
            // CountAsync: 异步计数
            var count = await stream.CountAsync();

            // SumAsync: 异步求和
            var sum = await stream.SumAsync(item => item.Amount);

            // AverageAsync: 异步平均值
            var average = await stream.AverageAsync(item => item.Value);

            // GroupBy: 异步分组
            var grouped = await st

            // 错误处理
            var safeStream = stream
                .Catch((Exception ex) =>
                {
                    _logger.LogError(ex, "Stream processing error");
                    return AsyncEnumerable.Empty<T>(); // 返回空流继续执行
                })
                .Retry(3); // 最多重试3次

            // 监控遥测
            await stream
                .Do(item => _telemetry.TrackItemProcessed()) // 插入监控点
                .ForEachAsync(item => ProcessItem(item));
        }
    }

    // 生产环境应用模式
    // 性能优化：
    // 使用缓冲和批量处理减少处理开销
    // 适当的并发控制避免资源浪费
    // 及时的取消令牌传递避免不必要的处理
}