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
using HslCommunication.Profinet.Melsec.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 三菱的R系列的MC协议，支持的地址类型和  有区别，详细请查看对应的API文档说明
/// </summary>
public class MelsecMcRNet : DeviceTcpNet, IReadWriteMc, IReadWriteDevice, IReadWriteNet
{
	public McType McType => McType.McRBinary;

	/// <summary>
	/// 网络号，通常为0
	/// Network number, usually 0
	/// </summary>
	/// <remarks>
	/// 依据PLC的配置而配置，如果PLC配置了1，那么此处也填0，如果PLC配置了2，此处就填2，测试不通的话，继续测试0
	/// </remarks>
	public byte NetworkNumber { get; set; } = 0;

	/// <summary>
	/// 网络站号，通常为0
	/// Network station number, usually 0
	/// </summary>
	/// <remarks>
	/// 依据PLC的配置而配置，如果PLC配置了1，那么此处也填0，如果PLC配置了2，此处就填2，测试不通的话，继续测试0
	/// </remarks>
	public byte NetworkStationNumber { get; set; } = 0;

	/// <summary>
	/// PLC编号，如果是本站信息，则是 0xFF 值，其他站则根据实际的情况指定。
	/// The PLC number, if it is the information of this station, is the 0xFF value, and other stations are specified according to the actual situation.
	/// </summary>
	public byte PLCNumber { get; set; } = byte.MaxValue;

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
	/// 实例化三菱R系列的Qna兼容3E帧协议的通讯对象
	/// Instantiate the communication object of Mitsubishi's Qna compatible 3E frame protocol
	/// </summary>
	public MelsecMcRNet()
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
	public MelsecMcRNet(string ipAddress, int port)
	{
		base.WordLength = 1;
		IpAddress = ipAddress;
		Port = port;
		base.ByteTransform = new RegularByteTransform();
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
		return McAddressData.ParseMelsecRFrom(address, length, isBit);
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
	public byte[] ExtractActualData(byte[] response, bool isBit)
	{
		return McBinaryHelper.ExtractActualDataHelper(response, isBit);
	}

	[HslMqttApi("ReadByteArray", "")]
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return McHelper.Read(this, address, length);
	}

