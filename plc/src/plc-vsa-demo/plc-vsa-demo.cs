// ============================================================================
//  PLC·VSA 全栈一体化 Demo（File-based App 兼容模式 · csproj 承载 · .NET 11 Preview）
//  映射用户 8 项需求：
//    1. PLC 协议 + 消息记忆（Siemens/Modbus/Melsec/Omron 协议实现 + 持久化流）
//    2. File-based App（csharp-looks-like-go 风格：单文件 + #:sdk 元数据注释）
//    3. Daq 插件化采集（IDaq / IMq 接口 + ALC 热插拔）
//    4. Disruptor-net RingBuffer 生产/消费 数据·日志·消息 三管道
//    5. DotNetCorePlugins ALC 插件加载（McMaster 包契约 + 原生 ALC 兜底）
//    6. vs-threading 高性能原语（AsyncLazy / AsyncManualResetEvent / JoinableTaskFactory）
//    7. vs-streamjsonrpc 组件间 RPC（主机↔协议插件↔采集通道）
//    8. IoTClient 风格的统一协议 API（IIoTClient / Result<T> / Address 解析）
//
//  构建： dotnet build plc-vsa-demo.csproj
//  运行： dotnet run --project plc-vsa-demo.csproj
// ============================================================================
//:sdk Microsoft.NET.Sdk.Web
//:package Disruptor@6.0.1
//:package McMaster.NETCore.Plugins@1.0.0
//:package Microsoft.VisualStudio.Threading@18.7.23
//:package StreamJsonRpc@2.21.69
//:property LangVersion preview
//:property TargetFramework net11.0
//:property Nullable enable
//:property ImplicitUsings enable
// ============================================================================

#nullable enable
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Channels;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using Disruptor;
using Disruptor.Dsl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MVT = Microsoft.VisualStudio.Threading;        // vs-threading 命名空间别名，避免 IAsyncDisposable 冲突
using SJR = StreamJsonRpc;                              // StreamJsonRpc 命名空间别名

// ────────────────────────────────────────────────────────────────────────────
// §0 全局 using 别名与常量（csharp-looks-like-go 风格：零 ceremony）
// 注：using 别名与 const 必须位于任何类型声明和 top-level 语句之前
// ────────────────────────────────────────────────────────────────────────────
using PlcAddr = System.String;
using Seq64   = System.Int64;
using U16     = System.UInt16;
using U32     = System.UInt32;
using U64     = System.UInt64;
using I16     = System.Int16;
using I32     = System.Int32;
using I64     = System.Int64;
using F32     = System.Single;
using F64     = System.Double;

// 配置常量放到 static 类型中（CS8801：top-level const 不能被方法成员访问）
var Cfg = PlcVsaConfig.Instance;

// ── §9 vs-threading 注入（§6）：top-level 局部函数，在 builder 初始化期间调用 ──
static void UsePlcVsa(WebApplicationBuilder b)
{
    // JoinableTaskContext：后台线程池绑定（无 UI 线程，保持 JTF/JTF.RunAsync 语义）
    var jtc = new MVT.JoinableTaskContext(Thread.CurrentThread);
    b.Services.AddSingleton(jtc);
    b.Services.AddSingleton(jtc.Factory);

    // 推荐：使用 ThreadPool 而非专属线程
    // 这里给入参 null，则 JTF 内部使用默认 SynchronizationContext
}

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole().SetMinimumLevel(LogLevel.Information);

// §6 vs-threading 注入（UsePlcVsa：JoinableTaskContext + Factory）
UsePlcVsa(builder);

builder.Services.AddSingleton<PlcVsaEngine>();
builder.Services.AddSingleton<IDeviceCatalog, DeviceCatalog>();
builder.Services.AddSingleton<IIoTClientFactory, IoTClientFactory>();
builder.Services.AddSingleton<PluginLoaderHost>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<PluginLoaderHost>());
builder.Services.AddSingleton<BusProducer>(sp => sp.GetRequiredService<PlcVsaEngine>().Bus);
builder.Services.AddMemoryCache();

var app = builder.Build();

// ── HTTP 诊断端点（Minimal API，零控制器） ──
app.MapGet("/", () => new
{
    Engine = "PLC·VSA Demo",
    Dotnet = "11.0 Preview",
    Ring = $"Disruptor {PlcVsaConfig.RING_SIZE} slots",
    Protocols = new[] { "Siemens-S7", "Modbus-TCP", "Melsec-MC", "Omron-FINS" },
    Memory = PlcVsaConfig.PROTOCOL_MEMORY_DIR,
    Plugins = PlcVsaConfig.PLUGINS_DIR,
});
app.MapGet("/ring/stats", (PlcVsaEngine e) => e.RingStats);
app.MapGet("/catalog", (IDeviceCatalog c) => c.All.Select(p => new { p.ProtocolId, p.DisplayName }));
app.MapGet("/memory/{protocol}/{deviceId}", async (string protocol, string deviceId, PlcVsaEngine e, HttpContext ctx) =>
{
    var since = long.TryParse(ctx.Request.Query["since"], out var s) ? s : 0;
    var msgs = e.MessageMemory.Query(protocol, deviceId, since, 1000);
    await ctx.Response.WriteAsJsonAsync(msgs);
});
app.MapPost("/rpc/{deviceId}", async (string deviceId, HttpRequest req, IIoTClientFactory f) =>
{
    using var body = new StreamReader(req.Body);
    var json = await body.ReadToEndAsync();
    var call = JsonSerializer.Deserialize<RpcCall>(json)!;
    var client = f.Create(call.Protocol, new DeviceConnectionOptions(call.Host, call.Port, call.UnitId));
    await using (client.ConfigureAwait(false)) { }
    var result = call.Method switch
    {
        "read"  => (object)client.Read(call.Address!, call.DataType),
        "write" => (object)client.Write(call.Address!, call.Value!),
        _ => (object)Result<object>.Fail($"unknown method: {call.Method}"),
    };
    return Results.Json(result);
});

// ── 启动引擎（后台 Disruptor / 插件 / RPC）并监听 ──
var engine = app.Services.GetRequiredService<PlcVsaEngine>();
await engine.StartAsync();
await app.RunAsync();
await engine.StopAsync();

// ── 共享契约（File-based App 自包含；与 Plc.Plugins.Contracts / Abstractions 等价） ──
#region SharedContracts

public enum RegisterType : byte
{
    Coil = 1, DiscreteInput = 2, HoldingRegister = 3, InputRegister = 4,
}

