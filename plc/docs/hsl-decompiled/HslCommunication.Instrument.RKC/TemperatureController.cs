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
using HslCommunication.Instrument.RKC.Helper;

namespace HslCommunication.Instrument.RKC;

/// <summary>
/// RKC的CD/CH系列数字式温度控制器的串口类对象，可以读取测量值，CT1输入值，CT2输入值等等，地址的地址需要参考API文档的示例
/// The serial port object of RKC's CD/CH series digital temperature controller can read the measured value, CT1 input value,
/// CT2 input value, etc. The address of the address needs to refer to the example of the API document
/// </summary>
/// <remarks>
/// 只能使用ReadDouble(string),Write(string,double)方法来读写数据，设备的串口默认参数为 8-1-N,8 个数据位，一个停止位，无奇偶校验
/// 地址支持站号信息，例如 s=2;M1
/// </remarks>
public class TemperatureController : DeviceSerialPort
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
	public TemperatureController()
	{
		base.ByteTransform = new RegularByteTransform();
		base.WordLength = 1;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new RkcTemperatureMessage();
	}

	/// <summary>
	/// 从RKC设备读取Double类型的数据信息，地址示例：M1,M2,M3,AA,AB,B1,ER等，更详细的地址及具体含义需要参考API文档
	/// Read Double type data information from RKC device. Examples of addresses: M1, M2, M3, AA, AB, B1, ER, etc.
	/// For more detailed addresses and specific meanings, please refer to the API documentation
	/// </summary>
	/// <param name="device">设备通信对象</param>
	/// <param name="station">表号信息，也叫站号信息</param>
	/// <param name="address">数据地址信息，地址示例：M1,M2,M3,AA,AB,B1,ER等</param>
	/// <returns>结果数据对象信息</returns>
	public override OperateResult<double[]> ReadDouble(string address, ushort length)
	{
		OperateResult<double> operateResult = TemperatureControllerHelper.ReadDouble(this, station, address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<double[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(new double[1] { operateResult.Content });
	}

	/// <summary>
	/// 将Double类型的数据写入到RKC设备中去，地址示例：M1,M2,M3,AA,AB,B1,ER等，更详细的地址及具体含义需要参考API文档
	/// Write Double type data to the RKC device. Examples of addresses: M1, M2, M3, AA, AB, B1, ER, etc.
	/// For more detailed addresses and specific meanings, please refer to the API documentation
	/// </summary>
	/// <param name="device">设备通信对象</param>
	/// <param name="station">表号信息，也叫站号信息</param>
	/// <param name="address">数据的地址信息，地址示例：M1,M2,M3,AA,AB,B1,ER等</param>
	/// <param name="value">等待写入的值</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, double[] values)
	{
		if (values == null || values.Length == 0)
		{
			return OperateResult.CreateSuccessResult();
		}
		return TemperatureControllerHelper.Write(this, station, address, values[0]);
	}

	/// <summary>
	/// 从RKC设备读取Double类型的数据信息，地址示例：M1,M2,M3,AA,AB,B1,ER等，更详细的地址及具体含义需要参考API文档
	/// Read Double type data information from RKC device. Examples of addresses: M1, M2, M3, AA, AB, B1, ER, etc.
	/// For more detailed addresses and specific meanings, please refer to the API documentation
	/// </summary>
	/// <param name="device">设备通信对象</param>
	/// <param name="station">表号信息，也叫站号信息</param>
	/// <param name="address">数据地址信息，地址示例：M1,M2,M3,AA,AB,B1,ER等</param>
	/// <returns>结果数据对象信息</returns>
	public override async Task<OperateResult<double[]>> ReadDoubleAsync(string address, ushort length)
	{
		return await Task.Run(() => ReadDouble(address, length));
	}

	/// <summary>
	/// 将Double类型的数据写入到RKC设备中去，地址示例：M1,M2,M3,AA,AB,B1,ER等，更详细的地址及具体含义需要参考API文档
	/// Write Double type data to the RKC device. Examples of addresses: M1, M2, M3, AA, AB, B1, ER, etc.
	/// For more detailed addresses and specific meanings, please refer to the API documentation
	/// </summary>
	/// <param name="device">设备通信对象</param>
	/// <param name="station">表号信息，也叫站号信息</param>
	/// <param name="address">数据的地址信息，地址示例：M1,M2,M3,AA,AB,B1,ER等</param>
	/// <param name="value">等待写入的值</param>
	/// <returns>是否写入成功</returns>
	public override async Task<OperateResult> WriteAsync(string address, double[] values)
	{
		return await Task.Run(() => Write(address, values));
	}

	public override string ToString()
	{
		return $"RkcTemperatureController[{base.PortName}:{base.BaudRate}]";
	}
}
