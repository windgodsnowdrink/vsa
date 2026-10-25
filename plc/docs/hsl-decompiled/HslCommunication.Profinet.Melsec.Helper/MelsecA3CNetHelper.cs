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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Core.Net;

namespace HslCommunication.Profinet.Melsec.Helper;

/// <summary>
/// MelsecA3CNet1协议通信的辅助类
/// </summary>
public class MelsecA3CNetHelper
{
	private static byte[] CalculatSum(byte[] command, int startLength, int endLength)
	{
		int num = 0;
		for (int i = startLength; i < command.Length - endLength; i++)
		{
			num += command[i];
		}
		return SoftBasic.BuildAsciiBytesFrom((byte)num);
	}

	/// <summary>
	/// 设置校验和的信息
	/// </summary>
	/// <param name="command">命令</param>
	/// <param name="startLength">开始字节索引</param>
	/// <param name="endLength">结束字节索引</param>
	/// <param name="skip">跳过字节数</param>
	public static void SetSumCheck(byte[] command, int startLength, int endLength, int skip = 0)
	{
		byte[] array = CalculatSum(command, startLength, endLength);
		command[command.Length - endLength + skip] = array[0];
		command[command.Length - endLength + 1 + skip] = array[1];
	}

	/// <summary>
	/// 将命令进行打包传送，可选站号及是否和校验机制
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <param name="mcCommand">mc协议的命令</param>
	/// <param name="station">PLC的站号</param>
	/// <returns>最终的原始报文信息</returns>
	public static byte[] PackCommand(IReadWriteA3C plc, byte[] mcCommand, byte station = 0)
	{
		if (plc.Format == 5)
		{
			byte[] array = (plc.SumCheck ? new byte[16 + mcCommand.Length] : new byte[14 + mcCommand.Length]);
			array[0] = 16;
			array[1] = 2;
			array[2] = BitConverter.GetBytes(8 + mcCommand.Length)[0];
			array[3] = BitConverter.GetBytes(8 + mcCommand.Length)[1];
			array[4] = 248;
			array[5] = station;
			array[6] = 0;
			array[7] = byte.MaxValue;
			array[8] = byte.MaxValue;
			array[9] = 3;
			array[10] = 0;
			array[11] = 0;
			mcCommand.CopyTo(array, 12);
			array[12 + mcCommand.Length] = 16;
			array[13 + mcCommand.Length] = 3;
			if (plc.SumCheck)
			{
				SetSumCheck(array, 2, 4, 2);
			}
			return array;
		}
		MemoryStream memoryStream = new MemoryStream();
		if (plc.Format != 3)
		{
			memoryStream.WriteByte(5);
		}
		else
		{
			memoryStream.WriteByte(2);
		}
		if (plc.Format == 2)
		{
			memoryStream.WriteByte(48);
			memoryStream.WriteByte(48);
		}
		memoryStream.WriteByte(70);
		if (plc.McMode == 0)
		{
			memoryStream.WriteByte(57);
		}
		else
		{
			memoryStream.WriteByte(56);
		}
		memoryStream.WriteByte(SoftBasic.BuildAsciiBytesFrom(station)[0]);
		memoryStream.WriteByte(SoftBasic.BuildAsciiBytesFrom(station)[1]);
		memoryStream.WriteByte(48);
		memoryStream.WriteByte(48);
		memoryStream.WriteByte(70);
		memoryStream.WriteByte(70);
		if (plc.McMode == 1)
		{
			memoryStream.WriteByte(48);
			memoryStream.WriteByte(51);
			memoryStream.WriteByte(70);
			memoryStream.WriteByte(70);
			memoryStream.WriteByte(48);
			memoryStream.WriteByte(48);
		}
		memoryStream.WriteByte(48);
		memoryStream.WriteByte(48);
		memoryStream.Write(mcCommand, 0, mcCommand.Length);
		if (plc.Format == 3)
		{
			memoryStream.WriteByte(3);
		}
		if (plc.SumCheck)
		{
			byte[] command = memoryStream.ToArray();
			memoryStream.Write(CalculatSum(command, 1, 0));
		}
		if (plc.Format == 4)
		{
			memoryStream.WriteByte(13);
			memoryStream.WriteByte(10);
		}
		byte[] result = memoryStream.ToArray();
		memoryStream.Dispose();
		return result;
	}

