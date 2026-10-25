// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.CNC.Fanuc;

/// <summary>
/// 设备的工作模式
/// </summary>
public enum CNCWorkMode
{
	MDI = 0,
	AUTO = 1,
	EDIT = 3,
	HANDLE = 4,
	JOG = 5,
	TeachInJOG = 6,
	TeachInHandle = 7,
	INCfeed = 8,
	REFerence = 9,
	ReMoTe = 10
}
