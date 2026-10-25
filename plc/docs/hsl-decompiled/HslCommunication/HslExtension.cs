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
using System.IO.Ports;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HslCommunication;

/// <summary>
/// 扩展的辅助类方法
/// </summary>
public static class HslExtension
{
	/// <summary>
	/// 字节数据转化成16进制表示的字符串
	/// Byte data into a string of 16 binary representations
	/// </summary>
	/// <param name="InBytes">字节数组</param>
	/// <returns>返回的字符串</returns>
	public static string ToHexString(this byte[] InBytes)
	{
		return SoftBasic.ByteToHexString(InBytes);
	}

	/// <summary>
	/// 字节数据转化成16进制表示的字符串
	/// Byte data into a string of 16 binary representations
	/// </summary>
	/// <param name="InBytes">字节数组</param>
	/// <param name="segment">分割符</param>
	/// <returns>返回的字符串</returns>
	public static string ToHexString(this byte[] InBytes, char segment)
	{
		return SoftBasic.ByteToHexString(InBytes, segment);
	}

	/// <summary>
	/// 字节数据转化成16进制表示的字符串
	/// Byte data into a string of 16 binary representations
	/// </summary>
	/// <param name="InBytes">字节数组</param>
	/// <param name="segment">分割符，如果设置为0，则没有分隔符信息</param>
	/// <param name="newLineCount">每隔指定数量的时候进行换行，如果小于等于0，则不进行分行显示</param>
	/// <param name="format">格式信息，默认为{0:X2}</param>
	/// <returns>返回的字符串</returns>
	public static string ToHexString(this byte[] InBytes, char segment, int newLineCount, string format = "{0:X2}")
	{
		return SoftBasic.ByteToHexString(InBytes, segment, newLineCount, format);
	}

	/// <summary>
	/// 将16进制的字符串转化成Byte数据，将检测每2个字符转化，也就是说，中间可以是任意字符
	/// Converts a 16-character string into byte data, which will detect every 2 characters converted, that is, the middle can be any character
	/// </summary>
	/// <param name="hex">十六进制的字符串，中间可以是任意的分隔符</param>
	/// <returns>转换后的字节数组</returns>
	/// <remarks>
	/// 参数举例：AA 01 34 A8
	/// </remarks>
	public static byte[] ToHexBytes(this string value)
	{
		return SoftBasic.HexStringToBytes(value);
	}

	/// <summary>
	/// 将bool数组转换到byte数组
	/// Converting a bool array to a byte array
	/// </summary>
	/// <param name="array">bool数组</param>
	/// <returns>转换后的字节数组</returns>
	public static byte[] ToByteArray(this bool[] array)
	{
		return SoftBasic.BoolArrayToByte(array);
	}

	/// <summary>
	/// 从Byte数组中提取位数组，length代表位数，例如数组 03 A1 长度10转为 [1100 0000 10]
	/// Extracts a bit array from a byte array, length represents the number of digits
	/// </summary>
	/// <param name="inBytes">原先的字节数组</param>
	/// <param name="length">想要转换的长度，如果超出自动会缩小到数组最大长度</param>
	/// <returns>转换后的bool数组</returns>
	public static bool[] ToBoolArray(this byte[] InBytes, int length)
	{
		return SoftBasic.ByteToBoolArray(InBytes, length);
	}

	public static T[] ReverseNew<T>(this T[] value)
	{
		T[] array = value.CopyArray();
		Array.Reverse(array);
		return array;
	}

	/// <summary>
	/// 从Byte数组中提取所有的位数组
	/// Extracts a bit array from a byte array, length represents the number of digits
	/// </summary>
	/// <param name="InBytes">原先的字节数组</param>
	/// <returns>转换后的bool数组</returns>
	public static bool[] ToBoolArray(this byte[] InBytes)
	{
		return SoftBasic.ByteToBoolArray(InBytes);
	}

	/// <summary>
	/// 获取Byte数组的第 bytIndex 个位置的，boolIndex偏移的bool值
	/// Get the bool value of the bytIndex position of the Byte array and the boolIndex offset
	/// </summary>
	/// <param name="bytes">字节数组信息</param>
	/// <param name="bytIndex">字节的偏移位置</param>
	/// <param name="boolIndex">指定字节的位偏移</param>
	/// <returns>bool值</returns>
	public static bool GetBoolValue(this byte[] bytes, int bytIndex, int boolIndex)
	{
		return SoftBasic.BoolOnByteIndex(bytes[bytIndex], boolIndex);
	}

