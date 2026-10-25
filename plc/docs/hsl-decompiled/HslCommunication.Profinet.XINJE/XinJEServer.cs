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
using HslCommunication.Core.Net;
using HslCommunication.ModBus;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.XINJE;

/// <summary>
/// 信捷内部TCP的虚拟服务器类，基于Modbus的虚拟服务器扩展而来，从服务上只支持modbus地址读写，从客户端额外支持M,D,SD,SM,HD 五个数据区
/// Xinjie's internal TCP virtual server class, based on Modbus virtual server extension,
/// only supports modbus address reading and writing from the service, and additionally supports M, D, SD, SM, HD five data areas from the client
/// </summary>
public class XinJEServer : ModbusTcpServer
{
	private SoftBuffer mBuffer;

	private SoftBuffer smBuffer;

	private SoftBuffer hmBuffer;

	private SoftBuffer dBuffer;

	private SoftBuffer sdBuffer;

	private SoftBuffer hdBuffer;

	private const int DataPoolLength = 65536;

	/// <summary>
	/// 实例化一个默认参数的mc协议的服务器
	/// Instantiate a mc protocol server with default parameters
	/// </summary>
	public XinJEServer()
	{
		mBuffer = new SoftBuffer(65536);
		smBuffer = new SoftBuffer(65536);
		hmBuffer = new SoftBuffer(65536);
		dBuffer = new SoftBuffer(1000000);
		sdBuffer = new SoftBuffer(131072);
		hdBuffer = new SoftBuffer(131072);
		base.WordLength = 1;
		base.ByteTransform = new RegularByteTransform(DataFormat.CDAB);
	}

	[HslMqttApi("ReadByteArray", "")]
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		try
		{
			if (address.StartsWithAndNumber("D"))
			{
				return OperateResult.CreateSuccessResult(dBuffer.GetBytes(Convert.ToInt32(address.Substring(1)) * 2, length * 2));
			}
			if (address.StartsWithAndNumber("SD"))
			{
				return OperateResult.CreateSuccessResult(sdBuffer.GetBytes(Convert.ToInt32(address.Substring(2)) * 2, length * 2));
			}
			if (address.StartsWithAndNumber("HD"))
			{
				return OperateResult.CreateSuccessResult(hdBuffer.GetBytes(Convert.ToInt32(address.Substring(2)) * 2, length * 2));
			}
			return base.Read(address, length);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
	}

	[HslMqttApi("WriteByteArray", "")]
	public override OperateResult Write(string address, byte[] value)
	{
		try
		{
			if (address.StartsWithAndNumber("D"))
			{
				dBuffer.SetBytes(value, Convert.ToInt32(address.Substring(1)) * 2);
				return OperateResult.CreateSuccessResult();
			}
			if (address.StartsWithAndNumber("SD"))
			{
				sdBuffer.SetBytes(value, Convert.ToInt32(address.Substring(2)) * 2);
				return OperateResult.CreateSuccessResult();
			}
			if (address.StartsWithAndNumber("HD"))
			{
				hdBuffer.SetBytes(value, Convert.ToInt32(address.Substring(2)) * 2);
				return OperateResult.CreateSuccessResult();
			}
			return base.Write(address, value);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
	}

	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		try
		{
			if (address.StartsWithAndNumber("M"))
			{
				return OperateResult.CreateSuccessResult(mBuffer.GetBool(Convert.ToInt32(address.Substring(1)), length));
			}
			if (address.StartsWithAndNumber("SM"))
			{
				return OperateResult.CreateSuccessResult(smBuffer.GetBool(Convert.ToInt32(address.Substring(2)), length));
			}
			if (address.StartsWithAndNumber("HM"))
			{
				return OperateResult.CreateSuccessResult(hmBuffer.GetBool(Convert.ToInt32(address.Substring(2)), length));
			}
			return base.ReadBool(address, length);
		}
		catch (Exception ex)
		{
			return new OperateResult<bool[]>(ex.Message);
		}
	}

