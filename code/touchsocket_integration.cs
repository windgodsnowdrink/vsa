#:sdk Microsoft.NET.Sdk
#:package TouchSocket.Core@2.1.0
#:package TouchSocket.Sockets@2.1.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Buffers;
using System.Net.Sockets;
using TouchSocket.Core;
using TouchSocket.Sockets;

public class TouchSocketOptions
{
    public int MemoryChunkSize { get; set; } = 8192;
    public bool UseTokenRingBuffer { get; set; } = true;
    public bool EnableTailLatencyOptimizer { get; set; } = true;
    public TieredMemoryConfig TieredMemory { get; set; } = new();
    
    // 安全配置
    public SslProtocols SslProtocols { get; set; } = SslProtocols.Tls12 | SslProtocols.Tls13;
    public string CertificatePath { get; set; }
    public string CertificatePassword { get; set; }
    
    // 协议配置
    public int MaxPackageSize { get; set; } = 1024 * 1024; // 1MB
    public string Protocol { get; set; } = "fixedheader"; // fixedheader|lengthprefix|terminator
    
    // 心跳配置
    public int KeepAliveInterval { get; set; } = 30; // 秒
    public int KeepAliveTimeout { get; set; } = 60; // 秒
    
    // 流量控制
    public int MaxConnections { get; set; } = 1000;
    public int ReceiveBufferSize { get; set; } = 8192;
    public int SendBufferSize { get; set; } = 8192;
}

public class TieredMemoryConfig
{
    public int HotMemorySize { get; set; } = 4096;
    public int WarmMemorySize { get; set; } = 32768;
    public int ColdMemorySize { get; set; } = 65536;
}

public interface ITouchSocketService
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}

public class TouchSocketService : ITouchSocketService
{
    private readonly ITokenRingBuffer _tokenRingBuffer;
    private readonly ITailLatencyOptimizer _tailLatencyOptimizer;
    private readonly ITieredMemoryService _tieredMemoryService;
    private readonly IOptions<TouchSocketOptions> _options;
    private TcpService _tcpService;

    public TouchSocketService(
        ITokenRingBuffer tokenRingBuffer,
        ITailLatencyOptimizer tailLatencyOptimizer,
        ITieredMemoryService tieredMemoryService,
        IOptions<TouchSocketOptions> options)
    {
        _tokenRingBuffer = tokenRingBuffer;
        _tailLatencyOptimizer = tailLatencyOptimizer;
        _tieredMemoryService = tieredMemoryService;
        _options = options;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _tcpService = new TcpService();
        _tcpService.Received += OnMessageReceived;
        _tcpService.Disconnected += OnClientDisconnected;
        
        var config = new TouchSocketConfig()
            .SetListenIPHosts(new IPHost[] { new IPHost("0.0.0.0:8080") })
            .ConfigureContainer(a =>
            {
                a.AddConsoleLogger();
                a.AddMemoryPool(_options.Value.MemoryChunkSize);
                
                // 添加协议解析器
                switch (_options.Value.Protocol)
                {
                    case "fixedheader":
                        a.AddFixedHeaderByteProtocol();
                        break;
                    case "lengthprefix":
                        a.AddLengthPrefixByteProtocol();
                        break;
                    case "terminator":
                        a.AddTerminatorByteProtocol("\r\n");
                        break;
                }
            })
            .SetMaxCount(_options.Value.MaxConnections)
            .SetBufferLength(_options.Value.ReceiveBufferSize, _options.Value.SendBufferSize)
            .SetKeepAlive(_options.Value.KeepAliveInterval, _options.Value.KeepAliveTimeout);
            
        // 配置SSL/TLS
        if (!string.IsNullOrEmpty(_options.Value.CertificatePath))
        {
            config.SetSslOption(new SslOption()
            {
                SslProtocols = _options.Value.SslProtocols,
                Certificate = new X509Certificate2(
                    _options.Value.CertificatePath, 
                    _options.Value.CertificatePassword)
            });
        }
            
        await _tcpService.StartAsync(config);
    }
    
    private void OnClientDisconnected(object sender, DisconnectEventArgs e)
    {
        // 客户端断开连接处理
        _tailLatencyOptimizer.Cleanup(e.Socket);
    }

    private void OnMessageReceived(object sender, ReceivedDataEventArgs e)
    {
        using var memoryOwner = _tieredMemoryService.Rent(e.ByteBlock.Length);
        var memory = memoryOwner.Memory;
        e.ByteBlock.CopyTo(memory.Span);
        
        // 处理消息
        _tailLatencyOptimizer.Optimize(e.Socket);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _tcpService.StopAsync();
        _tcpService.Dispose();
    }
}

public static class TouchSocketExtensions
{
    public static IServiceCollection AddTouchSocket(this IServiceCollection services, Action<TouchSocketOptions> configure)
    {
        services.Configure(configure);
        
        services.AddSingleton<ITokenRingBuffer>(sp => 
        {
            var options = sp.GetRequiredService<IOptions<TouchSocketOptions>>();
            return new TokenRingBuffer(Environment.ProcessorCount * 2, options.Value.MemoryChunkSize);
        });
            
        services.AddSingleton<ITailLatencyOptimizer, TailLatencyOptimizer>();
        
        services.AddSingleton<ITieredMemoryService>(sp => 
        {
            var options = sp.GetRequiredService<IOptions<TouchSocketOptions>>();
            return new TieredMemoryService(options.Value.TieredMemory);
        });
        
        // 添加流量监控服务
        services.AddSingleton<ITrafficMonitor, TrafficMonitor>();
        
        services.AddSingleton<ITouchSocketService, TouchSocketService>();
        services.AddHostedService(sp => sp.GetRequiredService<ITouchSocketService>());
        return services;
    }
}

// 流量监控服务实现
public class TrafficMonitor : ITrafficMonitor
{
    private readonly ConcurrentDictionary<string, TrafficStats> _stats = new();
    
    public void RecordIncoming(string clientId, int bytes)
    {
        var stats = _stats.GetOrAdd(clientId, _ => new TrafficStats());
        Interlocked.Add(ref stats.IncomingBytes, bytes);
        Interlocked.Increment(ref stats.IncomingCount);
    }
    
    public void RecordOutgoing(string clientId, int bytes)
    {
        var stats = _stats.GetOrAdd(clientId, _ => new TrafficStats());
        Interlocked.Add(ref stats.OutgoingBytes, bytes);
        Interlocked.Increment(ref stats.OutgoingCount);
    }
    
    public TrafficStats GetStats(string clientId)
    {
        return _stats.TryGetValue(clientId, out var stats) ? stats : new TrafficStats();
    }
}

public interface ITrafficMonitor
{
    void RecordIncoming(string clientId, int bytes);
    void RecordOutgoing(string clientId, int bytes);
    TrafficStats GetStats(string clientId);
}

public class TrafficStats
{
    public long IncomingBytes;
    public long OutgoingBytes;
    public long IncomingCount;
    public long OutgoingCount;
}

// 实现TokenRingBuffer、TailLatencyOptimizer和TieredMemoryService等核心组件
public class TokenRingBuffer : ITokenRingBuffer { /* 实现 */ }
public class TailLatencyOptimizer : ITailLatencyOptimizer { /* 实现 */ }
public class TieredMemoryService : ITieredMemoryService { /* 实现 */ }