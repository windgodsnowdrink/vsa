// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.Core.IMessage;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 基于Qna 兼容4C帧协议的通讯，支持格式1,2,3,4,5，支持设置开启和校验，具体的地址需要参照三菱的基本地址
/// Communication based on the QnA‑compatible 4C‑frame protocol supports Formats 1, 2, 3, 4, and 5.
/// Checksum verification can be enabled or disabled. Refer to Mitsubishi’s base address specifications for specific addresses.
/// </summary>
public class MelsecA4CNet : MelsecA3CNet
{
	public override int McMode => 1;

	protected override INetMessage GetNewNetMessage()
	{
		return new MelsecA4CMessage
		{
			Format = base.Format,
			SumCheck = base.SumCheck
		};
	}

	public override string ToString()
	{
		return $"MelsecA4CNet[{CommunicationPipe}]";
	}
}
