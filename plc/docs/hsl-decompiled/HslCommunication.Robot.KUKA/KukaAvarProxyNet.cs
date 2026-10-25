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
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Reflection;

namespace HslCommunication.Robot.KUKA;

/// <summary>
/// Kuka机器人的数据交互类，通讯支持的条件为KUKA 的 KRC4 控制器中运行KUKAVARPROXY 这个第三方软件，端口通常为7000
/// The data interaction class of Kuka robot is supported by the third-party software KUKAVARPROXY running in the KRC4 controller of Kuka. The port is usually 7000
/// </summary>
/// <remarks>
/// 非常感谢 昆山-LT 网友的测试和意见反馈。
/// 其中KUKAVARPROXY 这个第三方软件在来源地址：
/// https://github.com/ImtsSrl/KUKAVARPROXY
/// </remarks>
public class KukaAvarProxyNet : NetworkDoubleBase, IRobotNet
{
	private SoftIncrementCount softIncrementCount;

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public KukaAvarProxyNet()
	{
		softIncrementCount = new SoftIncrementCount(65535L, 0L);
		base.ByteTransform = new RegularByteTransform(DataFormat.CDAB);
	}

	/// <summary>
	/// 实例化一个默认的Kuka机器人对象，并指定IP地址和端口号，端口号通常为7000
	/// Instantiate a default Kuka robot object and specify the IP address and port number, usually 7000
	/// </summary>
	/// <param name="ipAddress">Ip地址</param>
	/// <param name="port">端口号</param>
	public KukaAvarProxyNet(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new KukaVarProxyMessage();
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
		return ByteTransformHelper.GetResultFromOther(ReadFromCoreServer(PackCommand(BuildReadValueCommand(address))), ExtractActualData);
	}

	[HslMqttApi(ApiTopic = "ReadRobotString", Description = "Read all the data information of the Kuka robot, return the string information, decode by ANSI, need to specify the variable name")]
	/// <summary>
	/// 读取Kuka机器人的所有的数据信息，返回字符串信息，解码方式为ANSI，需要指定变量名称
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
	/// 根据Kuka机器人的变量名称，写入ANSI编码的字符串数据信息
	/// Writes ansi-encoded string data information based on the variable name of the Kuka robot
	/// </summary>
	/// <param name="address">变量名称</param>
	/// <param name="value">ANSI编码的字符串</param>
	/// <returns>是否成功的写入</returns>
	public OperateResult Write(string address, string value)
	{
		return ByteTransformHelper.GetResultFromOther(ReadFromCoreServer(PackCommand(BuildWriteValueCommand(address, value))), ExtractActualData);
	}

	/// <summary>
	/// 读取Kuka机器人的数据内容，根据输入的变量名称来读取
	/// Read the data content of the Kuka robot according to the input variable name
	/// </summary>
	/// <param name="address">地址数据</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public async Task<OperateResult<byte[]>> ReadAsync(string address)
	{
		return ByteTransformHelper.GetResultFromOther(await ReadFromCoreServerAsync(PackCommand(BuildReadValueCommand(address))), ExtractActualData);
	}

	/// <summary>
	/// 读取Kuka机器人的所有的数据信息，返回字符串信息，解码方式为ANSI，需要指定变量名称
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
	/// 根据Kuka机器人的变量名称，写入ANSI编码的字符串数据信息
	/// Writes ansi-encoded string data information based on the variable name of the Kuka robot
	/// </summary>
	/// <param name="address">变量名称</param>
	/// <param name="value">ANSI编码的字符串</param>
	/// <returns>是否成功的写入</returns>
	public async Task<OperateResult> WriteAsync(string address, string value)
	{
		return ByteTransformHelper.GetResultFromOther(await ReadFromCoreServerAsync(PackCommand(BuildWriteValueCommand(address, value))), ExtractActualData);
	}

	/// <summary>
	/// 将核心的指令打包成一个可用于发送的消息对象
	/// Package the core instructions into a message object that can be sent
	/// </summary>
	/// <param name="commandCore">核心命令</param>
	/// <returns>最终实现的可以发送的机器人的字节数据</returns>
	private byte[] PackCommand(byte[] commandCore)
	{
		byte[] buffer = new byte[commandCore.Length + 4];
		base.ByteTransform.TransByte((ushort)softIncrementCount.GetCurrentValue()).CopyTo(buffer, 0);
		base.ByteTransform.TransByte((ushort)commandCore.Length).CopyTo(buffer, 2);
		commandCore.CopyTo(buffer, 4);
		return buffer;
	}

	private OperateResult<byte[]> ExtractActualData(byte[] response)
	{
		try
		{
			if (response[^1] != 1)
			{
				return new OperateResult<byte[]>(response[^1], "Wrong: " + SoftBasic.ByteToHexString(response, ' '));
			}
			int num = response[5] * 256 + response[6];
			byte[] buffer = new byte[num];
			Array.Copy(response, 7, buffer, 0, num);
			return OperateResult.CreateSuccessResult(buffer);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("Wrong:" + ex.Message + " Code:" + SoftBasic.ByteToHexString(response, ' '));
		}
	}

	private byte[] BuildCommands(byte function, string[] commands)
	{
		List<byte> list = new List<byte>();
		list.Add(function);
		for (int i = 0; i < commands.Length; i++)
		{
			byte[] bytes = Encoding.Default.GetBytes(commands[i]);
			list.AddRange(base.ByteTransform.TransByte((ushort)bytes.Length));
			list.AddRange(bytes);
		}
		return list.ToArray();
	}

	private byte[] BuildReadValueCommand(string address)
	{
		return BuildCommands(0, new string[1] { address });
	}

	private byte[] BuildWriteValueCommand(string address, string value)
	{
		return BuildCommands(1, new string[2] { address, value });
	}

	public override string ToString()
	{
		return $"KukaAvarProxyNet Robot[{IpAddress}:{Port}]";
	}
}
