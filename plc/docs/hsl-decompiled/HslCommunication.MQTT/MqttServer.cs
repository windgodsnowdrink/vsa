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
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Net;
using HslCommunication.Core.Pipe;
using HslCommunication.Core.Security;
using HslCommunication.LogNet;
using HslCommunication.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HslCommunication.MQTT;

/// <summary>
/// 一个Mqtt的服务器类对象，本服务器支持发布订阅操作，支持从服务器强制推送数据，支持往指定的客户端推送，支持基于一问一答的远程过程调用（RPC）的数据交互，支持文件上传下载。根据这些功能从而定制化出满足各个场景的服务器，详细的使用说明可以参见代码api文档示例。
/// An Mqtt server class object. This server supports publish and subscribe operations, supports forced push data from the server,
/// supports push to designated clients, supports data interaction based on one-question-one-answer remote procedure calls (RPC),
/// and supports file upload and download . According to these functions, the server can be customized to meet various scenarios.
/// For detailed instructions, please refer to the code api document example.
/// </summary>
/// <remarks>
/// 本MQTT服务器功能丰富，可以同时实现，用户名密码验证，在线客户端的管理，数据订阅推送，单纯的数据收发，心跳检测，订阅通配符，同步数据访问，文件上传，下载，删除，遍历，详细参照下面的示例说明
/// 通配符请查看属性，规则参考：http://public.dhe.ibm.com/software/dw/webservices/ws-mqtt/mqtt-v3r1.html#appendix-a
/// </remarks>
public class MqttServer : CommunicationTcpServer, IDisposable
{
	public delegate OperateResult FileOperateVerificationDelegate(MqttSession session, byte code, string[] groups, string[] fileNames);

	public delegate void FileChangedDelegate(MqttSession session, MqttFileOperateInfo operateInfo);

	public delegate void OnClientApplicationMessageReceiveDelegate(MqttSession session, MqttClientApplicationMessage message);

	public delegate void OnClientConnectedDelegate(MqttSession session);

	public delegate int OnClientSubscribeDelegate(MqttSession session, List<string> topics);

	public delegate int ClientVerificationDelegate(MqttSession mqttSession, string clientId, string userName, string password);

	public delegate IPEndPoint ServiceRedirectionDelegate(MqttSession mqttSession, string clientId, string userName, string password);

	private Dictionary<string, MqttRpcApiInfo> apiTopicServiceDict;

	private object rpcApiLock;

	private readonly Dictionary<string, FileMarkId> dictionaryFilesMarks;

	private readonly object dictHybirdLock;

	private string filesDirectoryPath = null;

	private bool fileServerEnabled = false;

	private Dictionary<string, GroupFileContainer> m_dictionary_group_marks = new Dictionary<string, GroupFileContainer>();

	private SimpleHybirdLock group_marks_lock = new SimpleHybirdLock();

	private MqttFileMonitor fileMonitor = new MqttFileMonitor();

	private readonly Dictionary<string, MqttClientApplicationMessage> retainKeys;

	private readonly object keysLock;

	private readonly List<MqttSession> mqttSessions = new List<MqttSession>();

	private readonly object sessionsLock = new object();

	private Timer timerHeart;

	private LogStatisticsDict statisticsDict;

	private bool disposedValue;

	private RSACryptoServiceProvider providerMqttServer = null;

	private bool topicWildcard = false;

	private LogStatistics topicStatistics;

	private AsyncCallback beginReceiveCallback;

	private TimeSpan syncClientActiveTime = TimeSpan.FromDays(1.0);

	/// <summary>
	/// 可以自定义重新指向需要下载的文件路径信息，第一个参数是下载文件名称，第个参数是实际映射名称，需要返回新的文件路径和下载完成是否删除的信息
	/// You can customize the redirection of the file path that needs to be downloaded. The first parameter is the name of the downloaded file,
	/// </summary>
	public Func<MqttSession, string, string, OperateResult<string, bool>> DownloadFileRedirect { get; set; }

	public LogStatisticsDict LogStatistics => statisticsDict;

	/// <summary>
	/// 获取或设置是否启用订阅主题通配符的功能，默认为 False
	/// Gets or sets whether to enable the function of subscribing to the topic wildcard, the default is False
	/// </summary>
	/// <remarks>
	/// 启动之后，通配符示例：finance/stock/ibm/#; finance/+; '#' 是匹配所有主题，'+' 是匹配一级主题树。
	/// 通配符的规则参考如下的网址：http://public.dhe.ibm.com/software/dw/webservices/ws-mqtt/mqtt-v3r1.html#appendix-a
	/// </remarks>
	public bool TopicWildcard
	{
		get
		{
			return topicWildcard;
		}
		set
		{
			topicWildcard = value;
		}
	}

	public int OnlineCount => mqttSessions.Count;

	/// <summary>
	/// 获得当前所有的在线的MQTT客户端信息，包括异步的客户端及同步请求的客户端。
	/// Obtain all current online MQTT client information, including asynchronous client and synchronous request client.
	/// </summary>
	public MqttSession[] OnlineSessions
	{
		get
		{
			MqttSession[] result = null;
			lock (sessionsLock)
			{
				result = mqttSessions.ToArray();
			}
			return result;
		}
	}

	/// <summary>
	/// 获得当前异步客户端在线的MQTT客户端信息。
	/// Get the MQTT client information of the current asynchronous client online.
	/// </summary>
	public MqttSession[] MqttOnlineSessions
	{
		get
		{
			MqttSession[] result = null;
			lock (sessionsLock)
			{
				result = mqttSessions.Where((MqttSession m) => m.Protocol == "MQTT").ToArray();
			}
			return result;
		}
	}

	/// <summary>
	/// 获得当前同步客户端在线的MQTT客户端信息，如果客户端是短连接，将难以捕获在在线信息。
	/// Obtain the MQTT client information of the current synchronization client online. If the client is a short connection, it will be difficult to capture the online information.
	/// </summary>
	public MqttSession[] SyncOnlineSessions
	{
		get
		{
			MqttSession[] result = null;
			lock (sessionsLock)
			{
				result = mqttSessions.Where((MqttSession m) => m.Protocol == "HUSL").ToArray();
			}
			return result;
		}
	}

	/// <summary>
	/// 获取或设置当前的主题统计信息，默认按照天进行统计，连续统计30天。
	/// You can obtain or set the statistics of the current topic. By default, the statistics are collected every day for 30 consecutive days.
	/// </summary>
	public LogStatistics TopicStatistics
	{
		get
		{
			return topicStatistics;
		}
		set
		{
			topicStatistics = value;
		}
	}

	/// <summary>
	/// 同步客户端超时时间，当同步客户端不活动时间超过该时间时，将会强制下线，默认为24小时
	/// Set the timeout duration for the synchronization client. If the synchronization client remains inactive for longer than this period, it will be forcibly disconnected.
	/// </summary>
	public TimeSpan SyncClientActiveTime
	{
		get
		{
			return syncClientActiveTime;
		}
		set
		{
			if (value >= TimeSpan.FromSeconds(29.0))
			{
				syncClientActiveTime = value;
			}
		}
	}

	/// <summary>
	/// 获取或设置当前的最大数据包长度，超过该长度的数据将会被拒绝，默认值为-1，表示不限制
	/// Get or set the maximum packet length. Data exceeding this length will be rejected. The default value is -1, indicating no limit.
	/// </summary>
	public int MaxMqttPacketSize { get; set; } = -1;

	/// <summary>
	/// 当客户端进行文件操作时，校验客户端合法性的事件，操作码具体查看的常量值
	/// When client performing file operations, it is an event to verify the legitimacy of the client. For the operation code, check the constant value of
	/// </summary>
	public event FileOperateVerificationDelegate FileOperateVerification;

	/// <summary>
	/// 文件变化的事件，当文件上传的时候，文件下载的时候，文件被删除的时候触发。
	/// The file change event is triggered when the file is uploaded, when the file is downloaded, or when the file is deleted.
	/// </summary>
	public event FileChangedDelegate OnFileChangedEvent;

	/// <summary>
	/// 当收到客户端发来的消息时触发
	/// Triggered when a  message is received from the client
	/// </summary>
	public event OnClientApplicationMessageReceiveDelegate OnClientApplicationMessageReceive;

	/// <summary>
	/// Mqtt的客户端连接上来时触发
	/// Triggered when Mqtt client connects
	/// </summary>
	public event OnClientConnectedDelegate OnClientConnected;

	/// <summary>
	/// Mqtt的客户端下线时触发
	/// Triggered when Mqtt client connects
	/// </summary>
	public event OnClientConnectedDelegate OnClientDisConnected;

	/// <summary>
	/// 订阅主题事件，当客户端订阅主题时触发，返回 0 标识订阅通过，返回 其他值标识订阅失败
	/// Subscribe to the topic event. This triggers when the client subscribes to the topic. Returns 0 to indicate successful subscription, and returns other values to indicate subscription failure.
	/// </summary>
	public event OnClientSubscribeDelegate OnClientSubscribe;

	/// <summary>
	/// 当客户端连接时，触发的验证事件
	/// Validation event triggered when the client connects
	/// </summary>
	public event ClientVerificationDelegate ClientVerification;

	/// <summary>
	/// 当客户端连接时，服务器决定是否需要重定向地址操作，通常用于服务器程序迁移，负载均衡操作
	/// </summary>
	public event ServiceRedirectionDelegate ServiceRedirection;

