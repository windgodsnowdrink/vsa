// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Threading;

namespace HslCommunication.MQTT;

/// <summary>
/// 订阅的主题信息
/// Subscribed topic information
/// </summary>
public class SubscribeTopic
{
	private long subscribeTick = 0L;

	/// <summary>
	/// 主题信息
	/// topic information
	/// </summary>
	public string Topic { get; set; }

	public long SubscribeTick => subscribeTick;

	/// <summary>
	/// 当接收到Mqtt订阅的信息的时候触发的事件
	/// Event triggered when a message subscribed to by Mqtt is received
	/// </summary>
	public event MqttClient.MqttMessageReceiveDelegate OnMqttMessageReceived;

	/// <summary>
	/// 使用指定的主题初始化
	/// Initialize with the specified theme
	/// </summary>
	/// <param name="topic">主题信息</param>
	public SubscribeTopic(string topic)
	{
		Topic = topic;
	}

	/// <summary>
	/// 使用指定的参数，触发订阅主题的事件
	/// Using the specified parameters, trigger the event of the subscribed topic
	/// </summary>
	/// <param name="client">客户端会话信息</param>
	/// <param name="message">Mqtt主题消息</param>
	public void TriggerSubscription(MqttClient client, MqttApplicationMessage message)
	{
		OnMqttMessageReceived?.Invoke(client, message);
	}

	/// <summary>
	/// 增加一个订阅的计数信息，不需要手动调用，在  订阅主题之后，会自动增加
	/// Add a subscription count information, no need to manually call it, it will be automatically added after  subscribes to the topic
	/// </summary>
	/// <returns>返回增加计数后的值</returns>
	public long AddSubscribeTick()
	{
		return Interlocked.Increment(ref subscribeTick);
	}

	/// <summary>
	/// 减少一个订阅的计数信息，用户可以手动调用，比如判断是否是最后一次移除，然后决定是否通过  取消订阅主题
	/// Reduce the count information of a subscription, the user can call it manually, such as judging whether it is the last removal,
	/// and then decide whether to unsubscribe the topic through
	/// </summary>
	/// <returns>返回减少计数后的值</returns>
	public long RemoveSubscribeTick()
	{
		return Interlocked.Decrement(ref subscribeTick);
	}

	public override string ToString()
	{
		return "SubscribeTopic[" + Topic + "]";
	}
}
