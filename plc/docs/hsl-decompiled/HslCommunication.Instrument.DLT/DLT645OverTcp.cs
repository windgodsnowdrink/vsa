// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Instrument.DLT.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Instrument.DLT;

/// <summary>
/// 基于多功能电能表通信协议实现的通讯类，参考的文档是DLT645-2007，主要实现了对电表数据的读取和一些功能方法，
/// 在点对点模式下，需要在连接后调用  方法，数据标识格式为 00-00-00-00，具体参照文档手册。
/// The communication type based on the communication protocol of the multifunctional electric energy meter.
/// The reference document is DLT645-2007, which mainly realizes the reading of the electric meter data and some functional methods.
/// In the point-to-point mode, you need to call  method after connect the device.
/// the data identification format is 00-00-00-00, refer to the documentation manual for details.
/// </summary>
/// <remarks>
/// 如果一对多的模式，地址可以携带地址域访问，例如 "s=2;00-00-00-00"，主要使用  方法来读取浮点数，
/// 方法来读取字符串
/// </remarks>
public class DLT645OverTcp : DeviceTcpNet, IDlt645, IReadWriteDevice, IReadWriteNet
{
	private string station = "1";

	private string password = "00000000";

	private string opCode = "00000000";

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
	/// 获取当前的DLT645的类型信息
	/// Gets the type information of the current DLT645
	/// </summary>
	public DLT645Type DLTType { get; } = DLT645Type.DLT2007;

	/// <summary>
	/// 获取或设置当前DLT645的密码，当进行写入数据操作的时候，需要正确的密码才能写入
	/// Obtain or set the password of the current DLT645, and the correct password is required to write data operations
	/// </summary>
	/// <remarks>
	/// 对于 DLT645/1997 协议来说无效
	/// </remarks>
	public string Password
	{
		get
		{
			return password;
		}
		set
		{
			password = value;
		}
	}

	/// <summary>
	/// 获取或设置当前DLT645的操作者代码，当进行写入数据操作的时候，需要指定正确的值
	/// Obtain or set the operator code of the current DLT645, and specify the correct value when writing data
	/// </summary>
	/// <remarks>
	/// 对于 DLT645/1997 协议来说无效
	/// </remarks>
	public string OpCode
	{
		get
		{
			return opCode;
		}
		set
		{
			opCode = value;
		}
	}

	/// <summary>
	/// 获取或设置当前的DLT645是否检查数据标识，默认为true
	/// Get or set whether the current DLT645 checks data identifiers, with the default being true
	/// </summary>
	public bool CheckDataId { get; set; } = true;

	/// <summary>
	/// 默认的无参构造函数
	/// Default no-parameter constructor
	/// </summary>
	public DLT645OverTcp()
	{
		base.ByteTransform = new RegularByteTransform();
		password = "00000000";
		opCode = "00000000";
		base.WordLength = 1;
	}

	/// <summary>
	/// 指定IP地址，端口，地址域，密码，操作者代码来实例化一个对象
	/// Specify the IP address, port, address field, password, and operator code to instantiate an object
	/// </summary>
	/// <param name="ipAddress">TcpServer的IP地址</param>
	/// <param name="port">TcpServer的端口</param>
	/// <param name="station">设备的站号信息</param>
	/// <param name="password">密码，写入的时候进行验证的信息</param>
	/// <param name="opCode">操作者代码</param>
	public DLT645OverTcp(string ipAddress, int port = 502, string station = "1", string password = "", string opCode = "")
	{
		IpAddress = ipAddress;
		Port = port;
		base.WordLength = 1;
		base.ByteTransform = new RegularByteTransform();
		this.station = station;
		this.password = (string.IsNullOrEmpty(password) ? "00000000" : password);
		this.opCode = (string.IsNullOrEmpty(opCode) ? "00000000" : opCode);
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new DLT645Message(CheckDataId);
	}

