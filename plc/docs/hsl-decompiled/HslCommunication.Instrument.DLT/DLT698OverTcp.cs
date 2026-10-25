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
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Pipe;
using HslCommunication.Instrument.DLT.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Instrument.DLT;

/// <summary>
/// 698.45协议的串口转网口透传通信类(不是TCP通信)，面向对象的用电信息数据交换协议，使用明文的通信方式。支持读取功率，总功，电压，电流，频率，功率因数等数据。
/// 698.45 protocol serial port to network port transparent transmission communication (not TCP communication),
/// object-oriented power consumption information data exchange protocol, using plaintext communication. Support reading power,
/// total power, voltage, current, frequency, power factor and other data.
/// </summary>
public class DLT698OverTcp : DeviceTcpNet, IDlt698, IReadWriteDevice, IReadWriteNet
{
	public delegate void DataReportingDelegate(CommunicationPipe pipe, OperateResult<byte[]> receive);

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
	/// 获取或设置是否启用服务器主动推送功能，默认为不启用，如果某些设备支持主动推送功能，可以设置为True，然后在事件中接收数据。
	/// Gets or sets whether active push is enabled on the server. By default, it is disabled. If some devices support active push, it can be set to True and then receive data in the event.
	/// </summary>
	public bool IsServerActivePush { get; set; } = false;

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
	/// 数据上报的事件，当设置了设置为 True 时才会生效。
	/// The data report event takes effect only when  is set to True.
	/// </summary>
	public event DataReportingDelegate OnDataReporting;

	/// <summary>
	/// 默认的无参构造函数
	/// Default no-parameter constructor
	/// </summary>
	public DLT698OverTcp()
	{
		base.ByteTransform = new ReverseBytesTransform();
	}

	/// <summary>
	/// 通过指定设备站号来初始化一个通信对象信息
	/// </summary>
	/// <param name="station">设备的地址信息，通常是一个12字符的BCD码</param>
	public DLT698OverTcp(string station)
		: this()
	{
		this.station = station;
	}

