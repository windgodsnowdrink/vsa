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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;

namespace HslCommunication.Profinet.AllenBradley;

/// <summary>
/// AB PLC的辅助类，用来辅助生成基本的指令信息
/// </summary>
public class AllenBradleyHelper
{
	/// <summary>
	/// CIP命令中PCCC命令相关的信息
	/// </summary>
	public const byte CIP_Execute_PCCC = 75;

	/// <summary>
	/// CIP命令中的读取数据的服务
	/// </summary>
	public const byte CIP_READ_DATA = 76;

	/// <summary>
	/// CIP命令中的写数据的服务
	/// </summary>
	public const int CIP_WRITE_DATA = 77;

	/// <summary>
	/// CIP命令中的读并写的数据服务
	/// </summary>
	public const int CIP_READ_WRITE_DATA = 78;

	/// <summary>
	/// CIP命令中的读片段的数据服务
	/// </summary>
	public const int CIP_READ_FRAGMENT = 82;

	/// <summary>
	/// CIP命令中的写片段的数据服务
	/// </summary>
	public const int CIP_WRITE_FRAGMENT = 83;

	/// <summary>
	/// CIP指令中读取数据的列表
	/// </summary>
	public const byte CIP_READ_LIST = 85;

	/// <summary>
	/// CIP命令中的对数据读取服务
	/// </summary>
	public const int CIP_MULTIREAD_DATA = 4096;

	/// <summary>
	/// 日期的格式
	/// </summary>
	public const ushort CIP_Type_DATE = 8;

	/// <summary>
	/// 时间的格式
	/// </summary>
	public const ushort CIP_Type_TIME = 9;

	/// <summary>
	/// 日期时间格式，最完整的时间格式
	/// </summary>
	public const ushort CIP_Type_TimeAndDate = 10;

	/// <summary>
	/// 一天中的时间格式
	/// </summary>
	public const ushort CIP_Type_TimeOfDate = 11;

	/// <summary>
	/// bool型数据，一个字节长度
	/// </summary>
	public const ushort CIP_Type_Bool = 193;

	/// <summary>
	/// byte型数据，一个字节长度，SINT
	/// </summary>
	public const ushort CIP_Type_Byte = 194;

	/// <summary>
	/// 整型，两个字节长度，INT
	/// </summary>
	public const ushort CIP_Type_Word = 195;

	/// <summary>
	/// 长整型，四个字节长度，DINT
	/// </summary>
	public const ushort CIP_Type_DWord = 196;

	/// <summary>
	/// 特长整型，8个字节，LINT
	/// </summary>
	public const ushort CIP_Type_LInt = 197;

	/// <summary>
	/// Unsigned 8-bit integer, USINT
	/// </summary>
	public const ushort CIP_Type_USInt = 198;

	/// <summary>
	/// Unsigned 16-bit integer, UINT
	/// </summary>
	public const ushort CIP_Type_UInt = 199;

	/// <summary>
	/// Unsigned 32-bit integer, UDINT
	/// </summary>
	public const ushort CIP_Type_UDint = 200;

	/// <summary>
	/// Unsigned 64-bit integer, ULINT
	/// </summary>
	public const ushort CIP_Type_ULint = 201;

	/// <summary>
	/// 实数数据，四个字节长度
	/// </summary>
	public const ushort CIP_Type_Real = 202;

	/// <summary>
	/// 实数数据，八个字节的长度
	/// </summary>
	public const ushort CIP_Type_Double = 203;

	/// <summary>
	/// 结构体数据，不定长度
	/// </summary>
	public const ushort CIP_Type_Struct = 204;

	/// <summary>
	/// 字符串数据内容
	/// </summary>
	public const ushort CIP_Type_String = 208;

	/// <summary>
	/// Bit string, 8 bits, BYTE,
	/// </summary>
	public const ushort CIP_Type_D1 = 209;

	/// <summary>
	/// Bit string, 16-bits, WORD
	/// </summary>
	public const ushort CIP_Type_D2 = 210;

	/// <summary>
	/// Bit string, 32 bits, DWORD
	/// </summary>
	public const ushort CIP_Type_D3 = 211;

	/// <summary>
	/// Bit string, 64 bits LWORD
	/// </summary>
	public const ushort CIP_Type_D4 = 212;

	/// <summary>
	/// 二进制数据内容
	/// </summary>
	public const ushort CIP_Type_BitArray = 211;

	/// <summary>
	/// 连接方的厂商标识
	/// </summary>
	public const ushort OriginatorVendorID = 4105;

	/// <summary>
	/// 连接方的序列号
	/// </summary>
	public const uint OriginatorSerialNumber = 3248834059u;

	private static byte[] BuildRequestPathCommand(string address, bool isConnectedAddress = false)
	{
		using MemoryStream memoryStream = new MemoryStream();
		int num = HslHelper.ExtractParameter(ref address, "class", -1);
		if (num != -1)
		{
			if (num < 256)
			{
				memoryStream.WriteByte(32);
				memoryStream.WriteByte((byte)num);
			}
			else
			{
				memoryStream.WriteByte(33);
				memoryStream.WriteByte(0);
				memoryStream.WriteByte(BitConverter.GetBytes(num)[0]);
				memoryStream.WriteByte(BitConverter.GetBytes(num)[1]);
			}
			int num2 = HslHelper.ExtractParameter(ref address, "attr", -1);
			int num3 = (address.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt32(address.Substring(2), 16) : Convert.ToInt32(address));
			if (num3 < 256)
			{
				memoryStream.WriteByte(36);
				memoryStream.WriteByte((byte)num3);
			}
			else
			{
				memoryStream.WriteByte(37);
				memoryStream.WriteByte(0);
				memoryStream.WriteByte(BitConverter.GetBytes(num3)[0]);
				memoryStream.WriteByte(BitConverter.GetBytes(num3)[1]);
			}
			if (num2 != -1)
			{
				if (num2 < 256)
				{
					memoryStream.WriteByte(48);
					memoryStream.WriteByte((byte)num2);
				}
				else
				{
					memoryStream.WriteByte(49);
					memoryStream.WriteByte(0);
					memoryStream.WriteByte(BitConverter.GetBytes(num2)[0]);
					memoryStream.WriteByte(BitConverter.GetBytes(num2)[1]);
				}
			}
		}
		else
		{
			string[] array = address.SplitDot();
			for (int i = 0; i < array.Length; i++)
			{
				string text = string.Empty;
				int num4 = array[i].IndexOf('[');
				int num5 = array[i].LastIndexOf(']');
				if (num4 > 0 && num5 > 0 && num5 > num4)
				{
					text = array[i].Substring(num4 + 1, num5 - num4 - 1);
					array[i] = array[i].Substring(0, num4);
				}
				memoryStream.WriteByte(145);
				byte[] bytes = Encoding.UTF8.GetBytes(array[i]);
				memoryStream.WriteByte((byte)bytes.Length);
				memoryStream.Write(bytes, 0, bytes.Length);
				if (bytes.Length % 2 == 1)
				{
					memoryStream.WriteByte(0);
				}
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				MatchCollection matchCollection = Regex.Matches(text, "[0-9]+");
				for (int j = 0; j < matchCollection.Count; j++)
				{
					int num6 = Convert.ToInt32(matchCollection[j].Value);
					if (num6 < 256 && !isConnectedAddress)
					{
						memoryStream.WriteByte(40);
						memoryStream.WriteByte((byte)num6);
					}
					else if (num6 < 65536)
					{
						memoryStream.WriteByte(41);
						memoryStream.WriteByte(0);
						memoryStream.WriteByte(BitConverter.GetBytes(num6)[0]);
						memoryStream.WriteByte(BitConverter.GetBytes(num6)[1]);
					}
					else
					{
						memoryStream.WriteByte(42);
						memoryStream.WriteByte(0);
						memoryStream.Write(BitConverter.GetBytes(num6));
					}
				}
			}
		}
		return memoryStream.ToArray();
	}

