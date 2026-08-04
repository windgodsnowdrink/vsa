#:sdk Microsoft.NET.Sdk.Web
#:package System.Threading.Tasks.Dataflow@8.0.0
#:package DotNetCore.CAP@7.2.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Threading.Tasks.Dataflow;
using DotNetCore.CAP;

public class DistributedTransactionCoordinator
{
    private readonly TransformBlock<TransactionCommand, TransactionResult> _prepareBlock;
    private readonly BatchBlock<TransactionResult> _batchBlock;
    private readonly ActionBlock<TransactionResult[]> _commitBlock;
    private readonly ICapPublisher _capPublisher;

    public DistributedTransactionCoordinator(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
        
        var options = new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 10000,
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            EnsureOrdered = false
        };

        _prepareBlock = new TransformBlock<TransactionCommand, TransactionResult>(cmd =>
        {
            // 准备阶段逻辑
            return new TransactionResult(cmd.Id, true);
        }, options);

        _batchBlock = new BatchBlock<TransactionResult>(100);
        _commitBlock = new ActionBlock<TransactionResult[]>(async results =>
        {
            using var trans = _capPublisher.BeginTransaction();
            try
            {
                // 提交事务逻辑
                await _capPublisher.PublishAsync("transaction.commit", results);
                trans.Commit();
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }, options);

        _prepareBlock.LinkTo(_batchBlock);
        _batchBlock.LinkTo(_commitBlock);
    }
}

public record TransactionCommand(string Id);
public record TransactionResult(string Id, bool Success);