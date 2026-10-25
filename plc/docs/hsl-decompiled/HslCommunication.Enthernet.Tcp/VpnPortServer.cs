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
using HslCommunication.Core.Net;
using HslCommunication.LogNet;
using HslCommunication.MQTT;

namespace HslCommunication.Enthernet.Tcp;

internal class VpnPortServer : CommunicationServer
{
	private MqttSession mqttSession = null;

	private MqttServer mqttServer = null;

	private ILogNet logNet = null;

	public void SetMqtt(MqttServer mqttServer, MqttSession mqttSession, ILogNet logNet)
	{
		this.mqttSession = mqttSession;
		this.mqttServer = mqttServer;
		this.logNet = logNet;
		base.OnClientOnline += Server_OnClientOnline;
		base.OnClientOffline += Server_OnClientOffline;
		base.OnPipeMessageReceived += Server_OnPipeMessageReceived;
	}

	private void Server_OnClientOnline(object server, PipeSession session)
	{
		session.SessionID = Guid.NewGuid().ToString("N");
		mqttServer.PublishTopicPayload(mqttSession, "Online", Encoding.UTF8.GetBytes(session.SessionID));
		logNet?.WriteInfo(mqttSession.ToString() + "   Client: " + session.ToString() + " online");
	}

	private void Server_OnClientOffline(object server, PipeSession session)
	{
		mqttServer.PublishTopicPayload(mqttSession, "Offline", Encoding.UTF8.GetBytes(session.SessionID));
		logNet?.WriteInfo(mqttSession.ToString() + "   Client: " + session.ToString() + " offline");
	}

	private void Server_OnPipeMessageReceived(PipeSession session, byte[] buffer)
	{
		mqttServer.PublishTopicPayload(mqttSession, "Data:" + session.SessionID, buffer);
	}
}
