#:sdk Microsoft.NET.Sdk.Web
#:package CsvHelper@30.0.1
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.ObjectPool;

[SkipLocalsInit]
public sealed class CsvService : IAsyncDisposable
{
    private readonly Channel<CsvRequest> _requestChannel;
    private readonly ObjectPool<MemoryStream> _streamPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly CancellationTokenSource _cts;

    public CsvService()
    {
        _latencyOptimizer = new TailLatencyOptimizer();
        _cts = new CancellationTokenSource();
        
        _requestChannel = Channel.CreateBounded<CsvRequest>(
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

    // CSV导出功能
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ExportCsvAsync<T>(IEnumerable<T> data, string filePath)
    {
        var request = new CsvRequest(data, filePath);
        await _requestChannel.Writer.WriteAsync(request, _cts.Token);
    }

    // CSV导入功能
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async IAsyncEnumerable<T> ImportCsvAsync<T>(string filePath) where T : class
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLowerInvariant()
        });
        
        await foreach (var record in csv.GetRecordsAsync<T>())
        {
            yield return record;
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
                await WriteToCsvAsync(stream, request.Data);
                await WriteToFileAsync(stream, request.FilePath);
            }
            finally
            {
                _streamPool.Return(stream);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task WriteToCsvAsync<T>(MemoryStream stream, IEnumerable<T> data)
    {
        using var writer = new StreamWriter(stream, leaveOpen: true);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(data);
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

// 启动配置
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<CsvService>();

var app = builder.Build();
app.MapGet("/", () => "CSV Processing Service");
app.Run();