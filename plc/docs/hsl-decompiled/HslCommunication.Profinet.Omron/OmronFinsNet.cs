// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Profinet.Omron.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Omron;

/// <summary>
/// 欧姆龙PLC通讯类，采用Fins-Tcp通信协议实现，支持的地址信息参见api文档信息。本协议下PLC默认的端口号为 9600，也可以手动更改，重启PLC更改生效。
/// Omron PLC communication class is implemented using Fins-Tcp communication protocol. For the supported address information, please refer to the api document information.
/// The default port number of the PLC under this protocol is 9600, and it can also be changed manually. Restart the PLC to make the changes take effect.
/// </summary>
/// <remarks>
/// PLC的IP地址的要求，最后一个整数的范围应该小于250，否则会发生连接不上的情况。
/// 如果在测试的时候报错误码64，经网友 上海-Lex 指点，是因为PLC中产生了报警，如伺服报警，模块错误等产生的，但是数据还是能正常读到的，屏蔽64报警或清除plc错误可解决
/// 如果碰到NX系列连接失败，或是无法读取的，需要使用网口2，配置ip地址，网线连接网口2，配置FINSTCP，把UDP的端口改成9601的，这样就可以读写了。
/// 需要特别注意属性，在超长数据读取时，规定了切割读取的长度，在不是CP1H及扩展模块的时候，可以设置为999，提高一倍的通信速度。
/// </remarks>
public class OmronFinsNet : DeviceTcpNet, IOmronFins, IReadWriteDevice, IReadWriteNet
{
	private readonly byte[] handSingle = new byte[20]
	{
		70, 73, 78, 83, 0, 0, 0, 12, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0
	};

	private readonly SoftIncrementCount incrementSID = new SoftIncrementCount(255L, 0L);

	/// <summary>
	/// 信息控制字段，默认0x80
	/// Information control field, default 0x80
	/// </summary>
	public byte ICF { get; set; } = 128;

	/// <summary>
	/// 系统使用的内部信息
	/// Internal information used by the system
	/// </summary>
	public byte RSV { get; private set; } = 0;

	/// <summary>
	/// 网络层信息，默认0x02，如果有八层消息，就设置为0x07
	/// Network layer information, default is 0x02, if there are eight layers of messages, set to 0x07
	/// </summary>
	public byte GCT { get; set; } = 2;

	/// <summary>
	/// PLC的网络号地址，默认0x00
	/// PLC network number address, default 0x00
	/// </summary>
	/// <remarks>
	/// 00: Local network
	/// 01-7F: Remote network address (decimal: 1 to 127)
	/// </remarks>
	public byte DNA { get; set; } = 0;

	[HslMqttApi(HttpMethod = "GET", Description = "The node address of the PLC is 0 by default. During the process of connecting with the PLC, the value of DA1 is automatically obtained from the PLC.")]
	/// <summary>
	/// PLC的节点地址，默认为0，在和PLC连接的过程中，自动从PLC获取到DA1的值。
	/// The node address of the PLC is 0 by default. During the process of connecting with the PLC, the value of DA1 is automatically obtained from the PLC.
	/// </summary>
	public byte DA1 { get; set; } = 0;

	/// <summary>
	/// PLC的单元号地址，通常都为0
	/// PLC unit number address, usually 0
	/// </summary>
	/// <remarks>
	/// 00: CPU Unit
	/// FE: Controller Link Unit or Ethernet Unit connected to network
	/// 10 TO 1F: CPU Bus Unit
	/// E1: Inner Board
	/// </remarks>
	public byte DA2 { get; set; } = 0;

	/// <summary>
	/// 上位机的网络号地址
	/// Network number and address of the computer
	/// </summary>
	/// <remarks>
	/// 00: Local network
	/// 01-7F: Remote network (1 to 127 decimal)
	/// </remarks>
	public byte SNA { get; set; } = 0;

	[HslMqttApi(HttpMethod = "GET", Description = "The node address of the host computer is 0x01 by default. After connecting to the PLC, the PLC will set the current value.")]
	/// <summary>
	/// 上位机的节点地址，默认是0x01，当连接PLC之后，将由PLC来设定当前的值。
	/// The node address of the host computer is 0x01 by default. After connecting to the PLC, the PLC will set the current value.
	/// </summary>
	/// <remarks>
	/// v9.6.5版本及之前的版本都需要手动设置，如果是多连接，相同的节点是连接不上PLC的。
	/// </remarks>
	public byte SA1 { get; set; } = 1;

