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
using HslCommunication.Core.Pipe;
using HslCommunication.Profinet.Melsec.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 三菱的串口通信的对象，适用于读取FX系列的串口数据，支持的类型参考文档说明
/// Mitsubishi's serial communication object is suitable for reading serial data of the FX series. Refer to the documentation for the supported types.
/// </summary>
/// <remarks>
/// 一般老旧的型号，例如FX2N之类的，需要将设置为False，如果是FX3U新的型号，则需要将设置为True
/// </remarks>
public class MelsecFxSerial : DeviceSerialPort, IMelsecFxSerial, IReadWriteNet
{
	/// <summary>
	/// 当前的编程口协议是否为新版，默认为新版，如果无法读取，切换旧版再次尝试
	/// Whether the current programming port protocol is the new version, the default is the new version,
	/// if it cannot be read, switch to the old version and try again
	/// </summary>
	public bool IsNewVersion { get; set; }

	/// <summary>
	/// 获取或设置是否动态修改PLC的波特率，如果为 True，那么如果本对象设置了波特率 115200，就会自动修改PLC的波特率到 115200，因为三菱PLC再重启后都会使用默认的波特率9600
	/// Get or set whether to dynamically modify the baud rate of the PLC. If it is True, then if the baud rate of this object is set to 115200,
	/// the baud rate of the PLC will be automatically modified to 115200, because the Mitsubishi PLC is not After restart, the default baud rate of 9600 will be used
	/// </summary>
	public bool AutoChangeBaudRate { get; set; } = false;

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public MelsecFxSerial()
	{
		base.ByteTransform = new RegularByteTransform();
		base.WordLength = 1;
		IsNewVersion = true;
		base.ByteTransform.IsStringReverseByteWord = true;
		LogMsgFormatBinary = false;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new MelsecFxSerialMessage();
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		int num = MelsecFxSerialHelper.FindSTXIndex(response);
		if (num > 0)
		{
			return OperateResult.CreateSuccessResult(response.RemoveBegin(num));
		}
		return OperateResult.CreateSuccessResult(response);
	}

	public override OperateResult Open()
	{
		if (!(CommunicationPipe is PipeSerialPort pipeSerialPort))
		{
			return new OperateResult("PipeSerialPort get failed");
		}
		int baudRate = pipeSerialPort.GetPipe().BaudRate;
		if (AutoChangeBaudRate && baudRate != 9600)
		{
			pipeSerialPort.GetPipe().BaudRate = 9600;
			OperateResult operateResult = pipeSerialPort.OpenCommunication();
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
			for (int i = 0; i < 3; i++)
			{
				OperateResult<byte[]> operateResult2 = pipeSerialPort.ReadFromCoreServer(GetNewNetMessage(), new byte[1] { 5 }, hasResponseData: true);
				if (!operateResult2.IsSuccess)
				{
					return operateResult2;
				}
				if (operateResult2.Content.Length >= 1 && operateResult2.Content[0] == 6)
				{
					break;
				}
				if (i == 2)
				{
					return new OperateResult("check 0x06 back before send data failed!");
				}
			}
			byte[] sendValue = baudRate switch
			{
				115200 => new byte[6] { 2, 65, 53, 3, 55, 57 }, 
				57600 => new byte[6] { 2, 65, 51, 3, 55, 55 }, 
				38400 => new byte[6] { 2, 65, 50, 3, 55, 54 }, 
				19200 => new byte[6] { 2, 65, 49, 3, 55, 53 }, 
				_ => new byte[6] { 2, 65, 53, 3, 55, 57 }, 
			};
			OperateResult<byte[]> operateResult3 = pipeSerialPort.ReadFromCoreServer(GetNewNetMessage(), sendValue, hasResponseData: true);
			if (!operateResult3.IsSuccess)
			{
				return operateResult3;
			}
			if (operateResult3.Content.Length < 1 || operateResult3.Content[0] != 6)
			{
				return new OperateResult("check 0x06 back after send data failed!");
			}
			pipeSerialPort.CloseCommunication();
			pipeSerialPort.GetPipe().BaudRate = baudRate;
		}
		return base.Open();
	}

