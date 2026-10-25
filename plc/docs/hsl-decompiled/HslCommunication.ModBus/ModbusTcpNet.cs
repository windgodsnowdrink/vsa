// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Reflection;

namespace HslCommunication.ModBus;

/// <summary>
/// Modbus-Tcp协议的客户端通讯类，方便的和服务器进行数据交互，支持标准的功能码，也支持扩展的功能码实现，地址采用富文本的形式，详细见API文档说明
/// The client communication class of Modbus-Tcp protocol is convenient for data interaction with the server. It supports standard function codes and also supports extended function codes.
/// The address is in rich text. For details, see the remarks.
/// </summary>
/// <remarks>
/// 本客户端支持的标准的modbus协议，Modbus-Tcp及Modbus-Udp内置的消息号会进行自增，地址支持富文本格式，具体参考示例代码。
/// 读取线圈，输入线圈，寄存器，输入寄存器的方法中的读取长度对商业授权用户不限制，内部自动切割读取，结果合并。
/// </remarks>
public class ModbusTcpNet : DeviceTcpNet, IModbus, IReadWriteDevice, IReadWriteNet
{
	private byte station = 1;

	private readonly SoftIncrementCount softIncrementCount;

	private bool isAddressStartWithZero = true;

	private Func<string, byte, OperateResult<string>> addressMapping = (string address, byte modbusCode) => OperateResult.CreateSuccessResult(address);

	/// <summary>
	/// 获取或设置起始的地址是否从0开始，默认为True
	/// Gets or sets whether the starting address starts from 0. The default is True
	/// </summary>
	/// <remarks>
	/// 因为有些设备的起始地址是从1开始的，就要设置本属性为False
	/// </remarks>
	public bool AddressStartWithZero
	{
		get
		{
			return isAddressStartWithZero;
		}
		set
		{
			isAddressStartWithZero = value;
		}
	}

	/// <summary>
	/// 获取或者重新修改服务器的默认站号信息，当然，你可以再读写的时候动态指定，例如地址 "s=2;100", 更详细的例子可以参考DEMO界面上的地址示例。
	/// Get or re-modify the server's default station number information, of course, you can dynamically specify when reading and writing,
	/// such as the address "s=2; 100", a more detailed example can refer to the DEMO interface address example.
	/// </summary>
	/// <remarks>
	/// 当你调用 ReadCoil("100") 时，对应的站号就是本属性的值，当你调用 ReadCoil("s=2;100") 时，就忽略本属性的值，读写寄存器的时候同理
	/// </remarks>
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
	/// 获取或设置数据解析的格式，可选ABCD, BADC，CDAB，DCBA格式，对int,uint,float,double,long,ulong类型有作用
	/// Get or set the format of the data analysis, optional ABCD, BADC, CDAB, DCBA format, effective for int, uint, float, double, long, ulong type
	/// </summary>
	public DataFormat DataFormat
	{
		get
		{
			return base.ByteTransform.DataFormat;
		}
		set
		{
			base.ByteTransform.DataFormat = value;
		}
	}

	/// <summary>
	/// 字符串数据是否按照字来反转，默认为False
	/// Whether the string data is reversed according to words. The default is False.
	/// </summary>
	/// <remarks>
	/// 字符串按照2个字节的排列进行颠倒，根据实际情况进行设置
	/// </remarks>
	public bool IsStringReverse
	{
		get
		{
			return base.ByteTransform.IsStringReverseByteWord;
		}
		set
		{
			base.ByteTransform.IsStringReverseByteWord = value;
		}
	}

	/// <summary>
	/// 获取或设置当前掩码写入的功能码是否激活状态，设置为 false 时，再执行写入位时，会通过读字，修改位，写字的方式来间接实现。
	/// When the function code is set to false, and then the write bit is executed, it will be indirectly implemented by reading, modifying the bit, and writing the word.
	/// </summary>
	public bool EnableWriteMaskCode { get; set; } = true;

	/// <summary>
	/// 获取或设置是否进行检查返回的消息ID和发送的消息ID是否一致，默认为true，也就是检查
	/// Get or set whether to check whether the returned message ID is consistent with the sent message ID, the default is true, that is, check
	/// </summary>
	public bool IsCheckMessageId { get; set; } = true;

	/// <summary>
	/// 获取或设置当前是否启用站号检查的功能，默认启用，读写数据时将进行站号确认操作，如果需要忽略站号，则设置为 false 即可。
	/// Gets or sets whether the station ID check function is enabled. It is enabled by default. When reading or writing data, the station ID is confirmed.
	/// </summary>
	public bool StationCheckMatch { get; set; } = true;

	/// <summary>
	/// 获取或是设置当前广播模式对应的站号，广播模式意味着不接收设备方的数据返回操作，默认为 -1，表示不使用广播模式。
	/// Gets or sets the station number corresponding to the current broadcast mode. Broadcast mode means that the data return operation of the device is not received. The default value is -1, indicating that broadcast mode is not used.
	/// </summary>
	public int BroadcastStation { get; set; } = -1;

	/// <summary>
	/// 当进行字读取的时候，商业授权用户支持分批读取，本属性表示分批的长度信息，默认为 120，可以根据实际情况来设置其他值。
	/// When reading of word type, commercial authorization users support batch reading. This attribute represents the length information of the batch, with a default of 120. Other values can be set according to the actual situation.
	/// </summary>
	public int WordReadBatchLength { get; set; } = 120;

	/// <summary>
	/// 获取或设置是否禁用功能码05的写入操作，设置为true时，表示不允许使用功能码 06 进行写入操作。写入单个的字数据也使用 0x10 功能码
	/// Get or set whether to disable the write operation of function code 06. When set to true, it indicates that the write operation of function code 05 is not allowed. Writing individual character data also uses the 0x10 function code
	/// </summary>
	public bool DisableFunctionCode06 { get; set; }

