// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using HslCommunication.Core;

namespace HslCommunication.Enthernet.Ftp;

internal class PasvServer
{
	/// <summary>
	/// 缓存数据
	/// </summary>
	public byte[] buffer;

	private Socket socket;

	private Socket client;

	private int port;

	public Socket ClientSocket => client;

	/// <summary>
	/// 存储的文件流
	/// </summary>
	public FileStream SaveFileStream { get; set; }

	/// <summary>
	/// 操作是否完成
	/// </summary>
	public bool IsCompleted { get; set; } = false;

	public int Start(int portMin, int portMax)
	{
		for (int i = 0; i < 20; i++)
		{
			try
			{
				port = HslHelper.HslRandom.Next(portMin, portMax);
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Bind(new IPEndPoint(IPAddress.Any, port));
				socket.Listen(10);
				socket.BeginAccept(AsyncAcceptCallback, socket);
			}
			catch (Exception)
			{
				port = -1;
				continue;
			}
			break;
		}
		return port;
	}

	public void Close()
	{
		NetSupport.CloseSocket(socket);
		NetSupport.CloseSocket(client);
	}

	private void AsyncAcceptCallback(IAsyncResult iar)
	{
		if (iar.AsyncState is Socket socket)
		{
			try
			{
				client = socket.EndAccept(iar);
			}
			catch (ObjectDisposedException)
			{
				return;
			}
			catch (SocketException)
			{
			}
			catch (Exception)
			{
			}
			NetSupport.CloseSocket(socket);
		}
	}
}
