// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Linq;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Profinet.YASKAWA.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.YASKAWA;

/// <summary>
/// 基于扩展memobus协议的虚拟服务器，支持线圈，输入线圈，保持型寄存器，输入寄存器，保持型寄存器（扩展），输入寄存器（扩展）的读写。对于远程客户端来说，还支持对扩展保持寄存器的随机字读取和写入操作。
/// A virtual server based on the extended memobus protocol, which supports the reading and writing of coils, input coils, holding registers, input registers,
/// holding registers (expansion), input registers (expansion). For remote clients, random word reads and writes to extended hold registers are also supported.
/// </summary>
/// <remarks>
/// 支持的功能码为 01 02 03 04 05 06 08 09 0A 0B 0D 0E 0F 10, 访问方式分为位读写和字读写，位读写：线圈：100; 输入线圈：x=2;100
/// 字读写时，保持型寄存器：100; 输入寄存器：x=4;100  保持型寄存器（扩展）: x=9;100   输入寄存器（扩展）: x=10;100
/// </remarks>
public class MemobusTcpServer : DeviceServer
{
	private SoftBuffer xBuffer;

	private SoftBuffer yBuffer;

	private SoftBuffer inputBuffer;

	private SoftBuffer rBuffer;

	private SoftBuffer rExtBuffer;

	private SoftBuffer inputExtBuffer;

	private SoftBuffer gBuffer;

	private const int DataPoolLength = 65536;

	/// <summary>
	/// 实例化一个memobus协议的PLC的服务器，支持线圈，输入线圈，保持型寄存器，输入寄存器，保持型寄存器（扩展），输入寄存器（扩展）的读写。
	/// </summary>
	public MemobusTcpServer()
	{
		xBuffer = new SoftBuffer(65536);
		yBuffer = new SoftBuffer(65536);
		inputBuffer = new SoftBuffer(131072)
		{
			IsBoolReverseByWord = true
		};
		rBuffer = new SoftBuffer(131072)
		{
			IsBoolReverseByWord = true
		};
		inputExtBuffer = new SoftBuffer(131072)
		{
			IsBoolReverseByWord = true
		};
		rExtBuffer = new SoftBuffer(131072)
		{
			IsBoolReverseByWord = true
		};
		gBuffer = new SoftBuffer(131072)
		{
			IsBoolReverseByWord = true
		};
		base.WordLength = 2;
		base.ByteTransform = new RegularByteTransform(DataFormat.CDAB);
	}

