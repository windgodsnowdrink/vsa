// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.Profinet.OpenProtocol;

/// <summary>
/// JobItem
/// </summary>
public class JobItem
{
	/// <summary>
	/// Channel-ID
	/// </summary>
	public int ChannelID { get; set; }

	/// <summary>
	/// Type-ID
	/// </summary>
	public int TypeID { get; set; }

	/// <summary>
	/// AutoValue
	/// </summary>
	public int AutoValue { get; set; }

	/// <summary>
	/// BatchSize
	/// </summary>
	public int BatchSize { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public JobItem()
	{
	}

	/// <summary>
	/// 指定原始数据实例化一个对象信息
	/// </summary>
	/// <param name="data">等待分析的原始数据，例如：15:011:0:22</param>
	public JobItem(string data)
	{
		if (data.Length == 12)
		{
			data = data.Substring(0, 11);
		}
		string[] array = data.Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);
		ChannelID = Convert.ToInt32(array[0]);
		TypeID = Convert.ToInt32(array[1]);
		AutoValue = Convert.ToInt32(array[2]);
		BatchSize = Convert.ToInt32(array[3]);
	}
}
