// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Yokogawa;

/// <summary>
/// 横河PLC的二进制通信类，支持X,Y,I,E,M,T,C,L继电器类型的数据读写，支持D,B,F,R,V,Z,W,TN,CN寄存器类型的数据读写，还支持一些高级的信息读写接口，详细参考Demo界面。
/// Yokogawa PLC's binary communication type, supports X, Y, I, E, M, T, C, L relay type data read and write,
/// supports D, B, F, R, V, Z, W, TN, CN registers Types of data reading and writing, and some advanced information reading and writing interfaces are also supported.
/// Please refer to the Demo UI.
/// </summary>
/// <remarks>
/// 基础的数据读写面向VIP用户开放，高级的读写随机数据，启动停止命令，读取程序状态，
/// 系统信息，PLC时间，读写特殊的模块数据需要商业用户授权，读取的数据长度，读取的随机地址长度，在商业授权下，长度不受限制，可以无限大。
/// </remarks>
public class YokogawaLinkTcp : DeviceTcpNet
{
	/// <summary>
	/// 获取或设置当前的CPU Number，默认值为1
	/// Get or set the current CPU Number, the default value is 1
	/// </summary>
	public byte CpuNumber { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public YokogawaLinkTcp()
	{
		base.ByteTransform = new RegularByteTransform(DataFormat.CDAB);
		CpuNumber = 1;
	}

	/// <summary>
	/// 指定IP地址和端口号来实例化一个对象
	/// Specify the IP address and port number to instantiate an object
	/// </summary>
	/// <param name="ipAddress">Ip地址</param>
	/// <param name="port">端口号</param>
	public YokogawaLinkTcp(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return CheckContent(response);
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new YokogawaLinkBinaryMessage();
	}

	[HslMqttApi("ReadByteArray", "Supports X,Y,I,E,M,T,C,L,D,B,F,R,V,Z,W,TN,CN, for example: D100; or cpu=2;D100 or Special:unit=0;slot=1;100")]
	/// <remarks>
	/// 读取的线圈地址支持X,Y,I,E,M,T,C,L，寄存器地址支持D,B,F,R,V,Z,W,TN,CN，举例：D100；也可以携带CPU进行访问，举例：cpu=2;D100
	/// [商业授权] 如果想要读取特殊模块的数据，需要使用 Special: 开头标记，举例：Special:unit=0;slot=1;100
	/// The read coil address supports X, Y, I, E, M, T, C, L, and the register address supports D, B, F, R, V, Z, W, TN, CN, for example: D100;
	/// it can also be carried CPU access, for example: cpu=2;D100.
	/// [Authorization] If you want to read the data of a special module, you need to use the Special: beginning tag, for example: Special:unit=0;slot=1;100
	/// </remarks>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<List<byte[]>> operateResult = BuildReadCommand(CpuNumber, address, length, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return ReadFromCoreServer(operateResult.Content);
	}

