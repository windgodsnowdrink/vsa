#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Data.Sqlite@8.0.0
#:package Disruptor-net@3.4.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package Dapper@2.1.28
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Disruptor;
using Disruptor.Dsl;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.ObjectPool;
using Dapper;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();
app.Run();

/// <summary>
/// 事件类型
/// </summary>
enum EventType
{
    UserCreated,
    UserUpdated,
    OrderPlaced,
    OrderShipped
}

/// <summary>
/// SQLite 事件（支持分片和校验）
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 64)]
public struct SqliteEvent
{
    [FieldOffset(0)]
    public long Id;

    [FieldOffset(8)]
    public long Timestamp;

    [FieldOffset(16)]
    public EventType Type;

    [FieldOffset(20)]
    public int PayloadSize;

    [FieldOffset(24)]
    public int ExtendedPayloadId;

    [FieldOffset(28)]
    public ushort ChunkIndex;

    [FieldOffset(30)]
    public ushort TotalChunks;

    [FieldOffset(32)]
    public uint Checksum;

    [FieldOffset(36)]
    private fixed byte _payload[28];

    /// <summary>
    /// 获取有效载荷跨度
    /// </summary>
    public Span<byte> Payload => GetPayloadSpan();

    /// <summary>
    /// 获取有效载荷跨度
    /// </summary>
    /// <returns>字节跨度</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe Span<byte> GetPayloadSpan()
    {
        fixed (byte* ptr = _payload)
        {
            return new Span<byte>(ptr, PayloadSize);
        }
    }

    /// <summary>
    /// 计算 CRC32 校验和
    /// </summary>
    /// <returns>校验和</returns>
    public uint ComputeChecksum()
    {
        using var crc32 = new Crc32();
        var payload = Payload;
        var buffer = new byte[payload.Length];
        payload.CopyTo(buffer);
        return crc32.ComputeHash(buffer);
    }
}

/// <summary>
/// CRC32 计算器
/// </summary>
public class Crc32 : HashAlgorithm
{
    private const uint Polynomial = 0xEDB88320;
    private uint[] _table = new uint[256];
    private uint _crc = 0xFFFFFFFF;

    /// <summary>
    /// 初始化 CRC32 计算器
    /// </summary>
    public Crc32()
    {
        InitializeTable();
    }

    /// <summary>
    /// 初始化表
    /// </summary>
    private void InitializeTable()
    {
        for (uint i = 0; i < 256; i++)
        {
            uint crc = i;
            for (int j = 0; j < 8; j++)
            {
                crc = (crc >> 1) ^ (Polynomial & (-(crc & 1)));
            }
            _table[i] = crc;
        }
    }

    /// <summary>
    /// 初始化哈希算法
    /// </summary>
    public override void Initialize()
    {
        _crc = 0xFFFFFFFF;
    }

    /// <summary>
    /// 哈希核心
    /// </summary>
    /// <param name="array">字节数组</param>
    /// <param name="ibStart">起始索引</param>
    /// <param name="cbSize">大小</param>
    protected override void HashCore(byte[] array, int ibStart, int cbSize)
    {
        for (int i = ibStart; i < ibStart + cbSize; i++)
        {
            _crc = (_crc >> 8) ^ _table[(_crc ^ array[i]) & 0xFF];
        }
    }

    /// <summary>
    /// 哈希结束
    /// </summary>
    /// <returns>哈希值</returns>
    protected override byte[] HashFinal()
    {
        _crc = ~_crc;
        return BitConverter.GetBytes(_crc);
    }

    /// <summary>
    /// 计算哈希值
    /// </summary>
    /// <param name="data">数据</param>
    /// <returns>哈希值</returns>
    public uint ComputeHash(byte[] data)
    {
        Initialize();
        HashCore(data, 0, data.Length);
        var hash = HashFinal();
        return BitConverter.ToUInt32(hash, 0);
    }
}

/// <summary>
/// 事务事件
/// </summary>
public class TransactionEvent
{
    /// <summary>
    /// 事务ID
    /// </summary>
    public string TransactionId { get; set; }

    /// <summary>
    /// 事件类型
    /// </summary>
    public string EventType { get; set; }

    /// <summary>
    /// 数据
    /// </summary>
    public string Data { get; set; }
}

/// <summary>
/// 分布式事务协调器
/// </summary>
public class DistributedTransactionCoordinator
{
    private readonly SqliteConnection _connection;
    private readonly Channel<TransactionEvent> _transactionChannel;

    /// <summary>
    /// 初始化分布式事务协调器
    /// </summary>
    /// <param name="connection">SQLite 连接</param>
    public DistributedTransactionCoordinator(SqliteConnection connection)
    {
        _connection = connection;
        _transactionChannel = Channel.CreateBounded<TransactionEvent>(1000);
    }

