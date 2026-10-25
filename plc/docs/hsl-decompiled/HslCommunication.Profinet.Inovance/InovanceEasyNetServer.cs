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
using HslCommunication.Profinet.Panasonic;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Inovance;

/// <summary>
/// EasyNet协议的汇川PLC的虚拟服务器，主要用于测试的通讯使用，实际使用中请使用真实的PLC设备。
/// </summary>
public class InovanceEasyNetServer : DeviceServer
{
	private SoftBuffer xBuffer;

	private SoftBuffer yBuffer;

	private SoftBuffer bBuffer;

	private SoftBuffer mBuffer;

	private SoftBuffer sBuffer;

	private SoftBuffer dBuffer;

	private SoftBuffer rBuffer;

	private SoftBuffer wBuffer;

	private SoftBuffer uBuffer;

	private const int DataPoolLength = 65536;

	/// <summary>
	/// 实例化一个基于EasyNet协议虚拟汇川PLC对象，可以用来和进行通信测试。
	/// </summary>
	public InovanceEasyNetServer()
	{
		xBuffer = new SoftBuffer(65536);
		yBuffer = new SoftBuffer(65536);
		mBuffer = new SoftBuffer(65536);
		sBuffer = new SoftBuffer(65536);
		rBuffer = new SoftBuffer(131072);
		dBuffer = new SoftBuffer(131072);
		wBuffer = new SoftBuffer(131072);
		uBuffer = new SoftBuffer(131072);
		base.ByteTransform = new RegularByteTransform();
		base.WordLength = 1;
		base.SerialReceiveAtleastTime = 2000;
	}

	protected override byte[] SaveToBytes()
	{
		return ByteTransformHelper.GetByteFromSoftBuffer(xBuffer, yBuffer, mBuffer, sBuffer, rBuffer, dBuffer, wBuffer, uBuffer).ToArray();
	}

