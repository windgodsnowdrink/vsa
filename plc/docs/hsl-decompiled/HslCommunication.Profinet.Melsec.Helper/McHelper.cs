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
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Core.Net;

namespace HslCommunication.Profinet.Melsec.Helper;

/// <summary>
/// MC协议的辅助类对象，提供了MC协议的读写操作的基本支持
/// </summary>
public class McHelper
{
	/// <summary>
	/// 返回按照字单位读取的最低的长度信息
	/// </summary>
	/// <param name="type">MC协议的类型</param>
	/// <returns>长度信息</returns>
	public static int GetReadWordLength(McType type)
	{
		if (type == McType.McBinary || type == McType.McRBinary)
		{
			return 950;
		}
		return 460;
	}

	/// <summary>
	/// 返回按照位单位读取的最低的长度信息
	/// </summary>
	/// <param name="type">MC协议的类型</param>
	/// <returns>长度信息</returns>
	public static int GetReadBoolLength(McType type)
	{
		if (type == McType.McBinary || type == McType.McRBinary)
		{
			return 7168;
		}
		return 3584;
	}

	internal static string AnalysisAddressLogMessage(PipeSession pipeSession, IByteTransform byteTransform, byte[] content)
	{
		if (content[0] == 80 && content.Length >= 21)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (content[11] == 1 && content[12] == 4)
			{
				if (content[13] == 0)
				{
					stringBuilder.Append(" --- Read(\"");
				}
				else
				{
					stringBuilder.Append(" --- ReadBool(\"");
				}
			}
			else
			{
				if (content[11] != 1 || content[12] != 20)
				{
					return string.Empty;
				}
				stringBuilder.Append(" --- Write(\"");
			}
			int num = content[15] + content[16] * 256 + content[17] * 256 * 256;
			MelsecMcDataType melsecAddress = MelsecMcDataType.GetMelsecAddress(content[18]);
			if (melsecAddress != null)
			{
				stringBuilder.Append(melsecAddress.AsciiCode.Trim('*'));
				stringBuilder.Append(Convert.ToString(num, melsecAddress.FromBase).ToUpper());
			}
			stringBuilder.Append("\", ");
			int num2 = BitConverter.ToUInt16(content, 19);
			if (content[12] == 4)
			{
				stringBuilder.Append(num2);
			}
			else if (content[13] == 0)
			{
				if (num2 == 1 && content.Length >= 23)
				{
					stringBuilder.Append($"{BitConverter.ToUInt16(content, 21)}");
				}
				else
				{
					stringBuilder.Append($"new byte[{num2 * 2}]");
				}
			}
			else if (num2 == 1 && content.Length >= 22)
			{
				stringBuilder.Append((content[21] == 0) ? "false" : "true");
			}
			else
			{
				stringBuilder.Append($"new bool[{num2}]");
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}
		return string.Empty;
	}

	/// <summary>
	/// 批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组
	/// Batch read byte array information, need to specify the address and length, return the original byte array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public static OperateResult<byte[]> Read(IReadWriteMc mc, string address, ushort length)
	{
		if (address.StartsWith("s=") || address.StartsWith("S="))
		{
			if (mc.McType == McType.McBinary)
			{
				return McBinaryHelper.ReadTags(mc, new string[1] { address.Substring(2) }, new ushort[1] { length });
			}
			if (mc.McType == McType.MCAscii)
			{
				return McAsciiHelper.ReadTags(mc, new string[1] { address.Substring(2) }, new ushort[1] { length });
			}
			return new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction);
		}
		if ((mc.McType == McType.McBinary || mc.McType == McType.MCAscii) && Regex.IsMatch(address, "ext=[0-9]+;", RegexOptions.IgnoreCase))
		{
			string value = Regex.Match(address, "ext=[0-9]+;").Value;
			ushort extend = ushort.Parse(Regex.Match(value, "[0-9]+").Value);
			return ReadExtend(mc, extend, address.Substring(value.Length), length);
		}
		if ((mc.McType == McType.McBinary || mc.McType == McType.MCAscii) && Regex.IsMatch(address, "mem=", RegexOptions.IgnoreCase))
		{
			return ReadMemory(mc, address.Substring(4), length);
		}
		if ((mc.McType == McType.McBinary || mc.McType == McType.MCAscii) && Regex.IsMatch(address, "module=[0-9]+;", RegexOptions.IgnoreCase))
		{
			string value2 = Regex.Match(address, "module=[0-9]+;").Value;
			ushort module = ushort.Parse(Regex.Match(value2, "[0-9]+").Value);
			return ReadSmartModule(mc, module, address.Substring(value2.Length), (ushort)(length * 2));
		}
		OperateResult<McAddressData> operateResult = mc.McAnalysisAddress(address, length, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		List<byte> list = new List<byte>();
		ushort num = 0;
		while (num < length)
		{
			ushort num2 = (ushort)Math.Min(length - num, GetReadWordLength(mc.McType));
			operateResult.Content.Length = num2;
			byte[] send = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildReadMcCoreCommand(operateResult.Content, isBit: false) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildAsciiReadMcCoreCommand(operateResult.Content, isBit: false) : ((mc.McType == McType.McRBinary) ? MelsecMcRNet.BuildReadMcCoreCommand(operateResult.Content, isBit: false) : null)));
			OperateResult<byte[]> operateResult2 = mc.ReadFromCoreServer(send);
			if (!operateResult2.IsSuccess)
			{
				return operateResult2;
			}
			list.AddRange(mc.ExtractActualData(operateResult2.Content, isBit: false));
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
	/// 写入原始的byte数组数据到指定的地址，返回是否写入成功
	/// Write the original byte array data to the specified address, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public static OperateResult Write(IReadWriteMc mc, string address, byte[] value)
	{
		if (address.StartsWith("s=") || address.StartsWith("S="))
		{
			if (mc.McType == McType.McBinary)
			{
				return McBinaryHelper.WriteTag(mc, address.Substring(2), value);
			}
			if (mc.McType == McType.MCAscii)
			{
				return McAsciiHelper.WriteTag(mc, address.Substring(2), value);
			}
		}
		OperateResult<McAddressData> operateResult = mc.McAnalysisAddress(address, 0, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		byte[] send = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildWriteWordCoreCommand(operateResult.Content, value) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildAsciiWriteWordCoreCommand(operateResult.Content, value) : ((mc.McType == McType.McRBinary) ? MelsecMcRNet.BuildWriteWordCoreCommand(operateResult.Content, value) : null)));
		OperateResult<byte[]> operateResult2 = mc.ReadFromCoreServer(send);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 读取单个的数据信息
	/// Read a single  data message
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <returns>带有成功标识的 bool 值</returns>
	public static OperateResult<bool> ReadBool(IReadWriteMc mc, string address)
	{
		return ByteTransformHelper.GetResultFromArray(ReadBool(mc, address, 1));
	}

