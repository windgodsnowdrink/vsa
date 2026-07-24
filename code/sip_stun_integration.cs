#:sdk Microsoft.NET.Sdk.Web
#:package SIPSorcery@6.0.0
#:package SIPSorcery.Net@6.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Net;
using System.Threading.Channels;
using SIPSorcery.SIP;
using SIPSorcery.SIP.App;
using SIPSorcery.Net;

// 1. STUN客户端实现(零拷贝优化)
[SkipLocalsInit]
public sealed class STUNClient : IDisposable
{
    private readonly ThreadLocal<Span<byte>> _buffer;
    private readonly ObjectPool<STUNMessage> _messagePool;
    private readonly ChannelWriter<STUNResult> _resultChannel;
    
    public STUNClient(Channel<STUNResult> resultChannel)
    {
        _buffer = new(() => stackalloc byte[512]);
        _messagePool = new DefaultObjectPool<STUNMessage>(
            new STUNMessagePooledPolicy(), 1000);
        _resultChannel = resultChannel.Writer;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe Task QueryAsync(IPEndPoint stunServer)
    {
        var buffer = _buffer.Value;
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0) // Cache-line对齐
            {
                var message = _messagePool.Get();
                try
                {
                    // 构建STUN绑定请求
                    message.Header = new STUNHeader()
                    {
                        MessageType = STUNMessageTypesEnum.BindingRequest,
                        TransactionId = Guid.NewGuid().ToString().Substring(0, 12)
                    };
                    
                    // 发送请求并处理响应
                    // ... STUN协议处理逻辑 ...
                    
                    _resultChannel.TryWrite(new STUNResult(stunServer, message));
                }
                finally
                {
                    _messagePool.Return(message);
                }
            }
        }
        return Task.CompletedTask;
    }
}

// 2. SIP信令引擎(集成STUN)
[SkipLocalsInit]
public sealed class SIPSignalingEngine : ISIPSignalingEngine
{
    private readonly SIPTransport _transport;
    private readonly ChannelWriter<SIPEvent> _eventChannel;
    private readonly ThreadLocal<Span<byte>> _buffer;
    private readonly STUNClient _stunClient;
    
    public SIPSignalingEngine(
        SIPTransport transport,
        Channel<SIPEvent> eventChannel,
        STUNClient stunClient)
    {
        _transport = transport;
        _eventChannel = eventChannel.Writer;
        _buffer = new(() => stackalloc byte[1024]);
        _stunClient = stunClient;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ProcessRequestAsync(SIPRequest request)
    {
        Span<byte> buffer = _buffer.Value;
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0)
            {
                // 处理SIP请求前先检查NAT状态
                await _stunClient.QueryAsync(new IPEndPoint(IPAddress.Parse("stun.sipsorcery.com"), 3478));
                
                // ... SIP消息处理逻辑 ...
                await _eventChannel.WriteAsync(new SIPEvent(request));
            }
        }
    }
}

// 3. 主程序集成
var builder = WebApplication.CreateBuilder();

// 配置高性能通道
var sipChannel = Channel.CreateBounded<SIPEvent>(10000);
var stunChannel = Channel.CreateBounded<STUNResult>(10000);

// 注册STUN客户端
builder.Services.AddSingleton<STUNClient>(sp => 
    new STUNClient(stunChannel));

// 注册SIP引擎
builder.Services.AddSingleton<ISIPSignalingEngine>(sp => 
    new SIPSignalingEngine(
        new SIPTransport(),
        sipChannel,
        sp.GetRequiredService<STUNClient>()));

// 配置后台服务
builder.Services.AddHostedService<SIPEventProcessor>();

var app = builder.Build();
app.MapGet("/", () => "SIP/STUN Integration Ready");
app.Run();