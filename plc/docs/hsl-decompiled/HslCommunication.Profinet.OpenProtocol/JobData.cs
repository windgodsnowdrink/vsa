// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Collections.Generic;

namespace HslCommunication.Profinet.OpenProtocol;

/// <summary>
/// Job data
/// </summary>
public class JobData
{
	/// <summary>
	/// Job ID
	/// </summary>
	public int JobID { get; set; }

	/// <summary>
	/// Job name
	/// </summary>
	public string JobName { get; set; }

	/// <summary>
	/// Forced order: 0=free order, 1=forced order, 2=free and forced
	/// </summary>
	public int ForcedOrder { get; set; }

	/// <summary>
	/// Max time for first tightening
	/// </summary>
	public int MaxTimeForFirstTightening { get; set; }

	/// <summary>
	/// Max time to complete Job
	/// </summary>
	public int MaxTimeToCompleteJob { get; set; }

	/// <summary>
	/// Job batch mode
	/// </summary>
	public int JobBatchMode { get; set; }

	/// <summary>
	/// Lock at Job done
	/// </summary>
	public bool LockAtJobDone { get; set; }

	/// <summary>
	/// Use line control
	/// </summary>
	public bool UseLineControl { get; set; }

	/// <summary>
	/// Repeat Job
	/// </summary>
	public bool RepeatJob { get; set; }

	/// <summary>
	/// Tool loosening: 0=Enable, 1=Disable, 2=Enable only on NOK tightening
	/// </summary>
	public int ToolLoosening { get; set; }

	/// <summary>
	/// Reserved for Job repair. 0=E, 1=G
	/// </summary>
	public int Reserved { get; set; }

	/// <summary>
	/// A list of parameter sets
	/// </summary>
	public List<JobItem> JobList { get; set; }
}
