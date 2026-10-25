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
/// 用于欧姆龙通信的Fins协议的消息解析规则
/// </summary>
public class FinsMessage : NetMessageBase, INetMessage
{
	public int ProtocolHeadBytesLength => 16;

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
		int num = BitConverter.ToInt32(new byte[4]
		{
			base.HeadBytes[7],
			base.HeadBytes[6],
			base.HeadBytes[5],
			base.HeadBytes[4]
		}, 0);
		if (num > 10000)
		{
			num = 10000;
		}
		if (num < 8)
		{
			num = 8;
		}
		return num - 8;
	}

	/// <summary>
	/// 检查头子节的合法性
	/// Check the legitimacy of the head subsection
	/// </summary>
	/// <param name="token">特殊的令牌，有些特殊消息的验证</param>
	/// <returns>是否成功的结果</returns>
	public override bool CheckHeadBytesLegal(byte[] token)
	{
		if (base.HeadBytes == null)
		{
			return true;
		}
		if (base.HeadBytes[0] == 70 && base.HeadBytes[1] == 73 && base.HeadBytes[2] == 78 && base.HeadBytes[3] == 83)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// 在接收头报文的时候，返回前置无效的报文头字节长度，默认为0，不处理
	/// When receiving a header message, return the header byte length of the invalid header, the default is 0, and no processing is performed.
	/// </summary>
	/// <param name="headByte">接收到的头子节信息</param>
	/// <returns>头子节中无效的字节长度信息</returns>
	public override int PependedUselesByteLength(byte[] headByte)
	{
		if (headByte == null)
		{
			return 0;
		}
		for (int i = 0; i < headByte.Length - 3; i++)
		{
			if (headByte[i] == 70 && headByte[i + 1] == 73 && headByte[i + 2] == 78 && headByte[i + 3] == 83)
			{
				return i;
			}
		}
		return base.PependedUselesByteLength(headByte);
	}

	public override int CheckMessageMatch(byte[] send, byte[] receive)
	{
		if (send == null || receive == null)
		{
			return 1;
		}
		if (send.Length > 25 && receive.Length > 25)
		{
			if (send[25] == receive[25])
			{
				return 1;
			}
			return -1;
		}
		return base.CheckMessageMatch(send, receive);
	}
}
