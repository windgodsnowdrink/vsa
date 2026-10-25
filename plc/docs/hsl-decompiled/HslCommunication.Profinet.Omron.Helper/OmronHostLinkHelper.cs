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
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;

namespace HslCommunication.Profinet.Omron.Helper;

/// <summary>
/// 欧姆龙的OmronHostLink相关辅助方法
/// </summary>
public class OmronHostLinkHelper
{
	/// <summary>
	/// 验证欧姆龙的Fins-TCP返回的数据是否正确的数据，如果正确的话，并返回所有的数据内容
	/// </summary>
	/// <param name="send">发送的报文信息</param>
	/// <param name="response">来自欧姆龙返回的数据内容</param>
	/// <returns>带有是否成功的结果对象</returns>
	public static OperateResult<byte[]> ResponseValidAnalysis(byte[] send, byte[] response)
	{
		if (response.Length >= 27)
		{
			try
			{
				string message = Encoding.ASCII.GetString(send, 14, 4);
				string message2 = Encoding.ASCII.GetString(response, 15, 4);
				if (message2 != message)
				{
					return new OperateResult<byte[]>("Send Command [" + message + "] not the same as receive command [" + message2 + "] source:[" + SoftBasic.GetAsciiStringRender(response) + "]");
				}
				int num;
				try
				{
					num = Convert.ToInt32(Encoding.ASCII.GetString(response, 19, 4), 16);
				}
				catch (Exception ex)
				{
					return new OperateResult<byte[]>("Get error code failed: " + ex.Message + Environment.NewLine + "Source Data: " + SoftBasic.GetAsciiStringRender(response));
				}
				byte[] buffer = new byte[0];
				if (response.Length > 27)
				{
					buffer = SoftBasic.HexStringToBytes(Encoding.ASCII.GetString(response, 23, response.Length - 27));
				}
				if (num > 0)
				{
					return new OperateResult<byte[]>
					{
						ErrorCode = num,
						Content = buffer,
						Message = GetErrorText(num)
					};
				}
				if (Encoding.ASCII.GetString(response, 15, 4) == "0104")
				{
					byte[] array2 = ((buffer.Length != 0) ? new byte[buffer.Length * 2 / 3] : new byte[0]);
					for (int i = 0; i < buffer.Length / 3; i++)
					{
						array2[i * 2] = buffer[i * 3 + 1];
						array2[i * 2 + 1] = buffer[i * 3 + 2];
					}
					buffer = array2;
				}
				return OperateResult.CreateSuccessResult(buffer);
			}
			catch (Exception ex2)
			{
				return new OperateResult<byte[]>("ResponseValidAnalysis failed: " + ex2.Message + " Source: " + response.ToHexString(' '));
			}
		}
		return new OperateResult<byte[]>(StringResources.Language.OmronReceiveDataError + " Source Data: " + response.ToHexString(' '));
	}

