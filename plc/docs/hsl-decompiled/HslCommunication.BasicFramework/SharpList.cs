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

namespace HslCommunication.BasicFramework;

public class SharpList<T>
{
	private T[] array;

	private int capacity = 2048;

	private int count = 0;

	private int lastIndex = 0;

	private object objLock = new object();

	public int Count => count;

	public T this[int index]
	{
		get
		{
			if (index < 0)
			{
				throw new IndexOutOfRangeException("Index must larger than zero");
			}
			if (index >= count)
			{
				throw new IndexOutOfRangeException("Index must smaller than array length");
			}
			T val = default(T);
			lock (objLock)
			{
				if (lastIndex < count)
				{
					return array[index];
				}
				return array[index + lastIndex - count];
			}
		}
		set
		{
			if (index < 0)
			{
				throw new IndexOutOfRangeException("Index must larger than zero");
			}
			if (index >= count)
			{
				throw new IndexOutOfRangeException("Index must smaller than array length");
			}
			lock (objLock)
			{
				if (lastIndex < count)
				{
					array[index] = value;
				}
				else
				{
					array[index + lastIndex - count] = value;
				}
			}
		}
	}

	public SharpList(int count, bool appendLast = false)
	{
		if (count > 65535)
		{
			capacity = 8192;
		}
		else if (count > 8192)
		{
			capacity = 4096;
		}
		array = new T[capacity + count];
		this.count = count;
		if (appendLast)
		{
			lastIndex = count;
		}
	}

	private void addItem(T value)
	{
		if (lastIndex >= capacity + count)
		{
			T[] destinationArray = new T[capacity + count];
			Array.Copy(array, capacity, destinationArray, 0, count);
			array = destinationArray;
			lastIndex = count;
		}
		array[lastIndex++] = value;
	}

	public void Add(T value)
	{
		lock (objLock)
		{
			addItem(value);
		}
	}

	public void Add(IEnumerable<T> values)
	{
		lock (objLock)
		{
			foreach (T value in values)
			{
				addItem(value);
			}
		}
	}

	public T[] ToArray()
	{
		T[] array = null;
		lock (objLock)
		{
			if (lastIndex < count)
			{
				array = new T[lastIndex];
				Array.Copy(this.array, 0, array, 0, lastIndex);
			}
			else
			{
				array = new T[count];
				Array.Copy(this.array, lastIndex - count, array, 0, count);
			}
		}
		return array;
	}

	public T LastValue()
	{
		T result = default(T);
		lock (objLock)
		{
			if (lastIndex - 1 < count + capacity)
			{
				result = array[lastIndex - 1];
				return result;
			}
		}
		return result;
	}

	public override string ToString()
	{
		return $"SharpList<{typeof(T)}>[{capacity}]";
	}
}
