// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.BasicFramework;

/// <summary>
/// 支持字符串信息加载存储的接口，定义了几个通用的方法
/// </summary>
public interface ISoftFileSaveBase
{
	/// <summary>
	/// 文件路径的存储
	/// </summary>
	string FileSavePath { get; set; }

	/// <summary>
	/// 获取需要保存的数据，需要重写实现
	/// </summary>
	/// <returns>需要存储的信息</returns>
	string ToSaveString();

	/// <summary>
	/// 从字符串加载数据，需要重写实现
	/// </summary>
	/// <param name="content">字符串数据</param>
	void LoadByString(string content);

	/// <summary>
	/// 不使用解密方法从文件读取数据
	/// </summary>
	void LoadByFile();

	/// <summary>
	/// 不使用加密方法保存数据到文件
	/// </summary>
	void SaveToFile();
}
