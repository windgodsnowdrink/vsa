// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Text;
using HslCommunication.BasicFramework;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 基于MC协议的A3C格式虚拟服务器，可以模拟A3C格式的PLC，支持格式1，2，3，4，具体可以使用来设置，
/// 支持设置是否校验，同时支持网口或是串口的访问
/// A3C format virtual server based on MC protocol can simulate A3C format PLC, support format 1, 2, 3, 4,
/// specifically you can use  to set, support whether to verify the setting,
/// and also support network Port or serial port access
/// </summary>
/// <remarks>
/// 可访问的地址支持M,X,Y,B,D,W,R,ZR地址，其中 M,X,Y,B 支持位访问
/// </remarks>
public class MelsecA3CServer : MelsecMcServer
{
	/// <summary>
	/// 当前A3C协议的站编号信息
	/// Station number information of the current A3C protocol
	/// </summary>
	public byte Station { get; set; }

	/// <summary>
	/// 当前的A3C协议是否使用和校验，默认使用
	/// Whether the current A3C protocol uses sum check, it is used by default
	/// </summary>
	public bool SumCheck { get; set; } = true;

	/// <summary>
	/// 当前的A3C协议的格式信息，可选格式1，2，3，4，默认格式1
	/// Format information of the current A3C protocol, optional format 1, 2, 3, 4, default format 1
	/// </summary>
	public virtual int Format { get; set; } = 1;

	/// <summary>
	/// 实例化一个虚拟的A3C服务器
	/// </summary>
	public MelsecA3CServer()
		: base(isBinary: false)
	{
		LogMsgFormatBinary = false;
		base.SerialReceiveAtleastTime = 2000;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new MelsecA3CServerMessage
		{
			Format = Format,
			SumCheck = SumCheck
		};
	}

	protected override bool CheckSerialReceiveDataComplete(byte[] buffer, int receivedLength)
	{
		try
		{
			return MelsecA3CServerMessage.CheckServer3CCommandComplete(buffer, Format, receivedLength, SumCheck);
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException("CheckSerialReceiveDataComplete", SoftBasic.GetAsciiStringRender(buffer.SelectBegin(receivedLength)), ex);
			return true;
		}
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		if (receive.Length < 3)
		{
			return new OperateResult<byte[]>("Uknown Data：" + receive.ToHexString(' '));
		}
		OperateResult<byte[]> operateResult = ExtraMcCore(receive);
		if (!operateResult.IsSuccess)
		{
			return new OperateResult<byte[]>(operateResult.Message);
		}
		return OperateResult.CreateSuccessResult((Format == 5) ? ReadFromMcCore(operateResult.Content) : ReadFromMcAsciiCore(operateResult.Content));
	}

	private static byte[] CalculatSum(byte[] command, int startLength, int endLength)
	{
		int num = 0;
		for (int i = startLength; i < command.Length - endLength; i++)
		{
			num += command[i];
		}
		return SoftBasic.BuildAsciiBytesFrom((byte)num);
	}

	/// <summary>
	/// 设置校验和的信息
	/// </summary>
	protected static void SetSumCheck(byte[] command, int startLength, int endLength, int skip = 0)
	{
		byte[] array = CalculatSum(command, startLength, endLength);
		command[command.Length - endLength + skip] = array[0];
		command[command.Length - endLength + 1 + skip] = array[1];
	}

	private bool CalculatSumCheck(byte[] command, int startLength, int endLength, int skip = 0)
	{
		byte[] array = CalculatSum(command, startLength, endLength);
		if (command[command.Length - endLength + skip] != array[0] || command[command.Length - endLength + 1 + skip] != array[1])
		{
			return false;
		}
		return true;
	}

