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
using HslCommunication.Profinet.LSIS.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.LSIS;

/// <summary>
/// XGB CPU I/F module supports Serial Port. The address can carry station number information, for example: s=2;D100
/// </summary>
/// <remarks>
/// XGB 主机的通道 0 仅支持 1:1 通信。 对于具有主从格式的 1:N 系统，在连接 XGL-C41A 模块的通道 1 或 XGB 主机中使用 RS-485 通信。 XGL-C41A 模块支持 RS-422/485 协议。
/// </remarks>
public class LSCpu : DeviceSerialPort
{
	/// <summary>
	/// 获取或设置当前设备站号信息，一般来说，需要在实例化之后设置本站号信息，在通信的时候也可以动态修改当前的站号信息
	/// To obtain or set the station number information of the current device, in general, the station number information needs to be set after instantiation,
	/// and the current station number information can also be dynamically modified during communication
	/// </summary>
	public byte Station { get; set; } = 5;

	/// <summary>
	/// Instantiate a Default object
	/// </summary>
	public LSCpu()
	{
		base.ByteTransform = new RegularByteTransform();
		base.WordLength = 2;
		LogMsgFormatBinary = false;
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return LSCpuHelper.UnpackResponseContent(send, response);
	}

	[HslMqttApi("ReadByte", "")]
	/// <summary>
	/// Read single byte value from plc
	/// </summary>
	/// <param name="address">Start address</param>
	/// <returns>result</returns>
	public OperateResult<byte> ReadByte(string address)
	{
		return ByteTransformHelper.GetResultFromArray(Read(address, 1));
	}

	[HslMqttApi("WriteByte", "")]
	/// <summary>
	/// Write single byte value to plc
	/// </summary>
	/// <param name="address">Start address</param>
	/// <param name="value">value</param>
	/// <returns>Whether to write the successful</returns>
	public OperateResult Write(string address, byte value)
	{
		return Write(address, new byte[1] { value });
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 从PLC的指定地址读取原始的位数据信息，地址示例：MB100.0, MW100.0
	/// Read the original bool data information from the designated address of the PLC.
	/// Examples of addresses: MB100.0, MW100.0
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">PLC的地址信息，例如 MB100.0, MW100.0</param>
	/// <param name="length">读取的长度信息</param>
	/// <returns>返回是否读取成功的结果对象</returns>
	/// <remarks>
	/// 地址类型支持 P,M,L,K,F,T,C,D,R,I,Q,W, 支持携带站号的形式，例如 s=2;MB100.0
	/// </remarks>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		return LSCpuHelper.ReadBool(this, Station, address, length);
	}

	/// <summary>
	/// ReadCoil
	/// </summary>
	/// <param name="address">Start address</param>
	/// <returns>Whether to read the successful</returns>
	public OperateResult<bool> ReadCoil(string address)
	{
		return ReadBool(address);
	}

	/// <summary>
	/// ReadCoil
	/// </summary>
	/// <param name="address">Start address</param>
	/// <param name="length">read address length</param>
	/// <returns>Whether to read the successful</returns>
	public OperateResult<bool[]> ReadCoil(string address, ushort length)
	{
		return ReadBool(address, length);
	}

	/// <summary>
	/// WriteCoil
	/// </summary>
	/// <param name="address">Start address</param>
	/// <param name="value">bool value</param>
	/// <returns>Whether to write the successful</returns>
	public OperateResult WriteCoil(string address, bool value)
	{
		return Write(address, value);
	}

	[HslMqttApi("WriteBool", "")]
	/// <summary>
	/// 将bool数据写入到PLC的指定的地址里，地址示例：MX100, MX10A
	/// Write the bool data to the designated address of the PLC. Examples of addresses: MX100, MX10A
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">PLC的地址信息，例如 MX100, MX10A</param>
	/// <param name="value">bool值信息</param>
	/// <returns>返回是否读取成功的结果对象</returns>
	/// <remarks>
	/// 地址类型支持 P,M,L,K,F,T,C,D,R,I,Q,W, 支持携带站号的形式，例如 s=2;MX100
	/// </remarks>
	public override OperateResult Write(string address, bool value)
	{
		return LSCpuHelper.Write(this, Station, address, value);
	}

