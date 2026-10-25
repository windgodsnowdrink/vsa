// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Net;
using System.Text;

namespace HslCommunication.Core.Net;

/// <summary>
/// DTU远程连接的一些信息
/// </summary>
public class RemoteConnectInfo
{
	private bool ack = false;

	private string dtuId = string.Empty;

	/// <summary>
	/// 远程端口号信息
	/// Remote port number information
	/// </summary>
	public IPEndPoint EndPoint { get; set; }

	/// <summary>
	/// 会话信息
	/// Session information
	/// </summary>
	public PipeSession Session { get; set; }

	/// <summary>
	/// DTU的注册包
	/// Registration package for DTU
	/// </summary>
	public byte[] DtuBytes { get; set; }

	public bool NeedAckResult => ack;

	public string DtuId => dtuId;

	/// <summary>
	/// 当前DTU会话的状态信息，如果需要关闭当前的会话并退出重连，设置本属性为  即可
	/// </summary>
	public DtuStatus Status { get; set; } = DtuStatus.Create;

	/// <summary>
	/// 实例化一个远程连接信息对象，指定自定义的远程 DTU 注册包
	/// </summary>
	/// <param name="ipAddress">IP地址</param>
	/// <param name="port">端口号</param>
	/// <param name="dtu">dtu的注册包</param>
	public RemoteConnectInfo(string ipAddress, int port, byte[] dtu)
	{
		EndPoint = new IPEndPoint(IPAddress.Parse(HslHelper.GetIpAddressFromInput(ipAddress)), port);
		DtuBytes = dtu;
	}

	/// <summary>
	/// 实例化一个远程连接信息对象，传入 IP，Port 来创建符合 HSL 标准的 DTU 注册包
	/// </summary>
	/// <param name="ipAddress">IP地址</param>
	/// <param name="port">端口号</param>
	/// <param name="dtuId">DTU信息</param>
	/// <param name="password">密码</param>
	/// <param name="needAckResult">是否需要返回注册结果</param>
	public RemoteConnectInfo(string ipAddress, int port, string dtuId, string password = "", bool needAckResult = true)
	{
		this.dtuId = dtuId;
		EndPoint = new IPEndPoint(IPAddress.Parse(HslHelper.GetIpAddressFromInput(ipAddress)), port);
		DtuBytes = CreateHslAlienMessage(dtuId, password, needAckResult);
		ack = needAckResult;
	}

	private byte[] CreateHslAlienMessage(string dtuId, string password, bool needAckResult)
	{
		if (dtuId.Length > 11)
		{
			dtuId = dtuId.Substring(11);
		}
		byte[] buffer = new byte[30]
		{
			72, 83, 76, 0, 25, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		};
		if (dtuId.Length > 11)
		{
			dtuId = dtuId.Substring(0, 11);
		}
		Encoding.ASCII.GetBytes(dtuId).CopyTo(buffer, 5);
		if (!string.IsNullOrEmpty(password))
		{
			if (password.Length > 6)
			{
				password = password.Substring(6);
			}
			Encoding.ASCII.GetBytes(password).CopyTo(buffer, 16);
		}
		if (!needAckResult)
		{
			buffer[28] = 1;
		}
		return buffer;
	}

	/// <summary>
	/// 关闭当前的 DTU 连接
	/// Close the current DTU connection
	/// </summary>
	public void Close()
	{
		Status = DtuStatus.Closed;
		Session?.Close();
	}
}