	private static int GetErrorCodeOrDataStartIndex(IReadWriteA3C plc)
	{
		int result = 11;
		if (plc.McMode == 0)
		{
			switch (plc.Format)
			{
			case 1:
				result = 11;
				break;
			case 2:
				result = 13;
				break;
			case 3:
				result = 15;
				break;
			case 4:
				result = 11;
				break;
			}
		}
		else
		{
			switch (plc.Format)
			{
			case 1:
				result = 17;
				break;
			case 2:
				result = 19;
				break;
			case 3:
				result = 21;
				break;
			case 4:
				result = 17;
				break;
			case 5:
				result = 16;
				break;
			}
		}
		return result;
	}

	/// <summary>
	/// 根据PLC返回的数据信息，获取到实际的数据内容
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <param name="response">PLC返回的数据信息</param>
	/// <returns>带有是否成功的读取结果对象内容</returns>
	public static OperateResult<byte[]> ExtraReadActualResponse(IReadWriteA3C plc, byte[] response)
	{
		try
		{
			int errorCodeOrDataStartIndex = GetErrorCodeOrDataStartIndex(plc);
			if (plc.Format == 1 || plc.Format == 2 || plc.Format == 4)
			{
				if (response[0] == 21)
				{
					int num = Convert.ToInt32(Encoding.ASCII.GetString(response, errorCodeOrDataStartIndex, 4), 16);
					return new OperateResult<byte[]>(num, MelsecHelper.GetErrorDescription(num));
				}
				if (response[0] != 2)
				{
					return new OperateResult<byte[]>(response[0], "Read Faild:" + SoftBasic.GetAsciiStringRender(response));
				}
			}
			else if (plc.Format == 3)
			{
				string message = Encoding.ASCII.GetString(response, (plc.McMode == 0) ? 11 : 17, 4);
				if (message == "QNAK")
				{
					int num2 = Convert.ToInt32(Encoding.ASCII.GetString(response, errorCodeOrDataStartIndex, 4), 16);
					return new OperateResult<byte[]>(num2, MelsecHelper.GetErrorDescription(num2));
				}
				if (message != "QACK")
				{
					return new OperateResult<byte[]>(response[0], "Read Faild:" + SoftBasic.GetAsciiStringRender(response));
				}
			}
			else if (plc.Format == 5)
			{
				if (response.Length < 16)
				{
					return new OperateResult<byte[]>("Receive too short: " + response.ToHexString(' '));
				}
				int num3 = BitConverter.ToUInt16(response, 14);
				if (num3 > 0)
				{
					return new OperateResult<byte[]>(num3, MelsecHelper.GetErrorDescription(num3));
				}
			}
			int num4 = -1;
			if (plc.Format == 5)
			{
				num4 = 4 + BitConverter.ToUInt16(response, 2);
			}
			else
			{
				for (int i = errorCodeOrDataStartIndex; i < response.Length; i++)
				{
					if (response[i] == 3)
					{
						num4 = i;
						break;
					}
				}
			}
			if (num4 == -1)
			{
				num4 = response.Length;
			}
			return OperateResult.CreateSuccessResult(response.SelectMiddle(errorCodeOrDataStartIndex, num4 - errorCodeOrDataStartIndex));
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("ExtraReadActualResponse Wrong:" + ex.Message + Environment.NewLine + "Source: " + response.ToHexString(' '));
		}
	}

