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
using HslCommunication.Profinet.FATEK.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.FATEK;

/// <summary>
/// 台湾永宏公司的编程口协议，此处是基于tcp的实现，地址信息请查阅api文档信息，地址可以携带站号信息，例如 s=2;D100
/// The programming port protocol of Taiwan Yonghong company, here is the implementation based on TCP,
/// please refer to the API information for the address information, The address can carry station number information, such as s=2;D100
/// </summary>
/// <remarks>
/// 支持位访问：M,X,Y,S,T(触点),C(触点)，字访问：RT(当前值),RC(当前值)，D，R；具体参照API文档
/// </remarks>
public class FatekProgramOverTcp : DeviceTcpNet, IFatekProgram, IReadWriteNet
{
	private byte station = 1;

	/// <summary>
	/// PLC的站号信息，需要和实际的设置值一致，默认为1
	/// The station number information of the PLC needs to be consistent with the actual setting value. The default is 1.
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
	/// 实例化默认的构造方法
	/// Instantiate the default constructor
	/// </summary>
	public FatekProgramOverTcp()
	{
		base.WordLength = 1;
		base.ByteTransform = new RegularByteTransform();
		LogMsgFormatBinary = false;
	}

	/// <summary>
	/// 使用指定的ip地址和端口来实例化一个对象
	/// Instantiate an object with the specified IP address and port
	/// </summary>
	/// <param name="ipAddress">设备的Ip地址</param>
	/// <param name="port">设备的端口号</param>
	public FatekProgramOverTcp(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new SpecifiedCharacterMessage(3);
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return FatekProgramHelper.UnpackResponseContent(send, response);
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 批量读取PLC的字节数据，以字为单位，支持读取X,Y,M,S,D,T,C,R,RT,RC具体的地址范围需要根据PLC型号来确认，地址可以携带站号信息，例如 s=2;D100
	/// Read PLC byte data in batches, in word units. Supports reading X, Y, M, S, D, T, C, R, RT, RC.
	/// The specific address range needs to be confirmed according to the PLC model, The address can carry station number information, such as s=2;D100
	/// </summary>
	/// <param name="device">PLC通信的对象</param>
	/// <param name="station">设备的站点信息</param>
	/// <param name="address">地址信息</param>
	/// <param name="length">数据长度</param>
	/// <returns>读取结果信息</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return FatekProgramHelper.Read(this, station, address, length);
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 批量写入PLC的数据，以字为单位，也就是说最少2个字节信息，支持X,Y,M,S,D,T,C,R,RT,RC具体的地址范围需要根据PLC型号来确认，地址可以携带站号信息，例如 s=2;D100
	/// The data written to the PLC in batches, in units of words, that is, at least 2 bytes of information,
	/// supporting X, Y, M, S, D, T, C, R, RT, and RC. The specific address range needs to be based on the PLC model To confirm, The address can carry station number information, such as s=2;D100
	/// </summary>
	/// <param name="device">PLC通信的对象</param>
	/// <param name="station">设备的站号信息</param>
	/// <param name="address">地址信息，举例，D100，R200，RC100，RT200</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		return FatekProgramHelper.Write(this, station, address, value);
	}

