#:sdk Microsoft.NET.Sdk.Web
#:package Magicodes.IE.Core@2.6.0
#:package Magicodes.IE.Excel@2.6.0
#:package Magicodes.IE.Pdf@2.6.0
#:package Magicodes.IE.Word@2.6.0
#:package Magicodes.IE.Html@2.6.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using Magicodes.IE.Core;
using Magicodes.IE.Excel;
using Magicodes.IE.Pdf;
using Magicodes.IE.Word;
using Magicodes.IE.Html;
using Microsoft.Extensions.ObjectPool;

[SkipLocalsInit]
public sealed class DocumentService : IAsyncDisposable
{
    private readonly Channel<DocumentRequest> _requestChannel;
    private readonly ObjectPool<MemoryStream> _streamPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly CancellationTokenSource _cts;
    private readonly IImporter _importer;
    private readonly IExporter _exporter;

    public DocumentService(IImporter importer, IExporter exporter)
    {
        _latencyOptimizer = new TailLatencyOptimizer();
        _cts = new CancellationTokenSource();
        _importer = importer;
        _exporter = exporter;
        
        _requestChannel = Channel.CreateBounded<DocumentRequest>(
            new BoundedChannelOptions(10_000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
        
        _streamPool = new DefaultObjectPool<MemoryStream>(
            new MemoryStreamPooledPolicy(), 
            Environment.ProcessorCount * 2);
        
        _ = Task.Run(ProcessRequestsAsync);
    }

    // Excel导出
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ExportExcelAsync<T>(IEnumerable<T> data, string filePath)
    {
        var request = new DocumentRequest(data, filePath, DocumentType.Excel);
        await _requestChannel.Writer.WriteAsync(request, _cts.Token);
    }

    // Word导出
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ExportWordAsync<T>(IEnumerable<T> data, string filePath)
    {
        var request = new DocumentRequest(data, filePath, DocumentType.Word);
        await _requestChannel.Writer.WriteAsync(request, _cts.Token);
    }

    // PDF导出
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ExportPdfAsync<T>(IEnumerable<T> data, string filePath)
    {
        var request = new DocumentRequest(data, filePath, DocumentType.Pdf);
        await _requestChannel.Writer.WriteAsync(request, _cts.Token);
    }

    // HTML导出
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ExportHtmlAsync<T>(IEnumerable<T> data, string filePath)
    {
        var request = new DocumentRequest(data, filePath, DocumentType.Html);
        await _requestChannel.Writer.WriteAsync(request, _cts.Token);
    }

    // Excel导入
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async IAsyncEnumerable<T> ImportExcelAsync<T>(string filePath) where T : class, new()
    {
        using var stream = new FileStream(filePath, FileMode.Open);
        var result = await _importer.Import<T>(stream);
        foreach (var item in result.Data)
        {
            yield return item;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessRequestsAsync()
    {
        await foreach (var request in _requestChannel.Reader.ReadAllAsync(_cts.Token))
        {
            using var latencyToken = _latencyOptimizer.BeginOperation();
            var stream = _streamPool.Get();
            try
            {
                switch (request.Type)
                {
                    case DocumentType.Excel:
                        await _exporter.Export(stream, request.Data);
                        break;
                    case DocumentType.Word:
                        await _exporter.ExportWord(stream, request.Data);
                        break;
                    case DocumentType.Pdf:
                        await _exporter.ExportPdf(stream, request.Data);
                        break;
                    case DocumentType.Html:
                        await _exporter.ExportHtml(stream, request.Data);
                        break;
                }
                
                await WriteToFileAsync(stream, request.FilePath);
            }
            finally
            {
                _streamPool.Return(stream);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task WriteToFileAsync(MemoryStream stream, string filePath)
    {
        using var fileStream = new FileStream(
            filePath, 
            FileMode.Create, 
            FileAccess.Write, 
            FileShare.None, 
            bufferSize: 4096, 
            useAsync: true);
        
        await stream.CopyToAsync(fileStream);
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _requestChannel.Writer.Complete();
        await _requestChannel.Reader.Completion;
    }
}

internal enum DocumentType { Excel, Word, Pdf, Html }
internal record DocumentRequest(IEnumerable<object> Data, string FilePath, DocumentType Type);

// 启动配置
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMagicodesIE();
builder.Services.AddSingleton<DocumentService>();

var app = builder.Build();
app.MapGet("/", () => "Document Processing Service");
app.Run();