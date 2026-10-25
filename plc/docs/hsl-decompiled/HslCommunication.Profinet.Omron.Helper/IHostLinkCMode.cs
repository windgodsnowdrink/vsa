// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.Core;

namespace HslCommunication.Profinet.Omron.Helper;

/// <summary>
/// HostLinkCMode协议的接口信息
/// </summary>
public interface IHostLinkCMode : IReadWriteNet
{
	/// <summary>
	/// [商业授权] 读取PLC的当前的型号信息
	/// [Authorization] Read the current model information of the PLC
	/// </summary>
	/// <param name="omron">PLC连接对象</param>
	/// <param name="unitNumber">站号信息</param>
	/// <returns>型号</returns>
	OperateResult<string> ReadPlcType();

	/// <summary>
	/// [商业授权] 读取PLC的当前的型号信息
	/// [Authorization] Read the current model information of the PLC
	/// </summary>
	/// <param name="omron">PLC连接对象</param>
	/// <param name="unitNumber">站号信息</param>
	/// <returns>型号</returns>
	OperateResult<string> ReadPlcType(byte unitNumber);

	/// <summary>
	/// [商业授权] 读取PLC当前的操作模式，0: 编程模式  1: 运行模式  2: 监视模式
	/// [Authorization] Reads the Operation mode of the CPU Unit. 0: PROGRAM mode  1: RUN mode  2: MONITOR mode
	/// </summary>
	/// <param name="omron">PLC连接对象</param>
	/// <param name="unitNumber">站号信息</param>
	/// <returns>0: 编程模式  1: 运行模式  2: 监视模式</returns>
	OperateResult<int> ReadPlcMode();

	/// <summary>
	/// [商业授权] 读取PLC当前的操作模式，0: 编程模式  1: 运行模式  2: 监视模式
	/// [Authorization] Reads the Operation mode of the CPU Unit. 0: PROGRAM mode  1: RUN mode  2: MONITOR mode
	/// </summary>
	/// <param name="omron">PLC连接对象</param>
	/// <param name="unitNumber">站号信息</param>
	/// <returns>0: 编程模式  1: 运行模式  2: 监视模式</returns>
	OperateResult<int> ReadPlcMode(byte unitNumber);

	/// <summary>
	/// [商业授权] 将当前PLC的模式变更为指定的模式，0: 编程模式  1: 运行模式  2: 监视模式
	/// [Authorization] Change the current PLC mode to the specified mode, 0: programming mode 1: running mode 2: monitoring mode
	/// </summary>
	/// <param name="omron">PLC连接对象</param>
	/// <param name="unitNumber">站号信息</param>
	/// <param name="mode">0: 编程模式  1: 运行模式  2: 监视模式</param>
	/// <returns>是否变更成功</returns>
	OperateResult ChangePlcMode(byte mode);

	/// <summary>
	/// [商业授权] 将当前PLC的模式变更为指定的模式，0: 编程模式  1: 运行模式  2: 监视模式
	/// [Authorization] Change the current PLC mode to the specified mode, 0: programming mode 1: running mode 2: monitoring mode
	/// </summary>
	/// <param name="omron">PLC连接对象</param>
	/// <param name="unitNumber">站号信息</param>
	/// <param name="mode">0: 编程模式  1: 运行模式  2: 监视模式</param>
	/// <returns>是否变更成功</returns>
	OperateResult ChangePlcMode(byte unitNumber, byte mode);
}
