// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Diagnostics;
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.ModBus;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.MegMeet;

/// <summary>
/// 深圳麦格米特PLC的通信对象，基于ModbusTcp协议实现，适用机型为 MC80/MC100/MC200/MC280/MC200E，具体支持的地址及范围参见DEMO界面
/// The communication object of Shenzhen MegMeet PLC is based on the ModbusTcp, and the applicable model is MC80/MC100/MC200/MC280/MC200E,
/// and the specific supported address and range are described in Demo UI
/// </summary>
/// <remarks>
/// 位读写地址支持：X,Y,M,SM,S,T,C，字读写地址为：D,SD,Z,R,T,C，期中 C200以上使用int/uint类型进行读写操作
/// </remarks>
public class MegMeetTcpNet : ModbusTcpNet
{
	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public MegMeetTcpNet()
	{
		base.DataFormat = DataFormat.CDAB;
	}

	/// <summary>
	/// 通过指定站号，ip地址，端口号来实例化一个新的对象
	/// </summary>
	/// <param name="ipAddress">Ip地址</param>
	/// <param name="port">端口号</param>
	/// <param name="station">站号信息</param>
	public MegMeetTcpNet(string ipAddress, int port = 502, byte station = 1)
		: base(ipAddress, port, station)
	{
		base.DataFormat = DataFormat.CDAB;
	}

	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		return MegMeetHelper.ReadBool(base.ReadBool, address, length);
	}

	[HslMqttApi("ReadByteArray", "")]
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return MegMeetHelper.Read(base.Read, address, length);
	}

	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		return await MegMeetHelper.ReadBoolAsync([DebuggerHidden] (string address2, ushort length2) => base.ReadBoolAsync(address2, length2), address, length);
	}

	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		return await MegMeetHelper.ReadAsync([DebuggerHidden] (string address2, ushort length2) => base.ReadAsync(address2, length2), address, length);
	}

	public override OperateResult<string> TranslateToModbusAddress(string address, byte modbusCode)
	{
		return MegMeetHelper.PraseMegMeetAddress(address, modbusCode);
	}

	public override string ToString()
	{
		return $"MegMeetTcpNet[{IpAddress}:{Port}]";
	}
}
