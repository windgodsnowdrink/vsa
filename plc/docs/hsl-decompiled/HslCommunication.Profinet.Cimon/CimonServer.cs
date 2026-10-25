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
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.Net;
using HslCommunication.Profinet.Cimon.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Cimon;

/// <summary>
/// Cimon的服务器端实现
/// </summary>
public class CimonServer : DeviceServer
{
	private const int OffsetX = 0;

	private const int OffsetY = 1000;

	private const int OffsetM = 2000;

	private const int OffsetK = 3000;

	private const int OffsetL = 4000;

	private const int OffsetC = 5000;

	private const int OffsetT = 6000;

	private const int OffsetFlag = 7000;

	private const int OffsetD = 0;

	private const int OffsetF = 40000;

	private const int OffsetZ = 41000;

	private const int OffsetS = 42000;

	private SoftBuffer bufferBit;

	private SoftBuffer bufferWord;

	private const int DataPoolLength = 65536;

	private byte frameNo = 1;

	/// <summary>
	/// 获取或设置当前的站号信息
	/// Gets or sets the current station number information
	/// </summary>
	public byte FrameNo
	{
		get
		{
			return frameNo;
		}
		set
		{
			frameNo = value;
		}
	}

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public CimonServer()
	{
		bufferBit = new SoftBuffer(131072)
		{
			IsBoolReverseByWord = true
		};
		bufferWord = new SoftBuffer(131072);
		base.WordLength = 1;
		base.ByteTransform = new ReverseWordTransform();
	}

	private OperateResult<byte[]> CreateReadWordResult(SoftBuffer softBuffer, int offset, int address, int length)
	{
		return OperateResult.CreateSuccessResult(softBuffer.GetBytes(address * 2 + offset * 2, length * 2));
	}

	[HslMqttApi("ReadByteArray", "")]
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		try
		{
			int address2 = int.Parse(address.Substring(1));
			if (address[0] == 'X')
			{
				return CreateReadWordResult(bufferBit, 0, address2, length);
			}
			if (address[0] == 'Y')
			{
				return CreateReadWordResult(bufferBit, 1000, address2, length);
			}
			if (address[0] == 'M')
			{
				return CreateReadWordResult(bufferBit, 2000, address2, length);
			}
			if (address[0] == 'K')
			{
				return CreateReadWordResult(bufferBit, 3000, address2, length);
			}
			if (address[0] == 'L')
			{
				return CreateReadWordResult(bufferBit, 4000, address2, length);
			}
			if (address[0] == 'C')
			{
				return CreateReadWordResult(bufferBit, 5000, address2, length);
			}
			if (address[0] == 'T')
			{
				return CreateReadWordResult(bufferBit, 6000, address2, length);
			}
			if (address[0] == 'D')
			{
				return CreateReadWordResult(bufferWord, 0, address2, length);
			}
			if (address[0] == 'F')
			{
				return CreateReadWordResult(bufferWord, 40000, address2, length);
			}
			if (address[0] == 'Z')
			{
				return CreateReadWordResult(bufferWord, 41000, address2, length);
			}
			if (address[0] == 'S')
			{
				return CreateReadWordResult(bufferWord, 42000, address2, length);
			}
			return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
	}

