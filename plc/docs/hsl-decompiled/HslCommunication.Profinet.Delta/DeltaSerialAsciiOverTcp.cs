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
using HslCommunication.Profinet.Delta.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Delta;

/// <summary>
/// 台达PLC的串口通讯类，基于Modbus-Ascii协议开发，使用串口转网口的透传实现，按照台达的地址进行实现。
/// The serial communication class of Delta PLC is developed based on the Modbus-Ascii protocol and implemented according to Delta's address.
/// </summary>
/// <remarks>
/// 适用于DVP-ES/EX/EC/SS型号，DVP-SA/SC/SX/EH型号以及AS300型号，地址参考API文档，同时地址可以携带站号信息，举例：[s=2;D100],[s=3;M100]，可以动态修改当前报文的站号信息。
/// Suitable for DVP-ES/EX/EC/SS models, DVP-SA/SC/SX/EH models and AS300 model, the address refers to the API document, and the address can carry station number information,
/// for example: [s=2;D100],[s= 3;M100], you can dynamically modify the station number information of the current message.
/// </remarks>
public class DeltaSerialAsciiOverTcp : ModbusAsciiOverTcp, IDelta, IReadWriteDevice, IReadWriteNet
{
	/// <summary>
	/// 获取或设置当前的台达PLC的系列信息，默认为 DVP 系列
	/// Get or set the current series information of Delta PLC, the default is DVP series
	/// </summary>
	public DeltaSeries Series { get; set; } = DeltaSeries.Dvp;

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public DeltaSerialAsciiOverTcp()
	{
		base.ByteTransform.DataFormat = DataFormat.CDAB;
	}

	/// <summary>
	/// 指定服务器地址，端口号，客户端自己的站号来初始化
	/// Specify the server address, port number, and client's own station number to initialize
	/// </summary>
	/// <param name="ipAddress">服务器的Ip地址</param>
	/// <param name="port">服务器的端口号</param>
	/// <param name="station">客户端自身的站号</param>
	public DeltaSerialAsciiOverTcp(string ipAddress, int port = 502, byte station = 1)
		: base(ipAddress, port, station)
	{
		base.ByteTransform.DataFormat = DataFormat.CDAB;
	}

	public override OperateResult<string> TranslateToModbusAddress(string address, byte modbusCode)
	{
		return DeltaHelper.TranslateToModbusAddress(this, address, modbusCode);
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <remarks>
	/// 地址支持X,Y,M,SM,S,T,C,HC，其中X和Y地址使用DD.DD格式，范围 X0.0~X63.15, Y0.0~Y63.15，其中X地址使用的是02功能码，其余的都是01功能码。
	/// Address supports X, Y, M, SM, S, T, C, HC, where X and Y addresses use the DD.DD format, and the range is X0.0~X63.15, Y0.0~Y63.15,
	/// where X addresses are used The one is 02 function code, and the rest are 01 function code.
	/// </remarks>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		return DeltaHelper.ReadBool(this, base.ReadBool, address, length);
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <remarks>
	/// 地址支持Y,M,SM,S,T,C,HC，其中Y地址使用DD.DD格式，Y0.0~Y63.15，不支持X地址的写入。
	/// The address supports Y, M, SM, S, T, C, HC, where the Y address uses the DD.DD format, Y0.0~Y63.15, the writing of the X address is not supported.
	/// </remarks>
	public override OperateResult Write(string address, bool[] values)
	{
		return DeltaHelper.Write(this, base.Write, address, values);
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组
	/// Batch read byte array information, need to specify the address and length, return the original byte array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return DeltaHelper.Read(this, base.Read, address, length);
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 写入原始的byte数组数据到指定的地址，返回是否写入成功
	/// Write the original byte array data to the specified address, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		return DeltaHelper.Write(this, base.Write, address, value);
	}

	/// <remarks>
	/// 地址支持X,Y,M,SM,S,T,C,HC，其中X和Y地址使用DD.DD格式，范围 X0.0~X63.15, Y0.0~Y63.15，其中X地址使用的是02功能码，其余的都是01功能码。
	/// Address supports X, Y, M, SM, S, T, C, HC, where X and Y addresses use the DD.DD format, and the range is X0.0~X63.15, Y0.0~Y63.15,
	/// where X addresses are used The one is 02 function code, and the rest are 01 function code.
	/// </remarks>
	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		return await DeltaHelper.ReadBoolAsync(this, [DebuggerHidden] (string address2, ushort length2) => base.ReadBoolAsync(address2, length2), address, length);
	}

	/// <remarks>
	/// 地址支持Y,M,SM,S,T,C,HC，其中Y地址使用DD.DD格式，Y0.0~Y63.15，不支持X地址的写入。
	/// The address supports Y, M, SM, S, T, C, HC, where the Y address uses the DD.DD format, Y0.0~Y63.15, the writing of the X address is not supported.
	/// </remarks>
	public override async Task<OperateResult> WriteAsync(string address, bool[] values)
	{
		return await DeltaHelper.WriteAsync(this, [DebuggerHidden] (string address2, bool[] values2) => base.WriteAsync(address2, values2), address, values);
	}

	/// <summary>
	/// 批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组
	/// Batch read byte array information, need to specify the address and length, return the original byte array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		return await DeltaHelper.ReadAsync(this, [DebuggerHidden] (string address2, ushort length2) => base.ReadAsync(address2, length2), address, length);
	}

	/// <summary>
	/// 写入原始的byte数组数据到指定的地址，返回是否写入成功
	/// Write the original byte array data to the specified address, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await DeltaHelper.WriteAsync(this, [DebuggerHidden] (string address2, byte[] value2) => base.WriteAsync(address2, value2), address, value);
	}

	public override string ToString()
	{
		return $"DeltaSerialAsciiOverTcp[{IpAddress}:{Port}]";
	}
}
