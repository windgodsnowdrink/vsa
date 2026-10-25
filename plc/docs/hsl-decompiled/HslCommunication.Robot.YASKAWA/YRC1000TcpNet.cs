// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Reflection;
using HslCommunication.Robot.YASKAWA.Helper;

namespace HslCommunication.Robot.YASKAWA;

/// <summary>
/// 安川机器人的Ethernet 服务器功能对应的客户端通讯类
/// Yaskawa robot's Ethernet server features a communication class
/// </summary>
/// <remarks>
/// 要想成功的通信，有两个至关重要的前提。
/// 1. 开启以太网服务器，[系统]-[设定]-[选项功能]-[网络功能设定]启用网络功能。
/// 2. 开启远程的命令，[输入输出]-[模拟输入]-[远程命令选择] 激活远程命令
/// </remarks>
public class YRC1000TcpNet : NetworkDoubleBase, IRobotNet
{
	/// <summary>
	/// 获取或设置当前的机器人类型，默认为
	/// Get or set the current robot type, the default is
	/// </summary>
	public YRCType Type { get; set; } = YRCType.YRC1000;

	/// <summary>
	/// 指定机器人的ip地址及端口号来实例化对象
	/// Specify the robot's IP address and port number to instantiate the object
	/// </summary>
	/// <param name="ipAddress">Ip地址</param>
	/// <param name="port">端口号</param>
	public YRC1000TcpNet(string ipAddress, int port)
	{
		IpAddress = ipAddress;
		Port = port;
		base.ByteTransform = new RegularByteTransform(DataFormat.CDAB);
		LogMsgFormatBinary = false;
	}

	[HslMqttApi(ApiTopic = "ReadRobotByte", Description = "Read the robot's original byte data information according to the address")]
	/// <summary>
	/// 根据地址读取机器人的原始的字节数据信息
	/// Read the robot's original byte data information according to the address
	/// </summary>
	/// <param name="address">指定的地址信息，对于某些机器人无效</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public OperateResult<byte[]> Read(string address)
	{
		OperateResult<string> operateResult = ReadString(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(Encoding.ASCII.GetBytes(operateResult.Content));
	}

	[HslMqttApi(ApiTopic = "ReadRobotString", Description = "Read the string data information of the robot based on the address")]
	/// <summary>
	/// 根据地址读取机器人的字符串的数据信息
	/// Read the string data information of the robot based on the address
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>带有成功标识的字符串数据</returns>
	public OperateResult<string> ReadString(string address)
	{
		if (address.IndexOf('.') >= 0 || address.IndexOf(':') >= 0 || address.IndexOf(';') >= 0)
		{
			string[] array = address.Split('.', ':', ';');
			return ReadByCommand(array[0], array[1]);
		}
		return ReadByCommand(address, null);
	}

	[HslMqttApi(ApiTopic = "WriteRobotByte", Description = "According to the address, to write the device related bytes data")]
	/// <summary>
	/// 根据地址，来写入设备的相关的字节数据
	/// According to the address, to write the device related bytes data
	/// </summary>
	/// <param name="address">指定的地址信息，有些机器人可能不支持</param>
	/// <param name="value">原始的字节数据信息</param>
	/// <returns>是否成功的写入</returns>
	public OperateResult Write(string address, byte[] value)
	{
		return Write(address, Encoding.ASCII.GetString(value));
	}

	[HslMqttApi(ApiTopic = "WriteRobotString", Description = "According to the address, to write the device related string data")]
	/// <summary>
	/// 根据地址，来写入设备相关的字符串数据
	/// According to the address, to write the device related string data
	/// </summary>
	/// <param name="address">指定的地址信息，有些机器人可能不支持</param>
	/// <param name="value">字符串的数据信息</param>
	/// <returns>是否成功的写入</returns>
	public OperateResult Write(string address, string value)
	{
		return ReadByCommand(address, value);
	}

	/// <summary>
	/// 根据地址读取机器人的原始的字节数据信息
	/// Read the robot's original byte data information according to the address
	/// </summary>
	/// <param name="address">指定的地址信息，对于某些机器人无效</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public async Task<OperateResult<byte[]>> ReadAsync(string address)
	{
		OperateResult<string> read = await ReadStringAsync(address);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return OperateResult.CreateSuccessResult(Encoding.ASCII.GetBytes(read.Content));
	}

	/// <summary>
	/// 根据地址读取机器人的字符串的数据信息
	/// Read the string data information of the robot based on the address
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>带有成功标识的字符串数据</returns>
	public async Task<OperateResult<string>> ReadStringAsync(string address)
	{
		if (address.IndexOf('.') >= 0 || address.IndexOf(':') >= 0 || address.IndexOf(';') >= 0)
		{
			string[] commands = address.Split('.', ':', ';');
			return await ReadByCommandAsync(commands[0], commands[1]);
		}
		return await ReadByCommandAsync(address, null);
	}

	/// <summary>
	/// 根据地址，来写入设备的相关的字节数据
	/// According to the address, to write the device related bytes data
	/// </summary>
	/// <param name="address">指定的地址信息，有些机器人可能不支持</param>
	/// <param name="value">原始的字节数据信息</param>
	/// <returns>是否成功的写入</returns>
	public async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await WriteAsync(address, Encoding.ASCII.GetString(value));
	}

	/// <summary>
	/// 根据地址，来写入设备相关的字符串数据
	/// According to the address, to write the device related string data
	/// </summary>
	/// <param name="address">指定的地址信息，有些机器人可能不支持</param>
	/// <param name="value">字符串的数据信息</param>
	/// <returns>是否成功的写入</returns>
	public async Task<OperateResult> WriteAsync(string address, string value)
	{
		return await ReadByCommandAsync(address, value);
	}