	[HslMqttApi("WriteByteArray", "Supports Y,I,E,M,T,C,L,D,B,F,R,V,Z,W,TN,CN, for example: D100; or cpu=2;D100 or Special:unit=0;slot=1;100")]
	/// <remarks>
	/// 写入的线圈地址支持Y,I,E,M,T,C,L，寄存器地址支持D,B,F,R,V,Z,W,TN,CN，举例：D100；也可以携带CPU进行访问，举例：cpu=2;D100
	/// 如果想要写入特殊模块的数据，需要使用 Special: 开头标记，举例：Special:unit=0;slot=1;100
	/// The read coil address supports Y, I, E, M, T, C, L, and the register address supports D, B, F, R, V, Z, W, TN, CN, for example: D100;
	/// it can also be carried CPU access, for example: cpu=2;D100.
	/// If you want to read the data of a special module, you need to use the Special: beginning tag, for example: Special:unit=0;slot=1;100
	/// </remarks>
	public override OperateResult Write(string address, byte[] value)
	{
		OperateResult<byte[]> operateResult = BuildWriteWordCommand(CpuNumber, address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return ReadFromCoreServer(operateResult.Content);
	}

	[HslMqttApi("ReadBoolArray", "Read coil address supports X, Y, I, E, M, T, C, L, for example: Y100; or cpu=2;Y100")]
	/// <remarks>
	/// 读取的线圈地址支持X,Y,I,E,M,T,C,L，举例：Y100；也可以携带CPU进行访问，举例：cpu=2;Y100
	/// The read coil address supports X, Y, I, E, M, T, C, L, for example: Y100; you can also carry the CPU for access, for example: cpu=2;Y100
	/// </remarks>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		if (address.Contains("."))
		{
			return HslHelper.ReadBool(this, address, length, 16, reverseByWord: true);
		}
		OperateResult<List<byte[]>> operateResult = BuildReadCommand(CpuNumber, address, length, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(operateResult2.Content.Select((byte m) => m != 0).ToArray());
	}

	[HslMqttApi("WriteBoolArray", "The write coil address supports Y, I, E, M, T, C, L, for example: Y100; or cpu=2;Y100")]
	/// <remarks>
	/// 写入的线圈地址支持Y,I,E,M,T,C,L，举例：Y100；也可以携带CPU进行访问，举例：cpu=2;Y100
	/// The write coil address supports Y, I, E, M, T, C, L, for example: Y100; you can also carry the CPU for access, for example: cpu=2;Y100
	/// </remarks>
	public override OperateResult Write(string address, bool[] value)
	{
		OperateResult<byte[]> operateResult = BuildWriteBoolCommand(CpuNumber, address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return ReadFromCoreServer(operateResult.Content);
	}

	[HslMqttApi(Description = "Read random relay, supports X, Y, I, E, M, T, C, L, for example: Y100;")]
	/// <summary>
	/// [商业授权] 随机读取数组信息，主需要出传入数组地址信息，就可以返回批量值
	/// [Authorization] Random read  array information, the master needs to pass in the  array address information, and then the batch can be returned to  value
	/// </summary>
	/// <param name="address">批量地址信息</param>
	/// <returns>带有成功标志的Bool数组信息</returns>
	/// <remarks>
	/// 读取的线圈地址支持X,Y,I,E,M,T,C,L，举例：Y100；也可以携带CPU进行访问，举例：cpu=2;Y100
	/// The read coil address supports X, Y, I, E, M, T, C, L, for example: Y100; you can also carry the CPU for access, for example: cpu=2;Y100
	/// </remarks>
	public OperateResult<bool[]> ReadRandomBool(string[] address)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<bool[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<List<byte[]>> operateResult = BuildReadRandomCommand(CpuNumber, address, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(operateResult2.Content.Select((byte m) => m != 0).ToArray());
	}

	[HslMqttApi(Description = "Write random relay, supports Y, I, E, M, T, C, L, for example: Y100;")]
	/// <summary>
	/// [商业授权] 随机写入数组信息，主需要出传入数组地址信息，以及对应的数组值
	/// [Authorization] Randomly write the  array information, the main need to pass in the  array address information,
	/// and the corresponding  array value
	/// </summary>
	/// <param name="address">批量地址信息</param>
	/// <param name="value">批量的数据值信息</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// 写入的线圈地址支持Y,I,E,M,T,C,L，举例：Y100；也可以携带CPU进行访问，举例：cpu=2;Y100
	/// The write coil address supports Y, I, E, M, T, C, L, for example: Y100; you can also carry the CPU for access, for example: cpu=2;Y100
	/// </remarks>
	public OperateResult WriteRandomBool(string[] address, bool[] value)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<bool[]>(StringResources.Language.InsufficientPrivileges);
		}
		if (address.Length != value.Length)
		{
			return new OperateResult(StringResources.Language.TwoParametersLengthIsNotSame);
		}
		OperateResult<byte[]> operateResult = BuildWriteRandomBoolCommand(CpuNumber, address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi(Description = "Read random register, supports D,B,F,R,V,Z,W,TN,CN，example: D100")]
	/// <summary>
	/// [商业授权] 随机读取数组信息，主需要出传入数组地址信息，就可以返回批量值
	/// [Authorization] Random read  array information, the master needs to pass in the  array address information, and then the batch can be returned to  value
	/// </summary>
	/// <param name="address">批量地址信息</param>
	/// <returns>带有成功标志的Bool数组信息</returns>
	/// <remarks>
	/// Supports D,B,F,R,V,Z,W,TN,CN，example: D100
	/// </remarks>
	public OperateResult<byte[]> ReadRandom(string[] address)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<List<byte[]>> operateResult = BuildReadRandomCommand(CpuNumber, address, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return ReadFromCoreServer(operateResult.Content);
	}

	[HslMqttApi(Description = "Read random register, and get short array, supports D, B, F, R, V, Z, W, TN, CN，example: D100")]
	/// <summary>
	/// [商业授权] 随机读取数组信息，主需要出传入数组地址信息，就可以返回批量值
	/// [Authorization] Random read  array information, the master needs to pass in the  array address information, and then the batch can be returned to  value
	/// </summary>
	/// <param name="address">批量地址信息</param>
	/// <returns>带有成功标志的Bool数组信息</returns>
	public OperateResult<short[]> ReadRandomInt16(string[] address)
	{
		return ReadRandom(address).Then((byte[] m) => OperateResult.CreateSuccessResult(base.ByteTransform.TransInt16(m, 0, address.Length)));
	}

	[HslMqttApi(Description = "Read random register, and get ushort array, supports D, B, F, R, V, Z, W, TN, CN，example: D100")]
	/// <summary>
	/// [商业授权] 随机读取数组信息，主需要出传入数组地址信息，就可以返回批量值
	/// [Authorization] Random read  array information, the master needs to pass in the  array address information, and then the batch can be returned to  value
	/// </summary>
	/// <param name="address">批量地址信息</param>
	/// <returns>带有成功标志的Bool数组信息</returns>
	/// <remarks>
	/// Supports D, B, F, R, V, Z, W, TN, CN，example: D100
	/// </remarks>
	public OperateResult<ushort[]> ReadRandomUInt16(string[] address)
	{
		return ReadRandom(address).Then((byte[] m) => OperateResult.CreateSuccessResult(base.ByteTransform.TransUInt16(m, 0, address.Length)));
	}

	[HslMqttApi(ApiTopic = "WriteRandom", Description = "Randomly write the byte array information, the main need to pass in the byte array address information")]
	/// <summary>
	/// [商业授权] 随机写入数组信息，主需要出传入数组地址信息，以及对应的数组值
	/// [Authorization] Randomly write the  array information, the main need to pass in the  array address information,
	/// and the corresponding  array value
	/// </summary>
	/// <param name="address">批量地址信息</param>
	/// <param name="value">批量的数据值信息</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// Supports D, B, F, R, V, Z, W, TN, CN，example: D100
	/// </remarks>
	public OperateResult WriteRandom(string[] address, byte[] value)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<bool[]>(StringResources.Language.InsufficientPrivileges);
		}
		if (address.Length * 2 != value.Length)
		{
			return new OperateResult(StringResources.Language.TwoParametersLengthIsNotSame);
		}
		OperateResult<byte[]> operateResult = BuildWriteRandomWordCommand(CpuNumber, address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi(ApiTopic = "WriteRandomInt16", Description = "Randomly write the short array information, the main need to pass in the short array address information")]
	/// <summary>
	/// [商业授权] 随机写入数组信息，主需要出传入数组地址信息，以及对应的数组值
	/// [Authorization] Randomly write the  array information, the main need to pass in the  array address information,
	/// and the corresponding  array value
	/// </summary>
	/// <param name="address">批量地址信息</param>
	/// <param name="value">批量的数据值信息</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// Supports D, B, F, R, V, Z, W, TN, CN，example: D100
	/// </remarks>
	public OperateResult WriteRandom(string[] address, short[] value)
	{
		return WriteRandom(address, base.ByteTransform.TransByte(value));
	}

	[HslMqttApi(ApiTopic = "WriteRandomUInt16", Description = "Randomly write the ushort array information, the main need to pass in the ushort array address information")]
	/// <summary>
	/// [商业授权] 随机写入数组信息，主需要出传入数组地址信息，以及对应的数组值
	/// [Authorization] Randomly write the  array information, the main need to pass in the  array address information,
	/// and the corresponding  array value
	/// </summary>
	/// <param name="address">批量地址信息</param>
	/// <param name="value">批量的数据值信息</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// Supports D, B, F, R, V, Z, W, TN, CN，example: D100
	/// </remarks>
	public OperateResult WriteRandom(string[] address, ushort[] value)
	{
		return WriteRandom(address, base.ByteTransform.TransByte(value));
	}

	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		OperateResult<List<byte[]>> command = BuildReadCommand(CpuNumber, address, length, isBit: false);
		if (!command.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(command);
		}
		return await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		OperateResult<byte[]> command = BuildWriteWordCommand(CpuNumber, address, value);
		if (!command.IsSuccess)
		{
			return command;
		}
		return await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		if (address.Contains("."))
		{
			return await HslHelper.ReadBoolAsync(this, address, length, 16, reverseByWord: true).ConfigureAwait(continueOnCapturedContext: false);
		}
		OperateResult<List<byte[]>> command = BuildReadCommand(CpuNumber, address, length, isBit: true);
		if (!command.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(command);
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(read);
		}
		return OperateResult.CreateSuccessResult(read.Content.Select((byte m) => m != 0).ToArray());
	}

