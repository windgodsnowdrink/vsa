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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.LogNet;

namespace HslCommunication.Core.Net;

/// <summary>
/// 基于webapi的数据访问的基类，提供了基本的http接口的交互功能
/// A base class for data access based on webapi that provides basic HTTP interface interaction
/// </summary>
/// <remarks>
/// 当前的基类在.net framework2.0上存在问题，在.net framework4.5及.net standard上运行稳定而且正常
/// </remarks>
public class NetworkWebApiBase
{
	private string host = "127.0.0.1";

	private string ipAddress = "127.0.0.1";

	private int port = 80;

	private string name = string.Empty;

	private string password = string.Empty;

	private HttpClient httpClient;

	/// <summary>
	/// 针对请求的头信息进行额外的处理
	/// </summary>
	public Action<HttpContentHeaders> AddRequestHeadersAction { get; set; }

	/// <summary>
	/// 获取或设置远程服务器的IP地址
	/// Gets or sets the IP address of the remote server
	/// </summary>
	public string IpAddress
	{
		get
		{
			return ipAddress;
		}
		set
		{
			host = value;
			ipAddress = HslHelper.GetIpAddressFromInput(value);
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
			return port;
		}
		set
		{
			port = value;
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
			return name;
		}
		set
		{
			name = value;
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
			return password;
		}
		set
		{
			password = value;
		}
	}

	/// <summary>
	/// 组件的日志工具，支持日志记录，只要实例化后，当前网络的基本信息，就以等级进行输出
	/// The component's logging tool supports logging. As long as the instantiation of the basic network information, the output will be output at
	/// </summary>
	/// <remarks>
	/// 只要实例化即可以记录日志，实例化的对象需要实现接口  ，本组件提供了三个日志记录类，你可以实现基于   的对象。
	/// </remarks>
	public ILogNet LogNet { get; set; }

	/// <summary>
	/// 是否启用Https的协议访问，对于Https来说，端口号默认为 443
	/// Whether to enable Https protocol access, for Https, the port number defaults to 443
	/// </summary>
	public bool UseHttps { get; set; }

	/// <summary>
	/// 默认的内容类型，如果为空，则不进行设置操作。例如设置为 "text/plain", "application/json", "text/html" 等等。
	/// The default content type, if it is empty, no setting operation will be performed. For example, set to "text/plain", "application/json", "text/html" and so on.
	/// </summary>
	public string DefaultContentType { get; set; }

	/// <summary>
	/// 获取或设置是否使用ISO的编码信息，默认为 False
	/// Get or set whether to use ISO encoding information, the default is False
	/// </summary>
	/// <remarks>
	/// 在访问某些特殊的API的时候，会发生异常"The character set provided in ContentType is invalid...."，这时候，只需要将本属性设置为 True 即可。
	/// </remarks>
	public bool UseEncodingISO { get; set; } = false;

	public HttpClient Client => httpClient;

	/// <summary>
	/// 获取或设置当前的超时时间，单位为毫秒
	/// Get or set the current timeout period, with the unit being milliseconds
	/// </summary>
	public int Timeout
	{
		get
		{
			return (int)httpClient.Timeout.TotalMilliseconds;
		}
		set
		{
			httpClient.Timeout = TimeSpan.FromMilliseconds(value);
		}
	}

	public string Host => host;

	/// <summary>
	/// 使用指定的ip地址来初始化对象
	/// Initializes the object using the specified IP address
	/// </summary>
	/// <param name="ipAddress">Ip地址信息</param>
	public NetworkWebApiBase(string ipAddress)
		: this(ipAddress, 80, string.Empty, string.Empty)
	{
	}

