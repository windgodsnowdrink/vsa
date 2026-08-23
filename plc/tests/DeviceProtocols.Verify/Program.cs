using System.Net;
using System.Net.Sockets;
using System.Text;
using Plc.Plugins.Contracts;

namespace DeviceProtocols.Verify;

/// <summary>
/// 设备协议插件帧往返验证工具（控制台）。
/// 为每种协议启动一个进程内「设备仿真端」(TcpListener)，用插件会话发起真实读 / 写，
/// 校验编解码对称性与数据一致性。验证的是「帧编解码自洽 + 端点往返」，
/// 真实设备互操作需以物理 PLC / 标准协议栈（如 BACnet VTS）校验。
/// </summary>
public static class Program
{
    private static int _fail;

    public static async Task<int> Main()
    {
        Console.WriteLine("=== 设备协议插件帧往返验证 ===");

        await TestMelsec();
        await TestBacnet();
        await TestFatek();
        await TestKeyence();
        await TestFuji();
        await TestLoRa();

        Console.WriteLine();
        if (_fail == 0)
        {
            Console.WriteLine("全部通过 ✅");
            return 0;
        }

        Console.WriteLine($"失败 {_fail} 项 ❌");
        return 1;
    }

    // ---------------- 通用服务器 ----------------

    private static async Task RunServer(Func<NetworkStream, CancellationToken, Task> handler)
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.Server.LocalEndPoint!).Port;
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var ct = cts.Token;
        _ports.Push(port);

        _ = Task.Run(async () =>
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    var client = await listener.AcceptTcpClientAsync(ct).ConfigureAwait(false);
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await handler(client.GetStream(), ct).ConfigureAwait(false);
                        }
                        catch (Exception ex) when (ct.IsCancellationRequested || ex is IOException) { }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"    [SIM 异常] {ex.GetType().Name}: {ex.Message}");
                        }
                        finally
                        {
                            client.Dispose();
                        }
                    }, ct);
                }
            }
            catch { }
            finally
            {
                listener.Stop();
            }
        }, ct);

        _listeners.Add(listener);
    }

    private static readonly Stack<int> _ports = new();
    private static readonly List<TcpListener> _listeners = new();

    private static int NextPort() => _ports.Pop();

    private static DeviceConnectionOptions Opt(int port) =>
        new("127.0.0.1", port, 1, 5000);

    private static void Check(string name, bool ok, string detail = "")
    {
        if (ok)
        {
            Console.WriteLine($"  ✅ {name}");
        }
        else
        {
            _fail++;
            Console.WriteLine($"  ❌ {name} {detail}");
        }
    }

    // ---------------- Melsec MC Qna-3E ----------------

    private static async Task TestMelsec()
    {
        Console.WriteLine("[Melsec MC Qna-3E]");
        var store = new Dictionary<string, byte[]>();
        await RunServer(async (stream, ct) =>
        {
            while (!ct.IsCancellationRequested)
            {
                var header = await ReadExactAsync(stream, 9, ct).ConfigureAwait(false);
                if (header[0] != 0x50) break;
                int len = header[7] | (header[8] << 8);
                var body = await ReadExactAsync(stream, len, ct).ConfigureAwait(false);
                // body = 监视定时器(2) + 请求数据；请求数据: command(2)+sub(2)+devCode(1)+addr(3)+count(2)+[data]
                byte cmd0 = body[2], cmd1 = body[3];
                byte dev = body[6];
                int addr = body[7] | (body[8] << 8) | (body[9] << 16);
                int count = body[10] | (body[11] << 8);
                bool isWrite = (cmd0 == 0x14);
                bool bit = (cmd1 == 0x01); // 位读/写命令次字节为 0x01，字为 0x04
                string key = $"{(char)dev}:{addr}";
                byte[] respData;
                if (isWrite)
                {
                    int dataLen = bit ? count : count * 2;
                    var data = new byte[dataLen];
                    Buffer.BlockCopy(body, 12, data, 0, dataLen);
                    store[key] = data;
                    respData = Array.Empty<byte>();
                }
                else
                {
                    if (!store.TryGetValue(key, out var existing))
                    {
                        existing = new byte[bit ? count : count * 2];
                    }

                    respData = existing;
                }

                // 响应：D0 00 + net/pc/io/station + len + endcode(0000) + data
                var resp = new byte[9 + 2 + respData.Length];
                resp[0] = 0xD0; resp[1] = 0x00;
                resp[2] = header[2]; resp[3] = header[3];
                resp[4] = header[4]; resp[5] = header[5]; resp[6] = header[6];
                int rl = 2 + respData.Length;
                resp[7] = (byte)(rl & 0xFF); resp[8] = (byte)((rl >> 8) & 0xFF);
                resp[9] = 0x00; resp[10] = 0x00;
                Buffer.BlockCopy(respData, 0, resp, 11, respData.Length);
                await stream.WriteAsync(resp, ct).ConfigureAwait(false);
                await stream.FlushAsync(ct).ConfigureAwait(false);
            }
        });

        int port = NextPort();
        var plugin = new MelsecDevicePlugin.MelsecDevicePlugin();
        await using var sWrite = plugin.CreateSession(Opt(port));
        await sWrite.WriteAsync(new WriteRequest(100, RegisterType.HoldingRegister, new[] { (ushort)0x1234, (ushort)0x5678 }), CancellationToken.None);
        await using var sRead = plugin.CreateSession(Opt(port));
        var r = await sRead.ReadAsync(new ReadRequest(100, 2, RegisterType.HoldingRegister), CancellationToken.None);
        Check("字设备 写→读 往返", r.Length == 4 && r[0] == 0x34 && r[1] == 0x12 && r[2] == 0x78 && r[3] == 0x56,
            $"得到 [{string.Join(" ", r.Select(b => b.ToString("X2")))}]");

        await using var sBitW = plugin.CreateSession(Opt(port));
        await sBitW.WriteAsync(new WriteRequest(10, RegisterType.Coil, new ushort[] { 1, 0, 1 }), CancellationToken.None);
        await using var sBitR = plugin.CreateSession(Opt(port));
        var rb = await sBitR.ReadAsync(new ReadRequest(10, 3, RegisterType.Coil), CancellationToken.None);
        Check("位设备 写→读 往返", rb.Length == 3 && rb[0] == 1 && rb[1] == 0 && rb[2] == 1,
            $"得到 [{string.Join(" ", rb)}]");
    }

    // ---------------- BACnet/IP ----------------

    private static async Task TestBacnet()
    {
        Console.WriteLine("[BACnet/IP]");
        var analog = new Dictionary<uint, float>();
        var binary = new Dictionary<uint, bool>();
        await RunServer(async (stream, ct) =>
        {
            while (!ct.IsCancellationRequested)
            {
                var head = await ReadExactAsync(stream, 6, ct).ConfigureAwait(false);
                int total = (head[2] << 8) | head[3];
                var apdu = await ReadExactAsync(stream, total - 6, ct).ConfigureAwait(false);
                if (apdu[0] != 0x00) break;
                byte svc = apdu[2];
                // ctx0 objId(4) at apdu[4..7]; decode type/instance
                uint v = (uint)(apdu[4] << 24 | apdu[5] << 16 | apdu[6] << 8 | apdu[7]);
                byte objType = (byte)(v >> 22);
                uint instance = v & 0x3FFFFF;
                if (svc == 0x0C) // ReadProperty
                {
                    byte[] valueBytes;
                    if (objType == 2)
                    {
                        analog.TryGetValue(instance, out float f);
                        valueBytes = BitConverter.GetBytes(f);
                        if (BitConverter.IsLittleEndian) Array.Reverse(valueBytes);
                    }
                    else
                    {
                        binary.TryGetValue(instance, out bool b);
                        valueBytes = new[] { (byte)(b ? 0x01 : 0x00) };
                    }

                    var body = new List<byte> { 0x30, apdu[1], 0x0C, 0x84,
                        apdu[4], apdu[5], apdu[6], apdu[7], 0x91, 0x55, 0x9E };
                    if (objType == 2)
                    {
                        body.Add(0x44); body.AddRange(valueBytes);
                    }
                    else
                    {
                        body.Add(0x11); body.Add(valueBytes[0]);
                    }

                    body.Add(0x9F);
                    await WriteBvlc(stream, body.ToArray(), ct).ConfigureAwait(false);
                }
                else if (svc == 0x0F) // WriteProperty
                {
                    // apdu: 0x30 invoke 0x0F 0x84 objId(4) 0x91 0x55 0x9E appTag value 0x9F
                    byte appTag = apdu[11];
                    if (objType == 2)
                    {
                        var fb = new byte[] { apdu[12], apdu[13], apdu[14], apdu[15] };
                        if (BitConverter.IsLittleEndian) Array.Reverse(fb);
                        analog[instance] = BitConverter.ToSingle(fb, 0);
                    }
                    else
                    {
                        binary[instance] = apdu[12] != 0x00;
                    }

                    var body = new List<byte> { 0x30, apdu[1], 0x0F, 0x84,
                        apdu[4], apdu[5], apdu[6], apdu[7], 0x91, 0x55, 0x9E, appTag };
                    if (objType == 2) body.AddRange(new byte[] { apdu[12], apdu[13], apdu[14], apdu[15] });
                    else body.Add(apdu[12]);
                    body.Add(0x9F);
                    await WriteBvlc(stream, body.ToArray(), ct).ConfigureAwait(false);
                }
            }
        });

        int port = NextPort();
        var plugin = new BacnetDevicePlugin.BacnetDevicePlugin();
        await using var w = plugin.CreateSession(Opt(port));
        await w.WriteAsync(new WriteRequest(5, RegisterType.HoldingRegister, new[] { (ushort)0x1234 }), CancellationToken.None);
        await using var r = plugin.CreateSession(Opt(port));
        var data = await r.ReadAsync(new ReadRequest(5, 1, RegisterType.HoldingRegister), CancellationToken.None);
        // 回读：float(0x1234)=4660 的四字节大端
        var expected = BitConverter.GetBytes((float)0x1234);
        if (BitConverter.IsLittleEndian) Array.Reverse(expected);
        Check("AnalogValue 写→读 往返", data.Length == 4 && data.SequenceEqual(expected),
            $"得到 [{string.Join(" ", data.Select(b => b.ToString("X2")))}]");

        await using var bw = plugin.CreateSession(Opt(port));
        await bw.WriteAsync(new WriteRequest(7, RegisterType.Coil, new ushort[] { 1 }), CancellationToken.None);
        await using var br = plugin.CreateSession(Opt(port));
        var bd = await br.ReadAsync(new ReadRequest(7, 1, RegisterType.Coil), CancellationToken.None);
        Check("BinaryValue 写→读 往返", bd.Length == 1 && bd[0] == 1, $"得到 {bd[0]}");
    }

    private static async Task WriteBvlc(NetworkStream stream, byte[] apdu, CancellationToken ct)
    {
        int total = apdu.Length + 6;
        var frame = new byte[total];
        frame[0] = 0x81; frame[1] = 0x0A;
        frame[2] = (byte)(total >> 8); frame[3] = (byte)total;
        frame[4] = 0x01; frame[5] = 0x00;
        Buffer.BlockCopy(apdu, 0, frame, 6, apdu.Length);
        await stream.WriteAsync(frame, ct).ConfigureAwait(false);
        await stream.FlushAsync(ct).ConfigureAwait(false);
    }

    // ---------------- FATEK ----------------

    private static async Task TestFatek()
    {
        Console.WriteLine("[FATEK]");
        var words = new Dictionary<int, ushort>();
        var bits = new Dictionary<int, byte>();
        await RunServer(async (stream, ct) =>
        {
            while (!ct.IsCancellationRequested)
            {
                var buf = new List<byte>(64);
                var one = new byte[1];
                while (true)
                {
                    int rd = await stream.ReadAsync(one, ct).ConfigureAwait(false);
                    if (rd == 0) return;
                    buf.Add(one[0]);
                    if (one[0] == 0x03) break;
                }

                string s = Encoding.ASCII.GetString(buf.ToArray());
                string slave = s.Substring(1, 2);
                string cmd = s.Substring(3, 2);
                string len = s.Substring(5, 2);
                string addr = s.Substring(7, cmd is "46" or "47" ? 6 : 4);
                int dataStart = 7 + addr.Length;
                int dataEnd = s.Length - 3; // SUM(2)+ETX(1)
                string data = s.Substring(dataStart, dataEnd - dataStart);
                int count = Convert.ToInt32(len, 16);

                string respData;
                if (cmd is "46" or "44")
                {
                    // read
                    if (cmd == "46")
                    {
                        var sb = new StringBuilder();
                        for (int i = 0; i < count; i++)
                        {
                            int a = int.Parse(addr.Substring(1), System.Globalization.NumberStyles.HexNumber) + i;
                            words.TryGetValue(a, out ushort val);
                            sb.Append(val.ToString("X4"));
                        }

                        respData = sb.ToString();
                    }
                    else
                    {
                        var sb = new StringBuilder();
                        for (int i = 0; i < count; i++)
                        {
                            int a = int.Parse(addr.Substring(1), System.Globalization.NumberStyles.HexNumber) + i;
                            bits.TryGetValue(a, out byte val);
                            sb.Append(val == 0 ? '0' : '1');
                        }

                        respData = sb.ToString();
                    }
                }
                else
                {
                    // write
                    if (cmd == "47")
                    {
                        for (int i = 0; i < count; i++)
                        {
                            int a = int.Parse(addr.Substring(1), System.Globalization.NumberStyles.HexNumber) + i;
                            words[a] = (ushort)Convert.ToInt32(data.Substring(i * 4, 4), 16);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            int a = int.Parse(addr.Substring(1), System.Globalization.NumberStyles.HexNumber) + i;
                            bits[a] = data[i] == '1' ? (byte)1 : (byte)0;
                        }
                    }

                    respData = string.Empty;
                }

                var rsb = new StringBuilder();
                rsb.Append((char)0x02);
                rsb.Append(slave);
                rsb.Append(cmd);
                rsb.Append('0'); // error 0
                rsb.Append(respData);
                rsb.Append(Checksum(rsb.ToString()));
                rsb.Append((char)0x03);
                var resp = Encoding.ASCII.GetBytes(rsb.ToString());
                await stream.WriteAsync(resp, ct).ConfigureAwait(false);
                await stream.FlushAsync(ct).ConfigureAwait(false);
            }
        });

        int port = NextPort();
        var plugin = new FatekDevicePlugin.FatekDevicePlugin();
        await using var w = plugin.CreateSession(Opt(port));
        await w.WriteAsync(new WriteRequest(0x10, RegisterType.HoldingRegister, new[] { (ushort)0x1234, (ushort)0x5678 }), CancellationToken.None);
        await using var r = plugin.CreateSession(Opt(port));
        var d = await r.ReadAsync(new ReadRequest(0x10, 2, RegisterType.HoldingRegister), CancellationToken.None);
        Check("D 寄存器 写→读 往返", d.Length == 4 && d[0] == 0x34 && d[1] == 0x12 && d[2] == 0x78 && d[3] == 0x56,
            $"得到 [{string.Join(" ", d.Select(b => b.ToString("X2")))}]");

        await using var bw = plugin.CreateSession(Opt(port));
        await bw.WriteAsync(new WriteRequest(10, RegisterType.Coil, new ushort[] { 1, 0, 1 }), CancellationToken.None);
        await using var br = plugin.CreateSession(Opt(port));
        var bd = await br.ReadAsync(new ReadRequest(10, 3, RegisterType.Coil), CancellationToken.None);
        Check("M 继电器 写→读 往返", bd.Length == 3 && bd[0] == 1 && bd[1] == 0 && bd[2] == 1,
            $"得到 [{string.Join(" ", bd)}]");
    }

    private static string Checksum(string s)
    {
        int sum = 0;
        foreach (char c in s) sum += c;
        return (sum & 0xFF).ToString("X2");
    }

    // ---------------- Keyence KV ----------------

    private static async Task TestKeyence()
    {
        Console.WriteLine("[Keyence KV Host Link]");
        var dm = new Dictionary<int, int>();
        var relay = new Dictionary<int, int>();
        await RunServer(async (stream, ct) =>
        {
            while (!ct.IsCancellationRequested)
            {
                var buf = new StringBuilder();
                var one = new byte[1];
                while (true)
                {
                    int rd = await stream.ReadAsync(one, ct).ConfigureAwait(false);
                    if (rd == 0) return;
                    buf.Append((char)one[0]);
                    if (one[0] == '\r') break;
                }

                string cmd = buf.ToString();
                string unit = cmd.Substring(1, 2);
                string verb = cmd.Substring(3, 2);
                string resp;
                if (verb == "RD")
                {
                    var parts = cmd.Substring(6).TrimEnd('\r').Trim().Split(' ', 2);
                    string dev = parts[0];
                    if (dev.StartsWith("DM"))
                    {
                        int a = int.Parse(dev.Substring(2));
                        dm.TryGetValue(a, out int val);
                        resp = $"@{unit}RD {val:D5}\r";
                    }
                    else if (dev.StartsWith('X'))
                    {
                        int a = int.Parse(dev.Substring(1));
                        relay.TryGetValue(a, out int val);
                        resp = $"@{unit}RD {val}\r";
                    }
                    else
                    {
                        int a = int.Parse(dev);
                        relay.TryGetValue(a, out int val);
                        resp = $"@{unit}RD {val}\r";
                    }
                }
                else if (verb == "WR")
                {
                    var parts = cmd.Substring(6).TrimEnd('\r').Trim().Split(' ', 2);
                    string dev = parts[0];
                    int val = int.Parse(parts[1]);
                    int a = int.Parse(dev.Substring(2));
                    dm[a] = val;
                    resp = $"@{unit}WR {val:D5}\r";
                }
                else // ST/RS
                {
                    string dev = cmd.Substring(6).TrimEnd('\r').Trim();
                    int a = int.Parse(dev);
                    relay[a] = verb == "ST" ? 1 : 0;
                    resp = $"@{unit}{verb} {dev}\r";
                }

                var rb = Encoding.ASCII.GetBytes(resp);
                await stream.WriteAsync(rb, ct).ConfigureAwait(false);
                await stream.FlushAsync(ct).ConfigureAwait(false);
            }
        });

        int port = NextPort();
        var plugin = new KeyenceDevicePlugin.KeyenceDevicePlugin();
        await using var w = plugin.CreateSession(Opt(port));
        await w.WriteAsync(new WriteRequest(10, RegisterType.HoldingRegister, new[] { (ushort)0x1234 }), CancellationToken.None);
        await using var r = plugin.CreateSession(Opt(port));
        var d = await r.ReadAsync(new ReadRequest(10, 1, RegisterType.HoldingRegister), CancellationToken.None);
        Check("DM 写→读 往返", d.Length == 2 && d[0] == 0x34 && d[1] == 0x12,
            $"得到 [{string.Join(" ", d.Select(b => b.ToString("X2")))}]");

        await using var bw = plugin.CreateSession(Opt(port));
        await bw.WriteAsync(new WriteRequest(20, RegisterType.Coil, new ushort[] { 1 }), CancellationToken.None);
        await using var br = plugin.CreateSession(Opt(port));
        var bd = await br.ReadAsync(new ReadRequest(20, 1, RegisterType.Coil), CancellationToken.None);
        Check("继电器 写→读 往返", bd.Length == 1 && bd[0] == 1, $"得到 {bd[0]}");
    }

    // ---------------- Fuji SPH NP1 ----------------

    private static async Task TestFuji()
    {
        Console.WriteLine("[Fuji MICREX-SPH NP1]");
        var words = new Dictionary<int, ushort>();
        var bits = new Dictionary<int, byte>();
        await RunServer(async (stream, ct) =>
        {
            while (!ct.IsCancellationRequested)
            {
                var h = await ReadExactAsync(stream, 3, ct).ConfigureAwait(false);
                if (h[0] != 0x5A) break;
                int dc = h[1] | (h[2] << 8);
                var block = await ReadExactAsync(stream, dc, ct).ConfigureAwait(false);
                if (block[0] != 0xFF) break;
                int numBytes = block[11] | (block[12] << 8);
                var data = new byte[numBytes];
                Buffer.BlockCopy(block, 13, data, 0, numBytes);
                byte command = data[0];
                byte devType = data[1];
                int addr = data[2] | (data[3] << 8) | (data[4] << 16) | (data[5] << 24);
                int count = data[6] | (data[7] << 8);

                byte[] respData;
                if (command == 0x01) // read
                {
                    var pl = new byte[devType == 0x01 ? count * 2 : count];
                    for (int i = 0; i < count; i++)
                    {
                        if (devType == 0x01)
                        {
                            words.TryGetValue(addr + i, out ushort v);
                            pl[i * 2] = (byte)v; pl[i * 2 + 1] = (byte)(v >> 8);
                        }
                        else
                        {
                            bits.TryGetValue(addr + i, out byte v);
                            pl[i] = v;
                        }
                    }

                    respData = pl;
                }
                else // write
                {
                    var pl = new byte[numBytes - 16];
                    Buffer.BlockCopy(data, 16, pl, 0, pl.Length);
                    for (int i = 0; i < count; i++)
                    {
                        if (devType == 0x01)
                        {
                            words[addr + i] = (ushort)(pl[i * 2] | (pl[i * 2 + 1] << 8));
                        }
                        else
                        {
                            bits[addr + i] = pl[i];
                        }
                    }

                    respData = Array.Empty<byte>();
                }

                // 响应块（含 BCC 1 字节）
                int n = 16 + respData.Length;
                var rblock = new byte[1 + 2 + 1 + 7 + 2 + n + 1];
                int o = 0;
                rblock[o++] = 0x00; // status OK
                rblock[o++] = block[1]; rblock[o++] = block[2]; // connId
                rblock[o++] = 0x11;
                for (int i = 0; i < 7; i++) rblock[o++] = 0x00;
                rblock[o++] = (byte)(n & 0xFF); rblock[o++] = (byte)((n >> 8) & 0xFF);
                Buffer.BlockCopy(data, 0, rblock, o, 16); o += 16;
                Buffer.BlockCopy(respData, 0, rblock, o, respData.Length); o += respData.Length;

                var frame = new byte[3 + rblock.Length];
                frame[0] = 0x5A;
                frame[1] = (byte)(rblock.Length & 0xFF);
                frame[2] = (byte)((rblock.Length >> 8) & 0xFF);
                Buffer.BlockCopy(rblock, 0, frame, 3, rblock.Length);
                frame[frame.Length - 1] = (byte)(rblock.Sum(b => b) & 0xFF);
                await stream.WriteAsync(frame, ct).ConfigureAwait(false);
                await stream.FlushAsync(ct).ConfigureAwait(false);
            }
        });

        int port = NextPort();
        var plugin = new FujiDevicePlugin.FujiDevicePlugin();
        await using var w = plugin.CreateSession(Opt(port));
        await w.WriteAsync(new WriteRequest(10, RegisterType.HoldingRegister, new[] { (ushort)0x1234, (ushort)0x5678 }), CancellationToken.None);
        await using var r = plugin.CreateSession(Opt(port));
        var d = await r.ReadAsync(new ReadRequest(10, 2, RegisterType.HoldingRegister), CancellationToken.None);
        Check("字设备 写→读 往返", d.Length == 4 && d[0] == 0x34 && d[1] == 0x12 && d[2] == 0x78 && d[3] == 0x56,
            $"得到 [{string.Join(" ", d.Select(b => b.ToString("X2")))}]");

        await using var bw = plugin.CreateSession(Opt(port));
        await bw.WriteAsync(new WriteRequest(20, RegisterType.Coil, new ushort[] { 1, 0, 1 }), CancellationToken.None);
        await using var br = plugin.CreateSession(Opt(port));
        var bd = await br.ReadAsync(new ReadRequest(20, 3, RegisterType.Coil), CancellationToken.None);
        Check("位设备 写→读 往返", bd.Length == 3 && bd[0] == 1 && bd[1] == 0 && bd[2] == 1,
            $"得到 [{string.Join(" ", bd)}]");
    }

    // ---------------- LoRaWAN ----------------

    private static async Task TestLoRa()
    {
        Console.WriteLine("[LoRaWAN (帧中继)]");
        byte[]? lastUplink = null;
        byte[]? lastDownlink = null;
        await RunServer(async (stream, ct) =>
        {
            while (!ct.IsCancellationRequested)
            {
                var one = new byte[1];
                int rd = await stream.ReadAsync(one, ct).ConfigureAwait(false);
                if (rd == 0) return;
                if (one[0] == 0xDD) // downlink
                {
                    var lenBuf = await ReadExactAsync(stream, 4, ct).ConfigureAwait(false);
                    int len = lenBuf[0] | (lenBuf[1] << 8) | (lenBuf[2] << 16) | (lenBuf[3] << 24);
                    lastDownlink = await ReadExactAsync(stream, len, ct).ConfigureAwait(false);
                    await stream.WriteAsync(new byte[] { 0x00 }, ct).ConfigureAwait(false);
                    await stream.FlushAsync(ct).ConfigureAwait(false);
                }
                else if (one[0] is 0x55 or 0x46) // fetch uplink / flags
                {
                    await ReadExactAsync(stream, 1, ct).ConfigureAwait(false); // port
                    if (lastUplink is null)
                    {
                        await stream.WriteAsync(new byte[] { 0xFF }, ct).ConfigureAwait(false);
                    }
                    else
                    {
                        var head = new List<byte> { 0x00 };
                        head.AddRange(BitConverter.GetBytes(lastUplink.Length));
                        head.AddRange(lastUplink);
                        await stream.WriteAsync(head.ToArray(), ct).ConfigureAwait(false);
                    }

                    await stream.FlushAsync(ct).ConfigureAwait(false);
                }
            }
        });

        int port = NextPort();
        var plugin = new LoRaDevicePlugin.LoRaDevicePlugin();

        // 预置上行帧（与插件 EncodeFrame 同构）：FPort=7, FRMPayload=[AA BB]
        lastUplink = LoRaCodec.EncodeFrame(2, 0, 7, new byte[] { 0xAA, 0xBB });
        await using var r = plugin.CreateSession(Opt(port));
        var d = await r.ReadAsync(new ReadRequest(7, 1, RegisterType.InputRegister), CancellationToken.None);
        Check("上行 FRMPayload 解码", d.Length == 2 && d[0] == 0xAA && d[1] == 0xBB,
            $"得到 [{string.Join(" ", d.Select(b => b.ToString("X2")))}]");

        await using var w = plugin.CreateSession(Opt(port));
        await w.WriteAsync(new WriteRequest(7, RegisterType.HoldingRegister, new[] { (ushort)0x1234 }), CancellationToken.None);
        // 解码下行帧校验 FPort 与 FRMPayload
        var dec = LoRaCodec.DecodeFrame(lastDownlink!);
        Check("下行帧 FPort/FRMPayload 编码",
            dec.FPort == 7 && dec.FrmPayload.Length == 2 && dec.FrmPayload[0] == 0x34 && dec.FrmPayload[1] == 0x12,
            $"FPort={dec.FPort} payload=[{string.Join(" ", dec.FrmPayload.Select(b => b.ToString("X2")))}]");
    }

    // ---------------- 工具 ----------------

    private static async Task<byte[]> ReadExactAsync(NetworkStream stream, int count, CancellationToken ct)
    {
        var buf = new byte[count];
        int off = 0;
        while (off < count)
        {
            int rd = await stream.ReadAsync(buf.AsMemory(off, count - off), ct).ConfigureAwait(false);
            if (rd == 0) throw new EndOfStreamException();
            off += rd;
        }

        return buf;
    }
}

