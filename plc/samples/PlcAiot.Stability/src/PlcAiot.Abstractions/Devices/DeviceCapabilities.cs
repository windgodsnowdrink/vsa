using System.Collections.Immutable;

namespace PlcAiot.Abstractions.Devices;

/// <summary>协议能力协商结果；连接后上报，上层据此自适应行为（§1.12.3）。</summary>
public sealed record DeviceCapabilities
{
    public bool SupportsWrite { get; init; }
    public bool SupportsSubscribe { get; init; }        // true→用订阅，false→轮询
    public int MaxRegistersPerFrame { get; init; } = 100;
    public IReadOnlySet<string> NativeDataTypes { get; init; } = ImmutableHashSet<string>.Empty;
    public int MaxConcurrentSessions { get; init; } = 1;
}
