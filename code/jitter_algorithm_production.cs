#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.Options@8.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

// 抖动算法配置选项
public class JitterOptions
{
    public int BaseDelayMs { get; set; } = 100;
    public int MaxDelayMs { get; set; } = 10000;
    public double Exponent { get; set; } = 2.0;
    public double JitterFactor { get; set; } = 0.5;
    public bool UseAdaptiveJitter { get; set; } = true;
    public bool UseSmoothJitter { get; set; } = true;
    public double MinJitterFactor { get; set; } = 0.1;
    public double MaxJitterFactor { get; set; } = 0.9;
}

// 抖动算法服务
public class JitterAlgorithm
{
    private readonly JitterOptions _options;
    private readonly Random _random = new();
    private int _currentAttempt;
    private double _smoothJitterFactor = 0.5;
    
    public JitterAlgorithm(IOptions<JitterOptions> options)
    {
        _options = options.Value;
    }

    // 指数退避+随机抖动
    public TimeSpan GetJitterDelay(int attempt)
    {
        _currentAttempt = attempt;
        var delay = CalculateExponentialBackoff();
        var jitter = CalculateRandomJitter(delay);
        return TimeSpan.FromMilliseconds(delay + jitter);
    }

    // 自适应抖动算法
    public TimeSpan GetAdaptiveJitterDelay(int attempt, bool lastOperationSuccessful)
    {
        _currentAttempt = attempt;
        var delay = CalculateExponentialBackoff();
        
        if (_options.UseAdaptiveJitter && lastOperationSuccessful)
        {
            delay = (int)(delay * 0.8); // 成功时减少延迟
        }
        else if (_options.UseAdaptiveJitter)
        {
            delay = (int)(delay * 1.2); // 失败时增加延迟
        }
        
        var jitter = CalculateRandomJitter(delay);
        return TimeSpan.FromMilliseconds(Math.Min(delay + jitter, _options.MaxDelayMs));
    }

    private int CalculateExponentialBackoff()
    {
        return (int)Math.Min(
            _options.BaseDelayMs * Math.Pow(_options.Exponent, _currentAttempt),
            _options.MaxDelayMs);
    }

    // 更平稳的抖动算法
private double CalculateRandomJitter(int delay)
{
    // 使用正弦函数平滑抖动
    var smoothJitter = Math.Sin(_random.NextDouble() * Math.PI) * _options.JitterFactor * delay;
    
    // 动态调整抖动因子
    _smoothJitterFactor = Math.Max(0.1, Math.Min(0.9, 
        _smoothJitterFactor * (_random.NextDouble() > 0.5 ? 1.05 : 0.95)));
    
    // 结合平滑抖动和动态因子
    return smoothJitter * _smoothJitterFactor;
}
}

// DI扩展方法
public static class JitterExtensions
{
    public static IServiceCollection AddJitterAlgorithm(this IServiceCollection services, Action<JitterOptions> configure = null)
    {
        services.Configure(configure ?? (opts => { }));
        services.AddSingleton<JitterAlgorithm>();
        return services;
    }
}

// 使用示例
public class JitterUsageExample
{
    private readonly JitterAlgorithm _jitter;

    public JitterUsageExample(JitterAlgorithm jitter)
    {
        _jitter = jitter;
    }

    public async Task ExecuteWithRetryAsync(Func<Task> operation, int maxRetries = 5)
    {
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                await operation();
                return;
            }
            catch
            {
                if (attempt == maxRetries - 1) throw;
                
                var delay = _jitter.GetJitterDelay(attempt);
                await Task.Delay(delay);
            }
        }
    }
}