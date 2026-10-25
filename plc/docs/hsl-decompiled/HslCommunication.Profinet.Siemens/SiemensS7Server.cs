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
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Core.Pipe;
using HslCommunication.Profinet.Siemens.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Siemens;

/// <summary>
/// 西门子S7协议的虚拟服务器，支持TCP协议，模拟的是1200的PLC进行通信，在客户端进行操作操作的时候，最好是选择1200的客户端对象进行通信。
/// The virtual server of Siemens S7 protocol supports TCP protocol. It simulates 1200 PLC for communication. When the client is operating, it is best to select the 1200 client object for communication.
/// </summary>
/// <remarks>
/// 本西门子的虚拟PLC仅限商业授权用户使用，感谢支持。
/// 对于200smartPLC的V区，就是DB1.X，例如，V100=DB1.100
/// </remarks>
public class SiemensS7Server : DeviceServer
{
	private SoftBuffer systemBuffer;

	private SoftBuffer inputBuffer;

	private SoftBuffer outputBuffer;

	private SoftBuffer memeryBuffer;

	private SoftBuffer countBuffer;

	private SoftBuffer timerBuffer;

	private SoftBuffer aiBuffer;

	private SoftBuffer aqBuffer;

	private Dictionary<int, SoftBuffer> dbBlockBuffer;

	private const int DataPoolLength = 65536;

	/// <summary>
	/// 是否分析通信报文信息，并将分析结果追加到报文日志里去
	/// Should we analyze the communication message information and add the analysis results to the message log?
	/// </summary>
	public bool AnalysisLogMessage { get; set; } = false;

	/// <summary>
	/// 实例化一个S7协议的服务器，支持I，Q，M，DB1.X, DB2.X, DB3.X 数据区块的读写操作
	/// Instantiate a server with S7 protocol, support I, Q, M, DB1.X data block read and write operations
	/// </summary>
	public SiemensS7Server()
	{
		inputBuffer = new SoftBuffer(65536);
		outputBuffer = new SoftBuffer(65536);
		memeryBuffer = new SoftBuffer(65536);
		countBuffer = new SoftBuffer(131072);
		timerBuffer = new SoftBuffer(131072);
		aiBuffer = new SoftBuffer(65536);
		aqBuffer = new SoftBuffer(65536);
		systemBuffer = new SoftBuffer(65536);
		systemBuffer.SetBytes("43 50 55 20 32 32 36 20 43 4E 20 20 20 20 20 20 30 32 30 31".ToHexBytes(), 0);
		base.WordLength = 2;
		base.ByteTransform = new ReverseBytesTransform();
		dbBlockBuffer = new Dictionary<int, SoftBuffer>();
		dbBlockBuffer.Add(1, new SoftBuffer(65536));
		dbBlockBuffer.Add(2, new SoftBuffer(65536));
		dbBlockBuffer.Add(3, new SoftBuffer(65536));
	}