	/// <summary>
	/// 根据错误信息获取当前的文本描述信息
	/// </summary>
	/// <param name="error">错误代号</param>
	/// <returns>文本消息</returns>
	public static string GetErrorText(int error)
	{
		switch (error)
		{
		case 1:
			return "Service was canceled.";
		case 257:
			return "Local node is not participating in the network.";
		case 258:
			return "Token does not arrive.";
		case 259:
			return "Send was not possible during the specified number of retries.";
		case 260:
			return "Cannot send because maximum number of event frames exceeded.";
		case 261:
			return "Node address setting error occurred.";
		case 262:
			return "The same node address has been set twice in the same network.";
		case 513:
			return "The destination node is not in the network.";
		case 514:
			return "There is no Unit with the specified unit address.";
		case 515:
			return "The third node does not exist.";
		case 516:
			return "The destination node is busy.";
		case 517:
			return "The message was destroyed by noise";
		case 769:
			return "An error occurred in the communications controller.";
		case 770:
			return "A CPU error occurred in the destination CPU Unit.";
		case 771:
			return "A response was not returned because an error occurred in the Board.";
		case 772:
			return "The unit number was set incorrectly";
		case 1025:
			return "The Unit/Board does not support the specified command code.";
		case 1026:
			return "The command cannot be executed because the model or version is incorrect";
		case 1281:
			return "The destination network or node address is not set in the routing tables.";
		case 1282:
			return "Relaying is not possible because there are no routing tables";
		case 1283:
			return "There is an error in the routing tables.";
		case 1284:
			return "An attempt was made to send to a network that was over 3 networks away";
		case 4097:
			return "The command is longer than the maximum permissible length.";
		case 4098:
			return "The command is shorter than the minimum permissible length.";
		case 4099:
			return "The designated number of elements differs from the number of write data items.";
		case 4100:
			return "An incorrect format was used.";
		case 4101:
			return "Either the relay table in the local node or the local network table in the relay node is incorrect.";
		case 4353:
			return "The specified word does not exist in the memory area or there is no EM Area.";
		case 4354:
			return "The access size specification is incorrect or an odd word address is specified.";
		case 4355:
			return "The start address in command process is beyond the accessible area";
		case 4356:
			return "The end address in command process is beyond the accessible area.";
		case 4358:
			return "FFFF hex was not specified.";
		case 4361:
			return "A large–small relationship in the elements in the command data is incorrect.";
		case 4363:
			return "The response format is longer than the maximum permissible length.";
		case 4364:
			return "There is an error in one of the parameter settings.";
		case 8194:
			return "The program area is protected.";
		case 8195:
			return "A table has not been registered.";
		case 8196:
			return "The search data does not exist.";
		case 8197:
			return "A non-existing program number has been specified.";
		case 8198:
			return "The file does not exist at the specified file device.";
		case 8199:
			return "A data being compared is not the same.";
		case 8449:
			return "The specified area is read-only.";
		case 8450:
			return "The program area is protected.";
		case 8451:
			return "The file cannot be created because the limit has been exceeded.";
		case 8453:
			return "A non-existing program number has been specified.";
		case 8454:
			return "The file does not exist at the specified file device.";
		case 8455:
			return "A file with the same name already exists in the specified file device.";
		case 8456:
			return "The change cannot be made because doing so would create a problem.";
		case 8705:
		case 8706:
		case 8712:
			return "The mode is incorrect.";
		case 8707:
			return "The PLC is in PROGRAM mode.";
		case 8708:
			return "The PLC is in DEBUG mode.";
		case 8709:
			return "The PLC is in MONITOR mode.";
		case 8710:
			return "The PLC is in RUN mode.";
		case 8711:
			return "The specified node is not the polling node.";
		case 8961:
			return "The specified memory does not exist as a file device.";
		case 8962:
			return "There is no file memory.";
		case 8963:
			return "There is no clock.";
		case 9217:
			return "The data link tables have not been registered or they contain an error.";
		default:
			return StringResources.Language.UnknownError;
		}
	}

