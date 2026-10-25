// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.Core.IMessage;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 基于Qna 兼容4C帧协议的通讯，支持格式1,2,3,4,5，支持设置开启和校验，具体的地址需要参照三菱的基本地址，本类是基于TCP的通信
/// Communication based on the QnA‑compatible 4C‑frame protocol supports Formats 1, 2, 3, 4, and 5.
/// Checksum verification can be enabled or disabled. Refer to Mitsubishi’s base address specifications for specific addresses. This class implements TCP‑based communication.
/// </summary>
public class MelsecA4CNetOverTcp : MelsecA3CNetOverTcp
{
	public override int McMode => 1;

	/// <summary>
	/// 实例化默认的对象
	/// Instantiate the default object
	/// </summary>
	public MelsecA4CNetOverTcp()
	{
	}

	/// <summary>
	/// 指定ip地址和端口号来实例化对象
	/// Specify the IP address and port number to instantiate the object
	/// </summary>
	/// <param name="ipAddress">Ip地址信息</param>
	/// <param name="port">端口号信息</param>
	public MelsecA4CNetOverTcp(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new MelsecA4CMessage
		{
			Format = base.Format,
			SumCheck = base.SumCheck
		};
	}

	public override string ToString()
	{
		return $"MelsecA4CNetOverTcp[{CommunicationPipe}]";
	}
}
