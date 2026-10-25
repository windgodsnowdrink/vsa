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
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Reflection;
using Newtonsoft.Json.Linq;

namespace HslCommunication.Profinet.Siemens;

/// <summary>
/// 西门子的基于WebApi协议读写数据对象，支持对PLC的标签进行读取，适用于1500系列，该数据标签需要共享开放出来。
/// Siemens reads and writes data objects based on the WebApi protocol, supports reading PLC tags,
/// and is suitable for the 1500 series. The data tags need to be shared and opened.
/// </summary>
public class SiemensWebApi : DeviceWebApi
{
	private string rawUrl = "api/jsonrpc";

	private string token = string.Empty;

	private SoftIncrementCount incrementCount = new SoftIncrementCount(65535L, 1L);

	/// <summary>
	/// 当前PLC的通信令牌，当调用时，会自动获取，当然你也可以手动赋值一个合法的令牌，跳过直接进行读写操作。
	/// The communication token of the current PLC will be automatically obtained when  is called. Of course,
	/// you can also manually assign a valid token, skip  and read it directly Write operation.
	/// </summary>
	public string Token
	{
		get
		{
			return token;
		}
		set
		{
			token = value;
		}
	}

	/// <summary>
	/// 实例化一个默认的西门子WebApi通信对象
	/// Instantiate a default Siemens WebApi communication object
	/// </summary>
	public SiemensWebApi()
		: this("127.0.0.1")
	{
	}

	/// <summary>
	/// 使用指定的ip地址及端口号来实例化一个对象，端口号默认使用443，如果是http访问，使用80端口号
	/// Use the specified ip address and port number to instantiate an object, the port number is 443 by default, if it is http access, port 80 is used
	/// </summary>
	/// <param name="ipAddress">ip地址信息</param>
	/// <param name="port">端口号信息</param>
	public SiemensWebApi(string ipAddress, int port = 443)
		: base(ipAddress, port)
	{
		base.WordLength = 2;
		base.UseHttps = true;
		base.DefaultContentType = "application/json";
		base.ByteTransform = new ReverseBytesTransform();
	}

	protected override void AddRequestHeaders(HttpContentHeaders headers)
	{
		if (!string.IsNullOrEmpty(token))
		{
			headers.Add("X-Auth-Token", token);
		}
	}

	/// <summary>
	/// 根据设置好的用户名和密码信息，登录远程的PLC，返回是否登录成功！在读写之前，必须成功调用当前的方法，获取到token，否则无法进行通信。
	/// According to the set user name and password information, log in to the remote PLC, and return whether the login is successful!
	/// Before reading and writing, the current method must be successfully called to obtain the token,
	/// otherwise communication cannot be carried out.
	/// </summary>
	/// <returns>是否连接成功</returns>
	public OperateResult ConnectServer()
	{
		JArray val = BuildConnectBody(incrementCount.GetCurrentValue(), base.UserName, base.Password);
		OperateResult<string> operateResult = Post(rawUrl, ((object)val).ToString());
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return CheckLoginResult(operateResult.Content);
	}

	/// <summary>
	/// 和PLC断开当前的连接信息，主要是使得Token信息失效。
	/// Disconnecting the current connection information from the PLC mainly makes the token information invalid.
	/// </summary>
	/// <returns>是否断开成功</returns>
	public OperateResult ConnectClose()
	{
		return Logout();
	}

	/// <summary>
	/// 根据设置好的用户名和密码信息，登录远程的PLC，返回是否登录成功！在读写之前，必须成功调用当前的方法，获取到token，否则无法进行通信。
	/// According to the set user name and password information, log in to the remote PLC, and return whether the login is successful!
	/// Before reading and writing, the current method must be successfully called to obtain the token,
	/// otherwise communication cannot be carried out.
	/// </summary>
	/// <returns>是否连接成功</returns>
	public async Task<OperateResult> ConnectServerAsync()
	{
		OperateResult<string> read = await PostAsync(body: ((object)BuildConnectBody(incrementCount.GetCurrentValue(), base.UserName, base.Password)).ToString(), rawUrl: rawUrl);
		if (!read.IsSuccess)
		{
			return read;
		}
		return CheckLoginResult(read.Content);
	}

	/// <summary>
	/// 和PLC断开当前的连接信息，主要是使得Token信息失效。
	/// Disconnecting the current connection information from the PLC mainly makes the token information invalid.
	/// </summary>
	/// <returns>是否断开成功</returns>
	public async Task<OperateResult> ConnectCloseAsync()
	{
		return await LogoutAsync();
	}