	public override async Task<OperateResult> WriteAsync(string address, bool[] value)
	{
		OperateResult<byte[]> command = BuildWriteBoolCommand(CpuNumber, address, value);
		if (!command.IsSuccess)
		{
			return command;
		}
		return await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// [商业授权] 随机读取数组信息，主需要出传入数组地址信息，就可以返回批量值
	/// [Authorization] Random read  array information, the master needs to pass in the  array address information, and then the batch can be returned to  value
	/// </summary>
	/// <param name="address">批量地址信息</param>
	/// <returns>带有成功标志的Bool数组信息</returns>
	/// <remarks>
	/// 读取的线圈地址支持X,Y,I,E,M,T,C,L，举例：Y100；也可以携带CPU进行访问，举例：cpu=2;Y100
	/// The read coil address supports X, Y, I, E, M, T, C, L, for example: Y100; you can also carry the CPU for access, for example: cpu=2;Y100
	/// </remarks>
	public async Task<OperateResult<bool[]>> ReadRandomBoolAsync(string[] address)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<bool[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<List<byte[]>> command = BuildReadRandomCommand(CpuNumber, address, isBit: true);
		if (!command.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(command);
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(read);
		}
		return OperateResult.CreateSuccessResult(read.Content.Select((byte m) => m != 0).ToArray());
	}

	/// <summary>
	/// [商业授权] 随机写入数组信息，主需要出传入数组地址信息，以及对应的数组值
	/// [Authorization] Randomly write the  array information, the main need to pass in the  array address information,
	/// and the corresponding  array value
	/// </summary>
	/// <param name="address">批量地址信息</param>
	/// <param name="value">批量的数据值信息</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// 写入的线圈地址支持Y,I,E,M,T,C,L，举例：Y100；也可以携带CPU进行访问，举例：cpu=2;Y100
	/// The write coil address supports Y, I, E, M, T, C, L, for example: Y100; you can also carry the CPU for access, for example: cpu=2;Y100
	/// </remarks>
	public async Task<OperateResult> WriteRandomBoolAsync(string[] address, bool[] value)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<bool[]>(StringResources.Language.InsufficientPrivileges);
		}
		if (address.Length != value.Length)
		{
			return new OperateResult(StringResources.Language.TwoParametersLengthIsNotSame);
		}
		OperateResult<byte[]> command = BuildWriteRandomBoolCommand(CpuNumber, address, value);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// [商业授权] 随机读取数组信息，主需要出传入数组地址信息，就可以返回批量值
	/// [Authorization] Random read  array information, the master needs to pass in the  array address information, and then the batch can be returned to  value
	/// </summary>
	/// <param name="address">批量地址信息</param>
	/// <returns>带有成功标志的Bool数组信息</returns>
	/// <remarks>
	/// Supports D,B,F,R,V,Z,W,TN,CN，example: D100
	/// </remarks>
	public async Task<OperateResult<byte[]>> ReadRandomAsync(string[] address)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<List<byte[]>> command = BuildReadRandomCommand(CpuNumber, address, isBit: false);
		if (!command.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(command);
		}
		return await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// [商业授权] 随机读取数组信息，主需要出传入数组地址信息，就可以返回批量值
	/// [Authorization] Random read  array information, the master needs to pass in the  array address information, and then the batch can be returned to  value
	/// </summary>
	/// <param name="address">批量地址信息</param>
	/// <returns>带有成功标志的Bool数组信息</returns>
	public async Task<OperateResult<short[]>> ReadRandomInt16Async(string[] address)
	{
		return (await ReadRandomAsync(address)).Then((byte[] m) => OperateResult.CreateSuccessResult(base.ByteTransform.TransInt16(m, 0, address.Length)));
	}

	/// <summary>
	/// [商业授权] 随机读取数组信息，主需要出传入数组地址信息，就可以返回批量值
	/// [Authorization] Random read  array information, the master needs to pass in the  array address information, and then the batch can be returned to  value
	/// </summary>
	/// <param name="address">批量地址信息</param>
	/// <returns>带有成功标志的Bool数组信息</returns>
	/// <remarks>
	/// Supports D, B, F, R, V, Z, W, TN, CN，example: D100
	/// </remarks>
	public async Task<OperateResult<ushort[]>> ReadRandomUInt16Async(string[] address)
	{
		return (await ReadRandomAsync(address)).Then((byte[] m) => OperateResult.CreateSuccessResult(base.ByteTransform.TransUInt16(m, 0, address.Length)));
	}

