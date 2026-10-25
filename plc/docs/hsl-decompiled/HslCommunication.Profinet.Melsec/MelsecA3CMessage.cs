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
using System.Text;
using HslCommunication.Core.IMessage;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// Melsec A3C协议的消息类，主要用于数据接收的判断，具体的格式请参考三菱的A3C协议文档
/// </summary>
public class MelsecA3CMessage : NetMessageBase, INetMessage
{
	/// <summary>
	/// 当前的A3C协议的格式信息，可选格式1，2，3，4，默认格式1
	/// Format information of the current A3C protocol, optional format 1, 2, 3, 4, default format 1
	/// </summary>
	public int Format { get; set; } = 1;

	/// <summary>
	/// 当前的A3C协议是否使用和校验，默认使用
	/// Whether the current A3C protocol uses sum check, it is used by default
	/// </summary>
	public bool SumCheck { get; set; } = true;

	public int ProtocolHeadBytesLength => -1;

	/// <summary>
	/// 从当前的头子节文件中提取出接下来需要接收的数据长度
	/// Extract the length of the data to be received from the current header file
	/// </summary>
	/// <returns>返回接下来的数据内容长度</returns>
	/// <remarks>
	/// 如果剩余字节的长度小于0，则表示消息头数据还没有接收完整，还需要接收一定的长度(返回值的绝对值)，然后再判断剩余字节长度是否小于0，直到结果大于等于0为止，最多判断的次数为16次，超过16次将返回失败
	/// If the length of the remaining bytes is less than 0, it means that the message header data has not been received completely, and a certain length (absolute value of the return value) needs to be received,
	/// and then it is judged whether the length of the remaining bytes is less than 0 until the result is greater than or equal to 0,
	/// the maximum number of judgments is 16, more than 16 times will return failure
	/// </remarks>
	public int GetContentLengthByHeadBytes()
	{
		return 0;
	}

	/// <summary>
	/// 当消息头报文的长度定义为-1的时候，则使用动态的长度信息，可以使用本方法来判断一个消息是否处于完整的状态。
	/// If the length of the message header is defined as -1, this method can be used to determine whether a message is in the complete state by using dynamic length information.
	/// </summary>
	/// <param name="send">发送的消息</param>
	/// <param name="ms">接收到的数据内容</param>
	/// <returns>是否是完整的消息</returns>
	public override bool CheckReceiveDataComplete(byte[] send, MemoryStream ms)
	{
		byte[] array = ms.ToArray();
		if (array.Length < 5)
		{
			return false;
		}
		if (Format == 1)
		{
			if (array[0] == 2)
			{
				if (SumCheck)
				{
					return array[^3] == 3;
				}
				return array[^1] == 3;
			}
			if (array[0] == 21)
			{
				return array.Length >= 15;
			}
			if (array[0] == 6)
			{
				return array.Length >= 11;
			}
			return false;
		}
		if (Format == 2)
		{
			if (array[0] == 2)
			{
				if (SumCheck)
				{
					return array[^3] == 3;
				}
				return array[^1] == 3;
			}
			if (array[0] == 21)
			{
				return array.Length >= 17;
			}
			if (array[0] == 6)
			{
				return array.Length >= 13;
			}
			return false;
		}
		if (Format == 3)
		{
			if (array[0] != 2)
			{
				return false;
			}
			if (array.Length < 15)
			{
				return false;
			}
			string message = Encoding.ASCII.GetString(array, 11, 4);
			if (message == "QNAK")
			{
				return array.Length >= 20;
			}
			if (message == "QACK")
			{
				if (!SumCheck)
				{
					return array[^1] == 3;
				}
				if (array.Length == 16 && array[15] == 3)
				{
					return true;
				}
				if (array[^3] == 3)
				{
					return true;
				}
			}
			return false;
		}
		if (Format == 4)
		{
			if (array[0] == 2)
			{
				return array[^2] == 13 && array[^1] == 10;
			}
			if (array[0] == 21)
			{
				return array.Length >= 17;
			}
			if (array[0] == 6)
			{
				return array.Length >= 11;
			}
			return false;
		}
		if (Format == 5)
		{
			if (array[0] != 16)
			{
				return false;
			}
			if (array[1] != 2)
			{
				return false;
			}
			int num = BitConverter.ToUInt16(array, 2);
			if (SumCheck)
			{
				return array.Length >= num + 8;
			}
			return array.Length >= num + 6;
		}
		return true;
	}
}
