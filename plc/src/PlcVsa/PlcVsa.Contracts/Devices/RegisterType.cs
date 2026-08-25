namespace PlcVsa.Contracts.Devices;

/// <summary>寄存器 / 线圈类型（对应 Modbus 功能码语义）。</summary>
public enum RegisterType : byte
{
    /// <summary>线圈（读 0x01 / 写 0x05/0x0F）。</summary>
    Coil = 1,
    /// <summary>离散输入（读 0x02）。</summary>
    DiscreteInput = 2,
    /// <summary>保持寄存器（读 0x03 / 写 0x06/0x10）。</summary>
    HoldingRegister = 3,
    /// <summary>输入寄存器（读 0x04）。</summary>
    InputRegister = 4,
}
