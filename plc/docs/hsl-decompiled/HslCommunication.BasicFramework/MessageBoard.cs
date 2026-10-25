// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.BasicFramework;

/// <summary>
/// 系统的消息类，用来发送消息，和确认消息的
/// </summary>
public class MessageBoard
{
	/// <summary>
	/// 发送方名称
	/// </summary>
	public string NameSend { get; set; } = "";

	/// <summary>
	/// 接收方名称
	/// </summary>
	public string NameReceive { get; set; } = "";

	/// <summary>
	/// 发送时间
	/// </summary>
	public DateTime SendTime { get; set; } = DateTime.Now;

	/// <summary>
	/// 发送的消息内容
	/// </summary>
	public string Content { get; set; } = "";

	/// <summary>
	/// 消息是否已经被查看
	/// </summary>
	public bool HasViewed { get; set; }
}
