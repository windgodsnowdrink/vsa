// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Core.Pipe;
using HslCommunication.Reflection;
using HslCommunication.Serial;

namespace HslCommunication.ModBus;

/// <summary>
/// Modbus的虚拟服务器，同时支持Tcp，Rtu，Ascii的机制，支持线圈，离散输入，寄存器和输入寄存器的读写操作，同时支持掩码写入功能，可以用来当做系统的数据交换池
/// Modbus virtual server supports Tcp and Rtu mechanisms at the same time, supports read and write operations of coils, discrete inputs, r
/// egisters and input registers, and supports mask write function, which can be used as a system data exchange pool
/// </summary>
/// <remarks>
/// 可以基于本类实现一个功能复杂的modbus服务器，支持Modbus-Tcp，启动串口后，还支持Modbus-Rtu和Modbus-ASCII，会根据报文进行动态的适配。
/// 线圈，功能码对应01，05，15
/// 离散输入，功能码对应02，服务器写入离散输入的地址使用 x=2;100
/// 寄存器，功能码对应03，06，16
/// 输入寄存器，功能码对应04，输入寄存器在服务器写入使用地址 x=4;100
/// 掩码写入，功能码对应22，可以对字寄存器进行位操作
/// 特别说明1:  属性如果设置为 True 的话，则服务器为每一个站号（0-255）都创建一个数据区，客户端使用站号作为区分可以写入不同的数据区，服务器也可以读取不同数据区的数据，例如 s=2;100
/// 特别说明2: 如果多个modbus server使用485总线连接，那么属性  需要设置为 True
/// </remarks>
public class ModbusTcpServer : DeviceServer
{
	private Func<string, byte, OperateResult<string>> addressMapping = (string address, byte modbusCode) => OperateResult.CreateSuccessResult(address);

	private List<ModBusMonitorAddress> subscriptions;

	private SimpleHybirdLock subcriptionHybirdLock;

	private ModbusDataDict dictModbusDataPool;

	private byte station = 1;

	private bool stationDataIsolation = false;

	private IByteTransform byteTransformSelf = new RegularByteTransform(DataFormat.CDAB);

	private SoftBuffer fileBuffer = new SoftBuffer(65535);

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
	/// 获取或设置是否对站号进行检测，当服务器只有一个站号的时候，设置为True表示客户端请求站号和服务器不一致的时候，拒绝返回数据给客户端，反之，始终会返回数据给客户端。
	/// When the server has only one station number, setting True means that when the client requests that the station number is inconsistent with the server,
	/// it refuses to return data to the client, and vice versa, it will always return data to the client.
	/// </summary>
	public bool StationCheck { get; set; } = true;

	/// <summary>
	/// 获取或设置当前的TCP服务器是否使用modbus-rtu报文进行通信，如果设置为 True，那么客户端需要使用
	/// Get or set whether the current TCP server uses modbus-rtu messages for communication.
	/// If it is set to True, then the client needs to use
	/// </summary>
	/// <remarks>
	/// 需要注意的是，本属性设置为False时，客户端使用，否则，使用，不能混合使用
	/// </remarks>
	public bool UseModbusRtuOverTcp { get; set; }

	/// <summary>
	/// 获取或设置两次请求直接的延时时间，单位毫秒，默认是0，不发生延时，设置为20的话，可以有效防止有客户端疯狂进行请求而导致服务器的CPU占用率上升。
	/// Get or set the direct delay time of two requests, in milliseconds, the default is 0, no delay occurs, if it is set to 20,
	/// it can effectively prevent the client from making crazy requests and causing the server's CPU usage to increase.
	/// </summary>
	public int RequestDelayTime { get; set; }

	/// <summary>
	/// 获取或设置当前掩码写入的功能码是否激活状态，设置为 false 时，再执行写入位时，会通过读字，修改位，写字的方式来间接实现。
	/// When the function code is set to false, and then the write bit is executed, it will be indirectly implemented by reading, modifying the bit, and writing the word.
	/// </summary>
	public bool EnableWriteMaskCode { get; set; } = true;

	/// <summary>
	/// 获取或设置是否启动站点数据隔离功能，默认为 False，也即虚拟服务器模拟一个站点服务器，客户端使用正确的站号才能通信。
	/// 当设置为 True 时，虚拟服务器模式256个站点，无论客户端使用的什么站点，都能读取或是写入对应站点里去。服务器同时也可以访问任意站点自身的数据。
	/// Get or set whether to enable the site data isolation function, the default is False, that is, the virtual server simulates a site server, and the client can communicate with the correct site number.
	/// When set to True, 256 sites in virtual server mode, no matter what site the client uses, can read or write to the corresponding site.The server can also access any site's own data.
	/// </summary>
	/// <remarks>
	/// 当启动站号隔离之后，服务器访问自身的站号2的数据，地址写为 s=2;100
	/// </remarks>
	public bool StationDataIsolation
	{
		get
		{
			return stationDataIsolation;
		}
		set
		{
			stationDataIsolation = value;
			dictModbusDataPool.Set(value);
		}
	}

