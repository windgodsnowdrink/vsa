// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.Core;

namespace HslCommunication.Profinet.Siemens.Helper;

/// <summary>
/// 西门子PPI的公用接口信息
/// </summary>
public interface ISiemensPPI : IReadWriteNet
{
	/// <summary>
	/// 启动西门子PLC为RUN模式，参数信息可以携带站号信息 "s=2;", 注意，分号是必须的。
	/// Start Siemens PLC in RUN mode, parameter information can carry station number information "s=2;", note that the semicolon is required.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="parameter">额外的参数信息，例如可以携带站号信息 &quot;s=2;&quot;, 注意，分号是必须的。</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="communicationLock">当前的同通信锁</param>
	/// <returns>是否启动成功</returns>
	OperateResult Start(string parameter = "");

	/// <summary>
	/// 停止西门子PLC，切换为Stop模式，参数信息可以携带站号信息 "s=2;", 注意，分号是必须的。
	/// Stop Siemens PLC and switch to Stop mode, parameter information can carry station number information "s=2;", note that the semicolon is required.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="parameter">额外的参数信息，例如可以携带站号信息 &quot;s=2;&quot;, 注意，分号是必须的。</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="communicationLock">当前的同通信锁</param>
	/// <returns>是否停止成功</returns>
	OperateResult Stop(string parameter = "");

	/// <summary>
	/// 读取西门子PLC的型号信息，参数信息可以携带站号信息 "s=2;", 注意，分号是必须的。
	/// Read the model information of Siemens PLC, the parameter information can carry the station number information "s=2;", note that the semicolon is required.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="parameter">额外的参数信息，例如可以携带站号信息 &quot;s=2;&quot;, 注意，分号是必须的。</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="communicationLock">当前的同通信锁</param>
	/// <returns>包含是否成功的结果对象</returns>
	OperateResult<string> ReadPlcType(string parameter = "");
}
