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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.AllenBradley;

/// <summary>
/// AB PLC的数据通信类，使用CIP协议实现，适用1756，1769等型号，支持使用标签的形式进行读写操作，支持标量数据，一维数组，二维数组，三维数组等等。如果是局部变量，那么使用 Program:MainProgram.[变量名]。
/// The data communication class of AB PLC is implemented using the CIP protocol. It is suitable for 1756, 1769 and other models.
/// It supports reading and writing in the form of tags, scalar data, one-dimensional array, two-dimensional array,
/// three-dimensional array, and so on. If it is a local variable, use the Program:MainProgram.[Variable name].
/// </summary>
/// <remarks>
/// thanks 江阴-  ∮溪风-⊙_⌒ help test the dll
/// thanks 上海-null 测试了这个dll
/// 默认的地址就是PLC里的TAG名字，比如A，B，C；如果你需要读取的数据是一个数组，那么A就是默认的A[0]，如果想要读取偏移量为10的数据，那么地址为A[10]，
/// 多维数组同理，使用A[10,10,10]的操作。
/// 假设你读取的是局部变量，那么使用 Program:MainProgram.变量名
/// 目前适用的系列为1756 ControlLogix, 1756 GuardLogix, 1769 CompactLogix, 1769 Compact GuardLogix, 1789SoftLogix, 5069 CompactLogix, 5069 Compact GuardLogix, Studio 5000 Logix Emulate
/// 如果你有个Bool数组要读取，变量名为 A, 那么读第0个位，可以通过 ReadBool("A")，但是第二个位需要使用
/// ReadBoolArray("A[0]")   // 返回32个bool长度，0-31的索引，如果我想读取32-63的位索引，就需要 ReadBoolArray("A[1]") ，以此类推。
/// 地址可以携带站号信息，只要在前面加上slot=2;即可，这就是访问站号2的数据了，例如 slot=2;AAA，如果使用了自定义的消息路由，例如：[IP or Hostname],1,[Optional Routing Path],CPU Slot 172.20.1.109,1,[15,2,18,1],12
/// 在实例化之后，连接PLC之前，需要调用如下代码 plc.MessageRouter = new MessageRouter( "1.15.2.18.1.12" )
/// </remarks>
public class AllenBradleyNet : DeviceTcpNet, IReadWriteCip, IReadWriteNet
{
	private long contextId = 0L;

	private Dictionary<string, int> boolValueDataBits = new Dictionary<string, int>();

	private object boolLock = new object();

	/// <summary>
	/// Gets or sets a value indicating whether the context ID is automatically incremented. The default is true
	/// </summary>
	public bool ContextIdAutoIncrement { get; set; } = true;

	/// <summary>
	/// The current session handle, which is determined by the PLC when communicating with the PLC handshake
	/// </summary>
	public uint SessionHandle { get; protected set; }

	/// <summary>
	/// Gets or sets the slot number information for the current plc, which should be set before connections
	/// </summary>
	public byte Slot { get; set; } = 0;

	/// <summary>
	/// port and slot information
	/// </summary>
	public byte[] PortSlot { get; set; }

	/// <summary>
	/// 获取或设置整个交互指令的控制码，默认为0x6F，通常不需要修改
	/// Gets or sets the control code of the entire interactive instruction. The default is 0x6F, and usually does not need to be modified.
	/// </summary>
	public ushort CipCommand { get; set; } = 111;

	/// <summary>
	/// 获取或设置当前的通信的消息路由信息，可以实现一些复杂情况的通信，数据包含背板号，路由参数，slot，例如：1.15.2.18.1.1
	/// Get or set the message routing information of the current communication, which can realize some complicated communication.
	/// The data includes the backplane number, routing parameters, and slot, for example: 1.15.2.18.1.1
	/// </summary>
	public MessageRouter MessageRouter { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the current context meets the required conditions.
	/// </summary>
	public bool ContextCheck { get; set; } = false;

	/// <summary>
	/// 获取或设置是否在读取数组的时候使用片段读取功能，默认是开启的。
	/// Get or set whether to use the fragment reading function when reading the array. By default, it is enabled.
	/// </summary>
	public bool ReadArrayUseSegment { get; set; } = true;

	public Dictionary<string, int> TagValueAndDataBits => boolValueDataBits;

	/// <summary>
	/// Instantiate a communication object for a Allenbradley PLC protocol
	/// </summary>
	public AllenBradleyNet()
	{
		base.WordLength = 2;
		base.ByteTransform = new RegularByteTransform();
	}

	/// <summary>
	/// Instantiate a communication object for a Allenbradley PLC protocol
	/// </summary>
	/// <param name="ipAddress">PLC IpAddress</param>
	/// <param name="port">PLC Port</param>
	public AllenBradleyNet(string ipAddress, int port = 44818)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new AllenBradleyMessage(ContextCheck);
	}

	public override byte[] PackCommandWithHeader(byte[] command)
	{
		byte[] bytes = BitConverter.GetBytes(ContextIdAutoIncrement ? Interlocked.Increment(ref contextId) : contextId);
		return AllenBradleyHelper.PackRequestHeader(CipCommand, SessionHandle, command, bytes);
	}

