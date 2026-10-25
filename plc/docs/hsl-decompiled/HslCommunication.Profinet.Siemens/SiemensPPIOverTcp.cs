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
using HslCommunication.Profinet.Siemens.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Siemens;

/// <summary>
/// 西门子的PPI协议，适用于s7-200plc，注意，由于本类库的每次通讯分成2次操作，内部增加了一个同步锁，所以单次通信时间比较久，另外，地址支持携带站号，例如：s=2;M100
/// Siemens' PPI protocol is suitable for s7-200plc. Note that since each communication of this class library is divided into two operations,
/// and a synchronization lock is added inside, the single communication time is relatively long. In addition,
/// the address supports carrying the station number, for example : S=2;M100
/// </summary>
/// <remarks>
/// 适用于西门子200的通信，非常感谢 合肥-加劲 的测试，让本类库圆满完成。注意：M地址范围有限 0-31地址
/// 在本类的实现类里，如果使用了Async的异步方法，没有增加同步锁，多线程调用可能会引发数据错乱的情况。
/// In the  implementation class of this class, if the asynchronous method of Async is used,
/// the synchronization lock is not added, and multi-threaded calls may cause data disorder.
/// </remarks>
public class SiemensPPIOverTcp : DeviceTcpNet, ISiemensPPI, IReadWriteNet
{
	private byte station = 2;

	private object communicationLock;

	/// <summary>
	/// 西门子PLC的站号信息
	/// Siemens PLC station number information
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
	/// 实例化一个西门子的PPI协议对象
	/// Instantiate a Siemens PPI protocol object
	/// </summary>
	public SiemensPPIOverTcp()
	{
		base.WordLength = 2;
		base.ByteTransform = new ReverseBytesTransform();
		communicationLock = new object();
	}

	/// <summary>
	/// 使用指定的ip地址和端口号来实例化对象
	/// Instantiate the object with the specified IP address and port number
	/// </summary>
	/// <param name="ipAddress">Ip地址信息</param>
	/// <param name="port">端口号信息</param>
	public SiemensPPIOverTcp(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new SiemensPPIMessage();
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 从西门子的PLC中读取数据信息，地址为"M100","AI100","I0","Q0","V100","S100"等
	/// Read data information from Siemens PLC with addresses "M100", "AI100", "I0", "Q0", "V100", "S100", etc.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="address">西门子的地址数据信息</param>
	/// <param name="length">数据长度</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="communicationLock">当前的同通信锁</param>
	/// <returns>带返回结果的结果对象</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return SiemensPPIHelper.Read(this, address, length, Station, communicationLock);
	}

