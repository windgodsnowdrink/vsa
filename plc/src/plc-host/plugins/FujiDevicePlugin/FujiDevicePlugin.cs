using System.Net.Sockets;
using System.Threading;
using Microsoft.Extensions.Logging;
using Plc.Plugins.Contracts;

namespace FujiDevicePlugin;

/// <summary>
/// 富士电机(Fuji Electric) MICREX-SX SPH 系列 NP1 通用通信模块 Loader 协议设备协议插件（优先级 3）。
/// 实现 <see cref="IDeviceProtocol"/>，启动期自注册到宿主 <see cref="IDeviceCatalog"/>。
/// 提供真实 NP1 Loader 二进制帧传输：START(5Ah) + DataCount(L,H) + ProcessingStatus(FFh) +
/// ConnectionID(L,H) + Mode(11h) + 7×00 + DataBytes(L,H) + [16 字节命令头 + 数据] + BCC(校验和)。
/// <para>
/// 寄存器模型映射：HoldingRegister/InputRegister → 字设备(类型 0x01)；Coil/DiscreteInput → 位设备(类型 0x02)。
/// 命令头(16 字节)：[0]命令(01h 读/02h 写)、[1]设备类型、[2..5]起始地址(小端)、[6..7]点数(小端)、[8..15]保留。
/// StartAddress 解释为设备编号（ushort）。BCC = 自 START 至数据末字节累加低 8 位。
/// （注：NP1 Loader 帧传输层严格按 Fuji 手册；命令头布局为本文档约定，实测互操作需以富士 NP1 手册为准。）
/// </para>
/// </summary>
public sealed class FujiDevicePlugin : IPlugin, IDeviceProtocol
{
    public string Id => "fuji.sph";
    public string Name => "富士 MICREX-SPH (NP1) 设备协议";
    public string Version => "1.0.0";

    public string ProtocolId => "fuji.sph";
    public string DisplayName => "Fuji MICREX-SPH (NP1)";
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
                "[Fuji] 设备协议已注册到 DeviceCatalog（协议={ProtocolId}）", ProtocolId);
        }
        else
        {
            context.Logger.LogWarning("[Fuji] 未找到 IDeviceCatalog，跳过注册");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public IDeviceSession CreateSession(DeviceConnectionOptions options) => new FujiSphSession(options);
}

/// <summary>NP1 Loader 会话：单条 TCP 连接，按请求构造二进制帧并收发。</summary>
internal sealed class FujiSphSession : IDeviceSession
{
    private readonly DeviceConnectionOptions _options;
    private TcpClient? _client;
    private NetworkStream? _stream;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public FujiSphSession(DeviceConnectionOptions options) => _options = options;

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

            bool isBit = request.Type is RegisterType.Coil or RegisterType.DiscreteInput;
            byte devType = isBit ? (byte)0x02 : (byte)0x01;
            var header = BuildCommandHeader(0x01, devType, request.StartAddress, request.Count);
            var frame = BuildFrame(header, Array.Empty<byte>());
            var resp = await TransactAsync(frame, cancellationToken).ConfigureAwait(false);
            byte[] payload = GetResponsePayload(resp);

            if (isBit)
            {
                var outBuf = new byte[request.Count];
                for (int i = 0; i < request.Count; i++)
                {
                    outBuf[i] = payload[i];
                }

                return outBuf;
            }

            var words = new byte[request.Count * 2];
            for (int i = 0; i < request.Count; i++)
            {
                words[i * 2] = payload[i * 2];
                words[i * 2 + 1] = payload[i * 2 + 1];
            }

            return words;
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

            bool isBit = request.Type is RegisterType.Coil or RegisterType.DiscreteInput;
            byte devType = isBit ? (byte)0x02 : (byte)0x01;
            var header = BuildCommandHeader(0x02, devType, request.StartAddress, (ushort)request.Values.Length);

