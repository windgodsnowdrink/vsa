// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HslCommunication.Core.Net;

namespace HslCommunication.Core.Device;

/// <summary>
/// 基于WebApi接口的设备基类
/// </summary>
public class DeviceWebApi : DeviceCommunication
{
	private NetworkWebApiBase webApi;

	/// <summary>
	/// 获取或设置远程服务器的IP地址
	/// Gets or sets the IP address of the remote server
	/// </summary>
	public string IpAddress
	{
		get
		{
			return webApi.IpAddress;
		}
		set
		{
			webApi.IpAddress = value;
		}
	}

	/// <summary>
	/// 获取或设置远程服务器的端口号信息
	/// Gets or sets the port number information for the remote server
	/// </summary>
	public int Port
	{
		get
		{
			return webApi.Port;
		}
		set
		{
			webApi.Port = value;
		}
	}

	/// <summary>
	/// 获取或设置当前的用户名
	/// Get or set the current username
	/// </summary>
	public string UserName
	{
		get
		{
			return webApi.UserName;
		}
		set
		{
			webApi.UserName = value;
		}
	}

	/// <summary>
	/// 获取或设置当前的密码
	/// Get or set the current password
	/// </summary>
	public string Password
	{
		get
		{
			return webApi.Password;
		}
		set
		{
			webApi.Password = value;
		}
	}

	/// <summary>
	/// 是否启用Https的协议访问，对于Https来说，端口号默认为 443
	/// Whether to enable Https protocol access, for Https, the port number defaults to 443
	/// </summary>
	public bool UseHttps
	{
		get
		{
			return webApi.UseHttps;
		}
		set
		{
			webApi.UseHttps = value;
		}
	}

	/// <summary>
	/// 默认的内容类型，如果为空，则不进行设置操作。例如设置为 "text/plain", "application/json", "text/html" 等等。
	/// The default content type, if it is empty, no setting operation will be performed. For example, set to "text/plain", "application/json", "text/html" and so on.
	/// </summary>
	public string DefaultContentType
	{
		get
		{
			return webApi.DefaultContentType;
		}
		set
		{
			webApi.DefaultContentType = value;
		}
	}

	/// <summary>
	/// 获取或设置是否使用ISO的编码信息，默认为 False
	/// Get or set whether to use ISO encoding information, the default is False
	/// </summary>
	/// <remarks>
	/// 在访问某些特殊的API的时候，会发生异常"The character set provided in ContentType is invalid...."，这时候，只需要将本属性设置为 True 即可。
	/// </remarks>
	public bool UseEncodingISO
	{
		get
		{
			return webApi.UseEncodingISO;
		}
		set
		{
			webApi.UseEncodingISO = value;
		}
	}

	public HttpClient Client => webApi.Client;

	/// <summary>
	/// 使用指定的ip地址来初始化对象
	/// Initializes the object using the specified IP address
	/// </summary>
	/// <param name="ipAddress">Ip地址信息</param>
	public DeviceWebApi(string ipAddress)
		: this(ipAddress, 80, string.Empty, string.Empty)
	{
	}

	/// <summary>
	/// 使用指定的ip地址及端口号来初始化对象
	/// Initializes the object with the specified IP address and port number
	/// </summary>
	/// <param name="ipAddress">Ip地址信息</param>
	/// <param name="port">端口号信息</param>
	public DeviceWebApi(string ipAddress, int port)
		: this(ipAddress, port, string.Empty, string.Empty)
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
	public DeviceWebApi(string ipAddress, int port, string name, string password)
	{
		webApi = new NetworkWebApiBase(ipAddress, port, name, password);
		webApi.AddRequestHeadersAction = AddRequestHeaders;
	}

	/// <summary>
	/// 针对请求的头信息进行额外的处理
	/// </summary>
	/// <param name="headers">头信息</param>
	protected virtual void AddRequestHeaders(HttpContentHeaders headers)
	{
	}

	/// <summary>
	/// 使用GET操作从网络中获取到数据信息，地址
	/// </summary>
	/// <param name="rawUrl">除去ip地址和端口的地址</param>
	/// <returns>返回的数据内容</returns>
	public OperateResult<string> Get(string rawUrl)
	{
		return webApi.Get(rawUrl);
	}

	/// <summary>
	/// 使用POST命令去提交数据内容，然后返回相关的数据信息
	/// </summary>
	/// <param name="rawUrl">已经去除ip地址，端口号的api信息</param>
	/// <param name="body">数据内容</param>
	/// <returns>从服务器返回的内容</returns>
	public OperateResult<string> Post(string rawUrl, string body)
	{
		return webApi.Post(rawUrl, body);
	}

	/// <summary>
	/// 使用GET操作从网络中获取到数据信息，地址
	/// </summary>
	/// <param name="rawUrl">除去ip地址和端口的地址</param>
	/// <returns>返回的数据内容</returns>
	public async Task<OperateResult<string>> GetAsync(string rawUrl)
	{
		return await webApi.GetAsync(rawUrl);
	}

	/// <summary>
	/// 使用POST命令去提交数据内容，然后返回相关的数据信息
	/// </summary>
	/// <param name="rawUrl">已经去除ip地址，端口号的api信息</param>
	/// <param name="body">数据内容</param>
	/// <returns>从服务器返回的内容</returns>
	public async Task<OperateResult<string>> PostAsync(string rawUrl, string body)
	{
		return await webApi.PostAsync(rawUrl, body);
	}

	public override string ToString()
	{
		return $"DeviceWebApi[{IpAddress}:{Port}]";
	}
}
