#:sdk Microsoft.NET.Sdk.Web
#:package StreamJsonRpc@2.16.33
#:package System.Text.Json@8.0.0
#:package MemoryPack@1.9.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using System.Text.Json;
using MemoryPack;
using Microsoft.Extensions.ObjectPool;

// 1. 高性能序列化包装器
[MemoryPackable]
public partial class RpcMessage
{
    public string Method { get; set; }
    public ReadOnlyMemory<byte> Payload { get; set; }

    [MemoryPackIgnore]
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public byte[] Serialize()
    {
        using var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        JsonSerializer.Serialize(writer, this, _jsonOptions);
        return buffer.WrittenMemory.ToArray();
    }

    public static RpcMessage Deserialize(ReadOnlySpan<byte> data)
    {
        return JsonSerializer.Deserialize<RpcMessage>(data, _jsonOptions)!;
    }

    public byte[] MemoryPackSerialize()
    {
        return MemoryPackSerializer.Serialize(this);
    }

    public static RpcMessage MemoryPackDeserialize(ReadOnlyMemory<byte> data)
    {
        return MemoryPackSerializer.Deserialize<RpcMessage>(data.Span)!;
    }
}

// 2. 连接池实现
public class RpcConnectionPool : IAsyncDisposable
{
    private readonly ObjectPool<JsonRpc> _pool;
    private readonly Channel<JsonRpc> _idleConnections;
    private readonly Func<Stream> _streamFactory;

    public RpcConnectionPool(Func<Stream> streamFactory, int maxConnections = 10)
    {
        _streamFactory = streamFactory;
        _idleConnections = Channel.CreateBounded<JsonRpc>(maxConnections);

        _pool = new DefaultObjectPool<JsonRpc>(new RpcPooledObjectPolicy(streamFactory), maxConnections);

        // 预热连接池
        for (int i = 0; i < maxConnections; i++)
        {
            var rpc = _pool.Get();
            _idleConnections.Writer.TryWrite(rpc);
        }
    }

    public async ValueTask<JsonRpc> RentAsync(CancellationToken cancellationToken = default)
    {
        if (_idleConnections.Reader.TryRead(out var rpc))
            return rpc;

        return _pool.Get();
    }

    public ValueTask ReturnAsync(JsonRpc rpc)
    {
        if (_idleConnections.Writer.TryWrite(rpc))
            return ValueTask.CompletedTask;

        _pool.Return(rpc);
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        _idleConnections.Writer.Complete();
        await foreach (var rpc in _idleConnections.Reader.ReadAllAsync())
        {
            await rpc.DisposeAsync();
        }
    }

    private class RpcPooledObjectPolicy : IPooledObjectPolicy<JsonRpc>
    {
        private readonly Func<Stream> _streamFactory;

        public RpcPooledObjectPolicy(Func<Stream> streamFactory)
        {
            _streamFactory = streamFactory;
        }

        public JsonRpc Create()
        {
            var stream = _streamFactory();
            return JsonRpc.Attach(new HeaderDelimitedMessageHandler(stream, stream));
        }

        public bool Return(JsonRpc obj)
        {
            return !obj.IsDisposed;
        }
    }
}