	/// <summary>
	/// 将 fins 命令的报文打包成 HostLink 格式的报文信息，打包之后的结果可以直接发送给PLC
	/// Pack the message of the fins command into the message information in the HostLink format, and the packaged result can be sent directly to the PLC
	/// </summary>
	/// <param name="hostLink">HostLink协议的plc通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="cmd">fins命令</param>
	/// <returns>可发送PLC的完整的报文信息</returns>
	public static byte[] PackCommand(IHostLink hostLink, byte station, byte[] cmd)
	{
		cmd = SoftBasic.BytesToAsciiBytes(cmd);
		byte[] buffer = new byte[18 + cmd.Length];
		buffer[0] = 64;
		buffer[1] = SoftBasic.BuildAsciiBytesFrom(station)[0];
		buffer[2] = SoftBasic.BuildAsciiBytesFrom(station)[1];
		buffer[3] = 70;
		buffer[4] = 65;
		buffer[5] = hostLink.ResponseWaitTime;
		buffer[6] = SoftBasic.BuildAsciiBytesFrom(hostLink.ICF)[0];
		buffer[7] = SoftBasic.BuildAsciiBytesFrom(hostLink.ICF)[1];
		buffer[8] = SoftBasic.BuildAsciiBytesFrom(hostLink.DA2)[0];
		buffer[9] = SoftBasic.BuildAsciiBytesFrom(hostLink.DA2)[1];
		buffer[10] = SoftBasic.BuildAsciiBytesFrom(hostLink.SA2)[0];
		buffer[11] = SoftBasic.BuildAsciiBytesFrom(hostLink.SA2)[1];
		buffer[12] = SoftBasic.BuildAsciiBytesFrom(hostLink.SID)[0];
		buffer[13] = SoftBasic.BuildAsciiBytesFrom(hostLink.SID)[1];
		buffer[^2] = 42;
		buffer[^1] = 13;
		cmd.CopyTo(buffer, 14);
		int num = buffer[0];
		for (int i = 1; i < buffer.Length - 4; i++)
		{
			num ^= buffer[i];
		}
		buffer[^4] = SoftBasic.BuildAsciiBytesFrom((byte)num)[0];
		buffer[^3] = SoftBasic.BuildAsciiBytesFrom((byte)num)[1];
		return buffer;
	}

