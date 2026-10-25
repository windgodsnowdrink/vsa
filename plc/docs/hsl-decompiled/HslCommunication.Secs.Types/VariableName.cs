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

/// <summary>
/// 变量名称类对象
/// </summary>
public class VariableName
{
	/// <summary>
	/// 变量的ID信息
	/// </summary>
	public long ID { get; set; }

	/// <summary>
	/// 变量的名称信息
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// 变量的单位信息
	/// </summary>
	public string Units { get; set; }

	public override string ToString()
	{
		return Name;
	}

	public static implicit operator VariableName(SecsValue value)
	{
		TypeHelper.TypeListCheck(value);
		if (value.Value is SecsValue[] array)
		{
			VariableName variableName = new VariableName();
			variableName.ID = Convert.ToInt64(array[0].Value);
			variableName.Name = array[1].Value as string;
			variableName.Units = array[2].Value as string;
			return variableName;
		}
		return null;
	}

	public static implicit operator SecsValue(VariableName value)
	{
		return new SecsValue(new object[3] { value.ID, value.Name, value.Units });
	}
}
