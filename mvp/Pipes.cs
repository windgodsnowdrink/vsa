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
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Principal;
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

    // 1. 管道通信消息模型
    public class PipeMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string MessageType { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public object Data { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }

    // 2. 管道通信异常定义
    public class PipeCommunicationException : Exception
    {
        public PipeCommunicationException(string message) : base(message) { }
        public PipeCommunicationException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class PipeSecurityException : Exception
    {
        public PipeSecurityException(string message) : base(message) { }
    }

    // 3. 管道安全管理器 - 生产级安全控制
    public class PipeSecurityManager
    {
        private readonly ILogger<PipeSecurityManager> _logger;
        private readonly HashSet<string> _allowedUsers;
        private readonly HashSet<string> _blockedUsers;

        public PipeSecurityManager(ILogger<PipeSecurityManager> logger)
        {
            _logger = logger;
            _allowedUsers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _blockedUsers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 初始化默认允许用户
            InitializeDefaultUsers();
        }

        /// <summary>
        /// 验证管道连接用户权限
        /// </summary>
        public bool ValidateUserAccess(string userName, string pipeName)
        {
            _logger.LogDebug("Validating access for user {UserName} to pipe {PipeName}", userName, pipeName);

            try
            {
                // 检查黑名单
                if (_blockedUsers.Contains(userName))
                {
                    _logger.LogWarning("User {UserName} is blocked from accessing pipe {PipeName}", userName, pipeName);
                    return false;
                }

                // 检查白名单
                if (_allowedUsers.Count > 0 && !_allowedUsers.Contains(userName))
                {
                    _logger.LogWarning("User {UserName} is not authorized to access pipe {PipeName}", userName, pipeName);
                    return false;
                }

                _logger.LogDebug("User {UserName} authorized for pipe {PipeName}", userName, pipeName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user access for {UserName}", userName);
                throw new PipeSecurityException("Failed to validate user access");
            }
        }

        /// <summary>
        /// 获取当前用户名称
        /// </summary>
        public string GetCurrentUserName()
        {
            try
            {
                var identity = WindowsIdentity.GetCurrent();
                var name = identity.Name;
                _logger.LogDebug("Current user identity retrieved: {UserName}", name);
                return name;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user identity");
                throw new PipeSecurityException("Failed to get current user identity", ex);
            }
        }

        /// <summary>
        /// 添加允许访问的用户
        /// </summary>
        public void AddAllowedUser(string userName)
        {
            _logger.LogInformation("Adding allowed user: {UserName}", userName);
            _allowedUsers.Add(userName);
        }

        /// <summary>
        /// 添加被阻止的用户
        /// </summary>
        public void AddBlockedUser(string userName)
        {
            _logger.LogWarning("Adding blocked user: {UserName}", userName);
            _blockedUsers.Add(userName);
        }

        /// <summary>
        /// 获取安全用户列表
        /// </summary>
        public SecurityUsersInfo GetSecurityUsersInfo()
        {
            return new SecurityUsersInfo
            {
                AllowedUsers = new List<string>(_allowedUsers),
                BlockedUsers = new List<string>(_blockedUsers),
                CurrentUser = GetCurrentUserName()
            };
        }

        private void InitializeDefaultUsers()
        {
            _allowedUsers.Add(GetCurrentUserName());
            _allowedUsers.Add("SYSTEM");
            _allowedUsers.Add("LOCAL SERVICE");
            _allowedUsers.Add("NETWORK SERVICE");
        }
    }

    // 4. 安全用户信息模型
    public class SecurityUsersInfo
    {
        public string CurrentUser { get; set; }
        public List<string> AllowedUsers { get; set; } = new List<string>();
        public List<string> BlockedUsers { get; set; } = new List<string>();
        public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
    }

    // 5. 生产级命名管道服务客户端
    public class NamedPipeClientService
    {
        private readonly ILogger<NamedPipeClientService> _logger;
        private readonly PipeSecurityManager _securityManager;

        public NamedPipeClientService(
            ILogger<NamedPipeClientService> logger,
            PipeSecurityManager securityManager)
        {
            _logger = logger;
            _securityManager = securityManager;
        }

        /// <summary>
        /// 发送消息到命名管道服务器
        /// </summary>
        public async Task<PipeMessage> SendMessageAsync(
            string pipeName,
            PipeMessage message,
            int timeoutMilliseconds = 30000,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Sending message {MessageId} of type {MessageType} to pipe {PipeName}",
                message.Id, message.MessageType, pipeName);

            NamedPipeClientStream clientStream = null;

            try
            {
                // 创建命名管道客户端
                clientStream = new NamedPipeClientStream(
                    ".",                  // 本地计算机
                    pipeName,             // 管道名称
                    PipeDirection.InOut,  // 读写
                    PipeOptions.Asynchronous | PipeOptions.WriteThrough); // 异步和直通写入

                // 连接服务器（带超时）
                using var timeoutToken = new CancellationTokenSource(timeoutMilliseconds);
                using var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken, timeoutToken.Token);

                await clientStream.ConnectAsync(combinedToken.Token);

                // 验证服务器用户权限
                var serverIdentity = clientStream.GetImpersonationUserName();
                if (!string.IsNullOrEmpty(serverIdentity) &&
                    !_securityManager.ValidateUserAccess(serverIdentity, pipeName))
                {
                    throw new PipeSecurityException($"Unauthorized server: {serverIdentity}");
                }

                _logger.LogDebug("Connected to named pipe server {PipeName} successfully", pipeName);

                // 发送消息
                await WritePipeMessageAsync(clientStream, message, cancellationToken);

                // 读取响应
                var response = await ReadPipeMessageAsync(clientStream, cancellationToken);

                _logger.LogInformation("Message exchange completed with pipe {PipeName} - Response ID: {ResponseId}",
                    pipeName, response?.Id ?? "null");

                return response;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Pipe client communication cancelled by token");
                throw new PipeCommunicationException("Communication cancelled");
            }
            catch (TimeoutException)
            {
                _logger.LogError("Pipe client connection timeout for pipe {PipeName}", pipeName);
                throw new PipeCommunicationException("Connection timeout");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error communicating with pipe server {PipeName}", pipeName);
                throw new PipeCommunicationException($"Failed to communicate with pipe server: {ex.Message}", ex);
            }
            finally
            {
                clientStream?.Dispose();
            }
        }

        /// <summary>
        /// 批量发送消息到管道服务器
        /// </summary>
        public async Task<List<PipeMessage>> SendBatchMessagesAsync(
            string pipeName,
            List<PipeMessage> messages,
            int maxConcurrency = 5,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Sending batch of {MessageCount} messages to pipe {PipeName}",
                messages.Count, pipeName);

            var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
            var results = new List<PipeMessage>();
            var tasks = new List<Task<PipeMessage>>();

            try
            {
                foreach (var message in messages)
                {
                    var task = Task.Run(async () =>
                    {
                        await semaphore.WaitAsync(cancellationToken);
                        try
                        {
                            return await SendMessageAsync(pipeName, message, cancellationToken: cancellationToken);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }, cancellationToken);

                    tasks.Add(task);
                }

                // 等待所有批次完成
                results = new List<PipeMessage>(await Task.WhenAll(tasks));

                _logger.LogInformation("Batch message sending completed - Success: {SuccessCount}",
                    results.Count(r => r != null));

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during batch message sending to pipe {PipeName}", pipeName);
                throw new PipeCommunicationException("Batch message sending failed", ex);
            }
        }

        private async Task WritePipeMessageAsync(
            NamedPipeClientStream clientStream,
            PipeMessage message,
            CancellationToken cancellationToken)
        {
            try
            {
                // 序列化消息
                var json = JsonSerializer.Serialize(message);
                var messageBytes = Encoding.UTF8.GetBytes(json);

                // 写入消息长度（前4字节）
                var lengthBytes = BitConverter.GetBytes(messageBytes.Length);
                await clientStream.WriteAsync(lengthBytes, 0, lengthBytes.Length, cancellationToken);

                // 写入消息内容
                await clientStream.WriteAsync(messageBytes, 0, messageBytes.Length, cancellationToken);
                await clientStream.FlushAsync(cancellationToken);

                _logger.LogDebug("Message {MessageId} written to pipe successfully", message.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing message {MessageId} to pipe", message.Id);
                throw new PipeCommunicationException("Failed to write message to pipe", ex);
            }
        }

        private async Task<PipeMessage> ReadPipeMessageAsync(
            NamedPipeClientStream clientStream,
            CancellationToken cancellationToken)
        {
            try
            {
                // 读取消息长度
                var lengthBuffer = new byte[4];
                await clientStream.ReadAsync(lengthBuffer, 0, lengthBuffer.Length, cancellationToken);
                var messageLength = BitConverter.ToInt32(lengthBuffer, 0);

                // 读取消息内容
                var messageBuffer = new byte[messageLength];
                var totalRead = 0;
                while (totalRead < messageLength)
                {
                    var read = await clientStream.ReadAsync(
                        messageBuffer, totalRead, messageLength - totalRead, cancellationToken);
                    if (read == 0) break;
                    totalRead += read;
                }

                var json = Encoding.UTF8.GetString(messageBuffer);
                var responseMessage = JsonSerializer.Deserialize<PipeMessage>(json);

                _logger.LogDebug("Response message {MessageId} read from pipe", responseMessage?.Id ?? "null");

                return responseMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading response from pipe");
                throw new PipeCommunicationException("Failed to read response from pipe", ex);
            }
        }
    }

    // 6. 生产级命名管道服务器服务
    public class NamedPipeServerService
    {
        private readonly ILogger<NamedPipeServerService> _logger;
        private readonly PipeSecurityManager _securityManager;
        private readonly List<NamedPipeServerStream> _serverStreams = new List<NamedPipeServerStream>();
        private readonly SemaphoreSlim _connectionSemaphore;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isRunning;

        public NamedPipeServerService(
            ILogger<NamedPipeServerService> logger,
            PipeSecurityManager securityManager,
            int maxConcurrentConnections = 10)
        {
            _logger = logger;
            _securityManager = securityManager;
            _connectionSemaphore = new SemaphoreSlim(maxConcurrentConnections, maxConcurrentConnections);
        }

        /// <summary>
        /// 启动命名管道服务器
        /// </summary>
        public async Task StartServerAsync(
            string pipeName,
            Func<PipeMessage, Task<PipeMessage>> messageHandler,
            int instanceCount = 5,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting named pipe server for {PipeName} with {InstanceCount} instances",
                pipeName, instanceCount);

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _isRunning = true;

            var tasks = new List<Task>();

            try
            {
                // 创建多个管道实例以支持并发连接
                for (int i = 0; i < instanceCount; i++)
                {
                    var serverTask = StartServerInstanceAsync(pipeName, messageHandler, _cancellationTokenSource.Token);
                    tasks.Add(serverTask);

                    _logger.LogDebug("Server instance {InstanceNumber} started for pipe {PipeName}",
                        i + 1, pipeName);
                }

                // 等待所有实例完成
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Named pipe server cancelled by token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in named pipe server operation");
                throw new PipeCommunicationException("Named pipe server failed", ex);
            }
            finally
            {
                _isRunning = false;
                await StopServerAsync();
            }
        }

        /// <summary>
        /// 启动单个管道服务器实例
        /// </summary>
        private async Task StartServerInstanceAsync(
            string pipeName,
            Func<PipeMessage, Task<PipeMessage>> messageHandler,
            CancellationToken cancellationToken)
        {
            NamedPipeServerStream serverStream = null;

            try
            {
                while (!cancellationToken.IsCancellationRequested && _isRunning)
                {
                    // 创建管道服务器流
                    serverStream = new NamedPipeServerStream(
                        pipeName,
                        PipeDirection.InOut,
                        NamedPipeServerStream.MaxAllowedServerInstances,
                        PipeTransmissionMode.Message,
                        PipeOptions.Asynchronous | PipeOptions.WriteThrough);

                    _serverStreams.Add(serverStream);

                    _logger.LogDebug("Waiting for client connection on pipe {PipeName}", pipeName);

                    // 等待客户端连接
                    await serverStream.WaitForConnectionAsync(cancellationToken);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    _logger.LogDebug("Client connected to pipe {PipeName}", pipeName);

                    // 验证客户端身份
                    if (!await ValidateClientAsync(serverStream, pipeName, cancellationToken))
                    {
                        _logger.LogWarning("Client validation failed, closing connection");
                        serverStream.Disconnect();
                        continue;
                    }

                    // 获取并发执行许可
                    await _connectionSemaphore.WaitAsync(cancellationToken);

                    try
                    {
                        // 处理客户端连接
                        await HandleClientConnectionAsync(serverStream, messageHandler, cancellationToken);
                    }
                    finally
                    {
                        _connectionSemaphore.Release();
                    }

                    // 断开当前连接，准备下一个连接
                    serverStream.Disconnect();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Server instance cancellation requested for pipe {PipeName}", pipeName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in server instance for pipe {PipeName}", pipeName);
            }
            finally
            {
                serverStream?.Dispose();
                _serverStreams.Remove(serverStream);
            }
        }

        /// <summary>
        /// 验证客户端连接安全性
        /// </summary>
        private async Task<bool> ValidateClientAsync(
            NamedPipeServerStream serverStream,
            string pipeName,
            CancellationToken cancellationToken)
        {
            await Task.Yield(); // 异步上下文切换

            try
            {
                var clientIdentity = serverStream.GetImpersonationUserName();

                if (string.IsNullOrEmpty(clientIdentity))
                {
                    _logger.LogDebug("Client connected anonymously to pipe {PipeName}", pipeName);
                    return true; // 允许匿名连接（根据需求调整）
                }

                var isValid = _securityManager.ValidateUserAccess(clientIdentity, pipeName);

                if (!isValid)
                {
                    _logger.LogWarning("Unauthorized client {ClientIdentity} attempted connection to {PipeName}",
                        clientIdentity, pipeName);
                }
                else
                {
                    _logger.LogInformation("Authorized client {ClientIdentity} connected to {PipeName}",
                        clientIdentity, pipeName);
                }

                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating client for pipe {PipeName}", pipeName);
                return false;
            }
        }

        /// <summary>
        /// 处理客户端连接和消息交换
        /// </summary>
        private async Task HandleClientConnectionAsync(
            NamedPipeServerStream serverStream,
            Func<PipeMessage, Task<PipeMessage>> messageHandler,
            CancellationToken cancellationToken)
        {
            var connectionId = Guid.NewGuid().ToString("N");
            _logger.LogInformation("Handling client connection {ConnectionId}", connectionId);

            try
            {
                while (serverStream.IsConnected && !cancellationToken.IsCancellationRequested)
                {
                    // 读取客户端消息
                    var message = await ReadClientMessageAsync(serverStream, cancellationToken);
                    if (message == null) break;

                    _logger.LogDebug("Received message {MessageId} of type {MessageType} from client {ConnectionId}",
                        message.Id, message.MessageType, connectionId);

                    // 处理消息
                    PipeMessage responseMessage;
                    try
                    {
                        responseMessage = await messageHandler(message);

                        // 添加服务器响应标识
                        responseMessage.Metadata["ServerResponse"] = "true";
                        responseMessage.Metadata["ProcessingServer"] = Environment.MachineName;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error handling message {MessageId}", message.Id);

                        // 创建错误响应
                        responseMessage = new PipeMessage
                        {
                            MessageType = "ErrorResponse",
                            Data = new
                            {
                                Error = ex.Message,
                                ErrorCode = ex.GetType().Name,
                                OriginalMessageId = message.Id
                            }
                        };
                    }

                    // 发送响应
                    await WriteServerResponseAsync(serverStream, responseMessage, cancellationToken);

                    _logger.LogDebug("Response {ResponseMessageId} sent to client {ConnectionId}",
                        responseMessage.Id, connectionId);
                }

                _logger.LogInformation("Client connection {ConnectionId} completed", connectionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling client connection {ConnectionId}", connectionId);
            }
        }

        /// <summary>
        /// 读取客户端发送的消息
        /// </summary>
        private async Task<PipeMessage> ReadClientMessageAsync(
            NamedPipeServerStream serverStream,
            CancellationToken cancellationToken)
        {
            try
            {
                // 读取消息长度
                var lengthBuffer = new byte[4];
                var lengthBytesRead = await serverStream.ReadAsync(lengthBuffer, 0, 4, cancellationToken);
                if (lengthBytesRead < 4) return null;

                var messageLength = BitConverter.ToInt32(lengthBuffer, 0);

                if (messageLength <= 0 || messageLength > 1024 * 1024) // 1MB最大限制
                {
                    throw new PipeCommunicationException($"Invalid message length: {messageLength}");
                }

                // 读取消息内容
                var messageBuffer = new byte[messageLength];
                var bytesRead = await serverStream.ReadAsync(messageBuffer, 0, messageLength, cancellationToken);

                if (bytesRead < messageLength)
                {
                    throw new PipeCommunicationException("Incomplete message received");
                }

                var json = Encoding.UTF8.GetString(messageBuffer);
                var message = JsonSerializer.Deserialize<PipeMessage>(json);

                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading client message");
                throw new PipeCommunicationException("Failed to read client message", ex);
            }
        }

        /// <summary>
        /// 向客户端发送服务器响应
        /// </summary>
        private async Task WriteServerResponseAsync(
            NamedPipeServerStream serverStream,
            PipeMessage responseMessage,
            CancellationToken cancellationToken)
        {
            try
            {
                // 序列化响应消息
                var json = JsonSerializer.Serialize(responseMessage);
                var messageBytes = Encoding.UTF8.GetBytes(json);

                // 写入消息长度
                var lengthBytes = BitConverter.GetBytes(messageBytes.Length);
                await serverStream.WriteAsync(lengthBytes, 0, lengthBytes.Length, cancellationToken);

                // 写入消息内容
                await serverStream.WriteAsync(messageBytes, 0, messageBytes.Length, cancellationToken);
                await serverStream.FlushAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing server response");
                throw new PipeCommunicationException("Failed to write server response", ex);
            }
        }

        /// <summary>
        /// 停止管道服务器
        /// </summary>
        public async Task StopServerAsync()
        {
            _logger.LogInformation("Stopping named pipe server");

            _cancellationTokenSource?.Cancel();
            _isRunning = false;

            // 断开所有连接
            foreach (var stream in _serverStreams.ToArray())
            {
                try
                {
                    if (stream.IsConnected)
                    {
                        stream.Disconnect();
                    }
                    stream.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error disconnecting server stream");
                }
            }

            _serverStreams.Clear();
        }
    }

    // 7. 管道性能监控服务
    public interface IPipeTelemetryService
    {
        Task TrackConnectionEstablished(string pipeName, string clientName);
        Task TrackMessageProcessed(string pipeName, string messageType, TimeSpan duration);
        Task TrackError(string pipeName, Exception exception, string operation);
        PipeMetrics GetMetrics(string pipeName);
        void ResetMetrics(string pipeName);
    }

    public class PipeTelemetryService : IPipeTelemetryService
    {
        private readonly ILogger<PipeTelemetryService> _logger;
        private readonly ConcurrentDictionary<string, PipeMetrics> _pipeMetrics;

        public PipeTelemetryService(ILogger<PipeTelemetryService> logger)
        {
            _logger = logger;
            _pipeMetrics = new ConcurrentDictionary<string, PipeMetrics>();
        }

        public async Task TrackConnectionEstablished(string pipeName, string clientName)
        {
            var metrics = _pipeMetrics.GetOrAdd(pipeName, _ => new PipeMetrics { PipeName = pipeName });
            metrics.ConnectionCount++;
            metrics.LastConnectionTime = DateTime.UtcNow;

            _logger.LogDebug("Connection established - Pipe: {PipeName}, Client: {ClientName}, Total: {ConnectionCount}",
                pipeName, clientName, metrics.ConnectionCount);
        }

        public async Task TrackMessageProcessed(string pipeName, string messageType, TimeSpan duration)
        {
            var metrics = _pipeMetrics.GetOrAdd(pipeName, _ => new PipeMetrics { PipeName = pipeName });
            metrics.MessageCount++;
            metrics.TotalProcessingTime += duration.TotalMilliseconds;
            metrics.MessageTypes.Add(messageType);
            metrics.LastMessageTime = DateTime.UtcNow;

            _logger.LogDebug("Message processed - Pipe: {PipeName}, Type: {MessageType}, Duration: {Duration}ms",
                pipeName, messageType, duration.TotalMilliseconds);
        }

        public async Task TrackError(string pipeName, Exception exception, string operation)
        {
            var metrics = _pipeMetrics.GetOrAdd(pipeName, _ => new PipeMetrics { PipeName = pipeName });
            metrics.ErrorCount++;
            metrics.LastErrorMessage = exception.Message;

            // 防止错误队列过大
            while (metrics.RecentErrors.Count >= 100)
            {
                metrics.RecentErrors.RemoveAt(0);
            }

            metrics.RecentErrors.Add(new PipeErrorInfo
            {
                Operation = operation,
                ErrorMessage = exception.Message,
                Timestamp = DateTime.UtcNow
            });

            _logger.LogError(exception, "Error tracked - Pipe: {PipeName}, Operation: {Operation}",
                pipeName, operation);
        }

        public PipeMetrics GetMetrics(string pipeName)
        {
            if (_pipeMetrics.TryGetValue(pipeName, out var metrics))
            {
                metrics.LastUpdated = DateTime.UtcNow;
                metrics.AverageProcessingTime = metrics.MessageCount > 0 ?
                    metrics.TotalProcessingTime / metrics.MessageCount : 0;
                return metrics;
            }

            return new PipeMetrics { PipeName = pipeName };
        }

        public void ResetMetrics(string pipeName)
        {
            _pipeMetrics.TryRemove(pipeName, out _);
            _logger.LogInformation("Metrics reset for pipe {PipeName}", pipeName);
        }
    }

    // 8. 管道监控模型类
    public class PipeMetrics
    {
        public string PipeName { get; set; }
        public long ConnectionCount { get; set; }
        public long MessageCount { get; set; }
        public long ErrorCount { get; set; }
        public double TotalProcessingTime { get; set; }
        public double AverageProcessingTime { get; set; }
        public HashSet<string> MessageTypes { get; set; } = new HashSet<string>();
        public DateTime LastConnectionTime { get; set; }
        public DateTime LastMessageTime { get; set; }
        public string LastErrorMessage { get; set; }
        public List<PipeErrorInfo> RecentErrors { get; set; } = new List<PipeErrorInfo>();
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public double ConnectionRate => (DateTime.UtcNow - LastConnectionTime).TotalSeconds > 0 ?
            ConnectionCount / (DateTime.UtcNow - LastConnectionTime).TotalSeconds : 0;

        public double MessageRate => (DateTime.UtcNow - LastMessageTime).TotalSeconds > 0 ?
            MessageCount / (DateTime.UtcNow - LastMessageTime).TotalSeconds : 0;
    }

    public class PipeErrorInfo
    {
        public string Operation { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime Timestamp { get; set; }
    }

    // 9. 管道管理服务 - 生产环境管道生命周期管理
    public class PipeManagementService
    {
        private readonly ILogger<PipeManagementService> _logger;
        private readonly IPipeTelemetryService _telemetry;
        private readonly PipeSecurityManager _securityManager;
        private readonly List<NamedPipeServerService> _servers = new List<NamedPipeServerService>();

        public PipeManagementService(
            ILogger<PipeManagementService> logger,
            IPipeTelemetryService telemetry,
            PipeSecurityManager securityManager)
        {
            _logger = logger;
            _telemetry = telemetry;
            _securityManager = securityManager;
        }

        /// <summary>
        /// 创建和管理命名管道服务器
        /// </summary>
        public async Task<NamedPipeServerService> CreateNamedPipeServerAsync(
            string pipeName,
            Func<PipeMessage, Task<PipeMessage>> messageHandler,
            int maxConcurrentConnections = 10,
            int instanceCount = 5)
        {
            _logger.LogInformation("Creating named pipe server {PipeName}", pipeName);

            var server = new NamedPipeServerService(_logger, _securityManager, maxConcurrentConnections);
            _servers.Add(server);

            // 在后台启动服务器
            _ = Task.Run(async () =>
            {
                try
                {
                    await server.StartServerAsync(pipeName, messageHandler, instanceCount);
                }
                catch (Exception ex)
                {
                    await _telemetry.TrackError(pipeName, ex, "ServerStart");
                }
            });

            return server;
        }

        /// <summary>
        /// 获取服务器运行状态
        /// </summary>
        public PipeServerStatus GetServerStatus(string pipeName)
        {
            var status = new PipeServerStatus
            {
                PipeName = pipeName,
                IsRunning = true, // 简化实现，实际应检查服务器状态
                Metrics = _telemetry.GetMetrics(pipeName),
                SecurityInfo = _securityManager.GetSecurityUsersInfo(),
                Uptime = DateTime.UtcNow
            };

            return status;
        }

        /// <summary>
        /// 重启指定管道服务器
        /// </summary>
        public async Task<bool> RestartPipeServerAsync(
            string pipeName,
            Func<PipeMessage, Task<PipeMessage>> messageHandler)
        {
            _logger.LogInformation("Restarting pipe server {PipeName}", pipeName);

            try
            {
                // 停止现有服务器
                await StopPipeServerAsync(pipeName);

                // 重新创建服务器实例
                var server = await CreateNamedPipeServerAsync(pipeName, messageHandler);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restarting pipe server {PipeName}", pipeName);
                return false;
            }
        }

        /// <summary>
        /// 停止指定管道服务器
        /// </summary>
        public async Task StopPipeServerAsync(string pipeName)
        {
            _logger.LogInformation("Stopping pipe server {PipeName}", pipeName);

            var server = _servers.FirstOrDefault(s =>
                s.GetType().GetProperty("Logger")?.GetValue(s)?.ToString()?.Contains(pipeName) == true);

            if (server != null)
            {
                await server.StopServerAsync();
                _servers.Remove(server);
            }

            _logger.LogInformation("Pipe server {PipeName} stopped", pipeName);
        }

        /// <summary>
        /// 获取系统中所有命名管道信息
        /// </summary>
        public List<string> GetSystemNamedPipes()
        {
            _logger.LogDebug("Retrieving system named pipes information");

            try
            {
                var processes = Process.GetProcessesByName("services");
                var pipeNames = new List<string>();

                // 实际生产环境可以通过更安全的方式获取管道信息
                return pipeNames;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving system named pipes");
                return new List<string>();
            }
        }

        /// <summary>
        /// 清理所有管道资源
        /// </summary>
        public async Task CleanupAsync()
        {
            _logger.LogInformation("Cleaning up pipe management service resources");

            var stopTasks = _servers.Select(server => server.StopServerAsync()).ToArray();
            await Task.WhenAll(stopTasks);

            _servers.Clear();
        }
    }

    // 10. 管道服务器状态信息
    public class PipeServerStatus
    {
        public string PipeName { get; set; }
        public bool IsRunning { get; set; }
        public PipeMetrics Metrics { get; set; }
        public SecurityUsersInfo SecurityInfo { get; set; }
        public DateTime Uptime { get; set; }
        public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
    }

    // 11. 管道客户端池服务 - 生产环境连接池
    public class PooledPipeClientService
    {
        private readonly ILogger<PooledPipeClientService> _logger;
        private readonly string _pipeName;
        private readonly int _maxPoolSize;
        private readonly Queue<NamedPipeClientStream> _clientPool;
        private readonly SemaphoreSlim _poolSemaphore;
        private readonly SemaphoreSlim _creationSemaphore;

        public PooledPipeClientService(
            ILogger<PooledPipeClientService> logger,
            string pipeName,
            int maxPoolSize = 10)
        {
            _logger = logger;
            _pipeName = pipeName;
            _maxPoolSize = maxPoolSize;
            _clientPool = new Queue<NamedPipeClientStream>();
            _poolSemaphore = new SemaphoreSlim(maxPoolSize, maxPoolSize);
            _creationSemaphore = new SemaphoreSlim(5, 5); // 限制同时创建的连接数
        }

        /// <summary>
        /// 从池中获取管道客户端连接
        /// </summary>
        public async Task<NamedPipeClientStream> GetClientAsync(
            CancellationToken cancellationToken = default)
        {
            await _poolSemaphore.WaitAsync(cancellationToken);

            lock (_clientPool)
            {
                if (_clientPool.Count > 0)
                {
                    var client = _clientPool.Dequeue();
                    if (client.IsConnected)
                    {
                        _logger.LogDebug("Reusing pooled pipe client connection");
                        return client;
                    }
                    else
                    {
                        client.Dispose();
                    }
                }
            }

            // 创建新连接
            await _creationSemaphore.WaitAsync(cancellationToken);

            try
            {
                _logger.LogDebug("Creating new pipe client connection");
                var newClient = new NamedPipeClientStream(
                    ".", _pipeName, PipeDirection.InOut,
                    PipeOptions.Asynchronous | PipeOptions.WriteThrough);

                await newClient.ConnectAsync(5000, cancellationToken); // 5秒连接超时

                return newClient;
            }
            catch (Exception ex)
            {
                _poolSemaphore.Release(); // 如果创建失败，释放池信号量
                _logger.LogError(ex, "Error creating pipe client connection");
                throw new PipeCommunicationException("Failed to create pipe client connection", ex);
            }
            finally
            {
                _creationSemaphore.Release();
            }
        }

        /// <summary>
        /// 归还客户端连接到池中
        /// </summary>
        public void ReturnClient(NamedPipeClientStream client)
        {
            if (client == null) return;

            lock (_clientPool)
            {
                if (_clientPool.Count < _maxPoolSize && client.IsConnected)
                {
                    _clientPool.Enqueue(client);
                    _logger.LogDebug("Pipe client returned to pool - Pool size: {PoolSize}", _clientPool.Count);
                    return;
                }
            }

            // 如果池满了或者连接已断开，直接销毁
            client.Dispose();
            _poolSemaphore.Release();
            _logger.LogDebug("Pipe client connection disposed");
        }

        /// <summary>
        /// 使用池中的连接发送消息
        /// </summary>
        public async Task<PipeMessage> SendPooledMessageAsync(
            PipeMessage message,
            CancellationToken cancellationToken = default)
        {
            NamedPipeClientStream client = null;

            try
            {
                client = await GetClientAsync(cancellationToken);

                // 发送消息并接收响应
                var json = JsonSerializer.Serialize(message);
                var messageBytes = Encoding.UTF8.GetBytes(json);
                var lengthBytes = BitConverter.GetBytes(messageBytes.Length);

                await client.WriteAsync(lengthBytes, 0, lengthBytes.Length, cancellationToken);
                await client.WriteAsync(messageBytes, 0, messageBytes.Length, cancellationToken);
                await client.FlushAsync(cancellationToken);

                // 读取响应
                var responseLengthBuffer = new byte[4];
                await client.ReadAsync(responseLengthBuffer, 0, 4, cancellationToken);
                var responseLength = BitConverter.ToInt32(responseLengthBuffer, 0);

                var responseBuffer = new byte[responseLength];
                await client.ReadAsync(responseBuffer, 0, responseLength, cancellationToken);

                var responseJson = Encoding.UTF8.GetString(responseBuffer);
                var responseMessage = JsonSerializer.Deserialize<PipeMessage>(responseJson);

                return responseMessage;
            }
            finally
            {
                ReturnClient(client);
            }
        }
    }

    // 12. 管道协议消息处理服务
    public class PipeMessageHandlerService
    {
        private readonly ILogger<PipeMessageHandlerService> _logger;
        private readonly IPipeTelemetryService _telemetry;

        public PipeMessageHandlerService(
            ILogger<PipeMessageHandlerService> logger,
            IPipeTelemetryService telemetry)
        {
            _logger = logger;
            _telemetry = telemetry;
        }

        /// <summary>
        /// 核心消息处理逻辑
        /// </summary>
        public async Task<PipeMessage> HandleMessageAsync(PipeMessage requestMessage)
        {
            var startTime = DateTime.UtcNow;
            var processingDuration = TimeSpan.Zero;

            try
            {
                _logger.LogInformation("Handling pipe message {MessageId} of type {MessageType}",
                    requestMessage.Id, requestMessage.MessageType);

                PipeMessage response;

                switch (requestMessage.MessageType.ToLower())
                {
                    case "ping":
                        response = await HandlePingMessageAsync(requestMessage);
                        break;

                    case "echo":
                        response = await HandleEchoMessageAsync(requestMessage);
                        break;

                    case "data":
                        response = await HandleDataMessageAsync(requestMessage);
                        break;

                    case "query":
                        response = await HandleQueryMessageAsync(requestMessage);
                        break;

                    case "command":
                        response = await HandleCommandMessageAsync(requestMessage);
                        break;

                    default:
                        await ProcessCustomMessageType(requestMessage);
                        response = CreateSuccessResponse(requestMessage, "Custom message type processed");
                        break;
                }

                processingDuration = DateTime.UtcNow - startTime;
                await _telemetry.TrackMessageProcessed(
                    requestMessage.Metadata.GetValueOrDefault("SourcePipe", "unknown"),
                    requestMessage.MessageType,
                    processingDuration);

                _logger.LogDebug("Message handling completed in {Duration}ms", processingDuration.TotalMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                processingDuration = DateTime.UtcNow - startTime;

                await _telemetry.TrackError(
                    requestMessage.Metadata.GetValueOrDefault("SourcePipe", "unknown"),
                    ex,
                    "MessageHandling");

                _logger.LogError(ex, "Error handling pipe message {MessageId}", requestMessage.Id);

                return CreateErrorResponse(requestMessage, ex);
            }
        }

        private async Task<PipeMessage> HandlePingMessageAsync(PipeMessage message)
        {
            await Task.Delay(1); // 最小延迟模拟处理

            return new PipeMessage
            {
                MessageType = "Pong",
                Data = new
                {
                    ServerTime = DateTime.UtcNow,
                    Status = "Alive",
                    Uptime = (DateTime.UtcNow - message.Timestamp).TotalMilliseconds
                },
                Metadata = new Dictionary<string, string>
                {
                    ["PingId"] = message.Id,
                    ["ResponseServer"] = Environment.MachineName
                }
            };
        }

        private async Task<PipeMessage> HandleEchoMessageAsync(PipeMessage message)
        {
            await Task.Delay(5); // 模拟处理延迟

            return new PipeMessage
            {
                MessageType = "Echo",
                Data = message.Data,
                Metadata = new Dictionary<string, string>
                {
                    ["EchoSource"] = message.Id,
                    ["EchoServer"] = Environment.MachineName,
                    ["EchoTime"] = DateTime.UtcNow.ToString("O")
                }
            };
        }

        private async Task<PipeMessage> HandleDataMessageAsync(PipeMessage message)
        {
            await Task.Delay(10); // 模拟数据处理

            var data = message.Data as JsonElement;
            var processedData = new
            {
                OriginalData = data.ToString(),
                ProcessedAt = DateTime.UtcNow,
                Server = Environment.MachineName,
                ProcessedSize = data.ToString()?.Length ?? 0
            };

            return new PipeMessage
            {
                MessageType = "DataResponse",
                Data = processedData,
                Metadata = new Dictionary<string, string>
                {
                    ["DataProcessor"] = "DataHandler",
                    ["ProcessingTime"] = (DateTime.UtcNow - message.Timestamp).TotalMilliseconds.ToString()
                }
            };
        }

        private async Task<PipeMessage> HandleQueryMessageAsync(PipeMessage message)
        {
            await Task.Delay(50); // 模拟查询延迟

            var queryData = new
            {
                QueryId = (message.Data as JsonElement).GetProperty("QueryId").GetString(),
                Results = new[]
                {
                new { Name = "Result1", Value = "Data1" },
                new { Name = "Result2", Value = "Data2" },
                new { Name = "Result3", Value = "Data3" }
            },
                Count = 3,
                ExecutionTime = DateTime.UtcNow
            };

            return new PipeMessage
            {
                MessageType = "QueryResults",
                Data = queryData,
                Metadata = new Dictionary<string, string>
                {
                    ["QuerySource"] = message.Id,
                    ["QueryServer"] = Environment.MachineName
                }
            };
        }

        private async Task<PipeMessage> HandleCommandMessageAsync(PipeMessage message)
        {
            await Task.Delay(100); // 模拟命令执行延迟

            var command = (message.Data as JsonElement).GetProperty("Command").GetString();
            var result = ExecuteCommand(command);

            return new PipeMessage
            {
                MessageType = "CommandResponse",
                Data = result,
                Metadata = new Dictionary<string, string>
                {
                    ["Command"] = command,
                    ["Server"] = Environment.MachineName,
                    ["ExecutionStatus"] = "Completed"
                }
            };
        }

        private async Task ProcessCustomMessageType(PipeMessage message)
        {
            // 处理自定义消息类型
            await Task.Delay(25);
            _logger.LogInformation("Processing custom message type: {MessageType}", message.MessageType);
        }

        private PipeMessage CreateSuccessResponse(PipeMessage request, string resultMessage)
        {
            return new PipeMessage
            {
                MessageType = "Success",
                Data = new { Message = resultMessage, RequestId = request.Id },
                Metadata = new Dictionary<string, string>
                {
                    ["ResponseType"] = "Success",
                    ["SourceMessage"] = request.Id
                }
            };
        }

        private PipeMessage CreateErrorResponse(PipeMessage request, Exception error)
        {
            return new PipeMessage
            {
                MessageType = "Error",
                Data = new
                {
                    Error = error.Message,
                    ErrorCode = error.GetType().Name,
                    RequestId = request.Id
                },
                Metadata = new Dictionary<string, string>
                {
                    ["ResponseType"] = "Error",
                    ["ErrorSource"] = request.Id,
                    ["ErrorTime"] = DateTime.UtcNow.ToString("O")
                }
            };
        }

        private object ExecuteCommand(string command)
        {
            return new
            {
                Command = command,
                Result = "Command executed successfully",
                ExecutionTime = DateTime.UtcNow
            };
        }
    }

    // 13. 主程序演示类
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            Console.WriteLine("System.IO.Pipes Production Demo");
            Console.WriteLine("===============================");
            Console.WriteLine();

            #region 服务配置

            // 配置日志服务
            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });

            // 注册核心管道服务
            builder.Services.AddSingleton<PipeSecurityManager>();
            builder.Services.AddSingleton<NamedPipeClientService>();
            builder.Services.AddSingleton<IPipeTelemetryService, PipeTelemetryService>();
            builder.Services.AddSingleton<PipeManagementService>();
            builder.Services.AddSingleton<PipeMessageHandlerService>();

            #endregion

            var host = builder.Build();

            await RunDemoAsync(host.Services);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task RunDemoAsync(IServiceProvider services)
        {
            var pipeName = "demo_pipe_" + Guid.NewGuid().ToString("N");
            var pipeRootName = pipeName;

            var securityManager = services.GetRequiredService<PipeSecurityManager>();
            var clientService = services.GetRequiredService<NamedPipeClientService>();
            var managementService = services.GetRequiredService<PipeManagementService>();
            var telemetryService = services.GetRequiredService<IPipeTelemetryService>();
            var messageHandler = services.GetRequiredService<PipeMessageHandlerService>();

            Console.WriteLine("1. Starting Named Pipe Server:");
            try
            {
                var server = await managementService.CreateNamedPipeServerAsync(
                    pipeRootName,
                    messageHandler.HandleMessageAsync);

                Console.WriteLine($"   Server started for pipe: {pipeRootName}");

                // 等待服务器启动
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error starting server: {ex.Message}");
            }

            Console.WriteLine("\n2. Security Management Demo:");
            try
            {
                var currentUser = securityManager.GetCurrentUserName();
                var isAllowed = securityManager.ValidateUserAccess(currentUser, pipeRootName);

                Console.WriteLine($"   Current user: {currentUser}");
                Console.WriteLine($"   Access allowed: {isAllowed}");

                if (!isAllowed)
                {
                    securityManager.AddAllowedUser(currentUser);
                    Console.WriteLine($"   Added current user to allowed list");
                }

                var securityInfo = securityManager.GetSecurityUsersInfo();
                Console.WriteLine($"   Allowed users count: {securityInfo.AllowedUsers.Count}");
                Console.WriteLine($"   Blocked users count: {securityInfo.BlockedUsers.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Security management error: {ex.Message}");
            }

            Console.WriteLine("\n3. Pipe Message Communication Demo:");
            try
            {
                // 发送不同类型的消息测试
                var testMessages = CreateTestMessages();

                foreach (var testMessage in testMessages.Take(3))
                {
                    var response = await clientService.SendMessageAsync(
                        pipeRootName, testMessage, timeoutMilliseconds: 10000);

                    Console.WriteLine($"   Sent {testMessage.MessageType}, received {response?.MessageType}");

                    if (response != null)
                    {
                        var responseJson = JsonSerializer.Serialize(response.Data);
                        Console.WriteLine($"   Response data: {responseJson.Substring(0, Math.Min(100, responseJson.Length))}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Communication error: {ex.Message}");
            }

            Console.WriteLine("\n4. Batch Message Sending Demo:");
            try
            {
                var batchMessages = CreateSampleMessages(10);
                var responses = await clientService.SendBatchMessagesAsync(
                    pipeRootName, batchMessages, maxConcurrency: 3);

                Console.WriteLine($"   Sent batch of {batchMessages.Count} messages");
                Console.WriteLine($"   Received {responses.Count(r => r != null)} responses");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Batch sending error: {ex.Message}");
            }

            Console.WriteLine("\n5. Pipe Telemetry Demo:");
            try
            {
                await telemetryService.TrackConnectionEstablished(pipeRootName, "DemoClient");

                var metrics = telemetryService.GetMetrics(pipeRootName);
                Console.WriteLine($"   Connection count: {metrics.ConnectionCount}");
                Console.WriteLine($"   Message count: {metrics.MessageCount}");
                Console.WriteLine($"   Error count: {metrics.ErrorCount}");
                Console.WriteLine($"   Average processing time: {metrics.AverageProcessingTime:F2}ms");

                foreach (var messageType in metrics.MessageTypes.Take(3))
                {
                    Console.WriteLine($"   - Handled message type: {messageType}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Telemetry error: {ex.Message}");
            }

            Console.WriteLine("\n6. Server Status Demo:");
            try
            {
                var status = managementService.GetServerStatus(pipeRootName);
                Console.WriteLine($"   Server: {pipeRootName}");
                Console.WriteLine($"   Running: {status.IsRunning}");
                Console.WriteLine($"   Connections handled: {status.Metrics.ConnectionCount}");
                Console.WriteLine($"   Messages processed: {status.Metrics.MessageCount}");
                Console.WriteLine($"   Uptime: {DateTime.UtcNow - status.Uptime}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Server status error: {ex.Message}");
            }

            Console.WriteLine("\n7. Anonymous Pipe Demo:");
            try
            {
                await DemoAnonymousPipeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Anonymous pipe error: {ex.Message}");
            }

            Console.WriteLine("\n8. Pipe Direction Control Demo:");
            try
            {
                await DemoBidirectionalPipesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Bidirectional pipe error: {ex.Message}");
            }

            // 清理资源
            await Task.Delay(1000);
            await managementService.CleanupAsync();

            Console.WriteLine("\n=== Demo Complete ===");
        }

        private static List<PipeMessage> CreateTestMessages()
        {
            return new List<PipeMessage>
        {
            new PipeMessage
            {
                MessageType = "ping",
                Data = new { Message = "Ping test" }
            },
            new PipeMessage
            {
                MessageType = "echo",
                Data = new { Content = "Echo test message", Type = "test" }
            },
            new PipeMessage
            {
                MessageType = "data",
                Data = new { Id = 123, Name = "TestData", Values = new[] { 1, 2, 3, 4, 5 } }
            },
            new PipeMessage
            {
                MessageType = "query",
                Data = new { QueryId = "Q001", Parameters = new { Type = "user_search" } }
            },
            new PipeMessage
            {
                MessageType = "command",
                Data = new { Command = "clear_cache", Arguments = new object[0] }
            }
        };
        }

        private static List<PipeMessage> CreateSampleMessages(int count)
        {
            var messages = new List<PipeMessage>();

            for (int i = 1; i <= count; i++)
            {
                messages.Add(new PipeMessage
                {
                    MessageType = "echo",
                    Data = new { Message = $"Sample message #{i}", Index = i }
                });
            }

            return messages;
        }

        private static async Task DemoAnonymousPipeAsync()
        {
            Console.WriteLine("   Creating anonymous pipe...");

            using var pipeServer = new AnonymousPipeServerStream(PipeDirection.Out);
            using var pipeClient = new AnonymousPipeClientStream(PipeDirection.In, pipeServer.ClientSafePipeHandle);

            var serverTask = Task.Run(async () =>
            {
                using var writer = new StreamWriter(pipeServer);
                writer.AutoFlush = true;

                for (int i = 1; i <= 5; i++)
                {
                    await writer.WriteLineAsync($"Message {i} from anonymous pipe server");
                    await Task.Delay(100);
                }

                pipeServer.DisposeLocalCopyOfClientHandle();
            });

            var clientTask = Task.Run(async () =>
            {
                using var reader = new StreamReader(pipeClient);
                var messages = new List<string>();

                for (int i = 0; i < 5; i++)
                {
                    var message = await reader.ReadLineAsync();
                    if (message != null)
                    {
                        messages.Add(message);
                        Console.WriteLine($"   - Received: {message}");
                    }
                }

                return messages;
            });

            await Task.WhenAll(serverTask, clientTask);
            Console.WriteLine("   Anonymous pipe communication completed");
        }

        private static async Task DemoBidirectionalPipesAsync()
        {
            Console.WriteLine("   Demonstrating bidirectional pipe handling...");

            // 创建两个管道处理双向通信
            using var serverOut = new AnonymousPipeServerStream(PipeDirection.Out);
            using var serverIn = new AnonymousPipeServerStream(PipeDirection.In);

            using var clientIn = new AnonymousPipeClientStream(PipeDirection.In, serverOut.ClientSafePipeHandle);
            using var clientOut = new AnonymousPipeClientStream(PipeDirection.Out, serverIn.ClientSafePipeHandle);

            var serverTask = Task.Run(async () =>
            {
                using var writer = new StreamWriter(serverOut) { AutoFlush = true };
                using var reader = new StreamReader(serverIn);

                await writer.WriteLineAsync("Hello from server");
                var response = await reader.ReadLineAsync();
                Console.WriteLine($"   Server received client response: {response}");
            });

            var clientTask = Task.Run(async () =>
            {
                using var reader = new StreamReader(clientIn);
                using var writer = new StreamWriter(clientOut) { AutoFlush = true };

                var message = await reader.ReadLineAsync();
                Console.WriteLine($"   Client received server message: {message}");

                await writer.WriteLineAsync("Hello back from client");
            });

            await Task.WhenAll(serverTask, clientTask);
            Console.WriteLine("   Bidirectional pipe communication completed");
        }
    }

    // 生产级安全控制
    // 用户身份验证
    public class PipeSecurityManager
    {
        public void Tast()
        {
            // 命名管道 (Named Pipes)
            // 服务器端创建
            // 创建命名管道服务器流
            var serverStream = new NamedPipeServerStream(
                pipeName,                    // 管道名称
                PipeDirection.InOut,         // 读写方向
                NamedPipeServerStream.MaxAllowedServerInstances, // 实例数量限制
                PipeTransmissionMode.Message, // 消息传输模式
                PipeOptions.Asynchronous | PipeOptions.WriteThrough); // 异步和直通写入选项

            // 客户端连接
            // 创建命名管道客户端流
            var clientStream = new NamedPipeClientStream(
                serverName,                 // 服务器名称（"."表示本地）
                pipeName,                   // 管道名称
                PipeDirection.InOut,        // 读写方向
                PipeOptions.Asynchronous | PipeOptions.WriteThrough); // 异步选项

            await clientStream.ConnectAsync(timeout, cancellationToken);

            // 匿名管道 (Anonymous Pipes)
            // 单向通信创建
            // 服务器端创建匿名管道
            using var serverStream = new AnonymousPipeServerStream(PipeDirection.Out);

            // 客户端端创建匿名管道
            using var clientStream = new AnonymousPipeClientStream(PipeDirection.In, clientHandle);

            // 双向通信模式
            // 使用两个匿名管道实现双向通信
            using var serverToClient = new AnonymousPipeServerStream(PipeDirection.Out);
            using var clientToServer = new AnonymousPipeServerStream(PipeDirection.In);

            // 在另一个进程中
            using var clientInput = new AnonymousPipeClientStream(PipeDirection.In, serverToClient.ClientHandle);
            using var clientOutput = new AnonymousPipeClientStream(PipeDirection.Out, clientToServer.ClientHandle);
        }

        // 验证连接用户的权限
        public bool ValidateUserAccess(string userName, string pipeName)

        // 获取当前用户身份
        public string GetCurrentUserName()

        // 访问控制列表管理
        private HashSet<string> _allowedUsers;
        private HashSet<string> _blockedUsers;
    }

    // 连接池优化
    // 客户端连接池
    public class PooledPipeClientService
    {
        // 连接池管理
        private Queue<NamedPipeClientStream> _clientPool;

        // 获取连接
        public async Task<NamedPipeClientStream> GetClientAsync()

        // 归还连接
        public void ReturnClient(NamedPipeClientStream client) { }

        // 异步处理和并发控制
        // 异步连接处理
        await serverStream.WaitForConnectionAsync(cancellationToken);
        await serverStream.ReadAsync(buffer, offset, count, cancellationToken);
        await serverStream.WriteAsync(buffer, offset, count, cancellationToken);
        await serverStream.FlushAsync(cancellationToken);

        // 并发连接限制
        private readonly SemaphoreSlim _connectionSemaphore = new SemaphoreSlim(maxConcurrentConnections, maxConcurrentConnections);
        await _connectionSemaphore.WaitAsync(cancellationToken);
        try
        {
            await HandleClientConnectionAsync();
        }
        finally
        {
            _connectionSemaphore.Release();
        }

        // 错误处理和恢复
        // 优雅错误处
        try
        {
            await pipeCommunication();
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // 处理取消操作
        }
        catch (TimeoutException)
        {
            // 处理超时错误
        }
        catch (UnauthorizedAccessException)
        {
            // 处理权限错误
        }
        catch (Exception ex)
        {
            // 记录通用错误并进行恢复操作
        }

        // 推荐的生产环境实践
        // 1. 消息长度前缀
        var lengthBytes = BitConverter.GetBytes(message.Length);
        await stream.WriteAsync(lengthBytes);

        // 2. 消息内容
        await stream.WriteAsync(messageBytes);
        await stream.FlushAsync();

        // 3. 读取响应
        var responseLengthBuffer = new byte[4];
        await stream.ReadAsync(responseLengthBuffer);

        // 资源管理
        // 确保正确释放资源
        using var serverStream = new NamedPipeServerStream(...);
        using var clientStream = new NamedPipeClientStream(...);

        // 异步启动时使用适当的资源清理
        var cancellationSource = new CancellationTokenSource();
        try
        {
            await StartServerAsync(cancellationSource.Token);
        }
        finally
        {
            cancellationSource.Dispose();
        }

        // 安全性最佳实践
        // 1. 身份验证
        var clientIdentity = stream.GetImpersonationUserName();
        if (!ValidateUserAccess(clientIdentity)) return;

        // 2. 消息大小限制
        if (message.Length > MaxMessageSize) throw new SecurityException();

        // 3. 时间戳验证防止重放攻击
        if (DateTime.UtcNow - message.Timestamp > MaxTimeDrift)
            throw new SecurityException();
    }

    // 监控和遥测
    // 性能监控服务
    public interface IPipeTelemetryService
    {
        Task TrackConnectionEstablished(string pipeName, string clientName);
        Task TrackMessageProcessed(string pipeName, string messageType, TimeSpan duration);
        Task TrackError(string pipeName, Exception exception, string operation);
        PipeMetrics GetMetrics(string pipeName);
    }
}