// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Net.Sockets;

namespace HslCommunication.Core.Net;

internal class AsyncStateSend
{
	/// <summary>
	/// 传输数据的对象
	/// </summary>
	internal Socket WorkSocket { get; set; }

	/// <summary>
	/// 发送的数据内容
	/// </summary>
	internal byte[] Content { get; set; }

	/// <summary>
	/// 已经发送长度
	/// </summary>
	internal int AlreadySendLength { get; set; }

	internal SimpleHybirdLock HybirdLockSend { get; set; }

	/// <summary>
	/// 关键字
	/// </summary>
	internal string Key { get; set; }

	/// <summary>
	/// 客户端的标识
	/// </summary>
	internal string ClientId { get; set; }
}