	private OperateResult CheckLoginResult(string response)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		try
		{
			JArray val = JArray.Parse(response);
			JObject val2 = (JObject)val[0];
			OperateResult operateResult = CheckErrorResult(val2);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<JToken>(operateResult);
			}
			if (val2.ContainsKey("result"))
			{
				JToken obj = val2["result"];
				JObject val3 = (JObject)(object)((obj is JObject) ? obj : null);
				token = ((JToken)val3).Value<string>((object)"token");
				return OperateResult.CreateSuccessResult();
			}
			return new OperateResult("Can't find result key and none token, login failed:" + Environment.NewLine + response);
		}
		catch (Exception ex)
		{
			return new OperateResult("CheckLoginResult failed:" + ex.Message + Environment.NewLine + response);
		}
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 从PLC中根据输入的数据标签名称，读取出原始的字节数组信息，长度参数无效，需要二次解析
	/// According to the input data tag name, read the original byte array information from the PLC,
	/// the length parameter is invalid, and a second analysis is required
	/// </summary>
	/// <param name="address">标签的地址信息，例如 &quot;全局DB&quot;.Static_21</param>
	/// <param name="length">无效的参数</param>
	/// <returns>原始的字节数组信息</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		JArray val = BuildReadRawBody(incrementCount.GetCurrentValue(), address);
		OperateResult<string> operateResult = Post(rawUrl, ((object)val).ToString());
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return CheckReadRawResult(operateResult.Content);
	}

	[HslMqttApi("ReadByte", "")]
	/// <summary>
	/// 从PLC中根据输入的数据标签名称，按照类型byte进行读取出数据信息
	/// According to the input data tag name from the PLC, read the data information according to the type byte
	/// </summary>
	/// <param name="address">标签的地址信息，例如 &quot;全局DB&quot;.Static_21</param>
	/// <returns>包含是否成功的结果对象</returns>
	public OperateResult<byte> ReadByte(string address)
	{
		return ByteTransformHelper.GetResultFromArray(Read(address, 1));
	}

	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		OperateResult<byte[]> operateResult = Read(address, 0);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content.ToBoolArray().SelectBegin(length));
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 将原始的字节数组信息写入到PLC的指定的数据标签里，写入方式为raw写入，是否写入成功取决于PLC的返回信息
	/// Write the original byte array information to the designated data tag of the PLC.
	/// The writing method is raw writing. Whether the writing is successful depends on the return information of the PLC.
	/// </summary>
	/// <param name="address">标签的地址信息，例如 &quot;全局DB&quot;.Static_21</param>
	/// <param name="value">原始的字节数据信息</param>
	/// <returns>是否成功写入PLC的结果对象</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		JArray val = BuildWriteRawBody(incrementCount.GetCurrentValue(), address, value);
		OperateResult<string> operateResult = Post(rawUrl, ((object)val).ToString());
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return CheckWriteResult(operateResult.Content);
	}

	[HslMqttApi("WriteByte", "")]
	/// <summary>
	/// 写入数组数据，返回是否成功
	/// Write  array data, return whether the write was successful
	/// </summary>
	/// <param name="address">标签的地址信息，例如 &quot;全局DB&quot;.Static_21</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public OperateResult Write(string address, byte value)
	{
		return Write(address, new byte[1] { value });
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// 批量写入数组数据，返回是否成功
	/// Batch write  array data, return whether the write was successful
	/// </summary>
	/// <param name="address">标签的地址信息，例如 &quot;全局DB&quot;.Static_21</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, bool[] value)
	{
		byte[] value2 = value.ToByteArray();
		return Write(address, value2);
	}

	[HslMqttApi("ReadString", "读取字符串")]
	/// <summary>
	/// 从PLC中读取字符串内容，需要指定数据标签名称，使用JSON方式读取，所以无论字符串是中英文都是支持读取的。
	/// To read the string content from the PLC, you need to specify the data tag name and use JSON to read it,
	/// so no matter whether the string is in Chinese or English, it supports reading.
	/// </summary>
	/// <param name="address">标签的地址信息，例如 &quot;全局DB&quot;.Static_21</param>
	/// <param name="length">无效参数</param>
	/// <returns>包含是否成功的结果对象</returns>
	public override OperateResult<string> ReadString(string address, ushort length)
	{
		JArray val = BuildReadJTokenBody(incrementCount.GetCurrentValue(), address);
		OperateResult<string> operateResult = Post(rawUrl, ((object)val).ToString());
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		OperateResult<JToken> operateResult2 = CheckAndExtraOneJsonResult(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(Extensions.Value<string>((IEnumerable<JToken>)operateResult2.Content));
	}

	[HslMqttApi("WriteString", "")]
	/// <summary>
	/// 将字符串信息写入到PLC中，需要指定数据标签名称，如果PLC指定了类型为WString，才支持写入中文，否则会出现乱码。
	/// To write string information into the PLC, you need to specify the data tag name.
	/// If the PLC specifies the type as WString, it supports writing in Chinese, otherwise garbled characters will appear.
	/// </summary>
	/// <param name="address">标签的地址信息，例如 &quot;全局DB&quot;.Static_21</param>
	/// <param name="value">字符串数据信息</param>
	/// <returns>是否成功写入</returns>
	public override OperateResult Write(string address, string value)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		JArray val = BuildWriteJTokenBody(incrementCount.GetCurrentValue(), address, (JToken)new JValue(value));
		OperateResult<string> operateResult = Post(rawUrl, ((object)val).ToString());
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return CheckWriteResult(operateResult.Content);
	}

	/// <summary>
	/// 从PLC中根据输入的数据标签名称，读取出原始的字节数组信息，长度参数无效，需要二次解析
	/// According to the input data tag name, read the original byte array information from the PLC,
	/// the length parameter is invalid, and a second analysis is required
	/// </summary>
	/// <param name="address">标签的地址信息，例如 &quot;全局DB&quot;.Static_21</param>
	/// <param name="length">无效的参数</param>
	/// <returns>原始的字节数组信息</returns>
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		OperateResult<string> read = await PostAsync(body: ((object)BuildReadRawBody(incrementCount.GetCurrentValue(), address)).ToString(), rawUrl: rawUrl);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return CheckReadRawResult(read.Content);
	}

	/// <summary>
	/// 从PLC中根据输入的数据标签名称，按照类型byte进行读取出数据信息
	/// According to the input data tag name from the PLC, read the data information according to the type byte
	/// </summary>
	/// <param name="address">标签的地址信息，例如 &quot;全局DB&quot;.Static_21</param>
	/// <returns>包含是否成功的结果对象</returns>
	public async Task<OperateResult<byte>> ReadByteAsync(string address)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadAsync(address, 1));
	}

	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		OperateResult<byte[]> read = await ReadAsync(address, 0);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(read);
		}
		return OperateResult.CreateSuccessResult(read.Content.ToBoolArray().SelectBegin(length));
	}

	/// <summary>
	/// 从PLC中读取字符串内容，需要指定数据标签名称，使用JSON方式读取，所以无论字符串是中英文都是支持读取的。
	/// To read the string content from the PLC, you need to specify the data tag name and use JSON to read it,
	/// so no matter whether the string is in Chinese or English, it supports reading.
	/// </summary>
	/// <param name="address">标签的地址信息，例如 &quot;全局DB&quot;.Static_21</param>
	/// <param name="length">无效参数</param>
	/// <returns>包含是否成功的结果对象</returns>
	public override async Task<OperateResult<string>> ReadStringAsync(string address, ushort length)
	{
		OperateResult<string> read = await PostAsync(body: ((object)BuildReadJTokenBody(incrementCount.GetCurrentValue(), address)).ToString(), rawUrl: rawUrl);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		OperateResult<JToken> extra = CheckAndExtraOneJsonResult(read.Content);
		if (!extra.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(extra);
		}
		return OperateResult.CreateSuccessResult(Extensions.Value<string>((IEnumerable<JToken>)extra.Content));
	}

	/// <summary>
	/// 将原始的字节数组信息写入到PLC的指定的数据标签里，写入方式为raw写入，是否写入成功取决于PLC的返回信息
	/// Write the original byte array information to the designated data tag of the PLC.
	/// The writing method is raw writing. Whether the writing is successful depends on the return information of the PLC.
	/// </summary>
	/// <param name="address">标签的地址信息，例如 &quot;全局DB&quot;.Static_21</param>
	/// <param name="value">原始的字节数据信息</param>
	/// <returns>是否成功写入PLC的结果对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		OperateResult<string> read = await PostAsync(body: ((object)BuildWriteRawBody(incrementCount.GetCurrentValue(), address, value)).ToString(), rawUrl: rawUrl);
		if (!read.IsSuccess)
		{
			return read;
		}
		return CheckWriteResult(read.Content);
	}

	/// <summary>
	/// 写入数组数据，返回是否成功
	/// Write  array data, return whether the write was successful
	/// </summary>
	/// <param name="address">标签的地址信息，例如 &quot;全局DB&quot;.Static_21</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteAsync(string address, byte value)
	{
		return await WriteAsync(address, new byte[1] { value });
	}

	/// <summary>
	/// 批量写入数组数据，返回是否成功
	/// Batch write  array data, return whether the write was successful
	/// </summary>
	/// <param name="address">标签的地址信息，例如 &quot;全局DB&quot;.Static_21</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public override async Task<OperateResult> WriteAsync(string address, bool[] value)
	{
		byte[] buffer = value.ToByteArray();
		return await WriteAsync(address, buffer);
	}

	/// <summary>
	/// 将字符串信息写入到PLC中，需要指定数据标签名称，如果PLC指定了类型为WString，才支持写入中文，否则会出现乱码。
	/// To write string information into the PLC, you need to specify the data tag name.
	/// If the PLC specifies the type as WString, it supports writing in Chinese, otherwise garbled characters will appear.
	/// </summary>
	/// <param name="address">标签的地址信息，例如 &quot;全局DB&quot;.Static_21</param>
	/// <param name="value">字符串数据信息</param>
	/// <returns>是否成功写入</returns>
	public override async Task<OperateResult> WriteAsync(string address, string value)
	{
		OperateResult<string> read = await PostAsync(body: ((object)BuildWriteJTokenBody(incrementCount.GetCurrentValue(), address, (JToken)new JValue(value))).ToString(), rawUrl: rawUrl);
		if (!read.IsSuccess)
		{
			return read;
		}
		return CheckWriteResult(read.Content);
	}

	[HslMqttApi("读取当前PLC的操作模式，如果读取成功，结果将会是如下值之一：STOP, STARTUP, RUN, HOLD, -")]
	/// <summary>
	/// 读取当前PLC的操作模式，如果读取成功，结果将会是如下值之一：STOP, STARTUP, RUN, HOLD, -
	/// Read the current operating mode of the PLC. If the reading is successful,
	/// the result will be one of the following values: STOP, STARTUP, RUN, HOLD,-
	/// </summary>
	/// <returns>结果对象</returns>
	public OperateResult<string> ReadOperatingMode()
	{
		JArray val = BuildRequestBody("Plc.ReadOperatingMode", null, incrementCount.GetCurrentValue());
		OperateResult<string> operateResult = Post(rawUrl, ((object)val).ToString());
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		OperateResult<JToken> operateResult2 = CheckAndExtraOneJsonResult(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(Extensions.Value<string>((IEnumerable<JToken>)operateResult2.Content));
	}

	/// <summary>
	/// 读取当前PLC的操作模式，如果读取成功，结果将会是如下值之一：STOP, STARTUP, RUN, HOLD, -
	/// Read the current operating mode of the PLC. If the reading is successful,
	/// the result will be one of the following values: STOP, STARTUP, RUN, HOLD,-
	/// </summary>
	/// <returns>结果对象</returns>
	public async Task<OperateResult<string>> ReadOperatingModeAsync()
	{
		OperateResult<string> read = await PostAsync(body: ((object)BuildRequestBody("Plc.ReadOperatingMode", null, incrementCount.GetCurrentValue())).ToString(), rawUrl: rawUrl);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		OperateResult<JToken> extra = CheckAndExtraOneJsonResult(read.Content);
		if (!extra.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(extra);
		}
		return OperateResult.CreateSuccessResult(Extensions.Value<string>((IEnumerable<JToken>)extra.Content));
	}

	[HslMqttApi("ReadJTokens", "从PLC读取多个地址的数据信息，每个地址的数据类型可以不一致")]
	/// <summary>
	/// [商业授权] 从PLC读取多个地址的数据信息，每个地址的数据类型可以不一致，需要自动从中提取出正确的数据
	/// [Authorization] Read the data information of multiple addresses from the PLC, the data type of each address can be inconsistent,
	/// you need to automatically extract the correct data from
	/// </summary>
	/// <param name="address">&quot;全局DB&quot;.Static_21</param>
	/// <returns>返回是否读取成功的结果对象</returns>
	/// <remarks>
	/// 一旦中间有一个地址失败了，本方法就会返回失败，所以在调用本方法时，需要确保所有的地址正确。
	/// </remarks>
	public OperateResult<JToken[]> Read(string[] address)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<JToken[]>(StringResources.Language.InsufficientPrivileges);
		}
		JArray val = BuildReadJTokenBody(incrementCount.GetCurrentValue(), address);
		OperateResult<string> operateResult = Post(rawUrl, ((object)val).ToString());
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<JToken[]>(operateResult);
		}
		return CheckAndExtraJsonResult(operateResult.Content);
	}

	/// <summary>
	/// [商业授权] 从PLC读取多个地址的数据信息，每个地址的数据类型可以不一致，需要自动从中提取出正确的数据
	/// [Authorization] Read the data information of multiple addresses from the PLC, the data type of each address can be inconsistent,
	/// you need to automatically extract the correct data from
	/// </summary>
	/// <param name="address">&quot;全局DB&quot;.Static_21</param>
	/// <returns>返回是否读取成功的结果对象</returns>
	/// <remarks>
	/// 一旦中间有一个地址失败了，本方法就会返回失败，所以在调用本方法时，需要确保所有的地址正确。
	/// </remarks>
	public async Task<OperateResult<JToken[]>> ReadAsync(string[] address)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<JToken[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<string> read = await PostAsync(body: ((object)BuildReadJTokenBody(incrementCount.GetCurrentValue(), address)).ToString(), rawUrl: rawUrl);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<JToken[]>(read);
		}
		return CheckAndExtraJsonResult(read.Content);
	}

	[HslMqttApi("ReadDateTime", "读取PLC的时间格式的数据，这个格式是s7格式的一种")]
	/// <summary>
	/// 从PLC中读取时间格式的数据
	/// Read time format data from PLC
	/// </summary>
	/// <param name="address">地址</param>
	/// <returns>时间对象</returns>
	public OperateResult<DateTime> ReadDateTime(string address)
	{
		return SiemensDateTime.FromByteArray(Read(address, 8));
	}

	[HslMqttApi("WriteDateTime", "写入PLC的时间格式的数据，这个格式是s7格式的一种")]
	/// <summary>
	/// 向PLC中写入时间格式的数据
	/// Writes data in time format to the PLC
	/// </summary>
	/// <param name="address">地址</param>
	/// <param name="dateTime">时间</param>
	/// <returns>是否写入成功</returns>
	public OperateResult Write(string address, DateTime dateTime)
	{
		return Write(address, SiemensDateTime.ToByteArray(dateTime));
	}

	/// <summary>
	/// 从PLC中读取时间格式的数据
	/// Read time format data from PLC
	/// </summary>
	/// <param name="address">地址</param>
	/// <returns>时间对象</returns>
	public async Task<OperateResult<DateTime>> ReadDateTimeAsync(string address)
	{
		return SiemensDateTime.FromByteArray(await ReadAsync(address, 8).ConfigureAwait(continueOnCapturedContext: false));
	}

	/// <summary>
	/// 向PLC中写入时间格式的数据
	/// Writes data in time format to the PLC
	/// </summary>
	/// <param name="address">地址</param>
	/// <param name="dateTime">时间</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteAsync(string address, DateTime dateTime)
	{
		return await WriteAsync(address, SiemensDateTime.ToByteArray(dateTime));
	}

	[HslMqttApi("读取PLC的RPC接口的版本号信息")]
	/// <summary>
	/// [商业授权] 读取PLC的RPC接口的版本号信息
	/// [Authorization] Read the version number information of the PLC's RPC interface
	/// </summary>
	/// <returns>包含是否成功的结果对象</returns>
	public OperateResult<double> ReadVersion()
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<double>(StringResources.Language.InsufficientPrivileges);
		}
		JArray val = BuildRequestBody("Api.Version", null, incrementCount.GetCurrentValue());
		OperateResult<string> operateResult = Post(rawUrl, ((object)val).ToString());
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<double>(operateResult);
		}
		OperateResult<JToken> operateResult2 = CheckAndExtraOneJsonResult(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<double>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(Extensions.Value<double>((IEnumerable<JToken>)operateResult2.Content));
	}

	/// <summary>
	/// [商业授权] 读取PLC的RPC接口的版本号信息
	/// [Authorization] Read the version number information of the PLC's RPC interface
	/// </summary>
	/// <returns>包含是否成功的结果对象</returns>
	public async Task<OperateResult<double>> ReadVersionAsync()
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<double>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<string> read = await PostAsync(body: ((object)BuildRequestBody("Api.Version", null, incrementCount.GetCurrentValue())).ToString(), rawUrl: rawUrl);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<double>(read);
		}
		OperateResult<JToken> extra = CheckAndExtraOneJsonResult(read.Content);
		if (!extra.IsSuccess)
		{
			return OperateResult.CreateFailedResult<double>(extra);
		}
		return OperateResult.CreateSuccessResult(Extensions.Value<double>((IEnumerable<JToken>)extra.Content));
	}

	[HslMqttApi("对PLC对象进行PING操作")]
	/// <summary>
	/// [商业授权] 对PLC对象进行PING操作
	/// [Authorization] PING the PLC object
	/// </summary>
	/// <returns>是否PING成功</returns>
	public OperateResult ReadPing()
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<double>(StringResources.Language.InsufficientPrivileges);
		}
		JArray val = BuildRequestBody("Api.Ping", null, incrementCount.GetCurrentValue());
		OperateResult<string> operateResult = Post(rawUrl, ((object)val).ToString());
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		JToken obj = JArray.Parse(operateResult.Content)[0];
		return CheckErrorResult((JObject)(object)((obj is JObject) ? obj : null));
	}

	/// <summary>
	/// [商业授权] 对PLC对象进行PING操作
	/// [Authorization] PING the PLC object
	/// </summary>
	/// <returns>是否PING成功</returns>
	public async Task<OperateResult> ReadPingAsync()
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<double>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<string> read = await PostAsync(body: ((object)BuildRequestBody("Api.Ping", null, incrementCount.GetCurrentValue())).ToString(), rawUrl: rawUrl);
		if (!read.IsSuccess)
		{
			return read;
		}
		JToken obj = JArray.Parse(read.Content)[0];
		return CheckErrorResult((JObject)(object)((obj is JObject) ? obj : null));
	}

	[HslMqttApi("从PLC退出登录，当前的token信息失效，需要再次调用ConnectServer获取新的token信息才可以")]
	/// <summary>
	/// 从PLC退出登录，当前的token信息失效，需要再次调用获取新的token信息才可以。
	/// Log out from the PLC, the current token information is invalid, you need to call  again to get the new token information.
	/// </summary>
	/// <returns>是否成功</returns>
	public OperateResult Logout()
	{
		JArray val = BuildRequestBody("Api.Logout", null, incrementCount.GetCurrentValue());
		OperateResult<string> operateResult = Post(rawUrl, ((object)val).ToString());
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		JToken obj = JArray.Parse(operateResult.Content)[0];
		return CheckErrorResult((JObject)(object)((obj is JObject) ? obj : null));
	}

	/// <summary>
	/// 从PLC退出登录，当前的token信息失效，需要再次调用获取新的token信息才可以。
	/// Log out from the PLC, the current token information is invalid, you need to call  again to get the new token information.
	/// </summary>
	/// <returns>是否成功</returns>
	public async Task<OperateResult> LogoutAsync()
	{
		OperateResult<string> read = await PostAsync(body: ((object)BuildRequestBody("Api.Logout", null, incrementCount.GetCurrentValue())).ToString(), rawUrl: rawUrl);
		if (!read.IsSuccess)
		{
			return read;
		}
		JToken obj = JArray.Parse(read.Content)[0];
		return CheckErrorResult((JObject)(object)((obj is JObject) ? obj : null));
	}

	private static JObject GetJsonRpc(string method, JObject paramsJson, long id)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("jsonrpc", (JToken)new JValue("2.0"));
		val.Add("method", (JToken)new JValue(method));
		val.Add("id", (JToken)new JValue(id));
		if (paramsJson != null)
		{
			val.Add("params", (JToken)(object)paramsJson);
		}
		return val;
	}

	private static JArray BuildRequestBody(string method, JObject paramsJson, long id)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		JArray val = new JArray();
		val.Add((JToken)(object)GetJsonRpc(method, paramsJson, id));
		return val;
	}

	private static JArray BuildConnectBody(long id, string name, string password)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("user", (JToken)new JValue(name));
		val.Add("password", (JToken)new JValue(password));
		return BuildRequestBody("Api.Login", val, id);
	}

	private static JArray BuildReadRawBody(long id, string address)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("var", (JToken)new JValue(address));
		val.Add("mode", (JToken)new JValue("raw"));
		return BuildRequestBody("PlcProgram.Read", val, id);
	}

	private static JArray BuildWriteRawBody(long id, string address, byte[] value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("var", (JToken)new JValue(address));
		val.Add("mode", (JToken)new JValue("raw"));
		val.Add("value", (JToken)new JArray((object)((IEnumerable<byte>)value).Select((Func<byte, int>)((byte m) => m)).ToArray()));
		return BuildRequestBody("PlcProgram.Write", val, id);
	}

	private static JArray BuildWriteJTokenBody(long id, string address, JToken value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("var", (JToken)new JValue(address));
		val.Add("value", value);
		return BuildRequestBody("PlcProgram.Write", val, id);
	}

	private static JArray BuildReadJTokenBody(long id, string address)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("var", (JToken)new JValue(address));
		return BuildRequestBody("PlcProgram.Read", val, id);
	}

	private static JArray BuildReadJTokenBody(long id, string[] address)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		JArray val = new JArray();
		for (int i = 0; i < address.Length; i++)
		{
			JObject val2 = new JObject();
			val2.Add("var", (JToken)new JValue(address[i]));
			val.Add((JToken)(object)GetJsonRpc("PlcProgram.Read", val2, id + i));
		}
		return val;
	}

	private static OperateResult CheckErrorResult(JObject json)
	{
		if (json.ContainsKey("error"))
		{
			JToken obj = json["error"];
			JObject val = (JObject)(object)((obj is JObject) ? obj : null);
			int err = ((JToken)val).Value<int>((object)"code");
			string msg = ((JToken)val).Value<string>((object)"message");
			return new OperateResult(err, msg);
		}
		return OperateResult.CreateSuccessResult();
	}

	private static OperateResult<byte[]> CheckReadRawResult(string response)
	{
		OperateResult<JToken> operateResult = CheckAndExtraOneJsonResult(response);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		JToken content = operateResult.Content;
		JArray source = (JArray)(object)((content is JArray) ? content : null);
		return OperateResult.CreateSuccessResult(((IEnumerable<JToken>)source).Select((JToken m) => Extensions.Value<byte>((IEnumerable<JToken>)m)).ToArray());
	}

	private static OperateResult CheckWriteResult(string response)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		JArray val = JArray.Parse(response);
		JObject val2 = (JObject)val[0];
		OperateResult operateResult = CheckErrorResult(val2);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<JToken>(operateResult);
		}
		if (val2.ContainsKey("result"))
		{
			return Extensions.Value<bool>((IEnumerable<JToken>)val2["result"]) ? OperateResult.CreateSuccessResult() : new OperateResult(((object)val2).ToString());
		}
		return new OperateResult<JToken>("Can't find result key and none token, login failed:" + Environment.NewLine + response);
	}

	private static OperateResult<JToken> CheckAndExtraOneJsonResult(string response)
	{
		OperateResult<JToken[]> operateResult = CheckAndExtraJsonResult(response);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<JToken>(operateResult);
		}
		return OperateResult.CreateSuccessResult<JToken>(operateResult.Content[0]);
	}

	private static OperateResult<JToken[]> CheckAndExtraJsonResult(string response)
	{
		try
		{
			JArray val = JArray.Parse(response);
			List<JToken> list = new List<JToken>();
			for (int i = 0; i < ((JContainer)val).Count; i++)
			{
				JToken obj = val[i];
				JObject val2 = (JObject)(object)((obj is JObject) ? obj : null);
				if (val2 != null)
				{
					OperateResult operateResult = CheckErrorResult(val2);
					if (!operateResult.IsSuccess)
					{
						return OperateResult.CreateFailedResult<JToken[]>(operateResult);
					}
					if (!val2.ContainsKey("result"))
					{
						return new OperateResult<JToken[]>("Can't find result key and none token, login failed:" + Environment.NewLine + response);
					}
					list.Add(val2["result"]);
				}
			}
			return OperateResult.CreateSuccessResult(list.ToArray());
		}
		catch (Exception ex)
		{
			return new OperateResult<JToken[]>("CheckAndExtraJsonResult failed: " + ex.Message + Environment.NewLine + "Content: " + response);
		}
	}
}
