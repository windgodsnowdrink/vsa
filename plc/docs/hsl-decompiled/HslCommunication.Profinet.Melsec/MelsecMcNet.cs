// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Profinet.Melsec.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 三菱PLC通讯类，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为二进制通讯
/// Mitsubishi PLC communication class is implemented using Qna compatible 3E frame protocol.
/// The Ethernet module on the PLC side needs to be configured first. It must be binary communication.
/// </summary>
/// <remarks>
/// 支持读写的数据类型详细参考API文档，支持高级的数据读取，例如读取智能模块，缓冲存储器等等。如果使用的Fx5u的PLC，如果之前有写过用户认证，需要对设备信息全部初始化
/// </remarks>
public class MelsecMcNet : DeviceTcpNet, IReadWriteMc, IReadWriteDevice, IReadWriteNet
{
	public virtual McType McType => McType.McBinary;

	/// <summary>
	/// 网络号，通常为0
	/// Network number, usually 0
	/// </summary>
	/// <remarks>
	/// 依据PLC的配置而配置，如果PLC配置了1，那么此处也填0，如果PLC配置了2，此处就填2，测试不通的话，继续测试0
	/// </remarks>
	public byte NetworkNumber { get; set; } = 0;

	/// <summary>
	/// PLC编号，如果是本站信息，则是 0xFF 值，其他站则根据实际的情况指定。
	/// The PLC number, if it is the information of this station, is the 0xFF value, and other stations are specified according to the actual situation.
	/// </summary>
	public byte PLCNumber { get; set; } = byte.MaxValue;

	/// <summary>
	/// 网络站号，通常为0
	/// Network station number, usually 0
	/// </summary>
	/// <remarks>
	/// 依据PLC的配置而配置，如果PLC配置了1，那么此处也填0，如果PLC配置了2，此处就填2，测试不通的话，继续测试0
	/// </remarks>
	public byte NetworkStationNumber { get; set; } = 0;

	/// <summary>
	/// 是否开启支持写入位到字寄存器的功能，该功能先读取字寄存器的字数据，然后修改其中的位，再写入回去，可能存在脏数据的风险
	/// Whether to enable supporting the function of writing bit-to-word registers, which first reads the word data of the word register, then modifies the bits in it, and then writes back, there may be a risk of dirty data
	/// </summary>
	/// <remarks>
	/// 关于脏数据风险：从读取数据，修改位，再次写入数据时，大概需要经过3ms~10ms不等的时间，如果此期间内PLC修改了该字寄存器的其他位，再次写入数据时会恢复该点位的数据到读取时的初始值，可能引发设备故障，请谨慎开启此功能。
	/// About dirty data risk: from reading data, modifying bits, writing data again, it takes about 3ms ~ 10ms to pass a time, if the PLC modifies other bits of the word register during this period, when writing data again,
	/// it will restore the data at the point to the initial value when reading, which may cause equipment failure, please turn on this function carefully.
	/// </remarks>
	public bool EnableWriteBitToWordRegister { get; set; } = true;

	/// <summary>
	/// 请求目标模块的IO编号，默认是管理CPU，也就是 0x03FF 的值，如果需要访问其他的非管理CPU的时候，请参考手册进行配置相关的值
	/// The IO number of the target module is the default management CPU, that is, the value of 0x03FF, if you need to access other non-management CPUs, please refer to the manual to configure the relevant values
	/// </summary>
	public ushort TargetIOStation { get; set; } = 1023;

	/// <summary>
	/// 实例化三菱的Qna兼容3E帧协议的通讯对象
	/// Instantiate the communication object of Mitsubishi's Qna compatible 3E frame protocol
	/// </summary>
	public MelsecMcNet()
	{
		base.WordLength = 1;
		base.ByteTransform = new RegularByteTransform();
	}

	/// <summary>
	/// 指定ip地址和端口号来实例化一个默认的对象
	/// Specify the IP address and port number to instantiate a default object
	/// </summary>
	/// <param name="ipAddress">PLC的Ip地址</param>
	/// <param name="port">PLC的端口</param>
	public MelsecMcNet(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new MelsecQnA3EBinaryMessage();
	}

