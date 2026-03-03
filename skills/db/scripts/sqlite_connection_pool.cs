#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package Microsoft.Data.Sqlite@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.ObjectPool;
using System.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<SqliteConnectionPool>(sp => 
    new SqliteConnectionPool("Data Source=app.db", 20));
builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();
app.Run();

/// <summary>
/// SQLite 连接池
/// </summary>
public class SqliteConnectionPool : IAsyncDisposable
{
    private readonly ObjectPool<SqliteConnection> _pool;
    private readonly string _connectionString;
    private readonly List<SqliteConnection> _createdConnections = new();

    /// <summary>
    /// 初始化 SQLite 连接池
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="maxPoolSize">最大连接池大小</param>
    public SqliteConnectionPool(string connectionString, int maxPoolSize = 10)
    {
        _connectionString = connectionString;
        _pool = new DefaultObjectPool<SqliteConnection>(
            new SqliteConnectionPoolPolicy(connectionString, _createdConnections), 
            maxPoolSize);
    }

    /// <summary>
    /// 从连接池获取连接
    /// </summary>
    /// <returns>SQLite 连接</returns>
    public SqliteConnection Rent() => _pool.Get();

    /// <summary>
    /// 归还连接到连接池
    /// </summary>
    /// <param name="connection">SQLite 连接</param>
    public void Return(SqliteConnection connection)
    {
        if (connection.State == ConnectionState.Open)
            _pool.Return(connection);
        else
            connection.Dispose();
    }

    /// <summary>
    /// 使用连接池中的连接执行操作
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="action">操作函数</param>
    /// <returns>操作结果</returns>
    public T Use<T>(Func<SqliteConnection, T> action)
    {
        var connection = Rent();
        try
        {
            return action(connection);
        }
        finally
        {
            Return(connection);
        }
    }

    /// <summary>
    /// 异步使用连接池中的连接执行操作
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="action">异步操作函数</param>
    /// <returns>操作结果</returns>
    public async Task<T> UseAsync<T>(Func<SqliteConnection, Task<T>> action)
    {
        var connection = Rent();
        try
        {
            return await action(connection);
        }
        finally
        {
            Return(connection);
        }
    }

    /// <summary>
    /// 使用连接池中的连接执行操作
    /// </summary>
    /// <param name="action">操作动作</param>
    public void Use(Action<SqliteConnection> action)
    {
        var connection = Rent();
        try
        {
            action(connection);
        }
        finally
        {
            Return(connection);
        }
    }

    /// <summary>
    /// 异步使用连接池中的连接执行操作
    /// </summary>
    /// <param name="action">异步操作动作</param>
    /// <returns>任务</returns>
    public async Task UseAsync(Func<SqliteConnection, Task> action)
    {
        var connection = Rent();
        try
        {
            await action(connection);
        }
        finally
        {
            Return(connection);
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <returns>任务</returns>
    public async ValueTask DisposeAsync()
    {
        foreach (var connection in _createdConnections)
        {
            await connection.DisposeAsync();
        }
        _createdConnections.Clear();
    }

    /// <summary>
    /// SQLite 连接池策略
    /// </summary>
    private class SqliteConnectionPoolPolicy : IPooledObjectPolicy<SqliteConnection>
    {
        private readonly string _connectionString;
        private readonly List<SqliteConnection> _createdConnections;

        /// <summary>
        /// 初始化 SQLite 连接池策略
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="createdConnections">创建的连接列表</param>
        public SqliteConnectionPoolPolicy(string connectionString, List<SqliteConnection> createdConnections)
        {
            _connectionString = connectionString;
            _createdConnections = createdConnections;
        }

        /// <summary>
        /// 创建新的 SQLite 连接
        /// </summary>
        /// <returns>SQLite 连接</returns>
        public SqliteConnection Create()
        {
            var conn = new SqliteConnection(_connectionString);
            conn.Open();
            _createdConnections.Add(conn);
            return conn;
        }

        /// <summary>
        /// 验证并归还 SQLite 连接
        /// </summary>
        /// <param name="obj">SQLite 连接</param>
        /// <returns>是否可以归还到池</returns>
        public bool Return(SqliteConnection obj)
        {
            return obj.State == ConnectionState.Open;
        }
    }
}

/// <summary>
/// SQLite 连接池扩展方法
/// </summary>
public static class SqliteConnectionPoolExtensions
{
    /// <summary>
    /// 注册 SQLite 连接池服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="maxPoolSize">最大连接池大小</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddSqliteConnectionPool(
        this IServiceCollection services,
        string connectionString,
        int maxPoolSize = 10)
    {
        services.AddSingleton<SqliteConnectionPool>(sp => 
            new SqliteConnectionPool(connectionString, maxPoolSize));
        return services;
    }
}

/// <summary>
/// SQLite 连接池控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConnectionPoolController : ControllerBase
{
    private readonly SqliteConnectionPool _pool;

    /// <summary>
    /// 初始化 SQLite 连接池控制器
    /// </summary>
    /// <param name="pool">SQLite 连接池</param>
    public ConnectionPoolController(SqliteConnectionPool pool)
    {
        _pool = pool;
    }

    /// <summary>
    /// 测试连接池
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpGet("test")]
    public async Task<IActionResult> TestPool(CancellationToken cancellationToken)
    {
        // 并行测试连接池
        var tasks = Enumerable.Range(1, 50).Select(async i =>
        {
            return await _pool.UseAsync(async (connection) =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT 1 + @Value";
                command.Parameters.AddWithValue("@Value", i);
                return await command.ExecuteScalarAsync(cancellationToken);
            });
        });

        var results = await Task.WhenAll(tasks);
        return Ok(new { count = results.Length, success = results.All(r => r != null) });
    }
}
