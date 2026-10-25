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
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Panasonic;

/// <summary>
/// 松下Mewtocol协议的虚拟服务器，支持串口和网口的操作
/// Panasonic Mewtocol protocol virtual server, supports serial and network port operations
/// </summary>
/// <remarks>
/// 地址的地址分为线圈型和整型，线圈支持X,Y,R,L, 字单位的整型支持 X,Y,R,L,D,LD,F
/// The address of the address is divided into coil type and integer type, the coil supports X, Y, R, L, and the integer type of word unit supports X, Y, R, L, D, LD, F
/// </remarks>
public class PanasonicMewtocolServer : DeviceServer
{
	private SoftBuffer xBuffer;

	private SoftBuffer rBuffer;

	private SoftBuffer dtBuffer;

	private SoftBuffer ldBuffer;

	private SoftBuffer flBuffer;

	private SoftBuffer yBuffer;

	private SoftBuffer lBuffer;

	private const int DataPoolLength = 65536;

	private byte station = 1;

	/// <summary>
	/// PLC设备的目标站号，需要根据实际的设置来填写
	/// The target station number of the PLC device needs to be filled in according to the actual settings
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
	/// 实例化一个默认的对象
	/// </summary>
	public PanasonicMewtocolServer()
	{
		rBuffer = new SoftBuffer(131072);
		dtBuffer = new SoftBuffer(131072);
		ldBuffer = new SoftBuffer(131072);
		flBuffer = new SoftBuffer(131072);
		xBuffer = new SoftBuffer(131072);
		lBuffer = new SoftBuffer(131072);
		yBuffer = new SoftBuffer(131072);
		base.ByteTransform = new RegularByteTransform();
		base.ByteTransform.DataFormat = DataFormat.DCBA;
		LogMsgFormatBinary = false;
		base.SerialReceiveAtleastTime = 2000;
	}

	protected override byte[] SaveToBytes()
	{
		byte[] buffer = new byte[917504];
		Array.Copy(rBuffer.GetBytes(), 0, buffer, 0, 131072);
		Array.Copy(dtBuffer.GetBytes(), 0, buffer, 131072, 131072);
		Array.Copy(ldBuffer.GetBytes(), 0, buffer, 262144, 131072);
		Array.Copy(flBuffer.GetBytes(), 0, buffer, 393216, 131072);
		Array.Copy(xBuffer.GetBytes(), 0, buffer, 524288, 131072);
		Array.Copy(lBuffer.GetBytes(), 0, buffer, 655360, 131072);
		Array.Copy(yBuffer.GetBytes(), 0, buffer, 786432, 131072);
		return buffer;
	}

