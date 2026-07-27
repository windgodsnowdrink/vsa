#:sdk Microsoft.NET.Sdk.Web
#:package ChoETL@2.3.0
#:package Microsoft.Data.Sqlite@8.0.0
#:package LiteDB@5.0.17
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:package ChoETL@2.0.1
#:package Microsoft.Extensions.Hosting@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ChoETL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using System.Threading;

namespace ChoETLIntegration
{
    /// <summary>
    /// ChoETL配置选项
    /// </summary>
public class ChoETLOptions
{
    /// <summary>
    /// 批量处理大小
    /// </summary>
    public int BatchSize { get; set; } = 1000;
    
    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetryCount { get; set; } = 3;
    
    /// <summary>
    /// 错误日志路径
    /// </summary>
    public string ErrorLogPath { get; set; } = "errors.log";
    
    /// <summary>
    /// 并行处理线程数
    /// </summary>
    public int ParallelDegree { get; set; } = Environment.ProcessorCount;
    
    /// <summary>
    /// SQLite连接字符串
    /// </summary>
    public string SqliteConnectionString { get; set; } = "Data Source=etl.db";
    
    /// <summary>
    /// LiteDB连接字符串
    /// </summary>
    public string LiteDbConnectionString { get; set; } = "etl_lite.db";

        /// <summary>
        /// 批量处理大小
        /// </summary>
        public int BatchSize { get; set; } = 1000;
        
        /// <summary>
        /// 最大重试次数
        /// </summary>
        public int MaxRetryCount { get; set; } = 3;
        
        /// <summary>
        /// 错误日志路径
        /// </summary>
        public string ErrorLogPath { get; set; } = "./Logs/etl_errors.log";
        
        /// <summary>
        /// 是否启用并行处理
        /// </summary>
        public bool EnableParallelProcessing { get; set; } = true;
    }
    