	private OperateResult<byte[]> ExtraMcCore(byte[] command)
	{
		byte b = 0;
		b = ((Format == 2) ? byte.Parse(Encoding.ASCII.GetString(command, 5, 2)) : ((Format != 5) ? byte.Parse(Encoding.ASCII.GetString(command, 3, 2)) : command[5]));
		if (Station != b)
		{
			return new OperateResult<byte[]>($"Station Not Match, need: {Station}  but: {b}");
		}
		if (Format == 1)
		{
			if (command[0] != 5)
			{
				return new OperateResult<byte[]>("First Byte Must Start with 0x05");
			}
			if (SumCheck && !CalculatSumCheck(command, 1, 2))
			{
				return new OperateResult<byte[]>("Sum Check Failed!");
			}
		}
		else if (Format == 2)
		{
			if (command[0] != 5)
			{
				return new OperateResult<byte[]>("First Byte Must Start with 0x05");
			}
			if (SumCheck && !CalculatSumCheck(command, 1, 2))
			{
				return new OperateResult<byte[]>("Sum Check Failed!");
			}
		}
		else if (Format == 3)
		{
			if (command[0] != 2)
			{
				return new OperateResult<byte[]>("First Byte Must Start with 0x02");
			}
			if (SumCheck)
			{
				if (command[^3] != 3)
				{
					return new OperateResult<byte[]>("The last three Byte Must be 0x03");
				}
				if (!CalculatSumCheck(command, 1, 2))
				{
					return new OperateResult<byte[]>("Sum Check Failed!");
				}
			}
			else if (command[^1] != 3)
			{
				return new OperateResult<byte[]>("The last Byte Must be 0x03");
			}
		}
		else if (Format == 4)
		{
			if (command[0] != 5)
			{
				return new OperateResult<byte[]>("First Byte Must Start with 0x05");
			}
			if (command[^1] != 10)
			{
				return new OperateResult<byte[]>("The last Byte must be 0x0D,0x0A");
			}
			if (command[^2] != 13)
			{
				return new OperateResult<byte[]>("The last Byte must be 0x0D,0x0A");
			}
			if (SumCheck && !CalculatSumCheck(command, 1, 4))
			{
				return new OperateResult<byte[]>("Sum Check Failed!");
			}
		}
		else if (Format == 5)
		{
			if (command[0] != 16)
			{
				return new OperateResult<byte[]>("First Byte Must Start with 0x10");
			}
			if (command[1] != 2)
			{
				return new OperateResult<byte[]>("Second Byte Must be 0x02");
			}
			ushort num = BitConverter.ToUInt16(command, 2);
			int num2 = num + (SumCheck ? 8 : 6);
			if (command.Length != num2)
			{
				return new OperateResult<byte[]>("Length not correct");
			}
			if (SumCheck && !CalculatSumCheck(command, 2, 4, 2))
			{
				return new OperateResult<byte[]>("Sum Check Failed!");
			}
		}
		return ExtraMcCommand(command);
	}

	/// <summary>
	/// 从收到设备的报文信息里解析出实际的 MC 命令报文
	/// </summary>
	protected virtual OperateResult<byte[]> ExtraMcCommand(byte[] command)
	{
		if (Format == 1)
		{
			if (SumCheck)
			{
				return OperateResult.CreateSuccessResult(command.SelectMiddle(11, command.Length - 13));
			}
			return OperateResult.CreateSuccessResult(command.SelectMiddle(11, command.Length - 11));
		}
		if (Format == 2)
		{
			if (SumCheck)
			{
				return OperateResult.CreateSuccessResult(command.SelectMiddle(13, command.Length - 15));
			}
			return OperateResult.CreateSuccessResult(command.SelectMiddle(13, command.Length - 13));
		}
		if (Format == 3)
		{
			if (SumCheck)
			{
				return OperateResult.CreateSuccessResult(command.SelectMiddle(11, command.Length - 14));
			}
			return OperateResult.CreateSuccessResult(command.SelectMiddle(11, command.Length - 12));
		}
		if (Format == 4)
		{
			if (SumCheck)
			{
				return OperateResult.CreateSuccessResult(command.SelectMiddle(11, command.Length - 15));
			}
			return OperateResult.CreateSuccessResult(command.SelectMiddle(11, command.Length - 13));
		}
		return new OperateResult<byte[]>("Not Support Format:" + Format);
	}

