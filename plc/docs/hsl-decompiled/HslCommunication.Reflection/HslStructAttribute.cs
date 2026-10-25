// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.Reflection;

/// <summary>
/// 结构体的字节偏移信息定义，用于方法 读取字节数据并实现解析操作的
/// The byte offset information of the structure is defined, Method used for  read byte data and implement the parsing operation
/// </summary>
public class HslStructAttribute : Attribute
{
	/// <summary>
	/// 字节偏移字节信息，如果是bool，就是位偏移地址，按照位为单位
	/// Byte offset Indicates the byte information. If the value is bool, it is the address of the bit offset. The unit is bits
	/// </summary>
	public int Index { get; set; }

	/// <summary>
	/// 长度信息，如果是普通类型，则表示数组，如果是字符串，则表示字符串占用的最大字节长度
	/// Length information, if a common type, represents an array, if a string, represents the maximum length of bytes taken by the string
	/// </summary>
	public int Length { get; set; }

	/// <summary>
	/// 编码信息，如果是字符串类型，则表示字符串的编码信息，可选 ASCII UNICODE UTF8 GB2312 ANSI BIG-UNICODE
	/// Encoding information. If it is a string, it indicates the encoding information of a string. The option is ASCII UNICODE UTF8 GB2312 ANSI BIG-UNICODE
	/// </summary>
	public string Encoding { get; set; }

	/// <summary>
	/// 字符串的解析模式，0：默认没有字节头 1: 一个头子节，表示长度，2: 两个头子节，最大字符数和实际字符数，3: 两个字节头，实际字符数，4: 四个头字节，最大字符数和实际字符数，5: 四个头字节，字符串实际长度
	/// Parsing mode of the string, 0: default byte header 1: one head section, indicating the length, 2: two head sections, maximum number of characters and actual number of characters,
	/// 3: two head bytes, actual number of characters, 4: four head bytes, maximum number of characters and actual number of characters, 5: four head bytes, actual length of the string
	/// </summary>
	public int StringMode { get; set; }

	/// <summary>
	/// bool类型的解析模式，0: 默认字节顺序 1: 按字反转之后再解析bool值
	/// </summary>
	public int BoolMode { get; set; }

	/// <summary>
	/// 指定偏移地址来实例化一个对象
	/// Specify an offset address to instantiate an object
	/// </summary>
	/// <param name="index">字节偏移字节信息，如果是bool，就是位偏移地址，按照位为单位</param>
	public HslStructAttribute(int index)
	{
		Index = index;
	}

	/// <summary>
	/// 指定偏移地址和长度信息来实例化一个对象
	/// Specify offset address and length information to instantiate an object
	/// </summary>
	/// <param name="index">字节偏移字节信息，如果是bool，就是位偏移地址，按照位为单位</param>
	/// <param name="length">长度信息，如果是普通类型，则表示数组，如果是字符串，则表示字符串占用的最大字节长度</param>
	public HslStructAttribute(int index, int length)
		: this(index)
	{
		Length = length;
	}

	/// <summary>
	/// 指定偏移地址和长度信息，bool解析模式来实例化一个对象
	/// Specify offset address and length information to instantiate an object
	/// </summary>
	/// <param name="index">字节偏移字节信息，如果是bool，就是位偏移地址，按照位为单位</param>
	/// <param name="length">长度信息，如果是普通类型，则表示数组</param>
	/// <param name="boolMode">bool类型解析模式, 0: 默认字节顺序 1: 按字反转之后再解析bool值</param>
	public HslStructAttribute(int index, int length, int boolMode)
		: this(index, length)
	{
		BoolMode = boolMode;
	}

	/// <summary>
	/// 指定偏移地址，长度信息，编码信息来实例化一个对象，通常应用于字符串数据
	/// Specify offset address, length information, and encoding information to instantiate an object, usually applied to string data
	/// </summary>
	/// <param name="index">字节偏移字节信息，如果是bool，就是位偏移地址，按照位为单位</param>
	/// <param name="length">长度信息，如果是普通类型，则表示数组，如果是字符串，则表示字符串占用的最大字节长度</param>
	/// <param name="encoding">编码信息，如果是字符串类型，则表示字符串的编码信息，可选 ASCII UNICODE UTF8 GB2312 ANSI BIG-UNICODE</param>
	public HslStructAttribute(int index, int length, string encoding)
		: this(index, length)
	{
		Encoding = encoding;
	}

	/// <summary>
	/// 指定偏移地址，长度信息，编码信息，字符串解析模式来实例化一个对象，通常应用于字符串数据
	/// Specify offset address, length information, encoding information, string parsing mode to instantiate an object, usually applied to string data
	/// </summary>
	/// <param name="index">字节偏移字节信息，如果是bool，就是位偏移地址，按照位为单位</param>
	/// <param name="length">长度信息，如果是普通类型，则表示数组，如果是字符串，则表示字符串占用的最大字节长度</param>
	/// <param name="encoding">编码信息，如果是字符串类型，则表示字符串的编码信息，可选 ASCII UNICODE UTF8 GB2312 ANSI BIG-UNICODE</param>
	/// <param name="mode">字符串的解析模式，0：默认没有字节头 1: 一个头子节，表示长度，2: 两个头子节，最大字符数和实际字符数，3: 两个字节头，实际字符数，4: 四个头字节，字符串实际长度</param>
	public HslStructAttribute(int index, int length, string encoding, int mode)
		: this(index, length, encoding)
	{
		StringMode = mode;
	}
}
