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
/// 刀具信息
/// </summary>
public class CutterInfo
{
	/// <summary>
	/// 长度形状补偿
	/// </summary>
	public double LengthSharpOffset { get; set; }

	/// <summary>
	/// 长度磨损补偿
	/// </summary>
	public double LengthWearOffset { get; set; }

	/// <summary>
	/// 半径形状补偿
	/// </summary>
	public double RadiusSharpOffset { get; set; }

	/// <summary>
	/// 半径磨损补偿
	/// </summary>
	public double RadiusWearOffset { get; set; }

	public override string ToString()
	{
		return $"LengthSharpOffset:{LengthSharpOffset:10} LengthWearOffset:{LengthWearOffset:10} RadiusSharpOffset:{RadiusSharpOffset:10} RadiusWearOffset:{RadiusWearOffset:10}";
	}
}
