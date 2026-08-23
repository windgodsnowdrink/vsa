using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Plc.Plugins.Contracts;

namespace DemoDevicePlugin;

/// <summary>
/// Modbus TCP 设备协议插件（优先级 3 参考实现）。
/// 实现 <see cref="IDeviceProtocol"/>，启动期自注册到宿主 <see cref="IDeviceCatalog"/>。
/// 提供真实 MBAP+PDU 帧：读保持/输入寄存器(0x03/0x04)、读线圈/离散输入(0x01/0x02)、
/// 写单寄存器(0x06)、写多寄存器(0x10)。基于 System.Net.Sockets，无额外依赖。
/// </summary>
public sealed class ModbusTcpDevicePlugin : IPlugin, IDeviceProtocol
{
    public string Id => "modbus.tcp";
    public string Name => "Modbus TCP 设备协议";
    public string Version => "1.0.0";

    // IDeviceProtocol
    public string ProtocolId => "modbus.tcp";
    public string DisplayName => "Modbus TCP";
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
                "[ModbusTcp] 设备协议已注册到 DeviceCatalog（协议={ProtocolId}）", ProtocolId);
        }
        else
        {
            context.Logger.LogWarning("[ModbusTcp] 未找到 IDeviceCatalog，跳过注册");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public IDeviceSession CreateSession(DeviceConnectionOptions options) => new ModbusTcpSession(options);
}

/// <summary>Modbus TCP 会话：一次 TCP 连接，按请求构造 MBAP+PDU 并收发。</summary>
internal sealed class ModbusTcpSession : IDeviceSession
{
    private readonly DeviceConnectionOptions _options;
    private TcpClient? _client;
    private NetworkStream? _stream;
    private ushort _transactionId;

    public ModbusTcpSession(DeviceConnectionOptions options) => _options = options;

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
        await EnsureConnectedAsync(cancellationToken).ConfigureAwait(false);

        // 读功能码映射：Holding=0x03, Input=0x04, Coil=0x01, DiscreteInput=0x02
        byte functionCode = request.Type switch
        {
            RegisterType.HoldingRegister => 0x03,
            RegisterType.InputRegister => 0x04,
            RegisterType.Coil => 0x01,
            RegisterType.DiscreteInput => 0x02,
            _ => 0x03,
        };

        var pdu = new byte[5];
        pdu[0] = functionCode;
        pdu[1] = (byte)(request.StartAddress >> 8);
        pdu[2] = (byte)request.StartAddress;
        pdu[3] = (byte)(request.Count >> 8);
        pdu[4] = (byte)request.Count;

        var body = await TransactAsync(pdu, cancellationToken).ConfigureAwait(false);

        // body[0]=FC; body[1]=byteCount; 之后为数据
        int byteCount = body[1];
        return body[2..(2 + byteCount)];
    }

    public async Task WriteAsync(WriteRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureConnectedAsync(cancellationToken).ConfigureAwait(false);

        byte[] pdu;
        if (request.Values.Length == 1 && request.Type == RegisterType.HoldingRegister)
        {
            // 写单寄存器 0x06
            pdu = new byte[5];
            pdu[0] = 0x06;
            pdu[1] = (byte)(request.StartAddress >> 8);
            pdu[2] = (byte)request.StartAddress;
            pdu[3] = (byte)(request.Values[0] >> 8);
            pdu[4] = (byte)request.Values[0];
        }
        else
        {
            // 写多寄存器 0x10
            var list = new List<byte>(7 + request.Values.Length * 2);
            list.Add(0x10);
            list.Add((byte)(request.StartAddress >> 8));
            list.Add((byte)request.StartAddress);
            list.Add((byte)(request.Values.Length >> 8));
            list.Add((byte)request.Values.Length);
            list.Add((byte)(request.Values.Length * 2));
            foreach (var v in request.Values)
            {
                list.Add((byte)(v >> 8));
                list.Add((byte)v);
            }

            pdu = list.ToArray();
        }

        await TransactAsync(pdu, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>构造 MBAP 头 + PDU 发送，读取并校验 MBAP+响应体。</summary>
    private async Task<byte[]> TransactAsync(byte[] pdu, CancellationToken ct)
    {
        var tid = ++_transactionId;
        var mbap = new byte[7];
        mbap[0] = (byte)(tid >> 8);
        mbap[1] = (byte)tid;
        mbap[2] = 0;
        mbap[3] = 0; // Protocol Id = 0 (Modbus)
        mbap[4] = (byte)((pdu.Length + 1) >> 8);
        mbap[5] = (byte)(pdu.Length + 1); // Length = UnitId(1) + PDU
        mbap[6] = _options.UnitId;

        var frame = new byte[mbap.Length + pdu.Length];
        Buffer.BlockCopy(mbap, 0, frame, 0, mbap.Length);
        Buffer.BlockCopy(pdu, 0, frame, mbap.Length, pdu.Length);

        await _stream!.WriteAsync(frame, ct).ConfigureAwait(false);
        await _stream.FlushAsync(ct).ConfigureAwait(false);

        // 读 MBAP(7) 以获取后续长度
        var header = await ReadExactAsync(7, ct).ConfigureAwait(false);
        int len = (header[4] << 8) | header[5];
        // Modbus TCP 的 UnitId 已包含在 7 字节 MBAP 的第 7 字节（header[6]），
        // Length 字段 = UnitId(1) + PDU。因此 MBAP 之后只需读取 (len-1) 字节的纯 PDU：
        // body[0]=功能码、body[1]=字节计数、body[2..]=数据，异常位判定才正确。
        var body = await ReadExactAsync(len - 1, ct).ConfigureAwait(false);

        // body[0]=FC；若高位置位则为异常响应（body[1]=异常码）
        if ((body[0] & 0x80) != 0)
        {
            throw new InvalidOperationException($"Modbus 异常响应，异常码={body[1]}");
        }

        return body;
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
                throw new EndOfStreamException("Modbus TCP 连接在对端关闭");
            }

            offset += read;
        }

        return buffer;
    }

    public async ValueTask DisposeAsync()
    {
        if (_stream is not null)
        {
            await _stream.DisposeAsync().ConfigureAwait(false);
        }

        _client?.Dispose();
    }
}
