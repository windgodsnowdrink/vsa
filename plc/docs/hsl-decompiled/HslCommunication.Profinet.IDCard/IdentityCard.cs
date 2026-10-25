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

namespace HslCommunication.Profinet.IDCard;

/// <summary>
/// 身份证的信息类
/// </summary>
public class IdentityCard
{
	/// <summary>
	/// 名字
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// 性别
	/// </summary>
	public string Sex { get; set; }

	/// <summary>
	/// 身份证号
	/// </summary>
	public string Id { get; set; }

	/// <summary>
	/// 民族
	/// </summary>
	public string Nation { get; set; }

	/// <summary>
	/// 生日
	/// </summary>
	public DateTime Birthday { get; set; }

	/// <summary>
	/// 地址
	/// </summary>
	public string Address { get; set; }

	/// <summary>
	/// 发证机关
	/// </summary>
	public string Organ { get; set; }

	/// <summary>
	/// 有效期日期的起始日期
	/// </summary>
	public DateTime ValidityStartDate { get; set; }

	/// <summary>
	/// 有效期日期的结束日期
	/// </summary>
	public DateTime ValidityEndDate { get; set; }

	/// <summary>
	/// 头像信息
	/// </summary>
	public byte[] Portrait { get; set; }

	/// <summary>
	/// 返回表示当前对象的字符串
	/// </summary>
	/// <returns>字符串</returns>
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("姓名：" + Name);
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("性别：" + Sex);
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("民族：" + Nation);
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("身份证号：" + Id);
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append($"出身日期：{Birthday.Year}年{Birthday.Month}月{Birthday.Day}日");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("地址：" + Address);
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("发证机关：" + Organ);
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append($"有效日期：{ValidityStartDate.Year}年{ValidityStartDate.Month}月{ValidityStartDate.Day}日 - {ValidityEndDate.Year}年{ValidityEndDate.Month}月{ValidityEndDate.Day}日");
		stringBuilder.Append(Environment.NewLine);
		return stringBuilder.ToString();
	}
}
