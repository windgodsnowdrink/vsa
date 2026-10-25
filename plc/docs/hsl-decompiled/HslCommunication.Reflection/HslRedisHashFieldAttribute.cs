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
/// 对应redis的一个哈希信息的内容
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class HslRedisHashFieldAttribute : Attribute
{
	/// <summary>
	/// 哈希键值的名称
	/// </summary>
	public string HaskKey { get; set; }

	/// <summary>
	/// 当前的哈希域名称
	/// </summary>
	public string Field { get; set; }

	/// <summary>
	/// 根据键名来读取写入当前的哈希的单个信息
	/// </summary>
	/// <param name="hashKey">哈希键名</param>
	/// <param name="filed">哈希域名称</param>
	public HslRedisHashFieldAttribute(string hashKey, string filed)
	{
		HaskKey = hashKey;
		Field = filed;
	}
}
