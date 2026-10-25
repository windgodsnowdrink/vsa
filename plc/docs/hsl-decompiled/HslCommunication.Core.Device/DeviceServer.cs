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
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using HslCommunication.Core.Net;

namespace HslCommunication.Core.Device;

/// <summary>
/// 设备服务器类
/// </summary>
public class DeviceServer : DeviceCommunication
{
	public delegate void DataReceivedDelegate(object sender, PipeSession session, byte[] data);

	public delegate void DataSendDelegate(object sender, byte[] data);

	private Timer timerHeart;

	private CommunicationServer server;

	private int udpServePort = 0;

	/// <summary>
	/// 服务器引擎是否启动
	/// Whether the server engine is started
	/// </summary>
	public bool IsStarted { get; private set; }

	/// <summary>
	/// 获取或设置服务器的端口号，如果是设置，需要在服务器启动前设置完成，才能生效。
	/// Gets or sets the port number of the server. If it is set, it needs to be set before the server starts to take effect.
	/// </summary>
	/// <remarks>
	/// 需要在服务器启动之前设置为有效
	/// </remarks>
	public int Port
	{
		get
		{
			return server.Port;
		}
		set
		{
			server.Port = value;
		}
	}

	public int BothModeUdpPort => udpServePort;

	/// <summary>
	/// 获取或设置服务器是否支持IPv6的地址协议信息
	/// Get or set whether the server supports IPv6 address protocol information
	/// </summary>
	/// <remarks>
	/// 默认为 False，也就是不启动
	/// </remarks>
	public bool EnableIPv6
	{
		get
		{
			return server.EnableIPv6;
		}
		set
		{
			server.EnableIPv6 = value;
		}
	}

	/// <summary>
	/// 获取或设置客户端的Socket的心跳时间信息，这个是Socket底层自动实现的心跳包，不基于协议层实现。默认小于0，不开启心跳检测，如果需要开启，设置 60_000 比较合适，单位毫秒
	/// Get or set the heartbeat time information of the Socket of the client. This is the heartbeat packet automatically implemented by the bottom layer of the Socket, not based on the protocol layer.
	/// The default value is less than 0, and heartbeat detection is not enabled. If you need to enable it, it is more appropriate to set 60_000, in milliseconds.
	/// </summary>
	/// <remarks>
	/// 经测试，在linux上，基于.net core3.1的程序运行时，设置了这个值是无效的。
	/// </remarks>
	public int SocketKeepAliveTime
	{
		get
		{
			return server.SocketKeepAliveTime;
		}
		set
		{
			server.SocketKeepAliveTime = value;
		}
	}

	/// <summary>
	/// 获取或设置当前的服务器是否允许远程客户端进行写入数据操作，默认为True
	/// Gets or sets whether the current server allows remote clients to write data, the default is True
	/// </summary>
	/// <remarks>
	/// 如果设置为False，那么所有远程客户端的操作都会失败，直接返回错误码或是关闭连接。
	/// </remarks>
	public bool EnableWrite { get; set; } = true;

	/// <summary>
	/// 获取或设置两次数据交互时的最小时间间隔，默认为24小时。如果超过该设定的时间不进行数据交互，服务器就会强制断开当前的连接操作。
	/// Get or set the minimum time interval between two data interactions, the default is 24 hours.
	/// If the data exchange is not performed for more than the set time, the server will forcibly disconnect the current connection operation.
	/// </summary>
	/// <remarks>
	/// 举例设置为10分钟，ActiveTimeSpan = TimeSpan.FromMinutes( 10 );
	/// </remarks>
	public TimeSpan ActiveTimeSpan { get; set; }

	/// <summary>
	/// 获取或设置串口模式下，接收一条数据最短的时间要求，当设备发送的数据非常慢的时候，或是分割发送数据的时候，就需要将本值设置的大一点，默认为20ms
	/// Get or set the shortest time required to receive a piece of data in serial port mode.
	/// When the data sent by the device is very slow, or when the data is divided and sent, you need to set this value to a larger value, the default is 20ms
	/// </summary>
	public int SerialReceiveAtleastTime
	{
		get
		{
			return server.SerialReceiveAtleastTime;
		}
		set
		{
			server.SerialReceiveAtleastTime = value;
		}
	}

