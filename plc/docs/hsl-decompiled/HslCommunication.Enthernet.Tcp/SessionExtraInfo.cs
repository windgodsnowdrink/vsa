// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.Enthernet.Tcp;

/// <summary>
/// 会话额外的属性
/// </summary>
public class SessionExtraInfo
{
	internal VpnPortServer Server { get; set; }

	/// <summary>
	/// 服务的启动时间
	/// </summary>
	public DateTime StartTime { get; set; } = DateTime.Now;

	/// <summary>
	/// 服务的运行时间，单位秒，默认15分钟，超过这个时间后，服务将自动关闭
	/// </summary>
	public int RunTime { get; set; } = 900;

	/// <summary>
	/// 是否可以自定义映射的端口号
	/// </summary>
	public bool CanDefinePort { get; set; } = false;
}
