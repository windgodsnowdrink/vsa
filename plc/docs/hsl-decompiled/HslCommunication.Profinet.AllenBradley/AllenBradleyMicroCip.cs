// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Profinet.AllenBradley;

/// <summary>
/// AB PLC的cip通信实现类，适用Micro800系列控制系统
/// AB PLC's cip communication implementation class, suitable for Micro800 series control system
/// </summary>
public class AllenBradleyMicroCip : AllenBradleyNet
{
	/// <summary>
	/// Instantiate a communication object for a Allenbradley PLC protocol
	/// </summary>
	public AllenBradleyMicroCip()
	{
	}

	/// <summary>
	/// Instantiate a communication object for a Allenbradley PLC protocol
	/// </summary>
	/// <param name="ipAddress">PLC IpAddress</param>
	/// <param name="port">PLC Port</param>
	public AllenBradleyMicroCip(string ipAddress, int port = 44818)
		: base(ipAddress, port)
	{
	}

	protected override byte[] PackCommandService(byte[] portSlot, params byte[][] cips)
	{
		return AllenBradleyHelper.PackCleanCommandService(portSlot, cips);
	}

	public override string ToString()
	{
		return $"AllenBradleyMicroCip[{IpAddress}:{Port}]";
	}
}
