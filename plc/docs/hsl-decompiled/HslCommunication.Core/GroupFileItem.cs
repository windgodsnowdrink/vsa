// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using HslCommunication.BasicFramework;

namespace HslCommunication.Core;

/// <summary>
/// 单个文件的存储信息，用来表示在文件目录管理器里表示所有的文件信息的
/// Storage information of a single file, used to represent all file information in the file directory manager
/// </summary>
public class GroupFileItem
{
	/// <summary>
	/// 文件的名称
	/// </summary>
	public string FileName { get; set; }

	/// <summary>
	/// 文件的大小
	/// </summary>
	public long FileSize { get; set; }

	/// <summary>
	/// 文件的映射名称
	/// </summary>
	public string MappingName { get; set; }

	/// <summary>
	/// 文件的下载次数
	/// </summary>
	public long DownloadTimes { get; set; }

	/// <summary>
	/// 文件的上传时间
	/// </summary>
	public DateTime UploadTime { get; set; }

	/// <summary>
	/// 文件的上传人，拥有者
	/// </summary>
	public string Owner { get; set; }

	/// <summary>
	/// 文件的额外描述
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// 获取文件的大小描述
	/// Get file size description
	/// </summary>
	/// <returns>文件大小的字符串描述形式</returns>
	public string GetTextFromFileSize()
	{
		return SoftBasic.GetSizeDescription(FileSize);
	}

	public override string ToString()
	{
		return "GroupFileItem[" + FileName + ":" + MappingName + "]";
	}
}
