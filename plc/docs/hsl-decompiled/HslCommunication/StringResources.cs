// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Globalization;
using HslCommunication.Language;

namespace HslCommunication;

/// <summary>
/// 系统的字符串资源及多语言管理中心
/// System string resource and multi-language management Center
/// </summary>
public static class StringResources
{
	/// <summary>
	/// 获取或设置系统的语言选项
	/// Gets or sets the language options for the system
	/// </summary>
	public static DefaultLanguage Language;

	static StringResources()
	{
		Language = new DefaultLanguage();
		if (CultureInfo.CurrentCulture.ToString().StartsWith("zh"))
		{
			SetLanguageChinese();
		}
		else
		{
			SeteLanguageEnglish();
		}
	}

	/// <summary>
	/// 将语言设置为中文
	/// Set the language to Chinese
	/// </summary>
	public static void SetLanguageChinese()
	{
		Language = new DefaultLanguage();
	}

	/// <summary>
	/// 将语言设置为英文
	/// Set the language to English
	/// </summary>
	public static void SeteLanguageEnglish()
	{
		Language = new English();
	}
}