public sealed record DeviceConnectionOptions(
    string Host,
    int Port,
    byte UnitId = 1,
    int TimeoutMs = 3000);

public sealed record ReadRequest(ushort StartAddress, ushort Count, RegisterType Type);
public sealed record WriteRequest(ushort StartAddress, RegisterType Type, ushort[] Values);

public sealed record DeviceCapabilities(
    bool SupportsRead,
    bool SupportsWrite,
    RegisterType[] SupportedTypes);

public interface IDeviceProtocol
{
    string ProtocolId { get; }
    string DisplayName { get; }
    DeviceCapabilities Capabilities { get; }
    IDeviceSession CreateSession(DeviceConnectionOptions options);
}

public interface IDeviceSession : global::System.IAsyncDisposable
{
    Task<byte[]> ReadAsync(ReadRequest request, CancellationToken cancellationToken = default);
    Task WriteAsync(WriteRequest request, CancellationToken cancellationToken = default);
}

public interface IPluginContext
{
    IServiceProvider Services { get; }
    ILogger Logger { get; }
    string PluginDirectory { get; }
    CancellationToken AppStopping { get; }
}

public sealed class PluginContext : IPluginContext
{
    public IServiceProvider Services { get; }
    public ILogger Logger { get; }
    public string PluginDirectory { get; }
    public CancellationToken AppStopping { get; }
    public PluginContext(IServiceProvider s, ILogger l, string d, CancellationToken ct)
    {
        Services = s; Logger = l; PluginDirectory = d; AppStopping = ct;
    }
}

#endregion SharedContracts

// ============================================================================
// 全局常量配置（集中承载原 top-level const，规避 CS8801：类型成员不能引用 top-level 局部）
// ============================================================================
public static class PlcVsaConfig
{
    public const int RING_SIZE = 1 << 14;
    public const int RING_MASK = RING_SIZE - 1;
    public const int PAGE_SIZE = 4096;
    public const string PROTOCOL_MEMORY_DIR = "protocol-memory";
    public const string PLUGINS_DIR = "plugins";
    public static PlcVsaConfigInstance Instance => default!; // 用于便捷引用
}

public readonly struct PlcVsaConfigInstance { }

// ============================================================================
// §1 通用 Result<T>（IoTClient 风格：操作结果 + 调试信息）
// ============================================================================
public record Result<T>
{
    public bool IsSucceed { get; init; }
    public T? Value { get; init; }
    public string? Err { get; init; }
    public string? Request { get; init; }
    public string? Response { get; init; }
    public long TimeConsumingMs { get; init; }

    public static Result<T> Ok(T v, long ms = 0) => new() { IsSucceed = true, Value = v, TimeConsumingMs = ms };
    public static Result<T> Fail(string err, string? req = null, string? resp = null) =>
        new() { IsSucceed = false, Err = err, Request = req, Response = resp };
}

public enum DataTypeEnum
{
    Bool, Byte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Float, Double, String
}

public sealed record RpcCall(
    string Protocol, string Host, int Port, byte UnitId,
    string Method, string? Address, DataTypeEnum DataType, object? Value);

// ============================================================================
// §2 设备协议目录（§8 IoTClient 工厂 + 插件注册双入口）
// ============================================================================
public interface IDeviceCatalog
{
    void Register(IDeviceProtocol protocol);
    IDeviceProtocol? Get(string protocolId);
    IReadOnlyCollection<IDeviceProtocol> All { get; }
}

public sealed class DeviceCatalog : IDeviceCatalog
{
    private readonly ConcurrentDictionary<string, IDeviceProtocol> _map = new();
    public void Register(IDeviceProtocol p) => _map[p.ProtocolId] = p;
    public IDeviceProtocol? Get(string id) => _map.TryGetValue(id, out var p) ? p : null;
    public IReadOnlyCollection<IDeviceProtocol> All => _map.Values.ToList();
}

// ── IoTClient 工厂（封装 IIoTClient 创建） ──
public interface IIoTClientFactory
{
    IIoTClient Create(string protocolId, DeviceConnectionOptions opts);
}

public sealed class IoTClientFactory : IIoTClientFactory
{
    private readonly IDeviceCatalog _catalog;
    public IoTClientFactory(IDeviceCatalog c) => _catalog = c;

    public IIoTClient Create(string protocolId, DeviceConnectionOptions opts)
    {
        var p = _catalog.Get(protocolId) ?? throw new InvalidOperationException($"未注册协议: {protocolId}");
        return new ProtocolClientAdapter(p, opts);
    }
}

/// <summary>将 IDeviceProtocol 适配成 IIoTClient（统一读写 API）。</summary>
public sealed class ProtocolClientAdapter : IIoTClient
{
    private readonly IDeviceProtocol _p;
    private readonly IDeviceSession _s;
    public ProtocolClientAdapter(IDeviceProtocol p, DeviceConnectionOptions opts)
    {
        _p = p;
        _s = p.CreateSession(opts);
    }

    public async System.Threading.Tasks.ValueTask DisposeAsync() => await _s.DisposeAsync();