	/// <summary>
	/// 实例化一个Modbus Tcp及Rtu的服务器，支持数据读写操作
	/// </summary>
	public ModbusTcpServer()
	{
		dictModbusDataPool = new ModbusDataDict();
		subscriptions = new List<ModBusMonitorAddress>();
		subcriptionHybirdLock = new SimpleHybirdLock();
		base.ByteTransform = new RegularByteTransform(DataFormat.CDAB);
		base.WordLength = 1;
		base.SerialReceiveAtleastTime = 2000;
	}

	protected override byte[] SaveToBytes()
	{
		return dictModbusDataPool.GetModbusPool(station).SaveToBytes();
	}

	protected override void LoadFromBytes(byte[] content)
	{
		dictModbusDataPool.GetModbusPool(station).LoadFromBytes(content, 0);
	}

	/// <summary>
	/// 读取地址的线圈的通断情况
	/// </summary>
	/// <param name="address">起始地址，示例：&quot;100&quot;</param>
	/// <returns>True或是False</returns>
	public bool ReadCoil(string address)
	{
		byte b = (byte)HslHelper.ExtractParameter(ref address, "s", station);
		return dictModbusDataPool.GetModbusPool(b).ReadCoil(address);
	}

	/// <summary>
	/// 批量读取地址的线圈的通断情况
	/// </summary>
	/// <param name="address">起始地址，示例：&quot;100&quot;</param>
	/// <param name="length">读取长度</param>
	/// <returns>True或是False</returns>
	public bool[] ReadCoil(string address, ushort length)
	{
		byte b = (byte)HslHelper.ExtractParameter(ref address, "s", station);
		return dictModbusDataPool.GetModbusPool(b).ReadCoil(address, length);
	}

	/// <summary>
	/// 写入线圈的通断值
	/// </summary>
	/// <param name="address">起始地址，示例：&quot;100&quot;</param>
	/// <param name="data">是否通断</param>
	/// <returns>True或是False</returns>
	public void WriteCoil(string address, bool data)
	{
		byte b = (byte)HslHelper.ExtractParameter(ref address, "s", station);
		dictModbusDataPool.GetModbusPool(b).WriteCoil(address, data);
	}

	/// <summary>
	/// 写入线圈数组的通断值
	/// </summary>
	/// <param name="address">起始地址，示例：&quot;100&quot;</param>
	/// <param name="data">是否通断</param>
	/// <returns>True或是False</returns>
	public void WriteCoil(string address, bool[] data)
	{
		byte b = (byte)HslHelper.ExtractParameter(ref address, "s", station);
		dictModbusDataPool.GetModbusPool(b).WriteCoil(address, data);
	}

	/// <summary>
	/// 读取地址的离散线圈的通断情况
	/// </summary>
	/// <param name="address">起始地址，示例：&quot;100&quot;</param>
	/// <returns>True或是False</returns>
	public bool ReadDiscrete(string address)
	{
		byte b = (byte)HslHelper.ExtractParameter(ref address, "s", station);
		return dictModbusDataPool.GetModbusPool(b).ReadDiscrete(address);
	}

	/// <summary>
	/// 批量读取地址的离散线圈的通断情况
	/// </summary>
	/// <param name="address">起始地址，示例：&quot;100&quot;</param>
	/// <param name="length">读取长度</param>
	/// <returns>True或是False</returns>
	public bool[] ReadDiscrete(string address, ushort length)
	{
		byte b = (byte)HslHelper.ExtractParameter(ref address, "s", station);
		return dictModbusDataPool.GetModbusPool(b).ReadDiscrete(address, length);
	}

	/// <summary>
	/// 写入离散线圈的通断值
	/// </summary>
	/// <param name="address">起始地址，示例：&quot;100&quot;</param>
	/// <param name="data">是否通断</param>
	public void WriteDiscrete(string address, bool data)
	{
		byte b = (byte)HslHelper.ExtractParameter(ref address, "s", station);
		dictModbusDataPool.GetModbusPool(b).WriteDiscrete(address, data);
	}