	/// <summary>
	/// 通过指定IP地址，端口号，设备站号来初始化一个通信对象信息
	/// </summary>
	/// <param name="ipAddress">IP地址信息</param>
	/// <param name="port">端口号信息</param>
	/// <param name="station">设备站号信息</param>
	public DLT698OverTcp(string ipAddress, int port, string station)
		: this(station)
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new DLT698Message();
	}

	public override byte[] PackCommandWithHeader(byte[] command)
	{
		return DLT698Helper.PackCommandWithHeader(this, command);
	}

	protected override OperateResult InitializationOnConnect()
	{
		if (IsServerActivePush)
		{
			CommunicationPipe.UseServerActivePush = true;
		}
		return base.InitializationOnConnect();
	}

	protected override async Task<OperateResult> InitializationOnConnectAsync()
	{
		if (IsServerActivePush)
		{
			CommunicationPipe.UseServerActivePush = true;
		}
		return await base.InitializationOnConnectAsync();
	}

	protected override bool DecideWhetherQAMessage(CommunicationPipe pipe, OperateResult<byte[]> receive)
	{
		if (IsServerActivePush)
		{
			if (!receive.IsSuccess)
			{
				return base.DecideWhetherQAMessage(pipe, receive);
			}
			try
			{
				byte[] content = receive.Content;
				string text = Enumerable.Reverse(content.SelectMiddle(5, (content[4] & 0xF) + 1)).ToArray().ToHexString();
				int num = 5 + (content[4] & 0xF) + 1 + 3;
				byte ca = 16;
				if (content[3] == 129)
				{
					if (content[num] == 1 && (content[num + 2] == 0 || content[num + 2] == 1))
					{
						LogRevcMessage(receive.Content);
						byte[] content2 = DLT698Helper.BuildEntireCommand(1, Station, ca, DLT698Helper.CreatePreLogin(content.SelectMiddle(num + 5, 10))).Content;
						LogSendMessage(content2);
						pipe.Send(content2);
						return false;
					}
				}
				else if (content[3] == 67)
				{
					byte[] array = content.RemoveDouble(num, 2);
					if (array[0] == 16)
					{
						array = array.SelectMiddle(3, array[2]);
					}
					if (array[0] == 136)
					{
						LogRevcMessage(receive.Content);
						OnDataReporting?.Invoke(pipe, receive);
						return false;
					}
				}
			}
			catch
			{
				return base.DecideWhetherQAMessage(pipe, receive);
			}
		}
		return base.DecideWhetherQAMessage(pipe, receive);
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

	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		return DLT698Helper.ReadBool(ReadStringArray(address), length);
	}

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
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		OperateResult<byte[]> command = DLT698Helper.BuildReadSingleObject(address, station, this);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		return DLT698Helper.CheckResponse(read.Content);
	}

	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		return DLT698Helper.ReadBool(await ReadStringArrayAsync(address), length);
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
	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		OperateResult<byte[]> build = DLT698Helper.BuildWriteSingleObject(address, station, value, this);
		if (!build.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(build);
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(build.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		return DLT698Helper.CheckResponse(read.Content);
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

	private async Task<OperateResult<T[]>> ReadDataAndParseAsync<T>(string address, ushort length, Func<string, T> trans)
	{
		return DLT698Helper.ReadDataAndParse(await ReadStringArrayAsync(address), length, trans);
	}

	/// <summary>
	/// 根据传入的APDU的命令读取原始的字节数据返回，并检查返回的字节数据是否合法
	/// Read the original byte data return according to the incoming APDU command, and check whether the returned byte data is valid
	/// </summary>
	/// <param name="dlt">通信的DLT对象</param>
	/// <param name="apdu">apdu报文信息</param>
	/// <returns>原始字节数据信息</returns>
	public async Task<OperateResult<byte[]>> ReadByApduAsync(byte[] apdu)
	{
		return await DLT698Helper.ReadByApduAsync(this, apdu);
	}

	/// <summary>
	/// 激活设备的命令，只发送数据到设备，不等待设备数据返回
	/// The command to activate the device, only send data to the device, do not wait for the device data to return
	/// </summary>
	/// <returns>是否发送成功</returns>
	public async Task<OperateResult> ActiveDeveiceAsync()
	{
		return await ReadFromCoreServerAsync(new byte[4] { 254, 254, 254, 254 }, hasResponseData: false, usePackAndUnpack: true);
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
	public async Task<OperateResult<string[]>> ReadStringArrayAsync(string address)
	{
		OperateResult<byte[]> read = await ReadAsync(address, 1).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string[]>(read);
		}
		int index = 8;
		return OperateResult.CreateSuccessResult(DLT698Helper.ExtraStringsValues(base.ByteTransform, read.Content, ref index));
	}

	/// <summary>
	/// 读取设备的通信地址，仅支持点对点通讯的情况，返回地址域数据，例如：149100007290
	/// Read the communication address of the device, only support point-to-point communication, and return the address field data, for example: 149100007290
	/// </summary>
	/// <param name="dlt">通信的DLT对象</param>
	/// <returns>设备的通信地址</returns>
	public async Task<OperateResult<string>> ReadAddressAsync()
	{
		OperateResult<byte[]> build = DLT698Helper.BuildReadSingleObject("40-01-02-00", "AAAAAAAAAAAA", this);
		if (!build.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(build);
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(build.Content);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		OperateResult<byte[]> extra = DLT698Helper.CheckResponse(read.Content);
		if (!extra.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(extra);
		}
		station = extra.Content.SelectMiddle(10, extra.Content[9]).ToHexString();
		return OperateResult.CreateSuccessResult(station);
	}

	/// <summary>
	/// 写入设备的地址域信息，仅支持点对点通讯的情况，需要指定地址域信息，例如：149100007290
	/// Write the address domain information of the device, only support point-to-point communication,
	/// you need to specify the address domain information, for example: 149100007290
	/// </summary>
	/// <param name="dlt">通信的DLT对象</param>
	/// <param name="address">等待写入的地址域</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteAddressAsync(string address)
	{
		OperateResult<byte[]> build = DLT698Helper.BuildWriteSingleObject("40-01-02-00", "AAAAAAAAAAAA", DLT698Helper.CreateStringValueBuffer(address), this);
		if (!build.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(build);
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(build.Content);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		return DLT698Helper.CheckResponse(read.Content);
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

	public override async Task<OperateResult<short[]>> ReadInt16Async(string address, ushort length)
	{
		return await ReadDataAndParseAsync(address, length, short.Parse);
	}

	public override async Task<OperateResult<ushort[]>> ReadUInt16Async(string address, ushort length)
	{
		return await ReadDataAndParseAsync(address, length, ushort.Parse);
	}

	public override async Task<OperateResult<int[]>> ReadInt32Async(string address, ushort length)
	{
		return await ReadDataAndParseAsync(address, length, int.Parse);
	}

	public override async Task<OperateResult<uint[]>> ReadUInt32Async(string address, ushort length)
	{
		return await ReadDataAndParseAsync(address, length, uint.Parse);
	}

	public override async Task<OperateResult<long[]>> ReadInt64Async(string address, ushort length)
	{
		return await ReadDataAndParseAsync(address, length, long.Parse);
	}

	public override async Task<OperateResult<ulong[]>> ReadUInt64Async(string address, ushort length)
	{
		return await ReadDataAndParseAsync(address, length, ulong.Parse);
	}

	public override async Task<OperateResult<float[]>> ReadFloatAsync(string address, ushort length)
	{
		return await ReadDataAndParseAsync(address, length, float.Parse);
	}

	public override async Task<OperateResult<double[]>> ReadDoubleAsync(string address, ushort length)
	{
		return await ReadDataAndParseAsync(address, length, double.Parse);
	}

	public override async Task<OperateResult<string>> ReadStringAsync(string address, ushort length, Encoding encoding)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadStringArrayAsync(address));
	}

	public override string ToString()
	{
		return $"DLT698OverTcp[{IpAddress}:{Port}]";
	}
}
