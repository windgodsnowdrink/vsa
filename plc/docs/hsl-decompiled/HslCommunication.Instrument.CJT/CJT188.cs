// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Instrument.CJT.Helper;
using HslCommunication.Instrument.DLT.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Instrument.CJT;

/// <summary>
/// 城市建设部的188协议，基于DJ/T188-2004实现的协议
/// </summary>
public class CJT188 : DeviceSerialPort, ICjt188, IReadWriteDevice, IReadWriteNet
{
	private string station = "1";

	/// <summary>
	/// 获取或设置仪表的类型，通常是 0x10:冷水水表  0x11:生活热水水表  0x12:直饮水水表  0x13:中水水表  0x20:热量表(热量)  0x21:热量表(冷量)  0x30:燃气表  0x40:电度表
	/// </summary>
	public byte InstrumentType { get; set; }

	/// <summary>
	/// 获取或设置当前的地址域信息，是一个14个字符的BCD码，例如：14910000729011
	/// Get or set the current address domain information, which is a 14-character BCD code, for example: 14910000729011
	/// </summary>
	public string Station
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
	/// 获取或设置是否在每一次的报文通信时，增加"FE FE FE FE"的命令头
	/// Get or set whether to add the command header of "FE FE FE FE" in each message communication
	/// </summary>
	public bool EnableCodeFE { get; set; } = true;

	/// <summary>
	/// 获取或设置是否验证匹配接收到的站号信息
	/// Gets or sets whether to verify that the received station number information is matched
	/// </summary>
	public bool StationMatch { get; set; } = false;

	/// <summary>
	/// 指定地址域来实例化一个对象，地址域是一个14个字符的BCD码，例如：14910000729011
	/// Specify the address field, to instantiate an object, which address field is a 14-character BCD code, for example: 14910000729011
	/// </summary>
	/// <param name="station">设备的地址信息，是一个14字符的BCD码</param>
	public CJT188(string station)
	{
		base.ByteTransform = new RegularByteTransform();
		this.station = station;
		base.ReceiveEmptyDataCount = 5;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new CJT188Message(StationMatch);
	}

	public override OperateResult<byte[]> ReadFromCoreServer(byte[] send)
	{
		OperateResult<byte[]> operateResult = base.ReadFromCoreServer(send);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		int num = DLT645Helper.FindHeadCode68H(operateResult.Content);
		if (num > 0)
		{
			return OperateResult.CreateSuccessResult(operateResult.Content.RemoveBegin(num));
		}
		return operateResult;
	}

	public override byte[] PackCommandWithHeader(byte[] command)
	{
		if (EnableCodeFE)
		{
			return SoftBasic.SpliceArray<byte>(new byte[2] { 254, 254 }, command);
		}
		return base.PackCommandWithHeader(command);
	}

