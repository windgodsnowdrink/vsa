// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using HslCommunication.Reflection;

namespace HslCommunication.LogNet;

public class LogStatisticsBase<T>
{
	private T[] statistics = null;

	protected GenerateMode generateMode = GenerateMode.ByEveryDay;

	private int arrayLength = 30;

	private long lastDataMark = -1L;

	private object lockStatistics;

	public GenerateMode GenerateMode => generateMode;

	public int ArrayLength => arrayLength;

	public LogStatisticsBase(GenerateMode generateMode, int arrayLength)
	{
		this.generateMode = generateMode;
		this.arrayLength = arrayLength;
		if (this.arrayLength >= 0)
		{
			statistics = new T[arrayLength];
		}
		else
		{
			statistics = new T[1024];
		}
		lastDataMark = GetDataMarkFromDateTime(DateTime.Now);
		lockStatistics = new object();
	}

	public void Reset(T[] statistics, long lastDataMark)
	{
		if (arrayLength >= 0)
		{
			if (statistics.Length > arrayLength)
			{
				Array.Copy(statistics, statistics.Length - arrayLength, this.statistics, 0, arrayLength);
			}
			else if (statistics.Length < arrayLength)
			{
				Array.Copy(statistics, 0, this.statistics, arrayLength - statistics.Length, statistics.Length);
			}
			else
			{
				this.statistics = statistics;
			}
			arrayLength = statistics.Length;
		}
		else
		{
			this.statistics = statistics;
		}
		this.lastDataMark = lastDataMark;
	}

	protected void StatisticsCustomAction(Func<T, T> newValue)
	{
		lock (lockStatistics)
		{
			long dataMarkFromDateTime = GetDataMarkFromDateTime(DateTime.Now);
			if (lastDataMark != dataMarkFromDateTime)
			{
				int times = (int)(dataMarkFromDateTime - lastDataMark);
				statistics = GetLeftMoveTimes(times);
				lastDataMark = dataMarkFromDateTime;
			}
			statistics[statistics.Length - 1] = newValue(statistics[statistics.Length - 1]);
		}
	}

	protected void StatisticsCustomAction(Func<T, T> newValue, DateTime time)
	{
		lock (lockStatistics)
		{
			long dataMarkFromDateTime = GetDataMarkFromDateTime(DateTime.Now);
			if (lastDataMark != dataMarkFromDateTime)
			{
				int num = (int)(dataMarkFromDateTime - lastDataMark);
				if (num > 0)
				{
					statistics = GetLeftMoveTimes(num);
				}
				lastDataMark = dataMarkFromDateTime;
			}
			long dataMarkFromDateTime2 = GetDataMarkFromDateTime(time);
			if (dataMarkFromDateTime2 <= dataMarkFromDateTime)
			{
				int num2 = (int)(dataMarkFromDateTime2 - (dataMarkFromDateTime - statistics.Length + 1));
				if (num2 >= 0 && num2 < statistics.Length)
				{
					statistics[num2] = newValue(statistics[num2]);
				}
			}
		}
	}

	[HslMqttApi(Description = "Get a data snapshot of the current statistics")]
	public T[] GetStatisticsSnapshot()
	{
		return GetStatisticsSnapAndDataMark().Content2;
	}

	[HslMqttApi(Description = "Get a snapshot of statistical data information according to the specified time range")]
	public T[] GetStatisticsSnapshotByTime(DateTime start, DateTime finish)
	{
		if (finish <= start)
		{
			return new T[0];
		}
		lock (lockStatistics)
		{
			long dataMarkFromDateTime = GetDataMarkFromDateTime(DateTime.Now);
			if (lastDataMark != dataMarkFromDateTime)
			{
				int times = (int)(dataMarkFromDateTime - lastDataMark);
				statistics = GetLeftMoveTimes(times);
				lastDataMark = dataMarkFromDateTime;
			}
			long num = dataMarkFromDateTime - statistics.Length + 1;
			long num2 = GetDataMarkFromDateTime(start);
			long num3 = GetDataMarkFromDateTime(finish);
			if (num2 < num)
			{
				num2 = num;
			}
			if (num3 > dataMarkFromDateTime)
			{
				num3 = dataMarkFromDateTime;
			}
			int num4 = (int)(num2 - num);
			int num5 = (int)(num3 - num2 + 1);
			if (num2 == num3)
			{
				return new T[1] { statistics[num4] };
			}
			T[] array = new T[num5];
			for (int i = 0; i < num5; i++)
			{
				array[i] = statistics[num4 + i];
			}
			return array;
		}
	}

