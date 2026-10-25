// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using HslCommunication.Core;

namespace HslCommunication.BasicFramework;

/// <summary>
/// 一个简单的不持久化的序号自增类，采用线程安全实现，并允许指定最大数字，将包含该最大值，到达后清空从指定数开始
/// A simple non-persistent serial number auto-increment class, which is implemented with thread safety, and allows the maximum number to be specified, which will contain the maximum number, and will be cleared from the specified number upon arrival.
/// </summary>
public sealed class SoftIncrementCount : IDisposable
{
	private long start = 0L;

	private long current = 0L;

	private long max = long.MaxValue;

	private SimpleHybirdLock hybirdLock;

	private bool disposedValue = false;

	/// <summary>
	/// 增加的单元，如果设置为0，就是不增加。如果为小于0，那就是减少，会变成负数的可能。
	/// Increased units, if set to 0, do not increase. If it is less than 0, it is a decrease and it may become a negative number.
	/// </summary>
	public int IncreaseTick { get; set; } = 1;

	public long MaxValue => max;

	/// <summary>
	/// 实例化一个自增信息的对象，包括最大值，初始值，增量值
	/// Instantiate an object with incremental information, including the maximum value and initial value, IncreaseTick
	/// </summary>
	/// <param name="max">数据的最大值，必须指定</param>
	/// <param name="start">数据的起始值，默认为0</param>
	/// <param name="tick">每次的增量值</param>
	public SoftIncrementCount(long max, long start = 0L, int tick = 1)
	{
		this.start = start;
		this.max = max;
		current = start;
		IncreaseTick = tick;
		hybirdLock = new SimpleHybirdLock();
	}

	/// <summary>
	/// 获取自增信息，获得数据之后，下一次获取将会自增，如果自增后大于最大值，则会重置为最小值，如果小于最小值，则会重置为最大值。
	/// Get the auto-increment information. After getting the data, the next acquisition will auto-increase.
	/// If the auto-increment is greater than the maximum value, it will reset to the minimum value.
	/// If the auto-increment is smaller than the minimum value, it will reset to the maximum value.
	/// </summary>
	/// <returns>计数自增后的值</returns>
	public long GetCurrentValue()
	{
		long num = 0L;
		hybirdLock.Enter();
		num = current;
		current += IncreaseTick;
		if (current > max)
		{
			current = start;
		}
		else if (current < start)
		{
			current = max;
		}
		hybirdLock.Leave();
		return num;
	}

	/// <summary>
	/// 重置当前序号的最大值，最大值应该大于初始值，如果当前值大于最大值，则当前值被重置为最大值
	/// Reset the maximum value of the current serial number. The maximum value should be greater than the initial value.
	/// If the current value is greater than the maximum value, the current value is reset to the maximum value.
	/// </summary>
	/// <param name="max">最大值</param>
	public void ResetMaxValue(long max)
	{
		hybirdLock.Enter();
		if (max > start)
		{
			if (max < current)
			{
				current = start;
			}
			this.max = max;
		}
		hybirdLock.Leave();
	}

	/// <summary>
	/// 重置当前序号的初始值，需要小于最大值，如果当前值小于初始值，则当前值被重置为初始值。
	/// To reset the initial value of the current serial number, it must be less than the maximum value.
	/// If the current value is less than the initial value, the current value is reset to the initial value.
	/// </summary>
	/// <param name="start">初始值</param>
	public void ResetStartValue(long start)
	{
		hybirdLock.Enter();
		if (start < max)
		{
			if (current < start)
			{
				current = start;
			}
			this.start = start;
		}
		hybirdLock.Leave();
	}

	/// <summary>
	/// 将当前的值重置为初始值。
	/// Reset the current value to the initial value.
	/// </summary>
	public void ResetCurrentValue()
	{
		hybirdLock.Enter();
		current = start;
		hybirdLock.Leave();
	}

	/// <summary>
	/// 将当前的值重置为指定值，该值不能大于max，如果大于max值，就会自动设置为max
	/// Reset the current value to the specified value. The value cannot be greater than max. If it is greater than max, it will be automatically set to max.
	/// </summary>
	/// <param name="value">指定的数据值</param>
	public void ResetCurrentValue(long value)
	{
		hybirdLock.Enter();
		if (value > max)
		{
			current = max;
		}
		else if (value < start)
		{
			current = start;
		}
		else
		{
			current = value;
		}
		hybirdLock.Leave();
	}

	public override string ToString()
	{
		return $"SoftIncrementCount[{current}]";
	}

	private void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				hybirdLock.Dispose();
			}
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}
}
