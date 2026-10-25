// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.IO.Ports;
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Profinet.Omron.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Omron;

/// <summary>
/// 欧姆龙的HostLink协议的实现，地址支持示例 DM区:D100; CIO区:C100; Work区:W100; Holding区:H100; Auxiliary区: A100
/// Implementation of Omron's HostLink protocol, address support example DM area: D100; CIO area: C100; Work area: W100; Holding area: H100; Auxiliary area: A100
/// </summary>
/// <remarks>
/// 感谢 深圳～拾忆 的测试，地址可以携带站号信息，例如 s=2;D100
/// 如果发现串口线和usb同时打开才能通信的情况，需要按照如下的操作：
/// 串口线不是标准的串口线，电脑的串口线的235引脚分别接PLC的329引脚，45线短接，就可以通讯，感谢 深圳-小君(QQ932507362)提供的解决方案。
/// </remarks>
public class OmronHostLink : DeviceSerialPort, IHostLink, IReadWriteDevice, IReadWriteNet
{
	/// <summary>
	/// Specifies whether or not there are network relays. Set “80” (ASCII: 38,30)
	/// when sending an FINS command to a CPU Unit on a network.Set “00” (ASCII: 30,30)
	/// when sending to a CPU Unit connected directly to the host computer.
	/// </summary>
	public byte ICF { get; set; } = 0;

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
	/// The response wait time sets the time from when the CPU Unit receives a command block until it starts
	/// to return a response.It can be set from 0 to F in hexadecimal, in units of 10 ms.
	/// If F(15) is set, the response will begin to be returned 150 ms (15 × 10 ms) after the command block was received.
	/// </summary>
	public byte ResponseWaitTime { get; set; } = 48;

	/// <summary>
	/// PLC设备的站号信息
	/// PLC device station number information
	/// </summary>
	public byte UnitNumber { get; set; }

	/// <summary>
	/// 进行字读取的时候对于超长的情况按照本属性进行切割，默认260。
	/// When reading words, it is cut according to this attribute for the case of overlength. The default is 260.
	/// </summary>
	public int ReadSplits { get; set; } = 260;

	/// <summary>
	/// 获取或设置欧姆龙PLC的系列信息
	/// Obtain or set the series information of Omron PLC
	/// </summary>
	public OmronPlcType PlcType { get; set; } = OmronPlcType.CSCJ;

	/// <summary>
	/// 实例化一个欧姆龙PLC Fins帧协议的通讯对象
	/// Instantiate a communication object of Omron PLC Fins frame protocol
	/// </summary>
	public OmronHostLink()
	{
		base.ByteTransform = new RegularByteTransform(DataFormat.CDAB);
		base.WordLength = 1;
		base.ByteTransform.IsStringReverseByteWord = true;
		LogMsgFormatBinary = false;
		base.ReceiveEmptyDataCount = 5;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new SpecifiedCharacterMessage(13);
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return OmronHostLinkHelper.ResponseValidAnalysis(send, response);
	}

	/// <summary>
	/// 初始化串口信息，9600波特率，7位数据位，1位停止位，偶校验
	/// Initial serial port information, 9600 baud rate, 7 data bits, 1 stop bit, even parity
	/// </summary>
	/// <param name="portName">端口号信息，例如&quot;COM3&quot;</param>
	public override void SerialPortInni(string portName)
	{
		base.SerialPortInni(portName);
	}

	/// <summary>
	/// 初始化串口信息，波特率，7位数据位，1位停止位，偶校验
	/// Initializes serial port information, baud rate, 7-bit data bit, 1-bit stop bit, even parity
	/// </summary>
	/// <param name="portName">端口号信息，例如&quot;COM3&quot;</param>
	/// <param name="baudRate">波特率</param>
	public override void SerialPortInni(string portName, int baudRate)
	{
		base.SerialPortInni(portName, baudRate, 7, (StopBits)1, (Parity)2);
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
		return OmronHostLinkHelper.Read(this, address, length);
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
		return OmronHostLinkHelper.Write(this, address, value);
	}

	/// <summary>
	/// 从欧姆龙PLC中读取多个地址的数据，返回读取结果，每个地址按照字为单位读取，地址格式为"D100","C100","W100","H100","A100"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">从欧姆龙PLC中读取多个地址的数据，返回读取结果，每个地址按照字为单位读取，地址格式为&quot;D100&quot;,&quot;C100&quot;,&quot;W100&quot;,&quot;H100&quot;,&quot;A100&quot;</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public OperateResult<byte[]> Read(string[] address)
	{
		return OmronHostLinkHelper.Read(this, address);
	}

	/// <summary>
	/// 从欧姆龙PLC中读取多个地址的数据，返回读取结果，每个地址按照字为单位读取，地址格式为"D100","C100","W100","H100","A100"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">从欧姆龙PLC中读取多个地址的数据，返回读取结果，每个地址按照字为单位读取，地址格式为&quot;D100&quot;,&quot;C100&quot;,&quot;W100&quot;,&quot;H100&quot;,&quot;A100&quot;</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public async Task<OperateResult<byte[]>> ReadAsync(string[] address)
	{
		return await OmronHostLinkHelper.ReadAsync(this, address);
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
		return OmronHostLinkHelper.ReadBool(this, address, length);
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
		return OmronHostLinkHelper.Write(this, address, values);
	}

	public override string ToString()
	{
		return $"OmronHostLink[{base.PortName}:{base.BaudRate}]";
	}
}
