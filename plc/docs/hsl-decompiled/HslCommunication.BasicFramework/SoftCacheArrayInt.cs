// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.BasicFramework;

/// <summary>
/// 一个内存队列缓存的类，数据类型为Int32
/// </summary>
public sealed class SoftCacheArrayInt : SoftCacheArrayBase
{
	/// <summary>
	/// 数据的本身面貌
	/// </summary>
	private int[] DataArray = null;

	/// <summary>
	/// 实例化一个数据对象
	/// </summary>
	public SoftCacheArrayInt(int capacity, int defaultValue)
	{
		if (capacity < 10)
		{
			capacity = 10;
		}
		base.ArrayLength = capacity;
		DataArray = new int[capacity];
		DataBytes = new byte[capacity * 4];
		if (defaultValue != 0)
		{
			for (int i = 0; i < capacity; i++)
			{
				DataArray[i] = defaultValue;
			}
		}
	}

	/// <summary>
	/// 用于从保存的数据对象初始化的
	/// </summary>
	public override void LoadFromBytes(byte[] dataSave)
	{
		int count = (base.ArrayLength = dataSave.Length / 4);
		DataArray = new int[count];
		DataBytes = new byte[count * 4];
		for (int i = 0; i < count; i++)
		{
			DataArray[i] = BitConverter.ToInt32(dataSave, i * 4);
		}
	}

	/// <summary>
	/// 线程安全的添加数据
	/// </summary>
	/// <param name="value">值</param>
	public void AddValue(int value)
	{
		HybirdLock.Enter();
		for (int i = 0; i < base.ArrayLength - 1; i++)
		{
			DataArray[i] = DataArray[i + 1];
		}
		DataArray[base.ArrayLength - 1] = value;
		for (int j = 0; j < base.ArrayLength; j++)
		{
			BitConverter.GetBytes(DataArray[j]).CopyTo(DataBytes, 4 * j);
		}
		HybirdLock.Leave();
	}

	/// <summary>
	/// 安全的获取数组队列
	/// </summary>
	public int[] GetIntArray()
	{
		int[] array = null;
		HybirdLock.Enter();
		array = new int[base.ArrayLength];
		for (int i = 0; i < base.ArrayLength; i++)
		{
			array[i] = DataArray[i];
		}
		HybirdLock.Leave();
		return array;
	}
}
