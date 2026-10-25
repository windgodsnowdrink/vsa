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
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Address;

namespace HslCommunication.Profinet.Vigor.Helper;

/// <summary>
/// 丰炜PLC的辅助类对象
/// </summary>
public class VigorVsHelper
{
	internal static byte[] PackCommand(byte[] command, byte code = 2)
	{
		if (command == null)
		{
			command = new byte[0];
		}
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.WriteByte(16);
		memoryStream.WriteByte(code);
		int num = 0;
		for (int i = 0; i < command.Length; i++)
		{
			num += command[i];
			memoryStream.WriteByte(command[i]);
			if (command[i] == 16)
			{
				memoryStream.WriteByte(command[i]);
			}
		}
		memoryStream.WriteByte(16);
		memoryStream.WriteByte(3);
		byte[] bytes = Encoding.ASCII.GetBytes((num % 256).ToString("X2"));
		memoryStream.WriteByte(bytes[0]);
		memoryStream.WriteByte(bytes[1]);
		return memoryStream.ToArray();
	}

	internal static byte[] UnPackCommand(byte[] command)
	{
		if (command == null)
		{
			command = new byte[0];
		}
		MemoryStream memoryStream = new MemoryStream();
		for (int i = 0; i < command.Length; i++)
		{
			memoryStream.WriteByte(command[i]);
			if (command[i] == 16 && i + 1 < command.Length && command[i + 1] == 16)
			{
				i++;
			}
		}
		return memoryStream.ToArray();
	}

	internal static bool CheckReceiveDataComplete(byte[] buffer, int length)
	{
		int num = 0;
		if (length < 10)
		{
			return false;
		}
		for (int i = 0; i < length; i++)
		{
			if (buffer[i] == 16 && i + 1 < length)
			{
				if (buffer[i + 1] == 16)
				{
					i++;
				}
				else if (buffer[i + 1] == 3)
				{
					num = i;
					break;
				}
			}
		}
		if (num == length - 4)
		{
			return true;
		}
		return false;
	}

	private static byte[] GetBytesAddress(int address)
	{
		string text = address.ToString("D6");
		if (text.Length > 6)
		{
			text = text.Substring(6);
		}
		return text.ToHexBytes();
	}

