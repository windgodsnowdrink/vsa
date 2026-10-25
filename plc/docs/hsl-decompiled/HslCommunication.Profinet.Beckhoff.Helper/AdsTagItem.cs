// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Profinet.Beckhoff.Helper;

/// <summary>
/// Ads标签信息
/// </summary>
public class AdsTagItem
{
	/// <summary>
	/// 标签的名称
	/// </summary>
	public string TagName { get; set; }

	/// <summary>
	/// 标签的数据缓存信息
	/// </summary>
	public byte[] Buffer { get; set; }

	/// <summary>
	/// 绝对地址信息
	/// </summary>
	public uint Location { get; set; }

	/// <summary>
	/// 类型的单位字节长度信息
	/// </summary>
	public int TypeLength { get; private set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	/// <param name="name">标签名</param>
	/// <param name="buffer">缓存的数据对象</param>
	/// <param name="typeLength">类型的单位字节长度信息</param>
	public AdsTagItem(string name, byte[] buffer, int typeLength)
	{
		TagName = name;
		Buffer = buffer;
		TypeLength = typeLength;
	}
}
