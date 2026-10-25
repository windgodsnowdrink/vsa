// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Threading.Tasks;
using HslCommunication.Profinet.AllenBradley;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 三菱PLC的EIP协议的实现，当PLC使用了 QJ71EIP71 模块时就需要使用本类来访问
/// </summary>
public class MelsecCipNet : AllenBradleyNet
{
	/// <summary>
	/// Instantiate a communication object for a Allenbradley PLC protocol
	/// </summary>
	public MelsecCipNet()
	{
	}

	/// <summary>
	/// Instantiate a communication object for a Allenbradley PLC protocol
	/// </summary>
	/// <param name="ipAddress">PLC IpAddress</param>
	/// <param name="port">PLC Port</param>
	public MelsecCipNet(string ipAddress, int port = 44818)
		: base(ipAddress, port)
	{
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// Read data information, data length for read array length information
	/// </summary>
	/// <param name="address">Address format of the node</param>
	/// <param name="length">In the case of arrays, the length of the array</param>
	/// <returns>Result data with result object</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return Read(new string[1] { address }, new ushort[1] { length });
	}

	/// <summary>
	/// Read data information, data length for read array length information
	/// </summary>
	/// <param name="address">Address format of the node</param>
	/// <param name="length">In the case of arrays, the length of the array</param>
	/// <returns>Result data with result object</returns>
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		return await ReadAsync(new string[1] { address }, new ushort[1] { length });
	}
}
