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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Net;
using HslCommunication.Core.Pipe;
using HslCommunication.Core.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HslCommunication.MQTT;

/// <summary>
/// 基于MQTT协议的同步访问的客户端程序，支持以同步的方式访问服务器的数据信息，并及时的反馈结果，当服务器启动文件功能时，也支持文件的上传，下载，删除操作等。
/// The client program based on MQTT protocol for synchronous access supports synchronous access to the server's data information and timely feedback of results,
/// When the server starts the file function, it also supports file upload, download, and delete operations.
/// </summary>
/// <remarks>
/// 在最新的V10.2.0及以上版本中，本客户端支持加密模式，启用加密模式后，就无法通过抓包的报文来分析出用户名密码，以及通信的数据细节，详细可以参考API文档。
/// </remarks>
public class MqttSyncClient : TcpNetCommunication
{
	private SoftIncrementCount incrementCount;

	private MqttConnectionOptions connectionOptions;

	private Encoding stringEncoding = Encoding.UTF8;

	private RSACryptoServiceProvider cryptoServiceProvider = null;

	private AesCryptography aesCryptography = null;

	/// <summary>
	/// 获取或设置当前的连接信息，客户端将根据这个连接配置进行连接服务器，在连接之前需要设置相关的信息才有效。
	/// To obtain or set the current connection information, the client will connect to the server according to this connection configuration.
	/// Before connecting, the relevant information needs to be set to be effective.
	/// </summary>
	public MqttConnectionOptions ConnectionOptions
	{
		get
		{
			return connectionOptions;
		}
		set
		{
			connectionOptions = value;
		}
	}

	/// <summary>
	/// 获取或设置使用字符串访问的时候，使用的编码信息，默认为UT8编码
	/// Get or set the encoding information used when accessing with a string, the default is UT8 encoding
	/// </summary>
	public Encoding StringEncoding
	{
		get
		{
			return stringEncoding;
		}
		set
		{
			stringEncoding = value;
		}
	}

	/// <summary>
	/// 实例化一个MQTT的同步客户端
	/// Instantiate an MQTT synchronization client
	/// </summary>
	/// <param name="options">连接的参数信息，可以指定IP地址，端口，账户名，密码，客户端ID信息</param>
	public MqttSyncClient(MqttConnectionOptions options)
	{
		connectionOptions = options;
		IpAddress = options.IpAddress;
		Port = options.Port;
		incrementCount = new SoftIncrementCount(65535L, 1L);
		ConnectTimeOut = options.ConnectTimeout;
		base.ReceiveTimeOut = 60000;
	}

	/// <summary>
	/// 通过指定的ip地址及端口来实例化一个同步的MQTT客户端
	/// Instantiate a synchronized MQTT client with the specified IP address and port
	/// </summary>
	/// <param name="ipAddress">IP地址信息</param>
	/// <param name="port">端口号信息</param>
	public MqttSyncClient(string ipAddress, int port)
	{
		connectionOptions = new MqttConnectionOptions
		{
			IpAddress = ipAddress,
			Port = port
		};
		IpAddress = ipAddress;
		Port = port;
		incrementCount = new SoftIncrementCount(65535L, 1L);
		base.ReceiveTimeOut = 60000;
	}

	/// <summary>
	/// 通过指定的ip地址及端口来实例化一个同步的MQTT客户端
	/// Instantiate a synchronized MQTT client with the specified IP address and port
	/// </summary>
	/// <param name="ipAddress">IP地址信息</param>
	/// <param name="port">端口号信息</param>
	public MqttSyncClient(IPAddress ipAddress, int port)
	{
		connectionOptions = new MqttConnectionOptions
		{
			IpAddress = ipAddress.ToString(),
			Port = port
		};
		IpAddress = ipAddress.ToString();
		Port = port;
		incrementCount = new SoftIncrementCount(65535L, 1L);
	}

	private bool ParseServiceRedirection(CommunicationPipe pipe, byte[] content)
	{
		if (content[0] == byte.MaxValue && content[1] == byte.MaxValue && content.Length >= 20)
		{
			int port = BitConverter.ToUInt16(content, 2);
			IPAddress iPAddress = new IPAddress(content.SelectMiddle(4, 4));
			pipe?.CloseCommunication();
			IpAddress = iPAddress.ToString();
			Port = port;
			return true;
		}
		return false;
	}

