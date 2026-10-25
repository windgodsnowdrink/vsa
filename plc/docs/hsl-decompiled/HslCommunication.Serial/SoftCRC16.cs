// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.Serial;

/// <summary>
/// 用于CRC16验证的类，提供了标准的验证方法，可以方便快速的对数据进行CRC校验
/// The class for CRC16 validation provides a standard validation method that makes it easy to CRC data quickly
/// </summary>
/// <remarks>
/// 本类提供了几个静态的方法，用来进行CRC16码的计算和验证的，多项式码可以自己指定配置，但是预置的寄存器为0xFF 0xFF
/// </remarks>
public class SoftCRC16
{
	/// <summary>
	/// 来校验对应的接收数据的CRC校验码，默认多项式码为0xA001
	/// To verify the CRC check code corresponding to the received data, the default polynomial code is 0xA001
	/// </summary>
	/// <param name="value">需要校验的数据，带CRC校验码</param>
	/// <returns>返回校验成功与否</returns>
	public static bool CheckCRC16(byte[] value)
	{
		return CheckCRC16(value, 160, 1);
	}

	/// <summary>
	/// 指定多项式码来校验对应的接收数据的CRC校验码
	/// Specifies a polynomial code to validate the corresponding CRC check code for the received data
	/// </summary>
	/// <param name="value">需要校验的数据，带CRC校验码</param>
	/// <param name="CH">多项式码高位</param>
	/// <param name="CL">多项式码低位</param>
	/// <returns>返回校验成功与否</returns>
	public static bool CheckCRC16(byte[] value, byte CH, byte CL)
	{
		if (value == null)
		{
			return false;
		}
		if (value.Length < 2)
		{
			return false;
		}
		int count = value.Length;
		byte[] buffer = new byte[count - 2];
		Array.Copy(value, 0, buffer, 0, buffer.Length);
		byte[] array2 = CRC16(buffer, CH, CL);
		if (array2[count - 2] == value[count - 2] && array2[count - 1] == value[count - 1])
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// 获取对应的数据的CRC校验码，默认多项式码为0xA001
	/// Get the CRC check code of the corresponding data, the default polynomial code is 0xA001
	/// </summary>
	/// <param name="value">需要校验的数据，不包含CRC字节</param>
	/// <returns>返回带CRC校验码的字节数组，可用于串口发送</returns>
	public static byte[] CRC16(byte[] value)
	{
		return CRC16(value, 160, 1);
	}

	/// <summary>
	/// 通过指定多项式码来获取对应的数据的CRC校验码
	/// The CRC check code of the corresponding data is obtained by specifying the polynomial code
	/// </summary>
	/// <param name="value">需要校验的数据，不包含CRC字节</param>
	/// <param name="CL">多项式码地位</param>
	/// <param name="CH">多项式码高位</param>
	/// <param name="preH">预置的高位值</param>
	/// <param name="preL">预置的低位值</param>
	/// <returns>返回带CRC校验码的字节数组，可用于串口发送</returns>
	public static byte[] CRC16(byte[] value, byte CH, byte CL, byte preH = byte.MaxValue, byte preL = byte.MaxValue)
	{
		byte[] buffer = new byte[value.Length + 2];
		value.CopyTo(buffer, 0);
		byte b = preL;
		byte b2 = preH;
		for (int i = 0; i < value.Length; i++)
		{
			b ^= value[i];
			for (int j = 0; j <= 7; j++)
			{
				byte b3 = b2;
				byte b4 = b;
				b2 >>= 1;
				b >>= 1;
				if ((b3 & 1) == 1)
				{
					b |= 0x80;
				}
				if ((b4 & 1) == 1)
				{
					b2 ^= CH;
					b ^= CL;
				}
			}
		}
		buffer[^2] = b;
		buffer[^1] = b2;
		return buffer;
	}

	/// <summary>
	/// 通过指定多项式码来获取对应的数据的CRC校验码
	/// The CRC check code of the corresponding data is obtained by specifying the polynomial code
	/// </summary>
	/// <param name="value">需要校验的数据，不包含CRC字节</param>
	/// <param name="index">计算的起始字节索引</param>
	/// <param name="length">计算的字节长度</param>
	/// <param name="CL">多项式码地位</param>
	/// <param name="CH">多项式码高位</param>
	/// <param name="preH">预置的高位值</param>
	/// <param name="preL">预置的低位值</param>
	/// <returns>返回带CRC校验码的字节数组，可用于串口发送</returns>
	public static byte[] CRC16Only(byte[] value, int index, int length, byte CH, byte CL, byte preH = byte.MaxValue, byte preL = byte.MaxValue)
	{
		byte b = preL;
		byte b2 = preH;
		for (int i = index; i < index + length; i++)
		{
			b ^= value[i];
			for (int j = 0; j <= 7; j++)
			{
				byte b3 = b2;
				byte b4 = b;
				b2 >>= 1;
				b >>= 1;
				if ((b3 & 1) == 1)
				{
					b |= 0x80;
				}
				if ((b4 & 1) == 1)
				{
					b2 ^= CH;
					b ^= CL;
				}
			}
		}
		return new byte[2] { b, b2 };
	}
}