	/// <summary>
	/// 从生成的报文里面反解出实际的数据地址，不支持结构体嵌套，仅支持数据，一维数组，不支持多维数据
	/// </summary>
	/// <param name="pathCommand">地址路径报文</param>
	/// <returns>实际的地址</returns>
	public static string ParseRequestPathCommand(byte[] pathCommand)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		while (num < pathCommand.Length && pathCommand[num] == 145)
		{
			string value = Encoding.UTF8.GetString(pathCommand, num + 2, pathCommand[num + 1]).TrimEnd('\0');
			stringBuilder.Append(value);
			int num2 = 2 + pathCommand[num + 1];
			if (pathCommand[num + 1] % 2 == 1)
			{
				num2++;
			}
			num += num2;
			while (num < pathCommand.Length)
			{
				if (pathCommand[num] == 40)
				{
					stringBuilder.Append($"[{pathCommand[num + 1]}]");
					num += 2;
					continue;
				}
				if (pathCommand[num] == 41)
				{
					stringBuilder.Append($"[{BitConverter.ToUInt16(pathCommand, num + 2)}]");
					num += 4;
					continue;
				}
				if (pathCommand[num] == 42)
				{
					stringBuilder.Append($"[{BitConverter.ToInt32(pathCommand, num + 2)}]");
					num += 6;
					continue;
				}
				break;
			}
			stringBuilder.Append(".");
		}
		if (stringBuilder[stringBuilder.Length - 1] == '.')
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		return stringBuilder.ToString();
	}

	/// <summary>
	/// 从生成的报文里面，反解出实际的 Symbol Class ID，以及  Instance ID 信息
	/// </summary>
	public static OperateResult<int, int, int> ParseRequestPathSymbolInstanceAddressing(byte[] pathCommand)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int value = -1;
		if (pathCommand[num3] == 32)
		{
			num3++;
			num = pathCommand[num3++];
		}
		else
		{
			if (pathCommand[num3] != 33)
			{
				return new OperateResult<int, int, int>();
			}
			num3 += 2;
			num = BitConverter.ToUInt16(pathCommand, num3);
			num3 += 2;
		}
		if (pathCommand[num3] == 36)
		{
			num3++;
			num2 = pathCommand[num3++];
		}
		else
		{
			if (pathCommand[num3] != 37)
			{
				return new OperateResult<int, int, int>();
			}
			num3 += 2;
			num2 = BitConverter.ToUInt16(pathCommand, num3);
			num3 += 2;
		}
		if (pathCommand.Length > num3)
		{
			if (pathCommand[num3] == 48)
			{
				num3++;
				value = pathCommand[num3++];
			}
			else if (pathCommand[num3] == 49)
			{
				num3 += 2;
				value = BitConverter.ToUInt16(pathCommand, num3);
				num3 += 2;
			}
		}
		return OperateResult.CreateSuccessResult(num, num2, value);
	}

	/// <summary>
	/// 获取枚举PLC数据信息的指令
	/// </summary>
	/// <param name="startInstance">实例的起始地址</param>
	/// <returns>结果数据</returns>
	public static byte[] BuildEnumeratorCommand(uint startInstance)
	{
		return new byte[16]
		{
			85,
			3,
			32,
			107,
			37,
			0,
			BitConverter.GetBytes(startInstance)[0],
			BitConverter.GetBytes(startInstance)[1],
			3,
			0,
			1,
			0,
			2,
			0,
			8,
			0
		};
	}

	/// <summary>
	/// 获取枚举PLC的局部变量的数据信息的指令
	/// </summary>
	/// <param name="startInstance">实例的起始地址</param>
	/// <returns>结果命令数据</returns>
	public static byte[] BuildEnumeratorProgrameMainCommand(uint startInstance)
	{
		byte[] buffer = new byte[38];
		buffer[0] = 85;
		buffer[1] = 14;
		buffer[2] = 145;
		buffer[3] = 19;
		Encoding.ASCII.GetBytes("Program:MainProgram").CopyTo(buffer, 4);
		buffer[23] = 0;
		buffer[24] = 32;
		buffer[25] = 107;
		buffer[26] = 37;
		buffer[27] = 0;
		buffer[28] = BitConverter.GetBytes(startInstance)[0];
		buffer[29] = BitConverter.GetBytes(startInstance)[1];
		buffer[30] = 3;
		buffer[31] = 0;
		buffer[32] = 1;
		buffer[33] = 0;
		buffer[34] = 2;
		buffer[35] = 0;
		buffer[36] = 8;
		buffer[37] = 0;
		return buffer;
	}

	/// <summary>
	/// 获取获得结构体句柄的命令
	/// </summary>
	/// <param name="symbolType">包含地址的信息</param>
	/// <returns>命令数据</returns>
	public static byte[] GetStructHandleCommand(ushort symbolType)
	{
		byte[] buffer = new byte[18];
		symbolType &= 0xFFF;
		buffer[0] = 3;
		buffer[1] = 3;
		buffer[2] = 32;
		buffer[3] = 108;
		buffer[4] = 37;
		buffer[5] = 0;
		buffer[6] = BitConverter.GetBytes(symbolType)[0];
		buffer[7] = BitConverter.GetBytes(symbolType)[1];
		buffer[8] = 4;
		buffer[9] = 0;
		buffer[10] = 4;
		buffer[11] = 0;
		buffer[12] = 5;
		buffer[13] = 0;
		buffer[14] = 2;
		buffer[15] = 0;
		buffer[16] = 1;
		buffer[17] = 0;
		return buffer;
	}

	/// <summary>
	/// 获取结构体内部数据结构的方法
	/// </summary>
	/// <param name="symbolType">地址</param>
	/// <param name="structHandle">句柄</param>
	/// <param name="offset">偏移量地址</param>
	/// <returns>指令</returns>
	public static byte[] GetStructItemNameType(ushort symbolType, AbStructHandle structHandle, int offset)
	{
		byte[] buffer = new byte[14];
		symbolType &= 0xFFF;
		byte[] bytes = BitConverter.GetBytes(structHandle.TemplateObjectDefinitionSize * 4 - 21);
		buffer[0] = 76;
		buffer[1] = 3;
		buffer[2] = 32;
		buffer[3] = 108;
		buffer[4] = 37;
		buffer[5] = 0;
		buffer[6] = BitConverter.GetBytes(symbolType)[0];
		buffer[7] = BitConverter.GetBytes(symbolType)[1];
		buffer[8] = BitConverter.GetBytes(offset)[0];
		buffer[9] = BitConverter.GetBytes(offset)[1];
		buffer[10] = BitConverter.GetBytes(offset)[2];
		buffer[11] = BitConverter.GetBytes(offset)[3];
		buffer[12] = bytes[0];
		buffer[13] = bytes[1];
		return buffer;
	}

	/// <summary>
	/// 将CommandSpecificData的命令，打包成可发送的数据指令
	/// </summary>
	/// <param name="command">实际的命令暗号</param>
	/// <param name="error">错误号信息</param>
	/// <param name="session">当前会话的id</param>
	/// <param name="commandSpecificData">CommandSpecificData命令</param>
	/// <param name="senderContext">发送的上下文内容</param>
	/// <returns>最终可发送的数据命令</returns>
	public static byte[] PackRequestHeader(ushort command, uint session, byte[] commandSpecificData, byte[] senderContext = null)
	{
		if (commandSpecificData == null)
		{
			commandSpecificData = new byte[0];
		}
		byte[] buffer = new byte[commandSpecificData.Length + 24];
		Array.Copy(commandSpecificData, 0, buffer, 24, commandSpecificData.Length);
		BitConverter.GetBytes(command).CopyTo(buffer, 0);
		BitConverter.GetBytes(session).CopyTo(buffer, 4);
		if (senderContext != null && senderContext.Length <= 8)
		{
			senderContext.CopyTo(buffer, 12);
		}
		BitConverter.GetBytes((ushort)commandSpecificData.Length).CopyTo(buffer, 2);
		return buffer;
	}

	/// <summary>
	/// 将CommandSpecificData的命令，打包成可发送的数据指令
	/// </summary>
	/// <param name="command">实际的命令暗号</param>
	/// <param name="error">错误号信息</param>
	/// <param name="session">当前会话的id</param>
	/// <param name="commandSpecificData">CommandSpecificData命令</param>
	/// <param name="senderContext">发送的上下文内容</param>
	/// <returns>最终可发送的数据命令</returns>
	public static byte[] PackRequestHeader(ushort command, uint error, uint session, byte[] commandSpecificData, byte[] senderContext = null)
	{
		byte[] array = PackRequestHeader(command, session, commandSpecificData, senderContext);
		BitConverter.GetBytes(error).CopyTo(array, 8);
		return array;
	}

	private static byte[] PackExecutePCCC(byte[] pccc)
	{
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.WriteByte(75);
		memoryStream.WriteByte(2);
		memoryStream.WriteByte(32);
		memoryStream.WriteByte(103);
		memoryStream.WriteByte(36);
		memoryStream.WriteByte(1);
		memoryStream.WriteByte(7);
		memoryStream.WriteByte(9);
		memoryStream.WriteByte(16);
		memoryStream.WriteByte(11);
		memoryStream.WriteByte(70);
		memoryStream.WriteByte(165);
		memoryStream.WriteByte(193);
		memoryStream.Write(pccc);
		byte[] array = memoryStream.ToArray();
		BitConverter.GetBytes((ushort)4105).CopyTo(array, 7);
		BitConverter.GetBytes(3248834059u).CopyTo(array, 9);
		return array;
	}

	/// <summary>
	/// 打包一个PCCC的读取的命令报文
	/// </summary>
	/// <param name="tns">请求序号信息</param>
	/// <param name="address">请求的文件地址，地址示例：N7:1</param>
	/// <param name="length">请求的字节长度</param>
	/// <returns>PCCC的读取报文信息</returns>
	public static OperateResult<byte[]> PackExecutePCCCRead(int tns, string address, int length)
	{
		OperateResult<byte[]> operateResult = AllenBradleyDF1Serial.BuildProtectedTypedLogicalReadWithThreeAddressFields(tns, address, length);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return OperateResult.CreateSuccessResult(PackExecutePCCC(operateResult.Content));
	}

	/// <summary>
	/// 打包一个PCCC的写入的命令报文
	/// </summary>
	/// <param name="tns">请求序号信息</param>
	/// <param name="address">请求的文件地址，地址示例：N7:1</param>
	/// <param name="value">写入的原始数据信息</param>
	/// <returns>PCCC的写入报文信息</returns>
	public static OperateResult<byte[]> PackExecutePCCCWrite(int tns, string address, byte[] value)
	{
		OperateResult<byte[]> operateResult = AllenBradleyDF1Serial.BuildProtectedTypedLogicalWriteWithThreeAddressFields(tns, address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return OperateResult.CreateSuccessResult(PackExecutePCCC(operateResult.Content));
	}

	/// <summary>
	/// 打包一个PCCC的写入的命令报文
	/// </summary>
	/// <param name="tns">请求序号信息</param>
	/// <param name="address">请求的文件地址，地址示例：N7:1</param>
	/// <param name="value">写入的原始数据信息</param>
	/// <returns>PCCC的写入报文信息</returns>
	internal static OperateResult<byte[]> PackExecutePCCCWrite(int tns, string address, int bitIndex, bool value)
	{
		OperateResult<byte[]> operateResult = AllenBradleyDF1Serial.BuildProtectedTypedLogicalMaskWithThreeAddressFields(tns, address, bitIndex, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return OperateResult.CreateSuccessResult(PackExecutePCCC(operateResult.Content));
	}

	/// <summary>
	/// 打包生成一个请求读取数据的节点信息，CIP指令信息
	/// </summary>
	/// <param name="address">地址</param>
	/// <param name="length">指代数组的长度</param>
	/// <param name="isConnectedAddress">是否是连接模式下的地址，默认为false</param>
	/// <returns>CIP的指令信息</returns>
	public static byte[] PackRequsetRead(string address, int length, bool isConnectedAddress = false)
	{
		byte b = (byte)(IsAddressClassAndAttribute(address) ? 14 : 76);
		byte[] buffer = new byte[1024];
		int num = 0;
		buffer[num++] = b;
		num++;
		byte[] array2 = BuildRequestPathCommand(address, isConnectedAddress);
		array2.CopyTo(buffer, num);
		num += array2.Length;
		buffer[1] = (byte)((num - 2) / 2);
		buffer[num++] = BitConverter.GetBytes(length)[0];
		buffer[num++] = BitConverter.GetBytes(length)[1];
		byte[] buffer2 = new byte[num];
		Array.Copy(buffer, 0, buffer2, 0, num);
		return buffer2;
	}

	/// <summary>
	/// 打包生成一个请求读取数据片段的节点信息，CIP指令信息
	/// </summary>
	/// <param name="address">节点的名称 -&gt; Tag Name</param>
	/// <param name="startIndex">起始的索引位置，以字节为单位 -&gt; The initial index position, in bytes</param>
	/// <param name="length">读取的数据长度，一次通讯总计490个字节 -&gt; Length of read data, a total of 490 bytes of communication</param>
	/// <returns>CIP的指令信息</returns>
	public static byte[] PackRequestReadSegment(string address, int startIndex, int length)
	{
		byte[] buffer = new byte[1024];
		int num = 0;
		buffer[num++] = 82;
		num++;
		byte[] array2 = BuildRequestPathCommand(address);
		array2.CopyTo(buffer, num);
		num += array2.Length;
		buffer[1] = (byte)((num - 2) / 2);
		buffer[num++] = BitConverter.GetBytes(length)[0];
		buffer[num++] = BitConverter.GetBytes(length)[1];
		buffer[num++] = BitConverter.GetBytes(startIndex)[0];
		buffer[num++] = BitConverter.GetBytes(startIndex)[1];
		buffer[num++] = BitConverter.GetBytes(startIndex)[2];
		buffer[num++] = BitConverter.GetBytes(startIndex)[3];
		byte[] buffer2 = new byte[num];
		Array.Copy(buffer, 0, buffer2, 0, num);
		return buffer2;
	}

	/// <summary>
	/// 判断地址是否是读取特性的地址
	/// </summary>
	public static bool IsAddressClassAndAttribute(string address)
	{
		if (string.IsNullOrEmpty(address))
		{
			return false;
		}
		return address.IndexOf("class=", StringComparison.OrdinalIgnoreCase) >= 0 && address.IndexOf("attr=", StringComparison.OrdinalIgnoreCase) >= 0;
	}

	/// <summary>
	/// 根据指定的数据和类型，生成对应的数据
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <param name="typeCode">数据类型</param>
	/// <param name="value">字节值</param>
	/// <param name="length">如果节点为数组，就是数组长度</param>
	/// <param name="isConnectedAddress">是否为连接模式的地址</param>
	/// <param name="paddingTail">是否需要在尾巴上补上一个尾巴字节，达到对齐的效果</param>
	/// <returns>CIP的指令信息</returns>
	public static byte[] PackRequestWrite(string address, ushort typeCode, byte[] value, int length = 1, bool isConnectedAddress = false, bool paddingTail = false)
	{
		byte[] buffer = new byte[1024];
		int num = 0;
		if (IsAddressClassAndAttribute(address))
		{
			buffer[num++] = 16;
			num++;
			byte[] array2 = BuildRequestPathCommand(address, isConnectedAddress);
			array2.CopyTo(buffer, num);
			num += array2.Length;
			buffer[1] = (byte)((num - 2) / 2);
			byte[] buffer2 = new byte[value.Length + num];
			Array.Copy(buffer, 0, buffer2, 0, num);
			value.CopyTo(buffer2, num);
			return buffer2;
		}
		buffer[num++] = 77;
		num++;
		byte[] array4 = BuildRequestPathCommand(address, isConnectedAddress);
		array4.CopyTo(buffer, num);
		num += array4.Length;
		buffer[1] = (byte)((num - 2) / 2);
		buffer[num++] = BitConverter.GetBytes(typeCode)[0];
		buffer[num++] = BitConverter.GetBytes(typeCode)[1];
		if (typeCode == 672 && value.Length >= 2)
		{
			buffer[num++] = value[0];
			buffer[num++] = value[1];
		}
		buffer[num++] = BitConverter.GetBytes(length)[0];
		buffer[num++] = BitConverter.GetBytes(length)[1];
		if (value == null)
		{
			value = new byte[0];
		}
		int num2 = 0;
		if (paddingTail && typeCode == 193 && length == 1 && value.Length % 2 > 0)
		{
			num2 = 1;
		}
		if (typeCode == 672 && value.Length >= 2)
		{
			byte[] buffer3 = new byte[value.Length + num + num2 - 2];
			Array.Copy(buffer, 0, buffer3, 0, num);
			Array.Copy(value, 2, buffer3, num, value.Length - 2);
			return buffer3;
		}
		byte[] buffer4 = new byte[value.Length + num + num2];
		Array.Copy(buffer, 0, buffer4, 0, num);
		value.CopyTo(buffer4, num);
		return buffer4;
	}

	/// <summary>
	/// 根据指定的数据和类型，生成对应的数据
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <param name="typeCode">数据类型</param>
	/// <param name="value">字节值</param>
	/// <param name="length">如果节点为数组，就是数组长度</param>
	/// <param name="isConnectedAddress">是否为连接模式的地址</param>
	/// <param name="paddingTail">是否需要在尾巴上补上一个尾巴字节，达到对齐的效果</param>
	/// <returns>CIP的指令信息</returns>
	public static byte[] PackRequestWriteSegment(string address, ushort typeCode, byte[] value, int startIndex, int length = 1, bool isConnectedAddress = false)
	{
		byte[] buffer = new byte[1024];
		int num = 0;
		buffer[num++] = 83;
		num++;
		byte[] array2 = BuildRequestPathCommand(address, isConnectedAddress);
		array2.CopyTo(buffer, num);
		num += array2.Length;
		buffer[1] = (byte)((num - 2) / 2);
		if (typeCode >= 32768)
		{
			buffer[num++] = 160;
			buffer[num++] = 2;
		}
		buffer[num++] = BitConverter.GetBytes(typeCode)[0];
		buffer[num++] = BitConverter.GetBytes(typeCode)[1];
		if (typeCode == 672 && value.Length >= 2)
		{
			buffer[num++] = value[0];
			buffer[num++] = value[1];
		}
		buffer[num++] = BitConverter.GetBytes(length)[0];
		buffer[num++] = BitConverter.GetBytes(length)[1];
		buffer[num++] = BitConverter.GetBytes(startIndex)[0];
		buffer[num++] = BitConverter.GetBytes(startIndex)[1];
		buffer[num++] = BitConverter.GetBytes(startIndex)[2];
		buffer[num++] = BitConverter.GetBytes(startIndex)[3];
		if (value == null)
		{
			value = new byte[0];
		}
		if (typeCode == 672 && value.Length >= 2)
		{
			byte[] buffer2 = new byte[value.Length + num - 2];
			Array.Copy(buffer, 0, buffer2, 0, num);
			Array.Copy(value, 2, buffer2, num, value.Length - 2);
			return buffer2;
		}
		byte[] buffer3 = new byte[value.Length + num];
		Array.Copy(buffer, 0, buffer3, 0, num);
		value.CopyTo(buffer3, num);
		return buffer3;
	}

	/// <summary>
	/// 根据传入的地址，或掩码，和掩码来创建一个读-修改-写的请求报文信息
	/// </summary>
	/// <param name="address">标签的地址信息</param>
	/// <param name="orMask">或的掩码</param>
	/// <param name="andMask">和的掩码</param>
	/// <param name="isConnectedAddress">是否为连接模式的地址</param>
	/// <returns>打包之后的CIP指令信息</returns>
	private static byte[] PackRequestReadModifyWrite(string address, byte[] orMask, byte[] andMask, bool isConnectedAddress = false)
	{
		byte[] buffer = new byte[1024];
		int num = 0;
		buffer[num++] = 78;
		num++;
		byte[] array2 = BuildRequestPathCommand(address, isConnectedAddress);
		array2.CopyTo(buffer, num);
		num += array2.Length;
		buffer[1] = (byte)((num - 2) / 2);
		buffer[num++] = (byte)orMask.Length;
		buffer[num++] = 0;
		orMask.CopyTo(buffer, num);
		num += orMask.Length;
		andMask.CopyTo(buffer, num);
		num += andMask.Length;
		return buffer.SelectBegin(num);
	}

	/// <summary>
	/// 根据传入的地址，或掩码，和掩码来创建一个读-修改-写的请求报文信息
	/// </summary>
	/// <param name="address">标签的地址信息</param>
	/// <param name="orMask">或的掩码</param>
	/// <param name="andMask">和的掩码</param>
	/// <param name="isConnectedAddress">是否为连接模式的地址</param>
	/// <returns>打包之后的CIP指令信息</returns>
	public static byte[] PackRequestReadModifyWrite(string address, uint orMask, uint andMask, bool isConnectedAddress = false)
	{
		return PackRequestReadModifyWrite(address, BitConverter.GetBytes(orMask), BitConverter.GetBytes(andMask), isConnectedAddress);
	}

	/// <summary>
	/// 根据传入的地址，或掩码，和掩码来创建一个读-修改-写的请求报文信息
	/// </summary>
	/// <param name="address">标签的地址信息</param>
	/// <param name="orMask">或的掩码</param>
	/// <param name="andMask">和的掩码</param>
	/// <param name="isConnectedAddress">是否为连接模式的地址</param>
	/// <returns>打包之后的CIP指令信息</returns>
	public static byte[] PackRequestReadModifyWrite(string address, int index, bool value, int dataBits, bool isConnectedAddress = false)
	{
		if (dataBits != 8 && dataBits != 16 && dataBits != 32 && dataBits != 64)
		{
			throw new ArgumentException("dataBits only support 8,16,32,64, but now: " + dataBits);
		}
		if (index / dataBits != 0)
		{
			address += $"[{index / dataBits}]";
		}
		index %= dataBits;
		if (value)
		{
			switch (dataBits)
			{
			case 64:
			{
				ulong num3 = 1uL;
				num3 <<= index;
				return PackRequestReadModifyWrite(address, BitConverter.GetBytes(num3), new byte[8] { 255, 255, 255, 255, 255, 255, 255, 255 }, isConnectedAddress);
			}
			case 32:
			{
				uint num2 = 1u;
				num2 <<= index;
				return PackRequestReadModifyWrite(address, BitConverter.GetBytes(num2), new byte[4] { 255, 255, 255, 255 }, isConnectedAddress);
			}
			case 16:
			{
				ushort num = 1;
				num = (ushort)(num << index);
				return PackRequestReadModifyWrite(address, BitConverter.GetBytes(num), new byte[2] { 255, 255 }, isConnectedAddress);
			}
			default:
			{
				byte b = 1;
				b = (byte)(b << index);
				return PackRequestReadModifyWrite(address, new byte[1] { b }, new byte[1] { 255 }, isConnectedAddress);
			}
			}
		}
		switch (dataBits)
		{
		case 64:
		{
			ulong num6 = 1uL;
			num6 <<= index;
			num6 = ~num6;
			return PackRequestReadModifyWrite(address, new byte[8], BitConverter.GetBytes(num6), isConnectedAddress);
		}
		case 32:
		{
			uint num5 = 1u;
			num5 <<= index;
			num5 = ~num5;
			return PackRequestReadModifyWrite(address, new byte[4], BitConverter.GetBytes(num5), isConnectedAddress);
		}
		case 16:
		{
			ushort num4 = 1;
			num4 = (ushort)(num4 << index);
			num4 = (ushort)(~num4);
			return PackRequestReadModifyWrite(address, new byte[2], BitConverter.GetBytes(num4), isConnectedAddress);
		}
		default:
		{
			byte b2 = 1;
			b2 = (byte)(b2 << index);
			b2 = (byte)(~b2);
			return PackRequestReadModifyWrite(address, new byte[1], new byte[1] { b2 }, isConnectedAddress);
		}
		}
	}

	/// <summary>
	/// 根据传入的地址，或掩码，和掩码来创建一个读-修改-写的请求报文信息
	/// </summary>
	/// <param name="address">标签的地址信息</param>
	/// <param name="orMask">或的掩码</param>
	/// <param name="andMask">和的掩码</param>
	/// <param name="isConnectedAddress">是否为连接模式的地址</param>
	/// <returns>打包之后的CIP指令信息</returns>
	public static byte[] PackRequestReadModifyWrite(string address, int index, bool value, bool isConnectedAddress = false)
	{
		if (index / 32 != 0)
		{
			address += $"[{index / 32}]";
		}
		index %= 32;
		if (value)
		{
			uint num = 1u;
			num <<= index;
			return PackRequestReadModifyWrite(address, num, uint.MaxValue, isConnectedAddress);
		}
		uint num2 = 1u;
		num2 <<= index;
		num2 = ~num2;
		return PackRequestReadModifyWrite(address, 0u, num2, isConnectedAddress);
	}

	/// <summary>
	/// 分析地址数据信息里的位索引的信息，例如a[10]  返回 a 和 10 索引，如果没有指定索引，就默认为0
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="arrayIndex">位索引</param>
	/// <returns>地址信息</returns>
	public static string AnalysisArrayIndex(string address, out int arrayIndex)
	{
		arrayIndex = 0;
		if (!address.EndsWith("]"))
		{
			Match match = Regex.Match(address, "\\.[0-9]+$");
			if (match.Success)
			{
				arrayIndex = int.Parse(match.Value.Substring(1));
				address = address.Substring(0, address.Length - match.Value.Length);
			}
			return address;
		}
		int num = address.LastIndexOf('[');
		if (num < 0)
		{
			return address;
		}
		address = address.Remove(address.Length - 1);
		try
		{
			arrayIndex = int.Parse(address.Substring(num + 1));
			address = address.Substring(0, num);
			return address;
		}
		catch
		{
			return address;
		}
	}

	/// <summary>
	/// 写入Bool数据的基本指令信息
	/// </summary>
	/// <param name="address">地址</param>
	/// <param name="value">值</param>
	/// <returns>报文信息</returns>
	public static byte[] PackRequestWrite(string address, bool value)
	{
		address = AnalysisArrayIndex(address, out var arrayIndex);
		return PackRequestReadModifyWrite(address, arrayIndex, value);
	}

	/// <summary>
	/// 将所有的cip指定进行打包操作。
	/// </summary>
	/// <param name="cip">所有的cip打包指令信息</param>
	/// <returns>包含服务的信息</returns>
	public static byte[] PackCommandService(byte[] cip)
	{
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.WriteByte(178);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		memoryStream.Write(cip, 0, cip.Length);
		byte[] array = memoryStream.ToArray();
		BitConverter.GetBytes((short)(array.Length - 4)).CopyTo(array, 2);
		return array;
	}

	/// <summary>
	/// 将所有的cip指定进行打包操作。
	/// </summary>
	/// <param name="portSlot">PLC所在的面板槽号</param>
	/// <param name="cips">所有的cip打包指令信息</param>
	/// <returns>包含服务的信息</returns>
	public static byte[] PackCommandService(byte[] portSlot, params byte[][] cips)
	{
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.WriteByte(178);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(82);
		memoryStream.WriteByte(2);
		memoryStream.WriteByte(32);
		memoryStream.WriteByte(6);
		memoryStream.WriteByte(36);
		memoryStream.WriteByte(1);
		memoryStream.WriteByte(10);
		memoryStream.WriteByte(240);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		int num = 0;
		if (cips.Length == 1)
		{
			memoryStream.Write(cips[0], 0, cips[0].Length);
			num += cips[0].Length;
			if (cips[0].Length % 2 == 1)
			{
				memoryStream.WriteByte(0);
			}
		}
		else
		{
			memoryStream.WriteByte(10);
			memoryStream.WriteByte(2);
			memoryStream.WriteByte(32);
			memoryStream.WriteByte(2);
			memoryStream.WriteByte(36);
			memoryStream.WriteByte(1);
			num += 8;
			memoryStream.Write(BitConverter.GetBytes((ushort)cips.Length), 0, 2);
			ushort num2 = (ushort)(2 + 2 * cips.Length);
			num += 2 * cips.Length;
			for (int i = 0; i < cips.Length; i++)
			{
				memoryStream.Write(BitConverter.GetBytes(num2), 0, 2);
				num2 = (ushort)(num2 + cips[i].Length);
			}
			for (int j = 0; j < cips.Length; j++)
			{
				memoryStream.Write(cips[j], 0, cips[j].Length);
				num += cips[j].Length;
			}
		}
		if (portSlot != null)
		{
			memoryStream.WriteByte((byte)((portSlot.Length + 1) / 2));
			memoryStream.WriteByte(0);
			memoryStream.Write(portSlot, 0, portSlot.Length);
			if (portSlot.Length % 2 == 1)
			{
				memoryStream.WriteByte(0);
			}
		}
		byte[] array = memoryStream.ToArray();
		BitConverter.GetBytes((short)num).CopyTo(array, 12);
		BitConverter.GetBytes((short)(array.Length - 4)).CopyTo(array, 2);
		return array;
	}

	/// <summary>
	/// 将所有的cip指定进行打包操作。
	/// </summary>
	/// <param name="portSlot">PLC所在的面板槽号</param>
	/// <param name="cips">所有的cip打包指令信息</param>
	/// <returns>包含服务的信息</returns>
	public static byte[] PackCleanCommandService(byte[] portSlot, params byte[][] cips)
	{
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.WriteByte(178);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		if (cips.Length == 1)
		{
			memoryStream.Write(cips[0], 0, cips[0].Length);
		}
		else
		{
			memoryStream.WriteByte(10);
			memoryStream.WriteByte(2);
			memoryStream.WriteByte(32);
			memoryStream.WriteByte(2);
			memoryStream.WriteByte(36);
			memoryStream.WriteByte(1);
			memoryStream.Write(BitConverter.GetBytes((ushort)cips.Length), 0, 2);
			ushort num = (ushort)(2 + 2 * cips.Length);
			for (int i = 0; i < cips.Length; i++)
			{
				memoryStream.Write(BitConverter.GetBytes(num), 0, 2);
				num = (ushort)(num + cips[i].Length);
			}
			for (int j = 0; j < cips.Length; j++)
			{
				memoryStream.Write(cips[j], 0, cips[j].Length);
			}
		}
		memoryStream.WriteByte((byte)((portSlot.Length + 1) / 2));
		memoryStream.WriteByte(0);
		memoryStream.Write(portSlot, 0, portSlot.Length);
		if (portSlot.Length % 2 == 1)
		{
			memoryStream.WriteByte(0);
		}
		byte[] array = memoryStream.ToArray();
		BitConverter.GetBytes((short)(array.Length - 4)).CopyTo(array, 2);
		return array;
	}

	/// <summary>
	/// 对于CIP的服务器返回多个标签读取的报文进行打包
	/// </summary>
	/// <param name="cips">多个读取结果</param>
	/// <returns>打包后的结束</returns>
	public static byte[] PackServerReturnMultiTags(List<byte[]> cips)
	{
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.WriteByte(178);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(138);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		memoryStream.Write(BitConverter.GetBytes((ushort)cips.Count), 0, 2);
		ushort num = (ushort)(2 + 2 * cips.Count);
		for (int i = 0; i < cips.Count; i++)
		{
			memoryStream.Write(BitConverter.GetBytes(num), 0, 2);
			num = (ushort)(num + cips[i].Length);
		}
		for (int j = 0; j < cips.Count; j++)
		{
			memoryStream.Write(cips[j], 0, cips[j].Length);
		}
		byte[] array = memoryStream.ToArray();
		BitConverter.GetBytes((short)(array.Length - 4)).CopyTo(array, 2);
		return array;
	}

	/// <summary>
	/// 打包一个读取所有特性数据的报文信息，需要传入slot
	/// </summary>
	/// <param name="portSlot">站号信息</param>
	/// <param name="sessionHandle">会话的ID信息</param>
	/// <returns>最终发送的报文数据</returns>
	public static byte[] PackCommandGetAttributesAll(byte[] portSlot, uint sessionHandle)
	{
		byte[] commandSpecificData = PackCommandSpecificData(new byte[4], PackCommandService(portSlot, new byte[6] { 1, 2, 32, 1, 36, 1 }));
		return PackRequestHeader(111, sessionHandle, commandSpecificData);
	}

	/// <summary>
	/// 根据数据创建反馈的数据信息
	/// </summary>
	/// <param name="data">反馈的数据信息</param>
	/// <param name="isRead">是否是读取</param>
	/// <returns>数据</returns>
	public static byte[] PackCommandResponse(byte[] data, bool isRead)
	{
		if (data == null)
		{
			return new byte[6] { 0, 0, 4, 0, 0, 0 };
		}
		return SoftBasic.SpliceArray<byte>(new byte[6]
		{
			(byte)(isRead ? 204u : 205u),
			0,
			0,
			0,
			0,
			0
		}, data);
	}

	/// <summary>
	/// 生成读取直接节点数据信息的内容
	/// </summary>
	/// <param name="service">cip指令内容</param>
	/// <returns>最终的指令值</returns>
	public static byte[] PackCommandSpecificData(params byte[][] service)
	{
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(10);
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(BitConverter.GetBytes(service.Length)[0]);
		memoryStream.WriteByte(BitConverter.GetBytes(service.Length)[1]);
		for (int i = 0; i < service.Length; i++)
		{
			memoryStream.Write(service[i], 0, service[i].Length);
		}
		return memoryStream.ToArray();
	}

	/// <summary>
	/// 将所有的cip指定进行打包操作。
	/// </summary>
	/// <param name="command">指令信息</param>
	/// <param name="code">服务器的代号信息</param>
	/// <param name="isConnected">是否基于连接的服务</param>
	/// <param name="sequence">当使用了基于连接的服务时，当前CIP消息号信息</param>
	/// <returns>包含服务的信息</returns>
	public static byte[] PackCommandSingleService(byte[] command, ushort code = 178, bool isConnected = false, ushort sequence = 0)
	{
		if (command == null)
		{
			command = new byte[0];
		}
		byte[] array = (isConnected ? new byte[6 + command.Length] : new byte[4 + command.Length]);
		array[0] = BitConverter.GetBytes(code)[0];
		array[1] = BitConverter.GetBytes(code)[1];
		array[2] = BitConverter.GetBytes(array.Length - 4)[0];
		array[3] = BitConverter.GetBytes(array.Length - 4)[1];
		command.CopyTo(array, isConnected ? 6 : 4);
		if (isConnected)
		{
			BitConverter.GetBytes(sequence).CopyTo(array, 4);
		}
		return array;
	}

	/// <summary>
	/// 向PLC注册会话ID的报文
	/// Register a message with the PLC for the session ID
	/// </summary>
	/// <param name="senderContext">发送的上下文信息</param>
	/// <returns>报文信息 -&gt; Message information</returns>
	public static byte[] RegisterSessionHandle(byte[] senderContext = null)
	{
		byte[] commandSpecificData = new byte[4] { 1, 0, 0, 0 };
		return PackRequestHeader(101, 0u, commandSpecificData, senderContext);
	}

	/// <summary>
	/// 获取卸载一个已注册的会话的报文
	/// Get a message to uninstall a registered session
	/// </summary>
	/// <param name="sessionHandle">当前会话的ID信息</param>
	/// <returns>字节报文信息 -&gt; BYTE message information</returns>
	public static byte[] UnRegisterSessionHandle(uint sessionHandle)
	{
		return PackRequestHeader(102, sessionHandle, new byte[0]);
	}

	/// <summary>
	/// 初步检查返回的CIP协议的报文是否正确
	/// Initially check whether the returned CIP protocol message is correct
	/// </summary>
	/// <param name="response">CIP的报文信息</param>
	/// <returns>是否正确的结果信息</returns>
	public static OperateResult CheckResponse(byte[] response)
	{
		try
		{
			int num = BitConverter.ToInt32(response, 8);
			if (num == 0)
			{
				return OperateResult.CreateSuccessResult();
			}
			string empty = string.Empty;
			return new OperateResult(num, num switch
			{
				1 => StringResources.Language.AllenBradleySessionStatus01, 
				2 => StringResources.Language.AllenBradleySessionStatus02, 
				3 => StringResources.Language.AllenBradleySessionStatus03, 
				100 => StringResources.Language.AllenBradleySessionStatus64, 
				101 => StringResources.Language.AllenBradleySessionStatus65, 
				105 => StringResources.Language.AllenBradleySessionStatus69, 
				_ => StringResources.Language.UnknownError, 
			});
		}
		catch (Exception ex)
		{
			return new OperateResult("CheckResponse failed: " + ex.Message + Environment.NewLine + "Source: " + response.ToHexString(' '));
		}
	}

	private static string GetErrorText(ushort err)
	{
		return err switch
		{
			4 => StringResources.Language.AllenBradley04, 
			5 => StringResources.Language.AllenBradley05, 
			6 => StringResources.Language.AllenBradley06, 
			10 => StringResources.Language.AllenBradley0A, 
			12 => StringResources.Language.AllenBradley0C, 
			19 => StringResources.Language.AllenBradley13, 
			28 => StringResources.Language.AllenBradley1C, 
			30 => StringResources.Language.AllenBradley1E, 
			32 => StringResources.Language.AllenBradley20, 
			38 => StringResources.Language.AllenBradley26, 
			_ => StringResources.Language.UnknownError, 
		};
	}

	/// <summary>
	/// 从PLC反馈的数据解析，返回解析后的数据内容，数据类型（在多项数据返回中无效），以及是否有更多的数据
	/// </summary>
	/// <param name="response">PLC的反馈数据</param>
	/// <param name="isRead">是否是返回的操作</param>
	/// <returns>带有结果标识的最终数据</returns>
	public static OperateResult<List<AbTagValue>, bool> ExtractActualDataList(byte[] response, bool isRead)
	{
		List<AbTagValue> list = new List<AbTagValue>();
		try
		{
			int num = 38;
			bool value = false;
			ushort num2 = BitConverter.ToUInt16(response, 38);
			if (BitConverter.ToInt32(response, 40) == 138)
			{
				num = 44;
				int num3 = BitConverter.ToUInt16(response, num);
				for (int i = 0; i < num3; i++)
				{
					int num4 = BitConverter.ToUInt16(response, num + 2 + i * 2) + num;
					int num5 = ((i == num3 - 1) ? response.Length : (BitConverter.ToUInt16(response, num + 4 + i * 2) + num));
					ushort num6 = BitConverter.ToUInt16(response, num4 + 2);
					switch (num6)
					{
					case 4:
						return new OperateResult<List<AbTagValue>, bool>
						{
							ErrorCode = num6,
							Message = StringResources.Language.AllenBradley04
						};
					case 5:
						return new OperateResult<List<AbTagValue>, bool>
						{
							ErrorCode = num6,
							Message = StringResources.Language.AllenBradley05
						};
					case 6:
						if (response[num + 2] == 210 || response[num + 2] == 204)
						{
							return new OperateResult<List<AbTagValue>, bool>
							{
								ErrorCode = num6,
								Message = StringResources.Language.AllenBradley06
							};
						}
						break;
					case 10:
						return new OperateResult<List<AbTagValue>, bool>
						{
							ErrorCode = num6,
							Message = StringResources.Language.AllenBradley0A
						};
					case 12:
						return new OperateResult<List<AbTagValue>, bool>
						{
							ErrorCode = num6,
							Message = StringResources.Language.AllenBradley0C
						};
					case 19:
						return new OperateResult<List<AbTagValue>, bool>
						{
							ErrorCode = num6,
							Message = StringResources.Language.AllenBradley13
						};
					case 28:
						return new OperateResult<List<AbTagValue>, bool>
						{
							ErrorCode = num6,
							Message = StringResources.Language.AllenBradley1C
						};
					case 30:
						return new OperateResult<List<AbTagValue>, bool>
						{
							ErrorCode = num6,
							Message = StringResources.Language.AllenBradley1E
						};
					case 38:
						return new OperateResult<List<AbTagValue>, bool>
						{
							ErrorCode = num6,
							Message = StringResources.Language.AllenBradley26
						};
					default:
						return new OperateResult<List<AbTagValue>, bool>
						{
							ErrorCode = num6,
							Message = StringResources.Language.UnknownError
						};
					case 0:
						break;
					}
					if (isRead)
					{
						AbTagValue abTagValue = new AbTagValue();
						abTagValue.Type = BitConverter.ToUInt16(response, num4 + 4);
						abTagValue.Value = response.SelectMiddle(num4 + 6, num5 - num4 - 6);
						list.Add(abTagValue);
					}
				}
			}
			else if (BitConverter.ToInt32(response, 40) == 142)
			{
				ushort num7 = BitConverter.ToUInt16(response, 42);
				if (num7 > 0)
				{
					return new OperateResult<List<AbTagValue>, bool>
					{
						ErrorCode = num7,
						Message = GetErrorText(num7)
					};
				}
				AbTagValue abTagValue2 = new AbTagValue();
				abTagValue2.Value = response.RemoveBegin(44);
				list.Add(abTagValue2);
			}
			else
			{
				if (BitConverter.ToInt32(response, 40) == 144)
				{
					ushort num8 = BitConverter.ToUInt16(response, 42);
					if (num8 > 0)
					{
						return new OperateResult<List<AbTagValue>, bool>
						{
							ErrorCode = num8,
							Message = GetErrorText(num8)
						};
					}
					return OperateResult.CreateSuccessResult(list, value);
				}
				byte b = response[num + 4];
				switch (b)
				{
				case 6:
					value = true;
					break;
				default:
					return new OperateResult<List<AbTagValue>, bool>
					{
						ErrorCode = b,
						Message = GetErrorText(b)
					};
				case 0:
					break;
				}
				if (response[num + 2] == 205 || response[num + 2] == 211)
				{
					return OperateResult.CreateSuccessResult(list, value);
				}
				if (response[num + 2] == 204 || response[num + 2] == 210)
				{
					AbTagValue abTagValue3 = new AbTagValue();
					abTagValue3.Type = BitConverter.ToUInt16(response, num + 6);
					abTagValue3.Value = response.SelectMiddle(num + 8, num2 - 6);
					list.Add(abTagValue3);
				}
				else if (response[num + 2] == 213)
				{
					AbTagValue abTagValue4 = new AbTagValue();
					abTagValue4.Value = response.SelectMiddle(num + 6, num2 - 4);
					list.Add(abTagValue4);
				}
			}
			return OperateResult.CreateSuccessResult(list, value);
		}
		catch (Exception ex)
		{
			return new OperateResult<List<AbTagValue>, bool>("ExtractActualData failed: " + ex.Message + Environment.NewLine + response.ToHexString(' '));
		}
	}

	/// <summary>
	/// 从PLC反馈的数据解析，返回解析后的数据内容，数据类型（在多项数据返回中无效），以及是否有更多的数据
	/// </summary>
	/// <param name="response">PLC的反馈数据</param>
	/// <param name="isRead">是否是返回的操作</param>
	/// <returns>带有结果标识的最终数据</returns>
	public static OperateResult<byte[], ushort, bool> ExtractActualData(byte[] response, bool isRead)
	{
		OperateResult<List<AbTagValue>, bool> operateResult = ExtractActualDataList(response, isRead);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[], ushort, bool>(operateResult);
		}
		if (operateResult.Content1.Count == 0)
		{
			return OperateResult.CreateSuccessResult(new byte[0], (ushort)0, operateResult.Content2);
		}
		List<byte> list = new List<byte>();
		foreach (AbTagValue item in operateResult.Content1)
		{
			list.AddRange(item.Value);
		}
		return OperateResult.CreateSuccessResult(list.ToArray(), operateResult.Content1[0].Type, operateResult.Content2);
	}

	internal static OperateResult<string> ExtractActualString(OperateResult<byte[], ushort, bool> read, IByteTransform byteTransform, Encoding encoding)
	{
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		try
		{
			if (read.Content2 == 218)
			{
				if (read.Content1.Length >= 1)
				{
					if (read.Content1[0] == 0)
					{
						return OperateResult.CreateSuccessResult(string.Empty);
					}
					if (read.Content1[0] >= read.Content1.Length)
					{
						return OperateResult.CreateSuccessResult(encoding.GetString(read.Content1));
					}
					return OperateResult.CreateSuccessResult(encoding.GetString(read.Content1, 1, read.Content1[0]));
				}
				return OperateResult.CreateSuccessResult(encoding.GetString(read.Content1));
			}
			if (read.Content1.Length >= 6)
			{
				int num = byteTransform.TransInt32(read.Content1, 2);
				if (num == 0)
				{
					return OperateResult.CreateSuccessResult(string.Empty);
				}
				return OperateResult.CreateSuccessResult(encoding.GetString(read.Content1, 6, num));
			}
			return OperateResult.CreateSuccessResult(encoding.GetString(read.Content1));
		}
		catch (Exception ex)
		{
			return new OperateResult<string>(ex.Message + " Source: " + read.Content1.ToHexString(' '));
		}
	}

	/// <summary>
	/// 从PLC里读取当前PLC的型号信息
	/// Read the current PLC model information from the PLC
	/// </summary>
	/// <param name="plc">PLC对象</param>
	/// <returns>型号数据信息</returns>
	public static OperateResult<string> ReadPlcType(IReadWriteDevice plc)
	{
		byte[] send = "00 00 00 00 00 00 02 00 00 00 00 00 b2 00 06 00 01 02 20 01 24 01".ToHexBytes();
		OperateResult<byte[]> operateResult = plc.ReadFromCoreServer(send);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		if (operateResult.Content.Length > 59 && operateResult.Content.Length >= 59 + operateResult.Content[58])
		{
			return OperateResult.CreateSuccessResult(Encoding.UTF8.GetString(operateResult.Content, 59, operateResult.Content[58]));
		}
		return new OperateResult<string>("Data is too short: " + operateResult.Content.ToHexString(' '));
	}

	/// <summary>
	/// 从PLC里读取当前PLC的型号信息
	/// Read the current PLC model information from the PLC
	/// </summary>
	/// <param name="plc">PLC对象</param>
	/// <returns>型号数据信息</returns>
	public static async Task<OperateResult<string>> ReadPlcTypeAsync(IReadWriteDevice plc)
	{
		byte[] buffer = "00 00 00 00 00 00 02 00 00 00 00 00 b2 00 06 00 01 02 20 01 24 01".ToHexBytes();
		OperateResult<byte[]> read = await plc.ReadFromCoreServerAsync(buffer);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		if (read.Content.Length > 59 && read.Content.Length >= 59 + read.Content[58])
		{
			return OperateResult.CreateSuccessResult(Encoding.UTF8.GetString(read.Content, 59, read.Content[58]));
		}
		return new OperateResult<string>("Data is too short: " + read.Content.ToHexString(' '));
	}

	/// <summary>
	/// 读取指定地址的日期数据，最小日期为 1970年1月1日，当PLC的变量类型为 "Date" 和 "TimeAndDate" 时，都可以用本方法读取。
	/// Read the date data of the specified address. The minimum date is January 1, 1970. When the PLC variable type is "Date" and "TimeAndDate", this method can be used to read.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <returns>日期结果对象</returns>
	public static OperateResult<DateTime> ReadDate(IReadWriteCip plc, string address)
	{
		OperateResult<long> operateResult = plc.ReadInt64(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<DateTime>(operateResult);
		}
		long value = operateResult.Content / 100;
		return OperateResult.CreateSuccessResult(new DateTime(1970, 1, 1).AddTicks(value));
	}

	/// <summary>
	/// 使用日期格式（Date）将指定的数据写入到指定的地址里，PLC的地址类型变量必须为 "Date"，否则写入失败。
	/// Use the date format (Date) to write the specified data to the specified address. The PLC address type variable must be "Date", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="date">时间信息</param>
	/// <returns>是否写入成功</returns>
	public static OperateResult WriteDate(IReadWriteCip plc, string address, DateTime date)
	{
		long value = (date.Date - new DateTime(1970, 1, 1)).Ticks * 100;
		return plc.WriteTag(address, 8, plc.ByteTransform.TransByte(value));
	}

	/// <summary>
	/// 使用日期格式（Date）将指定的数据写入到指定的地址里，PLC的地址类型变量必须为 "Date"，否则写入失败。
	/// Use the date format (Date) to write the specified data to the specified address. The PLC address type variable must be "Date", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="date">时间信息</param>
	/// <returns>是否写入成功</returns>
	public static OperateResult WriteTimeAndDate(IReadWriteCip plc, string address, DateTime date)
	{
		long value = (date - new DateTime(1970, 1, 1)).Ticks * 100;
		return plc.WriteTag(address, 10, plc.ByteTransform.TransByte(value));
	}

	/// <summary>
	/// 读取指定地址的时间数据，最小时间为 0，如果获取秒，可以访问 ，当PLC的变量类型为 "Time" 和 "TimeOfDate" 时，都可以用本方法读取。
	/// Read the time data of the specified address. The minimum time is 0. If you get seconds, you can access .
	/// When the PLC variable type is "Time" and "TimeOfDate", you can use this Method to read.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <returns>时间的结果对象</returns>
	public static OperateResult<TimeSpan> ReadTime(IReadWriteCip plc, string address)
	{
		OperateResult<long> operateResult = plc.ReadInt64(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<TimeSpan>(operateResult);
		}
		long value = operateResult.Content / 100;
		return OperateResult.CreateSuccessResult(TimeSpan.FromTicks(value));
	}

	/// <summary>
	/// 使用时间格式（TIME）将时间数据写入到PLC中指定的地址里去，PLC的地址类型变量必须为 "TIME"，否则写入失败。
	/// Use the time format (TIME) to write the time data to the address specified in the PLC. The PLC address type variable must be "TIME", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="time">时间参数变量</param>
	/// <returns>是否写入成功</returns>
	public static OperateResult WriteTime(IReadWriteCip plc, string address, TimeSpan time)
	{
		return plc.WriteTag(address, 9, plc.ByteTransform.TransByte(time.Ticks * 100));
	}

	/// <summary>
	/// 使用时间格式（TimeOfDate）将时间数据写入到PLC中指定的地址里去，PLC的地址类型变量必须为 "TimeOfDate"，否则写入失败。
	/// Use the time format (TimeOfDate) to write the time data to the address specified in the PLC. The PLC address type variable must be "TimeOfDate", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="timeOfDate">时间参数变量</param>
	/// <returns>是否写入成功</returns>
	public static OperateResult WriteTimeOfDate(IReadWriteCip plc, string address, TimeSpan timeOfDate)
	{
		return plc.WriteTag(address, 11, plc.ByteTransform.TransByte(timeOfDate.Ticks * 100));
	}

	/// <summary>
	/// 读取指定地址的日期数据，最小日期为 1970年1月1日，当PLC的变量类型为 "Date" 和 "TimeAndDate" 时，都可以用本方法读取。
	/// Read the date data of the specified address. The minimum date is January 1, 1970. When the PLC variable type is "Date" and "TimeAndDate", this method can be used to read.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <returns>日期结果对象</returns>
	public static async Task<OperateResult<DateTime>> ReadDateAsync(IReadWriteCip plc, string address)
	{
		OperateResult<long> read = await plc.ReadInt64Async(address);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<DateTime>(read);
		}
		long tick = read.Content / 100;
		return OperateResult.CreateSuccessResult(new DateTime(1970, 1, 1).AddTicks(tick));
	}

	/// <summary>
	/// 使用日期格式（Date）将指定的数据写入到指定的地址里，PLC的地址类型变量必须为 "Date"，否则写入失败。
	/// Use the date format (Date) to write the specified data to the specified address. The PLC address type variable must be "Date", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="date">时间信息</param>
	/// <returns>是否写入成功</returns>
	public static async Task<OperateResult> WriteDateAsync(IReadWriteCip plc, string address, DateTime date)
	{
		long tick = (date.Date - new DateTime(1970, 1, 1)).Ticks * 100;
		return await plc.WriteTagAsync(address, 8, plc.ByteTransform.TransByte(tick));
	}

	/// <summary>
	/// 使用日期格式（Date）将指定的数据写入到指定的地址里，PLC的地址类型变量必须为 "Date"，否则写入失败。
	/// Use the date format (Date) to write the specified data to the specified address. The PLC address type variable must be "Date", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="date">时间信息</param>
	/// <returns>是否写入成功</returns>
	public static async Task<OperateResult> WriteTimeAndDateAsync(IReadWriteCip plc, string address, DateTime date)
	{
		long tick = (date - new DateTime(1970, 1, 1)).Ticks * 100;
		return await plc.WriteTagAsync(address, 10, plc.ByteTransform.TransByte(tick));
	}

	/// <summary>
	/// 读取指定地址的时间数据，最小时间为 0，如果获取秒，可以访问 ，当PLC的变量类型为 "Time" 和 "TimeOfDate" 时，都可以用本方法读取。
	/// Read the time data of the specified address. The minimum time is 0. If you get seconds, you can access .
	/// When the PLC variable type is "Time" and "TimeOfDate", you can use this Method to read.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <returns>时间的结果对象</returns>
	public static async Task<OperateResult<TimeSpan>> ReadTimeAsync(IReadWriteCip plc, string address)
	{
		OperateResult<long> read = await plc.ReadInt64Async(address);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<TimeSpan>(read);
		}
		long tick = read.Content / 100;
		return OperateResult.CreateSuccessResult(TimeSpan.FromTicks(tick));
	}

	/// <summary>
	/// 使用时间格式（TIME）将时间数据写入到PLC中指定的地址里去，PLC的地址类型变量必须为 "TIME"，否则写入失败。
	/// Use the time format (TIME) to write the time data to the address specified in the PLC. The PLC address type variable must be "TIME", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="time">时间参数变量</param>
	/// <returns>是否写入成功</returns>
	public static async Task<OperateResult> WriteTimeAsync(IReadWriteCip plc, string address, TimeSpan time)
	{
		return await plc.WriteTagAsync(address, 9, plc.ByteTransform.TransByte(time.Ticks * 100));
	}

	/// <summary>
	/// 使用时间格式（TimeOfDate）将时间数据写入到PLC中指定的地址里去，PLC的地址类型变量必须为 "TimeOfDate"，否则写入失败。
	/// Use the time format (TimeOfDate) to write the time data to the address specified in the PLC. The PLC address type variable must be "TimeOfDate", otherwise the writing will fail.
	/// </summary>
	/// <param name="plc">当前的通信对象信息</param>
	/// <param name="address">PLC里变量的地址</param>
	/// <param name="timeOfDate">时间参数变量</param>
	/// <returns>是否写入成功</returns>
	public static async Task<OperateResult> WriteTimeOfDateAsync(IReadWriteCip plc, string address, TimeSpan timeOfDate)
	{
		return await plc.WriteTagAsync(address, 11, plc.ByteTransform.TransByte(timeOfDate.Ticks * 100));
	}
}
