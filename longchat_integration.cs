#:package Microsoft.Extensions.Hosting@8.0.0
#:package Polly@8.0.0
#:package System.Threading.Channels@7.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Channels;
using Polly;

namespace LongChatIntegration
{
    public class LongChatOptions
    {
        public string ServerUrl { get; set; } = "https://api.longchat.ai/v1";
        public string ApiKey { get; set; } = string.Empty;
        public int MaxConcurrentConnections { get; set; } = 10;
        public int MessageBufferSize { get; set; } = 1024;
        public int RetryCount { get; set; } = 3;
        public bool EnableCompression { get; set; } = true;
        public bool EnableZeroCopy { get; set; } = true;
    }

    public interface ILongChatService
    {
        Task SendMessageAsync(string message);
        Task<string> ReceiveMessageAsync();
        Task ProcessMessageStreamAsync(CancellationToken cancellationToken);
    }

    public class LongChatService : ILongChatService, IAsyncDisposable
    {
        private readonly Channel<string> _messageChannel;
        private readonly LongChatOptions _options;
        private readonly ILogger<LongChatService> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly MemoryPool<byte> _memoryPool;
        private readonly ThreadLocal<Memory<byte>> _threadLocalBuffer;

        public LongChatService(
            IOptions<LongChatOptions> options,
            ILogger<LongChatService> logger)
        {
            _options = options.Value;
            _logger = logger;
            
            _messageChannel = Channel.CreateBounded<string>(
                new BoundedChannelOptions(_options.MessageBufferSize)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = false,
                    SingleWriter = false
                });
                
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    _options.RetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time, retryCount, context) => 
                    {
                        _logger.LogWarning(ex, $"Message send failed. Retry attempt {retryCount} in {time.TotalSeconds} seconds.");
                    });
                    
            _memoryPool = MemoryPool<byte>.Shared;
            _threadLocalBuffer = new ThreadLocal<Memory<byte>>(() => 
                _memoryPool.Rent(4096).Memory);
        }

        public async Task SendMessageAsync(string message)
        {
            await _messageChannel.Writer.WriteAsync(message);
        }

        public async Task<string> ReceiveMessageAsync()
        {
            return await _messageChannel.Reader.ReadAsync();
        }

        public async Task ProcessMessageStreamAsync(CancellationToken cancellationToken)
        {
            await foreach (var message in _messageChannel.Reader.ReadAllAsync(cancellationToken))
            {
                try
                {
                    await ProcessMessageInternalAsync(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process message: {Message}", message);
                }
            }
        }

        private async Task ProcessMessageInternalAsync(string message)
        {
            if (_options.EnableZeroCopy)
            {
                var buffer = _threadLocalBuffer.Value;
                Encoding.UTF8.GetBytes(message, buffer.Span);
                await ProcessWithZeroCopy(buffer.Span);
            }
            else
            {
                await _retryPolicy.ExecuteAsync(async () => 
                {
                    // 实际发送消息到LongChat服务器的逻辑
                    await Task.Delay(100); // 模拟网络请求
                });
            }
        }

        private async Task ProcessWithZeroCopy(ReadOnlySpan<byte> messageSpan)
        {
            try
            {
                var message = Encoding.UTF8.GetString(messageSpan);
                await _retryPolicy.ExecuteAsync(async () => 
                {
                    // 实际发送消息到LongChat服务器的逻辑
                    await Task.Delay(100); // 模拟网络请求
                });
            }
            finally
            {
                messageSpan.Clear();
            }
        }

        public async ValueTask DisposeAsync()
        {
            _messageChannel.Writer.Complete();
            _memoryPool.Dispose();
        }
    }

    public static class LongChatExtensions
    {
        public static IServiceCollection AddLongChatService(
            this IServiceCollection services,
            Action<LongChatOptions> configure)
        {
            services.AddOptions<LongChatOptions>()
                .Configure(configure)
                .Validate(options => 
                {
                    if (string.IsNullOrEmpty(options.ApiKey))
                    {
                        return false;
                    }
                    return true;
                });

            services.AddSingleton<ILongChatService, LongChatService>();
            services.AddLogging();
            
            return services;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddLongChatService(options =>
                    {
                        options.ApiKey = "your-api-key";
                        options.ServerUrl = "https://api.longchat.ai/v1";
                        options.MaxConcurrentConnections = 8;
                    });
                })
                .Build();

            host.Run();
        }
    }
}