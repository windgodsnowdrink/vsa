#:sdk Microsoft.NET.Sdk
#:package LiteDB@5.0.17
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Threading.Channels;
using LiteDB;
using Microsoft.Extensions.ObjectPool;

// 1. 日志实体模型
[MemoryPackable]
public partial class LogEntry
{
    [BsonId]
    public ObjectId Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Level { get; set; }
    public string Message { get; set; }
    public string Exception { get; set; }
    public string Source { get; set; }
}

// 2. 日志数据库服务
public class LiteDbLogService : IAsyncDisposable
{
    private readonly Channel<LogEntry> _logChannel;
    private readonly Task _processingTask;
    private readonly ObjectPool<ILiteDatabase> _dbPool;
    private readonly CancellationTokenSource _cts = new();

    public LiteDbLogService(string connectionString, int maxDegreeOfParallelism = 4)
    {
        _dbPool = new DefaultObjectPool<ILiteDatabase>(
            new LiteDbPoolPolicy(connectionString), 
            maxDegreeOfParallelism);

        _logChannel = Channel.CreateBounded<LogEntry>(10000);
        _processingTask = Task.Run(ProcessLogsAsync);
    }

    public ValueTask LogAsync(LogEntry entry)
    {
        return _logChannel.Writer.WriteAsync(entry, _cts.Token).AsTask();
    }

    private async Task ProcessLogsAsync()
    {
        await foreach (var entry in _logChannel.Reader.ReadAllAsync(_cts.Token))
        {
            using var db = _dbPool.Get();
            try
            {
                var collection = db.GetCollection<LogEntry>("logs");
                collection.Insert(entry);
                collection.EnsureIndex(x => x.Timestamp);
                collection.EnsureIndex(x => x.Level);
            }
            finally
            {
                _dbPool.Return(db);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _logChannel.Writer.Complete();
        _cts.Cancel();
        await _processingTask;
    }

    private class LiteDbPoolPolicy : IPooledObjectPolicy<ILiteDatabase>
    {
        private readonly string _connectionString;

        public LiteDbPoolPolicy(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ILiteDatabase Create()
        {
            var db = new LiteDatabase(_connectionString);
            // 启用WAL模式提高性能
            db.Pragma("WAL", true);
            return db;
        }

        public bool Return(ILiteDatabase obj)
        {
            return !obj.IsDisposed;
        }
    }
}

// 3. 日志查询服务
public class LogQueryService
{
    private readonly ObjectPool<ILiteDatabase> _dbPool;

    public LogQueryService(ObjectPool<ILiteDatabase> dbPool)
    {
        _dbPool = dbPool;
    }

    public IEnumerable<LogEntry> Query(DateTime from, DateTime to, string level = null)
    {
        using var db = _dbPool.Get();
        var collection = db.GetCollection<LogEntry>("logs");
        
        var query = collection.Query()
            .Where(x => x.Timestamp >= from && x.Timestamp <= to);
            
        if (!string.IsNullOrEmpty(level))
            query = query.Where(x => x.Level == level);
            
        return query.ToEnumerable();
    }
}

// 4. 使用示例
public static class LiteDbLogDemo
{
    public static async Task RunAsync()
    {
        var logService = new LiteDbLogService("Filename=logs.db;Connection=shared");
        
        // 写入日志
        await logService.LogAsync(new LogEntry
        {
            Timestamp = DateTime.UtcNow,
            Level = "INFO",
            Message = "Application started",
            Source = "Startup"
        });
        
        // 查询日志
        var queryService = new LogQueryService(logService._dbPool);
        var logs = queryService.Query(
            DateTime.UtcNow.AddHours(-1), 
            DateTime.UtcNow, 
            "INFO");
            
        await logService.DisposeAsync();
    }
}