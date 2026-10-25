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
using HslCommunication.Core.Net;

namespace HslCommunication.Enthernet;

/// <summary>
/// 发布订阅类的客户端，使用指定的关键订阅相关的数据推送信息
/// </summary>
/// <remarks>
/// 详细的使用说明，请参照博客http://www.cnblogs.com/dathlin/p/8992315.html
/// </remarks>
public class NetPushClient : NetworkXBase
{
	private readonly IPEndPoint endPoint;

	private readonly string keyWord = string.Empty;

	private Action<NetPushClient, string> action;

	private int reconnectTime = 10000;

	private bool closed = false;

	public string KeyWord => keyWord;

	/// <summary>
	/// 获取或设置重连服务器的间隔时间，单位：毫秒
	/// </summary>
	public int ReConnectTime
	{
		get
		{
			return reconnectTime;
		}
		set
		{
			reconnectTime = value;
		}
	}

	/// <summary>
	/// 当接收到数据的事件信息，接收到数据的时候触发。
	/// </summary>
	public event Action<NetPushClient, string> OnReceived;

	/// <summary>
	/// 实例化一个发布订阅类的客户端，需要指定ip地址，端口，及订阅关键字
	/// </summary>
	/// <param name="ipAddress">服务器的IP地址</param>
	/// <param name="port">服务器的端口号</param>
	/// <param name="key">订阅关键字</param>
	public NetPushClient(string ipAddress, int port, string key)
	{
		endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
		keyWord = key;
		if (string.IsNullOrEmpty(key))
		{
			throw new Exception(StringResources.Language.KeyIsNotAllowedNull);
		}
	}

	/// <summary>
	/// 创建数据推送服务
	/// </summary>
	/// <param name="pushCallBack">触发数据推送的委托</param>
	/// <returns>是否创建成功</returns>
	public OperateResult CreatePush(Action<NetPushClient, string> pushCallBack)
	{
		action = pushCallBack;
		return CreatePush();
	}

	/// <summary>
	/// 创建数据推送服务，使用事件绑定的机制实现
	/// </summary>
	/// <returns>是否创建成功</returns>
	public OperateResult CreatePush()
	{
		CoreSocket?.Close();
		OperateResult<Socket> operateResult = CreateSocketAndConnect(endPoint, 5000);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult operateResult2 = SendStringAndCheckReceive(operateResult.Content, 0, keyWord);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		OperateResult<int, string> operateResult3 = ReceiveStringContentFromSocket(operateResult.Content);
		if (!operateResult3.IsSuccess)
		{
			return operateResult3;
		}
		if (operateResult3.Content1 != 0)
		{
			operateResult.Content?.Close();
			return new OperateResult(operateResult3.Content2);
		}
		AppSession appSession = new AppSession(operateResult.Content);
		CoreSocket = operateResult.Content;
		try
		{
			appSession.WorkSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, ReceiveCallback, appSession);
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), StringResources.Language.SocketReceiveException, ex);
			return new OperateResult(ex.Message);
		}
		closed = false;
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 关闭消息推送的界面
	/// </summary>
	public void ClosePush()
	{
		action = null;
		closed = true;
		if (CoreSocket != null && CoreSocket.Connected)
		{
			CoreSocket?.Send(BitConverter.GetBytes(100));
		}
		HslHelper.ThreadSleep(20);
		CoreSocket?.Close();
	}

	private void ReconnectServer(object obj)
	{
		do
		{
			if (closed)
			{
				return;
			}
			Console.WriteLine(StringResources.Language.ReConnectServerAfterTenSeconds);
			HslHelper.ThreadSleep(reconnectTime);
			if (closed)
			{
				return;
			}
		}
		while (!CreatePush().IsSuccess);
		Console.WriteLine(StringResources.Language.ReConnectServerSuccess);
	}

	private async void ReceiveCallback(IAsyncResult ar)
	{
		object asyncState = ar.AsyncState;
		if (!(asyncState is AppSession appSession))
		{
			return;
		}
		try
		{
			appSession.WorkSocket.EndReceive(ar);
		}
		catch
		{
			ThreadPool.QueueUserWorkItem(ReconnectServer, null);
			return;
		}
		OperateResult<int, int, byte[]> read = await ReceiveHslMessageAsync(appSession.WorkSocket);
		if (!read.IsSuccess)
		{
			ThreadPool.QueueUserWorkItem(ReconnectServer, null);
			return;
		}
		int protocol = read.Content1;
		_ = read.Content2;
		byte[] content = read.Content3;
		switch (protocol)
		{
		case 1001:
			action?.Invoke(this, Encoding.Unicode.GetString(content));
			OnReceived?.Invoke(this, Encoding.Unicode.GetString(content));
			break;
		case 1:
			Send(appSession.WorkSocket, HslProtocol.CommandBytes(1, 0, base.Token, new byte[0]));
			break;
		}
		try
		{
			appSession.WorkSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, ReceiveCallback, appSession);
		}
		catch
		{
			ThreadPool.QueueUserWorkItem(ReconnectServer, null);
		}
	}

	public override string ToString()
	{
		return $"NetPushClient[{endPoint}]";
	}
}
