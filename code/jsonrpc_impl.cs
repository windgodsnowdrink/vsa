#:sdk Microsoft.NET.Sdk
#:package StreamJsonRpc@2.16.33
#:package Microsoft.VisualStudio.Threading@17.6.40
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.IO.Pipelines;
using System.Threading.Channels;
using StreamJsonRpc;
using Microsoft.VisualStudio.Threading;

// 1. 定义RPC接口
public interface IRpcService
{
    Task<string> ProcessAsync(string input);
    Task NotifyAsync(string message);
}

// 2. 实现高性能RPC服务端
public class RpcServer : IRpcService, IAsyncDisposable
{
    private readonly JsonRpc _rpc;
    private readonly Channel<string> _notificationChannel;
    private readonly ObjectPool<Memory<byte>> _memoryPool;

    public RpcServer(Stream stream)
    {
        // 使用Pipe优化IO性能
        var pipe = new Pipe();
        _memoryPool = new DefaultObjectPool<Memory<byte>>(
            new DefaultPooledObjectPolicy<Memory<byte>>(), 
            1024 * 1024); // 1MB内存池

        // 配置JsonRpc
        _rpc = JsonRpc.Attach(pipe.Reader.AsStream(), pipe.Writer.AsStream(), this, new JsonRpcOptions
        {
            MessageHandler = new HeaderDelimitedMessageHandler(pipe.Reader.AsStream(), pipe.Writer.AsStream()),
            CancellationStrategy = new CancellationStrategy(),
            ExceptionStrategy = ExceptionProcessing.CommonErrorData
        });

        // 通知通道
        _notificationChannel = Channel.CreateBounded<string>(new BoundedChannelOptions(1000)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait
        });

        _ = ProcessNotificationsAsync();
    }

    public async Task<string> ProcessAsync(string input)
    {
        // 使用内存池优化
        var memory = _memoryPool.Get();
        try
        {
            // 高性能处理逻辑
            return $"Processed: {input}";
        }
        finally
        {
            _memoryPool.Return(memory);
        }
    }

    public Task NotifyAsync(string message)
    {
        return _notificationChannel.Writer.WriteAsync(message).AsTask();
    }

    private async Task ProcessNotificationsAsync()
    {
        await foreach (var message in _notificationChannel.Reader.ReadAllAsync())
        {
            await _rpc.NotifyAsync("OnNotification", message);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _notificationChannel.Writer.Complete();
        await _rpc.DisposeAsync();
    }
}

// 3. 实现高性能RPC客户端
public class RpcClient : IAsyncDisposable
{
    private readonly JsonRpc _rpc;
    private readonly IRpcService _proxy;

    public RpcClient(Stream stream)
    {
        var pipe = new Pipe();
        _rpc = JsonRpc.Attach(pipe.Reader.AsStream(), pipe.Writer.AsStream(), new JsonRpcOptions
        {
            MessageHandler = new HeaderDelimitedMessageHandler(pipe.Reader.AsStream(), pipe.Writer.AsStream()),
            CancellationStrategy = new CancellationStrategy()
        });

        _proxy = _rpc.Attach<IRpcService>();
    }

    public Task<string> ProcessAsync(string input) => _proxy.ProcessAsync(input);
    public Task NotifyAsync(string message) => _proxy.NotifyAsync(message);

    public async ValueTask DisposeAsync()
    {
        await _rpc.DisposeAsync();
    }
}

// 4. 使用示例
public static class RpcDemo
{
    public static async Task RunAsync()
    {
        // 创建双向通信流
        var serverPipe = new Pipe();
        var clientPipe = new Pipe();

        // 启动服务端
        var server = new RpcServer(new DuplexStream(
            serverPipe.Reader.AsStream(), 
            clientPipe.Writer.AsStream()));

        // 启动客户端
        var client = new RpcClient(new DuplexStream(
            clientPipe.Reader.AsStream(),
            serverPipe.Writer.AsStream()));

        // 执行RPC调用
        var result = await client.ProcessAsync("test");
        Console.WriteLine(result);

        // 发送通知
        await client.NotifyAsync("Hello from client");

        // 清理
        await server.DisposeAsync();
        await client.DisposeAsync();
    }
}

// 5. 双向流包装器
public class DuplexStream : Stream
{
    private readonly Stream _readStream;
    private readonly Stream _writeStream;

    public DuplexStream(Stream readStream, Stream writeStream)
    {
        _readStream = readStream;
        _writeStream = writeStream;
    }

    public override bool CanRead => true;
    public override bool CanWrite => true;
    // ... 其他Stream成员实现 ...
}