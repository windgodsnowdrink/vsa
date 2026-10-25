// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Diagnostics;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Profinet.YASKAWA.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.YASKAWA;

/// <summary>
/// 扩展的Memobus协议信息，除了支持普通的线圈，输入继电器，保持寄存器，输入寄存器的读写操作，还支持扩展的保持寄存器和输入寄存器读写操作。
/// The extended Memobus protocol information not only supports reading and writing operations of ordinary coils, input relays,
/// holding registers, and input registers, but also supports reading and writing operations of extended holding registers and input registers.
/// </summary>
/// <remarks>
/// 其中线圈和输入继电器使用和,的方法，读取输入继电器地址：x=2;100。
/// 其他的方法针对的是寄存器，保持型寄存器地址：100或 x=3;100，输入寄存器：x=4;100，扩展保持型寄存器x=9;100，写入x=11;100, 扩展输入寄存器：x=10;100
/// The coil and input relay use  and , method,
/// read the input relay address: x=2;100. Other methods are for registers, holding register address: 100 or x=3;100, input register: x=4;100,
/// extended holding register x=9;100, writing x=11;100, extended input Register: x=10;100
/// 读取的最大的字为 2044 个字，写入的最大的字数为 2043 个字
/// </remarks>
public class MemobusTcpNet : DeviceTcpNet, IMemobus, IReadWriteDevice, IReadWriteNet
{
	private byte cpuTo = 2;

	private byte cpuFrom = 1;

	private readonly SoftIncrementCount softIncrementCount;

	/// <summary>
	/// 获取或设置发送目标的CPU的编号信息，默认为 2
	/// Get or set the CPU number information of the sending destination, the default is 2
	/// </summary>
	public byte CpuTo
	{
		get
		{
			return cpuTo;
		}
		set
		{
			cpuTo = value;
		}
	}

	/// <summary>
	/// 获取或设置发送源的CPU的编号信息，默认为 1
	/// Get or set the number information of the sending source CPU, the default is 1
	/// </summary>
	public byte CpuFrom
	{
		get
		{
			return cpuFrom;
		}
		set
		{
			cpuFrom = value;
		}
	}

	/// <summary>
	/// 实例化一个Memobus-Tcp协议的客户端对象
	/// Instantiate a client object of the Memobus-Tcp protocol
	/// </summary>
	public MemobusTcpNet()
	{
		softIncrementCount = new SoftIncrementCount(255L, 0L);
		base.WordLength = 1;
		base.ByteTransform = new RegularByteTransform(DataFormat.CDAB);
	}

