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
/// 对应redis的一个列表信息的内容
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class HslRedisListItemAttribute : Attribute
{
	/// <summary>
	/// 列表键值的名称
	/// </summary>
	public string ListKey { get; set; }

	/// <summary>
	/// 当前的位置的索引
	/// </summary>
	public long Index { get; set; }

	/// <summary>
	/// 根据键名来读取写入当前的列表中的单个信息
	/// </summary>
	/// <param name="listKey">列表键名</param>
	/// <param name="index">当前的索引位置</param>
	public HslRedisListItemAttribute(string listKey, long index)
	{
		ListKey = listKey;
		Index = index;
	}
}
