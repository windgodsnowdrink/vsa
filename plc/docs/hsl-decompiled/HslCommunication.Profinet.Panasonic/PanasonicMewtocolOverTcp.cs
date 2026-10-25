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
using HslCommunication.Profinet.Panasonic.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Panasonic;

/// <summary>
/// 松下PLC的数据交互协议，采用Mewtocol协议通讯，基于Tcp透传实现的机制，支持的地址列表参考api文档
/// The data exchange protocol of Panasonic PLC adopts Mewtocol protocol for communication.
/// It is based on the mechanism of Tcp transparent transmission. For the list of supported addresses, refer to the api document.
/// </summary>
/// <remarks>
/// 地址支持携带站号的访问方式，例如：s=2;D100
/// </remarks>
public class PanasonicMewtocolOverTcp : DeviceTcpNet
{
	/// <summary>
	/// PLC设备的目标站号，需要根据实际的设置来填写
	/// The target station number of the PLC device needs to be filled in according to the actual settings
	/// </summary>
	public byte Station { get; set; }

	/// <summary>
	/// 实例化一个默认的松下PLC通信对象，默认站号为0xEE
	/// Instantiate a default Panasonic PLC communication object, the default station number is 0xEE
	/// </summary>
	/// <param name="station">站号信息，默认为0xEE</param>
	public PanasonicMewtocolOverTcp(byte station = 238)
	{
		base.ByteTransform = new RegularByteTransform();
		Station = station;
		base.ByteTransform.DataFormat = DataFormat.DCBA;
		base.WordLength = 1;
		LogMsgFormatBinary = false;
	}

