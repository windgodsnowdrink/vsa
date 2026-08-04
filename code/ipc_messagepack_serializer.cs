#:sdk Microsoft.NET.Sdk.Web
#:package MessagePack@2.5.122
#:package Microsoft.IO.RecyclableMemoryStream@3.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.IO.Pipelines;
using MessagePack;
using Microsoft.IO;

// MessagePack序列化处理器
public class IpcMessagePackSerializer : IAsyncDisposable
{
    private readonly Pipe _pipe;  // 进程间通信管道
    private readonly RecyclableMemoryStreamManager _memoryManager;  // 内存管理器
    private readonly Task _processingTask;  // 后台处理任务
    private readonly CancellationTokenSource _cts = new();  // 取消令牌
    
    // 构造函数初始化组件
    public IpcMessagePackSerializer()
    {
        // 使用可回收内存流管理器，优化内存分配
        _memoryManager = new RecyclableMemoryStreamManager();
        
        // 配置管道选项
        _pipe = new Pipe(new PipeOptions(
            pool: _memoryManager,  // 使用内存池
            readerScheduler: PipeScheduler.Inline,  // 内联调度
            writerScheduler: PipeScheduler.Inline,
            pauseWriterThreshold: 1024 * 1024,  // 写入暂停阈值
            resumeWriterThreshold: 512 * 1024,  // 写入恢复阈值
            minimumSegmentSize: 4096));  // 最小段大小
            
        // 启动后台处理任务
        _processingTask = Task.Run(ProcessPipeAsync);
    }

    // 序列化方法
    public async ValueTask SerializeAsync<T>(T value)
    {
        // 从内存池获取流
        using var memoryStream = _memoryManager.GetStream();
        
        // 使用MessagePack序列化到内存流
        await MessagePackSerializer.SerializeAsync(memoryStream, value, cancellationToken: _cts.Token);
        
        // 重置流位置并写入管道
        memoryStream.Position = 0;
        await memoryStream.CopyToAsync(_pipe.Writer, _cts.Token);
        await _pipe.Writer.FlushAsync(_cts.Token);
    }

    // 反序列化方法
    public async ValueTask<T?> DeserializeAsync<T>()
    {
        // 从管道读取并反序列化
        var result = await MessagePackSerializer.DeserializeAsync<T>(
            _pipe.Reader.AsStream(), 
            cancellationToken: _cts.Token);
            
        return result;
    }

    // 管道处理后台任务
    private async Task ProcessPipeAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            var readResult = await _pipe.Reader.ReadAsync(_cts.Token);
            if (readResult.IsCompleted || readResult.IsCanceled)
                break;

            // 推进读取位置
            _pipe.Reader.AdvanceTo(readResult.Buffer.End);
        }
    }

    // 释放资源
    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();  // 取消操作
        await _processingTask;  // 等待任务完成
        _pipe.Reader.Complete();  // 完成读取
        _pipe.Writer.Complete();  // 完成写入
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder();

// 注册序列化服务
builder.Services.AddSingleton<IpcMessagePackSerializer>();

var app = builder.Build();

// 测试端点
app.MapGet("/", () => "MessagePack IPC Serializer Ready");
app.Run();