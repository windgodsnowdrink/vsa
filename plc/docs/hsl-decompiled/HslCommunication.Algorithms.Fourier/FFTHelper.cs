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

namespace HslCommunication.Algorithms.Fourier;

/// <summary>
/// 离散傅氏变换的快速算法，处理的信号，适合单周期信号数为2的N次方个，支持变换及逆变换
/// </summary>
public class FFTHelper
{
	private static void bitrp(double[] xreal, double[] ximag, int n)
	{
		int num = 1;
		int num2 = 0;
		while (num < n)
		{
			num2++;
			num *= 2;
		}
		for (num = 0; num < n; num++)
		{
			int num3 = num;
			int num4 = 0;
			for (int i = 0; i < num2; i++)
			{
				num4 = num4 * 2 + num3 % 2;
				num3 /= 2;
			}
			if (num4 > num)
			{
				double num5 = xreal[num];
				xreal[num] = xreal[num4];
				xreal[num4] = num5;
				num5 = ximag[num];
				ximag[num] = ximag[num4];
				ximag[num4] = num5;
			}
		}
	}

	/// <summary>
	/// 快速傅立叶变换
	/// </summary>
	/// <param name="xreal">实数部分</param>
	/// <returns>变换后的数组值</returns>
	public static double[] FFT(double[] xreal)
	{
		return FFTValue(xreal, new double[xreal.Length]);
	}

	/// <summary>
	/// 快速傅立叶变换
	/// </summary>
	/// <param name="xreal">实数部分，数组长度最好为2的n次方</param>
	/// <param name="ximag">虚数部分，数组长度最好为2的n次方</param>
	/// <param name="isSqrtDouble">是否开两次根，显示的噪点信息会更新明显</param>
	/// <returns>变换后的数组值</returns>
	public static double[] FFTValue(double[] xreal, double[] ximag, bool isSqrtDouble = false)
	{
		int num;
		for (num = 2; num <= xreal.Length; num *= 2)
		{
		}
		num /= 2;
		double[] array = new double[num / 2];
		double[] array2 = new double[num / 2];
		bitrp(xreal, ximag, num);
		double num2 = Math.PI * -2.0 / (double)num;
		double num3 = Math.Cos(num2);
		double num4 = Math.Sin(num2);
		array[0] = 1.0;
		array2[0] = 0.0;
		for (int i = 1; i < num / 2; i++)
		{
			array[i] = array[i - 1] * num3 - array2[i - 1] * num4;
			array2[i] = array[i - 1] * num4 + array2[i - 1] * num3;
		}
		for (int num5 = 2; num5 <= num; num5 *= 2)
		{
			for (int j = 0; j < num; j += num5)
			{
				for (int i = 0; i < num5 / 2; i++)
				{
					int num6 = j + i;
					int num7 = num6 + num5 / 2;
					int num8 = num * i / num5;
					num3 = array[num8] * xreal[num7] - array2[num8] * ximag[num7];
					num4 = array[num8] * ximag[num7] + array2[num8] * xreal[num7];
					double num9 = xreal[num6];
					double num10 = ximag[num6];
					xreal[num6] = num9 + num3;
					ximag[num6] = num10 + num4;
					xreal[num7] = num9 - num3;
					ximag[num7] = num10 - num4;
				}
			}
		}
		double[] array3 = new double[num];
		for (int k = 0; k < array3.Length; k++)
		{
			array3[k] = Math.Sqrt(Math.Pow(xreal[k], 2.0) + Math.Pow(ximag[k], 2.0));
			if (isSqrtDouble)
			{
				array3[k] = Math.Sqrt(array3[k]);
			}
		}
		return array3;
	}

	/// <summary>
	/// 快速傅立叶变换
	/// </summary>
	/// <param name="xreal">实数部分，数组长度最好为2的n次方</param>
	/// <param name="ximag">虚数部分，数组长度最好为2的n次方</param>
	/// <returns>变换后的数组值</returns>
	public static int FFT(double[] xreal, double[] ximag)
	{
		return FFTValue(xreal, ximag).Length;
	}

	/// <summary>
	/// 快速傅立叶变换
	/// </summary>
	/// <param name="xreal">实数部分，数组长度最好为2的n次方</param>
	/// <param name="ximag">虚数部分，数组长度最好为2的n次方</param>
	/// <returns>变换后的数组值</returns>
	public static int FFT(float[] xreal, float[] ximag)
	{
		return FFT(((IEnumerable<float>)xreal).Select((Func<float, double>)((float m) => m)).ToArray(), ((IEnumerable<float>)ximag).Select((Func<float, double>)((float m) => m)).ToArray());
	}

	/// <summary>
	/// 快速傅立叶变换的逆变换
	/// </summary>
	/// <param name="xreal">实数部分，数组长度最好为2的n次方</param>
	/// <param name="ximag">虚数部分，数组长度最好为2的n次方</param>
	/// <returns>2的多少次方</returns>
	public static int IFFT(float[] xreal, float[] ximag)
	{
		return IFFT(((IEnumerable<float>)xreal).Select((Func<float, double>)((float m) => m)).ToArray(), ((IEnumerable<float>)ximag).Select((Func<float, double>)((float m) => m)).ToArray());
	}

	/// <summary>
	/// 快速傅立叶变换的逆变换
	/// </summary>
	/// <param name="xreal">实数部分，数组长度最好为2的n次方</param>
	/// <param name="ximag">虚数部分，数组长度最好为2的n次方</param>
	/// <returns>2的多少次方</returns>
	public static int IFFT(double[] xreal, double[] ximag)
	{
		int num;
		for (num = 2; num <= xreal.Length; num *= 2)
		{
		}
		num /= 2;
		double[] array = new double[num / 2];
		double[] array2 = new double[num / 2];
		bitrp(xreal, ximag, num);
		double num2 = Math.PI * 2.0 / (double)num;
		double num3 = Math.Cos(num2);
		double num4 = Math.Sin(num2);
		array[0] = 1.0;
		array2[0] = 0.0;
		for (int i = 1; i < num / 2; i++)
		{
			array[i] = array[i - 1] * num3 - array2[i - 1] * num4;
			array2[i] = array[i - 1] * num4 + array2[i - 1] * num3;
		}
		for (int num5 = 2; num5 <= num; num5 *= 2)
		{
			for (int j = 0; j < num; j += num5)
			{
				for (int i = 0; i < num5 / 2; i++)
				{
					int num6 = j + i;
					int num7 = num6 + num5 / 2;
					int num8 = num * i / num5;
					num3 = array[num8] * xreal[num7] - array2[num8] * ximag[num7];
					num4 = array[num8] * ximag[num7] + array2[num8] * xreal[num7];
					double num9 = xreal[num6];
					double num10 = ximag[num6];
					xreal[num6] = num9 + num3;
					ximag[num6] = num10 + num4;
					xreal[num7] = num9 - num3;
					ximag[num7] = num10 - num4;
				}
			}
		}
		for (int i = 0; i < num; i++)
		{
			xreal[i] /= num;
			ximag[i] /= num;
		}
		return num;
	}
}
