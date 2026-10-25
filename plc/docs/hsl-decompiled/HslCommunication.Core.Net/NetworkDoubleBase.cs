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
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Pipe;
using HslCommunication.MQTT;
using HslCommunication.Reflection;

namespace HslCommunication.Core.Net;

/// <summary>
/// 支持长连接，短连接两个模式的通用客户端基类
/// Universal client base class that supports long connections and short connections to two modes
/// </summary>
public class NetworkDoubleBase : NetworkBase, IDisposable
{
	/// <summary>
	/// 当前的网络的管道信息
	/// </summary>
	protected PipeSocket pipeSocket;

	private IByteTransform byteTransform;

	private string connectionId = string.Empty;

	private bool isUseSpecifiedSocket = false;

	private bool useServerActivePush = false;

	private AutoResetEvent autoResetEvent;

	private byte[] bufferQA = null;

	/// <summary>
	/// 是否是长连接的状态
	/// Whether it is a long connection state
	/// </summary>
	protected bool isPersistentConn = false;

	/// <summary>
	/// 设置日志记录报文是否二进制，如果为False，那就使用ASCII码
	/// Set whether the log message is binary, if it is False, then use ASCII code
	/// </summary>
	protected bool LogMsgFormatBinary = true;

	/// <summary>
	/// 是否使用账号登录，这个账户登录的功能是HSL组件创建的服务器特有的功能。
	/// Whether to log in using an account. The function of this account login is a server-specific function created by the  HSL  component.
	/// </summary>
	protected bool isUseAccountCertificate = false;

	private string userName = string.Empty;

	private string password = string.Empty;

	private bool disposedValue = false;

	private MqttClient mqttClient = null;

	private string writeTopic = string.Empty;

	private string readTopic = string.Empty;

	private byte[] sendbyteBefore = null;

	private string sendBefore = string.Empty;

	private Lazy<Ping> ping = new Lazy<Ping>(() => new Ping());

	/// <summary>
	/// 获取或设置当前的连接是否激活从服务器主动推送的功能
	/// </summary>
	protected bool UseServerActivePush
	{
		get
		{
			return useServerActivePush;
		}
		set
		{
			if (value)
			{
				if (autoResetEvent == null)
				{
					autoResetEvent = new AutoResetEvent(initialState: false);
				}
				isPersistentConn = true;
			}
			useServerActivePush = value;
		}
	}

	/// <summary>
	/// 当前的数据变换机制，当你需要从字节数据转换类型数据的时候需要。
	/// The current data transformation mechanism is required when you need to convert type data from byte data.
	/// </summary>
	/// <remarks>
	/// 在HSL里提供了三种数据变换机制，分别是 , ,
	/// ，各自的属性也可以自定调整，基本满足所有的情况使用。
	/// Three data transformation mechanisms are provided in HSL, namely , ,
	/// , and their respective  property can also be adjusted by itself, basically satisfying all situations.
	/// </remarks>
	public IByteTransform ByteTransform
	{
		get
		{
			return byteTransform;
		}
		set
		{
			byteTransform = value;
		}
	}

	[HslMqttApi(HttpMethod = "GET", Description = "Gets or sets the timeout for the connection, in milliseconds")]
	/// <summary>
	/// 获取或设置连接的超时时间，单位是毫秒
	/// Gets or sets the timeout for the connection, in milliseconds
	/// </summary>
	/// <remarks>
	/// 不适用于异形模式的连接。
	/// </remarks>
	public virtual int ConnectTimeOut
	{
		get
		{
			return pipeSocket.ConnectTimeOut;
		}
		set
		{
			if (value >= 0)
			{
				pipeSocket.ConnectTimeOut = value;
			}
		}
	}

	[HslMqttApi(HttpMethod = "GET", Description = "Gets or sets the time to receive server feedback, and if it is a negative number, does not receive feedback")]
	/// <summary>
	/// 获取或设置接收服务器反馈的时间，如果为负数，则不接收反馈
	/// Gets or sets the time to receive server feedback, and if it is a negative number, does not receive feedback
	/// </summary>
	/// <remarks>
	/// 超时的通常原因是服务器端没有配置好，导致访问失败，为了不卡死软件，所以有了这个超时的属性。
	/// </remarks>
	public int ReceiveTimeOut
	{
		get
		{
			return pipeSocket.ReceiveTimeOut;
		}
		set
		{
			pipeSocket.ReceiveTimeOut = value;
		}
	}

	[HslMqttApi(HttpMethod = "GET", Description = "Get or set the IP address of the remote server. If it is a local test, then it needs to be set to 127.0.0.1")]
	/// <summary>
	/// 获取或是设置远程服务器的IP地址，如果是本机测试，那么需要设置为127.0.0.1
	/// Get or set the IP address of the remote server. If it is a local test, then it needs to be set to 127.0.0.1
	/// </summary>
	/// <remarks>
	/// 最好实在初始化的时候进行指定，当使用短连接的时候，支持动态更改，切换；当使用长连接后，无法动态更改
	/// 支持使用域名的网址方式，例如：www.hslcommunication.cn
	/// </remarks>
	public virtual string IpAddress
	{
		get
		{
			return pipeSocket.IpAddress;
		}
		set
		{
			pipeSocket.IpAddress = value;
		}
	}

	[HslMqttApi(HttpMethod = "GET", Description = "Gets or sets the port number of the server. The specific value depends on the configuration of the other party.")]
	/// <summary>
	/// 获取或设置服务器的端口号，具体的值需要取决于对方的配置
	/// Gets or sets the port number of the server. The specific value depends on the configuration of the other party.
	/// </summary>
	/// <remarks>
	/// 最好实在初始化的时候进行指定，当使用短连接的时候，支持动态更改，切换；当使用长连接后，无法动态更改
	/// </remarks>
	public virtual int Port
	{
		get
		{
			return pipeSocket.Port;
		}
		set
		{
			pipeSocket.Port = value;
		}
	}

	[HslMqttApi(HttpMethod = "GET", Description = "The unique ID number of the current connection. The default is a 20-digit guid code plus a random number.")]
	/// <summary>
	/// 当前连接的唯一ID号，默认为长度20的guid码加随机数组成，方便列表管理，也可以自己指定
	/// The unique ID number of the current connection. The default is a 20-digit guid code plus a random number.
	/// </summary>
	/// <remarks>
	/// 当前的唯一连接ID信息在DTU的模式下，将发挥关键作用，标记唯一的DTU ID
	/// </remarks>
	public string ConnectionId
	{
		get
		{
			return connectionId;
		}
		set
		{
			connectionId = value;
		}
	}

	[HslMqttApi(HttpMethod = "GET", Description = "Get or set the time required to rest before officially receiving the data from the other party. When it is set to 0, no rest is required.")]
	/// <summary>
	/// 获取或设置在正式接收对方返回数据前的时候，需要休息的时间，当设置为0的时候，不需要休息。
	/// Get or set the time required to rest before officially receiving the data from the other party. When it is set to 0, no rest is required.
	/// </summary>
	public int SleepTime
	{
		get
		{
			return pipeSocket.SleepTime;
		}
		set
		{
			pipeSocket.SleepTime = value;
		}
	}

