namespace PlcAiot.Abstractions.Devices;

/// <summary>多字节寄存器排列顺序（Modbus/S7 等常需处理字节序）。</summary>
public enum ByteOrder { ABCD, DCBA, BADC, CDAB }
