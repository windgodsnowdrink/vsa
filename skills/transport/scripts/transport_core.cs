#:sdk Microsoft.NET.Sdk
#:package System.CommandLine@2.0.0-beta4.22272.1
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Microsoft.Extensions.Logging@8.0.0
#:package Microsoft.Extensions.Logging.Console@8.0.0
#:package System.Net.Http@8.0.0
#:package System.IO.Pipelines@8.0.0
#:package System.Threading.Channels@8.0.0
#:package System.Buffers@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property SelfContained=true
#:property PublishSingleFile=true

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Channels;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TransportSkill
{
    public interface IHttpTransportService
    {
        Task<string> SendRequestAsync(string url, string method = "GET", string data = null, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> SendHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
    }

    public interface ITcpTransportService
    {
        Task<string> SendMessageAsync(string host, int port, string message, CancellationToken cancellationToken = default);
        Task StartServerAsync(int port, Func<string, Task<string>> handler, CancellationToken cancellationToken = default);
    }

    public interface IUdpTransportService
    {
        Task SendMessageAsync(string host, int port, string message, CancellationToken cancellationToken = default);
        Task<string> ReceiveMessageAsync(int port, CancellationToken cancellationToken = default);
    }

    public interface IPipelineTransportService
    {
        Task ProcessPipelineAsync(int itemCount, CancellationToken cancellationToken = default);
        Task<byte[]> ReadFromPipelineAsync(PipeReader reader, CancellationToken cancellationToken = default);
        Task WriteToPipelineAsync(PipeWriter writer, byte[] data, CancellationToken cancellationToken = default);
    }

    public interface IChannelTransportService
    {
        Task ProcessChannelAsync(int messageCount, CancellationToken cancellationToken = default);
        Task SendToChannelAsync<T>(ChannelWriter<T> writer, T message, CancellationToken cancellationToken = default);
        Task<T> ReceiveFromChannelAsync<T>(ChannelReader<T> reader, CancellationToken cancellationToken = default);
    }

    public interface IBufferTransportService
    {
        Task ProcessBufferAsync(int bufferSize, CancellationToken cancellationToken = default);
        byte[] RentBuffer(int size);
        void ReturnBuffer(byte[] buffer);
    }

    public class HttpTransportService : IHttpTransportService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpTransportService> _logger;

        public HttpTransportService(ILogger<HttpTransportService> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient {
                Timeout = TimeSpan.FromMilliseconds(int.Parse(Environment.GetEnvironmentVariable("TRANSPORT_TIMEOUT") ?? "30000"))
            };
        }

        public async Task<string> SendRequestAsync(string url, string method = "GET", string data = null, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"发送HTTP请求: {method} {url}");
            
            var request = new HttpRequestMessage(new HttpMethod(method), url);
            
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            
            if (!string.IsNullOrEmpty(data))
            {
                request.Content = new StringContent(data, Encoding.UTF8, "application/json");
            }
            
            var response = await SendHttpRequestAsync(request, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            
            _logger.LogInformation($"HTTP响应状态: {response.StatusCode}");
            return content;
        }

        public async Task<HttpResponseMessage> SendHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HTTP请求失败");
                throw;
            }
        }
    }

    public class TcpTransportService : ITcpTransportService
    {
        private readonly ILogger<TcpTransportService> _logger;
        private readonly int _bufferSize = int.Parse(Environment.GetEnvironmentVariable("TRANSPORT_TCP_BUFFER_SIZE") ?? "8192");

        public TcpTransportService(ILogger<TcpTransportService> logger)
        {
            _logger = logger;
        }

        public async Task<string> SendMessageAsync(string host, int port, string message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"发送TCP消息到 {host}:{port}");
            
            using var client = new TcpClient();
            await client.ConnectAsync(host, port, cancellationToken);
            
            using var stream = client.GetStream();
            var data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length, cancellationToken);
            
            var buffer = new byte[_bufferSize];
            var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
            var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            
            _logger.LogInformation($"收到TCP响应: {response}");
            return response;
        }

        public async Task StartServerAsync(int port, Func<string, Task<string>> handler, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"启动TCP服务器在端口 {port}");
            
            using var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var client = await listener.AcceptTcpClientAsync(cancellationToken);
                    _ = ProcessClientAsync(client, handler, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("TCP服务器已停止");
            }
            finally
            {
                listener.Stop();
            }
        }

        private async Task ProcessClientAsync(TcpClient client, Func<string, Task<string>> handler, CancellationToken cancellationToken)
        {
            try
            {
                using (client)
                using (var stream = client.GetStream())
                {
                    var buffer = new byte[_bufferSize];
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    
                    _logger.LogInformation($"收到TCP消息: {message}");
                    var response = await handler(message);
                    
                    var responseData = Encoding.UTF8.GetBytes(response);
                    await stream.WriteAsync(responseData, 0, responseData.Length, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理TCP客户端失败");
            }
        }
    }

    public class UdpTransportService : IUdpTransportService
    {
        private readonly ILogger<UdpTransportService> _logger;
        private readonly int _bufferSize = int.Parse(Environment.GetEnvironmentVariable("TRANSPORT_UDP_BUFFER_SIZE") ?? "8192");

        public UdpTransportService(ILogger<UdpTransportService> logger)
        {
            _logger = logger;
        }

        public async Task SendMessageAsync(string host, int port, string message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"发送UDP消息到 {host}:{port}");
            
            using var client = new UdpClient();
            var data = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(data, data.Length, host, port, cancellationToken);
            
            _logger.LogInformation("UDP消息发送完成");
        }

        public async Task<string> ReceiveMessageAsync(int port, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"开始接收UDP消息在端口 {port}");
            
            using var client = new UdpClient(port);
            var result = await client.ReceiveAsync(cancellationToken);
            var message = Encoding.UTF8.GetString(result.Buffer);
            
            _logger.LogInformation($"收到UDP消息: {message} 来自 {result.RemoteEndPoint}");
            return message;
        }
    }

    public class PipelineTransportService : IPipelineTransportService
    {
        private readonly ILogger<PipelineTransportService> _logger;
        private readonly int _minSize = int.Parse(Environment.GetEnvironmentVariable("TRANSPORT_PIPELINE_MIN_SIZE") ?? "512");
        private readonly int _maxSize = int.Parse(Environment.GetEnvironmentVariable("TRANSPORT_PIPELINE_MAX_SIZE") ?? "65536");

        public PipelineTransportService(ILogger<PipelineTransportService> logger)
        {
            _logger = logger;
        }

        public async Task ProcessPipelineAsync(int itemCount, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"处理管道传输，项目数量: {itemCount}");
            
            var pipe = new Pipe(new PipeOptions {
                MinimumSegmentSize = _minSize,
                MaximumSizeHighWatermark = _maxSize
            });
            
            var writerTask = WriteItemsAsync(pipe.Writer, itemCount, cancellationToken);
            var readerTask = ReadItemsAsync(pipe.Reader, itemCount, cancellationToken);
            
            await Task.WhenAll(writerTask, readerTask);
            _logger.LogInformation("管道传输处理完成");
        }

        private async Task WriteItemsAsync(PipeWriter writer, int itemCount, CancellationToken cancellationToken)
        {
            try
            {
                for (int i = 0; i < itemCount; i++)
                {
                    var data = Encoding.UTF8.GetBytes($"Item {i}");
                    await WriteToPipelineAsync(writer, data, cancellationToken);
                    await Task.Delay(10, cancellationToken);
                }
                await writer.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "写入管道失败");
                await writer.CompleteAsync(ex);
            }
        }

        private async Task ReadItemsAsync(PipeReader reader, int itemCount, CancellationToken cancellationToken)
        {
            try
            {
                int count = 0;
                while (count < itemCount)
                {
                    var data = await ReadFromPipelineAsync(reader, cancellationToken);
                    if (data.Length == 0) break;
                    var message = Encoding.UTF8.GetString(data);
                    _logger.LogInformation($"读取管道数据: {message}");
                    count++;
                }
                await reader.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "读取管道失败");
                await reader.CompleteAsync(ex);
            }
        }

        public async Task<byte[]> ReadFromPipelineAsync(PipeReader reader, CancellationToken cancellationToken = default)
        {
            var result = await reader.ReadAsync(cancellationToken);
            var buffer = result.Buffer;
            
            try
            {
                if (buffer.IsEmpty && result.IsCompleted)
                {
                    return Array.Empty<byte>();
                }
                
                var length = (int)buffer.Length;
                var data = new byte[length];
                buffer.CopyTo(data);
                
                return data;
            }
            finally
            {
                reader.AdvanceTo(buffer.End);
            }
        }

        public async Task WriteToPipelineAsync(PipeWriter writer, byte[] data, CancellationToken cancellationToken = default)
        {
            var memory = writer.GetMemory(data.Length);
            data.CopyTo(memory.Span);
            writer.Advance(data.Length);
            
            var result = await writer.FlushAsync(cancellationToken);
            if (result.IsCompleted)
            {
                throw new InvalidOperationException("管道已完成");
            }
        }
    }

    public class ChannelTransportService : IChannelTransportService
    {
        private readonly ILogger<ChannelTransportService> _logger;
        private readonly int _capacity = int.Parse(Environment.GetEnvironmentVariable("TRANSPORT_CHANNEL_CAPACITY") ?? "-1");

        public ChannelTransportService(ILogger<ChannelTransportService> logger)
        {
            _logger = logger;
        }

        public async Task ProcessChannelAsync(int messageCount, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"处理通道传输，消息数量: {messageCount}");
            
            Channel<string> channel;
            if (_capacity > 0)
            {
                channel = Channel.CreateBounded<string>(_capacity);
                _logger.LogInformation($"创建有界通道，容量: {_capacity}");
            }
            else
            {
                channel = Channel.CreateUnbounded<string>();
                _logger.LogInformation("创建无界通道");
            }
            
            var writerTask = WriteMessagesAsync(channel.Writer, messageCount, cancellationToken);
            var readerTask = ReadMessagesAsync(channel.Reader, messageCount, cancellationToken);
            
            await Task.WhenAll(writerTask, readerTask);
            _logger.LogInformation("通道传输处理完成");
        }

        private async Task WriteMessagesAsync(ChannelWriter<string> writer, int messageCount, CancellationToken cancellationToken)
        {
            try
            {
                for (int i = 0; i < messageCount; i++)
                {
                    await SendToChannelAsync(writer, $"Message {i}", cancellationToken);
                    await Task.Delay(10, cancellationToken);
                }
                await writer.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "写入通道失败");
                await writer.CompleteAsync(ex);
            }
        }

        private async Task ReadMessagesAsync(ChannelReader<string> reader, int messageCount, CancellationToken cancellationToken)
        {
            try
            {
                int count = 0;
                await foreach (var message in reader.ReadAllAsync(cancellationToken))
                {
                    _logger.LogInformation($"读取通道消息: {message}");
                    count++;
                    if (count >= messageCount) break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "读取通道失败");
            }
        }

        public async Task SendToChannelAsync<T>(ChannelWriter<T> writer, T message, CancellationToken cancellationToken = default)
        {
            await writer.WriteAsync(message, cancellationToken);
        }

        public async Task<T> ReceiveFromChannelAsync<T>(ChannelReader<T> reader, CancellationToken cancellationToken = default)
        {
            return await reader.ReadAsync(cancellationToken);
        }
    }

    public class BufferTransportService : IBufferTransportService
    {
        private readonly ILogger<BufferTransportService> _logger;
        private readonly ArrayPool<byte> _bufferPool = ArrayPool<byte>.Shared;

        public BufferTransportService(ILogger<BufferTransportService> logger)
        {
            _logger = logger;
        }

        public async Task ProcessBufferAsync(int bufferSize, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"处理缓冲区操作，大小: {bufferSize}");
            
            byte[] buffer = null;
            try
            {
                buffer = RentBuffer(bufferSize);
                _logger.LogInformation($"租用缓冲区，大小: {buffer.Length}");
                
                // 模拟缓冲区操作
                for (int i = 0; i < bufferSize; i++)
                {
                    buffer[i] = (byte)(i % 256);
                }
                
                await Task.Delay(100, cancellationToken);
                _logger.LogInformation("缓冲区操作完成");
            }
            finally
            {
                if (buffer != null)
                {
                    ReturnBuffer(buffer);
                    _logger.LogInformation("归还缓冲区");
                }
            }
        }

        public byte[] RentBuffer(int size)
        {
            return _bufferPool.Rent(size);
        }

        public void ReturnBuffer(byte[] buffer)
        {
            _bufferPool.Return(buffer);
        }
    }

    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Transport 技能 - 提供各种传输协议和机制的实现");

            var httpCommand = new Command("http", "HTTP传输操作");
            var urlOption = new Option<string>("--url", description: "请求URL");
            var methodOption = new Option<string>("--method", getDefaultValue: () => "GET", description: "HTTP方法");
            var dataOption = new Option<string>("--data", description: "请求数据");
            httpCommand.AddOption(urlOption);
            httpCommand.AddOption(methodOption);
            httpCommand.AddOption(dataOption);
            httpCommand.SetHandler(async (url, method, data, cancellationToken) =>
            {
                using var serviceProvider = CreateServiceProvider();
                var httpService = serviceProvider.GetRequiredService<IHttpTransportService>();
                var result = await httpService.SendRequestAsync(url, method, data, null, cancellationToken);
                Console.WriteLine(result);
            }, urlOption, methodOption, dataOption);

            var tcpCommand = new Command("tcp", "TCP传输操作");
            var hostOption = new Option<string>("--host", getDefaultValue: () => "localhost", description: "主机名");
            var portOption = new Option<int>("--port", getDefaultValue: () => 8080, description: "端口号");
            var messageOption = new Option<string>("--message", description: "消息内容");
            var serverOption = new Option<bool>("--server", getDefaultValue: () => false, description: "启动服务器");
            tcpCommand.AddOption(hostOption);
            tcpCommand.AddOption(portOption);
            tcpCommand.AddOption(messageOption);
            tcpCommand.AddOption(serverOption);
            tcpCommand.SetHandler(async (host, port, message, server, cancellationToken) =>
            {
                using var serviceProvider = CreateServiceProvider();
                var tcpService = serviceProvider.GetRequiredService<ITcpTransportService>();
                
                if (server)
                {
                    await tcpService.StartServerAsync(port, async msg => {
                        return $"服务器响应: {msg}";
                    }, cancellationToken);
                }
                else if (!string.IsNullOrEmpty(message))
                {
                    var response = await tcpService.SendMessageAsync(host, port, message, cancellationToken);
                    Console.WriteLine(response);
                }
                else
                {
                    Console.WriteLine("请指定 --message 参数或 --server 选项");
                }
            }, hostOption, portOption, messageOption, serverOption);

            var udpCommand = new Command("udp", "UDP传输操作");
            var udpHostOption = new Option<string>("--host", getDefaultValue: () => "localhost", description: "主机名");
            var udpPortOption = new Option<int>("--port", getDefaultValue: () => 8081, description: "端口号");
            var udpMessageOption = new Option<string>("--message", description: "消息内容");
            var receiveOption = new Option<bool>("--receive", getDefaultValue: () => false, description: "接收消息");
            udpCommand.AddOption(udpHostOption);
            udpCommand.AddOption(udpPortOption);
            udpCommand.AddOption(udpMessageOption);
            udpCommand.AddOption(receiveOption);
            udpCommand.SetHandler(async (host, port, message, receive, cancellationToken) =>
            {
                using var serviceProvider = CreateServiceProvider();
                var udpService = serviceProvider.GetRequiredService<IUdpTransportService>();
                
                if (receive)
                {
                    var receivedMessage = await udpService.ReceiveMessageAsync(port, cancellationToken);
                    Console.WriteLine(receivedMessage);
                }
                else if (!string.IsNullOrEmpty(message))
                {
                    await udpService.SendMessageAsync(host, port, message, cancellationToken);
                    Console.WriteLine("UDP消息发送完成");
                }
                else
                {
                    Console.WriteLine("请指定 --message 参数或 --receive 选项");
                }
            }, udpHostOption, udpPortOption, udpMessageOption, receiveOption);

            var pipelineCommand = new Command("pipeline", "管道传输操作");
            var pipelineCountOption = new Option<int>("--count", getDefaultValue: () => 10, description: "数据项数量");
            pipelineCommand.AddOption(pipelineCountOption);
            pipelineCommand.SetHandler(async (count, cancellationToken) =>
            {
                using var serviceProvider = CreateServiceProvider();
                var pipelineService = serviceProvider.GetRequiredService<IPipelineTransportService>();
                await pipelineService.ProcessPipelineAsync(count, cancellationToken);
            }, pipelineCountOption);

            var channelCommand = new Command("channel", "通道传输操作");
            var channelCountOption = new Option<int>("--count", getDefaultValue: () => 10, description: "消息数量");
            channelCommand.AddOption(channelCountOption);
            channelCommand.SetHandler(async (count, cancellationToken) =>
            {
                using var serviceProvider = CreateServiceProvider();
                var channelService = serviceProvider.GetRequiredService<IChannelTransportService>();
                await channelService.ProcessChannelAsync(count, cancellationToken);
            }, channelCountOption);

            var bufferCommand = new Command("buffer", "缓冲区操作");
            var bufferSizeOption = new Option<int>("--size", getDefaultValue: () => 1024, description: "缓冲区大小");
            var poolOption = new Option<bool>("--pool", getDefaultValue: () => false, description: "使用缓冲区池");
            bufferCommand.AddOption(bufferSizeOption);
            bufferCommand.AddOption(poolOption);
            bufferCommand.SetHandler(async (size, pool, cancellationToken) =>
            {
                using var serviceProvider = CreateServiceProvider();
                var bufferService = serviceProvider.GetRequiredService<IBufferTransportService>();
                await bufferService.ProcessBufferAsync(size, cancellationToken);
            }, bufferSizeOption, poolOption);

            var scrutorCommand = new Command("scrutor", "Scrutor依赖注入演示");
            var scrutorTypeOption = new Option<string>("--type", getDefaultValue: () => "all", description: "演示类型");
            scrutorCommand.AddOption(scrutorTypeOption);
            scrutorCommand.SetHandler((type) =>
            {
                Console.WriteLine($"Scrutor演示类型: {type}");
                Console.WriteLine("请运行 transport_generator.exe 查看完整的Scrutor演示");
            }, scrutorTypeOption);

            rootCommand.AddCommand(httpCommand);
            rootCommand.AddCommand(tcpCommand);
            rootCommand.AddCommand(udpCommand);
            rootCommand.AddCommand(pipelineCommand);
            rootCommand.AddCommand(channelCommand);
            rootCommand.AddCommand(bufferCommand);
            rootCommand.AddCommand(scrutorCommand);

            var parser = new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .Build();

            return await parser.InvokeAsync(args);
        }

        private static ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            
            services.AddLogging(logging => {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });

            services.AddTransient<IHttpTransportService, HttpTransportService>();
            services.AddTransient<ITcpTransportService, TcpTransportService>();
            services.AddTransient<IUdpTransportService, UdpTransportService>();
            services.AddTransient<IPipelineTransportService, PipelineTransportService>();
            services.AddTransient<IChannelTransportService, ChannelTransportService>();
            services.AddTransient<IBufferTransportService, BufferTransportService>();

            return services.BuildServiceProvider();
        }
    }
}