	private static OperateResult CheckWriteResponse(IReadWriteA3C plc, byte[] response)
	{
		int errorCodeOrDataStartIndex = GetErrorCodeOrDataStartIndex(plc);
		try
		{
			if (plc.Format == 1 || plc.Format == 2)
			{
				if (response[0] == 21)
				{
					int num = Convert.ToInt32(Encoding.ASCII.GetString(response, errorCodeOrDataStartIndex, 4), 16);
					return new OperateResult<byte[]>(num, MelsecHelper.GetErrorDescription(num));
				}
				if (response[0] != 6)
				{
					return new OperateResult<byte[]>(response[0], "Write Faild:" + SoftBasic.GetAsciiStringRender(response));
				}
			}
			else if (plc.Format == 3)
			{
				if (response[0] != 2)
				{
					return new OperateResult<byte[]>(response[0], "Write Faild:" + SoftBasic.GetAsciiStringRender(response));
				}
				string message = Encoding.ASCII.GetString(response, (plc.McMode == 0) ? 11 : 17, 4);
				if (message == "QNAK")
				{
					int num2 = Convert.ToInt32(Encoding.ASCII.GetString(response, errorCodeOrDataStartIndex, 4), 16);
					return new OperateResult<byte[]>(num2, MelsecHelper.GetErrorDescription(num2));
				}
				if (message != "QACK")
				{
					return new OperateResult<byte[]>(response[0], "Write Faild:" + SoftBasic.GetAsciiStringRender(response));
				}
			}
			else if (plc.Format == 4)
			{
				if (response[0] == 21)
				{
					int num3 = Convert.ToInt32(Encoding.ASCII.GetString(response, errorCodeOrDataStartIndex, 4), 16);
					return new OperateResult<byte[]>(num3, MelsecHelper.GetErrorDescription(num3));
				}
				if (response[0] != 6)
				{
					return new OperateResult<byte[]>(response[0], "Write Faild:" + SoftBasic.GetAsciiStringRender(response));
				}
			}
			else if (plc.Format == 5)
			{
				if (response.Length < 16)
				{
					return new OperateResult<byte[]>("Receive too short: " + response.ToHexString(' '));
				}
				int num4 = BitConverter.ToUInt16(response, 14);
				if (num4 > 0)
				{
					return new OperateResult<byte[]>(num4, MelsecHelper.GetErrorDescription(num4));
				}
			}
			return OperateResult.CreateSuccessResult();
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("CheckWriteResponse failed: " + ex.Message + Environment.NewLine + "Content: " + SoftBasic.GetAsciiStringRender(response));
		}
	}

