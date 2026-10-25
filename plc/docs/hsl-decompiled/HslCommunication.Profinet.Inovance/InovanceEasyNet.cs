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
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;

namespace HslCommunication.Profinet.Inovance;

/// <summary>
/// Easy系列的通信协议，使用的是专用的报文，地址说明参考Demo界面
/// </summary>
public class InovanceEasyNet : DeviceTcpNet
{
	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public InovanceEasyNet()
	{
		base.ByteTransform = new RegularByteTransform();
		base.WordLength = 1;
	}

	/// <summary>
	/// 指定IP地址和端口号来实例化一个新的对象
	/// Specify the IP address and port number to instantiate a new object
	/// </summary>
	/// <param name="ipAddress">Ip地址</param>
	/// <param name="port">端口号</param>
	public InovanceEasyNet(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new InovanceEasyNetMessage();
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		if (response.Length < 9)
		{
			return new OperateResult<byte[]>("Received data length is too short: " + response.Length + " Source: " + response.ToHexString(' '));
		}
		if (response.Length >= 9 && response[8] == 15)
		{
			int err = ((response.Length >= 14) ? BitConverter.ToUInt16(response, 10) : 10000);
			return new OperateResult<byte[]>(err, "PLC returned an error code: " + err + " Source: " + response.ToHexString(' '));
		}
		return base.UnpackResponseContent(send, response);
	}

