#:sdk Microsoft.NET.Sdk.Web
#:package ClickHouse.Client@2.6.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using ClickHouse.Client.ADO;
using ClickHouse.Client.Copy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using System.Buffers;
using System.Threading.Channels;

namespace ClickHouseIntegration
{
    public class ClickHouseOptions
    {
        /// <summary>
        /// ClickHouse连接字符串
        /// </summary>
        public string ConnectionString { get; set; } = "Host=localhost;Port=9000;Database=default;User=default";
        
        /// <summary>
        /// 最大重试次数
        /// </summary>
        public int MaxRetryCount { get; set; } = 3;
        
        /// <summary>
        /// 重试延迟时间
        /// </summary>
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
        
        /// <summary>
        /// 熔断器阈值
        /// </summary>
        public int CircuitBreakerThreshold { get; set; } = 5;
        
        /// <summary>
        /// 熔断持续时间
        /// </summary>
        public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromMinutes(1);
        
        /// <summary>
        /// 批量插入批次大小
        /// </summary>
        public int BulkInsertBatchSize { get; set; } = 1000;
        
        /// <summary>
        /// 批量插入通道容量
        /// </summary>
        public int BulkInsertChannelCapacity { get; set; } = 10000;
        
        /// <summary>
        /// 默认集群名称(用于分布式查询)
        /// </summary>
        public string DefaultClusterName { get; set; } = "default_cluster";
        
        /// <summary>
        /// 查询超时时间
        /// </summary>
        public TimeSpan QueryTimeout { get; set; } = TimeSpan.FromMinutes(1);
    }

