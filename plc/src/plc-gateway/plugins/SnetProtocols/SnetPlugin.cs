using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Plc.Plugins.Contracts;
using Snet.Model;

namespace SnetProtocols;

/// <summary>
/// SNET 协议插件：把反编译恢复的 Snet.* 驱动（Modbus / Siemens / Mitsubishi / Fatek / Omron /
/// Keyence / Fuji / AllenBradley / Delta / Panasonic / Sim …）统一适配为宿主 IDeviceProtocol，
/// 由 DotNetCorePlugins 按目录隔离加载。
/// 所有驱动都实现 IOn / IRead / IWrite（均派生自 DaqAbstract），故用一套反射式适配器覆盖；
/// 适配器按品牌探测 Basics 的连接属性（IpAddress / Port / Station / Rack / Slot）与 ProtocolType，
/// 覆盖不同 PLC 的连接模型（TCP / Rack-Slot / Station / 串口默认覆盖为 TCP）。
/// </summary>
public sealed class SnetPlugin : IPlugin
{
    public string Id => "snet.protocols";
    public string Name => "SNET PLC Protocols (series)";
    public string Version => "1.1.0";

    // 注册项：协议标识 + 展示名 + Operate 类型 + Basics 类型。
    // 适配器经反射探测 Basics 的连接属性与 ProtocolType，无需为每个品牌硬编码。
    private static readonly (string ProtocolId, string Display, Type Operate, Type Basics)[] Brands =
    {
        ("snet.modbus.tcp",       "Modbus TCP (SNET)",      typeof(Snet.Modbus.ModbusOperate),         typeof(Snet.Modbus.ModbusData.Basics)),
        ("snet.siemens.tcp",      "Siemens TCP (SNET)",     typeof(Snet.Siemens.SiemensOperate),       typeof(Snet.Siemens.SiemensData.Basics)),
        ("snet.mitsubishi.tcp",   "Mitsubishi TCP (SNET)",  typeof(Snet.Mitsubishi.MitsubishiOperate), typeof(Snet.Mitsubishi.MitsubishiData.Basics)),
        ("snet.fatek.tcp",        "Fatek TCP (SNET)",       typeof(Snet.Fatek.FatekOperate),           typeof(Snet.Fatek.FatekData.Basics)),
        ("snet.omron.tcp",        "Omron TCP (SNET)",       typeof(Snet.Omron.OmronOperate),           typeof(Snet.Omron.OmronData.Basics)),
        ("snet.keyence.tcp",      "Keyence TCP (SNET)",     typeof(Snet.Keyence.KeyenceOperate),       typeof(Snet.Keyence.KeyenceData.Basics)),
        ("snet.fuji.tcp",         "Fuji TCP (SNET)",        typeof(Snet.Fuji.FujiOperate),             typeof(Snet.Fuji.FujiData.Basics)),
        ("snet.allenbradley.tcp", "AllenBradley TCP (SNET)",typeof(Snet.AllenBradley.AllenBradleyOperate), typeof(Snet.AllenBradley.AllenBradleyData.Basics)),
        ("snet.delta.tcp",        "Delta TCP (SNET)",       typeof(Snet.Delta.DeltaOperate),           typeof(Snet.Delta.DeltaData.Basics)),
        ("snet.panasonic.tcp",    "Panasonic TCP (SNET)",   typeof(Snet.Panasonic.PanasonicOperate),   typeof(Snet.Panasonic.PanasonicData.Basics)),
        ("snet.sim",              "Sim (SNET, in-memory)",  typeof(Snet.Sim.SimOperate),               typeof(Snet.Sim.SimData.Basics)),
    };