	protected override void LoadFromBytes(byte[] content)
	{
		ByteTransformHelper.SetByteToSoftBuffer(content, 0, xBuffer, yBuffer, mBuffer, sBuffer, rBuffer, dBuffer, wBuffer, uBuffer);
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组
	/// Batch read byte array information, need to specify the address and length, return the original byte array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		try
		{
			if (address.StartsWithAndNumber("D"))
			{
				return OperateResult.CreateSuccessResult(dBuffer.GetBytes(Convert.ToInt32(address.Substring(1)) * 2, length * 2));
			}
			if (address.StartsWithAndNumber("R"))
			{
				return OperateResult.CreateSuccessResult(rBuffer.GetBytes(Convert.ToInt32(address.Substring(1)) * 2, length * 2));
			}
			if (address.StartsWithAndNumber("W"))
			{
				return OperateResult.CreateSuccessResult(wBuffer.GetBytes(Convert.ToInt32(address.Substring(1)) * 2, length * 2));
			}
			if (address.StartsWithAndNumber("UB") || address.StartsWithAndNumber("UW"))
			{
				return OperateResult.CreateSuccessResult(uBuffer.GetBytes(Convert.ToInt32(address.Substring(2), 16), length * 2));
			}
			if (address.StartsWithAndNumber("U"))
			{
				return OperateResult.CreateSuccessResult(uBuffer.GetBytes(Convert.ToInt32(address.Substring(1), 16), length * 2));
			}
			if (address.StartsWithAndNumber("X"))
			{
				return OperateResult.CreateSuccessResult(xBuffer.GetBool(Convert.ToInt32(address.Substring(1), 8), length * 16).ToByteArray());
			}
			if (address.StartsWithAndNumber("Y"))
			{
				return OperateResult.CreateSuccessResult(yBuffer.GetBool(Convert.ToInt32(address.Substring(1), 8), length * 16).ToByteArray());
			}
			if (address.StartsWithAndNumber("M"))
			{
				return OperateResult.CreateSuccessResult(mBuffer.GetBool(Convert.ToInt32(address.Substring(1)), length * 16).ToByteArray());
			}
			if (address.StartsWithAndNumber("S"))
			{
				return OperateResult.CreateSuccessResult(sBuffer.GetBool(Convert.ToInt32(address.Substring(1)), length * 16).ToByteArray());
			}
			if (address.StartsWithAndNumber("B"))
			{
				return OperateResult.CreateSuccessResult(bBuffer.GetBool(Convert.ToInt32(address.Substring(1)), length * 16).ToByteArray());
			}
			return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType + " Reason:" + ex.Message);
		}
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 写入原始的byte数组数据到指定的地址，返回是否写入成功
	/// Write the original byte array data to the specified address, and return whether the write was successful
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">写入值</param>
	/// <returns>是否写入成功</returns>
	public override OperateResult Write(string address, byte[] value)
	{
		try
		{
			if (address.StartsWithAndNumber("D"))
			{
				dBuffer.SetBytes(value, Convert.ToInt32(address.Substring(1)) * 2);
			}
			if (address.StartsWithAndNumber("R"))
			{
				rBuffer.SetBytes(value, Convert.ToInt32(address.Substring(1)) * 2);
			}
			if (address.StartsWithAndNumber("W"))
			{
				wBuffer.SetBytes(value, Convert.ToInt32(address.Substring(1)) * 2);
			}
			if (address.StartsWithAndNumber("UB") || address.StartsWithAndNumber("UW"))
			{
				uBuffer.SetBytes(value, Convert.ToInt32(address.Substring(2), 16));
			}
			if (address.StartsWithAndNumber("U"))
			{
				uBuffer.SetBytes(value, Convert.ToInt32(address.Substring(1), 16));
			}
			if (address.StartsWithAndNumber("X"))
			{
				xBuffer.SetBool(value.ToBoolArray(), Convert.ToInt32(address.Substring(1), 8));
			}
			if (address.StartsWithAndNumber("Y"))
			{
				yBuffer.SetBool(value.ToBoolArray(), Convert.ToInt32(address.Substring(1), 8));
			}
			if (address.StartsWithAndNumber("M"))
			{
				mBuffer.SetBool(value.ToBoolArray(), Convert.ToInt32(address.Substring(1)));
			}
			if (address.StartsWithAndNumber("S"))
			{
				sBuffer.SetBool(value.ToBoolArray(), Convert.ToInt32(address.Substring(1)));
			}
			if (address.StartsWithAndNumber("B"))
			{
				bBuffer.SetBool(value.ToBoolArray(), Convert.ToInt32(address.Substring(1)));
			}
			return OperateResult.CreateSuccessResult();
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType + " Reason:" + ex.Message);
		}
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 批量读取数组信息，需要指定地址和长度，返回 数组
	/// Batch read  array information, need to specify the address and length, return  array
	/// </summary>
	/// <param name="address">数据地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>带有成功标识的 bool[] 数组</returns>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		try
		{
			if (address.StartsWithAndNumber("D"))
			{
				return OperateResult.CreateSuccessResult(dBuffer.GetBool(PanasonicHelper.CalculateComplexAddress(address.Substring(1)), length));
			}
			if (address.StartsWithAndNumber("R"))
			{
				return OperateResult.CreateSuccessResult(rBuffer.GetBool(PanasonicHelper.CalculateComplexAddress(address.Substring(1)), length));
			}
			if (address.StartsWithAndNumber("W"))
			{
				return OperateResult.CreateSuccessResult(wBuffer.GetBool(PanasonicHelper.CalculateComplexAddress(address.Substring(1)), length));
			}
			if (address.StartsWithAndNumber("X"))
			{
				return OperateResult.CreateSuccessResult(xBuffer.GetBool(Convert.ToInt32(address.Substring(1), 8), length));
			}
			if (address.StartsWithAndNumber("Y"))
			{
				return OperateResult.CreateSuccessResult(yBuffer.GetBool(Convert.ToInt32(address.Substring(1), 8), length));
			}
			if (address.StartsWithAndNumber("M"))
			{
				return OperateResult.CreateSuccessResult(mBuffer.GetBool(Convert.ToInt32(address.Substring(1)), length));
			}
			if (address.StartsWithAndNumber("S"))
			{
				return OperateResult.CreateSuccessResult(sBuffer.GetBool(Convert.ToInt32(address.Substring(1)), length));
			}
			if (address.StartsWithAndNumber("B"))
			{
				return OperateResult.CreateSuccessResult(bBuffer.GetBool(Convert.ToInt32(address.Substring(1)), length));
			}
			return new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType);
		}
		catch (Exception ex)
		{
			return new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType + " Reason:" + ex.Message);
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
			if (address.StartsWithAndNumber("D"))
			{
				dBuffer.SetBool(value, PanasonicHelper.CalculateComplexAddress(address.Substring(1)));
			}
			if (address.StartsWithAndNumber("R"))
			{
				rBuffer.SetBool(value, PanasonicHelper.CalculateComplexAddress(address.Substring(1)));
			}
			if (address.StartsWithAndNumber("W"))
			{
				wBuffer.SetBool(value, PanasonicHelper.CalculateComplexAddress(address.Substring(1)));
			}
			if (address.StartsWithAndNumber("X"))
			{
				xBuffer.SetBool(value, Convert.ToInt32(address.Substring(1), 8));
			}
			if (address.StartsWithAndNumber("Y"))
			{
				yBuffer.SetBool(value, Convert.ToInt32(address.Substring(1), 8));
			}
			if (address.StartsWithAndNumber("M"))
			{
				mBuffer.SetBool(value, Convert.ToInt32(address.Substring(1)));
			}
			if (address.StartsWithAndNumber("S"))
			{
				sBuffer.SetBool(value, Convert.ToInt32(address.Substring(1)));
			}
			if (address.StartsWithAndNumber("B"))
			{
				bBuffer.SetBool(value, Convert.ToInt32(address.Substring(1)));
			}
			return OperateResult.CreateSuccessResult();
		}
		catch (Exception ex)
		{
			return new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType + " Reason:" + ex.Message);
		}
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new InovanceEasyNetMessage();
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		return OperateResult.CreateSuccessResult(ReadFromNanoCore(receive));
	}

	private byte[] ReadFromNanoCore(byte[] receive)
	{
		if (receive[8] == 1)
		{
			return ReadByCommand(receive);
		}
		if (receive[8] == 2)
		{
			return WriteByCommand(receive);
		}
		return Encoding.ASCII.GetBytes("E0\r\n");
	}

	private byte[] GetReadBack(byte[] receive, SoftBuffer softBuffer, int address, int length)
	{
		byte[] array = softBuffer.GetBool(address, length).ToByteArray();
		byte[] buffer = new byte[receive.Length + array.Length];
		receive.CopyTo(buffer, 0);
		array.CopyTo(buffer, receive.Length);
		buffer[0] = BitConverter.GetBytes(buffer.Length)[0];
		buffer[1] = BitConverter.GetBytes(buffer.Length)[1];
		return buffer;
	}

	private byte[] GetWriteBack(byte[] receive, int err)
	{
		if (err == 0)
		{
			return "09 00 01 03 01 00 00 00 06".ToHexBytes();
		}
		byte[] array = "0E 00 01 03 01 00 00 00 0F 00 81 00 00 00".ToHexBytes();
		BitConverter.GetBytes(err).CopyTo(array, 10);
		return array;
	}

	private byte[] ReadByCommand(byte[] receive)
	{
		try
		{
			int num = receive[16] & 0xF0;
			receive[16] = (byte)(receive[16] & 0xF);
			int num2 = BitConverter.ToInt32(new byte[4]
			{
				receive[14],
				receive[15],
				(byte)(receive[16] & 0xF),
				0
			}, 0);
			int length = BitConverter.ToInt32(receive, 18);
			switch (num)
			{
			case 0:
				if (num2 < 524288)
				{
					return GetReadBack(receive, xBuffer, num2, length);
				}
				return GetReadBack(receive, yBuffer, num2 - 524288, length);
			case 16:
				if (num2 < 524288)
				{
					return GetReadBack(receive, mBuffer, num2, length);
				}
				return GetReadBack(receive, sBuffer, num2 - 524288, length);
			case 32:
				return GetReadBack(receive, bBuffer, num2, length);
			case 64:
				return GetReadBack(receive, dBuffer, num2, length);
			case 80:
				return GetReadBack(receive, rBuffer, num2, length);
			case 96:
				return GetReadBack(receive, wBuffer, num2, length);
			case 240:
				return GetReadBack(receive, uBuffer, num2, length);
			default:
				return GetWriteBack(receive, 1);
			}
		}
		catch
		{
			return GetWriteBack(receive, 2);
		}
	}

	private byte[] WriteByCommand(byte[] receive)
	{
		if (!base.EnableWrite)
		{
			return GetWriteBack(receive, 3);
		}
		try
		{
			int num = receive[16] & 0xF0;
			receive[16] = (byte)(receive[16] & 0xF);
			int num2 = BitConverter.ToInt32(new byte[4]
			{
				receive[14],
				receive[15],
				(byte)(receive[16] & 0xF),
				0
			}, 0);
			int num3 = BitConverter.ToInt32(receive, 18);
			bool[] array = receive.RemoveBegin(22).ToBoolArray();
			if (array.Length > num3)
			{
				array = array.SelectBegin(num3);
			}
			switch (num)
			{
			case 0:
				if (num2 < 524288)
				{
					xBuffer.SetBool(array, num2);
				}
				else
				{
					yBuffer.SetBool(array, num2);
				}
				break;
			case 16:
				if (num2 < 524288)
				{
					mBuffer.SetBool(array, num2);
				}
				else
				{
					sBuffer.SetBool(array, num2);
				}
				break;
			case 32:
				bBuffer.SetBool(array, num2);
				break;
			case 64:
				dBuffer.SetBool(array, num2);
				break;
			case 80:
				rBuffer.SetBool(array, num2);
				break;
			case 96:
				wBuffer.SetBool(array, num2);
				break;
			case 240:
				uBuffer.SetBool(array, num2);
				break;
			}
			return GetWriteBack(receive, 0);
		}
		catch
		{
			return GetWriteBack(receive, 2);
		}
	}

	protected override bool CheckSerialReceiveDataComplete(byte[] buffer, int receivedLength)
	{
		if (receivedLength < 1)
		{
			return false;
		}
		return buffer[receivedLength - 1] == 13;
	}

	public override string ToString()
	{
		return $"KeyenceNanoServer[{base.Port}]";
	}
}
