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
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core.Pipe;

namespace HslCommunication.Core.Net;

/// <summary>
/// 基于Udp的应答式通信类
/// Udp - based responsive communication class
/// </summary>
public class NetworkUdpBase : NetworkBase
{
	/// <summary>
	/// 设置日志记录报文是否二进制，如果为False，那就使用ASCII码
	/// Set whether the log message is binary, if it is False, then use ASCII code
	/// </summary>
	protected bool LogMsgFormatBinary = true;

	private int connectErrorCount = 0;

	private PipeSocket pipeSocket;

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

	/// <summary>
	/// 获取或设置接收服务器反馈的时间，如果为负数，则不接收反馈
	/// Gets or sets the time to receive server feedback, and if it is a negative number, does not receive feedback
	/// </summary>
	/// <remarks>
	/// 超时的通常原因是服务器端没有配置好，导致访问失败，为了不卡死软件，所以有了这个超时的属性。
	/// </remarks>
	public int ReceiveTimeout { get; set; }

	/// <summary>
	/// 当前连接的唯一ID号，默认为长度20的guid码加随机数组成，方便列表管理，也可以自己指定
	/// The unique ID number of the current connection. The default is a 20-digit guid code plus a random number.
	/// </summary>
	/// <remarks>
	/// 当前的唯一连接ID信息在DTU的模式下，将发挥关键作用，标记唯一的DTU ID
	/// </remarks>
	public string ConnectionId { get; set; }

	/// <summary>
	/// 获取或设置一次接收时的数据长度，默认2KB数据长度，特殊情况的时候需要调整
	/// Gets or sets the length of data received at a time. The default length is 2KB
	/// </summary>
	public int ReceiveCacheLength { get; set; } = 2048;

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
	/// 实例化一个默认的方法
	/// Instantiate a default method
	/// </summary>
	public NetworkUdpBase()
	{
		ReceiveTimeout = 5000;
		ConnectionId = SoftBasic.GetUniqueStringByGuidAndRandom();
		pipeSocket = new PipeSocket();
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
	/// 核心的数据交互读取，发数据发送到通道上去，然后从通道上接收返回的数据
	/// The core data is read interactively, the data is sent to the serial port, and the returned data is received from the serial port
	/// </summary>
	/// <param name="send">完整的报文内容</param>
	/// <param name="hasResponseData">是否有等待的数据返回，默认为 true</param>
	/// <param name="usePackAndUnpack">是否需要对命令重新打包，在重写方法后才会有影响</param>
	/// <returns>是否成功的结果对象</returns>
	public virtual OperateResult<byte[]> ReadFromCoreServer(byte[] send)
	{
		return ReadFromCoreServer(send, hasResponseData: true, usePackAndUnpack: true);
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
	/// 从UDP接收相关的数据信息，允许子类重写实现一些更加特殊的功能验证。
	/// </summary>
	/// <param name="socket">网络套接字</param>
	/// <param name="timeOut">超时时间</param>
	/// <param name="send">发送的报文信息</param>
	/// <returns>返回接收到的报文</returns>
	protected virtual byte[] ReceiveFromUdpSocket(Socket socket, int timeOut, byte[] send)
	{
		socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeOut);
		IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(IpAddress), Port);
		IPEndPoint iPEndPoint2 = new IPEndPoint((iPEndPoint.AddressFamily == AddressFamily.InterNetworkV6) ? IPAddress.IPv6Any : IPAddress.Any, 0);
		EndPoint remoteEP = iPEndPoint2;
		byte[] buffer = new byte[ReceiveCacheLength];
		int length = socket.ReceiveFrom(buffer, ref remoteEP);
		return buffer.SelectBegin(length);
	}