	/// <summary>
	/// 获取或设置当前的服务器接收串口数据时候，是否强制只接收一次数据，默认为false，适合点对点通信，如果你总线形式的连接，则需要设置 True
	/// Get or set whether to force the data to be received only once when the current server receives serial port data. The default value is false,
	/// which is suitable for point-to-point communication. If you have a bus connection, you need to set True
	/// </summary>
	public bool ForceSerialReceiveOnce
	{
		get
		{
			return server.ForceSerialReceiveOnce;
		}
		set
		{
			server.ForceSerialReceiveOnce = value;
		}
	}

	/// <summary>
	/// 获取或设置服务器绑定的本地IP地址，默认为空，使用本地所有可用的ip地址
	/// Obtain or set the local IP address bound to the server, which is empty by default, and use all available IP addresses locally
	/// </summary>
	public IPAddress LocalAddress
	{
		get
		{
			return server.LocalAddress;
		}
		set
		{
			server.LocalAddress = value;
		}
	}

	public int OnlineCount => server.GetPipeSessions().Length;

	/// <summary>
	/// 当前服务器的模式，0：TCP服务器，1：UDP服务器，2：TCP及UDP服务器
	/// Gets whether the current server is a TCP server or a UDP server
	/// </summary>
	public int ServerMode { get; private set; }

	/// <summary>
	/// 故障注入器，用于在响应发回客户端之前注入延迟、丢包、错帧、断链，方便上位机调试超时、重连、
	/// 校验等异常场景。默认是关闭状态，关闭时不影响服务器的任何原有行为
	/// Fault injector, used to inject delay, packet loss, wrong frame and disconnection before the response
	/// is sent back to the client. It is disabled by default and does not affect any original behavior of
	/// the server when it is disabled
	/// </summary>
	public ServerFaultInjection FaultInjection { get; } = new ServerFaultInjection();

	/// <summary>
	/// 接收到数据的时候就触发的事件，示例详细参考API文档信息
	/// An event that is triggered when data is received
	/// </summary>
	/// <remarks>
	/// 事件共有三个参数，sender指服务器本地的对象，例如  对象，source 指会话对象，网口对象为 ，
	/// 串口为 对象，需要根据实际判断，data 为收到的原始数据 byte[] 对象
	/// </remarks>
	public event DataReceivedDelegate OnDataReceived;

	/// <summary>
	/// 数据发送的时候就触发的事件
	/// Events that are triggered when data is sent
	/// </summary>
	public event DataSendDelegate OnDataSend;

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public DeviceServer()
	{
		ActiveTimeSpan = TimeSpan.FromHours(24.0);
		server = new CommunicationServer();
		server.OnPipeMessageReceived += Server_OnPipeMessageReceived;
		server.LogDebugMessage = delegate(string m)
		{
			base.LogNet?.WriteDebug(ToString(), m);
		};
		server.CreateNewMessage = GetNewNetMessage;
		server.ThreadPoolLoginAfterClientCheck = ThreadPoolLoginAfterClientCheck;
		server.CheckSerialDataComplete = CheckSerialReceiveDataComplete;
		timerHeart = new Timer(ThreadTimerHeartCheck, null, 2000, 10000);
	}

	/// <summary>
	/// 获取当前的核心服务器信息
	/// </summary>
	/// <returns>核心服务器</returns>
	public CommunicationServer GetCommunicationServer()
	{
		return server;
	}

