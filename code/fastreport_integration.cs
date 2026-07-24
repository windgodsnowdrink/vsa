#:sdk Microsoft.NET.Sdk.Web
#:package FastReport.Core@2024.1.0
#:package FastReport.OpenSource@2024.1.0
#:package FastReport.DataVisualization@2024.1.0
#:package Microsoft.Extensions.Hosting@8.0.0
#:package Microsoft.Extensions.Options@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using FastReport;
using FastReport.Export.Pdf;
using FastReport.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Channels;
using System.Threading.Tasks;

public class FastReportOptions
{
    public string TemplatesPath { get; set; } = "Reports";
    public bool EnableCache { get; set; } = true;
    public int CacheSize { get; set; } = 100;
}

public interface IReportService
{
    Task<byte[]> GeneratePdfReportAsync(string templateName, object dataSource);
    Task<MemoryStream> GenerateExcelReportAsync(string templateName, object dataSource);
    IAsyncEnumerable<byte[]> ContinuousExportAsync(string templateName, IAsyncEnumerable<object> dataStream);
}

public class ReportService : IReportService, IDisposable
{
    private readonly FastReportOptions _options;
    private readonly Channel<Report> _reportPool;
    private bool _disposed;

    public ReportService(IOptions<FastReportOptions> options)
    {
        _options = options.Value;
        RegisteredObjects.AddConnection(typeof(JsonDataSource));
        
        _reportPool = Channel.CreateBounded<Report>(new BoundedChannelOptions(_options.CacheSize)
        {
            FullMode = BoundedChannelFullMode.Wait
        });

        if (_options.EnableCache)
        {
            for (int i = 0; i < _options.CacheSize; i++)
            {
                var report = new Report();
                _reportPool.Writer.TryWrite(report);
            }
        }
    }

    public async Task<byte[]> GeneratePdfReportAsync(string templateName, object dataSource)
    {
        var report = await GetReportAsync(templateName);
        try
        {
            report.RegisterData(dataSource, "DataSource");
            report.Prepare();

            using var pdfExport = new PDFExport();
            using var ms = new MemoryStream();
            report.Export(pdfExport, ms);
            return ms.ToArray();
        }
        finally
        {
            ReturnReport(report);
        }
    }

    public async Task<MemoryStream> GenerateExcelReportAsync(string templateName, object dataSource)
    {
        var report = await GetReportAsync(templateName);
        try
        {
            report.RegisterData(dataSource, "DataSource");
            report.Prepare();

            var ms = new MemoryStream();
            report.Export(new FastReport.Export.OoXML.Excel2007Export(), ms);
            return ms;
        }
        finally
        {
            ReturnReport(report);
        }
    }

    public async IAsyncEnumerable<byte[]> ContinuousExportAsync(
        string templateName, 
        IAsyncEnumerable<object> dataStream)
    {
        await foreach (var data in dataStream)
        {
            yield return await GeneratePdfReportAsync(templateName, data);
        }
    }

    private async Task<Report> GetReportAsync(string templateName)
    {
        if (_options.EnableCache && _reportPool.Reader.TryRead(out var cachedReport))
        {
            return cachedReport;
        }

        var report = new Report();
        var templatePath = Path.Combine(_options.TemplatesPath, $"{templateName}.frx");
        report.Load(templatePath);
        return report;
    }

    private void ReturnReport(Report report)
    {
        if (_options.EnableCache)
        {
            report.Dispose();
            report = new Report();
            _reportPool.Writer.TryWrite(report);
        }
        else
        {
            report.Dispose();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        while (_reportPool.Reader.TryRead(out var report))
        {
            report.Dispose();
        }
        
        _disposed = true;
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFastReport(this IServiceCollection services, Action<FastReportOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<IReportService, ReportService>();
        return services;
    }
}

// 示例用法
public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddFastReport(options =>
                {
                    options.TemplatesPath = "Templates";
                    options.EnableCache = true;
                    options.CacheSize = 50;
                });
            })
            .Build();

        var reportService = host.Services.GetRequiredService<IReportService>();
        var pdfBytes = await reportService.GeneratePdfReportAsync("Invoice", new { Id = 1, Name = "Test" });
        await File.WriteAllBytesAsync("Invoice.pdf", pdfBytes);

        await host.RunAsync();
    }
}