	/// <summary>
	/// before read data , the connection should be Initialized
	/// </summary>
	/// <param name="socket">connected socket</param>
	/// <returns>whether is the Initialization is success.</returns>
	protected override OperateResult InitializationOnConnect(Socket socket)
	{
		OperateResult<string> operateResult = ReadFromCoreServer(socket, "CONNECT Robot_access KeepAlive:-1\r\n");
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		if (operateResult.Content == "OK:YR Information Server(Ver) Keep-Alive:-1.\r\n")
		{
			return OperateResult.CreateSuccessResult();
		}
		if (!operateResult.Content.StartsWith("OK:"))
		{
			return new OperateResult(operateResult.Content);
		}
		isPersistentConn = false;
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// before read data , the connection should be Initialized
	/// </summary>
	/// <param name="socket">connected socket</param>
	/// <returns>whether is the Initialization is success.</returns>
	protected override async Task<OperateResult> InitializationOnConnectAsync(Socket socket)
	{
		OperateResult<string> read = await ReadFromCoreServerAsync(socket, "CONNECT Robot_access KeepAlive:-1\r\n");
		if (!read.IsSuccess)
		{
			return read;
		}
		if (read.Content == "OK:YR Information Server(Ver) Keep-Alive:-1.\r\n")
		{
			return OperateResult.CreateSuccessResult();
		}
		if (!read.Content.StartsWith("OK:"))
		{
			return new OperateResult(read.Content);
		}
		isPersistentConn = false;
		return OperateResult.CreateSuccessResult();
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new SpecifiedCharacterMessage(13, 10);
	}

	/// <summary>
	/// Read string value from socket
	/// </summary>
	/// <param name="socket">connected socket</param>
	/// <param name="send">string value</param>
	/// <returns>received string value with is successfully</returns>
	protected OperateResult<string> ReadFromCoreServer(Socket socket, string send)
	{
		OperateResult<byte[]> operateResult = ReadFromCoreServer(socket, Encoding.Default.GetBytes(send));
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		return OperateResult.CreateSuccessResult(Encoding.Default.GetString(operateResult.Content));
	}

	[HslMqttApi(Description = "Read the device information according to the instructions. If the command data is empty, pass in null. Note that all commands do not have a newline character")]
	/// <summary>
	/// 根据指令来读取设备的信息，如果命令数据为空，则传入null即可，注意，所有的命令不带换行符
	/// Read the device information according to the instructions. If the command data is empty, pass in null. Note that all commands do not have a newline character
	/// </summary>
	/// <param name="command">命令的内容</param>
	/// <param name="commandData">命令数据内容</param>
	/// <returns>最终的结果内容，需要对IsSuccess进行验证</returns>
	/// <remarks>
	/// 此处举几个例子
	/// "RALARM", NULL    错误报警代码读取。
	/// "RPOSJ", NULL     关节坐标系的坐标位置读取。
	/// "RJSEQ", NULL     读取当前的程序名，行编号，步编号。
	/// "SAVEV", "7,000" 读取变量数据。字符串变量
	/// </remarks>
	public OperateResult<string> ReadByCommand(string command, string commandData)
	{
		pipeSocket.PipeLockEnter();
		OperateResult<Socket> availableSocket = GetAvailableSocket();
		if (!availableSocket.IsSuccess)
		{
			pipeSocket.IsSocketError = true;
			base.AlienSession?.Offline();
			pipeSocket.PipeLockLeave();
			return OperateResult.CreateFailedResult<string>(availableSocket);
		}
		string send = (string.IsNullOrEmpty(commandData) ? ("HOSTCTRL_REQUEST " + command + " 0\r\n") : $"HOSTCTRL_REQUEST {command} {commandData.Length + 1}\r\n");
		OperateResult<string> operateResult = ReadFromCoreServer(availableSocket.Content, send);
		if (!operateResult.IsSuccess)
		{
			pipeSocket.IsSocketError = true;
			base.AlienSession?.Offline();
			pipeSocket.PipeLockLeave();
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		if (!operateResult.Content.StartsWith("OK:"))
		{
			if (!isPersistentConn)
			{
				availableSocket.Content?.Close();
			}
			pipeSocket.PipeLockLeave();
			return new OperateResult<string>(operateResult.Content.Remove(operateResult.Content.Length - 2));
		}
		if (!string.IsNullOrEmpty(commandData))
		{
			byte[] bytes = Encoding.ASCII.GetBytes(commandData + "\r");
			base.LogNet?.WriteDebug(ToString(), StringResources.Language.Send + " : " + SoftBasic.ByteToHexString(bytes, ' '));
			OperateResult operateResult2 = Send(availableSocket.Content, bytes);
			if (!operateResult2.IsSuccess)
			{
				availableSocket.Content?.Close();
				pipeSocket.IsSocketError = true;
				base.AlienSession?.Offline();
				pipeSocket.PipeLockLeave();
				return OperateResult.CreateFailedResult<string>(operateResult2);
			}
		}
		OperateResult<byte[]> operateResult3 = ReceiveCommandLineFromSocket(availableSocket.Content, 13, base.ReceiveTimeOut);
		if (!operateResult3.IsSuccess)
		{
			pipeSocket.IsSocketError = true;
			base.AlienSession?.Offline();
			pipeSocket.PipeLockLeave();
			return OperateResult.CreateFailedResult<string>(operateResult3);
		}
		string message = Encoding.ASCII.GetString(operateResult3.Content);
		if (string.IsNullOrEmpty(message))
		{
			if (!isPersistentConn)
			{
				availableSocket.Content?.Close();
			}
			pipeSocket.PipeLockLeave();
			return new OperateResult<string>("Return is Null");
		}
		if (message.StartsWith("ERROR:"))
		{
			if (!isPersistentConn)
			{
				availableSocket.Content?.Close();
			}
			pipeSocket.PipeLockLeave();
			Receive(availableSocket.Content, 1);
			return YRCHelper.ExtraErrorMessage(message);
		}
		if (message.StartsWith("0000\r"))
		{
			if (!isPersistentConn)
			{
				availableSocket.Content?.Close();
			}
			Receive(availableSocket.Content, 1);
			pipeSocket.PipeLockLeave();
			return OperateResult.CreateSuccessResult("0000");
		}
		if (!isPersistentConn)
		{
			availableSocket.Content?.Close();
		}
		pipeSocket.PipeLockLeave();
		return OperateResult.CreateSuccessResult(message.Remove(message.Length - 1));
	}

	/// <summary>
	/// Read string value from socket
	/// </summary>
	/// <param name="socket">connected socket</param>
	/// <param name="send">string value</param>
	/// <returns>received string value with is successfully</returns>
	protected async Task<OperateResult<string>> ReadFromCoreServerAsync(Socket socket, string send)
	{
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(socket, Encoding.Default.GetBytes(send));
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		return OperateResult.CreateSuccessResult(Encoding.Default.GetString(read.Content));
	}

	/// <summary>
	/// 根据指令来读取设备的信息，如果命令数据为空，则传入null即可，注意，所有的命令不带换行符
	/// Read the device information according to the instructions. If the command data is empty, pass in null. Note that all commands do not have a newline character
	/// </summary>
	/// <param name="command">命令的内容</param>
	/// <param name="commandData">命令数据内容</param>
	/// <returns>最终的结果内容，需要对IsSuccess进行验证</returns>
	/// <remarks>
	/// 此处举几个例子
	/// "RALARM", NULL    错误报警代码读取。
	/// "RPOSJ", NULL     关节坐标系的坐标位置读取。
	/// "RJSEQ", NULL     读取当前的程序名，行编号，步编号。
	/// "SAVEV", "7,000" 读取变量数据。字符串变量
	/// </remarks>
	public async Task<OperateResult<string>> ReadByCommandAsync(string command, string commandData)
	{
		await Task.Run(delegate
		{
			pipeSocket.PipeLockEnter();
		});
		OperateResult<Socket> resultSocket = await GetAvailableSocketAsync();
		if (!resultSocket.IsSuccess)
		{
			pipeSocket.IsSocketError = true;
			base.AlienSession?.Offline();
			pipeSocket.PipeLockLeave();
			return OperateResult.CreateFailedResult<string>(resultSocket);
		}
		OperateResult<string> readCommand = await ReadFromCoreServerAsync(send: string.IsNullOrEmpty(commandData) ? ("HOSTCTRL_REQUEST " + command + " 0\r\n") : $"HOSTCTRL_REQUEST {command} {commandData.Length + 1}\r\n", socket: resultSocket.Content);
		if (!readCommand.IsSuccess)
		{
			pipeSocket.IsSocketError = true;
			base.AlienSession?.Offline();
			pipeSocket.PipeLockLeave();
			return OperateResult.CreateFailedResult<string>(readCommand);
		}
		if (!readCommand.Content.StartsWith("OK:"))
		{
			if (!isPersistentConn)
			{
				resultSocket.Content?.Close();
			}
			pipeSocket.PipeLockLeave();
			return new OperateResult<string>(readCommand.Content.Remove(readCommand.Content.Length - 2));
		}
		if (!string.IsNullOrEmpty(commandData))
		{
			byte[] send2 = Encoding.ASCII.GetBytes(commandData + "\r");
			base.LogNet?.WriteDebug(ToString(), StringResources.Language.Send + " : " + SoftBasic.ByteToHexString(send2, ' '));
			OperateResult sendResult2 = await SendAsync(resultSocket.Content, send2);
			if (!sendResult2.IsSuccess)
			{
				resultSocket.Content?.Close();
				pipeSocket.IsSocketError = true;
				base.AlienSession?.Offline();
				pipeSocket.PipeLockLeave();
				return OperateResult.CreateFailedResult<string>(sendResult2);
			}
		}
		OperateResult<byte[]> resultReceive2 = await ReceiveCommandLineFromSocketAsync(resultSocket.Content, 13, base.ReceiveTimeOut);
		if (!resultReceive2.IsSuccess)
		{
			pipeSocket.IsSocketError = true;
			base.AlienSession?.Offline();
			pipeSocket.PipeLockLeave();
			return OperateResult.CreateFailedResult<string>(resultReceive2);
		}
		string commandDataReturn = Encoding.ASCII.GetString(resultReceive2.Content);
		if (string.IsNullOrEmpty(commandDataReturn))
		{
			if (!isPersistentConn)
			{
				resultSocket.Content?.Close();
			}
			pipeSocket.PipeLockLeave();
			return new OperateResult<string>("Return is Null");
		}
		if (commandDataReturn.StartsWith("ERROR:"))
		{
			if (!isPersistentConn)
			{
				resultSocket.Content?.Close();
			}
			pipeSocket.PipeLockLeave();
			await ReceiveAsync(resultSocket.Content, 1);
			return YRCHelper.ExtraErrorMessage(commandDataReturn);
		}
		if (commandDataReturn.StartsWith("0000\r"))
		{
			if (!isPersistentConn)
			{
				resultSocket.Content?.Close();
			}
			await ReceiveAsync(resultSocket.Content, 1);
			pipeSocket.PipeLockLeave();
			return OperateResult.CreateSuccessResult("0000");
		}
		if (!isPersistentConn)
		{
			resultSocket.Content?.Close();
		}
		pipeSocket.PipeLockLeave();
		return OperateResult.CreateSuccessResult(commandDataReturn.Remove(commandDataReturn.Length - 1));
	}

	[HslMqttApi(Description = "Read the alarm information of the robot")]
	/// <summary>
	/// 读取机器人的报警信息
	/// Read the alarm information of the robot
	/// </summary>
	/// <returns>原始的报警信息</returns>
	public OperateResult<string> ReadALARM()
	{
		return ReadByCommand("RALARM", null);
	}

	[HslMqttApi(Description = "Read the coordinate data information of the robot")]
	/// <summary>
	/// 关节坐标系的坐标位置读取。
	/// Read the coordinate data information of the robot
	/// </summary>
	/// <returns>原始的报警信息</returns>
	public OperateResult<string> ReadPOSJ()
	{
		return ReadByCommand("RPOSJ", null);
	}

	[HslMqttApi(Description = "指定坐标系的当前值读取。并且可以指定外部轴的有无。")]
	/// <summary>
	/// 指定坐标系的当前值读取。并且可以指定外部轴的有无。
	/// The current value of the specified coordinate system is read. And you can specify the presence or absence of an external axis.
	/// </summary>
	/// <param name="coordinate">指定读取坐标 0:基座坐标，1:机器人坐标，2-65分别表示用户坐标1-64</param>
	/// <param name="hasExteralAxis">外部轴的有/无</param>
	/// <returns>坐标系当前值</returns>
	public OperateResult<YRCRobotData> ReadPOSC(int coordinate, bool hasExteralAxis)
	{
		OperateResult<string> operateResult = ReadByCommand("RPOSC", string.Format("{0},{1}", coordinate, hasExteralAxis ? "1" : "0"));
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<YRCRobotData>(operateResult);
		}
		return OperateResult.CreateSuccessResult(new YRCRobotData(Type, operateResult.Content));
	}

	[HslMqttApi(Description = "模式状态，循环状态，动作状态，报警错误状态，伺服状态的读取。")]
	/// <summary>
	/// 模式状态，循环状态，动作状态，报警错误状态，伺服状态的读取。
	/// Reading of mode status, cycle status, action status, alarm error status, and servo status.
	/// </summary>
	/// <returns>状态信息</returns>
	/// <remarks>
	/// [0]: 单步
	/// [1]: 1循环
	/// [2]: 自动连续
	/// [3]: 运行中
	/// [4]: 运转中
	/// [5]: 示教
	/// [6]: 在线
	/// [7]: 命令模式
	/// [9]: 示教编程器HOLD中
	/// [10]: 外部HOLD中
	/// [11]: 命令HOLD中
	/// [12]: 发生警报
	/// [13]: 发生错误
	/// [14]: 伺服ON
	/// </remarks>
	public OperateResult<bool[]> ReadStats()
	{
		OperateResult<string> operateResult = ReadByCommand("RSTATS", null);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content.ToStringArray<byte>().ToBoolArray());
	}

