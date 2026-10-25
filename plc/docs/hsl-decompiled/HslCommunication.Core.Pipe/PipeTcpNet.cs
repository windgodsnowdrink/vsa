// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using HslCommunication.Core.IMessage;

namespace HslCommunication.Core.Pipe;

/// <summary>
/// 用于TCP/IP协议的传输管道信息
/// Transport pipe information of the IP protocol
/// </summary>
public class PipeTcpNet : CommunicationPipe
{
	/// <summary>
	/// 配置的远程主机的名称，有可能是网址，也可能是IP
	/// </summary>
	protected string host = "127.0.0.1";

	private string ipAddress = "127.0.0.1";

	private int[] _port = new int[1] { 2000 };

	private int indexPort = -1;

	private Socket socket;

	private int connectTimeOut = 10000;

	private int recvTimeOutTick = 0;

	private Func<IAsyncResult, int> socketEndMethod;

	/// <summary>
	/// 获取或设置绑定的本地的IP地址和端口号信息，如果端口设置为0，代表任何可用的端口
	/// Get or set the bound local IP address and port number information, if the port is set to 0, it means any available port
	/// </summary>
	/// <remarks>
	/// 默认为NULL, 也即是不绑定任何本地的IP及端口号信息，使用系统自动分配的方式。
	/// The default is NULL, which means that no local IP and port number information are bound, and the system automatically assigns it.
	/// </remarks>
	public IPEndPoint LocalBinding { get; set; }

	/// <summary>
	/// 获取或是设置远程服务器的IP地址，如果是本机测试，那么需要设置为127.0.0.1
	/// Get or set the IP address of the remote server. If it is a local test, then it needs to be set to 127.0.0.1
	/// </summary>
	/// <remarks>
	/// 最好实在初始化的时候进行指定，当使用短连接的时候，支持动态更改，切换；当使用长连接后，无法动态更改
	/// 支持使用域名的网址方式，例如：www.hslcommunication.cn
	/// </remarks>
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
	/// 配置的远程主机的名称，有可能是网址，也可能是IP
	/// </summary>
	public string Host => host;

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
	/// 获取或设置服务器的端口号，具体的值需要取决于对方的配置
	/// Gets or sets the port number of the server. The specific value depends on the configuration of the other party.
	/// </summary>
	/// <remarks>
	/// 最好实在初始化的时候进行指定，当使用短连接的时候，支持动态更改，切换；当使用长连接后，无法动态更改
	/// </remarks>
	public int Port
	{
		get
		{
			if (_port.Length == 1)
			{
				return _port[0];
			}
			int num = indexPort;
			if (num < 0 || num >= _port.Length)
			{
				num = 0;
			}
			return _port[num];
		}
		set
		{
			if (_port.Length == 1)
			{
				_port[0] = value;
				return;
			}
			int num = indexPort;
			if (num < 0 || num >= _port.Length)
			{
				num = 0;
			}
			_port[num] = value;
		}
	}

	/// <summary>
	/// 获取或设置当前的客户端用于服务器连接的套接字。
	/// Gets or sets the socket currently used by the client for server connection.
	/// </summary>
	public Socket Socket
	{
		get
		{
			return socket;
		}
		set
		{
			socket = value;
			if (socket != null)
			{
				socketEndMethod = socket.EndReceive;
			}
			else
			{
				socketEndMethod = null;
			}
		}
	}

	/// <summary>
	/// 获取或设置接收服务器反馈的时间，如果为负数，则不接收反馈
	/// Gets or sets the time to receive server feedback, and if it is a negative number, does not receive feedback
	/// </summary>
	/// <remarks>
	/// 超时的通常原因是服务器端没有配置好，导致访问失败，为了不卡死软件，所以有了这个超时的属性。
	/// </remarks>
	public int ConnectTimeOut
	{
		get
		{
			return connectTimeOut;
		}
		set
		{
			connectTimeOut = value;
		}
	}

	/// <summary>
	/// 当因为接收超时而关闭连接的时候，获取或设置超过指定次数的时候，是否需要重新连接
	/// When the connection is closed due to a reception timeout, and when the number of retrieves or Settings exceeds the specified limit, is it necessary to reconnect
	/// </summary>
	/// <remarks>
	/// 需要注意的是，当异步方法通信的时候，是不支持的，异步方法会自动处理连接的关闭和重新连接，如果小于0，则接收数据超时的次数无论多少，都不再关闭连接
	/// </remarks>
	public int CloseOnRecvTimeOutTick { get; set; } = 1;

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	/// <param name="serverMode">是否为服务器模式</param>
	public PipeTcpNet()
	{
	}

	/// <summary>
	/// 通过指定的IP地址和端口号来实例化一个对象
	/// Instantiate an object with the specified IP address and port number
	/// </summary>
	/// <param name="ipAddress">IP地址信息</param>
	/// <param name="port">端口号</param>
	/// <param name="serverMode">是否为服务器模式</param>
	public PipeTcpNet(string ipAddress, int port)
	{
		IpAddress = ipAddress;
		Port = port;
	}

