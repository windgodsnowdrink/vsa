#:sdk Microsoft.NET.Sdk.Web
#:package TailwindCSS@3.4.0
#:package Microsoft.AspNetCore.SpaServices.Extensions@7.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class TailwindOptions
{
    public string ConfigPath { get; set; } = "./tailwind.config.js";
    public string InputCssPath { get; set; } = "./wwwroot/css/app.css";
    public string OutputCssPath { get; set; } = "./wwwroot/css/tailwind.css";
    public bool EnableJIT { get; set; } = true;
    public bool WatchMode { get; set; } = true;
    public string Theme { get; set; } = "light";
    public bool EnablePurge { get; set; } = true;
    public string[] PurgeContent { get; set; } = Array.Empty<string>();
    public bool EnableMinify { get; set; } = true;
    public bool EnableSourceMaps { get; set; } = false;
    public string[] Plugins { get; set; } = Array.Empty<string>();
}

public interface ITailwindService
{
    Task BuildAsync();
    Task WatchAsync();
    Task SetThemeAsync(string theme);
    Task OptimizeAsync();
    Task InstallPluginAsync(string pluginName);
    Task RemovePluginAsync(string pluginName);
}

public class TailwindService : ITailwindService
{
    private readonly TailwindOptions _options;
    private readonly ILogger<TailwindService> _logger;
    private readonly ObjectPool<StringBuilder> _stringBuilderPool;

    public TailwindService(IOptions<TailwindOptions> options, 
                          ILogger<TailwindService> logger,
                          ObjectPool<StringBuilder> stringBuilderPool)
    {
        _options = options.Value;
        _logger = logger;
        _stringBuilderPool = stringBuilderPool;
    }

    public async Task BuildAsync()
    {
        var sb = _stringBuilderPool.Get();
        try
        {
            sb.Append("npx tailwindcss")
              .Append(" -i ").Append(_options.InputCssPath)
              .Append(" -o ").Append(_options.OutputCssPath)
              .Append(" --config ").Append(_options.ConfigPath);

            if (_options.EnableJIT) sb.Append(" --jit");
            if (_options.EnablePurge) 
            {
                sb.Append(" --purge ").Append(string.Join(" ", _options.PurgeContent));
            }
            if (_options.EnableMinify) sb.Append(" --minify");
            if (_options.EnableSourceMaps) sb.Append(" --source-maps");

            var processInfo = new ProcessStartInfo
            {
                FileName = "npx",
                Arguments = sb.ToString(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            await process.WaitForExitAsync();
        }
        finally
        {
            _stringBuilderPool.Return(sb);
        }
    }

    public async Task WatchAsync()
    {
        var sb = _stringBuilderPool.Get();
        try
        {
            sb.Append("npx tailwindcss")
              .Append(" -i ").Append(_options.InputCssPath)
              .Append(" -o ").Append(_options.OutputCssPath)
              .Append(" --config ").Append(_options.ConfigPath)
              .Append(" --watch");

            if (_options.EnableJIT) sb.Append(" --jit");
            if (_options.Theme == "dark") sb.Append(" --dark");

            var processInfo = new ProcessStartInfo
            {
                FileName = "npx",
                Arguments = sb.ToString(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            await process.WaitForExitAsync();
        }
        finally
        {
            _stringBuilderPool.Return(sb);
        }
    }

    public async Task SetThemeAsync(string theme)
    {
        _options.Theme = theme;
        await BuildAsync();
    }

    public async Task OptimizeAsync()
    {
        _options.EnablePurge = true;
        _options.EnableMinify = true;
        await BuildAsync();
    }

    public async Task InstallPluginAsync(string pluginName)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "npm",
            Arguments = $"install {pluginName}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processInfo);
        await process.WaitForExitAsync();

        _options.Plugins = _options.Plugins.Append(pluginName).ToArray();
        await BuildAsync();
    }

    public async Task RemovePluginAsync(string pluginName)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "npm",
            Arguments = $"uninstall {pluginName}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processInfo);
        await process.WaitForExitAsync();

        _options.Plugins = _options.Plugins.Where(p => p != pluginName).ToArray();
        await BuildAsync();
    }
}

public static class TailwindExtensions
{
    public static IServiceCollection AddTailwind(this IServiceCollection services, Action<TailwindOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<ITailwindService, TailwindService>();
        services.AddLogging();
        services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.AddSingleton(serviceProvider => 
        {
            var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
            return provider.Create<StringBuilder>(new StringBuilderPooledPolicy());
        });
        return services;
    }

    public static IApplicationBuilder UseTailwind(this IApplicationBuilder app)
    {
        var tailwind = app.ApplicationServices.GetRequiredService<ITailwindService>();
        tailwind.BuildAsync().GetAwaiter().GetResult();
        
        if (app.ApplicationServices.GetRequiredService<IOptions<TailwindOptions>>().Value.WatchMode)
        {
            tailwind.WatchAsync().ConfigureAwait(false);
        }
        
        return app;
    }
}

// 示例用法
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTailwind(options =>
{
    options.ConfigPath = "./ClientApp/tailwind.config.js";
    options.InputCssPath = "./ClientApp/src/styles/app.css";
    options.OutputCssPath = "./wwwroot/css/tailwind.css";
    options.EnableJIT = true;
    options.WatchMode = true;
});

var app = builder.Build();

app.UseTailwind();

app.MapGet("/", () => "Hello World!");

app.Run();