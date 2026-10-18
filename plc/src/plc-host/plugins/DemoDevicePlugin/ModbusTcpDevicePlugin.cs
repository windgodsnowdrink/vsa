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

/// <summary>
/// Modbus TCP 会话：一次 TCP 连接，按请求构造 MBAP+PDU 并收发。
/// <para>
/// 连接复用：同一会话内的多个请求复用同一条 TCP 连接（见 <see cref="SupportsConnectionReuse"/>）。
/// 代价是——一旦某次交互超时或帧错位，迟到的响应会被<strong>下一次</strong>请求读走，造成静默脏数据。
/// 因此本类对所有「连接状态不可信」的异常一律作废底层连接（<see cref="InvalidateAsync"/>），下次调用自动重连。
/// </para>
/// </summary>
internal sealed class ModbusTcpSession : IDeviceSession
{
    private readonly DeviceConnectionOptions _options;
    private TcpClient? _client;
    private NetworkStream? _stream;
    private ushort _transactionId;

    public ModbusTcpSession(DeviceConnectionOptions options) => _options = options;

    /// <summary>帧元数据用，与 <see cref="ModbusTcpDevicePlugin.ProtocolId"/> 一致。</summary>
    public string ProtocolId => "modbus.tcp";

    /// <summary>
    /// 支持连接复用：会话内多请求共用一条 TCP 连接（实测单会话 200 次读仅新建 1 条连接）。
    /// 置 true 的前提是「出错必作废」——见 <see cref="InvalidateAsync"/> 与类注释。
    /// </summary>
    public bool SupportsConnectionReuse => true;

    private async Task EnsureConnectedAsync(CancellationToken ct)
    {
        if (_client is { Connected: true } && _stream is not null)
        {
            return;
        }

        _client = new TcpClient();
        await _client.ConnectAsync(_options.Host, _options.Port, ct).ConfigureAwait(false);
        // 注意：不要设 TcpClient.SendTimeout / ReceiveTimeout —— 它们只作用于**同步** Send/Read，
        // 对 await stream.ReadAsync(...) 完全无效，会给人「已经设了超时」的错觉（正是修前的 P0 缺陷）。
        // 超时改由 CreateTimeoutSource 的 CancelAfter 统一兜底。
        _stream = _client.GetStream();
    }

    /// <summary>
    /// 生成「单次操作」的超时令牌：覆盖建连 + 收发的全过程，而不是只覆盖某一跳。
    /// 与调用方令牌链接——调用方取消优先，自身超时次之。
    /// </summary>
    private CancellationTokenSource CreateTimeoutSource(CancellationToken callerToken)
    {
        var linked = CancellationTokenSource.CreateLinkedTokenSource(callerToken);
        if (_options.TimeoutMs > 0)
        {
            linked.CancelAfter(_options.TimeoutMs);
        }

        return linked;
    }

    /// <summary>作废底层连接：超时 / 取消 / 帧错位之后，流上可能残留未读取的响应，必须丢弃重建。</summary>
    private async ValueTask InvalidateAsync()
    {
        if (_stream is not null)
        {
            await _stream.DisposeAsync().ConfigureAwait(false);
        }

        _client?.Dispose();
        _stream = null;
        _client = null;
    }

    public async Task<byte[]> ReadAsync(ReadRequest request, CancellationToken cancellationToken = default)
    {
        using var timeout = CreateTimeoutSource(cancellationToken);
        var token = timeout.Token;
        try
        {
            await EnsureConnectedAsync(token).ConfigureAwait(false);

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

            var body = await TransactAsync(pdu, token).ConfigureAwait(false);

            // body[0]=FC; body[1]=byteCount; 之后为数据
            if (body.Length < 2 || body[1] + 2 > body.Length)
            {
                throw new ModbusFrameMismatchException(
                    $"Modbus 响应长度异常：声明 {body[1]} 字节，实际可用 {Math.Max(0, body.Length - 2)} 字节");
            }

            int byteCount = body[1];
            return body[2..(2 + byteCount)];
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // 自身超时（而非调用方取消）：转成语义明确的 TimeoutException，并作废连接
            await InvalidateAsync().ConfigureAwait(false);
            throw new TimeoutException(
                $"Modbus TCP 读取超时：{_options.TimeoutMs}ms 内未收到完整响应（{_options.Host}:{_options.Port}）");
        }
        catch (Exception ex) when (ex is OperationCanceledException
                                    or IOException
                                    or SocketException
                                    or EndOfStreamException
                                    or ModbusFrameMismatchException)
        {
            // 连接状态已不可信（取消 / 断链 / 帧错位）：作废，下次自动重连
            await InvalidateAsync().ConfigureAwait(false);
            throw;
        }
        // 设备返回的 Modbus 异常响应（InvalidOperationException）不在此列：连接仍然可用，不作废。
    }

    public async Task WriteAsync(WriteRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Values.Length == 0)
        {
            throw new ArgumentException("WriteRequest.Values 为空，无可写入的值", nameof(request));
        }

        using var timeout = CreateTimeoutSource(cancellationToken);
        var token = timeout.Token;
        try
        {
            await EnsureConnectedAsync(token).ConfigureAwait(false);

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

            await TransactAsync(pdu, token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            await InvalidateAsync().ConfigureAwait(false);
            throw new TimeoutException(
                $"Modbus TCP 写入超时：{_options.TimeoutMs}ms 内未收到完整响应（{_options.Host}:{_options.Port}）");
        }
        catch (Exception ex) when (ex is OperationCanceledException
                                    or IOException
                                    or SocketException
                                    or EndOfStreamException
                                    or ModbusFrameMismatchException)
        {
            await InvalidateAsync().ConfigureAwait(false);
            throw;
        }
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

        // 响应一致性校验（ADR-119 M5 风险 6）：事务号 / 单元号必须回显请求值。
        // 不校验的话，一旦收发错位（如超时后迟到响应、网关串号），会静默把别人的响应当成自己的返回。
        var responseTid = (ushort)((header[0] << 8) | header[1]);
        if (responseTid != tid)
        {
            throw new ModbusFrameMismatchException(
                $"Modbus 响应事务号不匹配：期望 0x{tid:X4}，实际 0x{responseTid:X4}");
        }

        if (header[6] != _options.UnitId)
        {
            throw new ModbusFrameMismatchException(
                $"Modbus 响应单元号不匹配：期望 {_options.UnitId}，实际 {header[6]}");
        }

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
        if (count <= 0)
        {
            throw new ModbusFrameMismatchException($"Modbus 响应帧长度非法：{count}");
        }

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

    public async ValueTask DisposeAsync() => await InvalidateAsync().ConfigureAwait(false);
}

/// <summary>
/// 帧级不一致（事务号 / 单元号不匹配、长度非法）：说明收发已错位，连接不可再复用，必须作废重建。
/// 继承自 <see cref="InvalidOperationException"/>，兼容既有的调用方异常捕获。
/// </summary>
internal sealed class ModbusFrameMismatchException : InvalidOperationException
{
    public ModbusFrameMismatchException(string message)
        : base(message)
    {
    }
}