	[HslMqttApi(Description = "读取当前的程序名，行编号，步编号。")]
	/// <summary>
	/// 读取当前的程序名，行编号，步编号。
	/// Read the current program name, line number, and step number.
	/// </summary>
	/// <returns>读取结果</returns>
	public OperateResult<string> ReadJSeq()
	{
		return ReadByCommand("RJSEQ", null);
	}

	[HslMqttApi(Description = "读取指定用户的坐标数据。")]
	/// <summary>
	/// 读取指定用户的坐标数据。
	/// Read the coordinate data of the specified user.
	/// </summary>
	/// <param name="frame">用户坐标编号，1-64</param>
	/// <returns>坐标值</returns>
	public OperateResult<string> ReadUFrame(int frame)
	{
		return ReadByCommand("RUFRAME", $"{frame}");
	}

	[HslMqttApi(Description = "读取机器人的字节型变量的数据，需要传入变量的编号。")]
	/// <summary>
	/// 读取机器人的字节型变量的数据，需要传入变量的编号
	/// To read the data of the byte variable of the robot, the number of the variable needs to be passed in
	/// </summary>
	/// <param name="variableAdderss">变量的编号</param>
	/// <returns>包含是否成功的结果数据</returns>
	public OperateResult<string> ReadByteVariable(string variableAdderss)
	{
		return ReadByCommand("SAVEV", "0," + variableAdderss);
	}

