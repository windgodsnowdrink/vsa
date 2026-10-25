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
/// 系统状态信息
/// </summary>
public class SysStatusInfo
{
	/// <summary>
	/// dummy
	/// </summary>
	public short Dummy { get; set; }

	/// <summary>
	/// T/M mode
	/// </summary>
	public short TMMode { get; set; }

	/// <summary>
	/// selected automatic mode
	/// </summary>
	public CNCWorkMode WorkMode { get; set; }

	/// <summary>
	/// running status
	/// </summary>
	public CNCRunStatus RunStatus { get; set; }

	/// <summary>
	/// axis, dwell status
	/// </summary>
	public short Motion { get; set; }

	/// <summary>
	/// m, s, t, b status
	/// </summary>
	public short MSTB { get; set; }

	/// <summary>
	/// emergency stop status，为1就是急停，为0就是正常
	/// </summary>
	public short Emergency { get; set; }

	/// <summary>
	/// alarm status
	/// </summary>
	public short Alarm { get; set; }

	/// <summary>
	/// editting status
	/// </summary>
	public short Edit { get; set; }

	public override string ToString()
	{
		return $"Dummy: {Dummy}, TMMode:{TMMode}, WorkMode:{WorkMode}, RunStatus:{RunStatus}, " + $"Motion:{Motion}, MSTB:{MSTB}, Emergency:{Emergency}, Alarm:{Alarm}, Edit:{Edit}";
	}
}