	/// <summary>
	/// 获取或设置绑定的本地的IP地址和端口号信息，如果端口设置为0，代表任何可用的端口
	/// Get or set the bound local IP address and port number information, if the port is set to 0, it means any available port
	/// </summary>
	/// <remarks>
	/// 默认为NULL, 也即是不绑定任何本地的IP及端口号信息，使用系统自动分配的方式。
	/// The default is NULL, which means that no local IP and port number information are bound, and the system automatically assigns it.
	/// </remarks>
	public IPEndPoint LocalBinding
	{
		get
		{
			return pipeSocket.LocalBinding;
		}
		set
		{
			pipeSocket.LocalBinding = value;
		}
	}

	/// <summary>
	/// 当前的异形连接对象，如果设置了异形连接的话，仅用于异形模式的情况使用
	/// The current alien connection object, if alien connection is set, is only used in the case of alien mode
	/// </summary>
	/// <remarks>
	/// 具体的使用方法请参照Demo项目中的异形modbus实现。
	/// </remarks>
	public AlienSession AlienSession { get; set; }

	/// <summary>
	/// 获取或设置客户端的Socket的心跳时间信息，这个是Socket底层自动实现的心跳包，不基于协议层实现。默认小于0，不开启心跳检测，如果需要开启，设置 60_000 比较合适，单位毫秒
	/// Get or set the heartbeat time information of the Socket of the client. This is the heartbeat packet automatically implemented by the bottom layer of the Socket, not based on the protocol layer.
	/// The default value is less than 0, and heartbeat detection is not enabled. If you need to enable it, it is more appropriate to set 60_000, in milliseconds.
	/// </summary>
	/// <remarks>
	/// 经测试，在linux上，基于.net core3.1的程序运行时，设置了这个值是无效的。
	/// </remarks>
	public int SocketKeepAliveTime { get; set; } = -1;

	/// <summary>
	/// 获取或设置在发送通信报文前追加发送的字节信息，HEX格式，通常用于lora组网时，需要携带 00 00 00 02 四个字节的站地址功能。
	/// Obtain or set the byte information sent before sending communication packets, HEX format, usually used for LORA networking, you need to carry 00 00 00 02 four-byte station address function.
	/// </summary>
	public string SendBeforeHex
	{
		get
		{
			return sendBefore;
		}
		set
		{
			sendBefore = value;
			if (string.IsNullOrEmpty(value))
			{
				sendbyteBefore = null;
			}
			else
			{
				sendbyteBefore = value.ToHexBytes();
			}
		}
	}

	/// <summary>
	/// 默认的无参构造函数
	/// Default no-parameter constructor
	/// </summary>
	public NetworkDoubleBase()
	{
		pipeSocket = new PipeSocket();
		connectionId = SoftBasic.GetUniqueStringByGuidAndRandom();
	}

	/// <summary>
	/// 获取一个新的消息对象的方法，需要在继承类里面进行重写
	/// The method to get a new message object needs to be overridden in the inheritance class
	/// </summary>
	/// <returns>消息类对象</returns>
	protected virtual INetMessage GetNewNetMessage()
	{
		return null;
	}

	/// <summary>
	/// 设置一个新的网络管道，一般来说不需要调用本方法，当多个网口设备共用一个网络连接时才需要使用本方法进行设置共享的管道。
	/// To set up a new network channel, generally speaking, you do not need to call this method. This method is only needed to set up a shared channel when multiple network port devices share a network connection.
	/// </summary>
	/// <param name="pipeSocket">共享的网络通道</param>
	/// <remarks>
	/// 如果需要设置共享的网络管道的话，需要是设备类对象实例化之后立即进行设置。
	/// If you need to set up a shared network pipe, you need to set it immediately after the device class object is instantiated.
	/// </remarks>
	public void SetPipeSocket(PipeSocket pipeSocket)
	{
		if (pipeSocket != null)
		{
			this.pipeSocket = pipeSocket;
			SetPersistentConnection();
		}
	}

	/// <summary>
	/// 获取当前用于通信的管道信息
	/// Get the current pipe information used for communication
	/// </summary>
	/// <returns>管道对象</returns>
	public PipeSocket GetPipeSocket()
	{
		return pipeSocket;
	}

	/// <summary>
	/// 在读取数据之前可以调用本方法将客户端设置为长连接模式，相当于跳过了ConnectServer的结果验证，对异形客户端无效，当第一次进行通信时再进行创建连接请求。
	/// Before reading the data, you can call this method to set the client to the long connection mode, which is equivalent to skipping the result verification of ConnectServer,
	/// and it is invalid for the alien client. When the first communication is performed, the connection creation request is performed.
	/// </summary>
	public void SetPersistentConnection()
	{
		isPersistentConn = true;
	}

	/// <summary>
	/// 对当前设备的IP地址进行PING的操作，返回PING的结果，正常来说，返回
	/// PING the IP address of the current device and return the PING result. Normally, it returns
	/// </summary>
	/// <returns>返回PING的结果</returns>
	public IPStatus IpAddressPing()
	{
		return ping.Value.Send(IpAddress).Status;
	}

	/// <summary>
	/// 尝试连接远程的服务器，如果连接成功，就切换短连接模式到长连接模式，后面的每次请求都共享一个通道，使得通讯速度更快速
	/// Try to connect to a remote server. If the connection is successful, switch the short connection mode to the long connection mode.
	/// Each subsequent request will share a channel, making the communication speed faster.
	/// </summary>
	/// <returns>返回连接结果，如果失败的话（也即IsSuccess为False），包含失败信息</returns>
	public OperateResult ConnectServer()
	{
		isPersistentConn = true;
		pipeSocket.Socket?.Close();
		OperateResult<Socket> operateResult = CreateSocketAndInitialication();
		if (!operateResult.IsSuccess)
		{
			pipeSocket.IsSocketError = true;
			operateResult.Content = null;
		}
		else
		{
			pipeSocket.Socket = operateResult.Content;
			if (SocketKeepAliveTime > 0)
			{
				operateResult.Content.SetKeepAlive(SocketKeepAliveTime, SocketKeepAliveTime);
			}
			base.LogNet?.WriteDebug(ToString(), StringResources.Language.NetEngineStart);
		}
		return operateResult;
	}

	/// <summary>
	/// 使用指定的套接字创建异形客户端，在异形客户端的模式下，网络通道需要被动创建。
	/// Use the specified socket to create the alien client. In the alien client mode, the network channel needs to be created passively.
	/// </summary>
	/// <param name="session">异形客户端对象，查看类型创建的客户端</param>
	/// <returns>通常都为成功</returns>
	/// <remarks>
	/// 不能和之前的长连接和短连接混用，详细参考 Demo程序
	/// </remarks>
	public OperateResult ConnectServer(AlienSession session)
	{
		isPersistentConn = true;
		isUseSpecifiedSocket = true;
		if (session != null)
		{
			AlienSession?.Pipe?.CloseCommunication();
			if (string.IsNullOrEmpty(ConnectionId))
			{
				ConnectionId = session.DTU;
			}
			if (ConnectionId == session.DTU)
			{
				if (session.IsStatusOk)
				{
					OperateResult operateResult = InitializationOnConnect(session.Pipe.Socket);
					if (operateResult.IsSuccess)
					{
						pipeSocket.Socket = session.Pipe.Socket;
						pipeSocket.IsSocketError = !session.IsStatusOk;
						AlienSession = session;
					}
					else
					{
						pipeSocket.IsSocketError = true;
					}
					return operateResult;
				}
				return new OperateResult();
			}
			pipeSocket.IsSocketError = true;
			return new OperateResult();
		}
		pipeSocket.IsSocketError = true;
		return new OperateResult();
	}