	protected override OperateResult<List<byte[]>> BuildDeviceReadCommand(string address, ushort length, bool isBit)
	{
		return BuildReadCommand(address, length, isBit);
	}

	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, byte[] value)
	{
		return BuildWriteCommand(address, value);
	}

	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, bool[] value)
	{
		return BuildWriteCommand(address, value);
	}

	protected override OperateResult<byte[]> ParseDeviceReadResult(string address, byte[] buildCommand, byte[] content)
	{
		if (content.Length > 22)
		{
			return OperateResult.CreateSuccessResult(content.RemoveBegin(22));
		}
		return new OperateResult<byte[]>("Received data length is too short");
	}

	protected override OperateResult<bool[]> ParseDeviceReadBoolResult(string address, byte[] buildCommand, byte[] content, ushort length)
	{
		if (content.Length > 22)
		{
			return OperateResult.CreateSuccessResult(content.RemoveBegin(22).ToBoolArray(length));
		}
		return new OperateResult<bool[]>("Received data length is too short");
	}

	/// <summary>
	/// 构建读取的报文，返回一个列表，包含了所有需要发送的报文内容
	/// Build the message for reading, return a list that contains all the message content that needs to be sent
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <param name="length">读取的数据长度</param>
	/// <param name="isBit">是否为位读取</param>
	/// <returns>包含所有需要发送的报文内容的列表</returns>
	public OperateResult<List<byte[]>> BuildReadCommand(string address, ushort length, bool isBit)
	{
		List<byte[]> list = new List<byte[]>();
		byte[] array = "16 00 01 03 01 00 00 00 01 00 01 00 00 00 40 00 60 00 10 00 00 00".ToHexBytes();
		OperateResult<byte[]> operateResult = AnalysisAddressBytes(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<List<byte[]>>(operateResult);
		}
		operateResult.Content.CopyTo(array, 14);
		if (isBit)
		{
			array[18] = BitConverter.GetBytes(length)[0];
			array[19] = BitConverter.GetBytes(length)[1];
		}
		else
		{
			array[18] = BitConverter.GetBytes(length * 16)[0];
			array[19] = BitConverter.GetBytes(length * 16)[1];
			array[20] = BitConverter.GetBytes(length * 16)[2];
		}
		list.Add(array);
		return OperateResult.CreateSuccessResult(list);
	}

	/// <summary>
	/// 构建写入的报文，返回一个列表，包含了所有需要发送的报文内容
	/// Build the message for writing, return a list that contains all the message content that needs to be sent
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <param name="value">要写入的数据</param>
	/// <returns>包含所有需要发送的报文内容的列表</returns>
	public OperateResult<byte[]> BuildWriteCommand(string address, byte[] value)
	{
		byte[] buffer = new byte[22 + value.Length];
		"16 00 01 03 01 00 00 00 02 00 01 00 00 00 20 00 60 00 10 00 00 00".ToHexBytes().CopyTo(buffer, 0);
		OperateResult<byte[]> operateResult = AnalysisAddressBytes(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		operateResult.Content.CopyTo(buffer, 14);
		value.CopyTo(buffer, 22);
		buffer[0] = BitConverter.GetBytes(buffer.Length)[0];
		buffer[1] = BitConverter.GetBytes(buffer.Length)[1];
		buffer[18] = BitConverter.GetBytes(value.Length * 8)[0];
		buffer[19] = BitConverter.GetBytes(value.Length * 8)[1];
		buffer[20] = BitConverter.GetBytes(value.Length * 8)[2];
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 构建写入的报文，返回一个列表，包含了所有需要发送的报文内容
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <param name="value">要写入的数据</param>
	/// <returns>包含所有需要发送的报文内容的列表</returns>
	public OperateResult<byte[]> BuildWriteCommand(string address, bool[] value)
	{
		byte[] array = value.ToByteArray();
		byte[] buffer = new byte[22 + array.Length];
		"16 00 01 03 01 00 00 00 02 00 01 00 00 00 20 00 60 00 10 00 00 00".ToHexBytes().CopyTo(buffer, 0);
		OperateResult<byte[]> operateResult = AnalysisAddressBytes(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		operateResult.Content.CopyTo(buffer, 14);
		array.CopyTo(buffer, 22);
		buffer[0] = BitConverter.GetBytes(buffer.Length)[0];
		buffer[1] = BitConverter.GetBytes(buffer.Length)[1];
		buffer[18] = BitConverter.GetBytes(value.Length)[0];
		buffer[19] = BitConverter.GetBytes(value.Length)[1];
		buffer[20] = BitConverter.GetBytes(value.Length)[2];
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 分析地址信息，返回一个3字节的数组，前两字节是地址信息，第三个字节包含了类型和位的信息
	/// </summary>
	/// <param name="address">PLC的地址</param>
	public static OperateResult<byte[]> AnalysisAddressBytes(string address)
	{
		try
		{
			byte[] buffer = new byte[4];
			byte b = 0;
			int value = 0;
			if (address.StartsWithAndNumber("W"))
			{
				b = 96;
			}
			else if (address.StartsWithAndNumber("D"))
			{
				b = 64;
			}
			else if (address.StartsWithAndNumber("R"))
			{
				b = 80;
			}
			else if (address.StartsWithAndNumber("X"))
			{
				b = 0;
				value = Convert.ToInt32(address.Substring(1), 8);
			}
			else if (address.StartsWithAndNumber("Y"))
			{
				b = 0;
				value = Convert.ToInt32(address.Substring(1), 8) + 524288;
			}
			else if (address.StartsWithAndNumber("M"))
			{
				b = 16;
				value = Convert.ToInt32(address.Substring(1));
			}
			else if (address.StartsWithAndNumber("S"))
			{
				b = 16;
				value = Convert.ToInt32(address.Substring(1)) + 524288;
			}
			else if (address.StartsWithAndNumber("B"))
			{
				b = 32;
				value = Convert.ToInt32(address.Substring(1));
			}
			else if (address.StartsWithNoCase("UB") || address.StartsWithNoCase("UW"))
			{
				b = 240;
				value = Convert.ToInt32(address.Substring(2), 16);
			}
			else
			{
				if (!address.StartsWithNoCase("U"))
				{
					return new OperateResult<byte[]>("Only support W type address");
				}
				b = 240;
				value = Convert.ToInt32(address.Substring(1), 16);
			}
			address = address.Substring(1);
			if (b == 240)
			{
				BitConverter.GetBytes(value).CopyTo(buffer, 0);
			}
			else if (b >= 64)
			{
				string[] array2 = address.SplitDot();
				value = ushort.Parse(array2[0]) * 16;
				if (array2.Length > 1)
				{
					value += HslHelper.CalculateBitStartIndex(array2[1]);
				}
				buffer[0] = BitConverter.GetBytes(value)[0];
				buffer[1] = BitConverter.GetBytes(value)[1];
				buffer[2] = BitConverter.GetBytes(value)[2];
				buffer[2] = (byte)((buffer[2] & 0xF) | b);
			}
			else
			{
				buffer[0] = BitConverter.GetBytes(value)[0];
				buffer[1] = BitConverter.GetBytes(value)[1];
				buffer[2] = BitConverter.GetBytes(value)[2];
				buffer[2] = (byte)((buffer[2] & 0xF) | b);
			}
			return OperateResult.CreateSuccessResult(buffer);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("Address analysis failed: " + ex.Message);
		}
	}

	public override string ToString()
	{
		return $"InovanceEasyNet[{IpAddress}:{Port}]";
	}
}
