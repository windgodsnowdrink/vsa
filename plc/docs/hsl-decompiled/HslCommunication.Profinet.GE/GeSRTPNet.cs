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
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.GE;

/// <summary>
/// Ge(通用电气)集团的SRTP协议实现的客户端，支持 I,Q,M,T,SA,SB,SC,S,G 的位和字节读写，支持 AI,AQ,R 的字读写操作，支持读取PLC时间，程序名操作
/// The client implemented by Ge (General Electric) Group's SRTP protocol, supports I, Q, M, T, SA, SB, SC, S, G bit and byte read and write,
/// supports AI, AQ, R word read and write Operation, support reading PLC time, program name operation
/// </summary>
/// <remarks>
/// PLC的端口号默认18245，其中读取R，AI，AQ寄存器的原始字节时，传入的长度参数为字节长度。
/// 对其他寄存器而言，M1-M8的位读取，相当于 M1的字节读取。写入也是同理。
/// </remarks>
public class GeSRTPNet : DeviceTcpNet
{
	private SoftIncrementCount incrementCount = new SoftIncrementCount(65535L, 0L);

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public GeSRTPNet()
	{
		base.ByteTransform = new RegularByteTransform();
		base.WordLength = 2;
	}

	/// <summary>
	/// 指定IP地址和端口号来实例化一个对象
	/// Specify the IP address and port number to instantiate an object
	/// </summary>
	/// <param name="ipAddress">Ip地址</param>
	/// <param name="port">端口号</param>
	public GeSRTPNet(string ipAddress, int port = 18245)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	public override byte[] PackCommandWithHeader(byte[] command)
	{
		if (command != null && command.Length > 3)
		{
			long currentValue = incrementCount.GetCurrentValue();
			command[2] = BitConverter.GetBytes(currentValue)[0];
			command[3] = BitConverter.GetBytes(currentValue)[1];
		}
		return command;
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return GeHelper.ExtraResponseContent(response);
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new GeSRTPMessage();
	}

	protected override OperateResult InitializationOnConnect()
	{
		incrementCount.ResetCurrentValue();
		OperateResult<byte[]> operateResult = ReadFromCoreServer(CommunicationPipe, new byte[56], hasResponseData: true, usePackAndUnpack: true);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override async Task<OperateResult> InitializationOnConnectAsync()
	{
		incrementCount.ResetCurrentValue();
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(CommunicationPipe, new byte[56], hasResponseData: true, usePackAndUnpack: true).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override OperateResult<List<byte[]>> BuildDeviceReadCommand(string address, ushort length, bool isBit)
	{
		if (!isBit)
		{
			return GeHelper.BuildReadCommand(address, length, isBit: false);
		}
		return base.BuildDeviceReadCommand(address, length, isBit);
	}

	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, byte[] value)
	{
		return GeHelper.BuildWriteCommand(address, value);
	}

	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, bool[] value)
	{
		return GeHelper.BuildWriteCommand(address, value);
	}

	[HslMqttApi("ReadByte", "")]
	/// <summary>
	/// 根据指定的地址来读取一个字节的数据，按照字节为单位，例如 M1 字节，就是指 M1-M8 位组成的字节，M2 字节就是 M9-M16 组成的字节。不支持对 AI,AQ,R 寄存器的字节读取
	/// A byte of data is read according to the specified address, and the unit is byte. For example, M1 byte refers to a byte composed of M1-M8 bits,
	/// and M2 byte is a byte composed of M9-M16. Does not support byte reading of AI, AQ, R registers
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>带有成功标识的  数据</returns>
	public OperateResult<byte> ReadByte(string address)
	{
		OperateResult<GeSRTPAddress> operateResult = GeSRTPAddress.ParseFrom(address, 1, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte>(operateResult);
		}
		if (operateResult.Content.DataCode == 10 || operateResult.Content.DataCode == 12 || operateResult.Content.DataCode == 8)
		{
			return new OperateResult<byte>(StringResources.Language.GeSRTPNotSupportByteReadWrite);
		}
		return ByteTransformHelper.GetResultFromArray(Read(address, 1));
	}