	/// <summary>
	/// 通过指定的套接字及连接的终结点来实例化一个对象
	/// Instantiate an object by specifying the socket and the endpoint of the connection
	/// </summary>
	/// <param name="socket">连接的套接字对象</param>
	/// <param name="iPEndPoint">连接的远程地址信息</param>
	/// <param name="serverMode">是否为服务器模式</param>
	public PipeTcpNet(Socket socket, IPEndPoint iPEndPoint)
	{
		Socket = socket;
		if (iPEndPoint != null)
		{
			IpAddress = iPEndPoint.Address.ToString();
			Port = iPEndPoint.Port;
		}
	}

	/// <summary>
	/// 设置多个可选的端口号信息，例如在三菱的PLC里，支持配置多个端口号，当一个网络发生异常时，立即切换端口号连接读写，提升系统的稳定性
	/// Set multiple optional port number information. For example, in Mitsubishi PLC, it supports to configure multiple port numbers.
	/// When an abnormality occurs in a network, the port number is immediately switched to connect to read and write to improve the stability of the system.
	/// </summary>
	/// <param name="ports">端口号数组信息</param>
	public void SetMultiPorts(int[] ports)
	{
		if (ports != null && ports.Length != 0)
		{
			_port = ports;
			indexPort = -1;
		}
	}

	/// <summary>
	/// 获取当前的远程连接信息，如果端口号设置了可选的数组，那么每次获取对象就会发生端口号切换的操作。
	/// Get the current remote connection information. If the port number is set to an optional array, the port number switching operation will occur every time the object is obtained.
	/// </summary>
	/// <returns>远程连接的对象</returns>
	public IPEndPoint GetConnectIPEndPoint()
	{
		if (_port.Length == 1)
		{
			return new IPEndPoint(IPAddress.Parse(IpAddress), _port[0]);
		}
		ChangePorts();
		int port = _port[indexPort];
		return new IPEndPoint(IPAddress.Parse(IpAddress), port);
	}

	/// <summary>
	/// 变更当前的端口号信息，如果设置了多个端口号的话，就切换其他可用的端口
	/// Change the current port number information, and if multiple port numbers are set, switch to other available ports
	/// </summary>
	private void ChangePorts()
	{
		if (_port.Length != 1)
		{
			if (indexPort < _port.Length - 1)
			{
				indexPort++;
			}
			else
			{
				indexPort = 0;
			}
		}
	}

	public override bool IsConnectError()
	{
		if (socket == null)
		{
			return true;
		}
		return base.IsConnectError();
	}

	/// <summary>
	/// 当管道打开成功的时候执行的事件，如果返回失败，则管道的打开操作返回失败
	/// </summary>
	/// <param name="socket">通信的套接字</param>
	/// <returns>是否真的打开成功</returns>
	protected virtual OperateResult OnCommunicationOpen(Socket socket)
	{
		return OperateResult.CreateSuccessResult();
	}

	public override OperateResult<bool> OpenCommunication()
	{
		if (IsConnectError())
		{
			NetSupport.CloseSocket(socket);
			Socket = null;
			IPEndPoint connectIPEndPoint = GetConnectIPEndPoint();
			OperateResult<Socket> operateResult = NetSupport.CreateSocketAndConnect(connectIPEndPoint, ConnectTimeOut, LocalBinding);
			if (operateResult.IsSuccess)
			{
				OperateResult operateResult2 = OnCommunicationOpen(operateResult.Content);
				if (!operateResult2.IsSuccess)
				{
					return operateResult2.ConvertFailed<bool>();
				}
				if (SocketKeepAliveTime > 0)
				{
					operateResult.Content.SetKeepAlive(SocketKeepAliveTime, SocketKeepAliveTime);
				}
				Socket = operateResult.Content;
				ResetConnectErrorCount();
				return OperateResult.CreateSuccessResult(value: true);
			}
			return new OperateResult<bool>(-IncrConnectErrorCount(), operateResult.Message);
		}
		return OperateResult.CreateSuccessResult(value: false);
	}

	public override OperateResult CloseCommunication()
	{
		NetSupport.CloseSocket(socket);
		Socket = null;
		return OperateResult.CreateSuccessResult();
	}

	public override OperateResult Send(byte[] data, int offset, int size)
	{
		OperateResult operateResult = NetSupport.SocketSend(socket, data, offset, size);
		if (!operateResult.IsSuccess && operateResult.ErrorCode == NetSupport.SocketErrorCode)
		{
			CloseCommunication();
			return new OperateResult<byte[]>(-IncrConnectErrorCount(), operateResult.Message);
		}
		return operateResult;
	}