	/// <summary>
	/// 实例化一个MQTT协议的服务器
	/// Instantiate a MQTT protocol server
	/// </summary>
	/// <param name="providerServer">RSA秘钥对象，默认为空，表示使用随机生成的秘钥信息，如果需要自定义的RSA密钥，则需要实例化当前参数对象，具体通信的时候加密与否，取决于客户端连接的方式。 The RSA key object, which is empty by default, indicates that the randomly generated key information is used. If a custom RSA key is required, the current parameter object needs to be instantiated. Whether the specific communication is encrypted or not depends on the way the client connects. .</param>
	public MqttServer(RSACryptoServiceProvider providerServer = null)
	{
		beginReceiveCallback = SocketReceiveCallback;
		statisticsDict = new LogStatisticsDict(GenerateMode.ByEveryDay, 60);
		retainKeys = new Dictionary<string, MqttClientApplicationMessage>();
		apiTopicServiceDict = new Dictionary<string, MqttRpcApiInfo>();
		keysLock = new object();
		rpcApiLock = new object();
		timerHeart = new Timer(ThreadTimerHeartCheck, null, 2000, 10000);
		dictionaryFilesMarks = new Dictionary<string, FileMarkId>();
		dictHybirdLock = new object();
		providerMqttServer = providerServer;
		topicStatistics = new LogStatistics(GenerateMode.ByEveryDay, 30);
	}

	protected override async void ThreadPoolLogin(PipeTcpNet pipe, IPEndPoint endPoint)
	{
		OperateResult<byte, byte[]> readMqtt = await MqttHelper.ReceiveMqttMessageAsync(pipe, 10000, null, MaxMqttPacketSize);
		if (!readMqtt.IsSuccess)
		{
			pipe.CloseCommunication();
			return;
		}
		RSACryptoServiceProvider clientKey = null;
		RSACryptoServiceProvider serverKey = null;
		if (readMqtt.Content1 == byte.MaxValue)
		{
			try
			{
				serverKey = providerMqttServer ?? new RSACryptoServiceProvider();
				clientKey = RSAHelper.CreateRsaProviderFromPublicKey(HslSecurity.ByteDecrypt(readMqtt.Content2));
				OperateResult send = pipe.Send(MqttHelper.BuildMqttCommand(byte.MaxValue, null, HslSecurity.ByteEncrypt(clientKey.EncryptLargeData(serverKey.GetPEMPublicKey()))).Content);
				if (!send.IsSuccess)
				{
					return;
				}
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				base.LogNet?.WriteError("创建客户端的公钥发生了异常！" + ex2.Message);
				pipe?.CloseCommunication();
				return;
			}
			readMqtt = await MqttHelper.ReceiveMqttMessageAsync(pipe, 10000, null, MaxMqttPacketSize);
			if (!readMqtt.IsSuccess)
			{
				pipe.CloseCommunication();
				return;
			}
		}
		HandleMqttConnection(pipe, endPoint, readMqtt, clientKey, serverKey);
	}

	private async void SocketReceiveCallback(IAsyncResult ar)
	{
		object asyncState = ar.AsyncState;
		MqttSession mqttSession = asyncState as MqttSession;
		if (mqttSession == null)
		{
			return;
		}
		try
		{
			mqttSession.MqttPipe.Socket.EndReceive(ar);
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			RemoveAndCloseSession(mqttSession, "Socket EndReceive -> " + ex2.Message);
			return;
		}
		if (mqttSession.Protocol == "FILE")
		{
			if (fileServerEnabled)
			{
				await HandleFileMessageAsync(mqttSession);
			}
			RemoveAndCloseSession(mqttSession, string.Empty);
		}
		else
		{
			await HandleWithReceiveMqtt(readMqtt: (!(mqttSession.Protocol == "MQTT")) ? (await MqttHelper.ReceiveMqttMessageAsync(mqttSession.MqttPipe, 60000, delegate(long already, long total)
			{
				SyncMqttReceiveProgressBack(mqttSession.MqttPipe, already, total);
			})) : (await MqttHelper.ReceiveMqttMessageAsync(mqttSession.MqttPipe, 60000, null, MaxMqttPacketSize)), mqttSession: mqttSession);
		}
	}

	private void SyncMqttReceiveProgressBack(PipeTcpNet pipe, long already, long total)
	{
		string message = ((total > 0) ? (already * 100 / total).ToString() : "100");
		byte[] buffer = new byte[16];
		BitConverter.GetBytes(already).CopyTo(buffer, 0);
		BitConverter.GetBytes(total).CopyTo(buffer, 8);
		pipe.Send(MqttHelper.BuildMqttCommand(15, 0, MqttHelper.BuildSegCommandByString(message), buffer).Content);
	}