	private OperateResult InitializationMqttSocket(CommunicationPipe pipe, string protocol)
	{
		RSACryptoServiceProvider rsa = null;
		if (connectionOptions.UseRSAProvider)
		{
			cryptoServiceProvider = new RSACryptoServiceProvider();
			OperateResult operateResult = pipe.Send(MqttHelper.BuildMqttCommand(byte.MaxValue, null, HslSecurity.ByteEncrypt(cryptoServiceProvider.GetPEMPublicKey())).Content);
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
			OperateResult<byte, byte[]> operateResult2 = MqttHelper.ReceiveMqttMessage(pipe, base.ReceiveTimeOut);
			if (!operateResult2.IsSuccess)
			{
				return operateResult2;
			}
			try
			{
				byte[] publicKey = cryptoServiceProvider.DecryptLargeData(HslSecurity.ByteDecrypt(operateResult2.Content2));
				rsa = RSAHelper.CreateRsaProviderFromPublicKey(publicKey);
			}
			catch (Exception ex)
			{
				pipe?.CloseCommunication();
				return new OperateResult("RSA check failed: " + ex.Message);
			}
		}
		OperateResult<byte[]> operateResult3 = MqttHelper.BuildConnectMqttCommand(connectionOptions, protocol, rsa);
		if (!operateResult3.IsSuccess)
		{
			return operateResult3;
		}
		OperateResult operateResult4 = pipe.Send(operateResult3.Content);
		if (!operateResult4.IsSuccess)
		{
			return operateResult4;
		}
		OperateResult<byte, byte[]> operateResult5 = MqttHelper.ReceiveMqttMessage(pipe, base.ReceiveTimeOut);
		if (!operateResult5.IsSuccess)
		{
			return operateResult5;
		}
		if (ParseServiceRedirection(pipe, operateResult5.Content2))
		{
			return ConnectServer();
		}
		OperateResult operateResult6 = MqttHelper.CheckConnectBack(operateResult5.Content1, operateResult5.Content2);
		if (!operateResult6.IsSuccess)
		{
			pipe?.CloseCommunication();
			return operateResult6;
		}
		if (connectionOptions.UseRSAProvider)
		{
			string key = Encoding.UTF8.GetString(cryptoServiceProvider.Decrypt(operateResult5.Content2.RemoveBegin(2), fOAEP: false));
			aesCryptography = new AesCryptography(key);
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override OperateResult InitializationOnConnect()
	{
		OperateResult operateResult = InitializationMqttSocket(CommunicationPipe, "HUSL");
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		incrementCount.ResetCurrentValue();
		return OperateResult.CreateSuccessResult();
	}

	private async Task<OperateResult> InitializationMqttSocketAsync(CommunicationPipe pipe, string protocol)
	{
		RSACryptoServiceProvider rsa = null;
		if (connectionOptions.UseRSAProvider)
		{
			cryptoServiceProvider = new RSACryptoServiceProvider();
			OperateResult sendKey = await pipe.SendAsync(MqttHelper.BuildMqttCommand(byte.MaxValue, null, HslSecurity.ByteEncrypt(cryptoServiceProvider.GetPEMPublicKey())).Content);
			if (!sendKey.IsSuccess)
			{
				return sendKey;
			}
			OperateResult<byte, byte[]> key = await MqttHelper.ReceiveMqttMessageAsync(pipe, base.ReceiveTimeOut);
			if (!key.IsSuccess)
			{
				return key;
			}
			try
			{
				byte[] serverPublicToken = cryptoServiceProvider.DecryptLargeData(HslSecurity.ByteDecrypt(key.Content2));
				rsa = RSAHelper.CreateRsaProviderFromPublicKey(serverPublicToken);
			}
			catch (Exception ex)
			{
				pipe?.CloseCommunication();
				return new OperateResult("RSA check failed: " + ex.Message);
			}
		}
		OperateResult<byte[]> command = MqttHelper.BuildConnectMqttCommand(connectionOptions, protocol, rsa);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult send = await pipe.SendAsync(command.Content);
		if (!send.IsSuccess)
		{
			return send;
		}
		OperateResult<byte, byte[]> receive = await MqttHelper.ReceiveMqttMessageAsync(pipe, base.ReceiveTimeOut);
		if (!receive.IsSuccess)
		{
			return receive;
		}
		if (ParseServiceRedirection(pipe, receive.Content2))
		{
			return await ConnectServerAsync();
		}
		OperateResult check = MqttHelper.CheckConnectBack(receive.Content1, receive.Content2);
		if (!check.IsSuccess)
		{
			pipe?.CloseCommunication();
			return check;
		}
		if (connectionOptions.UseRSAProvider)
		{
			string key2 = Encoding.UTF8.GetString(cryptoServiceProvider.Decrypt(receive.Content2.RemoveBegin(2), fOAEP: false));
			aesCryptography = new AesCryptography(key2);
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override async Task<OperateResult> InitializationOnConnectAsync()
	{
		OperateResult ini = await InitializationMqttSocketAsync(CommunicationPipe, "HUSL");
		if (!ini.IsSuccess)
		{
			return ini;
		}
		incrementCount.ResetCurrentValue();
		return OperateResult.CreateSuccessResult();
	}

	public override OperateResult<byte[]> ReadFromCoreServer(CommunicationPipe pipe, byte[] send, bool hasResponseData = true, bool usePackHeader = true)
	{
		OperateResult<byte, byte[]> operateResult = ReadMqttFromCoreServer(pipe, send, null, null, null);
		if (operateResult.IsSuccess)
		{
			return OperateResult.CreateSuccessResult(operateResult.Content2);
		}
		return OperateResult.CreateFailedResult<byte[]>(operateResult);
	}

	private OperateResult<byte, byte[]> ReadMqttFromCoreServer(CommunicationPipe pipe, byte[] send, Action<long, long> sendProgress, Action<string, string> handleProgress, Action<long, long> receiveProgress)
	{
		OperateResult operateResult = pipe.Send(send);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte, byte[]>(operateResult);
		}
		long num;
		long num2;
		do
		{
			OperateResult<byte, byte[]> operateResult2 = MqttHelper.ReceiveMqttMessage(pipe, base.ReceiveTimeOut);
			if (!operateResult2.IsSuccess)
			{
				return operateResult2;
			}
			OperateResult<string, byte[]> operateResult3 = MqttHelper.ExtraMqttReceiveData(operateResult2.Content1, operateResult2.Content2);
			if (!operateResult3.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte, byte[]>(operateResult3);
			}
			if (operateResult3.Content2.Length != 16)
			{
				return new OperateResult<byte, byte[]>(StringResources.Language.ReceiveDataLengthTooShort);
			}
			num = BitConverter.ToInt64(operateResult3.Content2, 0);
			num2 = BitConverter.ToInt64(operateResult3.Content2, 8);
			sendProgress?.Invoke(num, num2);
		}
		while (num != num2);
		OperateResult<byte, byte[]> operateResult4;
		while (true)
		{
			operateResult4 = MqttHelper.ReceiveMqttMessage(pipe, base.ReceiveTimeOut, receiveProgress);
			if (!operateResult4.IsSuccess)
			{
				return operateResult4;
			}
			if (operateResult4.Content1 >> 4 != 15)
			{
				break;
			}
			OperateResult<string, byte[]> operateResult5 = MqttHelper.ExtraMqttReceiveData(operateResult4.Content1, operateResult4.Content2);
			handleProgress?.Invoke(operateResult5.Content1, Encoding.UTF8.GetString(operateResult5.Content2));
		}
		return OperateResult.CreateSuccessResult(operateResult4.Content1, operateResult4.Content2);
	}

	private OperateResult<byte[]> ReadMqttFromCoreServer(byte control, byte flags, byte[] variableHeader, byte[] payLoad, Action<long, long> sendProgress, Action<string, string> handleProgress, Action<long, long> receiveProgress)
	{
		OperateResult<byte[]> operateResult = new OperateResult<byte[]>();
		OperateResult operateResult2 = CommunicationPipe.CommunicationLock.EnterLock(base.ReceiveTimeOut);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		try
		{
			OperateResult<bool> operateResult3 = CommunicationPipe.OpenCommunication();
			if (!operateResult3.IsSuccess)
			{
				CommunicationPipe.CommunicationLock.LeaveLock();
				return OperateResult.CreateFailedResult<byte[]>(operateResult3);
			}
			if (operateResult3.Content)
			{
				OperateResult operateResult4 = InitializationOnConnect();
				if (!operateResult4.IsSuccess)
				{
					CommunicationPipe.CommunicationLock.LeaveLock();
					return OperateResult.CreateFailedResult<byte[]>(operateResult4);
				}
			}
			OperateResult<byte[]> operateResult5 = MqttHelper.BuildMqttCommand(control, flags, variableHeader, payLoad, aesCryptography);
			if (!operateResult5.IsSuccess)
			{
				CommunicationPipe.CommunicationLock.LeaveLock();
				operateResult.CopyErrorFromOther(operateResult5);
				return operateResult;
			}
			OperateResult<byte, byte[]> operateResult6 = ReadMqttFromCoreServer(CommunicationPipe, operateResult5.Content, sendProgress, handleProgress, receiveProgress);
			if (operateResult6.IsSuccess)
			{
				if (operateResult6.Content1 >> 4 == 0)
				{
					OperateResult<string, byte[]> operateResult7 = MqttHelper.ExtraMqttReceiveData(operateResult6.Content1, operateResult6.Content2, aesCryptography);
					operateResult.IsSuccess = false;
					operateResult.ErrorCode = int.Parse(operateResult7.Content1);
					operateResult.Message = Encoding.UTF8.GetString(operateResult7.Content2);
				}
				else
				{
					operateResult.IsSuccess = operateResult6.IsSuccess;
					operateResult.Content = operateResult6.Content2;
					operateResult.Message = StringResources.Language.SuccessText;
				}
			}
			else
			{
				operateResult.CopyErrorFromOther(operateResult6);
			}
			ExtraAfterReadFromCoreServer(operateResult6);
			CommunicationPipe.CommunicationLock.LeaveLock();
		}
		catch
		{
			CommunicationPipe.CommunicationLock.LeaveLock();
			throw;
		}
		if (!CommunicationPipe.IsPersistentConnection)
		{
			ExtraOnDisconnect();
			CommunicationPipe.CloseCommunication();
		}
		return operateResult;
	}

	public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(CommunicationPipe pipe, byte[] send, bool hasResponseData = true, bool usePackHeader = true)
	{
		OperateResult<byte, byte[]> read = await ReadMqttFromCoreServerAsync(pipe, send, null, null, null).ConfigureAwait(continueOnCapturedContext: false);
		if (read.IsSuccess)
		{
			return OperateResult.CreateSuccessResult(read.Content2);
		}
		return OperateResult.CreateFailedResult<byte[]>(read);
	}

	private async Task<OperateResult<byte, byte[]>> ReadMqttFromCoreServerAsync(CommunicationPipe pipe, byte[] send, Action<long, long> sendProgress, Action<string, string> handleProgress, Action<long, long> receiveProgress)
	{
		OperateResult sendResult = await pipe.SendAsync(send).ConfigureAwait(continueOnCapturedContext: false);
		if (!sendResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte, byte[]>(sendResult);
		}
		long already;
		long total;
		do
		{
			OperateResult<byte, byte[]> server_receive = await MqttHelper.ReceiveMqttMessageAsync(pipe, base.ReceiveTimeOut).ConfigureAwait(continueOnCapturedContext: false);
			if (!server_receive.IsSuccess)
			{
				return server_receive;
			}
			OperateResult<string, byte[]> server_back = MqttHelper.ExtraMqttReceiveData(server_receive.Content1, server_receive.Content2);
			if (!server_back.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte, byte[]>(server_back);
			}
			if (server_back.Content2.Length != 16)
			{
				return new OperateResult<byte, byte[]>(StringResources.Language.ReceiveDataLengthTooShort);
			}
			already = BitConverter.ToInt64(server_back.Content2, 0);
			total = BitConverter.ToInt64(server_back.Content2, 8);
			sendProgress?.Invoke(already, total);
		}
		while (already != total);
		OperateResult<byte, byte[]> receive;
		while (true)
		{
			receive = await MqttHelper.ReceiveMqttMessageAsync(pipe, base.ReceiveTimeOut, receiveProgress).ConfigureAwait(continueOnCapturedContext: false);
			if (!receive.IsSuccess)
			{
				return receive;
			}
			if (receive.Content1 >> 4 != 15)
			{
				break;
			}
			OperateResult<string, byte[]> extra = MqttHelper.ExtraMqttReceiveData(receive.Content1, receive.Content2);
			handleProgress?.Invoke(extra.Content1, Encoding.UTF8.GetString(extra.Content2));
		}
		return OperateResult.CreateSuccessResult(receive.Content1, receive.Content2);
	}

	private async Task<OperateResult<byte[]>> ReadMqttFromCoreServerAsync(byte control, byte flags, byte[] variableHeader, byte[] payLoad, Action<long, long> sendProgress, Action<string, string> handleProgress, Action<long, long> receiveProgress)
	{
		OperateResult<byte[]> result = new OperateResult<byte[]>();
		OperateResult enter = await Task.Run(() => CommunicationPipe.CommunicationLock.EnterLock(base.ReceiveTimeOut)).ConfigureAwait(continueOnCapturedContext: false);
		if (!enter.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(enter);
		}
		try
		{
			OperateResult<bool> pipe = await CommunicationPipe.OpenCommunicationAsync().ConfigureAwait(continueOnCapturedContext: false);
			if (!pipe.IsSuccess)
			{
				CommunicationPipe.CommunicationLock.LeaveLock();
				return OperateResult.CreateFailedResult<byte[]>(pipe);
			}
			if (pipe.Content)
			{
				OperateResult ini = await InitializationOnConnectAsync().ConfigureAwait(continueOnCapturedContext: false);
				if (!ini.IsSuccess)
				{
					CommunicationPipe.CommunicationLock.LeaveLock();
					return OperateResult.CreateFailedResult<byte[]>(ini);
				}
			}
			OperateResult<byte[]> command = MqttHelper.BuildMqttCommand(control, flags, variableHeader, payLoad, aesCryptography);
			if (!command.IsSuccess)
			{
				CommunicationPipe.CommunicationLock.LeaveLock();
				result.CopyErrorFromOther(command);
				return result;
			}
			OperateResult<byte, byte[]> read = await ReadMqttFromCoreServerAsync(CommunicationPipe, command.Content, sendProgress, handleProgress, receiveProgress).ConfigureAwait(continueOnCapturedContext: false);
			if (read.IsSuccess)
			{
				if (read.Content1 >> 4 == 0)
				{
					OperateResult<string, byte[]> extra = MqttHelper.ExtraMqttReceiveData(read.Content1, read.Content2, aesCryptography);
					result.IsSuccess = false;
					result.ErrorCode = int.Parse(extra.Content1);
					result.Message = Encoding.UTF8.GetString(extra.Content2);
				}
				else
				{
					result.IsSuccess = read.IsSuccess;
					result.Content = read.Content2;
					result.Message = StringResources.Language.SuccessText;
				}
			}
			else
			{
				result.CopyErrorFromOther(read);
			}
			ExtraAfterReadFromCoreServer(read);
			CommunicationPipe.CommunicationLock.LeaveLock();
		}
		catch
		{
			CommunicationPipe.CommunicationLock.LeaveLock();
			throw;
		}
		if (!CommunicationPipe.IsPersistentConnection)
		{
			ExtraOnDisconnect();
			CommunicationPipe.CloseCommunication();
		}
		return result;
	}

	/// <summary>
	/// 从MQTT服务器同步读取数据，将payload发送到服务器，然后从服务器返回相关的数据，支持数据发送进度报告，服务器执行进度报告，接收数据进度报告操作
	/// Synchronously read data from the MQTT server, send the payload to the server, and then return relevant data from the server,
	/// support data transmission progress report, the server executes the progress report, and receives the data progress report
	/// </summary>
	/// <param name="topic">主题信息</param>
	/// <param name="payload">负载数据</param>
	/// <param name="sendProgress">发送数据给服务器时的进度报告，第一个参数为已发送数据，第二个参数为总发送数据</param>
	/// <param name="handleProgress">服务器处理数据的进度报告，第一个参数Topic自定义，通常用来传送操作百分比，第二个参数自定义，通常用来表示服务器消息</param>
	/// <param name="receiveProgress">从服务器接收数据的进度报告，第一个参数为已接收数据，第二个参数为总接收数据</param>
	/// <returns>服务器返回的数据信息</returns>
	/// <remarks>
	/// 进度报告可以实现一个比较有意思的功能，可以用来数据的上传和下载，提供一个友好的进度条，因为网络的好坏通常是不确定的。
	/// </remarks>
	public OperateResult<string, byte[]> Read(string topic, byte[] payload, Action<long, long> sendProgress = null, Action<string, string> handleProgress = null, Action<long, long> receiveProgress = null)
	{
		OperateResult<byte[]> operateResult = ReadMqttFromCoreServer(3, 0, MqttHelper.BuildSegCommandByString(topic), payload, sendProgress, handleProgress, receiveProgress);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string, byte[]>(operateResult);
		}
		return MqttHelper.ExtraMqttReceiveData(3, operateResult.Content, aesCryptography);
	}

	/// <summary>
	/// 从MQTT服务器同步读取数据，将指定编码的字符串payload发送到服务器，然后从服务器返回相关的数据，并转换为指定编码的字符串，支持数据发送进度报告，服务器执行进度报告，接收数据进度报告操作
	/// Synchronously read data from the MQTT server, send the specified encoded string payload to the server,
	/// and then return the data from the server, and convert it to the specified encoded string,
	/// support data transmission progress report, the server executes the progress report, and receives the data progress report
	/// </summary>
	/// <param name="topic">主题信息</param>
	/// <param name="payload">负载数据</param>
	/// <param name="sendProgress">发送数据给服务器时的进度报告，第一个参数为已发送数据，第二个参数为总发送数据</param>
	/// <param name="handleProgress">服务器处理数据的进度报告，第一个参数Topic自定义，通常用来传送操作百分比，第二个参数自定义，通常用来表示服务器消息</param>
	/// <param name="receiveProgress">从服务器接收数据的进度报告，第一个参数为已接收数据，第二个参数为总接收数据</param>
	/// <returns>服务器返回的数据信息</returns>
	public OperateResult<string, string> ReadString(string topic, string payload, Action<long, long> sendProgress = null, Action<string, string> handleProgress = null, Action<long, long> receiveProgress = null)
	{
		OperateResult<string, byte[]> operateResult = Read(topic, string.IsNullOrEmpty(payload) ? null : stringEncoding.GetBytes(payload), sendProgress, handleProgress, receiveProgress);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string, string>(operateResult);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content1, stringEncoding.GetString(operateResult.Content2));
	}

	public OperateResult<T> ReadRpc<T>(string topic, string payload)
	{
		OperateResult<string, string> operateResult = ReadString(topic, payload);
		if (!operateResult.IsSuccess)
		{
			return operateResult.ConvertFailed<T>();
		}
		try
		{
			return OperateResult.CreateSuccessResult(JsonConvert.DeserializeObject<T>(operateResult.Content2));
		}
		catch (Exception ex)
		{
			return new OperateResult<T>("JSON failed: " + ex.Message + Environment.NewLine + "Source Data: " + operateResult.Content2);
		}
	}

	public OperateResult<T> ReadRpc<T>(string topic, object payload)
	{
		return ReadRpc<T>(topic, (payload == null) ? "{}" : payload.ToJsonString((Formatting)1));
	}

	/// <summary>
	/// 读取服务器的已经注册的API信息列表，将返回API的主题路径，注释信息，示例的传入的数据信息。
	/// Read the registered API information list of the server, and return the API subject path, annotation information, and sample incoming data information.
	/// </summary>
	/// <returns>包含是否成功的api信息的列表</returns>
	public OperateResult<MqttRpcApiInfo[]> ReadRpcApis()
	{
		OperateResult<byte[]> operateResult = ReadMqttFromCoreServer(8, 0, MqttHelper.BuildSegCommandByString(""), null, null, null, null);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<MqttRpcApiInfo[]>(operateResult);
		}
		OperateResult<string, byte[]> operateResult2 = MqttHelper.ExtraMqttReceiveData(3, operateResult.Content, aesCryptography);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<MqttRpcApiInfo[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(((JToken)JArray.Parse(Encoding.UTF8.GetString(operateResult2.Content2))).ToObject<MqttRpcApiInfo[]>());
	}

	/// <summary>
	/// 读取服务器的指定的API接口的每天的调用次数，如果API接口不存在，或是还没有调用数据，则返回失败。
	/// Read the number of calls per day of the designated API interface of the server.
	/// If the API interface does not exist or the data has not been called yet, it returns a failure.
	/// </summary>
	/// <param name="api">等待请求的API的接口信息，如果为空，就是请求所有的接口的调用的统计信息。</param>
	/// <returns>最近几日的连续的调用情况，例如[1,2,3]，表示前提调用1次，昨天调用2次，今天3次</returns>
	/// <remarks>
	/// 如果api的参数为空字符串，就是请求所有的接口的调用的统计信息。
	/// </remarks>
	public OperateResult<long[]> ReadRpcApiLog(string api)
	{
		OperateResult<byte[]> operateResult = ReadMqttFromCoreServer(6, 0, MqttHelper.BuildSegCommandByString(api), null, null, null, null);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<long[]>(operateResult);
		}
		OperateResult<string, byte[]> operateResult2 = MqttHelper.ExtraMqttReceiveData(3, operateResult.Content, aesCryptography);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<long[]>(operateResult2);
		}
		string value = Encoding.UTF8.GetString(operateResult2.Content2);
		return OperateResult.CreateSuccessResult(value.ToStringArray<long>());
	}

	/// <summary>
	/// 读取服务器的已经驻留的所有消息的主题列表
	/// Read the topic list of all messages that have resided on the server
	/// </summary>
	/// <returns>消息列表对象</returns>
	public OperateResult<string[]> ReadRetainTopics()
	{
		OperateResult<byte[]> operateResult = ReadMqttFromCoreServer(4, 0, MqttHelper.BuildSegCommandByString(""), null, null, null, null);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string[]>(operateResult);
		}
		OperateResult<string, byte[]> operateResult2 = MqttHelper.ExtraMqttReceiveData(3, operateResult.Content, aesCryptography);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(HslProtocol.UnPackStringArrayFromByte(operateResult2.Content2));
	}

	/// <summary>
	/// 读取服务器的已经驻留的指定主题的消息内容
	/// Read the topic list of all messages that have resided on the server
	/// </summary>
	/// <param name="topic">指定的主题消息</param>
	/// <param name="receiveProgress">结果进度报告</param>
	/// <returns>消息列表对象</returns>
	public OperateResult<MqttClientApplicationMessage> ReadTopicPayload(string topic, Action<long, long> receiveProgress = null)
	{
		OperateResult<byte[]> operateResult = ReadMqttFromCoreServer(5, 0, MqttHelper.BuildSegCommandByString(topic), null, null, null, receiveProgress);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<MqttClientApplicationMessage>(operateResult);
		}
		OperateResult<string, byte[]> operateResult2 = MqttHelper.ExtraMqttReceiveData(3, operateResult.Content, aesCryptography);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<MqttClientApplicationMessage>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(((JToken)JObject.Parse(Encoding.UTF8.GetString(operateResult2.Content2))).ToObject<MqttClientApplicationMessage>());
	}

	/// <summary>
	/// 读取服务器里当前的会话信息
	/// </summary>
	/// <returns>会话信息</returns>
	public OperateResult<MqttSessionInfo[]> ReadSessions()
	{
		OperateResult<byte[]> operateResult = ReadMqttFromCoreServer(11, 0, MqttHelper.BuildSegCommandByString(""), null, null, null, null);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<MqttSessionInfo[]>(operateResult);
		}
		OperateResult<string, byte[]> operateResult2 = MqttHelper.ExtraMqttReceiveData(3, operateResult.Content, aesCryptography);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<MqttSessionInfo[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(((JToken)JArray.Parse(Encoding.UTF8.GetString(operateResult2.Content2))).ToObject<MqttSessionInfo[]>());
	}

	/// <summary>
	/// 从MQTT服务器同步读取数据，将payload发送到服务器，然后从服务器返回相关的数据，支持数据发送进度报告，服务器执行进度报告，接收数据进度报告操作
	/// Synchronously read data from the MQTT server, send the payload to the server, and then return relevant data from the server,
	/// support data transmission progress report, the server executes the progress report, and receives the data progress report
	/// </summary>
	/// <param name="topic">主题信息</param>
	/// <param name="payload">负载数据</param>
	/// <param name="sendProgress">发送数据给服务器时的进度报告，第一个参数为已发送数据，第二个参数为总发送数据</param>
	/// <param name="handleProgress">服务器处理数据的进度报告，第一个参数Topic自定义，通常用来传送操作百分比，第二个参数自定义，通常用来表示服务器消息</param>
	/// <param name="receiveProgress">从服务器接收数据的进度报告，第一个参数为已接收数据，第二个参数为总接收数据</param>
	/// <returns>服务器返回的数据信息</returns>
	/// <remarks>
	/// 进度报告可以实现一个比较有意思的功能，可以用来数据的上传和下载，提供一个友好的进度条，因为网络的好坏通常是不确定的。
	/// </remarks>
	public async Task<OperateResult<string, byte[]>> ReadAsync(string topic, byte[] payload, Action<long, long> sendProgress = null, Action<string, string> handleProgress = null, Action<long, long> receiveProgress = null)
	{
		OperateResult<byte[]> read = await ReadMqttFromCoreServerAsync(3, 0, MqttHelper.BuildSegCommandByString(topic), payload, sendProgress, handleProgress, receiveProgress);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string, byte[]>(read);
		}
		return MqttHelper.ExtraMqttReceiveData(3, read.Content, aesCryptography);
	}

	/// <summary>
	/// 从MQTT服务器同步读取数据，将指定编码的字符串payload发送到服务器，然后从服务器返回相关的数据，并转换为指定编码的字符串，支持数据发送进度报告，服务器执行进度报告，接收数据进度报告操作
	/// Synchronously read data from the MQTT server, send the specified encoded string payload to the server,
	/// and then return the data from the server, and convert it to the specified encoded string,
	/// support data transmission progress report, the server executes the progress report, and receives the data progress report
	/// </summary>
	/// <param name="topic">主题信息</param>
	/// <param name="payload">负载数据</param>
	/// <param name="sendProgress">发送数据给服务器时的进度报告，第一个参数为已发送数据，第二个参数为总发送数据</param>
	/// <param name="handleProgress">服务器处理数据的进度报告，第一个参数Topic自定义，通常用来传送操作百分比，第二个参数自定义，通常用来表示服务器消息</param>
	/// <param name="receiveProgress">从服务器接收数据的进度报告，第一个参数为已接收数据，第二个参数为总接收数据</param>
	/// <returns>服务器返回的数据信息</returns>
	public async Task<OperateResult<string, string>> ReadStringAsync(string topic, string payload, Action<long, long> sendProgress = null, Action<string, string> handleProgress = null, Action<long, long> receiveProgress = null)
	{
		OperateResult<string, byte[]> read = await ReadAsync(topic, string.IsNullOrEmpty(payload) ? null : stringEncoding.GetBytes(payload), sendProgress, handleProgress, receiveProgress);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string, string>(read);
		}
		return OperateResult.CreateSuccessResult(read.Content1, stringEncoding.GetString(read.Content2));
	}

	public async Task<OperateResult<T>> ReadRpcAsync<T>(string topic, string payload)
	{
		OperateResult<string, string> read = await ReadStringAsync(topic, payload);
		if (!read.IsSuccess)
		{
			return read.ConvertFailed<T>();
		}
		try
		{
			return OperateResult.CreateSuccessResult(JsonConvert.DeserializeObject<T>(read.Content2));
		}
		catch (Exception ex)
		{
			return new OperateResult<T>("JSON failed: " + ex.Message + Environment.NewLine + "Source Data: " + read.Content2);
		}
	}

	public async Task<OperateResult<T>> ReadRpcAsync<T>(string topic, object payload)
	{
		return await ReadRpcAsync<T>(topic, (payload == null) ? "{}" : payload.ToJsonString((Formatting)1));
	}

	/// <summary>
	/// 读取服务器的已经注册的API信息列表，将返回API的主题路径，注释信息，示例的传入的数据信息。
	/// Read the registered API information list of the server, and return the API subject path, annotation information, and sample incoming data information.
	/// </summary>
	/// <returns>包含是否成功的api信息的列表</returns>
	public async Task<OperateResult<MqttRpcApiInfo[]>> ReadRpcApisAsync()
	{
		OperateResult<byte[]> read = await ReadMqttFromCoreServerAsync(8, 0, MqttHelper.BuildSegCommandByString(""), null, null, null, null);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<MqttRpcApiInfo[]>(read);
		}
		OperateResult<string, byte[]> mqtt = MqttHelper.ExtraMqttReceiveData(3, read.Content, aesCryptography);
		if (!mqtt.IsSuccess)
		{
			return OperateResult.CreateFailedResult<MqttRpcApiInfo[]>(mqtt);
		}
		return OperateResult.CreateSuccessResult(((JToken)JArray.Parse(Encoding.UTF8.GetString(mqtt.Content2))).ToObject<MqttRpcApiInfo[]>());
	}

	/// <summary>
	/// 读取服务器的指定的API接口的每天的调用次数，如果API接口不存在，或是还没有调用数据，则返回失败。
	/// Read the number of calls per day of the designated API interface of the server.
	/// If the API interface does not exist or the data has not been called yet, it returns a failure.
	/// </summary>
	/// <param name="api">等待请求的API的接口信息，如果为空，就是请求所有的接口的调用的统计信息。</param>
	/// <returns>最近几日的连续的调用情况，例如[1,2,3]，表示前提调用1次，昨天调用2次，今天3次</returns>
	/// <remarks>
	/// 如果api的参数为空字符串，就是请求所有的接口的调用的统计信息。
	/// </remarks>
	public async Task<OperateResult<long[]>> ReadRpcApiLogAsync(string api)
	{
		OperateResult<byte[]> read = await ReadMqttFromCoreServerAsync(6, 0, MqttHelper.BuildSegCommandByString(api), null, null, null, null);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<long[]>(read);
		}
		OperateResult<string, byte[]> mqtt = MqttHelper.ExtraMqttReceiveData(3, read.Content, aesCryptography);
		if (!mqtt.IsSuccess)
		{
			return OperateResult.CreateFailedResult<long[]>(mqtt);
		}
		string content = Encoding.UTF8.GetString(mqtt.Content2);
		return OperateResult.CreateSuccessResult(content.ToStringArray<long>());
	}

	/// <summary>
	/// 读取服务器的已经驻留的所有消息的主题列表
	/// Read the topic list of all messages that have resided on the server
	/// </summary>
	/// <returns>消息列表对象</returns>
	public async Task<OperateResult<string[]>> ReadRetainTopicsAsync()
	{
		OperateResult<byte[]> read = await ReadMqttFromCoreServerAsync(4, 0, MqttHelper.BuildSegCommandByString(""), null, null, null, null);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string[]>(read);
		}
		OperateResult<string, byte[]> mqtt = MqttHelper.ExtraMqttReceiveData(3, read.Content, aesCryptography);
		if (!mqtt.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string[]>(mqtt);
		}
		return OperateResult.CreateSuccessResult(HslProtocol.UnPackStringArrayFromByte(mqtt.Content2));
	}

	/// <summary>
	/// 读取服务器的已经驻留的指定主题的消息内容
	/// Read the topic list of all messages that have resided on the server
	/// </summary>
	/// <param name="topic">指定的主题消息</param>
	/// <param name="receiveProgress">结果进度报告</param>
	/// <returns>消息列表对象</returns>
	public async Task<OperateResult<MqttClientApplicationMessage>> ReadTopicPayloadAsync(string topic, Action<long, long> receiveProgress = null)
	{
		OperateResult<byte[]> read = await ReadMqttFromCoreServerAsync(5, 0, MqttHelper.BuildSegCommandByString(topic), null, null, null, receiveProgress);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<MqttClientApplicationMessage>(read);
		}
		OperateResult<string, byte[]> mqtt = MqttHelper.ExtraMqttReceiveData(3, read.Content, aesCryptography);
		if (!mqtt.IsSuccess)
		{
			return OperateResult.CreateFailedResult<MqttClientApplicationMessage>(mqtt);
		}
		return OperateResult.CreateSuccessResult(((JToken)JObject.Parse(Encoding.UTF8.GetString(mqtt.Content2))).ToObject<MqttClientApplicationMessage>());
	}

	private OperateResult<CommunicationPipe> ConnectMqttFileServer(byte opCode, string groups, string[] fileNames)
	{
		PipeTcpNet pipeTcpNet = new PipeTcpNet(IpAddress, Port);
		pipeTcpNet.ConnectTimeOut = ConnectTimeOut;
		OperateResult operateResult = pipeTcpNet.OpenCommunication();
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<CommunicationPipe>(operateResult);
		}
		OperateResult operateResult2 = InitializationMqttSocket(pipeTcpNet, "FILE");
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<CommunicationPipe>(operateResult2);
		}
		OperateResult operateResult3 = pipeTcpNet.Send(MqttHelper.BuildMqttCommand(opCode, null, HslProtocol.PackStringArrayToByte(string.IsNullOrEmpty(groups) ? null : groups.Split(new char[2] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries))).Content);
		if (!operateResult3.IsSuccess)
		{
			return OperateResult.CreateFailedResult<CommunicationPipe>(operateResult3);
		}
		OperateResult operateResult4 = pipeTcpNet.Send(MqttHelper.BuildMqttCommand(opCode, null, HslProtocol.PackStringArrayToByte(fileNames)).Content);
		if (!operateResult4.IsSuccess)
		{
			return OperateResult.CreateFailedResult<CommunicationPipe>(operateResult4);
		}
		OperateResult<byte, byte[]> operateResult5 = MqttHelper.ReceiveMqttMessage(pipeTcpNet, base.ReceiveTimeOut);
		if (!operateResult5.IsSuccess)
		{
			return OperateResult.CreateFailedResult<CommunicationPipe>(operateResult5);
		}
		if (operateResult5.Content1 == 0)
		{
			pipeTcpNet.CloseCommunication();
			return new OperateResult<CommunicationPipe>(Encoding.UTF8.GetString(operateResult5.Content2));
		}
		return OperateResult.CreateSuccessResult((CommunicationPipe)pipeTcpNet);
	}

	private OperateResult DownloadFileBase(string groups, string fileName, Action<long, long> processReport, object source, HslCancelToken cancelToken)
	{
		OperateResult<CommunicationPipe> operateResult = ConnectMqttFileServer(101, groups, new string[1] { fileName });
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult operateResult2 = MqttHelper.ReceiveMqttFile(operateResult.Content, source, processReport, aesCryptography, cancelToken);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		operateResult.Content?.CloseCommunication();
		return OperateResult.CreateSuccessResult();
	}

	private OperateResult<T> OperateMqttFileContent<T>(byte opCode, string groups, string[] fileNames, Func<OperateResult<byte, byte[]>, T> trans)
	{
		OperateResult<CommunicationPipe> operateResult = ConnectMqttFileServer(opCode, groups, fileNames);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T>(operateResult);
		}
		OperateResult<byte, byte[]> operateResult2 = MqttHelper.ReceiveMqttMessage(operateResult.Content, 60000);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T>(operateResult2);
		}
		operateResult.Content?.CloseCommunication();
		if (operateResult2.Content1 == 0)
		{
			return new OperateResult<T>((operateResult2.Content2 == null) ? string.Empty : Encoding.UTF8.GetString(operateResult2.Content2));
		}
		try
		{
			return OperateResult.CreateSuccessResult(trans(operateResult2));
		}
		catch (Exception ex)
		{
			return new OperateResult<T>(ex.Message);
		}
	}

	/// <summary>
	/// [文件引擎] 从远程服务器下载一个文件到本地，需要指定文件类别，文件名，进度报告，本地保存的文件名
	/// [File Engine] To download a file from a remote server to the local, you need to specify the file category, file name, progress report, and file name saved locally
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="fileName">文件名称，例如 123.txt</param>
	/// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数，如果不需要，传入NULL即可</param>
	/// <param name="fileSaveName">本地保存的文件名</param>
	/// <param name="cancelToken">取消下载操作的令牌</param>
	/// <returns>是否下载成功</returns>
	public OperateResult DownloadFile(string groups, string fileName, Action<long, long> processReport, string fileSaveName, HslCancelToken cancelToken = null)
	{
		return DownloadFileBase(groups, fileName, processReport, fileSaveName, cancelToken);
	}

	/// <summary>
	/// [文件引擎] 从远程服务器下载一个文件到流中，需要指定文件类别，文件名，进度报告，本地保存的文件名
	/// To download a file from a remote server to the stream, you need to specify the file category, file name, progress report, and file name saved locally
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="fileName">文件名称，例如 123.txt</param>
	/// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数</param>
	/// <param name="stream">数据流</param>
	/// <param name="cancelToken">取消下载操作的令牌</param>
	/// <returns>是否下载成功</returns>
	public OperateResult DownloadFile(string groups, string fileName, Action<long, long> processReport, Stream stream, HslCancelToken cancelToken = null)
	{
		return DownloadFileBase(groups, fileName, processReport, stream, cancelToken);
	}

	/// <summary>
	/// [文件引擎] 上传文件给服务器，需要指定上传的数据内容，上传到服务器的分类信息，支持进度汇报功能。
	/// [File Engine] To upload files to the server, you need to specify the content of the uploaded data,
	/// the classification information uploaded to the server, and support the progress report function.
	/// </summary>
	/// <param name="source">数据源，可以是文件名，也可以是数据流</param>
	/// <param name="serverName">在服务器保存的文件名，不包含驱动器路径</param>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="fileTag">文件的额外的描述信息</param>
	/// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数</param>
	/// <param name="cancelToken">用户取消的令牌信息</param>
	/// <returns>是否成功的结果对象</returns>
	private OperateResult UploadFileBase(object source, string groups, string serverName, string fileTag, Action<long, long> processReport, HslCancelToken cancelToken)
	{
		OperateResult<CommunicationPipe> operateResult = ConnectMqttFileServer(102, groups, new string[1] { serverName });
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		if (source is string filename)
		{
			OperateResult operateResult2 = MqttHelper.SendMqttFile(operateResult.Content, filename, serverName, fileTag, processReport, aesCryptography, cancelToken);
			if (!operateResult2.IsSuccess)
			{
				return operateResult2;
			}
		}
		else
		{
			if (!(source is Stream stream))
			{
				operateResult.Content?.CloseCommunication();
				base.LogNet?.WriteError(ToString(), StringResources.Language.DataSourceFormatError);
				return new OperateResult(StringResources.Language.DataSourceFormatError);
			}
			OperateResult operateResult3 = MqttHelper.SendMqttFile(operateResult.Content, stream, serverName, fileTag, processReport, aesCryptography, cancelToken);
			if (!operateResult3.IsSuccess)
			{
				return operateResult3;
			}
		}
		OperateResult<byte, byte[]> operateResult4 = MqttHelper.ReceiveMqttMessage(operateResult.Content, 60000);
		if (!operateResult4.IsSuccess)
		{
			return operateResult4;
		}
		operateResult.Content?.CloseCommunication();
		return (operateResult4.Content1 != 0) ? OperateResult.CreateSuccessResult() : new OperateResult(Encoding.UTF8.GetString(operateResult4.Content2));
	}

	/// <summary>
	/// [文件引擎] 上传文件给服务器，需要指定上传文件的路径信息，服务器保存的名字，以及上传到服务器的分类信息，支持进度汇报功能。
	/// [File Engine] To upload a file to the server, you need to specify the path information of the uploaded file, the name saved by the server,
	/// and the classification information uploaded to the server to support the progress report function.
	/// </summary>
	/// <param name="fileName">文件名，需要指定完整的路径信息，文件必须存在，否则发送失败</param>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="serverName">服务器端保存的文件名</param>
	/// <param name="fileTag">文件的额外的描述信息</param>
	/// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数</param>
	/// <param name="cancelToken">取消上传操作的令牌</param>
	/// <returns>是否上传成功的结果对象</returns>
	public OperateResult UploadFile(string fileName, string groups, string serverName, string fileTag, Action<long, long> processReport, HslCancelToken cancelToken = null)
	{
		if (!File.Exists(fileName))
		{
			return new OperateResult(StringResources.Language.FileNotExist);
		}
		return UploadFileBase(fileName, groups, serverName, fileTag, processReport, cancelToken);
	}

	/// <summary>
	/// [文件引擎] 上传文件给服务器，需要指定上传文件的路径信息(服务器保存的名称就是文件名)，以及上传到服务器的分类信息，支持进度汇报功能。
	/// [File Engine] To upload a file to the server, you need to specify the path information of the uploaded file (the name saved by the server is the file name),
	/// as well as the classification information uploaded to the server, to support the progress report function.
	/// </summary>
	/// <param name="fileName">文件名，需要指定完整的路径信息，文件必须存在，否则发送失败</param>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="fileTag">文件的额外的描述信息</param>
	/// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数</param>
	/// <param name="cancelToken">取消上传操作的令牌</param>
	/// <returns>是否上传成功的结果对象</returns>
	public OperateResult UploadFile(string fileName, string groups, string fileTag, Action<long, long> processReport, HslCancelToken cancelToken = null)
	{
		if (!File.Exists(fileName))
		{
			return new OperateResult(StringResources.Language.FileNotExist);
		}
		FileInfo fileInfo = new FileInfo(fileName);
		return UploadFileBase(fileName, groups, fileInfo.Name, fileTag, processReport, cancelToken);
	}

	/// <summary>
	/// [文件引擎] 上传流给服务器，需要指定流，服务器保存的名字，以及上传到服务器的分类信息，支持进度汇报功能。
	/// [File Engine] To upload a stream to the server, you need to specify the stream, the name saved by the server, and the classification information uploaded to the server to support the progress reporting function.
	/// </summary>
	/// <param name="stream">流</param>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="serverName">文件的额外的描述信息</param>
	/// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数</param>
	/// <param name="cancelToken">取消上传操作的令牌</param>
	/// <returns>是否上传成功的结果对象</returns>
	public OperateResult UploadFile(Stream stream, string groups, string serverName, string fileTag, Action<long, long> processReport, HslCancelToken cancelToken = null)
	{
		return UploadFileBase(stream, groups, serverName, fileTag, processReport, cancelToken);
	}

	private OperateResult<T[]> DownloadStringArrays<T>(byte protocol, string groups, string[] fileNames)
	{
		return OperateMqttFileContent(protocol, groups, fileNames, (OperateResult<byte, byte[]> m) => ((JToken)JArray.Parse(Encoding.UTF8.GetString(m.Content2))).ToObject<T[]>());
	}

	/// <summary>
	/// [文件引擎] 下载指定分类信息的所有的文件描述信息，需要指定分类信息，例如：Files/Personal/Admin
	/// [File Engine] To download all the file description information of the specified classification information,
	/// you need to specify the classification information, for example: Files/Personal/Admin
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <returns>当前分类下所有的文件描述信息</returns>
	public OperateResult<GroupFileItem[]> DownloadPathFileNames(string groups)
	{
		return DownloadStringArrays<GroupFileItem>(105, groups, null);
	}

	/// <summary>
	/// 下载指定分类信息的全部子分类信息
	/// Download all sub-category information of the specified category information
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <returns>当前分类下所有的子分类信息</returns>
	public OperateResult<string[]> DownloadPathFolders(string groups)
	{
		return DownloadStringArrays<string>(106, groups, null);
	}

	/// <summary>
	/// [文件引擎] 请求服务器指定分类是否存在指定的文件名，需要指定分类信息，文件名
	/// [File Engine] Request the server to specify whether the specified file name exists in the specified category, need to specify the category information, file name
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="fileName">文件名信息，例如 123.txt</param>
	/// <returns>Content为True表示存在，否则为不存在</returns>
	public OperateResult<bool> IsFileExists(string groups, string fileName)
	{
		return OperateMqttFileContent(107, groups, new string[1] { fileName }, (OperateResult<byte, byte[]> m) => m.Content1 == 1);
	}

	/// <summary>
	/// [文件引擎] 删除服务器的指定的文件名，需要指定分类信息，文件名
	/// [File Engine] Delete the specified file name of the server, need to specify the classification information, file name
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="fileName">文件名信息</param>
	/// <returns>是否删除成功</returns>
	public OperateResult DeleteFile(string groups, string fileName)
	{
		return DeleteFile(groups, new string[1] { fileName });
	}

	/// <summary>
	/// [文件引擎] 删除服务器的指定的文件名，需要指定分类信息，文件名
	/// [File Engine] Delete the specified file name of the server, need to specify the classification information, file name
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="fileName">文件名信息</param>
	/// <returns>是否删除成功</returns>
	public OperateResult DeleteFile(string groups, string[] fileNames)
	{
		return OperateMqttFileContent(103, groups, fileNames, (OperateResult<byte, byte[]> m) => m.Content1);
	}

	/// <summary>
	/// [文件引擎] 删除服务器上指定的分类的所有的文件，不可逆操作，谨慎操作。
	/// [File Engine] Delete the specified classification information and all files managed on the server, irreversible operation, and careful operation.
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <returns>是否删除成功</returns>
	public OperateResult DeleteFolderFiles(string groups)
	{
		return OperateMqttFileContent(110, groups, null, (OperateResult<byte, byte[]> m) => m.Content1);
	}

	/// <summary>
	/// [文件引擎] 删除服务器上指定的分类信息及管理的所有的文件，包含所有的子分类信息，不可逆操作，谨慎操作。
	/// [File Engine] Delete the specified classification information and all files managed on the server,
	/// including all sub-classification information, irreversible operation, and careful operation.
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <returns>是否删除成功</returns>
	public OperateResult DeleteFolder(string groups)
	{
		return OperateMqttFileContent(104, groups, null, (OperateResult<byte, byte[]> m) => m.Content1);
	}

	/// <summary>
	/// [文件引擎] 重命名服务器上指定的路径信息，需要指定新的路径名称，当新的路径已经存在的时候，命名失败。
	/// [File Engine] Renames the specified path information on the server, you need to specify a new path name, and when the new path already exists, the naming fails.
	/// </summary>
	/// <param name="groups">旧的路径信息</param>
	/// <param name="newName">新的路径名称</param>
	/// <returns>是否重命名成功</returns>
	public OperateResult RenameFolder(string groups, string newName)
	{
		return OperateMqttFileContent(111, groups, new string[1] { newName }, (OperateResult<byte, byte[]> m) => m.Content1);
	}

	/// <summary>
	/// [文件引擎] 获取服务器文件夹的指定目录的文件统计信息，包括文件数量，总大小，最后更新时间
	/// [File Engine] Get the file statistics of the specified directory of the server folder, including the number of files, the total size, and the last update time
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <returns>服务器文件大小的结果对象，单位：字节数</returns>
	public OperateResult<GroupFileInfo> GetGroupFileInfo(string groups)
	{
		return OperateMqttFileContent(108, groups, null, (OperateResult<byte, byte[]> m) => ((JToken)JObject.Parse(Encoding.UTF8.GetString(m.Content2))).ToObject<GroupFileInfo>());
	}

	/// <summary>
	/// [文件引擎] 获取服务器文件夹的指定目录的所有子目录的文件信息，包括每个子目录的文件数量，总大小，最后更新时间
	/// [File Engine] Get the file information of all subdirectories of the specified directory of the server folder, including the number of files in each subdirectory, the total size, and the last update time
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="withLastFileInfo">是否让服务器携带最新的文件信息返回</param>
	/// <returns>服务器文件大小的结果对象，单位：字节数</returns>
	public OperateResult<GroupFileInfo[]> GetSubGroupFileInfos(string groups, bool withLastFileInfo = false)
	{
		return OperateMqttFileContent(109, groups, (!withLastFileInfo) ? null : new string[1] { "1" }, (OperateResult<byte, byte[]> m) => ((JToken)JArray.Parse(Encoding.UTF8.GetString(m.Content2))).ToObject<GroupFileInfo[]>());
	}

	private async Task<OperateResult<CommunicationPipe>> ConnectMqttFileServerAsync(byte opCode, string groups, string[] fileNames)
	{
		PipeTcpNet pipe = new PipeTcpNet(IpAddress, Port)
		{
			ConnectTimeOut = ConnectTimeOut
		};
		OperateResult open = await pipe.OpenCommunicationAsync();
		if (!open.IsSuccess)
		{
			return OperateResult.CreateFailedResult<CommunicationPipe>(open);
		}
		OperateResult ini = await InitializationMqttSocketAsync(pipe, "FILE");
		if (!ini.IsSuccess)
		{
			return OperateResult.CreateFailedResult<CommunicationPipe>(ini);
		}
		OperateResult sendClass = await pipe.SendAsync(MqttHelper.BuildMqttCommand(opCode, null, HslProtocol.PackStringArrayToByte(string.IsNullOrEmpty(groups) ? null : groups.Split(new char[2] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries))).Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!sendClass.IsSuccess)
		{
			return OperateResult.CreateFailedResult<CommunicationPipe>(sendClass);
		}
		OperateResult sendString = await pipe.SendAsync(MqttHelper.BuildMqttCommand(opCode, null, HslProtocol.PackStringArrayToByte(fileNames)).Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!sendString.IsSuccess)
		{
			return OperateResult.CreateFailedResult<CommunicationPipe>(sendString);
		}
		OperateResult<byte, byte[]> legal = await MqttHelper.ReceiveMqttMessageAsync(pipe, 60000).ConfigureAwait(continueOnCapturedContext: false);
		if (!legal.IsSuccess)
		{
			return OperateResult.CreateFailedResult<CommunicationPipe>(legal);
		}
		if (legal.Content1 == 0)
		{
			pipe?.CloseCommunication();
			return new OperateResult<CommunicationPipe>(Encoding.UTF8.GetString(legal.Content2));
		}
		return OperateResult.CreateSuccessResult((CommunicationPipe)pipe);
	}

	private async Task<OperateResult> DownloadFileBaseAsync(string groups, string fileName, Action<long, long> processReport, object source, HslCancelToken cancelToken)
	{
		OperateResult<CommunicationPipe> socketResult = await ConnectMqttFileServerAsync(101, groups, new string[1] { fileName }).ConfigureAwait(continueOnCapturedContext: false);
		if (!socketResult.IsSuccess)
		{
			return socketResult;
		}
		OperateResult result = await MqttHelper.ReceiveMqttFileAsync(socketResult.Content, source, processReport, aesCryptography, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
		if (!result.IsSuccess)
		{
			return result;
		}
		socketResult.Content?.CloseCommunication();
		return OperateResult.CreateSuccessResult();
	}

	private async Task<OperateResult<T>> OperateMqttFileContentAsync<T>(byte opCode, string groups, string[] fileNames, Func<OperateResult<byte, byte[]>, T> trans)
	{
		OperateResult<CommunicationPipe> socketResult = await ConnectMqttFileServerAsync(opCode, groups, fileNames).ConfigureAwait(continueOnCapturedContext: false);
		if (!socketResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T>(socketResult);
		}
		OperateResult<byte, byte[]> receive = await MqttHelper.ReceiveMqttMessageAsync(socketResult.Content, 60000);
		if (!receive.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T>(receive);
		}
		socketResult.Content?.CloseCommunication();
		if (receive.Content1 == 0)
		{
			return new OperateResult<T>((receive.Content2 == null) ? string.Empty : Encoding.UTF8.GetString(receive.Content2));
		}
		try
		{
			return OperateResult.CreateSuccessResult(trans(receive));
		}
		catch (Exception ex)
		{
			return new OperateResult<T>(ex.Message);
		}
	}

	/// <summary>
	/// [文件引擎] 从远程服务器下载一个文件到本地，需要指定文件类别，文件名，进度报告，本地保存的文件名
	/// [File Engine] To download a file from a remote server to the local, you need to specify the file category, file name, progress report, and file name saved locally
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="fileName">文件名称，例如 123.txt</param>
	/// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数，如果不需要，传入NULL即可</param>
	/// <param name="fileSaveName">本地保存的文件名</param>
	/// <param name="cancelToken">取消下载操作的令牌</param>
	/// <returns>是否下载成功</returns>
	public async Task<OperateResult> DownloadFileAsync(string groups, string fileName, Action<long, long> processReport, string fileSaveName, HslCancelToken cancelToken = null)
	{
		return await DownloadFileBaseAsync(groups, fileName, processReport, fileSaveName, cancelToken);
	}

	/// <summary>
	/// [文件引擎] 从远程服务器下载一个文件到流中，需要指定文件类别，文件名，进度报告，本地保存的文件名
	/// To download a file from a remote server to the stream, you need to specify the file category, file name, progress report, and file name saved locally
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="fileName">文件名称，例如 123.txt</param>
	/// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数</param>
	/// <param name="stream">数据流</param>
	/// <param name="cancelToken">取消下载操作的令牌</param>
	/// <returns>是否下载成功</returns>
	public async Task<OperateResult> DownloadFileAsync(string groups, string fileName, Action<long, long> processReport, Stream stream, HslCancelToken cancelToken = null)
	{
		return await DownloadFileBaseAsync(groups, fileName, processReport, stream, cancelToken);
	}

	/// <summary>
	/// [文件引擎] 上传文件给服务器，需要指定上传的数据内容，上传到服务器的分类信息，支持进度汇报功能。
	/// [File Engine] To upload files to the server, you need to specify the content of the uploaded data,
	/// the classification information uploaded to the server, and support the progress report function.
	/// </summary>
	/// <param name="source">数据源，可以是文件名，也可以是数据流</param>
	/// <param name="serverName">在服务器保存的文件名，不包含驱动器路径</param>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="fileTag">文件的额外的描述信息</param>
	/// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数</param>
	/// <param name="cancelToken">用户取消的令牌信息</param>
	/// <returns>是否成功的结果对象</returns>
	private async Task<OperateResult> UploadFileBaseAsync(object source, string groups, string serverName, string fileTag, Action<long, long> processReport, HslCancelToken cancelToken)
	{
		OperateResult<CommunicationPipe> socketResult = await ConnectMqttFileServerAsync(102, groups, new string[1] { serverName }).ConfigureAwait(continueOnCapturedContext: false);
		if (!socketResult.IsSuccess)
		{
			return socketResult;
		}
		if (source is string fileName)
		{
			OperateResult result = await MqttHelper.SendMqttFileAsync(socketResult.Content, fileName, serverName, fileTag, processReport, aesCryptography, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
			if (!result.IsSuccess)
			{
				return result;
			}
		}
		else
		{
			if (!(source is Stream stream))
			{
				socketResult.Content?.CloseCommunication();
				base.LogNet?.WriteError(ToString(), StringResources.Language.DataSourceFormatError);
				return new OperateResult(StringResources.Language.DataSourceFormatError);
			}
			OperateResult result2 = await MqttHelper.SendMqttFileAsync(socketResult.Content, stream, serverName, fileTag, processReport, aesCryptography, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
			if (!result2.IsSuccess)
			{
				return result2;
			}
		}
		OperateResult<byte, byte[]> resultCheck = await MqttHelper.ReceiveMqttMessageAsync(socketResult.Content, 60000).ConfigureAwait(continueOnCapturedContext: false);
		if (!resultCheck.IsSuccess)
		{
			return resultCheck;
		}
		socketResult.Content?.CloseCommunication();
		return (resultCheck.Content1 != 0) ? OperateResult.CreateSuccessResult() : new OperateResult(Encoding.UTF8.GetString(resultCheck.Content2));
	}

	/// <summary>
	/// [文件引擎] 上传文件给服务器，需要指定上传文件的路径信息，服务器保存的名字，以及上传到服务器的分类信息，支持进度汇报功能。
	/// [File Engine] To upload a file to the server, you need to specify the path information of the uploaded file, the name saved by the server,
	/// and the classification information uploaded to the server to support the progress report function.
	/// </summary>
	/// <param name="fileName">文件名，需要指定完整的路径信息，文件必须存在，否则发送失败</param>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="serverName">服务器端保存的文件名</param>
	/// <param name="fileTag">文件的额外的描述信息</param>
	/// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数</param>
	/// <param name="cancelToken">取消上传操作的令牌</param>
	/// <returns>是否上传成功的结果对象</returns>
	public async Task<OperateResult> UploadFileAsync(string fileName, string groups, string serverName, string fileTag, Action<long, long> processReport, HslCancelToken cancelToken = null)
	{
		if (!File.Exists(fileName))
		{
			return new OperateResult(StringResources.Language.FileNotExist);
		}
		return await UploadFileBaseAsync(fileName, groups, serverName, fileTag, processReport, cancelToken);
	}

	/// <summary>
	/// [文件引擎] 上传文件给服务器，需要指定上传文件的路径信息(服务器保存的名称就是文件名)，以及上传到服务器的分类信息，支持进度汇报功能。
	/// [File Engine] To upload a file to the server, you need to specify the path information of the uploaded file (the name saved by the server is the file name),
	/// as well as the classification information uploaded to the server, to support the progress report function.
	/// </summary>
	/// <param name="fileName">文件名，需要指定完整的路径信息，文件必须存在，否则发送失败</param>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="fileTag">文件的额外的描述信息</param>
	/// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数</param>
	/// <param name="cancelToken">取消上传操作的令牌</param>
	/// <returns>是否上传成功的结果对象</returns>
	public async Task<OperateResult> UploadFileAsync(string fileName, string groups, string fileTag, Action<long, long> processReport, HslCancelToken cancelToken = null)
	{
		if (!File.Exists(fileName))
		{
			return new OperateResult(StringResources.Language.FileNotExist);
		}
		FileInfo fileInfo = new FileInfo(fileName);
		return await UploadFileBaseAsync(fileName, groups, fileInfo.Name, fileTag, processReport, cancelToken);
	}

	/// <summary>
	/// [文件引擎] 上传流给服务器，需要指定流，服务器保存的名字，以及上传到服务器的分类信息，支持进度汇报功能。
	/// [File Engine] To upload a stream to the server, you need to specify the stream, the name saved by the server, and the classification information uploaded to the server to support the progress reporting function.
	/// </summary>
	/// <param name="stream">流</param>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="serverName">文件的额外的描述信息</param>
	/// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数</param>
	/// <param name="cancelToken">取消上传操作的令牌</param>
	/// <returns>是否上传成功的结果对象</returns>
	public async Task<OperateResult> UploadFileAsync(Stream stream, string groups, string serverName, string fileTag, Action<long, long> processReport, HslCancelToken cancelToken = null)
	{
		return await UploadFileBaseAsync(stream, groups, serverName, fileTag, processReport, cancelToken);
	}

	private async Task<OperateResult<T[]>> DownloadStringArraysAsync<T>(byte protocol, string groups, string[] fileNames)
	{
		return await OperateMqttFileContentAsync(protocol, groups, fileNames, (OperateResult<byte, byte[]> m) => ((JToken)JArray.Parse(Encoding.UTF8.GetString(m.Content2))).ToObject<T[]>());
	}

	/// <summary>
	/// [文件引擎] 下载指定分类信息的所有的文件描述信息，需要指定分类信息，例如：Files/Personal/Admin
	/// [File Engine] To download all the file description information of the specified classification information,
	/// you need to specify the classification information, for example: Files/Personal/Admin
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <returns>当前分类下所有的文件描述信息</returns>
	public async Task<OperateResult<GroupFileItem[]>> DownloadPathFileNamesAsync(string groups)
	{
		return await DownloadStringArraysAsync<GroupFileItem>(105, groups, null);
	}

	/// <summary>
	/// 下载指定分类信息的全部子分类信息
	/// Download all sub-category information of the specified category information
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <returns>当前分类下所有的子分类信息</returns>
	public async Task<OperateResult<string[]>> DownloadPathFoldersAsync(string groups)
	{
		return await DownloadStringArraysAsync<string>(106, groups, null);
	}

	/// <summary>
	/// [文件引擎] 请求服务器指定分类是否存在指定的文件名，需要指定分类信息，文件名
	/// [File Engine] Request the server to specify whether the specified file name exists in the specified category, need to specify the category information, file name
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="fileName">文件名信息，例如 123.txt</param>
	/// <returns>Content为True表示存在，否则为不存在</returns>
	public async Task<OperateResult<bool>> IsFileExistsAsync(string groups, string fileName)
	{
		return await OperateMqttFileContentAsync(107, groups, new string[1] { fileName }, (OperateResult<byte, byte[]> m) => m.Content1 == 1);
	}

	/// <summary>
	/// [文件引擎] 删除服务器的指定的文件名，需要指定分类信息，文件名
	/// [File Engine] Delete the specified file name of the server, need to specify the classification information, file name
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="fileName">文件名信息</param>
	/// <returns>是否删除成功</returns>
	public async Task<OperateResult> DeleteFileAsync(string groups, string fileName)
	{
		return await DeleteFileAsync(groups, new string[1] { fileName });
	}

	/// <summary>
	/// [文件引擎] 删除服务器的指定的文件名，需要指定分类信息，文件名
	/// [File Engine] Delete the specified file name of the server, need to specify the classification information, file name
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="fileName">文件名信息</param>
	/// <returns>是否删除成功</returns>
	public async Task<OperateResult> DeleteFileAsync(string groups, string[] fileNames)
	{
		return await OperateMqttFileContentAsync(103, groups, fileNames, (OperateResult<byte, byte[]> m) => m.Content1);
	}

	/// <summary>
	/// [文件引擎] 删除服务器上指定的分类的所有的文件，不可逆操作，谨慎操作。
	/// [File Engine] Delete the specified classification information and all files managed on the server, irreversible operation, and careful operation.
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <returns>是否删除成功</returns>
	public async Task<OperateResult> DeleteFolderFilesAsync(string groups)
	{
		return await OperateMqttFileContentAsync(110, groups, null, (OperateResult<byte, byte[]> m) => m.Content1);
	}

	/// <summary>
	/// [文件引擎] 删除服务器上指定的分类信息及管理的所有的文件，包含所有的子分类信息，不可逆操作，谨慎操作。
	/// [File Engine] Delete the specified classification information and all files managed on the server,
	/// including all sub-classification information, irreversible operation, and careful operation.
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <returns>是否删除成功</returns>
	public async Task<OperateResult> DeleteFolderAsync(string groups)
	{
		return await OperateMqttFileContentAsync(104, groups, null, (OperateResult<byte, byte[]> m) => m.Content1);
	}

	/// <summary>
	/// [文件引擎] 重命名服务器上指定的路径信息，需要指定新的路径名称，当新的路径已经存在的时候，命名失败。
	/// [File Engine] Renames the specified path information on the server, you need to specify a new path name, and when the new path already exists, the naming fails.
	/// </summary>
	/// <param name="groups">旧的路径信息</param>
	/// <param name="newName">新的路径名称</param>
	/// <returns>是否重命名成功</returns>
	public async Task<OperateResult> RenameFolderAsync(string groups, string newName)
	{
		return await OperateMqttFileContentAsync(111, groups, new string[1] { newName }, (OperateResult<byte, byte[]> m) => m.Content1);
	}

	/// <summary>
	/// [文件引擎] 获取服务器文件夹的指定目录的文件统计信息，包括文件数量，总大小，最后更新时间
	/// [File Engine] Get the file statistics of the specified directory of the server folder, including the number of files, the total size, and the last update time
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <returns>服务器文件大小的结果对象，单位：字节数</returns>
	public async Task<OperateResult<GroupFileInfo>> GetGroupFileInfoAsync(string groups)
	{
		return await OperateMqttFileContentAsync(108, groups, null, (OperateResult<byte, byte[]> m) => ((JToken)JObject.Parse(Encoding.UTF8.GetString(m.Content2))).ToObject<GroupFileInfo>());
	}

	/// <summary>
	/// [文件引擎] 获取服务器文件夹的指定目录的所有子目录的文件信息，包括每个子目录的文件数量，总大小，最后更新时间
	/// [File Engine] Get the file information of all subdirectories of the specified directory of the server folder, including the number of files in each subdirectory, the total size, and the last update time
	/// </summary>
	/// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
	/// <param name="withLastFileInfo">是否让服务器携带最新的文件信息返回</param>
	/// <returns>服务器文件大小的结果对象，单位：字节数</returns>
	public async Task<OperateResult<GroupFileInfo[]>> GetSubGroupFileInfosAsync(string groups, bool withLastFileInfo = false)
	{
		return await OperateMqttFileContentAsync(109, groups, (!withLastFileInfo) ? null : new string[1] { "1" }, (OperateResult<byte, byte[]> m) => ((JToken)JArray.Parse(Encoding.UTF8.GetString(m.Content2))).ToObject<GroupFileInfo[]>());
	}

	public override string ToString()
	{
		return $"MqttSyncClient[{connectionOptions.IpAddress}:{connectionOptions.Port}]";
	}
}
