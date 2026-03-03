#!/usr/bin/env dotnet
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
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using ClickHouse.Client.ADO;
using ClickHouse.Client.Copy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using System.Buffers;
using System.Threading.Channels;

namespace ClickHouseIntegration
{
    /// <summary>
    /// ClickHouse配置选项
    /// </summary>
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

    /// <summary>
    /// ClickHouse操作结果
    /// </summary>
    public class ClickHouseResult
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 执行结果数据
        /// </summary>
        public object? ResultData { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// 执行时间（毫秒）
        /// </summary>
        public long ExecutionTimeMs { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string? OperationType { get; set; }

        /// <summary>
        /// 受影响的行数
        /// </summary>
        public long RowsAffected { get; set; }
    }

    /// <summary>
    /// 带泛型结果的ClickHouse操作结果
    /// </summary>
    /// <typeparam name="T">结果类型</typeparam>
    public class ClickHouseResult<T> : ClickHouseResult
    {
        /// <summary>
        /// 泛型结果数据
        /// </summary>
        public new T? ResultData { get; set; }
    }

    /// <summary>
    /// ClickHouse服务接口
    /// </summary>
    public interface IClickHouseService
    {
        /// <summary>
        /// 执行查询并返回单个结果
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>查询结果</returns>
        Task<ClickHouseResult<T>> ExecuteScalarAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行查询并返回结果集
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>结果集</returns>
        Task<ClickHouseResult<List<T>>> ExecuteQueryAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行非查询操作
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>受影响的行数</returns>
        Task<ClickHouseResult<long>> ExecuteNonQueryAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="data">数据列表</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>插入结果</returns>
        Task<ClickHouseResult<long>> BulkInsertAsync<T>(string tableName, List<T> data, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步批量插入数据（使用通道）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="dataChannel">数据通道</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>插入结果</returns>
        Task<ClickHouseResult<long>> BulkInsertAsync<T>(string tableName, Channel<T> dataChannel, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取ClickHouse连接
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>ClickHouse连接</returns>
        Task<ClickHouseConnection> GetConnectionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 创建分布式表
        /// </summary>
        /// <param name="localTableName">本地表名</param>
        /// <param name="distributedTableName">分布式表名</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        Task<ClickHouseResult> CreateDistributedTableAsync(string localTableName, string distributedTableName, CancellationToken cancellationToken = default);

        /// <summary>
        /// 优化表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="partitionClause">分区子句</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        Task<ClickHouseResult> OptimizeTableAsync(string tableName, string? partitionClause = null, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// ClickHouse服务实现
    /// </summary>
    public class ClickHouseService : IClickHouseService
    {
        private readonly ClickHouseOptions _options;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">配置选项</param>
        public ClickHouseService(IOptions<ClickHouseOptions> options)
        {
            _options = options.Value;

            // 配置重试策略
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: _options.MaxRetryCount,
                    sleepDurationProvider: attempt => _options.RetryDelay * Math.Pow(2, attempt - 1),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"ClickHouse操作重试 {retryCount}，等待 {timeSpan.TotalMilliseconds}ms: {exception.Message}");
                    });

            // 配置断路器策略
            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: _options.CircuitBreakerThreshold,
                    durationOfBreak: _options.CircuitBreakerDuration,
                    onBreak: (exception, breakDuration) =>
                    {
                        Console.WriteLine($"ClickHouse断路器打开，持续 {breakDuration.TotalSeconds}s: {exception.Message}");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("ClickHouse断路器关闭");
                    });
        }

