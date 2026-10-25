// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Profinet.Melsec.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 基于Qna 兼容3C帧的格式一的通讯，具体的地址需要参照三菱的基本地址
/// Based on Qna-compatible 3C frame format one communication, the specific address needs to refer to the basic address of Mitsubishi.
/// </summary>
public class MelsecA3CNet : DeviceSerialPort, IReadWriteA3C, IReadWriteDevice, IReadWriteNet
{
	private byte station = 0;

	/// <summary>
	/// 当前A3C协议的站编号信息
	/// Station number information of the current A3C protocol
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
	/// 当前的A3C协议是否使用和校验，默认使用
	/// Whether the current A3C protocol uses sum check, it is used by default
	/// </summary>
	public bool SumCheck { get; set; } = true;

	/// <summary>
	/// 当前的A3C协议的格式信息，可选格式1，2，3，4，默认格式1
	/// Format information of the current A3C protocol, optional format 1, 2, 3, 4, default format 1
	/// </summary>
	public int Format { get; set; } = 1;

	/// <summary>
	/// 是否开启支持写入位到字寄存器的功能，该功能先读取字寄存器的字数据，然后修改其中的位，再写入回去，可能存在脏数据的风险
	/// Whether to enable supporting the function of writing bit-to-word registers, which first reads the word data of the word register, then modifies the bits in it, and then writes back, there may be a risk of dirty data
	/// </summary>
	/// <remarks>
	/// 关于脏数据风险：从读取数据，修改位，再次写入数据时，大概需要经过3ms~10ms不等的时间，如果此期间内PLC修改了该字寄存器的其他位，再次写入数据时会恢复该点位的数据到读取时的初始值，可能引发设备故障，请谨慎开启此功能。
	/// About dirty data risk: from reading data, modifying bits, writing data again, it takes about 3ms ~ 10ms to pass a time, if the PLC modifies other bits of the word register during this period, when writing data again,
	/// it will restore the data at the point to the initial value when reading, which may cause equipment failure, please turn on this function carefully.
	/// </remarks>
	public bool EnableWriteBitToWordRegister { get; set; }

	/// <summary>
	/// 0表示 3C 协议，1表示4C协议
	/// </summary>
	public virtual int McMode { get; } = 0;

	/// <summary>
	/// 实例化默认的对象
	/// Instantiate the default object
	/// </summary>
	public MelsecA3CNet()
	{
		base.ByteTransform = new RegularByteTransform();
		base.WordLength = 1;
		LogMsgFormatBinary = false;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new MelsecA3CMessage
		{
			Format = Format,
			SumCheck = SumCheck
		};
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 批量读取PLC的数据，以字为单位，支持读取X,Y,M,S,D,T,C，具体的地址范围需要根据PLC型号来确认
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <param name="address">地址信息</param>
	/// <param name="length">数据长度</param>
	/// <returns>读取结果信息</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return MelsecA3CNetHelper.Read(this, address, length);
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 批量写入PLC的数据，以字为单位，也就是说最少2个字节信息，支持X,Y,M,S,D,T,C，具体的地址范围需要根据PLC型号来确认
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		return MelsecA3CNetHelper.Write(this, address, value);
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 批量读取bool类型数据，支持的类型为X,Y,S,T,C，具体的地址范围取决于PLC的类型
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <param name="address">地址信息，比如X10,Y17，注意X，Y的地址是8进制的</param>
	/// <param name="length">读取的长度</param>
	/// <returns>读取结果信息</returns>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		return MelsecA3CNetHelper.ReadBool(this, address, length);
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// 批量写入bool类型的数组，支持的类型为X,Y,S,T,C，具体的地址范围取决于PLC的类型
	/// Write arrays of type bool in batches. The supported types are X, Y, S, T, C. The specific address range depends on the type of PLC.
	/// </summary>
	/// <param name="address">PLC的地址信息</param>
	/// <param name="value">数据信息</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// 当需要写入D寄存器的位时，可以开启为True，然后地址使用 D100.2 等格式进行批量写入位操作，该操作有一定风险，参考属性。
	/// </remarks>
	public override OperateResult Write(string address, bool[] value)
	{
		return MelsecA3CNetHelper.Write(this, address, value);
	}

	[HslMqttApi]
	/// <summary>
	/// 远程Run操作
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <returns>是否成功</returns>
	public OperateResult RemoteRun()
	{
		return MelsecA3CNetHelper.RemoteRun(this);
	}

	[HslMqttApi]
	/// <summary>
	/// 远程Stop操作
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <returns>是否成功</returns>
	public OperateResult RemoteStop()
	{
		return MelsecA3CNetHelper.RemoteStop(this);
	}

	[HslMqttApi]
	/// <summary>
	/// 读取PLC的型号信息
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <returns>返回型号的结果对象</returns>
	public OperateResult<string> ReadPlcType()
	{
		return MelsecA3CNetHelper.ReadPlcType(this);
	}

	public override string ToString()
	{
		return $"MelsecA3CNet[{base.PortName}:{base.BaudRate}]";
	}
}