	public override byte[] PackCommandWithHeader(byte[] command)
	{
		if (EnableCodeFE)
		{
			return SoftBasic.SpliceArray<byte>(new byte[4] { 254, 254, 254, 254 }, command);
		}
		return base.PackCommandWithHeader(command);
	}

	/// <summary>
	/// 激活设备的命令，只发送数据到设备，不等待设备数据返回
	/// The command to activate the device, only send data to the device, do not wait for the device data to return
	/// </summary>
	/// <returns>是否发送成功</returns>
	public OperateResult ActiveDeveice()
	{
		return ReadFromCoreServer(new byte[4] { 254, 254, 254, 254 }, hasResponseData: false, usePackAndUnpack: true);
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 根据指定的数据标识来读取相关的原始数据信息，地址标识根据手册来，从高位到地位，例如 00-00-00-00，分割符可以任意特殊字符或是没有分隔符。
	/// Read the relevant original data information according to the specified data identifier. The address identifier is based on the manual,
	/// from high to position, such as 00-00-00-00. The separator can be any special character or no separator.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="address">数据标识，具体需要查找手册来对应</param>
	/// <param name="length">数据长度信息</param>
	/// <returns>结果信息</returns>
	/// <remarks>
	/// 地址可以携带地址域信息，例如 "s=2;00-00-00-00" 或是 "s=100000;00-00-02-00"，关于数据域信息，需要查找手册，例如:00-01-00-00 表示： (当前)正向有功总电能
	/// </remarks>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return DLT645Helper.Read(this, address, length);
	}

	[HslMqttApi("ReadDoubleArray", "")]
	public override OperateResult<double[]> ReadDouble(string address, ushort length)
	{
		return DLT645Helper.ReadDouble(this, address, length);
	}

	public override OperateResult<string> ReadString(string address, ushort length, Encoding encoding)
	{
		return ByteTransformHelper.GetResultFromArray(ReadStringArray(address));
	}

	/// <summary>
	/// 读取指定地址的所有的字符串数据信息，一般来说，一个地址只有一个数据，但是少部分的地址存在多个数据，例如 01-01-00-00 正向有功总需求及发生时间
	/// Read all the string data information of the specified address, in general, there is only one data for one address, but there are multiple data for a small number of addresses,
	/// such as 01-01-00-00 Forward active total demand and occurrence time
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="address">数据标识，具体需要查找手册来对应</param>
	/// <returns>字符串数组信息</returns>
	/// <remarks>
	/// 地址可以携带地址域信息，例如 "s=2;00-00-00-00" 或是 "s=100000;00-00-02-00"，关于数据域信息，需要查找手册，例如:00-01-00-00 表示： (当前)正向有功总电能
	/// 地址也可以携带是否数据翻转的标记，例如 "reverse=false;00-00-00-00" 解析数据的时候就不发生反转的操作
	/// </remarks>
	public OperateResult<string[]> ReadStringArray(string address)
	{
		return DLT645Helper.ReadStringArray(this, address);
	}

	/// <summary>
	/// 跳闸功能，需要指定有效截止时间，如果有需要可以指定其他的站号信息
	/// </summary>
	/// <param name="station">站号信息，如果传入空，将使用本对象的站号</param>
	/// <param name="validTime">有效截止时间</param>
	/// <returns>是否操作成功</returns>
	public OperateResult Trip(DateTime validTime)
	{
		return Trip(Station, validTime);
	}

	/// <summary>
	/// 跳闸功能，需要指定有效截止时间，如果有需要可以指定其他的站号信息
	/// </summary>
	/// <param name="station">站号信息，如果传入空，将使用本对象的站号</param>
	/// <param name="validTime">有效截止时间</param>
	/// <returns>是否操作成功</returns>
	public OperateResult Trip(string station, DateTime validTime)
	{
		return DLT645Helper.Function1C(this, password, opCode, station, 26, validTime);
	}

