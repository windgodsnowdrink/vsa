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
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using HslCommunication.Core.Pipe;
using HslCommunication.LogNet;

namespace HslCommunication.Core.Net;

/// <summary>
/// 仅仅包含 TCP 服务器
/// </summary>
public class CommunicationTcpServer
{
	private List<string> TrustedClients = null;

	private bool IsTrustedClientsOnly = false;

	private object lock_trusted_clients = new object();

	/// <summary>
	/// 核心的socket服务器
	/// </summary>
	protected Socket socketServer = null;

	private bool useSSL = false;

	private X509Certificate certificate;

	private Action<PipeSslNet> pipeSslNetAction = null;

	private AsyncCallback beginAcceptCallback = null;

	/// <summary>
	/// 服务器引擎是否启动
	/// Whether the server engine is started
	/// </summary>
	public bool IsStarted { get; protected set; }

	/// <summary>
	/// 获取或设置服务器的端口号，如果是设置，需要在服务器启动前设置完成，才能生效。
	/// Gets or sets the port number of the server. If it is set, it needs to be set before the server starts to take effect.
	/// </summary>
	/// <remarks>
	/// 需要在服务器启动之前设置为有效
	/// </remarks>
	public int Port { get; set; } = 10000;

	/// <summary>
	/// 获取或设置服务器是否支持IPv6的地址协议信息
	/// Get or set whether the server supports IPv6 address protocol information
	/// </summary>
	/// <remarks>
	/// 默认为 False，也就是不启动
	/// </remarks>
	public bool EnableIPv6 { get; set; }

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
	/// 组件的日志工具，支持日志记录，只要实例化后，当前网络的基本信息，就以等级进行输出
	/// The component's logging tool supports logging. As long as the instantiation of the basic network information, the output will be output at
	/// </summary>
	/// <remarks>
	/// 只要实例化即可以记录日志，实例化的对象需要实现接口  ，本组件提供了三个日志记录类，你可以实现基于   的对象。
	/// </remarks>
	public ILogNet LogNet { get; set; }

	/// <summary>
	/// 记录一些调试日志的委托，将会进行输出调试文本。
	/// The delegate that records some debug logs will output debug text.
	/// </summary>
	public Action<string> LogDebugMessage { get; set; }

	public bool IsUseSSL => useSSL;

