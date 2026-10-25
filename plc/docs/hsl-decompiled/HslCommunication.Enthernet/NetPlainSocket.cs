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
using System.Threading;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Core.Pipe;

namespace HslCommunication.Enthernet;

/// <summary>
/// 一个基于字节流的Socket通讯类，支持自动重连功能，接收数据以字符串或是字节数组的形式返回
/// A byte stream-based Socket communication class that supports automatic reconnection functionality, and the received data is returned in the form of strings or byte arrays
/// </summary>
public class NetPlainSocket : NetworkXBase
{
	/// <summary>
	/// 获取或设置接收服务器反馈的时间，如果为负数，则不接收反馈
	/// Gets or sets the time to receive server feedback, and if it is a negative number, does not receive feedback
	/// </summary>
	/// <remarks>
	/// 超时的通常原因是服务器端没有配置好，导致访问失败，为了不卡死软件，所以有了这个超时的属性。
	/// </remarks>
	public int ConnectTimeOut = 5000;

	private Encoding encoding;

	private object connectLock = new object();

	private string ipAddress = "127.0.0.1";

	private int port = 10000;

	private int bufferLength = 2048;

	private byte[] buffer = null;

	private AsyncCallback socketAsyncCallBack = null;

	private WaitCallback reConnectCallBack = null;

	private int reconnectTick = 0;

	private PipeTcpNet pipeTcpNet = null;

	/// <summary>
	/// 获取或设置当前的字符串编码格式
	/// Get or set the current string encoding format
	/// </summary>
	public Encoding Encoding
	{
		get
		{
			return encoding;
		}
		set
		{
			encoding = value;
		}
	}

	/// <summary>
	/// 获取或设置当前的缓存区的大小信息，默认2048字节
	/// Get or set the size information of the current cache, with a default of 2048 bytes
	/// </summary>
	public int SocketBufferLength
	{
		get
		{
			return bufferLength;
		}
		set
		{
			if (value <= 0)
			{
				value = 2048;
			}
			bufferLength = value;
			buffer = new byte[bufferLength];
		}
	}

	/// <summary>
	/// 获取一个新的消息对象的方法，需要在继承类里面进行重写
	/// The method to get a new message object needs to be overridden in the inheritance class
	/// </summary>
	/// <returns>消息类对象</returns>
	public Func<INetMessage> CreateNewMessage { get; set; }

	/// <summary>
	/// 当接收到字符串时候的触发事件
	/// The trigger event when a string is received
	/// </summary>
	public event Action<string> ReceivedString;

	/// <summary>
	/// 当接收到字节数据时候的触发事件
	/// Triggered when byte data is received
	/// </summary>
	public event Action<byte[]> ReceivedData;

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public NetPlainSocket()
	{
		buffer = new byte[bufferLength];
		encoding = Encoding.UTF8;
		socketAsyncCallBack = ReceiveCallBack;
		reConnectCallBack = ReConnectServer;
	}

	/// <summary>
	/// 使用指定的ip地址和端口号来实例化这个对象
	/// </summary>
	/// <param name="ipAddress">Ip地址</param>
	/// <param name="port">端口号</param>
	public NetPlainSocket(string ipAddress, int port)
		: this()
	{
		this.ipAddress = ipAddress;
		this.port = port;
	}

