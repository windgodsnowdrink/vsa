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
using System.Threading;
using HslCommunication.Core;
using HslCommunication.Reflection;

namespace HslCommunication.LogNet;

/// <summary>
/// 一个统计次数的辅助类，可用于实现分析一些次数统计信息，比如统计某个API最近每天的访问次数，
/// 统计日志组件最近每天访问的次数，调用者只需要关心统计方式和数据个数，详细参照API文档。
/// An auxiliary class for counting the number of times, which can be used to realize the analysis of some number of times statistical information,
/// such as counting the number of daily visits of an API, and counting the number of daily visits of the log component.
/// The caller only needs to care about the statistical method and the number of data. Refer to details API documentation.
/// </summary>
public class LogStatistics : LogStatisticsBase<long>
{
	private RegularByteTransform byteTransform;

	private long totalSum = 0L;

	[HslMqttApi(HttpMethod = "GET", Description = "Get the sum of all current values")]
	public long TotalSum => totalSum;

	/// <summary>
	/// 实例化一个新的数据统计内容，需要指定当前的时间统计方式，按小时，按天，按月等等，还需要指定统计的数据数量，比如按天统计30天。
	/// To instantiate a new data statistics content, you need to specify the current time statistics method, by hour, by day, by month, etc.,
	/// and also need to specify the number of statistics, such as 30 days by day.
	/// </summary>
	/// <param name="generateMode">时间的统计方式</param>
	/// <param name="arrayLength">数据的数量信息，如果本值为-1，则表示数据一直增长</param>
	public LogStatistics(GenerateMode generateMode, int dataCount)
		: base(generateMode, dataCount)
	{
		byteTransform = new RegularByteTransform();
	}

	[HslMqttApi(Description = "Adding a new statistical information will determine the position to insert the data according to the current time.")]
	/// <summary>
	/// 新增一个统计信息，将会根据当前的时间来决定插入数据位置，如果数据位置发生了变化，则数据向左发送移动。如果没有移动或是移动完成后，最后一个数进行新增数据 frequency 次
	/// Adding a new statistical information will determine the position to insert the data according to the current time. If the data position changes,
	/// the data will be sent to the left. If there is no movement or after the movement is completed, add data to the last number frequency times
	/// </summary>
	/// <param name="frequency">新增的次数信息，默认为1</param>
	public void StatisticsAdd(long frequency = 1L)
	{
		Interlocked.Add(ref totalSum, frequency);
		StatisticsCustomAction((long m) => m + frequency);
	}

	[HslMqttApi(Description = "Adding a new statistical information will determine the position to insert the data according to the specified time.")]
	/// <summary>
	/// 新增一个统计信息，将会根据指定的时间来决定插入数据位置，如果数据位置发生了变化，则数据向左发送移动。如果没有移动或是移动完成后，最后一个数进行新增数据 frequency 次
	/// Adding a new statistical information will determine the position to insert the data according to the specified time. If the data position changes,
	/// the data will be sent to the left. If there is no movement or after the movement is completed, add data to the last number frequency times
	/// </summary>
	/// <param name="frequency">新增的次数信息</param>
	/// <param name="time">新增的次数的时间</param>
	public void StatisticsAddByTime(long frequency, DateTime time)
	{
		Interlocked.Add(ref totalSum, frequency);
		StatisticsCustomAction((long m) => m + frequency, time);
	}

	/// <summary>
	/// 将当前所有的数据都写入到二进制的内存里去，可以用来写入文件或是网络发送。
	/// Write all current data into binary memory, which can be used to write files or send over the network.
	/// </summary>
	/// <returns>二进制的byte数组</returns>
	public byte[] SaveToBinary()
	{
		OperateResult<long, long[]> statisticsSnapAndDataMark = GetStatisticsSnapAndDataMark();
		int num = 1024;
		byte[] buffer = new byte[statisticsSnapAndDataMark.Content2.Length * 8 + num];
		BitConverter.GetBytes(305419896).CopyTo(buffer, 0);
		BitConverter.GetBytes((ushort)num).CopyTo(buffer, 4);
		BitConverter.GetBytes((ushort)base.GenerateMode).CopyTo(buffer, 6);
		BitConverter.GetBytes(statisticsSnapAndDataMark.Content2.Length).CopyTo(buffer, 8);
		BitConverter.GetBytes(statisticsSnapAndDataMark.Content1).CopyTo(buffer, 12);
		BitConverter.GetBytes(TotalSum).CopyTo(buffer, 20);
		for (int i = 0; i < statisticsSnapAndDataMark.Content2.Length; i++)
		{
			BitConverter.GetBytes(statisticsSnapAndDataMark.Content2[i]).CopyTo(buffer, num + i * 8);
		}
		return buffer;
	}

	/// <summary>
	/// 将当前的统计信息及数据内容写入到指定的文件里面，需要指定文件的路径名称
	/// Write the current statistical information and data content to the specified file, you need to specify the path name of the file
	/// </summary>
	/// <param name="fileName">文件的完整的路径名称</param>
	public void SaveToFile(string fileName)
	{
		File.WriteAllBytes(fileName, SaveToBinary());
	}

	/// <summary>
	/// 从二进制的数据内容加载，会对数据的合法性进行检查，如果数据不匹配，会报异常
	/// Loading from the binary data content will check the validity of the data. If the data does not match, an exception will be reported
	/// </summary>
	/// <param name="buffer">等待加载的二进制数据</param>
	public void LoadFromBinary(byte[] buffer)
	{
		int num = BitConverter.ToInt32(buffer, 0);
		if (num != 305419896)
		{
			throw new Exception("File is not LogStatistics file, can't load data.");
		}
		int index = BitConverter.ToUInt16(buffer, 4);
		GenerateMode generateMode = (GenerateMode)BitConverter.ToUInt16(buffer, 6);
		int length = BitConverter.ToInt32(buffer, 8);
		long num2 = BitConverter.ToInt64(buffer, 12);
		long num3 = BitConverter.ToInt64(buffer, 20);
		base.generateMode = generateMode;
		totalSum = num3;
		long[] array = byteTransform.TransInt64(buffer, index, length);
		Reset(array, num2);
	}

	/// <summary>
	/// 从指定的文件加载对应的统计信息，通常是调用方法存储的文件，如果文件不存在，将会跳过加载
	/// Load the corresponding statistical information from the specified file, usually the file stored by calling the  method.
	/// If the file does not exist, the loading will be skipped
	/// </summary>
	/// <param name="fileName">文件的完整的路径名称</param>
	public void LoadFromFile(string fileName)
	{
		if (File.Exists(fileName))
		{
			LoadFromBinary(File.ReadAllBytes(fileName));
		}
	}

	public override string ToString()
	{
		return $"LogStatistics[{base.GenerateMode}:{base.ArrayLength}]";
	}
}
