#:sdk Microsoft.NET.Sdk
#:package PdfReport.Core@2.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@7.0.0
#:package System.Threading.Channels@7.0.0
#:property LangVersion preview
#:property TargetFramework net8.0
#:property Nullable enable

using PdfReport.Core;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Channels;
using System.Collections.Concurrent;

namespace PdfReportIntegration
{
    public class PdfReportOptions
    {
        public string RedisConnectionString { get; set; } = string.Empty;
        public int BatchSize { get; set; } = 100;
        public int MaxConcurrentExports { get; set; } = 4;
        public string TemplatePath { get; set; } = string.Empty;
        public bool EnableWatermark { get; set; } = true;
        public bool EnableDistributedRendering { get; set; } = false;
        public bool EnableEncryption { get; set; } = false;
        public string EncryptionPassword { get; set; } = string.Empty;
        public bool EnableDigitalSignature { get; set; } = false;
        public string CertificatePath { get; set; } = string.Empty;
        public string CertificatePassword { get; set; } = string.Empty;
        public string DefaultCulture { get; set; } = "en-US";
    }

    public interface IPdfReportService
    {
        Task<byte[]> GeneratePdfAsync(object data, string templateName = "default");
        Task<IEnumerable<byte[]>> GeneratePdfsInBatchAsync(IEnumerable<object> data, string templateName = "default");
        Task CacheTemplateAsync(string templateName, byte[] templateBytes);
        Task<byte[]> GetTemplateAsync(string templateName);
        Task<byte[]> AddWatermarkAsync(byte[] pdfBytes, string watermarkText);
        Task<byte[]> EncryptPdfAsync(byte[] pdfBytes, string password);
        Task<byte[]> SignPdfAsync(byte[] pdfBytes, string certificatePath, string certificatePassword);
        Task SetCultureAsync(string cultureName);
    }

    public class PdfReportService : IPdfReportService, IDisposable
    {
        private readonly PdfReportOptions _options;
        private readonly IDistributedCache _cache;
        private readonly Channel<object> _batchChannel;
        private readonly ConcurrentDictionary<string, byte[]> _templateCache = new();
        private readonly CancellationTokenSource _cts = new();

        public PdfReportService(IOptions<PdfReportOptions> options, IDistributedCache cache)
        {
            _options = options.Value;
            _cache = cache;
            _batchChannel = Channel.CreateBounded<object>(new BoundedChannelOptions(_options.BatchSize * 2)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = false,
                SingleWriter = false
            });

            // Start background batch processing
            _ = Task.WhenAll(Enumerable.Range(0, _options.MaxConcurrentExports)
                .Select(_ => ProcessBatchAsync(_cts.Token)));
        }

        public async Task<byte[]> GeneratePdfAsync(object data, string templateName = "default")
        {
            var template = await GetTemplateAsync(templateName);
            var report = new PdfReportBuilder()
                .WithTemplate(template)
                .WithData(data)
                .WithCulture(_options.DefaultCulture)
                .Build();

            if (_options.EnableWatermark)
            {
                report = await AddWatermarkAsync(report, "Confidential");
            }

            if (_options.EnableEncryption && !string.IsNullOrEmpty(_options.EncryptionPassword))
            {
                report = await EncryptPdfAsync(report, _options.EncryptionPassword);
            }

            if (_options.EnableDigitalSignature && !string.IsNullOrEmpty(_options.CertificatePath))
            {
                report = await SignPdfAsync(report, _options.CertificatePath, _options.CertificatePassword);
            }

            return report;
        }

        public async Task<IEnumerable<byte[]>> GeneratePdfsInBatchAsync(IEnumerable<object> data, string templateName = "default")
        {
            foreach (var item in data)
            {
                await _batchChannel.Writer.WriteAsync(item);
            }

            // Implementation of batch processing would be in ProcessBatchAsync
            return Array.Empty<byte[]>();
        }

        public async Task CacheTemplateAsync(string templateName, byte[] templateBytes)
        {
            _templateCache[templateName] = templateBytes;
            if (_options.EnableDistributedRendering)
            {
                await _cache.SetAsync(templateName, templateBytes);
            }
        }

        public async Task<byte[]> GetTemplateAsync(string templateName)
        {
            if (_templateCache.TryGetValue(templateName, out var template))
            {
                return template;
            }

            if (_options.EnableDistributedRendering)
            {
                template = await _cache.GetAsync(templateName);
                if (template != null)
                {
                    _templateCache[templateName] = template;
                    return template;
                }
            }

            throw new FileNotFoundException($"Template {templateName} not found");
        }

        public async Task<byte[]> AddWatermarkAsync(byte[] pdfBytes, string watermarkText)
        {
            // Implementation of watermark would use PdfReport's watermark feature
            return pdfBytes;
        }

        public async Task<byte[]> EncryptPdfAsync(byte[] pdfBytes, string password)
        {
            // Implementation of PDF encryption
            return pdfBytes;
        }

        public async Task<byte[]> SignPdfAsync(byte[] pdfBytes, string certificatePath, string certificatePassword)
        {
            // Implementation of digital signature
            return pdfBytes;
        }

        public async Task SetCultureAsync(string cultureName)
        {
            // Implementation of culture setting
            await Task.CompletedTask;
        }

        private async Task ProcessBatchAsync(CancellationToken cancellationToken)
        {
            await foreach (var item in _batchChannel.Reader.ReadAllAsync(cancellationToken))
            {
                // Process batch items here
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPdfReportService(this IServiceCollection services, Action<PdfReportOptions> configureOptions)
        {
            services.Configure(configureOptions);
            
            if (configureOptions.Target.GetInvocationList().Any(x => 
                ((PdfReportOptions)x.DynamicInvoke()).EnableDistributedRendering))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configureOptions.Target.GetInvocationList()
                        .Select(x => ((PdfReportOptions)x.DynamicInvoke()).RedisConnectionString)
                        .FirstOrDefault();
                });
            }
            
            services.AddSingleton<IPdfReportService, PdfReportService>();
            return services;
        }
    }
}