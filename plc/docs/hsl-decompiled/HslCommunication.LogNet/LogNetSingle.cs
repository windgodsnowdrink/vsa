// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.IO;
using System.Text;

namespace HslCommunication.LogNet;

/// <summary>
/// 单日志文件对象，所有的日志信息的记录都会写入一个文件里面去。文件名指定为空的时候，自动不存储文件。
/// Single log file object, all log information records will be written to a file. When the file name is specified as empty, the file is not stored automatically.
/// </summary>
/// <remarks>
/// 此日志实例化需要指定一个完整的文件路径，当需要记录日志的时候调用方法，会使得日志越来越大，对于写入的性能没有太大影响，但是会影响文件读取。
/// </remarks>
public class LogNetSingle : LogNetBase, ILogNet, IDisposable
{
	private readonly string fileName = string.Empty;

	/// <summary>
	/// 实例化一个单文件日志的对象，如果日志的路径为空，那么就不存储数据，只触发事件
	/// Instantiate a single file log object. If the log path is empty, then no data is stored and only the  event is triggered.
	/// </summary>
	/// <param name="filePath">文件的路径</param>
	public LogNetSingle(string filePath)
	{
		fileName = filePath;
		base.LogSaveMode = LogSaveMode.SingleFile;
		if (!string.IsNullOrEmpty(fileName))
		{
			FileInfo fileInfo = new FileInfo(filePath);
			if (!Directory.Exists(fileInfo.DirectoryName))
			{
				Directory.CreateDirectory(fileInfo.DirectoryName);
			}
		}
	}

	/// <summary>
	/// 单日志文件允许清空日志内容
	/// Single log file allows clearing log contents
	/// </summary>
	public void ClearLog()
	{
		m_fileSaveLock.Enter();
		try
		{
			if (!string.IsNullOrEmpty(fileName))
			{
				File.Create(fileName).Dispose();
			}
		}
		catch
		{
			throw;
		}
		finally
		{
			m_fileSaveLock.Leave();
		}
	}

	/// <summary>
	/// 获取单日志文件的所有保存记录
	/// Get all saved records of a single log file
	/// </summary>
	/// <returns>字符串信息</returns>
	public string GetAllSavedLog()
	{
		string result = string.Empty;
		m_fileSaveLock.Enter();
		try
		{
			if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
			{
				using StreamReader streamReader = new StreamReader(fileName, Encoding.UTF8);
				result = streamReader.ReadToEnd();
			}
		}
		catch
		{
			throw;
		}
		finally
		{
			m_fileSaveLock.Leave();
		}
		return result;
	}

	/// <summary>
	/// 获取所有的日志文件数组，对于单日志文件来说就只有一个
	/// Get all log file arrays, only one for a single log file
	/// </summary>
	/// <returns>字符串数组，包含了所有的存在的日志数据</returns>
	public string[] GetExistLogFileNames()
	{
		if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
		{
			return new string[0];
		}
		return new string[1] { fileName };
	}

	protected override string GetFileSaveName()
	{
		return fileName;
	}

	public override string ToString()
	{
		return "LogNetSingle[" + fileName + "]";
	}
}
