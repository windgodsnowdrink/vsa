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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Address;

namespace HslCommunication.Profinet.Keyence;

/// <summary>
/// KeyenceNano的基本辅助方法
/// </summary>
public class KeyenceNanoHelper
{
	/// <summary>
	/// 连接PLC的命令报文
	/// Command message to connect to PLC
	/// </summary>
	/// <param name="station">当前PLC的站号信息</param>
	/// <param name="useStation">是否启动站号命令</param>
	public static byte[] GetConnectCmd(byte station, bool useStation)
	{
		return useStation ? Encoding.ASCII.GetBytes($"CR {station:D2}\r") : Encoding.ASCII.GetBytes("CR\r");
	}

	/// <summary>
	/// 断开PLC连接的命令报文
	/// Command message to disconnect PLC
	/// </summary>
	/// <param name="station">当前PLC的站号信息</param>
	/// <param name="useStation">是否启动站号命令</param>
	public static byte[] GetDisConnectCmd(byte station, bool useStation)
	{
		return Encoding.ASCII.GetBytes("CQ\r");
	}

	/// <summary>
	/// 获取当前的地址类型是字数据的倍数关系
	/// </summary>
	/// <param name="type">地址的类型</param>
	/// <returns>倍数关系</returns>
	public static int GetWordAddressMultiple(string type)
	{
		switch (type)
		{
		default:
			if (!(type == "AT"))
			{
				switch (type)
				{
				default:
					if (!(type == "VM"))
					{
						return 1;
					}
					goto case "DM";
				case "DM":
				case "CM":
				case "TM":
				case "EM":
				case "FM":
				case "Z":
				case "W":
				case "ZF":
					return 1;
				}
			}
			goto case "CTH";
		case "CTH":
		case "CTC":
		case "C":
		case "T":
		case "TS":
		case "TC":
		case "CS":
		case "CC":
			return 2;
		}
	}

	/// <summary>
	/// 建立读取PLC数据的指令，需要传入地址数据，以及读取的长度，地址示例参照类的说明文档
	/// To create a command to read PLC data, you need to pass in the address data, and the length of the read. For an example of the address, refer to the class documentation
	/// </summary>
	/// <param name="address">软元件地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>是否建立成功</returns>
	public static OperateResult<List<byte[]>> BuildReadCommand(string address, ushort length)
	{
		OperateResult<KeyenceNanoAddress> operateResult = KeyenceNanoAddress.ParseFrom(address, length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<List<byte[]>>(operateResult);
		}
		return BuildReadCommand(operateResult.Content, length, isBit: false);
	}

	/// <summary>
	/// 建立读取PLC数据的指令，需要传入地址数据，以及读取的长度，地址示例参照类的说明文档
	/// To create a command to read PLC data, you need to pass in the address data, and the length of the read. For an example of the address, refer to the class documentation
	/// </summary>
	/// <param name="address">软元件地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>是否建立成功</returns>
	public static OperateResult<List<byte[]>> BuildReadCommand(KeyenceNanoAddress address, ushort length, bool isBit)
	{
		if (length > 1)
		{
			length = (ushort)(length / GetWordAddressMultiple(address.DataCode));
		}
		int[] array = SoftBasic.SplitIntegerToArray(length, address.SplitLength);
		List<byte[]> list = new List<byte[]>();
		for (int i = 0; i < array.Length; i++)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("RDS");
			stringBuilder.Append(" ");
			stringBuilder.Append(address.DataCode);
			stringBuilder.Append(address.GetAddressStartFormat());
			if (!isBit && address.IsBitAddressDefault())
			{
				stringBuilder.Append(".U");
			}
			stringBuilder.Append(" ");
			stringBuilder.Append(array[i].ToString());
			stringBuilder.Append("\r");
			list.Add(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
			address.AddressStart += array[i];
		}
		return OperateResult.CreateSuccessResult(list);
	}

