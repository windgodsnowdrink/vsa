// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Collections.Generic;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.ModBus;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.XINJE;

/// <summary>
/// 信捷内部的TCP信息，该协议是信捷基于modbus协议扩展而来，支持更多的地址类型，以及更广泛的地址范围。具体参考Demo界面信息。
/// The TCP information inside Xinjie, which is extended by Xinjie based on the modbus protocol,
/// supports more address types and a wider range of addresses. For details, refer to the Demo UI.
/// </summary>
public class XinJEInternalNet : DeviceTcpNet
{
	private byte station = 1;

	private readonly SoftIncrementCount softIncrementCount;

	/// <summary>
	/// 获取或者重新修改服务器的默认站号信息，当然，你可以再读写的时候动态指定，参见备注
	/// Get or modify the default station number information of the server. Of course, you can specify it dynamically when reading and writing, see note
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

	public SoftIncrementCount MessageId => softIncrementCount;

	/// <summary>
	/// 实例化一个XINJE-Tcp协议的客户端对象
	/// Instantiate a client object of the Modbus-Tcp protocol
	/// </summary>
	public XinJEInternalNet()
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
	public XinJEInternalNet(string ipAddress, int port = 502, byte station = 1)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
		this.station = station;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new ModbusTcpMessage();
	}

	public override byte[] PackCommandWithHeader(byte[] command)
	{
		return ModbusInfo.PackCommandToTcp(command, (ushort)softIncrementCount.GetCurrentValue());
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(response));
	}

	private OperateResult<bool[]> ExtraBoolArray(OperateResult<byte[]> read, ushort length)
	{
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(read);
		}
		return OperateResult.CreateSuccessResult(read.Content.ToBoolArray().SelectBegin(length));
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <remarks>
	/// 地址支持 D100, SD100, TD100, CD100, HD100, FD100, ETD100, HTD100, HCD100, HSD100, 各自的地址范围取决于实际PLC的范围，比如D的地址在XLH型号上可达 0~499999
	/// Address support D100, SD100, TD100, CD100, HD100, FD100, ETD100, HTD100, HCD100, HSD100, the respective address range depends on the actual PLC range, for example, the address of D can reach 0~499999 on the XLH model
	/// </remarks>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<List<byte[]>> operateResult = XinJEHelper.BuildReadCommand(Station, address, length, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return ReadFromCoreServer(operateResult.Content);
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <remarks>
	/// 地址支持 M100, X100, Y100, SM100, T100, C100, HM100, HS100, HT100, HSC100 各自的地址范围取决于实际PLC的范围，比如M的地址在XLH型号上可达 0~199999
	/// The address supports M100, X100, Y100, SM100, T100, C100, HM100, HS100, HT100, HSC100, the respective address range depends on the actual PLC range, for example, the address of M can reach 0~199999 on the XLH model
	/// </remarks>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		if (address.Contains("."))
		{
			return HslHelper.ReadBool(this, address, length, 16, reverseByWord: true);
		}
		OperateResult<List<byte[]>> operateResult = XinJEHelper.BuildReadCommand(Station, address, length, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		return ExtraBoolArray(ReadFromCoreServer(operateResult.Content), length);
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <remarks>
	/// 地址支持 D100, SD100, TD100, CD100, HD100, FD100, ETD100, HTD100, HCD100, HSD100,  各自的地址范围取决于实际PLC的范围，比如D的地址在XLH型号上可达 0~499999
	/// Address support D100, SD100, TD100, CD100, HD100, FD100, ETD100, HTD100, HCD100, HSD100,  the respective address range depends on the actual PLC range, for example, the address of D can reach 0~499999 on the XLH model
	/// </remarks>
	public override OperateResult Write(string address, byte[] value)
	{
		OperateResult<byte[]> operateResult = XinJEHelper.BuildWriteWordCommand(Station, address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return ReadFromCoreServer(operateResult.Content);
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <remarks>
	/// 地址支持 M100, Y100, SM100, T100, C100, 各自的地址范围取决于实际PLC的范围，比如M的地址在XLH型号上可达 0~199999
	/// The address supports M100, Y100, SM100, T100, C100, the respective address range depends on the actual PLC range, for example, the address of M can reach 0~199999 on the XLH model
	/// </remarks>
	public override OperateResult Write(string address, bool[] value)
	{
		OperateResult<byte[]> operateResult = XinJEHelper.BuildWriteBoolCommand(Station, address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return ReadFromCoreServer(operateResult.Content);
	}

	/// <remarks>
	/// 地址支持 D100, SD100, TD100, CD100, HD100, FD100, ETD100, HTD100, HCD100, HSD100, 各自的地址范围取决于实际PLC的范围，比如D的地址在XLH型号上可达 0~499999
	/// Address support D100, SD100, TD100, CD100, HD100, FD100, ETD100, HTD100, HCD100, HSD100, the respective address range depends on the actual PLC range, for example, the address of D can reach 0~499999 on the XLH model
	/// </remarks>
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		OperateResult<List<byte[]>> command = XinJEHelper.BuildReadCommand(Station, address, length, isBit: false);
		if (!command.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(command);
		}
		return await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <remarks>
	/// 地址支持 M100, X100, Y100, SM100, T100, C100, HM100, HS100, HT100, HSC100 各自的地址范围取决于实际PLC的范围，比如M的地址在XLH型号上可达 0~199999
	/// The address supports M100, X100, Y100, SM100, T100, C100, HM100, HS100, HT100, HSC100, the respective address range depends on the actual PLC range, for example, the address of M can reach 0~199999 on the XLH model
	/// </remarks>
	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		if (address.Contains("."))
		{
			return await HslHelper.ReadBoolAsync(this, address, length, 16, reverseByWord: true).ConfigureAwait(continueOnCapturedContext: false);
		}
		OperateResult<List<byte[]>> command = XinJEHelper.BuildReadCommand(Station, address, length, isBit: true);
		if (!command.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(command);
		}
		return ExtraBoolArray(await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false), length);
	}

	/// <remarks>
	/// 地址支持 D100, SD100, TD100, CD100, HD100, FD100, ETD100, HTD100, HCD100, HSD100,  各自的地址范围取决于实际PLC的范围，比如D的地址在XLH型号上可达 0~499999
	/// Address support D100, SD100, TD100, CD100, HD100, FD100, ETD100, HTD100, HCD100, HSD100,  the respective address range depends on the actual PLC range, for example, the address of D can reach 0~499999 on the XLH model
	/// </remarks>
	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		OperateResult<byte[]> command = XinJEHelper.BuildWriteWordCommand(Station, address, value);
		if (!command.IsSuccess)
		{
			return command;
		}
		return await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <remarks>
	/// 地址支持 M100, Y100, SM100, T100, C100, 各自的地址范围取决于实际PLC的范围，比如M的地址在XLH型号上可达 0~199999
	/// The address supports M100, Y100, SM100, T100, C100, the respective address range depends on the actual PLC range, for example, the address of M can reach 0~199999 on the XLH model
	/// </remarks>
	public override async Task<OperateResult> WriteAsync(string address, bool[] value)
	{
		OperateResult<byte[]> command = XinJEHelper.BuildWriteBoolCommand(Station, address, value);
		if (!command.IsSuccess)
		{
			return command;
		}
		return await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override string ToString()
	{
		return $"XinJEInternalNet[{IpAddress}:{Port}]";
	}
}