	[HslMqttApi(Description = "读取机器人的整型变量的数据，需要传入变量的编号")]
	/// <summary>
	/// 读取机器人的整型变量的数据，需要传入变量的编号
	/// To read the data of the integer variable of the robot, the number of the variable needs to be passed in
	/// </summary>
	/// <param name="variableAdderss">变量的编号</param>
	/// <returns>包含是否成功的结果数据</returns>
	public OperateResult<string> ReadIntegerVariable(string variableAdderss)
	{
		return ReadByCommand("SAVEV", "1," + variableAdderss);
	}

	[HslMqttApi(Description = "读取机器人的双精度整型变量的数据，需要传入变量的编号")]
	/// <summary>
	/// 读取机器人的双精度整型变量的数据，需要传入变量的编号
	/// To read the data of the double integer variable of the robot, the number of the variable needs to be passed in
	/// </summary>
	/// <param name="variableAdderss">变量的编号</param>
	/// <returns>包含是否成功的结果数据</returns>
	public OperateResult<string> ReadDoubleIntegerVariable(string variableAdderss)
	{
		return ReadByCommand("SAVEV", "2," + variableAdderss);
	}

	[HslMqttApi(Description = "读取机器人的实数变量的数据，需要传入变量的编号")]
	/// <summary>
	/// 读取机器人的实数变量的数据，需要传入变量的编号
	/// To read the data of the real variable of the robot, the number of the variable needs to be passed in
	/// </summary>
	/// <param name="variableAdderss">变量的编号</param>
	/// <returns>包含是否成功的结果数据</returns>
	public OperateResult<string> ReadRealVariable(string variableAdderss)
	{
		return ReadByCommand("SAVEV", "3," + variableAdderss);
	}

	[HslMqttApi(Description = "读取机器人的字符串变量的数据，需要传入变量的编号")]
	/// <summary>
	/// 读取机器人的字符串变量的数据，需要传入变量的编号
	/// To read the data of the string variable of the robot, the number of the variable needs to be passed in
	/// </summary>
	/// <param name="variableAdderss">变量的编号</param>
	/// <returns>包含是否成功的结果数据</returns>
	public OperateResult<string> ReadStringVariable(string variableAdderss)
	{
		return ReadByCommand("SAVEV", "7," + variableAdderss);
	}

	[HslMqttApi(Description = "进行HOLD 的 ON/OFF 操作，状态参数 False: OFF，True: ON")]
	/// <summary>
	/// 进行HOLD 的 ON/OFF 操作，状态参数 False: OFF操作，True: ON操作
	/// Perform HOLD ON operation, False: OFF，True: ON
	/// </summary>
	/// <param name="status">状态参数 False: OFF操作，True: ON操作</param>
	/// <returns>是否成功的HOLD操作</returns>
	public OperateResult Hold(bool status)
	{
		return ReadByCommand("HOLD", status ? "1" : "0");
	}

	[HslMqttApi(Description = "对机械手的报警进行复位")]
	/// <summary>
	/// 对机械手的报警进行复位
	/// Reset the alarm of the manipulator
	/// </summary>
	/// <returns>是否复位成功</returns>
	/// <remarks>
	/// 传输报警仅可在示教编程器上进行复位。
	/// </remarks>
	public OperateResult Reset()
	{
		return ReadByCommand("RESET", null);
	}

