#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.Data.Sqlite@8.0.0
#:package Dapper@2.1.35
#:package MySqlConnector@2.3.5
#:package Npgsql@8.0.3
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Db.AOT
{
    /// <summary>
    /// 数据库类型枚举
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// SQLite数据库
        /// </summary>
        Sqlite,
        /// <summary>
        /// MySQL数据库
        /// </summary>
        MySql,
        /// <summary>
        /// PostgreSQL数据库
        /// </summary>
        PostgreSql
    }
    
    /// <summary>
    /// 数据库配置选项
    /// </summary>
    public class DatabaseOptions
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType DatabaseType { get; set; } = DatabaseType.Sqlite;
        
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; } = "Data Source=app.db"; // 默认SQLite连接字符串
        
        /// <summary>
        /// 是否启用连接池
        /// </summary>
        public bool EnableConnectionPooling { get; set; } = true;
        
        /// <summary>
        /// 最大连接池大小
        /// </summary>
        public int MaxPoolSize { get; set; } = 100;
        
        /// <summary>
        /// 连接超时时间（秒）
        /// </summary>
        public int ConnectionTimeout { get; set; } = 30;
        
        /// <summary>
        /// 命令超时时间（秒）
        /// </summary>
        public int CommandTimeout { get; set; } = 30;
        
        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
        
        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;
        
        /// <summary>
        /// 批量操作大小
        /// </summary>
        public int BatchSize { get; set; } = 1000;
        
        /// <summary>
        /// 是否启用事务支持
        /// </summary>
        public bool EnableTransactions { get; set; } = true;
    }
    
    /// <summary>
    /// 数据库操作结果
    /// </summary>
    public class DatabaseResult
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
        public int RowsAffected { get; set; }
    }
    
    /// <summary>
    /// 带泛型结果的数据库操作结果
    /// </summary>
    /// <typeparam name="T">结果类型</typeparam>
    public class DatabaseResult<T> : DatabaseResult
    {
        /// <summary>
        /// 泛型结果数据
        /// </summary>
        public new T? ResultData { get; set; }
    }
    
    /// <summary>
    /// 数据库状态信息
    /// </summary>
    public class DatabaseStatus
    {
        /// <summary>
        /// 服务是否正常运行
        /// </summary>
        public bool IsRunning { get; set; }
        
        /// <summary>
        /// 已处理的请求数
        /// </summary>
        public long ProcessedRequests { get; set; }
        
        /// <summary>
        /// 成功处理的请求数
        /// </summary>
        public long SuccessfulRequests { get; set; }
        
        /// <summary>
        /// 失败处理的请求数
        /// </summary>
        public long FailedRequests { get; set; }
        
        /// <summary>
        /// 平均执行时间（毫秒）
        /// </summary>
        public double AverageExecutionTimeMs { get; set; }
        
        /// <summary>
        /// 服务启动时间
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType DatabaseType { get; set; }
        
        /// <summary>
        /// 连接字符串（部分隐藏）
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;
        
        /// <summary>
        /// 当前活动连接数
        /// </summary>
        public int ActiveConnections { get; set; }
        
        /// <summary>
        /// 最大连接池大小
        /// </summary>
        public int MaxPoolSize { get; set; }
    }
    
    /// <summary>
    /// 数据库服务接口
    /// 定义了数据库的核心功能
    /// </summary>
    public interface IDatabaseService
    {
        /// <summary>
        /// 执行查询并返回单个结果
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>查询结果</returns>
        Task<DatabaseResult<T>> ExecuteScalarAsync<T>(string sql, object? parameters = null);
        
        /// <summary>
        /// 执行查询并返回结果集
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>结果集</returns>
        Task<DatabaseResult<List<T>>> ExecuteQueryAsync<T>(string sql, object? parameters = null);
        
        /// <summary>
        /// 执行非查询操作
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>受影响的行数</returns>
        Task<DatabaseResult<int>> ExecuteNonQueryAsync(string sql, object? parameters = null);
        
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameters">参数</param>
        /// <returns>结果集</returns>
        Task<DatabaseResult<List<T>>> ExecuteStoredProcedureAsync<T>(string procedureName, object? parameters = null);
        
        /// <summary>
        /// 执行事务中的多个操作
        /// </summary>
        /// <param name="actions">事务中的操作</param>
        /// <returns>操作结果</returns>
        Task<DatabaseResult> ExecuteTransactionAsync(List<Func<IDbConnection, Task<bool>>> actions);
        
        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="data">数据列表</param>
        /// <returns>插入结果</returns>
        Task<DatabaseResult<int>> BulkInsertAsync<T>(string tableName, List<T> data);
        
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns>数据库连接</returns>
        Task<DbConnection> GetConnectionAsync();
        
        /// <summary>
        /// 获取数据库状态
        /// </summary>
        /// <returns>状态信息</returns>
        Task<DatabaseStatus> GetStatusAsync();
        
        /// <summary>
        /// 重置数据库状态
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> ResetStatusAsync();
    }
    
    /// <summary>
    /// 数据库服务实现
    /// 基于.NET 10 AOT架构，提供高性能数据库功能
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        private readonly ILogger<DatabaseService> _logger;
        private readonly DatabaseOptions _options;
        private long _processedRequests = 0;
        private long _successfulRequests = 0;
        private long _failedRequests = 0;
        private long _totalExecutionTime = 0;
        private int _activeConnections = 0;
        private readonly DateTime _startTime = DateTime.UtcNow;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="options">配置选项</param>
        public DatabaseService(ILogger<DatabaseService> logger, IOptions<DatabaseOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            
            _logger.LogInformation("DatabaseService初始化成功，配置选项：DatabaseType={DatabaseType}, MaxPoolSize={MaxPoolSize}, EnableConnectionPooling={EnableConnectionPooling}",
                _options.DatabaseType, _options.MaxPoolSize, _options.EnableConnectionPooling);
        }
        
        /// <summary>
        /// 创建数据库连接
        /// </summary>
        /// <returns>数据库连接</returns>
        private DbConnection CreateConnection()
        {
            DbConnection connection = _options.DatabaseType switch
            {
                DatabaseType.Sqlite => new SqliteConnection(_options.ConnectionString),
                // 可以根据需要添加其他数据库类型的支持
                _ => new SqliteConnection(_options.ConnectionString)
            };
            
            return connection;
        }
        
        /// <summary>
        /// 执行查询并返回单个结果
        /// </summary>
        public async Task<DatabaseResult<T>> ExecuteScalarAsync<T>(string sql, object? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DatabaseResult<T> { OperationType = "ExecuteScalar" };
            
            try
            {
                Interlocked.Increment(ref _processedRequests);
                Interlocked.Increment(ref _activeConnections);
                
                _logger.LogDebug("执行标量查询：{Sql}, 参数：{Parameters}", sql, parameters);
                
                using var connection = CreateConnection();
                await connection.OpenAsync();
                
                var scalarResult = await connection.ExecuteScalarAsync<T>(sql, parameters, commandTimeout: _options.CommandTimeout);
                
                result.Success = true;
                result.ResultData = scalarResult;
                result.RowsAffected = 1;
                
                Interlocked.Increment(ref _successfulRequests);
                
                _logger.LogDebug("标量查询完成，结果：{Result}", scalarResult);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedRequests);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "标量查询失败：{Sql}", sql);
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                Interlocked.Add(ref _totalExecutionTime, result.ExecutionTimeMs);
                Interlocked.Decrement(ref _activeConnections);
            }
            
            return result;
        }
        
        /// <summary>
        /// 执行查询并返回结果集
        /// </summary>
        public async Task<DatabaseResult<List<T>>> ExecuteQueryAsync<T>(string sql, object? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DatabaseResult<List<T>> { OperationType = "ExecuteQuery" };
            
            try
            {
                Interlocked.Increment(ref _processedRequests);
                Interlocked.Increment(ref _activeConnections);
                
                _logger.LogDebug("执行查询：{Sql}, 参数：{Parameters}", sql, parameters);
                
                using var connection = CreateConnection();
                await connection.OpenAsync();
                
                var queryResult = await connection.QueryAsync<T>(sql, parameters, commandTimeout: _options.CommandTimeout);
                var resultList = queryResult.ToList();
                
                result.Success = true;
                result.ResultData = resultList;
                result.RowsAffected = resultList.Count;
                
                Interlocked.Increment(ref _successfulRequests);
                
                _logger.LogDebug("查询完成，返回行数：{RowCount}", resultList.Count);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedRequests);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "查询失败：{Sql}", sql);
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                Interlocked.Add(ref _totalExecutionTime, result.ExecutionTimeMs);
                Interlocked.Decrement(ref _activeConnections);
            }
            
            return result;
        }
        
        /// <summary>
        /// 执行非查询操作
        /// </summary>
        public async Task<DatabaseResult<int>> ExecuteNonQueryAsync(string sql, object? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DatabaseResult<int> { OperationType = "ExecuteNonQuery" };
            
            try
            {
                Interlocked.Increment(ref _processedRequests);
                Interlocked.Increment(ref _activeConnections);
                
                _logger.LogDebug("执行非查询操作：{Sql}, 参数：{Parameters}", sql, parameters);
                
                using var connection = CreateConnection();
                await connection.OpenAsync();
                
                var rowsAffected = await connection.ExecuteAsync(sql, parameters, commandTimeout: _options.CommandTimeout);
                
                result.Success = true;
                result.ResultData = rowsAffected;
                result.RowsAffected = rowsAffected;
                
                Interlocked.Increment(ref _successfulRequests);
                
                _logger.LogDebug("非查询操作完成，受影响行数：{RowCount}", rowsAffected);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedRequests);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "非查询操作失败：{Sql}", sql);
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                Interlocked.Add(ref _totalExecutionTime, result.ExecutionTimeMs);
                Interlocked.Decrement(ref _activeConnections);
            }
            
            return result;
        }
        
        /// <summary>
        /// 执行存储过程
        /// </summary>
        public async Task<DatabaseResult<List<T>>> ExecuteStoredProcedureAsync<T>(string procedureName, object? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DatabaseResult<List<T>> { OperationType = "ExecuteStoredProcedure" };
            
            try
            {
                Interlocked.Increment(ref _processedRequests);
                Interlocked.Increment(ref _activeConnections);
                
                _logger.LogDebug("执行存储过程：{ProcedureName}, 参数：{Parameters}", procedureName, parameters);
                
                using var connection = CreateConnection();
                await connection.OpenAsync();
                
                var procResult = await connection.QueryAsync<T>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: _options.CommandTimeout);
                
                var resultList = procResult.ToList();
                
                result.Success = true;
                result.ResultData = resultList;
                result.RowsAffected = resultList.Count;
                
                Interlocked.Increment(ref _successfulRequests);
                
                _logger.LogDebug("存储过程执行完成，返回行数：{RowCount}", resultList.Count);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedRequests);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "存储过程执行失败：{ProcedureName}", procedureName);
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                Interlocked.Add(ref _totalExecutionTime, result.ExecutionTimeMs);
                Interlocked.Decrement(ref _activeConnections);
            }
            
            return result;
        }
        
        /// <summary>
        /// 执行事务中的多个操作
        /// </summary>
        public async Task<DatabaseResult> ExecuteTransactionAsync(List<Func<IDbConnection, Task<bool>>> actions)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DatabaseResult { OperationType = "ExecuteTransaction" };
            
            try
            {
                Interlocked.Increment(ref _processedRequests);
                Interlocked.Increment(ref _activeConnections);
                
                _logger.LogDebug("开始事务，操作数：{ActionCount}", actions.Count);
                
                using var connection = CreateConnection();
                await connection.OpenAsync();
                using var transaction = await connection.BeginTransactionAsync();
                
                try
                {
                    int totalRowsAffected = 0;
                    
                    foreach (var action in actions)
                    {
                        if (!await action(connection))
                        {
                            await transaction.RollbackAsync();
                            result.Success = false;
                            result.ErrorMessage = "事务中的操作失败，已回滚";
                            return result;
                        }
                    }
                    
                    await transaction.CommitAsync();
                    
                    result.Success = true;
                    result.RowsAffected = totalRowsAffected;
                    
                    Interlocked.Increment(ref _successfulRequests);
                    
                    _logger.LogDebug("事务提交成功");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedRequests);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "事务执行失败");
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                Interlocked.Add(ref _totalExecutionTime, result.ExecutionTimeMs);
                Interlocked.Decrement(ref _activeConnections);
            }
            
            return result;
        }
        
        /// <summary>
        /// 批量插入数据
        /// </summary>
        public async Task<DatabaseResult<int>> BulkInsertAsync<T>(string tableName, List<T> data)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DatabaseResult<int> { OperationType = "BulkInsert" };
            
            try
            {
                Interlocked.Increment(ref _processedRequests);
                Interlocked.Increment(ref _activeConnections);
                
                _logger.LogDebug("开始批量插入，表名：{TableName}, 数据量：{DataCount}", tableName, data.Count);
                
                using var connection = CreateConnection();
                await connection.OpenAsync();
                
                // 简单实现，实际生产环境中可以使用更高效的批量插入方式
                int rowsInserted = 0;
                int batchSize = _options.BatchSize;
                
                for (int i = 0; i < data.Count; i += batchSize)
                {
                    var batch = data.Skip(i).Take(batchSize).ToList();
                    rowsInserted += await connection.ExecuteAsync(
                        GenerateInsertSql(tableName, batch.First()),
                        batch,
                        commandTimeout: _options.CommandTimeout);
                }
                
                result.Success = true;
                result.ResultData = rowsInserted;
                result.RowsAffected = rowsInserted;
                
                Interlocked.Increment(ref _successfulRequests);
                
                _logger.LogDebug("批量插入完成，插入行数：{RowCount}", rowsInserted);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedRequests);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "批量插入失败，表名：{TableName}", tableName);
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                Interlocked.Add(ref _totalExecutionTime, result.ExecutionTimeMs);
                Interlocked.Decrement(ref _activeConnections);
            }
            
            return result;
        }
        
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        public async Task<DbConnection> GetConnectionAsync()
        {
            Interlocked.Increment(ref _activeConnections);
            
            var connection = CreateConnection();
            await connection.OpenAsync();
            
            return connection;
        }
        
        /// <summary>
        /// 获取数据库状态
        /// </summary>
        public async Task<DatabaseStatus> GetStatusAsync()
        {
            await Task.CompletedTask; // 模拟异步操作
            
            var requestCount = Interlocked.Read(ref _processedRequests);
            var averageTime = requestCount > 0 ? (double)Interlocked.Read(ref _totalExecutionTime) / requestCount : 0;
            
            // 隐藏连接字符串中的敏感信息
            var maskedConnectionString = _options.ConnectionString;
            if (_options.DatabaseType == DatabaseType.Sqlite)
            {
                // SQLite连接字符串不需要特别处理
            }
            else
            {
                // 简单处理：替换密码部分
                var parts = maskedConnectionString.Split(';');
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].StartsWith("Password=", StringComparison.OrdinalIgnoreCase))
                    {
                        parts[i] = "Password=***";
                    }
                    else if (parts[i].StartsWith("User ID=", StringComparison.OrdinalIgnoreCase))
                    {
                        parts[i] = "User ID=***";
                    }
                }
                maskedConnectionString = string.Join(";", parts);
            }
            
            var status = new DatabaseStatus
            {
                IsRunning = true,
                ProcessedRequests = requestCount,
                SuccessfulRequests = Interlocked.Read(ref _successfulRequests),
                FailedRequests = Interlocked.Read(ref _failedRequests),
                AverageExecutionTimeMs = Math.Round(averageTime, 2),
                StartTime = _startTime,
                DatabaseType = _options.DatabaseType,
                ConnectionString = maskedConnectionString,
                ActiveConnections = Interlocked.CompareExchange(ref _activeConnections, 0, 0),
                MaxPoolSize = _options.MaxPoolSize
            };
            
            _logger.LogDebug("获取数据库状态: {@Status}", status);
            
            return status;
        }
        
        /// <summary>
        /// 重置数据库状态
        /// </summary>
        public async Task<bool> ResetStatusAsync()
        {
            await Task.CompletedTask; // 模拟异步操作
            
            try
            {
                Interlocked.Exchange(ref _processedRequests, 0);
                Interlocked.Exchange(ref _successfulRequests, 0);
                Interlocked.Exchange(ref _failedRequests, 0);
                Interlocked.Exchange(ref _totalExecutionTime, 0);
                
                _logger.LogInformation("数据库状态已重置");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "重置数据库状态失败");
                return false;
            }
        }
        
        /// <summary>
        /// 生成插入SQL语句
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="sampleData">示例数据</param>
        /// <returns>插入SQL语句</returns>
        private string GenerateInsertSql<T>(string tableName, T sampleData)
        {
            var properties = typeof(T).GetProperties();
            var columns = string.Join(", ", properties.Select(p => p.Name));
            var values = string.Join(", ", properties.Select(p => $"@{p.Name}"));
            
            return $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
        }
    }
    
    /// <summary>
    /// 数据库AOT执行引擎
    /// 管理数据库功能调用
    /// </summary>
    public class DatabaseAotEngine
    {
        private readonly ILogger<DatabaseAotEngine> _logger;
        private readonly IDatabaseService _databaseService;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="databaseService">数据库服务</param>
        public DatabaseAotEngine(ILogger<DatabaseAotEngine> logger, IDatabaseService databaseService)
        {
            _logger = logger;
            _databaseService = databaseService;
            
            _logger.LogInformation("DatabaseAotEngine初始化成功");
        }
        
        /// <summary>
        /// 执行查询并返回单个结果
        /// </summary>
        public async Task<DatabaseResult<T>> ExecuteScalarAsync<T>(string sql, object? parameters = null)
        {
            return await _databaseService.ExecuteScalarAsync<T>(sql, parameters);
        }
        
        /// <summary>
        /// 执行查询并返回结果集
        /// </summary>
        public async Task<DatabaseResult<List<T>>> ExecuteQueryAsync<T>(string sql, object? parameters = null)
        {
            return await _databaseService.ExecuteQueryAsync<T>(sql, parameters);
        }
        
        /// <summary>
        /// 执行非查询操作
        /// </summary>
        public async Task<DatabaseResult<int>> ExecuteNonQueryAsync(string sql, object? parameters = null)
        {
            return await _databaseService.ExecuteNonQueryAsync(sql, parameters);
        }
        
        /// <summary>
        /// 执行存储过程
        /// </summary>
        public async Task<DatabaseResult<List<T>>> ExecuteStoredProcedureAsync<T>(string procedureName, object? parameters = null)
        {
            return await _databaseService.ExecuteStoredProcedureAsync<T>(procedureName, parameters);
        }
        
        /// <summary>
        /// 执行事务中的多个操作
        /// </summary>
        public async Task<DatabaseResult> ExecuteTransactionAsync(List<Func<IDbConnection, Task<bool>>> actions)
        {
            return await _databaseService.ExecuteTransactionAsync(actions);
        }
        
        /// <summary>
        /// 批量插入数据
        /// </summary>
        public async Task<DatabaseResult<int>> BulkInsertAsync<T>(string tableName, List<T> data)
        {
            return await _databaseService.BulkInsertAsync(tableName, data);
        }
        
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        public async Task<DbConnection> GetConnectionAsync()
        {
            return await _databaseService.GetConnectionAsync();
        }
        
        /// <summary>
        /// 获取数据库状态
        /// </summary>
        public async Task<DatabaseStatus> GetStatusAsync()
        {
            return await _databaseService.GetStatusAsync();
        }
        
        /// <summary>
        /// 重置数据库状态
        /// </summary>
        public async Task<bool> ResetStatusAsync()
        {
            return await _databaseService.ResetStatusAsync();
        }
    }
    
    /// <summary>
    /// 数据库扩展
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// 注册数据库服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddSingleton<DatabaseAotEngine>();
            
            return services;
        }
        
        /// <summary>
        /// 注册数据库服务并配置选项
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configureOptions">配置选项的委托</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddDatabase(this IServiceCollection services, Action<DatabaseOptions> configureOptions)
        {
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }
            
            services.Configure(configureOptions);
            services.AddDatabase();
            
            return services;
        }
    }
    
    /// <summary>
    /// 主程序
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 主入口点
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码</returns>
        public static async Task<int> Main(string[] args)
        {
            // 构建主机
            var builder = Host.CreateApplicationBuilder(args);
            
            // 配置数据库选项
            builder.Configuration.AddJsonFile("db_aot.setting.json", optional: true);
            builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));
            
            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);
            
            // 注册服务
            builder.Services.AddDatabase();
            
            // 构建主机
            var host = builder.Build();
            var serviceProvider = host.Services;
            
            // 获取引擎实例
            var engine = serviceProvider.GetRequiredService<DatabaseAotEngine>();
            
            // 解析命令行参数
            if (args.Length < 1)
            {
                Console.WriteLine("用法:");
                Console.WriteLine("  db_aot.exe <command> [arguments]");
                Console.WriteLine("  ");
                Console.WriteLine("命令:");
                Console.WriteLine("  status    获取数据库服务状态");
                Console.WriteLine("  reset     重置数据库服务状态");
                Console.WriteLine("  demo      运行数据库演示");
                Console.WriteLine("  ");
                Console.WriteLine("示例:");
                Console.WriteLine("  db_aot.exe status");
                Console.WriteLine("  db_aot.exe reset");
                Console.WriteLine("  db_aot.exe demo");
                return 1;
            }
            
            try
            {
                string command = args[0].ToLower();
                
                switch (command)
                {
                    case "status":
                        var status = await engine.GetStatusAsync();
                        Console.WriteLine("数据库服务状态:");
                        Console.WriteLine($"  运行状态: {status.IsRunning ? "正常" : "异常"}");
                        Console.WriteLine($"  数据库类型: {status.DatabaseType}");
                        Console.WriteLine($"  连接字符串: {status.ConnectionString}");
                        Console.WriteLine($"  已处理请求: {status.ProcessedRequests}");
                        Console.WriteLine($"  成功请求: {status.SuccessfulRequests}");
                        Console.WriteLine($"  失败请求: {status.FailedRequests}");
                        Console.WriteLine($"  平均执行时间: {status.AverageExecutionTimeMs} ms");
                        Console.WriteLine($"  活动连接: {status.ActiveConnections}/{status.MaxPoolSize}");
                        Console.WriteLine($"  服务启动时间: {status.StartTime.ToLocalTime()}");
                        return 0;
                        
                    case "reset":
                        var resetResult = await engine.ResetStatusAsync();
                        Console.WriteLine($"重置服务状态: {(resetResult ? "成功" : "失败"}");
                        return resetResult ? 0 : 1;
                        
                    case "demo":
                        Console.WriteLine("运行数据库演示...");
                        
                        // 创建测试表（如果不存在）
                        Console.WriteLine("\n1. 创建测试表...");
                        var createTableResult = await engine.ExecuteNonQueryAsync(@"CREATE TABLE IF NOT EXISTS TestTable (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name TEXT NOT NULL,
                            Value INTEGER,
                            CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                        )");
                        Console.WriteLine($"   结果: {createTableResult.Success ? "成功" : "失败"}");
                        
                        // 插入测试数据
                        Console.WriteLine("\n2. 插入测试数据...");
                        var insertResult = await engine.ExecuteNonQueryAsync(
                            "INSERT INTO TestTable (Name, Value) VALUES (@Name, @Value)",
                            new { Name = "Test Record", Value = 42 });
                        Console.WriteLine($"   结果: {insertResult.Success ? "成功" : "失败"}");
                        if (insertResult.Success)
                        {
                            Console.WriteLine($"   插入行数: {insertResult.ResultData}");
                        }
                        
                        // 查询测试数据
                        Console.WriteLine("\n3. 查询测试数据...");
                        var queryResult = await engine.ExecuteQueryAsync<dynamic>("SELECT * FROM TestTable");
                        Console.WriteLine($"   结果: {queryResult.Success ? "成功" : "失败"}");
                        if (queryResult.Success && queryResult.ResultData != null)
                        {
                            Console.WriteLine($"   查询到 {queryResult.ResultData.Count} 条记录:");
                            foreach (var record in queryResult.ResultData.Take(5))
                            {
                                Console.WriteLine($"   - Id: {record.Id}, Name: {record.Name}, Value: {record.Value}, CreatedAt: {record.CreatedAt}");
                            }
                        }
                        
                        Console.WriteLine("\n演示完成！");
                        return 0;
                        
                    default:
                        Console.WriteLine($"未知命令: {command}");
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行错误: {ex.Message}");
                return 1;
            }
        }
    }
}