	public SoftIncrementCount MessageId => softIncrementCount;

	/// <summary>
	/// 实例化一个Modbus-Tcp协议的客户端对象
	/// Instantiate a client object of the Modbus-Tcp protocol
	/// </summary>
	public ModbusTcpNet()
	{
		softIncrementCount = new SoftIncrementCount(65535L, 0L);
		base.WordLength = 1;
		station = 1;
		base.ByteTransform = new RegularByteTransform(DataFormat.CDAB);
	}

	/// <summary>
	/// 指定服务器地址，端口号，客户端自己的站号来初始化
	/// Specify the server address, port number, and client's own station number to initialize
	/// </summary>
	/// <param name="ipAddress">服务器的Ip地址</param>
	/// <param name="port">服务器的端口号</param>
	/// <param name="station">客户端自身的站号</param>
	public ModbusTcpNet(string ipAddress, int port = 502, byte station = 1)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
		this.station = station;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new ModbusTcpMessage
		{
			IsCheckMessageId = IsCheckMessageId,
			StationCheckMatch = StationCheckMatch
		};
	}

	/// <summary>
	/// 将Modbus报文数据发送到当前的通道中，并从通道中接收Modbus的报文，通道将根据当前连接自动获取，本方法是线程安全的。
	/// Send Modbus message data to the current channel, and receive Modbus messages from the channel. The channel will automatically obtain it according to the current connection. This method is thread-safe.
	/// </summary>
	/// <param name="send">发送的完整的报文信息</param>
	/// <returns>接收到的Modbus报文信息</returns>
	/// <remarks>
	/// 需要注意的是，本方法的发送和接收都只需要输入Modbus核心报文，例如读取寄存器0的字数据 01 03 00 00 00 01，最前面的6个字节是自动添加的，收到的数据也是只有modbus核心报文，例如：01 03 02 00 00 , 所以在解析的时候需要注意。
	/// It should be noted that the sending and receiving of this method only need to input the Modbus core message, for example, read the word data 01 03 00 00 00 01 of register 0,
	/// the first 6 bytes are automatically added, and the received The data is also only modbus core messages, for example: 01 03 02 00 00, so you need to pay attention when parsing.
	/// </remarks>
	public override OperateResult<byte[]> ReadFromCoreServer(byte[] send)
	{
		if (BroadcastStation >= 0 && send[0] == BroadcastStation)
		{
			return ReadFromCoreServer(send, hasResponseData: false, usePackAndUnpack: true);
		}
		return base.ReadFromCoreServer(send);
	}

	/// <summary>
	/// 将Modbus报文数据发送到当前的通道中，并从通道中接收Modbus的报文，通道将根据当前连接自动获取，本方法是线程安全的。
	/// Send Modbus message data to the current channel, and receive Modbus messages from the channel. The channel will automatically obtain it according to the current connection. This method is thread-safe.
	/// </summary>
	/// <param name="send">发送的完整的报文信息</param>
	/// <returns>接收到的Modbus报文信息</returns>
	/// <remarks>
	/// 需要注意的是，本方法的发送和接收都只需要输入Modbus核心报文，例如读取寄存器0的字数据 01 03 00 00 00 01，最前面的6个字节是自动添加的，收到的数据也是只有modbus核心报文，例如：01 03 02 00 00 , 所以在解析的时候需要注意。
	/// It should be noted that the sending and receiving of this method only need to input the Modbus core message, for example, read the word data 01 03 00 00 00 01 of register 0,
	/// the first 6 bytes are automatically added, and the received The data is also only modbus core messages, for example: 01 03 02 00 00, so you need to pay attention when parsing.
	/// </remarks>
	public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(byte[] send)
	{
		if (BroadcastStation >= 0 && send[0] == BroadcastStation)
		{
			return ReadFromCoreServer(send, hasResponseData: false, usePackAndUnpack: true);
		}
		return await base.ReadFromCoreServerAsync(send);
	}

	[HslMqttApi("ReadInt32Array", "")]
	/// <summary>
	/// 读取32位有符号整型数组
	/// Read 32-bit signed integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的int数组</returns>
	public override OperateResult<int[]> ReadInt32(string address, ushort length)
	{
		IByteTransform transform = HslHelper.ExtractTransformParameter(ref address, base.ByteTransform);
		return ByteTransformHelper.GetResultFromBytes(Read(address, GetWordLength(address, length, 2)), (byte[] m) => transform.TransInt32(m, 0, length));
	}

	[HslMqttApi("ReadUInt32Array", "")]
	/// <summary>
	/// 读取32位的无符号整型数组
	/// Read 32-bit unsigned integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的uint数组</returns>
	public override OperateResult<uint[]> ReadUInt32(string address, ushort length)
	{
		IByteTransform transform = HslHelper.ExtractTransformParameter(ref address, base.ByteTransform);
		return ByteTransformHelper.GetResultFromBytes(Read(address, GetWordLength(address, length, 2)), (byte[] m) => transform.TransUInt32(m, 0, length));
	}

	[HslMqttApi("ReadFloatArray", "")]
	/// <summary>
	/// 读取单浮点精度的数组
	/// Read single floating point array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的float数组</returns>
	public override OperateResult<float[]> ReadFloat(string address, ushort length)
	{
		IByteTransform transform = HslHelper.ExtractTransformParameter(ref address, base.ByteTransform);
		return ByteTransformHelper.GetResultFromBytes(Read(address, GetWordLength(address, length, 2)), (byte[] m) => transform.TransSingle(m, 0, length));
	}

	[HslMqttApi("ReadInt64Array", "")]
	/// <summary>
	/// 读取64位的有符号整型数组
	/// Read 64-bit signed integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的long数组</returns>
	public override OperateResult<long[]> ReadInt64(string address, ushort length)
	{
		IByteTransform transform = HslHelper.ExtractTransformParameter(ref address, base.ByteTransform);
		return ByteTransformHelper.GetResultFromBytes(Read(address, GetWordLength(address, length, 4)), (byte[] m) => transform.TransInt64(m, 0, length));
	}

	[HslMqttApi("ReadUInt64Array", "")]
	/// <summary>
	/// 读取64位的无符号整型的数组
	/// Read 64-bit unsigned integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public override OperateResult<ulong[]> ReadUInt64(string address, ushort length)
	{
		IByteTransform transform = HslHelper.ExtractTransformParameter(ref address, base.ByteTransform);
		return ByteTransformHelper.GetResultFromBytes(Read(address, GetWordLength(address, length, 4)), (byte[] m) => transform.TransUInt64(m, 0, length));
	}

	[HslMqttApi("ReadDoubleArray", "")]
	/// <summary>
	/// 读取双浮点数据的数组
	/// Read double floating point data array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的double数组</returns>
	public override OperateResult<double[]> ReadDouble(string address, ushort length)
	{
		IByteTransform transform = HslHelper.ExtractTransformParameter(ref address, base.ByteTransform);
		return ByteTransformHelper.GetResultFromBytes(Read(address, GetWordLength(address, length, 4)), (byte[] m) => transform.TransDouble(m, 0, length));
	}

	[HslMqttApi("WriteInt32Array", "")]
	/// <summary>
	/// 写入int[]数组，返回是否成功
	/// Write int array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override OperateResult Write(string address, int[] values)
	{
		IByteTransform byteTransform = HslHelper.ExtractTransformParameter(ref address, base.ByteTransform);
		return Write(address, byteTransform.TransByte(values));
	}

	[HslMqttApi("WriteUInt32Array", "")]
	/// <summary>
	/// 写入uint[]数组，返回是否成功
	/// Write uint array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override OperateResult Write(string address, uint[] values)
	{
		IByteTransform byteTransform = HslHelper.ExtractTransformParameter(ref address, base.ByteTransform);
		return Write(address, byteTransform.TransByte(values));
	}

	[HslMqttApi("WriteFloatArray", "")]
	/// <summary>
	/// 写入float数组，返回是否成功
	/// Write float array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override OperateResult Write(string address, float[] values)
	{
		IByteTransform byteTransform = HslHelper.ExtractTransformParameter(ref address, base.ByteTransform);
		return Write(address, byteTransform.TransByte(values));
	}

	[HslMqttApi("WriteInt64Array", "")]
	/// <summary>
	/// 写入long数组，返回是否成功
	/// Write long array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override OperateResult Write(string address, long[] values)
	{
		IByteTransform byteTransform = HslHelper.ExtractTransformParameter(ref address, base.ByteTransform);
		return Write(address, byteTransform.TransByte(values));
	}

	[HslMqttApi("WriteUInt64Array", "")]
	/// <summary>
	/// 写入ulong数组，返回是否成功
	/// Write ulong array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override OperateResult Write(string address, ulong[] values)
	{
		IByteTransform byteTransform = HslHelper.ExtractTransformParameter(ref address, base.ByteTransform);
		return Write(address, byteTransform.TransByte(values));
	}

	[HslMqttApi("WriteDoubleArray", "")]
	/// <summary>
	/// 写入double数组，返回是否成功
	/// Write double array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override OperateResult Write(string address, double[] values)
	{
		IByteTransform byteTransform = HslHelper.ExtractTransformParameter(ref address, base.ByteTransform);
		return Write(address, byteTransform.TransByte(values));
	}

	/// <summary>
	/// 异步读取32位有符号整型数组
	/// Asynchronously read 32-bit signed integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的int数组</returns>
	public override async Task<OperateResult<int[]>> ReadInt32Async(string address, ushort length)
	{
		IByteTransform transform = HslHelper.ExtractTransformParameter(ref address, base.ByteTransform);
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, GetWordLength(address, length, 2)), (byte[] m) => transform.TransInt32(m, 0, length));
	}

	/// <summary>
	/// 异步读取32位的无符号整型数组
	/// Asynchronously read 32-bit unsigned integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的uint数组</returns>
	public override async Task<OperateResult<uint[]>> ReadUInt32Async(string address, ushort length)
	{
		IByteTransform transform = HslHelper.ExtractTransformParameter(ref address, base.ByteTransform);
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, GetWordLength(address, length, 2)), (byte[] m) => transform.TransUInt32(m, 0, length));
	}

	/// <summary>
	/// 异步读取单浮点精度的数组
	/// Asynchronously read single floating point array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的float数组</returns>
	public override async Task<OperateResult<float[]>> ReadFloatAsync(string address, ushort length)
	{
		IByteTransform transform = HslHelper.ExtractTransformParameter(ref address, base.ByteTransform);
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, GetWordLength(address, length, 2)), (byte[] m) => transform.TransSingle(m, 0, length));
	}

	/// <summary>
	/// 异步读取64位的有符号整型数组
	/// Asynchronously read 64-bit signed integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的long数组</returns>
	public override async Task<OperateResult<long[]>> ReadInt64Async(string address, ushort length)
	{
		IByteTransform transform = HslHelper.ExtractTransformParameter(ref address, base.ByteTransform);
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, GetWordLength(address, length, 4)), (byte[] m) => transform.TransInt64(m, 0, length));
	}

	/// <summary>
	/// 异步读取64位的无符号整型的数组
	/// Asynchronously read 64-bit unsigned integer array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的ulong数组</returns>
	public override async Task<OperateResult<ulong[]>> ReadUInt64Async(string address, ushort length)
	{
		IByteTransform transform = HslHelper.ExtractTransformParameter(ref address, base.ByteTransform);
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, GetWordLength(address, length, 4)), (byte[] m) => transform.TransUInt64(m, 0, length));
	}

	/// <summary>
	/// 异步读取双浮点数据的数组
	/// Asynchronously read double floating point data array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的double数组</returns>
	public override async Task<OperateResult<double[]>> ReadDoubleAsync(string address, ushort length)
	{
		IByteTransform transform = HslHelper.ExtractTransformParameter(ref address, base.ByteTransform);
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, GetWordLength(address, length, 4)), (byte[] m) => transform.TransDouble(m, 0, length));
	}

	/// <summary>
	/// 异步写入int[]数组，返回是否成功
	/// Asynchronously write int array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, int[] values)
	{
		return await WriteAsync(value: HslHelper.ExtractTransformParameter(ref address, base.ByteTransform).TransByte(values), address: address);
	}

	/// <summary>
	/// 异步写入uint[]数组，返回是否成功
	/// Asynchronously write uint array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, uint[] values)
	{
		return await WriteAsync(value: HslHelper.ExtractTransformParameter(ref address, base.ByteTransform).TransByte(values), address: address);
	}

	/// <summary>
	/// 异步写入float数组，返回是否成功
	/// Asynchronously write float array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, float[] values)
	{
		return await WriteAsync(value: HslHelper.ExtractTransformParameter(ref address, base.ByteTransform).TransByte(values), address: address);
	}

	/// <summary>
	/// 异步写入long数组，返回是否成功
	/// Asynchronously write long array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, long[] values)
	{
		return await WriteAsync(value: HslHelper.ExtractTransformParameter(ref address, base.ByteTransform).TransByte(values), address: address);
	}

	/// <summary>
	/// 异步写入ulong数组，返回是否成功
	/// Asynchronously write ulong array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, ulong[] values)
	{
		return await WriteAsync(value: HslHelper.ExtractTransformParameter(ref address, base.ByteTransform).TransByte(values), address: address);
	}

	/// <summary>
	/// 异步写入double数组，返回是否成功
	/// Asynchronously write double array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, double[] values)
	{
		return await WriteAsync(value: HslHelper.ExtractTransformParameter(ref address, base.ByteTransform).TransByte(values), address: address);
	}

	/// <summary>
	/// 将当前的地址信息转换成Modbus格式的地址，如果转换失败，返回失败的消息。默认不进行任何的转换。
	/// Convert the current address information into a Modbus format address. If the conversion fails, a failure message will be returned. No conversion is performed by default.
	/// </summary>
	/// <param name="address">传入的地址</param>
	/// <param name="modbusCode">Modbus的功能码</param>
	/// <returns>转换之后Modbus的地址</returns>
	public virtual OperateResult<string> TranslateToModbusAddress(string address, byte modbusCode)
	{
		if (addressMapping != null)
		{
			return addressMapping(address, modbusCode);
		}
		return OperateResult.CreateSuccessResult(address);
	}

	/// <summary>
	/// 注册一个新的地址映射关系，注册地址映射关系后，就可以使用新的地址来读写Modbus数据了，通常用于其他的支持Modbus协议的PLC。
	/// After registering a new address mapping, you can use the new address to read and write Modbus data, which is usually used for other PLCs that support the Modbus protocol.
	/// </summary>
	/// <param name="mapping">地址映射关系信息</param>
	public void RegisteredAddressMapping(Func<string, byte, OperateResult<string>> mapping)
	{
		addressMapping = mapping;
	}

	public override byte[] PackCommandWithHeader(byte[] command)
	{
		return ModbusInfo.PackCommandToTcp(command, (ushort)softIncrementCount.GetCurrentValue());
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		if (BroadcastStation >= 0 && send[6] == BroadcastStation)
		{
			return OperateResult.CreateSuccessResult(new byte[0]);
		}
		if (response == null || response.Length < 6)
		{
			return new OperateResult<byte[]>(StringResources.Language.ReceiveDataLengthTooShort + " Content: " + response.ToHexString(' '));
		}
		return ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(response));
	}

	/// <summary>
	/// 读取线圈，需要指定起始地址，如果富文本地址不指定，默认使用的功能码是 0x01
	/// To read the coil, you need to specify the start address. If the rich text address is not specified, the default function code is 0x01.
	/// </summary>
	/// <param name="address">起始地址，格式为&quot;1234&quot;</param>
	/// <returns>带有成功标志的bool对象</returns>
	public OperateResult<bool> ReadCoil(string address)
	{
		return ReadBool(address);
	}

	/// <summary>
	/// 批量的读取线圈，需要指定起始地址，读取长度，如果富文本地址不指定，默认使用的功能码是 0x01
	/// For batch reading coils, you need to specify the start address and read length. If the rich text address is not specified, the default function code is 0x01.
	/// </summary>
	/// <param name="address">起始地址，格式为&quot;1234&quot;</param>
	/// <param name="length">读取长度</param>
	/// <returns>带有成功标志的bool数组对象</returns>
	public OperateResult<bool[]> ReadCoil(string address, ushort length)
	{
		return ReadBool(address, length);
	}

	/// <summary>
	/// 读取输入线圈，需要指定起始地址，如果富文本地址不指定，默认使用的功能码是 0x02
	/// To read the input coil, you need to specify the start address. If the rich text address is not specified, the default function code is 0x02.
	/// </summary>
	/// <param name="address">起始地址，格式为&quot;1234&quot;</param>
	/// <returns>带有成功标志的bool对象</returns>
	public OperateResult<bool> ReadDiscrete(string address)
	{
		return ByteTransformHelper.GetResultFromArray(ReadDiscrete(address, 1));
	}

	/// <summary>
	/// 批量的读取输入点，需要指定起始地址，读取长度，如果富文本地址不指定，默认使用的功能码是 0x02
	/// To read input points in batches, you need to specify the start address and read length. If the rich text address is not specified, the default function code is 0x02
	/// </summary>
	/// <param name="address">起始地址，格式为&quot;1234&quot;</param>
	/// <param name="length">读取长度</param>
	/// <returns>带有成功标志的bool数组对象</returns>
	public OperateResult<bool[]> ReadDiscrete(string address, ushort length)
	{
		return ModbusHelper.ReadBoolHelper(this, address, length, 2);
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 从Modbus服务器批量读取寄存器的信息，需要指定起始地址，读取长度，如果富文本地址不指定，默认使用的功能码是 0x03，如果需要使用04功能码，那么地址就写成 x=4;100
	/// To read the register information from the Modbus server in batches, you need to specify the start address and read length. If the rich text address is not specified,
	/// the default function code is 0x03. If you need to use the 04 function code, the address is written as x = 4; 100
	/// </summary>
	/// <param name="address">起始地址，比如&quot;100&quot;，&quot;x=4;100&quot;，&quot;s=1;100&quot;,&quot;s=1;x=4;100&quot;</param>
	/// <param name="length">读取的数量</param>
	/// <returns>带有成功标志的字节信息</returns>
	/// <remarks>
	/// 富地址格式，支持携带站号信息，功能码信息，具体参照类的示例代码
	/// </remarks>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return ModbusHelper.Read(this, address, length);
	}

	[HslMqttApi("ReadWrite", "Use 0x17 function code to write and read data at the same time, and use a message to implement it")]
	/// <summary>
	/// 使用0x17功能码来实现同时写入并读取数据的操作，使用一条报文来实现，需要指定读取的地址，长度，写入的地址，写入的数据信息，返回读取的结果数据。
	/// Use 0x17 function code to write and read data at the same time, and use a message to implement it,
	/// you need to specify the read address, length, written address, written data information, and return the read result data.
	/// </summary>
	/// <param name="modbus">Modbus通信对象</param>
	/// <param name="readAddress">读取的地址信息</param>
	/// <param name="length">读取的长度信息</param>
	/// <param name="writeAddress">写入的地址信息</param>
	/// <param name="value">写入的字节数据信息</param>
	/// <returns>读取的结果对象</returns>
	public OperateResult<byte[]> ReadWrite(string readAddress, ushort length, string writeAddress, byte[] value)
	{
		return ModbusHelper.ReadWrite(this, readAddress, length, writeAddress, value);
	}

	[HslMqttApi("ReadFile", "")]
	/// <summary>
	/// 使用0x14功能码读取文件的数据内容，需要指定文件编号，地址，长度，返回读取的结果数据，读取的长度为任意长度，内部自动根据实际情况来切割操作。
	/// </summary>
	/// <param name="modbus">Modbus设备通信对象</param>
	/// <param name="station">设备站号信息</param>
	/// <param name="fileNumber">文件号</param>
	/// <param name="address">起始地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>返回结果</returns>
	public OperateResult<byte[]> ReadFile(ushort fileNumber, ushort address, ushort length)
	{
		return ModbusHelper.ReadFile(this, Station, fileNumber, address, length);
	}

	[HslMqttApi("WriteFile", "")]
	/// <summary>
	/// 使用0x15功能码写入文件的数据内容，需要指定文件编号，地址，数据，返回写入的结果数据。写入的数据字节长度不能大于248个字节，如果想写入更多的字节，就需要切片写入操作
	/// </summary>
	/// <param name="modbus">Modbus设备通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="fileNumber">文件号</param>
	/// <param name="address">起始地址</param>
	/// <param name="data">写入的数据信息</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteFile(ushort fileNumber, ushort address, byte[] data)
	{
		return ModbusHelper.WriteFile(this, Station, fileNumber, address, data);
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 将数据写入到Modbus的寄存器上去，需要指定起始地址和数据内容，如果富文本地址不指定，默认使用的功能码是 0x10
	/// To write data to Modbus registers, you need to specify the start address and data content. If the rich text address is not specified, the default function code is 0x10
	/// </summary>
	/// <param name="address">起始地址，比如&quot;100&quot;，&quot;x=4;100&quot;，&quot;s=1;100&quot;,&quot;s=1;x=4;100&quot;</param>
	/// <param name="value">写入的数据，长度根据data的长度来指示</param>
	/// <returns>返回写入结果</returns>
	/// <remarks>
	/// 富地址格式，支持携带站号信息，功能码信息，具体参照类的示例代码
	/// </remarks>
	public override OperateResult Write(string address, byte[] value)
	{
		return ModbusHelper.Write(this, address, value);
	}

	[HslMqttApi("WriteInt16", "")]
	/// <summary>
	/// 将数据写入到Modbus的单个寄存器上去，需要指定起始地址和数据值，如果富文本地址不指定，默认使用的功能码是 0x06
	/// To write data to a single register of Modbus, you need to specify the start address and data value. If the rich text address is not specified, the default function code is 0x06.
	/// </summary>
	/// <param name="address">起始地址，比如&quot;100&quot;，&quot;x=4;100&quot;，&quot;s=1;100&quot;,&quot;s=1;x=4;100&quot;</param>
	/// <param name="value">写入的short数据</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, short value)
	{
		return ModbusHelper.Write(this, address, value);
	}

	[HslMqttApi("WriteUInt16", "")]
	/// <summary>
	/// 将数据写入到Modbus的单个寄存器上去，需要指定起始地址和数据值，如果富文本地址不指定，默认使用的功能码是 0x06
	/// To write data to a single register of Modbus, you need to specify the start address and data value. If the rich text address is not specified, the default function code is 0x06.
	/// </summary>
	/// <param name="address">起始地址，比如&quot;100&quot;，&quot;x=4;100&quot;，&quot;s=1;100&quot;,&quot;s=1;x=4;100&quot;</param>
	/// <param name="value">写入的ushort数据</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, ushort value)
	{
		return ModbusHelper.Write(this, address, value);
	}

	[HslMqttApi("WriteMask", "")]
	/// <summary>
	/// 向设备写入掩码数据，使用0x16功能码，需要确认对方是否支持相关的操作，掩码数据的操作主要针对寄存器。
	/// To write mask data to the server, using the 0x16 function code, you need to confirm whether the other party supports related operations.
	/// The operation of mask data is mainly directed to the register.
	/// </summary>
	/// <param name="address">起始地址，起始地址，比如&quot;100&quot;，&quot;x=4;100&quot;，&quot;s=1;100&quot;,&quot;s=1;x=4;100&quot;</param>
	/// <param name="andMask">等待与操作的掩码数据</param>
	/// <param name="orMask">等待或操作的掩码数据</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteMask(string address, ushort andMask, ushort orMask)
	{
		return ModbusHelper.WriteMask(this, address, andMask, orMask);
	}

	/// <summary>
	/// 将数据写入到Modbus的单个寄存器上去，需要指定起始地址和数据值，如果富文本地址不指定，默认使用的功能码是 0x06
	/// To write data to a single register of Modbus, you need to specify the start address and data value. If the rich text address is not specified, the default function code is 0x06.
	/// </summary>
	/// <param name="address">起始地址，比如&quot;100&quot;，&quot;x=4;100&quot;，&quot;s=1;100&quot;,&quot;s=1;x=4;100&quot;</param>
	/// <param name="value">写入的short数据</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteOneRegister(string address, short value)
	{
		return Write(address, value);
	}

	/// <summary>
	/// 将数据写入到Modbus的单个寄存器上去，需要指定起始地址和数据值，如果富文本地址不指定，默认使用的功能码是 0x06
	/// To write data to a single register of Modbus, you need to specify the start address and data value. If the rich text address is not specified, the default function code is 0x06.
	/// </summary>
	/// <param name="address">起始地址，比如&quot;100&quot;，&quot;x=4;100&quot;，&quot;s=1;100&quot;,&quot;s=1;x=4;100&quot;</param>
	/// <param name="value">写入的ushort数据</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteOneRegister(string address, ushort value)
	{
		return Write(address, value);
	}

	/// <summary>
	/// 读取线圈，需要指定起始地址，如果富文本地址不指定，默认使用的功能码是 0x01
	/// To read the coil, you need to specify the start address. If the rich text address is not specified, the default function code is 0x01.
	/// </summary>
	/// <param name="address">起始地址，格式为&quot;1234&quot;</param>
	/// <returns>带有成功标志的bool对象</returns>
	public async Task<OperateResult<bool>> ReadCoilAsync(string address)
	{
		return await ReadBoolAsync(address);
	}

	/// <summary>
	/// 批量的读取线圈，需要指定起始地址，读取长度，如果富文本地址不指定，默认使用的功能码是 0x01
	/// For batch reading coils, you need to specify the start address and read length. If the rich text address is not specified, the default function code is 0x01.
	/// </summary>
	/// <param name="address">起始地址，格式为&quot;1234&quot;</param>
	/// <param name="length">读取长度</param>
	/// <returns>带有成功标志的bool数组对象</returns>
	public async Task<OperateResult<bool[]>> ReadCoilAsync(string address, ushort length)
	{
		return await ReadBoolAsync(address, length);
	}

	/// <summary>
	/// 读取输入线圈，需要指定起始地址，如果富文本地址不指定，默认使用的功能码是 0x02
	/// To read the input coil, you need to specify the start address. If the rich text address is not specified, the default function code is 0x02.
	/// </summary>
	/// <param name="address">起始地址，格式为&quot;1234&quot;</param>
	/// <returns>带有成功标志的bool对象</returns>
	public async Task<OperateResult<bool>> ReadDiscreteAsync(string address)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadDiscreteAsync(address, 1));
	}

	/// <summary>
	/// 批量的读取输入点，需要指定起始地址，读取长度，如果富文本地址不指定，默认使用的功能码是 0x02
	/// To read input points in batches, you need to specify the start address and read length. If the rich text address is not specified, the default function code is 0x02
	/// </summary>
	/// <param name="address">起始地址，格式为&quot;1234&quot;</param>
	/// <param name="length">读取长度</param>
	/// <returns>带有成功标志的bool数组对象</returns>
	public async Task<OperateResult<bool[]>> ReadDiscreteAsync(string address, ushort length)
	{
		return await ReadBoolHelperAsync(address, length, 2);
	}

	/// <summary>
	/// 从Modbus服务器批量读取寄存器的信息，需要指定起始地址，读取长度，如果富文本地址不指定，默认使用的功能码是 0x03，如果需要使用04功能码，那么地址就写成 x=4;100
	/// To read the register information from the Modbus server in batches, you need to specify the start address and read length. If the rich text address is not specified,
	/// the default function code is 0x03. If you need to use the 04 function code, the address is written as x = 4; 100
	/// </summary>
	/// <param name="address">起始地址，比如&quot;100&quot;，&quot;x=4;100&quot;，&quot;s=1;100&quot;,&quot;s=1;x=4;100&quot;</param>
	/// <param name="length">读取的数量</param>
	/// <returns>带有成功标志的字节信息</returns>
	/// <remarks>
	/// 富地址格式，支持携带站号信息，功能码信息，具体参照类的示例代码
	/// </remarks>
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		return await ModbusHelper.ReadAsync(this, address, length);
	}

	/// <summary>
	/// 使用0x17功能码来实现同时写入并读取数据的操作，使用一条报文来实现，需要指定读取的地址，长度，写入的地址，写入的数据信息，返回读取的结果数据。
	/// Use 0x17 function code to write and read data at the same time, and use a message to implement it,
	/// you need to specify the read address, length, written address, written data information, and return the read result data.
	/// </summary>
	/// <param name="modbus">Modbus通信对象</param>
	/// <param name="readAddress">读取的地址信息</param>
	/// <param name="length">读取的长度信息</param>
	/// <param name="writeAddress">写入的地址信息</param>
	/// <param name="value">写入的字节数据信息</param>
	/// <returns>读取的结果对象</returns>
	public async Task<OperateResult<byte[]>> ReadWriteAsync(string readAddress, ushort length, string writeAddress, byte[] value)
	{
		return await ModbusHelper.ReadWriteAsync(this, readAddress, length, writeAddress, value);
	}

	/// <summary>
	/// 将数据写入到Modbus的寄存器上去，需要指定起始地址和数据内容，如果富文本地址不指定，默认使用的功能码是 0x10
	/// To write data to Modbus registers, you need to specify the start address and data content. If the rich text address is not specified, the default function code is 0x10
	/// </summary>
	/// <param name="address">起始地址，比如&quot;100&quot;，&quot;x=4;100&quot;，&quot;s=1;100&quot;,&quot;s=1;x=4;100&quot;</param>
	/// <param name="value">写入的数据，长度根据data的长度来指示</param>
	/// <returns>返回写入结果</returns>
	/// <remarks>
	/// 富地址格式，支持携带站号信息，功能码信息，具体参照类的示例代码
	/// </remarks>
	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await ModbusHelper.WriteAsync(this, address, value);
	}

	/// <summary>
	/// 将数据写入到Modbus的单个寄存器上去，需要指定起始地址和数据值，如果富文本地址不指定，默认使用的功能码是 0x06
	/// To write data to a single register of Modbus, you need to specify the start address and data value. If the rich text address is not specified, the default function code is 0x06.
	/// </summary>
	/// <param name="address">起始地址，比如&quot;100&quot;，&quot;x=4;100&quot;，&quot;s=1;100&quot;,&quot;s=1;x=4;100&quot;</param>
	/// <param name="value">写入的short数据</param>
	/// <returns>是否写入成功</returns>
	public override async Task<OperateResult> WriteAsync(string address, short value)
	{
		return await ModbusHelper.WriteAsync(this, address, value);
	}

	/// <summary>
	/// 将数据写入到Modbus的单个寄存器上去，需要指定起始地址和数据值，如果富文本地址不指定，默认使用的功能码是 0x06
	/// To write data to a single register of Modbus, you need to specify the start address and data value. If the rich text address is not specified, the default function code is 0x06.
	/// </summary>
	/// <param name="address">起始地址，比如&quot;100&quot;，&quot;x=4;100&quot;，&quot;s=1;100&quot;,&quot;s=1;x=4;100&quot;</param>
	/// <param name="value">写入的ushort数据</param>
	/// <returns>是否写入成功</returns>
	public override async Task<OperateResult> WriteAsync(string address, ushort value)
	{
		return await ModbusHelper.WriteAsync(this, address, value);
	}

	/// <summary>
	/// 向设备写入掩码数据，使用0x16功能码，需要确认对方是否支持相关的操作，掩码数据的操作主要针对寄存器。
	/// To write mask data to the server, using the 0x16 function code, you need to confirm whether the other party supports related operations.
	/// The operation of mask data is mainly directed to the register.
	/// </summary>
	/// <param name="address">起始地址，起始地址，比如&quot;100&quot;，&quot;x=4;100&quot;，&quot;s=1;100&quot;,&quot;s=1;x=4;100&quot;</param>
	/// <param name="andMask">等待与操作的掩码数据</param>
	/// <param name="orMask">等待或操作的掩码数据</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteMaskAsync(string address, ushort andMask, ushort orMask)
	{
		return await ModbusHelper.WriteMaskAsync(this, address, andMask, orMask);
	}

	/// <summary>
	/// 使用0x14功能码读取文件的数据内容，需要指定文件编号，地址，长度，返回读取的结果数据，读取的长度为任意长度，内部自动根据实际情况来切割操作。
	/// </summary>
	/// <param name="modbus">Modbus设备通信对象</param>
	/// <param name="station">设备站号信息</param>
	/// <param name="fileNumber">文件号</param>
	/// <param name="address">起始地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>返回结果</returns>
	public async Task<OperateResult<byte[]>> ReadFileAsync(ushort fileNumber, ushort address, ushort length)
	{
		return await ModbusHelper.ReadFileAsync(this, Station, fileNumber, address, length);
	}

	/// <summary>
	/// 将数据写入到Modbus的单个寄存器上去，需要指定起始地址和数据值，如果富文本地址不指定，默认使用的功能码是 0x06
	/// To write data to a single register of Modbus, you need to specify the start address and data value. If the rich text address is not specified, the default function code is 0x06.
	/// </summary>
	/// <param name="address">起始地址，比如&quot;100&quot;，&quot;x=4;100&quot;，&quot;s=1;100&quot;,&quot;s=1;x=4;100&quot;</param>
	/// <param name="value">写入的short数据</param>
	/// <returns>是否写入成功</returns>
	public virtual async Task<OperateResult> WriteOneRegisterAsync(string address, short value)
	{
		return await WriteAsync(address, value);
	}

	/// <summary>
	/// 将数据写入到Modbus的单个寄存器上去，需要指定起始地址和数据值，如果富文本地址不指定，默认使用的功能码是 0x06
	/// To write data to a single register of Modbus, you need to specify the start address and data value. If the rich text address is not specified, the default function code is 0x06.
	/// </summary>
	/// <param name="address">起始地址，比如&quot;100&quot;，&quot;x=4;100&quot;，&quot;s=1;100&quot;,&quot;s=1;x=4;100&quot;</param>
	/// <param name="value">写入的ushort数据</param>
	/// <returns>是否写入成功</returns>
	public virtual async Task<OperateResult> WriteOneRegisterAsync(string address, ushort value)
	{
		return await WriteAsync(address, value);
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 批量读取线圈或是离散的数据信息，需要指定地址和长度，具体的结果取决于实现，如果富文本地址不指定，默认使用的功能码是 0x01
	/// To read coils or discrete data in batches, you need to specify the address and length. The specific result depends on the implementation. If the rich text address is not specified, the default function code is 0x01.
	/// </summary>
	/// <param name="address">数据地址，比如 &quot;1234&quot;</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的bool[]数组</returns>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		return ModbusHelper.ReadBoolHelper(this, address, length, 1);
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// 向线圈中写入bool数组，返回是否写入成功，如果富文本地址不指定，默认使用的功能码是 0x0F
	/// Write the bool array to the coil, and return whether the writing is successful. If the rich text address is not specified, the default function code is 0x0F.
	/// </summary>
	/// <param name="address">要写入的数据地址，比如&quot;1234&quot;</param>
	/// <param name="values">要写入的实际数组</param>
	/// <returns>返回写入结果</returns>
	public override OperateResult Write(string address, bool[] values)
	{
		return ModbusHelper.Write(this, address, values);
	}

	[HslMqttApi("WriteBool", "")]
	/// <summary>
	/// 向线圈中写入bool数值，返回是否写入成功，如果富文本地址不指定，默认使用的功能码是 0x05，
	/// 如果你的地址为字地址，例如100.2，那么将使用0x16的功能码，通过掩码的方式来修改寄存器的某一位，需要Modbus服务器支持，否则自动切换为读取字数据，修改位，再写入字的方式。
	/// Write bool value to the coil and return whether the writing is successful. If the rich text address is not specified, the default function code is 0x05.
	/// If your address is a word address, such as 100.2, then you will use the function code of 0x16 to modify a bit of the register through a mask.
	/// It needs Modbus server support, Otherwise, it automatically switches to read the word data, modifies the bits, then writes the word in the way.
	/// </summary>
	/// <param name="address">要写入的数据地址，比如&quot;12345&quot;</param>
	/// <param name="value">要写入的实际数据</param>
	/// <returns>返回写入结果</returns>
	public override OperateResult Write(string address, bool value)
	{
		return ModbusHelper.Write(this, address, value);
	}

	private async Task<OperateResult<bool[]>> ReadBoolHelperAsync(string address, ushort length, byte function)
	{
		return await ModbusHelper.ReadBoolHelperAsync(this, address, length, function);
	}

	/// <summary>
	/// 批量读取线圈或是离散的数据信息，需要指定地址和长度，具体的结果取决于实现，如果富文本地址不指定，默认使用的功能码是 0x01
	/// To read coils or discrete data in batches, you need to specify the address and length. The specific result depends on the implementation. If the rich text address is not specified, the default function code is 0x01.
	/// </summary>
	/// <param name="address">数据地址，比如 &quot;1234&quot;</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的bool[]数组</returns>
	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		return await ReadBoolHelperAsync(address, length, 1);
	}

	/// <summary>
	/// 向线圈中写入bool数组，返回是否写入成功，如果富文本地址不指定，默认使用的功能码是 0x0F
	/// Write the bool array to the coil, and return whether the writing is successful. If the rich text address is not specified, the default function code is 0x0F.
	/// </summary>
	/// <param name="address">要写入的数据地址，比如&quot;1234&quot;</param>
	/// <param name="values">要写入的实际数组</param>
	/// <returns>返回写入结果</returns>
	public override async Task<OperateResult> WriteAsync(string address, bool[] values)
	{
		return await ModbusHelper.WriteAsync(this, address, values);
	}

	/// <summary>
	/// 向线圈中写入bool数值，返回是否写入成功，如果富文本地址不指定，默认使用的功能码是 0x05，
	/// 如果你的地址为字地址，例如100.2，那么将使用0x16的功能码，通过掩码的方式来修改寄存器的某一位，需要Modbus服务器支持，否则自动切换为读取字数据，修改位，再写入字的方式。
	/// Write bool value to the coil and return whether the writing is successful. If the rich text address is not specified, the default function code is 0x05.
	/// If your address is a word address, such as 100.2, then you will use the function code of 0x16 to modify a bit of the register through a mask.
	/// It needs Modbus server support, Otherwise, it automatically switches to read the word data, modifies the bits, then writes the word in the way.
	/// </summary>
	/// <param name="address">要写入的数据地址，比如&quot;12345&quot;</param>
	/// <param name="value">要写入的实际数据</param>
	/// <returns>返回写入结果</returns>
	public override async Task<OperateResult> WriteAsync(string address, bool value)
	{
		return await ModbusHelper.WriteAsync(this, address, value);
	}

	public override string ToString()
	{
		return $"ModbusTcpNet[{IpAddress}:{Port}]";
	}
}
