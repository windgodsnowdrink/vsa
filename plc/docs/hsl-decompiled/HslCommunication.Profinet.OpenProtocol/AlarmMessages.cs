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
/// 报警类消息功能方法
/// </summary>
public class AlarmMessages
{
	private OpenProtocolNet openProtocol;

	/// <summary>
	/// 指定Open通信类实例化一个对象
	/// </summary>
	/// <param name="openProtocol">开放协议的对象</param>
	public AlarmMessages(OpenProtocolNet openProtocol)
	{
		this.openProtocol = openProtocol;
	}

	/// <summary>
	/// A subscription for the alarms that can appear in the controller.
	/// </summary>
	/// <returns>是否订阅成功的结果对象</returns>
	public OperateResult AlarmSubscrib()
	{
		return openProtocol.ReadCustomer(70, 1, -1, -1, null);
	}

	/// <summary>
	/// Reset the subscription for the controller alarms
	/// </summary>
	/// <returns>取消订阅是否成功的结果对象</returns>
	public OperateResult AlarmUnsubscribe()
	{
		return openProtocol.ReadCustomer(73, 1, -1, -1, null);
	}

	/// <summary>
	/// The integrator can remotely acknowledge the current alarm on the controller by sending MID 0078. If no alarm is currently active when the controller receives the command, the command will be rejected.
	/// </summary>
	/// <returns>是否操作成功的结果对象</returns>
	public OperateResult AcknowledgeAlarmRemotelyOnController()
	{
		return openProtocol.ReadCustomer(78, 1, -1, -1, null);
	}
}
