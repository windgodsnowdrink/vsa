#:sdk Microsoft.NET.Sdk.Web
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Threading.Tasks.Dataflow;

public class TaskExecutionStrategy
{
    private readonly TransformBlock<TaskItem, TaskResult> _parallelBlock;
    private readonly TransformBlock<TaskItem, TaskResult> _sequentialBlock;
    private readonly JoinBlock<TaskResult, TaskResult> _joinBlock;
    private readonly ActionBlock<Tuple<TaskResult, TaskResult>> _resultBlock;

    public TaskExecutionStrategy()
    {
        var parallelOptions = new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 10000,
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            EnsureOrdered = false
        };

        var sequentialOptions = new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 10000,
            MaxDegreeOfParallelism = 1,
            EnsureOrdered = true
        };

        _parallelBlock = new TransformBlock<TaskItem, TaskResult>(item =>
        {
            // 并行执行逻辑
            return new TaskResult(item);
        }, parallelOptions);

        _sequentialBlock = new TransformBlock<TaskItem, TaskResult>(item =>
        {
            // 顺序执行逻辑
            return new TaskResult(item);
        }, sequentialOptions);

        _joinBlock = new JoinBlock<TaskResult, TaskResult>();
        _resultBlock = new ActionBlock<Tuple<TaskResult, TaskResult>>(result =>
        {
            // 结果处理逻辑
        });

        _parallelBlock.LinkTo(_joinBlock.Target1);
        _sequentialBlock.LinkTo(_joinBlock.Target2);
        _joinBlock.LinkTo(_resultBlock);
    }
}

public record TaskItem(string Id);
public record TaskResult(TaskItem Item);