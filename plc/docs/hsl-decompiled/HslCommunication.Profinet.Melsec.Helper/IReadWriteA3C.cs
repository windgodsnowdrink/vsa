// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.Core;

namespace HslCommunication.Profinet.Melsec.Helper;

/// <summary>
/// 三菱的A3C协议类接口对象，具有站号，是否和校验的属性
/// Mitsubishi's A3C protocol interface object, which has the attributes of station number, and checksum
/// </summary>
public interface IReadWriteA3C : IReadWriteDevice, IReadWriteNet
{
	/// <summary>
	/// 当前A3C协议的站编号信息
	/// Station number information of the current A3C protocol
	/// </summary>
	byte Station { get; set; }

	/// <summary>
	/// 当前的A3C协议是否使用和校验，默认使用
	/// Whether the current A3C protocol uses sum check, it is used by default
	/// </summary>
	bool SumCheck { get; set; }

	/// <summary>
	/// 当前的A3C协议的格式信息，可选格式1，2，3，4，默认格式1
	/// Format information of the current A3C protocol, optional format 1, 2, 3, 4, default format 1
	/// </summary>
	int Format { get; set; }

	/// <summary>
	/// 是否开启支持写入位到字寄存器的功能，该功能先读取字寄存器的字数据，然后修改其中的位，再写入回去，可能存在脏数据的风险
	/// Whether to enable supporting the function of writing bit-to-word registers, which first reads the word data of the word register, then modifies the bits in it, and then writes back, there may be a risk of dirty data
	/// </summary>
	/// <remarks>
	/// 关于脏数据风险：从读取数据，修改位，再次写入数据时，大概需要经过3ms~10ms不等的时间，如果此期间内PLC修改了该字寄存器的其他位，再次写入数据时会恢复该点位的数据到读取时的初始值，可能引发设备故障，请谨慎开启此功能。
	/// About dirty data risk: from reading data, modifying bits, writing data again, it takes about 3ms ~ 10ms to pass a time, if the PLC modifies other bits of the word register during this period, when writing data again,
	/// it will restore the data at the point to the initial value when reading, which may cause equipment failure, please turn on this function carefully.
	/// </remarks>
	bool EnableWriteBitToWordRegister { get; set; }

	/// <summary>
	/// 0表示 3C 协议，1表示4C协议
	/// </summary>
	int McMode { get; }
}
