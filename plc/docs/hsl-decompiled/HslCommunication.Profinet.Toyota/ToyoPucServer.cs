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

namespace HslCommunication.Profinet.Toyota;

/// <summary>
/// 丰田工机的PLC对象信息
/// </summary>
public class ToyoPucServer : DeviceServer
{
	private SoftBuffer buffer1;

	private SoftBuffer buffer00;

	private SoftBuffer buffer01;

	private SoftBuffer buffer02;

	private SoftBuffer buffer03;

	private SoftBuffer buffer08;

	private const int DataPoolLength = 65536;

	private int station = 0;

	/// <summary>
	/// 实例化一个丰田工机PLC的网口和串口服务器，支持数据读写操作
	/// </summary>
	public ToyoPucServer()
	{
		buffer1 = new SoftBuffer(65536);
		buffer00 = new SoftBuffer(65536);
		buffer01 = new SoftBuffer(65536);
		buffer02 = new SoftBuffer(65536);
		buffer03 = new SoftBuffer(65536);
		buffer08 = new SoftBuffer(65536);
		base.ByteTransform = new RegularByteTransform();
		base.WordLength = 1;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new ToyoPucMessage();
	}

	protected override byte[] SaveToBytes()
	{
		byte[] buffer = new byte[393216];
		buffer1.GetBytes().CopyTo(buffer, 0);
		buffer00.GetBytes().CopyTo(buffer, 65536);
		buffer01.GetBytes().CopyTo(buffer, 131072);
		buffer02.GetBytes().CopyTo(buffer, 196608);
		buffer03.GetBytes().CopyTo(buffer, 262144);
		buffer08.GetBytes().CopyTo(buffer, 327680);
		return buffer;
	}