	/// <summary>
	/// 获取Byte数组的第 boolIndex 偏移的bool值，这个偏移值可以为 10，就是第 1 个字节的 第3位
	/// Get the bool value of the boolIndex offset of the Byte array. The offset value can be 10, which is the third bit of the first byte
	/// </summary>
	/// <param name="bytes">字节数组信息</param>
	/// <param name="boolIndex">指定字节的位偏移</param>
	/// <returns>bool值</returns>
	public static bool GetBoolByIndex(this byte[] bytes, int boolIndex)
	{
		return SoftBasic.BoolOnByteIndex(bytes[boolIndex / 8], boolIndex % 8);
	}

	/// <summary>
	/// 获取Byte的第 boolIndex 偏移的bool值，比如3，就是第4位
	/// Get the bool value of Byte's boolIndex offset, such as 3, which is the 4th bit
	/// </summary>
	/// <param name="byt">字节信息</param>
	/// <param name="boolIndex">指定字节的位偏移</param>
	/// <returns>bool值</returns>
	public static bool GetBoolByIndex(this byte byt, int boolIndex)
	{
		return SoftBasic.BoolOnByteIndex(byt, boolIndex % 8);
	}

	/// <summary>
	/// 获取short类型数据的第 boolIndex (从0起始)偏移的bool值，比如3，就是第4位
	/// Get the bool value of the boolIndex (starting from 0) offset of the short type data, such as 3, which is the 4th bit
	/// </summary>
	/// <param name="value">short数据值</param>
	/// <param name="boolIndex">位偏移索引，从0开始</param>
	/// <returns>bool值</returns>
	public static bool GetBoolByIndex(this short value, int boolIndex)
	{
		return BitConverter.GetBytes(value).GetBoolByIndex(boolIndex);
	}

	/// <summary>
	/// 获取ushort类型数据的第 boolIndex (从0起始)偏移的bool值，比如3，就是第4位
	/// Get the bool value of the boolIndex (starting from 0) offset of the ushort type data, such as 3, which is the 4th bit
	/// </summary>
	/// <param name="value">ushort数据值</param>
	/// <param name="boolIndex">位偏移索引，从0开始</param>
	/// <returns>bool值</returns>
	public static bool GetBoolByIndex(this ushort value, int boolIndex)
	{
		return BitConverter.GetBytes(value).GetBoolByIndex(boolIndex);
	}

	/// <summary>
	/// 获取int类型数据的第 boolIndex (从0起始)偏移的bool值，比如3，就是第4位
	/// Get the bool value of the boolIndex (starting from 0) offset of the int type data, such as 3, which is the 4th bit
	/// </summary>
	/// <param name="value">int数据值</param>
	/// <param name="boolIndex">位偏移索引，从0开始</param>
	/// <returns>bool值</returns>
	public static bool GetBoolByIndex(this int value, int boolIndex)
	{
		return BitConverter.GetBytes(value).GetBoolByIndex(boolIndex);
	}

	/// <summary>
	/// 获取uint类型数据的第 boolIndex (从0起始)偏移的bool值，比如3，就是第4位
	/// Get the bool value of the boolIndex (starting from 0) offset of the uint type data, such as 3, which is the 4th bit
	/// </summary>
	/// <param name="value">uint数据值</param>
	/// <param name="boolIndex">位偏移索引，从0开始</param>
	/// <returns>bool值</returns>
	public static bool GetBoolByIndex(this uint value, int boolIndex)
	{
		return BitConverter.GetBytes(value).GetBoolByIndex(boolIndex);
	}

	/// <summary>
	/// 获取long类型数据的第 boolIndex (从0起始)偏移的bool值，比如3，就是第4位
	/// Get the bool value of the boolIndex (starting from 0) offset of the long type data, such as 3, which is the 4th bit
	/// </summary>
	/// <param name="value">long数据值</param>
	/// <param name="boolIndex">位偏移索引，从0开始</param>
	/// <returns>bool值</returns>
	public static bool GetBoolByIndex(this long value, int boolIndex)
	{
		return BitConverter.GetBytes(value).GetBoolByIndex(boolIndex);
	}

	/// <summary>
	/// 获取ulong类型数据的第 boolIndex (从0起始)偏移的bool值，比如3，就是第4位
	/// Get the bool value of the boolIndex (starting from 0) offset of the ulong type data, such as 3, which is the 4th bit
	/// </summary>
	/// <param name="value">ulong数据值</param>
	/// <param name="boolIndex">位偏移索引，从0开始</param>
	/// <returns>bool值</returns>
	public static bool GetBoolByIndex(this ulong value, int boolIndex)
	{
		return BitConverter.GetBytes(value).GetBoolByIndex(boolIndex);
	}

