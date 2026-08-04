#:sdk Microsoft.NET.Sdk.Web
#:package Dorisoy.SMS@1.0.0
#:package Microsoft.Extensions.Hosting@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable

using Dorisoy.SMS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

public class SmsOptions
{
    public HashSet<string> Blacklist { get; set; } = new();
    public HashSet<string> Whitelist { get; set; } = new();
    public TrieTree SensitiveWords { get; set; } = new();
    public int RateLimitPerMinute { get; set; } = 60;
}

// 高效Trie树实现敏感词过滤
// 实现Trie树敏感词管理
public class TrieTree
{
    private readonly TrieNode _root = new();
    
    public void AddWord(ReadOnlySpan<char> word)
    {
        var node = _root;
        foreach (var c in word)
        {
            if (!node.Children.TryGetValue(c, out var child))
            {
                child = new TrieNode();
                node.Children[c] = child;
            }
            node = child;
        }
        node.IsEnd = true;
    }
    
    public bool ContainsSensitiveWord(ReadOnlySpan<char> text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            var node = _root;
            for (int j = i; j < text.Length; j++)
            {
                if (!node.Children.TryGetValue(text[j], out node))
                    break;
                    
                if (node.IsEnd)
                    return true;
            }
        }
        return false;
    }
    
    private class TrieNode
    {
        public bool IsEnd { get; set; }
        public Dictionary<char, TrieNode> Children { get; } = new();
    }
}

// 实现敏感词替换逻辑
private string FilterSensitiveWords(string message)
{
    var result = new StringBuilder(message);
    var span = message.AsSpan();
    
    for (int i = 0; i < span.Length; i++)
    {
        var node = _options.Value.SensitiveWords._root;
        int end = i;
        
        for (int j = i; j < span.Length; j++)
        {
            if (!node.Children.TryGetValue(span[j], out node))
                break;
                
            if (node.IsEnd)
                end = j + 1;
        }
        
        if (end > i)
        {
            for (int k = i; k < end; k++)
                result[k] = '*';
            i = end - 1;
        }
    }
    
    return result.ToString();
}

// 黑白名单持久化实现
public class SmsListRepository
{
    private readonly IZoneTree<string, HashSet<string>> _zoneTree;
    
    public SmsListRepository()
    {
        _zoneTree = new ZoneTreeFactory<string, HashSet<string>>()
            .SetKeySerializer(new Utf8StringSerializer())
            .SetValueSerializer(new MessagePackHashSetSerializer<string>())
            .OpenOrCreate();
    }
    
    public async Task SaveListAsync(string listType, HashSet<string> items)
    {
        await _zoneTree.UpsertAsync(listType, items);
        await _zoneTree.SaveMetaDataAsync();
    }
    
    public async Task<HashSet<string>> LoadListAsync(string listType)
    {
        return await _zoneTree.GetAsync(listType) ?? new HashSet<string>();
    }
}

// 在SmsService中集成
public class SmsService
{
    private readonly SmsListRepository _listRepository;
    
    public async Task ReloadListsAsync()
    {
        _options.Value.Blacklist = await _listRepository.LoadListAsync("blacklist");
        _options.Value.Whitelist = await _listRepository.LoadListAsync("whitelist");
    }
    
    public async Task UpdateBlacklistAsync(HashSet<string> phoneNumbers)
    {
        await _listRepository.SaveListAsync("blacklist", phoneNumbers);
        _options.Value.Blacklist = phoneNumbers;
    }
    
    private bool CheckBlacklist(string phoneNumber) 
        => _options.Value.Blacklist.Contains(phoneNumber);
        
    private bool CheckWhitelist(string phoneNumber)
        => _options.Value.Whitelist.Count == 0 || 
           _options.Value.Whitelist.Contains(phoneNumber);
           
    private string FilterSensitiveWords(string message)
    {
        // 使用Span优化内存
        var span = message.AsSpan();
        // 实现敏感词替换逻辑
        // ...
        return message;
    }
    
    public async Task SendAsync(SmsMessage message)
    {
        if (CheckBlacklist(message.PhoneNumber)) 
            throw new InvalidOperationException("号码在黑名单中");
            
        if (!CheckWhitelist(message.PhoneNumber))
            throw new InvalidOperationException("号码不在白名单中");
            
        message.Message = FilterSensitiveWords(message.Message);
        // ...原有发送逻辑
    }
}
{
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }
    public string DefaultSender { get; set; }
    public int RetryCount { get; set; } = 3;
}

public interface ISmsService
{
    Task SendSmsAsync(string phoneNumber, string message, string sender = null, CancellationToken ct = default);
    Task SendBatchSmsAsync(string[] phoneNumbers, string message, string sender = null, CancellationToken ct = default);
}

