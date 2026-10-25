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
using HslCommunication.Profinet.YASKAWA.Helper;

namespace HslCommunication.Profinet.YASKAWA;

/// <summary>
/// 扩展的Memobus协议信息，除了支持普通的线圈，输入继电器，保持寄存器，输入寄存器的读写操作，还支持扩展的保持寄存器和输入寄存器读写操作。
/// The extended Memobus protocol information not only supports reading and writing operations of ordinary coils, input relays,
/// holding registers, and input registers, but also supports reading and writing operations of extended holding registers and input registers.
/// </summary>
/// <remarks>
/// 其中线圈和输入继电器使用和,的方法，读取输入继电器地址：x=2;100。
/// 其他的方法针对的是寄存器，保持型寄存器地址：100或 x=3;100，输入寄存器：x=4;100，扩展保持型寄存器x=9;100，写入x=11;100, 扩展输入寄存器：x=10;100
/// The coil and input relay use  and , method,
/// read the input relay address: x=2;100. Other methods are for registers, holding register address: 100 or x=3;100, input register: x=4;100,
/// extended holding register x=9;100, writing x=11;100, extended input Register: x=10;100
/// 读取的最大的字为 2044 个字，写入的最大的字数为 2043 个字
/// </remarks>
public class MemobusUdpNet : MemobusTcpNet, IMemobus, IReadWriteDevice, IReadWriteNet
{
	/// <summary>
	/// 实例化一个Memobus-Tcp协议的客户端对象
	/// Instantiate a client object of the Memobus-Tcp protocol
	/// </summary>
	public MemobusUdpNet()
	{
		CommunicationPipe = new PipeUdpNet();
	}

	/// <summary>
	/// 指定服务器地址，端口号，客户端自己的站号来初始化
	/// Specify the server address, port number, and client's own station number to initialize
	/// </summary>
	/// <param name="ipAddress">服务器的Ip地址</param>
	/// <param name="port">服务器的端口号</param>
	public MemobusUdpNet(string ipAddress, int port = 502)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	public override string ToString()
	{
		return $"MemobusUdpNet[{IpAddress}:{Port}]";
	}
}
