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
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Pipe;

namespace HslCommunication.Core.Net;

/// <summary>
/// 通信的服务器实现，包含了TCP服务器，UDP服务器，串口服务器操作
/// </summary>
public class CommunicationServer : CommunicationTcpServer
{
	public delegate void OnClientStatusChangeDelegate(object server, PipeSession session);

	public delegate void PipeMessageReceived(PipeSession session, byte[] buffer);

	private Socket udpServer = null;

	private EndPoint udpEndPoint = null;

	private byte[] udpBuffer;

	private List<PipeSession> pipes;

	private object lockSession = new object();

	private AsyncCallback beginReceiveCallback = null;

	private AsyncCallback udpBeginReceiveCallback = null;

	private AsyncCallback dtuSocketAsyncCallBack = null;

	/// <summary>
	/// 获取或设置一次接收时的数据长度，默认2KB数据长度，特殊情况的时候需要调整
	/// Gets or sets the length of data received at a time. The default length is 2KB
	/// </summary>
	public int UdpBufferSize { get; set; } = 2048;

	/// <summary>
	/// 获取一个新的消息对象的方法，需要在继承类里面进行重写
	/// The method to get a new message object needs to be overridden in the inheritance class
	/// </summary>
	/// <returns>消息类对象</returns>
	public Func<INetMessage> CreateNewMessage { get; set; }

	/// <summary>
	/// 检查串口接收到的数据是否完整
	/// </summary>
	public Func<byte[], int, bool> CheckSerialDataComplete { get; set; }

	/// <summary>
	/// 获取或设置当前允许登录的最大客户端数量，默认为 uint.MaxValue = 4294967295
	/// Gets or sets the maximum number of clients that are currently allowed to log in, which defaults to uint.MaxValue = 4294967295
	/// </summary>
	public uint SessionsMax { get; set; } = uint.MaxValue;

	/// <summary>
	/// 当线程检查后，进行登录之前的检查，通常用于自定义的握手包校验操作。仅对TCP通信的时候有效。
	/// </summary>
	public Func<PipeSession, IPEndPoint, OperateResult> ThreadPoolLoginAfterClientCheck { get; set; }

	/// <summary>
	/// 创建会话状态的委托对象，也就可以自己指定创建自定义的会话
	/// </summary>
	public Func<CommunicationPipe, PipeSession> CreatePipeSession { get; set; }

	/// <summary>
	/// 获取或设置串口模式下，接收一条数据最短的时间要求，当设备发送的数据非常慢的时候，或是分割发送数据的时候，就需要将本值设置的大一点，默认为20ms
	/// Get or set the shortest time required to receive a piece of data in serial port mode.
	/// When the data sent by the device is very slow, or when the data is divided and sent, you need to set this value to a larger value, the default is 20ms
	/// </summary>
	public int SerialReceiveAtleastTime { get; set; } = 20;

	/// <summary>
	/// 获取或设置当前的服务器接收串口数据时候，是否强制只接收一次数据，默认为false，适合点对点通信，如果你总线形式的连接，则需要设置 True
	/// Get or set whether to force the data to be received only once when the current server receives serial port data. The default value is false,
	/// which is suitable for point-to-point communication. If you have a bus connection, you need to set True
	/// </summary>
	public bool ForceSerialReceiveOnce { get; set; }

	/// <summary>
	/// 当客户端上线时候的触发的事件
	/// Event triggered when the client goes online
	/// </summary>
	public event OnClientStatusChangeDelegate OnClientOnline;

	/// <summary>
	/// 当客户端下线时候的触发的事件
	/// Event triggered when the client goes offline
	/// </summary>
	public event OnClientStatusChangeDelegate OnClientOffline;

	/// <summary>
	/// 当管道接收到消息时触发
	/// </summary>
	public event PipeMessageReceived OnPipeMessageReceived;

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public CommunicationServer()
	{
		beginReceiveCallback = ReceiveCallback;
		udpBeginReceiveCallback = UdpAsyncCallback;
		dtuSocketAsyncCallBack = InitiativeSocketAsyncCallBack;
		CreatePipeSession = (CommunicationPipe m) => new PipeSession
		{
			Communication = m
		};
		pipes = new List<PipeSession>();
	}