	/// <summary>
	/// 批量读取数组信息，需要指定地址和长度，返回 数组
	/// Batch read  array information, need to specify the address and length, return  array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的 bool[] 数组</returns>
	public static OperateResult<bool[]> ReadBool(IReadWriteMc mc, string address, ushort length, bool supportWordAdd = true)
	{
		if ((mc.McType == McType.McBinary && address.StartsWith("s=")) || address.StartsWith("S="))
		{
			return McBinaryHelper.ReadBoolTag(mc, address.Substring(2), length);
		}
		if (supportWordAdd && address.IndexOf('.') > 0)
		{
			return HslHelper.ReadBool(mc, address, length);
		}
		OperateResult<McAddressData> operateResult = mc.McAnalysisAddress(address, length, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		List<bool> list = new List<bool>();
		ushort num = 0;
		while (num < length)
		{
			ushort num2 = (ushort)Math.Min(length - num, GetReadBoolLength(mc.McType));
			operateResult.Content.Length = num2;
			byte[] send = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildReadMcCoreCommand(operateResult.Content, isBit: true) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildAsciiReadMcCoreCommand(operateResult.Content, isBit: true) : ((mc.McType == McType.McRBinary) ? MelsecMcRNet.BuildReadMcCoreCommand(operateResult.Content, isBit: true) : null)));
			OperateResult<byte[]> operateResult2 = mc.ReadFromCoreServer(send);
			if (!operateResult2.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(operateResult2);
			}
			list.AddRange((from m in mc.ExtractActualData(operateResult2.Content, isBit: true)
				select m == 1).Take(num2).ToArray());
			num += num2;
			operateResult.Content.AddressStart += num2;
		}
		return OperateResult.CreateSuccessResult(list.ToArray());
	}

	/// <summary>
	/// 批量写入数组数据，返回是否成功
	/// Batch write  array data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public static OperateResult Write(IReadWriteMc mc, string address, bool[] values, bool supportWordAdd = true)
	{
		if ((mc.McType == McType.McBinary && address.StartsWith("s=")) || address.StartsWith("S="))
		{
			return McBinaryHelper.WriteTag(mc, address.Substring(2), values);
		}
		if (supportWordAdd && mc.EnableWriteBitToWordRegister && address.Contains("."))
		{
			return ReadWriteNetHelper.WriteBoolWithWord(mc, address, values);
		}
		OperateResult<McAddressData> operateResult = mc.McAnalysisAddress(address, 0, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		byte[] send = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildWriteBitCoreCommand(operateResult.Content, values) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildAsciiWriteBitCoreCommand(operateResult.Content, values) : ((mc.McType == McType.McRBinary) ? MelsecMcRNet.BuildWriteBitCoreCommand(operateResult.Content, values) : null)));
		OperateResult<byte[]> operateResult2 = mc.ReadFromCoreServer(send);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
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
	public static async Task<OperateResult<byte[]>> ReadAsync(IReadWriteMc mc, string address, ushort length)
	{
		if ((mc.McType == McType.McBinary && address.StartsWith("s=")) || address.StartsWith("S="))
		{
			return await McBinaryHelper.ReadTagsAsync(mc, new string[1] { address.Substring(2) }, new ushort[1] { length });
		}
		if ((mc.McType == McType.McBinary || mc.McType == McType.MCAscii) && Regex.IsMatch(address, "ext=[0-9]+;", RegexOptions.IgnoreCase))
		{
			string extStr = Regex.Match(address, "ext=[0-9]+;").Value;
			ushort ext = ushort.Parse(Regex.Match(extStr, "[0-9]+").Value);
			return await ReadExtendAsync(mc, ext, address.Substring(extStr.Length), length);
		}
		if ((mc.McType == McType.McBinary || mc.McType == McType.MCAscii) && Regex.IsMatch(address, "mem=", RegexOptions.IgnoreCase))
		{
			return await ReadMemoryAsync(mc, address.Substring(4), length);
		}
		if ((mc.McType == McType.McBinary || mc.McType == McType.MCAscii) && Regex.IsMatch(address, "module=[0-9]+;", RegexOptions.IgnoreCase))
		{
			string moduleStr = Regex.Match(address, "module=[0-9]+;").Value;
			ushort module = ushort.Parse(Regex.Match(moduleStr, "[0-9]+").Value);
			return await ReadSmartModuleAsync(mc, module, address.Substring(moduleStr.Length), (ushort)(length * 2));
		}
		OperateResult<McAddressData> addressResult = mc.McAnalysisAddress(address, length, isBit: false);
		if (!addressResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(addressResult);
		}
		List<byte> bytesContent = new List<byte>();
		ushort alreadyFinished = 0;
		while (alreadyFinished < length)
		{
			ushort readLength = (ushort)Math.Min(length - alreadyFinished, GetReadWordLength(mc.McType));
			addressResult.Content.Length = readLength;
			byte[] command = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildReadMcCoreCommand(addressResult.Content, isBit: false) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildAsciiReadMcCoreCommand(addressResult.Content, isBit: false) : ((mc.McType == McType.McRBinary) ? MelsecMcRNet.BuildReadMcCoreCommand(addressResult.Content, isBit: false) : null)));
			OperateResult<byte[]> read = await mc.ReadFromCoreServerAsync(command);
			if (!read.IsSuccess)
			{
				return read;
			}
			bytesContent.AddRange(mc.ExtractActualData(read.Content, isBit: false));
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
	/// 写入原始的byte数组数据到指定的地址，返回是否写入成功
	/// Write the original byte array data to the specified address, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public static async Task<OperateResult> WriteAsync(IReadWriteMc mc, string address, byte[] value)
	{
		if ((mc.McType == McType.McBinary && address.StartsWith("s=")) || address.StartsWith("S="))
		{
			return await McBinaryHelper.WriteTagAsync(mc, address.Substring(2), value);
		}
		OperateResult<McAddressData> addressResult = mc.McAnalysisAddress(address, 0, isBit: false);
		if (!addressResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(addressResult);
		}
		byte[] coreResult = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildWriteWordCoreCommand(addressResult.Content, value) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildAsciiWriteWordCoreCommand(addressResult.Content, value) : ((mc.McType == McType.McRBinary) ? MelsecMcRNet.BuildWriteWordCoreCommand(addressResult.Content, value) : null)));
		OperateResult<byte[]> read = await mc.ReadFromCoreServerAsync(coreResult);
		if (!read.IsSuccess)
		{
			return read;
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
	public static async Task<OperateResult<bool[]>> ReadBoolAsync(IReadWriteMc mc, string address, ushort length, bool supportWordAdd = true)
	{
		if ((mc.McType == McType.McBinary && address.StartsWith("s=")) || address.StartsWith("S="))
		{
			return await McBinaryHelper.ReadBoolTagAsync(mc, address.Substring(2), length);
		}
		if (supportWordAdd && address.IndexOf('.') > 0)
		{
			return await HslHelper.ReadBoolAsync(mc, address, length);
		}
		OperateResult<McAddressData> addressResult = mc.McAnalysisAddress(address, length, isBit: true);
		if (!addressResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(addressResult);
		}
		List<bool> boolContent = new List<bool>();
		ushort alreadyFinished = 0;
		while (alreadyFinished < length)
		{
			ushort readLength = (ushort)Math.Min(length - alreadyFinished, GetReadBoolLength(mc.McType));
			addressResult.Content.Length = readLength;
			byte[] coreResult = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildReadMcCoreCommand(addressResult.Content, isBit: true) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildAsciiReadMcCoreCommand(addressResult.Content, isBit: true) : ((mc.McType == McType.McRBinary) ? MelsecMcRNet.BuildReadMcCoreCommand(addressResult.Content, isBit: true) : null)));
			OperateResult<byte[]> read = await mc.ReadFromCoreServerAsync(coreResult);
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(read);
			}
			boolContent.AddRange((from m in mc.ExtractActualData(read.Content, isBit: true)
				select m == 1).Take(readLength).ToArray());
			alreadyFinished += readLength;
			addressResult.Content.AddressStart += readLength;
		}
		return OperateResult.CreateSuccessResult(boolContent.ToArray());
	}

	/// <summary>
	/// 批量写入数组数据，返回是否成功
	/// Batch write  array data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public static async Task<OperateResult> WriteAsync(IReadWriteMc mc, string address, bool[] values, bool supportWordAdd = true)
	{
		if ((mc.McType == McType.McBinary && address.StartsWith("s=")) || address.StartsWith("S="))
		{
			return await McBinaryHelper.WriteTagAsync(mc, address.Substring(2), values);
		}
		if (supportWordAdd && mc.EnableWriteBitToWordRegister && address.Contains("."))
		{
			return await ReadWriteNetHelper.WriteBoolWithWordAsync(mc, address, values).ConfigureAwait(continueOnCapturedContext: false);
		}
		OperateResult<McAddressData> addressResult = mc.McAnalysisAddress(address, 0, isBit: true);
		if (!addressResult.IsSuccess)
		{
			return addressResult;
		}
		byte[] coreResult = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildWriteBitCoreCommand(addressResult.Content, values) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildAsciiWriteBitCoreCommand(addressResult.Content, values) : ((mc.McType == McType.McRBinary) ? MelsecMcRNet.BuildWriteBitCoreCommand(addressResult.Content, values) : null)));
		OperateResult<byte[]> read = await mc.ReadFromCoreServerAsync(coreResult);
		if (!read.IsSuccess)
		{
			return read;
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 随机读取PLC的数据信息，可以跨地址，跨类型组合，但是每个地址只能读取一个word，也就是2个字节的内容。收到结果后，需要自行解析数据
	/// Randomly read PLC data information, which can be combined across addresses and types, but each address can only read one word,
	/// which is the content of 2 bytes. After receiving the results, you need to parse the data yourself
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <param name="address">所有的地址的集合</param>
	/// <returns>结果</returns>
	/// <remarks>
	/// 访问安装有 Q 系列 C24/E71 的站 QCPU 上位站 经由 Q 系列兼容网络系统 MELSECNET/H MELSECNET/10 Ethernet 的 QCPU 其他站 时
	/// 访问点数········1≦ 字访问点数 双字访问点数 ≦192
	/// 访问 QnACPU 其他站 经由 QnA 系列兼容网络系统 MELSECNET/10 Ethernet 的 Q/QnACPU 其他站 时访问点数········1≦ 字访问点数 双字访问点数 ≦96
	/// 访问上述以外的 PLC CPU 其他站 时访问点数········1≦字访问点数≦10
	/// </remarks>
	public static OperateResult<byte[]> ReadRandom(IReadWriteMc mc, string[] address)
	{
		McAddressData[] array = new McAddressData[address.Length];
		for (int i = 0; i < address.Length; i++)
		{
			OperateResult<McAddressData> operateResult = McAddressData.ParseMelsecFrom(address[i], 1, isBit: false);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult);
			}
			array[i] = operateResult.Content;
		}
		List<byte[]> list = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildReadRandomWordCommand(array) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildAsciiReadRandomWordCommand(array) : ((mc.McType == McType.McRBinary) ? MelsecMcRNet.BuildReadRandomWordCommand(array) : null)));
		List<byte> list2 = new List<byte>();
		for (int j = 0; j < list.Count; j++)
		{
			OperateResult<byte[]> operateResult2 = mc.ReadFromCoreServer(list[j]);
			if (!operateResult2.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult2);
			}
			list2.AddRange(mc.ExtractActualData(operateResult2.Content, isBit: false));
		}
		return OperateResult.CreateSuccessResult(list2.ToArray());
	}

	/// <summary>
	/// 使用块读取PLC的数据信息，可以跨地址，跨类型组合，每个地址是任意的长度。收到结果后，需要自行解析数据，目前只支持字地址，比如D区，W区，R区，不支持X，Y，M，B，L等等
	/// Read the data information of the PLC randomly. It can be combined across addresses and types. Each address is of any length. After receiving the results,
	/// you need to parse the data yourself. Currently, only word addresses are supported, such as D area, W area, R area. X, Y, M, B, L, etc
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <param name="address">所有的地址的集合</param>
	/// <param name="length">每个地址的长度信息</param>
	/// <returns>结果</returns>
	/// <remarks>
	/// 实际测试不一定所有的plc都可以读取成功，具体情况需要具体分析
	/// 1 块数按照下列要求指定 120 ≧ 字软元件块数 + 位软元件块数
	/// 2 各软元件点数按照下列要求指定 960 ≧ 字软元件各块的合计点数 + 位软元件各块的合计点数
	/// </remarks>
	public static OperateResult<byte[]> ReadRandom(IReadWriteMc mc, string[] address, ushort[] length)
	{
		if (length.Length != address.Length)
		{
			return new OperateResult<byte[]>(StringResources.Language.TwoParametersLengthIsNotSame);
		}
		McAddressData[] array = new McAddressData[address.Length];
		for (int i = 0; i < address.Length; i++)
		{
			OperateResult<McAddressData> operateResult = McAddressData.ParseMelsecFrom(address[i], length[i], isBit: false);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult);
			}
			array[i] = operateResult.Content;
		}
		byte[] send = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildReadRandomCommand(array) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildAsciiReadRandomCommand(array) : ((mc.McType == McType.McRBinary) ? MelsecMcRNet.BuildReadRandomCommand(array) : null)));
		OperateResult<byte[]> operateResult2 = mc.ReadFromCoreServer(send);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(mc.ExtractActualData(operateResult2.Content, isBit: false));
	}

	/// <summary>
	/// 随机字写入数据到PLC里，可以跨地址写入，指定一个字符串数组，一个地址需要对应2个字节，所以写入的字节长度为地址两倍。
	/// Randomly write data into the PLC. It can be written across addresses. Specify a string array. One address needs to correspond to 2 bytes, so the length of the written bytes is twice the address.
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <param name="address">所有的地址集合</param>
	/// <param name="data">写入数据</param>
	/// <returns>是否写入成功</returns>
	public static OperateResult WriteRandom(IReadWriteMc mc, string[] address, byte[] data)
	{
		if (data.Length != address.Length * 2)
		{
			return new OperateResult<byte[]>(StringResources.Language.TwoParametersLengthIsNotSame);
		}
		McAddressData[] array = new McAddressData[address.Length];
		for (int i = 0; i < address.Length; i++)
		{
			OperateResult<McAddressData> operateResult = McAddressData.ParseMelsecFrom(address[i], 0, isBit: false);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult);
			}
			array[i] = operateResult.Content;
		}
		byte[] send = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildWriteRandomWordCommand(array, data) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildWriteRandomWordCommand(array, data) : ((mc.McType == McType.McRBinary) ? MelsecMcRNet.BuildWriteRandomWordCommand(array, data) : null)));
		OperateResult<byte[]> operateResult2 = mc.ReadFromCoreServer(send);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(mc.ExtractActualData(operateResult2.Content, isBit: false));
	}

	/// <summary>
	/// 随机读取PLC的数据信息，可以跨地址，跨类型组合，但是每个地址只能读取一个word，也就是2个字节的内容。收到结果后，自动转换为了short类型的数组
	/// Randomly read PLC data information, which can be combined across addresses and types, but each address can only read one word,
	/// which is the content of 2 bytes. After receiving the result, it is automatically converted to an array of type short.
	/// </summary>
	/// <param name="mc">MC协议的通信对象</param>
	/// <param name="address">所有的地址的集合</param>
	/// <returns>包含是否成功的结果对象</returns>
	/// <remarks>
	/// 访问安装有 Q 系列 C24/E71 的站 QCPU 上位站 经由 Q 系列兼容网络系统 MELSECNET/H MELSECNET/10 Ethernet 的 QCPU 其他站 时
	/// 访问点数········1≦ 字访问点数 双字访问点数 ≦192
	/// 访问 QnACPU 其他站 经由 QnA 系列兼容网络系统 MELSECNET/10 Ethernet 的 Q/QnACPU 其他站 时访问点数········1≦ 字访问点数 双字访问点数 ≦96
	/// 访问上述以外的 PLC CPU 其他站 时访问点数········1≦字访问点数≦10
	/// </remarks>
	public static OperateResult<short[]> ReadRandomInt16(IReadWriteMc mc, string[] address)
	{
		OperateResult<byte[]> operateResult = ReadRandom(mc, address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<short[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(mc.ByteTransform.TransInt16(operateResult.Content, 0, address.Length));
	}

	/// <summary>
	/// 随机读取PLC的数据信息，可以跨地址，跨类型组合，但是每个地址只能读取一个word，也就是2个字节的内容。收到结果后，自动转换为了ushort类型的数组
	/// Randomly read PLC data information, which can be combined across addresses and types, but each address can only read one word,
	/// which is the content of 2 bytes. After receiving the result, it is automatically converted to an array of type ushort.
	/// </summary>
	/// <param name="mc">MC协议的通信对象</param>
	/// <param name="address">所有的地址的集合</param>
	/// <returns>包含是否成功的结果对象</returns>
	/// <remarks>
	/// 访问安装有 Q 系列 C24/E71 的站 QCPU 上位站 经由 Q 系列兼容网络系统 MELSECNET/H MELSECNET/10 Ethernet 的 QCPU 其他站 时
	/// 访问点数········1≦ 字访问点数 双字访问点数 ≦192
	/// 访问 QnACPU 其他站 经由 QnA 系列兼容网络系统 MELSECNET/10 Ethernet 的 Q/QnACPU 其他站 时访问点数········1≦ 字访问点数 双字访问点数 ≦96
	/// 访问上述以外的 PLC CPU 其他站 时访问点数········1≦字访问点数≦10
	/// </remarks>
	public static OperateResult<ushort[]> ReadRandomUInt16(IReadWriteMc mc, string[] address)
	{
		OperateResult<byte[]> operateResult = ReadRandom(mc, address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<ushort[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(mc.ByteTransform.TransUInt16(operateResult.Content, 0, address.Length));
	}

	/// <summary>
	/// 随机读取PLC的数据信息，可以跨地址，跨类型组合，但是每个地址只能读取一个word，也就是2个字节的内容。收到结果后，需要自行解析数据
	/// Randomly read PLC data information, which can be combined across addresses and types, but each address can only read one word,
	/// which is the content of 2 bytes. After receiving the results, you need to parse the data yourself
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <param name="address">所有的地址的集合</param>
	/// <returns>结果</returns>
	/// <remarks>
	/// 访问安装有 Q 系列 C24/E71 的站 QCPU 上位站 经由 Q 系列兼容网络系统 MELSECNET/H MELSECNET/10 Ethernet 的 QCPU 其他站 时
	/// 访问点数········1≦ 字访问点数 双字访问点数 ≦192
	/// 访问 QnACPU 其他站 经由 QnA 系列兼容网络系统 MELSECNET/10 Ethernet 的 Q/QnACPU 其他站 时访问点数········1≦ 字访问点数 双字访问点数 ≦96
	/// 访问上述以外的 PLC CPU 其他站 时访问点数········1≦字访问点数≦10
	/// </remarks>
	public static async Task<OperateResult<byte[]>> ReadRandomAsync(IReadWriteMc mc, string[] address)
	{
		McAddressData[] mcAddressDatas = new McAddressData[address.Length];
		for (int i = 0; i < address.Length; i++)
		{
			OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom(address[i], 1, isBit: false);
			if (!addressResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(addressResult);
			}
			mcAddressDatas[i] = addressResult.Content;
		}
		List<byte[]> coreResult = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildReadRandomWordCommand(mcAddressDatas) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildAsciiReadRandomWordCommand(mcAddressDatas) : ((mc.McType == McType.McRBinary) ? MelsecMcRNet.BuildReadRandomWordCommand(mcAddressDatas) : null)));
		List<byte> array = new List<byte>();
		for (int j = 0; j < coreResult.Count; j++)
		{
			OperateResult<byte[]> read = await mc.ReadFromCoreServerAsync(coreResult[j]);
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(read);
			}
			array.AddRange(mc.ExtractActualData(read.Content, isBit: false));
		}
		return OperateResult.CreateSuccessResult(array.ToArray());
	}

	/// <summary>
	/// 随机字写入数据到PLC里，可以跨地址写入，指定一个字符串数组，一个地址需要对应2个字节，所以写入的字节长度为地址两倍。
	/// Randomly write data into the PLC. It can be written across addresses. Specify a string array. One address needs to correspond to 2 bytes, so the length of the written bytes is twice the address.
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <param name="address">所有的地址集合</param>
	/// <param name="data">写入数据</param>
	/// <returns>是否写入成功</returns>
	public static async Task<OperateResult> WriteRandomAsync(IReadWriteMc mc, string[] address, byte[] data)
	{
		if (data.Length != address.Length * 2)
		{
			return new OperateResult<byte[]>(StringResources.Language.TwoParametersLengthIsNotSame);
		}
		McAddressData[] mcAddressDatas = new McAddressData[address.Length];
		for (int i = 0; i < address.Length; i++)
		{
			OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom(address[i], 0, isBit: false);
			if (!addressResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(addressResult);
			}
			mcAddressDatas[i] = addressResult.Content;
		}
		byte[] coreResult = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildWriteRandomWordCommand(mcAddressDatas, data) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildWriteRandomWordCommand(mcAddressDatas, data) : ((mc.McType == McType.McRBinary) ? MelsecMcRNet.BuildWriteRandomWordCommand(mcAddressDatas, data) : null)));
		OperateResult<byte[]> read = await mc.ReadFromCoreServerAsync(coreResult);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return OperateResult.CreateSuccessResult(mc.ExtractActualData(read.Content, isBit: false));
	}

	/// <summary>
	/// 使用块读取PLC的数据信息，可以跨地址，跨类型组合，每个地址是任意的长度。收到结果后，需要自行解析数据，目前只支持字地址，比如D区，W区，R区，不支持X，Y，M，B，L等等
	/// Read the data information of the PLC randomly. It can be combined across addresses and types. Each address is of any length. After receiving the results,
	/// you need to parse the data yourself. Currently, only word addresses are supported, such as D area, W area, R area. X, Y, M, B, L, etc
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <param name="address">所有的地址的集合</param>
	/// <param name="length">每个地址的长度信息</param>
	/// <returns>结果</returns>
	/// <remarks>
	/// 实际测试不一定所有的plc都可以读取成功，具体情况需要具体分析
	/// 1 块数按照下列要求指定 120 ≧ 字软元件块数 + 位软元件块数
	/// 2 各软元件点数按照下列要求指定 960 ≧ 字软元件各块的合计点数 + 位软元件各块的合计点数
	/// </remarks>
	public static async Task<OperateResult<byte[]>> ReadRandomAsync(IReadWriteMc mc, string[] address, ushort[] length)
	{
		if (length.Length != address.Length)
		{
			return new OperateResult<byte[]>(StringResources.Language.TwoParametersLengthIsNotSame);
		}
		McAddressData[] mcAddressDatas = new McAddressData[address.Length];
		for (int i = 0; i < address.Length; i++)
		{
			OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom(address[i], length[i], isBit: false);
			if (!addressResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(addressResult);
			}
			mcAddressDatas[i] = addressResult.Content;
		}
		byte[] coreResult = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildReadRandomCommand(mcAddressDatas) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildAsciiReadRandomCommand(mcAddressDatas) : ((mc.McType == McType.McRBinary) ? MelsecMcRNet.BuildReadRandomCommand(mcAddressDatas) : null)));
		OperateResult<byte[]> read = await mc.ReadFromCoreServerAsync(coreResult);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return OperateResult.CreateSuccessResult(mc.ExtractActualData(read.Content, isBit: false));
	}

	/// <summary>
	/// 随机读取PLC的数据信息，可以跨地址，跨类型组合，但是每个地址只能读取一个word，也就是2个字节的内容。收到结果后，自动转换为了short类型的数组
	/// Randomly read PLC data information, which can be combined across addresses and types, but each address can only read one word,
	/// which is the content of 2 bytes. After receiving the result, it is automatically converted to an array of type short.
	/// </summary>
	/// <param name="mc">MC协议的通信对象</param>
	/// <param name="address">所有的地址的集合</param>
	/// <returns>包含是否成功的结果对象</returns>
	/// <remarks>
	/// 访问安装有 Q 系列 C24/E71 的站 QCPU 上位站 经由 Q 系列兼容网络系统 MELSECNET/H MELSECNET/10 Ethernet 的 QCPU 其他站 时
	/// 访问点数········1≦ 字访问点数 双字访问点数 ≦192
	/// 访问 QnACPU 其他站 经由 QnA 系列兼容网络系统 MELSECNET/10 Ethernet 的 Q/QnACPU 其他站 时访问点数········1≦ 字访问点数 双字访问点数 ≦96
	/// 访问上述以外的 PLC CPU 其他站 时访问点数········1≦字访问点数≦10
	/// </remarks>
	public static async Task<OperateResult<short[]>> ReadRandomInt16Async(IReadWriteMc mc, string[] address)
	{
		OperateResult<byte[]> read = await ReadRandomAsync(mc, address);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<short[]>(read);
		}
		return OperateResult.CreateSuccessResult(mc.ByteTransform.TransInt16(read.Content, 0, address.Length));
	}

	/// <summary>
	/// 随机读取PLC的数据信息，可以跨地址，跨类型组合，但是每个地址只能读取一个word，也就是2个字节的内容。收到结果后，自动转换为了ushort类型的数组
	/// Randomly read PLC data information, which can be combined across addresses and types, but each address can only read one word,
	/// which is the content of 2 bytes. After receiving the result, it is automatically converted to an array of type ushort.
	/// </summary>
	/// <param name="mc">MC协议的通信对象</param>
	/// <param name="address">所有的地址的集合</param>
	/// <returns>包含是否成功的结果对象</returns>
	/// <remarks>
	/// 访问安装有 Q 系列 C24/E71 的站 QCPU 上位站 经由 Q 系列兼容网络系统 MELSECNET/H MELSECNET/10 Ethernet 的 QCPU 其他站 时
	/// 访问点数········1≦ 字访问点数 双字访问点数 ≦192
	/// 访问 QnACPU 其他站 经由 QnA 系列兼容网络系统 MELSECNET/10 Ethernet 的 Q/QnACPU 其他站 时访问点数········1≦ 字访问点数 双字访问点数 ≦96
	/// 访问上述以外的 PLC CPU 其他站 时访问点数········1≦字访问点数≦10
	/// </remarks>
	public static async Task<OperateResult<ushort[]>> ReadRandomUInt16Async(IReadWriteMc mc, string[] address)
	{
		OperateResult<byte[]> read = await ReadRandomAsync(mc, address);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<ushort[]>(read);
		}
		return OperateResult.CreateSuccessResult(mc.ByteTransform.TransUInt16(read.Content, 0, address.Length));
	}

	/// <summary>
	/// [商业授权] 读取缓冲寄存器的数据信息，地址直接为偏移地址
	/// [Authorization] Read the data information of the buffer register, the address is directly the offset address
	/// </summary>
	/// <param name="mc">MC通信对象</param>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>读取的内容</returns>
	/// <remarks>
	/// 本指令不可以访问下述缓冲存储器:
	/// 1. 本站(SLMP对应设备)上安装的智能功能模块
	/// 2. 其它站缓冲存储器
	/// </remarks>
	public static OperateResult<byte[]> ReadMemory(IReadWriteMc mc, string address, ushort length)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> operateResult = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildReadMemoryCommand(address, length) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildAsciiReadMemoryCommand(address, length) : null));
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = mc.ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(mc.ExtractActualData(operateResult2.Content, isBit: false));
	}

	/// <summary>
	/// [商业授权] 读取缓冲寄存器的数据信息，地址直接为偏移地址
	/// [Authorization] Read the data information of the buffer register, the address is directly the offset address
	/// </summary>
	/// <param name="mc">MC通信对象</param>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>读取的内容</returns>
	/// <remarks>
	/// 本指令不可以访问下述缓冲存储器:
	/// 1. 本站(SLMP对应设备)上安装的智能功能模块
	/// 2. 其它站缓冲存储器
	/// </remarks>
	public static async Task<OperateResult<byte[]>> ReadMemoryAsync(IReadWriteMc mc, string address, ushort length)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> coreResult = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildReadMemoryCommand(address, length) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildAsciiReadMemoryCommand(address, length) : null));
		if (!coreResult.IsSuccess)
		{
			return coreResult;
		}
		OperateResult<byte[]> read = await mc.ReadFromCoreServerAsync(coreResult.Content);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return OperateResult.CreateSuccessResult(mc.ExtractActualData(read.Content, isBit: false));
	}

	/// <summary>
	/// [商业授权] 读取智能模块的数据信息，需要指定模块地址，偏移地址，读取的字节长度
	/// [Authorization] To read the extended data information, you need to enter the extended value information in addition to the original address and length information
	/// </summary>
	/// <param name="mc">MC通信对象</param>
	/// <param name="module">模块地址</param>
	/// <param name="address">地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>返回结果</returns>
	public static OperateResult<byte[]> ReadSmartModule(IReadWriteMc mc, ushort module, string address, ushort length)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> operateResult = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildReadSmartModule(module, address, length) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildAsciiReadSmartModule(module, address, length) : null));
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = mc.ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(mc.ExtractActualData(operateResult2.Content, isBit: false));
	}

	/// <summary>
	/// [商业授权] 读取智能模块的数据信息，需要指定模块地址，偏移地址，读取的字节长度
	/// [Authorization] To read the extended data information, you need to enter the extended value information in addition to the original address and length information
	/// </summary>
	/// <param name="mc">MC通信对象</param>
	/// <param name="module">模块地址</param>
	/// <param name="address">地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>返回结果</returns>
	public static async Task<OperateResult<byte[]>> ReadSmartModuleAsync(IReadWriteMc mc, ushort module, string address, ushort length)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> coreResult = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildReadSmartModule(module, address, length) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildAsciiReadSmartModule(module, address, length) : null));
		if (!coreResult.IsSuccess)
		{
			return coreResult;
		}
		OperateResult<byte[]> read = await mc.ReadFromCoreServerAsync(coreResult.Content);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return OperateResult.CreateSuccessResult(mc.ExtractActualData(read.Content, isBit: false));
	}

	/// <summary>
	/// [商业授权] 读取扩展的数据信息，需要在原有的地址，长度信息之外，输入扩展值信息
	/// [Authorization] To read the extended data information, you need to enter the extended value information in addition to the original address and length information
	/// </summary>
	/// <param name="mc">MC通信对象</param>
	/// <param name="extend">扩展信息</param>
	/// <param name="address">地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>返回结果</returns>
	public static OperateResult<byte[]> ReadExtend(IReadWriteMc mc, ushort extend, string address, ushort length)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<McAddressData> operateResult = mc.McAnalysisAddress(address, length, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		byte[] send = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildReadMcCoreExtendCommand(operateResult.Content, extend, isBit: false) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildAsciiReadMcCoreExtendCommand(operateResult.Content, extend, isBit: false) : null));
		OperateResult<byte[]> operateResult2 = mc.ReadFromCoreServer(send);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(mc.ExtractActualData(operateResult2.Content, isBit: false));
	}

	/// <summary>
	/// [商业授权] 读取扩展的数据信息，需要在原有的地址，长度信息之外，输入扩展值信息
	/// [Authorization] To read the extended data information, you need to enter the extended value information in addition to the original address and length information
	/// </summary>
	/// <param name="mc">MC通信对象</param>
	/// <param name="extend">扩展信息</param>
	/// <param name="address">地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>返回结果</returns>
	public static async Task<OperateResult<byte[]>> ReadExtendAsync(IReadWriteMc mc, ushort extend, string address, ushort length)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<McAddressData> addressResult = mc.McAnalysisAddress(address, length, isBit: false);
		if (!addressResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(addressResult);
		}
		byte[] coreResult = ((mc.McType == McType.McBinary) ? McBinaryHelper.BuildReadMcCoreExtendCommand(addressResult.Content, extend, isBit: false) : ((mc.McType == McType.MCAscii) ? McAsciiHelper.BuildAsciiReadMcCoreExtendCommand(addressResult.Content, extend, isBit: false) : null));
		OperateResult<byte[]> read = await mc.ReadFromCoreServerAsync(coreResult);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		return OperateResult.CreateSuccessResult(mc.ExtractActualData(read.Content, isBit: false));
	}

	/// <summary>
	/// 远程Run操作
	/// Remote Run Operation
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>是否成功</returns>
	public static OperateResult RemoteRun(IReadWriteMc mc)
	{
		return (mc.McType == McType.McBinary) ? mc.ReadFromCoreServer(new byte[8] { 1, 16, 0, 0, 1, 0, 0, 0 }) : ((mc.McType == McType.MCAscii) ? mc.ReadFromCoreServer(Encoding.ASCII.GetBytes("1001000000010000")) : new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction));
	}

	/// <summary>
	/// 远程Stop操作
	/// Remote Stop operation
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>是否成功</returns>
	public static OperateResult RemoteStop(IReadWriteMc mc)
	{
		return (mc.McType == McType.McBinary) ? mc.ReadFromCoreServer(new byte[6] { 2, 16, 0, 0, 1, 0 }) : ((mc.McType == McType.MCAscii) ? mc.ReadFromCoreServer(Encoding.ASCII.GetBytes("100200000001")) : new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction));
	}

	/// <summary>
	/// 远程Reset操作
	/// Remote Reset Operation
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>是否成功</returns>
	public static OperateResult RemoteReset(IReadWriteMc mc)
	{
		return (mc.McType == McType.McBinary) ? mc.ReadFromCoreServer(new byte[6] { 6, 16, 0, 0, 1, 0 }) : ((mc.McType == McType.MCAscii) ? mc.ReadFromCoreServer(Encoding.ASCII.GetBytes("100600000001")) : new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction));
	}

	/// <summary>
	/// 读取PLC的型号信息，例如 Q02HCPU
	/// Read PLC model information, such as Q02HCPU
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>返回型号的结果对象</returns>
	public static OperateResult<string> ReadPlcType(IReadWriteMc mc)
	{
		OperateResult<byte[]> operateResult = ((mc.McType == McType.McBinary) ? mc.ReadFromCoreServer(new byte[4] { 1, 1, 0, 0 }) : ((mc.McType == McType.MCAscii) ? mc.ReadFromCoreServer(Encoding.ASCII.GetBytes("01010000")) : new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction)));
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		return OperateResult.CreateSuccessResult(Encoding.ASCII.GetString(operateResult.Content, 0, 16).TrimEnd());
	}

	/// <summary>
	/// LED 熄灭 出错代码初始化
	/// LED off Error code initialization
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>是否成功</returns>
	public static OperateResult ErrorStateReset(IReadWriteMc mc)
	{
		return (mc.McType == McType.McBinary) ? mc.ReadFromCoreServer(new byte[4] { 23, 22, 0, 0 }) : ((mc.McType == McType.MCAscii) ? mc.ReadFromCoreServer(Encoding.ASCII.GetBytes("16170000")) : new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction));
	}

	/// <summary>
	/// 远程Run操作
	/// Remote Run Operation
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>是否成功</returns>
	public static async Task<OperateResult> RemoteRunAsync(IReadWriteMc mc)
	{
		OperateResult<byte[]> result;
		if (mc.McType == McType.McBinary)
		{
			result = await mc.ReadFromCoreServerAsync(new byte[8] { 1, 16, 0, 0, 1, 0, 0, 0 });
		}
		else
		{
			OperateResult<byte[]> operateResult = ((mc.McType != McType.MCAscii) ? new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction) : (await mc.ReadFromCoreServerAsync(Encoding.ASCII.GetBytes("1001000000010000"))));
			result = operateResult;
		}
		return result;
	}

	/// <summary>
	/// 远程Stop操作
	/// Remote Stop operation
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>是否成功</returns>
	public static async Task<OperateResult> RemoteStopAsync(IReadWriteMc mc)
	{
		OperateResult<byte[]> result;
		if (mc.McType == McType.McBinary)
		{
			result = await mc.ReadFromCoreServerAsync(new byte[6] { 2, 16, 0, 0, 1, 0 });
		}
		else
		{
			OperateResult<byte[]> operateResult = ((mc.McType != McType.MCAscii) ? new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction) : (await mc.ReadFromCoreServerAsync(Encoding.ASCII.GetBytes("100200000001"))));
			result = operateResult;
		}
		return result;
	}

	/// <summary>
	/// 远程Reset操作
	/// Remote Reset Operation
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>是否成功</returns>
	public static async Task<OperateResult> RemoteResetAsync(IReadWriteMc mc)
	{
		OperateResult<byte[]> result;
		if (mc.McType == McType.McBinary)
		{
			result = await mc.ReadFromCoreServerAsync(new byte[6] { 6, 16, 0, 0, 1, 0 });
		}
		else
		{
			OperateResult<byte[]> operateResult = ((mc.McType != McType.MCAscii) ? new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction) : (await mc.ReadFromCoreServerAsync(Encoding.ASCII.GetBytes("100600000001"))));
			result = operateResult;
		}
		return result;
	}

	/// <summary>
	/// 读取PLC的型号信息，例如 Q02HCPU
	/// Read PLC model information, such as Q02HCPU
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>返回型号的结果对象</returns>
	public static async Task<OperateResult<string>> ReadPlcTypeAsync(IReadWriteMc mc)
	{
		OperateResult<byte[]> operateResult;
		if (mc.McType == McType.McBinary)
		{
			operateResult = await mc.ReadFromCoreServerAsync(new byte[4] { 1, 1, 0, 0 });
		}
		else
		{
			OperateResult<byte[]> operateResult2 = ((mc.McType != McType.MCAscii) ? new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction) : (await mc.ReadFromCoreServerAsync(Encoding.ASCII.GetBytes("01010000"))));
			operateResult = operateResult2;
		}
		OperateResult<byte[]> read = operateResult;
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(read);
		}
		return OperateResult.CreateSuccessResult(Encoding.ASCII.GetString(read.Content, 0, 16).TrimEnd());
	}

	/// <summary>
	/// LED 熄灭 出错代码初始化
	/// LED off Error code initialization
	/// </summary>
	/// <param name="mc">MC协议通信对象</param>
	/// <returns>是否成功</returns>
	public static async Task<OperateResult> ErrorStateResetAsync(IReadWriteMc mc)
	{
		OperateResult<byte[]> result;
		if (mc.McType == McType.McBinary)
		{
			result = await mc.ReadFromCoreServerAsync(new byte[4] { 23, 22, 0, 0 });
		}
		else
		{
			OperateResult<byte[]> operateResult = ((mc.McType != McType.MCAscii) ? new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction) : (await mc.ReadFromCoreServerAsync(Encoding.ASCII.GetBytes("16170000"))));
			result = operateResult;
		}
		return result;
	}
}