	/// <summary>
	/// 激活设备的命令，只发送数据到设备，不等待设备数据返回
	/// The command to activate the device, only send data to the device, do not wait for the device data to return
	/// </summary>
	/// <returns>是否发送成功</returns>
	public OperateResult ActiveDeveice()
	{
		return ReadFromCoreServer(new byte[2] { 254, 254 }, hasResponseData: false, usePackAndUnpack: false);
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 根据指定的数据标识来读取相关的原始数据信息，地址标识根据手册来，从高位到地位，例如 91-1F，分割符可以任意特殊字符或是没有分隔符。
	/// Read the relevant original data information according to the specified data identifier. The address identifier is based on the manual,
	/// from high to position, such as 91-1F. The separator can be any special character or no separator.
	/// </summary>
	/// <param name="cjt">CJT188的通信对象</param>
	/// <param name="address">数据标识，具体需要查找手册来对应</param>
	/// <param name="length">数据长度信息</param>
	/// <returns>结果信息</returns>
	/// <remarks>
	/// 地址可以携带地址域信息，例如 "s=2;90-1F" 或是 "s=100000;90-1F"，关于数据域信息，需要查找手册，例如:D1-20 表示： 上一月结算日累积流量
	/// </remarks>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return CJT188Helper.Read(this, address, length);
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 根据指定的数据标识来写入相关的原始数据信息，地址标识根据手册来，从高位到地位，例如 90-1F，分割符可以任意特殊字符或是没有分隔符。
	/// Read the relevant original data information according to the specified data identifier. The address identifier is based on the manual,
	/// from high to position, such as 90-1F. The separator can be any special character or no separator.
	/// </summary>
	/// <param name="cjt">CJT188的通信对象</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">写入的数据值</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// 地址可以携带地址域信息，例如 "s=2;90-1F" 或是 "s=100000;90-1F"，关于数据域信息，需要查找手册，例如:00-01-00-00 表示： (当前)正向有功总电能
	/// 注意：本命令必须与编程键配合使用
	/// </remarks>
	public override OperateResult Write(string address, byte[] value)
	{
		return CJT188Helper.Write(this, address, value);
	}

	[HslMqttApi("ReadFloatArray", "")]
	public override OperateResult<float[]> ReadFloat(string address, ushort length)
	{
		return CJT188Helper.ReadValue(this, address, length, float.Parse);
	}

	[HslMqttApi("ReadDoubleArray", "")]
	public override OperateResult<double[]> ReadDouble(string address, ushort length)
	{
		return CJT188Helper.ReadValue(this, address, length, double.Parse);
	}

	public override OperateResult<string> ReadString(string address, ushort length, Encoding encoding)
	{
		return ByteTransformHelper.GetResultFromArray(ReadStringArray(address));
	}

	/// <summary>
	/// 读取指定地址的所有的字符串数据信息，一般来说，一个地址只有一个数据
	/// </summary>
	/// <param name="cjt">CJT通信对象</param>
	/// <param name="address">数据标识，具体需要查找手册来对应</param>
	/// <returns>字符串数组信息</returns>
	/// <remarks>
	/// 地址可以携带地址域信息，例如 "s=2;90-1F" 或是 "s=100000;90-1F"
	/// </remarks>
	public OperateResult<string[]> ReadStringArray(string address)
	{
		return CJT188Helper.ReadStringArray(this, address);
	}

	public override async Task<OperateResult<float[]>> ReadFloatAsync(string address, ushort length)
	{
		return await Task.Run(() => ReadFloat(address, length));
	}

	public override async Task<OperateResult<double[]>> ReadDoubleAsync(string address, ushort length)
	{
		return await Task.Run(() => ReadDouble(address, length));
	}

	public override async Task<OperateResult<string>> ReadStringAsync(string address, ushort length, Encoding encoding)
	{
		return await Task.Run(() => ReadString(address, length, encoding));
	}

	/// <summary>
	/// 读取设备的通信地址，仅支持点对点通讯的情况，返回地址域数据，例如：14910000729012
	/// Read the communication address of the device, only support point-to-point communication, and return the address field data, for example: 149100007290
	/// </summary>
	/// <param name="cjt">CJT188的通信对象</param>
	/// <returns>设备的通信地址</returns>
	public OperateResult<string> ReadAddress()
	{
		return CJT188Helper.ReadAddress(this);
	}

	/// <summary>
	/// 写入设备的地址域信息，仅支持点对点通讯的情况，需要指定地址域信息，例如：14910000729011
	/// Write the address domain information of the device, only support point-to-point communication,
	/// you need to specify the address domain information, for example: 14910000729011
	/// </summary>
	/// <param name="cjt">CJT188的通信对象</param>
	/// <param name="address">等待写入的地址域</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteAddress(string address)
	{
		return CJT188Helper.WriteAddress(this, address);
	}

	public override string ToString()
	{
		return $"CJT188[{base.PortName}:{base.BaudRate}]";
	}
}
