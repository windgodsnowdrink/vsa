// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Instrument.DLT.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Instrument.DLT;

/// <summary>
/// 698.45协议的串口通信类，面向对象的用电信息数据交换协议，使用明文的通信方式。支持读取功率，总功，电压，电流，频率，功率因数等数据。
/// The serial communication class of the 698.45 protocol, an object-oriented power consumption information data exchange protocol,
/// uses the communication method of clear text. Support reading power, total power, voltage, current, frequency, power factor and other data.
/// </summary>
/// <remarks>
/// 如果不知道表的地址，可以使用方法来获取表的地址，读取的数据地址使用 OAD 的标识方式，具体可以参照api文档
/// If you don't know the address of the table, you can use the  method to get the address of the table,
/// and the read data address uses the OAD identification method. For details, please refer to the api documentation.
/// </remarks>
public class DLT698 : DeviceSerialPort, IDlt698, IReadWriteDevice, IReadWriteNet
{
	private string station = "1";

	/// <summary>
	/// 获取或设置是否使用安全的请求模式，对于有些仪表来说，不支持使用安全的模式，就需要设置为False。
	/// Get or set whether to use the secure request mode, for some meters, the safe mode is not supported, so you need to set it to False.
	/// </summary>
	public bool UseSecurityResquest { get; set; } = true;

	/// <summary>
	/// 客户机地址CA用1字节表示， 0表示不关注客户机地址。
	/// Client address CA is represented by 1 byte, and 0 means that the client address is not concerned.
	/// </summary>
	public byte CA { get; set; } = 0;

	/// <summary>
	/// 获取或设置当前的地址域信息，是一个12个字符的BCD码，例如：149100007290
	/// Get or set the current address domain information, which is a 12-character BCD code, for example: 149100007290
	/// </summary>
	public string Station
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
	/// 获取或设置是否在每一次的报文通信时，增加"FE FE FE FE"的命令头
	/// Get or set whether to add the command header of "FE FE FE FE" in each message communication
	/// </summary>
	public bool EnableCodeFE { get; set; }

	/// <summary>
	/// 默认的无参构造函数
	/// Default no-parameter constructor
	/// </summary>
	public DLT698()
	{
		base.ByteTransform = new ReverseBytesTransform();
		base.ReceiveEmptyDataCount = 20;
	}

	/// <summary>
	/// 指定地址域来实例化一个对象，密码及操作者代码在写入操作的时候进行验证
	/// Specify the address field to instantiate an object, and the password and operator code are validated during write operations,
	/// which address field is a 12-character BCD code, for example: 149100007290
	/// </summary>
	/// <param name="station">设备的地址信息，通常是一个12字符的BCD码</param>
	public DLT698(string station)
		: this()
	{
		this.station = station;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new DLT698Message();
	}

	public override byte[] PackCommandWithHeader(byte[] command)
	{
		return DLT698Helper.PackCommandWithHeader(this, command);
	}

	/// <summary>
	/// 根据传入的APDU的命令读取原始的字节数据返回，并检查返回的字节数据是否合法
	/// Read the original byte data return according to the incoming APDU command, and check whether the returned byte data is valid
	/// </summary>
	/// <param name="dlt">通信的DLT对象</param>
	/// <param name="apdu">apdu报文信息</param>
	/// <returns>原始字节数据信息</returns>
	public OperateResult<byte[]> ReadByApdu(byte[] apdu)
	{
		return DLT698Helper.ReadByApdu(this, apdu);
	}

