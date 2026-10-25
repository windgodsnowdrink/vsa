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

namespace HslCommunication.BasicFramework;

/// <summary>
/// 版本信息类，用于展示版本发布信息
/// </summary>
public sealed class VersionInfo
{
	/// <summary>
	/// 版本的发行日期
	/// </summary>
	public DateTime ReleaseDate { get; set; } = DateTime.Now;

	/// <summary>
	/// 版本的更新细节
	/// </summary>
	public StringBuilder UpdateDetails { get; set; } = new StringBuilder();

	/// <summary>
	/// 版本号
	/// </summary>
	public SystemVersion VersionNum { get; set; } = new SystemVersion(1, 0, 0);

	public override string ToString()
	{
		return VersionNum.ToString();
	}
}
