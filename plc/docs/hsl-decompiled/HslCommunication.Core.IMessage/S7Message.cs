// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Core.IMessage;

/// <summary>
/// 西门子S7协议的消息解析规则
/// </summary>
public class S7Message : NetMessageBase, INetMessage
{
	public int ProtocolHeadBytesLength => 4;

	/// <summary>
	/// 是否检查消息的合法性，默认为true，如果设置为false，则不检查消息的合法性，直接返回接收的数据内容，适用于一些特殊的协议解析需求
	/// </summary>
	public bool CheckMessage { get; set; } = true;

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
			return false;
		}
		if (base.HeadBytes[0] == 3 && base.HeadBytes[1] == 0)
		{
			return true;
		}
		return false;
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
		byte[] headBytes = base.HeadBytes;
		if (headBytes != null && headBytes.Length >= 4)
		{
			int num = base.HeadBytes[2] * 256 + base.HeadBytes[3] - 4;
			if (num < 0)
			{
				num = 0;
			}
			return num;
		}
		return 0;
	}

	public override int CheckMessageMatch(byte[] send, byte[] receive)
	{
		if (!CheckMessage)
		{
			return 1;
		}
		if (receive != null && receive.Length >= 14 && send != null && send.Length >= 14 && receive[5] == 240)
		{
			if (send[11] == receive[11] && send[12] == receive[12])
			{
				return 1;
			}
			return -1;
		}
		return base.CheckMessageMatch(send, receive);
	}
}
