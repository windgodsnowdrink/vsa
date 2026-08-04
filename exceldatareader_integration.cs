#:sdk Microsoft.NET.Sdk.Web
#:package ExcelDataReader@3.6.0
#:package ExcelDataReader.DataSet@3.6.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using ExcelDataReader;
using Microsoft.Extensions.ObjectPool;

// 启动配置
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ExcelDataService>();

var app = builder.Build();
app.MapGet("/", () => "Excel Data Service");
app.Run();

[SkipLocalsInit]
public sealed class ExcelDataService : IAsyncDisposable
{
    private readonly Channel<ExcelRequest> _requestChannel;
    private readonly ObjectPool<MemoryStream> _streamPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly CancellationTokenSource _cts;

    public ExcelDataService()
    {
        _latencyOptimizer = new TailLatencyOptimizer();
        _cts = new CancellationTokenSource();
        
        _requestChannel = Channel.CreateBounded<ExcelRequest>(
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

    // Excel读取功能
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async IAsyncEnumerable<T> ReadExcelAsync<T>(string filePath) where T : class, new()
    {
        using var stream = new FileStream(filePath, FileMode.Open);
        using var reader = ExcelReaderFactory.CreateReader(stream);
        do
        {
            while (reader.Read())
            {
                yield return MapRowToModel<T>(reader);
            }
        } while (reader.NextResult());
    }

    // Excel写入功能
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task WriteExcelAsync<T>(IEnumerable<T> data, string filePath)
    {
        var request = new ExcelRequest(data, filePath);
        await _requestChannel.Writer.WriteAsync(request, _cts.Token);
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
                await WriteToExcelAsync(stream, request.Data);
                await WriteToFileAsync(stream, request.FilePath);
            }
            finally
            {
                _streamPool.Return(stream);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task WriteToExcelAsync<T>(MemoryStream stream, IEnumerable<T> data)
    {
        using var writer = ExcelDataReader.ExcelWriterFactory.CreateWriter(stream);
        writer.WriteDataSet(data.ToDataSet());
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private T MapRowToModel<T>(IExcelDataReader reader) where T : class, new()
    {
        var model = new T();
        var properties = typeof(T).GetProperties();
        
        for (int i = 0; i < properties.Length; i++)
        {
            var value = reader.GetValue(i);
            properties[i].SetValue(model, Convert.ChangeType(value, properties[i].PropertyType));
        }
        
        return model;
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