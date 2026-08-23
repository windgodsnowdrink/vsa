using System.Net.Sockets;
using System.Threading;
using Microsoft.Extensions.Logging;
using Plc.Plugins.Contracts;

namespace LoRaDevicePlugin;

/// <summary>
/// LoRaWAN 设备协议插件（优先级 3，模型适配）。
/// 实现 <see cref="IDeviceProtocol"/>，启动期自注册到宿主 <see cref="IDeviceCatalog"/>。
/// <para>
/// LoRaWAN 是帧/包协议，本质没有 Modbus 式寄存器/线圈模型。本插件以「帧编解码中继」方式，
/// 将寄存器 API 适配到 LoRaWAN MAC 帧：真实实现 MHDR/FHDR(FPort/FRMPayload)/MIC 的
/// 编解码，并以 TCP 帧中继与网络服务器(NS)仿真端交互（真实 NS 走 UDP 与 GW 协议，此处为简化传输）。
/// </para>
/// <para>
/// 寄存器模型映射：
/// HoldingRegister 写 → 编码下行 PHYPayload（Values 每 ushort 解释为 2 字节小端 FRMPayload，StartAddress 低 8 位为 FPort）；
/// InputRegister 读 → 向 NS 取回上行 PHYPayload 并解码 FRMPayload 返回；
/// Coil/DiscreteInput → 读/写 MAC 帧 FCtrl 标志位（ADR/ACK 等）。
/// </para>
/// <para>注：MIC 为文档约定的 4 字节累加校验（非真实 AES-128 CMAC）；帧结构遵循 LoRaWAN 1.0 MAC 布局。</para>
/// </summary>
public sealed class LoRaDevicePlugin : IPlugin, IDeviceProtocol
{
    public string Id => "lora.wan";
    public string Name => "LoRaWAN 设备协议（帧编解码中继）";
    public string Version => "1.0.0";

    public string ProtocolId => "lora.wan";
    public string DisplayName => "LoRaWAN (MAC 帧中继)";
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
                "[LoRa] 设备协议已注册到 DeviceCatalog（协议={ProtocolId}）", ProtocolId);
        }
        else
        {
            context.Logger.LogWarning("[LoRa] 未找到 IDeviceCatalog，跳过注册");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public IDeviceSession CreateSession(DeviceConnectionOptions options) => new LoRaWanSession(options);
}

/// <summary>LoRaWAN 会话：单条 TCP 连接，作为帧中继与 NS 仿真端通信。</summary>
internal sealed class LoRaWanSession : IDeviceSession
{
    private readonly DeviceConnectionOptions _options;
    private TcpClient? _client;
    private NetworkStream? _stream;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private byte _downlinkFctrl;

    // 会话级常量（真实部署应由设备配置注入）
    private const uint DevAddr = 0x01020304;

    public LoRaWanSession(DeviceConnectionOptions options) => _options = options;

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

            bool isFlag = request.Type is RegisterType.Coil or RegisterType.DiscreteInput;
            byte[] frame = isFlag ? await FetchFrameAsync(0x46, request.StartAddress, cancellationToken).ConfigureAwait(false)
                                  : await FetchFrameAsync(0x55, request.StartAddress, cancellationToken).ConfigureAwait(false);
            if (frame.Length == 0)
            {
                return isFlag ? new byte[] { 0 } : Array.Empty<byte>();
            }

            var decoded = DecodeFrame(frame);
            if (isFlag)
            {
                return new[] { decoded.FCtrl };
            }

            return decoded.FrmPayload;
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

            bool isFlag = request.Type is RegisterType.Coil or RegisterType.DiscreteInput;
            if (isFlag)
            {
                // 设置下行 FCtrl 标志
                _downlinkFctrl = (byte)(request.Values.Length > 0 ? request.Values[0] & 0xFF : 0);
                return;
            }

            byte fPort = (byte)(request.StartAddress & 0xFF);
            var frm = new byte[request.Values.Length * 2];
            for (int i = 0; i < request.Values.Length; i++)
            {
                frm[i * 2] = (byte)request.Values[i];
                frm[i * 2 + 1] = (byte)(request.Values[i] >> 8);
            }