	/// <summary>
	/// 使用指定的ip地址及端口号来初始化对象
	/// Initializes the object with the specified IP address and port number
	/// </summary>
	/// <param name="ipAddress">Ip地址信息</param>
	/// <param name="port">端口号信息</param>
	public NetworkWebApiBase(string ipAddress, int port)
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
	public NetworkWebApiBase(string ipAddress, int port, string name, string password)
	{
		host = ipAddress;
		this.ipAddress = HslHelper.GetIpAddressFromInput(ipAddress);
		this.port = port;
		this.name = name;
		this.password = password;
		if (!string.IsNullOrEmpty(name))
		{
			HttpClientHandler httpClientHandler = new HttpClientHandler
			{
				Credentials = new NetworkCredential(name, password)
			};
			httpClientHandler.Proxy = null;
			httpClientHandler.UseProxy = false;
			ServicePointManager.ServerCertificateValidationCallback = TrustAllValidationCallback;
			httpClient = new HttpClient(httpClientHandler);
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		}
		else
		{
			ServicePointManager.ServerCertificateValidationCallback = TrustAllValidationCallback;
			httpClient = new HttpClient();
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		}
	}

	private bool TrustAllValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
	{
		return true;
	}

	/// <summary>
	/// 针对请求的头信息进行额外的处理，可以重写用来实现一些特殊的信息添加到请求头中
	/// </summary>
	/// <param name="headers">头信息</param>
	protected virtual void AddRequestHeaders(HttpContentHeaders headers)
	{
	}

	/// <summary>
	/// 根据当前的url信息，获取到完整的url地址，如果没有http头信息，则自动添加上
	/// Based on the current url information, the complete url address is obtained. If there is no http header information, it will be automatically added
	/// </summary>
	/// <param name="url">url地址</param>
	/// <returns>完整的url地址</returns>
	public string GetEntireUrl(string url)
	{
		if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
		{
			return url;
		}
		return string.Format("{0}://{1}:{2}/{3}", UseHttps ? "https" : "http", host, port, url.StartsWith("/") ? url.Substring(1) : url);
	}

	/// <summary>
	/// 使用GET操作从网络中获取到数据信息，地址
	/// </summary>
	/// <param name="rawUrl">除去ip地址和端口的地址</param>
	/// <returns>返回的数据内容</returns>
	public OperateResult<string> Get(string rawUrl)
	{
		string entireUrl = GetEntireUrl(rawUrl);
		try
		{
			return GetAsync(rawUrl).Result;
		}
		catch (Exception ex)
		{
			return new OperateResult<string>(ex.Message);
		}
	}

	/// <summary>
	/// 使用POST命令去提交数据内容，然后返回相关的数据信息
	/// </summary>
	/// <param name="rawUrl">已经去除ip地址，端口号的api信息</param>
	/// <param name="body">数据内容</param>
	/// <returns>从服务器返回的内容</returns>
	public OperateResult<string> Post(string rawUrl, string body)
	{
		string entireUrl = GetEntireUrl(rawUrl);
		try
		{
			return PostAsync(rawUrl, body).Result;
		}
		catch (Exception ex)
		{
			return new OperateResult<string>(ex.Message + Environment.NewLine + "Url: " + entireUrl);
		}
	}

	/// <summary>
	/// 生成 HTTP 基本认证的 Authorization 头值
	/// </summary>
	/// <param name="username">用户名</param>
	/// <param name="password">密码</param>
	/// <returns>Basic + Base64编码字符串</returns>
	private static string GenerateBasicAuthHeader(string username, string password)
	{
		string s = username + ":" + password;
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		string text = Convert.ToBase64String(bytes);
		return "Basic " + text;
	}

