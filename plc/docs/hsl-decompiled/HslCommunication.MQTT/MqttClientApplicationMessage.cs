// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.MQTT;

/// <summary>
/// 来自客户端的一次消息的内容，当前类主要是在MQTT的服务端进行使用
/// The content of a message from the client. The current class is mainly used on the MQTT server
/// </summary>
public class MqttClientApplicationMessage : MqttApplicationMessage
{
	/// <summary>
	/// 客户端的Id信息
	/// Client Id information
	/// </summary>
	public string ClientId { get; set; }

	/// <summary>
	/// 当前的客户端的用户名
	/// Username of the current client
	/// </summary>
	public string UserName { get; set; }

	/// <summary>
	/// 获取或设置当前的消息是否取消发布，默认False，也就是发布出去
	/// Get or set whether the current message is unpublished, the default is False, which means it is published
	/// </summary>
	public bool IsCancelPublish { get; set; } = false;

	/// <summary>
	/// 当前消息的生成时间
	/// The generation time of the current message
	/// </summary>
	public DateTime CreateTime { get; set; }

	/// <summary>
	/// 当前消息的ID信息，在Qos大于0的时候，才是有值的
	/// </summary>
	public int MsgID { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public MqttClientApplicationMessage()
	{
		CreateTime = DateTime.Now;
	}
}
