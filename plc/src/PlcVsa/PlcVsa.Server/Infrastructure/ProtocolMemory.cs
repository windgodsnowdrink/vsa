// ────────────────────────────────────────────────────────────────────────────
// §7 协议消息记忆持久化（ProtocolMemoryStore：append-only mem-*.dat）
// §6 Bus 事件（ProtocolFrame + SHA-256 校验）
// ────────────────────────────────────────────────────────────────────────────
using System.Security.Cryptography;
using System.Text;
using PlcVsa.Contracts.Devices;
using Seq64 = System.Int64;
using U16 = System.UInt16;

namespace PlcVsa.Server.Infrastructure;

/// <summary>§1 协议帧：SHA-256 校验（防篡改审计），可被 Disruptor 消费者写入页式文件。</summary>
public sealed class ProtocolFrame
{
    public Seq64 Seq { get; set; }
    public DateTimeOffset Ts { get; set; } = DateTimeOffset.UtcNow;
    public string Protocol { get; set; } = "";
    public string DeviceId { get; set; } = "";
    public U16 StartAddr { get; set; }
    public U16 Count { get; set; }
    public RegisterType Rt { get; set; }
    public byte[] Payload { get; set; } = Array.Empty<byte>();
    public bool IsWrite { get; set; }
    public string Checksum { get; set; } = "";

    public void ComputeChecksum()
    {
        using var sha = SHA256.Create();
        using var ms = new MemoryStream(512);
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
        {
            bw.Write(Seq); bw.Write(Ts.UtcTicks);
            bw.Write(Protocol); bw.Write(DeviceId);
            bw.Write(StartAddr); bw.Write(Count); bw.Write((U16)Rt); bw.Write(IsWrite);
            bw.Write(Payload.Length); bw.Write(Payload);
        }
        Checksum = Convert.ToHexString(sha.ComputeHash(ms.GetBuffer().AsSpan(0, (int)ms.Length).ToArray()));
    }
}

/// <summary>§1.1 协议消息持久化：append-only + 页式滚动 + 顺序扫描查询。</summary>
public sealed class ProtocolMemoryStore
{
    private readonly string _dir;
    private readonly object _gate = new();
    private FileStream? _curStream;
    private int _curFileIdx;

    public ProtocolMemoryStore(string dir)
    {
        _dir = dir;
        Directory.CreateDirectory(dir);
        for (int i = 0; i < 1_000_000; i++)
        {
            if (!File.Exists(Path.Combine(dir, $"mem-{i:000000}.dat"))) { _curFileIdx = i; break; }
        }
        OpenCurrentFile();
    }
    private void OpenCurrentFile() => _curStream = new FileStream(
        Path.Combine(_dir, $"mem-{_curFileIdx:000000}.dat"),
        FileMode.Append, FileAccess.Write, FileShare.Read,
        bufferSize: PlcVsaConfig.PAGE_SIZE,
        options: FileOptions.SequentialScan | FileOptions.Asynchronous);

    public void Append(ProtocolFrame f)
    {
        lock (_gate)
        {
            using var ms = new MemoryStream(512);
            using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
            {
                bw.Write(f.Seq); bw.Write(f.Ts.UtcTicks);
                bw.Write(f.Protocol); bw.Write(f.DeviceId);
                bw.Write(f.StartAddr); bw.Write(f.Count); bw.Write((U16)f.Rt); bw.Write(f.IsWrite);
                bw.Write(f.Payload.Length); bw.Write(f.Payload);
                bw.Write(f.Checksum);
            }
            var body = ms.ToArray();
            var header = BitConverter.GetBytes(body.Length);
            _curStream!.Write(header);
            _curStream.Write(body);
            _curStream.Flush();
            if (_curStream.Length >= 32 * 1024 * 1024)
            {
                _curStream.Dispose();
                _curFileIdx++;
                OpenCurrentFile();
            }
        }
    }

    public List<ProtocolFrame> Query(string protocol, string deviceId, long sinceSeq, int limit)
    {
        var result = new List<ProtocolFrame>(limit);
        var files = Directory.EnumerateFiles(_dir, "mem-*.dat").OrderBy(s => s, StringComparer.Ordinal);
        foreach (var file in files)
        {
            using var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var br = new BinaryReader(fs, Encoding.UTF8);
            while (fs.Position < fs.Length)
            {
                if (fs.Length - fs.Position < 4) break;
                int len = br.ReadInt32();
                if (len <= 0 || len > 10_000_000) break;
                var body = br.ReadBytes(len);
                using var ms = new MemoryStream(body);
                using var r = new BinaryReader(ms, Encoding.UTF8);
                var f = new ProtocolFrame
                {
                    Seq       = r.ReadInt64(),
                    Ts        = new DateTimeOffset(r.ReadInt64(), TimeSpan.Zero),
                    Protocol  = r.ReadString(),
                    DeviceId  = r.ReadString(),
                    StartAddr = r.ReadUInt16(),
                    Count     = r.ReadUInt16(),
                    Rt        = (RegisterType)r.ReadUInt16(),
                    IsWrite   = r.ReadBoolean(),
                };
                int pLen = r.ReadInt32();
                f.Payload  = r.ReadBytes(pLen);
                f.Checksum = r.ReadString();

                if (f.Seq < sinceSeq) continue;
                if (!f.Protocol.Equals(protocol, StringComparison.OrdinalIgnoreCase)) continue;
                if (!f.DeviceId.Equals(deviceId, StringComparison.OrdinalIgnoreCase)) continue;
                result.Add(f);
                if (result.Count >= limit) return result;
            }
        }
        return result;
    }
}
