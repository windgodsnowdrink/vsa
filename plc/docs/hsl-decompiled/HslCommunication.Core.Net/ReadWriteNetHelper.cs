// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using HslCommunication.Reflection;

namespace HslCommunication.Core.Net;

/// <summary>
/// 读写网络的辅助类
/// </summary>
public class ReadWriteNetHelper
{
	/// <summary>
	/// 写入位到字寄存器的功能，该功能先读取字寄存器的字数据，然后修改其中的位，再写入回去，可能存在脏数据的风险
	/// The function of writing bit-to-word registers, which first reads the word data of the word register, then modifies the bits in it, and then writes back, which may be the risk of dirty data
	/// </summary>
	/// <param name="readWrite">通信对象信息</param>
	/// <param name="address">写入的地址信息，需要携带&apos;.&apos;号</param>
	/// <param name="values">写入的值信息</param>
	/// <param name="addLength">多少长度的bit位组成一个字地址信息</param>
	/// <param name="reverseWord">对原始数据是否按照字单位进行反转操作</param>
	/// <param name="bitStr">额外指定的位索引，如果为空，则使用中的地址位偏移信息</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// 关于脏数据风险：从读取数据，修改位，再次写入数据时，大概需要经过3ms~10ms不等的时间，如果此期间内PLC修改了该字寄存器的其他位，再次写入数据时会恢复该点位的数据到读取时的初始值，可能引发设备故障，请谨慎开启此功能。
	/// </remarks>
	public static OperateResult WriteBoolWithWord(IReadWriteNet readWrite, string address, bool[] values, int addLength = 16, bool reverseWord = false, string bitStr = null)
	{
		string[] array = address.SplitDot();
		int num = 0;
		try
		{
			if (string.IsNullOrEmpty(bitStr))
			{
				if (array.Length > 1)
				{
					num = Convert.ToInt32(array[1]);
				}
			}
			else
			{
				num = Convert.ToInt32(bitStr);
			}
		}
		catch (Exception ex)
		{
			return new OperateResult(address + " Bit index input wrong: " + ex.Message);
		}
		ushort length = (ushort)((num + values.Length + addLength - 1) / addLength);
		OperateResult<byte[]> operateResult = readWrite.Read(array[0], length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		bool[] array2 = (reverseWord ? operateResult.Content.ReverseByWord().ToBoolArray() : operateResult.Content.ToBoolArray());
		if (num + values.Length <= array2.Length)
		{
			values.CopyTo(array2, num);
		}
		return readWrite.Write(array[0], reverseWord ? array2.ToByteArray().ReverseByWord() : array2.ToByteArray());
	}

	/// <summary>
	/// 写入位到字寄存器的功能，该功能先读取字寄存器的字数据，然后修改其中的位，再写入回去，可能存在脏数据的风险
	/// The function of writing bit-to-word registers, which first reads the word data of the word register, then modifies the bits in it, and then writes back, which may be the risk of dirty data
	/// </summary>
	/// <param name="readWrite">通信对象信息</param>
	/// <param name="address">写入的地址信息，需要携带&apos;.&apos;号</param>
	/// <param name="values">写入的值信息</param>
	/// <param name="addLength">多少长度的bit位组成一个字地址信息</param>
	/// <param name="reverseWord">对原始数据是否按照字单位进行反转操作</param>
	/// <param name="bitStr">额外指定的位索引，如果为空，则使用中的地址位偏移信息</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// 关于脏数据风险：从读取数据，修改位，再次写入数据时，大概需要经过3ms~10ms不等的时间，如果此期间内PLC修改了该字寄存器的其他位，再次写入数据时会恢复该点位的数据到读取时的初始值，可能引发设备故障，请谨慎开启此功能。
	/// </remarks>
	public static async Task<OperateResult> WriteBoolWithWordAsync(IReadWriteNet readWrite, string address, bool[] values, int addLength = 16, bool reverseWord = false, string bitStr = null)
	{
		string[] adds = address.SplitDot();
		int bit = 0;
		try
		{
			if (string.IsNullOrEmpty(bitStr))
			{
				if (adds.Length > 1)
				{
					bit = Convert.ToInt32(adds[1]);
				}
			}
			else
			{
				bit = Convert.ToInt32(bitStr);
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			return new OperateResult(address + " Bit index input wrong: " + ex2.Message);
		}
		OperateResult<byte[]> read = await readWrite.ReadAsync(length: (ushort)((bit + values.Length + addLength - 1) / addLength), address: adds[0]);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(read);
		}
		bool[] array = (reverseWord ? read.Content.ReverseByWord().ToBoolArray() : read.Content.ToBoolArray());
		if (bit + values.Length <= array.Length)
		{
			values.CopyTo(array, bit);
		}
		return await readWrite.WriteAsync(adds[0], reverseWord ? array.ToByteArray().ReverseByWord() : array.ToByteArray());
	}

	/// <param name="readWriteNet">通信对象</param>
	public static OperateResult<TimeSpan> Wait(IReadWriteNet readWriteNet, string address, bool waitValue, int readInterval, int waitTimeout)
	{
		DateTime now = DateTime.Now;
		while (true)
		{
			OperateResult<bool> operateResult = readWriteNet.ReadBool(address);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<TimeSpan>(operateResult);
			}
			if (operateResult.Content == waitValue)
			{
				return OperateResult.CreateSuccessResult(DateTime.Now - now);
			}
			if (waitTimeout > 0 && (DateTime.Now - now).TotalMilliseconds > (double)waitTimeout)
			{
				break;
			}
			HslHelper.ThreadSleep(readInterval);
		}
		return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout);
	}

	/// <param name="readWriteNet">通信对象</param>
	public static OperateResult<TimeSpan> Wait(IReadWriteNet readWriteNet, string address, short waitValue, int readInterval, int waitTimeout)
	{
		DateTime now = DateTime.Now;
		while (true)
		{
			OperateResult<short> operateResult = readWriteNet.ReadInt16(address);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<TimeSpan>(operateResult);
			}
			if (operateResult.Content == waitValue)
			{
				return OperateResult.CreateSuccessResult(DateTime.Now - now);
			}
			if (waitTimeout > 0 && (DateTime.Now - now).TotalMilliseconds > (double)waitTimeout)
			{
				break;
			}
			HslHelper.ThreadSleep(readInterval);
		}
		return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout);
	}

	/// <param name="readWriteNet">通信对象</param>
	public static OperateResult<TimeSpan> Wait(IReadWriteNet readWriteNet, string address, ushort waitValue, int readInterval, int waitTimeout)
	{
		DateTime now = DateTime.Now;
		while (true)
		{
			OperateResult<ushort> operateResult = readWriteNet.ReadUInt16(address);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<TimeSpan>(operateResult);
			}
			if (operateResult.Content == waitValue)
			{
				return OperateResult.CreateSuccessResult(DateTime.Now - now);
			}
			if (waitTimeout > 0 && (DateTime.Now - now).TotalMilliseconds > (double)waitTimeout)
			{
				break;
			}
			HslHelper.ThreadSleep(readInterval);
		}
		return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout);
	}

	/// <param name="readWriteNet">通信对象</param>
	public static OperateResult<TimeSpan> Wait(IReadWriteNet readWriteNet, string address, int waitValue, int readInterval, int waitTimeout)
	{
		DateTime now = DateTime.Now;
		while (true)
		{
			OperateResult<int> operateResult = readWriteNet.ReadInt32(address);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<TimeSpan>(operateResult);
			}
			if (operateResult.Content == waitValue)
			{
				return OperateResult.CreateSuccessResult(DateTime.Now - now);
			}
			if (waitTimeout > 0 && (DateTime.Now - now).TotalMilliseconds > (double)waitTimeout)
			{
				break;
			}
			HslHelper.ThreadSleep(readInterval);
		}
		return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout);
	}

	/// <param name="readWriteNet">通信对象</param>
	public static OperateResult<TimeSpan> Wait(IReadWriteNet readWriteNet, string address, uint waitValue, int readInterval, int waitTimeout)
	{
		DateTime now = DateTime.Now;
		while (true)
		{
			OperateResult<uint> operateResult = readWriteNet.ReadUInt32(address);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<TimeSpan>(operateResult);
			}
			if (operateResult.Content == waitValue)
			{
				return OperateResult.CreateSuccessResult(DateTime.Now - now);
			}
			if (waitTimeout > 0 && (DateTime.Now - now).TotalMilliseconds > (double)waitTimeout)
			{
				break;
			}
			HslHelper.ThreadSleep(readInterval);
		}
		return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout);
	}

	/// <param name="readWriteNet">通信对象</param>
	public static OperateResult<TimeSpan> Wait(IReadWriteNet readWriteNet, string address, long waitValue, int readInterval, int waitTimeout)
	{
		DateTime now = DateTime.Now;
		while (true)
		{
			OperateResult<long> operateResult = readWriteNet.ReadInt64(address);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<TimeSpan>(operateResult);
			}
			if (operateResult.Content == waitValue)
			{
				return OperateResult.CreateSuccessResult(DateTime.Now - now);
			}
			if (waitTimeout > 0 && (DateTime.Now - now).TotalMilliseconds > (double)waitTimeout)
			{
				break;
			}
			HslHelper.ThreadSleep(readInterval);
		}
		return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout);
	}

	/// <param name="readWriteNet">通信对象</param>
	public static OperateResult<TimeSpan> Wait(IReadWriteNet readWriteNet, string address, ulong waitValue, int readInterval, int waitTimeout)
	{
		DateTime now = DateTime.Now;
		while (true)
		{
			OperateResult<ulong> operateResult = readWriteNet.ReadUInt64(address);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<TimeSpan>(operateResult);
			}
			if (operateResult.Content == waitValue)
			{
				return OperateResult.CreateSuccessResult(DateTime.Now - now);
			}
			if (waitTimeout > 0 && (DateTime.Now - now).TotalMilliseconds > (double)waitTimeout)
			{
				break;
			}
			HslHelper.ThreadSleep(readInterval);
		}
		return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout);
	}

	public static OperateResult<T> ReadCustomer<T>(IReadWriteNet readWriteNet, string address) where T : IDataTransfer, new()
	{
		T obj = new T();
		return ReadCustomer(readWriteNet, address, obj);
	}

	public static OperateResult<T> ReadCustomer<T>(IReadWriteNet readWriteNet, string address, T obj) where T : IDataTransfer, new()
	{
		OperateResult<byte[]> operateResult = readWriteNet.Read(address, obj.ReadCount);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T>(operateResult);
		}
		obj.ParseSource(operateResult.Content);
		return OperateResult.CreateSuccessResult(obj);
	}

	public static OperateResult WriteCustomer<T>(IReadWriteNet readWriteNet, string address, T data) where T : IDataTransfer, new()
	{
		return readWriteNet.Write(address, data.ToSource());
	}

	public static async Task<OperateResult<T>> ReadCustomerAsync<T>(IReadWriteNet readWriteNet, string address) where T : IDataTransfer, new()
	{
		T Content = new T();
		return await ReadCustomerAsync(readWriteNet, address, Content);
	}

	public static async Task<OperateResult<T>> ReadCustomerAsync<T>(IReadWriteNet readWriteNet, string address, T obj) where T : IDataTransfer, new()
	{
		OperateResult<byte[]> read = await readWriteNet.ReadAsync(address, obj.ReadCount);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T>(read);
		}
		obj.ParseSource(read.Content);
		return OperateResult.CreateSuccessResult(obj);
	}

	public static async Task<OperateResult> WriteCustomerAsync<T>(IReadWriteNet readWriteNet, string address, T data) where T : IDataTransfer, new()
	{
		return await readWriteNet.WriteAsync(address, data.ToSource());
	}

	public static OperateResult<T> ReadStruct<T>(IReadWriteNet readWriteNet, string address, ushort length, IByteTransform byteTransform, int startIndex = 0) where T : class, new()
	{
		OperateResult<byte[]> operateResult = readWriteNet.Read(address, length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T>(operateResult);
		}
		try
		{
			return OperateResult.CreateSuccessResult(HslReflectionHelper.PraseStructContent<T>(operateResult.Content, startIndex, byteTransform));
		}
		catch (Exception ex)
		{
			return new OperateResult<T>("Prase struct faild: " + ex.Message + Environment.NewLine + "Source Data: " + operateResult.Content.ToHexString(' '));
		}
	}

	public static async Task<OperateResult<T>> ReadStructAsync<T>(IReadWriteNet readWriteNet, string address, ushort length, IByteTransform byteTransform, int startIndex = 0) where T : class, new()
	{
		OperateResult<byte[]> read = await readWriteNet.ReadAsync(address, length);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T>(read);
		}
		try
		{
			return OperateResult.CreateSuccessResult(HslReflectionHelper.PraseStructContent<T>(read.Content, startIndex, byteTransform));
		}
		catch (Exception ex)
		{
			return new OperateResult<T>("Prase struct faild: " + ex.Message + Environment.NewLine + "Source Data: " + read.Content.ToHexString(' '));
		}
	}

	/// <param name="readWriteNet">通信对象</param>
	public static async Task<OperateResult<TimeSpan>> WaitAsync(IReadWriteNet readWriteNet, string address, bool waitValue, int readInterval, int waitTimeout)
	{
		DateTime start = DateTime.Now;
		while (true)
		{
			OperateResult<bool> read = await readWriteNet.ReadBoolAsync(address);
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<TimeSpan>(read);
			}
			if (read.Content == waitValue)
			{
				return OperateResult.CreateSuccessResult(DateTime.Now - start);
			}
			if (waitTimeout > 0 && (DateTime.Now - start).TotalMilliseconds > (double)waitTimeout)
			{
				break;
			}
			await Task.Delay(readInterval);
		}
		return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout);
	}

	/// <param name="readWriteNet">通信对象</param>
	public static async Task<OperateResult<TimeSpan>> WaitAsync(IReadWriteNet readWriteNet, string address, short waitValue, int readInterval, int waitTimeout)
	{
		DateTime start = DateTime.Now;
		while (true)
		{
			OperateResult<short> read = await readWriteNet.ReadInt16Async(address);
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<TimeSpan>(read);
			}
			if (read.Content == waitValue)
			{
				return OperateResult.CreateSuccessResult(DateTime.Now - start);
			}
			if (waitTimeout > 0 && (DateTime.Now - start).TotalMilliseconds > (double)waitTimeout)
			{
				break;
			}
			await Task.Delay(readInterval);
		}
		return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout);
	}

	/// <param name="readWriteNet">通信对象</param>
	public static async Task<OperateResult<TimeSpan>> WaitAsync(IReadWriteNet readWriteNet, string address, ushort waitValue, int readInterval, int waitTimeout)
	{
		DateTime start = DateTime.Now;
		while (true)
		{
			OperateResult<ushort> read = await readWriteNet.ReadUInt16Async(address);
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<TimeSpan>(read);
			}
			if (read.Content == waitValue)
			{
				return OperateResult.CreateSuccessResult(DateTime.Now - start);
			}
			if (waitTimeout > 0 && (DateTime.Now - start).TotalMilliseconds > (double)waitTimeout)
			{
				break;
			}
			await Task.Delay(readInterval);
		}
		return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout);
	}

	/// <param name="readWriteNet">通信对象</param>
	public static async Task<OperateResult<TimeSpan>> WaitAsync(IReadWriteNet readWriteNet, string address, int waitValue, int readInterval, int waitTimeout)
	{
		DateTime start = DateTime.Now;
		while (true)
		{
			OperateResult<int> read = await readWriteNet.ReadInt32Async(address);
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<TimeSpan>(read);
			}
			if (read.Content == waitValue)
			{
				return OperateResult.CreateSuccessResult(DateTime.Now - start);
			}
			if (waitTimeout > 0 && (DateTime.Now - start).TotalMilliseconds > (double)waitTimeout)
			{
				break;
			}
			await Task.Delay(readInterval);
		}
		return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout);
	}

	/// <param name="readWriteNet">通信对象</param>
	public static async Task<OperateResult<TimeSpan>> WaitAsync(IReadWriteNet readWriteNet, string address, uint waitValue, int readInterval, int waitTimeout)
	{
		DateTime start = DateTime.Now;
		while (true)
		{
			OperateResult<uint> read = readWriteNet.ReadUInt32(address);
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<TimeSpan>(read);
			}
			if (read.Content == waitValue)
			{
				return OperateResult.CreateSuccessResult(DateTime.Now - start);
			}
			if (waitTimeout > 0 && (DateTime.Now - start).TotalMilliseconds > (double)waitTimeout)
			{
				break;
			}
			await Task.Delay(readInterval);
		}
		return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout);
	}

	/// <param name="readWriteNet">通信对象</param>
	public static async Task<OperateResult<TimeSpan>> WaitAsync(IReadWriteNet readWriteNet, string address, long waitValue, int readInterval, int waitTimeout)
	{
		DateTime start = DateTime.Now;
		while (true)
		{
			OperateResult<long> read = readWriteNet.ReadInt64(address);
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<TimeSpan>(read);
			}
			if (read.Content == waitValue)
			{
				return OperateResult.CreateSuccessResult(DateTime.Now - start);
			}
			if (waitTimeout > 0 && (DateTime.Now - start).TotalMilliseconds > (double)waitTimeout)
			{
				break;
			}
			await Task.Delay(readInterval);
		}
		return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout);
	}

	/// <param name="readWriteNet">通信对象</param>
	public static async Task<OperateResult<TimeSpan>> WaitAsync(IReadWriteNet readWriteNet, string address, ulong waitValue, int readInterval, int waitTimeout)
	{
		DateTime start = DateTime.Now;
		while (true)
		{
			OperateResult<ulong> read = readWriteNet.ReadUInt64(address);
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<TimeSpan>(read);
			}
			if (read.Content == waitValue)
			{
				return OperateResult.CreateSuccessResult(DateTime.Now - start);
			}
			if (waitTimeout > 0 && (DateTime.Now - start).TotalMilliseconds > (double)waitTimeout)
			{
				break;
			}
			await Task.Delay(readInterval);
		}
		return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout);
	}
}
