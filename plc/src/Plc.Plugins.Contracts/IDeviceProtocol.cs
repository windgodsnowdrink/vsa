using System.Diagnostics;

namespace Plc.Plugins.Contracts;

/// <summary>寄存器 / 线圈类型（对应 Modbus 功能码语义）。</summary>
public enum RegisterType
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

/// <summary>元素数据类型（品牌无关；适配层映射到各驱动原生的 DataType）。</summary>
public enum ElementType
{
    Unspecified = 0,
    Bool,
    Int16,
    UInt16,
    Int32,
    UInt32,
    Float,
    Double,
    String,
    ByteArray,
}

/// <summary>设备连接参数。</summary>
public sealed record DeviceConnectionOptions(
    string Host,
    int Port,
    byte UnitId = 1,
    int TimeoutMs = 3000,
    int? Rack = null,
    int? Slot = null);

/// <summary>读请求。</summary>
public sealed record ReadRequest(
    ushort StartAddress,
    ushort Count,
    RegisterType Type,
    string? AddressText = null,
    ElementType ElementType = ElementType.Unspecified);

/// <summary>写请求（单值或多值）。</summary>
public sealed record WriteRequest(
    ushort StartAddress,
    RegisterType Type,
    ushort[] Values,
    string? AddressText = null,
    ElementType ElementType = ElementType.Unspecified,
    object[]? TypedValues = null);

/// <summary>协议能力声明。</summary>
public sealed record DeviceCapabilities(
    bool SupportsRead,
    bool SupportsWrite,
    RegisterType[] SupportedTypes);

/// <summary>
/// 设备协议抽象（Tier1 ALC 插件实现）。一个插件可实现一个或多个协议。
/// 宿主经共享契约（ALC 共享程序集）加载；插件在 <see cref="IPlugin.StartAsync"/> 内自注册到 <see cref="IDeviceCatalog"/>。
/// </summary>
public interface IDeviceProtocol
{
    /// <summary>协议唯一标识，如 "modbus.tcp"。</summary>
    string ProtocolId { get; }

    /// <summary>展示名。</summary>
    string DisplayName { get; }

    /// <summary>能力声明。</summary>
    DeviceCapabilities Capabilities { get; }

    /// <summary>基于连接参数创建一次设备会话。</summary>
    IDeviceSession CreateSession(DeviceConnectionOptions options);
}

/// <summary>
/// 一次设备连接的生命周期会话：按寄存器类型读 / 写原始字节，并产出统一设备帧 <see cref="DeviceFrame"/>。
/// <para>
/// ADR-119 决策 ①：帧能力折叠进本契约，不再引入 <c>IProtocolAdapter</c> 平行抽象。
/// 新增成员一律采用<strong>默认接口方法（C# 14 DIM）</strong>给出默认实现，
/// 现有协议实现（7 个 ALC 设备插件 + plc-gateway 内置协议）零改动即可编译。
/// </para>
/// </summary>
public interface IDeviceSession : IAsyncDisposable
{
    /// <summary>读取原始字节（协议相关：如 Modbus 为寄存器值大端排列）。</summary>
    Task<byte[]> ReadAsync(ReadRequest request, CancellationToken cancellationToken = default);

    /// <summary>写入值（values 含义与协议相关）。</summary>
    Task WriteAsync(WriteRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 协议标识（用于 <see cref="DeviceFrame.ProtocolId"/> 元数据）。
    /// 默认 <see cref="string.Empty"/>；协议实现应覆写为 <see cref="IDeviceProtocol.ProtocolId"/>。
    /// </summary>
    string ProtocolId => string.Empty;

    /// <summary>
    /// 是否支持跨请求复用底层连接（长连接）。
    /// 宿主数据面据此决定：true → 放入会话池复用；false → 每次请求后释放。
    /// 默认 false（保守：不复用，语义与既有「每次请求新建连接」的实现一致）。
    /// </summary>
    bool SupportsConnectionReuse => false;

    /// <summary>
    /// 批量读：把多个 <see cref="ReadRequest"/> 聚合成一次设备交互（或最少的交互次数），
    /// 返回与入参<strong>同序同长</strong>的帧集合。
    /// <para>
    /// 默认实现：串行转调 <see cref="ReadAsync"/> 并封装为帧。协议实现应覆写为真正的批量 / 合并读
    /// （如 Modbus 把连续地址合并为单次 0x03 请求），这是数据面吞吐的主要来源。
    /// </para>
    /// <para>
    /// 容错语义：<strong>单项失败不抛异常</strong>，落为 <see cref="DeviceFrame.Failure"/> 帧，
    /// 保证数据面一次采集的部分结果不因单点故障丢失；<see cref="OperationCanceledException"/> 原样上抛。
    /// </para>
    /// </summary>
    /// <param name="requests">读请求集合（顺序即返回顺序）。</param>
    /// <param name="deviceId">设备逻辑标识，用于帧元数据；null 时为 <see cref="string.Empty"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    async Task<IReadOnlyList<DeviceFrame>> ReadBatchAsync(
        IReadOnlyList<ReadRequest> requests,
        string? deviceId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requests);
        if (requests.Count == 0)
        {
            return Array.Empty<DeviceFrame>();
        }

        var protocol = ProtocolId;
        var id = deviceId ?? string.Empty;
        var frames = new DeviceFrame[requests.Count];

        for (int i = 0; i < requests.Count; i++)
        {
            var request = requests[i];
            var startedAt = Stopwatch.GetTimestamp();
            try
            {
                var payload = await ReadAsync(request, cancellationToken).ConfigureAwait(false);
                frames[i] = DeviceFrame.Read(
                    protocol,
                    id,
                    request,
                    payload,
                    sequence: i,
                    elapsed: Stopwatch.GetElapsedTime(startedAt));
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                frames[i] = DeviceFrame.Failure(
                    protocol,
                    id,
                    ex.Message,
                    request.Type,
                    request.StartAddress,
                    sequence: i,
                    elapsed: Stopwatch.GetElapsedTime(startedAt));
            }
        }

        return frames;
    }
}

/// <summary>
/// 宿主设备协议注册中心（共享接口，宿主提供实现）。
/// 插件经 <see cref="IPluginContext.Services"/> 解析并注册自身协议，HMI / AIOT 经此发现可用协议。
/// </summary>
public interface IDeviceCatalog
{
    /// <summary>注册一个设备协议实现。</summary>
    void Register(IDeviceProtocol protocol);

    /// <summary>按协议标识获取协议，未找到返回 null。</summary>
    IDeviceProtocol? Get(string protocolId);

    /// <summary>已注册协议快照。</summary>
    IReadOnlyCollection<IDeviceProtocol> All { get; }
}
