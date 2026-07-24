#:sdk Microsoft.NET.Sdk.Web
#:package NUlid@2.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using NUlid;
using Microsoft.Extensions.ObjectPool;

// 1. ULID生成服务(Disruptor模式)
[SkipLocalsInit]
public sealed class UlidGenerator : BackgroundService
{
    private readonly Channel<UlidRequest> _requestChannel;
    private readonly ObjectPool<UlidContext> _contextPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    
    public UlidGenerator()
    {
        _latencyOptimizer = new TailLatencyOptimizer();
        
        // Disruptor模式通道配置
        _requestChannel = Channel.CreateBounded<UlidRequest>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        // 上下文对象池
        _contextPool = new DefaultObjectPool<UlidContext>(
            new UlidContextPooledPolicy(), 1000);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<Ulid> GenerateAsync()
    {
        var request = new UlidRequest();
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
                    context.Process(request);
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

// 注册ULID生成服务
builder.Services.AddSingleton<UlidGenerator>();
builder.Services.AddHostedService<UlidGenerator>();

var app = builder.Build();

// ULID生成端点
app.MapGet("/ulid", async (UlidGenerator generator) =>
{
    var ulid = await generator.GenerateAsync();
    return Results.Ok(ulid.ToString());
});

app.Run();

// 3. 辅助类
public class UlidRequest
{
    public TaskCompletionSource<Ulid> Completion { get; } = new();
}

public class UlidContext
{
    [ThreadStatic]
    private static byte[] _ulidBuffer = new byte[16];

    public void Process(UlidRequest request)
    {
        // 使用线程专用内存避免竞争
        var buffer = _ulidBuffer ??= new byte[16];
        var ulid = Ulid.NewUlid();
        request.Completion.SetResult(ulid);
    }
}

public class UlidContextPooledPolicy : IPooledObjectPolicy<UlidContext>
{
    public UlidContext Create() => new UlidContext();
    public bool Return(UlidContext obj) => true;
}

// 4. 实体集成示例
public class Order
{
    public Ulid Id { get; private set; }
    public string Name { get; private set; }

    public Order(string name)
    {
        Id = Ulid.NewUlid();
        Name = name;
    }
}