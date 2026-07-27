#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Net.Http@4.3.4
#:package System.Net.Http.Json@8.0.0
#:package System.Net.Http.Formatting.Extension@5.2.3
#:package Refit@7.0.0
#:package RestSharp@108.0.3
#:package Polly@8.3.1
#:package Microsoft.Extensions.Http.Polly@10.0.0
#:package System.Net.WebSockets@4.3.0
#:package System.Net.NameResolution@4.3.0
#:package System.Net.Security@4.3.2
#:package System.Net.Sockets@4.3.0
#:package System.Text.Json@8.0.0
#:package System.Text.RegularExpressions@4.3.1
#:package System.Collections.Immutable@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property RuntimeIdentifier=win-x64

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("HTTP AOT 引擎");
        Console.WriteLine("=" * 60);
        
        var serviceProvider = BuildServiceProvider();
        var httpService = serviceProvider.GetRequiredService<HttpService>();
        var settings = serviceProvider.GetRequiredService<IOptions<HttpSettings>>().Value;
        
        var command = args.Length > 0 ? args[0].ToLower() : "help";
        var arguments = args.Skip(1).ToArray();
        
        try
        {
            switch (command)
            {
                case "get":
                case "g":
                    await SendGetRequest(httpService, arguments);
                    break;
                case "post":
                case "p":
                    await SendPostRequest(httpService, arguments);
                    break;
                case "put":
                case "pu":
                    await SendPutRequest(httpService, arguments);
                    break;
                case "delete":
                case "d":
                    await SendDeleteRequest(httpService, arguments);
                    break;
                case "head":
                case "he":
                    await SendHeadRequest(httpService, arguments);
                    break;
                case "options":
                case "o":
                    await SendOptionsRequest(httpService, arguments);
                    break;
                case "benchmark":
                case "b":
                    await RunBenchmark(httpService, arguments);
                    break;
                case "proxy":
                case "pr":
                    await TestProxy(httpService, arguments);
                    break;
                case "timeout":
                case "t":
                    await TestTimeout(httpService, arguments);
                    break;
                case "circuit":
                case "c":
                    await TestCircuitBreaker(httpService, arguments);
                    break;
                case "config":
                case "co":
                    ShowConfig(settings);
                    break;
                case "help":
                case "h":
                case "?":
                    ShowHelp();
                    break;
                default:
                    Console.WriteLine($"未知命令: {command}");
                    ShowHelp();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
        finally
        {
            await httpService.DisposeAsync();
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.Configure<HttpSettings>(options => {
            options.BaseUrl = "https://example.com";
            options.Timeout = TimeSpan.FromSeconds(30);
            options.RetryCount = 3;
            options.RetryDelay = TimeSpan.FromSeconds(1);
            options.EnableCircuitBreaker = true;
            options.CircuitBreakDuration = TimeSpan.FromSeconds(30);
            options.CircuitBreakFailureThreshold = 0.5;
            options.CircuitBreakSamplingDuration = TimeSpan.FromSeconds(30);
            options.CircuitBreakMinimumThroughput = 10;
            options.EnableProxy = false;
            options.ProxyAddress = "";
            options.EnableCompression = true;
            options.EnableCaching = true;
            options.CacheSize = 100;
            options.CacheDuration = TimeSpan.FromMinutes(5);
            options.EnableDetailedLogging = false;
        });
        
        services.AddSingleton<HttpService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        return services.BuildServiceProvider();
    }
    
    private static async Task SendGetRequest(HttpService service, string[] arguments)
    {
        var url = arguments.Length > 0 ? arguments[0] : "https://example.com";
        Console.WriteLine($"发送 GET 请求: {url}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.SendGetRequestAsync(url);
        stopwatch.Stop();
        
        Console.WriteLine($"请求完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"状态码: {result.StatusCode}");
        Console.WriteLine($"内容长度: {result.ContentLength} 字节");
        Console.WriteLine($"内容类型: {result.ContentType}");
        Console.WriteLine($"响应头: {result.Headers.Count} 个");
        
        if (!string.IsNullOrEmpty(result.Content))
        {
            Console.WriteLine($"\n响应内容:");
            Console.WriteLine(result.Content.Length > 1000 ? result.Content.Substring(0, 1000) + "..." : result.Content);
        }
    }
    
    private static async Task SendPostRequest(HttpService service, string[] arguments)
    {
        var url = arguments.Length > 0 ? arguments[0] : "https://example.com";
        var content = arguments.Length > 1 ? arguments[1] : "{\"name\": \"test\", \"value\": 123}";
        Console.WriteLine($"发送 POST 请求: {url}");
        Console.WriteLine($"请求内容: {content}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.SendPostRequestAsync(url, content);
        stopwatch.Stop();
        
        Console.WriteLine($"请求完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"状态码: {result.StatusCode}");
        Console.WriteLine($"内容长度: {result.ContentLength} 字节");
        
        if (!string.IsNullOrEmpty(result.Content))
        {
            Console.WriteLine($"\n响应内容:");
            Console.WriteLine(result.Content.Length > 1000 ? result.Content.Substring(0, 1000) + "..." : result.Content);
        }
    }
    
    private static async Task SendPutRequest(HttpService service, string[] arguments)
    {
        var url = arguments.Length > 0 ? arguments[0] : "https://example.com";
        var content = arguments.Length > 1 ? arguments[1] : "{\"name\": \"test\", \"value\": 456}";
        Console.WriteLine($"发送 PUT 请求: {url}");
        Console.WriteLine($"请求内容: {content}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.SendPutRequestAsync(url, content);
        stopwatch.Stop();
        
        Console.WriteLine($"请求完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"状态码: {result.StatusCode}");
        Console.WriteLine($"内容长度: {result.ContentLength} 字节");
    }
    
    private static async Task SendDeleteRequest(HttpService service, string[] arguments)
    {
        var url = arguments.Length > 0 ? arguments[0] : "https://example.com";
        Console.WriteLine($"发送 DELETE 请求: {url}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.SendDeleteRequestAsync(url);
        stopwatch.Stop();
        
        Console.WriteLine($"请求完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"状态码: {result.StatusCode}");
    }
    
    private static async Task SendHeadRequest(HttpService service, string[] arguments)
    {
        var url = arguments.Length > 0 ? arguments[0] : "https://example.com";
        Console.WriteLine($"发送 HEAD 请求: {url}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.SendHeadRequestAsync(url);
        stopwatch.Stop();
        
        Console.WriteLine($"请求完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"状态码: {result.StatusCode}");
        Console.WriteLine($"响应头:");
        foreach (var header in result.Headers)
        {
            Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
        }
    }
    
    private static async Task SendOptionsRequest(HttpService service, string[] arguments)
    {
        var url = arguments.Length > 0 ? arguments[0] : "https://example.com";
        Console.WriteLine($"发送 OPTIONS 请求: {url}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.SendOptionsRequestAsync(url);
        stopwatch.Stop();
        
        Console.WriteLine($"请求完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"状态码: {result.StatusCode}");
        Console.WriteLine($"允许的方法: {result.Allow}");
    }
    
    private static async Task RunBenchmark(HttpService service, string[] arguments)
    {
        var url = arguments.Length > 0 ? arguments[0] : "https://example.com";
        var iterations = arguments.Length > 1 ? int.Parse(arguments[1]) : 100;
        Console.WriteLine($"运行基准测试: {url}");
        Console.WriteLine($"迭代次数: {iterations}");
        
        var stopwatch = Stopwatch.StartNew();
        var successes = 0;
        var failures = 0;
        var totalBytes = 0L;
        
        for (int i = 0; i < iterations; i++)
        {
            try
            {
                var result = await service.SendGetRequestAsync(url);
                successes++;
                totalBytes += result.ContentLength;
            }
            catch (Exception)
            {
                failures++;
            }
        }
        
        stopwatch.Stop();
        var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
        var requestsPerSecond = iterations / elapsedSeconds;
        var bytesPerSecond = totalBytes / elapsedSeconds;
        
        Console.WriteLine($"基准测试完成!");
        Console.WriteLine($"总用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"成功: {successes}");
        Console.WriteLine($"失败: {failures}");
        Console.WriteLine($"每秒请求数: {requestsPerSecond:F2} req/s");
        Console.WriteLine($"每秒字节数: {bytesPerSecond:F2} B/s");
    }
    
    private static async Task TestProxy(HttpService service, string[] arguments)
    {
        var url = arguments.Length > 0 ? arguments[0] : "https://example.com";
        var proxyUrl = arguments.Length > 1 ? arguments[1] : "http://localhost:8080";
        Console.WriteLine($"测试代理: {url}");
        Console.WriteLine($"代理地址: {proxyUrl}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.SendGetRequestWithProxyAsync(url, proxyUrl);
        stopwatch.Stop();
        
        Console.WriteLine($"请求完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"状态码: {result.StatusCode}");
    }
    
    private static async Task TestTimeout(HttpService service, string[] arguments)
    {
        var url = arguments.Length > 0 ? arguments[0] : "https://example.com";
        var timeout = arguments.Length > 1 ? int.Parse(arguments[1]) : 1;
        Console.WriteLine($"测试超时: {url}");
        Console.WriteLine($"超时时间: {timeout} 秒");
        
        try
        {
            var result = await service.SendGetRequestWithTimeoutAsync(url, TimeSpan.FromSeconds(timeout));
            Console.WriteLine($"请求完成! 状态码: {result.StatusCode}");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"超时错误: {ex.Message}");
        }
    }
    
    private static async Task TestCircuitBreaker(HttpService service, string[] arguments)
    {
        var url = arguments.Length > 0 ? arguments[0] : "https://example.com";
        Console.WriteLine($"测试断路器: {url}");
        
        for (int i = 0; i < 10; i++)
        {
            try
            {
                var result = await service.SendGetRequestAsync(url);
                Console.WriteLine($"请求 {i + 1}: 成功, 状态码: {result.StatusCode}");
            }
            catch (BrokenCircuitException ex)
            {
                Console.WriteLine($"请求 {i + 1}: 断路器打开, 错误: {ex.Message}");
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"请求 {i + 1}: 失败, 错误: {ex.Message}");
            }
            await Task.Delay(500);
        }
    }
    
    private static void ShowConfig(HttpSettings settings)
    {
        Console.WriteLine("HTTP 配置:");
        Console.WriteLine("=" * 60);
        Console.WriteLine($"基础 URL: {settings.BaseUrl}");
        Console.WriteLine($"超时: {settings.Timeout}");
        Console.WriteLine($"重试次数: {settings.RetryCount}");
        Console.WriteLine($"重试延迟: {settings.RetryDelay}");
        Console.WriteLine($"启用断路器: {settings.EnableCircuitBreaker}");
        Console.WriteLine($"断路器持续时间: {settings.CircuitBreakDuration}");
        Console.WriteLine($"断路器失败阈值: {settings.CircuitBreakFailureThreshold}");
        Console.WriteLine($"断路器采样持续时间: {settings.CircuitBreakSamplingDuration}");
        Console.WriteLine($"断路器最小吞吐量: {settings.CircuitBreakMinimumThroughput}");
        Console.WriteLine($"启用代理: {settings.EnableProxy}");
        Console.WriteLine($"代理地址: {settings.ProxyAddress}");
        Console.WriteLine($"启用压缩: {settings.EnableCompression}");
        Console.WriteLine($"启用缓存: {settings.EnableCaching}");
        Console.WriteLine($"缓存大小: {settings.CacheSize}");
        Console.WriteLine($"缓存持续时间: {settings.CacheDuration}");
        Console.WriteLine($"启用详细日志: {settings.EnableDetailedLogging}");
    }
    
    private static void ShowHelp()
    {
        Console.WriteLine("HTTP AOT 引擎 命令帮助:");
        Console.WriteLine("=" * 60);
        Console.WriteLine("get (g)        - 发送 GET 请求");
        Console.WriteLine("post (p)       - 发送 POST 请求");
        Console.WriteLine("put (pu)       - 发送 PUT 请求");
        Console.WriteLine("delete (d)     - 发送 DELETE 请求");
        Console.WriteLine("head (he)      - 发送 HEAD 请求");
        Console.WriteLine("options (o)    - 发送 OPTIONS 请求");
        Console.WriteLine("benchmark (b)  - 运行基准测试");
        Console.WriteLine("proxy (pr)     - 测试代理");
        Console.WriteLine("timeout (t)    - 测试超时");
        Console.WriteLine("circuit (c)    - 测试断路器");
        Console.WriteLine("config (co)    - 显示配置信息");
        Console.WriteLine("help (h, ?)    - 显示帮助信息");
    }
}

public class HttpSettings
{
    public string BaseUrl { get; set; } = "https://example.com";
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public int RetryCount { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    public bool EnableCircuitBreaker { get; set; } = true;
    public TimeSpan CircuitBreakDuration { get; set; } = TimeSpan.FromSeconds(30);
    public double CircuitBreakFailureThreshold { get; set; } = 0.5;
    public TimeSpan CircuitBreakSamplingDuration { get; set; } = TimeSpan.FromSeconds(30);
    public int CircuitBreakMinimumThroughput { get; set; } = 10;
    public bool EnableProxy { get; set; } = false;
    public string ProxyAddress { get; set; } = "";
    public bool EnableCompression { get; set; } = true;
    public bool EnableCaching { get; set; } = true;
    public int CacheSize { get; set; } = 100;
    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(5);
    public bool EnableDetailedLogging { get; set; } = false;
}

public class HttpResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public string Content { get; set; }
    public long ContentLength { get; set; }
    public string ContentType { get; set; }
    public Dictionary<string, IEnumerable<string>> Headers { get; set; } = new();
    public string Allow { get; set; }
}

public class HttpService : IAsyncDisposable
{
    private readonly ILogger<HttpService> _logger;
    private readonly HttpClient _httpClient;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;
    private readonly AsyncTimeoutPolicy<HttpResponseMessage> _timeoutPolicy;
    private readonly Dictionary<string, (string Content, DateTime Expires)> _cache;
    private readonly object _cacheLock = new();
    
    public HttpService(ILogger<HttpService> logger)
    {
        _logger = logger;
        
        // 创建 HttpClient
        var handler = new HttpClientHandler {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        _httpClient = new HttpClient(handler);
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("HTTP-AOT-Engine/1.0");
        
        // 配置重试策略
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) * 0.5));
        
        // 配置断路器策略
        _circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(
                failureThreshold: 0.5,
                samplingDuration: TimeSpan.FromSeconds(30),
                minimumThroughput: 10,
                durationOfBreak: TimeSpan.FromSeconds(30)
            );
        
        // 配置超时策略
        _timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30));
        
        // 初始化缓存
        _cache = new Dictionary<string, (string, DateTime)>();
    }
    
    public async Task<HttpResponse> SendGetRequestAsync(string url)
    {
        // 检查缓存
        if (_cache.TryGetValue(url, out var cachedItem) && cachedItem.Expires > DateTime.UtcNow)
        {
            return new HttpResponse {
                StatusCode = HttpStatusCode.OK,
                Content = cachedItem.Content,
                ContentLength = cachedItem.Content.Length,
                ContentType = "text/plain"
            };
        }
        
        var response = await _timeoutPolicy.ExecuteAsync(async () => {
            return await _circuitBreakerPolicy.ExecuteAsync(async () => {
                return await _retryPolicy.ExecuteAsync(async () => {
                    return await _httpClient.GetAsync(url);
                });
            });
        });
        
        var result = await ProcessResponseAsync(response);
        
        // 添加到缓存
        if (result.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(result.Content))
        {
            lock (_cacheLock)
            {
                if (_cache.Count >= 100)
                {
                    var oldestKey = _cache.OrderBy(kv => kv.Value.Expires).First().Key;
                    _cache.Remove(oldestKey);
                }
                _cache[url] = (result.Content, DateTime.UtcNow.AddMinutes(5));
            }
        }
        
        return result;
    }
    
    public async Task<HttpResponse> SendPostRequestAsync(string url, string content)
    {
        var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
        
        var response = await _timeoutPolicy.ExecuteAsync(async () => {
            return await _circuitBreakerPolicy.ExecuteAsync(async () => {
                return await _retryPolicy.ExecuteAsync(async () => {
                    return await _httpClient.PostAsync(url, httpContent);
                });
            });
        });
        
        return await ProcessResponseAsync(response);
    }
    
    public async Task<HttpResponse> SendPutRequestAsync(string url, string content)
    {
        var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
        
        var response = await _timeoutPolicy.ExecuteAsync(async () => {
            return await _circuitBreakerPolicy.ExecuteAsync(async () => {
                return await _retryPolicy.ExecuteAsync(async () => {
                    return await _httpClient.PutAsync(url, httpContent);
                });
            });
        });
        
        return await ProcessResponseAsync(response);
    }
    
    public async Task<HttpResponse> SendDeleteRequestAsync(string url)
    {
        var response = await _timeoutPolicy.ExecuteAsync(async () => {
            return await _circuitBreakerPolicy.ExecuteAsync(async () => {
                return await _retryPolicy.ExecuteAsync(async () => {
                    return await _httpClient.DeleteAsync(url);
                });
            });
        });
        
        return await ProcessResponseAsync(response);
    }
    
    public async Task<HttpResponse> SendHeadRequestAsync(string url)
    {
        var response = await _timeoutPolicy.ExecuteAsync(async () => {
            return await _circuitBreakerPolicy.ExecuteAsync(async () => {
                return await _retryPolicy.ExecuteAsync(async () => {
                    return await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                });
            });
        });
        
        var result = new HttpResponse {
            StatusCode = response.StatusCode
        };
        
        foreach (var header in response.Headers)
        {
            result.Headers[header.Key] = header.Value;
        }
        
        return result;
    }
    
    public async Task<HttpResponse> SendOptionsRequestAsync(string url)
    {
        var response = await _timeoutPolicy.ExecuteAsync(async () => {
            return await _circuitBreakerPolicy.ExecuteAsync(async () => {
                return await _retryPolicy.ExecuteAsync(async () => {
                    return await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Options, url));
                });
            });
        });
        
        var result = new HttpResponse {
            StatusCode = response.StatusCode
        };
        
        if (response.Headers.TryGetValues("Allow", out var allowValues))
        {
            result.Allow = string.Join(", ", allowValues);
        }
        
        return result;
    }
    
    public async Task<HttpResponse> SendGetRequestWithProxyAsync(string url, string proxyUrl)
    {
        var proxyHandler = new HttpClientHandler {
            Proxy = new WebProxy(proxyUrl),
            UseProxy = true,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        
        using var proxyClient = new HttpClient(proxyHandler);
        var response = await proxyClient.GetAsync(url);
        
        return await ProcessResponseAsync(response);
    }
    
    public async Task<HttpResponse> SendGetRequestWithTimeoutAsync(string url, TimeSpan timeout)
    {
        using var timeoutClient = new HttpClient {
            Timeout = timeout
        };
        
        var response = await timeoutClient.GetAsync(url);
        return await ProcessResponseAsync(response);
    }
    
    private async Task<HttpResponse> ProcessResponseAsync(HttpResponseMessage response)
    {
        var result = new HttpResponse {
            StatusCode = response.StatusCode,
            ContentLength = response.Content.Headers.ContentLength ?? 0,
            ContentType = response.Content.Headers.ContentType?.ToString()
        };
        
        // 复制响应头
        foreach (var header in response.Headers)
        {
            result.Headers[header.Key] = header.Value;
        }
        
        // 读取内容
        if (response.Content != null)
        {
            result.Content = await response.Content.ReadAsStringAsync();
            if (result.ContentLength == 0 && !string.IsNullOrEmpty(result.Content))
            {
                result.ContentLength = result.Content.Length;
            }
        }
        
        return result;
    }
    
    public async ValueTask DisposeAsync()
    {
        _httpClient.Dispose();
        _cache.Clear();
        await Task.CompletedTask;
    }
}