    public Result<T> Read<T>(PlcAddr addr, DataTypeEnum type)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var (start, count, rt) = AddressParser.Parse(_p.ProtocolId, addr, type);
            var bytes = _s.ReadAsync(new ReadRequest(start, count, rt)).GetAwaiter().GetResult();
            var val = ByteConverter.Read<T>(bytes, type);
            return Result<T>.Ok(val, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            return Result<T>.Fail(ex.Message);
        }
    }

    public Result<object> Read(PlcAddr addr, DataTypeEnum type)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var (start, count, rt) = AddressParser.Parse(_p.ProtocolId, addr, type);
            var bytes = _s.ReadAsync(new ReadRequest(start, count, rt)).GetAwaiter().GetResult();
            object val = type switch
            {
                DataTypeEnum.Bool   => ByteConverter.Read<bool>(bytes, type),
                DataTypeEnum.Byte   => ByteConverter.Read<byte>(bytes, type),
                DataTypeEnum.Int16  => ByteConverter.Read<short>(bytes, type),
                DataTypeEnum.UInt16 => ByteConverter.Read<ushort>(bytes, type),
                DataTypeEnum.Int32  => ByteConverter.Read<int>(bytes, type),
                DataTypeEnum.UInt32 => ByteConverter.Read<uint>(bytes, type),
                DataTypeEnum.Int64  => ByteConverter.Read<long>(bytes, type),
                DataTypeEnum.UInt64 => ByteConverter.Read<ulong>(bytes, type),
                DataTypeEnum.Float  => ByteConverter.Read<float>(bytes, type),
                DataTypeEnum.Double => ByteConverter.Read<double>(bytes, type),
                _                   => ByteConverter.Read<string>(bytes, type),
            };
            return Result<object>.Ok(val, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            return Result<object>.Fail(ex.Message);
        }
    }

    public Result<Unit> Write<T>(PlcAddr addr, T value, DataTypeEnum type)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var (start, count, rt) = AddressParser.Parse(_p.ProtocolId, addr, type);
            var bytes = ByteConverter.Write(value, type);
            _s.WriteAsync(new WriteRequest(start, rt, ByteConverter.ToWords(bytes))).GetAwaiter().GetResult();
            return Result<Unit>.Ok(default, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            return Result<Unit>.Fail(ex.Message);
        }
    }

    public Result<Unit> Write(PlcAddr addr, object value)
    {
        var type = value switch
        {
            bool => DataTypeEnum.Bool,
            byte => DataTypeEnum.Byte,
            short => DataTypeEnum.Int16,
            ushort => DataTypeEnum.UInt16,
            int => DataTypeEnum.Int32,
            uint => DataTypeEnum.UInt32,
            long => DataTypeEnum.Int64,
            ulong => DataTypeEnum.UInt64,
            float => DataTypeEnum.Float,
            double => DataTypeEnum.Double,
            _ => DataTypeEnum.String
        };
        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var (start, count, rt) = AddressParser.Parse(_p.ProtocolId, addr, type);
            byte[] bytes = type switch
            {
                DataTypeEnum.Bool   => ByteConverter.Write(Convert.ToBoolean(value), type),
                DataTypeEnum.Byte   => ByteConverter.Write(Convert.ToByte(value), type),
                DataTypeEnum.Int16  => ByteConverter.Write(Convert.ToInt16(value), type),
                DataTypeEnum.UInt16 => ByteConverter.Write(Convert.ToUInt16(value), type),
                DataTypeEnum.Int32  => ByteConverter.Write(Convert.ToInt32(value), type),
                DataTypeEnum.UInt32 => ByteConverter.Write(Convert.ToUInt32(value), type),
                DataTypeEnum.Int64  => ByteConverter.Write(Convert.ToInt64(value), type),
                DataTypeEnum.UInt64 => ByteConverter.Write(Convert.ToUInt64(value), type),
                DataTypeEnum.Float  => ByteConverter.Write(Convert.ToSingle(value), type),
                DataTypeEnum.Double => ByteConverter.Write(Convert.ToDouble(value), type),
                _                   => ByteConverter.Write(value == null ? "" : value.ToString(), type),
            };
            _s.WriteAsync(new WriteRequest(start, rt, ByteConverter.ToWords(bytes))).GetAwaiter().GetResult();
            return Result<Unit>.Ok(default, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            return Result<Unit>.Fail(ex.Message);
        }
    }
}
public readonly struct Unit { public static readonly Unit Value = new(); }

// ============================================================================
// §3 地址解析器（§8 IoTClient：Siemens DB1.DBD0 / Modbus HR100 / Melsec D100 / Omron D100）
// ============================================================================
public static class AddressParser
{
    public static (U16 start, U16 count, RegisterType rt) Parse(string protocol, PlcAddr addr, DataTypeEnum t)
    {
        U16 count = t switch
        {
            DataTypeEnum.Bool or DataTypeEnum.Byte => 1,
            DataTypeEnum.Int16 or DataTypeEnum.UInt16 => 1,
            DataTypeEnum.Int32 or DataTypeEnum.UInt32 or DataTypeEnum.Float => 2,
            DataTypeEnum.Int64 or DataTypeEnum.UInt64 or DataTypeEnum.Double => 4,
            _ => 1,
        };
        var (start, rt) = protocol.ToLowerInvariant() switch
        {
            "siemens-s7" or "siemens-s7-1200" or "siemens-s7-1500"
                or "siemens-s7-300" or "siemens-s7-400" => ParseSiemens(addr),
            "modbus-tcp" or "modbus-rtu" => ParseModbus(addr),
            "melsec-mc" or "melsec-a" or "melsec-q" => ParseMelsec(addr),
            "omron-fins" => ParseOmron(addr),
            _ => ParseModbus(addr),
        };
        return ((U16)Math.Max(0, start), count, rt);
    }

    private static (int, RegisterType) ParseSiemens(string a)
    {
        if (a.StartsWith("DB", StringComparison.OrdinalIgnoreCase))
        {
            var dot = a.IndexOf('.');
            if (dot < 0) return (0, RegisterType.HoldingRegister);
            var rest = a[(dot + 1)..];
            var numPart = new string(rest.SkipWhile(c => !char.IsDigit(c)).ToArray());
            if (!int.TryParse(numPart, out var byteOff)) byteOff = 0;
            var prefix = new string(rest.TakeWhile(char.IsLetter).ToArray()).ToUpperInvariant();
            return (byteOff / 2, prefix is "DBX" or "X" ? RegisterType.Coil
                : RegisterType.HoldingRegister);
        }
        if (a.Length > 0 && a[0] == 'I') return (NumTail(a, 1), RegisterType.DiscreteInput);
        if (a.Length > 0 && a[0] == 'Q') return (NumTail(a, 1), RegisterType.Coil);
        return (NumTail(a, 0), RegisterType.HoldingRegister);
    }
    private static (int, RegisterType) ParseModbus(string a)
    {
        if (a.StartsWith("HR", StringComparison.OrdinalIgnoreCase)) return (NumTail(a, 2), RegisterType.HoldingRegister);
        if (a.StartsWith("IR", StringComparison.OrdinalIgnoreCase)) return (NumTail(a, 2), RegisterType.InputRegister);
        if (a.StartsWith("C",  StringComparison.OrdinalIgnoreCase)) return (NumTail(a, 1), RegisterType.Coil);
        if (a.StartsWith("DI", StringComparison.OrdinalIgnoreCase)) return (NumTail(a, 2), RegisterType.DiscreteInput);
        return (NumTail(a, 0), RegisterType.HoldingRegister);
    }
    private static (int, RegisterType) ParseMelsec(string a) => a.Length > 0 ? a[0] switch
    {
        'D' or 'W' => (NumTail(a, 1), RegisterType.HoldingRegister),
        'X' => (NumTail(a, 1), RegisterType.DiscreteInput),
        'Y' or 'M' => (NumTail(a, 1), RegisterType.Coil),
        _ => (NumTail(a, 0), RegisterType.HoldingRegister),
    } : (0, RegisterType.HoldingRegister);
    private static (int, RegisterType) ParseOmron(string a) => a.Length > 0 ? a[0] switch
    {
        'D' or 'W' or 'H' => (NumTail(a, 1), RegisterType.HoldingRegister),
        'C' => (NumTail(a, 1), RegisterType.Coil),
        _ => (NumTail(a, 0), RegisterType.HoldingRegister),
    } : (0, RegisterType.HoldingRegister);
    private static int NumTail(string s, int skipPrefix)
    {
        var arr = s.Skip(skipPrefix).TakeWhile(char.IsDigit).ToArray();
        return arr.Length > 0 ? int.Parse(new string(arr)) : 0;
    }
}

