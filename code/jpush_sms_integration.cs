#:sdk Microsoft.NET.Sdk
#:package JPush.SMS@1.0.0
#:package System.Threading.Channels@7.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable

using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using JPush.SMS;

public record SmsMessage(string PhoneNumber, string Content, string SignName, string TemplateCode);

public class RateLimiter
{
    private readonly SemaphoreSlim _semaphore;
    private readonly TimeSpan _timeWindow;
    private readonly Timer _timer;
    
    public RateLimiter(int maxRequests, TimeSpan timeWindow)
    {
        _semaphore = new SemaphoreSlim(maxRequests, maxRequests);
        _timeWindow = timeWindow;
        _timer = new Timer(ReleaseTokens, null, timeWindow, timeWindow);
    }
    
    private void ReleaseTokens(object state)
    {
        try
        {
            var currentCount = _semaphore.CurrentCount;
            var releaseCount = _semaphore.MaxCount - currentCount;
            if (releaseCount > 0)
                _semaphore.Release(releaseCount);
        }
        catch { /* 忽略异常 */ }
    }
    
    public async Task WaitAsync(CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
    }
    
    public void Dispose()
    {
        _timer?.Dispose();
        _semaphore?.Dispose();
    }
}

// 在JpushSmsOptions中添加限流配置
public class JpushSmsOptions
{
    public string AppKey { get; set; } = string.Empty;
    public string MasterSecret { get; set; } = string.Empty;
    public int MaxRetryCount { get; set; } = 3;
    public int ChannelCapacity { get; set; } = 1000;
    public int RateLimitPerMinute { get; set; } = 100; // 默认每分钟100条
}

public interface IJpushSmsService
{
    Task<bool> SendAsync(SmsMessage message);
    ValueTask<bool> EnqueueAsync(SmsMessage message);
}

public class JpushSmsService : IJpushSmsService, IDisposable
{
    private readonly Channel<SmsMessage> _channel;
    private readonly ISmsClient _smsClient;
    private readonly JpushSmsOptions _options;
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _processingTask;

    public JpushSmsService(ISmsClient smsClient, IOptions<JpushSmsOptions> options)
    {
        _smsClient = smsClient;
        _options = options.Value;
        _channel = Channel.CreateBounded<SmsMessage>(_options.ChannelCapacity);
        _processingTask = ProcessMessagesAsync(_cts.Token);
        _rateLimiter = new RateLimiter(
            options.Value.RateLimitPerMinute, 
            TimeSpan.FromMinutes(1));
    }

    private async Task ProcessMessagesAsync(CancellationToken cancellationToken)
    {
        await foreach (var message in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            await SendWithRetryAsync(message, cancellationToken);
        }
    }

    private async Task SendWithRetryAsync(SmsMessage message, CancellationToken cancellationToken)
    {
        int attempt = 0;
        while (attempt < _options.MaxRetryCount)
        {
            try
            {
                var response = await _smsClient.SendAsync(
                    message.PhoneNumber,
                    message.Content,
                    message.SignName,
                    message.TemplateCode,
                    cancellationToken);
                    
                if (response.IsSuccess)
                    return;
                    
                await Task.Delay(1000 * (attempt + 1), cancellationToken);
                attempt++;
            }
            catch (Exception ex) when (attempt < _options.MaxRetryCount - 1)
            {
                await Task.Delay(1000 * (attempt + 1), cancellationToken);
                attempt++;
            }
        }
        throw new InvalidOperationException($"短信发送失败，重试{_options.MaxRetryCount}次后仍失败");
    }

    public async Task<bool> SendAsync(SmsMessage message)
  {
        try
        {
            var response = await _smsClient.SendAsync(
                message.PhoneNumber,
                message.Content,
                message.SignName,
                message.TemplateCode,
                CancellationToken.None);
                
            return response.IsSuccess;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    // 参考dorisoy_sms_integration.cs添加的功能
    public async Task<bool> CheckBlacklistAsync(string phoneNumber)
    {
        return _blacklist.Contains(phoneNumber);
    }

    public async Task<bool> CheckWhitelistAsync(string phoneNumber)
    {
        return _whitelist.Count == 0 || _whitelist.Contains(phoneNumber);
    }

    public async Task<string> FilterSensitiveWordsAsync(string message)
    {
        if (_options.SensitiveWords == null || !_options.SensitiveWords.ContainsSensitiveWord(message.AsSpan()))
            return message;
            
        var filtered = new StringBuilder(message);
        // ... 敏感词过滤实现 ...
        return filtered.ToString();
    }

    public ValueTask<bool> EnqueueAsync(SmsMessage message)
    {
        return _channel.Writer.WriteAsync(message, _cts.Token);
    }

    public void Dispose()
    {
        _cts.Cancel();
        _processingTask.Wait();
        _cts.Dispose();
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJpushSmsServices(this IServiceCollection services, Action<JpushSmsOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<ISmsClient>(sp => 
        {
            var options = sp.GetRequiredService<IOptions<JpushSmsOptions>>().Value;
            return new SmsClient(options.AppKey, options.MasterSecret);
        });
        services.AddSingleton<IJpushSmsService, JpushSmsService>();
        return services;
    }
}