	/// <summary>
	/// 上位机的单元号地址
	/// Unit number and address of the computer
	/// </summary>
	/// <remarks>
	/// 00: CPU Unit
	/// 10-1F: CPU Bus Unit
	/// </remarks>
	public byte SA2 { get; set; }

	/// <summary>
	/// 服务的标识号，由客户端生成自增的顺序值，用来标识和校验通信报文的ID
	/// Service ID. Used to identify the process generating the transmission. Set the SID to any number between 00 and FF
	/// </summary>
	public byte SID { get; set; } = 0;

	/// <summary>
	/// 进行字读取的时候对于超长的情况按照本属性进行切割，默认500，如果不是CP1H及扩展模块的，可以设置为999，可以提高一倍的通信速度。
	/// When reading words, it is cut according to this attribute for the case of overlength. The default is 500.
	/// If it is not for CP1H and expansion modules, it can be set to 999, which can double the communication speed.
	/// </summary>
	public int ReadSplits { get; set; } = 500;

	/// <summary>
	/// 当接收PLC返回的数据的时候，获取或设置是否需要接收数据直到空为止，在一些及其特殊的场景里，可以设置为 true 防止数据错误的情况。
	/// When receiving the data returned by the PLC, it is necessary to obtain or set whether to receive data until empty,
	/// and in some special scenarios, it can be set to true to prevent data errors.
	/// </summary>
	public bool ReceiveUntilEmpty { get; set; } = false;

	/// <summary>
	/// 获取或设置欧姆龙PLC的系列信息
	/// Obtain or set the series information of Omron PLC
	/// </summary>
	public OmronPlcType PlcType { get; set; } = OmronPlcType.CSCJ;

	/// <summary>
	/// 实例化一个欧姆龙PLC Fins帧协议的通讯对象
	/// Instantiate a communication object of Omron PLC Fins frame protocol
	/// </summary>
	public OmronFinsNet()
	{
		base.WordLength = 1;
		base.ByteTransform = new RegularByteTransform(DataFormat.CDAB);
		base.ByteTransform.IsStringReverseByteWord = true;
	}

	/// <summary>
	/// 指定ip地址和端口号来实例化一个欧姆龙PLC Fins帧协议的通讯对象
	/// Specify the IP address and port number to instantiate a communication object of the Omron PLC Fins frame protocol
	/// </summary>
	/// <param name="ipAddress">PLCd的Ip地址</param>
	/// <param name="port">PLC的端口</param>
	public OmronFinsNet(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new FinsMessage();
	}

	/// <summary>
	/// 将普通的指令打包成完整的指令
	/// </summary>
	/// <param name="cmd">FINS的核心指令</param>
	/// <returns>完整的可用于发送PLC的命令</returns>
	private byte[] PackCommand(byte[] cmd)
	{
		byte[] buffer = new byte[26 + cmd.Length];
		Array.Copy(handSingle, 0, buffer, 0, 4);
		byte[] bytes = BitConverter.GetBytes(buffer.Length - 8);
		Array.Reverse(bytes);
		bytes.CopyTo(buffer, 4);
		buffer[11] = 2;
		buffer[16] = ICF;
		buffer[17] = RSV;
		buffer[18] = GCT;
		buffer[19] = DNA;
		buffer[20] = DA1;
		buffer[21] = DA2;
		buffer[22] = SNA;
		buffer[23] = SA1;
		buffer[24] = SA2;
		buffer[25] = (byte)incrementSID.GetCurrentValue();
		cmd.CopyTo(buffer, 26);
		SID = buffer[25];
		return buffer;
	}

	protected override OperateResult InitializationOnConnect()
	{
		OperateResult<byte[]> operateResult = ReadFromCoreServer(CommunicationPipe, handSingle, hasResponseData: true, usePackAndUnpack: false);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		int num = BitConverter.ToInt32(new byte[4]
		{
			operateResult.Content[15],
			operateResult.Content[14],
			operateResult.Content[13],
			operateResult.Content[12]
		}, 0);
		if (num != 0)
		{
			return new OperateResult(num, OmronFinsNetHelper.GetStatusDescription(num));
		}
		if (operateResult.Content.Length >= 20)
		{
			SA1 = operateResult.Content[19];
		}
		if (operateResult.Content.Length >= 24)
		{
			DA1 = operateResult.Content[23];
		}
		incrementSID.ResetStartValue(0L);
		return OperateResult.CreateSuccessResult();
	}