	/// <summary>
	/// 指定端口号来启动服务器的引擎
	/// Specify the port number to start the server's engine
	/// </summary>
	/// <param name="port">指定一个端口号</param>
	/// <param name="modeTcp">是否使用TCP格式，如果需要UDP，则为 false</param>
	public void ServerStart(int port, bool modeTcp)
	{
		if (modeTcp)
		{
			ServerStart(port);
		}
		else if (!base.IsStarted)
		{
			if (!base.EnableIPv6)
			{
				udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				udpServer.Bind(new IPEndPoint(base.LocalAddress ?? IPAddress.Any, port));
			}
			else
			{
				udpServer = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
				udpServer.Bind(new IPEndPoint(base.LocalAddress ?? IPAddress.IPv6Any, port));
			}
			base.IsStarted = true;
			base.Port = port;
			if (udpBuffer == null)
			{
				udpBuffer = new byte[UdpBufferSize];
			}
			udpEndPoint = new IPEndPoint(base.EnableIPv6 ? IPAddress.Parse("::1") : IPAddress.Parse("127.0.0.1"), 0);
			PipeUdpNet arg = new PipeUdpNet
			{
				Socket = udpServer
			};
			PipeSession pipeSession = CreatePipeSession(arg);
			SetUdpIp(pipeSession);
			AddSession(pipeSession);
			UdpRefreshReceive(pipeSession);
			ExtraOnStart();
			LogDebugMsg(StringResources.Language.NetEngineStart);
		}
	}

	/// <summary>
	/// 指定一个TCP端口及UDP端口，同时启动两种模式的服务器
	/// Specify a TCP port and a UDP port to start the server in both modes at the same time
	/// </summary>
	/// <param name="tcpPort">tcp端口</param>
	/// <param name="udpPort">udp端口</param>
	public void ServerStart(int tcpPort, int udpPort)
	{
		if (!base.IsStarted)
		{
			ServerStart(tcpPort);
			if (!base.EnableIPv6)
			{
				udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				udpServer.Bind(new IPEndPoint(base.LocalAddress ?? IPAddress.Any, udpPort));
			}
			else
			{
				udpServer = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
				udpServer.Bind(new IPEndPoint(base.LocalAddress ?? IPAddress.IPv6Any, udpPort));
			}
			if (udpBuffer == null)
			{
				udpBuffer = new byte[UdpBufferSize];
			}
			udpEndPoint = new IPEndPoint(base.EnableIPv6 ? IPAddress.Parse("::1") : IPAddress.Parse("127.0.0.1"), 0);
			PipeUdpNet arg = new PipeUdpNet
			{
				Socket = udpServer
			};
			PipeSession pipeSession = CreatePipeSession(arg);
			SetUdpIp(pipeSession);
			AddSession(pipeSession);
			UdpRefreshReceive(pipeSession);
		}
	}

	protected override void ExtraOnClose()
	{
		base.ExtraOnClose();
		lock (lockSession)
		{
			for (int i = 0; i < pipes.Count; i++)
			{
				pipes[i].Close();
			}
			pipes.Clear();
		}
		if (udpServer != null)
		{
			NetSupport.CloseSocket(udpServer);
		}
	}

	/// <summary>
	/// 设置当前的服务器接收的消息信息
	/// </summary>
	/// <param name="netMessage">消息对象</param>
	public void SetNetMessage(INetMessage netMessage)
	{
		CreateNewMessage = () => netMessage;
	}

	/// <summary>
	/// 新增加一个管道会话信息
	/// A new pipeline session information has been added
	/// </summary>
	/// <param name="session">管道会话</param>
	public void AddSession(PipeSession session)
	{
		lock (lockSession)
		{
			pipes.Add(session);
		}
		LogDebugMsg(string.Format(StringResources.Language.ClientOnlineInfo, session.Communication));
		OnClientOnline?.Invoke(this, session);
	}

	/// <summary>
	/// 移除一个管道会话
	/// Remove a pipeline session
	/// </summary>
	/// <param name="session">管道会话</param>
	/// <param name="reason">移除的原因</param>
	public void RemoveSession(PipeSession session, string reason)
	{
		bool success = false;
		lock (lockSession)
		{
			success = pipes.Remove(session);
		}
		if (success)
		{
			session.Close();
			LogDebugMsg(string.Format(StringResources.Language.ClientOfflineInfo, session.Communication) + " " + reason);
			OnClientOffline?.Invoke(this, session);
		}
	}

