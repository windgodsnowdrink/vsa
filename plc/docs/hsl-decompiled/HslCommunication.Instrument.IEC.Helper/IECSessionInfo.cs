// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Threading;

namespace HslCommunication.Instrument.IEC.Helper;

/// <summary>
/// IEC会话的额外信息，主要包含了这个会话的收发消息ID
/// </summary>
public class IECSessionInfo
{
	private int sendMessageID = 0;

	private int recvMessageID = 0;

	public int RecvMessageID => recvMessageID;

	/// <summary>
	/// 获取发送的消息ID信息，并进行自增操作
	/// </summary>
	public int GetSendMessageID()
	{
		int result = sendMessageID;
		Interlocked.Increment(ref sendMessageID);
		return result;
	}

	/// <summary>
	/// 自增一个接收到的消息ID
	/// </summary>
	public int IncrRecvMessageID()
	{
		int result = Interlocked.Increment(ref recvMessageID);
		if (recvMessageID > 32767)
		{
			recvMessageID = 0;
		}
		return result;
	}
}
