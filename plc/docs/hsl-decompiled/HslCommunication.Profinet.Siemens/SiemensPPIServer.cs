// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Collections.Generic;
using HslCommunication.BasicFramework;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Core.Pipe;
using HslCommunication.Serial;

namespace HslCommunication.Profinet.Siemens;

/// <summary>
/// PPIServer的虚拟服务器对象，支持的地址类型和S7虚拟服务器一致
/// </summary>
public class SiemensPPIServer : SiemensS7Server
{
	/// <summary>
	/// 西门子PLC的站号信息
	/// Siemens PLC station number information
	/// </summary>
	public byte Station { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public SiemensPPIServer()
	{
		base.SerialReceiveAtleastTime = 2000;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new SiemensPPIServerMessage();
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		CommunicationPipe communication = session.Communication;
		if (receive[4] != Station)
		{
			return new OperateResult<byte[]>($"Station not match, expect: {Station}, but actual: {receive[4]}");
		}
		byte[] buffer = new byte[1] { 229 };
		LogSendMessage(buffer, session);
		communication.Send(buffer);
		OperateResult<byte[]> operateResult = communication.ReceiveMessage(new SpecifiedCharacterMessage(22), null, useActivePush: false, null, delegate(byte[] m)
		{
			LogRevcMessage(m, session);
		});
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return base.ReadFromCoreServer(session, receive);
	}

	protected override bool IsNeedShakeHands()
	{
		return false;
	}

	protected override byte[] PackReadBack(byte[] command, List<byte> content)
	{
		byte[] buffer = new byte[21 + content.Count + 2];
		SoftBasic.HexStringToBytes("68 1D 1D 68 00 02 08 32 03 00 00 00 00 00 02 00 0C 00 00 04 01").CopyTo(buffer, 0);
		buffer[1] = (byte)(buffer.Length - 6);
		buffer[2] = (byte)(buffer.Length - 6);
		buffer[15] = (byte)(content.Count / 256);
		buffer[16] = (byte)(content.Count % 256);
		buffer[20] = command[18];
		content.CopyTo(buffer, 21);
		buffer[^2] = (byte)SoftLRC.CalculateAcc(buffer, 4, 2);
		buffer[^1] = 22;
		return buffer;
	}

	protected override byte[] PackWriteBack(byte[] packCommand, byte[] status)
	{
		byte[] buffer = new byte[23 + status.Length];
		SoftBasic.HexStringToBytes("68 12 12 68 00 02 08 32 03 00 00 00 01 00 02 00 01 00 00 05 01 04 00 16").CopyTo(buffer, 0);
		buffer[20] = (byte)status.Length;
		status.CopyTo(buffer, 21);
		buffer[^2] = (byte)SoftLRC.CalculateAcc(buffer, 4, 2);
		buffer[^1] = 22;
		return buffer;
	}

	protected override bool CheckSerialReceiveDataComplete(byte[] buffer, int receivedLength)
	{
		if (receivedLength == 6 && buffer[0] == 16 && buffer[1] == 2 && buffer[5] == 22)
		{
			return true;
		}
		if (receivedLength > 6 && buffer[0] == 104 && buffer[1] + 6 == receivedLength && buffer[receivedLength - 1] == 22)
		{
			return true;
		}
		return base.CheckSerialReceiveDataComplete(buffer, receivedLength);
	}

	public override string ToString()
	{
		return $"SiemensPPIServer[{base.Port}]";
	}
}
