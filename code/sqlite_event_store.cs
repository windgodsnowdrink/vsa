#:sdk Microsoft.NET.Sdk
#:package Microsoft.Data.Sqlite@8.0.0
#:package Disruptor-net@3.4.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package Dapper@2.1.28
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Disruptor;
using Disruptor.Dsl;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.ObjectPool;
using Dapper;

// 1. 增强的事件模型（支持分片和校验）,Cache Line对齐的事件模型
[StructLayout(LayoutKind.Explicit, Size = 64)]
public struct SqliteEvent
{
    [FieldOffset(0)] public long Id;
    [FieldOffset(8)] public long Timestamp;
    [FieldOffset(16)] public EventType Type;
    [FieldOffset(20)] public int PayloadSize;
    [FieldOffset(24)] public int ExtendedPayloadId;
    [FieldOffset(28)] public ushort ChunkIndex;
    [FieldOffset(30)] public ushort TotalChunks;
    [FieldOffset(32)] public uint Checksum;
    [FieldOffset(36)] private fixed byte _payload[28];

    public Span<byte> Payload => GetPayloadSpan();
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe Span<byte> GetPayloadSpan()
    {
        fixed (byte* ptr = _payload)
        {
            return new Span<byte>(ptr, PayloadSize);
        }
    }

    // 新增：计算CRC32校验和
    public uint ComputeChecksum()
    {
        // ... existing checksum computation ...
    }
}

// 2. 分布式事务支持
public class DistributedTransactionCoordinator
{
    private readonly SqliteConnection _connection;
    private readonly Channel<TransactionEvent> _transactionChannel;

    public DistributedTransactionCoordinator(SqliteConnection connection)
    {
        _connection = connection;
        _transactionChannel = Channel.CreateBounded<TransactionEvent>(1000);
    }

    public async Task BeginTransactionAsync(string transactionId)
    {
        await _connection.ExecuteAsync(
            "INSERT INTO DistributedTransactions VALUES (@Id, 'Pending', @Timestamp)",
            new { Id = transactionId, Timestamp = DateTime.UtcNow.Ticks });
    }

    public async Task CommitTransactionAsync(string transactionId)
    {
        // 两阶段提交实现
        await _connection.ExecuteAsync(
            "UPDATE DistributedTransactions SET Status = 'Prepared' WHERE Id = @Id",
            new { Id = transactionId });

        await _connection.ExecuteAsync(
            "UPDATE DistributedTransactions SET Status = 'Committed' WHERE Id = @Id",
            new { Id = transactionId });
    }
}

// 3. 增强的事件存储服务（支持事件溯源）
public sealed class SqliteEventStore : IAsyncDisposable
{
    private readonly Disruptor<SqliteEvent> _disruptor;
    private readonly RingBuffer<SqliteEvent> _ringBuffer;
    private readonly SqliteConnection _connection;
    private readonly ObjectPool<SqliteEvent> _eventPool;
    private readonly Channel<SqliteEvent> _overflowChannel;
    private readonly Task _processingTask;
    private readonly CancellationTokenSource _cts = new();

