// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Profinet.Melsec.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 三菱计算机链接协议，适用FX3U系列，FX3G，FX3S等等系列，通常在PLC侧连接的是485的接线口
/// Mitsubishi Computer Link Protocol, suitable for FX3U series, FX3G, FX3S, etc., usually the 485 connection port is connected on the PLC side
/// </summary>
/// <remarks>
/// 关于在PLC侧的配置信息，协议：专用协议  传送控制步骤：格式一  站号设置：0
/// </remarks>
public class MelsecFxLinks : DeviceSerialPort, IReadWriteFxLinks, IReadWriteDevice, IReadWriteNet, IReadWriteDeviceStation
{
	private byte station = 0;

	private byte watiingTime = 0;

	private bool sumCheck = true;

	/// <summary>
	/// PLC的当前的站号，需要根据实际的值来设定，默认是0
	/// The current station number of the PLC needs to be set according to the actual value. The default is 0.
	/// </summary>
	public byte Station
	{
		get
		{
			return station;
		}
		set
		{
			station = value;
		}
	}

	/// <summary>
	/// 报文等待时间，单位10ms，设置范围为0-15
	/// Message waiting time, unit is 10ms, setting range is 0-15
	/// </summary>
	public byte WaittingTime
	{
		get
		{
			return watiingTime;
		}
		set
		{
			if (watiingTime > 15)
			{
				watiingTime = 15;
			}
			else
			{
				watiingTime = value;
			}
		}
	}

	/// <summary>
	/// 是否启动和校验
	/// Whether to start and sum verify
	/// </summary>
	public bool SumCheck
	{
		get
		{
			return sumCheck;
		}
		set
		{
			sumCheck = value;
		}
	}

	/// <summary>
	/// 当前的PLC的Fxlinks协议格式，通常是格式1，或是格式4，所以此处可以设置1，或者是4
	/// The current PLC Fxlinks protocol format is usually format 1 or format 4, so it can be set to 1 or 4 here
	/// </summary>
	public int Format { get; set; } = 1;

	/// <summary>
	/// 实例化默认的对象
	/// Instantiate the default object
	/// </summary>
	public MelsecFxLinks()
	{
		base.ByteTransform = new RegularByteTransform();
		base.WordLength = 1;
		LogMsgFormatBinary = false;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new MelsecFxLinksMessage(Format, SumCheck);
	}

	public override byte[] PackCommandWithHeader(byte[] command)
	{
		return MelsecFxLinksHelper.PackCommandWithHeader(this, command);
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return MelsecFxLinksHelper.CheckPlcResponse(response);
	}

	public virtual OperateResult<MelsecFxLinksAddress> AnalyzeAddress(string address)
	{
		return MelsecFxLinksAddress.ParseFrom(address);
	}

	protected override OperateResult<List<byte[]>> BuildDeviceReadCommand(string address, ushort length, bool isBit)
	{
		return MelsecFxLinksHelper.BuildReadCommand(this, address, length, isBit);
	}

	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, byte[] value)
	{
		return MelsecFxLinksHelper.BuildWriteByteCommand(this, address, value);
	}

	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, bool[] value)
	{
		return MelsecFxLinksHelper.BuildWriteBoolCommand(this, address, value);
	}

	protected override OperateResult<byte[]> ParseDeviceReadResult(string address, byte[] buildCommand, byte[] content)
	{
		return MelsecFxLinksHelper.ExtraResponse(content);
	}

	protected override OperateResult<bool[]> ParseDeviceReadBoolResult(string address, byte[] buildCommand, byte[] content, ushort length)
	{
		return OperateResult.CreateSuccessResult(content.Select((byte m) => m == 49).ToArray());
	}

	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		if (address.IndexOf('.') > 0)
		{
			return HslHelper.ReadBool(this, address, length);
		}
		return base.ReadBool(address, length);
	}

	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		if (address.IndexOf('.') > 0)
		{
			return await HslHelper.ReadBoolAsync(this, address, length);
		}
		return await base.ReadBoolAsync(address, length);
	}

	[HslMqttApi(Description = "Start the PLC operation, you can carry additional parameter information and specify the station number. Example: s=2; Note: The semicolon is required.")]
	/// <summary>
	/// [商业授权] 启动PLC的操作，可以携带额外的参数信息，指定站号。举例：s=2; 注意：分号是必须的。
	/// [Authorization] Start the PLC operation, you can carry additional parameter information and specify the station number. Example: s=2; Note: The semicolon is required.
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="parameter">允许携带的参数信息，例如s=2; 也可以为空</param>
	/// <returns>是否启动成功</returns>
	public OperateResult StartPLC(string parameter = "")
	{
		return MelsecFxLinksHelper.StartPLC(this, parameter);
	}

	[HslMqttApi(Description = "Stop PLC operation, you can carry additional parameter information and specify the station number. Example: s=2; Note: The semicolon is required.")]
	/// <summary>
	/// [商业授权] 停止PLC的操作，可以携带额外的参数信息，指定站号。举例：s=2; 注意：分号是必须的。
	/// [Authorization] Stop PLC operation, you can carry additional parameter information and specify the station number. Example: s=2; Note: The semicolon is required.
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="parameter">允许携带的参数信息，例如s=2; 也可以为空</param>
	/// <returns>是否停止成功</returns>
	public OperateResult StopPLC(string parameter = "")
	{
		return MelsecFxLinksHelper.StopPLC(this, parameter);
	}

	[HslMqttApi(Description = "Read the PLC model information, you can carry additional parameter information, and specify the station number. Example: s=2; Note: The semicolon is required.")]
	/// <summary>
	/// [商业授权] 读取PLC的型号信息，可以携带额外的参数信息，指定站号。举例：s=2; 注意：分号是必须的。
	/// [Authorization] Read the PLC model information, you can carry additional parameter information, and specify the station number. Example: s=2; Note: The semicolon is required.
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="parameter">允许携带的参数信息，例如s=2; 也可以为空</param>
	/// <returns>带PLC型号的结果信息</returns>
	public OperateResult<string> ReadPlcType(string parameter = "")
	{
		return MelsecFxLinksHelper.ReadPlcType(this, parameter);
	}

	public override string ToString()
	{
		return $"MelsecFxLinks[{base.PortName}:{base.BaudRate}]";
	}
}
