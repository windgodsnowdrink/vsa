// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.IO.Ports;

namespace HslCommunication.Core.Pipe;

/// <summary>
/// 串口的管道类对象，可以在不同的串口类中使用一个串口的通道信息
/// The pipe class object of the serial port can use the channel information of a serial port in different serial port classes
/// </summary>
[Obsolete("Use class PipeSerialPort instead")]
public class PipeSerial : PipeBase, IDisposable
{
	private SerialPort serialPort;

	/// <summary>
	/// 获取或设置一个值，该值指示在串行通信中是否启用请求发送 (RTS) 信号。
	/// Gets or sets a value indicating whether the request sending (RTS) signal is enabled in serial communication.
	/// </summary>
	public bool RtsEnable
	{
		get
		{
			return serialPort.RtsEnable;
		}
		set
		{
			serialPort.RtsEnable = value;
		}
	}

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public PipeSerial()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		serialPort = new SerialPort();
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
	public void SerialPortInni(string portName, int baudRate, int dataBits, StopBits stopBits, Parity parity)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (!serialPort.IsOpen)
		{
			serialPort.PortName = portName;
			serialPort.BaudRate = baudRate;
			serialPort.DataBits = dataBits;
			serialPort.StopBits = stopBits;
			serialPort.Parity = parity;
		}
	}

	/// <summary>
	/// 根据自定义初始化方法进行初始化串口信息
	/// Initialize the serial port information according to the custom initialization method
	/// </summary>
	/// <param name="initi">初始化的委托方法</param>
	public void SerialPortInni(Action<SerialPort> initi)
	{
		if (!serialPort.IsOpen)
		{
			serialPort.PortName = "COM1";
			initi(serialPort);
		}
	}

	/// <summary>
	/// 打开一个新的串行端口连接
	/// Open a new serial port connection
	/// </summary>
	public OperateResult Open()
	{
		try
		{
			if (!serialPort.IsOpen)
			{
				serialPort.Open();
			}
			return OperateResult.CreateSuccessResult();
		}
		catch (Exception ex)
		{
			return new OperateResult(ex.Message);
		}
	}

	/// <summary>
	/// 获取一个值，指示串口是否处于打开状态
	/// Gets a value indicating whether the serial port is open
	/// </summary>
	/// <returns>是或否</returns>
	public bool IsOpen()
	{
		return serialPort.IsOpen;
	}

	/// <summary>
	/// 关闭当前的串口连接
	/// Close the current serial connection
	/// </summary>
	public OperateResult Close(Func<SerialPort, OperateResult> extraOnClose)
	{
		if (serialPort.IsOpen)
		{
			if (extraOnClose != null)
			{
				OperateResult operateResult = extraOnClose(serialPort);
				if (!operateResult.IsSuccess)
				{
					return operateResult;
				}
			}
			try
			{
				serialPort.Close();
			}
			catch (Exception ex)
			{
				return new OperateResult(ex.Message);
			}
		}
		return OperateResult.CreateSuccessResult();
	}

	public override void Dispose()
	{
		base.Dispose();
		((Component)(object)serialPort)?.Dispose();
	}

	/// <summary>
	/// 获取当前的串口对象信息
	/// Get current serial port object information
	/// </summary>
	/// <returns>串口对象</returns>
	public SerialPort GetPipe()
	{
		return serialPort;
	}

	public override string ToString()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		return $"PipeSerial[{serialPort.PortName},{serialPort.BaudRate},{serialPort.DataBits},{serialPort.StopBits},{serialPort.Parity}]";
	}
}
