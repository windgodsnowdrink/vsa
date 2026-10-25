// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.ModBus;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Inovance;

/// <summary>
/// 汇川的串口转网口通信协议，A适用于AM400、 AM400_800、 AC800、H3U, XP, H5U 等系列底层走的是MODBUS-RTU协议，地址说明参见标记
/// Huichuan's serial communication protocol is applicable to AM400, AM400_800, AC800 and other series. The bottom layer is MODBUS-RTU protocol. For the address description, please refer to the mark
/// </summary>
/// <remarks>
/// AM400_800 的元件有 Q 区，I 区，M 区这三种，分别都可以按位，按字节，按字和按双字进行访问，在本组件的条件下，仅支持按照位，字访问。
/// 位地址支持 Q, I, M 地址类型，字地址支持 SM, SD，支持对字地址的位访问，例如 ReadBool("SD0.5");
/// H3U 系列控制器支持 M/SM/S/T/C/X/Y 等 bit 型变量（也称线圈） 的访问、 D/SD/R/T/C 等 word 型变量的访问；
/// H5U 系列控制器支持 M/B/S/X/Y 等 bit 型变量（也称线圈） 的访问、 D/R 等 word 型变量的访问；内部 W 元件，不支持通信访问。
/// </remarks>
public class InovanceSerialOverTcp : ModbusRtuOverTcp
{
	/// <summary>
	/// 获取或设置汇川的系列，默认为AM系列
	/// </summary>
	public InovanceSeries Series { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public InovanceSerialOverTcp()
	{
		Series = InovanceSeries.AM;
		base.DataFormat = DataFormat.CDAB;
	}

	/// <summary>
	/// 通过指定站号，ip地址，端口号来实例化一个新的对象
	/// </summary>
	/// <param name="ipAddress">Ip地址</param>
	/// <param name="port">端口号</param>
	/// <param name="station">站号信息</param>
	public InovanceSerialOverTcp(string ipAddress, int port = 502, byte station = 1)
		: base(ipAddress, port, station)
	{
		Series = InovanceSeries.AM;
		base.DataFormat = DataFormat.CDAB;
	}

	/// <summary>
	/// 通过指定站号，IP地址，端口以及PLC的系列来实例化一个新的对象
	/// Instantiate a new object by specifying the station number and PLC series
	/// </summary>
	/// <param name="series">PLC的系列</param>
	/// <param name="ipAddress">Ip地址</param>
	/// <param name="port">端口号</param>
	/// <param name="station">站号信息</param>
	public InovanceSerialOverTcp(InovanceSeries series, string ipAddress, int port = 502, byte station = 1)
		: base(ipAddress, port, station)
	{
		Series = series;
		Series = InovanceSeries.AM;
		base.DataFormat = DataFormat.CDAB;
	}

	public override OperateResult<string> TranslateToModbusAddress(string address, byte modbusCode)
	{
		return InovanceHelper.PraseInovanceAddress(Series, address, modbusCode);
	}

	public override OperateResult<string> ReadString(string address, ushort length, Encoding encoding)
	{
		if (Series == InovanceSeries.AM && Regex.IsMatch(address, "MB[0-9]*[13579]$", RegexOptions.IgnoreCase))
		{
			return InovanceHelper.ReadAMString(this, address, length, encoding);
		}
		return base.ReadString(address, length, encoding);
	}

	public override async Task<OperateResult<string>> ReadStringAsync(string address, ushort length, Encoding encoding)
	{
		if (Series == InovanceSeries.AM && Regex.IsMatch(address, "MB[0-9]*[13579]$", RegexOptions.IgnoreCase))
		{
			return await InovanceHelper.ReadAMStringAsync(this, address, length, encoding);
		}
		return await base.ReadStringAsync(address, length, encoding);
	}

	[HslMqttApi("ReadByte", "")]
	/// <summary>
	/// 按照字节读取汇川M地址的数据，地址示例： MB100，MB101，需要注意的是，MB100 及 MB101 的地址是 MW50 的数据。
	/// Read the data of Inovance M address according to the byte, address example: MB100, MB101, it should be noted that the addresses of MB100 and MB101 are the data of MW50.
	/// </summary>
	/// <param name="modbus">汇川的PLC对象</param>
	/// <param name="address">地址信息</param>
	/// <returns>读取的结果数据</returns>
	public OperateResult<byte> ReadByte(string address)
	{
		return InovanceHelper.ReadByte(this, address);
	}

	/// <summary>
	/// 按照字节读取汇川M地址的数据，地址示例： MB100，MB101，需要注意的是，MB100 及 MB101 的地址是 MW50 的数据。
	/// Read the data of Inovance M address according to the byte, address example: MB100, MB101, it should be noted that the addresses of MB100 and MB101 are the data of MW50.
	/// </summary>
	/// <param name="modbus">汇川的PLC对象</param>
	/// <param name="address">地址信息</param>
	/// <returns>读取的结果数据</returns>
	public async Task<OperateResult<byte>> ReadByteAsync(string address)
	{
		return await InovanceHelper.ReadByteAsync(this, address);
	}

	public override string ToString()
	{
		return $"InovanceSerialOverTcp<{Series}>[{IpAddress}:{Port}]";
	}
}
