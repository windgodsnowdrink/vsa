#:sdk Microsoft.NET.Sdk
#:package Microsoft.Data.Sqlite@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Threading.Channels;
using Microsoft.Data.Sqlite;

public class SqliteBulkWriter<T> : IAsyncDisposable
{
    private readonly Channel<T> _channel;
    private readonly SqliteConnection _connection;
    private readonly Task _processingTask;
    private readonly string _insertSql;

    public SqliteBulkWriter(
        SqliteConnection connection, 
        string tableName,
        int bufferSize = 10000)
    {
        _connection = connection;
        _insertSql = $"INSERT INTO {tableName} VALUES ({GenerateParameters(typeof(T))})";
        _channel = Channel.CreateBounded<T>(bufferSize);
        _processingTask = Task.Run(ProcessAsync);
    }

    public async ValueTask WriteAsync(T item, CancellationToken ct = default)
    {
        await _channel.Writer.WriteAsync(item, ct);
    }

    private async Task ProcessAsync()
    {
        await using var transaction = await _connection.BeginTransactionAsync();
        
        await foreach (var item in _channel.Reader.ReadAllAsync())
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = _insertSql;
            // 参数绑定逻辑...
            await cmd.ExecuteNonQueryAsync();
        }
        
        await transaction.CommitAsync();
    }

    public async ValueTask DisposeAsync()
    {
        _channel.Writer.Complete();
        await _processingTask;
    }

    private static string GenerateParameters(Type type)
    {
        // 生成参数占位符...
        return string.Join(",", Enumerable.Range(0, type.GetProperties().Length).Select(i => $"@p{i}"));
    }
}