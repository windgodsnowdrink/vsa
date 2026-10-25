// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.Core;
using HslCommunication.Core.Address;

namespace HslCommunication.Profinet.Melsec.Helper;

/// <summary>
/// 三菱的FxLink协议接口的设备信息
/// </summary>
public interface IReadWriteFxLinks : IReadWriteDevice, IReadWriteNet
{
	/// <summary>
	/// PLC的当前的站号，需要根据实际的值来设定，默认是0
	/// The current station number of the PLC needs to be set according to the actual value. The default is 0.
	/// </summary>
	byte Station { get; set; }

	/// <summary>
	/// 报文等待时间，单位10ms，设置范围为0-15
	/// Message waiting time, unit is 10ms, setting range is 0-15
	/// </summary>
	byte WaittingTime { get; set; }

	/// <summary>
	/// 是否启动和校验
	/// Whether to start and sum verify
	/// </summary>
	bool SumCheck { get; set; }

	/// <summary>
	/// 当前的PLC的Fxlinks协议格式，通常是格式1，或是格式4，所以此处可以设置1，或者是4
	/// The current PLC Fxlinks protocol format is usually format 1 or format 4, so it can be set to 1 or 4 here
	/// </summary>
	int Format { get; set; }

	/// <summary>
	/// 分析输入的地址，如果地址合法，转换成实际的 对象
	/// </summary>
	/// <param name="address">输入的地址</param>
	/// <returns>实际的三菱地址对象</returns>
	OperateResult<MelsecFxLinksAddress> AnalyzeAddress(string address);

	/// <summary>
	/// [商业授权] 启动PLC的操作，可以携带额外的参数信息，指定站号。举例：s=2; 注意：分号是必须的。
	/// [Authorization] Start the PLC operation, you can carry additional parameter information and specify the station number. Example: s=2; Note: The semicolon is required.
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="parameter">允许携带的参数信息，例如s=2; 也可以为空</param>
	/// <returns>是否启动成功</returns>
	OperateResult StartPLC(string parameter = "");

	/// <summary>
	/// [商业授权] 停止PLC的操作，可以携带额外的参数信息，指定站号。举例：s=2; 注意：分号是必须的。
	/// [Authorization] Stop PLC operation, you can carry additional parameter information and specify the station number. Example: s=2; Note: The semicolon is required.
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="parameter">允许携带的参数信息，例如s=2; 也可以为空</param>
	/// <returns>是否停止成功</returns>
	OperateResult StopPLC(string parameter = "");

	/// <summary>
	/// [商业授权] 读取PLC的型号信息，可以携带额外的参数信息，指定站号。举例：s=2; 注意：分号是必须的。
	/// [Authorization] Read the PLC model information, you can carry additional parameter information, and specify the station number. Example: s=2; Note: The semicolon is required.
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="parameter">允许携带的参数信息，例如s=2; 也可以为空</param>
	/// <returns>带PLC型号的结果信息</returns>
	OperateResult<string> ReadPlcType(string parameter = "");
}
