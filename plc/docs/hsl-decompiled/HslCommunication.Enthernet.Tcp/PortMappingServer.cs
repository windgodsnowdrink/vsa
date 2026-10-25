// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Text;
using System.Threading;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Net;
using HslCommunication.LogNet;
using HslCommunication.MQTT;

namespace HslCommunication.Enthernet.Tcp;

/// <summary>
/// 端口映射服务器
/// </summary>
public class PortMappingServer
{
	private int serverPort = 8888;

	private MqttServer mqttServer;

	private int freeTime = 3600;

	private int randomPortStart = 25000;

	private int randomPortEnd = 30000;

	private ILogNet logNet;

	private Timer timer;

	/// <summary>
	/// 获取或设置MQTT服务器的监听端口号，默认8888
	/// Get or set the listening port number of the MQTT server, the default is 8888
	/// </summary>
	public int ServerPort
	{
		get
		{
			return serverPort;
		}
		set
		{
			serverPort = value;
		}
	}

	/// <summary>
	/// 获取当前的远程客户端数量
	/// </summary>
	public int SocketClientCount { get; private set; }

	/// <summary>
	/// 获取或设置当前端口转发得免费使用时间，默认为 3600 秒
	/// </summary>
	public int FreeTime
	{
		get
		{
			return freeTime;
		}
		set
		{
			freeTime = value;
		}
	}

	/// <summary>
	/// 获取或设置随机端口的起始值
	/// </summary>
	public int RandomPortStart
	{
		get
		{
			return randomPortStart;
		}
		set
		{
			randomPortStart = value;
		}
	}

	/// <summary>
	/// 获取或设置随机端口的结束值
	/// </summary>
	public int RandomPortEnd
	{
		get
		{
			return randomPortEnd;
		}
		set
		{
			randomPortEnd = value;
		}
	}

	/// <summary>
	/// 获取或设置当前的日志对象
	/// </summary>
	public ILogNet LogNet
	{
		get
		{
			return logNet;
		}
		set
		{
			logNet = value;
		}
	}

	/// <summary>
	/// 当客户端连接时，触发的验证事件
	/// Validation event triggered when the client connects
	/// </summary>
	public event MqttServer.ClientVerificationDelegate ClientVerification;

	public PortMappingServer()
	{
		timer = new Timer(ThreadTimerHeartCheck, null, 1000, 2000);
	}

	private void ThreadTimerHeartCheck(object obj)
	{
		if (mqttServer == null)
		{
			return;
		}
		int num = 0;
		MqttSession[] onlineSessions = mqttServer.OnlineSessions;
		MqttSession[] array = onlineSessions;
		foreach (MqttSession mqttSession in array)
		{
			if (!(mqttSession.Tag is SessionExtraInfo sessionExtraInfo))
			{
				continue;
			}
			if ((DateTime.Now - sessionExtraInfo.StartTime).TotalSeconds > (double)sessionExtraInfo.RunTime)
			{
				if (sessionExtraInfo.Server != null)
				{
					sessionExtraInfo.Server.ServerClose();
				}
				mqttServer.RemoveAndCloseSession(mqttSession, "Time out[RunTime]");
			}
			else if (sessionExtraInfo.Server != null)
			{
				int num2 = num;
				PipeSession[] pipeSessions = sessionExtraInfo.Server.GetPipeSessions();
				num = num2 + ((pipeSessions != null) ? pipeSessions.Length : 0);
			}
		}
		SocketClientCount = num;
	}

	/// <summary>
	/// 启动服务
	/// </summary>
	public void ServerStart()
	{
		mqttServer = new MqttServer();
		mqttServer.ClientVerification += MqttServer_ClientVerification;
		mqttServer.OnClientApplicationMessageReceive += MqttServer_OnClientApplicationMessageReceive;
		mqttServer.OnClientConnected += MqttServer_OnClientConnected;
		mqttServer.OnClientDisConnected += MqttServer_OnClientDisConnected;
		mqttServer.LogNet = logNet;
		mqttServer.ServerStart(serverPort);
	}

	/// <summary>
	/// 获取当前的MQTT服务器
	/// </summary>
	public MqttServer GetMqttServer()
	{
		return mqttServer;
	}

