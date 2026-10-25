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
/// 一个用于数值范围记录的类，可以按照时间进行分类统计，比如计算一个温度值的每天的开始值，结束值，最大值，最小值，平均值信息。详细见API文档信息。
/// A class used to record the value range, which can be classified according to time, such as calculating the start value, end value,
/// maximum value, minimum value, and average value of a temperature value. See the API documentation for details.
/// </summary>
public class LogValueLimit : LogStatisticsBase<ValueLimit>
{
	private RegularByteTransform byteTransform;

	private long valueCount = 0L;

	[HslMqttApi(HttpMethod = "GET", Description = "The total amount of data currently set")]
	public long ValueCount => valueCount;

	/// <summary>
	/// 实例化一个新的数据统计内容，需要指定当前的时间统计方式，按小时，按天，按月等等，还需要指定统计的数据数量，比如按天统计30天。
	/// To instantiate a new data statistics content, you need to specify the current time statistics method, by hour, by day, by month, etc.,
	/// and also need to specify the number of statistics, such as 30 days by day.
	/// </summary>
	/// <param name="generateMode">时间的统计方式</param>
	/// <param name="arrayLength">数据的数量信息，如果本值为-1，则表示数据一直增长</param>
	public LogValueLimit(GenerateMode generateMode, int dataCount)
		: base(generateMode, dataCount)
	{
		byteTransform = new RegularByteTransform();
	}

	[HslMqttApi(Description = "Add a new data for analysis")]
	/// <summary>
	/// 新增一个数据用于分析，将会根据当前的时间来决定插入数据位置，如果数据位置发生了变化，则数据向左发送移动。如果没有移动或是移动完成后，最后一个数进行数据更新，包括最大值，最小值，平均值。
	/// Add a new data for analysis, and will determine the position to insert the data according to the current time. If the data position changes,
	/// the data will be sent to the left. If there is no movement or after the movement is completed, data update for the last number, including maximum, minimum, and average.
	/// </summary>
	/// <param name="value">当前的新的数据值</param>
	public void AnalysisNewValue(double value)
	{
		Interlocked.Increment(ref valueCount);
		StatisticsCustomAction((ValueLimit m) => m.SetNewValue(value));
	}

	[HslMqttApi(Description = "Add a new data for analysis")]
	/// <summary>
	/// 新增一个数据用于分析，将会指定的时间来决定插入数据位置，如果数据位置发生了变化，则数据向左发送移动。如果没有移动或是移动完成后，最后一个数进行数据更新，包括最大值，最小值，平均值。
	/// dd a new data for analysis, and will determine the position to insert the data according to the specified time. If the data position changes,
	/// the data will be sent to the left. If there is no movement or after the movement is completed, data update for the last number, including maximum, minimum, and average.
	/// </summary>
	/// <param name="value">当前的新的数据值</param>
	/// <param name="time">指定的时间信息</param>
	public void AnalysisNewValueByTime(double value, DateTime time)
	{
		Interlocked.Increment(ref valueCount);
		StatisticsCustomAction((ValueLimit m) => m.SetNewValue(value), time);
	}

	/// <summary>
	/// 将当前所有的数据都写入到二进制的内存里去，可以用来写入文件或是网络发送。
	/// Write all current data into binary memory, which can be used to write files or send over the network.
	/// </summary>
	/// <returns>二进制的byte数组</returns>
	public byte[] SaveToBinary()
	{
		OperateResult<long, ValueLimit[]> statisticsSnapAndDataMark = GetStatisticsSnapAndDataMark();
		int num = 1024;
		int num2 = 64;
		byte[] buffer = new byte[statisticsSnapAndDataMark.Content2.Length * num2 + num];
		BitConverter.GetBytes(305419897).CopyTo(buffer, 0);
		BitConverter.GetBytes((ushort)num).CopyTo(buffer, 4);
		BitConverter.GetBytes((ushort)base.GenerateMode).CopyTo(buffer, 6);
		BitConverter.GetBytes(statisticsSnapAndDataMark.Content2.Length).CopyTo(buffer, 8);
		BitConverter.GetBytes(statisticsSnapAndDataMark.Content1).CopyTo(buffer, 12);
		BitConverter.GetBytes(valueCount).CopyTo(buffer, 20);
		BitConverter.GetBytes(num2).CopyTo(buffer, 28);
		for (int i = 0; i < statisticsSnapAndDataMark.Content2.Length; i++)
		{
			byteTransform.TransByte(statisticsSnapAndDataMark.Content2[i].StartValue).CopyTo(buffer, i * num2 + num);
			byteTransform.TransByte(statisticsSnapAndDataMark.Content2[i].Current).CopyTo(buffer, i * num2 + num + 8);
			byteTransform.TransByte(statisticsSnapAndDataMark.Content2[i].MaxValue).CopyTo(buffer, i * num2 + num + 16);
			byteTransform.TransByte(statisticsSnapAndDataMark.Content2[i].MinValue).CopyTo(buffer, i * num2 + num + 24);
			byteTransform.TransByte(statisticsSnapAndDataMark.Content2[i].Average).CopyTo(buffer, i * num2 + num + 32);
			byteTransform.TransByte(statisticsSnapAndDataMark.Content2[i].Count).CopyTo(buffer, i * num2 + num + 40);
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
		if (num != 305419897)
		{
			throw new Exception("File is not LogValueLimit file, can't load data.");
		}
		int num2 = BitConverter.ToUInt16(buffer, 4);
		GenerateMode generateMode = (GenerateMode)BitConverter.ToUInt16(buffer, 6);
		int num3 = BitConverter.ToInt32(buffer, 8);
		long num4 = BitConverter.ToInt64(buffer, 12);
		long num5 = BitConverter.ToInt64(buffer, 20);
		int num6 = BitConverter.ToInt32(buffer, 28);
		base.generateMode = generateMode;
		valueCount = num5;
		ValueLimit[] array = new ValueLimit[num3];
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StartValue = byteTransform.TransDouble(buffer, i * num6 + num2);
			array[i].Current = byteTransform.TransDouble(buffer, i * num6 + num2 + 8);
			array[i].MaxValue = byteTransform.TransDouble(buffer, i * num6 + num2 + 16);
			array[i].MinValue = byteTransform.TransDouble(buffer, i * num6 + num2 + 24);
			array[i].Average = byteTransform.TransDouble(buffer, i * num6 + num2 + 32);
			array[i].Count = byteTransform.TransInt32(buffer, i * num6 + num2 + 40);
		}
		Reset(array, num4);
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
		return $"LogValueLimit[{base.GenerateMode}:{base.ArrayLength}]";
	}
}
