#:sdk Microsoft.NET.Sdk.Web
#:package GenVue@3.2.0
#:package Microsoft.VisualStudio.Azure.Containers.Tools.Targets@1.19.6
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property UserSecretsId 210f4926-30c7-45ca-a020-391f82b3b3a1
#:property DockerDefaultTargetOS Linux
#:property DockerComposeProjectPath ..\docker-compose.dcproj

using GenVue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Channels;

namespace GenVueIntegration
{
    public class GenVueOptions
    {
        public string Title { get; set; } = "GenVue Application";
        public int DefaultWidth { get; set; } = 1024;
        public int DefaultHeight { get; set; } = 768;
        public bool EnableHotReload { get; set; } = true;
        public string DefaultTheme { get; set; } = "light";
        public string ApiBaseUrl { get; set; } = "https://api.example.com";
        public string DefaultCulture { get; set; } = "en-US";
        public string PluginDirectory { get; set; } = "plugins";
        public bool EnablePerformanceMonitoring { get; set; } = true;
        public int MaxConcurrentRequests { get; set; } = 10;
        public bool EnableCrashReporting { get; set; } = true;
    }

    public interface IGenVueService
    {
        Task InitializeAsync();
        Task SetThemeAsync(string theme);
        Task RegisterComponentsAsync();
        Task ConfigureApiClientAsync(string baseUrl);
        Task SetCultureAsync(string culture);
        Task LoadPluginsAsync();
        Task StartPerformanceMonitoringAsync();
        Task ReportCrashAsync(Exception ex);
        Task OptimizePerformanceAsync();
    }

    public class GenVueService : IGenVueService
    {
        private readonly GenVueOptions _options;
        private readonly Channel<string> _messageChannel;
        private readonly ILogger<GenVueService> _logger;
        private readonly ObjectPool<StringBuilder> _stringBuilderPool;

        public GenVueService(IOptions<GenVueOptions> options, ILogger<GenVueService> logger)
        {
            _options = options.Value;
            _logger = logger;
            _messageChannel = Channel.CreateUnbounded<string>();
            _stringBuilderPool = new DefaultObjectPoolProvider().CreateStringBuilderPool();
        }

        public async Task InitializeAsync()
        {
            await GenVue.InitAsync(new GenVueConfig
            {
                Title = _options.Title,
                Width = _options.DefaultWidth,
                Height = _options.DefaultHeight,
                EnableHotReload = _options.EnableHotReload
            });
        }

        public async Task SetThemeAsync(string theme)
        {
            await GenVue.SetThemeAsync(theme);
        }

        public async Task RegisterComponentsAsync()
        {
            await GenVue.RegisterComponentsAsync();
        }

        public async Task ConfigureApiClientAsync(string baseUrl)
        {
            await GenVue.ConfigureApiClientAsync(baseUrl);
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGenVue(this IServiceCollection services, Action<GenVueOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddSingleton<IGenVueService, GenVueService>();
            return services;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddGenVue(options =>
            {
                options.Title = "My GenVue App";
                options.DefaultWidth = 1280;
                options.DefaultHeight = 800;
                options.EnableHotReload = true;
                options.DefaultTheme = "dark";
                options.ApiBaseUrl = "https://api.myapp.com";
            });

            var app = builder.Build();

            app.MapGet("/", () => "GenVue Integration Example");

            app.Run();
        }
    }
}