	public override OperateResult<int> Receive(byte[] buffer, int offset, int length, int timeOut = 60000, Action<long, long> reportProgress = null)
	{
		OperateResult<int> operateResult = NetSupport.SocketReceive(socket, buffer, offset, length, timeOut, reportProgress);
		if (!operateResult.IsSuccess && operateResult.ErrorCode == NetSupport.SocketErrorCode)
		{
			if (operateResult.Content == int.MaxValue)
			{
				if (CloseOnRecvTimeOutTick < 0 || Interlocked.Increment(ref recvTimeOutTick) < CloseOnRecvTimeOutTick)
				{
					return new OperateResult<int>(-1, operateResult.Message);
				}
				CloseCommunication();
				Interlocked.Exchange(ref recvTimeOutTick, 0);
			}
			else
			{
				CloseCommunication();
			}
			return new OperateResult<int>(-IncrConnectErrorCount(), operateResult.Message);
		}
		if (operateResult.IsSuccess)
		{
			Interlocked.Exchange(ref recvTimeOutTick, 0);
		}
		return operateResult;
	}

	public override OperateResult StartReceiveBackground(INetMessage netMessage)
	{
		if (base.UseServerActivePush)
		{
			OperateResult operateResult = socket.BeginReceiveResult(ServerSocketActivePushAsync, netMessage);
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
		}
		return base.StartReceiveBackground(netMessage);
	}

	private async void ServerSocketActivePushAsync(IAsyncResult ar)
	{
		object asyncState = ar.AsyncState;
		if (!(asyncState is INetMessage netMessage))
		{
			return;
		}
		OperateResult<int> endResult = socket.EndReceiveResult(ar);
		if (!endResult.IsSuccess)
		{
			IncrConnectErrorCount();
			return;
		}
		OperateResult<byte[]> receive = await ReceiveMessageAsync(netMessage, null, useActivePush: false).ConfigureAwait(continueOnCapturedContext: false);
		if (!receive.IsSuccess)
		{
			IncrConnectErrorCount();
			return;
		}
		if (base.DecideWhetherQAMessageFunction != null)
		{
			if (base.DecideWhetherQAMessageFunction(this, receive))
			{
				SetBufferQA(receive.Content);
			}
		}
		else
		{
			SetBufferQA(receive.Content);
		}
		OperateResult receiveAgain = socket.BeginReceiveResult(ServerSocketActivePushAsync, netMessage);
		if (!receiveAgain.IsSuccess)
		{
			IncrConnectErrorCount();
		}
	}

	public override async Task<OperateResult<bool>> OpenCommunicationAsync()
	{
		if (IsConnectError())
		{
			NetSupport.CloseSocket(socket);
			Socket = null;
			IPEndPoint endPoint = GetConnectIPEndPoint();
			OperateResult<Socket> connect = await NetSupport.CreateSocketAndConnectAsync(endPoint, ConnectTimeOut, LocalBinding).ConfigureAwait(continueOnCapturedContext: false);
			if (connect.IsSuccess)
			{
				OperateResult onOpen = OnCommunicationOpen(connect.Content);
				if (!onOpen.IsSuccess)
				{
					return onOpen.ConvertFailed<bool>();
				}
				if (SocketKeepAliveTime > 0)
				{
					connect.Content.SetKeepAlive(SocketKeepAliveTime, SocketKeepAliveTime);
				}
				Socket = connect.Content;
				ResetConnectErrorCount();
				return OperateResult.CreateSuccessResult(value: true);
			}
			return new OperateResult<bool>(-IncrConnectErrorCount(), connect.Message);
		}
		return OperateResult.CreateSuccessResult(value: false);
	}

	public override async Task<OperateResult> CloseCommunicationAsync()
	{
		NetSupport.CloseSocket(socket);
		Socket = null;
		return await Task.FromResult(OperateResult.CreateSuccessResult());
	}

	public override async Task<OperateResult> SendAsync(byte[] data, int offset, int size)
	{
		OperateResult send = await NetSupport.SocketSendAsync(socket, data, offset, size).ConfigureAwait(continueOnCapturedContext: false);
		if (!send.IsSuccess && send.ErrorCode == NetSupport.SocketErrorCode)
		{
			await CloseCommunicationAsync().ConfigureAwait(continueOnCapturedContext: false);
			return new OperateResult<byte[]>(-IncrConnectErrorCount(), send.Message);
		}
		return send;
	}

	public override async Task<OperateResult<int>> ReceiveAsync(byte[] buffer, int offset, int length, int timeOut = 60000, Action<long, long> reportProgress = null)
	{
		Func<IAsyncResult, int> endMethodTmp = socketEndMethod;
		if (endMethodTmp == null && socket != null)
		{
			endMethodTmp = socket.EndReceive;
		}
		OperateResult<int> receive = await NetSupport.SocketReceiveAsync(socket, buffer, offset, length, timeOut, reportProgress, endMethodTmp).ConfigureAwait(continueOnCapturedContext: false);
		if (!receive.IsSuccess && receive.ErrorCode == NetSupport.SocketErrorCode)
		{
			await CloseCommunicationAsync().ConfigureAwait(continueOnCapturedContext: false);
			return new OperateResult<int>(-IncrConnectErrorCount(), receive.Message);
		}
		return receive;
	}

	public override string ToString()
	{
		return $"PipeTcpNet[{ipAddress}:{Port}]";
	}
}