	[HslMqttApi("WriteByte", "")]
	/// <summary>
	/// 向PLC中写入byte数据，返回是否写入成功
	/// Write byte data to PLC and return whether the writing is successful
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,mM100,Q100,DB20.100</param>
	/// <param name="value">byte数据 -&gt; Byte data</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	public OperateResult Write(string address, byte value)
	{
		OperateResult<GeSRTPAddress> operateResult = GeSRTPAddress.ParseFrom(address, 1, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte>(operateResult);
		}
		if (operateResult.Content.DataCode == 10 || operateResult.Content.DataCode == 12 || operateResult.Content.DataCode == 8)
		{
			return new OperateResult<byte>(StringResources.Language.GeSRTPNotSupportByteReadWrite);
		}
		return Write(address, new byte[1] { value });
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 按照位为单位，批量从指定的地址里读取 bool 数组数据，不支持 AI，AQ，R 地址类型，地址比如从1开始，例如 I1,Q1,M1,T1,SA1,SB1,SC1,S1,G1
	/// In units of bits, read bool array data from the specified address in batches. AI, AQ,
	/// R address types are not supported. For example, the address starts from 1, such as I1, Q1, M1, T1, SA1, SB1, SC1, S1, G1
	/// </summary>
	/// <param name="address">PLC的地址信息，例如 M1, G1</param>
	/// <param name="length">读取的长度信息，按照位为单位</param>
	/// <returns>包含是否读取成功的bool数组</returns>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		OperateResult<GeSRTPAddress> operateResult = GeSRTPAddress.ParseFrom(address, length, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		OperateResult<byte[]> operateResult2 = GeHelper.BuildReadCommand(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult2);
		}
		OperateResult<byte[]> operateResult3 = ReadFromCoreServer(operateResult2.Content);
		if (!operateResult3.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult3);
		}
		return OperateResult.CreateSuccessResult(operateResult3.Content.ToBoolArray().SelectMiddle(operateResult.Content.AddressStart % 8, length));
	}

	/// <summary>
	/// 根据指定的地址来读取一个字节的数据，按照字节为单位，例如 M1 字节，就是指 M1-M8 位组成的字节，M2 字节就是 M9-M16 组成的字节。不支持对 AI,AQ,R 寄存器的字节读取
	/// A byte of data is read according to the specified address, and the unit is byte. For example, M1 byte refers to a byte composed of M1-M8 bits,
	/// and M2 byte is a byte composed of M9-M16. Does not support byte reading of AI, AQ, R registers
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>带有成功标识的  数据</returns>
	public async Task<OperateResult<byte>> ReadByteAsync(string address)
	{
		OperateResult<GeSRTPAddress> analysis = GeSRTPAddress.ParseFrom(address, 1, isBit: true);
		if (!analysis.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte>(analysis);
		}
		if (analysis.Content.DataCode == 10 || analysis.Content.DataCode == 12 || analysis.Content.DataCode == 8)
		{
			return new OperateResult<byte>(StringResources.Language.GeSRTPNotSupportByteReadWrite);
		}
		return ByteTransformHelper.GetResultFromArray(await ReadAsync(address, 1));
	}

	/// <summary>
	/// 向PLC中写入byte数据，返回是否写入成功
	/// Write byte data to PLC and return whether the writing is successful
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,mM100,Q100,DB20.100</param>
	/// <param name="value">byte数据 -&gt; Byte data</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	public async Task<OperateResult> WriteAsync(string address, byte value)
	{
		OperateResult<GeSRTPAddress> analysis = GeSRTPAddress.ParseFrom(address, 1, isBit: true);
		if (!analysis.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte>(analysis);
		}
		if (analysis.Content.DataCode == 10 || analysis.Content.DataCode == 12 || analysis.Content.DataCode == 8)
		{
			return new OperateResult<byte>(StringResources.Language.GeSRTPNotSupportByteReadWrite);
		}
		return await WriteAsync(address, new byte[1] { value });
	}

	/// <summary>
	/// 按照位为单位，批量从指定的地址里读取 bool 数组数据，不支持 AI，AQ，R 地址类型，地址比如从1开始，例如 I1,Q1,M1,T1,SA1,SB1,SC1,S1,G1
	/// In units of bits, read bool array data from the specified address in batches. AI, AQ,
	/// R address types are not supported. For example, the address starts from 1, such as I1, Q1, M1, T1, SA1, SB1, SC1, S1, G1
	/// </summary>
	/// <param name="address">PLC的地址信息，例如 M1, G1</param>
	/// <param name="length">读取的长度信息，按照位为单位</param>
	/// <returns>包含是否读取成功的bool数组</returns>
	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		OperateResult<GeSRTPAddress> analysis = GeSRTPAddress.ParseFrom(address, length, isBit: true);
		if (!analysis.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(analysis);
		}
		OperateResult<byte[]> build = GeHelper.BuildReadCommand(analysis.Content);
		if (!build.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(build);
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(build.Content);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(read);
		}
		return OperateResult.CreateSuccessResult(read.Content.ToBoolArray().SelectMiddle(analysis.Content.AddressStart % 8, length));
	}

	[HslMqttApi(Description = "Read the current time of the PLC")]
	/// <summary>
	/// [商业授权] 读取PLC当前的时间，这个时间可能是不包含时区的，需要自己转换成本地的时间。
	/// [Authorization] Read the current time of the PLC, this time may not include the time zone, you need to convert the local time yourself.
	/// </summary>
	/// <returns>包含是否成功的时间信息</returns>
	public OperateResult<DateTime> ReadPLCTime()
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<DateTime>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> operateResult = GeHelper.BuildReadCoreCommand(37, new byte[5] { 0, 0, 0, 2, 0 });
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<DateTime>(operateResult);
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<DateTime>(operateResult2);
		}
		return GeHelper.ExtraDateTime(operateResult2.Content);
	}

	[HslMqttApi(Description = "Read the name of the current program of the PLC")]
	/// <summary>
	/// [商业授权] 读取PLC当前的程序的名称
	/// [Authorization] Read the name of the current program of the PLC
	/// </summary>
	/// <returns>包含是否成的程序名称信息</returns>
	public OperateResult<string> ReadProgramName()
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<string>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> operateResult = GeHelper.BuildReadCoreCommand(1, new byte[5] { 0, 0, 0, 2, 0 });
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult2);
		}
		return GeHelper.ExtraProgramName(operateResult2.Content);
	}

	/// <summary>
	/// [商业授权] 读取PLC当前的时间，这个时间可能是不包含时区的，需要自己转换成本地的时间。
	/// [Authorization] Read the current time of the PLC, this time may not include the time zone, you need to convert the local time yourself.
	/// </summary>
	/// <returns>包含是否成功的时间信息</returns>
	public async Task<OperateResult<DateTime>> ReadPLCTimeAsync()
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<DateTime>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> build = GeHelper.BuildReadCoreCommand(37, new byte[5] { 0, 0, 0, 2, 0 });
		if (!build.IsSuccess)
		{
			return OperateResult.CreateFailedResult<DateTime>(build);
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(build.Content);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<DateTime>(read);
		}
		return GeHelper.ExtraDateTime(read.Content);
	}

	/// <summary>
	/// [商业授权] 读取PLC当前的程序的名称
	/// [Authorization] Read the name of the current program of the PLC
	/// </summary>
	/// <returns>包含是否成的程序名称信息</returns>
	public async Task<OperateResult<string>> ReadProgramNameAsync()
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<string>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> build = GeHelper.BuildReadCoreCommand(1, new byte[5] { 0, 0, 0, 2, 0 });
		if (!build.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(build);
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(build.Content);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		return GeHelper.ExtraProgramName(read.Content);
	}

	public override string ToString()
	{
		return $"GeSRTPNet[{IpAddress}:{Port}]";
	}
}
