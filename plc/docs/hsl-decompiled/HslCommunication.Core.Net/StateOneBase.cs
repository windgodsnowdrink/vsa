// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Threading;

namespace HslCommunication.Core.Net;

/// <summary>
/// 异步消息的对象
/// </summary>
internal class StateOneBase
{
	/// <summary>
	/// 本次接收或是发送的数据长度
	/// </summary>
	public int DataLength { get; set; } = 32;

	/// <summary>
	/// 已经处理的字节长度
	/// </summary>
	public int AlreadyDealLength { get; set; }

	/// <summary>
	/// 操作完成的信号
	/// </summary>
	public ManualResetEvent WaitDone { get; set; }

	/// <summary>
	/// 缓存器
	/// </summary>
	public byte[] Buffer { get; set; }

	/// <summary>
	/// 是否发生了错误
	/// </summary>
	public bool IsError { get; set; }

	/// <summary>
	/// 错误消息
	/// </summary>
	public string ErrerMsg { get; set; }
}
