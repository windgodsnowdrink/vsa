// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.Core;

/// <summary>
/// 当前MQTT服务器文件操作的对象类
/// Object class of current MQTT server file operation
/// </summary>
public class MqttFileOperateInfo
{
	/// <summary>
	/// 指示上传还是下载的操作，Upload上传，Download下载，Delete删除文件，DeleteFolder删除目录
	/// Indicate upload or download operation, "Upload": upload, "Download": download, "Delete": delete file, "DeleteFolder": delete directory
	/// </summary>
	public string Operate { get; set; }

	/// <summary>
	/// 文件上传或是下载的类别
	/// </summary>
	public string Groups { get; set; }

	/// <summary>
	/// 上传,下载或是删除的文件名
	/// File name uploaded, downloaded or deleted
	/// </summary>
	public string[] FileNames { get; set; }

	/// <summary>
	/// 操作的文件映射的名称
	/// </summary>
	public string[] MappingNames { get; set; }

	/// <summary>
	/// 当前操作消耗的时间
	/// Time consumed by current operation
	/// </summary>
	public TimeSpan TimeCost { get; set; }
}
