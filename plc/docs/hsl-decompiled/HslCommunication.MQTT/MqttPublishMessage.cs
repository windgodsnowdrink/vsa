// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.MQTT;

/// <summary>
/// Mqtt发送的消息封装对象，是对  对象的封装，添加了序号，还有是否重发的信息
/// The message encapsulation object sent by Mqtt is an encapsulation of the  object, with the serial number added, and whether to retransmit
/// </summary>
public class MqttPublishMessage
{
	/// <summary>
	/// 是否第一次发送数据信息
	/// Whether to send data information for the first time
	/// </summary>
	public bool IsSendFirstTime { get; set; }

	/// <summary>
	/// 当前的消息的标识符，当质量等级为0的时候，不需要重发以及考虑标识情况
	/// The identifier of the current message, when the quality level is 0, do not need to retransmit and consider the identification situation
	/// </summary>
	public int Identifier { get; set; }

	/// <summary>
	/// 当前发布消息携带的mqtt的应用消息，包含主题，消息等级，负载。
	/// The application message of mqtt carried in the current published message, including the subject, message level, and load.
	/// </summary>
	public MqttApplicationMessage Message { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public MqttPublishMessage()
	{
		IsSendFirstTime = true;
	}
}
