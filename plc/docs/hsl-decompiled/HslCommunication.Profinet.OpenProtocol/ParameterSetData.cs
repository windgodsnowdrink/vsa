// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Profinet.OpenProtocol;

/// <summary>
/// parameter set data
/// </summary>
public class ParameterSetData
{
	/// <summary>
	/// Parameter set ID
	/// 参数设置ID
	/// </summary>
	public int ParameterSetID { get; set; }

	/// <summary>
	/// Parameter set name
	/// 参数设置名称
	/// </summary>
	public string ParameterSetName { get; set; }

	/// <summary>
	/// Rotation direction, CW: 顺时针, CCW: 逆时针
	/// 旋转方向，CW: 顺时针, CCW: 逆时针
	/// </summary>
	public string RotationDirection { get; set; }

	/// <summary>
	/// Batch size
	/// </summary>
	public int BatchSize { get; set; }

	/// <summary>
	/// Torque min
	/// 最小力矩
	/// </summary>
	public double TorqueMin { get; set; }

	/// <summary>
	/// Torque max
	/// 最大力矩
	/// </summary>
	public double TorqueMax { get; set; }

	/// <summary>
	/// Torque final target
	/// 最终目标力矩
	/// </summary>
	public double TorqueFinalTarget { get; set; }

	/// <summary>
	/// Angle min
	/// 最小角度
	/// </summary>
	public int AngleMin { get; set; }

	/// <summary>
	/// Angle max
	/// 最大角度
	/// </summary>
	public int AngleMax { get; set; }

	/// <summary>
	/// The target angle is specified in degree
	/// 指定的目标角度
	/// </summary>
	public int AngleFinalTarget { get; set; }
}