	[HslMqttApi("ReadBool", "")]
	/// <summary>
	/// 从西门子的PLC中读取bool数据信息，地址为"M100.0","AI100.1","I0.3","Q0.6","V100.4","S100"等
	/// Read bool data information from Siemens PLC, the addresses are "M100.0", "AI100.1", "I0.3", "Q0.6", "V100.4", "S100", etc.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="address">西门子的地址数据信息</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="communicationLock">当前的同通信锁</param>
	/// <returns>带返回结果的结果对象</returns>
	public override OperateResult<bool> ReadBool(string address)
	{
		return SiemensPPIHelper.ReadBool(this, address, Station, communicationLock);
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 从西门子的PLC中读取bool数据信息，地址为"M100.0","AI100.1","I0.3","Q0.6","V100.4","S100"等
	/// Read bool data information from Siemens PLC, the addresses are "M100.0", "AI100.1", "I0.3", "Q0.6", "V100.4", "S100", etc.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="address">西门子的地址数据信息</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="communicationLock">当前的同通信锁</param>
	/// <returns>带返回结果的结果对象</returns>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		return SiemensPPIHelper.ReadBool(this, address, length, Station, communicationLock);
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 将字节数据写入到西门子PLC中，地址为"M100.0","AI100.1","I0.3","Q0.6","V100.4","S100"等
	/// Write byte data to Siemens PLC with addresses "M100.0", "AI100.1", "I0.3", "Q0.6", "V100.4", "S100", etc.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="address">西门子的地址数据信息</param>
	/// <param name="value">数据长度</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="communicationLock">当前的同通信锁</param>
	/// <returns>带返回结果的结果对象</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		return SiemensPPIHelper.Write(this, address, value, Station, communicationLock);
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// 将bool数据写入到西门子PLC中，地址为"M100.0","AI100.1","I0.3","Q0.6","V100.4","S100"等
	/// Write the bool data to Siemens PLC with the addresses "M100.0", "AI100.1", "I0.3", "Q0.6", "V100.4", "S100", etc.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="address">西门子的地址数据信息</param>
	/// <param name="value">数据长度</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="communicationLock">当前的同通信锁</param>
	/// <returns>带返回结果的结果对象</returns>
	public override OperateResult Write(string address, bool[] value)
	{
		return SiemensPPIHelper.Write(this, address, value, Station, communicationLock);
	}

	[HslMqttApi("ReadByte", "")]
	/// <summary>
	/// 从西门子的PLC中读取byte数据信息，地址为"M100","AI100","I0","Q0","V100","S100"等，详细请参照API文档
	/// Read byte data information from Siemens PLC. The addresses are "M100", "AI100", "I0", "Q0", "V100", "S100", etc. Please refer to the API documentation for details.
	/// </summary>
	/// <param name="address">西门子的地址数据信息</param>
	/// <returns>带返回结果的结果对象</returns>
	public OperateResult<byte> ReadByte(string address)
	{
		return ByteTransformHelper.GetResultFromArray(Read(address, 1));
	}

	[HslMqttApi("WriteByte", "")]
	/// <summary>
	/// 向西门子的PLC中写入byte数据，地址为"M100","AI100","I0","Q0","V100","S100"等，详细请参照API文档
	/// Write byte data from Siemens PLC with addresses "M100", "AI100", "I0", "Q0", "V100", "S100", etc. For details, please refer to the API documentation
	/// </summary>
	/// <param name="address">西门子的地址数据信息</param>
	/// <param name="value">数据长度</param>
	/// <returns>带返回结果的结果对象</returns>
	public OperateResult Write(string address, byte value)
	{
		return Write(address, new byte[1] { value });
	}

	public override async Task<OperateResult<bool>> ReadBoolAsync(string address)
	{
		return await Task.Run(() => ReadBool(address));
	}

	/// <summary>
	/// 从西门子的PLC中读取byte数据信息，地址为"M100","AI100","I0","Q0","V100","S100"等，详细请参照API文档
	/// Read byte data information from Siemens PLC. The addresses are "M100", "AI100", "I0", "Q0", "V100", "S100", etc. Please refer to the API documentation for details.
	/// </summary>
	/// <param name="address">西门子的地址数据信息</param>
	/// <returns>带返回结果的结果对象</returns>
	public async Task<OperateResult<byte>> ReadByteAsync(string address)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadAsync(address, 1));
	}

	/// <summary>
	/// 向西门子的PLC中写入byte数据，地址为"M100","AI100","I0","Q0","V100","S100"等，详细请参照API文档
	/// Write byte data from Siemens PLC with addresses "M100", "AI100", "I0", "Q0", "V100", "S100", etc. For details, please refer to the API documentation
	/// </summary>
	/// <param name="address">西门子的地址数据信息</param>
	/// <param name="value">数据长度</param>
	/// <returns>带返回结果的结果对象</returns>
	public async Task<OperateResult> WriteAsync(string address, byte value)
	{
		return await WriteAsync(address, new byte[1] { value });
	}

	[HslMqttApi]
	/// <summary>
	/// 启动西门子PLC为RUN模式，参数信息可以携带站号信息 "s=2;", 注意，分号是必须的。
	/// Start Siemens PLC in RUN mode, parameter information can carry station number information "s=2;", note that the semicolon is required.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="parameter">额外的参数信息，例如可以携带站号信息 &quot;s=2;&quot;, 注意，分号是必须的。</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="communicationLock">当前的同通信锁</param>
	/// <returns>是否启动成功</returns>
	public OperateResult Start(string parameter = "")
	{
		return SiemensPPIHelper.Start(this, parameter, Station, communicationLock);
	}

	[HslMqttApi]
	/// <summary>
	/// 停止西门子PLC，切换为Stop模式，参数信息可以携带站号信息 "s=2;", 注意，分号是必须的。
	/// Stop Siemens PLC and switch to Stop mode, parameter information can carry station number information "s=2;", note that the semicolon is required.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="parameter">额外的参数信息，例如可以携带站号信息 &quot;s=2;&quot;, 注意，分号是必须的。</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="communicationLock">当前的同通信锁</param>
	/// <returns>是否停止成功</returns>
	public OperateResult Stop(string parameter = "")
	{
		return SiemensPPIHelper.Stop(this, parameter, Station, communicationLock);
	}

	[HslMqttApi]
	/// <summary>
	/// 读取西门子PLC的型号信息，参数信息可以携带站号信息 "s=2;", 注意，分号是必须的。
	/// Read the model information of Siemens PLC, the parameter information can carry the station number information "s=2;", note that the semicolon is required.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="parameter">额外的参数信息，例如可以携带站号信息 &quot;s=2;&quot;, 注意，分号是必须的。</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="communicationLock">当前的同通信锁</param>
	/// <returns>包含是否成功的结果对象</returns>
	public OperateResult<string> ReadPlcType(string parameter = "")
	{
		return SiemensPPIHelper.ReadPlcType(this, parameter, Station, communicationLock);
	}

	/// <summary>
	/// 启动西门子PLC为RUN模式，参数信息可以携带站号信息 "s=2;", 注意，分号是必须的。
	/// Start Siemens PLC in RUN mode, parameter information can carry station number information "s=2;", note that the semicolon is required.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="parameter">额外的参数信息，例如可以携带站号信息 &quot;s=2;&quot;, 注意，分号是必须的。</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="communicationLock">当前的同通信锁</param>
	/// <returns>是否启动成功</returns>
	public async Task<OperateResult> StartAsync(string parameter = "")
	{
		return await Task.Run(() => Start(parameter));
	}

	/// <summary>
	/// 停止西门子PLC，切换为Stop模式，参数信息可以携带站号信息 "s=2;", 注意，分号是必须的。
	/// Stop Siemens PLC and switch to Stop mode, parameter information can carry station number information "s=2;", note that the semicolon is required.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="parameter">额外的参数信息，例如可以携带站号信息 &quot;s=2;&quot;, 注意，分号是必须的。</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="communicationLock">当前的同通信锁</param>
	/// <returns>是否停止成功</returns>
	public async Task<OperateResult> StopAsync(string parameter = "")
	{
		return await Task.Run(() => Stop(parameter));
	}

	/// <summary>
	/// 读取西门子PLC的型号信息，参数信息可以携带站号信息 "s=2;", 注意，分号是必须的。
	/// Read the model information of Siemens PLC, the parameter information can carry the station number information "s=2;", note that the semicolon is required.
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="parameter">额外的参数信息，例如可以携带站号信息 &quot;s=2;&quot;, 注意，分号是必须的。</param>
	/// <param name="station">当前的站号信息</param>
	/// <param name="communicationLock">当前的同通信锁</param>
	/// <returns>包含是否成功的结果对象</returns>
	public async Task<OperateResult<string>> ReadPlcTypeAsync(string parameter = "")
	{
		return await Task.Run(() => SiemensPPIHelper.ReadPlcType(this, parameter, Station, communicationLock));
	}

	public override string ToString()
	{
		return $"SiemensPPIOverTcp[{IpAddress}:{Port}]";
	}
}
