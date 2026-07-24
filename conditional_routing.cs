#:sdk Microsoft.NET.Sdk.Web
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Threading.Tasks.Dataflow;

public class ConditionalRouter
{
    private readonly BroadcastBlock<TaskItem> _broadcaster;
    private readonly TransformBlock<TaskItem, TaskResult> _highPriorityBlock;
    private readonly TransformBlock<TaskItem, TaskResult> _lowPriorityBlock;
    private readonly ActionBlock<TaskResult> _resultBlock;

    public ConditionalRouter()
    {
        var highPriorityOptions = new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 10000,
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            EnsureOrdered = false
        };

        var lowPriorityOptions = new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 10000,
            MaxDegreeOfParallelism = 1,
            EnsureOrdered = true
        };

        _broadcaster = new BroadcastBlock<TaskItem>(item => item);
        _highPriorityBlock = new TransformBlock<TaskItem, TaskResult>(item =>
        {
            // 高优先级处理逻辑
            return new TaskResult(item);
        }, highPriorityOptions);

        _lowPriorityBlock = new TransformBlock<TaskItem, TaskResult>(item =>
        {
            // 低优先级处理逻辑
            return new TaskResult(item);
        }, lowPriorityOptions);

        _resultBlock = new ActionBlock<TaskResult>(result =>
        {
            // 结果处理逻辑
        });

        _broadcaster.LinkTo(_highPriorityBlock, item => item.Priority == Priority.High);
        _broadcaster.LinkTo(_lowPriorityBlock, item => item.Priority == Priority.Low);
        _highPriorityBlock.LinkTo(_resultBlock);
        _lowPriorityBlock.LinkTo(_resultBlock);
    }
}

public record TaskItem(string Id, Priority Priority);
public record TaskResult(TaskItem Item);
public enum Priority { High, Low }