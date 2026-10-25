// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;

namespace HslCommunication.Profinet.YASKAWA.Helper;

/// <summary>
/// Memobus的辅助类对象
/// </summary>
public class MemobusHelper
{
	internal static byte[] PackCommandWithHeader(byte[] command, long id)
	{
		byte[] buffer = new byte[12 + command.Length];
		buffer[0] = 17;
		buffer[1] = (byte)id;
		buffer[2] = 0;
		buffer[3] = 0;
		buffer[6] = BitConverter.GetBytes(buffer.Length)[0];
		buffer[7] = BitConverter.GetBytes(buffer.Length)[1];
		command.CopyTo(buffer, 12);
		return buffer;
	}

	internal static string GetErrorText(byte err)
	{
		return err switch
		{
			1 => StringResources.Language.Memobus01, 
			2 => StringResources.Language.Memobus02, 
			3 => StringResources.Language.Memobus03, 
			64 => StringResources.Language.Memobus40, 
			65 => StringResources.Language.Memobus41, 
			66 => StringResources.Language.Memobus42, 
			_ => StringResources.Language.UnknownError, 
		};
	}

	internal static OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		try
		{
			if (send.Length > 15 && response.Length > 15)
			{
				if (send[15] + 128 == response[15] && response.Length >= 18)
				{
					return new OperateResult<byte[]>(response[17], GetErrorText(response[17]) + " Source: " + response.ToHexString(' '));
				}
				if (send[15] != response[15])
				{
					return new OperateResult<byte[]>(response[15], "Send SFC not same as back SFC:" + response.ToHexString());
				}
			}
			return OperateResult.CreateSuccessResult(response.RemoveBegin(12));
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("UnpackResponseContent failed: " + ex.Message + "  Source: " + response.ToHexString(' '));
		}
	}

	private static void SetByteHead(byte[] buffer, byte mfc, byte sfc, byte cpuTo, byte cpuFrom)
	{
		buffer[0] = BitConverter.GetBytes(buffer.Length - 2)[0];
		buffer[1] = BitConverter.GetBytes(buffer.Length - 2)[1];
		buffer[2] = mfc;
		buffer[3] = sfc;
		buffer[4] = (byte)((cpuTo << 4) + cpuFrom);
	}

	internal static byte GetAddressDataType(string address)
	{
		byte result = 77;
		if (address[0] == 'M' || address[0] == 'm')
		{
			result = 77;
		}
		else if (address[0] == 'G' || address[0] == 'g')
		{
			result = 71;
		}
		else if (address[0] == 'I' || address[0] == 'i')
		{
			result = 73;
		}
		else if (address[0] == 'O' || address[0] == 'o')
		{
			result = 79;
		}
		else if (address[0] == 'S' || address[0] == 's')
		{
			result = 83;
		}
		return result;
	}

	internal static int CalculateBoolIndex(string address)
	{
		address = ((address[1] != 'B' && address[1] != 'b') ? address.Substring(1) : address.Substring(2));
		int num = address.IndexOf('.');
		if (num > 0)
		{
			return Convert.ToInt32(address.Substring(0, num)) * 16 + HslHelper.CalculateBitStartIndex(address.Substring(num + 1));
		}
		return Convert.ToInt32(address.Substring(0, address.Length - 1)) * 16 + HslHelper.CalculateBitStartIndex(address.Substring(address.Length - 1));
	}

	/// <summary>
	/// 构建读取的命令报文，支持功能码 01,02,03,04,09,0A
	/// </summary>
	/// <param name="mfc">主功能码</param>
	/// <param name="sfc">子功能码</param>
	/// <param name="cpuTo">目标的CPU编号</param>
	/// <param name="cpuFrom">发送源CPU编号</param>
	/// <param name="address">起始地址</param>
	/// <param name="length">读取地址长度</param>
	/// <returns>结果报文信息</returns>
	internal static OperateResult<byte[]> BuildReadCommand(byte mfc, byte sfc, byte cpuTo, byte cpuFrom, string address, ushort length)
	{
		if (address.StartsWith(new string[5] { "M", "G", "I", "O", "S" }))
		{
			byte addressDataType = GetAddressDataType(address);
			if (address[1] == 'B' || address[1] == 'b' || address.IndexOf('.') > 0)
			{
				int value = CalculateBoolIndex(address);
				byte[] buffer = new byte[16];
				SetByteHead(buffer, 67, 65, cpuTo, cpuFrom);
				buffer[6] = addressDataType;
				BitConverter.GetBytes(value).CopyTo(buffer, 8);
				BitConverter.GetBytes(length).CopyTo(buffer, 12);
				return OperateResult.CreateSuccessResult(buffer);
			}
			byte[] buffer2 = new byte[14];
			SetByteHead(buffer2, 67, 73, cpuTo, cpuFrom);
			buffer2[6] = addressDataType;
			BitConverter.GetBytes(Convert.ToUInt32(address.Substring(1))).CopyTo(buffer2, 8);
			BitConverter.GetBytes(length).CopyTo(buffer2, 12);
			return OperateResult.CreateSuccessResult(buffer2);
		}
		if (!ushort.TryParse(address, out var result))
		{
			return new OperateResult<byte[]>("Address[" + address + "] wrong, not supported");
		}
		if (sfc == 1 || sfc == 2 || sfc == 3 || sfc == 4)
		{
			byte[] buffer3 = new byte[9];
			SetByteHead(buffer3, mfc, sfc, cpuTo, cpuFrom);
			buffer3[5] = BitConverter.GetBytes(result)[1];
			buffer3[6] = BitConverter.GetBytes(result)[0];
			buffer3[7] = BitConverter.GetBytes(length)[1];
			buffer3[8] = BitConverter.GetBytes(length)[0];
			return OperateResult.CreateSuccessResult(buffer3);
		}
		if (sfc == 9 || sfc == 10)
		{
			byte[] buffer4 = new byte[10];
			SetByteHead(buffer4, mfc, sfc, cpuTo, cpuFrom);
			buffer4[6] = BitConverter.GetBytes(result)[0];
			buffer4[7] = BitConverter.GetBytes(result)[1];
			buffer4[8] = BitConverter.GetBytes(length)[0];
			buffer4[9] = BitConverter.GetBytes(length)[1];
			return OperateResult.CreateSuccessResult(buffer4);
		}
		return new OperateResult<byte[]>($"SFC:{sfc} {StringResources.Language.NotSupportedFunction}");
	}

	internal static OperateResult<byte[]> BuildReadRandomCommand(byte mfc, byte sfc, byte cpuTo, byte cpuFrom, ushort[] address)
	{
		byte[] buffer = new byte[8 + address.Length * 2];
		SetByteHead(buffer, mfc, sfc, cpuTo, cpuFrom);
		buffer[6] = BitConverter.GetBytes(address.Length)[0];
		buffer[7] = BitConverter.GetBytes(address.Length)[1];
		for (int i = 0; i < address.Length; i++)
		{
			buffer[8 + i * 2] = BitConverter.GetBytes(address[i])[0];
			buffer[8 + i * 2 + 1] = BitConverter.GetBytes(address[i])[1];
		}
		return OperateResult.CreateSuccessResult(buffer);
	}

	internal static OperateResult<byte[]> BuildReadRandomCommand(byte cpuTo, byte cpuFrom, string[] address)
	{
		byte[] buffer = new byte[8 + address.Length * 6];
		SetByteHead(buffer, 67, 77, cpuTo, cpuFrom);
		buffer[6] = BitConverter.GetBytes(address.Length)[0];
		buffer[7] = BitConverter.GetBytes(address.Length)[1];
		for (int i = 0; i < address.Length; i++)
		{
			byte addressDataType = GetAddressDataType(address[i]);
			buffer[8 + i * 6] = addressDataType;
			buffer[8 + i * 6 + 1] = 2;
			BitConverter.GetBytes(Convert.ToUInt32(address[i].Substring(1))).CopyTo(buffer, 8 + i * 6 + 2);
		}
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 构建写入单一的线圈的状态变更的报文
	/// </summary>
	/// <param name="cpuTo">目标的CPU编号</param>
	/// <param name="cpuFrom">发送源CPU编号</param>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入的通断值信息</param>
	/// <returns>写入的报文</returns>
	internal static OperateResult<byte[]> BuildWriteCommand(byte cpuTo, byte cpuFrom, string address, bool value)
	{
		byte mfc = (byte)HslHelper.ExtractParameter(ref address, "mfc", 32);
		byte sfc = (byte)HslHelper.ExtractParameter(ref address, "x", 5);
		if (!ushort.TryParse(address, out var result))
		{
			return new OperateResult<byte[]>("Address[" + address + "] wrong, not supported(ushort)");
		}
		byte[] buffer = new byte[9];
		SetByteHead(buffer, mfc, sfc, cpuTo, cpuFrom);
		buffer[5] = BitConverter.GetBytes(result)[1];
		buffer[6] = BitConverter.GetBytes(result)[0];
		buffer[7] = (byte)(value ? 255u : 0u);
		buffer[8] = 0;
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 构建写入单一的线圈的状态变更的报文
	/// </summary>
	/// <param name="cpuTo">目标的CPU编号</param>
	/// <param name="cpuFrom">发送源CPU编号</param>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入的通断值信息</param>
	/// <returns>写入的报文</returns>
	internal static OperateResult<byte[]> BuildWriteCommand(byte cpuTo, byte cpuFrom, string address, bool[] value)
	{
		byte mfc = (byte)HslHelper.ExtractParameter(ref address, "mfc", 32);
		byte sfc = (byte)HslHelper.ExtractParameter(ref address, "x", 15);
		if (address.StartsWith(new string[5] { "M", "G", "I", "O", "S" }))
		{
			byte addressDataType = GetAddressDataType(address);
			if (address[1] == 'B' || address[1] == 'b' || address.IndexOf('.') > 0)
			{
				int value2 = CalculateBoolIndex(address);
				byte[] buffer = new byte[16 + (value.Length + 7) / 8];
				SetByteHead(buffer, 67, 79, cpuTo, cpuFrom);
				buffer[6] = addressDataType;
				BitConverter.GetBytes(value2).CopyTo(buffer, 8);
				BitConverter.GetBytes(value.Length).CopyTo(buffer, 12);
				value.ToByteArray().CopyTo(buffer, 16);
				return OperateResult.CreateSuccessResult(buffer);
			}
			return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
		}
		if (!ushort.TryParse(address, out var result))
		{
			return new OperateResult<byte[]>("Address[" + address + "] wrong, not supported");
		}
		byte[] array2 = SoftBasic.BoolArrayToByte(value);
		byte[] buffer2 = new byte[9 + array2.Length];
		SetByteHead(buffer2, mfc, sfc, cpuTo, cpuFrom);
		buffer2[5] = BitConverter.GetBytes(result)[1];
		buffer2[6] = BitConverter.GetBytes(result)[0];
		buffer2[7] = BitConverter.GetBytes(value.Length)[1];
		buffer2[8] = BitConverter.GetBytes(value.Length)[0];
		array2.CopyTo(buffer2, 9);
		return OperateResult.CreateSuccessResult(buffer2);
	}

	/// <summary>
	/// 构建写入单一的线圈的状态变更的报文
	/// </summary>
	/// <param name="cpuTo">目标的CPU编号</param>
	/// <param name="cpuFrom">发送源CPU编号</param>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入的通断值信息</param>
	/// <returns>写入的报文</returns>
	internal static OperateResult<byte[]> BuildWriteCommand(byte mfc, byte sfc, byte cpuTo, byte cpuFrom, string address, short value)
	{
		if (!ushort.TryParse(address, out var result))
		{
			return new OperateResult<byte[]>("Address[" + address + "] wrong, not supported(ushort)");
		}
		byte[] buffer = new byte[9];
		SetByteHead(buffer, mfc, sfc, cpuTo, cpuFrom);
		buffer[5] = BitConverter.GetBytes(result)[1];
		buffer[6] = BitConverter.GetBytes(result)[0];
		buffer[7] = BitConverter.GetBytes(value)[1];
		buffer[8] = BitConverter.GetBytes(value)[0];
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 构建写入单一的线圈的状态变更的报文
	/// </summary>
	/// <param name="cpuTo">目标的CPU编号</param>
	/// <param name="cpuFrom">发送源CPU编号</param>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入的通断值信息</param>
	/// <returns>写入的报文</returns>
	internal static OperateResult<byte[]> BuildWriteCommand(byte mfc, byte sfc, byte cpuTo, byte cpuFrom, string address, ushort value)
	{
		if (!ushort.TryParse(address, out var result))
		{
			return new OperateResult<byte[]>("Address[" + address + "] wrong, not supported(ushort)");
		}
		byte[] buffer = new byte[9];
		SetByteHead(buffer, mfc, sfc, cpuTo, cpuFrom);
		buffer[5] = BitConverter.GetBytes(result)[1];
		buffer[6] = BitConverter.GetBytes(result)[0];
		buffer[7] = BitConverter.GetBytes(value)[1];
		buffer[8] = BitConverter.GetBytes(value)[0];
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 构建写入单一的线圈的状态变更的报文
	/// </summary>
	/// <param name="cpuTo">目标的CPU编号</param>
	/// <param name="cpuFrom">发送源CPU编号</param>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入的通断值信息</param>
	/// <returns>写入的报文</returns>
	internal static OperateResult<byte[]> BuildWriteCommand(byte cpuTo, byte cpuFrom, string address, byte[] value)
	{
		byte mfc = (byte)HslHelper.ExtractParameter(ref address, "mfc", 32);
		byte b = (byte)HslHelper.ExtractParameter(ref address, "x", 16);
		if (b == 3)
		{
			b = 16;
		}
		if (b == 9)
		{
			b = 11;
		}
		if (address.StartsWithAndNumber(new string[5] { "M", "G", "I", "O", "S" }))
		{
			byte addressDataType = GetAddressDataType(address);
			byte[] buffer = new byte[14 + value.Length];
			SetByteHead(buffer, 67, 75, cpuTo, cpuFrom);
			buffer[6] = addressDataType;
			BitConverter.GetBytes(Convert.ToUInt32(address.Substring(1))).CopyTo(buffer, 8);
			BitConverter.GetBytes(value.Length / 2).CopyTo(buffer, 12);
			SoftBasic.BytesReverseByWord(value).CopyTo(buffer, 14);
			return OperateResult.CreateSuccessResult(buffer);
		}
		if (!ushort.TryParse(address, out var result))
		{
			return new OperateResult<byte[]>("Address[" + address + "] wrong, not supported");
		}
		switch (b)
		{
		case 11:
		{
			byte[] buffer2 = new byte[10 + value.Length];
			SetByteHead(buffer2, mfc, b, cpuTo, cpuFrom);
			buffer2[6] = BitConverter.GetBytes(result)[0];
			buffer2[7] = BitConverter.GetBytes(result)[1];
			buffer2[8] = BitConverter.GetBytes(value.Length / 2)[0];
			buffer2[9] = BitConverter.GetBytes(value.Length / 2)[1];
			SoftBasic.BytesReverseByWord(value).CopyTo(buffer2, 10);
			return OperateResult.CreateSuccessResult(buffer2);
		}
		case 16:
		{
			byte[] buffer3 = new byte[9 + value.Length];
			SetByteHead(buffer3, mfc, b, cpuTo, cpuFrom);
			buffer3[5] = BitConverter.GetBytes(result)[1];
			buffer3[6] = BitConverter.GetBytes(result)[0];
			buffer3[7] = BitConverter.GetBytes(value.Length / 2)[1];
			buffer3[8] = BitConverter.GetBytes(value.Length / 2)[0];
			value.CopyTo(buffer3, 9);
			return OperateResult.CreateSuccessResult(buffer3);
		}
		default:
			return new OperateResult<byte[]>($"SFC:{b} {StringResources.Language.NotSupportedFunction}");
		}
	}

	internal static OperateResult<byte[]> BuildWriteRandomCommand(byte mfc, byte sfc, byte cpuTo, byte cpuFrom, ushort[] address, byte[] value)
	{
		if (value.Length != address.Length * 2)
		{
			return new OperateResult<byte[]>("value.Length must be twice as much as address.Length");
		}
		byte[] buffer = new byte[8 + address.Length * 4];
		SetByteHead(buffer, mfc, sfc, cpuTo, cpuFrom);
		buffer[6] = BitConverter.GetBytes(address.Length)[0];
		buffer[7] = BitConverter.GetBytes(address.Length)[1];
		for (int i = 0; i < address.Length; i++)
		{
			buffer[8 + i * 4] = BitConverter.GetBytes(address[i])[0];
			buffer[8 + i * 4 + 1] = BitConverter.GetBytes(address[i])[1];
			buffer[8 + i * 4 + 2] = value[i * 2 + 1];
			buffer[8 + i * 4 + 3] = value[i * 2];
		}
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 批量读取数组信息，需要指定地址和长度，返回 数组
	/// Batch read  array information, need to specify the address and length, return  array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的 bool[] 数组</returns>
	public static OperateResult<bool[]> ReadBool(IMemobus memobus, string address, ushort length)
	{
		byte mfc = (byte)HslHelper.ExtractParameter(ref address, "mfc", 32);
		byte sfc = (byte)HslHelper.ExtractParameter(ref address, "x", 1);
		OperateResult<byte[]> operateResult = BuildReadCommand(mfc, sfc, memobus.CpuTo, memobus.CpuFrom, address, length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		OperateResult<byte[]> operateResult2 = memobus.ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult2);
		}
		if (operateResult2.Content[3] == 65)
		{
			return OperateResult.CreateSuccessResult(operateResult2.Content.RemoveBegin(8).ToBoolArray().SelectBegin(length));
		}
		return OperateResult.CreateSuccessResult(operateResult2.Content.RemoveBegin(5).ToBoolArray().SelectBegin(length));
	}

	/// <summary>
	/// 写入单个的数据，返回是否成功
	/// Write a single  data, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public static OperateResult Write(IMemobus memobus, string address, bool value)
	{
		if (address.StartsWith(new string[5] { "M", "G", "I", "O", "S" }))
		{
			return Write(memobus, address, new bool[1] { value });
		}
		OperateResult<byte[]> operateResult = BuildWriteCommand(memobus.CpuTo, memobus.CpuFrom, address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = memobus.ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 批量写入数组数据，返回是否成功
	/// Batch write  array data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public static OperateResult Write(IMemobus memobus, string address, bool[] value)
	{
		OperateResult<byte[]> operateResult = BuildWriteCommand(memobus.CpuTo, memobus.CpuFrom, address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = memobus.ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult();
	}

	private static OperateResult<byte[]> ExtraContent(string address, byte[] content)
	{
		if (content[2] == 32)
		{
			if (content[3] == 3 || content[3] == 4)
			{
				return OperateResult.CreateSuccessResult(content.RemoveBegin(5));
			}
			if (content[3] == 9 || content[3] == 10)
			{
				return OperateResult.CreateSuccessResult(SoftBasic.BytesReverseByWord(content.RemoveBegin(8)));
			}
			return OperateResult.CreateSuccessResult(content.RemoveBegin(5));
		}
		if (content[2] == 67)
		{
			if (content[3] == 73 || content[3] == 77)
			{
				return OperateResult.CreateSuccessResult(SoftBasic.BytesReverseByWord(content.RemoveBegin(10)));
			}
			return OperateResult.CreateSuccessResult(content.RemoveBegin(8));
		}
		return new OperateResult<byte[]>($"[{address}], mfc[{content[2]}] is not supported");
	}

	/// <summary>
	/// 批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组
	/// Batch read byte array information, need to specify the address and length, return the original byte array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public static OperateResult<byte[]> Read(IMemobus memobus, string address, ushort length)
	{
		byte mfc = (byte)HslHelper.ExtractParameter(ref address, "mfc", 32);
		byte sfc = (byte)HslHelper.ExtractParameter(ref address, "x", 3);
		OperateResult<byte[]> operateResult = BuildReadCommand(mfc, sfc, memobus.CpuTo, memobus.CpuFrom, address, length);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = memobus.ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		return ExtraContent(address, operateResult2.Content);
	}

	/// <summary>
	/// 写入原始的byte数组数据到指定的地址，返回是否写入成功
	/// Write the original byte array data to the specified address, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public static OperateResult Write(IMemobus memobus, string address, byte[] value)
	{
		OperateResult<byte[]> operateResult = BuildWriteCommand(memobus.CpuTo, memobus.CpuFrom, address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = memobus.ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 写入short数据，返回是否成功
	/// Write short data, returns whether success
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public static OperateResult Write(IMemobus memobus, string address, short value, Func<string, short, OperateResult> writeShort)
	{
		byte mfc = (byte)HslHelper.ExtractParameter(ref address, "mfc", 32);
		byte b = (byte)HslHelper.ExtractParameter(ref address, "x", 6);
		if (b == 11 || b == 9)
		{
			return writeShort($"x={b};{address}", value);
		}
		if (b == 3)
		{
			b = 6;
		}
		OperateResult<byte[]> operateResult = BuildWriteCommand(mfc, b, memobus.CpuTo, memobus.CpuFrom, address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = memobus.ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 写入ushort数据，返回是否成功
	/// Write ushort data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public static OperateResult Write(IMemobus memobus, string address, ushort value, Func<string, ushort, OperateResult> writeUShort)
	{
		byte mfc = (byte)HslHelper.ExtractParameter(ref address, "mfc", 32);
		byte b = (byte)HslHelper.ExtractParameter(ref address, "x", 6);
		if (b == 11 || b == 9)
		{
			return writeUShort($"x={b};{address}", value);
		}
		if (b == 3)
		{
			b = 6;
		}
		OperateResult<byte[]> operateResult = BuildWriteCommand(mfc, b, memobus.CpuTo, memobus.CpuFrom, address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = memobus.ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 随机读取扩展的保持寄存器的内容，也即读取不连续地址的字数据，可以指定多个地址，然后一次性读取所有的数据，然后解析出实际的数据
	/// Randomly read the contents of the extended hold register, that is, read word data of discontinuous addresses,
	/// you can specify multiple addresses, then read all the data at once, and then parse out the actual data
	/// </summary>
	/// <param name="memobus">PLC通信对象</param>
	/// <param name="address">地址信息</param>
	/// <returns>读取的原始字节结果信息</returns>
	/// <remarks>
	/// 注意，本方法只能针对扩展的保持寄存器进行读取
	/// </remarks>
	public static OperateResult<byte[]> ReadRandom(IMemobus memobus, ushort[] address)
	{
		OperateResult<byte[]> operateResult = BuildReadRandomCommand(32, 13, memobus.CpuTo, memobus.CpuFrom, address);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = memobus.ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(SoftBasic.BytesReverseByWord(operateResult2.Content.RemoveBegin(8)));
	}

	/// <summary>
	/// 随机读取扩展的保持寄存器的内容，也即读取不连续地址的字数据，可以指定多个地址，然后一次性读取所有的数据，然后解析出实际的数据
	/// Randomly read the contents of the extended hold register, that is, read word data of discontinuous addresses,
	/// you can specify multiple addresses, then read all the data at once, and then parse out the actual data
	/// </summary>
	/// <param name="memobus">PLC通信对象</param>
	/// <param name="address">地址信息</param>
	/// <returns>读取的原始字节结果信息</returns>
	/// <remarks>
	/// 注意，本方法只能针对扩展的保持寄存器进行读取
	/// </remarks>
	public static OperateResult<byte[]> ReadRandom(IMemobus memobus, string[] address)
	{
		OperateResult<byte[]> operateResult = BuildReadRandomCommand(memobus.CpuTo, memobus.CpuFrom, address);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = memobus.ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(SoftBasic.BytesReverseByWord(operateResult2.Content.RemoveBegin(8)));
	}

	/// <summary>
	/// 随机写入扩展的保持寄存器的内容，也即写入不连续的地址的字数据，字节数组的长度必须为地址数组长度的两倍，才能正确写入。
	/// Write the contents of the extended hold registers randomly, that is, write word data for discontinuous addresses,
	/// and the byte array must be twice the length of the address array to be written correctly.
	/// </summary>
	/// <param name="memobus">PLC通信对象</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">数据信息</param>
	/// <returns>是否写入成功的结果对象</returns>
	/// <remarks>
	/// 注意，本方法只能针对扩展的保持寄存器进行读取
	/// </remarks>
	public static OperateResult WriteRandom(IMemobus memobus, ushort[] address, byte[] value)
	{
		OperateResult<byte[]> operateResult = BuildWriteRandomCommand(32, 14, memobus.CpuTo, memobus.CpuFrom, address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = memobus.ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 批量读取数组信息，需要指定地址和长度，返回 数组
	/// Batch read  array information, need to specify the address and length, return  array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的 bool[] 数组</returns>
	public static async Task<OperateResult<bool[]>> ReadBoolAsync(IMemobus memobus, string address, ushort length)
	{
		byte mfc = (byte)HslHelper.ExtractParameter(ref address, "mfc", 32);
		byte sfc = (byte)HslHelper.ExtractParameter(ref address, "x", 1);
		OperateResult<byte[]> command = BuildReadCommand(mfc, sfc, memobus.CpuTo, memobus.CpuFrom, address, length);
		if (!command.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(command);
		}
		OperateResult<byte[]> read = await memobus.ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(read);
		}
		if (read.Content[3] == 65)
		{
			return OperateResult.CreateSuccessResult(read.Content.RemoveBegin(8).ToBoolArray().SelectBegin(length));
		}
		return OperateResult.CreateSuccessResult(read.Content.RemoveBegin(5).ToBoolArray().SelectBegin(length));
	}

	/// <summary>
	/// 写入单个的数据，返回是否成功
	/// Write a single  data, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public static async Task<OperateResult> WriteAsync(IMemobus memobus, string address, bool value)
	{
		if (address.StartsWith(new string[5] { "M", "G", "I", "O", "S" }))
		{
			return await WriteAsync(memobus, address, new bool[1] { value }).ConfigureAwait(continueOnCapturedContext: false);
		}
		OperateResult<byte[]> command = BuildWriteCommand(memobus.CpuTo, memobus.CpuFrom, address, value);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await memobus.ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 批量写入数组数据，返回是否成功
	/// Batch write  array data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public static async Task<OperateResult> WriteAsync(IMemobus memobus, string address, bool[] value)
	{
		OperateResult<byte[]> command = BuildWriteCommand(memobus.CpuTo, memobus.CpuFrom, address, value);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await memobus.ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组
	/// Batch read byte array information, need to specify the address and length, return the original byte array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public static async Task<OperateResult<byte[]>> ReadAsync(IMemobus memobus, string address, ushort length)
	{
		byte mfc = (byte)HslHelper.ExtractParameter(ref address, "mfc", 32);
		byte sfc = (byte)HslHelper.ExtractParameter(ref address, "x", 3);
		OperateResult<byte[]> command = BuildReadCommand(mfc, sfc, memobus.CpuTo, memobus.CpuFrom, address, length);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await memobus.ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return ExtraContent(address, read.Content);
	}

	/// <summary>
	/// 写入原始的byte数组数据到指定的地址，返回是否写入成功
	/// Write the original byte array data to the specified address, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public static async Task<OperateResult> WriteAsync(IMemobus memobus, string address, byte[] value)
	{
		OperateResult<byte[]> command = BuildWriteCommand(memobus.CpuTo, memobus.CpuFrom, address, value);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await memobus.ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 写入short数据，返回是否成功
	/// Write short data, returns whether success
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public static async Task<OperateResult> WriteAsync(IMemobus memobus, string address, short value, Func<string, short, Task<OperateResult>> writeShort)
	{
		byte mfc = (byte)HslHelper.ExtractParameter(ref address, "mfc", 32);
		byte sfc = (byte)HslHelper.ExtractParameter(ref address, "x", 6);
		if (sfc == 11 || sfc == 9)
		{
			return await writeShort($"x={sfc};{address}", value);
		}
		if (sfc == 3)
		{
			sfc = 6;
		}
		OperateResult<byte[]> command = BuildWriteCommand(mfc, sfc, memobus.CpuTo, memobus.CpuFrom, address, value);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await memobus.ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 写入ushort数据，返回是否成功
	/// Write ushort data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public static async Task<OperateResult> WriteAsync(IMemobus memobus, string address, ushort value, Func<string, ushort, Task<OperateResult>> writeUShort)
	{
		byte mfc = (byte)HslHelper.ExtractParameter(ref address, "mfc", 32);
		byte sfc = (byte)HslHelper.ExtractParameter(ref address, "x", 6);
		if (sfc == 11 || sfc == 9)
		{
			return await writeUShort($"x={sfc};{address}", value);
		}
		if (sfc == 3)
		{
			sfc = 6;
		}
		OperateResult<byte[]> command = BuildWriteCommand(mfc, sfc, memobus.CpuTo, memobus.CpuFrom, address, value);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await memobus.ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 随机读取扩展的保持寄存器的内容，也即读取不连续地址的字数据，可以指定多个地址，然后一次性读取所有的数据，然后解析出实际的数据
	/// Randomly read the contents of the extended hold register, that is, read word data of discontinuous addresses,
	/// you can specify multiple addresses, then read all the data at once, and then parse out the actual data
	/// </summary>
	/// <param name="memobus">PLC通信对象</param>
	/// <param name="address">地址信息</param>
	/// <returns>读取的原始字节结果信息</returns>
	/// <remarks>
	/// 注意，本方法只能针对扩展的保持寄存器进行读取
	/// </remarks>
	public static async Task<OperateResult<byte[]>> ReadRandomAsync(IMemobus memobus, ushort[] address)
	{
		OperateResult<byte[]> command = BuildReadRandomCommand(32, 13, memobus.CpuTo, memobus.CpuFrom, address);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await memobus.ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return OperateResult.CreateSuccessResult(SoftBasic.BytesReverseByWord(read.Content.RemoveBegin(8)));
	}

	/// <summary>
	/// 随机读取扩展的保持寄存器的内容，也即读取不连续地址的字数据，可以指定多个地址，然后一次性读取所有的数据，然后解析出实际的数据
	/// Randomly read the contents of the extended hold register, that is, read word data of discontinuous addresses,
	/// you can specify multiple addresses, then read all the data at once, and then parse out the actual data
	/// </summary>
	/// <param name="memobus">PLC通信对象</param>
	/// <param name="address">地址信息</param>
	/// <returns>读取的原始字节结果信息</returns>
	/// <remarks>
	/// 注意，本方法只能针对扩展的保持寄存器进行读取
	/// </remarks>
	public static async Task<OperateResult<byte[]>> ReadRandomAsync(IMemobus memobus, string[] address)
	{
		OperateResult<byte[]> command = BuildReadRandomCommand(memobus.CpuTo, memobus.CpuFrom, address);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await memobus.ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return OperateResult.CreateSuccessResult(SoftBasic.BytesReverseByWord(read.Content.RemoveBegin(8)));
	}

	/// <summary>
	/// 随机写入扩展的保持寄存器的内容，也即写入不连续的地址的字数据，字节数组的长度必须为地址数组长度的两倍，才能正确写入。
	/// Write the contents of the extended hold registers randomly, that is, write word data for discontinuous addresses,
	/// and the byte array must be twice the length of the address array to be written correctly.
	/// </summary>
	/// <param name="memobus">PLC通信对象</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">数据信息</param>
	/// <returns>是否写入成功的结果对象</returns>
	/// <remarks>
	/// 注意，本方法只能针对扩展的保持寄存器进行读取
	/// </remarks>
	public static async Task<OperateResult> WriteRandomAsync(IMemobus memobus, ushort[] address, byte[] value)
	{
		OperateResult<byte[]> command = BuildWriteRandomCommand(32, 14, memobus.CpuTo, memobus.CpuFrom, address, value);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await memobus.ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return OperateResult.CreateSuccessResult();
	}
}
