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
using System.Text;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 三菱PLC通讯协议，采用A兼容1E帧协议实现，使用ASCII码通讯，请根据实际型号来进行选取
/// Mitsubishi PLC communication protocol, implemented using A compatible 1E frame protocol, using ascii code communication, please choose according to the actual model
/// </summary>
public class MelsecA1EAsciiNet : DeviceTcpNet
{
	/// <summary>
	/// PLC编号，默认为0xFF
	/// PLC number, default is 0xFF
	/// </summary>
	public byte PLCNumber { get; set; } = byte.MaxValue;

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public MelsecA1EAsciiNet()
	{
		base.WordLength = 1;
		LogMsgFormatBinary = false;
		base.ByteTransform = new RegularByteTransform();
	}

	/// <summary>
	/// 指定ip地址和端口来来实例化一个默认的对象
	/// Specify the IP address and port to instantiate a default object
	/// </summary>
	/// <param name="ipAddress">PLC的Ip地址</param>
	/// <param name="port">PLC的端口</param>
	public MelsecA1EAsciiNet(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new MelsecA1EAsciiMessage();
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		OperateResult operateResult = CheckResponseLegal(response);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return base.UnpackResponseContent(send, response);
	}

	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, byte[] value)
	{
		return BuildWriteWordCommand(address, value, PLCNumber);
	}

	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, bool[] value)
	{
		return BuildWriteBoolCommand(address, value, PLCNumber);
	}

	protected override OperateResult<byte[]> ParseDeviceReadResult(string address, byte[] buildCommand, byte[] content)
	{
		return ExtractActualData(content, isBit: false);
	}

	protected override OperateResult<bool[]> ParseDeviceReadBoolResult(string address, byte[] buildCommand, byte[] content, ushort length)
	{
		OperateResult<byte[]> operateResult = ExtractActualData(content, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content.Select((byte m) => m == 1).ToArray());
	}

	protected override OperateResult<List<byte[]>> BuildDeviceReadCommand(string address, ushort length, bool isBit)
	{
		return BuildReadCommand(address, length, isBit, PLCNumber);
	}

	public override string ToString()
	{
		return $"MelsecA1ENet[{IpAddress}:{Port}]";
	}

	/// <summary>
	/// 根据类型地址长度确认需要读取的指令头
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">长度</param>
	/// <param name="isBit">指示是否按照位成批的读出</param>
	/// <param name="plcNumber">PLC编号</param>
	/// <returns>带有成功标志的指令数据</returns>
	public static OperateResult<List<byte[]>> BuildReadCommand(string address, ushort length, bool isBit, byte plcNumber)
	{
		OperateResult<MelsecA1EDataType, int> operateResult = MelsecHelper.McA1EAnalysisAddress(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<List<byte[]>>(operateResult);
		}
		byte value = ((!isBit) ? ((byte)1) : ((byte)0));
		int[] array = SoftBasic.SplitIntegerToArray(length, isBit ? 256 : 64);
		List<byte[]> list = new List<byte[]>();
		for (int i = 0; i < array.Length; i++)
		{
			byte[] buffer = new byte[24]
			{
				SoftBasic.BuildAsciiBytesFrom(value)[0],
				SoftBasic.BuildAsciiBytesFrom(value)[1],
				SoftBasic.BuildAsciiBytesFrom(plcNumber)[0],
				SoftBasic.BuildAsciiBytesFrom(plcNumber)[1],
				48,
				48,
				48,
				65,
				SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content1.DataCode)[1])[0],
				SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content1.DataCode)[1])[1],
				SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content1.DataCode)[0])[0],
				SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content1.DataCode)[0])[1],
				SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[3])[0],
				SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[3])[1],
				SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[2])[0],
				SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[2])[1],
				SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[1])[0],
				SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[1])[1],
				SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[0])[0],
				SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[0])[1],
				0,
				0,
				0,
				0
			};
			int num = array[i];
			if (num == 256)
			{
				num = 0;
			}
			buffer[20] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(num % 256)[0])[0];
			buffer[21] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(num % 256)[0])[1];
			buffer[22] = 48;
			buffer[23] = 48;
			list.Add(buffer);
			if (isBit)
			{
				operateResult.Content2 += array[i];
			}
			else if (operateResult.Content1.DataType == 1)
			{
				operateResult.Content2 += array[i] * 16;
			}
			else
			{
				operateResult.Content2 += array[i];
			}
		}
		return OperateResult.CreateSuccessResult(list);
	}

	/// <summary>
	/// 根据类型地址以及需要写入的数据来生成指令头
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">数据值</param>
	/// <param name="plcNumber">PLC编号</param>
	/// <returns>带有成功标志的指令数据</returns>
	public static OperateResult<byte[]> BuildWriteWordCommand(string address, byte[] value, byte plcNumber)
	{
		OperateResult<MelsecA1EDataType, int> operateResult = MelsecHelper.McA1EAnalysisAddress(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		value = MelsecHelper.TransByteArrayToAsciiByteArray(value);
		byte[] buffer = new byte[24 + value.Length];
		buffer[0] = 48;
		buffer[1] = 51;
		buffer[2] = SoftBasic.BuildAsciiBytesFrom(plcNumber)[0];
		buffer[3] = SoftBasic.BuildAsciiBytesFrom(plcNumber)[1];
		buffer[4] = 48;
		buffer[5] = 48;
		buffer[6] = 48;
		buffer[7] = 65;
		buffer[8] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content1.DataCode)[1])[0];
		buffer[9] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content1.DataCode)[1])[1];
		buffer[10] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content1.DataCode)[0])[0];
		buffer[11] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content1.DataCode)[0])[1];
		buffer[12] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[3])[0];
		buffer[13] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[3])[1];
		buffer[14] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[2])[0];
		buffer[15] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[2])[1];
		buffer[16] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[1])[0];
		buffer[17] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[1])[1];
		buffer[18] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[0])[0];
		buffer[19] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[0])[1];
		buffer[20] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(value.Length / 4)[0])[0];
		buffer[21] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(value.Length / 4)[0])[1];
		buffer[22] = 48;
		buffer[23] = 48;
		value.CopyTo(buffer, 24);
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 根据类型地址以及需要写入的数据来生成指令头
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">数据值</param>
	/// <param name="plcNumber">PLC编号</param>
	/// <returns>带有成功标志的指令数据</returns>
	public static OperateResult<byte[]> BuildWriteBoolCommand(string address, bool[] value, byte plcNumber)
	{
		OperateResult<MelsecA1EDataType, int> operateResult = MelsecHelper.McA1EAnalysisAddress(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		byte[] array = value.Select((bool m) => (byte)(m ? 49 : 48)).ToArray();
		if (array.Length % 2 == 1)
		{
			array = SoftBasic.SpliceArray<byte>(array, new byte[1] { 48 });
		}
		byte[] buffer = new byte[24 + array.Length];
		buffer[0] = 48;
		buffer[1] = 50;
		buffer[2] = SoftBasic.BuildAsciiBytesFrom(plcNumber)[0];
		buffer[3] = SoftBasic.BuildAsciiBytesFrom(plcNumber)[1];
		buffer[4] = 48;
		buffer[5] = 48;
		buffer[6] = 48;
		buffer[7] = 65;
		buffer[8] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content1.DataCode)[1])[0];
		buffer[9] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content1.DataCode)[1])[1];
		buffer[10] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content1.DataCode)[0])[0];
		buffer[11] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content1.DataCode)[0])[1];
		buffer[12] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[3])[0];
		buffer[13] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[3])[1];
		buffer[14] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[2])[0];
		buffer[15] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[2])[1];
		buffer[16] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[1])[0];
		buffer[17] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[1])[1];
		buffer[18] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[0])[0];
		buffer[19] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[0])[1];
		buffer[20] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(value.Length)[0])[0];
		buffer[21] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(value.Length)[0])[1];
		buffer[22] = 48;
		buffer[23] = 48;
		array.CopyTo(buffer, 24);
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 检测反馈的消息是否合法
	/// </summary>
	/// <param name="response">接收的报文</param>
	/// <returns>是否成功</returns>
	public static OperateResult CheckResponseLegal(byte[] response)
	{
		if (response.Length < 4)
		{
			return new OperateResult(StringResources.Language.ReceiveDataLengthTooShort);
		}
		if (response[2] == 48 && response[3] == 48)
		{
			return OperateResult.CreateSuccessResult();
		}
		if (response[2] == 53 && response[3] == 66)
		{
			return new OperateResult(Convert.ToInt32(Encoding.ASCII.GetString(response, 4, 2), 16), StringResources.Language.MelsecPleaseReferToManualDocument);
		}
		return new OperateResult(Convert.ToInt32(Encoding.ASCII.GetString(response, 2, 2), 16), StringResources.Language.MelsecPleaseReferToManualDocument);
	}

	/// <summary>
	/// 从PLC反馈的数据中提取出实际的数据内容，需要传入反馈数据，是否位读取
	/// </summary>
	/// <param name="response">反馈的数据内容</param>
	/// <param name="isBit">是否位读取</param>
	/// <returns>解析后的结果对象</returns>
	public static OperateResult<byte[]> ExtractActualData(byte[] response, bool isBit)
	{
		if (isBit)
		{
			return OperateResult.CreateSuccessResult((from m in response.RemoveBegin(4)
				select (m != 48) ? ((byte)1) : ((byte)0)).ToArray());
		}
		return OperateResult.CreateSuccessResult(MelsecHelper.TransAsciiByteArrayToByteArray(response.RemoveBegin(4)));
	}
}