	/// <summary>
	/// 当客户端的socket登录的时候额外检查的操作，并返回操作的结果信息。
	/// The operation is additionally checked when the client's socket logs in, and the result information of the operation is returned.
	/// </summary>
	/// <param name="socket">套接字</param>
	/// <param name="endPoint">终结点</param>
	/// <returns>验证的结果</returns>
	protected virtual OperateResult SocketAcceptExtraCheck(Socket socket, IPEndPoint endPoint)
	{
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 服务器启动时额外的初始化信息，可以用于启动一些额外的服务的操作。
	/// The extra initialization information when the server starts can be used to start some additional service operations.
	/// </summary>
	/// <remarks>
	/// 需要在派生类中重写
	/// </remarks>
	protected virtual void StartInitialization()
	{
	}

	/// <summary>
	/// 指定端口号来启动服务器的引擎
	/// Specify the port number to start the server's engine
	/// </summary>
	/// <param name="port">指定一个端口号</param>
	/// <param name="modeTcp">是否使用TCP格式，如果需要UDP，则为 false</param>
	public virtual void ServerStart(int port, bool modeTcp = true)
	{
		if (!IsStarted)
		{
			IsStarted = true;
			ServerMode = ((!modeTcp) ? 1 : 0);
			StartInitialization();
			server.ServerStart(port, modeTcp);
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
		if (!IsStarted)
		{
			IsStarted = true;
			ServerMode = 2;
			StartInitialization();
			server.ServerStart(tcpPort, udpPort);
			udpServePort = udpPort;
		}
	}

	/// <summary>
	/// 使用已经配置好的端口启动服务器的引擎，并且使用TCP模式
	/// Use the configured port to start the server's engine
	/// </summary>
	public void ServerStart()
	{
		ServerStart(Port);
	}

	/// <summary>
	/// 服务器关闭的时候需要做的事情
	/// Things to do when the server is down
	/// </summary>
	protected virtual void CloseAction()
	{
	}

	/// <summary>
	/// 关闭服务器的引擎
	/// Shut down the server's engine
	/// </summary>
	public virtual void ServerClose()
	{
		if (IsStarted)
		{
			IsStarted = false;
			server.ServerClose();
			CloseAction();
		}
	}

	/// <summary>
	/// 关闭提供从机服务的串口对象
	/// Close the serial port object that provides slave services
	/// </summary>
	public void CloseSerialSlave()
	{
		server.CloseSerialSlave();
	}

	/// <param name="cert">证书对象</param>
	public void UseSSL(X509Certificate cert)
	{
		server.UseSSL(cert);
	}

	/// <summary>
	/// 使用SSL通信，传递一个证书的路径，以及证书的密码
	/// </summary>
	/// <param name="cert">传递一个证书</param>
	/// <param name="password">证书的密码</param>
	public void UseSSL(string cert, string password = "")
	{
		server.UseSSL(cert, password);
	}

	/// <summary>
	/// 设置并启动受信任的客户端登录并读写，如果为null，将关闭对客户端的ip验证
	/// Set and start the trusted client login and read and write, if it is null, the client's IP verification will be turned off
	/// </summary>
	/// <param name="clients">受信任的客户端列表</param>
	public void SetTrustedIpAddress(List<string> clients)
	{
		server.SetTrustedIpAddress(clients);
	}

	/// <summary>
	/// 获取受信任的客户端列表
	/// Get a list of trusted clients
	/// </summary>
	/// <returns>字符串数据信息</returns>
	public string[] GetTrustedClients()
	{
		return server.GetTrustedClients();
	}

	/// <summary>
	/// 将本系统的数据池数据存储到指定的文件
	/// Store the data pool data of this system to the specified file
	/// </summary>
	/// <param name="path">指定文件的路径</param>
	public void SaveDataPool(string path)
	{
		byte[] bytes = SaveToBytes();
		File.WriteAllBytes(path, bytes);
	}

	/// <summary>
	/// 从文件加载数据池信息
	/// Load datapool information from a file
	/// </summary>
	/// <param name="path">文件路径</param>
	public void LoadDataPool(string path)
	{
		if (File.Exists(path))
		{
			byte[] content = File.ReadAllBytes(path);
			LoadFromBytes(content);
		}
	}

	/// <summary>
	/// 从字节数据加载数据信息，需要进行重写方法
	/// Loading data information from byte data requires rewriting method
	/// </summary>
	/// <param name="content">字节数据</param>
	protected virtual void LoadFromBytes(byte[] content)
	{
	}

	/// <summary>
	/// 将数据信息存储到字节数组去，需要进行重写方法
	/// To store data information into a byte array, a rewrite method is required
	/// </summary>
	/// <returns>所有的内容</returns>
	protected virtual byte[] SaveToBytes()
	{
		return new byte[0];
	}

	/// <summary>
	/// 触发一个数据接收的事件信息
	/// Event information that triggers a data reception
	/// </summary>
	/// <param name="session">管道会话</param>
	/// <param name="receive">接收数据信息</param>
	protected void RaiseDataReceived(PipeSession session, byte[] receive)
	{
		OnDataReceived?.Invoke(this, session, receive);
	}

	/// <summary>
	/// 触发一个数据发送的事件信息
	/// Event information that triggers a data transmission
	/// </summary>
	/// <param name="send">数据内容</param>
	protected void RaiseDataSend(byte[] send)
	{
		OnDataSend?.Invoke(this, send);
	}

	private void Server_OnPipeMessageReceived(PipeSession session, byte[] buffer)
	{
		LogRevcMessage(buffer, session);
		OperateResult<byte[]> operateResult = null;
		try
		{
			operateResult = ReadFromCoreServer(session, buffer);
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException("ReadFromCoreServer", "Source Data: " + buffer.ToHexString(' '), ex);
			return;
		}
		if (!operateResult.IsSuccess)
		{
			if (operateResult.ErrorCode != int.MinValue)
			{
				base.LogNet?.WriteDebug(ToString(), $"<{session.Communication}> {operateResult.Message}");
			}
			if (operateResult.Content != null && operateResult.Content.Length != 0)
			{
				operateResult.IsSuccess = true;
			}
		}
		if (operateResult.IsSuccess && operateResult.Content != null && operateResult.Content.Length != 0)
		{
			byte[] content = operateResult.Content;
			ServerFaultAction serverFaultAction = FaultInjection.Process(session, ref content);
			if (serverFaultAction != ServerFaultAction.DropResponse)
			{
				session.Communication.Send(content);
				RaiseDataSend(content);
				LogSendMessage(content, session);
			}
			if (serverFaultAction == ServerFaultAction.Disconnect)
			{
				session.Close();
			}
		}
		RaiseDataReceived(session, buffer);
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
	protected virtual OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		return new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction);
	}

	/// <summary>
	/// 当客户端登录后，在Ip信息的过滤后，然后触发本方法，进行后续的数据接收，处理，并返回相关的数据信息
	/// When the client logs in, after filtering the IP information, this method is then triggered to perform subsequent data reception,
	/// processing, and return related data information
	/// </summary>
	/// <param name="session">管道信息</param>
	/// <param name="endPoint">终端节点</param>
	protected virtual OperateResult ThreadPoolLoginAfterClientCheck(PipeSession session, IPEndPoint endPoint)
	{
		return OperateResult.CreateSuccessResult();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			OnDataSend = null;
			OnDataReceived = null;
			ServerClose();
		}
		base.Dispose(disposing);
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
		return server.StartSerialSlave(com);
	}

