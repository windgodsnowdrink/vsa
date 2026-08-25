using StreamJsonRpc;
using System.IO.Pipes;

namespace PlcGateway.Client;

/// <summary>与服务器 DeviceCfg 序列化字段对齐的只读 DTO（Newtonsoft PascalCase）。</summary>
internal sealed record DeviceInfo(
    string Id,
    string Protocol,
    string Host,
    int Port,
    int? UnitId,
    int? Rack,
    int? Slot,
    int PollIntervalMs,
    bool Enabled);

/// <summary>
/// RPC 冒烟测试客户端（需求 7 验证）：连接正在运行的 plc-gateway 的 NamedPipe，
/// 依次调用 gateway.listProtocols / listDevices / read / write，对返回做断言。
/// 退出码 0 = 全部 PASS，1 = 任一检查失败或无法连接。
/// 用法：GatewayClient [--pipe=名称] [--timeout=秒]
/// </summary>
public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var pipeName = GetArg(args, "--pipe=", "plc-gateway");
        var timeoutSec = int.Parse(GetArg(args, "--timeout=", "20"));

        Console.WriteLine($"[smoke] 连接 NamedPipe: {pipeName} (超时 {timeoutSec}s)");
        NamedPipeClientStream? pipe = null;
        var deadline = DateTime.UtcNow.AddSeconds(timeoutSec);
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                await pipe.ConnectAsync(1000);
                break;
            }
            catch
            {
                pipe?.Dispose();
                pipe = null;
                await Task.Delay(400);
            }
        }

        if (pipe is null || !pipe.IsConnected)
        {
            Console.Error.WriteLine("[smoke] FAIL: 无法在超时内连接到网关");
            return 1;
        }

        var rpc = JsonRpc.Attach(pipe);
        var failures = 0;
        void Check(string name, bool ok, string detail)
        {
            Console.WriteLine(ok ? $"[smoke] PASS {name}: {detail}" : $"[smoke] FAIL {name}: {detail}");
            if (!ok) failures++;
        }

        try
        {
            // 1) 协议清单：必须包含内置 sim 兜底 + SNET 三种协议
            var protocols = await rpc.InvokeAsync<string[]>("gateway.listProtocols");
            var protoSet = new HashSet<string>(protocols ?? Array.Empty<string>());
            Check("listProtocols",
                protoSet.Contains("builtin.sim") &&
                protoSet.Contains("snet.modbus.tcp") &&
                protoSet.Contains("snet.siemens.tcp") &&
                protoSet.Contains("snet.mitsubishi.tcp"),
                string.Join(", ", protoSet));

            // 2) 设备清单：应包含 enabled 的 dev-sim-1。
            // StreamJsonRpc 默认用 Newtonsoft.Json 序列化，结果与服务器 DeviceCfg 的
            // PascalCase 字段对齐，故用同名 POCO 接收（不可用 System.Text.Json.JsonElement）。
            var devices = await rpc.InvokeAsync<DeviceInfo[]>("gateway.listDevices");
            var devIds = new List<string>();
            bool simEnabled = false;
            foreach (var d in devices ?? Array.Empty<DeviceInfo>())
            {
                devIds.Add(d.Id);
                if (d.Id == "dev-sim-1") simEnabled = d.Enabled;
            }
            Check("listDevices", devIds.Contains("dev-sim-1") && simEnabled,
                "devices=[" + string.Join(", ", devIds) + $"], simEnabled={simEnabled}");

            // 3) 按需读：dev-sim-1 读 2 个保持寄存器（start=0, count=2, type=3）→ 4 字节
            var read = await rpc.InvokeAsync<byte[]>("gateway.read", "dev-sim-1", (ushort)0, (ushort)2, 3);
            Check("read", read is { Length: 4 },
                read is null ? "null" : Convert.ToHexString(read));

            // 4) 按需写：dev-sim-1 写 2 个保持寄存器（start=0, type=3, values=[11,22]）
            await rpc.InvokeAsync("gateway.write", "dev-sim-1", (ushort)0, 3, new ushort[] { 11, 22 });
            // 写后再次读，验证流水线未被写打断且返回仍为 4 字节
            var read2 = await rpc.InvokeAsync<byte[]>("gateway.read", "dev-sim-1", (ushort)0, (ushort)2, 3);
            Check("write+read", read2 is { Length: 4 },
                read2 is null ? "null" : Convert.ToHexString(read2));
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[smoke] FAIL: RPC 调用异常: {ex.Message}");
            failures++;
        }
        finally
        {
            try { rpc.Dispose(); } catch { }
            pipe.Dispose();
        }

        Console.WriteLine(failures == 0 ? "[smoke] ALL PASS" : $"[smoke] {failures} CHECK(S) FAILED");
        return failures == 0 ? 0 : 1;
    }

    private static string GetArg(string[] args, string prefix, string fallback)
    {
        foreach (var a in args)
            if (a.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return a.Substring(prefix.Length);
        return fallback;
    }
}