	/// <summary>
	/// 合闸允许功能，需要指定有效截止时间，如果有需要可以指定其他的站号信息
	/// </summary>
	/// <param name="station">站号信息，如果传入空，将使用本对象的站号</param>
	/// <param name="validTime">有效截止时间</param>
	/// <returns>是否操作成功</returns>
	public OperateResult SwitchingOn(DateTime validTime)
	{
		return SwitchingOn(Station, validTime);
	}

	/// <summary>
	/// 合闸允许功能，需要指定有效截止时间，如果有需要可以指定其他的站号信息
	/// </summary>
	/// <param name="station">站号信息，如果传入空，将使用本对象的站号</param>
	/// <param name="validTime">有效截止时间</param>
	/// <returns>是否操作成功</returns>
	public OperateResult SwitchingOn(string station, DateTime validTime)
	{
		return DLT645Helper.Function1C(this, password, opCode, station, 27, validTime);
	}

	/// <summary>
	/// 激活设备的命令，只发送数据到设备，不等待设备数据返回
	/// The command to activate the device, only send data to the device, do not wait for the device data to return
	/// </summary>
	/// <returns>是否发送成功</returns>
	public async Task<OperateResult> ActiveDeveiceAsync()
	{
		return await ReadFromCoreServerAsync(new byte[4] { 254, 254, 254, 254 }, hasResponseData: false, usePackAndUnpack: false);
	}

