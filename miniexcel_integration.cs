#:sdk Microsoft.NET.Sdk.Web
#:package MiniExcel@1.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using MiniExcelLibs;
using Microsoft.Extensions.ObjectPool;

[SkipLocalsInit]
public sealed class ExcelService : IAsyncDisposable
{
    private readonly Channel<ExcelRequest> _requestChannel;
    private readonly ObjectPool<MemoryStream> _streamPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly CancellationTokenSource _cts;

    public ExcelService()
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

    // 写入Excel增强版
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async ValueTask GenerateExcelAsync<T>(IEnumerable<T> data, string filePath)
    {
        var request = new ExcelRequest(data, filePath);
        await _requestChannel.Writer.WriteAsync(request, _cts.Token);
    }

    // 新增Excel读取功能
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async IAsyncEnumerable<T> ReadExcelAsync<T>(string filePath) where T : class, new()
    {
        using var stream = new FileStream(filePath, FileMode.Open);
        await foreach (var item in MiniExcel.QueryAsync<T>(stream))
        {
            yield return item;
        }
    }

    // 新增Excel处理功能
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ProcessExcelAsync<T>(string sourcePath, string targetPath, Func<T, T> processor)
    {
        var items = ReadExcelAsync<T>(sourcePath);
        await GenerateExcelAsync(items.Select(processor), targetPath);
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
                MiniExcel.SaveAs(stream, request.Data);
                await WriteToFileAsync(stream, request.FilePath);
            }
            finally
            {
                _streamPool.Return(stream);
            }
        }
    }

    // 优化后的WriteToFileAsync实现
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
builder.Services.AddSingleton<ExcelService>();

var app = builder.Build();
app.MapGet("/", () => "Excel Processing Service");
app.Run();