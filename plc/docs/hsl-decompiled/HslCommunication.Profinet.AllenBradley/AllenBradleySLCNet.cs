// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.AllenBradley;

/// <summary>
/// AllenBradley品牌的PLC，针对SLC系列的通信的实现，测试PLC为1747。
/// AllenBradley brand PLC, for the realization of SLC series communication, the test PLC is 1747.
/// </summary>
/// <remarks>
/// 地址支持的举例：A9:0, N9:0, B9:0, F9:0, S:0, ST1:0, C9:0, I:0/10, O:0/1, R9:0, T9:0, L9:0
/// </remarks>
public class AllenBradleySLCNet : DeviceTcpNet
{
	/// <summary>
	/// The current session handle, which is determined by the PLC when communicating with the PLC handshake
	/// </summary>
	public uint SessionHandle { get; protected set; }

	/// <summary>
	/// Instantiate a communication object for a Allenbradley PLC protocol
	/// </summary>
	public AllenBradleySLCNet()
	{
		base.WordLength = 2;
		base.ByteTransform = new RegularByteTransform();
	}

	/// <summary>
	/// Instantiate a communication object for a Allenbradley PLC protocol
	/// </summary>
	/// <param name="ipAddress">PLC IpAddress</param>
	/// <param name="port">PLC Port</param>
	public AllenBradleySLCNet(string ipAddress, int port = 44818)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new AllenBradleySLCMessage();
	}

	protected override OperateResult InitializationOnConnect()
	{
		OperateResult<byte[]> operateResult = ReadFromCoreServer(CommunicationPipe, "01 01 00 00 00 00 00 00 00 00 00 00 00 04 00 05 00 00 00 00 00 00 00 00 00 00 00 00".ToHexBytes(), hasResponseData: true, usePackAndUnpack: true);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		if (operateResult.Content.Length >= 8)
		{
			SessionHandle = base.ByteTransform.TransUInt32(operateResult.Content, 4);
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override async Task<OperateResult> InitializationOnConnectAsync()
	{
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(CommunicationPipe, "01 01 00 00 00 00 00 00 00 00 00 00 00 04 00 05 00 00 00 00 00 00 00 00 00 00 00 00".ToHexBytes(), hasResponseData: true, usePackAndUnpack: true).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return read;
		}
		if (read.Content.Length >= 8)
		{
			SessionHandle = base.ByteTransform.TransUInt32(read.Content, 4);
		}
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// Read data information, data length for read array length information
	/// </summary>
	/// <param name="address">Address format of the node</param>
	/// <param name="length">In the case of arrays, the length of the array</param>
	/// <returns>Result data with result object</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<byte[]> operateResult = BuildReadCommand(address, length);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(PackCommand(operateResult.Content));
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		OperateResult<byte[]> operateResult3 = ExtraActualContent(operateResult2.Content);
		if (!operateResult3.IsSuccess)
		{
			return operateResult3;
		}
		return OperateResult.CreateSuccessResult(operateResult3.Content);
	}

	[HslMqttApi("WriteByteArray", "")]
	public override OperateResult Write(string address, byte[] value)
	{
		OperateResult<byte[]> operateResult = BuildWriteCommand(address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(PackCommand(operateResult.Content));
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		OperateResult<byte[]> operateResult3 = ExtraActualContent(operateResult2.Content);
		if (!operateResult3.IsSuccess)
		{
			return operateResult3;
		}
		return OperateResult.CreateSuccessResult(operateResult3.Content);
	}

	[HslMqttApi("ReadBool", "")]
	public override OperateResult<bool> ReadBool(string address)
	{
		address = AnalysisBitIndex(address, out var bitIndex);
		OperateResult<byte[]> operateResult = Read(address, 1);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool>(operateResult);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content.ToBoolArray()[bitIndex]);
	}

	[HslMqttApi("WriteBool", "")]
	public override OperateResult Write(string address, bool value)
	{
		OperateResult<byte[]> operateResult = BuildWriteCommand(address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(PackCommand(operateResult.Content));
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		OperateResult<byte[]> operateResult3 = ExtraActualContent(operateResult2.Content);
		if (!operateResult3.IsSuccess)
		{
			return operateResult3;
		}
		return OperateResult.CreateSuccessResult(operateResult3.Content);
	}

	/// <summary>
	/// Read data information, data length for read array length information
	/// </summary>
	/// <param name="address">Address format of the node</param>
	/// <param name="length">In the case of arrays, the length of the array</param>
	/// <returns>Result data with result object</returns>
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		OperateResult<byte[]> command = BuildReadCommand(address, length);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(PackCommand(command.Content));
		if (!read.IsSuccess)
		{
			return read;
		}
		OperateResult<byte[]> extra = ExtraActualContent(read.Content);
		if (!extra.IsSuccess)
		{
			return extra;
		}
		return OperateResult.CreateSuccessResult(extra.Content);
	}

	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		OperateResult<byte[]> command = BuildWriteCommand(address, value);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(PackCommand(command.Content));
		if (!read.IsSuccess)
		{
			return read;
		}
		OperateResult<byte[]> extra = ExtraActualContent(read.Content);
		if (!extra.IsSuccess)
		{
			return extra;
		}
		return OperateResult.CreateSuccessResult(extra.Content);
	}

	public override async Task<OperateResult<bool>> ReadBoolAsync(string address)
	{
		address = AnalysisBitIndex(address, out var bitIndex);
		OperateResult<byte[]> read = await ReadAsync(address, 1);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool>(read);
		}
		return OperateResult.CreateSuccessResult(read.Content.ToBoolArray()[bitIndex]);
	}

	public override async Task<OperateResult> WriteAsync(string address, bool value)
	{
		OperateResult<byte[]> command = BuildWriteCommand(address, value);
		if (!command.IsSuccess)
		{
			return command;
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(PackCommand(command.Content));
		if (!read.IsSuccess)
		{
			return read;
		}
		OperateResult<byte[]> extra = ExtraActualContent(read.Content);
		if (!extra.IsSuccess)
		{
			return extra;
		}
		return OperateResult.CreateSuccessResult(extra.Content);
	}

	private byte[] PackCommand(byte[] coreCmd)
	{
		byte[] buffer = new byte[28 + coreCmd.Length];
		buffer[0] = 1;
		buffer[1] = 7;
		buffer[2] = (byte)(coreCmd.Length / 256);
		buffer[3] = (byte)(coreCmd.Length % 256);
		BitConverter.GetBytes(SessionHandle).CopyTo(buffer, 4);
		coreCmd.CopyTo(buffer, 28);
		return buffer;
	}

	/// <summary>
	/// 分析地址数据信息里的位索引的信息
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="bitIndex">位索引</param>
	/// <returns>地址信息</returns>
	public static string AnalysisBitIndex(string address, out int bitIndex)
	{
		bitIndex = 0;
		int num = address.IndexOf('/');
		if (num < 0)
		{
			num = address.IndexOf('.');
		}
		if (num > 0)
		{
			bitIndex = int.Parse(address.Substring(num + 1));
			address = address.Substring(0, num);
		}
		return address;
	}

	/// <summary>
	/// 构建读取的指令信息
	/// </summary>
	/// <param name="address">地址信息，举例：A9:0</param>
	/// <param name="length">读取的长度信息</param>
	/// <returns>是否成功</returns>
	public static OperateResult<byte[]> BuildReadCommand(string address, int length)
	{
		OperateResult<AllenBradleySLCAddress> operateResult = AllenBradleySLCAddress.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		if (length < 2)
		{
			length = 2;
		}
		if (operateResult.Content.DataCode == 142)
		{
			operateResult.Content.AddressStart /= 2;
		}
		byte[] buffer = new byte[14]
		{
			0,
			5,
			0,
			0,
			15,
			0,
			0,
			1,
			162,
			(byte)length,
			(byte)operateResult.Content.DbBlock,
			operateResult.Content.DataCode,
			0,
			0
		};
		BitConverter.GetBytes((ushort)operateResult.Content.AddressStart).CopyTo(buffer, 12);
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 构建写入的报文内容，变成实际的数据
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <param name="value">数据值</param>
	/// <returns>是否成功的结果对象</returns>
	public static OperateResult<byte[]> BuildWriteCommand(string address, byte[] value)
	{
		OperateResult<AllenBradleySLCAddress> operateResult = AllenBradleySLCAddress.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		if (operateResult.Content.DataCode == 142)
		{
			operateResult.Content.AddressStart /= 2;
		}
		byte[] buffer = new byte[18 + value.Length];
		buffer[0] = 0;
		buffer[1] = 5;
		buffer[2] = 0;
		buffer[3] = 0;
		buffer[4] = 15;
		buffer[5] = 0;
		buffer[6] = 0;
		buffer[7] = 1;
		buffer[8] = 171;
		buffer[9] = byte.MaxValue;
		buffer[10] = BitConverter.GetBytes(value.Length)[0];
		buffer[11] = BitConverter.GetBytes(value.Length)[1];
		buffer[12] = (byte)operateResult.Content.DbBlock;
		buffer[13] = operateResult.Content.DataCode;
		BitConverter.GetBytes((ushort)operateResult.Content.AddressStart).CopyTo(buffer, 14);
		buffer[16] = byte.MaxValue;
		buffer[17] = byte.MaxValue;
		value.CopyTo(buffer, 18);
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 构建写入的报文内容，变成实际的数据
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <param name="value">数据值</param>
	/// <returns>是否成功的结果对象</returns>
	public static OperateResult<byte[]> BuildWriteCommand(string address, bool value)
	{
		address = AnalysisBitIndex(address, out var bitIndex);
		OperateResult<AllenBradleySLCAddress> operateResult = AllenBradleySLCAddress.ParseFrom(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		if (operateResult.Content.DataCode == 142)
		{
			operateResult.Content.AddressStart /= 2;
		}
		int value2 = 1 << bitIndex;
		byte[] buffer = new byte[20]
		{
			0,
			5,
			0,
			0,
			15,
			0,
			0,
			1,
			171,
			255,
			2,
			0,
			(byte)operateResult.Content.DbBlock,
			operateResult.Content.DataCode,
			0,
			0,
			0,
			0,
			0,
			0
		};
		BitConverter.GetBytes((ushort)operateResult.Content.AddressStart).CopyTo(buffer, 14);
		buffer[16] = BitConverter.GetBytes(value2)[0];
		buffer[17] = BitConverter.GetBytes(value2)[1];
		if (value)
		{
			buffer[18] = BitConverter.GetBytes(value2)[0];
			buffer[19] = BitConverter.GetBytes(value2)[1];
		}
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 解析当前的实际报文内容，变成数据内容
	/// </summary>
	/// <param name="content">报文内容</param>
	/// <returns>是否成功</returns>
	public static OperateResult<byte[]> ExtraActualContent(byte[] content)
	{
		if (content.Length < 36)
		{
			return new OperateResult<byte[]>(StringResources.Language.ReceiveDataLengthTooShort + content.ToHexString(' '));
		}
		return OperateResult.CreateSuccessResult(content.RemoveBegin(36));
	}
}