	/// <summary>
	/// 实例化一个默认的松下PLC通信对象，指定ip地址，端口，默认站号为0xEE
	/// Instantiate a default Panasonic PLC communication object, specify the IP address, port, and the default station number is 0xEE
	/// </summary>
	/// <param name="ipAddress">Ip地址数据</param>
	/// <param name="port">端口号</param>
	/// <param name="station">站号信息，默认为0xEE</param>
	public PanasonicMewtocolOverTcp(string ipAddress, int port, byte station = 238)
		: this(station)
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new SpecifiedCharacterMessage(13);
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 读取指定地址的原始数据，地址示例：D0  F0  K0  T0  C0, 地址支持携带站号的访问方式，例如：s=2;D100
	/// Read the original data of the specified address, address example: D0 F0 K0 T0 C0, the address supports carrying station number information, for example: s=2;D100
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">起始地址</param>
	/// <param name="length">长度</param>
	/// <returns>原始的字节数据的信息</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return MewtocolHelper.Read(this, Station, address, length);
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 将数据写入到指定的地址里去，地址示例：D0  F0  K0  T0  C0, 地址支持携带站号的访问方式，例如：s=2;D100
	/// Write data to the specified address, address example: D0 F0 K0 T0 C0, the address supports carrying station number information, for example: s=2;D100
	/// </summary>
	/// <param name="plc">PLC对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">起始地址</param>
	/// <param name="value">真实数据</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		return MewtocolHelper.Write(this, Station, address, value);
	}

	/// <summary>
	/// 读取指定地址的原始数据，地址示例：D0  F0  K0  T0  C0, 地址支持携带站号的访问方式，例如：s=2;D100
	/// Read the original data of the specified address, address example: D0 F0 K0 T0 C0, the address supports carrying station number information, for example: s=2;D100
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">起始地址</param>
	/// <param name="length">长度</param>
	/// <returns>原始的字节数据的信息</returns>
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		return await MewtocolHelper.ReadAsync(this, Station, address, length);
	}

	/// <summary>
	/// 将数据写入到指定的地址里去，地址示例：D0  F0  K0  T0  C0, 地址支持携带站号的访问方式，例如：s=2;D100
	/// Write data to the specified address, address example: D0 F0 K0 T0 C0, the address supports carrying station number information, for example: s=2;D100
	/// </summary>
	/// <param name="plc">PLC对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">起始地址</param>
	/// <param name="value">真实数据</param>
	/// <returns>是否写入成功</returns>
	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await MewtocolHelper.WriteAsync(this, Station, address, value);
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 批量读取松下PLC的位数据，按照字为单位，地址为 X0,X10,Y10，读取的长度为16的倍数
	/// Read the bit data of Panasonic PLC in batches, the unit is word, the address is X0, X10, Y10, and the read length is a multiple of 16
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">起始地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>读取结果对象</returns>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		return MewtocolHelper.ReadBool(this, Station, address, length);
	}

	[HslMqttApi("ReadBool", "")]
	/// <summary>
	/// 读取单个的地址信息的bool值，地址举例：SR0.0  X0.0  Y0.0  R0.0  L0.0
	/// Read the bool value of a single address, for example: SR0.0 X0.0 Y0.0 R0.0 L0.0
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">起始地址</param>
	/// <returns>读取结果对象</returns>
	public override OperateResult<bool> ReadBool(string address)
	{
		return MewtocolHelper.ReadBool(this, Station, address);
	}

	/// <summary>
	/// 批量读取松下PLC的位数据，传入一个读取的地址列表，地址支持X,Y,R,T,C,L, 举例：R1.0, X2.0, R3.A
	/// Batch read the bit data of Panasonic PLC, pass in a read address list, the address supports X, Y, R, T, C, L, for example: R1.0, X2.0, R3.A
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">等待读取的地址列表，数组长度不限制</param>
	/// <returns>读取结果对象</returns>
	public OperateResult<bool[]> ReadBool(string[] address)
	{
		return MewtocolHelper.ReadBool(this, Station, address);
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// 往指定的地址写入  数组，地址举例 X0.0  Y0.0  R0.0  L0.0，
	/// 起始的位地址必须为16的倍数，写入的  数组长度也为16的倍数。
	/// Write the  array to the specified address, address example: SR0.0 X0.0 Y0.0 R0.0 L0.0,
	/// the starting bit address must be a multiple of 16.  The length of the array is also a multiple of 16.
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">起始地址</param>
	/// <param name="values">数据值信息</param>
	/// <returns>返回是否成功的结果对象</returns>
	public override OperateResult Write(string address, bool[] values)
	{
		return MewtocolHelper.Write(this, Station, address, values);
	}

	[HslMqttApi("WriteBool", "")]
	/// <summary>
	/// 往指定的地址写入bool数据，地址举例：SR0.0  X0.0  Y0.0  R0.0  L0.0
	/// Write bool data to the specified address. Example address: SR0.0 X0.0 Y0.0 R0.0 L0.0
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">起始地址</param>
	/// <param name="value">数据值信息</param>
	/// <returns>返回是否成功的结果对象</returns>
	public override OperateResult Write(string address, bool value)
	{
		return MewtocolHelper.Write(this, Station, address, value);
	}

	/// <summary>
	/// 将Bool数组值写入到指定的离散地址里，一个地址对应一个bool值，地址数组长度和值数组长度必须相等，地址支持X,Y,R,T,C,L, 举例：R1.0, X2.0, R3.A
	/// Write the Bool array value to the specified discrete address, one address corresponds to one bool value,
	/// the length of the address array and the length of the value array must be equal, the address supports X, Y, R, T, C, L, for example: R1.0, X2.0, R3.A
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">离散的地址列表</param>
	/// <param name="value">bool数组值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public OperateResult Write(string[] address, bool[] value)
	{
		return MewtocolHelper.Write(this, Station, address, value);
	}

	/// <summary>
	/// 读取单个的地址信息的bool值，地址举例：SR0.0  X0.0  Y0.0  R0.0  L0.0
	/// Read the bool value of a single address, for example: SR0.0 X0.0 Y0.0 R0.0 L0.0
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">起始地址</param>
	/// <returns>读取结果对象</returns>
	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		return await MewtocolHelper.ReadBoolAsync(this, Station, address, length);
	}

	/// <summary>
	/// 读取单个的地址信息的bool值，地址举例：SR0.0  X0.0  Y0.0  R0.0  L0.0
	/// Read the bool value of a single address, for example: SR0.0 X0.0 Y0.0 R0.0 L0.0
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">起始地址</param>
	/// <returns>读取结果对象</returns>
	public override async Task<OperateResult<bool>> ReadBoolAsync(string address)
	{
		return await MewtocolHelper.ReadBoolAsync(this, Station, address);
	}

	/// <summary>
	/// 往指定的地址写入  数组，地址举例 X0.0  Y0.0  R0.0  L0.0，
	/// 起始的位地址必须为16的倍数，写入的  数组长度也为16的倍数。
	/// Write the  array to the specified address, address example: SR0.0 X0.0 Y0.0 R0.0 L0.0,
	/// the starting bit address must be a multiple of 16.  The length of the array is also a multiple of 16.
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">起始地址</param>
	/// <param name="values">数据值信息</param>
	/// <returns>返回是否成功的结果对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, bool[] values)
	{
		return await MewtocolHelper.WriteAsync(this, Station, address, values);
	}

	/// <summary>
	/// 往指定的地址写入bool数据，地址举例：SR0.0  X0.0  Y0.0  R0.0  L0.0
	/// Write bool data to the specified address. Example address: SR0.0 X0.0 Y0.0 R0.0 L0.0
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">起始地址</param>
	/// <param name="value">数据值信息</param>
	/// <returns>返回是否成功的结果对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, bool value)
	{
		return await MewtocolHelper.WriteAsync(this, Station, address, value);
	}

	/// <summary>
	/// 批量读取松下PLC的位数据，传入一个读取的地址列表，地址支持X,Y,R,T,C,L, 举例：R1.0, X2.0, R3.A
	/// Batch read the bit data of Panasonic PLC, pass in a read address list, the address supports X, Y, R, T, C, L, for example: R1.0, X2.0, R3.A
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">等待读取的地址列表，数组长度不限制</param>
	/// <returns>读取结果对象</returns>
	public async Task<OperateResult<bool[]>> ReadBoolAsync(string[] address)
	{
		return await MewtocolHelper.ReadBoolAsync(this, Station, address);
	}

	/// <summary>
	/// 将Bool数组值写入到指定的离散地址里，一个地址对应一个bool值，地址数组长度和值数组长度必须相等，地址支持X,Y,R,T,C,L, 举例：R1.0, X2.0, R3.A
	/// Write the Bool array value to the specified discrete address, one address corresponds to one bool value,
	/// the length of the address array and the length of the value array must be equal, the address supports X, Y, R, T, C, L, for example: R1.0, X2.0, R3.A
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">离散的地址列表</param>
	/// <param name="value">bool数组值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public async Task<OperateResult> WriteAsync(string[] address, bool[] value)
	{
		return await MewtocolHelper.WriteAsync(this, Station, address, value);
	}

	/// <summary>
	/// 读取PLC的型号信息
	/// Read the model information of the PLC
	/// </summary>
	/// <param name="plc">通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>PLC型号</returns>
	public OperateResult<string> ReadPlcType()
	{
		return MewtocolHelper.ReadPlcType(this, Station);
	}

	public override string ToString()
	{
		return $"PanasonicMewtocolOverTcp[{IpAddress}:{Port}]";
	}
}