	/// <summary>
	/// 写入离散线圈数组的通断值
	/// </summary>
	/// <param name="address">起始地址，示例：&quot;100&quot;</param>
	/// <param name="data">是否通断</param>
	public void WriteDiscrete(string address, bool[] data)
	{
		byte b = (byte)HslHelper.ExtractParameter(ref address, "s", station);
		dictModbusDataPool.GetModbusPool(b).WriteDiscrete(address, data);
	}

	/// <summary>
	/// 注册一个新的地址映射关系，注册地址映射关系后，就可以使用新的地址来读写Modbus数据了，通常用于其他的支持Modbus协议的PLC。
	/// After registering a new address mapping, you can use the new address to read and write Modbus data, which is usually used for other PLCs that support the Modbus protocol.
	/// </summary>
	/// <param name="mapping">地址映射关系信息</param>
	public void RegisteredAddressMapping(Func<string, byte, OperateResult<string>> mapping)
	{
		if (mapping == null)
		{
			mapping = (string address, byte modbusCode) => OperateResult.CreateSuccessResult(address);
		}
		addressMapping = mapping;
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
		byte b = (byte)HslHelper.ExtractParameter(ref address, "s", station);
		if (address.StartsWithNoCase("file="))
		{
			int num = HslHelper.ExtractParameter(ref address, "file", 0);
			ushort num2 = ushort.Parse(address);
			return OperateResult.CreateSuccessResult(fileBuffer.GetBytes(num2 * 2, length * 2));
		}
		OperateResult<string> operateResult = addressMapping(address, 3);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return dictModbusDataPool.GetModbusPool(b).Read(operateResult.Content, length);
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
		byte b = (byte)HslHelper.ExtractParameter(ref address, "s", station);
		if (address.StartsWithNoCase("file="))
		{
			int num = HslHelper.ExtractParameter(ref address, "file", 0);
			ushort num2 = ushort.Parse(address);
			fileBuffer.SetBytes(value, num2 * 2);
			return OperateResult.CreateSuccessResult();
		}
		OperateResult<string> operateResult = addressMapping(address, 3);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return dictModbusDataPool.GetModbusPool(b).Write(operateResult.Content, value);
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
		byte b = (byte)HslHelper.ExtractParameter(ref address, "s", station);
		OperateResult<string> operateResult = addressMapping(address, 1);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		return dictModbusDataPool.GetModbusPool(b).ReadBool(operateResult.Content, length, DataFormat);
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// 向线圈中写入bool数组，返回是否写入成功，如果富文本地址不指定，默认使用的功能码是 0x0F
	/// Write the bool array to the coil, and return whether the writing is successful. If the rich text address is not specified, the default function code is 0x0F.
	/// </summary>
	/// <param name="address">要写入的数据地址，比如&quot;1234&quot;</param>
	/// <param name="values">要写入的实际数组</param>
	/// <returns>返回写入结果</returns>
	public override OperateResult Write(string address, bool[] value)
	{
		byte b = (byte)HslHelper.ExtractParameter(ref address, "s", station);
		OperateResult<string> operateResult = addressMapping(address, 1);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		return dictModbusDataPool.GetModbusPool(b).Write(operateResult.Content, value);
	}

	/// <summary>
	/// 写入寄存器数据，指定字节数据
	/// </summary>
	/// <param name="address">起始地址，示例：&quot;100&quot;，如果是输入寄存器：&quot;x=4;100&quot;</param>
	/// <param name="high">高位数据</param>
	/// <param name="low">地位数据</param>
	public void Write(string address, byte high, byte low)
	{
		Write(address, new byte[2] { high, low });
	}

	protected override INetMessage GetNewNetMessage()
	{
		return UseModbusRtuOverTcp ? null : new ModbusTcpMessage();
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		if (receive.Length < 3)
		{
			return new OperateResult<byte[]>("Uknown Data：" + receive.ToHexString(' '));
		}
		if (RequestDelayTime > 0)
		{
			HslHelper.ThreadSleep(RequestDelayTime);
		}
		if (session.Communication is PipeSerialPort)
		{
			if (receive[0] == 58 && receive[1] >= 48 && receive[1] < 128)
			{
				OperateResult<byte[]> operateResult = ModbusInfo.TransAsciiPackCommandToCore(receive);
				if (!operateResult.IsSuccess)
				{
					return operateResult;
				}
				byte[] content = operateResult.Content;
				if (!CheckModbusMessageLegal(content))
				{
					return new OperateResult<byte[]>("Unlegal Data：" + receive.ToHexString(' '));
				}
				if (content[0] != 0 && content[0] != byte.MaxValue && !StationDataIsolation && StationCheck && station != content[0])
				{
					return new OperateResult<byte[]>("Station not match Modbus-Ascii : " + SoftBasic.GetAsciiStringRender(receive));
				}
				byte[] value = ModbusInfo.TransModbusCoreToAsciiPackCommand(ReadFromModbusCore(content));
				if (content[0] != 0)
				{
					return OperateResult.CreateSuccessResult(value);
				}
				byte[] value2 = null;
				return OperateResult.CreateSuccessResult(value2);
			}
			if (SoftCRC16.CheckCRC16(receive))
			{
				byte[] array = receive.RemoveLast(2);
				if (!CheckModbusMessageLegal(array))
				{
					return new OperateResult<byte[]>("Unlegal Data：" + receive.ToHexString(' '));
				}
				if (array[0] != 0 && array[0] != byte.MaxValue && !StationDataIsolation && StationCheck && station != array[0])
				{
					return new OperateResult<byte[]>("Station not match Modbus-rtu : " + receive.ToHexString(' '));
				}
				byte[] value3 = ModbusInfo.PackCommandToRtu(ReadFromModbusCore(array));
				if (array[0] != 0)
				{
					return OperateResult.CreateSuccessResult(value3);
				}
				byte[] value4 = null;
				return OperateResult.CreateSuccessResult(value4);
			}
			return new OperateResult<byte[]>("CRC Check Failed : " + receive.ToHexString(' '));
		}
		if (UseModbusRtuOverTcp)
		{
			if (receive[0] == 58 && receive[1] >= 48 && receive[1] < 128)
			{
				OperateResult<byte[]> operateResult2 = ModbusInfo.TransAsciiPackCommandToCore(receive);
				if (!operateResult2.IsSuccess)
				{
					return new OperateResult<byte[]>("ASCII Check Failed: " + operateResult2.Message + " Source: " + receive.ToHexString(' '));
				}
				if (!CheckModbusMessageLegal(operateResult2.Content))
				{
					return new OperateResult<byte[]>("Modbus Ascii message check failed ");
				}
				if (operateResult2.Content[0] != 0 && !StationDataIsolation && StationCheck && station != operateResult2.Content[0])
				{
					return new OperateResult<byte[]>($"Station not match Modbus-ascii, Need {station} actual {operateResult2.Content[0]}");
				}
				byte[] value5 = ModbusInfo.TransModbusCoreToAsciiPackCommand(ReadFromModbusCore(operateResult2.Content));
				if (operateResult2.Content[0] != 0)
				{
					return OperateResult.CreateSuccessResult(value5);
				}
				byte[] value6 = null;
				return OperateResult.CreateSuccessResult(value6);
			}
			if (!SoftCRC16.CheckCRC16(receive))
			{
				return new OperateResult<byte[]>("CRC Check Failed: " + receive.ToHexString(' '));
			}
			byte[] array2 = receive.RemoveLast(2);
			if (!CheckModbusMessageLegal(array2))
			{
				return new OperateResult<byte[]>("Modbus rtu message check failed ");
			}
			if (array2[0] != 0 && array2[0] != byte.MaxValue && !StationDataIsolation && StationCheck && station != array2[0])
			{
				return new OperateResult<byte[]>($"Station not match Modbus-rtu, Need {station} actual {array2[0]}");
			}
			byte[] value7 = ModbusInfo.PackCommandToRtu(ReadFromModbusCore(array2));
			if (array2[0] != 0)
			{
				return OperateResult.CreateSuccessResult(value7);
			}
			byte[] value8 = null;
			return OperateResult.CreateSuccessResult(value8);
		}
		if (!CheckModbusMessageLegal(receive.RemoveBegin(6)))
		{
			return new OperateResult<byte[]>("Modbus message check failed");
		}
		ushort id = (ushort)(receive[0] * 256 + receive[1]);
		if (receive[6] != 0 && receive[6] != byte.MaxValue && !StationDataIsolation && StationCheck && station != receive[6])
		{
			return new OperateResult<byte[]>("Station not match Modbus-tcp ");
		}
		byte[] value9 = ModbusInfo.PackCommandToTcp(ReadFromModbusCore(receive.RemoveBegin(6)), id);
		if (receive[6] == 0)
		{
			byte[] value10 = null;
			return OperateResult.CreateSuccessResult(value10);
		}
		return OperateResult.CreateSuccessResult(value9);
	}

	/// <summary>
	/// 创建特殊的功能标识，然后返回该信息
	/// Create a special feature ID and return this information
	/// </summary>
	/// <param name="modbusCore">modbus核心报文</param>
	/// <param name="error">错误码</param>
	/// <returns>携带错误码的modbus报文</returns>
	private byte[] CreateExceptionBack(byte[] modbusCore, byte error)
	{
		return new byte[3]
		{
			modbusCore[0],
			(byte)(modbusCore[1] + 128),
			error
		};
	}

	/// <summary>
	/// 创建返回消息
	/// Create return message
	/// </summary>
	/// <param name="modbusCore">modbus核心报文</param>
	/// <param name="content">返回的实际数据内容</param>
	/// <returns>携带内容的modbus报文</returns>
	private byte[] CreateReadBack(byte[] modbusCore, byte[] content)
	{
		return SoftBasic.SpliceArray<byte>(new byte[3]
		{
			modbusCore[0],
			modbusCore[1],
			(byte)content.Length
		}, content);
	}

	/// <summary>
	/// 创建写入成功的反馈信号
	/// Create feedback signal for successful write
	/// </summary>
	/// <param name="modbus">modbus核心报文</param>
	/// <returns>携带成功写入的信息</returns>
	private byte[] CreateWriteBack(byte[] modbus)
	{
		return modbus.SelectBegin(6);
	}

	private byte[] ReadCoilBack(byte[] modbus, string addressHead)
	{
		try
		{
			ushort num = byteTransformSelf.TransUInt16(modbus, 2);
			ushort num2 = byteTransformSelf.TransUInt16(modbus, 4);
			if (num + num2 > 65536)
			{
				return CreateExceptionBack(modbus, 2);
			}
			if (num2 > 2040)
			{
				return CreateExceptionBack(modbus, 3);
			}
			bool[] content = dictModbusDataPool.GetModbusPool(modbus[0]).ReadBool(addressHead + num, num2, DataFormat).Content;
			return CreateReadBack(modbus, SoftBasic.BoolArrayToByte(content));
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), StringResources.Language.ModbusTcpReadCoilException, ex);
			return CreateExceptionBack(modbus, 4);
		}
	}

	private byte[] ReadRegisterBack(byte[] modbus, string addressHead)
	{
		try
		{
			ushort num = byteTransformSelf.TransUInt16(modbus, 2);
			ushort num2 = byteTransformSelf.TransUInt16(modbus, 4);
			if (num + num2 > 65536)
			{
				return CreateExceptionBack(modbus, 2);
			}
			if (num2 > 127)
			{
				return CreateExceptionBack(modbus, 3);
			}
			byte[] content = dictModbusDataPool.GetModbusPool(modbus[0]).Read(addressHead + num, num2).Content;
			return CreateReadBack(modbus, content);
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), StringResources.Language.ModbusTcpReadRegisterException, ex);
			return CreateExceptionBack(modbus, 4);
		}
	}

	private byte[] ReadFileRecordBack(byte[] modbus)
	{
		try
		{
			int num = (modbus.Length - 3) / 7;
			MemoryStream memoryStream = new MemoryStream();
			for (int i = 0; i < num; i++)
			{
				ushort num2 = byteTransformSelf.TransUInt16(modbus, 4 + 7 * i);
				ushort num3 = byteTransformSelf.TransUInt16(modbus, 6 + 7 * i);
				ushort num4 = byteTransformSelf.TransUInt16(modbus, 8 + 7 * i);
				if (num4 * 2 + 1 > 255)
				{
					return CreateExceptionBack(modbus, 3);
				}
				memoryStream.WriteByte((byte)(num4 * 2 + 1));
				memoryStream.WriteByte(6);
				memoryStream.Write(fileBuffer.GetBytes(num3 * 2, num4 * 2));
				if (memoryStream.Length > 255)
				{
					return CreateExceptionBack(modbus, 3);
				}
			}
			return CreateReadBack(modbus, memoryStream.ToArray());
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), "ReadFileRecordBack", ex);
			return CreateExceptionBack(modbus, 4);
		}
	}

	private byte[] WriteFileRecordBack(byte[] modbus)
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream();
			ushort num3;
			for (int i = 3; i < modbus.Length; i += 7 + num3 * 2)
			{
				ushort num = byteTransformSelf.TransUInt16(modbus, i + 1);
				ushort num2 = byteTransformSelf.TransUInt16(modbus, i + 3);
				num3 = byteTransformSelf.TransUInt16(modbus, i + 5);
				OperateResult operateResult = Write($"file={num};{num2}", modbus.SelectMiddle(i + 7, num3 * 2));
				if (!operateResult.IsSuccess)
				{
					return CreateExceptionBack(modbus, 4);
				}
				memoryStream.Write(modbus.SelectMiddle(i, 7 + num3 * 2));
			}
			return CreateReadBack(modbus, memoryStream.ToArray());
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), "WriteFileRecordBack", ex);
			return CreateExceptionBack(modbus, 4);
		}
	}

	private byte[] ReadWriteRegisterBack(byte[] modbus, string addressHead)
	{
		try
		{
			byte[] modbus2 = new byte[6]
			{
				modbus[0],
				3,
				modbus[2],
				modbus[3],
				modbus[4],
				modbus[5]
			};
			byte[] array = ReadRegisterBack(modbus2, addressHead);
			if (array[1] > 128)
			{
				return CreateExceptionBack(modbus, array[2]);
			}
			byte[] modbus3 = SoftBasic.SpliceArray<byte>(new byte[2]
			{
				modbus[0],
				16
			}, modbus.RemoveBegin(6));
			byte[] array2 = WriteRegisterBack(modbus3);
			if (array2[1] > 128)
			{
				return CreateExceptionBack(modbus, array2[2]);
			}
			array[1] = modbus[1];
			return array;
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), StringResources.Language.ModbusTcpFunctionCodeReadWriteException, ex);
			return CreateExceptionBack(modbus, 4);
		}
	}

	private byte[] WriteOneCoilBack(byte[] modbus)
	{
		try
		{
			if (!base.EnableWrite)
			{
				return CreateExceptionBack(modbus, 4);
			}
			ushort num = byteTransformSelf.TransUInt16(modbus, 2);
			if (modbus[4] == byte.MaxValue && modbus[5] == 0)
			{
				dictModbusDataPool.GetModbusPool(modbus[0]).Write(num.ToString(), new bool[1] { true });
			}
			else if (modbus[4] == 0 && modbus[5] == 0)
			{
				dictModbusDataPool.GetModbusPool(modbus[0]).Write(num.ToString(), new bool[1]);
			}
			return CreateWriteBack(modbus);
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), StringResources.Language.ModbusTcpWriteCoilException, ex);
			return CreateExceptionBack(modbus, 4);
		}
	}

	private byte[] WriteOneRegisterBack(byte[] modbus)
	{
		try
		{
			if (!base.EnableWrite)
			{
				return CreateExceptionBack(modbus, 4);
			}
			ushort address = byteTransformSelf.TransUInt16(modbus, 2);
			short content = ReadInt16(address.ToString()).Content;
			dictModbusDataPool.GetModbusPool(modbus[0]).Write(address.ToString(), new byte[2]
			{
				modbus[4],
				modbus[5]
			});
			short content2 = ReadInt16(address.ToString()).Content;
			OnRegisterBeforWrite(address, content, content2);
			return CreateWriteBack(modbus);
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), StringResources.Language.ModbusTcpWriteRegisterException, ex);
			return CreateExceptionBack(modbus, 4);
		}
	}

	private byte[] WriteCoilsBack(byte[] modbus)
	{
		try
		{
			if (!base.EnableWrite)
			{
				return CreateExceptionBack(modbus, 4);
			}
			ushort num = byteTransformSelf.TransUInt16(modbus, 2);
			ushort num2 = byteTransformSelf.TransUInt16(modbus, 4);
			if (num + num2 > 65536)
			{
				return CreateExceptionBack(modbus, 2);
			}
			if (num2 > 2040)
			{
				return CreateExceptionBack(modbus, 3);
			}
			dictModbusDataPool.GetModbusPool(modbus[0]).Write(num.ToString(), modbus.RemoveBegin(7).ToBoolArray(num2));
			return CreateWriteBack(modbus);
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), StringResources.Language.ModbusTcpWriteCoilException, ex);
			return CreateExceptionBack(modbus, 4);
		}
	}

	private byte[] WriteRegisterBack(byte[] modbus)
	{
		try
		{
			if (!base.EnableWrite)
			{
				return CreateExceptionBack(modbus, 4);
			}
			ushort num = byteTransformSelf.TransUInt16(modbus, 2);
			ushort num2 = byteTransformSelf.TransUInt16(modbus, 4);
			if (num + num2 > 65536)
			{
				return CreateExceptionBack(modbus, 2);
			}
			if (num2 > 127)
			{
				return CreateExceptionBack(modbus, 3);
			}
			byte[] content = dictModbusDataPool.GetModbusPool(modbus[0]).Read(num.ToString(), num2).Content;
			dictModbusDataPool.GetModbusPool(modbus[0]).Write(num.ToString(), modbus.RemoveBegin(7).SelectBegin(num2 * 2));
			MonitorAddress[] array = new MonitorAddress[num2];
			for (ushort num3 = 0; num3 < num2; num3++)
			{
				short valueOrigin = base.ByteTransform.TransInt16(content, num3 * 2);
				short valueNew = base.ByteTransform.TransInt16(modbus, num3 * 2 + 7);
				array[num3] = new MonitorAddress
				{
					Address = (ushort)(num + num3),
					ValueOrigin = valueOrigin,
					ValueNew = valueNew
				};
			}
			for (int i = 0; i < array.Length; i++)
			{
				OnRegisterBeforWrite(array[i].Address, array[i].ValueOrigin, array[i].ValueNew);
			}
			return CreateWriteBack(modbus);
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), StringResources.Language.ModbusTcpWriteRegisterException, ex);
			return CreateExceptionBack(modbus, 4);
		}
	}

	private byte[] WriteMaskRegisterBack(byte[] modbus)
	{
		try
		{
			if (!base.EnableWrite)
			{
				return CreateExceptionBack(modbus, 4);
			}
			if (!EnableWriteMaskCode)
			{
				return CreateExceptionBack(modbus, 1);
			}
			ushort address = byteTransformSelf.TransUInt16(modbus, 2);
			int num = base.ByteTransform.TransUInt16(modbus, 4);
			int num2 = base.ByteTransform.TransUInt16(modbus, 6);
			int content = ReadInt16($"s={modbus[0]};" + address).Content;
			short num3 = (short)((content & num) | num2);
			Write($"s={modbus[0]};" + address, num3);
			MonitorAddress monitorAddress = new MonitorAddress
			{
				Address = address,
				ValueOrigin = (short)content,
				ValueNew = num3
			};
			OnRegisterBeforWrite(monitorAddress.Address, monitorAddress.ValueOrigin, monitorAddress.ValueNew);
			return modbus;
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), StringResources.Language.ModbusTcpWriteRegisterException, ex);
			return CreateExceptionBack(modbus, 4);
		}
	}

	/// <summary>
	/// 新增一个数据监视的任务，针对的是寄存器地址的数据
	/// Added a data monitoring task for data at register addresses
	/// </summary>
	/// <param name="monitor">监视地址对象</param>
	public void AddSubcription(ModBusMonitorAddress monitor)
	{
		subcriptionHybirdLock.Enter();
		subscriptions.Add(monitor);
		subcriptionHybirdLock.Leave();
	}

	/// <summary>
	/// 移除一个数据监视的任务
	/// Remove a data monitoring task
	/// </summary>
	/// <param name="monitor">监视地址对象</param>
	public void RemoveSubcrption(ModBusMonitorAddress monitor)
	{
		subcriptionHybirdLock.Enter();
		subscriptions.Remove(monitor);
		subcriptionHybirdLock.Leave();
	}

	/// <summary>
	/// 在数据变更后，进行触发是否产生订阅
	/// Whether to generate a subscription after triggering data changes
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="before">修改之前的数</param>
	/// <param name="after">修改之后的数</param>
	private void OnRegisterBeforWrite(ushort address, short before, short after)
	{
		subcriptionHybirdLock.Enter();
		for (int i = 0; i < subscriptions.Count; i++)
		{
			if (subscriptions[i].Address == address)
			{
				subscriptions[i].SetValue(after);
				if (before != after)
				{
					subscriptions[i].SetChangeValue(before, after);
				}
			}
		}
		subcriptionHybirdLock.Leave();
	}

	/// <summary>
	/// 检测当前的Modbus接收的指定是否是合法的
	/// Check if the current Modbus datad designation is valid
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <returns>是否合格</returns>
	private bool CheckModbusMessageLegal(byte[] buffer)
	{
		bool success = false;
		switch (buffer[1])
		{
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
			success = buffer.Length == 6;
			break;
		case 15:
		case 16:
			success = buffer.Length > 6 && buffer[6] == buffer.Length - 7;
			break;
		case 22:
			success = buffer.Length == 8;
			break;
		case 20:
			success = buffer[2] + 3 == buffer.Length;
			break;
		case 21:
			success = buffer[2] + 3 == buffer.Length;
			break;
		default:
			success = true;
			break;
		}
		if (!success)
		{
			base.LogNet?.WriteError(ToString(), "Receive Nosense Modbus Msg : " + buffer.ToHexString(' '));
		}
		return success;
	}

	/// <summary>
	/// Modbus核心数据交互方法，允许重写自己来实现，报文只剩下核心的Modbus信息，去除了MPAB报头信息
	/// The Modbus core data interaction method allows you to rewrite it to achieve the message.
	/// Only the core Modbus information is left in the message, and the MPAB header information is removed.
	/// </summary>
	/// <param name="modbusCore">核心的Modbus报文</param>
	/// <returns>进行数据交互之后的结果</returns>
	protected virtual byte[] ReadFromModbusCore(byte[] modbusCore)
	{
		return modbusCore[1] switch
		{
			1 => ReadCoilBack(modbusCore, string.Empty), 
			2 => ReadCoilBack(modbusCore, "x=2;"), 
			3 => ReadRegisterBack(modbusCore, string.Empty), 
			4 => ReadRegisterBack(modbusCore, "x=4;"), 
			5 => WriteOneCoilBack(modbusCore), 
			6 => WriteOneRegisterBack(modbusCore), 
			15 => WriteCoilsBack(modbusCore), 
			16 => WriteRegisterBack(modbusCore), 
			22 => WriteMaskRegisterBack(modbusCore), 
			23 => ReadWriteRegisterBack(modbusCore, string.Empty), 
			20 => ReadFileRecordBack(modbusCore), 
			21 => WriteFileRecordBack(modbusCore), 
			_ => CreateExceptionBack(modbusCore, 1), 
		};
	}

	protected override bool CheckSerialReceiveDataComplete(byte[] buffer, int dataLength)
	{
		if (dataLength > 5)
		{
			if (ModbusInfo.CheckAsciiReceiveDataComplete(buffer, dataLength))
			{
				return true;
			}
			if (ModbusInfo.CheckServerRtuReceiveDataComplete(buffer.SelectBegin(dataLength)))
			{
				return true;
			}
		}
		return false;
	}

	protected override string GetLogTextFromBinary(PipeSession session, byte[] content)
	{
		if (session.Communication is PipeSerialPort)
		{
			if (content[0] == 58 && content[1] >= 48 && content[1] < 128)
			{
				return "[Ascii] " + SoftBasic.GetAsciiStringRender(content);
			}
			return "[Rtu] " + content.ToHexString(' ');
		}
		if (session.Communication is PipeTcpNet && content != null && content.Length > 2 && content[0] == 58 && content[1] >= 48 && content[1] < 128)
		{
			return "[Ascii] " + SoftBasic.GetAsciiStringRender(content);
		}
		return base.GetLogTextFromBinary(session, content);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			subcriptionHybirdLock?.Dispose();
			subscriptions?.Clear();
			dictModbusDataPool?.Dispose();
			GC.Collect();
		}
		base.Dispose(disposing);
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
		return ByteTransformHelper.GetResultFromBytes(Read(address, (ushort)(length * base.WordLength * 2)), (byte[] m) => transform.TransInt32(m, 0, length));
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
		return ByteTransformHelper.GetResultFromBytes(Read(address, (ushort)(length * base.WordLength * 2)), (byte[] m) => transform.TransUInt32(m, 0, length));
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
		return ByteTransformHelper.GetResultFromBytes(Read(address, (ushort)(length * base.WordLength * 2)), (byte[] m) => transform.TransSingle(m, 0, length));
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
		return ByteTransformHelper.GetResultFromBytes(Read(address, (ushort)(length * base.WordLength * 4)), (byte[] m) => transform.TransInt64(m, 0, length));
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
		return ByteTransformHelper.GetResultFromBytes(Read(address, (ushort)(length * base.WordLength * 4)), (byte[] m) => transform.TransUInt64(m, 0, length));
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
		return ByteTransformHelper.GetResultFromBytes(Read(address, (ushort)(length * base.WordLength * 4)), (byte[] m) => transform.TransDouble(m, 0, length));
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
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, (ushort)(length * base.WordLength * 2)), (byte[] m) => transform.TransInt32(m, 0, length));
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
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, (ushort)(length * base.WordLength * 2)), (byte[] m) => transform.TransUInt32(m, 0, length));
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
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, (ushort)(length * base.WordLength * 2)), (byte[] m) => transform.TransSingle(m, 0, length));
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
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, (ushort)(length * base.WordLength * 4)), (byte[] m) => transform.TransInt64(m, 0, length));
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
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, (ushort)(length * base.WordLength * 4)), (byte[] m) => transform.TransUInt64(m, 0, length));
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
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, (ushort)(length * base.WordLength * 4)), (byte[] m) => transform.TransDouble(m, 0, length));
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

	public override string ToString()
	{
		return $"ModbusTcpServer[{base.Port}]";
	}
}
