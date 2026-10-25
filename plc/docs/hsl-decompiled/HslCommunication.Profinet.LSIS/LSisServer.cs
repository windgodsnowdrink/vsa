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
using System.Globalization;
using System.Text;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Core.Pipe;
using HslCommunication.Profinet.Panasonic;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.LSIS;

/// <summary>
/// Lsis的虚拟服务器，其中TCP的端口支持Fnet协议，串口支持Cnet协议
/// LSisServer
/// </summary>
public class LSisServer : DeviceServer
{
	private SoftBuffer pBuffer;

	private SoftBuffer qBuffer;

	private SoftBuffer mBuffer;

	private SoftBuffer iBuffer;

	private SoftBuffer uBuffer;

	private SoftBuffer dBuffer;

	private SoftBuffer tBuffer;

	private const int DataPoolLength = 65536;

	private int station = 1;

	/// <summary>
	/// set plc
	/// </summary>
	public string SetCpuType { get; set; }

	/// <summary>
	/// LSisServer
	/// </summary>
	public LSisServer()
	{
		pBuffer = new SoftBuffer(65536);
		qBuffer = new SoftBuffer(65536);
		iBuffer = new SoftBuffer(65536);
		uBuffer = new SoftBuffer(65536);
		mBuffer = new SoftBuffer(65536);
		dBuffer = new SoftBuffer(131072);
		tBuffer = new SoftBuffer(131072);
		base.WordLength = 2;
		base.ByteTransform = new RegularByteTransform();
		base.SerialReceiveAtleastTime = 2000;
	}

	/// <summary>
	/// LSisServer
	/// </summary>
	public LSisServer(string CpuType)
		: this()
	{
		SetCpuType = CpuType;
	}