	private void BeginReceiveFrom(PipeTcpNet pipeTcpNet)
	{
		if (CreateNewMessage == null)
		{
			pipeTcpNet.Socket.BeginReceive(buffer, 0, bufferLength, SocketFlags.None, socketAsyncCallBack, pipeTcpNet);
		}
		else
		{
			pipeTcpNet.Socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, socketAsyncCallBack, pipeTcpNet);
		}
	}

	/// <summary>
	/// 连接到远程服务器，返回是否连接成功
	/// Connect to the remote server and return whether the connection was successful
	/// </summary>
	/// <returns>返回是否连接成功</returns>
	public OperateResult ConnectServer()
	{
		CoreSocket?.Close();
		OperateResult<Socket> operateResult = CreateSocketAndConnect(ipAddress, port, ConnectTimeOut);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		try
		{
			CoreSocket = operateResult.Content;
			pipeTcpNet = new PipeTcpNet(CoreSocket, new IPEndPoint(IPAddress.Parse(ipAddress), port));
			BeginReceiveFrom(pipeTcpNet);
			return OperateResult.CreateSuccessResult();
		}
		catch (Exception ex)
		{
			return new OperateResult(ex.Message);
		}
	}

	/// <summary>
	/// 关闭当前的连接对象
	/// Close the current connection object
	/// </summary>
	/// <returns>错误信息</returns>
	public OperateResult ConnectClose()
	{
		try
		{
			CoreSocket?.Close();
			return OperateResult.CreateSuccessResult();
		}
		catch (Exception ex)
		{
			return new OperateResult(ex.Message);
		}
	}

	/// <summary>
	/// 发送字符串到服务器上去，编码使用的是属性  来指定
	/// Send the string to the server, and the Encoding is specified using the attribute
	/// </summary>
	/// <param name="text">文本信息</param>
	/// <returns>发送是否成功</returns>
	public OperateResult SendString(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return OperateResult.CreateSuccessResult();
		}
		return Send(CoreSocket, encoding.GetBytes(text));
	}

	/// <summary>
	/// 发送字节数据到服务器上去
	/// Send byte data to the server
	/// </summary>
	/// <param name="data">二进制的字节数组</param>
	/// <returns>是否发送成功</returns>
	public OperateResult SendData(byte[] data)
	{
		if (data == null || data.Length == 0)
		{
			return OperateResult.CreateSuccessResult();
		}
		return Send(CoreSocket, data);
	}

	private void ReceiveCallBack(IAsyncResult ar)
	{
		if (!(ar.AsyncState is PipeTcpNet pipeTcpNet))
		{
			return;
		}
		byte[] array = null;
		try
		{
			int num = pipeTcpNet.Socket.EndReceive(ar);
			if (CreateNewMessage == null)
			{
				if (num == 0)
				{
					CoreSocket?.Close();
					return;
				}
				array = new byte[num];
				Array.Copy(buffer, 0, array, 0, num);
			}
			else
			{
				OperateResult<byte[]> operateResult = pipeTcpNet.ReceiveMessage(CreateNewMessage(), null);
				if (!operateResult.IsSuccess)
				{
					CoreSocket?.Close();
					return;
				}
				array = operateResult.Content;
			}
		}
		catch (ObjectDisposedException)
		{
			return;
		}
		catch (Exception ex2)
		{
			base.LogNet?.WriteWarn(StringResources.Language.SocketContentReceiveException + ":" + ex2.Message);
			ThreadPool.QueueUserWorkItem(reConnectCallBack, null);
			return;
		}
		if (array != null)
		{
			ReceivedData?.Invoke(array);
			if (ReceivedString != null)
			{
				ReceivedString?.Invoke(encoding.GetString(array));
			}
		}
		BeginReceiveFrom(pipeTcpNet);
	}

	/// <summary>
	/// 重新连接服务器的委托方法
	/// </summary>
	/// <param name="obj">无用的对象</param>
	private void ReConnectServer(object obj)
	{
		base.LogNet?.WriteWarn(StringResources.Language.ReConnectServerAfterTenSeconds);
		for (int i = 0; i < 10; i++)
		{
			HslHelper.ThreadSleep(1000);
			base.LogNet?.WriteWarn($"Wait for connecting server after {9 - i} seconds");
		}
		if (Interlocked.CompareExchange(ref reconnectTick, 1, 0) != 0)
		{
			return;
		}
		OperateResult<Socket> operateResult = CreateSocketAndConnect(ipAddress, port, ConnectTimeOut);
		if (!operateResult.IsSuccess)
		{
			ThreadPool.QueueUserWorkItem(reConnectCallBack, obj);
			Interlocked.Exchange(ref reconnectTick, 0);
			return;
		}
		lock (connectLock)
		{
			try
			{
				CoreSocket?.Close();
				CoreSocket = operateResult.Content;
				pipeTcpNet = new PipeTcpNet(CoreSocket, new IPEndPoint(IPAddress.Parse(ipAddress), port));
				BeginReceiveFrom(pipeTcpNet);
				base.LogNet?.WriteWarn(StringResources.Language.ReConnectServerSuccess);
			}
			catch (Exception ex)
			{
				base.LogNet?.WriteWarn(StringResources.Language.RemoteClosedConnection + ":" + ex.Message);
				ThreadPool.QueueUserWorkItem(reConnectCallBack, obj);
			}
			finally
			{
				Interlocked.Exchange(ref reconnectTick, 0);
			}
		}
	}

	public override string ToString()
	{
		return $"NetPlainSocket[{ipAddress}:{port}]";
	}
}
