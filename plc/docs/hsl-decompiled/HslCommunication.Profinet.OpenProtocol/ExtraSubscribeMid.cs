// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Profinet.OpenProtocol;

/// <summary>
/// 用于额外的订阅的MID信息，包含当前的MID以及回复的MID信息，如果是订阅的消息，则回复的MID通常是当前MID+1
/// </summary>
public class ExtraSubscribeMid
{
	/// <summary>
	/// 订阅的MID信息，通常是设备方发过来的数据的Mid
	/// Gets or sets the middle identifier.
	/// </summary>
	public int Mid { get; set; }

	/// <summary>
	/// 用于订阅的消息，回复的MID通常是当前MID+1
	/// Gets or sets the message ID to which this message is a reply.
	/// </summary>
	public int ReplyMid { get; set; } = -1;
}