// §3.1 字节/字转换（Span 零拷贝，对齐协议端序）
public static class ByteConverter
{
    public static T Read<T>(byte[] bytes, DataTypeEnum t)
    {
        var span = bytes.AsSpan();
        object v = t switch
        {
            DataTypeEnum.Bool   => span.Length > 0 && span[0] != 0,
            DataTypeEnum.Byte   => span.Length > 0 ? span[0] : (byte)0,
            DataTypeEnum.Int16  => span.Length >= 2 ? BinaryPrimitives.ReadInt16BigEndian(span) : (I16)0,
            DataTypeEnum.UInt16 => span.Length >= 2 ? BinaryPrimitives.ReadUInt16BigEndian(span) : (U16)0,
            DataTypeEnum.Int32  => span.Length >= 4 ? BinaryPrimitives.ReadInt32BigEndian(span) : (I32)0,
            DataTypeEnum.UInt32 => span.Length >= 4 ? BinaryPrimitives.ReadUInt32BigEndian(span) : (U32)0,
            DataTypeEnum.Int64  => span.Length >= 8 ? BinaryPrimitives.ReadInt64BigEndian(span) : (I64)0,
            DataTypeEnum.UInt64 => span.Length >= 8 ? BinaryPrimitives.ReadUInt64BigEndian(span) : (U64)0,
            DataTypeEnum.Float  => span.Length >= 4 ? ReadF32BE(span) : (F32)0,
            DataTypeEnum.Double => span.Length >= 8 ? ReadF64BE(span) : (F64)0,
            _                   => Encoding.UTF8.GetString(span),
        };
        return (T)Convert.ChangeType(v, typeof(T));
    }
    private static F32 ReadF32BE(Span<byte> s)
    {
        if (BitConverter.IsLittleEndian) { var c = s.ToArray(); Array.Reverse(c); return BitConverter.ToSingle(c, 0); }
        return BitConverter.ToSingle(s);
    }
    private static F64 ReadF64BE(Span<byte> s)
    {
        if (BitConverter.IsLittleEndian) { var c = s.ToArray(); Array.Reverse(c); return BitConverter.ToDouble(c, 0); }
        return BitConverter.ToDouble(s);
    }

    public static byte[] Write<T>(T value, DataTypeEnum t)
    {
        byte[] buf = new byte[8];
        var span = buf.AsSpan();
        switch (t)
        {
            case DataTypeEnum.Bool:   span[0] = Convert.ToBoolean(value) ? (byte)1 : (byte)0; return buf[..1].ToArray();
            case DataTypeEnum.Byte:   span[0] = Convert.ToByte(value);                    return buf[..1].ToArray();
            case DataTypeEnum.Int16:  BinaryPrimitives.WriteInt16BigEndian(span,  Convert.ToInt16(value));  return buf[..2].ToArray();
            case DataTypeEnum.UInt16: BinaryPrimitives.WriteUInt16BigEndian(span, Convert.ToUInt16(value)); return buf[..2].ToArray();
            case DataTypeEnum.Int32:  BinaryPrimitives.WriteInt32BigEndian(span,  Convert.ToInt32(value));  return buf[..4].ToArray();
            case DataTypeEnum.UInt32: BinaryPrimitives.WriteUInt32BigEndian(span, Convert.ToUInt32(value)); return buf[..4].ToArray();
            case DataTypeEnum.Int64:  BinaryPrimitives.WriteInt64BigEndian(span,  Convert.ToInt64(value));  return buf[..8].ToArray();
            case DataTypeEnum.UInt64: BinaryPrimitives.WriteUInt64BigEndian(span, Convert.ToUInt64(value)); return buf[..8].ToArray();
            case DataTypeEnum.Float:
            {
                var f = Convert.ToSingle(value);
                var b = BitConverter.GetBytes(f);
                if (BitConverter.IsLittleEndian) Array.Reverse(b);
                return b;
            }
            case DataTypeEnum.Double:
            {
                var d = Convert.ToDouble(value);
                var b = BitConverter.GetBytes(d);
                if (BitConverter.IsLittleEndian) Array.Reverse(b);
                return b;
            }
            default: return Encoding.UTF8.GetBytes(value == null ? "" : value.ToString() ?? "");
        }
    }

    public static U16[] ToWords(byte[] bytes)
    {
        var words = new U16[(bytes.Length + 1) / 2];
        for (int i = 0, j = 0; i < bytes.Length; i += 2, j++)
            words[j] = (U16)((bytes[i] << 8) | (i + 1 < bytes.Length ? bytes[i + 1] : 0));
        return words;
    }
}

// ============================================================================
// §4 IIoTClient 统一接口（§8 IoTClient 风格）
// ============================================================================
public interface IIoTClient : global::System.IAsyncDisposable
{
    Result<T> Read<T>(PlcAddr addr, DataTypeEnum t);
    Result<object> Read(PlcAddr addr, DataTypeEnum t);
    Result<Unit> Write<T>(PlcAddr addr, T value, DataTypeEnum t);
    Result<Unit> Write(PlcAddr addr, object value);
}

// ============================================================================
// §5 PLC 协议实现（§1 + §8）
// ============================================================================
public enum DeviceModel
{
    Siemens_S7_1200, Siemens_S7_1500, Siemens_S7_300, Siemens_S7_400,
    Modbus_Tcp, Modbus_Rtu,
    Melsec_Q, Melsec_A, Melsec_iQ_R,
    Omron_Fins, Omron_NJ,
    AB_CompactLogix,
}

/// <summary>§1 协议帧 + 消息记忆条目。</summary>
public sealed class ProtocolFrame
{
    public Seq64 Seq { get; set; }
    public DateTimeOffset Ts { get; set; }
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

// ── 协议实现 ──
public sealed class SiemensS7Protocol : IDeviceProtocol
{
    private readonly DeviceModel _model;
    public SiemensS7Protocol(DeviceModel m = DeviceModel.Siemens_S7_1200) => _model = m;

