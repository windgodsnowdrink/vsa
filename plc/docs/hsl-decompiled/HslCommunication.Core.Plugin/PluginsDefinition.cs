// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.Core.Plugin;

internal class PluginsDefinition
{
	public string DllName { get; set; }

	public string Company { get; set; }

	public string Description { get; set; }

	public string Http { get; set; }

	public string Framework { get; set; }

	public string Version { get; set; }

	public byte[] Icon16 { get; set; }

	public DateTime ReleaseData { get; set; }

	/// <summary>
	/// 获取插件类型的字符串信息，如果没有定义的话，就使用dll本身的名字作为插件类型信息
	/// </summary>
	/// <returns>字符串内容</returns>
	public string GetPluginTypeString()
	{
		if (DllName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
		{
			return DllName.Substring(0, DllName.Length - 4);
		}
		return DllName;
	}
}
