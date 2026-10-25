// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.IO;
using HslCommunication.BasicFramework;

namespace HslCommunication.Core;

/// <summary>
/// 所有数据转换类的静态辅助方法
/// Static helper method for all data conversion classes
/// </summary>
public static class ByteTransformHelper
{
	public static OperateResult<TResult> GetResultFromBytes<TResult>(OperateResult<byte[]> result, Func<byte[], TResult> translator)
	{
		try
		{
			if (result.IsSuccess)
			{
				return OperateResult.CreateSuccessResult(translator(result.Content));
			}
			return OperateResult.CreateFailedResult<TResult>(result);
		}
		catch (Exception ex)
		{
			OperateResult<TResult> operateResult = new OperateResult<TResult>();
			operateResult.Message = $"{StringResources.Language.DataTransformError} {SoftBasic.ByteToHexString(result.Content)} : Length({result.Content.Length}) {ex.Message}";
			return operateResult;
		}
	}

	public static OperateResult<TResult> GetResultFromArray<TResult>(OperateResult<TResult[]> result)
	{
		return GetSuccessResultFromOther(result, (TResult[] m) => m[0]);
	}

	public static OperateResult<TResult> GetSuccessResultFromOther<TResult, TIn>(OperateResult<TIn> result, Func<TIn, TResult> trans)
	{
		if (!result.IsSuccess)
		{
			return OperateResult.CreateFailedResult<TResult>(result);
		}
		try
		{
			TResult value = trans(result.Content);
			return OperateResult.CreateSuccessResult(value);
		}
		catch (Exception ex)
		{
			return new OperateResult<TResult>(StringResources.Language.DataTransformError + " " + ex.Message);
		}
	}

	public static OperateResult GetResultFromOther<TIn>(OperateResult<TIn> result, Func<TIn, OperateResult> trans)
	{
		if (!result.IsSuccess)
		{
			return result;
		}
		return trans(result.Content);
	}

	public static OperateResult<TResult> GetResultFromOther<TResult, TIn>(OperateResult<TIn> result, Func<TIn, OperateResult<TResult>> trans)
	{
		return result.Then(trans);
	}

	public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2>(OperateResult<TIn1> result, Func<TIn1, OperateResult<TIn2>> trans1, Func<TIn2, OperateResult<TResult>> trans2)
	{
		return result.Then(trans1).Then(trans2);
	}

	public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3>(OperateResult<TIn1> result, Func<TIn1, OperateResult<TIn2>> trans1, Func<TIn2, OperateResult<TIn3>> trans2, Func<TIn3, OperateResult<TResult>> trans3)
	{
		return result.Then(trans1).Then(trans2).Then(trans3);
	}

	public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4>(OperateResult<TIn1> result, Func<TIn1, OperateResult<TIn2>> trans1, Func<TIn2, OperateResult<TIn3>> trans2, Func<TIn3, OperateResult<TIn4>> trans3, Func<TIn4, OperateResult<TResult>> trans4)
	{
		return result.Then(trans1).Then(trans2).Then(trans3)
			.Then(trans4);
	}

	public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4, TIn5>(OperateResult<TIn1> result, Func<TIn1, OperateResult<TIn2>> trans1, Func<TIn2, OperateResult<TIn3>> trans2, Func<TIn3, OperateResult<TIn4>> trans3, Func<TIn4, OperateResult<TIn5>> trans4, Func<TIn5, OperateResult<TResult>> trans5)
	{
		return result.Then(trans1).Then(trans2).Then(trans3)
			.Then(trans4)
			.Then(trans5);
	}

	public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4, TIn5, TIn6>(OperateResult<TIn1> result, Func<TIn1, OperateResult<TIn2>> trans1, Func<TIn2, OperateResult<TIn3>> trans2, Func<TIn3, OperateResult<TIn4>> trans3, Func<TIn4, OperateResult<TIn5>> trans4, Func<TIn5, OperateResult<TIn6>> trans5, Func<TIn6, OperateResult<TResult>> trans6)
	{
		return result.Then(trans1).Then(trans2).Then(trans3)
			.Then(trans4)
			.Then(trans5)
			.Then(trans6);
	}

	public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TIn7>(OperateResult<TIn1> result, Func<TIn1, OperateResult<TIn2>> trans1, Func<TIn2, OperateResult<TIn3>> trans2, Func<TIn3, OperateResult<TIn4>> trans3, Func<TIn4, OperateResult<TIn5>> trans4, Func<TIn5, OperateResult<TIn6>> trans5, Func<TIn6, OperateResult<TIn7>> trans6, Func<TIn7, OperateResult<TResult>> trans7)
	{
		return result.Then(trans1).Then(trans2).Then(trans3)
			.Then(trans4)
			.Then(trans5)
			.Then(trans6)
			.Then(trans7);
	}

