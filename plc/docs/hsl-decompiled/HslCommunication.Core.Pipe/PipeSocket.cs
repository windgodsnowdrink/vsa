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

namespace HslCommunication.Core.Pipe;

/// <summary>
/// 基于网络通信的管道信息，可以设置额外的一些参数信息，例如连接超时时间，读取超时时间等等。
/// Based on the pipe information of network communication, some additional parameter information can be set, such as connection timeout time, read timeout time and so on.
/// </summary>
public class PipeSocket : PipeBase, IDisposable
{
	private string ipAddress = "127.0.0.1";

	private int[] _port = new int[1] { 2000 };

	private int indexPort = -1;

	private Socket socket;

	private int receiveTimeOut = 5000;

	private int connectTimeOut = 10000;

	private int sleepTime = 0;

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
			ipAddress = HslHelper.GetIpAddressFromInput(value);
		}
	}

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
	/// 指示长连接的套接字是否处于错误的状态
	/// Indicates if the long-connected socket is in the wrong state
	/// </summary>
	public bool IsSocketError { get; set; }

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
			return receiveTimeOut;
		}
		set
		{
			receiveTimeOut = value;
		}
	}

	/// <summary>
	/// 获取或设置在正式接收对方返回数据前的时候，需要休息的时间，当设置为0的时候，不需要休息。
	/// Get or set the time required to rest before officially receiving the data from the other party. When it is set to 0, no rest is required.
	/// </summary>
	public int SleepTime
	{
		get
		{
			return sleepTime;
		}
		set
		{
			sleepTime = value;
		}
	}

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public PipeSocket()
	{
	}

	/// <summary>
	/// 通过指定的IP地址和端口号来实例化一个对象
	/// Instantiate an object with the specified IP address and port number
	/// </summary>
	/// <param name="ipAddress">IP地址信息</param>
	/// <param name="port">端口号</param>
	public PipeSocket(string ipAddress, int port)
	{
		this.ipAddress = ipAddress;
		_port = new int[1] { port };
	}

	/// <summary>
	/// 获取当前的连接状态是否发生了异常，如果发生了异常，返回 False
	/// Gets whether an exception has occurred in the current connection state, and returns False if an exception has occurred
	/// </summary>
	/// <returns>如果有异常，返回 True, 否则返回 False</returns>
	public bool IsConnectitonError()
	{
		return IsSocketError || socket == null;
	}

	public override void Dispose()
	{
		base.Dispose();
		socket?.Close();
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
	public void ChangePorts()
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

	public override string ToString()
	{
		return $"PipeSocket[{ipAddress}:{Port}]";
	}
}
