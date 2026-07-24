//#:sdk Microsoft.NET.Sdk
//#:package Util.Core@latest
//#:property LangVersion preview
//#:property TargetFramework net11.0
//#:property Nullable enable
//#:property ImplicitUsings enable

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Util;

namespace UtilIntegration
{
    public class UtilOptions
    {
        public string DefaultConnectionString { get; set; }
        public int MaxRetryCount { get; set; } = 3;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    }

    public interface IUtilService
    {
        Task<string> GetDataAsync(string key);
        Task<bool> SaveDataAsync(string key, string value);
        Task<int> GetRetryCountAsync();
    }

    public class UtilService : IUtilService
    {
        private readonly UtilOptions _options;
        private readonly ILogger<UtilService> _logger;

        public UtilService(IOptions<UtilOptions> options, ILogger<UtilService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task<string> GetDataAsync(string key)
        {
            _logger.LogInformation($"Getting data for key: {key}");
            // 实现Util库的数据获取逻辑
            return await Task.FromResult($"Data for {key}");
        }

        public async Task<bool> SaveDataAsync(string key, string value)
        {
            _logger.LogInformation($"Saving data for key: {key}");
            // 实现Util库的数据保存逻辑
            return await Task.FromResult(true);
        }

        public async Task<int> GetRetryCountAsync()
        {
            return await Task.FromResult(_options.MaxRetryCount);
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUtilServices(this IServiceCollection services, Action<UtilOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddSingleton<IUtilService, UtilService>();
            return services;
        }
    }

    public static class ExampleUsage
    {
        public static async Task Demo()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddUtilServices(options =>
            {
                options.DefaultConnectionString = "Server=.;Database=Test;Trusted_Connection=True;";
                options.MaxRetryCount = 5;
                options.Timeout = TimeSpan.FromMinutes(1);
            });

            var provider = services.BuildServiceProvider();
            var utilService = provider.GetRequiredService<IUtilService>();

            await utilService.SaveDataAsync("test", "value");
            var data = await utilService.GetDataAsync("test");
            var retryCount = await utilService.GetRetryCountAsync();
        }
    }
}