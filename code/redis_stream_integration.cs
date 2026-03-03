using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using StackExchange.Redis;

namespace RedisStreamIntegration
{
    public class RedisStreamOptions
    {
        public string ConnectionString { get; set; } = "localhost";
        public string ConsumerGroupName { get; set; } = "default-group";
        public int StreamBufferSize { get; set; } = 1000;
        public bool AutoClaimMessages { get; set; } = true;
        public TimeSpan ClaimMessagesInterval { get; set; } = TimeSpan.FromSeconds(30);
        public int RetryCount { get; set; } = 3;
    }

    public interface IRedisStreamService
    {
        Task PublishAsync(string streamName, NameValueEntry[] entries);
        Task SubscribeAsync(string streamName, Func<StreamEntry, Task> handler);
        Task CreateConsumerGroupAsync(string streamName);
    }

    public class RedisStreamService : IRedisStreamService, IDisposable
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly RedisStreamOptions _options;
        private readonly Meter _meter;
        private readonly Counter<int> _messagesPublished;
        private readonly Counter<int> _messagesProcessed;
        private readonly ConcurrentDictionary<string, Channel<StreamEntry>> _streamChannels = new();
        private readonly ConcurrentDictionary<string, Task> _processingTasks = new();
        private readonly IAsyncPolicy _retryPolicy;

        public RedisStreamService(IOptions<RedisStreamOptions> options)
        {
            _options = options.Value;
            _redis = ConnectionMultiplexer.Connect(_options.ConnectionString);
            
            _meter = new Meter("RedisStreamService");
            _messagesPublished = _meter.CreateCounter<int>("messages_published");
            _messagesProcessed = _meter.CreateCounter<int>("messages_processed");
            
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(_options.RetryCount, retryAttempt => 
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public async Task PublishAsync(string streamName, NameValueEntry[] entries)
        {
            var db = _redis.GetDatabase();
            var messageId = await db.StreamAddAsync(streamName, entries);
            _messagesPublished.Add(1);
        }

        public async Task SubscribeAsync(string streamName, Func<StreamEntry, Task> handler)
        {
            if (!_streamChannels.TryGetValue(streamName, out var channel))
            {
                channel = Channel.CreateBounded<StreamEntry>(_options.StreamBufferSize);
                _streamChannels[streamName] = channel;
                
                var processingTask = Task.Run(async () =>
                {
                    await foreach (var entry in channel.Reader.ReadAllAsync())
                    {
                        await _retryPolicy.ExecuteAsync(async () =>
                        {
                            await handler(entry);
                            _messagesProcessed.Add(1);
                        });
                    }
                });
                
                _processingTasks[streamName] = processingTask;
            }

            var db = _redis.GetDatabase();
            await EnsureConsumerGroupExists(streamName, db);
            
            _ = Task.Run(async () =>
            {
                var consumerName = $"{Environment.MachineName}:{Guid.NewGuid()}";
                var lastId = "0-0";
                
                while (true)
                {
                    try
                    {
                        var entries = await db.StreamReadGroupAsync(
                            streamName, _options.ConsumerGroupName, consumerName, ">", count: 10);
                        
                        if (entries.Length > 0)
                        {
                            foreach (var entry in entries)
                            {
                                await channel.Writer.WriteAsync(entry);
                                lastId = entry.Id;
                            }
                        }
                        else if (_options.AutoClaimMessages)
                        {
                            await ClaimPendingMessages(streamName, db, consumerName);
                        }
                        
                        await Task.Delay(100);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error processing stream: {ex.Message}");
                        await Task.Delay(1000);
                    }
                }
            });
        }

        public async Task CreateConsumerGroupAsync(string streamName)
        {
            var db = _redis.GetDatabase();
            await EnsureConsumerGroupExists(streamName, db);
        }

        private async Task EnsureConsumerGroupExists(string streamName, IDatabase db)
        {
            try
            {
                await db.StreamCreateConsumerGroupAsync(streamName, _options.ConsumerGroupName, "0-0");
            }
            catch (RedisServerException ex) when (ex.Message.Contains("BUSYGROUP"))
            {
                // Group already exists
            }
        }

        private async Task ClaimPendingMessages(string streamName, IDatabase db, string consumerName)
        {
            var pendingMessages = await db.StreamPendingMessagesAsync(
                streamName, _options.ConsumerGroupName, 10, consumerName);
                
            if (pendingMessages.Length > 0)
            {
                var claimedMessages = await db.StreamClaimAsync(
                    streamName, _options.ConsumerGroupName, consumerName, 
                    _options.ClaimMessagesInterval, pendingMessages.Select(x => x.MessageId).ToArray());
                    
                foreach (var entry in claimedMessages)
                {
                    await _streamChannels[streamName].Writer.WriteAsync(entry);
                }
            }
        }

        public void Dispose()
        {
            foreach (var channel in _streamChannels.Values)
            {
                channel.Writer.Complete();
            }
            
            Task.WaitAll(_processingTasks.Values.ToArray());
            _meter.Dispose();
            _redis.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public static class RedisStreamExtensions
    {
        public static IServiceCollection AddRedisStreamService(this IServiceCollection services, Action<RedisStreamOptions> configure)
        {
            services.Configure(configure);
            services.AddSingleton<IRedisStreamService, RedisStreamService>();
            return services;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddRedisStreamService(options =>
            {
                options.ConnectionString = "localhost";
                options.ConsumerGroupName = "orders-group";
                options.StreamBufferSize = 5000;
            });
            
            var app = builder.Build();
            
            app.MapGet("/", () => "Redis Stream Service");
            
            var streamService = app.Services.GetRequiredService<IRedisStreamService>();
            streamService.CreateConsumerGroupAsync("orders").Wait();
            streamService.SubscribeAsync("orders", async entry =>
            {
                Console.WriteLine($"Processing order: {entry.Id}");
                await Task.Delay(100);
            }).Wait();
            
            app.Run();
        }
    }
}