	/// <summary>
	/// 使用一个MQTT中转服务器来连接设备对象，并进行相关的读取操作
	/// </summary>
	/// <param name="mqttClient">MQTT客户端信息</param>
	/// <param name="readTopic">获取数据的主题</param>
	/// <param name="writeTopic">写入数据的主题信息</param>
	/// <returns>是否成功</returns>
	public OperateResult ConnectServer(MqttClient mqttClient, string readTopic, string writeTopic)
	{
		isPersistentConn = true;
		this.writeTopic = writeTopic;
		this.readTopic = readTopic;
		pipeSocket.IsSocketError = true;
		if (mqttClient != null)
		{
			SubscribeTopic subscribeTopic = mqttClient.GetSubscribeTopic(readTopic);
			if (subscribeTopic == null)
			{
				mqttClient.OnClientConnected += MqttClient_OnClientConnected;
				mqttClient.SubscribeMessage(readTopic);
				subscribeTopic = mqttClient.GetSubscribeTopic(readTopic);
			}
			subscribeTopic.OnMqttMessageReceived += SubscribeTopic_OnMqttMessageReceived;
		}
		if (autoResetEvent == null)
		{
			autoResetEvent = new AutoResetEvent(initialState: false);
		}
		this.mqttClient = mqttClient;
		return OperateResult.CreateSuccessResult();
	}

	private void MqttClient_OnClientConnected(MqttClient client)
	{
		client.SubscribeMessage(readTopic);
	}