	[HslMqttApi(Description = "进行错误取消")]
	/// <summary>
	/// 进行错误取消
	/// Make an error cancellation
	/// </summary>
	/// <returns>是否取消成功</returns>
	public OperateResult Cancel()
	{
		return ReadByCommand("CANCEL", null);
	}

	[HslMqttApi(Description = "选择模式。模式编号为1:示教模式，2:再现模式")]
	/// <summary>
	/// 选择模式。模式编号为1:示教模式，2:再现模式
	/// Choose a mode. The mode number is 1: teaching mode, 2: reproduction mode
	/// </summary>
	/// <param name="number">模式编号为1:示教模式，2:再现模式</param>
	/// <returns>模式是否选择成功</returns>
	/// <remarks>
	/// MODE 命令，是在「操作条件」 画面中获得外部模式切换的许可后可以使用。
	/// </remarks>
	public OperateResult Mode(int number)
	{
		return ReadByCommand("MODE", number.ToString());
	}

	[HslMqttApi(Description = "选择循环。循环编号 1:步骤，2:1循环，3:连续自动")]
	/// <summary>
	/// 选择循环。循环编号 1:步骤，2:1循环，3:连续自动
	/// Choose loop. Cycle number 1: step, 2:1 cycle, 3: continuous automatic
	/// </summary>
	/// <param name="number">循环编号 1:步骤，2:1循环，3:连续自动</param>
	/// <returns>循环是否选择成功</returns>
	public OperateResult Cycle(int number)
	{
		return ReadByCommand("CYCLE", number.ToString());
	}

	[HslMqttApi(Description = "进行伺服电源的ON/OFF操作，状态参数 False: OFF，True: ON")]
	/// <summary>
	/// 进行伺服电源的ON/OFF操作，状态参数 False: OFF，True: ON
	/// Carry out the ON/OFF operation of the servo power, the status parameter False: OFF，True: ON
	/// </summary>
	/// <param name="status">状态参数 False: OFF，True: ON</param>
	/// <returns>是否伺服电源是否成功</returns>
	/// <remarks>
	/// 通过此命令伺服ON的时候，请连接机器人专用端子台（ MTX） 的外部伺服ON（ EXSVON）信号的29和 30 。
	/// </remarks>
	public OperateResult Svon(bool status)
	{
		return ReadByCommand("SVON", status ? "1" : "0");
	}

	[HslMqttApi(Description = "设定示教编程器和 I/O的操作信号的联锁。 状态参数 False: OFF，True: ON")]
	/// <summary>
	/// 设定示教编程器和 I/O的操作信号的联锁。 状态参数 False: OFF，True: ON
	/// Set the interlock between the programming pendant and the operation signal of I/O. Status parameter False: OFF，True: ON
	/// </summary>
	/// <param name="status">状态参数 False: OFF，True: ON</param>
	/// <returns>是否设定成功</returns>
	/// <remarks>
	/// 联锁为ON时，仅可执行以下操作。
	/// 示教编程器的非常停止
	/// Ｉ /O 的模式切换， 外部启动， 外部伺服ON，循环切换， I/O 禁止、 PP/PANEL 禁止、 主程序调出以外的输入信号
	/// 示教编程器在编辑中或者通过其他的功能访问文件时，不能使用HLOCK.
	/// </remarks>
	public OperateResult HLock(bool status)
	{
		return ReadByCommand("HLOCK", status ? "1" : "0");
	}

	[HslMqttApi(Description = "接受消息数据时， 在YRC1000的示教编程器的远程画面下显示消息若。若不是远程画面时，强制切换到远程画面。显示MDSP命令的消息。")]
	/// <summary>
	/// 接受消息数据时， 在YRC1000的示教编程器的远程画面下显示消息若。若不是远程画面时，强制切换到远程画面。显示MDSP命令的消息。
	/// When receiving message data, a message is displayed on the remote screen of the YRC1000 programming pendant.
	/// If it is not a remote screen, it is forced to switch to the remote screen. Display the message of the MDSP command.
	/// </summary>
	/// <param name="message">显示信息（最大 30byte 字符串）</param>
	/// <returns>是否显示成功</returns>
	public OperateResult MSDP(string message)
	{
		return ReadByCommand("MDSP", message);
	}

	[HslMqttApi(Description = "开始程序。操作时指定程序名时，此程序能附带对应主程序，则从该程序的开头开始执行。如果没有指定，则从前行开始执行")]
	/// <summary>
	/// 开始程序。操作时指定程序名时，此程序能附带对应主程序，则从该程序的开头开始执行。如果没有指定，则从前行开始执行
	/// Start the program. When the program name is specified during operation, the program can be accompanied by the corresponding main program,
	/// and the execution starts from the beginning of the program. If not specified, execute from the previous line
	/// </summary>
	/// <param name="programName">开始动作程序名称，可以省略</param>
	/// <returns>是否启动成功</returns>
	public OperateResult Start(string programName = null)
	{
		return ReadByCommand("START", programName);
	}

	[HslMqttApi(Description = "删除指定的程序。指定「*」 时， 删除当前登录的所有程序。指定「 删除程序名称」 时，仅删除指定的程序。")]
	/// <summary>
	/// 删除指定的程序。指定「*」 时， 删除当前登录的所有程序。指定「 删除程序名称」 时，仅删除指定的程序。
	/// Delete the specified program. When "*" is specified, all currently registered programs will be deleted.
	/// When "delete program name" is specified, only the specified program will be deleted.
	/// </summary>
	/// <param name="programName">删除的程序名称，如果设置为「*」时，删除当前登录的所有程序。</param>
	/// <returns>是否删除成功</returns>
	public OperateResult Delete(string programName = null)
	{
		return ReadByCommand("DELETE", programName);
	}

	[HslMqttApi(Description = "指定的程序设定为主程序。设定主程序的同时执行程序也被设定。")]
	/// <summary>
	/// 指定的程序设定为主程序。设定主程序的同时执行程序也被设定。
	/// The specified program is set as the main program. The execution program is also set when the main program is set.
	/// </summary>
	/// <param name="programName">设定的程序名称</param>
	/// <returns>是否设定成功</returns>
	public OperateResult SetMJ(string programName = null)
	{
		return ReadByCommand("SETMJ", programName);
	}

