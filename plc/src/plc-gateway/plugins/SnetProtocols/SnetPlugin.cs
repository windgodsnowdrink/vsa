using Plc.Plugins.Contracts;
using Snet.Model;
using Microsoft.Extensions.Logging;

namespace SnetProtocols;

/// <summary>
/// SNET 协议插件：把反编译恢复的 Snet.* 驱动（Modbus / Siemens / Mitsubishi …）
/// 统一适配为宿主的 IDeviceProtocol，由 DotNetCorePlugins 按目录隔离加载。
/// 所有驱动都实现 IOn(连接) / IRead / IWrite，故可用一套反射式适配器覆盖。
/// </summary>
public sealed class SnetPlugin : IPlugin
{
    public string Id => "snet.protocols";
    public string Name => "SNET PLC Protocols (Modbus/Siemens/Mitsubishi)";
    public string Version => "1.0.0";

    public Task StartAsync(IPluginContext context, CancellationToken cancellationToken = default)
    {
        var catalog = (IDeviceCatalog?)context.Services.GetService(typeof(IDeviceCatalog));
        if (catalog is null)
        {
            context.Logger.LogWarning("未找到 IDeviceCatalog，SNET 协议未注册");
            return Task.CompletedTask;
        }

        Register(catalog, "snet.modbus.tcp", "Modbus TCP (SNET)", typeof(Snet.Modbus.ModbusOperate), typeof(Snet.Modbus.ModbusData.Basics));
        Register(catalog, "snet.siemens.tcp", "Siemens TCP (SNET)", typeof(Snet.Siemens.SiemensOperate), typeof(Snet.Siemens.SiemensData.Basics));
        Register(catalog, "snet.mitsubishi.tcp", "Mitsubishi TCP (SNET)", typeof(Snet.Mitsubishi.MitsubishiOperate), typeof(Snet.Mitsubishi.MitsubishiData.Basics));

        context.Logger.LogInformation("SNET 协议已注册：snet.modbus.tcp / snet.siemens.tcp / snet.mitsubishi.tcp");
        return Task.CompletedTask;
    }

    private static void Register(IDeviceCatalog catalog, string protocolId, string display, Type operateType, Type basicsType)
    {
        catalog.Register(new SnetDeviceProtocol(protocolId, display, operateType, basicsType));
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
        => new SnetDeviceSession(_operateType, _basicsType, options);
}

/// <summary>单一设备会话：构造 Basics -> 连接 -> Read/Write，复用 Snet 驱动。</summary>
public sealed class SnetDeviceSession : IDeviceSession
{
    private readonly object _operate;
    private readonly object _basics;

    public SnetDeviceSession(Type operateType, Type basicsType, DeviceConnectionOptions options)
    {
        _basics = Activator.CreateInstance(basicsType)!;
        SetProp(_basics, "IpAddress", options.Host);
        SetProp(_basics, "Port", options.Port);
        SetProp(_basics, "Station", (byte)options.UnitId);
        _operate = Activator.CreateInstance(operateType, _basics)!;
    }

    public async Task<byte[]> ReadAsync(ReadRequest request, CancellationToken cancellationToken = default)
    {
        await ((Snet.Model.@interface.IOn)_operate).OnAsync(cancellationToken);
        var details = new Snet.Model.data.AddressDetails(request.StartAddress.ToString(), MapType(request.Type), (ushort)request.Count);
        var address = new Snet.Model.data.Address(details);
        var result = await ((Snet.Model.@interface.IRead)_operate).ReadAsync(address, cancellationToken);
        if (result is null || !result.GetDetails(out object? data))
            return Array.Empty<byte>();

        return ConvertContent(data);
    }

    public async Task WriteAsync(WriteRequest request, CancellationToken cancellationToken = default)
    {
        await ((Snet.Model.@interface.IOn)_operate).OnAsync(cancellationToken);
        var dict = new System.Collections.Concurrent.ConcurrentDictionary<string, (object value, Snet.Model.@enum.EncodingType? encodingType)>();
        dict[request.StartAddress.ToString()] = (request.Values, null);
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
        if (prop is { CanWrite: true }) prop.SetValue(target, value);
    }

    private static Snet.Model.@enum.DataType MapType(RegisterType type) => type switch
    {
        RegisterType.Coil => Snet.Model.@enum.DataType.Bool,
        RegisterType.DiscreteInput => Snet.Model.@enum.DataType.Bool,
        RegisterType.InputRegister => Snet.Model.@enum.DataType.Int16,
        _ => Snet.Model.@enum.DataType.Int16,
    };

    private static byte[] ConvertContent(object? content)
    {
        if (content is byte[] b) return b;
        if (content is System.Collections.IDictionary dict)
        {
            using var ms = new MemoryStream();
            foreach (var v in dict.Values)
            {
                var val = v?.GetType().GetProperty("Value")?.GetValue(v);
                if (val is int i) ms.Write(BitConverter.GetBytes(i));
                else if (val is short s) ms.Write(BitConverter.GetBytes(s));
                else if (val is float f) ms.Write(BitConverter.GetBytes(f));
            }
            return ms.ToArray();
        }
        return Array.Empty<byte>();
    }
}
