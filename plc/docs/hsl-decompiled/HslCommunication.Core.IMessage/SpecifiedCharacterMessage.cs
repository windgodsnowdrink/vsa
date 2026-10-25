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
/// 专门用于接收指定字符结尾的网络消息
/// </summary>
public class SpecifiedCharacterMessage : NetMessageBase, INetMessage
{
	private int protocolHeadBytesLength = -1;

	/// <summary>
	/// 获取或设置在结束字符之后剩余的固定字节长度，有些则还包含两个字节的校验码，这时该值就需要设置为2。
	/// Gets or sets the remaining fixed byte length after the end character, and some also contain a two-byte check code. In this case, the value needs to be set to 2.
	/// </summary>
	public byte EndLength
	{
		get
		{
			return BitConverter.GetBytes(protocolHeadBytesLength)[2];
		}
		set
		{
			byte[] bytes = BitConverter.GetBytes(protocolHeadBytesLength);
			bytes[2] = value;
			protocolHeadBytesLength = BitConverter.ToInt32(bytes, 0);
		}
	}

	public int ProtocolHeadBytesLength => protocolHeadBytesLength;

	/// <summary>
	/// 使用固定的一个字符结尾作为当前的报文接收条件，来实例化一个对象
	/// Instantiate an object using a fixed end of one character as the current message reception condition
	/// </summary>
	/// <param name="endCode">结尾的字符</param>
	public SpecifiedCharacterMessage(byte endCode)
	{
		byte[] buffer = new byte[4];
		buffer[3] = (byte)(buffer[3] | 0x80);
		buffer[3] = (byte)(buffer[3] | 1);
		buffer[1] = endCode;
		protocolHeadBytesLength = BitConverter.ToInt32(buffer, 0);
	}

	/// <summary>
	/// 使用固定的两个个字符结尾作为当前的报文接收条件，来实例化一个对象
	/// Instantiate an object using a fixed two-character end as the current message reception condition
	/// </summary>
	/// <param name="endCode1">第一个结尾的字符</param>
	/// <param name="endCode2">第二个结尾的字符</param>
	public SpecifiedCharacterMessage(byte endCode1, byte endCode2)
	{
		byte[] buffer = new byte[4];
		buffer[3] = (byte)(buffer[3] | 0x80);
		buffer[3] = (byte)(buffer[3] | 2);
		buffer[1] = endCode1;
		buffer[0] = endCode2;
		protocolHeadBytesLength = BitConverter.ToInt32(buffer, 0);
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
		return 0;
	}

	/// <summary>
	/// 检查头子节的合法性
	/// Check the legitimacy of the head subsection
	/// </summary>
	/// <param name="token">特殊的令牌，有些特殊消息的验证</param>
	/// <returns>是否成功的结果</returns>
	public override bool CheckHeadBytesLegal(byte[] token)
	{
		return true;
	}
}
