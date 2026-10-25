// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Text;
using HslCommunication.BasicFramework;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;

namespace HslCommunication.Profinet.Omron;

/// <summary>
/// 欧姆龙的HostLink虚拟服务器，支持DM区，CIO区，Work区，Hold区，Auxiliary区，可以方便的进行测试
/// Omron's HostLink virtual server supports DM area, CIO area, Work area, Hold area, and Auxiliary area, which can be easily tested
/// </summary>
/// <remarks>
/// 支持TCP的接口以及串口，方便客户端进行测试，或是开发用于教学的虚拟服务器对象
/// </remarks>
public class OmronHostLinkServer : OmronFinsServer
{
	/// <summary>
	/// PLC设备的站号信息
	/// PLC device station number information
	/// </summary>
	public byte UnitNumber { get; set; }

	/// <summary>
	/// 实例化一个Fins协议的服务器
	/// Instantiate a Fins protocol server
	/// </summary>
	public OmronHostLinkServer()
	{
		connectionInitialization = false;
		LogMsgFormatBinary = false;
		base.SerialReceiveAtleastTime = 2000;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new SpecifiedCharacterMessage(13);
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		if (receive.Length < 22)
		{
			return new OperateResult<byte[]>("Uknown Data：" + receive.ToHexString(' '));
		}
		string hex = Encoding.ASCII.GetString(receive, 14, receive.Length - 18);
		byte[] array = ReadFromFinsCore(SoftBasic.HexStringToBytes(hex));
		array[13] = receive[12];
		array[14] = receive[13];
		return OperateResult.CreateSuccessResult(array);
	}

	protected override byte[] PackCommand(int status, byte[] finsCore, byte[] data)
	{
		if (data == null)
		{
			data = new byte[0];
		}
		data = SoftBasic.BytesToAsciiBytes(data);
		byte[] buffer = new byte[27 + data.Length];
		Encoding.ASCII.GetBytes("@00FA0040000000").CopyTo(buffer, 0);
		Encoding.ASCII.GetBytes(UnitNumber.ToString("X2")).CopyTo(buffer, 1);
		if (data.Length != 0)
		{
			data.CopyTo(buffer, 23);
		}
		Encoding.ASCII.GetBytes(finsCore.SelectBegin(2).ToHexString()).CopyTo(buffer, 15);
		Encoding.ASCII.GetBytes(status.ToString("X4")).CopyTo(buffer, 19);
		int num = buffer[0];
		for (int i = 1; i < buffer.Length - 4; i++)
		{
			num ^= buffer[i];
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
		return $"OmronHostLinkServer[{base.Port}]";
	}
}
