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
using HslCommunication.Core.Address;

namespace HslCommunication.Profinet.GE;

/// <summary>
/// GE plc相关的辅助类对象
/// </summary>
public class GeHelper
{
	/// <summary>
	/// 构建一个读取数据的报文信息，需要指定操作的数据代码，读取的参数信息
	/// To construct a message information for reading data, you need to specify the data code of the operation and the parameter information to be read
	/// </summary>
	/// <param name="code">操作代码</param>
	/// <param name="data">数据参数</param>
	/// <returns>包含是否成功的报文信息</returns>
	public static OperateResult<byte[]> BuildReadCoreCommand(byte code, byte[] data)
	{
		byte[] buffer = new byte[56]
		{
			2, 0, 0, 0, 0, 0, 0, 0, 0, 1,
			0, 0, 0, 0, 0, 0, 0, 1, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			6, 192, 0, 0, 0, 0, 16, 14, 0, 0,
			1, 1, code, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0
		};
		data.CopyTo(buffer, 43);
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 构建一个读取数据的报文命令，需要指定消息号，读取的 GE 地址信息
	/// To construct a message command to read data, you need to specify the message number and read GE address information
	/// </summary>
	/// <param name="address">GE 的地址</param>
	/// <returns>包含是否成功的报文信息</returns>
	public static OperateResult<byte[]> BuildReadCommand(GeSRTPAddress address)
	{
		if (address.DataCode == 10 || address.DataCode == 12 || address.DataCode == 8)
		{
			address.Length /= 2;
		}
		return BuildReadCoreCommand(4, new byte[5]
		{
			address.DataCode,
			BitConverter.GetBytes(address.AddressStart)[0],
			BitConverter.GetBytes(address.AddressStart)[1],
			BitConverter.GetBytes(address.Length)[0],
			BitConverter.GetBytes(address.Length)[1]
		});
	}

	/// <summary>
	/// 构建一个读取数据的报文命令，需要指定消息号，地址，长度，是否位读取，返回完整的报文信息。
	/// To construct a message command to read data, you need to specify the message number,
	/// address, length, whether to read in bits, and return the complete message information.
	/// </summary>
	/// <param name="address">地址</param>
	/// <param name="length">读取的长度</param>
	/// <returns>包含是否成功的报文对象</returns>
	public static OperateResult<byte[]> BuildReadCommand(string address, ushort length, bool isBit)
	{
		OperateResult<GeSRTPAddress> operateResult = GeSRTPAddress.ParseFrom(address, length, isBit);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return BuildReadCommand(operateResult.Content);
	}

	/// <summary>
	/// 构建一个批量写入 byte 数组变量的报文，需要指定消息号，写入的地址，地址参照  说明。
	/// To construct a message to be written into byte array variables in batches,
	/// you need to specify the message number and write address. For the address, refer to the description of .
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <param name="value">byte数组的原始数据</param>
	/// <returns>包含结果信息的报文内容</returns>
	public static OperateResult<byte[]> BuildWriteCommand(GeSRTPAddress address, byte[] value)
	{
		int count = address.Length;
		if (address.DataCode == 10 || address.DataCode == 12 || address.DataCode == 8)
		{
			count /= 2;
		}
		byte[] buffer = new byte[56 + value.Length];
		buffer[0] = 2;
		buffer[1] = 0;
		buffer[2] = 0;
		buffer[3] = 0;
		buffer[4] = BitConverter.GetBytes(value.Length)[0];
		buffer[5] = BitConverter.GetBytes(value.Length)[1];
		buffer[9] = 2;
		buffer[17] = 2;
		buffer[18] = 0;
		buffer[30] = 9;
		buffer[31] = 128;
		buffer[36] = 16;
		buffer[37] = 14;
		buffer[40] = 1;
		buffer[41] = 1;
		buffer[42] = 2;
		buffer[48] = 1;
		buffer[49] = 1;
		buffer[50] = 7;
		buffer[51] = address.DataCode;
		buffer[52] = BitConverter.GetBytes(address.AddressStart)[0];
		buffer[53] = BitConverter.GetBytes(address.AddressStart)[1];
		buffer[54] = BitConverter.GetBytes(count)[0];
		buffer[55] = BitConverter.GetBytes(count)[1];
		value.CopyTo(buffer, 56);
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 构建一个批量写入 byte 数组变量的报文，需要指定消息号，写入的地址，地址参照  说明。
	/// To construct a message to be written into byte array variables in batches,
	/// you need to specify the message number and write address. For the address, refer to the description of .
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <param name="value">byte数组的原始数据</param>
	/// <returns>包含结果信息的报文内容</returns>
	public static OperateResult<byte[]> BuildWriteCommand(string address, byte[] value)
	{
		OperateResult<GeSRTPAddress> operateResult = GeSRTPAddress.ParseFrom(address, (ushort)value.Length, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return BuildWriteCommand(operateResult.Content, value);
	}

	/// <summary>
	/// 构建一个批量写入 bool 数组变量的报文，需要指定消息号，写入的地址，地址参照  说明。
	/// To construct a message to be written into bool array variables in batches,
	/// you need to specify the message number and write address. For the address, refer to the description of .
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <param name="value">bool数组</param>
	/// <returns>包含结果信息的报文内容</returns>
	public static OperateResult<byte[]> BuildWriteCommand(string address, bool[] value)
	{
		OperateResult<GeSRTPAddress> operateResult = GeSRTPAddress.ParseFrom(address, (ushort)value.Length, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		bool[] array = new bool[operateResult.Content.AddressStart % 8 + value.Length];
		value.CopyTo(array, operateResult.Content.AddressStart % 8);
		return BuildWriteCommand(operateResult.Content, SoftBasic.BoolArrayToByte(array));
	}

	/// <summary>
	/// 从PLC返回的数据中，提取出实际的数据内容，最少6个字节的数据。超出实际的数据长度的部分没有任何意义。
	/// From the data returned by the PLC, extract the actual data content, at least 6 bytes of data. The part beyond the actual data length has no meaning.
	/// </summary>
	/// <param name="content">PLC返回的数据信息</param>
	/// <returns>解析后的实际数据内容</returns>
	public static OperateResult<byte[]> ExtraResponseContent(byte[] content)
	{
		try
		{
			if (content[0] == 1)
			{
				return OperateResult.CreateSuccessResult(content);
			}
			if (content[0] != 3)
			{
				return new OperateResult<byte[]>(content[0], StringResources.Language.UnknownError + " Source:" + content.ToHexString(' '));
			}
			if (content[31] == 212)
			{
				ushort num = BitConverter.ToUInt16(content, 42);
				if (num != 0)
				{
					return new OperateResult<byte[]>(num, StringResources.Language.UnknownError);
				}
				return OperateResult.CreateSuccessResult(content.SelectMiddle(44, 6));
			}
			if (content[31] == 148)
			{
				return OperateResult.CreateSuccessResult(content.RemoveBegin(56));
			}
			return new OperateResult<byte[]>("Extra Wrong:" + StringResources.Language.UnknownError + " Source:" + content.ToHexString(' '));
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("Extra Wrong:" + ex.Message + " Source:" + content.ToHexString(' '));
		}
	}

	/// <summary>
	/// 从实际的时间的字节数组里解析出C#格式的时间对象，这个时间可能是时区0的时间，需要自行转化本地时间。
	/// Analyze the time object in C# format from the actual time byte array.
	/// This time may be the time in time zone 0, and you need to convert the local time yourself.
	/// </summary>
	/// <param name="content">字节数组</param>
	/// <returns>包含是否成功的结果对象</returns>
	public static OperateResult<DateTime> ExtraDateTime(byte[] content)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<DateTime>(StringResources.Language.InsufficientPrivileges);
		}
		try
		{
			return OperateResult.CreateSuccessResult(new DateTime(int.Parse(content[5].ToString("X2")) + 2000, int.Parse(content[4].ToString("X2")), int.Parse(content[3].ToString("X2")), int.Parse(content[2].ToString("X2")), int.Parse(content[1].ToString("X2")), int.Parse(content[0].ToString("X2"))));
		}
		catch (Exception ex)
		{
			return new OperateResult<DateTime>(ex.Message + " Source:" + content.ToHexString(' '));
		}
	}

	/// <summary>
	/// 从实际的时间的字节数组里解析出PLC的程序的名称。
	/// Parse the name of the PLC program from the actual time byte array
	/// </summary>
	/// <param name="content">字节数组</param>
	/// <returns>包含是否成功的结果对象</returns>
	public static OperateResult<string> ExtraProgramName(byte[] content)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<string>(StringResources.Language.InsufficientPrivileges);
		}
		try
		{
			return OperateResult.CreateSuccessResult(Encoding.UTF8.GetString(content, 18, 16).Trim('\0'));
		}
		catch (Exception ex)
		{
			return new OperateResult<string>(ex.Message + " Source:" + content.ToHexString(' '));
		}
	}
}
