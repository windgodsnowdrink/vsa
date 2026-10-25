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
using System.Text;
using HslCommunication.Core.Net;

namespace HslCommunication.Enthernet;

/// <summary>
/// Udp网络的服务器端类，您可以使用本类构建一个简单的，高性能的udp服务器，接收来自其他客户端的数据，当然，您也可以自定义返回你要返回的数据
/// Server-side class of Udp network. You can use this class to build a simple, high-performance udp server that receives data from other clients. Of course, you can also customize the data you want to return.
/// </summary>
public class NetUdpServer : NetworkServerBase
{
	/// <summary>
	/// 获取或设置一次接收时的数据长度，默认2KB数据长度
	/// </summary>
	public int ReceiveCacheLength { get; set; } = 2048;

	/// <summary>
	/// 当接收到文本数据的时候,触发此事件
	/// </summary>
	public event Action<AppSession, NetHandle, string> AcceptString;

	/// <summary>
	/// 当接收到字节数据的时候,触发此事件
	/// </summary>
	public event Action<AppSession, NetHandle, byte[]> AcceptByte;

	public override void ServerStart(int port)
	{
		if (!base.IsStarted)
		{
			CoreSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			CoreSocket.Bind(new IPEndPoint(IPAddress.Any, port));
			RefreshReceive();
			base.LogNet?.WriteInfo(ToString(), StringResources.Language.NetEngineStart);
			base.IsStarted = true;
		}
	}

	protected override void CloseAction()
	{
		AcceptString = null;
		AcceptByte = null;
		base.CloseAction();
	}

	/// <summary>
	/// 重新开始接收数据
	/// </summary>
	private void RefreshReceive()
	{
		AppSession appSession = new AppSession(CoreSocket);
		appSession.UdpEndPoint = new IPEndPoint(IPAddress.Any, 0);
		appSession.BytesBuffer = new byte[ReceiveCacheLength];
		CoreSocket.BeginReceiveFrom(appSession.BytesBuffer, 0, ReceiveCacheLength, SocketFlags.None, ref appSession.UdpEndPoint, AsyncCallback, appSession);
	}

	private void AsyncCallback(IAsyncResult ar)
	{
		if (!(ar.AsyncState is AppSession appSession))
		{
			return;
		}
		try
		{
			int num = appSession.WorkSocket.EndReceiveFrom(ar, ref appSession.UdpEndPoint);
			RefreshReceive();
			if (num >= 32)
			{
				if (CheckRemoteToken(appSession.BytesBuffer))
				{
					int num2 = BitConverter.ToInt32(appSession.BytesBuffer, 28);
					if (num2 == num - 32)
					{
						byte[] buffer = new byte[32];
						byte[] buffer2 = new byte[num2];
						Array.Copy(appSession.BytesBuffer, 0, buffer, 0, 32);
						if (num2 > 0)
						{
							Array.Copy(appSession.BytesBuffer, 32, buffer2, 0, num2);
						}
						buffer2 = HslProtocol.CommandAnalysis(buffer, buffer2);
						int protocol = BitConverter.ToInt32(buffer, 0);
						int customer = BitConverter.ToInt32(buffer, 4);
						DataProcessingCenter(appSession, protocol, customer, buffer2);
					}
					else
					{
						base.LogNet?.WriteWarn(ToString(), $"Should Rece：{BitConverter.ToInt32(appSession.BytesBuffer, 4) + 8} Actual：{num}");
					}
				}
				else
				{
					base.LogNet?.WriteWarn(ToString(), StringResources.Language.TokenCheckFailed);
				}
			}
			else
			{
				base.LogNet?.WriteWarn(ToString(), $"Receive error, Actual：{num}");
			}
		}
		catch (ObjectDisposedException)
		{
		}
		catch (Exception ex2)
		{
			base.LogNet?.WriteException(ToString(), StringResources.Language.SocketEndReceiveException, ex2);
			RefreshReceive();
		}
		finally
		{
		}
	}

	/// <summary>
	/// 数据处理中心
	/// </summary>
	/// <param name="session">会话信息</param>
	/// <param name="protocol">暗号</param>
	private void DataProcessingCenter(AppSession session, int protocol, int customer, byte[] content)
	{
		switch (protocol)
		{
		case 1002:
			AcceptByte?.Invoke(session, customer, content);
			break;
		case 1001:
		{
			string arg = Encoding.Unicode.GetString(content);
			AcceptString?.Invoke(session, customer, arg);
			break;
		}
		}
	}

	/// <summary>
	/// 向指定的通信对象发送字符串数据
	/// </summary>
	/// <param name="session">通信对象</param>
	/// <param name="customer">用户的指令头</param>
	/// <param name="str">实际发送的字符串数据</param>
	public void SendMessage(AppSession session, int customer, string str)
	{
		SendBytesAsync(session, HslProtocol.CommandBytes(customer, base.Token, str));
	}

	/// <summary>
	/// 向指定的通信对象发送字节数据
	/// </summary>
	/// <param name="session">连接对象</param>
	/// <param name="customer">用户的指令头</param>
	/// <param name="bytes">实际的数据</param>
	public void SendMessage(AppSession session, int customer, byte[] bytes)
	{
		SendBytesAsync(session, HslProtocol.CommandBytes(customer, base.Token, bytes));
	}

	private void SendBytesAsync(AppSession session, byte[] data)
	{
		try
		{
			session.WorkSocket.SendTo(data, data.Length, SocketFlags.None, session.UdpEndPoint);
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException("SendMessage", ex);
		}
	}

	public override string ToString()
	{
		return $"NetUdpServer[{base.Port}]";
	}
}
