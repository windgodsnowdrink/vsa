// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Text;

namespace HslCommunication.Profinet.Special;

/// <summary>
/// 数据对象
/// </summary>
public class EcFanData
{
	/// <summary>
	/// 应急状态，True表示应急状态，False表示正常
	/// </summary>
	public bool EmergencyState { get; set; } = false;

	/// <summary>
	/// 运行状态，True表示运行，False表示停止
	/// </summary>
	public bool RunState { get; set; }

	/// <summary>
	/// 堵转状态，True表示堵转，False表示正常
	/// </summary>
	public bool LockState { get; set; }

	/// <summary>
	/// 过热状态，True表示过热，False表示正常
	/// </summary>
	public bool OverHotState { get; set; }

	/// <summary>
	/// 失速状态，True表示失速，False表示正常
	/// </summary>
	public bool LostSpeedState { get; set; }

	/// <summary>
	/// 实际的转速
	/// </summary>
	public int Speed { get; set; } = 0;

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("应急状态: " + (EmergencyState ? "处于应急状态" : "正常状态"));
		stringBuilder.AppendLine("运行状态: " + (RunState ? "运行中" : "停止"));
		stringBuilder.AppendLine("堵转状态: " + (LockState ? "处于堵转中" : "正常状态"));
		stringBuilder.AppendLine("过热状态: " + (OverHotState ? "驱动器处于过热状态" : "驱动器处于正常状态"));
		stringBuilder.AppendLine("失速状态: " + (LostSpeedState ? "处于失速中" : "正常状态"));
		stringBuilder.Append("实际转速: " + Speed);
		return stringBuilder.ToString();
	}
}