	/// <summary>
	/// 核心的数据交互读取，发数据发送到通道上去，然后从通道上接收返回的数据
	/// The core data is read interactively, the data is sent to the serial port, and the returned data is received from the serial port
	/// </summary>
	/// <param name="send">完整的报文内容</param>
	/// <param name="hasResponseData">是否有等待的数据返回，默认为 true</param>
	/// <param name="usePackAndUnpack">是否需要对命令重新打包，在重写方法后才会有影响</param>
	/// <returns>是否成功的结果对象</returns>
	public virtual OperateResult<byte[]> ReadFromCoreServer(byte[] send, bool hasResponseData, bool usePackAndUnpack)
	{
		if (!Authorization.nzugaydgwadawdibbas())
		{
			return new OperateResult<byte[]>(StringResources.Language.AuthorizationFailed);
		}
		byte[] array = (usePackAndUnpack ? PackCommandWithHeader(send) : send);
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.Send + " : " + (LogMsgFormatBinary ? SoftBasic.ByteToHexString(array) : Encoding.ASCII.GetString(array)));
		if (pipeSocket.LockingTick > HslHelper.LockLimit)
		{
			return new OperateResult<byte[]>(StringResources.Language.TooManyLock);
		}
		pipeSocket.PipeLockEnter();
		try
		{
			IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(IpAddress), Port);
			OperateResult<Socket> availableSocketAsync = GetAvailableSocketAsync(iPEndPoint);
			if (!availableSocketAsync.IsSuccess)
			{
				pipeSocket.PipeLockLeave();
				return OperateResult.CreateFailedResult<byte[]>(availableSocketAsync);
			}
			availableSocketAsync.Content.SendTo(array, array.Length, SocketFlags.None, iPEndPoint);
			if (ReceiveTimeout < 0)
			{
				pipeSocket.PipeLockLeave();
				return OperateResult.CreateSuccessResult(new byte[0]);
			}
			if (!hasResponseData)
			{
				pipeSocket.PipeLockLeave();
				return OperateResult.CreateSuccessResult(new byte[0]);
			}
			byte[] array2 = ReceiveFromUdpSocket(availableSocketAsync.Content, ReceiveTimeout, array);
			pipeSocket.PipeLockLeave();
			base.LogNet?.WriteDebug(ToString(), StringResources.Language.Receive + " : " + (LogMsgFormatBinary ? SoftBasic.ByteToHexString(array2) : Encoding.ASCII.GetString(array2)));
			connectErrorCount = 0;
			pipeSocket.IsSocketError = false;
			try
			{
				return usePackAndUnpack ? UnpackResponseContent(array, array2) : OperateResult.CreateSuccessResult(array2);
			}
			catch (Exception ex)
			{
				return new OperateResult<byte[]>("UnpackResponseContent failed: " + ex.Message);
			}
		}
		catch (Exception ex2)
		{
			pipeSocket.ChangePorts();
			pipeSocket.IsSocketError = true;
			if (connectErrorCount < 100000000)
			{
				connectErrorCount++;
			}
			pipeSocket.PipeLockLeave();
			return new OperateResult<byte[]>(-connectErrorCount, ex2.Message);
		}
	}

	/// <summary>
	/// 核心的数据交互读取，发数据发送到通道上去，然后从通道上接收返回的数据
	/// The core data is read interactively, the data is sent to the serial port, and the returned data is received from the serial port
	/// </summary>
	/// <param name="send">完整的报文内容</param>
	/// <param name="hasResponseData">是否有等待的数据返回，默认为 true</param>
	/// <param name="usePackAndUnpack">是否需要对命令重新打包，在重写方法后才会有影响</param>
	/// <returns>是否成功的结果对象</returns>
	public async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(byte[] value)
	{
		return await Task.Run(() => ReadFromCoreServer(value));
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
		return await NetSupport.ReadFromCoreServerAsync(send, ReadFromCoreServerAsync);
	}

	/// <summary>
	/// 对当前设备的IP地址进行PING的操作，返回PING的结果，正常来说，返回
	/// PING the IP address of the current device and return the PING result. Normally, it returns
	/// </summary>
	/// <returns>返回PING的结果</returns>
	public IPStatus IpAddressPing()
	{
		Ping ping = new Ping();
		return ping.Send(IpAddress).Status;
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

	private OperateResult<Socket> GetAvailableSocketAsync(IPEndPoint endPoint)
	{
		if (pipeSocket.IsConnectitonError())
		{
			OperateResult operateResult = null;
			Socket socket = null;
			try
			{
				pipeSocket.Socket?.Close();
				socket = new Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
				if (LocalBinding != null)
				{
					socket.Bind(LocalBinding);
				}
				pipeSocket.Socket = socket;
				operateResult = OperateResult.CreateSuccessResult();
			}
			catch (Exception ex)
			{
				if (socket != null)
				{
					NetSupport.CloseSocket(socket);
				}
				pipeSocket.IsSocketError = true;
				operateResult = new OperateResult(ex.Message);
			}
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

	public override string ToString()
	{
		return $"NetworkUdpBase[{IpAddress}:{Port}]";
	}
}
