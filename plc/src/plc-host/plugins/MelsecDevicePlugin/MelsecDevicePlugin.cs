using System.Net.Sockets;
using System.Threading;
using Microsoft.Extensions.Logging;
using Plc.Plugins.Contracts;

namespace MelsecDevicePlugin;

/// <summary>
/// 三菱 MELSEC MC 协议（Qna-3E / 4E 二进制帧）设备协议插件（优先级 3）。
/// 实现 <see cref="IDeviceProtocol"/>，启动期自注册到宿主 <see cref="IDeviceCatalog"/>。
/// 提供真实 Qna-3E 二进制帧编解码：
/// 读字设备(D/W) 命令 0x0401、读位设备(M/X) 命令 0x0101、
/// 写字设备 命令 0x0414、写位设备 命令 0x0114。基于 System.Net.Sockets，无额外依赖。
/// <para>
/// 寄存器模型映射（本协议无统一寄存器表，按类型映射到三菱软元件）：
/// Coil→M(位，可读写)、DiscreteInput→X(位，只读)、HoldingRegister→D(字，可读写)、InputRegister→W(字，只读)。
/// </para>
/// </summary>
public sealed class MelsecDevicePlugin : IPlugin, IDeviceProtocol
{
    public string Id => "melsec.mc";
    public string Name => "三菱 MC 设备协议 (Qna-3E)";
    public string Version => "1.0.0";

    // IDeviceProtocol
    public string ProtocolId => "melsec.mc";
    public string DisplayName => "MELSEC MC (Qna-3E)";
    public DeviceCapabilities Capabilities => new(
        SupportsRead: true,
        SupportsWrite: true,
        SupportedTypes: new[] { RegisterType.HoldingRegister, RegisterType.InputRegister, RegisterType.Coil, RegisterType.DiscreteInput });