	/// <summary>
	/// 建立写入PLC数据的指令，需要传入地址数据，以及写入的数据信息，地址示例参照类的说明文档
	/// To create a command to write PLC data, you need to pass in the address data and the written data information. For an example of the address, refer to the class documentation
	/// </summary>
	/// <param name="address">软元件地址</param>
	/// <param name="value">转换后的数据</param>
	/// <returns>是否成功的信息</returns>
	public static OperateResult<byte[]> BuildWriteCommand(string address, byte[] value)
	{
		OperateResult<KeyenceNanoAddress> operateResult = KeyenceNanoAddress.ParseFrom(address, 0);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("WRS");
		stringBuilder.Append(" ");
		stringBuilder.Append(operateResult.Content.DataCode);
		stringBuilder.Append(operateResult.Content.GetAddressStartFormat());
		if (operateResult.Content.IsBitAddressDefault())
		{
			stringBuilder.Append(".U");
		}
		stringBuilder.Append(" ");
		int num = value.Length / (GetWordAddressMultiple(operateResult.Content.DataCode) * 2);
		stringBuilder.Append(num.ToString());
		for (int i = 0; i < num; i++)
		{
			stringBuilder.Append(" ");
			stringBuilder.Append(BitConverter.ToUInt16(value, i * GetWordAddressMultiple(operateResult.Content.DataCode) * 2));
		}
		stringBuilder.Append("\r");
		return OperateResult.CreateSuccessResult(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
	}

	/// <summary>
	/// 构建写入扩展单元缓冲寄存器的报文命令，需要传入单元编号，地址，写入的数据，实际写入的数据格式才有无符号的方式
	/// To construct a message command to write to the buffer register of the expansion unit, the unit number, address,
	/// and data to be written need to be passed in, and the format of the actually written data is unsigned.
	/// </summary>
	/// <param name="unit">单元编号0~48</param>
	/// <param name="address">地址0~32767</param>
	/// <param name="value">写入的数据信息，单次交互最大256个字</param>
	/// <returns>包含是否成功的报文对象</returns>
	public static OperateResult<byte[]> BuildWriteExpansionMemoryCommand(byte unit, ushort address, byte[] value)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("UWR");
		stringBuilder.Append(" ");
		stringBuilder.Append(unit);
		stringBuilder.Append(" ");
		stringBuilder.Append(address);
		stringBuilder.Append(".U");
		stringBuilder.Append(" ");
		int count = value.Length / 2;
		stringBuilder.Append(count.ToString());
		for (int i = 0; i < count; i++)
		{
			stringBuilder.Append(" ");
			stringBuilder.Append(BitConverter.ToUInt16(value, i * 2));
		}
		stringBuilder.Append("\r");
		return OperateResult.CreateSuccessResult(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
	}

	/// <summary>
	/// 建立写入bool数据的指令，针对地址类型为 R,CR,MR,LR
	/// Create instructions to write bool data, address type is R, CR, MR, LR
	/// </summary>
	/// <param name="address">软元件地址</param>
	/// <param name="value">转换后的数据</param>
	/// <returns>是否成功的信息</returns>
	public static OperateResult<byte[]> BuildWriteCommand(string address, bool value)
	{
		OperateResult<KeyenceNanoAddress> operateResult = KeyenceNanoAddress.ParseFrom(address, 0);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (value)
		{
			stringBuilder.Append("ST");
		}
		else
		{
			stringBuilder.Append("RS");
		}
		stringBuilder.Append(" ");
		stringBuilder.Append(operateResult.Content.DataCode);
		stringBuilder.Append(operateResult.Content.GetAddressStartFormat());
		stringBuilder.Append("\r");
		return OperateResult.CreateSuccessResult(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
	}

	/// <summary>
	/// 批量写入数据位到plc地址，针对地址格式为 R,B,CR,MR,LR,VB
	/// Write data bits in batches to the plc address, and the address format is R, B, CR, MR, LR, VB
	/// </summary>
	/// <param name="address">PLC的地址</param>
	/// <param name="value">等待写入的bool数组</param>
	/// <returns>写入bool数组的命令报文</returns>
	public static OperateResult<byte[]> BuildWriteCommand(string address, bool[] value)
	{
		OperateResult<KeyenceNanoAddress> operateResult = KeyenceNanoAddress.ParseFrom(address, 0);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("WRS");
		stringBuilder.Append(" ");
		stringBuilder.Append(operateResult.Content.DataCode);
		stringBuilder.Append(operateResult.Content.GetAddressStartFormat());
		stringBuilder.Append(" ");
		stringBuilder.Append(value.Length.ToString());
		for (int i = 0; i < value.Length; i++)
		{
			stringBuilder.Append(" ");
			stringBuilder.Append(value[i] ? "1" : "0");
		}
		stringBuilder.Append("\r");
		return OperateResult.CreateSuccessResult(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
	}

	internal static string GetErrorText(string err)
	{
		if (err.StartsWith("E0"))
		{
			return StringResources.Language.KeyenceNanoE0;
		}
		if (err.StartsWith("E1"))
		{
			return StringResources.Language.KeyenceNanoE1;
		}
		if (err.StartsWith("E2"))
		{
			return StringResources.Language.KeyenceNanoE2;
		}
		if (err.StartsWith("E4"))
		{
			return StringResources.Language.KeyenceNanoE4;
		}
		if (err.StartsWith("E5"))
		{
			return StringResources.Language.KeyenceNanoE5;
		}
		if (err.StartsWith("E6"))
		{
			return StringResources.Language.KeyenceNanoE6;
		}
		return StringResources.Language.UnknownError + " " + err;
	}

	/// <summary>
	/// 校验读取返回数据状态，主要返回的第一个字节是不是E
	/// Check the status of the data returned from reading, whether the first byte returned is E
	/// </summary>
	/// <param name="ack">反馈信息</param>
	/// <returns>是否成功的信息</returns>
	public static OperateResult CheckPlcReadResponse(byte[] ack)
	{
		try
		{
			if (ack.Length == 0)
			{
				return new OperateResult(StringResources.Language.MelsecFxReceiveZero);
			}
			if (ack[0] == 69)
			{
				return new OperateResult(GetErrorText(Encoding.ASCII.GetString(ack)));
			}
			if (ack[^1] != 10 && ack[^2] != 13)
			{
				return new OperateResult(StringResources.Language.MelsecFxAckWrong + " Actual: " + SoftBasic.ByteToHexString(ack, ' '));
			}
			return OperateResult.CreateSuccessResult();
		}
		catch (Exception ex)
		{
			return new OperateResult("CheckPlcReadResponse failed: " + ex.Message + Environment.NewLine + ack.ToHexString(' '));
		}
	}

	/// <summary>
	/// 校验写入返回数据状态，检测返回的数据是不是OK
	/// Verify the status of the returned data written and check whether the returned data is OK
	/// </summary>
	/// <param name="ack">反馈信息</param>
	/// <returns>是否成功的信息</returns>
	public static OperateResult CheckPlcWriteResponse(byte[] ack)
	{
		try
		{
			if (ack.Length == 0)
			{
				return new OperateResult(StringResources.Language.MelsecFxReceiveZero);
			}
			if (ack[0] == 79 && ack[1] == 75)
			{
				return OperateResult.CreateSuccessResult();
			}
			return new OperateResult(GetErrorText(Encoding.ASCII.GetString(ack)));
		}
		catch (Exception ex)
		{
			return new OperateResult("CheckPlcWriteResponse failed: " + ex.Message + Environment.NewLine + ack.ToHexString(' '));
		}
	}

	/// <summary>
	/// 从PLC反馈的数据进行提炼Bool操作
	/// Refine Bool operation from data fed back from PLC
	/// </summary>
	/// <param name="addressType">地址的数据类型</param>
	/// <param name="response">PLC反馈的真实数据</param>
	/// <returns>数据提炼后的真实数据</returns>
	public static OperateResult<bool[]> ExtractActualBoolData(string addressType, byte[] response)
	{
		try
		{
			if (string.IsNullOrEmpty(addressType))
			{
				addressType = "R";
			}
			string message = Encoding.Default.GetString(response.RemoveLast(2));
			switch (addressType)
			{
			default:
				if (!(addressType == "VB"))
				{
					switch (addressType)
					{
					default:
						if (!(addressType == "CTC"))
						{
							return new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType);
						}
						goto case "T";
					case "T":
					case "C":
					case "CTH":
						return OperateResult.CreateSuccessResult((from m in message.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
							select m.StartsWith("1")).ToArray());
					}
				}
				goto case "R";
			case "R":
			case "CR":
			case "MR":
			case "LR":
			case "B":
				return OperateResult.CreateSuccessResult((from m in message.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
					select m == "1").ToArray());
			}
		}
		catch (Exception ex)
		{
			OperateResult<bool[]> operateResult = new OperateResult<bool[]>();
			operateResult.Message = "Extract Msg：" + ex.Message + Environment.NewLine + "Data: " + SoftBasic.ByteToHexString(response);
			return operateResult;
		}
	}

	/// <summary>
	/// 从PLC反馈的数据进行提炼操作
	/// Refining operation from data fed back from PLC
	/// </summary>
	/// <param name="addressType">地址的数据类型</param>
	/// <param name="response">PLC反馈的真实数据</param>
	/// <returns>数据提炼后的真实数据</returns>
	public static OperateResult<byte[]> ExtractActualData(string addressType, byte[] response)
	{
		try
		{
			if (string.IsNullOrEmpty(addressType))
			{
				addressType = "R";
			}
			string message = Encoding.Default.GetString(response.RemoveLast(2));
			string[] array = message.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			switch (addressType)
			{
			default:
				if (!(addressType == "VM"))
				{
					switch (addressType)
					{
					default:
						if (!(addressType == "CS"))
						{
							switch (addressType)
							{
							default:
								if (!(addressType == "CTC"))
								{
									switch (addressType)
									{
									default:
										if (!(addressType == "VB"))
										{
											return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
										}
										goto case "R";
									case "R":
									case "B":
									case "MR":
									case "LR":
									case "CR":
									{
										byte[] buffer = new byte[array.Length * 2];
										for (int i = 0; i < array.Length; i++)
										{
											BitConverter.GetBytes(ushort.Parse(array[i])).CopyTo(buffer, i * 2);
										}
										return OperateResult.CreateSuccessResult(buffer);
									}
									}
								}
								goto case "T";
							case "T":
							case "C":
							case "CTH":
							{
								byte[] buffer2 = new byte[array.Length * 4];
								for (int j = 0; j < array.Length; j++)
								{
									string[] array4 = array[j].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
									BitConverter.GetBytes(uint.Parse(array4[1])).CopyTo(buffer2, j * 4);
								}
								return OperateResult.CreateSuccessResult(buffer2);
							}
							}
						}
						goto case "AT";
					case "AT":
					case "TC":
					case "CC":
					case "TS":
					{
						byte[] buffer3 = new byte[array.Length * 4];
						for (int k = 0; k < array.Length; k++)
						{
							BitConverter.GetBytes(uint.Parse(array[k])).CopyTo(buffer3, k * 4);
						}
						return OperateResult.CreateSuccessResult(buffer3);
					}
					}
				}
				goto case "DM";
			case "DM":
			case "EM":
			case "FM":
			case "ZF":
			case "W":
			case "TM":
			case "Z":
			case "CM":
			{
				byte[] buffer4 = new byte[array.Length * 2];
				for (int l = 0; l < array.Length; l++)
				{
					BitConverter.GetBytes(ushort.Parse(array[l])).CopyTo(buffer4, l * 2);
				}
				return OperateResult.CreateSuccessResult(buffer4);
			}
			}
		}
		catch (Exception ex)
		{
			OperateResult<byte[]> operateResult = new OperateResult<byte[]>();
			operateResult.Message = "Extract Msg：" + ex.Message + Environment.NewLine + "Data: " + SoftBasic.ByteToHexString(response);
			return operateResult;
		}
	}

	/// <summary>
	/// 批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组
	/// Batch read byte array information, need to specify the address and length, return the original byte array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public static OperateResult<byte[]> Read(IReadWriteDevice keyence, string address, ushort length)
	{
		if (address.StartsWith("unit="))
		{
			byte unit = (byte)HslHelper.ExtractParameter(ref address, "unit", 0);
			if (!ushort.TryParse(address, out var _))
			{
				return new OperateResult<byte[]>("Address is not right, convert ushort wrong!");
			}
			return ReadExpansionMemory(keyence, unit, ushort.Parse(address), length);
		}
		OperateResult<KeyenceNanoAddress> operateResult = KeyenceNanoAddress.ParseFrom(address, length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		OperateResult<List<byte[]>> operateResult2 = BuildReadCommand(operateResult.Content, length, isBit: false);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		List<byte> list = new List<byte>();
		for (int i = 0; i < operateResult2.Content.Count; i++)
		{
			OperateResult<byte[]> operateResult3 = keyence.ReadFromCoreServer(operateResult2.Content[i]);
			if (!operateResult3.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult3);
			}
			OperateResult operateResult4 = CheckPlcReadResponse(operateResult3.Content);
			if (!operateResult4.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult4);
			}
			OperateResult<byte[]> operateResult5 = ExtractActualData(operateResult.Content.DataCode, operateResult3.Content);
			if (!operateResult5.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult5);
			}
			list.AddRange(operateResult5.Content);
		}
		return OperateResult.CreateSuccessResult(list.ToArray());
	}