/// <summary>LoRaWAN MAC 帧编解码（与插件实现同构，供验证端预置/校验帧）。</summary>
internal static class LoRaCodec
{
    private const uint DevAddr = 0x01020304;

    public static byte[] EncodeFrame(byte mtype, byte fctrl, byte fPort, byte[] frm)
    {
        var body = new List<byte>
        {
            (byte)(DevAddr & 0xFF), (byte)((DevAddr >> 8) & 0xFF), (byte)((DevAddr >> 16) & 0xFF), (byte)((DevAddr >> 24) & 0xFF),
            fctrl, 0x01, 0x00, fPort
        };
        body.AddRange(frm);
        uint sum = 0;
        foreach (byte b in body) sum = (sum + b) & 0xFFFFFFFF;
        var mic = new[] { (byte)sum, (byte)(sum >> 8), (byte)(sum >> 16), (byte)(sum >> 24) };
        var frame = new byte[1 + body.Count + 4];
        frame[0] = (byte)(mtype << 5);
        body.CopyTo(frame, 1);
        Buffer.BlockCopy(mic, 0, frame, 1 + body.Count, 4);
        return frame;
    }

    public static (byte MType, uint DevAddr, byte FCtrl, byte FPort, byte[] FrmPayload, byte[] Mic)
        DecodeFrame(byte[] frame)
    {
        byte mtype = (byte)(frame[0] >> 5);
        uint devAddr = (uint)(frame[1] | (frame[2] << 8) | (frame[3] << 16) | (frame[4] << 24));
        byte fctrl = frame[5];
        byte fPort = frame[8]; // FHDR = DevAddr(4)+FCtrl(1)+FCnt(2)，故 FPort 在索引 8
        int pl = frame.Length - 13; // 减去 MHDR(1)+FHDR(7)+FPort(1)+MIC(4)
        var frm = new byte[Math.Max(pl, 0)];
        if (pl > 0) Buffer.BlockCopy(frame, 9, frm, 0, pl);
        var mic = new byte[4];
        Buffer.BlockCopy(frame, frame.Length - 4, mic, 0, 4);
        return (mtype, devAddr, fctrl, fPort, frm, mic);
    }
}
