namespace PlcGateway;

/// <summary>流水线事件：一次采集帧（原始字节 + 元数据），在 Disruptor RingBuffer 中流转。</summary>
public sealed class RawFrame
{
    public string DeviceId { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public byte[] Payload { get; set; } = Array.Empty<byte>();
    public string? Note { get; set; }

    public void CopyFrom(RawFrame other)
    {
        DeviceId = other.DeviceId;
        Timestamp = other.Timestamp;
        Payload = other.Payload;
        Note = other.Note;
    }
}