	protected override async Task<OperateResult> InitializationOnConnectAsync()
	{
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(CommunicationPipe, handSingle, hasResponseData: true, usePackAndUnpack: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		int status = BitConverter.ToInt32(new byte[4]
		{
			read.Content[15],
			read.Content[14],
			read.Content[13],
			read.Content[12]
		}, 0);
		if (status != 0)
		{
			return new OperateResult(status, OmronFinsNetHelper.GetStatusDescription(status));
		}
		if (read.Content.Length >= 20)
		{
			SA1 = read.Content[19];
		}
		if (read.Content.Length >= 24)
		{
			DA1 = read.Content[23];
		}
		incrementSID.ResetStartValue(0L);
		return OperateResult.CreateSuccessResult();
	}

	public override byte[] PackCommandWithHeader(byte[] command)
	{
		return PackCommand(command);
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return OmronFinsNetHelper.ResponseValidAnalysis(response);
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 从欧姆龙PLC中读取想要的数据，返回读取结果，读取长度的单位为字，地址格式为"D100","C100","W100","H100","A100"
	/// Read the desired data from the Omron PLC and return the read result. The unit of the read length is word. The address format is "D100", "C100", "W100", "H100", "A100"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">读取地址，格式为&quot;D100&quot;,&quot;C100&quot;,&quot;W100&quot;,&quot;H100&quot;,&quot;A100&quot;</param>
	/// <param name="length">读取的数据长度</param>
	/// <param name="splits">分割信息</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return OmronFinsNetHelper.Read(this, address, length, ReadSplits);
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 向PLC写入数据，数据格式为原始的字节类型，地址格式为"D100","C100","W100","H100","A100"
	/// Write data to PLC, the data format is the original byte type, and the address format is "D100", "C100", "W100", "H100", "A100"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">初始地址</param>
	/// <param name="value">原始的字节数据</param>
	/// <returns>结果</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		return OmronFinsNetHelper.Write(this, address, value);
	}

	[HslMqttApi("ReadString", "")]
	public override OperateResult<string> ReadString(string address, ushort length)
	{
		return base.ReadString(address, length, Encoding.UTF8);
	}

	[HslMqttApi("WriteString", "")]
	public override OperateResult Write(string address, string value)
	{
		return base.Write(address, value, Encoding.UTF8);
	}

	/// <summary>
	/// 从欧姆龙PLC中读取多个地址的数据，返回读取结果，每个地址按照字为单位读取，地址格式为"D100","C100","W100","H100","A100"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">从欧姆龙PLC中读取多个地址的数据，返回读取结果，每个地址按照字为单位读取，地址格式为&quot;D100&quot;,&quot;C100&quot;,&quot;W100&quot;,&quot;H100&quot;,&quot;A100&quot;</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public OperateResult<byte[]> Read(string[] address)
	{
		return OmronFinsNetHelper.Read(this, address);
	}

	/// <summary>
	/// 从欧姆龙PLC中读取想要的数据，返回读取结果，读取长度的单位为字，地址格式为"D100","C100","W100","H100","A100"
	/// Read the desired data from the Omron PLC and return the read result. The unit of the read length is word. The address format is "D100", "C100", "W100", "H100", "A100"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">读取地址，格式为&quot;D100&quot;,&quot;C100&quot;,&quot;W100&quot;,&quot;H100&quot;,&quot;A100&quot;</param>
	/// <param name="length">读取的数据长度</param>
	/// <param name="splits">分割信息</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		return await OmronFinsNetHelper.ReadAsync(this, address, length, ReadSplits);
	}

