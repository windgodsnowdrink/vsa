// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.IO;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HslCommunication.Core.IMessage;
using HslCommunication.Reflection;

namespace HslCommunication.Core.Pipe;

/// <summary>
/// 基于MOXA公司提供的串口驱动实现的管道类，目前仅支持windows平台，需要当前目录下存在 PCOMM.DLL 组件
/// The pipe class based on the serial port driver provided by MOXA currently only supports the windows platform and requires the existence of the PCOMM.DLL component in the current directory
/// </summary>
public class PipeMoxa : CommunicationPipe, IDisposable
{
	private int portNumber = -1;

	private bool rts = false;

	private bool dtr = false;

	private bool isOpen = false;

	private int moxaBaudRate = 12;

	private int moxaDataBits = 3;

	private int moxaStopBits = 0;

	private int moxaParity = 0;

	private string moxaFormate = string.Empty;

	/// <summary>
	/// 获取或设置一个值，该值指示在串行通信中是否启用请求发送 (RTS) 信号。
	/// Gets or sets a value indicating whether the request sending (RTS) signal is enabled in serial communication.
	/// </summary>
	public bool RtsEnable
	{
		get
		{
			return rts;
		}
		set
		{
			rts = value;
			if (isOpen)
			{
				PCommHelper.sio_RTS(portNumber, value ? 1 : 0);
			}
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
			return dtr;
		}
		set
		{
			dtr = value;
			if (isOpen)
			{
				PCommHelper.sio_DTR(portNumber, value ? 1 : 0);
			}
		}
	}

	/// <summary>
	/// 从串口中至少接收的字节长度信息，默认为1个字节
	/// </summary>
	public int AtLeastReceiveLength { get; set; } = 1;

	[HslMqttApi(Description = "Get or set the number of consecutive empty data receptions, which is valid when data reception is completed, default is 1")]
	/// <summary>
	/// 获取或设置连续接收空的数据次数，在数据接收完成时有效，每个单位消耗的时间为。
	/// Obtain or set the number of consecutive times to receive empty data, which is valid when the data is received, and the time consumed by each unit is
	/// </summary>
	public int ReceiveEmptyDataCount { get; set; } = 1;

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
	public PipeMoxa()
	{
		base.SleepTime = 5;
		ReceiveEmptyDataCount = 4;
	}