	/// <summary>
	/// 写入原始的byte数组数据到指定的地址，返回是否写入成功
	/// Write the original byte array data to the specified address, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public static OperateResult Write(IReadWriteDevice keyence, string address, byte[] value)
	{
		OperateResult<byte[]> operateResult = BuildWriteCommand(address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = keyence.ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		OperateResult operateResult3 = CheckPlcWriteResponse(operateResult2.Content);
		if (!operateResult3.IsSuccess)
		{
			return operateResult3;
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
	public static async Task<OperateResult<byte[]>> ReadAsync(IReadWriteDevice keyence, string address, ushort length)
	{
		if (address.StartsWith("unit="))
		{
			byte unit = (byte)HslHelper.ExtractParameter(ref address, "unit", 0);
			if (!ushort.TryParse(address, out var _))
			{
				return new OperateResult<byte[]>("Address is not right, convert ushort wrong!");
			}
			return await ReadExpansionMemoryAsync(keyence, unit, ushort.Parse(address), length);
		}
		OperateResult<KeyenceNanoAddress> addressResult = KeyenceNanoAddress.ParseFrom(address, length);
		if (!addressResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(addressResult);
		}
		OperateResult<List<byte[]>> command = BuildReadCommand(addressResult.Content, length, isBit: false);
		if (!command.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(command);
		}
		List<byte> array = new List<byte>();
		for (int i = 0; i < command.Content.Count; i++)
		{
			OperateResult<byte[]> read = await keyence.ReadFromCoreServerAsync(command.Content[i]);
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(read);
			}
			OperateResult ackResult = CheckPlcReadResponse(read.Content);
			if (!ackResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(ackResult);
			}
			OperateResult<byte[]> extra = ExtractActualData(addressResult.Content.DataCode, read.Content);
			if (!extra.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(extra);
			}
			array.AddRange(extra.Content);
		}
		return OperateResult.CreateSuccessResult(array.ToArray());
	}

	/// <summary>
	/// 写入原始的byte数组数据到指定的地址，返回是否写入成功
	/// Write the original byte array data to the specified address, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public static async Task<OperateResult> WriteAsync(IReadWriteDevice keyence, string address, byte[] value)
	{
		OperateResult<byte[]> command = BuildWriteCommand(address, value);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await keyence.ReadFromCoreServerAsync(command.Content);
		if (!read.IsSuccess)
		{
			return read;
		}
		OperateResult checkResult = CheckPlcWriteResponse(read.Content);
		if (!checkResult.IsSuccess)
		{
			return checkResult;
		}
		return OperateResult.CreateSuccessResult();
	}

	private static bool CheckBoolOnWordAddress(string address)
	{
		return Regex.IsMatch(address, "^(DM|CM|EM|FM|ZF|VM)[0-9]+\\.[0-9]+$", RegexOptions.IgnoreCase);
	}

	public static OperateResult<bool[]> ReadBool(IReadWriteDevice keyence, string address, ushort length)
	{
		if (CheckBoolOnWordAddress(address))
		{
			return HslHelper.ReadBool(keyence, address, length);
		}
		OperateResult<KeyenceNanoAddress> operateResult = KeyenceNanoAddress.ParseFrom(address, length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		OperateResult<List<byte[]>> operateResult2 = BuildReadCommand(operateResult.Content, length, isBit: true);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult2);
		}
		List<bool> list = new List<bool>();
		for (int i = 0; i < operateResult2.Content.Count; i++)
		{
			OperateResult<byte[]> operateResult3 = keyence.ReadFromCoreServer(operateResult2.Content[i]);
			if (!operateResult3.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(operateResult3);
			}
			OperateResult operateResult4 = CheckPlcReadResponse(operateResult3.Content);
			if (!operateResult4.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(operateResult4);
			}
			OperateResult<bool[]> operateResult5 = ExtractActualBoolData(operateResult.Content.DataCode, operateResult3.Content);
			if (!operateResult5.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(operateResult5);
			}
			list.AddRange(operateResult5.Content);
		}
		return OperateResult.CreateSuccessResult(list.ToArray());
	}

	public static OperateResult Write(IReadWriteDevice keyence, string address, bool value)
	{
		if (CheckBoolOnWordAddress(address))
		{
			return HslHelper.WriteBool(keyence, address, new bool[1] { value });
		}
		OperateResult<byte[]> operateResult = BuildWriteCommand(address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = keyence.ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		OperateResult operateResult3 = CheckPlcWriteResponse(operateResult2.Content);
		if (!operateResult3.IsSuccess)
		{
			return operateResult3;
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
	public static OperateResult Write(IReadWriteDevice keyence, string address, bool[] value)
	{
		if (CheckBoolOnWordAddress(address))
		{
			return HslHelper.WriteBool(keyence, address, value);
		}
		OperateResult<byte[]> operateResult = BuildWriteCommand(address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = keyence.ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		OperateResult operateResult3 = CheckPlcWriteResponse(operateResult2.Content);
		if (!operateResult3.IsSuccess)
		{
			return operateResult3;
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
	public static async Task<OperateResult<bool[]>> ReadBoolAsync(IReadWriteDevice keyence, string address, ushort length)
	{
		if (CheckBoolOnWordAddress(address))
		{
			return await HslHelper.ReadBoolAsync(keyence, address, length).ConfigureAwait(continueOnCapturedContext: false);
		}
		OperateResult<KeyenceNanoAddress> addressResult = KeyenceNanoAddress.ParseFrom(address, length);
		if (!addressResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(addressResult);
		}
		OperateResult<List<byte[]>> command = BuildReadCommand(addressResult.Content, length, isBit: true);
		if (!command.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(command);
		}
		List<bool> array = new List<bool>();
		for (int i = 0; i < command.Content.Count; i++)
		{
			OperateResult<byte[]> read = await keyence.ReadFromCoreServerAsync(command.Content[i]).ConfigureAwait(continueOnCapturedContext: false);
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(read);
			}
			OperateResult ackResult = CheckPlcReadResponse(read.Content);
			if (!ackResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(ackResult);
			}
			OperateResult<bool[]> extra = ExtractActualBoolData(addressResult.Content.DataCode, read.Content);
			if (!extra.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(extra);
			}
			array.AddRange(extra.Content);
		}
		return OperateResult.CreateSuccessResult(array.ToArray());
	}

	/// <summary>
	/// 写入单个的数据，返回是否成功
	/// Write a single  data, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public static async Task<OperateResult> WriteAsync(IReadWriteDevice keyence, string address, bool value)
	{
		if (CheckBoolOnWordAddress(address))
		{
			return await HslHelper.WriteBoolAsync(keyence, address, new bool[1] { value }).ConfigureAwait(continueOnCapturedContext: false);
		}
		OperateResult<byte[]> command = BuildWriteCommand(address, value);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await keyence.ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		OperateResult checkResult = CheckPlcWriteResponse(read.Content);
		if (!checkResult.IsSuccess)
		{
			return checkResult;
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
	public static async Task<OperateResult> WriteAsync(IReadWriteDevice keyence, string address, bool[] value)
	{
		if (CheckBoolOnWordAddress(address))
		{
			return await HslHelper.WriteBoolAsync(keyence, address, value).ConfigureAwait(continueOnCapturedContext: false);
		}
		OperateResult<byte[]> command = BuildWriteCommand(address, value);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await keyence.ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		OperateResult checkResult = CheckPlcWriteResponse(read.Content);
		if (!checkResult.IsSuccess)
		{
			return checkResult;
		}
		return OperateResult.CreateSuccessResult();
	}

	private static OperateResult<KeyencePLCS> ExtraPlcType(OperateResult<byte[]> read)
	{
		if (!read.IsSuccess)
		{
			return read.ConvertFailed<KeyencePLCS>();
		}
		OperateResult operateResult = CheckPlcReadResponse(read.Content);
		if (!operateResult.IsSuccess)
		{
			return operateResult.ConvertFailed<KeyencePLCS>();
		}
		string message = Encoding.ASCII.GetString(read.Content.RemoveLast(2));
		switch (message)
		{
		case "48":
		case "49":
			return OperateResult.CreateSuccessResult(KeyencePLCS.KV700);
		case "50":
			return OperateResult.CreateSuccessResult(KeyencePLCS.KV1000);
		case "51":
			return OperateResult.CreateSuccessResult(KeyencePLCS.KV3000);
		case "52":
			return OperateResult.CreateSuccessResult(KeyencePLCS.KV5000);
		case "53":
			return OperateResult.CreateSuccessResult(KeyencePLCS.KV5500);
		default:
			return new OperateResult<KeyencePLCS>("Unknow type:" + message);
		}
	}

	/// <summary>
	/// [商业授权] 查询PLC的型号信息
	/// [Authorization] Query PLC model information
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <returns>包含型号的结果对象</returns>
	internal static OperateResult<KeyencePLCS> ReadPlcType(IReadWriteDevice keyence)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<KeyencePLCS>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> read = keyence.ReadFromCoreServer(Encoding.ASCII.GetBytes("?K\r"));
		return ExtraPlcType(read);
	}

	/// <summary>
	/// [商业授权] 查询PLC的型号信息
	/// [Authorization] Query PLC model information
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <returns>包含型号的结果对象</returns>
	internal static async Task<OperateResult<KeyencePLCS>> ReadPlcTypeAsync(IReadWriteDevice keyence)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<KeyencePLCS>(StringResources.Language.InsufficientPrivileges);
		}
		return ExtraPlcType(await keyence.ReadFromCoreServerAsync(Encoding.ASCII.GetBytes("?K\r")).ConfigureAwait(continueOnCapturedContext: false));
	}

	/// <summary>
	/// [商业授权] 清除CPU单元发生的错误
	/// [Authorization] Clear CPU unit errors
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <returns>是否成功</returns>
	public static OperateResult ClearError(IReadWriteDevice keyence)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<int>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> operateResult = keyence.ReadFromCoreServer(Encoding.ASCII.GetBytes("ER\r"));
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return CheckPlcWriteResponse(operateResult.Content);
	}

	/// <summary>
	/// [商业授权] 清除CPU单元发生的错误
	/// [Authorization] Clear CPU unit errors
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <returns>是否成功</returns>
	public static async Task<OperateResult> ClearErrorAsync(IReadWriteDevice keyence)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<int>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> read = await keyence.ReadFromCoreServerAsync(Encoding.ASCII.GetBytes("ER\r")).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		return CheckPlcWriteResponse(read.Content);
	}

	private static OperateResult<int> ExtraPlcMode(OperateResult<byte[]> read)
	{
		if (!read.IsSuccess)
		{
			return read.ConvertFailed<int>();
		}
		OperateResult operateResult = CheckPlcReadResponse(read.Content);
		if (!operateResult.IsSuccess)
		{
			return operateResult.ConvertFailed<int>();
		}
		string message = Encoding.ASCII.GetString(read.Content.RemoveLast(2));
		if (message == "0")
		{
			return OperateResult.CreateSuccessResult(0);
		}
		return OperateResult.CreateSuccessResult(1);
	}

	/// <summary>
	/// [商业授权] 读取当前PLC的模式，如果是0，代表 PROG模式或者梯形图未登录，如果为1，代表RUN模式
	/// [Authorization] Read the current PLC mode, if it is 0, it means PROG mode or the ladder diagram is not registered, if it is 1, it means RUN mode
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <returns>包含模式的结果对象</returns>
	internal static OperateResult<int> ReadPlcMode(IReadWriteDevice keyence)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<int>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> read = keyence.ReadFromCoreServer(Encoding.ASCII.GetBytes("?M\r"));
		return ExtraPlcMode(read);
	}

	/// <summary>
	/// [商业授权] 读取当前PLC的模式，如果是0，代表 PROG模式或者梯形图未登录，如果为1，代表RUN模式
	/// [Authorization] Read the current PLC mode, if it is 0, it means PROG mode or the ladder diagram is not registered, if it is 1, it means RUN mode
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <returns>包含模式的结果对象</returns>
	internal static async Task<OperateResult<int>> ReadPlcModeAsync(IReadWriteDevice keyence)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<int>(StringResources.Language.InsufficientPrivileges);
		}
		return ExtraPlcMode(await keyence.ReadFromCoreServerAsync(Encoding.ASCII.GetBytes("?M\r")).ConfigureAwait(continueOnCapturedContext: false));
	}