	/// <summary>
	/// 指定服务器地址，端口号，客户端自己的站号来初始化
	/// Specify the server address, port number, and client's own station number to initialize
	/// </summary>
	/// <param name="ipAddress">服务器的Ip地址</param>
	/// <param name="port">服务器的端口号</param>
	public MemobusTcpNet(string ipAddress, int port = 502)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new MemobusMessage();
	}

	public override byte[] PackCommandWithHeader(byte[] command)
	{
		return MemobusHelper.PackCommandWithHeader(command, softIncrementCount.GetCurrentValue());
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return MemobusHelper.UnpackResponseContent(send, response);
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
		return MemobusHelper.ReadBool(this, address, length);
	}

	[HslMqttApi("WriteBool", "")]
	/// <summary>
	/// 写入单个的数据，返回是否成功
	/// Write a single  data, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, bool value)
	{
		return MemobusHelper.Write(this, address, value);
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
		return MemobusHelper.Write(this, address, value);
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
		return MemobusHelper.Read(this, address, length);
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 写入原始的byte数组数据到指定的地址，返回是否写入成功
	/// Write the original byte array data to the specified address, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		return MemobusHelper.Write(this, address, value);
	}

	[HslMqttApi("WriteInt16", "")]
	/// <summary>
	/// 写入short数据，返回是否成功
	/// Write short data, returns whether success
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override OperateResult Write(string address, short value)
	{
		if (ushort.TryParse(address, out var _))
		{
			return MemobusHelper.Write(this, address, value, base.Write);
		}
		return base.Write(address, value);
	}

	[HslMqttApi("WriteUInt16", "")]
	/// <summary>
	/// 写入ushort数据，返回是否成功
	/// Write ushort data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override OperateResult Write(string address, ushort value)
	{
		if (ushort.TryParse(address, out var _))
		{
			return MemobusHelper.Write(this, address, value, base.Write);
		}
		return base.Write(address, value);
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
		return await MemobusHelper.ReadBoolAsync(this, address, length).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 写入单个的数据，返回是否成功
	/// Write a single  data, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public override async Task<OperateResult> WriteAsync(string address, bool value)
	{
		return await MemobusHelper.WriteAsync(this, address, value).ConfigureAwait(continueOnCapturedContext: false);
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
		return await MemobusHelper.WriteAsync(this, address, value).ConfigureAwait(continueOnCapturedContext: false);
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
		return await MemobusHelper.ReadAsync(this, address, length).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 写入原始的byte数组数据到指定的地址，返回是否写入成功
	/// Write the original byte array data to the specified address, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await MemobusHelper.WriteAsync(this, address, value).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 写入short数据，返回是否成功
	/// Write short data, returns whether success
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, short value)
	{
		if (ushort.TryParse(address, out var _))
		{
			return await MemobusHelper.WriteAsync(this, address, value, [DebuggerHidden] (string address2, short value2) => base.WriteAsync(address2, value2)).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, value).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 写入ushort数据，返回是否成功
	/// Write ushort data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, ushort value)
	{
		if (ushort.TryParse(address, out var _))
		{
			return await MemobusHelper.WriteAsync(this, address, value, [DebuggerHidden] (string address2, ushort value2) => base.WriteAsync(address2, value2)).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, value).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 随机读取扩展的保持寄存器的内容，也即读取不连续地址的字数据，可以指定多个地址，然后一次性读取所有的数据，然后解析出实际的数据
	/// Randomly read the contents of the extended hold register, that is, read word data of discontinuous addresses,
	/// you can specify multiple addresses, then read all the data at once, and then parse out the actual data
	/// </summary>
	/// <param name="memobus">PLC通信对象</param>
	/// <param name="address">地址信息</param>
	/// <returns>读取的原始字节结果信息</returns>
	/// <remarks>
	/// 注意，本方法只能针对扩展的保持寄存器进行读取
	/// </remarks>
	public OperateResult<byte[]> ReadRandom(string[] address)
	{
		return MemobusHelper.ReadRandom(this, address);
	}

	/// <summary>
	/// 随机读取扩展的保持寄存器的内容，也即读取不连续地址的字数据，可以指定多个地址，然后一次性读取所有的数据，然后解析出实际的数据
	/// Randomly read the contents of the extended hold register, that is, read word data of discontinuous addresses,
	/// you can specify multiple addresses, then read all the data at once, and then parse out the actual data
	/// </summary>
	/// <param name="memobus">PLC通信对象</param>
	/// <param name="address">地址信息</param>
	/// <returns>读取的原始字节结果信息</returns>
	/// <remarks>
	/// 注意，本方法只能针对扩展的保持寄存器进行读取
	/// </remarks>
	public OperateResult<byte[]> ReadRandom(ushort[] address)
	{
		return MemobusHelper.ReadRandom(this, address);
	}

	/// <summary>
	/// 随机写入扩展的保持寄存器的内容，也即写入不连续的地址的字数据，字节数组的长度必须为地址数组长度的两倍，才能正确写入。
	/// Write the contents of the extended hold registers randomly, that is, write word data for discontinuous addresses,
	/// and the byte array must be twice the length of the address array to be written correctly.
	/// </summary>
	/// <param name="memobus">PLC通信对象</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">数据信息</param>
	/// <returns>是否写入成功的结果对象</returns>
	/// <remarks>
	/// 注意，本方法只能针对扩展的保持寄存器进行读取
	/// </remarks>
	public OperateResult WriteRandom(ushort[] address, byte[] value)
	{
		return MemobusHelper.WriteRandom(this, address, value);
	}

	/// <summary>
	/// 随机读取扩展的保持寄存器的内容，也即读取不连续地址的字数据，可以指定多个地址，然后一次性读取所有的数据，然后解析出实际的数据
	/// Randomly read the contents of the extended hold register, that is, read word data of discontinuous addresses,
	/// you can specify multiple addresses, then read all the data at once, and then parse out the actual data
	/// </summary>
	/// <param name="memobus">PLC通信对象</param>
	/// <param name="address">地址信息</param>
	/// <returns>读取的原始字节结果信息</returns>
	/// <remarks>
	/// 注意，本方法只能针对扩展的保持寄存器进行读取
	/// </remarks>
	public async Task<OperateResult<byte[]>> ReadRandomAsync(string[] address)
	{
		return await MemobusHelper.ReadRandomAsync(this, address).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 随机读取扩展的保持寄存器的内容，也即读取不连续地址的字数据，可以指定多个地址，然后一次性读取所有的数据，然后解析出实际的数据
	/// Randomly read the contents of the extended hold register, that is, read word data of discontinuous addresses,
	/// you can specify multiple addresses, then read all the data at once, and then parse out the actual data
	/// </summary>
	/// <param name="memobus">PLC通信对象</param>
	/// <param name="address">地址信息</param>
	/// <returns>读取的原始字节结果信息</returns>
	/// <remarks>
	/// 注意，本方法只能针对扩展的保持寄存器进行读取
	/// </remarks>
	public async Task<OperateResult<byte[]>> ReadRandomAsync(ushort[] address)
	{
		return await MemobusHelper.ReadRandomAsync(this, address).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 随机写入扩展的保持寄存器的内容，也即写入不连续的地址的字数据，字节数组的长度必须为地址数组长度的两倍，才能正确写入。
	/// Write the contents of the extended hold registers randomly, that is, write word data for discontinuous addresses,
	/// and the byte array must be twice the length of the address array to be written correctly.
	/// </summary>
	/// <param name="memobus">PLC通信对象</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">数据信息</param>
	/// <returns>是否写入成功的结果对象</returns>
	/// <remarks>
	/// 注意，本方法只能针对扩展的保持寄存器进行读取
	/// </remarks>
	public async Task<OperateResult> WriteRandomAsync(ushort[] address, byte[] value)
	{
		return await MemobusHelper.WriteRandomAsync(this, address, value).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override string ToString()
	{
		return $"MemobusTcpNet[{IpAddress}:{Port}]";
	}
}
