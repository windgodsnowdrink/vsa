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
using System.Threading.Tasks;
using HslCommunication.Core.IMessage;

namespace HslCommunication.Core.Pipe;

/// <summary>
/// 用于调试的远程管道信息
/// Remote pipeline information for debugging
/// </summary>
/// <remarks>
/// 当前的管道主要用于调试，DEMO界面测试时可以直接连接远程的Debug服务器，方便调试，屏蔽了握手报文
/// </remarks>
public class PipeDebugRemote : PipeTcpNet
{
	/// <summary>
	/// 获取或设置客户端的key信息，用于调试服务器区分不同的客户端，这样就支持多个客户端连接到同一个服务器，默认是0
	/// Get or set the client's key information, used to distinguish different clients in the debug server, so that multiple clients can connect to the same server, the default is 0
	/// </summary>
	public ushort ClientKey { get; set; } = 0;

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	/// <param name="serverMode">是否为服务器模式</param>
	public PipeDebugRemote()
	{
	}

	/// <summary>
	/// 通过指定的IP地址和端口号来实例化一个对象
	/// Instantiate an object with the specified IP address and port number
	/// </summary>
	/// <param name="ipAddress">IP地址信息</param>
	/// <param name="port">端口号</param>
	/// <param name="serverMode">是否为服务器模式</param>
	public PipeDebugRemote(string ipAddress, int port)
	{
		base.IpAddress = ipAddress;
		base.Port = port;
	}

	/// <summary>
	/// 通过指定的套接字及连接的终结点来实例化一个对象
	/// Instantiate an object by specifying the socket and the endpoint of the connection
	/// </summary>
	/// <param name="socket">连接的套接字对象</param>
	/// <param name="iPEndPoint">连接的远程地址信息</param>
	/// <param name="serverMode">是否为服务器模式</param>
	public PipeDebugRemote(Socket socket, IPEndPoint iPEndPoint)
	{
		base.Socket = socket;
		base.IpAddress = iPEndPoint.Address.ToString();
		base.Port = iPEndPoint.Port;
	}

	private byte[] CreateHeaderBytes(byte[] sendValue)
	{
		byte[] buffer = new byte[6];
		BitConverter.GetBytes(ClientKey).CopyTo(buffer, 0);
		BitConverter.GetBytes(sendValue.Length).CopyTo(buffer, 2);
		return buffer;
	}

	public override OperateResult<byte[]> ReadFromCoreServer(INetMessage netMessage, byte[] sendValue, bool hasResponseData, Action<byte[]> logMessage = null)
	{
		Send(CreateHeaderBytes(sendValue));
		return base.ReadFromCoreServer(netMessage, sendValue, hasResponseData, logMessage);
	}

	public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(INetMessage netMessage, byte[] sendValue, bool hasResponseData, Action<byte[]> logMessage = null)
	{
		await SendAsync(CreateHeaderBytes(sendValue));
		return await base.ReadFromCoreServerAsync(netMessage, sendValue, hasResponseData, logMessage);
	}

	public override string ToString()
	{
		return $"PipeTcpNet[{base.IpAddress}:{base.Port}]";
	}
}