	[HslMqttApi("WriteByteArray", "")]
	public override OperateResult Write(string address, byte[] value)
	{
		return McHelper.Write(this, address, value);
	}

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

	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		return McHelper.ReadBool(this, address, length);
	}

	[HslMqttApi("WriteBoolArray", "")]
	public override OperateResult Write(string address, bool[] values)
	{
		return McHelper.Write(this, address, values);
	}

	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		return await McHelper.ReadBoolAsync(this, address, length);
	}

	public override async Task<OperateResult> WriteAsync(string address, bool[] values)
	{
		return await McHelper.WriteAsync(this, address, values);
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
		return $"MelsecMcRNet[{IpAddress}:{Port}]";
	}

	/// <summary>
	/// 分析三菱R系列的地址，并返回解析后的数据对象
	/// </summary>
	/// <param name="address">字符串地址</param>
	/// <returns>是否解析成功</returns>
	public static OperateResult<MelsecMcDataType, int> AnalysisAddress(string address)
	{
		try
		{
			if (address.StartsWith("LSTS"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_LSTS, Convert.ToInt32(address.Substring(4), MelsecMcDataType.R_LSTS.FromBase));
			}
			if (address.StartsWith("LSTC"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_LSTC, Convert.ToInt32(address.Substring(4), MelsecMcDataType.R_LSTC.FromBase));
			}
			if (address.StartsWith("LSTN"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_LSTN, Convert.ToInt32(address.Substring(4), MelsecMcDataType.R_LSTN.FromBase));
			}
			if (address.StartsWith("STS"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_STS, Convert.ToInt32(address.Substring(3), MelsecMcDataType.R_STS.FromBase));
			}
			if (address.StartsWith("STC"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_STC, Convert.ToInt32(address.Substring(3), MelsecMcDataType.R_STC.FromBase));
			}
			if (address.StartsWith("STN"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_STN, Convert.ToInt32(address.Substring(3), MelsecMcDataType.R_STN.FromBase));
			}
			if (address.StartsWith("LTS"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_LTS, Convert.ToInt32(address.Substring(3), MelsecMcDataType.R_LTS.FromBase));
			}
			if (address.StartsWith("LTC"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_LTC, Convert.ToInt32(address.Substring(3), MelsecMcDataType.R_LTC.FromBase));
			}
			if (address.StartsWith("LTN"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_LTN, Convert.ToInt32(address.Substring(3), MelsecMcDataType.R_LTN.FromBase));
			}
			if (address.StartsWith("LCS"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_LCS, Convert.ToInt32(address.Substring(3), MelsecMcDataType.R_LCS.FromBase));
			}
			if (address.StartsWith("LCC"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_LCC, Convert.ToInt32(address.Substring(3), MelsecMcDataType.R_LCC.FromBase));
			}
			if (address.StartsWith("LCN"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_LCN, Convert.ToInt32(address.Substring(3), MelsecMcDataType.R_LCN.FromBase));
			}
			if (address.StartsWith("TS"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_TS, Convert.ToInt32(address.Substring(2), MelsecMcDataType.R_TS.FromBase));
			}
			if (address.StartsWith("TC"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_TC, Convert.ToInt32(address.Substring(2), MelsecMcDataType.R_TC.FromBase));
			}
			if (address.StartsWith("TN"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_TN, Convert.ToInt32(address.Substring(2), MelsecMcDataType.R_TN.FromBase));
			}
			if (address.StartsWith("CS"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_CS, Convert.ToInt32(address.Substring(2), MelsecMcDataType.R_CS.FromBase));
			}
			if (address.StartsWith("CC"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_CC, Convert.ToInt32(address.Substring(2), MelsecMcDataType.R_CC.FromBase));
			}
			if (address.StartsWith("CN"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_CN, Convert.ToInt32(address.Substring(2), MelsecMcDataType.R_CN.FromBase));
			}
			if (address.StartsWith("SM"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_SM, Convert.ToInt32(address.Substring(2), MelsecMcDataType.R_SM.FromBase));
			}
			if (address.StartsWith("SB"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_SB, Convert.ToInt32(address.Substring(2), MelsecMcDataType.R_SB.FromBase));
			}
			if (address.StartsWith("DX"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_DX, Convert.ToInt32(address.Substring(2), MelsecMcDataType.R_DX.FromBase));
			}
			if (address.StartsWith("DY"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_DY, Convert.ToInt32(address.Substring(2), MelsecMcDataType.R_DY.FromBase));
			}
			if (address.StartsWith("SD"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_SD, Convert.ToInt32(address.Substring(2), MelsecMcDataType.R_SD.FromBase));
			}
			if (address.StartsWith("SW"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_SW, Convert.ToInt32(address.Substring(2), MelsecMcDataType.R_SW.FromBase));
			}
			if (address.StartsWith("X"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_X, Convert.ToInt32(address.Substring(1), MelsecMcDataType.R_X.FromBase));
			}
			if (address.StartsWith("Y"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_Y, Convert.ToInt32(address.Substring(1), MelsecMcDataType.R_Y.FromBase));
			}
			if (address.StartsWith("M"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_M, Convert.ToInt32(address.Substring(1), MelsecMcDataType.R_M.FromBase));
			}
			if (address.StartsWith("L"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_L, Convert.ToInt32(address.Substring(1), MelsecMcDataType.R_L.FromBase));
			}
			if (address.StartsWith("F"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_F, Convert.ToInt32(address.Substring(1), MelsecMcDataType.R_F.FromBase));
			}
			if (address.StartsWith("V"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_V, Convert.ToInt32(address.Substring(1), MelsecMcDataType.R_V.FromBase));
			}
			if (address.StartsWith("S"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_S, Convert.ToInt32(address.Substring(1), MelsecMcDataType.R_S.FromBase));
			}
			if (address.StartsWith("B"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_B, Convert.ToInt32(address.Substring(1), MelsecMcDataType.R_B.FromBase));
			}
			if (address.StartsWith("D"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_D, Convert.ToInt32(address.Substring(1), MelsecMcDataType.R_D.FromBase));
			}
			if (address.StartsWith("W"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_W, Convert.ToInt32(address.Substring(1), MelsecMcDataType.R_W.FromBase));
			}
			if (address.StartsWith("R"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_R, Convert.ToInt32(address.Substring(1), MelsecMcDataType.R_R.FromBase));
			}
			if (address.StartsWith("Z"))
			{
				return OperateResult.CreateSuccessResult(MelsecMcDataType.R_Z, Convert.ToInt32(address.Substring(1), MelsecMcDataType.R_Z.FromBase));
			}
			return new OperateResult<MelsecMcDataType, int>(StringResources.Language.NotSupportedDataType);
		}
		catch (Exception ex)
		{
			return new OperateResult<MelsecMcDataType, int>(ex.Message);
		}
	}

	/// <summary>
	/// 从三菱地址，是否位读取进行创建读取的MC的核心报文
	/// </summary>
	/// <param name="address">地址数据</param>
	/// <param name="isBit">是否进行了位读取操作</param>
	/// <returns>带有成功标识的报文对象</returns>
	public static byte[] BuildReadMcCoreCommand(McAddressData address, bool isBit)
	{
		return new byte[12]
		{
			1,
			4,
			(byte)(isBit ? 3 : 2),
			0,
			BitConverter.GetBytes(address.AddressStart)[0],
			BitConverter.GetBytes(address.AddressStart)[1],
			BitConverter.GetBytes(address.AddressStart)[2],
			BitConverter.GetBytes(address.AddressStart)[3],
			BitConverter.GetBytes(address.McDataType.DataCode)[0],
			BitConverter.GetBytes(address.McDataType.DataCode)[1],
			(byte)(address.Length % 256),
			(byte)(address.Length / 256)
		};
	}

	/// <summary>
	/// 以字为单位，创建数据写入的核心报文
	/// </summary>
	/// <param name="address">三菱的数据地址</param>
	/// <param name="value">实际的原始数据信息</param>
	/// <returns>带有成功标识的报文对象</returns>
	public static byte[] BuildWriteWordCoreCommand(McAddressData address, byte[] value)
	{
		if (value == null)
		{
			value = new byte[0];
		}
		byte[] buffer = new byte[12 + value.Length];
		buffer[0] = 1;
		buffer[1] = 20;
		buffer[2] = 2;
		buffer[3] = 0;
		buffer[4] = BitConverter.GetBytes(address.AddressStart)[0];
		buffer[5] = BitConverter.GetBytes(address.AddressStart)[1];
		buffer[6] = BitConverter.GetBytes(address.AddressStart)[2];
		buffer[7] = BitConverter.GetBytes(address.AddressStart)[3];
		buffer[8] = BitConverter.GetBytes(address.McDataType.DataCode)[0];
		buffer[9] = BitConverter.GetBytes(address.McDataType.DataCode)[1];
		buffer[10] = (byte)(value.Length / 2 % 256);
		buffer[11] = (byte)(value.Length / 2 / 256);
		value.CopyTo(buffer, 12);
		return buffer;
	}

	/// <summary>
	/// 按字为单位随机读取的指令创建
	/// </summary>
	/// <param name="address">地址数组</param>
	/// <returns>指令</returns>
	public static List<byte[]> BuildReadRandomWordCommand(McAddressData[] address)
	{
		List<byte[]> list = new List<byte[]>();
		int length = 90;
		List<McAddressData[]> list2 = SoftBasic.ArraySplitByLength(address, length);
		for (int i = 0; i < list2.Count; i++)
		{
			McAddressData[] array = list2[i];
			byte[] buffer = new byte[6 + array.Length * 6];
			buffer[0] = 3;
			buffer[1] = 4;
			buffer[2] = 2;
			buffer[3] = 0;
			buffer[4] = (byte)array.Length;
			buffer[5] = 0;
			for (int j = 0; j < array.Length; j++)
			{
				buffer[j * 6 + 6] = BitConverter.GetBytes(array[j].AddressStart)[0];
				buffer[j * 6 + 7] = BitConverter.GetBytes(array[j].AddressStart)[1];
				buffer[j * 6 + 8] = BitConverter.GetBytes(array[j].AddressStart)[2];
				buffer[j * 6 + 9] = BitConverter.GetBytes(array[j].AddressStart)[3];
				buffer[j * 6 + 10] = BitConverter.GetBytes(array[j].McDataType.DataCode)[0];
				buffer[j * 6 + 11] = BitConverter.GetBytes(array[j].McDataType.DataCode)[1];
			}
			list.Add(buffer);
		}
		return list;
	}

	/// <summary>
	/// 随机读取的指令创建
	/// </summary>
	/// <param name="address">地址数组</param>
	/// <returns>指令</returns>
	public static byte[] BuildReadRandomCommand(McAddressData[] address)
	{
		byte[] buffer = new byte[6 + address.Length * 8];
		buffer[0] = 6;
		buffer[1] = 4;
		buffer[2] = 2;
		buffer[3] = 0;
		buffer[4] = (byte)address.Length;
		buffer[5] = 0;
		for (int i = 0; i < address.Length; i++)
		{
			buffer[i * 8 + 6] = BitConverter.GetBytes(address[i].AddressStart)[0];
			buffer[i * 8 + 7] = BitConverter.GetBytes(address[i].AddressStart)[1];
			buffer[i * 8 + 8] = BitConverter.GetBytes(address[i].AddressStart)[2];
			buffer[i * 8 + 9] = BitConverter.GetBytes(address[i].AddressStart)[3];
			buffer[i * 8 + 10] = BitConverter.GetBytes(address[i].McDataType.DataCode)[0];
			buffer[i * 8 + 11] = BitConverter.GetBytes(address[i].McDataType.DataCode)[1];
			buffer[i * 8 + 12] = (byte)(address[i].Length % 256);
			buffer[i * 8 + 13] = (byte)(address[i].Length / 256);
		}
		return buffer;
	}

	/// <summary>
	/// 按字为单位随机写入的指令创建，一次最多写入80/160个字
	/// </summary>
	/// <param name="address">地址数组</param>
	/// <param name="value">写入的值</param>
	/// <returns>指令</returns>
	public static byte[] BuildWriteRandomWordCommand(McAddressData[] address, byte[] value)
	{
		byte[] buffer = new byte[6 + address.Length * 6 + value.Length];
		buffer[0] = 2;
		buffer[1] = 20;
		buffer[2] = 2;
		buffer[3] = 0;
		buffer[4] = (byte)address.Length;
		buffer[5] = 0;
		for (int i = 0; i < address.Length; i++)
		{
			buffer[i * 8 + 6] = BitConverter.GetBytes(address[i].AddressStart)[0];
			buffer[i * 8 + 7] = BitConverter.GetBytes(address[i].AddressStart)[1];
			buffer[i * 8 + 8] = BitConverter.GetBytes(address[i].AddressStart)[2];
			buffer[i * 8 + 9] = BitConverter.GetBytes(address[i].AddressStart)[3];
			buffer[i * 8 + 10] = BitConverter.GetBytes(address[i].McDataType.DataCode)[0];
			buffer[i * 8 + 11] = BitConverter.GetBytes(address[i].McDataType.DataCode)[1];
			Array.Copy(value, i * 2, buffer, i * 8 + 12, 2);
		}
		return buffer;
	}

	/// <summary>
	/// 以位为单位，创建数据写入的核心报文
	/// </summary>
	/// <param name="address">三菱的地址信息</param>
	/// <param name="value">原始的bool数组数据</param>
	/// <returns>带有成功标识的报文对象</returns>
	public static byte[] BuildWriteBitCoreCommand(McAddressData address, bool[] value)
	{
		if (value == null)
		{
			value = new bool[0];
		}
		byte[] array = MelsecHelper.TransBoolArrayToByteData(value);
		byte[] buffer = new byte[12 + array.Length];
		buffer[0] = 1;
		buffer[1] = 20;
		buffer[2] = 3;
		buffer[3] = 0;
		buffer[4] = BitConverter.GetBytes(address.AddressStart)[0];
		buffer[5] = BitConverter.GetBytes(address.AddressStart)[1];
		buffer[6] = BitConverter.GetBytes(address.AddressStart)[2];
		buffer[7] = BitConverter.GetBytes(address.AddressStart)[3];
		buffer[8] = BitConverter.GetBytes(address.McDataType.DataCode)[0];
		buffer[9] = BitConverter.GetBytes(address.McDataType.DataCode)[1];
		buffer[10] = (byte)(value.Length % 256);
		buffer[11] = (byte)(value.Length / 256);
		array.CopyTo(buffer, 12);
		return buffer;
	}
}
