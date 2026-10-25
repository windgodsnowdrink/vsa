// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using HslCommunication.Core.Net;

namespace HslCommunication.Profinet.Toledo;

/// <summary>
/// 托利多电子秤的TCP服务器，启动服务器后，等待电子秤的数据连接。
/// </summary>
public class ToledoTcpServer : CommunicationServer
{
	public delegate void ToledoStandardDataReceivedDelegate(object sender, ToledoStandardData toledoStandardData);

	/// <summary>
	/// 获取或设置当前的报文否是含有校验的，默认为含有校验
	/// </summary>
	public bool HasChk { get; set; } = true;

	/// <summary>
	/// 当接收到一条新的托利多的数据的时候触发
	/// </summary>
	public event ToledoStandardDataReceivedDelegate OnToledoStandardDataReceived;

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public ToledoTcpServer()
	{
		base.OnPipeMessageReceived += ToledoTcpServer_OnPipeMessageReceived;
	}

	private void ToledoTcpServer_OnPipeMessageReceived(PipeSession session, byte[] buffer)
	{
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.Receive + " : " + buffer.ToHexString(' '));
		ToledoStandardData toledoStandardData = null;
		try
		{
			toledoStandardData = new ToledoStandardData(buffer);
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), "ToledoStandardData new failed: " + buffer.ToHexString(' '), ex);
		}
		if (toledoStandardData != null)
		{
			OnToledoStandardDataReceived?.Invoke(session, toledoStandardData);
		}
	}

	public override string ToString()
	{
		return $"ToledoTcpServer[{base.Port}]";
	}
}
