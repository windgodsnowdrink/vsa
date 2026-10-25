// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Text;
using HslCommunication.Core.Net;
using HslCommunication.Core.Pipe;

namespace HslCommunication.Profinet.Sick;

/// <summary>
/// Sick的扫码器的服务器信息，只要启动服务器之后，扫码器配置将条码发送到PC的指定端口上来即可，就可以持续的接收条码信息，同样也适用于海康，基恩士，DATELOGIC 。
/// The server information of Sick's code scanner, as long as the server is started, the code scanner is configured to send the barcode to the designated port of the PC, and it can continuously receive the barcode information.
/// </summary>
public class SickIcrTcpServer : CommunicationServer
{
	public delegate void ReceivedBarCodeDelegate(string ipAddress, string barCode);

	/// <summary>
	/// 当接收到条码数据的时候触发
	/// Triggered when barcode data is received
	/// </summary>
	public event ReceivedBarCodeDelegate OnReceivedBarCode;

	/// <summary>
	/// 实例化一个默认的服务器对象
	/// Instantiate a default server object
	/// </summary>
	public SickIcrTcpServer()
	{
		base.OnPipeMessageReceived += SickIcrTcpServer_OnPipeMessageReceived;
	}

	private void SickIcrTcpServer_OnPipeMessageReceived(PipeSession session, byte[] buffer)
	{
		string ipAddress = string.Empty;
		if (session.Communication is PipeTcpNet pipeTcpNet)
		{
			ipAddress = pipeTcpNet.IpAddress;
		}
		if (session != null)
		{
			base.LogNet?.WriteDebug(ToString(), $"<{session.Communication}> Recv: " + buffer.ToHexString(' '));
		}
		OnReceivedBarCode?.Invoke(ipAddress, TranslateCode(Encoding.ASCII.GetString(buffer)));
	}

	private string TranslateCode(string code)
	{
		StringBuilder stringBuilder = new StringBuilder("");
		for (int i = 0; i < code.Length; i++)
		{
			if (char.IsLetterOrDigit(code, i))
			{
				stringBuilder.Append(code[i]);
			}
		}
		return stringBuilder.ToString();
	}

	public override string ToString()
	{
		return $"SickIcrTcpServer[{base.Port}]";
	}
}
