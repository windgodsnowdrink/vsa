#:sdk Microsoft.NET.Sdk
#:package Westwind.Utilities@7.0.0
#:property LangVersion preview
#:property TargetFramework net11.0

using Westwind.Utilities;
using Westwind.Utilities.Web;

public class WestwindOptions
{
    public string DefaultConnectionString { get; set; }
    public int MaxRetryCount { get; set; } = 3;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}

public interface IWestwindService
{
    // 字符串处理
    string SanitizeHtml(string html);
    string ToCamelCase(string str);
    
    // 文件操作
    Task<string> ReadFileAsync(string path);
    Task WriteFileAsync(string path, string content);
    
    // HTTP工具
    Task<string> HttpGetAsync(string url);
    Task<string> HttpPostJsonAsync(string url, object data);
    
    // 数据转换
    T JsonToObject<T>(string json);
    string ObjectToJson(object obj);
}

public class WestwindService : IWestwindService
{
    private readonly WestwindOptions _options;
    
    public WestwindService(IOptions<WestwindOptions> options)
    {
        _options = options.Value;
    }
    
    // 字符串处理实现
    public string SanitizeHtml(string html)
    {
        return HtmlUtils.SanitizeHtml(html);
    }
    
    public string ToCamelCase(string str)
    {
        return StringUtils.ToCamelCase(str);
    }
    
    // 文件操作实现
    public async Task<string> ReadFileAsync(string path)
    {
        return await FileUtils.ReadAllTextAsync(path);
    }
    
    public async Task WriteFileAsync(string path, string content)
    {
        await FileUtils.WriteAllTextAsync(path, content);
    }
    
    // HTTP工具实现
    public async Task<string> HttpGetAsync(string url)
    {
        using var client = new HttpClient { Timeout = _options.Timeout };
        return await client.GetStringAsync(url);
    }
    
    public async Task<string> HttpPostJsonAsync(string url, object data)
    {
        using var client = new HttpClient { Timeout = _options.Timeout };
        return await client.PostJsonAsync(url, data);
    }
    
    // 数据转换实现
    public T JsonToObject<T>(string json)
    {
        return JsonSerializationUtils.Deserialize<T>(json);
    }
    
    public string ObjectToJson(object obj)
    {
        return JsonSerializationUtils.Serialize(obj);
    }
}

// DI扩展方法
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWestwindServices(
        this IServiceCollection services,
        Action<WestwindOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddScoped<IWestwindService, WestwindService>();
        return services;
    }
}

// 示例用法
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWestwindServices(options =>
{
    options.DefaultConnectionString = "Server=.;Database=Test;Trusted_Connection=True;";
    options.MaxRetryCount = 5;
    options.Timeout = TimeSpan.FromMinutes(1);
});

var app = builder.Build();

app.MapGet("/sanitize", (IWestwindService service) =>
{
    return service.SanitizeHtml("<script>alert('xss')</script><p>safe</p>");
});

app.MapGet("/json", (IWestwindService service) =>
{
    var obj = new { Name = "Test", Value = 123 };
    return service.ObjectToJson(obj);
});

app.Run();