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
using System.Net;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.Pipe;
using HslCommunication.Profinet.Melsec.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 三菱串口协议的网络版，如果使用的是 FX3U编程口(fx2n) -> GOT1000(RS232)(或是GOT2000)  -> 上位机(以太网) 的方式，那么就需要把设置为 True
/// The network version of the Mitsubishi serial port protocol, if you use the FX3U programming port (fx2n) -> GOT1000 (RS232) (or GOT2000) -> host computer (Ethernet) method,
/// then you need to put  is set to True
/// </summary>
/// <remarks>
/// 一般老旧的型号，例如FX2N之类的，需要将设置为False，如果是FX3U新的型号，则需要将设置为True
/// </remarks>
public class MelsecFxSerialOverTcp : DeviceTcpNet, IMelsecFxSerial, IReadWriteNet
{
	private List<string> inis = new List<string>
	{
		"00 10 02 FF FF FC 01 10 03", "10025E000000FC00000000001212000000FFFF030000FF0300002D001C091A18000000000000000000FC000012120414000113960AC4E5D7010201000000023030453032303203364310033543", "10025E000000FC00000000001212000000FFFF030000FF0300002D001C091A18000000000000000000FC000012120414000113960AC4E5D7010202000000023030454341303203384510033833", "10025E000000FC00000000001212000000FFFF030000FF0300002D001C091A18000000000000000000FC000012120414000113960AC4E5D7010203000000023030453032303203364310033545", "10025E000000FC00000000001212000000FFFF030000FF0300002D001C091A18000000000000000000FC000012120414000113960AC4E5D7010204000000023030454341303203384510033835", "10025E000000FC00000000001212000000FFFF030000FF0300002F001C091A18000000000000000000FC000012120414000113960AC4E5D70102050000000245303138303030343003443510034342", "10025E000000FC00000000001212000000FFFF030000FF0300002F001C091A18000000000000000000FC000012120414000113960AC4E5D70102060000000245303138303430314303453910034535", "10025E000000FC00000000001212000000FFFF030000FF0300002F001C091A18000000000000000000FC000012120414000113960AC4E5D70102070000000245303030453030343003453110034436", "10025E000000FC00000000001212000000FFFF030000FF0300002F001C091A18000000000000000000FC000012120414000113960AC4E5D70102080000000245303030453430343003453510034446", "10025E000000FC00000000001212000000FFFF030000FF0300002F001C091A18000000000000000000FC000012120414000113960AC4E5D70102090000000245303030453830343003453910034538",
		"10025E000000FC00000000001212000000FFFF030000FF0300002F001C091A18000000000000000000FC000012120414000113960AC4E5D701020A0000000245303030454330343003463410034630"
	};

	private bool useGot = false;

	private SoftIncrementCount incrementCount;

	/// <summary>
	/// 获取或设置是否使用GOT连接三菱的PLC，当使用了GOT连接到
	/// </summary>
	public bool UseGOT
	{
		get
		{
			return useGot;
		}
		set
		{
			useGot = value;
		}
	}

	/// <summary>
	/// 当前的编程口协议是否为新版，默认为新版，如果无法读取，切换旧版再次尝试
	/// Whether the current programming port protocol is the new version, the default is the new version,
	/// if it cannot be read, switch to the old version and try again
	/// </summary>
	public bool IsNewVersion { get; set; }

	/// <summary>
	/// 实例化网络版的三菱的串口协议的通讯对象
	/// Instantiate the communication object of Mitsubishi's serial protocol on the network
	/// </summary>
	public MelsecFxSerialOverTcp()
	{
		base.WordLength = 1;
		base.ByteTransform = new RegularByteTransform();
		IsNewVersion = true;
		base.ByteTransform.IsStringReverseByteWord = true;
		base.SleepTime = 20;
		incrementCount = new SoftIncrementCount(2147483647L, 1L);
		LogMsgFormatBinary = false;
	}

