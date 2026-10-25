// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Reflection;
using Newtonsoft.Json;

namespace HslCommunication.Robot.EFORT;

/// <summary>
/// 埃夫特机器人对应型号为ER7B-C10，此协议为定制版，新版报文对齐
/// The corresponding model of efort robot is er7b-c10. This protocol is the customized version, and the new version is the message alignment
/// </summary>
/// <remarks>
/// 针对的控制器是KEBA控制器
/// </remarks>
public class ER7BC10 : NetworkDoubleBase, IRobotNet
{
	private SoftIncrementCount softIncrementCount;

	/// <summary>
	/// 实例化一个默认的对象，并指定IP地址和端口号，端口号通常为8008
	/// Instantiate a default object and specify the IP address and port number, usually 8008
	/// </summary>
	/// <param name="ipAddress">Ip地址</param>
	/// <param name="port">端口号</param>
	public ER7BC10(string ipAddress, int port)
	{
		IpAddress = ipAddress;
		Port = port;
		base.ByteTransform = new RegularByteTransform();
		softIncrementCount = new SoftIncrementCount(65535L, 0L);
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new EFORTMessage();
	}

	/// <summary>
	/// 获取发送的消息的命令
	/// Gets the command to send the message
	/// </summary>
	/// <returns>字节数组命令</returns>
	public byte[] GetReadCommand()
	{
		byte[] buffer = new byte[38];
		Encoding.ASCII.GetBytes("MessageHead").CopyTo(buffer, 0);
		BitConverter.GetBytes((ushort)buffer.Length).CopyTo(buffer, 16);
		BitConverter.GetBytes((ushort)1001).CopyTo(buffer, 18);
		BitConverter.GetBytes((ushort)softIncrementCount.GetCurrentValue()).CopyTo(buffer, 20);
		Encoding.ASCII.GetBytes("MessageTail").CopyTo(buffer, 22);
		return buffer;
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
		return ReadFromCoreServer(GetReadCommand());
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
		OperateResult<EfortData> operateResult = ReadEfortData();
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		return OperateResult.CreateSuccessResult(JsonConvert.SerializeObject((object)operateResult.Content, (Formatting)1));
	}

	[HslMqttApi(ApiTopic = "WriteRobotByte", Description = "This robot does not support this method operation, will always return failed, invalid operation")]
	/// <summary>
	/// 本机器人不支持该方法操作，将永远返回失败，无效的操作
	/// This robot does not support this method operation, will always return failed, invalid operation
	/// </summary>
	/// <param name="address">指定的地址信息，有些机器人可能不支持</param>
	/// <param name="value">原始的字节数据信息</param>
	/// <returns>是否成功的写入</returns>
	public OperateResult Write(string address, byte[] value)
	{
		return new OperateResult(StringResources.Language.NotSupportedFunction);
	}

	[HslMqttApi(ApiTopic = "WriteRobotString", Description = "This robot does not support this method operation, will always return failed, invalid operation")]
	/// <summary>
	/// 本机器人不支持该方法操作，将永远返回失败，无效的操作
	/// This robot does not support this method operation, will always return failed, invalid operation
	/// </summary>
	/// <param name="address">指定的地址信息，有些机器人可能不支持</param>
	/// <param name="value">字符串的数据信息</param>
	/// <returns>是否成功的写入</returns>
	public OperateResult Write(string address, string value)
	{
		return new OperateResult(StringResources.Language.NotSupportedFunction);
	}

	[HslMqttApi(Description = "Read the details of the robot and return the resolved data type")]
	/// <summary>
	/// 读取机器人的详细信息，返回解析后的数据类型
	/// Read the details of the robot and return the resolved data type
	/// </summary>
	/// <returns>结果数据信息</returns>
	public OperateResult<EfortData> ReadEfortData()
	{
		OperateResult<byte[]> operateResult = Read("");
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<EfortData>(operateResult);
		}
		return EfortData.PraseFrom(operateResult.Content);
	}

	/// <summary>
	/// 根据地址读取机器人的原始的字节数据信息
	/// Read the robot's original byte data information according to the address
	/// </summary>
	/// <param name="address">指定的地址信息，对于某些机器人无效</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public async Task<OperateResult<byte[]>> ReadAsync(string address)
	{
		return await ReadFromCoreServerAsync(GetReadCommand());
	}

	/// <summary>
	/// 根据地址读取机器人的字符串的数据信息
	/// Read the string data information of the robot based on the address
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>带有成功标识的字符串数据</returns>
	public async Task<OperateResult<string>> ReadStringAsync(string address)
	{
		OperateResult<EfortData> read = await ReadEfortDataAsync();
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		return OperateResult.CreateSuccessResult(JsonConvert.SerializeObject((object)read.Content, (Formatting)1));
	}

	/// <summary>
	/// 本机器人不支持该方法操作，将永远返回失败，无效的操作
	/// This robot does not support this method operation, will always return failed, invalid operation
	/// </summary>
	/// <param name="address">指定的地址信息，有些机器人可能不支持</param>
	/// <param name="value">原始的字节数据信息</param>
	/// <returns>是否成功的写入</returns>
	public async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return new OperateResult(StringResources.Language.NotSupportedFunction);
	}

	/// <summary>
	/// 本机器人不支持该方法操作，将永远返回失败，无效的操作
	/// This robot does not support this method operation, will always return failed, invalid operation
	/// </summary>
	/// <param name="address">指定的地址信息，有些机器人可能不支持</param>
	/// <param name="value">字符串的数据信息</param>
	/// <returns>是否成功的写入</returns>
	public async Task<OperateResult> WriteAsync(string address, string value)
	{
		return new OperateResult(StringResources.Language.NotSupportedFunction);
	}

	/// <summary>
	/// 读取机器人的详细信息，返回解析后的数据类型
	/// Read the details of the robot and return the resolved data type
	/// </summary>
	/// <returns>结果数据信息</returns>
	public async Task<OperateResult<EfortData>> ReadEfortDataAsync()
	{
		OperateResult<byte[]> read = await ReadAsync("");
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<EfortData>(read);
		}
		return EfortData.PraseFrom(read.Content);
	}

	public override string ToString()
	{
		return $"ER7BC10 Robot[{IpAddress}:{Port}]";
	}
}