	protected override OperateResult InitializationOnConnect()
	{
		if (AutoChangeBaudRate)
		{
			return CommunicationPipe.ReadFromCoreServer(GetNewNetMessage(), new byte[1] { 5 }, hasResponseData: true);
		}
		return base.InitializationOnConnect();
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 根据指定的地址及长度信息从三菱PLC中读取原始的字节数据，根据PLC中实际定义的规则，可以解析出任何类的数据信息
	/// Read the original byte data from the Mitsubishi PLC according to the specified address and length information.
	/// According to the rules actually defined in the PLC, any type of data information can be parsed
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="address">读取地址，，支持的类型参考文档说明</param>
	/// <param name="length">读取的数据长度</param>
	/// <param name="isNewVersion">是否是新版的串口访问类</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		return MelsecFxSerialHelper.Read(this, address, length, IsNewVersion);
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 根据指定的地址向PLC写入数据，数据格式为原始的字节类型
	/// Write data to the PLC according to the specified address, the data format is the original byte type
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="address">初始地址，支持的类型参考文档说明</param>
	/// <param name="value">原始的字节数据</param>
	/// <param name="isNewVersion">是否是新版的串口访问类</param>
	/// <returns>是否写入成功的结果对象</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		return MelsecFxSerialHelper.Write(this, address, value, IsNewVersion);
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 从三菱PLC中批量读取位软元件，返回读取结果，该读取地址最好从0，16，32...等开始读取，这样可以读取比较长的数据数组
	/// Read bit devices in batches from Mitsubishi PLC and return the read results.
	/// The read address should preferably be read from 0, 16, 32... etc., so that a relatively long data array can be read
	/// </summary>
	/// <param name="plc">PLC的通信对象</param>
	/// <param name="address">起始地址</param>
	/// <param name="length">读取的长度</param>
	/// <param name="isNewVersion">是否是新版的串口访问类</param>
	/// <returns>带成功标志的结果数据对象</returns>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		return MelsecFxSerialHelper.ReadBool(this, address, length, IsNewVersion);
	}

	[HslMqttApi("WriteBool", "")]
	/// <summary>
	/// 强制写入位数据的通断，支持的类型参考文档说明
	/// The on-off of the forced write bit data, please refer to the document description for the supported types
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">是否为通</param>
	/// <param name="isNewVersion">是否是新版的串口访问类</param>
	/// <returns>是否写入成功的结果对象</returns>
	public override OperateResult Write(string address, bool value)
	{
		return MelsecFxSerialHelper.Write(this, address, value, IsNewVersion);
	}

	[HslMqttApi("WriteBoolArray", "")]
	public override OperateResult Write(string address, bool[] value)
	{
		return MelsecFxSerialHelper.Write(this, address, value, IsNewVersion);
	}

	[HslMqttApi]
	/// <summary>
	/// 激活PLC的接收状态，需要再和PLC交互之前进行调用，之后就需要再调用了。
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <returns>是否激活成功</returns>
	public OperateResult ActivePlc()
	{
		return MelsecFxSerialHelper.ActivePlc(this);
	}

	/// <summary>
	/// 激活PLC的接收状态，需要再和PLC交互之前进行调用，之后就需要再调用了。
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <returns>是否激活成功</returns>
	public async Task<OperateResult> ActivePlcAsync()
	{
		return await MelsecFxSerialHelper.ActivePlcAsync(this);
	}

	public override async Task<OperateResult> WriteAsync(string address, bool value)
	{
		return await Task.Run(() => Write(address, value));
	}

	public override string ToString()
	{
		return $"MelsecFxSerial[{CommunicationPipe}]";
	}
}