            var payload = new byte[isBit ? request.Values.Length : request.Values.Length * 2];
            for (int i = 0; i < request.Values.Length; i++)
            {
                if (isBit)
                {
                    payload[i] = (byte)(request.Values[i] == 0 ? 0 : 1);
                }
                else
                {
                    payload[i * 2] = (byte)request.Values[i];
                    payload[i * 2 + 1] = (byte)(request.Values[i] >> 8);
                }
            }

            var frame = BuildFrame(header, payload);
            await TransactAsync(frame, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    private static byte[] BuildCommandHeader(byte command, byte devType, ushort address, ushort count)
    {
        var h = new byte[16];
        h[0] = command;
        h[1] = devType;
        h[2] = (byte)(address & 0xFF);
        h[3] = (byte)((address >> 8) & 0xFF);
        h[4] = 0;
        h[5] = 0;
        h[6] = (byte)(count & 0xFF);
        h[7] = (byte)((count >> 8) & 0xFF);
        return h;
    }

    private byte[] BuildFrame(byte[] header, byte[] payload)
    {
        int n = header.Length + payload.Length; // Data 字段（命令头 + 数据）
        int dc = n + 14; // DataCount = 命令(ProcessingStatus..Data) + BCC
        var frame = new byte[3 + dc];
        int o = 0;
        frame[o++] = 0x5A; // START
        frame[o++] = (byte)(dc & 0xFF);
        frame[o++] = (byte)((dc >> 8) & 0xFF);
        frame[o++] = 0xFF; // Processing status (request)
        frame[o++] = (byte)(_options.UnitId & 0xFF);
        frame[o++] = 0; // Connection ID H
        frame[o++] = 0x11; // Connection mode
        for (int i = 0; i < 7; i++)
        {
            frame[o++] = 0x00;
        }

        frame[o++] = (byte)(n & 0xFF);
        frame[o++] = (byte)((n >> 8) & 0xFF);
        Buffer.BlockCopy(header, 0, frame, o, header.Length);
        o += header.Length;
        Buffer.BlockCopy(payload, 0, frame, o, payload.Length);
        o += payload.Length;

        frame[o] = Bcc(frame, o); // BCC over all bytes so far
        return frame;
    }

    private static byte Bcc(byte[] data, int length)
    {
        int sum = 0;
        for (int i = 0; i < length; i++)
        {
            sum = (sum + data[i]) & 0xFF;
        }

        return (byte)sum;
    }

    private async Task<byte[]> TransactAsync(byte[] frame, CancellationToken ct)
    {
        await _stream!.WriteAsync(frame, ct).ConfigureAwait(false);
        await _stream.FlushAsync(ct).ConfigureAwait(false);

        var head = await ReadExactAsync(3, ct).ConfigureAwait(false);
        if (head[0] != 0x5A)
        {
            throw new InvalidOperationException($"NP1 响应缺少 START(5Ah)，收到 0x{head[0]:X2}");
        }

        int dc = head[1] | (head[2] << 8);
        var block = await ReadExactAsync(dc, ct).ConfigureAwait(false);

        // block[0] = Processing status（响应），00 为正常
        if (block[0] != 0x00)
        {
            throw new InvalidOperationException($"NP1 设备返回处理状态异常: 0x{block[0]:X2}");
        }

        return block;
    }

    private static byte[] GetResponsePayload(byte[] block)
    {
        // block: [ProcessingStatus][ConnIdL][ConnIdH][Mode][7×00][NumBytesL][NumBytesH][Data...]
        int numBytes = block[11] | (block[12] << 8);
        var data = new byte[numBytes];
        Buffer.BlockCopy(block, 13, data, 0, numBytes);
        // data = 16 字节命令头 + 实际负载
        int payloadLen = numBytes - 16;
        if (payloadLen <= 0)
        {
            return Array.Empty<byte>();
        }

        var payload = new byte[payloadLen];
        Buffer.BlockCopy(data, 16, payload, 0, payloadLen);
        return payload;
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
                throw new EndOfStreamException("NP1 连接在对端关闭");
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
