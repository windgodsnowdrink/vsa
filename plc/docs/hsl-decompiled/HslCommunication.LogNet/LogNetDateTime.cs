// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace HslCommunication.LogNet;

/// <summary>
/// 一个日志组件，可以根据时间来区分不同的文件存储
/// A log component that can distinguish different file storages based on time
/// </summary>
/// <remarks>
/// 此日志实例将根据日期时间来进行分类，支持的时间分类如下：分钟, 小时, 天, 周, 月份, 季度, 年
/// 当然你也可以指定允许存在多少个日志文件，比如我允许存在最多10个文件，如果你的日志是根据天来分文件的，那就是10天的数据。
/// 同理，如果你的日志是根据年来分文件的，那就是10年的日志文件。
/// 2025年9月9日更新: 路径里支持日期时间的格式化，支持动态路径，例如我希望按照"年"，"月"，"日"目录逐级存储，可以指定路径：D:\\Logs\\{yyyy}\\{MM}\\{dd}\\ 日志文件再按小时存储
/// </remarks>
public class LogNetDateTime : LogPathBase, ILogNet, IDisposable
{
	private GenerateMode generateMode = GenerateMode.ByEveryYear;

	private int fileSize = -1;

	/// <summary>
	/// 实例化一个根据时间存储的日志组件，需要指定每个文件的存储时间范围
	/// Instantiate a log component based on time, you need to specify the storage time range for each file
	/// </summary>
	/// <param name="filePath">文件存储的路径</param>
	/// <param name="generateMode">存储文件的间隔</param>
	/// <param name="fileQuantity">指定当前的日志文件数量上限，如果小于0，则不限制，文件一直增加，如果设置为10，就限制最多10个文件，会删除最近时间的10个文件之外的文件。</param>
	public LogNetDateTime(string filePath, GenerateMode generateMode = GenerateMode.ByEveryYear, int fileQuantity = -1)
	{
		base.filePath = filePath;
		this.generateMode = generateMode;
		base.LogSaveMode = LogSaveMode.Time;
		controlFileQuantity = fileQuantity;
		if (string.IsNullOrEmpty(filePath))
		{
			return;
		}
		if (filePath.Contains("{") && filePath.Contains("}"))
		{
			string path = Regex.Replace(filePath, "{[yYmMdDHhsS]+}", DateMatchEvaluator);
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}
		else if (!Directory.Exists(filePath))
		{
			Directory.CreateDirectory(filePath);
		}
	}

	/// <summary>
	/// 实例化一个根据时间存储的日志组件，需要指定每个文件的存储时间范围
	/// Instantiate a log component based on time, you need to specify the storage time range for each file
	/// </summary>
	/// <param name="filePath">文件存储的路径</param>
	/// <param name="generateMode">存储文件的间隔</param>
	/// <param name="timeQuantity">指定当前的日志文件的时间单位数量，如果小于0，则不限制，文件一直增加，如果设置为10，就限制最多10个单位的文件，会删除最近时间的10天之外的文件。</param>
	/// <param name="fileSize">指定文件的大小</param>
	public LogNetDateTime(string filePath, GenerateMode generateMode, int timeQuantity, int fileSize)
		: this(filePath, generateMode, timeQuantity)
	{
		this.fileSize = fileSize;
	}

	private string DateMatchEvaluator(Match match)
	{
		return DateTime.Now.ToString(match.Value.Substring(1, match.Value.Length - 2));
	}

