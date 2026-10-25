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
using HslCommunication.Profinet.Yamatake.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Yamatake;

/// <summary>
/// 山武的数字指示调节器的虚拟设备，支持和HSL本身进行数据通信测试
/// Yamatake’s digital indicating regulator is a virtual device that supports data communication testing with HSL itself
/// </summary>
public class DigitronCPLServer : DeviceServer
{
	private SoftBuffer softBuffer;

	private const int DataPoolLength = 65536;

	/// <summary>
	/// 获取或设置当前虚拟仪表的站号信息，如果站号不一致，将不予访问
	/// Get or set the station number information of the current virtual instrument. If the station number is inconsistent, it will not be accessed
	/// </summary>
	public byte Station { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public DigitronCPLServer()
	{
		softBuffer = new SoftBuffer(131072);
		base.ByteTransform = new RegularByteTransform();
		Station = 1;
		LogMsgFormatBinary = false;
		base.SerialReceiveAtleastTime = 1000;
	}

	[HslMqttApi("ReadByteArray", "")]
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		try
		{
			ushort num = ushort.Parse(address);
			return OperateResult.CreateSuccessResult(softBuffer.GetBytes(num * 2, length * 2));
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("Read Failed: " + ex.Message);
		}
	}

	[HslMqttApi("WriteByteArray", "")]
	public override OperateResult Write(string address, byte[] value)
	{
		try
		{
			ushort num = ushort.Parse(address);
			softBuffer.SetBytes(value, num * 2);
			return OperateResult.CreateSuccessResult();
		}
		catch (Exception ex)
		{
			return new OperateResult("Write Failed: " + ex.Message);
		}
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new SpecifiedCharacterMessage(10);
	}

	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		return OperateResult.CreateSuccessResult(ReadFromCore(session, receive));
	}

	private byte[] ReadFromCore(PipeSession session, byte[] command)
	{
		try
		{
			int num = 9;
			for (int i = 9; i < command.Length; i++)
			{
				if (command[i] == 3)
				{
					num = i;
					break;
				}
			}
			byte b = Convert.ToByte(Encoding.ASCII.GetString(command, 1, 2), 16);
			if (b != Station)
			{
				base.LogNet?.WriteDebug(ToString(), $"<{session.Communication}> Station not same, need: {Station} but {b}");
				return DigitronCPLHelper.PackResponseContent(Station, 40, null, 87);
			}
			string[] array = Encoding.ASCII.GetString(command, 9, num - 9).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			string message = Encoding.ASCII.GetString(command, 6, 2);
			int num2 = int.Parse(array[0].Substring(0, array[0].Length - 1));
			byte b2 = (byte)(array[0].EndsWith("W") ? 87 : 83);
			if (num2 >= 65536 || num2 < 0)
			{
				return DigitronCPLHelper.PackResponseContent(Station, 42, null, b2);
			}
			if (message == "RS")
			{
				int num3 = int.Parse(array[1]);
				if (num2 + num3 > 65535)
				{
					return DigitronCPLHelper.PackResponseContent(Station, 42, null, b2);
				}
				if (num3 > 16)
				{
					return DigitronCPLHelper.PackResponseContent(Station, 41, null, b2);
				}
				return DigitronCPLHelper.PackResponseContent(Station, 0, softBuffer.GetBytes(num2 * 2, num3 * 2), b2);
			}
			if (message == "WS")
			{
				if (!base.EnableWrite)
				{
					return DigitronCPLHelper.PackResponseContent(Station, 46, null, b2);
				}
				if (array.Length > 17)
				{
					return DigitronCPLHelper.PackResponseContent(Station, 41, null, b2);
				}
				byte[] buffer = new byte[(array.Length - 1) * 2];
				for (int j = 1; j < array.Length; j++)
				{
					if (b2 == 87)
					{
						BitConverter.GetBytes(short.Parse(array[j])).CopyTo(buffer, j * 2 - 2);
					}
					else
					{
						BitConverter.GetBytes(ushort.Parse(array[j])).CopyTo(buffer, j * 2 - 2);
					}
				}
				softBuffer.SetBytes(buffer, num2 * 2);
				return DigitronCPLHelper.PackResponseContent(Station, 0, null, b2);
			}
			return DigitronCPLHelper.PackResponseContent(Station, 40, null, b2);
		}
		catch
		{
			return null;
		}
	}

	protected override bool CheckSerialReceiveDataComplete(byte[] buffer, int receivedLength)
	{
		if (receivedLength > 5)
		{
			return buffer[receivedLength - 2] == 13 && buffer[receivedLength - 1] == 10;
		}
		return base.CheckSerialReceiveDataComplete(buffer, receivedLength);
	}

	public override string ToString()
	{
		return $"DigitronCPLServer[{base.Port}]";
	}
}
