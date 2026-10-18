using System.Diagnostics;

namespace Plc.Plugins.Contracts;

/// <summary>帧方向：入站（设备 → 宿主）/ 出站（宿主 → 设备）。</summary>
public enum FrameDirection
{
    /// <summary>入站：读响应、设备主动上报等由设备流向宿主的帧。</summary>
    Inbound = 0,

    /// <summary>出站：写命令、轮询请求等由宿主流向设备的帧。</summary>
    Outbound = 1,
}

/// <summary>
/// 统一设备帧（ADR-119 决策 ③）：一次协议交互的原始字节 + 显式元数据。
/// <para>
/// 位于 <c>Plc.Plugins.Contracts</c>（ALC 共享契约），宿主与插件看到的是同一个类型，
/// 因此可作为 Disruptor 流水线事件的载荷在两个 ALC 之间无损传递。
/// </para>
/// <para>
/// 不可变（<c>init</c> 只读）：并发流水线中多个 <c>IEventHandler</c> 共享同一帧实例时无需加锁。
/// 不依赖任何序列化框架——序列化是上层（MQTT / InfluxDB / Outbox）的事，契约层只负责载荷与元数据。
/// </para>
/// </summary>
/// <remarks>
/// 值相等语义：<see cref="Payload"/> 按<strong>字节内容</strong>比较（非引用比较），
/// 因此两个由不同缓冲区构造但内容相同的帧视为相等。
/// </remarks>
public sealed record DeviceFrame
{
    /// <summary>协议标识，如 "modbus.tcp"。</summary>
    public required string ProtocolId { get; init; }

    /// <summary>设备标识（宿主侧逻辑 ID；未知时为 <see cref="string.Empty"/>）。</summary>
    public required string DeviceId { get; init; }

    /// <summary>帧时间戳（UTC）。</summary>
    public required DateTimeOffset Timestamp { get; init; }

    /// <summary>帧方向。</summary>
    public required FrameDirection Direction { get; init; }

    /// <summary>原始载荷（协议相关；如 Modbus 为寄存器值大端排列）。</summary>
    public required ReadOnlyMemory<byte> Payload { get; init; }

    /// <summary>单调递增序号（同批次内的下标，或全局流水线序号）。</summary>
    public long Sequence { get; init; }

    /// <summary>寄存器 / 线圈类型。</summary>
    public RegisterType RegisterType { get; init; }

    /// <summary>元素数据类型。</summary>
    public ElementType ElementType { get; init; }

    /// <summary>起始地址。</summary>
    public ushort StartAddress { get; init; }

    /// <summary>元素个数（写帧为写入值个数）。</summary>
    public ushort Count { get; init; }

    /// <summary>品牌原生地址文本（如 "D100"、"M0"），无则 null。</summary>
    public string? AddressText { get; init; }

    /// <summary>本次交互耗时（用于 SLA / 背压观测）。</summary>
    public TimeSpan Elapsed { get; init; }

    /// <summary>错误信息；null 表示成功。</summary>
    public string? Error { get; init; }

    /// <summary>是否成功帧。</summary>
    public bool IsSuccess => Error is null;

    /// <summary>载荷字节数。</summary>
    public int Length => Payload.Length;

    /// <summary>构造一个成功的入站（读）帧。</summary>
    public static DeviceFrame Read(
        string protocolId,
        string deviceId,
        ReadRequest request,
        ReadOnlyMemory<byte> payload,
        long sequence = 0,
        DateTimeOffset? timestamp = null,
        TimeSpan elapsed = default)
        => new()
        {
            ProtocolId = protocolId,
            DeviceId = deviceId,
            Timestamp = timestamp ?? DateTimeOffset.UtcNow,
            Direction = FrameDirection.Inbound,
            Payload = payload,
            Sequence = sequence,
            RegisterType = request.Type,
            ElementType = request.ElementType,
            StartAddress = request.StartAddress,
            Count = request.Count,
            AddressText = request.AddressText,
            Elapsed = elapsed,
        };

    /// <summary>构造一个成功的出站（写）帧（写操作无返回载荷，故 Payload 为空）。</summary>
    public static DeviceFrame Write(
        string protocolId,
        string deviceId,
        WriteRequest request,
        long sequence = 0,
        DateTimeOffset? timestamp = null,
        TimeSpan elapsed = default)
        => new()
        {
            ProtocolId = protocolId,
            DeviceId = deviceId,
            Timestamp = timestamp ?? DateTimeOffset.UtcNow,
            Direction = FrameDirection.Outbound,
            Payload = ReadOnlyMemory<byte>.Empty,
            Sequence = sequence,
            RegisterType = request.Type,
            ElementType = request.ElementType,
            StartAddress = request.StartAddress,
            Count = (ushort)request.Values.Length,
            AddressText = request.AddressText,
            Elapsed = elapsed,
        };

    /// <summary>构造一个失败帧（批量读的单项失败落为此形态，不抛异常）。</summary>
    public static DeviceFrame Failure(
        string protocolId,
        string deviceId,
        string error,
        RegisterType registerType = RegisterType.HoldingRegister,
        ushort startAddress = 0,
        long sequence = 0,
        FrameDirection direction = FrameDirection.Inbound,
        DateTimeOffset? timestamp = null,
        TimeSpan elapsed = default)
        => new()
        {
            ProtocolId = protocolId,
            DeviceId = deviceId,
            Timestamp = timestamp ?? DateTimeOffset.UtcNow,
            Direction = direction,
            Payload = ReadOnlyMemory<byte>.Empty,
            Sequence = sequence,
            RegisterType = registerType,
            StartAddress = startAddress,
            Error = error,
            Elapsed = elapsed,
        };

    /// <summary>载荷拷贝为字节数组（跨 ALC / 序列化边界时使用）。</summary>
    public byte[] ToByteArray() => Payload.ToArray();

    /// <summary>按字节内容比较的相等语义。</summary>
    public bool Equals(DeviceFrame? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return ProtocolId == other.ProtocolId
            && DeviceId == other.DeviceId
            && Timestamp.Equals(other.Timestamp)
            && Direction == other.Direction
            && Sequence == other.Sequence
            && RegisterType == other.RegisterType
            && ElementType == other.ElementType
            && StartAddress == other.StartAddress
            && Count == other.Count
            && AddressText == other.AddressText
            && Elapsed.Equals(other.Elapsed)
            && Error == other.Error
            && Payload.Span.SequenceEqual(other.Payload.Span);
    }

    /// <summary>与 <see cref="Equals(DeviceFrame?)"/> 一致的哈希。</summary>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(ProtocolId);
        hash.Add(DeviceId);
        hash.Add(Timestamp);
        hash.Add(Direction);
        hash.Add(Sequence);
        hash.Add(RegisterType);
        hash.Add(ElementType);
        hash.Add(StartAddress);
        hash.Add(Count);
        hash.Add(AddressText);
        hash.Add(Elapsed);
        hash.Add(Error);
        hash.AddBytes(Payload.Span);
        return hash.ToHashCode();
    }

    /// <summary>单行摘要（日志 / 门禁断言可读）。</summary>
    public override string ToString() =>
        Error is null
            ? $"{Direction} {ProtocolId}/{DeviceId} seq={Sequence} {RegisterType}@{StartAddress}x{Count} {Length}B {Elapsed.TotalMilliseconds:F1}ms"
            : $"{Direction} {ProtocolId}/{DeviceId} seq={Sequence} {RegisterType}@{StartAddress} ERR:{Error}";
}