	/// <summary>
	/// 根据S7格式的地址，获取当前的数据缓存类对象
	/// </summary>
	/// <param name="s7Address">S7格式的数据地址信息</param>
	/// <returns>内存缓存对象</returns>
	private OperateResult<SoftBuffer> GetDataAreaFromS7Address(S7AddressData s7Address)
	{
		switch (s7Address.DataCode)
		{
		case 3:
			return OperateResult.CreateSuccessResult(systemBuffer);
		case 129:
			return OperateResult.CreateSuccessResult(inputBuffer);
		case 130:
			return OperateResult.CreateSuccessResult(outputBuffer);
		case 131:
			return OperateResult.CreateSuccessResult(memeryBuffer);
		case 132:
			if (dbBlockBuffer.ContainsKey(s7Address.DbBlock))
			{
				return OperateResult.CreateSuccessResult(dbBlockBuffer[s7Address.DbBlock]);
			}
			return new OperateResult<SoftBuffer>(10, StringResources.Language.SiemensError000A);
		case 30:
			return OperateResult.CreateSuccessResult(countBuffer);
		case 31:
			return OperateResult.CreateSuccessResult(timerBuffer);
		case 6:
			return OperateResult.CreateSuccessResult(aiBuffer);
		case 7:
			return OperateResult.CreateSuccessResult(aqBuffer);
		default:
			return new OperateResult<SoftBuffer>(6, StringResources.Language.SiemensError0006);
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
		if (operateResult.Content.DataCode == 30 || operateResult.Content.DataCode == 31)
		{
			return OperateResult.CreateSuccessResult(dataAreaFromS7Address.Content.GetBytes(operateResult.Content.AddressStart * 2, length * 2));
		}
		int num = operateResult.Content.AddressStart / 8;
		if (num + length > dataAreaFromS7Address.Content.Capacity)
		{
			return new OperateResult<byte[]>(5, StringResources.Language.SiemensReadLengthOverPlcAssign);
		}
		return OperateResult.CreateSuccessResult(dataAreaFromS7Address.Content.GetBytes(num, length));
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
		if (operateResult.Content.DataCode == 30 || operateResult.Content.DataCode == 31)
		{
			dataAreaFromS7Address.Content.SetBytes(value, operateResult.Content.AddressStart * 2);
		}
		else
		{
			int num = operateResult.Content.AddressStart / 8;
			if (num + value.Length > dataAreaFromS7Address.Content.Capacity)
			{
				return new OperateResult<byte[]>(5, StringResources.Language.SiemensReadLengthOverPlcAssign);
			}
			dataAreaFromS7Address.Content.SetBytes(value, num);
		}
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

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 读取指定地址的bool数组，地址格式为I100，M100，Q100，DB20.100
	/// reads bool array data for the specified address in the format I100，M100，Q100，DB20.100
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,M100,Q100,DB20.100</param>
	/// <param name="length">读取的长度信息</param>
	/// <returns>是否读取成功的结果对象 -&gt; Whether to read the successful result object</returns>
	/// <remarks>
	/// 对于200smartPLC的V区，就是DB1.X，例如，V100=DB1.100
	/// </remarks>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		OperateResult<S7AddressData> operateResult = S7AddressData.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		OperateResult<SoftBuffer> dataAreaFromS7Address = GetDataAreaFromS7Address(operateResult.Content);
		if (!dataAreaFromS7Address.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(dataAreaFromS7Address);
		}
		if (operateResult.Content.AddressStart + length > dataAreaFromS7Address.Content.Capacity * 8)
		{
			return new OperateResult<bool[]>(5, StringResources.Language.SiemensReadLengthOverPlcAssign);
		}
		return OperateResult.CreateSuccessResult(dataAreaFromS7Address.Content.GetBool(operateResult.Content.AddressStart, length));
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// [警告] 向PLC中写入bool数组，比如你写入M100,那么data[0]对应M100.0，写入的长度应该小于1600位
	/// [Warn] Write the bool array to the PLC, for example, if you write M100, then data[0] corresponds to M100.0,
	/// The length of the write should be less than 1600 bits
	/// </summary>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,mM100,Q100,DB20.100</param>
	/// <param name="values">要写入的bool数组，长度为8的倍数 -&gt; The bool array to write, a multiple of 8 in length</param>
	/// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
	/// <remarks>
	/// 批量写入bool数组存在一定的风险，举例写入M100.5的值 [true,false,true,true,false,true]，会读取M100-M101的byte[]，然后修改中间的位，再写入回去，
	/// 如果读取之后写入之前，PLC修改了其他位，则会影响其他的位的数据，请谨慎使用。
	/// There is a certain risk in batch writing bool arrays. For example, writing the value of M100.5 [true,false,true,true,false,true],
	/// will read the byte[] of M100-M101, then modify the middle bit, and then Write back.
	/// If the PLC modifies other bits after reading and before writing, it will affect the data of other bits. Please use it with caution.
	/// </remarks>
	public override OperateResult Write(string address, bool[] value)
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
		if (operateResult.Content.AddressStart + value.Length > dataAreaFromS7Address.Content.Capacity * 8)
		{
			return new OperateResult(5, StringResources.Language.SiemensReadLengthOverPlcAssign);
		}
		dataAreaFromS7Address.Content.SetBool(value, operateResult.Content.AddressStart);
		return OperateResult.CreateSuccessResult();
	}

	public override OperateResult Write(string address, string value, Encoding encoding)
	{
		return SiemensS7Helper.Write(this, SiemensPLCS.S1200, address, value, encoding);
	}

	[HslMqttApi(ApiTopic = "WriteWString", Description = "写入unicode编码的字符串，支持中文")]
	/// <summary>
	/// 使用双字节编码的方式，将字符串以 Unicode 编码写入到PLC的地址里，可以使用中文。
	/// Use the double-byte encoding method to write the character string to the address of the PLC in Unicode encoding. Chinese can be used.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="currentPlc">PLC的系列信息</param>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,mM100,Q100,DB20.100</param>
	/// <param name="value">字符串的值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public OperateResult WriteWString(string address, string value)
	{
		return SiemensS7Helper.WriteWString(this, SiemensPLCS.S1200, address, value);
	}

	public override OperateResult<string> ReadString(string address, ushort length, Encoding encoding)
	{
		return (length == 0) ? ReadString(address, encoding) : base.ReadString(address, length, encoding);
	}

	[HslMqttApi("ReadS7String", "读取S7格式的字符串")]
	/// <summary>
	/// 读取西门子的地址的字符串信息，这个信息是和西门子绑定在一起，长度随西门子的信息动态变化的
	/// Read the Siemens address string information. This information is bound to Siemens and its length changes dynamically with the Siemens information
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="currentPlc">PLC的系列信息</param>
	/// <param name="address">数据地址，具体的格式需要参照类的说明文档</param>
	/// <param name="encoding">自定的编码信息，一般即可，中文需要 Encoding.GetEncoding(&quot;gb2312&quot;)</param>
	/// <returns>带有是否成功的字符串结果类对象</returns>
	/// <remarks>
	/// 如果指定编码，一般即可，中文需要 Encoding.GetEncoding("gb2312")
	/// </remarks>
	public OperateResult<string> ReadString(string address)
	{
		return ReadString(address, Encoding.ASCII);
	}

	/// <summary>
	/// 读取西门子的地址的字符串信息，这个信息是和西门子绑定在一起，长度随西门子的信息动态变化的
	/// Read the Siemens address string information. This information is bound to Siemens and its length changes dynamically with the Siemens information
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="currentPlc">PLC的系列信息</param>
	/// <param name="address">数据地址，具体的格式需要参照类的说明文档</param>
	/// <param name="encoding">自定的编码信息，一般即可，中文需要 Encoding.GetEncoding(&quot;gb2312&quot;)</param>
	/// <returns>带有是否成功的字符串结果类对象</returns>
	/// <remarks>
	/// 如果指定编码，一般即可，中文需要 Encoding.GetEncoding("gb2312")
	/// </remarks>
	public OperateResult<string> ReadString(string address, Encoding encoding)
	{
		return SiemensS7Helper.ReadString(this, SiemensPLCS.S1200, address, encoding);
	}

	[HslMqttApi("ReadWString", "读取S7格式的双字节字符串")]
	/// <summary>
	/// 读取西门子的地址的字符串信息，这个信息是和西门子绑定在一起，长度随西门子的信息动态变化的
	/// Read the Siemens address string information. This information is bound to Siemens and its length changes dynamically with the Siemens information
	/// </summary>
	/// <param name="address">数据地址，具体的格式需要参照类的说明文档</param>
	/// <returns>带有是否成功的字符串结果类对象</returns>
	public OperateResult<string> ReadWString(string address)
	{
		return SiemensS7Helper.ReadWString(this, SiemensPLCS.S1200, address);
	}

	/// <summary>
	/// 将指定的字符串写入到西门子PLC里面去，将自动添加字符串长度信息，方便PLC识别字符串的内容。
	/// Write the specified string into Siemens PLC, and the string length information will be automatically added, which is convenient for PLC to identify the content of the string.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="currentPlc">PLC的系列信息</param>
	/// <param name="address">数据地址，具体的格式需要参照类的说明文档</param>
	/// <param name="value">写入的字符串值</param>
	/// <param name="encoding">编码信息</param>
	/// <returns>是否写入成功</returns>
	public override async Task<OperateResult> WriteAsync(string address, string value, Encoding encoding)
	{
		return await SiemensS7Helper.WriteAsync(this, SiemensPLCS.S1200, address, value, encoding);
	}

	/// <summary>
	/// 使用双字节编码的方式，将字符串以 Unicode 编码写入到PLC的地址里，可以使用中文。
	/// Use the double-byte encoding method to write the character string to the address of the PLC in Unicode encoding. Chinese can be used.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="currentPlc">PLC的系列信息</param>
	/// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,mM100,Q100,DB20.100</param>
	/// <param name="value">字符串的值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public async Task<OperateResult> WriteWStringAsync(string address, string value)
	{
		return await SiemensS7Helper.WriteWStringAsync(this, SiemensPLCS.S1200, address, value);
	}

	public override async Task<OperateResult<string>> ReadStringAsync(string address, ushort length, Encoding encoding)
	{
		return (length != 0) ? (await base.ReadStringAsync(address, length, encoding)) : (await ReadStringAsync(address, encoding));
	}

	/// <summary>
	/// 读取西门子的地址的字符串信息，这个信息是和西门子绑定在一起，长度随西门子的信息动态变化的
	/// Read the Siemens address string information. This information is bound to Siemens and its length changes dynamically with the Siemens information
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="currentPlc">PLC的系列信息</param>
	/// <param name="address">数据地址，具体的格式需要参照类的说明文档</param>
	/// <param name="encoding">自定的编码信息，一般即可，中文需要 Encoding.GetEncoding(&quot;gb2312&quot;)</param>
	/// <returns>带有是否成功的字符串结果类对象</returns>
	/// <remarks>
	/// 如果指定编码，一般即可，中文需要 Encoding.GetEncoding("gb2312")
	/// </remarks>
	public async Task<OperateResult<string>> ReadStringAsync(string address)
	{
		return await ReadStringAsync(address, Encoding.ASCII);
	}

	/// <summary>
	/// 读取西门子的地址的字符串信息，这个信息是和西门子绑定在一起，长度随西门子的信息动态变化的
	/// Read the Siemens address string information. This information is bound to Siemens and its length changes dynamically with the Siemens information
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="currentPlc">PLC的系列信息</param>
	/// <param name="address">数据地址，具体的格式需要参照类的说明文档</param>
	/// <param name="encoding">自定的编码信息，一般即可，中文需要 Encoding.GetEncoding(&quot;gb2312&quot;)</param>
	/// <returns>带有是否成功的字符串结果类对象</returns>
	/// <remarks>
	/// 如果指定编码，一般即可，中文需要 Encoding.GetEncoding("gb2312")
	/// </remarks>
	public async Task<OperateResult<string>> ReadStringAsync(string address, Encoding encoding)
	{
		return await SiemensS7Helper.ReadStringAsync(this, SiemensPLCS.S1200, address, encoding);
	}

	/// <summary>
	/// 读取西门子的地址的字符串信息，这个信息是和西门子绑定在一起，长度随西门子的信息动态变化的
	/// Read the Siemens address string information. This information is bound to Siemens and its length changes dynamically with the Siemens information
	/// </summary>
	/// <param name="address">数据地址，具体的格式需要参照类的说明文档</param>
	/// <returns>带有是否成功的字符串结果类对象</returns>
	public async Task<OperateResult<string>> ReadWStringAsync(string address)
	{
		return await SiemensS7Helper.ReadWStringAsync(this, SiemensPLCS.S1200, address);
	}

	protected override string GetLogTextFromBinary(PipeSession session, byte[] content)
	{
		string text = base.GetLogTextFromBinary(session, content);
		if (AnalysisLogMessage)
		{
			text += SiemensS7Helper.AnalysisAddressLogMessage(session, base.ByteTransform, content);
		}
		return text;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new S7Message();
	}

	private byte[] CreateHandleOneBack(byte[] handle)
	{
		byte[] buffer = new byte[handle.Length];
		handle.CopyTo(buffer, 0);
		buffer[5] = 208;
		buffer[6] = handle[8];
		buffer[7] = handle[9];
		buffer[8] = 0;
		buffer[9] = 12;
		return buffer;
	}

	private byte[] CreateHandleTwoBack(byte[] handle)
	{
		byte[] array = SoftBasic.HexStringToBytes("03 00 00 1B 02 f0 80 32 03 00 00 00 00 00 08 00 00 00 00 f0 01 00 01 00 f0 00 f0");
		if (handle.Length > 14)
		{
			array[11] = handle[11];
			array[12] = handle[12];
		}
		return array;
	}

	protected override OperateResult ThreadPoolLoginAfterClientCheck(PipeSession session, IPEndPoint endPoint)
	{
		if (IsNeedShakeHands())
		{
			CommunicationPipe communication = session.Communication;
			OperateResult<byte[]> operateResult = communication.ReceiveMessage(new S7Message(), null, useActivePush: false, null, delegate(byte[] m)
			{
				LogRevcMessage(m, session);
			});
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
			if (operateResult.Content == null || operateResult.Content.Length < 10)
			{
				return new OperateResult("Data is too short: " + operateResult.Content.ToHexString(' '));
			}
			byte[] array = CreateHandleOneBack(operateResult.Content);
			LogSendMessage(array, session);
			OperateResult operateResult2 = communication.Send(array);
			if (!operateResult2.IsSuccess)
			{
				return operateResult2;
			}
			OperateResult<byte[]> operateResult3 = communication.ReceiveMessage(new S7Message(), null, useActivePush: false, null, delegate(byte[] m)
			{
				LogRevcMessage(m, session);
			});
			if (!operateResult3.IsSuccess)
			{
				return operateResult3;
			}
			byte[] array2 = CreateHandleTwoBack(operateResult3.Content);
			LogSendMessage(array2, session);
			OperateResult operateResult4 = communication.Send(array2);
			if (!operateResult4.IsSuccess)
			{
				return operateResult4;
			}
		}
		return base.ThreadPoolLoginAfterClientCheck(session, endPoint);
	}

	/// <summary>
	/// 获取是否需要进行握手报文信息
	/// </summary>
	/// <returns>是否需要进行握手操作</returns>
	protected virtual bool IsNeedShakeHands()
	{
		return true;
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		byte[] array = null;
		try
		{
			if (receive[5] == 224)
			{
				return OperateResult.CreateSuccessResult(CreateHandleOneBack(receive));
			}
			if (receive[3] == 25 && receive[14] == 8)
			{
				return OperateResult.CreateSuccessResult(CreateHandleTwoBack(receive));
			}
			if (receive[17] == 4)
			{
				array = ReadByMessage(receive);
			}
			else if (receive[17] == 5)
			{
				array = WriteByMessage(receive);
			}
			else
			{
				if (receive[17] != 0)
				{
					return new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction);
				}
				array = ReadPlcType();
			}
			receive.SelectMiddle(11, 2).CopyTo(array, 11);
			return OperateResult.CreateSuccessResult(array);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
	}

	private byte[] ReadPlcType()
	{
		return SoftBasic.HexStringToBytes("03 00 00 7D 02 F0 80 32 07 00 00 00 01 00 0C 00 60 00 01 12 08 12 84 01 01 00 00 00 00 FF 09 00 5C 00 11 00 00 00 1C 00 03 00 01 36 45 53 37 20 32 31 35 2D 31 41 47 34 30 2D 30 58 42 30 20 00 00 00 06 20 20 00 06 36 45 53 37 20 32 31 35 2D 31 41 47 34 30 2D 30 58 42 30 20 00 00 00 06 20 20 00 07 36 45 53 37 20 32 31 35 2D 31 41 47 34 30 2D 30 58 42 30 20 00 00 56 04 02 01");
	}

	/// <summary>
	/// 将读取的结果数据内容进行打包，返回客户端读取
	/// </summary>
	/// <param name="command">接收的命令信息</param>
	/// <param name="content">读取的原始字节信息</param>
	/// <returns>返回客户端的原始报文信息</returns>
	protected virtual byte[] PackReadBack(byte[] command, List<byte> content)
	{
		if (content.Count > 226)
		{
			content = new List<byte>(PackReadWordCommandBack(5, null));
		}
		byte[] buffer = new byte[21 + content.Count];
		SoftBasic.HexStringToBytes("03 00 00 1A 02 F0 80 32 03 00 00 00 01 00 02 00 05 00 00 04 01").CopyTo(buffer, 0);
		buffer[2] = (byte)(buffer.Length / 256);
		buffer[3] = (byte)(buffer.Length % 256);
		buffer[15] = (byte)(content.Count / 256);
		buffer[16] = (byte)(content.Count % 256);
		buffer[20] = command[18];
		content.CopyTo(buffer, 21);
		return buffer;
	}

	private byte[] ReadByMessage(byte[] packCommand)
	{
		List<byte> list = new List<byte>();
		int num = packCommand[18];
		int num2 = 19;
		for (int i = 0; i < num; i++)
		{
			byte b = packCommand[num2 + 1];
			byte[] command = packCommand.SelectMiddle(num2, b + 2);
			num2 += b + 2;
			list.AddRange(ReadByCommand(command));
		}
		return PackReadBack(packCommand, list);
	}

	private byte[] ReadByCommand(byte[] command)
	{
		if (command[3] == 1)
		{
			int num = command[9] * 65536 + command[10] * 256 + command[11];
			ushort dbBlock = base.ByteTransform.TransUInt16(command, 6);
			ushort num2 = base.ByteTransform.TransUInt16(command, 4);
			OperateResult<SoftBuffer> dataAreaFromS7Address = GetDataAreaFromS7Address(new S7AddressData
			{
				AddressStart = num,
				DataCode = command[8],
				DbBlock = dbBlock,
				Length = 1
			});
			if (!dataAreaFromS7Address.IsSuccess)
			{
				return PackReadWordCommandBack((short)dataAreaFromS7Address.ErrorCode, null);
			}
			if (num + num2 > dataAreaFromS7Address.Content.Capacity * 8)
			{
				return PackReadWordCommandBack(5, null);
			}
			return PackReadBitCommandBack(dataAreaFromS7Address.Content.GetBool(num));
		}
		if (command[3] == 30 || command[3] == 31)
		{
			ushort num3 = base.ByteTransform.TransUInt16(command, 4);
			int num4 = command[9] * 65536 + command[10] * 256 + command[11];
			OperateResult<SoftBuffer> dataAreaFromS7Address2 = GetDataAreaFromS7Address(new S7AddressData
			{
				AddressStart = num4,
				DataCode = command[8],
				DbBlock = 0,
				Length = num3
			});
			if (!dataAreaFromS7Address2.IsSuccess)
			{
				throw new Exception(dataAreaFromS7Address2.Message);
			}
			return PackReadCTCommandBack(dataAreaFromS7Address2.Content.GetBytes(num4 * 2, num3 * 2), (command[3] == 30) ? 3 : 5);
		}
		ushort num5 = base.ByteTransform.TransUInt16(command, 4);
		if (command[3] == 4)
		{
			num5 *= 2;
		}
		ushort dbBlock2 = base.ByteTransform.TransUInt16(command, 6);
		int num6 = (command[9] * 65536 + command[10] * 256 + command[11]) / 8;
		OperateResult<SoftBuffer> dataAreaFromS7Address3 = GetDataAreaFromS7Address(new S7AddressData
		{
			AddressStart = num6,
			DataCode = command[8],
			DbBlock = dbBlock2,
			Length = num5
		});
		if (!dataAreaFromS7Address3.IsSuccess)
		{
			return PackReadWordCommandBack((short)dataAreaFromS7Address3.ErrorCode, null);
		}
		if (num6 + num5 > dataAreaFromS7Address3.Content.Capacity)
		{
			return PackReadWordCommandBack(5, null);
		}
		return PackReadWordCommandBack(0, dataAreaFromS7Address3.Content.GetBytes(num6, num5));
	}

	private byte[] PackReadWordCommandBack(short err, byte[] result)
	{
		if (err > 0)
		{
			byte[] buffer = new byte[4];
			BitConverter.GetBytes(err).CopyTo(buffer, 0);
			return buffer;
		}
		byte[] buffer2 = new byte[4 + result.Length];
		buffer2[0] = byte.MaxValue;
		buffer2[1] = 4;
		base.ByteTransform.TransByte((ushort)(result.Length * 8)).CopyTo(buffer2, 2);
		result.CopyTo(buffer2, 4);
		return buffer2;
	}

	private byte[] PackReadCTCommandBack(byte[] result, int dataLength)
	{
		byte[] buffer = new byte[4 + result.Length * dataLength / 2];
		buffer[0] = byte.MaxValue;
		buffer[1] = 9;
		base.ByteTransform.TransByte((ushort)(buffer.Length - 4)).CopyTo(buffer, 2);
		for (int i = 0; i < result.Length / 2; i++)
		{
			result.SelectMiddle(i * 2, 2).CopyTo(buffer, 4 + dataLength - 2 + i * dataLength);
		}
		return buffer;
	}

	private byte[] PackReadBitCommandBack(bool value)
	{
		return new byte[5]
		{
			255,
			3,
			0,
			1,
			(byte)(value ? 1u : 0u)
		};
	}

	/// <summary>
	/// 创建返回的报文信息
	/// </summary>
	/// <param name="packCommand">接收到的报文命令</param>
	/// <param name="status">返回的状态信息</param>
	/// <returns>返回的原始字节数组</returns>
	private byte[] PackWriteBack(byte[] packCommand, byte status)
	{
		return PackWriteBack(packCommand, new byte[1] { status });
	}

	/// <summary>
	/// 创建返回的报文信息
	/// </summary>
	/// <param name="packCommand">接收到的报文命令</param>
	/// <param name="status">返回的状态信息</param>
	/// <returns>返回的原始字节数组</returns>
	protected virtual byte[] PackWriteBack(byte[] packCommand, byte[] status)
	{
		byte[] buffer = new byte[21 + status.Length];
		SoftBasic.HexStringToBytes("03 00 00 16 02 F0 80 32 03 00 00 00 01 00 02 00 01 00 00 05 01").CopyTo(buffer, 0);
		buffer[20] = (byte)status.Length;
		status.CopyTo(buffer, 21);
		buffer[2] = BitConverter.GetBytes(buffer.Length)[1];
		buffer[3] = BitConverter.GetBytes(buffer.Length)[0];
		return buffer;
	}

	private byte[] WriteByMessage(byte[] packCommand)
	{
		if (!base.EnableWrite)
		{
			return PackWriteBack(packCommand, 4);
		}
		int num = packCommand[18];
		byte[] buffer = new byte[num];
		int num2 = 19 + num * 12;
		for (int i = 0; i < num; i++)
		{
			if (packCommand[22 + 12 * i] == 2 || packCommand[22 + 12 * i] == 4)
			{
				ushort dbBlock = base.ByteTransform.TransUInt16(packCommand, 25 + 12 * i);
				int num3 = base.ByteTransform.TransInt16(packCommand, 23 + 12 * i);
				if (packCommand[22 + 12 * i] == 4)
				{
					num3 *= 2;
				}
				int num4 = 0;
				num4 = ((packCommand[27 + 12 * i] < 28 || packCommand[27 + 12 * i] > 31) ? ((packCommand[28 + 12 * i] * 65536 + packCommand[29 + 12 * i] * 256 + packCommand[30 + 12 * i]) / 8) : ((packCommand[28 + 12 * i] * 65536 + packCommand[29 + 12 * i] * 256 + packCommand[30 + 12 * i]) * 2));
				byte[] array2 = base.ByteTransform.TransByte(packCommand, num2 + 4, num3);
				num2 += 4 + num3;
				if (i < num - 1 && num3 % 2 == 1)
				{
					num2++;
				}
				OperateResult<SoftBuffer> dataAreaFromS7Address = GetDataAreaFromS7Address(new S7AddressData
				{
					DataCode = packCommand[27 + 12 * i],
					DbBlock = dbBlock,
					Length = 1
				});
				if (!dataAreaFromS7Address.IsSuccess)
				{
					buffer[i] = (byte)dataAreaFromS7Address.ErrorCode;
					continue;
				}
				if (num4 + array2.Length > dataAreaFromS7Address.Content.Capacity)
				{
					return PackWriteBack(packCommand, 5);
				}
				dataAreaFromS7Address.Content.SetBytes(array2, num4);
				buffer[i] = byte.MaxValue;
				continue;
			}
			ushort dbBlock2 = base.ByteTransform.TransUInt16(packCommand, 25 + 12 * i);
			int num5 = packCommand[28 + 12 * i] * 65536 + packCommand[29 + 12 * i] * 256 + packCommand[30 + 12 * i];
			int num6 = packCommand[num2 + 2] * 256 + packCommand[num2 + 3];
			int num7 = (num6 + 7) / 8;
			bool[] value = SoftBasic.ByteToBoolArray(packCommand.SelectMiddle(num2 + 4, num7), num6);
			num2 += 4 + num7;
			if (i < num - 1)
			{
				num2++;
			}
			OperateResult<SoftBuffer> dataAreaFromS7Address2 = GetDataAreaFromS7Address(new S7AddressData
			{
				DataCode = packCommand[27 + 12 * i],
				DbBlock = dbBlock2,
				Length = 1
			});
			if (!dataAreaFromS7Address2.IsSuccess)
			{
				buffer[i] = (byte)dataAreaFromS7Address2.ErrorCode;
				continue;
			}
			if (num5 >= dataAreaFromS7Address2.Content.Capacity * 8)
			{
				return PackWriteBack(packCommand, 5);
			}
			dataAreaFromS7Address2.Content.SetBool(value, num5);
			buffer[i] = byte.MaxValue;
		}
		return PackWriteBack(packCommand, buffer);
	}

	/// <summary>
	/// 新增一个独立的DB块数据区，如果这个DB块已经存在，则新增无效。
	/// Add a separate DB block data area, if the DB block already exists, the new one is invalid.
	/// </summary>
	/// <param name="db">DB块ID信息</param>
	/// <param name="length">指定新增的DB块长度信息</param>
	public void AddDbBlock(int db)
	{
		if (!dbBlockBuffer.ContainsKey(db))
		{
			dbBlockBuffer.Add(db, new SoftBuffer(65536));
		}
	}

	/// <summary>
	/// 新增一个独立的DB块数据区，如果这个DB块已经存在，则新增无效。
	/// Add a separate DB block data area, if the DB block already exists, the new one is invalid.
	/// </summary>
	/// <param name="db">DB块ID信息</param>
	/// <param name="length">指定新增的DB块长度信息</param>
	public void AddDbBlock(int db, int length)
	{
		if (!dbBlockBuffer.ContainsKey(db))
		{
			dbBlockBuffer.Add(db, new SoftBuffer(length));
		}
	}

	/// <summary>
	/// 移除指定的DB块数据区，如果这个DB块不存在的话，操作无效，本方法无法移除1，2，3的DB块
	/// Remove the specified DB block data area, if the DB block does not exist, the operation is invalid, and this method cannot remove the DB block of 1, 2, 3
	/// </summary>
	/// <param name="db">指定的ID信息</param>
	public void RemoveDbBlock(int db)
	{
		if (db != 1 && db != 2 && db != 3 && dbBlockBuffer.ContainsKey(db))
		{
			dbBlockBuffer.Remove(db);
		}
	}

	/// <summary>
	/// 获取当前的DB块数据区信息，返回一个字典对象，Key为DB块的ID，Value为对应的SoftBuffer对象
	/// Get the current DB block data area information, returning a dictionary object, where the key is the ID of the DB block and the value is the corresponding SoftBuffer object.
	/// </summary>
	/// <returns>词典对象</returns>
	public Dictionary<int, SoftBuffer> GetDbBlockBuffer()
	{
		return dbBlockBuffer;
	}

	protected override void LoadFromBytes(byte[] content)
	{
		if (content.Length < 393216)
		{
			throw new Exception("File is not correct");
		}
		inputBuffer.SetBytes(content, 0, 0, 65536);
		outputBuffer.SetBytes(content, 65536, 0, 65536);
		memeryBuffer.SetBytes(content, 131072, 0, 65536);
		dbBlockBuffer[1].SetBytes(content, 196608, 0, 65536);
		dbBlockBuffer[2].SetBytes(content, 262144, 0, 65536);
		dbBlockBuffer[3].SetBytes(content, 327680, 0, 65536);
		if (content.Length <= 458752)
		{
			return;
		}
		countBuffer.SetBytes(content, 458752, 0, 131072);
		timerBuffer.SetBytes(content, 589824, 0, 131072);
		if (content.Length <= 720896)
		{
			return;
		}
		int num = 720896;
		while (num < content.Length)
		{
			int key = BitConverter.ToInt32(content, num);
			int num2 = BitConverter.ToInt32(content, num + 4);
			num += 8;
			if (num + num2 > content.Length)
			{
				break;
			}
			if (!dbBlockBuffer.ContainsKey(key))
			{
				dbBlockBuffer.Add(key, new SoftBuffer(num2));
			}
			dbBlockBuffer[key].SetBytes(content, num, 0, num2);
			num += num2;
		}
	}

	protected override byte[] SaveToBytes()
	{
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(inputBuffer.GetBytes());
		memoryStream.Write(outputBuffer.GetBytes());
		memoryStream.Write(memeryBuffer.GetBytes());
		memoryStream.Write(dbBlockBuffer[1].GetBytes());
		memoryStream.Write(dbBlockBuffer[2].GetBytes());
		memoryStream.Write(dbBlockBuffer[3].GetBytes());
		memoryStream.Write(inputBuffer.GetBytes());
		memoryStream.Write(countBuffer.GetBytes());
		memoryStream.Write(timerBuffer.GetBytes());
		foreach (KeyValuePair<int, SoftBuffer> item in dbBlockBuffer)
		{
			if (item.Key != 1 && item.Key != 2 && item.Key != 3)
			{
				memoryStream.Write(BitConverter.GetBytes(item.Key));
				memoryStream.Write(BitConverter.GetBytes(item.Value.Capacity));
				memoryStream.Write(item.Value.GetBytes());
			}
		}
		return memoryStream.ToArray();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			systemBuffer?.Dispose();
			inputBuffer?.Dispose();
			outputBuffer?.Dispose();
			memeryBuffer?.Dispose();
			countBuffer?.Dispose();
			timerBuffer?.Dispose();
			foreach (SoftBuffer value in dbBlockBuffer.Values)
			{
				value?.Dispose();
			}
			aiBuffer?.Dispose();
			aqBuffer?.Dispose();
		}
		base.Dispose(disposing);
	}

	public override string ToString()
	{
		return $"SiemensS7Server[{base.Port}]";
	}
}