	protected override byte[] PackCommand(ushort status, byte[] data)
	{
		if (data == null)
		{
			data = new byte[0];
		}
		if (data.Length == 0)
		{
			if (Format == 1)
			{
				if (status == 0)
				{
					byte[] bytes = Encoding.ASCII.GetBytes("\u0006F90000FF00");
					SoftBasic.BuildAsciiBytesFrom(Station).CopyTo(bytes, 3);
					return bytes;
				}
				byte[] bytes2 = Encoding.ASCII.GetBytes("\u0015F90000FF000000");
				SoftBasic.BuildAsciiBytesFrom(Station).CopyTo(bytes2, 3);
				SoftBasic.BuildAsciiBytesFrom(status).CopyTo(bytes2, bytes2.Length - 4);
				return bytes2;
			}
			if (Format == 2)
			{
				if (status == 0)
				{
					byte[] bytes3 = Encoding.ASCII.GetBytes("\u000600F90000FF00");
					SoftBasic.BuildAsciiBytesFrom(Station).CopyTo(bytes3, 5);
					return bytes3;
				}
				byte[] bytes4 = Encoding.ASCII.GetBytes("\u001500F90000FF000000");
				SoftBasic.BuildAsciiBytesFrom(Station).CopyTo(bytes4, 5);
				SoftBasic.BuildAsciiBytesFrom(status).CopyTo(bytes4, bytes4.Length - 4);
				return bytes4;
			}
			if (Format == 3)
			{
				if (status == 0)
				{
					byte[] bytes5 = Encoding.ASCII.GetBytes("\u0002F90000FF00QACK\u0003");
					SoftBasic.BuildAsciiBytesFrom(Station).CopyTo(bytes5, 3);
					return bytes5;
				}
				byte[] bytes6 = Encoding.ASCII.GetBytes("\u0002F90000FF00QNAK0000\u0003");
				SoftBasic.BuildAsciiBytesFrom(Station).CopyTo(bytes6, 3);
				SoftBasic.BuildAsciiBytesFrom(status).CopyTo(bytes6, bytes6.Length - 5);
				return bytes6;
			}
			if (Format == 4)
			{
				if (status == 0)
				{
					byte[] bytes7 = Encoding.ASCII.GetBytes("\u0006F90000FF00");
					SoftBasic.BuildAsciiBytesFrom(Station).CopyTo(bytes7, 3);
					return bytes7;
				}
				byte[] bytes8 = Encoding.ASCII.GetBytes("\u0015F90000FF000000\r\n");
				SoftBasic.BuildAsciiBytesFrom(Station).CopyTo(bytes8, 3);
				SoftBasic.BuildAsciiBytesFrom(status).CopyTo(bytes8, bytes8.Length - 6);
				return bytes8;
			}
			return null;
		}
		if (Format == 1)
		{
			if (status != 0)
			{
				byte[] bytes9 = Encoding.ASCII.GetBytes("\u0015F90000FF000000");
				SoftBasic.BuildAsciiBytesFrom(Station).CopyTo(bytes9, 3);
				SoftBasic.BuildAsciiBytesFrom(status).CopyTo(bytes9, bytes9.Length - 4);
				return bytes9;
			}
			byte[] buffer = new byte[(SumCheck ? 14 : 12) + data.Length];
			Encoding.ASCII.GetBytes("\u0002F90000FF00").CopyTo(buffer, 0);
			SoftBasic.BuildAsciiBytesFrom(Station).CopyTo(buffer, 3);
			data.CopyTo(buffer, 11);
			buffer[^((!SumCheck) ? 1 : 3)] = 3;
			if (SumCheck)
			{
				SetSumCheck(buffer, 1, 2);
			}
			return buffer;
		}
		if (Format == 2)
		{
			if (status != 0)
			{
				byte[] bytes10 = Encoding.ASCII.GetBytes("\u001500F90000FF000000");
				SoftBasic.BuildAsciiBytesFrom(Station).CopyTo(bytes10, 5);
				SoftBasic.BuildAsciiBytesFrom(status).CopyTo(bytes10, bytes10.Length - 4);
				return bytes10;
			}
			byte[] buffer2 = new byte[(SumCheck ? 16 : 14) + data.Length];
			Encoding.ASCII.GetBytes("\u000200F90000FF00").CopyTo(buffer2, 0);
			SoftBasic.BuildAsciiBytesFrom(Station).CopyTo(buffer2, 5);
			data.CopyTo(buffer2, 13);
			buffer2[^((!SumCheck) ? 1 : 3)] = 3;
			if (SumCheck)
			{
				SetSumCheck(buffer2, 1, 2);
			}
			return buffer2;
		}
		if (Format == 3)
		{
			if (status != 0)
			{
				byte[] bytes11 = Encoding.ASCII.GetBytes("\u0002F90000FF00QNAK0000\u0003");
				SoftBasic.BuildAsciiBytesFrom(Station).CopyTo(bytes11, 3);
				SoftBasic.BuildAsciiBytesFrom(status).CopyTo(bytes11, bytes11.Length - 5);
				return bytes11;
			}
			byte[] buffer3 = new byte[(SumCheck ? 18 : 16) + data.Length];
			Encoding.ASCII.GetBytes("\u0002F90000FF00QACK").CopyTo(buffer3, 0);
			SoftBasic.BuildAsciiBytesFrom(Station).CopyTo(buffer3, 3);
			buffer3[^((!SumCheck) ? 1 : 3)] = 3;
			data.CopyTo(buffer3, 15);
			if (SumCheck)
			{
				SetSumCheck(buffer3, 1, 2);
			}
			return buffer3;
		}
		if (Format == 4)
		{
			if (status != 0)
			{
				byte[] bytes12 = Encoding.ASCII.GetBytes("\u0015F90000FF000000\r\n");
				SoftBasic.BuildAsciiBytesFrom(Station).CopyTo(bytes12, 3);
				SoftBasic.BuildAsciiBytesFrom(status).CopyTo(bytes12, bytes12.Length - 6);
				return bytes12;
			}
			byte[] buffer4 = new byte[(SumCheck ? 16 : 14) + data.Length];
			Encoding.ASCII.GetBytes("\u0002F90000FF00").CopyTo(buffer4, 0);
			SoftBasic.BuildAsciiBytesFrom(Station).CopyTo(buffer4, 3);
			buffer4[^(SumCheck ? 5 : 3)] = 3;
			data.CopyTo(buffer4, 11);
			if (SumCheck)
			{
				SetSumCheck(buffer4, 1, 4);
			}
			buffer4[^2] = 13;
			buffer4[^1] = 10;
			return buffer4;
		}
		return null;
	}

	public override string ToString()
	{
		return $"MelsecA3CServer[{base.Port}]";
	}
}
