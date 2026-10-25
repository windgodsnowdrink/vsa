// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Profinet.Geniitek;

/// <summary>
/// 振动传感器的峰值数据类
/// </summary>
public class VibrationSensorPeekValue
{
	/// <summary>
	/// X轴的加速度，单位 m/s2
	/// </summary>
	public float AcceleratedSpeedX { get; set; }

	/// <summary>
	/// Y轴的加速度，单位 m/s2
	/// </summary>
	public float AcceleratedSpeedY { get; set; }

	/// <summary>
	/// Z轴的加速度，单位 m/s2
	/// </summary>
	public float AcceleratedSpeedZ { get; set; }

	/// <summary>
	/// X轴的速度，单位 mm/s
	/// </summary>
	public float SpeedX { get; set; }

	/// <summary>
	/// Y轴的速度，单位 mm/s
	/// </summary>
	public float SpeedY { get; set; }

	/// <summary>
	/// Z轴的速度，单位 mm/s
	/// </summary>
	public float SpeedZ { get; set; }

	/// <summary>
	/// X轴的位置，单位 um
	/// </summary>
	public int OffsetX { get; set; }

	/// <summary>
	/// Y轴的位移，单位 um
	/// </summary>
	public int OffsetY { get; set; }

	/// <summary>
	/// Z轴的位移，单位 um
	/// </summary>
	public int OffsetZ { get; set; }

	/// <summary>
	/// 温度，单位 摄氏度
	/// </summary>
	public float Temperature { get; set; }

	/// <summary>
	/// 电压，单位 伏特
	/// </summary>
	public float Voltage { get; set; }

	/// <summary>
	/// 数据的发送间隔，单位秒
	/// </summary>
	public int SendingInterval { get; set; }
}