	/// <summary>
	/// 向PLC写入数据，数据格式为原始的字节类型，地址格式为"D100","C100","W100","H100","A100"
	/// Write data to PLC, the data format is the original byte type, and the address format is "D100", "C100", "W100", "H100", "A100"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">初始地址</param>
	/// <param name="value">原始的字节数据</param>
	/// <returns>结果</returns>
	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await OmronFinsNetHelper.WriteAsync(this, address, value);
	}

	public override async Task<OperateResult<string>> ReadStringAsync(string address, ushort length)
	{
		return await base.ReadStringAsync(address, length, Encoding.UTF8);
	}

	public override async Task<OperateResult> WriteAsync(string address, string value)
	{
		return await base.WriteAsync(address, value, Encoding.UTF8);
	}

	/// <summary>
	/// 从欧姆龙PLC中读取多个地址的数据，返回读取结果，每个地址按照字为单位读取，地址格式为"D100","C100","W100","H100","A100"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">从欧姆龙PLC中读取多个地址的数据，返回读取结果，每个地址按照字为单位读取，地址格式为&quot;D100&quot;,&quot;C100&quot;,&quot;W100&quot;,&quot;H100&quot;,&quot;A100&quot;</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public async Task<OperateResult<byte[]>> ReadAsync(string[] address)
	{
		return await OmronFinsNetHelper.ReadAsync(this, address);
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 从欧姆龙PLC中批量读取位软元件，地址格式为"D100.0","C100.0","W100.0","H100.0","A100.0"
	/// Read bit devices in batches from Omron PLC with address format "D100.0", "C100.0", "W100.0", "H100.0", "A100.0"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">读取地址，格式为&quot;D100&quot;,&quot;C100&quot;,&quot;W100&quot;,&quot;H100&quot;,&quot;A100&quot;</param>
	/// <param name="length">读取的长度</param>
	/// <param name="splits">分割信息</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		return OmronFinsNetHelper.ReadBool(this, address, length, ReadSplits);
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// 向PLC中位软元件写入bool数组，返回是否写入成功，比如你写入D100,values[0]对应D100.0，地址格式为"D100.0","C100.0","W100.0","H100.0","A100.0"
	/// Write the bool array to the PLC's median device and return whether the write was successful. For example, if you write D100, values [0] corresponds to D100.0
	/// and the address format is "D100.0", "C100.0", "W100. 0 "," H100.0 "," A100.0 "
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">要写入的数据地址</param>
	/// <param name="values">要写入的实际数据，可以指定任意的长度</param>
	/// <returns>返回写入结果</returns>
	public override OperateResult Write(string address, bool[] values)
	{
		return OmronFinsNetHelper.Write(this, address, values);
	}

	/// <summary>
	/// 从欧姆龙PLC中批量读取位软元件，地址格式为"D100.0","C100.0","W100.0","H100.0","A100.0"
	/// Read bit devices in batches from Omron PLC with address format "D100.0", "C100.0", "W100.0", "H100.0", "A100.0"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">读取地址，格式为&quot;D100&quot;,&quot;C100&quot;,&quot;W100&quot;,&quot;H100&quot;,&quot;A100&quot;</param>
	/// <param name="length">读取的长度</param>
	/// <param name="splits">分割信息</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		return await OmronFinsNetHelper.ReadBoolAsync(this, address, length, ReadSplits);
	}

	/// <summary>
	/// 向PLC中位软元件写入bool数组，返回是否写入成功，比如你写入D100,values[0]对应D100.0，地址格式为"D100.0","C100.0","W100.0","H100.0","A100.0"
	/// Write the bool array to the PLC's median device and return whether the write was successful. For example, if you write D100, values [0] corresponds to D100.0
	/// and the address format is "D100.0", "C100.0", "W100. 0 "," H100.0 "," A100.0 "
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">要写入的数据地址</param>
	/// <param name="values">要写入的实际数据，可以指定任意的长度</param>
	/// <returns>返回写入结果</returns>
	public override async Task<OperateResult> WriteAsync(string address, bool[] values)
	{
		return await OmronFinsNetHelper.WriteAsync(this, address, values);
	}

	[HslMqttApi(ApiTopic = "Run", Description = "将CPU单元的操作模式更改为RUN，从而使PLC能够执行其程序。")]
	/// <summary>
	/// 将CPU单元的操作模式更改为RUN，从而使PLC能够执行其程序。
	/// Changes the CPU Unit’s operating mode to RUN, enabling the PLC to execute its program.
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <returns>是否启动成功</returns>
	/// <remarks>
	/// 当执行RUN时，CPU单元将开始运行。 在执行RUN之前，您必须确认系统的安全性。 启用“禁止覆盖受保护程序”设置时，无法执行此命令。
	/// The CPU Unit will start operation when RUN is executed. You must confirm the safety of the system before executing RUN.
	/// When the “prohibit overwriting of protected program” setting is enabled, this command cannot be executed.
	/// </remarks>
	public OperateResult Run()
	{
		return OmronFinsNetHelper.Run(this);
	}

	[HslMqttApi(ApiTopic = "Stop", Description = "将CPU单元的操作模式更改为PROGRAM，停止程序执行。")]
	/// <summary>
	/// 将CPU单元的操作模式更改为PROGRAM，停止程序执行。
	/// Changes the CPU Unit’s operating mode to PROGRAM, stopping program execution.
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <returns>是否停止成功</returns>
	/// <remarks>
	/// 当执行STOP时，CPU单元将停止操作。 在执行STOP之前，您必须确认系统的安全性。
	/// The CPU Unit will stop operation when STOP is executed. You must confirm the safety of the system before executing STOP.
	/// </remarks>
	public OperateResult Stop()
	{
		return OmronFinsNetHelper.Stop(this);
	}

	[HslMqttApi(ApiTopic = "ReadCpuUnitData", Description = "读取CPU的一些数据信息，主要包含型号，版本，一些数据块的大小。")]
	/// <summary>
	/// [商业授权] 读取CPU的一些数据信息，主要包含型号，版本，一些数据块的大小
	/// [Authorization] Read some data information of the CPU, mainly including the model, version, and the size of some data blocks
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <returns>是否读取成功</returns>
	public OperateResult<OmronCpuUnitData> ReadCpuUnitData()
	{
		return OmronFinsNetHelper.ReadCpuUnitData(this);
	}

	[HslMqttApi(ApiTopic = "ReadCpuUnitStatus", Description = "读取CPU单元的一些操作状态数据，主要包含运行状态，工作模式，错误信息等。")]
	/// <summary>
	/// [商业授权] 读取CPU单元的一些操作状态数据，主要包含运行状态，工作模式，错误信息等。
	/// [Authorization] Read some operating status data of the CPU unit, mainly including operating status, working mode, error information, etc.
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <returns>是否读取成功</returns>
	public OperateResult<OmronCpuUnitStatus> ReadCpuUnitStatus()
	{
		return OmronFinsNetHelper.ReadCpuUnitStatus(this);
	}

	[HslMqttApi(ApiTopic = "ReadCpuTime", Description = "读取CPU单元的时间信息。")]
	/// <summary>
	/// [商业授权] 读取CPU的时间信息
	/// [Authorization] Read the time information of the CPU
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <returns>是否读取成功</returns>
	public OperateResult<DateTime> ReadCpuTime()
	{
		return OmronFinsNetHelper.ReadCpuTime(this);
	}

	/// <summary>
	/// 将CPU单元的操作模式更改为RUN，从而使PLC能够执行其程序。
	/// Changes the CPU Unit’s operating mode to RUN, enabling the PLC to execute its program.
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <returns>是否启动成功</returns>
	/// <remarks>
	/// 当执行RUN时，CPU单元将开始运行。 在执行RUN之前，您必须确认系统的安全性。 启用“禁止覆盖受保护程序”设置时，无法执行此命令。
	/// The CPU Unit will start operation when RUN is executed. You must confirm the safety of the system before executing RUN.
	/// When the “prohibit overwriting of protected program” setting is enabled, this command cannot be executed.
	/// </remarks>
	public async Task<OperateResult> RunAsync()
	{
		return await OmronFinsNetHelper.RunAsync(this).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 将CPU单元的操作模式更改为PROGRAM，停止程序执行。
	/// Changes the CPU Unit’s operating mode to PROGRAM, stopping program execution.
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <returns>是否停止成功</returns>
	/// <remarks>
	/// 当执行STOP时，CPU单元将停止操作。 在执行STOP之前，您必须确认系统的安全性。
	/// The CPU Unit will stop operation when STOP is executed. You must confirm the safety of the system before executing STOP.
	/// </remarks>
	public async Task<OperateResult> StopAsync()
	{
		return await OmronFinsNetHelper.StopAsync(this).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// [商业授权] 读取CPU的一些数据信息，主要包含型号，版本，一些数据块的大小
	/// [Authorization] Read some data information of the CPU, mainly including the model, version, and the size of some data blocks
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <returns>是否读取成功</returns>
	public async Task<OperateResult<OmronCpuUnitData>> ReadCpuUnitDataAsync()
	{
		return await OmronFinsNetHelper.ReadCpuUnitDataAsync(this).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// [商业授权] 读取CPU单元的一些操作状态数据，主要包含运行状态，工作模式，错误信息等。
	/// [Authorization] Read some operating status data of the CPU unit, mainly including operating status, working mode, error information, etc.
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <returns>是否读取成功</returns>
	public async Task<OperateResult<OmronCpuUnitStatus>> ReadCpuUnitStatusAsync()
	{
		return await OmronFinsNetHelper.ReadCpuUnitStatusAsync(this).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// [商业授权] 读取CPU的时间信息
	/// [Authorization] Read the time information of the CPU
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <returns>是否读取成功</returns>
	public async Task<OperateResult<DateTime>> ReadCpuTimeAsync()
	{
		return await OmronFinsNetHelper.ReadCpuTimeAsync(this).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override string ToString()
	{
		return $"OmronFinsNet[{IpAddress}:{Port}]";
	}
}
