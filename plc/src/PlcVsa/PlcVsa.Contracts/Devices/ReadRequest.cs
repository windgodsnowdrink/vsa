namespace PlcVsa.Contracts.Devices;

/// <summary>按协议寄存器粒度的原始读请求。</summary>
public sealed record ReadRequest(
    ushort StartAddress,
    ushort Count,
    RegisterType Type);