	/// <summary>
	/// [商业授权] 设置PLC的时间
	/// [Authorization] Set PLC time
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <param name="dateTime">时间数据</param>
	/// <returns>是否设置成功</returns>
	public static OperateResult SetPlcDateTime(IReadWriteDevice keyence, DateTime dateTime)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<int>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> operateResult = keyence.ReadFromCoreServer(Encoding.ASCII.GetBytes($"WRT {dateTime.Year - 2000:D2} {dateTime.Month:D2} {dateTime.Day:D2} " + $"{dateTime.Hour:D2} {dateTime.Minute:D2} {dateTime.Second:D2} {(int)dateTime.DayOfWeek}\r"));
		if (!operateResult.IsSuccess)
		{
			return operateResult.ConvertFailed<int>();
		}
		return CheckPlcWriteResponse(operateResult.Content);
	}

	/// <summary>
	/// [商业授权] 设置PLC的时间
	/// [Authorization] Set PLC time
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <param name="dateTime">时间数据</param>
	/// <returns>是否设置成功</returns>
	public static async Task<OperateResult> SetPlcDateTimeAsync(IReadWriteDevice keyence, DateTime dateTime)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<int>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> read = await keyence.ReadFromCoreServerAsync(Encoding.ASCII.GetBytes($"WRT {dateTime.Year - 2000:D2} {dateTime.Month:D2} {dateTime.Day:D2} " + $"{dateTime.Hour:D2} {dateTime.Minute:D2} {dateTime.Second:D2} {(int)dateTime.DayOfWeek}\r")).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		return CheckPlcWriteResponse(read.Content);
	}

	/// <summary>
	/// [商业授权] 读取指定软元件的注释信息
	/// [Authorization] Read the comment information of the specified device
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <param name="address">软元件的地址</param>
	/// <returns>软元件的注释信息</returns>
	public static OperateResult<string> ReadAddressAnnotation(IReadWriteDevice keyence, string address)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<string>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> operateResult = keyence.ReadFromCoreServer(Encoding.ASCII.GetBytes("RDC " + address + "\r"));
		if (!operateResult.IsSuccess)
		{
			return operateResult.ConvertFailed<string>();
		}
		OperateResult operateResult2 = CheckPlcReadResponse(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2.ConvertFailed<string>();
		}
		return OperateResult.CreateSuccessResult(Encoding.ASCII.GetString(operateResult.Content.RemoveLast(2)).Trim(' '));
	}

	/// <summary>
	/// [商业授权] 读取指定软元件的注释信息
	/// [Authorization] Read the comment information of the specified device
	/// </summary>
	/// <param name="keyence">PLC通信对象</param>
	/// <param name="address">软元件的地址</param>
	/// <returns>软元件的注释信息</returns>
	public static async Task<OperateResult<string>> ReadAddressAnnotationAsync(IReadWriteDevice keyence, string address)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<string>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> read = await keyence.ReadFromCoreServerAsync(Encoding.ASCII.GetBytes("RDC " + address + "\r")).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read.ConvertFailed<string>();
		}
		OperateResult check = CheckPlcReadResponse(read.Content);
		if (!check.IsSuccess)
		{
			return check.ConvertFailed<string>();
		}
		return OperateResult.CreateSuccessResult(Encoding.ASCII.GetString(read.Content.RemoveLast(2)).Trim(' '));
	}

	/// <summary>
	/// [商业授权] 从扩展单元缓冲存储器连续读取指定个数的数据，单位为字
	/// [Authorization] Continuously read the specified number of data from the expansion unit buffer memory, the unit is word
	/// </summary>
	/// <param name="keyence">PLC的通信对象</param>
	/// <param name="unit">单元编号</param>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取的长度，单位为字</param>
	/// <returns>包含是否成功的原始字节数组</returns>
	public static OperateResult<byte[]> ReadExpansionMemory(IReadWriteDevice keyence, byte unit, ushort address, ushort length)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> operateResult = keyence.ReadFromCoreServer(Encoding.ASCII.GetBytes($"URD {unit} {address}.U {length}\r"));
		if (!operateResult.IsSuccess)
		{
			return operateResult.ConvertFailed<byte[]>();
		}
		OperateResult operateResult2 = CheckPlcReadResponse(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2.ConvertFailed<byte[]>();
		}
		return ExtractActualData("DM", operateResult.Content);
	}

	/// <summary>
	/// [商业授权] 从扩展单元缓冲存储器连续读取指定个数的数据，单位为字
	/// [Authorization] Continuously read the specified number of data from the expansion unit buffer memory, the unit is word
	/// </summary>
	/// <param name="keyence">PLC的通信对象</param>
	/// <param name="unit">单元编号</param>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取的长度，单位为字</param>
	/// <returns>包含是否成功的原始字节数组</returns>
	public static async Task<OperateResult<byte[]>> ReadExpansionMemoryAsync(IReadWriteDevice keyence, byte unit, ushort address, ushort length)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> read = await keyence.ReadFromCoreServerAsync(Encoding.ASCII.GetBytes($"URD {unit} {address}.U {length}\r")).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read.ConvertFailed<byte[]>();
		}
		OperateResult check = CheckPlcReadResponse(read.Content);
		if (!check.IsSuccess)
		{
			return check.ConvertFailed<byte[]>();
		}
		return ExtractActualData("DM", read.Content);
	}

	/// <summary>
	/// [商业授权] 将原始字节数据写入到扩展的缓冲存储器，需要指定单元编号，偏移地址，写入的数据
	/// [Authorization] To write the original byte data to the extended buffer memory, you need to specify the unit number, offset address, and write data
	/// </summary>
	/// <param name="keyence">PLC通信对象信息</param>
	/// <param name="unit">单元编号</param>
	/// <param name="address">偏移地址</param>
	/// <param name="value">等待写入的原始字节数据</param>
	/// <returns>是否写入成功的结果对象</returns>
	public static OperateResult WriteExpansionMemory(IReadWriteDevice keyence, byte unit, ushort address, byte[] value)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> operateResult = keyence.ReadFromCoreServer(BuildWriteExpansionMemoryCommand(unit, address, value).Content);
		if (!operateResult.IsSuccess)
		{
			return operateResult.ConvertFailed<byte[]>();
		}
		return CheckPlcWriteResponse(operateResult.Content);
	}

	/// <summary>
	/// [商业授权] 将原始字节数据写入到扩展的缓冲存储器，需要指定单元编号，偏移地址，写入的数据
	/// [Authorization] To write the original byte data to the extended buffer memory, you need to specify the unit number, offset address, and write data
	/// </summary>
	/// <param name="keyence">PLC通信对象信息</param>
	/// <param name="unit">单元编号</param>
	/// <param name="address">偏移地址</param>
	/// <param name="value">等待写入的原始字节数据</param>
	/// <returns>是否写入成功的结果对象</returns>
	public static async Task<OperateResult> WriteExpansionMemoryAsync(IReadWriteDevice keyence, byte unit, ushort address, byte[] value)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> read = await keyence.ReadFromCoreServerAsync(BuildWriteExpansionMemoryCommand(unit, address, value).Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read.ConvertFailed<byte[]>();
		}
		return CheckPlcWriteResponse(read.Content);
	}
}
