#:sdk Microsoft.NET.Sdk
#:package System.CommandLine@2.0.0-beta4.22272.1
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Microsoft.Extensions.Logging@8.0.0
#:package Microsoft.Extensions.Logging.Console@8.0.0
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property SelfContained=true
#:property PublishSingleFile=true

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TPLSkill
{
    public interface ITaskService
    {
        Task RunParallelTasksAsync(int taskCount, CancellationToken cancellationToken = default);
        Task<T[]> WhenAllAsync<T>(params Task<T>[] tasks);
        Task<T> WhenAnyAsync<T>(params Task<T>[] tasks);
    }

    public interface IDataflowService
    {
        Task ProcessDataflowPipelineAsync(IEnumerable<int> data, CancellationToken cancellationToken = default);
        Task<Dictionary<string, int>> ProcessWithTransformBlockAsync(IEnumerable<string> data);
    }

    public interface IParallelService
    {
        IEnumerable<T> ParallelForEach<T>(IEnumerable<T> source, Action<T> action);
        T[] ParallelInvoke<T>(params Func<T>[] functions);
        double[] PLINQTransform(IEnumerable<double> source);
    }

    public interface IAsyncService
    {
        Task<long> CalculateFactorialAsync(int n);
        Task<string> SimulateAsyncOperationAsync(string input, int delayMs);
        Task<int> RetryAsync(Func<Task<int>> operation, int maxRetries);
    }

    public class TaskService : ITaskService
    {
        private readonly ILogger<TaskService> _logger;

        public TaskService(ILogger<TaskService> logger)
        {
            _logger = logger;
        }

        public async Task RunParallelTasksAsync(int taskCount, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"开始执行{taskCount}个并行任务");
            
            var tasks = Enumerable.Range(1, taskCount)
                .Select(async i =>
                {
                    _logger.LogDebug($"任务{i}开始执行");
                    await Task.Delay(100 * i, cancellationToken);
                    _logger.LogDebug($"任务{i}执行完成");
                    return i;
                })
                .ToArray();

            await Task.WhenAll(tasks);
            _logger.LogInformation("所有并行任务执行完成");
        }

        public async Task<T[]> WhenAllAsync<T>(params Task<T>[] tasks)
        {
            return await Task.WhenAll(tasks);
        }

        public async Task<T> WhenAnyAsync<T>(params Task<T>[] tasks)
        {
            var completedTask = await Task.WhenAny(tasks);
            return await completedTask;
        }
    }

    public class DataflowService : IDataflowService
    {
        private readonly ILogger<DataflowService> _logger;

        public DataflowService(ILogger<DataflowService> logger)
        {
            _logger = logger;
        }

        public async Task ProcessDataflowPipelineAsync(IEnumerable<int> data, CancellationToken cancellationToken = default)
        {
            var options = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                CancellationToken = cancellationToken
            };

            var transformBlock = new TransformBlock<int, int>(
                item =>
                {
                    _logger.LogDebug($"转换数据: {item} -> {item * 2}");
                    Task.Delay(50).Wait();
                    return item * 2;
                },
                options);

            var actionBlock = new ActionBlock<int>(
                item =>
                {
                    _logger.LogInformation($"处理结果: {item}");
                    Task.Delay(30).Wait();
                },
                options);

            transformBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });

            foreach (var item in data)
            {
                await transformBlock.SendAsync(item, cancellationToken);
            }

            transformBlock.Complete();
            await actionBlock.Completion;
        }

        public async Task<Dictionary<string, int>> ProcessWithTransformBlockAsync(IEnumerable<string> data)
        {
            var transformBlock = new TransformBlock<string, KeyValuePair<string, int>>(
                item =>
                {
                    _logger.LogDebug($"处理字符串: {item}");
                    return new KeyValuePair<string, int>(item, item.Length);
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = Environment.ProcessorCount });

            var results = new Dictionary<string, int>();
            var actionBlock = new ActionBlock<KeyValuePair<string, int>>(
                item => results[item.Key] = item.Value);

            transformBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });

            foreach (var item in data)
            {
                await transformBlock.SendAsync(item);
            }

            transformBlock.Complete();
            await actionBlock.Completion;

            return results;
        }
    }

    public class ParallelService : IParallelService
    {
        private readonly ILogger<ParallelService> _logger;

        public ParallelService(ILogger<ParallelService> logger)
        {
            _logger = logger;
        }

        public IEnumerable<T> ParallelForEach<T>(IEnumerable<T> source, Action<T> action)
        {
            _logger.LogInformation($"开始并行处理{source.Count()}个项目");
            
            var results = new List<T>();
            Parallel.ForEach(source, item =>
            {
                action(item);
                lock (results)
                {
                    results.Add(item);
                }
            });

            return results;
        }

        public T[] ParallelInvoke<T>(params Func<T>[] functions)
        {
            _logger.LogInformation($"并行执行{functions.Length}个函数");
            return Parallel.Invoke(functions);
        }

        public double[] PLINQTransform(IEnumerable<double> source)
        {
            _logger.LogInformation($"使用PLINQ转换{source.Count()}个数据项");
            
            return source
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .Select(x => Math.Sqrt(x) * Math.PI)
                .ToArray();
        }
    }

    public class AsyncService : IAsyncService
    {
        private readonly ILogger<AsyncService> _logger;

        public AsyncService(ILogger<AsyncService> logger)
        {
            _logger = logger;
        }

        public async Task<long> CalculateFactorialAsync(int n)
        {
            _logger.LogInformation($"计算{ n }的阶乘");
            
            if (n < 0)
                throw new ArgumentException("阶乘不能为负数", nameof(n));
            if (n == 0 || n == 1)
                return 1;

            long result = 1;
            for (int i = 2; i <= n; i++)
            {
                result *= i;
                await Task.Yield();
            }

            return result;
        }

        public async Task<string> SimulateAsyncOperationAsync(string input, int delayMs)
        {
            _logger.LogInformation($"模拟异步操作: {input}, 延迟{delayMs}ms");
            await Task.Delay(delayMs);
            return $"处理完成: {input}";
        }

        public async Task<int> RetryAsync(Func<Task<int>> operation, int maxRetries)
        {
            int attempts = 0;
            while (true)
            {
                try
                {
                    attempts++;
                    _logger.LogInformation($"尝试操作, 第{attempts}次");
                    return await operation();
                }
                catch (Exception ex) when (attempts < maxRetries)
                {
                    _logger.LogWarning($"操作失败: {ex.Message}, 将在1秒后重试");
                    await Task.Delay(1000);
                }
            }
        }
    }

    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("TPL技能 - 任务并行库示例");

            var taskCommand = new Command("task", "并行任务操作");
            var taskCountOption = new Option<int>("--count", getDefaultValue: () => 5, description: "任务数量");
            taskCommand.AddOption(taskCountOption);
            taskCommand.SetHandler(async (count, cancellationToken) =>
            {
                using var serviceProvider = CreateServiceProvider();
                var taskService = serviceProvider.GetRequiredService<ITaskService>();
                await taskService.RunParallelTasksAsync(count, cancellationToken);
            }, taskCountOption);

            var dataflowCommand = new Command("dataflow", "数据流操作");
            var dataCountOption = new Option<int>("--count", getDefaultValue: () => 10, description: "数据项数量");
            dataflowCommand.AddOption(dataCountOption);
            dataflowCommand.SetHandler(async (count, cancellationToken) =>
            {
                using var serviceProvider = CreateServiceProvider();
                var dataflowService = serviceProvider.GetRequiredService<IDataflowService>();
                var data = Enumerable.Range(1, count);
                await dataflowService.ProcessDataflowPipelineAsync(data, cancellationToken);
            }, dataCountOption);

            var parallelCommand = new Command("parallel", "并行处理操作");
            var parallelCountOption = new Option<int>("--count", getDefaultValue: () => 100, description: "数据项数量");
            parallelCommand.AddOption(parallelCountOption);
            parallelCommand.SetHandler((count) =>
            {
                using var serviceProvider = CreateServiceProvider();
                var parallelService = serviceProvider.GetRequiredService<IParallelService>();
                var data = Enumerable.Range(1, count).Select(i => (double)i);
                var results = parallelService.PLINQTransform(data);
                Console.WriteLine($"PLINQ转换完成, 结果数量: {results.Length}");
                Console.WriteLine($"前5个结果: {string.Join(", ", results.Take(5))}");
            }, parallelCountOption);

            var asyncCommand = new Command("async", "异步操作");
            var factorialOption = new Option<int>("--factorial", description: "计算阶乘");
            var simulateOption = new Option<string>("--simulate", description: "模拟异步操作");
            var delayOption = new Option<int>("--delay", getDefaultValue: () => 1000, description: "模拟延迟(ms)");
            asyncCommand.AddOption(factorialOption);
            asyncCommand.AddOption(simulateOption);
            asyncCommand.AddOption(delayOption);
            asyncCommand.SetHandler(async (factorial, simulate, delay) =>
            {
                using var serviceProvider = CreateServiceProvider();
                var asyncService = serviceProvider.GetRequiredService<IAsyncService>();
                
                if (factorial.HasValue)
                {
                    var result = await asyncService.CalculateFactorialAsync(factorial.Value);
                    Console.WriteLine($"{factorial.Value}的阶乘: {result}");
                }
                else if (!string.IsNullOrEmpty(simulate))
                {
                    var result = await asyncService.SimulateAsyncOperationAsync(simulate, delay);
                    Console.WriteLine(result);
                }
                else
                {
                    Console.WriteLine("请指定 --factorial 或 --simulate 参数");
                }
            }, factorialOption, simulateOption, delayOption);

            rootCommand.AddCommand(taskCommand);
            rootCommand.AddCommand(dataflowCommand);
            rootCommand.AddCommand(parallelCommand);
            rootCommand.AddCommand(asyncCommand);

            var parser = new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .Build();

            return await parser.InvokeAsync(args);
        }

        private static ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            
            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });

            services.AddTransient<ITaskService, TaskService>();
            services.AddTransient<IDataflowService, DataflowService>();
            services.AddTransient<IParallelService, ParallelService>();
            services.AddTransient<IAsyncService, AsyncService>();

            return services.BuildServiceProvider();
        }
    }
}
