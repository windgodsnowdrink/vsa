#:sdk Microsoft.NET.Sdk.Web
#:package SIPSorcery@6.0.0
#:package DotNetZip@1.16.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using System.Net;
using System.Threading.Channels;
using SIPSorcery.SIP;
using SIPSorcery.Net;

// 1. Coturn客户端集成(高性能实现)
[SkipLocalsInit]
public sealed class CoturnClient : IDisposable
{
    private readonly ThreadLocal<Span<byte>> _buffer;
    private readonly ObjectPool<TurnAllocation> _allocationPool;
    private readonly Channel<CoturnEvent> _eventChannel;
    
    public CoturnClient()
    {
        _buffer = new(() => stackalloc byte[2048]);
        _allocationPool = new DefaultObjectPool<TurnAllocation>(
            new TurnAllocationPooledPolicy(), 1000);
        _eventChannel = Channel.CreateBounded<CoturnEvent>(10000);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe Task<IPEndPoint> AllocateAsync(string turnServer, string username, string password)
    {
        var buffer = _buffer.Value;
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0) // Cache-line对齐
            {
                var allocation = _allocationPool.Get();
                try
                {
                    // 连接Coturn服务器并创建分配
                    // ... Coturn协议处理逻辑 ...
                    
                    _eventChannel.Writer.TryWrite(new CoturnEvent(allocation));
                    return Task.FromResult(allocation.RelayEndPoint);
                }
                finally
                {
                    _allocationPool.Return(allocation);
                }
            }
        }
        return Task.FromResult<IPEndPoint>(null);
    }
}

// 2. SIP信令引擎(集成Coturn)
[SkipLocalsInit]
public sealed class SIPSignalingEngine : ISIPSignalingEngine
{
    private readonly SIPTransport _transport;
    private readonly CoturnClient _coturnClient;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public SIPSignalingEngine(
        SIPTransport transport,
        CoturnClient coturnClient)
    {
        _transport = transport;
        _coturnClient = coturnClient;
        _buffer = new(() => stackalloc byte[4096]);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ProcessRequestAsync(SIPRequest request)
    {
        Span<byte> buffer = _buffer.Value;
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0)
            {
                // 通过Coturn中继处理SIP请求
                var relayEP = await _coturnClient.AllocateAsync(
                    "turn.example.com", 
                    "username", 
                    "password");
                
                // ... SIP消息处理逻辑 ...
            }
        }
    }
}

// 3. 主程序集成
var builder = WebApplication.CreateBuilder();

// 配置Coturn客户端
builder.Services.AddSingleton<CoturnClient>();

// 注册SIP引擎
builder.Services.AddSingleton<ISIPSignalingEngine>(sp => 
    new SIPSignalingEngine(
        new SIPTransport(),
        sp.GetRequiredService<CoturnClient>()));

var app = builder.Build();
app.MapGet("/", () => "SIP/Coturn Integration Ready");
app.Run();