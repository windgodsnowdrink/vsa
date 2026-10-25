// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Net;
using System.Net.NetworkInformation;
using HslCommunication.Core.Pipe;

namespace HslCommunication.Core.Device;

/// <summary>
/// 基于UDP/IP管道的设备基类信息
/// </summary>
public class DeviceUdpNet : DeviceCommunication
{
	private PipeUdpNet pipe;

	public override CommunicationPipe CommunicationPipe
	{
		get
		{
			return base.CommunicationPipe;
		}
		set
		{
			base.CommunicationPipe = value;
			if (value is PipeUdpNet pipeUdpNet)
			{
				pipe = pipeUdpNet;
			}
		}
	}

	/// <summary>
	/// 获取或是设置远程服务器的IP地址，如果是本机测试，那么需要设置为127.0.0.1
	/// Get or set the IP address of the remote server. If it is a local test, then it needs to be set to 127.0.0.1
	/// </summary>
	/// <remarks>
	/// 最好实在初始化的时候进行指定，当使用短连接的时候，支持动态更改，切换；当使用长连接后，无法动态更改
	/// 支持使用域名的网址方式，例如：www.hslcommunication.cn
	/// </remarks>
	public virtual string IpAddress
	{
		get
		{
			return pipe.IpAddress;
		}
		set
		{
			pipe.IpAddress = value;
		}
	}

	/// <summary>
	/// 获取或设置服务器的端口号，具体的值需要取决于对方的配置
	/// Gets or sets the port number of the server. The specific value depends on the configuration of the other party.
	/// </summary>
	/// <remarks>
	/// 最好实在初始化的时候进行指定，当使用短连接的时候，支持动态更改，切换；当使用长连接后，无法动态更改
	/// </remarks>
	public virtual int Port
	{
		get
		{
			return pipe.Port;
		}
		set
		{
			pipe.Port = value;
		}
	}

	/// <summary>
	/// 获取或设置一次接收时的数据长度，默认2KB数据长度，特殊情况的时候需要调整
	/// Gets or sets the length of data received at a time. The default length is 2KB
	/// </summary>
	public int ReceiveCacheLength
	{
		get
		{
			return pipe.ReceiveCacheLength;
		}
		set
		{
			pipe.ReceiveCacheLength = value;
		}
	}

	/// <summary>
	/// 获取或设置绑定的本地的IP地址和端口号信息，如果端口设置为0，代表任何可用的端口
	/// Get or set the bound local IP address and port number information, if the port is set to 0, it means any available port
	/// </summary>
	/// <remarks>
	/// 默认为NULL, 也即是不绑定任何本地的IP及端口号信息，使用系统自动分配的方式。
	/// The default is NULL, which means that no local IP and port number information are bound, and the system automatically assigns it.
	/// </remarks>
	public IPEndPoint LocalBinding
	{
		get
		{
			return pipe.LocalBinding;
		}
		set
		{
			pipe.LocalBinding = value;
		}
	}

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public DeviceUdpNet()
		: this("127.0.0.1", 5000)
	{
	}

	/// <summary>
	/// 指定IP地址以及端口号信息来初始化对象
	/// </summary>
	/// <param name="ipAddress">IP地址信息，可以是IPv4, IPv6, 也可以是域名</param>
	/// <param name="port">设备方的端口号信息</param>
	public DeviceUdpNet(string ipAddress, int port)
	{
		CommunicationPipe = new PipeUdpNet
		{
			IpAddress = ipAddress,
			Port = port
		};
	}

	/// <summary>
	/// 对当前设备的IP地址进行PING的操作，返回PING的结果，正常来说，返回
	/// PING the IP address of the current device and return the PING result. Normally, it returns
	/// </summary>
	/// <returns>返回PING的结果</returns>
	public IPStatus IpAddressPing()
	{
		Ping ping = new Ping();
		return ping.Send(IpAddress).Status;
	}

	public override string ToString()
	{
		return $"DeviceUdpNet<{base.ByteTransform}>{{{CommunicationPipe}}}";
	}
}