    public interface IClickHouseService
    {
        /// <summary>
        /// 执行非查询SQL语句
        /// </summary>
        Task ExecuteNonQueryAsync(string sql, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 执行查询SQL语句并返回DataTable
        /// </summary>
        Task<DataTable> ExecuteQueryAsync(string sql, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 批量插入数据到指定表
        /// </summary>
        Task BulkInsertAsync<T>(string tableName, IEnumerable<T> data, CancellationToken cancellationToken = default) where T : class;
        
        /// <summary>
        /// 创建批量插入通道，返回ChannelWriter用于写入数据
        /// </summary>
        Task<ChannelWriter<T>> CreateBulkInsertChannel<T>(string tableName, CancellationToken cancellationToken = default) where T : class;
        
        /// <summary>
        /// 分布式查询 - 在集群上执行查询
        /// </summary>
        Task<DataTable> DistributedQueryAsync(string clusterName, string sql, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 创建表
        /// </summary>
        Task CreateTableAsync(string tableName, string schema, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 删除表
        /// </summary>
        Task DropTableAsync(string tableName, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 添加表分区
        /// </summary>
        Task AddPartitionAsync(string tableName, string partitionExpr, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 创建物化视图
        /// </summary>
        Task CreateMaterializedViewAsync(string viewName, string selectQuery, string targetTable, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 重置熔断器状态
        /// </summary>
        Task ResetCircuitBreakerAsync();
        
        /// <summary>
        /// 获取连接统计信息
        /// </summary>
        Task<ConnectionStats> GetConnectionStatsAsync();
    }

    public class ClickHouseService : IClickHouseService, IDisposable
    {
        /// <summary>
        /// 在指定集群上执行分布式查询
        /// </summary>
        public async Task<DataTable> DistributedQueryAsync(string clusterName, string sql, CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _activeConnections);
            try
            {
                var distributedSql = $"SELECT * FROM remote('{clusterName}', {sql})";
                
                var result = await _circuitBreakerPolicy.WrapAsync(_retryPolicy)
                    .ExecuteAsync(async ct =>
                    {
                        using var connection = new ClickHouseConnection(_options.ConnectionString);
                        await connection.OpenAsync(ct);
                        using var command = connection.CreateCommand();
                        command.CommandText = distributedSql;
                        using var reader = await command.ExecuteReaderAsync(ct);
                        var dataTable = new DataTable();
                        dataTable.Load(reader);
                        return dataTable;
                    }, cancellationToken);
                
                Interlocked.Increment(ref _successfulQueries);
                Interlocked.Increment(ref _distributedQueries);
                return result;
            }
            catch
            {
                Interlocked.Increment(ref _failedQueries);
                throw;
            }
            finally
            {
                Interlocked.Decrement(ref _activeConnections);
            }
        }
        
        /// <summary>
        /// 创建表
        /// </summary>
        public async Task CreateTableAsync(string tableName, string schema, CancellationToken cancellationToken = default)
        {
            var sql = $"CREATE TABLE IF NOT EXISTS {tableName} ({schema})";
            await ExecuteNonQueryAsync(sql, cancellationToken);
        }
        
        /// <summary>
        /// 删除表
        /// </summary>
        public async Task DropTableAsync(string tableName, CancellationToken cancellationToken = default)
        {
            var sql = $"DROP TABLE IF EXISTS {tableName}";
            await ExecuteNonQueryAsync(sql, cancellationToken);
        }
        
        /// <summary>
        /// 添加表分区
        /// </summary>
        public async Task AddPartitionAsync(string tableName, string partitionExpr, CancellationToken cancellationToken = default)
        {
            var sql = $"ALTER TABLE {tableName} ADD PARTITION {partitionExpr}";
            await ExecuteNonQueryAsync(sql, cancellationToken);
        }
        
        /// <summary>
        /// 创建物化视图
        /// </summary>
        public async Task CreateMaterializedViewAsync(string viewName, string selectQuery, string targetTable, CancellationToken cancellationToken = default)
        {
            var sql = $"CREATE MATERIALIZED VIEW {viewName} TO {targetTable} AS {selectQuery}";
            await ExecuteNonQueryAsync(sql, cancellationToken);
        }
        private readonly ClickHouseOptions _options;
        private readonly IAsyncPolicy _retryPolicy;
        private readonly IAsyncPolicy _circuitBreakerPolicy;
        private readonly ArrayPool<byte> _memoryPool;
        private readonly Channel<BulkInsertItem> _bulkInsertChannel;
        private readonly Task _bulkInsertProcessor;
        private readonly CancellationTokenSource _cts = new();
        private int _activeConnections;
        private int _failedQueries;
        private int _successfulQueries;
        private int _distributedQueries;
        private int _bulkInsertOperations;

        public ClickHouseService(IOptions<ClickHouseOptions> options)
        {
            _options = options.Value;
            
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(_options.MaxRetryCount, attempt => _options.RetryDelay);
                
            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(_options.CircuitBreakerThreshold, _options.CircuitBreakerDuration);
                
            _memoryPool = ArrayPool<byte>.Shared;
            
            _bulkInsertChannel = Channel.CreateBounded<BulkInsertItem>(
                new BoundedChannelOptions(_options.BulkInsertChannelCapacity)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = true,
                    SingleWriter = false
                });
                
            _bulkInsertProcessor = ProcessBulkInsertsAsync(_cts.Token);
        }

        public async Task ExecuteNonQueryAsync(string sql, CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _activeConnections);
            try
            {
                await _circuitBreakerPolicy.WrapAsync(_retryPolicy)
                    .ExecuteAsync(async ct =>
                    {
                        using var connection = new ClickHouseConnection(_options.ConnectionString);
                        await connection.OpenAsync(ct);
                        using var command = connection.CreateCommand();
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync(ct);
                    }, cancellationToken);
                
                Interlocked.Increment(ref _successfulQueries);
            }
            catch
            {
                Interlocked.Increment(ref _failedQueries);
                throw;
            }
            finally
            {
                Interlocked.Decrement(ref _activeConnections);
            }
        }

        public async Task<DataTable> ExecuteQueryAsync(string sql, CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _activeConnections);
            try
            {
                var result = await _circuitBreakerPolicy.WrapAsync(_retryPolicy)
                    .ExecuteAsync(async ct =>
                    {
                        using var connection = new ClickHouseConnection(_options.ConnectionString);
                        await connection.OpenAsync(ct);
                        using var command = connection.CreateCommand();
                        command.CommandText = sql;
                        using var reader = await command.ExecuteReaderAsync(ct);
                        var dataTable = new DataTable();
                        dataTable.Load(reader);
                        return dataTable;
                    }, cancellationToken);
                
                Interlocked.Increment(ref _successfulQueries);
                return result;
            }
            catch
            {
                Interlocked.Increment(ref _failedQueries);
                throw;
            }
            finally
            {
                Interlocked.Decrement(ref _activeConnections);
            }
        }

        public async Task BulkInsertAsync<T>(string tableName, IEnumerable<T> data, CancellationToken cancellationToken = default) where T : class
        {
            Interlocked.Increment(ref _activeConnections);
            try
            {
                await _circuitBreakerPolicy.WrapAsync(_retryPolicy)
                    .ExecuteAsync(async ct =>
                    {
                        using var connection = new ClickHouseConnection(_options.ConnectionString);
                        await connection.OpenAsync(ct);
                        using var bulkCopy = new ClickHouseBulkCopy(connection)
                        {
                            DestinationTableName = tableName,
                            BatchSize = _options.BulkInsertBatchSize
                        };
                        await bulkCopy.WriteToServerAsync(data, ct);
                    }, cancellationToken);
                
                Interlocked.Increment(ref _successfulQueries);
            }
            catch
            {
                Interlocked.Increment(ref _failedQueries);
                throw;
            }
            finally
            {
                Interlocked.Decrement(ref _activeConnections);
            }
        }

        public async Task<ChannelWriter<T>> CreateBulkInsertChannel<T>(string tableName, CancellationToken cancellationToken = default) where T : class
        {
            var channel = Channel.CreateBounded<T>(_options.BulkInsertBatchSize * 2);
            _ = Task.Run(async () =>
            {
                var batch = new List<T>(_options.BulkInsertBatchSize);
                
                while (await channel.Reader.WaitToReadAsync(cancellationToken))
                {
                    while (channel.Reader.TryRead(out var item))
                    {
                        batch.Add(item);
                        if (batch.Count >= _options.BulkInsertBatchSize)
                        {
                            await BulkInsertAsync(tableName, batch, cancellationToken);
                            batch.Clear();
                        }
                    }
                }
                
                if (batch.Count > 0)
                {
                    await BulkInsertAsync(tableName, batch, cancellationToken);
                }
            }, cancellationToken);
            
            return channel.Writer;
        }

        public Task ResetCircuitBreakerAsync()
        {
            if (_circuitBreakerPolicy is AsyncCircuitBreakerPolicy cb)
            {
                cb.Reset();
            }
            return Task.CompletedTask;
        }

        public Task<ConnectionStats> GetConnectionStatsAsync()
        {
            return Task.FromResult(new ConnectionStats
            {
                ActiveConnections = _activeConnections,
                SuccessfulQueries = _successfulQueries,
                FailedQueries = _failedQueries
            });
        }

        private async Task ProcessBulkInsertsAsync(CancellationToken cancellationToken)
        {
            await foreach (var item in _bulkInsertChannel.Reader.ReadAllAsync(cancellationToken))
            {
                try
                {
                    await _circuitBreakerPolicy.WrapAsync(_retryPolicy)
                        .ExecuteAsync(async ct =>
                        {
                            using var connection = new ClickHouseConnection(_options.ConnectionString);
                            await connection.OpenAsync(ct);
                            using var bulkCopy = new ClickHouseBulkCopy(connection)
                            {
                                DestinationTableName = item.TableName,
                                BatchSize = _options.BulkInsertBatchSize
                            };
                            await bulkCopy.WriteToServerAsync(item.Data, ct);
                        }, cancellationToken);
                }
                catch
                {
                    // Log error or implement dead letter queue
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _bulkInsertChannel.Writer.Complete();
            _bulkInsertProcessor.Wait();
            _cts.Dispose();
        }

        private class BulkInsertItem
        {
            public string TableName { get; set; }
            public IEnumerable<object> Data { get; set; }
        }
    }

    public class ConnectionStats
    {
        /// <summary>
        /// 当前活跃连接数
        /// </summary>
        public int ActiveConnections { get; set; }
        
        /// <summary>
        /// 成功查询数
        /// </summary>
        public int SuccessfulQueries { get; set; }
        
        /// <summary>
        /// 失败查询数
        /// </summary>
        public int FailedQueries { get; set; }
        
        /// <summary>
        /// 分布式查询执行次数
        /// </summary>
        public int DistributedQueries { get; set; }
        
        /// <summary>
        /// 批量插入操作次数
        /// </summary>
        public int BulkInsertOperations { get; set; }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddClickHouseService(this IServiceCollection services, Action<ClickHouseOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddSingleton<IClickHouseService, ClickHouseService>();
            return services;
        }
    }
}