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

namespace HslCommunication.Core.Net;

/// <summary>
/// 会话的基类信息
/// </summary>
public abstract class SessionBase
{
	/// <summary>
	/// 实际传输数据的对象，除非你清楚自己在收发什么数据信息，否则尽量不直接使用本对象
	/// The actual data transmission object, unless you know what data information you are sending and receiving, try not to directly use this  object
	/// </summary>
	public Socket WorkSocket { get; private set; }

	/// <summary>
	/// 获取当前的客户端的上线时间
	/// Get the online time of the current client
	/// </summary>
	public DateTime OnlineTime { get; protected set; }

	/// <summary>
	/// 获取当前的远程连接对象的IP地址
	/// Get the IP address of the current remote connection object
	/// </summary>
	public string IpAddress { get; protected set; }

	/// <summary>
	/// 获取当前的连接对象连接的远程客户端
	/// Get the remote client connected by the current connection object
	/// </summary>
	public IPEndPoint IpEndPoint { get; protected set; }

	/// <summary>
	/// 获取心跳验证的时间点
	/// Get the time point of heartbeat verification
	/// </summary>
	public DateTime HeartTime { get; private set; } = DateTime.Now;

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public SessionBase()
	{
		OnlineTime = DateTime.Now;
	}

	/// <summary>
	/// 通过指定的对象来初始化一个会话内容
	/// </summary>
	/// <param name="socket">连接的Socket对象</param>
	public SessionBase(Socket socket)
		: this()
	{
		UpdateSocket(socket);
	}

	/// <summary>
	/// 更新当前的心跳时间
	/// Update the current heartbeat time
	/// </summary>
	public void UpdateHeartTime()
	{
		HeartTime = DateTime.Now;
	}

	/// <summary>
	/// 更新当前的连接对象信息
	/// </summary>
	/// <param name="socket">连接的对象</param>
	public void UpdateSocket(Socket socket)
	{
		if (socket == null)
		{
			return;
		}
		WorkSocket = socket;
		try
		{
			IpEndPoint = WorkSocket.RemoteEndPoint as IPEndPoint;
			IpAddress = ((IpEndPoint == null) ? string.Empty : IpEndPoint.Address.ToString());
		}
		catch
		{
		}
	}
}