	/// <summary>
	/// 指定ip地址及端口号来实例化三菱的串口协议的通讯对象
	/// Specify the IP address and port number to instantiate the communication object of Mitsubishi's serial protocol
	/// </summary>
	/// <param name="ipAddress">Ip地址</param>
	/// <param name="port">端口号</param>
	public MelsecFxSerialOverTcp(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	private byte[] GetBytesSend(byte[] command)
	{
		List<byte> list = new List<byte>();
		for (int i = 0; i < command.Length; i++)
		{
			if (i < 2)
			{
				list.Add(command[i]);
			}
			else if (i < command.Length - 4)
			{
				if (command[i] == 16)
				{
					list.Add(command[i]);
				}
				list.Add(command[i]);
			}
			else
			{
				list.Add(command[i]);
			}
		}
		return list.ToArray();
	}

	private byte[] GetBytesReceive(byte[] response)
	{
		List<byte> list = new List<byte>();
		for (int i = 0; i < response.Length; i++)
		{
			if (i < 2)
			{
				list.Add(response[i]);
			}
			else if (i < response.Length - 4)
			{
				if (response[i] == 16 && response[i + 1] == 16)
				{
					list.Add(response[i]);
					i++;
				}
				else
				{
					list.Add(response[i]);
				}
			}
			else
			{
				list.Add(response[i]);
			}
		}
		return list.ToArray();
	}

	public override byte[] PackCommandWithHeader(byte[] command)
	{
		if (useGot)
		{
			byte[] buffer = new byte[66 + command.Length];
			buffer[0] = 16;
			buffer[1] = 2;
			buffer[2] = 94;
			buffer[6] = 252;
			buffer[12] = 18;
			buffer[13] = 18;
			buffer[17] = byte.MaxValue;
			buffer[18] = byte.MaxValue;
			buffer[19] = 3;
			buffer[22] = byte.MaxValue;
			buffer[23] = 3;
			buffer[26] = BitConverter.GetBytes(34 + command.Length)[0];
			buffer[27] = BitConverter.GetBytes(34 + command.Length)[1];
			buffer[28] = 28;
			buffer[29] = 9;
			buffer[30] = 26;
			buffer[31] = 24;
			buffer[41] = 252;
			buffer[44] = 18;
			buffer[45] = 18;
			buffer[46] = 4;
			buffer[47] = 20;
			buffer[49] = 1;
			buffer[50] = BitConverter.GetBytes(Port)[1];
			buffer[51] = BitConverter.GetBytes(Port)[0];
			buffer[52] = IPAddress.Parse(IpAddress).GetAddressBytes()[0];
			buffer[53] = IPAddress.Parse(IpAddress).GetAddressBytes()[1];
			buffer[54] = IPAddress.Parse(IpAddress).GetAddressBytes()[2];
			buffer[55] = IPAddress.Parse(IpAddress).GetAddressBytes()[3];
			buffer[56] = 1;
			buffer[57] = 2;
			BitConverter.GetBytes((int)incrementCount.GetCurrentValue()).CopyTo(buffer, 58);
			command.CopyTo(buffer, 62);
			buffer[^4] = 16;
			buffer[^3] = 3;
			MelsecHelper.FxCalculateCRC(buffer, 2, 4).CopyTo(buffer, buffer.Length - 2);
			return GetBytesSend(buffer);
		}
		return base.PackCommandWithHeader(command);
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		if (useGot)
		{
			if (response.Length > 68)
			{
				response = GetBytesReceive(response);
				int num = -1;
				for (int i = 0; i < response.Length - 4; i++)
				{
					if (response[i] == 16 && response[i + 1] == 2)
					{
						num = i;
						break;
					}
				}
				if (num >= 0)
				{
					return OperateResult.CreateSuccessResult(response.RemoveDouble(64 + num, 4));
				}
			}
			return new OperateResult<byte[]>("Got failed: " + response.ToHexString(' ', 16));
		}
		return base.UnpackResponseContent(send, response);
	}

	protected override OperateResult InitializationOnConnect()
	{
		if (useGot)
		{
			for (int i = 0; i < inis.Count; i++)
			{
				OperateResult operateResult = ReadFromCoreServer(CommunicationPipe, inis[i].ToHexBytes(), hasResponseData: true, usePackAndUnpack: false);
				if (!operateResult.IsSuccess)
				{
					return operateResult;
				}
			}
		}
		return base.InitializationOnConnect();
	}

	public override OperateResult<byte[]> ReadFromCoreServer(CommunicationPipe pipe, byte[] send, bool hasResponseData = true, bool usePackAndUnpack = true)
	{
		OperateResult<byte[]> operateResult = base.ReadFromCoreServer(pipe, send, hasResponseData, usePackAndUnpack);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		if (operateResult.Content == null)
		{
			return operateResult;
		}
		if (operateResult.Content.Length >= 2)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = base.ReadFromCoreServer(pipe, send, hasResponseData, usePackAndUnpack);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return OperateResult.CreateSuccessResult(SoftBasic.SpliceArray<byte>(operateResult.Content, operateResult2.Content));
	}

	public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(CommunicationPipe pipe, byte[] send, bool hasResponseData = true, bool usePackAndUnpack = true)
	{
		OperateResult<byte[]> read = await base.ReadFromCoreServerAsync(pipe, send, hasResponseData, usePackAndUnpack).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		if (read.Content == null)
		{
			return read;
		}
		if (read.Content.Length >= 2)
		{
			return read;
		}
		OperateResult<byte[]> read2 = await base.ReadFromCoreServerAsync(pipe, send, hasResponseData, usePackAndUnpack).ConfigureAwait(continueOnCapturedContext: false);
		if (!read2.IsSuccess)
		{
			return read2;
		}
		return OperateResult.CreateSuccessResult(SoftBasic.SpliceArray<byte>(read.Content, read2.Content));
	}

	protected override async Task<OperateResult> InitializationOnConnectAsync()
	{
		if (useGot)
		{
			for (int i = 0; i < inis.Count; i++)
			{
				OperateResult ini1 = await ReadFromCoreServerAsync(CommunicationPipe, inis[i].ToHexBytes(), hasResponseData: true, usePackAndUnpack: false).ConfigureAwait(continueOnCapturedContext: false);
				if (!ini1.IsSuccess)
				{
					return ini1;
				}
			}
		}
		return await base.InitializationOnConnectAsync();
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
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		return await MelsecFxSerialHelper.ReadAsync(this, address, length, IsNewVersion);
	}

	/// <summary>
	/// 根据指定的地址向PLC写入数据，数据格式为原始的字节类型
	/// Write data to the PLC according to the specified address, the data format is the original byte type
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="address">初始地址，支持的类型参考文档说明</param>
	/// <param name="value">原始的字节数据</param>
	/// <param name="isNewVersion">是否是新版的串口访问类</param>
	/// <returns>是否写入成功的结果对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await MelsecFxSerialHelper.WriteAsync(this, address, value, IsNewVersion);
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
	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		return await MelsecFxSerialHelper.ReadBoolAsync(this, address, length, IsNewVersion);
	}

	/// <summary>
	/// 强制写入位数据的通断，支持的类型参考文档说明
	/// The on-off of the forced write bit data, please refer to the document description for the supported types
	/// </summary>
	/// <param name="plc">PLC通信对象</param>
	/// <param name="address">地址信息</param>
	/// <param name="value">是否为通</param>
	/// <param name="isNewVersion">是否是新版的串口访问类</param>
	/// <returns>是否写入成功的结果对象</returns>
	public override async Task<OperateResult> WriteAsync(string address, bool value)
	{
		return await MelsecFxSerialHelper.WriteAsync(this, address, value, IsNewVersion);
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

	public override string ToString()
	{
		return $"MelsecFxSerialOverTcp[{IpAddress}:{Port}]";
	}
}