	protected override OperateResult InitializationOnConnect()
	{
		Interlocked.Exchange(ref contextId, 0L);
		ClearBoolTypeCache();
		OperateResult<byte[]> operateResult = ReadFromCoreServer(CommunicationPipe, AllenBradleyHelper.RegisterSessionHandle(), hasResponseData: true, usePackAndUnpack: false);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult operateResult2 = AllenBradleyHelper.CheckResponse(operateResult.Content);
		if (!operateResult2.IsSuccess && operateResult2.ErrorCode != 1)
		{
			return operateResult2;
		}
		if (operateResult.Content.Length >= 8)
		{
			SessionHandle = BitConverter.ToUInt32(operateResult.Content, 4);
		}
		if (MessageRouter != null)
		{
			byte[] routerCIP = MessageRouter.GetRouterCIP();
			OperateResult<byte[]> operateResult3 = ReadFromCoreServer(CommunicationPipe, AllenBradleyHelper.PackRequestHeader(111, SessionHandle, AllenBradleyHelper.PackCommandSpecificData(new byte[4], AllenBradleyHelper.PackCommandSingleService(routerCIP, 178, isConnected: false, 0))), hasResponseData: true, usePackAndUnpack: false);
			if (!operateResult3.IsSuccess)
			{
				return operateResult3;
			}
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override OperateResult ExtraOnDisconnect()
	{
		if (CommunicationPipe != null)
		{
			OperateResult<byte[]> operateResult = ReadFromCoreServer(CommunicationPipe, AllenBradleyHelper.UnRegisterSessionHandle(SessionHandle), hasResponseData: true, usePackAndUnpack: false);
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override async Task<OperateResult> InitializationOnConnectAsync()
	{
		Interlocked.Exchange(ref contextId, 0L);
		ClearBoolTypeCache();
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(CommunicationPipe, AllenBradleyHelper.RegisterSessionHandle(), hasResponseData: true, usePackAndUnpack: false).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		OperateResult check = AllenBradleyHelper.CheckResponse(read.Content);
		if (!check.IsSuccess && check.ErrorCode != 1)
		{
			return check;
		}
		if (read.Content.Length >= 8)
		{
			SessionHandle = BitConverter.ToUInt32(read.Content, 4);
		}
		if (MessageRouter != null)
		{
			byte[] cip = MessageRouter.GetRouterCIP();
			OperateResult<byte[]> messageRouter = await ReadFromCoreServerAsync(CommunicationPipe, AllenBradleyHelper.PackRequestHeader(111, SessionHandle, AllenBradleyHelper.PackCommandSpecificData(new byte[4], AllenBradleyHelper.PackCommandSingleService(cip, 178, isConnected: false, 0))), hasResponseData: true, usePackAndUnpack: false).ConfigureAwait(continueOnCapturedContext: false);
			if (!messageRouter.IsSuccess)
			{
				return messageRouter;
			}
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override async Task<OperateResult> ExtraOnDisconnectAsync()
	{
		if (CommunicationPipe != null)
		{
			OperateResult<byte[]> read = await ReadFromCoreServerAsync(CommunicationPipe, AllenBradleyHelper.UnRegisterSessionHandle(SessionHandle), hasResponseData: true, usePackAndUnpack: false);
			if (!read.IsSuccess)
			{
				return read;
			}
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 创建一个读取标签的报文指定，标签地址可以手动动态指定slot编号，例如 slot=2;AAA
	/// Build a read command bytes, The label address can manually specify the slot number dynamically, for example slot=2;AAA
	/// </summary>
	/// <param name="address">the address of the tag name</param>
	/// <param name="length">Array information, if not arrays, is 1</param>
	/// <returns>Message information that contains the result object</returns>
	public virtual OperateResult<byte[]> BuildReadCommand(string[] address, ushort[] length)
	{
		if (address == null || length == null)
		{
			return new OperateResult<byte[]>("address or length is null");
		}
		if (address.Length != length.Length)
		{
			return new OperateResult<byte[]>("address and length is not same array");
		}
		try
		{
			if (address.Length == 1 && AllenBradleyHelper.IsAddressClassAndAttribute(address[0]))
			{
				byte[] cip = AllenBradleyHelper.PackRequsetRead(address[0], length[0]);
				byte[] value = AllenBradleyHelper.PackCommandSpecificData(new byte[4], AllenBradleyHelper.PackCommandService(cip));
				return OperateResult.CreateSuccessResult(value);
			}
			byte b = Slot;
			List<byte[]> list = new List<byte[]>();
			for (int i = 0; i < address.Length; i++)
			{
				b = (byte)HslHelper.ExtractParameter(ref address[i], "slot", Slot);
				list.Add(AllenBradleyHelper.PackRequsetRead(address[i], length[i]));
			}
			byte[] value2 = AllenBradleyHelper.PackCommandSpecificData(new byte[4], PackCommandService(PortSlot ?? new byte[2] { 1, b }, list.ToArray()));
			return OperateResult.CreateSuccessResult(value2);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("Address Wrong:" + ex.Message);
		}
	}

	/// <summary>
	/// 创建一个读取多标签的报文
	/// Build a read command bytes
	/// </summary>
	/// <param name="address">The address of the tag name</param>
	/// <returns>Message information that contains the result object</returns>
	public OperateResult<byte[]> BuildReadCommand(string[] address)
	{
		if (address == null)
		{
			return new OperateResult<byte[]>("address or length is null");
		}
		ushort[] array = new ushort[address.Length];
		for (int i = 0; i < address.Length; i++)
		{
			array[i] = 1;
		}
		return BuildReadCommand(address, array);
	}

	/// <summary>
	/// Create a written message instruction
	/// </summary>
	/// <param name="address">The address of the tag name</param>
	/// <param name="typeCode">Data type</param>
	/// <param name="data">Source Data</param>
	/// <param name="length">In the case of arrays, the length of the array</param>
	/// <returns>Message information that contains the result object</returns>
	protected virtual OperateResult<List<byte[]>> BuildWriteCommand(string address, ushort typeCode, byte[] data, int length = 1)
	{
		try
		{
			if (AllenBradleyHelper.IsAddressClassAndAttribute(address))
			{
				byte[] cip = AllenBradleyHelper.PackRequestWrite(address, typeCode, data, length, isConnectedAddress: false, GetBoolWritePadding());
				byte[] item = AllenBradleyHelper.PackCommandSpecificData(new byte[4], AllenBradleyHelper.PackCommandService(cip));
				return OperateResult.CreateSuccessResult(new List<byte[]> { item });
			}
			byte b = (byte)HslHelper.ExtractParameter(ref address, "slot", Slot);
			int num = HslHelper.ExtractParameter(ref address, "x", -1);
			if (num == 83 || num == 82)
			{
				int num2 = 0;
				List<byte[]> list = SoftBasic.ArraySplitByLength(data, 474);
				for (int i = 0; i < list.Count; i++)
				{
					byte[] array = AllenBradleyHelper.PackRequestWriteSegment(address, typeCode, list[i], num2, length);
					num2 += list[i].Length;
					byte[] value = AllenBradleyHelper.PackCommandSpecificData(new byte[4], PackCommandService(PortSlot ?? new byte[2] { 1, b }, array));
					list[i] = value;
				}
				return OperateResult.CreateSuccessResult(list);
			}
			byte[] array2 = AllenBradleyHelper.PackRequestWrite(address, typeCode, data, length, isConnectedAddress: false, GetBoolWritePadding());
			byte[] item2 = AllenBradleyHelper.PackCommandSpecificData(new byte[4], PackCommandService(PortSlot ?? new byte[2] { 1, b }, array2));
			return OperateResult.CreateSuccessResult(new List<byte[]> { item2 });
		}
		catch (Exception ex)
		{
			return new OperateResult<List<byte[]>>("Address Wrong:" + ex.Message);
		}
	}

	/// <summary>
	/// 当进行bool写入的时候，是否需要补齐到字节长度，默认不需要
	/// When performing bool writing, whether it is necessary to complement to the byte length is not required by default
	/// </summary>
	/// <returns>是否需要字节对齐</returns>
	protected virtual bool GetBoolWritePadding()
	{
		return false;
	}

	/// <summary>
	/// Create a written message instruction
	/// </summary>
	/// <param name="address">The address of the tag name</param>
	/// <param name="data">Bool Data</param>
	/// <returns>Message information that contains the result object</returns>
	public OperateResult<byte[]> BuildWriteCommand(string address, bool data)
	{
		try
		{
			byte b = (byte)HslHelper.ExtractParameter(ref address, "slot", Slot);
			address = AllenBradleyHelper.AnalysisArrayIndex(address, out var arrayIndex);
			OperateResult<int> boolValueTypeBitLength = GetBoolValueTypeBitLength(address);
			if (!boolValueTypeBitLength.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(boolValueTypeBitLength);
			}
			byte[] array = AllenBradleyHelper.PackRequestReadModifyWrite(address, arrayIndex, data, boolValueTypeBitLength.Content);
			byte[] value = AllenBradleyHelper.PackCommandSpecificData(new byte[4], PackCommandService(PortSlot ?? new byte[2] { 1, b }, array));
			return OperateResult.CreateSuccessResult(value);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("Address Wrong:" + ex.Message);
		}
	}

	private OperateResult CheckResponse(byte[] response)
	{
		OperateResult operateResult = AllenBradleyHelper.CheckResponse(response);
		if (!operateResult.IsSuccess && operateResult.ErrorCode == 100)
		{
			CommunicationPipe.RaisePipeError();
		}
		return operateResult;
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 读取指定地址的二进制数据内容，长度为地址长度，一般都是1，除非读取数组时，如果需要强制使用 片段读取功能码，则地址里携带 x=0x52; 或是 x=82; 则强制使用片段读取。
	/// Read the binary data content of the specified address, the length is the address length, generally 1, unless the array is read, if you need to force the fragment reading function code,
	/// the address carries x=0x52; or x=82; then the fragment read is forced.
	/// </summary>
	/// <param name="address">Address format of the node</param>
	/// <param name="length">In the case of arrays, the length of the array</param>
	/// <returns>Result data with result object</returns>
	/// <remarks>
	/// 使用片段读取的时候，可以读取一些数量量非常大的地址，例如一个结构体标签有100_000个字节长度的时候。
	/// When using fragment reading, you can read a very large number of addresses, such as when a struct tag is 100_000 bytes long.
	/// </remarks>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		HslHelper.ExtractParameter(ref address, "type", 0);
		int num = HslHelper.ExtractParameter(ref address, "x", -1);
		if (num == 82 || num == 83)
		{
			return ReadSegment(address, 0, length);
		}
		if (length > 1 && ReadArrayUseSegment)
		{
			return ReadSegment(address, 0, length);
		}
		return Read(new string[1] { address }, new ushort[1] { length });
	}

	[HslMqttApi("ReadAddress", "")]
	/// <summary>
	/// [商业授权] 批量读取多地址的数据信息，例如我可以读取两个标签的数据 "A","B[0]"，每个地址的数据长度为1，表示一个数据，最终读取返回的是一整个的字节数组，需要自行解析
	/// [Authorization] Batch read data information of multiple addresses, for example, I can read the data of two tags "A", "B[0]", the data length of each address is 1,
	/// which means one data, and the final read returns a The entire byte array, which needs to be parsed by itself
	/// </summary>
	/// <param name="address">Name of the node</param>
	/// <returns>Result data with result object</returns>
	public OperateResult<byte[]> Read(string[] address)
	{
		if (address == null)
		{
			return new OperateResult<byte[]>("address can not be null");
		}
		ushort[] array = new ushort[address.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = 1;
		}
		return Read(address, array);
	}

	/// <summary>
	/// [商业授权] 批量读取多地址的数据信息，例如我可以读取两个标签的数据 "A","B[0]"， 长度为 [1, 5]，返回的是一整个的字节数组，需要自行解析
	/// [Authorization] Read the data information of multiple addresses in batches. For example, I can read the data "A", "B[0]" of two tags,
	/// the length is [1, 5], and the return is an entire byte array, and I need to do it myself Parsing
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <param name="length">如果是数组，就为数组长度 -&gt; In the case of arrays, the length of the array</param>
	/// <returns>带有结果对象的结果数据 -&gt; Result data with result object</returns>
	public OperateResult<byte[]> Read(string[] address, ushort[] length)
	{
		if (address != null && address.Length > 1 && !Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[], ushort, bool> operateResult = ReadWithType(address, length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content1);
	}

	/// <summary>
	/// [商业授权] 批量读取多地址的数据信息，例如我可以读取两个标签的数据 "A","B[0]"， 长度为 [1, 5]，返回的是一整个的字节数组，需要自行解析
	/// [Authorization] Read the data information of multiple addresses in batches. For example, I can read the data "A", "B[0]" of two tags,
	/// the length is [1, 5], and the return is an entire byte array, and I need to do it myself Parsing
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <param name="length">如果是数组，就为数组长度 -&gt; In the case of arrays, the length of the array</param>
	/// <returns>带有结果对象的结果数据 -&gt; Result data with result object</returns>
	public OperateResult<AbTagValue[]> ReadTags(string[] address, ushort[] length)
	{
		if (address != null && address.Length > 1 && !Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<AbTagValue[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> operateResult = BuildReadCommand(address, length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<AbTagValue[]>(operateResult);
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<AbTagValue[]>(operateResult2);
		}
		OperateResult operateResult3 = CheckResponse(operateResult2.Content);
		if (!operateResult3.IsSuccess)
		{
			return OperateResult.CreateFailedResult<AbTagValue[]>(operateResult3);
		}
		OperateResult<List<AbTagValue>, bool> operateResult4 = AllenBradleyHelper.ExtractActualDataList(operateResult2.Content, isRead: true);
		if (!operateResult4.IsSuccess)
		{
			return OperateResult.CreateFailedResult<AbTagValue[]>(operateResult4);
		}
		return OperateResult.CreateSuccessResult(operateResult4.Content1.ToArray());
	}

	private OperateResult<byte[], ushort, bool> ReadWithType(string[] address, ushort[] length)
	{
		OperateResult<byte[]> operateResult = BuildReadCommand(address, length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[], ushort, bool>(operateResult);
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[], ushort, bool>(operateResult2);
		}
		OperateResult operateResult3 = CheckResponse(operateResult2.Content);
		if (!operateResult3.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[], ushort, bool>(operateResult3);
		}
		return AllenBradleyHelper.ExtractActualData(operateResult2.Content, isRead: true);
	}

	[HslMqttApi("ReadSegment", "")]
	/// <summary>
	/// Read Segment Data Array form plc, use address tag name
	/// </summary>
	/// <param name="address">Tag name in plc</param>
	/// <param name="startIndex">array start index, uint byte index</param>
	/// <param name="length">array length, data item length</param>
	/// <returns>Results Bytes</returns>
	public OperateResult<byte[]> ReadSegment(string address, int startIndex, int length)
	{
		try
		{
			List<byte> list = new List<byte>();
			OperateResult<byte[], ushort, bool> operateResult2;
			do
			{
				OperateResult<byte[]> operateResult = ReadCipFromServer(AllenBradleyHelper.PackRequestReadSegment(address, startIndex, length));
				if (!operateResult.IsSuccess)
				{
					return operateResult;
				}
				operateResult2 = AllenBradleyHelper.ExtractActualData(operateResult.Content, isRead: true);
				if (!operateResult2.IsSuccess)
				{
					return OperateResult.CreateFailedResult<byte[]>(operateResult2);
				}
				startIndex += operateResult2.Content1.Length;
				if (list.Count != 0 && operateResult2.Content2 == 672 && operateResult2.Content1.Length >= 2 && operateResult2.Content1[0] == list[0] && operateResult2.Content1[1] == list[1])
				{
					list.AddRange(operateResult2.Content1.Skip(2));
				}
				else
				{
					list.AddRange(operateResult2.Content1);
				}
				if (operateResult2.Content2 == 672)
				{
					startIndex -= 2;
				}
			}
			while (operateResult2.Content3);
			return OperateResult.CreateSuccessResult(list.ToArray());
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("Address Wrong:" + ex.Message);
		}
	}

	private OperateResult<byte[]> ReadByCips(params byte[][] cips)
	{
		OperateResult<byte[]> operateResult = ReadCipFromServer(cips);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[], ushort, bool> operateResult2 = AllenBradleyHelper.ExtractActualData(operateResult.Content, isRead: true);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(operateResult2.Content1);
	}

	/// <summary>
	/// 使用CIP报文和服务器进行核心的数据交换
	/// </summary>
	/// <param name="cips">Cip commands</param>
	/// <returns>Results Bytes</returns>
	public OperateResult<byte[]> ReadCipFromServer(params byte[][] cips)
	{
		byte[] send = AllenBradleyHelper.PackCommandSpecificData(new byte[4], PackCommandService(PortSlot ?? new byte[2] { 1, Slot }, cips.ToArray()));
		OperateResult<byte[]> operateResult = ReadFromCoreServer(send);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult operateResult2 = CheckResponse(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content);
	}

	/// <summary>
	/// 使用EIP报文和服务器进行核心的数据交换
	/// </summary>
	/// <param name="eip">eip commands</param>
	/// <returns>Results Bytes</returns>
	public OperateResult<byte[]> ReadEipFromServer(params byte[][] eip)
	{
		byte[] send = AllenBradleyHelper.PackCommandSpecificData(eip);
		OperateResult<byte[]> operateResult = ReadFromCoreServer(send);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult operateResult2 = CheckResponse(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content);
	}

	[HslMqttApi("ReadBool", "")]
	/// <summary>
	/// 读取单个的bool数据信息，如果读取的是单bool变量，就直接写变量名，如果是由int组成的bool数组的一个值，一律带"i="开头访问，例如"i=A[0]"
	/// Read a single bool data information, if it is a single bool variable, write the variable name directly,
	/// if it is a value of a bool array composed of int, it is always accessed with "i=" at the beginning, for example, "i=A[0]"
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <returns>带有结果对象的结果数据 -&gt; Result data with result info</returns>
	public override OperateResult<bool> ReadBool(string address)
	{
		if (address.StartsWith("i="))
		{
			return ByteTransformHelper.GetResultFromArray(ReadBool(address, 1));
		}
		OperateResult<byte[]> operateResult = Read(address, 1);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool>(operateResult);
		}
		return OperateResult.CreateSuccessResult(base.ByteTransform.TransBool(operateResult.Content, 0));
	}

	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		if (address.StartsWith("i="))
		{
			address = address.Substring(2);
			address = AllenBradleyHelper.AnalysisArrayIndex(address, out var arrayIndex);
			OperateResult<int> boolValueTypeBitLength = GetBoolValueTypeBitLength(address);
			if (!boolValueTypeBitLength.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(boolValueTypeBitLength);
			}
			int content = boolValueTypeBitLength.Content;
			string text = ((arrayIndex / content == 0) ? "" : $"[{arrayIndex / content}]");
			ushort length2 = (ushort)HslHelper.CalculateOccupyLength(arrayIndex, length, content);
			OperateResult<byte[]> operateResult = Read(address + text, length2);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(operateResult);
			}
			return OperateResult.CreateSuccessResult(operateResult.Content.ToBoolArray().SelectMiddle(arrayIndex % content, length));
		}
		OperateResult<byte[]> operateResult2 = Read(address, length);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(SoftBasic.ByteToBoolArray(operateResult2.Content, length));
	}

	[HslMqttApi("ReadBoolArrayAddress", "")]
	/// <summary>
	/// 批量读取的bool数组信息，如果你有个Bool数组变量名为 A, 那么读第0个位，可以通过 ReadBool("A")，但是第二个位需要使用
	/// ReadBoolArray("A[0]")   // 返回32个bool长度，0-31的索引，如果我想读取32-63的位索引，就需要 ReadBoolArray("A[1]") ，以此类推。
	/// For batch read bool array information, if you have a Bool array variable named A, then you can read the 0th bit through ReadBool("A"),
	/// but the second bit needs to use ReadBoolArray("A[0]" ) // Returns the length of 32 bools, the index is 0-31,
	/// if I want to read the bit index of 32-63, I need ReadBoolArray("A[1]"), and so on.
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <returns>带有结果对象的结果数据 -&gt; Result data with result info</returns>
	public OperateResult<bool[]> ReadBoolArray(string address)
	{
		OperateResult<byte[]> operateResult = Read(address, 1);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content.ToBoolArray());
	}

	[HslMqttApi("ReadByte", "")]
	/// <summary>
	/// 读取PLC的byte类型的数据
	/// Read the byte type of PLC data
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <returns>带有结果对象的结果数据 -&gt; Result data with result info</returns>
	public OperateResult<byte> ReadByte(string address)
	{
		return ByteTransformHelper.GetResultFromArray(Read(address, 1));
	}

	/// <summary>
	/// 从PLC里读取一个指定标签名的原始数据信息及其数据类型信息
	/// Read the original data information of a specified tag name and its data type information from the PLC
	/// </summary>
	/// <param name="address">PLC的标签地址信息</param>
	/// <param name="length">读取的数据长度</param>
	/// <returns>包含原始数据信息及数据类型的结果对象</returns>
	/// <remarks>
	/// 数据类型的定义，可以参考  的常量资源信息。
	/// </remarks>
	public OperateResult<ushort, byte[]> ReadTag(string address, ushort length = 1)
	{
		HslHelper.ExtractParameter(ref address, "type");
		OperateResult<byte[], ushort, bool> operateResult = ReadWithType(new string[1] { address }, new ushort[1] { length });
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<ushort, byte[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content2, operateResult.Content1);
	}

	/// <summary>
	/// 读取指定地址的二进制数据内容，长度为地址长度，一般都是1，除非读取数组时，如果需要强制使用 片段读取功能码，则地址里携带 x=0x52; 或是 x=82; 则强制使用片段读取。
	/// Read the binary data content of the specified address, the length is the address length, generally 1, unless the array is read, if you need to force the fragment reading function code,
	/// the address carries x=0x52; or x=82; then the fragment read is forced.
	/// </summary>
	/// <param name="address">Address format of the node</param>
	/// <param name="length">In the case of arrays, the length of the array</param>
	/// <returns>Result data with result object</returns>
	/// <remarks>
	/// 使用片段读取的时候，可以读取一些数量量非常大的地址，例如一个结构体标签有100_000个字节长度的时候。
	/// When using fragment reading, you can read a very large number of addresses, such as when a struct tag is 100_000 bytes long.
	/// </remarks>
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		HslHelper.ExtractParameter(ref address, "type", 0);
		int x = HslHelper.ExtractParameter(ref address, "x", -1);
		if (x == 82 || x == 83)
		{
			return await ReadSegmentAsync(address, 0, length).ConfigureAwait(continueOnCapturedContext: false);
		}
		if (length > 1 && ReadArrayUseSegment)
		{
			return await ReadSegmentAsync(address, 0, length).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await ReadAsync(new string[1] { address }, new ushort[1] { length }).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// [商业授权] 批量读取多地址的数据信息，例如我可以读取两个标签的数据 "A","B[0]"，每个地址的数据长度为1，表示一个数据，最终读取返回的是一整个的字节数组，需要自行解析
	/// [Authorization] Batch read data information of multiple addresses, for example, I can read the data of two tags "A", "B[0]", the data length of each address is 1,
	/// which means one data, and the final read returns a The entire byte array, which needs to be parsed by itself
	/// </summary>
	/// <param name="address">Name of the node</param>
	/// <returns>Result data with result object</returns>
	public async Task<OperateResult<byte[]>> ReadAsync(string[] address)
	{
		if (address == null)
		{
			return new OperateResult<byte[]>("address can not be null");
		}
		ushort[] length = new ushort[address.Length];
		for (int i = 0; i < length.Length; i++)
		{
			length[i] = 1;
		}
		return await ReadAsync(address, length);
	}

	/// <summary>
	/// [商业授权] 批量读取多地址的数据信息，例如我可以读取两个标签的数据 "A","B[0]"， 长度为 [1, 5]，返回的是一整个的字节数组，需要自行解析
	/// [Authorization] Read the data information of multiple addresses in batches. For example, I can read the data "A", "B[0]" of two tags,
	/// the length is [1, 5], and the return is an entire byte array, and I need to do it myself Parsing
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <param name="length">如果是数组，就为数组长度 -&gt; In the case of arrays, the length of the array</param>
	/// <returns>带有结果对象的结果数据 -&gt; Result data with result object</returns>
	public async Task<OperateResult<byte[]>> ReadAsync(string[] address, ushort[] length)
	{
		if (address != null && address.Length > 1 && !Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[], ushort, bool> read = await ReadWithTypeAsync(address, length).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return OperateResult.CreateSuccessResult(read.Content1);
	}

	/// <summary>
	/// [商业授权] 批量读取多地址的数据信息，例如我可以读取两个标签的数据 "A","B[0]"， 长度为 [1, 5]，返回的是一整个的字节数组，需要自行解析
	/// [Authorization] Read the data information of multiple addresses in batches. For example, I can read the data "A", "B[0]" of two tags,
	/// the length is [1, 5], and the return is an entire byte array, and I need to do it myself Parsing
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <param name="length">如果是数组，就为数组长度 -&gt; In the case of arrays, the length of the array</param>
	/// <returns>带有结果对象的结果数据 -&gt; Result data with result object</returns>
	public async Task<OperateResult<AbTagValue[]>> ReadTagsAsync(string[] address, ushort[] length)
	{
		if (address != null && address.Length > 1 && !Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<AbTagValue[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> command = BuildReadCommand(address, length);
		if (!command.IsSuccess)
		{
			return OperateResult.CreateFailedResult<AbTagValue[]>(command);
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<AbTagValue[]>(read);
		}
		OperateResult check = CheckResponse(read.Content);
		if (!check.IsSuccess)
		{
			return OperateResult.CreateFailedResult<AbTagValue[]>(check);
		}
		OperateResult<List<AbTagValue>, bool> extra = AllenBradleyHelper.ExtractActualDataList(read.Content, isRead: true);
		if (!extra.IsSuccess)
		{
			return OperateResult.CreateFailedResult<AbTagValue[]>(extra);
		}
		return OperateResult.CreateSuccessResult(extra.Content1.ToArray());
	}

	private async Task<OperateResult<byte[], ushort, bool>> ReadWithTypeAsync(string[] address, ushort[] length)
	{
		OperateResult<byte[]> command = BuildReadCommand(address, length);
		if (!command.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[], ushort, bool>(command);
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[], ushort, bool>(read);
		}
		OperateResult check = CheckResponse(read.Content);
		if (!check.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[], ushort, bool>(check);
		}
		return AllenBradleyHelper.ExtractActualData(read.Content, isRead: true);
	}

	/// <summary>
	/// Read Segment Data Array form plc, use address tag name
	/// </summary>
	/// <param name="address">Tag name in plc</param>
	/// <param name="startIndex">array start index, uint byte index</param>
	/// <param name="length">array length, data item length</param>
	/// <returns>Results Bytes</returns>
	public async Task<OperateResult<byte[]>> ReadSegmentAsync(string address, int startIndex, int length)
	{
		try
		{
			List<byte> bytesContent = new List<byte>();
			OperateResult<byte[], ushort, bool> analysis;
			do
			{
				OperateResult<byte[]> read = await ReadCipFromServerAsync(AllenBradleyHelper.PackRequestReadSegment(address, startIndex, length)).ConfigureAwait(continueOnCapturedContext: false);
				if (!read.IsSuccess)
				{
					return read;
				}
				analysis = AllenBradleyHelper.ExtractActualData(read.Content, isRead: true);
				if (!analysis.IsSuccess)
				{
					return OperateResult.CreateFailedResult<byte[]>(analysis);
				}
				startIndex += analysis.Content1.Length;
				if (bytesContent.Count != 0 && analysis.Content2 == 672 && analysis.Content1.Length >= 2 && analysis.Content1[0] == bytesContent[0] && analysis.Content1[1] == bytesContent[1])
				{
					bytesContent.AddRange(analysis.Content1.Skip(2));
				}
				else
				{
					bytesContent.AddRange(analysis.Content1);
				}
				if (analysis.Content2 == 672)
				{
					startIndex -= 2;
				}
			}
			while (analysis.Content3);
			return OperateResult.CreateSuccessResult(bytesContent.ToArray());
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			return new OperateResult<byte[]>("Address Wrong:" + ex2.Message);
		}
	}

	/// <summary>
	/// 使用CIP报文和服务器进行核心的数据交换
	/// </summary>
	/// <param name="cips">Cip commands</param>
	/// <returns>Results Bytes</returns>
	public async Task<OperateResult<byte[]>> ReadCipFromServerAsync(params byte[][] cips)
	{
		byte[] commandSpecificData = AllenBradleyHelper.PackCommandSpecificData(new byte[4], PackCommandService(PortSlot ?? new byte[2] { 1, Slot }, cips.ToArray()));
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(commandSpecificData).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		OperateResult check = CheckResponse(read.Content);
		if (!check.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(check);
		}
		return OperateResult.CreateSuccessResult(read.Content);
	}

	/// <summary>
	/// 使用EIP报文和服务器进行核心的数据交换
	/// </summary>
	/// <param name="eip">eip commands</param>
	/// <returns>Results Bytes</returns>
	public async Task<OperateResult<byte[]>> ReadEipFromServerAsync(params byte[][] eip)
	{
		byte[] commandSpecificData = AllenBradleyHelper.PackCommandSpecificData(eip);
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(commandSpecificData).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		OperateResult check = CheckResponse(read.Content);
		if (!check.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(check);
		}
		return OperateResult.CreateSuccessResult(read.Content);
	}

	/// <summary>
	/// 读取单个的bool数据信息，如果读取的是单bool变量，就直接写变量名，如果是由int组成的bool数组的一个值，一律带"i="开头访问，例如"i=A[0]"
	/// Read a single bool data information, if it is a single bool variable, write the variable name directly,
	/// if it is a value of a bool array composed of int, it is always accessed with "i=" at the beginning, for example, "i=A[0]"
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <returns>带有结果对象的结果数据 -&gt; Result data with result info</returns>
	public override async Task<OperateResult<bool>> ReadBoolAsync(string address)
	{
		if (address.StartsWith("i="))
		{
			return ByteTransformHelper.GetResultFromArray(await ReadBoolAsync(address, 1).ConfigureAwait(continueOnCapturedContext: false));
		}
		OperateResult<byte[]> read = await ReadAsync(address, 1).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool>(read);
		}
		return OperateResult.CreateSuccessResult(base.ByteTransform.TransBool(read.Content, 0));
	}

	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		if (address.StartsWith("i="))
		{
			address = address.Substring(2);
			address = AllenBradleyHelper.AnalysisArrayIndex(address, out var bitIndex);
			OperateResult<int> dataBitsResult = GetBoolValueTypeBitLength(address);
			if (!dataBitsResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(dataBitsResult);
			}
			int dataBits = dataBitsResult.Content;
			string uintIndex = ((bitIndex / dataBits == 0) ? "" : $"[{bitIndex / dataBits}]");
			OperateResult<byte[]> read = await ReadAsync(length: (ushort)HslHelper.CalculateOccupyLength(bitIndex, length, dataBits), address: address + uintIndex).ConfigureAwait(continueOnCapturedContext: false);
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(read);
			}
			return OperateResult.CreateSuccessResult(read.Content.ToBoolArray().SelectMiddle(bitIndex % dataBits, length));
		}
		OperateResult<byte[]> read2 = await ReadAsync(address, length).ConfigureAwait(continueOnCapturedContext: false);
		if (!read2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(read2);
		}
		return OperateResult.CreateSuccessResult(SoftBasic.ByteToBoolArray(read2.Content, length));
	}

	/// <summary>
	/// 批量读取的bool数组信息，如果你有个Bool数组变量名为 A, 那么读第0个位，可以通过 ReadBool("A")，但是第二个位需要使用
	/// ReadBoolArray("A[0]")   // 返回32个bool长度，0-31的索引，如果我想读取32-63的位索引，就需要 ReadBoolArray("A[1]") ，以此类推。
	/// For batch read bool array information, if you have a Bool array variable named A, then you can read the 0th bit through ReadBool("A"),
	/// but the second bit needs to use ReadBoolArray("A[0]" ) // Returns the length of 32 bools, the index is 0-31,
	/// if I want to read the bit index of 32-63, I need ReadBoolArray("A[1]"), and so on.
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <returns>带有结果对象的结果数据 -&gt; Result data with result info</returns>
	public async Task<OperateResult<bool[]>> ReadBoolArrayAsync(string address)
	{
		OperateResult<byte[]> read = await ReadAsync(address, 1).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(read);
		}
		return OperateResult.CreateSuccessResult(read.Content.ToBoolArray());
	}

	/// <summary>
	/// 读取PLC的byte类型的数据
	/// Read the byte type of PLC data
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <returns>带有结果对象的结果数据 -&gt; Result data with result info</returns>
	public async Task<OperateResult<byte>> ReadByteAsync(string address)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadAsync(address, 1));
	}

	/// <summary>
	/// 从PLC里读取一个指定标签名的原始数据信息及其数据类型信息
	/// Read the original data information of a specified tag name and its data type information from the PLC
	/// </summary>
	/// <param name="address">PLC的标签地址信息</param>
	/// <param name="length">读取的数据长度</param>
	/// <returns>包含原始数据信息及数据类型的结果对象</returns>
	/// <remarks>
	/// 数据类型的定义，可以参考  的常量资源信息。
	/// </remarks>
	public async Task<OperateResult<ushort, byte[]>> ReadTagAsync(string address, ushort length = 1)
	{
		HslHelper.ExtractParameter(ref address, "type");
		OperateResult<byte[], ushort, bool> read = await ReadWithTypeAsync(new string[1] { address }, new ushort[1] { length }).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<ushort, byte[]>(read);
		}
		return OperateResult.CreateSuccessResult(read.Content2, read.Content1);
	}

	/// <summary>
	/// 枚举当前的所有的变量名字，包含结构体信息，除去系统自带的名称数据信息
	/// Enumerate all the current variable names, including structure information, except the name data information that comes with the system
	/// </summary>
	/// <returns>结果对象</returns>
	public OperateResult<AbTagItem[]> TagEnumerator()
	{
		List<AbTagItem> list = new List<AbTagItem>();
		for (int i = 0; i < 2; i++)
		{
			uint startInstance = 0u;
			OperateResult<byte[], ushort, bool> operateResult2;
			do
			{
				OperateResult<byte[]> operateResult = ReadCipFromServer((i == 0) ? AllenBradleyHelper.BuildEnumeratorCommand(startInstance) : AllenBradleyHelper.BuildEnumeratorProgrameMainCommand(startInstance));
				if (!operateResult.IsSuccess)
				{
					return OperateResult.CreateFailedResult<AbTagItem[]>(operateResult);
				}
				operateResult2 = AllenBradleyHelper.ExtractActualData(operateResult.Content, isRead: true);
				if (!operateResult2.IsSuccess)
				{
					if (i == 1)
					{
						return OperateResult.CreateSuccessResult(list.ToArray());
					}
					return OperateResult.CreateFailedResult<AbTagItem[]>(operateResult2);
				}
				if (operateResult.Content.Length >= 43 && BitConverter.ToUInt16(operateResult.Content, 40) == 213)
				{
					list.AddRange(AbTagItem.PraseAbTagItems(operateResult.Content, 44, i == 0, out var instance));
					startInstance = instance + 1;
					continue;
				}
				return new OperateResult<AbTagItem[]>(StringResources.Language.UnknownError + " Source: " + operateResult.Content.ToHexString(' '));
			}
			while (operateResult2.Content3);
		}
		return OperateResult.CreateSuccessResult(list.ToArray());
	}

	/// <summary>
	/// 枚举当前的所有的变量名字，包含结构体信息，除去系统自带的名称数据信息
	/// Enumerate all the current variable names, including structure information, except the name data information that comes with the system
	/// </summary>
	/// <returns>结果对象</returns>
	public async Task<OperateResult<AbTagItem[]>> TagEnumeratorAsync()
	{
		List<AbTagItem> lists = new List<AbTagItem>();
		for (int i = 0; i < 2; i++)
		{
			uint instanceAddress = 0u;
			OperateResult<byte[], ushort, bool> analysis;
			do
			{
				OperateResult<byte[]> readCip = await ReadCipFromServerAsync((i == 0) ? AllenBradleyHelper.BuildEnumeratorCommand(instanceAddress) : AllenBradleyHelper.BuildEnumeratorProgrameMainCommand(instanceAddress));
				if (!readCip.IsSuccess)
				{
					return OperateResult.CreateFailedResult<AbTagItem[]>(readCip);
				}
				analysis = AllenBradleyHelper.ExtractActualData(readCip.Content, isRead: true);
				if (!analysis.IsSuccess)
				{
					if (i == 1)
					{
						return OperateResult.CreateSuccessResult(lists.ToArray());
					}
					return OperateResult.CreateFailedResult<AbTagItem[]>(analysis);
				}
				if (readCip.Content.Length >= 43 && BitConverter.ToUInt16(readCip.Content, 40) == 213)
				{
					lists.AddRange(AbTagItem.PraseAbTagItems(readCip.Content, 44, i == 0, out var instance));
					instanceAddress = instance + 1;
					continue;
				}
				return new OperateResult<AbTagItem[]>(StringResources.Language.UnknownError + " Source: " + readCip.Content.ToHexString(' '));
			}
			while (analysis.Content3);
		}
		return OperateResult.CreateSuccessResult(lists.ToArray());
	}

	private OperateResult<AbStructHandle> ReadTagStructHandle(AbTagItem structTag)
	{
		OperateResult<byte[]> operateResult = ReadCipFromServer(AllenBradleyHelper.GetStructHandleCommand(structTag.SymbolType));
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<AbStructHandle>(operateResult);
		}
		if (operateResult.Content.Length >= 43 && BitConverter.ToInt32(operateResult.Content, 40) == 131)
		{
			return OperateResult.CreateSuccessResult(new AbStructHandle(operateResult.Content, 44));
		}
		return new OperateResult<AbStructHandle>(StringResources.Language.UnknownError + " Source Data: " + operateResult.Content.ToHexString(' '));
	}

	/// <summary>
	/// 枚举结构体的方法，传入结构体的标签对象，返回结构体子属性标签列表信息，子属性有可能是标量数据，也可能是另一个结构体。
	/// The method of enumerating the structure, passing in the tag object of the structure,
	/// and returning the tag list information of the sub-attributes of the structure. The sub-attributes may be scalar data or another structure.
	/// </summary>
	/// <param name="structTag">结构体的标签</param>
	/// <returns>是否成功</returns>
	public OperateResult<AbTagItem[]> StructTagEnumerator(AbTagItem structTag)
	{
		OperateResult<AbStructHandle> operateResult = ReadTagStructHandle(structTag);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<AbTagItem[]>(operateResult);
		}
		OperateResult<byte[]> operateResult2 = ReadCipFromServer(AllenBradleyHelper.GetStructItemNameType(structTag.SymbolType, operateResult.Content, 0));
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<AbTagItem[]>(operateResult2);
		}
		if (operateResult2.Content.Length >= 43 && operateResult2.Content[40] == 204 && operateResult2.Content[41] == 0 && operateResult2.Content[42] == 0)
		{
			return OperateResult.CreateSuccessResult(AbTagItem.PraseAbTagItemsFromStruct(operateResult2.Content, 44, operateResult.Content).ToArray());
		}
		return new OperateResult<AbTagItem[]>(StringResources.Language.UnknownError + " Status:" + operateResult2.Content[42]);
	}

	[HslMqttApi("ReadInt16Array", "")]
	public override OperateResult<short[]> ReadInt16(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, length), (byte[] m) => base.ByteTransform.TransInt16(m, 0, length));
	}

	[HslMqttApi("ReadUInt16Array", "")]
	public override OperateResult<ushort[]> ReadUInt16(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, length), (byte[] m) => base.ByteTransform.TransUInt16(m, 0, length));
	}

	[HslMqttApi("ReadInt32Array", "")]
	public override OperateResult<int[]> ReadInt32(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, length), (byte[] m) => base.ByteTransform.TransInt32(m, 0, length));
	}

	[HslMqttApi("ReadUInt32Array", "")]
	public override OperateResult<uint[]> ReadUInt32(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, length), (byte[] m) => base.ByteTransform.TransUInt32(m, 0, length));
	}

	[HslMqttApi("ReadFloatArray", "")]
	public override OperateResult<float[]> ReadFloat(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, length), (byte[] m) => base.ByteTransform.TransSingle(m, 0, length));
	}

	[HslMqttApi("ReadInt64Array", "")]
	public override OperateResult<long[]> ReadInt64(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, length), (byte[] m) => base.ByteTransform.TransInt64(m, 0, length));
	}

	[HslMqttApi("ReadUInt64Array", "")]
	public override OperateResult<ulong[]> ReadUInt64(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, length), (byte[] m) => base.ByteTransform.TransUInt64(m, 0, length));
	}

	[HslMqttApi("ReadDoubleArray", "")]
	public override OperateResult<double[]> ReadDouble(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(Read(address, length), (byte[] m) => base.ByteTransform.TransDouble(m, 0, length));
	}

	public OperateResult<string> ReadString(string address)
	{
		return ReadString(address, 1);
	}

	[HslMqttApi("ReadString", "")]
	/// <summary>
	/// 读取字符串数据，默认为编码
	/// Read string data, default is the  encoding
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的string数据</returns>
	public override OperateResult<string> ReadString(string address, ushort length)
	{
		return ReadString(address, length, Encoding.UTF8);
	}

	public override OperateResult<string> ReadString(string address, ushort length, Encoding encoding)
	{
		HslHelper.ExtractParameter(ref address, "type", 0);
		OperateResult<byte[], ushort, bool> read = ReadWithType(new string[1] { address }, new ushort[1] { length });
		return AllenBradleyHelper.ExtractActualString(read, base.ByteTransform, encoding);
	}

	[HslMqttApi(Description = "获取PLC的型号信息")]
	/// <summary>
	/// 从PLC里读取当前PLC的型号信息
	/// Read the current PLC model information from the PLC
	/// </summary>
	/// <param name="plc">PLC对象</param>
	/// <returns>型号数据信息</returns>
	public OperateResult<string> ReadPlcType()
	{
		return AllenBradleyHelper.ReadPlcType(this);
	}

	public override async Task<OperateResult<short[]>> ReadInt16Async(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, length), (byte[] m) => base.ByteTransform.TransInt16(m, 0, length));
	}

	public override async Task<OperateResult<ushort[]>> ReadUInt16Async(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, length), (byte[] m) => base.ByteTransform.TransUInt16(m, 0, length));
	}

	public override async Task<OperateResult<int[]>> ReadInt32Async(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, length), (byte[] m) => base.ByteTransform.TransInt32(m, 0, length));
	}

	public override async Task<OperateResult<uint[]>> ReadUInt32Async(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, length), (byte[] m) => base.ByteTransform.TransUInt32(m, 0, length));
	}

	public override async Task<OperateResult<float[]>> ReadFloatAsync(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, length), (byte[] m) => base.ByteTransform.TransSingle(m, 0, length));
	}

	public override async Task<OperateResult<long[]>> ReadInt64Async(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, length), (byte[] m) => base.ByteTransform.TransInt64(m, 0, length));
	}

	public override async Task<OperateResult<ulong[]>> ReadUInt64Async(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, length), (byte[] m) => base.ByteTransform.TransUInt64(m, 0, length));
	}

	public override async Task<OperateResult<double[]>> ReadDoubleAsync(string address, ushort length)
	{
		return ByteTransformHelper.GetResultFromBytes(await ReadAsync(address, length), (byte[] m) => base.ByteTransform.TransDouble(m, 0, length));
	}

	public async Task<OperateResult<string>> ReadStringAsync(string address)
	{
		return await ReadStringAsync(address, 1);
	}

	/// <summary>
	/// 读取字符串数据，默认为编码
	/// Read string data, default is the  encoding
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的string数据</returns>
	public override async Task<OperateResult<string>> ReadStringAsync(string address, ushort length)
	{
		return await ReadStringAsync(address, length, Encoding.UTF8);
	}

	public override async Task<OperateResult<string>> ReadStringAsync(string address, ushort length, Encoding encoding)
	{
		HslHelper.ExtractParameter(ref address, "type", 0);
		return AllenBradleyHelper.ExtractActualString(await ReadWithTypeAsync(new string[1] { address }, new ushort[1] { length }), base.ByteTransform, encoding);
	}

	/// <summary>
	/// 从PLC里读取当前PLC的型号信息
	/// Read the current PLC model information from the PLC
	/// </summary>
	/// <param name="plc">PLC对象</param>
	/// <returns>型号数据信息</returns>
	public async Task<OperateResult<string>> ReadPlcTypeAsync()
	{
		return await AllenBradleyHelper.ReadPlcTypeAsync(this);
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 当前写入字节数组使用数据类型 0xD1 写入，如果其他的字节类型需要调用  方法来实现。
	/// The currently written byte array is written using the data type 0xD1. If other byte types need to be called  Method to achieve.
	/// </summary>
	/// <param name="address">地址</param>
	/// <param name="value">值</param>
	/// <returns>写入结果值</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		return WriteTag(address, 209, value, (!HslHelper.IsAddressEndWithIndex(address)) ? 1 : value.Length);
	}

	/// <summary>
	/// 使用指定的类型写入指定的节点数据，类型信息参考API文档，地址支持协议类型代号信息，例如 "type=0xD1;A"
	/// Use the specified type to write the specified node data. For type information, refer to the API documentation. The address supports protocol type code information, such as "type=0xD1;A"
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <param name="typeCode">类型代码，详细参见上的常用字段 -&gt;  Type code, see the commonly used Fields section on the  in detail</param>
	/// <param name="value">实际的数据值 -&gt; The actual data value</param>
	/// <param name="length">如果节点是数组，就是数组长度 -&gt; If the node is an array, it is the array length</param>
	/// <returns>是否写入成功 -&gt; Whether to write successfully</returns>
	/// <remarks>
	/// 关于参数 length 的含义，表示的是地址长度，一般的标量数据都是 1，如果PLC有个标签是 A，数据类型为 byte[10]，那我们写入 3 个byte就是 WriteTag( "A[5]", 0xD1, new byte[]{1,2,3}, 3 );
	/// Regarding the meaning of the parameter length, it represents the address length. The general scalar data is 1. If the PLC has a tag of A and the data type is byte[10], then we write 3 bytes as WriteTag( "A[5 ]", 0xD1, new byte[]{1,2,3}, 3 );
	/// </remarks>
	public virtual OperateResult WriteTag(string address, ushort typeCode, byte[] value, int length = 1)
	{
		OperateResult<int> operateResult = HslHelper.ExtractParameterWithDefault(ref address, "type", typeCode);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		typeCode = (ushort)operateResult.Content;
		OperateResult<List<byte[]>> operateResult2 = BuildWriteCommand(address, typeCode, value, length);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		for (int i = 0; i < operateResult2.Content.Count; i++)
		{
			OperateResult<byte[]> operateResult3 = ReadFromCoreServer(operateResult2.Content[i]);
			if (!operateResult3.IsSuccess)
			{
				return operateResult3;
			}
			OperateResult operateResult4 = CheckResponse(operateResult3.Content);
			if (!operateResult4.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult4);
			}
			OperateResult operateResult5 = AllenBradleyHelper.ExtractActualData(operateResult3.Content, isRead: false);
			if (!operateResult5.IsSuccess)
			{
				return operateResult5;
			}
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 当前写入字节数组使用数据类型 0xD1 写入，如果其他的字节类型需要调用  方法来实现。
	/// The currently written byte array is written using the data type 0xD1. If other byte types need to be called  Method to achieve.
	/// </summary>
	/// <param name="address">地址</param>
	/// <param name="value">值</param>
	/// <returns>写入结果值</returns>
	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await WriteTagAsync(address, 209, value, (!HslHelper.IsAddressEndWithIndex(address)) ? 1 : value.Length);
	}

	/// <summary>
	/// 使用指定的类型写入指定的节点数据，类型信息参考API文档，地址支持协议类型代号信息，例如 "type=0xD1;A"
	/// Use the specified type to write the specified node data. For type information, refer to the API documentation. The address supports protocol type code information, such as "type=0xD1;A"
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Name of the node</param>
	/// <param name="typeCode">类型代码，详细参见上的常用字段 -&gt;  Type code, see the commonly used Fields section on the  in detail</param>
	/// <param name="value">实际的数据值 -&gt; The actual data value</param>
	/// <param name="length">如果节点是数组，就是数组长度 -&gt; If the node is an array, it is the array length</param>
	/// <returns>是否写入成功 -&gt; Whether to write successfully</returns>
	/// <remarks>
	/// 关于参数 length 的含义，表示的是地址长度，一般的标量数据都是 1，如果PLC有个标签是 A，数据类型为 byte[10]，那我们写入 3 个byte就是 WriteTag( "A[5]", 0xD1, new byte[]{1,2,3}, 3 );
	/// Regarding the meaning of the parameter length, it represents the address length. The general scalar data is 1. If the PLC has a tag of A and the data type is byte[10], then we write 3 bytes as WriteTag( "A[5 ]", 0xD1, new byte[]{1,2,3}, 3 );
	/// </remarks>
	public virtual async Task<OperateResult> WriteTagAsync(string address, ushort typeCode, byte[] value, int length = 1)
	{
		OperateResult<int> extraType = HslHelper.ExtractParameterWithDefault(ref address, "type", typeCode);
		if (!extraType.IsSuccess)
		{
			return extraType;
		}
		typeCode = (ushort)extraType.Content;
		OperateResult<List<byte[]>> command = BuildWriteCommand(address, typeCode, value, length);
		if (!command.IsSuccess)
		{
			return command;
		}
		for (int i = 0; i < command.Content.Count; i++)
		{
			OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content[i]).ConfigureAwait(continueOnCapturedContext: false);
			if (!read.IsSuccess)
			{
				return read;
			}
			OperateResult check = CheckResponse(read.Content);
			if (!check.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(check);
			}
			OperateResult extra = AllenBradleyHelper.ExtractActualData(read.Content, isRead: false);
			if (!extra.IsSuccess)
			{
				return extra;
			}
		}
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi("WriteInt16Array", "")]
	public override OperateResult Write(string address, short[] values)
	{
		return WriteTag(address, 195, base.ByteTransform.TransByte(values), GetWriteValueLength(address, values.Length));
	}

	[HslMqttApi("WriteUInt16Array", "")]
	public override OperateResult Write(string address, ushort[] values)
	{
		return WriteTag(address, 199, base.ByteTransform.TransByte(values), GetWriteValueLength(address, values.Length));
	}

	[HslMqttApi("WriteInt32Array", "")]
	public override OperateResult Write(string address, int[] values)
	{
		return WriteTag(address, 196, base.ByteTransform.TransByte(values), GetWriteValueLength(address, values.Length));
	}

	[HslMqttApi("WriteUInt32Array", "")]
	public override OperateResult Write(string address, uint[] values)
	{
		return WriteTag(address, 200, base.ByteTransform.TransByte(values), GetWriteValueLength(address, values.Length));
	}

	[HslMqttApi("WriteFloatArray", "")]
	public override OperateResult Write(string address, float[] values)
	{
		return WriteTag(address, 202, base.ByteTransform.TransByte(values), GetWriteValueLength(address, values.Length));
	}

	[HslMqttApi("WriteInt64Array", "")]
	public override OperateResult Write(string address, long[] values)
	{
		return WriteTag(address, 197, base.ByteTransform.TransByte(values), GetWriteValueLength(address, values.Length));
	}

	[HslMqttApi("WriteUInt64Array", "")]
	public override OperateResult Write(string address, ulong[] values)
	{
		return WriteTag(address, 201, base.ByteTransform.TransByte(values), GetWriteValueLength(address, values.Length));
	}

	[HslMqttApi("WriteDoubleArray", "")]
	public override OperateResult Write(string address, double[] values)
	{
		return WriteTag(address, 203, base.ByteTransform.TransByte(values), GetWriteValueLength(address, values.Length));
	}

	public override OperateResult Write(string address, string value, Encoding encoding)
	{
		if (string.IsNullOrEmpty(value))
		{
			value = string.Empty;
		}
		ushort num = (ushort)HslHelper.ExtractParameter(ref address, "type", 194);
		if (num == 218)
		{
			byte[] bytes = encoding.GetBytes(value);
			return WriteTag(address, num, SoftBasic.SpliceArray<byte>(new byte[1] { (byte)bytes.Length }, bytes));
		}
		byte[] bytes2 = encoding.GetBytes(value);
		OperateResult operateResult = Write(address + ".LEN", bytes2.Length);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		if (bytes2 != null && bytes2.Length != 0)
		{
			byte[] value2 = SoftBasic.ArrayExpandToLengthEven(bytes2);
			return WriteTag(address + ".DATA[0]", num, value2, bytes2.Length);
		}
		return operateResult;
	}

	[HslMqttApi("WriteBool", "")]
	/// <summary>
	/// 写入单个Bool的数据信息。如果读取的是单bool变量，就直接写变量名，如果是bool数组的一个值，一律带下标访问，例如a[0]
	/// Write the data information of a single Bool. If the read is a single bool variable, write the variable name directly,
	/// if it is a value of the bool array, it will always be accessed with a subscript, such as a[0]
	/// </summary>
	/// <param name="address">标签的地址数据</param>
	/// <param name="value">bool数据值</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// 如果写入的是类型代号 0xC1 的bool变量或是数组，直接使用标签名即可，比如：A,A[10]，如果写入的是类型代号0xD3的bool数组的值，则需要使用地址"i="开头，例如：i=A[10]
	/// If you write a bool variable or array of type code 0xC1, you can use the tag name directly, such as: A,A[10],
	/// if you write the value of a bool array of type code 0xD3, you need to use the address" i=" at the beginning, for example: i=A[10]
	/// </remarks>
	public override OperateResult Write(string address, bool value)
	{
		if (address.StartsWith("i=") && (Regex.IsMatch(address, "\\[[0-9]+\\]$") || Regex.IsMatch(address, "\\.[0-9]+$")))
		{
			OperateResult<byte[]> operateResult = BuildWriteCommand(address.Substring(2), value);
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
			OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
			if (!operateResult2.IsSuccess)
			{
				return operateResult2;
			}
			OperateResult operateResult3 = CheckResponse(operateResult2.Content);
			if (!operateResult3.IsSuccess)
			{
				ClearBoolTypeCache(operateResult3, address.Substring(2));
				return OperateResult.CreateFailedResult<byte[]>(operateResult3);
			}
			return AllenBradleyHelper.ExtractActualData(operateResult2.Content, isRead: false);
		}
		return WriteTag(address, 193, (!value) ? new byte[2] : new byte[2] { 255, 255 });
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// 批量写入数组数据，返回是否成功
	/// Batch write  array data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, bool[] value)
	{
		return WriteTag(address, 193, value.Select((bool m) => (byte)(m ? 1 : 0)).ToArray(), (!HslHelper.IsAddressEndWithIndex(address)) ? 1 : value.Length);
	}

	[HslMqttApi("WriteByte", "")]
	/// <summary>
	/// 写入Byte数据，返回是否写入成功，默认使用类型 0xC2, 如果PLC的变量类型不一样，则需要指定实际的变量类型，例如PLC的变量 A 是0xD1类型，那么地址需要携带类型信息，type=0xD1;A
	/// Write Byte data and return whether the writing is successful. The default type is 0xC2. If the variable types of the PLC are different, you need to specify the actual variable type.
	/// For example, the variable A of the PLC is of type 0xD1, then the address needs to carry the type information, type= 0xD1;A
	/// </summary>
	/// <param name="address">标签的地址数据</param>
	/// <param name="value">Byte数据</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// 如何确认PLC的变量的类型呢？可以在HslCommunicationDemo程序上测试知道，也可以直接调用  来知道类型信息。
	/// </remarks>
	public virtual OperateResult Write(string address, byte value)
	{
		return WriteTag(address, 194, new byte[2] { value, 0 });
	}

	public override async Task<OperateResult> WriteAsync(string address, short[] values)
	{
		return await WriteTagAsync(address, 195, base.ByteTransform.TransByte(values), GetWriteValueLength(address, values.Length)).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, ushort[] values)
	{
		return await WriteTagAsync(address, 199, base.ByteTransform.TransByte(values), GetWriteValueLength(address, values.Length)).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, int[] values)
	{
		return await WriteTagAsync(address, 196, base.ByteTransform.TransByte(values), GetWriteValueLength(address, values.Length)).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, uint[] values)
	{
		return await WriteTagAsync(address, 200, base.ByteTransform.TransByte(values), GetWriteValueLength(address, values.Length)).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, float[] values)
	{
		return await WriteTagAsync(address, 202, base.ByteTransform.TransByte(values), GetWriteValueLength(address, values.Length)).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, long[] values)
	{
		return await WriteTagAsync(address, 197, base.ByteTransform.TransByte(values), GetWriteValueLength(address, values.Length)).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, ulong[] values)
	{
		return await WriteTagAsync(address, 201, base.ByteTransform.TransByte(values), GetWriteValueLength(address, values.Length)).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, double[] values)
	{
		return await WriteTagAsync(address, 203, base.ByteTransform.TransByte(values), GetWriteValueLength(address, values.Length)).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, string value, Encoding encoding)
	{
		if (string.IsNullOrEmpty(value))
		{
			value = string.Empty;
		}
		ushort typeCode = (ushort)HslHelper.ExtractParameter(ref address, "type", 194);
		if (typeCode == 218)
		{
			byte[] bytes = encoding.GetBytes(value);
			return WriteTag(address, typeCode, SoftBasic.SpliceArray<byte>(new byte[1] { (byte)bytes.Length }, bytes));
		}
		byte[] bytes2 = encoding.GetBytes(value);
		OperateResult write = await WriteAsync(address + ".LEN", bytes2.Length).ConfigureAwait(continueOnCapturedContext: false);
		if (!write.IsSuccess)
		{
			return write;
		}
		if (bytes2 != null && bytes2.Length != 0)
		{
			return await WriteTagAsync(value: SoftBasic.ArrayExpandToLengthEven(bytes2), address: address + ".DATA[0]", typeCode: typeCode, length: bytes2.Length).ConfigureAwait(continueOnCapturedContext: false);
		}
		return write;
	}

	/// <summary>
	/// 写入单个Bool的数据信息。如果读取的是单bool变量，就直接写变量名，如果是bool数组的一个值，一律带下标访问，例如a[0]
	/// Write the data information of a single Bool. If the read is a single bool variable, write the variable name directly,
	/// if it is a value of the bool array, it will always be accessed with a subscript, such as a[0]
	/// </summary>
	/// <param name="address">标签的地址数据</param>
	/// <param name="value">bool数据值</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// 如果写入的是类型代号 0xC1 的bool变量或是数组，直接使用标签名即可，比如：A,A[10]，如果写入的是类型代号0xD3的bool数组的值，则需要使用地址"i="开头，例如：i=A[10]
	/// If you write a bool variable or array of type code 0xC1, you can use the tag name directly, such as: A,A[10],
	/// if you write the value of a bool array of type code 0xD3, you need to use the address" i=" at the beginning, for example: i=A[10]
	/// </remarks>
	public override async Task<OperateResult> WriteAsync(string address, bool value)
	{
		if (address.StartsWith("i=") && (Regex.IsMatch(address, "\\[[0-9]+\\]$") || Regex.IsMatch(address, "\\.[0-9]+$")))
		{
			OperateResult<byte[]> command = BuildWriteCommand(address.Substring(2), value);
			if (!command.IsSuccess)
			{
				return command;
			}
			OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
			if (!read.IsSuccess)
			{
				return read;
			}
			OperateResult check = CheckResponse(read.Content);
			if (!check.IsSuccess)
			{
				ClearBoolTypeCache(check, address.Substring(2));
				return OperateResult.CreateFailedResult<byte[]>(check);
			}
			return AllenBradleyHelper.ExtractActualData(read.Content, isRead: false);
		}
		return await WriteTagAsync(address, 193, (!value) ? new byte[2] : new byte[2] { 255, 255 }).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 批量写入数组数据，返回是否成功
	/// Batch write  array data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public override async Task<OperateResult> WriteAsync(string address, bool[] value)
	{
		return await WriteTagAsync(address, 193, value.Select((bool m) => (byte)(m ? 1 : 0)).ToArray(), (!HslHelper.IsAddressEndWithIndex(address)) ? 1 : value.Length).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 写入Byte数据，返回是否写入成功，默认使用类型 0xC2, 如果PLC的变量类型不一样，则需要指定实际的变量类型，例如PLC的变量 A 是0xD1类型，那么地址需要携带类型信息，type=0xD1;A
	/// Write Byte data and return whether the writing is successful. The default type is 0xC2. If the variable types of the PLC are different, you need to specify the actual variable type.
	/// For example, the variable A of the PLC is of type 0xD1, then the address needs to carry the type information, type= 0xD1;A
	/// </summary>
	/// <param name="address">标签的地址数据</param>
	/// <param name="value">Byte数据</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// 如何确认PLC的变量的类型呢？可以在HslCommunicationDemo程序上测试知道，也可以直接调用  来知道类型信息。
	/// </remarks>
	public virtual async Task<OperateResult> WriteAsync(string address, byte value)
	{
		return await WriteTagAsync(address, 194, new byte[2] { value, 0 }).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 读取指定地址的日期数据，最小日期为 1970年1月1日，当PLC的变量类型为 "Date" 和 "TimeAndDate" 时，都可以用本方法读取。
	/// Read the date data of the specified address. The minimum date is January 1, 1970. When the PLC variable type is "Date" and "TimeAndDate", this method can be used to read.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <returns>日期结果对象</returns>
	public OperateResult<DateTime> ReadDate(string address)
	{
		return AllenBradleyHelper.ReadDate(this, address);
	}

	/// <summary>
	/// 使用日期格式（Date）将指定的数据写入到指定的地址里，PLC的地址类型变量必须为 "Date"，否则写入失败。
	/// Use the date format (Date) to write the specified data to the specified address. The PLC address type variable must be "Date", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="date">时间信息</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteDate(string address, DateTime date)
	{
		return AllenBradleyHelper.WriteDate(this, address, date);
	}

	/// <summary>
	/// 使用日期格式（Date）将指定的数据写入到指定的地址里，PLC的地址类型变量必须为 "Date"，否则写入失败。
	/// Use the date format (Date) to write the specified data to the specified address. The PLC address type variable must be "Date", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="date">时间信息</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteTimeAndDate(string address, DateTime date)
	{
		return AllenBradleyHelper.WriteTimeAndDate(this, address, date);
	}

	/// <summary>
	/// 读取指定地址的时间数据，最小时间为 0，如果获取秒，可以访问 ，当PLC的变量类型为 "Time" 和 "TimeOfDate" 时，都可以用本方法读取。
	/// Read the time data of the specified address. The minimum time is 0. If you get seconds, you can access .
	/// When the PLC variable type is "Time" and "TimeOfDate", you can use this Method to read.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <returns>时间的结果对象</returns>
	public OperateResult<TimeSpan> ReadTime(string address)
	{
		return AllenBradleyHelper.ReadTime(this, address);
	}

	/// <summary>
	/// 使用时间格式（TIME）将时间数据写入到PLC中指定的地址里去，PLC的地址类型变量必须为 "TIME"，否则写入失败。
	/// Use the time format (TIME) to write the time data to the address specified in the PLC. The PLC address type variable must be "TIME", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="time">时间参数变量</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteTime(string address, TimeSpan time)
	{
		return AllenBradleyHelper.WriteTime(this, address, time);
	}

	/// <summary>
	/// 使用时间格式（TimeOfDate）将时间数据写入到PLC中指定的地址里去，PLC的地址类型变量必须为 "TimeOfDate"，否则写入失败。
	/// Use the time format (TimeOfDate) to write the time data to the address specified in the PLC. The PLC address type variable must be "TimeOfDate", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="timeOfDate">时间参数变量</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteTimeOfDate(string address, TimeSpan timeOfDate)
	{
		return AllenBradleyHelper.WriteTimeOfDate(this, address, timeOfDate);
	}

	/// <summary>
	/// 读取指定地址的日期数据，最小日期为 1970年1月1日，当PLC的变量类型为 "Date" 和 "TimeAndDate" 时，都可以用本方法读取。
	/// Read the date data of the specified address. The minimum date is January 1, 1970. When the PLC variable type is "Date" and "TimeAndDate", this method can be used to read.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <returns>日期结果对象</returns>
	public async Task<OperateResult<DateTime>> ReadDateAsync(string address)
	{
		return await AllenBradleyHelper.ReadDateAsync(this, address).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 使用日期格式（Date）将指定的数据写入到指定的地址里，PLC的地址类型变量必须为 "Date"，否则写入失败。
	/// Use the date format (Date) to write the specified data to the specified address. The PLC address type variable must be "Date", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="date">时间信息</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteDateAsync(string address, DateTime date)
	{
		return await AllenBradleyHelper.WriteDateAsync(this, address, date).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 使用日期格式（Date）将指定的数据写入到指定的地址里，PLC的地址类型变量必须为 "Date"，否则写入失败。
	/// Use the date format (Date) to write the specified data to the specified address. The PLC address type variable must be "Date", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="date">时间信息</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteTimeAndDateAsync(string address, DateTime date)
	{
		return await AllenBradleyHelper.WriteTimeAndDateAsync(this, address, date).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 读取指定地址的时间数据，最小时间为 0，如果获取秒，可以访问 ，当PLC的变量类型为 "Time" 和 "TimeOfDate" 时，都可以用本方法读取。
	/// Read the time data of the specified address. The minimum time is 0. If you get seconds, you can access .
	/// When the PLC variable type is "Time" and "TimeOfDate", you can use this Method to read.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <returns>时间的结果对象</returns>
	public async Task<OperateResult<TimeSpan>> ReadTimeAsync(string address)
	{
		return await AllenBradleyHelper.ReadTimeAsync(this, address).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 使用时间格式（TIME）将时间数据写入到PLC中指定的地址里去，PLC的地址类型变量必须为 "TIME"，否则写入失败。
	/// Use the time format (TIME) to write the time data to the address specified in the PLC. The PLC address type variable must be "TIME", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="time">时间参数变量</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteTimeAsync(string address, TimeSpan time)
	{
		return await AllenBradleyHelper.WriteTimeAsync(this, address, time).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 使用时间格式（TimeOfDate）将时间数据写入到PLC中指定的地址里去，PLC的地址类型变量必须为 "TimeOfDate"，否则写入失败。
	/// Use the time format (TimeOfDate) to write the time data to the address specified in the PLC. The PLC address type variable must be "TimeOfDate", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="timeOfDate">时间参数变量</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteTimeOfDateAsync(string address, TimeSpan timeOfDate)
	{
		return await AllenBradleyHelper.WriteTimeOfDateAsync(this, address, timeOfDate).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 将所有的cip指定进行打包操作。
	/// </summary>
	/// <param name="portSlot">PLC所在的面板槽号</param>
	/// <param name="cips">所有的cip打包指令信息</param>
	/// <returns>包含服务的信息</returns>
	protected virtual byte[] PackCommandService(byte[] portSlot, params byte[][] cips)
	{
		if (MessageRouter != null)
		{
			portSlot = MessageRouter.GetRouter();
		}
		return AllenBradleyHelper.PackCommandService(portSlot, cips);
	}

	/// <summary>
	/// 获取写入数据的长度信息，此处直接返回数组的长度信息
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <param name="length">数组长度信息</param>
	/// <returns>实际的写入长度信息</returns>
	protected virtual int GetWriteValueLength(string address, int length)
	{
		return length;
	}

	private OperateResult<int> GetBoolValueTypeBitLength(string address)
	{
		lock (boolLock)
		{
			if (boolValueDataBits.ContainsKey(address))
			{
				return OperateResult.CreateSuccessResult(boolValueDataBits[address]);
			}
		}
		OperateResult<AbTagValue[]> operateResult = ReadTags(new string[1] { address }, new ushort[1] { 1 });
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<int>(operateResult);
		}
		ushort type = operateResult.Content[0].Type;
		lock (boolLock)
		{
			if (boolValueDataBits.ContainsKey(address))
			{
				return OperateResult.CreateSuccessResult(boolValueDataBits[address]);
			}
			int value = 32;
			if (type == 194 || type == 209 || type == 198)
			{
				value = 8;
			}
			else if (type == 195 || type == 199 || type == 210)
			{
				value = 16;
			}
			else if (type == 196 || type == 200 || type == 211)
			{
				value = 32;
			}
			else if (type == 197 || type == 201 || type == 212)
			{
				value = 64;
			}
			boolValueDataBits.Add(address, value);
			return OperateResult.CreateSuccessResult(value);
		}
	}

	private void ClearBoolTypeCache()
	{
		lock (boolLock)
		{
			boolValueDataBits.Clear();
		}
	}

	private void ClearBoolTypeCache(OperateResult check, string address)
	{
		if (check.IsSuccess || check.ErrorCode == 10000)
		{
			return;
		}
		lock (boolLock)
		{
			address = AllenBradleyHelper.AnalysisArrayIndex(address, out var _);
			if (boolValueDataBits.ContainsKey(address))
			{
				boolValueDataBits.Remove(address);
			}
		}
	}

	public override string ToString()
	{
		return $"AllenBradleyNet[{IpAddress}:{Port}]";
	}
}
