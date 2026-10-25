// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Profinet.OpenProtocol;

/// <summary>
/// 拧紧结果消息
/// </summary>
public class TighteningResultMessages
{
	private OpenProtocolNet openProtocol;

	/// <summary>
	/// 指定Open通信类实例化一个对象
	/// </summary>
	/// <param name="openProtocol">开放协议的对象</param>
	public TighteningResultMessages(OpenProtocolNet openProtocol)
	{
		this.openProtocol = openProtocol;
	}

	/// <summary>
	/// Set the subscription for the result tightenings. The result of this command will be the transmission of the tightening result after the tightening is performed( push function ).
	/// </summary>
	/// <param name="revision">Revision</param>
	/// <returns>是否订阅成功的结果对象</returns>
	/// <remarks>
	/// The MID revision in the header is used to subscribe to different revisions of MID 0061 Last tightening result data upload reply.
	/// </remarks>
	public OperateResult LastTighteningResultDataSubscribe(int revision)
	{
		return openProtocol.ReadCustomer(60, revision, -1, -1, null);
	}

	/// <summary>
	/// Reset the last tightening result subscription.
	/// </summary>
	/// <returns>取消订阅是否成功的结果对象</returns>
	public OperateResult LastTighteningResultDataUnsubscribe()
	{
		return openProtocol.ReadCustomer(63, 1, -1, -1, null);
	}
}
