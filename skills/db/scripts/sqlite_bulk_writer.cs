#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Data.Sqlite@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Reflection;
using System.Threading.Channels;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();
app.Run();

/// <summary>
/// SQLite 批量写入器
/// </summary>
/// <typeparam name="T">写入的数据类型</typeparam>
public class SqliteBulkWriter<T> : IAsyncDisposable
{
    private readonly Channel<T> _channel;
    private readonly SqliteConnection _connection;
    private readonly Task _processingTask;
    private readonly string _insertSql;
    private readonly int _batchSize;

    /// <summary>
    /// 初始化 SQLite 批量写入器
    /// </summary>
    /// <param name="connection">SQLite 连接</param>
    /// <param name="tableName">表名</param>
    /// <param name="batchSize">批处理大小</param>
    /// <param name="bufferSize">缓冲区大小</param>
    public SqliteBulkWriter(
        SqliteConnection connection, 
        string tableName, 
        int batchSize = 1000,
        int bufferSize = 10000)
    {
        _connection = connection;
        _batchSize = batchSize;
        _insertSql = $"INSERT INTO {tableName} VALUES ({GenerateParameters(typeof(T))})";
        _channel = Channel.CreateBounded<T>(new BoundedChannelOptions(bufferSize)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });
        _processingTask = Task.Run(ProcessAsync);
    }

    /// <summary>
    /// 写入单个项目
    /// </summary>
    /// <param name="item">要写入的项目</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>任务</returns>
    public async ValueTask WriteAsync(T item, CancellationToken ct = default)
    {
        await _channel.Writer.WriteAsync(item, ct);
    }

    /// <summary>
    /// 写入多个项目
    /// </summary>
    /// <param name="items">要写入的项目集合</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>任务</returns>
    public async ValueTask WriteManyAsync(IEnumerable<T> items, CancellationToken ct = default)
    {
        foreach (var item in items)
        {
            await _channel.Writer.WriteAsync(item, ct);
        }
    }

    /// <summary>
    /// 处理写入操作
    /// </summary>
    /// <returns>任务</returns>
    private async Task ProcessAsync()
    {
        var batch = new List<T>(_batchSize);

        await using var transaction = await _connection.BeginTransactionAsync();
        try
        {
            await foreach (var item in _channel.Reader.ReadAllAsync())
            {
                batch.Add(item);

                if (batch.Count >= _batchSize)
                {
                    await ExecuteBatchAsync(batch, transaction);
                    batch.Clear();
                }
            }

            // 处理剩余项目
            if (batch.Count > 0)
            {
                await ExecuteBatchAsync(batch, transaction);
            }

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// 执行批处理写入
    /// </summary>
    /// <param name="items">项目集合</param>
    /// <param name="transaction">事务</param>
    /// <returns>任务</returns>
    private async Task ExecuteBatchAsync(IEnumerable<T> items, SqliteTransaction transaction)
    {
        foreach (var item in items)
        {
            using var cmd = _connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = _insertSql;
            BindParameters(cmd, item);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    /// <summary>
    /// 绑定参数
    /// </summary>
    /// <param name="command">SQLite 命令</param>
    /// <param name="item">项目</param>
    private void BindParameters(SqliteCommand command, T item)
    {
        var properties = typeof(T).GetProperties();
        for (int i = 0; i < properties.Length; i++)
        {
            var value = properties[i].GetValue(item);
            command.Parameters.AddWithValue($"@p{i}", value ?? DBNull.Value);
        }
    }

    /// <summary>
    /// 生成参数占位符
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>参数占位符字符串</returns>
    private static string GenerateParameters(Type type)
    {
        var propertyCount = type.GetProperties().Length;
        return string.Join(",", Enumerable.Range(0, propertyCount).Select(i => $"@p{i}"));
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <returns>任务</returns>
    public async ValueTask DisposeAsync()
    {
        _channel.Writer.Complete();
        await _processingTask;
    }
}

/// <summary>
/// SQLite 批量写入器扩展方法
/// </summary>
public static class SqliteBulkWriterExtensions
{
    /// <summary>
    /// 创建 SQLite 批量写入器
    /// </summary>
    /// <typeparam name="T">写入的数据类型</typeparam>
    /// <param name="connection">SQLite 连接</param>
    /// <param name="tableName">表名</param>
    /// <param name="batchSize">批处理大小</param>
    /// <param name="bufferSize">缓冲区大小</param>
    /// <returns>SQLite 批量写入器</returns>
    public static SqliteBulkWriter<T> CreateBulkWriter<T>(this SqliteConnection connection, string tableName, int batchSize = 1000, int bufferSize = 10000)
    {
        return new SqliteBulkWriter<T>(connection, tableName, batchSize, bufferSize);
    }
}

/// <summary>
/// 示例数据模型
/// </summary>
public class SampleData
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// SQLite 控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SqliteController : ControllerBase
{
    /// <summary>
    /// 测试批量写入
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("bulk-write")]
    public async Task<IActionResult> TestBulkWrite(CancellationToken cancellationToken)
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(cancellationToken);

        // 创建测试表
        using var createTableCmd = connection.CreateCommand();
        createTableCmd.CommandText = @"
            CREATE TABLE SampleData (
                Id INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                Value REAL NOT NULL,
                CreatedAt TEXT NOT NULL
            );
        ";
        await createTableCmd.ExecuteNonQueryAsync(cancellationToken);

        // 生成测试数据
        var testData = Enumerable.Range(1, 10000).Select(i => new SampleData
        {
            Id = i,
            Name = $"Item {i}",
            Value = i * 1.5,
            CreatedAt = DateTime.UtcNow
        });

        // 使用批量写入器
        await using var bulkWriter = connection.CreateBulkWriter<SampleData>("SampleData");
        await bulkWriter.WriteManyAsync(testData, cancellationToken);

        // 验证写入结果
        using var countCmd = connection.CreateCommand();
        countCmd.CommandText = "SELECT COUNT(*) FROM SampleData";
        var count = await countCmd.ExecuteScalarAsync(cancellationToken);

        return Ok(new { count });
    }
}
