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

namespace HslCommunication.Profinet.Siemens;

/// <summary>
/// 使用了Fetch/Write协议来和西门子进行通讯，该种方法需要在PLC侧进行一些配置
/// Using the Fetch/write protocol to communicate with Siemens, this method requires some configuration on the PLC side
/// </summary>
/// <remarks>
/// 配置的参考文章地址：https://www.cnblogs.com/dathlin/p/8685855.html
/// 与S7协议相比较而言，本协议不支持对单个的点位的读写操作。如果读取M100.0，需要读取M100的值，然后进行提取位数据。
/// 如果需要写入位地址的数据，可以读取plc的byte值，然后进行与或非，然后写入到plc之中。
/// </remarks>
public class SiemensFetchWriteNet : DeviceTcpNet
{
	/// <summary>
	/// 实例化一个西门子的Fetch/Write协议的通讯对象
	/// Instantiate a communication object for a Siemens Fetch/write protocol
	/// </summary>
	public SiemensFetchWriteNet()
	{
		base.WordLength = 2;
		base.ByteTransform = new ReverseBytesTransform();
	}

	/// <summary>
	/// 实例化一个西门子的Fetch/Write协议的通讯对象
	/// Instantiate a communication object for a Siemens Fetch/write protocol
	/// </summary>
	/// <param name="ipAddress">PLC的Ip地址 -&gt; Specify IP Address</param>
	/// <param name="port">PLC的端口 -&gt; Specify IP Port</param>
	public SiemensFetchWriteNet(string ipAddress, int port)
	{
		base.WordLength = 2;
		IpAddress = ipAddress;
		Port = port;
		base.ByteTransform = new ReverseBytesTransform();
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new FetchWriteMessage();
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，T100，C100，以字节为单位
	/// Read data from PLC, address format I100,Q100,DB20.100,M100,T100,C100, in bytes
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100，T100，C100 -&gt; Starting address, formatted as I100,M100,Q100,DB20.100,T100,C100</param>
	/// <param name="length">读取的数量，以字节为单位 -&gt; The number of reads, in bytes</param>
	/// <returns>带有成功标志的字节信息 -&gt; Byte information with a success flag</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<byte[]> operateResult = BuildReadCommand(address, length);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		OperateResult operateResult3 = CheckResponseContent(operateResult2.Content);
		if (!operateResult3.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult3);
		}
		return OperateResult.CreateSuccessResult(SoftBasic.ArrayRemoveBegin(operateResult2.Content, 16));
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 将数据写入到PLC数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位
	/// Writes data to the PLC data, in the address format i100,q100,db20.100,m100, in bytes
	/// </summary>
	/// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -&gt; Starting address, formatted as M100,I100,Q100,DB1.100</param>
	/// <param name="value">要写入的实际数据 -&gt; The actual data to write</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		OperateResult<byte[]> operateResult = BuildWriteCommand(address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		OperateResult operateResult3 = CheckResponseContent(operateResult2.Content);
		if (!operateResult3.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult3);
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，T100，C100，以字节为单位
	/// Read data from PLC, address format I100,Q100,DB20.100,M100,T100,C100, in bytes
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100，T100，C100 -&gt; Starting address, formatted as I100,M100,Q100,DB20.100,T100,C100</param>
	/// <param name="length">读取的数量，以字节为单位 -&gt; The number of reads, in bytes</param>
	/// <returns>带有成功标志的字节信息 -&gt; Byte information with a success flag</returns>
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		OperateResult<byte[]> command = BuildReadCommand(address, length);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content);
		if (!read.IsSuccess)
		{
			return read;
		}
		OperateResult check = CheckResponseContent(read.Content);
		if (!check.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(check);
		}
		return OperateResult.CreateSuccessResult(SoftBasic.ArrayRemoveBegin(read.Content, 16));
	}

	/// <summary>
	/// 将数据写入到PLC数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位
	/// Writes data to the PLC data, in the address format i100,q100,db20.100,m100, in bytes
	/// </summary>
	/// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -&gt; Starting address, formatted as M100,I100,Q100,DB1.100</param>
	/// <param name="value">要写入的实际数据 -&gt; The actual data to write</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		OperateResult<byte[]> command = BuildWriteCommand(address, value);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> write = await ReadFromCoreServerAsync(command.Content);
		if (!write.IsSuccess)
		{
			return write;
		}
		OperateResult check = CheckResponseContent(write.Content);
		if (!check.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(check);
		}
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi("ReadByte", "")]
	/// <summary>
	/// 读取指定地址的byte数据
	/// Reads the byte data for the specified address
	/// </summary>
	/// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -&gt; Starting address, formatted as M100,I100,Q100,DB1.100</param>
	/// <returns>byte类型的结果对象 -&gt; Result object of type Byte</returns>
	/// <remarks>
	/// 不适用于DB块，定时器，计数器的数据读取，会提示相应的错误，读取长度必须为偶数
	/// </remarks>
	public OperateResult<byte> ReadByte(string address)
	{
		return ByteTransformHelper.GetResultFromArray(Read(address, 1));
	}

	[HslMqttApi("WriteByte", "")]
	/// <summary>
	/// 向PLC中写入byte数据，返回是否写入成功
	/// Writes byte data to the PLC and returns whether the write succeeded
	/// </summary>
	/// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -&gt; Starting address, formatted as M100,I100,Q100,DB1.100</param>
	/// <param name="value">要写入的实际数据 -&gt; The actual data to write</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	public OperateResult Write(string address, byte value)
	{
		return Write(address, new byte[1] { value });
	}

	/// <summary>
	/// 读取指定地址的byte数据
	/// Reads the byte data for the specified address
	/// </summary>
	/// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -&gt; Starting address, formatted as M100,I100,Q100,DB1.100</param>
	/// <returns>byte类型的结果对象 -&gt; Result object of type Byte</returns>
	/// <remarks>
	/// 不适用于DB块，定时器，计数器的数据读取，会提示相应的错误，读取长度必须为偶数
	/// </remarks>
	public async Task<OperateResult<byte>> ReadByteAsync(string address)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadAsync(address, 1));
	}

	/// <summary>
	/// 向PLC中写入byte数据，返回是否写入成功
	/// Writes byte data to the PLC and returns whether the write succeeded
	/// </summary>
	/// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -&gt; Starting address, formatted as M100,I100,Q100,DB1.100</param>
	/// <param name="value">要写入的实际数据 -&gt; The actual data to write</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	public async Task<OperateResult> WriteAsync(string address, byte value)
	{
		return await WriteAsync(address, new byte[1] { value });
	}

	public override string ToString()
	{
		return $"SiemensFetchWriteNet[{IpAddress}:{Port}]";
	}

	/// <summary>
	/// 计算特殊的地址信息
	/// Calculate special address information
	/// </summary>
	/// <param name="address">字符串信息</param>
	/// <returns>实际值</returns>
	private static int CalculateAddressStarted(string address)
	{
		if (address.IndexOf('.') < 0)
		{
			return Convert.ToInt32(address);
		}
		string[] array = address.Split('.');
		return Convert.ToInt32(array[0]);
	}

	private static OperateResult CheckResponseContent(byte[] content)
	{
		if (content == null || content.Length < 9)
		{
			return new OperateResult(StringResources.Language.ReceiveDataLengthTooShort + "9, Content: " + content.ToHexString(' '));
		}
		if (content[8] != 0)
		{
			return new OperateResult(content[8], StringResources.Language.SiemensWriteError + content[8]);
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 解析数据地址，解析出地址类型，起始地址，DB块的地址
	/// Parse data address, parse out address type, start address, db block address
	/// </summary>
	/// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -&gt; Starting address, formatted as M100,I100,Q100,DB1.100</param>
	/// <returns>解析出地址类型，起始地址，DB块的地址 -&gt; Resolves address type, start address, db block address</returns>
	private static OperateResult<byte, int, ushort> AnalysisAddress(string address)
	{
		OperateResult<byte, int, ushort> operateResult = new OperateResult<byte, int, ushort>();
		try
		{
			operateResult.Content3 = 0;
			if (address[0] == 'I')
			{
				operateResult.Content1 = 3;
				operateResult.Content2 = CalculateAddressStarted(address.Substring(1));
			}
			else if (address[0] == 'Q')
			{
				operateResult.Content1 = 4;
				operateResult.Content2 = CalculateAddressStarted(address.Substring(1));
			}
			else if (address[0] == 'M')
			{
				operateResult.Content1 = 2;
				operateResult.Content2 = CalculateAddressStarted(address.Substring(1));
			}
			else if (address[0] == 'D' || address.Substring(0, 2) == "DB")
			{
				operateResult.Content1 = 1;
				string[] array = address.Split('.');
				if (address[1] == 'B')
				{
					operateResult.Content3 = Convert.ToUInt16(array[0].Substring(2));
				}
				else
				{
					operateResult.Content3 = Convert.ToUInt16(array[0].Substring(1));
				}
				if (operateResult.Content3 > 255)
				{
					operateResult.Message = StringResources.Language.SiemensDBAddressNotAllowedLargerThan255;
					return operateResult;
				}
				operateResult.Content2 = CalculateAddressStarted(address.Substring(address.IndexOf('.') + 1));
			}
			else if (address[0] == 'T')
			{
				operateResult.Content1 = 7;
				operateResult.Content2 = CalculateAddressStarted(address.Substring(1));
			}
			else
			{
				if (address[0] != 'C')
				{
					operateResult.Message = StringResources.Language.NotSupportedDataType;
					operateResult.Content1 = 0;
					operateResult.Content2 = 0;
					operateResult.Content3 = 0;
					return operateResult;
				}
				operateResult.Content1 = 6;
				operateResult.Content2 = CalculateAddressStarted(address.Substring(1));
			}
		}
		catch (Exception ex)
		{
			operateResult.Message = ex.Message;
			return operateResult;
		}
		operateResult.IsSuccess = true;
		return operateResult;
	}

	/// <summary>
	/// 生成一个读取字数据指令头的通用方法
	/// A general method for generating a command header to read a Word data
	/// </summary>
	/// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -&gt; Starting address, formatted as M100,I100,Q100,DB1.100</param>
	/// <param name="count">读取数据个数 -&gt; Number of Read data</param>
	/// <returns>带结果对象的报文数据 -&gt; Message data with a result object</returns>
	public static OperateResult<byte[]> BuildReadCommand(string address, ushort count)
	{
		OperateResult<byte, int, ushort> operateResult = AnalysisAddress(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		byte[] buffer = new byte[16]
		{
			83,
			53,
			16,
			1,
			3,
			5,
			3,
			8,
			operateResult.Content1,
			(byte)operateResult.Content3,
			(byte)(operateResult.Content2 / 256),
			(byte)(operateResult.Content2 % 256),
			0,
			0,
			0,
			0
		};
		if (operateResult.Content1 == 1 || operateResult.Content1 == 6 || operateResult.Content1 == 7)
		{
			if (count % 2 != 0)
			{
				return new OperateResult<byte[]>(StringResources.Language.SiemensReadLengthMustBeEvenNumber);
			}
			buffer[12] = BitConverter.GetBytes(count / 2)[1];
			buffer[13] = BitConverter.GetBytes(count / 2)[0];
		}
		else
		{
			buffer[12] = BitConverter.GetBytes(count)[1];
			buffer[13] = BitConverter.GetBytes(count)[0];
		}
		buffer[14] = byte.MaxValue;
		buffer[15] = 2;
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 生成一个写入字节数据的指令
	/// Generate an instruction to write byte data
	/// </summary>
	/// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -&gt; Starting address, formatted as M100,I100,Q100,DB1.100</param>
	/// <param name="data">实际的写入的内容 -&gt; The actual content of the write</param>
	/// <returns>带结果对象的报文数据 -&gt; Message data with a result object</returns>
	public static OperateResult<byte[]> BuildWriteCommand(string address, byte[] data)
	{
		if (data == null)
		{
			data = new byte[0];
		}
		OperateResult<byte, int, ushort> operateResult = AnalysisAddress(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		byte[] buffer = new byte[16 + data.Length];
		buffer[0] = 83;
		buffer[1] = 53;
		buffer[2] = 16;
		buffer[3] = 1;
		buffer[4] = 3;
		buffer[5] = 3;
		buffer[6] = 3;
		buffer[7] = 8;
		buffer[8] = operateResult.Content1;
		buffer[9] = (byte)operateResult.Content3;
		buffer[10] = (byte)(operateResult.Content2 / 256);
		buffer[11] = (byte)(operateResult.Content2 % 256);
		if (operateResult.Content1 == 1 || operateResult.Content1 == 6 || operateResult.Content1 == 7)
		{
			if (data.Length % 2 != 0)
			{
				return new OperateResult<byte[]>(StringResources.Language.SiemensReadLengthMustBeEvenNumber);
			}
			buffer[12] = BitConverter.GetBytes(data.Length / 2)[1];
			buffer[13] = BitConverter.GetBytes(data.Length / 2)[0];
		}
		else
		{
			buffer[12] = BitConverter.GetBytes(data.Length)[1];
			buffer[13] = BitConverter.GetBytes(data.Length)[0];
		}
		buffer[14] = byte.MaxValue;
		buffer[15] = 2;
		Array.Copy(data, 0, buffer, 16, data.Length);
		return OperateResult.CreateSuccessResult(buffer);
	}
}
