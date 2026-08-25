// ============================================================================
// PlcVsa.Server — 微单体宿主（Vertical Slice Architecture · ASP.NET Core 11）
// 引用：PlcVsa.Contracts（共享接口）、PlcVsa.Frontend（Razor UI 静态资源）
// 集成：Disruptor / ALC 插件 / vs-threading / StreamJsonRpc / IoTClient 风格协议
// ============================================================================
#nullable enable
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Disruptor;
using Disruptor.Dsl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MVT = Microsoft.VisualStudio.Threading;
using SJR = StreamJsonRpc;
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
using PlcVsa.Contracts.Devices;
using PlcVsa.Contracts.Faults;
using PlcVsa.Contracts.Plugins;
using PlcVsa.Contracts.Models;
using PlcVsa.Server.Infrastructure;
using PlcVsa.Server.Slices;

// ── §A 注入 vs-threading（JTC/JTF） ──
static void AddPlcVsaThreading(WebApplicationBuilder b)
{
    var jtc = new MVT.JoinableTaskContext(Thread.CurrentThread);
    b.Services.AddSingleton(jtc);
    b.Services.AddSingleton(jtc.Factory);
}

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole().SetMinimumLevel(LogLevel.Information);

AddPlcVsaThreading(builder);

// 核心基础设施（单例）
builder.Services.AddSingleton<PlcVsaConfig>();
builder.Services.AddSingleton<PlcVsaEngine>();
builder.Services.AddSingleton<IDeviceCatalog, DeviceCatalog>();
builder.Services.AddSingleton<IIoTClientFactory, IoTClientFactory>();
builder.Services.AddSingleton<PluginLoaderHost>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<PluginLoaderHost>());
builder.Services.AddSingleton<BusProducer>(sp => sp.GetRequiredService<PlcVsaEngine>().Bus);
builder.Services.AddSingleton(sp => new ProtocolMemoryStore(PlcVsaConfig.PROTOCOL_MEMORY_DIR));
builder.Services.AddSingleton<IHostRpcServer, HostRpcServer>();
builder.Services.AddMemoryCache();

var app = builder.Build();

// 中间件管道：静态文件（承载 PlcVsa.Frontend 的 StaticWebAssets + wwwroot）
app.UseDefaultFiles();
app.UseStaticFiles();

// ── HTTP Minimal API（诊断 + RPC） ──
app.MapGet("/", () => new
{
    Engine = "PLC·VSA Server (Micro-Monolith)",
    Dotnet = "11.0 Preview",
    Ring = $"Disruptor {PlcVsaConfig.RING_SIZE} slots",
    Protocols = new[] { "Siemens-S7", "Modbus-TCP", "Melsec-MC", "Omron-FINS" },
    Memory = PlcVsaConfig.PROTOCOL_MEMORY_DIR,
    Plugins = PlcVsaConfig.PLUGINS_DIR,
});
app.MapGet("/ring/stats", (PlcVsaEngine e) => e.RingStats);
app.MapGet("/catalog", (IDeviceCatalog c) => c.All.Select(p => new { p.ProtocolId, p.DisplayName }));
app.MapGet("/memory/{protocol}/{deviceId}", async (string protocol, string deviceId, ProtocolMemoryStore mem, HttpContext ctx) =>
{
    var since = long.TryParse(ctx.Request.Query["since"], out var s) ? s : 0;
    var msgs = mem.Query(protocol, deviceId, since, 1000);
    await ctx.Response.WriteAsJsonAsync(msgs);
});
app.MapPost("/rpc/{deviceId}", async (string deviceId, HttpRequest req, IIoTClientFactory f) =>
{
    using var body = new StreamReader(req.Body);
    var json = await body.ReadToEndAsync();
    var call = JsonSerializer.Deserialize<RpcCall>(json)!;
    using var client = f.Create(call.Protocol, new DeviceConnectionOptions(call.Host, call.Port, call.UnitId));

    // IIoTClient.Read<T> 强类型泛型：根据 call.DataType 分发到具体 T，避免 box 丢失。
    // IIoTClient.Write<T> 通过 dynamic 运行期绑定到正确 T 重载。
    string dataTypeStr = call.DataType.ToString();
    object result = call.Method switch
    {
        "read" => call.DataType switch
        {
            DataTypeEnum.Bool   => client.Read<bool>(call.Address!, dataTypeStr),
            DataTypeEnum.Byte   => client.Read<byte>(call.Address!, dataTypeStr),
            DataTypeEnum.Int16  => client.Read<short>(call.Address!, dataTypeStr),
            DataTypeEnum.UInt16 => client.Read<ushort>(call.Address!, dataTypeStr),
            DataTypeEnum.Int32  => client.Read<int>(call.Address!, dataTypeStr),
            DataTypeEnum.UInt32 => client.Read<uint>(call.Address!, dataTypeStr),
            DataTypeEnum.Int64  => client.Read<long>(call.Address!, dataTypeStr),
            DataTypeEnum.UInt64 => client.Read<ulong>(call.Address!, dataTypeStr),
            DataTypeEnum.Float  => client.Read<float>(call.Address!, dataTypeStr),
            DataTypeEnum.Double => client.Read<double>(call.Address!, dataTypeStr),
            _                   => client.Read<string>(call.Address!, dataTypeStr),
        },
        "write" => DispatchWrite(client, call.Address!, call.DataType, call.Value),
        _ => Result.Fail($"unknown method: {call.Method}"),
    };
    return Results.Json(result);

    // 局部函数：同上文 read 一样，按 DataTypeEnum 分发到 IIoTClient.Write<T>(addr, T value)。
    static Result DispatchWrite(IIoTClient client, string addr, DataTypeEnum dt, object? v)
    {
        try
        {
            return dt switch
            {
                DataTypeEnum.Bool   => client.Write(addr, Convert.ToBoolean(v)),
                DataTypeEnum.Byte   => client.Write(addr, Convert.ToByte(v)),
                DataTypeEnum.Int16  => client.Write(addr, Convert.ToInt16(v)),
                DataTypeEnum.UInt16 => client.Write(addr, Convert.ToUInt16(v)),
                DataTypeEnum.Int32  => client.Write(addr, Convert.ToInt32(v)),
                DataTypeEnum.UInt32 => client.Write(addr, Convert.ToUInt32(v)),
                DataTypeEnum.Int64  => client.Write(addr, Convert.ToInt64(v)),
                DataTypeEnum.UInt64 => client.Write(addr, Convert.ToUInt64(v)),
                DataTypeEnum.Float  => client.Write(addr, Convert.ToSingle(v)),
                DataTypeEnum.Double => client.Write(addr, Convert.ToDouble(v)),
                _                   => client.Write(addr, v?.ToString() ?? ""),
            };
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
});

// ── 启动引擎并运行 ──
var engine = app.Services.GetRequiredService<PlcVsaEngine>();
await engine.StartAsync();
await app.RunAsync();
await engine.StopAsync();