	/// <summary>
	/// [商业授权] 随机写入数组信息，主需要出传入数组地址信息，以及对应的数组值
	/// [Authorization] Randomly write the  array information, the main need to pass in the  array address information,
	/// and the corresponding  array value
	/// </summary>
	/// <param name="address">批量地址信息</param>
	/// <param name="value">批量的数据值信息</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// Supports D, B, F, R, V, Z, W, TN, CN，example: D100
	/// </remarks>
	public async Task<OperateResult> WriteRandomAsync(string[] address, byte[] value)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<bool[]>(StringResources.Language.InsufficientPrivileges);
		}
		if (address.Length * 2 != value.Length)
		{
			return new OperateResult(StringResources.Language.TwoParametersLengthIsNotSame);
		}
		OperateResult<byte[]> command = BuildWriteRandomWordCommand(CpuNumber, address, value);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// [商业授权] 随机写入数组信息，主需要出传入数组地址信息，以及对应的数组值
	/// [Authorization] Randomly write the  array information, the main need to pass in the  array address information,
	/// and the corresponding  array value
	/// </summary>
	/// <param name="address">批量地址信息</param>
	/// <param name="value">批量的数据值信息</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// Supports D, B, F, R, V, Z, W, TN, CN，example: D100
	/// </remarks>
	public async Task<OperateResult> WriteRandomAsync(string[] address, short[] value)
	{
		return await WriteRandomAsync(address, base.ByteTransform.TransByte(value)).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// [商业授权] 随机写入数组信息，主需要出传入数组地址信息，以及对应的数组值
	/// [Authorization] Randomly write the  array information, the main need to pass in the  array address information,
	/// and the corresponding  array value
	/// </summary>
	/// <param name="address">批量地址信息</param>
	/// <param name="value">批量的数据值信息</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// Supports D, B, F, R, V, Z, W, TN, CN，example: D100
	/// </remarks>
	public async Task<OperateResult> WriteRandomAsync(string[] address, ushort[] value)
	{
		return await WriteRandomAsync(address, base.ByteTransform.TransByte(value)).ConfigureAwait(continueOnCapturedContext: false);
	}

	[HslMqttApi(Description = "Starts executing a program if it is not being executed")]
	/// <summary>
	/// [商业授权] 如果未执行程序，则开始执行程序
	/// [Authorization] Starts executing a program if it is not being executed
	/// </summary>
	/// <returns>是否启动成功</returns>
	/// <remarks>
	/// This command will be ignored if it is executed while a program is being executed.
	/// Refer to the users manual for the individual modules for the response formats that are used at error times.
	/// </remarks>
	public OperateResult Start()
	{
		OperateResult<byte[]> operateResult = BuildStartCommand(CpuNumber);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi(Description = "Stops the executing program.")]
	/// <summary>
	/// [商业授权] 停止当前正在执行程序
	/// [Authorization] Stops the executing program.
	/// </summary>
	/// <returns>是否启动成功</returns>
	/// <remarks>
	/// This command will be ignored if it is executed when no program is being executed.
	/// Refer to the users manual for the individual modules for the response formats that are used at error times.
	/// </remarks>
	public OperateResult Stop()
	{
		OperateResult<byte[]> operateResult = BuildStopCommand(CpuNumber);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi(Description = "Reset the connection which is currently open is forced to close")]
	/// <summary>
	/// [商业授权] 重置当前的模块，当前打开的连接被强制关闭。 模块中所做的设置也将被清除。然后当前对象需要重连PLC。
	/// [Authorization] When this command is executed via an Ethernet interface module or an Ethernet connection of an F3SP66, F3SP67,
	/// F3SP71 or F3SP76 sequence CPU module, the connection which is currently open is forced to close.
	/// The settings made in the modules are also cleared. Then the current object needs to reconnect to the PLC.
	/// </summary>
	/// <returns>是否重置成功</returns>
	public OperateResult ModuleReset()
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> operateResult = ReadFromCoreServer(new byte[4] { 97, CpuNumber, 0, 0 }, hasResponseData: false, usePackAndUnpack: true);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi(Description = "Read the program status. return code 1:RUN; 2:Stop; 3:Debug; 255:ROM writer")]
	/// <summary>
	/// [商业授权] 读取当前PLC的程序状态，返回1：RUN；2：Stop；3：Debug；255：ROM writer
	/// [Authorization] Read the program status. return code 1:RUN; 2:Stop; 3:Debug; 255:ROM writer
	/// </summary>
	/// <returns>当前PLC的程序状态，返回1：RUN；2：Stop；3：Debug；255：ROM writer</returns>
	public OperateResult<int> ReadProgramStatus()
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<int>(StringResources.Language.InsufficientPrivileges);
		}
		byte[] buffer = new byte[6] { 98, 0, 0, 2, 0, 1 };
		buffer[1] = CpuNumber;
		OperateResult<byte[]> operateResult = ReadFromCoreServer(buffer);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<int>(operateResult);
		}
		return OperateResult.CreateSuccessResult((int)operateResult.Content[1]);
	}

	[HslMqttApi(Description = "Read current PLC system status, system ID, CPU type, program size information")]
	/// <summary>
	/// [商业授权] 读取当前PLC的系统状态，系统的ID，CPU类型，程序大小信息
	/// [Authorization] Read current PLC system status, system ID, CPU type, program size information
	/// </summary>
	/// <returns>系统信息的结果对象</returns>
	public OperateResult<YokogawaSystemInfo> ReadSystemInfo()
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<YokogawaSystemInfo>(StringResources.Language.InsufficientPrivileges);
		}
		byte[] buffer = new byte[6] { 98, 0, 0, 2, 0, 2 };
		buffer[1] = CpuNumber;
		return YokogawaSystemInfo.Parse(ReadFromCoreServer(buffer));
	}

	private OperateResult<DateTime> ParseDateTime(OperateResult<byte[]> read)
	{
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<DateTime>(read);
		}
		try
		{
			return OperateResult.CreateSuccessResult(new DateTime(2000 + base.ByteTransform.TransUInt16(read.Content, 0), base.ByteTransform.TransUInt16(read.Content, 2), base.ByteTransform.TransUInt16(read.Content, 4), base.ByteTransform.TransUInt16(read.Content, 6), base.ByteTransform.TransUInt16(read.Content, 8), base.ByteTransform.TransUInt16(read.Content, 10)));
		}
		catch (Exception ex)
		{
			return new OperateResult<DateTime>("Parse DateTime failed: " + ex.Message);
		}
	}

	[HslMqttApi(Description = "Read current PLC time information, including year, month, day, hour, minute, and second")]
	/// <summary>
	/// [商业授权] 读取当前PLC的时间信息，包含年月日时分秒
	/// [Authorization] Read current PLC time information, including year, month, day, hour, minute, and second
	/// </summary>
	/// <returns>PLC的当前的时间信息</returns>
	public OperateResult<DateTime> ReadDateTime()
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<DateTime>(StringResources.Language.InsufficientPrivileges);
		}
		return ParseDateTime(ReadFromCoreServer(new byte[4] { 99, CpuNumber, 0, 0 }));
	}

	[HslMqttApi(Description = "Read the data information of a special module, you need to specify the module unit number, module slot number, data address, and length information.")]
	/// <summary>
	/// [商业授权] 读取特殊模块的数据信息，需要指定模块单元号，模块站号，数据地址，长度信息。
	/// [Authorization] To read the data information of a special module, you need to specify the module unit number, module slot number, data address, and length information.
	/// </summary>
	/// <param name="moduleUnit">模块的单元号</param>
	/// <param name="moduleSlot">模块的站号</param>
	/// <param name="dataPosition">模块的数据地址</param>
	/// <param name="length">长度信息</param>
	/// <returns>带有成功标识的byte[]，可以自行解析出所需要的各种类型的数据</returns>
	public OperateResult<byte[]> ReadSpecialModule(byte moduleUnit, byte moduleSlot, ushort dataPosition, ushort length)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		List<byte> list = new List<byte>();
		List<byte[]> list2 = BuildReadSpecialModule(CpuNumber, moduleUnit, moduleSlot, dataPosition, length);
		for (int i = 0; i < list2.Count; i++)
		{
			OperateResult<byte[]> operateResult = ReadFromCoreServer(list2[i]);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult);
			}
			list.AddRange(operateResult.Content);
		}
		return OperateResult.CreateSuccessResult(list.ToArray());
	}

	/// <summary>
	/// [商业授权] 如果未执行程序，则开始执行程序
	/// [Authorization] Starts executing a program if it is not being executed
	/// </summary>
	/// <returns>是否启动成功</returns>
	/// <remarks>
	/// This command will be ignored if it is executed while a program is being executed.
	/// Refer to the users manual for the individual modules for the response formats that are used at error times.
	/// </remarks>
	public async Task<OperateResult> StartAsync()
	{
		OperateResult<byte[]> command = BuildStartCommand(CpuNumber);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// [商业授权] 停止当前正在执行程序
	/// [Authorization] Stops the executing program.
	/// </summary>
	/// <returns>是否启动成功</returns>
	/// <remarks>
	/// This command will be ignored if it is executed when no program is being executed.
	/// Refer to the users manual for the individual modules for the response formats that are used at error times.
	/// </remarks>
	public async Task<OperateResult> StopAsync()
	{
		OperateResult<byte[]> command = BuildStopCommand(CpuNumber);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(command.Content).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// [商业授权] 重置当前的模块，当前打开的连接被强制关闭。 模块中所做的设置也将被清除。然后当前对象需要重连PLC。
	/// [Authorization] When this command is executed via an Ethernet interface module or an Ethernet connection of an F3SP66, F3SP67,
	/// F3SP71 or F3SP76 sequence CPU module, the connection which is currently open is forced to close.
	/// The settings made in the modules are also cleared. Then the current object needs to reconnect to the PLC.
	/// </summary>
	/// <returns>是否重置成功</returns>
	public async Task<OperateResult> ModuleResetAsync()
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult(StringResources.Language.InsufficientPrivileges);
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(new byte[4] { 97, CpuNumber, 0, 0 }, hasResponseData: false, usePackAndUnpack: true).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// [商业授权] 读取当前PLC的程序状态，返回1：RUN；2：Stop；3：Debug；255：ROM writer
	/// [Authorization] Read the program status. return code 1:RUN; 2:Stop; 3:Debug; 255:ROM writer
	/// </summary>
	/// <returns>当前PLC的程序状态，返回1：RUN；2：Stop；3：Debug；255：ROM writer</returns>
	public async Task<OperateResult<int>> ReadProgramStatusAsync()
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<int>(StringResources.Language.InsufficientPrivileges);
		}
		YokogawaLinkTcp yokogawaLinkTcp = this;
		byte[] buffer = new byte[6] { 98, 0, 0, 2, 0, 1 };
		buffer[1] = CpuNumber;
		OperateResult<byte[]> read = await yokogawaLinkTcp.ReadFromCoreServerAsync(buffer).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<int>(read);
		}
		return OperateResult.CreateSuccessResult((int)read.Content[1]);
	}

	/// <summary>
	/// [商业授权] 读取当前PLC的系统状态，系统的ID，CPU类型，程序大小信息
	/// [Authorization] Read current PLC system status, system ID, CPU type, program size information
	/// </summary>
	/// <returns>系统信息的结果对象</returns>
	public async Task<OperateResult<YokogawaSystemInfo>> ReadSystemInfoAsync()
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<YokogawaSystemInfo>(StringResources.Language.InsufficientPrivileges);
		}
		YokogawaLinkTcp yokogawaLinkTcp = this;
		byte[] buffer = new byte[6] { 98, 0, 0, 2, 0, 2 };
		buffer[1] = CpuNumber;
		return YokogawaSystemInfo.Parse(await yokogawaLinkTcp.ReadFromCoreServerAsync(buffer).ConfigureAwait(continueOnCapturedContext: false));
	}

	/// <summary>
	/// [商业授权] 读取当前PLC的时间信息，包含年月日时分秒
	/// [Authorization] Read current PLC time information, including year, month, day, hour, minute, and second
	/// </summary>
	/// <returns>PLC的当前的时间信息</returns>
	public async Task<OperateResult<DateTime>> ReadDateTimeAsync()
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<DateTime>(StringResources.Language.InsufficientPrivileges);
		}
		return ParseDateTime(await ReadFromCoreServerAsync(new byte[4] { 99, CpuNumber, 0, 0 }).ConfigureAwait(continueOnCapturedContext: false));
	}

	/// <summary>
	/// [商业授权] 读取特殊模块的数据信息，需要指定模块单元号，模块站号，数据地址，长度信息。
	/// [Authorization] To read the data information of a special module, you need to specify the module unit number, module slot number, data address, and length information.
	/// </summary>
	/// <param name="moduleUnit">模块的单元号</param>
	/// <param name="moduleSlot">模块的站号</param>
	/// <param name="dataPosition">模块的数据地址</param>
	/// <param name="length">长度信息</param>
	/// <returns>带有成功标识的byte[]，可以自行解析出所需要的各种类型的数据</returns>
	public async Task<OperateResult<byte[]>> ReadSpecialModuleAsync(byte moduleUnit, byte moduleSlot, ushort dataPosition, ushort length)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		List<byte> content = new List<byte>();
		List<byte[]> commands = BuildReadSpecialModule(CpuNumber, moduleUnit, moduleSlot, dataPosition, length);
		for (int i = 0; i < commands.Count; i++)
		{
			OperateResult<byte[]> read = await ReadFromCoreServerAsync(commands[i]).ConfigureAwait(continueOnCapturedContext: false);
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(read);
			}
			content.AddRange(read.Content);
		}
		return OperateResult.CreateSuccessResult(content.ToArray());
	}

	/// <summary>
	/// 检查当前的反馈内容，如果没有发生错误，就解析出实际的数据内容。
	/// Check the current feedback content, if there is no error, parse out the actual data content.
	/// </summary>
	/// <param name="content">原始的数据内容</param>
	/// <returns>解析之后的数据内容</returns>
	public static OperateResult<byte[]> CheckContent(byte[] content)
	{
		if (content == null || content.Length < 2)
		{
			return new OperateResult<byte[]>(StringResources.Language.ReceiveDataLengthTooShort + " 2 content: " + content.ToHexString(' '));
		}
		if (content[1] != 0)
		{
			return new OperateResult<byte[]>(YokogawaLinkHelper.GetErrorMsg(content[1]));
		}
		if (content.Length > 4)
		{
			return OperateResult.CreateSuccessResult(content.RemoveBegin(4));
		}
		return OperateResult.CreateSuccessResult(new byte[0]);
	}

	/// <summary>
	/// 构建读取命令的原始报文信息
	/// </summary>
	/// <param name="cpu">Cpu Number</param>
	/// <param name="address">地址数据信息</param>
	/// <param name="length">数据长度信息</param>
	/// <param name="isBit">是否位访问</param>
	/// <returns>实际的读取的报文信息</returns>
	public static OperateResult<List<byte[]>> BuildReadCommand(byte cpu, string address, ushort length, bool isBit)
	{
		cpu = (byte)HslHelper.ExtractParameter(ref address, "cpu", cpu);
		if (address.StartsWith("Special:", StringComparison.OrdinalIgnoreCase))
		{
			if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
			{
				return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
			}
			return BuildReadSpecialModule(cpu, address, length);
		}
		OperateResult<YokogawaLinkAddress> operateResult = YokogawaLinkAddress.ParseFrom(address, length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<List<byte[]>>(operateResult);
		}
		OperateResult<int[], int[]> operateResult2 = ((!Authorization.asdniasnfaksndiqwhawfskhfaiw()) ? HslHelper.SplitReadLength(operateResult.Content.AddressStart, length, 65535) : ((!isBit) ? HslHelper.SplitReadLength(operateResult.Content.AddressStart, length, 64) : HslHelper.SplitReadLength(operateResult.Content.AddressStart, length, 256)));
		List<byte[]> list = new List<byte[]>();
		for (int i = 0; i < operateResult2.Content1.Length; i++)
		{
			operateResult.Content.AddressStart = operateResult2.Content1[i];
			byte[] buffer = new byte[12]
			{
				(byte)(isBit ? 1 : 17),
				cpu,
				0,
				8,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			};
			operateResult.Content.GetAddressBinaryContent().CopyTo(buffer, 4);
			buffer[10] = BitConverter.GetBytes(operateResult2.Content2[i])[1];
			buffer[11] = BitConverter.GetBytes(operateResult2.Content2[i])[0];
			list.Add(buffer);
		}
		return OperateResult.CreateSuccessResult(list);
	}

	/// <summary>
	/// 构建随机读取的原始报文的初始命令
	/// </summary>
	/// <param name="cpu">Cpu Number</param>
	/// <param name="address">实际的数据地址信息</param>
	/// <param name="isBit">是否是位读取</param>
	/// <returns>实际的读取的报文信息</returns>
	public static OperateResult<List<byte[]>> BuildReadRandomCommand(byte cpu, string[] address, bool isBit)
	{
		List<string[]> list = SoftBasic.ArraySplitByLength(address, 32);
		List<byte[]> list2 = new List<byte[]>();
		foreach (string[] item in list)
		{
			byte[] buffer = new byte[6 + 6 * item.Length];
			buffer[0] = (byte)(isBit ? 4 : 20);
			buffer[1] = cpu;
			buffer[2] = BitConverter.GetBytes(buffer.Length - 4)[1];
			buffer[3] = BitConverter.GetBytes(buffer.Length - 4)[0];
			buffer[4] = BitConverter.GetBytes(item.Length)[1];
			buffer[5] = BitConverter.GetBytes(item.Length)[0];
			for (int i = 0; i < item.Length; i++)
			{
				buffer[1] = (byte)HslHelper.ExtractParameter(ref item[i], "cpu", cpu);
				OperateResult<YokogawaLinkAddress> operateResult = YokogawaLinkAddress.ParseFrom(item[i], 1);
				if (!operateResult.IsSuccess)
				{
					return OperateResult.CreateFailedResult<List<byte[]>>(operateResult);
				}
				operateResult.Content.GetAddressBinaryContent().CopyTo(buffer, 6 * i + 6);
			}
			list2.Add(buffer);
		}
		return OperateResult.CreateSuccessResult(list2);
	}

	/// <summary>
	/// 构建批量写入Bool数组的命令，需要指定CPU Number信息和设备地址信息
	/// </summary>
	/// <param name="cpu">Cpu Number</param>
	/// <param name="address">设备地址数据</param>
	/// <param name="value">实际的bool数组</param>
	/// <returns>构建的写入指令</returns>
	public static OperateResult<byte[]> BuildWriteBoolCommand(byte cpu, string address, bool[] value)
	{
		cpu = (byte)HslHelper.ExtractParameter(ref address, "cpu", cpu);
		OperateResult<YokogawaLinkAddress> operateResult = YokogawaLinkAddress.ParseFrom(address, 0);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		byte[] buffer = new byte[12 + value.Length];
		buffer[0] = 2;
		buffer[1] = cpu;
		buffer[2] = 0;
		buffer[3] = (byte)(8 + value.Length);
		operateResult.Content.GetAddressBinaryContent().CopyTo(buffer, 4);
		buffer[10] = BitConverter.GetBytes(value.Length)[1];
		buffer[11] = BitConverter.GetBytes(value.Length)[0];
		for (int i = 0; i < value.Length; i++)
		{
			buffer[12 + i] = (byte)(value[i] ? 1 : 0);
		}
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 构建批量随机写入Bool数组的命令，需要指定CPU Number信息和设备地址信息
	/// </summary>
	/// <param name="cpu">Cpu Number</param>
	/// <param name="address">设备地址数据</param>
	/// <param name="value">实际的bool数组</param>
	/// <returns>构建的写入指令</returns>
	public static OperateResult<byte[]> BuildWriteRandomBoolCommand(byte cpu, string[] address, bool[] value)
	{
		if (address.Length != value.Length)
		{
			return new OperateResult<byte[]>(StringResources.Language.TwoParametersLengthIsNotSame);
		}
		byte[] buffer = new byte[6 + address.Length * 8 - 1];
		buffer[0] = 5;
		buffer[1] = cpu;
		buffer[2] = BitConverter.GetBytes(buffer.Length - 4)[1];
		buffer[3] = BitConverter.GetBytes(buffer.Length - 4)[0];
		buffer[4] = BitConverter.GetBytes(address.Length)[1];
		buffer[5] = BitConverter.GetBytes(address.Length)[0];
		for (int i = 0; i < address.Length; i++)
		{
			buffer[1] = (byte)HslHelper.ExtractParameter(ref address[i], "cpu", cpu);
			OperateResult<YokogawaLinkAddress> operateResult = YokogawaLinkAddress.ParseFrom(address[i], 0);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult);
			}
			operateResult.Content.GetAddressBinaryContent().CopyTo(buffer, 6 + 8 * i);
			buffer[12 + 8 * i] = (byte)(value[i] ? 1 : 0);
		}
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 构建字写入的命令报文信息，需要指定设备地址
	/// </summary>
	/// <param name="cpu">Cpu Number</param>
	/// <param name="address">地址</param>
	/// <param name="value">原始的数据值</param>
	/// <returns>原始的报文命令</returns>
	public static OperateResult<byte[]> BuildWriteWordCommand(byte cpu, string address, byte[] value)
	{
		cpu = (byte)HslHelper.ExtractParameter(ref address, "cpu", cpu);
		if (address.StartsWith("Special:", StringComparison.OrdinalIgnoreCase))
		{
			if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
			{
				return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
			}
			return BuildWriteSpecialModule(cpu, address, value);
		}
		OperateResult<YokogawaLinkAddress> operateResult = YokogawaLinkAddress.ParseFrom(address, 0);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		byte[] buffer = new byte[12 + value.Length];
		buffer[0] = 18;
		buffer[1] = cpu;
		buffer[2] = 0;
		buffer[3] = (byte)(8 + value.Length);
		operateResult.Content.GetAddressBinaryContent().CopyTo(buffer, 4);
		buffer[10] = BitConverter.GetBytes(value.Length / 2)[1];
		buffer[11] = BitConverter.GetBytes(value.Length / 2)[0];
		value.CopyTo(buffer, 12);
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 构建随机写入字的命令的报文
	/// </summary>
	/// <param name="cpu">Cpu Number</param>
	/// <param name="address">地址</param>
	/// <param name="value">原始的数据值</param>
	/// <returns>原始的报文命令</returns>
	public static OperateResult<byte[]> BuildWriteRandomWordCommand(byte cpu, string[] address, byte[] value)
	{
		if (address.Length * 2 != value.Length)
		{
			return new OperateResult<byte[]>(StringResources.Language.TwoParametersLengthIsNotSame);
		}
		byte[] buffer = new byte[6 + address.Length * 8];
		buffer[0] = 21;
		buffer[1] = cpu;
		buffer[2] = BitConverter.GetBytes(buffer.Length - 4)[1];
		buffer[3] = BitConverter.GetBytes(buffer.Length - 4)[0];
		buffer[4] = BitConverter.GetBytes(address.Length)[1];
		buffer[5] = BitConverter.GetBytes(address.Length)[0];
		for (int i = 0; i < address.Length; i++)
		{
			buffer[1] = (byte)HslHelper.ExtractParameter(ref address[i], "cpu", cpu);
			OperateResult<YokogawaLinkAddress> operateResult = YokogawaLinkAddress.ParseFrom(address[i], 0);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult);
			}
			operateResult.Content.GetAddressBinaryContent().CopyTo(buffer, 6 + 8 * i);
			buffer[12 + 8 * i] = value[i * 2];
			buffer[13 + 8 * i] = value[i * 2 + 1];
		}
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 构建启动PLC的命令报文
	/// </summary>
	/// <param name="cpu">Cpu Number</param>
	/// <returns>原始的报文命令</returns>
	public static OperateResult<byte[]> BuildStartCommand(byte cpu)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		return OperateResult.CreateSuccessResult(new byte[4] { 69, cpu, 0, 0 });
	}

	/// <summary>
	/// 构建停止PLC的命令报文
	/// </summary>
	/// <param name="cpu">Cpu Number</param>
	/// <returns>原始的报文命令</returns>
	public static OperateResult<byte[]> BuildStopCommand(byte cpu)
	{
		if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
		}
		return OperateResult.CreateSuccessResult(new byte[4] { 70, cpu, 0, 0 });
	}

	/// <summary>
	/// 构建读取特殊模块的命令报文
	/// </summary>
	/// <param name="cpu">Cpu Number</param>
	/// <param name="moduleUnit">模块单元号</param>
	/// <param name="moduleSlot">模块站号</param>
	/// <param name="dataPosition">数据位置</param>
	/// <param name="length">长度信息</param>
	/// <returns>原始的报文命令</returns>
	public static List<byte[]> BuildReadSpecialModule(byte cpu, byte moduleUnit, byte moduleSlot, ushort dataPosition, ushort length)
	{
		List<byte[]> list = new List<byte[]>();
		OperateResult<int[], int[]> operateResult = HslHelper.SplitReadLength(dataPosition, length, 64);
		for (int i = 0; i < operateResult.Content1.Length; i++)
		{
			byte[] buffer = new byte[10];
			buffer[0] = 49;
			buffer[1] = cpu;
			buffer[2] = BitConverter.GetBytes(buffer.Length - 4)[1];
			buffer[3] = BitConverter.GetBytes(buffer.Length - 4)[0];
			buffer[4] = moduleUnit;
			buffer[5] = moduleSlot;
			buffer[6] = BitConverter.GetBytes(operateResult.Content1[i])[1];
			buffer[7] = BitConverter.GetBytes(operateResult.Content1[i])[0];
			buffer[8] = BitConverter.GetBytes(operateResult.Content2[i])[1];
			buffer[9] = BitConverter.GetBytes(operateResult.Content2[i])[0];
			list.Add(buffer);
		}
		return list;
	}

	/// <summary>
	/// 构建读取特殊模块的命令报文，需要传入高级地址，必须以 Special: 开头表示特殊模块地址，示例：Special:cpu=1;unit=0;slot=1;100
	/// To construct a command message to read a special module, the advanced address needs to be passed in.
	/// It must start with Special: to indicate the address of the special module, for example: Special:cpu=1;unit=0;slot=1;100
	/// </summary>
	/// <param name="cpu">Cpu Number</param>
	/// <param name="address">高级的混合地址，除了Cpu可以不携带，例如：Special:unit=0;slot=1;100</param>
	/// <param name="length">长度信息</param>
	/// <returns>原始的报文命令</returns>
	public static OperateResult<List<byte[]>> BuildReadSpecialModule(byte cpu, string address, ushort length)
	{
		if (address.StartsWith("Special:", StringComparison.OrdinalIgnoreCase))
		{
			address = address.Substring(8);
			cpu = (byte)HslHelper.ExtractParameter(ref address, "cpu", cpu);
			OperateResult<int> operateResult = HslHelper.ExtractParameter(ref address, "unit");
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<List<byte[]>>(operateResult);
			}
			OperateResult<int> operateResult2 = HslHelper.ExtractParameter(ref address, "slot");
			if (!operateResult2.IsSuccess)
			{
				return OperateResult.CreateFailedResult<List<byte[]>>(operateResult2);
			}
			try
			{
				return OperateResult.CreateSuccessResult(BuildReadSpecialModule(cpu, (byte)operateResult.Content, (byte)operateResult2.Content, ushort.Parse(address), length));
			}
			catch (Exception ex)
			{
				return new OperateResult<List<byte[]>>("Address format wrong: " + ex.Message);
			}
		}
		return new OperateResult<List<byte[]>>("Special module address must start with Special:");
	}

	/// <summary>
	/// 构建读取特殊模块的命令报文
	/// </summary>
	/// <param name="cpu">Cpu Number</param>
	/// <param name="moduleUnit">模块单元号</param>
	/// <param name="moduleSlot">模块站号</param>
	/// <param name="dataPosition">数据位置</param>
	/// <param name="data">数据内容</param>
	/// <returns>原始的报文命令</returns>
	public static byte[] BuildWriteSpecialModule(byte cpu, byte moduleUnit, byte moduleSlot, ushort dataPosition, byte[] data)
	{
		byte[] buffer = new byte[10 + data.Length];
		buffer[0] = 50;
		buffer[1] = cpu;
		buffer[2] = BitConverter.GetBytes(buffer.Length - 4)[1];
		buffer[3] = BitConverter.GetBytes(buffer.Length - 4)[0];
		buffer[4] = moduleUnit;
		buffer[5] = moduleSlot;
		buffer[6] = BitConverter.GetBytes(dataPosition)[1];
		buffer[7] = BitConverter.GetBytes(dataPosition)[0];
		buffer[8] = BitConverter.GetBytes(data.Length / 2)[1];
		buffer[9] = BitConverter.GetBytes(data.Length / 2)[0];
		data.CopyTo(buffer, 10);
		return buffer;
	}

	/// <summary>
	/// 构建写入特殊模块的命令报文，需要传入高级地址，必须以 Special: 开头表示特殊模块地址，示例：Special:cpu=1;unit=0;slot=1;100
	/// To construct a command message to write a special module, the advanced address needs to be passed in.
	/// It must start with Special: to indicate the address of the special module, for example: Special:cpu=1;unit=0;slot=1;100
	/// </summary>
	/// <param name="cpu">Cpu Number</param>
	/// <param name="address">高级的混合地址，除了Cpu可以不携带，例如：Special:unit=0;slot=1;100</param>
	/// <param name="data">写入的原始数据内容</param>
	/// <returns>原始的报文命令</returns>
	public static OperateResult<byte[]> BuildWriteSpecialModule(byte cpu, string address, byte[] data)
	{
		OperateResult<List<byte[]>> operateResult = BuildReadSpecialModule(cpu, address, 0);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		byte[] buffer = new byte[10 + data.Length];
		buffer[0] = 50;
		buffer[1] = operateResult.Content[0][1];
		buffer[2] = BitConverter.GetBytes(buffer.Length - 4)[1];
		buffer[3] = BitConverter.GetBytes(buffer.Length - 4)[0];
		buffer[4] = operateResult.Content[0][4];
		buffer[5] = operateResult.Content[0][5];
		buffer[6] = operateResult.Content[0][6];
		buffer[7] = operateResult.Content[0][7];
		buffer[8] = BitConverter.GetBytes(data.Length / 2)[1];
		buffer[9] = BitConverter.GetBytes(data.Length / 2)[0];
		data.CopyTo(buffer, 10);
		return OperateResult.CreateSuccessResult(buffer);
	}

	public override string ToString()
	{
		return $"YokogawaLinkTcp[{IpAddress}:{Port}]";
	}
}
