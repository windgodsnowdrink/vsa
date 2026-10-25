// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.Secs.Types;

internal class TypeHelper
{
	/// <summary>
	/// 检查类型  的类型是否为 ，如果不是，就抛出异常
	/// </summary>
	/// <param name="secsItem">等待转换的对象</param>
	public static void TypeListCheck(SecsValue secsItem)
	{
		if (secsItem.ItemType != SecsItemType.List)
		{
			throw new InvalidCastException($"Current type must be List, but now is {secsItem.ItemType} {Environment.NewLine} Source: {secsItem.ToXElement()}");
		}
	}
}