	/// <summary>
	/// 根据指定的数据标识来读取相关的原始数据信息，地址标识根据手册来，从高位到地位，例如 00-00-00-00，分割符可以任意特殊字符或是没有分隔符。
	/// Read the relevant original data information according to the specified data identifier. The address identifier is based on the manual,
	/// from high to position, such as 00-00-00-00. The separator can be any special character or no separator.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="address">数据标识，具体需要查找手册来对应</param>
	/// <param name="length">数据长度信息</param>
	/// <returns>结果信息</returns>
	/// <remarks>
	/// 地址可以携带地址域信息，例如 "s=2;00-00-00-00" 或是 "s=100000;00-00-02-00"，关于数据域信息，需要查找手册，例如:00-01-00-00 表示： (当前)正向有功总电能
	/// </remarks>
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		return await DLT645Helper.ReadAsync(this, address, length);
	}

	public override async Task<OperateResult<double[]>> ReadDoubleAsync(string address, ushort length)
	{
		return await DLT645Helper.ReadDoubleAsync(this, address, length);
	}

	public override async Task<OperateResult<string>> ReadStringAsync(string address, ushort length, Encoding encoding)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadStringArrayAsync(address));
	}

	/// <summary>
	/// 读取指定地址的所有的字符串数据信息，一般来说，一个地址只有一个数据，但是少部分的地址存在多个数据，例如 01-01-00-00 正向有功总需求及发生时间
	/// Read all the string data information of the specified address, in general, there is only one data for one address, but there are multiple data for a small number of addresses,
	/// such as 01-01-00-00 Forward active total demand and occurrence time
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="address">数据标识，具体需要查找手册来对应</param>
	/// <returns>字符串数组信息</returns>
	/// <remarks>
	/// 地址可以携带地址域信息，例如 "s=2;00-00-00-00" 或是 "s=100000;00-00-02-00"，关于数据域信息，需要查找手册，例如:00-01-00-00 表示： (当前)正向有功总电能
	/// 地址也可以携带是否数据翻转的标记，例如 "reverse=false;00-00-00-00" 解析数据的时候就不发生反转的操作
	/// </remarks>
	public async Task<OperateResult<string[]>> ReadStringArrayAsync(string address)
	{
		return await DLT645Helper.ReadStringArrayAsync(this, address);
	}

	/// <summary>
	/// 根据指定的数据标识来写入相关的原始数据信息，地址标识根据手册来，从高位到地位，例如 00-00-00-00，分割符可以任意特殊字符或是没有分隔符。
	/// Read the relevant original data information according to the specified data identifier. The address identifier is based on the manual,
	/// from high to position, such as 00-00-00-00. The separator can be any special character or no separator.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="password">密钥信息</param>
	/// <param name="opCode">操作者代码</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">写入的数据值</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// 地址可以携带地址域信息，例如 "s=2;00-00-00-00" 或是 "s=100000;00-00-02-00"，关于数据域信息，需要查找手册，例如:00-01-00-00 表示： (当前)正向有功总电能
	/// 注意：本命令必须与编程键配合使用
	/// </remarks>
	public override OperateResult Write(string address, byte[] value)
	{
		return DLT645Helper.Write(this, password, opCode, address, value);
	}

	/// <summary>
	/// 将指定的数据写入到仪表中，，地址标识根据手册来，从高位到地位，例如 00-00-00-00，分割符可以任意特殊字符或是没有分隔符。
	/// Write the data to the gauge, address identification according to the manual, from high bit to position,
	/// such as 00-00-00-00, the separator can be any special character or no delimiter.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="password">密钥信息</param>
	/// <param name="opCode">操作者代码</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">写入的数据值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public override OperateResult Write(string address, short[] values)
	{
		return DLT645Helper.Write(this, password, opCode, address, values.Select((short m) => m.ToString()).ToArray());
	}

	/// <summary>
	/// 将指定的数据写入到仪表中，，地址标识根据手册来，从高位到地位，例如 00-00-00-00，分割符可以任意特殊字符或是没有分隔符。
	/// Write the data to the gauge, address identification according to the manual, from high bit to position,
	/// such as 00-00-00-00, the separator can be any special character or no delimiter.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="password">密钥信息</param>
	/// <param name="opCode">操作者代码</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">写入的数据值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public override OperateResult Write(string address, ushort[] values)
	{
		return DLT645Helper.Write(this, password, opCode, address, values.Select((ushort m) => m.ToString()).ToArray());
	}

	/// <summary>
	/// 将指定的数据写入到仪表中，，地址标识根据手册来，从高位到地位，例如 00-00-00-00，分割符可以任意特殊字符或是没有分隔符。
	/// Write the data to the gauge, address identification according to the manual, from high bit to position,
	/// such as 00-00-00-00, the separator can be any special character or no delimiter.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="password">密钥信息</param>
	/// <param name="opCode">操作者代码</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">写入的数据值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public override OperateResult Write(string address, int[] values)
	{
		return DLT645Helper.Write(this, password, opCode, address, values.Select((int m) => m.ToString()).ToArray());
	}

	/// <summary>
	/// 将指定的数据写入到仪表中，，地址标识根据手册来，从高位到地位，例如 00-00-00-00，分割符可以任意特殊字符或是没有分隔符。
	/// Write the data to the gauge, address identification according to the manual, from high bit to position,
	/// such as 00-00-00-00, the separator can be any special character or no delimiter.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="password">密钥信息</param>
	/// <param name="opCode">操作者代码</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">写入的数据值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public override OperateResult Write(string address, uint[] values)
	{
		return DLT645Helper.Write(this, password, opCode, address, values.Select((uint m) => m.ToString()).ToArray());
	}

	/// <summary>
	/// 将指定的数据写入到仪表中，，地址标识根据手册来，从高位到地位，例如 00-00-00-00，分割符可以任意特殊字符或是没有分隔符。
	/// Write the data to the gauge, address identification according to the manual, from high bit to position,
	/// such as 00-00-00-00, the separator can be any special character or no delimiter.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="password">密钥信息</param>
	/// <param name="opCode">操作者代码</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">写入的数据值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public override OperateResult Write(string address, float[] values)
	{
		return DLT645Helper.Write(this, password, opCode, address, values.Select((float m) => m.ToString()).ToArray());
	}

	/// <summary>
	/// 将指定的数据写入到仪表中，，地址标识根据手册来，从高位到地位，例如 00-00-00-00，分割符可以任意特殊字符或是没有分隔符。
	/// Write the data to the gauge, address identification according to the manual, from high bit to position,
	/// such as 00-00-00-00, the separator can be any special character or no delimiter.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="password">密钥信息</param>
	/// <param name="opCode">操作者代码</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">写入的数据值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public override OperateResult Write(string address, double[] values)
	{
		return DLT645Helper.Write(this, password, opCode, address, values.Select((double m) => m.ToString()).ToArray());
	}

	public override OperateResult Write(string address, string value, Encoding encoding)
	{
		return DLT645Helper.Write(this, password, opCode, address, new string[1] { value });
	}

	/// <summary>
	/// 读取设备的通信地址，仅支持点对点通讯的情况，返回地址域数据，例如：149100007290
	/// Read the communication address of the device, only support point-to-point communication, and return the address field data, for example: 149100007290
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <returns>设备的通信地址</returns>
	public OperateResult<string> ReadAddress()
	{
		return DLT645Helper.ReadAddress(this);
	}

	/// <summary>
	/// 写入设备的地址域信息，仅支持点对点通讯的情况，需要指定地址域信息，例如：149100007290
	/// Write the address domain information of the device, only support point-to-point communication,
	/// you need to specify the address domain information, for example: 149100007290
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="address">等待写入的地址域</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteAddress(string address)
	{
		return DLT645Helper.WriteAddress(this, address);
	}

	/// <summary>
	/// 广播指定的时间，强制从站与主站时间同步，传入时间对象，没有数据返回。
	/// Broadcast the specified time, force the slave station to synchronize with the master station time,
	/// pass in the  time object, and no data will be returned.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="dateTime">时间对象</param>
	/// <returns>是否成功</returns>
	public OperateResult BroadcastTime(DateTime dateTime)
	{
		return DLT645Helper.BroadcastTime(this, dateTime);
	}

	/// <summary>
	/// 对设备发送冻结命令，默认点对点操作，地址域为 99999999999999 时为广播，数据域格式说明：MMDDhhmm(月日时分)，
	/// 99DDhhmm表示月为周期定时冻结，9999hhmm表示日为周期定时冻结，999999mm表示以小时为周期定时冻结，99999999表示瞬时冻结
	/// Send a freeze command to the device, the default point-to-point operation, when the address field is 9999999999999,
	/// it is broadcast, and the data field format description: MMDDhhmm (month, day, hour and minute),
	/// 99DDhhmm means the month is the periodic fixed freeze, 9999hhmm means the day is the periodic periodic freeze,
	/// and 999999mm means the hour It is periodic timed freezing, 99999999 means instantaneous freezing
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="dataArea">数据域信息</param>
	/// <returns>是否成功冻结</returns>
	public OperateResult FreezeCommand(string dataArea)
	{
		return DLT645Helper.FreezeCommand(this, dataArea);
	}

	/// <summary>
	/// 更改通信速率，波特率可选 600,1200,2400,4800,9600,19200，其他值无效，可以携带地址域信息，s=1;9600
	/// Change the communication rate, the baud rate can be 600, 1200, 2400, 4800, 9600, 19200,
	/// other values are invalid, you can carry address domain information, s=1;9600
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="baudRate">波特率的信息</param>
	/// <returns>是否更改成功</returns>
	/// <remarks>
	/// 对于DLT1997来说，只支持 300, 600, 2400, 4800, 9600
	/// </remarks>
	public OperateResult ChangeBaudRate(string baudRate)
	{
		return DLT645Helper.ChangeBaudRate(this, baudRate);
	}

	/// <summary>
	/// 根据指定的数据标识来写入相关的原始数据信息，地址标识根据手册来，从高位到地位，例如 00-00-00-00，分割符可以任意特殊字符或是没有分隔符。
	/// Read the relevant original data information according to the specified data identifier. The address identifier is based on the manual,
	/// from high to position, such as 00-00-00-00. The separator can be any special character or no separator.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="password">密钥信息</param>
	/// <param name="opCode">操作者代码</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">写入的数据值</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// 地址可以携带地址域信息，例如 "s=2;00-00-00-00" 或是 "s=100000;00-00-02-00"，关于数据域信息，需要查找手册，例如:00-01-00-00 表示： (当前)正向有功总电能
	/// 注意：本命令必须与编程键配合使用
	/// </remarks>
	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await DLT645Helper.WriteAsync(this, password, opCode, address, value);
	}

	/// <summary>
	/// 将指定的数据写入到仪表中，，地址标识根据手册来，从高位到地位，例如 00-00-00-00，分割符可以任意特殊字符或是没有分隔符。
	/// Write the data to the gauge, address identification according to the manual, from high bit to position,
	/// such as 00-00-00-00, the separator can be any special character or no delimiter.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="password">密钥信息</param>
	/// <param name="opCode">操作者代码</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">写入的数据值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, short[] values)
	{
		return await DLT645Helper.WriteAsync(this, password, opCode, address, values.Select((short m) => m.ToString()).ToArray());
	}

	/// <summary>
	/// 将指定的数据写入到仪表中，，地址标识根据手册来，从高位到地位，例如 00-00-00-00，分割符可以任意特殊字符或是没有分隔符。
	/// Write the data to the gauge, address identification according to the manual, from high bit to position,
	/// such as 00-00-00-00, the separator can be any special character or no delimiter.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="password">密钥信息</param>
	/// <param name="opCode">操作者代码</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">写入的数据值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, ushort[] values)
	{
		return await DLT645Helper.WriteAsync(this, password, opCode, address, values.Select((ushort m) => m.ToString()).ToArray());
	}

	/// <summary>
	/// 将指定的数据写入到仪表中，，地址标识根据手册来，从高位到地位，例如 00-00-00-00，分割符可以任意特殊字符或是没有分隔符。
	/// Write the data to the gauge, address identification according to the manual, from high bit to position,
	/// such as 00-00-00-00, the separator can be any special character or no delimiter.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="password">密钥信息</param>
	/// <param name="opCode">操作者代码</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">写入的数据值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, int[] values)
	{
		return await DLT645Helper.WriteAsync(this, password, opCode, address, values.Select((int m) => m.ToString()).ToArray());
	}

	/// <summary>
	/// 将指定的数据写入到仪表中，，地址标识根据手册来，从高位到地位，例如 00-00-00-00，分割符可以任意特殊字符或是没有分隔符。
	/// Write the data to the gauge, address identification according to the manual, from high bit to position,
	/// such as 00-00-00-00, the separator can be any special character or no delimiter.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="password">密钥信息</param>
	/// <param name="opCode">操作者代码</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">写入的数据值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, uint[] values)
	{
		return await DLT645Helper.WriteAsync(this, password, opCode, address, values.Select((uint m) => m.ToString()).ToArray());
	}

	/// <summary>
	/// 将指定的数据写入到仪表中，，地址标识根据手册来，从高位到地位，例如 00-00-00-00，分割符可以任意特殊字符或是没有分隔符。
	/// Write the data to the gauge, address identification according to the manual, from high bit to position,
	/// such as 00-00-00-00, the separator can be any special character or no delimiter.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="password">密钥信息</param>
	/// <param name="opCode">操作者代码</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">写入的数据值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, float[] values)
	{
		return await DLT645Helper.WriteAsync(this, password, opCode, address, values.Select((float m) => m.ToString()).ToArray());
	}

	/// <summary>
	/// 将指定的数据写入到仪表中，，地址标识根据手册来，从高位到地位，例如 00-00-00-00，分割符可以任意特殊字符或是没有分隔符。
	/// Write the data to the gauge, address identification according to the manual, from high bit to position,
	/// such as 00-00-00-00, the separator can be any special character or no delimiter.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="password">密钥信息</param>
	/// <param name="opCode">操作者代码</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">写入的数据值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, double[] values)
	{
		return await DLT645Helper.WriteAsync(this, password, opCode, address, values.Select((double m) => m.ToString()).ToArray());
	}

	public override async Task<OperateResult> WriteAsync(string address, string value, Encoding encoding)
	{
		return await DLT645Helper.WriteAsync(this, password, opCode, address, new string[1] { value });
	}

	/// <summary>
	/// 读取设备的通信地址，仅支持点对点通讯的情况，返回地址域数据，例如：149100007290
	/// Read the communication address of the device, only support point-to-point communication, and return the address field data, for example: 149100007290
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <returns>设备的通信地址</returns>
	public async Task<OperateResult<string>> ReadAddressAsync()
	{
		return await DLT645Helper.ReadAddressAsync(this);
	}

	/// <summary>
	/// 写入设备的地址域信息，仅支持点对点通讯的情况，需要指定地址域信息，例如：149100007290
	/// Write the address domain information of the device, only support point-to-point communication,
	/// you need to specify the address domain information, for example: 149100007290
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="address">等待写入的地址域</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteAddressAsync(string address)
	{
		return await DLT645Helper.WriteAddressAsync(this, address);
	}

	/// <summary>
	/// 广播指定的时间，强制从站与主站时间同步，传入时间对象，没有数据返回。
	/// Broadcast the specified time, force the slave station to synchronize with the master station time,
	/// pass in the  time object, and no data will be returned.
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="dateTime">时间对象</param>
	/// <returns>是否成功</returns>
	public async Task<OperateResult> BroadcastTimeAsync(DateTime dateTime)
	{
		return await DLT645Helper.BroadcastTimeAsync(this, dateTime, ReadFromCoreServerAsync);
	}

	/// <summary>
	/// 对设备发送冻结命令，默认点对点操作，地址域为 99999999999999 时为广播，数据域格式说明：MMDDhhmm(月日时分)，
	/// 99DDhhmm表示月为周期定时冻结，9999hhmm表示日为周期定时冻结，999999mm表示以小时为周期定时冻结，99999999表示瞬时冻结
	/// Send a freeze command to the device, the default point-to-point operation, when the address field is 9999999999999,
	/// it is broadcast, and the data field format description: MMDDhhmm (month, day, hour and minute),
	/// 99DDhhmm means the month is the periodic fixed freeze, 9999hhmm means the day is the periodic periodic freeze,
	/// and 999999mm means the hour It is periodic timed freezing, 99999999 means instantaneous freezing
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="dataArea">数据域信息</param>
	/// <returns>是否成功冻结</returns>
	public async Task<OperateResult> FreezeCommandAsync(string dataArea)
	{
		return await DLT645Helper.FreezeCommandAsync(this, dataArea);
	}

	/// <summary>
	/// 更改通信速率，波特率可选 600,1200,2400,4800,9600,19200，其他值无效，可以携带地址域信息，s=1;9600
	/// Change the communication rate, the baud rate can be 600, 1200, 2400, 4800, 9600, 19200,
	/// other values are invalid, you can carry address domain information, s=1;9600
	/// </summary>
	/// <param name="dlt">DLT通信对象</param>
	/// <param name="baudRate">波特率的信息</param>
	/// <returns>是否更改成功</returns>
	/// <remarks>
	/// 对于DLT1997来说，只支持 300, 600, 2400, 4800, 9600
	/// </remarks>
	public async Task<OperateResult> ChangeBaudRateAsync(string baudRate)
	{
		return await DLT645Helper.ChangeBaudRateAsync(this, baudRate);
	}

	public override string ToString()
	{
		return $"DLT645OverTcp[{IpAddress}:{Port}]";
	}
}
