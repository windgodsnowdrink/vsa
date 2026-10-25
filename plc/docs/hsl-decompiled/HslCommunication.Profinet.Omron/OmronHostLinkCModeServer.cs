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
using System.Text;
using HslCommunication.BasicFramework;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;

namespace HslCommunication.Profinet.Omron;

/// <summary>
/// 欧姆龙的HostLinkCMode协议的虚拟服务器
/// </summary>
public class OmronHostLinkCModeServer : OmronFinsServer
{
	private byte operationMode = 1;

	private SoftBuffer timerCounterBuffer = null;

	/// <summary>
	/// PLC设备的站号信息
	/// PLC device station number information
	/// </summary>
	public byte UnitNumber { get; set; }

	/// <summary>
	/// 实例化一个Fins协议的服务器
	/// Instantiate a Fins protocol server
	/// </summary>
	public OmronHostLinkCModeServer()
	{
		LogMsgFormatBinary = false;
		connectionInitialization = false;
		base.SerialReceiveAtleastTime = 2000;
		timerCounterBuffer = new SoftBuffer(131072);
	}

	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		try
		{
			if (address.StartsWith("CNT"))
			{
				return OperateResult.CreateSuccessResult((from m in timerCounterBuffer.GetBytes(Convert.ToInt32(address.Substring(3)) + 10000 + 2048, length)
					select m != 0).ToArray());
			}
			if (address.StartsWith("TIM"))
			{
				return OperateResult.CreateSuccessResult((from m in timerCounterBuffer.GetBytes(Convert.ToInt32(address.Substring(3)) + 10000, length)
					select m != 0).ToArray());
			}
		}
		catch (Exception ex)
		{
			return new OperateResult<bool[]>("Address Error: " + ex.Message);
		}
		return base.ReadBool(address, length);
	}

	public override OperateResult Write(string address, bool[] value)
	{
		try
		{
			if (address.StartsWith("CNT"))
			{
				timerCounterBuffer.SetBytes(value.Select((bool m) => (byte)(m ? 1 : 0)).ToArray(), Convert.ToInt32(address.Substring(3)) + 10000 + 2048);
				return OperateResult.CreateSuccessResult();
			}
			if (address.StartsWith("TIM"))
			{
				timerCounterBuffer.SetBytes(value.Select((bool m) => (byte)(m ? 1 : 0)).ToArray(), Convert.ToInt32(address.Substring(3)) + 10000);
				return OperateResult.CreateSuccessResult();
			}
		}
		catch (Exception ex)
		{
			return new OperateResult<bool[]>("Address Error: " + ex.Message);
		}
		return base.Write(address, value);
	}

	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		try
		{
			if (address.StartsWith("CNT"))
			{
				return OperateResult.CreateSuccessResult(timerCounterBuffer.GetBytes((Convert.ToInt32(address.Substring(3)) + 2048) * 2, length * 2));
			}
			if (address.StartsWith("TIM"))
			{
				return OperateResult.CreateSuccessResult(timerCounterBuffer.GetBytes(Convert.ToInt32(address.Substring(3)) * 2, length * 2));
			}
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("Address Error: " + ex.Message);
		}
		return base.Read(address, length);
	}

	public override OperateResult Write(string address, byte[] value)
	{
		try
		{
			if (address.StartsWith("CNT"))
			{
				timerCounterBuffer.SetBytes(value, (Convert.ToInt32(address.Substring(3)) + 2048) * 2);
				return OperateResult.CreateSuccessResult();
			}
			if (address.StartsWith("TIM"))
			{
				timerCounterBuffer.SetBytes(value, Convert.ToInt32(address.Substring(3)) * 2);
				return OperateResult.CreateSuccessResult();
			}
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("Address Error: " + ex.Message);
		}
		return base.Write(address, value);
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new SpecifiedCharacterMessage(13);
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		if (receive.Length < 9)
		{
			return new OperateResult<byte[]>("Uknown Data：" + SoftBasic.GetAsciiStringRender(receive));
		}
		return OperateResult.CreateSuccessResult(ReadFromFinsCore(receive));
	}

	protected override byte[] ReadFromFinsCore(byte[] finsCore)
	{
		string message = Encoding.ASCII.GetString(finsCore, 3, 2);
		switch (message)
		{
		default:
			if (!(message == "RG"))
			{
				switch (message)
				{
				default:
					if (!(message == "WC"))
					{
						switch (message)
						{
						case "MM":
							return PackCommand(0, finsCore, new byte[1] { 48 });
						case "MS":
							return PackCommand(0, finsCore, new byte[2] { operationMode, 48 });
						case "SC":
						{
							byte b = Convert.ToByte(Encoding.ASCII.GetString(finsCore, 5, 2), 16);
							if (b >= 0 && b <= 2)
							{
								operationMode = b;
							}
							return PackCommand(0, finsCore, null);
						}
						default:
							return PackCommand(22, finsCore, null);
						}
					}
					goto case "WD";
				case "WD":
				case "WR":
				case "WL":
				case "WH":
				case "WJ":
				case "WE":
				{
					SoftBuffer softBuffer = null;
					switch (message)
					{
					case "WD":
						softBuffer = dBuffer;
						break;
					case "WR":
						softBuffer = cioBuffer;
						break;
					case "WL":
						softBuffer = cioBuffer;
						break;
					case "WH":
						softBuffer = hBuffer;
						break;
					case "WJ":
						softBuffer = arBuffer;
						break;
					case "WE":
						softBuffer = emBuffer;
						break;
					case "WC":
						softBuffer = timerCounterBuffer;
						break;
					default:
						return PackCommand(22, finsCore, null);
					}
					if (message == "WE")
					{
						int num = Convert.ToInt32(Encoding.ASCII.GetString(finsCore, 7, 4));
						byte[] data = Encoding.ASCII.GetString(finsCore, 11, finsCore.Length - 15).ToHexBytes();
						softBuffer.SetBytes(data, num * 2);
						return PackCommand(0, finsCore, null);
					}
					if (message == "WC")
					{
						try
						{
							int num2 = Convert.ToInt32(Encoding.ASCII.GetString(finsCore, 5, 4));
							string message2 = Encoding.ASCII.GetString(finsCore, 9, finsCore.Length - 13);
							byte[] buffer = new byte[message2.Length / 2];
							for (int i = 0; i < buffer.Length / 2; i++)
							{
								ushort num3 = Convert.ToUInt16(message2.Substring(i * 4, 4));
								buffer[i * 2] = (byte)(num3 / 256);
								buffer[i * 2 + 1] = (byte)(num3 % 256);
							}
							softBuffer.SetBytes(buffer, num2 * 2);
							return PackCommand(0, finsCore, null);
						}
						catch (Exception ex)
						{
							base.LogNet?.WriteException("OmronHostLinkCModeServer WC command error", ex);
							return PackCommand(22, finsCore, null);
						}
					}
					int num4 = Convert.ToInt32(Encoding.ASCII.GetString(finsCore, 5, 4));
					byte[] data2 = Encoding.ASCII.GetString(finsCore, 9, finsCore.Length - 13).ToHexBytes();
					softBuffer.SetBytes(data2, num4 * 2);
					return PackCommand(0, finsCore, null);
				}
				}
			}
			goto case "RD";
		case "RD":
		case "RR":
		case "RL":
		case "RH":
		case "RJ":
		case "RE":
		case "RC":
		{
			SoftBuffer softBuffer2 = null;
			switch (message)
			{
			case "RD":
				softBuffer2 = dBuffer;
				break;
			case "RR":
				softBuffer2 = cioBuffer;
				break;
			case "RL":
				softBuffer2 = cioBuffer;
				break;
			case "RH":
				softBuffer2 = hBuffer;
				break;
			case "RJ":
				softBuffer2 = arBuffer;
				break;
			case "RE":
				softBuffer2 = emBuffer;
				break;
			case "RC":
				softBuffer2 = timerCounterBuffer;
				break;
			case "RG":
				softBuffer2 = timerCounterBuffer;
				break;
			default:
				return PackCommand(22, finsCore, null);
			}
			if (message == "RE")
			{
				int num5 = Convert.ToInt32(Encoding.ASCII.GetString(finsCore, 7, 4));
				ushort num6 = Convert.ToUInt16(Encoding.ASCII.GetString(finsCore, 11, 4));
				byte[] bytes = softBuffer2.GetBytes(num5 * 2, num6 * 2);
				return PackCommand(0, finsCore, bytes);
			}
			if (message == "RG")
			{
				int num7 = Convert.ToInt32(Encoding.ASCII.GetString(finsCore, 5, 4));
				ushort length = Convert.ToUInt16(Encoding.ASCII.GetString(finsCore, 9, 4));
				byte[] bytes2 = softBuffer2.GetBytes(num7 + 10000, length);
				return PackCommand(0, finsCore, bytes2);
			}
			int num8 = Convert.ToInt32(Encoding.ASCII.GetString(finsCore, 5, 4));
			ushort num9 = Convert.ToUInt16(Encoding.ASCII.GetString(finsCore, 9, 4));
			byte[] bytes3 = softBuffer2.GetBytes(num8 * 2, num9 * 2);
			return PackCommand(0, finsCore, bytes3);
		}
		}
	}

	private byte[] GetReturnBackData(byte[] data)
	{
		byte[] buffer = new byte[data.Length * 2];
		for (int i = 0; i < data.Length / 2; i++)
		{
			ushort num = base.ByteTransform.TransUInt16(data, i * 2);
			if (num > 9999)
			{
				num = 9999;
			}
			Encoding.ASCII.GetBytes(num.ToString("D4")).CopyTo(buffer, i * 4);
		}
		return buffer;
	}

	protected override byte[] PackCommand(int status, byte[] finsCore, byte[] data)
	{
		string message = Encoding.ASCII.GetString(finsCore, 3, 2);
		if (data == null)
		{
			data = new byte[0];
		}
		if (message == "RC")
		{
			data = GetReturnBackData(data);
		}
		else if (message == "RG")
		{
			for (int i = 0; i < data.Length; i++)
			{
				if (data[i] == 0)
				{
					data[i] = 48;
				}
				else if (data[i] == 1)
				{
					data[i] = 49;
				}
			}
		}
		else
		{
			data = SoftBasic.BytesToAsciiBytes(data);
		}
		byte[] buffer = new byte[11 + data.Length];
		Encoding.ASCII.GetBytes("@0000").CopyTo(buffer, 0);
		Encoding.ASCII.GetBytes(UnitNumber.ToString("X2")).CopyTo(buffer, 1);
		Array.Copy(finsCore, 3, buffer, 3, 2);
		Encoding.ASCII.GetBytes(status.ToString("X2")).CopyTo(buffer, 5);
		if (data.Length != 0)
		{
			data.CopyTo(buffer, 7);
		}
		int num = buffer[0];
		for (int j = 1; j < buffer.Length - 4; j++)
		{
			num ^= buffer[j];
		}
		SoftBasic.BuildAsciiBytesFrom((byte)num).CopyTo(buffer, buffer.Length - 4);
		buffer[^2] = 42;
		buffer[^1] = 13;
		return buffer;
	}

	protected override bool CheckSerialReceiveDataComplete(byte[] buffer, int receivedLength)
	{
		if (receivedLength > 1)
		{
			return buffer[receivedLength - 1] == 13;
		}
		return false;
	}

	public override string ToString()
	{
		return $"OmronHostLinkCModeServer[{base.Port}]";
	}
}