	private int MqttServer_ClientVerification(MqttSession mqttSession, string clientId, string userName, string password)
	{
		SessionExtraInfo sessionExtraInfo = new SessionExtraInfo();
		sessionExtraInfo.RunTime = freeTime;
		mqttSession.Tag = sessionExtraInfo;
		if (ClientVerification != null)
		{
			return ClientVerification(mqttSession, clientId, userName, password);
		}
		return 0;
	}

	private void MqttServer_OnClientApplicationMessageReceive(MqttSession session, MqttClientApplicationMessage message)
	{
		message.IsCancelPublish = true;
		if (message.Topic == "GetPort")
		{
			if (!(session.Tag is SessionExtraInfo sessionExtraInfo))
			{
				return;
			}
			if (sessionExtraInfo.Server != null)
			{
				sessionExtraInfo.Server.ServerClose();
			}
			int num = -1;
			if (message.Payload != null && message.Payload.Length >= 4)
			{
				num = BitConverter.ToInt32(message.Payload, 0);
				if (num > 65535 || num <= 0)
				{
					num = -1;
				}
			}
			sessionExtraInfo.Server = new VpnPortServer();
			sessionExtraInfo.Server.SetMqtt(mqttServer, session, logNet);
			bool success = false;
			for (int i = 0; i < 5; i++)
			{
				int port = HslHelper.HslRandom.Next(randomPortStart, randomPortEnd);
				if (i == 0 && sessionExtraInfo.CanDefinePort && num > 0)
				{
					port = num;
				}
				sessionExtraInfo.Server.Port = port;
				try
				{
					sessionExtraInfo.Server.ServerStart();
					success = true;
				}
				catch
				{
					continue;
				}
				break;
			}
			if (success)
			{
				byte[] buffer = new byte[8];
				BitConverter.GetBytes(sessionExtraInfo.Server.Port).CopyTo(buffer, 0);
				BitConverter.GetBytes(sessionExtraInfo.RunTime).CopyTo(buffer, 4);
				mqttServer.PublishTopicPayload(session, "GetPort", buffer);
			}
			else
			{
				mqttServer.PublishTopicPayload(session, "GetPort", SoftBasic.SpliceArray<byte>(BitConverter.GetBytes(1000000), Encoding.UTF8.GetBytes("There is no port to start server, try again later")));
				HslHelper.ThreadSleep(1000);
				mqttServer.RemoveAndCloseSession(session, "No ports");
			}
		}
		else if (message.Topic.StartsWith("Data:"))
		{
			string text = message.Topic.Substring(5);
			if (!(session.Tag is SessionExtraInfo sessionExtraInfo2))
			{
				return;
			}
			if (sessionExtraInfo2.Server == null)
			{
				mqttServer.RemoveAndCloseSession(session, "not Started");
				return;
			}
			PipeSession[] pipeSessions = sessionExtraInfo2.Server.GetPipeSessions();
			if (pipeSessions == null || pipeSessions.Length == 0)
			{
				return;
			}
			for (int j = 0; j < pipeSessions.Length; j++)
			{
				if (pipeSessions[j].SessionID == text)
				{
					pipeSessions[j].Communication.Send(message.Payload);
					break;
				}
			}
		}
		else
		{
			if (!(message.Topic == "Offline") || !(session.Tag is SessionExtraInfo { Server: not null } sessionExtraInfo3))
			{
				return;
			}
			string text2 = string.Empty;
			if (message.Payload != null)
			{
				text2 = Encoding.UTF8.GetString(message.Payload);
			}
			PipeSession[] pipeSessions2 = sessionExtraInfo3.Server.GetPipeSessions();
			if (pipeSessions2 == null || pipeSessions2.Length == 0)
			{
				return;
			}
			for (int k = 0; k < pipeSessions2.Length; k++)
			{
				if (string.IsNullOrEmpty(text2) || pipeSessions2[k].SessionID == text2)
				{
					sessionExtraInfo3.Server.RemoveSession(pipeSessions2[k], "");
				}
			}
		}
	}

	private void MqttServer_OnClientConnected(MqttSession session)
	{
		logNet?.WriteInfo(session.ToString() + " connected");
	}

	private void MqttServer_OnClientDisConnected(MqttSession session)
	{
		if (session.Tag is SessionExtraInfo sessionExtraInfo)
		{
			if (sessionExtraInfo.Server != null)
			{
				sessionExtraInfo.Server.ServerClose();
			}
			logNet?.WriteInfo(session.ToString() + " disconnected");
		}
	}
}
