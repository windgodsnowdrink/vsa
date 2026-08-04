#:sdk Microsoft.NET.Sdk.Web
#:package P2P.NET@2.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable

using System;
using System.Buffers;
using System.Net;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using P2P.NET;

public interface IP2PService
{
    Task SendAsync(IPEndPoint endpoint, ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);
    IAsyncEnumerable<ReadOnlyMemory<byte>> ReceiveAsync(CancellationToken cancellationToken = default);
}

public class P2PService : IP2PService, IDisposable
{
    private readonly ObjectPool<P2PConnection> _connectionPool;
    private readonly ThreadLocal<Memory<byte>> _threadLocalBuffer;
    private readonly Channel<ReceivedMessage> _messageChannel;
    
    public P2PService(ObjectPool<P2PConnection> connectionPool)
    {
        _connectionPool = connectionPool;
        _threadLocalBuffer = new ThreadLocal<Memory<byte>>(() => 
            new byte[8192].AsMemory().Slice(0, 8192));
        
        _messageChannel = Channel.CreateUnbounded<ReceivedMessage>();
    }

    public async Task SendAsync(IPEndPoint endpoint, ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var connection = _connectionPool.Get();
        try
        {
            await connection.SendAsync(endpoint, data, cancellationToken);
        }
        finally
        {
            _connectionPool.Return(connection);
        }
    }

    public async IAsyncEnumerable<ReadOnlyMemory<byte>> ReceiveAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var message in _messageChannel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return message.Data;
        }
    }

    private async Task StartReceivingAsync(CancellationToken cancellationToken)
    {
        var connection = _connectionPool.Get();
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var buffer = _threadLocalBuffer.Value;
                var result = await connection.ReceiveAsync(buffer, cancellationToken);
                
                if (result.Received > 0)
                {
                    await _messageChannel.Writer.WriteAsync(
                        new ReceivedMessage(result.RemoteEndPoint, buffer.Slice(0, result.Received)), 
                        cancellationToken);
                }
            }
        }
        finally
        {
            _connectionPool.Return(connection);
        }
    }

    public void Dispose()
    {
        _threadLocalBuffer.Dispose();
        _messageChannel.Writer.Complete();
    }

    private record ReceivedMessage(IPEndPoint Endpoint, ReadOnlyMemory<byte> Data);
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddP2PServices(this IServiceCollection services)
    {
        services.AddSingleton<ObjectPool<P2PConnection>>(sp =>
        {
            var policy = new DefaultPooledObjectPolicy<P2PConnection>();
            return new DefaultObjectPool<P2PConnection>(policy, Environment.ProcessorCount * 2);
        });

        services.AddSingleton<IP2PService, P2PService>();
        return services;
    }
}