	/// <summary>
	/// 从PLC的指定地址读取原始的位数据信息，地址示例：MB100.0, MW100.0
	/// Read the original bool data information from the designated address of the PLC.
	/// Examples of addresses: MB100.0, MW100.0
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">PLC的地址信息，例如 MB100.0, MW100.0</param>
	/// <param name="length">读取的长度信息</param>
	/// <returns>返回是否读取成功的结果对象</returns>
	/// <remarks>
	/// 地址类型支持 P,M,L,K,F,T,C,D,R,I,Q,W, 支持携带站号的形式，例如 s=2;MB100.0
	/// </remarks>
	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		return await LSCpuHelper.ReadBoolAsync(this, Station, address, length);
	}

	/// <summary>
	/// ReadCoil
	/// </summary>
	/// <param name="address">Start address</param>
	/// <returns>Whether to read the successful</returns>
	public async Task<OperateResult<bool>> ReadCoilAsync(string address)
	{
		return await ReadBoolAsync(address);
	}

	/// <summary>
	/// ReadCoil
	/// </summary>
	/// <param name="address">Start address</param>
	/// <param name="length">read address length</param>
	/// <returns>Whether to read the successful</returns>
	public async Task<OperateResult<bool[]>> ReadCoilAsync(string address, ushort length)
	{
		return await ReadBoolAsync(address, length);
	}

	/// <summary>
	/// WriteCoil
	/// </summary>
	/// <param name="address">Start address</param>
	/// <param name="value">bool value</param>
	/// <returns>Whether to write the successful</returns>
	public async Task<OperateResult> WriteCoilAsync(string address, bool value)
	{
		return await WriteAsync(address, value);
	}

	/// <summary>
	/// 将bool数据写入到PLC的指定的地址里，地址示例：MX100, MX10A
	/// Write the bool data to the designated address of the PLC. Examples of addresses: MX100, MX10A
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">PLC的地址信息，例如 MX100, MX10A</param>
	/// <param name="value">bool值信息</param>
	/// <returns>返回是否读取成功的结果对象</returns>
	/// <remarks>
	/// 地址类型支持 P,M,L,K,F,T,C,D,R,I,Q,W, 支持携带站号的形式，例如 s=2;MX100
	/// </remarks>
	public override async Task<OperateResult> WriteAsync(string address, bool value)
	{
		return await LSCpuHelper.WriteAsync(this, Station, address, value);
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 从PLC的指定地址读取原始的字节数据信息，地址示例：MB100, MW100, MD100, 如果输入了M100等同于MB100
	/// Read the original byte data information from the designated address of the PLC.
	/// Examples of addresses: MB100, MW100, MD100, if the input M100 is equivalent to MB100
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">PLC的地址信息，例如 M100, MB100, MW100, MD100</param>
	/// <param name="length">读取的长度信息</param>
	/// <returns>返回是否读取成功的结果对象</returns>
	/// <remarks>
	/// 地址类型支持 P,M,L,K,F,T,C,D,R,I,Q,W, 支持携带站号的形式，例如 s=2;MW100
	/// </remarks>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return LSCpuHelper.Read(this, Station, address, length);
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 将原始数据写入到PLC的指定的地址里，地址示例：MB100, MW100, MD100, 如果输入了M100等同于MB100
	/// Write the original data to the designated address of the PLC.
	/// Examples of addresses: MB100, MW100, MD100, if input M100 is equivalent to MB100
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">PLC的地址信息，例如 M100, MB100, MW100, MD100</param>
	/// <param name="value">等待写入的原始数据内容</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		return LSCpuHelper.Write(this, Station, address, value);
	}

	/// <summary>
	/// 从PLC的指定地址读取原始的字节数据信息，地址示例：MB100, MW100, MD100, 如果输入了M100等同于MB100
	/// Read the original byte data information from the designated address of the PLC.
	/// Examples of addresses: MB100, MW100, MD100, if the input M100 is equivalent to MB100
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">PLC的地址信息，例如 M100, MB100, MW100, MD100</param>
	/// <param name="length">读取的长度信息</param>
	/// <returns>返回是否读取成功的结果对象</returns>
	/// <remarks>
	/// 地址类型支持 P,M,L,K,F,T,C,D,R,I,Q,W, 支持携带站号的形式，例如 s=2;MW100
	/// </remarks>
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		return await LSCpuHelper.ReadAsync(this, Station, address, length);
	}

	/// <summary>
	/// 将原始数据写入到PLC的指定的地址里，地址示例：MB100, MW100, MD100, 如果输入了M100等同于MB100
	/// Write the original data to the designated address of the PLC.
	/// Examples of addresses: MB100, MW100, MD100, if input M100 is equivalent to MB100
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">PLC的地址信息，例如 M100, MB100, MW100, MD100</param>
	/// <param name="value">等待写入的原始数据内容</param>
	/// <returns>是否写入成功</returns>
	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await LSCpuHelper.WriteAsync(this, Station, address, value);
	}

	public override string ToString()
	{
		return $"LSCpu[{base.PortName}:{base.BaudRate}]";
	}
}