	protected override void LoadFromBytes(byte[] content)
	{
		if (content.Length < 393216)
		{
			throw new Exception("File is not correct");
		}
		buffer1.SetBytes(content, 0, 65536);
		buffer00.SetBytes(content, 65536, 65536);
		buffer01.SetBytes(content, 131072, 65536);
		buffer02.SetBytes(content, 196608, 65536);
		buffer03.SetBytes(content, 262144, 65536);
		buffer08.SetBytes(content, 327680, 65536);
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <remarks>
	/// 地址类型地址 K,V,T,C,L,X,Y,M,EK,EV,EC,ET,EL,EX,EY,EM,S,N,R,D,B,ES,EN,H,U,EB地址，也可以指定程序号(prg)参数，例如 prg=1;K000
	/// </remarks>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<ToyoPucAddress> operateResult = ToyoPucAddress.ParseFrom(address, length, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		ToyoPucAddress content = operateResult.Content;
		if (content.PRG >= 0)
		{
			if (content.PRG == 0)
			{
				return OperateResult.CreateSuccessResult(buffer00.GetBytes(content.AddressStart * 2, length * 2));
			}
			if (content.PRG == 1)
			{
				return OperateResult.CreateSuccessResult(buffer01.GetBytes(content.AddressStart * 2, length * 2));
			}
			if (content.PRG == 2)
			{
				return OperateResult.CreateSuccessResult(buffer02.GetBytes(content.AddressStart * 2, length * 2));
			}
			if (content.PRG == 3)
			{
				return OperateResult.CreateSuccessResult(buffer03.GetBytes(content.AddressStart * 2, length * 2));
			}
			if (content.PRG == 8)
			{
				return OperateResult.CreateSuccessResult(buffer08.GetBytes(content.AddressStart * 2, length * 2));
			}
			return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
		}
		return OperateResult.CreateSuccessResult(buffer1.GetBytes(content.AddressStart * 2, length * 2));
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <remarks>
	/// 地址类型地址 K,V,T,C,L,X,Y,M,EK,EV,EC,ET,EL,EX,EY,EM,S,N,R,D,B,ES,EN,H,U,EB地址，也可以指定程序号(prg)参数，例如 prg=1;K000
	/// </remarks>
	public override OperateResult Write(string address, byte[] value)
	{
		OperateResult<ToyoPucAddress> operateResult = ToyoPucAddress.ParseFrom(address, 1, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		ToyoPucAddress content = operateResult.Content;
		if (content.PRG >= 0)
		{
			if (content.PRG == 0)
			{
				buffer00.SetBytes(value, content.AddressStart * 2);
			}
			else if (content.PRG == 1)
			{
				buffer01.SetBytes(value, content.AddressStart * 2);
			}
			else if (content.PRG == 2)
			{
				buffer02.SetBytes(value, content.AddressStart * 2);
			}
			else if (content.PRG == 3)
			{
				buffer03.SetBytes(value, content.AddressStart * 2);
			}
			else
			{
				if (content.PRG != 8)
				{
					return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
				}
				buffer08.SetBytes(value, content.AddressStart * 2);
			}
			return OperateResult.CreateSuccessResult();
		}
		buffer1.SetBytes(value, content.AddressStart * 2);
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <remarks>
	/// 地址类型地址 K,V,T,C,L,X,Y,M,EK,EV,EC,ET,EL,EX,EY,EM 地址，也可以指定程序号(prg)参数，例如 prg=1;K000
	/// </remarks>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		if (address.IndexOf(".") > 0)
		{
			return HslHelper.ReadBool(this, address, length);
		}
		OperateResult<ToyoPucAddress> operateResult = ToyoPucAddress.ParseFrom(address, length, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		ToyoPucAddress content = operateResult.Content;
		if (content.PRG >= 0)
		{
			if (content.PRG == 0)
			{
				return OperateResult.CreateSuccessResult(buffer00.GetBool(content.AddressStart, length));
			}
			if (content.PRG == 1)
			{
				return OperateResult.CreateSuccessResult(buffer01.GetBool(content.AddressStart, length));
			}
			if (content.PRG == 2)
			{
				return OperateResult.CreateSuccessResult(buffer02.GetBool(content.AddressStart, length));
			}
			if (content.PRG == 3)
			{
				return OperateResult.CreateSuccessResult(buffer03.GetBool(content.AddressStart, length));
			}
			if (content.PRG == 8)
			{
				return OperateResult.CreateSuccessResult(buffer08.GetBool(content.AddressStart, length));
			}
			return new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType);
		}
		return OperateResult.CreateSuccessResult(buffer1.GetBool(content.AddressStart, length));
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
		if (address.IndexOf(".") > 0)
		{
			return HslHelper.WriteBool(this, address, value);
		}
		OperateResult<ToyoPucAddress> operateResult = ToyoPucAddress.ParseFrom(address, 1, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		ToyoPucAddress content = operateResult.Content;
		if (content.PRG >= 0)
		{
			if (content.PRG == 0)
			{
				buffer00.SetBool(value, content.AddressStart);
			}
			else if (content.PRG == 1)
			{
				buffer01.SetBool(value, content.AddressStart);
			}
			else if (content.PRG == 2)
			{
				buffer02.SetBool(value, content.AddressStart);
			}
			else if (content.PRG == 3)
			{
				buffer03.SetBool(value, content.AddressStart);
			}
			else
			{
				if (content.PRG != 8)
				{
					return new OperateResult<byte[]>("Not supported prg write bool");
				}
				buffer08.SetBool(value, content.AddressStart);
			}
			return OperateResult.CreateSuccessResult();
		}
		buffer1.SetBool(value, content.AddressStart);
		return OperateResult.CreateSuccessResult();
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		if (receive.Length < 5)
		{
			return null;
		}
		try
		{
			byte[] array = null;
			if (receive[4] == 28)
			{
				array = ReadWordByCommand(receive);
			}
			else if (receive[4] == 32)
			{
				array = ReadBoolByCommand(receive);
			}
			else if (receive[4] == 29)
			{
				array = WriteWordByCommand(receive);
			}
			else if (receive[4] == 33)
			{
				array = WriteBoolByCommand(receive);
			}
			else if (receive[4] == 34)
			{
				array = ReadRandomWord(receive);
			}
			else if (receive[4] == 35)
			{
				array = WriteRandomWord(receive);
			}
			else if (receive[4] == 148)
			{
				array = ReadExWordByCommand(receive);
			}
			else if (receive[4] == 149)
			{
				array = WriteExWordByCommand(receive);
			}
			if (array != null)
			{
				return OperateResult.CreateSuccessResult(array);
			}
			return OperateResult.CreateSuccessResult(CreateResponseBack(receive, 35, null));
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
	}

	private byte[] CreateResponseBack(byte[] request, byte err, byte[] data)
	{
		if (err != 0)
		{
			return new byte[5] { 128, 16, 1, 0, err };
		}
		if (data == null)
		{
			data = new byte[0];
		}
		byte[] buffer = new byte[5 + data.Length];
		buffer[0] = 128;
		buffer[1] = 0;
		buffer[2] = BitConverter.GetBytes(data.Length + 1)[0];
		buffer[3] = BitConverter.GetBytes(data.Length + 1)[1];
		buffer[4] = request[4];
		if (data.Length != 0)
		{
			data.CopyTo(buffer, 5);
		}
		return buffer;
	}

	private byte[] ReadWordByCommand(byte[] command)
	{
		ushort num = BitConverter.ToUInt16(command, 5);
		ushort num2 = BitConverter.ToUInt16(command, 7);
		return CreateResponseBack(command, 0, buffer1.GetBytes(num * 2, num2 * 2));
	}

	private byte[] ReadExWordByCommand(byte[] command)
	{
		ushort num = BitConverter.ToUInt16(command, 6);
		ushort num2 = BitConverter.ToUInt16(command, 8);
		if (command[5] == 0)
		{
			return CreateResponseBack(command, 0, buffer00.GetBytes(num * 2, num2 * 2));
		}
		if (command[5] == 1)
		{
			return CreateResponseBack(command, 0, buffer01.GetBytes(num * 2, num2 * 2));
		}
		if (command[5] == 2)
		{
			return CreateResponseBack(command, 0, buffer02.GetBytes(num * 2, num2 * 2));
		}
		if (command[5] == 3)
		{
			return CreateResponseBack(command, 0, buffer03.GetBytes(num * 2, num2 * 2));
		}
		if (command[5] == 8)
		{
			if (num * 2 > 65535 || num * 2 + num2 * 2 > 65536)
			{
				return CreateResponseBack(command, 64, null);
			}
			return CreateResponseBack(command, 0, buffer08.GetBytes(num * 2, num2 * 2));
		}
		return CreateResponseBack(command, 52, null);
	}

	private byte[] ReadRandomWord(byte[] command)
	{
		int num = (command.Length - 5) / 2;
		if (num > 80)
		{
			return CreateResponseBack(command, 64, null);
		}
		byte[] buffer = new byte[num * 2];
		for (int i = 5; i < command.Length - 1; i += 2)
		{
			ushort num2 = BitConverter.ToUInt16(command, i);
			buffer1.GetBytes(num2 * 2, 2).CopyTo(buffer, i - 5);
		}
		return CreateResponseBack(command, 0, buffer);
	}

	private byte[] WriteRandomWord(byte[] command)
	{
		int num = (command.Length - 5) / 4;
		if (num > 80)
		{
			return CreateResponseBack(command, 64, null);
		}
		for (int i = 0; i < num; i++)
		{
			ushort destIndex = BitConverter.ToUInt16(command, i * 4 + 5);
			buffer1.SetBytes(command.SelectMiddle(i * 4 + 7, 2), destIndex);
		}
		return CreateResponseBack(command, 0, null);
	}

	private byte[] ReadBoolByCommand(byte[] command)
	{
		ushort destIndex = BitConverter.ToUInt16(command, 5);
		bool flag = buffer1.GetBool(destIndex);
		return CreateResponseBack(command, 0, new byte[1] { (byte)(flag ? 1 : 0) });
	}

	private byte[] WriteWordByCommand(byte[] command)
	{
		if (!base.EnableWrite)
		{
			return CreateResponseBack(command, 52, null);
		}
		ushort num = BitConverter.ToUInt16(command, 5);
		buffer1.SetBytes(command.RemoveBegin(7), num * 2);
		return CreateResponseBack(command, 0, null);
	}

	private byte[] WriteExWordByCommand(byte[] command)
	{
		if (!base.EnableWrite)
		{
			return CreateResponseBack(command, 52, null);
		}
		ushort num = BitConverter.ToUInt16(command, 6);
		if (command[5] == 0)
		{
			buffer00.SetBytes(command.RemoveBegin(8), num * 2);
		}
		else if (command[5] == 1)
		{
			buffer01.SetBytes(command.RemoveBegin(8), num * 2);
		}
		else if (command[5] == 2)
		{
			buffer02.SetBytes(command.RemoveBegin(8), num * 2);
		}
		else if (command[5] == 3)
		{
			buffer03.SetBytes(command.RemoveBegin(8), num * 2);
		}
		else
		{
			if (command[5] != 8)
			{
				return CreateResponseBack(command, 52, null);
			}
			if (num * 2 > 65535)
			{
				return CreateResponseBack(command, 64, null);
			}
			buffer08.SetBytes(command.RemoveBegin(8), num * 2);
		}
		return CreateResponseBack(command, 0, null);
	}

	private byte[] WriteBoolByCommand(byte[] command)
	{
		if (!base.EnableWrite)
		{
			return CreateResponseBack(command, 52, null);
		}
		ushort destIndex = BitConverter.ToUInt16(command, 5);
		buffer1.SetBool(command[7] != 0, destIndex);
		return CreateResponseBack(command, 0, null);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			buffer1.Dispose();
			buffer00.Dispose();
			buffer01.Dispose();
			buffer02.Dispose();
			buffer03.Dispose();
			buffer08.Dispose();
		}
		base.Dispose(disposing);
	}

	public override string ToString()
	{
		return $"ToyoPucServer[{base.Port}]";
	}
}
