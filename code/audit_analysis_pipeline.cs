#:sdk Microsoft.NET.Sdk
#:package MemoryPack@2.1.0
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0

using System.Threading.Tasks.Dataflow;
using MemoryPack;

// 高性能分析管道
public class AuditAnalysisPipeline
{
    private readonly TransformBlock<AuditEvent, AnalysisResult> _transformBlock;
    private readonly ActionBlock<AnalysisResult> _actionBlock;
    
    public AuditAnalysisPipeline()
    {
        // 使用MemoryPack进行零拷贝序列化
        _transformBlock = new TransformBlock<AuditEvent, AnalysisResult>(ev => {
            using var memory = MemoryPackSerializer.Serialize(ev);
            return Analyze(memory);
        });
        
        // 使用线程专用内存处理结果
        _actionBlock = new ActionBlock<AnalysisResult>(result => {
            var buffer = ThreadLocalBuffer.Value;
            ProcessResult(result, buffer);
        });
        
        _transformBlock.LinkTo(_actionBlock);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private AnalysisResult Analyze(ReadOnlyMemory<byte> data)
    {
        // 分析逻辑...
    }
}