	/// <summary>
	/// 发起一个请求，然后返回结果，支持任意的http方法
	/// Initiate a request and then return the result, supporting any http method
	/// </summary>
	/// <param name="httpMethod">请求方法</param>
	/// <param name="rawUrl">url信息</param>
	/// <param name="body">body数据</param>
	/// <param name="contentType">内容数据格式</param>
	/// <returns>返回的结果数据</returns>
	public async Task<OperateResult<string>> RequestAsync(HttpMethod httpMethod, string rawUrl, HttpContent body, string contentType = "none")
	{
		string url = GetEntireUrl(rawUrl);
		DateTime dateTime = DateTime.Now;
		try
		{
			using HttpRequestMessage request = new HttpRequestMessage(httpMethod, url);
			if (!string.IsNullOrEmpty(name) && !request.Headers.Contains("Authorization"))
			{
				request.Headers.Add("Authorization", GenerateBasicAuthHeader(name, password));
			}
			if (body != null)
			{
				AddRequestHeaders(body.Headers);
				AddRequestHeadersAction?.Invoke(body.Headers);
			}
			if (httpMethod != HttpMethod.Get)
			{
				request.Content = body;
				if (!string.IsNullOrEmpty(contentType) && contentType != "none")
				{
					request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
				}
			}
			using HttpResponseMessage response = await Client.SendAsync(request).ConfigureAwait(continueOnCapturedContext: false);
			using HttpContent content = response.Content;
			response.EnsureSuccessStatusCode();
			if (UseEncodingISO)
			{
				using (StreamReader sr = new StreamReader(await content.ReadAsStreamAsync().ConfigureAwait(continueOnCapturedContext: false), Encoding.GetEncoding("iso-8859-1")))
				{
					return OperateResult.CreateSuccessResult(await sr.ReadToEndAsync().ConfigureAwait(continueOnCapturedContext: false));
				}
			}
			return OperateResult.CreateSuccessResult(await content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext: false));
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			return new OperateResult<string>(ex2.Message + Environment.NewLine + $"Timecost: {(DateTime.Now - dateTime).TotalMilliseconds:F0}ms  Url: " + url);
		}
	}

	/// <summary>
	/// 发起一个请求，然后返回结果，支持任意的http方法
	/// Initiate a request and then return the result, supporting any http method
	/// </summary>
	/// <param name="httpMethod">请求方法</param>
	/// <param name="rawUrl">url信息</param>
	/// <param name="body">body数据</param>
	/// <param name="contentType">内容数据格式</param>
	/// <returns>返回的结果数据</returns>
	public async Task<OperateResult<string>> RequestAsync(HttpMethod httpMethod, string rawUrl, string body, string contentType = "application/json")
	{
		if (string.IsNullOrEmpty(body))
		{
			body = "";
		}
		return await RequestAsync(httpMethod, rawUrl, new StringContent(body), contentType).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 发起一个请求，然后返回结果，支持任意的http方法
	/// Initiate a request and then return the result, supporting any http method
	/// </summary>
	/// <param name="httpMethod">请求方法</param>
	/// <param name="rawUrl">url信息</param>
	/// <param name="body">body数据</param>
	/// <param name="contentType">内容数据格式</param>
	/// <returns>返回的结果数据</returns>
	public async Task<OperateResult<string>> RequestAsync(HttpMethod httpMethod, string rawUrl, MultipartFormDataContent body)
	{
		return await RequestAsync(httpMethod, rawUrl, body, "multipart/form-data").ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 使用GET操作从网络中获取到数据信息，地址
	/// </summary>
	/// <param name="rawUrl">除去ip地址和端口的地址</param>
	/// <returns>返回的数据内容</returns>
	public async Task<OperateResult<string>> GetAsync(string rawUrl)
	{
		HttpContent httpContent = null;
		return await RequestAsync(HttpMethod.Get, rawUrl, httpContent).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 使用POST命令去提交数据内容，然后返回相关的数据信息
	/// </summary>
	/// <param name="rawUrl">已经去除ip地址，端口号的api信息</param>
	/// <param name="body">数据内容</param>
	/// <returns>从服务器返回的内容</returns>
	public async Task<OperateResult<string>> PostAsync(string rawUrl, string body)
	{
		return await RequestAsync(HttpMethod.Post, rawUrl, new StringContent(body), DefaultContentType).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override string ToString()
	{
		return $"NetworkWebApiBase[{ipAddress}:{port}]";
	}
}