        /// <summary>
        /// 创建ClickHouse连接
        /// </summary>
        /// <returns>ClickHouse连接</returns>
        private ClickHouseConnection CreateConnection()
        {
            var connection = new ClickHouseConnection(_options.ConnectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// 执行查询并返回单个结果
        /// </summary>
        public async Task<ClickHouseResult<T>> ExecuteScalarAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ClickHouseResult<T> { OperationType = "ExecuteScalar" };

            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    await _circuitBreakerPolicy.ExecuteAsync(async () =>
                    {
                        using var connection = CreateConnection();
                        using var command = connection.CreateCommand();
                        command.CommandText = sql;
                        command.CommandTimeout = (int)_options.QueryTimeout.TotalSeconds;

                        // 添加参数
                        if (parameters != null)
                        {
                            foreach (var property in parameters.GetType().GetProperties())
                            {
                                var parameter = command.CreateParameter();
                                parameter.ParameterName = property.Name;
                                parameter.Value = property.GetValue(parameters) ?? DBNull.Value;
                                command.Parameters.Add(parameter);
                            }
                        }

                        result.ResultData = (T)await command.ExecuteScalarAsync(cancellationToken);
                        result.Success = true;
                        result.RowsAffected = 1;
                    });
                });
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                Console.WriteLine($"ClickHouse标量查询失败: {sql}\n{ex.Message}");
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <summary>
        /// 执行查询并返回结果集
        /// </summary>
        public async Task<ClickHouseResult<List<T>>> ExecuteQueryAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ClickHouseResult<List<T>> { OperationType = "ExecuteQuery" };

            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    await _circuitBreakerPolicy.ExecuteAsync(async () =>
                    {
                        using var connection = CreateConnection();
                        using var command = connection.CreateCommand();
                        command.CommandText = sql;
                        command.CommandTimeout = (int)_options.QueryTimeout.TotalSeconds;

                        // 添加参数
                        if (parameters != null)
                        {
                            foreach (var property in parameters.GetType().GetProperties())
                            {
                                var parameter = command.CreateParameter();
                                parameter.ParameterName = property.Name;
                                parameter.Value = property.GetValue(parameters) ?? DBNull.Value;
                                command.Parameters.Add(parameter);
                            }
                        }

                        using var reader = await command.ExecuteReaderAsync(cancellationToken);
                        var data = new List<T>();

                        while (await reader.ReadAsync(cancellationToken))
                        {
                            // 这里需要根据T的类型进行适当的映射
                            // 简化实现，实际应用中可能需要更复杂的映射逻辑
                            var item = (T)Activator.CreateInstance(typeof(T));
                            data.Add(item);
                        }

                        result.ResultData = data;
                        result.Success = true;
                        result.RowsAffected = data.Count;
                    });
                });
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                Console.WriteLine($"ClickHouse查询失败: {sql}\n{ex.Message}");
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <summary>
        /// 执行非查询操作
        /// </summary>
        public async Task<ClickHouseResult<long>> ExecuteNonQueryAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ClickHouseResult<long> { OperationType = "ExecuteNonQuery" };

            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    await _circuitBreakerPolicy.ExecuteAsync(async () =>
                    {
                        using var connection = CreateConnection();
                        using var command = connection.CreateCommand();
                        command.CommandText = sql;
                        command.CommandTimeout = (int)_options.QueryTimeout.TotalSeconds;

                        // 添加参数
                        if (parameters != null)
                        {
                            foreach (var property in parameters.GetType().GetProperties())
                            {
                                var parameter = command.CreateParameter();
                                parameter.ParameterName = property.Name;
                                parameter.Value = property.GetValue(parameters) ?? DBNull.Value;
                                command.Parameters.Add(parameter);
                            }
                        }

                        var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
                        result.ResultData = rowsAffected;
                        result.Success = true;
                        result.RowsAffected = rowsAffected;
                    });
                });
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                Console.WriteLine($"ClickHouse非查询操作失败: {sql}\n{ex.Message}");
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        public async Task<ClickHouseResult<long>> BulkInsertAsync<T>(string tableName, List<T> data, CancellationToken cancellationToken = default)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ClickHouseResult<long> { OperationType = "BulkInsert" };

            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    await _circuitBreakerPolicy.ExecuteAsync(async () =>
                    {
                        using var connection = CreateConnection();
                        using var bulkCopy = new ClickHouseBulkCopy(connection)
                        {
                            DestinationTableName = tableName,
                            BatchSize = _options.BulkInsertBatchSize
                        };

                        var rowsInserted = await bulkCopy.WriteToServerAsync(data, cancellationToken);
                        result.ResultData = rowsInserted;
                        result.Success = true;
                        result.RowsAffected = rowsInserted;
                    });
                });
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                Console.WriteLine($"ClickHouse批量插入失败: {tableName}\n{ex.Message}");
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <summary>
        /// 异步批量插入数据（使用通道）
        /// </summary>
        public async Task<ClickHouseResult<long>> BulkInsertAsync<T>(string tableName, Channel<T> dataChannel, CancellationToken cancellationToken = default)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ClickHouseResult<long> { OperationType = "BulkInsertChannel" };

            try
            {
                long totalRowsInserted = 0;
                var batch = new List<T>(_options.BulkInsertBatchSize);

                await foreach (var item in dataChannel.Reader.ReadAllAsync(cancellationToken))
                {
                    batch.Add(item);

                    if (batch.Count >= _options.BulkInsertBatchSize)
                    {
                        var insertResult = await BulkInsertAsync(tableName, batch, cancellationToken);
                        if (insertResult.Success)
                        {
                            totalRowsInserted += insertResult.RowsAffected;
                        }
                        batch.Clear();
                    }
                }

                // 插入剩余数据
                if (batch.Count > 0)
                {
                    var insertResult = await BulkInsertAsync(tableName, batch, cancellationToken);
                    if (insertResult.Success)
                    {
                        totalRowsInserted += insertResult.RowsAffected;
                    }
                }

                result.ResultData = totalRowsInserted;
                result.Success = true;
                result.RowsAffected = totalRowsInserted;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                Console.WriteLine($"ClickHouse通道批量插入失败: {tableName}\n{ex.Message}");
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <summary>
        /// 获取ClickHouse连接
        /// </summary>
        public async Task<ClickHouseConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(CreateConnection());
        }

        /// <summary>
        /// 创建分布式表
        /// </summary>
        public async Task<ClickHouseResult> CreateDistributedTableAsync(string localTableName, string distributedTableName, CancellationToken cancellationToken = default)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ClickHouseResult { OperationType = "CreateDistributedTable" };

            try
            {
                var sql = $"CREATE TABLE IF NOT EXISTS {distributedTableName} AS {localTableName} DISTRIBUTED BY HASH(id) TO cluster('{_options.DefaultClusterName}', '{localTableName}')";
                var executeResult = await ExecuteNonQueryAsync(sql, null, cancellationToken);
                result.Success = executeResult.Success;
                result.ErrorMessage = executeResult.ErrorMessage;
                result.RowsAffected = executeResult.RowsAffected;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                Console.WriteLine($"创建分布式表失败: {localTableName} → {distributedTableName}\n{ex.Message}");
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <summary>
        /// 优化表
        /// </summary>
        public async Task<ClickHouseResult> OptimizeTableAsync(string tableName, string? partitionClause = null, CancellationToken cancellationToken = default)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ClickHouseResult { OperationType = "OptimizeTable" };

            try
            {
                var sql = $"OPTIMIZE TABLE {tableName} {(partitionClause != null ? $"PARTITION {partitionClause}" : "")} FINAL";
                var executeResult = await ExecuteNonQueryAsync(sql, null, cancellationToken);
                result.Success = executeResult.Success;
                result.ErrorMessage = executeResult.ErrorMessage;
                result.RowsAffected = executeResult.RowsAffected;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                Console.WriteLine($"优化表失败: {tableName}\n{ex.Message}");
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }
    }

    /// <summary>
    /// ClickHouse服务扩展
    /// </summary>
    public static class ClickHouseServiceExtensions
    {
        /// <summary>
        /// 添加ClickHouse服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddClickHouseService(
            this IServiceCollection services,
            Action<ClickHouseOptions> configureOptions = null)
        {
            services.Configure<ClickHouseOptions>(options =>
            {
                configureOptions?.Invoke(options);
            });

            services.AddSingleton<IClickHouseService, ClickHouseService>();

            return services;
        }
    }

    /// <summary>
    /// ClickHouse控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ClickHouseController : ControllerBase
    {
        private readonly IClickHouseService _clickHouseService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="clickHouseService">ClickHouse服务</param>
        public ClickHouseController(IClickHouseService clickHouseService)
        {
            _clickHouseService = clickHouseService;
        }

        /// <summary>
        /// 测试ClickHouse连接
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>测试结果</returns>
        [HttpGet("test")]
        public async Task<IActionResult> TestConnection(CancellationToken cancellationToken = default)
        {
            var result = await _clickHouseService.ExecuteScalarAsync<string>("SELECT version()", cancellationToken: cancellationToken);
            if (result.Success)
            {
                return Ok(new { version = result.ResultData });
            }
            else
            {
                return StatusCode(500, new { error = result.ErrorMessage });
            }
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="request">查询请求</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>查询结果</returns>
        [HttpPost("query")]
        public async Task<IActionResult> ExecuteQuery([FromBody] QueryRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _clickHouseService.ExecuteQueryAsync<dynamic>(request.Sql, request.Parameters, cancellationToken);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="request">批量插入请求</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>插入结果</returns>
        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsert([FromBody] BulkInsertRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _clickHouseService.BulkInsertAsync(request.TableName, request.Data, cancellationToken);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(500, result);
            }
        }
    }

    /// <summary>
    /// 查询请求
    /// </summary>
    public class QueryRequest
    {
        /// <summary>
        /// SQL语句
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public object? Parameters { get; set; }
    }

    /// <summary>
    /// 批量插入请求
    /// </summary>
    public class BulkInsertRequest
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public List<dynamic> Data { get; set; }
    }
}