public class SmsService : ISmsService, IHostedService, IDisposable
{
    private readonly SmsOptions _options;
    private readonly Channel<SmsMessage> _channel;
    private readonly CancellationTokenSource _cts = new();
    private Task _processingTask;

    private readonly RateLimiter _rateLimiter;
    
    public SmsService(SmsOptions options)
    {
        _options = options;
        _channel = Channel.CreateBounded<SmsMessage>(new BoundedChannelOptions(10000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        });
        _rateLimiter = new RateLimiter(options.RateLimitPerMinute);
    }

    public async Task SendSmsAsync(string phoneNumber, string message, string sender = null, CancellationToken ct = default)
    {
        await _channel.Writer.WriteAsync(new SmsMessage
        {
            PhoneNumber = phoneNumber,
            Message = message,
            Sender = sender ?? _options.DefaultSender,
            RetryCount = 0
        }, ct);
    }

    public async Task SendBatchSmsAsync(string[] phoneNumbers, string message, string sender = null, CancellationToken ct = default)
    {
        foreach (var phoneNumber in phoneNumbers)
        {
            await SendSmsAsync(phoneNumber, message, sender, ct);
        }
    }

    private async Task ProcessQueueAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var message = await _channel.Reader.ReadAsync(stoppingToken);
                await ProcessMessageAsync(message, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error processing SMS: {ex.Message}");
            }
        }
    }

    private async Task ProcessMessageAsync(SmsMessage message, CancellationToken ct)
    {
        if (!_rateLimiter.TryAcquire())
        {
            // 延迟重试
            await Task.Delay(1000, ct);
            await _channel.Writer.WriteAsync(message, ct);
            return;
        }
        try
        {
            using var smsClient = new SmsClient(_options.ApiKey, _options.ApiSecret);
            await smsClient.SendSmsAsync(message.PhoneNumber, message.Message, message.Sender, ct);
        }
        catch (Exception) when (message.RetryCount < _options.RetryCount)
        {
            message.RetryCount++;
            await _channel.Writer.WriteAsync(message, ct);
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _processingTask = Task.Run(() => ProcessQueueAsync(_cts.Token), cancellationToken);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        await _processingTask;
    }

    public void Dispose()
    {
        _cts?.Dispose();
    }
}

[MemoryPackable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public partial struct SmsMessage
{
    [MemoryPackOrder(0)]
    public string PhoneNumber { get; set; }
    
    [MemoryPackOrder(1)]
    public string Message { get; set; }
    
    [MemoryPackOrder(2)]
    public string Sender { get; set; }
    
    [MemoryPackOrder(3)]
    public int RetryCount { get; set; }
}

// DI扩展方法需要添加MemoryPack支持
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmsServices(this IServiceCollection services, Action<SmsOptions> configure)
    {
        var options = new SmsOptions();
        configure(options);
        
        services.AddSingleton(options);
        services.AddSingleton<ISmsService, SmsService>();
        services.AddHostedService(provider => provider.GetRequiredService<ISmsService>() as SmsService);
        
        services.AddMemoryPack(); // 添加MemoryPack支持
        return services;
    }
}

// 在Startup.cs或Program.cs中配置
builder.Services.AddSmsServices(options => 
{
    options.ApiKey = "your_api_key";
    options.ApiSecret = "your_api_secret";
    options.DefaultSender = "DefaultSender";
    options.RetryCount = 5;
    options.RateLimitPerMinute = 100; // 每分钟100条
});

// 在控制器或服务中注入使用
public class MyService
{
    private readonly ISmsService _smsService;
    
    public MyService(ISmsService smsService)
    {
        _smsService = smsService;
    }
    
    public async Task SendNotification(string phoneNumber)
    {
        await _smsService.SendSmsAsync(phoneNumber, "您的验证码是123456");
    }
}

public class RateLimiter
{
    private readonly int _tokensPerInterval;
    private readonly TimeSpan _interval;
    private int _tokens;
    private DateTime _lastRefillTime;
    private readonly object _lock = new();

    public RateLimiter(int tokensPerMinute)
    {
        _tokensPerInterval = tokensPerMinute;
        _interval = TimeSpan.FromMinutes(1);
        _tokens = tokensPerMinute;
        _lastRefillTime = DateTime.UtcNow;
    }

    public bool TryAcquire()
    {
        lock (_lock)
        {
            Refill();
            if (_tokens <= 0) return false;
            _tokens--;
            return true;
        }
    }

    private void Refill()
    {
        var now = DateTime.UtcNow;
        var timePassed = now - _lastRefillTime;
        var intervalsPassed = (int)(timePassed.TotalMilliseconds / _interval.TotalMilliseconds);
        
        if (intervalsPassed > 0)
        {
            _tokens = Math.Min(_tokensPerInterval, _tokens + intervalsPassed * _tokensPerInterval);
            _lastRefillTime = now;
        }
    }
}