	/// <summary>
	/// 当前MC协议的分析地址的方法，对传入的字符串格式的地址进行数据解析。
	/// The current MC protocol's address analysis method performs data parsing on the address of the incoming string format.
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <param name="length">数据长度</param>
	/// <param name="isBit">当前是否读写bool操作</param>
	/// <returns>解析后的数据信息</returns>
	public virtual OperateResult<McAddressData> McAnalysisAddress(string address, ushort length, bool isBit)
	{
		return McAddressData.ParseMelsecFrom(address, length, isBit);
	}

	public override byte[] PackCommandWithHeader(byte[] command)
	{
		return McBinaryHelper.PackMcCommand(this, command);
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		OperateResult operateResult = McBinaryHelper.CheckResponseContentHelper(response);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(response.RemoveBegin(11));
	}

	/// <summary>
	/// 从PLC反馈的数据中提取出实际的数据内容，需要传入反馈数据，是否位读取
	/// </summary>
	/// <param name="response">反馈的数据内容</param>
	/// <param name="isBit">是否位读取</param>
	/// <returns>解析后的结果对象</returns>
	public virtual byte[] ExtractActualData(byte[] response, bool isBit)
	{
		return McBinaryHelper.ExtractActualDataHelper(response, isBit);
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组
	/// Batch read byte array information, need to specify the address and length, return the original byte array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return McHelper.Read(this, address, length);
	}

	[HslMqttApi("WriteByteArray", "")]
	public override OperateResult Write(string address, byte[] value)
	{
		return McHelper.Write(this, address, value);
	}