	private string GetFileSaveName(int index)
	{
		if (string.IsNullOrEmpty(filePath))
		{
			return string.Empty;
		}
		string text = ((index > 0) ? $"({index}).txt" : ".txt");
		string path = Regex.Replace(filePath, "{[yYmMdDHhsS]+}", DateMatchEvaluator);
		switch (generateMode)
		{
		case GenerateMode.ByEveryMinute:
			return Path.Combine(path, LogNetManagment.LogFileHeadString + DateTime.Now.ToString("yyyyMMdd_HHmm") + text);
		case GenerateMode.ByEveryHour:
			return Path.Combine(path, LogNetManagment.LogFileHeadString + DateTime.Now.ToString("yyyyMMdd_HH") + text);
		case GenerateMode.ByEveryDay:
			return Path.Combine(path, LogNetManagment.LogFileHeadString + DateTime.Now.ToString("yyyyMMdd") + text);
		case GenerateMode.ByEveryWeek:
		{
			GregorianCalendar gregorianCalendar = new GregorianCalendar();
			int weekOfYear = gregorianCalendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
			return Path.Combine(path, LogNetManagment.LogFileHeadString + DateTime.Now.Year + "_W" + weekOfYear + text);
		}
		case GenerateMode.ByEveryMonth:
			return Path.Combine(path, LogNetManagment.LogFileHeadString + DateTime.Now.ToString("yyyy_MM") + text);
		case GenerateMode.ByEverySeason:
			return Path.Combine(path, LogNetManagment.LogFileHeadString + DateTime.Now.Year + "_Q" + ((DateTime.Now.Month - 1) / 3 + 1) + text);
		case GenerateMode.ByEveryYear:
			return Path.Combine(path, LogNetManagment.LogFileHeadString + DateTime.Now.Year + text);
		default:
			return string.Empty;
		}
	}

	protected override string GetFileSaveName()
	{
		if (string.IsNullOrEmpty(filePath))
		{
			return string.Empty;
		}
		if (fileSize <= 0)
		{
			return GetFileSaveName(0);
		}
		int num = 0;
		string fileSaveName;
		while (true)
		{
			fileSaveName = GetFileSaveName(num);
			if (!File.Exists(fileSaveName))
			{
				break;
			}
			try
			{
				FileInfo fileInfo = new FileInfo(fileSaveName);
				if (fileInfo.Length < fileSize)
				{
					return fileSaveName;
				}
			}
			catch
			{
				return fileSaveName;
			}
			num++;
		}
		return fileSaveName;
	}

	private DateTime GetDeleteTime()
	{
		return generateMode switch
		{
			GenerateMode.ByEveryMinute => DateTime.Now.AddMinutes(-controlFileQuantity), 
			GenerateMode.ByEveryHour => DateTime.Now.AddHours(-controlFileQuantity), 
			GenerateMode.ByEveryDay => DateTime.Now.AddDays(-controlFileQuantity), 
			GenerateMode.ByEveryWeek => DateTime.Now.AddDays(-controlFileQuantity * 7), 
			GenerateMode.ByEveryMonth => DateTime.Now.AddMonths(-controlFileQuantity), 
			GenerateMode.ByEverySeason => DateTime.Now.AddMonths(-controlFileQuantity * 3), 
			GenerateMode.ByEveryYear => DateTime.Now.AddYears(-controlFileQuantity), 
			_ => DateTime.Now.AddDays(-controlFileQuantity), 
		};
	}

	public override string[] GetExistLogFileNames()
	{
		if (!string.IsNullOrEmpty(filePath))
		{
			string path = Regex.Replace(filePath, "{[yYmMdDHhsS]+}", DateMatchEvaluator);
			return Directory.GetFiles(path, LogNetManagment.LogFileHeadString + "*.txt");
		}
		return new string[0];
	}

	protected override void OnWriteCompleted(bool createNewLogFile)
	{
		if (fileSize <= 0)
		{
			base.OnWriteCompleted(createNewLogFile);
		}
		else
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
					DateTime deleteTime = GetDeleteTime();
					for (int num = 0; num < list.Count && list[num].CreationTime < deleteTime; num++)
					{
						File.Delete(list[num].FullName);
					}
				}
			}
			catch
			{
			}
		}
	}

	public override string ToString()
	{
		return $"LogNetDateTime[{generateMode}]";
	}
}
