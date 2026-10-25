// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Profinet.Yokogawa;

/// <summary>
/// 横河PLC的通信辅助类。
/// </summary>
public class YokogawaLinkHelper
{
	/// <summary>
	/// 获取横河PLC的错误的具体描述信息
	/// </summary>
	/// <param name="code">错误码</param>
	/// <returns>错误的描述信息</returns>
	public static string GetErrorMsg(byte code)
	{
		return code switch
		{
			1 => StringResources.Language.YokogawaLinkError01, 
			2 => StringResources.Language.YokogawaLinkError02, 
			3 => StringResources.Language.YokogawaLinkError03, 
			4 => StringResources.Language.YokogawaLinkError04, 
			5 => StringResources.Language.YokogawaLinkError05, 
			6 => StringResources.Language.YokogawaLinkError06, 
			7 => StringResources.Language.YokogawaLinkError07, 
			8 => StringResources.Language.YokogawaLinkError08, 
			65 => StringResources.Language.YokogawaLinkError41, 
			66 => StringResources.Language.YokogawaLinkError42, 
			67 => StringResources.Language.YokogawaLinkError43, 
			68 => StringResources.Language.YokogawaLinkError44, 
			81 => StringResources.Language.YokogawaLinkError51, 
			82 => StringResources.Language.YokogawaLinkError52, 
			241 => StringResources.Language.YokogawaLinkErrorF1, 
			_ => StringResources.Language.UnknownError, 
		};
	}
}
