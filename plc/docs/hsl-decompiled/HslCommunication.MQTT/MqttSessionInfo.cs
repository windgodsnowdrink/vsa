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

namespace HslCommunication.MQTT;

/// <summary>
/// 用于客户端获取服务器会话状态监视数据的类
/// </summary>
public class MqttSessionInfo
{
	/// <summary>
	/// 远程的ip地址端口信息
	/// Remote ip address port information
	/// </summary>
	public string EndPoint { get; set; }

	/// <summary>
	/// 当前接收的客户端ID信息
	/// Client ID information currently received
	/// </summary>
	public string ClientId { get; set; }

	/// <summary>
	/// 当前客户端的激活时间
	/// The activation time of the current client
	/// </summary>
	public DateTime ActiveTime { get; set; }

	/// <summary>
	/// 获取当前的客户端的上线时间
	/// Get the online time of the current client
	/// </summary>
	public DateTime OnlineTime { get; set; }

	/// <summary>
	/// 当前客户端订阅的所有的Topic信息
	/// All Topic information subscribed by the current client
	/// </summary>
	public string[] Topics { get; set; }

	/// <summary>
	/// 当前的用户名
	/// Current username
	/// </summary>
	public string UserName { get; set; }

	/// <summary>
	/// 当前的协议信息，一般为 "MQTT"，如果是同步客户端那么是 "HUSL"，如果是文件客户端就是 "FILE"
	/// The current protocol information, generally "MQTT", if it is a synchronous client then it is "HUSL", if it is a file client it is "FILE"
	/// </summary>
	public string Protocol { get; set; }

	/// <summary>
	/// 遗嘱主题
	/// will topic
	/// </summary>
	/// <remarks>
	/// 当前的会话如果因为非正常原因下线的时候，服务器立即发布该客户端的遗嘱主题及数据信息
	/// </remarks>
	public string WillTopic { get; set; }

	/// <summary>
	/// 获取或设置当前的会话是否拥有开发者权限，在开发者权限下，可以遍历接口信息，默认只有admin账户支持开发者权限，如果自定义设置账户权限，则需要在会话登录的事件里，将本属性值设置为 True
	/// Obtain or set whether the current session has developer permissions, under the developer permissions, you can traverse the interface information, by default,
	/// only the admin account supports developer permissions, if you customize the account permissions, you need to set the value of this property to True in the session login event
	/// </summary>
	public bool DeveloperPermissions { get; set; }

	/// <summary>
	/// 获取设置客户端的加密信息
	/// </summary>
	public bool IsAesCryptography { get; set; }

	/// <summary>
	/// 获取或设置当前的MQTT客户端是否允许发布消息，默认为False，如果设置为True，就是禁止发布消息，服务器不会触发收到消息的事件。
	/// Gets or sets whether the current MQTT client is allowed to publish messages, the default is False,
	/// if set to True, it is forbidden to publish messages, The server does not trigger the event of receiving a message.
	/// </summary>
	public bool ForbidPublishTopic { get; set; }

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(Protocol + " Session[IP:" + EndPoint + "]");
		if (!string.IsNullOrEmpty(ClientId))
		{
			stringBuilder.Append(" [ID:" + ClientId + "]");
		}
		if (!string.IsNullOrEmpty(UserName))
		{
			stringBuilder.Append(" [Name:" + UserName + "]");
		}
		if (IsAesCryptography)
		{
			stringBuilder.Append("[RSA/AES]");
		}
		return stringBuilder.ToString();
	}
}
