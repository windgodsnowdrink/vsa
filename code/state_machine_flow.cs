#:sdk Microsoft.NET.Sdk.Web
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Threading.Tasks.Dataflow;

public class StateMachineFlow
{
    private readonly TransformBlock<TaskItem, TaskState> _initBlock;
    private readonly TransformBlock<TaskState, TaskState> _processBlock;
    private readonly TransformBlock<TaskState, TaskState> _verifyBlock;
    private readonly ActionBlock<TaskState> _completeBlock;

    public StateMachineFlow()
    {
        var options = new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 10000,
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            EnsureOrdered = false
        };

        _initBlock = new TransformBlock<TaskItem, TaskState>(item =>
        {
            return new TaskState(item, State.Initialized);
        }, options);

        _processBlock = new TransformBlock<TaskState, TaskState>(state =>
        {
            // 处理逻辑
            return state with { CurrentState = State.Processed };
        }, options);

        _verifyBlock = new TransformBlock<TaskState, TaskState>(state =>
        {
            // 验证逻辑
            return state with { CurrentState = State.Verified };
        }, options);

        _completeBlock = new ActionBlock<TaskState>(state =>
        {
            // 完成逻辑
        });

        _initBlock.LinkTo(_processBlock);
        _processBlock.LinkTo(_verifyBlock);
        _verifyBlock.LinkTo(_completeBlock);
    }
}

public record TaskItem(string Id);
public record TaskState(TaskItem Item, State CurrentState);
public enum State { Initialized, Processed, Verified, Completed }