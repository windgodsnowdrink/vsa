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

namespace HslCommunication.Secs.Types;

/// <summary>
/// 扩展类
/// </summary>
public static class SecsMessageExtension
{
	/// <summary>
	/// 获取显示的字符串文本信息
	/// </summary>
	/// <param name="secsMessages">SECS消息类</param>
	/// <returns>字符串信息</returns>
	public static string ToRenderString(this SecsValue[] secsMessages)
	{
		if (secsMessages == null || secsMessages.Length == 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (SecsValue secsValue in secsMessages)
		{
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(secsValue.ToString());
		}
		return stringBuilder.ToString();
	}
}
