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
using System.Threading.Tasks;
using HslCommunication.Core.IMessage;
using HslCommunication.MQTT;

namespace HslCommunication.Core.Pipe;

/// <summary>
/// 基于MQTT通信实现的管道信息
/// </summary>
public class PipeMqttClient : CommunicationPipe
{
	private MqttClient mqttClient;

	private string writeTopic = string.Empty;

	private string readTopic = string.Empty;

	public MqttClient MqttClient => mqttClient;

	public string ReadTopic => readTopic;

	public string WriteTopic => writeTopic;

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public PipeMqttClient(MqttClient mqttClient, string readTopic, string writeTopic)
	{
		this.readTopic = readTopic;
		this.writeTopic = writeTopic;
		if (mqttClient != null)
		{
			SubscribeTopic subscribeTopic = mqttClient.GetSubscribeTopic(readTopic);
			if (subscribeTopic == null)
			{
				OperateResult operateResult = mqttClient.SubscribeMessage(readTopic);
				if (operateResult.IsSuccess)
				{
					mqttClient.OnClientConnected += MqttClient_OnClientConnected;
					subscribeTopic = mqttClient.GetSubscribeTopic(readTopic);
					subscribeTopic.OnMqttMessageReceived += SubscribeTopic_OnMqttMessageReceived;
				}
			}
		}
		base.UseServerActivePush = true;
		this.mqttClient = mqttClient;
	}

	private void MqttClient_OnClientConnected(MqttClient client)
	{
		client.SubscribeMessage(readTopic);
	}

	private void SubscribeTopic_OnMqttMessageReceived(MqttClient client, MqttApplicationMessage message)
	{
		SetBufferQA(message.Payload);
	}

	public override OperateResult<bool> OpenCommunication()
	{
		SubscribeTopic subscribeTopic = mqttClient.GetSubscribeTopic(readTopic);
		if (subscribeTopic == null)
		{
			OperateResult operateResult = mqttClient.SubscribeMessage(readTopic);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool>(operateResult);
			}
			mqttClient.OnClientConnected += MqttClient_OnClientConnected;
			subscribeTopic = mqttClient.GetSubscribeTopic(readTopic);
			subscribeTopic.OnMqttMessageReceived += SubscribeTopic_OnMqttMessageReceived;
		}
		return OperateResult.CreateSuccessResult(value: false);
	}

	public override OperateResult CloseCommunication()
	{
		SubscribeTopic subscribeTopic = mqttClient.GetSubscribeTopic(readTopic);
		if (subscribeTopic != null)
		{
			subscribeTopic.OnMqttMessageReceived -= SubscribeTopic_OnMqttMessageReceived;
		}
		mqttClient.OnClientConnected -= MqttClient_OnClientConnected;
		mqttClient.UnSubscribeMessage(readTopic);
		return OperateResult.CreateSuccessResult();
	}

	public override OperateResult Send(byte[] data, int offset, int size)
	{
		if (data == null)
		{
			return OperateResult.CreateSuccessResult();
		}
		return mqttClient.PublishMessage(new MqttApplicationMessage
		{
			Topic = writeTopic,
			Payload = ((size == data.Length) ? data : data.SelectMiddle(offset, size))
		});
	}

	public override OperateResult<byte[]> ReceiveMessage(INetMessage netMessage, byte[] sendValue, bool useActivePush = true, Action<long, long> reportProgress = null, Action<byte[]> logMessage = null)
	{
		DateTime now = DateTime.Now;
		MemoryStream ms = new MemoryStream();
		do
		{
			if (base.ReceiveTimeOut >= 0 && (DateTime.Now - now).TotalMilliseconds > (double)base.ReceiveTimeOut)
			{
				return new OperateResult<byte[]>(StringResources.Language.ReceiveDataTimeout + base.ReceiveTimeOut);
			}
			if (autoResetEvent.WaitOne(base.ReceiveTimeOut))
			{
				byte[] array = bufferQA;
				if (array != null && array.Length != 0)
				{
					ms.Write(bufferQA);
					logMessage?.Invoke(bufferQA);
				}
				continue;
			}
			return new OperateResult<byte[]>(-10000, StringResources.Language.ReceiveDataTimeout + base.ReceiveTimeOut + " Received: " + ms.ToArray().ToHexString(' '));
		}
		while (netMessage != null && !CheckMessageComplete(netMessage, sendValue, ref ms));
		return OperateResult.CreateSuccessResult(ms.ToArray());
	}

	public override async Task<OperateResult> SendAsync(byte[] data, int offset, int size)
	{
		if (data == null)
		{
			return OperateResult.CreateSuccessResult();
		}
		return await mqttClient.PublishMessageAsync(new MqttApplicationMessage
		{
			Topic = writeTopic,
			Payload = ((size == data.Length) ? data : data.SelectMiddle(offset, size))
		});
	}

	public override async Task<OperateResult<byte[]>> ReceiveMessageAsync(INetMessage netMessage, byte[] sendValue, bool useActivePush = true, Action<long, long> reportProgress = null, Action<byte[]> logMessage = null)
	{
		DateTime startTime = DateTime.Now;
		MemoryStream ms = new MemoryStream();
		do
		{
			if (base.ReceiveTimeOut >= 0 && (DateTime.Now - startTime).TotalMilliseconds > (double)base.ReceiveTimeOut)
			{
				return new OperateResult<byte[]>(StringResources.Language.ReceiveDataTimeout + base.ReceiveTimeOut);
			}
			if (autoResetEvent.WaitOne(base.ReceiveTimeOut))
			{
				byte[] array = bufferQA;
				if (array != null && array.Length != 0)
				{
					ms.Write(bufferQA);
					logMessage?.Invoke(bufferQA);
				}
				continue;
			}
			return new OperateResult<byte[]>(-10000, StringResources.Language.ReceiveDataTimeout + base.ReceiveTimeOut + " Received: " + ms.ToArray().ToHexString(' '));
		}
		while (netMessage != null && !CheckMessageComplete(netMessage, sendValue, ref ms));
		return await Task.FromResult(OperateResult.CreateSuccessResult(ms.ToArray()));
	}

	public override string ToString()
	{
		return $"PipeMqttClient[{mqttClient}]";
	}
}
