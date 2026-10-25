// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using HslCommunication.Core.Pipe;

namespace HslCommunication.Core.Net;

/// <summary>
/// 管道会话信息，包含了一个管道对象，管道可以是TCP,UDP，也可以是串口，甚至是其他自定义的。
/// Pipeline session information, which contains a pipeline object, which can be TCP, UDP, serial port, or even other custom interfaces.
/// </summary>
public class PipeSession
{
	/// <summary>
	/// 获取当前的客户端的上线时间
	/// Get the online time of the current client
	/// </summary>
	public DateTime OnlineTime { get; set; } = DateTime.Now;

	/// <summary>
	/// 获取心跳验证的时间点
	/// Get the time point of heartbeat verification
	/// </summary>
	public DateTime HeartTime { get; set; } = DateTime.Now;

	/// <summary>
	/// 当前会话绑定的自定义的对象内容
	/// The content of the custom object bound to the current session
	/// </summary>
	public object Tag { get; set; }

	/// <summary>
	/// 当前的会话的管道信息
	/// Pipeline information for the current session
	/// </summary>
	public CommunicationPipe Communication { get; set; }

	/// <summary>
	/// 获取或设置当前会话的ID信息，某些特殊的会话需要用到该字段
	/// Obtain or set the ID information of the current session, which is required for some special sessions
	/// </summary>
	public string SessionID { get; set; }

	/// <summary>
	/// 关闭当前的会话状态
	/// Close the current session state
	/// </summary>
	public virtual void Close()
	{
		Communication?.CloseCommunication();
	}

	public override string ToString()
	{
		if (Communication == null)
		{
			return "Session<NULL>";
		}
		return $"Session<{Communication}>";
	}
}
