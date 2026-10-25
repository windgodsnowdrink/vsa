// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.Profinet.Melsec;

namespace HslCommunication.Profinet.Inovance;

/// <summary>
/// 汇川的计算机链接协议，使用PLC得COM1口，设置 D8126=06h, D8120.14=1 才能正常通信。
/// Inovance's computer connection protocol, when using the COM1 port of the PLC, requires setting D8126=06h and D8120.14=1 for normal communication.
/// </summary>
public class InovanceComputerLink : MelsecFxLinks
{
	/// <summary>
	/// 实例化一个欧姆龙计算机链接协议的对象
	/// Instantiate an object of the Omron Computer Link Protocol
	/// </summary>
	public InovanceComputerLink()
	{
		base.Format = 4;
	}

	public override string ToString()
	{
		return $"InovanceComputerLink<{CommunicationPipe}>";
	}
}
