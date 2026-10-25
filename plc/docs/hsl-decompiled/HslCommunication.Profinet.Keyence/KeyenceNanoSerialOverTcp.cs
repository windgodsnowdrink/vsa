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
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Keyence;

/// <summary>
/// 基恩士KV上位链路协议的通信对象,适用于KV5000/5500/3000,KV1000,KV700,以及L20V通信模块，本类是基于tcp通信
/// The communication object of KEYENCE KV upper link protocol is suitable for KV5000/5500/3000, KV1000, KV700, and L20V communication modules. This type is based on tcp communication
/// </summary>
/// <remarks>
/// 位读写的数据类型为 R,B,MR,LR,CR,VB,以及读定时器的计数器的触点，字读写的数据类型为 DM,EM,FM,ZF,W,TM,Z,AT,CM,VM 双字读写为T,C,TC,CC,TS,CS。如果想要读写扩展的缓存器，地址示例：unit=2;1000  前面的是单元编号，后面的是偏移地址
/// 注意：在端口 2 以多分支连接 KV-L21V 时，请一定加上站号。在将端口 2 设定为使用 RS-422A、 RS-485 时， KV-L21V 即使接收本站以外的带站号的指令，也将变为无应答，不返回响应消息。
/// </remarks>
public class KeyenceNanoSerialOverTcp : DeviceTcpNet
{
	/// <summary>
	/// 获取或设置当前的站号信息，在RS232连接模式下，设置为0，如果是RS485/RS422连接下，必须设置正确的站号
	/// Get or set the current station number information. In RS232 connection mode, set it to 0.
	/// If it is RS485/RS422 connection, you must set the correct station number.
	/// </summary>
	public byte Station { get; set; }

	/// <summary>
	/// 获取或设置当前是否启用站号信息，当不启动站号时，在连接和断开的时候，将不使用站号报文。
	/// Get or set whether the station number information is currently enabled or not.
	/// When the station number is not activated, the station number message will not be used when connecting and disconnecting.
	/// </summary>
	public bool UseStation { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public KeyenceNanoSerialOverTcp()
	{
		base.WordLength = 1;
		base.ByteTransform = new RegularByteTransform();
		base.ByteTransform.IsStringReverseByteWord = true;
		LogMsgFormatBinary = false;
	}

	/// <summary>
	/// 使用指定的ip地址和端口号来初始化对象
	/// Initialize the object with the specified IP address and port number
	/// </summary>
	/// <param name="ipAddress">Ip地址数据</param>
	/// <param name="port">端口号</param>
	public KeyenceNanoSerialOverTcp(string ipAddress, int port = 8501)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new SpecifiedCharacterMessage(13, 10);
	}

	protected override OperateResult InitializationOnConnect()
	{
		OperateResult<byte[]> operateResult = ReadFromCoreServer(CommunicationPipe, KeyenceNanoHelper.GetConnectCmd(Station, UseStation), hasResponseData: true, usePackAndUnpack: true);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override OperateResult ExtraOnDisconnect()
	{
		OperateResult<byte[]> operateResult = ReadFromCoreServer(CommunicationPipe, KeyenceNanoHelper.GetDisConnectCmd(Station, UseStation), hasResponseData: true, usePackAndUnpack: true);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override async Task<OperateResult> InitializationOnConnectAsync()
	{
		OperateResult<byte[]> result = await ReadFromCoreServerAsync(CommunicationPipe, KeyenceNanoHelper.GetConnectCmd(Station, UseStation), hasResponseData: true, usePackAndUnpack: true);
		if (!result.IsSuccess)
		{
			return result;
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override async Task<OperateResult> ExtraOnDisconnectAsync()
	{
		OperateResult<byte[]> result = await ReadFromCoreServerAsync(CommunicationPipe, KeyenceNanoHelper.GetDisConnectCmd(Station, UseStation), hasResponseData: true, usePackAndUnpack: true);
		if (!result.IsSuccess)
		{
			return result;
		}
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi("ReadByteArray", "")]
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return KeyenceNanoHelper.Read(this, address, length);
	}

	[HslMqttApi("WriteByteArray", "")]
	public override OperateResult Write(string address, byte[] value)
	{
		return KeyenceNanoHelper.Write(this, address, value);
	}

	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		return await KeyenceNanoHelper.ReadAsync(this, address, length);
	}

	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await KeyenceNanoHelper.WriteAsync(this, address, value);
	}

	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		return KeyenceNanoHelper.ReadBool(this, address, length);
	}