	/// <summary>
	/// 构建读取的报文命令，对于字地址，单次最多读取64字节，支持D,SD,R,T,C的数据读取，对于位地址，最多读取1024位，支持X,Y,M,SM,S,TS(定时器触点),TC（定时器线圈）,CS(计数器触点),CC（计数器线圈）
	/// Construct a read message command. For word addresses, up to 64 bytes can be read at a time, and data reading of D, SD, R, T, and C is supported. For bit addresses,
	/// up to 1024 bits are read, and X, Y are supported. , M, SM, S, TS (timer contact), TC (timer coil), CS (counter contact), CC (counter coil)
	/// </summary>
	/// <param name="station">站号信息</param>
	/// <param name="address">PLC的数据地址</param>
	/// <param name="length">读取的长度</param>
	/// <param name="isBool">是否进行位读取</param>
	/// <returns>完整的读取的报文信息</returns>
	public static OperateResult<List<byte[]>> BuildReadCommand(byte station, string address, ushort length, bool isBool)
	{
		station = (byte)HslHelper.ExtractParameter(ref address, "s", station);
		OperateResult<VigorAddress> operateResult = VigorAddress.ParseFrom(address, length, isBool);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<List<byte[]>>(operateResult);
		}
		int[] array = SoftBasic.SplitIntegerToArray(length, isBool ? 1024 : ((operateResult.Content.DataCode == 173) ? 32 : 64));
		List<byte[]> list = new List<byte[]>();
		for (int i = 0; i < array.Length; i++)
		{
			byte[] bytesAddress = GetBytesAddress(operateResult.Content.AddressStart);
			list.Add(PackCommand(new byte[10]
			{
				station,
				7,
				0,
				(byte)(isBool ? 33 : 32),
				operateResult.Content.DataCode,
				bytesAddress[2],
				bytesAddress[1],
				bytesAddress[0],
				BitConverter.GetBytes(array[i])[0],
				BitConverter.GetBytes(array[i])[1]
			}, 2));
			operateResult.Content.AddressStart += array[i];
		}
		return OperateResult.CreateSuccessResult(list);
	}

	/// <summary>
	/// 构建以字单位写入的报文，单次最多写入64个word，地址支持 D,SD,R,T,C，对于C200~C255,是属于32位的计数器
	/// Construct a message written in word units, and write up to 64 words in a single time. The address supports D, SD, R, T, C. For C200~C255, it is a 32-bit counter
	/// </summary>
	/// <param name="station">站号信息</param>
	/// <param name="address">PLC的地址</param>
	/// <param name="value">写入的原始数据</param>
	/// <returns>写入命令的完整报文</returns>
	public static OperateResult<byte[]> BuildWriteWordCommand(byte station, string address, byte[] value)
	{
		station = (byte)HslHelper.ExtractParameter(ref address, "s", station);
		OperateResult<VigorAddress> operateResult = VigorAddress.ParseFrom(address, 1, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		byte[] bytesAddress = GetBytesAddress(operateResult.Content.AddressStart);
		byte[] buffer = new byte[10 + value.Length];
		buffer[0] = station;
		buffer[1] = BitConverter.GetBytes(7 + value.Length)[0];
		buffer[2] = BitConverter.GetBytes(7 + value.Length)[1];
		buffer[3] = 40;
		buffer[4] = operateResult.Content.DataCode;
		buffer[5] = bytesAddress[2];
		buffer[6] = bytesAddress[1];
		buffer[7] = bytesAddress[0];
		if (operateResult.Content.DataCode == 173)
		{
			buffer[8] = BitConverter.GetBytes(value.Length / 4)[0];
			buffer[9] = BitConverter.GetBytes(value.Length / 4)[1];
		}
		else
		{
			buffer[8] = BitConverter.GetBytes(value.Length / 2)[0];
			buffer[9] = BitConverter.GetBytes(value.Length / 2)[1];
		}
		value.CopyTo(buffer, 10);
		return OperateResult.CreateSuccessResult(PackCommand(buffer, 2));
	}

	/// <summary>
	/// 构建以位单位写入的报文，单次最多写入1024bit，支持X,Y,M,SM,S,TS(定时器触点),TC（定时器线圈）,CS(计数器触点),CC（计数器线圈）
	/// </summary>
	/// <param name="station">站号信息</param>
	/// <param name="address">PLC的地址</param>
	/// <param name="value">等待写入的bool数组</param>
	/// <returns>写入位数据的完整报文信息</returns>
	public static OperateResult<byte[]> BuildWriteBoolCommand(byte station, string address, bool[] value)
	{
		station = (byte)HslHelper.ExtractParameter(ref address, "s", station);
		OperateResult<VigorAddress> operateResult = VigorAddress.ParseFrom(address, 1, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		byte[] bytesAddress = GetBytesAddress(operateResult.Content.AddressStart);
		byte[] array = value.ToByteArray();
		byte[] buffer = new byte[10 + array.Length];
		buffer[0] = station;
		buffer[1] = BitConverter.GetBytes(7 + array.Length)[0];
		buffer[2] = BitConverter.GetBytes(7 + array.Length)[1];
		buffer[3] = 41;
		buffer[4] = operateResult.Content.DataCode;
		buffer[5] = bytesAddress[2];
		buffer[6] = bytesAddress[1];
		buffer[7] = bytesAddress[0];
		buffer[8] = BitConverter.GetBytes(value.Length)[0];
		buffer[9] = BitConverter.GetBytes(value.Length)[1];
		array.CopyTo(buffer, 10);
		return OperateResult.CreateSuccessResult(PackCommand(buffer, 2));
	}

	/// <summary>
	/// 检查从PLC返回的报文是否正确，以及提取出正确的结果数据
	/// </summary>
	/// <param name="response">PLC返回的报文</param>
	/// <returns>提取的结果数据内容</returns>
	public static OperateResult<byte[]> CheckResponseContent(byte[] response)
	{
		response = UnPackCommand(response);
		if (response.Length < 6)
		{
			return new OperateResult<byte[]>(StringResources.Language.ReceiveDataLengthTooShort + " Source: " + response.ToHexString(' '));
		}
		if (response[5] != 0)
		{
			return new OperateResult<byte[]>(response[5], GetErrorText(response[5]) + " Source: " + response.ToHexString(' '));
		}
		try
		{
			int num = BitConverter.ToUInt16(response, 3);
			if (num + 9 == response.Length)
			{
				if (num == 1)
				{
					return OperateResult.CreateSuccessResult(new byte[0]);
				}
				return OperateResult.CreateSuccessResult(response.SelectMiddle(6, num - 1));
			}
			return new OperateResult<byte[]>(response[5], "Length check failed, Source: " + response.ToHexString(' '));
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("CheckResponseContent failed: " + ex.Message + Environment.NewLine + "Source: " + response.ToHexString(' '));
		}
	}

	internal static string GetErrorText(byte status)
	{
		return status switch
		{
			2 => StringResources.Language.Vigor02, 
			4 => StringResources.Language.Vigor04, 
			6 => StringResources.Language.Vigor06, 
			8 => StringResources.Language.Vigor08, 
			49 => StringResources.Language.Vigor31, 
			_ => StringResources.Language.UnknownError, 
		};
	}
}
