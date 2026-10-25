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
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core.Net;
using HslCommunication.Core.Pipe;

namespace HslCommunication.WebSocket;

/// <summary>
/// WebSocket协议的实现，支持创建自定义的websocket服务器，直接给其他的网页端，客户端，手机端发送数据信息，详细看api文档说明
/// The implementation of the WebSocket protocol supports the creation of custom websocket servers and sends data information directly to other web pages, clients, and mobile phones. See the API documentation for details.
/// </summary>
public class WebSocketServer : CommunicationTcpServer, IDisposable
{
	public delegate void OnClientApplicationMessageReceiveDelegate(WebSocketSession session, WebSocketMessage message);

	public delegate void OnClientConnectedDelegate(WebSocketSession session);

	private readonly Dictionary<string, string> retainKeys;

	private readonly object keysLock;

	private bool isRetain = true;

	private readonly List<WebSocketSession> wsSessions = new List<WebSocketSession>();

	private readonly object sessionsLock = new object();

	private Timer timerHeart;

	private bool disposedValue;

	private bool topicWildcard = false;

	private AsyncCallback beginReceiveCallback = null;

	public int OnlineCount => wsSessions.Count;

	/// <summary>
	/// 获取或设置是否启用订阅主题通配符的功能，默认为 False
	/// Gets or sets whether to enable the function of subscribing to the topic wildcard, the default is False
	/// </summary>
	/// <remarks>
	/// 启动之后，通配符示例：finance/stock/ibm/#; finance/+; '#' 是匹配所有主题，'+' 是匹配一级主题树。
	/// 通配符的规则参考如下的网址：http://public.dhe.ibm.com/software/dw/webservices/ws-mqtt/mqtt-v3r1.html#appendix-a
	/// </remarks>
	public bool TopicWildcard
	{
		get
		{
			return topicWildcard;
		}
		set
		{
			topicWildcard = value;
		}
	}

	/// <summary>
	/// 获取或设置当前的服务器是否对订阅主题信息缓存，方便订阅客户端立即收到结果，默认开启
	/// Gets or sets whether the current server caches the topic information of the subscription, so that the subscription client can receive the results immediately. It is enabled by default.
	/// </summary>
	public bool IsTopicRetain
	{
		get
		{
			return isRetain;
		}
		set
		{
			isRetain = value;
		}
	}

	/// <summary>
	/// 获取或设置当前的数据是否进行压缩后发送，默认不压缩
	/// Get or set whether the current data is compressed before being sent. By default, it is not compressed
	/// </summary>
	public bool DataCompress { get; set; } = false;

	/// <summary>
	/// 获取当前的在线的客户端信息，可以用于额外的分析或是显示。
	/// Obtain the current online client information, which can be used for additional analysis or display.
	/// </summary>
	public WebSocketSession[] OnlineSessions
	{
		get
		{
			WebSocketSession[] result = null;
			lock (sessionsLock)
			{
				result = wsSessions.ToArray();
			}
			return result;
		}
	}

	/// <summary>
	/// 设置的参数，最小单位为1s，当超过设置的时间间隔必须回复PONG报文，否则服务器认定为掉线。默认120秒
	/// Set the minimum unit of the parameter is 1s. When the set time interval is exceeded, the PONG packet must be returned, otherwise the server considers it to be offline. 120 seconds by default
	/// </summary>
	/// <remarks>
	/// 保持连接（Keep Alive）是一个以秒为单位的时间间隔，它是指客户端返回一个PONG报文到下一次返回PONG报文的时候，
	/// 两者之间允许空闲的最大时间间隔。客户端负责保证控制报文发送的时间间隔不超过保持连接的值。
	/// </remarks>
	public TimeSpan KeepAlivePeriod { get; set; } = TimeSpan.FromSeconds(120.0);

