// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Net;
using System.Net.Sockets;
using HslCommunication.BasicFramework;

namespace HslCommunication.Core.Net;

/// <summary>
/// 当前的网络会话信息，还包含了一些客户端相关的基本的参数信息
/// The current network session information also contains some basic parameter information related to the client
/// </summary>
public class AppSession : SessionBase
{
	/// <summary>
	/// UDP通信中的远程端
	/// Remote side in UDP communication
	/// </summary>
	internal EndPoint UdpEndPoint = null;

	/// <summary>
	/// 远程对象的别名信息
	/// Alias information for remote objects
	/// </summary>
	public string LoginAlias { get; set; }

	/// <summary>
	/// 客户端唯一的标识，在NetPushServer及客户端类里有使用
	/// The unique identifier of the client, used in the NetPushServer and client classes
	/// </summary>
	public string ClientUniqueID { get; set; }

	/// <summary>
	/// 数据内容缓存
	/// data content cache
	/// </summary>
	internal byte[] BytesBuffer { get; set; }

	/// <summary>
	/// 用于关键字分类使用
	/// Used for keyword classification
	/// </summary>
	internal string KeyGroup { get; set; }

	/// <summary>
	/// 当前会话绑定的自定义的对象内容
	/// The content of the custom object bound to the current session
	/// </summary>
	public object Tag { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public AppSession()
	{
		ClientUniqueID = SoftBasic.GetUniqueStringByGuidAndRandom();
	}

	/// <summary>
	/// 通过指定的对象来初始化一个会话内容
	/// </summary>
	/// <param name="socket">连接的Socket对象</param>
	public AppSession(Socket socket)
		: base(socket)
	{
		ClientUniqueID = SoftBasic.GetUniqueStringByGuidAndRandom();
	}

	public override bool Equals(object obj)
	{
		return this == obj;
	}

	public override string ToString()
	{
		return string.IsNullOrEmpty(LoginAlias) ? $"AppSession[{base.IpEndPoint}]" : $"AppSession[{base.IpEndPoint}] [{LoginAlias}]";
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
