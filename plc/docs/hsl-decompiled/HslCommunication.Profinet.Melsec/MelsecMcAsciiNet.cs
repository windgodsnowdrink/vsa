// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Profinet.Melsec.Helper;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 三菱PLC通讯类，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为ASCII通讯格式
/// Mitsubishi PLC communication class is implemented using Qna compatible 3E frame protocol.
/// The Ethernet module on the PLC side needs to be configured first. It must be ascii communication.
/// </summary>
public class MelsecMcAsciiNet : MelsecMcNet
{
	public override McType McType => McType.MCAscii;

	/// <summary>
	/// 实例化三菱的Qna兼容3E帧协议的通讯对象
	/// Instantiate the communication object of Mitsubishi's Qna compatible 3E frame protocol
	/// </summary>
	public MelsecMcAsciiNet()
	{
		base.WordLength = 1;
		LogMsgFormatBinary = false;
		base.ByteTransform = new RegularByteTransform();
	}

	/// <summary>
	/// 指定ip地址和端口号来实例化一个默认的对象
	/// Specify the IP address and port number to instantiate a default object
	/// </summary>
	/// <param name="ipAddress">PLC的Ip地址</param>
	/// <param name="port">PLC的端口</param>
	public MelsecMcAsciiNet(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new MelsecQnA3EAsciiMessage();
	}

	public override byte[] PackCommandWithHeader(byte[] command)
	{
		return McAsciiHelper.PackMcCommand(this, command);
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		OperateResult operateResult = McAsciiHelper.CheckResponseContent(response);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(response.RemoveBegin(22));
	}

	/// <summary>
	/// 从PLC反馈的数据中提取出实际的数据内容，需要传入反馈数据，是否位读取
	/// </summary>
	/// <param name="response">反馈的数据内容</param>
	/// <param name="isBit">是否位读取</param>
	/// <returns>解析后的结果对象</returns>
	public override byte[] ExtractActualData(byte[] response, bool isBit)
	{
		return McAsciiHelper.ExtractActualDataHelper(response, isBit);
	}

	public override string ToString()
	{
		return $"MelsecMcAsciiNet[{IpAddress}:{Port}]";
	}
}
