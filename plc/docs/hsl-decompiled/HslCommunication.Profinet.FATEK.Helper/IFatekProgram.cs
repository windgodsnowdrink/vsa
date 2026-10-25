// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.Core;

namespace HslCommunication.Profinet.FATEK.Helper;

/// <summary>
/// FatekProgram协议的接口
/// </summary>
public interface IFatekProgram : IReadWriteNet
{
	/// <summary>
	/// 使PLC处于RUN的状态
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>是否操作成功</returns>
	OperateResult Run(byte station);

	/// <summary>
	/// 使PLC处于RUN的状态
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>是否操作成功</returns>
	OperateResult Run();

	/// <summary>
	/// 使PLC处于STOP状态
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>是否操作成功</returns>
	OperateResult Stop(byte station);

	/// <summary>
	/// 使PLC处于STOP状态
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>是否操作成功</returns>
	OperateResult Stop();

	/// <summary>
	/// 读取当前PLC的状态信息，返回一个bool数组，同时包含了几种电量信息，分别为 0: RUN/STOP, 1: BAT LOW/正常, 2: Ladder checksum error/正常, 3: 使用ROM PACK/未使用,
	/// 4: WDT Timeout/正常, 5: 设定ID/未设ID， 6： 紧急停机/正常
	/// Read the status information of the current PLC and return a bool array, which also contains several power information, 0: RUN/STOP, 1: BAT LOW/normal,
	/// 2: Ladder checksum error/normal, 3: Use ROM PACK/ Not used, 4: WDT Timeout/Normal, 5: ID set/ID not set, 6: Emergency stop/Normal
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>状态结果信息</returns>
	OperateResult<bool[]> ReadStatus(byte station);

	/// <summary>
	/// 读取当前PLC的状态信息，返回一个bool数组，同时包含了几种电量信息，分别为 0: RUN/STOP, 1: BAT LOW/正常, 2: Ladder checksum error/正常, 3: 使用ROM PACK/未使用,
	/// 4: WDT Timeout/正常, 5: 设定ID/未设ID， 6： 紧急停机/正常
	/// Read the status information of the current PLC and return a bool array, which also contains several power information, 0: RUN/STOP, 1: BAT LOW/normal,
	/// 2: Ladder checksum error/normal, 3: Use ROM PACK/ Not used, 4: WDT Timeout/Normal, 5: ID set/ID not set, 6: Emergency stop/Normal
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>状态结果信息</returns>
	OperateResult<bool[]> ReadStatus();
}
