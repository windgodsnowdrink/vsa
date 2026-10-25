// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Net;

namespace HslCommunication.Profinet.Knx;

/// <summary>
/// Knx协议
/// </summary>
public class KnxCode
{
	public delegate void ReturnData(byte[] data);

	public delegate void GetData(short addr, byte len, byte[] data);

	private bool is_fresh = false;

	/// <summary>
	/// 序号计数
	/// </summary>
	public byte SequenceCounter { get; set; }

	/// <summary>
	/// 通道
	/// </summary>
	public byte Channel { get; set; }

	/// <summary>
	/// 连接状态
	/// </summary>
	public bool IsConnect { get; private set; }

	/// <summary>
	/// 返回需要写入KNX总线的应答报文（应答数据）
	/// </summary>
	public event ReturnData Return_data_msg;

	/// <summary>
	/// 返回需要写入的KNX系统的报文（写入数据）
	/// </summary>
	public event ReturnData Set_knx_data;

	/// <summary>
	/// 返回从knx系统得到的数据
	/// </summary>
	public event GetData GetData_msg;

	/// <summary>
	/// 关闭KNX连接
	/// </summary>
	/// <param name="channel">通道号</param>
	/// <param name="IP_PROT">本机IP</param>
	public byte[] Disconnect_knx(byte channel, IPEndPoint IP_PROT)
	{
		byte[] addressBytes = IP_PROT.Address.GetAddressBytes();
		byte[] bytes = BitConverter.GetBytes(IP_PROT.Port);
		return new byte[16]
		{
			6,
			16,
			2,
			9,
			0,
			16,
			channel,
			0,
			8,
			1,
			addressBytes[0],
			addressBytes[1],
			addressBytes[2],
			addressBytes[3],
			bytes[1],
			bytes[0]
		};
	}

	/// <summary>
	/// 返回握手报文
	/// </summary>
	/// <param name="IP_PROT">本机ip地址</param>
	public byte[] Handshake(IPEndPoint IP_PROT)
	{
		byte[] addressBytes = IP_PROT.Address.GetAddressBytes();
		byte[] bytes = BitConverter.GetBytes(IP_PROT.Port);
		return new byte[26]
		{
			6,
			16,
			2,
			5,
			0,
			26,
			8,
			1,
			addressBytes[0],
			addressBytes[1],
			addressBytes[2],
			addressBytes[3],
			bytes[1],
			bytes[0],
			8,
			1,
			addressBytes[0],
			addressBytes[1],
			addressBytes[2],
			addressBytes[3],
			bytes[1],
			bytes[0],
			4,
			4,
			2,
			0
		};
	}

	/// <summary>
	/// KNX报文解析
	/// </summary>
	public void KNX_check(byte[] in_data)
	{
		switch (in_data[2])
		{
		case 2:
			KNX_serverOF_2(in_data);
			break;
		case 4:
			KNX_serverOF_4(in_data);
			break;
		}
	}

	/// <summary>
	/// 写入数据到KNX系统
	/// </summary>
	/// <param name="addr">地址</param>
	/// <param name="len">长度</param>
	/// <param name="data">数据</param>
	public void Knx_Write(short addr, byte len, byte[] data)
	{
		byte[] bytes = BitConverter.GetBytes(addr);
		byte[] buffer = new byte[20 + len];
		byte[] bytes2 = BitConverter.GetBytes(buffer.Length);
		if (SequenceCounter + 1 <= 255)
		{
			if (is_fresh)
			{
				byte sequenceCounter = (byte)(SequenceCounter + 1);
				SequenceCounter = sequenceCounter;
			}
			else
			{
				is_fresh = true;
			}
		}
		else
		{
			SequenceCounter = 0;
		}
		buffer[0] = 6;
		buffer[1] = 16;
		buffer[2] = 4;
		buffer[3] = 32;
		buffer[4] = bytes2[1];
		buffer[5] = bytes2[0];
		buffer[6] = 4;
		buffer[7] = Channel;
		buffer[8] = SequenceCounter;
		buffer[9] = 0;
		buffer[10] = 17;
		buffer[11] = 0;
		buffer[12] = 188;
		buffer[13] = 224;
		buffer[14] = 0;
		buffer[15] = 0;
		buffer[16] = bytes[1];
		buffer[17] = bytes[0];
		buffer[18] = len;
		buffer[19] = 0;
		if (len == 1)
		{
			byte[] bytes3 = BitConverter.GetBytes((data[0] & 0x3F) | 0x80);
			buffer[20] = bytes3[0];
		}
		else
		{
			buffer[20] = 128;
			for (int i = 2; i <= len; i++)
			{
				buffer[len - 1 + 20] = data[i - 2];
			}
		}
		if (Set_knx_data != null)
		{
			Set_knx_data(buffer);
		}
	}

	/// <summary>
	/// 从KNX获取数据
	/// </summary>
	public void Knx_Resd_step1(short addr)
	{
		byte[] bytes = BitConverter.GetBytes(addr);
		byte[] buffer = new byte[21];
		byte[] bytes2 = BitConverter.GetBytes(buffer.Length);
		if (SequenceCounter + 1 <= 255)
		{
			if (is_fresh)
			{
				byte sequenceCounter = (byte)(SequenceCounter + 1);
				SequenceCounter = sequenceCounter;
			}
			else
			{
				is_fresh = true;
			}
		}
		else
		{
			SequenceCounter = 0;
		}
		buffer[0] = 6;
		buffer[1] = 16;
		buffer[2] = 4;
		buffer[3] = 32;
		buffer[4] = bytes2[1];
		buffer[5] = bytes2[0];
		buffer[6] = 4;
		buffer[7] = Channel;
		buffer[8] = SequenceCounter;
		buffer[9] = 0;
		buffer[10] = 17;
		buffer[11] = 0;
		buffer[12] = 188;
		buffer[13] = 224;
		buffer[14] = 0;
		buffer[15] = 0;
		buffer[16] = bytes[1];
		buffer[17] = bytes[0];
		buffer[18] = 1;
		buffer[19] = 0;
		buffer[20] = 0;
		if (Set_knx_data != null)
		{
			Return_data_msg(buffer);
		}
	}