	/// <summary>
	/// 从字节数组里提取字符串数据，如果碰到0x00字节，就直接结束
	/// Extract string data from a byte array, and if a 0x00 byte is encountered, it ends directly
	/// </summary>
	/// <param name="buffer">原始字节信息</param>
	/// <param name="index">起始的偏移地址</param>
	/// <param name="length">字节长度信息</param>
	/// <param name="encoding">编码</param>
	/// <returns>字符串信息</returns>
	public static string GetStringOrEndChar(this byte[] buffer, int index, int length, Encoding encoding)
	{
		for (int i = index; i < index + length; i++)
		{
			if (buffer[i] == 0)
			{
				length = i - index;
				break;
			}
		}
		if (encoding == null)
		{
			return Encoding.UTF8.GetString(buffer, index, length);
		}
		return encoding.GetString(buffer, index, length);
	}

	/// <summary>
	/// 设置Byte的第 boolIndex 位的bool值，可以强制为 true 或是 false, 不影响其他的位
	/// Set the bool value of the boolIndex bit of Byte, which can be forced to true or false, without affecting other bits
	/// </summary>
	/// <param name="byt">字节信息</param>
	/// <param name="boolIndex">指定字节的位偏移</param>
	/// <param name="value">bool的值</param>
	/// <returns>修改之后的byte值</returns>
	public static byte SetBoolByIndex(this byte byt, int boolIndex, bool value)
	{
		return SoftBasic.SetBoolOnByteIndex(byt, boolIndex, value);
	}

	/// <summary>
	/// 设置Byte[]的第 boolIndex 位的bool值，可以强制为 true 或是 false, 不影响其他的位，如果是第 10 位，则表示第 1 个字节的第 2 位（都是从 0 地址开始算的）
	/// Set the bool value of the boolIndex bit of Byte[], which can be forced to true or false, without affecting other bits.
	/// If it is the 10th bit, it means the second bit of the first byte (both starting from the 0 address Calculated)
	/// </summary>
	/// <param name="buffer">字节数组信息</param>
	/// <param name="boolIndex">位偏移的索引</param>
	/// <param name="value">bool的值</param>
	public static void SetBoolByIndex(this byte[] buffer, int boolIndex, bool value)
	{
		buffer[boolIndex / 8] = buffer[boolIndex / 8].SetBoolByIndex(boolIndex % 8, value);
	}

	/// <summary>
	/// 修改short数据的某个位，并且返回修改后的值，不影响原来的值。位索引为 0~15，之外的值会引发异常
	/// Modify a bit of short data and return the modified value without affecting the original value. Bit index is 0~15, values outside will raise an exception
	/// </summary>
	/// <param name="shortValue">等待修改的short值</param>
	/// <param name="boolIndex">位索引，位索引为 0~15，之外的值会引发异常</param>
	/// <param name="value">bool值</param>
	/// <returns>修改之后的short值</returns>
	public static short SetBoolByIndex(this short shortValue, int boolIndex, bool value)
	{
		byte[] bytes = BitConverter.GetBytes(shortValue);
		bytes.SetBoolByIndex(boolIndex, value);
		return BitConverter.ToInt16(bytes, 0);
	}

	/// <summary>
	/// 修改ushort数据的某个位，并且返回修改后的值，不影响原来的值。位索引为 0~15，之外的值会引发异常
	/// Modify a bit of ushort data and return the modified value without affecting the original value. Bit index is 0~15, values outside will raise an exception
	/// </summary>
	/// <param name="ushortValue">等待修改的ushort值</param>
	/// <param name="boolIndex">位索引，位索引为 0~15，之外的值会引发异常</param>
	/// <param name="value">bool值</param>
	/// <returns>修改之后的ushort值</returns>
	public static ushort SetBoolByIndex(this ushort ushortValue, int boolIndex, bool value)
	{
		byte[] bytes = BitConverter.GetBytes(ushortValue);
		bytes.SetBoolByIndex(boolIndex, value);
		return BitConverter.ToUInt16(bytes, 0);
	}

	/// <summary>
	/// 修改int数据的某个位，并且返回修改后的值，不影响原来的值。位索引为 0~31，之外的值会引发异常
	/// Modify a bit of int data and return the modified value without affecting the original value. Bit index is 0~31, values outside will raise an exception
	/// </summary>
	/// <param name="intValue">等待修改的int值</param>
	/// <param name="boolIndex">位索引，位索引为 0~31，之外的值会引发异常</param>
	/// <param name="value">bool值</param>
	/// <returns>修改之后的int值</returns>
	public static int SetBoolByIndex(this int intValue, int boolIndex, bool value)
	{
		byte[] bytes = BitConverter.GetBytes(intValue);
		bytes.SetBoolByIndex(boolIndex, value);
		return BitConverter.ToInt32(bytes, 0);
	}

