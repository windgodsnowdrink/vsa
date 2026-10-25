// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.Core.IMessage;

namespace HslCommunication.ModBus;

/// <summary>
/// Modbus-Ascii通讯协议的网口透传类，基于rtu类库完善过来，支持标准的功能码，也支持扩展的功能码实现，地址采用富文本的形式，详细见备注说明
/// The client communication class of Modbus-Ascii protocol is convenient for data interaction with the server. It supports standard function codes and also supports extended function codes.
/// The address is in rich text. For details, see the remarks.
/// </summary>
/// <remarks>
/// 本客户端支持的标准的modbus-ascii协议，地址支持富文本格式，具体参考示例代码。
/// 读取线圈，输入线圈，寄存器，输入寄存器的方法中的读取长度对商业授权用户不限制，内部自动切割读取，结果合并。
/// </remarks>
public class ModbusAsciiOverTcp : ModbusRtuOverTcp
{
	/// <summary>
	/// 实例化一个Modbus-ascii协议的客户端对象
	/// Instantiate a client object of the Modbus-ascii protocol
	/// </summary>
	public ModbusAsciiOverTcp()
	{
		LogMsgFormatBinary = false;
	}

	/// <summary>
	/// 指定服务器地址，端口号，客户端自己的站号来初始化
	/// Specify the server address, port number, and client's own station number to initialize
	/// </summary>
	/// <param name="ipAddress">服务器的Ip地址</param>
	/// <param name="port">服务器的端口号</param>
	/// <param name="station">客户端自身的站号</param>
	public ModbusAsciiOverTcp(string ipAddress, int port = 502, byte station = 1)
		: base(ipAddress, port, station)
	{
		LogMsgFormatBinary = false;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new SpecifiedCharacterMessage(13, 10);
	}

	public override byte[] PackCommandWithHeader(byte[] command)
	{
		return ModbusInfo.TransModbusCoreToAsciiPackCommand(command);
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return ModbusHelper.ExtraAsciiResponseContent(send, response, base.BroadcastStation);
	}

	public override string ToString()
	{
		return $"ModbusAsciiOverTcp[{IpAddress}:{Port}]";
	}
}