    /// <summary>
    /// ETL服务接口
    /// </summary>
    /// <summary>
/// ETL服务接口
/// </summary>
public interface IETLService
{
    /// <summary>
    /// 处理单个CSV文件
    /// </summary>
    Task ProcessCsvFileAsync(string filePath, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 批量处理CSV文件
    /// </summary>
    Task ProcessCsvFilesAsync(IEnumerable<string> filePaths, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 执行SQL查询
    /// </summary>
    Task<IEnumerable<T>> QueryAsync<T>(string sql, CancellationToken cancellationToken = default) where T : class, new();
    
    /// <summary>
    /// 执行聚合计算
    /// </summary>
    Task<object> AggregateAsync(string sql, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 从LiteDB查询数据
    /// </summary>
    Task<IEnumerable<T>> QueryLiteDbAsync<T>(string collectionName, BsonExpression query, CancellationToken cancellationToken = default) where T : class;
    
    /// <summary>
    /// 从LiteDB执行聚合计算
    /// </summary>
    Task<BsonValue> AggregateLiteDbAsync(string collectionName, BsonExpression query, CancellationToken cancellationToken = default);

        /// <summary>
        /// 从CSV转换到JSON
        /// </summary>
        Task ConvertCsvToJsonAsync(string csvPath, string jsonPath);
        
        /// <summary>
        /// 从JSON转换到CSV
        /// </summary>
        Task ConvertJsonToCsvAsync(string jsonPath, string csvPath);
        
        /// <summary>
        /// 批量处理数据
        /// </summary>
        Task ProcessBatchAsync<T>(IEnumerable<T> data, Func<T, Task> processItem);
        
        /// <summary>
        /// 并行ETL处理
        /// </summary>
        Task ParallelETLProcessAsync<TInput, TOutput>(IEnumerable<TInput> input, Func<TInput, Task<TOutput>> transform);
    }
    
    /// <summary>
    /// ETL服务实现
    /// </summary>
    /// <summary>
/// ETL服务实现
/// </summary>
/// <summary>
/// 数据处理结果记录
/// </summary>
public class DataProcessingResult
{
    /// <summary>
    /// 数据ID
    /// </summary>
    public string DataId { get; set; }
    
    /// <summary>
    /// 处理状态
    /// </summary>
    public string Status { get; set; }
    
    /// <summary>
    /// 处理时间
    /// </summary>
    public DateTime ProcessTime { get; set; }
    
    /// <summary>
    /// 处理耗时(毫秒)
    /// </summary>
    public long DurationMs { get; set; }
    
    /// <summary>
    /// 错误信息
    /// </summary>
    public string Error { get; set; }
}

/// <summary>
/// ETL服务实现(支持AOT)
/// </summary>
[JsonSerializable(typeof(DataProcessingResult))]
[JsonSerializable(typeof(List<DataProcessingResult>))]
public class ETLService : IETLService, IAsyncDisposable
{
    private readonly ChoETLOptions _options;
    private readonly ILogger<ETLService> _logger;
    private readonly SqliteConnection _sqliteConnection;
    private readonly LiteDatabase _liteDatabase;
    private readonly Channel<string> _fileProcessingChannel;
    private readonly ITargetBlock<DataProcessingResult> _resultLoggerBlock;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    public ETLService(IOptions<ChoETLOptions> options, ILogger<ETLService> logger)
    {
        _options = options.Value;
        _logger = logger;
        
        // 初始化SQLite连接
        _sqliteConnection = new SqliteConnection(_options.SqliteConnectionString);
        _sqliteConnection.Open();
        
        // 初始化LiteDB连接
        _liteDatabase = new LiteDatabase(_options.LiteDbConnectionString);
        
        // 创建文件处理通道
        _fileProcessingChannel = Channel.CreateBounded<string>(
            new BoundedChannelOptions(_options.ParallelDegree * 2)
            {
                FullMode = BoundedChannelFullMode.Wait
            });
        
        // 初始化Dataflow处理管道
        _resultLoggerBlock = SetupDataflowPipeline();
        
        // 启动后台处理任务
        _ = ProcessFilesFromChannelAsync();
    }
    
    /// <summary>
    /// 处理单个CSV文件
    /// </summary>
    public async Task ProcessCsvFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            await _fileProcessingChannel.Writer.WriteAsync(filePath, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理文件 {FilePath} 时发生错误", filePath);
            throw;
        }
    }
    
    /// <summary>
    /// 批量处理CSV文件
    /// </summary>
    public async Task ProcessCsvFilesAsync(IEnumerable<string> filePaths, CancellationToken cancellationToken = default)
    {
        var tasks = filePaths.Select(filePath => ProcessCsvFileAsync(filePath, cancellationToken));
        await Task.WhenAll(tasks);
    }
    
    /// <summary>
    /// 执行SQL查询
    /// </summary>
    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, CancellationToken cancellationToken = default) where T : class, new()
    {
        using var command = _sqliteConnection.CreateCommand();
        command.CommandText = sql;
        
        var result = new List<T>();
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        while (await reader.ReadAsync(cancellationToken))
        {
            var item = new T();
            var properties = typeof(T).GetProperties();
            
            foreach (var prop in properties)
            {
                if (!reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                {
                    prop.SetValue(item, reader[prop.Name]);
                }
            }
            
            result.Add(item);
        }
        
        return result;
    }
    
    /// <summary>
    /// 执行聚合计算
    /// </summary>
    public async Task<object> AggregateAsync(string sql, CancellationToken cancellationToken = default)
    {
        using var command = _sqliteConnection.CreateCommand();
        command.CommandText = sql;
        return await command.ExecuteScalarAsync(cancellationToken);
    }
    
    /// <summary>
    /// 从LiteDB查询数据
    /// </summary>
    public Task<IEnumerable<T>> QueryLiteDbAsync<T>(string collectionName, BsonExpression query, CancellationToken cancellationToken = default) where T : class
    {
        var collection = _liteDatabase.GetCollection<T>(collectionName);
        return Task.FromResult(collection.Find(query).AsEnumerable());
    }
    
    /// <summary>
    /// 从LiteDB执行聚合计算
    /// </summary>
    public Task<BsonValue> AggregateLiteDbAsync(string collectionName, BsonExpression query, CancellationToken cancellationToken = default)
    {
        var collection = _liteDatabase.GetCollection(collectionName);
        return Task.FromResult(collection.Aggregate(query));
    }
    
    /// <summary>
    /// 设置Dataflow处理管道
    /// </summary>
    private ITargetBlock<DataProcessingResult> SetupDataflowPipeline()
    {
        var options = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = _options.ParallelDegree,
            EnsureOrdered = false,
            SingleProducerConstrained = true
        };
        
        var transformBlock = new TransformBlock<DataProcessingResult, string>(
            result => JsonSerializer.Serialize(result),
            options);
        
        var actionBlock = new ActionBlock<string>(
            json => File.AppendAllTextAsync("processing_logs.jsonl", json + "\n"),
            options);
        
        transformBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });
        
        return transformBlock;
    }
    
