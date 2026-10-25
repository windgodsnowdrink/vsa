// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace HslCommunication.Enthernet.Ftp;

/// <summary>
/// FTP文件的信息对象
/// </summary>
public class FtpFileItem
{
	/// <summary>
	/// 文件名或是文件夹名
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// 文件的大小，字节为单位
	/// </summary>
	public long Size { get; set; }

	/// <summary>
	/// 文件的创建时间
	/// </summary>
	public DateTime CreateTime { get; set; }

	/// <summary>
	/// 是否文件夹的标识
	/// </summary>
	public bool IsDirectory { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public FtpFileItem()
	{
	}

	/// <summary>
	/// 指定行内容解析出对象信息
	/// </summary>
	/// <param name="content">原始的数据信息</param>
	public FtpFileItem(string content)
	{
		if (Regex.IsMatch(content, "^[0-9][0-9][-/]"))
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(DateTime.Now.Year.ToString().Substring(0, 2));
			stringBuilder.Append(content.Substring(6, 2));
			stringBuilder.Append("-");
			stringBuilder.Append(content.Substring(0, 2));
			stringBuilder.Append("-");
			stringBuilder.Append(content.Substring(3, 2));
			stringBuilder.Append(" ");
			stringBuilder.Append(content.Substring(10, 5));
			stringBuilder.Append(":00");
			CreateTime = DateTime.Parse(stringBuilder.ToString());
			if (content[15] == 'P')
			{
				CreateTime = CreateTime.AddHours(12.0);
			}
			if (content.Substring(24, 5) == "<DIR>")
			{
				IsDirectory = true;
				Size = 0L;
				Name = content.Substring(39).Trim();
			}
			else
			{
				IsDirectory = false;
				string s = content.Substring(18, 21).Trim();
				Size = long.Parse(s);
				Name = content.Substring(39).Trim();
			}
			return;
		}
		if (content.StartsWith("type=", StringComparison.OrdinalIgnoreCase))
		{
			if (Regex.IsMatch(content, "type=dir;"))
			{
				IsDirectory = true;
				Size = 0L;
				Match match = Regex.Match(content, "modify=[0-9]+;", RegexOptions.IgnoreCase);
				if (match.Success)
				{
					string value = match.Value;
					CreateTime = DateTime.Parse(value.Substring(7, 4) + "-" + value.Substring(11, 2) + "-" + value.Substring(13, 2) + " " + value.Substring(15, 2) + ":" + value.Substring(17, 2) + ":" + value.Substring(19, 2)).ToLocalTime();
					Name = content.Substring(match.Index + match.Length + 1);
				}
				return;
			}
			IsDirectory = false;
			Match match2 = Regex.Match(content, "modify=[0-9]+;", RegexOptions.IgnoreCase);
			if (match2.Success)
			{
				string value2 = match2.Value;
				CreateTime = DateTime.Parse(value2.Substring(7, 4) + "-" + value2.Substring(11, 2) + "-" + value2.Substring(13, 2) + " " + value2.Substring(15, 2) + ":" + value2.Substring(17, 2) + ":" + value2.Substring(19, 2)).ToLocalTime();
			}
			match2 = Regex.Match(content, "size=[0-9]+;", RegexOptions.IgnoreCase);
			if (match2.Success)
			{
				string value3 = match2.Value;
				Size = long.Parse(value3.Substring(5, value3.Length - 6));
				Name = content.Substring(match2.Index + value3.Length + 1);
			}
			return;
		}
		Name = content.Substring(55).Trim();
		string s2 = content.Substring(42, 3).Trim();
		int month = DateTime.ParseExact(s2, "MMM", CultureInfo.InvariantCulture).Month;
		string s3 = content.Substring(46, 2).Trim();
		int num = int.Parse(s3);
		string text = content.Substring(49, 5).Trim();
		if (text.Contains(":"))
		{
			string text2 = DateTime.Now.Year.ToString();
			CreateTime = DateTime.Parse($"{text2}-{month:D2}-{num:D2} {text}:00");
			if (CreateTime > DateTime.Now)
			{
				CreateTime = CreateTime.AddYears(-1);
			}
		}
		else
		{
			int num2 = int.Parse(text);
			CreateTime = DateTime.Parse($"{num2}-{month:D2}-{num:D2} 00:00:00");
		}
		if (content.StartsWith("d"))
		{
			IsDirectory = true;
			Size = 0L;
		}
		else
		{
			IsDirectory = false;
			string s4 = content.Substring(29, 12).Trim();
			Size = long.Parse(s4);
		}
	}
}
