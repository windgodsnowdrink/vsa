// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Core.Net;

/// <summary>
/// 用于读写的设备接口，相较于，额外增加的一个站号信息的属性，参见
/// Device interface for reading and writing, with an additional attribute for Station number information compared to , see
/// </summary>
public interface IReadWriteDeviceStation : IReadWriteDevice, IReadWriteNet
{
	/// <summary>
	/// 获取或设置当前设备站号信息，一般来说，需要在实例化之后设置本站号信息，在通信的时候也可以动态修改当前的站号信息
	/// To obtain or set the station number information of the current device, in general, the station number information needs to be set after instantiation,
	/// and the current station number information can also be dynamically modified during communication
	/// </summary>
	byte Station { get; set; }
}
