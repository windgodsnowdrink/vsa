#:sdk Microsoft.NET.Sdk
#:package Serilog@3.1.1
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;

public static class SensitiveDataProtectionExtensions
{
    private static readonly Regex _emailRegex = new(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b");
    private static readonly Regex _phoneRegex = new(@"\b\d{3}[-.]?\d{3}[-.]?\d{4}\b");
    private static readonly Regex _passwordRegex = new(@"\b(password|pwd|pass)=([^&\s]+)\b", RegexOptions.IgnoreCase);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static LoggerConfiguration WithDataProtection(this LoggerConfiguration config, IServiceProvider serviceProvider)
    {
        var protector = serviceProvider.GetRequiredService<DataProtectionService>();
        return config.Destructure.With<ProtectedDataDestructuringPolicy>(protector);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static string ProtectSensitiveData(this DataProtectionService protector, string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var protectedText = _emailRegex.Replace(input, match => protector.ProtectAsync(match.Value).Result);
        protectedText = _phoneRegex.Replace(protectedText, match => protector.ProtectAsync(match.Value).Result);
        protectedText = _passwordRegex.Replace(protectedText, match => $"{match.Groups[1].Value}={protector.ProtectAsync(match.Groups[2].Value).Result}");
        
        return protectedText;
    }
}

[SkipLocalsInit]
internal class ProtectedDataDestructuringPolicy : IDestructuringPolicy
{
    private readonly DataProtectionService _protector;

    public ProtectedDataDestructuringPolicy(DataProtectionService protector)
    {
        _protector = protector;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
    {
        if (value is string str)
        {
            result = new ScalarValue(_protector.ProtectSensitiveData(str));
            return true;
        }
        
        result = null;
        return false;
    }
}

var services = new ServiceCollection();
services.AddDataProtection()
    .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect("localhost"))
    .SetApplicationName("MyApp");

services.AddSingleton<DataProtectionService>();
services.AddLogging(logging => 
{
    var logger = new LoggerConfiguration()
        .WithDataProtection(services.BuildServiceProvider())
        .WriteTo.Console()
        .CreateLogger();
    
    logging.AddSerilog(logger);
});

var serviceProvider = services.BuildServiceProvider();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

// 测试日志输出
logger.LogInformation("User email: {Email}", "user@example.com");
logger.LogInformation("Phone number: {Phone}", "13812345678");
logger.LogWarning("Login failed for password: {Password}", "myp@ssw0rd");