	/// <summary>
	/// 获取管道会话的列表
	/// Get a list of pipeline sessions
	/// </summary>
	/// <returns>会话列表</returns>
	public PipeSession[] GetPipeSessions()
	{
		PipeSession[] result = null;
		lock (lockSession)
		{
			result = pipes.ToArray();
		}
		return result;
	}

	/// <summary>
	/// 指定超时时间移除当前的会话列表，只有是TCP的管道（）才需要被移除。
	/// Specify a timeout to remove the current session list. Only TCP pipe () need to be removed.
	/// </summary>
	/// <param name="timeSpan">指定的超时时间</param>
	public void RemoveSession(TimeSpan timeSpan)
	{
		lock (lockSession)
		{
			for (int num = pipes.Count - 1; num >= 0; num--)
			{
				PipeSession pipeSession = pipes[num];
				if (pipeSession.Communication is PipeTcpNet pipeTcpNet && pipeTcpNet.GetType() == typeof(PipeTcpNet) && DateTime.Now - pipeSession.HeartTime > timeSpan)
				{
					pipeSession.Close();
					pipes.RemoveAt(num);
					LogDebugMsg(string.Format(StringResources.Language.ClientOfflineInfo, pipeSession.Communication) + $" Not communication for times[{timeSpan}]");
				}
			}
		}
	}

	/// <summary>
	/// 新增一个主动连接的请求，将不会收到是否连接成功的信息，当网络中断及奔溃之后，会自动重新连接。
	/// A new active connection request will not receive a message whether the connection is successful. When the network is interrupted and crashed, it will automatically reconnect.
	/// </summary>
	/// <param name="ipAddress">对方的Ip地址</param>
	/// <param name="port">端口号</param>
	/// <param name="dtu">使用自定义的DTU数据报文</param>
	public RemoteConnectInfo ConnectRemoteServer(string ipAddress, int port, byte[] dtu = null)
	{
		RemoteConnectInfo remoteConnectInfo = new RemoteConnectInfo(ipAddress, port, dtu);
		ThreadPool.QueueUserWorkItem(ConnectIpEndPoint, remoteConnectInfo);
		return remoteConnectInfo;
	}

	/// <summary>
	/// 创建一个指定的异形客户端连接，使用Hsl协议来发送注册包
	/// Create a specified profiled client connection and use the Hsl protocol to send registration packets
	/// </summary>
	/// <param name="ipAddress">Ip地址</param>
	/// <param name="port">端口号</param>
	/// <param name="dtuId">设备唯一ID号，最长11</param>
	/// <param name="password">密码信息</param>
	/// <param name="needAckResult">是否需要返回注册结果报文</param>
	/// <returns>是否成功连接</returns>
	public RemoteConnectInfo ConnectHslAlientClient(string ipAddress, int port, string dtuId, string password = "", bool needAckResult = true)
	{
		RemoteConnectInfo remoteConnectInfo = new RemoteConnectInfo(ipAddress, port, dtuId, password, needAckResult);
		ThreadPool.QueueUserWorkItem(ConnectIpEndPoint, remoteConnectInfo);
		return remoteConnectInfo;
	}

	private OperateResult CheckConnectRemote(RemoteConnectInfo remoteConnect, PipeSession session)
	{
		PipeTcpNet pipeTcpNet = session.Communication as PipeTcpNet;
		if (remoteConnect.DtuBytes != null)
		{
			OperateResult operateResult = pipeTcpNet.Send(remoteConnect.DtuBytes);
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
			if (remoteConnect.NeedAckResult)
			{
				OperateResult<byte[]> operateResult2 = pipeTcpNet.ReceiveMessage(new AlienMessage(), null);
				if (!operateResult2.IsSuccess)
				{
					return operateResult2;
				}
				switch (operateResult2.Content[5])
				{
				case 1:
					return new OperateResult(StringResources.Language.DeviceCurrentIsLoginRepeat);
				case 2:
					return new OperateResult(StringResources.Language.DeviceCurrentIsLoginForbidden);
				case 3:
					return new OperateResult(StringResources.Language.PasswordCheckFailed);
				}
			}
		}
		OperateResult operateResult3 = SocketAcceptExtraCheck(pipeTcpNet.Socket, remoteConnect.EndPoint);
		if (!operateResult3.IsSuccess)
		{
			return operateResult3;
		}
		if (ThreadPoolLoginAfterClientCheck != null)
		{
			OperateResult operateResult4 = ThreadPoolLoginAfterClientCheck(session, remoteConnect.EndPoint);
			if (!operateResult4.IsSuccess)
			{
				return operateResult4;
			}
		}
		return OperateResult.CreateSuccessResult();
	}

