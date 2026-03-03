using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text;

namespace PromptIntegration
{
    public class PromptOptions
{
    public string DefaultSystemPrompt { get; set; } = "You are a helpful AI assistant";
    public int MaxPromptLength { get; set; } = 4000;
    public bool EnableJsonOutput { get; set; } = false;
    public string OutputSchema { get; set; }
    public string TemplateDirectory { get; set; }
    public string MultimodalModelPath { get; set; }
    public int BatchSize { get; set; } = 10;
    public string CacheConnectionString { get; set; }
    public bool EnableStreaming { get; set; } = true;
    {
        public string DefaultSystemPrompt { get; set; } = "You are a helpful AI assistant.";
        public int MaxPromptLength { get; set; } = 4000;
        public bool EnableJsonOutput { get; set; } = false;
        public string OutputSchema { get; set; } = string.Empty;
        public string TemplatesDirectory { get; set; } = "./prompt_templates";
    }

    public interface IPromptService
{
    Task<string> RenderTemplateAsync(string templateName, object model);
    Task<string> GenerateWithPromptAsync(string prompt);
    Task<string> GenerateWithContextAsync(string context, string prompt);
    IAsyncEnumerable<string> StreamWithPromptAsync(string prompt);
    Task<string> AnalyzeImageAsync(byte[] imageData, string prompt);
    Task<string> TranscribeAudioAsync(byte[] audioData, string prompt);
    Task<IReadOnlyList<string>> BatchGenerateAsync(IEnumerable<string> prompts);
    Task BatchIndexDocumentsAsync(IEnumerable<string> documents);
    Task UnloadModelAsync();
    Task ClearCacheAsync();
    {
        string RenderTemplate(string templateName, IDictionary<string, object> variables);
        Task<string> GenerateWithPromptAsync(string prompt, IDictionary<string, object>? variables = null);
        Task<string> GenerateWithContextAsync(string prompt, string context, IDictionary<string, object>? variables = null);
        void RegisterTemplate(string name, string template);
    }

    public class PromptService : IPromptService
{
    private readonly PromptOptions _options;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PromptService> _logger;
    private readonly Channel<string> _batchChannel;
    private readonly ConcurrentDictionary<string, string> _templates = new();

    public PromptService(PromptOptions options, IMemoryCache cache, ILogger<PromptService> logger)
    {
        _options = options;
        _cache = cache;
        _logger = logger;
        _batchChannel = Channel.CreateBounded<string>(_options.BatchSize * 2);
        InitializeBatchProcessing();
        LoadTemplatesFromDirectory();
    }
    {
        private readonly PromptOptions _options;
        private readonly ConcurrentDictionary<string, string> _templates;
        private readonly ILogger<PromptService> _logger;

        public PromptService(
            IOptions<PromptOptions> options,
            ILogger<PromptService> logger)
        {
            _options = options.Value;
            _logger = logger;
            _templates = new ConcurrentDictionary<string, string>();
            
            // Load templates from directory
            if (Directory.Exists(_options.TemplatesDirectory))
            {
                foreach (var file in Directory.GetFiles(_options.TemplatesDirectory, "*.txt"))
                {
                    var templateName = Path.GetFileNameWithoutExtension(file);
                    var templateContent = File.ReadAllText(file);
                    _templates.TryAdd(templateName, templateContent);
                }
            }
        }

        public string RenderTemplate(string templateName, IDictionary<string, object> variables)
        {
            if (!_templates.TryGetValue(templateName, out var template))
                throw new KeyNotFoundException($"Template '{templateName}' not found");

            var sb = new StringBuilder(template);
            foreach (var kvp in variables)
            {
                sb.Replace($"{{{kvp.Key}}}", kvp.Value?.ToString() ?? string.Empty);
            }
            
            return sb.ToString();
        }

        public async Task<string> GenerateWithPromptAsync(string prompt, IDictionary<string, object>? variables = null)
        {
            var renderedPrompt = variables != null 
                ? RenderTemplate(prompt, variables) 
                : prompt;
                
            // Call LLM API with the rendered prompt
            return await Task.FromResult("Generated response based on prompt");
        }

        public async Task<string> GenerateWithContextAsync(string prompt, string context, IDictionary<string, object>? variables = null)
        {
            var fullPrompt = $"Context: {context}\n\nPrompt: {prompt}";
            return await GenerateWithPromptAsync(fullPrompt, variables);
        }

        public void RegisterTemplate(string name, string template)
        {
            _templates.AddOrUpdate(name, template, (_, _) => template);
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPromptService(this IServiceCollection services, Action<PromptOptions> configureOptions)
{
    services.AddOptions<PromptOptions>().Configure(configureOptions);
    services.AddMemoryCache();
    services.AddSingleton<IPromptService, PromptService>();
    services.AddHostedService<PromptBackgroundService>();
    
    // 分布式缓存配置
    if (!string.IsNullOrEmpty(configureOptions.CacheConnectionString))
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configureOptions.CacheConnectionString;
        });
    }
    
    return services;(
            this IServiceCollection services,
            Action<PromptOptions> configure)
        {
            services.Configure(configure);
            services.AddSingleton<IPromptService, PromptService>();
            
            // Ensure templates directory exists
            var options = new PromptOptions();
            configure(options);
            
            if (!Directory.Exists(options.TemplatesDirectory))
            {
                Directory.CreateDirectory(options.TemplatesDirectory);
            }
            
            return services;
        }
    }
}

// Example usage:
// builder.Services.AddPromptService(options =>
// {
//     options.DefaultSystemPrompt = "You are an expert in .NET development.";
//     options.TemplatesDirectory = "./custom_prompts";
// });