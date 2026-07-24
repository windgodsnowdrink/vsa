#:sdk Microsoft.NET.Sdk.Web
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;

// 1. 雪花算法实现
[SkipLocalsInit]
public sealed class SnowflakeGenerator
{
    private const int TIMESTAMP_BITS = 41;
    private const int WORKER_ID_BITS = 10;
    private const int SEQUENCE_BITS = 12;
    
    private readonly object _lock = new();
    private readonly long _workerId;
    private long _lastTimestamp = -1L;
    private long _sequence = 0L;

    public SnowflakeGenerator(long workerId)
    {
        _workerId = workerId & ((1 << WORKER_ID_BITS) - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public long NextId()
    {
        lock (_lock)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            
            if (timestamp < _lastTimestamp)
                throw new InvalidOperationException("Clock moved backwards");
            
            if (timestamp == _lastTimestamp)
            {
                _sequence = (_sequence + 1) & ((1 << SEQUENCE_BITS) - 1);
                if (_sequence == 0)
                    timestamp = WaitNextMillis(_lastTimestamp);
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = timestamp;
            
            return ((timestamp << (WORKER_ID_BITS + SEQUENCE_BITS))
                   | (_workerId << SEQUENCE_BITS)
                   | _sequence);
        }
    }

    private long WaitNextMillis(long lastTimestamp)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        while (timestamp <= lastTimestamp)
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return timestamp;
    }
}

// 2. 高性能ID生成服务(Disruptor模式)
public sealed class SnowflakeService : BackgroundService
{
    private readonly Channel<IdRequest> _requestChannel;
    private readonly ObjectPool<IdContext> _contextPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly SnowflakeGenerator _generator;

    public SnowflakeService()
    {
        _generator = new SnowflakeGenerator(Environment.MachineName.GetHashCode() & 0x3FF);
        _latencyOptimizer = new TailLatencyOptimizer();
        
        _requestChannel = Channel.CreateBounded<IdRequest>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        _contextPool = new DefaultObjectPool<IdContext>(new IdContextPooledPolicy(), 1000);
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

// 3. 主程序集成
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<SnowflakeService>();
builder.Services.AddHostedService<SnowflakeService>();

var app = builder.Build();
app.MapGet("/snowflake", async (SnowflakeService service) => 
    Results.Ok(await service.GenerateIdAsync()));

app.Run();

// 4. 辅助类
public class IdRequest
{
    public TaskCompletionSource<long> Completion { get; } = new();
}

public class IdContext
{
    public void Process(IdRequest request, SnowflakeGenerator generator)
    {
        request.Completion.SetResult(generator.NextId());
    }
}

public class IdContextPooledPolicy : IPooledObjectPolicy<IdContext>
{
    public IdContext Create() => new IdContext();
    public bool Return(IdContext obj) => true;
}