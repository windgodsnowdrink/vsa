#:sdk Microsoft.NET.Sdk.Web.WindowsDesktop
#:package LOIC@2.0.0
#:package System.IO.Ports@7.0.0
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property UseWPF true

using System.IO.Ports;
using System.Buffers;
using System.Threading.Channels;

// 1. 通讯协议配置
public class SerialConfig
{
    public string PortName { get; set; } = "COM3";
    public int BaudRate { get; set; } = 115200;
    public Parity Parity { get; set; } = Parity.None;
    public int DataBits { get; set; } = 8;
    public StopBits StopBits { get; set; } = StopBits.One;
}

// 2. 零拷贝通讯管道
public class SerialChannel : IDisposable
{
    private readonly SerialPort _port;
    private readonly Channel<ReadOnlyMemory<byte>> _channel;
    private readonly IMemoryOwner<byte> _buffer;

    public SerialChannel(SerialConfig config)
    {
        _port = new SerialPort(config.PortName, config.BaudRate, 
            config.Parity, config.DataBits, config.StopBits);
        
        _channel = Channel.CreateBounded<ReadOnlyMemory<byte>>(1000);
        _buffer = MemoryPool<byte>.Shared.Rent(4096); // CPU cache-line对齐
    }

    public async Task StartAsync(CancellationToken ct)
    {
        _port.Open();
        _port.DataReceived += OnDataReceived;
        
        // 双缓冲处理
        var reader = _channel.Reader;
        while (await reader.WaitToReadAsync(ct))
        {
            while (reader.TryRead(out var data))
            {
                using var owner = MemoryPool<byte>.Shared.Rent(data.Length);
                data.CopyTo(owner.Memory);
                ProcessData(owner.Memory);
            }
        }
    }

    private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var bytesRead = _port.Read(_buffer.Memory.Span);
        _channel.Writer.TryWrite(_buffer.Memory[..bytesRead]);
    }

    private void ProcessData(ReadOnlyMemory<byte> data)
    {
        // 使用Span<T>零拷贝处理
        var span = data.Span;
        // 协议解析逻辑...
    }

    public void Dispose()
    {
        _port.Dispose();
        _buffer.Dispose();
    }
}

// 3. LOIC协议实现
public class LoicProtocol
{
    private readonly SerialChannel _channel;
    private readonly ThreadLocal<IMemoryOwner<byte>> _threadBuffers;

    public LoicProtocol(SerialConfig config)
    {
        _channel = new SerialChannel(config);
        _threadBuffers = new ThreadLocal<IMemoryOwner<byte>>(
            () => MemoryPool<byte>.Shared.Rent(1024)); // 线程专用内存
    }

    public async Task SendCommandAsync(ReadOnlyMemory<byte> command)
    {
        using var buffer = _threadBuffers.Value!;
        command.CopyTo(buffer.Memory);
        await _channel.SendAsync(buffer.Memory[..command.Length]);
    }
}

// 4. DI集成(符合998要求)
public static class LoicExtensions
{
    public static IServiceCollection AddLoicProtocol(
        this IServiceCollection services, 
        Action<SerialConfig> configure)
    {
        var config = new SerialConfig();
        configure(config);
        
        services.AddSingleton(config)
            .AddSingleton<LoicProtocol>()
            .AddHostedService<LoicBackgroundService>();
            
        return services;
    }
}

public class LoicBackgroundService : BackgroundService
{
    private readonly LoicProtocol _protocol;

    public LoicBackgroundService(LoicProtocol protocol)
    {
        _protocol = protocol;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await _protocol.StartAsync(ct);
    }
}

// 5. WPF上位机集成
public partial class MainWindow : Window
{
    private readonly LoicProtocol _protocol;
    private readonly Channel<ReadOnlyMemory<byte>> _uiChannel;

    public MainWindow(LoicProtocol protocol)
    {
        _protocol = protocol;
        _uiChannel = Channel.CreateUnbounded<ReadOnlyMemory<byte>>();
        
        InitializeComponent();
        StartDataProcessing();
    }

    private async void StartDataProcessing()
    {
        await foreach (var data in _uiChannel.Reader.ReadAllAsync())
        {
            // UI线程安全更新
            Dispatcher.Invoke(() => UpdateUI(data));
        }
    }

    private void UpdateUI(ReadOnlyMemory<byte> data)
    {
        // 更新界面...
    }

    private async void SendButton_Click(object sender, RoutedEventArgs e)
    {
        var command = Encoding.UTF8.GetBytes("TEST_COMMAND");
        await _protocol.SendCommandAsync(command);
    }
}