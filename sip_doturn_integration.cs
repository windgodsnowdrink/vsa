#:sdk Microsoft.NET.Sdk.Web
#:package SIPSorcery@6.0.0
#:package Doturn@2.3.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using System.Net;
using System.Threading.Channels;
using SIPSorcery.SIP;
using Doturn;

// 1. TURN服务器集成(高性能实现)
[SkipLocalsInit]
public sealed class TurnIntegration : BackgroundService
{
    private readonly TurnServer _turnServer;
    private readonly ThreadLocal<Span<byte>> _buffer;
    private readonly ObjectPool<Allocation> _allocationPool;
    private readonly Channel<TurnEvent> _eventChannel;
    
    public TurnIntegration(TurnServer turnServer)
    {
        _turnServer = turnServer;
        _buffer = new(() => stackalloc byte[2048]);
        _allocationPool = new DefaultObjectPool<Allocation>(
            new AllocationPoolPolicy(), 1000);
        _eventChannel = Channel.CreateBounded<TurnEvent>(10000);
        
        _turnServer.OnAllocationCreated += OnAllocation;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var turnEvent in _eventChannel.Reader.ReadAllAsync(ct))
        {
            Span<byte> buffer = _buffer.Value;
            fixed (byte* ptr = buffer)
            {
                if ((long)ptr % 64 == 0)
                {
                    ProcessTurnEvent(turnEvent, buffer);
                }
            }
        }
    }

    private unsafe void OnAllocation(Allocation allocation)
    {
        var buffer = _buffer.Value;
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0)
            {
                _eventChannel.Writer.TryWrite(new TurnEvent(allocation));
            }
        }
    }
}

// 2. SIP信令引擎(集成TURN)
[SkipLocalsInit]
public sealed class SIPSignalingEngine : ISIPSignalingEngine
{
    private readonly SIPTransport _transport;
    private readonly TurnIntegration _turnIntegration;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public SIPSignalingEngine(
        SIPTransport transport,
        TurnIntegration turnIntegration)
    {
        _transport = transport;
        _turnIntegration = turnIntegration;
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
                // 处理SIP请求并通过TURN中继
                await ProcessViaTurn(request, buffer);
            }
        }
    }
}

// 3. 主程序集成
var builder = WebApplication.CreateBuilder();

// 配置TURN服务器
builder.Services.AddSingleton<TurnServer>(_ => 
    new TurnServer(new IPEndPoint(IPAddress.Any, 3478)));

// 注册TURN集成服务
builder.Services.AddHostedService<TurnIntegration>();

// 注册SIP引擎
builder.Services.AddSingleton<ISIPSignalingEngine>(sp => 
    new SIPSignalingEngine(
        new SIPTransport(),
        sp.GetRequiredService<TurnIntegration>()));

var app = builder.Build();
app.MapGet("/", () => "SIP/TURN Integration Ready");
app.Run();