	private void ConnectIpEndPoint(object obj)
	{
		if (!(obj is RemoteConnectInfo remoteConnectInfo))
		{
			return;
		}
		if (remoteConnectInfo.Status == DtuStatus.Closed)
		{
			remoteConnectInfo.Session?.Close();
			return;
		}
		remoteConnectInfo.Status = DtuStatus.Connecting;
		OperateResult<Socket> operateResult = NetSupport.CreateSocketAndConnect(remoteConnectInfo.EndPoint, 10000);
		if (!operateResult.IsSuccess)
		{
			LogDebugMsg($"RemoteConnectInfo[{remoteConnectInfo.EndPoint}] Socket Connected Failed : {operateResult.Message} 10s later retry...");
			HslHelper.ThreadSleep(10000);
			ThreadPool.QueueUserWorkItem(ConnectIpEndPoint, remoteConnectInfo);
			return;
		}
		LogDebugMsg($"RemoteConnectInfo[{remoteConnectInfo.EndPoint}] Socket Connected Success");
		PipeTcpNet pipeTcpNet = new PipeTcpNet(remoteConnectInfo.EndPoint.Address.ToString(), remoteConnectInfo.EndPoint.Port);
		pipeTcpNet.Socket = operateResult.Content;
		PipeSession pipeSession = CreatePipeSession(pipeTcpNet);
		OperateResult operateResult2 = CheckConnectRemote(remoteConnectInfo, pipeSession);
		if (!operateResult2.IsSuccess)
		{
			LogDebugMsg($"RemoteConnectInfo[{remoteConnectInfo.EndPoint}] Socket Check Failed : {operateResult2.Message} 10s later retry...");
			NetSupport.CloseSocket(operateResult.Content);
			HslHelper.ThreadSleep(10000);
			ThreadPool.QueueUserWorkItem(ConnectIpEndPoint, remoteConnectInfo);
			return;
		}
		LogDebugMsg($"RemoteConnectInfo[{remoteConnectInfo.EndPoint}] Socket Check Success");
		remoteConnectInfo.Session = pipeSession;
		remoteConnectInfo.Status = DtuStatus.Connected;
		pipeSession.OnlineTime = DateTime.Now;
		try
		{
			pipeTcpNet.Socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, dtuSocketAsyncCallBack, remoteConnectInfo);
			AddSession(pipeSession);
		}
		catch (Exception ex)
		{
			LogDebugMsg($"ConnectIpEndPoint[{remoteConnectInfo.EndPoint}] Socket.BeginReceive failed: " + ex.Message);
			NetSupport.CloseSocket(pipeTcpNet.Socket);
			ThreadPool.QueueUserWorkItem(ConnectIpEndPoint, remoteConnectInfo);
		}
	}

	private void InitiativeSocketAsyncCallBack(IAsyncResult ar)
	{
		if (!(ar.AsyncState is RemoteConnectInfo { Session: var session } remoteConnectInfo))
		{
			return;
		}
		PipeTcpNet pipeTcpNet = session.Communication as PipeTcpNet;
		byte[] array = null;
		try
		{
			Socket socket = pipeTcpNet.Socket;
			if (socket == null)
			{
				RemoveSession(session, string.Empty);
				return;
			}
			int num = socket.EndReceive(ar);
			INetMessage newNetMessage = GetNewNetMessage();
			OperateResult<byte[]> operateResult = pipeTcpNet.ReceiveMessage(newNetMessage, null, useActivePush: false);
			if (!operateResult.IsSuccess)
			{
				RemoveSession(session, operateResult.Message);
				if (base.IsStarted)
				{
					LogDebugMsg($"RemoteConnectInfo[{remoteConnectInfo.EndPoint}] Socket Connected 10s later retry...");
					HslHelper.ThreadSleep(10000);
					ConnectIpEndPoint(remoteConnectInfo);
				}
				return;
			}
			session.HeartTime = DateTime.Now;
			array = operateResult.Content;
		}
		catch (Exception ex)
		{
			RemoveSession(session, ex.Message);
			if (base.IsStarted)
			{
				ConnectIpEndPoint(remoteConnectInfo);
			}
			return;
		}
		OnPipeMessageReceived?.Invoke(session, array);
		try
		{
			pipeTcpNet.Socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, dtuSocketAsyncCallBack, remoteConnectInfo);
		}
		catch (Exception ex2)
		{
			RemoveSession(session, ex2.Message);
			HslHelper.ThreadSleep(1000);
			if (base.IsStarted)
			{
				ConnectIpEndPoint(remoteConnectInfo);
			}
		}
	}

	private void SetUdpIp(PipeSession session)
	{
		PipeUdpNet pipeUdpNet = session.Communication as PipeUdpNet;
		if (udpEndPoint is IPEndPoint iPEndPoint)
		{
			pipeUdpNet.IpAddress = iPEndPoint.Address.ToString();
			pipeUdpNet.Port = iPEndPoint.Port;
		}
	}

	private void UdpRefreshReceive(PipeSession session)
	{
		try
		{
			udpServer.BeginReceiveFrom(udpBuffer, 0, udpBuffer.Length, SocketFlags.None, ref udpEndPoint, udpBeginReceiveCallback, session);
		}
		catch (Exception ex)
		{
			RemoveSession(session, "UdpRefreshReceive exception: " + ex.Message);
		}
	}

	private void UdpAsyncCallback(IAsyncResult ar)
	{
		if (!(ar.AsyncState is PipeSession pipeSession))
		{
			return;
		}
		PipeUdpNet pipeUdpNet = pipeSession.Communication as PipeUdpNet;
		byte[] array = null;
		if (pipeUdpNet.Socket == null)
		{
			RemoveSession(pipeSession, "Server closed");
			return;
		}
		try
		{
			int num = pipeUdpNet.Socket.EndReceiveFrom(ar, ref udpEndPoint);
			pipeSession.HeartTime = DateTime.Now;
			SetUdpIp(pipeSession);
			if (!Authorization.nzugaydgwadawdibbas())
			{
				RemoveSession(pipeSession, StringResources.Language.AuthorizationFailed);
				return;
			}
			INetMessage newNetMessage = GetNewNetMessage();
			if (newNetMessage != null)
			{
				OperateResult<byte[]> operateResult = pipeUdpNet.ReceiveMessage(newNetMessage, null, udpBuffer.SelectBegin(num), null, closeOnException: false);
				if (!operateResult.IsSuccess)
				{
					LogDebugMsg($"<{pipeUdpNet}> Udp ReceiveMessage faild: " + operateResult.Message + ((num > 0) ? udpBuffer.SelectBegin(num).ToHexString(' ') : string.Empty));
					UdpRefreshReceive(pipeSession);
					return;
				}
				array = operateResult.Content;
			}
			else
			{
				array = udpBuffer.SelectBegin(num);
			}
		}
		catch (ObjectDisposedException)
		{
			RemoveSession(pipeSession, "Socket ObjectDisposedException");
			return;
		}
		catch (Exception ex2)
		{
			LogDebugMsg($"<{pipeUdpNet}> UdpAsyncCallback faild: " + ex2.Message);
			UdpRefreshReceive(pipeSession);
			return;
		}
		OnPipeMessageReceived?.Invoke(pipeSession, array);
		UdpRefreshReceive(pipeSession);
	}

	/// <summary>
	/// 当客户端连接到服务器，并听过额外的检查后，进行回调的方法
	/// Callback method when the client connects to the server and has heard additional checks
	/// </summary>
	/// <param name="pipeTcpNet">socket对象</param>
	/// <param name="endPoint">远程的终结点</param>
	protected override void ThreadPoolLogin(PipeTcpNet pipeTcpNet, IPEndPoint endPoint)
	{
		PipeSession pipeSession = CreatePipeSession(pipeTcpNet);
		if (ThreadPoolLoginAfterClientCheck != null)
		{
			OperateResult operateResult = ThreadPoolLoginAfterClientCheck(pipeSession, endPoint);
			if (!operateResult.IsSuccess)
			{
				base.LogNet?.WriteDebug(ToString(), string.Format(StringResources.Language.ClientDisableLogin, endPoint) + " LoginAfterClientCheck failed");
				pipeTcpNet?.CloseCommunication();
				return;
			}
		}
		if (GetPipeSessions().Length >= SessionsMax)
		{
			base.LogNet?.WriteDebug(ToString(), string.Format(StringResources.Language.ClientDisableLogin, endPoint) + $" Online count > SessionsMax({SessionsMax})");
			pipeTcpNet?.CloseCommunication();
			return;
		}
		try
		{
			pipeTcpNet.Socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, beginReceiveCallback, pipeSession);
			AddSession(pipeSession);
		}
		catch (Exception ex)
		{
			LogDebugMsg(StringResources.Language.SocketReceiveException + " " + ex.Message);
		}
	}

	private async void ReceiveCallback(IAsyncResult ar)
	{
		object asyncState = ar.AsyncState;
		if (!(asyncState is PipeSession session))
		{
			return;
		}
		PipeTcpNet pipeTcpNet = session.Communication as PipeTcpNet;
		byte[] buffer;
		try
		{
			Socket client = pipeTcpNet.Socket;
			if (client == null)
			{
				RemoveSession(session, string.Empty);
				return;
			}
			client.EndReceive(ar);
			INetMessage netMessage = GetNewNetMessage();
			OperateResult<byte[]> read = await pipeTcpNet.ReceiveMessageAsync(netMessage, null, useActivePush: false);
			if (!read.IsSuccess)
			{
				RemoveSession(session, read.Message);
				return;
			}
			session.HeartTime = DateTime.Now;
			buffer = read.Content;
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			if (ex2.Message.Contains(StringResources.Language.SocketRemoteCloseException))
			{
				RemoveSession(session, string.Empty);
			}
			else
			{
				RemoveSession(session, ex2.Message);
			}
			return;
		}
		OnPipeMessageReceived?.Invoke(session, buffer);
		try
		{
			pipeTcpNet.Socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, beginReceiveCallback, session);
		}
		catch (Exception ex3)
		{
			RemoveSession(session, ex3.Message);
		}
	}

	/// <summary>
	/// 启动串口的从机服务，使用默认的参数进行初始化串口，9600波特率，8位数据位，无奇偶校验，1位停止位
	/// Start the slave service of serial, initialize the serial port with default parameters, 9600 baud rate, 8 data bits, no parity, 1 stop bit
	/// </summary>
	/// <param name="com">串口信息</param>
	/// <remarks>
	/// com支持格式化的方式，例如输入 COM3-9600-8-N-1，COM5-19200-7-E-2，其中奇偶校验的字母可选，N:无校验，O：奇校验，E:偶校验，停止位可选 0, 1, 2, 1.5 四种选项
	/// </remarks>
	public OperateResult StartSerialSlave(string com)
	{
		if (com.Contains("-") || com.Contains(";"))
		{
			return StartSerialSlave(delegate(SerialPort sp)
			{
				sp.IniSerialByFormatString(com);
			});
		}
		return StartSerialSlave(com, 9600);
	}

	/// <summary>
	/// 启动串口的从机服务，使用默认的参数进行初始化串口，8位数据位，无奇偶校验，1位停止位
	/// Start the slave service of serial, initialize the serial port with default parameters, 8 data bits, no parity, 1 stop bit
	/// </summary>
	/// <param name="com">串口信息</param>
	/// <param name="baudRate">波特率</param>
	public OperateResult StartSerialSlave(string com, int baudRate)
	{
		return StartSerialSlave(delegate(SerialPort sp)
		{
			sp.PortName = com;
			sp.BaudRate = baudRate;
			sp.DataBits = 8;
			sp.Parity = (Parity)0;
			sp.StopBits = (StopBits)1;
		});
	}

	/// <summary>
	/// 启动串口的从机服务，使用指定的参数进行初始化串口，指定数据位，指定奇偶校验，指定停止位
	/// </summary>
	/// <param name="com">串口信息</param>
	/// <param name="baudRate">波特率</param>
	/// <param name="dataBits">数据位</param>
	/// <param name="parity">奇偶校验</param>
	/// <param name="stopBits">停止位</param>
	public OperateResult StartSerialSlave(string com, int baudRate, int dataBits, Parity parity, StopBits stopBits)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		return StartSerialSlave(delegate(SerialPort sp)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			sp.PortName = com;
			sp.BaudRate = baudRate;
			sp.DataBits = dataBits;
			sp.Parity = parity;
			sp.StopBits = stopBits;
		});
	}

	/// <summary>
	/// 启动串口的从机服务，使用自定义的初始化方法初始化串口的参数
	/// Start the slave service of serial and initialize the parameters of the serial port using a custom initialization method
	/// </summary>
	/// <param name="inni">初始化信息的委托</param>
	public OperateResult StartSerialSlave(Action<SerialPort> inni)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		PipeSerialPort pipeSerialPort = new PipeSerialPort();
		pipeSerialPort.SerialPortInni(inni);
		pipeSerialPort.GetPipe().DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
		OperateResult operateResult = pipeSerialPort.OpenCommunication();
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		AddSession(CreatePipeSession(pipeSerialPort));
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 关闭提供从机服务的串口对象
	/// Close the serial port object that provides slave services
	/// </summary>
	public void CloseSerialSlave()
	{
		lock (lockSession)
		{
			for (int num = pipes.Count - 1; num >= 0; num--)
			{
				PipeSession pipeSession = pipes[num];
				if (pipeSession.Communication is PipeSerialPort pipeSerialPort)
				{
					pipeSerialPort.CloseCommunication();
					pipes.RemoveAt(num);
				}
			}
		}
	}

	private PipeSession FindSerialPortSession(string portName)
	{
		lock (lockSession)
		{
			for (int num = pipes.Count - 1; num >= 0; num--)
			{
				PipeSession pipeSession = pipes[num];
				if (pipeSession.Communication is PipeSerialPort pipeSerialPort && pipeSerialPort.GetPipe().PortName == portName)
				{
					return pipeSession;
				}
			}
		}
		return null;
	}

	/// <summary>
	/// 接收到串口数据的时候触发
	/// </summary>
	/// <param name="sender">串口对象</param>
	/// <param name="e">消息</param>
	private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		if (!Authorization.nzugaydgwadawdibbas())
		{
			return;
		}
		SerialPort val = (SerialPort)((sender is SerialPort) ? sender : null);
		PipeSession pipeSession = FindSerialPortSession(val.PortName);
		if (pipeSession == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		byte[] buffer = new byte[2048];
		DateTime now = DateTime.Now;
		while (true)
		{
			try
			{
				int bytesRead = val.Read(buffer, num, Math.Min(buffer.Length - num, val.BytesToRead));
				if (bytesRead != 0 || num2 == 0 || !((DateTime.Now - now).TotalMilliseconds >= (double)SerialReceiveAtleastTime))
				{
					num += bytesRead;
					if (bytesRead > 0 && CheckSerialDataComplete != null)
					{
						now = DateTime.Now;
					}
					if (num < buffer.Length)
					{
						num2++;
						goto IL_0102;
					}
				}
			}
			catch (Exception ex)
			{
				LogDebugMsg("SerialPort_DataReceived Error: " + ex.Message);
			}
			break;
			IL_0102:
			if ((ForceSerialReceiveOnce && num > 0) || CheckSerialReceiveDataComplete(buffer, num))
			{
				break;
			}
			HslHelper.ThreadSleep(20);
		}
		if (num != 0)
		{
			try
			{
				buffer = buffer.SelectBegin(num);
			}
			catch (Exception ex2)
			{
				LogDebugMsg("SerialPort_DataReceived: " + ex2.Message);
			}
			pipeSession.HeartTime = DateTime.Now;
			OnPipeMessageReceived?.Invoke(pipeSession, buffer);
		}
	}

	/// <summary>
	/// 检查串口接收的数据是否完成的方法，如果接收完成，则返回True
	/// </summary>
	/// <param name="buffer">缓存的数据信息</param>
	/// <param name="receivedLength">当前已经接收的数据长度信息</param>
	/// <returns>是否接收完成</returns>
	protected virtual bool CheckSerialReceiveDataComplete(byte[] buffer, int receivedLength)
	{
		if (CheckSerialDataComplete == null)
		{
			return false;
		}
		return CheckSerialDataComplete(buffer, receivedLength);
	}

	/// <summary>
	/// 获取一个新的消息对象的方法，需要在继承类里面进行重写
	/// The method to get a new message object needs to be overridden in the inheritance class
	/// </summary>
	/// <returns>消息类对象</returns>
	protected virtual INetMessage GetNewNetMessage()
	{
		return (CreateNewMessage == null) ? null : CreateNewMessage();
	}

	public override string ToString()
	{
		return $"CommunicationServer[{base.Port}]";
	}
}