    public SqliteEventStore(string connectionString, int bufferSize = 1024 * 1024)
    {
        _connection = new SqliteConnection(connectionString);
        _connection.Open();
        
        // 优化SQLite性能设置
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            PRAGMA journal_mode=WAL;
            PRAGMA synchronous=NORMAL;
            PRAGMA cache_size=-10000;
            PRAGMA temp_store=MEMORY;
            PRAGMA mmap_size=268435456;
        ";
        cmd.ExecuteNonQuery();

        _eventPool = new DefaultObjectPool<SqliteEvent>(
            new SqliteEventPoolPolicy(), 
            Environment.ProcessorCount * 2);
            
        _disruptor = new Disruptor<SqliteEvent>(
            () => _eventPool.Get(),
            bufferSize,
            TaskScheduler.Default,
            ProducerType.Multi,
            new BlockingWaitStrategy());
            
        _disruptor.HandleEventsWith(new SqliteEventHandler(_connection));
        _ringBuffer = _disruptor.Start();
        
        _overflowChannel = Channel.CreateBounded<SqliteEvent>(10000);
        _processingTask = Task.Run(ProcessOverflowEventsAsync);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendEvent(in SqliteEvent @event)
    {
        if (!_ringBuffer.TryNext(out var sequence))
        {
            _overflowChannel.Writer.TryWrite(@event);
            return;
        }

        try
        {
            _ringBuffer[sequence] = @event;
        }
        finally
        {
            _ringBuffer.Publish(sequence);
        }
    }

    private async Task ProcessOverflowEventsAsync()
    {
        await foreach (var @event in _overflowChannel.Reader.ReadAllAsync(_cts.Token))
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Events (Id, Timestamp, Type, PayloadSize, Payload, ExtendedPayloadId, Checksum)
                VALUES (@id, @timestamp, @type, @payloadSize, @payload, @extId, @checksum)";
                
            cmd.Parameters.AddWithValue("@id", @event.Id);
            cmd.Parameters.AddWithValue("@timestamp", @event.Timestamp);
            cmd.Parameters.AddWithValue("@type", (int)@event.Type);
            cmd.Parameters.AddWithValue("@payloadSize", @event.PayloadSize);
            cmd.Parameters.AddWithValue("@payload", @event.Payload.ToArray());
            cmd.Parameters.AddWithValue("@extId", @event.ExtendedPayloadId);
            cmd.Parameters.AddWithValue("@checksum", @event.Checksum);
            
            await cmd.ExecuteNonQueryAsync();
            _eventPool.Return(@event);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _disruptor.Halt();
        _overflowChannel.Writer.Complete();
        _cts.Cancel();
        await _processingTask;
        await _connection.DisposeAsync();
    }
}

// 3. 事件处理器
public class SqliteEventHandler : IEventHandler<SqliteEvent>
{
    private readonly SqliteConnection _connection;
    private readonly ObjectPool<SqliteCommand> _commandPool;

    public SqliteEventHandler(SqliteConnection connection)
    {
        _connection = connection;
        _commandPool = new DefaultObjectPool<SqliteCommand>(
            new SqliteCommandPoolPolicy(connection), 
            Environment.ProcessorCount * 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void OnEvent(SqliteEvent @event, long sequence, bool endOfBatch)
    {
        // 新增：校验和验证
        if (@event.ComputeChecksum() != @event.Checksum)
        {
            // 处理校验失败
            return;
        }

        var cmd = _commandPool.Get();
        try
        {
            cmd.CommandText = @"
                INSERT INTO Events (Id, Timestamp, Type, PayloadSize, Payload, ExtendedPayloadId, Checksum)
                VALUES (@id, @timestamp, @type, @payloadSize, @payload, @extId, @checksum)";
                
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@id", @event.Id);
            cmd.Parameters.AddWithValue("@timestamp", @event.Timestamp);
            cmd.Parameters.AddWithValue("@type", (int)@event.Type);
            cmd.Parameters.AddWithValue("@payloadSize", @event.PayloadSize);
            cmd.Parameters.AddWithValue("@payload", @event.Payload.ToArray());
            cmd.Parameters.AddWithValue("@extId", @event.ExtendedPayloadId);
            cmd.Parameters.AddWithValue("@checksum", @event.Checksum);
            
            cmd.ExecuteNonQuery();
        }
        finally
        {
            _commandPool.Return(cmd);
        }
    }
}

// 4. 对象池策略
public class SqliteEventPoolPolicy : IPooledObjectPolicy<SqliteEvent>
{
    public SqliteEvent Create() => new SqliteEvent();
    public bool Return(SqliteEvent obj) => true;
}

public class SqliteCommandPoolPolicy : IPooledObjectPolicy<SqliteCommand>
{
    private readonly SqliteConnection _connection;

    public SqliteCommandPoolPolicy(SqliteConnection connection) => _connection = connection;

    public SqliteCommand Create() => _connection.CreateCommand();
    public bool Return(SqliteCommand obj) => true;
}