	/// <summary>
	/// 获取或是设置用于保持连接的心跳时间的发送间隔。默认30秒钟，需要在服务启动之前设置
	/// Gets or sets the sending interval of the heartbeat time used to keep the connection. 30 seconds by default, need to be set before the service starts
	/// </summary>
	public TimeSpan KeepAliveSendInterval { get; set; } = TimeSpan.FromSeconds(30.0);

	/// <summary>
	/// websocket的消息收到时触发
	/// Triggered when a websocket message is received
	/// </summary>
	public event OnClientApplicationMessageReceiveDelegate OnClientApplicationMessageReceive;

	/// <summary>
	/// Websocket的客户端连接上来时触发
	/// Triggered when a Websocket client connects
	/// </summary>
	public event OnClientConnectedDelegate OnClientConnected;

	/// <summary>
	/// Websocket的客户端下线时触发
	/// Triggered when Websocket client connects
	/// </summary>
	public event OnClientConnectedDelegate OnClientDisConnected;

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public WebSocketServer()
	{
		beginReceiveCallback = ReceiveCallback;
		retainKeys = new Dictionary<string, string>();
		keysLock = new object();
	}

	protected override void ExtraOnStart()
	{
		base.ExtraOnStart();
		if (KeepAliveSendInterval.TotalMilliseconds > 0.0 && timerHeart == null)
		{
			timerHeart = new Timer(ThreadTimerHeartCheck, null, 2000, (int)KeepAliveSendInterval.TotalMilliseconds);
		}
	}

	protected override void ExtraOnClose()
	{
		base.ExtraOnClose();
		CleanWsSession();
	}