	/// <summary>
	/// 根据串口的名称来实例化一个对象
	/// Instantiate a default object
	/// </summary>
	/// <remarks>
	/// portName 支持格式化的方式，例如输入 COM3-9600-8-N-1，COM5-19200-7-E-2，其中奇偶校验的字母可选，N:无校验，O：奇校验，E:偶校验，停止位可选 0, 1, 2, 1.5 四种选项
	/// </remarks>
	public PipeMoxa(string portName)
	{
		base.SleepTime = 5;
		ReceiveEmptyDataCount = 4;
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
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		if (portName.Contains("-"))
		{
			string portName2 = "COM1";
			int baudRate = 9600;
			int dataBits = 8;
			Parity parity = (Parity)0;
			StopBits stopBits = (StopBits)1;
			string[] array = portName.Split(new char[2] { '-', ';' }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length != 0)
			{
				int num = 0;
				if (!Regex.IsMatch(array[0], "^[0-9]+$"))
				{
					portName2 = array[0];
					num = 1;
				}
				if (num < array.Length)
				{
					baudRate = Convert.ToInt32(array[num++]);
				}
				if (num < array.Length)
				{
					dataBits = Convert.ToInt32(array[num++]);
				}
				if (num < array.Length)
				{
					parity = (Parity)(array[num++].ToUpper() switch
					{
						"E" => 2, 
						"O" => 1, 
						"N" => 0, 
						_ => 4, 
					});
				}
				if (num < array.Length)
				{
					stopBits = (StopBits)(array[num++] switch
					{
						"0" => 0, 
						"2" => 2, 
						"1" => 1, 
						_ => 3, 
					});
				}
				SerialPortInni(portName2, baudRate, dataBits, stopBits, parity);
			}
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
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Invalid comparison between Unknown and I4
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Expected I4, but got Unknown
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Invalid comparison between Unknown and I4
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		if (portName.StartsWith("COM", StringComparison.OrdinalIgnoreCase))
		{
			portNumber = Convert.ToInt32(portName.Substring(3));
		}
		else
		{
			portNumber = Convert.ToInt32(portName);
		}
		switch (baudRate)
		{
		case 50:
			moxaBaudRate = 0;
			break;
		case 75:
			moxaBaudRate = 1;
			break;
		case 110:
			moxaBaudRate = 2;
			break;
		case 134:
			moxaBaudRate = 3;
			break;
		case 150:
			moxaBaudRate = 4;
			break;
		case 300:
			moxaBaudRate = 5;
			break;
		case 600:
			moxaBaudRate = 6;
			break;
		case 1200:
			moxaBaudRate = 7;
			break;
		case 1800:
			moxaBaudRate = 8;
			break;
		case 2400:
			moxaBaudRate = 9;
			break;
		case 4800:
			moxaBaudRate = 10;
			break;
		case 7200:
			moxaBaudRate = 11;
			break;
		case 9600:
			moxaBaudRate = 12;
			break;
		case 19200:
			moxaBaudRate = 13;
			break;
		case 38400:
			moxaBaudRate = 14;
			break;
		case 57600:
			moxaBaudRate = 15;
			break;
		case 115200:
			moxaBaudRate = 16;
			break;
		case 230400:
			moxaBaudRate = 17;
			break;
		case 460800:
			moxaBaudRate = 18;
			break;
		case 921600:
			moxaBaudRate = 19;
			break;
		}
		switch (dataBits)
		{
		case 5:
			moxaDataBits = 0;
			break;
		case 6:
			moxaDataBits = 1;
			break;
		case 7:
			moxaDataBits = 2;
			break;
		case 8:
			moxaDataBits = 3;
			break;
		}
		if ((int)stopBits != 1)
		{
			if ((int)stopBits == 2)
			{
				moxaStopBits = 4;
			}
		}
		else
		{
			moxaStopBits = 0;
		}
		switch ((int)parity)
		{
		case 0:
			moxaParity = 0;
			break;
		case 2:
			moxaParity = 24;
			break;
		case 1:
			moxaParity = 8;
			break;
		case 3:
			moxaParity = 40;
			break;
		case 4:
			moxaParity = 48;
			break;
		}
		moxaFormate = HslHelper.ToFormatString(portName, baudRate, dataBits, parity, stopBits);
	}

	/// <summary>
	/// 获取一个值，指示串口是否处于打开状态
	/// Gets a value indicating whether the serial port is open
	/// </summary>
	/// <returns>是或否</returns>
	public bool IsOpen()
	{
		return isOpen;
	}

	/// <summary>
	/// 清除串口缓冲区的数据，并返回该数据，如果缓冲区没有数据，返回的字节数组长度为0
	/// The number sent clears the data in the serial port buffer and returns that data, or if there is no data in the buffer, the length of the byte array returned is 0
	/// </summary>
	/// <returns>是否操作成功的方法</returns>
	public OperateResult<byte[]> ClearSerialCache()
	{
		return SPReceived(null, null, awaitData: false);
	}

	private string GetErrorText(int ret)
	{
		return ret switch
		{
			0 => "OK", 
			-1 => "Port number is invalid.", 
			-2 => "The board is not the MOXA compatible intelligent board", 
			-4 => "No data to read.", 
			-5 => "No such port or port is occupied by other program.", 
			-6 => "Can't control the port because it is set as auto H/W flow control by sio_flowctrl.", 
			-7 => "Bad parameter.", 
			-8 => "Calling Win32 function failed Call GetLastError to get the error code.", 
			-9 => "The com port does not support this function.", 
			-11 => "User abort blocked write.", 
			-12 => "Write timeout.", 
			_ => "Unkown", 
		};
	}

	public override OperateResult<bool> OpenCommunication()
	{
		try
		{
			if (!isOpen)
			{
				int num = PCommHelper.sio_open(portNumber);
				if (num == 0)
				{
					int mode = moxaDataBits | moxaParity | moxaStopBits;
					PCommHelper.sio_ioctl(portNumber, moxaBaudRate, mode);
					if (RtsEnable)
					{
						PCommHelper.sio_RTS(portNumber, 1);
					}
					if (DtrEnable)
					{
						PCommHelper.sio_DTR(portNumber, 1);
					}
					ResetConnectErrorCount();
					isOpen = true;
					return OperateResult.CreateSuccessResult(value: true);
				}
				return new OperateResult<bool>(num, GetErrorText(num));
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
		if (isOpen)
		{
			try
			{
				PCommHelper.sio_close(portNumber);
				isOpen = false;
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
				int num = 0;
				if (offset == 0)
				{
					num = PCommHelper.sio_write(portNumber, data, data.Length);
				}
				else
				{
					byte[] array = data.SelectMiddle(offset, size);
					num = PCommHelper.sio_write(portNumber, array, array.Length);
				}
				if (num >= 0)
				{
					return OperateResult.CreateSuccessResult();
				}
				return new OperateResult(num, GetErrorText(num));
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
				int num = PCommHelper.sio_read(portNumber, ref buffer[offset], length);
				if (num >= 0)
				{
					return OperateResult.CreateSuccessResult(num);
				}
				return new OperateResult<int>(num, GetErrorText(num));
			}
			int num2 = PCommHelper.sio_read(portNumber, ref buffer[offset], buffer.Length - offset);
			if (num2 >= 0)
			{
				return OperateResult.CreateSuccessResult(num2);
			}
			return new OperateResult<int>(num2, GetErrorText(num2));
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
	/// <param name="netMessage">定义的消息体对象</param>
	/// <param name="sendValue">等待发送的数据对象</param>
	/// <param name="awaitData">是否必须要等待数据返回</param>
	/// <param name="logMessage">用于消息记录的日志信息</param>
	/// <returns>结果数据对象</returns>
	private OperateResult<byte[]> SPReceived(INetMessage netMessage, byte[] sendValue, bool awaitData, Action<byte[]> logMessage = null)
	{
		if (!Authorization.nzugaydgwadawdibbas())
		{
			return new OperateResult<byte[]>(StringResources.Language.AuthorizationFailed);
		}
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
		int num = 0;
		int num2 = 0;
		while (true)
		{
			num2++;
			if (num2 > 1 && base.SleepTime >= 0)
			{
				HslHelper.ThreadSleep(base.SleepTime);
			}
			try
			{
				if (PCommHelper.sio_iqueue(portNumber) < 1)
				{
					if (num2 == 1)
					{
						continue;
					}
					if ((DateTime.Now - now).TotalMilliseconds > (double)base.ReceiveTimeOut)
					{
						return new OperateResult<byte[]>(-IncrConnectErrorCount(), $"Time out: {base.ReceiveTimeOut}, received: {memoryStream.ToArray().ToHexString(' ')}");
					}
					if (memoryStream.Length >= AtLeastReceiveLength)
					{
						num++;
						if (netMessage != null || num < ReceiveEmptyDataCount)
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
				num = 0;
				int count = PCommHelper.sio_read(portNumber, ref array[0], array.Length);
				if (count < 0)
				{
					return new OperateResult<byte[]>(count, GetErrorText(count));
				}
				if (count > 0)
				{
					memoryStream.Write(array, 0, count);
					logMessage?.Invoke(array.SelectBegin(count));
				}
				if (netMessage != null && CheckMessageComplete(netMessage, sendValue, ref memoryStream))
				{
					break;
				}
				if (base.ReceiveTimeOut > 0 && (DateTime.Now - now).TotalMilliseconds > (double)base.ReceiveTimeOut)
				{
					return new OperateResult<byte[]>(-IncrConnectErrorCount(), $"Time out: {base.ReceiveTimeOut}, received: {memoryStream.ToArray().ToHexString(' ')}");
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
		return SPReceived(netMessage, sendValue, awaitData: true, logMessage);
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
		return await Task.Run(() => SPReceived(netMessage, sendValue, awaitData: true, logMessage)).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(INetMessage netMessage, byte[] sendValue, bool hasResponseData, Action<byte[]> logMessage = null)
	{
		return await Task.Run(() => ReadFromCoreServer(netMessage, sendValue, hasResponseData, logMessage)).ConfigureAwait(continueOnCapturedContext: false);
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		CloseCommunication();
	}

	/// <summary>
	/// 获取当前串口参数的信息，以格式化的字符串返回
	/// Obtain the information of the current serial port parameters and return them as formatted strings
	/// </summary>
	/// <returns>串口参数</returns>
	public string ToFormatString()
	{
		return moxaFormate;
	}

	public override string ToString()
	{
		return $"PipeMoxa[COM{portNumber}]";
	}
}
