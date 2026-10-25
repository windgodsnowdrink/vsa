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
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Fuji;

/// <summary>
/// 富士PLC的SPB协议，详细的地址信息见api文档说明，地址可以携带站号信息，例如：s=2;D100，PLC侧需要配置无BCC计算，包含0D0A结束码
/// Fuji PLC's SPB protocol. For detailed address information, see the api documentation,
/// The address can carry station number information, for example: s=2;D100, PLC side needs to be configured with no BCC calculation, including 0D0A end code
/// </summary>
public class FujiSPB : DeviceSerialPort
{
	private byte station = 1;

	/// <summary>
	/// PLC的站号信息
	/// PLC station number information
	/// </summary>
	public byte Station
	{
		get
		{
			return station;
		}
		set
		{
			station = value;
		}
	}

	/// <summary>
	/// 使用默认的构造方法实例化对象
	/// Instantiate the object using the default constructor
	/// </summary>
	public FujiSPB()
	{
		base.ByteTransform = new RegularByteTransform();
		base.WordLength = 1;
		LogMsgFormatBinary = false;
		base.ReceiveEmptyDataCount = 5;
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return FujiSPBHelper.CheckResponseData(response);
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new FujiSPBMessage();
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 批量读取PLC的数据，以字为单位，支持读取X,Y,L,M,D,TN,CN,TC,CC,R,W具体的地址范围需要根据PLC型号来确认，地址可以携带站号信息，例如：s=2;D100
	/// Read PLC data in batches, in units of words. Supports reading X, Y, L, M, D, TN, CN, TC, CC, R, W.
	/// The specific address range needs to be confirmed according to the PLC model, The address can carry station number information, for example: s=2;D100
	/// </summary>
	/// <param name="device">PLC设备通信对象</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="address">地址信息</param>
	/// <param name="length">数据长度</param>
	/// <returns>读取结果信息</returns>
	/// <remarks>
	/// 单次读取的最大的字数为105，如果读取的字数超过这个值，请分批次读取。
	/// </remarks>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return FujiSPBHelper.Read(this, station, address, length);
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 批量写入PLC的数据，以字为单位，也就是说最少2个字节信息，支持读取X,Y,L,M,D,TN,CN,TC,CC,R具体的地址范围需要根据PLC型号来确认，地址可以携带站号信息，例如：s=2;D100
	/// The data written to the PLC in batches, in units of words, that is, a minimum of 2 bytes of information. It supports reading X, Y, L, M, D, TN, CN, TC, CC, and R.
	/// The specific address range needs to be based on PLC model to confirm, The address can carry station number information, for example: s=2;D100
	/// </summary>
	/// <param name="device">PLC设备通信对象</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="address">地址信息，举例，D100，R200，TN100，CN200</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// 单次写入的最大的字数为103个字，如果写入的数据超过这个长度，请分批次写入
	/// </remarks>
	public override OperateResult Write(string address, byte[] value)
	{
		return FujiSPBHelper.Write(this, station, address, value);
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 批量读取PLC的Bool数据，以位为单位，支持读取X,Y,L,M,D,TN,CN,TC,CC,R,W，例如 M100, 如果是寄存器地址，可以使用D10.12来访问第10个字的12位，地址可以携带站号信息，例如：s=2;M100
	/// Read PLC's Bool data in batches, in units of bits, support reading X, Y, L, M, D, TN, CN, TC, CC, R, W, such as M100, if it is a register address,
	/// you can use D10. 12 to access the 12 bits of the 10th word, the address can carry station number information, for example: s=2;M100
	/// </summary>
	/// <param name="device">PLC设备通信对象</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="address">地址信息，举例：M100, D10.12</param>
	/// <param name="length">读取的bool长度信息</param>
	/// <returns>Bool[]的结果对象</returns>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		return FujiSPBHelper.ReadBool(this, station, address, length);
	}

	[HslMqttApi("WriteBool", "")]
	/// <summary>
	/// 写入一个Bool值到一个地址里，地址可以是线圈地址，也可以是寄存器地址，例如：M100, D10.12，地址可以携带站号信息，例如：s=2;D10.12
	/// Write a Bool value to an address. The address can be a coil address or a register address, for example: M100, D10.12.
	/// The address can carry station number information, for example: s=2;D10.12
	/// </summary>
	/// <param name="device">PLC设备通信对象</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="address">地址信息，举例：M100, D10.12</param>
	/// <param name="value">写入的bool值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public override OperateResult Write(string address, bool value)
	{
		return FujiSPBHelper.Write(this, station, address, value);
	}

	/// <summary>
	/// 写入一个Bool值到一个地址里，地址可以是线圈地址，也可以是寄存器地址，例如：M100, D10.12，地址可以携带站号信息，例如：s=2;D10.12
	/// Write a Bool value to an address. The address can be a coil address or a register address, for example: M100, D10.12.
	/// The address can carry station number information, for example: s=2;D10.12
	/// </summary>
	/// <param name="device">PLC设备通信对象</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="address">地址信息，举例：M100, D10.12</param>
	/// <param name="value">写入的bool值</param>
	/// <returns>是否写入成功的结果对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, bool value)
	{
		return await FujiSPBHelper.WriteAsync(this, station, address, value);
	}

	public override string ToString()
	{
		return $"FujiSPB[{base.PortName}:{base.BaudRate}]";
	}
}
