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

internal class MelsecA3CServerMessage : NetMessageBase, INetMessage
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

	protected bool CalculateMcLengthComplete(byte[] receive, int startIndex, int tail = 0)
	{
		return CheckServerMcCommandComplete(receive, receive.Length, startIndex, SumCheck, tail);
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
		return CheckServer3CCommandComplete(array, Format, array.Length, SumCheck);
	}

	public static bool CheckServer3CCommandComplete(byte[] receive, int format, int actualLength, bool sumCheck)
	{
		if (receive.Length < 5)
		{
			return false;
		}
		switch (format)
		{
		case 1:
			if (receive[0] == 5)
			{
				return CheckServerMcCommandComplete(receive, actualLength, 11, sumCheck);
			}
			break;
		case 2:
			if (receive[0] == 5)
			{
				return CheckServerMcCommandComplete(receive, actualLength, 13, sumCheck);
			}
			break;
		case 3:
			if (receive[0] == 2)
			{
				return CheckServerMcCommandComplete(receive, actualLength, 11, sumCheck, 1);
			}
			break;
		case 4:
			if (receive[0] == 5)
			{
				return CheckServerMcCommandComplete(receive, actualLength, 11, sumCheck, 2);
			}
			break;
		}
		return true;
	}

	public static bool CheckServerMcCommandComplete(byte[] receive, int actualLength, int startIndex, bool sumCheck, int tail = 0)
	{
		int num = -1;
		if (actualLength < startIndex + 4)
		{
			return false;
		}
		if (Encoding.ASCII.GetString(receive, startIndex, 4) == "0401")
		{
			num = 20;
		}
		else if (Encoding.ASCII.GetString(receive, startIndex, 4) == "1401")
		{
			if (actualLength < startIndex + 20)
			{
				return false;
			}
			string message = Encoding.ASCII.GetString(receive, startIndex + 4, 4);
			if (message == "0001")
			{
				num = 20 + Convert.ToInt32(Encoding.ASCII.GetString(receive, startIndex + 16, 4), 16);
			}
			else if (message == "0000")
			{
				num = 20 + Convert.ToInt32(Encoding.ASCII.GetString(receive, startIndex + 16, 4), 16) * 4;
			}
		}
		else if (Encoding.ASCII.GetString(receive, startIndex, 4) == "0403")
		{
			if (actualLength < startIndex + 12)
			{
				return false;
			}
			num = 12 + (Convert.ToInt32(Encoding.ASCII.GetString(receive, startIndex + 8, 2), 16) + Convert.ToInt32(Encoding.ASCII.GetString(receive, startIndex + 10, 2), 16)) * 8;
		}
		else
		{
			if (!(Encoding.ASCII.GetString(receive, startIndex, 4) == "1402"))
			{
				return true;
			}
			if (actualLength < startIndex + 12)
			{
				return false;
			}
			string message2 = Encoding.ASCII.GetString(receive, startIndex + 4, 4);
			if (message2 == "0001")
			{
				num = 10 + Convert.ToInt32(Encoding.ASCII.GetString(receive, startIndex + 8, 2), 16) * 10;
			}
			if (message2 == "0000")
			{
				num = 12 + Convert.ToInt32(Encoding.ASCII.GetString(receive, startIndex + 8, 2), 16) * 12 + Convert.ToInt32(Encoding.ASCII.GetString(receive, startIndex + 10, 2), 16) * 16;
			}
		}
		if (num < 0)
		{
			return false;
		}
		return actualLength >= startIndex + num + tail + (sumCheck ? 2 : 0);
	}
}