    public Task StartAsync(IPluginContext context, CancellationToken cancellationToken = default)
    {
        var catalog = (IDeviceCatalog?)context.Services.GetService(typeof(IDeviceCatalog));
        if (catalog is null)
        {
            context.Logger.LogWarning("未找到 IDeviceCatalog，SNET 协议未注册");
            return Task.CompletedTask;
        }

        int ok = 0;
        foreach (var b in Brands)
        {
            try
            {
                catalog.Register(new SnetDeviceProtocol(b.ProtocolId, b.Display, b.Operate, b.Basics));
                ok++;
            }
            catch (Exception ex)
            {
                context.Logger.LogError(ex, "SNET 协议注册失败: {Id} (Operate={Operate}, Basics={Basics})",
                    b.ProtocolId, b.Operate.FullName, b.Basics.FullName);
            }
        }

        context.Logger.LogInformation("SNET 协议已注册 {Ok}/{Total}", ok, Brands.Length);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

/// <summary>把任意一个 Snet.XxxOperate 适配为 IDeviceProtocol。</summary>
public sealed class SnetDeviceProtocol : IDeviceProtocol
{
    private readonly Type _operateType;
    private readonly Type _basicsType;

    public SnetDeviceProtocol(string protocolId, string display, Type operateType, Type basicsType)
    {
        ProtocolId = protocolId;
        DisplayName = display;
        _operateType = operateType;
        _basicsType = basicsType;
    }

    public string ProtocolId { get; }
    public string DisplayName { get; }
    public DeviceCapabilities Capabilities { get; } = new(true, true,
        new[] { RegisterType.HoldingRegister, RegisterType.InputRegister, RegisterType.Coil, RegisterType.DiscreteInput });

    public IDeviceSession CreateSession(DeviceConnectionOptions options)
        => new SnetDeviceSession(_operateType, _basicsType, options, _operateType == typeof(Snet.Sim.SimOperate));
}

/// <summary>单一设备会话：构造 Basics -> 连接 -> Read/Write，复用 Snet 驱动。</summary>
public sealed class SnetDeviceSession : IDeviceSession
{
    private readonly object _operate;
    private readonly object _basics;
    private readonly bool _isSim;

    public SnetDeviceSession(Type operateType, Type basicsType, DeviceConnectionOptions options, bool isSim)
    {
        _isSim = isSim;
        _basics = Activator.CreateInstance(basicsType)!;
        SetProp(_basics, "IpAddress", options.Host);
        SetProp(_basics, "Port", options.Port);
        SetProp(_basics, "Station", (int)options.UnitId); // snet Basics.Station 为 int
        if (options.Rack.HasValue) SetProp(_basics, "Rack", options.Rack.Value);
        if (options.Slot.HasValue) SetProp(_basics, "Slot", options.Slot.Value);
        PickTcpProtocolType(_basics); // 覆盖各品牌默认串口变体（如 SiemensPPIOverTcp）
        _operate = Activator.CreateInstance(operateType, _basics)!;
    }

    public async Task<byte[]> ReadAsync(ReadRequest request, CancellationToken cancellationToken = default)
    {
        await ((Snet.Model.@interface.IOn)_operate).OnAsync(cancellationToken);
        var addr = request.AddressText ?? request.StartAddress.ToString();
        var dt = MapElementType(request.ElementType) ?? MapType(request.Type);
        var details = new Snet.Model.data.AddressDetails(addr, dt, (ushort)request.Count);
        // Sim 为内存模拟器：地址需标记为虚拟静态地址，否则按实际地址读取（无设备）返回失败
        if (_isSim) details.AddressType = Snet.Model.@enum.AddressType.VirtualStatic;
        var address = new Snet.Model.data.Address(details);
        var result = await ((Snet.Model.@interface.IRead)_operate).ReadAsync(address, cancellationToken);
        if (result is null || !result.GetDetails(out object? data))
            return Array.Empty<byte>();
        return ConvertContent(data);
    }

    public async Task WriteAsync(WriteRequest request, CancellationToken cancellationToken = default)
    {
        await ((Snet.Model.@interface.IOn)_operate).OnAsync(cancellationToken);
        var addr = request.AddressText ?? request.StartAddress.ToString();
        var dict = new ConcurrentDictionary<string, (object value, Snet.Model.@enum.EncodingType? encodingType)>();
        if (request.TypedValues is { Length: > 0 })
            dict[addr] = (request.TypedValues[0], null);
        else
            foreach (var v in request.Values) dict[addr] = (v, null);
        await ((Snet.Model.@interface.IWrite)_operate).WriteAsync(dict, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_operate is IAsyncDisposable ad) await ad.DisposeAsync();
        else if (_operate is IDisposable d) d.Dispose();
    }

    private static void SetProp(object target, string name, object value)
    {
        var prop = target.GetType().GetProperty(name);
        if (prop is not { CanWrite: true }) return;
        object? v = value;
        var t = prop.PropertyType;
        if (t != value.GetType())
        {
            var u = Nullable.GetUnderlyingType(t);
            v = Convert.ChangeType(value, u ?? t);
        }
        prop.SetValue(target, v);
    }

    /// <summary>挑选 TCP 版 ProtocolType（覆盖各品牌默认串口变体，如 SiemensPPIOverTcp / MelsecMcRtu）。</summary>
    private static void PickTcpProtocolType(object basics)
    {
        var prop = basics.GetType().GetProperty("ProtocolType");
        if (prop is not { CanWrite: true }) return;
        var enumType = prop.PropertyType;
        if (!enumType.IsEnum) return;
        var values = Enum.GetValues(enumType).Cast<object>().ToList();
        string Low(object e) => e.ToString()!.ToLowerInvariant();
        object? Pick(Func<string, bool> good) =>
            values.FirstOrDefault(e => { var s = Low(e); return good(s) && !s.Contains("ppi") && !s.Contains("rtu") && !s.Contains("serial") && !s.Contains("com"); });
        var chosen = Pick(s => s.Contains("net"))
                 ?? Pick(s => s.Contains("tcp"))
                 ?? values.First();
        prop.SetValue(basics, chosen);
    }

    private static Snet.Model.@enum.DataType? MapElementType(ElementType t) => t switch
    {
        ElementType.Bool => Snet.Model.@enum.DataType.Bool,
        ElementType.Int16 => Snet.Model.@enum.DataType.Int16,
        ElementType.UInt16 => Snet.Model.@enum.DataType.UInt16,
        ElementType.Int32 => Snet.Model.@enum.DataType.Int32,
        ElementType.UInt32 => Snet.Model.@enum.DataType.UInt32,
        ElementType.Float => Snet.Model.@enum.DataType.Float,
        ElementType.Double => Snet.Model.@enum.DataType.Double,
        ElementType.String => Snet.Model.@enum.DataType.String,
        ElementType.ByteArray => Snet.Model.@enum.DataType.Byte,
        _ => null,
    };

    private static Snet.Model.@enum.DataType MapType(RegisterType type) => type switch
    {
        RegisterType.Coil => Snet.Model.@enum.DataType.Bool,
        RegisterType.DiscreteInput => Snet.Model.@enum.DataType.Bool,
        _ => Snet.Model.@enum.DataType.Int16,
    };

    private static byte[] ConvertContent(object? content)
    {
        if (content is null) return Array.Empty<byte>();
        if (content is byte[] b) return b;
        if (content is string s) return System.Text.Encoding.UTF8.GetBytes(s);
        if (content is System.Collections.IDictionary dict)
        {
            using var ms = new MemoryStream();
            foreach (var v in dict.Values)
            {
                // Snet.Model.data.AddressValue 继承 AddressDetails，实际值在 ResultValue / OriginalValue
                object? val = v?.GetType().GetProperty("ResultValue")?.GetValue(v)
                          ?? v?.GetType().GetProperty("OriginalValue")?.GetValue(v)
                          ?? v;
                WriteValue(ms, val);
            }
            return ms.ToArray();
        }
        using var ms2 = new MemoryStream();
        WriteValue(ms2, content);
        return ms2.ToArray();
    }

    private static void WriteValue(MemoryStream ms, object? val)
    {
        switch (val)
        {
            case int i: ms.Write(BitConverter.GetBytes(i)); break;
            case short s: ms.Write(BitConverter.GetBytes(s)); break;
            case ushort us: ms.Write(BitConverter.GetBytes(us)); break;
            case uint ui: ms.Write(BitConverter.GetBytes(ui)); break;
            case float f: ms.Write(BitConverter.GetBytes(f)); break;
            case double d: ms.Write(BitConverter.GetBytes(d)); break;
            case bool bo: ms.Write(BitConverter.GetBytes(bo)); break;
            case byte by: ms.WriteByte(by); break;
            case byte[] arr: ms.Write(arr); break;
            case string str: ms.Write(System.Text.Encoding.UTF8.GetBytes(str)); break;
            default: break;
        }
    }
}