	/// <summary>
	/// 批量读取PLC的字节数据，以字为单位，支持读取X,Y,M,S,D,T,C,R,RT,RC具体的地址范围需要根据PLC型号来确认，地址可以携带站号信息，例如 s=2;D100
	/// Read PLC byte data in batches, in word units. Supports reading X, Y, M, S, D, T, C, R, RT, RC.
	/// The specific address range needs to be confirmed according to the PLC model, The address can carry station number information, such as s=2;D100
	/// </summary>
	/// <param name="device">PLC通信的对象</param>
	/// <param name="station">设备的站点信息</param>
	/// <param name="address">地址信息</param>
	/// <param name="length">数据长度</param>
	/// <returns>读取结果信息</returns>
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		return await FatekProgramHelper.ReadAsync(this, station, address, length);
	}

	/// <summary>
	/// 批量写入PLC的数据，以字为单位，也就是说最少2个字节信息，支持X,Y,M,S,D,T,C,R,RT,RC具体的地址范围需要根据PLC型号来确认，地址可以携带站号信息，例如 s=2;D100
	/// The data written to the PLC in batches, in units of words, that is, at least 2 bytes of information,
	/// supporting X, Y, M, S, D, T, C, R, RT, and RC. The specific address range needs to be based on the PLC model To confirm, The address can carry station number information, such as s=2;D100
	/// </summary>
	/// <param name="device">PLC通信的对象</param>
	/// <param name="station">设备的站号信息</param>
	/// <param name="address">地址信息，举例，D100，R200，RC100，RT200</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await FatekProgramHelper.WriteAsync(this, station, address, value);
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 批量读取bool类型数据，支持的类型为X,Y,M,S,T,C，具体的地址范围取决于PLC的类型，地址可以携带站号信息，例如 s=2;M100
	/// Read bool data in batches. The supported types are X, Y, M, S, T, C. The specific address range depends on the type of PLC,
	/// The address can carry station number information, such as s=2;M100
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">设备的站号信息</param>
	/// <param name="address">地址信息，比如X10，Y17，M100</param>
	/// <param name="length">读取的长度</param>
	/// <returns>读取结果信息</returns>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		if (address.Contains('.'))
		{
			return HslHelper.ReadBool(this, address, length);
		}
		return FatekProgramHelper.ReadBool(this, station, address, length);
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// 批量写入bool类型的数组，支持的类型为X,Y,M,S,T,C，具体的地址范围取决于PLC的类型，地址可以携带站号信息，例如 s=2;M100
	/// Write arrays of type bool in batches. The supported types are X, Y, M, S, T, C. The specific address range depends on the type of PLC,
	/// The address can carry station number information, such as s=2;M100
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">PLC的地址信息</param>
	/// <param name="value">数据信息</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, bool[] value)
	{
		return FatekProgramHelper.Write(this, station, address, value);
	}

	/// <summary>
	/// 批量读取bool类型数据，支持的类型为X,Y,M,S,T,C，具体的地址范围取决于PLC的类型，地址可以携带站号信息，例如 s=2;M100
	/// Read bool data in batches. The supported types are X, Y, M, S, T, C. The specific address range depends on the type of PLC,
	/// The address can carry station number information, such as s=2;M100
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">设备的站号信息</param>
	/// <param name="address">地址信息，比如X10，Y17，M100</param>
	/// <param name="length">读取的长度</param>
	/// <returns>读取结果信息</returns>
	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		if (address.Contains("."))
		{
			return await HslHelper.ReadBoolAsync(this, address, length);
		}
		return await FatekProgramHelper.ReadBoolAsync(this, station, address, length);
	}

	/// <summary>
	/// 批量写入bool类型的数组，支持的类型为X,Y,M,S,T,C，具体的地址范围取决于PLC的类型，地址可以携带站号信息，例如 s=2;M100
	/// Write arrays of type bool in batches. The supported types are X, Y, M, S, T, C. The specific address range depends on the type of PLC,
	/// The address can carry station number information, such as s=2;M100
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">PLC的地址信息</param>
	/// <param name="value">数据信息</param>
	/// <returns>是否写入成功</returns>
	public override async Task<OperateResult> WriteAsync(string address, bool[] value)
	{
		return await FatekProgramHelper.WriteAsync(this, station, address, value);
	}

	/// <summary>
	/// 使PLC处于RUN的状态
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>是否操作成功</returns>
	public OperateResult Run(byte station)
	{
		return FatekProgramHelper.Run(this, station);
	}

	[HslMqttApi("Run", "使PLC处于RUN状态")]
	/// <summary>
	/// 使PLC处于RUN的状态
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>是否操作成功</returns>
	public OperateResult Run()
	{
		return Run(Station);
	}

	/// <summary>
	/// 使PLC处于STOP状态
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>是否操作成功</returns>
	public OperateResult Stop(byte station)
	{
		return FatekProgramHelper.Stop(this, station);
	}

	[HslMqttApi("Stop", "使PLC处于STOP状态")]
	/// <summary>
	/// 使PLC处于STOP状态
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>是否操作成功</returns>
	public OperateResult Stop()
	{
		return Stop(Station);
	}

	/// <summary>
	/// 读取当前PLC的状态信息，返回一个bool数组，同时包含了几种电量信息，分别为 0: RUN/STOP, 1: BAT LOW/正常, 2: Ladder checksum error/正常, 3: 使用ROM PACK/未使用,
	/// 4: WDT Timeout/正常, 5: 设定ID/未设ID， 6： 紧急停机/正常
	/// Read the status information of the current PLC and return a bool array, which also contains several power information, 0: RUN/STOP, 1: BAT LOW/normal,
	/// 2: Ladder checksum error/normal, 3: Use ROM PACK/ Not used, 4: WDT Timeout/Normal, 5: ID set/ID not set, 6: Emergency stop/Normal
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>状态结果信息</returns>
	public OperateResult<bool[]> ReadStatus(byte station)
	{
		return FatekProgramHelper.ReadStatus(this, station);
	}

	[HslMqttApi("ReadStatus", "读取PLC基本的状态信息")]
	/// <summary>
	/// 读取当前PLC的状态信息，返回一个bool数组，同时包含了几种电量信息，分别为 0: RUN/STOP, 1: BAT LOW/正常, 2: Ladder checksum error/正常, 3: 使用ROM PACK/未使用,
	/// 4: WDT Timeout/正常, 5: 设定ID/未设ID， 6： 紧急停机/正常
	/// Read the status information of the current PLC and return a bool array, which also contains several power information, 0: RUN/STOP, 1: BAT LOW/normal,
	/// 2: Ladder checksum error/normal, 3: Use ROM PACK/ Not used, 4: WDT Timeout/Normal, 5: ID set/ID not set, 6: Emergency stop/Normal
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>状态结果信息</returns>
	public OperateResult<bool[]> ReadStatus()
	{
		return ReadStatus(Station);
	}

	/// <summary>
	/// 使PLC处于RUN的状态
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>是否操作成功</returns>
	public async Task<OperateResult> RunAsync(byte station)
	{
		return await FatekProgramHelper.RunAsync(this, station);
	}

	/// <summary>
	/// 使PLC处于RUN的状态
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>是否操作成功</returns>
	public async Task<OperateResult> RunAsync()
	{
		return await RunAsync(Station);
	}

	/// <summary>
	/// 使PLC处于STOP状态
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>是否操作成功</returns>
	public async Task<OperateResult> StopAsync(byte station)
	{
		return await FatekProgramHelper.StopAsync(this, station);
	}

	/// <summary>
	/// 使PLC处于STOP状态
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>是否操作成功</returns>
	public async Task<OperateResult> StopAsync()
	{
		return await StopAsync(Station);
	}

	/// <summary>
	/// 读取当前PLC的状态信息，返回一个bool数组，同时包含了几种电量信息，分别为 0: RUN/STOP, 1: BAT LOW/正常, 2: Ladder checksum error/正常, 3: 使用ROM PACK/未使用,
	/// 4: WDT Timeout/正常, 5: 设定ID/未设ID， 6： 紧急停机/正常
	/// Read the status information of the current PLC and return a bool array, which also contains several power information, 0: RUN/STOP, 1: BAT LOW/normal,
	/// 2: Ladder checksum error/normal, 3: Use ROM PACK/ Not used, 4: WDT Timeout/Normal, 5: ID set/ID not set, 6: Emergency stop/Normal
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>状态结果信息</returns>
	public async Task<OperateResult<bool[]>> ReadStatusAsync(byte station)
	{
		return await FatekProgramHelper.ReadStatusAsync(this, station);
	}

	/// <summary>
	/// 读取当前PLC的状态信息，返回一个bool数组，同时包含了几种电量信息，分别为 0: RUN/STOP, 1: BAT LOW/正常, 2: Ladder checksum error/正常, 3: 使用ROM PACK/未使用,
	/// 4: WDT Timeout/正常, 5: 设定ID/未设ID， 6： 紧急停机/正常
	/// Read the status information of the current PLC and return a bool array, which also contains several power information, 0: RUN/STOP, 1: BAT LOW/normal,
	/// 2: Ladder checksum error/normal, 3: Use ROM PACK/ Not used, 4: WDT Timeout/Normal, 5: ID set/ID not set, 6: Emergency stop/Normal
	/// </summary>
	/// <param name="device">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <returns>状态结果信息</returns>
	public async Task<OperateResult<bool[]>> ReadStatusAsync()
	{
		return await ReadStatusAsync(Station);
	}

	public override string ToString()
	{
		return $"FatekProgramOverTcp[{IpAddress}:{Port}]";
	}
}
