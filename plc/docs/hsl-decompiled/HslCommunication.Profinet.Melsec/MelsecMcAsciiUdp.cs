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
using HslCommunication.Profinet.Melsec.Helper;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 三菱PLC通讯类，采用UDP的协议实现，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为ascii通讯
/// Mitsubishi PLC communication class is implemented using UDP protocol and Qna compatible 3E frame protocol.
/// The Ethernet module needs to be configured first on the PLC side, and it must be ascii communication.
/// </summary>
public class MelsecMcAsciiUdp : MelsecMcAsciiNet, IReadWriteMc, IReadWriteDevice, IReadWriteNet
{
	/// <summary>
	/// 实例化三菱的Qna兼容3E帧协议的通讯对象
	/// Instantiate the communication object of Mitsubishi's Qna compatible 3E frame protocol
	/// </summary>
	public MelsecMcAsciiUdp()
		: this("127.0.0.1", 6000)
	{
	}

	/// <summary>
	/// 指定ip地址和端口号来实例化一个默认的对象
	/// Specify the IP address and port number to instantiate a default object
	/// </summary>
	/// <param name="ipAddress">PLC的Ip地址</param>
	/// <param name="port">PLC的端口</param>
	public MelsecMcAsciiUdp(string ipAddress, int port)
	{
		base.WordLength = 1;
		LogMsgFormatBinary = false;
		base.ByteTransform = new RegularByteTransform();
		CommunicationPipe = new PipeUdpNet
		{
			ReceiveCacheLength = 4096
		};
		IpAddress = ipAddress;
		Port = port;
	}

	public override string ToString()
	{
		return $"MelsecMcAsciiUdp[{IpAddress}:{Port}]";
	}
}
