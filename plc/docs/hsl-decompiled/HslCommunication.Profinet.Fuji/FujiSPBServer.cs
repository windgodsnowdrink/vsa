// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Text;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.ModBus;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Fuji;

/// <summary>
/// 富士的SPB虚拟的PLC，线圈支持X,Y,M的读写，其中X只能远程读，寄存器支持D,R,W的读写操作。
/// Fuji's SPB virtual PLC, the coil supports X, Y, M read and write,
/// X can only be read remotely, and the register supports D, R, W read and write operations.
/// </summary>
public class FujiSPBServer : DeviceServer
{
	private SoftBuffer xBuffer;

	private SoftBuffer yBuffer;

	private SoftBuffer mBuffer;

	private SoftBuffer dBuffer;

	private SoftBuffer rBuffer;

	private SoftBuffer wBuffer;

	private const int DataPoolLength = 65536;

	private int station = 1;

	/// <summary>
	/// 获取或设置数据解析的格式，可选ABCD, BADC，CDAB，DCBA格式，对int,uint,float,double,long,ulong类型有作用
	/// Get or set the format of the data analysis, optional ABCD, BADC, CDAB, DCBA format, effective for int, uint, float, double, long, ulong type
	/// </summary>
	public DataFormat DataFormat
	{
		get
		{
			return base.ByteTransform.DataFormat;
		}
		set
		{
			base.ByteTransform.DataFormat = value;
		}
	}

	/// <summary>
	/// 字符串数据是否按照字来反转，默认为False
	/// Whether the string data is reversed according to words. The default is False.
	/// </summary>
	/// <remarks>
	/// 字符串按照2个字节的排列进行颠倒，根据实际情况进行设置
	/// </remarks>
	public bool IsStringReverse
	{
		get
		{
			return base.ByteTransform.IsStringReverseByteWord;
		}
		set
		{
			base.ByteTransform.IsStringReverseByteWord = value;
		}
	}

	/// <summary>
	/// PLC的站号信息
	/// PLC station number information
	/// </summary>
	public int Station
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
	/// 实例化一个富士SPB的网口和串口服务器，支持数据读写操作
	/// </summary>
	public FujiSPBServer()
	{
		xBuffer = new SoftBuffer(65536);
		yBuffer = new SoftBuffer(65536);
		mBuffer = new SoftBuffer(65536);
		dBuffer = new SoftBuffer(131072);
		rBuffer = new SoftBuffer(131072);
		wBuffer = new SoftBuffer(131072);
		base.ByteTransform = new RegularByteTransform();
		base.WordLength = 1;
		LogMsgFormatBinary = false;
		base.SerialReceiveAtleastTime = 2000;
	}

	protected override byte[] SaveToBytes()
	{
		byte[] buffer = new byte[589824];
		xBuffer.GetBytes().CopyTo(buffer, 0);
		yBuffer.GetBytes().CopyTo(buffer, 65536);
		mBuffer.GetBytes().CopyTo(buffer, 131072);
		dBuffer.GetBytes().CopyTo(buffer, 196608);
		rBuffer.GetBytes().CopyTo(buffer, 327680);
		wBuffer.GetBytes().CopyTo(buffer, 458752);
		return buffer;
	}