    /// <summary>
    /// 开始事务
    /// </summary>
    /// <param name="transactionId">事务ID</param>
    /// <returns>任务</returns>
    public async Task BeginTransactionAsync(string transactionId)
    {
        await _connection.ExecuteAsync(
            "INSERT INTO Transactions (TransactionId, Status, CreatedAt) VALUES (@TransactionId, 'Started', @CreatedAt)",
            new { TransactionId = transactionId, CreatedAt = DateTime.UtcNow });
    }

    /// <summary>
    /// 提交事务
    /// </summary>
    /// <param name="transactionId">事务ID</param>
    /// <returns>任务</returns>
    public async Task CommitTransactionAsync(string transactionId)
    {
        await _connection.ExecuteAsync(
            "UPDATE Transactions SET Status = 'Committed', UpdatedAt = @UpdatedAt WHERE TransactionId = @TransactionId",
            new { TransactionId = transactionId, UpdatedAt = DateTime.UtcNow });
    }

    /// <summary>
    /// 回滚事务
    /// </summary>
    /// <param name="transactionId">事务ID</param>
    /// <returns>任务</returns>
    public async Task RollbackTransactionAsync(string transactionId)
    {
        await _connection.ExecuteAsync(
            "UPDATE Transactions SET Status = 'RolledBack', UpdatedAt = @UpdatedAt WHERE TransactionId = @TransactionId",
            new { TransactionId = transactionId, UpdatedAt = DateTime.UtcNow });
    }
}

/// <summary>
/// SQLite 事件存储
/// </summary>
public class SqliteEventStore : IAsyncDisposable
{
    private readonly SqliteConnection _connection;
    private readonly Disruptor<SqliteEvent> _disruptor;
    private readonly RingBuffer<SqliteEvent> _ringBuffer;
    private readonly ObjectPool<SqliteConnection> _connectionPool;

    /// <summary>
    /// 初始化 SQLite 事件存储
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    public SqliteEventStore(string connectionString)
    {
        _connection = new SqliteConnection(connectionString);
        _connection.Open();
        InitializeSchema();

        var factory = new EventFactory();
        var ringBufferSize = 1024;
        var disruptor = new Disruptor<SqliteEvent>(factory, ringBufferSize, TaskScheduler.Default);
        disruptor.HandleEventsWith(new EventHandler());
        _disruptor = disruptor;
        _ringBuffer = disruptor.Start();

        _connectionPool = new DefaultObjectPool<SqliteConnection>(
            new SqliteConnectionPoolPolicy(connectionString),
            10);
    }

    /// <summary>
    /// 初始化架构
    /// </summary>
    private void InitializeSchema()
    {
        _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Events (
                Id INTEGER PRIMARY KEY,
                Timestamp INTEGER NOT NULL,
                Type INTEGER NOT NULL,
                Payload BLOB NOT NULL,
                ExtendedPayloadId INTEGER,
                ChunkIndex INTEGER,
                TotalChunks INTEGER,
                Checksum INTEGER,
                CreatedAt TEXT NOT NULL
            );
            CREATE INDEX IF NOT EXISTS IX_Events_Timestamp ON Events(Timestamp);
            CREATE INDEX IF NOT EXISTS IX_Events_Type ON Events(Type);
            CREATE TABLE IF NOT EXISTS Transactions (
                Id INTEGER PRIMARY KEY,
                TransactionId TEXT UNIQUE NOT NULL,
                Status TEXT NOT NULL,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT
            );
        ");
    }

