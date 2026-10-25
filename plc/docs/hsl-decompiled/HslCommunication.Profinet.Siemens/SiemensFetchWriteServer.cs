// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Siemens;

/// <summary>
/// 西门子的Fetch/Write协议的虚拟PLC，可以用来调试通讯，也可以实现一个虚拟的PLC功能，从而开发一套带虚拟环境的上位机系统，可以用来演示，测试。
/// The virtual PLC of Siemens Fetch/Write protocol can be used for debugging communication, and can also realize a virtual PLC function, so as to develop a set of upper computer system with virtual environment, which can be used for demonstration and testing.
/// </summary>
/// <remarks>
/// 本虚拟服务器的使用需要企业商业授权，否则只能运行24小时。本协议实现的虚拟PLC服务器，主要支持I,Q,M,DB块的数据读写操作，例如 M100, DB1.100，服务器端也可以对位进行读写操作，例如M100.1，DB1.100.2；
/// 但是不支持连接的远程客户端对位进行操作。
/// </remarks>
public class SiemensFetchWriteServer : DeviceServer
{
	private SoftBuffer inputBuffer;

	private SoftBuffer outputBuffer;

	private SoftBuffer memeryBuffer;

	private SoftBuffer counterBuffer;

	private SoftBuffer timerBuffer;

	private SoftBuffer db1BlockBuffer;

	private SoftBuffer db2BlockBuffer;

	private SoftBuffer db3BlockBuffer;

	private SoftBuffer dbOtherBlockBuffer;

	private const int DataPoolLength = 65536;

	/// <summary>
	/// 实例化一个S7协议的服务器，支持I，Q，M，DB1.X, DB2.X, DB3.X 数据区块的读写操作
	/// Instantiate a server with S7 protocol, support I, Q, M, DB1.X data block read and write operations
	/// </summary>
	public SiemensFetchWriteServer()
	{
		inputBuffer = new SoftBuffer(65536);
		outputBuffer = new SoftBuffer(65536);
		memeryBuffer = new SoftBuffer(65536);
		db1BlockBuffer = new SoftBuffer(65536);
		db2BlockBuffer = new SoftBuffer(65536);
		db3BlockBuffer = new SoftBuffer(65536);
		dbOtherBlockBuffer = new SoftBuffer(65536);
		counterBuffer = new SoftBuffer(65536);
		timerBuffer = new SoftBuffer(65536);
		base.WordLength = 2;
		base.ByteTransform = new ReverseBytesTransform();
	}

	private OperateResult<SoftBuffer> GetDataAreaFromS7Address(S7AddressData s7Address)
	{
		switch (s7Address.DataCode)
		{
		case 129:
			return OperateResult.CreateSuccessResult(inputBuffer);
		case 130:
			return OperateResult.CreateSuccessResult(outputBuffer);
		case 131:
			return OperateResult.CreateSuccessResult(memeryBuffer);
		case 132:
			if (s7Address.DbBlock == 1)
			{
				return OperateResult.CreateSuccessResult(db1BlockBuffer);
			}
			if (s7Address.DbBlock == 2)
			{
				return OperateResult.CreateSuccessResult(db2BlockBuffer);
			}
			if (s7Address.DbBlock == 3)
			{
				return OperateResult.CreateSuccessResult(db3BlockBuffer);
			}
			return OperateResult.CreateSuccessResult(dbOtherBlockBuffer);
		default:
			return new OperateResult<SoftBuffer>(StringResources.Language.NotSupportedDataType);
		}
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 从PLC读取原始的字节数据，地址格式为I100，Q100，DB20.100，M100，长度参数以字节为单位
	/// Read the original byte data from the PLC, the address format is I100, Q100, DB20.100, M100, length parameters in bytes
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <param name="length">读取的数量，以字节为单位 The number of reads, in bytes</param>
	/// <returns>是否读取成功的结果对象 Whether to read the successful result object</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<S7AddressData> operateResult = S7AddressData.ParseFrom(address, length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		OperateResult<SoftBuffer> dataAreaFromS7Address = GetDataAreaFromS7Address(operateResult.Content);
		if (!dataAreaFromS7Address.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(dataAreaFromS7Address);
		}
		return OperateResult.CreateSuccessResult(dataAreaFromS7Address.Content.GetBytes(operateResult.Content.AddressStart / 8, length));
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 将数据写入到PLC数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位
	/// Writes data to the PLC data, in the address format I100,Q100,DB20.100,M100, in bytes
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <param name="value">写入的原始数据 -&gt; Raw data written to</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		OperateResult<S7AddressData> operateResult = S7AddressData.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		OperateResult<SoftBuffer> dataAreaFromS7Address = GetDataAreaFromS7Address(operateResult.Content);
		if (!dataAreaFromS7Address.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(dataAreaFromS7Address);
		}
		dataAreaFromS7Address.Content.SetBytes(value, operateResult.Content.AddressStart / 8);
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi("ReadByte", "")]
	/// <summary>
	/// 读取指定地址的byte数据，地址格式I100，M100，Q100，DB20.100
	/// Reads the byte data of the specified address, the address format I100,Q100,DB20.100,M100
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <returns>是否读取成功的结果对象 -&gt; Whether to read the successful result object</returns>
	public OperateResult<byte> ReadByte(string address)
	{
		return ByteTransformHelper.GetResultFromArray(Read(address, 1));
	}

