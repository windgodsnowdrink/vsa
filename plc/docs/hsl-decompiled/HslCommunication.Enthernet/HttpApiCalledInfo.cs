// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Enthernet;

/// <summary>
/// Http的Webapi接口调用的一些信息
/// Some information about the Webapi interface call for HTTP
/// </summary>
public class HttpApiCalledInfo
{
	/// <summary>
	/// 获取或设置当前的URL的名称
	/// Gets or sets the name of the current url
	/// </summary>
	public string Url { get; set; }

	/// <summary>
	/// 获取或设置当前的接口传入的数据
	/// Gets or sets the data passed in by the current interface
	/// </summary>
	public string Body { get; set; }

	/// <summary>
	/// 获取或设置当前接口的模式，GET 或是 POST
	/// Gets or sets the mode of the current interface, GET or POST
	/// </summary>
	public string HttpMethod { get; set; }

	/// <summary>
	/// 获取或设置当前接口的耗时，单位：毫秒
	/// The time elapsed to get or set the current interface in milliseconds
	/// </summary>
	public double CostTime { get; set; }

	/// <summary>
	/// 获取或设置当前接口的结果数据
	/// Gets or sets the result data for the current interface
	/// </summary>
	public string Result { get; set; }

	/// <summary>
	/// 获取或设置当前接口的总的调用次数
	/// Gets or sets the total number of calls to the current interface
	/// </summary>
	public long CalledCount { get; set; }

	public override string ToString()
	{
		return Url;
	}
}
