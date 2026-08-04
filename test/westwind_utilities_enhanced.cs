#:sdk Microsoft.NET.Sdk.Web
#:package Westwind.Utilities@7.0.0
#:package Westwind.Data@7.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0

using Westwind.Utilities;
using Westwind.Utilities.Data;
using Westwind.Utilities.Email;

public class WestwindEnhancedOptions
{
    public DatabaseOptions Database { get; set; }
    public EmailOptions Email { get; set; }
    public MarkdownOptions Markdown { get; set; }
}

public interface IWestwindEnhancedService
{
    // 数据库访问
    Task<List<T>> QueryAsync<T>(string sql, object parameters = null);
    Task<int> ExecuteAsync(string sql, object parameters = null);
    
    // 邮件发送
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    
    // Markdown处理
    string MarkdownToHtml(string markdown);
    string HtmlToMarkdown(string html);
}

public class WestwindEnhancedService : IWestwindEnhancedService
{
    private readonly DataAccess _dataAccess;
    private readonly EmailSender _emailSender;
    private readonly WestwindEnhancedOptions _options;
    
    public WestwindEnhancedService(
        DataAccess dataAccess,
        EmailSender emailSender,
        IOptions<WestwindEnhancedOptions> options)
    {
        _dataAccess = dataAccess;
        _emailSender = emailSender;
        _options = options.Value;
    }
    
    // 数据库访问实现
    public async Task<List<T>> QueryAsync<T>(string sql, object parameters = null)
    {
        return await _dataAccess.QueryAsync<T>(sql, parameters);
    }
    
    public async Task<int> ExecuteAsync(string sql, object parameters = null)
    {
        return await _dataAccess.ExecuteAsync(sql, parameters);
    }
    
    // 邮件发送实现
    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        var message = new EmailMessage
        {
            To = to,
            Subject = subject,
            Body = body,
            IsHtml = isHtml
        };
        return await _emailSender.SendEmailAsync(message);
    }
    
    // Markdown处理实现
    public string MarkdownToHtml(string markdown)
    {
        return MarkdownUtils.MarkdownToHtml(markdown);
    }
    
    public string HtmlToMarkdown(string html)
    {
        return MarkdownUtils.HtmlToMarkdown(html);
    }
}

// DI扩展方法
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWestwindEnhancedServices(
        this IServiceCollection services,
        Action<WestwindEnhancedOptions> configureOptions)
    {
        services.Configure(configureOptions);
        
        // 配置数据库访问
        var options = new WestwindEnhancedOptions();
        configureOptions(options);
        
        services.AddSingleton<DataAccess>(sp => 
            new DataAccess(options.Database.ConnectionString));
            
        // 配置邮件发送
        services.AddSingleton<EmailSender>(sp => 
            new EmailSender(options.Email.SmtpServer, options.Email.SmtpUsername, options.Email.SmtpPassword));
        
        services.AddScoped<IWestwindEnhancedService, WestwindEnhancedService>();
        
        return services;
    }
}

// 示例用法
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWestwindEnhancedServices(options =>
{
    options.Database = new DatabaseOptions
    {
        ConnectionString = "Server=.;Database=Test;Trusted_Connection=True;"
    };
    
    options.Email = new EmailOptions
    {
        SmtpServer = "smtp.example.com",
        SmtpUsername = "user@example.com",
        SmtpPassword = "password"
    };
    
    options.Markdown = new MarkdownOptions
    {
        AutoHyperlink = true,
        AutoNewLines = true
    };
});

var app = builder.Build();

app.MapGet("/query", async (IWestwindEnhancedService service) =>
{
    return await service.QueryAsync<dynamic>("SELECT * FROM Users");
});

app.MapGet("/email", async (IWestwindEnhancedService service) =>
{
    return await service.SendEmailAsync("recipient@example.com", "Test", "<b>Hello</b>");
});

app.MapGet("/markdown", (IWestwindEnhancedService service) =>
{
    return service.MarkdownToHtml("**bold** and *italic*");
});

app.Run();