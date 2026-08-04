#:sdk Microsoft.NET.Sdk.Web
#:package DotNetCore.Natasha.CSharp@5.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Runtime.CompilerServices;
using DotNetCore.Natasha;
using DotNetCore.Natasha.CSharp;
using System.Threading.Channels;
using System.Buffers;

[SkipLocalsInit]
public sealed class NatashaDynamicService : IAsyncDisposable
{
    private readonly NatashaDomain _domain;
    private readonly Channel<(string, TaskCompletionSource<Delegate>)> _compileChannel;
    private readonly CancellationTokenSource _cts;
    private readonly MemoryPool<byte> _memoryPool;
    private readonly ObjectPool<StringBuilder> _scriptBuilderPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public NatashaDynamicService()
    {
        _domain = new NatashaDomain("ProductionDomain");
        _cts = new CancellationTokenSource();
        _latencyOptimizer = new TailLatencyOptimizer();
        
        _compileChannel = Channel.CreateBounded<(string, TaskCompletionSource<Delegate>)>(
            new BoundedChannelOptions(10_000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
        
        _memoryPool = new TieredMemoryPool();
        _scriptBuilderPool = new DefaultObjectPool<StringBuilder>(
            new StringBuilderPooledPolicy(), 
            Environment.ProcessorCount * 2);
        
        _ = Task.Run(ProcessCompileRequestsAsync);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async ValueTask<TDelegate> CompileAsync<TDelegate>(string script) 
        where TDelegate : Delegate
    {
        using var latencyToken = _latencyOptimizer.BeginOperation();
        var tcs = new TaskCompletionSource<Delegate>();
        await _compileChannel.Writer.WriteAsync((script, tcs), _cts.Token);
        return (TDelegate)await tcs.Task;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessCompileRequestsAsync()
    {
        await foreach (var (script, tcs) in _compileChannel.Reader.ReadAllAsync(_cts.Token))
        {
            try
            {
                var builder = _scriptBuilderPool.Get();
                try
                {
                    builder.AppendLine("using System;");
                    builder.AppendLine("using System.Runtime.CompilerServices;");
                    builder.AppendLine("[SkipLocalsInit]");
                    builder.AppendLine("public static class DynamicClass {");
                    builder.AppendLine(script);
                    builder.AppendLine("}");
                    
                    using var memory = _memoryPool.Rent(builder.Length);
                    builder.ToString().AsSpan().CopyTo(memory.Memory.Span);
                    
                    var assembly = _domain.CreateAssembly(memory.Memory);
                    var type = assembly.GetType("DynamicClass");
                    var method = type.GetMethod("Execute");
                    
                    tcs.SetResult(Delegate.CreateDelegate(
                        typeof(Action<>).MakeGenericType(method.GetParameters()[0].ParameterType),
                        method));
                }
                finally
                {
                    _scriptBuilderPool.Return(builder);
                }
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _compileChannel.Writer.Complete();
        await _compileChannel.Reader.Completion;
        _domain.Dispose();
    }
}

[StructLayout(LayoutKind.Auto)]
internal sealed class TieredMemoryPool : MemoryPool<byte>
{
    protected override void Dispose(bool disposing) { }
    
    public override IMemoryOwner<byte> Rent(int minBufferSize = -1)
    {
        return minBufferSize > 1024 ? 
            new LargeMemoryOwner(minBufferSize) : 
            new SmallMemoryOwner(minBufferSize);
    }
    
    public override int MaxBufferSize => 1024 * 1024; // 1MB
}

[SkipLocalsInit]
internal sealed class SmallMemoryOwner : IMemoryOwner<byte>
{
    private byte[]? _array;
    
    public SmallMemoryOwner(int size)
    {
        _array = GC.AllocateArray<byte>(size, pinned: true);
    }
    
    public Memory<byte> Memory => _array ?? throw new ObjectDisposedException(nameof(SmallMemoryOwner));
    
    public void Dispose()
    {
        if (_array == null) return;
        Array.Clear(_array, 0, _array.Length);
        _array = null;
    }
}

[SkipLocalsInit]
public class StringBuilderPooledPolicy : IPooledObjectPolicy<StringBuilder>
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public StringBuilder Create() => new(4096);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public bool Return(StringBuilder obj)
    {
        obj.Clear();
        return true;
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<NatashaDynamicService>();

var app = builder.Build();
app.MapGet("/", () => "Natasha Dynamic Service Running");
app.Run();