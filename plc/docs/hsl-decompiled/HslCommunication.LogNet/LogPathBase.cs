// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Collections.Generic;
using System.IO;

namespace HslCommunication.LogNet;

/// <summary>
/// 基于路径实现的日志类的基类，提供几个基础的方法信息。
/// The base class of the log class implemented based on the path provides several basic method information.
/// </summary>
public abstract class LogPathBase : LogNetBase
{
	/// <summary>
	/// 当前正在存储的文件名
	/// File name currently being stored
	/// </summary>
	protected string fileName = string.Empty;

	/// <summary>
	/// 存储文件的路径，如果设置为空，就不进行存储。
	/// The path for storing the file. If it is set to empty, it will not be stored.
	/// </summary>
	protected string filePath = string.Empty;

	/// <summary>
	/// 控制文件的数量，小于1则不进行任何操作，当设置为10的时候，就限制文件数量为10。
	/// Control the number of files. If it is less than 1, no operation is performed. When it is set to 10, the number of files is limited to 10.
	/// </summary>
	protected int controlFileQuantity = -1;

	protected override void OnWriteCompleted(bool createNewLogFile)
	{
		if (!createNewLogFile || controlFileQuantity < 1)
		{
			return;
		}
		try
		{
			string[] existLogFileNames = GetExistLogFileNames();
			if (existLogFileNames.Length > controlFileQuantity)
			{
				List<FileInfo> list = new List<FileInfo>();
				for (int i = 0; i < existLogFileNames.Length; i++)
				{
					list.Add(new FileInfo(existLogFileNames[i]));
				}
				list.Sort((FileInfo m, FileInfo n) => m.CreationTime.CompareTo(n.CreationTime));
				for (int num = 0; num < list.Count - controlFileQuantity; num++)
				{
					File.Delete(list[num].FullName);
				}
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// 返回所有的日志文件名称，返回一个列表
	/// Returns all log file names, returns a list
	/// </summary>
	/// <returns>所有的日志文件信息</returns>
	public virtual string[] GetExistLogFileNames()
	{
		if (!string.IsNullOrEmpty(filePath))
		{
			return Directory.GetFiles(filePath, LogNetManagment.LogFileHeadString + "*.txt");
		}
		return new string[0];
	}

	public override string ToString()
	{
		return "LogPathBase";
	}
}
