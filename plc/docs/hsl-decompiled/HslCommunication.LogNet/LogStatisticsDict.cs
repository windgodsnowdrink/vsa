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
using System.IO;
using System.Linq;
using HslCommunication.Core;
using HslCommunication.Reflection;

namespace HslCommunication.LogNet;

/// <summary>
/// 的词典集合类，用于多个数据的统计信息，例如可以统计多个规格的产量信息，统计多个方法的调用次数信息
/// The dictionary collection class of  is used for the statistical information of multiple data, for example,
/// the output information of multiple specifications can be counted, and the number of calls of multiple methods can be counted.
/// </summary>
public class LogStatisticsDict
{
	private GenerateMode generateMode = GenerateMode.ByEveryDay;

	private int arrayLength = 30;

	private Dictionary<string, LogStatistics> dict;

	private object dictLock;

	private LogStatistics logStat;

	public GenerateMode GenerateMode => generateMode;

	[HslMqttApi(HttpMethod = "GET", Description = "Get the total amount of current statistical information")]
	public int ArrayLength => arrayLength;

	[HslMqttApi(PropertyUnfold = true, Description = "Get the log statistics object of the current dictionary class itself, and count the statistics of all elements")]
	public LogStatistics LogStat => logStat;

	/// <summary>
	/// 根据指定的存储模式，数据个数来实例化一个对象
	/// According to the specified storage mode, the number of data to instantiate an object
	/// </summary>
	/// <param name="generateMode">当前的数据存储模式</param>
	/// <param name="arrayLength">准备存储的数据总个数</param>
	public LogStatisticsDict(GenerateMode generateMode, int arrayLength)
	{
		this.generateMode = generateMode;
		this.arrayLength = arrayLength;
		dictLock = new object();
		dict = new Dictionary<string, LogStatistics>(128);
		logStat = new LogStatistics(generateMode, arrayLength);
	}

	/// <summary>
	/// 根据给定的关键字信息，获取相关的  对象，进而执行很多的操作
	/// According to the given keyword information, obtain related  objects, and then perform many operations
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>日志对象，如果当前的日志对象不存在，就返回为NULL</returns>
	public LogStatistics GetLogStatistics(string key)
	{
		lock (dictLock)
		{
			if (dict.ContainsKey(key))
			{
				return dict[key];
			}
			return null;
		}
	}

	/// <summary>
	/// 手动新增一个对象，需要指定相关的关键字
	/// Manually add a  object, you need to specify related keywords
	/// </summary>
	/// <param name="key">关键字信息</param>
	/// <param name="logStatistics">日志统计对象</param>
	public void AddLogStatistics(string key, LogStatistics logStatistics)
	{
		lock (dictLock)
		{
			if (dict.ContainsKey(key))
			{
				dict[key] = logStatistics;
			}
			else
			{
				dict.Add(key, logStatistics);
			}
		}
	}

	/// <summary>
	/// 移除一个对象，需要指定相关的关键字，如果关键字本来就存在，返回 True, 如果不存在，返回 False
	/// To remove a  object, you need to specify the relevant keyword. If the keyword already exists, return True, if it does not exist, return False
	/// </summary>
	/// <param name="key">关键字信息</param>
	/// <returns>如果关键字本来就存在，返回 True, 如果不存在，返回 False</returns>
	public bool RemoveLogStatistics(string key)
	{
		lock (dictLock)
		{
			if (dict.ContainsKey(key))
			{
				dict.Remove(key);
				return true;
			}
			return false;
		}
	}

	[HslMqttApi(Description = "Adding a new statistical information will determine the position to insert the data according to the current time")]
	/// <summary>
	/// 新增一个统计信息，将会根据当前的时间来决定插入数据位置，如果数据位置发生了变化，则数据向左发送移动。如果没有移动或是移动完成后，最后一个数进行新增数据 frequency 次
	/// Adding a new statistical information will determine the position to insert the data according to the current time. If the data position changes,
	/// the data will be sent to the left. If there is no movement or after the movement is completed, add data to the last number frequency times
	/// </summary>
	/// <param name="key">当前选择的关键字</param>
	/// <param name="frequency">新增的次数信息，默认为1</param>
	public void StatisticsAdd(string key, long frequency = 1L)
	{
		logStat.StatisticsAdd(frequency);
		LogStatistics logStatistics = GetLogStatistics(key);
		if (logStatistics == null)
		{
			lock (dictLock)
			{
				if (!dict.ContainsKey(key))
				{
					logStatistics = new LogStatistics(generateMode, arrayLength);
					dict.Add(key, logStatistics);
				}
				else
				{
					logStatistics = dict[key];
				}
			}
		}
		logStatistics?.StatisticsAdd(frequency);
	}

