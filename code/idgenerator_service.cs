#:sdk Microsoft.NET.Sdk.Web
#:package IdGen@2.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Threading.Channels;
using IdGen;
using Microsoft.Extensions.ObjectPool;

// 1. 高性能ID生成服务(Disruptor模式)
[SkipLocalsInit]
public sealed class IdGeneratorService : BackgroundService
{
    private readonly Channel<IdRequest> _requestChannel;
    private readonly ObjectPool<IdContext> _contextPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly IdGenerator _generator;

    public IdGeneratorService()
    {
        // 配置雪花算法生成器
        var epoch = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var structure = new IdStructure(45, 2, 16); // 时间戳45位，生成器2位，序列号16位
        var options = new IdGeneratorOptions(structure, new DefaultTimeSource(epoch));
        
        _generator = new IdGenerator(0, options);
        _latencyOptimizer = new TailLatencyOptimizer();
        
        // Disruptor模式通道配置
        _requestChannel = Channel.CreateBounded<IdRequest>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        // 上下文对象池
        _contextPool = new DefaultObjectPool<IdContext>(
            new IdContextPooledPolicy(), 1000);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<long> GenerateIdAsync()
    {
        var request = new IdRequest();
        await _requestChannel.Writer.WriteAsync(request);
        return await request.Completion.Task;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var request in _requestChannel.Reader.ReadAllAsync(stoppingToken))
        {
            var context = _contextPool.Get();
            try
            {
                _latencyOptimizer.Optimize(() => 
                {
                    context.Process(request, _generator);
                });
            }
            finally
            {
                _contextPool.Return(context);
            }
        }
    }
}

// 2. 主程序集成
var builder = WebApplication.CreateBuilder(args);

// 注册ID生成服务
builder.Services.AddSingleton<IdGeneratorService>();
builder.Services.AddHostedService<IdGeneratorService>();

var app = builder.Build();

// ID生成端点
app.MapGet("/id", async (IdGeneratorService generator) =>
{
    var id = await generator.GenerateIdAsync();
    return Results.Ok(id);
});

app.Run();

// 3. 辅助类
public class IdRequest
{
    public TaskCompletionSource<long> Completion { get; } = new();
}

public class IdContext
{
    [ThreadStatic]
    private static byte[] _idBuffer = new byte[8];

    public void Process(IdRequest request, IdGenerator generator)
    {
        var id = generator.CreateId();
        request.Completion.SetResult(id);
    }
}

public class IdContextPooledPolicy : IPooledObjectPolicy<IdContext>
{
    public IdContext Create() => new IdContext();
    public bool Return(IdContext obj) => true;
}

// 4. 实体集成示例
public class Order
{
    public long Id { get; private set; }
    public string Name { get; private set; }

    public Order(string name, IdGeneratorService generator)
    {
        Id = generator.GenerateIdAsync().GetAwaiter().GetResult();
        Name = name;
    }
}