	private void SubscribeTopic_OnMqttMessageReceived(MqttClient client, MqttApplicationMessage message)
	{
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.Receive + " : " + (LogMsgFormatBinary ? message.Payload.ToHexString(' ') : SoftBasic.GetAsciiStringRender(message.Payload)));
		bufferQA = message.Payload;
		autoResetEvent.Set();
	}

	/// <summary>
	/// 手动断开与远程服务器的连接，如果当前是长连接模式，那么就会切换到短连接模式
	/// Manually disconnect from the remote server, if it is currently in long connection mode, it will switch to short connection mode
	/// </summary>
	/// <returns>关闭连接，不需要查看IsSuccess属性查看</returns>
	public OperateResult ConnectClose()
	{
		OperateResult operateResult = new OperateResult();
		isPersistentConn = false;
		try
		{
			operateResult = ((pipeSocket.Socket != null) ? ExtraOnDisconnect(pipeSocket.Socket) : OperateResult.CreateSuccessResult());
			if (mqttClient == null)
			{
				pipeSocket.Socket?.Close();
				pipeSocket.Socket = null;
			}
			else
			{
				SubscribeTopic subscribeTopic = mqttClient.GetSubscribeTopic(readTopic);
				if (subscribeTopic != null)
				{
					subscribeTopic.OnMqttMessageReceived -= SubscribeTopic_OnMqttMessageReceived;
				}
				mqttClient.OnClientConnected -= MqttClient_OnClientConnected;
				mqttClient.UnSubscribeMessage(readTopic);
				mqttClient = null;
			}
		}
		catch
		{
			throw;
		}
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.Close);
		return operateResult;
	}

	private async void ServerSocketActivePushAsync(IAsyncResult ar)
	{
		object asyncState = ar.AsyncState;
		if (!(asyncState is Socket socket))
		{
			return;
		}
		OperateResult<int> endResult = socket.EndReceiveResult(ar);
		if (!endResult.IsSuccess)
		{
			pipeSocket.IsSocketError = true;
			return;
		}
		OperateResult<byte[]> receive = await base.ReceiveByMessageAsync(socket, ReceiveTimeOut, GetNewNetMessage()).ConfigureAwait(continueOnCapturedContext: false);
		if (!receive.IsSuccess)
		{
			pipeSocket.IsSocketError = true;
			return;
		}
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.Receive + " : " + (LogMsgFormatBinary ? receive.Content.ToHexString(' ') : SoftBasic.GetAsciiStringRender(receive.Content)));
		OperateResult receiveAgain = socket.BeginReceiveResult(ServerSocketActivePushAsync);
		if (!receiveAgain.IsSuccess)
		{
			pipeSocket.IsSocketError = true;
		}
		if (DecideWhetherQAMessage(socket, receive))
		{
			bufferQA = receive.Content;
			autoResetEvent.Set();
		}
	}

	/// <summary>
	/// 决定当前的消息是否是应答机制的消息内容，需要在客户端进行重写实现，如果是应答机制，返回 True, 否则返回 False
	/// To determine whether the current message is the message content of the response mechanism,
	/// it needs to be rewritten on the client side. If it is the response mechanism, return True, otherwise return False
	/// </summary>
	/// <param name="socket">通信使用的网络套接字</param>
	/// <param name="receive">服务器返回的内容</param>
	/// <returns>是否应答机制的数据报文</returns>
	protected virtual bool DecideWhetherQAMessage(Socket socket, OperateResult<byte[]> receive)
	{
		return true;
	}

	/// <summary>
	/// 根据实际的协议选择是否重写本方法，有些协议在创建连接之后，需要进行一些初始化的信号握手，才能最终建立网络通道。
	/// Whether to rewrite this method is based on the actual protocol. Some protocols require some initial signal handshake to establish a network channel after the connection is created.
	/// </summary>
	/// <param name="socket">网络套接字</param>
	/// <returns>是否初始化成功，依据具体的协议进行重写</returns>
	protected virtual OperateResult InitializationOnConnect(Socket socket)
	{
		if (useServerActivePush)
		{
			OperateResult operateResult = socket.BeginReceiveResult(ServerSocketActivePushAsync);
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 根据实际的协议选择是否重写本方法，有些协议在断开连接之前，需要发送一些报文来关闭当前的网络通道
	/// Select whether to rewrite this method according to the actual protocol. Some protocols need to send some packets to close the current network channel before disconnecting.
	/// </summary>
	/// <param name="socket">网络套接字</param>
	/// <returns>当断开连接时额外的操作结果</returns>
	protected virtual OperateResult ExtraOnDisconnect(Socket socket)
	{
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 和服务器交互完成的时候调用的方法，可以根据读写结果进行一些额外的操作，具体的操作需要根据实际的需求来重写实现
	/// The method called when the interaction with the server is completed can perform some additional operations based on the read and write results.
	/// The specific operations need to be rewritten according to actual needs.
	/// </summary>
	/// <param name="read">读取结果</param>
	protected virtual void ExtraAfterReadFromCoreServer(OperateResult read)
	{
	}

	/// <summary>
	/// 设置当前的登录的账户名和密码信息，并启用账户验证的功能，账户名为空时设置不生效
	/// Set the current login account name and password information, and enable the account verification function. The account name setting will not take effect when it is empty
	/// </summary>
	/// <param name="userName">账户名</param>
	/// <param name="password">密码</param>
	/// <remarks>
	/// 当对方的服务器是使用HslCommunication创建的虚拟服务器时，例如modbus服务器，西门子s7服务器等等，就支持进行账户名验证操作，此时的客户端连接之前，就需要调用本方法设置账户密码信息。
	/// When the other party's server is a virtual server created by HslCommunication, such as modbus server, Siemens s7 server, etc.,
	/// it supports account name verification operation. At this time, before the client connects, you need to call this method to set account password information.
	/// </remarks>
	public void SetLoginAccount(string userName, string password)
	{
		if (!string.IsNullOrEmpty(userName.Trim()))
		{
			isUseAccountCertificate = true;
			this.userName = userName;
			this.password = password;
		}
		else
		{
			isUseAccountCertificate = false;
		}
	}

	/// <summary>
	/// 认证账号，根据已经设置的用户名和密码，进行发送服务器进行账号认证。
	/// Authentication account, according to the user name and password that have been set, sending server for account authentication.
	/// </summary>
	/// <param name="socket">套接字</param>
	/// <returns>认证结果</returns>
	protected OperateResult AccountCertificate(Socket socket)
	{
		OperateResult operateResult = SendAccountAndCheckReceive(socket, 1, userName, password);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<int, string[]> operateResult2 = ReceiveStringArrayContentFromSocket(socket);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		if (operateResult2.Content1 == 0)
		{
			return new OperateResult(operateResult2.Content2[0]);
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 认证账号，根据已经设置的用户名和密码，进行发送服务器进行账号认证。
	/// Authentication account, according to the user name and password that have been set, sending server for account authentication.
	/// </summary>
	/// <param name="socket">套接字</param>
	/// <returns>认证结果</returns>
	protected async Task<OperateResult> AccountCertificateAsync(Socket socket)
	{
		OperateResult send = await SendAccountAndCheckReceiveAsync(socket, 1, userName, password).ConfigureAwait(continueOnCapturedContext: false);
		if (!send.IsSuccess)
		{
			return send;
		}
		OperateResult<int, string[]> read = await ReceiveStringArrayContentFromSocketAsync(socket).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		if (read.Content1 == 0)
		{
			return new OperateResult(read.Content2[0]);
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 根据实际的协议选择是否重写本方法，有些协议在创建连接之后，需要进行一些初始化的信号握手，才能最终建立网络通道。
	/// Whether to rewrite this method is based on the actual protocol. Some protocols require some initial signal handshake to establish a network channel after the connection is created.
	/// </summary>
	/// <param name="socket">网络套接字</param>
	/// <returns>是否初始化成功，依据具体的协议进行重写</returns>
	protected virtual async Task<OperateResult> InitializationOnConnectAsync(Socket socket)
	{
		if (useServerActivePush)
		{
			OperateResult receive = socket.BeginReceiveResult(ServerSocketActivePushAsync);
			if (!receive.IsSuccess)
			{
				return receive;
			}
		}
		return await Task.FromResult(OperateResult.CreateSuccessResult()).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 根据实际的协议选择是否重写本方法，有些协议在断开连接之前，需要发送一些报文来关闭当前的网络通道
	/// Select whether to rewrite this method according to the actual protocol. Some protocols need to send some packets to close the current network channel before disconnecting.
	/// </summary>
	/// <param name="socket">网络套接字</param>
	/// <returns>当断开连接时额外的操作结果</returns>
	protected virtual async Task<OperateResult> ExtraOnDisconnectAsync(Socket socket)
	{
		return await Task.FromResult(OperateResult.CreateSuccessResult());
	}

	/// <summary>
	/// 尝试连接服务器，如果成功，并执行的初始化方法，并返回最终的结果。
	/// Attempt to connect to the server, if successful, and execute the initialization method of , and return the final result.
	/// </summary>
	/// <returns>带有socket的结果对象</returns>
	private async Task<OperateResult<Socket>> CreateSocketAndInitialicationAsync()
	{
		OperateResult<Socket> operateResult = ((mqttClient != null) ? OperateResult.CreateSuccessResult(pipeSocket.Socket) : (await CreateSocketAndConnectAsync(pipeSocket.GetConnectIPEndPoint(), ConnectTimeOut, LocalBinding).ConfigureAwait(continueOnCapturedContext: false)));
		OperateResult<Socket> result = operateResult;
		if (result.IsSuccess)
		{
			OperateResult initi = await InitializationOnConnectAsync(result.Content).ConfigureAwait(continueOnCapturedContext: false);
			if (!initi.IsSuccess)
			{
				result.Content?.Close();
				result.IsSuccess = initi.IsSuccess;
				result.CopyErrorFromOther(initi);
			}
		}
		return result;
	}

	/// <summary>
	/// 获取本次操作的可用的网络通道，如果是短连接，就重新生成一个新的网络通道，如果是长连接，就复用当前的网络通道。
	/// Obtain the available network channels for this operation. If it is a short connection, a new network channel is regenerated.
	/// If it is a long connection, the current network channel is reused.
	/// </summary>
	/// <returns>是否成功，如果成功，使用这个套接字</returns>
	protected async Task<OperateResult<Socket>> GetAvailableSocketAsync()
	{
		if (isPersistentConn)
		{
			if (isUseSpecifiedSocket)
			{
				if (pipeSocket.IsSocketError)
				{
					return new OperateResult<Socket>(StringResources.Language.ConnectionIsNotAvailable);
				}
				return OperateResult.CreateSuccessResult(pipeSocket.Socket);
			}
			if ((mqttClient == null) ? pipeSocket.IsConnectitonError() : pipeSocket.IsSocketError)
			{
				OperateResult connect = await ConnectServerAsync().ConfigureAwait(continueOnCapturedContext: false);
				if (!connect.IsSuccess)
				{
					pipeSocket.IsSocketError = true;
					return OperateResult.CreateFailedResult<Socket>(connect);
				}
				pipeSocket.IsSocketError = false;
				return OperateResult.CreateSuccessResult(pipeSocket.Socket);
			}
			return OperateResult.CreateSuccessResult(pipeSocket.Socket);
		}
		return await CreateSocketAndInitialicationAsync().ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 尝试连接远程的服务器，如果连接成功，就切换短连接模式到长连接模式，后面的每次请求都共享一个通道，使得通讯速度更快速
	/// Try to connect to a remote server. If the connection is successful, switch the short connection mode to the long connection mode.
	/// Each subsequent request will share a channel, making the communication speed faster.
	/// </summary>
	/// <returns>返回连接结果，如果失败的话（也即IsSuccess为False），包含失败信息</returns>
	public async Task<OperateResult> ConnectServerAsync()
	{
		isPersistentConn = true;
		pipeSocket.Socket?.Close();
		OperateResult<Socket> rSocket = await CreateSocketAndInitialicationAsync().ConfigureAwait(continueOnCapturedContext: false);
		if (!rSocket.IsSuccess)
		{
			pipeSocket.IsSocketError = true;
			rSocket.Content = null;
			return rSocket;
		}
		pipeSocket.Socket = rSocket.Content;
		if (SocketKeepAliveTime > 0)
		{
			rSocket.Content.SetKeepAlive(SocketKeepAliveTime, SocketKeepAliveTime);
		}
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.ConnectedSuccess);
		return rSocket;
	}

	/// <summary>
	/// 手动断开与远程服务器的连接，如果当前是长连接模式，那么就会切换到短连接模式
	/// Manually disconnect from the remote server, if it is currently in long connection mode, it will switch to short connection mode
	/// </summary>
	/// <returns>关闭连接，不需要查看IsSuccess属性查看</returns>
	public async Task<OperateResult> ConnectCloseAsync()
	{
		new OperateResult();
		isPersistentConn = false;
		OperateResult result;
		try
		{
			result = await ExtraOnDisconnectAsync(pipeSocket.Socket).ConfigureAwait(continueOnCapturedContext: false);
			if (mqttClient == null)
			{
				pipeSocket.Socket?.Close();
				pipeSocket.Socket = null;
			}
			else
			{
				SubscribeTopic subscribeTopic = mqttClient.GetSubscribeTopic(readTopic);
				if (subscribeTopic != null)
				{
					subscribeTopic.OnMqttMessageReceived -= SubscribeTopic_OnMqttMessageReceived;
				}
				mqttClient.OnClientConnected -= MqttClient_OnClientConnected;
				mqttClient.UnSubscribeMessage(readTopic);
				mqttClient = null;
			}
		}
		catch
		{
			throw;
		}
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.Close);
		return result;
	}

	/// <summary>
	/// 将数据报文发送指定的网络通道上，根据当前指定的类型，返回一条完整的数据指令
	/// Sends a data message to the specified network channel, and returns a complete data command according to the currently specified  type
	/// </summary>
	/// <param name="socket">指定的套接字</param>
	/// <param name="send">发送的完整的报文信息</param>
	/// <param name="hasResponseData">是否有等待的数据返回，默认为 true</param>
	/// <param name="usePackAndUnpack">是否需要对命令重新打包，在重写方法后才会有影响</param>
	/// <returns>接收的完整的报文信息</returns>
	/// <remarks>
	/// 无锁的基于套接字直接进行叠加协议的操作。
	/// </remarks>
	public virtual async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(Socket socket, byte[] send, bool hasResponseData = true, bool usePackAndUnpack = true)
	{
		byte[] sendValue = (usePackAndUnpack ? PackCommandWithHeader(send) : send);
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.Send + " : " + (LogMsgFormatBinary ? sendValue.ToHexString(' ') : SoftBasic.GetAsciiStringRender(sendValue)));
		INetMessage netMessage = GetNewNetMessage();
		if (netMessage != null)
		{
			netMessage.SendBytes = sendValue;
		}
		if (sendbyteBefore != null)
		{
			await SendAsync(socket, sendbyteBefore).ConfigureAwait(continueOnCapturedContext: false);
		}
		OperateResult operateResult = ((mqttClient != null) ? mqttClient.PublishMessage(new MqttApplicationMessage
		{
			Topic = writeTopic,
			Payload = sendValue
		}) : (await SendAsync(socket, sendValue).ConfigureAwait(continueOnCapturedContext: false)));
		OperateResult sendResult = operateResult;
		if (!sendResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(sendResult);
		}
		if (ReceiveTimeOut < 0)
		{
			return OperateResult.CreateSuccessResult(new byte[0]);
		}
		if (!hasResponseData)
		{
			return OperateResult.CreateSuccessResult(new byte[0]);
		}
		if (SleepTime > 0)
		{
			HslHelper.ThreadSleep(SleepTime);
		}
		OperateResult<byte[]> resultReceive;
		if (useServerActivePush)
		{
			if (!(await Task.Run(() => autoResetEvent.WaitOne(ReceiveTimeOut)).ConfigureAwait(continueOnCapturedContext: false)))
			{
				NetSupport.CloseSocket(socket);
				pipeSocket.IsSocketError = true;
				return new OperateResult<byte[]>(-10000, StringResources.Language.ReceiveDataTimeout + ReceiveTimeOut);
			}
			netMessage.HeadBytes = bufferQA;
			resultReceive = OperateResult.CreateSuccessResult(bufferQA);
		}
		else if (mqttClient != null)
		{
			MemoryStream ms = new MemoryStream();
			while (true)
			{
				if (await Task.Run(() => autoResetEvent.WaitOne(ReceiveTimeOut)).ConfigureAwait(continueOnCapturedContext: false))
				{
					byte[] array = bufferQA;
					if (array != null && array.Length != 0)
					{
						ms.Write(bufferQA);
					}
					if (netMessage == null)
					{
						break;
					}
					if (ms.Length < netMessage.ProtocolHeadBytesLength)
					{
						continue;
					}
					if (netMessage.HeadBytes == null)
					{
						byte[] head = ms.ToArray();
						int start = netMessage.PependedUselesByteLength(head);
						if (start > 0)
						{
							ms = new MemoryStream();
							ms.Write(head.RemoveBegin(start));
							if (ms.Length < netMessage.ProtocolHeadBytesLength)
							{
								continue;
							}
						}
						netMessage.HeadBytes = ms.ToArray().SelectBegin(netMessage.ProtocolHeadBytesLength);
					}
					int contentLength = netMessage.GetContentLengthByHeadBytes();
					if (ms.Length < netMessage.ProtocolHeadBytesLength + contentLength)
					{
						continue;
					}
					break;
				}
				return new OperateResult<byte[]>(-10000, StringResources.Language.ReceiveDataTimeout + ReceiveTimeOut);
			}
			resultReceive = OperateResult.CreateSuccessResult(ms.ToArray());
		}
		else if (netMessage == null)
		{
			DateTime startTime = DateTime.Now;
			MemoryStream ms2 = new MemoryStream();
			while (true)
			{
				OperateResult<byte[]> read = await ReceiveByMessageAsync(socket, ReceiveTimeOut, netMessage).ConfigureAwait(continueOnCapturedContext: false);
				if (!read.IsSuccess)
				{
					return read;
				}
				ms2.Write(read.Content);
				if (CheckReceiveDataComplete(sendValue, ms2))
				{
					resultReceive = OperateResult.CreateSuccessResult(ms2.ToArray());
					break;
				}
				if (ReceiveTimeOut > 0 && (DateTime.Now - startTime).TotalMilliseconds > (double)ReceiveTimeOut)
				{
					resultReceive = new OperateResult<byte[]>(StringResources.Language.ReceiveDataTimeout + ReceiveTimeOut);
					break;
				}
			}
		}
		else
		{
			resultReceive = await ReceiveByMessageAsync(socket, ReceiveTimeOut, netMessage).ConfigureAwait(continueOnCapturedContext: false);
		}
		if (!resultReceive.IsSuccess)
		{
			return resultReceive;
		}
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.Receive + " : " + (LogMsgFormatBinary ? resultReceive.Content.ToHexString(' ') : SoftBasic.GetAsciiStringRender(resultReceive.Content)));
		if (netMessage != null && !netMessage.CheckHeadBytesLegal(base.Token.ToByteArray()))
		{
			if (mqttClient == null)
			{
				NetSupport.CloseSocket(socket);
			}
			return new OperateResult<byte[]>(StringResources.Language.CommandHeadCodeCheckFailed + Environment.NewLine + StringResources.Language.Send + ": " + SoftBasic.ByteToHexString(sendValue, ' ') + Environment.NewLine + StringResources.Language.Receive + ": " + SoftBasic.ByteToHexString(resultReceive.Content, ' '));
		}
		return usePackAndUnpack ? UnpackResponseContent(sendValue, resultReceive.Content) : resultReceive;
	}

	/// <summary>
	/// 将数据发送到当前的网络通道中，并从网络通道中接收一个指定的完整的报文，网络通道将根据方法自动获取，本方法是线程安全的。
	/// Send data to the current network channel and receive a complete message specified by  from the network channel.
	/// The network channel will be automatically obtained according to the  method This method is thread-safe.
	/// </summary>
	/// <param name="send">发送的完整的报文信息</param>
	/// <param name="hasResponseData">是否有等待的数据返回，默认为 true</param>
	/// <param name="usePackAndUnpack">是否需要对命令重新打包，在重写方法后才会有影响</param>
	/// <returns>接收的完整的报文信息</returns>
	/// <remarks>
	/// 本方法用于实现本组件还未实现的一些报文功能，例如有些modbus服务器会有一些特殊的功能码支持，需要收发特殊的报文，详细请看示例
	/// </remarks>
	public virtual async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(byte[] send)
	{
		return await ReadFromCoreServerAsync(send, hasResponseData: true).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 将多个数据报文按顺序发到设备，并从设备接收返回的数据内容，然后拼接成一个Byte[]信息，需要重写方法才能返回正确的结果。
	/// Send multiple data packets to the device in sequence, and receive the returned data content from the device, and then splicing them into a Byte[] message,
	/// you need to rewrite  method to return the correct result.
	/// </summary>
	/// <param name="send">发送的报文列表信息</param>
	/// <returns>是否接收成功</returns>
	public async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(IEnumerable<byte[]> send)
	{
		return await NetSupport.ReadFromCoreServerAsync(send, ReadFromCoreServerAsync).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 将数据发送到当前的网络通道中，并从网络通道中接收一个指定的完整的报文，网络通道将根据方法自动获取，本方法是线程安全的。
	/// Send data to the current network channel and receive a complete message specified by  from the network channel.
	/// The network channel will be automatically obtained according to the  method This method is thread-safe.
	/// </summary>
	/// <param name="send">发送的完整的报文信息</param>
	/// <param name="hasResponseData">是否有等待的数据返回，默认为 true</param>
	/// <param name="usePackAndUnpack">是否需要对命令重新打包，在重写方法后才会有影响</param>
	/// <returns>接收的完整的报文信息</returns>
	/// <remarks>
	/// 本方法用于实现本组件还未实现的一些报文功能，例如有些modbus服务器会有一些特殊的功能码支持，需要收发特殊的报文，详细请看示例
	/// </remarks>
	public async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(byte[] send, bool hasResponseData, bool usePackAndUnpack = true)
	{
		if (pipeSocket.LockingTick > HslHelper.LockLimit)
		{
			return new OperateResult<byte[]>(StringResources.Language.TooManyLock);
		}
		OperateResult<byte[]> result = new OperateResult<byte[]>();
		if (HslHelper.UseAsyncLock)
		{
			await Task.Run(delegate
			{
				pipeSocket.PipeLockEnter();
			}).ConfigureAwait(continueOnCapturedContext: false);
		}
		else
		{
			pipeSocket.PipeLockEnter();
		}
		OperateResult<Socket> resultSocket;
		try
		{
			resultSocket = await GetAvailableSocketAsync().ConfigureAwait(continueOnCapturedContext: false);
			if (!resultSocket.IsSuccess)
			{
				pipeSocket.IsSocketError = true;
				AlienSession?.Offline();
				pipeSocket.PipeLockLeave();
				result.CopyErrorFromOther(resultSocket);
				return result;
			}
			OperateResult<byte[]> read = await ReadFromCoreServerAsync(resultSocket.Content, send, hasResponseData, usePackAndUnpack).ConfigureAwait(continueOnCapturedContext: false);
			if (read.IsSuccess)
			{
				pipeSocket.IsSocketError = false;
				result.IsSuccess = read.IsSuccess;
				result.Content = read.Content;
				result.Message = StringResources.Language.SuccessText;
			}
			else
			{
				if (read.ErrorCode != int.MinValue)
				{
					pipeSocket.IsSocketError = true;
					AlienSession?.Offline();
				}
				else
				{
					read.ErrorCode = 10000;
				}
				result.CopyErrorFromOther(read);
			}
			ExtraAfterReadFromCoreServer(read);
			pipeSocket.PipeLockLeave();
		}
		catch
		{
			pipeSocket.PipeLockLeave();
			throw;
		}
		if (!isPersistentConn)
		{
			resultSocket?.Content?.Close();
		}
		return result;
	}

	/// <summary>
	/// 检查当前从网口接收的数据是否是完整的，如果是完整的，则需要返回 True，表示数据接收立即完成，默认返回 True
	/// Check whether the data currently received from the network port is complete, and if it is complete,
	/// you need to return True, indicating that the data reception is completed immediately, and the default value is True
	/// </summary>
	/// <param name="send">当前发送的数据信息</param>
	/// <param name="ms">目前已经接收到数据流</param>
	/// <returns>如果数据接收完成，则返回True, 否则返回False</returns>
	/// <remarks>
	/// 在默认情况下，网口在接收数据之后，直接认为本次的数据接收已经完成，如果碰到有结束标记的协议，则可以重写本方法，然后加入额外的验证信息，直到全部数据接收完成。
	/// By default, after receiving data, the network port directly believes that the data reception has been completed,
	/// if it encounters a protocol with an end tag, you can override this method, and then add additional verification information until all data is received.
	/// </remarks>
	protected virtual bool CheckReceiveDataComplete(byte[] send, MemoryStream ms)
	{
		return true;
	}

	/// <summary>
	/// 对当前的命令进行打包处理，通常是携带命令头内容，标记当前的命令的长度信息，需要进行重写，否则默认不打包
	/// The current command is packaged, usually carrying the content of the command header, marking the length of the current command,
	/// and it needs to be rewritten, otherwise it is not packaged by default
	/// </summary>
	/// <param name="command">发送的数据命令内容</param>
	/// <returns>打包之后的数据结果信息</returns>
	/// <remarks>
	/// 对发送的命令打包之后，直接发送给真实的对方设备了，例如在AB-PLC里面，就重写了打包方法，将当前的会话ID参数传递给PLC设备
	/// After packaging the sent command, it is directly sent to the real counterpart device. For example, in AB-PLC,
	/// the packaging method is rewritten and the current session ID parameter is passed to the PLC device.
	/// </remarks>
	public virtual byte[] PackCommandWithHeader(byte[] command)
	{
		return command;
	}

	/// <summary>
	/// 根据对方返回的报文命令，对命令进行基本的拆包，例如各种Modbus协议拆包为统一的核心报文，还支持对报文的验证
	/// According to the message command returned by the other party, the command is basically unpacked, for example,
	/// various Modbus protocols are unpacked into a unified core message, and the verification of the message is also supported
	/// </summary>
	/// <param name="send">发送的原始报文数据</param>
	/// <param name="response">设备方反馈的原始报文内容</param>
	/// <returns>返回拆包之后的报文信息，默认不进行任何的拆包操作</returns>
	/// <remarks>
	/// 在实际解包的操作过程中，通常对状态码，错误码等消息进行判断，如果校验不通过，将携带错误消息返回
	/// During the actual unpacking operation, the status code, error code and other messages are usually judged. If the verification fails, the error message will be returned.
	/// </remarks>
	public virtual OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return OperateResult.CreateSuccessResult(response);
	}

	/// <summary>
	/// 获取本次操作的可用的网络通道，如果是短连接，就重新生成一个新的网络通道，如果是长连接，就复用当前的网络通道。
	/// Obtain the available network channels for this operation. If it is a short connection, a new network channel is regenerated.
	/// If it is a long connection, the current network channel is reused.
	/// </summary>
	/// <returns>是否成功，如果成功，使用这个套接字</returns>
	protected OperateResult<Socket> GetAvailableSocket()
	{
		if (isPersistentConn)
		{
			if (isUseSpecifiedSocket)
			{
				if (pipeSocket.IsSocketError)
				{
					return new OperateResult<Socket>(StringResources.Language.ConnectionIsNotAvailable);
				}
				return OperateResult.CreateSuccessResult(pipeSocket.Socket);
			}
			if ((mqttClient == null) ? pipeSocket.IsConnectitonError() : pipeSocket.IsSocketError)
			{
				OperateResult operateResult = ConnectServer();
				if (!operateResult.IsSuccess)
				{
					pipeSocket.IsSocketError = true;
					return OperateResult.CreateFailedResult<Socket>(operateResult);
				}
				pipeSocket.IsSocketError = false;
				return OperateResult.CreateSuccessResult(pipeSocket.Socket);
			}
			return OperateResult.CreateSuccessResult(pipeSocket.Socket);
		}
		return CreateSocketAndInitialication();
	}

	/// <summary>
	/// 尝试连接服务器，如果成功，并执行的初始化方法，并返回最终的结果。
	/// Attempt to connect to the server, if successful, and execute the initialization method of , and return the final result.
	/// </summary>
	/// <returns>带有socket的结果对象</returns>
	private OperateResult<Socket> CreateSocketAndInitialication()
	{
		OperateResult<Socket> operateResult = ((mqttClient == null) ? CreateSocketAndConnect(pipeSocket.GetConnectIPEndPoint(), ConnectTimeOut, LocalBinding) : OperateResult.CreateSuccessResult(pipeSocket.Socket));
		if (operateResult.IsSuccess)
		{
			OperateResult operateResult2 = InitializationOnConnect(operateResult.Content);
			if (!operateResult2.IsSuccess)
			{
				operateResult.Content?.Close();
				operateResult.IsSuccess = operateResult2.IsSuccess;
				operateResult.CopyErrorFromOther(operateResult2);
			}
		}
		return operateResult;
	}

	/// <summary>
	/// 将数据报文发送指定的网络通道上，根据当前指定的类型，返回一条完整的数据指令
	/// Sends a data message to the specified network channel, and returns a complete data command according to the currently specified  type
	/// </summary>
	/// <param name="socket">指定的套接字</param>
	/// <param name="send">发送的完整的报文信息</param>
	/// <param name="hasResponseData">是否有等待的数据返回，默认为 true</param>
	/// <param name="usePackAndUnpack">是否需要对命令重新打包，在重写方法后才会有影响</param>
	/// <returns>接收的完整的报文信息</returns>
	/// <remarks>
	/// 无锁的基于套接字直接进行叠加协议的操作。
	/// </remarks>
	public virtual OperateResult<byte[]> ReadFromCoreServer(Socket socket, byte[] send, bool hasResponseData = true, bool usePackAndUnpack = true)
	{
		byte[] array = (usePackAndUnpack ? PackCommandWithHeader(send) : send);
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.Send + " : " + (LogMsgFormatBinary ? array.ToHexString(' ') : SoftBasic.GetAsciiStringRender(array)));
		INetMessage newNetMessage = GetNewNetMessage();
		if (newNetMessage != null)
		{
			newNetMessage.SendBytes = array;
		}
		if (sendbyteBefore != null)
		{
			Send(socket, sendbyteBefore);
		}
		OperateResult operateResult = ((mqttClient == null) ? Send(socket, array) : mqttClient.PublishMessage(new MqttApplicationMessage
		{
			Topic = writeTopic,
			Payload = array
		}));
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		if (ReceiveTimeOut < 0)
		{
			return OperateResult.CreateSuccessResult(new byte[0]);
		}
		if (!hasResponseData)
		{
			return OperateResult.CreateSuccessResult(new byte[0]);
		}
		if (SleepTime > 0)
		{
			HslHelper.ThreadSleep(SleepTime);
		}
		OperateResult<byte[]> operateResult2 = null;
		if (useServerActivePush)
		{
			if (!autoResetEvent.WaitOne(ReceiveTimeOut))
			{
				NetSupport.CloseSocket(socket);
				pipeSocket.IsSocketError = true;
				return new OperateResult<byte[]>(-10000, StringResources.Language.ReceiveDataTimeout + ReceiveTimeOut);
			}
			newNetMessage.HeadBytes = bufferQA;
			operateResult2 = OperateResult.CreateSuccessResult(bufferQA);
		}
		else if (mqttClient != null)
		{
			MemoryStream memoryStream = new MemoryStream();
			while (true)
			{
				if (autoResetEvent.WaitOne(ReceiveTimeOut))
				{
					byte[] array2 = bufferQA;
					if (array2 != null && array2.Length != 0)
					{
						memoryStream.Write(bufferQA);
					}
					if (newNetMessage == null)
					{
						break;
					}
					if (memoryStream.Length < newNetMessage.ProtocolHeadBytesLength)
					{
						continue;
					}
					if (newNetMessage.HeadBytes == null)
					{
						byte[] array3 = memoryStream.ToArray();
						int num = newNetMessage.PependedUselesByteLength(array3);
						if (num > 0)
						{
							memoryStream = new MemoryStream();
							memoryStream.Write(array3.RemoveBegin(num));
							if (memoryStream.Length < newNetMessage.ProtocolHeadBytesLength)
							{
								continue;
							}
						}
						newNetMessage.HeadBytes = memoryStream.ToArray().SelectBegin(newNetMessage.ProtocolHeadBytesLength);
					}
					int contentLengthByHeadBytes = newNetMessage.GetContentLengthByHeadBytes();
					if (memoryStream.Length < newNetMessage.ProtocolHeadBytesLength + contentLengthByHeadBytes)
					{
						continue;
					}
					break;
				}
				return new OperateResult<byte[]>(-10000, StringResources.Language.ReceiveDataTimeout + ReceiveTimeOut);
			}
			operateResult2 = OperateResult.CreateSuccessResult(memoryStream.ToArray());
		}
		else if (newNetMessage == null)
		{
			DateTime now = DateTime.Now;
			MemoryStream memoryStream2 = new MemoryStream();
			while (true)
			{
				OperateResult<byte[]> operateResult3 = ReceiveByMessage(socket, ReceiveTimeOut, newNetMessage);
				if (!operateResult3.IsSuccess)
				{
					return operateResult3;
				}
				memoryStream2.Write(operateResult3.Content);
				if (CheckReceiveDataComplete(array, memoryStream2))
				{
					operateResult2 = OperateResult.CreateSuccessResult(memoryStream2.ToArray());
					break;
				}
				if (ReceiveTimeOut > 0 && (DateTime.Now - now).TotalMilliseconds > (double)ReceiveTimeOut)
				{
					operateResult2 = new OperateResult<byte[]>(StringResources.Language.ReceiveDataTimeout + ReceiveTimeOut);
					break;
				}
			}
		}
		else
		{
			operateResult2 = ReceiveByMessage(socket, ReceiveTimeOut, newNetMessage);
		}
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.Receive + " : " + (LogMsgFormatBinary ? operateResult2.Content.ToHexString(' ') : SoftBasic.GetAsciiStringRender(operateResult2.Content)));
		if (newNetMessage != null && !newNetMessage.CheckHeadBytesLegal(base.Token.ToByteArray()))
		{
			if (mqttClient == null)
			{
				NetSupport.CloseSocket(socket);
			}
			return new OperateResult<byte[]>(StringResources.Language.CommandHeadCodeCheckFailed + Environment.NewLine + StringResources.Language.Send + ": " + SoftBasic.ByteToHexString(array, ' ') + Environment.NewLine + StringResources.Language.Receive + ": " + SoftBasic.ByteToHexString(operateResult2.Content, ' '));
		}
		return usePackAndUnpack ? UnpackResponseContent(array, operateResult2.Content) : OperateResult.CreateSuccessResult(operateResult2.Content);
	}

	/// <summary>
	/// 将数据发送到当前的网络通道中，并从网络通道中接收一个指定的完整的报文，网络通道将根据方法自动获取，本方法是线程安全的。
	/// Send data to the current network channel and receive a complete message specified by  from the network channel.
	/// The network channel will be automatically obtained according to the  method This method is thread-safe.
	/// </summary>
	/// <param name="send">发送的完整的报文信息</param>
	/// <param name="hasResponseData">是否有等待的数据返回，默认为 true</param>
	/// <param name="usePackAndUnpack">是否需要对命令重新打包，在重写方法后才会有影响</param>
	/// <returns>接收的完整的报文信息</returns>
	/// <remarks>
	/// 本方法用于实现本组件还未实现的一些报文功能，例如有些modbus服务器会有一些特殊的功能码支持，需要收发特殊的报文，详细请看示例
	/// </remarks>
	public virtual OperateResult<byte[]> ReadFromCoreServer(byte[] send)
	{
		return ReadFromCoreServer(send, hasResponseData: true);
	}

	/// <summary>
	/// 将多个数据报文按顺序发到设备，并从设备接收返回的数据内容，然后拼接成一个Byte[]信息，需要重写方法才能返回正确的结果。
	/// Send multiple data packets to the device in sequence, and receive the returned data content from the device, and then splicing them into a Byte[] message,
	/// you need to rewrite  method to return the correct result.
	/// </summary>
	/// <param name="send">发送的报文列表信息</param>
	/// <returns>是否接收成功</returns>
	public OperateResult<byte[]> ReadFromCoreServer(IEnumerable<byte[]> send)
	{
		return NetSupport.ReadFromCoreServer(send, ReadFromCoreServer);
	}

	/// <summary>
	/// 将数据发送到当前的网络通道中，并从网络通道中接收一个指定的完整的报文，网络通道将根据方法自动获取，本方法是线程安全的。
	/// Send data to the current network channel and receive a complete message specified by  from the network channel.
	/// The network channel will be automatically obtained according to the  method This method is thread-safe.
	/// </summary>
	/// <param name="send">发送的完整的报文信息</param>
	/// <param name="hasResponseData">是否有等待的数据返回，默认为 true</param>
	/// <param name="usePackAndUnpack">是否需要对命令重新打包，在重写方法后才会有影响</param>
	/// <returns>接收的完整的报文信息</returns>
	/// <remarks>
	/// 本方法用于实现本组件还未实现的一些报文功能，例如有些modbus服务器会有一些特殊的功能码支持，需要收发特殊的报文，详细请看示例
	/// </remarks>
	public OperateResult<byte[]> ReadFromCoreServer(byte[] send, bool hasResponseData, bool usePackAndUnpack = true)
	{
		if (pipeSocket.LockingTick > HslHelper.LockLimit)
		{
			return new OperateResult<byte[]>(StringResources.Language.TooManyLock);
		}
		OperateResult<byte[]> operateResult = new OperateResult<byte[]>();
		OperateResult<Socket> operateResult2 = null;
		pipeSocket.PipeLockEnter();
		try
		{
			operateResult2 = GetAvailableSocket();
			if (!operateResult2.IsSuccess)
			{
				pipeSocket.IsSocketError = true;
				AlienSession?.Offline();
				pipeSocket.PipeLockLeave();
				operateResult.CopyErrorFromOther(operateResult2);
				return operateResult;
			}
			OperateResult<byte[]> operateResult3 = ReadFromCoreServer(operateResult2.Content, send, hasResponseData, usePackAndUnpack);
			if (operateResult3.IsSuccess)
			{
				pipeSocket.IsSocketError = false;
				operateResult.IsSuccess = operateResult3.IsSuccess;
				operateResult.Content = operateResult3.Content;
				operateResult.Message = StringResources.Language.SuccessText;
			}
			else
			{
				if (operateResult3.ErrorCode != int.MinValue)
				{
					pipeSocket.IsSocketError = true;
					AlienSession?.Offline();
				}
				else
				{
					operateResult3.ErrorCode = 10000;
				}
				operateResult.CopyErrorFromOther(operateResult3);
			}
			ExtraAfterReadFromCoreServer(operateResult3);
			pipeSocket.PipeLockLeave();
		}
		catch
		{
			pipeSocket.PipeLockLeave();
			throw;
		}
		if (!isPersistentConn)
		{
			operateResult2?.Content?.Close();
		}
		return operateResult;
	}

	/// <summary>
	/// 释放当前的资源，并自动关闭长连接，如果设置了的话
	/// </summary>
	/// <param name="disposing">是否释放托管的资源信息</param>
	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				ConnectClose();
			}
			disposedValue = true;
		}
	}

	/// <summary>
	/// 释放当前的资源，如果调用了本方法，那么该对象再使用的时候，需要重新实例化。
	/// Release the current resource. If this method is called, the object needs to be instantiated again when it is used again.
	/// </summary>
	public void Dispose()
	{
		Dispose(disposing: true);
	}

	public override string ToString()
	{
		INetMessage newNetMessage = GetNewNetMessage();
		string text = ((newNetMessage == null) ? "INetMessage" : newNetMessage.GetType().ToString());
		string text2 = ((ByteTransform == null) ? "IByteTransform" : ByteTransform.GetType().ToString());
		return $"NetworkDoubleBase<{newNetMessage}, {text2}>[{IpAddress}:{Port}]";
	}
}
