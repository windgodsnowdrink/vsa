#:sdk Microsoft.NET.Sdk.Web
#:package llcom@1.0.0
#:package System.IO.Pipelines@7.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable

using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Channels;
using System.Threading.Tasks;
using llcom;

// 零拷贝通讯管道
public class ZeroCopyCommunicationPipe : IDisposable
{
    private readonly Pipe _pipe = new Pipe();
    private readonly Channel<ReadOnlyMemory<byte>> _messageChannel = Channel.CreateUnbounded<ReadOnlyMemory<byte>>();
    
    // ... existing code ...
    
    public async ValueTask WriteAsync(ReadOnlyMemory<byte> data)
    {
        await _pipe.Writer.WriteAsync(data);
        await _messageChannel.Writer.WriteAsync(data);
    }
    
    // ... existing code ...
}

// LOIC协议实现
public class LoicProtocolHandler
{
    private readonly ZeroCopyCommunicationPipe _pipe;
    
    public LoicProtocolHandler(ZeroCopyCommunicationPipe pipe)
    {
        _pipe = pipe;
    }
    
    // ... existing code ...
    
    public async Task ProcessMessageAsync(ReadOnlyMemory<byte> message)
    {
        // 使用Span<T>进行零拷贝处理
        var span = message.Span;
        // 协议解析逻辑...
        await _pipe.WriteAsync(message);
    }
}

// DI集成
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLoicCommunication(this IServiceCollection services)
    {
        services.AddSingleton<ZeroCopyCommunicationPipe>();
        services.AddSingleton<LoicProtocolHandler>();
        return services;
    }
}

// WPF上位机集成
public class MainViewModel : IDisposable
{
    private readonly LoicProtocolHandler _protocolHandler;
    
    public MainViewModel(LoicProtocolHandler protocolHandler)
    {
        _protocolHandler = protocolHandler;
    }
    
    // ... existing code ...
    
    public async Task SendCommandAsync(ReadOnlyMemory<byte> command)
    {
        await _protocolHandler.ProcessMessageAsync(command);
    }
}