	[HslMqttApi("WriteBoolArray", "")]
	public override OperateResult Write(string address, bool[] value)
	{
		try
		{
			if (address.StartsWithAndNumber("M"))
			{
				mBuffer.SetBool(value, Convert.ToInt32(address.Substring(1)));
				return OperateResult.CreateSuccessResult();
			}
			if (address.StartsWithAndNumber("SM"))
			{
				smBuffer.SetBool(value, Convert.ToInt32(address.Substring(2)));
				return OperateResult.CreateSuccessResult();
			}
			if (address.StartsWithAndNumber("HM"))
			{
				hmBuffer.SetBool(value, Convert.ToInt32(address.Substring(2)));
				return OperateResult.CreateSuccessResult();
			}
			return base.Write(address, value);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(ex.Message);
		}
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		if (receive[7] == 32 || receive[7] == 30)
		{
			if (!base.StationDataIsolation && base.StationCheck && receive[6] != base.Station)
			{
				return new OperateResult<byte[]>($"Station not match XinJe, Need {base.Station} actual {receive[6]}");
			}
			return OperateResult.CreateSuccessResult(ReadByCommand(receive));
		}
		if (receive[7] == 33 || receive[7] == 31)
		{
			if (!base.StationDataIsolation && base.StationCheck && receive[6] != base.Station)
			{
				return new OperateResult<byte[]>($"Station not match XinJe, Need {base.Station} actual {receive[6]}");
			}
			return OperateResult.CreateSuccessResult(WriteByMessage(receive));
		}
		return base.ReadFromCoreServer(session, receive);
	}

	/// <summary>
	/// 将状态码，数据打包成一个完成的回复报文信息
	/// </summary>
	/// <param name="command">原始的命令数据</param>
	/// <param name="status">状态信息</param>
	/// <param name="data">数据</param>
	/// <returns>状态信息</returns>
	private byte[] PackCommand(byte[] command, ushort status, byte[] data)
	{
		if (data == null)
		{
			byte[] array = command.SelectBegin(14);
			array[4] = 0;
			array[5] = 8;
			if (status == 0)
			{
				return array;
			}
			array[7] = (byte)(array[7] + 128 + status);
			return array;
		}
		byte[] buffer = new byte[9 + data.Length];
		Array.Copy(command, 0, buffer, 0, 8);
		buffer[4] = 0;
		buffer[5] = (byte)(buffer.Length - 6);
		buffer[8] = (byte)data.Length;
		data.CopyTo(buffer, 9);
		return buffer;
	}

	private byte[] ReadByCommand(byte[] command)
	{
		ushort num = base.ByteTransform.TransUInt16(command, 12);
		int num2 = command[9] * 65536 + command[10] * 256 + command[11];
		byte b = command[8];
		if (command[7] == 32)
		{
			if (num > 125)
			{
				return PackCommand(command, 1, null);
			}
			return b switch
			{
				128 => PackCommand(command, 0, dBuffer.GetBytes(num2 * 2, num * 2)), 
				131 => PackCommand(command, 0, sdBuffer.GetBytes(num2 * 2, num * 2)), 
				136 => PackCommand(command, 0, hdBuffer.GetBytes(num2 * 2, num * 2)), 
				_ => PackCommand(command, 1, null), 
			};
		}
		if (command[7] == 30)
		{
			if (num > 2000)
			{
				return PackCommand(command, 1, null);
			}
			return b switch
			{
				3 => PackCommand(command, 0, mBuffer.GetBool(num2, num).ToByteArray()), 
				8 => PackCommand(command, 0, hmBuffer.GetBool(num2, num).ToByteArray()), 
				13 => PackCommand(command, 0, smBuffer.GetBool(num2, num).ToByteArray()), 
				_ => PackCommand(command, 1, null), 
			};
		}
		return PackCommand(command, 1, null);
	}

	private byte[] WriteByMessage(byte[] command)
	{
		if (!base.EnableWrite)
		{
			return PackCommand(command, 1, null);
		}
		ushort length = base.ByteTransform.TransUInt16(command, 12);
		int num = command[9] * 65536 + command[10] * 256 + command[11];
		byte b = command[8];
		if (command[7] == 33)
		{
			byte[] data = command.SelectMiddle(15, command[14]);
			switch (b)
			{
			case 128:
				dBuffer.SetBytes(data, num * 2);
				return PackCommand(command, 0, null);
			case 131:
				sdBuffer.SetBytes(data, num * 2);
				return PackCommand(command, 0, null);
			case 136:
				hdBuffer.SetBytes(data, num * 2);
				return PackCommand(command, 0, null);
			default:
				return PackCommand(command, 1, null);
			}
		}
		if (command[7] == 31)
		{
			bool[] value = command.SelectMiddle(15, command[14]).ToBoolArray().SelectBegin(length);
			switch (b)
			{
			case 3:
				mBuffer.SetBool(value, num);
				return PackCommand(command, 0, null);
			case 8:
				hmBuffer.SetBool(value, num);
				return PackCommand(command, 0, null);
			case 13:
				smBuffer.SetBool(value, num);
				return PackCommand(command, 0, null);
			default:
				return PackCommand(command, 1, null);
			}
		}
		return PackCommand(command, 1, null);
	}

	public override string ToString()
	{
		return $"XinJEServer[{base.Port}]";
	}
}