	/// <summary>
	/// 启动串口的从机服务，使用默认的参数进行初始化串口，8位数据位，无奇偶校验，1位停止位
	/// Start the slave service of serial, initialize the serial port with default parameters, 8 data bits, no parity, 1 stop bit
	/// </summary>
	/// <param name="com">串口信息</param>
	/// <param name="baudRate">波特率</param>
	public OperateResult StartSerialSlave(string com, int baudRate)
	{
		return server.StartSerialSlave(com, baudRate);
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
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return server.StartSerialSlave(com, baudRate, dataBits, parity, stopBits);
	}

	/// <summary>
	/// 启动串口的从机服务，使用自定义的初始化方法初始化串口的参数
	/// Start the slave service of serial and initialize the parameters of the serial port using a custom initialization method
	/// </summary>
	/// <param name="inni">初始化信息的委托</param>
	public OperateResult StartSerialSlave(Action<SerialPort> inni)
	{
		return server.StartSerialSlave(inni);
	}

	/// <summary>
	/// 检查串口接收的数据是否完成的方法，如果接收完成，则返回True
	/// </summary>
	/// <param name="buffer">缓存的数据信息</param>
	/// <param name="receivedLength">当前已经接收的数据长度信息</param>
	/// <returns>是否接收完成</returns>
	protected virtual bool CheckSerialReceiveDataComplete(byte[] buffer, int receivedLength)
	{
		return false;
	}

	private void ThreadTimerHeartCheck(object obj)
	{
		server.RemoveSession(ActiveTimeSpan);
	}
}
