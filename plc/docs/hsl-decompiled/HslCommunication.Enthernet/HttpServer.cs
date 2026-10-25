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
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.LogNet;
using HslCommunication.MQTT;
using HslCommunication.Reflection;
using Newtonsoft.Json;

namespace HslCommunication.Enthernet;

/// <summary>
/// 一个支持完全自定义的Http服务器，支持返回任意的数据信息，方便调试信息，详细的案例请查看API文档信息
/// A Http server that supports fully customized, supports returning arbitrary data information, which is convenient for debugging information. For detailed cases, please refer to the API documentation information
/// </summary>
/// <remarks>
/// 使用RPC接口注册的方式，可以更加便捷快速的实现webapi接口创建及设计，自带接口列表浏览查看，注释查看，签名查看，甚至调用次数及耗时查看。
/// Using the RPC interface registration method, you can more conveniently and quickly realize the creation and design of webapi interfaces, browse and view the built-in interface list,
/// view comments, view signatures, and even view the number of calls and time-consuming.
/// </remarks>
public class HttpServer
{
	private Dictionary<string, MqttRpcApiInfo> apiTopicServiceDict;

	private object rpcApiLock;

	private int receiveBufferSize = 2048;

	private int port = 80;

	private HttpListener listener;

	private ILogNet logNet;

	private Encoding encoding = Encoding.UTF8;

	private Func<HttpListenerRequest, HttpListenerResponse, string, object> handleRequestFunc;

	private LogStatisticsDict statisticsDict;

	private bool loginAccess = false;

	private MqttCredential[] loginCredentials;

	private Action<HttpApiCalledInfo> apiCalledAction;

	private bool useHttps = false;

	/// <summary>
	/// 额外的处理请求信息的委托定义，将可以自定义处理一些特殊的请求头数据，例如一些账户相关的其他属性，语言属性等等。
	/// Additional delegate definitions for processing request information will be able to customize some special request header data,
	/// such as some other account-related attributes, language attributes, and so on.
	/// </summary>
	public Action<HttpListenerRequest, ISessionContext> DealWithHttpListenerRequest { get; set; }

	public LogStatisticsDict LogStatistics => statisticsDict;

	/// <summary>
	/// 组件的日志工具，支持日志记录，只要实例化后，当前网络的基本信息，就以等级进行输出
	/// The component's logging tool supports logging. As long as the instantiation of the basic network information, the output will be output at
	/// </summary>
	/// <remarks>
	/// 只要实例化即可以记录日志，实例化的对象需要实现接口  ，本组件提供了三个日志记录类，你可以实现基于   的对象。
	/// </remarks>
	public ILogNet LogNet
	{
		get
		{
			return logNet;
		}
		set
		{
			logNet = value;
		}
	}

	/// <summary>
	/// 获取或设置当前服务器的编码信息，默认为UTF8编码
	/// Get or set the encoding information of the current server, the default is UTF8 encoding
	/// </summary>
	public Encoding ServerEncoding
	{
		get
		{
			return encoding;
		}
		set
		{
			encoding = value;
		}
	}

	/// <summary>
	/// 获取或设置是否支持跨域操作
	/// Get or set whether to support cross-domain operations
	/// </summary>
	public bool IsCrossDomain { get; set; } = true;

	/// <summary>
	/// 获取或设置是否记录HTTP的请求内容，默认不记录
	/// Get or set whether to record the HTTP request content. By default, it is not recorded
	/// </summary>
	public bool LogHttpBody { get; set; } = false;

	/// <summary>
	/// 获取或设置是否记录HTTP的请求头信息，默认不记录
	/// Get or set whether to record the HTTP header content. By default, it is not recorded
	/// </summary>
	public bool LogHttpHeader { get; set; } = false;