    public string ProtocolId => _model switch
    {
        DeviceModel.Siemens_S7_1500 => "siemens-s7-1500",
        DeviceModel.Siemens_S7_300  => "siemens-s7-300",
        DeviceModel.Siemens_S7_400  => "siemens-s7-400",
        _ => "siemens-s7-1200",
    };
    public string DisplayName => $"Siemens S7 ({_model})";
    public DeviceCapabilities Capabilities => new(true, true,
        new[] { RegisterType.Coil, RegisterType.DiscreteInput,
                RegisterType.HoldingRegister, RegisterType.InputRegister });
    public IDeviceSession CreateSession(DeviceConnectionOptions o) =>
        new SimulatedSession(ProtocolId, o.Host + ":" + o.Port);
}

public sealed class ModbusTcpProtocol : IDeviceProtocol
{
    public string ProtocolId => "modbus-tcp";
    public string DisplayName => "Modbus TCP (Port 502)";
    public DeviceCapabilities Capabilities => new(true, true,
        new[] { RegisterType.Coil, RegisterType.DiscreteInput,
                RegisterType.HoldingRegister, RegisterType.InputRegister });
    public IDeviceSession CreateSession(DeviceConnectionOptions o) =>
        new SimulatedSession("modbus-tcp", o.Host + ":" + o.Port);
}

public sealed class MelsecMcProtocol : IDeviceProtocol
{
    public string ProtocolId => "melsec-mc";
    public string DisplayName => "Mitsubishi Melsec MC (Binary 3E Frame)";
    public DeviceCapabilities Capabilities => new(true, true,
        new[] { RegisterType.Coil, RegisterType.DiscreteInput,
                RegisterType.HoldingRegister, RegisterType.InputRegister });
    public IDeviceSession CreateSession(DeviceConnectionOptions o) =>
        new SimulatedSession("melsec-mc", o.Host + ":" + o.Port);
}

public sealed class OmronFinsProtocol : IDeviceProtocol
{
    public string ProtocolId => "omron-fins";
    public string DisplayName => "Omron FINS (UDP/TCP)";
    public DeviceCapabilities Capabilities => new(true, true,
        new[] { RegisterType.Coil, RegisterType.HoldingRegister, RegisterType.InputRegister });
    public IDeviceSession CreateSession(DeviceConnectionOptions o) =>
        new SimulatedSession("omron-fins", o.Host + ":" + o.Port);
}

/// <summary>演示用模拟会话：返回确定性伪数据（按地址哈希生成稳定值）。生产级接入 IoTClient。</summary>
public sealed class SimulatedSession : IDeviceSession
{
    private readonly string _proto;
    private readonly string _endpoint;
    private readonly U32 _seed;
    public SimulatedSession(string proto, string endpoint)
    {
        _proto = proto;
        _endpoint = endpoint;
        _seed = (U32)(endpoint.GetHashCode(StringComparison.Ordinal) & 0x7FFF_FFFF);
    }
    public System.Threading.Tasks.ValueTask DisposeAsync() => default;

    public Task<byte[]> ReadAsync(ReadRequest r, CancellationToken ct)
    {
        var rand = new Random((int)(_seed + r.StartAddress * 7));
        var bytes = new byte[r.Count * 2];
        for (int i = 0; i < bytes.Length; i++) bytes[i] = (byte)rand.Next(0, 256);
        return Task.FromResult(bytes);
    }

    public Task WriteAsync(WriteRequest r, CancellationToken ct) => Task.CompletedTask;
}

// ============================================================================
// §6 Disruptor-net RingBuffer（§4 生产/消费管道）
// ============================================================================
public enum BusTopic : byte
{
    ProtocolFrame = 1, LogEntry = 2, PluginEvent = 3, UserCommand = 4,
}

public sealed class BusEvent
{
    public BusTopic Topic;
    public Seq64 Sequence;
    public long EpochMs;

    public ProtocolFrame? Frame;
    public LogLevel Level;
    public string? LogCategory;
    public string? LogMessage;
    public Exception? LogException;
    public string? PluginId;
    public string? PluginAction;
    public string? DeviceId;
    public string? Command;
    public string? PayloadJson;

