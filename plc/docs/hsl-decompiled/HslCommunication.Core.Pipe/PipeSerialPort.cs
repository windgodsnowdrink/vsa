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
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using HslCommunication.Core.IMessage;
using HslCommunication.Reflection;

namespace HslCommunication.Core.Pipe;

/// <summary>
/// 串口管道信息
/// </summary>
public class PipeSerialPort : CommunicationPipe, IDisposable
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
	/// 获取或设置一个值，该值指示在串行通信中是否启用数据终端就绪 (Drt) 信号。
	/// Gets or sets a value that indicates whether the Data Terminal Ready (DRT) signal is enabled in serial communication.
	/// </summary>
	public bool DtrEnable
	{
		get
		{
			return serialPort.DtrEnable;
		}
		set
		{
			serialPort.DtrEnable = value;
		}
	}

	/// <summary>
	/// 从串口中至少接收的字节长度信息，默认为1个字节
	/// </summary>
	public int AtLeastReceiveLength { get; set; }

	[HslMqttApi(Description = "Get or set the number of consecutive empty data receptions, which is valid when data reception is completed, default is 1")]
	/// <summary>
	/// 获取或设置连续接收空的数据次数，在数据接收完成时有效，每个单位消耗的时间为。
	/// Obtain or set the number of consecutive times to receive empty data, which is valid when the data is received, and the time consumed by each unit is
	/// </summary>
	public int ReceiveEmptyDataCount { get; set; }

	[HslMqttApi(Description = "Whether to empty the buffer before sending data, the default is false")]
	/// <summary>
	/// 是否在发送数据前清空缓冲数据，默认是false
	/// Whether to empty the buffer before sending data, the default is false
	/// </summary>
	public bool IsClearCacheBeforeRead { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public PipeSerialPort()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		AtLeastReceiveLength = 1;
		ReceiveEmptyDataCount = 1;
		serialPort = new SerialPort();
		base.SleepTime = 20;
	}

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	/// <param name="portName">portName 支持格式化的方式，例如输入 COM3-9600-8-N-1，COM5-19200-7-E-2，其中奇偶校验的字母可选，N:无校验，O：奇校验，E:偶校验，停止位可选 0, 1, 2, 1.5 四种选项</param>
	public PipeSerialPort(string portName)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		AtLeastReceiveLength = 1;
		ReceiveEmptyDataCount = 1;
		serialPort = new SerialPort();
		base.SleepTime = 20;
		SerialPortInni(portName);
	}

	/// <summary>
	/// 初始化串口信息，9600波特率，8位数据位，1位停止位，无奇偶校验
	/// Initial serial port information, 9600 baud rate, 8 data bits, 1 stop bit, no parity
	/// </summary>
	/// <param name="portName">端口号信息，例如&quot;COM3&quot;</param>
	/// <remarks>
	/// portName 支持格式化的方式，例如输入 COM3-9600-8-N-1，COM5-19200-7-E-2，其中奇偶校验的字母可选，N:无校验，O：奇校验，E:偶校验，停止位可选 0, 1, 2, 1.5 四种选项
	/// </remarks>
	public void SerialPortInni(string portName)
	{
		if (portName.Contains("-") || portName.Contains(";"))
		{
			SerialPortInni(delegate(SerialPort sp)
			{
				sp.IniSerialByFormatString(portName);
			});
		}
		else
		{
			SerialPortInni(portName, 9600, 8, (StopBits)1, (Parity)0);
		}
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
	/// 获取一个值，指示串口是否处于打开状态
	/// Gets a value indicating whether the serial port is open
	/// </summary>
	/// <returns>是或否</returns>
	public bool IsOpen()
	{
		return serialPort.IsOpen;
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

	/// <summary>
	/// 清除串口缓冲区的数据，并返回该数据，如果缓冲区没有数据，返回的字节数组长度为0
	/// The number sent clears the data in the serial port buffer and returns that data, or if there is no data in the buffer, the length of the byte array returned is 0
	/// </summary>
	/// <returns>是否操作成功的方法</returns>
	public OperateResult<byte[]> ClearSerialCache()
	{
		return SPReceived(serialPort, null, null, awaitData: false);
	}

	public override OperateResult<bool> OpenCommunication()
	{
		try
		{
			if (!serialPort.IsOpen)
			{
				serialPort.Open();
				ResetConnectErrorCount();
				return OperateResult.CreateSuccessResult(value: true);
			}
			return OperateResult.CreateSuccessResult(value: false);
		}
		catch (Exception ex)
		{
			return new OperateResult<bool>("OpenCommunication failed: " + ex.Message);
		}
	}

	public override OperateResult CloseCommunication()
	{
		if (serialPort.IsOpen)
		{
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

	public override OperateResult Send(byte[] data, int offset, int size)
	{
		if (data != null && data.Length != 0)
		{
			if (!Authorization.nzugaydgwadawdibbas())
			{
				return new OperateResult<byte[]>(StringResources.Language.AuthorizationFailed);
			}
			try
			{
				serialPort.Write(data, offset, size);
				return OperateResult.CreateSuccessResult();
			}
			catch (Exception ex)
			{
				return new OperateResult(-IncrConnectErrorCount(), ex.Message);
			}
		}
		return OperateResult.CreateSuccessResult();
	}

	public override OperateResult<int> Receive(byte[] buffer, int offset, int length, int timeOut = 60000, Action<long, long> reportProgress = null)
	{
		if (!Authorization.nzugaydgwadawdibbas())
		{
			return new OperateResult<int>(StringResources.Language.AuthorizationFailed);
		}
		try
		{
			if (length > 0)
			{
				int value = serialPort.Read(buffer, offset, length);
				return OperateResult.CreateSuccessResult(value);
			}
			int value2 = serialPort.Read(buffer, offset, buffer.Length - offset);
			return OperateResult.CreateSuccessResult(value2);
		}
		catch (Exception ex)
		{
			return new OperateResult<int>(-IncrConnectErrorCount(), ex.Message);
		}
	}

	/// <summary>
	/// 从串口接收一串字节数据信息，直到没有数据为止，如果参数awaitData为false, 第一轮接收没有数据则返回
	/// Receives a string of bytes of data information from the serial port until there is no data, and returns if the parameter awaitData is false
	/// </summary>
	/// <param name="serialPort">串口对象</param>
	/// <param name="netMessage">定义的消息体对象</param>
	/// <param name="sendValue">等待发送的数据对象</param>
	/// <param name="awaitData">是否必须要等待数据返回</param>
	/// <param name="logMessage">用于消息记录的日志信息</param>
	/// <returns>结果数据对象</returns>
	private OperateResult<byte[]> SPReceived(SerialPort serialPort, INetMessage netMessage, byte[] sendValue, bool awaitData, Action<byte[]> logMessage = null)
	{
		if (!Authorization.nzugaydgwadawdibbas())
		{
			return new OperateResult<byte[]>(StringResources.Language.AuthorizationFailed);
		}
		int num = base.ReceiveTimeOut;
		byte[] array = null;
		MemoryStream memoryStream = null;
		try
		{
			array = new byte[1024];
			memoryStream = new MemoryStream();
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
		DateTime now = DateTime.Now;
		int num2 = 0;
		int num3 = 0;
		int num4 = base.SleepTime;
		while (true)
		{
			num3++;
			if (num3 > 1 && num4 >= 0)
			{
				if (num3 < 7)
				{
					HslHelper.ThreadSleep((num4 < 20) ? 1 : (num4 / 10));
				}
				else
				{
					HslHelper.ThreadSleep(num4);
				}
			}
			try
			{
				if (serialPort.BytesToRead < 1)
				{
					if (num3 == 1)
					{
						continue;
					}
					if (num > 0 && (DateTime.Now - now).TotalMilliseconds > (double)num)
					{
						return new OperateResult<byte[]>(-IncrConnectErrorCount(), $"Time out: {num}, received: {memoryStream.ToArray().ToHexString(' ')}");
					}
					if (memoryStream.Length >= AtLeastReceiveLength)
					{
						num2++;
						if (netMessage != null || num2 < ReceiveEmptyDataCount)
						{
							continue;
						}
					}
					else if (awaitData)
					{
						continue;
					}
					break;
				}
				num2 = 0;
				if (num > 0)
				{
					serialPort.ReadTimeout = num;
				}
				int bytesRead = serialPort.Read(array, 0, Math.Min(array.Length, serialPort.BytesToRead));
				if (bytesRead > 0)
				{
					memoryStream.Write(array, 0, bytesRead);
					logMessage?.Invoke(array.SelectBegin(bytesRead));
				}
				if (netMessage != null && CheckMessageComplete(netMessage, sendValue, ref memoryStream))
				{
					break;
				}
				if (num > 0 && (DateTime.Now - now).TotalMilliseconds > (double)num)
				{
					return new OperateResult<byte[]>(-IncrConnectErrorCount(), $"Time out: {num}, received: {memoryStream.ToArray().ToHexString(' ')}");
				}
				continue;
			}
			catch (Exception ex2)
			{
				return new OperateResult<byte[]>(-IncrConnectErrorCount(), ex2.Message);
			}
		}
		ResetConnectErrorCount();
		return OperateResult.CreateSuccessResult(memoryStream.ToArray());
	}

	public override OperateResult<byte[]> ReceiveMessage(INetMessage netMessage, byte[] sendValue, bool useActivePush = true, Action<long, long> reportProgress = null, Action<byte[]> logMessage = null)
	{
		if (base.UseServerActivePush)
		{
			return base.ReceiveMessage(netMessage, sendValue, useActivePush, reportProgress);
		}
		return SPReceived(serialPort, netMessage, sendValue, awaitData: true, logMessage);
	}

	public override OperateResult<byte[]> ReadFromCoreServer(INetMessage netMessage, byte[] sendValue, bool hasResponseData, Action<byte[]> logMessage = null)
	{
		if (IsClearCacheBeforeRead)
		{
			ClearSerialCache();
		}
		OperateResult<byte[]> operateResult = ReadFromCoreServerHelper(netMessage, sendValue, hasResponseData, 0, logMessage);
		if (operateResult.IsSuccess)
		{
			ResetConnectErrorCount();
		}
		return operateResult;
	}

	public override async Task<OperateResult<byte[]>> ReceiveMessageAsync(INetMessage netMessage, byte[] sendValue, bool useActivePush = true, Action<long, long> reportProgress = null, Action<byte[]> logMessage = null)
	{
		return await Task.Run(() => SPReceived(serialPort, netMessage, sendValue, awaitData: true, logMessage)).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(INetMessage netMessage, byte[] sendValue, bool hasResponseData, Action<byte[]> logMessage = null)
	{
		return await Task.Run(() => ReadFromCoreServer(netMessage, sendValue, hasResponseData, logMessage)).ConfigureAwait(continueOnCapturedContext: false);
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		((Component)(object)serialPort)?.Dispose();
	}

	public override string ToString()
	{
		return "PipeSerialPort[" + serialPort.ToFormatString() + "]";
	}
}
