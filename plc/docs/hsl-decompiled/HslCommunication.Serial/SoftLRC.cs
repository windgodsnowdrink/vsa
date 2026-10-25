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

namespace HslCommunication.Serial;

/// <summary>
/// 用于LRC验证的类，提供了标准的验证方法
/// The class used for LRC verification provides a standard verification method
/// </summary>
public class SoftLRC
{
	/// <summary>
	/// 获取对应的数据的LRC校验码
	/// Class for LRC validation that provides a standard validation method
	/// </summary>
	/// <param name="value">需要校验的数据，不包含LRC字节</param>
	/// <returns>返回带LRC校验码的字节数组，可用于串口发送</returns>
	public static byte[] LRC(byte[] value)
	{
		if (value == null)
		{
			return null;
		}
		int num = 0;
		for (int i = 0; i < value.Length; i++)
		{
			num += value[i];
		}
		num %= 256;
		num = 256 - num;
		byte[] buffer = new byte[1] { (byte)num };
		return SoftBasic.SpliceArray<byte>(value, buffer);
	}

	/// <summary>
	/// 获取对应的数据的LRC校验码
	/// Class for LRC validation that provides a standard validation method
	/// </summary>
	/// <param name="value">需要校验的数据，不包含LRC字节</param>
	/// <param name="left">忽略的左边的字节数量</param>
	/// <param name="right">忽略的右边的字节数量</param>
	/// <returns>返回LRC校验码</returns>
	public static byte LRC(byte[] value, int left, int right)
	{
		int num = 0;
		for (int i = left; i < value.Length - right; i++)
		{
			num += value[i];
		}
		num %= 256;
		num = 256 - num;
		return (byte)num;
	}

	/// <summary>
	/// 检查数据是否符合LRC的验证
	/// Check data for compliance with LRC validation
	/// </summary>
	/// <param name="value">等待校验的数据，是否正确</param>
	/// <returns>是否校验成功</returns>
	public static bool CheckLRC(byte[] value)
	{
		if (value == null)
		{
			return false;
		}
		int count = value.Length;
		byte[] buffer = new byte[count - 1];
		Array.Copy(value, 0, buffer, 0, buffer.Length);
		byte[] array2 = LRC(buffer);
		if (array2[count - 1] == value[count - 1])
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// 根据传入的原始字节数组，计算和校验信息，可以指定起始的偏移地址和尾部的字节数量信息
	/// Calculate and check the information according to the incoming original byte array, you can specify the starting offset address and the number of bytes at the end
	/// </summary>
	/// <param name="buffer">原始字节数组信息</param>
	/// <param name="headCount">起始的偏移地址信息</param>
	/// <param name="lastCount">尾部的字节数量信息</param>
	/// <returns>和校验的结果</returns>
	public static int CalculateAcc(byte[] buffer)
	{
		return CalculateAcc(buffer, 0, 0);
	}

	/// <summary>
	/// 根据传入的原始字节数组，计算和校验信息，可以指定起始的偏移地址和尾部的字节数量信息
	/// Calculate and check the information according to the incoming original byte array, you can specify the starting offset address and the number of bytes at the end
	/// </summary>
	/// <param name="buffer">原始字节数组信息</param>
	/// <param name="headCount">起始的偏移地址信息</param>
	/// <param name="lastCount">尾部的字节数量信息</param>
	/// <returns>和校验的结果</returns>
	public static int CalculateAcc(byte[] buffer, int headCount, int lastCount)
	{
		int num = 0;
		for (int i = headCount; i < buffer.Length - lastCount; i++)
		{
			num += buffer[i];
		}
		return num;
	}

	/// <summary>
	/// 计算数据的和校验，并且输入和校验的值信息
	/// Calculate the sum check of the data, and enter the value information of the sum check
	/// </summary>
	/// <param name="buffer">原始字节数组信息</param>
	/// <param name="headCount">起始的偏移地址信息</param>
	/// <param name="lastCount">尾部的字节数量信息</param>
	public static void CalculateAccAndFill(byte[] buffer, int headCount, int lastCount)
	{
		byte b = (byte)CalculateAcc(buffer, headCount, lastCount);
		Encoding.ASCII.GetBytes(b.ToString("X2")).CopyTo(buffer, buffer.Length - lastCount);
	}

	/// <summary>
	/// 计算数据的和校验，并且和当前已经存在的和校验信息进行匹配，返回是否匹配成功
	/// Calculate the sum check of the data, and match it with the existing sum check information, and return whether the match is successful
	/// </summary>
	/// <param name="buffer">原始字节数组信息</param>
	/// <param name="headCount">起始的偏移地址信息</param>
	/// <param name="lastCount">尾部的字节数量信息</param>
	/// <returns>和校验是否检查通过</returns>
	public static bool CalculateAccAndCheck(byte[] buffer, int headCount, int lastCount)
	{
		return ((byte)CalculateAcc(buffer, headCount, lastCount)).ToString("X2") == Encoding.ASCII.GetString(buffer, buffer.Length - lastCount, 2);
	}

	/// <summary>
	/// 计算数据的异或信息，也称为 FCS，可以指定前面无用的字节数量，以及尾部无用的字节数量
	/// </summary>
	/// <param name="source">数据源信息</param>
	/// <param name="left">前面无用的字节数量</param>
	/// <param name="right">后面无用的字节数量</param>
	/// <returns>返回异或校验后的值</returns>
	public static byte[] CalculateFcs(byte[] source, int left, int right)
	{
		int num = source[left];
		for (int i = left + 1; i < source.Length - right; i++)
		{
			num ^= source[i];
		}
		return new byte[2]
		{
			SoftBasic.BuildAsciiBytesFrom((byte)num)[0],
			SoftBasic.BuildAsciiBytesFrom((byte)num)[1]
		};
	}
}
