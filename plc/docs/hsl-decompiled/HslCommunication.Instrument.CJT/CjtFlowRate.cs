// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.Instrument.CJT;

/// <summary>
/// CJT协议的流量数据，主要是用来获取水表流量及燃气流量的
/// </summary>
public class CjtFlowRate
{
	/// <summary>
	/// 当前累积流量
	/// </summary>
	public double CurrentFlowRate { get; set; }

	/// <summary>
	/// 当前累计流量的单位
	/// </summary>
	public string CurrentUnit { get; set; }

	/// <summary>
	/// 结算日累积流量
	/// </summary>
	public double SettlementDateFlowRate { get; set; }

	/// <summary>
	/// 结算日的流量单位
	/// </summary>
	public string SettlementDateUnit { get; set; }

	/// <summary>
	/// 实时时间
	/// </summary>
	public DateTime DateTime { get; set; }
}