	public OperateResult<long, T[]> GetStatisticsSnapAndDataMark()
	{
		lock (lockStatistics)
		{
			long dataMarkFromDateTime = GetDataMarkFromDateTime(DateTime.Now);
			if (lastDataMark != dataMarkFromDateTime)
			{
				int times = (int)(dataMarkFromDateTime - lastDataMark);
				statistics = GetLeftMoveTimes(times);
				lastDataMark = dataMarkFromDateTime;
			}
			return OperateResult.CreateSuccessResult(dataMarkFromDateTime, statistics.CopyArray());
		}
	}

	[HslMqttApi(Description = "Obtain the latest data mark information according to the time mode of current data statistics")]
	public long GetDataMarkFromTimeNow()
	{
		return GetDataMarkFromDateTime(DateTime.Now);
	}

	[HslMqttApi(Description = "According to the specified time, get the data mark information specified at that time")]
	public long GetDataMarkFromDateTime(DateTime dateTime)
	{
		return generateMode switch
		{
			GenerateMode.ByEveryMinute => GetMinuteFromTime(dateTime), 
			GenerateMode.ByEveryHour => GetHourFromTime(dateTime), 
			GenerateMode.ByEveryDay => GetDayFromTime(dateTime), 
			GenerateMode.ByEveryWeek => GetWeekFromTime(dateTime), 
			GenerateMode.ByEveryMonth => GetMonthFromTime(dateTime), 
			GenerateMode.ByEverySeason => GetSeasonFromTime(dateTime), 
			GenerateMode.ByEveryYear => GetYearFromTime(dateTime), 
			_ => GetDayFromTime(dateTime), 
		};
	}

	private long GetMinuteFromTime(DateTime dateTime)
	{
		return (long)(dateTime.Date - new DateTime(1970, 1, 1)).Days * 24L * 60 + dateTime.Hour * 60 + dateTime.Minute;
	}

	private long GetHourFromTime(DateTime dateTime)
	{
		return (long)(dateTime.Date - new DateTime(1970, 1, 1)).Days * 24L + dateTime.Hour;
	}

	private long GetDayFromTime(DateTime dateTime)
	{
		return (dateTime.Date - new DateTime(1970, 1, 1)).Days;
	}

	private long GetWeekFromTime(DateTime dateTime)
	{
		return ((long)(dateTime.Date - new DateTime(1970, 1, 1)).Days + 3L) / 7;
	}

	private long GetMonthFromTime(DateTime dateTime)
	{
		return (long)(dateTime.Year - 1970) * 12L + (dateTime.Month - 1);
	}

	private long GetSeasonFromTime(DateTime dateTime)
	{
		return (long)(dateTime.Year - 1970) * 4L + (dateTime.Month - 1) / 3;
	}

	private long GetYearFromTime(DateTime dateTime)
	{
		return dateTime.Year - 1970;
	}

	private T[] GetLeftMoveTimes(int times)
	{
		if (arrayLength >= 0)
		{
			if (times >= statistics.Length || times <= -statistics.Length)
			{
				return new T[arrayLength];
			}
			T[] array = new T[arrayLength];
			if (times >= 0)
			{
				Array.Copy(statistics, times, array, 0, statistics.Length - times);
			}
			else
			{
				Array.Copy(statistics, 0, array, -times, statistics.Length + times);
			}
			return array;
		}
		T[] array2 = new T[statistics.Length + times];
		Array.Copy(statistics, 0, array2, 0, Math.Min(statistics.Length, array2.Length));
		return array2;
	}
}