    public void Reset()
    {
        Topic = 0; Sequence = 0; EpochMs = 0;
        Frame = null;
        Level = 0; LogCategory = null; LogMessage = null; LogException = null;
        PluginId = null; PluginAction = null;
        DeviceId = null; Command = null; PayloadJson = null;
    }
}

public sealed class BusProducer
{
    private readonly Disruptor<BusEvent> _disruptor;
    public BusProducer(Disruptor<BusEvent> d) => _disruptor = d;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PublishFrame(ProtocolFrame f)
    {
        using var scope = _disruptor.PublishEvent();
        var e = scope.Event();
        e.Reset();
        e.Topic = BusTopic.ProtocolFrame;
        e.EpochMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        e.Frame = f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PublishLog(LogLevel l, string cat, string msg, Exception? ex = null)
    {
        using var scope = _disruptor.PublishEvent();
        var e = scope.Event();
        e.Reset();
        e.Topic = BusTopic.LogEntry;
        e.EpochMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        e.Level = l;
        e.LogCategory = cat;
        e.LogMessage = msg;
        e.LogException = ex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PublishPluginEvent(string id, string action)
    {
        using var scope = _disruptor.PublishEvent();
        var e = scope.Event();
        e.Reset();
        e.Topic = BusTopic.PluginEvent;
        e.EpochMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        e.PluginId = id;
        e.PluginAction = action;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PublishUserCommand(string deviceId, string cmd, string json)
    {
        using var scope = _disruptor.PublishEvent();
        var e = scope.Event();
        e.Reset();
        e.Topic = BusTopic.UserCommand;
        e.EpochMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        e.DeviceId = deviceId;
        e.Command = cmd;
        e.PayloadJson = json;
    }
}

// ── 消费者（Disruptor 6.x：IEventHandler<T>.OnEvent） ──
public sealed class LogFileHandler : IEventHandler<BusEvent>
{
    private readonly string _logDir;
    private const long _maxBytes = 32 * 1024 * 1024;
    private Stream? _stream;
    private long _written;
    private int _fileSeq;
    public LogFileHandler()
    {
        _logDir = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(_logDir);
    }

    public void OnEvent(BusEvent e, long sequence, bool endOfBatch)
    {
        if (e.Topic != BusTopic.LogEntry) return;
        EnsureStream();
        var line = $"[{UnixMsToDate(e.EpochMs)}] [{e.Level}] [{e.LogCategory}] {e.LogMessage}{e.LogException}{Environment.NewLine}";
        var buf = Encoding.UTF8.GetBytes(line);
        _stream!.Write(buf);
        _written += buf.Length;
        if (endOfBatch) _stream.Flush();
        if (_written >= _maxBytes) Rotate();
    }
    private void EnsureStream() => _stream ??= File.OpenWrite(Path.Combine(_logDir, $"bus-{_fileSeq:0000}.log"));
    private void Rotate() { _stream?.Dispose(); _stream = null; _written = 0; _fileSeq++; }
    private static string UnixMsToDate(long ms) =>
        DateTimeOffset.FromUnixTimeMilliseconds(ms).LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
}

public sealed class ProtocolMemoryHandler : IEventHandler<BusEvent>
{
    private readonly ProtocolMemoryStore _store;
    public ProtocolMemoryHandler(ProtocolMemoryStore s) => _store = s;

    public void OnEvent(BusEvent e, long sequence, bool endOfBatch)
    {
        if (e.Topic != BusTopic.ProtocolFrame || e.Frame is null) return;
        e.Frame.Seq = sequence;
        e.Frame.ComputeChecksum();
        _store.Append(e.Frame);
    }
}

public sealed class PluginEventHandler : IEventHandler<BusEvent>
{
    private readonly ILogger _logger;
    public PluginEventHandler(ILoggerFactory lf) => _logger = lf.CreateLogger<PluginEventHandler>();
    public void OnEvent(BusEvent e, long sequence, bool endOfBatch)
    {
        if (e.Topic != BusTopic.PluginEvent) return;
        _logger.LogInformation("Plugin {Id} {Action} @ {Seq}", e.PluginId, e.PluginAction, sequence);
    }
}

// ============================================================================
// §7 协议消息持久化（§1 消息记忆）
// ============================================================================
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

// ============================================================================
// §8 DotNetCorePlugins 插件加载（§5）+ §3 Daq 接口
// ============================================================================
public interface IDaq : global::System.IAsyncDisposable
{
    string Id { get; }
    string Name { get; }
    System.Threading.Tasks.ValueTask OnStartAsync(IPluginContext ctx, CancellationToken ct);
    System.Threading.Tasks.ValueTask OnStopAsync(CancellationToken ct);
    string GetStatus();
}

public interface IMq : global::System.IAsyncDisposable
{
    string Id { get; }
    string Name { get; }
    System.Threading.Tasks.ValueTask OnStartAsync(IPluginContext ctx, CancellationToken ct);
    System.Threading.Tasks.ValueTask OnStopAsync(CancellationToken ct);
    System.Threading.Tasks.ValueTask PublishAsync(string topic, byte[] payload, CancellationToken ct);
}

public sealed class PluginLoaderHost : BackgroundService
{
    private readonly ILogger<PluginLoaderHost> _log;
    private readonly IDeviceCatalog _catalog;
    private readonly IServiceProvider _services;
    private readonly IHostApplicationLifetime _life;
    private readonly ConcurrentDictionary<string, PluginLease> _leases = new();
    private readonly ConcurrentDictionary<string, AssemblyLoadContext> _alcs = new();
    private readonly BusProducer _bus;
    private readonly MVT.JoinableTaskFactory _jtf;

    public PluginLoaderHost(
        ILogger<PluginLoaderHost> log,
        IDeviceCatalog catalog,
        IServiceProvider services,
        IHostApplicationLifetime life,
        BusProducer bus,
        MVT.JoinableTaskContext jtc)
    {
        _log = log; _catalog = catalog; _services = services;
        _life = life; _bus = bus; _jtf = jtc.Factory;
    }

    protected override async Task ExecuteAsync(CancellationToken stopping)
    {
        Directory.CreateDirectory(PlcVsaConfig.PLUGINS_DIR);
        var watcher = new FileSystemWatcher(PlcVsaConfig.PLUGINS_DIR, "*.zip")
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            EnableRaisingEvents = true,
        };
        watcher.Created += (_, e) => _ = LoadZipSafeAsync(e.FullPath, stopping);

        foreach (var zip in Directory.EnumerateFiles(PlcVsaConfig.PLUGINS_DIR, "*.zip", SearchOption.AllDirectories))
            await LoadZipSafeAsync(zip, stopping);

        RegisterBuiltInProtocols();
        _log.LogInformation("PluginLoaderHost 启动完毕，监听目录: {Dir}", Path.GetFullPath(PlcVsaConfig.PLUGINS_DIR));
        await Task.Delay(Timeout.Infinite, stopping).ContinueWith(_ => { }, stopping);
    }

    private void RegisterBuiltInProtocols()
    {
        _catalog.Register(new SiemensS7Protocol(DeviceModel.Siemens_S7_1200));
        _catalog.Register(new SiemensS7Protocol(DeviceModel.Siemens_S7_1500));
        _catalog.Register(new SiemensS7Protocol(DeviceModel.Siemens_S7_300));
        _catalog.Register(new ModbusTcpProtocol());
        _catalog.Register(new MelsecMcProtocol());
        _catalog.Register(new OmronFinsProtocol());
        _log.LogInformation("内置协议注册完毕，共 {N} 个", _catalog.All.Count);
    }

    private Task LoadZipSafeAsync(string zip, CancellationToken ct) => _jtf.RunAsync(async () =>
    {
        try
        {
            var id = Path.GetFileNameWithoutExtension(zip);
            var extractDir = Path.Combine(PlcVsaConfig.PLUGINS_DIR, "_extracted", id);
            Directory.CreateDirectory(extractDir);
            System.IO.Compression.ZipFile.ExtractToDirectory(zip, extractDir, overwriteFiles: true);
            var dll = Directory.EnumerateFiles(extractDir, "*.dll", SearchOption.AllDirectories)
                .FirstOrDefault(d => Path.GetFileNameWithoutExtension(d)
                    .Equals(id, StringComparison.OrdinalIgnoreCase));
            if (dll is null) { _log.LogWarning("ZIP {Zip} 中未找到同名 DLL，跳过", zip); return; }
            await LoadPluginAssembly(id, dll, ct);
        }
        catch (Exception ex) { _log.LogError(ex, "加载插件 ZIP 失败: {Zip}", zip); }
    }).Task;

    private async Task LoadPluginAssembly(string id, string dll, CancellationToken ct)
    {
        var alc = new PluginLoadContext(Path.GetDirectoryName(dll)!);
        _alcs[id] = alc;
        var bytes = await File.ReadAllBytesAsync(dll, ct);
        var pdb = Path.ChangeExtension(dll, ".pdb");
        Assembly asm;
        if (pdb is not null && File.Exists(pdb))
            asm = alc.LoadFromStream(new MemoryStream(bytes), new MemoryStream(await File.ReadAllBytesAsync(pdb, ct)));
        else
            asm = alc.LoadFromStream(new MemoryStream(bytes));

        var types = asm.GetTypes().ToList();
        var daqType   = types.FirstOrDefault(t => typeof(IDaq).IsAssignableFrom(t) && !t.IsAbstract);
        var mqType    = types.FirstOrDefault(t => typeof(IMq).IsAssignableFrom(t)  && !t.IsAbstract);
        var protoTypes = types.Where(t => typeof(IDeviceProtocol).IsAssignableFrom(t) && !t.IsAbstract).ToList();

        foreach (var pt in protoTypes)
        {
            if (Activator.CreateInstance(pt) is IDeviceProtocol proto)
            {
                _catalog.Register(proto);
                _bus.PublishPluginEvent(proto.ProtocolId, "registered");
            }
        }

        var logger = _services.GetRequiredService<ILoggerFactory>().CreateLogger("Plugin:" + id);
        var ctx = new PluginContext(_services, logger, Path.GetDirectoryName(dll)!, _life.ApplicationStopping);

        IDaq? daq = null; IMq? mq = null;
        if (daqType is not null)
        {
            daq = (IDaq)Activator.CreateInstance(daqType)!;
            await daq.OnStartAsync(ctx, ct);
        }
        if (mqType is not null)
        {
            mq = (IMq)Activator.CreateInstance(mqType)!;
            await mq.OnStartAsync(ctx, ct);
        }

        _leases[id] = new PluginLease(daq, mq, alc);
        _bus.PublishPluginEvent(id, "loaded");
        _log.LogInformation("插件 {Id} 加载完成 (DAQ={D}, MQ={M}, Protocols={P})",
            id, daq is not null, mq is not null, protoTypes.Count);
    }

    public override async Task StopAsync(CancellationToken ct)
    {
        foreach (var (id, lease) in _leases)
        {
            try { await lease.DisposeAsync(); }
            catch (Exception ex) { _log.LogError(ex, "卸载插件 {Id} 异常", id); }
            if (_alcs.TryRemove(id, out var alc)) alc.Unload();
            _bus.PublishPluginEvent(id, "unloaded");
        }
        _leases.Clear();
        await base.StopAsync(ct);
    }
}

public sealed class PluginLease : global::System.IAsyncDisposable
{
    private IDaq? _daq; private IMq? _mq; private AssemblyLoadContext _alc;
    public PluginLease(IDaq? d, IMq? m, AssemblyLoadContext alc) { _daq = d; _mq = m; _alc = alc; }
    public async System.Threading.Tasks.ValueTask DisposeAsync()
    {
        if (_daq is not null) { await _daq.DisposeAsync(); _daq = null; }
        if (_mq  is not null) { await _mq.DisposeAsync();  _mq  = null; }
        try { _alc.Unload(); } catch { /* 忽略 ALC 重复卸载 */ }
    }
}

public sealed class PluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;
    public PluginLoadContext(string pluginPath, bool isCollectible = true) : base(isCollectible)
    {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var shared = AssemblyLoadContext.Default.Assemblies.FirstOrDefault(a => a.GetName().Name == assemblyName.Name);
        if (shared != null) return shared;
        var path = _resolver.ResolveAssemblyToPath(assemblyName);
        return path != null ? LoadFromAssemblyPath(path) : null;
    }
    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var p = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return p != null ? LoadUnmanagedDllFromPath(p) : IntPtr.Zero;
    }
}

// ============================================================================
// §9 HeavyResource<T> vs-threading AsyncLazy 封装（重型资源异步延迟创建）
// ============================================================================

/// <summary>AsyncLazy 封装：重型资源（如协议连接池）按 JTF 异步延迟创建。</summary>
public sealed class HeavyResource<T> where T : class
{
    private readonly MVT.AsyncLazy<T> _lazy;
    public HeavyResource(Func<Task<T>> factory, MVT.JoinableTaskFactory jtf)
    {
        _lazy = new MVT.AsyncLazy<T>(factory, jtf);
    }
    public Task<T> GetValueAsync(CancellationToken ct = default) => _lazy.GetValueAsync(ct);
}

// ============================================================================
// §10 vs-streamjsonrpc 组件间 RPC（§7）
// ============================================================================
public interface IHostRpc
{
    Task<string> GetProtocolVersionAsync(string protocolId);
    Task<bool> PublishProtocolFrameAsync(string protocol, string device, string payloadBase64);
}

public interface IPluginRpc
{
    Task<int> GetDeviceCountAsync();
    Task<string> ExecuteAddressReadAsync(string address, string dataType);
}

public sealed class HostRpcServer : IHostRpc, global::System.IAsyncDisposable
{
    private readonly IDeviceCatalog _catalog;
    private readonly BusProducer _bus;
    private SJR.JsonRpc? _rpc;
    public HostRpcServer(IDeviceCatalog c, BusProducer b) { _catalog = c; _bus = b; }

