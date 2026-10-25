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
/// HostLink的接口实现
/// </summary>
public interface IHostLink : IReadWriteDevice, IReadWriteNet
{
	/// <summary>
	/// Specifies whether or not there are network relays. Set “80” (ASCII: 38,30)
	/// when sending an FINS command to a CPU Unit on a network.Set “00” (ASCII: 30,30)
	/// when sending to a CPU Unit connected directly to the host computer.
	/// </summary>
	byte ICF { get; set; }

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
	/// The response wait time sets the time from when the CPU Unit receives a command block until it starts
	/// to return a response.It can be set from 0 to F in hexadecimal, in units of 10 ms.
	/// If F(15) is set, the response will begin to be returned 150 ms (15 × 10 ms) after the command block was received.
	/// </summary>
	byte ResponseWaitTime { get; set; }

	/// <summary>
	/// PLC设备的站号信息
	/// PLC device station number information
	/// </summary>
	byte UnitNumber { get; set; }

	/// <summary>
	/// 进行字读取的时候对于超长的情况按照本属性进行切割，默认260。
	/// When reading words, it is cut according to this attribute for the case of overlength. The default is 260.
	/// </summary>
	int ReadSplits { get; set; }

	/// <summary>
	/// 获取或设置欧姆龙PLC的系列信息
	/// Obtain or set the series information of Omron PLC
	/// </summary>
	OmronPlcType PlcType { get; set; }
}