	/// <summary>
	/// 获取或设置当前的自定义的处理信息，如果不想继承实现方法，可以使用本属性来关联你自定义的方法。
	/// Get or set the current custom processing information. If you don't want to inherit the implementation method, you can use this attribute to associate your custom method.
	/// </summary>
	public Func<HttpListenerRequest, HttpListenerResponse, string, object> HandleRequestFunc
	{
		get
		{
			return handleRequestFunc;
		}
		set
		{
			handleRequestFunc = value;
		}
	}

	/// <summary>
	/// 获取或设置当前的自定义处理文件上传的信息，自动解析好文件的基本信息
	/// Obtain or set the current custom processing file upload information, and automatically parse the basic information of the file
	/// </summary>
	public Func<HttpListenerRequest, HttpListenerResponse, HttpUploadFile, string> HandleFileUpload { get; set; }

	public int Port => port;

	/// <summary>
	/// 获取或设置当前接口调用信息处理的委托，可以用于对接口调用的二次分析，在接口调用完成的时候，将触发本委托
	/// Obtain or set the delegate for the current interface call information processing, which can be used for secondary analysis of the interface call, and the delegate will be triggered when the interface call is completed
	/// </summary>
	public Action<HttpApiCalledInfo> ApiCalledAction
	{
		get
		{
			return apiCalledAction;
		}
		set
		{
			apiCalledAction = value;
		}
	}

	/// <summary>
	/// 实例化一个默认的对象，当前的运行，需要使用管理员的模式运行
	/// Instantiate a default object, the current operation, you need to use the administrator mode to run
	/// </summary>
	public HttpServer()
	{
		statisticsDict = new LogStatisticsDict(GenerateMode.ByEveryDay, 60);
		apiTopicServiceDict = new Dictionary<string, MqttRpcApiInfo>();
		rpcApiLock = new object();
	}

	/// <summary>
	/// 使用HTTPS模式，关于如何使用证书，具体的教程可以参考：http://www.hsltechnology.cn/Doc/HslCommunication?chapter=HslCommChapter6-5
	/// For more information about how to use the certificate in the HTTPS mode, please refer to http://www.hsltechnology.cn/Doc/HslCommunication?chapter=HslCommChapter6-5
	/// </summary>
	public void UseHttps()
	{
		useHttps = true;
	}

	/// <summary>
	/// 启动服务器，正常调用该方法时，应该使用try...catch...来捕获错误信息
	/// Start the server and use try...catch... to capture the error message when calling this method normally
	/// </summary>
	/// <param name="port">端口号信息</param>
	public void Start(int port)
	{
		this.port = port;
		listener = new HttpListener();
		if (useHttps)
		{
			listener.Prefixes.Add($"https://+:{port}/");
			listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
		}
		else
		{
			listener.Prefixes.Add($"http://+:{port}/");
		}
		listener.Start();
		listener.BeginGetContext(GetConnectCallBack, listener);
		logNet?.WriteDebug(ToString(), "Server Started, wait for connections");
	}

	/// <summary>
	/// 启动服务器，正常调用该方法时，应该使用try...catch...来捕获错误信息
	/// </summary>
	/// <param name="prefix">例如 http://127.0.0.1:8000/</param>
	public void Start(string prefix)
	{
		listener = new HttpListener();
		listener.Prefixes.Add(prefix);
		if (prefix.StartsWith("https:", StringComparison.OrdinalIgnoreCase))
		{
			useHttps = true;
			listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
		}
		listener.Start();
		listener.BeginGetContext(GetConnectCallBack, listener);
		logNet?.WriteDebug(ToString(), "Server Started, wait for connections");
	}

	/// <summary>
	/// 关闭服务器
	/// Shut down the server
	/// </summary>
	public void Close()
	{
		listener?.Close();
	}

