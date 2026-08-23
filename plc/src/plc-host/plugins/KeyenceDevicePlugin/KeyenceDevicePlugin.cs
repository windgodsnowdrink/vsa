using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Plc.Plugins.Contracts;

namespace KeyenceDevicePlugin;

/// <summary>
/// 基恩士(Keyence) KV 系列 Host Link 协议设备协议插件（优先级 3）。
/// 实现 <see cref="IDeviceProtocol"/>，启动期自注册到宿主 <see cref="IDeviceCatalog"/>。
/// 提供真实 KV Host Link 帧：以 '@' 起始、单元号 + 命令 + 数据、以 CR(0x0D) 结束的 ASCII 文本协议。
/// 命令：RD(读)/WR(写) 数据存储器 DM；RD 读继电器/输入；ST/RS 强制置位/复位。
/// <para>寄存器模型映射：</para>
/// HoldingRegister/InputRegister → DM 数据存储器（十进制 5 位）；
/// Coil → 内部继电器（RD 读、ST/RS 强制置位/复位）；
/// DiscreteInput → 输入 X（RD 只读）。
/// StartAddress 解释为设备编号（ushort）。响应以 '@' 单元号 开头，含命令回显与数据/OK。
/// </summary>
public sealed class KeyenceDevicePlugin : IPlugin, IDeviceProtocol
{
    public string Id => "keyence.kv";
    public string Name => "基恩士 KV Host Link 设备协议";
    public string Version => "1.0.0";

    public string ProtocolId => "keyence.kv";
    public string DisplayName => "Keyence KV (Host Link)";
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
                "[Keyence] 设备协议已注册到 DeviceCatalog（协议={ProtocolId}）", ProtocolId);
        }
        else
        {
            context.Logger.LogWarning("[Keyence] 未找到 IDeviceCatalog，跳过注册");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public IDeviceSession CreateSession(DeviceConnectionOptions options) => new KeyenceKvSession(options);
}

/// <summary>KV Host Link 会话：单条 TCP 连接，发送 ASCII 命令 + CR，读取至 CR。</summary>
internal sealed class KeyenceKvSession : IDeviceSession
{
    private readonly DeviceConnectionOptions _options;
    private TcpClient? _client;
    private NetworkStream? _stream;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public KeyenceKvSession(DeviceConnectionOptions options) => _options = options;

    private string Unit => _options.UnitId.ToString("D2");

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

            var outBuf = new byte[isBit ? request.Count : request.Count * 2];
            int offset = 0;
            for (int i = 0; i < request.Count; i++)
            {
                // 多点的设备偏移：位设备按编号递增；DM 按编号递增
                string devI = isBit
                    ? (request.Type == RegisterType.Coil ? $"{request.StartAddress + i}" : $"X{request.StartAddress + i}")
                    : $"DM{(request.StartAddress + i):D5}";
                string cmd = $"@{Unit}RD {devI}\r";
                string resp = await TransactAsync(cmd, cancellationToken).ConfigureAwait(false);
                int v = ParseReadData(resp);
                if (isBit)
                {
                    outBuf[offset++] = (byte)(v == 0 ? 0 : 1);
                }
                else
                {
                    outBuf[offset++] = (byte)v;
                    outBuf[offset++] = (byte)(v >> 8);
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
                if (isBit)
                {
                    string devI = $"{request.StartAddress + i}";
                    string verb = request.Values[i] == 0 ? "RS" : "ST"; // 复位 / 置位
                    string cmd = $"@{Unit}{verb} {devI}\r";
                    await TransactAsync(cmd, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    string devI = $"DM{(request.StartAddress + i):D5}";
                    string cmd = $"@{Unit}WR {devI} {request.Values[i]:D5}\r";
                    await TransactAsync(cmd, cancellationToken).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    private static int ParseReadData(string resp)
    {
        if (resp.StartsWith("@") == false)
        {
            throw new InvalidOperationException($"KV 响应缺少 '@' 头: {resp}");
        }

        // @ + 2 位单元号 + 2 位命令(RD) + 空格 + 数据
        int sp = resp.IndexOf(' ', 3);
        if (sp < 0)
        {
            throw new InvalidOperationException($"KV 响应缺少数据段: {resp}");
        }

        string data = resp[(sp + 1)..].TrimEnd('\r', '\n').Trim();
        if (int.TryParse(data, out int v))
        {
            return v;
        }

        throw new InvalidOperationException($"KV 响应数据无法解析为整数: '{data}'");
    }

    private async Task<string> TransactAsync(string command, CancellationToken ct)
    {
        var send = Encoding.ASCII.GetBytes(command);
        await _stream!.WriteAsync(send, ct).ConfigureAwait(false);
        await _stream.FlushAsync(ct).ConfigureAwait(false);

        var buf = new StringBuilder();
        var one = new byte[1];
        while (true)
        {
            int r = await _stream.ReadAsync(one, ct).ConfigureAwait(false);
            if (r == 0)
            {
                throw new EndOfStreamException("KV 连接在对端关闭");
            }

            char c = (char)one[0];
            buf.Append(c);
            if (c == '\r')
            {
                break;
            }
        }

        string resp = buf.ToString();
        if (resp.Contains("ER") || (resp.Length > 5 && resp[5] == 'E' && resp[6] != 'R'))
        {
            throw new InvalidOperationException($"KV 设备返回错误: {resp.Trim()}");
        }

        return resp;
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