	/// <summary>
	/// 连接保持（每隔1s发送一次到设备）
	/// </summary>
	public void knx_server_is_real(IPEndPoint IP_PROT)
	{
		byte[] buffer = new byte[16];
		byte[] addressBytes = IP_PROT.Address.GetAddressBytes();
		byte[] bytes = BitConverter.GetBytes(IP_PROT.Port);
		buffer[0] = 6;
		buffer[1] = 16;
		buffer[2] = 2;
		buffer[3] = 7;
		buffer[4] = 0;
		buffer[5] = 16;
		buffer[6] = Channel;
		buffer[7] = 0;
		buffer[8] = 8;
		buffer[9] = 1;
		buffer[10] = addressBytes[0];
		buffer[11] = addressBytes[1];
		buffer[12] = addressBytes[2];
		buffer[13] = addressBytes[3];
		buffer[14] = bytes[1];
		buffer[15] = bytes[0];
		if (Return_data_msg != null)
		{
			Return_data_msg(buffer);
		}
	}

	/// <summary>
	/// 暂时没有注释
	/// </summary>
	public short Get_knx_addr(string addr, out bool is_ok)
	{
		short result = 0;
		string[] array = addr.Split('\\');
		if (array.Length == 3)
		{
			int num = int.Parse(array[0]);
			int num2 = int.Parse(array[1]);
			int num3 = int.Parse(array[2]);
			if (num > 31 || num2 > 7 || num3 > 255 || num < 0 || num2 < 0 || num3 < 0)
			{
				Console.WriteLine("地址不合法");
				is_ok = false;
				return result;
			}
			num <<= 11;
			num2 <<= 8;
			int num4 = num | num2 | num3;
			result = (short)num4;
			is_ok = true;
			return result;
		}
		Console.WriteLine("地址不合法");
		is_ok = false;
		return result;
	}

	private void KNX_serverOF_2(byte[] in_data)
	{
		switch (in_data[3])
		{
		case 5:
			break;
		case 6:
			Extraction_of_Channel(in_data);
			break;
		case 7:
			Return_status();
			break;
		case 8:
			break;
		}
	}

	/// <summary>
	/// 返回连接状态
	/// </summary>
	private void Return_status()
	{
		byte[] buffer = new byte[8] { 6, 16, 2, 8, 0, 8, Channel, 0 };
		if (Return_data_msg != null)
		{
			Return_data_msg(buffer);
		}
	}

	/// <summary>
	/// 从握手回复报文获取通道号
	/// </summary>
	private void Extraction_of_Channel(byte[] in_data)
	{
		Channel = in_data[6];
		if ((in_data[5] == 8) & (in_data[7] == 37))
		{
			IsConnect = false;
		}
		if (Channel > 0)
		{
			IsConnect = true;
		}
	}

	private void KNX_serverOF_4(byte[] in_data)
	{
		switch (in_data[3])
		{
		case 32:
			Read_com_CEMI(in_data);
			break;
		}
	}

	/// <summary>
	/// 解析控制包头和CEMI
	/// </summary>
	private void Read_com_CEMI(byte[] in_data)
	{
		Read_CEMI(in_data);
	}

	/// <summary>
	/// 具体解析CEMI
	/// </summary>
	private void Read_CEMI(byte[] in_data)
	{
		if (in_data.Length > 11)
		{
			switch (in_data[10])
			{
			case 41:
				Read_CEMI_29(in_data);
				break;
			case 46:
				Read_CEMI_2e(in_data);
				break;
			}
		}
	}

	private void Read_CEMI_2e(byte[] in_data)
	{
		byte[] buffer = new byte[10]
		{
			6,
			16,
			4,
			33,
			0,
			10,
			4,
			Channel,
			in_data[8],
			0
		};
		if (Set_knx_data != null)
		{
			Return_data_msg(buffer);
		}
	}

	private void Read_CEMI_29(byte[] in_data)
	{
		short addr = BitConverter.ToInt16(new byte[2]
		{
			in_data[17],
			in_data[16]
		}, 0);
		byte[] array;
		if (in_data[18] > 1)
		{
			array = new byte[in_data[17]];
			for (int i = 0; i < in_data[18] - 1; i++)
			{
				array[i] = in_data[21 + i];
			}
		}
		else
		{
			array = BitConverter.GetBytes(in_data[20] & 0x3F);
		}
		if (GetData_msg != null)
		{
			GetData_msg(addr, in_data[18], array);
		}
		Read_setp6(in_data);
	}

	private void Read_setp6(byte[] in_data)
	{
		byte[] buffer = new byte[10]
		{
			6,
			16,
			4,
			33,
			0,
			10,
			4,
			Channel,
			in_data[8],
			0
		};
		if (Return_data_msg != null)
		{
			Return_data_msg(buffer);
		}
	}
}
