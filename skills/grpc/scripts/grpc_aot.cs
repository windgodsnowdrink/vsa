#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Grpc.AspNetCore@2.62.0
#:package Grpc.Net.Client@2.62.0
#:package Google.Protobuf@3.26.1
#:package System.Reflection.DispatchProxy@4.7.1
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property RuntimeIdentifier=win-x64
#:property SelfContained=true
#:property PublishTrimmed=true
#:property TrimMode=partial
#:property EnableCompilation=false

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GrpcAOT
{
    public class GrpcOptions
    {
        public string ServerHost { get; set; } = "localhost";
        public int ServerPort { get; set; } = 50051;
        public int MaxConcurrentCalls { get; set; } = 100;
        public int KeepAliveTimeSeconds { get; set; } = 60;
        public int MaxMessageSize { get; set; } = 4194304;
        public bool EnableTls { get; set; } = false;
        public string CertificatePath { get; set; } = "";
        public string CertificatePassword { get; set; } = "";
    }

    [ProtoContract]
    public class HelloRequest
    {
        [ProtoMember(1)]
        public string Name { get; set; }
    }

    [ProtoContract]
    public class HelloResponse
    {
        [ProtoMember(1)]
        public string Message { get; set; }
    }

    [ProtoContract]
    public class HealthCheckRequest
    {
        [ProtoMember(1)]
        public string Service { get; set; }
    }

    [ProtoContract]
    public class HealthCheckResponse
    {
        [ProtoMember(1)]
        public string Status { get; set; }
    }

    [ProtoContract]
    public class PingRequest
    {
        [ProtoMember(1)]
        public string Message { get; set; }
    }

    [ProtoContract]
    public class PingResponse
    {
        [ProtoMember(1)]
        public string Message { get; set; }
        [ProtoMember(2)]
        public long Timestamp { get; set; }
    }

    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context)
        {
            _logger.LogInformation("收到Hello请求: {Name}", request.Name);
            var response = new HelloResponse
            {
                Message = $"你好, {request.Name}! 这是来自gRPC AOT服务器的响应"
            };
            _logger.LogInformation("发送Hello响应: {Message}", response.Message);
            return Task.FromResult(response);
        }
    }

    public class HealthService : Health.HealthBase
    {
        private readonly ILogger<HealthService> _logger;

        public HealthService(ILogger<HealthService> logger)
        {
            _logger = logger;
        }

        public override Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
        {
            _logger.LogInformation("收到健康检查请求: {Service}", request.Service);
            var response = new HealthCheckResponse
            {
                Status = "SERVING"
            };
            _logger.LogInformation("发送健康检查响应: {Status}", response.Status);
            return Task.FromResult(response);
        }
    }

    public class PingService : Ping.PingBase
    {
        private readonly ILogger<PingService> _logger;

        public PingService(ILogger<PingService> logger)
        {
            _logger = logger;
        }

        public override Task<PingResponse> Ping(PingRequest request, ServerCallContext context)
        {
            _logger.LogInformation("收到Ping请求: {Message}", request.Message);
            var response = new PingResponse
            {
                Message = $"Pong: {request.Message}",
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
            _logger.LogInformation("发送Ping响应: {Message}, 时间戳: {Timestamp}", response.Message, response.Timestamp);
            return Task.FromResult(response);
        }
    }

    public class GrpcServer
    {
        private readonly GrpcOptions _options;
        private readonly ILogger<GrpcServer> _logger;
        private Server _server;

        public GrpcServer(IOptions<GrpcOptions> options, ILogger<GrpcServer> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("正在启动gRPC服务器...");
            _logger.LogInformation("服务器配置: 主机={ServerHost}, 端口={ServerPort}, 最大并发调用={MaxConcurrentCalls}", 
                _options.ServerHost, _options.ServerPort, _options.MaxConcurrentCalls);

            var serverPort = new ServerPort(_options.ServerHost, _options.ServerPort, ServerCredentials.Insecure);
            
            _server = new Server
            {
                Ports = { serverPort },
                Services = {
                    Greeter.BindService(new GreeterService(_logger)),
                    Health.BindService(new HealthService(_logger)),
                    Ping.BindService(new PingService(_logger))
                },
                MaxConcurrentCalls = _options.MaxConcurrentCalls
            };

            _server.Start();
            _logger.LogInformation("gRPC服务器已启动，监听地址: {ServerHost}:{ServerPort}", _options.ServerHost, _options.ServerPort);

            await Task.Delay(Timeout.Infinite, cancellationToken);
        }

        public async Task StopAsync()
        {
            if (_server != null)
            {
                _logger.LogInformation("正在停止gRPC服务器...");
                await _server.ShutdownAsync();
                _logger.LogInformation("gRPC服务器已停止");
            }
        }
    }

    public class GrpcClient
    {
        private readonly GrpcOptions _options;
        private readonly ILogger<GrpcClient> _logger;
        private GrpcChannel _channel;
        private Greeter.GreeterClient _greeterClient;
        private Health.HealthClient _healthClient;
        private Ping.PingClient _pingClient;

        public GrpcClient(IOptions<GrpcOptions> options, ILogger<GrpcClient> logger)
        {
            _options = options.Value;
            _logger = logger;
            InitializeChannel();
        }

        private void InitializeChannel()
        {
            var address = $"http://{_options.ServerHost}:{_options.ServerPort}";
            _logger.LogInformation("初始化gRPC客户端通道: {Address}", address);
            
            var httpClient = new HttpClient(new SocketsHttpHandler
            {
                EnableMultipleHttp2Connections = true,
                KeepAlivePingDelay = TimeSpan.FromSeconds(_options.KeepAliveTimeSeconds),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(30)
            });

            _channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions
            {
                HttpClient = httpClient,
                MaxReceiveMessageSize = _options.MaxMessageSize,
                MaxSendMessageSize = _options.MaxMessageSize
            });

            _greeterClient = new Greeter.GreeterClient(_channel);
            _healthClient = new Health.HealthClient(_channel);
            _pingClient = new Ping.PingClient(_channel);

            _logger.LogInformation("gRPC客户端通道初始化完成");
        }

        public async Task<HelloResponse> SayHelloAsync(string name, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("发送Hello请求: {Name}", name);
            var request = new HelloRequest { Name = name };
            var response = await _greeterClient.SayHelloAsync(request, cancellationToken: cancellationToken);
            _logger.LogInformation("收到Hello响应: {Message}", response.Message);
            return response;
        }

        public async Task<HealthCheckResponse> CheckHealthAsync(string service = "", CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("发送健康检查请求: {Service}", service);
            var request = new HealthCheckRequest { Service = service };
            var response = await _healthClient.CheckAsync(request, cancellationToken: cancellationToken);
            _logger.LogInformation("收到健康检查响应: {Status}", response.Status);
            return response;
        }

        public async Task<PingResponse> PingAsync(string message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("发送Ping请求: {Message}", message);
            var request = new PingRequest { Message = message };
            var response = await _pingClient.PingAsync(request, cancellationToken: cancellationToken);
            _logger.LogInformation("收到Ping响应: {Message}, 时间戳: {Timestamp}", response.Message, response.Timestamp);
            return response;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _logger.LogInformation("gRPC客户端通道已释放");
        }
    }

    public class GrpcAotEngine
    {
        private readonly GrpcOptions _options;
        private readonly ILogger<GrpcAotEngine> _logger;
        private GrpcServer _server;
        private GrpcClient _client;
        private CancellationTokenSource _cts;

        public GrpcAotEngine(IOptions<GrpcOptions> options, ILogger<GrpcAotEngine> logger)
        {
            _options = options.Value;
            _logger = logger;
            _cts = new CancellationTokenSource();
        }

        public async Task StartServerAsync()
        {
            _logger.LogInformation("启动gRPC AOT服务器...");
            _server = new GrpcServer(Options.Create(_options), _logger);
            await _server.StartAsync(_cts.Token);
        }

        public async Task StopServerAsync()
        {
            _cts.Cancel();
            if (_server != null)
            {
                await _server.StopAsync();
            }
        }

        public GrpcClient CreateClient()
        {
            _logger.LogInformation("创建gRPC客户端...");
            _client = new GrpcClient(Options.Create(_options), _logger);
            return _client;
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            _client?.Dispose();
            _logger.LogInformation("gRPC AOT引擎已释放");
        }
    }

    public class GrpcCommandHandler
    {
        private readonly GrpcOptions _options;
        private readonly ILogger<GrpcCommandHandler> _logger;
        private GrpcAotEngine _engine;

        public GrpcCommandHandler(IOptions<GrpcOptions> options, ILogger<GrpcCommandHandler> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task HandleCommandAsync(string[] args)
        {
            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            var command = args[0].ToLower();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                switch (command)
                {
                    case "server":
                        await HandleServerCommand(args);
                        break;
                    case "client":
                        await HandleClientCommand(args);
                        break;
                    case "hello":
                        await HandleHelloCommand(args);
                        break;
                    case "ping":
                        await HandlePingCommand(args);
                        break;
                    case "health":
                        await HandleHealthCommand(args);
                        break;
                    case "config":
                        await HandleConfigCommand(args);
                        break;
                    case "help":
                    case "--help":
                    case "-h":
                        ShowHelp();
                        break;
                    default:
                        _logger.LogError("未知命令: {Command}", command);
                        ShowHelp();
                        break;
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "执行命令时发生错误");
                Console.WriteLine($"错误: {ex.Message}");
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("命令执行完成，耗时: {Elapsed}ms", stopwatch.ElapsedMilliseconds);
            }
        }

        private async Task HandleServerCommand(string[] args)
        {
            _logger.LogInformation("启动gRPC服务器...");
            _engine = new GrpcAotEngine(Options.Create(_options), _logger);

            try
            {
                await _engine.StartServerAsync();
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("服务器启动被取消");
            }
            finally
            {
                _engine.Dispose();
            }
        }

        private async Task HandleClientCommand(string[] args)
        {
            _logger.LogInformation("启动gRPC客户端...");
            _engine = new GrpcAotEngine(Options.Create(_options), _logger);

            try
            {
                var client = _engine.CreateClient();
                Console.WriteLine("gRPC客户端已启动，请使用具体的客户端命令进行操作");
                Console.WriteLine("例如: grpc_aot.cs hello 世界");
            }
            finally
            {
                _engine.Dispose();
            }
        }

        private async Task HandleHelloCommand(string[] args)
        {
            string name = "World";
            if (args.Length > 1)
            {
                name = args[1];
            }

            _engine = new GrpcAotEngine(Options.Create(_options), _logger);
            try
            {
                var client = _engine.CreateClient();
                var response = await client.SayHelloAsync(name);
                Console.WriteLine($"\n响应: {response.Message}");
            }
            finally
            {
                _engine.Dispose();
            }
        }

        private async Task HandlePingCommand(string[] args)
        {
            string message = "Hello gRPC";
            if (args.Length > 1)
            {
                message = args[1];
            }

            _engine = new GrpcAotEngine(Options.Create(_options), _logger);
            try
            {
                var client = _engine.CreateClient();
                var response = await client.PingAsync(message);
                Console.WriteLine($"\n响应: {response.Message}");
                Console.WriteLine($"时间戳: {response.Timestamp}");
                Console.WriteLine($"当前时间: {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}");
            }
            finally
            {
                _engine.Dispose();
            }
        }

        private async Task HandleHealthCommand(string[] args)
        {
            string service = "";
            if (args.Length > 1)
            {
                service = args[1];
            }

            _engine = new GrpcAotEngine(Options.Create(_options), _logger);
            try
            {
                var client = _engine.CreateClient();
                var response = await client.CheckHealthAsync(service);
                Console.WriteLine($"\n健康状态: {response.Status}");
            }
            finally
            {
                _engine.Dispose();
            }
        }

        private async Task HandleConfigCommand(string[] args)
        {
            Console.WriteLine("\ngRPC配置信息:");
            Console.WriteLine("=================================");
            Console.WriteLine($"服务器主机: {_options.ServerHost}");
            Console.WriteLine($"服务器端口: {_options.ServerPort}");
            Console.WriteLine($"最大并发调用: {_options.MaxConcurrentCalls}");
            Console.WriteLine($"心跳时间: {_options.KeepAliveTimeSeconds}秒");
            Console.WriteLine($"最大消息大小: {_options.MaxMessageSize}字节");
            Console.WriteLine($"启用TLS: {_options.EnableTls}");
            Console.WriteLine($"证书路径: {_options.CertificatePath}");
            Console.WriteLine("=================================");
        }

        private void ShowHelp()
        {
            Console.WriteLine("\ngRPC AOT 命令行工具");
            Console.WriteLine("=================================");
            Console.WriteLine("命令列表:");
            Console.WriteLine("  server        - 启动gRPC服务器");
            Console.WriteLine("  client        - 启动gRPC客户端");
            Console.WriteLine("  hello         - 发送Hello请求");
            Console.WriteLine("    参数: <名称> (可选，默认为World)");
            Console.WriteLine("  ping          - 发送Ping请求");
            Console.WriteLine("    参数: <消息> (可选，默认为Hello gRPC)");
            Console.WriteLine("  health        - 发送健康检查请求");
            Console.WriteLine("    参数: <服务名称> (可选)");
            Console.WriteLine("  config        - 显示配置信息");
            Console.WriteLine("  help          - 显示帮助信息");
            Console.WriteLine("=================================");
        }
    }

    public static class GrpcProtobuf
    {
        public static class Greeter
        {
            public static readonly string ServiceName = "greeter.Greeter";

            public static Grpc.Core.ServerServiceDefinition BindService(GreeterBase serviceImpl)
            {
                return Grpc.Core.ServerServiceDefinition.CreateBuilder()
                    .AddMethod(Method, "SayHello", serviceImpl.SayHello)
                    .Build();
            }

            public static Grpc.Core.Method<HelloRequest, HelloResponse> Method = 
                new Grpc.Core.Method<HelloRequest, HelloResponse>(
                    Grpc.Core.MethodType.Unary,
                    ServiceName,
                    "SayHello",
                    Grpc.Core.Marshallers.Create(HelloRequest.Parser.ParseFrom, HelloRequest.ToByteArray),
                    Grpc.Core.Marshallers.Create(HelloResponse.Parser.ParseFrom, HelloResponse.ToByteArray));

            public abstract class GreeterBase
            {
                public virtual Task<HelloResponse> SayHello(HelloRequest request, Grpc.Core.ServerCallContext context)
                {
                    throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Unimplemented, "方法未实现"));
                }
            }

            public class GreeterClient
            {
                private readonly Grpc.Core.CallInvoker _callInvoker;

                public GreeterClient(Grpc.Core.CallInvoker callInvoker)
                {
                    _callInvoker = callInvoker;
                }

                public async Task<HelloResponse> SayHelloAsync(HelloRequest request, Grpc.Core.CallOptions options = default)
                {
                    return await _callInvoker.AsyncUnaryCall(Method, null, options, request);
                }
            }
        }

        public static class Health
        {
            public static readonly string ServiceName = "grpc.health.v1.Health";

            public static Grpc.Core.ServerServiceDefinition BindService(HealthBase serviceImpl)
            {
                return Grpc.Core.ServerServiceDefinition.CreateBuilder()
                    .AddMethod(Method, "Check", serviceImpl.Check)
                    .Build();
            }

            public static Grpc.Core.Method<HealthCheckRequest, HealthCheckResponse> Method = 
                new Grpc.Core.Method<HealthCheckRequest, HealthCheckResponse>(
                    Grpc.Core.MethodType.Unary,
                    ServiceName,
                    "Check",
                    Grpc.Core.Marshallers.Create(HealthCheckRequest.Parser.ParseFrom, HealthCheckRequest.ToByteArray),
                    Grpc.Core.Marshallers.Create(HealthCheckResponse.Parser.ParseFrom, HealthCheckResponse.ToByteArray));

            public abstract class HealthBase
            {
                public virtual Task<HealthCheckResponse> Check(HealthCheckRequest request, Grpc.Core.ServerCallContext context)
                {
                    throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Unimplemented, "方法未实现"));
                }
            }

            public class HealthClient
            {
                private readonly Grpc.Core.CallInvoker _callInvoker;

                public HealthClient(Grpc.Core.CallInvoker callInvoker)
                {
                    _callInvoker = callInvoker;
                }

                public async Task<HealthCheckResponse> CheckAsync(HealthCheckRequest request, Grpc.Core.CallOptions options = default)
                {
                    return await _callInvoker.AsyncUnaryCall(Method, null, options, request);
                }
            }
        }

        public static class Ping
        {
            public static readonly string ServiceName = "ping.Ping";

            public static Grpc.Core.ServerServiceDefinition BindService(PingBase serviceImpl)
            {
                return Grpc.Core.ServerServiceDefinition.CreateBuilder()
                    .AddMethod(Method, "Ping", serviceImpl.Ping)
                    .Build();
            }

            public static Grpc.Core.Method<PingRequest, PingResponse> Method = 
                new Grpc.Core.Method<PingRequest, PingResponse>(
                    Grpc.Core.MethodType.Unary,
                    ServiceName,
                    "Ping",
                    Grpc.Core.Marshallers.Create(PingRequest.Parser.ParseFrom, PingRequest.ToByteArray),
                    Grpc.Core.Marshallers.Create(PingResponse.Parser.ParseFrom, PingResponse.ToByteArray));

            public abstract class PingBase
            {
                public virtual Task<PingResponse> Ping(PingRequest request, Grpc.Core.ServerCallContext context)
                {
                    throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Unimplemented, "方法未实现"));
                }
            }

            public class PingClient
            {
                private readonly Grpc.Core.CallInvoker _callInvoker;

                public PingClient(Grpc.Core.CallInvoker callInvoker)
                {
                    _callInvoker = callInvoker;
                }

                public async Task<PingResponse> PingAsync(PingRequest request, Grpc.Core.CallOptions options = default)
                {
                    return await _callInvoker.AsyncUnaryCall(Method, null, options, request);
                }
            }
        }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.AddDebug();
                    builder.SetMinimumLevel(LogLevel.Information);
                })
                .Configure<GrpcOptions>(options =>
                {
                    options.ServerHost = Environment.GetEnvironmentVariable("GRPC_SERVER_HOST") ?? "localhost";
                    options.ServerPort = int.Parse(Environment.GetEnvironmentVariable("GRPC_SERVER_PORT") ?? "50051");
                    options.MaxConcurrentCalls = int.Parse(Environment.GetEnvironmentVariable("GRPC_MAX_CONCURRENT_CALLS") ?? "100");
                    options.KeepAliveTimeSeconds = int.Parse(Environment.GetEnvironmentVariable("GRPC_KEEP_ALIVE_TIME_SECONDS") ?? "60");
                    options.MaxMessageSize = int.Parse(Environment.GetEnvironmentVariable("GRPC_MAX_MESSAGE_SIZE") ?? "4194304");
                    options.EnableTls = bool.Parse(Environment.GetEnvironmentVariable("GRPC_ENABLE_TLS") ?? "false");
                    options.CertificatePath = Environment.GetEnvironmentVariable("GRPC_CERTIFICATE_PATH") ?? "";
                    options.CertificatePassword = Environment.GetEnvironmentVariable("GRPC_CERTIFICATE_PASSWORD") ?? "";
                })
                .AddSingleton<GrpcCommandHandler>()
                .BuildServiceProvider();

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            var commandHandler = serviceProvider.GetRequiredService<GrpcCommandHandler>();

            try
            {
                logger.LogInformation("gRPC AOT 引擎启动");
                await commandHandler.HandleCommandAsync(args);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "应用程序启动失败");
                Console.WriteLine($"错误: {ex.Message}");
            }
            finally
            {
                if (serviceProvider is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                logger.LogInformation("gRPC AOT 引擎已关闭");
            }
        }
    }
}