	protected override void LoadFromBytes(byte[] content)
	{
		if (content.Length < 917504)
		{
			throw new Exception("File is not correct");
		}
		rBuffer.SetBytes(content, 0, 0, 131072);
		dtBuffer.SetBytes(content, 131072, 0, 131072);
		ldBuffer.SetBytes(content, 262144, 0, 131072);
		flBuffer.SetBytes(content, 393216, 0, 131072);
		xBuffer.SetBytes(content, 524288, 0, 131072);
		lBuffer.SetBytes(content, 655360, 0, 131072);
		yBuffer.SetBytes(content, 786432, 0, 131072);
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 读取指定地址的原始数据，地址示例：D0  F0  K0  T0  C0, 地址支持携带站号的访问方式，例如：s=2;D100
	/// Read the original data of the specified address, address example: D0 F0 K0 T0 C0, the address supports carrying station number information, for example: s=2;D100
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">起始地址</param>
	/// <param name="length">长度</param>
	/// <returns>原始的字节数据的信息</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<string, int> operateResult = PanasonicHelper.AnalysisAddress(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		if (operateResult.Content1 == "D")
		{
			return OperateResult.CreateSuccessResult(dtBuffer.GetBytes(operateResult.Content2 * 2, length * 2));
		}
		if (operateResult.Content1 == "LD")
		{
			return OperateResult.CreateSuccessResult(ldBuffer.GetBytes(operateResult.Content2 * 2, length * 2));
		}
		if (operateResult.Content1 == "F")
		{
			return OperateResult.CreateSuccessResult(flBuffer.GetBytes(operateResult.Content2 * 2, length * 2));
		}
		if (operateResult.Content1 == "X")
		{
			return OperateResult.CreateSuccessResult(xBuffer.GetBool(operateResult.Content2, length * 16).ToByteArray());
		}
		if (operateResult.Content1 == "Y")
		{
			return OperateResult.CreateSuccessResult(yBuffer.GetBool(operateResult.Content2, length * 16).ToByteArray());
		}
		if (operateResult.Content1 == "R")
		{
			return OperateResult.CreateSuccessResult(rBuffer.GetBool(operateResult.Content2, length * 16).ToByteArray());
		}
		if (operateResult.Content1 == "L")
		{
			return OperateResult.CreateSuccessResult(lBuffer.GetBool(operateResult.Content2, length * 16).ToByteArray());
		}
		return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 将数据写入到指定的地址里去，地址示例：D0  F0  K0  T0  C0, 地址支持携带站号的访问方式，例如：s=2;D100
	/// Write data to the specified address, address example: D0 F0 K0 T0 C0, the address supports carrying station number information, for example: s=2;D100
	/// </summary>
	/// <param name="plc">PLC对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">起始地址</param>
	/// <param name="value">真实数据</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		OperateResult<string, int> operateResult = PanasonicHelper.AnalysisAddress(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		if (operateResult.Content1 == "D")
		{
			dtBuffer.SetBytes(value, operateResult.Content2 * 2);
		}
		else if (operateResult.Content1 == "LD")
		{
			ldBuffer.SetBytes(value, operateResult.Content2 * 2);
		}
		else if (operateResult.Content1 == "F")
		{
			flBuffer.SetBytes(value, operateResult.Content2 * 2);
		}
		else if (operateResult.Content1 == "X")
		{
			xBuffer.SetBool(value.ToBoolArray(), operateResult.Content2);
		}
		else if (operateResult.Content1 == "Y")
		{
			yBuffer.SetBool(value.ToBoolArray(), operateResult.Content2);
		}
		else if (operateResult.Content1 == "R")
		{
			rBuffer.SetBool(value.ToBoolArray(), operateResult.Content2);
		}
		else
		{
			if (!(operateResult.Content1 == "L"))
			{
				return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
			}
			lBuffer.SetBool(value.ToBoolArray(), operateResult.Content2);
		}
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 批量读取松下PLC的位数据，按照字为单位，地址为 X0,X10,Y10，读取的长度为16的倍数
	/// Read the bit data of Panasonic PLC in batches, the unit is word, the address is X0, X10, Y10, and the read length is a multiple of 16
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">起始地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>读取结果对象</returns>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		OperateResult<string, int> operateResult = PanasonicHelper.AnalysisAddress(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		if (operateResult.Content1 == "X")
		{
			return OperateResult.CreateSuccessResult(xBuffer.GetBool(operateResult.Content2, length));
		}
		if (operateResult.Content1 == "Y")
		{
			return OperateResult.CreateSuccessResult(yBuffer.GetBool(operateResult.Content2, length));
		}
		if (operateResult.Content1 == "R")
		{
			return OperateResult.CreateSuccessResult(rBuffer.GetBool(operateResult.Content2, length));
		}
		if (operateResult.Content1 == "L")
		{
			return OperateResult.CreateSuccessResult(lBuffer.GetBool(operateResult.Content2, length));
		}
		return new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType);
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// 往指定的地址写入  数组，地址举例 X0.0  Y0.0  R0.0  L0.0，
	/// 起始的位地址必须为16的倍数，写入的  数组长度也为16的倍数。
	/// Write the  array to the specified address, address example: SR0.0 X0.0 Y0.0 R0.0 L0.0,
	/// the starting bit address must be a multiple of 16.  The length of the array is also a multiple of 16.
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="station">站号信息</param>
	/// <param name="address">起始地址</param>
	/// <param name="values">数据值信息</param>
	/// <returns>返回是否成功的结果对象</returns>
	public override OperateResult Write(string address, bool[] value)
	{
		OperateResult<string, int> operateResult = PanasonicHelper.AnalysisAddress(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		if (operateResult.Content1 == "X")
		{
			xBuffer.SetBool(value, operateResult.Content2);
		}
		else if (operateResult.Content1 == "Y")
		{
			yBuffer.SetBool(value, operateResult.Content2);
		}
		else if (operateResult.Content1 == "R")
		{
			rBuffer.SetBool(value, operateResult.Content2);
		}
		else
		{
			if (!(operateResult.Content1 == "L"))
			{
				return new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType);
			}
			lBuffer.SetBool(value, operateResult.Content2);
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new SpecifiedCharacterMessage(13);
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		if (receive.Length < 5)
		{
			return new OperateResult<byte[]>("Uknown Data：" + receive.ToHexString(' '));
		}
		return PanasonicHelper.PackPanasonicCommand(station, ReadFromCommand(receive), receive[0] == 60);
	}

	/// <summary>
	/// 创建一个失败的返回消息，指定错误码即可，会自动计算出来BCC校验和
	/// </summary>
	/// <param name="code">错误码</param>
	/// <returns>原始字节报文，用于反馈消息</returns>
	protected string CreateFailedResponse(byte code)
	{
		return "!" + code.ToString("D2");
	}

	/// <summary>
	/// 根据命令来获取相关的数据内容
	/// </summary>
	/// <param name="cmd">原始的命令码</param>
	/// <returns>返回的数据信息</returns>
	public virtual string ReadFromCommand(byte[] cmd)
	{
		try
		{
			string message = Encoding.ASCII.GetString(cmd);
			if (message[0] != '%' && message[0] != '<')
			{
				return CreateFailedResponse(41);
			}
			byte b = Convert.ToByte(message.Substring(1, 2), 16);
			bool flag = message[0] == '<';
			if (b != station)
			{
				base.LogNet?.WriteError(ToString(), $"Station not match, need:{station}, but now: {b}");
				return CreateFailedResponse(50);
			}
			if (message[3] != '#')
			{
				return CreateFailedResponse(41);
			}
			if (message.Substring(4, 3) == "RCS")
			{
				int destIndex = Convert.ToInt32(message.Substring(8, 3)) * 16 + Convert.ToInt32(message.Substring(11, 1), 16);
				if (message[7] == 'R')
				{
					return "$RC" + (rBuffer.GetBool(destIndex) ? "1" : "0");
				}
				if (message[7] == 'X')
				{
					return "$RC" + (xBuffer.GetBool(destIndex) ? "1" : "0");
				}
				if (message[7] == 'Y')
				{
					return "$RC" + (yBuffer.GetBool(destIndex) ? "1" : "0");
				}
				if (message[7] == 'L')
				{
					return "$RC" + (lBuffer.GetBool(destIndex) ? "1" : "0");
				}
				return CreateFailedResponse(42);
			}
			if (message.Substring(4, 3) == "WCS")
			{
				int destIndex2 = Convert.ToInt32(message.Substring(8, 3)) * 16 + Convert.ToInt32(message.Substring(11, 1), 16);
				if (message[7] == 'R')
				{
					rBuffer.SetBool(message[12] == '1', destIndex2);
					return "$WC";
				}
				if (message[7] == 'X')
				{
					xBuffer.SetBool(message[12] == '1', destIndex2);
					return "$WC";
				}
				if (message[7] == 'Y')
				{
					yBuffer.SetBool(message[12] == '1', destIndex2);
					return "$WC";
				}
				if (message[7] == 'L')
				{
					lBuffer.SetBool(message[12] == '1', destIndex2);
					return "$WC";
				}
				return CreateFailedResponse(42);
			}
			if (message.Substring(4, 3) == "RCP")
			{
				int num = message[7] - 48;
				if (num > 8)
				{
					return CreateFailedResponse(42);
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < num; i++)
				{
					int destIndex3 = Convert.ToInt32(message.Substring(9 + 5 * i, 3)) * 16 + Convert.ToInt32(message.Substring(12 + 5 * i, 1), 16);
					if (message[8 + 5 * i] == 'R')
					{
						stringBuilder.Append(rBuffer.GetBool(destIndex3) ? "1" : "0");
					}
					else if (message[8 + 5 * i] == 'X')
					{
						stringBuilder.Append(xBuffer.GetBool(destIndex3) ? "1" : "0");
					}
					else if (message[8 + 5 * i] == 'Y')
					{
						stringBuilder.Append(yBuffer.GetBool(destIndex3) ? "1" : "0");
					}
					else if (message[8 + 5 * i] == 'L')
					{
						stringBuilder.Append(lBuffer.GetBool(destIndex3) ? "1" : "0");
					}
				}
				return "$RC" + stringBuilder.ToString();
			}
			if (message.Substring(4, 3) == "WCP")
			{
				int num2 = message[7] - 48;
				if (num2 > 8)
				{
					return CreateFailedResponse(42);
				}
				for (int j = 0; j < num2; j++)
				{
					int destIndex4 = Convert.ToInt32(message.Substring(9 + 6 * j, 3)) * 16 + Convert.ToInt32(message.Substring(12 + 6 * j, 1), 16);
					if (message[8 + 6 * j] == 'R')
					{
						rBuffer.SetBool(message[13 + 6 * j] == '1', destIndex4);
					}
					else if (message[8 + 6 * j] == 'X')
					{
						xBuffer.SetBool(message[13 + 6 * j] == '1', destIndex4);
					}
					else if (message[8 + 6 * j] == 'Y')
					{
						yBuffer.SetBool(message[13 + 6 * j] == '1', destIndex4);
					}
					else if (message[8 + 6 * j] == 'L')
					{
						lBuffer.SetBool(message[13 + 6 * j] == '1', destIndex4);
					}
				}
				return "$WC";
			}
			if (message.Substring(4, 3) == "RCC")
			{
				int num3 = Convert.ToInt32(message.Substring(8, 4));
				int num4 = Convert.ToInt32(message.Substring(12, 4));
				int num5 = num4 - num3 + 1;
				if (num5 > (flag ? 509 : 27))
				{
					return CreateFailedResponse(42);
				}
				if (message[7] == 'R')
				{
					return "$RC" + rBuffer.GetBytes(num3 * 2, num5 * 2).ToHexString();
				}
				if (message[7] == 'X')
				{
					return "$RC" + xBuffer.GetBytes(num3 * 2, num5 * 2).ToHexString();
				}
				if (message[7] == 'Y')
				{
					return "$RC" + yBuffer.GetBytes(num3 * 2, num5 * 2).ToHexString();
				}
				if (message[7] == 'L')
				{
					return "$RC" + lBuffer.GetBytes(num3 * 2, num5 * 2).ToHexString();
				}
				return CreateFailedResponse(42);
			}
			if (message.Substring(4, 3) == "WCC")
			{
				int num6 = Convert.ToInt32(message.Substring(8, 4));
				int num7 = Convert.ToInt32(message.Substring(12, 4));
				int num8 = num7 - num6 + 1;
				byte[] array = message.Substring(16, num8 * 4).ToHexBytes();
				if (array.Length > (flag ? 2028 : 98))
				{
					return CreateFailedResponse(42);
				}
				if (message[7] == 'R')
				{
					rBuffer.SetBytes(array, num6 * 2);
					return "$WC";
				}
				if (message[7] == 'X')
				{
					xBuffer.SetBytes(array, num6 * 2);
					return "$WC";
				}
				if (message[7] == 'Y')
				{
					yBuffer.SetBytes(array, num6 * 2);
					return "$WC";
				}
				if (message[7] == 'L')
				{
					lBuffer.SetBytes(array, num6 * 2);
					return "$WC";
				}
				return CreateFailedResponse(42);
			}
			if (message.Substring(4, 2) == "RD")
			{
				int num9 = Convert.ToInt32(message.Substring(7, 5));
				int num10 = Convert.ToInt32(message.Substring(12, 5));
				int num11 = num10 - num9 + 1;
				if (num11 > (flag ? 509 : 27))
				{
					return CreateFailedResponse(42);
				}
				if (message[6] == 'D')
				{
					return "$RD" + dtBuffer.GetBytes(num9 * 2, num11 * 2).ToHexString();
				}
				if (message[6] == 'L')
				{
					return "$RD" + ldBuffer.GetBytes(num9 * 2, num11 * 2).ToHexString();
				}
				if (message[6] == 'F')
				{
					return "$RD" + flBuffer.GetBytes(num9 * 2, num11 * 2).ToHexString();
				}
				return CreateFailedResponse(42);
			}
			if (message.Substring(4, 2) == "WD")
			{
				int num12 = Convert.ToInt32(message.Substring(7, 5));
				int num13 = Convert.ToInt32(message.Substring(12, 5));
				int num14 = num13 - num12 + 1;
				byte[] array2 = message.Substring(17, num14 * 4).ToHexBytes();
				if (array2.Length > (flag ? 2028 : 98))
				{
					return CreateFailedResponse(42);
				}
				if (message[6] == 'D')
				{
					dtBuffer.SetBytes(array2, num12 * 2);
					return "$WD";
				}
				if (message[6] == 'L')
				{
					ldBuffer.SetBytes(array2, num12 * 2);
					return "$WD";
				}
				if (message[6] == 'F')
				{
					flBuffer.SetBytes(array2, num12 * 2);
					return "$WD";
				}
				return CreateFailedResponse(42);
			}
			if (message.Substring(4, 2) == "RT")
			{
				return "$RT0300160000000000";
			}
			return CreateFailedResponse(41);
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), ex);
			return CreateFailedResponse(41);
		}
	}

	protected override bool CheckSerialReceiveDataComplete(byte[] buffer, int receivedLength)
	{
		if (receivedLength > 5)
		{
			return buffer[receivedLength - 1] == 13;
		}
		return base.CheckSerialReceiveDataComplete(buffer, receivedLength);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			xBuffer?.Dispose();
			rBuffer?.Dispose();
			dtBuffer?.Dispose();
			ldBuffer?.Dispose();
			flBuffer?.Dispose();
			yBuffer?.Dispose();
			lBuffer?.Dispose();
		}
		base.Dispose(disposing);
	}

	public override string ToString()
	{
		return $"PanasonicMewtocolServer[{base.Port}]";
	}
}
