// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Net;
using HslCommunication.Reflection;

namespace HslCommunication.Robot.KUKA;

/// <summary>
/// Kuka机器人的数据交互类，通讯支持的条件为KUKA 的 TCP通讯
/// </summary>
public class KukaTcpNet : NetworkDoubleBase, IRobotNet
{
	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public KukaTcpNet()
	{
		base.ByteTransform = new RegularByteTransform(DataFormat.CDAB);
		LogMsgFormatBinary = false;
	}

	/// <summary>
	/// 实例化一个默认的Kuka机器人对象，并指定IP地址和端口号，端口号通常为9999
	/// Instantiate a default Kuka robot object and specify the IP address and port number, usually 9999
	/// </summary>
	/// <param name="ipAddress">Ip地址</param>
	/// <param name="port">端口号</param>
	public KukaTcpNet(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	public override OperateResult<byte[]> ReadFromCoreServer(Socket socket, byte[] send, bool hasResponseData = true, bool usePackHeader = true)
	{
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.Send + " : " + (LogMsgFormatBinary ? send.ToHexString(' ') : SoftBasic.GetAsciiStringRender(send)));
		OperateResult operateResult = Send(socket, send);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		if (base.ReceiveTimeOut < 0)
		{
			return OperateResult.CreateSuccessResult(new byte[0]);
		}
		OperateResult<byte[]> operateResult2 = Receive(socket, -1, base.ReceiveTimeOut);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.Receive + " : " + (LogMsgFormatBinary ? operateResult2.Content.ToHexString(' ') : SoftBasic.GetAsciiStringRender(operateResult2.Content)));
		return OperateResult.CreateSuccessResult(operateResult2.Content);
	}

	public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(Socket socket, byte[] send, bool hasResponseData = true, bool usePackHeader = true)
	{
		byte[] sendValue = (usePackHeader ? PackCommandWithHeader(send) : send);
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.Send + " : " + (LogMsgFormatBinary ? sendValue.ToHexString(' ') : SoftBasic.GetAsciiStringRender(sendValue)));
		OperateResult sendResult = await SendAsync(socket, sendValue);
		if (!sendResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(sendResult);
		}
		if (base.ReceiveTimeOut < 0)
		{
			return OperateResult.CreateSuccessResult(new byte[0]);
		}
		OperateResult<byte[]> resultReceive = await ReceiveAsync(socket, -1, base.ReceiveTimeOut);
		if (!resultReceive.IsSuccess)
		{
			return resultReceive;
		}
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.Receive + " : " + (LogMsgFormatBinary ? resultReceive.Content.ToHexString(' ') : SoftBasic.GetAsciiStringRender(resultReceive.Content)));
		return UnpackResponseContent(sendValue, resultReceive.Content);
	}

	[HslMqttApi(ApiTopic = "ReadRobotByte", Description = "Read the data content of the Kuka robot according to the input variable name")]
	/// <summary>
	/// 读取Kuka机器人的数据内容，根据输入的变量名称来读取
	/// Read the data content of the Kuka robot according to the input variable name
	/// </summary>
	/// <param name="address">地址数据</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public OperateResult<byte[]> Read(string address)
	{
		return ByteTransformHelper.GetResultFromOther(ReadFromCoreServer(Encoding.UTF8.GetBytes(BuildReadCommands(address))), ExtractActualData);
	}

	[HslMqttApi(ApiTopic = "ReadRobotString", Description = "Read all the data information of the Kuka robot, return the string information, decode by ANSI, need to specify the variable name")]
	/// <summary>
	/// 读取Kuka机器人的所有的数据信息，返回字符串信息，解码方式为UTF8，需要指定变量名称
	/// Read all the data information of the Kuka robot, return the string information, decode by ANSI, need to specify the variable name
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>带有成功标识的字符串数据</returns>
	public OperateResult<string> ReadString(string address)
	{
		return ByteTransformHelper.GetSuccessResultFromOther(Read(address), Encoding.Default.GetString);
	}

	[HslMqttApi(ApiTopic = "WriteRobotByte", Description = "Write the original data content according to the variable name of the Kuka robot")]
	/// <summary>
	/// 根据Kuka机器人的变量名称，写入原始的数据内容
	/// Write the original data content according to the variable name of the Kuka robot
	/// </summary>
	/// <param name="address">变量名称</param>
	/// <param name="value">原始的字节数据信息</param>
	/// <returns>是否成功的写入</returns>
	public OperateResult Write(string address, byte[] value)
	{
		return Write(address, Encoding.Default.GetString(value));
	}

	[HslMqttApi(ApiTopic = "WriteRobotString", Description = "Writes ansi-encoded string data information based on the variable name of the Kuka robot")]
	/// <summary>
	/// 根据Kuka机器人的变量名称，写入UTF8编码的字符串数据信息
	/// Writes ansi-encoded string data information based on the variable name of the Kuka robot
	/// </summary>
	/// <param name="address">变量名称</param>
	/// <param name="value">ANSI编码的字符串</param>
	/// <returns>是否成功的写入</returns>
	public OperateResult Write(string address, string value)
	{
		return Write(new string[1] { address }, new string[1] { value });
	}

	[HslMqttApi(ApiTopic = "WriteRobotStrings", Description = "Write multiple UTF8 encoded string data information according to the variable name of the Kuka robot")]
	/// <summary>
	/// 根据Kuka机器人的变量名称，写入多个UTF8编码的字符串数据信息
	/// Write multiple UTF8 encoded string data information according to the variable name of the Kuka robot
	/// </summary>
	/// <param name="address">变量名称</param>
	/// <param name="value">ANSI编码的字符串</param>
	/// <returns>是否成功的写入</returns>
	public OperateResult Write(string[] address, string[] value)
	{
		return ReadCmd(BuildWriteCommands(address, value));
	}

	private OperateResult ReadCmd(string cmd)
	{
		OperateResult<byte[]> operateResult = ReadFromCoreServer(Encoding.UTF8.GetBytes(cmd));
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		string message = Encoding.UTF8.GetString(operateResult.Content);
		if (message.Contains("err"))
		{
			return new OperateResult("Result contains err: " + message);
		}
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi(Description = "Start the specified program of the robot")]
	/// <summary>
	/// 启动机器人的指定的程序
	/// Start the specified program of the robot
	/// </summary>
	/// <param name="program">程序的名字</param>
	/// <returns>是否启动成功</returns>
	public OperateResult StartProgram(string program)
	{
		return ReadCmd("03" + program);
	}

	[HslMqttApi(Description = "Reset current program")]
	/// <summary>
	/// 复位当前的程序
	/// Reset current program
	/// </summary>
	/// <returns>复位结果</returns>
	public OperateResult ResetProgram()
	{
		return ReadCmd("0601");
	}

	[HslMqttApi(Description = "Stop current program")]
	/// <summary>
	/// 停止当前的程序
	/// Stop current program
	/// </summary>
	/// <returns>复位结果</returns>
	public OperateResult StopProgram()
	{
		return ReadCmd("0621");
	}

	/// <summary>
	/// 读取Kuka机器人的数据内容，根据输入的变量名称来读取
	/// Read the data content of the Kuka robot according to the input variable name
	/// </summary>
	/// <param name="address">地址数据</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public async Task<OperateResult<byte[]>> ReadAsync(string address)
	{
		return ByteTransformHelper.GetResultFromOther(await ReadFromCoreServerAsync(Encoding.UTF8.GetBytes(BuildReadCommands(address))), ExtractActualData);
	}

	/// <summary>
	/// 读取Kuka机器人的所有的数据信息，返回字符串信息，解码方式为UTF8，需要指定变量名称
	/// Read all the data information of the Kuka robot, return the string information, decode by ANSI, need to specify the variable name
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>带有成功标识的字符串数据</returns>
	public async Task<OperateResult<string>> ReadStringAsync(string address)
	{
		return ByteTransformHelper.GetSuccessResultFromOther(await ReadAsync(address), Encoding.Default.GetString);
	}

	/// <summary>
	/// 根据Kuka机器人的变量名称，写入原始的数据内容
	/// Write the original data content according to the variable name of the Kuka robot
	/// </summary>
	/// <param name="address">变量名称</param>
	/// <param name="value">原始的字节数据信息</param>
	/// <returns>是否成功的写入</returns>
	public async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await WriteAsync(address, Encoding.Default.GetString(value));
	}

	/// <summary>
	/// 根据Kuka机器人的变量名称，写入UTF8编码的字符串数据信息
	/// Writes ansi-encoded string data information based on the variable name of the Kuka robot
	/// </summary>
	/// <param name="address">变量名称</param>
	/// <param name="value">ANSI编码的字符串</param>
	/// <returns>是否成功的写入</returns>
	public async Task<OperateResult> WriteAsync(string address, string value)
	{
		return await WriteAsync(new string[1] { address }, new string[1] { value });
	}

	/// <summary>
	/// 根据Kuka机器人的变量名称，写入多个UTF8编码的字符串数据信息
	/// Write multiple UTF8 encoded string data information according to the variable name of the Kuka robot
	/// </summary>
	/// <param name="address">变量名称</param>
	/// <param name="value">ANSI编码的字符串</param>
	/// <returns>是否成功的写入</returns>
	public async Task<OperateResult> WriteAsync(string[] address, string[] value)
	{
		return await ReadCmdAsync(BuildWriteCommands(address, value));
	}

	private async Task<OperateResult> ReadCmdAsync(string cmd)
	{
		OperateResult<byte[]> write = await ReadFromCoreServerAsync(Encoding.UTF8.GetBytes(cmd));
		if (!write.IsSuccess)
		{
			return write;
		}
		string msg = Encoding.UTF8.GetString(write.Content);
		if (msg.Contains("err"))
		{
			return new OperateResult("Result contains err: " + msg);
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 启动机器人的指定的程序
	/// Start the specified program of the robot
	/// </summary>
	/// <param name="program">程序的名字</param>
	/// <returns>是否启动成功</returns>
	public async Task<OperateResult> StartProgramAsync(string program)
	{
		return await ReadCmdAsync("03" + program);
	}

	/// <summary>
	/// 复位当前的程序
	/// Reset current program
	/// </summary>
	/// <returns>复位结果</returns>
	public async Task<OperateResult> ResetProgramAsync()
	{
		return await ReadCmdAsync("0601");
	}

	/// <summary>
	/// 停止当前的程序
	/// Stop current program
	/// </summary>
	/// <returns>复位结果</returns>
	public async Task<OperateResult> StopProgramAsync()
	{
		return await ReadCmdAsync("0621");
	}

	private OperateResult<byte[]> ExtractActualData(byte[] response)
	{
		return OperateResult.CreateSuccessResult(response);
	}

	public override string ToString()
	{
		return $"KukaTcpNet[{IpAddress}:{Port}]";
	}

	/// <summary>
	/// 构建读取变量的报文命令
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>报文内容</returns>
	public static string BuildReadCommands(string[] address)
	{
		if (address == null)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder("00");
		for (int i = 0; i < address.Length; i++)
		{
			stringBuilder.Append(address[i] ?? "");
			if (i != address.Length - 1)
			{
				stringBuilder.Append(",");
			}
		}
		return stringBuilder.ToString();
	}

	/// <summary>
	/// 构建读取变量的报文命令
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>报文内容</returns>
	public static string BuildReadCommands(string address)
	{
		return BuildReadCommands(new string[1] { address });
	}

	/// <summary>
	/// 构建写入变量的报文命令
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <param name="values">数据信息</param>
	/// <returns>字符串信息</returns>
	public static string BuildWriteCommands(string[] address, string[] values)
	{
		if (address == null || values == null)
		{
			return string.Empty;
		}
		if (address.Length != values.Length)
		{
			throw new Exception(StringResources.Language.TwoParametersLengthIsNotSame);
		}
		StringBuilder stringBuilder = new StringBuilder("01");
		for (int i = 0; i < address.Length; i++)
		{
			stringBuilder.Append(address[i] + "=");
			stringBuilder.Append(values[i] ?? "");
			if (i != address.Length - 1)
			{
				stringBuilder.Append(",");
			}
		}
		return stringBuilder.ToString();
	}

	/// <summary>
	/// 构建写入变量的报文命令
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <param name="value">数据信息</param>
	/// <returns>字符串信息</returns>
	public static string BuildWriteCommands(string address, string value)
	{
		return BuildWriteCommands(new string[1] { address }, new string[1] { value });
	}
}
