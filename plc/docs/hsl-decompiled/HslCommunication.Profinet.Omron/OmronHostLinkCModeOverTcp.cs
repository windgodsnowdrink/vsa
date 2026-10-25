// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Profinet.Omron.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Omron;

/// <summary>
/// 欧姆龙的HostLink的C-Mode实现形式，当前的类是通过以太网透传实现。地址支持携带站号信息，例如：s=2;D100
/// The C-Mode implementation form of Omron’s HostLink, the current class is realized through Ethernet transparent transmission.
/// Address supports carrying station number information, for example: s=2;D100
/// </summary>
/// <remarks>
/// 暂时只支持的字数据的读写操作，不支持位的读写操作。另外本模式下，程序要在监视模式运行才能写数据，欧姆龙官方回复的。
/// </remarks>
public class OmronHostLinkCModeOverTcp : DeviceTcpNet, IHostLinkCMode, IReadWriteNet
{
	/// <summary>
	/// PLC设备的站号信息
	/// PLC device station number information
	/// </summary>
	public byte UnitNumber { get; set; }

	/// <summary>
	/// 实例化一个欧姆龙PLC Fins帧协议的通讯对象
	/// Instantiate a communication object of Omron PLC Fins frame protocol
	/// </summary>
	public OmronHostLinkCModeOverTcp()
	{
		base.ByteTransform = new RegularByteTransform(DataFormat.CDAB);
		base.WordLength = 1;
		base.ByteTransform.IsStringReverseByteWord = true;
		LogMsgFormatBinary = false;
	}

