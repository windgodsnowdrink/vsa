// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.Core.Net;

namespace HslCommunication.Profinet.OpenProtocol;

internal class OpenProtocolSession : PipeSession
{
	public bool MID0051Subscribe { get; set; } = false;

	public bool MID0060Subscribe { get; set; } = false;

	public int MID0060Revison { get; set; } = 1;

	public bool MID0034Subscribe { get; set; } = false;

	public bool MID0070Subscribe { get; set; } = false;

	public bool MID0014Subscribe { get; set; } = false;

	public bool MID0105Subscribe { get; set; } = false;
}