	[HslMqttApi(Description = "设定执行程序的名称和行编号。")]
	/// <summary>
	/// 设定执行程序的名称和行编号。
	/// Set the name and line number of the executed program.
	/// </summary>
	/// <param name="programName">设定程序名称</param>
	/// <param name="line">设定行编号（ 0 ～ 9999）</param>
	/// <returns>是否设定成功</returns>
	public OperateResult JSeq(string programName, int line)
	{
		return ReadByCommand("JSEQ", $"{programName},{line}");
	}

	/// <summary>
	/// 向指定的坐标系位置进行关节动作。其中没有外部轴的系统， 7-12外部轴的值设定为「0」
	/// Perform joint motions to the specified coordinate system position.
	/// where there is no external axis system, the value of 7-12 external axis is set to "0"
	/// </summary>
	/// <param name="robotData">机器的的数据信息</param>
	/// <returns>是否动作成功</returns>
	/// <remarks>
	/// 其中形态数据由6个bool数组组成，每个bool含义参考参数说明，0表示 False，1表示 True
	/// </remarks>
	public OperateResult MoveJ(YRCRobotData robotData)
	{
		return ReadByCommand("MOVJ", robotData.ToWriteString(Type));
	}

	[HslMqttApi(Description = "读取I/O 信号。 I/O 数据是每8个点输出，所以读出接点数是8的倍数。")]
	/// <summary>
	/// 读取I/O 信号。 I/O 数据是每8个点输出，所以读出接点数是8的倍数。
	/// Read I/O signal. I/O data is output every 8 points, so the number of read contacts is a multiple of 8.
	/// </summary>
	/// <param name="address">读出开始点编号</param>
	/// <param name="length">读出的接点数</param>
	/// <returns>读取的结果点位信息</returns>
	public OperateResult<bool[]> IORead(int address, int length)
	{
		OperateResult<string> operateResult = ReadByCommand("IOREAD", $"{address},{length}");
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		byte[] inBytes = operateResult.Content.ToStringArray<byte>();
		return OperateResult.CreateSuccessResult(inBytes.ToBoolArray());
	}

