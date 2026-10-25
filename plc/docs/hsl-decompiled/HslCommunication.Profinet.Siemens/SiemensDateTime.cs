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
using HslCommunication.Core;

namespace HslCommunication.Profinet.Siemens;

/// <summary>
/// Contains the methods to convert between  and S7 representation of datetime values.
/// </summary>
/// <remarks>
/// 这部分的代码参考了另一个s7的库，感谢原作者，此处贴出出处，遵循 MIT 协议
/// https://github.com/S7NetPlus/s7netplus
/// </remarks>
public class SiemensDateTime
{
	/// <summary>
	/// The minimum  value supported by the specification.
	/// </summary>
	public static readonly DateTime SpecMinimumDateTime = new DateTime(1990, 1, 1);

	/// <summary>
	/// The maximum  value supported by the specification.
	/// </summary>
	public static readonly DateTime SpecMaximumDateTime = new DateTime(2089, 12, 31, 23, 59, 59, 999);

	/// <summary>
	/// Parses a  value from bytes.
	/// </summary>
	/// <param name="bytes">Input bytes read from PLC.</param>
	/// <returns>A  object representing the value read from PLC.</returns>
	public static OperateResult<DateTime> FromByteArray(byte[] bytes)
	{
		try
		{
			return OperateResult.CreateSuccessResult(FromByteArrayImpl(bytes));
		}
		catch (Exception ex)
		{
			return new OperateResult<DateTime>("Prase DateTime failed: " + ex.Message);
		}
	}

