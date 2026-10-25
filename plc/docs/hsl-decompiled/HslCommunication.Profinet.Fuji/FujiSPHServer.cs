// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Fuji;

/// <summary>
/// 富士的SPH虚拟的PLC，支持M1.0，M3.0，M10.0，I0，Q0的位与字的读写操作。
/// </summary>
public class FujiSPHServer : DeviceServer
{
	private SoftBuffer m1Buffer;

	private SoftBuffer m3Buffer;

	private SoftBuffer m10Buffer;

	private SoftBuffer iqBuffer;

	private const int DataPoolLength = 65536;

	/// <summary>
	/// 实例化一个基于SPH协议的虚拟的富士PLC对象，可以用来和进行通信测试。
	/// </summary>
	public FujiSPHServer()
	{
		m1Buffer = new SoftBuffer(131072);
		m3Buffer = new SoftBuffer(131072);
		m10Buffer = new SoftBuffer(131072);
		iqBuffer = new SoftBuffer(131072);
		base.ByteTransform = new RegularByteTransform();
		base.WordLength = 1;
	}

	protected override byte[] SaveToBytes()
	{
		byte[] buffer = new byte[524288];
		m1Buffer.GetBytes().CopyTo(buffer, 0);
		m3Buffer.GetBytes().CopyTo(buffer, 131072);
		m10Buffer.GetBytes().CopyTo(buffer, 262144);
		iqBuffer.GetBytes().CopyTo(buffer, 393216);
		return buffer;
	}

	protected override void LoadFromBytes(byte[] content)
	{
		if (content.Length < 524288)
		{
			throw new Exception("File is not correct");
		}
		m1Buffer.SetBytes(content, 0, 131072);
		m3Buffer.SetBytes(content, 131072, 131072);
		m10Buffer.SetBytes(content, 262144, 131072);
		iqBuffer.SetBytes(content, 393216, 131072);
	}

