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

namespace HslCommunication.Core.IMessage;

/// <summary>
/// 三菱的A兼容1E帧ASCII协议解析规则
/// </summary>
public class MelsecA1EAsciiMessage : NetMessageBase, INetMessage
{
	public int ProtocolHeadBytesLength => 4;

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
		if (base.HeadBytes[2] == 53 && base.HeadBytes[3] == 66)
		{
			return 4;
		}
		if (base.HeadBytes[2] == 48 && base.HeadBytes[3] == 48)
		{
			int num = Convert.ToInt32(Encoding.ASCII.GetString(base.SendBytes, 20, 2), 16);
			if (num == 0)
			{
				num = 256;
			}
			switch (base.HeadBytes[1])
			{
			case 48:
				return (num % 2 == 1) ? (num + 1) : num;
			case 49:
				return num * 4;
			case 50:
			case 51:
				return 0;
			default:
				return 0;
			}
		}
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
		if (base.HeadBytes != null)
		{
			return base.HeadBytes[0] - base.SendBytes[0] == 8;
		}
		return false;
	}
}
