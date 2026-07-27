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
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text.Json;
using System.IO;

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

    // 1. 数据模型定义
    public class ProcessingRequest
    {
        public string RequestId { get; set; } = Guid.NewGuid().ToString("N");
        public string DataType { get; set; }
        public object Data { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int Priority { get; set; } = 0;
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        public bool IsValid => !string.IsNullOrEmpty(DataType) && Data != null;
        public TimeSpan Age => DateTime.UtcNow - CreatedAt;
    }

    public class ProcessingResult<T>
    {
        public string RequestId { get; set; }
        public bool Success { get; set; }
        public T Result { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
        public TimeSpan ProcessingDuration { get; set; }
        public Dictionary<string, object> Metrics { get; set; } = new Dictionary<string, object>();
    }

    // 2. 异常定义
    public class DataflowProcessingException : Exception
    {
        public string BlockName { get; }
        public string RequestId { get; }

        public DataflowProcessingException(string blockName, string requestId, string message)
            : base(message)
        {
            BlockName = blockName;
            RequestId = requestId;
        }

        public DataflowProcessingException(string blockName, string requestId, string message, Exception innerException)
            : base(message, innerException)
        {
            BlockName = blockName;
            RequestId = requestId;
        }
    }

    // 3. 生产级数据流配置
    public class DataflowConfiguration
    {
        public string FlowName { get; set; }
        public int BufferCapacity { get; set; } = 1000;
        public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);
        public bool EnableLogging { get; set; } = true;
        public bool EnableMetrics { get; set; } = true;
        public bool StopOnFirstError { get; set; } = false;
        public int RetryAttempts { get; set; } = 3;
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
        public DataflowBlockOptions BlockOptions { get; set; } = new DataflowBlockOptions();
        public ExecutionDataflowBlockOptions ExecutionOptions { get; set; } = new ExecutionDataflowBlockOptions();

        // 初始化执行选项
        public void InitializeExecutionOptions()
        {
            ExecutionOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = MaxDegreeOfParallelism,
                BoundedCapacity = BufferCapacity,

                // 确保数据流块在任务取消时不会继续处理消息
                CancellationToken = CancellationToken.None,

                // 启用任务调度优化
                TaskScheduler = TaskScheduler.Default
            };
        }
    }

    // 4. 数据流监控遥测服务
    public class DataflowTelemetryService
    {
        private readonly ILogger<DataflowTelemetryService> _logger;
        private readonly Dictionary<string, DataflowMetrics> _flowMetrics = new Dictionary<string, DataflowMetrics>();

        public DataflowTelemetryService(ILogger<DataflowTelemetryService> logger)
        {
            _logger = logger;
        }

        public async Task TrackBlockMetricsAsync(
            string flowName,
            string blockName,
            int inputCount,
            int outputCount,
            int errorCount,
            TimeSpan processingTime)
        {
            var metrics = GetOrAddMetrics(flowName);
            var blockMetrics = metrics.GetOrAddBlockMetrics(blockName);

            blockMetrics.TotalInputMessages += inputCount;
            blockMetrics.TotalOutputMessages += outputCount;
            blockMetrics.TotalErrorMessages += errorCount;
            blockMetrics.TotalProcessingTime += processingTime.TotalMilliseconds;
            blockMetrics.UpdateCount++;
            blockMetrics.LastUpdate = DateTime.UtcNow;

            _logger.LogDebug("TRACKING - Flow: {FlowName}, Block: {BlockName} - Input: {Input}, Output: {Output}, Error: {Error}, Time: {Time}ms",
                flowName, blockName, inputCount, outputCount, errorCount, processingTime.TotalMilliseconds);
        }

        public DataflowMetrics GetFlowMetrics(string flowName)
        {
            return _flowMetrics.GetValueOrDefault(flowName, new DataflowMetrics { FlowName = flowName });
        }

        public Dictionary<string, DataflowMetrics> GetAllMetrics()
        {
            return new Dictionary<string, DataflowMetrics>(_flowMetrics);
        }

        public void ResetMetrics(string flowName = null)
        {
            if (string.IsNullOrEmpty(flowName))
            {
                _flowMetrics.Clear();
                _logger.LogInformation("All dataflow metrics reset");
            }
            else
            {
                _flowMetrics.Remove(flowName);
                _logger.LogInformation("Dataflow metrics reset for {FlowName}", flowName);
            }
        }

        private DataflowMetrics GetOrAddMetrics(string flowName)
        {
            if (!_flowMetrics.TryGetValue(flowName, out var metrics))
            {
                metrics = new DataflowMetrics { FlowName = flowName };
                _flowMetrics[flowName] = metrics;
            }

            return metrics;
        }
    }

    // 5. 数据流指标模型
    public class DataflowMetrics
    {
        public string FlowName { get; set; }
        public Dictionary<string, DataflowBlockMetrics> BlockMetrics { get; set; } = new Dictionary<string, DataflowBlockMetrics>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DataflowBlockMetrics GetOrAddBlockMetrics(string blockName)
        {
            if (!BlockMetrics.TryGetValue(blockName, out var blockMetrics))
            {
                blockMetrics = new DataflowBlockMetrics { BlockName = blockName };
                BlockMetrics[blockName] = blockMetrics;
            }

            return blockMetrics;
        }
    }

    public class DataflowBlockMetrics
    {
        public string BlockName { get; set; }
        public long TotalInputMessages { get; set; }
        public long TotalOutputMessages { get; set; }
        public long TotalErrorMessages { get; set; }
        public double TotalProcessingTime { get; set; }
        public long UpdateCount { get; set; }
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;

        public double AverageProcessingTime => UpdateCount > 0 ? TotalProcessingTime / UpdateCount : 0;
        public double ProcessingSuccessRate => TotalInputMessages > 0 ?
            ((double)TotalOutputMessages / TotalInputMessages) * 100 : 0;
        public double ErrorRate => TotalInputMessages > 0 ?
            ((double)TotalErrorMessages / TotalInputMessages) * 100 : 0;
    }

    // 6. 生产级数据流构建器
    public class DataflowBuilder
    {
        private readonly ILogger<DataflowBuilder> _logger;
        private readonly DataflowTelemetryService _telemetryService;

        public DataflowBuilder(
            ILogger<DataflowBuilder> logger,
            DataflowTelemetryService telemetryService)
        {
            _logger = logger;
            _telemetryService = telemetryService;
        }

        /// <summary>
        /// 创建数据验证块
        /// </summary>
        public TransformBlock<ProcessingRequest, ProcessingRequest> CreateValidationBlock(
            DataflowConfiguration config,
            Func<ProcessingRequest, bool> validator)
        {
            _logger.LogInformation("Creating validation block for flow {FlowName}", config.FlowName);

            var block = new TransformBlock<ProcessingRequest, ProcessingRequest>(
                request =>
                {
                    if (validator(request))
                    {
                        return request;
                    }
                    else
                    {
                        _logger.LogWarning("Validation failed for request {RequestId}", request.RequestId);
                        return null; // 输入验证失败的数据将被丢弃
                    }
                },
                config.ExecutionOptions);

            return block;
        }

        /// <summary>
        /// 创建并行处理块
        /// </summary>
        public TransformBlock<TIn, TOut> CreateProcessingBlock<TIn, TOut>(
            DataflowConfiguration config,
            Func<TIn, CancellationToken, Task<TOut>> processor,
            string blockName = null,
            bool enableBatching = false)
        {
            _logger.LogInformation("Creating processing block {BlockName} for flow {FlowName} with DOP={DOP}",
                blockName ?? "ProcessingBlock", config.FlowName, config.MaxDegreeOfParallelism);

            TransformBlock<TIn, TOut> block;

            if (enableBatching)
            {
                // 批量处理优化
                var batchConfig = new GroupingDataflowBlockOptions
                {
                    BoundedCapacity = config.BufferCapacity,
                    MaxDegreeOfParallelism = config.MaxDegreeOfParallelism
                };

                var batchBlock = new BatchBlock<TIn>(10, batchConfig);
                var batchProcessorBlock = new TransformBlock<TIn[], List<TOut>>(
                    async items =>
                    {
                        var results = new List<TOut>();
                        foreach (var item in items)
                        {
                            var result = await processor(item, CancellationToken.None);
                            results.Add(result);
                        }
                        return results;
                    },
                    config.ExecutionOptions);

                block = new TransformBlock<TIn, TOut>(
                    async item =>
                    {
                        var startTime = DateTime.UtcNow;
                        try
                        {
                            var result = await processor(item, CancellationToken.None);
                            var duration = DateTime.UtcNow - startTime;

                            await _telemetryService.TrackBlockMetricsAsync(
                                config.FlowName, blockName ?? "BatchProcessingBlock", 1, 1, 0, duration);

                            return result;
                        }
                        catch (Exception ex)
                        {
                            var duration = DateTime.UtcNow - startTime;
                            await _telemetryService.TrackBlockMetricsAsync(
                                config.FlowName, blockName ?? "BatchProcessingBlock", 1, 0, 1, duration);

                            _logger.LogError(ex, "Error processing item in block {BlockName}", blockName ?? "BatchProcessingBlock");
                            throw new DataflowProcessingException(blockName ?? "BatchProcessingBlock",
                                item is ProcessingRequest request ? request.RequestId : "unknown",
                                "Processing failed", ex);
                        }
                    },
                    config.ExecutionOptions);
            }
            else
            {
                // 单项处理
                block = new TransformBlock<TIn, TOut>(
                    async item =>
                    {
                        var startTime = DateTime.UtcNow;
                        var duration = TimeSpan.Zero;

                        try
                        {
                            // 执行处理逻辑
                            var result = await processor(item, CancellationToken.None);
                            duration = DateTime.UtcNow - startTime;

                            await _telemetryService.TrackBlockMetricsAsync(
                                config.FlowName, blockName ?? "ProcessingBlock", 1, 1, 0, duration);

                            return result;
                        }
                        catch (Exception ex)
                        {
                            duration = DateTime.UtcNow - startTime;
                            await _telemetryService.TrackBlockMetricsAsync(
                                config.FlowName, blockName ?? "ProcessingBlock", 1, 0, 1, duration);

                            _logger.LogError(ex, "Error processing item in block {BlockName}", blockName ?? "ProcessingBlock");

                            if (config.StopOnFirstError)
                            {
                                throw new DataflowProcessingException(blockName ?? "ProcessingBlock",
                                    item is ProcessingRequest request ? request.RequestId : "unknown",
                                    "Processing failed", ex);
                            }

                            return default(TOut); // 返回默认值继续数据流
                        }
                    },
                    config.ExecutionOptions);
            }

            return block;
        }

        /// <summary>
        /// 创建聚合处理块
        /// </summary>
        public ActionBlock<T> CreateAggregationBlock<T>(
            DataflowConfiguration config,
            Func<T, CancellationToken, Task> aggregator,
            string blockName = null)
        {
            _logger.LogInformation("Creating aggregation block {BlockName} for flow {FlowName}",
                blockName ?? "AggregationBlock", config.FlowName);

            var block = new ActionBlock<T>(
                async item =>
                {
                    var startTime = DateTime.UtcNow;
                    var duration = TimeSpan.Zero;

                    try
                    {
                        await aggregator(item, CancellationToken.None);
                        duration = DateTime.UtcNow - startTime;

                        await _telemetryService.TrackBlockMetricsAsync(
                            config.FlowName, blockName ?? "AggregationBlock", 1, 1, 0, duration);

                        return true;
                    }
                    catch (Exception ex)
                    {
                        duration = DateTime.UtcNow - startTime;
                        await _telemetryService.TrackBlockMetricsAsync(
                            config.FlowName, blockName ?? "AggregationBlock", 1, 0, 1, duration);

                        _logger.LogError(ex, "Error aggregating item in block {BlockName}", blockName ?? "AggregationBlock");
                        return false; // 处理失败但数据流继续
                    }
                },
                config.ExecutionOptions);

            return block;
        }

        /// <summary>
        /// 创建广播块
        /// </summary>
        public BroadcastBlock<T> CreateBroadcastBlock<T>(
            DataflowConfiguration config,
            string blockName = null)
        {
            _logger.LogInformation("Creating broadcast block {BlockName} for flow {FlowName}",
                blockName ?? "BroadcastBlock", config.FlowName);

            var options = new DataflowBlockOptions
            {
                BoundedCapacity = config.BufferCapacity
            };

            return new BroadcastBlock<T>(null, options);
        }

        /// <summary>
        /// 创建缓冲块
        /// </summary>
        public BufferBlock<T> CreateBufferBlock<T>(
            DataflowConfiguration config,
            string blockName = null)
        {
            _logger.LogInformation("Creating buffer block {BlockName} for flow {FlowName} with capacity {Capacity}",
                blockName ?? "BufferBlock", config.FlowName, config.BufferCapacity);

            var options = new DataflowBlockOptions
            {
                BoundedCapacity = config.BufferCapacity
            };

            return new BufferBlock<T>(options);
        }
    }

    // 7. 数据流编排服务
    public class DataflowOrchestrationService
    {
        private readonly ILogger<DataflowOrchestrationService> _logger;
        private readonly DataflowBuilder _builder;
        private readonly DataflowTelemetryService _telemetryService;
        private readonly Dictionary<string, List<IDataflowBlock>> _flowBlocks = new Dictionary<string, List<IDataflowBlock>>();

        public DataflowOrchestrationService(
            ILogger<DataflowOrchestrationService> logger,
            DataflowBuilder builder,
            DataflowTelemetryService telemetryService)
        {
            _logger = logger;
            _builder = builder;
            _telemetryService = telemetryService;
        }

        /// <summary>
        /// 创建简单线性数据流管道
        /// </summary>
        public DataflowPipeline<TIn, TOut> CreateLinearPipeline<TIn, TOut>(
            DataflowConfiguration config,
            List<Func<TIn, CancellationToken, Task<TIn>>> processors)
        {
            _logger.LogInformation("Creating linear pipeline for flow {FlowName} with {ProcessorCount} steps",
                config.FlowName, processors.Count);

            // 初始化数据流块选项
            config.InitializeExecutionOptions();

            var pipeline = new DataflowPipeline<TIn, TOut> { FlowName = config.FlowName };
            var blocks = new List<IDataflowBlock>();

            // 创建初始缓冲块
            var inputBuffer = _builder.CreateBufferBlock<TIn>(config, "InputBuffer");
            blocks.Add(inputBuffer);
            pipeline.InputBlock = inputBuffer;

            // 创建处理块链
            IDataflowBlock currentBlock = inputBuffer;

            for (int i = 0; i < processors.Count; i++)
            {
                var stepName = $"ProcessorStep_{i + 1}";
                var processorBlock = _builder.CreateProcessingBlock<TIn, TIn>(
                    config, processors[i], stepName);

                blocks.Add(processorBlock);
                pipeline.ProcessorBlocks.Add(processorBlock);

                // 链接块
                var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
                currentBlock.LinkTo(processorBlock, linkOptions);
                currentBlock = processorBlock;
            }

            // 创建输出处理块（类型转换）
            var outputProcessor = new TransformBlock<TIn, TOut>(
                input => (TOut)(object)input,
                config.ExecutionOptions);

            blocks.Add(outputProcessor);
            pipeline.OutputBlock = outputProcessor;

            // 链接最后一个处理器到输出块
            var finalLinkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            currentBlock.LinkTo(outputProcessor, finalLinkOptions);

            // 记录管道中的块
            _flowBlocks[config.FlowName] = blocks;

            _logger.LogInformation("Linear pipeline created successfully for flow {FlowName}", config.FlowName);

            return pipeline;
        }

        /// <summary>
        /// 创建分支数据流
        /// </summary>
        public void CreateBranchingFlow(
            DataflowConfiguration config,
            ITargetBlock<ProcessingRequest> inputBlock,
            List<ITargetBlock<ProcessingRequest>> targetBlocks,
            Func<ProcessingRequest, int> routingFunction)
        {
            _logger.LogInformation("Creating branching flow for {FlowName} with {TargetCount} branches",
                config.FlowName, targetBlocks.Count);

            config.InitializeExecutionOptions();

            var splitter = new ActionBlock<ProcessingRequest>(
                async request =>
                {
                    var targetIndex = routingFunction(request) % targetBlocks.Count;
                    await targetBlocks[targetIndex].SendAsync(request, config.ExecutionOptions.CancellationToken);
                    return true;
                },
                config.ExecutionOptions);

            inputBlock.LinkTo(splitter, new DataflowLinkOptions { PropagateCompletion = true });

            _flowBlocks[config.FlowName] = new List<IDataflowBlock> { inputBlock, splitter };
            _flowBlocks[config.FlowName].AddRange(targetBlocks);

            _logger.LogInformation("Branching flow created successfully");
        }

        /// <summary>
        /// 获取数据流状态信息
        /// </summary>
        public DataflowStatus GetFlowStatus(string flowName)
        {
            var status = new DataflowStatus { FlowName = flowName };

            if (_flowBlocks.TryGetValue(flowName, out var blocks))
            {
                foreach (var block in blocks)
                {
                    switch (block)
                    {
                        case BufferBlock<object> buffer:
                            status.Blocks.Add(new DataflowBlockStatus
                            {
                                Name = "BufferBlock",
                                InputCount = buffer.InputCount,
                                OutputCount = buffer.OutputCount,
                                IsCompleted = buffer.Completion.IsCompleted
                            });
                            break;

                        case TransformBlock<object, object> transform:
                            status.Blocks.Add(new DataflowBlockStatus
                            {
                                Name = "TransformBlock",
                                InputCount = transform.InputCount,
                                OutputCount = transform.OutputCount,
                                IsCompleted = transform.Completion.IsCompleted
                            });
                            break;

                        case ActionBlock<object> action:
                            status.Blocks.Add(new DataflowBlockStatus
                            {
                                Name = "ActionBlock",
                                InputCount = action.InputCount,
                                OutputCount = 0, // ActionBlock无输出
                                IsCompleted = action.Completion.IsCompleted
                            });
                            break;
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// 完成并关闭数据流
        /// </summary>
        public async Task<bool> CompleteFlowAsync(string flowName, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Completing dataflow {FlowName}", flowName);

            try
            {
                if (_flowBlocks.TryGetValue(flowName, out var blocks))
                {
                    // 通知输入块完成
                    foreach (var block in blocks.OfType<ITargetBlock<object>>())
                    {
                        block.Complete();
                    }

                    // 等待所有块完成
                    var completionTasks = blocks.Select(b => b.Completion).ToArray();

                    if (cancellationToken != CancellationToken.None)
                    {
                        var allCompleted = Task.WhenAll(completionTasks);
                        var timeoutTask = Task.Delay(config.Timeout, cancellationToken);

                        var completedTask = await Task.WhenAny(allCompleted, timeoutTask);
                        if (completedTask == timeoutTask)
                        {
                            _logger.LogWarning("Dataflow {FlowName} completion timed out", flowName);
                            return false;
                        }
                    }
                    else
                    {
                        await Task.WhenAll(completionTasks);
                    }

                    _flowBlocks.Remove(flowName);
                    _logger.LogInformation("Dataflow {FlowName} completed successfully", flowName);

                    return true;
                }

                _logger.LogWarning("Dataflow {FlowName} not found", flowName);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing dataflow {FlowName}", flowName);
                return false;
            }
        }
    }

    // 8. 数据流管道模型
    public class DataflowPipeline<TIn, TOut>
    {
        public string FlowName { get; set; }
        public ITargetBlock<TIn> InputBlock { get; set; }
        public ISourceBlock<TOut> OutputBlock { get; set; }
        public List<TransformBlock<TIn, TIn>> ProcessorBlocks { get; set; } = new List<TransformBlock<TIn, TIn>>();

        public async Task<bool> SendAsync(TIn data, CancellationToken cancellationToken = default)
        {
            return await InputBlock.SendAsync(data, cancellationToken);
        }

        public async Task<TOut> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            return await OutputBlock.ReceiveAsync(cancellationToken);
        }

        public bool TryReceive(out TOut data)
        {
            return OutputBlock.TryReceive(out data);
        }

        public void Complete()
        {
            InputBlock.Complete();
        }
    }

    // 9. 数据流状态模型
    public class DataflowStatus
    {
        public string FlowName { get; set; }
        public List<DataflowBlockStatus> Blocks { get; set; } = new List<DataflowBlockStatus>();
        public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;

        public long TotalInputCount => Blocks.Sum(b => b.InputCount);
        public long TotalOutputCount => Blocks.Sum(b => b.OutputCount);
        public int ActiveBlocks => Blocks.Count(b => !b.IsCompleted);
    }

    public class DataflowBlockStatus
    {
        public string Name { get; set; }
        public long InputCount { get; set; }
        public long OutputCount { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    // 10. 错误恢复和重试服务
    public class DataflowErrorRecoveryService
    {
        private readonly ILogger<DataflowErrorRecoveryService> _logger;
        private readonly int _maxRetries;
        private readonly TimeSpan _retryDelay;

        public DataflowErrorRecoveryService(
            ILogger<DataflowErrorRecoveryService> logger,
            int maxRetries = 3,
            TimeSpan retryDelay = default)
        {
            _logger = logger;
            _maxRetries = maxRetries;
            _retryDelay = retryDelay == default ? TimeSpan.FromSeconds(1) : retryDelay;
        }

        /// <summary>
        /// 重试处理逻辑
        /// </summary>
        public async Task<TOut> ProcessWithRetryAsync<TIn, TOut>(
            TIn input,
            Func<TIn, Task<TOut>> processor,
            string operationName,
            CancellationToken cancellationToken = default)
        {
            Exception lastException = null;

            for (int attempt = 0; attempt <= _maxRetries; attempt++)
            {
                try
                {
                    var result = await processor(input);

                    if (attempt > 0)
                    {
                        _logger.LogInformation("Operation {OperationName} succeeded after {AttemptCount} retries",
                            operationName, attempt);
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    lastException = ex;

                    if (attempt == _maxRetries)
                    {
                        _logger.LogError(ex, "Operation {OperationName} failed after {MaxRetries} attempts",
                            operationName, _maxRetries);
                        throw;
                    }

                    _logger.LogWarning(ex, "Operation {OperationName} attempt {AttemptNumber} failed, retrying",
                        operationName, attempt + 1);

                    if (_retryDelay > TimeSpan.Zero)
                    {
                        await Task.Delay(_retryDelay, cancellationToken);
                    }
                }
            }

            throw lastException;
        }
    }

    // 11. 生产级数据流服务封装
    public class DataflowService
    {
        private readonly DataflowOrchestrationService _orchestrationService;
        private readonly DataflowErrorRecoveryService _errorService;
        private readonly ILogger<DataflowService> _logger;
        private readonly DataflowTelemetryService _telemetryService;

        public DataflowService(
            DataflowOrchestrationService orchestrationService,
            DataflowErrorRecoveryService errorService,
            ILogger<DataflowService> logger,
            DataflowTelemetryService telemetryService)
        {
            _orchestrationService = orchestrationService;
            _errorService = errorService;
            _logger = logger;
            _telemetryService = telemetryService;
        }

        /// <summary>
        /// 创建图像处理数据流管道
        /// </summary>
        public DataflowPipeline<byte[], ProcessingResult<string>> CreateImageProcessingPipeline(
            DataflowConfiguration config)
        {
            _logger.LogInformation("Creating image processing pipeline: {FlowName}", config.FlowName);

            // 图像验证处理器
            var validators = new List<Func<byte[], CancellationToken, Task<byte[]>>>
        {
            ValidateImageAsync,
            ResizeImageAsync,
            ConvertImageFormatAsync
        };

            var pipeline = _orchestrationService.CreateLinearPipeline<byte[], ProcessingResult<string>>(
                config, validators);

            return pipeline;
        }

        /// <summary>
        /// 创建日志处理数据流
        /// </summary>
        public ITargetBlock<ProcessingRequest> CreateLoggingDataflow(
            DataflowConfiguration loggingConfig,
            Func<ProcessingRequest, Task> logProcessor)
        {
            _logger.LogInformation("Creating logging dataflow: {FlowName}", loggingConfig.FlowName);

            var inputBuffer = new BufferBlock<ProcessingRequest>(
                new DataflowBlockOptions { BoundedCapacity = loggingConfig.BufferCapacity });

            var logProcessorBlock = new ActionBlock<ProcessingRequest>(
                async request =>
                {
                    try
                    {
                        await logProcessor(request);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Logging dataflow error for request {RequestId}", request.RequestId);
                        return false; // 继续处理后续消息
                    }
                },
                loggingConfig.ExecutionOptions);

            inputBuffer.LinkTo(logProcessorBlock, new DataflowLinkOptions { PropagateCompletion = true });

            return inputBuffer;
        }

        /// <summary>
        /// 批量处理请求
        /// </summary>
        public async Task<List<ProcessingResult<T>>> ProcessBatchAsync<T>(
            DataflowConfiguration config,
            List<T> inputs,
            Func<T, CancellationToken, Task<ProcessingResult<T>>> processor,
            int batchSize = 100,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Processing batch of {InputCount} items using dataflow", inputs.Count);

            config.InitializeExecutionOptions();

            var batchBlock = new BatchBlock<T>(batchSize,
                new GroupingDataflowBlockOptions
                {
                    BoundedCapacity = config.BufferCapacity
                });

            var processorBlock = new TransformBlock<T[], List<ProcessingResult<T>>>(
                async items =>
                {
                    var results = new List<ProcessingResult<T>>();
                    foreach (var item in items)
                    {
                        var result = await processor(item, cancellationToken);
                        results.Add(result);
                    }
                    return results;
                },
                config.ExecutionOptions);

            var outputBuffer = new BufferBlock<List<ProcessingResult<T>>>(
                new DataflowBlockOptions { BoundedCapacity = config.BufferCapacity });

            // 链接数据流块
            batchBlock.LinkTo(processorBlock, new DataflowLinkOptions { PropagateCompletion = true });
            processorBlock.LinkTo(outputBuffer, new DataflowLinkOptions { PropagateCompletion = true });

            // 发送输入数据
            foreach (var input in inputs)
            {
                await batchBlock.SendAsync(input, cancellationToken);
            }

            // 标记完成
            batchBlock.Complete();

            // 收集处理结果
            var allResults = new List<ProcessingResult<T>>();
            await foreach (var batchResult in outputBuffer.ReceiveAllAsync(cancellationToken))
            {
                allResults.AddRange(batchResult);
            }

            _logger.LogInformation("Batch processing completed with {ResultCount} results", allResults.Count);

            return allResults;
        }

        private async Task<byte[]> ValidateImageAsync(byte[] imageData, CancellationToken cancellationToken)
        {
            // 模拟图像验证逻辑
            await Task.Delay(10, cancellationToken);

            if (imageData?.Length > 0)
            {
                return imageData;
            }

            throw new InvalidOperationException("Invalid image data");
        }

        private async Task<byte[]> ResizeImageAsync(byte[] imageData, CancellationToken cancellationToken)
        {
            // 模拟图像调整大小逻辑
            await Task.Delay(20, cancellationToken);
            return imageData;
        }

        private async Task<byte[]> ConvertImageFormatAsync(byte[] imageData, CancellationToken cancellationToken)
        {
            // 模拟图像格式转换逻辑
            await Task.Delay(30, cancellationToken);
            return imageData;
        }
    }

    // 12. 扩展数据流块功能
    public static class DataflowExtensions
    {
        /// <summary>
        /// 异步接收所有消息直到完成
        /// </summary>
        public static async IAsyncEnumerable<T> ReceiveAllAsync<T>(
            this ISourceBlock<T> source,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            while (await source.OutputAvailableAsync(cancellationToken))
            {
                if (source.TryReceive(out var item))
                {
                    yield return item;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 创建带过滤的链接
        /// </summary>
        public static IDisposable LinkTo<T>(
            this ISourceBlock<T> source,
            ITargetBlock<T> target,
            Func<T, bool> predicate,
            DataflowLinkOptions linkOptions = null)
        {
            return source.LinkTo(target, linkOptions ?? new DataflowLinkOptions(), predicate);
        }

        /// <summary>
        /// 监控数据流块的健康状态
        /// </summary>
        public static Task MonitorBlockHealthAsync<T>(
            this IDataflowBlock block,
            ILogger logger,
            string blockName,
            TimeSpan checkInterval,
            CancellationToken cancellationToken = default)
        {
            return Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        // 检查各种块类型的内部状态
                        switch (block)
                        {
                            case IReceivableSourceBlock<T> receivableBlock:
                                logger.LogDebug("Block {BlockName} health - Queue length: {QueueLength}",
                                    blockName, receivableBlock.Count);
                                break;

                            case ITargetBlock<T> targetBlock:
                                // 可以访问输入队列统计信息
                                var completionStatus = block.Completion.Status;
                                logger.LogDebug("Block {BlockName} completion status: {Status}",
                                    blockName, completionStatus);
                                break;
                        }

                        await Task.Delay(checkInterval, cancellationToken);
                    }
                    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error monitoring block {BlockName} health", blockName);
                    }
                }

                logger.LogInformation("Block {BlockName} monitoring stopped", blockName);
            }, cancellationToken);
        }
    }

    // 13. 主程序演示类
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            Console.WriteLine("System.Threading.Tasks.Dataflow Production Demo");
            Console.WriteLine("===============================================");
            Console.WriteLine();

            #region 服务配置

            // 配置日志服务
            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });

            // 注册核心数据流服务
            builder.Services.AddSingleton<DataflowTelemetryService>();
            builder.Services.AddSingleton<DataflowBuilder>();
            builder.Services.AddSingleton<DataflowOrchestrationService>();
            builder.Services.AddSingleton<DataflowErrorRecoveryService>();
            builder.Services.AddSingleton<DataflowService>();

            #endregion

            var host = builder.Build();

            await RunDemoAsync(host.Services);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task RunDemoAsync(IServiceProvider services)
        {
            var telemetryService = services.GetRequiredService<DataflowTelemetryService>();
            var builder = services.GetRequiredService<DataflowBuilder>();
            var orchestrationService = services.GetRequiredService<DataflowOrchestrationService>();
            var recoveryService = services.GetRequiredService<DataflowErrorRecoveryService>();
            var dataflowService = services.GetRequiredService<DataflowService>();

            Console.WriteLine("1. Basic Linear Dataflow Pipeline Demo:");
            try
            {
                var config = new DataflowConfiguration
                {
                    FlowName = "text_processing_flow",
                    MaxDegreeOfParallelism = 3,
                    BufferCapacity = 100
                };

                // 创建文本处理管道
                var textProcessors = new List<Func<string, CancellationToken, Task<string>>>
            {
                async (text, ct) => { await Task.Delay(50, ct); return text?.ToUpper(); },
                async (text, ct) => { await Task.Delay(30, ct); return text?.Replace(" ", "_"); },
                async (text, ct) => { await Task.Delay(20, ct); return $"Processed: {text}"; }
            };

                var textPipeline = new TransformBlock<string, string>(
                    text => text?.ToUpper(),
                    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3 });

                var textPipeline2 = new TransformBlock<string, string>(
                    text => text?.Replace(" ", "_"),
                    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3 });

                var textPipeline3 = new TransformBlock<string, string>(
                    text => $"Processed: {text}",
                    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3 });

                var outputBlock = new ActionBlock<string>(
                    text => { Console.WriteLine($"   Final result: {text}"); return Task.CompletedTask; },
                    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });

                // 链接管道
                textPipeline.LinkTo(textPipeline2);
                textPipeline2.LinkTo(textPipeline3);
                textPipeline3.LinkTo(outputBlock);

                // 发送测试数据
                for (int i = 1; i <= 10; i++)
                {
                    await textPipeline.SendAsync($"message {i}");
                }

                textPipeline.Complete();
                textPipeline2.Complete();
                textPipeline3.Complete();

                await outputBlock.Completion;

                Console.WriteLine("   Text processing pipeline completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n2. Dataflow Telemetry and Monitoring Demo:");
            try
            {
                var config = new DataflowConfiguration
                {
                    FlowName = "monitoring_flow",
                    MaxDegreeOfParallelism = 2,
                    BufferCapacity = 50
                };

                config.InitializeExecutionOptions();

                var inputBuffer = builder.CreateBufferBlock<ProcessingRequest>(config, "InputBuffer");
                var processorBlock = builder.CreateProcessingBlock<ProcessingRequest, ProcessingRequest>(
                    config,
                    async (request, cancellationToken) =>
                    {
                        await Task.Delay(100, cancellationToken);
                        return request;
                    },
                    "ProcessingBlock");

                var outputBlock = builder.CreateAggregationBlock<ProcessingRequest>(
                    config,
                    async (request, cancellationToken) =>
                    {
                        Console.WriteLine($"   Processed request: {request.RequestId}");
                        await Task.Delay(50, cancellationToken);
                    },
                    "OutputBlock");

                // 链接块
                inputBuffer.LinkTo(processorBlock);
                processorBlock.LinkTo(outputBlock);

                // 发送监控消息
                for (int i = 1; i <= 5; i++)
                {
                    var request = new ProcessingRequest
                    {
                        DataType = "Monitor",
                        Data = $"Sample data {i}"
                    };

                    await inputBuffer.SendAsync(request);
                }

                inputBuffer.Complete();
                await outputBlock.Completion;

                // 显示监控指标
                var metrics = telemetryService.GetFlowMetrics("monitoring_flow");
                var blockMetrics = metrics.BlockMetrics;

                Console.WriteLine($"   Dataflow metrics for 'monitoring_flow':");
                foreach (var kvp in blockMetrics)
                {
                    var name = kvp.Key;
                    var metric = kvp.Value;
                    Console.WriteLine($"   - {name}: Input={metric.TotalInputMessages}, " +
                                    $"Output={metric.TotalOutputMessages}, " +
                                    $"AvgTime={metric.AverageProcessingTime:F2}ms, " +
                                    $"SuccessRate={metric.ProcessingSuccessRate:F2}%");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n3. Batch Processing Dataflow Demo:");
            try
            {
                var config = new DataflowConfiguration
                {
                    FlowName = "batch_processing",
                    MaxDegreeOfParallelism = 4,
                    BufferCapacity = 1000
                };

                var inputs = Enumerable.Range(1, 20).Select(i => $"input_data_{i}").ToList();

                var batchBlock = new BatchBlock<string>(5,
                    new GroupingDataflowBlockOptions { BoundedCapacity = config.BufferCapacity });

                var batchProcessor = new TransformBlock<string[], List<string>>(
                    async items =>
                    {
                        await Task.Delay(200);
                        Console.WriteLine($"   Processing batch of {items.Length} items");
                        return items.Select(item => $"processed_{item}").ToList();
                    },
                    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2 });

                var outputBlock = new ActionBlock<List<string>>(
                    async results =>
                    {
                        foreach (var result in results)
                        {
                            Console.WriteLine($"   Batch output: {result}");
                        }
                        await Task.Delay(50);
                        return true;
                    });

                // 链接批量处理管道
                batchBlock.LinkTo(batchProcessor);
                batchProcessor.LinkTo(outputBlock);

                // 发送所有输入
                foreach (var input in inputs)
                {
                    await batchBlock.SendAsync(input);
                }

                batchBlock.Complete();
                await outputBlock.Completion;

                Console.WriteLine("   Batch processing dataflow completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n4. Error Handling and Recovery Demo:");
            try
            {
                var config = new DataflowConfiguration
                {
                    FlowName = "recovery_flow",
                    MaxDegreeOfParallelism = 2,
                    BufferCapacity = 100,
                    RetryAttempts = 3,
                    RetryDelay = TimeSpan.FromMilliseconds(500)
                };

                var unreliableProcessor = new TransformBlock<string, string>(
                    async input =>
                    {
                        await Task.Delay(100);
                        // 模拟不稳定的处理
                        if (new Random().NextDouble() > 0.7)
                        {
                            throw new InvalidOperationException("Processing failed randomly");
                        }
                        return $"processed_{input}";
                    },
                    new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = config.MaxDegreeOfParallelism
                    });

                var reliableOutput = new ActionBlock<string>(
                    output =>
                    {
                        Console.WriteLine($"   Reliable output: {output}");
                        return Task.CompletedTask;
                    });

                unreliableProcessor.LinkTo(reliableOutput, new DataflowLinkOptions(),
                    output => output != null); // 只链接成功的结果

                // 发送测试数据
                for (int i = 1; i <= 8; i++)
                {
                    await unreliableProcessor.SendAsync($"data_{i}");
                }

                unreliableProcessor.Complete();
                await reliableOutput.Completion;

                Console.WriteLine("   Error recovery dataflow completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n5. Broadcasting and Splitting Demo:");
            try
            {
                var config = new DataflowConfiguration
                {
                    FlowName = "broadcast_flow",
                    MaxDegreeOfParallelism = 1,
                    BufferCapacity = 100
                };

                var broadcastBlock = new BroadcastBlock<string>(null);
                var receiver1 = new ActionBlock<string>(msg =>
                {
                    Console.WriteLine($"   Receiver 1: {msg?.ToUpper()}");
                    return Task.CompletedTask;
                });
                var receiver2 = new ActionBlock<string>(msg =>
                {
                    Console.WriteLine($"   Receiver 2: {msg?.ToLower()}");
                    return Task.CompletedTask;
                });
                var receiver3 = new ActionBlock<string>(msg =>
                {
                    Console.WriteLine($"   Receiver 3: COUNT_{msg?.Length}");
                    return Task.CompletedTask;
                });

                // 链接广播接收者
                broadcastBlock.LinkTo(receiver1);
                broadcastBlock.LinkTo(receiver2);
                broadcastBlock.LinkTo(receiver3);

                // 发送广播消息
                for (int i = 1; i <= 3; i++)
                {
                    broadcastBlock.Post($"broadcast_message_{i}");
                }

                broadcastBlock.Complete();

                // 等待所有接收者完成
                await Task.WhenAll(receiver1.Completion, receiver2.Completion, receiver3.Completion);

                Console.WriteLine("   Broadcasting dataflow completed - message sent to all receivers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n6. Dataflow with Custom Business Logic Demo:");
            try
            {
                var dataflowConfig = new DataflowConfiguration
                {
                    FlowName = "business_logic_flow",
                    MaxDegreeOfParallelism = Environment.ProcessorCount,
                    BufferCapacity = 500
                };

                // 创建验证块 - 验证请求数据
                var validator = new TransformBlock<ProcessingRequest, ProcessingRequest>(
                    request =>
                    {
                        if (request.IsValid)
                        {
                            Console.WriteLine($"   Validated request {request.RequestId}");
                            return request;
                        }
                        else
                        {
                            Console.WriteLine($"   Invalid request {request.RequestId} - rejected");
                            return null;
                        }
                    },
                    dataflowConfig.ExecutionOptions);

                // 创建业务处理块
                var businessProcessor = new TransformBlock<ProcessingRequest, ProcessingResult<ProcessingRequest>>(
                    async request =>
                    {
                        await Task.Delay(150);

                        var success = new Random().NextDouble() > 0.2; // 80% 成功率
                        var result = new ProcessingResult<ProcessingRequest>
                        {
                            RequestId = request.RequestId,
                            Success = success,
                            Result = success ? request : null,
                            ProcessingDuration = TimeSpan.FromMilliseconds(150),
                            Metrics = new Dictionary<string, object>
                            {
                            { "ProcessorId", Environment.CurrentManagedThreadId },
                            { "MemoryUsage", GC.GetTotalMemory(false) }
                            }
                        };

                        if (!success)
                        {
                            result.ErrorMessage = "Business rule validation failed";
                            Console.WriteLine($"   Processing failed for request {request.RequestId}");
                        }
                        else
                        {
                            Console.WriteLine($"   Successfully processed request {request.RequestId}");
                        }

                        return result;
                    },
                    dataflowConfig.ExecutionOptions);

                // 创建结果保存块
                var resultSaver = new ActionBlock<ProcessingResult<ProcessingRequest>>(
                    async result =>
                    {
                        await Task.Delay(100);
                        Console.WriteLine($"   Saved result for {result.RequestId} - Success: {result.Success}");
                        return true;
                    },
                    dataflowConfig.ExecutionOptions);

                // 链接数据流块
                validator.LinkTo(businessProcessor,
                    new DataflowLinkOptions { PropagateCompletion = true });
                businessProcessor.LinkTo(resultSaver,
                    new DataflowLinkOptions { PropagateCompletion = true });

                // 发送业务数据
                var businessRequests = CreateBusinessRequests(10);
                foreach (var request in businessRequests)
                {
                    validator.Post(request);
                }

                validator.Complete();
                await resultSaver.Completion;

                Console.WriteLine("   Business logic dataflow completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n7. Advanced Dataflow Composition Demo:");
            try
            {
                // 创建输入聚合块 - 收集不同类型请求
                var inputAggregator = new BufferBlock<ProcessingRequest>();

                // 创建优先级路由块
                var priorityRouter = new ActionBlock<ProcessingRequest>(
                    async request =>
                    {
                        switch (request.Priority)
                        {
                            case 0: // 低优先级
                                await Task.Delay(300);
                                Console.WriteLine($"   Low priority request {request.RequestId} processed");
                                break;
                            case 1: // 中优先级
                                await Task.Delay(150);
                                Console.WriteLine($"   Normal priority request {request.RequestId} processed");
                                break;
                            case 2: // 高优先级
                                await Task.Delay(50);
                                Console.WriteLine($"   High priority request {request.RequestId} processed");
                                break;
                        }
                        return true;
                    });

                // 链接输入到路由
                inputAggregator.LinkTo(priorityRouter);

                // 发送不同优先级请求
                for (int i = 1; i <= 6; i++)
                {
                    var priority = i % 3; // 0,1,2循环
                    var request = new ProcessingRequest
                    {
                        RequestId = $"req_{i}",
                        Priority = priority,
                        DataType = $"Type{priority}",
                        Data = $"Data for priority {priority}"
                    };

                    await inputAggregator.SendAsync(request);
                }

                inputAggregator.Complete();
                await priorityRouter.Completion;

                Console.WriteLine("   Advanced composition dataflow completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n8. Dataflow Health Monitoring Demo:");
            try
            {
                var monitoringConfig = new DataflowConfiguration
                {
                    FlowName = "health_monitoring",
                    MaxDegreeOfParallelism = 2
                };

                var healthMonitorBlock = new BufferBlock<string>(
                    new DataflowBlockOptions { BoundedCapacity = 10 });

                var processorBlock = new ActionBlock<string>(
                    async msg =>
                    {
                        await Task.Delay(100);
                        Console.WriteLine($"   Health monitoring processor: {msg}");
                        return true;
                    });

                healthMonitorBlock.LinkTo(processorBlock);

                // 启动健康监控
                Console.WriteLine("   Starting health monitoring...");

                // 发送监控消息
                for (int i = 1; i <= 3; i++)
                {
                    await healthMonitorBlock.SendAsync($"monitor_message_{i}");
                }

                healthMonitorBlock.Complete();
                await processorBlock.Completion;

                Console.WriteLine("   Health monitoring dataflow completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n9. Cleanup and Status Demo:");
            try
            {
                // 显示所有监控指标
                var allMetrics = telemetryService.GetAllMetrics();
                Console.WriteLine($"   Total monitored dataflows: {allMetrics.Count}");

                foreach (var flowMetric in allMetrics)
                {
                    Console.WriteLine($"   Flow: {flowMetric.Key}");
                    Console.WriteLine($"   Blocks monitored: {flowMetric.Value.BlockMetrics.Count}");
                }

                // 样本状态查询示例
                var statusSample = new DataflowStatus
                {
                    FlowName = "sample_flow",
                    Blocks = new List<DataflowBlockStatus>
                {
                    new DataflowBlockStatus { Name = "InputBuffer", InputCount = 10, OutputCount = 8, IsCompleted = false },
                    new DataflowBlockStatus { Name = "Processor", InputCount = 8, OutputCount = 6, IsCompleted = false },
                    new DataflowBlockStatus { Name = "OutputBuffer", InputCount = 6, OutputCount = 0, IsCompleted = true }
                }
                };

                Console.WriteLine($"   Sample flow status: {statusSample.FlowName}");
                Console.WriteLine($"   Total input messages: {statusSample.TotalInputCount}");
                Console.WriteLine($"   Total output messages: {statusSample.TotalOutputCount}");
                Console.WriteLine($"   Active blocks: {statusSample.ActiveBlocks}");

                // 清理指标
                telemetryService.ResetMetrics();
                Console.WriteLine("   Telemetry metrics cleaned up");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n10. Comprehensive Pipeline Demo:");
            try
            {
                var comprehensiveConfig = new DataflowConfiguration
                {
                    FlowName = "comprehensive_pipeline",
                    MaxDegreeOfParallelism = 4,
                    BufferCapacity = 200,
                    RetryAttempts = 2
                };

                // 创建完整的处理管道
                var inputBuffer = new BufferBlock<ProcessingRequest>();
                var validationBlock = new TransformBlock<ProcessingRequest, ProcessingRequest>(
                    request => request.IsValid ? request : null);
                var processingBlock = new TransformBlock<ProcessingRequest, ProcessingResult<ProcessingRequest>>(
                    async request =>
                    {
                        await Task.Delay(200);
                        return new ProcessingResult<ProcessingRequest>
                        {
                            RequestId = request.RequestId,
                            Success = true,
                            Result = request,
                            ProcessingDuration = TimeSpan.FromMilliseconds(200)
                        };
                    });
                var errorHandlingBlock = new TransformBlock<ProcessingResult<ProcessingRequest>, ProcessingResult<ProcessingRequest>>(
                    result => result.Success ? result : null);
                var outputBlock = new ActionBlock<ProcessingResult<ProcessingRequest>>(
                    result =>
                    {
                        Console.WriteLine($"   Pipeline final result: {result.RequestId}");
                        return Task.CompletedTask;
                    });

                // 链接管道
                inputBuffer.LinkTo(validationBlock);
                validationBlock.LinkTo(processingBlock);
                processingBlock.LinkTo(errorHandlingBlock);
                errorHandlingBlock.LinkTo(outputBlock);

                // 发送综合测试数据
                var comprehensiveRequests = CreateBusinessRequests(15);
                foreach (var request in comprehensiveRequests)
                {
                    await inputBuffer.SendAsync(request);
                }

                inputBuffer.Complete();
                await outputBlock.Completion;

                Console.WriteLine("   Comprehensive pipeline dataflow completed - All stages processed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }
        }

        private static List<ProcessingRequest> CreateBusinessRequests(int count)
        {
            var requests = new List<ProcessingRequest>();

            for (int i = 1; i <= count; i++)
            {
                requests.Add(new ProcessingRequest
                {
                    DataType = $"BusinessType{i % 3}",
                    Data = $"Business data {i}",
                    Priority = i % 3 // 优先级0,1,2
                });
            }

            return requests;
        }
    }

    public class Test
    {
        public void TestDataflow()
        {
            // 1. 基础块类型和它们的用途
            // 缓冲块(BufferBlock<T>)
            // 创建输入缓冲
            var inputBlock = new BufferBlock<ProcessingRequest>(
                new DataflowBlockOptions { BoundedCapacity = 100 });

            // 多生产者可以安全地发送消息到缓冲块
            await inputBlock.SendAsync(request);

            // 转换块 (TransformBlock<TIn, TOut>)
            // 创建处理块
            var processor = new TransformBlock<string, string>(
                async input =>
                {
                    // 并行处理逻辑
                    await Task.Delay(100);
                    return input?.ToUpper();
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                });

            // 操作块 (ActionBlock<T>)
            // 创建终端处理块（无输出）
            var aggregator = new ActionBlock<ProcessingResult>(
                async result =>
                {
                    // 保存结果、发送通知等
                    await SaveResult(result);
                    return true; // 或 false 表示处理失败
                });

            // 2. 块链接和管道构建
            // 基本块链接
            // 标准链接方式
            sourceBlock.LinkTo(targetBlock);

            // 带传播完成状态的链接
            sourceBlock.LinkTo(targetBlock, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });

            // 带过滤条件的链接
            sourceBlock.LinkTo(targetBlock, input => input.Priority == 1);

            // 创建复杂管道结构
            inputBuffer.LinkTo(validator);
            validator.LinkTo(processor);
            processor.LinkTo(aggregator);

            // 4. 批处理和聚合模式
            // 批量处理优化
            // 创建批处理块
            var batchBlock = new BatchBlock<T>(batchSize: 100);

            // 创建批量处理器
            var batchProcessor = new TransformBlock<T[], List<ProcessingResult<T>>>(
                async batch =>
                {
                    // 对整个批次进行优化处理
                    var results = await ProcessBatchAsync(batch);
                    return results;
                });

            // 5. 广播和分发模式
            // 消息广播
            // 创建广播块
            var broadcastBlock = new BroadcastBlock<string>(null);

            // 连接多个消费者
            broadcastBlock.LinkTo(receiver1);
            broadcastBlock.LinkTo(receiver2);
            broadcastBlock.LinkTo(receiver3);

            // 发送消息给所有连接的块
            broadcastBlock.Post("broadcast_message");

            // 6. 错误处理和恢复机制
            // 生产级错误处理
            var processor = new TransformBlock<T, TResult>(
            async input =>
            {
                try
                {
                    var startTime = DateTime.UtcNow;
                    var result = await ProcessData(input);

                    // 记录成功指标
                    await _telemetry.TrackBlockMetricsAsync(..., success: true);
                    return result;
                }
                catch (Exception ex)
                {
                    // 记录错误指标
                    await _telemetry.TrackBlockMetricsAsync(..., errorCount: 1);

                    if (shouldRetry)
                    {
                        await RetryProcessing(input); // 重试机制
                    }

                    return null; // 或默认值继续数据流
                }
            });

            // 8. 生命周期管理和完成传播
            // 安全完成处理
            public async Task<bool> CompleteFlowAsync(string flowName)
            {
                if (_flowBlocks.TryGetValue(flowName, out var blocks))
                {
                    // 通知所有输入块完成
                    foreach (var block in blocks.OfType<ITargetBlock<object>>())
                    {
                        block.Complete();
                    }

                    // 等待所有块完成处理
                    var completionTasks = blocks.Select(b => b.Completion).ToArray();
                    await Task.WhenAll(completionTasks);

                    _flowBlocks.Remove(flowName);
                    return true;
                }

                return false;
            }

            // 9. 扩展功能和实用工具
            //接收所有消息的扩展
            public static async IAsyncEnumerable<T> ReceiveAllAsync<T>(
            this ISourceBlock<T> source,
            CancellationToken cancellationToken = default)
            {
                while (await source.OutputAvailableAsync(cancellationToken))
                {
                    if (source.TryReceive(out var item))
                    {
                        yield return item;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // 10. 推荐生产环境实践
            // 性能优化配置
            // 生产环境推荐配置
            var options = new ExecutionDataflowBlockOptions
            {
                // 根据硬件资源调整
                MaxDegreeOfParallelism = Math.Min(8, Environment.ProcessorCount * 2),

                // 限制内存使用
                BoundedCapacity = 1000,

                // 启用任务调度优化
                TaskScheduler = TaskScheduler.Default,

                // 正确处理取消令牌
                CancellationToken = cancellationTokenSource.Token
            };

            // 健康监控
            // 启动块健康监控
            var monitorTask = block.MonitorBlockHealthAsync(
                logger, "ProcessorBlock", TimeSpan.FromSeconds(30), cancellationToken);

            // 在应用关闭时取消监控
            cancellationTokenSource.Cancel();
            await monitorTask;

            // 内存效率考
            // 使用正确的取消令牌避免内存泄漏
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(globalToken, localToken);

            // 定期清理完成的数据流
            await orchestrationService.CompleteFlowAsync("my_flow");

            // 合理设置缓冲容量防止内存溢出
            var boundedOptions = new DataflowBlockOptions { BoundedCapacity = 10000 };

            // 故障恢复
            // 实现重试逻辑
            for (int attempt = 0; attempt<maxRetries; attempt++)
            {
                try
                {
                    var result = await processor(data);
                    return result;
                }
                catch (Exception ex)
                {
                    if (attempt == maxRetries - 1) throw;
                    await Task.Delay(retryDelay);
                }
            }

            // 使用默认值继续处理（优雅降级）
            var processor = new TransformBlock<string, string>(
            input =>
            {
                try { return ExpensiveProcess(input); }
                catch { return input; } // 遇到错误返回原始数据
            });
    }
}

    // 3. 生产级配置选项
    // 数据流块选项配置
    public class DataflowConfiguration
    {
        public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
        public int BufferCapacity { get; set; } = 1000;
        public bool EnableLogging { get; set; } = true;
        public int RetryAttempts { get; set; } = 3;

        public void InitializeExecutionOptions()
        {
            ExecutionOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = MaxDegreeOfParallelism,
                BoundedCapacity = BufferCapacity,
                CancellationToken = CancellationToken.None,
                TaskScheduler = TaskScheduler.Default
            };
        }
    }

    // 7. 数据流状态监控
    // 实时性能指标
    public class DataflowTelemetryService
    {
        public async Task TrackBlockMetricsAsync(...)
        {
            var metrics = GetOrAddFlowMetrics(flowName);
            var blockMetrics = metrics.GetOrAddBlockMetrics(blockName);

            // 更新实时统计
            blockMetrics.TotalInputMessages += inputCount;
            blockMetrics.TotalOutputMessages += outputCount;
            blockMetrics.TotalProcessingTime += processingTime.TotalMilliseconds;
            blockMetrics.ErrorCount += errorCount;
        }

        public DataflowStatus GetFlowStatus(string flowName)
        {
            // 返回当前数据流状态
            var status = new DataflowStatus();
            // ... 收集所有块的状态信息
            return status;
        }
    }
}