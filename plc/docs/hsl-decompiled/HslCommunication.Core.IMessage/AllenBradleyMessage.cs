// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.Core.IMessage;

/// <summary>
/// 用于和 AllenBradley PLC 交互的消息协议类
/// </summary>
public class AllenBradleyMessage : NetMessageBase, INetMessage
{
	private bool contextCheck = false;

	public int ProtocolHeadBytesLength => 24;

	/// <summary>
	/// 实例化一个默认的  对象
	/// </summary>
	public AllenBradleyMessage()
	{
	}

	/// <summary>
	/// Initializes a new instance of the  class.
	/// </summary>
	/// <param name="contextCheck">A value indicating whether the message should perform a context check. If , the message will validate its context before processing.</param>
	public AllenBradleyMessage(bool contextCheck)
	{
		this.contextCheck = contextCheck;
	}

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
		return BitConverter.ToUInt16(base.HeadBytes, 2);
	}

	public override int CheckMessageMatch(byte[] send, byte[] receive)
	{
		if (receive == null || receive.Length < 24 || send == null || send.Length < 24)
		{
			return 1;
		}
		if (contextCheck)
		{
			for (int i = 12; i < 20; i++)
			{
				if (receive[i] != send[i])
				{
					return -1;
				}
			}
		}
		return base.CheckMessageMatch(send, receive);
	}
}