	/// <summary>
	/// 批量读取PLC的数据，以字为单位，支持读取X,Y,M,S,D,T,C，具体的地址范围需要根据PLC型号来确认
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <param name="address">地址信息</param>
	/// <param name="length">数据长度</param>
	/// <returns>读取结果信息</returns>
	public static OperateResult<byte[]> Read(IReadWriteA3C plc, string address, ushort length)
	{
		byte station = (byte)HslHelper.ExtractParameter(ref address, "s", plc.Station);
		OperateResult<McAddressData> operateResult = McAddressData.ParseMelsecFrom(address, length, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		List<byte> list = new List<byte>();
		ushort num = 0;
		while (num < length)
		{
			ushort num2 = (ushort)Math.Min(length - num, McHelper.GetReadWordLength(McType.MCAscii));
			operateResult.Content.Length = num2;
			byte[] mcCommand = ((plc.Format == 5) ? McBinaryHelper.BuildReadMcCoreCommand(operateResult.Content, isBit: false) : McAsciiHelper.BuildAsciiReadMcCoreCommand(operateResult.Content, isBit: false));
			OperateResult<byte[]> operateResult2 = plc.ReadFromCoreServer(PackCommand(plc, mcCommand, station));
			if (!operateResult2.IsSuccess)
			{
				return operateResult2;
			}
			OperateResult<byte[]> operateResult3 = ExtraReadActualResponse(plc, operateResult2.Content);
			if (!operateResult3.IsSuccess)
			{
				return operateResult3;
			}
			list.AddRange((plc.Format == 5) ? operateResult3.Content : MelsecHelper.TransAsciiByteArrayToByteArray(operateResult3.Content));
			num += num2;
			if (operateResult.Content.McDataType.DataType == 0)
			{
				operateResult.Content.AddressStart += num2;
			}
			else
			{
				operateResult.Content.AddressStart += num2 * 16;
			}
		}
		return OperateResult.CreateSuccessResult(list.ToArray());
	}

	/// <summary>
	/// 批量读取PLC的数据，以字为单位，支持读取X,Y,M,S,D,T,C，具体的地址范围需要根据PLC型号来确认
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <param name="address">地址信息</param>
	/// <param name="length">数据长度</param>
	/// <returns>读取结果信息</returns>
	public static async Task<OperateResult<byte[]>> ReadAsync(IReadWriteA3C plc, string address, ushort length)
	{
		byte stat = (byte)HslHelper.ExtractParameter(ref address, "s", plc.Station);
		OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom(address, length, isBit: false);
		if (!addressResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(addressResult);
		}
		List<byte> bytesContent = new List<byte>();
		ushort alreadyFinished = 0;
		while (alreadyFinished < length)
		{
			ushort readLength = (ushort)Math.Min(length - alreadyFinished, McHelper.GetReadWordLength(McType.MCAscii));
			addressResult.Content.Length = readLength;
			byte[] command = ((plc.Format == 5) ? McBinaryHelper.BuildReadMcCoreCommand(addressResult.Content, isBit: false) : McAsciiHelper.BuildAsciiReadMcCoreCommand(addressResult.Content, isBit: false));
			OperateResult<byte[]> read = await plc.ReadFromCoreServerAsync(PackCommand(plc, command, stat));
			if (!read.IsSuccess)
			{
				return read;
			}
			OperateResult<byte[]> check = ExtraReadActualResponse(plc, read.Content);
			if (!check.IsSuccess)
			{
				return check;
			}
			bytesContent.AddRange((plc.Format == 5) ? check.Content : MelsecHelper.TransAsciiByteArrayToByteArray(check.Content));
			alreadyFinished += readLength;
			if (addressResult.Content.McDataType.DataType == 0)
			{
				addressResult.Content.AddressStart += readLength;
			}
			else
			{
				addressResult.Content.AddressStart += readLength * 16;
			}
		}
		return OperateResult.CreateSuccessResult(bytesContent.ToArray());
	}

	/// <summary>
	/// 批量写入PLC的数据，以字为单位，也就是说最少2个字节信息，支持X,Y,M,S,D,T,C，具体的地址范围需要根据PLC型号来确认
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public static OperateResult Write(IReadWriteA3C plc, string address, byte[] value)
	{
		byte station = (byte)HslHelper.ExtractParameter(ref address, "s", plc.Station);
		OperateResult<McAddressData> operateResult = McAddressData.ParseMelsecFrom(address, 0, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		byte[] mcCommand = ((plc.Format == 5) ? McBinaryHelper.BuildWriteWordCoreCommand(operateResult.Content, value) : McAsciiHelper.BuildAsciiWriteWordCoreCommand(operateResult.Content, value));
		OperateResult<byte[]> operateResult2 = plc.ReadFromCoreServer(PackCommand(plc, mcCommand, station));
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return CheckWriteResponse(plc, operateResult2.Content);
	}

	/// <summary>
	/// 批量写入PLC的数据，以字为单位，也就是说最少2个字节信息，支持X,Y,M,S,D,T,C，具体的地址范围需要根据PLC型号来确认
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public static async Task<OperateResult> WriteAsync(IReadWriteA3C plc, string address, byte[] value)
	{
		byte stat = (byte)HslHelper.ExtractParameter(ref address, "s", plc.Station);
		OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom(address, 0, isBit: false);
		if (!addressResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(addressResult);
		}
		byte[] command = ((plc.Format == 5) ? McBinaryHelper.BuildWriteWordCoreCommand(addressResult.Content, value) : McAsciiHelper.BuildAsciiWriteWordCoreCommand(addressResult.Content, value));
		OperateResult<byte[]> read = await plc.ReadFromCoreServerAsync(PackCommand(plc, command, stat));
		if (!read.IsSuccess)
		{
			return read;
		}
		return CheckWriteResponse(plc, read.Content);
	}

	private static IEnumerable<bool> ParseBoolValue(IReadWriteA3C plc, byte[] content)
	{
		if (plc.Format == 5)
		{
			byte[] source = McBinaryHelper.ExtractActualDataHelper(content, isBit: true);
			return source.Select((byte m) => m != 0);
		}
		return content.Select((byte m) => m == 49);
	}

	/// <summary>
	/// 批量读取bool类型数据，支持的类型为X,Y,S,T,C，具体的地址范围取决于PLC的类型
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <param name="address">地址信息，比如X10,Y17，注意X，Y的地址是8进制的</param>
	/// <param name="length">读取的长度</param>
	/// <returns>读取结果信息</returns>
	public static OperateResult<bool[]> ReadBool(IReadWriteA3C plc, string address, ushort length)
	{
		if (address.IndexOf('.') > 0)
		{
			return HslHelper.ReadBool(plc, address, length);
		}
		byte station = (byte)HslHelper.ExtractParameter(ref address, "s", plc.Station);
		OperateResult<McAddressData> operateResult = McAddressData.ParseMelsecFrom(address, length, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		List<bool> list = new List<bool>();
		ushort num = 0;
		while (num < length)
		{
			ushort num2 = (ushort)Math.Min(length - num, McHelper.GetReadBoolLength(McType.MCAscii));
			operateResult.Content.Length = num2;
			byte[] mcCommand = ((plc.Format == 5) ? McBinaryHelper.BuildReadMcCoreCommand(operateResult.Content, isBit: true) : McAsciiHelper.BuildAsciiReadMcCoreCommand(operateResult.Content, isBit: true));
			OperateResult<byte[]> operateResult2 = plc.ReadFromCoreServer(PackCommand(plc, mcCommand, station));
			if (!operateResult2.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(operateResult2);
			}
			OperateResult<byte[]> operateResult3 = ExtraReadActualResponse(plc, operateResult2.Content);
			if (!operateResult3.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(operateResult3);
			}
			list.AddRange(ParseBoolValue(plc, operateResult3.Content));
			num += num2;
			operateResult.Content.AddressStart += num2;
		}
		return OperateResult.CreateSuccessResult(list.Take(length).ToArray());
	}

	/// <summary>
	/// 批量读取bool类型数据，支持的类型为X,Y,S,T,C，具体的地址范围取决于PLC的类型
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <param name="address">地址信息，比如X10,Y17，注意X，Y的地址是8进制的</param>
	/// <param name="length">读取的长度</param>
	/// <returns>读取结果信息</returns>
	public static async Task<OperateResult<bool[]>> ReadBoolAsync(IReadWriteA3C plc, string address, ushort length)
	{
		if (address.IndexOf('.') > 0)
		{
			return await HslHelper.ReadBoolAsync(plc, address, length);
		}
		byte stat = (byte)HslHelper.ExtractParameter(ref address, "s", plc.Station);
		OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom(address, length, isBit: true);
		if (!addressResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(addressResult);
		}
		List<bool> boolContent = new List<bool>();
		ushort alreadyFinished = 0;
		while (alreadyFinished < length)
		{
			ushort readLength = (ushort)Math.Min(length - alreadyFinished, McHelper.GetReadBoolLength(McType.MCAscii));
			addressResult.Content.Length = readLength;
			byte[] command = ((plc.Format == 5) ? McBinaryHelper.BuildReadMcCoreCommand(addressResult.Content, isBit: true) : McAsciiHelper.BuildAsciiReadMcCoreCommand(addressResult.Content, isBit: true));
			OperateResult<byte[]> read = await plc.ReadFromCoreServerAsync(PackCommand(plc, command, stat));
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(read);
			}
			OperateResult<byte[]> check = ExtraReadActualResponse(plc, read.Content);
			if (!check.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(check);
			}
			boolContent.AddRange(ParseBoolValue(plc, check.Content));
			alreadyFinished += readLength;
			addressResult.Content.AddressStart += readLength;
		}
		return OperateResult.CreateSuccessResult(boolContent.Take(length).ToArray());
	}

	/// <summary>
	/// 批量写入bool类型的数组，支持的类型为X,Y,S,T,C，具体的地址范围取决于PLC的类型
	/// Write arrays of type bool in batches. The supported types are X, Y, S, T, C. The specific address range depends on the type of PLC.
	/// </summary>
	/// <param name="address">PLC的地址信息</param>
	/// <param name="value">数据信息</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// 当需要写入D寄存器的位时，可以开启为True，然后地址使用 D100.2 等格式进行批量写入位操作，该操作有一定风险，参考属性。
	/// </remarks>
	public static OperateResult Write(IReadWriteA3C plc, string address, bool[] value)
	{
		if (plc.EnableWriteBitToWordRegister && address.Contains("."))
		{
			return ReadWriteNetHelper.WriteBoolWithWord(plc, address, value);
		}
		byte station = (byte)HslHelper.ExtractParameter(ref address, "s", plc.Station);
		OperateResult<McAddressData> operateResult = McAddressData.ParseMelsecFrom(address, 0, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		byte[] mcCommand = ((plc.Format == 5) ? McBinaryHelper.BuildWriteBitCoreCommand(operateResult.Content, value) : McAsciiHelper.BuildAsciiWriteBitCoreCommand(operateResult.Content, value));
		OperateResult<byte[]> operateResult2 = plc.ReadFromCoreServer(PackCommand(plc, mcCommand, station));
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return CheckWriteResponse(plc, operateResult2.Content);
	}

	/// <summary>
	/// 批量写入bool类型的数组，支持的类型为X,Y,S,T,C，具体的地址范围取决于PLC的类型
	/// Write arrays of type bool in batches. The supported types are X, Y, S, T, C. The specific address range depends on the type of PLC.
	/// </summary>
	/// <param name="address">PLC的地址信息</param>
	/// <param name="value">数据信息</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// 当需要写入D寄存器的位时，可以开启为True，然后地址使用 D100.2 等格式进行批量写入位操作，该操作有一定风险，参考属性。
	/// </remarks>
	public static async Task<OperateResult> WriteAsync(IReadWriteA3C plc, string address, bool[] value)
	{
		if (plc.EnableWriteBitToWordRegister && address.Contains("."))
		{
			return await ReadWriteNetHelper.WriteBoolWithWordAsync(plc, address, value).ConfigureAwait(continueOnCapturedContext: false);
		}
		byte stat = (byte)HslHelper.ExtractParameter(ref address, "s", plc.Station);
		OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom(address, 0, isBit: true);
		if (!addressResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(addressResult);
		}
		byte[] command = ((plc.Format == 5) ? McBinaryHelper.BuildWriteBitCoreCommand(addressResult.Content, value) : McAsciiHelper.BuildAsciiWriteBitCoreCommand(addressResult.Content, value));
		OperateResult<byte[]> read = await plc.ReadFromCoreServerAsync(PackCommand(plc, command, stat));
		if (!read.IsSuccess)
		{
			return read;
		}
		return CheckWriteResponse(plc, read.Content);
	}

	/// <summary>
	/// 远程Run操作
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <returns>是否成功</returns>
	public static OperateResult RemoteRun(IReadWriteA3C plc)
	{
		OperateResult<byte[]> operateResult = plc.ReadFromCoreServer(PackCommand(plc, Encoding.ASCII.GetBytes("1001000000010000"), plc.Station));
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return CheckWriteResponse(plc, operateResult.Content);
	}

	/// <summary>
	/// 远程Stop操作
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <returns>是否成功</returns>
	public static OperateResult RemoteStop(IReadWriteA3C plc)
	{
		OperateResult<byte[]> operateResult = plc.ReadFromCoreServer(PackCommand(plc, Encoding.ASCII.GetBytes("100200000001"), plc.Station));
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return CheckWriteResponse(plc, operateResult.Content);
	}

	/// <summary>
	/// 读取PLC的型号信息
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <returns>返回型号的结果对象</returns>
	public static OperateResult<string> ReadPlcType(IReadWriteA3C plc)
	{
		OperateResult<byte[]> operateResult = plc.ReadFromCoreServer(PackCommand(plc, Encoding.ASCII.GetBytes("01010000"), plc.Station));
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		OperateResult<byte[]> operateResult2 = ExtraReadActualResponse(plc, operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(Encoding.ASCII.GetString(operateResult2.Content, 0, 16).TrimEnd());
	}

	/// <summary>
	/// 远程Run操作
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <returns>是否成功</returns>
	public static async Task<OperateResult> RemoteRunAsync(IReadWriteA3C plc)
	{
		OperateResult<byte[]> read = await plc.ReadFromCoreServerAsync(PackCommand(plc, Encoding.ASCII.GetBytes("1001000000010000"), plc.Station));
		if (!read.IsSuccess)
		{
			return read;
		}
		return CheckWriteResponse(plc, read.Content);
	}

	/// <summary>
	/// 远程Stop操作
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <returns>是否成功</returns>
	public static async Task<OperateResult> RemoteStopAsync(IReadWriteA3C plc)
	{
		OperateResult<byte[]> read = await plc.ReadFromCoreServerAsync(PackCommand(plc, Encoding.ASCII.GetBytes("100200000001"), plc.Station));
		if (!read.IsSuccess)
		{
			return read;
		}
		return CheckWriteResponse(plc, read.Content);
	}

	/// <summary>
	/// 读取PLC的型号信息
	/// </summary>
	/// <param name="plc">PLC设备通信对象</param>
	/// <returns>返回型号的结果对象</returns>
	public static async Task<OperateResult<string>> ReadPlcTypeAsync(IReadWriteA3C plc)
	{
		OperateResult<byte[]> read = await plc.ReadFromCoreServerAsync(PackCommand(plc, Encoding.ASCII.GetBytes("01010000"), plc.Station));
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		OperateResult<byte[]> check = ExtraReadActualResponse(plc, read.Content);
		if (!check.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(check);
		}
		return OperateResult.CreateSuccessResult(Encoding.ASCII.GetString(check.Content, 0, 16).TrimEnd());
	}
}