	protected override void LoadFromBytes(byte[] content)
	{
		if (content.Length < 589824)
		{
			throw new Exception("File is not correct");
		}
		xBuffer.SetBytes(content, 0, 65536);
		yBuffer.SetBytes(content, 65536, 65536);
		mBuffer.SetBytes(content, 131072, 65536);
		dBuffer.SetBytes(content, 196608, 131072);
		rBuffer.SetBytes(content, 327680, 131072);
		wBuffer.SetBytes(content, 458752, 131072);
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
		OperateResult<byte[]> operateResult = new OperateResult<byte[]>();
		try
		{
			switch (address[0])
			{
			case 'X':
			case 'x':
				return OperateResult.CreateSuccessResult(xBuffer.GetBytes(Convert.ToInt32(address.Substring(1)) * 2, length * 2));
			case 'Y':
			case 'y':
				return OperateResult.CreateSuccessResult(yBuffer.GetBytes(Convert.ToInt32(address.Substring(1)) * 2, length * 2));
			case 'M':
			case 'm':
				return OperateResult.CreateSuccessResult(mBuffer.GetBytes(Convert.ToInt32(address.Substring(1)) * 2, length * 2));
			case 'D':
			case 'd':
				return OperateResult.CreateSuccessResult(dBuffer.GetBytes(Convert.ToInt32(address.Substring(1)) * 2, length * 2));
			case 'R':
			case 'r':
				return OperateResult.CreateSuccessResult(rBuffer.GetBytes(Convert.ToInt32(address.Substring(1)) * 2, length * 2));
			case 'W':
			case 'w':
				return OperateResult.CreateSuccessResult(wBuffer.GetBytes(Convert.ToInt32(address.Substring(1)) * 2, length * 2));
			default:
				throw new Exception(StringResources.Language.NotSupportedDataType);
			}
		}
		catch (Exception ex)
		{
			operateResult.Message = ex.Message;
			return operateResult;
		}
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
		OperateResult<byte[]> operateResult = new OperateResult<byte[]>();
		try
		{
			switch (address[0])
			{
			case 'X':
			case 'x':
				xBuffer.SetBytes(value, Convert.ToInt32(address.Substring(1)) * 2);
				return OperateResult.CreateSuccessResult();
			case 'Y':
			case 'y':
				yBuffer.SetBytes(value, Convert.ToInt32(address.Substring(1)) * 2);
				return OperateResult.CreateSuccessResult();
			case 'M':
			case 'm':
				mBuffer.SetBytes(value, Convert.ToInt32(address.Substring(1)) * 2);
				return OperateResult.CreateSuccessResult();
			case 'D':
			case 'd':
				dBuffer.SetBytes(value, Convert.ToInt32(address.Substring(1)) * 2);
				return OperateResult.CreateSuccessResult();
			case 'R':
			case 'r':
				rBuffer.SetBytes(value, Convert.ToInt32(address.Substring(1)) * 2);
				return OperateResult.CreateSuccessResult();
			case 'W':
			case 'w':
				wBuffer.SetBytes(value, Convert.ToInt32(address.Substring(1)) * 2);
				return OperateResult.CreateSuccessResult();
			default:
				throw new Exception(StringResources.Language.NotSupportedDataType);
			}
		}
		catch (Exception ex)
		{
			operateResult.Message = ex.Message;
			return operateResult;
		}
	}

