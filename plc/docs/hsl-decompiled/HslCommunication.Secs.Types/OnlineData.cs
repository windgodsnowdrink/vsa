// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Secs.Types;

/// <summary>
/// 在线数据信息
/// </summary>
public class OnlineData
{
	/// <summary>
	/// equipment model type
	/// </summary>
	public string ModelType { get; set; }

	/// <summary>
	/// software revision
	/// </summary>
	public string SoftVersion { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public OnlineData()
	{
	}

	/// <summary>
	/// 指定类型及其版本号来实例化一个对象
	/// </summary>
	/// <param name="model">类型信息</param>
	/// <param name="version">版本号</param>
	public OnlineData(string model, string version)
	{
		ModelType = model;
		SoftVersion = version;
	}

	/// <summary>
	/// 指定类型及其版本号来实例化一个对象
	/// </summary>
	/// <param name="model">类型信息</param>
	/// <param name="version">版本号</param>
	public static implicit operator OnlineData(SecsValue value)
	{
		TypeHelper.TypeListCheck(value);
		if (value.Value is SecsValue[] array)
		{
			return new OnlineData(array[0].Value.ToString(), array[1].Value.ToString());
		}
		return null;
	}

	public static implicit operator SecsValue(OnlineData value)
	{
		return new SecsValue(new object[2] { value.ModelType, value.SoftVersion });
	}
}
