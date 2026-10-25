// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.Core;
using HslCommunication.Core.Pipe;

namespace HslCommunication.Profinet.Freedom;

/// <summary>
/// 基于UDP/IP协议的自由协议，需要在地址里传入报文信息，也可以传入数据偏移信息，默认为
/// </summary>
public class FreedomUdpNet : FreedomTcpNet
{
	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public FreedomUdpNet()
	{
		base.ByteTransform = new RegularByteTransform();
		CommunicationPipe = new PipeUdpNet();
	}

	/// <summary>
	/// 指定IP地址及端口号来实例化自由的TCP协议
	/// </summary>
	/// <param name="ipAddress">Ip地址</param>
	/// <param name="port">端口</param>
	public FreedomUdpNet(string ipAddress, int port)
	{
		CommunicationPipe = new PipeUdpNet();
		IpAddress = ipAddress;
		Port = port;
		base.ByteTransform = new RegularByteTransform();
	}

	public override string ToString()
	{
		return $"FreedomUdpNet<{base.ByteTransform.GetType()}>[{IpAddress}:{Port}]";
	}
}
