// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.IO.Ports;
using HslCommunication.Core.Pipe;
using HslCommunication.Reflection;

namespace HslCommunication.Core.Device;

/// <summary>
/// 串口的设备类对象信息
/// </summary>
public class DeviceSerialPort : DeviceCommunication
{
	private PipeSerialPort pipe;

	public override CommunicationPipe CommunicationPipe
	{
		get
		{
			return base.CommunicationPipe;
		}
		set
		{
			base.CommunicationPipe = value;
			if (value is PipeSerialPort pipeSerialPort)
			{
				pipe = pipeSerialPort;
				PortName = pipe.GetPipe().PortName;
				BaudRate = pipe.GetPipe().BaudRate;
			}
		}
	}

	[HslMqttApi(Description = "Gets or sets a value indicating whether the request sending (RTS) signal is enabled in serial communication.")]
	/// <summary>
	/// 获取或设置一个值，该值指示在串行通信中是否启用请求发送 (RTS) 信号。
	/// Gets or sets a value indicating whether the request sending (RTS) signal is enabled in serial communication.
	/// </summary>
	public bool RtsEnable
	{
		get
		{
			return pipe.RtsEnable;
		}
		set
		{
			pipe.RtsEnable = value;
		}
	}

	[HslMqttApi(Description = "Get or set the number of consecutive empty data receptions, which is valid when data reception is completed, default is 1")]
	/// <summary>
	/// 获取或设置连续接收空的数据次数，在数据接收完成时有效，每个单位消耗的时间为。
	/// Obtain or set the number of consecutive times to receive empty data, which is valid when the data is received, and the time consumed by each unit is
	/// </summary>
	public int ReceiveEmptyDataCount
	{
		get
		{
			return pipe.ReceiveEmptyDataCount;
		}
		set
		{
			pipe.ReceiveEmptyDataCount = value;
		}
	}

	[HslMqttApi(Description = "Whether to empty the buffer before sending data, the default is false")]
	/// <summary>
	/// 是否在发送数据前清空缓冲数据，默认是false
	/// Whether to empty the buffer before sending data, the default is false
	/// </summary>
	public bool IsClearCacheBeforeRead
	{
		get
		{
			return pipe.IsClearCacheBeforeRead;
		}
		set
		{
			pipe.IsClearCacheBeforeRead = value;
		}
	}

	[HslMqttApi(Description = "The port name of the current connection serial port information")]
	/// <summary>
	/// 当前连接串口信息的端口号名称
	/// The port name of the current connection serial port information
	/// </summary>
	public string PortName { get; private set; }

	[HslMqttApi(Description = "Baud rate of current connection serial port information")]
	/// <summary>
	/// 当前连接串口信息的波特率
	/// Baud rate of current connection serial port information
	/// </summary>
	public int BaudRate { get; private set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public DeviceSerialPort()
	{
		pipe = new PipeSerialPort();
		CommunicationPipe = pipe;
	}

	/// <summary>
	/// 初始化串口信息，9600波特率，8位数据位，1位停止位，无奇偶校验
	/// Initial serial port information, 9600 baud rate, 8 data bits, 1 stop bit, no parity
	/// </summary>
	/// <param name="portName">端口号信息，例如&quot;COM3&quot;</param>
	/// <remarks>
	/// portName 支持格式化的方式，例如输入 COM3-9600-8-N-1，COM5-19200-7-E-2，其中奇偶校验的字母可选，N:无校验，O：奇校验，E:偶校验，停止位可选 0, 1, 2, 1.5 四种选项
	/// </remarks>
	public virtual void SerialPortInni(string portName)
	{
		pipe.SerialPortInni(portName);
	}

	/// <summary>
	/// 初始化串口信息，波特率，8位数据位，1位停止位，无奇偶校验
	/// Initializes serial port information, baud rate, 8-bit data bit, 1-bit stop bit, no parity
	/// </summary>
	/// <param name="portName">端口号信息，例如&quot;COM3&quot;</param>
	/// <param name="baudRate">波特率</param>
	public virtual void SerialPortInni(string portName, int baudRate)
	{
		SerialPortInni(portName, baudRate, 8, (StopBits)1, (Parity)0);
	}

	/// <summary>
	/// 初始化串口信息，波特率，数据位，停止位，奇偶校验需要全部自己来指定
	/// Start serial port information, baud rate, data bit, stop bit, parity all need to be specified
	/// </summary>
	/// <param name="portName">端口号信息，例如&quot;COM3&quot;</param>
	/// <param name="baudRate">波特率</param>
	/// <param name="dataBits">数据位</param>
	/// <param name="stopBits">停止位</param>
	/// <param name="parity">奇偶校验</param>
	public virtual void SerialPortInni(string portName, int baudRate, int dataBits, StopBits stopBits, Parity parity)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		pipe.SerialPortInni(portName, baudRate, dataBits, stopBits, parity);
		PortName = portName;
		BaudRate = baudRate;
	}

	/// <summary>
	/// 根据自定义初始化方法进行初始化串口信息
	/// Initialize the serial port information according to the custom initialization method
	/// </summary>
	/// <param name="initi">初始化的委托方法</param>
	public void SerialPortInni(Action<SerialPort> initi)
	{
		pipe.SerialPortInni(initi);
		PortName = pipe.GetPipe().PortName;
		BaudRate = pipe.GetPipe().BaudRate;
	}

	/// <summary>
	/// 打开一个新的串行端口连接
	/// Open a new serial port connection
	/// </summary>
	public virtual OperateResult Open()
	{
		OperateResult<bool> operateResult = CommunicationPipe.OpenCommunication();
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		if (operateResult.Content)
		{
			return InitializationOnConnect();
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 获取一个值，指示串口是否处于打开状态
	/// Gets a value indicating whether the serial port is open
	/// </summary>
	/// <returns>是或否</returns>
	public bool IsOpen()
	{
		if (CommunicationPipe is PipeSerialPort pipeSerialPort)
		{
			return pipeSerialPort.GetPipe().IsOpen;
		}
		if (CommunicationPipe is PipeMoxa pipeMoxa)
		{
			return pipeMoxa.IsOpen();
		}
		return pipe.GetPipe().IsOpen;
	}

	/// <summary>
	/// 关闭当前的串口连接
	/// Close the current serial connection
	/// </summary>
	public void Close()
	{
		if (CommunicationPipe is PipeSerialPort)
		{
			if (pipe.GetPipe().IsOpen)
			{
				ExtraOnDisconnect();
				pipe.CloseCommunication();
			}
		}
		else
		{
			ExtraOnDisconnect();
			CommunicationPipe.CloseCommunication();
		}
	}

	public override string ToString()
	{
		return $"DeviceSerialPort<{base.ByteTransform}>{{{CommunicationPipe}}}";
	}
}