	/// <summary>
	/// 从欧姆龙PLC中读取想要的数据，返回读取结果，读取长度的单位为字，地址格式为"D100","C100","W100","H100","A100"
	/// Read the desired data from the Omron PLC and return the read result. The unit of the read length is word. The address format is "D100", "C100", "W100", "H100", "A100"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">读取地址，格式为&quot;D100&quot;,&quot;C100&quot;,&quot;W100&quot;,&quot;H100&quot;,&quot;A100&quot;</param>
	/// <param name="length">读取的数据长度</param>
	/// <param name="splits">分割信息</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public static OperateResult<byte[]> Read(IHostLink hostLink, string address, ushort length)
	{
		byte station = (byte)HslHelper.ExtractParameter(ref address, "s", hostLink.UnitNumber);
		OperateResult<List<byte[]>> operateResult = OmronFinsNetHelper.BuildReadCommand(hostLink.PlcType, address, length, isBit: false, hostLink.ReadSplits);
		if (!operateResult.IsSuccess)
		{
			return operateResult.ConvertFailed<byte[]>();
		}
		List<byte> list = new List<byte>();
		for (int i = 0; i < operateResult.Content.Count; i++)
		{
			OperateResult<byte[]> operateResult2 = hostLink.ReadFromCoreServer(PackCommand(hostLink, station, operateResult.Content[i]));
			if (!operateResult2.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult2);
			}
			list.AddRange(operateResult2.Content);
		}
		return OperateResult.CreateSuccessResult(list.ToArray());
	}

	/// <summary>
	/// 从欧姆龙PLC中读取多个地址的数据，返回读取结果，每个地址按照字为单位读取，地址格式为"D100","C100","W100","H100","A100"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">从欧姆龙PLC中读取多个地址的数据，返回读取结果，每个地址按照字为单位读取，地址格式为&quot;D100&quot;,&quot;C100&quot;,&quot;W100&quot;,&quot;H100&quot;,&quot;A100&quot;</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public static OperateResult<byte[]> Read(IHostLink hostLink, string[] address)
	{
		byte station = hostLink.UnitNumber;
		if (address != null && address.Length != 0)
		{
			station = (byte)HslHelper.ExtractParameter(ref address[0], "s", hostLink.UnitNumber);
		}
		OperateResult<List<byte[]>> operateResult = OmronFinsNetHelper.BuildReadCommand(address, hostLink.PlcType);
		if (!operateResult.IsSuccess)
		{
			return operateResult.ConvertFailed<byte[]>();
		}
		List<byte> list = new List<byte>();
		for (int i = 0; i < operateResult.Content.Count; i++)
		{
			OperateResult<byte[]> operateResult2 = hostLink.ReadFromCoreServer(PackCommand(hostLink, station, operateResult.Content[i]));
			if (!operateResult2.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult2);
			}
			list.AddRange(operateResult2.Content);
		}
		return OperateResult.CreateSuccessResult(list.ToArray());
	}

	/// <summary>
	/// 向PLC写入数据，数据格式为原始的字节类型，地址格式为"D100","C100","W100","H100","A100"
	/// Write data to PLC, the data format is the original byte type, and the address format is "D100", "C100", "W100", "H100", "A100"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">初始地址</param>
	/// <param name="value">原始的字节数据</param>
	/// <returns>结果</returns>
	public static OperateResult Write(IHostLink hostLink, string address, byte[] value)
	{
		byte station = (byte)HslHelper.ExtractParameter(ref address, "s", hostLink.UnitNumber);
		OperateResult<byte[]> operateResult = OmronFinsNetHelper.BuildWriteWordCommand(hostLink.PlcType, address, value, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = hostLink.ReadFromCoreServer(PackCommand(hostLink, station, operateResult.Content));
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 从欧姆龙PLC中读取想要的数据，返回读取结果，读取长度的单位为字，地址格式为"D100","C100","W100","H100","A100"
	/// Read the desired data from the Omron PLC and return the read result. The unit of the read length is word. The address format is "D100", "C100", "W100", "H100", "A100"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">读取地址，格式为&quot;D100&quot;,&quot;C100&quot;,&quot;W100&quot;,&quot;H100&quot;,&quot;A100&quot;</param>
	/// <param name="length">读取的数据长度</param>
	/// <param name="splits">分割信息</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public static async Task<OperateResult<byte[]>> ReadAsync(IHostLink hostLink, string address, ushort length)
	{
		byte station = (byte)HslHelper.ExtractParameter(ref address, "s", hostLink.UnitNumber);
		OperateResult<List<byte[]>> command = OmronFinsNetHelper.BuildReadCommand(hostLink.PlcType, address, length, isBit: false, hostLink.ReadSplits);
		if (!command.IsSuccess)
		{
			return command.ConvertFailed<byte[]>();
		}
		List<byte> contentArray = new List<byte>();
		for (int i = 0; i < command.Content.Count; i++)
		{
			OperateResult<byte[]> read = await hostLink.ReadFromCoreServerAsync(PackCommand(hostLink, station, command.Content[i]));
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(read);
			}
			contentArray.AddRange(read.Content);
		}
		return OperateResult.CreateSuccessResult(contentArray.ToArray());
	}

	/// <summary>
	/// 向PLC写入数据，数据格式为原始的字节类型，地址格式为"D100","C100","W100","H100","A100"
	/// Write data to PLC, the data format is the original byte type, and the address format is "D100", "C100", "W100", "H100", "A100"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">初始地址</param>
	/// <param name="value">原始的字节数据</param>
	/// <returns>结果</returns>
	public static async Task<OperateResult> WriteAsync(IHostLink hostLink, string address, byte[] value)
	{
		byte station = (byte)HslHelper.ExtractParameter(ref address, "s", hostLink.UnitNumber);
		OperateResult<byte[]> command = OmronFinsNetHelper.BuildWriteWordCommand(hostLink.PlcType, address, value, isBit: false);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await hostLink.ReadFromCoreServerAsync(PackCommand(hostLink, station, command.Content));
		if (!read.IsSuccess)
		{
			return read;
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 从欧姆龙PLC中读取多个地址的数据，返回读取结果，每个地址按照字为单位读取，地址格式为"D100","C100","W100","H100","A100"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">从欧姆龙PLC中读取多个地址的数据，返回读取结果，每个地址按照字为单位读取，地址格式为&quot;D100&quot;,&quot;C100&quot;,&quot;W100&quot;,&quot;H100&quot;,&quot;A100&quot;</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public static async Task<OperateResult<byte[]>> ReadAsync(IHostLink hostLink, string[] address)
	{
		byte station = hostLink.UnitNumber;
		if (address != null && address.Length != 0)
		{
			station = (byte)HslHelper.ExtractParameter(ref address[0], "s", hostLink.UnitNumber);
		}
		OperateResult<List<byte[]>> command = OmronFinsNetHelper.BuildReadCommand(address, hostLink.PlcType);
		if (!command.IsSuccess)
		{
			return command.ConvertFailed<byte[]>();
		}
		List<byte> contentArray = new List<byte>();
		for (int i = 0; i < command.Content.Count; i++)
		{
			OperateResult<byte[]> read = await hostLink.ReadFromCoreServerAsync(PackCommand(hostLink, station, command.Content[i]));
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(read);
			}
			contentArray.AddRange(read.Content);
		}
		return OperateResult.CreateSuccessResult(contentArray.ToArray());
	}

	/// <summary>
	/// 从欧姆龙PLC中批量读取位软元件，地址格式为"D100.0","C100.0","W100.0","H100.0","A100.0"
	/// Read bit devices in batches from Omron PLC with address format "D100.0", "C100.0", "W100.0", "H100.0", "A100.0"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">读取地址，格式为&quot;D100&quot;,&quot;C100&quot;,&quot;W100&quot;,&quot;H100&quot;,&quot;A100&quot;</param>
	/// <param name="length">读取的长度</param>
	/// <param name="splits">分割信息</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public static OperateResult<bool[]> ReadBool(IHostLink hostLink, string address, ushort length)
	{
		byte b = (byte)HslHelper.ExtractParameter(ref address, "s", hostLink.UnitNumber);
		if (address.StartsWith("DR", StringComparison.OrdinalIgnoreCase) || address.StartsWith("IR", StringComparison.OrdinalIgnoreCase))
		{
			if (!address.Contains("."))
			{
				address += ".0";
			}
			string text = ((b != hostLink.UnitNumber) ? $"s={b};" : string.Empty);
			return HslHelper.ReadBool(hostLink, text + address, length, 16, reverseByWord: true);
		}
		OperateResult<List<byte[]>> operateResult = OmronFinsNetHelper.BuildReadCommand(hostLink.PlcType, address, length, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		List<bool> list = new List<bool>();
		for (int i = 0; i < operateResult.Content.Count; i++)
		{
			OperateResult<byte[]> operateResult2 = hostLink.ReadFromCoreServer(PackCommand(hostLink, b, operateResult.Content[i]));
			if (!operateResult2.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(operateResult2);
			}
			if (operateResult2.Content.Length == 0)
			{
				return new OperateResult<bool[]>("Data is empty.");
			}
			list.AddRange(operateResult2.Content.Select((byte m) => m != 0));
		}
		return OperateResult.CreateSuccessResult(list.ToArray());
	}

	/// <summary>
	/// 向PLC中位软元件写入bool数组，返回是否写入成功，比如你写入D100,values[0]对应D100.0，地址格式为"D100.0","C100.0","W100.0","H100.0","A100.0"
	/// Write the bool array to the PLC's median device and return whether the write was successful. For example, if you write D100, values [0] corresponds to D100.0
	/// and the address format is "D100.0", "C100.0", "W100. 0 "," H100.0 "," A100.0 "
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">要写入的数据地址</param>
	/// <param name="values">要写入的实际数据，可以指定任意的长度</param>
	/// <returns>返回写入结果</returns>
	public static OperateResult Write(IHostLink hostLink, string address, bool[] values)
	{
		byte station = (byte)HslHelper.ExtractParameter(ref address, "s", hostLink.UnitNumber);
		if (address.StartsWith("DR", StringComparison.OrdinalIgnoreCase) || address.StartsWith("IR", StringComparison.OrdinalIgnoreCase))
		{
			return new OperateResult("DR and IR address not support bit write");
		}
		OperateResult<byte[]> operateResult = OmronFinsNetHelper.BuildWriteWordCommand(hostLink.PlcType, address, values.Select((bool m) => (byte)(m ? 1 : 0)).ToArray(), isBit: true);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = hostLink.ReadFromCoreServer(PackCommand(hostLink, station, operateResult.Content));
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 从欧姆龙PLC中批量读取位软元件，地址格式为"D100.0","C100.0","W100.0","H100.0","A100.0"
	/// Read bit devices in batches from Omron PLC with address format "D100.0", "C100.0", "W100.0", "H100.0", "A100.0"
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">读取地址，格式为&quot;D100&quot;,&quot;C100&quot;,&quot;W100&quot;,&quot;H100&quot;,&quot;A100&quot;</param>
	/// <param name="length">读取的长度</param>
	/// <param name="splits">分割信息</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public static async Task<OperateResult<bool[]>> ReadBoolAsync(IHostLink hostLink, string address, ushort length)
	{
		byte station = (byte)HslHelper.ExtractParameter(ref address, "s", hostLink.UnitNumber);
		if (address.StartsWith("DR", StringComparison.OrdinalIgnoreCase) || address.StartsWith("IR", StringComparison.OrdinalIgnoreCase))
		{
			if (!address.Contains("."))
			{
				address += ".0";
			}
			string stationAddress = ((station != hostLink.UnitNumber) ? $"s={station};" : string.Empty);
			return await HslHelper.ReadBoolAsync(hostLink, stationAddress + address, length, 16, reverseByWord: true);
		}
		OperateResult<List<byte[]>> command = OmronFinsNetHelper.BuildReadCommand(hostLink.PlcType, address, length, isBit: true);
		if (!command.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(command);
		}
		List<bool> contentArray = new List<bool>();
		for (int i = 0; i < command.Content.Count; i++)
		{
			OperateResult<byte[]> read = await hostLink.ReadFromCoreServerAsync(PackCommand(hostLink, station, command.Content[i]));
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<bool[]>(read);
			}
			contentArray.AddRange(read.Content.Select((byte m) => m != 0));
		}
		return OperateResult.CreateSuccessResult(contentArray.ToArray());
	}

	/// <summary>
	/// 向PLC中位软元件写入bool数组，返回是否写入成功，比如你写入D100,values[0]对应D100.0，地址格式为"D100.0","C100.0","W100.0","H100.0","A100.0"
	/// Write the bool array to the PLC's median device and return whether the write was successful. For example, if you write D100, values [0] corresponds to D100.0
	/// and the address format is "D100.0", "C100.0", "W100. 0 "," H100.0 "," A100.0 "
	/// </summary>
	/// <param name="omron">PLC设备的连接对象</param>
	/// <param name="address">要写入的数据地址</param>
	/// <param name="values">要写入的实际数据，可以指定任意的长度</param>
	/// <returns>返回写入结果</returns>
	public static async Task<OperateResult> WriteAsync(IHostLink hostLink, string address, bool[] values)
	{
		byte station = (byte)HslHelper.ExtractParameter(ref address, "s", hostLink.UnitNumber);
		if (address.StartsWith("DR", StringComparison.OrdinalIgnoreCase) || address.StartsWith("IR", StringComparison.OrdinalIgnoreCase))
		{
			return new OperateResult("DR and IR address not support bit write");
		}
		OperateResult<byte[]> command = OmronFinsNetHelper.BuildWriteWordCommand(hostLink.PlcType, address, values.Select((bool m) => (byte)(m ? 1 : 0)).ToArray(), isBit: true);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await hostLink.ReadFromCoreServerAsync(PackCommand(hostLink, station, command.Content));
		if (!read.IsSuccess)
		{
			return read;
		}
		return OperateResult.CreateSuccessResult();
	}
}