	[HslMqttApi("WriteBool", "")]
	public override OperateResult Write(string address, bool value)
	{
		return KeyenceNanoHelper.Write(this, address, value);
	}

	[HslMqttApi("WriteBoolArray", "")]
	public override OperateResult Write(string address, bool[] value)
	{
		return KeyenceNanoHelper.Write(this, address, value);
	}

	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		return await KeyenceNanoHelper.ReadBoolAsync(this, address, length);
	}

	public override async Task<OperateResult> WriteAsync(string address, bool value)
	{
		return await KeyenceNanoHelper.WriteAsync(this, address, value);
	}

	public override async Task<OperateResult> WriteAsync(string address, bool[] value)
	{
		return await KeyenceNanoHelper.WriteAsync(this, address, value);
	}

	[HslMqttApi("清除CPU单元发生的错误")]
	/// <summary>
	/// [商业授权] 清除CPU单元发生的错误
	/// [Authorization] Clear CPU unit errors
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <returns>是否成功</returns>
	public OperateResult ClearError()
	{
		return KeyenceNanoHelper.ClearError(this);
	}

	[HslMqttApi("查询PLC的型号信息")]
	/// <summary>
	/// [商业授权] 查询PLC的型号信息
	/// [Authorization] Query PLC model information
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <returns>包含型号的结果对象</returns>
	public OperateResult<KeyencePLCS> ReadPlcType()
	{
		return KeyenceNanoHelper.ReadPlcType(this);
	}

	[HslMqttApi("读取当前PLC的模式，如果是0，代表 PROG模式或者梯形图未登录，如果为1，代表RUN模式")]
	/// <summary>
	/// [商业授权] 读取当前PLC的模式，如果是0，代表 PROG模式或者梯形图未登录，如果为1，代表RUN模式
	/// [Authorization] Read the current PLC mode, if it is 0, it means PROG mode or the ladder diagram is not registered, if it is 1, it means RUN mode
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <returns>包含模式的结果对象</returns>
	public OperateResult<int> ReadPlcMode()
	{
		return KeyenceNanoHelper.ReadPlcMode(this);
	}

	[HslMqttApi("设置PLC的时间")]
	/// <summary>
	/// [商业授权] 设置PLC的时间
	/// [Authorization] Set PLC time
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <param name="dateTime">时间数据</param>
	/// <returns>是否设置成功</returns>
	public OperateResult SetPlcDateTime(DateTime dateTime)
	{
		return KeyenceNanoHelper.SetPlcDateTime(this, dateTime);
	}

	[HslMqttApi("读取指定软元件的注释信息")]
	/// <summary>
	/// [商业授权] 读取指定软元件的注释信息
	/// [Authorization] Read the comment information of the specified device
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <param name="address">软元件的地址</param>
	/// <returns>软元件的注释信息</returns>
	public OperateResult<string> ReadAddressAnnotation(string address)
	{
		return KeyenceNanoHelper.ReadAddressAnnotation(this, address);
	}

	[HslMqttApi("从扩展单元缓冲存储器连续读取指定个数的数据，单位为字")]
	/// <summary>
	/// [商业授权] 从扩展单元缓冲存储器连续读取指定个数的数据，单位为字
	/// [Authorization] Continuously read the specified number of data from the expansion unit buffer memory, the unit is word
	/// </summary>
	/// <param name="keyence">PLC的通信对象</param>
	/// <param name="unit">单元编号</param>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取的长度，单位为字</param>
	/// <returns>包含是否成功的原始字节数组</returns>
	public OperateResult<byte[]> ReadExpansionMemory(byte unit, ushort address, ushort length)
	{
		return KeyenceNanoHelper.ReadExpansionMemory(this, unit, address, length);
	}

	[HslMqttApi("将原始字节数据写入到扩展的缓冲存储器，需要指定单元编号，偏移地址，写入的数据")]
	/// <summary>
	/// [商业授权] 将原始字节数据写入到扩展的缓冲存储器，需要指定单元编号，偏移地址，写入的数据
	/// [Authorization] To write the original byte data to the extended buffer memory, you need to specify the unit number, offset address, and write data
	/// </summary>
	/// <param name="keyence">PLC通信对象信息</param>
	/// <param name="unit">单元编号</param>
	/// <param name="address">偏移地址</param>
	/// <param name="value">等待写入的原始字节数据</param>
	/// <returns>是否写入成功的结果对象</returns>
	public OperateResult WriteExpansionMemory(byte unit, ushort address, byte[] value)
	{
		return KeyenceNanoHelper.WriteExpansionMemory(this, unit, address, value);
	}

	/// <summary>
	/// [商业授权] 清除CPU单元发生的错误
	/// [Authorization] Clear CPU unit errors
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <returns>是否成功</returns>
	public async Task<OperateResult> ClearErrorAsync()
	{
		return await KeyenceNanoHelper.ClearErrorAsync(this);
	}

	/// <summary>
	/// [商业授权] 查询PLC的型号信息
	/// [Authorization] Query PLC model information
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <returns>包含型号的结果对象</returns>
	public async Task<OperateResult<KeyencePLCS>> ReadPlcTypeAsync()
	{
		return await KeyenceNanoHelper.ReadPlcTypeAsync(this);
	}

	/// <summary>
	/// [商业授权] 读取当前PLC的模式，如果是0，代表 PROG模式或者梯形图未登录，如果为1，代表RUN模式
	/// [Authorization] Read the current PLC mode, if it is 0, it means PROG mode or the ladder diagram is not registered, if it is 1, it means RUN mode
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <returns>包含模式的结果对象</returns>
	public async Task<OperateResult<int>> ReadPlcModeAsync()
	{
		return await KeyenceNanoHelper.ReadPlcModeAsync(this);
	}

	/// <summary>
	/// [商业授权] 设置PLC的时间
	/// [Authorization] Set PLC time
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <param name="dateTime">时间数据</param>
	/// <returns>是否设置成功</returns>
	public async Task<OperateResult> SetPlcDateTimeAsync(DateTime dateTime)
	{
		return await KeyenceNanoHelper.SetPlcDateTimeAsync(this, dateTime);
	}

	/// <summary>
	/// [商业授权] 读取指定软元件的注释信息
	/// [Authorization] Read the comment information of the specified device
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <param name="address">软元件的地址</param>
	/// <returns>软元件的注释信息</returns>
	public async Task<OperateResult<string>> ReadAddressAnnotationAsync(string address)
	{
		return await KeyenceNanoHelper.ReadAddressAnnotationAsync(this, address);
	}

	/// <summary>
	/// [商业授权] 从扩展单元缓冲存储器连续读取指定个数的数据，单位为字
	/// [Authorization] Continuously read the specified number of data from the expansion unit buffer memory, the unit is word
	/// </summary>
	/// <param name="keyence">PLC的通信对象</param>
	/// <param name="unit">单元编号</param>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取的长度，单位为字</param>
	/// <returns>包含是否成功的原始字节数组</returns>
	public async Task<OperateResult<byte[]>> ReadExpansionMemoryAsync(byte unit, ushort address, ushort length)
	{
		return await KeyenceNanoHelper.ReadExpansionMemoryAsync(this, unit, address, length);
	}

	/// <summary>
	/// [商业授权] 将原始字节数据写入到扩展的缓冲存储器，需要指定单元编号，偏移地址，写入的数据
	/// [Authorization] To write the original byte data to the extended buffer memory, you need to specify the unit number, offset address, and write data
	/// </summary>
	/// <param name="keyence">PLC通信对象信息</param>
	/// <param name="unit">单元编号</param>
	/// <param name="address">偏移地址</param>
	/// <param name="value">等待写入的原始字节数据</param>
	/// <returns>是否写入成功的结果对象</returns>
	public async Task<OperateResult> WriteExpansionMemoryAsync(byte unit, ushort address, byte[] value)
	{
		return await KeyenceNanoHelper.WriteExpansionMemoryAsync(this, unit, address, value);
	}

	public override string ToString()
	{
		return $"KeyenceNanoSerialOverTcp[{IpAddress}:{Port}]";
	}
}
