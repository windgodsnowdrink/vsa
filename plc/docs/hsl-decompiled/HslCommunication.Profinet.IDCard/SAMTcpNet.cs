// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.IDCard;

/// <summary>
/// 基于SAM协议的Tcp实现的网络类，支持读取身份证的数据信息，通过透传的形式实现，除了初始化和串口类不一致，调用方法是几乎一模一样的，详细参见API文档
/// The network class implemented by Tcp based on the SAM protocol supports reading ID card data information and is implemented in the form of transparent transmission.
/// Except for the inconsistency between the initialization and the serial port class, the calling method is almost the same.
/// See the API documentation for details
/// </summary>
public class SAMTcpNet : NetworkDoubleBase
{
	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public SAMTcpNet()
	{
		base.ByteTransform = new RegularByteTransform();
	}

	/// <summary>
	/// 通过指定的ip地址以及端口来实例化对象
	/// Instantiate the object with the specified IP address and port
	/// </summary>
	/// <param name="ipAddress">ip地址</param>
	/// <param name="port">端口号</param>
	public SAMTcpNet(string ipAddress, int port)
	{
		IpAddress = ipAddress;
		Port = port;
		base.ByteTransform = new RegularByteTransform();
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new SAMMessage();
	}

	[HslMqttApi]
	/// <summary>
	/// 读取身份证设备的安全模块号
	/// Read the security module number of the ID device
	/// </summary>
	/// <returns>结果数据内容</returns>
	public OperateResult<string> ReadSafeModuleNumber()
	{
		byte[] send = SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand(18, byte.MaxValue, null));
		OperateResult<byte[]> operateResult = ReadFromCoreServer(send);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		OperateResult operateResult2 = SAMSerial.CheckADSCommandAndSum(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult2);
		}
		return SAMSerial.ExtractSafeModuleNumber(operateResult.Content);
	}

	[HslMqttApi]
	/// <summary>
	/// 检测安全模块状态
	/// Detecting Security Module Status
	/// </summary>
	/// <returns>返回是否检测成功</returns>
	public OperateResult CheckSafeModuleStatus()
	{
		byte[] send = SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand(18, byte.MaxValue, null));
		OperateResult<byte[]> operateResult = ReadFromCoreServer(send);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		OperateResult operateResult2 = SAMSerial.CheckADSCommandAndSum(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult2);
		}
		if (operateResult.Content[9] != 144)
		{
			return new OperateResult(SAMSerial.GetErrorDescription(operateResult.Content[9]));
		}
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi]
	/// <summary>
	/// 寻找卡片，并返回是否成功
	/// Find cards and return success
	/// </summary>
	/// <returns>是否寻找成功</returns>
	public OperateResult SearchCard()
	{
		byte[] send = SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand(32, 1, null));
		OperateResult<byte[]> operateResult = ReadFromCoreServer(send);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		OperateResult operateResult2 = SAMSerial.CheckADSCommandAndSum(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult2);
		}
		if (operateResult.Content[9] != 159)
		{
			return new OperateResult(SAMSerial.GetErrorDescription(operateResult.Content[9]));
		}
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi]
	/// <summary>
	/// 选择卡片，并返回是否成功
	/// Select card and return success
	/// </summary>
	/// <returns>是否寻找成功</returns>
	public OperateResult SelectCard()
	{
		byte[] send = SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand(32, 2, null));
		OperateResult<byte[]> operateResult = ReadFromCoreServer(send);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		OperateResult operateResult2 = SAMSerial.CheckADSCommandAndSum(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult2);
		}
		if (operateResult.Content[9] != 144)
		{
			return new OperateResult(SAMSerial.GetErrorDescription(operateResult.Content[9]));
		}
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi]
	/// <summary>
	/// 读取卡片，如果成功的话，就返回身份证的所有的信息
	/// Read the card, if successful, return all the information of the ID cards
	/// </summary>
	/// <returns>是否寻找成功</returns>
	public OperateResult<IdentityCard> ReadCard()
	{
		byte[] send = SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand(48, 1, null));
		OperateResult<byte[]> operateResult = ReadFromCoreServer(send);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<IdentityCard>(operateResult);
		}
		OperateResult operateResult2 = SAMSerial.CheckADSCommandAndSum(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<IdentityCard>(operateResult2);
		}
		return SAMSerial.ExtractIdentityCard(operateResult.Content);
	}

	/// <summary>
	/// 读取身份证设备的安全模块号
	/// Read the security module number of the ID device
	/// </summary>
	/// <returns>结果数据内容</returns>
	public async Task<OperateResult<string>> ReadSafeModuleNumberAsync()
	{
		byte[] command = SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand(18, byte.MaxValue, null));
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		OperateResult check = SAMSerial.CheckADSCommandAndSum(read.Content);
		if (!check.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(check);
		}
		return SAMSerial.ExtractSafeModuleNumber(read.Content);
	}

	/// <summary>
	/// 检测安全模块状态
	/// Detecting Security Module Status
	/// </summary>
	/// <returns>返回是否检测成功</returns>
	public async Task<OperateResult> CheckSafeModuleStatusAsync()
	{
		byte[] command = SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand(18, byte.MaxValue, null));
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		OperateResult check = SAMSerial.CheckADSCommandAndSum(read.Content);
		if (!check.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(check);
		}
		if (read.Content[9] != 144)
		{
			return new OperateResult(SAMSerial.GetErrorDescription(read.Content[9]));
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 寻找卡片，并返回是否成功
	/// Find cards and return success
	/// </summary>
	/// <returns>是否寻找成功</returns>
	public async Task<OperateResult> SearchCardAsync()
	{
		byte[] command = SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand(32, 1, null));
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		OperateResult check = SAMSerial.CheckADSCommandAndSum(read.Content);
		if (!check.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(check);
		}
		if (read.Content[9] != 159)
		{
			return new OperateResult(SAMSerial.GetErrorDescription(read.Content[9]));
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 选择卡片，并返回是否成功
	/// Select card and return success
	/// </summary>
	/// <returns>是否寻找成功</returns>
	public async Task<OperateResult> SelectCardAsync()
	{
		byte[] command = SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand(32, 2, null));
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		OperateResult check = SAMSerial.CheckADSCommandAndSum(read.Content);
		if (!check.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(check);
		}
		if (read.Content[9] != 144)
		{
			return new OperateResult(SAMSerial.GetErrorDescription(read.Content[9]));
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 读取卡片，如果成功的话，就返回身份证的所有的信息
	/// Read the card, if successful, return all the information of the ID cards
	/// </summary>
	/// <returns>是否寻找成功</returns>
	public async Task<OperateResult<IdentityCard>> ReadCardAsync()
	{
		byte[] command = SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand(48, 1, null));
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<IdentityCard>(read);
		}
		OperateResult check = SAMSerial.CheckADSCommandAndSum(read.Content);
		if (!check.IsSuccess)
		{
			return OperateResult.CreateFailedResult<IdentityCard>(check);
		}
		return SAMSerial.ExtractIdentityCard(read.Content);
	}

	public override string ToString()
	{
		return $"SAMTcpNet[{IpAddress}:{Port}]";
	}
}