	[HslMqttApi("ReadByteArray", "")]
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<string> operateResult = AnalysisAddressToByteUnit(address, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		int index = int.Parse(operateResult.Content.Substring(1));
		return operateResult.Content[0] switch
		{
			'P' => OperateResult.CreateSuccessResult(pBuffer.GetBytes(index, length)), 
			'Q' => OperateResult.CreateSuccessResult(qBuffer.GetBytes(index, length)), 
			'M' => OperateResult.CreateSuccessResult(mBuffer.GetBytes(index, length)), 
			'I' => OperateResult.CreateSuccessResult(iBuffer.GetBytes(index, length)), 
			'U' => OperateResult.CreateSuccessResult(uBuffer.GetBytes(index, length)), 
			'D' => OperateResult.CreateSuccessResult(dBuffer.GetBytes(index, length)), 
			'T' => OperateResult.CreateSuccessResult(tBuffer.GetBytes(index, length)), 
			_ => new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType), 
		};
	}

	[HslMqttApi("WriteByteArray", "")]
	public override OperateResult Write(string address, byte[] value)
	{
		OperateResult<string> operateResult = AnalysisAddressToByteUnit(address, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		int destIndex = int.Parse(operateResult.Content.Substring(1));
		switch (operateResult.Content[0])
		{
		case 'P':
			pBuffer.SetBytes(value, destIndex);
			break;
		case 'Q':
			qBuffer.SetBytes(value, destIndex);
			break;
		case 'M':
			mBuffer.SetBytes(value, destIndex);
			break;
		case 'I':
			iBuffer.SetBytes(value, destIndex);
			break;
		case 'U':
			uBuffer.SetBytes(value, destIndex);
			break;
		case 'D':
			dBuffer.SetBytes(value, destIndex);
			break;
		case 'T':
			tBuffer.SetBytes(value, destIndex);
			break;
		default:
			return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
		}
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi("ReadByte", "")]
	/// <summary>
	/// Read single byte value from plc
	/// </summary>
	/// <param name="address">Start address</param>
	/// <returns>Whether to write the successful</returns>
	public OperateResult<byte> ReadByte(string address)
	{
		return ByteTransformHelper.GetResultFromArray(Read(address, 1));
	}

	[HslMqttApi("WriteByte", "")]
	/// <summary>
	/// Write single byte value to plc
	/// </summary>
	/// <param name="address">Start address</param>
	/// <param name="value">value</param>
	/// <returns>Whether to write the successful</returns>
	public OperateResult Write(string address, byte value)
	{
		return Write(address, new byte[1] { value });
	}

	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		OperateResult<string> operateResult = AnalysisAddressToByteUnit(address, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		int destIndex = int.Parse(operateResult.Content.Substring(1));
		return operateResult.Content[0] switch
		{
			'P' => OperateResult.CreateSuccessResult(pBuffer.GetBool(destIndex, length)), 
			'Q' => OperateResult.CreateSuccessResult(qBuffer.GetBool(destIndex, length)), 
			'M' => OperateResult.CreateSuccessResult(mBuffer.GetBool(destIndex, length)), 
			'I' => OperateResult.CreateSuccessResult(iBuffer.GetBool(destIndex, length)), 
			'U' => OperateResult.CreateSuccessResult(uBuffer.GetBool(destIndex, length)), 
			_ => new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType), 
		};
	}

	[HslMqttApi("WriteBoolArray", "")]
	public override OperateResult Write(string address, bool[] value)
	{
		OperateResult<string> operateResult = AnalysisAddressToByteUnit(address, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		int destIndex = int.Parse(operateResult.Content.Substring(1));
		switch (operateResult.Content[0])
		{
		case 'P':
			pBuffer.SetBool(value, destIndex);
			return OperateResult.CreateSuccessResult();
		case 'Q':
			qBuffer.SetBool(value, destIndex);
			return OperateResult.CreateSuccessResult();
		case 'M':
			mBuffer.SetBool(value, destIndex);
			return OperateResult.CreateSuccessResult();
		case 'I':
			iBuffer.SetBool(value, destIndex);
			return OperateResult.CreateSuccessResult();
		case 'U':
			uBuffer.SetBool(value, destIndex);
			return OperateResult.CreateSuccessResult();
		default:
			return new OperateResult(StringResources.Language.NotSupportedDataType);
		}
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new LsisFastEnetMessage();
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		if (session.Communication is PipeSerialPort)
		{
			try
			{
				byte[] value = null;
				if (receive[3] == 114 || receive[3] == 82)
				{
					value = ReadSerialByCommand(receive);
				}
				else if (receive[3] == 119 || receive[3] == 87)
				{
					value = WriteSerialByMessage(receive);
				}
				return OperateResult.CreateSuccessResult(value);
			}
			catch (Exception ex)
			{
				return new OperateResult<byte[]>(ex.Message + " Source: " + receive.ToHexString(' '));
			}
		}
		byte[] value2 = null;
		if (receive[20] == 84)
		{
			value2 = ReadByCommand(receive);
		}
		else if (receive[20] == 88)
		{
			value2 = WriteByMessage(receive);
		}
		return OperateResult.CreateSuccessResult(value2);
	}

	private byte[] ReadByCommand(byte[] command)
	{
		List<byte> list = new List<byte>();
		list.AddRange(command.SelectBegin(20));
		list[9] = 17;
		list[10] = 1;
		list[12] = 160;
		list[13] = 17;
		list[18] = 3;
		byte[] buffer = new byte[10] { 85, 0, 0, 0, 8, 1, 0, 0, 1, 0 };
		buffer[2] = command[22];
		buffer[3] = command[23];
		list.AddRange(buffer);
		int num = command[28];
		string message = Encoding.ASCII.GetString(command, 31, num - 1);
		byte[] array;
		if (command[22] == 0)
		{
			int num2 = Convert.ToInt32(message.Substring(2));
			array = ((!ReadBool(message.Substring(0, 2) + num2 / 16 + (num2 % 16).ToString("X1")).Content) ? new byte[1] : new byte[1] { 1 });
		}
		else if (command[22] == 1)
		{
			array = Read(message, 1).Content;
		}
		else if (command[22] == 2)
		{
			array = Read(message, 2).Content;
		}
		else if (command[22] == 3)
		{
			array = Read(message, 4).Content;
		}
		else if (command[22] == 4)
		{
			array = Read(message, 8).Content;
		}
		else if (command[22] == 20)
		{
			ushort length = BitConverter.ToUInt16(command, 30 + num);
			array = Read(message, length).Content;
		}
		else
		{
			array = Read(message, 1).Content;
		}
		list.AddRange(BitConverter.GetBytes((ushort)array.Length));
		list.AddRange(array);
		list[16] = BitConverter.GetBytes(list.Count - 20)[0];
		list[17] = BitConverter.GetBytes(list.Count - 20)[1];
		return list.ToArray();
	}

	private byte[] WriteByMessage(byte[] packCommand)
	{
		if (!base.EnableWrite)
		{
			return null;
		}
		List<byte> list = new List<byte>();
		list.AddRange(packCommand.SelectBegin(20));
		list[9] = 17;
		list[10] = 1;
		list[12] = 160;
		list[13] = 17;
		list[18] = 3;
		list.AddRange(new byte[10] { 89, 0, 20, 0, 8, 1, 0, 0, 1, 0 });
		int num = packCommand[28];
		string message = Encoding.ASCII.GetString(packCommand, 31, num - 1);
		int length = BitConverter.ToUInt16(packCommand, 30 + num);
		int num2 = 0;
		byte[] array = base.ByteTransform.TransByte(packCommand, 32 + num, length);
		if (packCommand[22] == 0)
		{
			if (message.IndexOf('.') < 0)
			{
				num2 = Convert.ToInt32(message.Substring(2));
			}
			else
			{
				string[] array2 = message.Substring(2, message.Length - 2).Split('.');
				num2 = ((array2.Length < 3) ? int.Parse(array2[1], NumberStyles.HexNumber) : int.Parse(array2[2], NumberStyles.HexNumber));
			}
			int num3 = Convert.ToInt32(num2);
			Write(message.Substring(0, 2) + num3 / 16 + (num3 % 16).ToString("X1"), array[0] == 1);
		}
		else
		{
			Write(message, array);
		}
		list[16] = BitConverter.GetBytes(list.Count - 20)[0];
		list[17] = BitConverter.GetBytes(list.Count - 20)[1];
		return list.ToArray();
	}

	protected override void LoadFromBytes(byte[] content)
	{
		if (content.Length < 262144)
		{
			throw new Exception("File is not correct");
		}
		pBuffer.SetBytes(content, 0, 0, 65536);
		qBuffer.SetBytes(content, 65536, 0, 65536);
		mBuffer.SetBytes(content, 131072, 0, 65536);
		dBuffer.SetBytes(content, 196608, 0, 65536);
	}

	protected override byte[] SaveToBytes()
	{
		byte[] buffer = new byte[262144];
		Array.Copy(pBuffer.GetBytes(), 0, buffer, 0, 65536);
		Array.Copy(qBuffer.GetBytes(), 0, buffer, 65536, 65536);
		Array.Copy(mBuffer.GetBytes(), 0, buffer, 131072, 65536);
		Array.Copy(dBuffer.GetBytes(), 0, buffer, 196608, 65536);
		return buffer;
	}

	/// <summary>
	/// NumberStyles HexNumber
	/// </summary>
	private static bool IsHex(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return false;
		}
		bool success = false;
		for (int i = 0; i < value.Length; i++)
		{
			switch (value[i])
			{
			case 'A':
			case 'B':
			case 'C':
			case 'D':
			case 'E':
			case 'F':
			case 'a':
			case 'b':
			case 'c':
			case 'd':
			case 'e':
			case 'f':
				success = true;
				break;
			}
		}
		return success;
	}

	/// <summary>
	/// Check the intput string address
	/// </summary>
	public static int CheckAddress(string address)
	{
		int result = 0;
		if (IsHex(address))
		{
			if (int.TryParse(address, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var result2))
			{
				result = result2;
			}
		}
		else
		{
			result = int.Parse(address);
		}
		return result;
	}

	protected override bool CheckSerialReceiveDataComplete(byte[] buffer, int receivedLength)
	{
		if (receivedLength > 5)
		{
			return buffer[receivedLength - 3] == 4;
		}
		return base.CheckSerialReceiveDataComplete(buffer, receivedLength);
	}

	protected override string GetLogTextFromBinary(PipeSession session, byte[] content)
	{
		if (session != null && session.Communication is PipeSerialPort)
		{
			return SoftBasic.GetAsciiStringRender(content);
		}
		return base.GetLogTextFromBinary(session, content);
	}

	private byte[] PackReadSerialResponse(byte[] receive, short err, List<byte[]> data)
	{
		List<byte> list = new List<byte>(24);
		if (err == 0)
		{
			list.Add(6);
		}
		else
		{
			list.Add(21);
		}
		list.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)station));
		list.Add(receive[3]);
		list.Add(receive[4]);
		list.Add(receive[5]);
		if (err == 0)
		{
			if (data != null)
			{
				if (Encoding.ASCII.GetString(receive, 4, 2) == "SS")
				{
					list.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)data.Count));
				}
				else if (Encoding.ASCII.GetString(receive, 4, 2) == "SB")
				{
					list.AddRange(Encoding.ASCII.GetBytes("01"));
				}
				for (int i = 0; i < data.Count; i++)
				{
					list.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)data[i].Length));
					list.AddRange(SoftBasic.BytesToAsciiBytes(data[i]));
				}
			}
		}
		else
		{
			list.AddRange(SoftBasic.BuildAsciiBytesFrom(err));
		}
		list.Add(3);
		int num = 0;
		for (int j = 0; j < list.Count; j++)
		{
			num += list[j];
		}
		list.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)num));
		return list.ToArray();
	}

	private byte[] ReadSerialByCommand(byte[] command)
	{
		string asciiStringRender = SoftBasic.GetAsciiStringRender(command);
		if (Encoding.ASCII.GetString(command, 4, 2) == "SS")
		{
			int num = int.Parse(Encoding.ASCII.GetString(command, 6, 2));
			if (num > 16)
			{
				return PackReadSerialResponse(command, 4, null);
			}
			List<byte[]> list = new List<byte[]>();
			int num2 = 8;
			for (int i = 0; i < num; i++)
			{
				int num3 = Convert.ToInt32(Encoding.ASCII.GetString(command, num2, 2), 16);
				string message = Encoding.ASCII.GetString(command, num2 + 2 + 1, num3 - 1);
				if (message[1] != 'X')
				{
					OperateResult<byte[]> operateResult = Read(message, AnalysisAddressLength(message));
					if (!operateResult.IsSuccess)
					{
						return PackReadSerialResponse(command, 1, null);
					}
					list.Add(operateResult.Content);
				}
				else
				{
					OperateResult<bool> operateResult2 = ReadBool(message);
					if (!operateResult2.IsSuccess)
					{
						return PackReadSerialResponse(command, 1, null);
					}
					list.Add((!operateResult2.Content) ? new byte[1] : new byte[1] { 1 });
				}
				num2 += 2 + num3;
			}
			return PackReadSerialResponse(command, 0, list);
		}
		if (Encoding.ASCII.GetString(command, 4, 2) == "SB")
		{
			int num4 = Convert.ToInt32(Encoding.ASCII.GetString(command, 6, 2), 16);
			string address = Encoding.ASCII.GetString(command, 9, num4 - 1);
			ushort num5 = Convert.ToUInt16(Encoding.ASCII.GetString(command, 8 + num4, 2), 16);
			ushort num6 = (ushort)(num5 * AnalysisAddressLength(address));
			if (num6 > 120)
			{
				return PackReadSerialResponse(command, 4658, null);
			}
			OperateResult<byte[]> operateResult3 = Read(address, num6);
			if (!operateResult3.IsSuccess)
			{
				return PackReadSerialResponse(command, 1, null);
			}
			return PackReadSerialResponse(command, 0, new List<byte[]> { operateResult3.Content });
		}
		return PackReadSerialResponse(command, 1, null);
	}

	private byte[] WriteSerialByMessage(byte[] command)
	{
		if (!base.EnableWrite)
		{
			return null;
		}
		if (Encoding.ASCII.GetString(command, 4, 2) == "SS")
		{
			int num = int.Parse(Encoding.ASCII.GetString(command, 6, 2));
			int num2 = 8;
			if (num > 16)
			{
				return PackReadSerialResponse(command, 4, null);
			}
			for (int i = 0; i < num; i++)
			{
				int num3 = Convert.ToInt32(Encoding.ASCII.GetString(command, num2, 2), 16);
				string message = Encoding.ASCII.GetString(command, num2 + 2 + 1, num3 - 1);
				switch (message[1])
				{
				case 'B':
				case 'D':
				case 'L':
				case 'W':
				{
					byte[] value = Encoding.ASCII.GetString(command, num2 + 2 + num3, AnalysisAddressLength(message) * 2).ToHexBytes();
					OperateResult operateResult2 = Write(message, value);
					if (!operateResult2.IsSuccess)
					{
						return PackReadSerialResponse(command, 1, null);
					}
					num2 += 2 + num3 + AnalysisAddressLength(message) * 2;
					break;
				}
				case 'X':
				{
					OperateResult operateResult = Write(message, Convert.ToByte(Encoding.ASCII.GetString(command, num2 + 2 + num3, 2), 16) != 0);
					if (!operateResult.IsSuccess)
					{
						return PackReadSerialResponse(command, 1, null);
					}
					num2 += 2 + num3 + 2;
					break;
				}
				}
			}
			return PackReadSerialResponse(command, 0, null);
		}
		if (Encoding.ASCII.GetString(command, 4, 2) == "SB")
		{
			int num4 = Convert.ToInt32(Encoding.ASCII.GetString(command, 6, 2), 16);
			string message2 = Encoding.ASCII.GetString(command, 9, num4 - 1);
			if (message2[1] == 'X')
			{
				return PackReadSerialResponse(command, 4402, null);
			}
			ushort num5 = Convert.ToUInt16(Encoding.ASCII.GetString(command, 8 + num4, 2), 16);
			int num6 = num5 * AnalysisAddressLength(message2);
			OperateResult operateResult3 = Write(message2, Encoding.ASCII.GetString(command, 10 + num4, num6 * 2).ToHexBytes());
			if (!operateResult3.IsSuccess)
			{
				return PackReadSerialResponse(command, 1, null);
			}
			return PackReadSerialResponse(command, 0, null);
		}
		return PackReadSerialResponse(command, 4402, null);
	}

	public override string ToString()
	{
		return $"LSisServer[{base.Port}]";
	}

	private static ushort AnalysisAddressLength(string address)
	{
		return address[1] switch
		{
			'X' => 1, 
			'B' => 1, 
			'W' => 2, 
			'D' => 4, 
			'L' => 8, 
			_ => 1, 
		};
	}

	/// <summary>
	/// 将带有数据类型的地址，转换成实际的byte数组的地址信息，例如 MW100 转成 M200
	/// </summary>
	/// <param name="address">带有类型的地址</param>
	/// <param name="isBit">是否是位操作</param>
	/// <returns>最终的按照字节为单位的地址信息</returns>
	public OperateResult<string> AnalysisAddressToByteUnit(string address, bool isBit)
	{
		if (!"PMLKFTCDSQINUZR".Contains(address.Substring(0, 1)))
		{
			return new OperateResult<string>(StringResources.Language.NotSupportedDataType);
		}
		try
		{
			int num;
			if (address[0] == 'D' || address[0] == 'T')
			{
				num = address[1] switch
				{
					'B' => Convert.ToInt32(address.Substring(2)), 
					'W' => Convert.ToInt32(address.Substring(2)) * 2, 
					'D' => Convert.ToInt32(address.Substring(2)) * 4, 
					'L' => Convert.ToInt32(address.Substring(2)) * 8, 
					_ => Convert.ToInt32(address.Substring(1)) * 2, 
				};
			}
			else if (!isBit)
			{
				num = address[1] switch
				{
					'X' => Convert.ToInt32(address.Substring(2)), 
					'B' => Convert.ToInt32(address.Substring(2)), 
					'W' => Convert.ToInt32(address.Substring(2)) * 2, 
					'D' => Convert.ToInt32(address.Substring(2)) * 4, 
					'L' => Convert.ToInt32(address.Substring(2)) * 8, 
					_ => Convert.ToInt32(address.Substring(1)) * (isBit ? 1 : 2), 
				};
			}
			else
			{
				char c = address[1];
				char c2 = c;
				num = ((c2 != 'X') ? PanasonicHelper.CalculateComplexAddress(address.Substring(1)) : PanasonicHelper.CalculateComplexAddress(address.Substring(2)));
			}
			return OperateResult.CreateSuccessResult(address.Substring(0, 1) + num);
		}
		catch (Exception ex)
		{
			return new OperateResult<string>("AnalysisAddress Failed: " + ex.Message + " Source: " + address);
		}
	}
}