    /// <summary>
    /// 写入事件
    /// </summary>
    /// <param name="@event">事件</param>
    /// <returns>任务</returns>
    public Task WriteEventAsync(SqliteEvent @event)
    {
        var sequence = _ringBuffer.Next();
        try
        {
            var eventToPublish = _ringBuffer[sequence];
            // 复制事件数据
            eventToPublish.Id = @event.Id;
            eventToPublish.Timestamp = @event.Timestamp;
            eventToPublish.Type = @event.Type;
            eventToPublish.PayloadSize = @event.PayloadSize;
            eventToPublish.ExtendedPayloadId = @event.ExtendedPayloadId;
            eventToPublish.ChunkIndex = @event.ChunkIndex;
            eventToPublish.TotalChunks = @event.TotalChunks;
            eventToPublish.Checksum = @event.Checksum;
            // 复制有效载荷
            var payload = @event.Payload;
            var eventPayload = eventToPublish.Payload;
            payload.CopyTo(eventPayload);
        }
        finally
        {
            _ringBuffer.Publish(sequence);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// 读取事件
    /// </summary>
    /// <param name="startId">起始ID</param>
    /// <param name="count">数量</param>
    /// <returns>事件集合</returns>
    public async Task<List<SqliteEvent>> ReadEventsAsync(long startId, int count)
    {
        var connection = _connectionPool.Get();
        try
        {
            return (await connection.QueryAsync<dynamic>(
                "SELECT Id, Timestamp, Type, Payload, ExtendedPayloadId, ChunkIndex, TotalChunks, Checksum FROM Events WHERE Id >= @StartId LIMIT @Count",
                new { StartId = startId, Count = count })).Select(row =>
            {
                var @event = new SqliteEvent
                {
                    Id = row.Id,
                    Timestamp = row.Timestamp,
                    Type = (EventType)row.Type,
                    ExtendedPayloadId = row.ExtendedPayloadId,
                    ChunkIndex = (ushort)row.ChunkIndex,
                    TotalChunks = (ushort)row.TotalChunks,
                    Checksum = (uint)row.Checksum
                };
                // 设置有效载荷
                return @event;
            }).ToList();
        }
        finally
        {
            _connectionPool.Return(connection);
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <returns>任务</returns>
    public async ValueTask DisposeAsync()
    {
        _disruptor.Shutdown();
        await _connection.DisposeAsync();
    }

    /// <summary>
    /// 事件工厂
    /// </summary>
    private class EventFactory : IEventFactory<SqliteEvent>
    {
        /// <summary>
        /// 创建事件
        /// </summary>
        /// <returns>事件</returns>
        public SqliteEvent NewInstance()
        {
            return new SqliteEvent();
        }
    }

    /// <summary>
    /// 事件处理器
    /// </summary>
    private class EventHandler : IEventHandler<SqliteEvent>
    {
        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="@event">事件</param>
        /// <param name="sequence">序列</param>
        /// <param name="endOfBatch">是否批处理结束</param>
        public void OnEvent(SqliteEvent @event, long sequence, bool endOfBatch)
        {
            // 处理事件，例如写入数据库
        }
    }

    /// <summary>
    /// SQLite 连接池策略
    /// </summary>
    private class SqliteConnectionPoolPolicy : IPooledObjectPolicy<SqliteConnection>
    {
        private readonly string _connectionString;

        /// <summary>
        /// 初始化 SQLite 连接池策略
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        public SqliteConnectionPoolPolicy(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns>SQLite 连接</returns>
        public SqliteConnection Create()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// 归还连接
        /// </summary>
        /// <param name="obj">SQLite 连接</param>
        /// <returns>是否可以归还到池</returns>
        public bool Return(SqliteConnection obj)
        {
            return obj.State == System.Data.ConnectionState.Open;
        }
    }
}

/// <summary>
/// SQLite 事件存储扩展方法
/// </summary>
public static class SqliteEventStoreExtensions
{
    /// <summary>
    /// 注册 SQLite 事件存储服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="connectionString">连接字符串</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddSqliteEventStore(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddSingleton<SqliteEventStore>(sp => 
            new SqliteEventStore(connectionString));
        return services;
    }
}

/// <summary>
/// 事件存储控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EventStoreController : ControllerBase
{
    private readonly SqliteEventStore _eventStore;

    /// <summary>
    /// 初始化事件存储控制器
    /// </summary>
    /// <param name="eventStore">SQLite 事件存储</param>
    public EventStoreController(SqliteEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    /// <summary>
    /// 写入事件
    /// </summary>
    /// <param name="eventData">事件数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("write")]
    public async Task<IActionResult> WriteEvent([FromBody] EventData eventData, CancellationToken cancellationToken)
    {
        var @event = new SqliteEvent
        {
            Id = eventData.Id,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Type = eventData.Type,
            PayloadSize = eventData.Payload.Length,
            Checksum = 0 // 将在写入时计算
        };
        // 设置有效载荷
        // ...
        await _eventStore.WriteEventAsync(@event);
        return Ok();
    }

    /// <summary>
    /// 读取事件
    /// </summary>
    /// <param name="startId">起始ID</param>
    /// <param name="count">数量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>事件集合</returns>
    [HttpGet("read")]
    public async Task<IActionResult> ReadEvents(long startId = 1, int count = 10, CancellationToken cancellationToken = default)
    {
        var events = await _eventStore.ReadEventsAsync(startId, count);
        return Ok(events);
    }
}

/// <summary>
/// 事件数据
/// </summary>
public class EventData
{
    /// <summary>
    /// ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public EventType Type { get; set; }

    /// <summary>
    /// 有效载荷
    /// </summary>
    public byte[] Payload { get; set; }
}