	/// <summary>
	/// 获取或设置服务器绑定的本地IP地址，默认为空，使用本地所有可用的ip地址
	/// Obtain or set the local IP address bound to the server, which is empty by default, and use all available IP addresses locally
	/// </summary>
	public IPAddress LocalAddress { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public CommunicationTcpServer()
	{
		beginAcceptCallback = AsyncAcceptCallback;
	}

	/// <summary>
	/// 使用SSL通信，传递一个证书的对象
	/// </summary>
	/// <param name="cert">证书对象</param>
	public void UseSSL(X509Certificate cert)
	{
		useSSL = true;
		certificate = cert;
	}

	/// <summary>
	/// 使用SSL通信，传递一个证书的路径，以及证书的密码
	/// </summary>
	/// <param name="cert">传递一个证书</param>
	/// <param name="password">证书的密码</param>
	public void UseSSL(string cert, string password = "")
	{
		useSSL = true;
		certificate = (string.IsNullOrEmpty(password) ? new X509Certificate(cert) : new X509Certificate(cert, password));
	}

	/// <summary>
	/// 设置一个SSL的管道操作对象，在管道实例化之后，可以进行一些初始化的属性设置，例如自定义 SslProtocols 枚举
	/// </summary>
	/// <param name="action">设置方法</param>
	public void SetSslPipeAction(Action<PipeSslNet> action)
	{
		pipeSslNetAction = action;
	}

	/// <summary>
	/// 指定端口号来启动服务器的引擎
	/// Specify the port number to start the server's engine
	/// </summary>
	/// <param name="port">指定一个端口号</param>
	public void ServerStart(int port)
	{
		if (!IsStarted)
		{
			if (!EnableIPv6)
			{
				socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socketServer.Bind(new IPEndPoint(LocalAddress ?? IPAddress.Any, port));
			}
			else
			{
				socketServer = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
				socketServer.Bind(new IPEndPoint(LocalAddress ?? IPAddress.IPv6Any, port));
			}
			socketServer.Listen(500);
			socketServer.BeginAccept(beginAcceptCallback, socketServer);
			IsStarted = true;
			Port = port;
			ExtraOnStart();
			LogDebugMsg(StringResources.Language.NetEngineStart);
		}
	}

	/// <summary>
	/// 指定端口号来启动服务器的引擎
	/// Specify the port number to start the server's engine
	/// </summary>
	/// <param name="port">指定一个端口号</param>
	public void ServerStart()
	{
		ServerStart(Port);
	}

	/// <summary>
	/// 关闭服务器的引擎
	/// Shut down the server's engine
	/// </summary>
	public void ServerClose()
	{
		if (IsStarted)
		{
			IsStarted = false;
			ExtraOnClose();
			NetSupport.CloseSocket(socketServer);
			LogDebugMsg(StringResources.Language.NetEngineClose);
		}
	}

	/// <summary>
	/// 设置并启动受信任的客户端登录并读写，如果为null，将关闭对客户端的ip验证
	/// Set and start the trusted client login and read and write, if it is null, the client's IP verification will be turned off
	/// </summary>
	/// <param name="clients">受信任的客户端列表</param>
	public void SetTrustedIpAddress(List<string> clients)
	{
		lock (lock_trusted_clients)
		{
			if (clients != null && clients.Count > 0)
			{
				TrustedClients = clients.Select((string m) => HslHelper.GetIpAddressFromInput(m)).ToList();
				IsTrustedClientsOnly = true;
			}
			else
			{
				TrustedClients = new List<string>();
				IsTrustedClientsOnly = false;
			}
		}
	}

	/// <summary>
	/// 检查该Ip地址是否是受信任的
	/// Check if the IP address is trusted
	/// </summary>
	/// <param name="ipAddress">Ip地址信息</param>
	/// <returns>是受信任的返回True，否则返回False</returns>
	private bool CheckIpAddressTrusted(string ipAddress)
	{
		if (IsTrustedClientsOnly)
		{
			bool success = false;
			lock (lock_trusted_clients)
			{
				for (int i = 0; i < TrustedClients.Count; i++)
				{
					if (TrustedClients[i] == ipAddress)
					{
						success = true;
						break;
					}
				}
			}
			return success;
		}
		return false;
	}

	/// <summary>
	/// 获取受信任的客户端列表
	/// Get a list of trusted clients
	/// </summary>
	/// <returns>字符串数据信息</returns>
	public string[] GetTrustedClients()
	{
		string[] result = new string[0];
		lock (lock_trusted_clients)
		{
			if (TrustedClients != null)
			{
				result = TrustedClients.ToArray();
			}
		}
		return result;
	}

	private void AsyncAcceptCallback(IAsyncResult iar)
	{
		if (!(iar.AsyncState is Socket socket))
		{
			return;
		}
		Socket socket2 = null;
		try
		{
			socket2 = socket.EndAccept(iar);
			if (SocketKeepAliveTime > 0)
			{
				socket2.SetKeepAlive(SocketKeepAliveTime, SocketKeepAliveTime);
			}
			ThreadPool.QueueUserWorkItem(ThreadPoolLogin, socket2);
		}
		catch (ObjectDisposedException)
		{
			return;
		}
		catch (SocketException ex2)
		{
			if (!IsStarted && ex2.SocketErrorCode == SocketError.OperationAborted)
			{
				return;
			}
			NetSupport.CloseSocket(socket2);
			LogDebugMsg(StringResources.Language.SocketAcceptCallbackException + " " + ex2.Message);
		}
		catch (Exception ex3)
		{
			NetSupport.CloseSocket(socket2);
			LogDebugMsg(StringResources.Language.SocketAcceptCallbackException + " " + ex3.Message);
		}
		int num = 0;
		while (num < 3)
		{
			if (!IsStarted)
			{
				NetSupport.CloseSocket(socket2);
				return;
			}
			try
			{
				socket.BeginAccept(beginAcceptCallback, socket);
			}
			catch (ObjectDisposedException)
			{
				if (!IsStarted)
				{
					return;
				}
				continue;
			}
			catch (Exception ex5)
			{
				HslHelper.ThreadSleep(100);
				LogDebugMsg(StringResources.Language.SocketReAcceptCallbackException + " " + ex5.Message);
				num++;
				continue;
			}
			break;
		}
		if (num >= 3)
		{
			LogDebugMsg(StringResources.Language.SocketReAcceptCallbackException);
			throw new Exception(StringResources.Language.SocketReAcceptCallbackException);
		}
	}

	/// <summary>
	/// 当客户端连接到服务器，并听过额外的检查后，进行回调的方法
	/// Callback method when the client connects to the server and has heard additional checks
	/// </summary>
	/// <param name="pipe">socket对象</param>
	/// <param name="endPoint">远程的终结点</param>
	private void ThreadPoolLogin(object obj)
	{
		if (!(obj is Socket socket))
		{
			return;
		}
		IPEndPoint iPEndPoint = null;
		try
		{
			iPEndPoint = socket.RemoteEndPoint as IPEndPoint;
		}
		catch (ObjectDisposedException)
		{
			return;
		}
		catch (Exception ex2)
		{
			LogDebugMsg("Client RemoteEndPoint Get Failed: " + ex2.Message);
		}
		if (IsTrustedClientsOnly)
		{
			string ipAddress = ((iPEndPoint == null) ? string.Empty : iPEndPoint.Address.ToString());
			if (!CheckIpAddressTrusted(ipAddress))
			{
				LogDebugMsg(string.Format(StringResources.Language.ClientDisableLogin, iPEndPoint));
				NetSupport.CloseSocket(socket);
				return;
			}
		}
		OperateResult operateResult = SocketAcceptExtraCheck(socket, iPEndPoint);
		if (!operateResult.IsSuccess)
		{
			LogDebugMsg($"Client <{iPEndPoint}> SocketAcceptExtraCheck failed: {operateResult.Message}");
			NetSupport.CloseSocket(socket);
			return;
		}
		PipeTcpNet pipeTcpNet = null;
		if (useSSL)
		{
			PipeSslNet pipeSslNet = new PipeSslNet(socket, iPEndPoint, serverMode: true);
			pipeSslNet.Certificate = certificate;
			pipeSslNetAction?.Invoke(pipeSslNet);
			OperateResult<SslStream> operateResult2 = pipeSslNet.CreateSslStream(socket, createNew: true);
			if (!operateResult2.IsSuccess)
			{
				pipeSslNet.CloseCommunication();
				LogNet?.WriteDebug(ToString(), $"[{iPEndPoint}] WebScoket SSL Check Failed:" + operateResult2.Message);
				return;
			}
			pipeTcpNet = pipeSslNet;
		}
		else
		{
			pipeTcpNet = new PipeTcpNet(socket, iPEndPoint);
		}
		ThreadPoolLogin(pipeTcpNet, iPEndPoint);
	}

	/// <summary>
	/// 当客户端连接到服务器，并听过额外的检查后，进行回调的方法
	/// Callback method when the client connects to the server and has heard additional checks
	/// </summary>
	/// <param name="pipe">socket对象</param>
	/// <param name="endPoint">远程的终结点</param>
	protected virtual void ThreadPoolLogin(PipeTcpNet pipe, IPEndPoint endPoint)
	{
	}

	/// <summary>
	/// 关闭的时候额外执行的功能代码
	/// </summary>
	protected virtual void ExtraOnClose()
	{
	}

	/// <summary>
	/// 服务器启动的时候额外执行的功能代码
	/// </summary>
	protected virtual void ExtraOnStart()
	{
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
	/// 记录当前的日志信息
	/// </summary>
	/// <param name="message">消息文本</param>
	protected void LogDebugMsg(string message)
	{
		LogNet?.WriteDebug(ToString(), message);
		LogDebugMessage?.Invoke(message);
	}
}