	[HslMqttApi(Description = "Adding a new statistical information will determine the position to insert the data according to the specified time")]
	/// <summary>
	/// 新增一个统计信息，将会根据当前的时间来决定插入数据位置，如果数据位置发生了变化，则数据向左发送移动。如果没有移动或是移动完成后，最后一个数进行新增数据 frequency 次
	/// Adding a new statistical information will determine the position to insert the data according to the current time. If the data position changes,
	/// the data will be sent to the left. If there is no movement or after the movement is completed, add data to the last number frequency times
	/// </summary>
	/// <param name="key">当前的关键字</param>
	/// <param name="frequency">新增的次数信息</param>
	/// <param name="time">新增的次数的时间</param>
	public void StatisticsAddByTime(string key, long frequency, DateTime time)
	{
		logStat.StatisticsAddByTime(frequency, time);
		LogStatistics logStatistics = GetLogStatistics(key);
		if (logStatistics == null)
		{
			lock (dictLock)
			{
				if (!dict.ContainsKey(key))
				{
					logStatistics = new LogStatistics(generateMode, arrayLength);
					dict.Add(key, logStatistics);
				}
				else
				{
					logStatistics = dict[key];
				}
			}
		}
		logStatistics?.StatisticsAddByTime(frequency, time);
	}

	[HslMqttApi(Description = "Get a data snapshot of the current statistics")]
	/// <summary>
	/// 获取当前的统计信息的数据快照，这是数据的副本，修改了里面的值不影响
	/// Get a data snapshot of the current statistics. This is a copy of the data. Modifying the value inside does not affect
	/// </summary>
	/// <param name="key">当前的关键字的信息</param>
	/// <returns>实际的统计数据信息</returns>
	public long[] GetStatisticsSnapshot(string key)
	{
		return GetLogStatistics(key)?.GetStatisticsSnapshot();
	}

	[HslMqttApi(Description = "Get a snapshot of statistical data information according to the specified time range")]
	/// <summary>
	/// 根据指定的时间范围来获取统计的数据信息快照，包含起始时间，包含结束时间，这是数据的副本，修改了里面的值不影响
	/// Get a snapshot of statistical data information according to the specified time range, including the start time,
	/// also the end time. This is a copy of the data. Modifying the value inside does not affect
	/// </summary>
	/// <param name="key">当前的关键字信息</param>
	/// <param name="start">起始时间</param>
	/// <param name="finish">结束时间</param>
	/// <returns>指定实际范围内的数据副本</returns>
	public long[] GetStatisticsSnapshotByTime(string key, DateTime start, DateTime finish)
	{
		return GetLogStatistics(key)?.GetStatisticsSnapshotByTime(start, finish);
	}

	[HslMqttApi(Description = "Get data information of all keywords")]
	/// <summary>
	/// 获取所有的关键字的数据信息
	/// Get data information of all keywords
	/// </summary>
	/// <returns>字符串数组</returns>
	public string[] GetKeys()
	{
		lock (dictLock)
		{
			return dict.Keys.ToArray();
		}
	}

	/// <summary>
	/// 将当前的统计信息及数据内容写入到指定的文件里面，需要指定文件的路径名称
	/// Write the current statistical information and data content to the specified file, you need to specify the path name of the file
	/// </summary>
	/// <param name="fileName">文件的完整的路径名称</param>
	public void SaveToFile(string fileName)
	{
		using FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
		byte[] buffer = new byte[1024];
		BitConverter.GetBytes(305419906).CopyTo(buffer, 0);
		BitConverter.GetBytes((ushort)buffer.Length).CopyTo(buffer, 4);
		BitConverter.GetBytes((ushort)GenerateMode).CopyTo(buffer, 6);
		string[] keys = GetKeys();
		BitConverter.GetBytes(keys.Length).CopyTo(buffer, 8);
		fileStream.Write(buffer, 0, buffer.Length);
		string[] array2 = keys;
		foreach (string text in array2)
		{
			LogStatistics logStatistics = GetLogStatistics(text);
			if (logStatistics != null)
			{
				HslHelper.WriteStringToStream(fileStream, text);
				HslHelper.WriteBinaryToStream(fileStream, logStatistics.SaveToBinary());
			}
		}
	}

	/// <summary>
	/// 从指定的文件加载对应的统计信息，通常是调用方法存储的文件，如果文件不存在，将会跳过加载
	/// Load the corresponding statistical information from the specified file, usually the file stored by calling the  method.
	/// If the file does not exist, the loading will be skipped
	/// </summary>
	/// <param name="fileName">文件的完整的路径名称</param>
	public void LoadFromFile(string fileName)
	{
		if (!File.Exists(fileName))
		{
			return;
		}
		using FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
		byte[] value = HslHelper.ReadSpecifiedLengthFromStream(stream, 1024);
		generateMode = (GenerateMode)BitConverter.ToUInt16(value, 6);
		int num = BitConverter.ToInt32(value, 8);
		for (int i = 0; i < num; i++)
		{
			string key = HslHelper.ReadStringFromStream(stream);
			byte[] buffer = HslHelper.ReadBinaryFromStream(stream);
			LogStatistics logStatistics = new LogStatistics(generateMode, arrayLength);
			logStatistics.LoadFromBinary(buffer);
			AddLogStatistics(key, logStatistics);
		}
	}
}