	private async void GetConnectCallBack(IAsyncResult ar)
	{
		object asyncState = ar.AsyncState;
		if (!(asyncState is HttpListener listener))
		{
			return;
		}
		HttpListenerContext context = null;
		try
		{
			context = listener.EndGetContext(ar);
		}
		catch (ObjectDisposedException)
		{
			return;
		}
		catch (Exception ex2)
		{
			Exception ex3 = ex2;
			logNet?.WriteException(ToString(), ex3);
		}
		int restartcount = 0;
		while (true)
		{
			try
			{
				listener.BeginGetContext(GetConnectCallBack, listener);
			}
			catch (Exception ex2)
			{
				Exception ex4 = ex2;
				logNet?.WriteException(ToString(), ex4);
				restartcount++;
				if (restartcount >= 3)
				{
					logNet?.WriteError(ToString(), "ReGet Content Failed!");
					return;
				}
				HslHelper.ThreadSleep(1000);
				continue;
			}
			break;
		}
		if (context == null)
		{
			return;
		}
		HttpListenerRequest request = context.Request;
		HttpListenerResponse response = context.Response;
		if (response != null)
		{
			try
			{
				if (IsCrossDomain)
				{
					context.Response.AppendHeader("Access-Control-Allow-Origin", request.Headers["Origin"]);
					context.Response.AppendHeader("Access-Control-Allow-Headers", "*");
					context.Response.AppendHeader("Access-Control-Allow-Method", "POST,GET,PUT,OPTIONS,DELETE");
					context.Response.AppendHeader("Access-Control-Allow-Credentials", "true");
					context.Response.AppendHeader("Access-Control-Max-Age", "3600");
				}
				context.Response.AddHeader("Content-type", "text/html; charset=utf-8");
			}
			catch (Exception ex2)
			{
				Exception ex5 = ex2;
				logNet?.WriteError(ToString(), ex5.Message);
			}
		}
		byte[] data = await GetDataFromRequestAsync(request);
		try
		{
			response.StatusCode = 200;
			DateTime start = DateTime.Now;
			object ret = await HandleRequest(request, response, data);
			StringBuilder stringBuilder = new StringBuilder();
			if (LogHttpHeader)
			{
				stringBuilder.AppendLine("Header Request=======================");
				if (request.Headers != null)
				{
					string[] allKeys = request.Headers.AllKeys;
					foreach (string key in allKeys)
					{
						stringBuilder.AppendLine(key + " : " + request.Headers[key]);
					}
				}
			}
			if (LogHttpBody)
			{
				stringBuilder.AppendLine("Body Request=========================");
				stringBuilder.AppendLine(Encoding.UTF8.GetString(data));
			}
			if (LogHttpHeader)
			{
				stringBuilder.AppendLine("Header Response =======================");
				if (response.Headers != null)
				{
					string[] allKeys2 = response.Headers.AllKeys;
					foreach (string key2 in allKeys2)
					{
						stringBuilder.AppendLine(key2 + " : " + response.Headers[key2]);
					}
				}
			}
			if (ret == null)
			{
				return;
			}
			using (Stream stream = response.OutputStream)
			{
				if (ret is string ret_str)
				{
					if (string.IsNullOrEmpty(ret_str))
					{
						await stream.WriteAsync(new byte[0], 0, 0);
					}
					else
					{
						byte[] buffer = encoding.GetBytes(ret_str);
						await stream.WriteAsync(buffer, 0, buffer.Length);
					}
					if (LogHttpBody)
					{
						stringBuilder.AppendLine("Body Response ==========================");
						stringBuilder.AppendLine(ret_str);
					}
				}
				else if (ret is byte[] ret_bytes)
				{
					response.ContentLength64 = ret_bytes.Length;
					await stream.WriteAsync(ret_bytes, 0, ret_bytes.Length);
					if (LogHttpBody)
					{
						stringBuilder.AppendLine("Body Response ==========================");
						stringBuilder.AppendLine(SoftBasic.ByteToHexString(ret_bytes));
					}
				}
			}
			logNet?.WriteDebug(ToString(), $"Request [{request.HttpMethod}], Url: {HttpUtility.UrlDecode(request.RawUrl)} Timecost: {(DateTime.Now - start).TotalMilliseconds:F1}");
			if (LogHttpBody || LogHttpHeader)
			{
				logNet?.WriteDebug(ToString(), stringBuilder.ToString());
			}
		}
		catch (Exception ex6)
		{
			logNet?.WriteException(ToString(), "Request[" + request.HttpMethod + "], " + request.RawUrl, ex6);
		}
	}

