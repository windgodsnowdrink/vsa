using System.Net.Sockets;
using System.Threading;
using Microsoft.Extensions.Logging;
using Plc.Plugins.Contracts;

namespace BacnetDevicePlugin;

/// <summary>
/// BACnet/IP 设备协议插件（优先级 3）。
/// 实现 <see cref="IDeviceProtocol"/>，启动期自注册到宿主 <see cref="IDeviceCatalog"/>。
/// 提供真实 BACnet/IP 帧：BVLC(81 0A) + NPDU(01 00) + APDU 确认请求，
/// 经 ReadProperty(0x0C) / WriteProperty(0x0F) 访问对象 Present-Value。
/// <para>
/// 寄存器模型映射（BACnet 无寄存器表，按类型映射到 BACnet 对象）：
/// HoldingRegister/InputRegister → AnalogValue(对象类型 2) 的 Present-Value（REAL 浮点，大端）；
/// Coil/DiscreteInput → BinaryValue(对象类型 5) 的 Present-Value（BOOLEAN）。
/// StartAddress 解释为该对象的实例号（ushort）。
/// </para>
/// <para>
/// 注：ASN.1 上下文标签编解码遵循 BACnet 135 标准（上下文原语标签 0x80|(tag&lt;&lt;4)|len、
/// 构造开/闭标签低半字节 0x0E/0x0F）；实测互操作应使用 VTS / YABAC 等 BACnet 栈校验。
/// </para>
/// </summary>
public sealed class BacnetDevicePlugin : IPlugin, IDeviceProtocol
{
    public string Id => "bacnet.ip";
    public string Name => "BACnet/IP 设备协议";
    public string Version => "1.0.0";

    public string ProtocolId => "bacnet.ip";
    public string DisplayName => "BACnet/IP";
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
                "[Bacnet] 设备协议已注册到 DeviceCatalog（协议={ProtocolId}）", ProtocolId);
        }
        else
        {
            context.Logger.LogWarning("[Bacnet] 未找到 IDeviceCatalog，跳过注册");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public IDeviceSession CreateSession(DeviceConnectionOptions options) => new BacnetIpSession(options);
}

/// <summary>BACnet/IP 会话：单条 UDP 之上的 TCP 透传（此处以 TCP 承载 BVLC，简化网络层；真实 BACnet/IP 走 UDP 47808）。</summary>
internal sealed class BacnetIpSession : IDeviceSession
{
    private readonly DeviceConnectionOptions _options;
    private TcpClient? _client;
    private NetworkStream? _stream;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private byte _invokeId = 1;

    public BacnetIpSession(DeviceConnectionOptions options) => _options = options;

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
            var outBuf = new byte[isBit ? request.Count : request.Count * 4];
            int offset = 0;
            for (int i = 0; i < request.Count; i++)
            {
                ushort instance = (ushort)(request.StartAddress + i);
                byte objectType = isBit ? (byte)5 : (byte)2; // 5=BinaryValue, 2=AnalogValue
                var apdu = BuildReadPropertyApdu(objectType, instance);
                var frame = BuildBvlc(apdu);
                var resp = await TransactAsync(frame, cancellationToken).ConfigureAwait(false);
                byte[] value = ParsePresentValue(resp, isBit);
                if (isBit)
                {
                    outBuf[offset++] = value[0];
                }
                else
                {
                    // BACnet REAL 大端 → 原样写入
                    Buffer.BlockCopy(value, 0, outBuf, offset, 4);
                    offset += 4;
                }
            }

