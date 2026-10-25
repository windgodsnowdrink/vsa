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

/// <summary>
/// 网络中的异步对象
/// </summary>
internal class StateObject : StateOneBase
{
	/// <summary>
	/// 唯一的一串信息
	/// </summary>
	public string UniqueId { get; set; }

	/// <summary>
	/// 网络套接字
	/// </summary>
	public Socket WorkSocket { get; set; }

	/// <summary>
	/// 是否关闭了通道
	/// </summary>
	public bool IsClose { get; set; }

	/// <summary>
	/// 实例化一个对象
	/// </summary>
	public StateObject()
	{
	}

	/// <summary>
	/// 实例化一个对象，指定接收或是发送的数据长度
	/// </summary>
	/// <param name="length">数据长度</param>
	public StateObject(int length)
	{
		base.DataLength = length;
		base.Buffer = new byte[length];
	}

	/// <summary>
	/// 清空旧的数据
	/// </summary>
	public void Clear()
	{
		base.IsError = false;
		IsClose = false;
		base.AlreadyDealLength = 0;
		base.Buffer = null;
	}
}