	/// <summary>
	/// 激活设备的命令，只发送数据到设备，不等待设备数据返回
	/// The command to activate the device, only send data to the device, do not wait for the device data to return
	/// </summary>
	/// <returns>是否发送成功</returns>
	public OperateResult ActiveDeveice()
	{
		return DLT698Helper.ActiveDeveice(this);
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 根据指定的数据标识来读取相关的原始数据信息，地址标识根据手册来，从高位到地位，例如 00-00-00-00，分割符可以任意特殊字符或是没有分隔符。
	/// Read the relevant original data information according to the specified data identifier. The address identifier is based on the manual,
	/// from high to position, such as 00-00-00-00. The separator can be any special character or no separator.
	/// </summary>
	/// <param name="dlt">通信的DLT对象</param>
	/// <param name="address">数据标识，具体需要查找手册来对应</param>
	/// <param name="length">数据长度信息</param>
	/// <returns>结果信息</returns>
	/// <remarks>
	/// 地址可以携带地址域信息，例如 "s=2;00-00-00-00" 或是 "s=100000;00-00-02-00"，关于数据域信息，需要查找手册，例如:00-01-00-00 表示： (当前)正向有功总电能
	/// </remarks>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return DLT698Helper.Read(this, address, length);
	}

	/// <summary>
	/// 读取指定地址的所有的字符串数据信息，一般来说，一个地址只有一个数据，当属性为数组或是结构体的时候，存在多个数据，具体几个数据，需要根据
	/// </summary>
	/// <param name="dlt">通信的DLT对象</param>
	/// <param name="address">数据标识，具体需要查找手册来对应</param>
	/// <returns>字符串数组信息</returns>
	/// <remarks>
	/// 地址可以携带地址域信息，例如 "s=2;20-00-02-00" 或是 "s=100000;20-00-02-00"，
	/// </remarks>
	public OperateResult<string[]> ReadStringArray(string address)
	{
		return DLT698Helper.ReadStringArray(this, address);
	}

	/// <summary>
	/// 批量读取多个地址的数据信息，返回所有的数据对象信息
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="address">地址数组信息</param>
	/// <returns>字符串结果数据</returns>
	public OperateResult<string[]> ReadStringArray(string[] address)
	{
		return DLT698Helper.ReadStringArray(this, address);
	}

