// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Instrument.DLT.Helper;

namespace HslCommunication.Instrument.DLT;

/// <summary>
/// 698.45协议的TCP通信类(不是串口透传通信)，面向对象的用电信息数据交换协议，使用明文的通信方式。支持读取功率，总功，电压，电流，频率，功率因数等数据。
/// The TCP communication class of the 698.45 protocol (not the serial port transparent transmission communication), the object-oriented power consumption information data exchange protocol,
/// uses the clear text communication method. Support reading power, total power, voltage, current, frequency, power factor and other data.
/// </summary>
public class DLT698TcpNet : DLT698OverTcp, IDlt698, IReadWriteDevice, IReadWriteNet
{
	/// <summary>
	/// 默认的无参构造函数
	/// Default no-parameter constructor
	/// </summary>
	public DLT698TcpNet()
	{
	}

	/// <summary>
	/// 指定地址域，密码，操作者代码来实例化一个对象，密码及操作者代码在写入操作的时候进行验证
	/// Specify the address field, password, and operator code to instantiate an object, and the password and operator code are validated during write operations,
	/// which address field is a 12-character BCD code, for example: 149100007290
	/// </summary>
	/// <param name="station">设备的地址信息，通常是一个12字符的BCD码</param>
	public DLT698TcpNet(string station = "AAAAAAAAAAAA")
		: base(station)
	{
	}

	/// <summary>
	/// 通过指定IP地址，端口号，设备站号来初始化一个通信对象信息
	/// </summary>
	/// <param name="ipAddress">IP地址信息</param>
	/// <param name="port">端口号信息</param>
	/// <param name="station">设备站号信息</param>
	public DLT698TcpNet(string ipAddress, int port, string station = "AAAAAAAAAAAA")
		: base(ipAddress, port, station)
	{
	}

	protected override OperateResult InitializationOnConnect()
	{
		OperateResult<byte[]> operateResult = ReadFromCoreServer(CommunicationPipe, DLT698Helper.BuildEntireCommand(129, base.Station, base.CA, CreateLoginApdu(1, 0, 0)).Content, hasResponseData: true, usePackAndUnpack: true);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(CommunicationPipe, DLT698Helper.BuildEntireCommand(129, base.Station, base.CA, CreateConnectApdu()).Content, hasResponseData: true, usePackAndUnpack: true);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return base.InitializationOnConnect();
	}

	protected override async Task<OperateResult> InitializationOnConnectAsync()
	{
		OperateResult<byte[]> read1 = await ReadFromCoreServerAsync(CommunicationPipe, DLT698Helper.BuildEntireCommand(129, base.Station, base.CA, CreateLoginApdu(1, 0, 0)).Content, hasResponseData: true, usePackAndUnpack: true);
		if (!read1.IsSuccess)
		{
			return read1;
		}
		OperateResult<byte[]> read2 = await ReadFromCoreServerAsync(CommunicationPipe, DLT698Helper.BuildEntireCommand(129, base.Station, base.CA, CreateConnectApdu()).Content, hasResponseData: true, usePackAndUnpack: true);
		if (!read2.IsSuccess)
		{
			return read2;
		}
		return await base.InitializationOnConnectAsync();
	}

	public override string ToString()
	{
		return $"DLT698TcpNet[{IpAddress}:{Port}]";
	}

	internal static byte[] CreateLoginApdu(byte services = 1, byte piid = 0, byte type = 0)
	{
		byte[] buffer = new byte[15]
		{
			services, piid, type, 0, 132, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0
		};
		DLT698Helper.SetDltDataTime(buffer, 5, DateTime.Now);
		return buffer;
	}

	internal static byte[] CreateConnectApdu()
	{
		return "02 00 00 10 FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF 04 00 04 00 01 04 00 00 00 00 64 00 00".ToHexBytes();
	}
}