	private int GetBitIndex(string address)
	{
		int num = 0;
		if (address.LastIndexOf('.') > 0)
		{
			num = HslHelper.GetBitIndexInformation(ref address);
			return Convert.ToInt32(address.Substring(1)) * 16 + num;
		}
		if (address[0] == 'X' || address[0] == 'x' || address[0] == 'Y' || address[0] == 'y' || address[0] == 'M' || address[0] == 'm')
		{
			return Convert.ToInt32(address.Substring(1));
		}
		return Convert.ToInt32(address.Substring(1)) * 16;
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
		try
		{
			int bitIndex = GetBitIndex(address);
			switch (address[0])
			{
			case 'X':
			case 'x':
				return OperateResult.CreateSuccessResult(xBuffer.GetBool(bitIndex, length));
			case 'Y':
			case 'y':
				return OperateResult.CreateSuccessResult(yBuffer.GetBool(bitIndex, length));
			case 'M':
			case 'm':
				return OperateResult.CreateSuccessResult(mBuffer.GetBool(bitIndex, length));
			case 'D':
			case 'd':
				return OperateResult.CreateSuccessResult(dBuffer.GetBool(bitIndex, length));
			case 'R':
			case 'r':
				return OperateResult.CreateSuccessResult(rBuffer.GetBool(bitIndex, length));
			case 'W':
			case 'w':
				return OperateResult.CreateSuccessResult(wBuffer.GetBool(bitIndex, length));
			default:
				throw new Exception(StringResources.Language.NotSupportedDataType);
			}
		}
		catch (Exception ex)
		{
			return new OperateResult<bool[]>(ex.Message);
		}
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// 批量写入数组数据，返回是否成功
	/// Batch write  array data, return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, bool[] value)
	{
		try
		{
			int bitIndex = GetBitIndex(address);
			switch (address[0])
			{
			case 'X':
			case 'x':
				xBuffer.SetBool(value, bitIndex);
				return OperateResult.CreateSuccessResult();
			case 'Y':
			case 'y':
				yBuffer.SetBool(value, bitIndex);
				return OperateResult.CreateSuccessResult();
			case 'M':
			case 'm':
				mBuffer.SetBool(value, bitIndex);
				return OperateResult.CreateSuccessResult();
			case 'D':
			case 'd':
				dBuffer.SetBool(value, bitIndex);
				return OperateResult.CreateSuccessResult();
			case 'R':
			case 'r':
				rBuffer.SetBool(value, bitIndex);
				return OperateResult.CreateSuccessResult();
			case 'W':
			case 'w':
				wBuffer.SetBool(value, bitIndex);
				return OperateResult.CreateSuccessResult();
			default:
				throw new Exception(StringResources.Language.NotSupportedDataType);
			}
		}
		catch (Exception ex)
		{
			return new OperateResult<bool[]>(ex.Message);
		}
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new FujiSPBMessage();
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		if (receive.Length < 5)
		{
			return new OperateResult<byte[]>("Uknown message: " + receive.ToHexString(' '));
		}
		if (receive[0] != 58)
		{
			return new OperateResult<byte[]>("Message must start with 0x3A: " + receive.ToHexString(' '));
		}
		if (Encoding.ASCII.GetString(receive, 1, 2) != station.ToString("X2"))
		{
			return new OperateResult<byte[]>($"Station not match , Except: {station:X2} , Actual: {Encoding.ASCII.GetString(receive, 1, 2)}");
		}
		return OperateResult.CreateSuccessResult(ReadFromSPBCore(receive));
	}

	private byte[] CreateResponseBack(byte err, string command, byte[] data, bool addLength = true)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(':');
		stringBuilder.Append(Station.ToString("X2"));
		stringBuilder.Append("00");
		stringBuilder.Append(command.Substring(9, 4));
		stringBuilder.Append(err.ToString("X2"));
		if (err == 0 && data != null)
		{
			if (addLength)
			{
				stringBuilder.Append(FujiSPBHelper.AnalysisIntegerAddress(data.Length / 2));
			}
			stringBuilder.Append(data.ToHexString());
		}
		stringBuilder[3] = ((stringBuilder.Length - 5) / 2).ToString("X2")[0];
		stringBuilder[4] = ((stringBuilder.Length - 5) / 2).ToString("X2")[1];
		stringBuilder.Append("\r\n");
		return Encoding.ASCII.GetBytes(stringBuilder.ToString());
	}

	private int AnalysisAddress(string address)
	{
		string value = address.Substring(2) + address.Substring(0, 2);
		return Convert.ToInt32(value);
	}

	private byte[] ReadFromSPBCore(byte[] receive)
	{
		if (receive.Length < 15)
		{
			return null;
		}
		if (receive[^2] == 13 && receive[^1] == 10)
		{
			receive = receive.RemoveLast(2);
		}
		string message = Encoding.ASCII.GetString(receive);
		int num = Convert.ToInt32(message.Substring(3, 2), 16);
		if (num != (message.Length - 5) / 2)
		{
			return CreateResponseBack(3, message, null);
		}
		if (message.Substring(9, 4) == "0000")
		{
			return ReadByCommand(message);
		}
		if (message.Substring(9, 4) == "0100")
		{
			return WriteByCommand(message);
		}
		if (message.Substring(9, 4) == "0102")
		{
			return WriteBitByCommand(message);
		}
		return null;
	}

