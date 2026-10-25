// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.Core;

namespace HslCommunication.CNC.Fanuc;

/// <summary>
/// 刀具信息
/// </summary>
public class ToolInformation
{
	/// <summary>
	/// 当前刀具的寿命
	/// </summary>
	public int Life { get; set; }

	/// <summary>
	/// 当前刀具的使用次数
	/// </summary>
	public int Use { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public ToolInformation()
	{
	}

	/// <summary>
	/// 指定内存数据来初始化对象
	/// </summary>
	/// <param name="content">机床返回的数据</param>
	/// <param name="byteTransform">字节变换规则</param>
	public ToolInformation(byte[] content, IByteTransform byteTransform)
	{
		Life = byteTransform.TransInt32(content, 26);
		Use = byteTransform.TransInt32(content, 34);
	}

	public override string ToString()
	{
		return $"Life: {Life}, Use: {Use}";
	}
}