	private OperateResult<T[]> ReadDataAndParse<T>(string address, ushort length, Func<string, T> trans)
	{
		return DLT698Helper.ReadDataAndParse(ReadStringArray(address), length, trans);
	}

	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		return DLT698Helper.ReadBool(ReadStringArray(address), length);
	}

	[HslMqttApi("ReadInt16Array", "")]
	public override OperateResult<short[]> ReadInt16(string address, ushort length)
	{
		return ReadDataAndParse(address, length, short.Parse);
	}

	[HslMqttApi("ReadUInt16Array", "")]
	public override OperateResult<ushort[]> ReadUInt16(string address, ushort length)
	{
		return ReadDataAndParse(address, length, ushort.Parse);
	}

	[HslMqttApi("ReadInt32Array", "")]
	public override OperateResult<int[]> ReadInt32(string address, ushort length)
	{
		return ReadDataAndParse(address, length, int.Parse);
	}

	[HslMqttApi("ReadUInt32Array", "")]
	public override OperateResult<uint[]> ReadUInt32(string address, ushort length)
	{
		return ReadDataAndParse(address, length, uint.Parse);
	}

	[HslMqttApi("ReadInt64Array", "")]
	public override OperateResult<long[]> ReadInt64(string address, ushort length)
	{
		return ReadDataAndParse(address, length, long.Parse);
	}

	[HslMqttApi("ReadUInt64Array", "")]
	public override OperateResult<ulong[]> ReadUInt64(string address, ushort length)
	{
		return ReadDataAndParse(address, length, ulong.Parse);
	}

	[HslMqttApi("ReadFloatArray", "")]
	public override OperateResult<float[]> ReadFloat(string address, ushort length)
	{
		return ReadDataAndParse(address, length, float.Parse);
	}

	[HslMqttApi("ReadDoubleArray", "")]
	public override OperateResult<double[]> ReadDouble(string address, ushort length)
	{
		return ReadDataAndParse(address, length, double.Parse);
	}

	public override OperateResult<string> ReadString(string address, ushort length, Encoding encoding)
	{
		return ByteTransformHelper.GetResultFromArray(ReadStringArray(address));
	}

	/// <summary>
	/// 根据指定的数据标识来写入相关的原始数据信息，地址标识根据手册来，从高位到地位，例如 00-00-00-00，分割符可以任意特殊字符或是没有分隔符。
	/// Read the relevant original data information according to the specified data identifier. The address identifier is based on the manual,
	/// from high to position, such as 00-00-00-00. The separator can be any special character or no separator.
	/// </summary>
	/// <param name="dlt">通信的DLT对象</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">写入的数据值</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// 写入数据的时候，需要使用类型+值信息，例如写入地址 40-00-02-00  值为 1C 07 E0 01 14 10 1B 0B  表示 时间：2016-01-20 16：27：11
	/// </remarks>
	public override OperateResult Write(string address, byte[] value)
	{
		return DLT698Helper.Write(this, address, value);
	}

	/// <summary>
	/// 读取设备的通信地址，仅支持点对点通讯的情况，返回地址域数据，例如：149100007290
	/// Read the communication address of the device, only support point-to-point communication, and return the address field data, for example: 149100007290
	/// </summary>
	/// <param name="dlt">通信的DLT对象</param>
	/// <returns>设备的通信地址</returns>
	public OperateResult<string> ReadAddress()
	{
		return DLT698Helper.ReadAddress(this);
	}

	/// <summary>
	/// 写入设备的地址域信息，仅支持点对点通讯的情况，需要指定地址域信息，例如：149100007290
	/// Write the address domain information of the device, only support point-to-point communication,
	/// you need to specify the address domain information, for example: 149100007290
	/// </summary>
	/// <param name="dlt">通信的DLT对象</param>
	/// <param name="address">等待写入的地址域</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteAddress(string address)
	{
		return DLT698Helper.WriteAddress(this, address);
	}

	/// <summary>
	/// 写入设备的时间信息到指定的地址，返回是否成功，使用的时间类型为 0x1C, 有效数据为 年月日时分秒。
	/// Write the time information of the device to the specified address, return whether it is successful, the time type used is 0x1C, and the valid data is year, month, day, hour, minute, and second.
	/// </summary>
	/// <param name="dlt">通信的DLT对象</param>
	/// <param name="address">写入的地址的信息</param>
	/// <param name="time">时间数据</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteDateTime(string address, DateTime time)
	{
		return DLT698Helper.WriteDateTime(this, address, time);
	}

	public override async Task<OperateResult<short[]>> ReadInt16Async(string address, ushort length)
	{
		return await Task.Run(() => ReadInt16(address, length));
	}

	public override async Task<OperateResult<ushort[]>> ReadUInt16Async(string address, ushort length)
	{
		return await Task.Run(() => ReadUInt16(address, length));
	}

	public override async Task<OperateResult<int[]>> ReadInt32Async(string address, ushort length)
	{
		return await Task.Run(() => ReadInt32(address, length));
	}

	public override async Task<OperateResult<uint[]>> ReadUInt32Async(string address, ushort length)
	{
		return await Task.Run(() => ReadUInt32(address, length));
	}

	public override async Task<OperateResult<long[]>> ReadInt64Async(string address, ushort length)
	{
		return await Task.Run(() => ReadInt64(address, length));
	}

	public override async Task<OperateResult<ulong[]>> ReadUInt64Async(string address, ushort length)
	{
		return await Task.Run(() => ReadUInt64(address, length));
	}

	public override async Task<OperateResult<float[]>> ReadFloatAsync(string address, ushort length)
	{
		return await Task.Run(() => ReadFloat(address, length));
	}

	public override async Task<OperateResult<double[]>> ReadDoubleAsync(string address, ushort length)
	{
		return await Task.Run(() => ReadDouble(address, length));
	}

	public override async Task<OperateResult<string>> ReadStringAsync(string address, ushort length, Encoding encoding)
	{
		return await Task.Run(() => ReadString(address, length, encoding));
	}

	public override string ToString()
	{
		return $"DLT698[{base.PortName}:{base.BaudRate}]";
	}
}
