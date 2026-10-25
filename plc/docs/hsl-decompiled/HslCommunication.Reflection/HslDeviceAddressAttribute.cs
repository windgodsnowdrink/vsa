// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Text;

namespace HslCommunication.Reflection;

/// <summary>
/// 应用于Hsl组件库读取的动态地址解析，具体用法为创建一个类，创建数据属性，如果这个属性需要绑定PLC的真实数据，就在属性的特性上应用本特性。
/// Applied to the dynamic address resolution read by the Hsl component library, the specific usage is to create a class and create data attributes.
/// If this attribute needs to be bound to the real data of the PLC, this feature is applied to the characteristics of the attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class HslDeviceAddressAttribute : Attribute
{
	/// <summary>
	/// 设备的类型，如果指定了特殊的PLC，那么该地址就可以支持多种不同PLC
	/// The type of equipment, if a special PLC is specified, then the address can support a variety of different PLCs
	/// </summary>
	public Type DeviceType { get; set; }

	/// <summary>
	/// 数据的地址信息，真实的设备的地址信息
	/// Data address information, real device address information
	/// </summary>
	public string Address { get; set; }

	/// <summary>
	/// 读取的数据长度
	/// Length of data read
	/// </summary>
	public int Length { get; set; }

	/// <summary>
	/// 如果关联了字符串类型的数据，则表示指定的字符编码，默认 ASCII 编码
	/// </summary>
	public string Encoding { get; set; } = "ASCII";

	/// <summary>
	/// 读取的字符串数组长度信息，仅用于字符串数组时的数组长度定义，此时 Length 输入单个字符串长度
	/// </summary>
	public int StringArrayLength { get; set; } = -1;

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public HslDeviceAddressAttribute()
	{
	}

	/// <summary>
	/// 实例化一个地址特性，指定地址信息，用于单变量的数据
	/// Instantiate an address feature, specify the address information, for single variable data
	/// </summary>
	/// <param name="address">真实的地址信息</param>
	public HslDeviceAddressAttribute(string address)
	{
		Address = address;
		Length = -1;
		DeviceType = null;
	}

	/// <summary>
	/// 实例化一个地址特性，指定地址信息，用于单变量的数据，并指定设备类型
	/// Instantiate an address feature, specify address information, data for a single variable, and specify the device type
	/// </summary>
	/// <param name="address">真实的地址信息</param>
	/// <param name="deviceType">设备的地址信息</param>
	public HslDeviceAddressAttribute(string address, Type deviceType)
	{
		Address = address;
		Length = -1;
		DeviceType = deviceType;
	}

	/// <summary>
	/// 实例化一个地址特性，指定地址信息和数据长度，通常应用于数组的批量读取
	/// Instantiate an address feature, specify address information and data length, usually used in batch reading of arrays
	/// </summary>
	/// <param name="address">真实的地址信息</param>
	/// <param name="length">读取的数据长度</param>
	/// <param name="encoding">如果是字符串类型的数据的话，就是字符串编码</param>
	public HslDeviceAddressAttribute(string address, int length)
	{
		Address = address;
		Length = length;
		DeviceType = null;
	}

	/// <summary>
	/// 实例化一个地址特性，指定地址信息和数据长度，通常应用于数组的批量读取
	/// Instantiate an address feature, specify address information and data length, usually used in batch reading of arrays
	/// </summary>
	/// <param name="address">真实的地址信息</param>
	/// <param name="length">读取的数据长度</param>
	/// <param name="encoding">如果是字符串类型的数据的话，就是字符串编码</param>
	public HslDeviceAddressAttribute(string address, int length, string encoding)
	{
		Address = address;
		Length = length;
		DeviceType = null;
		Encoding = encoding;
	}

	/// <summary>
	/// 实例化一个地址特性，指定地址信息和数据长度，通常应用于数组的批量读取，并指定设备的类型，可用于不同种类的PLC
	/// Instantiate an address feature, specify address information and data length, usually used in batch reading of arrays,
	/// and specify the type of equipment, which can be used for different types of PLCs
	/// </summary>
	/// <param name="address">真实的地址信息</param>
	/// <param name="length">读取的数据长度</param>
	/// <param name="deviceType">设备类型</param>
	/// <param name="encoding">如果是字符串类型的数据的话，就是字符串编码</param>
	public HslDeviceAddressAttribute(string address, int length, Type deviceType)
	{
		Address = address;
		Length = length;
		DeviceType = deviceType;
	}

	/// <summary>
	/// 实例化一个地址特性，指定地址信息和数据长度，通常应用于数组的批量读取，并指定设备的类型，可用于不同种类的PLC
	/// Instantiate an address feature, specify address information and data length, usually used in batch reading of arrays,
	/// and specify the type of equipment, which can be used for different types of PLCs
	/// </summary>
	/// <param name="address">真实的地址信息</param>
	/// <param name="length">读取的数据长度</param>
	/// <param name="deviceType">设备类型</param>
	/// <param name="encoding">如果是字符串类型的数据的话，就是字符串编码</param>
	public HslDeviceAddressAttribute(string address, int length, Type deviceType, string encoding)
	{
		Address = address;
		Length = length;
		DeviceType = deviceType;
		Encoding = encoding;
	}

	/// <summary>
	/// 获取数据的数量信息，如果小于0，则返回1
	/// Get the quantity information of the data, if it is less than 0, return 1
	/// </summary>
	/// <returns>数据的个数</returns>
	public int GetDataLength()
	{
		return (Length < 0) ? 1 : Length;
	}

	public override string ToString()
	{
		return $"HslDeviceAddressAttribute[{Address}:{Length}]";
	}

	/// <summary>
	/// 获取当前关联的编码信息，通常用于解析字符串的操作
	/// Gets the encoding information of the current association, usually used for string parsing operations
	/// </summary>
	/// <returns>字符编码信息</returns>
	public Encoding GetEncoding()
	{
		if (Encoding.Equals("ASCII", StringComparison.OrdinalIgnoreCase))
		{
			return System.Text.Encoding.ASCII;
		}
		if (Encoding.Equals("Unicode", StringComparison.OrdinalIgnoreCase))
		{
			return System.Text.Encoding.Unicode;
		}
		if (Encoding.Equals("BigEndianUnicode", StringComparison.OrdinalIgnoreCase))
		{
			return System.Text.Encoding.BigEndianUnicode;
		}
		if (Encoding.Equals("UTF8", StringComparison.OrdinalIgnoreCase))
		{
			return System.Text.Encoding.UTF8;
		}
		if (Encoding.Equals("ANSI", StringComparison.OrdinalIgnoreCase))
		{
			return System.Text.Encoding.Default;
		}
		if (Encoding.Equals("UTF32", StringComparison.OrdinalIgnoreCase))
		{
			return System.Text.Encoding.UTF32;
		}
		if (Encoding.Equals("UTF7", StringComparison.OrdinalIgnoreCase))
		{
			return System.Text.Encoding.UTF7;
		}
		if (Encoding.Equals("GB2312", StringComparison.OrdinalIgnoreCase))
		{
			return System.Text.Encoding.GetEncoding("gb2312");
		}
		return System.Text.Encoding.GetEncoding(Encoding);
	}
}
