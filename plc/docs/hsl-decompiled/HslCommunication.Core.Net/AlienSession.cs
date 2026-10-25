// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Text;
using HslCommunication.BasicFramework;
using HslCommunication.Core.Pipe;

namespace HslCommunication.Core.Net;

/// <summary>
/// 异形客户端的连接对象
/// </summary>
public class AlienSession
{
	/// <summary>
	/// 网络套接字
	/// </summary>
	public PipeTcpNet Pipe { get; set; }

	/// <summary>
	/// 唯一的标识
	/// </summary>
	public string DTU { get; set; }

	/// <summary>
	/// 密码信息
	/// </summary>
	public string Pwd { get; set; }

	/// <summary>
	/// 指示当前的网络状态
	/// </summary>
	public bool IsStatusOk { get; set; }

	/// <summary>
	/// 上线时间
	/// </summary>
	public DateTime OnlineTime { get; set; }

	/// <summary>
	/// 最后一次下线的时间
	/// </summary>
	public DateTime OfflineTime { get; set; }

	/// <summary>
	/// 实例化一个默认的参数
	/// </summary>
	public AlienSession()
	{
		IsStatusOk = true;
		OnlineTime = DateTime.Now;
		OfflineTime = DateTime.MinValue;
	}

	/// <summary>
	/// 进行下线操作
	/// </summary>
	public void Offline()
	{
		if (IsStatusOk)
		{
			IsStatusOk = false;
			OfflineTime = DateTime.Now;
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("DtuSession[" + DTU + "] [" + (IsStatusOk ? "Online" : "Offline") + "]");
		if (IsStatusOk)
		{
			stringBuilder.Append(" [" + SoftBasic.GetTimeSpanDescription(DateTime.Now - OnlineTime) + "]");
		}
		else if (OfflineTime == DateTime.MinValue)
		{
			stringBuilder.Append(" [----]");
		}
		else
		{
			stringBuilder.Append(" [" + SoftBasic.GetTimeSpanDescription(DateTime.Now - OfflineTime) + "]");
		}
		return stringBuilder.ToString();
	}
}