	public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TIn7, TIn8>(OperateResult<TIn1> result, Func<TIn1, OperateResult<TIn2>> trans1, Func<TIn2, OperateResult<TIn3>> trans2, Func<TIn3, OperateResult<TIn4>> trans3, Func<TIn4, OperateResult<TIn5>> trans4, Func<TIn5, OperateResult<TIn6>> trans5, Func<TIn6, OperateResult<TIn7>> trans6, Func<TIn7, OperateResult<TIn8>> trans7, Func<TIn8, OperateResult<TResult>> trans8)
	{
		return result.Then(trans1).Then(trans2).Then(trans3)
			.Then(trans4)
			.Then(trans5)
			.Then(trans6)
			.Then(trans7)
			.Then(trans8);
	}

	public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TIn7, TIn8, TIn9>(OperateResult<TIn1> result, Func<TIn1, OperateResult<TIn2>> trans1, Func<TIn2, OperateResult<TIn3>> trans2, Func<TIn3, OperateResult<TIn4>> trans3, Func<TIn4, OperateResult<TIn5>> trans4, Func<TIn5, OperateResult<TIn6>> trans5, Func<TIn6, OperateResult<TIn7>> trans6, Func<TIn7, OperateResult<TIn8>> trans7, Func<TIn8, OperateResult<TIn9>> trans8, Func<TIn9, OperateResult<TResult>> trans9)
	{
		return result.Then(trans1).Then(trans2).Then(trans3)
			.Then(trans4)
			.Then(trans5)
			.Then(trans6)
			.Then(trans7)
			.Then(trans8)
			.Then(trans9);
	}

	public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TIn7, TIn8, TIn9, TIn10>(OperateResult<TIn1> result, Func<TIn1, OperateResult<TIn2>> trans1, Func<TIn2, OperateResult<TIn3>> trans2, Func<TIn3, OperateResult<TIn4>> trans3, Func<TIn4, OperateResult<TIn5>> trans4, Func<TIn5, OperateResult<TIn6>> trans5, Func<TIn6, OperateResult<TIn7>> trans6, Func<TIn7, OperateResult<TIn8>> trans7, Func<TIn8, OperateResult<TIn9>> trans8, Func<TIn9, OperateResult<TIn10>> trans9, Func<TIn10, OperateResult<TResult>> trans10)
	{
		return result.Then(trans1).Then(trans2).Then(trans3)
			.Then(trans4)
			.Then(trans5)
			.Then(trans6)
			.Then(trans7)
			.Then(trans8)
			.Then(trans9)
			.Then(trans10);
	}

	/// <summary>
	/// 将多个SoftBuffer实例中的字节数组合并成一个MemoryStream
	/// Combines the byte arrays from multiple SoftBuffer instances into a single MemoryStream.
	/// </summary>
	/// <param name="softBuffers">An array of SoftBuffer instances whose bytes will be combined.</param>
	/// <returns>A MemoryStream containing the concatenated bytes from all provided SoftBuffer instances.</returns>
	public static MemoryStream GetByteFromSoftBuffer(params SoftBuffer[] softBuffers)
	{
		MemoryStream memoryStream = new MemoryStream();
		for (int i = 0; i < softBuffers.Length; i++)
		{
			memoryStream.Write(softBuffers[i].GetBytes());
		}
		return memoryStream;
	}

	/// <summary>
	/// 将一个字节数组的内容依次分配到多个SoftBuffer实例中
	/// Distributes the contents of a byte array into multiple SoftBuffer instances sequentially.
	/// </summary>
	/// <param name="buffer">The source byte array to be distributed.</param>
	/// <param name="startIndex">起始偏移的字节索引</param>
	/// <param name="softBuffers">The target SoftBuffer instances to receive the byte data.</param>
	public static void SetByteToSoftBuffer(byte[] buffer, int startIndex, params SoftBuffer[] softBuffers)
	{
		if (buffer != null && buffer.Length != 0)
		{
			int num = startIndex;
			for (int i = 0; i < softBuffers.Length && num + softBuffers[i].Capacity <= buffer.Length; i++)
			{
				softBuffers[i].SetBytes(buffer, num, 0, softBuffers[i].Capacity);
				num += softBuffers[i].Capacity;
			}
		}
	}
}
