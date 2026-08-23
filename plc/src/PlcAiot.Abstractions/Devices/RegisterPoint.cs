namespace PlcAiot.Abstractions.Devices;

/// <summary>单个寄存器/点位定义（点表的一行）。与协议无关，上层只认 Tag。</summary>
public sealed record RegisterPoint
{
    public string Tag { get; init; } = "";
    public ushort Address { get; init; }
    public byte FunctionCode { get; init; }
    public string DataType { get; init; } = "float";   // bool/int16/uint16/float/...
    public ByteOrder ByteOrder { get; init; } = ByteOrder.ABCD;
    public double Scale { get; init; } = 1.0;
    public double Offset { get; init; } = 0.0;
    public string? Unit { get; init; }
}