	private void ThreadTimerHeartCheck(object obj)
	{
		WebSocketSession[] array = null;
		lock (sessionsLock)
		{
			array = wsSessions.ToArray();
		}
		if (array == null || array.Length == 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].IsQASession)
			{
				if (DateTime.Now - array[i].HeartTime > KeepAlivePeriod)
				{
					RemoveAndCloseSession(array[i], "Heart check timeout[" + SoftBasic.GetTimeSpanDescription(DateTime.Now - array[i].HeartTime) + "]");
				}
				else
				{
					SendWebsocket(array[i], WebSocketHelper.WebScoketPackData(9, isMask: false, "Heart Check"));
				}
			}
		}
	}

	protected override void ThreadPoolLogin(PipeTcpNet pipe, IPEndPoint endPoint)
	{
		HandleWebsocketConnection(pipe, endPoint);
	}

	private async void ReceiveCallback(IAsyncResult ar)
	{
		object asyncState = ar.AsyncState;
		if (asyncState is WebSocketSession session)
		{
			PipeTcpNet pipe = (PipeTcpNet)session.Communication;
			try
			{
				pipe.Socket.EndReceive(ar);
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				session.Close();
				base.LogNet?.WriteDebug(ToString(), "ReceiveCallback Failed:" + ex2.Message);
				RemoveAndCloseSession(session);
				return;
			}
			HandleWebsocketMessage(session, await WebSocketHelper.ReceiveWebSocketPayloadAsync(pipe));
		}
	}

	private OperateResult SendWebsocket(WebSocketSession session, byte[] data)
	{
		return session.Communication.Send(data);
	}

	private async Task<OperateResult> SendWebsocketAsync(WebSocketSession session, byte[] data)
	{
		return await session.Communication.SendAsync(data).ConfigureAwait(continueOnCapturedContext: false);
	}

	private void HandleWebsocketConnection(PipeTcpNet pipe, IPEndPoint endPoint)
	{
		WebSocketSession webSocketSession = new WebSocketSession
		{
			HeartTime = DateTime.Now,
			Remote = endPoint,
			Communication = pipe
		};
		OperateResult<byte[]> operateResult = webSocketSession.Communication.Receive(-1, 5000);
		if (!operateResult.IsSuccess)
		{
			return;
		}
		string message = Encoding.UTF8.GetString(operateResult.Content);
		OperateResult operateResult2 = WebSocketHelper.CheckWebSocketLegality(message);
		if (!operateResult2.IsSuccess)
		{
			pipe?.CloseCommunication();
			base.LogNet?.WriteDebug(ToString(), $"[{endPoint}] WebScoket Check Failed:" + operateResult2.Message + Environment.NewLine + message);
			return;
		}
		OperateResult<byte[]> response = WebSocketHelper.GetResponse(message, DataCompress);
		if (!response.IsSuccess)
		{
			pipe?.CloseCommunication();
			base.LogNet?.WriteDebug(ToString(), $"[{endPoint}] GetResponse Failed:" + response.Message);
			return;
		}
		OperateResult operateResult3 = SendWebsocket(webSocketSession, response.Content);
		if (!operateResult3.IsSuccess)
		{
			return;
		}
		webSocketSession.IsQASession = message.Contains("HslRequestAndAnswer: true") || message.Contains("HslRequestAndAnswer:true");
		Match match = Regex.Match(message, "GET [\\S\\s]+ HTTP/1", RegexOptions.IgnoreCase);
		if (match.Success)
		{
			webSocketSession.Url = match.Value.Substring(4, match.Value.Length - 11);
		}
		try
		{
			string[] array = WebSocketHelper.GetWebSocketSubscribes(message);
			if (array == null)
			{
				array = WebSocketHelper.GetWebSocketSubscribesFromUrl(webSocketSession.Url);
			}
			if (array != null)
			{
				webSocketSession.Topics = new List<string>(array);
				if (isRetain)
				{
					lock (keysLock)
					{
						if (TopicWildcard)
						{
							foreach (KeyValuePair<string, string> retainKey in retainKeys)
							{
								if (webSocketSession.IsClientSubscribe(retainKey.Key, TopicWildcard))
								{
									operateResult3 = SendWebsocket(webSocketSession, WebSocketHelper.WebScoketPackData(1, isMask: false, retainKey.Value, DataCompress));
									if (!operateResult3.IsSuccess)
									{
										return;
									}
								}
							}
						}
						else
						{
							for (int i = 0; i < webSocketSession.Topics.Count; i++)
							{
								if (retainKeys.ContainsKey(webSocketSession.Topics[i]))
								{
									operateResult3 = SendWebsocket(webSocketSession, WebSocketHelper.WebScoketPackData(1, isMask: false, retainKeys[webSocketSession.Topics[i]], DataCompress));
									if (!operateResult3.IsSuccess)
									{
										return;
									}
								}
							}
						}
					}
				}
			}
			pipe.Socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, beginReceiveCallback, webSocketSession);
			AddWsSession(webSocketSession);
		}
		catch (Exception ex)
		{
			pipe?.CloseCommunication();
			base.LogNet?.WriteDebug(ToString(), $"[{webSocketSession.Remote}] BeginReceive Failed: {ex.Message}");
			return;
		}
		OnClientConnected?.Invoke(webSocketSession);
	}

	private void HandleWebsocketMessage(WebSocketSession session, OperateResult<WebSocketMessage> read)
	{
		if (!read.IsSuccess)
		{
			RemoveAndCloseSession(session);
			return;
		}
		session.HeartTime = DateTime.Now;
		if (read.Content.OpCode == 8)
		{
			session.Close();
			RemoveAndCloseSession(session, Encoding.UTF8.GetString(read.Content.Payload));
			return;
		}
		if (read.Content.OpCode == 9)
		{
			base.LogNet?.WriteDebug(ToString(), $"[{session.Remote}] PING: {read.Content}");
			OperateResult operateResult = SendWebsocket(session, WebSocketHelper.WebScoketPackData(10, isMask: false, read.Content.Payload));
			if (!operateResult.IsSuccess)
			{
				RemoveAndCloseSession(session, "HandleWebsocketMessage -> 09 opCode send back exception -> " + operateResult.Message);
				return;
			}
		}
		else if (read.Content.OpCode == 10)
		{
			base.LogNet?.WriteDebug(ToString(), $"[{session.Remote}] PONG: {read.Content}");
		}
		else
		{
			OnClientApplicationMessageReceive?.Invoke(session, read.Content);
		}
		try
		{
			PipeTcpNet pipeTcpNet = session.Communication as PipeTcpNet;
			pipeTcpNet.Socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, beginReceiveCallback, session);
		}
		catch (Exception ex)
		{
			session.Close();
			RemoveAndCloseSession(session, "BeginReceive Exception -> " + ex.Message);
		}
	}

	private void PublishSessionList(IEnumerable<WebSocketSession> sessions, string payload)
	{
		foreach (WebSocketSession session in sessions)
		{
			OperateResult operateResult = SendWebsocket(session, WebSocketHelper.WebScoketPackData(1, isMask: false, payload, DataCompress));
			if (!operateResult.IsSuccess)
			{
				base.LogNet?.WriteError(ToString(), $"[{session.Remote}] Send Failed: {operateResult.Message}");
			}
		}
	}

	private void PublishSessionList(IEnumerable<WebSocketSession> sessions, byte[] payload)
	{
		foreach (WebSocketSession session in sessions)
		{
			OperateResult operateResult = SendWebsocket(session, WebSocketHelper.WebScoketPackData(2, isMask: false, payload, DataCompress));
			if (!operateResult.IsSuccess)
			{
				base.LogNet?.WriteError(ToString(), $"[{session.Remote}] Send Failed: {operateResult.Message}");
			}
		}
	}

	/// <summary>
	/// 向所有的客户端强制发送消息，消息类型为文本消息，也就是 opCode = 1
	/// Force the sending of messages to all clients, with the message type being text messages, that is, opCode = 1
	/// </summary>
	/// <param name="payload">消息内容</param>
	public void PublishAllClientPayload(string payload)
	{
		List<WebSocketSession> list = new List<WebSocketSession>();
		lock (sessionsLock)
		{
			for (int i = 0; i < wsSessions.Count; i++)
			{
				if (!wsSessions[i].IsQASession)
				{
					list.Add(wsSessions[i]);
				}
			}
		}
		PublishSessionList(list, payload);
	}

	/// <summary>
	/// 向订阅了topic主题的客户端发送消息
	/// Send messages to clients subscribed to topic
	/// </summary>
	/// <param name="topic">主题</param>
	/// <param name="payload">消息内容</param>
	/// <param name="retain">指定当前的主题是否驻留</param>
	public void PublishClientPayload(string topic, string payload)
	{
		PublishClientPayload(topic, payload, isRetain);
	}

	/// <summary>
	/// 向所有的客户端强制发送消息，消息类型为二进制消息，也就是 opCode = 2
	/// Force the sending of messages to all clients, with the message type being binary messages, that is, opCode = 2
	/// </summary>
	/// <param name="payload">消息内容</param>
	public void PublishAllClientPayload(byte[] payload)
	{
		List<WebSocketSession> list = new List<WebSocketSession>();
		lock (sessionsLock)
		{
			for (int i = 0; i < wsSessions.Count; i++)
			{
				if (!wsSessions[i].IsQASession)
				{
					list.Add(wsSessions[i]);
				}
			}
		}
		PublishSessionList(list, payload);
	}

	/// <summary>
	/// 向订阅了topic主题的客户端发送二进制的消息，注意：二进制数据无法设置为消息驻留
	/// Send binary messages to the clients that have subscribed to the topic. Note: Binary data cannot be set for message residence
	/// </summary>
	/// <param name="topic">主题</param>
	/// <param name="payload">二进制的数据消息</param>
	public void PublishClientPayload(string topic, byte[] payload)
	{
		List<WebSocketSession> list = new List<WebSocketSession>();
		lock (sessionsLock)
		{
			for (int i = 0; i < wsSessions.Count; i++)
			{
				if (!wsSessions[i].IsQASession && wsSessions[i].IsClientSubscribe(topic, topicWildcard))
				{
					list.Add(wsSessions[i]);
				}
			}
		}
		PublishSessionList(list, payload);
	}

	/// <summary>
	/// 向所有的客户端强制发送二进制数据，这个二进制数据是一个完整的websocket数据包，可以调用方法 来构建一个数据包
	/// Force the sending of binary data to all clients. This binary data is a complete websocket data packet Can call methods  to build a data packet
	/// </summary>
	/// <param name="binary">原始的二进制数据</param>
	public void PublishClientBinary(byte[] binary)
	{
		for (int i = 0; i < wsSessions.Count; i++)
		{
			OperateResult operateResult = SendWebsocket(wsSessions[i], binary);
			if (!operateResult.IsSuccess)
			{
				base.LogNet?.WriteError(ToString(), $"[{wsSessions[i].Remote}] Send Failed: {operateResult.Message}");
			}
		}
	}

	/// <summary>
	/// 向订阅了topic主题的客户端发送消息
	/// Send messages to clients subscribed to topic
	/// </summary>
	/// <param name="topic">主题</param>
	/// <param name="payload">消息内容</param>
	/// <param name="retain">指定当前的主题是否驻留</param>
	public void PublishClientPayload(string topic, string payload, bool retain)
	{
		List<WebSocketSession> list = new List<WebSocketSession>();
		lock (sessionsLock)
		{
			for (int i = 0; i < wsSessions.Count; i++)
			{
				if (!wsSessions[i].IsQASession && wsSessions[i].IsClientSubscribe(topic, topicWildcard))
				{
					list.Add(wsSessions[i]);
				}
			}
		}
		PublishSessionList(list, payload);
		if (retain)
		{
			AddTopicRetain(topic, payload);
		}
	}

	private async Task PublishSessionListAsync(List<WebSocketSession> sessions, string payload)
	{
		for (int i = 0; i < sessions.Count; i++)
		{
			OperateResult send = await SendWebsocketAsync(sessions[i], WebSocketHelper.WebScoketPackData(1, isMask: false, payload, DataCompress));
			if (!send.IsSuccess)
			{
				base.LogNet?.WriteError(ToString(), $"[{sessions[i].Remote}] Send Failed: {send.Message}");
			}
		}
	}

	/// <summary>
	/// 向所有的客户端强制发送消息，消息类型为文本消息，也就是 opCode = 1
	/// Force the sending of messages to all clients, with the message type being text messages, that is, opCode = 1
	/// </summary>
	/// <param name="payload">消息内容</param>
	public async Task PublishAllClientPayloadAsync(string payload)
	{
		List<WebSocketSession> sessions = new List<WebSocketSession>();
		lock (sessionsLock)
		{
			for (int i = 0; i < wsSessions.Count; i++)
			{
				if (!wsSessions[i].IsQASession)
				{
					sessions.Add(wsSessions[i]);
				}
			}
		}
		await PublishSessionListAsync(sessions, payload);
	}

	/// <summary>
	/// 向订阅了topic主题的客户端发送消息
	/// Send messages to clients subscribed to topic
	/// </summary>
	/// <param name="topic">主题</param>
	/// <param name="payload">消息内容</param>
	/// <param name="retain">指定当前的主题是否驻留</param>
	public async Task PublishClientPayloadAsync(string topic, string payload)
	{
		await PublishClientPayloadAsync(topic, payload, isRetain);
	}

	/// <summary>
	/// 向订阅了topic主题的客户端发送消息
	/// Send messages to clients subscribed to topic
	/// </summary>
	/// <param name="topic">主题</param>
	/// <param name="payload">消息内容</param>
	/// <param name="retain">指定当前的主题是否驻留</param>
	public async Task PublishClientPayloadAsync(string topic, string payload, bool retain)
	{
		List<WebSocketSession> sessions = new List<WebSocketSession>();
		lock (sessionsLock)
		{
			for (int i = 0; i < wsSessions.Count; i++)
			{
				if (!wsSessions[i].IsQASession && wsSessions[i].IsClientSubscribe(topic, topicWildcard))
				{
					sessions.Add(wsSessions[i]);
				}
			}
		}
		await PublishSessionListAsync(sessions, payload);
		if (retain)
		{
			AddTopicRetain(topic, payload);
		}
	}

	/// <summary>
	/// 向指定的客户端发送数据
	/// Send data to the specified client
	/// </summary>
	/// <param name="session">会话内容</param>
	/// <param name="payload">消息内容</param>
	public void SendClientPayload(WebSocketSession session, string payload)
	{
		SendWebsocket(session, WebSocketHelper.WebScoketPackData(1, isMask: false, payload, DataCompress));
	}

	/// <summary>
	/// 向指定的客户端发送数据
	/// Send data to the specified client
	/// </summary>
	/// <param name="session">会话内容</param>
	/// <param name="payload">消息内容</param>
	public void SendClientPayload(WebSocketSession session, byte[] payload)
	{
		SendWebsocket(session, WebSocketHelper.WebScoketPackData(1, isMask: false, payload, DataCompress));
	}

	/// <summary>
	/// 给一个当前的会话信息动态添加订阅的主题
	/// Dynamically add subscribed topics to a current session message
	/// </summary>
	/// <param name="session">会话内容</param>
	/// <param name="topic">主题信息</param>
	public void AddSessionTopic(WebSocketSession session, string topic)
	{
		session.AddTopic(topic);
		PublishSessionTopic(session, topic);
	}

	/// <summary>
	/// 从指定的WebSocket会话中移除指定的主题。
	/// Removes the specified topic from the given WebSocket session.
	/// </summary>
	/// <param name="session">会话内容</param>
	/// <param name="topic">主题信息</param>
	public bool RemoveSessionTopic(WebSocketSession session, string topic)
	{
		return session.RemoveTopic(topic);
	}

	private void CleanWsSession()
	{
		lock (sessionsLock)
		{
			for (int i = 0; i < wsSessions.Count; i++)
			{
				wsSessions[i].Close();
			}
			wsSessions.Clear();
		}
	}

	private void AddWsSession(WebSocketSession session)
	{
		lock (sessionsLock)
		{
			wsSessions.Add(session);
		}
		base.LogNet?.WriteDebug(ToString(), $"Client[{session.Remote}] Online");
	}

	/// <summary>
	/// 让Websocket客户端正常下线，调用本方法即可自由控制会话客户端强制下线操作。
	/// Let the Websocket client go offline normally. Call this method to freely control the session client to force offline operation.
	/// </summary>
	/// <param name="session">当前的会话信息</param>
	/// <param name="reason">下线的原因，默认为空</param>
	public void RemoveAndCloseSession(WebSocketSession session, string reason = null)
	{
		lock (sessionsLock)
		{
			wsSessions.Remove(session);
		}
		session.Close();
		base.LogNet?.WriteDebug(ToString(), $"Client[{session.Remote}]  Offline {reason}");
		OnClientDisConnected?.Invoke(session);
	}

	private void AddTopicRetain(string topic, string payload)
	{
		lock (keysLock)
		{
			if (retainKeys.ContainsKey(topic))
			{
				retainKeys[topic] = payload;
			}
			else
			{
				retainKeys.Add(topic, payload);
			}
		}
	}

	private void PublishSessionTopic(WebSocketSession session, string topic)
	{
		bool success = false;
		string message = string.Empty;
		lock (keysLock)
		{
			if (retainKeys.ContainsKey(topic))
			{
				success = true;
				message = retainKeys[topic];
			}
		}
		if (success)
		{
			SendWebsocket(session, WebSocketHelper.WebScoketPackData(1, isMask: false, message, DataCompress));
		}
	}

	/// <summary>
	/// 释放当前的对象
	/// </summary>
	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				OnClientApplicationMessageReceive = null;
				OnClientConnected = null;
				OnClientDisConnected = null;
			}
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public override string ToString()
	{
		return $"WebSocketServer[{base.Port}]";
	}
}
