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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Core.Pipe;
using HslCommunication.Reflection;

namespace HslCommunication.Robot.YAMAHA;

/// <summary>
/// 雅马哈机器人的数据访问类
/// </summary>
public class YamahaRCX : TcpNetCommunication
{
	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public YamahaRCX()
	{
		base.ReceiveTimeOut = 30000;
		LogMsgFormatBinary = false;
	}

	/// <summary>
	/// 指定IP地址和端口来实例化一个对象
	/// </summary>
	/// <param name="ipAddress">IP地址</param>
	/// <param name="port">端口号</param>
	public YamahaRCX(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new SpecifiedCharacterMessage(13, 10);
	}

	private int CalculateReceiveTimes(byte[] buffer)
	{
		int num = 0;
		for (int i = 0; i < buffer.Length - 1; i++)
		{
			if (buffer[i] == 13 && buffer[i + 1] == 10)
			{
				num++;
				i++;
			}
		}
		return num;
	}

	private string GetErrorText(string err)
	{
		string text = err.Substring(3);
		if (text == "14.000")
		{
			return "通信中断错误 (Communicate disconnected)";
		}
		return string.Empty;
	}

	public override OperateResult<byte[]> ReadFromCoreServer(CommunicationPipe pipe, byte[] send, bool hasResponseData, bool usePackAndUnpack)
	{
		if (usePackAndUnpack)
		{
			send = PackCommandWithHeader(send);
		}
		LogSendMessage(send);
		OperateResult<byte[]> operateResult = pipe.ReadFromCoreServer(GetNewNetMessage(), send, hasResponseData: false, base.LogRevcMessage);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		int num = CalculateReceiveTimes(send);
		MemoryStream memoryStream = new MemoryStream();
		int num2 = 0;
		while (true)
		{
			OperateResult<byte[]> operateResult2 = pipe.ReadFromCoreServer(GetNewNetMessage(), new byte[0], hasResponseData, base.LogRevcMessage);
			if (!operateResult2.IsSuccess)
			{
				return operateResult2;
			}
			string message = Encoding.ASCII.GetString(operateResult2.Content);
			if (message.StartsWith("Welcome to ", StringComparison.OrdinalIgnoreCase) || message == "\r\n")
			{
				continue;
			}
			memoryStream.Write(operateResult2.Content);
			if (message.StartsWith("OK\r\n", StringComparison.OrdinalIgnoreCase))
			{
				num2++;
				if (num2 >= num)
				{
					break;
				}
			}
			if (message.StartsWith("END\r\n", StringComparison.OrdinalIgnoreCase))
			{
				num2++;
				if (num2 >= num)
				{
					break;
				}
			}
			if (!message.StartsWith("NG=", StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}
			num2++;
			if (num2 < num)
			{
				continue;
			}
			if (message.EndsWith("\r\n"))
			{
				message = message.RemoveLast(2);
			}
			return new OperateResult<byte[]>("faild: " + message + GetErrorText(message));
		}
		if (!usePackAndUnpack)
		{
			return OperateResult.CreateSuccessResult(memoryStream.ToArray());
		}
		OperateResult<byte[]> operateResult3 = UnpackResponseContent(send, memoryStream.ToArray());
		if (!operateResult3.IsSuccess && operateResult3.ErrorCode == int.MinValue)
		{
			operateResult3.ErrorCode = 10000;
		}
		return operateResult3;
	}

	public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(CommunicationPipe pipe, byte[] send, bool hasResponseData, bool usePackAndUnpack)
	{
		if (usePackAndUnpack)
		{
			send = PackCommandWithHeader(send);
		}
		LogSendMessage(send);
		OperateResult<byte[]> cmdSend = await pipe.ReadFromCoreServerAsync(GetNewNetMessage(), send, hasResponseData: false, base.LogRevcMessage);
		if (!cmdSend.IsSuccess)
		{
			return cmdSend;
		}
		int count = CalculateReceiveTimes(send);
		MemoryStream ms = new MemoryStream();
		int received = 0;
		while (true)
		{
			OperateResult<byte[]> cmdRecv = await pipe.ReadFromCoreServerAsync(GetNewNetMessage(), new byte[0], hasResponseData, base.LogRevcMessage);
			if (!cmdRecv.IsSuccess)
			{
				return cmdRecv;
			}
			string str = Encoding.ASCII.GetString(cmdRecv.Content);
			if (str.StartsWith("Welcome to ", StringComparison.OrdinalIgnoreCase) || str == "\r\n")
			{
				continue;
			}
			ms.Write(cmdRecv.Content);
			if (str.StartsWith("OK\r\n", StringComparison.OrdinalIgnoreCase))
			{
				received++;
				if (received >= count)
				{
					break;
				}
			}
			if (str.StartsWith("END\r\n", StringComparison.OrdinalIgnoreCase))
			{
				received++;
				if (received >= count)
				{
					break;
				}
			}
			if (!str.StartsWith("NG=", StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}
			received++;
			if (received < count)
			{
				continue;
			}
			if (str.EndsWith("\r\n"))
			{
				str = str.RemoveLast(2);
			}
			return new OperateResult<byte[]>("faild: " + str + GetErrorText(str));
		}
		if (!usePackAndUnpack)
		{
			return OperateResult.CreateSuccessResult(ms.ToArray());
		}
		OperateResult<byte[]> unpack = UnpackResponseContent(send, ms.ToArray());
		if (!unpack.IsSuccess && unpack.ErrorCode == int.MinValue)
		{
			unpack.ErrorCode = 10000;
		}
		return unpack;
	}

	[Obsolete]
	/// <summary>
	/// 读取指定的命令的方法，需要指定命令
	/// The method of reading the specified command requires the specified command and the line number information of the received command
	/// </summary>
	/// <param name="command">命令</param>
	/// <returns>接收的命令</returns>
	public async Task<OperateResult<string[]>> ReadCommandAsync(string command, int lines)
	{
		return await ReadCommandAsync(command);
	}

	/// <summary>
	/// 读取指定的命令的方法，需要指定命令
	/// The method of reading the specified command requires the specified command and the line number information of the received command
	/// </summary>
	/// <param name="command">命令</param>
	/// <returns>接收的命令</returns>
	public async Task<OperateResult<string[]>> ReadCommandAsync(string command)
	{
		if (command == null)
		{
			throw new ArgumentNullException("command");
		}
		if (!command.EndsWith("\r\n"))
		{
			command += "\r\n";
		}
		byte[] buffer = Encoding.ASCII.GetBytes(command);
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(buffer);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string[]>(read);
		}
		return OperateResult.CreateSuccessResult(Encoding.ASCII.GetString(read.Content).Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
	}

	/// <summary>
	/// 指定程序复位信息，对所有的程序进行复位。当重新启动了程序时，从主程序或者任务 1 中最后执行的程序开头开始执行。
	/// Specify the program reset information to reset all programs. When the program is restarted,
	/// execution starts from the beginning of the main program or the last executed program in task 1.
	/// </summary>
	/// <returns>执行结果是否成功</returns>
	public async Task<OperateResult> ResetAsync()
	{
		return await ReadCommandAsync("@ RESET ");
	}

	/// <summary>
	/// 执行程序运行。执行所有的 RUN 状态程序。
	/// Execute the program to run. Execute all RUN state programs.
	/// </summary>
	/// <returns>执行结果是否成功</returns>
	public async Task<OperateResult> RunAsync()
	{
		return await ReadCommandAsync("@ RUN ");
	}

	/// <summary>
	/// 执行程序停止。执行所有的 STOP 状态程序。
	/// The execution program stops. Execute all STOP state programs.
	/// </summary>
	/// <returns>执行结果是否成功</returns>
	public async Task<OperateResult> StopAsync()
	{
		return await ReadCommandAsync("@ STOP ");
	}

	/// <summary>
	/// 获取马达电源状态，返回的0:马达电源关闭; 1:马达电源开启; 2:马达电源开启＋所有机器人伺服开启
	/// Get the motor power status, return 0: motor power off; 1: motor power on; 2: motor power on + all robot servos on
	/// </summary>
	/// <returns>返回的0:马达电源关闭; 1:马达电源开启; 2:马达电源开启＋所有机器人伺服开启</returns>
	public async Task<OperateResult<int>> ReadMotorStatusAsync()
	{
		return GetTValueHelper(await ReadCommandAsync("@?MOTOR "), Convert.ToInt32);
	}

	/// <summary>
	/// 读取模式状态
	/// Read mode status
	/// </summary>
	/// <returns>模式的状态信息</returns>
	public async Task<OperateResult<int>> ReadModeStatusAsync()
	{
		return GetTValueHelper(await ReadCommandAsync("@?MODE "), Convert.ToInt32);
	}

	/// <summary>
	/// 读取关节的基本数据信息
	/// Read the basic data information of the joint
	/// </summary>
	/// <returns>关节信息</returns>
	public async Task<OperateResult<float[]>> ReadJointsAsync()
	{
		return GetTValueHelper(await ReadCommandAsync("@?WHERE "), ConvertToSingleHelper);
	}

	/// <summary>
	/// 读取紧急停止状态，0 ：正常状态、1 ：紧急停止状态
	/// Read emergency stop state, 0: normal state, 1: emergency stop state
	/// </summary>
	/// <returns>0 ：正常状态、1 ：紧急停止状态</returns>
	public async Task<OperateResult<int>> ReadEmergencyStatusAsync()
	{
		return GetTValueHelper(await ReadCommandAsync("@?EMG "), Convert.ToInt32);
	}

	/// <summary>
	/// 读取输入的点位信息，返回bool数组
	/// Read the input point information and return a bool array
	/// </summary>
	/// <param name="index">哪个位置的数据</param>
	/// <returns>点位的通断信息</returns>
	public async Task<OperateResult<bool[]>> ReadDIAsync(int index)
	{
		return GetBoolArray(await ReadCommandAsync($"@?DI{index}()"));
	}

	/// <summary>
	/// 读取输出的点位信息，返回bool数组
	/// Read the output point information and return a bool array
	/// </summary>
	/// <param name="index">哪个位置的数据</param>
	/// <returns>点位的通断信息</returns>
	public async Task<OperateResult<bool[]>> ReadDOAsync(int index)
	{
		return GetBoolArray(await ReadCommandAsync($"@?DO{index}()"));
	}

	[Obsolete]
	/// <summary>
	/// 读取指定的命令的方法，需要指定命令
	/// The method of reading the specified command requires the specified command and the line number information of the received command
	/// </summary>
	/// <param name="command">命令</param>
	/// <returns>接收的命令</returns>
	public OperateResult<string[]> ReadCommand(string command, int lines)
	{
		return ReadCommand(command);
	}

	[HslMqttApi(Description = "The method of reading the specified command requires the specified command and the line number information of the received command")]
	/// <summary>
	/// 读取指定的命令的方法，需要指定命令
	/// The method of reading the specified command requires the specified command and the line number information of the received command
	/// </summary>
	/// <param name="command">命令</param>
	/// <returns>接收的命令</returns>
	public OperateResult<string[]> ReadCommand(string command)
	{
		if (command == null)
		{
			throw new ArgumentNullException("command");
		}
		if (!command.EndsWith("\r\n"))
		{
			command += "\r\n";
		}
		byte[] bytes = Encoding.ASCII.GetBytes(command);
		OperateResult<byte[]> operateResult = ReadFromCoreServer(bytes);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(Encoding.ASCII.GetString(operateResult.Content).Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
	}

	[HslMqttApi(Description = "Specify the program reset information to reset all programs. When the program is restarted")]
	/// <summary>
	/// 指定程序复位信息，对所有的程序进行复位。当重新启动了程序时，从主程序或者任务 1 中最后执行的程序开头开始执行。
	/// Specify the program reset information to reset all programs. When the program is restarted,
	/// execution starts from the beginning of the main program or the last executed program in task 1.
	/// </summary>
	/// <returns>执行结果是否成功</returns>
	public OperateResult Reset()
	{
		return ReadCommand("@ RESET ");
	}

	[HslMqttApi(Description = "Execute the program to run. Execute all RUN state programs.")]
	/// <summary>
	/// 执行程序运行。执行所有的 RUN 状态程序。
	/// Execute the program to run. Execute all RUN state programs.
	/// </summary>
	/// <returns>执行结果是否成功</returns>
	public OperateResult Run()
	{
		return ReadCommand("@ RUN ");
	}

	/// <summary>
	/// 按照优先顺序 p 将指定的程序登录到任务 n 中。已登录程序变为 STOP 状态。
	/// Logs the specified program into task n in order of p. The logged-in program changes to the STOP state.
	/// </summary>
	/// <param name="program">程序名称</param>
	/// <param name="taskId">任务编号</param>
	/// <returns>是否加载成功</returns>
	public OperateResult Load(string program, int taskId)
	{
		return ReadCommand($"＠ LOAD <{program}>, T{taskId}");
	}

	[HslMqttApi(Description = "The execution program stops. Execute all STOP state programs.")]
	/// <summary>
	/// 执行程序停止。执行所有的 STOP 状态程序。
	/// The execution program stops. Execute all STOP state programs.
	/// </summary>
	/// <returns>执行结果是否成功</returns>
	public OperateResult Stop()
	{
		return ReadCommand("@ STOP ");
	}

	private string GetJogXYCommand(int axis, int robot = 1, bool tail = false)
	{
		StringBuilder stringBuilder = new StringBuilder("@ JOGXY ");
		if (robot != 1)
		{
			stringBuilder.Append($"[{robot}] ");
		}
		stringBuilder.Append(Math.Abs(axis).ToString());
		stringBuilder.Append((axis > 0) ? "+" : "-");
		if (tail)
		{
			stringBuilder.Append("\r\n");
		}
		return stringBuilder.ToString();
	}

	/// <summary>
	/// 只是发送机器人的指定轴进行手动移动（点动）的命令
	/// </summary>
	/// <param name="robot">机器人编号</param>
	/// <param name="axis">轴ID信息</param>
	/// <returns>是否操作成功</returns>
	public OperateResult SendJogXY(int axis, int robot = 1)
	{
		return CommunicationPipe.Send(Encoding.ASCII.GetBytes(GetJogXYCommand(axis, robot, tail: true)));
	}

	/// <summary>
	/// 对＜机器人编号＞指定机器人的指定轴进行手动移动（点动）。＜机器人编号＞可以省略。当进行省略时，机器人 1 被指定。
	/// </summary>
	/// <param name="robot">机器人编号</param>
	/// <param name="axis">轴ID信息</param>
	/// <returns>是否操作成功</returns>
	public OperateResult JogXY(int axis, int robot = 1)
	{
		OperateResult<string[]> operateResult = ReadCommand(GetJogXYCommand(axis, robot));
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<int>(operateResult);
		}
		return operateResult;
	}

	[HslMqttApi(Description = "Get the motor power status, return 0: motor power off; 1: motor power on; 2: motor power on + all robot servos on")]
	/// <summary>
	/// 获取马达电源状态，返回的0:马达电源关闭; 1:马达电源开启; 2:马达电源开启＋所有机器人伺服开启
	/// Get the motor power status, return 0: motor power off; 1: motor power on; 2: motor power on + all robot servos on
	/// </summary>
	/// <returns>返回的0:马达电源关闭; 1:马达电源开启; 2:马达电源开启＋所有机器人伺服开启</returns>
	public OperateResult<int> ReadMotorStatus()
	{
		return GetTValueHelper(ReadCommand("@?MOTOR "), Convert.ToInt32);
	}

	[HslMqttApi(Description = "Read mode status")]
	/// <summary>
	/// 读取模式状态
	/// Read mode status
	/// </summary>
	/// <returns>模式的状态信息</returns>
	public OperateResult<int> ReadModeStatus()
	{
		return GetTValueHelper(ReadCommand("@?MODE "), Convert.ToInt32);
	}

	[HslMqttApi(Description = "Read the basic data information of the joint")]
	/// <summary>
	/// 读取关节的基本数据信息
	/// Read the basic data information of the joint
	/// </summary>
	/// <returns>关节信息</returns>
	public OperateResult<float[]> ReadJoints()
	{
		return GetTValueHelper(ReadCommand("@?WHERE "), ConvertToSingleHelper);
	}

	[HslMqttApi(Description = "Read emergency stop state, 0: normal state, 1: emergency stop state")]
	/// <summary>
	/// 读取紧急停止状态，0 ：正常状态、1 ：紧急停止状态
	/// Read emergency stop state, 0: normal state, 1: emergency stop state
	/// </summary>
	/// <returns>0 ：正常状态、1 ：紧急停止状态</returns>
	public OperateResult<int> ReadEmergencyStatus()
	{
		return GetTValueHelper(ReadCommand("@?EMG "), Convert.ToInt32);
	}

	[HslMqttApi(Description = "Read the input point information and return a bool array")]
	/// <summary>
	/// 读取输入的点位信息，返回bool数组
	/// Read the input point information and return a bool array
	/// </summary>
	/// <param name="index">哪个位置的数据</param>
	/// <returns>点位的通断信息</returns>
	public OperateResult<bool[]> ReadDI(int index)
	{
		return GetBoolArray(ReadCommand($"@?DI{index}()"));
	}

	[HslMqttApi(Description = "Read the output point information and return a bool array")]
	/// <summary>
	/// 读取输出的点位信息，返回bool数组
	/// Read the output point information and return a bool array
	/// </summary>
	/// <param name="index">哪个位置的数据</param>
	/// <returns>点位的通断信息</returns>
	public OperateResult<bool[]> ReadDO(int index)
	{
		return GetBoolArray(ReadCommand($"@?DO{index}()"));
	}

	private OperateResult<bool[]> GetBoolArray(OperateResult<string[]> read)
	{
		return GetTValueHelper(read, delegate(string m)
		{
			int num = Convert.ToInt32(m);
			return new byte[1] { (byte)num }.ToBoolArray();
		});
	}

	private OperateResult<T> GetTValueHelper<T>(OperateResult<string[]> read, Func<string, T> func)
	{
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T>(read);
		}
		string text = string.Empty;
		try
		{
			text = read.Content[0];
			return OperateResult.CreateSuccessResult(func(text));
		}
		catch (Exception ex)
		{
			return new OperateResult<T>(ex.Message + " Source: " + text);
		}
	}

	private float[] ConvertToSingleHelper(string content)
	{
		return (from m in content.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
			select Convert.ToSingle(m)).ToArray();
	}

	public override string ToString()
	{
		return $"YamahaRCX[{IpAddress}:{Port}]";
	}
}