	/// <summary>
	/// Parses a  value from bytes.
	/// </summary>
	/// <param name="bytes">Input bytes read from PLC.</param>
	/// <returns>A  object representing the value read from PLC.</returns>
	public static OperateResult<DateTime> FromByteArray(OperateResult<byte[]> read)
	{
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<DateTime>(read);
		}
		return FromByteArray(read.Content);
	}

	/// <summary>
	/// 从西门子的原始字节数据中，提取出DTL格式的时间信息
	/// </summary>
	/// <param name="byteTransform">西门子的字节变换对象</param>
	/// <param name="buffer">原始字节数据</param>
	/// <param name="index">字节偏移索引</param>
	/// <returns>时间信息</returns>
	public static OperateResult<DateTime> GetDTLTime(IByteTransform byteTransform, byte[] buffer, int index)
	{
		try
		{
			int year = byteTransform.TransInt16(buffer, index);
			int month = buffer[index + 2];
			int day = buffer[index + 3];
			int hour = buffer[index + 5];
			int minute = buffer[index + 6];
			int second = buffer[index + 7];
			int millisecond = byteTransform.TransInt32(buffer, index + 8) / 1000 / 1000;
			return OperateResult.CreateSuccessResult(new DateTime(year, month, day, hour, minute, second, millisecond));
		}
		catch (Exception ex)
		{
			return new OperateResult<DateTime>("GetDTLTime failed: " + ex.Message);
		}
	}

	/// <summary>
	/// 从西门子的原始字节数据中，提取出DTL格式的时间信息
	/// </summary>
	/// <param name="byteTransform">西门子的字节变换对象</param>
	/// <param name="buffer">原始字节数据</param>
	/// <param name="index">字节偏移索引</param>
	/// <returns>时间信息</returns>
	public static OperateResult<DateTime> GetDTLTime(IByteTransform byteTransform, OperateResult<byte[]> read)
	{
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<DateTime>(read);
		}
		return GetDTLTime(byteTransform, read.Content, 0);
	}

	/// <summary>
	/// 将时间数据转换为西门子的DTL格式的时间数据
	/// </summary>
	/// <param name="byteTransform">西门子的字节变换对象</param>
	/// <param name="dateTime">指定的时间信息</param>
	/// <returns>原始字节数据信息</returns>
	public static byte[] GetBytesFromDTLTime(IByteTransform byteTransform, DateTime dateTime)
	{
		byte[] buffer = new byte[12];
		byteTransform.TransByte((short)dateTime.Year).CopyTo(buffer, 0);
		buffer[2] = (byte)dateTime.Month;
		buffer[3] = (byte)dateTime.Day;
		buffer[4] = 5;
		buffer[5] = (byte)dateTime.Hour;
		buffer[6] = (byte)dateTime.Minute;
		buffer[7] = (byte)dateTime.Second;
		byteTransform.TransByte(dateTime.Millisecond * 1000 * 1000).CopyTo(buffer, 8);
		return buffer;
	}

	/// <summary>
	/// Parses an array of  values from bytes.
	/// </summary>
	/// <param name="bytes">Input bytes read from PLC.</param>
	/// <returns>An array of  objects representing the values read from PLC.</returns>
	public static DateTime[] ToArray(byte[] bytes)
	{
		if (bytes.Length % 8 != 0)
		{
			throw new ArgumentOutOfRangeException("bytes", bytes.Length, $"Parsing an array of DateTime requires a multiple of 8 bytes of input data, input data is '{bytes.Length}' long.");
		}
		int count = bytes.Length / 8;
		DateTime[] array = new DateTime[bytes.Length / 8];
		for (int i = 0; i < count; i++)
		{
			array[i] = FromByteArrayImpl(new ArraySegment<byte>(bytes, i * 8, 8).Array);
		}
		return array;
	}

	private static DateTime FromByteArrayImpl(IList<byte> bytes)
	{
		if (bytes.Count != 8)
		{
			throw new ArgumentOutOfRangeException("bytes", bytes.Count, $"Parsing a DateTime requires exactly 8 bytes of input data, input data is {bytes.Count} bytes long.");
		}
		int year = ByteToYear(bytes[0]);
		int month = AssertRangeInclusive(DecodeBcd(bytes[1]), 1, 12, "month");
		int day = AssertRangeInclusive(DecodeBcd(bytes[2]), 1, 31, "day of month");
		int hour = AssertRangeInclusive(DecodeBcd(bytes[3]), 0, 23, "hour");
		int minute = AssertRangeInclusive(DecodeBcd(bytes[4]), 0, 59, "minute");
		int second = AssertRangeInclusive(DecodeBcd(bytes[5]), 0, 59, "second");
		int num = AssertRangeInclusive(DecodeBcd(bytes[6]), 0, 99, "first two millisecond digits");
		int num2 = AssertRangeInclusive(bytes[7] >> 4, 0, 9, "third millisecond digit");
		int num3 = AssertRangeInclusive(bytes[7] & 0xF, 1, 7, "day of week");
		return new DateTime(year, month, day, hour, minute, second, num * 10 + num2);
		static int AssertRangeInclusive(int input, byte min, byte max, string field)
		{
			if (input < min)
			{
				throw new ArgumentOutOfRangeException("input", input, $"Value '{input}' is lower than the minimum '{min}' allowed for {field}.");
			}
			if (input > max)
			{
				throw new ArgumentOutOfRangeException("input", input, $"Value '{input}' is higher than the maximum '{max}' allowed for {field}.");
			}
			return input;
		}
		static int ByteToYear(byte bcdYear)
		{
			int num4 = DecodeBcd(bcdYear);
			if (num4 < 90)
			{
				return num4 + 2000;
			}
			if (num4 >= 100)
			{
				throw new ArgumentOutOfRangeException("bcdYear", bcdYear, $"Value '{num4}' is higher than the maximum '99' of S7 date and time representation.");
			}
			return num4 + 1900;
		}
		static int DecodeBcd(byte input)
		{
			return 10 * (input >> 4) + (input & 0xF);
		}
	}

	/// <summary>
	/// Converts a  value to a byte array.
	/// </summary>
	/// <param name="dateTime">The DateTime value to convert.</param>
	/// <returns>A byte array containing the S7 date time representation of .</returns>
	public static byte[] ToByteArray(DateTime dateTime)
	{
		if (dateTime < SpecMinimumDateTime)
		{
			throw new ArgumentOutOfRangeException("dateTime", dateTime, $"Date time '{dateTime}' is before the minimum '{SpecMinimumDateTime}' supported in S7 date time representation.");
		}
		if (dateTime > SpecMaximumDateTime)
		{
			throw new ArgumentOutOfRangeException("dateTime", dateTime, $"Date time '{dateTime}' is after the maximum '{SpecMaximumDateTime}' supported in S7 date time representation.");
		}
		return new byte[8]
		{
			EncodeBcd(MapYear(dateTime.Year)),
			EncodeBcd(dateTime.Month),
			EncodeBcd(dateTime.Day),
			EncodeBcd(dateTime.Hour),
			EncodeBcd(dateTime.Minute),
			EncodeBcd(dateTime.Second),
			EncodeBcd(dateTime.Millisecond / 10),
			(byte)((dateTime.Millisecond % 10 << 4) | DayOfWeekToInt(dateTime.DayOfWeek))
		};
		static int DayOfWeekToInt(DayOfWeek dayOfWeek)
		{
			return (int)(dayOfWeek + 1);
		}
		static byte EncodeBcd(int value)
		{
			return (byte)((value / 10 << 4) | (value % 10));
		}
		static byte MapYear(int year)
		{
			return (byte)((year < 2000) ? (year - 1900) : (year - 2000));
		}
	}

	/// <summary>
	/// Converts an array of  values to a byte array.
	/// </summary>
	/// <param name="dateTimes">The DateTime values to convert.</param>
	/// <returns>A byte array containing the S7 date time representations of .</returns>
	public static byte[] ToByteArray(DateTime[] dateTimes)
	{
		List<byte> list = new List<byte>(dateTimes.Length * 8);
		foreach (DateTime dateTime in dateTimes)
		{
			list.AddRange(ToByteArray(dateTime));
		}
		return list.ToArray();
	}
}
