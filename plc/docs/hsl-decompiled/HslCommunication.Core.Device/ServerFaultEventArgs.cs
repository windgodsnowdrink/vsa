// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using HslCommunication.Core.Net;

namespace HslCommunication.Core.Device;

/// <summary>
/// 故障注入事件的信息
/// Information about the fault injection event
/// </summary>
public class ServerFaultEventArgs : EventArgs
{
	/// <summary>
	/// 本次注入的故障类型
	/// </summary>
	public ServerFaultType FaultType { get; set; }

	/// <summary>
	/// 发生注入的会话信息
	/// </summary>
	public PipeSession Session { get; set; }

	/// <summary>
	/// 本次注入的描述信息
	/// </summary>
	public string Message { get; set; }
}
