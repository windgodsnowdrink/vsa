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
using Microsoft.IO;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Buffers;

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

    // 1. RecyclableMemoryStream管理器配置类
    public class MemoryStreamManagerConfiguration
    {
        /// <summary>
        /// 创建RecyclableMemoryStreamManager实例的配置方法
        /// </summary>
        /// <returns>配置好的RecyclableMemoryStreamManager</returns>
        public static RecyclableMemoryStreamManager CreateMemoryStreamManager()
        {
            // 最小块大小 - 每个内存块的大小（建议2的幂次）
            const int blockSize = 1024; // 1KB

            // 大对象池阈值 - 当对象超过此大小时，会进入专用的大对象池
            const int largeBufferMultiple = 1024 * 1024; // 1MB

            // 最大缓冲区大小 - 能够分配的最大缓冲区大小
            const int maximumBufferSize = 1024 * 1024 * 128; // 128MB

            // 创建内存流管理器
            var memoryManager = new RecyclableMemoryStreamManager(
                blockSize,
                largeBufferMultiple,
                maximumBufferSize,
                useExponentialLargeBuffer: true); // 使用指数增长的大缓冲区策略

            #region 配置内存流管理器选项

            // 启用额外的统计信息收集
            memoryManager.GenerateCallStacks = false; // 生产环境中通常不启用，影响性能
            memoryManager.AggressiveBufferReturn = true; // 立即返回缓冲区而不是等待GC

            #endregion

            #region 配置事件处理

            // 监听内存流创建事件
            memoryManager.StreamCreated += (sender, args) =>
            {
                Console.WriteLine($"Stream created - Id: {args.StreamId}, Tag: {args.Tag}");
            };

            // 监听内存流销毁事件
            memoryManager.StreamDisposed += (sender, args) =>
            {
                Console.WriteLine($"Stream disposed - Id: {args.StreamId}, Tag: {args.Tag}, " +
                                $"AllocationStack: {args.AllocationStack}, DisposeStack: {args.DisposeStack}");
            };

            // 监听大对象分配事件
            memoryManager.LargeBufferCreated += (sender, args) =>
            {
                Console.WriteLine($"Large buffer created - StreamId: {args.StreamId}, " +
                                $"Size: {args.Size}, Tag: {args.Tag}");
            };

            // 监听缓冲区使用情况（用于监控和调优）
            memoryManager.BufferDiscarded += (sender, args) =>
            {
                Console.WriteLine($"Buffer discarded - StreamId: {args.StreamId}, " +
                                $"Reason: {args.Reason}, Tag: {args.Tag}");
            };

            #endregion

            return memoryManager;
        }
    }

    // 2. 生产级文件处理服务类
    public class FileProcessingService
    {
        private readonly RecyclableMemoryStreamManager _memoryManager;

        public FileProcessingService(RecyclableMemoryStreamManager memoryManager)
        {
            _memoryManager = memoryManager;
        }

        /// <summary>
        /// 高效处理大文件读取 - 使用RecyclableMemoryStream减少GC压力
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="chunkSize">处理块大小</param>
        /// <returns>处理结果</returns>
        public async Task<string> ProcessLargeFileAsync(string filePath, int chunkSize = 4096)
        {
            // 创建可回收的内存流，使用标签便于监控追踪
            using var memoryStream = _memoryManager.GetStream("FileProcessingService.ProcessLargeFile");

            try
            {
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                var buffer = new byte[chunkSize];
                int bytesRead;

                // 分块读取大文件内容
                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await memoryStream.WriteAsync(buffer, 0, bytesRead);
                }

                // 转换为字符串返回
                memoryStream.Position = 0;
                using var reader = new StreamReader(memoryStream, Encoding.UTF8);
                return await reader.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
                throw;
            }
            // memoryStream会在此处自动归还到池中
        }

        /// <summary>
        /// 处理多个文件的批处理操作
        /// </summary>
        /// <param name="filePaths">文件路径集合</param>
        /// <returns>合并后的文件内容</returns>
        public async Task<byte[]> BatchProcessFilesAsync(string[] filePaths)
        {
            // 使用池化内存流进行批处理
            using var batchStream = _memoryManager.GetStream("FileProcessingService.BatchProcessFiles");

            foreach (var filePath in filePaths)
            {
                try
                {
                    using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                    // 添加文件分隔符
                    var separator = Encoding.UTF8.GetBytes($"\n=== Content of {Path.GetFileName(filePath)} ===\n");
                    await batchStream.WriteAsync(separator, 0, separator.Length);

                    // 复制文件内容到批处理流
                    await fileStream.CopyToAsync(batchStream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
                    // 继续处理其他文件，不中断整个批处理流程
                }
            }

            // 返回字节数组
            return batchStream.ToArray();
        }

        /// <summary>
        /// 安全处理可能的超大文件，防止单个流占用过多内存
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="maxSize">最大允许大小</param>
        /// <returns>处理结果</returns>
        public async Task<byte[]> ProcessFileWithSizeLimit(string filePath, long maxSize = 10 * 1024 * 1024) // 10MB
        {
            using var stream = _memoryManager.GetStream("FileProcessingService.ProcessWithSizeLimit");

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // 检查文件大小，避免创建过大的内存流
            if (fileStream.Length > maxSize)
            {
                throw new InvalidOperationException($"File size {fileStream.Length} exceeds maximum allowed size {maxSize}");
            }

            await fileStream.CopyToAsync(stream);
            return stream.ToArray();
        }
    }

    // 3. 网络数据处理服务类
    public class NetworkDataService
    {
        private readonly RecyclableMemoryStreamManager _memoryManager;

        public NetworkDataService(RecyclableMemoryStreamManager memoryManager)
        {
            _memoryManager = memoryManager;
        }

        /// <summary>
        /// 处理HTTP响应数据
        /// </summary>
        /// <param name="responseStream">HTTP响应流</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>响应内容的字节数组</returns>
        public async Task<byte[]> ProcessHttpResponseAsync(Stream responseStream,
            System.Threading.CancellationToken cancellationToken = default)
        {
            // 使用带标签的池化内存流处理网络响应
            using var pooledStream = _memoryManager.GetStream("NetworkDataService.ProcessHTTPResponse");

            await responseStream.CopyToAsync(pooledStream, cancellationToken);
            return pooledStream.ToArray();
        }

        /// <summary>
        /// 处理WebSocket消息数据
        /// </summary>
        /// <param name="messageData">消息数据</param>
        /// <param name="messageType">消息类型</param>
        /// <returns>处理后的消息</returns>
        public async Task<string> ProcessWebSocketMessageAsync(byte[] messageData, string messageType)
        {
            // 使用可回收内存流处理WebSocket数据
            using var stream = _memoryManager.GetStream("NetworkDataService.ProcessWebSocketMessage", messageData);
            using var reader = new StreamReader(stream, Encoding.UTF8);

            return await reader.ReadToEndAsync();
        }

        /// <summary>
        /// 创建分块传输的数据流
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="chunkSize">分块大小</param>
        /// <returns>分块数据集合</returns>
        public async Task<byte[][]> CreateChunkedStreamsAsync(byte[] data, int chunkSize = 8192)
        {
            var chunks = new List<byte[]>();
            var offset = 0;

            while (offset < data.Length)
            {
                var currentChunkSize = Math.Min(chunkSize, data.Length - offset);

                // 每个块使用独立的可回收内存流
                using var chunkStream = _memoryManager.GetStream("NetworkDataService.CreateChunkedStreams");
                await chunkStream.WriteAsync(data, offset, currentChunkSize);

                chunks.Add(chunkStream.ToArray());
                offset += currentChunkSize;
            }

            return chunks.ToArray();
        }
    }

    // 4. 数据序列化服务类
    public class SerializationService
    {
        private readonly RecyclableMemoryStreamManager _memoryManager;

        public SerializationService(RecyclableMemoryStreamManager memoryManager)
        {
            _memoryManager = memoryManager;
        }

        /// <summary>
        /// JSON序列化对象为字节数组，使用池化内存减少GC压力
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">要序列化的对象</param>
        /// <returns>JSON字节数组</returns>
        public byte[] SerializeToJson<T>(T obj)
        {
            // 使用可回收内存流进行序列化
            using var stream = _memoryManager.GetStream("SerializationService.SerializeToJson");

            var json = System.Text.Json.JsonSerializer.Serialize(obj);
            var jsonBytes = Encoding.UTF8.GetBytes(json);

            stream.Write(jsonBytes, 0, jsonBytes.Length);
            return stream.ToArray();
        }

        /// <summary>
        /// JSON反序列化字节数组为对象，使用池化内存流
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="jsonBytes">JSON字节数组</param>
        /// <returns>反序列化的对象</returns>
        public T DeserializeFromJson<T>(byte[] jsonBytes)
        {
            // 使用可回收内存流进行反序列化
            using var stream = _memoryManager.GetStream("SerializationService.DeserializeFromJson", jsonBytes);
            using var reader = new StreamReader(stream, Encoding.UTF8);

            var json = reader.ReadToEnd();
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// 处理大批量数据序列化，避免单次操作占用过多内存
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="items">对象集合</param>
        /// <param name="batchSize">批量大小</param>
        /// <returns>序列化后的字节数据集合</returns>
        public async Task<byte[][]> BatchSerializeAsync<T>(T[] items, int batchSize = 1000)
        {
            var batches = new List<byte[]>();

            for (int i = 0; i < items.Length; i += batchSize)
            {
                // 每批数据使用独立的内存流
                using var batchStream = _memoryManager.GetStream("SerializationService.BatchSerialize");

                var currentBatch = items.Skip(i).Take(batchSize);
                var json = System.Text.Json.JsonSerializer.Serialize(currentBatch);
                var jsonBytes = Encoding.UTF8.GetBytes(json);

                await batchStream.WriteAsync(jsonBytes, 0, jsonBytes.Length);
                batches.Add(batchStream.ToArray());
            }

            return batches.ToArray();
        }
    }

    // 5. 缓冲池监控服务类
    public class MemoryStreamMonitoringService
    {
        private readonly RecyclableMemoryStreamManager _memoryManager;

        public MemoryStreamMonitoringService(RecyclableMemoryStreamManager memoryManager)
        {
            _memoryManager = memoryManager;
        }

        /// <summary>
        /// 打印内存流池的统计信息
        /// </summary>
        public void PrintPoolStatistics()
        {
            var stats = _memoryManager.GetMemoryReport();

            Console.WriteLine("=== RecyclableMemoryStream Pool Statistics ===");
            Console.WriteLine($"Small pool free size: {stats.SmallPoolFreeSize} bytes");
            Console.WriteLine($"Small pool in use size: {stats.SmallPoolInUseSize} bytes");
            Console.WriteLine($"Large pool free size: {stats.LargePoolFreeSize} bytes");
            Console.WriteLine($"Large pool in use size: {stats.LargePoolInUseSize} bytes");
            Console.WriteLine($"Small pool free blocks: {stats.SmallPoolFreeBlocks}");
            Console.WriteLine($"Small pool in use blocks: {stats.SmallPoolInUseBlocks}");
            Console.WriteLine($"Large pool free buffers: {stats.LargePoolFreeBuffers}");
            Console.WriteLine($"Large pool in use buffers: {stats.LargePoolInUseBuffers}");
            Console.WriteLine($"Total allocated: {stats.TotalAllocatedBytes} bytes");
            Console.WriteLine($"Total free: {stats.TotalFreeBytes} bytes");
            Console.WriteLine();
        }

        /// <summary>
        /// 检查内存池使用情况，用于预警
        /// </summary>
        /// <returns>内存池健康状况</returns>
        public bool CheckMemoryPoolHealth()
        {
            var stats = _memoryManager.GetMemoryReport();

            // 检查小型池使用率
            var smallPoolUsageRatio = (double)stats.SmallPoolInUseBlocks /
                (stats.SmallPoolInUseBlocks + stats.SmallPoolFreeBlocks);

            // 检查大型池使用率
            var largePoolUsageRatio = (double)stats.LargePoolInUseBuffers /
                (stats.LargePoolInUseBuffers + stats.LargePoolFreeBuffers);

            Console.WriteLine($"Small pool usage ratio: {smallPoolUsageRatio:P2}");
            Console.WriteLine($"Large pool usage ratio: {largePoolUsageRatio:P2}");

            // 如果使用率过高，可能存在内存泄漏
            if (smallPoolUsageRatio > 0.8 || largePoolUsageRatio > 0.8)
            {
                Console.WriteLine("Warning: High memory pool usage detected!");
                return false;
            }

            return true;
        }
    }

    // 6. 生产级使用示例服务类
    public class ProductionUsageDemo
    {
        private readonly RecyclableMemoryStreamManager _memoryManager;
        private readonly FileProcessingService _fileService;
        private readonly NetworkDataService _networkService;
        private readonly MemoryStreamMonitoringService _monitoringService;

        public ProductionUsageDemo()
        {
            // 创建内存管理器
            _memoryManager = MemoryStreamManagerConfiguration.CreateMemoryStreamManager();

            // 初始化相关服务
            _fileService = new FileProcessingService(_memoryManager);
            _networkService = new NetworkDataService(_memoryManager);
            _monitoringService = new MemoryStreamMonitoringService(_memoryManager);
        }

        /// <summary>
        /// 演示文件处理场景
        /// </summary>
        public async Task DemoFileProcessing()
        {
            Console.WriteLine("=== File Processing Demo ===");

            try
            {
                // 模拟创建一个测试文件
                var testContent = "This is test content for RecyclableMemoryStream demo.\n" +
                                "It shows how to efficiently process file operations without\n" +
                                "creating unnecessary garbage collection pressure.";
                var testFilePath = "testfile.txt";
                await File.WriteAllTextAsync(testFilePath, testContent);

                // 处理文件
                var result = await _fileService.ProcessLargeFileAsync(testFilePath);
                Console.WriteLine($"File content length: {result.Length}");

                // 清理测试文件
                File.Delete(testFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Demo file processing error: {ex.Message}");
            }
        }

        /// <summary>
        /// 演示网络数据处理场景
        /// </summary>
        public async Task DemoNetworkDataProcessing()
        {
            Console.WriteLine("\n=== Network Data Processing Demo ===");

            try
            {
                // 模拟网络数据
                var networkData = Encoding.UTF8.GetBytes(
                    "Simulated network response data with varying sizes");

                using var sourceStream = new MemoryStream(networkData);
                var processedData = await _networkService.ProcessHttpResponseAsync(sourceStream);

                Console.WriteLine($"Processed {processedData.Length} bytes of network data");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Demo network processing error: {ex.Message}");
            }
        }

        /// <summary>
        /// 演示序列化场景
        /// </summary>
        public void DemoSerialization()
        {
            Console.WriteLine("\n=== Serialization Demo ===");

            var testObject = new { Id = 1, Name = "Test User", Timestamp = DateTime.Now };

            try
            {
                // 序列化对象
                var serializedBytes = _memoryManager.GetStream("DemoSerialization.Serialize");
                using (serializedBytes)
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(testObject);
                    var jsonBytes = Encoding.UTF8.GetBytes(json);
                    serializedBytes.Write(jsonBytes, 0, jsonBytes.Length);

                    Console.WriteLine($"Serialized object to {serializedBytes.Length} bytes");

                    // 反序列化对象
                    serializedBytes.Position = 0;
                    var deserializedJson = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(
                        serializedBytes.ToArray());

                    Console.WriteLine($"Deserialized object with {deserializedJson.Count} properties");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Demo serialization error: {ex.Message}");
            }
        }

        /// <summary>
        /// 演示内存池监控
        /// </summary>
        public void DemoMemoryPoolMonitoring()
        {
            Console.WriteLine("\n=== Memory Pool Monitoring Demo ===");

            // 执行多个池化操作以查看统计变化
            for (int i = 0; i < 10; i++)
            {
                using var stream = _memoryManager.GetStream($"Demo.Monitoring.Stream.{i}");
                var testData = new byte[1000];
                new Random().NextBytes(testData);
                stream.Write(testData, 0, testData.Length);
            }

            // 打印统计信息
            _monitoringService.PrintPoolStatistics();
            _monitoringService.CheckMemoryPoolHealth();
        }

        /// <summary>
        /// 演示性能基准测试
        /// </summary>
        public void PerformanceBenchmark()
        {
            Console.WriteLine("\n=== Performance Benchmark Demo ===");

            const int iterations = 10000;
            const int dataLength = 1000;

            // 使用RecyclableMemoryStream的性能测试
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                using var stream = _memoryManager.GetStream("PerformanceBenchmark.Recyclable");
                var data = new byte[dataLength];
                stream.Write(data, 0, data.Length);
            }
            sw.Stop();
            Console.WriteLine($"RecyclableMemoryStream: {sw.ElapsedMilliseconds}ms for {iterations} iterations");

            // 使用常规MemoryStream的性能测试（会产生更多GC压力）
            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                using var stream = new MemoryStream();
                var data = new byte[dataLength];
                stream.Write(data, 0, data.Length);
            }
            sw.Stop();
            Console.WriteLine($"Regular MemoryStream: {sw.ElapsedMilliseconds}ms for {iterations} iterations");
        }
    }

    // 7. 工厂模式封装
    public static class MemoryStreamFactory
    {
        private static readonly RecyclableMemoryStreamManager _memoryManager;

        static MemoryStreamFactory()
        {
            // 在静态构造函数中初始化内存管理器
            _memoryManager = new RecyclableMemoryStreamManager(
                blockSize: 1024,
                largeBufferMultiple: 1024 * 1024,
                maximumBufferSize: 1024 * 1024 * 128);
        }

        /// <summary>
        /// 获取命名的可回收内存流
        /// </summary>
        /// <param name="tag">标签，用于追踪和调试</param>
        /// <returns>RecyclableMemoryStream实例</returns>
        public static RecyclableMemoryStream GetStream(string tag)
        {
            return _memoryManager.GetStream(tag);
        }

        /// <summary>
        /// 获取带初始数据的可回收内存流
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="initialData">初始数据</param>
        /// <returns>RecyclableMemoryStream实例</returns>
        public static RecyclableMemoryStream GetStream(string tag, byte[] initialData)
        {
            return _memoryManager.GetStream(tag, initialData);
        }

        /// <summary>
        /// 获取空的可回收内存流
        /// </summary>
        /// <returns>RecyclableMemoryStream实例</returns>
        public static RecyclableMemoryStream GetStream()
        {
            return _memoryManager.GetStream();
        }
    }

    // 8. ASP.NET Core集成示例
    public class StartupConfiguration
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            // 在依赖注入容器中注册MemoryStreamManager
            services.AddSingleton<RecyclableMemoryStreamManager>(provider =>
            {
                var manager = MemoryStreamManagerConfiguration.CreateMemoryStreamManager();

                // 生产环境下可以禁用调用堆栈生成以提升性能
#if DEBUG
            manager.GenerateCallStacks = true;
#else
                manager.GenerateCallStacks = false;
#endif

                return manager;
            });

            // 注册相关服务
            services.AddTransient<FileProcessingService>();
            services.AddTransient<NetworkDataService>();
            services.AddTransient<SerializationService>();
            services.AddTransient<MemoryStreamMonitoringService>();
        }
    }

    // 9. 程序入口点
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Microsoft.IO.RecyclableMemoryStream Production Demo");
            Console.WriteLine("==================================================\n");

            var demo = new ProductionUsageDemo();

            // 演示文件处理
            await demo.DemoFileProcessing();

            // 演示网络数据处理
            await demo.DemoNetworkDataProcessing();

            // 演示序列化
            demo.DemoSerialization();

            // 演示内存池监控
            demo.DemoMemoryPoolMonitoring();

            // 演示性能基准测试
            demo.PerformanceBenchmark();

            Console.WriteLine("\n=== Demo Complete ===");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    // 10. 高级功能示例 - 定制内存池策略
    public class AdvancedMemoryStreamManager : RecyclableMemoryStreamManager
    {
        // 自定义内存池回收策略
        private readonly Queue<byte[]> _customLargePool = new Queue<byte[]>();

        public AdvancedMemoryStreamManager() : base(1024, 1024 * 1024, 1024 * 1024 * 128)
        {
            // 配置自定义选项
            this.GenerateCallStacks = false;
            this.AggressiveBufferReturn = true;
            this.MaximumFreeSmallPoolBytes = 1024 * 1024 * 100; // 100MB小池上限
            this.MaximumFreeLargePoolBytes = 1024 * 1024 * 50;  // 50MB大池上限
        }

        /// <summary>
        /// 自定义获取大型缓冲区的方法
        /// </summary>
        /// <param name="size">请求的缓冲区大小</param>
        /// <param name="tag">标签</param>
        /// <returns>字节数组</returns>
        public byte[] GetCustomLargeBuffer(int size, string tag = null)
        {
            if (_customLargePool.Count > 0 && _customLargePool.Peek().Length >= size)
            {
                return _customLargePool.Dequeue();
            }

            return new byte[size];
        }

        /// <summary>
        /// 自定义归还大型缓冲区的方法
        /// </summary>
        /// <param name="buffer">要归还的缓冲区</param>
        /// <param name="tag">标签</param>
        public void ReturnCustomLargeBuffer(byte[] buffer, string tag = null)
        {
            // 简单的缓存策略：保留最近使用的10个大缓冲区
            if (_customLargePool.Count < 10)
            {
                _customLargePool.Enqueue(buffer);
            }
            // 超出容量的缓冲区会被GC回收
        }
    }

    // 核心优势
    // 回收内存池，避免频繁GC
    // 提供更可预测的内存使用情况
    // 支持详细的内存使用追踪和监控

    // 生产级配置要点
    // BlockSize: 小内存块大小，建议1024或4096字节
    // LargeBufferMultiple: 大对象池阈值，默认1MB
    // MaximumBufferSize: 允许的最大缓冲区大小
    // GenerateCallStacks: 调用堆栈生成（调试用，生产环境通常关闭）
    // AggressiveBufferReturn: 是否立即归还缓冲区

    // 内存管理最佳实践
    // 正确使用using语句确保内存流正确归还
    // using var stream = memoryManager.GetStream("MyOperation.Tag");
    // 流操作...
    // 流自动归还到池中
    // 适用场景
    // 文件处理: 大文件读写，减少GC压力
    // 网络传输: HTTP响应、WebSocket消息处理
    // 序列化操作: JSON/XML序列化反序列化
    // 缓存数据: 临时数据存储和转换
    // 批量操作: 大批量数据处理

    // 监控和诊断
    // 事件追踪: StreamCreated、StreamDisposed等事件
    // 统计报告: GetMemoryReport()获取详细统计信息
    // 内存使用监控: 避免内存泄漏和过度使用
}