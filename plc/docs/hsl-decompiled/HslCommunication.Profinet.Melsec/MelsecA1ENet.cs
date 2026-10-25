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
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 三菱PLC通讯协议，采用A兼容1E帧协议实现，使用二进制码通讯，请根据实际型号来进行选取
/// Mitsubishi PLC communication protocol, implemented using A compatible 1E frame protocol, using binary code communication, please choose according to the actual model
/// </summary>
/// <remarks>
/// 本类适用于的PLC列表
/// FX3U(C) PLC   测试人sandy_liao
/// 本通讯类由CKernal推送，感谢
/// </remarks>
public class MelsecA1ENet : DeviceTcpNet
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
	public MelsecA1ENet()
	{
		base.WordLength = 1;
		base.ByteTransform = new RegularByteTransform();
	}

	/// <summary>
	/// 指定ip地址和端口来来实例化一个默认的对象
	/// Specify the IP address and port to instantiate a default object
	/// </summary>
	/// <param name="ipAddress">PLC的Ip地址</param>
	/// <param name="port">PLC的端口</param>
	public MelsecA1ENet(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new MelsecA1EBinaryMessage();
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
		byte b = ((!isBit) ? ((byte)1) : ((byte)0));
		int[] array = SoftBasic.SplitIntegerToArray(length, isBit ? 256 : 64);
		List<byte[]> list = new List<byte[]>();
		for (int i = 0; i < array.Length; i++)
		{
			byte[] buffer = new byte[12]
			{
				b,
				plcNumber,
				10,
				0,
				BitConverter.GetBytes(operateResult.Content2)[0],
				BitConverter.GetBytes(operateResult.Content2)[1],
				BitConverter.GetBytes(operateResult.Content2)[2],
				BitConverter.GetBytes(operateResult.Content2)[3],
				BitConverter.GetBytes(operateResult.Content1.DataCode)[0],
				BitConverter.GetBytes(operateResult.Content1.DataCode)[1],
				0,
				0
			};
			int num = array[i];
			if (num == 256)
			{
				num = 0;
			}
			buffer[10] = BitConverter.GetBytes(num)[0];
			buffer[11] = BitConverter.GetBytes(num)[1];
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
		byte[] buffer = new byte[12 + value.Length];
		buffer[0] = 3;
		buffer[1] = plcNumber;
		buffer[2] = 10;
		buffer[3] = 0;
		buffer[4] = BitConverter.GetBytes(operateResult.Content2)[0];
		buffer[5] = BitConverter.GetBytes(operateResult.Content2)[1];
		buffer[6] = BitConverter.GetBytes(operateResult.Content2)[2];
		buffer[7] = BitConverter.GetBytes(operateResult.Content2)[3];
		buffer[8] = BitConverter.GetBytes(operateResult.Content1.DataCode)[0];
		buffer[9] = BitConverter.GetBytes(operateResult.Content1.DataCode)[1];
		buffer[10] = BitConverter.GetBytes(value.Length / 2)[0];
		buffer[11] = BitConverter.GetBytes(value.Length / 2)[1];
		Array.Copy(value, 0, buffer, 12, value.Length);
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
		byte[] array = MelsecHelper.TransBoolArrayToByteData(value);
		byte[] buffer = new byte[12 + array.Length];
		buffer[0] = 2;
		buffer[1] = plcNumber;
		buffer[2] = 10;
		buffer[3] = 0;
		buffer[4] = BitConverter.GetBytes(operateResult.Content2)[0];
		buffer[5] = BitConverter.GetBytes(operateResult.Content2)[1];
		buffer[6] = BitConverter.GetBytes(operateResult.Content2)[2];
		buffer[7] = BitConverter.GetBytes(operateResult.Content2)[3];
		buffer[8] = BitConverter.GetBytes(operateResult.Content1.DataCode)[0];
		buffer[9] = BitConverter.GetBytes(operateResult.Content1.DataCode)[1];
		buffer[10] = BitConverter.GetBytes(value.Length)[0];
		buffer[11] = BitConverter.GetBytes(value.Length)[1];
		Array.Copy(array, 0, buffer, 12, array.Length);
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 检测反馈的消息是否合法
	/// </summary>
	/// <param name="response">接收的报文</param>
	/// <returns>是否成功</returns>
	public static OperateResult CheckResponseLegal(byte[] response)
	{
		if (response.Length < 2)
		{
			return new OperateResult(StringResources.Language.ReceiveDataLengthTooShort);
		}
		if (response[1] == 0)
		{
			return OperateResult.CreateSuccessResult();
		}
		if (response[1] == 91 && response.Length > 2)
		{
			return new OperateResult(response[2], StringResources.Language.MelsecPleaseReferToManualDocument);
		}
		return new OperateResult(response[1], StringResources.Language.MelsecPleaseReferToManualDocument);
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
			byte[] buffer = new byte[(response.Length - 2) * 2];
			for (int i = 2; i < response.Length; i++)
			{
				if ((response[i] & 0x10) == 16)
				{
					buffer[(i - 2) * 2] = 1;
				}
				if ((response[i] & 1) == 1)
				{
					buffer[(i - 2) * 2 + 1] = 1;
				}
			}
			return OperateResult.CreateSuccessResult(buffer);
		}
		return OperateResult.CreateSuccessResult(response.RemoveBegin(2));
	}
}