	private void HandleMqttConnection(PipeTcpNet pipe, IPEndPoint endPoint, OperateResult<byte, byte[]> readMqtt, RSACryptoServiceProvider providerClient, RSACryptoServiceProvider providerServer)
	{
		if (!readMqtt.IsSuccess)
		{
			pipe.CloseCommunication();
			return;
		}
		byte[] array = readMqtt.Content2;
		if (providerClient != null)
		{
			try
			{
				array = providerServer.DecryptLargeData(array);
			}
			catch (Exception ex)
			{
				base.LogNet?.WriteError(ToString(), $"[{endPoint}] Decrypt the client's logon data exception！" + ex.Message);
				pipe?.CloseCommunication();
				return;
			}
		}
		OperateResult<int, MqttSession> operateResult = CheckMqttConnection(readMqtt.Content1, array, pipe, endPoint);
		if (!operateResult.IsSuccess)
		{
			base.LogNet?.WriteInfo(ToString(), $"[{endPoint}] Check client login failure: " + operateResult.Message);
			pipe?.CloseCommunication();
			return;
		}
		if (operateResult.Content1 != 0)
		{
			pipe.Send(MqttHelper.BuildMqttCommand(2, 0, null, new byte[2]
			{
				0,
				(byte)operateResult.Content1
			}).Content);
			pipe?.CloseCommunication();
			return;
		}
		if (providerClient == null)
		{
			pipe.Send(MqttHelper.BuildMqttCommand(2, 0, null, new byte[2]).Content);
		}
		else
		{
			operateResult.Content2.AesCryptography = new AesCryptography(HslHelper.HslRandom.GetBytes(16).ToHexString());
			byte[] payLoad = providerClient.Encrypt(Encoding.UTF8.GetBytes(operateResult.Content2.AesCryptography.Key), fOAEP: false);
			pipe.Send(MqttHelper.BuildMqttCommand(2, 0, new byte[2], payLoad).Content);
		}
		try
		{
			pipe.Socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, beginReceiveCallback, operateResult.Content2);
			AddMqttSession(operateResult.Content2);
		}
		catch (Exception ex2)
		{
			base.LogNet?.WriteDebug(ToString(), "Client Online Exception : " + ex2.Message);
			return;
		}
		if (operateResult.Content2.Protocol == "MQTT")
		{
			OnClientConnected?.Invoke(operateResult.Content2);
		}
	}

	private OperateResult<int, MqttSession> CheckMqttConnection(byte mqttCode, byte[] content, PipeTcpNet pipe, IPEndPoint endPoint)
	{
		if (mqttCode >> 4 != 1)
		{
			return new OperateResult<int, MqttSession>("Client Send Faied, And Close!");
		}
		if (content.Length < 10)
		{
			return new OperateResult<int, MqttSession>("Receive Data Too Short:" + SoftBasic.ByteToHexString(content, ' '));
		}
		string message = Encoding.ASCII.GetString(content, 2, 4);
		if (!(message == "MQTT") && !(message == "HUSL") && !(message == "FILE"))
		{
			return new OperateResult<int, MqttSession>("Not Mqtt Client Connection");
		}
		try
		{
			int index = 10;
			string clientId = MqttHelper.ExtraMsgFromBytes(content, ref index);
			string willTopic = (((content[7] & 4) == 4) ? MqttHelper.ExtraMsgFromBytes(content, ref index) : string.Empty);
			byte[] willMessage = (((content[7] & 4) == 4) ? MqttHelper.ExtraByteMsgFromBytes(content, ref index) : new byte[0]);
			string userName = (((content[7] & 0x80) == 128) ? MqttHelper.ExtraMsgFromBytes(content, ref index) : string.Empty);
			string password = (((content[7] & 0x40) == 64) ? MqttHelper.ExtraMsgFromBytes(content, ref index) : string.Empty);
			int num = content[8] * 256 + content[9];
			MqttSession mqttSession = new MqttSession(endPoint, message)
			{
				MqttPipe = pipe,
				ClientId = clientId,
				UserName = userName,
				WillTopic = willTopic,
				WillMessage = willMessage
			};
			if (message == "MQTT")
			{
				mqttSession.DeveloperPermissions = false;
			}
			else if (string.Equals(mqttSession.UserName, "admin", StringComparison.OrdinalIgnoreCase))
			{
				mqttSession.DeveloperPermissions = true;
			}
			if (ServiceRedirection != null)
			{
				IPEndPoint iPEndPoint = ServiceRedirection(mqttSession, clientId, userName, password);
				if (iPEndPoint != null)
				{
					byte[] buffer = new byte[20];
					try
					{
						iPEndPoint.Address.GetAddressBytes().CopyTo(buffer, 4);
						BitConverter.GetBytes((ushort)iPEndPoint.Port).CopyTo(buffer, 2);
						buffer[0] = byte.MaxValue;
						buffer[1] = byte.MaxValue;
						pipe.Send(MqttHelper.BuildMqttCommand(2, 0, null, buffer).Content);
						return new OperateResult<int, MqttSession>($"ServiceRedirection : {iPEndPoint}");
					}
					catch
					{
					}
				}
			}
			int value = ((ClientVerification != null) ? ClientVerification(mqttSession, clientId, userName, password) : 0);
			if (num > 0)
			{
				mqttSession.ActiveTimeSpan = TimeSpan.FromSeconds((double)num * 1.5);
			}
			return OperateResult.CreateSuccessResult(value, mqttSession);
		}
		catch (Exception ex)
		{
			return new OperateResult<int, MqttSession>("Client Online Exception : " + ex.Message);
		}
	}

	private async Task HandleWithReceiveMqtt(MqttSession mqttSession, OperateResult<byte, byte[]> readMqtt)
	{
		if (!readMqtt.IsSuccess)
		{
			RemoveAndCloseSession(mqttSession, readMqtt.Message);
			return;
		}
		byte code = readMqtt.Content1;
		byte[] data = readMqtt.Content2;
		try
		{
			if (code >> 4 == 14)
			{
				RemoveAndCloseSession(mqttSession, string.Empty);
				return;
			}
			mqttSession.MqttPipe.Socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, beginReceiveCallback, mqttSession);
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			RemoveAndCloseSession(mqttSession, "HandleWithReceiveMqtt exception:" + ex2.Message);
			return;
		}
		mqttSession.ActiveTime = DateTime.Now;
		if (mqttSession.Protocol != "MQTT")
		{
			await DealWithPublish(mqttSession, code, data);
		}
		else if (code >> 4 == 3)
		{
			await DealWithPublish(mqttSession, code, data);
		}
		else if (code >> 4 != 4 && code >> 4 != 5)
		{
			if (code >> 4 == 6)
			{
				mqttSession.MqttPipe.Send(MqttHelper.BuildMqttCommand(7, 0, null, data).Content);
			}
			else if (code >> 4 == 8)
			{
				DealWithSubscribe(mqttSession, code, data);
			}
			else if (code >> 4 == 10)
			{
				DealWithUnSubscribe(mqttSession, code, data);
			}
			else if (code >> 4 == 12)
			{
				mqttSession.MqttPipe.Send(MqttHelper.BuildMqttCommand(13, 0, null, null).Content);
			}
		}
	}

	protected override void ExtraOnStart()
	{
		base.ExtraOnStart();
	}

	protected override void ExtraOnClose()
	{
		base.ExtraOnClose();
		lock (sessionsLock)
		{
			for (int i = 0; i < mqttSessions.Count; i++)
			{
				mqttSessions[i].MqttPipe?.CloseCommunication();
			}
			mqttSessions.Clear();
		}
	}

	private void ThreadTimerHeartCheck(object obj)
	{
		MqttSession[] array = null;
		lock (sessionsLock)
		{
			array = mqttSessions.ToArray();
		}
		if (array == null || array.Length == 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Protocol == "MQTT" && DateTime.Now - array[i].ActiveTime > array[i].ActiveTimeSpan)
			{
				RemoveAndCloseSession(array[i], "Thread Timer Heart Check failed:" + SoftBasic.GetTimeSpanDescription(DateTime.Now - array[i].ActiveTime));
			}
			else if (array[i].Protocol == "HUSL" && DateTime.Now - array[i].ActiveTime > syncClientActiveTime)
			{
				RemoveAndCloseSession(array[i], "Thread Timer Heart Check failed:" + SoftBasic.GetTimeSpanDescription(DateTime.Now - array[i].ActiveTime));
			}
		}
	}

	private async Task DealWithPublish(MqttSession session, byte code, byte[] data)
	{
		OperateResult<MqttClientApplicationMessage> messageResult = MqttHelper.ParseMqttClientApplicationMessage(session, code, data);
		if (!messageResult.IsSuccess)
		{
			RemoveAndCloseSession(session, messageResult.Message);
			return;
		}
		MqttClientApplicationMessage mqttClientApplicationMessage = messageResult.Content;
		if (session.Protocol == "MQTT")
		{
			MqttQualityOfServiceLevel mqttQuality = mqttClientApplicationMessage.QualityOfServiceLevel;
			switch (mqttQuality)
			{
			case MqttQualityOfServiceLevel.AtLeastOnce:
				session.SendMqttCommand(4, 0, null, MqttHelper.BuildIntBytes(mqttClientApplicationMessage.MsgID));
				break;
			case MqttQualityOfServiceLevel.ExactlyOnce:
				session.SendMqttCommand(5, 0, null, MqttHelper.BuildIntBytes(mqttClientApplicationMessage.MsgID));
				break;
			}
			if (session.ForbidPublishTopic)
			{
				return;
			}
			OnClientApplicationMessageReceive?.Invoke(session, mqttClientApplicationMessage);
			if (mqttQuality != MqttQualityOfServiceLevel.OnlyTransfer && !mqttClientApplicationMessage.IsCancelPublish)
			{
				PublishTopicPayload(mqttClientApplicationMessage.Topic, mqttClientApplicationMessage.Payload, retain: false);
				if (mqttClientApplicationMessage.Retain)
				{
					RetainTopicPayload(mqttClientApplicationMessage.Topic, mqttClientApplicationMessage);
				}
				topicStatistics.StatisticsAdd(1L);
			}
		}
		else if (code >> 4 == 3)
		{
			string apiName = mqttClientApplicationMessage.Topic.Trim('/');
			MqttRpcApiInfo apiInformation = GetMqttRpcApiInfo(apiName);
			if (apiInformation == null)
			{
				OnClientApplicationMessageReceive?.Invoke(session, mqttClientApplicationMessage);
				return;
			}
			DateTime dateTime = DateTime.Now;
			OperateResult<string> result = await MqttHelper.HandleObjectMethod(session, mqttClientApplicationMessage, apiInformation);
			double timeSpend = Math.Round((DateTime.Now - dateTime).TotalSeconds, 5);
			apiInformation.CalledCountAddOne((long)(timeSpend * 100000.0));
			statisticsDict.StatisticsAdd(apiInformation.ApiTopic, 1L);
			base.LogNet?.WriteDebug(ToString(), $"{session} RPC: [{mqttClientApplicationMessage.Topic}] Spend:[{timeSpend * 1000.0:F2} ms] Count:[{apiInformation.CalledCount}] Return:[{result.IsSuccess}]");
			ReportOperateResult(session, result);
		}
		else if (code >> 4 == 8)
		{
			if (session.DeveloperPermissions)
			{
				ReportOperateResult(session, OperateResult.CreateSuccessResult(((object)JArray.FromObject((object)GetAllMqttRpcApiInfo())).ToString()));
			}
			else
			{
				ReportOperateResult(session, StringResources.Language.DeveloperPrivileges);
			}
		}
		else if (code >> 4 == 4)
		{
			if (session.DeveloperPermissions)
			{
				PublishTopicPayload(session, "", HslProtocol.PackStringArrayToByte(GetAllRetainTopics()));
			}
			else
			{
				ReportOperateResult(session, StringResources.Language.DeveloperPrivileges);
			}
		}
		else if (code >> 4 == 6)
		{
			if (session.DeveloperPermissions)
			{
				long[] logs = (string.IsNullOrEmpty(mqttClientApplicationMessage.Topic) ? LogStatistics.LogStat.GetStatisticsSnapshot() : LogStatistics.GetStatisticsSnapshot(mqttClientApplicationMessage.Topic));
				if (logs == null)
				{
					ReportOperateResult(session, new OperateResult<string>($"{session} RPC:{mqttClientApplicationMessage.Topic} has no data or not exist."));
				}
				else
				{
					ReportOperateResult(session, OperateResult.CreateSuccessResult(logs.ToArrayString()));
				}
			}
			else
			{
				ReportOperateResult(session, StringResources.Language.DeveloperPrivileges);
			}
		}
		else if (code >> 4 == 11)
		{
			if (session.DeveloperPermissions)
			{
				ReportOperateResult(session, OperateResult.CreateSuccessResult(((object)JArray.FromObject((object)OnlineSessions.Select((MqttSession m) => m.GetSessionInfo()))).ToString()));
			}
			else
			{
				ReportOperateResult(session, StringResources.Language.DeveloperPrivileges);
			}
		}
		else
		{
			if (code >> 4 != 5)
			{
				return;
			}
			if (session.DeveloperPermissions)
			{
				lock (keysLock)
				{
					if (retainKeys.ContainsKey(mqttClientApplicationMessage.Topic))
					{
						PublishTopicPayload(payload: Encoding.UTF8.GetBytes(retainKeys[mqttClientApplicationMessage.Topic].ToJsonString((Formatting)1)), session: session, topic: mqttClientApplicationMessage.Topic);
						return;
					}
					ReportOperateResult(session, StringResources.Language.KeyIsNotExist);
				}
			}
			else
			{
				ReportOperateResult(session, StringResources.Language.DeveloperPrivileges);
			}
		}
	}

	/// <summary>
	/// 将消息进行驻留到内存词典，方便进行其他的功能操作。
	/// </summary>
	/// <param name="topic">消息的主题</param>
	/// <param name="payload">当前的数据负载</param>
	private void RetainTopicPayload(string topic, byte[] payload)
	{
		MqttClientApplicationMessage value = new MqttClientApplicationMessage
		{
			ClientId = "MqttServer",
			QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce,
			Retain = true,
			Topic = topic,
			UserName = "MqttServer",
			Payload = payload
		};
		lock (keysLock)
		{
			if (retainKeys.ContainsKey(topic))
			{
				retainKeys[topic] = value;
			}
			else
			{
				retainKeys.Add(topic, value);
			}
		}
	}

	/// <summary>
	/// 将消息进行驻留到内存词典，方便进行其他的功能操作。
	/// </summary>
	/// <param name="topic">消息的主题</param>
	/// <param name="message">当前的Mqtt消息</param>
	private void RetainTopicPayload(string topic, MqttClientApplicationMessage message)
	{
		lock (keysLock)
		{
			if (retainKeys.ContainsKey(topic))
			{
				retainKeys[topic] = message;
			}
			else
			{
				retainKeys.Add(topic, message);
			}
		}
	}

	private void DealWithSubscribe(MqttSession session, byte code, byte[] data)
	{
		int num = 0;
		int index = 0;
		num = MqttHelper.ExtraIntFromBytes(data, ref index);
		List<string> list = new List<string>();
		List<byte> list2 = new List<byte>();
		try
		{
			while (index < data.Length - 1)
			{
				MqttHelper.ExtraSubscribeMsgFromBytes(data, ref index, list, list2);
			}
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteError(ToString(), $"{session} DealWithSubscribe exception: " + ex.Message + " Source: " + data.ToHexString(' '));
			return;
		}
		if (OnClientSubscribe != null)
		{
			int num2 = OnClientSubscribe(session, list);
			if (num2 > 0)
			{
				session.SendMqttCommand(9, (byte)num2, MqttHelper.BuildIntBytes(num), list2.ToArray());
				return;
			}
		}
		session.SendMqttCommand(9, 0, MqttHelper.BuildIntBytes(num), list2.ToArray());
		lock (keysLock)
		{
			if (topicWildcard)
			{
				foreach (KeyValuePair<string, MqttClientApplicationMessage> retainKey in retainKeys)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (MqttHelper.CheckMqttTopicWildcards(retainKey.Key, list[i]))
						{
							session.MqttPipe.Send(MqttHelper.BuildPublishMqttCommand(retainKey.Key, retainKey.Value.Payload, retain: true, session.IsAesCryptography ? session.AesCryptography : null).Content);
						}
					}
				}
			}
			else
			{
				for (int j = 0; j < list.Count; j++)
				{
					if (retainKeys.ContainsKey(list[j]))
					{
						session.MqttPipe.Send(MqttHelper.BuildPublishMqttCommand(list[j], retainKeys[list[j]].Payload, retain: true, session.IsAesCryptography ? session.AesCryptography : null).Content);
					}
				}
			}
		}
		session.AddSubscribe(list.ToArray());
		base.LogNet?.WriteDebug(ToString(), session.ToString() + " Subscribe: " + list.ToArray().ToArrayString());
	}

	private void DealWithUnSubscribe(MqttSession session, byte code, byte[] data)
	{
		int num = 0;
		int index = 0;
		num = MqttHelper.ExtraIntFromBytes(data, ref index);
		List<string> list = new List<string>();
		while (index < data.Length)
		{
			list.Add(MqttHelper.ExtraMsgFromBytes(data, ref index));
		}
		session.SendMqttCommand(11, 0, null, MqttHelper.BuildIntBytes(num));
		session.RemoveSubscribe(list.ToArray());
		base.LogNet?.WriteDebug(ToString(), session.ToString() + " UnSubscribe: " + list.ToArray().ToArrayString());
	}

	/// <summary>
	/// 向指定的客户端发送主题及负载数据
	/// Sends the topic and payload data to the specified client
	/// </summary>
	/// <param name="session">会话内容</param>
	/// <param name="topic">主题</param>
	/// <param name="payload">消息内容</param>
	public void PublishTopicPayload(MqttSession session, string topic, byte[] payload)
	{
		OperateResult operateResult = session.MqttPipe.Send(MqttHelper.BuildPublishMqttCommand(topic, payload, retain: false, session.IsAesCryptography ? session.AesCryptography : null).Content);
		if (!operateResult.IsSuccess)
		{
			base.LogNet?.WriteError(ToString(), $"{session} PublishTopicPayload Failed:" + operateResult.Message);
		}
		topicStatistics.StatisticsAdd(1L);
	}

	/// <summary>
	/// 向指定的客户端发送主题及负载数据，通常用于同步返回文件流给客户端
	/// Send the topic and payload data to the designated client, which is usually used to synchronously return the file stream to the client.
	/// </summary>
	/// <param name="session">会话内容</param>
	/// <param name="topic">主题</param>
	/// <param name="payload">数据流</param>
	public void PublishTopicPayload(MqttSession session, string topic, Stream payload)
	{
		OperateResult<byte[]> operateResult = MqttHelper.BuildPublishMqttCommandStream(topic, payload);
		OperateResult operateResult2 = session.MqttPipe.Send(operateResult.Content);
		byte[] buffer = new byte[4096];
		int num = 0;
		while (true)
		{
			if (!operateResult2.IsSuccess)
			{
				base.LogNet?.WriteError(ToString(), $"{session} PublishTopicPayload Stream Failed:" + operateResult2.Message);
				break;
			}
			try
			{
				int bytesRead = payload.Read(buffer, 0, buffer.Length);
				if (bytesRead > 0)
				{
					operateResult2 = session.MqttPipe.Send(buffer, 0, bytesRead);
					num += bytesRead;
					if (num < payload.Length)
					{
						continue;
					}
				}
			}
			catch (Exception ex)
			{
				base.LogNet?.WriteError(ToString(), $"{session} PublishTopicPayload Stream Failed:" + ex.Message);
				continue;
			}
			break;
		}
		topicStatistics.StatisticsAdd(1L);
	}

	/// <summary>
	/// 使用指定的规则向客户端发布主题及负载数据，可以根据会话的登录用户名，客户端ID信息进行筛选，例如只发布用户名admin的账户：( session ) => session.UserName == "admin"
	/// Use the specified rules to publish topic and load data to the client, which can be filtered according to the session login user name and client ID information.
	/// For example, only the account with the user name admin is published: ( session ) => session.UserName == "admin"
	/// </summary>
	/// <param name="topic">主题</param>
	/// <param name="payload">消息内容</param>
	/// <param name="retain">是否在服务器驻留</param>
	/// <param name="check">会话的检查委托</param>
	public void PublishTopicPayload(string topic, byte[] payload, bool retain, Func<MqttSession, bool> check)
	{
		MqttSession[] array = null;
		lock (sessionsLock)
		{
			array = mqttSessions.ToArray();
		}
		if (array == null || array.Length == 0)
		{
			return;
		}
		byte[] array2 = null;
		foreach (MqttSession mqttSession in array)
		{
			byte[] array3 = null;
			if (!(mqttSession.Protocol == "MQTT") || !check(mqttSession))
			{
				continue;
			}
			if (mqttSession.IsAesCryptography)
			{
				if (array3 == null)
				{
					array3 = MqttHelper.BuildPublishMqttCommand(topic, payload, retain: false, mqttSession.AesCryptography).Content;
				}
			}
			else if (array2 == null)
			{
				array2 = MqttHelper.BuildPublishMqttCommand(topic, payload).Content;
			}
			OperateResult operateResult = mqttSession.MqttPipe.Send(mqttSession.IsAesCryptography ? array3 : array2);
			if (!operateResult.IsSuccess)
			{
				base.LogNet?.WriteError(ToString(), $"{mqttSession} PublishTopicPayload Failed:" + operateResult.Message);
			}
		}
		if (retain)
		{
			RetainTopicPayload(topic, payload);
		}
		topicStatistics.StatisticsAdd(1L);
	}

	/// <summary>
	/// 从服务器向订阅了指定的主题的客户端发送消息，默认消息不驻留
	/// Sends a message from the server to a client that subscribes to the specified topic; the default message does not retain
	/// </summary>
	/// <param name="topic">主题</param>
	/// <param name="payload">消息内容</param>
	/// <param name="retain">指示消息是否驻留</param>
	public void PublishTopicPayload(string topic, byte[] payload, bool retain = true)
	{
		PublishTopicPayload(topic, payload, retain, (MqttSession session) => session.IsClientSubscribe(topic, topicWildcard));
	}

	/// <summary>
	/// 向所有的客户端强制发送主题及负载数据，默认消息不驻留
	/// Send subject and payload data to all clients compulsively, and the default message does not retain
	/// </summary>
	/// <param name="topic">主题</param>
	/// <param name="payload">消息内容</param>
	/// <param name="retain">指示消息是否驻留</param>
	public void PublishAllClientTopicPayload(string topic, byte[] payload, bool retain = false)
	{
		PublishTopicPayload(topic, payload, retain, (MqttSession session) => true);
	}

	/// <summary>
	/// 向指定的客户端ID强制发送消息，默认消息不驻留
	/// Forces a message to the specified client ID, and the default message does not retain
	/// </summary>
	/// <param name="clientId">指定的客户端ID信息</param>
	/// <param name="topic">主题</param>
	/// <param name="payload">消息内容</param>
	/// <param name="retain">指示消息是否驻留</param>
	public void PublishTopicPayload(string clientId, string topic, byte[] payload, bool retain = false)
	{
		PublishTopicPayload(topic, payload, retain, (MqttSession session) => session.ClientId == clientId);
	}

	/// <summary>
	/// 向客户端发布一个进度报告的信息，仅用于同步网络的时候才支持进度报告，将进度及消息发送给客户端，比如你的服务器需要分成5个部分完成，可以按照百分比提示给客户端当前服务器发生了什么
	/// Publish the information of a progress report to the client. The progress report is only supported when the network is synchronized.
	/// The progress and the message are sent to the client. For example, your server needs to be divided into 5 parts to complete.
	/// You can prompt the client according to the percentage. What happened to the server
	/// </summary>
	/// <param name="session">当前的网络会话</param>
	/// <param name="topic">回发客户端的关键数据，可以是百分比字符串，甚至是自定义的任意功能</param>
	/// <param name="payload">数据消息</param>
	public void ReportProgress(MqttSession session, string topic, string payload)
	{
		if (session.Protocol == "HUSL")
		{
			payload = payload ?? string.Empty;
			OperateResult operateResult = session.SendMqttCommand(15, 0, MqttHelper.BuildSegCommandByString(topic), Encoding.UTF8.GetBytes(payload));
			if (!operateResult.IsSuccess)
			{
				base.LogNet?.WriteError(ToString(), $"{session} PublishTopicPayload Failed:" + operateResult.Message);
			}
			return;
		}
		throw new Exception("ReportProgress only support sync communication");
	}

	/// <summary>
	/// 向客户端发布一个失败的操作信息，仅用于同步网络的时候反馈失败结果，将错误的信息反馈回客户端，客户端就知道服务器发生了什么，为什么反馈失败。
	/// Publish a failed operation information to the client, which is only used to feed back the failure result when synchronizing the network.
	/// If the error information is fed back to the client, the client will know what happened to the server and why the feedback failed.
	/// </summary>
	/// <param name="session">当前的网络会话</param>
	/// <param name="message">错误的消息文本信息</param>
	public void ReportOperateResult(MqttSession session, string message)
	{
		ReportOperateResult(session, new OperateResult<string>(message));
	}

	/// <summary>
	/// 向客户端发布一个操作结果的信息，仅用于同步网络的时候反馈操作结果，该操作可能成功，可能失败，客户端就知道服务器发生了什么，以及结果如何。
	/// Publish an operation result information to the client, which is only used to feed back the operation result when synchronizing the network.
	/// The operation may succeed or fail, and the client knows what happened to the server and the result.
	/// </summary>
	/// <param name="session">当前的网络会话</param>
	/// <param name="result">结果对象内容</param>
	public void ReportOperateResult(MqttSession session, OperateResult<string> result)
	{
		if (session.Protocol == "HUSL")
		{
			if (result.IsSuccess)
			{
				byte[] payload = (string.IsNullOrEmpty(result.Content) ? new byte[0] : Encoding.UTF8.GetBytes(result.Content));
				PublishTopicPayload(session, result.ErrorCode.ToString(), payload);
				return;
			}
			OperateResult operateResult = session.SendMqttCommand(0, 0, MqttHelper.BuildSegCommandByString(result.ErrorCode.ToString()), string.IsNullOrEmpty(result.Message) ? new byte[0] : Encoding.UTF8.GetBytes(result.Message), session.IsAesCryptography ? session.AesCryptography : null);
			if (!operateResult.IsSuccess)
			{
				base.LogNet?.WriteError(ToString(), $"{session} PublishTopicPayload Failed:" + operateResult.Message);
			}
			return;
		}
		throw new Exception("Report Result Message only support sync communication, client is MqttSyncClient");
	}

	/// <summary>
	/// 使用指定的对象来返回网络的API接口，前提是传入的数据为json参数，返回的数据为 OperateResult<string> 数据，详细参照说明
	/// Use the specified object to return the API interface of the network,
	/// provided that the incoming data is json parameters and the returned data is OperateResult<string> data,
	/// please refer to the description for details
	/// </summary>
	/// <param name="session">当前的会话内容</param>
	/// <param name="message">客户端发送的消息，其中的payload将会解析为一个json字符串，然后提取参数信息。</param>
	/// <param name="apiObject">当前的对象的内容信息</param>
	public async Task ReportObjectApiMethod(MqttSession session, MqttClientApplicationMessage message, object apiObject)
	{
		if (session.Protocol == "HUSL")
		{
			ReportOperateResult(session, await MqttHelper.HandleObjectMethod(session, message, apiObject));
			return;
		}
		throw new Exception("Report Result Message only support sync communication, client is MqttSyncClient");
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

	/// <summary>
	/// 获取当前所有注册的RPC接口信息，将返回一个数据列表。
	/// Get all currently registered RPC interface information, and a data list will be returned.
	/// </summary>
	/// <returns>信息列表</returns>
	public MqttRpcApiInfo[] GetAllMqttRpcApiInfo()
	{
		MqttRpcApiInfo[] result = null;
		lock (rpcApiLock)
		{
			result = apiTopicServiceDict.Values.ToArray();
		}
		return result;
	}

	/// <summary>
	/// 注册一个RPC的服务接口，可以指定当前的控制器名称，以及提供RPC服务的原始对象，指定统一的权限控制。
	/// Register an RPC service interface, you can specify the current controller name, and the original object that provides the RPC service, Specify unified access control
	/// </summary>
	/// <param name="api">前置的接口信息，可以理解为MVC模式的控制器</param>
	/// <param name="obj">原始对象信息，也可以是类型对象本身</param>
	/// <param name="permissionAttribute">统一的权限访问配置，将会覆盖单个方法的权限控制。</param>
	/// <remarks>
	/// 如果设置了的参数信息，那么请确保调用时获取了企业商业授权的权限，否则，只能使用24小时，然后因为权限问题报错。
	/// </remarks>
	public void RegisterMqttRpcApi(string api, object obj, HslMqttPermissionAttribute permissionAttribute)
	{
		lock (rpcApiLock)
		{
			foreach (MqttRpcApiInfo item in MqttHelper.GetSyncServicesApiInformationFromObject(api, obj, permissionAttribute))
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
	public void RegisterMqttRpcApi(string api, object obj)
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
	public void RegisterMqttRpcApi(object obj)
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
	public void UnRegisterMqttRpcApi(string api, object obj)
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
	public void UnRegisterMqttRpcApi(object obj)
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
	/// 启动文件服务功能，协议头为FILE，需要指定服务器存储的文件路径
	/// Start the file service function, the protocol header is FILE, you need to specify the file path stored by the server
	/// </summary>
	/// <param name="filePath">文件的存储路径</param>
	public void UseFileServer(string filePath)
	{
		filesDirectoryPath = filePath;
		fileServerEnabled = true;
		CheckFolderAndCreate();
	}

	/// <summary>
	/// 关闭文件服务功能
	/// </summary>
	public void CloseFileServer()
	{
		fileServerEnabled = false;
	}

	[HslMqttApi(Description = "Get the current number of file management containers for the folder")]
	/// <summary>
	/// 获取当前的针对文件夹的文件管理容器的数量
	/// Get the current number of file management containers for the folder
	/// </summary>
	public int GroupFileContainerCount()
	{
		return m_dictionary_group_marks.Count;
	}

	[HslMqttApi(Description = "Obtain current real-time file upload and download monitoring information, operating client information, file classification, file name, upload or download speed, etc.")]
	/// <summary>
	/// 获取当前实时的文件上传下载的监控信息，操作的客户端信息，文件分类，文件名，上传或下载的速度等
	/// Obtain current real-time file upload and download monitoring information, operating client information, file classification, file name, upload or download speed, etc.
	/// </summary>
	/// <returns>文件的监控信息</returns>
	public MqttFileMonitorItem[] GetMonitorItemsSnapShoot()
	{
		return fileMonitor.GetMonitorItemsSnapShoot();
	}

	private bool CheckPathAndFilenameLegal(string input)
	{
		if (input != null && input.Contains(".."))
		{
			return false;
		}
		return Regex.IsMatch(input, "[:?*/\\<>|\"]");
	}

	private async Task HandleFileMessageAsync(MqttSession session)
	{
		OperateResult<byte, byte[]> receiveGroupInfo = await MqttHelper.ReceiveMqttMessageAsync(session.MqttPipe, 60000);
		if (!receiveGroupInfo.IsSuccess)
		{
			return;
		}
		string[] groupInfo = HslProtocol.UnPackStringArrayFromByte(receiveGroupInfo.Content2);
		OperateResult<byte, byte[]> receiveFileNames = await MqttHelper.ReceiveMqttMessageAsync(session.MqttPipe, 60000);
		if (!receiveFileNames.IsSuccess)
		{
			return;
		}
		string[] fileNames = HslProtocol.UnPackStringArrayFromByte(receiveFileNames.Content2);
		for (int i = 0; i < groupInfo.Length; i++)
		{
			if (CheckPathAndFilenameLegal(groupInfo[i]))
			{
				session.SendMqttCommand(0, null, HslHelper.GetUTF8Bytes("Path Invalid, not include '\\/:*?\"<>|'"));
				RemoveAndCloseSession(session, "CheckPathAndFilenameLegal:" + groupInfo[i]);
				return;
			}
		}
		for (int j = 0; j < fileNames.Length; j++)
		{
			if (receiveFileNames.Content1 != 111)
			{
				if (CheckPathAndFilenameLegal(fileNames[j]))
				{
					session.SendMqttCommand(0, null, HslHelper.GetUTF8Bytes("FileName Invalid, not include '\\/:*?\"<>|'"));
					RemoveAndCloseSession(session, "CheckPathAndFilenameLegal:" + fileNames[j]);
					return;
				}
			}
			else if (Regex.IsMatch(fileNames[j], "[:?*<>|\"]"))
			{
				session.SendMqttCommand(0, null, HslHelper.GetUTF8Bytes("FileName Invalid, not include ':*?\"<>|'"));
				RemoveAndCloseSession(session, "CheckPathAndFilenameLegal:" + fileNames[j]);
				return;
			}
		}
		OperateResult opLegal = FileOperateVerification?.Invoke(session, receiveFileNames.Content1, groupInfo, fileNames);
		if (opLegal == null)
		{
			opLegal = OperateResult.CreateSuccessResult();
		}
		OperateResult sendLegal = await session.SendMqttCommandAsync((byte)(opLegal.IsSuccess ? 100 : 0), null, HslHelper.GetUTF8Bytes(opLegal.Message));
		if (!opLegal.IsSuccess)
		{
			RemoveAndCloseSession(session, "FileOperateVerification:" + opLegal.Message);
			return;
		}
		if (!sendLegal.IsSuccess)
		{
			RemoveAndCloseSession(session, "FileOperate SendLegal:" + sendLegal.Message);
			return;
		}
		string relativeName = GetRelativeFileName(groupInfo, (fileNames != null && fileNames.Length != 0) ? fileNames[0] : string.Empty);
		if (receiveFileNames.Content1 == 101)
		{
			string fileName = fileNames[0];
			string guidName = TransformFactFileName(groupInfo, fileName);
			FileMarkId fileMarkId = GetFileMarksFromDictionaryWithFileName(guidName);
			fileMarkId.EnterReadOperator();
			DateTime dateTimeStart = DateTime.Now;
			MqttFileMonitorItem monitorItem = new MqttFileMonitorItem
			{
				EndPoint = session.EndPoint,
				ClientId = session.ClientId,
				UserName = session.UserName,
				FileName = fileName,
				MappingName = guidName,
				Operate = "Download",
				Groups = HslHelper.PathCombine(groupInfo)
			};
			fileMonitor.Add(monitorItem);
			string fullDownloadFileName = ReturnAbsoluteFileName(groupInfo, guidName);
			bool deleteFile = false;
			if (DownloadFileRedirect != null)
			{
				OperateResult<string, bool> redirect = DownloadFileRedirect(session, fileName, fullDownloadFileName);
				if (redirect.IsSuccess)
				{
					fullDownloadFileName = redirect.Content1;
					deleteFile = redirect.Content2;
				}
			}
			OperateResult send = await MqttHelper.SendMqttFileAsync(session.MqttPipe, fullDownloadFileName, fileName, "", monitorItem.UpdateProgress, session.IsAesCryptography ? session.AesCryptography : null);
			fileMarkId.LeaveReadOperator();
			fileMonitor.Remove(monitorItem.UniqueId);
			OnFileChangedEvent?.Invoke(session, new MqttFileOperateInfo
			{
				Groups = HslHelper.PathCombine(groupInfo),
				FileNames = fileNames,
				MappingNames = new string[1] { guidName },
				Operate = "Download",
				TimeCost = DateTime.Now - dateTimeStart
			});
			if (deleteFile)
			{
				DeleteFileByName(fullDownloadFileName);
			}
			if (!send.IsSuccess)
			{
				base.LogNet?.WriteError(ToString(), $"{session} {StringResources.Language.FileDownloadFailed}[{send.Message}]:{relativeName} Name:{session.UserName}" + " Spend:" + SoftBasic.GetTimeSpanDescription(DateTime.Now - dateTimeStart));
			}
			else
			{
				base.LogNet?.WriteInfo(ToString(), $"{session} {StringResources.Language.FileDownloadSuccess}:{relativeName} Spend:{SoftBasic.GetTimeSpanDescription(DateTime.Now - dateTimeStart)}");
			}
		}
		else if (receiveFileNames.Content1 == 102)
		{
			string fileName2 = fileNames[0];
			string fullFileName = ReturnAbsoluteFileName(groupInfo, fileName2);
			CheckFolderAndCreate();
			FileInfo info = new FileInfo(fullFileName);
			try
			{
				if (!Directory.Exists(info.DirectoryName))
				{
					Directory.CreateDirectory(info.DirectoryName);
				}
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				base.LogNet?.WriteException(ToString(), StringResources.Language.FilePathCreateFailed + fullFileName, ex2);
				return;
			}
			string guidName2 = SoftBasic.GetUniqueStringByGuidAndRandom();
			DateTime dateTimeStart2 = DateTime.Now;
			MqttFileMonitorItem monitorItem2 = new MqttFileMonitorItem
			{
				EndPoint = session.EndPoint,
				ClientId = session.ClientId,
				UserName = session.UserName,
				FileName = fileName2,
				MappingName = guidName2,
				Operate = "Upload",
				Groups = HslHelper.PathCombine(groupInfo)
			};
			fileMonitor.Add(monitorItem2);
			OperateResult<FileBaseInfo> receive = await ReceiveMqttFileAndUpdateGroupAsync(session, info, guidName2, monitorItem2.UpdateProgress);
			fileMonitor.Remove(monitorItem2.UniqueId);
			if (receive.IsSuccess)
			{
				OnFileChangedEvent?.Invoke(session, new MqttFileOperateInfo
				{
					Groups = HslHelper.PathCombine(groupInfo),
					FileNames = fileNames,
					MappingNames = new string[1] { guidName2 },
					Operate = "Upload",
					TimeCost = DateTime.Now - dateTimeStart2
				});
				base.LogNet?.WriteInfo(ToString(), $"{session} {StringResources.Language.FileUploadSuccess}:{relativeName} Spend:{SoftBasic.GetTimeSpanDescription(DateTime.Now - dateTimeStart2)}");
			}
			else
			{
				base.LogNet?.WriteError(ToString(), $"{session} {StringResources.Language.FileUploadFailed}[{receive.Message}]:{relativeName} Spend:{SoftBasic.GetTimeSpanDescription(DateTime.Now - dateTimeStart2)}");
			}
		}
		else if (receiveFileNames.Content1 == 103)
		{
			DateTime dateTimeStart3 = DateTime.Now;
			List<string> mappings = new List<string>();
			string[] array = fileNames;
			foreach (string item in array)
			{
				string fullFileName2 = ReturnAbsoluteFileName(groupInfo, item);
				FileInfo info2 = new FileInfo(fullFileName2);
				GroupFileContainer fileManagment = GetGroupFromFilePath(info2.DirectoryName);
				string guid = fileManagment.DeleteFile(info2.Name);
				DeleteExsistingFile(info2.DirectoryName, guid);
				mappings.Add(guid);
				relativeName = GetRelativeFileName(groupInfo, item);
				base.LogNet?.WriteInfo(ToString(), $"{session} {StringResources.Language.FileDeleteSuccess}:{relativeName}");
			}
			await session.SendMqttCommandAsync(103, null, null);
			OnFileChangedEvent?.Invoke(session, new MqttFileOperateInfo
			{
				Groups = HslHelper.PathCombine(groupInfo),
				FileNames = fileNames,
				MappingNames = mappings.ToArray(),
				Operate = "Delete",
				TimeCost = DateTime.Now - dateTimeStart3
			});
		}
		else if (receiveFileNames.Content1 == 110)
		{
			DateTime dateTimeStart4 = DateTime.Now;
			string fullFileName3 = ReturnAbsoluteFileName(groupInfo, "123.txt");
			FileInfo info3 = new FileInfo(fullFileName3);
			GroupFileContainer fileManagment2 = GetGroupFromFilePath(info3.DirectoryName);
			List<string> guid_names = fileManagment2.ClearAllFiles(out var file_names);
			DeleteExsistingFile(info3.DirectoryName, guid_names);
			await session.SendMqttCommandAsync(110, null, null);
			OnFileChangedEvent?.Invoke(session, new MqttFileOperateInfo
			{
				Groups = HslHelper.PathCombine(groupInfo),
				FileNames = file_names.ToArray(),
				MappingNames = guid_names.ToArray(),
				Operate = "ClearFolder",
				TimeCost = DateTime.Now - dateTimeStart4
			});
			base.LogNet?.WriteInfo(ToString(), session.ToString() + "ClearFolder : " + relativeName);
		}
		else if (receiveFileNames.Content1 == 104)
		{
			DateTime dateTimeStart5 = DateTime.Now;
			string fullFileName4 = ReturnAbsoluteFileName(groupInfo, "123.txt");
			FileInfo info4 = new FileInfo(fullFileName4);
			GroupFileContainer fileManagment3 = GetGroupFromFilePath(info4.DirectoryName);
			List<string> guid_names2 = fileManagment3.GetAllFiles(out var file_names2);
			DeleteGroupFileContainer(info4.DirectoryName);
			await session.SendMqttCommandAsync(104, null, null);
			OnFileChangedEvent?.Invoke(session, new MqttFileOperateInfo
			{
				Groups = HslHelper.PathCombine(groupInfo),
				FileNames = file_names2.ToArray(),
				MappingNames = guid_names2.ToArray(),
				Operate = "DeleteFolder",
				TimeCost = DateTime.Now - dateTimeStart5
			});
			base.LogNet?.WriteInfo(ToString(), session.ToString() + " FolderDelete : " + relativeName);
		}
		else if (receiveFileNames.Content1 == 111)
		{
			DateTime dateTimeStart6 = DateTime.Now;
			string fullFileName5 = ReturnAbsoluteFileName(groupInfo, "123.txt");
			FileInfo info5 = new FileInfo(fullFileName5);
			OperateResult rename = RenameGroupFileContainer(newPath: Path.Combine(path2: HslHelper.PathCombine(fileNames[0].Split('/', '\\')), path1: filesDirectoryPath), filePath: info5.DirectoryName);
			if (!rename.IsSuccess)
			{
				base.LogNet?.WriteInfo(ToString(), session.ToString() + " RenameFolder : " + HslHelper.PathCombine(groupInfo) + " -> " + fileNames[0] + " failed: " + rename.Message);
				session.SendMqttCommand(0, null, Encoding.UTF8.GetBytes("File path already exist"));
			}
			else
			{
				await session.SendMqttCommandAsync(111, null, null);
				OnFileChangedEvent?.Invoke(session, new MqttFileOperateInfo
				{
					Groups = HslHelper.PathCombine(groupInfo),
					FileNames = null,
					Operate = "RenameFolder",
					TimeCost = DateTime.Now - dateTimeStart6
				});
				base.LogNet?.WriteInfo(ToString(), session.ToString() + " RenameFolder : " + HslHelper.PathCombine(groupInfo) + " -> " + fileNames[0]);
			}
		}
		else if (receiveFileNames.Content1 == 105)
		{
			GroupFileContainer fileManagment4 = GetGroupFromFilePath(ReturnAbsoluteFilePath(groupInfo));
			await session.SendMqttCommandAsync(104, null, Encoding.UTF8.GetBytes(fileManagment4.JsonArrayContent));
		}
		else if (receiveFileNames.Content1 == 108)
		{
			GroupFileContainer fileManagment5 = GetGroupFromFilePath(ReturnAbsoluteFilePath(groupInfo));
			await session.SendMqttCommandAsync(108, null, Encoding.UTF8.GetBytes(fileManagment5.GetGroupFileInfo(withLastFileInfo: true).ToJsonString((Formatting)1)));
		}
		else if (receiveFileNames.Content1 == 109)
		{
			bool withLastFileInfo = fileNames != null && fileNames.Length != 0 && fileNames[0] == "1";
			List<GroupFileInfo> folders = new List<GroupFileInfo>();
			string[] directories = GetDirectories(groupInfo);
			foreach (string m in directories)
			{
				DirectoryInfo directory = new DirectoryInfo(m);
				GroupFileContainer fileManagment6 = GetGroupFromFilePath(ReturnAbsoluteFilePath(new List<string>(groupInfo) { directory.Name }.ToArray()));
				GroupFileInfo groupFileInfo = fileManagment6.GetGroupFileInfo(withLastFileInfo);
				groupFileInfo.PathName = directory.Name;
				folders.Add(groupFileInfo);
			}
			await session.SendMqttCommandAsync(108, null, Encoding.UTF8.GetBytes(folders.ToJsonString((Formatting)1)));
		}
		else if (receiveFileNames.Content1 == 106)
		{
			List<string> folders2 = new List<string>();
			string[] directories2 = GetDirectories(groupInfo);
			foreach (string m2 in directories2)
			{
				DirectoryInfo directory2 = new DirectoryInfo(m2);
				folders2.Add(directory2.Name);
			}
			JArray jArray = JArray.FromObject((object)folders2.ToArray());
			await session.SendMqttCommandAsync(106, null, Encoding.UTF8.GetBytes(((object)jArray).ToString()));
		}
		else if (receiveFileNames.Content1 == 107)
		{
			string fileName3 = fileNames[0];
			string fullPath = ReturnAbsoluteFilePath(groupInfo);
			GroupFileContainer fileManagment7 = GetGroupFromFilePath(fullPath);
			bool isExists = fileManagment7.FileExists(fileName3);
			await session.SendMqttCommandAsync((byte)(isExists ? 1 : 0), null, Encoding.UTF8.GetBytes(StringResources.Language.FileNotExist));
		}
	}

	/// <summary>
	/// 从套接字接收文件并保存，更新文件列表
	/// </summary>
	/// <param name="session">当前的会话信息</param>
	/// <param name="info">保存的信息</param>
	/// <param name="guidName">映射的文件名称</param>
	/// <param name="reportProgress">当前的委托信息</param>
	/// <returns>是否成功的结果对象</returns>
	private async Task<OperateResult<FileBaseInfo>> ReceiveMqttFileAndUpdateGroupAsync(MqttSession session, FileInfo info, string guidName, Action<long, long> reportProgress)
	{
		string fileName = Path.Combine(info.DirectoryName, guidName);
		OperateResult<FileBaseInfo> receive = await MqttHelper.ReceiveMqttFileAsync(session.MqttPipe, fileName, reportProgress, session.IsAesCryptography ? session.AesCryptography : null);
		if (!receive.IsSuccess)
		{
			DeleteFileByName(fileName);
			return receive;
		}
		GroupFileContainer fileManagment = GetGroupFromFilePath(info.DirectoryName);
		DeleteExsistingFile(fileName: fileManagment.UpdateFileMappingName(info.Name, receive.Content.Size, guidName, session.UserName, receive.Content.Tag), path: info.DirectoryName);
		OperateResult sendBack = await session.SendMqttCommandAsync(100, null, Encoding.UTF8.GetBytes(StringResources.Language.SuccessText));
		if (!sendBack.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FileBaseInfo>(sendBack);
		}
		return OperateResult.CreateSuccessResult(receive.Content);
	}

	/// <summary>
	/// 返回相对路径的名称
	/// </summary>
	/// <param name="groups">文件的分类路径信息</param>
	/// <param name="fileName">文件名</param>
	/// <returns>是否成功的结果对象</returns>
	private string GetRelativeFileName(string[] groups, string fileName)
	{
		string path = "";
		for (int i = 0; i < groups.Length; i++)
		{
			if (!string.IsNullOrEmpty(groups[i]))
			{
				path = Path.Combine(path, groups[i]);
			}
		}
		return Path.Combine(path, fileName);
	}

	/// <summary>
	/// 返回服务器的绝对路径，包含根目录的信息  [Root Dir][A][B][C]... 信息
	/// </summary>
	/// <param name="groups">文件的路径分类信息</param>
	/// <returns>是否成功的结果对象</returns>
	private string ReturnAbsoluteFilePath(string[] groups)
	{
		return Path.Combine(filesDirectoryPath, Path.Combine(groups));
	}

	/// <summary>
	/// 返回服务器的绝对路径，包含根目录的信息  [Root Dir][A][B][C]...[FileName] 信息
	/// </summary>
	/// <param name="groups">路径分类信息</param>
	/// <param name="fileName">文件名</param>
	/// <returns>是否成功的结果对象</returns>
	protected string ReturnAbsoluteFileName(string[] groups, string fileName)
	{
		return Path.Combine(ReturnAbsoluteFilePath(groups), fileName);
	}

	/// <summary>
	/// 根据文件的显示名称转化为真实存储的名称，例如 123.txt 获取到在文件服务器里映射的文件名称，例如返回 b35a11ec533147ca80c7f7d1713f015b7909
	/// </summary>
	/// <param name="groups">文件的分类信息</param>
	/// <param name="fileName">文件显示名称</param>
	/// <returns>是否成功的结果对象</returns>
	private string TransformFactFileName(string[] groups, string fileName)
	{
		string filePath = ReturnAbsoluteFilePath(groups);
		GroupFileContainer groupFromFilePath = GetGroupFromFilePath(filePath);
		return groupFromFilePath.GetCurrentFileMappingName(fileName);
	}

	/// <summary>
	/// 获取当前目录的文件列表管理容器，如果没有会自动创建，通过该容器可以实现对当前目录的文件进行访问
	/// Get the file list management container of the current directory. If not, it will be created automatically.
	/// Through this container, you can access files in the current directory.
	/// </summary>
	/// <param name="filePath">路径信息</param>
	/// <param name="create">是否创建容器，当没有发现该路径的容器的情况</param>
	/// <returns>文件管理容器信息</returns>
	private GroupFileContainer GetGroupFromFilePath(string filePath, bool create = true)
	{
		GroupFileContainer groupFileContainer = null;
		filePath = filePath.ToUpper();
		group_marks_lock.Enter();
		if (m_dictionary_group_marks.ContainsKey(filePath))
		{
			groupFileContainer = m_dictionary_group_marks[filePath];
		}
		else if (create)
		{
			groupFileContainer = new GroupFileContainer(base.LogNet, filePath);
			m_dictionary_group_marks.Add(filePath, groupFileContainer);
		}
		group_marks_lock.Leave();
		return groupFileContainer;
	}

	/// <summary>
	/// 彻底删除当前的目录信息，整个目录及其所有的子文件夹，子文件信息
	/// Completely delete the current directory information, the entire directory and all its subfolders, subfile information
	/// </summary>
	/// <param name="filePath">文件路径信息</param>
	private void DeleteGroupFileContainer(string filePath)
	{
		filePath = filePath.ToUpper();
		group_marks_lock.Enter();
		if (m_dictionary_group_marks.ContainsKey(filePath))
		{
			GroupFileContainer groupFileContainer = m_dictionary_group_marks[filePath];
			groupFileContainer.DeleteFolder();
			m_dictionary_group_marks.Remove(filePath);
		}
		group_marks_lock.Leave();
	}

	/// <summary>
	/// 重命名当前的目录文件信息，如果修改后的名称还是存在，则返回错误信息
	/// </summary>
	/// <param name="filePath">旧的路径信息</param>
	/// <param name="newPath">新的路径信息</param>
	/// <returns>是否成功</returns>
	private OperateResult RenameGroupFileContainer(string filePath, string newPath)
	{
		filePath = filePath.ToUpper();
		string text = newPath.ToUpper();
		OperateResult result = OperateResult.CreateSuccessResult();
		group_marks_lock.Enter();
		if (m_dictionary_group_marks.ContainsKey(text) || Directory.Exists(text))
		{
			result = new OperateResult("修改的目录名称已经存在！");
		}
		else if (m_dictionary_group_marks.ContainsKey(filePath))
		{
			GroupFileContainer groupFileContainer = m_dictionary_group_marks[filePath];
			groupFileContainer.RenameFolder(newPath);
			m_dictionary_group_marks.Remove(filePath);
			m_dictionary_group_marks.Add(text, groupFileContainer);
		}
		group_marks_lock.Leave();
		return result;
	}

	/// <summary>
	/// 根据路径信息获取到文件列表管理容器，如果返回空，表示不存在。
	/// The file list management container is obtained according to the path information. If the return is empty, it means that it does not exist.
	/// </summary>
	/// <param name="groups">文件路径信息</param>
	/// <returns>文件列表管理容器</returns>
	public GroupFileContainer GetGroupFromFilePath(string[] groups)
	{
		return GetGroupFromFilePath(ReturnAbsoluteFilePath(groups), create: false);
	}

	/// <summary>
	/// 获取文件夹的所有文件夹列表
	/// </summary>
	/// <param name="groups">分类信息</param>
	/// <returns>文件夹列表</returns>
	private string[] GetDirectories(string[] groups)
	{
		if (string.IsNullOrEmpty(filesDirectoryPath))
		{
			return new string[0];
		}
		string path = ReturnAbsoluteFilePath(groups);
		if (!Directory.Exists(path))
		{
			return new string[0];
		}
		return Directory.GetDirectories(path);
	}

	/// <summary>
	/// 获取当前文件的读写锁，如果没有会自动创建，文件名应该是guid文件名，例如 b35a11ec533147ca80c7f7d1713f015b7909
	/// Acquire the read-write lock of the current file. If not, it will be created automatically.
	/// The file name should be the guid file name, for example, b35a11ec533147ca80c7f7d1713f015b7909
	/// </summary>
	/// <param name="fileName">完整的文件路径</param>
	/// <returns>返回携带文件信息的读写锁</returns>
	private FileMarkId GetFileMarksFromDictionaryWithFileName(string fileName)
	{
		FileMarkId fileMarkId;
		lock (dictHybirdLock)
		{
			if (dictionaryFilesMarks.ContainsKey(fileName))
			{
				fileMarkId = dictionaryFilesMarks[fileName];
			}
			else
			{
				fileMarkId = new FileMarkId(base.LogNet, fileName);
				dictionaryFilesMarks.Add(fileName, fileMarkId);
			}
		}
		return fileMarkId;
	}

	/// <summary>
	/// 检查文件夹是否存在，不存在就创建
	/// </summary>
	private void CheckFolderAndCreate()
	{
		if (!Directory.Exists(filesDirectoryPath))
		{
			Directory.CreateDirectory(filesDirectoryPath);
		}
	}

	/// <summary>
	/// 删除已经存在的文件信息，文件的名称需要是guid名称，例如 b35a11ec533147ca80c7f7d1713f015b7909
	/// </summary>
	/// <param name="path">文件的路径</param>
	/// <param name="fileName">文件的guid名称，例如 b35a11ec533147ca80c7f7d1713f015b7909</param>
	private void DeleteExsistingFile(string path, string fileName)
	{
		DeleteExsistingFile(path, new List<string> { fileName });
	}

	/// <summary>
	/// 删除已经存在的文件信息，文件的名称需要是guid名称，例如 b35a11ec533147ca80c7f7d1713f015b7909
	/// </summary>
	/// <param name="path">文件的路径</param>
	/// <param name="fileNames">文件的guid名称，例如 b35a11ec533147ca80c7f7d1713f015b7909</param>
	private void DeleteExsistingFile(string path, List<string> fileNames)
	{
		foreach (string fileName in fileNames)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				continue;
			}
			string fileUltimatePath = Path.Combine(path, fileName);
			FileMarkId fileMarksFromDictionaryWithFileName = GetFileMarksFromDictionaryWithFileName(fileName);
			fileMarksFromDictionaryWithFileName.AddOperation(delegate
			{
				if (!DeleteFileByName(fileUltimatePath))
				{
					base.LogNet?.WriteInfo(ToString(), StringResources.Language.FileDeleteFailed + fileUltimatePath);
				}
				else
				{
					base.LogNet?.WriteInfo(ToString(), StringResources.Language.FileDeleteSuccess + fileUltimatePath);
				}
			});
		}
	}

	private void AddMqttSession(MqttSession session)
	{
		lock (sessionsLock)
		{
			mqttSessions.Add(session);
		}
		base.LogNet?.WriteDebug(ToString(), $"{session} Online");
	}

	/// <summary>
	/// 让MQTT客户端正常下线，调用本方法即可自由控制会话客户端强制下线操作。
	/// </summary>
	/// <param name="session">当前的会话信息</param>
	/// <param name="reason">当前下线的原因，如果没有，代表正常下线</param>
	public void RemoveAndCloseSession(MqttSession session, string reason)
	{
		bool success = false;
		lock (sessionsLock)
		{
			success = mqttSessions.Remove(session);
		}
		if (!success)
		{
			return;
		}
		NetSupport.CloseSocket(session.MqttPipe.Socket);
		base.LogNet?.WriteDebug(ToString(), $"{session} Offline {reason}");
		if (session.Protocol == "MQTT")
		{
			OnClientDisConnected?.Invoke(session);
			if (!string.IsNullOrEmpty(reason) && !string.IsNullOrEmpty(session.WillTopic))
			{
				PublishTopicPayload(session.WillTopic, session.WillMessage);
			}
		}
	}

	/// <summary>
	/// 删除服务器里的指定主题的驻留消息。
	/// Delete the resident message of the specified topic in the server.
	/// </summary>
	/// <param name="topic">等待删除的主题关键字</param>
	public void DeleteRetainTopic(string topic)
	{
		lock (keysLock)
		{
			if (retainKeys.ContainsKey(topic))
			{
				retainKeys.Remove(topic);
			}
		}
	}

	/// <summary>
	/// 获取所有的驻留的消息的主题，如果消息发布的时候没有使用Retain属性，就无法通过本方法查到
	/// Get the subject of all resident messages. If the Retain attribute is not used when the message is published, it cannot be found by this method
	/// </summary>
	/// <returns>主题的数组</returns>
	public string[] GetAllRetainTopics()
	{
		string[] result = null;
		lock (keysLock)
		{
			result = retainKeys.Select((KeyValuePair<string, MqttClientApplicationMessage> m) => m.Key).ToArray();
		}
		return result;
	}

	/// <summary>
	/// 获取订阅了某个主题的所有的会话列表信息
	/// Get all the conversation list information subscribed to a topic
	/// </summary>
	/// <param name="topic">主题信息</param>
	/// <returns>会话列表</returns>
	public MqttSession[] GetMqttSessionsByTopic(string topic)
	{
		MqttSession[] result = null;
		lock (sessionsLock)
		{
			result = mqttSessions.Where((MqttSession m) => m.Protocol == "MQTT" && m.IsClientSubscribe(topic, topicWildcard)).ToArray();
		}
		return result;
	}

	/// <summary>
	/// 释放当前的对象
	/// </summary>
	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				timerHeart?.Dispose();
				group_marks_lock?.Dispose();
				ClientVerification = null;
				FileOperateVerification = null;
				OnClientApplicationMessageReceive = null;
				OnClientConnected = null;
				OnClientDisConnected = null;
				OnFileChangedEvent = null;
			}
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// 删除一个指定的文件，如果文件不存在，直接返回 True，如果文件存在则直接删除，删除成功返回 True，如果发生了异常，返回False
	/// Delete a specified file, if the file does not exist, return True directly, if the file exists, delete it directly,
	/// if the deletion is successful, return True, if an exception occurs, return  False
	/// </summary>
	/// <param name="fileName">完整的文件路径</param>
	/// <returns>是否删除成功</returns>
	protected bool DeleteFileByName(string fileName)
	{
		try
		{
			if (!File.Exists(fileName))
			{
				return true;
			}
			File.Delete(fileName);
			return true;
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), "delete file [" + fileName + "] failed: ", ex);
			return false;
		}
	}

	public override string ToString()
	{
		return $"MqttServer[{base.Port}]";
	}
}