	private byte[] GetDataFromRequest(HttpListenerRequest request)
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream();
			byte[] buffer = new byte[receiveBufferSize];
			int num = 0;
			do
			{
				num = request.InputStream.Read(buffer, 0, buffer.Length);
				if (num > 0)
				{
					memoryStream.Write(buffer, 0, num);
				}
			}
			while (num != 0);
			return memoryStream.ToArray();
		}
		catch
		{
			return new byte[0];
		}
	}

	private async Task<byte[]> GetDataFromRequestAsync(HttpListenerRequest request)
	{
		try
		{
			MemoryStream ms = new MemoryStream();
			byte[] byteArr = new byte[receiveBufferSize];
			int readLen;
			do
			{
				readLen = await request.InputStream.ReadAsync(byteArr, 0, byteArr.Length);
				if (readLen > 0)
				{
					ms.Write(byteArr, 0, readLen);
				}
			}
			while (readLen != 0);
			return ms.ToArray();
		}
		catch
		{
			return new byte[0];
		}
	}

	/// <summary>
	/// 根据客户端的请求进行处理的核心方法，可以返回自定义的数据内容，只需要集成重写即可。
	/// The core method of processing according to the client's request can return custom data content, and only needs to be integrated and rewritten.
	/// </summary>
	/// <param name="request">请求</param>
	/// <param name="response">回应</param>
	/// <param name="data">Body数据</param>
	/// <returns>返回的内容</returns>
	protected virtual async Task<object> HandleRequest(HttpListenerRequest request, HttpListenerResponse response, byte[] data)
	{
		if (request.HttpMethod == "OPTIONS")
		{
			return "OK";
		}
		if (loginAccess)
		{
			string[] values = request.Headers.GetValues("Authorization");
			if (values == null || values.Length < 1 || string.IsNullOrEmpty(values[0]))
			{
				response.StatusCode = 401;
				response.AddHeader("WWW-Authenticate", "Basic realm=\"Secure Area\"");
				return "";
			}
			try
			{
				string base64String = values[0].Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
				string accountString = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));
				string[] account = accountString.Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);
				if (account.Length < 2)
				{
					response.StatusCode = 401;
					response.AddHeader("WWW-Authenticate", "Basic realm=\"Secure Area\"");
					return "";
				}
				MqttCredential[] credentials = loginCredentials;
				bool loginEnable = false;
				for (int i = 0; i < credentials.Length; i++)
				{
					if (account[0] == credentials[i].UserName && account[1] == credentials[i].Password)
					{
						loginEnable = true;
						break;
					}
				}
				if (!loginEnable)
				{
					response.StatusCode = 401;
					response.AddHeader("WWW-Authenticate", "Basic realm=\"Secure Area\"");
					return "";
				}
			}
			catch
			{
				response.StatusCode = 401;
				response.AddHeader("WWW-Authenticate", "Basic realm=\"Secure Area\"");
				return "";
			}
		}
		if (request.HttpMethod == "HSL")
		{
			if (request.RawUrl.StartsWith("/Apis"))
			{
				response.AddHeader("Content-type", "application/json; charset=utf-8");
				return GetAllRpcApiInfo().ToJsonString((Formatting)1);
			}
			if (request.RawUrl.StartsWith("/Logs"))
			{
				response.AddHeader("Content-type", "application/json; charset=utf-8");
				if (request.RawUrl == "/Logs" || request.RawUrl == "/Logs/")
				{
					return LogStatistics.LogStat.GetStatisticsSnapshot().ToJsonString((Formatting)1);
				}
				return LogStatistics.GetStatisticsSnapshot(request.RawUrl.Substring(6)).ToJsonString((Formatting)1);
			}
			response.AddHeader("Content-type", "application/json; charset=utf-8");
			return GetAllRpcApiInfo().ToJsonString((Formatting)1);
		}
		if (request.HttpMethod == "OPTIONS")
		{
			return "OK";
		}
		MqttRpcApiInfo apiInformation = GetMqttRpcApiInfo(GetMethodName(HttpUtility.UrlDecode(request.RawUrl)));
		if (apiInformation == null)
		{
			if (request.ContentType != null && request.ContentType.StartsWith("multipart/form-data; boundary=--------------------------"))
			{
				int index = -1;
				for (int j = 0; j < data.Length - 4; j++)
				{
					if (data[j] == 13 && data[j + 1] == 10 && data[j + 2] == 13 && data[j + 3] == 10)
					{
						index = j + 4;
						break;
					}
				}
				if (index == -1)
				{
					return "Not file content!";
				}
				int last = data.Length - 3;
				for (int i2 = last; i2 > 0; i2--)
				{
					if (data[i2] == 13 && data[i2 + 1] == 10)
					{
						last = i2;
						break;
					}
				}
				if (HandleFileUpload != null)
				{
					string context = encoding.GetString(data, 0, index - 4);
					HttpUploadFile uploadFile = new HttpUploadFile
					{
						FileName = SoftBasic.UrlDecode(Regex.Match(context, "filename=\"[^\"]+").Value.Substring(10), Encoding.UTF8),
						Name = Regex.Match(context, "name=\"[^\"]+").Value.Substring(6),
						Content = data.SelectMiddle(index, last - index)
					};
					return HandleFileUpload(request, response, uploadFile);
				}
			}
			else if (HandleRequestFunc != null)
			{
				return HandleRequestFunc(request, response, encoding.GetString(data));
			}
			return "This is HslWebServer, Thank you for use!";
		}
		response.AddHeader("Content-type", "application/json; charset=utf-8");
		DateTime dateTime = DateTime.Now;
		string url = HttpUtility.UrlDecode(request.RawUrl);
		string body = encoding.GetString(data);
		string result = await HandleObjectMethod(request, url, body, apiInformation, DealWithHttpListenerRequest);
		double timeSpend = Math.Round((DateTime.Now - dateTime).TotalSeconds, 5);
		apiInformation.CalledCountAddOne((long)(timeSpend * 100000.0));
		statisticsDict.StatisticsAdd(apiInformation.ApiTopic, 1L);
		if (apiCalledAction != null)
		{
			HttpApiCalledInfo httpApiCalledInfo = new HttpApiCalledInfo
			{
				HttpMethod = request.HttpMethod,
				Url = url,
				Body = body,
				Result = result,
				CostTime = timeSpend * 1000.0,
				CalledCount = apiInformation.CalledCount
			};
			apiCalledAction?.Invoke(httpApiCalledInfo);
		}
		return result;
	}

	private MqttRpcApiInfo GetMqttRpcApiInfo(string apiTopic)
	{
		MqttRpcApiInfo result = null;
		lock (rpcApiLock)
		{
			if (apiTopicServiceDict.ContainsKey(apiTopic))
			{
				result = apiTopicServiceDict[apiTopic];
			}
		}
		return result;
	}

	private void MqttRpcAdd(string apiTopic, MqttRpcApiInfo apiInfo)
	{
		if (apiTopicServiceDict.ContainsKey(apiTopic))
		{
			apiTopicServiceDict[apiTopic] = apiInfo;
		}
		else
		{
			apiTopicServiceDict.Add(apiTopic, apiInfo);
		}
	}

	private bool MqttRpcRemove(string apiTopic)
	{
		if (apiTopicServiceDict.ContainsKey(apiTopic))
		{
			return apiTopicServiceDict.Remove(apiTopic);
		}
		return false;
	}

	/// <summary>
	/// 获取当前所有注册的RPC接口信息，将返回一个数据列表。
	/// Get all currently registered RPC interface information, and a data list will be returned.
	/// </summary>
	/// <returns>信息列表</returns>
	public MqttRpcApiInfo[] GetAllRpcApiInfo()
	{
		MqttRpcApiInfo[] result = null;
		lock (rpcApiLock)
		{
			result = apiTopicServiceDict.Values.ToArray();
		}
		return result;
	}

	/// <summary>
	/// 注册一个RPC的服务接口，可以指定当前的控制器名称，以及提供RPC服务的原始对象
	/// Register an RPC service interface, you can specify the current controller name, and the original object that provides the RPC service
	/// </summary>
	/// <param name="api">前置的接口信息，可以理解为MVC模式的控制器</param>
	/// <param name="obj">原始对象信息，也可以是类型对象本身</param>
	public void RegisterHttpRpcApi(string api, object obj)
	{
		lock (rpcApiLock)
		{
			foreach (MqttRpcApiInfo item in MqttHelper.GetSyncServicesApiInformationFromObject(api, obj))
			{
				MqttRpcAdd(item.ApiTopic, item);
			}
		}
	}

	/// <summary>
	/// 注册一个RPC的服务接口，可以指定当前的控制器名称，以及提供RPC服务的原始对象
	/// Register an RPC service interface, you can specify the current controller name, and the original object that provides the RPC service
	/// </summary>
	/// <param name="api">前置的接口信息，可以理解为MVC模式的控制器</param>
	/// <param name="obj">原始对象信息，也可以是类型对象本身</param>
	public void RegisterHttpRpcApi(object obj)
	{
		lock (rpcApiLock)
		{
			foreach (MqttRpcApiInfo item in MqttHelper.GetSyncServicesApiInformationFromObject(obj))
			{
				MqttRpcAdd(item.ApiTopic, item);
			}
		}
	}

	/// <summary>
	/// 卸载一个已经注册的RPC接口，想要卸载指定的接口，此处就需要传入注册时一样的参数
	/// Uninstall a registered RPC interface. If you want to uninstall the specified interface, you need to pass in the same parameters as the registration
	/// </summary>
	/// <param name="api">前置的接口信息，可以理解为MVC模式的控制器</param>
	/// <param name="obj">原始对象信息，也可以是类型对象本身</param>
	public void UnRegisterHttpRpcApi(string api, object obj)
	{
		lock (rpcApiLock)
		{
			foreach (MqttRpcApiInfo item in MqttHelper.GetSyncServicesApiInformationFromObject(api, obj))
			{
				MqttRpcRemove(item.ApiTopic);
			}
		}
	}

	/// <summary>
	/// 卸载一个已经注册的RPC接口，想要卸载指定的接口，此处就需要传入注册时一样的参数
	/// Uninstall a registered RPC interface. If you want to uninstall the specified interface, you need to pass in the same parameters as the registration
	/// </summary>
	/// <param name="api">前置的接口信息，可以理解为MVC模式的控制器</param>
	/// <param name="obj">原始对象信息，也可以是类型对象本身</param>
	public void UnRegisterHttpRpcApi(object obj)
	{
		lock (rpcApiLock)
		{
			foreach (MqttRpcApiInfo item in MqttHelper.GetSyncServicesApiInformationFromObject(obj))
			{
				MqttRpcRemove(item.ApiTopic);
			}
		}
	}

	/// <summary>
	/// 卸载一个指定的RPC接口，需要指定唯一的接口名称，如果接口重命名了，需要使用实际访问的时候的接口名称，返回是否成功卸载接口
	/// To uninstall a specified RPC interface, you need to specify a unique interface name. If the interface is renamed,
	/// you need to use the interface name during the actual access, and return whether the interface is successfully uninstalled
	/// </summary>
	/// <param name="apiTopic">接口的唯一名称</param>
	/// <returns>是否卸载成功</returns>
	public bool UnRegisterHttpRpcApiSingle(string apiTopic)
	{
		lock (rpcApiLock)
		{
			return MqttRpcRemove(apiTopic);
		}
	}

	/// <summary>
	/// 设置登录的账户信息，如果需要自己控制，可以自己实现委托
	/// Set the login account information, if you need to control by yourself, you can implement the delegation by yourself
	/// </summary>
	/// <param name="credentials">用户名的列表信息</param>
	public void SetLoginAccessControl(MqttCredential[] credentials)
	{
		if (credentials == null)
		{
			loginAccess = false;
			return;
		}
		if (credentials.Length == 0)
		{
			loginAccess = false;
			return;
		}
		loginAccess = true;
		loginCredentials = credentials;
	}

	public override string ToString()
	{
		return $"HttpServer[{port}]";
	}

	/// <summary>
	/// 使用指定的对象来返回网络的API接口，前提是传入的数据为json参数，返回的数据为json数据，详细参照说明
	/// Use the specified object to return the API interface of the network,
	/// provided that the incoming data is json parameters and the returned data is json data,
	/// please refer to the description for details
	/// </summary>
	/// <param name="request">当前的请求信息</param>
	/// <param name="deceodeUrl">已经解码过的Url地址信息</param>
	/// <param name="json">json格式的参数信息</param>
	/// <param name="obj">等待解析的api解析的对象</param>
	/// <param name="action">额外的解析Request参数的方法</param>
	/// <returns>等待返回客户的结果</returns>
	public static async Task<string> HandleObjectMethod(HttpListenerRequest request, string deceodeUrl, string json, object obj, Action<HttpListenerRequest, ISessionContext> action)
	{
		string method = GetMethodName(deceodeUrl);
		if (method.LastIndexOf('/') >= 0)
		{
			method = method.Substring(method.LastIndexOf('/') + 1);
		}
		MethodInfo methodInfo = obj.GetType().GetMethod(method);
		if (methodInfo == null)
		{
			return new OperateResult<string>("Current MqttSync Api ：[" + method + "] not exsist").ToJsonString((Formatting)1);
		}
		OperateResult<MqttRpcApiInfo> apiResult = MqttHelper.GetMqttSyncServicesApiFromMethod("", methodInfo, obj);
		if (!apiResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(apiResult).ToJsonString((Formatting)1);
		}
		return await HandleObjectMethod(request, deceodeUrl, json, apiResult.Content, action);
	}

	/// <summary>
	/// 根据完整的地址获取当前的url地址信息
	/// </summary>
	/// <param name="url">地址信息</param>
	/// <returns>方法名称</returns>
	public static string GetMethodName(string url)
	{
		string empty = string.Empty;
		empty = ((url.IndexOf('?') <= 0) ? url : url.Substring(0, url.IndexOf('?')));
		if (empty.EndsWith("/") || empty.StartsWith("/"))
		{
			empty = empty.Trim('/');
		}
		return empty;
	}

	private static ISessionContext GetSessionContextFromHeaders(HttpListenerRequest request, Action<HttpListenerRequest, ISessionContext> userParse)
	{
		try
		{
			string[] values = request.Headers.GetValues("Authorization");
			if (values == null || values.Length < 1 || string.IsNullOrEmpty(values[0]))
			{
				return null;
			}
			string[] array = values[0].Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length < 2)
			{
				return null;
			}
			string s = array[1];
			string message = Encoding.UTF8.GetString(Convert.FromBase64String(s));
			string[] array2 = message.Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);
			if (array2.Length < 1)
			{
				return null;
			}
			SessionContext sessionContext = new SessionContext
			{
				UserName = array2[0]
			};
			userParse?.Invoke(request, sessionContext);
			return sessionContext;
		}
		catch
		{
			return null;
		}
	}

	/// <summary>
	/// 使用指定的对象来返回网络的API接口，前提是传入的数据为json参数，返回的数据为json数据，详细参照说明
	/// Use the specified object to return the API interface of the network,
	/// provided that the incoming data is json parameters and the returned data is json data,
	/// please refer to the description for details
	/// </summary>
	/// <param name="request">当前的请求信息</param>
	/// <param name="deceodeUrl">已经解码过的Url地址信息</param>
	/// <param name="json">json格式的参数信息</param>
	/// <param name="apiInformation">等待解析的api解析的对象</param>
	/// <param name="action">额外的解析Request参数的方法</param>
	/// <returns>等待返回客户的结果</returns>
	public static async Task<string> HandleObjectMethod(HttpListenerRequest request, string deceodeUrl, string json, MqttRpcApiInfo apiInformation, Action<HttpListenerRequest, ISessionContext> action)
	{
		ISessionContext context = GetSessionContextFromHeaders(request, action);
		if (apiInformation.PermissionAttribute != null)
		{
			if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
			{
				return new OperateResult<string>("Permission function need authorization ：" + StringResources.Language.InsufficientPrivileges).ToJsonString((Formatting)1);
			}
			try
			{
				string[] values = request.Headers.GetValues("Authorization");
				if (values == null || values.Length < 1 || string.IsNullOrEmpty(values[0]))
				{
					return new OperateResult<string>("Mqtt RPC Api ：[" + apiInformation.ApiTopic + "] has none Authorization information, access not permission").ToJsonString((Formatting)1);
				}
				string base64String = values[0].Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
				string accountString = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));
				string[] account = accountString.Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);
				if (account.Length < 1)
				{
					return new OperateResult<string>("Mqtt RPC Api ：[" + apiInformation.ApiTopic + "] has none Username information, access not permission").ToJsonString((Formatting)1);
				}
				if (!apiInformation.PermissionAttribute.CheckUserName(account[0]))
				{
					return new OperateResult<string>("Mqtt RPC Api ：[" + apiInformation.ApiTopic + "] Check Username[" + account[0] + "] failed, access not permission").ToJsonString((Formatting)1);
				}
			}
			catch (Exception ex)
			{
				return new OperateResult<string>("Mqtt RPC Api ：[" + apiInformation.ApiTopic + "] Check Username failed, access not permission, reason:" + ex.Message).ToJsonString((Formatting)1);
			}
		}
		try
		{
			if (apiInformation.Method != null)
			{
				MethodInfo methodInfo = apiInformation.Method;
				string apiName = apiInformation.ApiTopic;
				if (request.HttpMethod != apiInformation.HttpMethod)
				{
					return new OperateResult("Current Api ：" + apiName + " not support diffrent httpMethod").ToJsonString((Formatting)1);
				}
				object obj = methodInfo.Invoke(parameters: (!(request.HttpMethod == "GET")) ? HslReflectionHelper.GetParametersFromJson(context, request, methodInfo.GetParameters(), json) : ((deceodeUrl.IndexOf('?') <= 0) ? HslReflectionHelper.GetParametersFromJson(context, request, methodInfo.GetParameters(), json) : HslReflectionHelper.GetParametersFromUrl(context, request, methodInfo.GetParameters(), deceodeUrl)), obj: apiInformation.SourceObject);
				if (obj is Task task)
				{
					await task;
					return task.GetType().GetProperty("Result").GetValue(task, null)
						.ToJsonString((Formatting)1);
				}
				return obj.ToJsonString((Formatting)1);
			}
			if (apiInformation.Property != null)
			{
				string apiName2 = apiInformation.ApiTopic;
				if (request.HttpMethod != apiInformation.HttpMethod)
				{
					return new OperateResult("Current Api ：" + apiName2 + " not support diffrent httpMethod").ToJsonString((Formatting)1);
				}
				if (request.HttpMethod != "GET")
				{
					return new OperateResult("Current Api ：" + apiName2 + " not support POST").ToJsonString((Formatting)1);
				}
				return apiInformation.Property.GetValue(apiInformation.SourceObject, null).ToJsonString((Formatting)1);
			}
			return new OperateResult("Current Api ：" + deceodeUrl + " not supported").ToJsonString((Formatting)1);
		}
		catch (Exception ex2)
		{
			return new OperateResult("Current Api ：" + deceodeUrl + " Wrong，Reason：" + ex2.Message).ToJsonString((Formatting)1);
		}
	}
}
