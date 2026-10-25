// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using HslCommunication.Core.Address;
using HslCommunication.Profinet.Melsec;

namespace HslCommunication.Profinet.Panasonic;

/// <summary>
/// 松下PLC的数据读写类，基于MC协议的实现，具体的地址格式请参考备注说明
/// Data reading and writing of Panasonic PLC, based on the implementation of the MC protocol, please refer to the note for specific address format
/// </summary>
public class PanasonicMcNet : MelsecMcNet
{
	/// <summary>
	/// 实例化松下的的Qna兼容3E帧协议的通讯对象
	/// Instantiate Panasonic's Qna compatible 3E frame protocol communication object
	/// </summary>
	public PanasonicMcNet()
	{
	}

	/// <summary>
	/// 指定ip地址及端口号来实例化一个松下的Qna兼容3E帧协议的通讯对象
	/// Specify an IP address and port number to instantiate a Panasonic Qna compatible 3E frame protocol communication object
	/// </summary>
	/// <param name="ipAddress">PLC的Ip地址</param>
	/// <param name="port">PLC的端口</param>
	public PanasonicMcNet(string ipAddress, int port)
		: base(ipAddress, port)
	{
	}

	public override OperateResult<McAddressData> McAnalysisAddress(string address, ushort length, bool isBit)
	{
		return McAddressData.ParsePanasonicFrom(address, length, isBit);
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		try
		{
			ushort num = BitConverter.ToUInt16(response, 9);
			if (num != 0)
			{
				return new OperateResult<byte[]>(num, PanasonicHelper.GetMcErrorDescription(num));
			}
			return OperateResult.CreateSuccessResult(response.RemoveBegin(11));
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("UnpackResponseContent failed: " + ex.Message + " Source: " + response.ToHexString(' '));
		}
	}

	public override string ToString()
	{
		return $"PanasonicMcNet[{IpAddress}:{Port}]";
	}
}