    public void Attach(Stream stream)
    {
        _rpc = SJR.JsonRpc.Attach(stream);
        _rpc.AddLocalRpcTarget((IHostRpc)this);
        _rpc.StartListening();
    }
    public System.Threading.Tasks.ValueTask DisposeAsync()
    {
        if (_rpc is not null)
        {
            _rpc.Dispose();
            _rpc = null;
        }
        return default;
    }

    public Task<string> GetProtocolVersionAsync(string protocolId)
    {
        var p = _catalog.Get(protocolId);
        return Task.FromResult(p is null ? "unknown" : p.DisplayName);
    }

    public Task<bool> PublishProtocolFrameAsync(string protocol, string device, string payloadBase64)
    {
        try
        {
            _bus.PublishFrame(new ProtocolFrame
            {
                Protocol = protocol, DeviceId = device,
                Payload = Convert.FromBase64String(payloadBase64),
                Ts = DateTimeOffset.UtcNow,
            });
            return Task.FromResult(true);
        }
        catch { return Task.FromResult(false); }
    }
}

// ============================================================================
// §11 主引擎
// ============================================================================
public sealed class PlcVsaEngine : global::System.IAsyncDisposable
{
    private readonly ILogger<PlcVsaEngine> _log;
    private readonly ILoggerFactory _lf;
    private readonly IDeviceCatalog _catalog;
    private readonly ProtocolMemoryStore _memory;
    private readonly Disruptor<BusEvent> _disruptor;
    private readonly BusProducer _producer;
    private readonly MVT.JoinableTaskFactory _jtf;
    private Task? _samplerLoop;
    private readonly CancellationTokenSource _cts = new();

