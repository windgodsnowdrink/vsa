// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Core;

/// <summary>
/// 一个数据范围管理的的类型对象，可以方便的管理一个数据值的最大值，最小值，平均值信息。
/// A type object managed by a data range can conveniently manage the maximum, minimum, and average information of a data value.
/// </summary>
public struct ValueLimit
{
	/// <summary>
	/// 当前的数据的最大值
	/// The maximum value of the current data
	/// </summary>
	public double MaxValue { get; set; }

	/// <summary>
	/// 当前的数据的最小值
	/// The minimum value of the current data
	/// </summary>
	public double MinValue { get; set; }

	/// <summary>
	/// 当前的数据的平均值
	/// Average value of current data
	/// </summary>
	public double Average { get; set; }

	/// <summary>
	/// 当前的数据的起始值
	/// The starting value of the current data
	/// </summary>
	public double StartValue { get; set; }

	/// <summary>
	/// 当前的数据的当前值，也是最后一次更新的数据值
	/// The current value of the current data is also the last updated data value
	/// </summary>
	public double Current { get; set; }

	/// <summary>
	/// 当前的数据的更新总个数
	/// The total number of current data updates
	/// </summary>
	public int Count { get; set; }

	/// <summary>
	/// 重新设置当前的最新值，然后计算新的最大值，最小值，平均值等信息
	/// Reset the current latest value, and then calculate the new maximum, minimum, average and other information
	/// </summary>
	/// <param name="value">新的数据值信息</param>
	/// <returns>新的值对象</returns>
	public ValueLimit SetNewValue(double value)
	{
		if (!double.IsNaN(value))
		{
			if (Count == 0)
			{
				MaxValue = value;
				MinValue = value;
				Count = 1;
				Current = value;
				Average = value;
				StartValue = value;
			}
			else
			{
				if (value < MinValue)
				{
					MinValue = value;
				}
				if (value > MaxValue)
				{
					MaxValue = value;
				}
				Current = value;
				Average = ((double)Count * Average + value) / (double)(Count + 1);
				Count++;
			}
		}
		return this;
	}

	public override string ToString()
	{
		return $"Avg[{Current}]";
	}

	public static bool operator ==(ValueLimit value1, ValueLimit value2)
	{
		if (value1.Count != value2.Count)
		{
			return false;
		}
		if (value1.MaxValue != value2.MaxValue)
		{
			return false;
		}
		if (value1.MinValue != value2.MinValue)
		{
			return false;
		}
		if (value1.Current != value2.Current)
		{
			return false;
		}
		if (value1.Average != value2.Average)
		{
			return false;
		}
		if (value1.StartValue != value2.StartValue)
		{
			return false;
		}
		return true;
	}

	public static bool operator !=(ValueLimit value1, ValueLimit value2)
	{
		return !(value1 == value2);
	}

	public override bool Equals(object obj)
	{
		if (obj is ValueLimit valueLimit)
		{
			return this == valueLimit;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
