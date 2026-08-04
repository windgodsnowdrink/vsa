#:sdk Microsoft.NET.Sdk.Web
#:package Bit.SourceGenerators@18.0.0
#:package Bit.Server.Data@18.0.0
#:package Bit.Server.Data.EntityFrameworkCore@18.0.0
#:package Bit.Server.Core@18.0.0
#:package Bit.Server.WebApi@18.0.0
#:package Bit.Server.IdentityServer@18.0.0
#:package Bit.Server.Hangfire@18.0.0
#:package Bit.Server.Signalr@18.0.0
#:package Bit.Bswup@9.11.3
#:package Bit.Butil@9.11.3
#:package Bit.Besql@9.11.3
#:package Bit.Bup@9.11.3
#:package Bit.SourceGenerators@9.11.3
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using BitPlatform;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class BitPlatformOptions
{
    public string DefaultTheme { get; set; } = "light";
    public bool EnableAOT { get; set; } = true;
    public int MaxConcurrentOperations { get; set; } = 100;
    public TimeSpan OperationTimeout { get; set; } = TimeSpan.FromSeconds(30);
}

public interface IBitPlatformService
{
    Task InitializeAsync();
    Task<T> ExecuteOperationAsync<T>(Func<T> operation);
    Task RenderUIAsync(string componentName, object parameters);
}

public class BitPlatformService : IBitPlatformService, IDisposable
{
    private readonly BitPlatformOptions _options;
    private readonly BitPlatformInstance _platformInstance;

    public BitPlatformService(IOptions<BitPlatformOptions> options)
    {
        _options = options.Value;
        _platformInstance = new BitPlatformInstance()
            .WithTheme(_options.DefaultTheme)
            .WithAOT(_options.EnableAOT)
            .WithMaxConcurrency(_options.MaxConcurrentOperations);
    }

    public async Task InitializeAsync()
    {
        await _platformInstance.InitializeAsync();
    }

    public async Task<T> ExecuteOperationAsync<T>(Func<T> operation)
    {
        return await _platformInstance.ExecuteOperationAsync(operation, _options.OperationTimeout);
    }

    public async Task RenderUIAsync(string componentName, object parameters)
    {
        await _platformInstance.RenderUIAsync(componentName, parameters);
    }

    public void Dispose()
    {
        _platformInstance?.Dispose();
    }
}

public static class BitPlatformExtensions
{
    public static IServiceCollection AddBitPlatform(this IServiceCollection services, Action<BitPlatformOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<IBitPlatformService, BitPlatformService>();
        return services;
    }
}

public class BitPlatformExample
{
    public static async Task Demo()
    {
        var services = new ServiceCollection();
        services.AddBitPlatform(options =>
        {
            options.DefaultTheme = "dark";
            options.EnableAOT = true;
            options.MaxConcurrentOperations = 200;
            options.OperationTimeout = TimeSpan.FromMinutes(1);
        });

        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IBitPlatformService>();

        try
        {
            await service.InitializeAsync();
            await service.RenderUIAsync("MainComponent", new { Title = "BitPlatform Demo" });
            var result = await service.ExecuteOperationAsync(() => 42);
            Console.WriteLine($"Operation result: {result}");
        }
        finally
        {
            (service as IDisposable)?.Dispose();
        }
    }
}