	[HslMqttApi("WriteByte", "")]
	/// <summary>
	/// 向PLC中写入byte数据，返回值说明
	/// Write byte data to the PLC, return value description
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,mM100,Q100,DB20.100</param>
	/// <param name="value">byte数据 -&gt; Byte data</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	public OperateResult Write(string address, byte value)
	{
		return Write(address, new byte[1] { value });
	}

	[HslMqttApi("ReadBool", "")]
	/// <summary>
	/// 读取指定地址的bool数据，地址格式为I100，M100，Q100，DB20.100
	/// reads bool data for the specified address in the format I100，M100，Q100，DB20.100
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <returns>是否读取成功的结果对象 -&gt; Whether to read the successful result object</returns>
	/// <remarks>
	/// 对于200smartPLC的V区，就是DB1.X，例如，V100=DB1.100
	/// </remarks>
	public override OperateResult<bool> ReadBool(string address)
	{
		OperateResult<S7AddressData> operateResult = S7AddressData.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool>(operateResult);
		}
		OperateResult<SoftBuffer> dataAreaFromS7Address = GetDataAreaFromS7Address(operateResult.Content);
		if (!dataAreaFromS7Address.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool>(dataAreaFromS7Address);
		}
		return OperateResult.CreateSuccessResult(dataAreaFromS7Address.Content.GetBool(operateResult.Content.AddressStart));
	}

	[HslMqttApi("WriteBool", "")]
	/// <summary>
	/// 写入PLC的一个位，例如"M100.6"，"I100.7"，"Q100.0"，"DB20.100.0"，如果只写了"M100"默认为"M100.0"
	/// Write a bit of PLC, for example  "M100.6",  "I100.7",  "Q100.0",  "DB20.100.0", if only write  "M100" defaults to  "M100.0"
	/// </summary>
	/// <param name="address">起始地址，格式为&quot;M100.6&quot;,  &quot;I100.7&quot;,  &quot;Q100.0&quot;,  &quot;DB20.100.0&quot; -&gt; Start address, format  &quot;M100.6&quot;,  &quot;I100.7&quot;,  &quot;Q100.0&quot;,  &quot;DB20.100.0&quot;</param>
	/// <param name="value">写入的数据，True或是False -&gt; Writes the data, either True or False</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	public override OperateResult Write(string address, bool value)
	{
		OperateResult<S7AddressData> operateResult = S7AddressData.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<SoftBuffer> dataAreaFromS7Address = GetDataAreaFromS7Address(operateResult.Content);
		if (!dataAreaFromS7Address.IsSuccess)
		{
			return dataAreaFromS7Address;
		}
		dataAreaFromS7Address.Content.SetBool(value, operateResult.Content.AddressStart);
		return OperateResult.CreateSuccessResult();
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new FetchWriteMessage();
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		byte[] value = null;
		if (receive[5] == 3)
		{
			value = WriteByMessage(receive);
		}
		else if (receive[5] == 5)
		{
			value = ReadByMessage(receive);
		}
		return OperateResult.CreateSuccessResult(value);
	}

	private SoftBuffer GetBufferFromCommand(byte[] command)
	{
		if (command[8] == 2)
		{
			return memeryBuffer;
		}
		if (command[8] == 3)
		{
			return inputBuffer;
		}
		if (command[8] == 4)
		{
			return outputBuffer;
		}
		if (command[8] == 1)
		{
			if (command[9] == 1)
			{
				return db1BlockBuffer;
			}
			if (command[9] == 2)
			{
				return db2BlockBuffer;
			}
			if (command[9] == 3)
			{
				return db3BlockBuffer;
			}
			return dbOtherBlockBuffer;
		}
		if (command[8] == 6)
		{
			return counterBuffer;
		}
		if (command[8] == 7)
		{
			return timerBuffer;
		}
		return null;
	}

	private byte[] ReadByMessage(byte[] command)
	{
		SoftBuffer bufferFromCommand = GetBufferFromCommand(command);
		int index = command[10] * 256 + command[11];
		int num = command[12] * 256 + command[13];
		if (bufferFromCommand == null)
		{
			return PackCommandResponse(6, 1, null);
		}
		if (command[8] == 1 || command[8] == 6 || command[8] == 7)
		{
			return PackCommandResponse(6, 0, bufferFromCommand.GetBytes(index, num * 2));
		}
		return PackCommandResponse(6, 0, bufferFromCommand.GetBytes(index, num));
	}

	private byte[] WriteByMessage(byte[] command)
	{
		if (!base.EnableWrite)
		{
			return PackCommandResponse(4, 1, null);
		}
		SoftBuffer bufferFromCommand = GetBufferFromCommand(command);
		int destIndex = command[10] * 256 + command[11];
		int num = command[12] * 256 + command[13];
		if (bufferFromCommand == null)
		{
			return PackCommandResponse(4, 1, null);
		}
		if (command[8] == 1 || command[8] == 6 || command[8] == 7)
		{
			if (num != (command.Length - 16) / 2)
			{
				return PackCommandResponse(4, 1, null);
			}
			bufferFromCommand.SetBytes(command.RemoveBegin(16), destIndex);
			return PackCommandResponse(4, 0, null);
		}
		if (num != command.Length - 16)
		{
			return PackCommandResponse(4, 1, null);
		}
		bufferFromCommand.SetBytes(command.RemoveBegin(16), destIndex);
		return PackCommandResponse(4, 0, null);
	}

	private byte[] PackCommandResponse(byte opCode, byte err, byte[] data)
	{
		if (data == null)
		{
			data = new byte[0];
		}
		byte[] buffer = new byte[16 + data.Length];
		buffer[0] = 83;
		buffer[1] = 53;
		buffer[2] = 16;
		buffer[3] = 1;
		buffer[4] = 3;
		buffer[5] = opCode;
		buffer[6] = 15;
		buffer[7] = 3;
		buffer[8] = err;
		buffer[9] = byte.MaxValue;
		buffer[10] = 7;
		if (data.Length != 0)
		{
			data.CopyTo(buffer, 16);
		}
		return buffer;
	}

	protected override void LoadFromBytes(byte[] content)
	{
		if (content.Length < 589824)
		{
			throw new Exception("File is not correct");
		}
		inputBuffer.SetBytes(content, 0, 0, 65536);
		outputBuffer.SetBytes(content, 65536, 0, 65536);
		memeryBuffer.SetBytes(content, 131072, 0, 65536);
		db1BlockBuffer.SetBytes(content, 196608, 0, 65536);
		db2BlockBuffer.SetBytes(content, 262144, 0, 65536);
		db3BlockBuffer.SetBytes(content, 327680, 0, 65536);
		dbOtherBlockBuffer.SetBytes(content, 393216, 0, 65536);
		counterBuffer.SetBytes(content, 458752, 0, 65536);
		timerBuffer.SetBytes(content, 524288, 0, 65536);
	}

	protected override byte[] SaveToBytes()
	{
		byte[] buffer = new byte[589824];
		Array.Copy(inputBuffer.GetBytes(), 0, buffer, 0, 65536);
		Array.Copy(outputBuffer.GetBytes(), 0, buffer, 65536, 65536);
		Array.Copy(memeryBuffer.GetBytes(), 0, buffer, 131072, 65536);
		Array.Copy(db1BlockBuffer.GetBytes(), 0, buffer, 196608, 65536);
		Array.Copy(db2BlockBuffer.GetBytes(), 0, buffer, 262144, 65536);
		Array.Copy(db3BlockBuffer.GetBytes(), 0, buffer, 327680, 65536);
		Array.Copy(dbOtherBlockBuffer.GetBytes(), 0, buffer, 393216, 65536);
		Array.Copy(counterBuffer.GetBytes(), 0, buffer, 458752, 65536);
		Array.Copy(timerBuffer.GetBytes(), 0, buffer, 524288, 65536);
		return buffer;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			inputBuffer?.Dispose();
			outputBuffer?.Dispose();
			memeryBuffer?.Dispose();
			db1BlockBuffer?.Dispose();
			db2BlockBuffer?.Dispose();
			db3BlockBuffer?.Dispose();
			dbOtherBlockBuffer?.Dispose();
		}
		base.Dispose(disposing);
	}

	public override string ToString()
	{
		return $"SiemensFetchWriteServer[{base.Port}]";
	}
}
