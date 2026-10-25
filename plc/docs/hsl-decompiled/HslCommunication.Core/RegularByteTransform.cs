// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Text;
using HslCommunication.BasicFramework;

namespace HslCommunication.Core;

/// <summary>
/// 数据转换类的基础，提供了一些基础的方法实现，默认  的顺序，和C#的字节顺序是一致的。
/// The basis of the data conversion class provides some basic method implementations.
/// The default order of  is consistent with the byte order of C#.
/// </summary>
public class RegularByteTransform : IByteTransform
{
	/// <summary>
	/// 获取或设置数据解析的格式，可选ABCD, BADC，CDAB，DCBA格式，对int,uint,float,double,long,ulong类型有作用
	/// Get or set the format of the data analysis, optional ABCD, BADC, CDAB, DCBA format, effective for int, uint, float, double, long, ulong type
	/// </summary>
	public DataFormat DataFormat { get; set; }

	/// <summary>
	/// 获取或设置在解析字符串的时候是否将字节按照字单位反转
	/// Gets or sets whether to reverse the bytes in word units when parsing strings
	/// </summary>
	public bool IsStringReverseByteWord { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public RegularByteTransform()
	{
		DataFormat = DataFormat.DCBA;
	}

	/// <summary>
	/// 使用指定的数据解析来实例化对象
	/// Instantiate the object using the specified data parsing
	/// </summary>
	/// <param name="dataFormat">数据规则</param>
	public RegularByteTransform(DataFormat dataFormat)
	{
		DataFormat = dataFormat;
	}

	/// <summary>
	/// 从缓存中提取出bool结果，需要传入想要提取的位索引，注意：是从0开始的位索引，10则表示 buffer[1] 的第二位。
	/// To extract the bool result from the cache, you need to pass in the bit index you want to extract. Note: the bit index starts from 0, and 10 represents the second bit of buffer[1].
	/// </summary>
	/// <param name="buffer">等待提取的缓存数据</param>
	/// <param name="index">位的索引，注意：是从0开始的位索引，10则表示 buffer[1] 的第二位。</param>
	/// <returns>表示不是 true 就是 false 的 bool 数据</returns>
	public virtual bool TransBool(byte[] buffer, int index)
	{
		return buffer.GetBoolByIndex(index);
	}

	/// <summary>
	/// 从缓存中提取出bool数组结果，需要传入想要提取的位索引，注意：是从0开始的位索引，10则表示 buffer[1] 的第二位。长度为 bool 数量的长度，传入 10 则获取 10 个长度的 bool[] 数组。
	/// To extract the result of the bool array from the cache, you need to pass in the bit index you want to extract. Note: the bit index starts from 0,
	/// and 10 represents the second bit of buffer[1]. The length is the length of the number of bools. If you pass in 10, you will get a bool[] array of 10 lengths.
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">位的起始索引，需要传入想要提取的位索引，注意：是从0开始的位索引，10则表示 buffer[1] 的第二位</param>
	/// <param name="length">读取的 bool 长度，按照位为单位，传入 10 则表示获取 10 个长度的 bool[]</param>
	/// <returns>bool数组</returns>
	public virtual bool[] TransBool(byte[] buffer, int index, int length)
	{
		bool[] array = new bool[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = buffer.GetBoolByIndex(i + index);
		}
		return array;
	}

	/// <summary>
	/// 从缓存中提取byte结果，需要指定起始的字节索引
	/// To extract the byte result from the cache, you need to specify the starting byte index
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <returns>byte对象</returns>
	public virtual byte TransByte(byte[] buffer, int index)
	{
		return buffer[index];
	}

	/// <summary>
	/// 从缓存中提取byte数组结果，需要指定起始的字节索引，以及指定读取的字节长度
	/// To extract the byte array result from the cache, you need to specify the starting byte index and the byte length to be read
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <param name="length">读取的数组长度</param>
	/// <returns>byte数组对象</returns>
	public virtual byte[] TransByte(byte[] buffer, int index, int length)
	{
		byte[] buffer2 = new byte[length];
		Array.Copy(buffer, index, buffer2, 0, length);
		return buffer2;
	}

	/// <summary>
	/// 从缓存中提取short结果，需要指定起始的字节索引，按照字节为单位，一个short占用两个字节
	/// To extract short results from the cache, you need to specify the starting byte index, in bytes, A short occupies two bytes
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <returns>short对象</returns>
	public virtual short TransInt16(byte[] buffer, int index)
	{
		return BitConverter.ToInt16(ByteTransDataFormat2(buffer, index), 0);
	}

	/// <summary>
	/// 从缓存中提取short数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 short 数组的长度，如果传入 10 ，则表示提取 10 个连续的 short 数据，该数据共占用 20 字节。
	/// To extract the result of the short array from the cache, you need to specify the starting byte index, in bytes,
	/// and then specify the length of the extracted short array. If 10 is passed in, it means to extract 10 consecutive short data. Occupies 20 bytes.
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <param name="length">读取的数组长度</param>
	/// <returns>short数组对象</returns>
	public virtual short[] TransInt16(byte[] buffer, int index, int length)
	{
		short[] array = new short[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = TransInt16(buffer, index + 2 * i);
		}
		return array;
	}

	/// <summary>
	/// 从缓存中提取short二维数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 short 数组的行和列的长度，按照 short 为单位的个数。
	/// To extract the result of a short two-dimensional array from the cache, you need to specify the starting byte index, in bytes,
	/// and then specify the length of the rows and columns of the extracted short array, in terms of the number of shorts.
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <param name="row">二维数组行</param>
	/// <param name="col">二维数组列</param>
	/// <returns>二维short数组</returns>
	public short[,] TransInt16(byte[] buffer, int index, int row, int col)
	{
		return HslHelper.CreateTwoArrayFromOneArray(TransInt16(buffer, index, row * col), row, col);
	}

	/// <summary>
	/// 从缓存中提取ushort结果，需要指定起始的字节索引，按照字节为单位，一个ushort占用两个字节
	/// To extract ushort results from the cache, you need to specify the starting byte index, in bytes, A ushort occupies two bytes
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <returns>ushort对象</returns>
	public virtual ushort TransUInt16(byte[] buffer, int index)
	{
		return BitConverter.ToUInt16(ByteTransDataFormat2(buffer, index), 0);
	}

	/// <summary>
	/// 从缓存中提取ushort数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 ushort 数组的长度，如果传入 10 ，则表示提取 10 个连续的 ushort 数据，该数据共占用 20 字节。
	/// To extract the ushort array result from the cache, you need to specify the starting byte index, in bytes,
	/// and then specify the length of the extracted ushort array. If 10 is passed in, it means to extract 10 consecutive ushort data. Occupies 20 bytes.
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <param name="length">读取的数组长度</param>
	/// <returns>ushort数组对象</returns>
	public virtual ushort[] TransUInt16(byte[] buffer, int index, int length)
	{
		ushort[] array = new ushort[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = TransUInt16(buffer, index + 2 * i);
		}
		return array;
	}

	/// <summary>
	/// 从缓存中提取ushort二维数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 ushort 数组的行和列的长度，按照 ushort 为单位的个数。
	/// To extract the result of the ushort two-dimensional array from the cache, you need to specify the starting byte index, in bytes,
	/// and then specify the length of the rows and columns of the extracted ushort array, in terms of the number of ushorts.
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <param name="row">二维数组行</param>
	/// <param name="col">二维数组列</param>
	/// <returns>二维ushort数组</returns>
	public ushort[,] TransUInt16(byte[] buffer, int index, int row, int col)
	{
		return HslHelper.CreateTwoArrayFromOneArray(TransUInt16(buffer, index, row * col), row, col);
	}

	/// <summary>
	/// 从缓存中提取int结果，需要指定起始的字节索引，按照字节为单位，一个int占用四个字节
	/// To extract the int result from the cache, you need to specify the starting byte index, in bytes, A int occupies four bytes
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <returns>int对象</returns>
	public virtual int TransInt32(byte[] buffer, int index)
	{
		return BitConverter.ToInt32(ByteTransDataFormat4(buffer, index), 0);
	}

	/// <summary>
	/// 从缓存中提取int数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 int 数组的长度，如果传入 10 ，则表示提取 10 个连续的 int 数据，该数据共占用 40 字节。
	/// To extract the int array result from the cache, you need to specify the starting byte index, in bytes,
	/// and then specify the length of the extracted int array. If 10 is passed in, it means to extract 10 consecutive int data. Occupies 40 bytes.
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <param name="length">读取的数组长度</param>
	/// <returns>int数组对象</returns>
	public virtual int[] TransInt32(byte[] buffer, int index, int length)
	{
		int[] array = new int[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = TransInt32(buffer, index + 4 * i);
		}
		return array;
	}

	/// <summary>
	/// 从缓存中提取int二维数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 int 数组的行和列的长度，按照 int 为单位的个数。
	/// To extract the result of an int two-dimensional array from the cache, you need to specify the starting byte index,
	/// in bytes, and then specify the length of the rows and columns of the extracted int array, in the number of int units.
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <param name="row">二维数组行</param>
	/// <param name="col">二维数组列</param>
	/// <returns>二维int数组</returns>
	public int[,] TransInt32(byte[] buffer, int index, int row, int col)
	{
		return HslHelper.CreateTwoArrayFromOneArray(TransInt32(buffer, index, row * col), row, col);
	}

	/// <summary>
	/// 从缓存中提取uint结果，需要指定起始的字节索引，按照字节为单位，一个uint占用四个字节
	/// To extract uint results from the cache, you need to specify the starting byte index, in bytes, A uint occupies four bytes
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <returns>uint对象</returns>
	public virtual uint TransUInt32(byte[] buffer, int index)
	{
		return BitConverter.ToUInt32(ByteTransDataFormat4(buffer, index), 0);
	}

	/// <summary>
	/// 从缓存中提取uint数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 uint 数组的长度，如果传入 10 ，则表示提取 10 个连续的 uint 数据，该数据共占用 40 字节。
	/// To extract the uint array result from the cache, you need to specify the starting byte index, in bytes,
	/// and then specify the length of the extracted uint array. If 10 is passed in, it means to extract 10 consecutive uint data. Occupies 40 bytes.
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <param name="length">读取的数组长度</param>
	/// <returns>uint数组对象</returns>
	public virtual uint[] TransUInt32(byte[] buffer, int index, int length)
	{
		uint[] array = new uint[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = TransUInt32(buffer, index + 4 * i);
		}
		return array;
	}

	/// <summary>
	/// 从缓存中提取uint二维数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 uint 数组的行和列的长度，按照 uint 为单位的个数。
	/// To extract the result of a uint two-dimensional array from the cache, you need to specify the starting byte index,
	/// in bytes, and then specify the length of the rows and columns of the extracted uint array, in terms of uint as the unit.
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <param name="row">二维数组行</param>
	/// <param name="col">二维数组列</param>
	/// <returns>uint二维数组对象</returns>
	public uint[,] TransUInt32(byte[] buffer, int index, int row, int col)
	{
		return HslHelper.CreateTwoArrayFromOneArray(TransUInt32(buffer, index, row * col), row, col);
	}

	/// <summary>
	/// 从缓存中提取long结果，需要指定起始的字节索引，按照字节为单位，一个long占用八个字节
	/// To extract the long result from the cache, you need to specify the starting byte index, in bytes, A long occupies eight bytes
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <returns>long对象</returns>
	public virtual long TransInt64(byte[] buffer, int index)
	{
		return BitConverter.ToInt64(ByteTransDataFormat8(buffer, index), 0);
	}

	/// <summary>
	/// 从缓存中提取long数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 long 数组的长度，如果传入 10 ，则表示提取 10 个连续的 long 数据，该数据共占用 80 字节。
	/// To extract the long array result from the cache, you need to specify the starting byte index, in bytes,
	/// and then specify the length of the long array to be extracted. If 10 is passed in, it means to extract 10 consecutive long data. Occupies 80 bytes.
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <param name="length">读取的数组长度</param>
	/// <returns>long数组对象</returns>
	public virtual long[] TransInt64(byte[] buffer, int index, int length)
	{
		long[] array = new long[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = TransInt64(buffer, index + 8 * i);
		}
		return array;
	}

	/// <summary>
	/// 从缓存中提取long二维数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 long 数组的行和列的长度，按照 long 为单位的个数。
	/// To extract the result of a long two-dimensional array from the cache, you need to specify the starting byte index, in bytes,
	/// and then specify the length of the rows and columns of the extracted long array, in long as the number of units.
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <param name="row">二维数组行</param>
	/// <param name="col">二维数组列</param>
	/// <returns>long二维数组对象</returns>
	public long[,] TransInt64(byte[] buffer, int index, int row, int col)
	{
		return HslHelper.CreateTwoArrayFromOneArray(TransInt64(buffer, index, row * col), row, col);
	}

	/// <summary>
	/// 从缓存中提取ulong结果，需要指定起始的字节索引，按照字节为单位，一个ulong占用八个字节
	/// To extract the ulong result from the cache, you need to specify the starting byte index, in bytes, A ulong occupies eight bytes
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <returns>ulong对象</returns>
	public virtual ulong TransUInt64(byte[] buffer, int index)
	{
		return BitConverter.ToUInt64(ByteTransDataFormat8(buffer, index), 0);
	}

	/// <summary>
	/// 从缓存中提取ulong数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 ulong 数组的长度，如果传入 10 ，则表示提取 10 个连续的 ulong 数据，该数据共占用 80 字节。
	/// To extract the ulong array result from the cache, you need to specify the starting byte index, in bytes,
	/// and then specify the length of the extracted ulong array. If 10 is passed in, it means to extract 10 consecutive ulong data. Occupies 80 bytes.
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <param name="length">读取的数组长度</param>
	/// <returns>ulong数组对象</returns>
	public virtual ulong[] TransUInt64(byte[] buffer, int index, int length)
	{
		ulong[] array = new ulong[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = TransUInt64(buffer, index + 8 * i);
		}
		return array;
	}

	/// <summary>
	/// 从缓存中提取ulong二维数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 ulong 数组的行和列的长度，按照 ulong 为单位的个数。
	/// To extract the result of the ulong two-dimensional array from the cache, you need to specify the starting byte index, in bytes,
	/// and then specify the length of the rows and columns of the extracted ulong array, in the number of ulong units.
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <param name="row">二维数组行</param>
	/// <param name="col">二维数组列</param>
	/// <returns>ulong二维数组对象</returns>
	public ulong[,] TransUInt64(byte[] buffer, int index, int row, int col)
	{
		return HslHelper.CreateTwoArrayFromOneArray(TransUInt64(buffer, index, row * col), row, col);
	}

	/// <summary>
	/// 从缓存中提取float结果，需要指定起始的字节索引，按照字节为单位，一个float占用四个字节
	/// To extract the float result from the cache, you need to specify the starting byte index, in units of bytes, A float occupies four bytes
	/// </summary>
	/// <param name="buffer">缓存对象</param>
	/// <param name="index">索引位置</param>
	/// <returns>float对象</returns>
	public virtual float TransSingle(byte[] buffer, int index)
	{
		return BitConverter.ToSingle(ByteTransDataFormat4(buffer, index), 0);
	}

	/// <summary>
	/// 从缓存中提取float数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 float 数组的长度，如果传入 10 ，则表示提取 10 个连续的 float 数据，该数据共占用 40 字节。
	/// To extract the result of the float array from the cache, you need to specify the starting byte index, in bytes,
	/// and then specify the length of the extracted float array. If 10 is passed in, it means that 10 consecutive float data are extracted. Occupies 40 bytes.
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <param name="length">读取的数组长度</param>
	/// <returns>float数组</returns>
	public virtual float[] TransSingle(byte[] buffer, int index, int length)
	{
		float[] array = new float[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = TransSingle(buffer, index + 4 * i);
		}
		return array;
	}

	/// <summary>
	/// 从缓存中提取float二维数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 float 数组的行和列的长度，按照 float 为单位的个数。
	/// To extract the result of a float two-dimensional array from the cache, you need to specify the starting byte index, in bytes,
	/// and then specify the length of the rows and columns of the extracted float array, in terms of the number of floats.
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <param name="row">二维数组行</param>
	/// <param name="col">二维数组列</param>
	/// <returns>float二维数组对象</returns>
	public float[,] TransSingle(byte[] buffer, int index, int row, int col)
	{
		return HslHelper.CreateTwoArrayFromOneArray(TransSingle(buffer, index, row * col), row, col);
	}

	/// <summary>
	/// 从缓存中提取double结果，需要指定起始的字节索引，按照字节为单位，一个double占用八个字节
	/// To extract the double result from the cache, you need to specify the starting byte index, in bytes, A double occupies eight bytes
	/// </summary>
	/// <param name="buffer">缓存对象</param>
	/// <param name="index">索引位置</param>
	/// <returns>double对象</returns>
	public virtual double TransDouble(byte[] buffer, int index)
	{
		return BitConverter.ToDouble(ByteTransDataFormat8(buffer, index), 0);
	}

	/// <summary>
	/// 从缓存中提取double数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 double 数组的长度，如果传入 10 ，则表示提取 10 个连续的 double 数据，该数据共占用 80 字节。
	/// To extract the double array result from the cache, you need to specify the starting byte index, in bytes,
	/// and then specify the length of the extracted double array. If 10 is passed in, it means to extract 10 consecutive double data. Occupies 80 bytes.
	/// </summary>
	/// <param name="buffer">缓存对象</param>
	/// <param name="index">索引位置</param>
	/// <param name="length">读取的数组长度</param>
	/// <returns>double数组</returns>
	public virtual double[] TransDouble(byte[] buffer, int index, int length)
	{
		double[] array = new double[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = TransDouble(buffer, index + 8 * i);
		}
		return array;
	}

	/// <summary>
	/// 从缓存中提取double二维数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 double 数组的行和列的长度，按照 double 为单位的个数。
	/// To extract the result of a double two-dimensional array from the cache, you need to specify the starting byte index, in bytes,
	/// and then specify the length of the rows and columns of the extracted double array, in terms of the number of doubles.
	/// </summary>
	/// <param name="buffer">缓存数据</param>
	/// <param name="index">索引位置</param>
	/// <param name="row">二维数组行</param>
	/// <param name="col">二维数组列</param>
	/// <returns>double二维数组对象</returns>
	public double[,] TransDouble(byte[] buffer, int index, int row, int col)
	{
		return HslHelper.CreateTwoArrayFromOneArray(TransDouble(buffer, index, row * col), row, col);
	}

	/// <summary>
	/// 从缓存中的部分字节数组转化为string结果，使用指定的编码，指定起始的字节索引，字节长度信息。
	/// Convert a part of the byte array in the buffer into a string result, use the specified encoding, specify the starting byte index, and byte length information.
	/// </summary>
	/// <param name="buffer">缓存对象</param>
	/// <param name="index">索引位置</param>
	/// <param name="length">byte数组长度</param>
	/// <param name="encoding">字符串的编码</param>
	/// <returns>string对象</returns>
	public virtual string TransString(byte[] buffer, int index, int length, Encoding encoding)
	{
		byte[] array = TransByte(buffer, index, length);
		if (IsStringReverseByteWord)
		{
			return encoding.GetString(SoftBasic.BytesReverseByWord(array));
		}
		return encoding.GetString(array);
	}

	/// <summary>
	/// 从缓存中提取string结果，使用指定的编码将全部的缓存转为字符串
	/// Extract the string result from the cache, use the specified encoding to convert all the cache into a string
	/// </summary>
	/// <param name="buffer">缓存对象</param>
	/// <param name="encoding">字符串的编码</param>
	/// <returns>string对象</returns>
	public virtual string TransString(byte[] buffer, Encoding encoding)
	{
		return encoding.GetString(buffer);
	}

	/// <summary>
	/// bool变量转化缓存数据，一般来说单bool只能转化为0x01 或是 0x00
	/// The bool variable is converted to cache data, a single bool can only be converted to 0x01 or 0x00
	/// </summary>
	/// <param name="value">等待转化的数据</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(bool value)
	{
		return TransByte(new bool[1] { value });
	}

	/// <summary>
	/// 将bool数组变量转化缓存数据，如果数组长度不满足8的倍数，则自动补0操作。
	/// Convert the bool array variable to the cache data. If the length of the array does not meet a multiple of 8, it will automatically add 0.
	/// </summary>
	/// <param name="values">等待转化的数组</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(bool[] values)
	{
		return (values == null) ? null : SoftBasic.BoolArrayToByte(values);
	}

	/// <summary>
	/// 将byte变量转化缓存数据
	/// Convert byte variables into cached data
	/// </summary>
	/// <param name="value">等待转化的数据</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(byte value)
	{
		return new byte[1] { value };
	}

	/// <summary>
	/// short变量转化缓存数据，一个short数据可以转为2个字节的byte数组
	/// Short variable is converted to cache data, a short data can be converted into a 2-byte byte array
	/// </summary>
	/// <param name="value">等待转化的数据</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(short value)
	{
		return TransByte(new short[1] { value });
	}

	/// <summary>
	/// short数组变量转化缓存数据，n个长度的short数组，可以转为2*n个长度的byte数组
	/// The short array variable transforms the buffered data, a short array of n lengths can be converted into a byte array of 2*n lengths
	/// </summary>
	/// <param name="values">等待转化的数组</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(short[] values)
	{
		if (values == null)
		{
			return null;
		}
		byte[] buffer = new byte[values.Length * 2];
		for (int i = 0; i < values.Length; i++)
		{
			ByteTransDataFormat2(BitConverter.GetBytes(values[i])).CopyTo(buffer, 2 * i);
		}
		return buffer;
	}

	/// <summary>
	/// ushort变量转化缓存数据，一个ushort数据可以转为2个字节的Byte数组
	/// ushort variable converts buffer data, a ushort data can be converted into a 2-byte Byte array
	/// </summary>
	/// <param name="value">等待转化的数据</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(ushort value)
	{
		return TransByte(new ushort[1] { value });
	}

	/// <summary>
	/// ushort数组变量转化缓存数据，n个长度的ushort数组，可以转为2*n个长度的byte数组
	/// The ushort array variable transforms the buffer data, the ushort array of n length can be converted into a byte array of 2*n length
	/// </summary>
	/// <param name="values">等待转化的数组</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(ushort[] values)
	{
		if (values == null)
		{
			return null;
		}
		byte[] buffer = new byte[values.Length * 2];
		for (int i = 0; i < values.Length; i++)
		{
			ByteTransDataFormat2(BitConverter.GetBytes(values[i])).CopyTo(buffer, 2 * i);
		}
		return buffer;
	}

	/// <summary>
	/// int变量转化缓存数据，一个int数据可以转为4个字节的byte数组
	/// Int variable converts cache data, an int data can be converted into a 4-byte byte array
	/// </summary>
	/// <param name="value">等待转化的数据</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(int value)
	{
		return TransByte(new int[1] { value });
	}

	/// <summary>
	/// int数组变量转化缓存数据，n个长度的int数组，可以转为4*n个长度的byte数组
	/// The int array variable transforms the cache data, the int array of n length can be converted to the byte array of 4*n length
	/// </summary>
	/// <param name="values">等待转化的数组</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(int[] values)
	{
		if (values == null)
		{
			return null;
		}
		byte[] buffer = new byte[values.Length * 4];
		for (int i = 0; i < values.Length; i++)
		{
			ByteTransDataFormat4(BitConverter.GetBytes(values[i])).CopyTo(buffer, 4 * i);
		}
		return buffer;
	}

	/// <summary>
	/// uint变量转化缓存数据，一个uint数据可以转为4个字节的byte数组
	/// uint variable converts buffer data, a uint data can be converted into a 4-byte byte array
	/// </summary>
	/// <param name="value">等待转化的数据</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(uint value)
	{
		return TransByte(new uint[1] { value });
	}

	/// <summary>
	/// uint数组变量转化缓存数据，n个长度的uint数组，可以转为4*n个长度的byte数组
	/// uint array variable converts buffer data, uint array of n length can be converted to byte array of 4*n length
	/// </summary>
	/// <param name="values">等待转化的数组</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(uint[] values)
	{
		if (values == null)
		{
			return null;
		}
		byte[] buffer = new byte[values.Length * 4];
		for (int i = 0; i < values.Length; i++)
		{
			ByteTransDataFormat4(BitConverter.GetBytes(values[i])).CopyTo(buffer, 4 * i);
		}
		return buffer;
	}

	/// <summary>
	/// long变量转化缓存数据，一个long数据可以转为8个字节的byte数组
	/// Long variable is converted into cache data, a long data can be converted into 8-byte byte array
	/// </summary>
	/// <param name="value">等待转化的数据</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(long value)
	{
		return TransByte(new long[1] { value });
	}

	/// <summary>
	/// long数组变量转化缓存数据，n个长度的long数组，可以转为8*n个长度的byte数组
	/// The long array variable transforms the buffer data, the long array of n length can be converted into the byte array of 8*n length
	/// </summary>
	/// <param name="values">等待转化的数组</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(long[] values)
	{
		if (values == null)
		{
			return null;
		}
		byte[] buffer = new byte[values.Length * 8];
		for (int i = 0; i < values.Length; i++)
		{
			ByteTransDataFormat8(BitConverter.GetBytes(values[i])).CopyTo(buffer, 8 * i);
		}
		return buffer;
	}

	/// <summary>
	/// ulong变量转化缓存数据，一个ulong数据可以转为8个字节的byte数组
	/// Ulong variable converts cache data, a ulong data can be converted into 8-byte byte array
	/// </summary>
	/// <param name="value">等待转化的数据</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(ulong value)
	{
		return TransByte(new ulong[1] { value });
	}

	/// <summary>
	/// ulong数组变量转化缓存数据，n个长度的ulong数组，可以转为8*n个长度的byte数组
	/// The ulong array variable transforms the buffer data, the ulong array of n length can be converted to the byte array of 8*n length
	/// </summary>
	/// <param name="values">等待转化的数组</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(ulong[] values)
	{
		if (values == null)
		{
			return null;
		}
		byte[] buffer = new byte[values.Length * 8];
		for (int i = 0; i < values.Length; i++)
		{
			ByteTransDataFormat8(BitConverter.GetBytes(values[i])).CopyTo(buffer, 8 * i);
		}
		return buffer;
	}

	/// <summary>
	/// float变量转化缓存数据，一个float数据可以转为4个字节的byte数组
	/// Float variable is converted into cache data, a float data can be converted into a 4-byte byte array
	/// </summary>
	/// <param name="value">等待转化的数据</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(float value)
	{
		return TransByte(new float[1] { value });
	}

	/// <summary>
	/// float数组变量转化缓存数据，n个长度的float数组，可以转为4*n个长度的byte数组
	/// Float array variable converts buffer data, n-length float array can be converted into 4*n-length byte array
	/// </summary>
	/// <param name="values">等待转化的数组</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(float[] values)
	{
		if (values == null)
		{
			return null;
		}
		byte[] buffer = new byte[values.Length * 4];
		for (int i = 0; i < values.Length; i++)
		{
			ByteTransDataFormat4(BitConverter.GetBytes(values[i])).CopyTo(buffer, 4 * i);
		}
		return buffer;
	}

	/// <summary>
	/// double变量转化缓存数据，一个double数据可以转为8个字节的byte数组
	/// The double variable is converted to cache data, a double data can be converted into an 8-byte byte array
	/// </summary>
	/// <param name="value">等待转化的数据</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(double value)
	{
		return TransByte(new double[1] { value });
	}

	/// <summary>
	/// double数组变量转化缓存数据，n个长度的double数组，可以转为8*n个长度的byte数组
	/// The double array variable transforms the buffer data, the double array of n length can be converted to the byte array of 8*n length
	/// </summary>
	/// <param name="values">等待转化的数组</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(double[] values)
	{
		if (values == null)
		{
			return null;
		}
		byte[] buffer = new byte[values.Length * 8];
		for (int i = 0; i < values.Length; i++)
		{
			ByteTransDataFormat8(BitConverter.GetBytes(values[i])).CopyTo(buffer, 8 * i);
		}
		return buffer;
	}

	/// <summary>
	/// 使用指定的编码字符串转化缓存数据
	/// Use the specified encoding string to convert the cached data
	/// </summary>
	/// <param name="value">等待转化的数据</param>
	/// <param name="encoding">字符串的编码方式</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(string value, Encoding encoding)
	{
		if (value == null)
		{
			return null;
		}
		byte[] bytes = encoding.GetBytes(value);
		return IsStringReverseByteWord ? SoftBasic.BytesReverseByWord(bytes) : bytes;
	}

	/// <summary>
	/// 使用指定的编码字符串转化缓存数据，指定转换之后的字节长度信息
	/// Use the specified encoding string to convert the cached data, specify the byte length information after conversion
	/// </summary>
	/// <param name="value">等待转化的数据</param>
	/// <param name="length">转换之后的数据长度</param>
	/// <param name="encoding">字符串的编码方式</param>
	/// <returns>buffer数据</returns>
	public virtual byte[] TransByte(string value, int length, Encoding encoding)
	{
		if (value == null)
		{
			return null;
		}
		byte[] bytes = encoding.GetBytes(value);
		return IsStringReverseByteWord ? SoftBasic.ArrayExpandToLength(SoftBasic.BytesReverseByWord(bytes), length) : SoftBasic.ArrayExpandToLength(bytes, length);
	}

	/// <summary>
	/// 反转两个字节的数据信息
	/// </summary>
	/// <param name="value">原始字节数据</param>
	/// <param name="index">起始的索引</param>
	/// <returns>反转后的字节</returns>
	protected virtual byte[] ByteTransDataFormat2(byte[] value, int index = 0)
	{
		byte[] buffer = new byte[2];
		switch (DataFormat)
		{
		case DataFormat.ABCD:
		case DataFormat.CDAB:
			buffer[0] = value[index + 1];
			buffer[1] = value[index];
			break;
		case DataFormat.BADC:
		case DataFormat.DCBA:
			buffer[0] = value[index];
			buffer[1] = value[index + 1];
			break;
		}
		return buffer;
	}

	/// <summary>
	/// 反转多字节的数据信息
	/// </summary>
	/// <param name="value">数据字节</param>
	/// <param name="index">起始索引，默认值为0</param>
	/// <returns>实际字节信息</returns>
	protected virtual byte[] ByteTransDataFormat4(byte[] value, int index = 0)
	{
		byte[] buffer = new byte[4];
		switch (DataFormat)
		{
		case DataFormat.ABCD:
			buffer[0] = value[index + 3];
			buffer[1] = value[index + 2];
			buffer[2] = value[index + 1];
			buffer[3] = value[index];
			break;
		case DataFormat.BADC:
			buffer[0] = value[index + 2];
			buffer[1] = value[index + 3];
			buffer[2] = value[index];
			buffer[3] = value[index + 1];
			break;
		case DataFormat.CDAB:
			buffer[0] = value[index + 1];
			buffer[1] = value[index];
			buffer[2] = value[index + 3];
			buffer[3] = value[index + 2];
			break;
		case DataFormat.DCBA:
			buffer[0] = value[index];
			buffer[1] = value[index + 1];
			buffer[2] = value[index + 2];
			buffer[3] = value[index + 3];
			break;
		}
		return buffer;
	}

	/// <summary>
	/// 反转多字节的数据信息
	/// </summary>
	/// <param name="value">数据字节</param>
	/// <param name="index">起始索引，默认值为0</param>
	/// <returns>实际字节信息</returns>
	protected virtual byte[] ByteTransDataFormat8(byte[] value, int index = 0)
	{
		byte[] buffer = new byte[8];
		switch (DataFormat)
		{
		case DataFormat.ABCD:
			buffer[0] = value[index + 7];
			buffer[1] = value[index + 6];
			buffer[2] = value[index + 5];
			buffer[3] = value[index + 4];
			buffer[4] = value[index + 3];
			buffer[5] = value[index + 2];
			buffer[6] = value[index + 1];
			buffer[7] = value[index];
			break;
		case DataFormat.BADC:
			buffer[0] = value[index + 6];
			buffer[1] = value[index + 7];
			buffer[2] = value[index + 4];
			buffer[3] = value[index + 5];
			buffer[4] = value[index + 2];
			buffer[5] = value[index + 3];
			buffer[6] = value[index];
			buffer[7] = value[index + 1];
			break;
		case DataFormat.CDAB:
			buffer[0] = value[index + 1];
			buffer[1] = value[index];
			buffer[2] = value[index + 3];
			buffer[3] = value[index + 2];
			buffer[4] = value[index + 5];
			buffer[5] = value[index + 4];
			buffer[6] = value[index + 7];
			buffer[7] = value[index + 6];
			break;
		case DataFormat.DCBA:
			buffer[0] = value[index];
			buffer[1] = value[index + 1];
			buffer[2] = value[index + 2];
			buffer[3] = value[index + 3];
			buffer[4] = value[index + 4];
			buffer[5] = value[index + 5];
			buffer[6] = value[index + 6];
			buffer[7] = value[index + 7];
			break;
		}
		return buffer;
	}

	/// <summary>
	/// 根据指定的格式，来实例化一个新的对象，除了不同，其他都相同
	/// According to the specified  format, to instantiate a new object, except that  is different, everything else is the same
	/// </summary>
	/// <param name="dataFormat">数据格式</param>
	/// <returns>新的对象</returns>
	public virtual IByteTransform CreateByDateFormat(DataFormat dataFormat)
	{
		return new RegularByteTransform(dataFormat)
		{
			IsStringReverseByteWord = IsStringReverseByteWord
		};
	}

	public override string ToString()
	{
		return $"RegularByteTransform[{DataFormat}]";
	}
}
