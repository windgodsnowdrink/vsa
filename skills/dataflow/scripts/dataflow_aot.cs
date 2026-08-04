#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Threading.Tasks.Dataflow@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dataflow.AOT
{
    /// <summary>
    /// Dataflow配置选项
    /// </summary>
    public class DataflowOptions
    {
        /// <summary>
        /// 是否启用Dataflow引擎
        /// </summary>
        public bool EnableDataflowEngine { get; set; } = true;
        
        /// <summary>
        /// 默认批处理大小
        /// </summary>
        public int DefaultBatchSize { get; set; } = 100;
        
        /// <summary>
        /// 默认并行度
        /// </summary>
        public int DefaultMaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
        
        /// <summary>
        /// 默认缓冲区大小
        /// </summary>
        public int DefaultBufferSize { get; set; } = 1000;
        
        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
        
        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;
        
        /// <summary>
        /// 超时时间
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
        
        /// <summary>
        /// 最大重试次数
        /// </summary>
        public int MaxRetryCount { get; set; } = 3;
        
        /// <summary>
        /// 重试间隔
        /// </summary>
        public TimeSpan RetryInterval { get; set; } = TimeSpan.FromMilliseconds(500);
        
        /// <summary>
        /// 是否启用背压机制
        /// </summary>
        public bool EnableBackpressure { get; set; } = true;
        
        /// <summary>
        /// 背压阈值
        /// </summary>
        public int BackpressureThreshold { get; set; } = 800;
    }
    
    /// <summary>
    /// Dataflow操作结果
    /// </summary>
    public class DataflowResult
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
        /// 处理的项目数量
        /// </summary>
        public int ProcessedItems { get; set; }
    }
    
    /// <summary>
    /// 带泛型结果的Dataflow操作结果
    /// </summary>
    /// <typeparam name="T">结果类型</typeparam>
    public class DataflowResult<T> : DataflowResult
    {
        /// <summary>
        /// 泛型结果数据
        /// </summary>
        public new T? ResultData { get; set; }
    }
    
    /// <summary>
    /// Dataflow状态信息
    /// </summary>
    public class DataflowStatus
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
        /// Dataflow引擎是否启用
        /// </summary>
        public bool IsDataflowEngineEnabled { get; set; }
        
        /// <summary>
        /// 当前活动的数据流数量
        /// </summary>
        public int ActiveDataflows { get; set; }
        
        /// <summary>
        /// 总处理项目数
        /// </summary>
        public long TotalProcessedItems { get; set; }
    }
    
    /// <summary>
    /// Dataflow服务接口
    /// 定义了Dataflow的核心功能
    /// </summary>
    public interface IDataflowService
    {
        /// <summary>
        /// 执行简单的数据流处理
        /// </summary>
        /// <typeparam name="TInput">输入类型</typeparam>
        /// <typeparam name="TOutput">输出类型</typeparam>
        /// <param name="inputData">输入数据列表</param>
        /// <param name="transformFunc">转换函数</param>
        /// <param name="parallelism">并行度</param>
        /// <returns>处理结果</returns>
        Task<DataflowResult<List<TOutput>>> ProcessSimpleFlowAsync<TInput, TOutput>(
            List<TInput> inputData,
            Func<TInput, Task<TOutput>> transformFunc,
            int? parallelism = null);
        
        /// <summary>
        /// 执行批处理数据流
        /// </summary>
        /// <typeparam name="TInput">输入类型</typeparam>
        /// <typeparam name="TOutput">输出类型</typeparam>
        /// <param name="inputData">输入数据列表</param>
        /// <param name="batchTransformFunc">批处理转换函数</param>
        /// <param name="batchSize">批处理大小</param>
        /// <param name="parallelism">并行度</param>
        /// <returns>处理结果</returns>
        Task<DataflowResult<List<TOutput>>> ProcessBatchFlowAsync<TInput, TOutput>(
            List<TInput> inputData,
            Func<List<TInput>, Task<List<TOutput>>> batchTransformFunc,
            int? batchSize = null,
            int? parallelism = null);
        
        /// <summary>
        /// 执行复杂的多步骤数据流
        /// </summary>
        /// <typeparam name="TInput">输入类型</typeparam>
        /// <typeparam name="TIntermediate">中间结果类型</typeparam>
        /// <typeparam name="TOutput">输出类型</typeparam>
        /// <param name="inputData">输入数据列表</param>
        /// <param name="firstTransform">第一步转换函数</param>
        /// <param name="secondTransform">第二步转换函数</param>
        /// <param name="parallelism">并行度</param>
        /// <returns>处理结果</returns>
        Task<DataflowResult<List<TOutput>>> ProcessComplexFlowAsync<TInput, TIntermediate, TOutput>(
            List<TInput> inputData,
            Func<TInput, Task<TIntermediate>> firstTransform,
            Func<TIntermediate, Task<TOutput>> secondTransform,
            int? parallelism = null);
        
        /// <summary>
        /// 获取Dataflow状态
        /// </summary>
        /// <returns>状态信息</returns>
        Task<DataflowStatus> GetStatusAsync();
        
        /// <summary>
        /// 重置Dataflow状态
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> ResetStatusAsync();
        
        /// <summary>
        /// 执行高性能管道处理
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="inputData">输入数据</param>
        /// <param name="pipelineSteps">管道步骤列表</param>
        /// <param name="parallelism">并行度</param>
        /// <returns>处理结果</returns>
        Task<DataflowResult<List<T>>> ProcessPipelineAsync<T>(
            List<T> inputData,
            List<Func<T, Task<T>>> pipelineSteps,
            int? parallelism = null);
    }
    
    /// <summary>
    /// Dataflow服务实现
    /// 基于.NET 10 AOT架构，提供高性能Dataflow功能
    /// </summary>
    public class DataflowService : IDataflowService
    {
        private readonly ILogger<DataflowService> _logger;
        private readonly DataflowOptions _options;
        private long _processedRequests = 0;
        private long _successfulRequests = 0;
        private long _failedRequests = 0;
        private long _totalExecutionTime = 0;
        private long _totalProcessedItems = 0;
        private int _activeDataflows = 0;
        private readonly DateTime _startTime = DateTime.UtcNow;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="options">配置选项</param>
        public DataflowService(ILogger<DataflowService> logger, IOptions<DataflowOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            
            _logger.LogInformation("DataflowService初始化成功，配置选项：Parallelism={Parallelism}, BatchSize={BatchSize}, EnableDataflowEngine={EnableDataflowEngine}",
                _options.DefaultMaxDegreeOfParallelism, _options.DefaultBatchSize, _options.EnableDataflowEngine);
        }
        
        /// <summary>
        /// 执行简单的数据流处理
        /// </summary>
        public async Task<DataflowResult<List<TOutput>>> ProcessSimpleFlowAsync<TInput, TOutput>(
            List<TInput> inputData,
            Func<TInput, Task<TOutput>> transformFunc,
            int? parallelism = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DataflowResult<List<TOutput>> { OperationType = "SimpleFlow" };
            
            try
            {
                Interlocked.Increment(ref _processedRequests);
                Interlocked.Increment(ref _activeDataflows);
                
                if (!_options.EnableDataflowEngine)
                {
                    throw new InvalidOperationException("Dataflow引擎已禁用");
                }
                
                var maxDegreeOfParallelism = parallelism ?? _options.DefaultMaxDegreeOfParallelism;
                
                _logger.LogInformation("开始执行简单数据流处理，输入项目数: {ItemCount}, 并行度: {Parallelism}",
                    inputData.Count, maxDegreeOfParallelism);
                
                // 创建转换块
                var transformBlock = new TransformBlock<TInput, TOutput>(
                    transformFunc,
                    new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = maxDegreeOfParallelism,
                        BoundedCapacity = _options.DefaultBufferSize
                    });
                
                // 创建操作块来收集结果
                var results = new List<TOutput>();
                var actionBlock = new ActionBlock<TOutput>(
                    result => results.Add(result),
                    new ExecutionDataflowBlockOptions { BoundedCapacity = _options.DefaultBufferSize });
                
                // 链接块
                transformBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });
                
                // 发布数据
                foreach (var item in inputData)
                {
                    await transformBlock.SendAsync(item);
                }
                
                // 标记输入完成
                transformBlock.Complete();
                
                // 等待处理完成
                await actionBlock.Completion;
                
                result.Success = true;
                result.ResultData = results;
                result.ProcessedItems = results.Count;
                
                Interlocked.Increment(ref _successfulRequests);
                Interlocked.Add(ref _totalProcessedItems, results.Count);
                
                _logger.LogInformation("简单数据流处理完成，输出项目数: {ItemCount}", results.Count);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedRequests);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "简单数据流处理失败");
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                Interlocked.Add(ref _totalExecutionTime, result.ExecutionTimeMs);
                Interlocked.Decrement(ref _activeDataflows);
            }
            
            return result;
        }
        
        /// <summary>
        /// 执行批处理数据流
        /// </summary>
        public async Task<DataflowResult<List<TOutput>>> ProcessBatchFlowAsync<TInput, TOutput>(
            List<TInput> inputData,
            Func<List<TInput>, Task<List<TOutput>>> batchTransformFunc,
            int? batchSize = null,
            int? parallelism = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DataflowResult<List<TOutput>> { OperationType = "BatchFlow" };
            
            try
            {
                Interlocked.Increment(ref _processedRequests);
                Interlocked.Increment(ref _activeDataflows);
                
                if (!_options.EnableDataflowEngine)
                {
                    throw new InvalidOperationException("Dataflow引擎已禁用");
                }
                
                var actualBatchSize = batchSize ?? _options.DefaultBatchSize;
                var maxDegreeOfParallelism = parallelism ?? _options.DefaultMaxDegreeOfParallelism;
                
                _logger.LogInformation("开始执行批处理数据流，输入项目数: {ItemCount}, 批大小: {BatchSize}, 并行度: {Parallelism}",
                    inputData.Count, actualBatchSize, maxDegreeOfParallelism);
                
                // 创建批处理块
                var batchBlock = new BatchBlock<TInput>(
                    actualBatchSize,
                    new GroupingDataflowBlockOptions { BoundedCapacity = _options.DefaultBufferSize });
                
                // 创建转换块
                var transformBlock = new TransformBlock<List<TInput>, List<TOutput>>(
                    batchTransformFunc,
                    new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = maxDegreeOfParallelism,
                        BoundedCapacity = _options.DefaultBufferSize
                    });
                
                // 创建操作块来收集结果
                var results = new List<TOutput>();
                var actionBlock = new ActionBlock<List<TOutput>>(
                    batchResults => results.AddRange(batchResults),
                    new ExecutionDataflowBlockOptions { BoundedCapacity = _options.DefaultBufferSize });
                
                // 链接块
                batchBlock.LinkTo(transformBlock, new DataflowLinkOptions { PropagateCompletion = true });
                transformBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });
                
                // 发布数据
                foreach (var item in inputData)
                {
                    await batchBlock.SendAsync(item);
                }
                
                // 标记输入完成
                batchBlock.Complete();
                
                // 等待处理完成
                await actionBlock.Completion;
                
                result.Success = true;
                result.ResultData = results;
                result.ProcessedItems = results.Count;
                
                Interlocked.Increment(ref _successfulRequests);
                Interlocked.Add(ref _totalProcessedItems, results.Count);
                
                _logger.LogInformation("批处理数据流处理完成，输出项目数: {ItemCount}", results.Count);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedRequests);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "批处理数据流处理失败");
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                Interlocked.Add(ref _totalExecutionTime, result.ExecutionTimeMs);
                Interlocked.Decrement(ref _activeDataflows);
            }
            
            return result;
        }
        
        /// <summary>
        /// 执行复杂的多步骤数据流
        /// </summary>
        public async Task<DataflowResult<List<TOutput>>> ProcessComplexFlowAsync<TInput, TIntermediate, TOutput>(
            List<TInput> inputData,
            Func<TInput, Task<TIntermediate>> firstTransform,
            Func<TIntermediate, Task<TOutput>> secondTransform,
            int? parallelism = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DataflowResult<List<TOutput>> { OperationType = "ComplexFlow" };
            
            try
            {
                Interlocked.Increment(ref _processedRequests);
                Interlocked.Increment(ref _activeDataflows);
                
                if (!_options.EnableDataflowEngine)
                {
                    throw new InvalidOperationException("Dataflow引擎已禁用");
                }
                
                var maxDegreeOfParallelism = parallelism ?? _options.DefaultMaxDegreeOfParallelism;
                
                _logger.LogInformation("开始执行复杂数据流，输入项目数: {ItemCount}, 并行度: {Parallelism}",
                    inputData.Count, maxDegreeOfParallelism);
                
                // 创建第一步转换块
                var firstTransformBlock = new TransformBlock<TInput, TIntermediate>(
                    firstTransform,
                    new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = maxDegreeOfParallelism,
                        BoundedCapacity = _options.DefaultBufferSize
                    });
                
                // 创建第二步转换块
                var secondTransformBlock = new TransformBlock<TIntermediate, TOutput>(
                    secondTransform,
                    new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = maxDegreeOfParallelism,
                        BoundedCapacity = _options.DefaultBufferSize
                    });
                
                // 创建操作块来收集结果
                var results = new List<TOutput>();
                var actionBlock = new ActionBlock<TOutput>(
                    item => results.Add(item),
                    new ExecutionDataflowBlockOptions { BoundedCapacity = _options.DefaultBufferSize });
                
                // 链接块
                firstTransformBlock.LinkTo(secondTransformBlock, new DataflowLinkOptions { PropagateCompletion = true });
                secondTransformBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });
                
                // 发布数据
                foreach (var item in inputData)
                {
                    await firstTransformBlock.SendAsync(item);
                }
                
                // 标记输入完成
                firstTransformBlock.Complete();
                
                // 等待处理完成
                await actionBlock.Completion;
                
                result.Success = true;
                result.ResultData = results;
                result.ProcessedItems = results.Count;
                
                Interlocked.Increment(ref _successfulRequests);
                Interlocked.Add(ref _totalProcessedItems, results.Count);
                
                _logger.LogInformation("复杂数据流处理完成，输出项目数: {ItemCount}", results.Count);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedRequests);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "复杂数据流处理失败");
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                Interlocked.Add(ref _totalExecutionTime, result.ExecutionTimeMs);
                Interlocked.Decrement(ref _activeDataflows);
            }
            
            return result;
        }
        
        /// <summary>
        /// 执行高性能管道处理
        /// </summary>
        public async Task<DataflowResult<List<T>>> ProcessPipelineAsync<T>(
            List<T> inputData,
            List<Func<T, Task<T>>> pipelineSteps,
            int? parallelism = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DataflowResult<List<T>> { OperationType = "Pipeline" };
            
            try
            {
                Interlocked.Increment(ref _processedRequests);
                Interlocked.Increment(ref _activeDataflows);
                
                if (!_options.EnableDataflowEngine)
                {
                    throw new InvalidOperationException("Dataflow引擎已禁用");
                }
                
                var maxDegreeOfParallelism = parallelism ?? _options.DefaultMaxDegreeOfParallelism;
                
                _logger.LogInformation("开始执行管道处理，输入项目数: {ItemCount}, 步骤数: {StepCount}, 并行度: {Parallelism}",
                    inputData.Count, pipelineSteps.Count, maxDegreeOfParallelism);
                
                if (pipelineSteps.Count == 0)
                {
                    result.Success = true;
                    result.ResultData = inputData;
                    result.ProcessedItems = inputData.Count;
                    return result;
                }
                
                // 创建管道块链
                TransformBlock<T, T>? previousBlock = null;
                
                for (int i = 0; i < pipelineSteps.Count; i++)
                {
                    var stepFunc = pipelineSteps[i];
                    var currentBlock = new TransformBlock<T, T>(
                        stepFunc,
                        new ExecutionDataflowBlockOptions
                        {
                            MaxDegreeOfParallelism = maxDegreeOfParallelism,
                            BoundedCapacity = _options.DefaultBufferSize
                        });
                    
                    if (previousBlock != null)
                    {
                        previousBlock.LinkTo(currentBlock, new DataflowLinkOptions { PropagateCompletion = true });
                    }
                    
                    previousBlock = currentBlock;
                }
                
                // 创建操作块来收集结果
                var results = new List<T>();
                var actionBlock = new ActionBlock<T>(
                    item => results.Add(item),
                    new ExecutionDataflowBlockOptions { BoundedCapacity = _options.DefaultBufferSize });
                
                // 链接最后一个管道块到结果收集块
                previousBlock?.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });
                
                // 发布数据
                if (previousBlock != null)
                {
                    var firstBlock = previousBlock;
                    foreach (var item in inputData)
                    {
                        await firstBlock.SendAsync(item);
                    }
                    
                    // 标记输入完成
                    firstBlock.Complete();
                    
                    // 等待处理完成
                    await actionBlock.Completion;
                }
                
                result.Success = true;
                result.ResultData = results;
                result.ProcessedItems = results.Count;
                
                Interlocked.Increment(ref _successfulRequests);
                Interlocked.Add(ref _totalProcessedItems, results.Count);
                
                _logger.LogInformation("管道处理完成，输出项目数: {ItemCount}", results.Count);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedRequests);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "管道处理失败");
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                Interlocked.Add(ref _totalExecutionTime, result.ExecutionTimeMs);
                Interlocked.Decrement(ref _activeDataflows);
            }
            
            return result;
        }
        
        /// <summary>
        /// 获取Dataflow状态
        /// </summary>
        public async Task<DataflowStatus> GetStatusAsync()
        {
            await Task.CompletedTask; // 模拟异步操作
            
            var requestCount = Interlocked.Read(ref _processedRequests);
            var averageTime = requestCount > 0 ? (double)Interlocked.Read(ref _totalExecutionTime) / requestCount : 0;
            
            var status = new DataflowStatus
            {
                IsRunning = true,
                ProcessedRequests = requestCount,
                SuccessfulRequests = Interlocked.Read(ref _successfulRequests),
                FailedRequests = Interlocked.Read(ref _failedRequests),
                AverageExecutionTimeMs = Math.Round(averageTime, 2),
                StartTime = _startTime,
                IsDataflowEngineEnabled = _options.EnableDataflowEngine,
                ActiveDataflows = Interlocked.CompareExchange(ref _activeDataflows, 0, 0),
                TotalProcessedItems = Interlocked.Read(ref _totalProcessedItems)
            };
            
            _logger.LogDebug("获取Dataflow状态: {@Status}", status);
            
            return status;
        }
        
        /// <summary>
        /// 重置Dataflow状态
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
                Interlocked.Exchange(ref _totalProcessedItems, 0);
                
                _logger.LogInformation("Dataflow状态已重置");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "重置Dataflow状态失败");
                return false;
            }
        }
    }
    
    /// <summary>
    /// Dataflow AOT执行引擎
    /// 管理Dataflow功能调用
    /// </summary>
    public class DataflowAotEngine
    {
        private readonly ILogger<DataflowAotEngine> _logger;
        private readonly IDataflowService _dataflowService;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="dataflowService">Dataflow服务</param>
        public DataflowAotEngine(ILogger<DataflowAotEngine> logger, IDataflowService dataflowService)
        {
            _logger = logger;
            _dataflowService = dataflowService;
            
            _logger.LogInformation("DataflowAotEngine初始化成功");
        }
        
        /// <summary>
        /// 执行简单的数据流处理
        /// </summary>
        public async Task<DataflowResult<List<TOutput>>> ProcessSimpleFlowAsync<TInput, TOutput>(
            List<TInput> inputData,
            Func<TInput, Task<TOutput>> transformFunc,
            int? parallelism = null)
        {
            return await _dataflowService.ProcessSimpleFlowAsync(inputData, transformFunc, parallelism);
        }
        
        /// <summary>
        /// 执行批处理数据流
        /// </summary>
        public async Task<DataflowResult<List<TOutput>>> ProcessBatchFlowAsync<TInput, TOutput>(
            List<TInput> inputData,
            Func<List<TInput>, Task<List<TOutput>>> batchTransformFunc,
            int? batchSize = null,
            int? parallelism = null)
        {
            return await _dataflowService.ProcessBatchFlowAsync(inputData, batchTransformFunc, batchSize, parallelism);
        }
        
        /// <summary>
        /// 执行复杂的多步骤数据流
        /// </summary>
        public async Task<DataflowResult<List<TOutput>>> ProcessComplexFlowAsync<TInput, TIntermediate, TOutput>(
            List<TInput> inputData,
            Func<TInput, Task<TIntermediate>> firstTransform,
            Func<TIntermediate, Task<TOutput>> secondTransform,
            int? parallelism = null)
        {
            return await _dataflowService.ProcessComplexFlowAsync(inputData, firstTransform, secondTransform, parallelism);
        }
        
        /// <summary>
        /// 执行高性能管道处理
        /// </summary>
        public async Task<DataflowResult<List<T>>> ProcessPipelineAsync<T>(
            List<T> inputData,
            List<Func<T, Task<T>>> pipelineSteps,
            int? parallelism = null)
        {
            return await _dataflowService.ProcessPipelineAsync(inputData, pipelineSteps, parallelism);
        }
        
        /// <summary>
        /// 获取Dataflow状态
        /// </summary>
        public async Task<DataflowStatus> GetStatusAsync()
        {
            return await _dataflowService.GetStatusAsync();
        }
        
        /// <summary>
        /// 重置Dataflow状态
        /// </summary>
        public async Task<bool> ResetStatusAsync()
        {
            return await _dataflowService.ResetStatusAsync();
        }
    }
    
    /// <summary>
    /// Dataflow扩展
    /// </summary>
    public static class DataflowExtensions
    {
        /// <summary>
        /// 注册Dataflow服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddDataflow(this IServiceCollection services)
        {
            services.AddSingleton<IDataflowService, DataflowService>();
            services.AddSingleton<DataflowAotEngine>();
            
            return services;
        }
        
        /// <summary>
        /// 注册Dataflow服务并配置选项
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configureOptions">配置选项的委托</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddDataflow(this IServiceCollection services, Action<DataflowOptions> configureOptions)
        {
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }
            
            services.Configure(configureOptions);
            services.AddDataflow();
            
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
            
            // 配置Dataflow选项
            builder.Configuration.AddJsonFile("dataflow_aot.setting.json", optional: true);
            builder.Services.Configure<DataflowOptions>(builder.Configuration.GetSection("Dataflow"));
            
            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);
            
            // 注册服务
            builder.Services.AddDataflow();
            
            // 构建主机
            var host = builder.Build();
            var serviceProvider = host.Services;
            
            // 获取引擎实例
            var engine = serviceProvider.GetRequiredService<DataflowAotEngine>();
            
            // 解析命令行参数
            if (args.Length < 1)
            {
                Console.WriteLine("用法:");
                Console.WriteLine("  dataflow_aot.exe <command> [arguments]");
                Console.WriteLine("  ");
                Console.WriteLine("命令:");
                Console.WriteLine("  status    获取服务状态");
                Console.WriteLine("  reset     重置服务状态");
                Console.WriteLine("  demo      运行演示数据流");
                Console.WriteLine("  ");
                Console.WriteLine("示例:");
                Console.WriteLine("  dataflow_aot.exe status");
                Console.WriteLine("  dataflow_aot.exe reset");
                Console.WriteLine("  dataflow_aot.exe demo");
                return 1;
            }
            
            try
            {
                string command = args[0].ToLower();
                
                switch (command)
                {
                    case "status":
                        var status = await engine.GetStatusAsync();
                        Console.WriteLine("Dataflow服务状态:");
                        Console.WriteLine($"  运行状态: {(status.IsRunning ? "正常" : "异常"}");
                        Console.WriteLine($"  Dataflow引擎: {(status.IsDataflowEngineEnabled ? "已启用" : "已禁用"}");
                        Console.WriteLine($"  活动数据流: {status.ActiveDataflows}");
                        Console.WriteLine($"  已处理请求: {status.ProcessedRequests}");
                        Console.WriteLine($"  成功请求: {status.SuccessfulRequests}");
                        Console.WriteLine($"  失败请求: {status.FailedRequests}");
                        Console.WriteLine($"  总处理项目: {status.TotalProcessedItems}");
                        Console.WriteLine($"  平均执行时间: {status.AverageExecutionTimeMs} ms");
                        Console.WriteLine($"  服务启动时间: {status.StartTime.ToLocalTime()}");
                        return 0;
                        
                    case "reset":
                        var resetResult = await engine.ResetStatusAsync();
                        Console.WriteLine($"重置服务状态: {(resetResult ? "成功" : "失败"}");
                        return resetResult ? 0 : 1;
                        
                    case "demo":
                        Console.WriteLine("运行Dataflow演示...");
                        
                        // 创建测试数据
                        var testData = Enumerable.Range(1, 100).Select(i => new { Id = i, Value = i * 2 }).ToList();
                        
                        // 简单数据流演示
                        Console.WriteLine("\n1. 简单数据流演示:");
                        var simpleResult = await engine.ProcessSimpleFlowAsync(
                            testData,
                            async item => {
                                // 模拟处理延迟
                                await Task.Delay(10);
                                return new { item.Id, item.Value, Processed = true, Timestamp = DateTime.Now };
                            },
                            4);
                        
                        Console.WriteLine($"   结果: {simpleResult.Success ? "成功" : "失败"}");
                        if (simpleResult.Success)
                        {
                            Console.WriteLine($"   处理项目数: {simpleResult.ProcessedItems}");
                            Console.WriteLine($"   执行时间: {simpleResult.ExecutionTimeMs} ms");
                        }
                        
                        // 批处理演示
                        Console.WriteLine("\n2. 批处理数据流演示:");
                        var batchResult = await engine.ProcessBatchFlowAsync(
                            testData,
                            async batch => {
                                // 模拟批处理
                                await Task.Delay(50);
                                return batch.Select(item => new { item.Id, item.Value, BatchProcessed = true, BatchSize = batch.Count }).ToList();
                            },
                            20,
                            2);
                        
                        Console.WriteLine($"   结果: {batchResult.Success ? "成功" : "失败"}");
                        if (batchResult.Success)
                        {
                            Console.WriteLine($"   处理项目数: {batchResult.ProcessedItems}");
                            Console.WriteLine($"   执行时间: {batchResult.ExecutionTimeMs} ms");
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