	[HslMqttApi("ReadByteArray", "")]
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<FujiSPHAddress> operateResult = FujiSPHAddress.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return operateResult.ConvertFailed<byte[]>();
		}
		return operateResult.Content.TypeCode switch
		{
			2 => OperateResult.CreateSuccessResult(m1Buffer.GetBytes(operateResult.Content.AddressStart * 2, length * 2)), 
			4 => OperateResult.CreateSuccessResult(m3Buffer.GetBytes(operateResult.Content.AddressStart * 2, length * 2)), 
			8 => OperateResult.CreateSuccessResult(m10Buffer.GetBytes(operateResult.Content.AddressStart * 2, length * 2)), 
			1 => OperateResult.CreateSuccessResult(iqBuffer.GetBytes(operateResult.Content.AddressStart * 2, length * 2)), 
			_ => new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType), 
		};
	}

	[HslMqttApi("WriteByteArray", "")]
	public override OperateResult Write(string address, byte[] value)
	{
		OperateResult<FujiSPHAddress> operateResult = FujiSPHAddress.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return operateResult.ConvertFailed<byte[]>();
		}
		switch (operateResult.Content.TypeCode)
		{
		case 2:
			m1Buffer.SetBytes(value, operateResult.Content.AddressStart * 2);
			break;
		case 4:
			m3Buffer.SetBytes(value, operateResult.Content.AddressStart * 2);
			break;
		case 8:
			m10Buffer.SetBytes(value, operateResult.Content.AddressStart * 2);
			break;
		case 1:
			iqBuffer.SetBytes(value, operateResult.Content.AddressStart * 2);
			break;
		default:
			return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
		}
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
		OperateResult<FujiSPHAddress> operateResult = FujiSPHAddress.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return operateResult.ConvertFailed<bool[]>();
		}
		int num = operateResult.Content.BitIndex + length;
		int num2 = ((num % 16 == 0) ? (num / 16) : (num / 16 + 1));
		OperateResult<byte[]> operateResult2 = Read(address, (ushort)num2);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2.ConvertFailed<bool[]>();
		}
		return OperateResult.CreateSuccessResult(operateResult2.Content.ToBoolArray().SelectMiddle(operateResult.Content.BitIndex, length));
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
		OperateResult<FujiSPHAddress> operateResult = FujiSPHAddress.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return operateResult.ConvertFailed<bool[]>();
		}
		switch (operateResult.Content.TypeCode)
		{
		case 2:
			m1Buffer.SetBool(value, operateResult.Content.AddressStart * 16 + operateResult.Content.BitIndex);
			break;
		case 4:
			m3Buffer.SetBool(value, operateResult.Content.AddressStart * 16 + operateResult.Content.BitIndex);
			break;
		case 8:
			m10Buffer.SetBool(value, operateResult.Content.AddressStart * 16 + operateResult.Content.BitIndex);
			break;
		case 1:
			iqBuffer.SetBool(value, operateResult.Content.AddressStart * 16 + operateResult.Content.BitIndex);
			break;
		default:
			return new OperateResult(StringResources.Language.NotSupportedDataType);
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new FujiSPHMessage();
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		if (receive[0] != 251 || receive[1] != 128)
		{
			return new OperateResult<byte[]>("Command must start with 0xfb 0x80");
		}
		return OperateResult.CreateSuccessResult(ReadFromSPBCore(receive));
	}

	private byte[] PackCommand(byte[] cmd, byte err, byte[] data)
	{
		if (data == null)
		{
			data = new byte[0];
		}
		byte[] buffer = new byte[26 + data.Length];
		buffer[0] = 251;
		buffer[1] = 128;
		buffer[2] = 128;
		buffer[3] = 0;
		buffer[4] = err;
		buffer[5] = 123;
		buffer[6] = cmd[6];
		buffer[7] = 0;
		buffer[8] = 17;
		buffer[9] = 0;
		buffer[10] = 0;
		buffer[11] = 0;
		buffer[12] = 0;
		buffer[13] = 0;
		buffer[14] = cmd[14];
		buffer[15] = cmd[15];
		buffer[16] = 0;
		buffer[17] = 1;
		buffer[18] = BitConverter.GetBytes(data.Length + 6)[0];
		buffer[19] = BitConverter.GetBytes(data.Length + 6)[1];
		Array.Copy(cmd, 20, buffer, 20, 6);
		if (data.Length != 0)
		{
			data.CopyTo(buffer, 26);
		}
		return buffer;
	}

	private byte[] ReadFromSPBCore(byte[] receive)
	{
		if (receive.Length < 20)
		{
			return PackCommand(receive, 16, null);
		}
		if (receive[14] == 0 && receive[15] == 0)
		{
			return ReadByCommand(receive);
		}
		if (receive[14] == 1 && receive[15] == 0)
		{
			return WriteByCommand(receive);
		}
		return PackCommand(receive, 32, null);
	}

	private byte[] ReadByCommand(byte[] command)
	{
		try
		{
			byte b = command[20];
			int num = command[23] * 256 * 256 + command[22] * 256 + command[21];
			int num2 = command[25] * 256 + command[24];
			if (num + num2 > 65535)
			{
				return PackCommand(command, 69, null);
			}
			return b switch
			{
				2 => PackCommand(command, 0, m1Buffer.GetBytes(num * 2, num2 * 2)), 
				4 => PackCommand(command, 0, m3Buffer.GetBytes(num * 2, num2 * 2)), 
				8 => PackCommand(command, 0, m10Buffer.GetBytes(num * 2, num2 * 2)), 
				1 => PackCommand(command, 0, iqBuffer.GetBytes(num * 2, num2 * 2)), 
				_ => PackCommand(command, 64, null), 
			};
		}
		catch
		{
			return PackCommand(command, 164, null);
		}
	}

	private byte[] WriteByCommand(byte[] command)
	{
		if (!base.EnableWrite)
		{
			return PackCommand(command, 16, null);
		}
		try
		{
			byte b = command[20];
			int num = command[23] * 256 * 256 + command[22] * 256 + command[21];
			int num2 = command[25] * 256 + command[24];
			byte[] array = command.RemoveBegin(26);
			if (num + num2 > 65535)
			{
				return PackCommand(command, 69, null);
			}
			if (num2 * 2 != array.Length)
			{
				return PackCommand(command, 69, null);
			}
			switch (b)
			{
			case 2:
				m1Buffer.SetBytes(array, num * 2);
				return PackCommand(command, 0, null);
			case 4:
				m3Buffer.SetBytes(array, num * 2);
				return PackCommand(command, 0, null);
			case 8:
				m10Buffer.SetBytes(array, num * 2);
				return PackCommand(command, 0, null);
			case 1:
				iqBuffer.SetBytes(array, num * 2);
				return PackCommand(command, 0, null);
			default:
				return PackCommand(command, 64, null);
			}
		}
		catch
		{
			return PackCommand(command, 164, null);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			m1Buffer.Dispose();
			m3Buffer.Dispose();
			m10Buffer.Dispose();
			iqBuffer.Dispose();
		}
		base.Dispose(disposing);
	}

	public override string ToString()
	{
		return $"FujiSPHServer[{base.Port}]";
	}
}
