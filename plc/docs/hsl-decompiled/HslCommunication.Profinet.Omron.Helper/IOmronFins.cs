// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using HslCommunication.Core;

namespace HslCommunication.Profinet.Omron.Helper;

/// <summary>
/// Fins协议的接口对象
/// </summary>
public interface IOmronFins : IReadWriteDevice, IReadWriteNet
{
	/// <summary>
	/// 信息控制字段，默认0x80
	/// Information control field, default 0x80
	/// </summary>
	byte ICF { get; set; }

	/// <summary>
	/// 系统使用的内部信息
	/// Internal information used by the system
	/// </summary>
	byte RSV { get; }

	/// <summary>
	/// 网络层信息，默认0x02，如果有八层消息，就设置为0x07
	/// Network layer information, default is 0x02, if there are eight layers of messages, set to 0x07
	/// </summary>
	byte GCT { get; set; }

	/// <summary>
	/// PLC的网络号地址，默认0x00
	/// PLC network number address, default 0x00
	/// </summary>
	/// <remarks>
	/// 00: Local network
	/// 01-7F: Remote network address (decimal: 1 to 127)
	/// </remarks>
	byte DNA { get; set; }

	/// <summary>
	/// PLC的节点地址，默认为0，在和PLC连接的过程中，自动从PLC获取到DA1的值。
	/// The node address of the PLC is 0 by default. During the process of connecting with the PLC, the value of DA1 is automatically obtained from the PLC.
	/// </summary>
	byte DA1 { get; set; }

	/// <summary>
	/// PLC的单元号地址，通常都为0
	/// PLC unit number address, usually 0
	/// </summary>
	/// <remarks>
	/// 00: CPU Unit
	/// FE: Controller Link Unit or Ethernet Unit connected to network
	/// 10 TO 1F: CPU Bus Unit
	/// E1: Inner Board
	/// </remarks>
	byte DA2 { get; set; }

	/// <summary>
	/// 上位机的网络号地址
	/// Network number and address of the computer
	/// </summary>
	/// <remarks>
	/// 00: Local network
	/// 01-7F: Remote network (1 to 127 decimal)
	/// </remarks>
	byte SNA { get; set; }

	/// <summary>
	/// 上位机的节点地址，默认是0x01，当连接PLC之后，将由PLC来设定当前的值。
	/// The node address of the host computer is 0x01 by default. After connecting to the PLC, the PLC will set the current value.
	/// </summary>
	/// <remarks>
	/// v9.6.5版本及之前的版本都需要手动设置，如果是多连接，相同的节点是连接不上PLC的。
	/// </remarks>
	byte SA1 { get; set; }

	/// <summary>
	/// 上位机的单元号地址
	/// Unit number and address of the computer
	/// </summary>
	/// <remarks>
	/// 00: CPU Unit
	/// 10-1F: CPU Bus Unit
	/// </remarks>
	byte SA2 { get; set; }

	/// <summary>
	/// 服务的标识号，由客户端生成自增的顺序值，用来标识和校验通信报文的ID
	/// Service ID. Used to identify the process generating the transmission. Set the SID to any number between 00 and FF
	/// </summary>
	byte SID { get; set; }

	/// <summary>
	/// 进行字读取的时候对于超长的情况按照本属性进行切割，默认500，如果不是CP1H及扩展模块的，可以设置为999，可以提高一倍的通信速度。
	/// When reading words, it is cut according to this attribute for the case of overlength. The default is 500.
	/// If it is not for CP1H and expansion modules, it can be set to 999, which can double the communication speed.
	/// </summary>
	int ReadSplits { get; set; }

	/// <summary>
	/// 获取或设置欧姆龙PLC的系列信息
	/// Obtain or set the series information of Omron PLC
	/// </summary>
	OmronPlcType PlcType { get; set; }

	/// <summary>
	/// 将CPU单元的操作模式更改为RUN，从而使PLC能够执行其程序。
	/// Changes the CPU Unit’s operating mode to RUN, enabling the PLC to execute its program.
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <returns>是否启动成功</returns>
	/// <remarks>
	/// 当执行RUN时，CPU单元将开始运行。 在执行RUN之前，您必须确认系统的安全性。 启用“禁止覆盖受保护程序”设置时，无法执行此命令。
	/// The CPU Unit will start operation when RUN is executed. You must confirm the safety of the system before executing RUN.
	/// When the “prohibit overwriting of protected program” setting is enabled, this command cannot be executed.
	/// </remarks>
	OperateResult Run();

	/// <summary>
	/// 将CPU单元的操作模式更改为PROGRAM，停止程序执行。
	/// Changes the CPU Unit’s operating mode to PROGRAM, stopping program execution.
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <returns>是否停止成功</returns>
	/// <remarks>
	/// 当执行STOP时，CPU单元将停止操作。 在执行STOP之前，您必须确认系统的安全性。
	/// The CPU Unit will stop operation when STOP is executed. You must confirm the safety of the system before executing STOP.
	/// </remarks>
	OperateResult Stop();

	/// <summary>
	/// [商业授权] 读取CPU的一些数据信息，主要包含型号，版本，一些数据块的大小
	/// [Authorization] Read some data information of the CPU, mainly including the model, version, and the size of some data blocks
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <returns>是否读取成功</returns>
	OperateResult<OmronCpuUnitData> ReadCpuUnitData();

	/// <summary>
	/// [商业授权] 读取CPU单元的一些操作状态数据，主要包含运行状态，工作模式，错误信息等。
	/// [Authorization] Read some operating status data of the CPU unit, mainly including operating status, working mode, error information, etc.
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <returns>是否读取成功</returns>
	OperateResult<OmronCpuUnitStatus> ReadCpuUnitStatus();

	/// <summary>
	/// [商业授权] 读取CPU的时间信息
	/// [Authorization] Read the time information of the CPU
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <returns>是否读取成功</returns>
	OperateResult<DateTime> ReadCpuTime();
}
