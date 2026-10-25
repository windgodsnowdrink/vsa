// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Core;

/// <summary>
/// 文件的基础信息
/// </summary>
public class FileBaseInfo
{
	/// <summary>
	/// 文件名称
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// 文件大小
	/// </summary>
	public long Size { get; set; }

	/// <summary>
	/// 文件的标识，注释
	/// </summary>
	public string Tag { get; set; }

	/// <summary>
	/// 文件上传人的名称
	/// </summary>
	public string Upload { get; set; }
}