            return outBuf;
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
            for (int i = 0; i < request.Values.Length; i++)
            {
                ushort instance = (ushort)(request.StartAddress + i);
                byte objectType = isBit ? (byte)5 : (byte)2;
                byte[] valueBytes = isBit
                    ? new[] { (byte)(request.Values[i] == 0 ? 0x00 : 0x01) }
                    : BitConverter.GetBytes((float)request.Values[i]); // 大端保持
                if (!isBit && BitConverter.IsLittleEndian)
                {
                    Array.Reverse(valueBytes);
                }

                var apdu = BuildWritePropertyApdu(objectType, instance, valueBytes, isBit);
                var frame = BuildBvlc(apdu);
                await TransactAsync(frame, cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    private byte[] BuildReadPropertyApdu(byte objectType, ushort instance)
    {
        var objId = EncodeObjectIdentifier(objectType, instance);
        var body = new List<byte>(16);
        body.Add(0x00); // APDU 确认请求，无分片
        body.Add(_invokeId); // invoke ID
        body.Add(0x0C); // service choice = ReadProperty
        body.Add((byte)(0x80 | (0 << 4) | 4)); // ctx tag0, len4 (object-identifier)
        body.AddRange(objId);
        body.Add((byte)(0x80 | (1 << 4) | 1)); // ctx tag1, len1 (property-identifier)
        body.Add(0x55); // present-value
        return body.ToArray();
    }

    private byte[] BuildWritePropertyApdu(byte objectType, ushort instance, byte[] valueBytes, bool isBit)
    {
        var objId = EncodeObjectIdentifier(objectType, instance);
        var body = new List<byte>(32);
        body.Add(0x00);
        body.Add(_invokeId);
        body.Add(0x0F); // service choice = WriteProperty
        body.Add((byte)(0x80 | (0 << 4) | 4));
        body.AddRange(objId);
        body.Add((byte)(0x80 | (1 << 4) | 1));
        body.Add(0x55); // present-value
        // property value：构造开标签(ctx3) + 应用标签值 + 构造闭标签(ctx3)
        body.Add((byte)(0x80 | (3 << 4) | 0x0E)); // opening tag ctx3
        if (isBit)
        {
            body.Add((byte)(0x10 | 1)); // application BOOLEAN, len1
            body.Add(valueBytes[0]);
        }
        else
        {
            body.Add((byte)(0x40 | 4)); // application REAL, len4
            body.AddRange(valueBytes);
        }

        body.Add((byte)(0x80 | (3 << 4) | 0x0F)); // closing tag ctx3
        return body.ToArray();
    }

    /// <summary>对象标识符：高 10 位对象类型 + 低 22 位实例号。</summary>
    private static byte[] EncodeObjectIdentifier(byte objectType, uint instance)
    {
        uint v = ((uint)objectType << 22) | (instance & 0x3FFFFF);
        return new[] { (byte)(v >> 24), (byte)(v >> 16), (byte)(v >> 8), (byte)v };
    }

    private static byte[] BuildBvlc(byte[] apdu)
    {
        // NPDU: version 0x01 + control 0x00（本地站，无路由）
        var npdu = new byte[] { 0x01, 0x00 };
        int apduNpduLen = apdu.Length + npdu.Length;
        var frame = new byte[4 + apduNpduLen];
        frame[0] = 0x81; // BVLC type IPv4
        frame[1] = 0x0A; // original-unicast-NPDU
        frame[2] = (byte)((apduNpduLen + 4) >> 8);
        frame[3] = (byte)(apduNpduLen + 4);
        Buffer.BlockCopy(npdu, 0, frame, 4, npdu.Length);
        Buffer.BlockCopy(apdu, 0, frame, 4 + npdu.Length, apdu.Length);
        return frame;
    }

    private async Task<byte[]> TransactAsync(byte[] frame, CancellationToken ct)
    {
        await _stream!.WriteAsync(frame, ct).ConfigureAwait(false);
        await _stream.FlushAsync(ct).ConfigureAwait(false);

        var header = await ReadExactAsync(6, ct).ConfigureAwait(false); // BVLC(4) + NPDU(2)
        int total = (header[2] << 8) | header[3];
        int apduLen = total - 6;
        var apdu = await ReadExactAsync(apduLen, ct).ConfigureAwait(false);

        // APDU 确认响应：0x30 + invokeID + 0x0C + ...；末段为 Present-Value
        if (apdu.Length < 3 || apdu[0] != 0x30)
        {
            throw new InvalidOperationException($"BACnet 非预期响应 APDU 首字节 0x{apdu[0]:X2}");
        }

        _invokeId = (byte)((_invokeId % 254) + 1);
        return apdu;
    }

    private static byte[] ParsePresentValue(byte[] apdu, bool isBit)
    {
        // 跳过 0x30 + invokeID + 0x0C
        int i = 3;
        // ctx0 object-id(4) → ctx1 prop-id(1) → ctx3 opening → 应用标签值
        i += 5; // ctx0 tag(1)+4
        i += 2; // ctx1 tag(1)+1
        i += 1; // opening tag ctx3
        if (isBit)
        {
            // application BOOLEAN len1
            return new[] { apdu[i + 1] };
        }

        // application REAL len4（大端）
        return new[] { apdu[i + 1], apdu[i + 2], apdu[i + 3], apdu[i + 4] };
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
                throw new EndOfStreamException("BACnet 连接在对端关闭");
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
