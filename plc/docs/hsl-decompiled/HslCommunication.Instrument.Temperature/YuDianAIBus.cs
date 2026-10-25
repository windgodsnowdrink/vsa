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
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Reflection;

namespace HslCommunication.Instrument.Temperature;

/// <summary>
/// 宇电AIBus协议的温度采集模块，基于串口通讯实现。
/// </summary>
public class YuDianAIBus : DeviceSerialPort
{
	/// <summary>
	/// 获取或者重新修改服务器的默认站号信息，当然，你可以再读写的时候动态指定，例如地址 "s=2;100", 更详细的例子可以参考DEMO界面上的地址示例。
	/// Get or re-modify the server's default station number information, of course, you can dynamically specify when reading and writing,
	/// such as the address "s=2; 100", a more detailed example can refer to the DEMO interface address example.
	/// </summary>
	/// <remarks>
	/// 当你调用 ReadCoil("100") 时，对应的站号就是本属性的值，当你调用 ReadCoil("s=2;100") 时，就忽略本属性的值，读写寄存器的时候同理
	/// </remarks>
	public byte Station { get; set; } = 0;

	/// <summary>
	/// 实例化一个默认的对象
	/// Initializes a new instance of the  class.
	/// </summary>
	/// <remarks>
	/// This constructor sets up the default configuration for the  instance,
	/// including initializing the byte transformation mechanism, setting the word length, and defining  the default sleep
	/// time between operations.
	/// </remarks>
	public YuDianAIBus()
	{
		base.ByteTransform = new RegularByteTransform();
		base.WordLength = 1;
		base.SleepTime = 20;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new YuDianMessage();
	}

	private OperateResult<byte[]> BuildCommandHelper(string address, byte code, byte data1, byte data2)
	{
		byte b = (byte)HslHelper.ExtractParameter(ref address, "s", Station);
		if (!byte.TryParse(address, out var result))
		{
			return new OperateResult<byte[]>($"Address [{result}] Wrong!");
		}
		byte b2 = ((b < 128) ? ((byte)(b + 128)) : b);
		byte[] buffer = new byte[8] { b2, b2, code, result, data1, data2, 0, 0 };
		int num = b;
		for (int i = 2; i < buffer.Length - 2; i += 2)
		{
			num += BitConverter.ToUInt16(buffer, i);
		}
		buffer[6] = (byte)(num % 256);
		buffer[7] = (byte)(num / 256);
		return OperateResult.CreateSuccessResult(buffer);
	}

	private OperateResult<byte[]> BuildReadCommand(string address, ushort length, bool isBit)
	{
		return BuildCommandHelper(address, 82, 0, 0);
	}

	private OperateResult<byte[]> BuildWriteCommand(string address, byte[] value)
	{
		if (value.Length != 2)
		{
			return new OperateResult<byte[]>("Value Length Must Be 2 Bytes");
		}
		return BuildCommandHelper(address, 67, value[0], value[1]);
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
		OperateResult<byte[]> operateResult = BuildReadCommand(address, length, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		if (length == 1)
		{
			return OperateResult.CreateSuccessResult(operateResult2.Content.SelectMiddle(6, 2));
		}
		return OperateResult.CreateSuccessResult(operateResult2.Content.SelectMiddle(0, 8));
	}

	[HslMqttApi("ReadDoubleArray", "")]
	/// <summary>
	/// 读取双浮点数据的数组
	/// Read double floating point data array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的double数组</returns>
	public override OperateResult<double[]> ReadDouble(string address, ushort length)
	{
		OperateResult<short[]> operateResult = ReadInt16(address, length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<double[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(((IEnumerable<short>)operateResult.Content).Select((Func<short, double>)((short m) => m)).ToArray());
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
		OperateResult<byte[]> operateResult = BuildWriteCommand(address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return ReadFromCoreServer(operateResult.Content);
	}

	[HslMqttApi("WriteDoubleArray", "")]
	/// <summary>
	/// 写入double数组，返回是否成功
	/// Write double array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override OperateResult Write(string address, double[] values)
	{
		return Write(address, values.Select((double m) => (short)m).ToArray());
	}

	/// <summary>
	/// 异步读取双浮点数据的数组
	/// Asynchronously read double floating point data array
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">数组长度</param>
	/// <returns>带有成功标识的double数组</returns>
	public override async Task<OperateResult<double[]>> ReadDoubleAsync(string address, ushort length)
	{
		OperateResult<short[]> read = await ReadInt16Async(address, length).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<double[]>(read);
		}
		return OperateResult.CreateSuccessResult(((IEnumerable<short>)read.Content).Select((Func<short, double>)((short m) => m)).ToArray());
	}

	/// <summary>
	/// 异步写入double数组，返回是否成功
	/// Asynchronously write double array, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="values">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, double[] values)
	{
		return await WriteAsync(address, values.Select((double m) => (short)m).ToArray()).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override string ToString()
	{
		return $"YuDianAIBus<{CommunicationPipe}>";
	}
}
