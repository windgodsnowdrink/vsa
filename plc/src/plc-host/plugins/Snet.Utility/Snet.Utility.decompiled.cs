#define TRACE
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Win32;
using Newtonsoft.Json;
using ProtoBuf;
using Snet.Utility.comparer;
using Snet.Utility.comparer.IgnoreOrderTypes;
using Snet.Utility.comparer.TypeComparers;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETCoreApp,Version=v8.0", FrameworkDisplayName = ".NET 8.0")]
[assembly: AssemblyCompany("Shun")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyDescription("核心组件：Utility（字节、枚举、文件、字符串、验证、比对、转换、反射、Json、Xml、Protobuf、Ftp、System ...）")]
[assembly: AssemblyFileVersion("26.235.1.0")]
[assembly: AssemblyInformationalVersion("26.235.1+6bec27efc0f2c65dc751ab34757e37960f2b43fe")]
[assembly: AssemblyProduct("Core")]
[assembly: AssemblyTitle("Snet.Utility")]
[assembly: AssemblyVersion("26.235.1.0")]
[module: RefSafetyRules(11)]
namespace Snet.Utility
{
	/// <summary>
	/// 字节处理
	/// </summary>
	public static class ByteHandler
	{
		private static readonly ushort[] CrcTable = new ushort[256]
		{
			0, 49345, 49537, 320, 49921, 960, 640, 49729, 50689, 1728,
			1920, 51009, 1280, 50625, 50305, 1088, 52225, 3264, 3456, 52545,
			3840, 53185, 52865, 3648, 2560, 51905, 52097, 2880, 51457, 2496,
			2176, 51265, 55297, 6336, 6528, 55617, 6912, 56257, 55937, 6720,
			7680, 57025, 57217, 8000, 56577, 7616, 7296, 56385, 5120, 54465,
			54657, 5440, 55041, 6080, 5760, 54849, 53761, 4800, 4992, 54081,
			4352, 53697, 53377, 4160, 61441, 12480, 12672, 61761, 13056, 62401,
			62081, 12864, 13824, 63169, 63361, 14144, 62721, 13760, 13440, 62529,
			15360, 64705, 64897, 15680, 65281, 16320, 16000, 65089, 64001, 15040,
			15232, 64321, 14592, 63937, 63617, 14400, 10240, 59585, 59777, 10560,
			60161, 11200, 10880, 59969, 60929, 11968, 12160, 61249, 11520, 60865,
			60545, 11328, 58369, 9408, 9600, 58689, 9984, 59329, 59009, 9792,
			8704, 58049, 58241, 9024, 57601, 8640, 8320, 57409, 40961, 24768,
			24960, 41281, 25344, 41921, 41601, 25152, 26112, 42689, 42881, 26432,
			42241, 26048, 25728, 42049, 27648, 44225, 44417, 27968, 44801, 28608,
			28288, 44609, 43521, 27328, 27520, 43841, 26880, 43457, 43137, 26688,
			30720, 47297, 47489, 31040, 47873, 31680, 31360, 47681, 48641, 32448,
			32640, 48961, 32000, 48577, 48257, 31808, 46081, 29888, 30080, 46401,
			30464, 47041, 46721, 30272, 29184, 45761, 45953, 29504, 45313, 29120,
			28800, 45121, 20480, 37057, 37249, 20800, 37633, 21440, 21120, 37441,
			38401, 22208, 22400, 38721, 21760, 38337, 38017, 21568, 39937, 23744,
			23936, 40257, 24320, 40897, 40577, 24128, 23040, 39617, 39809, 23360,
			39169, 22976, 22656, 38977, 34817, 18624, 18816, 35137, 19200, 35777,
			35457, 19008, 19968, 36545, 36737, 20288, 36097, 19904, 19584, 35905,
			17408, 33985, 34177, 17728, 34561, 18368, 18048, 34369, 33281, 17088,
			17280, 33601, 16640, 33217, 32897, 16448
		};

		/// <summary>
		/// CRC 查找表（只读，禁止外部修改）
		/// </summary>
		private static readonly int[] CRC_Key = new int[256]
		{
			0, 4129, 8258, 12387, 16516, 20645, 24774, 28903, 33032, 37161,
			41290, 45419, 49548, 53677, 57806, 61935, 4657, 528, 12915, 8786,
			21173, 17044, 29431, 25302, 37689, 33560, 45947, 41818, 54205, 50076,
			62463, 58334, 9314, 13379, 1056, 5121, 25830, 29895, 17572, 21637,
			42346, 46411, 34088, 38153, 58862, 62927, 50604, 54669, 13907, 9842,
			5649, 1584, 30423, 26358, 22165, 18100, 46939, 42874, 38681, 34616,
			63455, 59390, 55197, 51132, 18628, 22757, 26758, 30887, 2112, 6241,
			10242, 14371, 51660, 55789, 59790, 63919, 35144, 39273, 43274, 47403,
			23285, 19156, 31415, 27286, 6769, 2640, 14899, 10770, 56317, 52188,
			64447, 60318, 39801, 35672, 47931, 43802, 27814, 31879, 19684, 23749,
			11298, 15363, 3168, 7233, 60846, 64911, 52716, 56781, 44330, 48395,
			36200, 40265, 32407, 28342, 24277, 20212, 15891, 11826, 7761, 3696,
			65439, 61374, 57309, 53244, 48923, 44858, 40793, 36728, 37256, 33193,
			45514, 41451, 53516, 49453, 61774, 57711, 4224, 161, 12482, 8419,
			20484, 16421, 28742, 24679, 33721, 37784, 41979, 46042, 49981, 54044,
			58239, 62302, 689, 4752, 8947, 13010, 16949, 21012, 25207, 29270,
			46570, 42443, 38312, 34185, 62830, 58703, 54572, 50445, 13538, 9411,
			5280, 1153, 29798, 25671, 21540, 17413, 42971, 47098, 34713, 38840,
			59231, 63358, 50973, 55100, 9939, 14066, 1681, 5808, 26199, 30326,
			17941, 22068, 55628, 51565, 63758, 59695, 39368, 35305, 47498, 43435,
			22596, 18533, 30726, 26663, 6336, 2273, 14466, 10403, 52093, 56156,
			60223, 64286, 35833, 39896, 43963, 48026, 19061, 23124, 27191, 31254,
			2801, 6864, 10931, 14994, 64814, 60687, 56684, 52557, 48554, 44427,
			40424, 36297, 31782, 27655, 23652, 19525, 15522, 11395, 7392, 3265,
			61215, 65342, 53085, 57212, 44955, 49082, 36825, 40952, 28183, 32310,
			20053, 24180, 11923, 16050, 3793, 7920
		};

		/// <summary>
		/// CRC高位校验码表
		/// </summary>
		private static readonly byte[] cRCHighArray = new byte[256]
		{
			0, 193, 129, 64, 1, 192, 128, 65, 1, 192,
			128, 65, 0, 193, 129, 64, 1, 192, 128, 65,
			0, 193, 129, 64, 0, 193, 129, 64, 1, 192,
			128, 65, 1, 192, 128, 65, 0, 193, 129, 64,
			0, 193, 129, 64, 1, 192, 128, 65, 0, 193,
			129, 64, 1, 192, 128, 65, 1, 192, 128, 65,
			0, 193, 129, 64, 1, 192, 128, 65, 0, 193,
			129, 64, 0, 193, 129, 64, 1, 192, 128, 65,
			0, 193, 129, 64, 1, 192, 128, 65, 1, 192,
			128, 65, 0, 193, 129, 64, 0, 193, 129, 64,
			1, 192, 128, 65, 1, 192, 128, 65, 0, 193,
			129, 64, 1, 192, 128, 65, 0, 193, 129, 64,
			0, 193, 129, 64, 1, 192, 128, 65, 1, 192,
			128, 65, 0, 193, 129, 64, 0, 193, 129, 64,
			1, 192, 128, 65, 0, 193, 129, 64, 1, 192,
			128, 65, 1, 192, 128, 65, 0, 193, 129, 64,
			0, 193, 129, 64, 1, 192, 128, 65, 1, 192,
			128, 65, 0, 193, 129, 64, 1, 192, 128, 65,
			0, 193, 129, 64, 0, 193, 129, 64, 1, 192,
			128, 65, 0, 193, 129, 64, 1, 192, 128, 65,
			1, 192, 128, 65, 0, 193, 129, 64, 1, 192,
			128, 65, 0, 193, 129, 64, 0, 193, 129, 64,
			1, 192, 128, 65, 1, 192, 128, 65, 0, 193,
			129, 64, 0, 193, 129, 64, 1, 192, 128, 65,
			0, 193, 129, 64, 1, 192, 128, 65, 1, 192,
			128, 65, 0, 193, 129, 64
		};

		/// <summary>
		/// CRC低位校验码表
		/// </summary>
		private static readonly byte[] cRCLowArray = new byte[256]
		{
			0, 192, 193, 1, 195, 3, 2, 194, 198, 6,
			7, 199, 5, 197, 196, 4, 204, 12, 13, 205,
			15, 207, 206, 14, 10, 202, 203, 11, 201, 9,
			8, 200, 216, 24, 25, 217, 27, 219, 218, 26,
			30, 222, 223, 31, 221, 29, 28, 220, 20, 212,
			213, 21, 215, 23, 22, 214, 210, 18, 19, 211,
			17, 209, 208, 16, 240, 48, 49, 241, 51, 243,
			242, 50, 54, 246, 247, 55, 245, 53, 52, 244,
			60, 252, 253, 61, 255, 63, 62, 254, 250, 58,
			59, 251, 57, 249, 248, 56, 40, 232, 233, 41,
			235, 43, 42, 234, 238, 46, 47, 239, 45, 237,
			236, 44, 228, 36, 37, 229, 39, 231, 230, 38,
			34, 226, 227, 35, 225, 33, 32, 224, 160, 96,
			97, 161, 99, 163, 162, 98, 102, 166, 167, 103,
			165, 101, 100, 164, 108, 172, 173, 109, 175, 111,
			110, 174, 170, 106, 107, 171, 105, 169, 168, 104,
			120, 184, 185, 121, 187, 123, 122, 186, 190, 126,
			127, 191, 125, 189, 188, 124, 180, 116, 117, 181,
			119, 183, 182, 118, 114, 178, 179, 115, 177, 113,
			112, 176, 80, 144, 145, 81, 147, 83, 82, 146,
			150, 86, 87, 151, 85, 149, 148, 84, 156, 92,
			93, 157, 95, 159, 158, 94, 90, 154, 155, 91,
			153, 89, 88, 152, 136, 72, 73, 137, 75, 139,
			138, 74, 78, 142, 143, 79, 141, 77, 76, 140,
			68, 132, 133, 69, 135, 71, 70, 134, 130, 66,
			67, 131, 65, 129, 128, 64
		};

		/// <summary>
		///     将四个 UInt16 值转换为 IEEE 64 位浮点格式；<br />
		///     使用 Span 避免 LINQ Concat 多次数组分配
		/// </summary>
		/// <param name="b3">最高位 ushort 值</param>
		/// <param name="b2">次高位 ushort 值</param>
		/// <param name="b1">次低位 ushort 值</param>
		/// <param name="b0">最低位 ushort 值</param>
		/// <returns>IEEE 64 位浮点数值</returns>
		public static double GetDouble(ushort b3, ushort b2, ushort b1, ushort b0)
		{
			Span<byte> span = stackalloc byte[8];
			BitConverter.TryWriteBytes(span, b0);
			BitConverter.TryWriteBytes(span.Slice(2), b1);
			BitConverter.TryWriteBytes(span.Slice(4), b2);
			BitConverter.TryWriteBytes(span.Slice(6), b3);
			return BitConverter.ToDouble(span);
		}

		/// <summary>
		///     将两个 UInt16 值转换为 IEEE 32 位浮点格式；<br />
		///     使用 Span 避免 LINQ Concat 多次数组分配
		/// </summary>
		/// <param name="highOrderValue">高位 ushort 值</param>
		/// <param name="lowOrderValue">低位 ushort 值</param>
		/// <returns>IEEE 32 位浮点数值</returns>
		public static float GetSingle(ushort highOrderValue, ushort lowOrderValue)
		{
			Span<byte> span = stackalloc byte[4];
			BitConverter.TryWriteBytes(span, lowOrderValue);
			BitConverter.TryWriteBytes(span.Slice(2), highOrderValue);
			return BitConverter.ToSingle(span);
		}

		/// <summary>
		///     将两个 UInt16 值转换为 UInt32；<br />
		///     使用 Span 避免 LINQ Concat 多次数组分配
		/// </summary>
		public static uint GetUInt32(ushort highOrderValue, ushort lowOrderValue)
		{
			Span<byte> span = stackalloc byte[4];
			BitConverter.TryWriteBytes(span, lowOrderValue);
			BitConverter.TryWriteBytes(span.Slice(2), highOrderValue);
			return BitConverter.ToUInt32(span);
		}

		/// <summary>
		///     Converts an array of bytes to an ASCII byte array.
		/// </summary>
		/// <param name="numbers">The byte array.</param>
		/// <returns>An array of ASCII byte values.</returns>
		public static byte[] GetAsciiBytes(params byte[] numbers)
		{
			return Encoding.UTF8.GetBytes(numbers.SelectMany((byte n) => n.ToString("X2")).ToArray());
		}

		/// <summary>
		///     Converts an array of UInt16 to an ASCII byte array.
		/// </summary>
		/// <param name="numbers">The ushort array.</param>
		/// <returns>An array of ASCII byte values.</returns>
		public static byte[] GetAsciiBytes(params ushort[] numbers)
		{
			return Encoding.UTF8.GetBytes(numbers.SelectMany((ushort n) => n.ToString("X4")).ToArray());
		}

		/// <summary>
		///     Converts a network order byte array to an array of UInt16 values in host order.
		/// </summary>
		/// <param name="networkBytes">The network order byte array.</param>
		/// <returns>The host order ushort array.</returns>
		public static ushort[] NetworkBytesToHostUInt16(byte[] networkBytes)
		{
			if (networkBytes == null)
			{
				throw new ArgumentNullException("networkBytes");
			}
			if (networkBytes.Length % 2 != 0)
			{
				throw new FormatException("Array networkBytes must contain an even number of bytes");
			}
			ushort[] array = new ushort[networkBytes.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(networkBytes, i * 2));
			}
			return array;
		}

		/// <summary>
		///     Converts a hex string to a byte array.
		/// </summary>
		/// <param name="hex">The hex string.</param>
		/// <returns>Array of bytes.</returns>
		public static byte[] HexToBytes(string hex)
		{
			if (hex == null)
			{
				throw new ArgumentNullException("hex");
			}
			if (hex.Length % 2 != 0)
			{
				throw new FormatException("Hex string must have even number of characters.");
			}
			byte[] array = new byte[hex.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
			}
			return array;
		}

		/// <summary>
		///     Calculate Longitudinal Redundancy Check.
		/// </summary>
		/// <param name="data">The data used in LRC.</param>
		/// <returns>LRC value.</returns>
		public static byte CalculateLrc(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			byte b = 0;
			foreach (byte b2 in data)
			{
				b += b2;
			}
			return (byte)((b ^ 0xFF) + 1);
		}

		/// <summary>
		///     Calculate Cyclical Redundancy Check.
		/// </summary>
		/// <param name="data">The data used in CRC.</param>
		/// <returns>CRC value.</returns>
		public static byte[] CalculateCrc(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			ushort num = ushort.MaxValue;
			foreach (byte b in data)
			{
				byte b2 = (byte)(num ^ b);
				num >>= 8;
				num ^= CrcTable[b2];
			}
			return BitConverter.GetBytes(num);
		}

		/// <summary>
		/// 字节数据拼接成一个完整的包；<br />
		/// 将两个字节数组的指定范围拼接在一起返回新的字节数组
		/// </summary>
		/// <param name="firstBytes">累加数据缓存</param>
		/// <param name="firstIndex">累加数据缓存起始下标</param>
		/// <param name="firstLength">累加数据缓存长度</param>
		/// <param name="secondBytes">接收缓冲区</param>
		/// <param name="secondIndex">接收缓冲区起始下标</param>
		/// <param name="secondLength">接收缓冲区长度</param>
		/// <returns>拼接后的字节数组</returns>
		public static byte[] CombineBytes(byte[] firstBytes, int firstIndex, int firstLength, byte[] secondBytes, int secondIndex, int secondLength)
		{
			using MemoryStream memoryStream = new MemoryStream(firstLength + secondLength);
			memoryStream.Write(firstBytes, firstIndex, firstLength);
			memoryStream.Write(secondBytes, secondIndex, secondLength);
			return memoryStream.ToArray();
		}

		/// <summary>
		/// 计算 Modbus CRC16 校验码；<br />
		/// 使用标准 Modbus RTU 多项式 0xA001
		/// </summary>
		/// <param name="buffer">待计算的字节数组</param>
		/// <returns>CRC16 校验码的字节数组（低位在前）</returns>
		public static byte[] calc_modbus_crc(byte[] buffer)
		{
			ushort num = ushort.MaxValue;
			for (int i = 0; i < buffer.Length; i++)
			{
				num ^= buffer[i];
				for (int j = 0; j < 8; j++)
				{
					if ((num & 1) != 0)
					{
						num >>= 1;
						num ^= 0xA001;
					}
					else
					{
						num >>= 1;
					}
				}
			}
			return BitConverter.GetBytes(num);
		}

		/// <summary>
		/// 计算 CRC16 校验码；<br />
		/// 支持自定义多项式，初始值为 0x0000
		/// </summary>
		/// <param name="polynomials">多项式参数</param>
		/// <param name="buffer">待计算的字节数组</param>
		/// <returns>CRC16 校验码的字节数组</returns>
		public static byte[] calc_crc_16(ushort polynomials, byte[] buffer)
		{
			ushort num = 0;
			for (int i = 0; i < buffer.Length; i++)
			{
				num ^= (ushort)(buffer[i] << 8);
				for (int j = 0; j < 8; j++)
				{
					if ((num & 0x8000) != 0)
					{
						num <<= 1;
						num ^= polynomials;
					}
					else
					{
						num <<= 1;
					}
				}
			}
			return BitConverter.GetBytes(num);
		}

		/// <summary>
		/// 计算 16位累加和校验；<br />
		/// 将所有字节值累加后返回 UInt16 结果
		/// </summary>
		/// <param name="buffer">待计算的字节数组</param>
		/// <returns>累加和的字节数组</returns>
		public static byte[] calc_sum_16(byte[] buffer)
		{
			ushort num = 0;
			for (int i = 0; i < buffer.Length; i++)
			{
				num += buffer[i];
			}
			return BitConverter.GetBytes(num);
		}

		/// <summary>
		/// 计算异或校验（XOR）；<br />
		/// 将所有字节进行异或运算，返回单字节结果
		/// </summary>
		/// <param name="buffer">待计算的字节数组</param>
		/// <returns>异或校验结果的单字节数组</returns>
		public static byte[] calc_xor(byte[] buffer)
		{
			byte b = 0;
			for (int i = 0; i < buffer.Length; i++)
			{
				b ^= buffer[i];
			}
			return new byte[1] { b };
		}

		/// <summary>
		/// 将十六进制字符串转换为字节数组；<br />
		/// 支持包含空格分隔的十六进制字符串（如 "0A 1B 2C"）
		/// </summary>
		/// <param name="str_hex">十六进制字符串</param>
		/// <returns>解析后的字节数组</returns>
		public static byte[] hex_to_byte(string str_hex)
		{
			List<byte> list = new List<byte>();
			str_hex = str_hex.Trim();
			while (str_hex.Length > 0)
			{
				int num = str_hex.Length;
				if (num > 2)
				{
					num = 2;
				}
				list.Add(byte.Parse(str_hex.Substring(0, num), NumberStyles.HexNumber));
				str_hex = str_hex.Substring(num).TrimStart();
			}
			return list.ToArray();
		}

		/// <summary>
		/// 十六进制字符串转十进制
		/// </summary>
		/// <param name="str">十六进制字符</param>
		/// <returns></returns>
		public static int To16Convert10(string str)
		{
			int num = 0;
			try
			{
				str = str.Trim().Replace(" ", "");
				return int.Parse(str, NumberStyles.AllowHexSpecifier);
			}
			catch (Exception)
			{
				return 0;
			}
		}

		/// <summary>
		/// 去除 byte[] 数组缓冲区内的尾部空白区 (0x00)；<br />
		/// 从末尾向前查找第一个非零字节的位置，截取有效数据；<br />
		/// 性能优化：避免了 List + RemoveAt 的 O(n²) 复杂度，改为 O(n)
		/// </summary>
		/// <param name="bytes">待处理的字节数组</param>
		/// <returns>去除尾部零字节后的新数组</returns>
		public static byte[] TrimEnd(this byte[] bytes)
		{
			int num = bytes.Length - 1;
			while (num >= 0 && bytes[num] == 0)
			{
				num--;
			}
			return bytes.AsSpan(0, num + 1).ToArray();
		}

		/// <summary>
		/// 16进制转字符串
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string HexToStr(this byte[] bytes, bool ClearEmpty = false)
		{
			if (ClearEmpty)
			{
				bytes = bytes.TrimEnd();
			}
			return ByteArrayToHexString(bytes);
		}

		/// <summary>
		/// byte[] 分割
		/// </summary>
		/// <param name="Data">数据</param>
		/// <param name="Segmentation">几位几位分割</param>
		public static List<byte[]> ByteListSegmentation(this byte[] Data, int Segmentation)
		{
			List<byte[]> list = new List<byte[]>();
			for (int i = 0; i < Data.Length; i += Segmentation)
			{
				byte[] array = new byte[Segmentation];
				for (int j = 0; j < Segmentation; j++)
				{
					array[j] = Data[i + j];
				}
				list.Add(array);
				if (list.Count.Equals(Data.Length / Segmentation))
				{
					break;
				}
			}
			return list;
		}

		/// <summary>
		/// 将十六进制数字字符串(例如:E4 CA B2)转换为字节数组。
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static byte[]? HexStringToByteArray(string s)
		{
			return s.StrToHex();
		}

		/// <summary>
		/// 16进制转字符串；<br />
		/// 自动判断入参是 byte[] 还是 byte，分别调用对应的转换方法
		/// </summary>
		/// <param name="bytes">字节对象（byte[] 或 byte）</param>
		/// <param name="ClearEmpty">是否清除尾部零字节</param>
		/// <returns>十六进制字符串</returns>
		public static string HexToStr(this object bytes, bool ClearEmpty = false)
		{
			if (bytes is byte[] bytes2)
			{
				return bytes2.HexToStr(ClearEmpty);
			}
			return ((byte)bytes).HexToStr();
		}

		/// <summary>
		/// 16进制转字符串
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string HexToStr(this byte bytes)
		{
			return bytes.ToString("x2");
		}

		/// <summary>
		/// 16进制字符串转换
		/// </summary>
		/// <param name="hexstring">用空格切割字符串</param>
		/// <returns>返回一个二进制字符串</returns>
		public static byte[] StrToHex(this string hexstring)
		{
			string[] array = hexstring.Trim().Split(' ');
			byte[] array2 = new byte[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = Convert.ToByte(array[i], 16);
			}
			return array2;
		}

		/// <summary>
		/// 将字节数组转换为 EA AX 字符串
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static string ByteArrayToHexString(byte[] data)
		{
			return string.Join(" ", data.Select((byte t) => t.ToString("X2")));
		}

		/// <summary>
		/// 16进制字符串转字节数组
		/// </summary>
		/// <param name="str"></param>
		/// <param name="strict">严格模式（严格按两个字母间隔一个空格）</param>
		/// <returns></returns>
		public static byte[] StringToByteArray(this string str, bool strict = true)
		{
			if (string.IsNullOrWhiteSpace(str) || str.Trim().Replace(" ", "").Length % 2 != 0)
			{
				throw new ArgumentException("请传入有效的参数");
			}
			if (strict)
			{
				return (from t in str.Split(' ')
					where t != null && t.Length == 2
					select Convert.ToByte(t, 16)).ToArray();
			}
			str = str.Trim().Replace(" ", "");
			List<byte> list = new List<byte>();
			int num;
			for (num = 0; num < str.Length; num++)
			{
				string value = string.Concat(new ReadOnlySpan<char>(str[num]), new ReadOnlySpan<char>(str[++num]));
				list.Add(Convert.ToByte(value, 16));
			}
			return list.ToArray();
		}

		/// <summary>
		/// Asciis数组字符串装字节数组
		/// 如：30 31 =》 00 01
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static byte[] AsciiArrayToByteArray(this byte[] str)
		{
			if (str == null || !str.Any())
			{
				throw new ArgumentException("请传入有效的参数");
			}
			List<string> list = new List<string>();
			foreach (byte b in str)
			{
				char c = (char)b;
				list.Add(c.ToString());
			}
			return string.Join("", list).StringToByteArray(strict: false);
		}

		/// <summary>
		/// 字节数组转换成Ascii字节数组
		/// 如：00 01 =&gt; 30 31
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static byte[] ByteArrayToAsciiArray(this byte[] str)
		{
			return Encoding.ASCII.GetBytes(string.Join("", str.Select((byte t) => t.ToString("X2"))));
		}

		/// <summary>
		/// Int转二进制
		/// </summary>
		/// <param name="value"></param>
		/// <param name="minLength">补0长度</param>
		/// <returns></returns>
		public static string IntToBinaryArray(this int value, int minLength = 0)
		{
			return Convert.ToString(value, 2).PadLeft(minLength, '0');
		}

		/// <summary>
		/// 二进制转Int
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static int BinaryArrayToInt(this string value)
		{
			return Convert.ToInt32(value, 2);
		}

		/// <summary>
		/// byte[] 字节分割
		/// 保存预留位
		/// </summary>
		/// <param name="FileStreamData">文件流数据</param>
		/// <param name="ReserveLocation">头部预留位</param>
		/// <param name="SplitCount">分割数 几位几位分割</param>
		/// <returns></returns>
		public static List<byte[]> ByteDataSplit(this byte[] FileStreamData, int ReserveLocation, int SplitCount)
		{
			List<byte[]> list = new List<byte[]>();
			double num = Convert.ToDouble(FileStreamData.Length) / Convert.ToDouble(SplitCount - ReserveLocation);
			int num2 = (num.ToString("0.00").Contains(".00") ? int.Parse(num.ToString()) : int.Parse(((int)num + 1).ToString()));
			int num3 = 0;
			for (int i = 0; i < num2; i++)
			{
				byte[] array = new byte[SplitCount];
				for (int j = ReserveLocation; j < SplitCount; j++)
				{
					array[j] = FileStreamData[num3];
					if (num3.Equals(FileStreamData.Length - 1))
					{
						break;
					}
					num3++;
				}
				list.Add(array);
			}
			return list;
		}

		/// <summary>
		/// CRC效验
		/// </summary>
		/// <param name="Data">数据</param>
		/// <returns></returns>
		public static (byte High, byte Low) CRC(byte[] Data)
		{
			ushort num = 0;
			for (uint num2 = 7u; num2 < Data.Length; num2++)
			{
				num = (ushort)((num << 8) ^ CRC_Key[(num >> 8) ^ Data[num2]]);
			}
			byte item = (byte)((num >> 8) & 0xFF);
			byte item2 = (byte)(num & 0xFF);
			return (High: item, Low: item2);
		}

		/// <summary>
		/// 字符串形式的字节集合数据转成byte[]
		/// </summary>
		/// <returns></returns>
		public static byte[] StrByteToBytes(string str)
		{
			string[] array = str.Split('|');
			byte[] array2 = new byte[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				try
				{
					array2[i] = byte.Parse(array[i]);
				}
				catch
				{
					array2[i] = Convert.ToByte(array[i], 16);
				}
			}
			return array2;
		}

		/// <summary>
		/// byte集合 掐头去尾 起始位1开始
		/// </summary>
		/// <param name="Data">数据</param>
		/// <param name="StartIndex">起始位</param>
		/// <param name="DeleteAfterIndex">移除后几位</param>
		/// <returns></returns>
		public static byte[] BytesDispose(byte[] Data, int StartIndex, int DeleteAfterIndex)
		{
			byte[] array = new byte[Data.Length - (StartIndex + DeleteAfterIndex)];
			try
			{
				for (int i = StartIndex; i <= Data.Length; i++)
				{
					array[i - StartIndex] = Data[i];
					if (i.Equals(Data.Length - (DeleteAfterIndex + 1)))
					{
						break;
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Byte[] 掐头去尾异常：" + ex.Message);
			}
			return array;
		}

		/// <summary>
		/// 根据传入的枚举属性获得对应值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte GetEnumValue<T>(string value)
		{
			Type? typeFromHandle = typeof(T);
			return (byte)Convert.ToInt32(Enum.Format(typeFromHandle, Enum.Parse(typeFromHandle, value.ToUpper()), "d"));
		}

		/// <summary>
		/// 字节byte[] 转int   4个字节转换INT 字节不够自动补零
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static int ByteToInt(byte[] data, bool IsLittleEndian = false)
		{
			if (IsLittleEndian)
			{
				Array.Reverse(data);
			}
			if (data.Length != 4)
			{
				byte[] array = new byte[4];
				for (int i = 0; i < array.Length; i++)
				{
					if (i < data.Length)
					{
						array[i] = data[i];
					}
					else
					{
						array[i] = 0;
					}
				}
				data = array;
			}
			return BitConverter.ToInt32(data, 0);
		}

		/// <summary>
		/// 字节转换为整数；<br />
		/// 自动判断入参是 byte[] 还是 byte，分别调用对应的转换方法
		/// </summary>
		/// <param name="data">字节对象（byte[] 或 byte）</param>
		/// <param name="IsLittleEndian">是否为小端序</param>
		/// <returns>转换后的整数</returns>
		public static int ByteToInt(object data, bool IsLittleEndian = false)
		{
			if (data is byte[] data2)
			{
				return ByteToInt(data2, IsLittleEndian);
			}
			return ByteToInt((byte)data, IsLittleEndian);
		}

		public static int ByteToInt(byte data, bool IsLittleEndian = false)
		{
			byte[] array = new byte[1] { data };
			if (IsLittleEndian)
			{
				Array.Reverse(array);
			}
			if (array.Length != 4)
			{
				byte[] array2 = new byte[4];
				for (int i = 0; i < array2.Length; i++)
				{
					if (i < array.Length)
					{
						array2[i] = array[i];
					}
					else
					{
						array2[i] = 0;
					}
				}
				array = array2;
			}
			return BitConverter.ToInt32(array, 0);
		}

		/// <summary>
		/// 将int数值转换为占四个字节的byte数组
		/// 低位在前，高位在后
		/// </summary>
		/// <returns></returns>
		public static byte[] IntToBytes(int value)
		{
			byte[] array = new byte[4];
			array[3] = (byte)((value >> 24) & 0xFF);
			array[2] = (byte)((value >> 16) & 0xFF);
			array[1] = (byte)((value >> 8) & 0xFF);
			array[0] = (byte)(value & 0xFF);
			return array.TrimEnd();
		}

		/// 将int数值转换为占四个字节的byte数组，本方法适用于(低位在前，高位在后)的顺序。 和bytesToInt（）配套使用
		/// @param value
		///            要转换的int值
		/// @return byte数组
		public static byte[] intToBytes0(int value)
		{
			byte[] array = new byte[4];
			array[3] = (byte)((value >> 24) & 0xFF);
			array[2] = (byte)((value >> 16) & 0xFF);
			array[1] = (byte)((value >> 8) & 0xFF);
			array[0] = (byte)(value & 0xFF);
			return array;
		}

		/// 将int数值转换为占四个字节的byte数组，本方法适用于(高位在前，低位在后)的顺序。  和bytesToInt2（）配套使用
		public static byte[] intToBytes1(int value)
		{
			return new byte[4]
			{
				(byte)((value >> 24) & 0xFF),
				(byte)((value >> 16) & 0xFF),
				(byte)((value >> 8) & 0xFF),
				(byte)(value & 0xFF)
			};
		}

		/// <summary>
		/// 将一条十六进制字符串转换为ASCII
		/// </summary>
		/// <param name="hexstring">一条十六进制字符串</param>
		/// <returns>返回一条ASCII码</returns>
		public static string HexStringToASCII(string hexstring)
		{
			byte[] array = hexstring.StrToHex();
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text = text + array[i] + " ";
			}
			string[] array2 = text.Trim().Split(new char[1] { ' ' });
			char[] array3 = new char[array2.Length];
			for (int j = 0; j < array3.Length; j++)
			{
				int value = Convert.ToInt32(array2[j]);
				array3[j] = Convert.ToChar(value);
			}
			return new string(array3);
		}

		public static int StringToHexOrDec(string strData)
		{
			int result = -1;
			try
			{
				result = ((strData.Length <= 2) ? int.Parse(strData, NumberStyles.Integer) : ((!strData.Substring(0, 2).Equals("0x") && !strData.Substring(0, 2).Equals("0X")) ? int.Parse(strData, NumberStyles.Integer) : int.Parse(strData.Substring(2, strData.Length - 2), NumberStyles.HexNumber)));
			}
			catch (Exception)
			{
			}
			return result;
		}

		/// <summary>
		/// 对接收到的数据进行解包（将接收到的byte型数组解包为Unicode字符串）
		/// </summary>
		/// <param name="recbytes">byte型数组</param>
		/// <returns>Unicode编码的字符串</returns>
		public static string disPackage(byte[] recbytes)
		{
			string text = "";
			foreach (byte b in recbytes)
			{
				text = text + b.ToString("X2") + " ";
			}
			return text;
		}

		public static byte[] int2ByteArray(int i)
		{
			return new byte[4]
			{
				(byte)((i >> 24) & 0xFF),
				(byte)((i >> 16) & 0xFF),
				(byte)((i >> 8) & 0xFF),
				(byte)(i & 0xFF)
			};
		}

		/// byte[]转int
		/// 利用int2ByteArray方法，将一个int转为byte[]，但在解析时，需要将数据还原。同样使用移位的方式，将适当的位数进行还原，
		/// 0xFF为16进制的数据，所以在其后每加上一位，就相当于二进制加上4位。同时，使用|=号拼接数据，将其还原成最终的int数据
		/// @param bytes byte类型数组
		/// @return int数字
		/// <summary>
		/// 将 4 字节数组转换为 int（高字节在前，低字节在后）；<br />
		/// 与 int2ByteArray 方法配套使用；<br />
		/// 修复：最高位掩码由 0xFF0000 改为 0x7F000000（int 有符号位）
		/// </summary>
		/// <param name="bytes">byte 类型数组</param>
		/// <returns>int 数字</returns>
		public static int bytes2Int(byte[] bytes)
		{
			return (bytes[3] & 0xFF) | ((bytes[2] << 8) & 0xFF00) | ((bytes[1] << 16) & 0xFF0000) | ((bytes[0] << 24) & -16777216);
		}

		public static string Int2String(int str)
		{
			return Convert.ToString(str);
		}

		/// <summary>
		/// 字符串转换为整数；<br />
		/// 转换失败时返回 0
		/// </summary>
		/// <param name="str">待转换的字符串</param>
		/// <returns>转换后的整数值</returns>
		public static int String2Int(string str)
		{
			if (!int.TryParse(str, out var result))
			{
				return 0;
			}
			return result;
		}

		public static byte[] IntToByteArray2(int value)
		{
			return new byte[4]
			{
				(byte)((value >> 24) & 0xFF),
				(byte)((value >> 16) & 0xFF),
				(byte)((value >> 8) & 0xFF),
				(byte)(value & 0xFF)
			};
		}

		public static int ByteArrayToInt2(byte[] bArr)
		{
			if (bArr.Length != 4)
			{
				return -1;
			}
			return ((bArr[0] & 0xFF) << 24) | ((bArr[1] & 0xFF) << 16) | ((bArr[2] & 0xFF) << 8) | (bArr[3] & 0xFF);
		}

		/// <summary>
		/// 字节字符串转字节
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string StringToHexArray(string input)
		{
			char[] array = input.ToCharArray();
			StringBuilder stringBuilder = new StringBuilder(input.Length * 3);
			char[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				int num = Convert.ToInt32(array2[i]);
				_ = $"{num:X}";
				stringBuilder.Append(Convert.ToString(num, 16).PadLeft(2, '0').PadRight(3, ' '));
			}
			return stringBuilder.ToString().ToUpper();
		}

		/// <summary>
		/// 将数组格式化为显示的字符串的信息，支持所有的类型对象
		/// </summary>
		/// <typeparam name="T">数组的类型</typeparam>
		/// <param name="array">数组信息</param>
		/// <returns>最终显示的信息</returns>
		public static string ArrayFormat<T>(T[] array)
		{
			return ArrayFormat(array, string.Empty);
		}

		/// <summary>
		/// 将数组格式化为显示的字符串的信息，支持所有的类型对象
		/// </summary>
		/// <typeparam name="T">数组的类型</typeparam>
		/// <param name="array">数组信息</param>
		/// <param name="format">格式化的信息</param>
		/// <returns>最终显示的信息</returns>
		public static string ArrayFormat<T>(T[] array, string format)
		{
			if (array == null)
			{
				return "NULL";
			}
			StringBuilder stringBuilder = new StringBuilder("[");
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(string.IsNullOrEmpty(format) ? array[i].ToString() : string.Format(format, array[i]));
				if (i != array.Length - 1)
				{
					stringBuilder.Append(",");
				}
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		/// <summary>
		/// 将数组格式化为显示的字符串的信息，支持所有的类型对象
		/// </summary>
		/// <typeparam name="T">数组的类型</typeparam>
		/// <param name="array">数组信息</param>
		/// <returns>最终显示的信息</returns>
		public static string ArrayFormat<T>(T array)
		{
			return ArrayFormat(array, string.Empty);
		}

		/// <summary>
		/// 将数组格式化为显示的字符串的信息，支持所有的类型对象
		/// </summary>
		/// <typeparam name="T">数组的类型</typeparam>
		/// <param name="array">数组信息</param>
		/// <param name="format">格式化的信息</param>
		/// <returns>最终显示的信息</returns>
		public static string ArrayFormat<T>(T array, string format)
		{
			StringBuilder stringBuilder = new StringBuilder("[");
			if (array is Array array2)
			{
				foreach (object item in array2)
				{
					stringBuilder.Append(string.IsNullOrEmpty(format) ? item.ToString() : string.Format(format, item));
					stringBuilder.Append(",");
				}
				if (array2.Length > 0 && stringBuilder[stringBuilder.Length - 1] == ',')
				{
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
				}
			}
			else
			{
				stringBuilder.Append(string.IsNullOrEmpty(format) ? array.ToString() : string.Format(format, array));
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		/// <summary>
		/// 一个通用的数组新增个数方法，会自动判断越界情况，越界的情况下，会自动的截断或是填充
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="array">原数据</param>
		/// <param name="data">等待新增的数据</param>
		/// <param name="max">原数据的最大值</param>
		public static void AddArrayData<T>(ref T[] array, T[] data, int max)
		{
			if (data == null || data.Length == 0)
			{
				return;
			}
			if (array.Length == max)
			{
				Array.Copy(array, data.Length, array, 0, array.Length - data.Length);
				Array.Copy(data, 0, array, array.Length - data.Length, data.Length);
			}
			else if (array.Length + data.Length > max)
			{
				T[] array2 = new T[max];
				for (int i = 0; i < max - data.Length; i++)
				{
					array2[i] = array[i + (array.Length - max + data.Length)];
				}
				for (int j = 0; j < data.Length; j++)
				{
					array2[array2.Length - data.Length + j] = data[j];
				}
				array = array2;
			}
			else
			{
				T[] array3 = new T[array.Length + data.Length];
				for (int k = 0; k < array.Length; k++)
				{
					array3[k] = array[k];
				}
				for (int l = 0; l < data.Length; l++)
				{
					array3[array3.Length - data.Length + l] = data[l];
				}
				array = array3;
			}
		}

		/// <summary>
		/// 将字节数组按“字”(2字节)进行翻转
		/// 例如：
		/// [A,B,C,D] → [B,A,D,C]
		///
		/// 如果字节长度为奇数，则会自动补齐为偶数长度
		/// </summary>
		/// <param name="inBytes">输入字节数组</param>
		/// <returns>
		/// 翻转后的字节数组；
		/// 若输入为 null 则返回 null；
		/// 若为空数组则返回空数组
		/// </returns>
		public static byte[] BytesReverseByWord(byte[] inBytes)
		{
			if (inBytes == null)
			{
				return null;
			}
			if (inBytes.Length == 0)
			{
				return new byte[0];
			}
			byte[] array = ArrayExpandToLengthEven(inBytes.ToArray());
			for (int i = 0; i < array.Length / 2; i++)
			{
				byte b = array[i * 2];
				array[i * 2] = array[i * 2 + 1];
				array[i * 2 + 1] = b;
			}
			return array;
		}

		/// <summary>
		/// 如果数组长度为奇数，则自动扩展为偶数长度
		/// 扩展部分使用默认值填充
		/// </summary>
		/// <typeparam name="T">数组元素类型</typeparam>
		/// <param name="data">源数组</param>
		/// <returns>
		/// 偶数长度数组
		/// </returns>
		public static T[] ArrayExpandToLengthEven<T>(T[] data)
		{
			if (data == null)
			{
				return new T[0];
			}
			if (data.Length % 2 == 1)
			{
				return ArrayExpandToLength(data, data.Length + 1);
			}
			return data;
		}

		/// <summary>
		/// 将数组扩展到指定长度
		/// 若目标长度大于原长度，则补默认值；
		/// 若目标长度小于原长度，则截断
		/// </summary>
		/// <typeparam name="T">数组元素类型</typeparam>
		/// <param name="data">源数组</param>
		/// <param name="length">目标长度</param>
		/// <returns>扩展或截断后的新数组</returns>
		public static T[] ArrayExpandToLength<T>(T[] data, int length)
		{
			if (data == null)
			{
				return new T[length];
			}
			if (data.Length == length)
			{
				return data;
			}
			T[] array = new T[length];
			Array.Copy(data, array, Math.Min(data.Length, array.Length));
			return array;
		}

		/// <summary>
		/// 将指定的数据按照指定长度进行分割，例如int[10]，指定长度4，就分割成int[4],int[4],int[2]，然后拼接list
		/// </summary>
		/// <typeparam name="T">数组的类型</typeparam>
		/// <param name="array">等待分割的数组</param>
		/// <param name="length">指定的长度信息</param>
		/// <returns>分割后结果内容</returns>
		public static List<T[]> ArraySplitByLength<T>(T[] array, int length)
		{
			if (array == null)
			{
				return new List<T[]>();
			}
			List<T[]> list = new List<T[]>();
			int num = 0;
			while (num < array.Length)
			{
				if (num + length < array.Length)
				{
					T[] array2 = new T[length];
					Array.Copy(array, num, array2, 0, length);
					num += length;
					list.Add(array2);
				}
				else
				{
					T[] array3 = new T[array.Length - num];
					Array.Copy(array, num, array3, 0, array3.Length);
					num += length;
					list.Add(array3);
				}
			}
			return list;
		}

		/// <summary>
		/// 将整数进行有效的拆分成数组，指定每个元素的最大值
		/// </summary>
		/// <param name="integer">整数信息</param>
		/// <param name="everyLength">单个的数组长度</param>
		/// <returns>拆分后的数组长度</returns>
		public static int[] SplitIntegerToArray(int integer, int everyLength)
		{
			int[] array = new int[integer / everyLength + ((integer % everyLength != 0) ? 1 : 0)];
			for (int i = 0; i < array.Length; i++)
			{
				if (i == array.Length - 1)
				{
					array[i] = ((integer % everyLength == 0) ? everyLength : (integer % everyLength));
				}
				else
				{
					array[i] = everyLength;
				}
			}
			return array;
		}

		/// <summary>
		/// 判断两个字节的指定部分是否相同
		/// </summary>
		/// <param name="b1">第一个字节</param>
		/// <param name="start1">第一个字节的起始位置</param>
		/// <param name="b2">第二个字节</param>
		/// <param name="start2">第二个字节的起始位置</param>
		/// <param name="length">校验的长度</param>
		/// <returns>返回是否相等</returns>
		public static bool IsTwoBytesEquel(byte[] b1, int start1, byte[] b2, int start2, int length)
		{
			if (b1 == null || b2 == null)
			{
				return false;
			}
			for (int i = 0; i < length; i++)
			{
				if (b1[i + start1] != b2[i + start2])
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 判断两个字节的指定部分是否相同
		/// </summary>
		/// <param name="b1">第一个字节</param>
		/// <param name="b2">第二个字节</param>
		/// <returns>返回是否相等</returns>
		public static bool IsTwoBytesEquel(byte[] b1, byte[] b2)
		{
			if (b1 == null || b2 == null)
			{
				return false;
			}
			if (b1.Length != b2.Length)
			{
				return false;
			}
			return IsTwoBytesEquel(b1, 0, b2, 0, b1.Length);
		}

		/// <summary>
		/// 判断两个数据的令牌是否相等
		/// </summary>
		/// <param name="head">字节数据</param>
		/// <param name="token">GUID数据</param>
		/// <returns>返回是否相等</returns>
		public static bool IsByteTokenEquel(byte[] head, Guid token)
		{
			return IsTwoBytesEquel(head, 12, token.ToByteArray(), 0, 16);
		}

		/// <summary>
		/// 判断两个数据的令牌是否相等
		/// </summary>
		/// <param name="token1">第一个令牌</param>
		/// <param name="token2">第二个令牌</param>
		/// <returns>返回是否相等</returns>
		public static bool IsTwoTokenEquel(Guid token1, Guid token2)
		{
			return IsTwoBytesEquel(token1.ToByteArray(), 0, token2.ToByteArray(), 0, 16);
		}

		/// <summary>
		/// 获取一个枚举类型的所有枚举值，可直接应用于组合框数据
		/// </summary>
		/// <typeparam name="TEnum">枚举的类型值</typeparam>
		/// <returns>枚举值数组</returns>
		public static TEnum[] GetEnumValues<TEnum>() where TEnum : struct
		{
			return (TEnum[])Enum.GetValues(typeof(TEnum));
		}

		/// <summary>
		/// 从字符串的枚举值数据转换成真实的枚举值数据
		/// </summary>
		/// <typeparam name="TEnum">枚举的类型值</typeparam>
		/// <param name="value">枚举的字符串的数据值</param>
		/// <returns>真实的枚举值</returns>
		public static TEnum GetEnumFromString<TEnum>(string value) where TEnum : struct
		{
			return (TEnum)Enum.Parse(typeof(TEnum), value);
		}

		/// <summary>
		/// 一个泛型的方法，提供XML对象的数据读取为实际的数据，支持BCL的基础类型。
		/// </summary>
		/// <typeparam name="T">类型对象</typeparam>
		/// <param name="element">元素信息</param>
		/// <param name="name">属性的名称</param>
		/// <param name="defaultValue">默认值信息</param>
		/// <returns>解析后的值</returns>
		public static T GetXmlValue<T>(XElement element, string name, T defaultValue)
		{
			if (element.Attribute(name) == null)
			{
				return defaultValue;
			}
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(bool))
			{
				return (T)(object)bool.Parse(element.Attribute(name).Value);
			}
			if (typeFromHandle == typeof(bool[]))
			{
				return (T)(object)element.Attribute(name).Value.ToStringArray<bool>();
			}
			if (typeFromHandle == typeof(byte))
			{
				return (T)(object)byte.Parse(element.Attribute(name).Value);
			}
			if (typeFromHandle == typeof(byte[]))
			{
				return (T)(object)element.Attribute(name).Value.ToHexBytes();
			}
			if (typeFromHandle == typeof(sbyte))
			{
				return (T)(object)sbyte.Parse(element.Attribute(name).Value);
			}
			if (typeFromHandle == typeof(sbyte[]))
			{
				return (T)(object)element.Attribute(name).Value.ToStringArray<sbyte>();
			}
			if (typeFromHandle == typeof(short))
			{
				return (T)(object)short.Parse(element.Attribute(name).Value);
			}
			if (typeFromHandle == typeof(short[]))
			{
				return (T)(object)element.Attribute(name).Value.ToStringArray<short>();
			}
			if (typeFromHandle == typeof(ushort))
			{
				return (T)(object)ushort.Parse(element.Attribute(name).Value);
			}
			if (typeFromHandle == typeof(ushort[]))
			{
				return (T)(object)element.Attribute(name).Value.ToStringArray<ushort>();
			}
			if (typeFromHandle == typeof(int))
			{
				return (T)(object)int.Parse(element.Attribute(name).Value);
			}
			if (typeFromHandle == typeof(int[]))
			{
				return (T)(object)element.Attribute(name).Value.ToStringArray<int>();
			}
			if (typeFromHandle == typeof(uint))
			{
				return (T)(object)uint.Parse(element.Attribute(name).Value);
			}
			if (typeFromHandle == typeof(uint[]))
			{
				return (T)(object)element.Attribute(name).Value.ToStringArray<uint>();
			}
			if (typeFromHandle == typeof(long))
			{
				return (T)(object)long.Parse(element.Attribute(name).Value);
			}
			if (typeFromHandle == typeof(long[]))
			{
				return (T)(object)element.Attribute(name).Value.ToStringArray<long>();
			}
			if (typeFromHandle == typeof(ulong))
			{
				return (T)(object)ulong.Parse(element.Attribute(name).Value);
			}
			if (typeFromHandle == typeof(ulong[]))
			{
				return (T)(object)element.Attribute(name).Value.ToStringArray<ulong>();
			}
			if (typeFromHandle == typeof(float))
			{
				return (T)(object)float.Parse(element.Attribute(name).Value);
			}
			if (typeFromHandle == typeof(float[]))
			{
				return (T)(object)element.Attribute(name).Value.ToStringArray<float>();
			}
			if (typeFromHandle == typeof(double))
			{
				return (T)(object)double.Parse(element.Attribute(name).Value);
			}
			if (typeFromHandle == typeof(double[]))
			{
				return (T)(object)element.Attribute(name).Value.ToStringArray<double>();
			}
			if (typeFromHandle == typeof(DateTime))
			{
				return (T)(object)DateTime.Parse(element.Attribute(name).Value);
			}
			if (typeFromHandle == typeof(DateTime[]))
			{
				return (T)(object)element.Attribute(name).Value.ToStringArray<DateTime>();
			}
			if (typeFromHandle == typeof(string))
			{
				return (T)(object)element.Attribute(name).Value;
			}
			if (typeFromHandle == typeof(string[]))
			{
				return (T)(object)element.Attribute(name).Value.ToStringArray<string>();
			}
			throw new Exception("not supported type:" + typeFromHandle.Name);
		}

		/// <summary>
		/// 字节数据转化成16进制表示的字符串
		/// </summary>
		/// <param name="InBytes">字节数组</param>
		/// <returns>返回的字符串</returns>
		public static string ByteToHexString(byte[] InBytes)
		{
			return ByteToHexString(InBytes, '\0');
		}

		/// <summary>
		/// 字节数据转化成16进制表示的字符串
		/// </summary>
		/// <param name="InBytes">字节数组</param>
		/// <param name="segment">分割符</param>
		/// <returns>返回的字符串</returns>
		public static string ByteToHexString(byte[] InBytes, char segment)
		{
			return ByteToHexString(InBytes, segment, 0);
		}

		/// <summary>
		/// 字节数据转化成16进制表示的字符串
		/// </summary>
		/// <param name="InBytes">字节数组</param>
		/// <param name="segment">分割符，如果设置为0，则没有分隔符信息</param>
		/// <param name="newLineCount">每隔指定数量的时候进行换行，如果小于等于0，则不进行分行显示</param>
		/// <param name="format">格式信息，默认为{0:X2}</param>
		/// <returns>返回的字符串</returns>
		public static string ByteToHexString(byte[] InBytes, char segment, int newLineCount, string format = "{0:X2}")
		{
			if (InBytes == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			long num = 0L;
			foreach (byte b in InBytes)
			{
				if (segment == '\0')
				{
					stringBuilder.Append(string.Format(format, b));
				}
				else
				{
					stringBuilder.Append(string.Format(format + "{1}", b, segment));
				}
				num++;
				if (newLineCount > 0 && num >= newLineCount)
				{
					stringBuilder.Append(Environment.NewLine);
					num = 0L;
				}
			}
			if (segment != 0 && stringBuilder.Length > 1 && stringBuilder[stringBuilder.Length - 1] == segment)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// 字符串数据转化成16进制表示的字符串<br />
		/// String data into a string of 16 binary representations
		/// </summary>
		/// <param name="InString">输入的字符串数据</param>
		/// <returns>返回的字符串</returns>
		public static string ByteToHexString(string InString)
		{
			return ByteToHexString(Encoding.Unicode.GetBytes(InString));
		}

		private static int GetHexCharIndex(char ch)
		{
			switch (ch)
			{
			case '0':
				return 0;
			case '1':
				return 1;
			case '2':
				return 2;
			case '3':
				return 3;
			case '4':
				return 4;
			case '5':
				return 5;
			case '6':
				return 6;
			case '7':
				return 7;
			case '8':
				return 8;
			case '9':
				return 9;
			case 'A':
			case 'a':
				return 10;
			case 'B':
			case 'b':
				return 11;
			case 'C':
			case 'c':
				return 12;
			case 'D':
			case 'd':
				return 13;
			case 'E':
			case 'e':
				return 14;
			case 'F':
			case 'f':
				return 15;
			default:
				return -1;
			}
		}

		/// <summary>
		/// 将16进制的字符串转化成Byte数据，将检测每2个字符转化，也就是说，中间可以是任意字符
		/// </summary>
		/// <param name="hex">十六进制的字符串，中间可以是任意的分隔符</param>
		/// <returns>转换后的字节数组</returns>
		/// <remarks>参数举例：AA 01 34 A8</remarks>
		public static byte[] HexStringToBytes(string hex)
		{
			MemoryStream memoryStream = new MemoryStream();
			for (int i = 0; i < hex.Length; i++)
			{
				if (i + 1 < hex.Length && GetHexCharIndex(hex[i]) >= 0 && GetHexCharIndex(hex[i + 1]) >= 0)
				{
					memoryStream.WriteByte((byte)(GetHexCharIndex(hex[i]) * 16 + GetHexCharIndex(hex[i + 1])));
					i++;
				}
			}
			byte[] result = memoryStream.ToArray();
			memoryStream.Dispose();
			return result;
		}

		/// <summary>
		/// 将字节数组显示为ASCII格式的字符串，当遇到0x20以下及0x7E以上的不可见字符时，使用十六进制的数据显示
		/// </summary>
		/// <param name="content">字节数组信息</param>
		/// <returns>ASCII格式的字符串信息</returns>
		public static string GetAsciiStringRender(byte[] content)
		{
			if (content == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < content.Length; i++)
			{
				if (content[i] < 32 || content[i] > 126)
				{
					StringBuilder stringBuilder2 = stringBuilder;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 1, stringBuilder2);
					handler.AppendLiteral("\\");
					handler.AppendFormatted(content[i], "X2");
					stringBuilder2.Append(ref handler);
				}
				else
				{
					stringBuilder.Append((char)content[i]);
				}
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// 从显示的ASCII格式的字符串，转为原始字节数组，如果遇到 \00 这种表示原始字节的内容，则直接进行转换操作，遇到 \r 直接转换 0x0D, \n 直接转换 0x0A
		/// </summary>
		/// <param name="render">等待转换的字符串</param>
		/// <returns>原始字节数组</returns>
		public static byte[] GetFromAsciiStringRender(string render)
		{
			if (string.IsNullOrEmpty(render))
			{
				return new byte[0];
			}
			MatchEvaluator evaluator = (Match m) => $"{(char)Convert.ToByte(m.Value.Substring(1), 16)}";
			return Encoding.ASCII.GetBytes(Regex.Replace(render.Replace("\\r", "\r").Replace("\\n", "\n"), "\\\\[0-9A-Fa-f]{2}", evaluator));
		}

		/// <summary>
		/// 将原始的byte数组转换成ascii格式的byte数组
		/// </summary>
		/// <param name="inBytes">等待转换的byte数组</param>
		/// <returns>转换后的数组</returns>
		public static byte[] BytesToAsciiBytes(byte[] inBytes)
		{
			return Encoding.ASCII.GetBytes(ByteToHexString(inBytes));
		}

		/// <summary>
		/// 将ascii格式的byte数组转换成原始的byte数组<br />
		/// Converts an ASCII-formatted byte array to the original byte array
		/// </summary>
		/// <param name="inBytes">等待转换的byte数组</param>
		/// <returns>转换后的数组</returns>
		public static byte[] AsciiBytesToBytes(byte[] inBytes)
		{
			return HexStringToBytes(Encoding.ASCII.GetString(inBytes));
		}

		/// <summary>
		/// 从字节构建一个ASCII格式的数据内容<br />
		/// Build an ASCII-formatted data content from bytes
		/// </summary>
		/// <param name="value">数据</param>
		/// <returns>ASCII格式的字节数组</returns>
		public static byte[] BuildAsciiBytesFrom(byte value)
		{
			return Encoding.ASCII.GetBytes(value.ToString("X2"));
		}

		/// <summary>
		/// 从short构建一个ASCII格式的数据内容<br />
		/// Constructing an ASCII-formatted data content from a short
		/// </summary>
		/// <param name="value">数据</param>
		/// <returns>ASCII格式的字节数组</returns>
		public static byte[] BuildAsciiBytesFrom(short value)
		{
			return Encoding.ASCII.GetBytes(value.ToString("X4"));
		}

		/// <summary>
		/// 从ushort构建一个ASCII格式的数据内容<br />
		/// Constructing an ASCII-formatted data content from ushort
		/// </summary>
		/// <param name="value">数据</param>
		/// <returns>ASCII格式的字节数组</returns>
		public static byte[] BuildAsciiBytesFrom(ushort value)
		{
			return Encoding.ASCII.GetBytes(value.ToString("X4"));
		}

		/// <summary>
		/// 从uint构建一个ASCII格式的数据内容<br />
		/// Constructing an ASCII-formatted data content from uint
		/// </summary>
		/// <param name="value">数据</param>
		/// <returns>ASCII格式的字节数组</returns>
		public static byte[] BuildAsciiBytesFrom(uint value)
		{
			return Encoding.ASCII.GetBytes(value.ToString("X8"));
		}

		/// <summary>
		/// 从字节数组构建一个ASCII格式的数据内容<br />
		/// Byte array to construct an ASCII format data content
		/// </summary>
		/// <param name="value">字节信息</param>
		/// <returns>ASCII格式的地址</returns>
		public static byte[] BuildAsciiBytesFrom(byte[] value)
		{
			byte[] array = new byte[value.Length * 2];
			for (int i = 0; i < value.Length; i++)
			{
				BuildAsciiBytesFrom(value[i]).CopyTo(array, 2 * i);
			}
			return array;
		}

		/// <summary>
		/// 根据位偏移量获取对应的位掩码值
		/// 例如：
		/// offset = 0 → 0000 0001 (1)
		/// offset = 1 → 0000 0010 (2)
		/// offset = 7 → 1000 0000 (128)
		/// </summary>
		/// <param name="offset">
		/// 位偏移量（范围 0~7）
		/// 0 表示最低位（Bit0）
		/// 7 表示最高位（Bit7）
		/// </param>
		/// <returns>
		/// 对应位的字节掩码值；
		/// 若 offset 超出范围则返回 0
		/// </returns>
		private static byte GetDataByBitIndex(int offset)
		{
			return offset switch
			{
				0 => 1, 
				1 => 2, 
				2 => 4, 
				3 => 8, 
				4 => 16, 
				5 => 32, 
				6 => 64, 
				7 => 128, 
				_ => 0, 
			};
		}

		/// <summary>
		/// 获取byte数据类型的第offset位，是否为True
		/// </summary>
		/// <param name="value">byte数值</param>
		/// <param name="offset">索引位置</param>
		/// <returns>结果</returns>
		public static bool BoolOnByteIndex(byte value, int offset)
		{
			byte dataByBitIndex = GetDataByBitIndex(offset);
			return (value & dataByBitIndex) == dataByBitIndex;
		}

		/// <summary>
		/// 设置取byte数据类型的第offset位，是否为True
		/// </summary>
		/// <param name="byt">byte数值</param>
		/// <param name="offset">索引位置</param>
		/// <param name="value">写入的结果值</param>
		/// <returns>结果</returns>
		public static byte SetBoolOnByteIndex(byte byt, int offset, bool value)
		{
			byte dataByBitIndex = GetDataByBitIndex(offset);
			if (value)
			{
				return (byte)(byt | dataByBitIndex);
			}
			return (byte)(byt & ~dataByBitIndex);
		}

		/// <summary>
		/// 将布尔数组转换为字节数组
		/// 每8个bool组成一个字节
		/// </summary>
		/// <param name="array">布尔数组</param>
		/// <returns>
		/// 转换后的字节数组；
		/// 若输入为 null 则返回 null
		/// </returns>
		public static byte[] BoolArrayToByte(bool[] array)
		{
			if (array == null)
			{
				return null;
			}
			byte[] array2 = new byte[(array.Length % 8 == 0) ? (array.Length / 8) : (array.Length / 8 + 1)];
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i])
				{
					array2[i / 8] += GetDataByBitIndex(i % 8);
				}
			}
			return array2;
		}

		/// <summary>
		/// 将bool数组转换为字符串进行显示，true被转为1，false转换为0
		/// </summary>
		/// <param name="array">bool数组</param>
		/// <returns>转换后的字符串</returns>
		public static string BoolArrayToString(bool[] array)
		{
			if (array == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i] ? "1" : "0");
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// 从Byte数组中提取位数组，length代表位数，例如数组 03 A1 长度10转为 [1100 0000 10]
		/// </summary>
		/// <param name="inBytes">原先的字节数组</param>
		/// <param name="length">想要转换的长度，如果超出自动会缩小到数组最大长度</param>
		/// <returns>转换后的bool数组</returns>
		public static bool[] ByteToBoolArray(byte[] inBytes, int length)
		{
			if (inBytes == null)
			{
				return null;
			}
			if (length > inBytes.Length * 8)
			{
				length = inBytes.Length * 8;
			}
			bool[] array = new bool[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = BoolOnByteIndex(inBytes[i / 8], i % 8);
			}
			return array;
		}

		/// <summary>
		/// 从Byte数组中提取位数组，length代表位数，例如数组 01 00 长度10转为 [true,false]<br />
		/// </summary>
		/// <param name="inBytes">原先的字节数组</param>
		/// <param name="length">想要转换的长度，如果超出自动会缩小到数组最大长度</param>
		/// <param name="trueValue">当为true时对应的byte值</param>
		/// <returns>转换后bool数组</returns>
		public static bool[] ByteToBoolArray(byte[] inBytes, int length, byte trueValue)
		{
			if (inBytes == null)
			{
				return null;
			}
			if (length > inBytes.Length)
			{
				length = inBytes.Length;
			}
			bool[] array = new bool[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = inBytes[i] == trueValue;
			}
			return array;
		}

		/// <summary>
		/// 从Byte数组中提取所有的位数组
		/// </summary>
		/// <param name="InBytes">原先的字节数组</param>
		/// <returns>转换后的bool数组</returns>
		public static bool[] ByteToBoolArray(byte[] InBytes)
		{
			if (InBytes != null)
			{
				return ByteToBoolArray(InBytes, InBytes.Length * 8);
			}
			return null;
		}

		/// <summary>
		/// 将一个数组的前后移除指定位数，返回新的一个数组
		/// </summary>
		/// <param name="value">数组</param>
		/// <param name="leftLength">前面的位数</param>
		/// <param name="rightLength">后面的位数</param>
		/// <returns>新的数组</returns>
		public static T[] ArrayRemoveDouble<T>(T[] value, int leftLength, int rightLength)
		{
			if (value == null)
			{
				return null;
			}
			if (value.Length <= leftLength + rightLength)
			{
				return new T[0];
			}
			T[] array = new T[value.Length - leftLength - rightLength];
			Array.Copy(value, leftLength, array, 0, array.Length);
			return array;
		}

		/// <summary>
		/// 将一个数组的前面指定位数移除，返回新的一个数组
		/// </summary>
		/// <param name="value">数组</param>
		/// <param name="length">等待移除的长度</param>
		/// <returns>新的数组</returns>
		public static T[] ArrayRemoveBegin<T>(T[] value, int length)
		{
			return ArrayRemoveDouble(value, length, 0);
		}

		/// <summary>
		/// 将一个数组的后面指定位数移除，返回新的一个数组
		/// </summary>
		/// <param name="value">数组</param>
		/// <param name="length">等待移除的长度</param>
		/// <returns>新的数组</returns>
		public static T[] ArrayRemoveLast<T>(T[] value, int length)
		{
			return ArrayRemoveDouble(value, 0, length);
		}

		/// <summary>
		/// 获取到数组里面的中间指定长度的数组
		/// </summary>
		/// <param name="value">数组</param>
		/// <param name="index">起始索引</param>
		/// <param name="length">数据的长度</param>
		/// <returns>新的数组值</returns>
		public static T[] ArraySelectMiddle<T>(T[] value, int index, int length)
		{
			if (value == null)
			{
				return null;
			}
			if (length == 0)
			{
				return new T[0];
			}
			T[] array = new T[Math.Min(value.Length, length)];
			Array.Copy(value, index, array, 0, array.Length);
			return array;
		}

		/// <summary>
		/// 选择一个数组的前面的几个数据信息
		/// </summary>
		/// <param name="value">数组</param>
		/// <param name="length">数据的长度</param>
		/// <returns>新的数组</returns>
		public static T[] ArraySelectBegin<T>(T[] value, int length)
		{
			if (length == 0)
			{
				return new T[0];
			}
			T[] array = new T[Math.Min(value.Length, length)];
			if (array.Length != 0)
			{
				Array.Copy(value, 0, array, 0, array.Length);
			}
			return array;
		}

		/// <summary>
		/// 选择一个数组的后面的几个数据信息
		/// </summary>
		/// <param name="value">数组</param>
		/// <param name="length">数据的长度</param>
		/// <returns>新的数组信息</returns>
		public static T[] ArraySelectLast<T>(T[] value, int length)
		{
			if (length == 0)
			{
				return new T[0];
			}
			T[] array = new T[Math.Min(value.Length, length)];
			Array.Copy(value, value.Length - length, array, 0, array.Length);
			return array;
		}

		/// <summary>
		/// 拼接任意个泛型数组为一个总的泛型数组对象，采用深度拷贝实现。
		/// </summary>
		/// <typeparam name="T">数组的类型信息</typeparam>
		/// <param name="arrays">任意个长度的数组</param>
		/// <returns>拼接之后的最终的结果对象</returns>
		public static T[] SpliceArray<T>(params T[][] arrays)
		{
			int num = 0;
			for (int i = 0; i < arrays.Length; i++)
			{
				T[] array = arrays[i];
				if (array != null && array.Length != 0)
				{
					num += arrays[i].Length;
				}
			}
			int num2 = 0;
			T[] array2 = new T[num];
			for (int j = 0; j < arrays.Length; j++)
			{
				T[] array3 = arrays[j];
				if (array3 != null && array3.Length != 0)
				{
					arrays[j].CopyTo(array2, num2);
					num2 += arrays[j].Length;
				}
			}
			return array2;
		}

		/// <summary>
		/// 将一个<see cref="T:System.String" />的数组和多个<see cref="T:System.String" /> 类型的对象整合成一个数组
		/// </summary>
		/// <param name="first">第一个数组对象</param>
		/// <param name="array">字符串数组信息</param>
		/// <returns>总的数组对象</returns>
		public static string[] SpliceStringArray(string first, string[] array)
		{
			List<string> list = new List<string>();
			list.Add(first);
			list.AddRange(array);
			return list.ToArray();
		}

		/// <summary>
		/// 将两个<see cref="T:System.String" />的数组和多个<see cref="T:System.String" /> 类型的对象整合成一个数组
		/// </summary>
		/// <param name="first">第一个数据对象</param>
		/// <param name="second">第二个数据对象</param>
		/// <param name="array">字符串数组信息</param>
		/// <returns>总的数组对象</returns>
		public static string[] SpliceStringArray(string first, string second, string[] array)
		{
			List<string> list = new List<string>();
			list.Add(first);
			list.Add(second);
			list.AddRange(array);
			return list.ToArray();
		}

		/// <summary>
		/// 将两个<see cref="T:System.String" />的数组和多个<see cref="T:System.String" /> 类型的对象整合成一个数组
		/// </summary>
		/// <param name="first">第一个数据对象</param>
		/// <param name="second">第二个数据对象</param>
		/// <param name="third">第三个数据对象</param>
		/// <param name="array">字符串数组信息</param>
		/// <returns>总的数组对象</returns>
		public static string[] SpliceStringArray(string first, string second, string third, string[] array)
		{
			List<string> list = new List<string>();
			list.Add(first);
			list.Add(second);
			list.Add(third);
			list.AddRange(array);
			return list.ToArray();
		}

		private static int HexToInt(char h)
		{
			if (h < '0' || h > '9')
			{
				if (h < 'a' || h > 'f')
				{
					if (h < 'A' || h > 'F')
					{
						return -1;
					}
					return h - 65 + 10;
				}
				return h - 97 + 10;
			}
			return h - 48;
		}

		private static string ValidateString(string input, bool skipUtf16Validation)
		{
			if (skipUtf16Validation || string.IsNullOrEmpty(input))
			{
				return input;
			}
			int num = -1;
			for (int i = 0; i < input.Length; i++)
			{
				if (char.IsSurrogate(input[i]))
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				return input;
			}
			char[] array = input.ToCharArray();
			for (int j = num; j < array.Length; j++)
			{
				char c = array[j];
				if (char.IsLowSurrogate(c))
				{
					array[j] = '\ufffd';
				}
				else if (char.IsHighSurrogate(c))
				{
					if (j + 1 < array.Length && char.IsLowSurrogate(array[j + 1]))
					{
						j++;
					}
					else
					{
						array[j] = '\ufffd';
					}
				}
			}
			return new string(array);
		}

		private static bool ValidateUrlEncodingParameters(byte[] bytes, int offset, int count)
		{
			if (bytes == null && count == 0)
			{
				return false;
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (offset < 0 || offset > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || offset + count > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			return true;
		}

		private static bool IsUrlSafeChar(char ch)
		{
			if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
			{
				return true;
			}
			switch (ch)
			{
			case '!':
			case '(':
			case ')':
			case '*':
			case '-':
			case '.':
			case '_':
				return true;
			default:
				return false;
			}
		}

		private static string UrlEncodeSpaces(string str)
		{
			if (str != null && str.IndexOf(' ') >= 0)
			{
				str = str.Replace(" ", "%20");
			}
			return str;
		}

		private static char IntToHex(int n)
		{
			if (n <= 9)
			{
				return (char)(n + 48);
			}
			return (char)(n - 10 + 65);
		}

		private static byte[] UrlEncodeToBytes(byte[] bytes)
		{
			int num = 0;
			int num2 = bytes.Length;
			if (!ValidateUrlEncodingParameters(bytes, num, num2))
			{
				return null;
			}
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < num2; i++)
			{
				char c = (char)bytes[num + i];
				if (c == ' ')
				{
					num3++;
				}
				else if (!IsUrlSafeChar(c))
				{
					num4++;
				}
			}
			if (num3 == 0 && num4 == 0)
			{
				if (num == 0 && bytes.Length == num2)
				{
					return bytes;
				}
				byte[] array = new byte[num2];
				Buffer.BlockCopy(bytes, num, array, 0, num2);
				return array;
			}
			byte[] array2 = new byte[num2 + num4 * 2];
			int num5 = 0;
			for (int j = 0; j < num2; j++)
			{
				byte b = bytes[num + j];
				char c2 = (char)b;
				if (IsUrlSafeChar(c2))
				{
					array2[num5++] = b;
					continue;
				}
				if (c2 == ' ')
				{
					array2[num5++] = 43;
					continue;
				}
				array2[num5++] = 37;
				array2[num5++] = (byte)IntToHex((b >> 4) & 0xF);
				array2[num5++] = (byte)IntToHex(b & 0xF);
			}
			return array2;
		}

		private static byte[] UrlEncode(byte[] bytes, bool alwaysCreateNewReturnValue)
		{
			byte[] array = UrlEncodeToBytes(bytes);
			if (!alwaysCreateNewReturnValue || array == null || array != bytes)
			{
				return array;
			}
			return (byte[])array.Clone();
		}

		/// <summary>
		/// 将字符串编码为URL可以识别的字符串，中文会被编码为%开头的数据，例如 中文 转义为 %2F%E4%B8%AD%E6%96%87
		/// </summary>
		/// <param name="str">等待转换的字符串数据</param>
		/// <param name="e">编码信息，一般为 UTF8 </param>
		/// <returns>编码之后的结果</returns>
		public static string UrlEncode(string str, Encoding e)
		{
			if (str == null)
			{
				return null;
			}
			byte[] bytes = e.GetBytes(str);
			return Encoding.ASCII.GetString(UrlEncode(bytes, alwaysCreateNewReturnValue: true));
		}

		/// <summary>
		/// 将一维数组转换为二维数组
		/// </summary>
		/// <typeparam name="T">数组元素类型</typeparam>
		/// <param name="array">一维源数组</param>
		/// <param name="row">行数</param>
		/// <param name="col">列数</param>
		/// <returns>
		/// 指定行列的二维数组
		/// </returns>
		/// <exception cref="T:System.ArgumentException">
		/// 如果 row × col 不等于数组长度，将可能抛出索引异常
		/// </exception>
		public static T[,] CreateTwoArrayFromOneArray<T>(T[] array, int row, int col)
		{
			T[,] array2 = new T[row, col];
			int num = 0;
			for (int i = 0; i < row; i++)
			{
				for (int j = 0; j < col; j++)
				{
					array2[i, j] = array[num];
					num++;
				}
			}
			return array2;
		}

		/// <summary>
		/// 计算CRC16循环校验码
		/// ① 支持按位异或校验（XOR）
		/// ② 支持CRC16查表法校验
		/// ③ 支持CRC16带多项式计算法校验
		/// </summary>
		/// <param name="cmd">字节数据</param>
		/// <param name="isHighBefore">是否高位在前，默认false</param>
		/// <returns></returns>
		public static byte[] GetCRC16(byte[] cmd, bool isHighBefore = false)
		{
			int num = 255;
			int num2 = 255;
			for (int i = 0; i < cmd.Length; i++)
			{
				int num3 = num2 ^ cmd[i];
				num2 = num ^ cRCHighArray[num3];
				num = cRCLowArray[num3];
			}
			if (!isHighBefore)
			{
				return new byte[2]
				{
					(byte)num,
					(byte)num2
				};
			}
			return new byte[2]
			{
				(byte)num2,
				(byte)num
			};
		}
	}
	/// <summary>
	/// 扩展方法
	/// </summary>
	public static class ExtendMethod
	{
		/// <summary>
		/// 枚举记录实体信息
		/// </summary>
		public class EnumberEntity
		{
			/// <summary>
			/// 枚举名称
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			/// 枚举对象的值
			/// </summary>
			public int Value { get; set; }

			/// <summary>
			/// 枚举的描述
			/// </summary>
			public string Describe { get; set; }
		}

		/// <summary>
		/// 默认结果
		/// </summary>
		private static readonly (bool result, List<Difference>? difference) defaultResult;

		/// <summary>
		/// json序列化默认初始化数据
		/// </summary>
		private static readonly JsonSerializerSettings _serializerSettings;

		/// <summary>
		/// json序列化默认初始化数据
		/// </summary>
		private static readonly JsonSerializerOptions _serializerOptions_formatting_false;

		/// <summary>
		/// json序列化默认初始化数据
		/// </summary>
		private static readonly JsonSerializerOptions _serializerOptions_formatting_true;

		/// <summary>
		/// 判断当前系统是否是小端序
		/// </summary>
		public static bool IsLittleEndian => BitConverter.IsLittleEndian;

		/// <summary>
		/// 验证字符串是否匹配正则表达式描述的规则
		/// </summary>
		/// <param name="inputStr">待验证的字符串</param>
		/// <param name="patternStr">正则表达式字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsMatch(this string inputStr, string patternStr)
		{
			return inputStr.IsMatch(patternStr, ifIgnoreCase: false, ifValidateWhiteSpace: false);
		}

		/// <summary>
		/// 验证字符串是否匹配正则表达式描述的规则
		/// </summary>
		/// <param name="inputStr">待验证的字符串</param>
		/// <param name="patternStr">正则表达式字符串</param>
		/// <param name="ifIgnoreCase">匹配时是否不区分大小写</param>
		/// <returns>是否匹配</returns>
		public static bool IsMatch(this string inputStr, string patternStr, bool ifIgnoreCase)
		{
			return inputStr.IsMatch(patternStr, ifIgnoreCase, ifValidateWhiteSpace: false);
		}

		/// <summary>
		/// 验证字符串是否匹配正则表达式描述的规则
		/// </summary>
		/// <param name="inputStr">待验证的字符串</param>
		/// <param name="patternStr">正则表达式字符串</param>
		/// <param name="ifValidateWhiteSpace">是否验证空白字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsMatchs(this string inputStr, string patternStr, bool ifValidateWhiteSpace)
		{
			return inputStr.IsMatch(patternStr, ifIgnoreCase: false, ifValidateWhiteSpace);
		}

		/// <summary>
		/// 验证字符串是否匹配正则表达式描述的规则
		/// </summary>
		/// <param name="inputStr">待验证的字符串</param>
		/// <param name="patternStr">正则表达式字符串</param>
		/// <param name="ifIgnoreCase">匹配时是否不区分大小写</param>
		/// <param name="ifValidateWhiteSpace">是否验证空白字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsMatch(this string inputStr, string patternStr, bool ifIgnoreCase, bool ifValidateWhiteSpace)
		{
			if (!ifValidateWhiteSpace && string.IsNullOrWhiteSpace(inputStr))
			{
				return false;
			}
			Regex regex = null;
			regex = ((!ifIgnoreCase) ? new Regex(patternStr) : new Regex(patternStr, RegexOptions.IgnoreCase));
			return regex.IsMatch(inputStr);
		}

		/// <summary>
		/// 验证数字(double类型)
		/// [可以包含负号和小数点]
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsNumber(this string input)
		{
			double result = 0.0;
			if (double.TryParse(input, out result))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 验证整数
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsInteger(this string input)
		{
			int result = 0;
			if (int.TryParse(input, out result))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 验证非负整数
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsIntegerNotNagtive(this string input)
		{
			int result = -1;
			if (int.TryParse(input, out result) && result >= 0)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 验证正整数
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsIntegerPositive(this string input)
		{
			int result = 0;
			if (int.TryParse(input, out result) && result >= 1)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 验证只包含英文字母
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsEnglishCharacter(this string input)
		{
			string patternStr = "^[A-Za-z]+$";
			return input.IsMatch(patternStr);
		}

		/// <summary>
		/// 验证只包含数字和英文字母
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsIntegerAndEnglishCharacter(this string input)
		{
			string patternStr = "^[0-9A-Za-z]+$";
			return input.IsMatch(patternStr);
		}

		/// <summary>
		/// 验证只包含汉字
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsChineseCharacter(this string input)
		{
			string patternStr = "^[\\u4e00-\\u9fa5]+$";
			return input.IsMatch(patternStr);
		}

		/// <summary>
		/// 验证数字长度范围（数字前端的0计长度）
		/// [若要验证固定长度，可传入相同的两个长度数值]
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <param name="lengthBegin">长度范围起始值（含）</param>
		/// <param name="lengthEnd">长度范围结束值（含）</param>
		/// <returns>是否匹配</returns>
		public static bool IsIntegerLength(this string input, int lengthBegin, int lengthEnd)
		{
			if (input.Length >= lengthBegin && input.Length <= lengthEnd)
			{
				if (int.TryParse(input, out var _))
				{
					return true;
				}
				return false;
			}
			return false;
		}

		/// <summary>
		/// 验证字符串包含内容
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <param name="withEnglishCharacter">是否包含英文字母</param>
		/// <param name="withNumber">是否包含数字</param>
		/// <param name="withChineseCharacter">是否包含汉字</param>
		/// <returns>是否匹配</returns>
		public static bool IsStringInclude(this string input, bool withEnglishCharacter, bool withNumber, bool withChineseCharacter)
		{
			if (!withEnglishCharacter && !withNumber && !withChineseCharacter)
			{
				return false;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("^[");
			if (withEnglishCharacter)
			{
				stringBuilder.Append("a-zA-Z");
			}
			if (withNumber)
			{
				stringBuilder.Append("0-9");
			}
			if (withChineseCharacter)
			{
				stringBuilder.Append("\\u4E00-\\u9FA5");
			}
			stringBuilder.Append("]+$");
			return input.IsMatch(stringBuilder.ToString());
		}

		/// <summary>
		/// 验证字符串长度范围
		/// [若要验证固定长度，可传入相同的两个长度数值]
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <param name="lengthBegin">长度范围起始值（含）</param>
		/// <param name="lengthEnd">长度范围结束值（含）</param>
		/// <returns>是否匹配</returns>
		public static bool IsStringLength(this string input, int lengthBegin, int lengthEnd)
		{
			if (input.Length >= lengthBegin && input.Length <= lengthEnd)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 验证字符串长度范围（字符串内只包含数字和/或英文字母）
		/// [若要验证固定长度，可传入相同的两个长度数值]
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <param name="lengthBegin">长度范围起始值（含）</param>
		/// <param name="lengthEnd">长度范围结束值（含）</param>
		/// <returns>是否匹配</returns>
		public static bool IsStringLengthOnlyNumberAndEnglishCharacter(this string input, int lengthBegin, int lengthEnd)
		{
			string patternStr = "^[0-9a-zA-z]{" + lengthBegin + "," + lengthEnd + "}$";
			return input.IsMatch(patternStr);
		}

		/// <summary>
		/// 验证字符串长度范围
		/// [若要验证固定长度，可传入相同的两个长度数值]
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <param name="withEnglishCharacter">是否包含英文字母</param>
		/// <param name="withNumber">是否包含数字</param>
		/// <param name="withChineseCharacter">是否包含汉字</param>
		/// <param name="lengthBegin">长度范围起始值（含）</param>
		/// <param name="lengthEnd">长度范围结束值（含）</param>
		/// <returns>是否匹配</returns>
		public static bool IsStringLengthByInclude(this string input, bool withEnglishCharacter, bool withNumber, bool withChineseCharacter, int lengthBegin, int lengthEnd)
		{
			if (!withEnglishCharacter && !withNumber && !withChineseCharacter)
			{
				return false;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("^[");
			if (withEnglishCharacter)
			{
				stringBuilder.Append("a-zA-Z");
			}
			if (withNumber)
			{
				stringBuilder.Append("0-9");
			}
			if (withChineseCharacter)
			{
				stringBuilder.Append("\\u4E00-\\u9FA5");
			}
			stringBuilder.Append("]{" + lengthBegin + "," + lengthEnd + "}$");
			return input.IsMatch(stringBuilder.ToString());
		}

		/// <summary>
		/// 验证字符串字节数长度范围
		/// [若要验证固定长度，可传入相同的两个长度数值；每个汉字为两个字节长度]
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <param name="lengthBegin">长度范围起始值（含）</param>
		/// <param name="lengthEnd">长度范围结束值（含）</param>
		/// <returns></returns>
		public static bool IsStringByteLength(this string input, int lengthBegin, int lengthEnd)
		{
			int byteCount = Encoding.Default.GetByteCount(input);
			if (byteCount >= lengthBegin && byteCount <= lengthEnd)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 验证固定电话号码
		/// [3位或4位区号；区号可以用小括号括起来；区号可以省略；区号与本地号间可以用减号或空格隔开；可以有3位数的分机号，分机号前要加减号]
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsTelePhoneNumber(this string input)
		{
			string patternStr = "^(((0\\d2|0\\d{2})[- ]?)?\\d{8}|((0\\d3|0\\d{3})[- ]?)?\\d{7})(-\\d{3})?$";
			return input.IsMatch(patternStr);
		}

		/// <summary>
		/// 验证手机号码
		/// [可匹配"(+86)013325656352"，括号可以省略，+号可以省略，(+86)可以省略，11位手机号前的0可以省略；11位手机号第二位数可以是3、4、5、8中的任意一个]
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsMobilePhoneNumber(this string input)
		{
			string patternStr = "^((\\+)?86|((\\+)?86)?)0?1[3458]\\d{9}$";
			return input.IsMatch(patternStr);
		}

		/// <summary>
		/// 验证电话号码（可以是固定电话号码或手机号码）
		/// [固定电话：[3位或4位区号；区号可以用小括号括起来；区号可以省略；区号与本地号间可以用减号或空格隔开；可以有3位数的分机号，分机号前要加减号]]
		/// [手机号码：[可匹配"(+86)013325656352"，括号可以省略，+号可以省略，(+86)可以省略，手机号前的0可以省略；手机号第二位数可以是3、4、5、8中的任意一个]]
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsPhoneNumber(this string input)
		{
			string patternStr = "^((\\+)?86|((\\+)?86)?)0?1[3458]\\d{9}$|^(((0\\d2|0\\d{2})[- ]?)?\\d{8}|((0\\d3|0\\d{3})[- ]?)?\\d{7})(-\\d{3})?$";
			return input.IsMatch(patternStr);
		}

		/// <summary>
		/// 验证邮政编码
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsZipCode(this string input)
		{
			if (input.Length != 6)
			{
				return false;
			}
			if (int.TryParse(input, out var _))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 验证电子邮箱
		/// [@字符前可以包含字母、数字、下划线和点号；@字符后可以包含字母、数字、下划线和点号；@字符后至少包含一个点号且点号不能是最后一个字符；最后一个点号后只能是字母或数字]
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsEmail(this string input)
		{
			string patternStr = "^([\\w-\\.]+)@([\\w-\\.]+)(\\.[a-zA-Z0-9]+)$";
			return input.IsMatch(patternStr);
		}

		/// <summary>
		/// 验证网址（可以匹配IPv4地址但没对IPv4地址进行格式验证；IPv6暂时没做匹配）
		/// [允许省略"://"；可以添加端口号；允许层级；允许传参；域名中至少一个点号且此点号前要有内容]
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsURL(this string input)
		{
			string patternStr = "^([a-zA-Z]+://)?([\\w-\\.]+)(\\.[a-zA-Z0-9]+)(:\\d{0,5})?/?([\\w-/]*)\\.?([a-zA-Z]*)\\??(([\\w-]*=[\\w%]*&?)*)$";
			return input.IsMatch(patternStr);
		}

		/// <summary>
		/// 验证IPv4地址
		/// [第一位和最后一位数字不能是0或255；允许用0补位]
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsIPv4(this string input)
		{
			string[] array = input.Split('.');
			if (array.Length != 4)
			{
				return false;
			}
			int result = -1;
			for (int i = 0; i < array.Length; i++)
			{
				if (i == 0 || i == 3)
				{
					if (!int.TryParse(array[i], out result) || result <= 0 || result >= 255)
					{
						return false;
					}
				}
				else if (!int.TryParse(array[i], out result) || result < 0 || result > 255)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 验证IPv6地址
		/// [可用于匹配任何一个合法的IPv6地址]
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsIPv6(this string input)
		{
			string patternStr = "^\\s*((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:)))(%.+)?\\s*$";
			return input.IsMatch(patternStr);
		}

		/// <summary>
		/// 验证一代身份证号（15位数）
		/// [长度为15位的数字；匹配对应省份地址；生日能正确匹配]
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsIDCard15(this string input)
		{
			long result = 0L;
			if (!long.TryParse(input, out result) || result.ToString().Length != 15)
			{
				return false;
			}
			if (!"11,12,13,14,15,21,22,23,31,32,33,34,35,36,37,41,42,43,44,45,46,50,51,52,53,54,61,62,63,64,65,71,81,82,91,".Contains(input.Remove(2) + ","))
			{
				return false;
			}
			if (!DateTime.TryParse(input.Substring(6, 6).Insert(4, "/").Insert(2, "/"), out var _))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// 验证二代身份证号（18位数，GB11643-1999标准）
		/// [长度为18位；前17位为数字，最后一位(校验码)可以为大小写x；匹配对应省份地址；生日能正确匹配；校验码能正确匹配]
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsIDCard18(this string input)
		{
			long result = 0L;
			if (!long.TryParse(input.Remove(17), out result) || result.ToString().Length != 17 || !long.TryParse(input.Replace('x', '0').Replace('X', '0'), out result))
			{
				return false;
			}
			if (!"11,12,13,14,15,21,22,23,31,32,33,34,35,36,37,41,42,43,44,45,46,50,51,52,53,54,61,62,63,64,65,71,81,82,91,".Contains(input.Remove(2) + ","))
			{
				return false;
			}
			if (!DateTime.TryParse(input.Substring(6, 8).Insert(6, "/").Insert(4, "/"), out var _))
			{
				return false;
			}
			string[] array = "1,0,x,9,8,7,6,5,4,3,2".Split(',');
			string[] array2 = "7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2".Split(',');
			char[] array3 = input.Remove(17).ToCharArray();
			int num = 0;
			for (int i = 0; i < 17; i++)
			{
				num += int.Parse(array2[i]) * int.Parse(array3[i].ToString());
			}
			int result3 = -1;
			Math.DivRem(num, 11, out result3);
			if (array[result3] != input.Substring(17, 1).ToLower())
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// 验证身份证号（不区分一二代身份证号）
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsIDCard(this string input)
		{
			if (input.Length == 18)
			{
				return input.IsIDCard18();
			}
			if (input.Length == 15)
			{
				return input.IsIDCard15();
			}
			return false;
		}

		/// <summary>
		/// 验证经度
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsLongitude(this string input)
		{
			if (float.TryParse(input, out var result) && result >= -180f && result <= 180f)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 验证纬度
		/// </summary>
		/// <param name="input">待验证的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool IsLatitude(this string input)
		{
			if (float.TryParse(input, out var result) && result >= -90f && result <= 90f)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 判断是否为合法的16进制字符串
		/// </summary>
		/// <param name="hexString"></param>
		/// <returns></returns>
		public static bool IsHexadecimal(this string hexString)
		{
			try
			{
				hexString = hexString.Replace(" ", "").Replace("\r", "").Replace("\n", "");
				if (hexString.Length % 2 != 0)
				{
					hexString += " ";
				}
				byte[] array = new byte[hexString.Length / 2];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// 判断为空GUID
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public static bool IsEmptyGuid(this Guid guid)
		{
			return guid == Guid.Empty;
		}

		/// <summary>
		/// 判断为空GUID
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public static bool IsEmptyGuid(this Guid? guid)
		{
			if (guid.HasValue)
			{
				return guid.Value == Guid.Empty;
			}
			return true;
		}

		/// <summary>
		/// 判断不为空GUID
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public static bool IsNotEmptyGuid(this Guid guid)
		{
			return guid != Guid.Empty;
		}

		/// <summary>
		/// 判断不为NULL和空GUID
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public static bool IsNotEmptyGuid(this Guid? guid)
		{
			if (guid.HasValue)
			{
				return guid.Value != Guid.Empty;
			}
			return false;
		}

		/// <summary>
		/// 判断字符串是否为null或""
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

		/// <summary>
		/// 判断字符串是否为null或""或" "(包含空字符的字符串)
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsNullOrWhiteSpace(this string str)
		{
			return string.IsNullOrWhiteSpace(str);
		}

		/// <summary>
		/// 判断一个字符串是否是GUID
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsGuid(this string str)
		{
			Guid result;
			return Guid.TryParse(str.Trims(), out result);
		}

		/// <summary>
		/// 判断一个字符串是否是GUID
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsGuid(this string str, out Guid guid)
		{
			return Guid.TryParse(str.Trims(), out guid);
		}

		/// <summary>
		/// 判断一个字符串是否是字体图标(以fa开头)
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsFontIco(this string str)
		{
			return str.Trims().StartsWith("fa");
		}

		/// <summary>
		/// 判断字符串是否为整数
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsInt(this string str)
		{
			int result;
			return int.TryParse(str, out result);
		}

		/// <summary>
		/// 判断字符串是否为整数
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsInt(this string str, out int i)
		{
			return int.TryParse(str, out i);
		}

		/// <summary>
		/// 判断字符串是否是长整型
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsLong(this string str)
		{
			long result;
			return long.TryParse(str, out result);
		}

		/// <summary>
		/// 判断字符串是否是长整型
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsLong(this string str, out long l)
		{
			return long.TryParse(str, out l);
		}

		/// <summary>
		/// 判断字符串是否为数字
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsDecimal(this string str)
		{
			decimal result;
			return decimal.TryParse(str, out result);
		}

		/// <summary>
		/// 判断字符串是否为数字
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsDecimal(this string str, out decimal d)
		{
			return decimal.TryParse(str, out d);
		}

		/// <summary>
		/// 判断字符串是否为日期时间
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsDateTime(this string str)
		{
			DateTime result;
			return DateTime.TryParse(str, out result);
		}

		/// <summary>
		/// 判断字符串是否为日期时间
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsDateTime(this string str, out DateTime dt)
		{
			return DateTime.TryParse(str, out dt);
		}

		/// <summary>
		/// 验证字符串是否为数字；<br />
		/// 遍历字符串中的每个字符，判断是否全部为数字
		/// </summary>
		/// <param name="str">待验证字符串</param>
		/// <returns>true：全部为数字；false：含非数字字符</returns>
		public static bool IsDigital(this string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				if (!char.IsDigit(str[i]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 验证是否为固话号码
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsTelNumber(this string str)
		{
			if (!str.IsNullOrWhiteSpace() && !str.StartsWith("-"))
			{
				return str.Replace("-", "").IsDigital();
			}
			return false;
		}

		/// <summary>
		/// 是不是为空
		/// </summary>
		/// <param name="value">字符串</param>
		/// <returns></returns>
		public static bool IsEmpty(this string value)
		{
			return string.IsNullOrWhiteSpace(value);
		}

		public static bool IsNull(this string value)
		{
			return value == null;
		}

		public static bool IsEmpty(this Guid? value)
		{
			if (!value.HasValue)
			{
				return true;
			}
			return value.Value.IsEmpty();
		}

		public static bool IsEmpty(this Guid value)
		{
			if (value == Guid.Empty)
			{
				return true;
			}
			return false;
		}

		public static bool IsEmpty(this DateTime? value)
		{
			if (!value.HasValue)
			{
				return true;
			}
			return value.Value.IsEmpty();
		}

		public static bool IsEmpty(this DateTime value)
		{
			if (value == DateTime.MinValue)
			{
				return true;
			}
			return false;
		}

		public static bool IsEmpty(this object value)
		{
			if (value != null && !string.IsNullOrEmpty(value.ToString()))
			{
				return false;
			}
			return true;
		}

		public static bool IsNum(this string value)
		{
			return Regex.IsMatch(value, "^\\d+(\\.\\d*)?$");
		}

		/// <summary>
		/// 判断是否为有效的 JSON 数据
		/// </summary>
		/// <param name="input">Json 字符串</param>
		/// <returns></returns>
		public static bool IsJson(this string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return false;
			}
			ReadOnlySpan<char> readOnlySpan = input.AsSpan().Trim();
			if (!IsPotentialJsonStructure(readOnlySpan))
			{
				return false;
			}
			int byteCount = Encoding.UTF8.GetByteCount(readOnlySpan);
			byte[] array = null;
			Span<byte> span = ((byteCount > 1024) ? ((Span<byte>)(array = ArrayPool<byte>.Shared.Rent(byteCount))) : stackalloc byte[byteCount]);
			Span<byte> bytes = span;
			try
			{
				return bytes[..Encoding.UTF8.GetBytes(readOnlySpan, bytes)].IsJson();
			}
			finally
			{
				if (array != null)
				{
					ArrayPool<byte>.Shared.Return(array);
				}
			}
		}

		/// <summary>
		/// 判断是否为有效的 JSON 数据
		/// </summary>
		/// <returns></returns>
		public static bool IsJson(this ReadOnlySpan<byte> utf8Bytes)
		{
			try
			{
				Utf8JsonReader reader = new Utf8JsonReader(utf8Bytes, isFinalBlock: true, default(JsonReaderState));
				using (JsonDocument.ParseValue(ref reader))
				{
					return reader.BytesConsumed == utf8Bytes.Length;
				}
			}
			catch (JsonException)
			{
				return false;
			}
		}

		/// <summary>
		/// 判断是否为有效的 JSON 数据
		/// </summary>
		/// <returns></returns>
		public static bool IsJson(this ReadOnlySequence<byte> sequence)
		{
			if (sequence.IsSingleSegment)
			{
				return sequence.FirstSpan.IsJson();
			}
			try
			{
				Utf8JsonReader reader = new Utf8JsonReader(sequence, isFinalBlock: true, default(JsonReaderState));
				using (JsonDocument.ParseValue(ref reader))
				{
					return reader.BytesConsumed == sequence.Length;
				}
			}
			catch (JsonException)
			{
				return false;
			}
		}

		/// <summary>
		/// 核心结构检测
		/// </summary>
		/// <returns></returns>
		private static bool IsPotentialJsonStructure(ReadOnlySpan<char> span)
		{
			if (span.Length < 2)
			{
				return false;
			}
			char c = span[0];
			char c2 = span[span.Length - 1];
			if (c != '"')
			{
				if (c != '[')
				{
					if (c == '{' && c2 == '}')
					{
						goto IL_004b;
					}
				}
				else if (c2 == ']')
				{
					goto IL_004b;
				}
			}
			else if (c2 == '"')
			{
				return true;
			}
			return false;
			IL_004b:
			return span.Length > 2;
		}

		/// <summary>
		/// 将枚举转换为List,便于界面绑定
		/// </summary>
		/// <typeparam name="T">枚举类型</typeparam>
		/// <returns>枚举转换后的List列表信息</returns>
		public static List<EnumberEntity> EnumToList<T>(this T @enum)
		{
			List<EnumberEntity> list = new List<EnumberEntity>();
			foreach (object value in Enum.GetValues(typeof(T)))
			{
				EnumberEntity enumberEntity = new EnumberEntity();
				object[] customAttributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), inherit: true);
				if (customAttributes != null && customAttributes.Length != 0)
				{
					DescriptionAttribute descriptionAttribute = customAttributes[0] as DescriptionAttribute;
					enumberEntity.Describe = descriptionAttribute.Description;
				}
				enumberEntity.Value = Convert.ToInt32(value);
				enumberEntity.Name = value.ToString();
				list.Add(enumberEntity);
			}
			return list;
		}

		/// <summary>
		/// get all information of enum,include value,name and description
		/// </summary>
		/// <param name="enumName">the type of enumName</param>
		/// <returns></returns>
		public static List<dynamic> GetAllItems(this Type enumName)
		{
			List<object> list = new List<object>();
			FieldInfo[] fields = enumName.GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.FieldType.IsEnum)
				{
					int num = (int)enumName.InvokeMember(fieldInfo.Name, BindingFlags.GetField, null, null, null);
					string name = fieldInfo.Name;
					string empty = string.Empty;
					object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
					empty = ((customAttributes.Length == 0) ? "" : ((DescriptionAttribute)customAttributes[0]).Description);
					dynamic val = new ExpandoObject();
					val.Value = num;
					val.Name = name;
					val.Describe = empty;
					list.Add(val);
				}
			}
			return list;
		}

		/// <summary>
		/// 将枚举转换为List,便于界面绑定
		/// </summary>
		/// <returns>枚举转换后的List列表信息</returns>
		public static List<EnumberEntity> EnumToList(this Type @enum)
		{
			List<EnumberEntity> list = new List<EnumberEntity>();
			foreach (object value in Enum.GetValues(@enum))
			{
				EnumberEntity enumberEntity = new EnumberEntity();
				object[] customAttributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), inherit: true);
				if (customAttributes != null && customAttributes.Length != 0)
				{
					DescriptionAttribute descriptionAttribute = customAttributes[0] as DescriptionAttribute;
					enumberEntity.Describe = descriptionAttribute.Description;
				}
				enumberEntity.Value = Convert.ToInt32(value);
				enumberEntity.Name = value.ToString();
				list.Add(enumberEntity);
			}
			return list;
		}

		/// <summary>
		/// 获取成员元数据的Description特性描述信息
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		/// <exception cref="T:System.ArgumentNullException"></exception>
		/// <exception cref="T:System.ArgumentException"></exception>
		public static string GetDescription(this object obj, string propertyName)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			PropertyInfo? property = obj.GetType().GetProperty(propertyName);
			if (property == null)
			{
				throw new ArgumentException("Property '" + propertyName + "' not found.");
			}
			return property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? propertyName;
		}

		/// <summary>
		/// 获取成员元数据的Description特性描述信息
		/// </summary>
		/// <param name="member">成员元数据对象</param>
		/// <param name="inherit">是否搜索成员的继承链以查找描述特性</param>
		/// <returns>返回Description特性描述信息，如不存在则返回成员的名称</returns>
		public static string GetDescription(this MemberInfo member, bool inherit = false)
		{
			DescriptionAttribute attribute = member.GetAttribute<DescriptionAttribute>(inherit);
			if (attribute != null)
			{
				return attribute.Description;
			}
			DisplayNameAttribute attribute2 = member.GetAttribute<DisplayNameAttribute>(inherit);
			if (attribute2 != null)
			{
				return attribute2.DisplayName;
			}
			return member.Name;
		}

		/// <summary>
		/// 获取枚举项上的<see cref="T:System.ComponentModel.DescriptionAttribute" />特性的文字描述
		/// </summary>
		/// <param name="value">Enum</param>
		/// <returns>string</returns>
		public static string ToDescription(this object value)
		{
			MemberInfo memberInfo = value.GetType().GetMember(value.ToString()).FirstOrDefault();
			if (!(memberInfo != null))
			{
				return value.ToString();
			}
			return memberInfo.GetDescription();
		}

		/// <summary>
		/// 从类型成员获取指定Attribute特性
		/// </summary>
		/// <typeparam name="T">Attribute特性类型</typeparam>
		/// <param name="memberInfo">类型类型成员</param>
		/// <param name="inherit">是否从继承中查找</param>
		/// <returns>存在返回第一个，不存在返回null</returns>
		public static T GetAttribute<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
		{
			return memberInfo.GetCustomAttributes(typeof(T), inherit).FirstOrDefault() as T;
		}

		/// <summary>
		/// 通过枚举值，返回枚举对象
		/// </summary>
		/// <typeparam name="T">枚举</typeparam>
		/// <param name="value">值</param>
		/// <returns></returns>
		public static T EnumValueToEnumObj<T>(this int value)
		{
			return Enum.GetName(typeof(T), value).EnumKeyToEnumObj<T>();
		}

		/// <summary>
		/// 通过枚举值，返回枚举对象
		/// </summary>
		/// <typeparam name="T">枚举</typeparam>
		/// <param name="value">值</param>
		/// <returns></returns>
		public static T EnumValueToEnumObj<T>(this byte value)
		{
			return Enum.GetName(typeof(T), value).EnumKeyToEnumObj<T>();
		}

		/// <summary>
		/// 通过枚举名称返回枚举对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Key"></param>
		/// <returns></returns>
		public static T EnumKeyToEnumObj<T>(this string Key)
		{
			return (T)Enum.Parse(typeof(T), Key);
		}

		/// <summary>
		/// 微秒级延时（自旋等待）；<br />
		/// 使用 Stopwatch 高精度计时，适用于对延时精度要求较高的场景；<br />
		/// 注意：此方法会占用 CPU，不适合长时间延时
		/// </summary>
		/// <param name="time">延时时间，单位：ms（毫秒）</param>
		public static void DelayUs(this double time)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			while (stopwatch.Elapsed.TotalMilliseconds < time)
			{
			}
			stopwatch.Stop();
		}

		/// <summary>
		/// 微秒级延时（自旋等待）；<br />
		/// 将 int 参数转为 double 后委托给 double 版本执行
		/// </summary>
		/// <param name="time">延时时间，单位：ms（毫秒）</param>
		public static void DelayUs(this int time)
		{
			((double)time).DelayUs();
		}

		/// <summary>
		/// 对象比对器（线程安全）
		/// </summary>
		/// <typeparam name="T">泛型对象类型</typeparam>
		/// <param name="a">对象 A</param>
		/// <param name="b">对象 B</param>
		/// <param name="propertyNames">可选：指定要忽略比较的属性名称数组</param>
		/// <returns>(是否一致, 差异信息列表)</returns>
		public static (bool result, List<Difference>? difference) Comparer<T>(this T a, T b, string[]? propertyNames = null)
		{
			ComparisonConfig comparisonConfig = new ComparisonConfig
			{
				UseHashCodeIdentifier = true,
				IgnoreObjectTypes = true,
				CompareChildren = true
			};
			if (propertyNames != null && propertyNames.Length != 0)
			{
				comparisonConfig.MembersToIgnore.AddRange(propertyNames);
			}
			ComparisonResult comparisonResult = new CompareLogic(comparisonConfig).Compare(a, b);
			if (comparisonResult.AreEqual)
			{
				return defaultResult;
			}
			return (result: false, difference: comparisonResult.Differences);
		}

		/// <summary>
		/// 复制自身对象[ 深拷贝 ]
		/// </summary>
		/// <typeparam name="T">泛型对象</typeparam>
		/// <param name="obj">对象</param>
		/// <returns>返回深拷贝对象</returns>
		public static T? DeepCopy<T>(this T obj)
		{
			if (obj == null)
			{
				return default(T);
			}
			string text = obj.ToJson();
			if (text != null)
			{
				return text.ToJsonEntity<T>();
			}
			return default(T);
		}

		/// <summary>
		/// 拷贝当前的实例数组，是基于引用层的浅拷贝，如果类型为值类型，那就是深度拷贝，如果类型为引用类型，就是浅拷贝
		/// </summary>
		/// <typeparam name="T">类型对象</typeparam>
		/// <param name="value">数组对象</param>
		/// <returns>拷贝的结果内容</returns>
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

		/// <summary>
		/// 将值类型转换为字节数组
		/// </summary>
		/// <typeparam name="T">值类型（struct）</typeparam>
		/// <param name="value">要转换的值</param>
		/// <returns>字节数组</returns>
		/// <exception cref="T:System.ArgumentException">当T不是值类型时抛出</exception>
		public static byte[] ToBytes<T>(this T value) where T : struct
		{
			if (!typeof(T).IsValueType)
			{
				throw new ArgumentException("T must be a value type");
			}
			byte[] array = new byte[Marshal.SizeOf(value)];
			GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			try
			{
				Marshal.StructureToPtr(value, gCHandle.AddrOfPinnedObject(), fDeleteOld: false);
				return array;
			}
			finally
			{
				gCHandle.Free();
			}
		}

		/// <summary>
		/// 从字节数组转换为值类型
		/// </summary>
		/// <typeparam name="T">目标值类型</typeparam>
		/// <param name="bytes">字节数组</param>
		/// <returns>转换后的值类型实例</returns>
		public static T ToValue<T>(this byte[] bytes) where T : struct
		{
			if (!typeof(T).IsValueType)
			{
				throw new ArgumentException("T must be a value type");
			}
			GCHandle gCHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
			try
			{
				return (T)Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(T));
			}
			finally
			{
				gCHandle.Free();
			}
		}

		/// <summary>
		/// 将值类型数组转换为字节数组
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="array">值类型数组</param>
		/// <returns>字节数组</returns>
		public static byte[] ToBytes<T>(this T[] array) where T : struct
		{
			if (array == null || array.Length == 0)
			{
				return Array.Empty<byte>();
			}
			int num = Marshal.SizeOf(typeof(T));
			byte[] array2 = new byte[array.Length * num];
			GCHandle gCHandle = GCHandle.Alloc(array2, GCHandleType.Pinned);
			try
			{
				nint pointer = gCHandle.AddrOfPinnedObject();
				for (int i = 0; i < array.Length; i++)
				{
					Marshal.StructureToPtr(array[i], IntPtr.Add(pointer, i * num), fDeleteOld: false);
				}
				return array2;
			}
			finally
			{
				gCHandle.Free();
			}
		}

		/// <summary>
		/// 从字节数组转换为值类型数组
		/// </summary>
		/// <typeparam name="T">目标值类型</typeparam>
		/// <param name="bytes">字节数组</param>
		/// <returns>值类型数组</returns>
		public static T[] ToValueArray<T>(this byte[] bytes) where T : struct
		{
			if (bytes == null || bytes.Length == 0)
			{
				return Array.Empty<T>();
			}
			int num = Marshal.SizeOf(typeof(T));
			if (bytes.Length % num != 0)
			{
				throw new ArgumentException($"Byte array length must be a multiple of {num}");
			}
			int num2 = bytes.Length / num;
			T[] array = new T[num2];
			GCHandle gCHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
			try
			{
				nint pointer = gCHandle.AddrOfPinnedObject();
				for (int i = 0; i < num2; i++)
				{
					array[i] = (T)Marshal.PtrToStructure(IntPtr.Add(pointer, i * num), typeof(T));
				}
				return array;
			}
			finally
			{
				gCHandle.Free();
			}
		}

		/// <summary>
		/// 将字符串转换为UTF-8编码的字节数组
		/// </summary>
		/// <param name="str">要转换的字符串</param>
		/// <returns>UTF-8编码的字节数组</returns>
		public static byte[] ToBytes(this string str)
		{
			if (str == null)
			{
				return Array.Empty<byte>();
			}
			return Encoding.UTF8.GetBytes(str);
		}

		/// <summary>
		/// 将字符串转换为指定编码的字节数组
		/// </summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="encoding">字符编码</param>
		/// <returns>字节数组</returns>
		public static byte[] ToBytes(this string str, Encoding encoding)
		{
			if (str == null)
			{
				return Array.Empty<byte>();
			}
			return encoding.GetBytes(str);
		}

		/// <summary>
		/// 从字节数组转换为字符串（UTF-8编码）
		/// </summary>
		/// <param name="bytes">字节数组</param>
		/// <returns>字符串</returns>
		public static string ToString(this byte[] bytes)
		{
			if (bytes == null)
			{
				return string.Empty;
			}
			return Encoding.UTF8.GetString(bytes);
		}

		/// <summary>
		/// 从字节数组转换为指定编码的字符串
		/// </summary>
		/// <param name="bytes">字节数组</param>
		/// <param name="encoding">字符编码</param>
		/// <returns>字符串</returns>
		public static string ToString(this byte[] bytes, Encoding encoding)
		{
			if (bytes == null)
			{
				return string.Empty;
			}
			return encoding.GetString(bytes);
		}

		/// <summary>
		/// 反转字节数组的字节序（大端小端转换）
		/// </summary>
		/// <param name="bytes">要反转的字节数组</param>
		/// <returns>反转后的字节数组</returns>
		public static byte[] ReverseBytes(this byte[] bytes)
		{
			if (bytes == null)
			{
				return null;
			}
			byte[] array = new byte[bytes.Length];
			Array.Copy(bytes, array, bytes.Length);
			Array.Reverse(array);
			return array;
		}

		/// <summary>
		/// 反转值类型的字节序
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="value">要反转的值</param>
		/// <returns>字节序反转后的值</returns>
		public static T ReverseEndianness<T>(this T value) where T : struct
		{
			return value.ToBytes().ReverseBytes().ToValue<T>();
		}

		/// <summary>
		/// 反转值类型数组中每个元素的字节序
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="array">要反转的数组</param>
		/// <returns>字节序反转后的数组</returns>
		public static T[] ReverseEndianness<T>(this T[] array) where T : struct
		{
			if (array == null)
			{
				return null;
			}
			T[] array2 = new T[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].ReverseEndianness();
			}
			return array2;
		}

		/// <summary>
		/// 将值转换为网络字节序（大端序）
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="value">要转换的值</param>
		/// <returns>网络字节序的值</returns>
		public static T ToNetworkByteOrder<T>(this T value) where T : struct
		{
			if (!BitConverter.IsLittleEndian)
			{
				return value;
			}
			return value.ReverseEndianness();
		}

		/// <summary>
		/// 将网络字节序（大端序）的值转换回主机字节序
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="value">网络字节序的值</param>
		/// <returns>主机字节序的值</returns>
		public static T FromNetworkByteOrder<T>(this T value) where T : struct
		{
			if (!BitConverter.IsLittleEndian)
			{
				return value;
			}
			return value.ReverseEndianness();
		}

		/// <summary>
		/// 字节数组转值类型结构体；<br />
		/// 用于操作C/C++的结构体字节数据；<br />
		/// </summary>
		/// <typeparam name="T">值类型结构体</typeparam>
		/// <param name="data">字节数组</param>
		/// <param name="offset">起始位偏移量，默认0</param>
		/// <returns>值类型结构对象</returns>
		public static T ByteArrayToStructure<T>(this byte[] data, int offset = 0) where T : struct
		{
			int num = Marshal.SizeOf<T>();
			nint num2 = Marshal.AllocHGlobal(num);
			try
			{
				Marshal.Copy(data, offset, num2, num);
				return Marshal.PtrToStructure<T>(num2);
			}
			catch
			{
				return default(T);
			}
			finally
			{
				Marshal.FreeHGlobal(num2);
			}
		}

		/// <summary>
		/// 值类型结构体转字节数组；<br />
		/// 用于生成C/C++兼容的结构体二进制数据；<br />
		/// </summary>
		/// <typeparam name="T">值类型结构体</typeparam>
		/// <param name="structure">结构体对象</param>
		/// <returns>结构体对应的字节数组</returns>
		public static byte[] StructureToByteArray<T>(this T structure) where T : struct
		{
			int num = Marshal.SizeOf<T>();
			nint num2 = Marshal.AllocHGlobal(num);
			try
			{
				Marshal.StructureToPtr(structure, num2, fDeleteOld: false);
				byte[] array = new byte[num];
				Marshal.Copy(num2, array, 0, num);
				return array;
			}
			catch
			{
				return Array.Empty<byte>();
			}
			finally
			{
				Marshal.FreeHGlobal(num2);
			}
		}

		/// <summary>
		/// 属性是否在当前对象中存在
		/// </summary>
		/// <typeparam name="T">泛型对象</typeparam>
		/// <param name="data">泛型对象数据</param>
		/// <param name="propertyName">属性名称</param>
		/// <returns>状态</returns>
		public static bool PropertyExists<T>(this T data, string propertyName, out string? message, out PropertyInfo? propertyInfo)
		{
			if (data != null)
			{
				PropertyInfo property = data.GetType().GetProperty(propertyName);
				if (property != null)
				{
					message = $"[ {data.GetType().Name} ] 中存在 [ {propertyName} ] 属性";
					propertyInfo = property;
					return true;
				}
				message = $"[ {data.GetType().Name} ] 中不存在 [ {propertyName} ] 属性";
				propertyInfo = null;
				return false;
			}
			message = "入参不能为null";
			propertyInfo = null;
			return false;
		}

		/// <summary>
		/// 获取对应数据源
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">通用类型</param>
		/// <returns>指定类型的数据</returns>
		public static T GetSource<T>(this object obj)
		{
			if (obj is T)
			{
				return (T)obj;
			}
			return default(T);
		}

		/// <summary>
		/// 将GUID转换为整数
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public static int ToInt(this Guid guid)
		{
			return Math.Abs(guid.GetHashCode());
		}

		/// <summary>
		/// 转换GUID为大写字符串
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public static string ToUpperString(this Guid guid)
		{
			return guid.ToString().ToUpper();
		}

		/// <summary>
		/// 转换guid为小写字符串
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public static string ToLowerString(this Guid guid)
		{
			return guid.ToString().ToLower();
		}

		/// <summary>
		/// 没有分隔线的字符串
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public static string ToNString(this Guid guid)
		{
			return guid.ToString("N");
		}

		/// <summary>
		/// 没有分隔线的小写字符串
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public static string ToLowerNString(this Guid guid)
		{
			return guid.ToString("N").ToLower();
		}

		/// <summary>
		/// 没有分隔线的大写字符串
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public static string ToUpperNString(this Guid guid)
		{
			return guid.ToString("N").ToUpper();
		}

		/// <summary>
		/// 将int转换为GUID
		/// </summary>
		/// <param name="i">111</param>
		/// <returns>00000000-0000-0000-0000-000000000111</returns>
		public static Guid ToGuid(this int i)
		{
			return i.ToString().PadLeft(32, '0').ToGuid();
		}

		/// <summary>
		/// 将long转换为GUID
		/// </summary>
		/// <param name="i">111</param>
		/// <returns>00000000-0000-0000-0000-000000000111</returns>
		public static Guid ToGuid(this long i)
		{
			return i.ToString().PadLeft(32, '0').ToGuid();
		}

		/// <summary>
		/// 将GUID转换为int
		/// </summary>
		/// <param name="guid">00000000-0000-0000-0000-000000000111 的GUID</param>
		/// <returns>111</returns>
		public static int ToInteger(this Guid guid)
		{
			return guid.ToNString().TrimStart('0').ToInt();
		}

		/// <summary>
		/// 将GUID转换为long
		/// </summary>
		/// <param name="guid">00000000-0000-0000-0000-000000000111 的GUID</param>
		/// <returns>111</returns>
		public static long ToLong(this Guid guid)
		{
			return guid.ToNString().TrimStart('0').ToLong();
		}

		/// <summary>
		/// 格式化为长日期格式(yyyy年M月d日)
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static string ToLongDate(this DateTime date)
		{
			return date.ToString("yyyy年M月d日");
		}

		/// <summary>
		/// 格式化为长日期格式(yyyy年MM月dd日)
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static string ToLongDate1(this DateTime date)
		{
			return date.ToString("yyyy年MM月dd日");
		}

		/// <summary>
		/// 格式化为日期格式(yyyy-MM-dd)
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static string ToDateString(this DateTime date)
		{
			return date.ToString("yyyy-MM-dd");
		}

		/// <summary>
		/// 格式化为日期时间格式(yyyy-MM-dd HH:mm:ss.fff)
		/// </summary>
		/// <param name="date"></param>
		public static string ToDateTimeString(this DateTime date)
		{
			return date.ToString("yyyy-MM-dd HH:mm:ss.fff");
		}

		/// <summary>
		/// 格式化为日期时间格式(yyyy-MM-dd HH:mm)
		/// </summary>
		/// <param name="date"></param>
		public static string ToShortDateTimeString(this DateTime date)
		{
			return date.ToString("yyyy-MM-dd HH:mm");
		}

		/// <summary>
		/// 格式化为日期时间格式(yyyy-MM-dd HH:mm:ss)
		/// </summary>
		/// <param name="date"></param>
		public static string ToDateTimeString(this DateTime? date)
		{
			if (!date.HasValue)
			{
				return string.Empty;
			}
			return date.Value.ToDateTimeString();
		}

		/// <summary>
		/// 取日期时间的日期部分；<br />
		/// 使用 Date 属性直接获取，避免 ToString + Parse 的字符串往返开销
		/// </summary>
		/// <param name="date">日期时间</param>
		/// <returns>仅包含日期部分的 DateTime（时间为 00:00:00）</returns>
		public static DateTime GetDate(this DateTime date)
		{
			return date.Date;
		}

		/// <summary>
		/// 取日期时间的 yyyy-MM-dd HH:mm:ss.fff
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime GetDateDetails(this DateTime date)
		{
			return date.ToString("yyyy-MM-dd HH:mm:ss.fff").ToDateTime();
		}

		/// <summary>
		/// 比较字符串区分大小写
		/// </summary>
		/// <param name="str1">字符串1</param>
		/// <param name="str2">字符串2</param>
		/// <returns></returns>
		public static bool EqualsIgnoreCase(this string str1, string str2)
		{
			return str1?.Equals(str2, StringComparison.CurrentCultureIgnoreCase) ?? (str2 == null);
		}

		/// <summary>
		/// 判断字符串是否包含（不区分大不写）
		/// </summary>
		/// <param name="str1"></param>
		/// <param name="str2"></param>
		/// <returns></returns>
		public static bool ContainsIgnoreCase(this string str1, string str2)
		{
			if (str1 != null && str2 != null)
			{
				return str1.IndexOf(str2, StringComparison.CurrentCultureIgnoreCase) >= 0;
			}
			return false;
		}

		/// <summary>
		/// 去除空格
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string Trims(this string str)
		{
			if (!str.IsNullOrEmpty())
			{
				return str.Trim();
			}
			return "";
		}

		/// <summary>
		/// 将字符串转换为GUID
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static Guid ToGuid(this string str)
		{
			if (!Guid.TryParse(str.Trims(), out var result))
			{
				return Guid.Empty;
			}
			return result;
		}

		/// <summary>
		/// 将字符串转换为整数
		/// </summary>
		/// <param name="str"></param>
		/// <param name="defaultValue">转换失败时的默认值</param>
		/// <returns></returns>
		public static int ToInt(this string str, int defaultValue = int.MinValue)
		{
			if (!int.TryParse(str, out var result))
			{
				return defaultValue;
			}
			return result;
		}

		/// <summary>
		/// 将字符串转换为长整型
		/// </summary>
		/// <param name="str"></param>
		/// <param name="defauleValue">转换失败后的默认值</param>
		/// <returns></returns>
		public static long ToLong(this string str, long defauleValue = long.MinValue)
		{
			if (!long.TryParse(str, out var result))
			{
				return defauleValue;
			}
			return result;
		}

		/// <summary>
		/// 将字符串转换为数字
		/// </summary>
		/// <param name="str"></param>
		/// <param name="defaultValue">转换失败时的默认值</param>
		/// <returns></returns>
		public static decimal ToDecimal(this string str, decimal defaultValue = decimal.MinValue)
		{
			if (!decimal.TryParse(str, out var result))
			{
				return defaultValue;
			}
			return result;
		}

		/// <summary>
		/// 将字符串转换为日期时间
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(this string str)
		{
			if (!DateTime.TryParse(str, out var result))
			{
				return DateTime.MinValue;
			}
			return result;
		}

		/// <summary>
		/// 移出所有空格
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string TrimAll(this string str)
		{
			return Regex.Replace(str, "\\s", "");
		}

		/// <summary>
		/// 将逗号分隔的字符串转换为 SQL IN 子句的值列表；<br />
		/// 例如："a,b,c" → "'a','b','c'"；<br />
		/// 注意：内部已对单引号进行转义处理，防止 SQL 注入
		/// </summary>
		/// <param name="str">逗号分开的字符串</param>
		/// <param name="isSignle">是否加单引号包裹每个值</param>
		/// <returns>格式化后的 SQL IN 值列表字符串</returns>
		public static string ToSqlIn(this string str, bool isSignle = true)
		{
			if (str.IsNullOrEmpty())
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = str.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string value = array[i].Replace("'", "''");
				if (isSignle)
				{
					stringBuilder.Append("'");
				}
				stringBuilder.Append(value);
				if (isSignle)
				{
					stringBuilder.Append("'");
				}
				stringBuilder.Append(",");
			}
			return stringBuilder.ToString().TrimEnd(',');
		}

		/// <summary>
		/// 对 URL 进行编码；<br />
		/// 将特殊字符转换为 URL 安全的百分号编码格式
		/// </summary>
		/// <param name="url">原始 URL 字符串</param>
		/// <returns>编码后的 URL 字符串</returns>
		public static string UrlEncode(this string url)
		{
			return WebUtility.UrlEncode(url);
		}

		/// <summary>
		/// 对 URL 进行解码；<br />
		/// 将百分号编码还原为原始字符
		/// </summary>
		/// <param name="url">编码后的 URL 字符串</param>
		/// <returns>解码后的 URL 字符串</returns>
		public static string UrlDecode(this string url)
		{
			return WebUtility.UrlDecode(url);
		}

		/// <summary>
		/// 对 HTML 进行编码；<br />
		/// 将 HTML 特殊字符（如 &lt; &gt; &amp; 等）转换为实体引用
		/// </summary>
		/// <param name="str">原始 HTML 字符串</param>
		/// <returns>编码后的字符串</returns>
		public static string HtmlEncode(this string str)
		{
			return WebUtility.HtmlEncode(str);
		}

		/// <summary>
		/// 对 HTML 进行解码；<br />
		/// 将 HTML 实体引用还原为原始字符
		/// </summary>
		/// <param name="str">编码后的 HTML 字符串</param>
		/// <returns>解码后的字符串</returns>
		public static string HtmlDecode(this string str)
		{
			return WebUtility.HtmlDecode(str);
		}

		/// <summary>
		/// 将集合元素拼接为字符串；<br />
		/// 支持自定义分隔符、前缀和后缀
		/// </summary>
		/// <typeparam name="T">元素类型</typeparam>
		/// <param name="ts">源集合</param>
		/// <param name="split">分隔符，默认逗号</param>
		/// <param name="prefix">每个元素的前缀</param>
		/// <param name="suffix">每个元素的后缀</param>
		/// <returns>拼接后的字符串</returns>
		public static string JoinList<T>(this IEnumerable<T> ts, string split = ",", string prefix = "", string suffix = "")
		{
			if (ts == null || !ts.Any())
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (T t in ts)
			{
				stringBuilder.Append(prefix);
				stringBuilder.Append(t);
				stringBuilder.Append(suffix);
				stringBuilder.Append(split);
			}
			return stringBuilder.ToString().TrimEnd(split.ToCharArray());
		}

		/// <summary>
		/// 将 List 转换为 SQL IN 子句的值列表；<br />
		/// 例如：List{"a","b"} → "'a','b'"
		/// </summary>
		/// <typeparam name="T">元素类型</typeparam>
		/// <param name="ts">源集合</param>
		/// <param name="single">是否包含单引号</param>
		/// <returns>格式化后的 SQL IN 值列表字符串</returns>
		public static string JoinSqlIn<T>(this List<T> ts, bool single = true)
		{
			return ts.JoinList(",", single ? "'" : "", single ? "'" : "");
		}

		/// <summary>
		/// 获取字符串的实际字节长度；<br />
		/// 使用默认编码计算，中文等多字节字符会占用更多字节
		/// </summary>
		/// <param name="str">源字符串</param>
		/// <returns>字节数</returns>
		public static int Size(this string str)
		{
			return Encoding.Default.GetBytes(str).Length;
		}

		/// <summary>
		/// 将日期时间转换为 Unix 时间戳（秒级）；<br />
		/// 基准时间为 1970-01-01 08:00:00（东八区）
		/// </summary>
		/// <param name="dateTime">日期时间</param>
		/// <returns>Unix 时间戳（秒）</returns>
		public static int ToInt(this DateTime dateTime)
		{
			DateTime dateTime2 = new DateTime(1970, 1, 1, 8, 0, 0);
			return Convert.ToInt32((Convert.ToDateTime(dateTime) - dateTime2).TotalSeconds);
		}

		/// <summary>
		/// 将 Unix 时间戳（秒级）转换为本地日期时间；<br />
		/// 基准时间为 1970-01-01 00:00:00 UTC
		/// </summary>
		/// <param name="ticks">Unix 时间戳（秒）</param>
		/// <returns>本地日期时间</returns>
		public static DateTime ToDateTime(this int ticks)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(ticks).ToLocalTime();
		}

		public static string UTF8ToBase64(this string data)
		{
			if (data.IsEmpty())
			{
				return string.Empty;
			}
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
		}

		public static string Base64ToUTF8(this string data)
		{
			if (data.IsEmpty())
			{
				return string.Empty;
			}
			return Convert.FromBase64String(data).ToUTF8String();
		}

		public static string ToBase64(this byte[] bytes)
		{
			return Convert.ToBase64String(bytes);
		}

		public static byte[] Base64ToBytes(this string binary)
		{
			return Convert.FromBase64String(binary);
		}

		/// <summary>
		/// 计算字节数组的 SHA256 哈希值；<br />
		/// 返回小写十六进制字符串
		/// </summary>
		/// <param name="bytes">待计算的字节数组</param>
		/// <returns>SHA256 哈希值的十六进制字符串</returns>
		public static string ComputeSHA256(this byte[] bytes)
		{
			using SHA256 sHA = SHA256.Create();
			byte[] array = sHA.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder(array.Length * 2);
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// 计算字符串的 SHA256 哈希值；<br />
		/// 内部使用 UTF-8 编码转为字节数组后计算
		/// </summary>
		/// <param name="word">待计算的字符串</param>
		/// <returns>SHA256 哈希值的十六进制字符串</returns>
		public static string ComputeSHA256(this string word)
		{
			return Encoding.UTF8.GetBytes(word).ComputeSHA256();
		}

		/// <summary>
		/// 计算文件流的 SHA256 哈希值；<br />
		/// 适用于大文件的哈希计算，流式读取避免一次性加载到内存
		/// </summary>
		/// <param name="stream">文件流对象</param>
		/// <returns>SHA256 哈希值的十六进制字符串</returns>
		public static string ComputeSHA256(this FileStream stream)
		{
			using SHA256 sHA = SHA256.Create();
			byte[] array = sHA.ComputeHash(stream);
			StringBuilder stringBuilder = new StringBuilder(array.Length * 2);
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// 计算字节数组的 SHA1 哈希值；<br />
		/// 返回小写十六进制字符串；<br />
		/// 注意：SHA1 已不再被认为是安全的哈希算法，仅用于非安全场景
		/// </summary>
		/// <param name="bytes">待计算的字节数组</param>
		/// <returns>SHA1 哈希值的十六进制字符串</returns>
		public static string ComputeSHA1(this byte[] bytes)
		{
			using SHA1 sHA = SHA1.Create();
			byte[] array = sHA.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder(array.Length * 2);
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// 计算字符串的 SHA1 哈希值；<br />
		/// 内部使用 UTF-8 编码转为字节数组后计算
		/// </summary>
		/// <param name="word">待计算的字符串</param>
		/// <returns>SHA1 哈希值的十六进制字符串</returns>
		public static string ComputeSHA1(this string word)
		{
			return Encoding.UTF8.GetBytes(word).ComputeSHA1();
		}

		/// <summary>
		/// 计算文件流的 SHA1 哈希值；<br />
		/// 适用于大文件的哈希计算
		/// </summary>
		/// <param name="stream">文件流对象</param>
		/// <returns>SHA1 哈希值的十六进制字符串</returns>
		public static string ComputeSHA1(this FileStream stream)
		{
			using SHA1 sHA = SHA1.Create();
			byte[] array = sHA.ComputeHash(stream);
			StringBuilder stringBuilder = new StringBuilder(array.Length * 2);
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// 计算字符串的 MD5 哈希值；<br />
		/// 返回小写十六进制字符串；<br />
		/// 注意：MD5 已不再被认为是安全的哈希算法，仅用于非安全场景（如文件校验）
		/// </summary>
		/// <param name="plainText">待计算的明文字符串</param>
		/// <returns>MD5 哈希值的十六进制字符串</returns>
		public static string ToMD5(this string plainText)
		{
			StringBuilder stringBuilder = new StringBuilder(32);
			using (MD5 mD = MD5.Create())
			{
				byte[] array = mD.ComputeHash(Encoding.UTF8.GetBytes(plainText));
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array[i].ToString("x2"));
				}
			}
			return stringBuilder.ToString();
		}

		public static string ToEncodeString(this byte[] dealBytes, Encoding encode)
		{
			return encode.GetString(dealBytes);
		}

		public static string ToUTF8String(this byte[] dealBytes)
		{
			return Encoding.UTF8.GetString(dealBytes);
		}

		public static byte[] ToBytesDefault(this string data)
		{
			return Encoding.Default.GetBytes(data);
		}

		public static string ToStringExt(this object data)
		{
			return data?.ToString() ?? string.Empty;
		}

		public static string ToStringExt(this DateTime? data, string format)
		{
			return data?.ToString(format) ?? string.Empty;
		}

		public static string ToStringExt(this DateTimeOffset? data, string format)
		{
			return data?.ToString(format) ?? string.Empty;
		}

		public static ulong ToULong(this decimal data)
		{
			return decimal.ToUInt64(data);
		}

		public static uint ToUInt(this decimal data)
		{
			return decimal.ToUInt32(data);
		}

		public static long ToLong(this decimal data)
		{
			return decimal.ToInt64(data);
		}

		public static int ToInt(this decimal data)
		{
			return decimal.ToInt32(data);
		}

		public static long ToLong(this object data)
		{
			if (data == null)
			{
				return 0L;
			}
			if (data is bool)
			{
				return ((bool)data) ? 1 : 0;
			}
			if (long.TryParse(data.ToString(), out var result))
			{
				return result;
			}
			try
			{
				return Convert.ToInt64(data.ToDouble(0));
			}
			catch (Exception)
			{
				return 0L;
			}
		}

		public static ulong ToULong(this object data)
		{
			if (data == null)
			{
				return 0uL;
			}
			if (data is bool)
			{
				if (!(bool)data)
				{
					return 0uL;
				}
				return 1uL;
			}
			if (ulong.TryParse(data.ToString(), out var result))
			{
				return result;
			}
			try
			{
				return Convert.ToUInt64(data.ToDouble(0));
			}
			catch (Exception)
			{
				return 0uL;
			}
		}

		public static int ToInt(this object data)
		{
			if (data == null)
			{
				return 0;
			}
			if (data is bool)
			{
				if (!(bool)data)
				{
					return 0;
				}
				return 1;
			}
			if (int.TryParse(data.ToString(), out var result))
			{
				return result;
			}
			try
			{
				return Convert.ToInt32(data.ToDouble(0));
			}
			catch (Exception)
			{
				return 0;
			}
		}

		public static int? ToIntOrNull(this object data)
		{
			if (data == null)
			{
				return null;
			}
			if (int.TryParse(data.ToString(), out var result))
			{
				return result;
			}
			return null;
		}

		public static double ToDouble(this object data)
		{
			if (data == null)
			{
				return 0.0;
			}
			if (!double.TryParse(data.ToString(), out var result))
			{
				return 0.0;
			}
			return result;
		}

		public static double ToDouble(this object data, int digits)
		{
			return Math.Round(data.ToDouble(), digits, MidpointRounding.AwayFromZero);
		}

		public static double? ToDoubleOrNull(this object data)
		{
			if (data == null)
			{
				return null;
			}
			if (double.TryParse(data.ToString(), out var result))
			{
				return result;
			}
			return null;
		}

		public static decimal ToDecimal(this object data)
		{
			if (data == null)
			{
				return 0m;
			}
			if (!decimal.TryParse(data.ToString(), out var result))
			{
				return 0m;
			}
			return result;
		}

		public static decimal ToDecimal(this object data, int digits)
		{
			return Math.Round(data.ToDecimal(), digits, MidpointRounding.AwayFromZero);
		}

		public static decimal? ToDecimalOrNull(this object data)
		{
			if (data == null)
			{
				return null;
			}
			if (decimal.TryParse(data.ToString(), out var result))
			{
				return result;
			}
			return null;
		}

		public static decimal? ToDecimalOrNull(this object data, int digits)
		{
			decimal? num = data.ToDecimalOrNull();
			if (!num.HasValue)
			{
				return null;
			}
			return Math.Round(num.Value, digits, MidpointRounding.AwayFromZero);
		}

		public static DateTime ToDate(this object data)
		{
			try
			{
				if (data == null)
				{
					return DateTime.MinValue;
				}
				if (Regex.IsMatch(data.ToStringExt(), "^\\d{8}$"))
				{
					string text = data.ToStringExt();
					return new DateTime(text.Substring(0, 4).ToInt(), text.Substring(4, 2).ToInt(), text.Substring(6, 2).ToInt());
				}
				DateTime result;
				return DateTime.TryParse(data.ToString(), out result) ? result : DateTime.MinValue;
			}
			catch
			{
				return DateTime.MinValue;
			}
		}

		public static DateTime? ToDateOrNull(this object data)
		{
			try
			{
				if (data == null)
				{
					return null;
				}
				if (Regex.IsMatch(data.ToStringExt(), "^\\d{8}$"))
				{
					string text = data.ToStringExt();
					return new DateTime(text.Substring(0, 4).ToInt(), text.Substring(4, 2).ToInt(), text.Substring(6, 2).ToInt());
				}
				if (DateTime.TryParse(data.ToString(), out var result))
				{
					return result;
				}
				return null;
			}
			catch
			{
				return null;
			}
		}

		public static bool ToBool(this object data)
		{
			if (data == null)
			{
				return false;
			}
			bool? flag = data.GetBool();
			if (flag.HasValue)
			{
				return flag.Value;
			}
			bool result;
			return bool.TryParse(data.ToString(), out result) & result;
		}

		private static bool? GetBool(this object data)
		{
			return data.ToString().Trim().ToLower() switch
			{
				"0" => false, 
				"1" => true, 
				"是" => true, 
				"否" => false, 
				"yes" => true, 
				"no" => false, 
				_ => null, 
			};
		}

		public static bool? ToBoolOrNull(this object data)
		{
			if (data == null)
			{
				return null;
			}
			bool? flag = data.GetBool();
			if (flag.HasValue)
			{
				return flag.Value;
			}
			if (bool.TryParse(data.ToString(), out var result))
			{
				return result;
			}
			return null;
		}

		/// <summary>
		/// 把对象转换成 protobuf 字节组<br />
		/// 注意：对象类必须严格遵循 protobuf-net 的定义，在每个属性与类名上方添加对应特性
		/// </summary>
		/// <typeparam name="T">protobuf的对象类</typeparam>
		/// <param name="obj">对象</param>
		/// <returns></returns>
		public static byte[] ToProtobuf<T>(this T obj)
		{
			using MemoryStream memoryStream = new MemoryStream();
			Serializer.Serialize<T>((Stream)memoryStream, obj);
			return memoryStream.ToArray();
		}

		/// <summary>
		/// protobuf 字节组 转换成对象 <br />
		/// 注意：对象类必须严格遵循 protobuf-net 的定义，在每个属性与类名上方添加对应特性
		/// </summary>
		/// <param name="protoBuf">字节组</param>
		/// <returns></returns>
		public static T ToProtobufEntity<T>(this byte[] protoBuf)
		{
			using MemoryStream memoryStream = new MemoryStream(protoBuf);
			return Serializer.Deserialize<T>((Stream)memoryStream);
		}

		/// <summary>
		/// 转换JSON，使用Newtonsoft.Json
		/// </summary>
		/// <param name="obj">对象</param>
		/// <param name="formatting">是否需要格式化</param>
		/// <returns>字符串</returns>
		public static string ToNJson<T>(this T obj, bool formatting = false)
		{
			if (!formatting)
			{
				return JsonConvert.SerializeObject((object)obj);
			}
			return JsonConvert.SerializeObject((object)obj, _serializerSettings);
		}

		/// <summary>
		/// 转换JSON，使用Newtonsoft.Json
		/// </summary>
		/// <param name="obj">对象</param>
		/// <param name="serializerSettings">设置</param>
		/// <returns>字符串</returns>
		public static string ToNJson<T>(this T obj, JsonSerializerSettings serializerSettings)
		{
			return JsonConvert.SerializeObject((object)obj, serializerSettings);
		}

		/// <summary>
		/// JSON转实体，使用Newtonsoft.Json
		/// </summary>
		/// <typeparam name="T">对象</typeparam>
		/// <param name="json">Json字符串</param>
		/// <param name="serializerSettings">设置</param>
		/// <returns>对象</returns>
		public static T? ToNJsonEntity<T>(this string json, JsonSerializerSettings? serializerSettings = null)
		{
			if (serializerSettings != null)
			{
				return JsonConvert.DeserializeObject<T>(json, serializerSettings);
			}
			return JsonConvert.DeserializeObject<T>(json);
		}

		/// <summary>
		/// 转换JSON，使用System.Text.Json
		/// </summary>
		/// <param name="obj">对象</param>
		/// <param name="formatting">是否需要格式化</param>
		/// <returns>字符串</returns>
		public static string ToJson<T>(this T obj, bool formatting = false)
		{
			if (!formatting)
			{
				return JsonSerializer.Serialize(obj, _serializerOptions_formatting_false);
			}
			return JsonSerializer.Serialize(obj, _serializerOptions_formatting_true);
		}

		/// <summary>
		/// 转换JSON，使用System.Text.Json
		/// </summary>
		/// <param name="obj">对象</param>
		/// <param name="serializerOptions">设置</param>
		/// <returns>字符串</returns>
		public static string ToJson<T>(this T obj, JsonSerializerOptions serializerOptions)
		{
			return JsonSerializer.Serialize(obj, serializerOptions);
		}

		/// <summary>
		/// JSON转实体，使用System.Text.Json
		/// </summary>
		/// <typeparam name="T">对象</typeparam>
		/// <param name="json">Json字符串</param>
		/// <param name="serializerOptions">设置</param>
		/// <returns>对象</returns>
		public static T? ToJsonEntity<T>(this string json, JsonSerializerOptions? serializerOptions = null)
		{
			if (serializerOptions != null)
			{
				return JsonSerializer.Deserialize<T>(json, serializerOptions);
			}
			return JsonSerializer.Deserialize<T>(json);
		}

		/// <summary>
		/// JSON格式化，使用System.Text.Json
		/// </summary>
		/// <param name="json">Json 字符串</param>
		/// <returns></returns>
		public static string JsonFormatting(this string json)
		{
			JsonSerializerOptions options = new JsonSerializerOptions
			{
				WriteIndented = true,
				NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
			};
			return JsonSerializer.Serialize(JsonSerializer.Deserialize<object>(json, options), options);
		}

		/// <summary>  
		/// 实体对象序列化成xml字符串  
		/// </summary>  
		/// <typeparam name="T">对象</typeparam>  
		/// <param name="obj">对象</param>  
		/// <returns>XM字符串</returns>  
		public static string? ToXml<T>(this T obj)
		{
			if (obj == null)
			{
				return null;
			}
			using MemoryStream memoryStream = new MemoryStream();
			new XmlSerializer(obj.GetType()).Serialize(memoryStream, obj);
			memoryStream.Position = 0L;
			using StreamReader streamReader = new StreamReader(memoryStream);
			return streamReader.ReadToEnd();
		}

		/// <summary>  
		/// 反序列化xml字符为对象，默认为Utf-8编码  
		/// </summary>  
		/// <typeparam name="T">对象</typeparam>  
		/// <param name="xml">xml 字符串</param>  
		/// <returns></returns>  
		public static T? ToXmlEntity<T>(this string xml)
		{
			return xml.ToXmlEntity<T>(Encoding.UTF8);
		}

		/// <summary>  
		/// 反序列化xml字符为对象  
		/// </summary>  
		/// <typeparam name="T">对象</typeparam>  
		/// <param name="xml">xml字符串</param>  
		/// <param name="encoding">编码格式</param>  
		/// <returns></returns>  
		public static T? ToXmlEntity<T>(this string xml, Encoding encoding)
		{
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
				using MemoryStream stream = new MemoryStream(encoding.GetBytes(xml));
				using StreamReader textReader = new StreamReader(stream, encoding);
				return (T)xmlSerializer.Deserialize(textReader);
			}
			catch
			{
				return default(T);
			}
		}

		/// <summary>
		/// 安全获取可空值类型的实际值或默认值（若为 null）
		/// </summary>
		/// <typeparam name="T">值类型（如 int、DateTime 等）</typeparam>
		/// <param name="value">可空值类型实例（例如 int?）</param>
		/// <returns>
		/// 当 <paramref name="value" /> 不为 null 时返回其实际值；
		/// 当 <paramref name="value" /> 为 null 时返回 <typeparamref name="T" /> 的默认值
		/// </returns>
		/// <example>
		/// 示例：
		/// <code>
		/// int? number = null;
		/// DateTime? date = DateTime.Now;
		///
		/// Console.WriteLine(number.SafeValue()); // 输出 0（int 的默认值）
		/// Console.WriteLine(date.SafeValue());   // 输出当前时间
		/// </code>
		/// </example>
		/// <remarks>
		/// 注意：
		/// - 默认值可能不符合业务逻辑（如 0 可能不是有效的年龄值）
		/// - 仅适用于值类型（不可用于 string 等引用类型）
		/// </remarks>
		public static T SafeValue<T>(this T? value) where T : struct
		{
			return value.GetValueOrDefault();
		}

		/// <summary>
		/// 读取Json文件内容为集合
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filePath">文件地址</param>
		/// <returns></returns>
		public static async Task<List<T>> GetListAsync<T>(this string filePath)
		{
			return JsonSerializer.Deserialize<List<T>>(await filePath.ReadAsync());
		}

		/// <summary>
		/// 读取Json文件内容为实体
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filePath">文件地址</param>
		/// <returns></returns>
		public static async Task<T> GetModelAsync<T>(this string filePath)
		{
			return JsonSerializer.Deserialize<T>(await filePath.ReadAsync());
		}

		/// <summary>
		/// 将数据写入到文件
		/// </summary>
		/// <typeparam name="T">实体信息</typeparam>
		/// <param name="filePath">文件地址</param>
		/// <param name="data">数据</param>
		/// <returns></returns>
		public static async Task WriteDataAsync<T>(this string filePath, List<T> data)
		{
			if (data != null)
			{
				string content = JsonSerializer.Serialize(data);
				await filePath.WriteAsync(content);
			}
		}

		/// <summary>
		/// 将数据写入到文件
		/// </summary>
		/// <typeparam name="T">实体信息</typeparam>
		/// <param name="filePath">文件地址</param>
		/// <param name="data">数据</param>
		/// <returns></returns>
		public static async Task WriteDataAsync<T>(this string filePath, T data)
		{
			if (data != null)
			{
				string content = JsonSerializer.Serialize(data);
				await filePath.WriteAsync(content);
			}
		}

		/// <summary>
		/// 写入文件
		/// </summary>
		/// <param name="filePath">文件地址</param>
		/// <param name="content">写入内容</param>
		/// <returns></returns>
		private static async Task WriteAsync(this string filePath, string content)
		{
			filePath = Directory.GetCurrentDirectory() + filePath;
			using StreamWriter stream = new StreamWriter(filePath, append: false, Encoding.UTF8);
			await stream.WriteAsync(content);
			await stream.FlushAsync();
		}

		/// <summary>
		/// 读取文件
		/// </summary>
		/// <param name="filePath">文件地址</param>
		/// <returns></returns>
		private static async Task<string> ReadAsync(this string filePath)
		{
			if (!filePath.Contains(":\\"))
			{
				filePath = Directory.GetCurrentDirectory() + filePath;
			}
			using StreamReader stream = new StreamReader(filePath, Encoding.UTF8);
			return await stream.ReadToEndAsync();
		}

		/// <summary>
		/// string转ushort数组
		/// </summary>
		/// <param name="inString"></param>
		/// <returns></returns>
		public static ushort[] ToUshort(this string inString)
		{
			if (inString.Length % 2 == 1)
			{
				inString += " ";
			}
			char[] array = inString.ToCharArray();
			_ = new byte[array.Length];
			byte[] array2 = new byte[2];
			ushort[] array3 = new ushort[array.Length / 2];
			int num = 0;
			int num2 = 0;
			while (num < array.Length)
			{
				array2[0] = BitConverter.GetBytes(array[num])[0];
				array2[1] = BitConverter.GetBytes(array[num + 1])[0];
				array3[num2] = BitConverter.ToUInt16(array2, 0);
				num += 2;
				num2++;
			}
			return array3;
		}

		/// <summary>
		/// int转ushort
		/// </summary>
		/// <param name="inInt">int 数据</param>
		/// <returns></returns>
		public static ushort ToUshort(this int inInt)
		{
			return (ushort)inInt;
		}

		/// <summary>
		/// double转ushort
		/// </summary>
		/// <param name="inDouble">double 数据</param>
		/// <returns></returns>
		public static ushort ToUshort(this double inDouble)
		{
			return (ushort)inDouble;
		}

		/// <summary>
		/// float转ushort
		/// </summary>
		/// <param name="inFloat">float 数据</param>
		/// <returns></returns>
		public static ushort ToUshort(this float inFloat)
		{
			return (ushort)inFloat;
		}

		/// <summary>
		/// ushort 数组转换为 ASCII 字符串；<br />
		/// 将每个 ushort 拆分为两个字节，再用 ASCII 编码转为字符串
		/// </summary>
		/// <param name="inUshort">ushort 数据数组</param>
		/// <returns>ASCII 字符串</returns>
		public static string ToString(this ushort[] inUshort)
		{
			byte[] array = new byte[inUshort.Length * 2];
			for (int i = 0; i < inUshort.Length; i++)
			{
				byte[] bytes = BitConverter.GetBytes(inUshort[i]);
				array[i * 2] = bytes[0];
				array[i * 2 + 1] = bytes[1];
			}
			return Encoding.ASCII.GetString(array).Trim();
		}

		/// <summary>
		/// 获取西门子PLC字符串数组--String
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns></returns>
		public static byte[] GetString(this string str)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(str);
			return new byte[2]
			{
				Convert.ToByte(254),
				Convert.ToByte(str.Length)
			}.Concat(bytes).ToArray();
		}

		/// <summary>
		/// 字节数组转16进制字符
		/// </summary>
		/// <param name="bytes">字节数组</param>
		/// <returns></returns>
		public static string ToHexString(this byte[] bytes)
		{
			return string.Join(" ", bytes.Select((byte t) => t.ToString("X2")));
		}

		public static string ToHexString(this byte[] InBytes, char segment)
		{
			return ByteHandler.ByteToHexString(InBytes, segment);
		}

		public static string ToHexString(this byte[] InBytes, char segment, int newLineCount, string format = "{0:X2}")
		{
			return ByteHandler.ByteToHexString(InBytes, segment, newLineCount, format);
		}

		public static byte[] ToHexBytes(this string value)
		{
			return ByteHandler.HexStringToBytes(value);
		}

		public static byte[] ToByteArray(this bool[] array)
		{
			return ByteHandler.BoolArrayToByte(array);
		}

		public static bool[] ToBoolArray(this byte[] InBytes, int length)
		{
			return ByteHandler.ByteToBoolArray(InBytes, length);
		}

		/// <summary>
		/// 获取当前数组的倒序数组，这是一个新的实例，不改变原来的数组值
		/// </summary>
		/// <param name="value">输入的原始数组</param>
		/// <returns>反转之后的数组信息</returns>
		public static T[] ReverseNew<T>(this T[] value)
		{
			T[] array = value.CopyArray();
			Array.Reverse(array);
			return array;
		}

		public static bool[] ToBoolArray(this byte[] InBytes)
		{
			return ByteHandler.ByteToBoolArray(InBytes);
		}

		/// <summary>
		/// 获取Byte数组的第 bytIndex 个位置的，boolIndex偏移的bool值
		/// </summary>
		/// <param name="bytes">字节数组信息</param>
		/// <param name="bytIndex">字节的偏移位置</param>
		/// <param name="boolIndex">指定字节的位偏移</param>
		/// <returns>bool值</returns>
		public static bool GetBoolValue(this byte[] bytes, int bytIndex, int boolIndex)
		{
			return ByteHandler.BoolOnByteIndex(bytes[bytIndex], boolIndex);
		}

		/// <summary>
		/// 获取Byte数组的第 boolIndex 偏移的bool值，这个偏移值可以为 10，就是第 1 个字节的 第3位 
		/// </summary>
		/// <param name="bytes">字节数组信息</param>
		/// <param name="boolIndex">指定字节的位偏移</param>
		/// <returns>bool值</returns>
		public static bool GetBoolByIndex(this byte[] bytes, int boolIndex)
		{
			return ByteHandler.BoolOnByteIndex(bytes[boolIndex / 8], boolIndex % 8);
		}

		/// <summary>
		/// 获取Byte的第 boolIndex 偏移的bool值，比如3，就是第4位 
		/// </summary>
		/// <param name="byt">字节信息</param>
		/// <param name="boolIndex">指定字节的位偏移</param>
		/// <returns>bool值</returns>
		public static bool GetBoolByIndex(this byte byt, int boolIndex)
		{
			return ByteHandler.BoolOnByteIndex(byt, boolIndex % 8);
		}

		/// <summary>
		/// 获取short类型数据的第 boolIndex (从0起始)偏移的bool值，比如3，就是第4位
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
			return Encoding.UTF8.GetString(buffer, index, length);
		}

		/// <summary>
		/// 设置Byte的第 boolIndex 位的bool值，可以强制为 true 或是 false, 不影响其他的位
		/// </summary>
		/// <param name="byt">字节信息</param>
		/// <param name="boolIndex">指定字节的位偏移</param>
		/// <param name="value">bool的值</param>
		/// <returns>修改之后的byte值</returns>
		public static byte SetBoolByIndex(this byte byt, int boolIndex, bool value)
		{
			return ByteHandler.SetBoolOnByteIndex(byt, boolIndex, value);
		}

		/// <summary>
		/// 设置Byte[]的第 boolIndex 位的bool值，可以强制为 true 或是 false, 不影响其他的位，如果是第 10 位，则表示第 1 个字节的第 2 位（都是从 0 地址开始算的）
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
			return ByteHandler.ArrayRemoveDouble(value, leftLength, rightLength);
		}

		public static T[] RemoveBegin<T>(this T[] value, int length)
		{
			return ByteHandler.ArrayRemoveBegin(value, length);
		}

		public static T[] RemoveLast<T>(this T[] value, int length)
		{
			return ByteHandler.ArrayRemoveLast(value, length);
		}

		public static T[] SelectMiddle<T>(this T[] value, int index, int length)
		{
			return ByteHandler.ArraySelectMiddle(value, index, length);
		}

		public static T[] SelectBegin<T>(this T[] value, int length)
		{
			return ByteHandler.ArraySelectBegin(value, length);
		}

		public static T[] SelectLast<T>(this T[] value, int length)
		{
			return ByteHandler.ArraySelectLast(value, length);
		}

		public static T[] SpliceArray<T>(this T[] value, params T[][] arrays)
		{
			List<T[]> list = new List<T[]>(arrays.Length + 1);
			list.Add(value);
			list.AddRange(arrays);
			return ByteHandler.SpliceArray(list.ToArray());
		}

		/// <summary>
		/// 移除指定字符串数据的最后 length 个字符。如果字符串本身的长度不足 length，则返回为空字符串。
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

		public static string ToArrayString<T>(this T[] value)
		{
			return ByteHandler.ArrayFormat(value);
		}

		public static string ToArrayString<T>(this T[] value, string format)
		{
			return ByteHandler.ArrayFormat(value, format);
		}

		/// <summary>
		/// 将字符串数组转换为实际的数据数组。例如字符串格式[1,2,3,4,5]，可以转成实际的数组对象
		/// </summary>
		/// <typeparam name="T">类型对象</typeparam>
		/// <param name="value">字符串数据</param>
		/// <param name="selector">转换方法</param>
		/// <returns>实际的数组</returns>
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
			return value.Split(new char[2] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(selector).ToArray();
		}

		/// <summary>
		/// 将字符串数组转换为实际的数据数组。支持byte,sbyte,bool,short,ushort,int,uint,long,ulong,float,double，使用默认的十进制，例如字符串格式[1,2,3,4,5]，可以转成实际的数组对象
		/// </summary>
		/// <typeparam name="T">类型对象</typeparam>
		/// <param name="value">字符串数据</param>
		/// <returns>实际的数组</returns>
		public static T[] ToStringArray<T>(this string value)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(byte))
			{
				return (T[])(object)value.ToStringArray(byte.Parse);
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
				return (T[])(object)value.ToStringArray(short.Parse);
			}
			if (typeFromHandle == typeof(ushort))
			{
				return (T[])(object)value.ToStringArray(ushort.Parse);
			}
			if (typeFromHandle == typeof(int))
			{
				return (T[])(object)value.ToStringArray(int.Parse);
			}
			if (typeFromHandle == typeof(uint))
			{
				return (T[])(object)value.ToStringArray(uint.Parse);
			}
			if (typeFromHandle == typeof(long))
			{
				return (T[])(object)value.ToStringArray(long.Parse);
			}
			if (typeFromHandle == typeof(ulong))
			{
				return (T[])(object)value.ToStringArray(ulong.Parse);
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

		/// <summary>
		/// 根据英文小数点进行切割字符串，去除空白的字符
		/// </summary>
		/// <param name="str">字符串本身</param>
		/// <returns>切割好的字符串数组，例如输入 "100.5"，返回 "100", "5"</returns>
		public static string[] SplitDot(this string str)
		{
			return str.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
		}

		/// <summary>
		/// 写入
		/// </summary>
		/// <param name="ms">流对象</param>
		/// <param name="buffer">字节</param>
		public static void Write(this MemoryStream ms, byte[] buffer)
		{
			if (buffer != null)
			{
				ms.Write(buffer, 0, buffer.Length);
			}
		}

		/// <summary>
		/// 将<see cref="T:System.UInt16" />数据写入到字节流，字节顺序为相反
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
		/// 根据指定的字节长度信息，获取到随机的字节信息
		/// </summary>
		/// <param name="random">随机数对象</param>
		/// <param name="length">字节的长度信息</param>
		/// <returns>原始字节数组</returns>
		public static byte[] GetBytes(this Random random, int length)
		{
			byte[] array = new byte[length];
			random.NextBytes(array);
			return array;
		}

		/// <summary>
		/// 将byte数组按照双字节进行反转，如果为单数的情况，则自动补齐
		/// </summary>
		/// <remarks>
		/// 例如传入的字节数据是 01 02 03 04, 那么反转后就是 02 01 04 03
		/// </remarks>
		/// <param name="inBytes">输入的字节信息</param>
		/// <returns>反转后的数据</returns>
		public static byte[] ReverseByWord(this byte[] inBytes)
		{
			return ByteHandler.BytesReverseByWord(inBytes);
		}

		/// <summary>
		/// 16进制字符串转字节数组
		/// </summary>
		/// <param name="str">字符串</param>
		/// <param name="strict">严格模式（严格按两个字母间隔一个空格）</param>
		/// <returns></returns>
		public static byte[] ToHex(this string str, bool strict = true)
		{
			if (string.IsNullOrWhiteSpace(str) || str.Trim().Replace(" ", "").Length % 2 != 0)
			{
				throw new ArgumentException("请传入有效的参数");
			}
			if (strict)
			{
				return (from t in str.Split(' ')
					where t != null && t.Length == 2
					select Convert.ToByte(t, 16)).ToArray();
			}
			str = str.Trim().Replace(" ", "");
			List<byte> list = new List<byte>();
			int num;
			for (num = 0; num < str.Length; num++)
			{
				string value = string.Concat(new ReadOnlySpan<char>(str[num]), new ReadOnlySpan<char>(str[++num]));
				list.Add(Convert.ToByte(value, 16));
			}
			return list.ToArray();
		}

		/// <summary>
		/// 字节转字符串
		/// </summary>
		/// <param name="bytes">字节数据</param>
		/// <param name="encoding">编码格式</param>
		/// <returns>字符串</returns>
		public static string HexToStr(this byte[] bytes, Encoding encoding)
		{
			if (encoding == null)
			{
				encoding = Encoding.ASCII;
			}
			return encoding.GetString(bytes);
		}

		/// <summary>
		/// 字符串转字节
		/// </summary>
		/// <param name="strings">字符串</param>
		/// <param name="encoding">编码格式</param>
		/// <returns>字节</returns>
		public static byte[] StrToHex(this string strings, Encoding encoding)
		{
			if (encoding == null)
			{
				encoding = Encoding.ASCII;
			}
			return encoding.GetBytes(strings);
		}

		/// <summary>
		/// 动态排序扩展
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">数据源</param>
		/// <param name="propertyName">属性名</param>
		/// <param name="ascending">true升序/false倒序</param>
		/// <returns></returns>
		/// <exception cref="T:System.ArgumentException"></exception>
		public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string propertyName, bool ascending)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "p");
			PropertyInfo property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
			if (property == null)
			{
				throw new ArgumentException($"Property '{propertyName}' not found on type '{typeof(T).Name}'.");
			}
			LambdaExpression lambdaExpression = Expression.Lambda(Expression.MakeMemberAccess(parameterExpression, property), parameterExpression);
			string methodName = (ascending ? "OrderBy" : "OrderByDescending");
			MethodCallExpression expression = Expression.Call(typeof(Queryable), methodName, new Type[2]
			{
				typeof(T),
				property.PropertyType
			}, source.Expression, lambdaExpression);
			return source.Provider.CreateQuery<T>(expression);
		}

		/// <summary>
		/// 获取枚举字段上指定类型的特性。
		/// </summary>
		/// <typeparam name="T">特性的类型，必须继承自 Attribute。</typeparam>
		/// <param name="enumValue">枚举值。</param>
		/// <returns>特性对象，如果未找到则返回 null。</returns>
		public static T? GetAttribute<T>(this Enum enumValue) where T : Attribute
		{
			try
			{
				FieldInfo field = enumValue.GetType().GetField(enumValue.ToString());
				if (field == null)
				{
					return null;
				}
				return field.GetCustomAttribute<T>(inherit: false);
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// 获取类型上指定类型的特性。
		/// </summary>
		/// <typeparam name="T">特性类型，必须继承自 Attribute。</typeparam>
		/// <param name="type">类型对象。</param>
		/// <returns>特性对象，如果未找到则返回 null。</returns>
		public static T? GetAttribute<T>(this Type type) where T : Attribute
		{
			try
			{
				return type.GetCustomAttribute<T>(inherit: false);
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// AES加密
		/// </summary>
		/// <param name="plainText">加密内容</param>
		/// <param name="key">密钥</param>
		/// <param name="iv">向量,必须16个字节</param>
		/// <returns></returns>
		public static string AESEncrypt(this string plainText, string? key = null, string? iv = null)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				key = "sDhibwYX7EkqG6b6GfcO3/ft1FBeYb+Gdml4lhKXJ/o=";
			}
			if (string.IsNullOrWhiteSpace(iv))
			{
				iv = "zRDPEQHreJ2d7tANOdGoIQ==";
			}
			using Aes aes = Aes.Create();
			aes.Key = GetAssignKey(key);
			aes.IV = GetAssignBytes(iv, 16);
			byte[] bytes = Encoding.UTF8.GetBytes(plainText);
			using MemoryStream memoryStream = new MemoryStream();
			using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
			{
				cryptoStream.Write(bytes, 0, bytes.Length);
				cryptoStream.FlushFinalBlock();
			}
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array = memoryStream.ToArray();
			foreach (byte b in array)
			{
				stringBuilder.AppendFormat("{0:X2}", b);
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// AES 解密
		/// </summary>
		/// <param name="cipherText">加密后的内容</param>
		/// <param name="key">密钥</param>
		/// <param name="iv">向量,必须16个字节</param>
		/// <returns></returns>
		public static string AESDecrypt(this string cipherText, string? key = null, string? iv = null)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				key = "sDhibwYX7EkqG6b6GfcO3/ft1FBeYb+Gdml4lhKXJ/o=";
			}
			if (string.IsNullOrWhiteSpace(iv))
			{
				iv = "zRDPEQHreJ2d7tANOdGoIQ==";
			}
			using Aes aes = Aes.Create();
			aes.Key = GetAssignKey(key);
			aes.IV = GetAssignBytes(iv, 16);
			byte[] array = new byte[cipherText.Length / 2];
			for (int i = 0; i < cipherText.Length / 2; i++)
			{
				int num = Convert.ToInt32(cipherText.Substring(i * 2, 2), 16);
				array[i] = (byte)num;
			}
			using MemoryStream memoryStream = new MemoryStream();
			using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
			{
				cryptoStream.Write(array, 0, array.Length);
				cryptoStream.FlushFinalBlock();
			}
			return Encoding.UTF8.GetString(memoryStream.ToArray());
		}

		/// <summary>
		/// 不足自动补齐, 超过自动截取
		/// </summary>
		/// <param name="key">密钥</param>
		/// <param name="count">长度</param>
		/// <returns></returns>
		private static byte[] GetAssignBytes(string key, int count)
		{
			byte[] array = Encoding.UTF8.GetBytes(key);
			if (array.Length < count)
			{
				Array.Resize(ref array, count);
			}
			else if (array.Length > count)
			{
				Array.Resize(ref array, count);
			}
			return array;
		}

		/// <summary>
		/// 使用 SHA256 生成 32 字节的密钥
		/// </summary>
		/// <param name="key">密钥</param>
		/// <returns></returns>
		private static byte[] GetAssignKey(string key)
		{
			using SHA256 sHA = SHA256.Create();
			return sHA.ComputeHash(Encoding.UTF8.GetBytes(key));
		}

		/// <summary>
		/// 通用模糊查询扩展方法
		/// </summary>
		/// <typeparam name="T">实体类型</typeparam>
		/// <param name="query">当前查询对象</param>
		/// <param name="content">模糊查询的内容</param>
		/// <param name="propertyNames">需要模糊查询的属性名数组</param>
		/// <returns>经过模糊查询过滤后的查询对象</returns>
		public static IQueryable<T> ApplyFuzzySearch<T>(this IQueryable<T> query, string content, params string[] propertyNames)
		{
			if (string.IsNullOrWhiteSpace(content) || propertyNames == null || !propertyNames.Any())
			{
				return query;
			}
			ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");
			Expression expression = null;
			foreach (string propertyOrFieldName in propertyNames)
			{
				MethodCallExpression instance = Expression.Call(Expression.PropertyOrField(parameterExpression, propertyOrFieldName), "ToString", null);
				MethodInfo method = typeof(string).GetMethod("Contains", new Type[1] { typeof(string) });
				MethodCallExpression methodCallExpression = Expression.Call(instance, method, Expression.Constant(content));
				expression = ((expression == null) ? ((Expression)methodCallExpression) : ((Expression)Expression.OrElse(expression, methodCallExpression)));
			}
			Expression<Func<T, bool>> predicate = Expression.Lambda<Func<T, bool>>(expression, new ParameterExpression[1] { parameterExpression });
			return query.Where(predicate);
		}

		/// <summary>
		/// 多条件查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">源</param>
		/// <param name="filters">键值对应的字典</param>
		/// <returns></returns>
		public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> source, Dictionary<string, object> filters)
		{
			if (filters == null)
			{
				throw new ArgumentNullException("filters");
			}
			IQueryable<T> queryable = source;
			foreach (KeyValuePair<string, object> filter in filters)
			{
				string key = filter.Key;
				object obj = filter.Value;
				if (obj != null)
				{
					if (obj is JsonElement jsonElement)
					{
						obj = ConvertJsonElement(jsonElement);
					}
					if (obj is string value)
					{
						queryable = queryable.Where(CreateContainsExpression<T>(key, value));
					}
					else if (obj is DateTime || obj is DateTime?)
					{
						queryable = queryable.Where(CreateDateEqualExpression<T>(key, (DateTime?)obj));
					}
					else if (obj is int || obj is int?)
					{
						queryable = queryable.Where(CreateEqualExpression<T>(key, (int?)obj));
					}
					else if (obj is double || obj is double?)
					{
						queryable = queryable.Where(CreateEqualExpression<T>(key, (double?)obj));
					}
					else if (obj is bool || obj is bool?)
					{
						queryable = queryable.Where(CreateEqualExpression<T>(key, (bool?)obj));
					}
					else if (obj is IEnumerable<object> values)
					{
						queryable = queryable.Where(CreateInExpression<T>(key, values));
					}
				}
			}
			return queryable;
		}

		private static object ConvertJsonElement(JsonElement jsonElement)
		{
			switch (jsonElement.ValueKind)
			{
			case JsonValueKind.String:
			{
				if (DateTime.TryParse(jsonElement.GetString(), out var result))
				{
					return result;
				}
				return jsonElement.GetString();
			}
			case JsonValueKind.Number:
			{
				if (jsonElement.TryGetInt32(out var value))
				{
					return value;
				}
				if (jsonElement.TryGetDouble(out var value2))
				{
					return value2;
				}
				break;
			}
			case JsonValueKind.True:
			case JsonValueKind.False:
				return jsonElement.GetBoolean();
			}
			throw new NotSupportedException($"Unsupported JsonElement ValueKind: {jsonElement.ValueKind}");
		}

		private static Expression<Func<T, bool>> CreateContainsExpression<T>(string propertyName, string value)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");
			MemberExpression instance = Expression.Property(parameterExpression, propertyName);
			MethodInfo method = typeof(string).GetMethod("Contains", new Type[1] { typeof(string) });
			return Expression.Lambda<Func<T, bool>>(Expression.Call(instance, method, Expression.Constant(value)), new ParameterExpression[1] { parameterExpression });
		}

		private static Expression<Func<T, bool>> CreateEqualExpression<T>(string propertyName, object value)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");
			MemberExpression memberExpression = Expression.Property(parameterExpression, propertyName);
			Type type = memberExpression.Type;
			if (type.IsNullable())
			{
				Type underlyingType = Nullable.GetUnderlyingType(type);
				ConstantExpression right = Expression.Constant(Convert.ChangeType(value, underlyingType), type);
				return Expression.Lambda<Func<T, bool>>(Expression.Equal(memberExpression, right), new ParameterExpression[1] { parameterExpression });
			}
			ConstantExpression right2 = Expression.Constant(value, type);
			return Expression.Lambda<Func<T, bool>>(Expression.Equal(memberExpression, right2), new ParameterExpression[1] { parameterExpression });
		}

		private static Expression<Func<T, bool>> CreateDateEqualExpression<T>(string propertyName, DateTime? value)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");
			MemberExpression left = Expression.Property(parameterExpression, propertyName);
			if (value.HasValue)
			{
				ConstantExpression right = Expression.Constant(value.Value, typeof(DateTime?));
				return Expression.Lambda<Func<T, bool>>(Expression.Equal(left, right), new ParameterExpression[1] { parameterExpression });
			}
			return (T x) => true;
		}

		private static Expression<Func<T, bool>> CreateInExpression<T>(string propertyName, IEnumerable<object> values)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");
			MemberExpression memberExpression = Expression.Property(parameterExpression, propertyName);
			ConstantExpression arg = Expression.Constant(values);
			return Expression.Lambda<Func<T, bool>>(Expression.Call(typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First((MethodInfo m) => m.Name == "Contains" && m.GetParameters().Length == 2).MakeGenericMethod(memberExpression.Type), arg, memberExpression), new ParameterExpression[1] { parameterExpression });
		}

		private static bool IsNullable(this Type type)
		{
			return Nullable.GetUnderlyingType(type) != null;
		}

		/// <summary>
		/// 时间范围查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">源</param>
		/// <param name="propertyName">属性名</param>
		/// <param name="startDate">开始时间</param>
		/// <param name="endDate">结束时间</param>
		/// <returns></returns>
		/// <exception cref="T:System.ArgumentNullException"></exception>
		public static IQueryable<T> ApplyDateRangeFilter<T>(this IQueryable<T> source, string propertyName, DateTime? startDate, DateTime? endDate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentNullException("propertyName");
			}
			ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");
			MemberExpression memberExpression = Expression.Property(parameterExpression, propertyName);
			Expression expression = Expression.Constant(true);
			if (startDate.HasValue)
			{
				if (memberExpression.Type != typeof(DateTime?) && memberExpression.Type != typeof(DateTime))
				{
					throw new InvalidOperationException("Property '" + propertyName + "' is not of type DateTime or DateTime?");
				}
				BinaryExpression right = Expression.GreaterThanOrEqual(memberExpression, Expression.Convert(Expression.Constant(startDate.Value), typeof(DateTime?)));
				expression = Expression.AndAlso(expression, right);
			}
			if (endDate.HasValue)
			{
				if (memberExpression.Type != typeof(DateTime?) && memberExpression.Type != typeof(DateTime))
				{
					throw new InvalidOperationException("Property '" + propertyName + "' is not of type DateTime or DateTime?");
				}
				BinaryExpression right2 = Expression.LessThanOrEqual(memberExpression, Expression.Convert(Expression.Constant(endDate.Value), typeof(DateTime?)));
				expression = Expression.AndAlso(expression, right2);
			}
			Expression<Func<T, bool>> predicate = Expression.Lambda<Func<T, bool>>(expression, new ParameterExpression[1] { parameterExpression });
			return source.Where(predicate);
		}

		static ExtendMethod()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			defaultResult = (result: true, difference: null);
			_serializerSettings = new JsonSerializerSettings
			{
				Formatting = (Formatting)1
			};
			_serializerOptions_formatting_false = new JsonSerializerOptions
			{
				IncludeFields = true,
				NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
				WriteIndented = false
			};
			_serializerOptions_formatting_true = new JsonSerializerOptions
			{
				IncludeFields = true,
				NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
				WriteIndented = true
			};
		}
	}
	/// <summary>
	/// 文件处理
	/// </summary>
	public class FileHandler
	{
		/// <summary>
		/// 获取文件路径中的文件夹路径；<br />
		/// 可选择在目录不存在时自动创建
		/// </summary>
		/// <param name="Path">文件完整路径</param>
		/// <param name="Create">目录不存在时是否自动创建，默认 true</param>
		/// <returns>文件夹路径，路径无效时返回 null</returns>
		public static string? GetFolderPath(string Path, bool Create = true)
		{
			string directoryName = System.IO.Path.GetDirectoryName(Path);
			if (!string.IsNullOrWhiteSpace(directoryName) && Create && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			return directoryName;
		}

		/// <summary>
		/// 获取文件名称（含扩展名）
		/// </summary>
		/// <param name="Path">文件完整路径</param>
		/// <returns>文件名称字符串</returns>
		public static string GetFileName(string Path)
		{
			return System.IO.Path.GetFileName(Path);
		}

		/// <summary>
		/// 高效读取文件内容为字符串，支持指定编码和去除 BOM 头
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <param name="encoding">文件编码，默认 UTF-8</param>
		/// <param name="removeBom">是否自动检测并去除 BOM 头，默认 true</param>
		/// <returns>文件内容的字符串，若文件不存在返回空字符串</returns>
		public static string FileToString(string filePath, Encoding? encoding = null, bool removeBom = true)
		{
			if (!File.Exists(filePath))
			{
				return string.Empty;
			}
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			using StreamReader streamReader = new StreamReader(filePath, encoding, removeBom, 65536);
			return streamReader.ReadToEnd();
		}

		/// <summary>
		/// 高效读取文件内容为字符串，支持指定编码和去除 BOM 头
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <param name="encoding">文件编码，默认 UTF-8</param>
		/// <param name="removeBom">是否自动检测并去除 BOM 头，默认 true</param>
		/// <param name="token">取消通知</param>
		/// <returns>文件内容的字符串，若文件不存在返回空字符串</returns>
		public static async Task<string> FileToStringAsync(string filePath, Encoding? encoding = null, bool removeBom = true, CancellationToken token = default(CancellationToken))
		{
			if (!File.Exists(filePath))
			{
				return string.Empty;
			}
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			using StreamReader reader = new StreamReader(filePath, encoding, removeBom, 65536);
			return await reader.ReadToEndAsync(token);
		}

		/// <summary>
		/// 将字符串数据写入文件（覆盖模式），自动创建目录
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <param name="Data">要写入的字符串内容</param>
		/// <param name="encoding">文件编码，默认 UTF-8（无 BOM）</param>
		public static void StringToFile(string filePath, string Data, Encoding? encoding = null)
		{
			string folderPath = GetFolderPath(filePath);
			if (!string.IsNullOrEmpty(folderPath) && !Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			File.WriteAllText(filePath, Data, encoding);
		}

		/// <summary>
		/// 将字符串数据写入文件（覆盖模式），自动创建目录
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <param name="Data">要写入的字符串内容</param>
		/// <param name="encoding">文件编码，默认 UTF-8（无 BOM）</param>
		/// <param name="token">取消通知</param>
		public static async Task StringToFileAsync(string filePath, string Data, Encoding? encoding = null, CancellationToken token = default(CancellationToken))
		{
			string folderPath = GetFolderPath(filePath);
			if (!string.IsNullOrEmpty(folderPath) && !Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			await File.WriteAllTextAsync(filePath, Data, encoding, token);
		}

		/// <summary>
		/// 将文件转换为二进制字节数组；<br />
		/// 使用 File.ReadAllBytes 确保完整读取所有字节，避免 stream.Read 部分读取的风险
		/// </summary>
		/// <param name="Path">文件路径</param>
		/// <returns>文件内容的字节数组</returns>
		public static byte[] ConvertToBinary(string Path)
		{
			return File.ReadAllBytes(Path);
		}

		/// <summary>
		/// 删除⼀个⽂件，System.IO.File.Delete()函数不可以删除只读⽂件，这个函数可以强⾏把只读⽂件删除。
		/// </summary>
		/// <param name="path">路径</param>
		/// <returns></returns>
		public static bool FileDelete(string path)
		{
			FileInfo fileInfo = new FileInfo(path);
			FileAttributes attributes = FileAttributes.None;
			bool flag = false;
			try
			{
				attributes = fileInfo.Attributes;
				fileInfo.Attributes &= ~FileAttributes.ReadOnly;
				flag = true;
				fileInfo.Delete();
			}
			catch (Exception)
			{
				if (flag)
				{
					fileInfo.Attributes = attributes;
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// 清除文件夹内所有文件和子目录中的文件；<br />
		/// 递归处理所有子文件夹，目录不存在时安全返回
		/// </summary>
		/// <param name="path">目录路径</param>
		public static void FolderClear(string path)
		{
			if (Directory.Exists(path))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(path);
				FileInfo[] files = directoryInfo.GetFiles();
				for (int i = 0; i < files.Length; i++)
				{
					FileDelete(files[i].FullName);
				}
				DirectoryInfo[] directories = directoryInfo.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					FolderClear(directories[i].FullName);
				}
			}
		}

		/// <summary>
		/// 运行CMD
		/// </summary>
		/// <param name="cmd"></param>
		public static void RunCmd(string cmd)
		{
			Process.Start("cmd.exe", "/c " + cmd);
		}

		/// <summary>
		/// 删除历史记录
		/// </summary>
		public static void CleanHistory()
		{
			string[] files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.History), "*", SearchOption.AllDirectories);
			for (int i = 0; i < files.Length; i++)
			{
				FileDelete(files[i]);
			}
			RunCmd("RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 1");
		}

		/// <summary>
		/// 删除临时⽂件
		/// </summary>
		public static void CleanTempFiles()
		{
			FolderClear(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));
			RunCmd("RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 8");
		}

		/// <summary>
		/// 删除Cookie
		/// </summary>
		public static void CleanCookie()
		{
			string[] files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Cookies), "*", SearchOption.AllDirectories);
			for (int i = 0; i < files.Length; i++)
			{
				FileDelete(files[i]);
			}
			RunCmd("RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 2");
		}

		/// <summary>
		/// 删除全部
		/// </summary>
		public static void CleanAll()
		{
			CleanHistory();
			CleanCookie();
			CleanTempFiles();
		}
	}
	/// <summary>
	/// FTP处理
	/// </summary>
	public class FtpHandler
	{
		private string host = string.Empty;

		private string userId = string.Empty;

		private string password = string.Empty;

		private IWebProxy proxy;

		private int port = 21;

		private bool enableSsl;

		private bool usePassive = true;

		private bool useBinary = true;

		private string remotePath = "/";

		/// <summary>
		/// 主机
		/// </summary>
		public string Host => host ?? string.Empty;

		/// <summary>
		/// 登录用户名
		/// </summary>
		public string UserId => userId;

		/// <summary>
		/// 密码
		/// </summary>
		public string Password => password;

		/// <summary>
		/// 代理
		/// </summary>
		public IWebProxy Proxy
		{
			get
			{
				return proxy;
			}
			set
			{
				proxy = value;
			}
		}

		/// <summary>
		/// 端口
		/// </summary>
		public int Port
		{
			get
			{
				return port;
			}
			set
			{
				port = value;
			}
		}

		/// <summary>
		/// EnableSsl
		/// </summary>
		public bool EnableSsl => enableSsl;

		/// <summary>
		/// 被动模式
		/// </summary>
		public bool UsePassive
		{
			get
			{
				return usePassive;
			}
			set
			{
				usePassive = value;
			}
		}

		/// <summary>
		/// 二进制方式
		/// </summary>
		public bool UseBinary
		{
			get
			{
				return useBinary;
			}
			set
			{
				useBinary = value;
			}
		}

		/// <summary>
		/// 远端路径
		/// <para>
		///     返回FTP服务器上的当前路径(可以是 / 或 /a/../ 的形式)
		/// </para>
		/// </summary>
		public string RemotePath
		{
			get
			{
				return remotePath;
			}
			set
			{
				string text = "/";
				if (!string.IsNullOrEmpty(value) && value != "/")
				{
					text = "/" + value.TrimStart('/').TrimEnd('/') + "/";
				}
				remotePath = text;
			}
		}

		/// <summary>
		/// 获取FTP服务器上的当前路径
		/// </summary>
		public string CurrentDirectory
		{
			get
			{
				string result = string.Empty;
				string url = Host.TrimEnd('/') + RemotePath;
				using (FtpWebResponse ftpWebResponse = (FtpWebResponse)CreateRequest(url, "PWD").GetResponse())
				{
					string statusDescription = ftpWebResponse.StatusDescription;
					int num = statusDescription.IndexOf('"') + 1;
					int num2 = statusDescription.LastIndexOf('"');
					if (num2 >= num)
					{
						result = statusDescription.Substring(num, num2 - num);
					}
				}
				return result;
			}
		}

		/// <summary>
		/// 创建FTP工具
		/// <para>
		/// 默认不使用SSL,使用二进制传输方式,使用被动模式
		/// </para>
		/// </summary>
		/// <param name="host">主机名称</param>
		/// <param name="userId">用户名</param>
		/// <param name="password">密码</param>
		public FtpHandler(string host, string userId, string password, int port)
			: this(host, userId, password, port, null, enableSsl: false, useBinary: true, usePassive: true)
		{
		}

		/// <summary>
		/// 创建FTP工具
		/// </summary>
		/// <param name="host">主机名称</param>
		/// <param name="userId">用户名</param>
		/// <param name="password">密码</param>
		/// <param name="port">端口</param>
		/// <param name="enableSsl">允许Ssl</param>
		/// <param name="proxy">代理</param>
		/// <param name="useBinary">允许二进制</param>
		/// <param name="usePassive">允许被动模式</param>
		public FtpHandler(string host, string userId, string password, int port, IWebProxy proxy, bool enableSsl, bool useBinary, bool usePassive)
		{
			this.userId = userId;
			this.password = password;
			if (host.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase))
			{
				this.host = host;
			}
			else
			{
				this.host = "ftp://" + host;
			}
			this.port = port;
			this.host = this.host + ":" + port;
			this.proxy = proxy;
			this.enableSsl = enableSsl;
			this.useBinary = useBinary;
			this.usePassive = usePassive;
		}

		/// <summary>
		/// 创建一个FTP请求
		/// </summary>
		/// <param name="url">请求地址</param>
		/// <param name="method">请求方法</param>
		/// <returns>FTP请求</returns>
		private FtpWebRequest CreateRequest(string url, string method)
		{
			FtpWebRequest obj = (FtpWebRequest)WebRequest.Create(url);
			obj.Credentials = new NetworkCredential(userId, password);
			obj.Proxy = proxy;
			obj.KeepAlive = false;
			obj.UseBinary = useBinary;
			obj.UsePassive = usePassive;
			obj.EnableSsl = enableSsl;
			obj.Method = method;
			return obj;
		}

		/// <summary>
		/// 把文件上传到FTP服务器的RemotePath下
		/// </summary>
		/// <param name="localFile">本地文件信息</param>
		/// <param name="remoteFileName">要保存到FTP文件服务器上的名称</param>
		public bool Upload(FileInfo localFile, string remoteFileName)
		{
			bool result = false;
			try
			{
				if (localFile.Exists)
				{
					string url = Host.TrimEnd('/') + RemotePath + remoteFileName + localFile.Name;
					using (Stream stream = CreateRequest(url, "STOR").GetRequestStream())
					{
						using FileStream fileStream = localFile.OpenRead();
						byte[] array = new byte[4096];
						for (int num = fileStream.Read(array, 0, array.Length); num > 0; num = fileStream.Read(array, 0, array.Length))
						{
							stream.Write(array, 0, num);
						}
						fileStream.Close();
						result = true;
					}
					return result;
				}
				return result;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// 从当前目录下下载文件
		/// <para>
		/// 如果本地文件存在,则从本地文件结束的位置开始下载.
		/// </para>
		/// </summary>
		/// <param name="serverName">服务器上的文件名称</param>
		/// <param name="localName">本地文件名称</param>
		/// <returns>返回一个值,指示是否下载成功</returns>
		public bool Download(string serverName, string localName)
		{
			try
			{
				using FileStream fileStream = new FileStream(localName, FileMode.OpenOrCreate);
				string url = Host.TrimEnd('/') + RemotePath + serverName;
				FtpWebRequest ftpWebRequest = CreateRequest(url, "RETR");
				ftpWebRequest.ContentOffset = fileStream.Length;
				using (FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse())
				{
					fileStream.Position = fileStream.Length;
					byte[] array = new byte[GetFileSize(serverName)];
					for (int num = ftpWebResponse.GetResponseStream().Read(array, 0, array.Length); num > 0; num = ftpWebResponse.GetResponseStream().Read(array, 0, array.Length))
					{
						fileStream.Write(array, 0, num);
					}
					ftpWebResponse.GetResponseStream().Close();
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 从当前目录下下载文件内容为字符串；<br />
		/// 使用 StringBuilder 替代字符串拼接，避免循环中 O(n²) 分配
		/// </summary>
		/// <param name="serverName">服务器上的文件名称</param>
		/// <returns>文件内容字符串，失败返回空字符串</returns>
		public string Download(string serverName)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				string url = Host.TrimEnd('/') + RemotePath + serverName;
				using (FtpWebResponse ftpWebResponse = (FtpWebResponse)CreateRequest(url, "RETR").GetResponse())
				{
					byte[] array = new byte[GetFileSize(serverName)];
					for (int num = ftpWebResponse.GetResponseStream().Read(array, 0, array.Length); num > 0; num = ftpWebResponse.GetResponseStream().Read(array, 0, array.Length))
					{
						stringBuilder.Append(Encoding.UTF8.GetString(array, 0, num));
					}
					ftpWebResponse.GetResponseStream().Close();
				}
				return stringBuilder.ToString();
			}
			catch
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// 文件更名
		/// </summary>
		/// <param name="oldFileName">原文件名</param>
		/// <param name="newFileName">新文件名</param>
		/// <returns>返回一个值,指示更名是否成功</returns>
		public bool Rename(string oldFileName, string newFileName)
		{
			bool flag = false;
			string url = Host.TrimEnd('/') + RemotePath + oldFileName;
			FtpWebRequest ftpWebRequest = CreateRequest(url, "RENAME");
			ftpWebRequest.RenameTo = newFileName;
			using ((FtpWebResponse)ftpWebRequest.GetResponse())
			{
				return true;
			}
		}

		/// <summary>
		/// 获取当前目录下文件列表
		/// </summary>
		/// <returns></returns>
		public List<string> GetFileList()
		{
			List<string> list = new List<string>();
			string url = Host.TrimEnd('/') + RemotePath;
			using FtpWebResponse ftpWebResponse = (FtpWebResponse)CreateRequest(url, "NLST").GetResponse();
			using StreamReader streamReader = new StreamReader(ftpWebResponse.GetResponseStream(), Encoding.Default);
			for (string text = streamReader.ReadLine(); text != null; text = streamReader.ReadLine())
			{
				list.Add(text);
			}
			return list;
		}

		/// <summary>
		/// 获取详细列表
		/// </summary>
		/// <returns></returns>
		public List<string> GetFileDetails()
		{
			List<string> list = new List<string>();
			string url = Host.TrimEnd('/') + RemotePath;
			using FtpWebResponse ftpWebResponse = (FtpWebResponse)CreateRequest(url, "LIST").GetResponse();
			using StreamReader streamReader = new StreamReader(ftpWebResponse.GetResponseStream(), Encoding.Default);
			for (string text = streamReader.ReadLine(); text != null; text = streamReader.ReadLine())
			{
				list.Add(text);
			}
			return list;
		}

		/// <summary>
		/// 删除FTP服务器上的文件
		/// </summary>
		/// <param name="fileName">文件名称</param>
		/// <returns>返回一个值,指示是否删除成功</returns>
		public bool DeleteFile(string fileName)
		{
			bool flag = false;
			string url = Host.TrimEnd('/') + RemotePath + fileName;
			using ((FtpWebResponse)CreateRequest(url, "DELE").GetResponse())
			{
				return true;
			}
		}

		/// <summary>
		/// 在当前目录下创建文件夹
		/// </summary>
		/// <param name="dirName">文件夹名称</param>
		/// <returns>返回一个值,指示是否创建成功</returns>
		public bool MakeDirectory(string dirName)
		{
			bool flag = false;
			string url = Host.TrimEnd('/') + RemotePath + dirName;
			using ((FtpWebResponse)CreateRequest(url, "MKD").GetResponse())
			{
				return true;
			}
		}

		/// <summary>
		/// 删除文件夹
		/// </summary>
		/// <param name="dirName">文件夹名称</param>
		/// <returns>返回一个值,指示是否删除成功</returns>
		public bool DeleteDirectory(string dirName)
		{
			bool flag = false;
			string url = Host.TrimEnd('/') + RemotePath + dirName;
			using ((FtpWebResponse)CreateRequest(url, "RMD").GetResponse())
			{
				return true;
			}
		}

		/// <summary>
		/// 获取文件大小
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public long GetFileSize(string fileName)
		{
			long num = 0L;
			string url = Host.TrimEnd('/') + RemotePath + fileName;
			using FtpWebResponse ftpWebResponse = (FtpWebResponse)CreateRequest(url, "SIZE").GetResponse();
			return ftpWebResponse.ContentLength;
		}

		/// <summary>
		/// 给FTP服务器上的文件追加内容
		/// </summary>
		/// <param name="localFile">本地文件</param>
		/// <param name="remoteFileName">FTP服务器上的文件</param>
		/// <returns>返回一个值,指示是否追加成功</returns>
		public bool Append(FileInfo localFile, string remoteFileName)
		{
			if (localFile.Exists)
			{
				using (FileStream stream = new FileStream(localFile.FullName, FileMode.Open))
				{
					return Append(stream, remoteFileName);
				}
			}
			throw new Exception($"本地文件不存在,文件路径:{localFile.FullName}");
		}

		/// <summary>
		/// 给FTP服务器上的文件追加内容
		/// </summary>
		/// <param name="stream">数据流(可通过设置偏移来实现从特定位置开始上传)</param>
		/// <param name="remoteFileName">FTP服务器上的文件</param>
		/// <returns>返回一个值,指示是否追加成功</returns>
		public bool Append(Stream stream, string remoteFileName)
		{
			bool result = false;
			if (stream != null && stream.CanRead)
			{
				string url = Host.TrimEnd('/') + RemotePath + remoteFileName;
				using Stream stream2 = CreateRequest(url, "APPE").GetRequestStream();
				byte[] array = new byte[4096];
				for (int num = stream.Read(array, 0, array.Length); num > 0; num = stream.Read(array, 0, array.Length))
				{
					stream2.Write(array, 0, num);
				}
				result = true;
			}
			return result;
		}

		/// <summary>
		/// 检查文件是否存在；<br />
		/// 使用 OrdinalIgnoreCase 比较替代 ToLower()，避免额外字符串分配
		/// </summary>
		/// <param name="fileName">要检查的文件名</param>
		/// <returns>true：文件存在；false：文件不存在</returns>
		public bool CheckFileExist(string fileName)
		{
			if (string.IsNullOrWhiteSpace(fileName))
			{
				return false;
			}
			fileName = fileName.Trim();
			return GetFileList().Any((string f) => string.Equals(f, fileName, StringComparison.OrdinalIgnoreCase));
		}
	}
	/// <summary>
	/// 网络处理
	/// </summary>
	public class NetHandler
	{
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public enum IpType
		{
			/// <summary>
			/// 获取系统的IP地址
			/// </summary>
			System,
			/// <summary>
			/// 获取本地IP地址信息
			/// </summary>
			Local
		}

		/// <summary>
		/// 获取本机 IP 地址；<br />
		/// System：遍历所有网络适配器获取系统 IPv4 地址；<br />
		/// Local：通过 DNS 解析获取本地 IPv4 地址
		/// </summary>
		/// <param name="ipType">IP 地址获取方式</param>
		/// <returns>IP 地址字符串，获取失败返回 null</returns>
		public static string? GetIP(IpType ipType)
		{
			switch (ipType)
			{
			case IpType.System:
			{
				NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
				foreach (NetworkInterface networkInterface in allNetworkInterfaces)
				{
					if (!networkInterface.Supports(NetworkInterfaceComponent.IPv4))
					{
						continue;
					}
					UnicastIPAddressInformationCollection unicastAddresses = networkInterface.GetIPProperties().UnicastAddresses;
					if (unicastAddresses.Count <= 0)
					{
						continue;
					}
					foreach (UnicastIPAddressInformation item in unicastAddresses)
					{
						if (item.Address.AddressFamily == AddressFamily.InterNetwork)
						{
							return item.Address.ToString();
						}
					}
				}
				return null;
			}
			case IpType.Local:
			{
				IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
				foreach (IPAddress iPAddress in addressList)
				{
					if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
					{
						return iPAddress.ToString();
					}
				}
				return string.Empty;
			}
			default:
				return null;
			}
		}

		/// <summary>
		/// 清理本地指定端口占用的进程；<br />
		/// 通过 netstat 查找占用端口的 PID，然后使用 taskkill 强制结束
		/// </summary>
		/// <param name="Port">目标端口号</param>
		/// <returns>true：清理成功或端口未被占用；false：清理失败</returns>
		public static bool ClearSamePortProcess(int Port)
		{
			try
			{
				using Process process = new Process();
				process.StartInfo.FileName = "cmd.exe";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.RedirectStandardInput = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				List<int> pidByPort = GetPidByPort(process, Port);
				if (pidByPort.Count == 0)
				{
					return true;
				}
				PidKill(process, pidByPort);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// 通过PID获取进程名称
		/// </summary>
		/// <param name="p"></param>
		/// <param name="list_pid"></param>
		/// <returns></returns>
		private static List<string> GetProcessNameByPid(Process p, List<int> list_pid)
		{
			p.Start();
			List<string> list = new List<string>();
			foreach (int item in list_pid)
			{
				p.StandardInput.WriteLine($"tasklist |find \"{item}\"");
				p.StandardInput.WriteLine("exit");
				StreamReader standardOutput = p.StandardOutput;
				string text = standardOutput.ReadLine();
				while (!standardOutput.EndOfStream)
				{
					text = text.Trim();
					if (text.Length > 0 && text.Contains(".exe"))
					{
						string[] array = new Regex("\\s+").Split(text);
						if (array.Length != 0)
						{
							list.Add(array[0]);
						}
					}
					text = standardOutput.ReadLine();
				}
				p.WaitForExit();
				standardOutput.Close();
			}
			p.Close();
			return list;
		}

		/// <summary>
		/// 通过端口得到进程的PID
		/// </summary>
		/// <param name="p"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		private static List<int> GetPidByPort(Process p, int port)
		{
			p.Start();
			p.StandardInput.WriteLine($"netstat -ano|find \"{port}\"");
			p.StandardInput.WriteLine("exit");
			StreamReader standardOutput = p.StandardOutput;
			string text = standardOutput.ReadLine();
			List<int> list = new List<int>();
			while (!standardOutput.EndOfStream)
			{
				text = text.Trim();
				if (text.Length > 0 && (text.Contains("TCP") || text.Contains("UDP")))
				{
					string[] array = new Regex("\\s+").Split(text);
					if (array.Length >= 4 && int.TryParse(array[3], out var result) && !list.Contains(result))
					{
						list.Add(result);
					}
				}
				text = standardOutput.ReadLine();
			}
			p.WaitForExit();
			standardOutput.Close();
			p.Close();
			return list;
		}

		/// <summary>
		/// 强制结束PID进程
		/// </summary>
		/// <param name="p"></param>
		/// <param name="list_pid"></param>
		private static void PidKill(Process p, List<int> list_pid)
		{
			p.Start();
			foreach (int item in list_pid)
			{
				p.StandardInput.WriteLine("taskkill /pid " + item + " /f");
				p.StandardInput.WriteLine("exit");
			}
			p.Close();
		}

		/// <summary>
		/// 检测 IP 是否通畅（Ping 方式）；<br />
		/// 向目标 IP 发送 ICMP 包，判断是否可达
		/// </summary>
		/// <param name="Ip">目标 IP 地址</param>
		/// <returns>true：Ping 成功（目标可达）；false：Ping 失败</returns>
		/// <exception cref="T:System.Exception">Ping 过程中出现异常时抛出</exception>
		public static bool Ping(string Ip)
		{
			if (string.IsNullOrEmpty(Ip))
			{
				return false;
			}
			try
			{
				using Ping ping = new Ping();
				if (ping.Send(Ip).Status == IPStatus.Success)
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Ping " + Ip + " 失败：" + ex.Message, ex);
			}
			return false;
		}

		/// <summary>
		/// 检测 IP 与端口是否通畅（Telnet 方式）；<br />
		/// 通过 TCP Socket 连接指定 IP 和端口，判断连接是否成功
		/// </summary>
		/// <param name="Ip">目标 IP 地址</param>
		/// <param name="Port">目标端口</param>
		/// <returns>true：连接成功（端口通畅）；false：连接失败或超时</returns>
		/// <exception cref="T:System.Exception">连接过程中出现异常时抛出</exception>
		public static bool Telnet(string Ip, int Port)
		{
			if (string.IsNullOrEmpty(Ip) || Port == 0)
			{
				return false;
			}
			try
			{
				IPAddress address = IPAddress.Parse(Ip);
				IPEndPoint point = new IPEndPoint(address, Port);
				Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				try
				{
					bool num = Task.Run(delegate
					{
						sock.Connect(point);
					}).Wait(500);
					if (num)
					{
						sock.Close();
					}
					return num;
				}
				finally
				{
					if (sock != null)
					{
						((IDisposable)sock).Dispose();
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"Telnet {Ip}:{Port} 失败：{ex.Message}", ex);
			}
		}

		/// <summary>
		/// 通过 URL 获取对应的 IP 地址；<br />
		/// 使用 DNS 解析将域名转换为 IP 地址
		/// </summary>
		/// <param name="Url">目标 URL 或域名</param>
		/// <returns>解析后的 IP 地址字符串</returns>
		/// <exception cref="T:System.Exception">DNS 解析失败时抛出</exception>
		public static string GetUrlIp(string Url)
		{
			try
			{
				return Dns.GetHostAddresses(Url)[0].ToString();
			}
			catch (Exception ex)
			{
				throw new Exception("DNS 解析 " + Url + " 失败：" + ex.Message, ex);
			}
		}

		/// <summary>
		/// 判断指定端口是否被占用；<br />
		/// 尝试监听端口，如果抛出 SocketException 说明端口已被占用
		/// </summary>
		/// <param name="port">目标端口号</param>
		/// <returns>true：端口已被占用；false：端口可用</returns>
		public static bool IsPortInUse(int port)
		{
			bool result = false;
			try
			{
				using TcpListener tcpListener = new TcpListener(IPAddress.Any, port);
				tcpListener.Start();
				tcpListener.Stop();
			}
			catch (SocketException)
			{
				result = true;
			}
			return result;
		}
	}
	/// <summary>
	/// 反射处理
	/// </summary>
	public class ReflexHandler
	{
		/// <summary>
		/// 库文件实例参数
		/// </summary>
		public class LibInstanceParam
		{
			/// <summary>
			/// 名称
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			/// 描述
			/// </summary>
			public string Describe { get; set; }

			/// <summary>
			/// 参数类型
			/// </summary>
			public string ParamType { get; set; }

			/// <summary>
			/// 枚举集合
			/// </summary>
			public object EnumArray { get; set; }

			/// <summary>
			/// 对象数据集合
			/// </summary>
			public object ObjArray { get; set; }
		}

		/// <summary>
		/// 为指定对象分配参数；<br />
		/// 根据字典中的键值对，通过反射设置对象属性；<br />
		/// 使用 ContainsKey 替代 Keys.Contains 提升查找性能（O(1) vs O(n)）
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="dic">字段/值 字典</param>
		/// <returns>赋值完成的对象实例</returns>
		public T Assign<T>(Dictionary<string, object> dic) where T : new()
		{
			Type? typeFromHandle = typeof(T);
			T val = new T();
			PropertyInfo[] properties = typeFromHandle.GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (dic.ContainsKey(propertyInfo.Name))
				{
					object value = Convert.ChangeType(dic[propertyInfo.Name], propertyInfo.PropertyType);
					propertyInfo.SetValue(val, value, null);
				}
			}
			return val;
		}

		/// <summary>
		/// 取对象属性值；<br />
		/// 通过反射获取指定属性名的值，转换为字符串返回；<br />
		/// 属性不存在或值为空时返回 null
		/// </summary>
		/// <param name="FieldName">属性名称</param>
		/// <param name="obj">目标对象</param>
		/// <returns>属性值的字符串表示，不存在或为空时返回 null</returns>
		public static string GetModelValue(string FieldName, object obj)
		{
			try
			{
				PropertyInfo property = obj.GetType().GetProperty(FieldName);
				if (property == null)
				{
					return null;
				}
				object value = property.GetValue(obj, null);
				if (value == null)
				{
					return null;
				}
				string text = Convert.ToString(value);
				if (string.IsNullOrEmpty(text))
				{
					return null;
				}
				return text;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// 设置对象属性值；<br />
		/// 通过反射将字符串值转换为属性对应类型后赋值；<br />
		/// 属性不存在时不做任何操作
		/// </summary>
		/// <param name="FieldName">属性名称</param>
		/// <param name="Value">要设置的字符串值</param>
		/// <param name="obj">目标对象</param>
		/// <returns>设置属性后的对象</returns>
		public static object SetModelValue(string FieldName, string Value, object obj)
		{
			try
			{
				PropertyInfo property = obj.GetType().GetProperty(FieldName);
				if (property != null)
				{
					object value = Convert.ChangeType(Value, property.PropertyType);
					property.SetValue(obj, value, null);
					return obj;
				}
			}
			catch
			{
			}
			return obj;
		}

		/// <summary>
		/// 获取方法
		/// </summary>
		/// <param name="MethodName">方法名</param>
		/// <param name="obj">对象</param>
		/// <returns></returns>
		public static MethodInfo GetMethod(string MethodName, object obj)
		{
			try
			{
				return obj.GetType().GetMethod(MethodName);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// 将object对象转换为实体对象
		/// </summary>
		/// <typeparam name="T">实体对象类名</typeparam>
		/// <param name="asObject">object对象</param>
		/// <returns></returns>
		public static T ConvertObject_One<T>(object asObject) where T : new()
		{
			T val = new T();
			if (asObject != null)
			{
				Type type = asObject.GetType();
				PropertyInfo[] properties = typeof(T).GetProperties();
				foreach (PropertyInfo propertyInfo in properties)
				{
					object obj = null;
					object obj2 = type.GetProperty(propertyInfo.Name)?.GetValue(asObject);
					if (obj2 != null)
					{
						obj = ((!propertyInfo.PropertyType.IsGenericType) ? Convert.ChangeType(obj2, propertyInfo.PropertyType) : ((!(propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))) ? Convert.ChangeType(obj2, propertyInfo.PropertyType) : Convert.ChangeType(obj2, Nullable.GetUnderlyingType(propertyInfo.PropertyType))));
						propertyInfo.SetValue(val, obj, null);
					}
				}
			}
			return val;
		}

		/// <summary>
		/// 获取类中所有属性信息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static List<LibInstanceParam> GetClassAllPropertyData<T>()
		{
			List<LibInstanceParam> list = new List<LibInstanceParam>();
			PropertyInfo[] properties = Activator.CreateInstance<T>().GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				LibInstanceParam libInstanceParam = new LibInstanceParam();
				libInstanceParam.Name = propertyInfo.Name;
				libInstanceParam.Describe = propertyInfo.GetDescription();
				libInstanceParam.ParamType = propertyInfo.PropertyType.Name.ToString();
				Type? baseType = propertyInfo.PropertyType.BaseType;
				if ((object)baseType != null && baseType.Name.Equals("Enum"))
				{
					libInstanceParam.ParamType = "Enum";
					libInstanceParam.EnumArray = propertyInfo.PropertyType.GetAllItems();
				}
				else
				{
					libInstanceParam.ObjArray = GetClassAllPropertyData(propertyInfo.PropertyType);
				}
				list.Add(libInstanceParam);
			}
			return list;
		}

		/// <summary>
		/// 获取类中所有属性信息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static List<LibInstanceParam>? GetClassAllPropertyData(Type type)
		{
			try
			{
				List<LibInstanceParam> list = new List<LibInstanceParam>();
				PropertyInfo[] properties = Activator.CreateInstance(type).GetType().GetProperties();
				foreach (PropertyInfo propertyInfo in properties)
				{
					LibInstanceParam libInstanceParam = new LibInstanceParam();
					libInstanceParam.Name = propertyInfo.Name;
					libInstanceParam.Describe = propertyInfo.GetDescription();
					libInstanceParam.ParamType = propertyInfo.PropertyType.Name.ToString();
					Type? baseType = propertyInfo.PropertyType.BaseType;
					if ((object)baseType != null && baseType.Name.Equals("Enum"))
					{
						libInstanceParam.ParamType = "Enum";
						libInstanceParam.EnumArray = propertyInfo.PropertyType.GetAllItems();
					}
					list.Add(libInstanceParam);
				}
				if (list.Count > 0)
				{
					return list;
				}
				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
	/// <summary>
	/// 内存处理工具类；<br />
	/// 提供释放当前进程和所有进程工作集内存的方法；<br />
	/// 依赖 Windows API（kernel32.dll / psapi.dll），仅支持 Windows 平台
	/// </summary>
	public class RMAHandler
	{
		[DllImport("kernel32.dll")]
		public static extern int SetProcessWorkingSetSize(nint process, int minSize, int maxSize);

		[DllImport("psapi.dll")]
		public static extern int EmptyWorkingSet(nint hwProc);

		/// <summary>
		/// 释放当前程序内存工作集；<br />
		/// 先执行完整 GC 并等待终结器完成，再通过 Windows API 缩减工作集；<br />
		/// 仅在 Windows NT 平台上调用 SetProcessWorkingSetSize
		/// </summary>
		public static void ClearMemory()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
			}
		}

		/// <summary>
		/// 清理所有进程的内存工作集；<br />
		/// 先执行完整 GC，再遍历所有进程调用 EmptyWorkingSet；<br />
		/// 使用 using 确保 Process 对象正确释放，避免句柄泄漏；<br />
		/// 系统进程可能拒绝访问，异常被静默忽略
		/// </summary>
		public static void ClearAllMemory()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			Process[] processes = Process.GetProcesses();
			foreach (Process process in processes)
			{
				using (process)
				{
					try
					{
						EmptyWorkingSet(process.Handle);
					}
					catch
					{
					}
				}
			}
		}
	}
	/// <summary>
	/// SVG 代码处理工具类；<br />
	/// 将 SVG 原始代码转换为 WPF DrawingImage XAML 格式
	/// </summary>
	public class SvgHandler
	{
		/// <summary>
		/// 预编译的 SVG path + fill 正则表达式（d 和 fill 同时存在的情况）；<br />
		/// 使用 Compiled 选项提升多次匹配时的性能
		/// </summary>
		private static readonly Regex RegPathFill = new Regex("(d=\")(?<data>.*?)(\")(.*?)(fill=\")(?<color>.*?)(\")", RegexOptions.Compiled);

		/// <summary>
		/// 预编译的 SVG path 正则表达式（仅 d 属性）；<br />
		/// 用于无 fill 属性或含 p-id 属性的元素
		/// </summary>
		private static readonly Regex RegPathOnly = new Regex("( d=\")(?<data>.*?)(\")", RegexOptions.Compiled);

		/// <summary>
		/// SVG 代码转换器；<br />
		/// 将 SVG 中的 path 元素提取并转换为 WPF GeometryDrawing XAML 格式
		/// </summary>
		/// <param name="Name">资源名称</param>
		/// <param name="Annotation">注释说明</param>
		/// <param name="SvgCode">SVG 原始代码</param>
		/// <param name="VsCode">输出的 WPF DrawingImage XAML 代码</param>
		/// <param name="Color">颜色覆盖值，为 null 时使用原始颜色</param>
		/// <returns>true：转换成功；false：转换失败</returns>
		public static bool SvgCodeConverter(string Name, string Annotation, string SvgCode, out string VsCode, string Color = null)
		{
			VsCode = string.Empty;
			string[] array = SvgCode.Split('<');
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("<!--");
			handler.AppendFormatted(Annotation);
			handler.AppendLiteral("-->");
			stringBuilder3.AppendLine(ref handler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder4 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(23, 1, stringBuilder2);
			handler.AppendLiteral("<DrawingImage x:Key=\"");
			handler.AppendFormatted(Name);
			handler.AppendLiteral("\">");
			stringBuilder4.AppendLine(ref handler);
			stringBuilder.AppendLine("<DrawingImage.Drawing>");
			stringBuilder.AppendLine("<DrawingGroup>");
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.Contains(" d=\"") && text.Contains(" fill=\""))
				{
					Match match = RegPathFill.Match(text);
					string value = match.Groups["data"].Value;
					string value2 = ((!string.IsNullOrEmpty(Color)) ? Color : match.Groups["color"].Value);
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder5 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(40, 2, stringBuilder2);
					handler.AppendLiteral("<GeometryDrawing Brush=\"");
					handler.AppendFormatted(value2);
					handler.AppendLiteral("\" Geometry=\"");
					handler.AppendFormatted(value);
					handler.AppendLiteral("\" />");
					stringBuilder5.AppendLine(ref handler);
				}
				else if (text.Contains(" d=\"") && (text.Contains(" fill=\"") || text.Contains(" p-id=\"")))
				{
					string value3 = RegPathOnly.Match(text).Groups["data"].Value;
					string value4 = ((!string.IsNullOrEmpty(Color)) ? Color : "#000000");
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder6 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(40, 2, stringBuilder2);
					handler.AppendLiteral("<GeometryDrawing Brush=\"");
					handler.AppendFormatted(value4);
					handler.AppendLiteral("\" Geometry=\"");
					handler.AppendFormatted(value3);
					handler.AppendLiteral("\" />");
					stringBuilder6.AppendLine(ref handler);
				}
			}
			stringBuilder.AppendLine("</DrawingGroup>");
			stringBuilder.AppendLine("</DrawingImage.Drawing>");
			stringBuilder.AppendLine("</DrawingImage>");
			VsCode = stringBuilder.ToString();
			return true;
		}
	}
	/// <summary>
	/// 时间处理
	/// </summary>
	public class TimeHandler
	{
		/// <summary>
		/// 直接操作底层，性能消耗更小
		/// </summary>
		internal struct ValueStopwatch
		{
			private readonly long _startTimestamp;

			public bool IsActive => _startTimestamp != 0;

			private ValueStopwatch(long startTimestamp)
			{
				_startTimestamp = startTimestamp;
			}

			public static ValueStopwatch StartNew()
			{
				return new ValueStopwatch(Stopwatch.GetTimestamp());
			}

			public TimeSpan GetElapsedTime()
			{
				if (!IsActive)
				{
					throw new InvalidOperationException("An uninitialized, or 'default', ValueStopwatch cannot be used to get elapsed time.");
				}
				long timestamp = Stopwatch.GetTimestamp();
				return Stopwatch.GetElapsedTime(_startTimestamp, timestamp);
			}
		}

		/// <summary>
		/// 锁
		/// </summary>
		private static readonly object Lock = new object();

		/// <summary>
		/// 自身对象集合
		/// </summary>
		private static readonly ConcurrentDictionary<string, TimeHandler> ThisObjList = new ConcurrentDictionary<string, TimeHandler>();

		/// <summary>
		/// 开关
		/// </summary>
		private ValueStopwatch stopwatch;

		/// <summary>
		/// 标识符
		/// </summary>
		private string SN { get; set; }

		/// <summary>
		/// 有参构造函数
		/// </summary>
		/// <param name="SN"></param>
		public TimeHandler(string SN)
		{
			this.SN = SN;
		}

		/// <summary>
		/// 单例模式；<br />
		/// 使用 ConcurrentDictionary.GetOrAdd 替代 LINQ FirstOrDefault 查找；<br />
		/// 性能从 O(n) 提升到 O(1)，且原子操作无需额外加锁
		/// </summary>
		/// <param name="sn">标识符</param>
		/// <returns>对应的 TimeHandler 实例</returns>
		public static TimeHandler Instance(string sn)
		{
			return ThisObjList.GetOrAdd(sn, (string key) => new TimeHandler(key));
		}

		/// <summary>
		/// 开始记录
		/// </summary>
		public void StartRecord()
		{
			stopwatch = ValueStopwatch.StartNew();
		}

		/// <summary>
		/// 停止记录
		/// </summary>
		/// <returns>时，分，秒，毫秒</returns>
		public (int hours, int minutes, int seconds, int milliseconds) StopRecord()
		{
			TimeSpan elapsedTime = stopwatch.GetElapsedTime();
			int item = Math.Round(elapsedTime.TotalHours).ToInt();
			int item2 = Math.Round(elapsedTime.TotalMinutes).ToInt();
			int item3 = Math.Round(elapsedTime.TotalSeconds).ToInt();
			int item4 = Math.Ceiling(elapsedTime.TotalMilliseconds).ToInt();
			return (hours: item, minutes: item2, seconds: item3, milliseconds: item4);
		}

		/// <summary>
		/// 微秒级延时（自旋等待）；<br />
		/// 每次调用创建独立的 Stopwatch，保证线程安全；<br />
		/// 注意：此方法会占用 CPU，不适合长时间延时
		/// </summary>
		/// <param name="time">延时时间，单位：ms（毫秒）</param>
		public static void DelayUs(double time)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			while (stopwatch.Elapsed.TotalMilliseconds < time)
			{
			}
			stopwatch.Stop();
		}
	}
	/// <summary>
	/// XML处理
	/// </summary>
	public class XmlHandler
	{
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public enum XmlType
		{
			File,
			String
		}

		private static readonly ConcurrentDictionary<Type, XmlSerializer> _serializerCache = new ConcurrentDictionary<Type, XmlSerializer>();

		/// <summary>
		/// 获取缓存的 XmlSerializer 实例，避免重复构造导致内存泄漏和性能下降
		/// </summary>
		private static XmlSerializer GetSerializer(Type type)
		{
			return _serializerCache.GetOrAdd(type, (Type t) => new XmlSerializer(t));
		}

		/// <summary>
		/// 创建安全的 XmlDocument 实例（禁用 DTD 和外部实体解析，防止 XXE 攻击）
		/// </summary>
		private static XmlDocument CreateSafeXmlDocument()
		{
			return new XmlDocument
			{
				XmlResolver = null
			};
		}

		/// <summary>
		/// 私有方法，不被外部方法访问
		/// 序列化对象
		/// </summary>
		/// <param name="stream">流</param>
		/// <param name="o">对象</param>
		/// <param name="encoding">编码方式</param>
		private static void XmlSerializeInternal(Stream stream, object o, Encoding encoding)
		{
			if (o == null)
			{
				throw new ArgumentNullException("o");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			XmlSerializer serializer = GetSerializer(o.GetType());
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.NewLineChars = "\r\n";
			xmlWriterSettings.Encoding = encoding;
			xmlWriterSettings.IndentChars = "    ";
			using XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings);
			serializer.Serialize(xmlWriter, o);
			xmlWriter.Close();
		}

		/// <summary>
		/// 序列化，将一个对象序列化为XML字符串
		/// </summary>
		/// <param name="o">要序列化的对象</param>
		/// <param name="encoding">编码方式</param>
		/// <returns>序列化产生的XML字符串</returns>
		public static string XmlSerialize(object o, Encoding encoding)
		{
			using MemoryStream memoryStream = new MemoryStream();
			XmlSerializeInternal(memoryStream, o, encoding);
			memoryStream.Position = 0L;
			using StreamReader streamReader = new StreamReader(memoryStream, encoding);
			return streamReader.ReadToEnd();
		}

		/// <summary>
		/// 序列化，将一个对象按XML序列化的方式写入到一个文件
		/// </summary>
		/// <param name="o">要序列化的对象</param>
		/// <param name="path">保存文件路径</param>
		/// <param name="encoding">编码方式</param>
		public static void XmlSerializeToFile(object o, string path, Encoding encoding)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			using FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
			XmlSerializeInternal(stream, o, encoding);
		}

		/// <summary>
		/// 反序列化，从XML字符串中反序列化对象
		/// </summary>
		/// <typeparam name="T">结果对象类型</typeparam>
		/// <param name="s">包含对象的XML字符串</param>
		/// <param name="encoding">编码方式</param>
		/// <returns>反序列化得到的对象</returns>
		public static T XmlDeserialize<T>(string s, Encoding encoding)
		{
			if (string.IsNullOrEmpty(s))
			{
				throw new ArgumentNullException("s");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			XmlSerializer serializer = GetSerializer(typeof(T));
			using MemoryStream stream = new MemoryStream(encoding.GetBytes(s));
			using StreamReader textReader = new StreamReader(stream, encoding);
			return (T)serializer.Deserialize(textReader);
		}

		/// <summary>
		/// 反序列化，读入一个文件，并按XML的方式反序列化对象。
		/// </summary>
		/// <typeparam name="T">结果对象类型</typeparam>
		/// <param name="path">文件路径</param>
		/// <param name="encoding">编码方式</param>
		/// <returns>反序列化得到的对象</returns>
		public static T XmlDeserializeFromFile<T>(string path, Encoding encoding)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			return XmlDeserialize<T>(File.ReadAllText(path, encoding), encoding);
		}

		/// <summary>
		/// 创建XML文档；<br />
		/// 以指定名称作为根节点，并添加 type 属性
		/// </summary>
		/// <param name="name">根节点名称</param>
		/// <param name="type">根节点的 type 属性值</param>
		/// <returns>创建好的 XmlDocument 对象</returns>
		public static XmlDocument CreateXmlDocument(string name, string type)
		{
			XmlDocument xmlDocument = CreateSafeXmlDocument();
			xmlDocument.LoadXml("<" + name + "/>");
			xmlDocument.DocumentElement.SetAttribute("type", type);
			return xmlDocument;
		}

		/// <summary>
		/// 读取数据
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="node">节点</param>
		/// <param name="attribute">属性名，非空时返回该属性值，否则返回串联值</param>
		/// <returns>string</returns>
		public static string Read(string path, string node, string attribute)
		{
			string result = "";
			try
			{
				XmlDocument xmlDocument = CreateSafeXmlDocument();
				xmlDocument.Load(path);
				XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
				if (xmlNode == null)
				{
					return result;
				}
				result = (attribute.Equals("") ? xmlNode.InnerText : (xmlNode.Attributes[attribute]?.Value ?? ""));
			}
			catch (IOException)
			{
			}
			catch (XmlException)
			{
			}
			return result;
		}

		/// <summary>
		/// 插入数据
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="node">节点</param>
		/// <param name="element">元素名，非空时插入新元素，否则在该元素中插入属性</param>
		/// <param name="attribute">属性名，非空时插入该元素属性值，否则插入元素值</param>
		/// <param name="value">值</param>
		/// <returns></returns>
		public static void Insert(string path, string node, string element, string attribute, string value)
		{
			try
			{
				XmlDocument xmlDocument = CreateSafeXmlDocument();
				xmlDocument.Load(path);
				XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
				if (element.Equals(""))
				{
					if (!attribute.Equals(""))
					{
						((XmlElement)xmlNode).SetAttribute(attribute, value);
					}
				}
				else
				{
					XmlElement xmlElement = xmlDocument.CreateElement(element);
					if (attribute.Equals(""))
					{
						xmlElement.InnerText = value;
					}
					else
					{
						xmlElement.SetAttribute(attribute, value);
					}
					xmlNode.AppendChild(xmlElement);
				}
				xmlDocument.Save(path);
			}
			catch (Exception innerException)
			{
				throw new XmlException("插入XML节点失败: path=" + path + ", node=" + node, innerException);
			}
		}

		/// <summary>
		/// 修改数据
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="node">节点</param>
		/// <param name="attribute">属性名，非空时修改该节点属性值，否则修改节点值</param>
		/// <param name="value">值</param>
		/// <returns></returns>
		public static void Update(string path, string node, string attribute, string value)
		{
			try
			{
				XmlDocument xmlDocument = CreateSafeXmlDocument();
				xmlDocument.Load(path);
				XmlElement xmlElement = (XmlElement)xmlDocument.SelectSingleNode(node);
				if (attribute.Equals(""))
				{
					xmlElement.InnerText = value;
				}
				else
				{
					xmlElement.SetAttribute(attribute, value);
				}
				xmlDocument.Save(path);
			}
			catch (Exception innerException)
			{
				throw new XmlException("更新XML节点失败: path=" + path + ", node=" + node, innerException);
			}
		}

		/// <summary>
		/// 删除数据
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="node">节点</param>
		/// <param name="attribute">属性名，非空时删除该节点属性值，否则删除节点值</param>
		/// <returns></returns>
		public static void Delete(string path, string node, string attribute)
		{
			try
			{
				XmlDocument xmlDocument = CreateSafeXmlDocument();
				xmlDocument.Load(path);
				XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
				if (xmlNode != null)
				{
					if (string.IsNullOrEmpty(attribute))
					{
						xmlNode.ParentNode.RemoveChild(xmlNode);
					}
					else
					{
						(xmlNode as XmlElement)?.RemoveAttribute(attribute);
					}
					xmlDocument.Save(path);
				}
			}
			catch (Exception innerException)
			{
				throw new Exception($"删除XML节点失败: path={path}, node={node}, attribute={attribute}", innerException);
			}
		}

		/// <summary>
		/// 读取XML资源到DataSet中
		/// </summary>
		/// <param name="source">XML资源，文件为路径，否则为XML字符串</param>
		/// <param name="xmlType">XML资源类型</param>
		/// <returns>DataSet</returns>
		public static DataSet GetDataSet(string source, XmlType xmlType)
		{
			DataSet dataSet = new DataSet();
			if (xmlType == XmlType.File)
			{
				dataSet.ReadXml(source);
			}
			else
			{
				XmlDocument xmlDocument = CreateSafeXmlDocument();
				xmlDocument.LoadXml(source);
				using XmlNodeReader reader = new XmlNodeReader(xmlDocument);
				dataSet.ReadXml(reader);
			}
			return dataSet;
		}

		/// <summary>
		/// 获得xml文件中指定节点的节点数据
		/// </summary>
		/// <param name="TableName"></param>
		/// <returns></returns>
		public static string GetNodeInfoByNodeName(string path, string nodeName)
		{
			string result = "";
			XmlDocument xmlDocument = CreateSafeXmlDocument();
			xmlDocument.Load(path);
			XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("//" + nodeName);
			if (xmlNode != null)
			{
				result = xmlNode.InnerText;
			}
			return result;
		}

		/// <summary>
		/// 获取一个字符串 XML 文档中的 DataSet；<br />
		/// 将 XML 字符串解析为 DataSet 对象
		/// </summary>
		/// <param name="xml_string">含有 XML 信息的字符串</param>
		/// <param name="ds">输出的 DataSet 对象</param>
		public static void get_XmlValue_ds(string xml_string, ref DataSet ds)
		{
			XmlDocument xmlDocument = CreateSafeXmlDocument();
			xmlDocument.LoadXml(xml_string);
			using XmlNodeReader reader = new XmlNodeReader(xmlDocument);
			ds.ReadXml(reader);
		}

		/// <summary>
		/// 读取XML资源到DataTable中
		/// </summary>
		/// <param name="source">XML资源，文件为路径，否则为XML字符串</param>
		/// <param name="xmlType">XML资源类型：文件，字符串</param>
		/// <param name="tableName">表名称</param>
		/// <returns>DataTable</returns>
		public static DataTable GetTable(string source, XmlType xmlType, string tableName)
		{
			DataSet dataSet = new DataSet();
			if (xmlType == XmlType.File)
			{
				dataSet.ReadXml(source);
			}
			else
			{
				XmlDocument xmlDocument = CreateSafeXmlDocument();
				xmlDocument.LoadXml(source);
				using XmlNodeReader reader = new XmlNodeReader(xmlDocument);
				dataSet.ReadXml(reader);
			}
			return dataSet.Tables[tableName];
		}

		/// <summary>
		/// 读取XML资源中指定的DataTable的指定行指定列的值
		/// </summary>
		/// <param name="source">XML资源</param>
		/// <param name="xmlType">XML资源类型：文件，字符串</param>
		/// <param name="tableName">表名</param>
		/// <param name="rowIndex">行号</param>
		/// <param name="colName">列名</param>
		/// <returns>值，不存在时返回Null</returns>
		public static object GetTableCell(string source, XmlType xmlType, string tableName, int rowIndex, string colName)
		{
			DataSet dataSet = new DataSet();
			try
			{
				if (xmlType == XmlType.File)
				{
					dataSet.ReadXml(source);
				}
				else
				{
					XmlDocument xmlDocument = CreateSafeXmlDocument();
					xmlDocument.LoadXml(source);
					using XmlNodeReader reader = new XmlNodeReader(xmlDocument);
					dataSet.ReadXml(reader);
				}
				return dataSet.Tables[tableName].Rows[rowIndex][colName];
			}
			finally
			{
				((IDisposable)dataSet)?.Dispose();
			}
		}

		/// <summary>
		/// 读取XML资源中指定的DataTable的指定行指定列的值
		/// </summary>
		/// <param name="source">XML资源</param>
		/// <param name="xmlType">XML资源类型：文件，字符串</param>
		/// <param name="tableName">表名</param>
		/// <param name="rowIndex">行号</param>
		/// <param name="colIndex">列号</param>
		/// <returns>值，不存在时返回Null</returns>
		public static object GetTableCell(string source, XmlType xmlType, string tableName, int rowIndex, int colIndex)
		{
			DataSet dataSet = new DataSet();
			try
			{
				if (xmlType == XmlType.File)
				{
					dataSet.ReadXml(source);
				}
				else
				{
					XmlDocument xmlDocument = CreateSafeXmlDocument();
					xmlDocument.LoadXml(source);
					using XmlNodeReader reader = new XmlNodeReader(xmlDocument);
					dataSet.ReadXml(reader);
				}
				return dataSet.Tables[tableName].Rows[rowIndex][colIndex];
			}
			finally
			{
				((IDisposable)dataSet)?.Dispose();
			}
		}

		/// <summary>
		/// 将DataTable写入XML文件中
		/// </summary>
		/// <param name="dt">含有数据的DataTable</param>
		/// <param name="filePath">文件路径</param>
		public static void SaveTableToFile(DataTable dt, string filePath)
		{
			DataSet dataSet = new DataSet("Config");
			try
			{
				dataSet.Tables.Add(dt.Copy());
				dataSet.WriteXml(filePath);
			}
			finally
			{
				((IDisposable)dataSet)?.Dispose();
			}
		}

		/// <summary>
		/// 将DataTable以指定的根结点名称写入文件
		/// </summary>
		/// <param name="dt">含有数据的DataTable</param>
		/// <param name="rootName">根结点名称</param>
		/// <param name="filePath">文件路径</param>
		public static void SaveTableToFile(DataTable dt, string rootName, string filePath)
		{
			DataSet dataSet = new DataSet(rootName);
			try
			{
				dataSet.Tables.Add(dt.Copy());
				dataSet.WriteXml(filePath);
			}
			finally
			{
				((IDisposable)dataSet)?.Dispose();
			}
		}

		/// <summary>
		/// 使用DataSet方式更新XML文件节点
		/// </summary>
		/// <param name="filePath">XML文件路径</param>
		/// <param name="tableName">表名称</param>
		/// <param name="rowIndex">行号</param>
		/// <param name="colName">列名</param>
		/// <param name="content">更新值</param>
		/// <returns>更新是否成功</returns>
		public static bool UpdateTableCell(string filePath, string tableName, int rowIndex, string colName, string content)
		{
			DataSet dataSet = new DataSet();
			try
			{
				dataSet.ReadXml(filePath);
				DataTable dataTable = dataSet.Tables[tableName];
				if (dataTable.Rows[rowIndex][colName] != null)
				{
					dataTable.Rows[rowIndex][colName] = content;
					dataSet.WriteXml(filePath);
					return true;
				}
				return false;
			}
			finally
			{
				((IDisposable)dataSet)?.Dispose();
			}
		}

		/// <summary>
		/// 使用DataSet方式更新XML文件节点
		/// </summary>
		/// <param name="filePath">XML文件路径</param>
		/// <param name="tableName">表名称</param>
		/// <param name="rowIndex">行号</param>
		/// <param name="colIndex">列号</param>
		/// <param name="content">更新值</param>
		/// <returns>更新是否成功</returns>
		public static bool UpdateTableCell(string filePath, string tableName, int rowIndex, int colIndex, string content)
		{
			DataSet dataSet = new DataSet();
			try
			{
				dataSet.ReadXml(filePath);
				DataTable dataTable = dataSet.Tables[tableName];
				if (dataTable.Rows[rowIndex][colIndex] != null)
				{
					dataTable.Rows[rowIndex][colIndex] = content;
					dataSet.WriteXml(filePath);
					return true;
				}
				return false;
			}
			finally
			{
				((IDisposable)dataSet)?.Dispose();
			}
		}

		/// <summary>
		/// 读取XML资源中的指定节点内容
		/// </summary>
		/// <param name="source">XML资源</param>
		/// <param name="xmlType">XML资源类型：文件，字符串</param>
		/// <param name="nodeName">节点名称</param>
		/// <returns>节点内容</returns>
		public static object GetNodeValue(string source, XmlType xmlType, string nodeName)
		{
			XmlDocument xmlDocument = CreateSafeXmlDocument();
			if (xmlType == XmlType.File)
			{
				xmlDocument.Load(source);
			}
			else
			{
				xmlDocument.LoadXml(source);
			}
			return xmlDocument.DocumentElement.SelectSingleNode("//" + nodeName)?.InnerText;
		}

		/// <summary>
		/// 读取XML资源中的指定节点内容
		/// </summary>
		/// <param name="source">XML资源</param>
		/// <param name="nodeName">节点名称</param>
		/// <returns>节点内容</returns>
		public static object GetNodeValue(string source, string nodeName)
		{
			if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(nodeName) || source.Length < nodeName.Length * 2)
			{
				return null;
			}
			int num = source.IndexOf("<" + nodeName + ">", StringComparison.Ordinal);
			if (num == -1)
			{
				return null;
			}
			int num2 = num + nodeName.Length + 2;
			int num3 = source.IndexOf("</" + nodeName + ">", StringComparison.Ordinal);
			if (num3 == -1 || num2 >= num3)
			{
				return null;
			}
			return source.Substring(num2, num3 - num2);
		}

		/// <summary>
		/// 更新XML文件中的指定节点内容
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <param name="nodeName">节点名称</param>
		/// <param name="nodeValue">更新内容</param>
		/// <returns>更新是否成功</returns>
		public static bool UpdateNode(string filePath, string nodeName, string nodeValue)
		{
			bool flag = false;
			XmlDocument xmlDocument = CreateSafeXmlDocument();
			xmlDocument.Load(filePath);
			XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("//" + nodeName);
			if (xmlNode != null)
			{
				xmlNode.InnerText = nodeValue;
				xmlDocument.Save(filePath);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 读取xml文件，并将文件序列化为类
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path"></param>
		/// <returns></returns>
		public static T ReadXML<T>(string path)
		{
			XmlSerializer serializer = GetSerializer(typeof(T));
			using StreamReader textReader = new StreamReader(path);
			return (T)serializer.Deserialize(textReader);
		}

		/// <summary>
		/// 将对象写入XML文件
		/// </summary>
		/// <typeparam name="T">C#对象名</typeparam>
		/// <param name="item">对象实例</param>
		/// <param name="path">路径</param>
		/// <param name="jjdbh">标号</param>
		/// <param name="ends">结束符号（整个xml的路径类似如下：C:\xmltest\201111send.xml，其中path=C:\xmltest,jjdbh=201111,ends=send）</param>
		/// <returns></returns>
		public static string WriteXML<T>(T item, string path, string jjdbh, string ends)
		{
			if (string.IsNullOrEmpty(ends))
			{
				ends = "send";
			}
			int num = 0;
			XmlSerializer serializer = GetSerializer(item.GetType());
			string path2 = Path.Combine(path, jjdbh + ends + ".xml");
			while (true)
			{
				try
				{
					using FileStream stream = new FileStream(path2, FileMode.Create, FileAccess.Write, FileShare.None);
					using TextWriter textWriter = new StreamWriter(stream, Encoding.UTF8);
					XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
					xmlSerializerNamespaces.Add(string.Empty, string.Empty);
					serializer.Serialize(textWriter, item, xmlSerializerNamespaces);
				}
				catch (Exception)
				{
					if (num < 5)
					{
						num++;
						continue;
					}
				}
				break;
			}
			return SerializeToXmlStr(item, omitXmlDeclaration: true);
		}

		/// <summary>
		/// 静态扩展
		/// </summary>
		/// <typeparam name="T">需要序列化的对象类型，必须声明[Serializable]特征</typeparam>
		/// <param name="obj">需要序列化的对象</param>
		/// <param name="omitXmlDeclaration">true:省略XML声明;否则为false.默认false，即编写 XML 声明。</param>
		/// <returns></returns>
		public static string SerializeToXmlStr<T>(T obj, bool omitXmlDeclaration)
		{
			return XmlSerialize(obj, omitXmlDeclaration);
		}

		/// <summary>
		/// 使用XmlSerializer序列化对象
		/// </summary>
		/// <typeparam name="T">需要序列化的对象类型，必须声明[Serializable]特征</typeparam>
		/// <param name="obj">需要序列化的对象</param>
		/// <param name="omitXmlDeclaration">true:省略XML声明;否则为false.默认false，即编写 XML 声明。</param>
		/// <returns>序列化后的字符串</returns>
		public static string XmlSerialize<T>(T obj, bool omitXmlDeclaration)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.OmitXmlDeclaration = omitXmlDeclaration;
			xmlWriterSettings.Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
			using MemoryStream memoryStream = new MemoryStream();
			using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
			{
				XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
				xmlSerializerNamespaces.Add(string.Empty, string.Empty);
				GetSerializer(typeof(T)).Serialize(xmlWriter, obj, xmlSerializerNamespaces);
				xmlWriter.Flush();
			}
			return Encoding.UTF8.GetString(memoryStream.ToArray());
		}

		/// <summary>
		/// 使用XmlSerializer序列化对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path">文件路径</param>
		/// <param name="obj">需要序列化的对象</param>
		/// <param name="omitXmlDeclaration">true:省略XML声明;否则为false.默认false，即编写 XML 声明。</param>
		/// <param name="removeDefaultNamespace">是否移除默认名称空间(如果对象定义时指定了:XmlRoot(Namespace = "http://www.xxx.com/xsd")则需要传false值进来)</param>
		/// <returns>序列化后的字符串</returns>
		public static void XmlSerialize<T>(string path, T obj, bool omitXmlDeclaration, bool removeDefaultNamespace)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.OmitXmlDeclaration = omitXmlDeclaration;
			using XmlWriter xmlWriter = XmlWriter.Create(path, xmlWriterSettings);
			XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
			if (removeDefaultNamespace)
			{
				xmlSerializerNamespaces.Add(string.Empty, string.Empty);
			}
			GetSerializer(typeof(T)).Serialize(xmlWriter, obj, xmlSerializerNamespaces);
		}

		private static byte[] ShareReadFile(string filePath)
		{
			byte[] array;
			using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				array = new byte[fileStream.Length];
				int num = (int)fileStream.Length;
				int num2 = 0;
				while (num > 0)
				{
					int num3 = fileStream.Read(array, num2, num);
					if (num3 == 0)
					{
						break;
					}
					num2 += num3;
					num -= num3;
				}
			}
			return array;
		}

		/// <summary>
		/// 从文件读取并反序列化为对象 （解决: 多线程或多进程下读写并发问题）
		/// </summary>
		/// <typeparam name="T">返回的对象类型</typeparam>
		/// <param name="path">文件地址</param>
		/// <returns></returns>
		public static T XmlFileDeserialize<T>(string path)
		{
			byte[] array = ShareReadFile(path);
			if (array.Length < 1)
			{
				for (int i = 0; i < 5; i++)
				{
					array = ShareReadFile(path);
					if (array.Length != 0)
					{
						break;
					}
					Thread.Sleep(50);
				}
			}
			XmlDocument xmlDocument = CreateSafeXmlDocument();
			using MemoryStream inStream = new MemoryStream(array);
			xmlDocument.Load(inStream);
			if (xmlDocument.DocumentElement != null)
			{
				using (XmlNodeReader xmlReader = new XmlNodeReader(xmlDocument.DocumentElement))
				{
					return (T)GetSerializer(typeof(T)).Deserialize(xmlReader);
				}
			}
			return default(T);
		}

		/// <summary>
		/// 使用XmlSerializer反序列化对象
		/// </summary>
		/// <param name="xmlOfObject">需要反序列化的xml字符串</param>
		/// <returns>反序列化后的对象</returns>
		public static T XmlDeserialize<T>(string xmlOfObject) where T : class
		{
			XmlReaderSettings settings = new XmlReaderSettings
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			};
			using XmlReader xmlReader = XmlReader.Create(new StringReader(xmlOfObject), settings);
			return (T)GetSerializer(typeof(T)).Deserialize(xmlReader);
		}
	}
}
namespace Snet.Utility.comparer
{
	/// <summary>
	/// Cache for properties, fields, and methods to speed up reflection
	/// </summary>
	internal static class Cache
	{
		/// <summary>
		/// Reflection Cache for property info
		/// </summary>
		private static readonly Dictionary<Type, PropertyInfo[]> _propertyCache;

		/// <summary>
		/// Reflection Cache for field info
		/// </summary>
		private static readonly Dictionary<Type, FieldInfo[]> _fieldCache;

		/// <summary>
		/// Reflection Cache for methods
		/// </summary>
		private static readonly Dictionary<Type, MethodInfo[]> _methodList;

		/// <summary>
		/// Static constructor
		/// </summary>
		static Cache()
		{
			_propertyCache = new Dictionary<Type, PropertyInfo[]>();
			_fieldCache = new Dictionary<Type, FieldInfo[]>();
			_methodList = new Dictionary<Type, MethodInfo[]>();
		}

		/// <summary>
		/// Clear the cache
		/// </summary>
		public static void ClearCache()
		{
			lock (_propertyCache)
			{
				_propertyCache.Clear();
			}
			lock (_fieldCache)
			{
				_fieldCache.Clear();
			}
			lock (_methodList)
			{
				_methodList.Clear();
			}
		}

		/// <summary>
		/// Get a list of the fields within a type
		/// </summary>
		/// <param name="config"> </param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<FieldInfo> GetFieldInfo(ComparisonConfig config, Type type)
		{
			lock (_fieldCache)
			{
				bool flag = TypeHelper.IsDynamicObject(type);
				if (config.Caching && _fieldCache.ContainsKey(type))
				{
					return _fieldCache[type];
				}
				BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
				if (config.ComparePrivateFields | flag)
				{
					bindingFlags |= BindingFlags.NonPublic;
				}
				if (config.CompareStaticFields)
				{
					bindingFlags |= BindingFlags.Static;
				}
				Func<FieldInfo, bool> func = (FieldInfo field) => field.Name.Contains("k__BackingField");
				List<FieldInfo> list = new List<FieldInfo>();
				Type type2 = type;
				do
				{
					FieldInfo[] fields = type2.GetFields(bindingFlags);
					foreach (FieldInfo fieldInfo in fields)
					{
						if (!config.ComparePrivateFields)
						{
							list.Add(fieldInfo);
						}
						else if (config.ComparePrivateFields && !config.CompareBackingFields && !func(fieldInfo))
						{
							list.Add(fieldInfo);
						}
						else if (config.ComparePrivateFields && config.CompareBackingFields)
						{
							list.Add(fieldInfo);
						}
						else if (flag)
						{
							list.Add(fieldInfo);
						}
					}
					type2 = type2.BaseType;
				}
				while (type2 != null);
				FieldInfo[] array = list.ToArray();
				if (config.Caching)
				{
					_fieldCache.Add(type, array);
				}
				return array;
			}
		}

		/// <summary>
		/// Get the value of a property
		/// </summary>
		/// <param name="config"> </param>
		/// <param name="type"></param>
		/// <param name="objectValue"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static object GetPropertyValue(ComparisonConfig config, Type type, object objectValue, string propertyName)
		{
			lock (_propertyCache)
			{
				return GetPropertyInfo(config, type).First((PropertyInfo o) => o.Name == propertyName).GetValue(objectValue, null);
			}
		}

		/// <summary>
		/// Get a list of the properties in a type
		/// </summary>
		/// <param name="config"> </param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<PropertyInfo> GetPropertyInfo(ComparisonConfig config, Type type)
		{
			lock (_propertyCache)
			{
				bool flag = TypeHelper.IsDynamicObject(type);
				if (config.Caching && _propertyCache.ContainsKey(type))
				{
					return _propertyCache[type];
				}
				PropertyInfo[] array = (((config.ComparePrivateProperties | flag) && !config.CompareStaticProperties) ? type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : (((config.ComparePrivateProperties | flag) && config.CompareStaticProperties) ? type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) : (config.CompareStaticProperties ? type.GetProperties() : type.GetProperties(BindingFlags.Instance | BindingFlags.Public))));
				if (config.Caching)
				{
					_propertyCache.Add(type, array);
				}
				return array;
			}
		}

		/// <summary>
		/// Get a method by name
		/// </summary>
		/// <param name="type"></param>
		/// <param name="methodName"></param>
		/// <returns></returns>
		public static MethodInfo GetMethod(Type type, string methodName)
		{
			lock (_methodList)
			{
				return GetMethods(type).FirstOrDefault((MethodInfo m) => m.Name == methodName);
			}
		}

		/// <summary>
		/// Get the cached methods for a type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<MethodInfo> GetMethods(Type type)
		{
			lock (_methodList)
			{
				if (_methodList.ContainsKey(type))
				{
					return _methodList[type];
				}
				MethodInfo[] methods = type.GetMethods();
				_methodList.Add(type, methods);
				return methods;
			}
		}
	}
	/// <summary>
	/// CompareException with a Result Property
	/// </summary>
	public class CompareException : Exception
	{
		/// <summary>
		/// The comparison Result
		/// </summary>
		public ComparisonResult Result { get; private set; }

		/// <summary>
		/// CompareException Constructor
		/// </summary>
		/// <param name="result"></param>
		/// <param name="message"></param>
		public CompareException(ComparisonResult result, string message)
			: base(message)
		{
			Result = result;
		}
	}
	/// <summary>
	/// A set of BDD style comparison extensions for use with Testing Frameworks
	/// </summary>
	public static class CompareExtensions
	{
		/// <summary>
		/// Alter the configuration for the comparison
		/// </summary>
		public static ComparisonConfig Config { get; set; }

		static CompareExtensions()
		{
			Config = new ComparisonConfig();
		}

		/// <summary>
		/// Throws a CompareException if the classes are not equal
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="actual"></param>
		/// <param name="expected"></param>
		/// <param name="message"></param>
		/// <param name="compareConfig"></param>
		public static void ShouldCompare<T>(this T actual, T expected, string message = null, ComparisonConfig compareConfig = null)
		{
			ComparisonResult comparisonResult = new CompareLogic(compareConfig ?? Config).Compare(expected, actual);
			if (!comparisonResult.AreEqual)
			{
				throw new CompareException(comparisonResult, BuildExpectedEqualMessage(message, comparisonResult));
			}
		}

		/// <summary>
		/// Throws a CompareException if the classes are equal
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="actual"></param>
		/// <param name="expected"></param>
		/// <param name="message"></param>
		/// <param name="compareConfig"></param>
		public static void ShouldNotCompare<T>(this T actual, T expected, string message = null, ComparisonConfig compareConfig = null)
		{
			ComparisonResult comparisonResult = new CompareLogic(compareConfig ?? Config).Compare(expected, actual);
			if (comparisonResult.AreEqual)
			{
				throw new CompareException(comparisonResult, BuildExpectedNotEqualMessage(message, comparisonResult));
			}
		}

		private static string BuildExpectedEqualMessage(string message, ComparisonResult result)
		{
			message = message ?? "Objects expected to be equal";
			return message + Environment.NewLine + result.DifferencesString + Environment.NewLine;
		}

		private static string BuildExpectedNotEqualMessage(string message, ComparisonResult result)
		{
			message = message ?? "Objects expected NOT to be equal";
			return message + Environment.NewLine + result.DifferencesString + Environment.NewLine;
		}
	}
	/// <summary>
	/// Class that allows comparison of two objects of the same type to each other.  Supports classes, lists, arrays, dictionaries, child comparison and more.
	/// </summary>
	/// <example>
	/// CompareLogic compareLogic = new CompareLogic();
	///
	/// Person person1 = new Person();
	/// person1.DateCreated = DateTime.Now;
	/// person1.Name = "Greg";
	///
	/// Person person2 = new Person();
	/// person2.Name = "John";
	/// person2.DateCreated = person1.DateCreated;
	///
	/// ComparisonResult result = compareLogic.Compare(person1, person2);
	///
	/// if (!result.AreEqual)
	///    Console.WriteLine(result.DifferencesString);
	///
	/// </example>
	public class CompareLogic : ICompareLogic
	{
		private ComparisonConfig _config;

		/// <summary>
		/// The default configuration
		/// </summary>
		public ComparisonConfig Config
		{
			get
			{
				return _config;
			}
			set
			{
				new VerifyConfig().Verify(value);
				_config = value;
			}
		}

		/// <summary>
		/// Set up defaults for the comparison
		/// </summary>
		public CompareLogic()
		{
			Config = new ComparisonConfig();
		}

		/// <summary>
		/// Pass in the configuration
		/// </summary>
		/// <param name="config"></param>
		public CompareLogic(ComparisonConfig config)
		{
			Config = config;
		}

		/// <summary>
		/// Set up defaults for the comparison
		/// </summary>
		/// <param name="useAppConfigSettings">If true, use settings from the app.config</param>
		public CompareLogic(bool useAppConfigSettings)
		{
			Config = new ComparisonConfig();
			if (useAppConfigSettings)
			{
				SetupWithAppConfigSettings();
			}
		}

		private void SetupWithAppConfigSettings()
		{
			Config.MembersToIgnore = new List<string>();
			Config.CompareStaticFields = true;
			Config.CompareStaticProperties = true;
			Config.ComparePrivateProperties = false;
			Config.ComparePrivateFields = false;
			Config.CompareChildren = true;
			Config.CompareReadOnly = true;
			Config.CompareFields = true;
			Config.CompareProperties = true;
			Config.Caching = true;
			Config.AutoClearCache = true;
			Config.MaxDifferences = 1;
			Config.IgnoreCollectionOrder = false;
			Config.IgnoreUnknownObjectTypes = false;
			Config.IgnoreObjectDisposedException = false;
		}

		/// <summary>
		/// Compare two objects of the same type to each other.
		/// </summary>
		/// <remarks>
		/// Check the Differences or DifferencesString Properties for the differences.
		/// Default MaxDifferences is 1 for performance
		/// </remarks>
		/// <param name="expectedObject">The expected object value to compare</param>
		/// <param name="actualObject">The actual object value to compare</param>
		/// <returns>True if they are equal</returns>
		public ComparisonResult Compare(object expectedObject, object actualObject)
		{
			ComparisonResult comparisonResult = new ComparisonResult(Config);
			comparisonResult.Watch.Start();
			Config.PopulateHashSets();
			RootComparer rootComparer = RootComparerFactory.GetRootComparer();
			CompareParms parms = new CompareParms
			{
				Config = Config,
				Result = comparisonResult,
				Object1 = expectedObject,
				Object2 = actualObject,
				BreadCrumb = string.Empty
			};
			rootComparer.Compare(parms);
			if (Config.AutoClearCache)
			{
				ClearCache();
			}
			comparisonResult.Watch.Stop();
			return comparisonResult;
		}

		/// <summary>
		/// Reflection properties and fields are cached. By default this cache is cleared automatically after each compare.
		/// </summary>
		public void ClearCache()
		{
			Cache.ClearCache();
		}

		/// <summary>
		/// Save the current configuration to the passed stream
		/// </summary>
		/// <param name="stream"></param>
		public void SaveConfiguration(Stream stream)
		{
			new DataContractJsonSerializer(typeof(ComparisonConfig)).WriteObject(stream, Config);
			if (stream.CanSeek && stream.Position > 0)
			{
				stream.Seek(0L, SeekOrigin.Begin);
			}
		}

		/// <summary>
		/// Load the current configuration from a json stream
		/// </summary>
		/// <param name="stream"></param>
		public void LoadConfiguration(Stream stream)
		{
			if (stream.CanSeek && stream.Position > 0)
			{
				stream.Seek(0L, SeekOrigin.Begin);
			}
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(ComparisonConfig));
			Config = (ComparisonConfig)dataContractJsonSerializer.ReadObject(stream);
		}

		/// <summary>
		/// Load the current configuration from a json stream
		/// </summary>
		/// <param name="filePath"></param>
		public void LoadConfiguration(string filePath)
		{
			using FileStream stream = new FileStream(filePath, FileMode.Open);
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(ComparisonConfig));
			Config = (ComparisonConfig)dataContractJsonSerializer.ReadObject(stream);
			Config.DifferenceCallback = delegate
			{
			};
		}

		/// <summary>
		/// Save the current configuration to a json file
		/// </summary>
		/// <param name="filePath"></param>
		public void SaveConfiguration(string filePath)
		{
			using FileStream stream = new FileStream(filePath, FileMode.Create);
			new DataContractJsonSerializer(typeof(ComparisonConfig)).WriteObject(stream, Config);
		}
	}
	/// <summary>
	/// Obsolete Use CompareLogic instead
	/// </summary>
	[Obsolete("Use CompareLogic instead", true)]
	public class CompareObjects
	{
		private readonly CompareLogic _logic;

		private ComparisonResult _result;

		/// <summary>
		/// Obsolete Use the ComparisonResult.ElapsedMilliseconds returned from CompareLogic.Compare
		/// </summary>
		[Obsolete("Use the ComparisonResult.ElapsedMilliseconds returned from CompareLogic.Compare", true)]
		public long ElapsedMilliseconds => _result.ElapsedMilliseconds;

		/// <summary>
		/// Obsolete Use CompareLogic.Config.ShowBreadcrumb instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.ShowBreadcrumb instead", true)]
		public bool ShowBreadcrumb
		{
			get
			{
				return _logic.Config.ShowBreadcrumb;
			}
			set
			{
				_logic.Config.ShowBreadcrumb = value;
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.MembersToIgnore for members or CompareLogic.Config.ClassTypesToIgnore instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.MembersToIgnore for members or CompareLogic.Config.ClassTypesToIgnore instead", true)]
		public List<string> ElementsToIgnore
		{
			get
			{
				return _logic.Config.MembersToIgnore.ToList();
			}
			set
			{
				_logic.Config.MembersToIgnore.Clear();
				foreach (string item in value)
				{
					_logic.Config.MembersToIgnore.Add(item);
				}
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.MembersToInclude or CompareLogic.Config.ClassTypesToInclude instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.MembersToInclude or CompareLogic.Config.ClassTypesToInclude instead", true)]
		public List<string> ElementsToInclude
		{
			get
			{
				return _logic.Config.MembersToInclude.ToList();
			}
			set
			{
				_logic.Config.MembersToInclude.Clear();
				foreach (string item in value)
				{
					_logic.Config.MembersToInclude.Add(item);
				}
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.ComparePrivateProperties instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.ComparePrivateProperties instead", true)]
		public bool ComparePrivateProperties
		{
			get
			{
				return _logic.Config.ComparePrivateProperties;
			}
			set
			{
				_logic.Config.ComparePrivateProperties = value;
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.ComparePrivateFields instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.ComparePrivateFields instead", true)]
		public bool ComparePrivateFields
		{
			get
			{
				return _logic.Config.ComparePrivateFields;
			}
			set
			{
				_logic.Config.ComparePrivateFields = value;
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.CompareStaticProperties instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.CompareStaticProperties instead", true)]
		public bool CompareStaticProperties
		{
			get
			{
				return _logic.Config.CompareStaticProperties;
			}
			set
			{
				_logic.Config.CompareStaticProperties = value;
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.CompareStaticFields instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.CompareStaticFields instead", true)]
		public bool CompareStaticFields
		{
			get
			{
				return _logic.Config.CompareStaticFields;
			}
			set
			{
				_logic.Config.CompareStaticFields = value;
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.CompareChildren instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.CompareChildren instead", true)]
		public bool CompareChildren
		{
			get
			{
				return _logic.Config.CompareChildren;
			}
			set
			{
				_logic.Config.CompareChildren = value;
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.CompareReadOnly instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.CompareReadOnly instead", true)]
		public bool CompareReadOnly
		{
			get
			{
				return _logic.Config.CompareReadOnly;
			}
			set
			{
				_logic.Config.CompareReadOnly = value;
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.CompareFields instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.CompareFields instead", true)]
		public bool CompareFields
		{
			get
			{
				return _logic.Config.CompareFields;
			}
			set
			{
				_logic.Config.CompareFields = value;
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.IgnoreCollectionOrder instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.IgnoreCollectionOrder instead", true)]
		public bool IgnoreCollectionOrder
		{
			get
			{
				return _logic.Config.IgnoreCollectionOrder;
			}
			set
			{
				_logic.Config.IgnoreCollectionOrder = value;
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.CompareProperties instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.CompareProperties instead", true)]
		public bool CompareProperties
		{
			get
			{
				return _logic.Config.CompareProperties;
			}
			set
			{
				_logic.Config.CompareProperties = value;
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.MaxDifferences instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.MaxDifferences instead", true)]
		public int MaxDifferences
		{
			get
			{
				return _logic.Config.MaxDifferences;
			}
			set
			{
				_logic.Config.MaxDifferences = value;
			}
		}

		/// <summary>
		/// Obsolete Use the ComparisonResult.Differences returned from CompareLogic.Compare
		/// </summary>
		[Obsolete("Use the ComparisonResult.Differences returned from CompareLogic.Compare", true)]
		public List<Difference> Differences
		{
			get
			{
				return _result.Differences;
			}
			set
			{
				_result.Differences = value;
			}
		}

		/// <summary>
		/// Obsolete Use the ComparisonResult.DifferencesString returned from CompareLogic.Compare
		/// </summary>
		[Obsolete("Use the ComparisonResult.DifferencesString returned from CompareLogic.Compare", true)]
		public string DifferencesString => _result.DifferencesString;

		/// <summary>
		/// Obsolete Use CompareLogic.Config.AutoClearCache instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.AutoClearCache instead", true)]
		public bool AutoClearCache
		{
			get
			{
				return _logic.Config.AutoClearCache;
			}
			set
			{
				_logic.Config.AutoClearCache = value;
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.Caching instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.Caching instead", true)]
		public bool Caching
		{
			get
			{
				return _logic.Config.Caching;
			}
			set
			{
				_logic.Config.Caching = value;
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.AttributesToIgnore instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.AttributesToIgnore instead", true)]
		public List<Type> AttributesToIgnore
		{
			get
			{
				return _logic.Config.AttributesToIgnore.ToList();
			}
			set
			{
				_logic.Config.AttributesToIgnore.Clear();
				foreach (Type item in value)
				{
					_logic.Config.AttributesToIgnore.Add(item);
				}
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.IgnoreObjectTypes instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.IgnoreObjectTypes instead", true)]
		public bool IgnoreObjectTypes
		{
			get
			{
				return _logic.Config.IgnoreObjectTypes;
			}
			set
			{
				_logic.Config.IgnoreObjectTypes = value;
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.CustomComparers instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.CustomComparers", true)]
		public Func<Type, bool> IsUseCustomTypeComparer { get; set; }

		/// <summary>
		/// Obsolete Use CompareLogic.Config.CustomComparers instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.CustomComparers", true)]
		public Action<CompareObjects, object, object, string> CustomComparer { get; set; }

		/// <summary>
		/// Obsolete Use CompareLogic.Config.ExpectedName instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.ExpectedName instead", true)]
		public string ExpectedName
		{
			get
			{
				return _logic.Config.ExpectedName;
			}
			set
			{
				_logic.Config.ExpectedName = value;
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.ActualName instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.ActualName instead", true)]
		public string ActualName
		{
			get
			{
				return _logic.Config.ActualName;
			}
			set
			{
				_logic.Config.ActualName = value;
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.DifferenceCallback instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.DifferenceCallback instead", true)]
		public Action<Difference> DifferenceCallback
		{
			get
			{
				return _logic.Config.DifferenceCallback;
			}
			set
			{
				_logic.Config.DifferenceCallback = value;
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Config.CollectionMatchingSpec instead
		/// </summary>
		[Obsolete("Use CompareLogic.Config.CollectionMatchingSpec instead", true)]
		public Dictionary<Type, IEnumerable<string>> CollectionMatchingSpec
		{
			get
			{
				return _logic.Config.CollectionMatchingSpec;
			}
			set
			{
				_logic.Config.CollectionMatchingSpec = value;
			}
		}

		/// <summary>
		/// Obsolete Use CompareLogic instead
		/// </summary>
		[Obsolete("Use CompareLogic instead", true)]
		public CompareObjects()
		{
			_logic = new CompareLogic();
			_result = new ComparisonResult(_logic.Config);
		}

		/// <summary>
		/// Obsolete Use CompareLogic instead
		/// </summary>
		[Obsolete("Use CompareLogic instead", true)]
		public CompareObjects(bool useAppConfigSettings)
		{
			_logic = new CompareLogic(useAppConfigSettings);
			_result = new ComparisonResult(_logic.Config);
		}

		/// <summary>
		/// Obsolete Use CompareLogic.Compare instead
		/// </summary>
		[Obsolete("Use CompareLogic.Compare instead", true)]
		public bool Compare(object object1, object object2)
		{
			_result = _logic.Compare(object1, object2);
			return _result.AreEqual;
		}

		/// <summary>
		/// Obsolete Use CompareLogic.ClearCache instead
		/// </summary>
		[Obsolete("Use CompareLogic.ClearCache instead", true)]
		public void ClearCache()
		{
			_logic.ClearCache();
		}
	}
	/// <summary>
	/// Compare Parameters
	/// </summary>
	public class CompareParms
	{
		/// <summary>
		/// The configuration settings
		/// </summary>
		public ComparisonConfig Config { get; set; }

		/// <summary>
		/// The type of the first object
		/// </summary>
		public Type Object1Type { get; set; }

		/// <summary>
		/// The type of the second object
		/// </summary>
		public Type Object2Type { get; set; }

		public Type Object1DeclaredType { get; set; }

		public Type Object2DeclaredType { get; set; }

		/// <summary>
		/// Details about the comparison
		/// </summary>
		public ComparisonResult Result { get; set; }

		/// <summary>
		/// A reference to the parent object1
		/// </summary>
		public object ParentObject1 { get; set; }

		/// <summary>
		/// A reference to the parent object2
		/// </summary>
		public object ParentObject2 { get; set; }

		/// <summary>
		/// The first object to be compared
		/// </summary>
		public object Object1 { get; set; }

		/// <summary>
		/// The second object to be compared
		/// </summary>
		public object Object2 { get; set; }

		/// <summary>
		/// The breadcrumb in the tree
		/// </summary>
		public string BreadCrumb { get; set; }

		/// <summary>
		/// Custom comparer used to assert <para>Object1</para>
		/// </summary>
		public BaseTypeComparer CustomPropertyComparer { get; set; }
	}
	/// <summary>
	/// Configuration
	/// </summary>
	[DataContract]
	public class ComparisonConfig
	{
		private Action<Difference> _differenceCallback;

		private int _maxStructDepth;

		/// <summary>
		/// Backing member that supports <see cref="M:Snet.Utility.comparer.ComparisonConfig.HasWildcardMembersToExclude" />
		/// </summary>
		private bool? _hasWildcardInMembersToIgnore;

		internal HashSet<Type> AttributesToIgnoreSet { get; set; }

		internal HashSet<string> MembersToIgnoreSet { get; set; }

		internal HashSet<string> MembersToIncludeSet { get; set; }

		internal HashSet<Type> ClassTypesToIgnoreSet { get; set; }

		internal HashSet<Type> ClassTypesToIncludeSet { get; set; }

		internal HashSet<Type> TypesToIgnoreSet { get; set; }

		internal HashSet<Type> TypesToIncludeSet { get; set; }

		internal HashSet<Type> RequiredAttributesToCompareSet { get; set; }

		/// <summary>
		/// By default Compare .NET Objects uses reference equal to identify objects.
		/// Versions 4.61 and older used the hash code.  Setting this to true will identify objects by hash code instead of reference equals.
		/// The default is false
		/// </summary>
		public bool UseHashCodeIdentifier { get; set; }

		/// <summary>
		/// When comparing strings or StringBuilder types, perform a case sensitive comparison.  The default is true.
		/// </summary>
		[DataMember]
		public bool CaseSensitive { get; set; }

		/// <summary>
		/// Ignore exceptions when objects are disposed
		/// </summary>
		[DataMember]
		public bool IgnoreObjectDisposedException { get; set; }

		/// <summary>
		/// Ignore millisecond differences between DateTime values or DateTimeOffset values.  The default is 0 (any time difference will be shown).
		/// </summary>
		[DataMember]
		public int MaxMillisecondsDateDifference { get; set; }

		/// <summary>
		/// When comparing DateTimeOffsets, offsets will be compared as well as the UtcDateTimes. The default is false.
		/// </summary>
		[DataMember]
		public bool CompareDateTimeOffsetWithOffsets { get; set; }

		/// <summary>
		/// When comparing DateTimeOffsets, timezone difference will be ignored by changing both object to their UTC equivalent value. The default is false.
		/// </summary>
		[DataMember]
		public bool IgnoreDateTimeOffsetTimezones { get; set; }

		/// <summary>
		/// When comparing struct, the depth to compare for children.  The default is 2, the max is 5
		/// </summary>
		[DataMember]
		public int MaxStructDepth
		{
			get
			{
				return _maxStructDepth;
			}
			set
			{
				if (value < 1 || value > 5)
				{
					throw new ArgumentOutOfRangeException("MaxStructDepth", "Cannot be less than 1 or greater than 5");
				}
				_maxStructDepth = value;
			}
		}

		/// <summary>
		/// If true, unknown object types will be ignored instead of throwing an exception.  The default is false.
		/// </summary>
		[DataMember]
		public bool IgnoreUnknownObjectTypes { get; set; }

		/// <summary>
		/// If true, invalid indexers will be skipped.  The default is false.
		/// </summary>
		[DataMember]
		public bool SkipInvalidIndexers { get; set; }

		/// <summary>
		/// If a class implements an interface then only members of the interface will be compared.  The default is all members are compared. 
		/// </summary>
		public List<Type> InterfaceMembers { get; set; }

		[DataMember(Name = "InterfaceMembers")]
		private List<string> InterfaceMembersSerializer
		{
			get
			{
				return TypeHelper.ListOfTypesSerializer(InterfaceMembers);
			}
			set
			{
				InterfaceMembers = TypeHelper.ListOfTypesDeserializer(value);
			}
		}

		/// <summary>
		/// Show breadcrumb at each stage of the comparision.  The default is false.
		/// This is useful for debugging deep object graphs.
		/// </summary>
		[DataMember]
		public bool ShowBreadcrumb { get; set; }

		/// <summary>
		/// A list of class types to be ignored in the comparison. The default is to compare all class types.
		/// </summary>
		public List<Type> ClassTypesToIgnore { get; set; }

		[DataMember(Name = "ClassTypesToIgnore")]
		private List<string> ClassTypesToIgnoreSerializer
		{
			get
			{
				return TypeHelper.ListOfTypesSerializer(ClassTypesToIgnore);
			}
			set
			{
				ClassTypesToIgnore = TypeHelper.ListOfTypesDeserializer(value);
			}
		}

		/// <summary>
		/// Only these class types will be compared. The default is to compare all class types.
		/// </summary>
		/// <remarks>If you specify a class type here no other class types will be compared unless it is in this list.</remarks>
		public List<Type> ClassTypesToInclude { get; set; }

		[DataMember(Name = "ClassTypesToInclude")]
		private List<string> ClassTypesToIncludeSerializer
		{
			get
			{
				return TypeHelper.ListOfTypesSerializer(ClassTypesToInclude);
			}
			set
			{
				ClassTypesToInclude = TypeHelper.ListOfTypesDeserializer(value);
			}
		}

		/// <summary>
		/// A list of types to be ignored in the comparison. The default is to compare all types.  A typical thing to not compare are GUIDs
		/// </summary>
		public List<Type> TypesToIgnore { get; set; }

		[DataMember(Name = "TypesToIgnore")]
		private List<string> TypesToIgnoreSerializer
		{
			get
			{
				return TypeHelper.ListOfTypesSerializer(TypesToIgnore);
			}
			set
			{
				TypesToIgnore = TypeHelper.ListOfTypesDeserializer(value);
			}
		}

		/// <summary>
		/// Only these types will be compared. The default is to compare all types.
		/// </summary>
		/// <remarks>If you specify a type here no others will be compared unless it is in this list.  You must specify ALL Types that you want to compare.</remarks>
		public List<Type> TypesToInclude { get; set; }

		[DataMember(Name = "TypesToInclude")]
		private List<string> TypesToIncludeSerializer
		{
			get
			{
				return TypeHelper.ListOfTypesSerializer(TypesToInclude);
			}
			set
			{
				TypesToInclude = TypeHelper.ListOfTypesDeserializer(value);
			}
		}

		/// <summary>
		/// Ignore Data Table Names, Data Table Column Names, properties, or fields by name during the comparison. Case sensitive. The default is to compare all members.
		/// </summary>
		/// <example>MembersToIgnore.Add("CreditCardNumber");
		/// MembersToIgnore.Add("Invoice.InvoiceGuid");
		/// MembersToIgnore.Add("*Id");
		/// </example>
		[DataMember]
		public List<string> MembersToIgnore { get; set; }

		/// <summary>
		/// Only compare elements by name for Data Table Names, Data Table Column Names, properties and fields. Case sensitive. The default is to compare all members.
		/// </summary>
		/// <example>MembersToInclude.Add("FirstName")</example>
		[DataMember]
		public List<string> MembersToInclude { get; set; }

		/// <summary>
		/// If true, private properties and fields will be compared. The default is false.  Silverlight and WinRT restricts access to private variables.
		/// </summary>
		[DataMember]
		public bool ComparePrivateProperties { get; set; }

		/// <summary>
		/// If true, private fields will be compared. The default is false.  Silverlight and WinRT restricts access to private variables.
		/// </summary>
		[DataMember]
		public bool ComparePrivateFields { get; set; }

		/// <summary>
		/// If true and <see cref="P:Snet.Utility.comparer.ComparisonConfig.ComparePrivateFields" /> true, then backing fields will be compared. The default is true.  Silverlight and WinRT restricts access to private variables.
		/// </summary>
		[DataMember]
		public bool CompareBackingFields { get; set; }

		/// <summary>
		/// If true, static properties will be compared.  The default is true.
		/// </summary>
		[DataMember]
		public bool CompareStaticProperties { get; set; }

		/// <summary>
		/// If true, static fields will be compared.  The default is true.
		/// </summary>
		[DataMember]
		public bool CompareStaticFields { get; set; }

		/// <summary>
		/// If true, child objects will be compared. The default is true. 
		/// If false, and a list or array is compared list items will be compared but not their children.
		/// </summary>
		[DataMember]
		public bool CompareChildren { get; set; }

		/// <summary>
		/// If true, compare read only properties (only the getter is implemented). The default is true.
		/// </summary>
		[DataMember]
		public bool CompareReadOnly { get; set; }

		/// <summary>
		/// If true, compare fields of a class (see also CompareProperties).  The default is true.
		/// </summary>
		[DataMember]
		public bool CompareFields { get; set; }

		/// <summary>
		/// If true, compare each item within a collection to every item in the other.  The default is false. WARNING: setting this to true significantly impacts performance.  
		/// </summary>
		[DataMember]
		public bool IgnoreCollectionOrder { get; set; }

		/// <summary>
		/// If true, breadcrumb shows the name of the key for each item in a collection. If false, will only show the key value. Default is true.
		/// </summary>
		[DataMember]
		public bool ShowCollectionKeyName { get; set; }

		/// <summary>
		/// If true, compare properties of a class (see also CompareFields).  The default is true.
		/// </summary>
		[DataMember]
		public bool CompareProperties { get; set; }

		/// <summary>
		/// The maximum number of differences to detect.  The default is 1 for performance reasons.
		/// </summary>
		[DataMember]
		public int MaxDifferences { get; set; }

		/// <summary>
		/// The maximum number of differences to detect when comparing byte arrays.  The default is 1.
		/// </summary>
		[DataMember]
		public int MaxByteArrayDifferences { get; set; }

		/// <summary>
		/// Reflection properties and fields are cached. By default this cache is cleared after each compare.  Set to false to keep the cache for multiple compares.
		/// </summary>
		/// <seealso cref="P:Snet.Utility.comparer.ComparisonConfig.Caching" />
		[DataMember]
		public bool AutoClearCache { get; set; }

		/// <summary>
		/// By default properties and fields for types are cached for each compare.  By default this cache is cleared after each compare.
		/// </summary>
		/// <seealso cref="P:Snet.Utility.comparer.ComparisonConfig.AutoClearCache" />
		[DataMember]
		public bool Caching { get; set; }

		/// <summary>
		/// A list of attributes to ignore a class, property or field
		/// </summary>
		/// <example>AttributesToIgnore.Add(typeof(XmlIgnoreAttribute));</example>
		public List<Type> AttributesToIgnore { get; set; }

		[DataMember(Name = "AttributesToIgnore")]
		private List<string> AttributesToIgnoreSerializer
		{
			get
			{
				return TypeHelper.ListOfTypesSerializer(AttributesToIgnore);
			}
			set
			{
				AttributesToIgnore = TypeHelper.ListOfTypesDeserializer(value);
			}
		}

		/// <summary>
		/// If a property or field don't have at least one of the attributes in this list, it will be ignored
		/// </summary>
		/// <example>RequiredAttributesToCompare.Add(typeof(XmlIgnoreAttribute));</example>
		public List<Type> RequiredAttributesToCompare { get; set; }

		[DataMember(Name = "RequiredAttributesToCompare")]
		private List<string> RequiredAttributesToCompareSerializer
		{
			get
			{
				return TypeHelper.ListOfTypesSerializer(RequiredAttributesToCompare);
			}
			set
			{
				RequiredAttributesToCompare = TypeHelper.ListOfTypesDeserializer(value);
			}
		}

		/// <summary>
		/// If true, objects will be compared ignore their type diferences.  The default is false.
		/// </summary>
		[DataMember]
		public bool IgnoreObjectTypes { get; set; }

		/// <summary>
		/// In the differences string, this is the name for expected name. The default is: Expected 
		/// </summary>
		[DataMember]
		public string ExpectedName { get; set; }

		/// <summary>
		/// In the differences string, this is the name for the actual name. The default is: Actual
		/// </summary>
		[DataMember]
		public string ActualName { get; set; }

		/// <summary>
		/// Callback invoked each time the comparer finds a difference. The default is no call back.
		/// </summary>
		public Action<Difference> DifferenceCallback
		{
			get
			{
				return _differenceCallback;
			}
			set
			{
				if (value != null)
				{
					_differenceCallback = value;
				}
			}
		}

		/// <summary>
		/// This property is used when IgnoreCollectionOrder is set to true, otherwise it has no effect.
		/// Sometimes one wants to match items between collections by some key first, and then
		/// compare the matched objects.  Without this, the comparer basically says there is no 
		/// match in collection B for any given item in collection A that doesn't Compare with a result of true.  
		/// The results of this aren't particularly useful for object graphs that are mostly the same, but not quite. 
		/// Enter CollectionMatchingSpec
		///
		/// The enumerable strings should be property (not field, for now, to keep it simple) names of the
		/// Type when encountered that will be used for matching
		///
		/// You can use complex type properties, too, as part of the key to match.  To match on all props/fields on 
		/// such a matching key, Don't set this property (default comparer behavior)
		/// NOTE: types are looked up as exact.  e.g. if foo is an entry in the dictionary and bar is a 
		/// sub-class of foo, upon encountering a bar type, the comparer will not find the entry of foo.
		/// It can use derived types:  https://github.com/GregFinzer/Compare-Net-Objects/issues/280 
		/// </summary>
		[DataMember]
		public Dictionary<Type, IEnumerable<string>> CollectionMatchingSpec { get; set; }

		/// <summary>
		/// A list of custom comparers that take priority over the built in comparers
		/// </summary>
		[DataMember]
		public List<BaseTypeComparer> CustomComparers { get; set; }

		/// <summary>
		/// A list of custom property comparers that take priority over the built in and type comparers 
		/// </summary>
		[DataMember]
		public Dictionary<string, BaseTypeComparer> CustomPropertyComparers { get; set; }

		/// <summary>
		/// If true, string.empty and null will be treated as equal for Strings and String Builder. The default is false.
		/// </summary>
		[DataMember]
		public bool TreatStringEmptyAndNullTheSame { get; set; }

		/// <summary>
		/// If true, leading and trailing whitespaces will be ignored for Strings and String Builder. The default is false.
		/// </summary>
		[DataMember]
		public bool IgnoreStringLeadingTrailingWhitespace { get; set; }

		/// <summary>
		/// The precision to compare double values.  The default is 0.
		/// </summary>
		[DataMember]
		public double DoublePrecision { get; set; }

		/// <summary>
		/// The precision to compare decimal values.  The default is 0.
		/// </summary>
		[DataMember]
		public decimal DecimalPrecision { get; set; }

		/// <summary>
		/// When comparing Fields and Properties the types have to be the same.  If IgnoreConcretTypes is true then they will be ignored.  The default is false.
		/// </summary>
		[DataMember]
		public bool IgnoreConcreteTypes { get; set; }

		/// <summary>
		/// If true, properties that are defined in the actual object but missing in the expected object will not be flagged as differences. Default is true.
		/// </summary>
		[DataMember]
		public bool IgnoreMissingProperties { get; set; }

		/// <summary>
		/// If true, fields that are defined in the actual object but missing in the expected object will not be flagged as differences. Default is true.
		/// </summary>
		[DataMember]
		public bool IgnoreMissingFields { get; set; }

		/// <summary>
		/// Specify a DateTimeKind to use when it is unspecified.  The default is DateTimeKind.Utc.  If null is specified it will not be defaulted.
		/// </summary>
		[DataMember]
		public DateTimeKind? DateTimeKindToUseWhenUnspecified { get; set; }

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ComparisonConfig()
		{
			Reset();
		}

		/// <summary>
		/// Ignore property during the comparison.  Property is specific to the generic type.
		/// </summary>
		/// <param name="ignoredProperty"></param>
		/// <typeparam name="TClass"></typeparam>
		/// <exception cref="T:System.ArgumentException"></exception>
		/// <example>IgnoreProperty&lt;Person&gt;(x =&gt; x.Name)</example>
		public void IgnoreProperty<TClass>(Expression<Func<TClass, object>> ignoredProperty)
		{
			MemberExpression memberExpression = ((!(ignoredProperty.Body is UnaryExpression unaryExpression)) ? ((MemberExpression)ignoredProperty.Body) : ((unaryExpression.Operand as MemberExpression) ?? throw new ArgumentException($"IgnoreProperty can only be used with properties. {ignoredProperty} is not a property.")));
			PropertyInfo obj = memberExpression.Member as PropertyInfo;
			if (obj == null)
			{
				throw new ArgumentException($"IgnoreProperty can only be used with properties. {ignoredProperty} is not a property.");
			}
			string name = obj.Name;
			MembersToIgnore.Add(typeof(TClass).Name + "." + name);
		}

		/// <summary>
		/// Define a Custom Property Comparer using a lambda expression
		/// </summary>
		/// <typeparam name="TClass"></typeparam>
		/// <param name="customProperty"></param>
		/// <param name="validator"></param>
		public void CustomPropertyComparer<TClass>(Expression<Func<TClass, object>> customProperty, BaseTypeComparer validator)
		{
			MemberExpression memberExpression = ((!(customProperty.Body is UnaryExpression unaryExpression)) ? ((MemberExpression)customProperty.Body) : ((unaryExpression.Operand as MemberExpression) ?? throw new ArgumentException($"Custom property comparer can only be used with properties. {customProperty} is not a property.")));
			PropertyInfo obj = memberExpression.Member as PropertyInfo;
			if (obj == null)
			{
				throw new ArgumentException($"Custom property comparer can only be used with properties. {customProperty} is not a property.");
			}
			string name = obj.Name;
			CustomPropertyComparers.Add(typeof(TClass).Name + "." + name, validator);
		}

		internal void PopulateHashSets()
		{
			AttributesToIgnoreSet = new HashSet<Type>((AttributesToIgnore ?? new List<Type>()).Distinct());
			MembersToIgnoreSet = new HashSet<string>((MembersToIgnore ?? new List<string>()).Distinct());
			MembersToIncludeSet = new HashSet<string>((MembersToInclude ?? new List<string>()).Distinct());
			ClassTypesToIgnoreSet = new HashSet<Type>((ClassTypesToIgnore ?? new List<Type>()).Distinct());
			ClassTypesToIncludeSet = new HashSet<Type>((ClassTypesToInclude ?? new List<Type>()).Distinct());
			TypesToIgnoreSet = new HashSet<Type>((TypesToIgnore ?? new List<Type>()).Distinct());
			TypesToIncludeSet = new HashSet<Type>((TypesToInclude ?? new List<Type>()).Distinct());
			RequiredAttributesToCompareSet = new HashSet<Type>((RequiredAttributesToCompare ?? new List<Type>()).Distinct());
		}

		/// <summary>
		/// Computed value of whether or not exclusion list has wildcards.
		/// </summary>
		public bool HasWildcardMembersToExclude()
		{
			if (_hasWildcardInMembersToIgnore.HasValue)
			{
				return _hasWildcardInMembersToIgnore.Value;
			}
			_hasWildcardInMembersToIgnore = MembersToIgnoreSet.Any((string x) => x.IndexOf("*") > -1);
			return _hasWildcardInMembersToIgnore.Value;
		}

		/// <summary>
		/// Reset the configuration to the default values
		/// </summary>
		public void Reset()
		{
			AttributesToIgnore = new List<Type>();
			RequiredAttributesToCompare = new List<Type>();
			_differenceCallback = delegate
			{
			};
			MembersToIgnore = new List<string>();
			_hasWildcardInMembersToIgnore = null;
			MembersToInclude = new List<string>();
			ClassTypesToIgnore = new List<Type>();
			ClassTypesToInclude = new List<Type>();
			TypesToIgnore = new List<Type>();
			TypesToInclude = new List<Type>();
			CompareStaticFields = true;
			CompareStaticProperties = true;
			ComparePrivateProperties = false;
			ComparePrivateFields = false;
			CompareBackingFields = true;
			CustomPropertyComparers = new Dictionary<string, BaseTypeComparer>();
			CompareChildren = true;
			CompareReadOnly = true;
			CompareFields = true;
			CompareDateTimeOffsetWithOffsets = false;
			IgnoreCollectionOrder = false;
			CompareProperties = true;
			ShowCollectionKeyName = true;
			Caching = true;
			AutoClearCache = true;
			IgnoreObjectTypes = false;
			MaxDifferences = 1;
			ExpectedName = "Expected";
			ActualName = "Actual";
			CustomComparers = new List<BaseTypeComparer>();
			TreatStringEmptyAndNullTheSame = false;
			InterfaceMembers = new List<Type>();
			SkipInvalidIndexers = false;
			MaxByteArrayDifferences = 1;
			CollectionMatchingSpec = new Dictionary<Type, IEnumerable<string>>();
			IgnoreUnknownObjectTypes = false;
			MaxStructDepth = 2;
			CaseSensitive = true;
			IgnoreStringLeadingTrailingWhitespace = false;
			IgnoreMissingProperties = true;
			IgnoreMissingFields = true;
			DateTimeKindToUseWhenUnspecified = DateTimeKind.Utc;
		}
	}
	/// <summary>
	/// Details about the comparison
	/// </summary>
	public class ComparisonResult
	{
		private string _differencesString;

		/// <summary>
		/// Keep track of parent objects in the object hierarchy by using reference equals
		/// </summary>
		private readonly Dictionary<object, int> _referenceParents = new Dictionary<object, int>();

		private readonly Dictionary<int, int> _hashParents = new Dictionary<int, int>();

		/// <summary>
		/// Keep track of the depth of structs being compared.  Used with ComparisonConfig.MaxStructDepth
		/// </summary>
		internal int CurrentStructDepth { get; set; }

		/// <summary>
		/// Configuration
		/// </summary>
		public ComparisonConfig Config { get; private set; }

		/// <summary>
		/// Used to time how long the comparison took
		/// </summary>
		internal Stopwatch Watch { get; set; }

		/// <summary>
		/// The amount of time in milliseconds it took for the comparison
		/// </summary>
		public long ElapsedMilliseconds => Watch.ElapsedMilliseconds;

		/// <summary>
		/// The differences found during the compare
		/// </summary>
		public List<Difference> Differences { get; set; }

		/// <summary>
		/// The differences found in a string suitable for a textbox
		/// </summary>
		public string DifferencesString
		{
			get
			{
				if (string.IsNullOrEmpty(_differencesString))
				{
					StringBuilder stringBuilder = new StringBuilder(4096);
					stringBuilder.AppendLine();
					stringBuilder.AppendFormat("Begin Differences ({0} differences):{1}", Differences.Count, Environment.NewLine);
					foreach (Difference difference in Differences)
					{
						stringBuilder.AppendLine(difference.ToString());
					}
					stringBuilder.AppendFormat("End Differences (Maximum of {0} differences shown).", Config.MaxDifferences);
					_differencesString = stringBuilder.ToString();
				}
				return _differencesString;
			}
			set
			{
				_differencesString = value;
			}
		}

		/// <summary>
		/// Returns true if the objects are equal
		/// </summary>
		public bool AreEqual => Differences.Count == 0;

		/// <summary>
		/// Returns true if the number of differences has reached the maximum
		/// </summary>
		public bool ExceededDifferences => Differences.Count >= Config.MaxDifferences;

		/// <summary>
		/// Set the configuration for the comparison
		/// </summary>
		/// <param name="config"></param>
		public ComparisonResult(ComparisonConfig config)
		{
			Config = config;
			Differences = new List<Difference>();
			Watch = new Stopwatch();
		}

		/// <summary>
		/// Add parent, handle references count
		/// </summary>
		/// <param name="objectReference"></param>
		public void AddParent(object objectReference)
		{
			if (objectReference == null || (Config.UseHashCodeIdentifier && _hashParents == null) || (!Config.UseHashCodeIdentifier && _referenceParents == null))
			{
				return;
			}
			if (Config.UseHashCodeIdentifier)
			{
				int hashCode = objectReference.GetHashCode();
				if (hashCode != 0)
				{
					if (!_hashParents.ContainsKey(hashCode))
					{
						_hashParents.Add(hashCode, 1);
					}
					else
					{
						_hashParents[hashCode]++;
					}
				}
			}
			else if (!_referenceParents.ContainsKey(objectReference))
			{
				_referenceParents.Add(objectReference, 0);
			}
			else
			{
				_referenceParents[objectReference]++;
			}
		}

		/// <summary>
		/// Remove parent, handle references count
		/// </summary>
		/// <param name="objectReference"></param>
		public void RemoveParent(object objectReference)
		{
			if (objectReference == null || (Config.UseHashCodeIdentifier && _hashParents == null) || (!Config.UseHashCodeIdentifier && _referenceParents == null))
			{
				return;
			}
			try
			{
				if (Config.UseHashCodeIdentifier)
				{
					int hashCode = objectReference.GetHashCode();
					if (_hashParents.ContainsKey(hashCode))
					{
						if (_hashParents[hashCode] <= 1)
						{
							_hashParents.Remove(hashCode);
						}
						else
						{
							_hashParents[hashCode]--;
						}
					}
				}
				else if (_referenceParents.ContainsKey(objectReference))
				{
					if (_referenceParents[objectReference] <= 1)
					{
						_referenceParents.Remove(objectReference);
					}
					else
					{
						_referenceParents[objectReference]--;
					}
				}
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
		}

		/// <summary>
		/// Returns true if we have encountered this parent before
		/// </summary>
		/// <param name="objectReference"></param>
		/// <returns></returns>
		protected internal bool IsParent(object objectReference)
		{
			if (objectReference == null)
			{
				return false;
			}
			if (Config.UseHashCodeIdentifier && _hashParents == null)
			{
				return false;
			}
			if (!Config.UseHashCodeIdentifier && _referenceParents == null)
			{
				return false;
			}
			if (Config.UseHashCodeIdentifier)
			{
				int hashCode = objectReference.GetHashCode();
				if (hashCode == 0)
				{
					return false;
				}
				return _hashParents.ContainsKey(hashCode);
			}
			return _referenceParents.ContainsKey(objectReference);
		}
	}
	/// <summary>
	/// Get custom validator based on property
	/// </summary>
	public static class CustomValidationLogic
	{
		/// <summary>
		/// Get validator for a member of an expando object
		/// </summary>
		/// <param name="config"></param>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static BaseTypeComparer CustomValidatorForDynamicMember(ComparisonConfig config, string name, Type type)
		{
			BaseTypeComparer result = null;
			if (config.CustomPropertyComparers.Count == 0)
			{
				return result;
			}
			result = GetValidatorByName(config, name);
			if (result != null)
			{
				return result;
			}
			return GetValidatorByName(config, type.Name + "." + name);
		}

		/// <summary>
		/// Get validator for a member
		/// </summary>
		/// <param name="config"></param>
		/// <param name="info"></param>
		/// <param name="objectType"></param>
		/// <returns></returns>
		public static BaseTypeComparer CustomValidatorForMember(ComparisonConfig config, MemberInfo info, Type objectType)
		{
			BaseTypeComparer result = null;
			if (config.CustomPropertyComparers.Count == 0)
			{
				return result;
			}
			result = GetValidatorByName(config, objectType.Name + "." + info.Name);
			if (result != null)
			{
				return result;
			}
			if (info.DeclaringType != null)
			{
				result = GetValidatorByName(config, info.DeclaringType.Name + "." + info.Name);
				if (result != null)
				{
					return result;
				}
			}
			return GetValidatorByName(config, info.Name);
		}

		private static BaseTypeComparer GetValidatorByName(ComparisonConfig config, string name)
		{
			config.CustomPropertyComparers.TryGetValue(name, out BaseTypeComparer value);
			return value;
		}
	}
	/// <summary>
	/// Detailed information about the difference
	/// </summary>
	public class Difference
	{
		/// <summary>
		/// Name of Expected Object
		/// </summary>
		public string ExpectedName { get; set; }

		/// <summary>
		/// Name of Actual Object
		/// </summary>
		public string ActualName { get; set; }

		/// <summary>
		/// Returns the parent property name
		/// </summary>
		public string ParentPropertyName
		{
			get
			{
				if (PropertyName.EndsWith("]") && PropertyName.Contains("["))
				{
					int length = PropertyName.LastIndexOf('[');
					return PropertyName.Substring(0, length);
				}
				if (PropertyName.Contains("."))
				{
					int num = PropertyName.LastIndexOf('.');
					if (num > 0)
					{
						return PropertyName.Substring(0, num);
					}
				}
				return string.Empty;
			}
		}

		/// <summary>
		/// The breadcrumb of the property leading up to the value
		/// </summary>
		public string PropertyName { get; set; }

		/// <summary>
		/// The child property name
		/// </summary>
		public string ChildPropertyName { get; set; }

		/// <summary>
		/// Object1 Value as a string
		/// </summary>
		public string Object1Value { get; set; }

		/// <summary>
		/// Object2 Value as a string
		/// </summary>
		public string Object2Value { get; set; }

		/// <summary>
		/// The type of the first object
		/// </summary>
		public string Object1TypeName { get; set; }

		/// <summary>
		/// The type of the second object
		/// </summary>
		public string Object2TypeName { get; set; }

		/// <summary>
		/// A reference to the parent of object1
		/// </summary>
		public object ParentObject1 { get; set; }

		/// <summary>
		/// A reference to the parent of object2
		/// </summary>
		public object ParentObject2 { get; set; }

		/// <summary>
		/// Object1 as a reference
		/// </summary>
		public object Object1 { get; set; }

		/// <summary>
		/// Object2 as a reference
		/// </summary>
		public object Object2 { get; set; }

		/// <summary>
		/// Prefix to put on the beginning of the message
		/// </summary>
		public string MessagePrefix { get; set; }

		/// <summary>
		/// Item and property name only
		/// </summary>
		/// <returns></returns>
		public string GetShortItem()
		{
			string text = (string.IsNullOrEmpty(PropertyName) ? $"{ExpectedName} != {ActualName}" : ((!string.IsNullOrEmpty(ChildPropertyName)) ? $"{PropertyName}.{ChildPropertyName}" : $"{PropertyName}"));
			if (!string.IsNullOrEmpty(MessagePrefix))
			{
				text = $"{MessagePrefix}: {text}";
			}
			text = text.Replace("..", ".");
			return text.Replace(".[", "[");
		}

		/// <summary>
		/// The type and index of what is compared
		/// </summary>
		/// <returns></returns>
		public string GetWhatIsCompared()
		{
			string text = ((!string.IsNullOrEmpty(PropertyName)) ? ((!string.IsNullOrEmpty(ChildPropertyName)) ? string.Format("Types [{4},{5}], Item {0}.{2}.{3} != {1}.{2}.{3}", ExpectedName, ActualName, PropertyName, ChildPropertyName, Object1TypeName, Object2TypeName) : string.Format("Types [{3},{4}], Item {0}.{2} != {1}.{2}", ExpectedName, ActualName, PropertyName, Object1TypeName, Object2TypeName)) : (string.IsNullOrEmpty(ChildPropertyName) ? string.Format("Types [{2},{3}], Item {0} != {1}", ExpectedName, ActualName, Object1TypeName, Object2TypeName) : string.Format("Types [{2}.{4},{3}.{4}], Item {0} != {1}", ExpectedName, ActualName, Object1TypeName, Object2TypeName, ChildPropertyName)));
			if (!string.IsNullOrEmpty(MessagePrefix))
			{
				text = $"{MessagePrefix}: {text}";
			}
			text = text.Replace("..", ".");
			return text.Replace(".[", "[");
		}

		/// <summary>
		/// Nicely formatted string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{GetWhatIsCompared()}, Values ({Object1Value},{Object2Value})";
		}
	}
	/// <summary>
	/// Environment helper class
	/// </summary>
	public static class EnvironmentHelper
	{
		/// <summary>
		/// Returns true if we are running in Windows
		/// </summary>
		/// <returns></returns>
		public static bool IsWindows()
		{
			string environmentVariable = Environment.GetEnvironmentVariable("windir");
			if (!string.IsNullOrEmpty(environmentVariable) && environmentVariable.Contains("\\"))
			{
				return Directory.Exists(environmentVariable);
			}
			return false;
		}
	}
	/// <summary>
	/// Exclude types depending upon the configuration
	/// </summary>
	public static class ExcludeLogic
	{
		/// <summary>
		/// Exclude a member of an expando object
		/// </summary>
		/// <param name="config"></param>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool ShouldExcludeDynamicMember(ComparisonConfig config, string name, Type type)
		{
			if (config.MembersToIncludeSet.Count > 0 && !config.MembersToIncludeSet.Contains(name))
			{
				return true;
			}
			if (config.MembersToIgnoreSet.Count > 0)
			{
				if (type != null && config.MembersToIgnoreSet.Contains(type.Name + "." + name))
				{
					return true;
				}
				if (config.MembersToIgnoreSet.Count > 0 && config.MembersToIgnoreSet.Contains(name))
				{
					return true;
				}
				if (config.HasWildcardMembersToExclude() && ExcludedByWildcard(config, name))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns true if the property or field should be excluded
		/// </summary>
		/// <param name="config"></param>
		/// <param name="info"></param>
		/// <param name="objectType"></param>
		/// <returns></returns>
		public static bool ShouldExcludeMember(ComparisonConfig config, MemberInfo info, Type objectType)
		{
			if (config.MembersToIncludeSet.Count > 0 && !config.MembersToIncludeSet.Contains(info.Name))
			{
				return true;
			}
			if (config.MembersToIgnoreSet.Count > 0)
			{
				if (config.MembersToIgnoreSet.Contains(objectType.Name + "." + info.Name))
				{
					return true;
				}
				if (info.DeclaringType != null && config.MembersToIgnoreSet.Contains(info.DeclaringType.Name + "." + info.Name))
				{
					return true;
				}
				if (config.MembersToIgnoreSet.Count > 0 && config.MembersToIgnoreSet.Contains(info.Name))
				{
					return true;
				}
				if (config.HasWildcardMembersToExclude() && ExcludedByWildcard(config, info.Name))
				{
					return true;
				}
			}
			if (IgnoredByAttribute(config, info))
			{
				return true;
			}
			if (IgnoredByLackOfAttribute(config, info))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Returns true if the property or field should be exluded by wilcard
		/// </summary>
		/// <param name="config"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool ExcludedByWildcard(ComparisonConfig config, string name)
		{
			foreach (string item in config.MembersToIgnore)
			{
				if (item.StartsWith("*") && item.EndsWith("*") && item.Length > 2)
				{
					string value = item.Substring(1, item.Length - 2);
					if (name.Contains(value))
					{
						return true;
					}
					continue;
				}
				if (item.StartsWith("*") && item.Length >= 2 && name.EndsWith(item.Substring(1)))
				{
					return true;
				}
				if (item.EndsWith("*") && item.Length >= 2 && name.StartsWith(item.Substring(0, item.Length - 2)))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Check if the class should be exluded by Attribute
		/// </summary>
		/// <param name="config"></param>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public static bool ShouldExcludeClass(ComparisonConfig config, Type t1, Type t2)
		{
			if (config.ClassTypesToIncludeSet.Count > 0 && (!config.ClassTypesToIncludeSet.Contains(t1) || !config.ClassTypesToIncludeSet.Contains(t2)))
			{
				return true;
			}
			if (config.ClassTypesToIgnoreSet.Count > 0 && (config.ClassTypesToIgnoreSet.Contains(t1) || config.ClassTypesToIgnoreSet.Contains(t2)))
			{
				return true;
			}
			if (IgnoredByAttribute(config, t1.GetTypeInfo()) || IgnoredByAttribute(config, t2.GetTypeInfo()))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Check if the class type should be excluded based on the configuration
		/// </summary>
		/// <param name="config"></param>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public static bool ShouldExcludeType(ComparisonConfig config, Type t1, Type t2)
		{
			if (config.TypesToIncludeSet.Count > 0 && (!config.TypesToIncludeSet.Contains(t1) || !config.TypesToIncludeSet.Contains(t2)))
			{
				return true;
			}
			if (config.TypesToIgnoreSet.Count > 0 && (config.TypesToIgnoreSet.Contains(t1) || config.TypesToIgnoreSet.Contains(t2)))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Check if any type has attributes that should be bypassed
		/// </summary>
		/// <returns></returns>	
		public static bool IgnoredByAttribute(ComparisonConfig config, MemberInfo info)
		{
			if (config.AttributesToIgnoreSet.Count == 0)
			{
				return false;
			}
			return info.GetCustomAttributes(inherit: true).Any((object a) => config.AttributesToIgnoreSet.Contains(a.GetType()));
		}

		/// <summary>
		/// Check if any type lacks attributes that should be required
		/// </summary>
		/// <returns></returns>	
		public static bool IgnoredByLackOfAttribute(ComparisonConfig config, MemberInfo info)
		{
			if (config.RequiredAttributesToCompareSet.Count == 0)
			{
				return false;
			}
			return !info.GetCustomAttributes(inherit: true).Any((object a) => config.RequiredAttributesToCompareSet.Contains(a.GetType()));
		}
	}
	/// <summary>
	/// Helper methods for files and directories
	/// </summary>
	public static class FileHelper
	{
		/// <summary>
		/// Get the current directory of the executing assembly
		/// </summary>
		/// <returns></returns>
		public static string GetCurrentDirectory()
		{
			return PathSlash(AppDomain.CurrentDomain.BaseDirectory);
		}

		/// <summary>
		/// Ensure the passed string ends with a directory separator character unless the string is blank.
		/// </summary>
		/// <param name="path">The string to append the backslash to.</param>
		/// <returns>String with a "/" on the end</returns>
		public static string PathSlash(string path)
		{
			string text = Convert.ToString(Path.DirectorySeparatorChar);
			if (path.Length == 0)
			{
				return path;
			}
			if (path.EndsWith(text))
			{
				return path;
			}
			return path + text;
		}
	}
	/// <summary>
	/// Config object for HtmlReport
	/// </summary>
	public class HtmlConfig
	{
		private string _htmlHeader;

		private const string _header = "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"ie=edge\">\r\n    <title>%TITLE%</title>\r\n    <style>\r\n    %CSS%\r\n    </style>\r\n</head>\r\n<body>\r\n  ";

		private const string _footer = "</body>\r\n</html>";

		private const string _css = "\r\n    //.diff-crumb {background: gray;}\r\n    .diff-table tr:nth-child(odd) { background-color:#eee; }\r\n    .diff-table tr:nth-child(even) { background-color:#fff; }\r\n    //.diff-expected {color: red;}\r\n  .diff-actual {color:red;}\r\n  ";

		/// <summary>
		/// The header value of the Bread Crumb column
		/// </summary>
		public string BreadCrumbColumName { get; set; }

		/// <summary>
		/// The header value of the Expected column
		/// </summary>
		public string ExpectedColumnName { get; set; }

		/// <summary>
		/// The header value of the Actual column
		/// </summary>
		public string ActualColumnName { get; set; }

		/// <summary>
		/// If true, the output will be complete html, if false, it will just be the table
		/// </summary>
		public bool GenerateFullHtml { get; set; }

		/// <summary>
		/// Setting this will overwrite the default html header (html, head, body tags)
		/// </summary>
		public string HtmlHeader
		{
			get
			{
				return _htmlHeader.Replace("%TITLE%", HtmlTitle).Replace("%CSS%", Style);
			}
			set
			{
				_htmlHeader = value;
			}
		}

		/// <summary>
		/// Setting this will overwrite the default html footer (closing body, html tags)
		/// </summary>
		public string HtmlFooter { get; set; }

		/// <summary>
		/// The title of the page - only visible if GenerateFullHtml == true
		/// </summary>
		public string HtmlTitle { get; set; }

		/// <summary>
		/// The CSS Style of the page - only used if the GenerateFullHtml == true
		/// </summary>
		public string Style { get; set; }

		/// <summary>
		/// Default constructor, sets default values
		/// </summary>
		public HtmlConfig()
		{
			BreadCrumbColumName = "Bread Crumb";
			ExpectedColumnName = "Expected";
			ActualColumnName = "Actual";
			GenerateFullHtml = false;
			HtmlTitle = "Document";
			_htmlHeader = "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"ie=edge\">\r\n    <title>%TITLE%</title>\r\n    <style>\r\n    %CSS%\r\n    </style>\r\n</head>\r\n<body>\r\n  ";
			HtmlFooter = "</body>\r\n</html>";
			Style = "\r\n    //.diff-crumb {background: gray;}\r\n    .diff-table tr:nth-child(odd) { background-color:#eee; }\r\n    .diff-table tr:nth-child(even) { background-color:#fff; }\r\n    //.diff-expected {color: red;}\r\n  .diff-actual {color:red;}\r\n  ";
		}

		/// <summary>
		/// Appends to the existing Style value
		/// </summary>
		/// <param name="css">Any css to append</param>
		public void IncludeCustomCSS(string css)
		{
			Style += css;
		}

		/// <summary>
		/// Replaces the existing Style value
		/// </summary>
		/// <param name="css">Any css to use</param>
		public void ReplaceCSS(string css)
		{
			Style = css;
		}
	}
	/// <summary>
	/// Interface for mocking
	/// </summary>
	public interface ICompareLogic
	{
		/// <summary>
		/// The default configuration
		/// </summary>
		ComparisonConfig Config { get; set; }

		/// <summary>
		/// Compare two objects of the same type to each other.
		/// </summary>
		/// <remarks>
		/// Check the Differences or DifferencesString Properties for the differences.
		/// Default MaxDifferences is 1 for performance
		/// </remarks>
		/// <param name="expectedObject">The expected object value to compare</param>
		/// <param name="actualObject">The actual object value to compare</param>
		/// <returns>True if they are equal</returns>
		ComparisonResult Compare(object expectedObject, object actualObject);

		/// <summary>
		/// Reflection properties and fields are cached. By default this cache is cleared automatically after each compare.
		/// </summary>
		void ClearCache();
	}
	/// <summary>Implements methods to support the comparison of objects for equality, in a customizable fashion.</summary>
	public class LogicEqualityComparer : LogicEqualityComparer<object>
	{
	}
	/// <summary>Implements methods to support the comparison of objects for equality, in a customizable fashion.</summary>
	/// <typeparam name="T">The comparison object type.</typeparam>
	public class LogicEqualityComparer<T> : IEqualityComparer<T>
	{
		private readonly CompareLogic _comparer = new CompareLogic();

		/// <summary>Defines the configuration and logic by which Equals comparisons will be performed.</summary>
		public CompareLogic CompareLogic => _comparer;

		/// <summary>Gets or sets a value indicating whether the base object hashes should be used.</summary>
		/// <remarks>
		/// False by default to allow CompareLogic to evaluate equivalence of otherwise instance-sensitive hashing objects.
		/// NOTE: Any object which doesn't override GetHashCode will behave this way, so this property should generally be left false.
		/// </remarks>
		public bool UseObjectHashes { get; set; }

		/// <summary>Compare two objects of the same type to each other.</summary>
		/// <returns>True if the objects are considered equivalent, according to the current CompareLogic.</returns>
		public bool Equals(T x, T y)
		{
			return CompareLogic.Compare(x, y).AreEqual;
		}

		/// <summary>Retrieves the hash of the specified object.</summary>
		/// <param name="obj">The object to retrieve a hash for.</param>
		public int GetHashCode(T obj)
		{
			if (!UseObjectHashes)
			{
				return 0;
			}
			return obj.GetHashCode();
		}
	}
	/// <summary>
	/// Helper methods for processes
	/// </summary>
	public static class ProcessHelper
	{
		/// <summary>
		/// Execute an external program.
		/// </summary>
		/// <param name="executablePath">Path and filename of the executable.</param>
		/// <param name="arguments">Arguments to pass to the executable.</param>
		/// <param name="windowStyle">Window style for the process (hidden, minimized, maximized, etc).</param>
		/// <param name="waitUntilFinished">Wait for the process to finish.</param>
		/// <returns>Exit Code</returns>
		public static int Shell(string executablePath, string arguments, ProcessWindowStyle windowStyle, bool waitUntilFinished)
		{
			string arg = "";
			try
			{
				Process process = new Process();
				string text = Path.Combine(FileHelper.GetCurrentDirectory(), Path.GetFileName(executablePath) ?? string.Empty);
				if (File.Exists(text))
				{
					arg = text;
					process.StartInfo.FileName = text;
				}
				else
				{
					arg = executablePath;
					process.StartInfo.FileName = executablePath;
				}
				process.StartInfo.Arguments = arguments;
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.WindowStyle = windowStyle;
				process.Start();
				if (waitUntilFinished)
				{
					process.WaitForExit();
				}
				if (waitUntilFinished)
				{
					return process.ExitCode;
				}
				return 0;
			}
			catch
			{
				throw new ApplicationException($"Shell Fail: {arg} {arguments}");
			}
		}
	}
	/// <summary>
	/// Generic class for holding a Property Info, or Dynamic Info
	/// </summary>
	public class PropertyEntity
	{
		/// <summary>
		/// If true, this is a dynamic property
		/// </summary>
		public bool IsDynamic { get; set; }

		/// <summary>
		/// Name of the property
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Value of the property
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		/// Let me reflect on this day
		/// </summary>
		public Type ReflectedType { get; set; }

		/// <summary>
		/// The type of the parent
		/// </summary>
		public Type DeclaringType { get; set; }

		/// <summary>
		/// The type of the property
		/// </summary>
		public Type PropertyType { get; set; }

		/// <summary>
		/// If the property can be read from
		/// </summary>
		public bool CanRead { get; set; }

		/// <summary>
		/// If the property can be written to
		/// </summary>
		public bool CanWrite { get; set; }

		/// <summary>
		/// Indexers for the property
		/// </summary>
		public List<ParameterInfo> Indexers { get; set; }

		/// <summary>
		/// Reference to the property info
		/// </summary>
		public PropertyInfo PropertyInfo { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public PropertyEntity()
		{
			Indexers = new List<ParameterInfo>();
		}
	}
	/// <summary>
	/// The base comparer which contains all the type comparers
	/// </summary>
	public class RootComparer : BaseComparer
	{
		/// <summary>
		/// A list of the type comparers
		/// </summary>
		internal List<BaseTypeComparer> TypeComparers { get; set; }

		/// <summary>
		/// Compare two objects
		/// </summary>
		public bool Compare(CompareParms parms)
		{
			try
			{
				if (parms.Object1 == null && parms.Object2 == null)
				{
					return true;
				}
				Type t1 = ((parms.Object1 != null) ? parms.Object1.GetType() : null);
				Type t2 = ((parms.Object2 != null) ? parms.Object2.GetType() : null);
				if (ExcludeLogic.ShouldExcludeType(parms.Config, t1, t2))
				{
					return true;
				}
				BaseTypeComparer baseTypeComparer = parms.Config.CustomComparers.FirstOrDefault((BaseTypeComparer o) => o.IsTypeMatch(t1, t2));
				if (baseTypeComparer != null)
				{
					baseTypeComparer.CompareType(parms);
				}
				else if (parms.CustomPropertyComparer != null)
				{
					parms.CustomPropertyComparer.CompareType(parms);
				}
				else if (!PerformBaseTypeComparison(parms, t1, t2))
				{
					return false;
				}
			}
			catch (ObjectDisposedException)
			{
				if (!parms.Config.IgnoreObjectDisposedException)
				{
					throw;
				}
				return true;
			}
			return parms.Result.AreEqual;
		}

		private bool PerformBaseTypeComparison(CompareParms parms, Type t1, Type t2)
		{
			BaseTypeComparer baseTypeComparer = TypeComparers.FirstOrDefault((BaseTypeComparer o) => o.IsTypeMatch(t1, t2));
			if (baseTypeComparer != null)
			{
				if (parms.Config.IgnoreObjectTypes || !TypesDifferent(parms, t1, t2))
				{
					baseTypeComparer.CompareType(parms);
				}
			}
			else
			{
				if (EitherObjectIsNull(parms))
				{
					return false;
				}
				if (!parms.Config.IgnoreObjectTypes && t1 != null)
				{
					throw new NotSupportedException("Cannot compare object of type " + t1.Name);
				}
			}
			return true;
		}

		private bool TypesDifferent(CompareParms parms, Type t1, Type t2)
		{
			if (!parms.Config.IgnoreObjectTypes && parms.Object1 != null && parms.Object2 != null && t1 != t2)
			{
				if (parms.Config.IgnoreConcreteTypes && ((parms.Object1DeclaredType != null && parms.Object2DeclaredType != null && parms.Object1DeclaredType == parms.Object2DeclaredType) || t1.IsAssignableFrom(t2) || t2.IsAssignableFrom(t1)))
				{
					return false;
				}
				Difference difference = new Difference
				{
					ParentObject1 = parms.ParentObject1,
					ParentObject2 = parms.ParentObject2,
					PropertyName = parms.BreadCrumb,
					Object1Value = t1.FullName,
					Object2Value = t2.FullName,
					ChildPropertyName = "GetType()",
					MessagePrefix = "Different Types",
					Object1 = parms.Object1,
					Object2 = parms.Object2
				};
				AddDifference(parms.Result, difference);
				return true;
			}
			return false;
		}

		private bool EitherObjectIsNull(CompareParms parms)
		{
			if (parms.Object1 == null || parms.Object2 == null)
			{
				AddDifference(parms);
				return true;
			}
			return false;
		}
	}
	/// <summary>
	/// Factory to create a root comparer
	/// </summary>
	public static class RootComparerFactory
	{
		private static readonly object _locker = new object();

		private static RootComparer _rootComparer;

		/// <summary>
		/// Get the current root comparer
		/// </summary>
		/// <returns></returns>
		public static RootComparer GetRootComparer()
		{
			lock (_locker)
			{
				if (_rootComparer == null)
				{
					_rootComparer = BuildRootComparer();
				}
			}
			return _rootComparer;
		}

		private static RootComparer BuildRootComparer()
		{
			_rootComparer = new RootComparer();
			_rootComparer.TypeComparers = new List<BaseTypeComparer>();
			_rootComparer.TypeComparers.Add(new Snet.Utility.comparer.TypeComparers.StringComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new DateTimeComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new DecimalComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new DoubleComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new PointerComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new SimpleTypeComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new ReadOnlyCollectionComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new ListComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new RuntimeTypeComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new DatasetComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new DataTableComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new Snet.Utility.comparer.TypeComparers.DataRowComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new DataColumnComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new ByteArrayComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new DictionaryComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new HashSetComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new CollectionComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new ImmutableArrayComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new EnumerableComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new UriComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new StringBuilderComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new ClassComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new DateTimeOffSetComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new TimespanComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new EnumComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new DateOnlyComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new TimeOnlyComparer(_rootComparer));
			_rootComparer.TypeComparers.Add(new StructComparer(_rootComparer));
			return _rootComparer;
		}
	}
	/// <summary>
	/// Methods for manipulating strings
	/// </summary>
	public static class StringHelper
	{
		/// <summary>
		/// Insert spaces into a string 
		/// </summary>
		/// <example>
		/// OrderDetails = Order Details
		/// 10Net30 = 10 Net 30
		/// FTPHost = FTP Host
		/// </example> 
		/// <param name="input"></param>
		/// <returns></returns>
		public static string InsertSpaces(string input)
		{
			bool flag = true;
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(input.Length + input.Length / 2);
			input = input.Replace("_", " ");
			input = input.Replace("  ", " ");
			input = input.Trim();
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}
			char[] array = input.ToCharArray();
			stringBuilder.Append(array[0]);
			for (int i = 1; i < array.Length; i++)
			{
				bool flag2 = (array[i] >= 'A' && array[i] <= 'Z') || (array[i] >= '0' && array[i] <= '9') || array[i] == '-';
				bool flag3 = i < array.Length - 1 && array[i + 1] >= 'a' && array[i + 1] <= 'z';
				bool num = array[i] == ' ';
				bool flag4 = array[i] >= 'a' && array[i] <= 'z';
				if (!num)
				{
					if ((flag2 && !flag) || (flag2 & flag3))
					{
						stringBuilder.Append(' ');
						flag = true;
					}
					else if (flag4)
					{
						flag = false;
					}
				}
				stringBuilder.Append(array[i]);
			}
			stringBuilder.Replace("  ", " ");
			return stringBuilder.ToString();
		}
	}
	/// <summary>
	/// Extensions for Type to provide backward compatibility between latest and older .net Framework APIs.
	/// </summary>
	internal static class TypeBackwardsCompatibilityExtensions
	{
		/// <summary>
		/// Function to provide compilation compatibility between older code and newer style.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>The type.</returns>
		public static Type GetTypeInfo(this Type type)
		{
			return type;
		}
	}
	/// <summary>
	/// Methods for detecting types and converting types
	/// </summary>
	public static class TypeHelper
	{
		/// <summary>
		/// Returns true if it is a dynamic object
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsDynamicObject(Type type)
		{
			return typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type);
		}

		/// <summary>
		/// Determines whether the specified object is an expando object
		/// </summary>
		/// <param name="objectValue">The object value.</param>
		public static bool IsExpandoObject(object objectValue)
		{
			if (objectValue == null)
			{
				return false;
			}
			if (IsDynamicObject(objectValue.GetType()))
			{
				return objectValue is IDictionary<string, object>;
			}
			return false;
		}

		/// <summary>
		/// Returns true if it is a byte array
		/// </summary>
		public static bool IsByteArray(Type type)
		{
			if (IsIList(type) && (typeof(IEnumerable<byte>).IsAssignableFrom(type) || typeof(IEnumerable<byte?>).IsAssignableFrom(type)))
			{
				return type != typeof(sbyte[]);
			}
			return false;
		}

		/// <summary>
		/// Returns true if the type can have children
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool CanHaveChildren(Type type)
		{
			if (type == null)
			{
				return false;
			}
			if (!IsSimpleType(type) && !IsTimespan(type) && !IsDateTimeOffset(type) && !IsEnum(type) && !IsPointer(type) && !IsStringBuilder(type))
			{
				if (!IsClass(type) && !IsInterface(type) && !IsArray(type) && !IsIDictionary(type) && !IsIList(type) && !IsStruct(type))
				{
					return IsHashSet(type);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// True if the type is an array
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsArray(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return type.IsArray;
		}

		/// <summary>
		/// True if the type is an System.Collections.Immutable.ImmutableArray
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsImmutableArray(Type type)
		{
			if (type == null)
			{
				return false;
			}
			if (type.Namespace == "System.Collections.Immutable")
			{
				return type.Name == "ImmutableArray`1";
			}
			return false;
		}

		/// <summary>
		/// True if the type is a System.Collections.ObjectModel.ReadOnlyCollection
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsReadOnlyCollection(Type type)
		{
			if (type == null)
			{
				return false;
			}
			if (type.Namespace == "System.Collections.ObjectModel")
			{
				return type.Name == "ReadOnlyCollection`1";
			}
			return false;
		}

		/// <summary>
		/// Returns true if it is a struct
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsStruct(Type type)
		{
			if (type == null)
			{
				return false;
			}
			if (!type.GetTypeInfo().IsValueType || IsSimpleType(type) || IsImmutableArray(type))
			{
				return !IsEnum(type);
			}
			return true;
		}

		/// <summary>
		/// Returns true if the type is a timespan
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsTimespan(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return type == typeof(TimeSpan);
		}

		/// <summary>
		/// Return true if the type is a class
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsClass(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return type.GetTypeInfo().IsClass;
		}

		/// <summary>
		/// Return true if the type is an interface
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsInterface(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return type.GetTypeInfo().IsInterface;
		}

		/// <summary>
		/// Return true if the type is a URI
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsUri(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return typeof(Uri).IsAssignableFrom(type);
		}

		/// <summary>
		/// Return true if the type is a pointer
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsPointer(Type type)
		{
			if (type == null)
			{
				return false;
			}
			if (!(type == typeof(nint)))
			{
				return type == typeof(nuint);
			}
			return true;
		}

		/// <summary>
		/// Return true if the type is an enum
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsEnum(Type type)
		{
			if (type == null)
			{
				return false;
			}
			if (type.GetTypeInfo().IsGenericType && type.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				type = Nullable.GetUnderlyingType(type);
			}
			return type.GetTypeInfo().IsEnum;
		}

		/// <summary>
		/// Return true if the type is a dictionary
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsIDictionary(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return typeof(IDictionary).IsAssignableFrom(type);
		}

		/// <summary>
		/// Return true if the type is a hashset
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsHashSet(Type type)
		{
			if (type == null)
			{
				return false;
			}
			if (type.GetTypeInfo().IsGenericType)
			{
				return type.GetTypeInfo().GetGenericTypeDefinition() == typeof(HashSet<>);
			}
			return false;
		}

		/// <summary>
		/// Return true if the type is a List
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsIList(Type type)
		{
			if (type == null)
			{
				return false;
			}
			if (typeof(IList).IsAssignableFrom(type))
			{
				return !IsImmutableArray(type);
			}
			return false;
		}

		/// <summary>
		/// Return true if the type is an Enumerable
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsEnumerable(Type type)
		{
			if (type == null)
			{
				return false;
			}
			_ = type.ReflectedType;
			if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
			{
				return type.Name != "ExpandoObject";
			}
			return false;
		}

		/// <summary>
		/// Return true if the type is a Double
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsDouble(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return type == typeof(double);
		}

		/// <summary>
		/// Return true if the type is a Decimal
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsDecimal(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return type == typeof(decimal);
		}

		/// <summary>
		/// Return true if the type is a Decimal or Nullable Decimal
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsDecimal(object value)
		{
			if (value == null)
			{
				return false;
			}
			return value is decimal;
		}

		/// <summary>
		/// Return true if the type is a DateTime
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsDateTime(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return type == typeof(DateTime);
		}

		/// <summary>
		/// Return true if the type is a DateTime
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsDateOnly(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return type == typeof(DateOnly);
		}

		/// <summary>
		/// Return true if the type is a TimeOnly
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsTimeOnly(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return type == typeof(TimeOnly);
		}

		/// <summary>
		/// Return true if the type is a DateTimeOffset
		/// </summary>
		/// <param name="type"></param>
		public static bool IsDateTimeOffset(Type type)
		{
			if (type == null)
			{
				return false;
			}
			if (!(type == typeof(DateTimeOffset)))
			{
				return type == typeof(DateTimeOffset?);
			}
			return true;
		}

		/// <summary>
		/// Return true if the type is a StringBuilder
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsStringBuilder(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return type == typeof(StringBuilder);
		}

		/// <summary>
		/// Return true if the type is a string
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsString(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return type == typeof(string);
		}

		/// <summary>
		/// Return true if the type is a primitive type, date, decimal, string, or GUID
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsSimpleType(Type type)
		{
			if (type == null)
			{
				return false;
			}
			if (type.GetTypeInfo().IsGenericType && type.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				type = Nullable.GetUnderlyingType(type);
			}
			if (!type.GetTypeInfo().IsPrimitive && !(type == typeof(DateTime)) && !(type == typeof(string)) && !(type == typeof(Guid)))
			{
				return type == typeof(decimal);
			}
			return true;
		}

		/// <summary>
		/// Returns true if the Type is a Runtime type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsRuntimeType(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return typeof(Type).IsAssignableFrom(type);
		}

		/// <summary>
		/// Returns true if the type is a generic type.
		/// </summary>
		/// <param name="type">The type.</param>
		public static bool IsGenericType(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return type.IsGenericType;
		}

		/// <summary>
		/// Returns true if the type is a dataset
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsDataset(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return type == typeof(DataSet);
		}

		/// <summary>
		/// Returns true if the type is a data table
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsDataTable(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return type == typeof(DataTable);
		}

		/// <summary>
		/// Returns true if the type is a data row
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsDataRow(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return type == typeof(DataRow);
		}

		/// <summary>
		/// Returns true if the Type is Data Column
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsDataColumn(Type type)
		{
			if (type == null)
			{
				return false;
			}
			return type == typeof(DataColumn);
		}

		/// <summary>
		/// Turn a list of types into a list of string types
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		public static List<string> ListOfTypesSerializer(List<Type> types)
		{
			if (types == null || !types.Any())
			{
				return new List<string>();
			}
			List<string> list = new List<string>();
			foreach (Type type in types)
			{
				if (type != null)
				{
					list.Add(type.FullName);
				}
			}
			return list;
		}

		/// <summary>
		/// Turn a list of string into a list of types
		/// </summary>
		/// <param name="stringList"></param>
		/// <returns></returns>
		public static List<Type> ListOfTypesDeserializer(List<string> stringList)
		{
			if (stringList == null || !stringList.Any())
			{
				return new List<Type>();
			}
			List<Type> list = new List<Type>();
			foreach (string @string in stringList)
			{
				if (!string.IsNullOrEmpty(@string))
				{
					list.Add(Type.GetType(@string));
				}
			}
			return list;
		}
	}
	/// <summary>
	/// Used internally to verify the config settings before comparing
	/// </summary>
	public class VerifyConfig
	{
		/// <summary>
		/// Verifies the specified configuration.
		/// </summary>
		/// <param name="config">The configuration.</param>
		public void Verify(ComparisonConfig config)
		{
			VerifySpec(config);
		}

		/// <summary>
		/// Verifies the collection matching spec.
		/// </summary>
		/// <param name="config">The configuration.</param>
		/// <exception cref="T:System.Exception">
		/// </exception>
		public void VerifySpec(ComparisonConfig config)
		{
			if (config.CollectionMatchingSpec == null)
			{
				return;
			}
			foreach (KeyValuePair<Type, IEnumerable<string>> item in config.CollectionMatchingSpec)
			{
				if (TypeHelper.IsIList(item.Key))
				{
					throw new ArgumentException($"Collection Matching Spec Type {item.Key.Name} should be a class, not a List.  Expected something like Customer, not List<Customer>.  See https://github.com/GregFinzer/Compare-Net-Objects/wiki/Comparing-Lists-of-Different-Lengths", "config");
				}
				if (!TypeHelper.IsClass(item.Key) && !TypeHelper.IsInterface(item.Key))
				{
					throw new ArgumentException($"Collection matching spec Type {item.Key.Name} should be a class or an interface.  See https://github.com/GregFinzer/Compare-Net-Objects/wiki/Comparing-Lists-of-Different-Lengths", "config");
				}
				List<PropertyInfo> source = Cache.GetPropertyInfo(config, item.Key).ToList();
				foreach (string index in item.Value)
				{
					if (source.All((PropertyInfo o) => o.Name != index))
					{
						throw new ArgumentException($"Collection Matching Spec cannot find property {index} of type {item.Key.Name}", "config");
					}
				}
			}
		}
	}
	/// <summary>
	/// Helper class for web related methods
	/// </summary>
	public static class WebHelper
	{
		/// <summary>
		/// HTML-encodes a string and returns the encoded string.
		/// </summary>
		/// <param name="text">The text string to encode. </param>
		/// <returns>The HTML-encoded text.</returns>
		public static string HtmlEncode(string text)
		{
			if (text == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			int length = text.Length;
			for (int i = 0; i < length; i++)
			{
				switch (text[i])
				{
				case '<':
					stringBuilder.Append("&lt;");
					continue;
				case '>':
					stringBuilder.Append("&gt;");
					continue;
				case '"':
					stringBuilder.Append("&quot;");
					continue;
				case '&':
					stringBuilder.Append("&amp;");
					continue;
				}
				if (text[i] > '\u009f')
				{
					stringBuilder.Append("&#");
					stringBuilder.Append(((int)text[i]).ToString(CultureInfo.InvariantCulture));
					stringBuilder.Append(";");
				}
				else
				{
					stringBuilder.Append(text[i]);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
namespace Snet.Utility.comparer.TypeComparers
{
	/// <summary>
	/// Common functionality for all Comparers
	/// </summary>
	public class BaseComparer
	{
		/// <summary>
		/// Add a breadcrumb to an existing breadcrumb
		/// </summary>
		/// <param name="config">Comparison configuration</param>
		/// <param name="existing">The existing breadcrumb</param>
		/// <param name="name">The field or property name</param>
		/// <returns>The new breadcrumb</returns>
		protected string AddBreadCrumb(ComparisonConfig config, string existing, string name)
		{
			return AddBreadCrumb(config, existing, name, string.Empty, null);
		}

		/// <summary>
		/// Add a breadcrumb to an existing breadcrumb
		/// </summary>
		/// <param name="config">The comparison configuration</param>
		/// <param name="existing">The existing breadcrumb</param>
		/// <param name="name">The property or field name</param>
		/// <param name="extra">Extra information to output after the name</param>
		/// <param name="index">The index for an array, list, or row</param>
		/// <returns>The new breadcrumb</returns>
		protected string AddBreadCrumb(ComparisonConfig config, string existing, string name, string extra, int index)
		{
			return AddBreadCrumb(config, existing, name, extra, (index >= 0) ? index.ToString(CultureInfo.InvariantCulture) : null);
		}

		/// <summary>
		/// Add a breadcrumb to an existing breadcrumb
		/// </summary>
		/// <remarks>This originally used a string builder which had lower performance</remarks>
		/// <param name="config">Comparison configuration</param>
		/// <param name="existing">The existing breadcrumb</param>
		/// <param name="name">The field or property name</param>
		/// <param name="extra">Extra information to append after the name</param>
		/// <param name="index">The index if it is an array, list, row etc.</param>
		/// <returns>The new breadcrumb</returns>
		protected string AddBreadCrumb(ComparisonConfig config, string existing, string name, string extra, string index)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			bool num = !string.IsNullOrEmpty(index);
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			bool num2 = name.Length > 0;
			string text = existing;
			if (num2)
			{
				if (text.Length > 0)
				{
					text += ".";
				}
				text += name;
			}
			text += extra;
			if (num)
			{
				int result = -1;
				text += string.Format(int.TryParse(index, out result) ? "[{0}]" : "[\"{0}\"]", index);
			}
			if (config.ShowBreadcrumb)
			{
				Trace.WriteLine(text);
			}
			return text;
		}

		/// <summary>
		/// Add a difference for the current parameters
		/// </summary>
		/// <param name="parameters"></param>
		protected void AddDifference(CompareParms parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			Difference difference = new Difference
			{
				ParentObject1 = parameters.ParentObject1,
				ParentObject2 = parameters.ParentObject2,
				PropertyName = parameters.BreadCrumb,
				Object1Value = NiceString(parameters.Object1),
				Object2Value = NiceString(parameters.Object2),
				Object1 = parameters.Object1,
				Object2 = parameters.Object2
			};
			AddDifference(parameters.Result, difference);
		}

		/// <summary>
		/// Add a difference to the result
		/// </summary>
		/// <param name="difference">The difference to add to the result</param>
		/// <param name="result">The comparison result</param>
		protected void AddDifference(ComparisonResult result, Difference difference)
		{
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			if (difference == null)
			{
				throw new ArgumentNullException("difference");
			}
			difference.ActualName = result.Config.ActualName;
			difference.ExpectedName = result.Config.ExpectedName;
			difference.Object1TypeName = ((difference.Object1 != null && difference.Object1 != null) ? difference.Object1.GetType().Name : "null");
			difference.Object2TypeName = ((difference.Object2 != null && difference.Object2 != null) ? difference.Object2.GetType().Name : "null");
			result.Differences.Add(difference);
			result.Config.DifferenceCallback(difference);
		}

		/// <summary>
		/// Convert an object to a nicely formatted string
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected string NiceString(object value)
		{
			try
			{
				if (value == null)
				{
					return "(null)";
				}
				if (value == DBNull.Value)
				{
					return "System.DBNull.Value";
				}
				return value.ToString();
			}
			catch
			{
				return string.Empty;
			}
		}
	}
	/// <summary>
	/// Common functionality for all Type Comparers
	/// </summary>
	public abstract class BaseTypeComparer : BaseComparer
	{
		/// <summary>
		/// A reference to the root comparer as newed up by the RootComparerFactory
		/// </summary>
		public RootComparer RootComparer { get; set; }

		/// <summary>
		/// Protected constructor that references the root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		protected BaseTypeComparer(RootComparer rootComparer)
		{
			RootComparer = rootComparer;
		}

		/// <summary>
		/// If true the type comparer will handle the comparison for the type
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public abstract bool IsTypeMatch(Type type1, Type type2);

		/// <summary>
		/// Compare the two objects
		/// </summary>
		public abstract void CompareType(CompareParms parms);
	}
	/// <summary>
	/// Compare two byte arrays
	/// </summary>
	public class ByteArrayComparer : BaseTypeComparer
	{
		/// <summary>
		/// Protected constructor that references the root comparer
		/// </summary>
		/// <param name="rootComparer">The root comparer.</param>
		public ByteArrayComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// If true the type comparer will handle the comparison for the type
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns><c>true</c> if it is a byte array; otherwise, <c>false</c>.</returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsByteArray(type1))
			{
				return TypeHelper.IsByteArray(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two byte array objects
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			if (parms != null && parms.Object1 != null && parms.Object2 != null && !ListsHaveDifferentCounts(parms))
			{
				CompareItems(parms);
			}
		}

		private bool ListsHaveDifferentCounts(CompareParms parms)
		{
			IList list = parms.Object1 as IList;
			IList list2 = parms.Object2 as IList;
			if (list == null)
			{
				throw new ArgumentException("parms.Object1");
			}
			if (list2 == null)
			{
				throw new ArgumentException("parms.Object2");
			}
			if (list.Count != list2.Count)
			{
				Difference difference = new Difference
				{
					ParentObject1 = parms.ParentObject1,
					ParentObject2 = parms.ParentObject2,
					PropertyName = parms.BreadCrumb,
					Object1Value = list.Count.ToString(CultureInfo.InvariantCulture),
					Object2Value = list2.Count.ToString(CultureInfo.InvariantCulture),
					ChildPropertyName = "Count",
					Object1 = list,
					Object2 = list2
				};
				AddDifference(parms.Result, difference);
				if (parms.Result.ExceededDifferences)
				{
					return true;
				}
			}
			return false;
		}

		private void CompareItems(CompareParms parms)
		{
			int num = 0;
			int num2 = 0;
			IEnumerator enumerator = ((IList)parms.Object1).GetEnumerator();
			IEnumerator enumerator2 = ((IList)parms.Object2).GetEnumerator();
			while (enumerator.MoveNext() && enumerator2.MoveNext())
			{
				byte? b = enumerator.Current as byte?;
				byte? b2 = enumerator2.Current as byte?;
				if (b != b2)
				{
					string propertyName = AddBreadCrumb(parms.Config, parms.BreadCrumb, string.Empty, string.Empty, num);
					Difference difference = new Difference
					{
						ParentObject1 = parms.Object1,
						ParentObject2 = parms.Object2,
						PropertyName = propertyName,
						Object1Value = NiceString(b),
						Object2Value = NiceString(b2),
						Object1 = b,
						Object2 = b2
					};
					AddDifference(parms.Result, difference);
					num2++;
					if (num2 >= parms.Result.Config.MaxByteArrayDifferences)
					{
						break;
					}
				}
				num++;
			}
		}
	}
	/// <summary>
	/// Compare two objects of type class
	/// </summary>
	public class ClassComparer : BaseTypeComparer
	{
		private readonly PropertyComparer _propertyComparer;

		private readonly FieldComparer _fieldComparer;

		/// <summary>
		/// Constructor for the class comparer
		/// </summary>
		/// <param name="rootComparer">The root comparer instantiated by the RootComparerFactory</param>
		public ClassComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
			_propertyComparer = new PropertyComparer(rootComparer);
			_fieldComparer = new FieldComparer(rootComparer);
		}

		/// <summary>
		/// Returns true if the both objects are a class
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (!TypeHelper.IsClass(type1) || !TypeHelper.IsClass(type2))
			{
				if (TypeHelper.IsInterface(type1))
				{
					return TypeHelper.IsInterface(type2);
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// Compare two classes
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			try
			{
				parms.Result.AddParent(parms.Object1);
				parms.Result.AddParent(parms.Object2);
				if (!(parms.Object1 is IEnumerable) && parms.Object1 == parms.Object2)
				{
					return;
				}
				Type type = parms.Object1.GetType();
				Type type2 = parms.Object2.GetType();
				if (!ExcludeLogic.ShouldExcludeClass(parms.Config, type, type2))
				{
					parms.Object1Type = type;
					parms.Object2Type = type2;
					if (parms.Config.CompareProperties)
					{
						_propertyComparer.PerformCompareProperties(parms);
					}
					if (parms.Config.CompareFields)
					{
						_fieldComparer.PerformCompareFields(parms);
					}
				}
			}
			finally
			{
				parms.Result.RemoveParent(parms.Object1);
				parms.Result.RemoveParent(parms.Object2);
			}
		}
	}
	/// <summary>
	/// Logic to compare two collections of different types.
	/// </summary>
	public class CollectionComparer : BaseTypeComparer
	{
		/// <summary>
		/// The main constructor.
		/// </summary>
		public CollectionComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both objects are collections.
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (typeof(ICollection).IsAssignableFrom(type1))
			{
				return typeof(ICollection).IsAssignableFrom(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two collections.
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			try
			{
				parms.Result.AddParent(parms.Object1);
				parms.Result.AddParent(parms.Object2);
				Type type = parms.Object1.GetType();
				Type type2 = parms.Object2.GetType();
				if (ExcludeLogic.ShouldExcludeClass(parms.Config, type, type2))
				{
					return;
				}
				parms.Object1Type = type;
				parms.Object2Type = type2;
				bool countsDifferent = CollectionsDifferentCount(parms);
				if (!parms.Result.ExceededDifferences)
				{
					if (parms.Config.IgnoreCollectionOrder)
					{
						new IgnoreOrderLogic(base.RootComparer).CompareEnumeratorIgnoreOrder(parms, countsDifferent);
					}
					else
					{
						CompareItems(parms);
					}
				}
			}
			finally
			{
				parms.Result.RemoveParent(parms.Object1);
				parms.Result.RemoveParent(parms.Object2);
			}
		}

		private void CompareItems(CompareParms parms)
		{
			int num = 0;
			IEnumerator enumerator = ((ICollection)parms.Object1).GetEnumerator();
			IEnumerator enumerator2 = ((ICollection)parms.Object2).GetEnumerator();
			while (enumerator.MoveNext() && enumerator2.MoveNext())
			{
				string breadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, string.Empty, string.Empty, num);
				CompareParms parms2 = new CompareParms
				{
					Result = parms.Result,
					Config = parms.Config,
					ParentObject1 = parms.Object1,
					ParentObject2 = parms.Object2,
					Object1 = enumerator.Current,
					Object2 = enumerator2.Current,
					BreadCrumb = breadCrumb
				};
				base.RootComparer.Compare(parms2);
				if (parms.Result.ExceededDifferences)
				{
					break;
				}
				num++;
			}
		}

		private bool CollectionsDifferentCount(CompareParms parms)
		{
			int count = ((ICollection)parms.Object1).Count;
			int count2 = ((ICollection)parms.Object2).Count;
			if (count != count2)
			{
				Difference difference = new Difference
				{
					ParentObject1 = parms.ParentObject1,
					ParentObject2 = parms.ParentObject2,
					PropertyName = parms.BreadCrumb,
					Object1Value = count.ToString(CultureInfo.InvariantCulture),
					Object2Value = count2.ToString(CultureInfo.InvariantCulture),
					ChildPropertyName = "Count",
					Object1 = parms.Object1,
					Object2 = parms.Object2
				};
				AddDifference(parms.Result, difference);
				return true;
			}
			return false;
		}
	}
	/// <summary>
	/// Compare two generic objects
	/// </summary>
	/// <typeparam name="T1">The type of the first object</typeparam>
	/// <typeparam name="T2">The type of the second object</typeparam>
	public class CustomComparer<T1, T2> : BaseTypeComparer
	{
		/// <summary>
		/// Method to evaluate the results, return true if two objects are equal
		/// </summary>
		public Func<T1, T2, bool> Compare = (T1 t1, T2 t2) => false;

		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public CustomComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Constructor that takes a default root comparer
		/// </summary>
		public CustomComparer()
			: this(RootComparerFactory.GetRootComparer())
		{
		}

		/// <summary>
		/// Constructor that takes a the predication with a default root comparer
		/// </summary>
		/// <param name="compare">A function to determine if two objects are equal</param>
		public CustomComparer(Func<T1, T2, bool> compare)
			: this(RootComparerFactory.GetRootComparer(), compare)
		{
		}

		/// <summary>
		/// Constructor that takes a the predication with a root comparer
		/// </summary>
		/// <param name="rootComparer">The root comparer</param>
		/// <param name="compare">Method to determine if two objects are equal</param>
		public CustomComparer(RootComparer rootComparer, Func<T1, T2, bool> compare)
			: this(rootComparer)
		{
			Compare = compare;
		}

		/// <summary>
		/// Compare two objects
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			if (!Compare((T1)parms.Object1, (T2)parms.Object2))
			{
				AddDifference(parms);
			}
		}

		/// <summary>
		/// Returns true if both objects match their types
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (type1 == typeof(T1))
			{
				return type2 == typeof(T2);
			}
			return false;
		}
	}
	/// <summary>
	/// Compare a data column
	/// </summary>
	public class DataColumnComparer : BaseTypeComparer
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="rootComparer"></param>
		public DataColumnComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both types compared are a DataColumn
		/// </summary>
		/// <param name="type1"></param>
		/// <param name="type2"></param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsDataColumn(type1))
			{
				return TypeHelper.IsDataColumn(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare a Data Column
		/// </summary>
		/// <param name="parms"></param>
		public override void CompareType(CompareParms parms)
		{
			if (parms.Object1 != null && parms.Object2 != null)
			{
				DataColumn dataColumn = (DataColumn)parms.Object1;
				DataColumn dataColumn2 = (DataColumn)parms.Object2;
				CompareProp(parms, dataColumn, dataColumn2, dataColumn.AllowDBNull, dataColumn2.AllowDBNull, "AllowDBNull");
				CompareProp(parms, dataColumn, dataColumn2, dataColumn.AutoIncrement, dataColumn2.AutoIncrement, "AutoIncrement");
				CompareProp(parms, dataColumn, dataColumn2, dataColumn.AutoIncrementSeed, dataColumn2.AutoIncrementSeed, "AutoIncrementSeed");
				CompareProp(parms, dataColumn, dataColumn2, dataColumn.AutoIncrementStep, dataColumn2.AutoIncrementStep, "AutoIncrementStep");
				CompareProp(parms, dataColumn, dataColumn2, dataColumn.Caption, dataColumn2.Caption, "Caption");
				CompareProp(parms, dataColumn, dataColumn2, dataColumn.ColumnName, dataColumn2.ColumnName, "ColumnName");
				CompareProp(parms, dataColumn, dataColumn2, dataColumn.DataType, dataColumn2.DataType, "DataType");
				CompareProp(parms, dataColumn, dataColumn2, dataColumn.DefaultValue, dataColumn2.DefaultValue, "DefaultValue");
				CompareProp(parms, dataColumn, dataColumn2, dataColumn.Expression, dataColumn2.Expression, "Expression");
				CompareProp(parms, dataColumn, dataColumn2, dataColumn.MaxLength, dataColumn2.MaxLength, "MaxLength");
				CompareProp(parms, dataColumn, dataColumn2, dataColumn.Namespace, dataColumn2.Namespace, "Namespace");
				CompareProp(parms, dataColumn, dataColumn2, dataColumn.Ordinal, dataColumn2.Ordinal, "Ordinal");
				CompareProp(parms, dataColumn, dataColumn2, dataColumn.Prefix, dataColumn2.Prefix, "Prefix");
				CompareProp(parms, dataColumn, dataColumn2, dataColumn.ReadOnly, dataColumn2.ReadOnly, "ReadOnly");
				CompareProp(parms, dataColumn, dataColumn2, dataColumn.Unique, dataColumn2.Unique, "Unique");
			}
		}

		private void CompareProp<T>(CompareParms parms, DataColumn col1, DataColumn col2, T prop1, T prop2, string propName)
		{
			if (!parms.Result.ExceededDifferences)
			{
				string breadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, propName);
				CompareParms compareParms = new CompareParms();
				compareParms.Result = parms.Result;
				compareParms.Config = parms.Config;
				compareParms.BreadCrumb = breadCrumb;
				compareParms.ParentObject1 = col1;
				compareParms.ParentObject2 = col2;
				compareParms.Object1 = prop1;
				compareParms.Object2 = prop2;
				base.RootComparer.Compare(compareParms);
			}
		}
	}
	/// <summary>
	/// Compare all columns in a data row
	/// </summary>
	public class DataRowComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public DataRowComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if this is a DataRow
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsDataRow(type1))
			{
				return TypeHelper.IsDataRow(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two data rows
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			DataRow dataRow = parms.Object1 as DataRow;
			DataRow dataRow2 = parms.Object2 as DataRow;
			if (dataRow == null || dataRow2 == null)
			{
				return;
			}
			for (int i = 0; i < Math.Min(dataRow2.Table.Columns.Count, dataRow.Table.Columns.Count); i++)
			{
				if ((parms.Config.MembersToIncludeSet.Count <= 0 || parms.Config.MembersToIncludeSet.Contains(dataRow.Table.Columns[i].ColumnName)) && (parms.Config.MembersToIncludeSet.Count != 0 || !parms.Config.MembersToIgnoreSet.Contains(dataRow.Table.Columns[i].ColumnName)) && (parms.Config.CompareReadOnly || !dataRow.Table.Columns[i].ReadOnly) && (!dataRow.IsNull(i) || !dataRow2.IsNull(i)))
				{
					string text = AddBreadCrumb(parms.Config, parms.BreadCrumb, string.Empty, string.Empty, dataRow.Table.Columns[i].ColumnName);
					if (dataRow.IsNull(i))
					{
						Difference difference = new Difference
						{
							ParentObject1 = parms.ParentObject1,
							ParentObject2 = parms.ParentObject2,
							PropertyName = text,
							Object1Value = "(null)",
							Object2Value = NiceString(dataRow2[i]),
							Object1 = parms.Object1,
							Object2 = parms.Object2
						};
						AddDifference(parms.Result, difference);
						break;
					}
					if (dataRow2.IsNull(i))
					{
						Difference difference2 = new Difference
						{
							ParentObject1 = parms.ParentObject1,
							ParentObject2 = parms.ParentObject2,
							PropertyName = text,
							Object1Value = NiceString(dataRow[i]),
							Object2Value = "(null)",
							Object1 = parms.Object1,
							Object2 = parms.Object2
						};
						AddDifference(parms.Result, difference2);
						break;
					}
					if ((dataRow.RowState == DataRowState.Deleted) ^ (dataRow2.RowState == DataRowState.Deleted))
					{
						Difference difference3 = new Difference
						{
							ParentObject1 = parms.ParentObject1,
							ParentObject2 = parms.ParentObject2,
							PropertyName = text,
							Object1Value = dataRow.RowState.ToString(),
							Object2Value = dataRow2.RowState.ToString(),
							Object1 = parms.Object1,
							Object2 = parms.Object2
						};
						AddDifference(parms.Result, difference3);
						break;
					}
					CompareParms compareParms = new CompareParms();
					compareParms.Result = parms.Result;
					compareParms.Config = parms.Config;
					compareParms.ParentObject1 = parms.Object1;
					compareParms.ParentObject2 = parms.Object2;
					compareParms.Object1 = dataRow[i];
					compareParms.Object2 = dataRow2[i];
					compareParms.BreadCrumb = text;
					base.RootComparer.Compare(compareParms);
					if (parms.Result.ExceededDifferences)
					{
						break;
					}
				}
			}
		}
	}
	/// <summary>
	/// Compare all tables and all rows in all tables
	/// </summary>
	public class DatasetComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public DatasetComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both objects are data sets
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsDataset(type1))
			{
				return TypeHelper.IsDataset(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two data sets
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			DataSet dataSet = parms.Object1 as DataSet;
			DataSet dataSet2 = parms.Object2 as DataSet;
			if (dataSet != null && dataSet2 != null && !TableCountsDifferent(parms, dataSet2, dataSet))
			{
				CompareEachTable(parms, dataSet, dataSet2);
			}
		}

		private bool TableCountsDifferent(CompareParms parms, DataSet dataSet2, DataSet dataSet1)
		{
			if (dataSet1.Tables.Count != dataSet2.Tables.Count)
			{
				Difference difference = new Difference
				{
					ParentObject1 = parms.ParentObject1,
					ParentObject2 = parms.ParentObject2,
					PropertyName = parms.BreadCrumb,
					Object1Value = dataSet1.Tables.Count.ToString(CultureInfo.InvariantCulture),
					Object2Value = dataSet2.Tables.Count.ToString(CultureInfo.InvariantCulture),
					ChildPropertyName = "Tables.Count",
					Object1 = parms.Object1,
					Object2 = parms.Object2
				};
				AddDifference(parms.Result, difference);
				if (parms.Result.ExceededDifferences)
				{
					return true;
				}
			}
			return false;
		}

		private void CompareEachTable(CompareParms parms, DataSet dataSet1, DataSet dataSet2)
		{
			for (int i = 0; i < Math.Min(dataSet1.Tables.Count, dataSet2.Tables.Count); i++)
			{
				string breadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, "Tables", string.Empty, dataSet1.Tables[i].TableName);
				CompareParms compareParms = new CompareParms();
				compareParms.Result = parms.Result;
				compareParms.Config = parms.Config;
				compareParms.BreadCrumb = breadCrumb;
				compareParms.ParentObject1 = dataSet1;
				compareParms.ParentObject2 = dataSet2;
				compareParms.Object1 = dataSet1.Tables[i];
				compareParms.Object2 = dataSet2.Tables[i];
				base.RootComparer.Compare(compareParms);
				if (parms.Result.ExceededDifferences)
				{
					break;
				}
			}
		}
	}
	/// <summary>
	/// Compare all rows in a data table
	/// </summary>
	public class DataTableComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public DataTableComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both objects are of type DataTable
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsDataTable(type1))
			{
				return TypeHelper.IsDataTable(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two datatables
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			DataTable dataTable = parms.Object1 as DataTable;
			DataTable dataTable2 = parms.Object2 as DataTable;
			if (dataTable == null || dataTable2 == null || (parms.Config.MembersToInclude.Count > 0 && !parms.Config.MembersToInclude.Contains(dataTable.TableName)) || (parms.Config.MembersToInclude.Count == 0 && parms.Config.MembersToIgnore.Contains(dataTable.TableName)))
			{
				return;
			}
			if (dataTable.Rows.Count != dataTable2.Rows.Count)
			{
				Difference difference = new Difference
				{
					ParentObject1 = parms.ParentObject1,
					ParentObject2 = parms.ParentObject2,
					PropertyName = parms.BreadCrumb,
					Object1Value = dataTable.Rows.Count.ToString(CultureInfo.InvariantCulture),
					Object2Value = dataTable2.Rows.Count.ToString(CultureInfo.InvariantCulture),
					ChildPropertyName = "Rows.Count",
					Object1 = parms.Object1,
					Object2 = parms.Object2
				};
				AddDifference(parms.Result, difference);
				if (parms.Result.ExceededDifferences)
				{
					return;
				}
			}
			if (!ColumnsDifferent(parms))
			{
				CompareEachRow(parms);
			}
		}

		private bool ColumnsDifferent(CompareParms parms)
		{
			DataTable dataTable = parms.Object1 as DataTable;
			DataTable dataTable2 = parms.Object2 as DataTable;
			if (dataTable == null)
			{
				throw new ArgumentException("parms.Object1");
			}
			if (dataTable2 == null)
			{
				throw new ArgumentException("parms.Object2");
			}
			if (dataTable.Columns.Count != dataTable2.Columns.Count)
			{
				Difference difference = new Difference
				{
					ParentObject1 = parms.ParentObject1,
					ParentObject2 = parms.ParentObject2,
					PropertyName = parms.BreadCrumb,
					Object1Value = dataTable.Columns.Count.ToString(CultureInfo.InvariantCulture),
					Object2Value = dataTable2.Columns.Count.ToString(CultureInfo.InvariantCulture),
					ChildPropertyName = "Columns.Count",
					Object1 = parms.Object1,
					Object2 = parms.Object2
				};
				AddDifference(parms.Result, difference);
				if (parms.Result.ExceededDifferences)
				{
					return true;
				}
			}
			foreach (int item in Enumerable.Range(0, dataTable.Columns.Count))
			{
				string breadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, "Columns", string.Empty, item);
				CompareParms parms2 = new CompareParms
				{
					Result = parms.Result,
					Config = parms.Config,
					ParentObject1 = parms.Object1,
					ParentObject2 = parms.Object2,
					Object1 = dataTable.Columns[item],
					Object2 = dataTable2.Columns[item],
					BreadCrumb = breadCrumb
				};
				base.RootComparer.Compare(parms2);
				if (parms.Result.ExceededDifferences)
				{
					return true;
				}
			}
			return false;
		}

		private void CompareEachRow(CompareParms parms)
		{
			DataTable dataTable = parms.Object1 as DataTable;
			DataTable dataTable2 = parms.Object2 as DataTable;
			if (dataTable == null)
			{
				throw new ArgumentException("parms.Object1");
			}
			if (dataTable2 == null)
			{
				throw new ArgumentException("parms.Object2");
			}
			for (int i = 0; i < Math.Min(dataTable.Rows.Count, dataTable2.Rows.Count); i++)
			{
				string breadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, "Rows", string.Empty, i);
				CompareParms parms2 = new CompareParms
				{
					Result = parms.Result,
					Config = parms.Config,
					ParentObject1 = parms.Object1,
					ParentObject2 = parms.Object2,
					Object1 = dataTable.Rows[i],
					Object2 = dataTable2.Rows[i],
					BreadCrumb = breadCrumb
				};
				base.RootComparer.Compare(parms2);
				if (parms.Result.ExceededDifferences)
				{
					break;
				}
			}
		}
	}
	/// <summary>
	/// Compare dates
	/// </summary>
	public sealed class DateOnlyComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public DateOnlyComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both types are DateOnly
		/// </summary>
		/// <param name="type1"></param>
		/// <param name="type2"></param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsDateOnly(type1))
			{
				return TypeHelper.IsDateOnly(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two DateOnly variables
		/// </summary>
		/// <param name="parms"></param>
		public override void CompareType(CompareParms parms)
		{
			if (parms.Object1 != null && parms.Object2 != null)
			{
				DateOnly dateOnly = (DateOnly)parms.Object1;
				DateOnly value = (DateOnly)parms.Object2;
				if (!dateOnly.Equals(value))
				{
					AddDifference(parms);
				}
			}
		}
	}
	/// <summary>
	/// Compare dates with the option to ignore based on milliseconds
	/// </summary>
	public class DateTimeComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public DateTimeComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both types are DateTime
		/// </summary>
		/// <param name="type1"></param>
		/// <param name="type2"></param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsDateTime(type1))
			{
				return TypeHelper.IsDateTime(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two DateTime variables
		/// </summary>
		/// <param name="parms"></param>
		public override void CompareType(CompareParms parms)
		{
			if (parms.Object1 == null || parms.Object2 == null)
			{
				return;
			}
			DateTime value = (DateTime)parms.Object1;
			DateTime value2 = (DateTime)parms.Object2;
			if (value.Kind != value2.Kind)
			{
				if (value.Kind == DateTimeKind.Unspecified && parms.Config.DateTimeKindToUseWhenUnspecified.HasValue)
				{
					value = DateTime.SpecifyKind(value, parms.Config.DateTimeKindToUseWhenUnspecified.Value);
				}
				if (value2.Kind == DateTimeKind.Unspecified && parms.Config.DateTimeKindToUseWhenUnspecified.HasValue)
				{
					value2 = DateTime.SpecifyKind(value2, parms.Config.DateTimeKindToUseWhenUnspecified.Value);
				}
				value = value.ToUniversalTime();
				value2 = value2.ToUniversalTime();
			}
			if (Math.Abs(value.Subtract(value2).TotalMilliseconds) > (double)parms.Config.MaxMillisecondsDateDifference)
			{
				AddDifference(parms);
			}
		}
	}
	/// <summary>
	/// Compare DateTimeOffsets with the ability to ignore millisecond differences
	/// </summary>
	public class DateTimeOffSetComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public DateTimeOffSetComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both types are DateTimeOffset
		/// </summary>
		/// <param name="type1"></param>
		/// <param name="type2"></param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsDateTimeOffset(type1))
			{
				return TypeHelper.IsDateTimeOffset(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two DateTimeOffset
		/// </summary>
		/// <param name="parms"></param>
		public override void CompareType(CompareParms parms)
		{
			if (parms.Object1 == null || parms.Object2 == null)
			{
				return;
			}
			DateTimeOffset dateTimeOffset = (DateTimeOffset)parms.Object1;
			DateTimeOffset dateTimeOffset2 = (DateTimeOffset)parms.Object2;
			if (parms.Config.IgnoreDateTimeOffsetTimezones)
			{
				dateTimeOffset = dateTimeOffset.ToUniversalTime();
				dateTimeOffset2 = dateTimeOffset2.ToUniversalTime();
			}
			if (parms.Config.CompareDateTimeOffsetWithOffsets && Math.Abs((dateTimeOffset - dateTimeOffset2).TotalMilliseconds) > (double)parms.Config.MaxMillisecondsDateDifference)
			{
				AddDifference(parms);
			}
			else if (!parms.Config.CompareDateTimeOffsetWithOffsets)
			{
				DateTime dateTime = new DateTime(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second).AddMilliseconds(dateTimeOffset.Millisecond);
				DateTime value = new DateTime(dateTimeOffset2.Year, dateTimeOffset2.Month, dateTimeOffset2.Day, dateTimeOffset2.Hour, dateTimeOffset2.Minute, dateTimeOffset2.Second).AddMilliseconds(dateTimeOffset2.Millisecond);
				if (Math.Abs(dateTime.Subtract(value).TotalMilliseconds) > (double)parms.Config.MaxMillisecondsDateDifference)
				{
					AddDifference(parms);
				}
			}
		}
	}
	/// <summary>
	/// Compare Decimal values with the ability to specify the precision
	/// </summary>
	public class DecimalComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public DecimalComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both types are double
		/// </summary>
		/// <param name="type1"></param>
		/// <param name="type2"></param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsDecimal(type1))
			{
				return TypeHelper.IsDecimal(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two decimals
		/// </summary>
		/// <param name="parms"></param>
		public override void CompareType(CompareParms parms)
		{
			if (parms.Object1 != null && parms.Object2 != null)
			{
				decimal obj = (decimal)parms.Object1;
				decimal num = (decimal)parms.Object2;
				if (Math.Abs(obj - num) > parms.Config.DecimalPrecision)
				{
					AddDifference(parms);
				}
			}
		}
	}
	/// <summary>
	/// Logic to compare two dictionaries
	/// </summary>
	public class DictionaryComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public DictionaryComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both types are dictionaries
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if ((TypeHelper.IsIDictionary(type1) || type1 == null) && (TypeHelper.IsIDictionary(type2) || type2 == null))
			{
				if (type1 == null)
				{
					return !(type2 == null);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Compare two dictionaries
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			try
			{
				parms.Result.AddParent(parms.Object1);
				parms.Result.AddParent(parms.Object2);
				bool flag = DictionaryCountsDifferent(parms);
				if (!flag || !parms.Result.ExceededDifferences)
				{
					if (ShouldCompareByKeys(parms))
					{
						CompareByKeys(parms);
					}
					else if (parms.Config.IgnoreCollectionOrder)
					{
						new IgnoreOrderLogic(base.RootComparer).CompareEnumeratorIgnoreOrder(parms, flag);
					}
					else
					{
						CompareByEnumerator(parms);
					}
				}
			}
			finally
			{
				parms.Result.RemoveParent(parms.Object1);
				parms.Result.RemoveParent(parms.Object2);
			}
		}

		/// <summary>
		/// This is to handle funky situation of having a complex object as a key
		/// (In this case a dictionary as a key)
		/// https://github.com/GregFinzer/Compare-Net-Objects/issues/222
		/// </summary>
		/// <param name="parms"></param>
		/// <returns></returns>
		private static bool ShouldCompareByKeys(CompareParms parms)
		{
			bool result = true;
			if (parms.Object1 != null && ((IDictionary)parms.Object1).Keys.Count > 0)
			{
				IDictionaryEnumerator enumerator = ((IDictionary)parms.Object1).GetEnumerator();
				enumerator.MoveNext();
				result = enumerator.Key != null && TypeHelper.IsSimpleType(enumerator.Key.GetType());
			}
			return result;
		}

		private void CompareByKeys(CompareParms parms)
		{
			IDictionary dictionary = (IDictionary)parms.Object1;
			IDictionary dictionary2 = (IDictionary)parms.Object2;
			if (dictionary != null)
			{
				foreach (object key in dictionary.Keys)
				{
					string breadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, "[" + key.ToString() + "].Value");
					CompareParms parms2 = new CompareParms
					{
						Result = parms.Result,
						Config = parms.Config,
						ParentObject1 = parms.Object1,
						ParentObject2 = parms.Object2,
						Object1 = dictionary[key],
						Object2 = ((dictionary2 != null && dictionary2.Contains(key)) ? dictionary2[key] : null),
						BreadCrumb = breadCrumb
					};
					base.RootComparer.Compare(parms2);
					if (parms.Result.ExceededDifferences)
					{
						return;
					}
				}
			}
			if (dictionary2 == null)
			{
				return;
			}
			foreach (object key2 in dictionary2.Keys)
			{
				if (dictionary == null || !dictionary.Contains(key2))
				{
					string breadCrumb2 = AddBreadCrumb(parms.Config, parms.BreadCrumb, "[" + key2.ToString() + "].Value");
					CompareParms parms3 = new CompareParms
					{
						Result = parms.Result,
						Config = parms.Config,
						ParentObject1 = parms.Object1,
						ParentObject2 = parms.Object2,
						Object1 = null,
						Object2 = dictionary2[key2],
						BreadCrumb = breadCrumb2
					};
					base.RootComparer.Compare(parms3);
					if (parms.Result.ExceededDifferences)
					{
						break;
					}
				}
			}
		}

		private void CompareByEnumerator(CompareParms parms)
		{
			IDictionaryEnumerator enumerator = ((IDictionary)parms.Object1).GetEnumerator();
			IDictionaryEnumerator enumerator2 = ((IDictionary)parms.Object2).GetEnumerator();
			while (enumerator.MoveNext() && enumerator2.MoveNext())
			{
				string breadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, "Key");
				CompareParms parms2 = new CompareParms
				{
					Result = parms.Result,
					Config = parms.Config,
					ParentObject1 = parms.Object1,
					ParentObject2 = parms.Object2,
					Object1 = enumerator.Key,
					Object2 = enumerator2.Key,
					BreadCrumb = breadCrumb
				};
				base.RootComparer.Compare(parms2);
				if (parms.Result.ExceededDifferences)
				{
					break;
				}
				breadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, "Value");
				parms2 = new CompareParms
				{
					Result = parms.Result,
					Config = parms.Config,
					ParentObject1 = parms.Object1,
					ParentObject2 = parms.Object2,
					Object1 = enumerator.Value,
					Object2 = enumerator2.Value,
					BreadCrumb = breadCrumb
				};
				base.RootComparer.Compare(parms2);
				if (parms.Result.ExceededDifferences)
				{
					break;
				}
			}
		}

		private bool DictionaryCountsDifferent(CompareParms parms)
		{
			IDictionary dictionary = parms.Object1 as IDictionary;
			IDictionary dictionary2 = parms.Object2 as IDictionary;
			int num = dictionary?.Count ?? 0;
			int num2 = dictionary2?.Count ?? 0;
			if (num == num2)
			{
				return false;
			}
			Difference difference = new Difference
			{
				ParentObject1 = parms.ParentObject1,
				ParentObject2 = parms.ParentObject2,
				PropertyName = parms.BreadCrumb,
				Object1Value = num.ToString(CultureInfo.InvariantCulture),
				Object2Value = num2.ToString(CultureInfo.InvariantCulture),
				ChildPropertyName = "Count",
				Object1 = dictionary,
				Object2 = dictionary2
			};
			AddDifference(parms.Result, difference);
			return true;
		}
	}
	/// <summary>
	/// Compare Double values with the ability to specify the precision
	/// </summary>
	public class DoubleComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public DoubleComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both types are double
		/// </summary>
		/// <param name="type1"></param>
		/// <param name="type2"></param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsDouble(type1))
			{
				return TypeHelper.IsDouble(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two doubles
		/// </summary>
		/// <param name="parms"></param>
		public override void CompareType(CompareParms parms)
		{
			if (parms.Object1 == null || parms.Object2 == null)
			{
				return;
			}
			double num = (double)parms.Object1;
			double num2 = (double)parms.Object2;
			if ((!double.IsPositiveInfinity(num) || !double.IsPositiveInfinity(num2)) && (!double.IsNegativeInfinity(num) || !double.IsNegativeInfinity(num2)))
			{
				double num3 = num - num2;
				if (Math.Abs(num3) > parms.Config.DoublePrecision || (double.IsNaN(num3) && (!double.IsNaN(num) || !double.IsNaN(num2))))
				{
					AddDifference(parms);
				}
			}
		}
	}
	/// <summary>
	/// Logic to compare to enum values
	/// </summary>
	public class EnumComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor with a default root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public EnumComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both objects are of type enum
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (!TypeHelper.IsEnum(type1))
			{
				return TypeHelper.IsEnum(type2);
			}
			return true;
		}

		/// <summary>
		/// Compare two enums
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			if (parms.Object1?.ToString() != parms.Object2?.ToString())
			{
				AddDifference(parms);
			}
		}
	}
	/// <summary>
	/// Logic to compare two LINQ enumerators
	/// </summary>
	public class EnumerableComparer : BaseTypeComparer
	{
		private readonly ListComparer _compareIList;

		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public EnumerableComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
			_compareIList = new ListComparer(rootComparer);
		}

		/// <summary>
		/// Returns true if either object is of type LINQ Enumerator
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (type1 == null || type2 == null)
			{
				return false;
			}
			if (!TypeHelper.IsEnumerable(type1))
			{
				return TypeHelper.IsEnumerable(type2);
			}
			return true;
		}

		/// <summary>
		/// Compare two objects that implement LINQ Enumerator
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			object @object = parms.Object1;
			object object2 = parms.Object2;
			try
			{
				parms.Result.AddParent(parms.Object1);
				parms.Result.AddParent(parms.Object2);
				Type type = parms.Object1.GetType();
				Type type2 = parms.Object2.GetType();
				object object3 = (TypeHelper.IsEnumerable(type) ? ConvertEnumerableToList(parms.Object1) : parms.Object1);
				object object4 = (TypeHelper.IsEnumerable(type2) ? ConvertEnumerableToList(parms.Object2) : parms.Object2);
				parms.Object1 = object3;
				parms.Object2 = object4;
				_compareIList.CompareType(parms);
			}
			finally
			{
				parms.Result.RemoveParent(@object);
				parms.Result.RemoveParent(object2);
			}
		}

		private object ConvertEnumerableToList(object source)
		{
			Type type = source.GetType();
			if (type.IsArray)
			{
				return source;
			}
			Type type2 = null;
			Type[] interfaces = type.GetInterfaces();
			foreach (Type type3 in interfaces)
			{
				if (type3.GetTypeInfo().IsGenericType && type3.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				{
					type2 = type3.GetGenericArguments()[0];
					break;
				}
			}
			if (type2 == null)
			{
				throw new ApplicationException("Cannot get IEnumerable definition");
			}
			return typeof(Enumerable).GetMethod("ToList").MakeGenericMethod(type2).Invoke(null, new object[1] { source });
		}
	}
	/// <summary>
	/// Compare all the fields of a class or struct (Note: inherits from BaseComparer instead of TypeComparer).
	/// </summary>
	public class FieldComparer : BaseComparer
	{
		private readonly RootComparer _rootComparer;

		/// <summary>
		/// Constructor with a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public FieldComparer(RootComparer rootComparer)
		{
			_rootComparer = rootComparer;
		}

		/// <summary>
		/// Compare the fields of a class
		/// </summary>
		public void PerformCompareFields(CompareParms parms)
		{
			foreach (FieldInfo currentField in GetCurrentFields(parms))
			{
				CompareField(parms, currentField);
				if (parms.Result.ExceededDifferences)
				{
					break;
				}
			}
		}

		private void CompareField(CompareParms parms, FieldInfo item)
		{
			if ((!parms.Config.CompareChildren && TypeHelper.CanHaveChildren(item.FieldType)) || ExcludeLogic.ShouldExcludeMember(parms.Config, item, parms.Object1Type))
			{
				return;
			}
			FieldInfo secondFieldInfo = GetSecondFieldInfo(parms, item);
			if ((!parms.Config.IgnoreObjectTypes && !parms.Config.IgnoreConcreteTypes) || !(secondFieldInfo == null) || !parms.Config.IgnoreMissingFields)
			{
				object value = item.GetValue(parms.Object1);
				object obj = ((secondFieldInfo != null) ? secondFieldInfo.GetValue(parms.Object2) : null);
				bool flag = value != null && (value == parms.Object1 || parms.Result.IsParent(value));
				bool flag2 = obj != null && (obj == parms.Object2 || parms.Result.IsParent(obj));
				if ((!TypeHelper.IsClass(item.FieldType) && !TypeHelper.IsInterface(item.FieldType)) || !(flag | flag2))
				{
					string breadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, item.Name);
					CompareParms parms2 = new CompareParms
					{
						Result = parms.Result,
						Config = parms.Config,
						ParentObject1 = parms.Object1,
						ParentObject2 = parms.Object2,
						Object1 = value,
						Object2 = obj,
						BreadCrumb = breadCrumb
					};
					_rootComparer.Compare(parms2);
				}
			}
		}

		private static FieldInfo GetSecondFieldInfo(CompareParms parms, FieldInfo item)
		{
			FieldInfo result = null;
			if (parms.Config.IgnoreObjectTypes || parms.Config.IgnoreConcreteTypes)
			{
				foreach (FieldInfo item2 in Cache.GetFieldInfo(parms.Config, parms.Object2Type))
				{
					if (!(item2.Name != item.Name))
					{
						result = item2;
						break;
					}
				}
			}
			else
			{
				result = item;
			}
			return result;
		}

		private static IEnumerable<FieldInfo> GetCurrentFields(CompareParms parms)
		{
			IEnumerable<FieldInfo> enumerable = null;
			if (parms.Config.InterfaceMembers.Count > 0)
			{
				Type[] interfaces = parms.Object1Type.GetInterfaces();
				foreach (Type interfaceMember in parms.Config.InterfaceMembers)
				{
					if (Enumerable.Contains(interfaces, interfaceMember))
					{
						enumerable = Cache.GetFieldInfo(parms.Config, interfaceMember);
						break;
					}
				}
			}
			if (enumerable == null)
			{
				enumerable = Cache.GetFieldInfo(parms.Config, parms.Object1Type);
			}
			return enumerable;
		}
	}
	/// <summary>
	/// Logic to compare two hash sets
	/// </summary>
	public class HashSetComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public HashSetComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both objects are hash sets
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsHashSet(type1))
			{
				return TypeHelper.IsHashSet(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two hash sets
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			try
			{
				parms.Result.AddParent(parms.Object1);
				parms.Result.AddParent(parms.Object2);
				Type type = parms.Object1.GetType();
				parms.Object1Type = type;
				Type type2 = parms.Object2.GetType();
				parms.Object2Type = type2;
				bool countsDifferent = HashSetsDifferentCount(parms);
				if (!parms.Result.ExceededDifferences)
				{
					if (parms.Config.IgnoreCollectionOrder)
					{
						new IgnoreOrderLogic(base.RootComparer).CompareEnumeratorIgnoreOrder(parms, countsDifferent);
					}
					else
					{
						CompareItems(parms);
					}
				}
			}
			finally
			{
				parms.Result.RemoveParent(parms.Object1);
				parms.Result.RemoveParent(parms.Object2);
			}
		}

		private void CompareItems(CompareParms parms)
		{
			int num = 0;
			IEnumerator enumerator = (IEnumerator)Cache.GetMethod(parms.Object1Type, "GetEnumerator").Invoke(parms.Object1, null);
			IEnumerator enumerator2 = (IEnumerator)Cache.GetMethod(parms.Object2Type, "GetEnumerator").Invoke(parms.Object2, null);
			while (enumerator.MoveNext() && enumerator2.MoveNext())
			{
				string breadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, string.Empty, string.Empty, num);
				CompareParms parms2 = new CompareParms
				{
					Result = parms.Result,
					Config = parms.Config,
					ParentObject1 = parms.Object1,
					ParentObject2 = parms.Object2,
					Object1 = enumerator.Current,
					Object2 = enumerator2.Current,
					BreadCrumb = breadCrumb
				};
				base.RootComparer.Compare(parms2);
				if (parms.Result.ExceededDifferences)
				{
					break;
				}
				num++;
			}
		}

		private bool HashSetsDifferentCount(CompareParms parms)
		{
			int num = (int)Cache.GetPropertyValue(parms.Result.Config, parms.Object1Type, parms.Object1, "Count");
			int num2 = (int)Cache.GetPropertyValue(parms.Result.Config, parms.Object2Type, parms.Object2, "Count");
			if (num != num2)
			{
				Difference difference = new Difference
				{
					ParentObject1 = parms.ParentObject1,
					ParentObject2 = parms.ParentObject2,
					PropertyName = parms.BreadCrumb,
					Object1Value = num.ToString(CultureInfo.InvariantCulture),
					Object2Value = num2.ToString(CultureInfo.InvariantCulture),
					ChildPropertyName = "Count",
					Object1 = parms.Object1,
					Object2 = parms.Object2
				};
				AddDifference(parms.Result, difference);
				return true;
			}
			return false;
		}
	}
	/// <summary>
	/// Compares System.Collections.Immutable.ImmutableArray
	/// </summary>
	public class ImmutableArrayComparer : BaseTypeComparer
	{
		private readonly ListComparer _listComparer;

		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public ImmutableArrayComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
			_listComparer = new ListComparer(rootComparer);
		}

		/// <summary>
		/// Returns true if both are immutable arrays with same generic argument type
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsImmutableArray(type1))
			{
				return TypeHelper.IsImmutableArray(type2);
			}
			return false;
		}

		/// <summary>
		/// Compares two immutable arrays
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			if (parms.Object1 != null && parms.Object2 != null)
			{
				parms.Object1 = ((IList)parms.Object1).Cast<object>().ToArray();
				parms.Object2 = ((IList)parms.Object2).Cast<object>().ToArray();
				_listComparer.CompareType(parms);
			}
		}
	}
	/// <summary>
	/// Logic to compare an integer indexer (Note, inherits from BaseComparer, not TypeComparer)
	/// </summary>
	public class IndexerComparer : BaseComparer
	{
		private readonly RootComparer _rootComparer;

		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public IndexerComparer(RootComparer rootComparer)
		{
			_rootComparer = rootComparer;
		}

		/// <summary>
		/// Compare an integer indexer
		/// </summary>
		public void CompareIndexer(CompareParms parms, PropertyEntity info, PropertyEntity secondObjectInfo)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			Type reflectedType = info.ReflectedType;
			Type reflectedType2 = secondObjectInfo.ReflectedType;
			if (reflectedType == null || reflectedType2 == null)
			{
				throw new ArgumentNullException("info");
			}
			int num = (int)reflectedType.GetProperty("Count").GetGetMethod().Invoke(parms.Object1, new object[0]);
			int num2 = (int)reflectedType2.GetProperty("Count").GetGetMethod().Invoke(parms.Object2, new object[0]);
			bool countsDifferent = IndexersHaveDifferentLength(parms, info, num, num2);
			if (parms.Result.ExceededDifferences)
			{
				return;
			}
			if (parms.Config.IgnoreCollectionOrder)
			{
				IndexerCollectionLooper @object = new IndexerCollectionLooper(parms.Object1, info.PropertyInfo, num);
				IndexerCollectionLooper object2 = new IndexerCollectionLooper(parms.Object2, secondObjectInfo.PropertyInfo, num2);
				CompareParms parms2 = new CompareParms
				{
					Result = parms.Result,
					Config = parms.Config,
					ParentObject1 = parms.Object1,
					ParentObject2 = parms.Object2,
					Object1 = @object,
					Object2 = object2,
					BreadCrumb = parms.BreadCrumb
				};
				new IgnoreOrderLogic(_rootComparer).CompareEnumeratorIgnoreOrder(parms2, countsDifferent);
				return;
			}
			for (int i = 0; i < num; i++)
			{
				string breadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, info.Name, string.Empty, i);
				object value = info.PropertyInfo.GetValue(parms.Object1, new object[1] { i });
				object object3 = null;
				if (i < num2)
				{
					object3 = secondObjectInfo.PropertyInfo.GetValue(parms.Object2, new object[1] { i });
				}
				CompareParms parms3 = new CompareParms
				{
					Result = parms.Result,
					Config = parms.Config,
					ParentObject1 = parms.Object1,
					ParentObject2 = parms.Object2,
					Object1 = value,
					Object2 = object3,
					BreadCrumb = breadCrumb
				};
				_rootComparer.Compare(parms3);
				if (parms.Result.ExceededDifferences)
				{
					return;
				}
			}
			if (num >= num2)
			{
				return;
			}
			for (int j = num; j < num2; j++)
			{
				string breadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, info.Name, string.Empty, j);
				object value2 = secondObjectInfo.PropertyInfo.GetValue(parms.Object2, new object[1] { j });
				CompareParms parms4 = new CompareParms
				{
					Result = parms.Result,
					Config = parms.Config,
					ParentObject1 = parms.Object1,
					ParentObject2 = parms.Object2,
					Object1 = null,
					Object2 = value2,
					BreadCrumb = breadCrumb
				};
				_rootComparer.Compare(parms4);
				if (parms.Result.ExceededDifferences)
				{
					break;
				}
			}
		}

		private bool IndexersHaveDifferentLength(CompareParms parms, PropertyEntity info, int indexerCount1, int indexerCount2)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			if (indexerCount1 != indexerCount2)
			{
				string propertyName = AddBreadCrumb(parms.Config, parms.BreadCrumb, info.Name);
				Difference difference = new Difference
				{
					ParentObject1 = parms.ParentObject1,
					ParentObject2 = parms.ParentObject2,
					PropertyName = propertyName,
					Object1Value = indexerCount1.ToString(CultureInfo.InvariantCulture),
					Object2Value = indexerCount2.ToString(CultureInfo.InvariantCulture),
					ChildPropertyName = "Count",
					Object1 = parms.Object1,
					Object2 = parms.Object2
				};
				AddDifference(parms.Result, difference);
				return true;
			}
			return false;
		}
	}
	/// <summary>
	/// Compare objects that implement IList
	/// </summary>
	public class ListComparer : BaseTypeComparer
	{
		private readonly PropertyComparer _propertyComparer;

		private readonly FieldComparer _fieldComparer;

		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public ListComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
			_propertyComparer = new PropertyComparer(rootComparer);
			_fieldComparer = new FieldComparer(rootComparer);
		}

		/// <summary>
		/// Returns true if both objects implement IList
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsIList(type1))
			{
				return TypeHelper.IsIList(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two objects that implement IList
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			if (parms.Object1 == null || parms.Object2 == null)
			{
				return;
			}
			try
			{
				parms.Result.AddParent(parms.Object1);
				parms.Result.AddParent(parms.Object2);
				Type type = parms.Object1.GetType();
				Type type2 = parms.Object2.GetType();
				if (ExcludeLogic.ShouldExcludeClass(parms.Config, type, type2))
				{
					return;
				}
				parms.Object1Type = type;
				parms.Object2Type = type2;
				if (!parms.Result.ExceededDifferences)
				{
					bool countsDifferent = ListsHaveDifferentCounts(parms);
					if (parms.Config.IgnoreCollectionOrder && !ChildShouldBeComparedWithoutOrder(parms))
					{
						new IgnoreOrderLogic(base.RootComparer).CompareEnumeratorIgnoreOrder(parms, countsDifferent);
					}
					else
					{
						CompareItems(parms);
					}
					CompareProperties(parms);
					CompareFields(parms);
				}
			}
			finally
			{
				parms.Result.RemoveParent(parms.Object1);
				parms.Result.RemoveParent(parms.Object2);
			}
		}

		private void CompareFields(CompareParms parms)
		{
			if (parms.Config.CompareFields)
			{
				_fieldComparer.PerformCompareFields(parms);
			}
		}

		private void CompareProperties(CompareParms parms)
		{
			if (parms.Config.CompareProperties)
			{
				_propertyComparer.PerformCompareProperties(parms, ignoreBaseList: true);
			}
		}

		private bool ListsHaveDifferentCounts(CompareParms parms)
		{
			IList list = parms.Object1 as IList;
			IList list2 = parms.Object2 as IList;
			if (list == null)
			{
				throw new ArgumentException("parms.Object1");
			}
			if (list2 == null)
			{
				throw new ArgumentException("parms.Object2");
			}
			try
			{
				if (list.Count != list2.Count)
				{
					Difference difference = new Difference
					{
						ParentObject1 = parms.ParentObject1,
						ParentObject2 = parms.ParentObject2,
						PropertyName = parms.BreadCrumb,
						Object1Value = list.Count.ToString(CultureInfo.InvariantCulture),
						Object2Value = list2.Count.ToString(CultureInfo.InvariantCulture),
						ChildPropertyName = "Count",
						Object1 = list,
						Object2 = list2
					};
					AddDifference(parms.Result, difference);
					return true;
				}
			}
			catch (ObjectDisposedException)
			{
				if (!parms.Config.IgnoreObjectDisposedException)
				{
					throw;
				}
				return true;
			}
			return false;
		}

		private bool ChildShouldBeComparedWithoutOrder(CompareParms parms)
		{
			IEnumerator enumerator = ((IEnumerable)parms.Object1).GetEnumerator();
			bool flag = false;
			List<bool> list = new List<bool>();
			while (enumerator.MoveNext())
			{
				flag = true;
				if (enumerator.Current != null)
				{
					Type type = enumerator.Current.GetType();
					bool item = TypeHelper.IsEnumerable(type) || TypeHelper.IsIList(type) || TypeHelper.IsIDictionary(type);
					list.Add(item);
				}
			}
			if (flag && list.Count > 0)
			{
				bool firstResult = list[0];
				if (list.Any((bool x) => x != firstResult))
				{
					throw new NotSupportedException("Collection has nested collections and some other types. IgnoreCollectionOrder should be false for such cases.");
				}
				return firstResult;
			}
			return flag;
		}

		private void CompareItems(CompareParms parms)
		{
			int num = 0;
			IEnumerator enumerator = ((IList)parms.Object1).GetEnumerator();
			IEnumerator enumerator2 = ((IList)parms.Object2).GetEnumerator();
			while (enumerator.MoveNext() && enumerator2.MoveNext())
			{
				string breadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, string.Empty, string.Empty, num);
				CompareParms parms2 = new CompareParms
				{
					Result = parms.Result,
					Config = parms.Config,
					ParentObject1 = parms.Object1,
					ParentObject2 = parms.Object2,
					Object1 = enumerator.Current,
					Object2 = enumerator2.Current,
					BreadCrumb = breadCrumb
				};
				base.RootComparer.Compare(parms2);
				if (parms.Result.ExceededDifferences)
				{
					break;
				}
				num++;
			}
		}
	}
	/// <summary>
	/// Logic to compare to pointers
	/// </summary>
	public class PointerComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public PointerComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both types are a pointer
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsPointer(type1))
			{
				return TypeHelper.IsPointer(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two pointers
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			if ((parms.Object1 is nint && parms.Object2 is nint && (nint)parms.Object1 != (nint)parms.Object2) || (parms.Object1 is nuint && parms.Object2 is nuint && (nuint)parms.Object1 != (nuint)parms.Object2))
			{
				Difference difference = new Difference
				{
					ParentObject1 = parms.ParentObject1,
					ParentObject2 = parms.ParentObject2,
					PropertyName = parms.BreadCrumb,
					Object1 = parms.Object1,
					Object2 = parms.Object2
				};
				AddDifference(parms.Result, difference);
			}
		}
	}
	/// <summary>
	/// Compare two properties (Note: inherits from BaseComparer instead of TypeComparer).
	/// </summary>
	public class PropertyComparer : BaseComparer
	{
		private readonly RootComparer _rootComparer;

		private readonly IndexerComparer _indexerComparer;

		private static readonly string[] _baseList = new string[3] { "Count", "Capacity", "Item" };

		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public PropertyComparer(RootComparer rootComparer)
		{
			_rootComparer = rootComparer;
			_indexerComparer = new IndexerComparer(rootComparer);
		}

		/// <summary>
		/// Compare the properties of a class
		/// </summary>
		public void PerformCompareProperties(CompareParms parms, bool ignoreBaseList = false)
		{
			List<PropertyEntity> currentProperties = GetCurrentProperties(parms, parms.Object1, parms.Object1Type);
			List<PropertyEntity> currentProperties2 = GetCurrentProperties(parms, parms.Object2, parms.Object2Type);
			foreach (PropertyEntity item in currentProperties)
			{
				if (!ignoreBaseList || !Enumerable.Contains(_baseList, item.Name))
				{
					CompareProperty(parms, item, currentProperties2);
					if (parms.Result.ExceededDifferences)
					{
						break;
					}
				}
			}
		}

		/// <summary>
		/// Compare a single property of a class
		/// </summary>
		/// <param name="parms"></param>
		/// <param name="info"></param>
		/// <param name="object2Properties"></param>
		private void CompareProperty(CompareParms parms, PropertyEntity info, List<PropertyEntity> object2Properties)
		{
			if (!info.CanRead || (!parms.Config.CompareChildren && TypeHelper.CanHaveChildren(info.PropertyType)) || (info.PropertyInfo != null && ExcludeLogic.ShouldExcludeMember(parms.Config, info.PropertyInfo, info.DeclaringType)) || (info.IsDynamic && ExcludeLogic.ShouldExcludeDynamicMember(parms.Config, info.Name, info.DeclaringType)) || (!parms.Config.CompareReadOnly && !info.CanWrite))
			{
				return;
			}
			PropertyEntity propertyEntity = ((!parms.Config.IgnoreObjectTypes) ? object2Properties.FirstOrDefault((PropertyEntity x) => x.Name == info.Name && x.PropertyType == info.PropertyType) : (object2Properties.FirstOrDefault((PropertyEntity x) => x.Name == info.Name && x.PropertyType == info.PropertyType) ?? object2Properties.FirstOrDefault((PropertyEntity x) => x.Name == info.Name)));
			if ((parms.Config.IgnoreObjectTypes || parms.Config.IgnoreConcreteTypes) && propertyEntity == null && parms.Config.IgnoreMissingProperties)
			{
				return;
			}
			BaseTypeComparer customPropertyComparer = null;
			if (info.PropertyInfo != null)
			{
				customPropertyComparer = CustomValidationLogic.CustomValidatorForMember(parms.Config, info.PropertyInfo, info.DeclaringType) ?? CustomValidationLogic.CustomValidatorForDynamicMember(parms.Config, info.Name, info.DeclaringType);
			}
			if (!IsValidIndexer(parms.Config, info, parms.BreadCrumb))
			{
				object value = info.Value;
				object obj = propertyEntity?.Value;
				bool flag = value != null && (value == parms.Object1 || parms.Result.IsParent(value));
				bool flag2 = obj != null && (obj == parms.Object2 || parms.Result.IsParent(obj));
				if ((!TypeHelper.IsClass(info.PropertyType) && !TypeHelper.IsInterface(info.PropertyType) && !TypeHelper.IsStruct(info.PropertyType)) || !(flag & flag2))
				{
					string breadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, info.Name);
					CompareParms parms2 = new CompareParms
					{
						Result = parms.Result,
						Config = parms.Config,
						ParentObject1 = parms.Object1,
						ParentObject2 = parms.Object2,
						Object1 = value,
						Object2 = obj,
						BreadCrumb = breadCrumb,
						CustomPropertyComparer = customPropertyComparer,
						Object1DeclaredType = info?.PropertyType,
						Object2DeclaredType = propertyEntity?.PropertyType
					};
					_rootComparer.Compare(parms2);
				}
			}
			else
			{
				_indexerComparer.CompareIndexer(parms, info, propertyEntity);
			}
		}

		private static List<PropertyEntity> GetCurrentProperties(CompareParms parms, object objectValue, Type objectType)
		{
			return HandleDynamicObject(objectValue, objectType) ?? HandleInterfaceMembers(parms, objectValue, objectType) ?? HandleNormalProperties(parms, objectValue, objectType);
		}

		private static List<PropertyEntity> HandleNormalProperties(CompareParms parms, object objectValue, Type objectType)
		{
			IEnumerable<PropertyInfo> propertyInfo = Cache.GetPropertyInfo(parms.Result.Config, objectType);
			return AddPropertyInfos(parms, objectValue, objectType, propertyInfo);
		}

		private static List<PropertyEntity> AddPropertyInfos(CompareParms parms, object objectValue, Type objectType, IEnumerable<PropertyInfo> properties)
		{
			List<PropertyEntity> list = new List<PropertyEntity>();
			foreach (PropertyInfo property in properties)
			{
				if (ExcludeLogic.ShouldExcludeMember(parms.Config, property, objectType) || (!property.CanRead && !parms.Config.CompareReadOnly))
				{
					continue;
				}
				PropertyEntity propertyEntity = new PropertyEntity();
				propertyEntity.IsDynamic = false;
				propertyEntity.Name = property.Name;
				propertyEntity.CanRead = property.CanRead;
				propertyEntity.CanWrite = property.CanWrite;
				propertyEntity.PropertyType = property.PropertyType;
				propertyEntity.ReflectedType = property.ReflectedType;
				propertyEntity.Indexers.AddRange(property.GetIndexParameters());
				propertyEntity.DeclaringType = objectType;
				if (propertyEntity.CanRead && propertyEntity.Indexers.Count == 0)
				{
					try
					{
						propertyEntity.Value = property.GetValue(objectValue, null);
					}
					catch (TargetInvocationException)
					{
					}
					catch (NotSupportedException)
					{
					}
				}
				propertyEntity.PropertyInfo = property;
				list.Add(propertyEntity);
			}
			return list;
		}

		private static List<PropertyEntity> HandleInterfaceMembers(CompareParms parms, object objectValue, Type objectType)
		{
			List<PropertyEntity> list = new List<PropertyEntity>();
			if (parms.Config.InterfaceMembers.Count > 0)
			{
				Type[] interfaces = objectType.GetInterfaces();
				foreach (Type interfaceMember in parms.Config.InterfaceMembers)
				{
					if (!Enumerable.Contains(interfaces, interfaceMember))
					{
						continue;
					}
					foreach (PropertyInfo item in Cache.GetPropertyInfo(parms.Result.Config, interfaceMember))
					{
						PropertyEntity propertyEntity = new PropertyEntity();
						propertyEntity.IsDynamic = false;
						propertyEntity.Name = item.Name;
						propertyEntity.CanRead = item.CanRead;
						propertyEntity.CanWrite = item.CanWrite;
						propertyEntity.PropertyType = item.PropertyType;
						propertyEntity.Indexers.AddRange(item.GetIndexParameters());
						propertyEntity.DeclaringType = objectType;
						if (propertyEntity.Indexers.Count == 0)
						{
							propertyEntity.Value = item.GetValue(objectValue, null);
						}
						propertyEntity.PropertyInfo = item;
						list.Add(propertyEntity);
					}
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			return list;
		}

		private static List<PropertyEntity> HandleDynamicObject(object objectValue, Type objectType)
		{
			if (TypeHelper.IsExpandoObject(objectValue))
			{
				return AddExpandoPropertyValues(objectValue, objectType);
			}
			return null;
		}

		private static List<PropertyEntity> AddExpandoPropertyValues(object objectValue, Type objectType)
		{
			if (!(objectValue is IDictionary<string, object> dictionary))
			{
				return new List<PropertyEntity>();
			}
			List<PropertyEntity> list = new List<PropertyEntity>();
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				PropertyEntity propertyEntity = new PropertyEntity();
				propertyEntity.IsDynamic = true;
				propertyEntity.Name = item.Key;
				propertyEntity.Value = item.Value;
				propertyEntity.CanRead = true;
				propertyEntity.CanWrite = true;
				propertyEntity.DeclaringType = objectType;
				if (item.Value == null)
				{
					propertyEntity.PropertyType = null;
					propertyEntity.ReflectedType = null;
				}
				else
				{
					propertyEntity.PropertyType = item.GetType();
					propertyEntity.ReflectedType = propertyEntity.PropertyType;
				}
				list.Add(propertyEntity);
			}
			return list;
		}

		private bool IsValidIndexer(ComparisonConfig config, PropertyEntity info, string breadCrumb)
		{
			if (info.Indexers.Count == 0)
			{
				return false;
			}
			if (info.Indexers.Count > 1)
			{
				if (config.SkipInvalidIndexers)
				{
					return false;
				}
				throw new Exception("Cannot compare objects with more than one indexer for object " + breadCrumb);
			}
			if (info.Indexers[0].ParameterType != typeof(int))
			{
				if (config.SkipInvalidIndexers)
				{
					return false;
				}
				throw new Exception("Cannot compare objects with a non integer indexer for object " + breadCrumb);
			}
			Type reflectedType = info.ReflectedType;
			if (reflectedType == null)
			{
				if (config.SkipInvalidIndexers)
				{
					return false;
				}
				throw new Exception("Cannot compare objects with a null indexer for object " + breadCrumb);
			}
			if (reflectedType.GetProperty("Count") == null || reflectedType.GetProperty("Count").PropertyType != typeof(int))
			{
				if (config.SkipInvalidIndexers)
				{
					return false;
				}
				throw new Exception("Indexer must have a corresponding Count property that is an integer for object " + breadCrumb);
			}
			return true;
		}
	}
	/// <summary>
	/// Logic to compare two ReadOnlyCollections.
	/// </summary>
	public class ReadOnlyCollectionComparer : BaseTypeComparer
	{
		/// <summary>
		/// The main constructor.
		/// </summary>
		public ReadOnlyCollectionComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both objects are ReadOnlyCollections.
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsReadOnlyCollection(type1))
			{
				return TypeHelper.IsReadOnlyCollection(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two ReadOnlyCollections.
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			if (!parms.Config.CompareReadOnly)
			{
				return;
			}
			try
			{
				parms.Result.AddParent(parms.Object1);
				parms.Result.AddParent(parms.Object2);
				Type type = parms.Object1.GetType();
				Type type2 = parms.Object2.GetType();
				if (ExcludeLogic.ShouldExcludeClass(parms.Config, type, type2))
				{
					return;
				}
				parms.Object1Type = type;
				parms.Object2Type = type2;
				bool countsDifferent = CollectionsDifferentCount(parms);
				if (!parms.Result.ExceededDifferences)
				{
					if (parms.Config.IgnoreCollectionOrder)
					{
						new IgnoreOrderLogic(base.RootComparer).CompareEnumeratorIgnoreOrder(parms, countsDifferent);
					}
					else
					{
						CompareItems(parms);
					}
				}
			}
			finally
			{
				parms.Result.RemoveParent(parms.Object1);
				parms.Result.RemoveParent(parms.Object2);
			}
		}

		private void CompareItems(CompareParms parms)
		{
			int num = 0;
			IEnumerator enumerator = ((ICollection)parms.Object1).GetEnumerator();
			IEnumerator enumerator2 = ((ICollection)parms.Object2).GetEnumerator();
			while (enumerator.MoveNext() && enumerator2.MoveNext())
			{
				string breadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, string.Empty, string.Empty, num);
				CompareParms parms2 = new CompareParms
				{
					Result = parms.Result,
					Config = parms.Config,
					ParentObject1 = parms.Object1,
					ParentObject2 = parms.Object2,
					Object1 = enumerator.Current,
					Object2 = enumerator2.Current,
					BreadCrumb = breadCrumb
				};
				base.RootComparer.Compare(parms2);
				if (parms.Result.ExceededDifferences)
				{
					break;
				}
				num++;
			}
		}

		private bool CollectionsDifferentCount(CompareParms parms)
		{
			int count = ((ICollection)parms.Object1).Count;
			int count2 = ((ICollection)parms.Object2).Count;
			if (count != count2)
			{
				Difference difference = new Difference
				{
					ParentObject1 = parms.ParentObject1,
					ParentObject2 = parms.ParentObject2,
					PropertyName = parms.BreadCrumb,
					Object1Value = count.ToString(CultureInfo.InvariantCulture),
					Object2Value = count2.ToString(CultureInfo.InvariantCulture),
					ChildPropertyName = "Count",
					Object1 = parms.Object1,
					Object2 = parms.Object2
				};
				AddDifference(parms.Result, difference);
				return true;
			}
			return false;
		}
	}
	/// <summary>
	/// Logic to compare two runtime types
	/// </summary>
	public class RuntimeTypeComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public RuntimeTypeComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both types are of type runtme type
		/// </summary>
		/// <param name="type1"></param>
		/// <param name="type2"></param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsRuntimeType(type1))
			{
				return TypeHelper.IsRuntimeType(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two runtime types
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			Type type = (Type)parms.Object1;
			Type type2 = (Type)parms.Object2;
			if (type.FullName != type2.FullName)
			{
				Difference difference = new Difference
				{
					ParentObject1 = parms.ParentObject1,
					ParentObject2 = parms.ParentObject2,
					PropertyName = parms.BreadCrumb,
					Object1Value = type.FullName,
					Object2Value = type2.FullName,
					ChildPropertyName = "FullName",
					Object1 = parms.Object1,
					Object2 = parms.Object2
				};
				AddDifference(parms.Result, difference);
			}
		}
	}
	/// <summary>
	/// Compare primitive types (long, int, short, byte etc.) and DateTime, decimal, and Guid
	/// </summary>
	public class SimpleTypeComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public SimpleTypeComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if the type is a simple type
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsSimpleType(type1))
			{
				return TypeHelper.IsSimpleType(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two simple types
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			if (parms.Object1 != null && parms.Object2 != null && ((parms.Object1 as IComparable) ?? throw new Exception("Expected value does not implement IComparable")).CompareTo(parms.Object2) != 0)
			{
				AddDifference(parms);
			}
		}
	}
	/// <summary>
	/// Compare two StringBuilders
	/// </summary>
	public class StringBuilderComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public StringBuilderComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both objects are a StringBuilder
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if ((!TypeHelper.IsStringBuilder(type1) || !TypeHelper.IsStringBuilder(type2)) && (!TypeHelper.IsStringBuilder(type1) || !(type2 == null)))
			{
				if (TypeHelper.IsStringBuilder(type2))
				{
					return type1 == null;
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// Compare two string builders
		/// </summary>
		/// <param name="parms"></param>
		public override void CompareType(CompareParms parms)
		{
			if (parms.Config.TreatStringEmptyAndNullTheSame && ((parms.Object1 == null && parms.Object2 != null && parms.Object2.ToString() == string.Empty) || (parms.Object2 == null && parms.Object1 != null && parms.Object1.ToString() == string.Empty)))
			{
				return;
			}
			string text = parms.Object1.ToString();
			string text2 = parms.Object2.ToString();
			if (parms.Config.IgnoreStringLeadingTrailingWhitespace)
			{
				text = text.Trim();
				text2 = text2.Trim();
			}
			if (!parms.Config.CaseSensitive)
			{
				if (!string.Equals(text, text2, StringComparison.OrdinalIgnoreCase))
				{
					AddDifference(parms);
				}
			}
			else if (text != text2)
			{
				AddDifference(parms);
			}
		}
	}
	/// <summary>
	/// Compare two strings
	/// </summary>
	public class StringComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public StringComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both objects are a string or if one is a string and one is a a null
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if ((!TypeHelper.IsString(type1) || !TypeHelper.IsString(type2)) && (!TypeHelper.IsString(type1) || !(type2 == null)))
			{
				if (TypeHelper.IsString(type2))
				{
					return type1 == null;
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// Compare two strings
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			if ((parms.Config.TreatStringEmptyAndNullTheSame && ((parms.Object1 == null && parms.Object2 != null && parms.Object2.ToString() == string.Empty) || (parms.Object2 == null && parms.Object1 != null && parms.Object1.ToString() == string.Empty))) || OneOfTheStringsIsNull(parms))
			{
				return;
			}
			string text = parms.Object1 as string;
			string text2 = parms.Object2 as string;
			if (parms.Config.IgnoreStringLeadingTrailingWhitespace)
			{
				text = text.Trim();
				text2 = text2.Trim();
			}
			if (!parms.Config.CaseSensitive)
			{
				if (!string.Equals(text, text2, StringComparison.OrdinalIgnoreCase))
				{
					AddDifference(parms);
				}
			}
			else if (text != text2)
			{
				AddDifference(parms);
			}
		}

		private bool OneOfTheStringsIsNull(CompareParms parms)
		{
			if (parms.Object1 == null || parms.Object2 == null)
			{
				AddDifference(parms);
				return true;
			}
			return false;
		}
	}
	/// <summary>
	/// Compare two structs
	/// </summary>
	public class StructComparer : BaseTypeComparer
	{
		private readonly PropertyComparer _propertyComparer;

		private readonly FieldComparer _fieldComparer;

		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public StructComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
			_propertyComparer = new PropertyComparer(rootComparer);
			_fieldComparer = new FieldComparer(rootComparer);
		}

		/// <summary>
		/// Returns true if both objects are of type struct
		/// </summary>
		/// <param name="type1"></param>
		/// <param name="type2"></param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsStruct(type1))
			{
				return TypeHelper.IsStruct(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two structs
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			if (parms.Result.CurrentStructDepth >= parms.Config.MaxStructDepth)
			{
				return;
			}
			try
			{
				parms.Result.CurrentStructDepth++;
				parms.Object1Type = parms.Object1.GetType();
				parms.Object2Type = parms.Object2.GetType();
				if (parms.Config.CompareFields)
				{
					_fieldComparer.PerformCompareFields(parms);
				}
				if (parms.Config.CompareProperties)
				{
					_propertyComparer.PerformCompareProperties(parms);
				}
			}
			finally
			{
				parms.Result.CurrentStructDepth--;
			}
		}
	}
	/// <summary>
	/// Compare dates
	/// </summary>
	public sealed class TimeOnlyComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public TimeOnlyComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both types are TimeOnly
		/// </summary>
		/// <param name="type1"></param>
		/// <param name="type2"></param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsTimeOnly(type1))
			{
				return TypeHelper.IsTimeOnly(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two DateOnly variables
		/// </summary>
		/// <param name="parms"></param>
		public override void CompareType(CompareParms parms)
		{
			if (parms.Object1 != null && parms.Object2 != null)
			{
				TimeOnly timeOnly = (TimeOnly)parms.Object1;
				TimeOnly value = (TimeOnly)parms.Object2;
				if (!timeOnly.Equals(value))
				{
					AddDifference(parms);
				}
			}
		}
	}
	/// <summary>
	/// Logic to compare two timespans
	/// </summary>
	public class TimespanComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public TimespanComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both objects are timespans
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsTimespan(type1))
			{
				return TypeHelper.IsTimespan(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two timespans
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			if (((TimeSpan)parms.Object1).Ticks != ((TimeSpan)parms.Object2).Ticks)
			{
				Difference difference = new Difference
				{
					ParentObject1 = parms.ParentObject1,
					ParentObject2 = parms.ParentObject2,
					PropertyName = parms.BreadCrumb,
					Object1Value = ((TimeSpan)parms.Object1).Ticks.ToString(CultureInfo.InvariantCulture),
					Object2Value = ((TimeSpan)parms.Object2).Ticks.ToString(CultureInfo.InvariantCulture),
					ChildPropertyName = "Ticks",
					Object1 = parms.Object1,
					Object2 = parms.Object2
				};
				AddDifference(parms.Result, difference);
			}
		}
	}
	/// <summary>
	/// Compare two URIs
	/// </summary>
	public class UriComparer : BaseTypeComparer
	{
		/// <summary>
		/// Constructor that takes a root comparer
		/// </summary>
		/// <param name="rootComparer"></param>
		public UriComparer(RootComparer rootComparer)
			: base(rootComparer)
		{
		}

		/// <summary>
		/// Returns true if both types are a URI
		/// </summary>
		/// <param name="type1">The type of the first object</param>
		/// <param name="type2">The type of the second object</param>
		/// <returns></returns>
		public override bool IsTypeMatch(Type type1, Type type2)
		{
			if (TypeHelper.IsUri(type1))
			{
				return TypeHelper.IsUri(type2);
			}
			return false;
		}

		/// <summary>
		/// Compare two URIs
		/// </summary>
		public override void CompareType(CompareParms parms)
		{
			Uri uri = parms.Object1 as Uri;
			Uri uri2 = parms.Object2 as Uri;
			if (!(uri == null) && !(uri2 == null) && uri.OriginalString != uri2.OriginalString)
			{
				Difference difference = new Difference
				{
					ParentObject1 = parms.ParentObject1,
					ParentObject2 = parms.ParentObject2,
					PropertyName = parms.BreadCrumb,
					Object1Value = NiceString(uri.OriginalString),
					Object2Value = NiceString(uri2.OriginalString),
					ChildPropertyName = "OriginalString",
					Object1 = parms.Object1,
					Object2 = parms.Object2
				};
				AddDifference(parms.Result, difference);
			}
		}
	}
}
namespace Snet.Utility.comparer.Reports
{
	/// <summary>
	/// Abstract Base Duel File Report that has default Output
	/// </summary>
	public abstract class BaseDualFileReport : IDualFileReport
	{
		/// <summary>
		/// Create two difference files and compare in WinMerge
		/// </summary>
		/// <param name="differences">A list of the differences</param>
		/// <param name="expectedFilePath">The path to write the expected results</param>
		/// <param name="actualFilePath">The path to write the actual results</param>
		public virtual void OutputFiles(List<Difference> differences, string expectedFilePath, string actualFilePath)
		{
			if (differences == null)
			{
				throw new ArgumentNullException("differences");
			}
			StringBuilder stringBuilder = new StringBuilder(differences.Count * 80);
			StringBuilder stringBuilder2 = new StringBuilder(differences.Count * 80);
			bool flag = differences.Any((Difference o) => o.Object1Value.Replace(Environment.NewLine, "|").Length > 80 || o.Object2Value.Replace(Environment.NewLine, "|").Length > 80);
			foreach (Difference difference in differences)
			{
				if (flag)
				{
					stringBuilder.Append(difference.GetShortItem());
					stringBuilder.Append(", ");
					stringBuilder.AppendLine(difference.Object1Value.Replace(Environment.NewLine, "|"));
					stringBuilder2.Append(difference.GetShortItem());
					stringBuilder2.Append(", ");
					stringBuilder2.AppendLine(difference.Object2Value.Replace(Environment.NewLine, "|"));
				}
				else
				{
					stringBuilder.Append(difference.Object1Value.Replace(Environment.NewLine, "|"));
					stringBuilder.Append(", ");
					stringBuilder.AppendLine(difference.GetShortItem());
					stringBuilder2.Append(difference.Object2Value.Replace(Environment.NewLine, "|"));
					stringBuilder2.Append(", ");
					stringBuilder2.AppendLine(difference.GetShortItem());
				}
			}
			if (string.IsNullOrEmpty(Path.GetDirectoryName(expectedFilePath)))
			{
				expectedFilePath = Path.Combine(FileHelper.GetCurrentDirectory(), expectedFilePath);
			}
			if (string.IsNullOrEmpty(Path.GetDirectoryName(actualFilePath)))
			{
				actualFilePath = Path.Combine(FileHelper.GetCurrentDirectory(), actualFilePath);
			}
			File.WriteAllText(expectedFilePath, stringBuilder.ToString());
			File.WriteAllText(actualFilePath, stringBuilder2.ToString());
		}

		/// <summary>
		/// Launch application to compare two files
		/// </summary>
		/// <param name="expectedFilePath">The path for the expected file results</param>
		/// <param name="actualFilePath">The path for the actual file results</param>
		public abstract void LaunchApplication(string expectedFilePath, string actualFilePath);
	}
	/// <summary>
	/// Output files and launch Beyond Compare
	/// </summary>
	public class BeyondCompareReport : BaseDualFileReport
	{
		private const string APPLICATION_NAME = "BCompare.exe";

		/// <summary>
		/// Launch Beyond Compare
		/// </summary>
		/// <param name="expectedFilePath">The path to write the expected results</param>
		/// <param name="actualFilePath">The path to write the actual results</param>
		public override void LaunchApplication(string expectedFilePath, string actualFilePath)
		{
			if (!EnvironmentHelper.IsWindows())
			{
				throw new NotSupportedException();
			}
			if (string.IsNullOrEmpty(Path.GetDirectoryName(expectedFilePath)))
			{
				expectedFilePath = Path.Combine(FileHelper.GetCurrentDirectory(), expectedFilePath);
			}
			if (string.IsNullOrEmpty(Path.GetDirectoryName(actualFilePath)))
			{
				actualFilePath = Path.Combine(FileHelper.GetCurrentDirectory(), actualFilePath);
			}
			string arguments = $"\"{expectedFilePath}\" \"{actualFilePath}\"";
			string text = FindBeyondCompare();
			if (string.IsNullOrEmpty(text))
			{
				throw new FileNotFoundException("BCompare.exe");
			}
			ProcessHelper.Shell(text, arguments, ProcessWindowStyle.Normal, waitUntilFinished: false);
		}

		/// <summary>
		/// Find the path of the Beyond Compare executable
		/// </summary>
		/// <returns>The path or null if not found</returns>
		public string FindBeyondCompare()
		{
			string environmentVariable = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
			if (!string.IsNullOrEmpty(environmentVariable))
			{
				foreach (string item in from o in Directory.GetDirectories(environmentVariable, "Beyond Compare*")
					orderby o descending
					select o)
				{
					string[] files = Directory.GetFiles(item, "BCompare.exe");
					if (files.Any())
					{
						return files.First();
					}
				}
			}
			environmentVariable = Environment.GetEnvironmentVariable("ProgramFiles");
			if (!string.IsNullOrEmpty(environmentVariable))
			{
				foreach (string item2 in from o in Directory.GetDirectories(environmentVariable, "Beyond Compare*")
					orderby o descending
					select o)
				{
					string[] files2 = Directory.GetFiles(item2, "BCompare.exe");
					if (files2.Any())
					{
						return files2.First();
					}
				}
			}
			return null;
		}
	}
	/// <summary>
	/// Create a CSV file of the differences and launch the default CSV handler (usually Excel)
	/// </summary>
	public class CsvReport : ISingleFileReport
	{
		/// <summary>
		/// Output the differences to a file
		/// </summary>
		/// <param name="differences">A list of differences</param>
		/// <param name="filePath">The file path</param>
		public void OutputFile(List<Difference> differences, string filePath)
		{
			if (string.IsNullOrEmpty(Path.GetDirectoryName(filePath)))
			{
				filePath = Path.Combine(FileHelper.GetCurrentDirectory(), filePath);
			}
			using FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
			using TextWriter writer = new StreamWriter(stream);
			WriteItOut(differences, writer);
		}

		private void WriteItOut(List<Difference> differences, TextWriter writer)
		{
			writer.WriteLine("Bread Crumb,Expected,Actual");
			foreach (Difference difference in differences)
			{
				writer.Write("{0},", EscapeString(difference.GetShortItem()));
				writer.Write("{0},", EscapeString(difference.Object1Value));
				writer.WriteLine("{0}", EscapeString(difference.Object2Value));
			}
		}

		/// <summary>
		/// Output the differences to a stream
		/// </summary>
		/// <param name="differences">A list of differences</param>
		/// <param name="stream">An output stream</param>
		public void OutputStream(List<Difference> differences, Stream stream)
		{
			TextWriter textWriter = new StreamWriter(stream);
			WriteItOut(differences, textWriter);
			textWriter.Flush();
			if (stream.CanSeek)
			{
				stream.Seek(0L, SeekOrigin.Begin);
			}
		}

		/// <summary>
		/// Output the differences to a string
		/// </summary>
		/// <param name="differences">A list of differences</param>
		/// <returns>A string</returns>
		public string OutputString(List<Difference> differences)
		{
			StringBuilder stringBuilder = new StringBuilder(differences.Count * 40);
			TextWriter writer = new StringWriter(stringBuilder);
			WriteItOut(differences, writer);
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Launch the application associated with CSV files
		/// </summary>
		/// <param name="filePath">The differences file</param>
		public void LaunchApplication(string filePath)
		{
			if (!EnvironmentHelper.IsWindows())
			{
				throw new NotSupportedException();
			}
			ProcessHelper.Shell(filePath, string.Empty, ProcessWindowStyle.Normal, waitUntilFinished: false);
		}

		/// <summary>
		/// Escape special characters
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private string EscapeString(object value)
		{
			if (value == null)
			{
				return string.Empty;
			}
			string text = value.ToString();
			if (text.Contains("\""))
			{
				text = text.Replace("\"", "\"\"");
			}
			if (text.Contains(",") || text.Contains(Environment.NewLine))
			{
				text = string.Format("{0}{1}{2}", "\"", text, "\"");
			}
			return text;
		}
	}
	/// <summary>
	/// Create an HTML file of the differences and launch the default HTML handler
	/// </summary>
	public class HtmlReport : ISingleFileReport
	{
		/// <summary>
		/// HtmlReport Configuration
		/// </summary>
		public HtmlConfig Config { get; set; }

		/// <summary>
		/// Default constructor, sets up Config object
		/// </summary>
		public HtmlReport()
		{
			Config = new HtmlConfig();
		}

		/// <summary>
		/// Output the differences to a file
		/// </summary>
		/// <param name="differences">A list of differences</param>
		/// <param name="filePath">The file path</param>
		public void OutputFile(List<Difference> differences, string filePath)
		{
			if (string.IsNullOrEmpty(Path.GetDirectoryName(filePath)))
			{
				filePath = Path.Combine(FileHelper.GetCurrentDirectory(), filePath);
			}
			using FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
			using TextWriter writer = new StreamWriter(stream);
			WriteItOut(differences, writer);
		}

		private void WriteItOut(List<Difference> differences, TextWriter writer)
		{
			if (Config.GenerateFullHtml)
			{
				writer.Write(Config.HtmlHeader);
			}
			writer.WriteLine("<table class=\"diff-table\">");
			writer.WriteLine("<thead><th>{0}</th><th>{1}</th><th>{2}</th></thead>", Config.BreadCrumbColumName, Config.ExpectedColumnName, Config.ActualColumnName);
			foreach (Difference difference in differences)
			{
				writer.WriteLine("<tr><td class=\"diff-crumb\">{0}</td><td class=\"diff-expected\">{1}</td><td class=\"diff-actual\">{2}</td></tr>", EscapeString(difference.GetShortItem()), EscapeString(difference.Object1Value), EscapeString(difference.Object2Value));
			}
			writer.WriteLine("</table>");
			if (Config.GenerateFullHtml)
			{
				writer.Write(Config.HtmlFooter);
			}
		}

		/// <summary>
		/// Launch the HTML Report
		/// </summary>
		/// <param name="filePath">The differences file</param>
		public void LaunchApplication(string filePath)
		{
			if (!EnvironmentHelper.IsWindows())
			{
				throw new NotSupportedException();
			}
			ProcessHelper.Shell(filePath, string.Empty, ProcessWindowStyle.Normal, waitUntilFinished: false);
		}

		/// <summary>
		/// Output the differences to a stream
		/// </summary>
		/// <param name="differences">A list of differences</param>
		/// <param name="stream">An output stream</param>
		public void OutputStream(List<Difference> differences, Stream stream)
		{
			TextWriter textWriter = new StreamWriter(stream);
			WriteItOut(differences, textWriter);
			textWriter.Flush();
			if (stream.CanSeek)
			{
				stream.Seek(0L, SeekOrigin.Begin);
			}
		}

		/// <summary>
		/// Output the differences to a string
		/// </summary>
		/// <param name="differences">A list of differences</param>
		/// <returns>A string</returns>
		public string OutputString(List<Difference> differences)
		{
			StringBuilder stringBuilder = new StringBuilder(differences.Count * 40);
			TextWriter writer = new StringWriter(stringBuilder);
			WriteItOut(differences, writer);
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Escape special characters
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private string EscapeString(object value)
		{
			if (value == null)
			{
				return string.Empty;
			}
			return WebHelper.HtmlEncode(value.ToString());
		}
	}
	/// <summary>
	/// Define a dual file report like Beyond Compare, WinMerge etc.
	/// </summary>
	public interface IDualFileReport
	{
		/// <summary>
		/// Output the differences to two files
		/// </summary>
		/// <param name="differences">A list of differences</param>
		/// <param name="expectedFilePath">The path to write the expected results</param>
		/// <param name="actualFilePath">The path to write the actual results</param>
		void OutputFiles(List<Difference> differences, string expectedFilePath, string actualFilePath);

		/// <summary>
		/// Launch the comparison application
		/// </summary>
		/// <param name="expectedFilePath">The path to write the expected results</param>
		/// <param name="actualFilePath">The path to write the actual results</param>
		void LaunchApplication(string expectedFilePath, string actualFilePath);
	}
	/// <summary>
	/// Defines a Single File Report
	/// </summary>
	public interface ISingleFileReport
	{
		/// <summary>
		/// Output the differences to a file
		/// </summary>
		/// <param name="differences">A list of differences</param>
		/// <param name="filePath">The file path</param>
		void OutputFile(List<Difference> differences, string filePath);

		/// <summary>
		/// Output the differences to a stream
		/// </summary>
		/// <param name="differences">A list of differences</param>
		/// <param name="stream">Where to write to</param>
		void OutputStream(List<Difference> differences, Stream stream);

		/// <summary>
		/// Output the differences to a string
		/// </summary>
		/// <param name="differences">A list of differences</param>
		/// <returns>A string</returns>
		string OutputString(List<Difference> differences);

		/// <summary>
		/// Launch the application for showing the file
		/// </summary>
		/// <param name="filePath">The file path</param>
		void LaunchApplication(string filePath);
	}
	/// <summary>
	/// Report for showing differences to an end user
	/// </summary>
	public class UserFriendlyReport : ISingleFileReport
	{
		/// <summary>
		/// The text in between the values.  Defaults to: CHANGED TO -&gt;
		/// </summary>
		public string ChangedToText { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Snet.Utility.comparer.Reports.UserFriendlyReport" /> class.
		/// </summary>
		public UserFriendlyReport()
		{
			ChangedToText = "CHANGED TO ->";
		}

		/// <summary>
		/// Output the differences to a file
		/// </summary>
		/// <param name="differences">A list of differences</param>
		/// <param name="filePath">The file path</param>
		public void OutputFile(List<Difference> differences, string filePath)
		{
			if (string.IsNullOrEmpty(Path.GetDirectoryName(filePath)))
			{
				filePath = Path.Combine(FileHelper.GetCurrentDirectory(), filePath);
			}
			using FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
			using TextWriter writer = new StreamWriter(stream);
			WriteItOut(differences, writer);
		}

		private string FormatProperty(Difference difference)
		{
			string shortItem = difference.GetShortItem();
			StringBuilder stringBuilder = new StringBuilder(shortItem.Length);
			string[] array = shortItem.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string input in array)
			{
				stringBuilder.Append(StringHelper.InsertSpaces(input));
				stringBuilder.Append(" ");
			}
			return stringBuilder.ToString().Trim();
		}

		private void WriteItOut(List<Difference> differences, TextWriter writer)
		{
			foreach (Difference difference in differences)
			{
				writer.Write("{0}: ", FormatProperty(difference));
				writer.Write("{0} {1} ", difference.Object1Value, ChangedToText);
				writer.WriteLine("{0}", difference.Object2Value);
			}
		}

		/// <summary>
		/// Output the differences to a stream
		/// </summary>
		/// <param name="differences">A list of differences</param>
		/// <param name="stream">Where to write to</param>
		public void OutputStream(List<Difference> differences, Stream stream)
		{
			TextWriter textWriter = new StreamWriter(stream);
			WriteItOut(differences, textWriter);
			textWriter.Flush();
			if (stream.CanSeek)
			{
				stream.Seek(0L, SeekOrigin.Begin);
			}
		}

		/// <summary>
		/// Output the differences to a string
		/// </summary>
		/// <param name="differences">A list of differences</param>
		/// <returns>A string</returns>
		public string OutputString(List<Difference> differences)
		{
			StringBuilder stringBuilder = new StringBuilder(differences.Count * 40);
			TextWriter writer = new StringWriter(stringBuilder);
			WriteItOut(differences, writer);
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Launch the application for showing the file
		/// </summary>
		/// <param name="filePath">The file path</param>
		public void LaunchApplication(string filePath)
		{
			if (!EnvironmentHelper.IsWindows())
			{
				throw new NotSupportedException();
			}
			ProcessHelper.Shell(filePath, string.Empty, ProcessWindowStyle.Normal, waitUntilFinished: false);
		}
	}
	/// <summary>
	/// Output files and launch WinMerge
	/// </summary>
	public class WinMergeReport : BaseDualFileReport
	{
		private const string APPLICATION_NAME = "WinMergeU.exe";

		/// <summary>
		/// Launch the WinMerge
		/// </summary>
		/// <param name="expectedFilePath">The path to write the expected results</param>
		/// <param name="actualFilePath">The path to write the actual results</param>
		public override void LaunchApplication(string expectedFilePath, string actualFilePath)
		{
			if (!EnvironmentHelper.IsWindows())
			{
				throw new NotSupportedException();
			}
			if (string.IsNullOrEmpty(Path.GetDirectoryName(expectedFilePath)))
			{
				expectedFilePath = Path.Combine(FileHelper.GetCurrentDirectory(), expectedFilePath);
			}
			if (string.IsNullOrEmpty(Path.GetDirectoryName(actualFilePath)))
			{
				actualFilePath = Path.Combine(FileHelper.GetCurrentDirectory(), actualFilePath);
			}
			string arguments = $"\"{expectedFilePath}\" \"{actualFilePath}\"";
			string text = FindWinMerge();
			if (string.IsNullOrEmpty(text))
			{
				throw new FileNotFoundException("WinMergeU.exe");
			}
			ProcessHelper.Shell(text, arguments, ProcessWindowStyle.Normal, waitUntilFinished: false);
		}

		/// <summary>
		/// Find the path of the WinMerge executable
		/// </summary>
		/// <returns>The path or null if not found</returns>
		public string FindWinMerge()
		{
			return Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\WinMergeU.exe")?.GetValue("")?.ToString();
		}
	}
}
namespace Snet.Utility.comparer.IgnoreOrderTypes
{
	/// <summary>
	/// Logic for comparing lists that are out of order based on a key
	/// </summary>
	public class IgnoreOrderLogic : BaseComparer
	{
		private class InstanceCounter
		{
			public int Counter;

			public readonly object ObjectValue;

			public InstanceCounter(object value, int counter)
			{
				Counter = counter;
				ObjectValue = value;
			}
		}

		private readonly RootComparer _rootComparer;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Snet.Utility.comparer.IgnoreOrderTypes.IgnoreOrderLogic" /> class.
		/// </summary>
		/// <param name="rootComparer">The root comparer.</param>
		public IgnoreOrderLogic(RootComparer rootComparer)
		{
			_rootComparer = rootComparer;
		}

		/// <summary>
		/// Compares the enumerators and ignores the order
		/// </summary>
		public void CompareEnumeratorIgnoreOrder(CompareParms parms, bool countsDifferent)
		{
			CompareOutOfOrder(parms, reverseCompare: false);
		}

		private void CompareOutOfOrder(CompareParms parms, bool reverseCompare)
		{
			bool flag = false;
			int num = 0;
			int num2 = 0;
			Dictionary<string, InstanceCounter> dictionary = new Dictionary<string, InstanceCounter>();
			Dictionary<string, InstanceCounter> dictionary2 = new Dictionary<string, InstanceCounter>();
			Dictionary<Type, List<string>> dictionary3 = new Dictionary<Type, List<string>>();
			Dictionary<Type, List<string>> dictionary4 = new Dictionary<Type, List<string>>();
			Type type = ((!(parms.Object1Type != null)) ? null : (TypeHelper.IsGenericType(parms.Object1Type) ? parms.Object1Type.GetGenericArguments()[0] : parms.Object1Type.GetElementType()));
			Type type2 = ((!(parms.Object2Type != null)) ? null : (TypeHelper.IsGenericType(parms.Object2Type) ? parms.Object2Type.GetGenericArguments()[0] : parms.Object2Type.GetElementType()));
			IEnumerator enumerator;
			IEnumerator enumerator2;
			if (!reverseCompare)
			{
				enumerator = ((IEnumerable)parms.Object1).GetEnumerator();
				enumerator2 = ((IEnumerable)parms.Object2).GetEnumerator();
			}
			else
			{
				enumerator = ((IEnumerable)parms.Object2).GetEnumerator();
				enumerator2 = ((IEnumerable)parms.Object1).GetEnumerator();
			}
			while (enumerator.MoveNext())
			{
				object current = enumerator.Current;
				if (current == null || !parms.Config.ClassTypesToIgnore.Contains(current.GetType()))
				{
					Type type3 = current?.GetType() ?? type;
					if (!dictionary3.ContainsKey(type3))
					{
						dictionary3.Add(type3, GetMatchingSpec(parms.Result, type3));
					}
					string matchIndex = GetMatchIndex(parms.Result, dictionary3[type3], current);
					if (!dictionary.ContainsKey(matchIndex))
					{
						dictionary.Add(matchIndex, new InstanceCounter(current, 1));
					}
					else
					{
						dictionary[matchIndex].Counter++;
					}
					num++;
				}
			}
			while (enumerator2.MoveNext())
			{
				object current2 = enumerator2.Current;
				if (current2 == null || !parms.Config.ClassTypesToIgnore.Contains(current2.GetType()))
				{
					Type type4 = current2?.GetType() ?? type2;
					if (!dictionary4.ContainsKey(type4))
					{
						dictionary4.Add(type4, GetMatchingSpec(parms.Result, type4));
					}
					string matchIndex2 = GetMatchIndex(parms.Result, dictionary4[type4], current2);
					if (!dictionary2.ContainsKey(matchIndex2))
					{
						dictionary2.Add(matchIndex2, new InstanceCounter(current2, 1));
					}
					else
					{
						dictionary2[matchIndex2].Counter++;
					}
					num2++;
				}
			}
			while (dictionary.Count > 0)
			{
				KeyValuePair<string, InstanceCounter> keyValuePair = dictionary.First();
				string text = parms.BreadCrumb + "[" + keyValuePair.Key + "]";
				bool num3 = keyValuePair.Value.ObjectValue == null && dictionary2.ContainsKey(keyValuePair.Key) && dictionary2[keyValuePair.Key].ObjectValue == null;
				object obj = (dictionary2.ContainsKey(keyValuePair.Key) ? dictionary2[keyValuePair.Key].ObjectValue : null);
				if (num3)
				{
					if (--dictionary2[keyValuePair.Key].Counter == 0)
					{
						dictionary2.Remove(keyValuePair.Key);
					}
				}
				else if (obj != null)
				{
					CompareParms parms2 = new CompareParms
					{
						Result = parms.Result,
						Config = parms.Config,
						ParentObject1 = parms.Object1,
						ParentObject2 = parms.Object2,
						Object1 = keyValuePair.Value.ObjectValue,
						Object2 = obj,
						BreadCrumb = text
					};
					_rootComparer.Compare(parms2);
					if (--dictionary2[keyValuePair.Key].Counter == 0)
					{
						dictionary2.Remove(keyValuePair.Key);
					}
				}
				else
				{
					Difference difference = new Difference
					{
						ParentObject1 = parms.ParentObject1,
						ParentObject2 = parms.ParentObject2,
						PropertyName = text,
						Object1Value = (reverseCompare ? "(null)" : NiceString(keyValuePair.Value.ObjectValue)),
						Object2Value = (reverseCompare ? NiceString(keyValuePair.Value.ObjectValue) : "(null)"),
						ChildPropertyName = "Item",
						Object1 = (reverseCompare ? null : keyValuePair.Value.ObjectValue),
						Object2 = (reverseCompare ? keyValuePair.Value.ObjectValue : null)
					};
					AddDifference(parms.Result, difference);
					flag = true;
				}
				if (parms.Result.ExceededDifferences)
				{
					return;
				}
				if (--dictionary[keyValuePair.Key].Counter == 0)
				{
					dictionary.Remove(keyValuePair.Key);
				}
			}
			while (dictionary2.Count > 0)
			{
				KeyValuePair<string, InstanceCounter> keyValuePair2 = dictionary2.First();
				Difference difference2 = new Difference
				{
					ParentObject1 = parms.ParentObject1,
					ParentObject2 = parms.ParentObject2,
					PropertyName = parms.BreadCrumb + "[" + keyValuePair2.Key + "]",
					Object1Value = (reverseCompare ? NiceString(keyValuePair2.Value.ObjectValue) : "(null)"),
					Object2Value = (reverseCompare ? "(null)" : NiceString(keyValuePair2.Value.ObjectValue)),
					ChildPropertyName = "Item",
					Object1 = (reverseCompare ? keyValuePair2.Value.ObjectValue : null),
					Object2 = (reverseCompare ? null : keyValuePair2.Value.ObjectValue)
				};
				AddDifference(parms.Result, difference2);
				flag = true;
				dictionary2.Remove(keyValuePair2.Key);
				if (parms.Result.ExceededDifferences)
				{
					return;
				}
			}
			if (!flag && num != num2)
			{
				Difference difference3 = new Difference
				{
					ParentObject1 = parms.ParentObject1,
					ParentObject2 = parms.ParentObject2,
					PropertyName = parms.BreadCrumb,
					Object1Value = num.ToString(CultureInfo.InvariantCulture),
					Object2Value = num2.ToString(CultureInfo.InvariantCulture),
					ChildPropertyName = "Count",
					Object1 = parms.Object1,
					Object2 = parms.Object2
				};
				AddDifference(parms.Result, difference3);
			}
		}

		private string GetMatchIndex(ComparisonResult result, List<string> spec, object currentObject)
		{
			if (currentObject == null)
			{
				return "(null)";
			}
			List<PropertyInfo> source = Cache.GetPropertyInfo(result.Config, currentObject.GetType()).ToList();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string item in spec)
			{
				PropertyInfo propertyInfo = source.FirstOrDefault((PropertyInfo o) => o.Name == item);
				if (propertyInfo == null)
				{
					throw new Exception($"Invalid CollectionMatchingSpec.  No such property {item} for type {currentObject.GetType().Name} ");
				}
				if (propertyInfo.GetIndexParameters().Length != 0)
				{
					throw new ArgumentException("Invalid object " + currentObject.GetType().Name + " to compare. Object with indexers cannot be compared when IgnoreCollectionOrder = true.", "currentObject");
				}
				object obj = propertyInfo.GetValue(currentObject, null);
				if (result.Config.TreatStringEmptyAndNullTheSame && propertyInfo.PropertyType == typeof(string) && obj == null)
				{
					obj = string.Empty;
				}
				else if (obj != null && !result.Config.CaseSensitive && propertyInfo.PropertyType == typeof(string))
				{
					obj = ((string)obj).ToLowerInvariant();
				}
				if (result.Config.ShowCollectionKeyName)
				{
					if (obj == null)
					{
						stringBuilder.AppendFormat("{0}:(null),", item);
						continue;
					}
					byte value = BitConverter.GetBytes(decimal.GetBits(result.Config.DecimalPrecision)[3])[2];
					string format = "{0}:{1" + (TypeHelper.IsDecimal(obj) ? $":N{value}" : string.Empty) + "},";
					stringBuilder.Append(string.Format(format, item, obj));
				}
				else if (obj == null)
				{
					stringBuilder.Append("(null),");
				}
				else
				{
					byte value2 = BitConverter.GetBytes(decimal.GetBits(result.Config.DecimalPrecision)[3])[2];
					string format2 = "{0" + (TypeHelper.IsDecimal(obj) ? $":N{value2}" : string.Empty) + "},";
					stringBuilder.Append(string.Format(format2, obj));
				}
			}
			if (stringBuilder.Length == 0)
			{
				stringBuilder.Append(RespectNumberToString(currentObject));
			}
			return stringBuilder.ToString().TrimEnd(',');
		}

		private static string RespectNumberToString(object o)
		{
			return Type.GetTypeCode(o.GetType()) switch
			{
				TypeCode.Decimal => ((decimal)o).ToString("G29"), 
				TypeCode.Double => ((double)o).ToString("G"), 
				TypeCode.Single => ((float)o).ToString("G"), 
				_ => o.ToString(), 
			};
		}

		private List<string> GetMatchingSpec(ComparisonResult result, Type type)
		{
			if (type == null)
			{
				return new List<string> { "(null)" };
			}
			Type matchingBasePresent = result.Config.CollectionMatchingSpec.Keys.FirstOrDefault((Type k) => k.IsAssignableFrom(type));
			if (matchingBasePresent != null)
			{
				return result.Config.CollectionMatchingSpec.First<KeyValuePair<Type, IEnumerable<string>>>((KeyValuePair<Type, IEnumerable<string>> p) => p.Key == matchingBasePresent).Value.ToList();
			}
			return (from o in Cache.GetPropertyInfo(result.Config, type)
				where o.CanWrite && !ExcludeLogic.ShouldExcludeMember(result.Config, o, o.DeclaringType) && (TypeHelper.IsSimpleType(o.PropertyType) || TypeHelper.IsEnum(o.PropertyType))
				select o.Name).ToList();
		}
	}
	internal class IndexerCollectionLooper : IEnumerable
	{
		private readonly object _indexer;

		private readonly PropertyInfo _info;

		private readonly int _cnt;

		public IndexerCollectionLooper(object obj, PropertyInfo info, int cnt)
		{
			_indexer = obj;
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			_info = info;
			_cnt = cnt;
		}

		public IEnumerator GetEnumerator()
		{
			for (int i = 0; i < _cnt; i++)
			{
				yield return _info.GetValue(_indexer, new object[1] { i });
			}
		}
	}
}
