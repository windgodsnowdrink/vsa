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

namespace HslCommunication.Core.Net;

/// <summary>
/// 机器人的基于webapi接口的基础类信息
/// </summary>
public class NetworkWebApiRobotBase : NetworkWebApiBase
{
	/// <summary>
	/// 使用指定的ip地址来初始化对象
	/// Initializes the object using the specified IP address
	/// </summary>
	/// <param name="ipAddress">Ip地址信息</param>
	public NetworkWebApiRobotBase(string ipAddress)
		: base(ipAddress)
	{
	}

	/// <summary>
	/// 使用指定的ip地址及端口号来初始化对象
	/// Initializes the object with the specified IP address and port number
	/// </summary>
	/// <param name="ipAddress">Ip地址信息</param>
	/// <param name="port">端口号信息</param>
	public NetworkWebApiRobotBase(string ipAddress, int port)
		: base(ipAddress, port)
	{
	}

	/// <summary>
	/// 使用指定的ip地址，端口号，用户名，密码来初始化对象
	/// Initialize the object with the specified IP address, port number, username, and password
	/// </summary>
	/// <param name="ipAddress">Ip地址信息</param>
	/// <param name="port">端口号信息</param>
	/// <param name="name">用户名</param>
	/// <param name="password">密码</param>
	public NetworkWebApiRobotBase(string ipAddress, int port, string name, string password)
		: base(ipAddress, port, name, password)
	{
	}

	/// <summary>
	/// 等待重写的额外的指令信息的支持。除了url的形式之外，还支持基于命令的数据交互
	/// Additional instruction information waiting for rewriting is supported.In addition to the url format, command based data interaction is supported
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>是否读取成功的内容</returns>
	protected virtual OperateResult<string> ReadByAddress(string address)
	{
		return new OperateResult<string>(StringResources.Language.NotSupportedFunction);
	}

	/// <summary>
	/// 等待重写的额外的指令信息的支持。除了url的形式之外，还支持基于命令的数据交互
	/// Additional instruction information waiting for rewriting is supported.In addition to the url format, command based data interaction is supported
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>是否读取成功的内容</returns>
	protected virtual async Task<OperateResult<string>> ReadByAddressAsync(string address)
	{
		return new OperateResult<string>(StringResources.Language.NotSupportedFunction);
	}

	/// <summary>
	/// 读取对方信息的的数据信息，通常是针对GET的方法信息设计的。如果使用了url=开头，就表示是使用了原生的地址访问
	/// Read the other side of the data information, usually designed for the GET method information.If you start with url=, you are using native address access
	/// </summary>
	/// <param name="address">无效参数</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public virtual OperateResult<byte[]> Read(string address)
	{
		OperateResult<string> operateResult = ReadString(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(Encoding.UTF8.GetBytes(operateResult.Content));
	}

	/// <summary>
	/// 读取对方信息的的字符串数据信息，通常是针对GET的方法信息设计的。如果使用了url=开头，就表示是使用了原生的地址访问
	/// The string data information that reads the other party information, usually designed for the GET method information.If you start with url=, you are using native address access
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>带有成功标识的字符串数据</returns>
	public virtual OperateResult<string> ReadString(string address)
	{
		if (!Authorization.nzugaydgwadawdibbas())
		{
			return new OperateResult<string>(StringResources.Language.AuthorizationFailed);
		}
		if (address.StartsWith("url=", StringComparison.OrdinalIgnoreCase))
		{
			return Get(address.Substring(4));
		}
		return ReadByAddress(address);
	}

	/// <summary>
	/// 使用POST的方式来向对方进行请求数据信息，需要使用url=开头，来表示是使用了原生的地址访问
	/// Using POST to request data information from the other party, we need to start with url= to indicate that we are using native address access
	/// </summary>
	/// <param name="address">指定的地址信息，有些设备可能不支持</param>
	/// <param name="value">原始的字节数据信息</param>
	/// <returns>是否成功的写入</returns>
	public virtual OperateResult Write(string address, byte[] value)
	{
		return Write(address, Encoding.Default.GetString(value));
	}

	/// <summary>
	/// 使用POST的方式来向对方进行请求数据信息，需要使用url=开头，来表示是使用了原生的地址访问
	/// Using POST to request data information from the other party, we need to start with url= to indicate that we are using native address access
	/// </summary>
	/// <param name="address">指定的地址信息</param>
	/// <param name="value">字符串的数据信息</param>
	/// <returns>是否成功的写入</returns>
	public virtual OperateResult Write(string address, string value)
	{
		if (address.StartsWith("url=", StringComparison.OrdinalIgnoreCase))
		{
			return Post(address.Substring(4), value);
		}
		return new OperateResult<string>(StringResources.Language.NotSupportedFunction);
	}

	/// <summary>
	/// 读取对方信息的的数据信息，通常是针对GET的方法信息设计的。如果使用了url=开头，就表示是使用了原生的地址访问
	/// Read the other side of the data information, usually designed for the GET method information.If you start with url=, you are using native address access
	/// </summary>
	/// <param name="address">无效参数</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public virtual async Task<OperateResult<byte[]>> ReadAsync(string address)
	{
		OperateResult<string> read = await ReadStringAsync(address);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return OperateResult.CreateSuccessResult(Encoding.UTF8.GetBytes(read.Content));
	}

	/// <summary>
	/// 读取对方信息的的字符串数据信息，通常是针对GET的方法信息设计的。如果使用了url=开头，就表示是使用了原生的地址访问
	/// The string data information that reads the other party information, usually designed for the GET method information.If you start with url=, you are using native address access
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>带有成功标识的字符串数据</returns>
	public virtual async Task<OperateResult<string>> ReadStringAsync(string address)
	{
		if (!Authorization.nzugaydgwadawdibbas())
		{
			return new OperateResult<string>(StringResources.Language.AuthorizationFailed);
		}
		if (address.StartsWith("url=", StringComparison.OrdinalIgnoreCase))
		{
			return await GetAsync(address.Substring(4));
		}
		return await ReadByAddressAsync(address);
	}

	/// <summary>
	/// 使用POST的方式来向对方进行请求数据信息，需要使用url=开头，来表示是使用了原生的地址访问
	/// Using POST to request data information from the other party, we need to start with url= to indicate that we are using native address access
	/// </summary>
	/// <param name="address">指定的地址信息，有些设备可能不支持</param>
	/// <param name="value">原始的字节数据信息</param>
	/// <returns>是否成功的写入</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await WriteAsync(address, Encoding.Default.GetString(value));
	}

	/// <summary>
	/// 使用POST的方式来向对方进行请求数据信息，需要使用url=开头，来表示是使用了原生的地址访问
	/// Using POST to request data information from the other party, we need to start with url= to indicate that we are using native address access
	/// </summary>
	/// <param name="address">指定的地址信息</param>
	/// <param name="value">字符串的数据信息</param>
	/// <returns>是否成功的写入</returns>
	public virtual async Task<OperateResult> WriteAsync(string address, string value)
	{
		if (address.StartsWith("url=", StringComparison.OrdinalIgnoreCase))
		{
			return await PostAsync(address.Substring(4), value);
		}
		return new OperateResult<string>(StringResources.Language.NotSupportedFunction);
	}

	public override string ToString()
	{
		return $"NetworkWebApiRobotBase[{base.IpAddress}:{base.Port}]";
	}
}