	private byte[] ReadByCommand(string command)
	{
		string text = command.Substring(13, 2);
		int num = AnalysisAddress(command.Substring(15, 4));
		int num2 = AnalysisAddress(command.Substring(19, 4));
		if (num2 > 105)
		{
			CreateResponseBack(3, command, null);
		}
		return text switch
		{
			"0C" => CreateResponseBack(0, command, dBuffer.GetBytes(num * 2, num2 * 2)), 
			"0D" => CreateResponseBack(0, command, rBuffer.GetBytes(num * 2, num2 * 2)), 
			"0E" => CreateResponseBack(0, command, wBuffer.GetBytes(num * 2, num2 * 2)), 
			"01" => CreateResponseBack(0, command, xBuffer.GetBytes(num * 2, num2 * 2)), 
			"00" => CreateResponseBack(0, command, yBuffer.GetBytes(num * 2, num2 * 2)), 
			"02" => CreateResponseBack(0, command, mBuffer.GetBytes(num * 2, num2 * 2)), 
			_ => CreateResponseBack(2, command, null), 
		};
	}

	private byte[] WriteByCommand(string command)
	{
		if (!base.EnableWrite)
		{
			return CreateResponseBack(2, command, null);
		}
		string text = command.Substring(13, 2);
		int num = AnalysisAddress(command.Substring(15, 4));
		int num2 = AnalysisAddress(command.Substring(19, 4));
		if (num2 * 4 != command.Length - 23)
		{
			return CreateResponseBack(3, command, null);
		}
		byte[] data = command.Substring(23).ToHexBytes();
		switch (text)
		{
		case "0C":
			dBuffer.SetBytes(data, num * 2);
			return CreateResponseBack(0, command, null);
		case "0D":
			rBuffer.SetBytes(data, num * 2);
			return CreateResponseBack(0, command, null);
		case "0E":
			wBuffer.SetBytes(data, num * 2);
			return CreateResponseBack(0, command, null);
		case "00":
			yBuffer.SetBytes(data, num * 2);
			return CreateResponseBack(0, command, null);
		case "02":
			mBuffer.SetBytes(data, num * 2);
			return CreateResponseBack(0, command, null);
		default:
			return CreateResponseBack(2, command, null);
		}
	}

	private byte[] WriteBitByCommand(string command)
	{
		if (!base.EnableWrite)
		{
			return CreateResponseBack(2, command, null);
		}
		string text = command.Substring(13, 2);
		int num = AnalysisAddress(command.Substring(15, 4));
		int num2 = Convert.ToInt32(command.Substring(19, 2));
		bool value = command.Substring(21, 2) != "00";
		switch (text)
		{
		case "0C":
			dBuffer.SetBool(value, num * 8 + num2);
			return CreateResponseBack(0, command, null);
		case "0D":
			rBuffer.SetBool(value, num * 8 + num2);
			return CreateResponseBack(0, command, null);
		case "0E":
			wBuffer.SetBool(value, num * 8 + num2);
			return CreateResponseBack(0, command, null);
		case "00":
			yBuffer.SetBool(value, num * 8 + num2);
			return CreateResponseBack(0, command, null);
		case "02":
			mBuffer.SetBool(value, num * 8 + num2);
			return CreateResponseBack(0, command, null);
		default:
			return CreateResponseBack(2, command, null);
		}
	}

	protected override bool CheckSerialReceiveDataComplete(byte[] buffer, int receivedLength)
	{
		return ModbusInfo.CheckAsciiReceiveDataComplete(buffer, receivedLength);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			xBuffer.Dispose();
			yBuffer.Dispose();
			mBuffer.Dispose();
			dBuffer.Dispose();
			rBuffer.Dispose();
			wBuffer.Dispose();
		}
		base.Dispose(disposing);
	}

	public override string ToString()
	{
		return $"FujiSPBServer[{base.Port}]";
	}
}