    public Task StartAsync(IPluginContext context, CancellationToken cancellationToken = default)
    {
        var catalog = context.Services.GetService(typeof(IDeviceCatalog)) as IDeviceCatalog;
        if (catalog is not null)
        {
            catalog.Register(this);
            context.Logger.LogInformation(
                "[MelsecMC] 设备协议已注册到 DeviceCatalog（协议={ProtocolId}）", ProtocolId);
        }
        else
        {
            context.Logger.LogWarning("[MelsecMC] 未找到 IDeviceCatalog，跳过注册");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public IDeviceSession CreateSession(DeviceConnectionOptions options) => new MelsecMcSession(options);
}

/// <summary>MC Qna-3E 会话：单条 TCP 连接，按请求构造 4E 帧并收发。MC 帧无事务号，故以信号量串行化请求。</summary>
internal sealed class MelsecMcSession : IDeviceSession
{
    private readonly DeviceConnectionOptions _options;
    private TcpClient? _client;
    private NetworkStream? _stream;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public MelsecMcSession(DeviceConnectionOptions options) => _options = options;

    private async Task EnsureConnectedAsync(CancellationToken ct)
    {
        if (_client is { Connected: true } && _stream is not null)
        {
            return;
        }

        _client = new TcpClient();
        await _client.ConnectAsync(_options.Host, _options.Port, ct).ConfigureAwait(false);
        _client.SendTimeout = _options.TimeoutMs;
        _client.ReceiveTimeout = _options.TimeoutMs;
        _stream = _client.GetStream();
    }

    public async Task<byte[]> ReadAsync(ReadRequest request, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await EnsureConnectedAsync(cancellationToken).ConfigureAwait(false);

            var (deviceCode, command) = MapRead(request.Type);
            var reqData = BuildRequestData(command, deviceCode, request.StartAddress, request.Count);
            var frame = BuildFrame(reqData);

            var body = await TransactAsync(frame, cancellationToken).ConfigureAwait(false);

            // body = [EndCode(2)] + [ResponseData]
            // 字设备：每点 2 字节（小端）；位设备：每点 1 字节（0x01/0x00）
            int dataLen = request.Type is RegisterType.Coil or RegisterType.DiscreteInput
                ? request.Count
                : request.Count * 2;
            if (body.Length < 2 + dataLen)
            {
                throw new InvalidOperationException(
                    $"MC 读响应长度不足：期望≥{2 + dataLen} 字节，实际 {body.Length} 字节");
            }

            return body[2..(2 + dataLen)];
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task WriteAsync(WriteRequest request, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await EnsureConnectedAsync(cancellationToken).ConfigureAwait(false);

            var (deviceCode, command) = MapWrite(request.Type);
            var reqData = BuildWriteRequestData(command, deviceCode, request.StartAddress, request.Type, request.Values);
            var frame = BuildFrame(reqData);

            await TransactAsync(frame, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>构造 4E 请求帧：Subheader(50 00) + Net/PC/IO/Station + 数据长度 + 监视定时器 + 请求数据。</summary>
    private byte[] BuildFrame(byte[] requestData)
    {
        // 请求数据长度 = 监视定时器(2) + 请求数据
        int dataLen = 2 + requestData.Length;
        var frame = new byte[7 + 2 + dataLen];
        int o = 0;
        frame[o++] = 0x50; frame[o++] = 0x00;        // Subheader Qna-3E
        frame[o++] = 0x00;                            // Network No.
        frame[o++] = 0x00;                            // PC No.
        frame[o++] = 0xFF; frame[o++] = 0x03;        // Request destination module I/O No.
        frame[o++] = 0x00;                            // Request destination module station No.
        frame[o++] = (byte)(dataLen & 0xFF);
        frame[o++] = (byte)((dataLen >> 8) & 0xFF);   // Request data length
        int timerUnits = Math.Clamp(_options.TimeoutMs / 250, 1, 0xFFFF);
        frame[o++] = (byte)(timerUnits & 0xFF);
        frame[o++] = (byte)((timerUnits >> 8) & 0xFF); // Monitoring timer (250ms 单位)
        Buffer.BlockCopy(requestData, 0, frame, o, requestData.Length);
        return frame;
    }

    /// <summary>读请求数据：命令(2) + 子命令(2) + 软元件代码(1) + 软元件编号(3,小端) + 点数(2,小端)。</summary>
    private static byte[] BuildRequestData(byte[] command, byte deviceCode, ushort address, ushort count)
    {
        var d = new byte[2 + 2 + 1 + 3 + 2];
        int o = 0;
        d[o++] = command[0]; d[o++] = command[1];   // 命令
        d[o++] = 0x00; d[o++] = 0x00;               // 子命令
        d[o++] = deviceCode;                         // 软元件代码
        d[o++] = (byte)(address & 0xFF);
        d[o++] = (byte)((address >> 8) & 0xFF);
        d[o++] = (byte)((address >> 16) & 0xFF);     // 软元件编号 3 字节小端
        d[o++] = (byte)(count & 0xFF);
        d[o++] = (byte)((count >> 8) & 0xFF);        // 点数
        return d;
    }

    /// <summary>写请求数据：命令 + 子命令 + 软元件 + 点数 + 数据。字设备每点 2 字节小端，位设备每点 1 字节。</summary>
    private static byte[] BuildWriteRequestData(byte[] command, byte deviceCode, ushort address, RegisterType type, ushort[] values)
    {
        bool isBit = type is RegisterType.Coil or RegisterType.DiscreteInput;
        int dataBytes = isBit ? values.Length : values.Length * 2;
        var d = new byte[2 + 2 + 1 + 3 + 2 + dataBytes];
        int o = 0;
        d[o++] = command[0]; d[o++] = command[1];
        d[o++] = 0x00; d[o++] = 0x00;
        d[o++] = deviceCode;
        d[o++] = (byte)(address & 0xFF);
        d[o++] = (byte)((address >> 8) & 0xFF);
        d[o++] = (byte)((address >> 16) & 0xFF);
        d[o++] = (byte)(values.Length & 0xFF);
        d[o++] = (byte)((values.Length >> 8) & 0xFF);
        foreach (var v in values)
        {
            if (isBit)
            {
                d[o++] = v == 0 ? (byte)0x00 : (byte)0x01;
            }
            else
            {
                d[o++] = (byte)(v & 0xFF);
                d[o++] = (byte)((v >> 8) & 0xFF);
            }
        }

        return d;
    }

    /// <summary>发送帧并读取 4E 响应帧，校验结束代码。</summary>
    private async Task<byte[]> TransactAsync(byte[] frame, CancellationToken ct)
    {
        await _stream!.WriteAsync(frame, ct).ConfigureAwait(false);
        await _stream.FlushAsync(ct).ConfigureAwait(false);

        // 响应头：Subheader(2)+Net(1)+PC(1)+IO(2)+Station(1)+Length(2) = 9 字节
        var header = await ReadExactAsync(9, ct).ConfigureAwait(false);
        int len = header[7] | (header[8] << 8); // 响应数据长度 = 结束代码(2) + 响应数据（小端）
        var body = await ReadExactAsync(len, ct).ConfigureAwait(false);

        int endCode = body[0] | (body[1] << 8);
        if (endCode != 0)
        {
            throw new InvalidOperationException($"MC 响应结束代码异常: 0x{endCode:X4}");
        }

        return body; // body[0..1]=结束代码, body[2..]=响应数据
    }

    private async Task<byte[]> ReadExactAsync(int count, CancellationToken ct)
    {
        var buffer = new byte[count];
        int offset = 0;
        while (offset < count)
        {
            int read = await _stream!.ReadAsync(buffer.AsMemory(offset, count - offset), ct).ConfigureAwait(false);
            if (read == 0)
            {
                throw new EndOfStreamException("MC 连接在对端关闭");
            }

            offset += read;
        }

        return buffer;
    }

    private static (byte DeviceCode, byte[] Command) MapRead(RegisterType type) => type switch
    {
        RegisterType.Coil => (0x4D, new byte[] { 0x01, 0x01 }),          // M 位读
        RegisterType.DiscreteInput => (0x58, new byte[] { 0x01, 0x01 }), // X 位读
        RegisterType.HoldingRegister => (0x44, new byte[] { 0x04, 0x01 }), // D 字读
        RegisterType.InputRegister => (0x57, new byte[] { 0x04, 0x01 }),   // W 字读
        _ => (0x44, new byte[] { 0x04, 0x01 }),
    };

    private static (byte DeviceCode, byte[] Command) MapWrite(RegisterType type) => type switch
    {
        RegisterType.Coil => (0x4D, new byte[] { 0x14, 0x01 }),          // M 位写
        RegisterType.HoldingRegister => (0x44, new byte[] { 0x14, 0x04 }), // D 字写
        RegisterType.DiscreteInput => (0x58, new byte[] { 0x14, 0x01 }),   // X（设备侧会拒绝）
        RegisterType.InputRegister => (0x57, new byte[] { 0x14, 0x04 }),     // W（设备侧会拒绝）
        _ => (0x44, new byte[] { 0x14, 0x04 }),
    };

    public async ValueTask DisposeAsync()
    {
        if (_stream is not null)
        {
            await _stream.DisposeAsync().ConfigureAwait(false);
        }

        _client?.Dispose();
        _gate.Dispose();
    }
}