	[HslMqttApi(Description = "写入I/O信号状态，写入接点数请指定8的倍数。IO 信号的网络写入仅可是（ #27010 ～ #29567）。")]
	/// <summary>
	/// 写入I/O信号状态，写入接点数请指定8的倍数。IO 信号的网络写入仅可是（ #27010 ～ #29567）。
	/// To write I/O signal status, please specify a multiple of 8 for the number of write contacts.
	/// The network write of IO signal is only available (#27010 to #29567).
	/// </summary>
	/// <param name="address">写入开始接点编号</param>
	/// <param name="value">写入的bool值，写入接点数请指定8的倍数。</param>
	/// <returns>是否写入成功</returns>
	public OperateResult IOWrite(int address, bool[] value)
	{
		if (value == null || value.Length % 8 != 0)
		{
			return new OperateResult("Parameter [value] can't be null or length must be 8 *N");
		}
		byte[] array = value.ToByteArray();
		StringBuilder stringBuilder = new StringBuilder($"{address},{value.Length}");
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(",");
			stringBuilder.Append(array[i].ToString());
		}
		return ReadByCommand("IOWRITE", stringBuilder.ToString());
	}

	/// <summary>
	/// 读取机器人的报警信息
	/// Read the alarm information of the robot
	/// </summary>
	/// <returns>原始的报警信息</returns>
	public async Task<OperateResult<string>> ReadALARMAsync()
	{
		return await ReadByCommandAsync("RALARM", null);
	}

	/// <summary>
	/// 关节坐标系的坐标位置读取。
	/// Read the coordinate data information of the robot
	/// </summary>
	/// <returns>原始的报警信息</returns>
	public async Task<OperateResult<string>> ReadPOSJAsync()
	{
		return await ReadByCommandAsync("RPOSJ", null);
	}

	/// <summary>
	/// 指定坐标系的当前值读取。并且可以指定外部轴的有无。
	/// The current value of the specified coordinate system is read. And you can specify the presence or absence of an external axis.
	/// </summary>
	/// <param name="coordinate">指定读取坐标 0:基座坐标，1:机器人坐标，2-65分别表示用户坐标1-64</param>
	/// <param name="hasExteralAxis">外部轴的有/无</param>
	/// <returns>坐标系当前值</returns>
	public async Task<OperateResult<YRCRobotData>> ReadPOSCAsync(int coordinate, bool hasExteralAxis)
	{
		OperateResult<string> read = await ReadByCommandAsync("RPOSC", string.Format("{0},{1}", coordinate, hasExteralAxis ? "1" : "0"));
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<YRCRobotData>(read);
		}
		return OperateResult.CreateSuccessResult(new YRCRobotData(Type, read.Content));
	}

	/// <summary>
	/// 模式状态，循环状态，动作状态，报警错误状态，伺服状态的读取。
	/// Reading of mode status, cycle status, action status, alarm error status, and servo status.
	/// </summary>
	/// <returns>状态信息</returns>
	/// <remarks>
	/// [0]: 单步
	/// [1]: 1循环
	/// [2]: 自动连续
	/// [3]: 运行中
	/// [4]: 运转中
	/// [5]: 示教
	/// [6]: 在线
	/// [7]: 命令模式
	/// [9]: 示教编程器HOLD中
	/// [10]: 外部HOLD中
	/// [11]: 命令HOLD中
	/// [12]: 发生警报
	/// [13]: 发生错误
	/// [14]: 伺服ON
	/// </remarks>
	public async Task<OperateResult<bool[]>> ReadStatsAsync()
	{
		OperateResult<string> read = await ReadByCommandAsync("RSTATS", null);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(read);
		}
		return OperateResult.CreateSuccessResult(read.Content.ToStringArray<byte>().ToBoolArray());
	}

	/// <summary>
	/// 读取当前的程序名，行编号，步编号。
	/// Read the current program name, line number, and step number.
	/// </summary>
	/// <returns>读取结果</returns>
	public async Task<OperateResult<string>> ReadJSeqAsync()
	{
		return await ReadByCommandAsync("RJSEQ", null);
	}

	/// <summary>
	/// 读取指定用户的坐标数据。
	/// Read the coordinate data of the specified user.
	/// </summary>
	/// <param name="frame">用户坐标编号，1-64</param>
	/// <returns>坐标值</returns>
	public async Task<OperateResult<string>> ReadUFrameAsync(int frame)
	{
		return await ReadByCommandAsync("RUFRAME", $"{frame}");
	}

	/// <summary>
	/// 读取机器人的字节型变量的数据，需要传入变量的编号
	/// To read the data of the byte variable of the robot, the number of the variable needs to be passed in
	/// </summary>
	/// <param name="variableAdderss">变量的编号</param>
	/// <returns>包含是否成功的结果数据</returns>
	public async Task<OperateResult<string>> ReadByteVariableAsync(string variableAdderss)
	{
		return await ReadByCommandAsync("SAVEV", "0," + variableAdderss);
	}

	/// <summary>
	/// 读取机器人的整型变量的数据，需要传入变量的编号
	/// To read the data of the integer variable of the robot, the number of the variable needs to be passed in
	/// </summary>
	/// <param name="variableAdderss">变量的编号</param>
	/// <returns>包含是否成功的结果数据</returns>
	public async Task<OperateResult<string>> ReadIntegerVariableAsync(string variableAdderss)
	{
		return await ReadByCommandAsync("SAVEV", "1," + variableAdderss);
	}

	/// <summary>
	/// 读取机器人的双精度整型变量的数据，需要传入变量的编号
	/// To read the data of the double integer variable of the robot, the number of the variable needs to be passed in
	/// </summary>
	/// <param name="variableAdderss">变量的编号</param>
	/// <returns>包含是否成功的结果数据</returns>
	public async Task<OperateResult<string>> ReadDoubleIntegerVariableAsync(string variableAdderss)
	{
		return await ReadByCommandAsync("SAVEV", "2," + variableAdderss);
	}

	/// <summary>
	/// 读取机器人的实数变量的数据，需要传入变量的编号
	/// To read the data of the real variable of the robot, the number of the variable needs to be passed in
	/// </summary>
	/// <param name="variableAdderss">变量的编号</param>
	/// <returns>包含是否成功的结果数据</returns>
	public async Task<OperateResult<string>> ReadRealVariableAsync(string variableAdderss)
	{
		return await ReadByCommandAsync("SAVEV", "3," + variableAdderss);
	}

	/// <summary>
	/// 读取机器人的字符串变量的数据，需要传入变量的编号
	/// To read the data of the string variable of the robot, the number of the variable needs to be passed in
	/// </summary>
	/// <param name="variableAdderss">变量的编号</param>
	/// <returns>包含是否成功的结果数据</returns>
	public async Task<OperateResult<string>> ReadStringVariableAsync(string variableAdderss)
	{
		return await ReadByCommandAsync("SAVEV", "7," + variableAdderss);
	}

	/// <summary>
	/// 进行HOLD 的 ON/OFF 操作，状态参数 False: OFF操作，True: ON操作
	/// Perform HOLD ON operation, False: OFF，True: ON
	/// </summary>
	/// <param name="status">状态参数 False: OFF操作，True: ON操作</param>
	/// <returns>是否成功的HOLD操作</returns>
	public async Task<OperateResult> HoldAsync(bool status)
	{
		return await ReadByCommandAsync("HOLD", status ? "1" : "0");
	}

	/// <summary>
	/// 对机械手的报警进行复位
	/// Reset the alarm of the manipulator
	/// </summary>
	/// <returns>是否复位成功</returns>
	/// <remarks>
	/// 传输报警仅可在示教编程器上进行复位。
	/// </remarks>
	public async Task<OperateResult> ResetAsync()
	{
		return await ReadByCommandAsync("RESET", null);
	}

	/// <summary>
	/// 进行错误取消
	/// Make an error cancellation
	/// </summary>
	/// <returns>是否取消成功</returns>
	public async Task<OperateResult> CancelAsync()
	{
		return await ReadByCommandAsync("CANCEL", null);
	}

	/// <summary>
	/// 选择模式。模式编号为1:示教模式，2:再现模式
	/// Choose a mode. The mode number is 1: teaching mode, 2: reproduction mode
	/// </summary>
	/// <param name="number">模式编号为1:示教模式，2:再现模式</param>
	/// <returns>模式是否选择成功</returns>
	/// <remarks>
	/// MODE 命令，是在「操作条件」 画面中获得外部模式切换的许可后可以使用。
	/// </remarks>
	public async Task<OperateResult> ModeAsync(int number)
	{
		return await ReadByCommandAsync("MODE", number.ToString());
	}

	/// <summary>
	/// 选择循环。循环编号 1:步骤，2:1循环，3:连续自动
	/// Choose loop. Cycle number 1: step, 2:1 cycle, 3: continuous automatic
	/// </summary>
	/// <param name="number">循环编号 1:步骤，2:1循环，3:连续自动</param>
	/// <returns>循环是否选择成功</returns>
	public async Task<OperateResult> CycleAsync(int number)
	{
		return await ReadByCommandAsync("CYCLE", number.ToString());
	}

	/// <summary>
	/// 进行伺服电源的ON/OFF操作，状态参数 False: OFF，True: ON
	/// Carry out the ON/OFF operation of the servo power, the status parameter False: OFF，True: ON
	/// </summary>
	/// <param name="status">状态参数 False: OFF，True: ON</param>
	/// <returns>是否伺服电源是否成功</returns>
	/// <remarks>
	/// 通过此命令伺服ON的时候，请连接机器人专用端子台（ MTX） 的外部伺服ON（ EXSVON）信号的29和 30 。
	/// </remarks>
	public async Task<OperateResult> SvonAsync(bool status)
	{
		return await ReadByCommandAsync("SVON", status ? "1" : "0");
	}

	/// <summary>
	/// 设定示教编程器和 I/O的操作信号的联锁。 状态参数 False: OFF，True: ON
	/// Set the interlock between the programming pendant and the operation signal of I/O. Status parameter False: OFF，True: ON
	/// </summary>
	/// <param name="status">状态参数 False: OFF，True: ON</param>
	/// <returns>是否设定成功</returns>
	/// <remarks>
	/// 联锁为ON时，仅可执行以下操作。
	/// 示教编程器的非常停止
	/// Ｉ /O 的模式切换， 外部启动， 外部伺服ON，循环切换， I/O 禁止、 PP/PANEL 禁止、 主程序调出以外的输入信号
	/// 示教编程器在编辑中或者通过其他的功能访问文件时，不能使用HLOCK.
	/// </remarks>
	public async Task<OperateResult> HLockAsync(bool status)
	{
		return await ReadByCommandAsync("HLOCK", status ? "1" : "0");
	}

	/// <summary>
	/// 接受消息数据时， 在YRC1000的示教编程器的远程画面下显示消息若。若不是远程画面时，强制切换到远程画面。显示MDSP命令的消息。
	/// When receiving message data, a message is displayed on the remote screen of the YRC1000 programming pendant.
	/// If it is not a remote screen, it is forced to switch to the remote screen. Display the message of the MDSP command.
	/// </summary>
	/// <param name="message">显示信息（最大 30byte 字符串）</param>
	/// <returns>是否显示成功</returns>
	public async Task<OperateResult> MSDPAsync(string message)
	{
		return await ReadByCommandAsync("MDSP", message);
	}

	/// <summary>
	/// 开始程序。操作时指定程序名时，此程序能附带对应主程序，则从该程序的开头开始执行。如果没有指定，则从前行开始执行
	/// Start the program. When the program name is specified during operation, the program can be accompanied by the corresponding main program,
	/// and the execution starts from the beginning of the program. If not specified, execute from the previous line
	/// </summary>
	/// <param name="programName">开始动作程序名称，可以省略</param>
	/// <returns>是否启动成功</returns>
	public async Task<OperateResult> StartAsync(string programName = null)
	{
		return await ReadByCommandAsync("START", programName);
	}

	/// <summary>
	/// 删除指定的程序。指定「*」 时， 删除当前登录的所有程序。指定「 删除程序名称」 时，仅删除指定的程序。
	/// Delete the specified program. When "*" is specified, all currently registered programs will be deleted.
	/// When "delete program name" is specified, only the specified program will be deleted.
	/// </summary>
	/// <param name="programName">删除的程序名称，如果设置为「*」时，删除当前登录的所有程序。</param>
	/// <returns>是否删除成功</returns>
	public async Task<OperateResult> DeleteAsync(string programName = null)
	{
		return await ReadByCommandAsync("DELETE", programName);
	}

	/// <summary>
	/// 指定的程序设定为主程序。设定主程序的同时执行程序也被设定。
	/// The specified program is set as the main program. The execution program is also set when the main program is set.
	/// </summary>
	/// <param name="programName">设定的程序名称</param>
	/// <returns>是否设定成功</returns>
	public async Task<OperateResult> SetMJAsync(string programName = null)
	{
		return await ReadByCommandAsync("SETMJ", programName);
	}

	/// <summary>
	/// 设定执行程序的名称和行编号。
	/// Set the name and line number of the executed program.
	/// </summary>
	/// <param name="programName">设定程序名称</param>
	/// <param name="line">设定行编号（ 0 ～ 9999）</param>
	/// <returns>是否设定成功</returns>
	public async Task<OperateResult> JSeqAsync(string programName, int line)
	{
		return await ReadByCommandAsync("JSEQ", $"{programName},{line}");
	}

	/// <summary>
	/// 向指定的坐标系位置进行关节动作。其中没有外部轴的系统， 7-12外部轴的值设定为「0」
	/// Perform joint motions to the specified coordinate system position.
	/// where there is no external axis system, the value of 7-12 external axis is set to "0"
	/// </summary>
	/// <param name="robotData">机器的的数据信息</param>
	/// <returns>是否动作成功</returns>
	/// <remarks>
	/// 其中形态数据由6个bool数组组成，每个bool含义参考参数说明，0表示 False，1表示 True
	/// </remarks>
	public async Task<OperateResult> MoveJAsync(YRCRobotData robotData)
	{
		return await ReadByCommandAsync("MOVJ", robotData.ToWriteString(Type));
	}

	/// <summary>
	/// 读取I/O 信号。 I/O 数据是每8个点输出，所以读出接点数是8的倍数。
	/// Read I/O signal. I/O data is output every 8 points, so the number of read contacts is a multiple of 8.
	/// </summary>
	/// <param name="address">读出开始点编号</param>
	/// <param name="length">读出的接点数</param>
	/// <returns>读取的结果点位信息</returns>
	public async Task<OperateResult<bool[]>> IOReadAsync(int address, int length)
	{
		OperateResult<string> read = await ReadByCommandAsync("IOREAD", $"{address},{length}");
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(read);
		}
		byte[] buffer = read.Content.ToStringArray<byte>();
		return OperateResult.CreateSuccessResult(buffer.ToBoolArray());
	}

	/// <summary>
	/// 写入I/O信号状态，写入接点数请指定8的倍数。IO 信号的网络写入仅可是（ #27010 ～ #29567）。
	/// To write I/O signal status, please specify a multiple of 8 for the number of write contacts.
	/// The network write of IO signal is only available (#27010 to #29567).
	/// </summary>
	/// <param name="address">写入开始接点编号</param>
	/// <param name="value">写入的bool值，写入接点数请指定8的倍数。</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> IOWriteAsync(int address, bool[] value)
	{
		if (value == null || value.Length % 8 != 0)
		{
			return new OperateResult("Parameter [value] can't be null or length must be 8 *N");
		}
		byte[] buffer = value.ToByteArray();
		StringBuilder sb = new StringBuilder($"{address},{value.Length}");
		for (int i = 0; i < buffer.Length; i++)
		{
			sb.Append(",");
			sb.Append(buffer[i].ToString());
		}
		return await ReadByCommandAsync("IOWRITE", sb.ToString());
	}

	public override string ToString()
	{
		return $"YRC1000TcpNet Robot[{IpAddress}:{Port}]";
	}
}
