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

namespace HslCommunication.Profinet.Cimon.Helper;

/// <summary>
/// 辅助帮助类对象
/// </summary>
public class CimonHelper
{
	/// <summary>
	/// 所有支持的地址信息
	/// </summary>
	public const string AddressTypes = "YMLKFTCDSX";

	/// <summary>
	/// 根据错误码获取错误文本信息
	/// </summary>
	/// <param name="error">错误信息</param>
	/// <returns>错误消息</returns>
	public static string GetErrorText(int error)
	{
		return error switch
		{
			0 => "No Error", 
			1 => "Error in system (No link with CPU)", 
			2 => "Invalid Device Prefix", 
			3 => "Invalid Device Address", 
			4 => "UDP_ERR_READ_DATASIZE (Error in requested data size)", 
			5 => "UDP_ERR_BLOCK_SIZE (Over 16 requested blocks)", 
			6 => "The case that buffer memory send an error in data and size", 
			7 => "Over receiving buffer capacity", 
			8 => "Over sending time", 
			9 => "UDP_ERR_INVALID_HEADER (Error in header)", 
			10 => "Error in Check-Sum (Check-Sum of received data)", 
			11 => "Error in the information on Frame Length (Total received frame size)", 
			12 => "UDP_ERR_WRITE_DATASIZE (Error in the size to write)", 
			13 => "Unknown Bit Value (Error in Bit Write Data)", 
			14 => "Unknown Command", 
			15 => "Disabling state from writing", 
			16 => "Error in CPU process", 
			_ => StringResources.Language.UnknownError, 
		};
	}

	/// <summary>
	/// 打包一个返回错误信息的报文
	/// </summary>
	public static byte[] PackErrorResponse(byte frame, int err)
	{
		return PackEntireCommand(response: true, frame, 65, new byte[4]
		{
			0,
			0,
			BitConverter.GetBytes(err)[1],
			BitConverter.GetBytes(err)[0]
		});
	}

	/// <summary>
	/// 打包完整的命令信息
	/// </summary>
	/// <param name="response">是否响应的报文</param>
	/// <param name="frame">站号信息</param>
	/// <param name="cmd">命令码</param>
	/// <param name="data">返回的数据信息</param>
	/// <returns>完成的报文</returns>
	public static byte[] PackEntireCommand(bool response, byte frame, byte cmd, byte[] data)
	{
		MemoryStream memoryStream = new MemoryStream();
		if (response)
		{
			memoryStream.Write(Encoding.ASCII.GetBytes("KDT_PLC_S"));
		}
		else
		{
			memoryStream.Write(Encoding.ASCII.GetBytes("KDT_PLC_M"));
		}
		if (response)
		{
			memoryStream.WriteByte((byte)(frame + 128));
		}
		else
		{
			memoryStream.WriteByte(frame);
		}
		memoryStream.WriteByte(cmd);
		memoryStream.WriteByte(0);
		if (cmd == 82 || cmd == 114)
		{
			if (response)
			{
				memoryStream.WriteByte(BitConverter.GetBytes(data.Length)[1]);
				memoryStream.WriteByte(BitConverter.GetBytes(data.Length)[0]);
			}
			else
			{
				memoryStream.WriteByte(BitConverter.GetBytes(data.Length)[0]);
			}
		}
		else
		{
			memoryStream.WriteByte(BitConverter.GetBytes(data.Length)[1]);
			memoryStream.WriteByte(BitConverter.GetBytes(data.Length)[0]);
		}
		if (data.Length != 0)
		{
			memoryStream.Write(data);
		}
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		byte[] array = memoryStream.ToArray();
		int num = 0;
		for (int i = 0; i < array.Length - 2; i++)
		{
			num += array[i];
		}
		array[^2] = BitConverter.GetBytes(num)[1];
		array[^1] = BitConverter.GetBytes(num)[0];
		return array;
	}

	internal static OperateResult<byte[]> PackReadWriteCommand(byte frameNo, byte control, string address, int length, byte[] data)
	{
		if (string.IsNullOrEmpty(address))
		{
			return new OperateResult<byte[]>("Address is null");
		}
		string text = address.Substring(0, 1);
		if (!"YMLKFTCDSX".Contains(text))
		{
			return new OperateResult<byte[]>("Address type error: " + text);
		}
		List<byte> list = new List<byte>();
		list.AddRange(Encoding.ASCII.GetBytes(text.ToUpper()));
		list.Add(0);
		list.AddRange(Encoding.ASCII.GetBytes(address.Substring(1).PadLeft(6, '0')));
		list.Add(BitConverter.GetBytes(length)[1]);
		list.Add(BitConverter.GetBytes(length)[0]);
		if (data != null && data.Length != 0)
		{
			list.AddRange(data);
		}
		return OperateResult.CreateSuccessResult(PackEntireCommand(response: false, frameNo, control, list.ToArray()));
	}

	/// <summary>
	/// 解析出实际的数据信息
	/// </summary>
	/// <param name="response">PLC返回的报文数据</param>
	/// <returns>提炼的真实的数据</returns>
	public static OperateResult<byte[]> ExtractActualData(byte[] response)
	{
		if (response.Length < 20)
		{
			return new OperateResult<byte[]>("Length is less than 20:" + SoftBasic.ByteToHexString(response));
		}
		try
		{
			if (response[10] == 65)
			{
				int num = response[14] * 256 + response[15];
				return new OperateResult<byte[]>(num, GetErrorText(num) ?? "");
			}
			if (response[10] == 87 || response[10] == 119)
			{
				return OperateResult.CreateSuccessResult(new byte[0]);
			}
			if (response[10] == 82)
			{
				return OperateResult.CreateSuccessResult(response.RemoveDouble(23, 2));
			}
			if (response[10] == 114)
			{
				int num2 = response[12] * 256 + response[13];
				return OperateResult.CreateSuccessResult(response.SelectMiddle(23, num2 - 9));
			}
			return new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("ExtractActualData failed: " + ex.Message + " Souce: " + response.ToHexString(' '));
		}
	}
}