	/// <summary>
	/// Specify the IP address and port to instantiate a communication object for a OmronCipNet PLC protocol
	/// </summary>
	/// <param name="ipAddress">PLC IpAddress</param>
	/// <param name="port">PLC Port</param>
	public OmronHostLinkCModeOverTcp(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new SpecifiedCharacterMessage(13);
	}

	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		if (address.IndexOf(".") > 0)
		{
			return HslHelper.ReadBool(this, address, length, 16, reverseByWord: true);
		}
		return OmronHostLinkCModeHelper.ReadBool(this, UnitNumber, address, length);
	}

	[HslMqttApi("WriteBoolArray", "")]
	public override OperateResult Write(string address, bool[] value)
	{
		if (address.IndexOf(".") > 0)
		{
			return HslHelper.WriteBool(this, address, value, 16, reverseByWord: true);
		}
		return new OperateResult("The address is not a bit address, please use the Write(string, byte[]) method to write the word data");
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 从欧姆龙PLC中读取想要的数据，返回读取结果，读取长度的单位为字，地址格式为"D100","C100","W100","H100","A100"
	/// Read the desired data from the Omron PLC and return the read result. The unit of the read length is word. The address format is "D100", "C100", "W100", "H100", "A100"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">读取地址，格式为&quot;D100&quot;,&quot;C100&quot;,&quot;W100&quot;,&quot;H100&quot;,&quot;A100&quot;</param>
	/// <param name="length">读取的数据长度</param>
	/// <param name="splits">分割信息</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return OmronHostLinkCModeHelper.Read(this, UnitNumber, address, length);
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 向PLC写入数据，数据格式为原始的字节类型，地址格式为"D100","C100","W100","H100","A100"
	/// Write data to PLC, the data format is the original byte type, and the address format is "D100", "C100", "W100", "H100", "A100"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">初始地址</param>
	/// <param name="value">原始的字节数据</param>
	/// <returns>结果</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		return OmronHostLinkCModeHelper.Write(this, UnitNumber, address, value);
	}

	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		return await OmronHostLinkCModeHelper.ReadAsync(this, UnitNumber, address, length);
	}

	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await OmronHostLinkCModeHelper.WriteAsync(this, UnitNumber, address, value);
	}

	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		return await HslHelper.ReadBoolAsync(this, address, length, 16, reverseByWord: true);
	}

	public override async Task<OperateResult> WriteAsync(string address, bool[] value)
	{
		return await HslHelper.WriteBoolAsync(this, address, value, 16, reverseByWord: true);
	}

	[HslMqttApi("读取PLC的当前的型号信息")]
	/// <summary>
	/// [商业授权] 读取PLC的当前的型号信息
	/// [Authorization] Read the current model information of the PLC
	/// </summary>
	/// <param name="omron">PLC连接对象</param>
	/// <param name="unitNumber">站号信息</param>
	/// <returns>型号</returns>
	public OperateResult<string> ReadPlcType()
	{
		return ReadPlcType(UnitNumber);
	}

	/// <summary>
	/// [商业授权] 读取PLC的当前的型号信息
	/// [Authorization] Read the current model information of the PLC
	/// </summary>
	/// <param name="omron">PLC连接对象</param>
	/// <param name="unitNumber">站号信息</param>
	/// <returns>型号</returns>
	public OperateResult<string> ReadPlcType(byte unitNumber)
	{
		return OmronHostLinkCModeHelper.ReadPlcType(this, unitNumber);
	}

	[HslMqttApi("读取PLC当前的操作模式，0: 编程模式  1: 运行模式  2: 监视模式")]
	/// <summary>
	/// [商业授权] 读取PLC当前的操作模式，0: 编程模式  1: 运行模式  2: 监视模式
	/// [Authorization] Reads the Operation mode of the CPU Unit. 0: PROGRAM mode  1: RUN mode  2: MONITOR mode
	/// </summary>
	/// <param name="omron">PLC连接对象</param>
	/// <param name="unitNumber">站号信息</param>
	/// <returns>0: 编程模式  1: 运行模式  2: 监视模式</returns>
	public OperateResult<int> ReadPlcMode()
	{
		return ReadPlcMode(UnitNumber);
	}

	/// <summary>
	/// [商业授权] 读取PLC当前的操作模式，0: 编程模式  1: 运行模式  2: 监视模式
	/// [Authorization] Reads the Operation mode of the CPU Unit. 0: PROGRAM mode  1: RUN mode  2: MONITOR mode
	/// </summary>
	/// <param name="omron">PLC连接对象</param>
	/// <param name="unitNumber">站号信息</param>
	/// <returns>0: 编程模式  1: 运行模式  2: 监视模式</returns>
	public OperateResult<int> ReadPlcMode(byte unitNumber)
	{
		return OmronHostLinkCModeHelper.ReadPlcMode(this, unitNumber);
	}

	[HslMqttApi("将当前PLC的模式变更为指定的模式，0: 编程模式  1: 运行模式  2: 监视模式")]
	/// <summary>
	/// [商业授权] 将当前PLC的模式变更为指定的模式，0: 编程模式  1: 运行模式  2: 监视模式
	/// [Authorization] Change the current PLC mode to the specified mode, 0: programming mode 1: running mode 2: monitoring mode
	/// </summary>
	/// <param name="omron">PLC连接对象</param>
	/// <param name="unitNumber">站号信息</param>
	/// <param name="mode">0: 编程模式  1: 运行模式  2: 监视模式</param>
	/// <returns>是否变更成功</returns>
	public OperateResult ChangePlcMode(byte mode)
	{
		return ChangePlcMode(UnitNumber, mode);
	}

	/// <summary>
	/// [商业授权] 将当前PLC的模式变更为指定的模式，0: 编程模式  1: 运行模式  2: 监视模式
	/// [Authorization] Change the current PLC mode to the specified mode, 0: programming mode 1: running mode 2: monitoring mode
	/// </summary>
	/// <param name="omron">PLC连接对象</param>
	/// <param name="unitNumber">站号信息</param>
	/// <param name="mode">0: 编程模式  1: 运行模式  2: 监视模式</param>
	/// <returns>是否变更成功</returns>
	public OperateResult ChangePlcMode(byte unitNumber, byte mode)
	{
		return OmronHostLinkCModeHelper.ChangePlcMode(this, unitNumber, mode);
	}

	public override string ToString()
	{
		return $"OmronHostLinkCModeOverTcp[{IpAddress}:{Port}]";
	}
}
