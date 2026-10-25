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
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;

namespace HslCommunication.Profinet.Omron;

/// <summary>
/// 欧姆龙的虚拟服务器，支持DM区，CIO区，Work区，Hold区，Auxiliary区，可以方便的进行测试
/// Omron's virtual server supports DM area, CIO area, Work area, Hold area, and Auxiliary area, which can be easily tested
/// </summary>
public class OmronFinsUdpServer : OmronFinsServer
{
	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public OmronFinsUdpServer()
	{
		connectionInitialization = false;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return null;
	}

	protected override string GetLogTextFromBinary(PipeSession session, byte[] content)
	{
		string text = base.GetLogTextFromBinary(session, content);
		if (base.AnalysisLogMessage)
		{
			string text2 = OmronFinsNetHelper.AnalysisAddressLogMessage(session, base.ByteTransform, content, 10);
			text += text2;
		}
		return text;
	}

	protected override byte[] PackCommand(int status, byte[] finsCore, byte[] data)
	{
		if (data == null)
		{
			data = new byte[0];
		}
		byte[] buffer = new byte[14 + data.Length];
		SoftBasic.HexStringToBytes("C0 00 02 00 00 00 00 00 00 00 00 00 00 00").CopyTo(buffer, 0);
		if (data.Length != 0)
		{
			data.CopyTo(buffer, 14);
		}
		buffer[10] = finsCore[0];
		buffer[11] = finsCore[1];
		buffer[12] = BitConverter.GetBytes(status)[1];
		buffer[13] = BitConverter.GetBytes(status)[0];
		return buffer;
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		byte[] array = ReadFromFinsCore(receive.RemoveBegin(10));
		if (array != null)
		{
			array[4] = receive[7];
			array[7] = receive[4];
			if (base.CopySID)
			{
				array[9] = receive[9];
			}
			else
			{
				array[9] = 0;
			}
		}
		return OperateResult.CreateSuccessResult(array);
	}

	public override string ToString()
	{
		return $"OmronFinsUdpServer[{base.Port}]";
	}
}