	/// <summary>
	/// 修改uint数据的某个位，并且返回修改后的值，不影响原来的值。位索引为 0~31，之外的值会引发异常
	/// Modify a bit of uint data and return the modified value without affecting the original value. Bit index is 0~31, values outside will raise an exception
	/// </summary>
	/// <param name="uintValue">等待修改的uint值</param>
	/// <param name="boolIndex">位索引，位索引为 0~31，之外的值会引发异常</param>
	/// <param name="value">bool值</param>
	/// <returns>修改之后的uint值</returns>
	public static uint SetBoolByIndex(this uint uintValue, int boolIndex, bool value)
	{
		byte[] bytes = BitConverter.GetBytes(uintValue);
		bytes.SetBoolByIndex(boolIndex, value);
		return BitConverter.ToUInt32(bytes, 0);
	}

	/// <summary>
	/// 修改long数据的某个位，并且返回修改后的值，不影响原来的值。位索引为 0~63，之外的值会引发异常
	/// Modify a bit of long data and return the modified value without affecting the original value. Bit index is 0~63, values outside will raise an exception
	/// </summary>
	/// <param name="longValue">等待修改的long值</param>
	/// <param name="boolIndex">位索引，位索引为 0~63，之外的值会引发异常</param>
	/// <param name="value">bool值</param>
	/// <returns>修改之后的long值</returns>
	public static long SetBoolByIndex(this long longValue, int boolIndex, bool value)
	{
		byte[] bytes = BitConverter.GetBytes(longValue);
		bytes.SetBoolByIndex(boolIndex, value);
		return BitConverter.ToInt64(bytes, 0);
	}

	/// <summary>
	/// 修改ulong数据的某个位，并且返回修改后的值，不影响原来的值。位索引为 0~63，之外的值会引发异常
	/// Modify a bit of ulong data and return the modified value without affecting the original value. Bit index is 0~63, values outside will raise an exception
	/// </summary>
	/// <param name="ulongValue">等待修改的ulong值</param>
	/// <param name="boolIndex">位索引，位索引为 0~63，之外的值会引发异常</param>
	/// <param name="value">bool值</param>
	/// <returns>修改之后的ulong值</returns>
	public static ulong SetBoolByIndex(this ulong ulongValue, int boolIndex, bool value)
	{
		byte[] bytes = BitConverter.GetBytes(ulongValue);
		bytes.SetBoolByIndex(boolIndex, value);
		return BitConverter.ToUInt64(bytes, 0);
	}

	public static T[] RemoveDouble<T>(this T[] value, int leftLength, int rightLength)
	{
		return SoftBasic.ArrayRemoveDouble(value, leftLength, rightLength);
	}

	public static T[] RemoveBegin<T>(this T[] value, int length)
	{
		return SoftBasic.ArrayRemoveBegin(value, length);
	}

	public static T[] RemoveLast<T>(this T[] value, int length)
	{
		return SoftBasic.ArrayRemoveLast(value, length);
	}

	public static T[] SelectMiddle<T>(this T[] value, int index, int length)
	{
		return SoftBasic.ArraySelectMiddle(value, index, length);
	}

	public static T[] SelectBegin<T>(this T[] value, int length)
	{
		return SoftBasic.ArraySelectBegin(value, length);
	}

	public static T[] SelectLast<T>(this T[] value, int length)
	{
		return SoftBasic.ArraySelectLast(value, length);
	}

	public static T GetValueOrDefault<T>(JObject jObject, string name, T defaultValue)
	{
		return SoftBasic.GetValueFromJsonObject(jObject, name, defaultValue);
	}

	public static T[] SpliceArray<T>(this T[] value, params T[][] arrays)
	{
		List<T[]> list = new List<T[]>(arrays.Length + 1);
		list.Add(value);
		list.AddRange(arrays);
		return SoftBasic.SpliceArray(list.ToArray());
	}

	/// <summary>
	/// 移除指定字符串数据的最后 length 个字符。如果字符串本身的长度不足 length，则返回为空字符串。
	/// Remove the last "length" characters of the specified string data. If the length of the string itself is less than length,
	/// an empty string is returned.
	/// </summary>
	/// <param name="value">等待操作的字符串数据</param>
	/// <param name="length">准备移除的长度信息</param>
	/// <returns>移除之后的数据信息</returns>
	public static string RemoveLast(this string value, int length)
	{
		if (value == null)
		{
			return null;
		}
		if (value.Length < length)
		{
			return string.Empty;
		}
		return value.Remove(value.Length - length);
	}

