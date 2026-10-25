// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Threading.Tasks;
using HslCommunication.Core.Address;
using HslCommunication.Profinet.Melsec;
using HslCommunication.Profinet.Melsec.Helper;

namespace HslCommunication.Profinet.Keyence;

/// <summary>
/// 基恩士PLC的数据通信类，使用QnA兼容3E帧的通信协议实现，使用ASCII的格式，地址格式需要进行转换成三菱的格式，详细参照备注说明
/// Keyence PLC's data communication class is implemented using QnA compatible 3E frame communication protocol.
/// It uses ascii format. The address format needs to be converted to Mitsubishi format.
/// </summary>
public class KeyenceMcAsciiNet : MelsecMcAsciiNet
{
	/// <summary>
	/// 实例化基恩士的Qna兼容3E帧协议的通讯对象
	/// Instantiate Keyence Qna compatible 3E frame protocol communication object
	/// </summary>
	public KeyenceMcAsciiNet()
	{
	}

	/// <summary>
	/// 指定ip地址及端口号来实例化一个基恩士的Qna兼容3E帧协议的通讯对象
	/// Specify an IP address and port number to instantiate a Keynes Qna compatible 3E frame protocol communication object
	/// </summary>
	/// <param name="ipAddress">PLC的Ip地址</param>
	/// <param name="port">PLC的端口</param>
	public KeyenceMcAsciiNet(string ipAddress, int port)
		: base(ipAddress, port)
	{
	}

	public override OperateResult<McAddressData> McAnalysisAddress(string address, ushort length, bool isBit)
	{
		return McAddressData.ParseKeyenceFrom(address, length, isBit);
	}

	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		if (KeyenceMcNet.CheckKeyenceBoolAddress(address))
		{
			return McHelper.ReadBool(this, address, length, supportWordAdd: false);
		}
		return base.ReadBool(address, length);
	}

	public override OperateResult Write(string address, bool[] values)
	{
		if (KeyenceMcNet.CheckKeyenceBoolAddress(address))
		{
			return McHelper.Write(this, address, values, supportWordAdd: false);
		}
		return base.Write(address, values);
	}

	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		if (KeyenceMcNet.CheckKeyenceBoolAddress(address))
		{
			return await McHelper.ReadBoolAsync(this, address, length, supportWordAdd: false).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.ReadBoolAsync(address, length).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, bool[] values)
	{
		if (KeyenceMcNet.CheckKeyenceBoolAddress(address))
		{
			return await McHelper.WriteAsync(this, address, values, supportWordAdd: false).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, values).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override string ToString()
	{
		return $"KeyenceMcAsciiNet[{IpAddress}:{Port}]";
	}
}
