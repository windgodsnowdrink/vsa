// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Core;

/// <summary>
/// 大端顺序的字节的转换类，字节的顺序和C#的原生字节的顺序是完全相反的，高字节在前，低字节在后。
/// In the reverse byte order conversion class, the byte order is completely opposite to the native byte order of C#,
/// with the high byte first and the low byte following.
/// </summary>
/// <remarks>
/// 适用西门子PLC的S7协议的数据转换
/// </remarks>
public class ReverseBytesTransform : RegularByteTransform
{
	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public ReverseBytesTransform()
	{
		base.DataFormat = DataFormat.ABCD;
	}

	/// <summary>
	/// 使用指定的数据解析来实例化对象
	/// Instantiate the object using the specified data parsing
	/// </summary>
	/// <param name="dataFormat">数据规则</param>
	public ReverseBytesTransform(DataFormat dataFormat)
		: base(dataFormat)
	{
	}

	/// <summary>
	/// 根据指定的格式，来实例化一个新的对象，除了不同，其他都相同
	/// According to the specified  format, to instantiate a new object, except that  is different, everything else is the same
	/// </summary>
	/// <param name="dataFormat">数据格式</param>
	/// <returns>新的对象</returns>
	public override IByteTransform CreateByDateFormat(DataFormat dataFormat)
	{
		return new ReverseBytesTransform(dataFormat)
		{
			IsStringReverseByteWord = base.IsStringReverseByteWord
		};
	}

	public override string ToString()
	{
		return $"ReverseBytesTransform[{base.DataFormat}]";
	}
}
