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
using System.IO.Ports;
using HslCommunication.Core;
using HslCommunication.LogNet;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Toledo;

/// <summary>
/// 托利多电子秤的串口服务器对象
/// </summary>
public class ToledoSerial
{
	public delegate void ToledoStandardDataReceivedDelegate(object sender, ToledoStandardData toledoStandardData);

	private SerialPort serialPort;

	private ILogNet logNet;

	private int receiveTimeout;

	/// <summary>
	/// 组件的日志工具，支持日志记录，只要实例化后，当前网络的基本信息，就以等级进行输出
	/// The component's logging tool supports logging. As long as the instantiation of the basic network information, the output will be output at
	/// </summary>
	/// <remarks>
	/// 只要实例化即可以记录日志，实例化的对象需要实现接口  ，本组件提供了三个日志记录类，你可以实现基于   的对象。
	/// </remarks>
	public ILogNet LogNet
	{
		get
		{
			return logNet;
		}
		set
		{
			logNet = value;
		}
	}

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
	/// 当前连接串口信息的端口号名称
	/// The port name of the current connection serial port information
	/// </summary>
	public string PortName { get; private set; }

	/// <summary>
	/// 当前连接串口信息的波特率
	/// Baud rate of current connection serial port information
	/// </summary>
	public int BaudRate { get; private set; }

	[HslMqttApi(Description = "Timeout for receiving data, default is 5000ms")]
	/// <summary>
	/// 接收数据的超时时间，默认5000ms
	/// Timeout for receiving data, default is 5000ms
	/// </summary>
	public int ReceiveTimeout
	{
		get
		{
			return receiveTimeout;
		}
		set
		{
			receiveTimeout = value;
		}
	}

	/// <summary>
	/// 获取或设置当前的报文否是含有校验的，默认为含有校验
	/// </summary>
	public bool HasChk { get; set; }

	/// <summary>
	/// 当接收到一条新的托利多的数据的时候触发
	/// </summary>
	public event ToledoStandardDataReceivedDelegate OnToledoStandardDataReceived;

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public ToledoSerial()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		HasChk = false;
		receiveTimeout = 5000;
		serialPort = new SerialPort();
		serialPort.RtsEnable = true;
		serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
	}

	private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		List<byte> list = new List<byte>();
		byte[] buffer = new byte[1024];
		int num = 0;
		do
		{
			IL_014e:
			HslHelper.ThreadSleep(20);
			if (serialPort.BytesToRead < 1)
			{
				num++;
				continue;
			}
			num = 0;
			try
			{
				int bytesRead = serialPort.Read(buffer, 0, Math.Min(serialPort.BytesToRead, buffer.Length));
				byte[] buffer2 = new byte[bytesRead];
				Array.Copy(buffer, 0, buffer2, 0, bytesRead);
				list.AddRange(buffer2);
				if (HasChk)
				{
					if (list.Count <= 15 || list[list.Count - 2] != 13)
					{
						goto IL_014e;
					}
				}
				else if ((list.Count <= 15 || list[list.Count - 1] != 13) && (list.Count <= 15 || list[list.Count - 2] != 13))
				{
					goto IL_014e;
				}
			}
			catch (Exception ex)
			{
				logNet?.WriteException(ToString(), "SerialPort_DataReceived", ex);
				return;
			}
			break;
		}
		while (num < 3);
		if (list.Count != 0)
		{
			byte[] array3 = list.ToArray();
			LogNet?.WriteDebug(ToString(), StringResources.Language.Receive + " : " + array3.ToHexString(' '));
			ToledoStandardData toledoStandardData = null;
			try
			{
				toledoStandardData = new ToledoStandardData(array3);
			}
			catch (Exception ex2)
			{
				logNet?.WriteException(ToString(), "ToledoStandardData new failed: " + array3.ToHexString(' '), ex2);
			}
			if (toledoStandardData != null)
			{
				OnToledoStandardDataReceived?.Invoke(this, toledoStandardData);
			}
		}
	}

	/// <summary>
	/// 初始化串口信息，9600波特率，8位数据位，1位停止位，无奇偶校验
	/// Initial serial port information, 9600 baud rate, 8 data bits, 1 stop bit, no parity
	/// </summary>
	/// <param name="portName">端口号信息，例如&quot;COM3&quot;</param>
	public void SerialPortInni(string portName)
	{
		SerialPortInni(portName, 9600);
	}

	/// <summary>
	/// 初始化串口信息，波特率，8位数据位，1位停止位，无奇偶校验
	/// Initializes serial port information, baud rate, 8-bit data bit, 1-bit stop bit, no parity
	/// </summary>
	/// <param name="portName">端口号信息，例如&quot;COM3&quot;</param>
	/// <param name="baudRate">波特率</param>
	public void SerialPortInni(string portName, int baudRate)
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
			PortName = serialPort.PortName;
			BaudRate = serialPort.BaudRate;
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
			serialPort.PortName = "COM5";
			serialPort.BaudRate = 9600;
			serialPort.DataBits = 8;
			serialPort.StopBits = (StopBits)1;
			serialPort.Parity = (Parity)0;
			initi(serialPort);
			PortName = serialPort.PortName;
			BaudRate = serialPort.BaudRate;
		}
	}

	/// <summary>
	/// 打开一个新的串行端口连接
	/// Open a new serial port connection
	/// </summary>
	public void Open()
	{
		if (!serialPort.IsOpen)
		{
			serialPort.Open();
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
	public void Close()
	{
		if (serialPort.IsOpen)
		{
			serialPort.Close();
		}
	}

	public override string ToString()
	{
		return base.ToString();
	}
}
