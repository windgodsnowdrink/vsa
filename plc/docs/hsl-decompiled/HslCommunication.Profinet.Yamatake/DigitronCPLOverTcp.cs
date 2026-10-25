// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Profinet.Yamatake.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Yamatake;

/// <summary>
/// 山武的数字指定调节器的通信协议，基于CPL转网口的实现，测试型号 SDC40B
/// </summary>
public class DigitronCPLOverTcp : DeviceTcpNet
{
	/// <summary>
	/// 获取或设置当前的站号信息
	/// </summary>
	public byte Station { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public DigitronCPLOverTcp()
	{
		Station = 1;
		base.WordLength = 1;
		base.ByteTransform = new RegularByteTransform();
		LogMsgFormatBinary = false;
	}

	/// <summary>
	/// 指定服务器地址，端口号，客户端自己的站号来初始化
	/// Specify the server address, port number, and client's own station number to initialize
	/// </summary>
	/// <param name="ipAddress">服务器的Ip地址</param>
	/// <param name="port">服务器的端口号</param>
	/// <param name="station">客户端自身的站号</param>
	public DigitronCPLOverTcp(string ipAddress, int port = 502, byte station = 1)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
		Station = station;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new SpecifiedCharacterMessage(13, 10);
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return DigitronCPLHelper.ExtraActualResponse(response);
	}

	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, byte[] value)
	{
		return DigitronCPLHelper.BuildWriteCommand(Station, address, value);
	}

	[HslMqttApi("ReadByteArray", "")]
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<byte[]> operateResult = DigitronCPLHelper.BuildReadCommand(Station, address, length);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return ReadFromCoreServer(operateResult.Content);
	}

	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		OperateResult<byte[]> command = DigitronCPLHelper.BuildReadCommand(Station, address, length);
		if (!command.IsSuccess)
		{
			return command;
		}
		return await ReadFromCoreServerAsync(command.Content);
	}

	public override string ToString()
	{
		return $"DigitronCPLOverTcp[{IpAddress}:{Port}]";
	}
}
