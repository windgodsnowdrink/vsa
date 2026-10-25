// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Text;

namespace HslCommunication.WebSocket;

/// <summary>
/// websocket 协议下的单个消息的数据对象
/// Data object for a single message under the websocket protocol
/// </summary>
public class WebSocketMessage
{
	/// <summary>
	/// 是否存在掩码
	/// Whether a mask exists
	/// </summary>
	public bool HasMask { get; set; }

	/// <summary>
	/// 是否是压缩的数据
	/// Is it compressed data
	/// </summary>
	public bool IsCompressed { get; set; }

	/// <summary>
	/// 当前的websocket的操作码
	/// The current websocket opcode
	/// </summary>
	public int OpCode { get; set; }

	/// <summary>
	/// 负载数据
	/// </summary>
	public byte[] Payload { get; set; }

	public override string ToString()
	{
		return $"OpCode[{OpCode}] HasMask[{HasMask}] Payload: {Encoding.UTF8.GetString(Payload)}";
	}
}