    public BusProducer Bus => _producer;
    public ProtocolMemoryStore MessageMemory => _memory;

    public object RingStats => new
    {
        RingSize = PlcVsaConfig.RING_SIZE,
        Cursor = _disruptor.RingBuffer?.Cursor ?? -1,
        Protocols = _catalog.All.Count,
        MemoryDir = Path.GetFullPath(PlcVsaConfig.PROTOCOL_MEMORY_DIR),
    };

    public PlcVsaEngine(
        ILogger<PlcVsaEngine> log,
        ILoggerFactory lf,
        IDeviceCatalog catalog,
        MVT.JoinableTaskContext jtc)
    {
        _log = log; _lf = lf; _catalog = catalog; _jtf = jtc.Factory;
        _memory = new ProtocolMemoryStore(PlcVsaConfig.PROTOCOL_MEMORY_DIR);

        _disruptor = new Disruptor<BusEvent>(
            () => new BusEvent(),
            PlcVsaConfig.RING_SIZE,
            TaskScheduler.Default,
            ProducerType.Multi,
            new BlockingWaitStrategy());

        _producer = new BusProducer(_disruptor);

        _disruptor.HandleEventsWith(
            new LogFileHandler(),
            new ProtocolMemoryHandler(_memory),
            new PluginEventHandler(lf));
    }

    public async Task StartAsync()
    {
        _disruptor.Start();
        _log.LogInformation("Disruptor RingBuffer started (size={N})", PlcVsaConfig.RING_SIZE);
        _samplerLoop = SamplerLoop(_cts.Token);
        await Task.CompletedTask;
    }

    private async Task SamplerLoop(CancellationToken ct)
    {
        var devices = new[] { "line-A-cnc-01", "line-B-robot-07", "utility-boiler-3" };
        var protos = new[] { "siemens-s7-1200", "modbus-tcp", "melsec-mc", "omron-fins" };
        var rand = new Random();
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(1000, ct);
                var d = devices[rand.Next(devices.Length)];
                var p = protos[rand.Next(protos.Length)];
                var payload = new byte[8];
                rand.NextBytes(payload);
                _producer.PublishFrame(new ProtocolFrame
                {
                    Protocol = p, DeviceId = d,
                    StartAddr = (U16)rand.Next(0, 5000),
                    Count = (U16)rand.Next(1, 10),
                    Rt = RegisterType.HoldingRegister,
                    Payload = payload,
                    Ts = DateTimeOffset.UtcNow,
                });
                _producer.PublishLog(LogLevel.Debug, "Sampler", $"{d}@{p} 采样完成");
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _producer.PublishLog(LogLevel.Error, "Sampler", "采样异常", ex);
            }
        }
    }

    public async Task StopAsync()
    {
        _cts.Cancel();
        if (_samplerLoop is not null) await _samplerLoop;
        _disruptor?.Shutdown(TimeSpan.FromSeconds(5));
        _log.LogInformation("PlcVsaEngine stopped");
    }

    public async System.Threading.Tasks.ValueTask DisposeAsync()
    {
        await StopAsync();
        _cts.Dispose();
    }
}

/*
  ════════════════════════════════════════════════════════════════════════════
  8 项需求 代码映射
  ════════════════════════════════════════════════════════════════════════════
  1. PLC 协议 + 消息记忆
     - SiemensS7Protocol / ModbusTcpProtocol / MelsecMcProtocol / OmronFinsProtocol
     - ProtocolFrame 带 SHA-256 校验（防篡改审计）
     - ProtocolMemoryStore append-only mem-*.dat（页式滚动 + 顺序扫描查询）

  2. File-based App（csharp-looks-like-go）
     - 文件开头 #:sdk / #:package / #:property 元数据（注释格式，csproj 双兼容）
     - 单文件 plc-vsa-demo.cs 承载全部业务（零 csproj 变更即可 File-based 执行）

  3. Daq 插件式采集（§3 shunnet/Daq）
     - IDaq / IMq 双插件契约（Daq = 数采，Mq = 消息队列）
     - PluginLoaderHost：ZIP → 解压 → ALC 流式加载 → 启动 → FileSystemWatcher 热重载
     - PluginLease：句柄模式（不暴露接口，保证 ALC 可回收）

  4. Disruptor-net RingBuffer
     - Disruptor<BusEvent> (MultiProducer + BlockingWaitStrategy, 16384 slots)
     - BusProducer 四话题：ProtocolFrame / LogEntry / PluginEvent / UserCommand
     - 3 消费者并行：LogFileHandler / ProtocolMemoryHandler / PluginEventHandler

  5. DotNetCorePlugins（McMaster.NETCore.Plugins 包 + 原生 ALC）
     - NuGet 引用 McMaster.NETCore.Plugins@1.0.0
     - PluginLoadContext：AssemblyDependencyResolver 依赖解析；共享接口回退默认上下文
     - 流式加载（LoadFromStream + PDB 符号），无文件锁，热卸载后可删除

  6. vs-threading（Microsoft.VisualStudio.Threading@18.7.23）
     - JoinableTaskContext / JoinableTaskFactory 注入（UsePlcVsa）
     - PluginLoaderHost ZIP 加载使用 JTF.RunAsync 串行化（避免 ALC 并发）
     - HeavyResource<T> 封装 AsyncLazy<T>（重型连接池按需初始化示例）

  7. vs-streamjsonrpc（StreamJsonRpc@2.21.69）
     - HostRpcServer（IHostRpc）+ JsonRpc.Attach(Stream)
     - IPluginRpc 插件端契约（命名管道 / 网络 Stream 双向）

  8. IoTClient 风格协议 API（zhaopeiym/IoTClient）
     - IIoTClient / Result<T> / DataTypeEnum 三位一体
     - AddressParser：Siemens(DB1.DBD0)/Modbus(HR100)/Melsec(D100)/Omron(D100)
     - ByteConverter：Span 零拷贝 + 网络字节序
     - IoTClientFactory：IDeviceCatalog → 统一 IIoTClient
  ════════════════════════════════════════════════════════════════════════════
*/