	/// <summary>
	/// 将指定的数据添加到数组的每个元素上去，会改变每个元素的值
	/// </summary>
	/// <param name="array">原始数组</param>
	/// <param name="value">值</param>
	/// <returns>修改后的数组信息</returns>
	public static byte[] EveryByteAdd(this byte[] array, int value)
	{
		if (array == null)
		{
			return null;
		}
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (byte)(array[i] + value);
		}
		return array;
	}

	public static T[] IncreaseBy<T>(this T[] array, T value)
	{
		if (typeof(T) == typeof(byte))
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(int), "first");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(int), "second");
			Expression body = Expression.Add(parameterExpression, parameterExpression2);
			Expression<Func<int, int, int>> expression = Expression.Lambda<Func<int, int, int>>(body, new ParameterExpression[2] { parameterExpression, parameterExpression2 });
			Func<int, int, int> func = expression.Compile();
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (T)(object)(byte)func(Convert.ToInt32(array[i]), Convert.ToInt32(value));
			}
		}
		else
		{
			ParameterExpression parameterExpression3 = Expression.Parameter(typeof(T), "first");
			ParameterExpression parameterExpression4 = Expression.Parameter(typeof(T), "second");
			Expression body2 = Expression.Add(parameterExpression3, parameterExpression4);
			Expression<Func<T, T, T>> expression2 = Expression.Lambda<Func<T, T, T>>(body2, new ParameterExpression[2] { parameterExpression3, parameterExpression4 });
			Func<T, T, T> func2 = expression2.Compile();
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = func2(array[j], value);
			}
		}
		return array;
	}

	public static T[] CopyArray<T>(this T[] value)
	{
		if (value == null)
		{
			return null;
		}
		T[] array = new T[value.Length];
		Array.Copy(value, array, value.Length);
		return array;
	}

	public static string ToArrayString<T>(this T[] value)
	{
		return SoftBasic.ArrayFormat(value);
	}

	public static string ToArrayString<T>(this T[] value, string format)
	{
		return SoftBasic.ArrayFormat(value, format);
	}

	public static T[] ToStringArray<T>(this string value, Func<string, T> selector)
	{
		if (value.IndexOf('[') >= 0)
		{
			value = value.Replace("[", "");
		}
		if (value.IndexOf(']') >= 0)
		{
			value = value.Replace("]", "");
		}
		string[] source = value.Split(new char[2] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
		return source.Select(selector).ToArray();
	}

	private static byte ParseStringToByte(string value)
	{
		value = value.Trim();
		if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
		{
			return Convert.ToByte(value.Substring(2), 16);
		}
		return byte.Parse(value);
	}

	private static short ParseStringToInt16(string value)
	{
		value = value.Trim();
		if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
		{
			return Convert.ToInt16(value.Substring(2), 16);
		}
		return short.Parse(value);
	}

	private static ushort ParseStringToUInt16(string value)
	{
		value = value.Trim();
		if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
		{
			return Convert.ToUInt16(value.Substring(2), 16);
		}
		return ushort.Parse(value);
	}

	private static int ParseStringToInt32(string value)
	{
		value = value.Trim();
		if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
		{
			return Convert.ToInt32(value.Substring(2), 16);
		}
		return int.Parse(value);
	}

	private static uint ParseStringToUInt32(string value)
	{
		value = value.Trim();
		if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
		{
			return Convert.ToUInt32(value.Substring(2), 16);
		}
		return uint.Parse(value);
	}

	private static long ParseStringToInt64(string value)
	{
		value = value.Trim();
		if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
		{
			return Convert.ToInt64(value.Substring(2), 16);
		}
		return long.Parse(value);
	}

	private static ulong ParseStringToUInt64(string value)
	{
		value = value.Trim();
		if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
		{
			return Convert.ToUInt64(value.Substring(2), 16);
		}
		return ulong.Parse(value);
	}

	public static T[] ToStringArray<T>(this string value)
	{
		Type typeFromHandle = typeof(T);
		if (typeFromHandle == typeof(byte))
		{
			return (T[])(object)value.ToStringArray(ParseStringToByte);
		}
		if (typeFromHandle == typeof(sbyte))
		{
			return (T[])(object)value.ToStringArray(sbyte.Parse);
		}
		if (typeFromHandle == typeof(bool))
		{
			return (T[])(object)value.ToStringArray(bool.Parse);
		}
		if (typeFromHandle == typeof(short))
		{
			return (T[])(object)value.ToStringArray(ParseStringToInt16);
		}
		if (typeFromHandle == typeof(ushort))
		{
			return (T[])(object)value.ToStringArray(ParseStringToUInt16);
		}
		if (typeFromHandle == typeof(int))
		{
			return (T[])(object)value.ToStringArray(ParseStringToInt32);
		}
		if (typeFromHandle == typeof(uint))
		{
			return (T[])(object)value.ToStringArray(ParseStringToUInt32);
		}
		if (typeFromHandle == typeof(long))
		{
			return (T[])(object)value.ToStringArray(ParseStringToInt64);
		}
		if (typeFromHandle == typeof(ulong))
		{
			return (T[])(object)value.ToStringArray(ParseStringToUInt64);
		}
		if (typeFromHandle == typeof(float))
		{
			return (T[])(object)value.ToStringArray(float.Parse);
		}
		if (typeFromHandle == typeof(double))
		{
			return (T[])(object)value.ToStringArray(double.Parse);
		}
		if (typeFromHandle == typeof(DateTime))
		{
			return (T[])(object)value.ToStringArray(DateTime.Parse);
		}
		if (typeFromHandle == typeof(Guid))
		{
			return (T[])(object)value.ToStringArray(Guid.Parse);
		}
		if (typeFromHandle == typeof(string))
		{
			return (T[])(object)value.ToStringArray((string m) => m);
		}
		throw new Exception("use ToArray<T>(Func<string,T>) method instead");
	}

	internal static bool EqualsNoCase(this string input, string value)
	{
		return input.Equals(value, StringComparison.OrdinalIgnoreCase);
	}

	internal static bool StartsWithNoCase(this string input, string value)
	{
		return input.StartsWith(value, StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// 启动接收数据，需要传入回调方法，传递对象
	/// To start receiving data, you need to pass in a callback method and pass an object
	/// </summary>
	/// <param name="socket">socket对象</param>
	/// <param name="callback">回调方法</param>
	/// <param name="obj">数据对象</param>
	/// <returns>是否启动成功</returns>
	public static OperateResult BeginReceiveResult(this Socket socket, AsyncCallback callback, object obj)
	{
		try
		{
			socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, callback, obj);
			return OperateResult.CreateSuccessResult();
		}
		catch (Exception ex)
		{
			socket?.Close();
			return new OperateResult(ex.Message);
		}
	}

	/// <summary>
	/// 启动接收数据，需要传入回调方法，传递对象默认为socket本身
	/// To start receiving data, you need to pass in a callback method. The default object is the socket itself.
	/// </summary>
	/// <param name="socket">socket对象</param>
	/// <param name="callback">回调方法</param>
	/// <returns>是否启动成功</returns>
	public static OperateResult BeginReceiveResult(this Socket socket, AsyncCallback callback)
	{
		return socket.BeginReceiveResult(callback, socket);
	}

	/// <summary>
	/// 结束挂起的异步读取，返回读取的字节数，如果成功的情况。
	/// Ends the pending asynchronous read and returns the number of bytes read, if successful.
	/// </summary>
	/// <param name="socket">socket对象</param>
	/// <param name="ar">回调方法</param>
	/// <returns>是否启动成功</returns>
	public static OperateResult<int> EndReceiveResult(this Socket socket, IAsyncResult ar)
	{
		try
		{
			return OperateResult.CreateSuccessResult(socket.EndReceive(ar));
		}
		catch (Exception ex)
		{
			socket?.Close();
			return new OperateResult<int>(ex.Message);
		}
	}

	/// <summary>
	/// 根据英文小数点进行切割字符串，去除空白的字符
	/// Cut the string according to the English decimal point and remove the blank characters
	/// </summary>
	/// <param name="str">字符串本身</param>
	/// <returns>切割好的字符串数组，例如输入 &quot;100.5&quot;，返回 &quot;100&quot;, &quot;5&quot;</returns>
	public static string[] SplitDot(this string str)
	{
		return str.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
	}

	/// <summary>
	/// 获取当前对象的JSON格式表示的字符串。
	/// Gets the string represented by the JSON format of the current object.
	/// </summary>
	/// <returns>字符串对象</returns>
	public static string ToJsonString(this object obj, Formatting formatting = (Formatting)1)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return JsonConvert.SerializeObject(obj, formatting);
	}

	/// <summary>
	/// 从RSA的算法对象里，获取到PEM格式的原始私钥数据，如果需要存储，或是显示，只需要 Convert.ToBase64String 方法
	/// Obtain the original private key data in PEM format from the RSA algorithm object. If you need to store or display it,
	/// you only need the Convert.ToBase64String method
	/// </summary>
	/// <param name="rsa">RSA 算法的加密解密对象</param>
	/// <returns>原始的私钥数据</returns>
	public static byte[] GetPEMPrivateKey(this RSACryptoServiceProvider rsa)
	{
		return RSAHelper.GetPrivateKeyFromRSA(rsa);
	}

	/// <summary>
	/// 从RSA的算法对象里，获取到PEM格式的原始公钥数据，如果需要存储，或是显示，只需要 Convert.ToBase64String 方法
	/// Obtain the original public key data in PEM format from the RSA algorithm object. If you need to store or display it,
	/// you only need the Convert.ToBase64String method
	/// </summary>
	/// <param name="rsa">RSA 算法的加密解密对象</param>
	/// <returns>原始的公钥数据</returns>
	public static byte[] GetPEMPublicKey(this RSACryptoServiceProvider rsa)
	{
		return RSAHelper.GetPublicKeyFromRSA(rsa);
	}

	/// <summary>
	/// 对原始字节的数据进行加密，不限制长度，因为RSA本身限制了117字节，所以此处进行数据切割加密。
	/// Encrypt the original byte data without limiting the length, because RSA itself limits 117 bytes, so the data is cut and encrypted here.
	/// </summary>
	/// <param name="provider">RSA公钥对象</param>
	/// <param name="data">等待加密的原始数据</param>
	/// <returns>加密之后的结果信息</returns>
	public static byte[] EncryptLargeData(this RSACryptoServiceProvider rsa, byte[] data)
	{
		return RSAHelper.EncryptLargeDataByRSA(rsa, data);
	}

	/// <summary>
	/// 对超过117字节限制的加密数据进行解密，解密出原始的字节数据，因为RSA本身限制了117字节，所以此处进行数据切割解密。
	/// Decrypt the encrypted data that exceeds the 117-byte limit and decrypt the original byte data, because RSA itself limits 117 bytes, so the data is cut and decrypted here.
	/// </summary>
	/// <param name="provider">RSA私钥对象</param>
	/// <param name="data">等待解密的数据</param>
	/// <returns>解密之后的结果数据</returns>
	public static byte[] DecryptLargeData(this RSACryptoServiceProvider rsa, byte[] data)
	{
		return RSAHelper.DecryptLargeDataByRSA(rsa, data);
	}

	public static void Write(this MemoryStream ms, byte[] buffer)
	{
		if (buffer != null)
		{
			ms.Write(buffer, 0, buffer.Length);
		}
	}

	/// <summary>
	/// 将数据写入到字节流，字节顺序为相反
	/// Write  data to the byte stream, the byte order is reversed
	/// </summary>
	/// <param name="ms">字节流</param>
	/// <param name="value">等待写入的值</param>
	public static void WriteReverse(this MemoryStream ms, ushort value)
	{
		byte[] bytes = BitConverter.GetBytes(value);
		byte b = bytes[0];
		bytes[0] = bytes[1];
		bytes[1] = b;
		ms.Write(bytes);
	}

	/// <summary>
	/// 设置套接字的活动时间和活动间歇时间，此值会设置到socket低级别的控制中，传入值如果为负数，则表示不使用 KeepAlive 功能。
	/// Set the active time and active intermittent time of the socket. This value will be set to the low-level control of the socket.
	/// If the incoming value is a negative number, it means that the KeepAlive function is not used.
	/// </summary>
	/// <param name="socket">套接字对象</param>
	/// <param name="keepAliveTime">保持活动时间</param>
	/// <param name="keepAliveInterval">保持活动的间歇时间</param>
	/// <returns>返回获取的参数的字节</returns>
	public static int SetKeepAlive(this Socket socket, int keepAliveTime, int keepAliveInterval)
	{
		byte[] buffer = new byte[12];
		BitConverter.GetBytes((keepAliveTime >= 0) ? 1 : 0).CopyTo(buffer, 0);
		BitConverter.GetBytes(keepAliveTime).CopyTo(buffer, 4);
		BitConverter.GetBytes(keepAliveInterval).CopyTo(buffer, 8);
		try
		{
			return socket.IOControl(IOControlCode.KeepAliveValues, buffer, null);
		}
		catch
		{
			return 0;
		}
	}

	/// <summary>
	/// 使用格式化的串口参数信息来初始化串口的参数，举例：9600-8-N-1，分别表示波特率，数据位，奇偶校验，停止位，当然也可以携带串口名称，例如：COM3-9600-8-N-1，linux环境也是支持的。
	/// Use the formatted serial port parameter information to initialize the serial port parameters, for example: 9600-8-N-1, which means baud rate, data bit, parity,
	/// stop bit, of course, can also carry the serial port name, for example: COM3- 9600-8-N-1, linux environment is also supported.
	/// </summary>
	/// <param name="serialPort">串口对象信息</param>
	/// <param name="format">格式化的参数内容，例如：9600-8-N-1</param>
	/// <remarks>
	/// 其中奇偶校验的字母可选，N:无校验，O：奇校验，E:偶校验，停止位可选 0, 1, 2, 1.5 四种选项
	/// Among them, the letters of the parity check are optional, N: no check, O: odd check, E: even check, stop bit optional 0, 1, 2, 1.5 four options
	/// </remarks>
	public static void IniSerialByFormatString(this SerialPort serialPort, string format)
	{
		string[] array = format.Split(new char[2] { '-', ';' }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length != 0)
		{
			int num = 0;
			if (!Regex.IsMatch(array[0], "^[0-9]+$"))
			{
				serialPort.PortName = array[0];
				num = 1;
			}
			if (num < array.Length)
			{
				serialPort.BaudRate = Convert.ToInt32(array[num++]);
			}
			if (num < array.Length)
			{
				serialPort.DataBits = Convert.ToInt32(array[num++]);
			}
			if (num < array.Length)
			{
				serialPort.Parity = (Parity)(array[num++].ToUpper() switch
				{
					"E" => 2, 
					"O" => 1, 
					"N" => 0, 
					_ => 4, 
				});
			}
			if (num < array.Length)
			{
				serialPort.StopBits = (StopBits)(array[num++] switch
				{
					"0" => 0, 
					"2" => 2, 
					"1" => 1, 
					_ => 3, 
				});
			}
		}
	}

	/// <summary>
	/// 将一个串口对象的基本配置参数进行格式化字符串，例如 COM3-9600-8-N-1
	/// Format the basic configuration parameters of a serial port object as a string, for example, COM3-9600-8-N-1
	/// </summary>
	/// <param name="serialPort">串口对象信息</param>
	/// <returns>串口对的格式化字符串信息</returns>
	/// <remarks>
	/// 其中奇偶校验的字母可选，N:无校验，O：奇校验，E:偶校验，停止位可选 0, 1, 2, 1.5 四种选项
	/// Among them, the letters of the parity check are optional, N: no check, O: odd check, E: even check, stop bit optional 0, 1, 2, 1.5 four options
	/// </remarks>
	public static string ToFormatString(this SerialPort serialPort)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return HslHelper.ToFormatString(serialPort.PortName, serialPort.BaudRate, serialPort.DataBits, serialPort.Parity, serialPort.StopBits);
	}

	/// <summary>
	/// 根据指定的字节长度信息，获取到随机的字节信息
	/// Obtain random byte information according to the specified byte length information
	/// </summary>
	/// <param name="random">随机数对象</param>
	/// <param name="length">字节的长度信息</param>
	/// <returns>原始字节数组</returns>
	public static byte[] GetBytes(this Random random, int length)
	{
		byte[] buffer = new byte[length];
		random.NextBytes(buffer);
		return buffer;
	}

	/// <summary>
	/// 将byte数组按照双字节进行反转，如果为单数的情况，则自动补齐
	/// Reverses the byte array by double byte, or if the singular is the case, automatically
	/// </summary>
	/// <param name="inBytes">输入的字节信息</param>
	/// <returns>反转后的数据</returns>
	/// <remarks>
	/// 例如传入的字节数据是 01 02 03 04, 那么反转后就是 02 01 04 03
	/// </remarks>
	public static byte[] ReverseByWord(this byte[] inBytes)
	{
		return SoftBasic.BytesReverseByWord(inBytes);
	}

	/// <summary>
	/// 判断一个地址是否是指定的字符串开头，并且后面跟着数字，如果是，返回 true，反之，返回 false
	/// Determines whether an address starts with a specified string followed by a number, returns true if so, and false if not
	/// </summary>
	/// <param name="address">等待判断的地址</param>
	/// <param name="code">地址类型</param>
	/// <returns>是否指定的地址起始</returns>
	public static bool StartsWithAndNumber(this string address, string code)
	{
		if (address.StartsWith(code, StringComparison.OrdinalIgnoreCase))
		{
			if (address.Length <= code.Length)
			{
				return false;
			}
			if (char.IsNumber(address[code.Length]))
			{
				return true;
			}
			if (address.Length > code.Length + 1 && address[code.Length] == '-' && char.IsNumber(address[code.Length + 1]))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 判断一个地址是否是指定的字符串开头，并且后面跟着数字，如果是，返回 true，反之，返回 false
	/// Determines whether an address starts with a specified string followed by a number, returns true if so, and false if not
	/// </summary>
	/// <param name="address">等待判断的地址</param>
	/// <param name="code">地址类型</param>
	/// <returns>是否指定的地址起始</returns>
	public static bool StartsWithAndNumber(this string address, string[] code)
	{
		if (code == null)
		{
			return false;
		}
		for (int i = 0; i < code.Length; i++)
		{
			if (address.StartsWithAndNumber(code[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static bool StartsWith(this string address, string[] code)
	{
		if (code == null)
		{
			return false;
		}
		for (int i = 0; i < code.Length; i++)
		{
			if (address.StartsWith(code[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static bool EndsWith(this string str, string[] value)
	{
		if (value == null)
		{
			return false;
		}
		for (int i = 0; i < value.Length; i++)
		{
			if (str.EndsWith(value[i], StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public static bool Contains(this string str, string[] value)
	{
		if (value == null)
		{
			return false;
		}
		for (int i = 0; i < value.Length; i++)
		{
			if (str.Contains(value[i]))
			{
				return true;
			}
		}
		return false;
	}
}