	private OperateResult CreateWriteWordResult(SoftBuffer softBuffer, int offset, int address, byte[] value)
	{
		softBuffer.SetBytes(value, address * 2 + offset * 2);
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi("WriteByteArray", "")]
	public override OperateResult Write(string address, byte[] value)
	{
		try
		{
			int address2 = int.Parse(address.Substring(1));
			if (address[0] == 'X')
			{
				return CreateWriteWordResult(bufferBit, 0, address2, value);
			}
			if (address[0] == 'Y')
			{
				return CreateWriteWordResult(bufferBit, 1000, address2, value);
			}
			if (address[0] == 'M')
			{
				return CreateWriteWordResult(bufferBit, 2000, address2, value);
			}
			if (address[0] == 'K')
			{
				return CreateWriteWordResult(bufferBit, 3000, address2, value);
			}
			if (address[0] == 'L')
			{
				return CreateWriteWordResult(bufferBit, 4000, address2, value);
			}
			if (address[0] == 'C')
			{
				return CreateWriteWordResult(bufferBit, 5000, address2, value);
			}
			if (address[0] == 'T')
			{
				return CreateWriteWordResult(bufferBit, 2000, address2, value);
			}
			if (address[0] == 'D')
			{
				return CreateWriteWordResult(bufferWord, 0, address2, value);
			}
			if (address[0] == 'F')
			{
				return CreateWriteWordResult(bufferWord, 40000, address2, value);
			}
			if (address[0] == 'Z')
			{
				return CreateWriteWordResult(bufferWord, 41000, address2, value);
			}
			if (address[0] == 'S')
			{
				return CreateWriteWordResult(bufferWord, 42000, address2, value);
			}
			return new OperateResult(StringResources.Language.NotSupportedDataType);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
	}

	private int GetBitIndex(ref string address)
	{
		if (address.IndexOf('.') < 0)
		{
			int result = Convert.ToInt32(address.Substring(address.Length - 1), 16);
			address = address.Substring(0, address.Length - 1);
			return result;
		}
		int result2 = Convert.ToInt32(address.Substring(address.IndexOf('.') + 1), 16);
		address = address.Substring(0, address.IndexOf('.'));
		return result2;
	}

	private OperateResult<bool[]> CreateReadBitResult(SoftBuffer softBuffer, int offset, int bitIndex, int length)
	{
		return OperateResult.CreateSuccessResult(softBuffer.GetBool(bitIndex + offset * 16, length));
	}

	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		int bitIndex = GetBitIndex(ref address);
		try
		{
			int bitIndex2 = int.Parse(address.Substring(1)) * 16 + bitIndex;
			if (address[0] == 'X')
			{
				return CreateReadBitResult(bufferBit, 0, bitIndex2, length);
			}
			if (address[0] == 'Y')
			{
				return CreateReadBitResult(bufferBit, 1000, bitIndex2, length);
			}
			if (address[0] == 'M')
			{
				return CreateReadBitResult(bufferBit, 2000, bitIndex2, length);
			}
			if (address[0] == 'K')
			{
				return CreateReadBitResult(bufferBit, 3000, bitIndex2, length);
			}
			if (address[0] == 'L')
			{
				return CreateReadBitResult(bufferBit, 4000, bitIndex2, length);
			}
			if (address[0] == 'T')
			{
				return CreateReadBitResult(bufferBit, 6000, bitIndex2, length);
			}
			if (address[0] == 'C')
			{
				return CreateReadBitResult(bufferBit, 5000, bitIndex2, length);
			}
			if (address[0] == 'F')
			{
				return CreateReadBitResult(bufferBit, 7000, bitIndex2, length);
			}
			return new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType);
		}
		catch (Exception ex)
		{
			return new OperateResult<bool[]>(ex.Message);
		}
	}

	private OperateResult CreateWriteBitResult(SoftBuffer softBuffer, int offset, int bitIndex, bool[] value)
	{
		softBuffer.SetBool(value, bitIndex + offset * 16);
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi("WriteBoolArray", "")]
	public override OperateResult Write(string address, bool[] value)
	{
		int bitIndex = GetBitIndex(ref address);
		try
		{
			int bitIndex2 = int.Parse(address.Substring(1)) * 16 + bitIndex;
			if (address[0] == 'X')
			{
				return CreateWriteBitResult(bufferBit, 0, bitIndex2, value);
			}
			if (address[0] == 'Y')
			{
				return CreateWriteBitResult(bufferBit, 1000, bitIndex2, value);
			}
			if (address[0] == 'M')
			{
				return CreateWriteBitResult(bufferBit, 2000, bitIndex2, value);
			}
			if (address[0] == 'K')
			{
				return CreateWriteBitResult(bufferBit, 3000, bitIndex2, value);
			}
			if (address[0] == 'L')
			{
				return CreateWriteBitResult(bufferBit, 4000, bitIndex2, value);
			}
			if (address[0] == 'T')
			{
				return CreateWriteBitResult(bufferBit, 6000, bitIndex2, value);
			}
			if (address[0] == 'C')
			{
				return CreateWriteBitResult(bufferBit, 5000, bitIndex2, value);
			}
			if (address[0] == 'F')
			{
				return CreateWriteBitResult(bufferBit, 7000, bitIndex2, value);
			}
			return new OperateResult(StringResources.Language.NotSupportedDataType);
		}
		catch (Exception ex)
		{
			return new OperateResult(ex.Message);
		}
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		if (receive[10] == 82 || receive[10] == 114)
		{
			return OperateResult.CreateSuccessResult(ReadByCommand(receive));
		}
		if (receive[10] == 87 || receive[10] == 119)
		{
			return OperateResult.CreateSuccessResult(WriteByMessage(receive));
		}
		return OperateResult.CreateSuccessResult(CimonHelper.PackErrorResponse(frameNo, 14));
	}

	private byte[] ReadByCommand(byte[] command)
	{
		int num = command[21] * 256 + command[22];
		string message = Encoding.ASCII.GetString(command, 13, 1);
		string message2 = Encoding.ASCII.GetString(command, 15, 6);
		if (command[10] == 82)
		{
			List<byte> list = new List<byte>();
			list.AddRange(new byte[9]);
			OperateResult<byte[]> operateResult = Read(message + message2, (ushort)num);
			if (operateResult.IsSuccess)
			{
				list.AddRange(operateResult.Content);
				return CimonHelper.PackEntireCommand(response: true, frameNo, command[10], list.ToArray());
			}
			return CimonHelper.PackErrorResponse(frameNo, 3);
		}
		if (command[10] == 114)
		{
			List<byte> list2 = new List<byte>();
			list2.AddRange(new byte[9]);
			OperateResult<bool[]> operateResult2 = ReadBool(message + message2, (ushort)num);
			if (operateResult2.IsSuccess)
			{
				list2.AddRange(operateResult2.Content.Select((bool m) => (byte)(m ? 1 : 0)).ToArray());
			}
			return CimonHelper.PackEntireCommand(response: true, frameNo, command[10], list2.ToArray());
		}
		return CimonHelper.PackErrorResponse(frameNo, 14);
	}

	private byte[] WriteByMessage(byte[] command)
	{
		if (!base.EnableWrite)
		{
			return null;
		}
		string message = Encoding.ASCII.GetString(command, 14, 1);
		string message2 = Encoding.ASCII.GetString(command, 16, 6);
		int length = command[22] * 256 + command[23];
		byte[] array = base.ByteTransform.TransByte(command, 24, length);
		if (command[10] == 87)
		{
			OperateResult operateResult = Write(message + message2, array);
			if (operateResult.IsSuccess)
			{
				return CimonHelper.PackEntireCommand(response: true, frameNo, command[10], new byte[9]);
			}
			return CimonHelper.PackErrorResponse(frameNo, 3);
		}
		if (command[10] == 119)
		{
			bool[] array2 = new bool[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i] != 0;
			}
			OperateResult operateResult2 = Write(message + message2, array2);
			if (operateResult2.IsSuccess)
			{
				return CimonHelper.PackEntireCommand(response: true, frameNo, command[10], new byte[9]);
			}
			return CimonHelper.PackErrorResponse(frameNo, 3);
		}
		return CimonHelper.PackErrorResponse(frameNo, 14);
	}

	protected override byte[] SaveToBytes()
	{
		return ByteTransformHelper.GetByteFromSoftBuffer(bufferBit, bufferWord).ToArray();
	}

	protected override void LoadFromBytes(byte[] content)
	{
		ByteTransformHelper.SetByteToSoftBuffer(content, 0, bufferBit, bufferWord);
	}

	public override string ToString()
	{
		return $"CimonServer[{base.Port}]";
	}
}
