// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Profinet.AllenBradley;

/// <summary>
/// AB标签的数据信息
/// </summary>
public class AbTagValue
{
	/// <summary>
	/// 标签的数据类型
	/// </summary>
	public ushort Type { get; set; }

	/// <summary>
	/// 标签的数据内容
	/// </summary>
	public byte[] Value { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public AbTagValue()
	{
	}

	/// <summary>
	/// 指定标签的名称，数据类型，数据内容来实例化一个对象
	/// </summary>
	public AbTagValue(ushort type, byte[] value)
	{
		Type = type;
		Value = value;
	}
}
