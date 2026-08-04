#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package Microsoft.Data.Sqlite@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Microsoft.Extensions.ObjectPool;

public class SqliteConnectionPool : IAsyncDisposable
{
    private readonly ObjectPool<SqliteConnection> _pool;
    private readonly string _connectionString;

    public SqliteConnectionPool(string connectionString, int maxPoolSize = 10)
    {
        _connectionString = connectionString;
        _pool = new DefaultObjectPool<SqliteConnection>(
            new SqliteConnectionPoolPolicy(connectionString), 
            maxPoolSize);
    }

    public SqliteConnection Rent() => _pool.Get();

    public void Return(SqliteConnection connection)
    {
        if (connection.State == ConnectionState.Open)
            _pool.Return(connection);
        else
            connection.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        // 清理逻辑...
    }

    private class SqliteConnectionPoolPolicy : IPooledObjectPolicy<SqliteConnection>
    {
        private readonly string _connectionString;

        public SqliteConnectionPoolPolicy(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqliteConnection Create()
        {
            var conn = new SqliteConnection(_connectionString);
            conn.Open();
            return conn;
        }

        public bool Return(SqliteConnection obj)
        {
            return obj.State == ConnectionState.Open;
        }
    }
}