	/// <summary>
	/// 批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组
	/// Batch read byte array information, need to specify the address and length, return the original byte array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		return await McHelper.ReadAsync(this, address, length);
	}

	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await McHelper.WriteAsync(this, address, value);
	}

	[HslMqttApi("随机读取PLC的数据信息，可以跨地址，跨类型组合，但是每个地址只能读取一个word，也就是2个字节的内容。收到结果后，需要自行解析数据")]
	/// <summary>
	/// 随机读取PLC的数据信息，可以跨地址，跨类型组合，但是每个地址只能读取一个word，也就是2个字节的内容。收到结果后，需要自行解析数据
	/// Randomly read PLC data information, which can be combined across addresses and types, but each address can only read one word,
	/// which is the content of 2 bytes. After receiving the results, you need to parse the data yourself
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <param name="address">所有的地址的集合</param>
	/// <returns>结果</returns>
	/// <remarks>
	/// 访问安装有 Q 系列 C24/E71 的站 QCPU 上位站 经由 Q 系列兼容网络系统 MELSECNET/H MELSECNET/10 Ethernet 的 QCPU 其他站 时
	/// 访问点数········1≦ 字访问点数 双字访问点数 ≦192
	/// 访问 QnACPU 其他站 经由 QnA 系列兼容网络系统 MELSECNET/10 Ethernet 的 Q/QnACPU 其他站 时访问点数········1≦ 字访问点数 双字访问点数 ≦96
	/// 访问上述以外的 PLC CPU 其他站 时访问点数········1≦字访问点数≦10
	/// </remarks>
	public OperateResult<byte[]> ReadRandom(string[] address)
	{
		return McHelper.ReadRandom(this, address);
	}

	[HslMqttApi(ApiTopic = "ReadRandoms", Description = "随机读取PLC的数据信息，可以跨地址，跨类型组合，每个地址是任意的长度。收到结果后，需要自行解析数据，目前只支持字地址，比如D区，W区，R区，不支持X，Y，M，B，L等等")]
	/// <summary>
	/// 使用块读取PLC的数据信息，可以跨地址，跨类型组合，每个地址是任意的长度。收到结果后，需要自行解析数据，目前只支持字地址，比如D区，W区，R区，不支持X，Y，M，B，L等等
	/// Read the data information of the PLC randomly. It can be combined across addresses and types. Each address is of any length. After receiving the results,
	/// you need to parse the data yourself. Currently, only word addresses are supported, such as D area, W area, R area. X, Y, M, B, L, etc
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <param name="address">所有的地址的集合</param>
	/// <param name="length">每个地址的长度信息</param>
	/// <returns>结果</returns>
	/// <remarks>
	/// 实际测试不一定所有的plc都可以读取成功，具体情况需要具体分析
	/// 1 块数按照下列要求指定 120 ≧ 字软元件块数 + 位软元件块数
	/// 2 各软元件点数按照下列要求指定 960 ≧ 字软元件各块的合计点数 + 位软元件各块的合计点数
	/// </remarks>
	public OperateResult<byte[]> ReadRandom(string[] address, ushort[] length)
	{
		return McHelper.ReadRandom(this, address, length);
	}

	/// <summary>
	/// 随机字写入数据到PLC里，可以跨地址写入，指定一个字符串数组，一个地址需要对应2个字节，所以写入的字节长度为地址两倍。
	/// Randomly write data into the PLC. It can be written across addresses. Specify a string array. One address needs to correspond to 2 bytes, so the length of the written bytes is twice the address.
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <param name="address">所有的地址集合</param>
	/// <param name="data">写入数据</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteRandom(string[] address, byte[] values)
	{
		return McHelper.WriteRandom(this, address, values);
	}

	/// <summary>
	/// 随机读取PLC的数据信息，可以跨地址，跨类型组合，但是每个地址只能读取一个word，也就是2个字节的内容。收到结果后，自动转换为了short类型的数组
	/// Randomly read PLC data information, which can be combined across addresses and types, but each address can only read one word,
	/// which is the content of 2 bytes. After receiving the result, it is automatically converted to an array of type short.
	/// </summary>
	/// <param name="mc">MC协议的通信对象</param>
	/// <param name="address">所有的地址的集合</param>
	/// <returns>包含是否成功的结果对象</returns>
	/// <remarks>
	/// 访问安装有 Q 系列 C24/E71 的站 QCPU 上位站 经由 Q 系列兼容网络系统 MELSECNET/H MELSECNET/10 Ethernet 的 QCPU 其他站 时
	/// 访问点数········1≦ 字访问点数 双字访问点数 ≦192
	/// 访问 QnACPU 其他站 经由 QnA 系列兼容网络系统 MELSECNET/10 Ethernet 的 Q/QnACPU 其他站 时访问点数········1≦ 字访问点数 双字访问点数 ≦96
	/// 访问上述以外的 PLC CPU 其他站 时访问点数········1≦字访问点数≦10
	/// </remarks>
	public OperateResult<short[]> ReadRandomInt16(string[] address)
	{
		return McHelper.ReadRandomInt16(this, address);
	}

	/// <summary>
	/// 随机读取PLC的数据信息，可以跨地址，跨类型组合，但是每个地址只能读取一个word，也就是2个字节的内容。收到结果后，自动转换为了ushort类型的数组
	/// Randomly read PLC data information, which can be combined across addresses and types, but each address can only read one word,
	/// which is the content of 2 bytes. After receiving the result, it is automatically converted to an array of type ushort.
	/// </summary>
	/// <param name="mc">MC协议的通信对象</param>
	/// <param name="address">所有的地址的集合</param>
	/// <returns>包含是否成功的结果对象</returns>
	/// <remarks>
	/// 访问安装有 Q 系列 C24/E71 的站 QCPU 上位站 经由 Q 系列兼容网络系统 MELSECNET/H MELSECNET/10 Ethernet 的 QCPU 其他站 时
	/// 访问点数········1≦ 字访问点数 双字访问点数 ≦192
	/// 访问 QnACPU 其他站 经由 QnA 系列兼容网络系统 MELSECNET/10 Ethernet 的 Q/QnACPU 其他站 时访问点数········1≦ 字访问点数 双字访问点数 ≦96
	/// 访问上述以外的 PLC CPU 其他站 时访问点数········1≦字访问点数≦10
	/// </remarks>
	public OperateResult<ushort[]> ReadRandomUInt16(string[] address)
	{
		return McHelper.ReadRandomUInt16(this, address);
	}

	/// <summary>
	/// 随机读取PLC的数据信息，可以跨地址，跨类型组合，但是每个地址只能读取一个word，也就是2个字节的内容。收到结果后，需要自行解析数据
	/// Randomly read PLC data information, which can be combined across addresses and types, but each address can only read one word,
	/// which is the content of 2 bytes. After receiving the results, you need to parse the data yourself
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <param name="address">所有的地址的集合</param>
	/// <returns>结果</returns>
	/// <remarks>
	/// 访问安装有 Q 系列 C24/E71 的站 QCPU 上位站 经由 Q 系列兼容网络系统 MELSECNET/H MELSECNET/10 Ethernet 的 QCPU 其他站 时
	/// 访问点数········1≦ 字访问点数 双字访问点数 ≦192
	/// 访问 QnACPU 其他站 经由 QnA 系列兼容网络系统 MELSECNET/10 Ethernet 的 Q/QnACPU 其他站 时访问点数········1≦ 字访问点数 双字访问点数 ≦96
	/// 访问上述以外的 PLC CPU 其他站 时访问点数········1≦字访问点数≦10
	/// </remarks>
	public async Task<OperateResult<byte[]>> ReadRandomAsync(string[] address)
	{
		return await McHelper.ReadRandomAsync(this, address);
	}

	/// <summary>
	/// 使用块读取PLC的数据信息，可以跨地址，跨类型组合，每个地址是任意的长度。收到结果后，需要自行解析数据，目前只支持字地址，比如D区，W区，R区，不支持X，Y，M，B，L等等
	/// Read the data information of the PLC randomly. It can be combined across addresses and types. Each address is of any length. After receiving the results,
	/// you need to parse the data yourself. Currently, only word addresses are supported, such as D area, W area, R area. X, Y, M, B, L, etc
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <param name="address">所有的地址的集合</param>
	/// <param name="length">每个地址的长度信息</param>
	/// <returns>结果</returns>
	/// <remarks>
	/// 实际测试不一定所有的plc都可以读取成功，具体情况需要具体分析
	/// 1 块数按照下列要求指定 120 ≧ 字软元件块数 + 位软元件块数
	/// 2 各软元件点数按照下列要求指定 960 ≧ 字软元件各块的合计点数 + 位软元件各块的合计点数
	/// </remarks>
	public async Task<OperateResult<byte[]>> ReadRandomAsync(string[] address, ushort[] length)
	{
		return await McHelper.ReadRandomAsync(this, address, length);
	}

	/// <summary>
	/// 随机字写入数据到PLC里，可以跨地址写入，指定一个字符串数组，一个地址需要对应2个字节，所以写入的字节长度为地址两倍。
	/// Randomly write data into the PLC. It can be written across addresses. Specify a string array. One address needs to correspond to 2 bytes, so the length of the written bytes is twice the address.
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <param name="address">所有的地址集合</param>
	/// <param name="data">写入数据</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteRandomAsync(string[] address, byte[] values)
	{
		return await McHelper.WriteRandomAsync(this, address, values);
	}

	/// <summary>
	/// 随机读取PLC的数据信息，可以跨地址，跨类型组合，但是每个地址只能读取一个word，也就是2个字节的内容。收到结果后，自动转换为了short类型的数组
	/// Randomly read PLC data information, which can be combined across addresses and types, but each address can only read one word,
	/// which is the content of 2 bytes. After receiving the result, it is automatically converted to an array of type short.
	/// </summary>
	/// <param name="mc">MC协议的通信对象</param>
	/// <param name="address">所有的地址的集合</param>
	/// <returns>包含是否成功的结果对象</returns>
	/// <remarks>
	/// 访问安装有 Q 系列 C24/E71 的站 QCPU 上位站 经由 Q 系列兼容网络系统 MELSECNET/H MELSECNET/10 Ethernet 的 QCPU 其他站 时
	/// 访问点数········1≦ 字访问点数 双字访问点数 ≦192
	/// 访问 QnACPU 其他站 经由 QnA 系列兼容网络系统 MELSECNET/10 Ethernet 的 Q/QnACPU 其他站 时访问点数········1≦ 字访问点数 双字访问点数 ≦96
	/// 访问上述以外的 PLC CPU 其他站 时访问点数········1≦字访问点数≦10
	/// </remarks>
	public async Task<OperateResult<short[]>> ReadRandomInt16Async(string[] address)
	{
		return await McHelper.ReadRandomInt16Async(this, address);
	}

	/// <summary>
	/// 随机读取PLC的数据信息，可以跨地址，跨类型组合，但是每个地址只能读取一个word，也就是2个字节的内容。收到结果后，自动转换为了ushort类型的数组
	/// Randomly read PLC data information, which can be combined across addresses and types, but each address can only read one word,
	/// which is the content of 2 bytes. After receiving the result, it is automatically converted to an array of type ushort.
	/// </summary>
	/// <param name="mc">MC协议的通信对象</param>
	/// <param name="address">所有的地址的集合</param>
	/// <returns>包含是否成功的结果对象</returns>
	/// <remarks>
	/// 访问安装有 Q 系列 C24/E71 的站 QCPU 上位站 经由 Q 系列兼容网络系统 MELSECNET/H MELSECNET/10 Ethernet 的 QCPU 其他站 时
	/// 访问点数········1≦ 字访问点数 双字访问点数 ≦192
	/// 访问 QnACPU 其他站 经由 QnA 系列兼容网络系统 MELSECNET/10 Ethernet 的 Q/QnACPU 其他站 时访问点数········1≦ 字访问点数 双字访问点数 ≦96
	/// 访问上述以外的 PLC CPU 其他站 时访问点数········1≦字访问点数≦10
	/// </remarks>
	public async Task<OperateResult<ushort[]>> ReadRandomUInt16Async(string[] address)
	{
		return await McHelper.ReadRandomUInt16Async(this, address);
	}

	[HslMqttApi("ReadBool", "")]
	/// <summary>
	/// 读取单个的数据信息
	/// Read a single  data message
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <returns>带有成功标识的 bool 值</returns>
	public override OperateResult<bool> ReadBool(string address)
	{
		return base.ReadBool(address);
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 批量读取数组信息，需要指定地址和长度，返回 数组
	/// Batch read  array information, need to specify the address and length, return  array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的 bool[] 数组</returns>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		return McHelper.ReadBool(this, address, length);
	}

	[HslMqttApi("WriteBoolArray", "")]
	public override OperateResult Write(string address, bool[] values)
	{
		return McHelper.Write(this, address, values);
	}

	/// <summary>
	/// 读取单个的数据信息
	/// Read a single  data message
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <returns>带有成功标识的 bool 值</returns>
	public override async Task<OperateResult<bool>> ReadBoolAsync(string address)
	{
		return await base.ReadBoolAsync(address);
	}

	/// <summary>
	/// 批量读取数组信息，需要指定地址和长度，返回 数组
	/// Batch read  array information, need to specify the address and length, return  array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的 bool[] 数组</returns>
	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		return await McHelper.ReadBoolAsync(this, address, length);
	}

	public override async Task<OperateResult> WriteAsync(string address, bool[] values)
	{
		return await McHelper.WriteAsync(this, address, values);
	}

	[HslMqttApi(ApiTopic = "ReadTag", Description = "读取PLC的标签信息，需要传入标签的名称，读取的字长度，标签举例：A; label[1]; bbb[10,10,10]")]
	/// <param name="tag">数据标签</param>
	/// <param name="length">读取的数据长度</param>
	public OperateResult<byte[]> ReadTags(string tag, ushort length)
	{
		return ReadTags(new string[1] { tag }, new ushort[1] { length });
	}

	[HslMqttApi(ApiTopic = "ReadTags", Description = "批量读取PLC的标签信息，需要传入标签的名称，读取的字长度，标签举例：A; label[1]; bbb[10,10,10]")]
	/// <param name="tag">数据标签</param>
	/// <param name="length">读取的数据长度</param>
	public OperateResult<byte[]> ReadTags(string[] tags, ushort[] length)
	{
		return McBinaryHelper.ReadTags(this, tags, length);
	}

	/// <param name="tag">数据标签</param>
	/// <param name="length">读取的数据长度</param>
	public async Task<OperateResult<byte[]>> ReadTagsAsync(string tag, ushort length)
	{
		return await ReadTagsAsync(new string[1] { tag }, new ushort[1] { length });
	}

	/// <param name="tag">数据标签</param>
	/// <param name="length">读取的数据长度</param>
	public async Task<OperateResult<byte[]>> ReadTagsAsync(string[] tags, ushort[] length)
	{
		return await McBinaryHelper.ReadTagsAsync(this, tags, length);
	}

	[HslMqttApi(ApiTopic = "ReadExtend", Description = "读取扩展的数据信息，需要在原有的地址，长度信息之外，输入扩展值信息")]
	/// <summary>
	/// [商业授权] 读取扩展的数据信息，需要在原有的地址，长度信息之外，输入扩展值信息
	/// [Authorization] To read the extended data information, you need to enter the extended value information in addition to the original address and length information
	/// </summary>
	/// <param name="mc">MC通信对象</param>
	/// <param name="extend">扩展信息</param>
	/// <param name="address">地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>返回结果</returns>
	public OperateResult<byte[]> ReadExtend(ushort extend, string address, ushort length)
	{
		return McHelper.ReadExtend(this, extend, address, length);
	}

	/// <summary>
	/// [商业授权] 读取扩展的数据信息，需要在原有的地址，长度信息之外，输入扩展值信息
	/// [Authorization] To read the extended data information, you need to enter the extended value information in addition to the original address and length information
	/// </summary>
	/// <param name="mc">MC通信对象</param>
	/// <param name="extend">扩展信息</param>
	/// <param name="address">地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>返回结果</returns>
	public async Task<OperateResult<byte[]>> ReadExtendAsync(ushort extend, string address, ushort length)
	{
		return await McHelper.ReadExtendAsync(this, extend, address, length);
	}

	[HslMqttApi(ApiTopic = "ReadMemory", Description = "读取缓冲寄存器的数据信息，地址直接为偏移地址")]
	/// <summary>
	/// [商业授权] 读取缓冲寄存器的数据信息，地址直接为偏移地址
	/// [Authorization] Read the data information of the buffer register, the address is directly the offset address
	/// </summary>
	/// <param name="mc">MC通信对象</param>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>读取的内容</returns>
	/// <remarks>
	/// 本指令不可以访问下述缓冲存储器:
	/// 1. 本站(SLMP对应设备)上安装的智能功能模块
	/// 2. 其它站缓冲存储器
	/// </remarks>
	public OperateResult<byte[]> ReadMemory(string address, ushort length)
	{
		return McHelper.ReadMemory(this, address, length);
	}

	/// <summary>
	/// [商业授权] 读取缓冲寄存器的数据信息，地址直接为偏移地址
	/// [Authorization] Read the data information of the buffer register, the address is directly the offset address
	/// </summary>
	/// <param name="mc">MC通信对象</param>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>读取的内容</returns>
	/// <remarks>
	/// 本指令不可以访问下述缓冲存储器:
	/// 1. 本站(SLMP对应设备)上安装的智能功能模块
	/// 2. 其它站缓冲存储器
	/// </remarks>
	public async Task<OperateResult<byte[]>> ReadMemoryAsync(string address, ushort length)
	{
		return await McHelper.ReadMemoryAsync(this, address, length);
	}

	[HslMqttApi(ApiTopic = "ReadSmartModule", Description = "读取智能模块的数据信息，需要指定模块地址，偏移地址，读取的字节长度")]
	/// <summary>
	/// [商业授权] 读取智能模块的数据信息，需要指定模块地址，偏移地址，读取的字节长度
	/// [Authorization] To read the extended data information, you need to enter the extended value information in addition to the original address and length information
	/// </summary>
	/// <param name="mc">MC通信对象</param>
	/// <param name="module">模块地址</param>
	/// <param name="address">地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>返回结果</returns>
	public OperateResult<byte[]> ReadSmartModule(ushort module, string address, ushort length)
	{
		return McHelper.ReadSmartModule(this, module, address, length);
	}

	/// <summary>
	/// [商业授权] 读取智能模块的数据信息，需要指定模块地址，偏移地址，读取的字节长度
	/// [Authorization] To read the extended data information, you need to enter the extended value information in addition to the original address and length information
	/// </summary>
	/// <param name="mc">MC通信对象</param>
	/// <param name="module">模块地址</param>
	/// <param name="address">地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>返回结果</returns>
	public async Task<OperateResult<byte[]>> ReadSmartModuleAsync(ushort module, string address, ushort length)
	{
		return await McHelper.ReadSmartModuleAsync(this, module, address, length);
	}

	[HslMqttApi(ApiTopic = "RemoteRun", Description = "远程Run操作")]
	/// <summary>
	/// 远程Run操作
	/// Remote Run Operation
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>是否成功</returns>
	public OperateResult RemoteRun()
	{
		return McHelper.RemoteRun(this);
	}

	[HslMqttApi(ApiTopic = "RemoteStop", Description = "远程Stop操作")]
	/// <summary>
	/// 远程Stop操作
	/// Remote Stop operation
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>是否成功</returns>
	public OperateResult RemoteStop()
	{
		return McHelper.RemoteStop(this);
	}

	[HslMqttApi(ApiTopic = "RemoteReset", Description = "LED 熄灭 出错代码初始化")]
	/// <summary>
	/// 远程Reset操作
	/// Remote Reset Operation
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>是否成功</returns>
	public OperateResult RemoteReset()
	{
		return McHelper.RemoteReset(this);
	}

	[HslMqttApi(ApiTopic = "ReadPlcType", Description = "读取PLC的型号信息，例如 Q02HCPU")]
	/// <summary>
	/// 读取PLC的型号信息，例如 Q02HCPU
	/// Read PLC model information, such as Q02HCPU
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>返回型号的结果对象</returns>
	public OperateResult<string> ReadPlcType()
	{
		return McHelper.ReadPlcType(this);
	}

	[HslMqttApi(ApiTopic = "ErrorStateReset", Description = "LED 熄灭 出错代码初始化")]
	/// <summary>
	/// LED 熄灭 出错代码初始化
	/// LED off Error code initialization
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>是否成功</returns>
	public OperateResult ErrorStateReset()
	{
		return McHelper.ErrorStateReset(this);
	}

	/// <summary>
	/// 远程Run操作
	/// Remote Run Operation
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>是否成功</returns>
	public async Task<OperateResult> RemoteRunAsync()
	{
		return await McHelper.RemoteRunAsync(this);
	}

	/// <summary>
	/// 远程Stop操作
	/// Remote Stop operation
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>是否成功</returns>
	public async Task<OperateResult> RemoteStopAsync()
	{
		return await McHelper.RemoteStopAsync(this);
	}

	/// <summary>
	/// 远程Reset操作
	/// Remote Reset Operation
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>是否成功</returns>
	public async Task<OperateResult> RemoteResetAsync()
	{
		return await McHelper.RemoteResetAsync(this);
	}

	/// <summary>
	/// 读取PLC的型号信息，例如 Q02HCPU
	/// Read PLC model information, such as Q02HCPU
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>返回型号的结果对象</returns>
	public async Task<OperateResult<string>> ReadPlcTypeAsync()
	{
		return await McHelper.ReadPlcTypeAsync(this);
	}

	/// <summary>
	/// LED 熄灭 出错代码初始化
	/// LED off Error code initialization
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>是否成功</returns>
	public async Task<OperateResult> ErrorStateResetAsync()
	{
		return await McHelper.ErrorStateResetAsync(this);
	}

	public override string ToString()
	{
		return $"MelsecMcNet[{IpAddress}:{Port}]";
	}
}
