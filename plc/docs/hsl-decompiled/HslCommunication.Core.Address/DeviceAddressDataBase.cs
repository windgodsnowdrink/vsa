// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.Core.Address;

/// <summary>
/// 设备地址数据的信息，通常包含起始地址，数据类型，长度
/// Device address data information, usually including the starting address, data type, length
/// </summary>
public class DeviceAddressDataBase
{
	/// <summary>
	/// 数字的起始地址，也就是偏移地址
	/// The starting address of the number, which is the offset address
	/// </summary>
	public int AddressStart { get; set; }

	/// <summary>
	/// 读取的数据长度，单位是字节还是字取决于设备方
	/// The length of the data read, the unit is byte or word depends on the device side
	/// </summary>
	public ushort Length { get; set; }

	/// <summary>
	/// 从指定的地址信息解析成真正的设备地址信息
	/// Parsing from the specified address information into real device address information
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <param name="length">数据长度</param>
	public virtual void Parse(string address, ushort length)
	{
		AddressStart = int.Parse(address);
		Length = length;
	}

	public override string ToString()
	{
		return AddressStart.ToString();
	}

	/// <summary>
	/// 获取地址不支持的描述信息
	/// Get the description that the address is not supported
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <param name="ex">解析地址时候的异常信息</param>
	/// <returns>地址不支持的信息</returns>
	public static string GetUnsupportedAddressInfo(string address, Exception ex = null)
	{
		if (ex == null)
		{
			return "Address[" + address + "]: " + StringResources.Language.NotSupportedDataType;
		}
		return "Parse [" + address + "] failed: " + ex.Message;
	}
}
