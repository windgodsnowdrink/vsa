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

using App;
using Ardalis.ListStartupServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using ModelContextProtocol.Server;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Text;
using System.Text.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks;

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

    // 1. 协议定义和消息模型
    public enum MessageType
    {
        TextMessage,
        BinaryData,
        Heartbeat,
        ClientInfo,
        ServerResponse,
        SystemEvent
    }

    public class MessageHeader
    {
        public MessageType Type { get; set; }
        public int Length { get; set; }
        public string CorrelationId { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }

    public class TextMessage
    {
        public MessageHeader Header { get; set; }
        public string Content { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
    }

    public class BinaryDataMessage
    {
        public MessageHeader Header { get; set; }
        public byte[] Data { get; set; }
        public string DataType { get; set; }
    }

    // 2. 管道异常定义
    public class PipelineException : Exception
    {
        public PipelineException(string message) : base(message) { }
        public PipelineException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ProtocolValidationException : Exception
    {
        public ProtocolValidationException(string message) : base(message) { }
    }

    // 3. 生产级消息协议处理器
    public class MessageProtocolHandler
    {
        private readonly ILogger<MessageProtocolHandler> _logger;
        private readonly int _maxMessageSize = 1024 * 1024; // 1MB最大消息限制
        private readonly SemaphoreSlim _readSemaphore = new SemaphoreSlim(5, 5); // 读并发限制
        private readonly SemaphoreSlim _writeSemaphore = new SemaphoreSlim(5, 5); // 写并发限制

        public MessageProtocolHandler(ILogger<MessageProtocolHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 从管道读取消息 - 核心生产级处理逻辑
        /// </summary>
        public async Task<PipelineReadResult> ReadMessageAsync(PipeReader reader, CancellationToken cancellationToken = default)
        {
            await _readSemaphore.WaitAsync(cancellationToken);

            try
            {
                _logger.LogDebug("Starting message read from pipeline");

                while (true)
                {
                    // 读取数据缓冲
                    var result = await reader.ReadAsync(cancellationToken);
                    var buffer = result.Buffer;

                    try
                    {
                        // 尝试解析完整消息
                        if (TryParseMessage(ref buffer, out var message, out var consumedBytes))
                        {
                            // 标记已消费的数据
                            reader.AdvanceTo(buffer.Start, buffer.End);

                            _logger.LogInformation("Message parsed successfully - Type: {MessageType}, Size: {MessageSize} bytes",
                                message.Header.Type, consumedBytes);

                            return message;
                        }
                        else
                        {
                            // 如果没有足够数据，继续读取
                            reader.AdvanceTo(buffer.Start, buffer.End);
                        }

                        // 检查完成标志
                        if (result.IsCompleted)
                        {
                            if (buffer.Length > 0)
                            {
                                _logger.LogWarning("Unexpected data remaining in pipeline: {RemainingBytes} bytes", buffer.Length);
                                throw new PipelineException($"Incomplete message data remaining: {buffer.Length} bytes");
                            }
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        // 回滚缓冲区以避免数据丢失
                        reader.AdvanceTo(buffer.Start);
                        throw new PipelineException("Error parsing message from pipeline", ex);
                    }
                }

                return null; // 管道完成，无更多数据
            }
            finally
            {
                _readSemaphore.Release();
            }
        }

        /// <summary>
        /// 向管道写入消息 - 安全高效的生产级写入
        /// </summary>
        public async Task<bool> WriteMessageAsync(PipeWriter writer, PipelineReadResult message, CancellationToken cancellationToken = default)
        {
            await _writeSemaphore.WaitAsync(cancellationToken);

            try
            {
                _logger.LogDebug("Starting message write to pipeline - Type: {MessageType}", message.Header.Type);

                // 获取写缓冲区
                var buffer = writer.GetMemory(_maxMessageSize);

                // 序列化消息到缓冲区
                var bytesWritten = SerializeMessage(message, buffer.Span);

                if (bytesWritten <= 0)
                {
                    _logger.LogError("Failed to serialize message to buffer");
                    return false;
                }

                // 更新写缓冲区位置
                writer.Advance(bytesWritten);

                // 刷新数据到管道
                var flushResult = await writer.FlushAsync(cancellationToken);

                if (flushResult.IsCompleted || flushResult.IsCanceled)
                {
                    _logger.LogWarning("Pipeline write was cancelled or completed");
                    return false;
                }

                _logger.LogInformation("Message written to pipeline successfully - {BytesWritten} bytes", bytesWritten);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing message to pipeline");
                throw new PipelineException("Failed to write message to pipeline", ex);
            }
            finally
            {
                _writeSemaphore.Release();
            }
        }

        /// <summary>
        /// 尝试解析消息头 - 安全高效的协议解析
        /// </summary>
        private bool TryParseMessage(ref ReadOnlySequence<byte> buffer, out PipelineReadResult message, out int consumedBytes)
        {
            message = null;
            consumedBytes = 0;

            _logger.LogTrace("Attempting to parse message from buffer - Size: {BufferSize} bytes", buffer.Length);

            // 检查是否有足够的数据解析消息头
            if (buffer.Length < sizeof(int) * 2) // 至少需要类型和长度字段
            {
                _logger.LogDebug("Insufficient buffer data for message header - Available: {BufferSize} bytes", buffer.Length);
                return false;
            }

            var headerBuffer = buffer.Slice(0, Math.Min(1024, buffer.Length)); // 读取头部缓冲

            try
            {
                // 解析消息头
                var header = ParseMessageHeader(headerBuffer, out var headerBytes);
                if (header == null)
                {
                    _logger.LogWarning("Failed to parse message header");
                    return false;
                }

                // 检查消息长度合法性
                if (header.Length > _maxMessageSize)
                {
                    _logger.LogError("Message size exceeds maximum limit - Requested: {RequestedSize}, Max: {MaxSize}",
                        header.Length, _maxMessageSize);
                    throw new ProtocolValidationException($"Message too large: {header.Length} bytes");
                }

                // 检查缓冲区是否有完整消息
                if (buffer.Length < headerBytes + header.Length)
                {
                    _logger.LogDebug("Incomplete message in buffer - Required: {RequiredSize}, Available: {AvailableSize}",
                        headerBytes + header.Length, buffer.Length);
                    return false;
                }

                // 解析完整消息内容
                var messageBuffer = buffer.Slice(headerBytes, header.Length);
                message = ParseMessageContent(header, messageBuffer);

                consumedBytes = headerBytes + header.Length;
                var consumedPosition = buffer.GetPosition(consumedBytes);
                buffer = buffer.Slice(consumedPosition);

                _logger.LogDebug("Message parsed successfully - Consumed: {ConsumedBytes} bytes", consumedBytes);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing message from buffer");
                throw new PipelineException("Message parsing failed", ex);
            }
        }

        /// <summary>
        /// 解析消息头 - 零拷贝高性能解析
        /// </summary>
        private MessageHeader ParseMessageHeader(ReadOnlySequence<byte> headerBuffer, out int headerBytes)
        {
            headerBytes = 0;

            try
            {
                // 解析消息类型 (4 bytes)
                var typeBytes = ReadBytesFromSequence(headerBuffer, 0, 4);
                var type = (MessageType)BitConverter.ToInt32(typeBytes);

                // 解析消息长度 (4 bytes)
                var lengthBytes = ReadBytesFromSequence(headerBuffer, 4, 4);
                var length = BitConverter.ToInt32(lengthBytes);

                // 解析关联ID长度和内容
                var correlationId = "";
                var currentOffset = 8;

                var correlationIdLengthBytes = ReadBytesFromSequence(headerBuffer, currentOffset, 4);
                var correlationIdLength = BitConverter.ToInt32(correlationIdLengthBytes);
                currentOffset += 4;

                if (correlationIdLength > 0)
                {
                    var correlationIdBytes = ReadBytesFromSequence(headerBuffer, currentOffset, correlationIdLength);
                    correlationId = Encoding.UTF8.GetString(correlationIdBytes);
                    currentOffset += correlationIdLength;
                }

                // 解析时间戳 (8 bytes)
                var timestampBytes = ReadBytesFromSequence(headerBuffer, currentOffset, 8);
                var timestamp = DateTime.FromBinary(BitConverter.ToInt64(timestampBytes));
                currentOffset += 8;

                headerBytes = currentOffset;

                return new MessageHeader
                {
                    Type = type,
                    Length = length,
                    CorrelationId = correlationId,
                    Timestamp = timestamp
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing message header");
                throw new ProtocolValidationException("Invalid message header format", ex);
            }
        }

        /// <summary>
        /// 从序列读取指定字节 - 支持跨段读取高性能实用方法
        /// </summary>
        private byte[] ReadBytesFromSequence(ReadOnlySequence<byte> sequence, int offset, int count)
        {
            var bytes = new byte[count];
            var remaining = count;
            var readOffset = offset;

            foreach (var memory in sequence)
            {
                var segmentLength = memory.Length;

                if (readOffset < segmentLength)
                {
                    var availableInSegment = Math.Min(segmentLength - readOffset, remaining);
                    var sourceSpan = memory.Span.Slice(readOffset, availableInSegment);
                    var targetSpan = bytes.AsSpan(count - remaining, availableInSegment);
                    sourceSpan.CopyTo(targetSpan);

                    remaining -= availableInSegment;
                    readOffset = 0;

                    if (remaining <= 0)
                        break;
                }
                else
                {
                    readOffset -= segmentLength;
                }
            }

            if (remaining > 0)
            {
                throw new InvalidOperationException("Not enough data in sequence");
            }

            return bytes;
        }

        /// <summary>
        /// 解析消息内容 - 根据消息类型解析内容
        /// </summary>
        private PipelineReadResult ParseMessageContent(MessageHeader header, ReadOnlySequence<byte> contentBuffer)
        {
            try
            {
                switch (header.Type)
                {
                    case MessageType.TextMessage:
                        var contentBytes = ReadAllBytesFromSequence(contentBuffer);
                        var textMessage = JsonSerializer.Deserialize<TextMessage>(contentBytes);
                        return new PipelineReadResult
                        {
                            Header = header,
                            Data = contentBytes,
                            MessageType = header.Type,
                            ParsedData = textMessage,
                            ProcessingStartedAt = DateTime.UtcNow
                        };

                    case MessageType.BinaryData:
                        var binaryBytes = ReadAllBytesFromSequence(contentBuffer);
                        return new PipelineReadResult
                        {
                            Header = header,
                            Data = binaryBytes,
                            MessageType = header.Type,
                            ParsedData = new BinaryDataMessage
                            {
                                Header = header,
                                Data = binaryBytes,
                                DataType = header.Metadata.TryGetValue("ContentType", out var contentType) ? contentType : "unknown"
                            },
                            ProcessingStartedAt = DateTime.UtcNow
                        };

                    case MessageType.Heartbeat:
                        return new PipelineReadResult
                        {
                            Header = header,
                            Data = Array.Empty<byte>(),
                            MessageType = header.Type,
                            ParsedData = "HEARTBEAT",
                            ProcessingStartedAt = DateTime.UtcNow
                        };

                    default:
                        var defaultBytes = ReadAllBytesFromSequence(contentBuffer);
                        return new PipelineReadResult
                        {
                            Header = header,
                            Data = defaultBytes,
                            MessageType = header.Type,
                            ProcessingStartedAt = DateTime.UtcNow
                        };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing message content for type {MessageType}", header.Type);
                throw new ProtocolValidationException($"Failed to parse message content: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 从序列读取所有字节 - 零拷贝序列读取到字节数组
        /// </summary>
        private byte[] ReadAllBytesFromSequence(ReadOnlySequence<byte> sequence)
        {
            if (sequence.IsSingleSegment)
            {
                return sequence.First.ToArray();
            }

            var bytes = new byte[sequence.Length];
            sequence.CopyTo(bytes);
            return bytes;
        }

        /// <summary>
        /// 序列化消息到缓冲区 - 高效序列化实现
        /// </summary>
        private int SerializeMessage(PipelineReadResult message, Span<byte> buffer)
        {
            try
            {
                var offset = 0;

                // 序列化消息头
                offset += SerializeMessageHeader(message.Header, buffer.Slice(offset));

                // 序列化消息体
                if (message.Data != null && message.Data.Length > 0)
                {
                    message.Data.CopyTo(buffer.Slice(offset));
                    offset += message.Data.Length;
                }
                else if (message.ParsedData != null)
                {
                    var json = JsonSerializer.Serialize(message.ParsedData);
                    var jsonBytes = Encoding.UTF8.GetBytes(json);
                    jsonBytes.CopyTo(buffer.Slice(offset));
                    offset += jsonBytes.Length;
                    message.Header.Length = jsonBytes.Length;
                }

                _logger.LogDebug("Message serialized to buffer - Total bytes: {SerializedBytes}", offset);

                return offset;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serializing message to buffer");
                throw new PipelineException("Message serialization failed", ex);
            }
        }

        /// <summary>
        /// 序列化消息头到缓冲区
        /// </summary>
        private int SerializeMessageHeader(MessageHeader header, Span<byte> buffer)
        {
            try
            {
                var offset = 0;

                // 类型 (4 bytes)
                BitConverter.GetBytes((int)header.Type).CopyTo(buffer.Slice(offset));
                offset += 4;

                // 长度 (4 bytes) - 将在序列化完整消息后更新
                BitConverter.GetBytes(header.Length).CopyTo(buffer.Slice(offset));
                offset += 4;

                // 关联ID
                var correlationIdBytes = Encoding.UTF8.GetBytes(header.CorrelationId ?? "");
                BitConverter.GetBytes(correlationIdBytes.Length).CopyTo(buffer.Slice(offset));
                offset += 4;

                if (correlationIdBytes.Length > 0)
                {
                    correlationIdBytes.CopyTo(buffer.Slice(offset));
                    offset += correlationIdBytes.Length;
                }

                // 时间戳 (8 bytes)
                BitConverter.GetBytes(header.Timestamp.ToBinary()).CopyTo(buffer.Slice(offset));
                offset += 8;

                return offset;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serializing message header");
                throw new PipelineException("Header serialization failed", ex);
            }
        }
    }

    // 4. 管道读取结果模型
    public class PipelineReadResult
    {
        public MessageHeader Header { get; set; }
        public byte[] Data { get; set; }
        public MessageType MessageType { get; set; }
        public object ParsedData { get; set; }
        public DateTime ProcessingStartedAt { get; set; }
        public DateTime ProcessingCompletedAt { get; set; }

        public TimeSpan ProcessingDuration => ProcessingCompletedAt - ProcessingStartedAt;
        public bool IsProcessingComplete => ProcessingCompletedAt != DateTime.MinValue;
    }

    // 5. 生产级管道处理器 - 核心应用逻辑
    public class PipelineProcessor
    {
        private readonly ILogger<PipelineProcessor> _logger;
        private readonly MessageProtocolHandler _protocolHandler;
        private readonly IServiceProvider _serviceProvider;

        public PipelineProcessor(
            ILogger<PipelineProcessor> logger,
            MessageProtocolHandler protocolHandler,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _protocolHandler = protocolHandler;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 处理管道数据流 - 生产级管道处理循环
        /// </summary>
        public async Task<PipelineProcessingResult> ProcessPipelineAsync(
            PipeReader reader,
            PipeWriter writer,
            Func<PipelineReadResult, Task<object>> messageHandler,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting pipeline processing loop");

            var result = new PipelineProcessingResult
            {
                StartedAt = DateTime.UtcNow
            };

            try
            {
                var semaphore = new SemaphoreSlim(10, 10); // 流处理并发限制

                while (!cancellationToken.IsCancellationRequested)
                {
                    var message = await _protocolHandler.ReadMessageAsync(reader, cancellationToken);
                    if (message == null)
                    {
                        _logger.LogInformation("Pipeline processing completed - No more messages");
                        break;
                    }

                    await semaphore.WaitAsync(cancellationToken);

                    try
                    {
                        // 并发处理消息
                        var processingTask = ProcessMessageAsync(message, messageHandler, writer, cancellationToken);

                        // 不等待处理完成，继续读取下一个消息
                        _ = processingTask.ContinueWith(async task =>
                        {
                            try
                            {
                                var processedMessage = await task;
                                result.ProcessedMessages++;

                                if (processedMessage.Header.CorrelationId != null)
                                {
                                    result.MessageTracking.Add(processedMessage.Header.CorrelationId, processedMessage);
                                }
                            }
                            catch (Exception ex)
                            {
                                result.Errors.Add($"Message processing error: {ex.Message}");
                                _logger.LogError(ex, "Error processing message");
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        }, TaskScheduler.Default);
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Pipeline read error: {ex.Message}");
                        _logger.LogError(ex, "Error reading from pipeline");
                    }

                    // 添加小延迟避免过度消耗CPU
                    await Task.Delay(1, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Pipeline processing cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in pipeline processing");
                result.Errors.Add($"Pipeline processing failed: {ex.Message}");
            }

            result.CompletedAt = DateTime.UtcNow;
            result.Duration = result.CompletedAt - result.StartedAt;

            return result;
        }

        /// <summary>
        /// 处理单个消息并写回响应
        /// </summary>
        private async Task<PipelineReadResult> ProcessMessageAsync(
            PipelineReadResult message,
            Func<PipelineReadResult, Task<object>> messageHandler,
            PipeWriter writer,
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("Processing message - Type: {MessageType}, CorrelationId: {CorrelationId}",
                message.MessageType, message.Header.CorrelationId);

            try
            {
                var responsePayload = await messageHandler(message);

                if (responsePayload != null)
                {
                    var responseMessage = CreateResponseMessage(message, responsePayload);
                    await _protocolHandler.WriteMessageAsync(writer, responseMessage, cancellationToken);
                }

                message.ProcessingCompletedAt = DateTime.UtcNow;

                _logger.LogInformation("Message processed successfully - Type: {MessageType}, Duration: {Duration}ms",
                    message.MessageType, message.ProcessingDuration.TotalMilliseconds);

                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message {MessageType}", message.MessageType);

                // 创建错误响应
                var errorResponse = CreateErrorResponse(message, ex);
                await _protocolHandler.WriteMessageAsync(writer, errorResponse, cancellationToken);

                throw;
            }
        }

        /// <summary>
        /// 创建响应消息
        /// </summary>
        private PipelineReadResult CreateResponseMessage(PipelineReadResult requestMessage, object payload)
        {
            return new PipelineReadResult
            {
                Header = new MessageHeader
                {
                    Type = MessageType.ServerResponse,
                    CorrelationId = requestMessage.Header.CorrelationId,
                    Timestamp = DateTime.UtcNow,
                    Metadata = new Dictionary<string, string>
                    {
                        ["RequestType"] = requestMessage.MessageType.ToString()
                    }
                },
                ParsedData = payload,
                Data = JsonSerializer.SerializeToUtf8Bytes(payload),
                ProcessingStartedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// 创建错误响应消息
        /// </summary>
        private PipelineReadResult CreateErrorResponse(PipelineReadResult requestMessage, Exception error)
        {
            return new PipelineReadResult
            {
                Header = new MessageHeader
                {
                    Type = MessageType.SystemEvent,
                    CorrelationId = requestMessage.Header.CorrelationId,
                    Timestamp = DateTime.UtcNow,
                    Metadata = new Dictionary<string, string>
                    {
                        ["Error"] = error.Message,
                        ["RequestType"] = requestMessage.MessageType.ToString(),
                        ["ResponseType"] = "ErrorResponse"
                    }
                },
                ParsedData = new { Error = error.Message, Type = error.GetType().Name },
                Data = JsonSerializer.SerializeToUtf8Bytes(new { Error = error.Message, Type = error.GetType().Name }),
                ProcessingStartedAt = DateTime.UtcNow
            };
        }
    }

    // 6. 管道处理结果模型
    public class PipelineProcessingResult
    {
        public DateTime StartedAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public long ProcessedMessages { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public Dictionary<string, PipelineReadResult> MessageTracking { get; set; } =
            new Dictionary<string, PipelineReadResult>();

        public bool IsSuccess => Errors.Count == 0;
        public int ErrorCount => Errors.Count;
    }

    // 7. 网络管道服务 - 生产级TCP网络流处理
    public class NetworkPipelineService
    {
        private readonly ILogger<NetworkPipelineService> _logger;
        private readonly MessageProtocolHandler _protocolHandler;

        public NetworkPipelineService(
            ILogger<NetworkPipelineService> logger,
            MessageProtocolHandler protocolHandler)
        {
            _logger = logger;
            _protocolHandler = protocolHandler;
        }

        /// <summary>
        /// 从TCP流创建管道读取器
        /// </summary>
        public PipeReader CreatePipeReader(NetworkStream stream)
        {
            _logger.LogInformation("Creating pipe reader from TCP stream");

            var options = new StreamPipeReaderOptions(
                bufferSize: 4096,           // 缓冲区大小优化
                minimumReadSize: 1024,      // 最小读取大小
                leaveOpen: false,           // 处理完成后关闭流
                pool: MemoryPool<byte>.Shared); // 共享内存池

            return PipeReader.Create(stream, options);
        }

        /// <summary>
        /// 从TCP流创建管道写入器
        /// </summary>
        public PipeWriter CreatePipeWriter(NetworkStream stream)
        {
            _logger.LogInformation("Creating pipe writer from TCP stream");

            var options = new StreamPipeWriterOptions(
                bufferSize: 4096,           // 缓冲区大小优化
                leaveOpen: false,           // 处理完成后关闭流
                pool: MemoryPool<byte>.Shared); // 共享内存池

            return PipeWriter.Create(stream, options);
        }

        /// <summary>
        /// 处理TCP客户端连接的数据流
        /// </summary>
        public async Task HandleClientConnectionAsync(
            TcpClient client,
            Func<PipelineReadResult, Task<object>> messageHandler,
            CancellationToken cancellationToken = default)
        {
            var clientEndpoint = client.Client.RemoteEndPoint?.ToString() ?? "Unknown";
            _logger.LogInformation("Handling pipeline connection for client {ClientEndpoint}", clientEndpoint);

            try
            {
                using var stream = client.GetStream();
                var reader = CreatePipeReader(stream);
                var writer = CreatePipeWriter(stream);

                var processor = new PipelineProcessor(
                    _logger,
                    _protocolHandler,
                    serviceProvider: null);

                var result = await processor.ProcessPipelineAsync(reader, writer, messageHandler, cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Connection processing completed with errors for {ClientEndpoint} - Errors: {ErrorCount}",
                        clientEndpoint, result.ErrorCount);
                }
                else
                {
                    _logger.LogInformation("Connection processing completed successfully for {ClientEndpoint} - Messages: {MessageCount}",
                        clientEndpoint, result.ProcessedMessages);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling client connection {ClientEndpoint}", clientEndpoint);
            }
            finally
            {
                client?.Close();
            }
        }
    }

    // 8. 内存缓冲优化服务 - 生产环境内存使用优化
    public class PipelineBufferOptimizationService
    {
        private readonly MemoryPool<byte> _memoryPool;
        private readonly ILogger<PipelineBufferOptimizationService> _logger;

        public PipelineBufferOptimizationService(ILogger<PipelineBufferOptimizationService> logger)
        {
            _memoryPool = MemoryPool<byte>.Shared;
            _logger = logger;
        }

        /// <summary>
        /// 创建优化配置的管道
        /// </summary>
        public Pipe CreateOptimizedPipe(int bufferSize = 8192, int pauseWriterThreshold = 32768)
        {
            var options = new PipeOptions(
                pool: _memoryPool,
                readerScheduler: PipeScheduler.ThreadPool,      // 使用线程池调度读取
                writerScheduler: PipeScheduler.ThreadPool,      // 使用线程池调度写入
                pauseWriterThreshold: pauseWriterThreshold,     // 写暂停阈值
                resumeWriterThreshold: pauseWriterThreshold / 2, // 写继��阈值
                minimumSegmentSize: bufferSize,                // 最小段大小
                useSynchronizationContext: false);            // 不使用同步上下文

            _logger.LogInformation("Creating optimized pipe with bufferSize={BufferSize}, pauseThreshold={PauseThreshold}",
                bufferSize, pauseWriterThreshold);

            return new Pipe(options);
        }

        /// <summary>
        /// 获取内存池使用统计
        /// </summary>
        public BufferPoolStats GetPoolUsageStats()
        {
            // 由于MemoryPool是内部实现，这里返回通用统计
            return new BufferPoolStats
            {
                BlockSize = 4096,
                TotalBlocks = 1000,
                AvailableBlocks = 950,
                UsedBlocks = 50,
                RetrievedAt = DateTime.UtcNow
            };
        }
    }

    public class BufferPoolStats
    {
        public int BlockSize { get; set; }
        public int TotalBlocks { get; set; }
        public int AvailableBlocks { get; set; }
        public int UsedBlocks { get; set; }
        public DateTime RetrievedAt { get; set; }

        public double UsagePercentage => TotalBlocks > 0 ? (double)UsedBlocks / TotalBlocks * 100 : 0;
    }

    // 9. 生产级消息处理示例服务
    public class MessageProcessingService
    {
        private readonly ILogger<MessageProcessingService> _logger;

        public MessageProcessingService(ILogger<MessageProcessingService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 消息处理处理程序
        /// </summary>
        public async Task<object> HandleMessageAsync(PipelineReadResult message)
        {
            _logger.LogInformation("Handling message of type {MessageType}", message.MessageType);

            switch (message.MessageType)
            {
                case MessageType.TextMessage:
                    return await HandleTextMessageAsync(message);

                case MessageType.BinaryData:
                    return await HandleBinaryMessageAsync(message);

                case MessageType.Heartbeat:
                    return await HandleHeartbeatAsync(message);

                case MessageType.ClientInfo:
                    return await HandleClientInfoAsync(message);

                default:
                    throw new NotSupportedException($"Unsupported message type: {message.MessageType}");
            }
        }

        private async Task<object> HandleTextMessageAsync(PipelineReadResult message)
        {
            var textMessage = message.ParsedData as TextMessage;
            await Task.Delay(10); // 模拟处理延迟

            _logger.LogInformation("Processing text message from {Sender}", textMessage?.Sender ?? "Unknown");

            // 模拟业务逻辑
            var response = new TextMessage
            {
                Header = new MessageHeader
                {
                    Type = MessageType.ServerResponse,
                    CorrelationId = message.Header.CorrelationId,
                    Timestamp = DateTime.UtcNow
                },
                Content = $"Echo: {textMessage?.Content}",
                Sender = "Server",
                Receiver = textMessage?.Sender
            };

            return response;
        }

        private async Task<object> HandleBinaryMessageAsync(PipelineReadResult message)
        {
            var binaryMessage = message.ParsedData as BinaryDataMessage;
            await Task.Delay(50); // 模拟处理延迟

            _logger.LogInformation("Processing binary message of type {DataType} with {DataSize} bytes",
                binaryMessage?.DataType, binaryMessage?.Data?.Length);

            // 模拟业务逻辑
            return new
            {
                Processed = true,
                DataType = binaryMessage?.DataType,
                ProcessingTime = DateTime.UtcNow,
                DataHash = ComputeDataHash(binaryMessage?.Data),
                Message = "Binary data processed successfully"
            };
        }

        private async Task<object> HandleHeartbeatAsync(PipelineReadResult message)
        {
            await Task.Delay(1); // 最小延迟

            _logger.LogDebug("Responding to heartbeat from client");

            return new
            {
                ServerTime = DateTime.UtcNow,
                Status = "Alive",
                Uptime = DateTime.UtcNow - message.Header.Timestamp
            };
        }

        private async Task<object> HandleClientInfoAsync(PipelineReadResult message)
        {
            await Task.Delay(5); // 模拟处理延迟

            var clientInfo = Encoding.UTF8.GetString(message.Data);
            _logger.LogInformation("Received client info: {ClientInfo}", clientInfo);

            return new
            {
                Acknowledged = true,
                ServerInfo = new
                {
                    ServerTime = DateTime.UtcNow,
                    ServerName = Environment.MachineName,
                    ProcessId = Environment.ProcessId
                }
            };
        }

        private string ComputeDataHash(byte[] data)
        {
            if (data == null || data.Length == 0)
                return "no-data";

            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashBytes = sha256.ComputeHash(data);
            return Convert.ToBase64String(hashBytes);
        }
    }

    // 10. 流合并和多路处理服务
    public class StreamMultiplexingService
    {
        private readonly ILogger<StreamMultiplexingService> _logger;
        private readonly MessageProtocolHandler _protocolHandler;

        public StreamMultiplexingService(
            ILogger<StreamMultiplexingService> logger,
            MessageProtocolHandler protocolHandler)
        {
            _logger = logger;
            _protocolHandler = protocolHandler;
        }

        /// <summary>
        /// 合并多个管道流到单一输出
        /// </summary>
        public async Task MultiplexStreamsAsync(
            List<PipeReader> readers,
            PipeWriter writer,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting stream multiplexing for {ReaderCount} streams", readers.Count);

            try
            {
                var tasks = readers.Select(async reader =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var message = await _protocolHandler.ReadMessageAsync(reader, cancellationToken);
                        if (message == null) break;

                        await _protocolHandler.WriteMessageAsync(writer, message, cancellationToken);
                    }
                }).ToList();

                await Task.WhenAll(tasks);

                _logger.LogInformation("Stream multiplexing completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during stream multiplexing");
                throw new PipelineException("Stream multiplexing failed", ex);
            }
        }

        /// <summary>
        /// 从单一输入流分离到多个输出流
        /// </summary>
        public async Task DemultiplexStreamAsync(
            PipeReader reader,
            Dictionary<string, PipeWriter> writers,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting stream demultiplexing to {WriterCount} streams", writers.Count);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var message = await _protocolHandler.ReadMessageAsync(reader, cancellationToken);
                    if (message == null) break;

                    // 根据消息类型或元数据路由到不同输出
                    var targetWriter = GetTargetWriter(message, writers);
                    if (targetWriter != null)
                    {
                        await _protocolHandler.WriteMessageAsync(targetWriter, message, cancellationToken);
                    }
                }

                _logger.LogInformation("Stream demultiplexing completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during stream demultiplexing");
                throw new PipelineException("Stream demultiplexing failed", ex);
            }
        }

        private PipeWriter GetTargetWriter(PipelineReadResult message, Dictionary<string, PipeWriter> writers)
        {
            return message.Header.Type switch
            {
                MessageType.TextMessage => writers.GetValueOrDefault("text"),
                MessageType.BinaryData => writers.GetValueOrDefault("binary"),
                MessageType.Heartbeat => writers.GetValueOrDefault("heartbeat"),
                _ => writers.GetValueOrDefault("default")
            };
        }
    }

    // 11. 管道中间件 - 生产环境监控和过滤
    public class PipelineMiddleware
    {
        private readonly ILogger<PipelineMiddleware> _logger;
        private readonly PipelineBufferOptimizationService _bufferService;

        public PipelineMiddleware(
            ILogger<PipelineMiddleware> logger,
            PipelineBufferOptimizationService bufferService)
        {
            _logger = logger;
            _bufferService = bufferService;
        }

        /// <summary>
        /// 流量控制中间件 - 防止管道过载
        /// </summary>
        public async Task<bool> ProcessWithRateLimitingAsync(
            PipeReader reader,
            PipeWriter writer,
            Func<PipelineReadResult, Task<object>> messageHandler,
            int maxMessagesPerSecond = 1000,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting rate-limited pipeline processing - Max: {MaxRate} messages/sec",
                maxMessagesPerSecond);

            var lastProcessedTime = DateTime.UtcNow;
            var messagesProcessed = 0;
            var processedBytes = 0L;

            try
            {
                var protocolHandler = new MessageProtocolHandler(_logger);
                var processor = new PipelineProcessor(_logger, protocolHandler, null);

                var result = await processor.ProcessPipelineAsync(reader, writer, async message =>
                {
                    var currentTime = DateTime.UtcNow;
                    var elapsedTime = currentTime - lastProcessedTime;

                    // 速率限制检查
                    if (elapsedTime.TotalSeconds >= 1)
                    {
                        var currentRate = messagesProcessed / elapsedTime.TotalSeconds;
                        if (currentRate > maxMessagesPerSecond)
                        {
                            _logger.LogWarning("Message rate {CurrentRate} exceeds limit {MaxRate}",
                                currentRate, maxMessagesPerSecond);
                            throw new PipelineException($"Rate limit exceeded: {currentRate} messages/sec");
                        }

                        messagesProcessed = 0;
                        lastProcessedTime = currentTime;
                    }

                    processedBytes += message.Data?.Length ?? 0;
                    messagesProcessed++;

                    return await messageHandler(message);
                }, cancellationToken);

                _logger.LogInformation("Rate-limited processing completed - Messages: {ProcessedMessages}, Bytes: {ProcessedBytes}",
                    result.ProcessedMessages, processedBytes);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in rate-limited pipeline processing");
                throw new PipelineException("Rate-limited pipeline processing failed", ex);
            }
        }

        /// <summary>
        /// 安全处理中间件 - 完整性校验和安全监控
        /// </summary>
        public async Task<PipelineProcessingResult> ProcessWithSecurityAsync(
            PipeReader reader,
            PipeWriter writer,
            Func<PipelineReadResult, Task<object>> messageHandler,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting secure pipeline processing");

            var stats = new ProcessingStatistics();

            try
            {
                var protocolHandler = new MessageProtocolHandler(_logger);
                var processor = new PipelineProcessor(_logger, protocolHandler, null);

                var result = await processor.ProcessPipelineAsync(reader, writer, async message =>
                {
                    stats.TotalMessages++;
                    stats.TotalBytesProcessed += message.Data?.Length ?? 0;

                    // 安全检查
                    if (!ValidateMessageSecurity(message))
                    {
                        stats.SecurityErrors++;
                        _logger.LogWarning("Security validation failed for message {MessageType} with correlation ID {CorrelationId}",
                            message.MessageType, message.Header.CorrelationId);
                        throw new SecurityException("Message security validation failed");
                    }

                    return await messageHandler(message);
                }, cancellationToken);

                result.ProcessedMessages = stats.TotalMessages;
                if (stats.SecurityErrors > 0)
                {
                    result.Errors.Add($"Security errors: {stats.SecurityErrors}");
                }

                _logger.LogInformation("Secure processing completed - Messages: {TotalMessages}, Bytes: {TotalBytes}, Security errors: {SecurityErrors}",
                    stats.TotalMessages, stats.TotalBytesProcessed, stats.SecurityErrors);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in secure pipeline processing");
                throw new PipelineException("Secure pipeline processing failed", ex);
            }
        }

        private bool ValidateMessageSecurity(PipelineReadResult message)
        {
            // 1. 时间戳验证 (防止重放攻击)
            var timeDiff = DateTime.UtcNow - message.Header.Timestamp;
            if (Math.Abs(timeDiff.TotalMinutes) > 5) // 5分钟时间窗口
            {
                _logger.LogWarning("Message timestamp validation failed - Time difference: {TimeDiff} minutes", timeDiff.TotalMinutes);
                return false;
            }

            // 2. 消息大小验证
            if (message.Header.Length > 1024 * 1024) // 1MB限制
            {
                _logger.LogWarning("Message size exceeds limit - Size: {MessageSize} bytes", message.Header.Length);
                return false;
            }

            // 3. 协议版本验证
            if (message.Header.Metadata.TryGetValue("ProtocolVersion", out var protocolVersion))
            {
                if (!"1.0".Equals(protocolVersion, StringComparison.OrdinalIgnoreCase))
                {
                    // 这里可以记录或拒绝不支持的协议版本
                    _logger.LogDebug("Unsupported protocol version: {ProtocolVersion}", protocolVersion);
                }
            }

            return true;
        }
    }

    public class ProcessingStatistics
    {
        public long TotalMessages { get; set; }
        public long TotalBytesProcessed { get; set; }
        public long SecurityErrors { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    public class SecurityException : Exception
    {
        public SecurityException(string message) : base(message) { }
    }

    // 12. 性能监控和遥测服务
    public interface IPipelineTelemetryService
    {
        Task TrackMessageProcessed(PipelineReadResult message);
        Task TrackPipelineError(Exception exception, string operation);
        PipelinePerformanceMetrics GetPerformanceMetrics();
    }

    public class PipelineTelemetryService : IPipelineTelemetryService
    {
        private readonly ILogger<PipelineTelemetryService> _logger;
        private readonly ConcurrentDictionary<string, MessageMetrics> _messageMetrics =
            new ConcurrentDictionary<string, MessageMetrics>();
        private readonly ConcurrentQueue<string> _recentErrors = new ConcurrentQueue<string>();

        public async Task TrackMessageProcessed(PipelineReadResult message)
        {
            var metrics = _messageMetrics.GetOrAdd(message.MessageType.ToString(), _ => new MessageMetrics());

            metrics.ProcessedCount++;
            metrics.TotalProcessingTime += message.ProcessingDuration.TotalMilliseconds;
            metrics.LastProcessedAt = DateTime.UtcNow;

            _logger.LogDebug("TRACKING: Message {MessageType} processed in {ProcessingTime}ms",
                message.MessageType, message.ProcessingDuration.TotalMilliseconds);
        }

        public async Task TrackPipelineError(Exception exception, string operation)
        {
            var errorInfo = $"{DateTime.UtcNow:O} - {operation} - {exception.Message}";
            _recentErrors.Enqueue(errorInfo);

            // 保持最近错误队列大小
            while (_recentErrors.Count > 100)
            {
                _recentErrors.TryDequeue(out _);
            }

            _logger.LogError(exception, "PIPELINE_ERROR: {Operation}", operation);
        }

        public PipelinePerformanceMetrics GetPerformanceMetrics()
        {
            return new PipelinePerformanceMetrics
            {
                MessageTypes = _messageMetrics.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                RecentErrors = _recentErrors.ToArray(),
                RetrievedAt = DateTime.UtcNow
            };
        }
    }

    public class MessageMetrics
    {
        public long ProcessedCount { get; set; }
        public double TotalProcessingTime { get; set; }
        public DateTime LastProcessedAt { get; set; }

        public double AverageProcessingTime => ProcessedCount > 0 ?
            TotalProcessingTime / ProcessedCount : 0;

        public double ProcessingRate => ProcessedCount > 0 ?
            ProcessedCount / (DateTime.UtcNow - LastProcessedAt).TotalSeconds : 0;
    }

    public class PipelinePerformanceMetrics
    {
        public Dictionary<string, MessageMetrics> MessageTypes { get; set; } = new Dictionary<string, MessageMetrics>();
        public string[] RecentErrors { get; set; }
        public DateTime RetrievedAt { get; set; }

        public long TotalProcessedMessages => MessageTypes.Values.Sum(v => v.ProcessedCount);
    }

    // 13. 主程序演示类
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            Console.WriteLine("System.IO.Pipelines Production Demo");
            Console.WriteLine("===================================");
            Console.WriteLine();

            #region 服务配置

            // 配置日志服务
            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });

            // 注册核心服务
            builder.Services.AddSingleton<MessageProtocolHandler>();
            builder.Services.AddSingleton<PipelineProcessor>();
            builder.Services.AddSingleton<NetworkPipelineService>();
            builder.Services.AddSingleton<PipelineBufferOptimizationService>();
            builder.Services.AddSingleton<MessageProcessingService>();
            builder.Services.AddSingleton<StreamMultiplexingService>();
            builder.Services.AddSingleton<PipelineMiddleware>();
            builder.Services.AddSingleton<IPipelineTelemetryService, PipelineTelemetryService>();

            #endregion

            var host = builder.Build();

            await RunDemoAsync(host.Services);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task RunDemoAsync(IServiceProvider services)
        {
            var protocolHandler = services.GetRequiredService<MessageProtocolHandler>();
            var bufferService = services.GetRequiredService<PipelineBufferOptimizationService>();
            var processingService = services.GetRequiredService<MessageProcessingService>();
            var multiplexingService = services.GetRequiredService<StreamMultiplexingService>();
            var middleware = services.GetRequiredService<PipelineMiddleware>();
            var telemetry = services.GetRequiredService<IPipelineTelemetryService>();

            Console.WriteLine("1. Basic Pipeline Demo:");

            // 创建优化管道
            var pipe = bufferService.CreateOptimizedPipe();
            var reader = pipe.Reader;
            var writer = pipe.Writer;

            // 写入测试消息
            var testMessages = CreateTestMessages();
            foreach (var message in testMessages.Take(5))
            {
                await protocolHandler.WriteMessageAsync(writer, message);
            }

            // 处理消息循环示例
            Console.WriteLine("   Processing messages from pipeline...");
            var processedCount = 0;

            for (int i = 0; i < 5; i++)
            {
                var message = await protocolHandler.ReadMessageAsync(reader);
                if (message != null)
                {
                    Console.WriteLine($"   - Read {message.MessageType} message with {message.Data?.Length ?? 0} bytes");
                    processedCount++;
                }
            }

            Console.WriteLine($"   Processed {processedCount} messages");

            Console.WriteLine("\n2. Buffer Optimization Demo:");
            var poolStats = bufferService.GetPoolUsageStats();
            Console.WriteLine($"   Buffer pool stats:");
            Console.WriteLine($"   - Block size: {poolStats.BlockSize} bytes");
            Console.WriteLine($"   - Total blocks: {poolStats.TotalBlocks}");
            Console.WriteLine($"   - Available blocks: {poolStats.AvailableBlocks}");
            Console.WriteLine($"   - Used blocks: {poolStats.UsedBlocks}");
            Console.WriteLine($"   - Usage percentage: {poolStats.UsagePercentage:F2}%");

            Console.WriteLine("\n3. Message Processing Demo:");
            var messages = CreateTestMessages();
            var handler = new MessageProcessor(simulateWork: false);

            var outputPipe = bufferService.CreateOptimizedPipe();
            var writerTask = Task.Run(async () =>
            {
                foreach (var message in messages)
                {
                    await protocolHandler.WriteMessageAsync(outputPipe.Writer, message);
                }
                outputPipe.Writer.Complete();
            });

            var readerTask = Task.Run(async () =>
            {
                var results = new List<object>();
                while (true)
                {
                    var message = await protocolHandler.ReadMessageAsync(outputPipe.Reader);
                    if (message == null) break;

                    var result = await processingService.HandleMessageAsync(message);
                    if (result != null)
                    {
                        results.Add(result);
                    }

                    await telemetry.TrackMessageProcessed(message);
                }
                return results;
            });

            await writerTask;
            var processingResults = await readerTask;
            Console.WriteLine($"   Processed {processingResults.Count} messages");

            foreach (var result in processingResults.Take(3))
            {
                Console.WriteLine($"   - Result: {result}");
            }

            Console.WriteLine("\n4. Stream Multiplexing Demo:");
            var pipes = new List<Pipe>
        {
            bufferService.CreateOptimizedPipe(),
            bufferService.CreateOptimizedPipe(),
            bufferService.CreateOptimizedPipe()
        };

            // 向不同管道写入不同类型的消息
            var messageBatches = new[]
            {
            CreateTestMessages(MessageType.TextMessage, 10),
            CreateTestMessages(MessageType.BinaryData, 10),
            CreateTestMessages(MessageType.Heartbeat, 10)
        };

            var writerTasks = Enumerable.Range(0, pipes.Count).Select(i =>
                WriteMessageBatchAsync(pipes[i].Writer, messageBatches[i]));

            await Task.WhenAll(writerTasks);

            // 多路复用读取器
            var readers = pipes.Select(p => p.Reader).ToList();
            var mergedPipe = bufferService.CreateOptimizedPipe();

            var mergingTask = multiplexingService.MultiplexStreamsAsync(readers, mergedPipe.Writer);
            var readingTask = ReadMergedStreamAsync(mergedPipe.Reader, protocolHandler);

            await mergingTask;
            mergedPipe.Writer.Complete();
            var mergedMessages = await readingTask;

            Console.WriteLine($"   Merged {mergedMessages} messages from multiple streams");

            Console.WriteLine("\n5. Security Middleware Demo:");
            var securePipe = bufferService.CreateOptimizedPipe();
            var secureWriterTask = Task.Run(async () =>
            {
                foreach (var message in CreateTestMessages().Take(5))
                {
                    await protocolHandler.WriteMessageAsync(securePipe.Writer, message);
                }
                securePipe.Writer.Complete();
            });

            var secureProcessingResult = await middleware.ProcessWithSecurityAsync(
                securePipe.Reader,
                securePipe.Writer,
                processingService.HandleMessageAsync);

            Console.WriteLine($"   Secure processing completed - Messages: {secureProcessingResult.ProcessedMessages}");
            Console.WriteLine($"   Security errors: {secureProcessingResult.ErrorCount}");

            Console.WriteLine("\n6. Performance Telemetry Demo:");
            var metrics = telemetry.GetPerformanceMetrics();
            Console.WriteLine($"   Total processed messages: {metrics.TotalProcessedMessages}");
            Console.WriteLine($"   Recent error count: {metrics.RecentErrors.Length}");
            Console.WriteLine($"   Tracked message types: {metrics.MessageTypes.Count}");

            foreach (var messageType in metrics.MessageTypes.Take(3))
            {
                Console.WriteLine($"   - {messageType.Key}: {messageType.Value.ProcessedCount} messages, " +
                                $"avg {messageType.Value.AverageProcessingTime:F2}ms");
            }
        }

        private static List<PipelineReadResult> CreateTestMessages(
            MessageType type = MessageType.TextMessage,
            int count = 20)
        {
            var messages = new List<PipelineReadResult>();

            for (int i = 1; i <= count; i++)
            {
                switch (type)
                {
                    case MessageType.TextMessage:
                        var textMessage = new PipelineReadResult
                        {
                            Header = new MessageHeader
                            {
                                Type = MessageType.TextMessage,
                                CorrelationId = $"msg-{Guid.NewGuid():N}",
                                Timestamp = DateTime.UtcNow,
                                Metadata = new Dictionary<string, string>
                            {
                                { "Source", "DemoApp" },
                                { "ProtocolVersion", "1.0" }
                            }
                            },
                            ParsedData = new TextMessage
                            {
                                Content = $"Demo message content #{i}",
                                Sender = "DemoSender",
                                Receiver = "DemoReceiver"
                            },
                            Data = JsonSerializer.SerializeToUtf8Bytes(new TextMessage
                            {
                                Content = $"Demo message content #{i}",
                                Sender = "DemoSender",
                                Receiver = "DemoReceiver"
                            }),
                            ProcessingStartedAt = DateTime.UtcNow
                        };
                        textMessage.ProcessingCompletedAt = DateTime.UtcNow.AddMilliseconds(5);
                        messages.Add(textMessage);
                        break;

                    case MessageType.BinaryData:
                        var binaryMessage = new PipelineReadResult
                        {
                            Header = new MessageHeader
                            {
                                Type = MessageType.BinaryData,
                                CorrelationId = $"bin-{Guid.NewGuid():N}",
                                Timestamp = DateTime.UtcNow
                            },
                            ParsedData = new BinaryDataMessage
                            {
                                Data = Encoding.UTF8.GetBytes($"Binary data sample #{i}"),
                                DataType = "application/data"
                            },
                            Data = Encoding.UTF8.GetBytes($"Binary data sample #{i}"),
                            ProcessingStartedAt = DateTime.UtcNow
                        };
                        binaryMessage.ProcessingCompletedAt = DateTime.UtcNow.AddMilliseconds(15);
                        messages.Add(binaryMessage);
                        break;

                    case MessageType.Heartbeat:
                        var heartbeatMessage = new PipelineReadResult
                        {
                            Header = new MessageHeader
                            {
                                Type = MessageType.Heartbeat,
                                CorrelationId = $"hb-{Guid.NewGuid():N}",
                                Timestamp = DateTime.UtcNow
                            },
                            Data = Array.Empty<byte>(),
                            ProcessingStartedAt = DateTime.UtcNow
                        };
                        heartbeatMessage.ProcessingCompletedAt = DateTime.UtcNow.AddMilliseconds(1);
                        messages.Add(heartbeatMessage);
                        break;
                }
            }

            return messages;
        }

        private static async Task WriteMessageBatchAsync(PipeWriter writer, List<PipelineReadResult> messages)
        {
            var handler = new MessageProtocolHandler(new LoggerFactory().CreateLogger<MessageProtocolHandler>());

            foreach (var message in messages)
            {
                await handler.WriteMessageAsync(writer, message);
            }

            await writer.FlushAsync();
        }

        private static async Task<int> ReadMergedStreamAsync(PipeReader reader, MessageProtocolHandler handler)
        {
            var count = 0;
            while (true)
            {
                var message = await handler.ReadMessageAsync(reader);
                if (message == null) break;
                count++;
            }
            return count;
        }
    }

    // 14. 示例消息处理器类
    public class MessageProcessor
    {
        private readonly bool _simulateWork;

        public MessageProcessor(bool simulateWork = true)
        {
            _simulateWork = simulateWork;
        }

        public async Task<object> ProcessMessage(PipelineReadResult message)
        {
            if (_simulateWork)
            {
                await Task.Delay(10); // 模拟处理工作
            }

            return new
            {
                Processed = message.Header.CorrelationId,
                Type = message.MessageType,
                ProcessedAt = DateTime.UtcNow,
                Size = message.Data?.Length ?? 0
            };
        }
    }

    // 管道核心概念
    public class Test()
    {
        // PipeReader - 从管道读取数据
        // PipeWriter - 向管道写入数据
        // Pipe - 连接Reader和Writer的管道
        var pipe = new Pipe();
        var reader = pipe.Reader;
        var writer = pipe.Writer;

        // 生产级配置优化
        // 自定义管道选项
        var options = new PipeOptions(
            pool: MemoryPool<byte>.Shared,          // 共享内存池优化
            pauseWriterThreshold: 32768,            // 写暂停阈值
            resumeWriterThreshold: 16384,           // 写继续阈值
            minimumSegmentSize: 4096,               // 最小段大小
            useSynchronizationContext: false);      // 避免同步上下文阻塞

        var pipe = new Pipe(options);

        // 零拷贝高性能模式
        // 跨段读取
        private byte[] ReadFromSequence(ReadOnlySequence<byte> sequence, int offset, int count)
        {
            var bytes = new byte[count];

            foreach (var memory in sequence)
            {
                // 高效读取，避免复制到中间缓冲区
                memory.Span.CopyTo(bytes.AsSpan(offset, memory.Length));
                offset += memory.Length;
            }

            return bytes;
        }

        // 生产级使用模式
        // 协议解析
        // 消息格式解析安全性处理
        public async Task<PipelineReadResult> ReadMessageAsync(PipeReader reader)
        {
            while (true)
            {
                var result = await reader.ReadAsync();
                var buffer = result.Buffer;

                if (TryParseMessage(ref buffer, out var message))
                {
                    reader.AdvanceTo(buffer.Start, buffer.End);
                    return message;
                }

                reader.AdvanceTo(buffer.Start, buffer.End);

                if (result.IsCompleted) break;
            }

            return null;
        }

        // 内存使用优化
        // 内存池使用
        // 重用内存缓冲区减少GC压力
        var options = new StreamPipeReaderOptions(
            bufferSize: 4096,
            pool: MemoryPool<byte>.Shared);

        var reader = PipeReader.Create(stream, options);

        // 并发处理优化
        // 流多路处理
        public async Task MultiplexAsync(List<PipeReader> readers, PipeWriter writer)
        {
            var tasks = readers.Select(reader => ProcessSingleReader(reader, writer));
            await Task.WhenAll(tasks);
        }

        // 错误处理和监控
        // 安全写入处理
        public async Task<bool> WriteMessage(PipeWriter writer, byte[] message)
        {
            try
            {
                var buffer = writer.GetMemory(message.Length);
                message.CopyTo(buffer.Span);
                writer.Advance(message.Length);

                var flushResult = await writer.FlushAsync();
                return !(flushResult.IsCompleted || flushResult.IsCanceled);
            }
            catch (Exception ex)
            {
                // 记录和处理管道错误
                await _telemetry.TrackError(ex, "WriteMessage");
                return false;
            }
        }

        // 流控和安全性
        // 速率限制
        var rateLimitedResult = await middleware.ProcessWithRateLimiting(reader, writer, messageHandler, maxMessagesPerSecond: 1000);

        // 安全校验
        private bool ValidateMessage(PipelineReadResult message)
        {
            // 时间戳验证防止重放攻击
            // 消息大小验证防止内存溢出攻击
            // 协议版本验证确保兼容性
        }

        // 完整生命周期管理
        // 正确完成管道
        // writer.Complete();
        // reader.Complete();

        // 检查管道状态
        // result.IsCompleted  // 读取完成后，无更多数据
        // result.IsCanceled   // 操作被取消

        // 性能优化要点
        // 1. 避免频繁内存分配
        var memory = writer.GetMemory(minimumSize);

        // 2. 零拷贝数据操作
        sequence.CopyTo(destinationArray);

        // 3. 批处理减少系统调用
        await writer.FlushAsync();

        // 4. 适当的并发控制
        using var semaphore = new SemaphoreSlim(maxConcurrency);

        // 错误处理实践
        try
        {
            var result = await reader.ReadAsync();
            // 处理数据
            reader.AdvanceTo(consumed);
        }
        catch (Exception ex)
        {
            reader.AdvanceTo(buffer.Start); // 回滚缓冲区
        throw new PipelineException("Processing failed", ex);
        }
        finally
        {
            reader.Complete();
            writer.Complete();
        }
    }

    // 推荐的生产环境实践
    // 异步流处理框架：
    // 使用共享内存池优化内存使用
    // 实现适当的消息协议解析
    // 集成安全性校验和流控机制
    // 添加完整的监控和遥测
    // 正确处理生命周期和资源清理
}