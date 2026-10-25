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
/// 文件服务器的目录管理类的基本信息
/// </summary>
public class GroupFileInfo
{
	/// <summary>
	/// 文件目录的名称信息
	/// </summary>
	public string PathName { get; set; }

	/// <summary>
	/// 获取或设置文件的总大小
	/// </summary>
	public long FileTotalSize { get; set; }

	/// <summary>
	/// 获取或设置文件的总数量
	/// </summary>
	public int FileCount { get; set; }

	/// <summary>
	/// 获取或设置最后一次文件更新的时间，如果不存在文件，则为理论最小值
	/// </summary>
	public DateTime LastModifyTime { get; set; }

	/// <summary>
	/// 获取或设置最后一次更新的文件的基本信息，如果该目录不存在文件，则本值为空
	/// </summary>
	public GroupFileItem LastModifyFile { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public GroupFileInfo()
	{
		LastModifyTime = DateTime.Now;
	}

	public override string ToString()
	{
		return $"Count: {FileCount} TotalSize: {FileTotalSize} [{SoftBasic.GetSizeDescription(FileTotalSize)}] ModifyTime:{LastModifyTime:yyyy-MM-dd HH:mm:ss}";
	}
}
