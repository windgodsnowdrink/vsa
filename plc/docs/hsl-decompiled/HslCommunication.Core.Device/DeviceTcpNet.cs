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
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using HslCommunication.Core.Pipe;
using HslCommunication.Reflection;

namespace HslCommunication.Core.Device;

/// <summary>
/// 基于TCP管道的设备基类信息
/// </summary>
public class DeviceTcpNet : DeviceCommunication
{
	private PipeTcpNet pipeTcpNet;

	private Lazy<Ping> ping = new Lazy<Ping>(() => new Ping());

	[HslMqttApi(HttpMethod = "GET", Description = "Gets or sets the timeout for the connection, in milliseconds")]
	/// <summary>
	/// 获取或设置接收服务器反馈的时间，如果为负数，则不接收反馈
	/// Gets or sets the time to receive server feedback, and if it is a negative number, does not receive feedback
	/// </summary>
	/// <remarks>
	/// 超时的通常原因是服务器端没有配置好，导致访问失败，为了不卡死软件，所以有了这个超时的属性。
	/// </remarks>
	public virtual int ConnectTimeOut
	{
		get
		{
			if (!(CommunicationPipe is PipeTcpNet { ConnectTimeOut: var connectTimeOut }))
			{
				return this.pipeTcpNet.ConnectTimeOut;
			}
			return connectTimeOut;
		}
		set
		{
			if (value >= 0 && CommunicationPipe is PipeTcpNet pipeTcpNet)
			{
				pipeTcpNet.ConnectTimeOut = value;
			}
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
			if (!(CommunicationPipe is PipeTcpNet { IpAddress: var ipAddress }))
			{
				return this.pipeTcpNet.IpAddress;
			}
			return ipAddress;
		}
		set
		{
			if (CommunicationPipe is PipeTcpNet pipeTcpNet)
			{
				pipeTcpNet.IpAddress = value;
			}
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
			if (!(CommunicationPipe is PipeTcpNet { Port: var port }))
			{
				return this.pipeTcpNet.Port;
			}
			return port;
		}
		set
		{
			if (CommunicationPipe is PipeTcpNet pipeTcpNet)
			{
				pipeTcpNet.Port = value;
			}
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
			if (!(CommunicationPipe is PipeTcpNet { LocalBinding: var localBinding }))
			{
				return this.pipeTcpNet.LocalBinding;
			}
			return localBinding;
		}
		set
		{
			if (CommunicationPipe is PipeTcpNet pipeTcpNet)
			{
				pipeTcpNet.LocalBinding = value;
			}
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
			if (!(CommunicationPipe is PipeTcpNet { SocketKeepAliveTime: var socketKeepAliveTime }))
			{
				return this.pipeTcpNet.SocketKeepAliveTime;
			}
			return socketKeepAliveTime;
		}
		set
		{
			if (CommunicationPipe is PipeTcpNet pipeTcpNet)
			{
				pipeTcpNet.SocketKeepAliveTime = value;
			}
		}
	}

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public DeviceTcpNet()
		: this("127.0.0.1", 5000)
	{
	}

	/// <summary>
	/// 指定IP地址以及端口号信息来初始化对象
	/// </summary>
	/// <param name="ipAddress">IP地址信息，可以是IPv4, IPv6, 也可以是域名</param>
	/// <param name="port">设备方的端口号信息</param>
	public DeviceTcpNet(string ipAddress, int port)
	{
		pipeTcpNet = new PipeTcpNet();
		pipeTcpNet.IpAddress = ipAddress;
		pipeTcpNet.Port = port;
		CommunicationPipe = pipeTcpNet;
	}

	[Obsolete]
	/// <summary>
	/// V11版本及之前设置长连接的方法，在V12版本以上中没有任何效果，默认长连接，删除调用即可，此处保留方法是为了部分用户保持兼容性升级。
	/// The method of setting the long connection in V11 and before, has no effect in V12 and above. this method can be deleted.
	/// The method is retained here to maintain compatibility upgrades for some users.
	/// </summary>
	public void SetPersistentConnection()
	{
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
	public virtual OperateResult ConnectServer()
	{
		CommunicationPipe?.CloseCommunication();
		OperateResult<bool> operateResult = CommunicationPipe.OpenCommunication();
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.ConnectedSuccess);
		if (operateResult.Content && CommunicationPipe.GetType() != typeof(PipeDebugRemote))
		{
			return InitializationOnConnect();
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 尝试连接远程的服务器，如果连接成功，就切换短连接模式到长连接模式，后面的每次请求都共享一个通道，使得通讯速度更快速
	/// Try to connect to a remote server. If the connection is successful, switch the short connection mode to the long connection mode.
	/// Each subsequent request will share a channel, making the communication speed faster.
	/// </summary>
	/// <returns>返回连接结果，如果失败的话（也即IsSuccess为False），包含失败信息</returns>
	public virtual async Task<OperateResult> ConnectServerAsync()
	{
		await CommunicationPipe.CloseCommunicationAsync().ConfigureAwait(continueOnCapturedContext: false);
		OperateResult<bool> open = await CommunicationPipe.OpenCommunicationAsync().ConfigureAwait(continueOnCapturedContext: false);
		if (!open.IsSuccess)
		{
			return open;
		}
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.ConnectedSuccess);
		if (open.Content && CommunicationPipe.GetType() != typeof(PipeDebugRemote))
		{
			return await InitializationOnConnectAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 手动断开与远程服务器的连接，如果当前是长连接模式，那么就会切换到短连接模式
	/// Manually disconnect from the remote server, if it is currently in long connection mode, it will switch to short connection mode
	/// </summary>
	/// <returns>关闭连接，不需要查看IsSuccess属性查看</returns>
	public virtual OperateResult ConnectClose()
	{
		OperateResult operateResult = ExtraOnDisconnect();
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.Close);
		return CommunicationPipe.CloseCommunication();
	}

	/// <summary>
	/// 手动断开与远程服务器的连接，如果当前是长连接模式，那么就会切换到短连接模式
	/// Manually disconnect from the remote server, if it is currently in long connection mode, it will switch to short connection mode
	/// </summary>
	/// <returns>关闭连接，不需要查看IsSuccess属性查看</returns>
	public virtual async Task<OperateResult> ConnectCloseAsync()
	{
		OperateResult result = await ExtraOnDisconnectAsync().ConfigureAwait(continueOnCapturedContext: false);
		if (!result.IsSuccess)
		{
			return result;
		}
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.Close);
		return await CommunicationPipe.CloseCommunicationAsync().ConfigureAwait(continueOnCapturedContext: false);
	}

	public override string ToString()
	{
		return $"DeviceTcpNet<{base.ByteTransform}>{{{CommunicationPipe}}}";
	}
}