	private OperateResult<SoftBuffer> GetDataAreaFromYokogawaAddress(MemobusAddress address, bool isBit)
	{
		switch (address.SFC)
		{
		case 1:
		case 5:
		case 15:
			return OperateResult.CreateSuccessResult(yBuffer);
		case 2:
			return OperateResult.CreateSuccessResult(xBuffer);
		case 3:
		case 6:
		case 16:
			return OperateResult.CreateSuccessResult(rBuffer);
		case 4:
			return OperateResult.CreateSuccessResult(inputBuffer);
		case 9:
		case 11:
		case 13:
		case 14:
			return OperateResult.CreateSuccessResult(rExtBuffer);
		case 10:
			return OperateResult.CreateSuccessResult(inputExtBuffer);
		default:
			return new OperateResult<SoftBuffer>(StringResources.Language.NotSupportedDataType);
		}
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组
	/// Batch read byte array information, need to specify the address and length, return the original byte array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		if (address.StartsWith(new string[1] { "G" }))
		{
			int num = Convert.ToInt32(address.Substring(1));
			return OperateResult.CreateSuccessResult(gBuffer.GetBytes(num * 2, length * 2));
		}
		OperateResult<MemobusAddress> operateResult = MemobusAddress.ParseFrom(address, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		OperateResult<SoftBuffer> dataAreaFromYokogawaAddress = GetDataAreaFromYokogawaAddress(operateResult.Content, isBit: false);
		if (!dataAreaFromYokogawaAddress.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(dataAreaFromYokogawaAddress);
		}
		return OperateResult.CreateSuccessResult(dataAreaFromYokogawaAddress.Content.GetBytes(operateResult.Content.AddressStart * 2, length * 2));
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 写入原始的byte数组数据到指定的地址，返回是否写入成功
	/// Write the original byte array data to the specified address, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		if (address.StartsWith(new string[1] { "G" }))
		{
			int num = Convert.ToInt32(address.Substring(1));
			gBuffer.SetBytes(value, num * 2);
			return OperateResult.CreateSuccessResult();
		}
		OperateResult<MemobusAddress> operateResult = MemobusAddress.ParseFrom(address, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		OperateResult<SoftBuffer> dataAreaFromYokogawaAddress = GetDataAreaFromYokogawaAddress(operateResult.Content, isBit: false);
		if (!dataAreaFromYokogawaAddress.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(dataAreaFromYokogawaAddress);
		}
		dataAreaFromYokogawaAddress.Content.SetBytes(value, operateResult.Content.AddressStart * 2);
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 批量读取数组信息，需要指定地址和长度，返回 数组
	/// Batch read  array information, need to specify the address and length, return  array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的 bool[] 数组</returns>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		if (address.StartsWith(new string[1] { "G" }))
		{
			int destIndex = MemobusHelper.CalculateBoolIndex(address);
			return OperateResult.CreateSuccessResult(gBuffer.GetBool(destIndex, length));
		}
		OperateResult<MemobusAddress> operateResult = MemobusAddress.ParseFrom(address, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		OperateResult<SoftBuffer> dataAreaFromYokogawaAddress = GetDataAreaFromYokogawaAddress(operateResult.Content, isBit: true);
		if (!dataAreaFromYokogawaAddress.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(dataAreaFromYokogawaAddress);
		}
		if (operateResult.Content.SFC == 3 || operateResult.Content.SFC == 4)
		{
			return OperateResult.CreateSuccessResult(dataAreaFromYokogawaAddress.Content.GetBool(operateResult.Content.AddressStart, length));
		}
		return OperateResult.CreateSuccessResult((from m in dataAreaFromYokogawaAddress.Content.GetBytes(operateResult.Content.AddressStart, length)
			select m != 0).ToArray());
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// 批量写入数组数据，返回是否成功
	/// Batch write  array data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, bool[] value)
	{
		if (address.StartsWith(new string[1] { "G" }))
		{
			int destIndex = MemobusHelper.CalculateBoolIndex(address);
			gBuffer.SetBool(value, destIndex);
			return OperateResult.CreateSuccessResult();
		}
		OperateResult<MemobusAddress> operateResult = MemobusAddress.ParseFrom(address, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<SoftBuffer> dataAreaFromYokogawaAddress = GetDataAreaFromYokogawaAddress(operateResult.Content, isBit: true);
		if (!dataAreaFromYokogawaAddress.IsSuccess)
		{
			return dataAreaFromYokogawaAddress;
		}
		if (operateResult.Content.SFC == 3 || operateResult.Content.SFC == 4)
		{
			dataAreaFromYokogawaAddress.Content.SetBool(value, operateResult.Content.AddressStart);
		}
		else
		{
			dataAreaFromYokogawaAddress.Content.SetBytes(value.Select((bool m) => (byte)(m ? 1 : 0)).ToArray(), operateResult.Content.AddressStart);
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new MemobusMessage();
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		byte[] array = null;
		array = ((receive[15] != 1 && receive[15] != 2 && receive[15] != 65) ? ((receive[15] != 5 && receive[15] != 15 && receive[15] != 79) ? ((receive[15] != 3 && receive[15] != 4 && receive[15] != 9 && receive[15] != 10 && receive[15] != 73) ? ((receive[15] != 6 && receive[15] != 16 && receive[15] != 11 && receive[15] != 75) ? ((receive[15] == 13 || receive[15] == 77) ? ReadRandomWordByCommand(receive) : ((receive[15] == 14) ? WriteRandomWordByCommand(receive) : ((receive[15] != 8) ? PackCommandBack(receive, 1, null) : PackCommandBack(receive, 0, null)))) : WriteWordByCommand(receive)) : ReadWordByCommand(receive)) : WriteBoolByCommand(receive)) : ReadBoolByCommand(receive));
		return OperateResult.CreateSuccessResult(array);
	}

	private byte[] ReadBoolByCommand(byte[] command)
	{
		if (command[15] == 1 || command[15] == 2)
		{
			int num = base.ByteTransform.TransUInt16(command, 17);
			int num2 = base.ByteTransform.TransUInt16(command, 19);
			if (num + num2 > 65535)
			{
				return PackCommandBack(command, 3, null);
			}
			return command[15] switch
			{
				1 => PackCommandBack(command, 0, (from m in yBuffer.GetBytes(num, num2)
					select m != 0).ToArray().ToByteArray()), 
				2 => PackCommandBack(command, 0, (from m in xBuffer.GetBytes(num, num2)
					select m != 0).ToArray().ToByteArray()), 
				_ => PackCommandBack(command, 1, null), 
			};
		}
		if (command[15] == 65)
		{
			byte b = command[18];
			int destIndex = BitConverter.ToInt32(command, 20);
			int length = BitConverter.ToInt32(command, 24);
			return b switch
			{
				77 => PackCommandBack(command, 0, rBuffer.GetBool(destIndex, length).ToByteArray()), 
				73 => PackCommandBack(command, 0, inputBuffer.GetBool(destIndex, length).ToByteArray()), 
				71 => PackCommandBack(command, 0, gBuffer.GetBool(destIndex, length).ToByteArray()), 
				_ => PackCommandBack(command, 1, null), 
			};
		}
		return PackCommandBack(command, 1, null);
	}

	private byte[] WriteBoolByCommand(byte[] command)
	{
		if (!base.EnableWrite)
		{
			return PackCommandBack(command, 3, null);
		}
		if (command[15] == 5 || command[15] == 15)
		{
			int num = base.ByteTransform.TransUInt16(command, 17);
			int num2 = base.ByteTransform.TransUInt16(command, 19);
			switch (command[15])
			{
			case 5:
				if (command.Length != 21)
				{
					return PackCommandBack(command, 5, null);
				}
				yBuffer.SetBytes(new byte[1] { command[19] }, num);
				return PackCommandBack(command, 0, null);
			case 15:
			{
				if (num + num2 > 65535)
				{
					return PackCommandBack(command, 3, null);
				}
				if (command.Length != 21 + (num2 + 7) / 8)
				{
					return PackCommandBack(command, 5, null);
				}
				bool[] source = command.RemoveBegin(21).ToBoolArray().SelectBegin(num2);
				yBuffer.SetBytes(source.Select((bool m) => (byte)(m ? byte.MaxValue : 0)).ToArray(), num);
				return PackCommandBack(command, 0, null);
			}
			default:
				return PackCommandBack(command, 1, null);
			}
		}
		if (command[15] == 79)
		{
			byte b = command[18];
			int destIndex = BitConverter.ToInt32(command, 20);
			int length = BitConverter.ToUInt16(command, 24);
			bool[] value = command.RemoveBegin(28).ToBoolArray().SelectBegin(length);
			switch (b)
			{
			case 77:
				rBuffer.SetBool(value, destIndex);
				return PackCommandBack(command, 0, null);
			case 71:
				gBuffer.SetBool(value, destIndex);
				return PackCommandBack(command, 0, null);
			default:
				return PackCommandBack(command, 1, null);
			}
		}
		return PackCommandBack(command, 1, null);
	}

	private byte[] ReadWordByCommand(byte[] command)
	{
		if (command[15] == 3 || command[15] == 4)
		{
			int num = base.ByteTransform.TransUInt16(command, 17);
			int num2 = base.ByteTransform.TransUInt16(command, 19);
			if (num + num2 > 65535)
			{
				return PackCommandBack(command, 3, null);
			}
			if (command[15] == 3)
			{
				return PackCommandBack(command, 0, rBuffer.GetBytes(num * 2, num2 * 2));
			}
			if (command[15] == 4)
			{
				return PackCommandBack(command, 0, inputBuffer.GetBytes(num * 2, num2 * 2));
			}
		}
		else if (command[15] == 9 || command[15] == 10)
		{
			int num3 = BitConverter.ToUInt16(command, 18);
			int num4 = BitConverter.ToUInt16(command, 20);
			if (num3 + num4 > 65535)
			{
				return PackCommandBack(command, 3, null);
			}
			if (command[15] == 9)
			{
				return PackCommandBack(command, 0, SoftBasic.BytesReverseByWord(rExtBuffer.GetBytes(num3 * 2, num4 * 2)));
			}
			if (command[15] == 10)
			{
				return PackCommandBack(command, 0, SoftBasic.BytesReverseByWord(inputExtBuffer.GetBytes(num3 * 2, num4 * 2)));
			}
		}
		else if (command[15] == 73)
		{
			byte b = command[18];
			int num5 = BitConverter.ToInt32(command, 20);
			int num6 = BitConverter.ToUInt16(command, 24);
			switch (b)
			{
			case 77:
				return PackCommandBack(command, 0, SoftBasic.BytesReverseByWord(rBuffer.GetBytes(num5 * 2, num6 * 2)));
			case 73:
				return PackCommandBack(command, 0, SoftBasic.BytesReverseByWord(inputBuffer.GetBytes(num5 * 2, num6 * 2)));
			case 71:
				return PackCommandBack(command, 0, SoftBasic.BytesReverseByWord(gBuffer.GetBytes(num5 * 2, num6 * 2)));
			}
		}
		return PackCommandBack(command, 1, null);
	}

	private byte[] WriteWordByCommand(byte[] command)
	{
		if (!base.EnableWrite)
		{
			return PackCommandBack(command, 3, null);
		}
		if (command[15] == 6 || command[15] == 16)
		{
			int num = base.ByteTransform.TransUInt16(command, 17);
			int num2 = base.ByteTransform.TransUInt16(command, 19);
			if (command[15] == 6)
			{
				if (command.Length != 21)
				{
					return PackCommandBack(command, 3, null);
				}
				rBuffer.SetBytes(command.SelectLast(2), num * 2);
				return PackCommandBack(command, 0, null);
			}
			if (num + num2 > 65535)
			{
				return PackCommandBack(command, 3, null);
			}
			if (command.Length != 21 + num2 * 2)
			{
				return PackCommandBack(command, 3, null);
			}
			rBuffer.SetBytes(command.RemoveBegin(21), num * 2);
			return PackCommandBack(command, 0, null);
		}
		if (command[15] == 11)
		{
			int num3 = BitConverter.ToUInt16(command, 18);
			int num4 = BitConverter.ToUInt16(command, 20);
			if (num3 + num4 > 65535)
			{
				return PackCommandBack(command, 3, null);
			}
			if (command.Length != 22 + num4 * 2)
			{
				return PackCommandBack(command, 3, null);
			}
			rExtBuffer.SetBytes(SoftBasic.BytesReverseByWord(command.RemoveBegin(22)), num3 * 2);
			return PackCommandBack(command, 0, null);
		}
		if (command[15] == 75)
		{
			byte b = command[18];
			int num5 = BitConverter.ToInt32(command, 20);
			int num6 = BitConverter.ToUInt16(command, 24);
			if (command.Length != 26 + num6 * 2)
			{
				return PackCommandBack(command, 3, null);
			}
			switch (b)
			{
			case 77:
				rBuffer.SetBytes(SoftBasic.BytesReverseByWord(command.RemoveBegin(26)), num5 * 2);
				return PackCommandBack(command, 0, null);
			case 71:
				gBuffer.SetBytes(SoftBasic.BytesReverseByWord(command.RemoveBegin(26)), num5 * 2);
				return PackCommandBack(command, 0, null);
			}
		}
		return PackCommandBack(command, 1, null);
	}

	private byte[] ReadRandomWordByCommand(byte[] command)
	{
		int num = BitConverter.ToUInt16(command, 18);
		if (command[15] == 13)
		{
			if (command.Length != 20 + num * 2)
			{
				return PackCommandBack(command, 3, null);
			}
			byte[] buffer = new byte[num * 2];
			for (int i = 0; i < num; i++)
			{
				int num2 = BitConverter.ToUInt16(command, 20 + i * 2);
				byte[] bytes = rExtBuffer.GetBytes(num2 * 2, 2);
				buffer[i * 2] = bytes[1];
				buffer[i * 2 + 1] = bytes[0];
			}
			return PackCommandBack(command, 0, buffer);
		}
		if (command.Length != 20 + num * 6)
		{
			return PackCommandBack(command, 3, null);
		}
		byte[] buffer2 = new byte[num * 2];
		for (int j = 0; j < num; j++)
		{
			byte b = command[20 + j * 6];
			int num3 = BitConverter.ToInt32(command, 22 + j * 6);
			switch (b)
			{
			case 77:
			{
				byte[] bytes4 = rBuffer.GetBytes(num3 * 2, 2);
				buffer2[j * 2] = bytes4[1];
				buffer2[j * 2 + 1] = bytes4[0];
				break;
			}
			case 73:
			{
				byte[] bytes3 = inputBuffer.GetBytes(num3 * 2, 2);
				buffer2[j * 2] = bytes3[1];
				buffer2[j * 2 + 1] = bytes3[0];
				break;
			}
			case 71:
			{
				byte[] bytes2 = gBuffer.GetBytes(num3 * 2, 2);
				buffer2[j * 2] = bytes2[1];
				buffer2[j * 2 + 1] = bytes2[0];
				break;
			}
			}
		}
		return PackCommandBack(command, 0, buffer2);
	}

	private byte[] WriteRandomWordByCommand(byte[] command)
	{
		if (!base.EnableWrite)
		{
			return PackCommandBack(command, 3, null);
		}
		int num = BitConverter.ToUInt16(command, 18);
		if (command.Length != 20 + num * 4)
		{
			return PackCommandBack(command, 3, null);
		}
		for (int i = 0; i < num; i++)
		{
			int num2 = BitConverter.ToUInt16(command, 20 + i * 4);
			rExtBuffer.SetValue(command[20 + i * 4 + 2], num2 * 2 + 1);
			rExtBuffer.SetValue(command[20 + i * 4 + 3], num2 * 2);
		}
		return PackCommandBack(command, 0, null);
	}

	private byte TransByteHighLow(byte value)
	{
		int num = value & 0xF0;
		num >>= 4;
		return (byte)(((value & 0xF) << 4) | num);
	}

	private byte[] PackCommandBack(byte[] cmds, byte err, byte[] result)
	{
		if (result == null)
		{
			result = new byte[0];
		}
		if (err > 0)
		{
			byte[] buffer = new byte[6 + result.Length];
			buffer[0] = 4;
			buffer[1] = 0;
			buffer[2] = cmds[14];
			buffer[3] = (byte)(cmds[15] + 128);
			buffer[4] = TransByteHighLow(cmds[16]);
			buffer[5] = err;
			return MemobusHelper.PackCommandWithHeader(buffer, cmds[1]);
		}
		if (cmds[15] == 1 || cmds[15] == 2 || cmds[15] == 3 || cmds[15] == 4)
		{
			byte[] buffer2 = new byte[5 + result.Length];
			buffer2[0] = BitConverter.GetBytes(3 + result.Length)[0];
			buffer2[1] = BitConverter.GetBytes(3 + result.Length)[1];
			buffer2[2] = cmds[14];
			buffer2[3] = cmds[15];
			buffer2[4] = TransByteHighLow(cmds[16]);
			result.CopyTo(buffer2, 5);
			return MemobusHelper.PackCommandWithHeader(buffer2, cmds[1]);
		}
		if (cmds[15] == 5 || cmds[15] == 6 || cmds[15] == 8)
		{
			byte[] array3 = cmds.RemoveBegin(12);
			array3[0] = BitConverter.GetBytes(array3.Length - 2)[0];
			array3[1] = BitConverter.GetBytes(array3.Length - 2)[1];
			array3[4] = TransByteHighLow(cmds[16]);
			return MemobusHelper.PackCommandWithHeader(array3, cmds[1]);
		}
		if (cmds[15] == 9 || cmds[15] == 10 || cmds[15] == 13)
		{
			byte[] buffer3 = new byte[8 + result.Length];
			buffer3[0] = BitConverter.GetBytes(buffer3.Length - 2)[0];
			buffer3[1] = BitConverter.GetBytes(buffer3.Length - 2)[1];
			buffer3[2] = cmds[14];
			buffer3[3] = cmds[15];
			buffer3[4] = TransByteHighLow(cmds[16]);
			buffer3[6] = BitConverter.GetBytes(result.Length / 2)[0];
			buffer3[7] = BitConverter.GetBytes(result.Length / 2)[1];
			result.CopyTo(buffer3, 8);
			return MemobusHelper.PackCommandWithHeader(buffer3, cmds[1]);
		}
		if (cmds[15] == 11)
		{
			byte[] array5 = cmds.SelectMiddle(12, 10);
			array5[0] = BitConverter.GetBytes(array5.Length - 2)[0];
			array5[1] = BitConverter.GetBytes(array5.Length - 2)[1];
			array5[4] = TransByteHighLow(cmds[16]);
			return MemobusHelper.PackCommandWithHeader(array5, cmds[1]);
		}
		if (cmds[15] == 14)
		{
			byte[] array6 = cmds.SelectMiddle(12, 8);
			array6[0] = BitConverter.GetBytes(array6.Length - 2)[0];
			array6[1] = BitConverter.GetBytes(array6.Length - 2)[1];
			array6[4] = TransByteHighLow(cmds[16]);
			return MemobusHelper.PackCommandWithHeader(array6, cmds[1]);
		}
		if (cmds[15] == 15 || cmds[15] == 16)
		{
			byte[] array7 = cmds.SelectMiddle(12, 9);
			array7[0] = BitConverter.GetBytes(array7.Length - 2)[0];
			array7[1] = BitConverter.GetBytes(array7.Length - 2)[1];
			array7[4] = TransByteHighLow(cmds[16]);
			return MemobusHelper.PackCommandWithHeader(array7, cmds[1]);
		}
		if (cmds[15] == 65)
		{
			byte[] buffer4 = new byte[8 + result.Length];
			buffer4[0] = BitConverter.GetBytes(buffer4.Length - 2)[0];
			buffer4[1] = BitConverter.GetBytes(buffer4.Length - 2)[1];
			buffer4[2] = cmds[14];
			buffer4[3] = cmds[15];
			buffer4[4] = TransByteHighLow(cmds[16]);
			buffer4[6] = cmds[18];
			result.CopyTo(buffer4, 8);
			return MemobusHelper.PackCommandWithHeader(buffer4, cmds[1]);
		}
		if (cmds[15] == 73)
		{
			byte[] buffer5 = new byte[10 + result.Length];
			buffer5[0] = BitConverter.GetBytes(8 + result.Length)[0];
			buffer5[1] = BitConverter.GetBytes(8 + result.Length)[1];
			buffer5[2] = cmds[14];
			buffer5[3] = cmds[15];
			buffer5[4] = TransByteHighLow(cmds[16]);
			buffer5[6] = cmds[18];
			buffer5[8] = BitConverter.GetBytes(result.Length / 2)[0];
			buffer5[9] = BitConverter.GetBytes(result.Length / 2)[1];
			result.CopyTo(buffer5, 10);
			return MemobusHelper.PackCommandWithHeader(buffer5, cmds[1]);
		}
		if (cmds[15] == 75)
		{
			byte[] array10 = cmds.SelectMiddle(12, 14);
			array10[0] = BitConverter.GetBytes(array10.Length - 2)[0];
			array10[1] = BitConverter.GetBytes(array10.Length - 2)[1];
			array10[4] = TransByteHighLow(cmds[16]);
			return MemobusHelper.PackCommandWithHeader(array10, cmds[1]);
		}
		if (cmds[15] == 77)
		{
			byte[] buffer6 = new byte[8 + result.Length];
			buffer6[0] = BitConverter.GetBytes(6 + result.Length)[0];
			buffer6[1] = BitConverter.GetBytes(6 + result.Length)[1];
			buffer6[2] = cmds[14];
			buffer6[3] = cmds[15];
			buffer6[4] = TransByteHighLow(cmds[16]);
			buffer6[6] = BitConverter.GetBytes(result.Length / 2)[0];
			buffer6[7] = BitConverter.GetBytes(result.Length / 2)[1];
			result.CopyTo(buffer6, 8);
			return MemobusHelper.PackCommandWithHeader(buffer6, cmds[1]);
		}
		if (cmds[15] == 79)
		{
			byte[] array12 = cmds.SelectMiddle(12, 16);
			array12[0] = BitConverter.GetBytes(array12.Length - 2)[0];
			array12[1] = BitConverter.GetBytes(array12.Length - 2)[1];
			array12[4] = TransByteHighLow(cmds[16]);
			return MemobusHelper.PackCommandWithHeader(array12, cmds[1]);
		}
		return PackCommandBack(cmds, 3, null);
	}

	protected override void LoadFromBytes(byte[] content)
	{
		if (content.Length < 655360)
		{
			throw new Exception("File is not correct");
		}
		xBuffer.SetBytes(content, 0, 0, 65536);
		yBuffer.SetBytes(content, 65536, 0, 65536);
		inputBuffer.SetBytes(content, 131072, 0, 65536);
		rBuffer.SetBytes(content, 262144, 0, 65536);
		rExtBuffer.SetBytes(content, 393216, 0, 65536);
		inputExtBuffer.SetBytes(content, 524288, 0, 65536);
		if (content.Length >= 786432)
		{
			gBuffer.SetBytes(content, 655360, 0, 65536);
		}
	}

	protected override byte[] SaveToBytes()
	{
		byte[] buffer = new byte[786432];
		Array.Copy(xBuffer.GetBytes(), 0, buffer, 0, 65536);
		Array.Copy(yBuffer.GetBytes(), 0, buffer, 65536, 65536);
		Array.Copy(inputBuffer.GetBytes(), 0, buffer, 131072, 131072);
		Array.Copy(rBuffer.GetBytes(), 0, buffer, 262144, 131072);
		Array.Copy(rExtBuffer.GetBytes(), 0, buffer, 393216, 131072);
		Array.Copy(inputExtBuffer.GetBytes(), 0, buffer, 524288, 131072);
		Array.Copy(gBuffer.GetBytes(), 0, buffer, 655360, 131072);
		return buffer;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			xBuffer?.Dispose();
			yBuffer?.Dispose();
			inputBuffer?.Dispose();
			rBuffer?.Dispose();
			rExtBuffer?.Dispose();
			inputExtBuffer?.Dispose();
		}
		base.Dispose(disposing);
	}

	public override string ToString()
	{
		return $"MemobusTcpServer[{base.Port}]";
	}
}
