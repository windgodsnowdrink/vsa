namespace PlcVsa.Contracts.Devices;

/// <summary>按协议寄存器粒度的原始写请求。</summary>
public sealed record WriteRequest(
    ushort StartAddress,
    RegisterType Type,
    ushort[] Values);