            var downlink = EncodeFrame(mtype: 3, fctrl: _downlinkFctrl, fPort, frm);
            await SendDownlinkAsync(downlink, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task SendDownlinkAsync(byte[] frame, CancellationToken ct)
    {
        var msg = new byte[1 + 4 + frame.Length];
        msg[0] = 0xDD; // 下行帧标记
        msg[1] = (byte)(frame.Length & 0xFF);
        msg[2] = (byte)((frame.Length >> 8) & 0xFF);
        msg[3] = (byte)((frame.Length >> 16) & 0xFF);
        msg[4] = (byte)((frame.Length >> 24) & 0xFF);
        Buffer.BlockCopy(frame, 0, msg, 5, frame.Length);
        await _stream!.WriteAsync(msg, ct).ConfigureAwait(false);
        await _stream.FlushAsync(ct).ConfigureAwait(false);

        var ack = await ReadExactAsync(1, ct).ConfigureAwait(false);
        if (ack[0] != 0x00)
        {
            throw new InvalidOperationException($"LoRa NS 拒绝下行帧，状态 0x{ack[0]:X2}");
        }
    }

    private async Task<byte[]> FetchFrameAsync(byte tag, ushort port, CancellationToken ct)
    {
        var req = new[] { tag, (byte)(port & 0xFF) };
        await _stream!.WriteAsync(req, ct).ConfigureAwait(false);
        await _stream.FlushAsync(ct).ConfigureAwait(false);

        var status = await ReadExactAsync(1, ct).ConfigureAwait(false);
        if (status[0] == 0xFF)
        {
            return Array.Empty<byte>();
        }

        var lenBuf = await ReadExactAsync(4, ct).ConfigureAwait(false);
        int len = lenBuf[0] | (lenBuf[1] << 8) | (lenBuf[2] << 16) | (lenBuf[3] << 24);
        return await ReadExactAsync(len, ct).ConfigureAwait(false);
    }

    // ---- LoRaWAN MAC 帧编解码（结构与 LoRaWAN 1.0 一致；MIC 为文档约定校验）----

    private static byte[] EncodeFrame(byte mtype, byte fctrl, byte fPort, byte[] frmPayload)
    {
        var body = new List<byte>(32);
        // FHDR: DevAddr(4 LE) + FCtrl(1) + FCnt(2 LE)
        body.Add((byte)(DevAddr & 0xFF));
        body.Add((byte)((DevAddr >> 8) & 0xFF));
        body.Add((byte)((DevAddr >> 16) & 0xFF));
        body.Add((byte)((DevAddr >> 24) & 0xFF));
        body.Add(fctrl);
        body.Add(0x01); // FCnt L
        body.Add(0x00); // FCnt H
        body.Add(fPort);
        body.AddRange(frmPayload);

        var mic = Mic4(body.ToArray());
        var frame = new byte[1 + body.Count + 4];
        frame[0] = (byte)(mtype << 5); // MHDR, Major=0
        Buffer.BlockCopy(body.ToArray(), 0, frame, 1, body.Count);
        Buffer.BlockCopy(mic, 0, frame, 1 + body.Count, 4);
        return frame;
    }

    private static (byte MType, uint DevAddr, byte FCtrl, byte FPort, byte[] FrmPayload, byte[] Mic)
        DecodeFrame(byte[] frame)
    {
        byte mtype = (byte)(frame[0] >> 5);
        uint devAddr = (uint)(frame[1] | (frame[2] << 8) | (frame[3] << 16) | (frame[4] << 24));
        byte fctrl = frame[5];
        byte fPort = frame[8]; // FHDR = DevAddr(4)+FCtrl(1)+FCnt(2)，故 FPort 在索引 8
        int payloadLen = frame.Length - 13; // 减去 MHDR(1)+FHDR(7)+FPort(1)+MIC(4)
        var frm = new byte[Math.Max(payloadLen, 0)];
        if (payloadLen > 0)
        {
            Buffer.BlockCopy(frame, 9, frm, 0, payloadLen); // FRMPayload 自索引 9 起
        }

        var mic = new byte[4];
        Buffer.BlockCopy(frame, frame.Length - 4, mic, 0, 4);
        return (mtype, devAddr, fctrl, fPort, frm, mic);
    }

    private static byte[] Mic4(byte[] body)
    {
        uint sum = 0;
        foreach (byte b in body)
        {
            sum = (sum + b) & 0xFFFFFFFF;
        }

        return new[] { (byte)sum, (byte)(sum >> 8), (byte)(sum >> 16), (byte)(sum >> 24) };
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
                throw new EndOfStreamException("LoRa 连接在对端关闭");
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
        _gate.Dispose();
    }
}
