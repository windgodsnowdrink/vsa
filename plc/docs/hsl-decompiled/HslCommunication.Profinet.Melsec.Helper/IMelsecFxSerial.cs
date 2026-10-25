// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Threading.Tasks;
using HslCommunication.Core;

namespace HslCommunication.Profinet.Melsec.Helper;

/// <summary>
/// 三菱的串口的接口类对象
/// </summary>
public interface IMelsecFxSerial : IReadWriteNet
{
	/// <summary>
	/// 激活PLC的接收状态，需要再和PLC交互之前进行调用，之后就需要再调用了。
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <returns>是否激活成功</returns>
	OperateResult ActivePlc();

	/// <summary>
	/// 激活PLC的接收状态，需要再和PLC交互之前进行调用，之后就需要再调用了。
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <returns>是否激活成功</returns>
	Task<OperateResult> ActivePlcAsync();
}