    private async Task ProcessFilesFromChannelAsync()
    {
        await foreach (var filePath in _fileProcessingChannel.Reader.ReadAllAsync())
        {
            try
            {
                await using var reader = new ChoCSVReader(filePath)
                    .WithFirstLineHeader()
                    .WithMaxScanRows(1000);
                
                var batch = new List<dynamic>();
                
                foreach (var record in reader)
                {
                    batch.Add(record);
                    
                    if (batch.Count >= _options.BatchSize)
                    {
                        var result = new DataProcessingResult
                {
                    DataId = Guid.NewGuid().ToString(),
                    ProcessTime = DateTime.UtcNow,
                    Status = "Processing"
                };
                
                var sw = Stopwatch.StartNew();
                try
                {
                    await ProcessBatchAsync(batch);
                    result.Status = "Completed";
                }
                catch (Exception ex)
                {
                    result.Status = "Failed";
                    result.Error = ex.Message;
                }
                finally
                {
                    sw.Stop();
                    result.DurationMs = sw.ElapsedMilliseconds;
                    _resultLoggerBlock.Post(result);
                }
                        batch.Clear();
                    }
                }
                
                if (batch.Count > 0)
                {
                    await ProcessBatchAsync(batch);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理文件 {FilePath} 时发生错误", filePath);
                await File.AppendAllTextAsync(_options.ErrorLogPath, $"{DateTime.Now}: {filePath} - {ex.Message}\n");
            }
        }
    }
    
    private async Task ProcessBatchAsync(List<dynamic> batch)
    {
        // 并行处理批数据
        var tasks = new List<Task>
        {
            Task.Run(() => InsertToSqliteAsync(batch)),
            Task.Run(() => InsertToLiteDbAsync(batch))
        };
        
        await Task.WhenAll(tasks);
    }
    
    private async Task InsertToSqliteAsync(List<dynamic> batch)
    {
        using var transaction = _sqliteConnection.BeginTransaction();
        
        try
        {
            foreach (var record in batch)
            {
                var dict = (IDictionary<string, object>)record;
                var columns = string.Join(", ", dict.Keys);
                var values = string.Join(", ", dict.Keys.Select(k => $"@{k}"));
                
                var sql = $"INSERT OR IGNORE INTO records ({columns}) VALUES ({values})";
                
                using var command = _sqliteConnection.CreateCommand();
                command.CommandText = sql;
                
                foreach (var kvp in dict)
                {
                    command.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value ?? DBNull.Value);
                }
                
                await command.ExecuteNonQueryAsync();
            }
            
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
    
    private async Task InsertToLiteDbAsync(List<dynamic> batch)
    {
        var collection = _liteDatabase.GetCollection("records");
        
        foreach (var record in batch)
        {
            var dict = (IDictionary<string, object>)record;
            var doc = new BsonDocument();
            
            foreach (var kvp in dict)
            {
                doc[kvp.Key] = kvp.Value != null ? BsonValue.Create(kvp.Value) : BsonValue.Null;
            }
            
            collection.Insert(doc);
        }
    }
    
    /// <summary>
    /// 释放资源
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        _fileProcessingChannel.Writer.Complete();
        await _sqliteConnection.DisposeAsync();
        _liteDatabase.Dispose();
    }
    {
        private readonly ChoETLOptions _options;
        private readonly ILogger<ETLService> _logger;
        private readonly Channel<Action> _etlChannel;
        
        public ETLService(IOptions<ChoETLOptions> options, ILogger<ETLService> logger)
        {
            _options = options.Value;
            _logger = logger;
            _etlChannel = Channel.CreateUnbounded<Action>();
            
            // 启动后台ETL处理任务
            Task.Run(ProcessETLQueueAsync);
        }
        
        public async Task ConvertCsvToJsonAsync(string csvPath, string jsonPath)
        {
            using var csvReader = new ChoCSVReader(csvPath);
            using var jsonWriter = new ChoJSONWriter(jsonPath);
            
            jsonWriter.Write(csvReader);
            await Task.CompletedTask;
        }
        
        public async Task ConvertJsonToCsvAsync(string jsonPath, string csvPath)
        {
            using var jsonReader = new ChoJSONReader(jsonPath);
            using var csvWriter = new ChoCSVWriter(csvPath);
            
            csvWriter.Write(jsonReader);
            await Task.CompletedTask;
        }
        
        public async Task ProcessBatchAsync<T>(IEnumerable<T> data, Func<T, Task> processItem)
        {
            var batch = new List<T>();
            foreach (var item in data)
            {
                batch.Add(item);
                if (batch.Count >= _options.BatchSize)
                {
                    await ProcessBatch(batch, processItem);
                    batch.Clear();
                }
            }
            
            if (batch.Count > 0)
            {
                await ProcessBatch(batch, processItem);
            }
        }
        
        public async Task ParallelETLProcessAsync<TInput, TOutput>(IEnumerable<TInput> input, Func<TInput, Task<TOutput>> transform)
        {
            var tasks = new List<Task<TOutput>>();
            
            foreach (var item in input)
            {
                tasks.Add(transform(item));
                if (tasks.Count >= Environment.ProcessorCount * 2)
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }
            }
            
            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
            }
        }
        
        private async Task ProcessBatch<T>(List<T> batch, Func<T, Task> processItem)
        {
            var tasks = new List<Task>();
            foreach (var item in batch)
            {
                tasks.Add(processItem(item));
            }
            await Task.WhenAll(tasks);
        }
        
        private async Task ProcessETLQueueAsync()
        {
            await foreach (var action in _etlChannel.Reader.ReadAllAsync())
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ETL处理失败");
                    File.AppendAllText(_options.ErrorLogPath, $"{DateTime.Now}: {ex}\n");
                }
            }
        }
    }
    
    /// <summary>
    /// ChoETL服务扩展
    /// </summary>
    public static class ChoETLServiceCollectionExtensions
    {
        public static IServiceCollection AddChoETLServices(this IServiceCollection services, Action<ChoETLOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddSingleton<IETLService, ETLService>();
            return services;
        }
    }
}