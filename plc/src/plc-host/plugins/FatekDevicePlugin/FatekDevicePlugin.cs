using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Plc.Plugins.Contracts;

namespace FatekDevicePlugin;

/// <summary>
/// 永宏(FATEK) PLC 通讯协议（标准模式 ASCII）设备协议插件（优先级 3）。
/// 实现 <see cref="IDeviceProtocol"/>，启动期自注册到宿主 <see cref="IDeviceCatalog"/>。
/// 提供真实 FATEK 帧：STX(02) + SLAVE(2ASCII) + CMD(2ASCII) + LEN(2ASCII) + ADDRESS + DATA + SUM(2ASCII) + ETX(03)。
/// 命令：46H 连续多寄存器读、47H 连续多寄存器写、44H 连续多点状态读、45H 连续多点状态写。
/// <para>
/// 寄存器模型映射：
/// HoldingRegister/InputRegister → D 寄存器（16 位），地址字段 "D"+5 位十六进制；
/// Coil → M 继电器、DiscreteInput → X 输入，地址字段 区域字母+3 位十六进制。
/// StartAddress 解释为设备编号（ushort）。校验和为 STX..DATA 字节累加低 8 位转 2 位 ASCII。
/// </para>
/// </summary>
public sealed class FatekDevicePlugin : IPlugin, IDeviceProtocol
{
    public string Id => "fatek";
    public string Name => "永宏 FATEK 设备协议";
    public string Version => "1.0.0";

    public string ProtocolId => "fatek";
    public string DisplayName => "FATEK (ASCII)";
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
                "[Fatek] 设备协议已注册到 DeviceCatalog（协议={ProtocolId}）", ProtocolId);
        }
        else
        {
            context.Logger.LogWarning("[Fatek] 未找到 IDeviceCatalog，跳过注册");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public IDeviceSession CreateSession(DeviceConnectionOptions options) => new FatekSession(options);
}

/// <summary>FATEK ASCII 会话：单条 TCP 连接，按请求构造 ASCII 帧并收发。</summary>
internal sealed class FatekSession : IDeviceSession
{
    private readonly DeviceConnectionOptions _options;
    private TcpClient? _client;
    private NetworkStream? _stream;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public FatekSession(DeviceConnectionOptions options) => _options = options;

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
            string area = isBit
                ? (request.Type == RegisterType.Coil ? "M" : "X")
                : "D";
            string cmd = isBit ? "44" : "46";
            string addrField = isBit
                ? $"{area}{request.StartAddress:X3}"
                : $"D{request.StartAddress:X5}";
            string len = request.Count.ToString("X2");

            var frame = BuildFrame(_options.UnitId, cmd, len, addrField, string.Empty);
            var resp = await TransactAsync(frame, cancellationToken).ConfigureAwait(false);

            if (isBit)
            {
                var data = new byte[request.Count];
                for (int i = 0; i < request.Count; i++)
                {
                    data[i] = resp[i] == '1' ? (byte)1 : (byte)0;
                }

                return data;
            }

            var outBuf = new byte[request.Count * 2];
            for (int i = 0; i < request.Count; i++)
            {
                int v = Convert.ToInt32(resp.Substring(i * 4, 4), 16);
                outBuf[i * 2] = (byte)v;
                outBuf[i * 2 + 1] = (byte)(v >> 8);
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
            string area = isBit
                ? (request.Type == RegisterType.Coil ? "M" : "X")
                : "D";
            string cmd = isBit ? "45" : "47";
            string addrField = isBit
                ? $"{area}{request.StartAddress:X3}"
                : $"D{request.StartAddress:X5}";
            string len = request.Values.Length.ToString("X2");

            var sb = new StringBuilder();
            foreach (var v in request.Values)
            {
                if (isBit)
                {
                    sb.Append(v == 0 ? '0' : '1');
                }
                else
                {
                    sb.Append(((int)v).ToString("X4"));
                }
            }

            var frame = BuildFrame(_options.UnitId, cmd, len, addrField, sb.ToString());
            await TransactAsync(frame, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    private static byte[] BuildFrame(byte unitId, string cmd, string len, string addr, string data)
    {
        var sb = new StringBuilder();
        sb.Append((char)0x02); // STX
        sb.Append(unitId.ToString("X2"));
        sb.Append(cmd);
        sb.Append(len);
        sb.Append(addr);
        sb.Append(data);
        string sum = Checksum(sb.ToString());
        sb.Append(sum);
        sb.Append((char)0x03); // ETX
        return Encoding.ASCII.GetBytes(sb.ToString());
    }

    private static string Checksum(string s)
    {
        int sum = 0;
        foreach (char c in s)
        {
            sum += c;
        }

        return (sum & 0xFF).ToString("X2");
    }

    private async Task<string> TransactAsync(byte[] frame, CancellationToken ct)
    {
        await _stream!.WriteAsync(frame, ct).ConfigureAwait(false);
        await _stream.FlushAsync(ct).ConfigureAwait(false);

        // 响应：STX + SLAVE(2) + CMD(2) + ERROR(1) + [DATA] + SUM(2) + ETX
        var buf = new List<byte>(64);
        var one = new byte[1];
        while (true)
        {
            int r = await _stream.ReadAsync(one, ct).ConfigureAwait(false);
            if (r == 0)
            {
                throw new EndOfStreamException("FATEK 连接在对端关闭");
            }

            buf.Add(one[0]);
            if (one[0] == 0x03)
            {
                break;
            }
        }

        string resp = Encoding.ASCII.GetString(buf.ToArray());
        if (resp[0] != (char)0x02)
        {
            throw new InvalidOperationException("FATEK 响应缺少 STX");
        }

        // ERROR 位（索引 5，STX+2+2 = 5）
        char err = resp[5];
        if (err != '0')
        {
            throw new InvalidOperationException($"FATEK 设备返回错误码: {err}");
        }

        // DATA 起始于索引 6，结束于倒数第 3（SUM 占 2，ETX 占 1）
        int dataEnd = resp.Length - 3;
        return resp.Substring(6, dataEnd - 6);
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
