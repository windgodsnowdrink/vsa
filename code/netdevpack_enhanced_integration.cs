//#:sdk Microsoft.NET.Sdk
//#:package NetDevPack@5.2.0
//#:property LangVersion preview
//#:property TargetFramework net11.0
//#:property Nullable enable
//#:property ImplicitUsings enable

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetDevPack;
using NetDevPack.Domain;
using NetDevPack.Mediator;
using NetDevPack.Security;
using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NetDevPackIntegration
{
    public class NetDevPackEnhancedOptions
    {
        public string EncryptionKey { get; set; }
        public int CacheDurationMinutes { get; set; } = 30;
        public bool EnableDomainEvents { get; set; } = true;
        public int EventChannelCapacity { get; set; } = 1000;
        public bool EnableZeroCopyEncryption { get; set; } = true;
    }

    public interface INetDevPackEnhancedService
    {
        Task<string> EncryptDataAsync(string data);
        Task<string> DecryptDataAsync(string encryptedData);
        Task PublishDomainEventAsync<TEvent>(TEvent @event) where TEvent : Event;
        ValueTask<bool> TryPublishDomainEventAsync<TEvent>(TEvent @event) where TEvent : Event;
    }

    public class NetDevPackEnhancedService : INetDevPackEnhancedService
    {
        private readonly NetDevPackEnhancedOptions _options;
        private readonly IMediatorHandler _mediator;
        private readonly IEncryptionService _encryptionService;
        private readonly Channel<Event> _eventChannel;

        public NetDevPackEnhancedService(
            IOptions<NetDevPackEnhancedOptions> options,
            IMediatorHandler mediator,
            IEncryptionService encryptionService)
        {
            _options = options.Value;
            _mediator = mediator;
            _encryptionService = encryptionService;
            _eventChannel = Channel.CreateBounded<Event>(
                new BoundedChannelOptions(_options.EventChannelCapacity)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = false,
                    SingleWriter = false
                });

            _ = Task.Run(ProcessEventsAsync);
        }

        public async Task<string> EncryptDataAsync(string data)
        {
            if (_options.EnableZeroCopyEncryption)
            {
                using var memory = System.Buffers.MemoryPool<byte>.Shared.Rent(data.Length * 2);
                var span = memory.Memory.Span;
                System.Text.Encoding.UTF8.GetBytes(data, span);
                return await _encryptionService.Encrypt(span, _options.EncryptionKey);
            }
            return await _encryptionService.Encrypt(data, _options.EncryptionKey);
        }

        public async Task<string> DecryptDataAsync(string encryptedData)
        {
            return await _encryptionService.Decrypt(encryptedData, _options.EncryptionKey);
        }

        public async Task PublishDomainEventAsync<TEvent>(TEvent @event) where TEvent : Event
        {
            if (_options.EnableDomainEvents)
            {
                await _eventChannel.Writer.WriteAsync(@event);
            }
        }

        public async ValueTask<bool> TryPublishDomainEventAsync<TEvent>(TEvent @event) where TEvent : Event
        {
            if (_options.EnableDomainEvents)
            {
                return await _eventChannel.Writer.TryWriteAsync(@event);
            }
            return false;
        }

        private async Task ProcessEventsAsync()
        {
            await foreach (var @event in _eventChannel.Reader.ReadAllAsync())
            {
                try
                {
                    await _mediator.PublishEvent(@event);
                }
                catch (Exception ex)
                {
                    // 处理事件发布异常
                }
            }
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNetDevPackEnhancedServices(
            this IServiceCollection services,
            Action<NetDevPackEnhancedOptions> configureOptions)
        {
            services.Configure(configureOptions);
            
            // 注册NetDevPack核心服务
            services.AddNetDevPack();
            
            // 注册增强版服务
            services.AddSingleton<INetDevPackEnhancedService, NetDevPackEnhancedService>();
            
            return services;
        }
    }

    public static class ExampleEnhancedUsage
    {
        public static async Task Demo()
        {
            var services = new ServiceCollection();
            services.AddNetDevPackEnhancedServices(options =>
            {
                options.EncryptionKey = "your-secure-encryption-key";
                options.CacheDurationMinutes = 60;
                options.EnableDomainEvents = true;
                options.EventChannelCapacity = 5000;
                options.EnableZeroCopyEncryption = true;
            });

            var provider = services.BuildServiceProvider();
            var enhancedService = provider.GetRequiredService<INetDevPackEnhancedService>();

            // 高性能加密示例
            var encrypted = await enhancedService.EncryptDataAsync("Sensitive Data");
            var decrypted = await enhancedService.DecryptDataAsync(encrypted);

            // 高性能领域事件发布
            await enhancedService.PublishDomainEventAsync(new UserRegisteredEvent("user@example.com"));
            
            // 非阻塞事件发布
            var success = await enhancedService.TryPublishDomainEventAsync(
                new UserRegisteredEvent("another@example.com"));
        }
    }
}