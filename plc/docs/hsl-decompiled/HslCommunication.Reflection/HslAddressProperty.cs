// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Reflection;

namespace HslCommunication.Reflection;

/// <summary>
/// Hsl相关地址的属性信息
/// </summary>
public class HslAddressProperty
{
	/// <summary>
	/// 该属性绑定的地址特性
	/// </summary>
	public HslDeviceAddressAttribute DeviceAddressAttribute { get; set; }

	/// <summary>
	/// 地址绑定的属性信息
	/// </summary>
	public PropertyInfo PropertyInfo { get; set; }

	/// <summary>
	/// 起始的字节偏移信息
	/// </summary>
	public int ByteOffset { get; set; }

	/// <summary>
	/// 读取的字节的长度信息
	/// </summary>
	public int ByteLength { get; set; }

	/// <summary>
	/// 缓存的数据对象
	/// </summary>
	public byte[] Buffer { get; set; }
}
