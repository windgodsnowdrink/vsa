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
using HslCommunication.ModBus;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.XINJE;

/// <summary>
/// 信捷PLC的XC,XD,XL系列的网口通讯类，底层使用ModbusTcp协议实现，每个系列支持的地址类型及范围不一样，详细参考Demo程序
/// XC, XD, XL series of Xinje PLC's network port communication class, the bottom layer is realized by ModbusTcp protocol,
/// each series supports different address types and ranges, please refer to the Demo for details
/// </summary>
/// <remarks>
/// 对于XC系列适用于XC1/XC2/XC3/XC5/XCM/XCC系列，线圈支持X,Y,S,M,T,C，寄存器支持D,F,E,T,C
/// 对于XD,XL系列适用于XD1/XD2/XD3/XD5/XDM/XDC/XD5E/XDME/XDH/XL1/XL3/XL5/XL5E/XLME，
/// 线圈支持X,Y,S,M,SM,T,C,ET,SEM,HM,HS,HT,HC,HSC 寄存器支持D,ID,QD,SD,TD,CD,ETD,HD,HSD,HTD,HCD,HSCD,FD,SFD,FS
/// </remarks>
public class XinJETcpNet : ModbusTcpNet
{
	/// <summary>
	/// 获取或设置当前的信捷PLC的系列，默认XC系列
	/// </summary>
	public XinJESeries Series { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public XinJETcpNet()
	{
		Series = XinJESeries.XC;
	}

	/// <summary>
	/// 通过指定站号，ip地址，端口号来实例化一个新的对象
	/// </summary>
	/// <param name="ipAddress">Ip地址</param>
	/// <param name="port">端口号</param>
	/// <param name="station">站号信息</param>
	public XinJETcpNet(string ipAddress, int port = 502, byte station = 1)
		: base(ipAddress, port, station)
	{
		Series = XinJESeries.XC;
	}

	/// <summary>
	/// 通过指定站号，IP地址，端口以及PLC的系列来实例化一个新的对象
	/// Instantiate a new object by specifying the station number and PLC series
	/// </summary>
	/// <param name="series">PLC的系列</param>
	/// <param name="ipAddress">Ip地址</param>
	/// <param name="port">端口号</param>
	/// <param name="station">站号信息</param>
	public XinJETcpNet(XinJESeries series, string ipAddress, int port = 502, byte station = 1)
		: base(ipAddress, port, station)
	{
		Series = series;
	}

	public override OperateResult<string> TranslateToModbusAddress(string address, byte modbusCode)
	{
		return XinJEHelper.PraseXinJEAddress(Series, address, modbusCode);
	}

	[HslMqttApi("ReadByteArray", "")]
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return XinJEHelper.Read(this, address, length, base.Read);
	}

	[HslMqttApi("WriteByteArray", "")]
	public override OperateResult Write(string address, byte[] value)
	{
		return XinJEHelper.Write(this, address, value, base.Write);
	}

	[HslMqttApi("WriteInt16", "")]
	public override OperateResult Write(string address, short value)
	{
		return XinJEHelper.Write(this, address, value, base.Write);
	}

	[HslMqttApi("WriteUInt16", "")]
	public override OperateResult Write(string address, ushort value)
	{
		return XinJEHelper.Write(this, address, value, base.Write);
	}

	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		return XinJEHelper.ReadBool(this, address, length, base.ReadBool);
	}

	[HslMqttApi("WriteBoolArray", "")]
	public override OperateResult Write(string address, bool[] values)
	{
		return XinJEHelper.Write(this, address, values, base.Write);
	}

	[HslMqttApi("WriteBool", "")]
	public override OperateResult Write(string address, bool value)
	{
		return XinJEHelper.Write(this, address, value, base.Write);
	}

	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		return await XinJEHelper.ReadAsync(this, address, length, [DebuggerHidden] (string address2, ushort length2) => base.ReadAsync(address2, length2));
	}

	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await XinJEHelper.WriteAsync(this, address, value, [DebuggerHidden] (string address2, byte[] value2) => base.WriteAsync(address2, value2));
	}

	public override async Task<OperateResult> WriteAsync(string address, short value)
	{
		return await XinJEHelper.WriteAsync(this, address, value, [DebuggerHidden] (string address2, short value2) => base.WriteAsync(address2, value2));
	}

	public override async Task<OperateResult> WriteAsync(string address, ushort value)
	{
		return await XinJEHelper.WriteAsync(this, address, value, [DebuggerHidden] (string address2, ushort value2) => base.WriteAsync(address2, value2));
	}

	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		return await XinJEHelper.ReadBoolAsync(this, address, length, [DebuggerHidden] (string address2, ushort length2) => base.ReadBoolAsync(address2, length2));
	}

	public override async Task<OperateResult> WriteAsync(string address, bool[] values)
	{
		return await XinJEHelper.WriteAsync(this, address, values, [DebuggerHidden] (string address2, bool[] values2) => base.WriteAsync(address2, values2));
	}

	public override async Task<OperateResult> WriteAsync(string address, bool value)
	{
		return await XinJEHelper.WriteAsync(this, address, value, [DebuggerHidden] (string address2, bool value2) => base.WriteAsync(address2, value2));
	}

	public override string ToString()
	{
		return $"XinJETcpNet<{Series}>[{IpAddress}:{Port}]";
	}
}
