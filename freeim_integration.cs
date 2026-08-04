#:sdk Microsoft.NET.Sdk.Web
#:package FreeIM@2.5.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package Polly@8.0.0
#:package OpenTelemetry.Extensions.Hosting@1.8.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using FreeIM;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.Metrics;

public class FreeIMOptions
{
    public string RedisConnectionString { get; set; } = string.Empty;
    public bool MessagePersistenceEnabled { get; set; } = true;
    public bool OfflineMessageSyncEnabled { get; set; } = true;
    public bool ReadReceiptEnabled { get; set; } = true;
    public bool MessageRecallEnabled { get; set; } = true;
    public bool MessageSearchEnabled { get; set; } = true;
    public bool MessageEncryptionEnabled { get; set; } = true;
    public string EncryptionKey { get; set; } = string.Empty;"localhost:6379";
    public bool MultiTenantEnabled { get; set; } = false;
    public string TenantHeaderName { get; set; } = "X-Tenant-Id";
    public int RetryCount { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    public double CircuitBreakerFailureThreshold { get; set; } = 0.5;
    public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromSeconds(30);
    public int BufferSize { get; set; } = 8192;
    public bool ZeroCopyEnabled { get; set; } = true;
    public int MemoryPoolSize { get; set; } = 1024 * 1024 * 100;
    public bool MetricsEnabled { get; set; } = true;
    public bool TracingEnabled { get; set; } = true;
    public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromMinutes(1);
}

public interface IFreeIMService
{
    Task JoinRoomAsync(string roomId, string userId);
    Task LeaveRoomAsync(string roomId, string userId);
    Task SendMessageAsync(string roomId, string userId, string message);
    Task<MessageReceipt> SendMessageWithReceiptAsync(string roomId, string userId, string message);
    Task RecallMessageAsync(string messageId);
    Task<IEnumerable<ChatMessage>> SearchMessagesAsync(string roomId, string keyword);
    Task<IEnumerable<ChatMessage>> GetOfflineMessagesAsync(string userId);
    Task MarkMessageAsReadAsync(string messageId, string userId);
    Task SetTenantContextAsync(string tenantId);
    Task ResetCircuitBreakerAsync();
    Task<MemoryPoolStatistics> GetMemoryPoolAsync();
    Task<ConnectionStatistics> GetConnectionStatisticsAsync();
    Task<ThroughputStatistics> GetThroughputStatisticsAsync();
    Task<HealthCheckResult> CheckHealthAsync();
}

public class FreeIMService : IFreeIMService
{
    private readonly ILogger<FreeIMService> _logger;
    private readonly FreeIMOptions _options;
    private readonly IAsyncPolicy _resiliencyPolicy;
    private readonly IMessageStore _messageStore;
    private readonly IEncryptionService _encryptionService;
    private readonly ISearchService _searchService;
    
    public FreeIMService(
    ILogger<FreeIMService> logger, 
    IOptions<FreeIMOptions> options,
    IMessageStore messageStore,
    IEncryptionService encryptionService,
    ISearchService searchService)
{
    _logger = logger;
    _options = options.Value;
    _messageStore = messageStore;
    _encryptionService = encryptionService;
    _searchService = searchService;
    _resiliencyPolicy = Policy.Handle<Exception>()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}
    
    public async Task JoinRoomAsync(string roomId, string userId)
    {
        using var activity = _activitySource.StartActivity("JoinRoom");
        await _retryPolicy.ExecuteAsync(async () => 
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _client.JoinRoomAsync(roomId, userId)));
    }
    
    public async Task LeaveRoomAsync(string roomId, string userId)
    {
        using var activity = _activitySource.StartActivity("LeaveRoom");
        await _retryPolicy.ExecuteAsync(async () => 
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _client.LeaveRoomAsync(roomId, userId)));
    }
    
    public async Task SendMessageAsync(string roomId, string userId, string message)
    {
        if (_options.MessageEncryptionEnabled)
        {
            message = _encryptionService.Encrypt(message, _options.EncryptionKey);
        }

        if (_options.MessagePersistenceEnabled)
        {
            await _messageStore.SaveMessageAsync(new ChatMessage
            {
                RoomId = roomId,
                UserId = userId,
                Content = message,
                Timestamp = DateTime.UtcNow
            });
        }

        await _client.SendMessageAsync(roomId, userId, message);
    }

    public async Task<MessageReceipt> SendMessageWithReceiptAsync(string roomId, string userId, string message)
    {
        var msg = await SendMessageAsync(roomId, userId, message);
        return new MessageReceipt
        {
            MessageId = msg.Id,
            Status = MessageStatus.Sent,
            Timestamp = DateTime.UtcNow
        };
    }

    public async Task RecallMessageAsync(string messageId)
    {
        await _messageStore.MarkMessageAsRecalledAsync(messageId);
        await _client.RecallMessageAsync(messageId);
    }

    public async Task<IEnumerable<ChatMessage>> SearchMessagesAsync(string roomId, string keyword)
    {
        if (!_options.MessageSearchEnabled)
            throw new InvalidOperationException("Message search is disabled");

        return await _searchService.SearchMessagesAsync(roomId, keyword);
    }

    public async Task<IEnumerable<ChatMessage>> GetOfflineMessagesAsync(string userId)
    {
        if (!_options.OfflineMessageSyncEnabled)
            return Enumerable.Empty<ChatMessage>();

        return await _messageStore.GetUnreadMessagesAsync(userId);
    }

    public async Task MarkMessageAsReadAsync(string messageId, string userId)
    {
        await _messageStore.MarkMessageAsReadAsync(messageId, userId);
    }
    
    public Task SetTenantContextAsync(string tenantId) => Task.CompletedTask;
    public Task ResetCircuitBreakerAsync() => Task.FromResult(_circuitBreakerPolicy.Reset());
    public Task<MemoryPoolStatistics> GetMemoryPoolAsync() => Task.FromResult(new MemoryPoolStatistics());
    public Task<ConnectionStatistics> GetConnectionStatisticsAsync() => Task.FromResult(new ConnectionStatistics());
    public Task<ThroughputStatistics> GetThroughputStatisticsAsync() => Task.FromResult(new ThroughputStatistics());
    public Task<HealthCheckResult> CheckHealthAsync() => Task.FromResult(HealthCheckResult.Healthy());
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFreeIMService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FreeIMOptions>(configuration.GetSection("FreeIM"));
        
        // Core services
        services.AddSingleton<IFreeIMService, FreeIMService>();
        services.AddSingleton<FreeIMClient>();
        
        // Redis cache
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["FreeIM:RedisConnectionString"];
        });
        
        // Memory pool
        services.AddSingleton<ObjectPool<Memory<byte>>>(_ => 
            new DefaultObjectPool<Memory<byte>>(new MemoryPooledPolicy(), 100));
        
        // Message persistence
        services.AddSingleton<IMessageStore, RedisMessageStore>();
        
        // Encryption service
        services.AddSingleton<IEncryptionService, AesEncryptionService>();
        
        // Search service
        services.AddSingleton<ISearchService, ElasticSearchService>();
        
        // Health checks
        services.AddHealthChecks()
            .AddCheck<FreeIMHealthCheck>("freeim");
        
        return services;
    }
}

public class FreeIMHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}

public class TenantContext { }
public record MemoryPoolStatistics;
public record ConnectionStatistics;
public record ThroughputStatistics;